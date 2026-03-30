Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On
'
' SPDX-License-Identifier: GPL-3.0-or-later
'###########################################################################
'#                                                                         #
'#   Copyright © 2025–2026 Götz Kircher <mahjonggk@t-online.de>            #
'#                                                                         #
'#                     MahjongGK  -  Mahjong Solitär                       #
'#                                                                         #
'#   This program is free software: you can redistribute it and/or modify  #
'#   it under the terms of the GNU General Public License as published by  #
'#   the Free Software Fundament, either version 3 of the License, or     #
'#   at your option any later version.                                     #
'#                                                                         #
'#   This program is distributed in the hope that it will be useful,       #
'#   but WITHOUT ANY WARRANTY; without even the implied warranty of        #
'#   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the          #
'#   GNU General Public License for more details.                          #
'#   https://www.gnu.org/licenses/gpl-3.0.html                             #
'#                                                                         #
'###########################################################################
'
'
Imports System.Xml.Serialization

#Disable Warning IDE0079
#Disable Warning IDE1006

Namespace Spielfeld

    '
    ''' <summary>
    ''' Pfad: MahjongGK/Spielfeld/Daten
    ''' 
    ''' Beschreibt einen einzelnen Spielstein im MahjongGK-Spielfeld.
    ''' Enthält Identität/Zuordnung (<c>SteinInfoIndex</c>, <c>SteinIndex</c>, <c>KlickGruppe</c>),
    ''' Lage (<c>Postion3D</c>, abgeleitete <c>X/Y/Z</c>), Sichtbarkeitsmaske (<c>Verdeckung</c>/<c>Sichtbar</c>)
    ''' sowie optionale Animationsparameter (nur Laufzeit, <c>XmlIgnore</c>).
    ''' Bietet eine typsichere <see cref="DeepCopy"/> zur Erstellung echter, referenzgetrennter Kopien.
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' <b>Identität &amp; Paarlogik:</b> <c>SteinInfoIndex</c> entspricht dem Index in <c>SteinInfos</c>.
    ''' Die Zuordnung zu Paaren erfolgt über <c>KlickGruppe</c> (Mapping aus <c>SteinIndexEnum</c>),
    ''' sodass auch visuell unterschiedliche Steine paarweise entfernt werden können.
    ''' </para>
    ''' <para>
    ''' <b>Position:</b> <c>Postion3D</c> enthält die Koordinaten im Spielfeldraster.
    ''' Die ReadOnly-Properties <c>X</c>, <c>Y</c>, <c>Z</c> leiten direkt daraus ab.
    ''' </para>
    ''' <para>
    ''' <b>Sichtbarkeit:</b> Die Oberfläche ist in 4 Quadranten unterteilt.
    ''' <c>Verdeckung</c> (Bitmaske) und <c>Sichtbar</c> sind logisch komplementär (4-Bit-Raum).
    ''' Über <c>VerdecktQuadrant</c>/<c>SichtbarQuadrant</c> lassen sich einzelne Quadranten bequem lesen/setzen.
    ''' </para>
    ''' <para>
    ''' <b>Animation (nur Laufzeit):</b> <c>AnimTyp</c>, <c>AnimShowAnimated</c>, <c>AnimStartDelay</c>,
    ''' <c>AnimMaxStep</c> (intern ×100 skaliert), <c>AnimCurStep</c>, <c>AnimLoops</c>, <c>AnimLoopCount</c>
    ''' sind mit <c>XmlIgnore</c> markiert und werden nicht serialisiert.
    ''' </para>
    ''' <para>
    ''' <b>DeepCopy:</b> <see cref="DeepCopy"/> nutzt <c>MemberwiseClone</c> für die flache Kopie und
    ''' klont Referenzen explizit (z. B. <c>Postion3D.DeepCopy()</c>). So bleiben Original und Kopie
    ''' vollständig unabhängig (Editor ⇄ Testspiel).
    ''' </para>
    ''' </remarks>
    ''' <example>
    ''' <code language="vbnet">
    ''' ' Stein erzeugen und ins Spielfeld übernehmen
    ''' Dim s As New SteinInfo(steinInfoIndex:=0,
    '''                        steinIndex:=SteinIndexEnum.Bambus1,
    '''                        pos3D:=New Triple(5, 7, 0))
    '''
    ''' ' Sichtbarkeitsbits anpassen (Quadrant-bezogen)
    ''' s.SichtbarQuadrant(Quadrant.LinksOben) = True
    ''' s.VerdecktQuadrant(Quadrant.RechtsUnten) = True
    '''
    ''' ' Laufzeit-Animation konfigurieren (wird nicht serialisiert)
    ''' s.AnimTyp = Animation.Puls
    ''' s.AnimMaxStep = 12   ' wird intern als 1200 gespeichert
    ''' s.AnimLoops = 1.5F
    '''
    ''' ' Echte tiefe Kopie anlegen (z. B. für Testmodus)
    ''' Dim copy As SteinInfo = s.DeepCopy()
    ''' copy.Postion3D.x += 2  ' verändert nur die Kopie
    ''' </code>
    ''' </example>

    <Serializable>
    Public Class SteinInfo

        <XmlIgnore>
        Private _notifyDirty As Action

        <XmlIgnore>
        Private _pos3D As Triple

        <XmlIgnore>
        Private _tmpDebug As Integer

        <XmlIgnore>
        Private _SteinInfoIndex As Integer

        <XmlIgnore>
        Private _SteinIndex As SteinIndexEnum

        <XmlIgnore>
        Private _KlickGruppe As Integer

        <XmlIgnore>
        Private _SteinStatusIst As SteinStatus

        <XmlIgnore>
        Private _SteinStatusUsed As SteinStatus

        <XmlIgnore>
        Private _IsWerkbankStein As Boolean

        <XmlIgnore>
        Private _Verdeckung As Verdeckt

        <XmlIgnore>
        Private _RectQuadrant(3) As Rectangle

        <XmlIgnore>
        Private _RectStein As Rectangle

        <XmlIgnore>
        Private _AnimTyp As Animation

        <XmlIgnore>
        Private _AnimShowAnimated As Boolean

        <XmlIgnore>
        Private _AnimStartDelay As Integer

        <XmlIgnore>
        Private _AnimCurStep As Integer

        <XmlIgnore>
        Private _AnimLoopCount As Integer

        <XmlIgnore>
        Private _AnimMaxStep As Integer

        <XmlIgnore>
        Private _AnimLoops As Single

        <XmlIgnore>
        Public Property IsDirty As Boolean

        Sub New()

        End Sub

        Sub New(steinInfoIndex As Integer, steinIndex As SteinIndexEnum, pos3D As Triple, ByRef arrFB(,,) As Integer)
            Me.SteinInfoIndex = steinInfoIndex
            Me.SteinIndex = steinIndex
            KlickGruppe = Spielfeld.GetSteinClickGruppe(steinIndex, INI.Spielbetrieb_WindsAreInOneClickGroup)
            SteinStatusIst = SteinStatus.I01Normal
            SteinStatusUsed = SteinStatus.I01Normal
            Me.Pos3D = pos3D
            _arrFB = arrFB
        End Sub
        '
        ''' <summary>
        ''' Referenz auf den arrFB. Wird benötigt um das dort gespeicherte
        ''' Flag IsRemoved zu ändern oder auszulesen.
        ''' </summary>
        Private _arrFB(,,) As Integer
        '
        ''' <summary>
        ''' Wird benötigt um die Referenz nach dem Laden wieder herzustellen.
        ''' </summary>
        ''' <param name="arrFB"></param>
        Public Sub SetArrFbReferenz(ByRef arrFB(,,) As Integer)
            _arrFB = arrFB
        End Sub

        Private Sub MarkDirty()
            IsDirty = True
        End Sub

        Private Sub Pos3D_Changed(sender As Triple)
            ' Jede Änderung in Triple (x/y/z/Valide) zählt als "dirty"
            MarkDirty()
        End Sub
        '
        '
        '
        ''' <summary>
        ''' Das Flag von IsRemoved wird im arrFB gespeichert, weil so ein zeitkritisch
        ''' schneller Zugriff möglich ist.
        ''' Im Spielbetrieb werden keine Steine entnommen, sondern ausschließlich
        ''' als IsRemoved gekennzeichnet. Das ist programmiertechnich deutlich
        ''' einfacher zu handhaben.
        ''' Im Editierbetrieb werden die Steininfo entnommener Steine wieder
        ''' entfernt, die ArrFB und weitere Listen aktualisiert bzw. neu aufgebaut.
        ''' Durch die Speicherung im arrFB bleibt der Status von IsRemoved automatisch erhalten.
        ''' </summary>
        ''' <returns></returns>
        Public Property IsRemoved() As Boolean
            Get
                'Weil zeitkritisch doppelter Code
                Dim fb As Integer = _arrFB(X, Y, Z)
                Dim offsetX As Integer = If((fb And FLAG_XOffset) <> 0, 1, 0)
                Dim offsetY As Integer = If((fb And FLAG_YOffset) <> 0, 1, 0)
                Return (_arrFB(X - offsetX, Y - offsetY, Z) And FLAG_IsRemoved) <> 0
            End Get
            Set(value As Boolean)
                'Weil zeitkritisch doppelter Code
                Dim fb As Integer = _arrFB(X, Y, Z)
                Dim offsetX As Integer = If((fb And FLAG_XOffset) <> 0, 1, 0)
                Dim offsetY As Integer = If((fb And FLAG_YOffset) <> 0, 1, 0)
                If value Then
                    _arrFB(X - offsetX, Y - offsetY, Z) = _arrFB(X - offsetX, Y - offsetY, Z) Or FLAG_IsRemoved
                Else
                    _arrFB(X - offsetX, Y - offsetY, Z) = _arrFB(X - offsetX, Y - offsetY, Z) And Not FLAG_IsRemoved
                End If
            End Set
        End Property

        '
        ''' <summary>
        ''' Die Reihenfolge, in der die Steine entfernt wurden.
        ''' </summary>
        ''' <returns></returns>
        Public Property IsRemovedIndex As Integer

        ''' <summary>
        ''' Identisch des Index in SteinInfos
        ''' Wird beim Plazieren im Feld in SetStein vergeben.
        ''' (Das ist die tiefst mögliche Ebene den Index zu vergeben und die Sicherste zudem.
        ''' Nur wenn Steine entfernt werden, müssen die Index neu vergeben werden
        ''' weil dann arrFB und SteinInfos nicht mehr syncron laufen
        ''' </summary>
        <XmlElement("Idx")>
        Public Property SteinInfoIndex As Integer
            Get
                Return _SteinInfoIndex
            End Get
            Set(value As Integer)
                If _SteinInfoIndex = value Then Return
                _SteinInfoIndex = value
                MarkDirty()
            End Set
        End Property
        '
        ''' <summary>
        ''' Eine Enumeration aller 43 verschiedenen Grafiken für die Steine.
        ''' </summary>
        <XmlElement("SIdx")>
        Public Property SteinIndex As SteinIndexEnum
            Get
                Return _SteinIndex
            End Get
            Set(value As SteinIndexEnum)
                If _SteinIndex = value Then Return
                _SteinIndex = value
                MarkDirty()
            End Set
        End Property
        '
        ''' <summary>
        ''' Steine können immer nur paarweise entfernt werden. Meist gleich aussehende Steine,
        ''' also Steine mit gleicher Enum Stein. 
        ''' Es gibt aber Steine, die verschieden aussehen, aber paarweise entfernt werden können.
        ''' Diese Steine gehören zur gleichen Klickgruppe, obwohl sie verschiedene Enum Stein haben.
        ''' Es gibt eine Übersetzungstabelle Enum Stein -> Klickgruppe.
        ''' </summary>
        <XmlElement("KGrp")>
        Public Property KlickGruppe As Integer
            Get
                Return _KlickGruppe
            End Get
            Set(value As Integer)
                If _KlickGruppe = value Then Return
                _KlickGruppe = value
                MarkDirty()
            End Set
        End Property
        '
        ''' <summary>
        ''' Das ist der tatsächliche Steinstatus, den der Stein hat.
        ''' </summary>
        <XmlElement("StStIst")>
        Public Property SteinStatusIst As SteinStatus
            Get
                Return _SteinStatusIst
            End Get
            Set(value As SteinStatus)
                If _SteinStatusIst = value Then Return
                _SteinStatusIst = value
                MarkDirty()
            End Set
        End Property
        '
        ''' <summary>
        ''' Das ist der Steinstatus, der zur Renderung ausgewertet wird. Er kann sich vom
        ''' SteinStatusReal unterscheiden, wenn der Spieler z.B. nicht wissen will, welche
        ''' Steine klickbar sind, oder nicht sehen soll, welche Steinpaare entnommen werden
        ''' können.
        ''' </summary>
        <XmlElement("StStUsed")>
        Public Property SteinStatusUsed As SteinStatus
            Get
                Return _SteinStatusUsed
            End Get
            Set(value As SteinStatus)
                If _SteinStatusUsed = value Then Return
                _SteinStatusUsed = value
                MarkDirty()
            End Set
        End Property
        '
        ''' <summary>
        ''' Die IsWerkstattStein setzt dieses Flag als Kennzeichen,
        ''' das der SteinIndexEnum noch nicht zugewiesen wurde.
        ''' Wird sicherheitshalber in der Xml mit gespeichert.
        ''' </summary>
        ''' <returns></returns>
        <XmlElement("IsWbSt")>
        Public Property IsWerkbankStein As Boolean
            Get
                Return _IsWerkbankStein
            End Get
            Set(value As Boolean)
                If _IsWerkbankStein = value Then Return
                _IsWerkbankStein = value
                MarkDirty()
            End Set
        End Property
        '
        ''' <summary>
        ''' Die 3D-Position des Steines im Raum auf den Spielfeld.
        ''' </summary>
        <XmlElement("Pos3D")>
        Public Property Pos3D As Triple
            Get
                Return _pos3D
            End Get
            Set(value As Triple)
                If Not value?.IsEqual(_pos3D) Then
                    _pos3D = value
                    MarkDirty()
                End If
            End Set
        End Property

        ''' <summary>
        ''' Die Pos3D-Angabe bezieht sich immer auf den Quadranten LinksOben.
        ''' Hier erfolgt die Rückgabe der 3D-Position des Quadranten
        ''' </summary>
        ''' <param name="quadrant"></param>
        ''' <returns></returns>
        Public Function Pos3DQuadrant(quadrant As Quadrant) As Triple
            If IsNothing(_pos3D) Then
                Return Nothing
            Else
                Select Case quadrant
                    Case Quadrant.LO
                        Return _pos3D
                    Case Quadrant.RO
                        Return New Triple(_pos3D, addX:=1)
                    Case Quadrant.LU
                        Return New Triple(_pos3D, addY:=1)
                    Case Quadrant.RU
                        Return New Triple(_pos3D, addX:=1, addY:=1)
                    Case Else 'Quadrant.None
                        Return Nothing
                End Select
            End If
        End Function
        '
        ''' <summary>
        ''' Die X-Koordinate des Steines im Raum auf dem Spielfeld
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property X As Integer
            Get
                If _pos3D Is Nothing Then Return 0
                Return _pos3D.x
            End Get
        End Property
        '
        ''' <summary>
        ''' Die Y-Koordinate des Steines im Raum auf dem Spielfeld
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Y As Integer
            Get
                If _pos3D Is Nothing Then Return 0
                Return _pos3D.y
            End Get
        End Property
        '
        ''' <summary>
        ''' Die Z-Koordinate des Steines im Raum auf dem Spielfeld
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Z As Integer
            Get
                If _pos3D Is Nothing Then Return 0
                Return _pos3D.z
            End Get
        End Property
        '
        ''' <summary>
        ''' Die aktuelle Ausgabeposition des Steines
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property RectStein As Rectangle
            Get
                Return _RectStein
            End Get
        End Property
        '
        ''' <summary>
        ''' Die aktuelle Lage der vier Quadraten
        ''' </summary>
        ''' <param name="q"></param>
        ''' <returns></returns>
        Public ReadOnly Property RectQuadrant(q As Quadrant) As Rectangle
            Get
                Return _RectQuadrant(q)
            End Get
        End Property

        ''' <summary>
        ''' Eine Enumeration, die den Typ der Animation festlegt.
        ''' </summary>
        <XmlIgnore>
        Public Property AnimTyp As Animation
            Get
                Return _AnimTyp
            End Get
            Set(value As Animation)
                If _AnimTyp = value Then Return
                _AnimTyp = value
                MarkDirty()
            End Set
        End Property
        '
        ''' <summary>
        ''' Wenn dieses Flag gesetzt ist, verzweigt die Ausgabe beim nächsten Paint-Event
        ''' in die Animation.
        ''' </summary>
        <XmlIgnore>
        Public Property AnimShowAnimated As Boolean
            Get
                Return _AnimShowAnimated
            End Get
            Set(value As Boolean)
                If _AnimShowAnimated = value Then Return
                _AnimShowAnimated = value
                MarkDirty()
            End Set
        End Property
        '
        ''' <summary>
        ''' Wenn der Wert größer 0 ist, werden nach dem Start der Animation diese nicht wirklich
        ''' gestartet, sondern StartVerzögerung wird nach jedem Paint-Event rückwärts gezählt.
        ''' Erst wenn es 0 ist, wird die Animation gestartet.
        ''' </summary>
        <XmlIgnore>
        Public Property AnimStartDelay As Integer
            Get
                Return _AnimStartDelay
            End Get
            Set(value As Integer)
                If _AnimStartDelay = value Then Return
                _AnimStartDelay = value
                MarkDirty()
            End Set
        End Property
        '
        '
        ''' <summary>
        ''' Gibt an, wieviele Schritte zur Verfügung stehen um einen Loop der Animation ablaufen
        ''' zu lassen. Es ist nicht die Anzahl der Schritte, die auch ausgeführt werden.
        ''' Normalerweise wird die Animation mit Step 100 in der Schritterhöhung durchlaufen.
        ''' Doch diese Schrittweite wird bei jedem Paint-Event aus dem Vergleich der Soll
        ''' mit der Ist-Zeit-Differenz zum letztem Paint-Event verglichen und angepasst,
        ''' um einen gleichmäßigen Ablauf der Animation zu erreichen.
        ''' (Prinzip: kommt das Event 10% später als die Sollzeit, wird die Schrittweite um 10%
        ''' erhöht, die Animation also in einem Zustand etwas später angezeigt.)
        ''' Die Zuweisung erfolgt daher mit dem Faktor 100.
        ''' </summary>
        <XmlIgnore>
        Public Property AnimMaxStep As Integer
            Get
                Return _AnimMaxStep
            End Get
            Set(value As Integer)
                Dim scaled As Integer = value * 100
                If _AnimMaxStep = scaled Then Return
                _AnimMaxStep = scaled
                MarkDirty()
            End Set
        End Property
        '
        ''' <summary>
        ''' Siehe AnimationMaxStep
        ''' </summary>
        <XmlIgnore>
        Public Property AnimCurStep As Integer
            Get
                Return _AnimCurStep
            End Get
            Set(value As Integer)
                If _AnimCurStep = value Then Return
                _AnimCurStep = value
                MarkDirty()
            End Set
        End Property
        '        '
        '
        ''' <summary>
        ''' Anzahl der Wiederholungen der Animationen in Folge.
        ''' Gleitkommazahl da die Loops nicht vollständig sein müssen.
        ''' </summary>
        <XmlIgnore>
        Public Property AnimLoops As Single
            Get
                Return _AnimLoops
            End Get
            Set(value As Single)
                _AnimLoops = value
                MarkDirty()
                'TODO SFD-Anpassung 
                'Berechnung von AnimationMaxStepLastLoop aus AnimationMaxStep
                'und setzten des AnimationLoopCounter
            End Set
        End Property
        '
        ''' <summary>
        ''' Ein Rückwärtszähler. Wenn er 0 ist, ist die Animation fertig.
        ''' Wird erst abgefragt nachdem eine Animation ausgeführt wurde,
        ''' d.h. eine vollständige Animation wird immer durchgeführt.
        ''' Deshalb ist es nur nötig den Counter > 0 zu setzen, wenn mehr als eine
        ''' Runde abgearbeitet werden soll. Das geschieht automatisch wenn 
        ''' AnimLoops > 0 gesetzt wird.
        ''' </summary>
        <XmlIgnore>
        Public Property AnimLoopCount As Integer
            Get
                Return _AnimLoopCount
            End Get
            Set(value As Integer)
                If _AnimLoopCount = value Then Return
                _AnimLoopCount = value
                MarkDirty()
            End Set
        End Property

#Region "Sichtbarkeit"

        'alter Ansatz zlv (zum löschen vorgesehen)
        ''
        ''        ''' <summary>
        ''        ''' Die Steinoberfläche teilt sich in 4 Quadranten auf (Siehe Enumeration Quadrant),
        ''        ''' die sichtbar sein können oder mehr oder minder verdeckt sind von anderen Steinen.
        ''        ''' Was sichtbar ist und was nicht, sagen die Enumerationen Verdeckt und Sichtbar.
        ''        ''' Die beiden Enumerationen und die Enumeration Quadrant sind logisch miteinander
        ''        ''' verbunden, sodass Sichtbar und Verdeckt im Prinzip zwei Sichten auf denselben Bitwert
        ''        ''' sind, und dass gleichzeitig noch bequem einzelne Quadranten abgefragt und gesetzt werden
        ''        ''' können. Welche der drei Properties genutzt wird, ist daher völlig egal. Auch für eine
        ''        ''' Mischnutzung gibt es keinerlei Einschränkungen. 
        ''        ''' </summary>
        ''        <XmlIgnore>
        ''        Public Property Verdeckung As Verdeckt
        ''            Get
        ''                Return _Verdeckung
        ''            End Get
        ''            Set(value As Verdeckt)
        ''                If _Verdeckung = value Then Return
        ''                _Verdeckung = value
        ''                MarkDirty()
        ''            End Set
        ''        End Property
        ''        '
        ''        ' Sichtbar = logisches NOT (im Bitbereich) von Verdeckt, bei 4 relevanten Bits
        ''        <XmlIgnore>
        ''        Public Property Sichtbar() As Sichtbar
        ''            Get
        ''                ' 15 (1111) XOR Verdeckt ergibt invertierte Bits im Bereich 0..15
        ''                Return CType(15 Xor CInt(Verdeckung), Sichtbar)
        ''            End Get
        ''            Set(value As Sichtbar)
        ''                ' Setter läuft über Verdeckung => dirty kommt dort
        ''                ' Umgekehrt: Verdeckt ist invertiert zu Sichtbar
        ''                Verdeckung = CType(15 Xor CInt(value), Verdeckt)
        ''            End Set
        ''        End Property

        ''        ''' <summary>
        ''        ''' Zugriff auf ein einzelnes Verdeckt-Bit. 
        ''        ''' Die Oberfläche eines jeden Steines hat 4 Quadranten. Jeder einzelne kann sichtbar
        ''        ''' oder verdeckt sein. Die Feststellung, ob ein Quadrant sichtbar ist oder nicht, 
        ''        ''' geht ganz einfach. Der arrFB(x,y,z) wird über drei ineinander verschachtelte 
        ''        ''' For-Next-Schleifen durchlaufen. Die innerste Schleife ist die z-Schleife.
        ''        ''' Sie durchsucht das Feld von oben nach unten, also rückwärts.
        ''        ''' Das erste Feld, das ungleich 0 ist, und nicht den SteinStatus Unsichtbar hat,
        ''        ''' gehört zu einem Quadranten, der sichtbar ist.
        ''        ''' Alle darunter liegenden Quadranten sind unsichtbar.
        ''        ''' Jetzt muss nur noch herausgefunden werden, zu welchem Quadranten die x-y-z-Position
        ''        ''' gehört. Auch das ist einfach und bei der Beschreibung der Feldbeschreibers arrFB
        ''        ''' beschrieben, wo OffsetX und OffsetY beschrieben sind, über die das geht,
        ''        ''' über die man an den Index herankommt und damit an die SteinInfo des Steines.
        ''        ''' Siehe Spielfeld\liesmich.txt
        ''        ''' </summary>
        ''        ''' <param name="quadrant"></param>
        ''        ''' <returns></returns>
        ''        <XmlIgnore>
        ''        Public Property VerdecktQuadrant(quadrant As Quadrant) As Boolean
        ''            Get
        ''                Return (CInt(Verdeckung) And CInt(quadrant)) <> 0
        ''            End Get
        ''            Set(value As Boolean)
        ''                Dim mask As Integer = CInt(quadrant)
        ''                Dim v As Integer = CInt(Verdeckung)
        ''                Dim newV As Integer

        ''                If value Then
        ''                    newV = v Or mask 'Bit setzen
        ''                Else
        ''                    newV = v And Not mask 'Bit löschen
        ''                End If

        ''                If newV = v Then Return
        ''                Verdeckung = CType(newV, Verdeckt) ' dirty kommt im Verdeckung-Setter
        ''            End Set
        ''        End Property

        ''        ' Zugriff auf ein einzelnes Sichtbar-Bit (logische Umkehr von Verdeckt)
        ''        <XmlIgnore>
        ''        Public Property SichtbarQuadrant(quadrant As Quadrant) As Boolean
        ''            Get
        ''                ' Sichtbar bedeutet: das Verdeckt-Bit ist 0
        ''                Return (CInt(Verdeckung) And CInt(quadrant)) = 0
        ''            End Get
        ''            Set(value As Boolean)
        ''                Dim mask As Integer = CInt(quadrant)
        ''                Dim v As Integer = CInt(Verdeckung)
        ''                Dim newV As Integer
        ''                If value Then
        ''                    ' Sichtbar => Verdeckt-Bit löschen
        ''                    newV = v And Not mask
        ''                Else
        ''                    ' Nicht sichtbar => Verdeckt-Bit setzen
        ''                    newV = v Or mask
        ''                End If
        ''                If newV = v Then Return
        ''                Verdeckung = CType(newV, Verdeckt) ' dirty kommt im Verdeckung-Setter
        ''            End Set
        ''        End Property
        ''        '
        ''        ''' <summary>
        ''        ''' Aus Gründen der Programmlesbarkeit kann es hilfreich sein, die
        ''        ''' Enumeration Sichtbar in Verdeckt zu konvertieren.
        ''        ''' </summary>
        ''        ''' <param name="sichtbar"></param>
        ''        ''' <returns></returns>
        ''        Public Shared Function CvtSichtbarToVerdeckt(sichtbar As Sichtbar) As Verdeckt
        ''            Return CType(15 Xor CInt(sichtbar), Verdeckt)
        ''        End Function
        ''        '
        ''        ''' <summary>
        ''        ''' Aus Gründen der Programmlesbarkeit kann es hilfreich sein, die
        ''        ''' Enumeration Verdeckt in Sichtbar zu konvertieren.
        ''        ''' </summary>
        ''        ''' <param name="verdeckt"></param>
        ''        ''' <returns></returns>
        ''        Public Shared Function CvtVerdecktToSichtbar(verdeckt As Verdeckt) As Sichtbar
        ''            Return CType(15 Xor CInt(verdeckt), Sichtbar)
        ''        End Function

        ''        'Hier die vollständige Abfrage:
        ''        '
        ''        ''' <summary>
        ''        ''' Gibt True zurück, wenn mindestens einer der Quadranten sichtbar ist
        ''        ''' und auf einen der Quadranten geklickt wurde.
        ''        ''' </summary>
        ''        ''' <param name="ptMousePos"></param>
        ''        ''' <returns></returns>
        ''        Public Function RectSteinPartClicked(ptMousePos As Point) As Boolean

        ''            ' 15 (1111) XOR Verdeckt ergibt invertierte Bits im Bereich 0..15
        ''            ' dreht also die Verdeckung zur Sichtbarkeit
        ''            Dim q As Integer = 15 Xor CInt(_Verdeckung)

        ''            If (q And Quadrant.LO) <> 0 Then
        ''                If _RectQuadrant(0).Contains(ptMousePos) Then
        ''                    Return True
        ''                End If
        ''            End If

        ''            If (q And Quadrant.RO) <> 0 Then
        ''                If _RectQuadrant(1).Contains(ptMousePos) Then
        ''                    Return True
        ''                End If
        ''            End If

        ''            If (q And Quadrant.LU) <> 0 Then
        ''                If _RectQuadrant(2).Contains(ptMousePos) Then
        ''                    Return True
        ''                End If
        ''            End If

        ''            If (q And Quadrant.RU) <> 0 Then
        ''                If _RectQuadrant(3).Contains(ptMousePos) Then
        ''                    Return True
        ''                End If
        ''            End If

        ''            Return False

        ''        End Function

        ''        ''' <summary>
        ''        ''' Funktion nur für den Editor.
        ''        ''' Prüft, ob der unter der Mausposition liegende Quadrant
        ''        ''' sichtbar ist.
        ''        ''' Verwendung: zum Hinzufügen von Steinen.
        ''        ''' </summary>
        ''        ''' <param name="ptMousePos"></param>
        ''        ''' <returns></returns>
        ''        Public Function SteinQuadrantVisible(ptMousePos As Point) As Quadrant

        ''            ' 15 (1111) XOR Verdeckt ergibt invertierte Bits im Bereich 0..15
        ''            ' dreht also die Verdeckung zur Sichtbarkeit
        ''            Dim q As Integer = 15 Xor CInt(_Verdeckung)

        ''            If _RectQuadrant(0).Contains(ptMousePos) Then Return Quadrant.LO
        ''            If _RectQuadrant(1).Contains(ptMousePos) Then Return Quadrant.RO
        ''            If _RectQuadrant(2).Contains(ptMousePos) Then Return Quadrant.LU
        ''            If _RectQuadrant(3).Contains(ptMousePos) Then Return Quadrant.RU

        ''            Return Quadrant.none

        ''        End Function
        ''        '
        ''        ''' <summary>
        ''        ''' Funktion nur für den Editor.
        ''        ''' Prüft, ob der unter der Mausposition liegende Stein
        ''        ''' VOLLLSTÄNDIG sichtbar ist.
        ''        ''' Verwendung: zum Entnehmen von Steinen.
        ''        ''' </summary>
        ''        ''' <param name="ptMousePos"></param>
        ''        ''' <returns></returns>
        ''        Public Function SteinVisible(ptMousePos As Point) As Boolean

        ''            ' 15 (1111) XOR Verdeckt ergibt invertierte Bits im Bereich 0..15
        ''            ' dreht also die Verdeckung zur Sichtbarkeit
        ''            Dim q As Integer = 15 Xor CInt(_Verdeckung)

        ''            ' Wenn nicht alle vier Bits gesetzt sind,
        ''            ' ist der Stein nicht vollständig sichtbar.
        ''            If q <> 15 Then
        ''                Return False
        ''            End If

        ''            If _RectQuadrant(0).Contains(ptMousePos) Then Return True
        ''            If _RectQuadrant(1).Contains(ptMousePos) Then Return True
        ''            If _RectQuadrant(2).Contains(ptMousePos) Then Return True
        ''            If _RectQuadrant(3).Contains(ptMousePos) Then Return True

        ''            Return False

        ''        End Function

        ''' <summary>
        ''' Prüft ob die übergebene Mausposition auf einem der Quadranten liegt.
        ''' Wenn ja, ob der Quadrant als Fundament taugt.
        ''' Gibt ein Triple zurück mit ValidePlaceEnum.Yes oder ValidePlaceEnum.NoFundamentKandidat
        ''' </summary>
        ''' <param name="mousePos"></param>
        ''' <param name="arrFB"></param>
        ''' <returns></returns>
        Public Function IsFundamentPartKandidat(mousePos As Point, arrFB(,,) As Integer) As Triple

            If _pos3D.z = arrFB.GetUpperBound(2) Then
                'Steine der obersten Lage können nie Fundament sein.
                Return New Triple(ValidePlace.NoKandidat)
            End If

            With _pos3D
                If _RectQuadrant(0).Contains(mousePos) Then
                    If arrFB(.x, .y, .z + 1) = 0 Then
                        Return New Triple(.x, .y, .z + 1, ValidePlace.Yes)
                    End If
                End If
                If _RectQuadrant(1).Contains(mousePos) Then
                    If arrFB(.x + 1, .y, .z + 1) = 0 Then
                        Return New Triple(.x + 1, .y, .z + 1, ValidePlace.Yes)
                    End If
                End If
                If _RectQuadrant(2).Contains(mousePos) Then
                    If arrFB(.x, .y + 1, .z + 1) = 0 Then
                        Return New Triple(.x, .y + 1, .z + 1, ValidePlace.Yes)
                    End If
                End If
                If _RectQuadrant(3).Contains(mousePos) Then
                    If arrFB(.x + 1, .y + 1, .z + 1) = 0 Then
                        Return New Triple(.x + 1, .y + 1, .z + 1, ValidePlace.Yes)
                    End If
                End If
            End With

            Return New Triple(ValidePlace.NoKandidat)

        End Function
        '

        Public Function IsTopQuadrant(mousePos As Point, arrFB(,,) As Integer, sfd As SFDaten) As TripleX

            With _pos3D
                If _RectQuadrant(0).Contains(mousePos) Then
                    If .z = sfd.SFInf.zMax OrElse arrFB(.x, .y, .z + 1) = 0 Then
                        Return New TripleX(.x, .y, .z, ValidePlace.Yes, SteinInfoIndex, SteinIndex, Quadrant.LO, Me)
                    End If
                End If
                If _RectQuadrant(1).Contains(mousePos) Then
                    If .z = sfd.SFInf.zMax OrElse arrFB(.x + 1, .y, .z + 1) = 0 Then
                        Return New TripleX(.x + 1, .y, .z, ValidePlace.Yes, SteinInfoIndex, SteinIndex, Quadrant.RO, Me)
                    End If
                End If
                If _RectQuadrant(2).Contains(mousePos) Then
                    If .z = sfd.SFInf.zMax OrElse arrFB(.x, .y + 1, .z + 1) = 0 Then
                        Return New TripleX(.x, .y + 1, .z, ValidePlace.Yes, SteinInfoIndex, SteinIndex, Quadrant.LU, Me)
                    End If
                End If
                If _RectQuadrant(3).Contains(mousePos) Then
                    If .z = sfd.SFInf.zMax OrElse arrFB(.x + 1, .y + 1, .z + 1) = 0 Then
                        Return New TripleX(.x + 1, .y + 1, .z, ValidePlace.Yes, SteinInfoIndex, SteinIndex, Quadrant.RU, Me)
                    End If
                End If
            End With

            Return New TripleX(ValidePlace.NoKandidat)

        End Function

        Public Function IsTopQuadrant(tpl As TripleX, arrFB(,,) As Integer, sfd As SFDaten) As Boolean
            With tpl
                If .z = sfd.SFInf.zMax OrElse arrFB(.x, .y, .z + 1) = 0 Then
                    Return True
                Else
                    Return False
                End If
            End With
        End Function

        Public Function IsTopQuadrant(x As Integer, y As Integer, z As Integer, arrFB(,,) As Integer, sfd As SFDaten) As Boolean
            If z = sfd.SFInf.zMax OrElse arrFB(x, y, z + 1) = 0 Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function IsRemovingPartKandidat(mousePos As Point, arrFB(,,) As Integer) As Triple

            Dim isTop As Boolean = _pos3D.z = arrFB.GetUpperBound(2)

            With _pos3D
                If _RectQuadrant(0).Contains(mousePos) Then
                    If isTop Then
                        Return New Triple(.x, .y, .z, ValidePlace.Yes)
                    ElseIf arrFB(.x, .y, .z + 1) = 0 Then
                        Return New Triple(.x, .y, .z, ValidePlace.Yes)
                    End If
                End If
                If _RectQuadrant(1).Contains(mousePos) Then
                    If isTop Then
                        Return New Triple(.x + 1, .y, .z, ValidePlace.Yes)
                    ElseIf arrFB(.x + 1, .y, .z + 1) = 0 Then
                        Return New Triple(.x + 1, .y, .z, ValidePlace.Yes)
                    End If
                End If
                If _RectQuadrant(2).Contains(mousePos) Then
                    If isTop Then
                        Return New Triple(.x, .y + 1, .z, ValidePlace.Yes)
                    ElseIf arrFB(.x, .y + 1, .z + 1) = 0 Then
                        Return New Triple(.x, .y + 1, .z, ValidePlace.Yes)
                    End If
                End If
                If _RectQuadrant(3).Contains(mousePos) Then
                    If isTop Then
                        Return New Triple(.x + 1, .y + 1, .z, ValidePlace.Yes)
                    ElseIf arrFB(.x + 1, .y + 1, .z + 1) = 0 Then
                        Return New Triple(.x + 1, .y + 1, .z, ValidePlace.Yes)
                    End If
                End If
            End With

            Return New Triple(ValidePlace.NoKandidat)

        End Function

        Public Sub Update_RectQuadranten(sfd As SFDaten)

            Dim left As Integer = sfd.SFInf.GetSteinRenderLeft(_pos3D.x, _pos3D.z)
            Dim top As Integer = sfd.SFInf.GetSteinRenderTop(_pos3D.y, _pos3D.z)
            With sfd.SFLay
                'ganzer Stein
                _RectStein = New Rectangle(left, top, .steinWidth, .steinHeight)

                'Links oben
                _RectQuadrant(0) = New Rectangle(left, top, .steinWidthHalf, .steinHeightHalf)
                '
                'Rechts oben
                _RectQuadrant(1) = New Rectangle(left + .steinWidthHalf, top, .steinWidthHalf, .steinHeightHalf)

                'Links unten
                _RectQuadrant(2) = New Rectangle(left, top + .steinHeightHalf, .steinWidthHalf, .steinHeightHalf)

                'Rechts unten
                _RectQuadrant(3) = New Rectangle(left + .steinWidthHalf, top + .steinHeightHalf, .steinWidthHalf, .steinHeightHalf)
            End With
        End Sub
#End Region

        ''' <summary>
        ''' Erstellt eine echte tiefe Kopie von SteinInfo.
        ''' Alle Wertetypen/Enums sind ohnehin kopiert; Referenzen werden manuell geklont.
        ''' </summary>
        Public Function DeepCopy() As SteinInfo

            ' Flache Kopie aller Felder (inkl. privater Backing Fields wie _AnimMaxStep, _AnimLoops)
            Dim c As SteinInfo = DirectCast(Me.MemberwiseClone(), SteinInfo)

            ' Tiefer Kopieren aller Referenztypen/Felder:
            ' Triple ist eine Klasse → explizit klonen
            ' Triple ist Klasse -> tief kopieren, aber: Ereignisse werden später durch Container gesetzt
            If Me.Pos3D IsNot Nothing Then
                c.Pos3D = Me.Pos3D.DeepCopy
            End If

            ' Falls später weitere Referenztypen hinzukommen, hier analog tief kopieren.

            Return c

            'Antwort von ChatGPT, ob man das über Serialisierung/Deserialisierung
            'lösen könne:

            'Warum das besser ist als (De-)Serialisierung:
            '<XmlIgnore>-Felder würden bei XML-Serialisierung fehlen → unvollständige Kopie.
            'Keine Attribute / Type - Resolver nötig, kein Overhead.
            'Setter-Logiken(wie AnimMaxStep mit ×100) werden nicht versehentlich erneut ausgeführt
            '— die privaten Backing-Felder werden 1:1 kopiert, genau wie gewünscht.
            'Da Triple bereits eine saubere DeepCopy liefert, reicht das oben völlig aus.
            '
            'Die frühere Methode über den BinaryFormatter ist von Microsoft als depreceted eingestuft.
            '
            'Es ginge noch über den DataContractSerializer, das ist aber mit Vergabe vieler Attribute
            'versehen.
        End Function

        Public Function IsEmpty() As Boolean
            If _pos3D Is Nothing Then Return True
            Return Pos3D.x = 0
        End Function

    End Class
End Namespace
