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
Imports System.Drawing.Drawing2D



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
        'stehen im Modul "SpielfeldDaten"

        Public Sub DoPaintSpielfeld_Paint(gfx As Graphics, rectOutput As Rectangle, timeDifferenzFaktor As Double, clear As Boolean)

            If SFD.AktRendering = RenderingEnum.None OrElse clear OrElse IsNothing(SFD.AktSpielfeldInfo) OrElse SFD.AktSpielfeldInfo.IsEmpty Then

                gfx.Clear(INI.Rendering_BackgroundColor)

                Dim text As String = Nothing

                Select Case SFD.AktRendering
                    Case RenderingEnum.None
                        text = "Es ist Pause - Du kannst ein Spiel laden."
                    Case RenderingEnum.Spielfeld
                        text = "Es ist kein Spiel geladen."
                    Case RenderingEnum.Editor
                        text = "Es ist kein Spielentwurf geladen."
                    Case RenderingEnum.Werkbank
                        text = "Es ist kein Basiselement geladen."
                End Select

                Dim fnt As Font = INI.Rendering_EmptyMessageFont

                gfx.DrawString(text, fnt, If(INI.Global_DarkMode, Brushes.White, Brushes.Black), New PointF(rectOutput.Left + 20, rectOutput.Top + 20))

                fnt.Dispose()

                Exit Sub
            End If

            If SFD.AktSpielfeldInfo.IsEmpty Then
                gfx.Clear(INI.Rendering_BackgroundColor)
                Exit Sub
            End If


            If SFD.AktSpielfeldInfo.HasBitmapUGrd Then
                gfx.DrawImage(SFD.AktSpielfeldInfo.GetBitmapUGrd(rectOutput.Size), Point.Empty)
            Else
                gfx.Clear(SFD.AktSpielfeldInfo.HGrdSplFldColor)
            End If


            If INI.Rendering_DrawRenderRect Then
                Using pen As New Pen(INI.Rendering_RenderRectColor)

                    If SFD.AktRendering = RenderingEnum.Editor Then

                        pen.DashStyle = DashStyle.Solid
                        gfx.DrawRectangle(pen, SFD.ugrdRect)

                        pen.DashStyle = DashStyle.Dash
                        gfx.DrawRectangle(pen, SFD.stockRect)

                        pen.DashStyle = DashStyle.Dot
                        gfx.DrawRectangle(pen, SFD.scrollbarRect)

                        pen.DashStyle = DashStyle.Solid
                        gfx.DrawRectangle(pen, SFD.splfldFullRect)

                        pen.DashStyle = DashStyle.Dot
                        gfx.DrawRectangle(pen, SFD.splfldUsedRect)
                        gfx.DrawString()
                    Else

                        pen.DashStyle = DashStyle.Solid
                        gfx.DrawRectangle(pen, SFD.ugrdRect)

                        pen.DashStyle = DashStyle.Dash
                        gfx.DrawRectangle(pen, SFD.splfldFullRect)

                        pen.DashStyle = DashStyle.Dot
                        gfx.DrawRectangle(pen, SFD.splfldUsedRect)

                    End If

                End Using
            End If

            'If SFD.AktRendering = RenderingEnum.Editor Then
            '    'If HScrollRenderer.PumpAutoRepeat Then
            '    HScrollRenderer.PaintHScroll(gfx, scrollbarRect, Color.Cornsilk, enabled:=True)
            '    'End If
            'End If

            Dim aktSteinInfo As SteinInfo

            Dim toggleVergleichsflag As Boolean = SFD.AktSpielfeldInfo.GetFirstToggleFlagValue

            'Die Vaiable, auf die hier zugegriffen wird, und die nicht hier
            'deklariert sind, stehen alle im Modul SpielfeldDaten.
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
                                    PaintAnimatedStein(gfx, rectOutput, timeDifferenzFaktor, aktSteinInfo, New Triple(x, y, z))
                                Else
                                    Dim left As Integer = SFD.splfldUsedRect.Left + (SFD.steinWidthHalf * (x - 1) + SFD.offset3DLeftSumme - (SFD.offset3DLeftJeEbene * z))
                                    Dim top As Integer = SFD.splfldUsedRect.Top + (SFD.steinHeightHalf * (y - 1) + SFD.offset3DTopSumme - (SFD.offset3DTopJeEbene * z))
                                    ''Debug
                                    'debugInfo &= $"x={x},y={y},z={z},arrFB-SteinIdx={ SFD.AktSpielfeldInfo.GetIndexStein(x, y, z)},SII={aktSteinInfo.SteinInfoIndex},SI={aktSteinInfo.SteinIndex}|"
                                    ''/Debug
                                    gfx.DrawImage(BitmapContainer.GetBitmap(.SteinStatusUsed, .SteinIndex), left, top)

                                    If INI.IfRunningInIDE_InsertStoneIndex Then

                                        Dim dbg As String = String.Format("{0}-{1}-{2}-{3}", CType(.tmpDebug, PositionEnum), x, y, z)

                                        Dim r As New RectangleF(CSng(left + steinWidth * 0.1), CSng(top + steinHeight * 0.02), CSng(steinWidth * 0.65), CSng(steinHeight * 0.85))

                                        Using f As New Font("Consolas", 7.0F, FontStyle.Regular, GraphicsUnit.Point)
                                            Using sf As New StringFormat(StringFormat.GenericTypographic)
                                                sf.Alignment = StringAlignment.Center
                                                sf.FormatFlags = StringFormatFlags.NoWrap
                                                sf.LineAlignment = StringAlignment.Near     ' oben
                                                gfx.DrawString(dbg, f, Brushes.Black, r, sf)
                                                sf.LineAlignment = StringAlignment.Far     ' unten
                                                gfx.DrawString(dbg, f, Brushes.Black, r, sf)

                                            End Using
                                        End Using

                                        ' 1) Rechts oben, ohne Abstand
                                        Using f As New Font("Consolas", 7.0F, FontStyle.Regular, GraphicsUnit.Point)
                                            Using sf As New StringFormat(StringFormat.GenericTypographic)
                                                sf.Alignment = StringAlignment.Center
                                                sf.LineAlignment = StringAlignment.Near     ' oben
                                                sf.FormatFlags = StringFormatFlags.NoWrap
                                                gfx.DrawString(dbg, f, Brushes.Black, r, sf)
                                                sf.LineAlignment = StringAlignment.Far      ' unten
                                                gfx.DrawString(dbg, f, Brushes.Black, r, sf)
                                            End Using

                                            ' 2) Vertikal links (oben -> unten), höhenzentriert
                                            Dim stLeft As Drawing2D.GraphicsState = gfx.Save()
                                            gfx.TranslateTransform(r.Left, r.Top + r.Height / 2.0F)
                                            gfx.RotateTransform(-90.0F) ' oben -> unten am linken Rand

                                            Using sfV As New StringFormat(StringFormat.GenericTypographic)
                                                sfV.Alignment = StringAlignment.Center
                                                sfV.LineAlignment = StringAlignment.Center
                                                sfV.FormatFlags = StringFormatFlags.NoWrap
                                                Dim lineH As Single = f.GetHeight(gfx)
                                                Dim layout As New RectangleF(-r.Height / 2.0F, -lineH / 2.0F, r.Height, lineH)
                                                gfx.DrawString(dbg, f, Brushes.Black, layout, sfV)
                                            End Using
                                            gfx.Restore(stLeft)

                                            ' 3) Vertikal rechts (oben -> unten), höhenzentriert
                                            Dim stRight As Drawing2D.GraphicsState = gfx.Save()
                                            gfx.TranslateTransform(r.Right, r.Top + r.Height / 2.0F)
                                            gfx.RotateTransform(90.0F) ' oben -> unten am rechten Rand

                                            Using sfV As New StringFormat(StringFormat.GenericTypographic)
                                                sfV.Alignment = StringAlignment.Center
                                                sfV.LineAlignment = StringAlignment.Center
                                                sfV.FormatFlags = StringFormatFlags.NoWrap
                                                Dim lineH As Single = f.GetHeight(gfx)
                                                Dim layout As New RectangleF(-r.Height / 2.0F, -lineH / 2.0F, r.Height, lineH)
                                                gfx.DrawString(dbg, f, Brushes.Black, layout, sfV)
                                            End Using
                                            gfx.Restore(stRight)
                                        End Using


                                    End If

                                End If
                            End With
                        Next
                    Next
                Next
                'Debug.Print(debugInfo)
            End With
        End Sub

        Private Sub PaintAnimatedStein(gfx As Graphics, rectOutput As Rectangle, timeDifferenzFaktor As Double, aktSteinInfo As SteinInfo, pos3D As Triple)

            'Dim left As Integer = SFD.renderRectLeft + (SFD.steinWidthHalf * pos3D.x) - (SFD.offset3DLeftJeEbene * pos3D.z)
            'Dim top As Integer = SFD.renderRectTop + (SFD.steinHeightHalf * pos3D.y) - (SFD.offset3DTopJeEbene * pos3D.z)

            'Dim rectStein As New Rectangle(left, top, SFD.steinWidth, SFD.steinHeight)

        End Sub

    End Module
End Namespace