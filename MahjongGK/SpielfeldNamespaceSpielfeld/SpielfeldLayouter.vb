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
#Disable Warning IDE0079
#Disable Warning IDE1006


Namespace Spielfeld
    Module SpielfeldLayouter

        '
        Public Sub UpdateSpielfeld(outputRect As Rectangle, aktRenderingChanged As Boolean, Optional forceUpdate As Boolean = False)

            Dim saveToTmpVerz As Boolean = False

            'If SFD.ToolboxTabPageChanged Then
            '    Stop
            'End If

            'Auf eine Änderung von AktRendering reagieren
            Dim changed As Boolean = False
            Dim createFrozenBitmap As Boolean = (SFD.rxOutput IsNot Nothing AndAlso Not SFD.rxOutput.IsEmpty) 'es gibt ein gültiges Rect, d.h. es wurde bereits gerendert


            If SFD.AktSpielfeldInfo IsNot Nothing AndAlso aktRenderingChanged Then
                changed = True
            Else
                Select Case SFD.AktRendering
                    Case RenderingEnum.Spielfeld
                        If Not IsNothing(SFD.SpielfeldSpielfeldInfo) Then
                            If Not SFD.SpielfeldSpielfeldInfo.IsEqual(SFD.AktSpielfeldInfo) Then
                                SFD.AktSpielfeldInfo = SFD.SpielfeldSpielfeldInfo
                                changed = True
                            End If
                        End If

                    Case RenderingEnum.Editor
                        If Not IsNothing(SFD.SpielfeldSpielfeldInfo) Then
                            If Not SFD.SpielfeldSpielfeldInfo.IsEqual(SFD.AktSpielfeldInfo) Then
                                SFD.AktSpielfeldInfo = SFD.SpielfeldSpielfeldInfo
                                changed = True
                            End If
                        End If

                    Case RenderingEnum.Werkbank
                        If Not IsNothing(SFD.WerkbankSpielfeldInfo) Then
                            If Not SFD.WerkbankSpielfeldInfo.IsEqual(SFD.AktSpielfeldInfo) Then
                                SFD.AktSpielfeldInfo = SFD.WerkbankSpielfeldInfo
                                changed = True
                            End If
                        End If
                End Select
            End If

            '
            If IsNothing(SFD.AktSpielfeldInfo) Then
                Exit Sub
            End If
            '
            If changed Or forceUpdate Then
                SFD.rxOutput = outputRect
            Else
                If SFD.rxOutput = outputRect Then
                    Exit Sub
                Else
                    SFD.rxOutput = outputRect
                End If
            End If


            If SFD.AktSpielfeldInfo.IsEmpty Then
                Exit Sub
            End If

            If changed AndAlso SFD.AktRendering <> RenderingEnum.None Then
                ' Prüfen, ob gespeichert werden soll ins Temporär-Verzeichnis.
                ' Annahme: SFD.AktSpielfeldInfo.Ident ist die "neu beobachtete" ID.
                Dim identNew As String = SFD.AktSpielfeldInfo.PersistentIdent

                If String.IsNullOrEmpty(SFD.AktSpielfeldInfo_Ident) Then
                    ' Erstes Mal: Akt noch leer -> einmalig speichern
                    SFD.AktSpielfeldInfo_Ident = identNew
                    ' Last bleibt Nothing/leer
                    saveToTmpVerz = True

                ElseIf identNew = SFD.AktSpielfeldInfo_Ident Then
                    ' Keine Änderung
                    saveToTmpVerz = False

                ElseIf identNew = SFD.LastSpielfeldInfo_Ident Then
                    ' Sonderfall: Hin- und Her zwischen zwei bekannten IDs
                    ' (Aktuelle Daten von Spielfeld/Editor und Werkbank)
                    ' -> NICHT speichern, nur die Historie spiegeln/aktualisieren
                    Dim tmp As String = SFD.AktSpielfeldInfo_Ident
                    SFD.AktSpielfeldInfo_Ident = SFD.LastSpielfeldInfo_Ident
                    SFD.LastSpielfeldInfo_Ident = tmp
                    saveToTmpVerz = False

                Else
                    ' ECHTER Wechsel auf eine neue, bisher unbekannte ID
                    SFD.LastSpielfeldInfo_Ident = SFD.AktSpielfeldInfo_Ident
                    SFD.AktSpielfeldInfo_Ident = identNew
                    saveToTmpVerz = True
                End If
            End If
            '
            With SFD.AktSpielfeldInfo
                SFD.xMax = .xMax 'das sind die Felder
                SFD.yMax = .yMax
                SFD.zMax = .zMax
                SFD.xMaxSteine = SFD.xMax \ 2 'jeder Stein belegt 2 Felder --> \ 2
                SFD.yMaxSteine = SFD.yMax \ 2
                SFD.zMaxSteine = .zMax + 1
            End With

            INI.RaiseAllIniEvents()

            If SFD.AktRendering <> RenderingEnum.Werkbank Then
                If SFD.xMaxSteine < MJ_STEINE_SIDEBYSIDE_MIN OrElse SFD.yMaxSteine < MJ_STEINE_OVERANOTHER_MIN Then
                    If Debugger.IsAttached And Not IfRunningInIDE_ShowErrorMsgInsteadOfException Then
                        Throw New Exception($"Spielfeld Dimensionierung zu klein (xMax={SFD.xMax},yMax={SFD.yMax}) in SpielfeldManager.UpdateSpielfeld")
                    Else
                        MsgBox("Fehler in der Spielfelddimensionierung.||Wählen Sie ein anderes Spiel.", MsgBoxStyle.Critical)
                        PaintSpielfeld_DeInitialisierung()
                        Exit Sub
                    End If
                End If
                If SFD.xMaxSteine > MJ_STEINE_MAXX_SIDEBYSIDE OrElse SFD.yMaxSteine > MJ_STEINE_MAXY_OVERANOTHER OrElse SFD.zMax > MJ_STEINE_MAXZ_LAYER Then
                    If Debugger.IsAttached And Not IfRunningInIDE_ShowErrorMsgInsteadOfException Then
                        Throw New Exception($"Spielfeld Dimensionierung zu zu groß (xMax={SFD.xMax},yMax={SFD.yMax},zMax={SFD.zMax})) in SpielfeldManager.UpdateSpielfeld")
                    Else
                        MsgBox("Fehler in der Spielfelddimensionierung.||Wählen Sie ein anderes Spiel.", MsgBoxStyle.Critical)
                        PaintSpielfeld_DeInitialisierung()
                        Exit Sub
                    End If
                End If
            End If

            If SFD.AktRendering = RenderingEnum.Spielfeld Then
                'Es ist übersichtlicher die einzelnen Layouts jeweils komplett gesondert zu
                'bearbeiten und nicht nach Gemeinsamkeiten zu sortieren.
                'spätestens bei Änderungen geht es einfacher.
                'Daher zunächst, ganz übersichtlich und kurz:
                '
                Select Case INI.Spielbetrieb_PositionHistory
                    Case 0
                        SFD.AktLayout = Layout.SplfldWithHeaderAndHistNone
                    Case 2
                        SFD.AktLayout = Layout.SplfldWithHeaderAndHistRight
                    Case Else
                        SFD.AktLayout = Layout.SplfldWithHeaderAndHistLeft
                End Select

            ElseIf SFD.AktRendering = RenderingEnum.Editor Then
                SFD.AktLayout = Layout.EditorWithHeader

                'Initialisierung den Spielsteingenerators
                With SFD.AktSpielfeldInfo
                    If IsNothing(.Generator) Then
                        If IsNothing(.GeneratorValuesForXml) Then
                            'TODO so umbauen, daß der Default nur genommen wird, wenn nichts Anderes festgelegt wurde
                            If INI.SpielsteinGenerator_GeneratorModusDefault = GeneratorModus.None Then
                                .Generator = New SpielsteinGenerator(INI.SpielsteinGenerator_GeneratorModusDefault)
                            Else
                                .Generator = New SpielsteinGenerator(INI.SpielsteinGenerator_GeneratorModusDefault)
                            End If
                        Else
                            .Generator = New SpielsteinGenerator
                            'Das sind die bisherigen Werte, die beim Speichern von Spielfeldinfo gespeichert werden.
                            .GeneratorValuesForXml.CopySpielsteinGeneratorValues_To_SpielsteinGenerator(.Generator)
                        End If
                    End If
                End With

            ElseIf SFD.AktRendering = RenderingEnum.Werkbank Then
                SFD.AktLayout = Layout.WerkbankWithHeader
            Else ' SFD.AktRendering = RenderingEnum.None Then
                SFD.AktLayout = Layout.None
            End If


            'Die Steinabmessungen werden interativ berechnet.
            'Sie sind abhängig von der Feldgröße in Steinen, der Feldgröße in Pixeln
            'und der Feldhöhe in Pixelverschiebungen je Stein und Ebene und ob oben die Vorratssteine sind.
            'Zusätzlich fließen noch die Paddings rings um das Ausgaberechteck in die Berechnung ein.
            '
            'Startwerte:

            SFD.steinSize = New Size(INI.Rendering_OrgGrafikSizeWidth, INI.Rendering_OrgGrafikSizeHeight)

            CreateMainLayoutIterativStep()

            'die "+ 2" ergeben sich aus: Wert aufrunden + 1.
            SFD.steinSize = New Size(SFD.rxStageAvailable.Width \ SFD.xMaxSteine + 2, SFD.rxStageAvailable.Height \ SFD.yMaxSteine + 2)
            '
            Dim summeWidth As Integer
            Dim summeHeight As Integer

            Dim steinSizeIteration As Size
            Dim shrink As Integer = -1

            Do
                shrink += 1

                steinSizeIteration = GetNewSteinSize(shrink)

                SFD.offset3DLeftJeEbene = CInt(Math.Max(INI.Rendering_Offset3DFaktorAbsolutX * steinSizeIteration.Width, CDbl(INI.Rendering_Offset3DMinPerLayerX)))
                SFD.offset3DTopJeEbene = CInt(Math.Max(INI.Rendering_Offset3DFaktorAbsolutY * steinSizeIteration.Height, CDbl(INI.Rendering_Offset3DMinPerLayerY)))

                SFD.offset3DLeftSumme = SFD.offset3DLeftJeEbene * SFD.zMax
                SFD.offset3DTopSumme = SFD.offset3DTopJeEbene * SFD.zMax

                summeWidth = steinSizeIteration.Width * SFD.xMaxSteine + SFD.offset3DLeftSumme
                summeHeight = steinSizeIteration.Height * SFD.yMaxSteine + SFD.offset3DTopSumme


                SFD.steinSize = New Size(steinSizeIteration.Width, steinSizeIteration.Height)

                'SFD.rxStageAvailable wird hier berechnet in Abhängigkeit von SFD.steinSize und aller anderen Variablen
                CreateMainLayoutIterativStep()

                If summeWidth <= SFD.rxStageAvailable.WidthInside AndAlso summeHeight <= SFD.rxStageAvailable.HeightInside Then
                    Exit Do
                End If
            Loop

            'Zusätzlicher Aufruf außerhalb der Schreife zum Debuggen:
            'CreateMainLayoutIterativStep(fDoStop:=True) 

            'getestet mit einem Spielfeld mit 75 X 25 X 25 Steinen
            'und einem RenderRect mit 344 x 194 Pixeln
            'und einem MJ_SPIELFELD_MIN_WIDTH MJ_SPIELFELD_MIN_HEIGHT von 600 x 400 Pixeln.
            'Das läuft.
            ''Auf das Spielfeld passen 75 x 25 x 25 = 46,875 Steine (und der Computer schafft die Anzeige.) 
            '
            SFD.rxStageUsed = SFD.rxStageAvailable.GetRectangleXInside(summeWidth, summeHeight, align:=RectangleX.Align.Center, usePadding:=True)

            If Debugger.IsAttached Then

            End If

            SFD.rxStageUsed?.SetDrawBoundsAndContentDebug(Style)
            ' Initialisierung der RectangleX-Felder/Eigenschaften mit Debug-Namen
            ' Nur für Debug/Diagnose
            RectangleXDebugBinder.BindNames(SFD)

            If SFD.steinWidthLastCreated <> SFD.steinWidth OrElse SFD.steinHeightLastCreated <> SFD.steinHeight Then
                ' BitmapContainer.ChangeImagesSize(SFD.steinWidth, SFD.steinHeight)

                SFD.steinWidthLastCreated = SFD.steinWidth
                SFD.steinHeightLastCreated = SFD.steinHeight
            End If

            SFD.offset3DLeftJeEbene *= INI.Rendering_Offset3DFaktorSignX
            SFD.offset3DTopJeEbene *= INI.Rendering_Offset3DFaktorSignY

            If SFD.offset3DLeftJeEbene < 0 Then
                SFD.offset3DLeftSumme = 0
            End If
            If SFD.offset3DTopJeEbene < 0 Then
                SFD.offset3DTopSumme = 0
            End If



            'With SFD.AktSpielfeldInfo
            '    If IsNothing(.BitmapUGrdImgCache) Then
            '        If .HasBitmapUGrd Then
            '            .BitmapUGrdImgCache = New BackgroundSingleImageCache(BackgroundBitmapCache)
            '            .BitmapUGrdImgCache.LoadBitmap(.BitmapUGrdFullpath, BackgroundSingleImageCache.BackgroundRenderMode.CoverCrop)
            '        Else
            '            With SFD.AktSpielfeldInfo
            '                Dim bitmapUGrdFullpath As String = IO.Path.Combine(AppDataDirectory(AppDataSubDir.Hintergrundgrafiken),
            '                If(INI.Global_DarkMode, "water_3007467.jpg", "watercolor_2323195.jpg"))
            '                .BitmapUGrdFullpath = bitmapUGrdFullpath
            '                .BitmapUGrdImgCache = New BackgroundSingleImageCache(BackgroundBitmapCache)
            '                .BitmapUGrdImgCache.LoadBitmap(bitmapUGrdFullpath, BackgroundSingleImageCache.BackgroundRenderMode.PreserveOrgSize)
            '            End With
            '        End If
            '    End If
            'End With

            Select Case SFD.AktLayout
                Case Layout.None
                    SFD.HScrollStock = Nothing
                Case Layout.SplfldWithHeaderAndHistNone
                    SFD.HScrollStock = Nothing
                Case Layout.SplfldWithHeaderAndHistRight
                    SFD.HScrollStock = Nothing
                Case Layout.SplfldWithHeaderAndHistLeft
                    SFD.HScrollStock = Nothing
                Case Layout.EditorWithHeader
                    SFD.HScrollStock = New HScrollRenderer(SFD.rxStockScrollbar.ToRectangle, basisColor:=Color.Blue)

                Case Layout.WerkbankWithHeader
                    SFD.HScrollStock = Nothing
                Case Else
                    Stop 'Programmierfehler: Enum hinzugekommen
            End Select

            If saveToTmpVerz Then
                SFD.AktSpielfeldInfo.Save()
            End If
        End Sub

        ' Hinweis:
        ' - Es wird vorausgesetzt, dass die Konstanten
        '   MJ_GRAFIK_SRC_MIN_WIDTH_OR_HEIGHT,
        '   MJ_GRAFIK_SRC_MAX_WIDTH_OR_HEIGHT und
        '   MJ_GRAFIK_STEIN_MIN_WIDTH_OR_HEIGHT
        '   sowie die INI-Werte (OrgGrafikSizeWidth/-Height,
        '   OrgGrafikReferenceSizeWidth/-Height, UseGrafikOrgSize)
        '   im gleichen Projekt/Namespace verfügbar sind.
        '   Hinweise zur Verwendung in der Do-Loop

        'Du erhöhst shrink iterativ, rufst GetNewBitmapSize(shrink) auf und prüfst nach jeder Iteration,
        'ob die berechnete Steingröße zu deinen aktuellen Spielfeld-Randbedingungen passt (Spielfläche In Pixeln, max. Steinanzahl, etc.).

        'Sobald die Größe passt, verwendest du die zurückgegebenen (width, height).

        'Falls du zusätzliche Obergrenzen für das Ergebnis brauchst (nicht nur für die Quellmaße),
        'kannst du vor Schritt 5 noch eigene Caps setzen.

        ''' <summary>
        ''' Liefert eine neue Bitmap-Größe ausgehend von Original/Referenzmaßen,
        ''' indem vom längeren Maß der Wert <paramref name="shrink"/> abgezogen wird.
        ''' Das Seitenverhältnis bleibt erhalten. Ergebnisse werden auf die nächstkleinere
        ''' gerade Integerzahl abgerundet. Unterschreitet eine Seite das Mindestmaß,
        ''' werden **beide** Seiten auf das Mindestmaß gesetzt.
        ''' </summary>
        Public Function GetNewSteinSize(shrink As Integer) As Size
            ' --- 1) Ausgangsmaße bestimmen (Double) ------------------------------------------
            '     a) Falls UseGrafikOrgSize = True -> Originalmaße verwenden
            '     b) Sonst Referenzmaße; wenn eine Seite < SRC_MIN, dann auf Original ausweichen
            '     c) Beide Seiten am Ende auf SRC_MAX deckeln

            Dim baseW As Double
            Dim baseH As Double

            If INI.Rendering_UseGrafikOrgSize Then
                baseW = CDbl(INI.Rendering_OrgGrafikSizeWidth)
                baseH = CDbl(INI.Rendering_OrgGrafikSizeHeight)
            Else
                Dim refW As Integer = INI.Rendering_OrgGrafikReferenceSizeWidth
                Dim refH As Integer = INI.Rendering_OrgGrafikReferenceSizeHeight

                If refW < MJ_GRAFIK_SRC_MIN_WIDTH_OR_HEIGHT OrElse refH < MJ_GRAFIK_SRC_MIN_WIDTH_OR_HEIGHT Then
                    ' Referenz zu klein -> Original nehmen
                    baseW = CDbl(INI.Rendering_OrgGrafikSizeWidth)
                    baseH = CDbl(INI.Rendering_OrgGrafikSizeHeight)
                ElseIf refW > MJ_GRAFIK_SRC_MAX_WIDTH_OR_HEIGHT OrElse refH > MJ_GRAFIK_SRC_MAX_WIDTH_OR_HEIGHT Then
                    ' Referenz zu groß -> Original nehmen
                    baseW = CDbl(INI.Rendering_OrgGrafikSizeWidth)
                    baseH = CDbl(INI.Rendering_OrgGrafikSizeHeight)
                Else
                    baseW = CDbl(refW)
                    baseH = CDbl(refH)
                End If
            End If

            ' --- 2) Seitenverhältnis und längere Seite bestimmen ----------------------------
            '      Hinweis: baseW/baseH sind jetzt valide (>= SRC_MIN, <= SRC_MAX) oder wie Original
            Dim ratio As Double ' width / height
            If baseH > 0 Then
                ratio = baseW / baseH
            Else
                ' Sicherheitsnetz; sollte praktisch nie auftreten
                ratio = 1.0R
            End If

            Dim widthIsLonger As Boolean = (baseW >= baseH)
            Dim longSide As Double = If(widthIsLonger, baseW, baseH)

            ' --- 3) Ziel-Längenseite um shrink verkleinern ----------------------------------
            '      (Kann theoretisch <= 0 werden; wird durch Mindestmaß später abgefangen.)
            Dim targetLong As Double = longSide - CDbl(shrink)
            If targetLong < 0 Then targetLong = 0

            ' --- 4) Zweite Seite proportional berechnen -------------------------------------
            Dim targetW As Double
            Dim targetH As Double

            If widthIsLonger Then
                ' Breite war die längere Seite
                targetW = targetLong
                ' H = W / (W/H) = W / ratio
                If ratio > 0 Then
                    targetH = targetW / ratio
                Else
                    targetH = targetW ' Fallback
                End If
            Else
                ' Höhe war die längere Seite
                targetH = targetLong
                ' W = H * (W/H) = H * ratio
                targetW = targetH * ratio
            End If

            ' --- 5) Auf nächste gerade Integerzahl ABRUNDEN ---------------------------------
            Dim iW As Integer = FloorToEvenInt(targetW)
            Dim iH As Integer = FloorToEvenInt(targetH)

            ' --- 6) Mindestmaß-Prüfung -------------------------------------------------------
            ' Wenn eine Seite unter das Mindestmaß fällt, beide auf Mindestmaß setzen.
            If iW < MJ_GRAFIK_STEIN_MIN_WIDTH_OR_HEIGHT OrElse iH < MJ_GRAFIK_STEIN_MIN_WIDTH_OR_HEIGHT Then
                iW = MJ_GRAFIK_STEIN_MIN_WIDTH_OR_HEIGHT
                iH = MJ_GRAFIK_STEIN_MIN_WIDTH_OR_HEIGHT
            End If

            Return New Size(iW, iH)

        End Function

        ''' <summary>
        ''' Rundet einen Double-Wert ab und liefert die nächstkleinere gerade Integerzahl.
        ''' Beispiel: 101,9 -> 100; 100,0 -> 100; 2,0 -> 2; 1,2 -> 0.
        ''' </summary>
        Private Function FloorToEvenInt(value As Double) As Integer
            Dim n As Integer = CInt(Math.Floor(value))
            If (n And 1) <> 0 Then
                n -= 1 ' auf die nächstkleinere gerade Zahl
            End If
            Return n
        End Function


        ''' <summary>
        ''' Setzt die einzelnen Bereiche, außer splfldUsedRect
        ''' </summary>
        Private Sub CreateMainLayoutIterativStep(Optional fDoStop As Boolean = False)

            ' 1.) SFD.rxOutput ist gegeben.
            ' 2.) Der Renderer fragt SFD.AktRendering nicht ab. 
            '     Er fragt ab, ob ein rx... Nothing ist, deshalb sind alle nicht benötigten rx.. Nothing zu setzen.
            '     daher zunächst:


            SFD.rxStock = Nothing
            SFD.rxStockScrollbar = Nothing
            SFD.rxStockMark = Nothing
            SFD.rxBitmapUgrd = Nothing
            SFD.rxHeader = Nothing
            SFD.rxContent = Nothing
            SFD.rxHistoryBoxLeft = Nothing
            SFD.rxHistoryBoxRight = Nothing
            SFD.rxHistoryBoxLeftScrollbar = Nothing
            SFD.rxHistoryBoxRightScrollbar = Nothing
            SFD.rxStageAvailable = Nothing
            SFD.rxStageUsed = Nothing




            '
            '
            ' Überschreibung zum Debuggen
            'SFD.AktLayout = Layout.SplfldWithHeaderAndHistNone
            '
            '
            'Auf gehts,
            Dim histboxContainerWidth As Integer = 2 * SFD.steinWidth +
                INI.Rendering_HScrollbarHeightVScrollbarWitdh

            Select Case SFD.AktLayout
                Case Layout.SplfldWithHeaderAndHistNone
                    SFD.rxBitmapUgrd = New RectangleX(SFD.rxOutput)
                    SFD.rxHeader = SFD.rxBitmapUgrd.GetRectangleXInside(width:=-1, INI.Rendering_HeaderHeight, align:=RectangleX.Align.Top)
                    SFD.rxContent = SFD.rxBitmapUgrd.GetRectangleXInside(width:=-1, height:=-1, align:=RectangleX.Align.Center)
                    SFD.rxContent.IncTop(SFD.rxHeader.Height + 1)

                    SFD.rxStageAvailable = SFD.rxContent.GetRectangleXInside(width:=-1, height:=-1, align:=RectangleX.Align.Left, usePadding:=True, INI.Rendering_PaddingStageAvailable)

                Case Layout.SplfldWithHeaderAndHistLeft
                    SFD.rxBitmapUgrd = New RectangleX(SFD.rxOutput)
                    SFD.rxHeader = SFD.rxBitmapUgrd.GetRectangleXInside(width:=-1, INI.Rendering_HeaderHeight, align:=RectangleX.Align.Top)
                    SFD.rxContent = SFD.rxBitmapUgrd.GetRectangleXInside(width:=-1, height:=-1, align:=RectangleX.Align.Center)
                    SFD.rxContent.IncTop(SFD.rxHeader.Height + 1)



                    SFD.rxHistoryBoxLeft = SFD.rxContent.GetRectangleXInside(SFD.steinWidth * 2, height:=-1, align:=RectangleX.Align.Left)
                    SFD.rxHistoryBoxLeftScrollbar = SFD.rxContent.GetRectangleXInside(INI.Rendering_HScrollbarHeightVScrollbarWitdh, height:=-1, align:=RectangleX.Align.Left)
                    SFD.rxHistoryBoxLeftScrollbar.IncLeft(SFD.steinWidth * 2 + 1)

                    SFD.rxStageAvailable = SFD.rxContent.GetRectangleXInside(width:=-1, height:=-1, align:=RectangleX.Align.Left, usePadding:=True, INI.Rendering_PaddingStageAvailable)
                    SFD.rxStageAvailable.IncLeft(SFD.steinWidth * 2 + INI.Rendering_HScrollbarHeightVScrollbarWitdh + 2)

                Case Layout.SplfldWithHeaderAndHistRight
                    SFD.rxBitmapUgrd = New RectangleX(SFD.rxOutput)
                    SFD.rxHeader = SFD.rxBitmapUgrd.GetRectangleXInside(width:=-1, INI.Rendering_HeaderHeight, align:=RectangleX.Align.Top)
                    SFD.rxContent = SFD.rxBitmapUgrd.GetRectangleXInside(width:=-1, height:=-1, align:=RectangleX.Align.Center)
                    SFD.rxContent.IncTop(SFD.rxHeader.Height + 1)



                    SFD.rxHistoryBoxRightScrollbar = SFD.rxContent.GetRectangleXInside(INI.Rendering_HScrollbarHeightVScrollbarWitdh, height:=-1, align:=RectangleX.Align.Right)
                    SFD.rxHistoryBoxRight = SFD.rxContent.GetRectangleXInside(SFD.steinWidth * 2, height:=-1, align:=RectangleX.Align.Right)
                    SFD.rxHistoryBoxRight.MoveLeft(INI.Rendering_HScrollbarHeightVScrollbarWitdh + 1)

                    SFD.rxStageAvailable = SFD.rxContent.GetRectangleXInside(width:=-1, height:=-1, align:=RectangleX.Align.Right, usePadding:=True, INI.Rendering_PaddingStageAvailable)
                    SFD.rxStageAvailable.DecRight(SFD.steinWidth * 2 + INI.Rendering_HScrollbarHeightVScrollbarWitdh + 2)


                Case Layout.EditorWithHeader

                    'If fDoStop Then Stop

                    SFD.rxBitmapUgrd = New RectangleX(SFD.rxOutput)
                    SFD.rxHeader = SFD.rxBitmapUgrd.GetRectangleXInside(width:=-1, INI.Rendering_HeaderHeight, align:=RectangleX.Align.Top)
                    SFD.rxContent = SFD.rxBitmapUgrd.GetRectangleXInside(width:=-1, height:=-1, align:=RectangleX.Align.Center)
                    SFD.rxContent.IncTop(SFD.rxHeader.Height + 1)

                    SFD.rxStockMark = SFD.rxContent.GetRectangleXInside(width:=-1, height:=INI.Rendering_StockMarkHeight, align:=RectangleX.Align.Top)

                    SFD.rxStock = SFD.rxContent.GetRectangleXInside(width:=-1, height:=SFD.steinHeight, align:=RectangleX.Align.Top)
                    SFD.rxStock.MoveDown(INI.Rendering_StockMarkHeight + 1)

                    SFD.rxStockScrollbar = SFD.rxContent.GetRectangleXInside(width:=-1, height:=INI.Rendering_HScrollbarHeightVScrollbarWitdh, align:=RectangleX.Align.Top)
                    SFD.rxStockScrollbar.MoveDown(INI.Rendering_StockMarkHeight + SFD.steinHeight + 2)

                    SFD.rxStageAvailable = SFD.rxContent.GetRectangleXInside(width:=-1, height:=-1, align:=RectangleX.Align.Top, usePadding:=True, INI.Rendering_PaddingStageAvailable)
                    SFD.rxStageAvailable.IncTop(INI.Rendering_StockMarkHeight + SFD.steinHeight + INI.Rendering_HScrollbarHeightVScrollbarWitdh + 3)
                   ' SFD.rxStageAvailable.MoveDown(200)

                Case Layout.WerkbankWithHeader
                    SFD.rxBitmapUgrd = New RectangleX(SFD.rxOutput, New PaddingValues)
                    SFD.rxHeader = SFD.rxBitmapUgrd.GetRectangleXInside(width:=-1, INI.Rendering_HeaderHeight, align:=RectangleX.Align.Top)
                    SFD.rxContent = SFD.rxBitmapUgrd.GetRectangleXInside(width:=-1, height:=-1, align:=RectangleX.Align.Center)
                    SFD.rxContent.IncTop(SFD.rxHeader.Height + 1)


                    SFD.rxStageAvailable = SFD.rxContent.GetRectangleXInside(width:=-1, height:=-1, align:=RectangleX.Align.Center)
                Case Else
                    Stop
            End Select



            'If INI.Rendering_UseHistoryBoxLeft AndAlso INI.Rendering_UseHistoryBoxRight Then
            '    SFD.rxHistoryBoxLeft = SFD.rxContent.GetRectangleXInside(width:=-1, height:=-1, usePadding:=True, INI.Rendering_PaddingHistoryBoxLeft)
            '    SFD.rxHistoryBoxRight = SFD.rxContent.GetRectangleXInside(width:=-1, height:=-1, usePadding:=True, INI.Rendering_PaddingHistoryBoxRight)
            '    SFD.rxStageAvailable = SFD.rxContent.GetRectangleXInside(width:=-1, height:=-1, usePadding:=True, INI.Rendering_PaddingContent)
            '    SFD.rxStageAvailable.IncLeft(SFD.rxHistoryBoxLeft.Width)
            '    SFD.rxStageAvailable.IncRight(SFD.rxHistoryBoxRight.Width)

            'ElseIf INI.Rendering_UseHistoryBoxLeft Then
            '    SFD.rxHistoryBoxLeft = SFD.rxContent.GetRectangleXInside(width:=-1, height:=-1, usePadding:=True, INI.Rendering_PaddingHistoryBoxLeft)
            '    SFD.rxStageAvailable = SFD.rxContent.GetRectangleXInside(width:=-1, height:=-1, usePadding:=True, INI.Rendering_PaddingContent)
            '    SFD.rxStageAvailable.IncLeft(SFD.rxHistoryBoxLeft.Width)

            'ElseIf INI.Rendering_UseHistoryBoxRight Then
            '    SFD.rxHistoryBoxRight = SFD.rxContent.GetRectangleXInside(width:=-1, height:=-1, usePadding:=True, INI.Rendering_PaddingHistoryBoxRight)
            '    SFD.rxStageAvailable = SFD.rxContent.GetRectangleXInside(width:=-1, height:=-1, usePadding:=True, INI.Rendering_PaddingContent)
            '    SFD.rxStageAvailable.IncRight(SFD.rxHistoryBoxRight.Width)

            'Else
            '    SFD.rxHistoryBoxLeft = Nothing
            '    SFD.rxHistoryBoxRight = Nothing
            '    SFD.rxStageAvailable = SFD.rxContent.GetRectangleXInside(width:=-1, height:=-1, usePadding:=True, INI.Rendering_PaddingContent)
            'End If





            'ElseIf SFD.AktRendering = RenderingEnum.Editor Then
            '    SFD.rxStageAvailable = New RectangleX(SFD.rxOutput, INI.Rendering_PaddingBitmapUgrd)

            'ElseIf SFD.AktRendering = RenderingEnum.Werkbank Then

            'ElseIf SFD.AktRendering = RenderingEnum.None Then
            ' Stop 'Programmierfehler

            If INI.Rendering_DrawRenderRect Then

                SFD.rxOutput?.SetDrawBoundsAndContentDebug(Style(reset:=True))
                SFD.rxStock?.SetDrawBoundsAndContentDebug(Style)
                SFD.rxStockScrollbar?.SetDrawBoundsAndContentDebug(Style)
                SFD.rxStockMark?.SetDrawBoundsAndContentDebug(Style)
                SFD.rxBitmapUgrd?.SetDrawBoundsAndContentDebug(Style)
                SFD.rxHeader?.SetDrawBoundsAndContentDebug(Style)
                SFD.rxContent?.SetDrawBoundsAndContentDebug(Style)
                SFD.rxHistoryBoxLeft?.SetDrawBoundsAndContentDebug(Style)
                SFD.rxHistoryBoxRight?.SetDrawBoundsAndContentDebug(Style)
                SFD.rxHistoryBoxLeftScrollbar?.SetDrawBoundsAndContentDebug(Style)
                SFD.rxHistoryBoxRightScrollbar?.SetDrawBoundsAndContentDebug(Style)
                SFD.rxStageAvailable?.SetDrawBoundsAndContentDebug(Style)

            End If

            If SFD.rxStageAvailable Is Nothing Then
                Stop 'Programmierfehler
                'SFD.rxStageAvailable muß zugewiesen werden
            End If

        End Sub

        Private Function Style(Optional reset As Boolean = False) As Integer
            'monotoner Vorwärtszähler, der die verschiedenen Linienvarianten
            'der DrawDebug-Linien und Farben in RectangleX verteilt.
            Static value As Integer = -1
            If reset Then value = -1
            value += 1
            Return value
        End Function

        '################################################
        Public Sub UpdateMouseValues(outputRect As Rectangle, somethingMouseDone As Boolean, spielfeldIsUpdated As Boolean)

            If somethingMouseDone = False And spielfeldIsUpdated = False Then
                Exit Sub
            End If

            'If Not IsNothing(SFD.rxStockScrollbar) Then
            If SFD.rxStockScrollbar?.Contains(SFD.MousePolling.MousePos()) Then
                With SFD.MousePolling
                    SFD.HScrollStock.HandleMouseMove(.MousePos.X, .MousePos.Y, SFD.rxStockScrollbar.ContentRect)
                    If .LeftMouseChanged Then
                        If .LeftMousePressed Then
                            SFD.HScrollStock.HandleMouseDown(.MousePos.X, .MousePos.Y, SFD.rxStockScrollbar.ContentRect)
                        Else
                            SFD.HScrollStock.HandleMouseUp()
                        End If
                    End If
                End With
            Else
                SFD.HScrollStock?.HandleMouseUp()
            End If

            'End If
            '  Dim mpos As Point = 

            If Not IsNothing(SFD.HScrollStock) Then

            End If

        End Sub



    End Module

End Namespace
