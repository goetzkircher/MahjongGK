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

    ''' <summary>
    ''' Hier sind die Zeichenroutinen angesiedelt.
    ''' Auch das ist keine Klasse, weil von vorneherein feststeht, daß keine
    ''' Instanzierung notwenig ist.
    ''' </summary>
    Module SpielfeldRenderer


        'Hinweis: Die Daten, auf die hier ohne Deklaration zugegriffen wird, 
        'stehen im Modul "SpielfeldDaten, kurz SFD"

        Public Sub DoPaintSpielfeld_Paint(PaintEventGfx As Graphics, rectOutput As Rectangle, timeDifferenzFaktor As Double)

            If SFD.AktSpielfeldInfo.HasBitmapUGrd Then
                PaintEventGfx.DrawImage(SFD.AktSpielfeldInfo.GetBitmapUGrd(rectOutput.Size), Point.Empty)
            Else
                PaintEventGfx.Clear(SFD.AktSpielfeldInfo.HGrdSplFldColor)
            End If

            'Es wird grundsätzlich in den Backpuffer gezeichnet, der am Ende dann auf das Control geplittet wird.
            SFD.CreateBackbufferAndGfx()
            SFD.Backbuffer_HasContent = True





            'Den Zeichenknecht des Background-Backbuffers setzen
            SFD.rxOutput?.SetGfx(SFD.BackBufferGfx)
            SFD.rxStock?.SetGfx(SFD.BackBufferGfx)
            SFD.rxStockScrollbar?.SetGfx(SFD.BackBufferGfx)
            SFD.rxStockMark?.SetGfx(SFD.BackBufferGfx)
            SFD.rxBitmapUgrd?.SetGfx(SFD.BackBufferGfx)
            SFD.rxHeader?.SetGfx(SFD.BackBufferGfx)
            SFD.rxContent?.SetGfx(SFD.BackBufferGfx)
            SFD.rxHistoryBoxLeft?.SetGfx(SFD.BackBufferGfx)
            SFD.rxHistoryBoxRight?.SetGfx(SFD.BackBufferGfx)
            SFD.rxHistoryBoxLeftScrollbar?.SetGfx(SFD.BackBufferGfx)
            SFD.rxHistoryBoxRightScrollbar?.SetGfx(SFD.BackBufferGfx)
            SFD.rxStageAvailable?.SetGfx(SFD.BackBufferGfx)
            SFD.rxStageUsed?.SetGfx(SFD.BackBufferGfx)


            If INI.Rendering_DrawRenderRect Then
                SFD.DebugLabels.Start(rectOutput)
                'nur zum Debuggen: die Umrisskanten der Renderbereiche zeichnen.
                PaintEventGfx.Clear(Color.Silver)
                SFD.rxOutput?.SetDebugHost(SFD.VisibleUserControlUCtl)
                SFD.rxStock?.SetDebugHost(SFD.VisibleUserControlUCtl)
                SFD.rxStockScrollbar?.SetDebugHost(SFD.VisibleUserControlUCtl)
                SFD.rxStockMark?.SetDebugHost(SFD.VisibleUserControlUCtl)
                SFD.rxBitmapUgrd?.SetDebugHost(SFD.VisibleUserControlUCtl)
                SFD.rxHeader?.SetDebugHost(SFD.VisibleUserControlUCtl)
                SFD.rxContent?.SetDebugHost(SFD.VisibleUserControlUCtl)
                SFD.rxHistoryBoxLeft?.SetDebugHost(SFD.VisibleUserControlUCtl)
                SFD.rxHistoryBoxRight?.SetDebugHost(SFD.VisibleUserControlUCtl)
                SFD.rxHistoryBoxLeftScrollbar?.SetDebugHost(SFD.VisibleUserControlUCtl)
                SFD.rxHistoryBoxRightScrollbar?.SetDebugHost(SFD.VisibleUserControlUCtl)
                SFD.rxStageAvailable?.SetDebugHost(SFD.VisibleUserControlUCtl)
                SFD.rxStageUsed?.SetDebugHost(SFD.VisibleUserControlUCtl)

                SFD.rxOutput?.DrawDebug()
                SFD.rxBitmapUgrd?.DrawDebug()
                SFD.rxContent?.DrawDebug()
                SFD.rxHeader?.DrawDebug()
                SFD.rxStock?.DrawDebug()
                SFD.rxStockScrollbar?.DrawDebug()
                SFD.rxStockMark?.DrawDebug()
                SFD.rxHistoryBoxLeft?.DrawDebug()
                SFD.rxHistoryBoxRight?.DrawDebug()
                SFD.rxHistoryBoxLeftScrollbar?.DrawDebug()
                SFD.rxHistoryBoxRightScrollbar?.DrawDebug()
                SFD.rxStageAvailable?.DrawDebug()
                SFD.rxStageUsed?.DrawDebug()

            End If


            If INI.Rendering_DrawRenderingSkipDoneMarker Then
                'unterer rechter Marker, wenn der Backpuffer wiederverwendet wird
                DrawRenderingSkipDoneMarker(PaintEventGfx, rectOutput.Width, rectOutput.Height, SFD.RenderingDoneCounter, "Done")
            End If


            If SFD.AktSpielfeldInfo.IsEmpty Then
                SFD.rxHeader?.DrawStringCentered("Keine Daten geladen", New Font("Arial", 16, FontStyle.Bold), usePadding:=False, foreColor:=Nothing)
                Exit Sub
            End If



            SFD.rxHeader?.DrawStringCentered("Hallo", New Font("Arial", 16, FontStyle.Bold), usePadding:=False, foreColor:=Nothing)


            '   Exit Sub

            Using pen As New Pen(INI.Rendering_RenderRectColor)

                If SFD.AktRendering = RenderingEnum.Editor Then
                Else
                End If

            End Using

            If Not IsNothing(SFD.HScrollStock) Then
                'If HScrollRenderer.PumpAutoRepeat Then
                SFD.HScrollStock.PaintHScroll(SFD.BackBufferGfx, enabled:=True)
                'End If
            End If

            Dim aktSteinInfo As SteinInfo

            Dim toggleVergleichsflag As Boolean = SFD.AktSpielfeldInfo.GetFirstToggleFlagValue

            'Die Variable, auf die hier zugegriffen wird, und die nicht hier
            'deklariert sind, stehen alle im Modul SpielfeldDaten. (geändert, alle über SFD....)
            With SFD.AktSpielfeldInfo

                'Dim debugInfo As String = String.Empty

                For z As Integer = SFD.zMin To SFD.zMax
                    For x As Integer = SFD.xMin To SFD.xMax
                        For y As Integer = SFD.yMin To SFD.yMax

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
                                If .AnimShowAnimated Then
                                    PaintAnimatedStein(PaintEventGfx, rectOutput, timeDifferenzFaktor, aktSteinInfo, New Triple(x, y, z))
                                Else
                                    Dim left As Integer = SFD.rxStageUsed.Left + (SFD.steinWidthHalf * (x - 1) + SFD.offset3DLeftSumme - (SFD.offset3DLeftJeEbene * z))
                                    Dim top As Integer = SFD.rxStageUsed.Top + (SFD.steinHeightHalf * (y - 1) + SFD.offset3DTopSumme - (SFD.offset3DTopJeEbene * z))
                                    ''Debug
                                    'debugInfo &= $"x={x},y={y},z={z},arrFB-SteinIdx={ SFD.AktSpielfeldInfo.GetIndexStein(x, y, z)},SII={aktSteinInfo.SteinInfoIndex},SI={aktSteinInfo.SteinIndex}|"
                                    ''/Debug
                                    Dim bmp As Bitmap = SGM.GetStein(SFD.AktRendering, .SteinStatusUsed, .SteinIndex, SFD.steinSize, sichtbar:=SichtbarResult.Full, showGhost:=True)
                                    'alte Version:PaintEventGfx.DrawImage(BitmapContainer.GetBitmap(.SteinStatusUsed, .SteinIndex), left, top)
                                    SFD.BackBufferGfx.DrawImage(bmp, left, top)

                                    If INI.IfRunningInIDE_InsertStoneIndex Then

                                        Dim dbg As String = String.Format("{0}-{1}-{2}-{3}", CType(.tmpDebug, PositionEnum), x, y, z)

                                        Dim r As New RectangleF(CSng(left + SFD.steinWidth * 0.1), CSng(top + SFD.steinHeight * 0.02), CSng(SFD.steinWidth * 0.65), CSng(SFD.steinHeight * 0.85))

                                        Using f As New Font("Consolas", 7.0F, FontStyle.Regular, GraphicsUnit.Point)
                                            Using sf As New StringFormat(StringFormat.GenericTypographic)
                                                sf.Alignment = StringAlignment.Center
                                                sf.FormatFlags = StringFormatFlags.NoWrap
                                                sf.LineAlignment = StringAlignment.Near     ' oben
                                                SFD.BackBufferGfx.DrawString(dbg, f, Brushes.Black, r, sf)
                                                sf.LineAlignment = StringAlignment.Far     ' unten
                                                SFD.BackBufferGfx.DrawString(dbg, f, Brushes.Black, r, sf)

                                            End Using
                                        End Using

                                        ' 1) Rechts oben, ohne Abstand
                                        Using f As New Font("Consolas", 7.0F, FontStyle.Regular, GraphicsUnit.Point)
                                            Using sf As New StringFormat(StringFormat.GenericTypographic)
                                                sf.Alignment = StringAlignment.Center
                                                sf.LineAlignment = StringAlignment.Near     ' oben
                                                sf.FormatFlags = StringFormatFlags.NoWrap
                                                SFD.BackBufferGfx.DrawString(dbg, f, Brushes.Black, r, sf)
                                                sf.LineAlignment = StringAlignment.Far      ' unten
                                                SFD.BackBufferGfx.DrawString(dbg, f, Brushes.Black, r, sf)
                                            End Using

                                            ' 2) Vertikal links (oben -> unten), höhenzentriert
                                            Dim stLeft As Drawing2D.GraphicsState = SFD.BackBufferGfx.Save()
                                            SFD.BackBufferGfx.TranslateTransform(r.Left, r.Top + r.Height / 2.0F)
                                            SFD.BackBufferGfx.RotateTransform(-90.0F) ' oben -> unten am linken Rand

                                            Using sfV As New StringFormat(StringFormat.GenericTypographic)
                                                sfV.Alignment = StringAlignment.Center
                                                sfV.LineAlignment = StringAlignment.Center
                                                sfV.FormatFlags = StringFormatFlags.NoWrap
                                                Dim lineH As Single = f.GetHeight(SFD.BackBufferGfx)
                                                Dim layout As New RectangleF(-r.Height / 2.0F, -lineH / 2.0F, r.Height, lineH)
                                                SFD.BackBufferGfx.DrawString(dbg, f, Brushes.Black, layout, sfV)
                                            End Using
                                            SFD.BackBufferGfx.Restore(stLeft)

                                            ' 3) Vertikal rechts (oben -> unten), höhenzentriert
                                            Dim stRight As Drawing2D.GraphicsState = PaintEventGfx.Save()
                                            SFD.BackBufferGfx.TranslateTransform(r.Right, r.Top + r.Height / 2.0F)
                                            SFD.BackBufferGfx.RotateTransform(90.0F) ' oben -> unten am rechten Rand

                                            Using sfV As New StringFormat(StringFormat.GenericTypographic)
                                                sfV.Alignment = StringAlignment.Center
                                                sfV.LineAlignment = StringAlignment.Center
                                                sfV.FormatFlags = StringFormatFlags.NoWrap
                                                Dim lineH As Single = f.GetHeight(SFD.BackBufferGfx)
                                                Dim layout As New RectangleF(-r.Height / 2.0F, -lineH / 2.0F, r.Height, lineH)
                                                SFD.BackBufferGfx.DrawString(dbg, f, Brushes.Black, layout, sfV)
                                            End Using
                                            SFD.BackBufferGfx.Restore(stRight)
                                        End Using


                                    End If

                                End If
                            End With
                        Next
                    Next
                Next
                'Debug.Print(debugInfo)

                PaintEventGfx.DrawImageUnscaled(SFD.Backbuffer, rectOutput.Location)

            End With
        End Sub

        Private Sub PaintAnimatedStein(gfx As Graphics, rectOutput As Rectangle, timeDifferenzFaktor As Double, aktSteinInfo As SteinInfo, pos3D As Triple)

            'Dim left As Integer = SFD.renderRectLeft + (SFD.steinWidthHalf * pos3D.x) - (SFD.offset3DLeftJeEbene * pos3D.z)
            'Dim top As Integer = SFD.renderRectTop + (SFD.steinHeightHalf * pos3D.y) - (SFD.offset3DTopJeEbene * pos3D.z)

            'Dim rectStein As New Rectangle(left, top, SFD.steinWidth, SFD.steinHeight)

        End Sub


        Private Sub PaintMainLayout()


        End Sub


    End Module
End Namespace