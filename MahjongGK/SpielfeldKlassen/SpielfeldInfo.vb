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
Imports System.Xml.Serialization
Imports MahjongGK.Spielfeld



#Disable Warning IDE0079
#Disable Warning IDE1006

''' <summary>
''' Xml-Container für die Klasse Steininfo, die die Eigenschaften der einzelnen Steine kapselt.  
''' </summary>
Public Class SpielfeldInfo

#Region "Konstruktor / Initialisierung"
    Sub New()

    End Sub

    ''' <summary>
    ''' spielSize ist die Anzahl der Steine, die maximal nebeneinander und untereinander
    ''' auf dem Feld Platz haben. Z ist die Anzahl der Schichten übereinander.
    ''' </summary>
    ''' <param name="spielSize"></param>
    Sub New(spielSize As Triple)

        If spielSize.x < 3 OrElse spielSize.y < 3 OrElse spielSize.z < 1 Then
            Throw New Exception("Spielgröße ist die Größe des Spielfeld in Steinen, " &
                                "mindestens drei Steine breit, drei Steine tief und eine Schicht hoch." &
                                spielSize.ToString)
        End If

        With spielSize
            xMin = 1
            yMin = 1
            zMin = 0
            xMax = .x * 2
            yMax = .y * 2
            zMax = .z - 1

            xUBnd = xMax + 1
            yUBnd = yMax + 1
            zUBnd = zMax
        End With

        ReDim arrFB(xUBnd, yUBnd, zUBnd)
        SteinInfos = New List(Of SteinInfo)

        WindsInOneClickGroup = INI.Spielbetrieb_WindsAreInOneClickGroup

    End Sub

#End Region

#Region "Daten, die gespeichert werden"

#Region "Spielfeld"


    Public XmlInfo_SteinInfo_Count As Integer

    Public Name As String

    Public SteinInfos As List(Of SteinInfo) = Nothing

    Public Property arrFB() As Integer(,,)


    Public xMin As Integer = 1
    Public yMin As Integer = 1
    Public zMin As Integer = 0
    Public xMax As Integer
    Public yMax As Integer
    Public zMax As Integer

    Public xUBnd As Integer
    Public yUBnd As Integer
    Public zUBnd As Integer


    Public Generator As SpielsteinGenerator

#End Region

#Region "Editor und Spielfeld"

    Private _WindsAreInOneClickGroup As Boolean
    ''' <summary>
    ''' Gehört zu den Spielregeln und bestimmt, ob die 4 Winde in eine KlickGruppe gehören.
    ''' Wird beim Instanzieren von SpielfeldInfo auf den Defaultwert gesetzt, der durch
    ''' INI.Regeln_WindsInOneClickGroup festgelegt ist. Kann jederzeit geändert werden
    ''' </summary>
    ''' <returns></returns>
    <XmlIgnore>
    Public Property WindsInOneClickGroup As Boolean
        Get
            Return _WindsAreInOneClickGroup
        End Get
        Set(value As Boolean)
            If value <> _WindsAreInOneClickGroup AndAlso Not IsNothing(SteinInfos) AndAlso SteinInfos.Count > 0 Then
                UpdateWindsAreInOneClickGroup(value)
            End If
            _WindsAreInOneClickGroup = value
        End Set
    End Property

    ''' <summary>
    ''' Beim Xml-Zugriff darf die Zusatzfunktionalität im Setter
    ''' nicht ausgeführt werden, deshalb doppelte Property.
    ''' </summary>
    ''' <returns></returns>
    <XmlElement("WindsInOneClickGroup")>
    Public Property WindsInOneClickGroup_ForXmlOnly As Boolean
        Get
            Return _WindsAreInOneClickGroup
        End Get
        Set(value As Boolean)
            _WindsAreInOneClickGroup = value
        End Set
    End Property

    ''' <summary>
    ''' Spielbar ist ein Spiel dann, wenn Daten vorhanden sind und wenn
    ''' wenn alle Paare auf dem Spielfeld vollständig sind, also keine
    ''' "StrohWitwen" oder "Witwer" vorhanden sind und den Werkstattsteinen
    ''' alle andere Grafiken zugeordnet wurden
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property IsPlayable As Boolean
        Get
            If IsNothing(SteinInfos) OrElse SteinInfos.Count = 0 Then
                Return False
            End If

            Dim arrKlickGruppeCount(MJ_STEININDEX_MAX) As Integer
            Dim foundWerkstattStein As Boolean


            For Each stein As SteinInfo In SteinInfos
                arrKlickGruppeCount(stein.KlickGruppe) += 1
                If stein.IsWerkbankStein Then
                    foundWerkstattStein = True
                End If
            Next

            If foundWerkstattStein Then
                Return False
            End If

            For idx As Integer = 0 To MJ_STEININDEX_MAX
                If (arrKlickGruppeCount(idx) Mod 2) <> 0 Then
                    'Für 0 gilt: 0 Mod 2 = 0 -> braucht deshalb nicht abgefangen zu werden,
                    'denn nicht vorhandene Steinpaare brauchen nicht berücksichtigt werden,
                    'ausgenommen, es gibt überhaupt keine Steine. Das ist oben abgefangen.
                    Return False
                End If
            Next

            Return True

        End Get

    End Property

#End Region


