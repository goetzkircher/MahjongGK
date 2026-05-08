Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On
Imports MahjongGK.Contracts.GlobalEnum
Imports TileFactory

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
        Private _paintEventGfx As Graphics
        Private _backBufferGfx As Graphics
        Private _rectOutputOrg As Rectangle
        Private _rectOutputUsed As Rectangle
        Private _timeDifferenzFaktor As Double

        ''' <summary>
        ''' Der Hauptverteiler zum Rendern. Hier sind die Basisfunktionen angesiedelt
        ''' und der Verteiler, der nach Rendering von Spielfeld und Editor
        ''' sowie zu den Spielfeldanimationen veteilt.
        ''' </summary>
        ''' <param name="PaintEventGfx"></param>
        ''' <param name="rectOutput"></param>
        ''' <param name="timeDifferenzFaktor"></param>
        Public Sub RenderingDistributor(PaintEventGfx As Graphics, rectOutput As Rectangle, timeDifferenzFaktor As Double)

            'Es wird grundsätzlich in den Backpuffer gezeichnet, der am Ende dann auf das Control geplittet wird.
            _sfd.SFRun.CreateBackbufferAndGfx()
            _sfd.SFRun.Backbuffer_HasContent = True
            '() Reihenfolge einhalten
            _paintEventGfx = PaintEventGfx
            _backBufferGfx = _sfd.SFRun.BackBufferGfx
            _rectOutputOrg = rectOutput
            _rectOutputUsed = _sfd.SFLay.rxOutputUsed
            _timeDifferenzFaktor = timeDifferenzFaktor

            'Den Zeichenknecht des Background-Backbuffers setzen
            _sfd.SFLay.rxOutputUsed?.SetGfx(_backBufferGfx)
            _sfd.SFLay.rxStock?.SetGfx(_backBufferGfx)
            _sfd.SFLay.rxStockScrollbar?.SetGfx(_backBufferGfx)
            _sfd.SFLay.rxBitmapUgrd?.SetGfx(_backBufferGfx)
            _sfd.SFLay.rxHeader?.SetGfx(_backBufferGfx)
            _sfd.SFLay.rxContent?.SetGfx(_backBufferGfx)
            _sfd.SFLay.rxHistoryBoxLeft?.SetGfx(_backBufferGfx)
            _sfd.SFLay.rxHistoryBoxRight?.SetGfx(_backBufferGfx)
            _sfd.SFLay.rxHistoryBoxSmall?.SetGfx(_backBufferGfx)
            _sfd.SFLay.rxHistoryBoxLeftScrollbar?.SetGfx(_backBufferGfx)
            _sfd.SFLay.rxHistoryBoxRightScrollbar?.SetGfx(_backBufferGfx)
            _sfd.SFLay.rxStageAvailable?.SetGfx(_backBufferGfx)
            _sfd.SFLay.rxStageUsed?.SetGfx(_backBufferGfx)
            _sfd.SFLay.rxUndo?.SetGfx(_backBufferGfx)
            _sfd.SFLay.rxRedo?.SetGfx(_backBufferGfx)

            If _sfd.SFInf.IsEmpty Then
                _sfd.SFLay.rxHeader?.DrawStringCentered("Keine Daten geladen", New Font("Arial", 16, FontStyle.Bold), usePadding:=False, foreColor:=Nothing)
                _backBufferGfx.Clear(INI.Toolbox_HGrdSplFldColorFallback)
                Exit Sub
            End If

            If INI.Rendering_DrawRenderRect Then
                Dim vuctl As Control = SFMain.VisibleUserControlControl
                _sfd.SFRun.DebugLabels.Start(rectOutput)
                'nur zum Debuggen: die Umrisskanten der Renderbereiche zeichnen.
                PaintEventGfx.Clear(Color.Silver)
                _sfd.SFLay.rxOutputUsed?.SetDebugHost(vuctl)
                _sfd.SFLay.rxStock?.SetDebugHost(vuctl)
                _sfd.SFLay.rxStockScrollbar?.SetDebugHost(vuctl)
                _sfd.SFLay.rxBitmapUgrd?.SetDebugHost(vuctl)
                _sfd.SFLay.rxHeader?.SetDebugHost(vuctl)
                _sfd.SFLay.rxContent?.SetDebugHost(vuctl)
                _sfd.SFLay.rxHistoryBoxLeft?.SetDebugHost(vuctl)
                _sfd.SFLay.rxHistoryBoxRight?.SetDebugHost(vuctl)
                _sfd.SFLay.rxHistoryBoxSmall?.SetDebugHost(vuctl)
                _sfd.SFLay.rxHistoryBoxLeftScrollbar?.SetDebugHost(vuctl)
                _sfd.SFLay.rxHistoryBoxRightScrollbar?.SetDebugHost(vuctl)
                _sfd.SFLay.rxStageAvailable?.SetDebugHost(vuctl)
                _sfd.SFLay.rxStageUsed?.SetDebugHost(vuctl)
                _sfd.SFLay.rxUndo?.SetDebugHost(vuctl)
                _sfd.SFLay.rxRedo?.SetDebugHost(vuctl)

                _sfd.SFLay.rxOutputUsed?.DrawDebug()
                _sfd.SFLay.rxBitmapUgrd?.DrawDebug()
                _sfd.SFLay.rxContent?.DrawDebug()
                _sfd.SFLay.rxHeader?.DrawDebug()
                _sfd.SFLay.rxStock?.DrawDebug()
                _sfd.SFLay.rxStockScrollbar?.DrawDebug()
                _sfd.SFLay.rxHistoryBoxLeft?.DrawDebug()
                _sfd.SFLay.rxHistoryBoxRight?.DrawDebug()
                _sfd.SFLay.rxHistoryBoxSmall?.DrawDebug()
                _sfd.SFLay.rxHistoryBoxLeftScrollbar?.DrawDebug()
                _sfd.SFLay.rxHistoryBoxRightScrollbar?.DrawDebug()
                _sfd.SFLay.rxStageAvailable?.DrawDebug()
                _sfd.SFLay.rxStageUsed?.DrawDebug()
                _sfd.SFLay.rxUndo?.DrawDebug()
                _sfd.SFLay.rxRedo?.DrawDebug()

                'Den Untergrund einzeichnen
                'TODO BtmpUGrd verlagern? 
            Else
                If Not IsNothing(_sfd.SFLay.rxBitmapUgrd) Then
                    If _sfd.SFInf.HasBitmapUGrd Then
                        _backBufferGfx.DrawImage(_sfd.SFInf.GetBitmapUGrd(rectOutput.Size), Point.Empty)
                    Else
                        _backBufferGfx.Clear(_sfd.SFInf.HGrdSplFldColor)
                    End If
                Else
                    _backBufferGfx.Clear(_sfd.SFInf.HGrdSplFldColor)
                End If
            End If

            If INI.Rendering_DrawRenderingSkipDoneMarker Then
                'unterer rechter Marker, wenn der Backpuffer wiederverwendet wird
                DrawRenderingSkipDoneMarker(_paintEventGfx, rectOutput.Width, rectOutput.Height, _sfd.SFRun.RenderingDoneCounter, "Done")
            End If

            _sfd.SFLay.rxHeader?.DrawStringCentered("Hallo", New Font("Arial", 16, FontStyle.Bold), usePadding:=False, foreColor:=Nothing)

            If _sfd.SFRun.AktRenderMode = AktRenderMode.Spiel Then
                Paint_Spielfeld()

            ElseIf _sfd.SFRun.AktRenderMode = AktRenderMode.Edit Then

                If INI.Editor_ShowGrid Then
                    PaintGrid()
                End If

                Paint_Editor()

                If INI.Editor_ShowFrmTooltipSteinInfo Then
                    _sfd.SFRun.EditorFrmTooltipSteinInfo.UpdateInfo(_sfd.SFInf.GetSteinToolTipInfos(_sfd.SFRun.MousePolling.MousePos),
                                                           _sfd.SFRun.MousePolling.MousePos,
                                                           _sfd.SFLay.rxStageAvailable.ToRectangle)

                End If

            End If

            PaintEventGfx.DrawImageUnscaled(_sfd.SFRun.Backbuffer, rectOutput.Location)

        End Sub

        Private Sub Paint_Spielfeld()

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

                            If toggleVergleichsflag <> .GetToggleFlag(x, y, z) Then
                                'bereits verarbeitetes Feld
                                Continue For

                            ElseIf .arrFB(x, y, z) = 0 Then
                                'unbelegtes Feld
                                Continue For

                            ElseIf .GetIsRemoved(x, y, z) Then
                                Continue For

                            End If

                            'als bearbeitet markieren
                            .ToggleToggleFlag(x, y, z)
                            '
                            aktSteinInfo = .SteinInfos(.GetSteinInfoIndex(x, y, z))

                            With aktSteinInfo

                                'Dim left As Integer = _sfd.SteinOnStange_Left(x, z)
                                'Dim top As Integer = _sfd.SteinOnStange_Top(y, z)
                                ''Debug
                                'debugInfo &= $"x={x},y={y},z={z},arrFB-SteinIdx={ _sfd.AktSpielfeldInfo.GetIndexStein(x, y, z)},SII={aktSteinInfo.SteinInfoIndex},SI={aktSteinInfo.SteinTypIndex}|"
                                ''/Debug
                                Dim request As New TileRequest(_sfd.SFRun.AktRenderMode, INI.Tile_TileColors, aktSteinInfo.SteinTypIndex, aktSteinInfo.SteinStatus, SteinFrameVersion.Standard, _sfd.SFLay.steinSize, INI.Tile_BasisSize)

                                Dim bmpStein As Bitmap = TileFactory.GetTile(request)

                                If Not IsNothing(bmpStein) Then
                                    _backBufferGfx.DrawImage(bmpStein, .RectStein)
                                End If

                            End With
                        Next
                    Next
                Next

            End With
        End Sub

        Private Sub Paint_Editor()

            _sfd.SFRun.HScrollBarStock.PaintHScroll(_backBufferGfx, enabled:=True)

            Dim aktSteinInfo As SteinInfo

            'Dim Kandidat As Triple
            'If _sfd.SFRun.EditorStockValues.StockSelectedSteinJob = SelectedSteinJob.MouseMove Then
            '    Kandidat = _sfd.SFInf.IsFundamentKandidat(_sfd.SFRun.MousePolling.MousePos)
            'Else
            '    Kandidat = New Triple()
            'End If

            Dim toggleVergleichsflag As Boolean = _sfd.SFInf.GetFirstToggleFlagValue

            'Außerhalb der Schleife deklarieren
            Dim bmpSteinDC As Bitmap = Nothing
            Dim bmpStein As Bitmap = Nothing

            With _sfd.SFInf

                For z As Integer = .zMin To .zMax
                    For x As Integer = .xMin To .xMax
                        For y As Integer = .yMin To .yMax

                            If toggleVergleichsflag <> .GetToggleFlag(x, y, z) Then
                                'bereits verarbeitetes Feld
                                Continue For

                            ElseIf .arrFB(x, y, z) = 0 Then
                                'unbelegtes Feld
                                Continue For

                            ElseIf .GetIsRemoved(x, y, z) Then
                                Continue For

                            End If
                            '
                            'als bearbeitet markieren
                            .ToggleToggleFlag(x, y, z)
                            '

                            aktSteinInfo = .SteinInfos(.GetSteinInfoIndex(x, y, z))

                            Dim plane As Airplane = _sfd.SFAir.GetPlaneFromSteinInfoIndex(aktSteinInfo.SteinInfoIndex)

                            If plane IsNot Nothing AndAlso plane.RenderBitmapIsAvailable Then
                                'mindestens eine animierte oder an der Maus hängende Bitmap ist vorhanden
                                Do
                                    Dim nrb As (found As Boolean, bmp As Bitmap, rect As Rectangle) = plane.NextRenderBitmap()
                                    If nrb.found Then
                                        _backBufferGfx.DrawImage(nrb.bmp, nrb.rect)

                                    Else
                                        Exit Do
                                    End If
                                Loop
                            Else
                                Dim tsi As TopSteinInfo = _sfd.SFInf.GetTopSteinInfo(x, y, z)

                                If tsi IsNot Nothing Then
                                    'Zeichnet alle obersten Steine

                                    '    If tsi.MissingSecond Then
                                    '        bmpSteinDC = Bitmap32DeepCopy(Images.SGM.GetStein(aktSteinInfo.SteinTypIndex, tsi.SteinStatus, _sfd.SFLay.steinSize, _sfd.SFRun.AktRenderMode))
                                    '        Dim gfx As Graphics = Graphics.FromImage(bmpSteinDC)

                                    '        Dim fnt As Font = New Font("Arial", 10.0F, FontStyle.Bold)
                                    '        gfx.DrawString(tsi.MissingSecondText, fnt, Brushes.Black, New PointF(_sfd.SFLay.steinWidth \ 8, _sfd.SFLay.steinHeight \ 8))
                                    '        fnt.Dispose()
                                    '        _backBufferGfx.DrawImage(bmpSteinDC, aktSteinInfo.RectStein)
                                    '        bmpSteinDC.Dispose()
                                    '        bmpSteinDC = Nothing
                                    '    End If

                                    Dim request As New TileRequest(_sfd.SFRun.AktRenderMode, INI.Tile_TileColors, tsi.SteinTypIndex, tsi.SteinStatus, SteinFrameVersion.Standard, _sfd.SFLay.steinSize, INI.Tile_BasisSize)
                                    bmpStein = TileFactory.GetTile(request)

                                    _backBufferGfx.DrawImage(bmpStein, aktSteinInfo.RectStein)

                                Else
                                    'Zeichnet alle Steine, die nicht die obersten Steine sind.
                                    Dim request As New TileRequest(_sfd.SFRun.AktRenderMode, INI.Tile_TileColors, aktSteinInfo.SteinTypIndex, SteinStatus.I01Normal, SteinFrameVersion.Standard, _sfd.SFLay.steinSize, INI.Tile_BasisSize)
                                    bmpStein = TileFactory.GetTile(request)

                                    _backBufferGfx.DrawImage(bmpStein, aktSteinInfo.RectStein)

                                    ''_backBufferGfx.FillRectangle(Brushes.White, aktSteinInfo.RectStein)

                                    'End If

                                    'Dim left As Integer = _sfd.SteinOnStange_Left(x, z)
                                    'Dim top As Integer = _sfd.SteinOnStange_Top(y, z)
                                    ''Debug
                                    'debugInfo &= $"x={x},y={y},z={z},arrFB-SteinIdx={ _sfd.AktSpielfeldInfo.GetIndexStein(x, y, z)},SII={aktSteinInfo.SteinInfoIndex},SI={aktSteinInfo.SteinTypIndex}|"
                                    ''/Debug

                                    'If Not IsNothing(bmpStein) Then

                                    '    'If ContainsTripl(x, y, z, _arrTopSteinTriple) Then
                                    '    '    bmpStein = DrawOverlay(bmpStein, OverlayType.RahmenSteinMouseOver, copyBitmap:=True)
                                    '    'End If

                                    '    ''Je nach .SteinStatusUsed kann die Bitmap Nothing sein.
                                    '    'If Kandidat.IsEqual(x, y, z) Then
                                    '    '    'eine ungültige Position hat die Werte (0,0,0), die es auf den Spielfeld nicht gibt,
                                    '    '    'deshalb keine gesonderte Abfrage auf "ungültig" nötig.
                                    '    '    bmpStein = DrawOverlay(bmpStein, OverlayType.RahmenSteinMouseOver, copyBitmap:=True)
                                    '    '    _backBufferGfx.DrawImage(bmpStein, .RectStein)
                                    '    '    bmpStein.Dispose()
                                    '    'Else
                                    '    'End If
                                    'End If

                                End If
                            End If
                        Next
                    Next
                Next

            End With

            PaintStock()

            Paint_UndoRedo()

            'Hier sind Ausgaben angesiedelt, die in der Z-Order ganz oben aufliegen

            Dim buffer As AirRenderBuffer = _sfd.SFAir.RenderBuffer
            If buffer.RenderBitmapIsAvailable Then
                Do
                    Dim nrb As (found As Boolean, bmp As Bitmap, rect As Rectangle) = buffer.GetNextRenderBitmap
                    If nrb.found Then
                        _backBufferGfx.DrawImage(nrb.bmp, nrb.rect)
                        DebugDrawKreuz(_backBufferGfx, nrb.rect)
                    Else
                        Exit Do
                    End If
                Loop
            End If
        End Sub

        Private Sub PaintGrid()

            Dim penI As Pen = GetPenGridInside()
            Dim penO As Pen = GetPenGridOutside()

            Dim gfx As Graphics = _backBufferGfx
            Dim lay As SFLayout = _sfd.SFLay
            Dim inf As SFInfo = _sfd.SFInf

            Dim rxStage As Rectangle = lay.rxStageUsed.BoundsRect

            Dim leftStage As Integer = lay.rxStageUsed.Left
            Dim topStage As Integer = lay.rxStageUsed.Top
            Dim rightStage As Integer = lay.rxStageUsed.Right
            Dim bottomStage As Integer = lay.rxStageUsed.Bottom

            Dim offsetLeft As Integer = lay.offset3DLeftSumme
            Dim offsetTop As Integer = lay.offset3DTopSumme
            Dim steinWidthHalf As Integer = lay.steinWidthHalf
            Dim steinHeightHalf As Integer = lay.steinHeightHalf

            Dim xMin As Integer = inf.xMin
            Dim xMax As Integer = inf.xMax
            Dim yMin As Integer = inf.yMin
            Dim yMax As Integer = inf.yMax

            gfx.DrawRectangle(penO, rxStage)

            Dim xLine As Integer = leftStage + offsetLeft + (xMin - 1) * steinWidthHalf
            Dim yTop As Integer = topStage + offsetTop
            Dim yBottom As Integer = bottomStage

            For x As Integer = xMin To xMax
                gfx.DrawLine(penI, xLine, yTop, xLine, yBottom)
                xLine += steinWidthHalf
            Next

            Dim yLine As Integer = topStage + offsetTop + (yMin - 1) * steinHeightHalf
            Dim xLeft As Integer = leftStage + offsetLeft
            Dim xRight As Integer = rightStage

            For y As Integer = yMin To yMax
                gfx.DrawLine(penI, xLeft, yLine, xRight, yLine)
                yLine += steinHeightHalf
            Next

        End Sub

        Private _penGridInside As Pen
        Private _penGridInsideColor As Color
        Private _penGridOutside As Pen
        Private _penGridOutsideColor As Color
        Private Function GetPenGridInside() As Pen

            Dim color As Color = SFDat.SFLay.UGrdOverlayColorPalette.ColorNormal   'INI.Editor_GridColorInside

            If IsNothing(_penGridInside) OrElse _penGridInsideColor <> color Then

                If Not IsNothing(_penGridInside) Then
                    _penGridInside.Dispose()
                End If

                _penGridInside = New Pen(color, 1)
                _penGridInsideColor = color

            End If

            Return _penGridInside

        End Function

        Private Function GetPenGridOutside() As Pen

            Dim color As Color = SFDat.SFLay.UGrdOverlayColorPalette.ColorSelected 'INI.Editor_GridColorOutside

            If IsNothing(_penGridOutside) OrElse _penGridOutsideColor <> color Then

                If Not IsNothing(_penGridOutside) Then
                    _penGridOutside.Dispose()
                End If

                _penGridOutside = New Pen(color, 1)
                _penGridOutsideColor = color

            End If

            Return _penGridOutside

        End Function

        Private Sub Paint_UndoRedo()

            With _sfd.SFLay
                If Not IsNothing(.rxUndo) Then
                    If .rxUndo.Contains(_sfd.SFRun.MousePolling.MousePos) Then
                        If _sfd.SFRun.MousePolling.LeftMouseDown Then
                            _backBufferGfx.DrawImage(.BitmapUnRe(d0:=SFLayout.UndoRedoBmp.Undo, d1:=SFLayout.UndoRedoMode.MouseDown), .rxUndo.BoundsRect)
                        Else
                            _backBufferGfx.DrawImage(.BitmapUnRe(d0:=SFLayout.UndoRedoBmp.Undo, d1:=SFLayout.UndoRedoMode.MouseOver), .rxUndo.BoundsRect)
                        End If
                    Else
                        _backBufferGfx.DrawImage(.BitmapUnRe(d0:=SFLayout.UndoRedoBmp.Undo, d1:=SFLayout.UndoRedoMode.Normal), .rxUndo.BoundsRect)
                    End If
                End If
                '-----------------
                If Not IsNothing(.rxRedo) Then
                    If .rxRedo.Contains(_sfd.SFRun.MousePolling.MousePos) Then
                        If _sfd.SFRun.MousePolling.LeftMouseDown Then
                            _backBufferGfx.DrawImage(.BitmapUnRe(d0:=SFLayout.UndoRedoBmp.Redo, d1:=SFLayout.UndoRedoMode.MouseDown), .rxRedo.BoundsRect)
                        Else
                            _backBufferGfx.DrawImage(.BitmapUnRe(d0:=SFLayout.UndoRedoBmp.Redo, d1:=SFLayout.UndoRedoMode.MouseOver), .rxRedo.BoundsRect)
                        End If
                    Else
                        _backBufferGfx.DrawImage(.BitmapUnRe(d0:=SFLayout.UndoRedoBmp.Redo, d1:=SFLayout.UndoRedoMode.Normal), .rxRedo.BoundsRect)
                    End If
                End If
            End With
        End Sub

        Private Sub PaintStock()

            If _sfd.SFInf.Generator.StockAktCount = 0 Then
                _sfd.SFRun.HScrollBarStock.SetRange(0)
                Return
            Else
                _sfd.SFRun.EditorStockValues.SetHScrollBarValue(_sfd.SFRun.HScrollBarStock.GetValue)
            End If

            'ZLVxxx If _sfd.SFRun.EditorStockMouseAnkerVerschiebung_HasValue Then
            'ZLVxxx     _sfd.SFRun.EditorStockDeltaMouseToGhost = _sfd.SFRun.EditorStockMouseAnkerVerschiebung.NewMouseAnkerPos
            'ZLVxxx End If

            Dim stockAktUBnd As Integer = _sfd.SFInf.Generator.StockAktUBnd
            Dim xOffset As Integer = 0
            Dim bmpStein As Bitmap

            With _sfd.SFRun.EditorStockValues
                'Rendern der horizontalen Steinleiste.
                For idx As Integer = .SteinVisibleAktFistIdx To .SteinVisibleAktLastIdx
                    If idx >= _sfd.SFInf.Generator.Stock.Count Then
                        Exit For
                    Else
                        'ZLVxxx If idx = _sfd.SFRun.EditorStockValues.GhostIdx AndAlso
                        'ZLVxxx     _sfd.SFRun.EditorStockValues.StockSelectedSteinJob = SelectedSteinJob.MouseMove AndAlso
                        'ZLVxxx     _sfd.SFRun.MousePolling.LeftMouseChanged = True AndAlso
                        'ZLVxxx     _sfd.SFRun.MousePolling.LeftMouseDown = True Then
                        'ZLVxxx 
                        'ZLVxxx     bmpStein = _sfd.SFRun.EditorStockValues.MoveBmpGhost
                        'ZLVxxx     _sfd.SFRun.EditorStockGhostRect = New Rectangle(xOffset + .OffsetLeft, _sfd.SFLay.rxStock.Top, bmpStein.Width, bmpStein.Height)
                        'ZLVxxx     _sfd.SFRun.EditorStockDeltaMouseToGhost = New Point(_sfd.SFRun.MousePolling.MousePos.X - xOffset, _sfd.SFRun.MousePolling.MousePos.Y - _sfd.SFLay.rxStock.Top)
                        'ZLVxxx     _sfd.SFRun.EditorStockMouseAnkerVerschiebung = New MouseAnkerVerschiebung(_sfd, _sfd.SFRun.EditorStockDeltaMouseToGhost)
                        'ZLVxxx 
                        'ZLVxxx ElseIf idx = _sfd.SFRun.EditorStockValues.GhostIdx Then
                        'ZLVxxx     bmpStein = _sfd.SFRun.EditorStockValues.MoveBmpGhost
                        'ZLVxxx 
                        'ZLVxxx Else
                        'ZLVxxx 

                        Dim request As New TileRequest(_sfd.SFRun.AktRenderMode, INI.Tile_TileColors, steinTyp:=_sfd.SFInf.Generator.Stock(idx), SteinStatus.I01Normal, SteinFrameVersion.Standard, _sfd.SFLay.steinSize, INI.Tile_BasisSize)
                        bmpStein = TileFactory.GetTile(request)

                        'ZLVxxx End If

                        Dim rectAusgabe As Rectangle = New Rectangle(xOffset + .OffsetLeft, _sfd.SFLay.rxStock.Top, bmpStein.Width, bmpStein.Height)

                        'ZLVxxx If Not _sfd.SFRun.MousePolling.LeftMouseDown AndAlso rectAusgabe.Contains(_sfd.SFRun.MousePolling.MousePos) Then
                        'ZLVxxx     bmpStein = DrawOverlay(bmpStein, OverlayType.RahmenSteinMouseOver, copyBitmap:=True)
                        'ZLVxxx     _backBufferGfx.DrawImage(bmpStein, rectAusgabe)
                        'ZLVxxx     bmpStein.Dispose()
                        'ZLVxxx Else
                        _backBufferGfx.DrawImage(bmpStein, rectAusgabe)
                        'ZLVxxx End If

                        xOffset += _sfd.SFLay.steinWidth
                    End If
                Next
                '
                '
                Dim pos3D As New Triple

                'ZLVxxx If .StockSelectedSteinJob = SelectedSteinJob.MouseMove Then
                'ZLVxxx     Dim ptCur As Point = New Point(_sfd.SFRun.MousePolling.MousePos.X - _sfd.SFRun.EditorStockDeltaMouseToGhost.X, _sfd.SFRun.MousePolling.MousePos.Y - _sfd.SFRun.EditorStockDeltaMouseToGhost.Y)
                'ZLVxxx 
                'ZLVxxx     pos3D = _sfd.SFInf.IsFundamentKandidat(_sfd.SFRun.MousePolling.MousePos)
                'ZLVxxx 
                'ZLVxxx     If pos3D.IsValideYes Then
                'ZLVxxx         _sfd.SFRun.EditorSteinDoPlaceAtPosTriple = pos3D
                'ZLVxxx         _sfd.SFRun.EditorSteinDoPlace_SteinIndex = .MoveSelectedSteinIndex
                'ZLVxxx 
                'ZLVxxx         'Die GhostBmp ist vom Steinvorrat und wird beim Klick auf einen Stein im Vorrat erzeugt,
                'ZLVxxx         'wird aber erst überschrieben, wenn die Nächste erzeugt wird und kann daher hier verwendet werden. 
                'ZLVxxx 
                'ZLVxxx         _backBufferGfx.DrawImage(_sfd.SFRun.EditorStockValues.MoveBmpGhost, _sfd.SFInf.GetSteinRenderRect(pos3D))
                'ZLVxxx         bmpStein = _sfd.SFRun.EditorStockValues.MoveBmpPlaceable
                'ZLVxxx     Else
                'ZLVxxx         bmpStein = _sfd.SFRun.EditorStockValues.MoveBmpSelected
                'ZLVxxx         'löschen, sonst fliegt der Stein nicht zurück, sondern wird auf
                'ZLVxxx         'dem zuletzt gefundenem gültigem Platz abgelegt.
                'ZLVxxx         _sfd.SFRun.EditorSteinDoPlaceAtPosTriple = New Triple
                'ZLVxxx     End If
                'ZLVxxx     'zuletzt als oberstes den an die Maus gekoppelten Stein
                'ZLVxxx     _backBufferGfx.DrawImage(bmpStein, ptCur)
                'ZLVxxx End If

                'XXXzlv If .StockSelectedSteinJob = SelectedSteinJob.ChoiseInsertOrFlyBack Then
                'XXXzlv     'Hier Entscheidung
                'XXXzlv     If _sfd.SFRun.EditorSteinDoPlaceAtPosTriple.IsValideYes Then
                'XXXzlv 
                'XXXzlv         .StockSelectedSteinJob = SelectedSteinJob.Insert_Do
                'XXXzlv 
                'XXXzlv     Else
                'XXXzlv         .StockSelectedSteinJob = SelectedSteinJob.FlyBack_Do
                'XXXzlv     End If
                'XXXzlv End If
                'XXXzlv 
                'XXXzlv If .StockSelectedSteinJob = SelectedSteinJob.FlyBack_Do Then
                'XXXzlv 
                'XXXzlv     Dim ptCur As Point = _sfd.SFRun.EditorStockSteinFlugValues.GetNextPoint(_timeDifferenzFaktor)
                'XXXzlv 
                'XXXzlv     Dim idx As Integer = _sfd.SFRun.EditorStockValues.GhostIdx
                'XXXzlv     If idx = -1 Then
                'XXXzlv         idx = _sfd.SFRun.EditorStockValues.SelectedStockSteinIdx
                'XXXzlv     End If
                'XXXzlv     bmpStein = Images.SGM.GetStein(index:=_sfd.SFInf.Generator.Stock(idx),
                'XXXzlv                             status:=SteinStatus.I01Normal,
                'XXXzlv                            _sfd.SFLay.steinSize,
                'XXXzlv                            _sfd.SFRun.AktRenderMode)
                'XXXzlv 
                'XXXzlv     _backBufferGfx.DrawImage(bmpStein, ptCur)
                'XXXzlv     If _sfd.SFRun.EditorStockSteinFlugValues.IsFinished Then
                'XXXzlv         .StockSelectedSteinJob = SelectedSteinJob.FlyBack_Done
                'XXXzlv     End If
                'XXXzlv End If
                'XXXzlv 
                'XXXzlv If .StockSelectedSteinJob = SelectedSteinJob.Insert_Do Then
                'XXXzlv 
                'XXXzlv     _sfd.SFInf.AddSteinToSpielfeld(_sfd.SFRun.EditorSteinDoPlace_SteinIndex, _sfd.SFRun.EditorSteinDoPlaceAtPosTriple)
                'XXXzlv 
                'XXXzlv     _sfd.SFRun.EditorSteinDoPlaceAtPosTriple = New Triple
                'XXXzlv     .StockSelectedSteinJob = SelectedSteinJob.Insert_Done
                'XXXzlv End If

            End With

        End Sub

#Region "DebugHelfer"

        Private Sub DebugDrawKreuz(gfx As Graphics, rect As Rectangle)
            'Debug-Kreuz im Zielrechteck zeichnen
            Using pWhite As New Pen(Color.White, 1.0F),
                  pBlack As New Pen(Color.Black, 1.0F)

                'weißes Kreuz
                _backBufferGfx.DrawLine(pWhite, rect.Left, rect.Top, rect.Right - 1, rect.Bottom - 1)
                _backBufferGfx.DrawLine(pWhite, rect.Right - 1, rect.Top, rect.Left, rect.Bottom - 1)

                'schwarzes Kreuz leicht versetzt
                _backBufferGfx.DrawLine(pBlack, rect.Left + 1, rect.Top, rect.Right - 1, rect.Bottom - 2)
                _backBufferGfx.DrawLine(pBlack, rect.Right - 2, rect.Top, rect.Left, rect.Bottom - 1)

            End Using
        End Sub

#End Region

#Region "Helfer"
        Private Function Bitmap32DeepCopy(bmp As Bitmap) As Bitmap
            Return bmp.Clone(New Rectangle(0, 0, bmp.Width, bmp.Height), bmp.PixelFormat)
        End Function
#End Region
    End Class
End Namespace