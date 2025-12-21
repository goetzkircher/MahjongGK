Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On
Imports System.Drawing.Drawing2D
Imports System.Drawing.Imaging
Imports System.Drawing.Text

Namespace Spielfeld

    Public Module RenderingStartScreen

        Public Sub PaintStartScreen(gfxStartcreeen As Graphics, rectOutput As Rectangle)

            If rectOutput.Width < 8 OrElse rectOutput.Height < 8 Then Exit Sub

            If Not IsNothing(SFD.StartscreenBackgroundImage) AndAlso SFD.StartscreenBackgroundImage.Size = rectOutput.Size Then
                gfxStartcreeen.DrawImageUnscaledAndClipped(SFD.StartscreenBackgroundImage, rectOutput)
                Exit Sub
            End If

            SFD.StartscreenBackgroundImage = New Bitmap(rectOutput.Width, rectOutput.Height, PixelFormat.Format32bppArgb)
            Dim gfx As Graphics = Graphics.FromImage(SFD.StartscreenBackgroundImage)

            ' 1) Hintergrund
            If SFD.StartscreenBackgroundImageCache.HasBitmap Then
                gfx.DrawImageUnscaledAndClipped(SFD.StartscreenBackgroundImageCache.GetBitmap(rectOutput.Size), rectOutput)
            Else
                gfx.Clear(INI.Rendering_BackgroundColor)
            End If

            ' 2) Qualität
            gfx.SmoothingMode = SmoothingMode.AntiAlias
            gfx.InterpolationMode = InterpolationMode.HighQualityBicubic
            gfx.PixelOffsetMode = PixelOffsetMode.HighQuality
            gfx.TextRenderingHint = TextRenderingHint.ClearTypeGridFit
            gfx.CompositingQuality = CompositingQuality.HighQuality

            ' 3) Farbschema (Blau) – linearer Verlauf wird über Text-Bounds gespannt
            Dim bg As Color = GetBackgroundPreviewColor()
            Dim isDark As Boolean = PerceivedBrightness(bg) < 130.0


            Dim topLGBrushColor As Color
            Dim bottomLGBrushColor As Color
            Dim outlineColor As Color
            Dim subTextColor As Color
            Dim subTextBgColor As Color

            If INI.Global_DarkMode Then
                topLGBrushColor = Color.FromArgb(128, 0, 0, 255) 'Farbverlauf in den Buchstaben
                bottomLGBrushColor = Color.FromArgb(255, 0, 0, 255)
                outlineColor = Color.FromArgb(50, 50, 60)
                subTextColor = Color.FromArgb(45, 55, 70)
                subTextBgColor = Color.FromArgb(192, 99, 99, 99) 'die Basisfarbe des Hintergrunds   
            Else
                topLGBrushColor = Color.FromArgb(64, 0, 188, 255)
                bottomLGBrushColor = Color.FromArgb(255, 0, 64, 255)
                outlineColor = Color.FromArgb(50, 50, 60)
                subTextColor = Color.FromArgb(35, 45, 65)
                subTextBgColor = Color.FromArgb(192, 245, 245, 245) 'die Basisfarbe des Hintergrunds
            End If

            ' 4) Titel
            Dim titleText As String = "MahjongGK"
            Dim titleHeightPx As Single = rectOutput.Height * 0.25F

            Dim layoutRect As RectangleF = rectOutput
            layoutRect.Inflate(-rectOutput.Width * 0.02F, -rectOutput.Height * 0.25F)
            layoutRect.Y -= rectOutput.Height * 0.12F

            Dim titleBounds As RectangleF = DrawGlossTitleLinear(
                gfx, titleText, layoutRect, titleHeightPx,
                topLGBrushColor, bottomLGBrushColor, outlineColor
            )

            ' 5) Untertitel – rechtsbündig direkt unter dem Titel ausgerichtet
            Dim subText As String = "Open Source by Götz Kircher"
            Dim ff As FontFamily = FontFamily.GenericSansSerif
            Dim subHeightPx As Single = Math.Max(10.0F, titleHeightPx * 0.15F)

            Using subFont As Font = FitFontToHeight(gfx, subText, ff, FontStyle.Regular, subHeightPx)
                Dim gap As Single = Math.Max(2.0F, titleHeightPx * 0.08F)
                'Dim leftEdge As Single = rectOutput.Left + rectOutput.Width * 0.06F
                Dim rightEdge As Single = titleBounds.Right
                Dim size As SizeF = gfx.MeasureString(subText, subFont)
                size.Width *= 1.02F ' etwas mehr Platz
                Dim subRect As New RectangleF(
                    rightEdge - size.Width,
                    titleBounds.Bottom + gap,
                    size.Width,
                    subFont.GetHeight(gfx) * 1.35F
                )

                Using sb As New SolidBrush(subTextBgColor)
                    gfx.FillRectangle(sb, subRect)
                End Using

                Using sf As New StringFormat(StringFormatFlags.NoClip)
                    sf.Alignment = StringAlignment.Center
                    sf.LineAlignment = StringAlignment.Center
                    sf.FormatFlags = StringFormatFlags.NoWrap
                    Using br As New SolidBrush(subTextColor)
                        gfx.DrawString(subText, subFont, br, subRect, sf)
                    End Using
                End Using
            End Using

            gfxStartcreeen.DrawImageUnscaledAndClipped(SFD.StartscreenBackgroundImage, rectOutput)

        End Sub

        ' ─────────────────────────────────────────────────────────────────────────
        ' Titel mit KONSTANTEM linearen Verlauf + 8-stufigem Schatten + Highlight-Band
        ' ─────────────────────────────────────────────────────────────────────────
        Private Function DrawGlossTitleLinear(gfx As Graphics,
                                              text As String,
                                              layoutRect As RectangleF,
                                              desiredHeightPx As Single,
                                              topLGBrushColor As Color,
                                              bottomLGBrushColor As Color,
                                              outlineColor As Color) As RectangleF

            Dim ff As FontFamily = FontFamily.GenericSansSerif
            ff = New FontFamily("Segoe UI") ' besser als GenericSansSerif

            Using font As Font = FitFontToHeight(gfx, text, ff, FontStyle.Bold, desiredHeightPx)
                Using sf As New StringFormat(StringFormatFlags.NoClip)
                    sf.Alignment = StringAlignment.Center
                    sf.LineAlignment = StringAlignment.Center

                    ' Zielbox mittig in layoutRect
                    Dim measured As SizeF = gfx.MeasureString(text, font, New SizeF(layoutRect.Width, layoutRect.Height), sf)
                    Dim w As Single = Math.Min(layoutRect.Width, measured.Width)
                    Dim h As Single = Math.Min(layoutRect.Height, measured.Height)
                    Dim x As Single = layoutRect.Left + (layoutRect.Width - w) / 2.0F
                    Dim y As Single = layoutRect.Top + (layoutRect.Height - h) / 2.0F
                    Dim box As New RectangleF(x, y, w, h)

                    Using gp As New GraphicsPath()
                        gp.AddString(text, ff, CInt(FontStyle.Bold),
                                     font.SizeInPoints * gfx.DpiY / 72.0F,
                                     box, sf)

                        Dim bounds As RectangleF = gp.GetBounds()

                        ' 1) feiner Schlagschatten (kleine Steps, fallende Alpha)
                        DrawSoftShadow(gfx, gp, desiredHeightPx)

                        ' 2) Körper EINFARBIG füllen – keine „Horizont“-Risiken
                        Dim coreBlue As Color = Color.FromArgb(48, 108, 200) ' ruhiges Mittelblau
                        Using brBody As New SolidBrush(coreBlue)
                            gfx.FillPath(brBody, gp)
                        End Using

                        ' 3) Monotone Schattierung von oben→unten (kein mittiger Peak!)
                        '    Clip auf Textpfad, dann transparenter Schwarzverlauf über die GESAMTEN Bounds
                        bounds = gp.GetBounds()
                        Dim oldClip As Region = gfx.Clip
                        Using clip As New Region(gp)
                            gfx.SetClip(clip, CombineMode.Intersect)

                            ' Verlauf etwas über die Bounds hinaus spannen, damit die Ränder weich sind
                            Dim startY As Single = bounds.Top - bounds.Height * 0.1F
                            Dim endY As Single = bounds.Bottom + bounds.Height * 0.1F

                            Using lgShade As New LinearGradientBrush(
                                New PointF(bounds.Left, startY),
                                New PointF(bounds.Left, endY),
                                topLGBrushColor,
                                bottomLGBrushColor
                            )
                                lgShade.GammaCorrection = True
                                ' gleichmäßig fallend – KEINE Blend-Spitzen, KEIN Mittel-Stop!
                                gfx.FillRectangle(lgShade, New RectangleF(bounds.Left, startY, bounds.Width, endY - startY))
                            End Using

                            ' 4) Zarte Top-Highlight-Kante entlang der oberen Kante (1 px)
                            Using penHi As New Pen(Color.FromArgb(110, 255, 255, 255), Math.Max(1.0F, desiredHeightPx * 0.016F))
                                penHi.LineJoin = LineJoin.Round
                                Using gpHi As GraphicsPath = CType(gp.Clone(), GraphicsPath)
                                    Using mHi As New Matrix()
                                        mHi.Translate(-Math.Max(0.4F, desiredHeightPx * 0.005F),
                              -Math.Max(0.4F, desiredHeightPx * 0.005F))
                                        gpHi.Transform(mHi)
                                    End Using
                                    gfx.DrawPath(penHi, gpHi)
                                End Using
                            End Using

                            gfx.Clip = oldClip
                        End Using
                        oldClip?.Dispose()

                        ' 5) Außenkontur (wie bisher)
                        Using penOutline As New Pen(outlineColor, Math.Max(2.4F, desiredHeightPx * 0.038F))
                            penOutline.LineJoin = LineJoin.Round
                            gfx.DrawPath(penOutline, gp)
                        End Using


                        ' 5) Außenkontur
                        Using penOutline As New Pen(outlineColor, Math.Max(2.4F, desiredHeightPx * 0.038F))
                            penOutline.LineJoin = LineJoin.Round
                            gfx.DrawPath(penOutline, gp)
                        End Using

                        Return bounds
                    End Using
                End Using
            End Using
        End Function

        ' Font auf Zielhöhe bringen (per Binärsuche auf Path-Bounds)
        Private Function FitFontToHeight(gfx As Graphics,
                                         text As String,
                                         ff As FontFamily,
                                         style As FontStyle,
                                         desiredPx As Single) As Font
            Dim minPt As Single = 4.0F
            Dim maxPt As Single = 400.0F
            Dim bestPt As Single = minPt

            For i As Integer = 0 To 12
                Dim mid As Single = (minPt + maxPt) / 2.0F
                Using f As New Font(ff, mid, style, GraphicsUnit.Point)
                    Using gp As New GraphicsPath()
                        gp.AddString(text, ff, CInt(style), f.SizeInPoints * gfx.DpiY / 72.0F, New Point(0, 0), New StringFormat())
                        Dim h As Single = gp.GetBounds().Height
                        If h < desiredPx Then
                            bestPt = mid
                            minPt = mid
                        Else
                            maxPt = mid
                        End If
                    End Using
                End Using
            Next

            Return New Font(ff, bestPt, style, GraphicsUnit.Point)
        End Function

        Private Function PerceivedBrightness(c As Color) As Double
            Dim r As Double = c.R, g As Double = c.G, b As Double = c.B
            Return Math.Sqrt(r * r * 0.241 + g * g * 0.691 + b * b * 0.068)
        End Function

        Private Function GetBackgroundPreviewColor() As Color
            If Not SFD.StartscreenBackgroundImageCache.HasBitmap Then
                Return INI.Rendering_BackgroundColor
            End If
            ' Heuristik (Mittel der beiden Vorgaben)
            Dim darkBase As Color = Color.FromArgb(99, 99, 99)
            Dim lightBase As Color = Color.FromArgb(245, 245, 245)
            Dim r As Integer = (CInt(darkBase.R) + CInt(lightBase.R)) \ 2
            Dim g As Integer = (CInt(darkBase.G) + CInt(lightBase.G)) \ 2
            Dim b As Integer = (CInt(darkBase.B) + CInt(lightBase.B)) \ 2
            r = Math.Min(255, Math.Max(0, r))
            g = Math.Min(255, Math.Max(0, g))
            b = Math.Min(255, Math.Max(0, b))
            Return Color.FromArgb(255, r, g, b)
        End Function
        ' Sehr feiner, mehrstufiger Soft-Shadow für Textpfade
        Private Sub DrawSoftShadow(gfx As Graphics, textPath As GraphicsPath, desiredHeightPx As Single)

            ' Stufenzahl dynamisch zur Schriftgröße: min 24, max 48
            Dim steps As Integer = Math.Min(48, Math.Max(24, CInt(desiredHeightPx * 0.8F)))

            ' Grund-Offset (Schattenlänge) klein halten, skaliert mit Größe
            Dim baseOff As Single = Math.Max(0.8F, desiredHeightPx * 0.022F) * 2

            ' Exponentiell fallende Deckkraft; Start-Alpha abhängig von Größe
            Dim alphaStart As Integer = Math.Min(120, Math.Max(60, CInt(desiredHeightPx * 1.4F))) \ 2 ' \ 2 weglassen verstärkt den Schatten

            For i As Integer = 1 To steps
                Dim t As Single = CSng(i) / steps

                ' sehr kleine, nahezu lineare Offsets (subpixel → glatter)
                Dim off As Single = baseOff * (0.5F + 3.6F * t)

                ' weicher Abfall der Alpha (Gamma-Kurve)
                Dim a As Integer = CInt(alphaStart * Math.Pow(1.0 - t, 1.2))
                If a <= 0 Then Exit For

                Using shadowPath As GraphicsPath = CType(textPath.Clone(), GraphicsPath)
                    Using m As New Matrix()
                        m.Translate(off, off) ' unten-rechts
                        shadowPath.Transform(m)
                    End Using
                    Using br As New SolidBrush(Color.FromArgb(Math.Min(255, Math.Max(0, a)), 0, 0, 0))
                        gfx.FillPath(br, shadowPath)
                    End Using
                End Using
            Next
        End Sub


    End Module


End Namespace