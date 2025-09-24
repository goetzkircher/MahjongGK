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
    Module SpielfeldManager

        'Hinweis: Die Daten, die hier nicht deklariert sind
        'stehen alle im Modul "SpielfeldDaten"
        '
        Public Sub UpdateSpielfeld(outputRect As Rectangle, Optional forceUpdate As Boolean = False)

            Dim saveToTmpVerz As Boolean = False


            'Auf eine Änderung von AktRendering reagieren
            Dim changed As Boolean = False
            Dim createFrozenBitmap As Boolean = Not splfldUsedRect.IsEmpty 'es gibt ein gültiges Rect, d.h. es wurde bereits gerendert


            If SFD.AktRendering <> SFD.LastRendering(afterwardsSyncIt:=True) Then
                changed = True
            Else
                Select Case SFD.AktRendering
                    Case RenderingEnum.Spielfeld
                        If Not IsNothing(SFD.SpielerSpielfeldInfo) Then
                            If Not SpielerSpielfeldInfo.IsEqual(SFD.AktSpielfeldInfo) Then
                                SFD.AktSpielfeldInfo = SFD.SpielerSpielfeldInfo
                                changed = True
                            End If
                        End If

                    Case RenderingEnum.Editor
                        If Not IsNothing(SFD.SpielerSpielfeldInfo) Then
                            If Not SpielerSpielfeldInfo.IsEqual(SFD.AktSpielfeldInfo) Then
                                SFD.AktSpielfeldInfo = SFD.SpielerSpielfeldInfo
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
                SFD.outputRect = outputRect
            Else
                If SFD.outputRect = outputRect Then
                    Exit Sub
                Else
                    SFD.outputRect = outputRect
                End If
            End If


            If SFD.AktSpielfeldInfo.IsEmpty Then
                Exit Sub
            End If

            If changed AndAlso SFD.AktRendering <> RenderingEnum.None Then
                ' Prüfen, ob gespeichert werden soll ins Temporär-Verzeichnis.
                ' Annahme: SFD.AktSpielfeldInfo.Ident ist die "neu beobachtete" ID.
                Dim identNew As String = SFD.AktSpielfeldInfo.Ident

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
                SFD.xMaxSteine = xMax \ 2 'jeder Stein belegt 2 Felder --> \ 2
                SFD.yMaxSteine = yMax \ 2
                SFD.zMaxSteine = .zMax + 1
            End With

            INI.RaiseAllIniEvents()

            If SFD.xMaxSteine < MJ_STEINE_SIDEBYSIDE_MIN OrElse yMaxSteine < MJ_STEINE_OVERANOTHER_MIN Then
                If Debugger.IsAttached And Not IfRunningInIDE_ShowErrorMsgInsteadOfException Then
                    Throw New Exception($"Spielfeld Dimensionierung zu klein (xMax={xMax},yMax={yMax}) in SpielfeldManager.UpdateSpielfeld")
                Else
                    MsgBox("Fehler in der Spielfelddimensionierung.||Wählen Sie ein anderes Spiel.", MsgBoxStyle.Critical)
                    PaintSpielfeld_DeInitialisierung()
                    Exit Sub
                End If
            End If
            If SFD.xMaxSteine > MJ_STEINE_MAXX_SIDEBYSIDE OrElse yMaxSteine > MJ_STEINE_MAXY_OVERANOTHER OrElse zMax > MJ_STEINE_MAXZ_LAYER Then
                If Debugger.IsAttached And Not IfRunningInIDE_ShowErrorMsgInsteadOfException Then
                    Throw New Exception($"Spielfeld Dimensionierung zu zu groß (xMax={xMax},yMax={yMax},zMax={zMax})) in SpielfeldManager.UpdateSpielfeld")
                Else
                    MsgBox("Fehler in der Spielfelddimensionierung.||Wählen Sie ein anderes Spiel.", MsgBoxStyle.Critical)
                    PaintSpielfeld_DeInitialisierung()
                    Exit Sub
                End If
            End If


            'Startwerte
            CreateMainLayout(New Size(INI.Rendering_OrgGrafikSizeWidth, INI.Rendering_OrgGrafikSizeHeight))
            SFD.steinWidth = SFD.splfldFullRect.Width \ SFD.xMaxSteine + 2 '+2 = aufrunden + 1
            SFD.steinHeight = SFD.splfldFullRect.Height \ SFD.yMaxSteine + 2

            'Die Steinabmessungen werden interativ berechnet.
            'Sie sind abhängig von der Feldgröße in Steinen, der Feldgröße in Pixeln
            'und der Feldhöhe in Pixelverschiebungen je Stein und Ebene und ob oben die Vorratssteine sind.
            'Zusätzlich fließen noch die Paddings rings um das Ausgaberechteck in die Berechnung ein.
            '
            '
            Dim summeWidth As Integer
            Dim summeHeight As Integer

            Dim steinSize As Size
            Dim shrink As Integer = -1

            Dim test As Integer = INI.Rendering_OrgGrafikReferenceSizeWidth

            Do
                shrink += 1

                steinSize = GetNewSteinSize(shrink)


                SFD.offset3DLeftJeEbene = CInt(Math.Max(INI.Rendering_Offset3DFaktorAbsolutX * steinSize.Width, CDbl(INI.Rendering_Offset3DMinPerLayerX)))
                SFD.offset3DTopJeEbene = CInt(Math.Max(INI.Rendering_Offset3DFaktorAbsolutY * steinSize.Height, CDbl(INI.Rendering_Offset3DMinPerLayerY)))

                SFD.offset3DLeftSumme = SFD.offset3DLeftJeEbene * SFD.zMax ' + INI.Rendering_RectOutputPaddingLeft + INI.Rendering_RectOutputPaddingRight
                SFD.offset3DTopSumme = SFD.offset3DTopJeEbene * SFD.zMax '+ INI.Rendering_RectOutputPaddingTop + INI.Rendering_RectOutputPaddingBottom

                summeWidth = steinSize.Width * SFD.xMaxSteine + SFD.offset3DLeftSumme
                summeHeight = steinSize.Height * SFD.yMaxSteine + SFD.offset3DTopSumme + If(SFD.AktRendering = RenderingEnum.Editor, steinSize.Height + INI.Rendering_HScrollbarHeight, 0)

                CreateMainLayout(steinSize)
                If summeWidth <= SFD.splfldFullRect.Width AndAlso summeHeight <= SFD.splfldFullRect.Height Then
                    Exit Do
                End If
            Loop


            SFD.steinWidth = steinSize.Width
            SFD.steinHeight = steinSize.Height

            SFD.steinWidthHalf = steinWidth \ 2
            SFD.steinHeightHalf = steinHeight \ 2

            'getestet mit einem Spielfeld mit 75 X 25 X 25 Steinen
            'und einem RenderRect mit 344 x 194 Pixeln
            'und einem MJ_SPIELFELD_MIN_WIDTH MJ_SPIELFELD_MIN_HEIGHT von 600 x 400 Pixeln.
            'Das läuft.
            ''Auf das Spielfeld passen 75 x 25 x 25 = 46,875 Steine (und der Computer schafft die Anzeige.) 
            '
            'neu berechnen
            summeWidth = SFD.steinWidth * SFD.xMaxSteine + SFD.offset3DLeftSumme '+ 2
            summeHeight = SFD.steinHeight * SFD.yMaxSteine + SFD.offset3DTopSumme '+ 2

            Dim deltaWidth As Integer = SFD.splfldFullRect.Width - summeWidth
            Dim deltaHeigh As Integer = SFD.splfldFullRect.Height - summeHeight

            'Ausgabefeld zentrieren
            Dim splfldRectLeft As Integer = deltaWidth \ 2 + MJ_MARGIN_ABSOLUT_LEFT
            Dim splfldRectTop As Integer = deltaHeigh \ 2 + MJ_MARGIN_ABSOLUT_TOP
            Dim splfldRectWidth As Integer = summeWidth
            Dim splfldRectHeight As Integer = summeHeight

            SFD.splfldUsedRect = New Rectangle(splfldRectLeft, splfldRectTop, splfldRectWidth, splfldRectHeight)

            If SFD.steinWidthLastCreated <> SFD.steinWidth OrElse SFD.steinHeightLastCreated <> SFD.steinHeight Then
                BitmapContainer.ChangeImagesSize(SFD.steinWidth, SFD.steinHeight)
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
        ''' <param name="steinSize"></param>
        Private Sub CreateMainLayout(steinSize As Size)


            If SFD.AktRendering = RenderingEnum.Spielfeld Then

                ugrdRect = New Rectangle(outputRect.Left, outputRect.Top, outputRect.Width - 1, outputRect.Height - 1)
                splfldFullRect = New Rectangle(ugrdRect.Left + MJ_MARGIN_ABSOLUT_LEFT,
                                               ugrdRect.Top + MJ_MARGIN_ABSOLUT_TOP,
                                               ugrdRect.Width - MJ_MARGIN_ABSOLUT_LEFT - MJ_MARGIN_ABSOLUT_RIGHT,
                                               ugrdRect.Height - MJ_MARGIN_ABSOLUT_TOP - MJ_MARGIN_ABSOLUT_BOTTOM)

            ElseIf SFD.AktRendering = RenderingEnum.Editor Then


                ugrdRect = New Rectangle(outputRect.Left, outputRect.Top, outputRect.Width, outputRect.Height)

                stockRect = New Rectangle(ugrdRect.Left + MJ_MARGIN_ABSOLUT_LEFT,
                                                ugrdRect.Top + MJ_MARGIN_ABSOLUT_TOP,
                                                ugrdRect.Width - MJ_MARGIN_ABSOLUT_LEFT - MJ_MARGIN_ABSOLUT_RIGHT,
                                                steinSize.Height)

                scrollbarRect = New Rectangle(stockRect.Left, stockRect.Bottom + 1, stockRect.Width, INI.Rendering_HScrollbarHeight)

                splfldFullRect = New Rectangle(scrollbarRect.Left,
                                               scrollbarRect.Bottom + 1,
                                               scrollbarRect.Width,
                                               ugrdRect.Height - MJ_MARGIN_ABSOLUT_TOP - MJ_MARGIN_ABSOLUT_BOTTOM -
                                                    scrollbarRect.Height - stockRect.Height)

                splfldFullRect = New Rectangle(ugrdRect.Left + MJ_MARGIN_ABSOLUT_LEFT,
                                               ugrdRect.Top + MJ_MARGIN_ABSOLUT_TOP,
                                               ugrdRect.Width - MJ_MARGIN_ABSOLUT_LEFT - MJ_MARGIN_ABSOLUT_RIGHT,
                                               ugrdRect.Height - MJ_MARGIN_ABSOLUT_TOP - MJ_MARGIN_ABSOLUT_BOTTOM)
            End If

        End Sub



    End Module

End Namespace