#End Region
    ''' <summary>
    ''' Vergleicht diese Instanz mit einer anderen SpielfeldInfo.
    ''' Es wird nur geprüft, ob beide SpielfeldInfo dieselbe Instanz sind.
    ''' spielfeldinfo_Other darf Nothing sein, dann sind sie unterschiedlich.
    ''' Vorsicht Falle: CompareSpielfeldInfo darf natürlich nicht aufgerufen
    ''' werden, wenn die eigene Instanz Nothing ist.
    ''' Das geht mit der anderen Überladung.
    ''' </summary>
    Public Function IsEqual(spielfeldinfo_Other As SpielfeldInfo) As Boolean
        Return Me Is spielfeldinfo_Other
    End Function
    '
    ''' <summary>
    ''' Vergleicht zwei SpielfeldInfo-Instanzen.
    ''' Prüft:
    '''   - ob beide Nothing sind (--> Exception("Beide SpielfeldInfo sind Nothing (Initialisierungsfehler).")
    '''   - ob sie auf dieselbe Instanz zeigen
    ''' </summary>
    ''' <param name="spielfeldinfoA">Erste SpielfeldInfo</param>
    ''' <param name="spielfeldinfoB">Zweite SpielfeldInfo</param>
    ''' <returns>True, wenn beide dieselbe Instanz sind, False sonst.</returns>
    Public Shared Function IsEqual(spielfeldinfoA As SpielfeldInfo, spielfeldinfoB As SpielfeldInfo) As Boolean
        If spielfeldinfoA Is Nothing AndAlso spielfeldinfoB Is Nothing Then
            Throw New Exception("Beide SpielfeldInfo sind Nothing (Initialisierungsfehler).")
        End If

        Return spielfeldinfoA Is spielfeldinfoB
    End Function
    '


#Region "Manipulationen des Spielfeldes Teil 1  (ACHTUNG COPY-PASTE-REGION #1)"
    '
    'Diese Region gib es im Spiel zweimal: In der SpielfeldInfo und in der Werkbank!
    'Änderungen nur in der Spielfeldinfo vornehmen und die Region komplett in die
    'Werkbank kopieren!
    'Nur solche Funktionen hinzufügen, die in beiden Klassen gebraucht werden.


    'Jeder Stein belegt 4 Felder.
    'Alle 4 Felder beinhalten Flags.
    'Das linke obere Feld (Der Quadrant links oben) transporiert zusätzich einen Index.
    'Das linke obere Feld ist der FeldBeschreiber infoFB, die andern sind die childFB.
    'Die Flags in den childFB zeigen die Richtung an, wo der FB steht.
    '

    ''' <summary>
    ''' Setzt die X-Y-OffsetWerte der childFB des Feldbeschreibers und den Index des Feldbeschreibers selber. 
    ''' </summary>
    ''' <param name="infoFBTriple"></param>
    Public Sub CopySteinIndexToSpielfeldPos3DAndSetOffsetXY(infoFBTriple As Triple, SteinInfoIndex As Integer)
        '
        Dim infoFBx As Integer = infoFBTriple.x
        Dim infoFBy As Integer = infoFBTriple.y
        Dim infoFBz As Integer = infoFBTriple.z

        'Vom childFB rechts unter dem infoFB geht es zum infoFB, indem von beiden Koordinaten
        '1 subtrahiert wird. Daher:
        SetOffsetX(arrFB(infoFBx + 1, infoFBY + 1, infoFBz), True)
        SetOffsetY(arrFB(infoFBx + 1, infoFBY + 1, infoFBz), True)
        '
        'Der childFB genau unter dem infoFB zum infoFB geht mit OffsetX = 0 und OffsetY = -1
        SetOffsetX(arrFB(infoFBx, infoFBY + 1, infoFBz), False)
        SetOffsetY(arrFB(infoFBx, infoFBY + 1, infoFBz), True)
        '
        'Vom FB rechts vom infoFB zum infoFB: OffsetX = -1, OffsetY = 0
        SetOffsetX(arrFB(infoFBx + 1, infoFBY, infoFBz), True)
        SetOffsetY(arrFB(infoFBx + 1, infoFBY, infoFBz), False)

        'Der infoFB hat keinen Offset, daher beide 0 (Verweis auf sich selber)
        SetOffsetX(arrFB(infoFBx, infoFBY, infoFBz), False)
        SetOffsetY(arrFB(infoFBx, infoFBY, infoFBz), False)
        '
        SetIndexStein(arrFB(infoFBx, infoFBY, infoFBz), SteinInfoIndex)

    End Sub


    ''' <summary>
    ''' Setzt einen Stein auf das Spielfeld und Added einen Stein mit Basisinformationen
    ''' zu den SteinInfos. Wenn OK, gibt die Funktion True zurück.
    ''' Sind die Koordinaten des infoFBTriple ungültig, wird der Stein als Stein.Dummy
    ''' (Fehler-Grafik) auf die erste gefundene Position als frei schwebender Stein in
    ''' der obersten Ebene geparkt. Ungültig heißt, es würde ein OutOfRange-Error entstehen.
    ''' Das ist ein Sicherheitsgurt während der Programmentwicklung.
    ''' Die Funktion gibt dann False zurück.
    ''' </summary>
    ''' <param name="steinPos3D"></param>
    ''' <returns></returns>
    Public Function AddSteinToSpielfeld(steinIndex As SteinIndexEnum, steinPos3D As Triple, Optional tmpDebug As Integer = 0) As Boolean

        'Der steinInfoIndex wird hier gesichert, obwohl er gleichlautend ist mit dem
        'Index in SteinInfos. Grund: werden später Steine im Editor entfernt, verschieben sich die
        'Indexnummern in SteinInfos und da muss arrFB aktualisert werden. Dazu braucht man den
        '"alten" steinInfoIndex, eben diesen steinInfoIndex.
        Dim newSteinInfo As New SteinInfo(steinInfoIndex:=SteinInfos.Count, steinIndex, steinPos3D, tmpDebug)

        If Not steinPos3D.IsInsideSpielfeldBounds(arrFB) Then
            'Falsche Positionsangabe.
            'Kein Throw Nex Exception, sondern Anzeige auf dem Spielfeld
            'Fehlergrafik in die oberste Ebene als frei schwebender Stein.
            'Im fertig entwickeltem Programm sollte dieser Teil nicht mehr
            'aufgerufen werden :-)
            '
            'Linke obere Ecke der obersten Ebene
            Dim tpl As New Triple(1, 1, arrFB.GetUpperBound(2))

            Do
                Dim tplR As Triple = SearchPlace(tpl, direction:=Direction.Right)
                Select Case tplR.Valide
                    Case ValidePlaceEnum.NoFundamentFound   'Zeilenende erreicht.
                        tpl.y += 2 'Eine Steinreihe tiefer weitersuchen
                        tpl.x = 1
                        If tpl.y > arrFB.GetUpperBound(1) - 1 Then
                            'Die ganze oberste Ebene ist vollgepflastert mit Steinen.
                            'Da das ziemlich sicher nicht absichlich geschehen ist,
                            'unterstelle ich, das da viele Fehlergrafiken dabei sind
                            'und breche ab ohne weitere Prüfung und setze den
                            'Stein auf die Position links oben etwas versetzt,
                            'also optisch auf eine Ebene, die es garnicht gibt.
                            tpl.x = 2
                            tpl.y = 2

                            'SetStein rekursiv, aber mit jetzt gültigen Werten aufrufen
                            AddSteinToSpielfeld(SteinIndexEnum.ErrorSy, tpl)
                            Return False
                        End If

                    Case ValidePlaceEnum.Yes, ValidePlaceEnum.NoFundamentFound
                        ' FoundResult.NoFundament ist in diesem Fall OK, er wird zum freischwebendem Stein.
                        'SetStein rekursiv, aber mit jetzt gültigen Werten aufrufen
                        AddSteinToSpielfeld(SteinIndexEnum.ErrorSy, tplR)
                        Return False
                End Select
            Loop
        End If
        '
        'Hier ist jetzt die Normal -Routine
        '
        'SteinInfos.Count ist der Index, den der Stein in SteinInfos haben wird.
        CopySteinIndexToSpielfeldPos3DAndSetOffsetXY(steinPos3D, SteinInfoIndex:=SteinInfos.Count)
        '() nicht trennen, der Index muss stimmen. Späterer Zugriff über
        'Dim aktSteininfo As SteinInfo = SteinInfos(indexSteinInfo)
        SteinInfos.Add(newSteinInfo)

        Return True

    End Function

#End Region

#Region "Manipulation des Spielfeldes Teil 2 (Angepasster Code)"

    Public Function AddSteinToSpielfeld(wsSteinInfo As SteinInfo, steinPos3D As Triple, Optional tmpDebug As Integer = 0) As Boolean

        'unbedingt von wsSteinInfo eine tiefe Kopie machen!
        'Wird ein Werkstück mehrfach eingefügt, sind plötzlich Zwillinge
        'oder Drillinge auf dem Spielfeld, da die Referenzen auf die
        'gleiche SteinInfo zeigen.

        Dim siDc As SteinInfo = wsSteinInfo.DeepCopy

        With siDc
            .SteinInfoIndex = SteinInfos.Count
            .Postion3D = steinPos3D
        End With


        If Not steinPos3D.IsInsideSpielfeldBounds(arrFB) Then
            'Falsche Positionsangabe.
            'Kein Throw Nex Exception, sondern Anzeige auf dem Spielfeld
            'Fehlergrafik in die oberste Ebene als frei schwebender Stein.
            'Im fertig entwickeltem Programm sollte dieser Teil nicht mehr
            'aufgerufen werden :-)
            '
            'Linke obere Ecke der obersten Ebene
            Dim tpl As New Triple(1, 1, arrFB.GetUpperBound(2))

            Do
                Dim tplR As Triple = SearchPlace(tpl, direction:=Direction.Right)
                Select Case tplR.Valide
                    Case ValidePlaceEnum.NoFundamentFound   'Zeilenende erreicht.
                        tpl.y += 2 'Eine Steinreihe tiefer weitersuchen
                        tpl.x = 1
                        If tpl.y > arrFB.GetUpperBound(1) - 1 Then
                            'Die ganze oberste Ebene ist vollgepflastert mit Steinen.
                            'Da das ziemlich sicher nicht absichlich geschehen ist,
                            'unterstelle ich, das da viele Fehlergrafiken dabei sind
                            'und breche ab ohne weitere Prüfung und setze den
                            'Stein auf die Position links oben etwas versetzt,
                            'also optisch auf eine Ebene, die es garnicht gibt.
                            tpl.x = 2
                            tpl.y = 2

                            'SetStein rekursiv, aber mit jetzt gültigen Werten aufrufen
                            AddSteinToSpielfeld(SteinIndexEnum.ErrorSy, tpl)
                            Return False
                        End If

                    Case ValidePlaceEnum.Yes, ValidePlaceEnum.NoFundamentFound
                        ' FoundResult.NoFundament ist in diesem Fall OK, er wird zum freischwebendem Stein.
                        'SetStein rekursiv, aber mit jetzt gültigen Werten aufrufen
                        AddSteinToSpielfeld(SteinIndexEnum.ErrorSy, tplR)
                        Return False
                End Select
            Loop
        End If
        '
        'Hier ist jetzt die Normal -Routine
        '
        'SteinInfos.Count ist der Index, den der Stein in SteinInfos haben wird.
        CopySteinIndexToSpielfeldPos3DAndSetOffsetXY(steinPos3D, SteinInfoIndex:=SteinInfos.Count)
        '() nicht trennen, der Index muss stimmen. Späterer Zugriff über
        'Dim aktSteininfo As SteinInfo = SteinInfos(indexSteinInfo)
        SteinInfos.Add(siDc)

        Return True

    End Function

    ''' <summary>
    ''' Wie ValidePlace, nur wird ein von der Werkstatt 
    ''' erstellter Baustein Stein für Stein geprüft.
    ''' </summary>
    ''' <returns></returns>
    Public Function IsValideArea(werkbankResult As (wbSteinInfos As List(Of SteinInfo), wbArrFB As Integer(,,)), insertAt As Triple) As ValidePlaceEnum
        'Anmerkung: Ich hatte in der Deklaration von werkbankResult steinInfos und arrFB stehen.
        '           Mußte ich dann ändern in wbSteinInfos und wbArrFB, weil die Intellisenze
        '           die Werte von SteinInfos und arrFB aus dem Modul SpielfeldDaten anzeigt,
        '           was irritierend ist.

        With werkbankResult

            For wbrZ As Integer = 0 To .wbArrFB.GetUpperBound(2)
                For wbrX As Integer = 1 To .wbArrFB.GetUpperBound(0) - 1
                    For wbrY As Integer = 1 To .wbArrFB.GetUpperBound(1) - 1
                        If IsIndexQuadrant(.wbArrFB(wbrX, wbrY, wbrZ)) Then

                            Dim idx As Integer = GetIndexStein(.wbArrFB(wbrX, wbrY, wbrZ))
                            Dim newPlace As New Triple(insertAt.x + wbrX - 1, insertAt.y + wbrY - 1, insertAt.z + wbrZ)
                            Dim tpl As Triple = IsValidePlace(newPlace)

                            'If tpl.Valide <> ValidePlace.Yes Then
                            '    Stop
                            'End If

                            Select Case tpl.Valide
                                Case ValidePlaceEnum.NoFundamentFound
                                    'Hier nur die unterste Ebene prüfen, die Ebenen drüber haben
                                    'kein Fundament, das entsteht ja erst.
                                    '(Es wird entstehen, denn die Werkbank hat dafür gesorgt.)
                                    If wbrZ = 0 Then
                                        'Wenn die unterste Ebene des Werkbankresult nicht auf der untersten Ebene 
                                        'eingefügt wird, dann hat der Stein kein Fundament.
                                        '(Bei wbrZ = 0 And insertAt.z = 0 gibt es kein NoFundamentFound)
                                        Return tpl.Valide
                                    Else
                                        'bei allen anderen Steinen in den anderen Ebenen fehlt das Fundament,
                                        'das wird ja vom Werkbankresult erst hinzugefügt.
                                        'Nichts machen, nächsten Stein prüfen oder Ende der Prüfung
                                    End If
                                Case ValidePlaceEnum.Occupied
                                    Return tpl.Valide

                                Case ValidePlaceEnum.OutsideBorder
                                    Return tpl.Valide

                                Case ValidePlaceEnum.Yes
                               'Nichts machen, nächsten Stein prüfen oder Ende der Prüfung

                                Case ValidePlaceEnum.NotSet
                                    If Debugger.IsAttached Then
                                        Stop ' Programmierfehler
                                    End If
                            End Select

                        End If
                    Next
                Next
            Next

        End With

        Return ValidePlaceEnum.Yes

    End Function
    '
    ''' <summary>
    ''' Fügt das werkbankResult in das Spielfeld ein.Es muss vorher geprüft werden mit IsValideArea
    ''' ob das überhaupt möglich ist.
    ''' Reihenfolge 1. IsValideArea 2. mit dem SpielfeldSteineAustauscher die Steine ändern.
    ''' (Kann auch später insgesamt geschehen) 2./3. AddWerkbankResultToSpielfeld.
    ''' Es gibt aber noch ein Sicherheitsnetz: Gibt es beim Einfügen doch Probleme,
    ''' wird umgeschaltet und alle 
    ''' </summary>
    ''' <param name="werkbankResult"></param>
    ''' <param name="insertAt"></param>
    Public Sub AddWerkbankResultToSpielfeld(werkbankResult As (steinInfos As List(Of SteinInfo), arrFB As Integer(,,)), insertAt As Triple)

        If IsNothing(werkbankResult) OrElse IsNothing(werkbankResult.steinInfos) OrElse IsNothing(arrFB) Then
            If Debugger.IsAttached Then
                Stop 'Programmierfehler
            Else
                Exit Sub
            End If
        End If

        If werkbankResult.steinInfos.Count = 0 Then
            Exit Sub
        End If

        'Nochmalige Prüfung (oder erstmalige Prüfung), da ich während
        'der Werkstücke nicht den Dienstweg gehe.
        Dim vp As ValidePlaceEnum = IsValideArea(werkbankResult, insertAt)

        With werkbankResult
            If vp <> ValidePlaceEnum.Yes Then
                For idx As Integer = 0 To .steinInfos.Count - 1
                    .steinInfos(idx).IsWerkbankStein = True 'wieder einschalten (ob man's noch gebrauchen kann, weis ich derzeit nicht)
                    .steinInfos(idx).SteinStatusUsed = SteinStatus.WerkstückEinfügeFehler
                    .steinInfos(idx).SteinStatusIst = SteinStatus.WerkstückEinfügeFehler
                Next
            End If

            For wbrZ As Integer = 0 To .arrFB.GetUpperBound(2)
                For wbrX As Integer = 1 To .arrFB.GetUpperBound(0) - 1
                    For wbrY As Integer = 1 To .arrFB.GetUpperBound(1) - 1
                        If IsIndexQuadrant(.arrFB(wbrX, wbrY, wbrZ)) Then
                            Dim idx As Integer = GetIndexStein(.arrFB(wbrX, wbrY, wbrZ))
                            Dim newPlace As New Triple(insertAt.x + wbrX - 1, insertAt.y + wbrY - 1, insertAt.z + wbrZ)
                            If IsFreePlace(newPlace) Then
                                AddSteinToSpielfeld(.steinInfos(idx), newPlace)
                            End If
                        End If
                    Next
                Next
            Next
        End With

    End Sub
    '
    ''' <summary>
    ''' Fügt den Werkstück in das Spielfeld ein.Es muss vorher geprüft werden mit IsValideArea
    ''' ob das überhaupt möglich ist.
    ''' Reihenfolge 1. IsValideArea 2. mit dem Werkstück (hat den SpielfeldSteineAustauscher integriert) die Steine ändern.
    ''' (Kann auch später insgesamt geschehen) 2./3. AddWerkstückToSpielfeld.
    ''' </summary>
    ''' <param name="wbb"></param>
    ''' <param name="insertAt"></param>
    Public Sub AddWerkstückToSpielfeld(wbb As Werkstück, insertAt As Triple)

        Dim werkstück As (steinInfos As List(Of SteinInfo), arrFB As Integer(,,))
        werkstück.steinInfos = wbb.SteinInfos
        werkstück.arrFB = wbb.ArrFB
        AddWerkbankResultToSpielfeld(werkstück, insertAt)

    End Sub





    ''' <summary>
    ''' Entfernt einen Stein vollständig, d.h. einschließlich aller Daten vom Spielfeld
    ''' und aus steininfos
    ''' Hinweis: Um ihn nicht mehr anzuzeigen muß er nicht entfernt werden. Dazu wird
    ''' lediglich sein SteinStatus geändert.
    ''' </summary>
    ''' <param name="arrFB"></param>
    ''' <param name="steininfos"></param>
    ''' <param name="steininfo"></param>
    Public Sub RemoveSteinFromSpielfeld(arrFB(,,) As Integer, steininfos As SpielfeldInfo, steininfo As SteinInfo)


        ''
    End Sub

    Public Sub RemoveLastSteinFromSpielfeld(arrFB(,,) As Long, steininfos As SpielfeldInfo)

    End Sub

#End Region


#Region "Fragen an das Spielfeld (ACHTUNG COPY-PASTE-REGION #2)"
    '
    'Diese Region gib es im Spiel zweimal: In der SpielfeldInfo und in der Werkbank!
    'Änderungen nur in der Spielfeldinfo vornehmen und die Region komplett in die
    'Werkbank kopieren!
    'Nur solche Funktionen hinzufügen, die in beiden Klassen gebraucht werden.

    '
    ''' <summary>
    ''' Eine Kombination aus IsInsideSpielfeldBounds, IsFreePlace und HasFundament.
    ''' Valide = ValidePlace.Yes, heist, der Stein kann dort abgelegt werden.
    ''' </summary>
    ''' <param name="triple"></param>
    ''' <returns></returns>
    Public Function IsValidePlace(triple As Triple) As Triple

        If Not triple.IsInsideSpielfeldBounds(arrFB) Then
            Return New Triple(triple, ValidePlaceEnum.OutsideBorder)
        Else
            If IsFreePlace(triple) Then
                If HasFundament(triple) Then
                    Return New Triple(triple, ValidePlaceEnum.Yes)
                Else
                    Return New Triple(triple, ValidePlaceEnum.NoFundamentFound)
                End If
            Else
                Return New Triple(triple, ValidePlaceEnum.Occupied)
            End If
        End If

    End Function

    ''' <summary>
    ''' Prüft, ob der Platz frei ist
    ''' </summary>
    ''' <param name="infoFBTriple"></param>
    ''' <returns></returns>
    Public Function IsFreePlace(infoFBTriple As Triple) As Boolean

        Dim infoFBx As Integer = infoFBTriple.x
        Dim infoFBy As Integer = infoFBTriple.y
        Dim infoFBz As Integer = infoFBTriple.z

        If infoFBx < xMin Then Return False
        If infoFBy < yMin Then Return False

        'Bereits bei Gleichheit wird False zurück gemeldet, weil nach der
        'Position des InfoFB gefragt wird. Seine Childs sind auf dieser Position
        'bereits außerhalb.
        If infoFBx >= xMax Then Return False
        If infoFBy >= yMax Then Return False

        If infoFBz > zMax Then Return False
        '
        'die obere Zeile
        'Den FB rechts unter dem infoFB
        If arrFB(infoFBx + 1, infoFBy + 1, infoFBz) <> 0 Then Return False
        '
        'Den FB genau unter dem infoFB
        If arrFB(infoFBx, infoFBy + 1, infoFBz) <> 0 Then Return False

        'Den FB rechts vom infoFB 
        If arrFB(infoFBx + 1, infoFBy, infoFBz) <> 0 Then Return False

        'Der infoFB 
        If arrFB(infoFBx, infoFBy, infoFBz) <> 0 Then Return False
        '
        Return True

    End Function
    '
    ''' <summary>
    ''' Prüft, ob der Stein auf der Grundfläche, oder vollständig 
    ''' auf anderen Steinen stehen würde.
    ''' </summary>
    ''' <param name="infoFBTriple"></param>
    ''' <returns></returns>
    Public Function HasFundament(infoFBTriple As Triple) As Boolean

        'Im Unterschied zu IsFreePlace wird hier in der Ebene unter dem Stein gesucht,
        'nicht in der Steinebene.
        Dim infoFBx As Integer = infoFBTriple.x
        Dim infoFBy As Integer = infoFBTriple.y
        Dim infoFBz As Integer = infoFBTriple.z

        If infoFBx < xMin Then Return False
        If infoFBy < yMin Then Return False

        'Bereits bei Gleichheit wird False zurück gemeldet, weil nach der
        'Position des InfoFB gefragt wird. Seine Childs sind auf dieser Position
        'bereits außerhalb.
        If infoFBx >= xMax Then Return False
        If infoFBy >= yMax Then Return False

        If infoFBz > zMax Then Return False

        If infoFBz = 0 Then
            'Das ist die Grundfläche des Spieles.
            Return True
        Else
            'Alle Plätze des infoFB in der Ebene drunter müssen belegt sein.
            '(infoFBx, infoFBY und infoFBz sind die Koordinaten des abgefragten infoFB)
            '
            'genau eine Ebene tiefer gehen.
            infoFBz -= 1
            '
            'Den FB rechts unter dem infoFB
            If arrFB(infoFBx + 1, infoFBy + 1, infoFBz) = 0 Then Return False
            '
            'Den FB genau unter dem infoFB
            If arrFB(infoFBx, infoFBy + 1, infoFBz) = 0 Then Return False
            '
            'Den FB rechts vom infoFB 
            If arrFB(infoFBx + 1, infoFBy, infoFBz) = 0 Then Return False

            'Der infoFB selber 
            If arrFB(infoFBx, infoFBy, infoFBz) = 0 Then Return False

            Return True

        End If

    End Function

    ''' <summary>
    ''' Incrementiert die X- und Y-Koordinaten des Triple in die vorgegebene (Himmels-) Richtung.
    ''' </summary>
    Public Function IncDirection(triple As Triple, direction As Direction, Optional [Step] As Integer = 1) As Triple

        'DeepCopy erstellen um sicherzustellen, dass der Ausgangswert unverändert bleibt.
        Dim newTriple As Triple = triple.DeepCopy

        With newTriple
            Select Case direction
                Case Direction.Up
                    .y -= [Step]

                Case Direction.UpRight
                    .y -= [Step]
                    .x += [Step]

                Case Direction.Right
                    .x += [Step]

                Case Direction.DownRight
                    .y += [Step]
                    .x += [Step]

                Case Direction.Down
                    .y += [Step]

                Case Direction.DownLeft
                    .y += [Step]
                    .x -= [Step]

                Case Direction.Left
                    .x -= [Step]

                Case Direction.UpLeft
                    .y -= [Step]
                    .x -= [Step]

            End Select
        End With

        Return newTriple

    End Function

    '
    ''' <summary>
    ''' Sucht von der Startposition (Koordinaten in infoFBTriple)
    ''' in die angegebene  Richtung nach dem nächsten freiem Platz.
    ''' Wenn z = 0 (also auf der Grundfläche) wird jeder freie Platz zurückgegeben.
    ''' Wenn z > 0 nur Positionen, wo der Stein vollständig auf anderen Steinen
    ''' steht. 
    ''' Ob was gefunden wurde und wenn nicht, warum, steht in Triple.fr As SearchResult
    ''' </summary>
    ''' <param name="infoFBTriple"></param>
    ''' <param name="direction"></param>
    ''' <returns></returns>
    Public Function SearchPlace(infoFBTriple As Triple, direction As Direction) As Triple

        'DeepCopy um Seiteneffekte zu verhindern
        Dim tpl As Triple = infoFBTriple.DeepCopy

        'nicht mögliche Startpositionen anpassen
        If tpl.x < 1 Then tpl.x = 1
        If tpl.y < 1 Then tpl.y = 1

        If tpl.z = 0 Then
            Do
                If Not tpl.IsInsideSpielfeldBounds(arrFB) Then
                    Return New Triple(tpl, ValidePlaceEnum.OutsideBorder)
                Else
                    If IsFreePlace(tpl) Then
                        Return New Triple(tpl, ValidePlaceEnum.Yes)
                    End If
                End If
                tpl = IncDirection(tpl, direction)
            Loop
        Else
            Dim fFoundFreePlace As Boolean = False
            Do

                If Not tpl.IsInsideSpielfeldBounds(arrFB) Then
                    If fFoundFreePlace Then
                        Return New Triple(tpl, ValidePlaceEnum.NoFundamentFound)
                    Else
                        Return New Triple(tpl, ValidePlaceEnum.OutsideBorder)
                    End If
                Else
                    Dim fFound As Boolean = IsFreePlace(tpl)

                    If fFound And Not fFoundFreePlace Then
                        fFoundFreePlace = True
                    End If

                    If fFound AndAlso HasFundament(tpl) Then
                        Return New Triple(tpl, ValidePlaceEnum.Yes)
                    End If

                End If

                tpl = IncDirection(tpl, direction)
            Loop
        End If

    End Function
    '
    ''' <summary>
    ''' Zunächst wie die erste Überladung von SearchPlace, als die Suche nach einem freienm Platz.
    ''' Wenn einer gefunden wurde, wird der Platz verschoben nach Angabe von moveX und moveX
    ''' unter Berücksichtigung von stepHalberStein
    ''' </summary>
    ''' <param name="infoFBTriple"></param>
    ''' <param name="direction"></param>
    ''' <param name="moveX"></param>
    ''' <param name="moveY"></param>
    ''' <param name="stepHalberStein"></param>
    ''' <returns></returns>
    Public Function SearchPlace(infoFBTriple As Triple, direction As Direction, moveX As Move, moveY As Move, stepHalberStein As Boolean) As Triple

        Dim tpl As Triple = SearchPlace(infoFBTriple, direction)
        Dim add As Integer = If(stepHalberStein, 1, 2)

        Dim addX As Integer

        Do
            Select Case moveX
                Case Move.LeftOrUp
                    addX = -addX
                Case Move.RightOrDown
                    addX = add
                Case Else
                    addX = 0
            End Select

            Dim addY As Integer

            Select Case moveY
                Case Move.LeftOrUp
                    addY = -addY
                Case Move.RightOrDown
                    addY = add
                Case Else
                    addY = 0
            End Select

            Dim tpl2 As New Triple(infoFBTriple.x + addX, infoFBTriple.y + addY, infoFBTriple.z)

            Dim tpl3 As Triple = IsValidePlace(tpl2)

            Select Case tpl3.Valide
                Case ValidePlaceEnum.Yes
                    Return tpl3
                Case ValidePlaceEnum.NoFundamentFound
                    'nächster Versuch
                Case ValidePlaceEnum.OutsideBorder
                    Return tpl3
            End Select
        Loop

    End Function

    ''' <summary>
    ''' Gibt die die Koordinaten der Spielfeldmitte in einem Triple zurück.
    ''' Brauchbar um den den ersten Stein zu verlegen
    ''' Wird Ebene zu hoch angegeben, wird die oberste Ebene eingestellt.
    ''' Ist der Platz belegt, ist Found = FoundResult.NoFundamentFound 
    ''' </summary>
    ''' <param name="ebene"></param>
    ''' <returns></returns>
    Public Function GetSpielfeldCenter(ebene As Integer) As Triple

        If ebene > zMax Then
            ebene = zMax
        End If

        Dim tpl As New Triple(xMax \ 2, yMax \ 2, ebene)

        If IsFreePlace(tpl) Then
            If HasFundament(tpl) Then
                tpl.Valide = ValidePlaceEnum.Yes
            Else
                tpl.Valide = ValidePlaceEnum.NoFundamentFound
            End If
        Else
            tpl.Valide = ValidePlaceEnum.NoFundamentFound
        End If

        Return tpl

    End Function
    '
    '
    ''' <summary>
    ''' Gibt absolute Positionen im Spielfeld ohne Belegt-Prüfung zurück.
    ''' </summary>
    ''' <param name="position"></param>
    ''' <param name="ebene"></param>
    ''' <returns></returns>
    Public Function GetPositionImSpielfeld(position As PositionEnum, ebene As Integer) As Triple
        Return GetPositionImSpielfeld(position, New Triple(xMin, yMin, ebene), xMax \ 2, yMax \ 2)
    End Function
    '
    ''' <summary>
    ''' Gibt relative Positionen im Spielfeld ohne Belegt-Prüfung zurück.
    ''' Das Feld im Feld ist definiert durch startPosition, steineCountX und steineCountY 
    ''' </summary>
    ''' <param name="position"></param>
    ''' <param name="startPosition"></param>
    ''' <param name="steineCountX"></param>
    ''' <param name="steineCountY"></param>
    ''' <returns></returns>
    Public Function GetPositionImSpielfeld(position As PositionEnum, startPosition As Triple, steineCountX As Integer, steineCountY As Integer) As Triple

        Dim ebene As Integer = Math.Min(startPosition.z, zMax)

        Dim leftL As Integer = startPosition.x
        Dim leftM As Integer = steineCountX + leftL - 1
        Dim leftR As Integer = steineCountX * 2 + leftL - 2

        Dim topO As Integer = startPosition.y
        Dim topM As Integer = steineCountY + topO - 1
        Dim topU As Integer = steineCountY * 2 + topO - 2

        Dim deltaLeft As Integer = (leftM - leftL) \ 2
        Dim deltaTop As Integer = (topM - topO) \ 2

        Dim x As Integer
        Dim y As Integer

        Select Case position
            Case PositionEnum.EckeLO
                x = leftL : y = topO

            Case PositionEnum.EckeRO
                x = leftR : y = topO

            Case PositionEnum.EckeLU
                x = leftL : y = topU

            Case PositionEnum.EckeRU
                x = leftR : y = topU

            '-------------------
            Case PositionEnum.MitteL
                x = leftL : y = topM

            Case PositionEnum.MitteR
                x = leftR : y = topM

            Case PositionEnum.MitteO
                x = leftM : y = topO

            Case PositionEnum.MitteU
                x = leftM : y = topU

            '------------------- 
            Case PositionEnum.Center
                x = leftM : y = topM

            '-------------------
            Case PositionEnum.CenterLO
                x = leftL + deltaLeft : y = topO + deltaTop

            Case PositionEnum.CenterRO
                x = leftR - deltaLeft : y = topO + deltaTop

            Case PositionEnum.CenterLU
                x = leftL + deltaLeft : y = topU - deltaTop

            Case PositionEnum.CenterRU
                x = leftR - deltaLeft : y = topU - deltaTop

        End Select


        Return IsValidePlace(New Triple(x, y, ebene))

    End Function

#End Region

#Region "Manipulation des Feldbeschreibers FB"



    ' Kopiervorlagen der Funktionen und Methoden dieser Region als InlineCode
    '
    ' Lesen:
    ' Dim offsetX As Integer = If((fb And FLAG_XOffset) <> 0, 1, 0)
    ' Dim offsetY As Integer = If((fb And FLAG_YOffset) <> 0, 1, 0)
    ' Dim toggleFlag As Boolean = (fb And FLAG_ToggleFlag) <> 0
    ' Dim index As Integer = fb \ 1000
    '
    ' Schreiben:
    ' If value0or1 <> 0 Then fb = fb Or FLAG_XOffset Else fb = fb And Not FLAG_XOffset
    ' If value0or1 <> 0 Then fb = fb Or FLAG_YOffset Else fb = fb And Not FLAG_YOffset
    '
    ' If toggleValue Then
    '    fb = fb Or FLAG_ToggleFlag
    ' Else
    '    fb = fb And Not FLAG_ToggleFlag
    ' End If
    '
    ' Dim flags As Integer = fb And &HFF
    ' fb = newValue * 1000 + flags
    '
    ' fb = fb Xor FLAG_ToggleFlag ' Toggle
    '
    '
    ''' <summary>
    ''' Diese Überladung holt des Index direkt aus dem arrFB.
    ''' Wenn auf der Position kein Stein setht, wird -1 zurückgegeben VORSICHT, IsIndexQuadrant
    ''' funktioniert anders, weil es keinen Offset berücksichtigt. Zum Iterieren durch das Feld
    ''' IsIndexQuadrant verwenden. ---------
    ''' Jeder Stein belegt 4 Felder in arrFB. Die 4 Felder müssen alle einen Wert ungleich 0 haben,
    ''' damit sie als "belegt" erkannt werden können. Außerdem muss eines dieser Felder den Index
    ''' transportieren, der auf die SteinInformationen in SteinInfos zeigt.
    ''' Der Index steht nur im Feld links oben der 4 Felder. Zudem ist er um 1 erhöht, damit
    ''' der Index immer ungleich 0 ist, und mit 1000 multipliziert, damit die beiden niederwertigsten
    ''' Ziffern frei sind. Die verwende ich als Flagfeld und betrachte den Bereich bitweise.
    ''' In den drei anderen Feldern sind dann zwei Flags gesetzt, die besagen, ob der Feldbeschreiber
    ''' links, drüber oder linksdrüber steht, daraus werden die Offsets gebildet,
    ''' und erneut auf den Array zugegriffen. Eine Unterscheidung, ob beide Offsetz = 0 sind, ist nicht
    ''' nötig, Koordinate + 0 bleibt Koordinate. Weil im Feld rechts-unten die Offsetzs 0 sind,
    ''' muß der Index immer größer null sein, damit die "belegt" Bedingung erfüllt ist. 
    ''' </summary>
    ''' <param name="x"></param>
    ''' <param name="y"></param>
    ''' <param name="z"></param>
    ''' <returns></returns> '
    Public Function GetIndexStein(x As Integer, y As Integer, z As Integer) As Integer
        Dim fb As Integer = arrFB(x, y, z)
        Dim offsetX As Integer = If((fb And FLAG_XOffset) <> 0, 1, 0)
        Dim offsetY As Integer = If((fb And FLAG_YOffset) <> 0, 1, 0)
        Return (arrFB(x - offsetX, y - offsetY, z) \ 1000) - 1
        'Auf ein unbelegtes Feld angewendet passiert folgendes:
        'offsetX und Offset Y sind 0, d.h. x und y werden nicht verändert. 
        'Der Index  (arrFB(x - offsetX, y - offsetY, z) \ 1000) ist auch 0
        '1 ab, gibt minus 1. ==> es muss nicht extra auf arrFB(x, y, z) = 0 geprüft werden,
        'ob ein Feld leer ist, es kann immer gleich nach dem SteinIndex gefragt werden.
    End Function
    '
    ''' <summary>
    ''' Diese Überladung holt des Index direkt aus dem arrFB unter Berücksichtigung der OffsetXY
    ''' Siehe die erste Überladung.
    ''' VORSICHT, zum Iterieren IsIndexQuadrant verwenden.
    ''' </summary>
    ''' <param name="tripl"></param>
    ''' <returns></returns>'
    Public Function GetIndexStein(tripl As Triple) As Integer
        'Weil zeitkritisch doppelter Code
        With tripl
            Dim fb As Integer = arrFB(.x, .y, .z)
            Dim offsetX As Integer = If((fb And FLAG_XOffset) <> 0, 1, 0)
            Dim offsetY As Integer = If((fb And FLAG_YOffset) <> 0, 1, 0)
            Return (arrFB(.x - offsetX, .y - offsetY, .z) \ -1)
        End With
    End Function

    ''' <summary>
    ''' Den Index und zur Prüfung, ob einer der Quadranten zu einem Stein gehört.
    ''' VORSICHT, zum Iterieren auf der Suche nach IndexStein IsIndexQuadrant verwenden.
    ''' 
    ''' </summary>
    ''' <param name="fb"></param>
    ''' <returns></returns>
    Public Shared Function GetIndexStein(fb As Integer) As Integer
        'Der Index wird um 1 erhöht gespeichert, weil index = 0 gleichbedeutend
        'wäre mit "freier Platz"
        Return (fb \ 1000) - 1  ' Wert ab dritter Dezimalstelle (ab Hunderterstelle)
    End Function

    ''' <summary>
    ''' Prüft, ob an der Stelle ein Felbeschreiber FB ist, der einen SteinIndex enthält.
    ''' Kann zum Iterieren durch das Feld benutzt werden.
    ''' </summary>
    ''' <param name="fb"></param>
    ''' <returns></returns>
    Public Shared Function IsIndexQuadrant(fb As Integer) As Boolean
        'Der Index wird um 1 erhöht gespeichert, weil index = 0 gleichbedeutend
        'wäre mit "freier Platz" hier wird die 1 nicht abgezogen!
        'so ist sichergestellt, daß nur True ist bei den linken oberen Quadranten
        'und die anderen drei Quadranten nicht berücksichtigt werden.
        'Quadranten 
        Return (fb \ 1000) > 0  ' Wert ab dritter Dezimalstelle
    End Function

    ''' <summary>
    ''' Prüft, ob an der Stelle ein Felbeschreiber FB ist, der einen SteinIndex enthält.
    ''' Kann zum Iterieren durch das Feld benutzt werden.
    ''' </summary>
    Public Function IsIndexQuadrant(tripl As Triple) As Boolean
        'Der Index wird um 1 erhöht gespeichert, weil index = 0 gleichbedeutend
        'wäre mit "freier Platz" hier wird die 1 nicht abgezogen!
        'so ist sichergestellt, daß nur True ist bei den linken oberen Quadranten
        'und die anderen drei Quadranten nicht berücksichtigt werden.
        'Quadranten 
        With tripl
            Dim fb As Integer = arrFB(.x, .y, .z)
            Return (fb \ 1000) > 0  ' Wert ab dritter Dezimalstelle
        End With

    End Function

    Public Shared Function GetOffsetX(fb As Integer) As Integer
        Return If((fb And FLAG_XOffset) <> 0, 1, 0)
    End Function

    Public Shared Function GetOffsetY(fb As Integer) As Integer
        Return If((fb And FLAG_YOffset) <> 0, 1, 0)
    End Function
    '
    ''' <summary>
    ''' Holt den Wert des ToggleFlags koorigiert um den OffsetXY direkt aus dem arrFB 
    ''' </summary>
    ''' <param name="x"></param>
    ''' <param name="y"></param>
    ''' <param name="z"></param>
    ''' <returns></returns> '
    Public Function GetToggleFlag(x As Integer, y As Integer, z As Integer) As Boolean
        'Weil zeitkritisch doppelter Code
        Dim fb As Integer = arrFB(x, y, z)
        Dim offsetX As Integer = If((fb And FLAG_XOffset) <> 0, 1, 0)
        Dim offsetY As Integer = If((fb And FLAG_YOffset) <> 0, 1, 0)
        Return (arrFB(x - offsetX, y - offsetY, z) And FLAG_ToggleFlag) <> 0
    End Function
    '
    ''' <summary>
    ''' Holt den Wert des ToggleFlags koorigiert um den OffsetXY direkt aus dem arrFB
    ''' </summary>
    ''' <param name="tripl"></param>
    ''' <returns></returns> '
    Public Function GetToggleFlag(tripl As Triple) As Boolean
        'Weil zeitkritisch doppelter Code
        With tripl
            Dim fb As Integer = arrFB(.x, .y, .z)
            Dim offsetX As Integer = If((fb And FLAG_XOffset) <> 0, 1, 0)
            Dim offsetY As Integer = If((fb And FLAG_YOffset) <> 0, 1, 0)
            Return (arrFB(.x - offsetX, .y - offsetY, .z) And FLAG_ToggleFlag) <> 0
        End With
    End Function
    '
    ''' <summary>
    ''' Seperate Gewinnung des Wertes.
    ''' </summary>
    ''' <param name="fb"></param>
    ''' <returns></returns>
    Public Function GetToggleFlag(fb As Integer) As Boolean
        Return (fb And FLAG_ToggleFlag) <> 0
    End Function
    '
    ''' <summary>
    ''' Zum Syncronisieren von ToggleFlag und VergleichsToggleFlag muss das
    ''' VergleichsToggleFlag auf den Zustand im Feld gesetz werden. (sicherheitshalber)
    ''' Dazu wird das erste Toggleflag im Feld gesucht und ausgelesen.
    ''' </summary>
    ''' <returns></returns> '
    Public Function GetFirstToggleFlagValue() As Boolean

        For z As Integer = 0 To zMax 'auf der untersten Ebene beginnen. (da wird immer was gefunden)
            For x As Integer = 0 To xMax
                For y As Integer = 0 To yMax
                    Dim fb As Integer = arrFB(x, y, z)
                    If fb <> 0 Then
                        'das erste gefundene belegte Feld auswerten
                        Dim offsetX As Integer = If((fb And FLAG_XOffset) <> 0, 1, 0)
                        Dim offsetY As Integer = If((fb And FLAG_YOffset) <> 0, 1, 0)
                        Return (arrFB(x - offsetX, y - offsetY, z) And FLAG_ToggleFlag) <> 0
                    End If
                Next
            Next
        Next
        Return False 'leeres Feld
    End Function

    '
    'Hinweis: Die Setter-Prozeduren sind ByRef, damit fb geändert wird.
    Public Sub SetOffsetX(ByRef fb As Integer, value0or1 As Integer)
        If value0or1 <> 0 Then
            fb = fb Or FLAG_XOffset
        Else
            fb = fb And Not FLAG_XOffset
        End If
    End Sub

    Public Sub SetOffsetX(ByRef fb As Integer, value As Boolean)
        If value Then
            fb = fb Or FLAG_XOffset
        Else
            fb = fb And Not FLAG_XOffset
        End If
    End Sub

    Public Sub SetOffsetY(ByRef fb As Integer, value0or1 As Integer)
        If value0or1 <> 0 Then
            fb = fb Or FLAG_YOffset
        Else
            fb = fb And Not FLAG_YOffset
        End If
    End Sub

    Public Sub SetOffsetY(ByRef fb As Integer, value As Boolean)
        If value Then
            fb = fb Or FLAG_YOffset
        Else
            fb = fb And Not FLAG_YOffset
        End If
    End Sub
    '
    Public Sub SetToggleFlag(x As Integer, y As Integer, z As Integer, value As Boolean)
        'Weil zeitkritisch doppelter Code
        Dim fb As Integer = arrFB(x, y, z)
        Dim offsetX As Integer = If((fb And FLAG_XOffset) <> 0, 1, 0)
        Dim offsetY As Integer = If((fb And FLAG_YOffset) <> 0, 1, 0)
        If value Then
            arrFB(x - offsetX, y - offsetY, z) = arrFB(x - offsetX, y - offsetY, z) Or FLAG_ToggleFlag
        Else
            arrFB(x - offsetX, y - offsetY, z) = arrFB(x - offsetX, y - offsetY, z) And Not FLAG_ToggleFlag
        End If
    End Sub
    '
    Public Sub SetToggleFlag(tripl As Triple, value As Boolean)
        'Weil zeitkritisch doppelter Code
        With tripl
            Dim fb As Integer = arrFB(.x, .y, .z)
            Dim offsetX As Integer = If((fb And FLAG_XOffset) <> 0, 1, 0)
            Dim offsetY As Integer = If((fb And FLAG_YOffset) <> 0, 1, 0)
            If value Then
                arrFB(.x - offsetX, .y - offsetY, .z) = arrFB(.x - offsetX, .y - offsetY, .z) Or FLAG_ToggleFlag
            Else
                arrFB(.x - offsetX, .y - offsetY, .z) = arrFB(.x - offsetX, .y - offsetY, .z) And Not FLAG_ToggleFlag
            End If
        End With
    End Sub

    Public Shared Sub SetToggleFlag(ByRef fb As Integer, value As Boolean)
        If value Then
            fb = fb Or FLAG_ToggleFlag
        Else
            fb = fb And Not FLAG_ToggleFlag
        End If
    End Sub

    Public Shared Sub SetIndexStein(ByRef fb As Integer, value As Integer)
        ' Wert ab vierten Dezimalstelle setzen (ab Tausenderstelle)
        ' Flags in den unteren Bits behalten
        Dim flags As Integer = fb And &HFF  ' Bits 0-15, Dezimal 0 bi 255
        'Der Index wird um 1 erhöht gespeichert, weil index = 0 gleichbedeutend
        'wäre mit "freier Platz" (zumindest, wenn flags = 0, was möglich ist.)
        fb = (value + 1) * 1000 + flags
    End Sub
    '
    Public Sub ToggleToggleFlag(x As Integer, y As Integer, z As Integer)
        'Weil zeitkritisch doppelter Code
        Dim fb As Integer = arrFB(x, y, z)
        Dim offsetX As Integer = If((fb And FLAG_XOffset) <> 0, 1, 0)
        Dim offsetY As Integer = If((fb And FLAG_YOffset) <> 0, 1, 0)
        arrFB(x - offsetX, y - offsetY, z) = arrFB(x - offsetX, y - offsetY, z) Xor FLAG_ToggleFlag
    End Sub
    '
    Public Sub ToggleToggleFlag(tripl As Triple)
        'Weil zeitkritisch doppelter Code
        With tripl
            Dim fb As Integer = arrFB(.x, .y, .z)
            Dim offsetX As Integer = If((fb And FLAG_XOffset) <> 0, 1, 0)
            Dim offsetY As Integer = If((fb And FLAG_YOffset) <> 0, 1, 0)
            arrFB(.x - offsetX, .y - offsetY, .z) = arrFB(.x - offsetX, .y - offsetY, .z) Xor FLAG_ToggleFlag
        End With
    End Sub

    Public Shared Sub ToggleToggleFlag(ByRef fb As Integer)
        fb = fb Xor FLAG_ToggleFlag
    End Sub


#End Region

#Region "Update-Funktionen"

    Private Sub UpdateWindsAreInOneClickGroup(enabled As Boolean)

    End Sub

#End Region

#Region "Statistik"

    Public Sub ShowMessageBoxStatistik()

        Dim steineVorrat As New List(Of SteinIndexEnum)
        Dim steineSpielfeld As New List(Of SteinIndexEnum)

        If SteinInfos IsNot Nothing AndAlso SteinInfos.Count > 0 Then
            'Steine einsammeln
            For Each item As SteinInfo In SteinInfos
                steineSpielfeld.Add(item.SteinIndex)
            Next
        End If



        '  Public SteinInfos As List(Of SteinInfo) = Nothing


    End Sub

#End Region


#Region "Debug-Funktionen"
    '
    ''' <summary>
    ''' Die Funktion arbeitet nur, wenn Debugger.IsAttached, also nur innerhalb der IDE.
    ''' Prüft, ob alle Randfelder auf 0 sind. Wenn nicht, ist ein Stein falsch
    ''' eingetragen worden. Programmierfehler! Die Funktion wirft dann eine Exception,
    ''' </summary>
    Public Sub CheckRand()

        If Not Debugger.IsAttached Then
            Exit Sub
        End If

        ' Prüft den Rand in X- und Y-Richtung für alle Z-Ebenen
        For z As Integer = 0 To zMax
            ' Linker Rand (x=0)
            For y As Integer = 0 To yMax + 1
                If arrFB(0, y, z) <> 0 Then
                    Throw New Exception($"Programmierfehler: Linker Rand ist nicht 0 in x=0 y={y} z={z}")
                End If
            Next

            ' Rechter Rand (x = xMax+1)
            For y As Integer = 0 To yMax + 1
                If arrFB(xMax + 1, y, z) <> 0 Then
                    Throw New Exception($"Programmierfehler: Rechter Rand ist nicht 0 in x={xMax + 1} y={y} z={z}")
                End If
            Next

            ' Oberer Rand (y = 0)
            For x As Integer = 0 To xMax + 1
                If arrFB(x, 0, z) <> 0 Then
                    Throw New Exception($"Programmierfehler: Oberer Rand ist nicht 0 in x={x} y=0 z={z}")
                End If
            Next

            ' Unterer Rand (y = yMax+1)
            For x As Integer = 0 To xMax + 1
                If arrFB(x, yMax + 1, z) <> 0 Then
                    Throw New Exception($"Programmierfehler: Unterer Rand ist nicht 0 in x={x} y={yMax + 1} z={z}")
                End If
            Next
        Next
    End Sub

#End Region


#Region "Load, Save"

    '################################################################################################

    ''' <summary>
    ''' zLwpD = myIO.GetPathInAnwendungsdatenII("Suche", GetType(IndexData).Name + ".xml")
    ''' </summary>
    ''' <returns></returns>
    Public Shared Function Load(zLwpD As String) As SpielfeldInfo

        If IO.File.Exists(zLwpD) Then
            Try
                Using reader As New IO.StreamReader(zLwpD)
                    Dim serializer As New Xml.Serialization.XmlSerializer(GetType(SpielfeldInfo))
                    Dim daten As SpielfeldInfo = CType(serializer.Deserialize(reader), SpielfeldInfo)
                    Return daten

                End Using
            Catch
                ' Fehler beim Laden – neue Instanz zurückgeben
            End Try
        End If

        Dim neu As New SpielfeldInfo()
        Return neu

    End Function

    Public Sub Save(zLwpD As String)

        XmlInfo_SteinInfo_Count = SteinInfos.Count
        Try
            Using writer As New IO.StreamWriter(zLwpD)
                Dim serializer As New Xml.Serialization.XmlSerializer(Me.GetType())
                serializer.Serialize(writer, Me)
            End Using
        Catch ex As Exception
            'TODO Fehlerbehandlung
        End Try
    End Sub

#End Region

End Class
