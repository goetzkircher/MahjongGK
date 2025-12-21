Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On
Imports System.Drawing.Drawing2D

Public Module SteinOverlays

    '------------------------------------------------------------
    ' Varianten für Overlays
    '------------------------------------------------------------
    <Flags>
    Public Enum OverlayStyle
        None = 0
        [Error] = 1
        Warnung = 2
        Verbot = 4
        Blockiert = 8
        Info = 16
        Overlay = 32

        Transparent = 1024   ' Zusatz-Flag für Transparenz
    End Enum

    ' Transparenz-Regler (0..255), nur relevant wenn Transparent-Flag gesetzt ist
    Private Const OVERLAY_ALPHA As Integer = 160

    ' Hilfsfunktion: skaliert einen Basis-Alpha (0..255) mit OVERLAY_ALPHA
    Private Function MixAlpha(baseA As Integer, transparent As Boolean) As Integer
        If Not transparent Then Return baseA
        Dim a As Integer = CInt(baseA * OVERLAY_ALPHA / 255.0R)
        If a < 0 Then a = 0
        If a > 255 Then a = 255
        Return a
    End Function

    '------------------------------------------------------------
    ' Ein Sub für alle Varianten
    '------------------------------------------------------------
    Public Sub InsertOverlayGrafik(bmp As Bitmap, style As OverlayStyle)
        Using gfx As Graphics = Graphics.FromImage(bmp)
            MjGDI.GfxConfigureHighQuality(gfx)

            Dim width As Single = bmp.Width
            Dim height As Single = bmp.Height

            ' Maße aus deiner Variation
            Dim d As Single = 0.65F * Math.Min(width, height)
            Dim centerX As Single = (width / 2.0F) * 0.9F
            Dim centerY As Single = (height / 2.0F) * 0.9F
            Dim x As Single = centerX - d / 2.0F
            Dim y As Single = centerY - d / 2.0F
            Dim rect As New RectangleF(x, y, d, d)
            Dim offsetTop As Integer = 0

            ' Transparent-Flag auswerten
            Dim transparent As Boolean = (style And OverlayStyle.Transparent) = OverlayStyle.Transparent

            ' Farben & Symbole je nach Variante
            Dim topColor As Color
            Dim bottomColor As Color
            Dim symbol As String
            Dim textColor As Color
            'Transparentwert für "nicht transparent"
            Const noTras As Integer = 255

            Select Case style And Not OverlayStyle.Transparent
                Case OverlayStyle.Error
                    topColor = Color.FromArgb(MixAlpha(noTras, transparent), 255, 235, 150)
                    bottomColor = Color.FromArgb(MixAlpha(noTras, transparent), 194, 144, 20)
                    symbol = "✖"
                    textColor = Color.Black

                Case OverlayStyle.Warnung
                    topColor = Color.FromArgb(MixAlpha(noTras, transparent), 255, 245, 170)
                    bottomColor = Color.FromArgb(MixAlpha(noTras, transparent), 240, 200, 20)
                    symbol = "⚠"
                    textColor = Color.Black
                    offsetTop = -CInt(d * 0.1)

                Case OverlayStyle.Verbot
                    topColor = Color.FromArgb(MixAlpha(noTras, transparent), 255, 100, 100)
                    bottomColor = Color.FromArgb(MixAlpha(noTras, transparent), 180, 30, 30)
                    symbol = "🚫"
                    textColor = Color.White

                Case OverlayStyle.Blockiert
                    topColor = Color.FromArgb(MixAlpha(noTras, transparent), 180, 180, 180)
                    bottomColor = Color.FromArgb(MixAlpha(noTras, transparent), 90, 90, 90)
                    symbol = "🔒"
                    textColor = Color.White

                Case OverlayStyle.Info
                    topColor = Color.FromArgb(MixAlpha(noTras, transparent), 170, 210, 255)
                    bottomColor = Color.FromArgb(MixAlpha(noTras, transparent), 50, 90, 180)
                    symbol = "i"
                    textColor = Color.White

                Case OverlayStyle.Overlay
                    topColor = Color.FromArgb(MixAlpha(noTras, transparent), 255, 255, 255)
                    bottomColor = Color.FromArgb(MixAlpha(noTras, transparent), 200, 200, 200)
                    symbol = ""
                    textColor = Color.Black

                Case Else
                    Exit Sub
            End Select

            ' 1) Füllung (Verlauf)
            Using lg As New LinearGradientBrush(rect, topColor, bottomColor, 90.0F)
                Dim blend As New Blend() With {
                    .Positions = {0.0F, 0.35F, 1.0F},
                    .Factors = {1.0F, 0.9F, 0.6F}
                }
                lg.Blend = blend
                gfx.FillEllipse(lg, rect)
            End Using

            ' 2) Konturen
            Using outerPen As New Pen(Color.FromArgb(MixAlpha(180, transparent), 0, 0, 0), Math.Max(1.0F, d * 0.03F))
                gfx.DrawEllipse(outerPen, rect)
            End Using

            ' 3) Glanz
            Dim glossRect As New RectangleF(rect.X + d * 0.12F, rect.Y + d * 0.1F, d * 0.76F, d * 0.5F)
            Using path As New GraphicsPath()
                path.AddEllipse(glossRect)
                Using pgb As New PathGradientBrush(path)
                    pgb.CenterPoint = New PointF(glossRect.X + glossRect.Width * 0.35F, glossRect.Y + glossRect.Height * 0.35F)
                    pgb.CenterColor = Color.FromArgb(MixAlpha(110, transparent), 255, 255, 255)
                    pgb.SurroundColors = {Color.FromArgb(MixAlpha(0, transparent), 255, 255, 255)}
                    gfx.FillPath(pgb, path)
                End Using
            End Using

            ' 4) Symbol zeichnen
            If symbol <> "" Then
                Dim sf As New StringFormat() With {
                    .Alignment = StringAlignment.Center,
                    .LineAlignment = StringAlignment.Center
                }
                Dim textRect As New RectangleF(x, y + offsetTop, d, d)
                Using fnt As New Font("Segoe UI Symbol", bmp.Height * 0.4F, FontStyle.Bold, GraphicsUnit.Pixel)
                    ' Schatten
                    Using shadowBrush As New SolidBrush(Color.FromArgb(MixAlpha(140, transparent), 0, 0, 0))
                        gfx.DrawString(symbol, fnt, shadowBrush,
                                       New RectangleF(textRect.X + 1, textRect.Y + 1, textRect.Width, textRect.Height),
                                       sf)
                    End Using
                    ' Hauptsymbol
                    Using textBrush As New SolidBrush(Color.FromArgb(MixAlpha(255, transparent), textColor))
                        gfx.DrawString(symbol, fnt, textBrush, textRect, sf)
                    End Using
                End Using
            End If

        End Using
    End Sub


    '#####################################################################

    Public Sub InsertGrafik(bitmap As Bitmap, text As String)


        Dim fnt As New Font("Tahoma", 30.0F * bitmap.Width / 250, FontStyle.Bold)

        Using gfx As Graphics = Graphics.FromImage(bitmap)

            MjGDI.GfxConfigureHighQuality(gfx) ' Smoothing/Interpolation/TextRendering etc.

            ' Maße
            Dim d As Single = 0.65F * Math.Min(bitmap.Width, bitmap.Height)
            Dim centerX As Single = bitmap.Width / 2.0F * 0.85F
            Dim centerY As Single = bitmap.Height / 2.0F * 0.91F
            Dim x As Single = centerX - d / 2.0F
            Dim y As Single = centerY - d / 2.0F
            Dim rect As New RectangleF(x, y, d, d)

            ' ------------------------------------------------------------
            ' 1) Weicher Drop-Shadow (leicht nach rechts/unten versetzt)
            ' ------------------------------------------------------------
            Dim shadowOffset As Single = Math.Max(2.0F, d * 0.04F)
            Dim shadowRect As New RectangleF(rect.X + shadowOffset, rect.Y + shadowOffset, rect.Width, rect.Height)

            ' Mehrfaches, leicht größeres Füllen mit abnehmender Alpha simuliert Blur
            For i As Integer = 0 To 3
                Dim grow As Single = i * (d * 0.02F)
                Dim r As New RectangleF(shadowRect.X - grow / 2, shadowRect.Y - grow / 2, shadowRect.Width + grow, shadowRect.Height + grow)
                Using sb As New SolidBrush(Color.FromArgb(40 - i * 8, 0, 0, 0))
                    gfx.FillEllipse(sb, r)
                End Using
            Next

            ' ------------------------------------------------------------
            ' 2) Füllung mit sanftem Verlauf (oben heller, unten satter)
            ' ------------------------------------------------------------
            Dim topColor As Color = Color.FromArgb(255, 255, 230, 128)  ' helles Gold
            Dim bottomColor As Color = Color.FromArgb(255, 184, 134, 11) ' satter Goldton (Goldenrod-nah)

            Using lg As New Drawing2D.LinearGradientBrush(rect, topColor, bottomColor, 90.0F)
                ' Kleiner Trick: Fokus nach oben ziehen (Blend)
                Dim blend As New Drawing2D.Blend()
                blend.Positions = {0.0F, 0.35F, 1.0F}
                blend.Factors = {1.0F, 0.9F, 0.6F}
                lg.Blend = blend
                gfx.FillEllipse(lg, rect)
            End Using

            ' ------------------------------------------------------------
            ' 3) Konturen: dunkler Außenring + zarter Innen-Glanzring
            ' ------------------------------------------------------------
            Using outerPen As New Pen(Color.FromArgb(200, 120, 85, 10), Math.Max(1.0F, d * 0.03F))
                gfx.DrawEllipse(outerPen, rect)
            End Using

            ' innerer, halbtransparenter weißer Ring für „Bevel“-Look
            Dim inset As Single = d * 0.04F
            Dim innerRect As New RectangleF(rect.X + inset, rect.Y + inset, rect.Width - 2 * inset, rect.Height - 2 * inset)
            Using innerPen As New Pen(Color.FromArgb(100, 255, 255, 255), Math.Max(1.0F, d * 0.02F))
                gfx.DrawEllipse(innerPen, innerRect)
            End Using

            ' ------------------------------------------------------------
            ' 4) Glanz-Reflex oben links (weiche, halbmondförmige Highlight-Fläche)
            ' ------------------------------------------------------------
            Dim glossRect As New RectangleF(rect.X + d * 0.12F, rect.Y + d * 0.1F, d * 0.76F, d * 0.5F)
            Using path As New Drawing2D.GraphicsPath()
                path.AddEllipse(glossRect)
                Using pgb As New Drawing2D.PathGradientBrush(path)
                    pgb.CenterPoint = New PointF(glossRect.X + glossRect.Width * 0.35F, glossRect.Y + glossRect.Height * 0.35F)
                    pgb.CenterColor = Color.FromArgb(110, 255, 255, 255)
                    pgb.SurroundColors = {Color.FromArgb(0, 255, 255, 255)}
                    gfx.FillPath(pgb, path)
                End Using
            End Using

            ' ------------------------------------------------------------
            ' 5) Text mit kleinem Schlagschatten + sauber zentriert
            ' ------------------------------------------------------------
            Dim sf As New StringFormat() With {
                .Alignment = StringAlignment.Center,
                .LineAlignment = StringAlignment.Center
            }

            ' Textschatten (minimale Verschiebung, geringe Deckkraft)
            Dim textRect As New RectangleF(x, y, d, d)
            Using shadowBrush As New SolidBrush(Color.FromArgb(140, 0, 0, 0))
                Dim off As Single = Math.Max(1.0F, d * 0.015F)
                Dim shadowRectF As New RectangleF(textRect.X + off, textRect.Y + off, textRect.Width, textRect.Height)
                gfx.DrawString(text, fnt, shadowBrush, shadowRectF, sf)
            End Using

            ' Haupttext (je nach Schrift besser Schwarz oder sehr dunkles Braun)
            Using textBrush As New SolidBrush(Color.Black)
                gfx.DrawString(text, fnt, textBrush, textRect, sf)
            End Using

        End Using

        fnt.Dispose()

    End Sub

End Module

