Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On
Imports System.Drawing.Drawing2D
Imports System.Drawing.Text

'------------------------------------------------------------
'  Mahjong-Symbol-Fabrik (chinesisch wirkender Stil)
'------------------------------------------------------------
Namespace MahjongGKSymbolFactory

    '
    ''' <summary>
    ''' Erzeugt alle 42 Symbole (0..41) gemäß deiner SteinIndexEnum-Reihenfolge als Bitmaps.
    ''' </summary>
    Public NotInheritable Class MahjongSymbolFactory
        Private Sub New()
        End Sub

        '
        ''' <summary>
        ''' Erstellt eine Liste aller 42 Symbolbitmaps in Enum-Reihenfolge.
        ''' </summary>
        Public Shared Function GenerateAllSymbols(size As Size) As List(Of Bitmap)
            Dim list As New List(Of Bitmap)([Enum].GetValues(GetType(SteinIndexEnum)).Length)
            For Each si As SteinIndexEnum In [Enum].GetValues(GetType(SteinIndexEnum))
                list.Add(GenerateSymbolChinese(si, size))
            Next
            Return list
        End Function

        '
        ''' <summary>
        ''' Optional: Gibt ein Dictionary (Enum -> Bitmap) zurück.
        ''' </summary>
        Public Shared Function GenerateAllSymbolsMap(size As Size) As Dictionary(Of SteinIndexEnum, Bitmap)
            Dim dict As New Dictionary(Of SteinIndexEnum, Bitmap)()
            For Each si As SteinIndexEnum In [Enum].GetValues(GetType(SteinIndexEnum))
                dict(si) = GenerateSymbolChinese(si, size)
            Next
            Return dict
        End Function

        '
        ''' <summary>
        ''' Erstellt ein Bitmap mit einem chinesisch wirkenden Mahjong-Symbol.
        ''' </summary>
        Public Shared Function GenerateSymbolChinese(idx As SteinIndexEnum, size As Size) As Bitmap
            Dim bmp As New Bitmap(size.Width, size.Height, Imaging.PixelFormat.Format32bppPArgb)
            Using g As Graphics = Graphics.FromImage(bmp)
                g.SmoothingMode = SmoothingMode.AntiAlias
                g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit
                g.Clear(Color.Transparent)

                Select Case idx
                    ' --- Punkte 1..9 (klassische „Dots“) ---
                    Case SteinIndexEnum.Punkt01 To SteinIndexEnum.Punkt09
                        DrawChineseDots(g, CInt(idx - SteinIndexEnum.Punkt01 + 1), size)

                    ' --- Bambus 1..9 ---
                    Case SteinIndexEnum.Bambus1 To SteinIndexEnum.Bambus9
                        DrawChineseBamboo(g, CInt(idx - SteinIndexEnum.Bambus1 + 1), size)

                    ' --- Zeichen („萬“ 1..9) ---
                    Case SteinIndexEnum.Symbol1 To SteinIndexEnum.Symbol9
                        DrawChineseCharacterSeries(g, CInt(idx - SteinIndexEnum.Symbol1 + 1), size)

                    ' --- Drachen ---
                    Case SteinIndexEnum.DracheR
                        DrawChineseGlyph(g, "中", size, Color.Red)
                    Case SteinIndexEnum.DracheG
                        DrawChineseGlyph(g, "發", size, Color.FromArgb(10, 135, 10))
                    Case SteinIndexEnum.DracheW
                        ' Weißer Drache – klassisch leerer/umrandeter Rahmen. Wir zeichnen ein blaues Kästchen.
                        DrawWhiteDragon(g, size, Color.FromArgb(0, 90, 200))

                    ' --- Winde (東南西北) ---
                    Case SteinIndexEnum.WindOst : DrawChineseGlyph(g, "東", size, Color.Black)
                    Case SteinIndexEnum.WindSüd : DrawChineseGlyph(g, "南", size, Color.Black)
                    Case SteinIndexEnum.WindWst : DrawChineseGlyph(g, "西", size, Color.Black)
                    Case SteinIndexEnum.WindNrd : DrawChineseGlyph(g, "北", size, Color.Black)

                    ' --- Blumen (梅 蘭 菊 竹) ---
                    Case SteinIndexEnum.BlütePf : DrawChineseGlyph(g, "梅", size, Color.FromArgb(200, 50, 120))
                    Case SteinIndexEnum.BlüteOr : DrawChineseGlyph(g, "蘭", size, Color.FromArgb(140, 70, 190))
                    Case SteinIndexEnum.BlüteCt : DrawChineseGlyph(g, "菊", size, Color.FromArgb(210, 160, 40))
                    Case SteinIndexEnum.BlüteBa : DrawChineseGlyph(g, "竹", size, Color.FromArgb(40, 150, 80))

                    ' --- Jahreszeiten (春 夏 秋 冬) ---
                    Case SteinIndexEnum.JahrFrl : DrawChineseGlyph(g, "春", size, Color.FromArgb(50, 160, 90))
                    Case SteinIndexEnum.JahrSom : DrawChineseGlyph(g, "夏", size, Color.FromArgb(230, 120, 30))
                    Case SteinIndexEnum.JahrHer : DrawChineseGlyph(g, "秋", size, Color.FromArgb(180, 90, 20))
                    Case SteinIndexEnum.JahrWin : DrawChineseGlyph(g, "冬", size, Color.FromArgb(50, 110, 200))

                    ' --- Error / Fallback ---
                    Case SteinIndexEnum.ErrorSy
                        DrawErrorSymbol(g, size)

                    Case Else
                        DrawFallback(g, size)
                End Select
            End Using
            Return bmp
        End Function

        '======================
        '  Zeichnen: Dots
        '======================

        '
        ''' <summary>
        ''' Klassische Punkte (konzentrische Kreise für #1, mehrfarbig verteilt ab #2).
        ''' </summary>
        Private Shared Sub DrawChineseDots(g As Graphics, number As Integer, size As Size)
            Dim cellW As Single = size.Width / 3.0F
            Dim cellH As Single = size.Height / 3.0F
            Dim positions As New List(Of PointF)
            For row As Integer = 0 To 2
                For col As Integer = 0 To 2
                    positions.Add(New PointF(col * cellW + cellW / 2.0F, row * cellH + cellH / 2.0F))
                Next
            Next

            ' Reihenfolge: „Augen“ füllen von oben links nach unten rechts.
            ' 1-dot: großer Mittelpunkt mit konzentrischen Ringen (rot).
            If number = 1 Then
                DrawConcentricDot(g, New PointF(size.Width / 2.0F, size.Height / 2.0F),
                                  size.Width * 0.46F, Color.Red)
                Return
            End If

            Dim palette() As Color = {Color.Blue, Color.Green, Color.Red}
            Dim r As Single = Math.Min(cellW, cellH) * 0.45F * 0.55F

            For i As Integer = 0 To number - 1
                Dim pos As PointF = positions(i)
                Dim c As Color = palette(i Mod palette.Length)
                DrawRingedDot(g, pos, r, c)
            Next
        End Sub

        Private Shared Sub DrawConcentricDot(g As Graphics, center As PointF, diameter As Single, baseColor As Color)
            Dim outerR As Single = diameter / 2.0F
            Dim rect As New RectangleF(center.X - outerR, center.Y - outerR, diameter, diameter)

            Using br As New SolidBrush(Color.White)
                g.FillEllipse(br, rect)
            End Using
            Using pen As New Pen(Color.Black, Math.Max(2.0F, diameter * 0.03F))
                g.DrawEllipse(pen, rect)
            End Using

            ' farbiger Kern + Ring
            Using br2 As New SolidBrush(baseColor)
                Dim r2 As Single = diameter * 0.55F
                Dim rc2 As New RectangleF(center.X - r2 / 2, center.Y - r2 / 2, r2, r2)
                g.FillEllipse(br2, rc2)
            End Using
            Using pen2 As New Pen(Color.White, Math.Max(2.0F, diameter * 0.08F))
                Dim r3 As Single = diameter * 0.72F
                Dim rc3 As New RectangleF(center.X - r3 / 2, center.Y - r3 / 2, r3, r3)
                g.DrawEllipse(pen2, rc3)
            End Using
        End Sub

        Private Shared Sub DrawRingedDot(g As Graphics, center As PointF, radius As Single, baseColor As Color)
            Dim rect As New RectangleF(center.X - radius, center.Y - radius, radius * 2, radius * 2)
            Using brW As New SolidBrush(Color.White)
                g.FillEllipse(brW, rect)
            End Using
            Using penB As New Pen(Color.Black, Math.Max(1.5F, radius * 0.12F))
                g.DrawEllipse(penB, rect)
            End Using
            ' farbiger innerer Punkt
            Using br As New SolidBrush(baseColor)
                Dim r2 As Single = radius * 0.55F
                Dim rc2 As New RectangleF(center.X - r2, center.Y - r2, r2 * 2, r2 * 2)
                g.FillEllipse(br, rc2)
            End Using
        End Sub

        '======================
        '  Zeichnen: Bamboo
        '======================

        '
        ''' <summary>
        ''' Stilisiertes Bambus-Raster mit Segmentringen, 1..9 Stäbchen.
        ''' </summary>
        Private Shared Sub DrawChineseBamboo(g As Graphics, number As Integer, size As Size)
            ' Layout: max 3 Spalten x 3 Reihen
            Dim cols As Integer = Math.Min(3, Math.Max(1, CInt(Math.Ceiling(number / 3.0))))
            Dim rows As Integer = CInt(Math.Ceiling(number / cols))

            Dim cellW As Single = CSng(size.Width / cols)
            Dim cellH As Single = CSng(size.Height / rows)
            Dim margin As Single = Math.Min(cellW, cellH) * 0.18F

            Dim stickW As Single = Math.Min(cellW, cellH) * 0.3F
            Dim ringH As Single = Math.Min(cellW, cellH) * 0.08F
            Dim segCount As Integer = 3

            For i As Integer = 0 To number - 1
                Dim rIdx As Integer = i \ cols
                Dim cIdx As Integer = i Mod cols

                Dim x As Single = cIdx * cellW + (cellW - stickW) / 2.0F
                Dim y As Single = rIdx * cellH + margin
                Dim h As Single = cellH - 2 * margin

                Using br As New SolidBrush(Color.FromArgb(30, 140, 70))
                    g.FillRectangle(br, x, y, stickW, h)
                End Using
                Using pen As New Pen(Color.Black, Math.Max(1.2F, stickW * 0.08F))
                    g.DrawRectangle(pen, x, y, stickW, h)
                End Using

                ' Segmentringe
                Using penR As New Pen(Color.FromArgb(0, 90, 40), Math.Max(1.0F, stickW * 0.06F))
                    For s As Integer = 1 To segCount - 1
                        Dim yy As Single = y + h * s / segCount
                        g.DrawLine(penR, x, yy, x + stickW, yy)
                    Next
                End Using

                ' Knospe
                Using brK As New SolidBrush(Color.FromArgb(20, 100, 50))
                    Dim kW As Single = stickW * 0.6F
                    Dim kH As Single = ringH * 1.6F
                    g.FillEllipse(brK, x + (stickW - kW) / 2.0F, y - kH * 0.4F, kW, kH)
                End Using
            Next
        End Sub

        '======================
        '  Zeichnen: Characters/Drachen/Winde/… (Glyphs)
        '======================

        '
        ''' <summary>
        ''' „萬“ für Characters + kleine arabische Zahl unten rechts.
        ''' </summary>
        Private Shared Sub DrawChineseCharacterSeries(g As Graphics, num As Integer, size As Size)
            DrawChineseGlyph(g, "萬", size, Color.FromArgb(200, 40, 40))
            Using f As Font = PickCjkFont(CInt(size.Height / 6.0), FontStyle.Bold)
                Dim s As String = num.ToString()
                Dim br As Brush = Brushes.Black
                Dim x As Single = size.Width - g.MeasureString(s, f).Width - 4.0F
                Dim y As Single = size.Height - g.MeasureString(s, f).Height - 2.0F
                g.DrawString(s, f, br, x, y)
            End Using
        End Sub

        '
        ''' <summary>
        ''' Zeichnet ein zentriertes CJK-Glyph.
        ''' </summary>
        Private Shared Sub DrawChineseGlyph(g As Graphics, text As String, size As Size, color As Color)
            Using f As Font = PickCjkFont(CInt(size.Height * 0.58), FontStyle.Bold)
                Dim sf As New StringFormat With {.Alignment = StringAlignment.Center, .LineAlignment = StringAlignment.Center}
                Using br As New SolidBrush(color)
                    g.DrawString(text, f, br, New RectangleF(0, 0, size.Width, size.Height), sf)
                End Using
            End Using
        End Sub

        Private Shared Sub DrawWhiteDragon(g As Graphics, size As Size, frameColor As Color)
            Dim inset As Single = Math.Min(size.Width, size.Height) * 0.18F
            Dim r As New RectangleF(inset, inset, size.Width - 2 * inset, size.Height - 2 * inset)
            Using pen As New Pen(frameColor, Math.Max(4.0F, Math.Min(size.Width, size.Height) * 0.04F))
                g.DrawRectangle(pen, r.X, r.Y, r.Width, r.Height)
            End Using
            ' dezente Innenmarke
            Using pen2 As New Pen(Color.FromArgb(120, frameColor), Math.Max(2.0F, Math.Min(size.Width, size.Height) * 0.02F))
                g.DrawLine(pen2, r.Left, r.Top, r.Right, r.Bottom)
                g.DrawLine(pen2, r.Right, r.Top, r.Left, r.Bottom)
            End Using
        End Sub

        '
        ''' <summary>
        ''' Wählt eine verfügbare CJK-kompatible Schrift (Fallback: Segoe UI Symbol).
        ''' </summary>
        Private Shared Function PickCjkFont(emSize As Integer, style As FontStyle) As Font
            Dim candidates() As String = {"Microsoft YaHei UI", "Microsoft JhengHei UI", "Yu Gothic UI", "SimSun", "PMingLiU", "Segoe UI Symbol"}
            For Each name As String In candidates
                Try
                    Dim f As New Font(name, emSize, style, GraphicsUnit.Pixel)
                    ' simple check: ensure the font can be created
                    Return f
                Catch
                End Try
            Next
            ' letzter Fallback
            Return New Font(FontFamily.GenericSansSerif, emSize, style, GraphicsUnit.Pixel)
        End Function

        '======================
        '  Error / Fallback
        '======================

        Private Shared Sub DrawErrorSymbol(g As Graphics, size As Size)
            Using br As New SolidBrush(Color.FromArgb(220, 40, 40))
                Dim r As New RectangleF(size.Width * 0.18F, size.Height * 0.18F, size.Width * 0.64F, size.Height * 0.64F)
                g.FillEllipse(br, r)
            End Using
            Using pen As New Pen(Color.White, Math.Max(4.0F, size.Width * 0.04F))
                g.DrawLine(pen, size.Width * 0.3F, size.Height * 0.3F, size.Width * 0.7F, size.Height * 0.7F)
                g.DrawLine(pen, size.Width * 0.7F, size.Height * 0.3F, size.Width * 0.3F, size.Height * 0.7F)
            End Using
        End Sub

        Private Shared Sub DrawFallback(g As Graphics, size As Size)
            Using pen As New Pen(Color.Gray, 2.0F)
                g.DrawRectangle(pen, 2, 2, size.Width - 4, size.Height - 4)
            End Using
            Using f As Font = PickCjkFont(CInt(size.Height / 4.0), FontStyle.Bold)
                Dim sf As New StringFormat With {.Alignment = StringAlignment.Center, .LineAlignment = StringAlignment.Center}
                g.DrawString("？", f, Brushes.Gray, New RectangleF(0, 0, size.Width, size.Height), sf)
            End Using
        End Sub

    End Class

End Namespace

