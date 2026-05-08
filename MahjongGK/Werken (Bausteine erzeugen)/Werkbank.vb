Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On
Imports MahjongGK.Contracts.GlobalEnum

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
#Disable Warning IDE0079
#Disable Warning IDE1006

Public Class Werkbank

#Region "Konstruktor und Initialisierung"

    Sub New()

    End Sub

    ''' <summary>
    ''' spielSize ist die Anzahl der Steine, die maximal nebeneinander und untereinander
    ''' auf dem Feld Platz haben. Z ist die Anzahl der Schichten übereinander.
    ''' </summary>
    ''' <param name="bausteinSize"></param>
    Sub New(bausteinSize As Triple)

        If bausteinSize.x < 1 OrElse bausteinSize.y < 1 OrElse bausteinSize.z < 1 Then
            Throw New Exception("bausteinSize ist die Größe des bausteinSize in Steinen, " &
                                "mindestens 1 Stein breit, 1 Steine tief und 1 Schicht hoch." &
                                bausteinSize.ToString)
        End If

        With bausteinSize
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
        SteinInfos = New List(Of Spielfeld.SteinInfo)

    End Sub

    Public xMin As Integer = 1
    Public yMin As Integer = 1
    Public zMin As Integer = 0
    Public xMax As Integer
    Public yMax As Integer
    Public zMax As Integer

    Public xUBnd As Integer
    Public yUBnd As Integer
    Public zUBnd As Integer

    Public Property SteinInfos As List(Of Spielfeld.SteinInfo) = Nothing
    Public Property arrFB() As Integer(,,)

#End Region

#Region "Werkbank"

    Public ReadOnly Property SteineNeeded() As Integer
        Get
            Return SteinInfos.Count
        End Get
    End Property

    Public ReadOnly Property ResultAsSteinInfosArrFB() As (wbSteinInfos As List(Of Spielfeld.SteinInfo), wbArrFB As Integer(,,))
        Get
            If SteinInfos.Count > 0 Then
                For idx As Integer = 0 To SteinInfos.Count - 1
                    With SteinInfos(idx)
                        .IsWerkbankStein = True
                        'TODO andere Möglichkeit?
                        '.SteinStatusIst = SteinStatus.I09WerkstückZufallsgrafik
                        '.SteinStatusUsed = SteinStatus.I09WerkstückZufallsgrafik
                    End With
                Next
            End If
            Return (SteinInfos, arrFB)
        End Get
    End Property
    Public ReadOnly Property ResultAsWerkstück(demoMode As Boolean) As Werkstück
        Get
            If SteinInfos.Count > 0 Then
                For idx As Integer = 0 To SteinInfos.Count - 1
                    With SteinInfos(idx)
                        If demoMode Then
                            .IsWerkbankStein = False
                            'TODO
                            '.SteinStatusIst = SteinStatus.I01Normal
                            '.SteinStatusUsed = SteinStatus.I01Normal
                        Else
                            .IsWerkbankStein = True
                            '.SteinStatusIst = SteinStatus.I09WerkstückZufallsgrafik
                            '.SteinStatusUsed = SteinStatus.I09WerkstückZufallsgrafik
                        End If
                    End With
                Next
            End If
            Return New Werkstück(SteinInfos, arrFB)
        End Get
    End Property

#End Region

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
    Private Sub CopySteinIndexToSpielfeldPos3DAndSetOffsetXY(infoFBTriple As Triple, SteinInfoIndex As Integer)
        '
        Dim infoFBx As Integer = infoFBTriple.x
        Dim infoFBy As Integer = infoFBTriple.y
        Dim infoFBz As Integer = infoFBTriple.z

        'Vom childFB rechts unter dem infoFB geht es zum infoFB, indem von beiden Koordinaten
        '1 subtrahiert wird. Daher:
        SetOffsetX(arrFB(infoFBx + 1, infoFBy + 1, infoFBz), True)
        SetOffsetY(arrFB(infoFBx + 1, infoFBy + 1, infoFBz), True)
        '
        'Der childFB genau unter dem infoFB zum infoFB geht mit OffsetX = 0 und OffsetY = -1
        SetOffsetX(arrFB(infoFBx, infoFBy + 1, infoFBz), False)
        SetOffsetY(arrFB(infoFBx, infoFBy + 1, infoFBz), True)
        '
        'Vom FB rechts vom infoFB zum infoFB: OffsetX = -1, OffsetY = 0
        SetOffsetX(arrFB(infoFBx + 1, infoFBy, infoFBz), True)
        SetOffsetY(arrFB(infoFBx + 1, infoFBy, infoFBz), False)

        'Der infoFB hat keinen Offset, daher beide 0 (Verweis auf sich selber)
        SetOffsetX(arrFB(infoFBx, infoFBy, infoFBz), False)
        SetOffsetY(arrFB(infoFBx, infoFBy, infoFBz), False)
        '
        SetSteinInfoIndex(arrFB(infoFBx, infoFBy, infoFBz), SteinInfoIndex)

    End Sub

    ''' <summary>
    ''' WICHTIG: HIER FEHLT DIE REFFERENZ AUD DEN arrFB gegenüber der Spielfeld-Version
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
    Public Function AddSteinToSpielfeld(steinIndex As SteinTyp, steinPos3D As Triple, Optional tmpDebug As Integer = 0) As Boolean

        'Der steinInfoIndex wird hier gesichert, obwohl er gleichlautend ist mit dem
        'Index in SteinInfos. Grund: werden später Steine im Editor entfernt, verschieben sich die
        'Indexnummern in SteinInfos und da muss arrFB aktualisert werden. Dazu braucht man den
        '"alten" steinInfoIndex, eben diesen steinInfoIndex.
        'WICHTIG:Hier ist der arrFB und sfb Nothing gesetzt, weil unbekannt.
        Dim newSteinInfo As New Spielfeld.SteinInfo(steinInfoIndex:=SteinInfos.Count, steinIndex, steinPos3D, Nothing, sfd:=Nothing)

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
                    Case ValidePlace.NoFundamentFound   'Zeilenende erreicht.
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
                            AddSteinToSpielfeld(SteinTyp.ErrorSy, tpl)
                            Return False
                        End If

                    Case ValidePlace.Yes, ValidePlace.NoFundamentFound
                        ' FoundResult.NoFundament ist in diesem Fall OK, er wird zum freischwebendem Stein.
                        'SetStein rekursiv, aber mit jetzt gültigen Werten aufrufen
                        AddSteinToSpielfeld(SteinTyp.ErrorSy, tplR)
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

#Region "Fragen an das Spielfeld (ACHTUNG COPY-PASTE-REGION #2)"
    '
    'Diese Region gib es im Spel zweimal: In der SpielfeldInfo und in der Werkbank!
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
            Return New Triple(triple, ValidePlace.OutsideBorder)
        Else
            If IsFreePlace(triple) Then
                If HasFundament(triple) Then
                    Return New Triple(triple, ValidePlace.Yes)
                Else
                    Return New Triple(triple, ValidePlace.NoFundamentFound)
                End If
            Else
                Return New Triple(triple, ValidePlace.Occupied)
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
                    Return New Triple(tpl, ValidePlace.OutsideBorder)
                Else
                    If IsFreePlace(tpl) Then
                        Return New Triple(tpl, ValidePlace.Yes)
                    End If
                End If
                tpl = IncDirection(tpl, direction)
            Loop
        Else
            Dim fFoundFreePlace As Boolean = False
            Do

                If Not tpl.IsInsideSpielfeldBounds(arrFB) Then
                    If fFoundFreePlace Then
                        Return New Triple(tpl, ValidePlace.NoFundamentFound)
                    Else
                        Return New Triple(tpl, ValidePlace.OutsideBorder)
                    End If
                Else
                    Dim fFound As Boolean = IsFreePlace(tpl)

                    If fFound And Not fFoundFreePlace Then
                        fFoundFreePlace = True
                    End If

                    If fFound AndAlso HasFundament(tpl) Then
                        Return New Triple(tpl, ValidePlace.Yes)
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

            Dim tpl2 As New Triple(tpl.x + addX, tpl.y + addY, tpl.z)

            Dim tpl3 As Triple = IsValidePlace(tpl2)

            Select Case tpl3.Valide
                Case ValidePlace.Yes
                    Return tpl3
                Case ValidePlace.NoFundamentFound
                    'nächster Versuch
                Case ValidePlace.OutsideBorder
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
                tpl.Valide = ValidePlace.Yes
            Else
                tpl.Valide = ValidePlace.NoFundamentFound
            End If
        Else
            tpl.Valide = ValidePlace.NoFundamentFound
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

#Region "Helper aus Spielfeldinfo"
    Public Sub SetOffsetX(ByRef fb As Integer, value As Boolean)
        If value Then
            fb = fb Or FLAG_XOffset
        Else
            fb = fb And Not FLAG_XOffset
        End If
    End Sub

    Public Sub SetOffsetY(ByRef fb As Integer, value As Boolean)
        If value Then
            fb = fb Or FLAG_YOffset
        Else
            fb = fb And Not FLAG_YOffset
        End If
    End Sub
    Public Shared Sub SetSteinInfoIndex(ByRef fb As Integer, value As Integer)
        Dim flags As Integer = fb And FB_FLAG_MASK
        'Der Index wird um 1 erhöht gespeichert, weil index = 0 gleichbedeutend
        'wäre mit "freier Platz".
        fb = ((value + 1) << FB_INDEX_SHIFT) Or flags
    End Sub
#End Region

End Class
