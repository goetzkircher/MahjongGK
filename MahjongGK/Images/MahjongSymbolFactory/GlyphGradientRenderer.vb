Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On
Imports System.Drawing.Drawing2D
Imports System.Drawing.Text

Public Module GlyphGradientRenderer
    ''    Dim bmp1 = RenderGlyphLinearGradient("★", "Segoe UI Symbol", 200.0F, 256, 256, Color.OrangeRed, Color.Gold, 90.0F, Color.Black, 2.0F)
    ''    Dim bmp2 = RenderGlyphRadialGradient("❤", "Segoe UI Symbol", 200.0F, 256, 256, Color.White, Color.Crimson, Color.DarkRed, 2.0F)
    ''Tipps:
    'Für Emoji/Symbole nutze Fonts wie Segoe UI Symbol, Segoe UI Emoji.
    'Wenn eine Glyphe in der gewählten Schrift fehlt, wechsle auf
    'FontFamily.GenericSansSerif oder führe eine eigene Font-Fallback-Liste
    '(z. B. „Segoe UI Emoji“, dann „Segoe UI Symbol“, dann „Segoe UI“).

    'Für klarere Kanten kleine Outline zeichnen (1–2 px, LineJoin.Round)

    'Wann siehst du eingebaute Verläufe „einfach so“?
    'Wenn die Schrift COLR v1 oder SVG enthält und dein Renderer das kann (WPF/DirectWrite, moderne Browser/Skia).
    'In WinForms GDI+: selten/nie → besser selbst malen (Code oben) oder WPF hosten.

    ''' <summary>
    ''' Rendert ein einzelnes Zeichen als Bitmap mit linearem Farbverlauf.
    ''' </summary>
    Public Function RenderGlyphLinearGradient(ch As String,
                                              fontFamilyName As String,
                                              emSizePx As Single,
                                              width As Integer,
                                              height As Integer,
                                              color1 As Color,
                                              color2 As Color,
                                              angleDeg As Single,
                                              Optional outlineColor As Color = Nothing,
                                              Optional outlineWidth As Single = 0.0F) As Bitmap
        If String.IsNullOrEmpty(ch) Then Throw New ArgumentException(NameOf(ch))
        Dim bmp As New Bitmap(width, height, Imaging.PixelFormat.Format32bppArgb)
        Using g As Graphics = Graphics.FromImage(bmp)
            g.SmoothingMode = SmoothingMode.AntiAlias
            g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit
            g.Clear(Color.Transparent)

            Using path As GraphicsPath = BuildGlyphPath(ch, fontFamilyName, emSizePx, New RectangleF(0, 0, width, height))
                ' Verlauf auf die ganze Fläche
                Using lg As New LinearGradientBrush(New Rectangle(0, 0, width, height), color1, color2, angleDeg)
                    g.FillPath(lg, path)
                End Using
                If outlineWidth > 0.0F AndAlso outlineColor.A > 0 Then
                    Using pen As New Pen(outlineColor, outlineWidth)
                        pen.LineJoin = LineJoin.Round
                        g.DrawPath(pen, path)
                    End Using
                End If
            End Using
        End Using
        Return bmp
    End Function

    ''' <summary>
    ''' Rendert ein einzelnes Zeichen als Bitmap mit radialem Verlauf (PathGradientBrush).
    ''' </summary>
    Public Function RenderGlyphRadialGradient(ch As String,
                                              fontFamilyName As String,
                                              emSizePx As Single,
                                              width As Integer,
                                              height As Integer,
                                              centerColor As Color,
                                              surroundColor As Color,
                                              Optional outlineColor As Color = Nothing,
                                              Optional outlineWidth As Single = 0.0F) As Bitmap
        If String.IsNullOrEmpty(ch) Then Throw New ArgumentException(NameOf(ch))
        Dim bmp As New Bitmap(width, height, Imaging.PixelFormat.Format32bppArgb)
        Using g As Graphics = Graphics.FromImage(bmp)
            g.SmoothingMode = SmoothingMode.AntiAlias
            g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit
            g.Clear(Color.Transparent)

            Using path As GraphicsPath = BuildGlyphPath(ch, fontFamilyName, emSizePx, New RectangleF(0, 0, width, height))
                Using pgb As New PathGradientBrush(path)
                    pgb.CenterColor = centerColor
                    pgb.SurroundColors = Enumerable.Repeat(surroundColor, 1).ToArray()
                    ' Optional CenterPoint/FocusScales anpassen:
                    pgb.CenterPoint = New PointF(width / 2.0F, height / 2.0F)
                    g.FillPath(pgb, path)
                End Using
                If outlineWidth > 0.0F AndAlso outlineColor.A > 0 Then
                    Using pen As New Pen(outlineColor, outlineWidth)
                        pen.LineJoin = LineJoin.Round
                        g.DrawPath(pen, path)
                    End Using
                End If
            End Using
        End Using
        Return bmp
    End Function

    ''' <summary>
    ''' Baut den Vektorpfad eines Zeichens im Zielrechteck.
    ''' </summary>
    Private Function BuildGlyphPath(ch As String,
                                    fontFamilyName As String,
                                    emSizePx As Single,
                                    targetRect As RectangleF) As GraphicsPath
        Dim fam As FontFamily
        Try
            fam = New FontFamily(fontFamilyName)
        Catch
            fam = FontFamily.GenericSansSerif
        End Try

        Dim style As FontStyle = FontStyle.Regular
        If Not fam.IsStyleAvailable(style) Then style = FontStyle.Regular

        ' Pfad in Em-Einheiten anlegen
        Dim path As New GraphicsPath()
        path.AddString(ch, fam, CInt(style), emSizePx, New PointF(0, 0), StringFormat.GenericTypographic)

        ' Auf Zielrechteck skalieren und zentrieren
        Dim bounds As RectangleF = path.GetBounds()
        Dim scaleX As Single = targetRect.Width / Math.Max(bounds.Width, 1.0F)
        Dim scaleY As Single = targetRect.Height / Math.Max(bounds.Height, 1.0F)
        Dim scale As Single = Math.Min(scaleX, scaleY)

        Dim m As New Matrix()
        ' 1) an den Ursprung verschieben
        m.Translate(-bounds.Left, -bounds.Top, MatrixOrder.Append)
        ' 2) skalieren
        m.Scale(scale, scale, MatrixOrder.Append)
        ' 3) zentrieren
        Dim newBounds As RectangleF = RectangleF.Empty
        Using tmp As GraphicsPath = CType(path.Clone(), GraphicsPath)
            tmp.Transform(m)
            newBounds = tmp.GetBounds()
        End Using
        Dim dx As Single = targetRect.Left + (targetRect.Width - newBounds.Width) / 2.0F
        Dim dy As Single = targetRect.Top + (targetRect.Height - newBounds.Height) / 2.0F
        m.Translate(dx, dy, MatrixOrder.Append)

        path.Transform(m)
        Return path
    End Function

End Module

