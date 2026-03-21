'
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
Imports System.Drawing.Drawing2D
Imports System.Drawing.Text

Namespace Spielfeld

    Public Enum OverlayType
        EditorRemovable
        RahmenSteinMouseOver
        RahmenSteinSelected
        RahmenSteinPlaceable
    End Enum

    ''' <summary>
    ''' Pfad: MahjongGK/Spielfeld/Render
    ''' </summary>
    Module RenderHelper
        ' ─────────────────────────────────────────────────────────────────────────────
        ' Kleiner, kontrastreicher Marker unten rechts als Debughilfe.
        ' Zeigt RenderingDoneCounter und RenderingSkipCounter
        ' (ändert NICHT den Backpuffer)
        ' wenn RenderingDoneCounter = 1, blitzt die Markierung rot auf.
        ' damit ist sichtbar, ob mindestens 1 Rendering durchgeführt wurde.
        ' ─────────────────────────────────────────────────────────────────────────────
        Private ReadOnly _penWhite As New Pen(Color.FromArgb(240, 255, 255, 255), 1.0F)
        Private ReadOnly _penBlack As New Pen(Color.FromArgb(240, 0, 0, 0), 1.0F)
        Private ReadOnly _brushDot As New SolidBrush(Color.FromArgb(220, 0, 0, 0))

        Public Sub DrawRenderingSkipDoneMarker(g As Graphics, surfaceW As Integer, surfaceH As Integer, counter As Integer, srcText As String, Optional scaled As Boolean = False)

            If surfaceW <= 100 OrElse surfaceH <= 100 Then Exit Sub

            Dim w As Integer = 130               ' Markergröße
            Dim h As Integer = 20
            Dim x As Integer = surfaceW - w - 10
            Dim y As Integer = surfaceH - h - 10

            Dim r As New Rectangle(x, y, w, h)

            ' Schnell zeichnen, keine Extras
            Dim oldSmoothing As SmoothingMode = g.SmoothingMode
            Dim oldPixel As PixelOffsetMode = g.PixelOffsetMode
            g.SmoothingMode = SmoothingMode.None
            g.PixelOffsetMode = PixelOffsetMode.Half

            ' Doppelrahmen (weiß außen, schwarz innen) -> auf jedem Hintergrund sichtbar
            g.DrawRectangle(_penWhite, r)
            Dim r2 As New Rectangle(r.X + 1, r.Y + 1, r.Width - 3, r.Height - 3)
            g.DrawRectangle(_penBlack, r2)
            r2.Width -= 1 : r2.Height -= 1

            ' Diagonaler Punkt (Mini-Dreieck) als Füllung
            '  • schnell, null Allokation, gut erkennbar
            Using fnt As New Font("Arial", 12, FontStyle.Regular, GraphicsUnit.Pixel)

                If srcText = "Done" Then 'alternativ If SFD.RenderingDoneCounter >= 1 Then
                    g.FillRectangle(Brushes.LightGreen, r2)
                Else
                    If scaled Then
                        g.FillRectangle(Brushes.Yellow, r2)
                    Else
                        g.FillRectangle(Brushes.Magenta, r2)
                    End If
                End If
                'TODO SFD-Anpassung abgechaltet
                ' g.DrawString(_sfd.AktRendering.ToString & " - " & srcText & " - " & counter.ToString, fnt, Brushes.Black, r.X + 4, r.Y + 2)

            End Using

            g.SmoothingMode = oldSmoothing
            g.PixelOffsetMode = oldPixel
        End Sub

        ''    Public Sub DrawRuntimeOnly_AktRendering(g As Graphics, surfaceW As Integer, surfaceH As Integer)
        ''        If surfaceW <= 100 OrElse surfaceH <= 100 Then Exit Sub


        ''        Dim w As Integer = 75               ' Markergröße
        ''        Dim h As Integer = 20
        ''        Dim x As Integer = surfaceW - w - 10
        ''        Dim y As Integer = 10

        ''        Dim r As New Rectangle(x, y, w, h)

        ''        Schnell zeichnen, keine Extras
        ''        Dim oldSmoothing As SmoothingMode = g.SmoothingMode
        ''        Dim oldPixel As PixelOffsetMode = g.PixelOffsetMode
        ''        g.SmoothingMode = SmoothingMode.None
        ''        g.PixelOffsetMode = PixelOffsetMode.Half

        ''        Doppelrahmen(weiß außen, schwarz innen) -> auf jedem Hintergrund sichtbar
        ''        g.DrawRectangle(_penWhite, r)
        ''        Dim r2 As New Rectangle(r.X + 1, r.Y + 1, r.Width - 2, r.Height - 2)
        ''        g.DrawRectangle(_penBlack, r2)

        ''        Diagonaler Punkt(Mini - Dreieck) als Füllung
        ''          • schnell, null Allokation, gut erkennbar
        ''        Using fnt As New Font("Arial", 12, FontStyle.Regular, GraphicsUnit.Pixel)

        ''            If SFD.RenderingDoneCounter >= 1 Then
        ''                g.FillRectangle(Brushes.LightGreen, r2)
        ''            Else
        ''                g.FillRectangle(Brushes.Magenta, r2)
        ''            End If

        ''            g.DrawString(INI.RuntimeOnly_AktRendering.ToString, fnt, Brushes.Black, r.X + 4, r.Y + 2)

        ''        End Using

        ''        g.SmoothingMode = oldSmoothing
        ''        g.PixelOffsetMode = oldPixel
        ''    End Sub

        ''' <summary>
        ''' Fehlende Werte werden aus der INI bezogen.
        ''' (INI.Editor_Message...)
        ''' </summary>
        ''' <param name="gfx"></param>
        ''' <param name="msg"></param>
        ''' <param name="rect"></param>
        Public Sub WriteMessageCentered(
            gfx As Graphics,
            msg As String,
            rect As Rectangle)
            WriteMessageCentered(gfx, msg, INI.Editor_MessageFont, INI.Editor_MessageAlpha, INI.Editor_MessageGray, rect)
        End Sub
        ''' <summary>
        ''' Schriftfarbe ist Schwarz, alpha und gray beziehen sich auf den Untergrund
        ''' </summary>
        ''' <param name="gfx"></param>
        ''' <param name="msg"></param>
        ''' <param name="fnt"></param>
        ''' <param name="alpha"></param>
        ''' <param name="gray"></param>
        ''' <param name="rect"></param>
        Public Sub WriteMessageCentered(
            gfx As Graphics,
            msg As String,
            fnt As Font,
            alpha As Integer,
            gray As Integer,
            rect As Rectangle)

            If gfx Is Nothing Then Throw New ArgumentNullException(NameOf(gfx))
            If fnt Is Nothing Then Throw New ArgumentNullException(NameOf(fnt))
            If msg Is Nothing Then msg = String.Empty

            If alpha < 0 OrElse alpha > 255 Then Throw New ArgumentOutOfRangeException(NameOf(alpha))
            If gray < 0 OrElse gray > 255 Then Throw New ArgumentOutOfRangeException(NameOf(gray))

            Dim backgroundColor As Color = Color.FromArgb(alpha, gray, gray, gray)
            Dim fontColor As Color = Color.FromArgb(255, Color.Black)

            ' Hintergrund
            Using bgBrush As New SolidBrush(backgroundColor)
                gfx.FillRectangle(bgBrush, rect)
            End Using

            ' Zentrierformat
            Using sf As New StringFormat()
                sf.Alignment = StringAlignment.Center
                sf.LineAlignment = StringAlignment.Center
                sf.FormatFlags = StringFormatFlags.NoWrap

                Using textBrush As New SolidBrush(fontColor)
                    gfx.TextRenderingHint = Drawing.Text.TextRenderingHint.ClearTypeGridFit
                    gfx.DrawString(msg, fnt, textBrush, rect, sf)
                End Using
            End Using

        End Sub

        Public Sub WriteMessageCentered(
            gfx As Graphics,
            msg As String,
            fnt As Font,
            fntColor As Color,
            bgColor As Color,
            rect As Rectangle)

            If gfx Is Nothing Then Throw New ArgumentNullException(NameOf(gfx))
            If fnt Is Nothing Then Throw New ArgumentNullException(NameOf(fnt))
            If msg Is Nothing Then msg = String.Empty

            ' Hintergrund
            Using bgBrush As New SolidBrush(bgColor)
                gfx.FillRectangle(bgBrush, rect)
            End Using

            ' Zentrierformat
            Using sf As New StringFormat()
                sf.Alignment = StringAlignment.Center
                sf.LineAlignment = StringAlignment.Center
                sf.FormatFlags = StringFormatFlags.NoWrap

                Using textBrush As New SolidBrush(fntColor)
                    gfx.TextRenderingHint = Drawing.Text.TextRenderingHint.ClearTypeGridFit
                    gfx.DrawString(msg, fnt, textBrush, rect, sf)
                End Using
            End Using

        End Sub

#Region "DrawOverlaySymbol"
        '
        Public Function DrawOverlay(ByVal bmpStein As Bitmap, overlay As OverlayType, ByVal copyBitmap As Boolean) As Bitmap

            Select Case overlay

                Case OverlayType.EditorRemovable
                    Return DrawOverlay_EditorRemovable(bmpStein, copyBitmap)

                Case OverlayType.RahmenSteinMouseOver
                    Return DrawOverlay_RahmenAsymmetrisch(bmpStein, INI.Rendering_MouseOverSteinRahmenColor, copyBitmap)

                Case OverlayType.RahmenSteinSelected
                    Return DrawOverlay_RahmenAsymmetrisch(bmpStein, INI.Rendering_SelectedSteinRahmenColor, copyBitmap)

                Case OverlayType.RahmenSteinPlaceable
                    Return DrawOverlay_RahmenAsymmetrisch(bmpStein, INI.Rendering_PlaceableSteinRahmenColor, copyBitmap)

            End Select

            Return Nothing

        End Function


        ''' <summary>
        ''' Erstellt wahlweise eine Kopie der Stein-Bitmap und zeichnet darauf die
        ''' Editor-Markierung "entnehmbar", oder zeichnet direkt in die übergebene Bitmap.
        ''' </summary>
        ''' <param name="bmpStein">
        ''' Bitmap des Steins.
        ''' </param>
        ''' <param name="copyBitmap">
        ''' True  = Es wird eine neue Bitmap erzeugt und zurückgegeben.
        '''         Diese Rückgabe muß vom Aufrufer disposed werden.
        ''' False = Es wird direkt auf bmpStein gezeichnet und dieselbe Instanz zurückgegeben.
        '''         Keine zusätzliche Dispose-Pflicht für die Rückgabe.
        ''' </param>
        ''' <returns>
        ''' Entweder eine neue Bitmap oder dieselbe übergebene Bitmap.
        ''' </returns>
        Private Function DrawOverlay_EditorRemovable(ByVal bmpStein As Bitmap,
                                             ByVal copyBitmap As Boolean) As Bitmap

            If bmpStein Is Nothing Then
                Return Nothing
            End If

            '--------------------------------------------------
            ' Faktoren zum einfachen Nachjustieren
            '--------------------------------------------------
            Const SYMBOL_TEXT As String = "✓"
            'Const SYMBOL_TEXT As String = "●"
            'Const SYMBOL_TEXT As String = "↑"
            'Const SYMBOL_TEXT As String = "▲"

            Const FONT_NAME As String = "Segoe UI Symbol"

            Const SIZE_FACTOR As Single = 0.4F
            Const OFFSET_X_FACTOR As Single = 0.08F
            Const OFFSET_Y_FACTOR As Single = 0.02F

            Const SYMBOL_ALPHA As Integer = 128
            Const SYMBOL_R As Integer = 0
            Const SYMBOL_G As Integer = 160
            Const SYMBOL_B As Integer = 0

            '--------------------------------------------------
            ' Ziel-Bitmap festlegen
            '--------------------------------------------------
            Dim bmpOut As Bitmap

            If copyBitmap Then
                bmpOut = New Bitmap(bmpStein)
            Else
                bmpOut = bmpStein
            End If

            Dim minEdge As Integer = Math.Min(bmpOut.Width, bmpOut.Height)

            Dim fntSize As Single = CSng(minEdge) * SIZE_FACTOR
            If fntSize < 6.0F Then
                fntSize = 6.0F
            End If

            Dim x As Single = CSng(bmpOut.Width) * OFFSET_X_FACTOR
            Dim y As Single = CSng(bmpOut.Height) * OFFSET_Y_FACTOR

            Using gfx As Graphics = Graphics.FromImage(bmpOut),
          br As New SolidBrush(Color.FromArgb(SYMBOL_ALPHA, SYMBOL_R, SYMBOL_G, SYMBOL_B)),
          fnt As New Font(FONT_NAME, fntSize, FontStyle.Bold, GraphicsUnit.Pixel)

                gfx.SmoothingMode = SmoothingMode.AntiAlias
                gfx.TextRenderingHint = TextRenderingHint.AntiAliasGridFit

                gfx.DrawString(SYMBOL_TEXT, fnt, br, x, y)

            End Using

            Return bmpOut

        End Function

        '
        ''' <summary>
        ''' Erstellt wahlweise eine Kopie der Stein-Bitmap und zeichnet darauf
        ''' einen asymmetrischen, leicht gerundeten Overlay-Rahmen, oder zeichnet
        ''' direkt in die übergebene Bitmap.
        ''' Rechts und unten bleibt der 3D-Bereich ausgespart.
        ''' Alle Maße werden relativ aus Breite/Höhe der Bitmap berechnet.
        ''' </summary>
        ''' <param name="bmpStein">
        ''' Bitmap des Steins.
        ''' </param>
        ''' <param name="overlayColor">
        ''' Farbe des Overlay-Rahmens.
        ''' </param>
        ''' <param name="copyBitmap">
        ''' True  = Es wird eine neue Bitmap erzeugt und zurückgegeben.
        '''         Diese Rückgabe muß vom Aufrufer disposed werden.
        ''' False = Es wird direkt auf bmpStein gezeichnet und dieselbe Instanz zurückgegeben.
        '''         Keine zusätzliche Dispose-Pflicht für die Rückgabe.
        ''' </param>
        ''' <returns>
        ''' Entweder eine neue Bitmap oder dieselbe übergebene Bitmap.
        ''' </returns>
        Private Function DrawOverlay_RahmenAsymmetrisch(ByVal bmpStein As Bitmap,
                                                ByVal overlayColor As Color,
                                                ByVal copyBitmap As Boolean) As Bitmap

            If bmpStein Is Nothing Then
                Return Nothing
            End If

            Dim bmpOut As Bitmap

            If copyBitmap Then
                bmpOut = New Bitmap(bmpStein)
            Else
                bmpOut = bmpStein
            End If

            If bmpOut.Width < 4 OrElse bmpOut.Height < 4 Then
                Return bmpOut
            End If

            '------------------------------------------------------------
            ' Faktoren für die Geometrie
            '------------------------------------------------------------
            Const INSET_LEFT_FACTOR As Single = 0.035F
            Const INSET_TOP_FACTOR As Single = 0.03F
            Const INSET_RIGHT_FACTOR As Single = 0.165F
            Const INSET_BOTTOM_FACTOR As Single = 0.12F

            Const RADIUS_FACTOR As Single = 0.05F
            Const PEN_WIDTH_FACTOR As Single = 0.04F

            Dim bmpW As Integer = bmpOut.Width
            Dim bmpH As Integer = bmpOut.Height
            Dim minSide As Integer = Math.Min(bmpW, bmpH)

            Dim insetLeft As Single = CSng(bmpW) * INSET_LEFT_FACTOR
            Dim insetTop As Single = CSng(bmpH) * INSET_TOP_FACTOR
            Dim insetRight As Single = CSng(bmpW) * INSET_RIGHT_FACTOR
            Dim insetBottom As Single = CSng(bmpH) * INSET_BOTTOM_FACTOR

            Dim radius As Single = CSng(minSide) * RADIUS_FACTOR
            Dim penWidth As Single = CSng(minSide) * PEN_WIDTH_FACTOR

            If radius < 1.5F Then
                radius = 1.5F
            End If

            If penWidth < 1.0F Then
                penWidth = 1.0F
            End If

            ' Kontur-Rechteck des sichtbaren Steinbereichs
            Dim x0 As Single = insetLeft + penWidth * 0.5F
            Dim y0 As Single = insetTop + penWidth * 0.5F
            Dim x1 As Single = CSng(bmpW) - insetRight - penWidth * 0.5F
            Dim y1 As Single = CSng(bmpH) - insetBottom - penWidth * 0.5F

            If x1 <= x0 + 2.0F Then
                x1 = x0 + 2.0F
            End If

            If y1 <= y0 + 2.0F Then
                y1 = y0 + 2.0F
            End If

            Dim maxRadiusX As Single = (x1 - x0) * 0.5F
            Dim maxRadiusY As Single = (y1 - y0) * 0.5F
            Dim maxRadius As Single = Math.Min(maxRadiusX, maxRadiusY)

            If radius > maxRadius Then
                radius = maxRadius
            End If

            Dim d As Single = radius * 2.0F

            Using gfx As Graphics = Graphics.FromImage(bmpOut),
          pen As New Pen(overlayColor, penWidth)

                gfx.SmoothingMode = SmoothingMode.AntiAlias
                gfx.PixelOffsetMode = PixelOffsetMode.HighQuality

                pen.LineJoin = LineJoin.Round
                pen.StartCap = LineCap.Round
                pen.EndCap = LineCap.Round

                Using gp As New GraphicsPath()

                    gp.StartFigure()

                    ' Obere Kante
                    gp.AddLine(x0 + radius, y0, x1 - radius, y0)

                    ' Ecke rechts oben
                    gp.AddArc(x1 - d, y0, d, d, 270.0F, 90.0F)

                    ' Rechte Kante
                    gp.AddLine(x1, y0 + radius, x1, y1 - radius)

                    ' Ecke rechts unten
                    gp.AddArc(x1 - d, y1 - d, d, d, 0.0F, 90.0F)

                    ' Untere Kante
                    gp.AddLine(x1 - radius, y1, x0 + radius, y1)

                    ' Ecke links unten
                    gp.AddArc(x0, y1 - d, d, d, 90.0F, 90.0F)

                    ' Linke Kante
                    gp.AddLine(x0, y1 - radius, x0, y0 + radius)

                    ' Ecke links oben
                    gp.AddArc(x0, y0, d, d, 180.0F, 90.0F)

                    gp.CloseFigure()

                    gfx.DrawPath(pen, gp)

                End Using

            End Using

            Return bmpOut

        End Function
    End Module
#End Region

End Namespace