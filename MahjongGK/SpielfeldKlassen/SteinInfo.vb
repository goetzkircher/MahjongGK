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
'#   the Free Software Foundation, either version 3 of the License, or     #
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
Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

#Disable Warning IDE0079
#Disable Warning IDE1006

Imports System.Xml.Serialization
'
'
''' <summary>
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

    Sub New()

    End Sub

    Sub New(steinInfoIndex As Integer, steinIndex As SteinIndexEnum, pos3D As Triple, Optional tmpDebug As Integer = 0)
        Me.SteinInfoIndex = steinInfoIndex
        Me.SteinIndex = steinIndex
        KlickGruppe = Spielfeld.GetSteinClickGruppe(steinIndex, INI.Spielbetrieb_WindsAreInOneClickGroup)
        SteinStatusIst = SteinStatus.I01Normal
        SteinStatusUsed = SteinStatus.I01Normal
        Postion3D = pos3D
        Me.tmpDebug = tmpDebug
    End Sub
    '
    ''' <summary>
    ''' wird temporär zum Debuggen genutzt.
    ''' </summary>
    ''' <returns></returns>
    <XmlIgnore>
    Public Property tmpDebug As Integer

    ''' <summary>
    ''' Identisch des Index in SteinInfos
    ''' Wird beim Plazieren im Feld in SetStein vergeben.
    ''' (Das ist die tiefst mögliche Ebene den Index zu vergeben und die Sicherste zudem.
    ''' Nur wenn Steine entfernt werden, müssen die Index neu vergeben werden
    ''' weil dann arrFB und SteinInfos nicht mehr syncron laufen
    ''' </summary>
    Public Property SteinInfoIndex As Integer
    '
    ''' <summary>
    ''' Eine Enumeration aller 43 verschiedenen Grafiken für die Steine.
    ''' </summary>
    Public Property SteinIndex As SteinIndexEnum
    '
    ''' <summary>
    ''' Steine können immer nur paarweise entfernt werden. Meist gleich aussehende Steine,
    ''' also Steine mit gleicher Enum Stein. 
    ''' Es gibt aber Steine, die verschieden aussehen, aber paarweise entfernt werden können.
    ''' Diese Steine gehören zur gleichen Klickgruppe, obwohl sie verschiedene Enum Stein haben.
    ''' Es gibt eine Übersetzungstabelle Enum Stein -> Klickgruppe.
    ''' </summary>
    Public Property KlickGruppe As Integer
    '
    ''' <summary>
    ''' Das ist der tatsächliche Steinstatus, den der Stein hat.
    ''' </summary>
    Public Property SteinStatusIst As SteinStatus
    '
    ''' <summary>
    ''' Das ist der Steinstatus, der zur Renderung ausgewertet wird. Er kann sich vom
    ''' SteinStatusReal unterscheiden, wenn der Spieler z.B. nicht wissen will, welche
    ''' Steine klickbar sind, oder nicht sehen soll, welche Steinpaare entnommen werden
    ''' können.
    ''' </summary>
    Public Property SteinStatusUsed As SteinStatus
    '
    ''' <summary>
    ''' Die IsWerkstattStein setzt dieses Flag als Kennzeichen,
    ''' das der SteinIndexEnum noch nicht zugewiesen wurde.
    ''' Wird sicherheitshalber in der Xml mit gespeichert.
    ''' </summary>
    ''' <returns></returns>
    Public Property IsWerkbankStein As Boolean
    '
    ''' <summary>
    ''' Die 3D-Position des Steines im Raum auf den Spielfeld.
    ''' </summary>
    Public Property Postion3D As Triple
    '
    ''' <summary>
    ''' Die X-Koordinate des Steines im Raum auf dem Spielfeld
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property X As Integer
        Get
            Return Postion3D.x
        End Get
    End Property
    '
    ''' <summary>
    ''' Die Y-Koordinate des Steines im Raum auf dem Spielfeld
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property Y As Integer
        Get
            Return Postion3D.y
        End Get
    End Property
    '
    ''' <summary>
    ''' Die Z-Koordinate des Steines im Raum auf dem Spielfeld
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property Z As Integer
        Get
            Return Postion3D.z
        End Get
    End Property

    '
    ''' <summary>
    ''' Die Steinoberfläche teilt sich in 4 Quadranten auf (Siehe Enumeration Quadrant),
    ''' die sichtbar sein können oder mehr oder minder verdeckt sind von anderen Steinen.
    ''' Was sichtbar ist und was nicht, sagen die Enumerationen Verdeckt und Sichtbar.
    ''' Die beiden Enumerationen und die Enumeration Quadrant sind logisch miteinander
    ''' verbunden, sodass Sichtbar und Verdeckt im Prinzip zwei Sichten auf denselben Bitwert
    ''' sind, und dass gleichzeitig noch bequem einzelne Quadranten abgefragt und gesetzt werden
    ''' können. Welche der drei Properties genutzt wird, ist daher völlig egal. Auch für eine
    ''' Mischnutzung gibt es keinerlei Einschränkungen. 
    ''' </summary>
    Public Property Verdeckung As Verdeckt
    '
    ''' <summary>
    ''' Eine Enumeration, die den Typ der Animation festlegt.
    ''' </summary>
    <XmlIgnore>
    Public Property AnimTyp As Animation
    '
    ''' <summary>
    ''' Wenn dieses Flag gesetzt ist, verzweigt die Ausgabe beim nächsten Paint-Event
    ''' in die Animation.
    ''' </summary>
    <XmlIgnore>
    Public Property AnimShowAnimated As Boolean
    '
    ''' <summary>
    ''' Wenn der Wert größer 0 ist, werden nach dem Start der Animation diese nicht wirklich
    ''' gestartet, sondern StartVerzögerung wird nach jedem Paint-Event rückwärts gezählt.
    ''' Erst wenn es 0 ist, wird die Animation gestartet.
    ''' </summary>
    <XmlIgnore>
    Public Property AnimStartDelay As Integer
    '
    Private _AnimMaxStep As Integer
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
            _AnimMaxStep = value * 100
        End Set
    End Property
    '
    ''' <summary>
    ''' Siehe AnimationMaxStep
    ''' </summary>
    <XmlIgnore>
    Public Property AnimCurStep As Integer
    '        '
    <XmlIgnore>
    Private _AnimLoops As Single
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
            'TODO 
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

    ' Sichtbar = logisches NOT (im Bitbereich) von Verdeckt, bei 4 relevanten Bits
    <XmlIgnore>
    Public Property Sichtbar() As Sichtbar
        Get
            ' 15 (1111) XOR Verdeckt ergibt invertierte Bits im Bereich 0..15
            Return CType(15 Xor CInt(Verdeckung), Sichtbar)
        End Get
        Set(value As Sichtbar)
            ' Umgekehrt: Verdeckt ist invertiert zu Sichtbar
            Verdeckung = CType(15 Xor CInt(value), Verdeckt)
        End Set
    End Property

    ''' <summary>
    ''' Zugriff auf ein einzelnes Verdeckt-Bit. 
    ''' Die Oberfläche eines jeden Steines hat 4 Quadranten. Jeder einzelne kann sichtbar
    ''' oder verdeckt sein. Die Feststellung, ob ein Quadrant sichtbar ist oder nicht, 
    ''' geht ganz einfach. Der arrFB(x,y,z) wird über drei ineinander verschachtelte 
    ''' For-Next-Schleifen durchlaufen. Die innerste Schleife ist die z-Schleife.
    ''' Sie durchsucht das Feld von oben nach unten, also rückwärts.
    ''' Das erste Feld, das ungleich 0 ist, und nicht den SteinStatus Unsichtbar hat,
    ''' gehört zu einem Quadranten, der sichtbar ist.
    ''' Alle darunter liegenden Quadranten sind unsichtbar.
    ''' Jetzt muss nur noch herausgefunden werden, zu welchem Quadranten die x-y-z-Position
    ''' gehört. Auch das ist einfach und bei der Beschreibung der Feldbeschreibers arrFB
    ''' beschrieben, wo OffsetX und OffsetY beschrieben sind, über die das geht,
    ''' über die man an den Index herankommt und damit an die SteinInfo des Steines.
    ''' Siehe Spielfeld\liesmich.txt
    ''' </summary>
    ''' <param name="quadrant"></param>
    ''' <returns></returns>
    <XmlIgnore>
    Public Property VerdecktQuadrant(quadrant As Quadrant) As Boolean
        Get
            Return (CInt(Verdeckung) And CInt(quadrant)) <> 0
        End Get
        Set(value As Boolean)
            Dim mask As Integer = CInt(quadrant)
            Dim v As Integer = CInt(Verdeckung)
            If value Then
                v = v Or mask   ' Bit setzen
            Else
                v = v And Not mask ' Bit löschen
            End If
            Verdeckung = CType(v, Verdeckt)
        End Set
    End Property

    ' Zugriff auf ein einzelnes Sichtbar-Bit (logische Umkehr von Verdeckt)
    <XmlIgnore>
    Public Property SichtbarQuadrant(quadrant As Quadrant) As Boolean
        Get
            ' Sichtbar bedeutet: das Verdeckt-Bit ist 0
            Return (CInt(Verdeckung) And CInt(quadrant)) = 0
        End Get
        Set(value As Boolean)
            Dim mask As Integer = CInt(quadrant)
            Dim v As Integer = CInt(Verdeckung)
            If value Then
                ' Sichtbar => Verdeckt-Bit löschen
                v = v And Not mask
            Else
                ' Nicht sichtbar => Verdeckt-Bit setzen
                v = v Or mask
            End If
            Verdeckung = CType(v, Verdeckt)
        End Set
    End Property
    '
    ''' <summary>
    ''' Aus Gründen der Programmlesbarkeit kann es hilfreich sein, die
    ''' Enumeration Sichtbar in Verdeckt zu konvertieren.
    ''' </summary>
    ''' <param name="sichtbar"></param>
    ''' <returns></returns>
    Public Shared Function CvtSichtbarToVerdeckt(sichtbar As Sichtbar) As Verdeckt
        Return CType(15 Xor CInt(sichtbar), Verdeckt)
    End Function
    '
    ''' <summary>
    ''' Aus Gründen der Programmlesbarkeit kann es hilfreich sein, die
    ''' Enumeration Verdeckt in Sichtbar zu konvertieren.
    ''' </summary>
    ''' <param name="verdeckt"></param>
    ''' <returns></returns>
    Public Shared Function CvtVerdecktToSichtbar(verdeckt As Verdeckt) As Sichtbar
        Return CType(15 Xor CInt(verdeckt), Sichtbar)
    End Function

    ''' <summary>
    ''' Erstellt eine echte tiefe Kopie von SteinInfo.
    ''' Alle Wertetypen/Enums sind ohnehin kopiert; Referenzen werden manuell geklont.
    ''' </summary>
    Public Function DeepCopy() As SteinInfo

        ' Flache Kopie aller Felder (inkl. privater Backing Fields wie _AnimMaxStep, _AnimLoops)
        Dim c As SteinInfo = DirectCast(Me.MemberwiseClone(), SteinInfo)

        ' Tiefer Kopieren aller Referenztypen/Felder:
        ' Triple ist eine Klasse → explizit klonen
        If Me.Postion3D IsNot Nothing Then
            c.Postion3D = Me.Postion3D.DeepCopy
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
        Return Postion3D.x = 0
    End Function

End Class
