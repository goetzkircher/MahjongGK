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
        Public Sub RenderingHauptverteiler(PaintEventGfx As Graphics, rectOutput As Rectangle, timeDifferenzFaktor As Double)

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
                    _sfd.SFRun.EditorFrmTooltipSteinInfo.UpdateInfo(
                        _sfd.SFInf.GetSteinToolTipInfos(_sfd.SFRun.MousePolling.MousePos),
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

                            aktSteinInfo = .SteinInfos(.GetSteinInfoIndex(x, y, z))

                            Dim tsi As TopSteinInfo = _sfd.SFInf.GetTopSteinInfo(x, y, z)

                            If tsi IsNot Nothing Then

                                Dim request As New TileRequest(_sfd.SFRun.AktRenderMode, INI.Tile_TileColors, tsi.SteinTypIndex, tsi.SteinStatus, SteinFrameVersion.Standard, _sfd.SFLay.steinSize, INI.Tile_BasisSize)
                                bmpStein = TileFactory.GetTile(request)

                                _backBufferGfx.DrawImage(bmpStein, aktSteinInfo.RectStein)

                            Else
                                'Zeichnet alle Steine, die nicht die obersten Steine sind.
                                Dim request As New TileRequest(_sfd.SFRun.AktRenderMode, INI.Tile_TileColors, aktSteinInfo.SteinTypIndex, SteinStatus.I01Normal, SteinFrameVersion.Standard, _sfd.SFLay.steinSize, INI.Tile_BasisSize)
                                bmpStein = TileFactory.GetTile(request)

                                _backBufferGfx.DrawImage(bmpStein, aktSteinInfo.RectStein)

                            End If
                        Next
                    Next
                Next

            End With

            PaintStock()

            Paint_UndoRedo()

            'Hier sind Ausgaben angesiedelt, die in der Z-Order ganz oben aufliegen.
            'Reihenfolge beachten, bestimmt die Z-Order innerhalb der SpecialBmps..
            Dim values As (has As Boolean, bmp As Bitmap, rect As Rectangle)
            '
            values = _sfd.SFMouse.GetSpecialBmps(SpecialBmps.AtEditorSrcPos)
            If values.has Then
                _backBufferGfx.DrawImage(values.bmp, values.rect)
            End If
            '
            values = _sfd.SFMouse.GetSpecialBmps(SpecialBmps.AtEditorCanDropPos)
            If values.has Then
                _backBufferGfx.DrawImage(values.bmp, values.rect)
            End If
            '
            values = _sfd.SFMouse.GetSpecialBmps(SpecialBmps.AtEditorMousePos)
            If values.has Then
                _backBufferGfx.DrawImage(values.bmp, values.rect)
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

            Dim stockAktUBnd As Integer = _sfd.SFInf.Generator.StockAktUBnd
            Dim bmpStein As Bitmap

            With _sfd.SFStock
                'Rendern der horizontalen Steinleiste.
                For idx As Integer = .SteinVisibleAktFistIdx To .SteinVisibleAktLastIdx
                    If idx >= _sfd.SFInf.Generator.Stock.Count Then
                        Exit For
                    Else

                        Dim request As New TileRequest(_sfd.SFRun.AktRenderMode, INI.Tile_TileColors, steinTyp:=_sfd.SFInf.Generator.Stock(idx), SteinStatus.I01Normal, SteinFrameVersion.Standard, _sfd.SFLay.steinSize, INI.Tile_BasisSize)
                        bmpStein = TileFactory.GetTile(request)

                        Dim rectAusgabe As Rectangle = _sfd.SFStock.GetRectFromStockSteinIdx(idx)
                        _backBufferGfx.DrawImage(bmpStein, rectAusgabe)
                    End If
                Next
                '
                '

            End With

            Dim values As (has As Boolean, bmp As Bitmap, rect As Rectangle)
            '
            values = _sfd.SFMouse.GetSpecialBmps(SpecialBmps.AtStockCanDropPos)
            If values.has Then
                _backBufferGfx.DrawImage(values.bmp, values.rect)
            End If

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