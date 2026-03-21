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
Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On
'
#Disable Warning IDE0079
#Disable Warning IDE1006

Namespace Spielfeld


    '''<summary>
    ''' Pfad: MahjongGK/Spielfeld/Render
    '''
    ''' Hält die globalen Renderressourcen und renderbezogenen Laufzeitwerte. 
    ''' 
    ''' SFRen bündelt alles, was für die tatsächliche Ausgabe und das Rendering 
    ''' global benötigt wird. Dazu gehören insbesondere: 
    ''' 
    ''' - Backbuffer und Graphics, 
    ''' - Kennzeichen, ob der Backbuffer bereits Inhalt hat, 
    ''' - globale Renderzähler und Aktivitätsflags, 
    ''' - Startscreen- und ähnliche Bildcaches, 
    ''' - globale Hilfsobjekte zur Renderdiagnose oder Debug-Ausgabe. 
    ''' 
    ''' SFRen enthält nur die globalen Renderressourcen selbst, 
    ''' nicht aber feldbezogene Rendermetriken wie Steinbreiten oder 3D-Offsets, 
    ''' sofern diese zu genau einem konkreten Spielfeld gehören. 
    ''' Solche Werte gehören bevorzugt nach SFRun. 
    ''' 
    ''' Diese Klasse beschreibt also die technische Renderumgebung, 
    ''' nicht das fachliche Spielfeldmodell. 
    ''' </summary> 
    Public Class SFRender

        Public Sub New()

        End Sub

        Public Sub New(owner As SFDaten)
            _sfd = owner
        End Sub

        Private ReadOnly _sfd As SFDaten


        '##############################################



        ''' <summary>
        ''' Der Hauptverteiler zum Rendern. Hier sind die Basisfunktionen angesiedelt
        ''' und der Verteiler, der nach Rendering von Spielfeld und Editor
        ''' sowie zu den Spielfeldanimationen veteilt.
        ''' </summary>
        ''' <param name="PaintEventGfx"></param>
        ''' <param name="rectOutput"></param>
        ''' <param name="timeDifferenzFaktor"></param>
        Public Sub RenderRouter(PaintEventGfx As Graphics, rectOutput As Rectangle, timeDifferenzFaktor As Double)

            'Es wird grundsätzlich in den Backpuffer gezeichnet, der am Ende dann auf das Control geplittet wird.
            _sfd.SFRun.CreateBackbufferAndGfx()
            _sfd.SFRun.Backbuffer_HasContent = True


            'Den Zeichenknecht des Background-Backbuffers setzen
            _sfd.SFLay.rxOutput?.SetGfx(_sfd.SFRun.BackBufferGfx)
            _sfd.SFLay.rxStock?.SetGfx(_sfd.SFRun.BackBufferGfx)
            _sfd.SFLay.rxStockScrollbar?.SetGfx(_sfd.SFRun.BackBufferGfx)
            _sfd.SFLay.rxStockMark?.SetGfx(_sfd.SFRun.BackBufferGfx)
            _sfd.SFLay.rxBitmapUgrd?.SetGfx(_sfd.SFRun.BackBufferGfx)
            _sfd.SFLay.rxHeader?.SetGfx(_sfd.SFRun.BackBufferGfx)
            _sfd.SFLay.rxContent?.SetGfx(_sfd.SFRun.BackBufferGfx)
            _sfd.SFLay.rxHistoryBoxLeft?.SetGfx(_sfd.SFRun.BackBufferGfx)
            _sfd.SFLay.rxHistoryBoxRight?.SetGfx(_sfd.SFRun.BackBufferGfx)
            _sfd.SFLay.rxHistoryBoxLeftScrollbar?.SetGfx(_sfd.SFRun.BackBufferGfx)
            _sfd.SFLay.rxHistoryBoxRightScrollbar?.SetGfx(_sfd.SFRun.BackBufferGfx)
            _sfd.SFLay.rxStageAvailable?.SetGfx(_sfd.SFRun.BackBufferGfx)
            _sfd.SFLay.rxStageUsed?.SetGfx(_sfd.SFRun.BackBufferGfx)

            If _sfd.SFInf.IsEmpty Then
                _sfd.SFLay.rxHeader?.DrawStringCentered("Keine Daten geladen", New Font("Arial", 16, FontStyle.Bold), usePadding:=False, foreColor:=Nothing)
                _sfd.SFRun.BackBufferGfx.Clear(INI.Toolbox_HGrdSplFldColorFallback)
                Exit Sub
            End If

            If INI.Rendering_DrawRenderRect Then
                Dim vuctl As Control = SFMain.VisibleUserControlControl
                _sfd.SFRun.DebugLabels.Start(rectOutput)
                'nur zum Debuggen: die Umrisskanten der Renderbereiche zeichnen.
                PaintEventGfx.Clear(Color.Silver)
                _sfd.SFLay.rxOutput?.SetDebugHost(vuctl)
                _sfd.SFLay.rxStock?.SetDebugHost(vuctl)
                _sfd.SFLay.rxStockScrollbar?.SetDebugHost(vuctl)
                _sfd.SFLay.rxStockMark?.SetDebugHost(vuctl)
                _sfd.SFLay.rxBitmapUgrd?.SetDebugHost(vuctl)
                _sfd.SFLay.rxHeader?.SetDebugHost(vuctl)
                _sfd.SFLay.rxContent?.SetDebugHost(vuctl)
                _sfd.SFLay.rxHistoryBoxLeft?.SetDebugHost(vuctl)
                _sfd.SFLay.rxHistoryBoxRight?.SetDebugHost(vuctl)
                _sfd.SFLay.rxHistoryBoxLeftScrollbar?.SetDebugHost(vuctl)
                _sfd.SFLay.rxHistoryBoxRightScrollbar?.SetDebugHost(vuctl)
                _sfd.SFLay.rxStageAvailable?.SetDebugHost(vuctl)
                _sfd.SFLay.rxStageUsed?.SetDebugHost(vuctl)

                _sfd.SFLay.rxOutput?.DrawDebug()
                _sfd.SFLay.rxBitmapUgrd?.DrawDebug()
                _sfd.SFLay.rxContent?.DrawDebug()
                _sfd.SFLay.rxHeader?.DrawDebug()
                _sfd.SFLay.rxStock?.DrawDebug()
                _sfd.SFLay.rxStockScrollbar?.DrawDebug()
                _sfd.SFLay.rxStockMark?.DrawDebug()
                _sfd.SFLay.rxHistoryBoxLeft?.DrawDebug()
                _sfd.SFLay.rxHistoryBoxRight?.DrawDebug()
                _sfd.SFLay.rxHistoryBoxLeftScrollbar?.DrawDebug()
                _sfd.SFLay.rxHistoryBoxRightScrollbar?.DrawDebug()
                _sfd.SFLay.rxStageAvailable?.DrawDebug()
                _sfd.SFLay.rxStageUsed?.DrawDebug()

                'Den Untergrund einzeichnen
                'TODO BtmpUGrd verlagern? 
            Else
                If _sfd.SFInf.HasBitmapUGrd Then
                    _sfd.SFRun.BackBufferGfx.DrawImage(_sfd.SFInf.GetBitmapUGrd(rectOutput.Size), Point.Empty)
                Else
                    _sfd.SFRun.BackBufferGfx.Clear(_sfd.SFInf.HGrdSplFldColor)
                End If
            End If

            If INI.Rendering_DrawRenderingSkipDoneMarker Then
                'unterer rechter Marker, wenn der Backpuffer wiederverwendet wird
                DrawRenderingSkipDoneMarker(PaintEventGfx, rectOutput.Width, rectOutput.Height, _sfd.SFRun.RenderingDoneCounter, "Done")
            End If

            _sfd.SFLay.rxHeader?.DrawStringCentered("Hallo", New Font("Arial", 16, FontStyle.Bold), usePadding:=False, foreColor:=Nothing)



            If _sfd.SFRun.AktRenderMode = AktRenderMode.Spiel Then
                Paint_Spielfeld(PaintEventGfx, rectOutput, timeDifferenzFaktor)

            ElseIf _sfd.SFRun.AktRenderMode = AktRenderMode.Edit Then

                If INI.Editor_ShowGrid Then
                    PaintGrid()
                End If
                Paint_Editor(PaintEventGfx, rectOutput, timeDifferenzFaktor)

                If INI.Editor_ShowFrmSteinStackInfo Then
                    _sfd.SFRun.EditorFrmSteinStackInfo.UpdateInfo(_sfd.SFInf.GetSteinStackInfos(_sfd.SFRun.MousePolling.MousePos),
                                                           _sfd.SFRun.MousePolling.MousePos,
                                                           _sfd.SFLay.rxStageAvailable.ToRectangle)

                End If

            End If

            PaintEventGfx.DrawImageUnscaled(_sfd.SFRun.Backbuffer, rectOutput.Location)

        End Sub

        Private Sub Paint_Spielfeld(PaintEventGfx As Graphics, rectOutput As Rectangle, timeDifferenzFaktor As Double)

            Dim aktSteinInfo As SteinInfo

            Dim toggleVergleichsflag As Boolean = _sfd.SFInf.GetFirstToggleFlagValue

            With _sfd.SFInf

                For z As Integer = .zMin To .zMax
                    For x As Integer = .xMin To .xMax
                        For y As Integer = .yMin To .yMax

                            If .arrFB(x, y, z) = 0 Then
                                'unbelegtes Feld
                                Continue For
                            End If

                            'Was hier am Anfang passiert ist etwas komplexer, tricky und schnell.
                            'Es ist genau beschrieben unter Spielfeld/liesmich.txt
                            'Stichworte: XOffset, YOffset, ToggleFlag
                            If toggleVergleichsflag <> .GetToggleFlag(x, y, z) Then
                                'bereits verarbeitetes Feld
                                Continue For
                            End If

                            'als bearbeitet markieren
                            .ToggleToggleFlag(x, y, z)
                            '
                            aktSteinInfo = .SteinInfos(.GetIndexStein(x, y, z))

                            With aktSteinInfo

                                'Dim left As Integer = _sfd.SteinOnStange_Left(x, z)
                                'Dim top As Integer = _sfd.SteinOnStange_Top(y, z)
                                ''Debug
                                'debugInfo &= $"x={x},y={y},z={z},arrFB-SteinIdx={ _sfd.AktSpielfeldInfo.GetIndexStein(x, y, z)},SII={aktSteinInfo.SteinInfoIndex},SI={aktSteinInfo.SteinIndex}|"
                                ''/Debug
                                Dim bmpStein As Bitmap = Images.SGM.GetStein(.SteinIndex, .SteinStatusUsed, _sfd.SFLay.steinSize, _sfd.SFRun.AktRenderMode)

                                If Not IsNothing(bmpStein) Then
                                    _sfd.SFRun.BackBufferGfx.DrawImage(bmpStein, .RectStein)
                                End If

                            End With
                        Next
                    Next
                Next

            End With
        End Sub

        Private Sub Paint_Editor(PaintEventGfx As Graphics, rectOutput As Rectangle, timeDifferenzFaktor As Double)



            _sfd.SFRun.HScrollBarStock.PaintHScroll(_sfd.SFRun.BackBufferGfx, enabled:=True)

            Dim aktSteinInfo As SteinInfo
            Dim Kandidat As Triple
            If _sfd.SFRun.EditorStockValues.StockSelectedSteinJob = SelectedSteinJob.MouseMove Then
                Kandidat = _sfd.SFInf.IsFundamentKandidat(_sfd.SFRun.MousePolling.MousePos)
            Else
                Kandidat = New Triple()
            End If


            Dim toggleVergleichsflag As Boolean = _sfd.SFInf.GetFirstToggleFlagValue


            With _sfd.SFInf

                For z As Integer = .zMin To .zMax
                    For x As Integer = .xMin To .xMax
                        For y As Integer = .yMin To .yMax

                            If .arrFB(x, y, z) = 0 Then
                                'unbelegtes Feld
                                Continue For
                            End If

                            If toggleVergleichsflag <> .GetToggleFlag(x, y, z) Then
                                'bereits verarbeitetes Feld
                                Continue For
                            End If

                            'als bearbeitet markieren
                            .ToggleToggleFlag(x, y, z)
                            '
                            aktSteinInfo = .SteinInfos(.GetIndexStein(x, y, z))

                            With aktSteinInfo

                                'Dim left As Integer = _sfd.SteinOnStange_Left(x, z)
                                'Dim top As Integer = _sfd.SteinOnStange_Top(y, z)
                                ''Debug
                                'debugInfo &= $"x={x},y={y},z={z},arrFB-SteinIdx={ _sfd.AktSpielfeldInfo.GetIndexStein(x, y, z)},SII={aktSteinInfo.SteinInfoIndex},SI={aktSteinInfo.SteinIndex}|"
                                ''/Debug
                                Dim bmpStein As Bitmap = Images.SGM.GetStein(.SteinIndex, .SteinStatusUsed, _sfd.SFLay.steinSize, _sfd.SFRun.AktRenderMode)

                                If Not IsNothing(bmpStein) Then
                                    'Je nach .SteinStatusUsed kann die Bitmap Nothing sein.
                                    If Kandidat.IsEqual(x, y, z) Then
                                        'eine ungültige Position hat die Werte (0,0,0), die es auf den Spielfeld nicht gibt,
                                        'deshalb keine gesonderte Abfrage auf "ungültig" nötig.
                                        bmpStein = DrawOverlay(bmpStein, OverlayType.RahmenSteinMouseOver, copyBitmap:=True)
                                        _sfd.SFRun.BackBufferGfx.DrawImage(bmpStein, .RectStein)
                                        bmpStein.Dispose()
                                    Else
                                        _sfd.SFRun.BackBufferGfx.DrawImage(bmpStein, .RectStein)
                                    End If
                                End If

                            End With
                        Next
                    Next
                Next

            End With

            PaintStock(timeDifferenzFaktor)



        End Sub



        Private Sub PaintAnimatedStein(gfx As Graphics, rectOutput As Rectangle, timeDifferenzFaktor As Double, aktSteinInfo As SteinInfo, pos3D As Triple)

            'Dim left As Integer = _sfd.renderRectLeft + (_sfd.steinWidthHalf * pos3D.x) - (_sfd.offset3DLeftJeEbene * pos3D.z)
            'Dim top As Integer = _sfd.renderRectTop + (_sfd.steinHeightHalf * pos3D.y) - (_sfd.offset3DTopJeEbene * pos3D.z)

            'Dim rectStein As New Rectangle(left, top, _sfd.steinWidth, _sfd.steinHeight)

        End Sub

        Private Sub PaintGrid()

            Dim penI As Pen = GetPenGridInside()
            Dim penO As Pen = GetPenGridOutside()

            _sfd.SFRun.BackBufferGfx.DrawRectangle(penO, _sfd.SFLay.rxStageUsed.BoundsRect)

            Dim xLine As Integer = _sfd.SFLay.rxStageUsed.Left + _sfd.SFLay.offset3DLeftSumme + (_sfd.SFInf.xMin - 1) * _sfd.SFLay.steinWidthHalf
            Dim yTop As Integer = _sfd.SFLay.rxStageUsed.Top + _sfd.SFLay.offset3DTopSumme
            Dim yBottom As Integer = _sfd.SFLay.rxStageUsed.Bottom

            For x As Integer = _sfd.SFInf.xMin To _sfd.SFInf.xMax
                _sfd.SFRun.BackBufferGfx.DrawLine(penI, xLine, yTop, xLine, yBottom)
                xLine += _sfd.SFLay.steinWidthHalf
            Next

            Dim yLine As Integer = _sfd.SFLay.rxStageUsed.Top + _sfd.SFLay.offset3DTopSumme + (_sfd.SFInf.yMin - 1) * _sfd.SFLay.steinHeightHalf
            Dim xLeft As Integer = _sfd.SFLay.rxStageUsed.Left + _sfd.SFLay.offset3DLeftSumme
            Dim xRight As Integer = _sfd.SFLay.rxStageUsed.Right

            For y As Integer = _sfd.SFInf.yMin To _sfd.SFInf.yMax
                _sfd.SFRun.BackBufferGfx.DrawLine(penI, xLeft, yLine, xRight, yLine)
                yLine += _sfd.SFLay.steinHeightHalf
            Next

        End Sub

        Private _penGridInside As Pen
        Private _penGridInsideColor As Color
        Private _penGridOutside As Pen
        Private _penGridOutsideColor As Color
        Private Function GetPenGridInside() As Pen

            Dim clr As Color = INI.Editor_GridColorInside

            If IsNothing(_penGridInside) OrElse _penGridInsideColor <> clr Then

                If Not IsNothing(_penGridInside) Then
                    _penGridInside.Dispose()
                End If

                _penGridInside = New Pen(clr, 1)
                _penGridInsideColor = clr

            End If

            Return _penGridInside

        End Function

        Private Function GetPenGridOutside() As Pen

            Dim clr As Color = INI.Editor_GridColorOutside

            If IsNothing(_penGridOutside) OrElse _penGridOutsideColor <> clr Then

                If Not IsNothing(_penGridOutside) Then
                    _penGridOutside.Dispose()
                End If

                _penGridOutside = New Pen(clr, 1)
                _penGridOutsideColor = clr

            End If

            Return _penGridOutside

        End Function



        Public Sub PaintStock(timeDifferenzFaktor As Double)

            If _sfd.SFInf.Generator.StockAktCount = 0 Then
                _sfd.SFRun.HScrollBarStock.SetRange(0)
                Return
            Else
                _sfd.SFRun.EditorStockValues.SetHScrollBarValue(_sfd.SFRun.HScrollBarStock.GetValue)
            End If

            If _sfd.SFRun.EditorStockMouseAnkerVerschiebung_HasValue Then
                _sfd.SFRun.EditorStockDeltaMouseToGhost = _sfd.SFRun.EditorStockMouseAnkerVerschiebung.NewMouseAnkerPos
            End If

            Dim stockAktUBnd As Integer = _sfd.SFInf.Generator.StockAktUBnd
            Dim xOffset As Integer = 0
            Dim bmpStein As Bitmap

            With _sfd.SFRun.EditorStockValues
                'Rendern der horizontalen Steinleiste.
                For idx As Integer = .SteinVisibleAktFistIdx To .SteinVisibleAktLastIdx
                    If idx >= _sfd.SFInf.Generator.Stock.Count Then
                        Exit For
                    Else
                        If idx = _sfd.SFRun.EditorStockValues.GhostIdx AndAlso
                            _sfd.SFRun.EditorStockValues.StockSelectedSteinJob = SelectedSteinJob.MouseMove AndAlso
                            _sfd.SFRun.MousePolling.LeftMouseChanged = True AndAlso
                            _sfd.SFRun.MousePolling.LeftMousePressed = True Then

                            bmpStein = _sfd.SFRun.EditorStockValues.BmpGhost
                            _sfd.SFRun.EditorStockGhostRect = New Rectangle(xOffset + .OffsetLeft, _sfd.SFLay.rxStock.Top, bmpStein.Width, bmpStein.Height)
                            _sfd.SFRun.EditorStockDeltaMouseToGhost = New Point(_sfd.SFRun.MousePolling.MousePos.X - xOffset, _sfd.SFRun.MousePolling.MousePos.Y - _sfd.SFLay.rxStock.Top)
                            _sfd.SFRun.EditorStockMouseAnkerVerschiebung = New MouseAnkerVerschiebung(_sfd, _sfd.SFRun.EditorStockDeltaMouseToGhost)

                        ElseIf idx = _sfd.SFRun.EditorStockValues.GhostIdx Then
                            bmpStein = _sfd.SFRun.EditorStockValues.BmpGhost

                        Else

                            bmpStein = Images.SGM.GetStein(index:=_sfd.SFInf.Generator.Stock(idx), status:=SteinStatus.I01Normal, _sfd.SFLay.steinSize, _sfd.SFRun.AktRenderMode)
                        End If

                        Dim rectAusgabe As Rectangle = New Rectangle(xOffset + .OffsetLeft, _sfd.SFLay.rxStock.Top, bmpStein.Width, bmpStein.Height)

                        If Not _sfd.SFRun.MousePolling.LeftMousePressed AndAlso rectAusgabe.Contains(_sfd.SFRun.MousePolling.MousePos) Then
                            bmpStein = DrawOverlay(bmpStein, OverlayType.RahmenSteinMouseOver, copyBitmap:=True)
                            _sfd.SFRun.BackBufferGfx.DrawImage(bmpStein, rectAusgabe)
                            bmpStein.Dispose()
                        Else
                            _sfd.SFRun.BackBufferGfx.DrawImage(bmpStein, rectAusgabe)
                        End If

                        xOffset += _sfd.SFLay.steinWidth
                    End If
                Next
                '
                '
                Dim pos3D As New Triple

                If .StockSelectedSteinJob = SelectedSteinJob.MouseMove Then
                    Dim ptCur As Point = New Point(_sfd.SFRun.MousePolling.MousePos.X - _sfd.SFRun.EditorStockDeltaMouseToGhost.X, _sfd.SFRun.MousePolling.MousePos.Y - _sfd.SFRun.EditorStockDeltaMouseToGhost.Y)


                    pos3D = _sfd.SFInf.IsFundamentKandidat(_sfd.SFRun.MousePolling.MousePos)


                    If pos3D.IsValideYes Then
                        _sfd.SFRun.EditorSteinDoPlaceAtPosTriple = pos3D
                        _sfd.SFRun.EditorSteinDoPlace_SteinIndex = .SelectedSteinIndex

                        'Die GhostBmp ist vom Steinvorrat und wird beim Klick auf einen Stein im Vorrat erzeugt,
                        'wird aber erst überschrieben, wenn die Nächste erzeugt wird und kann daher hier verwendet werden. 

                        _sfd.SFRun.BackBufferGfx.DrawImage(_sfd.SFRun.EditorStockValues.BmpGhost, _sfd.SFInf.GetSteinRenderRect(pos3D))
                        bmpStein = _sfd.SFRun.EditorStockValues.BmpPlaceable
                    Else
                        bmpStein = _sfd.SFRun.EditorStockValues.BmpSelected
                        'löschen, sonst fliegt der Stein nicht zurück, sondern wird auf
                        'dem zuletzt gefundenem gültigem Platz abgelegt.
                        _sfd.SFRun.EditorSteinDoPlaceAtPosTriple = New Triple
                    End If
                    'zuletzt als oberstes den an die Maus gekoppelten Stein
                    _sfd.SFRun.BackBufferGfx.DrawImage(bmpStein, ptCur)
                End If

                If .StockSelectedSteinJob = SelectedSteinJob.ChoiseInsertOrFlyBack Then
                    'Hier Entscheidung
                    If _sfd.SFRun.EditorSteinDoPlaceAtPosTriple.IsValideYes Then

                        .StockSelectedSteinJob = SelectedSteinJob.Insert_Do

                    Else
                        .StockSelectedSteinJob = SelectedSteinJob.FlyBack_Do
                    End If
                End If

                If .StockSelectedSteinJob = SelectedSteinJob.FlyBack_Do Then

                    Dim ptCur As Point = _sfd.SFRun.EditorStockSteinFlugValues.GetNextPoint(timeDifferenzFaktor)

                    Dim idx As Integer = _sfd.SFRun.EditorStockValues.GhostIdx
                    If idx = -1 Then
                        idx = _sfd.SFRun.EditorStockValues.SelectedStockSteinIdx
                    End If
                    bmpStein = Images.SGM.GetStein(index:=_sfd.SFInf.Generator.Stock(idx),
                                            status:=SteinStatus.I01Normal,
                                           _sfd.SFLay.steinSize,
                                           _sfd.SFRun.AktRenderMode)

                    _sfd.SFRun.BackBufferGfx.DrawImage(bmpStein, ptCur)
                    If _sfd.SFRun.EditorStockSteinFlugValues.IsFinished Then
                        .StockSelectedSteinJob = SelectedSteinJob.FlyBack_Done
                    End If
                End If

                If .StockSelectedSteinJob = SelectedSteinJob.Insert_Do Then

                    _sfd.SFInf.AddSteinToSpielfeld(_sfd.SFRun.EditorSteinDoPlace_SteinIndex, _sfd.SFRun.EditorSteinDoPlaceAtPosTriple)


                    _sfd.SFRun.EditorSteinDoPlaceAtPosTriple = New Triple
                    .StockSelectedSteinJob = SelectedSteinJob.Insert_Done
                End If


            End With

        End Sub

    End Class
End Namespace