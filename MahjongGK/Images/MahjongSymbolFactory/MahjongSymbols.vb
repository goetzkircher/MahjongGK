Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On
Imports System.Drawing.Imaging
Imports System.Drawing.Text

Namespace MahjongGKSymbolFactory
    Public Module MahjongSymbols

        '
        ''' <summary>
        ''' Erstellt ein Bitmap mit einem chinesisch wirkenden Mahjong-Symbol.
        ''' </summary>
        Public Function GenerateSymbolChinese(idx As SteinIndexEnum, size As Size) As Bitmap
            Dim bmp As New Bitmap(size.Width, size.Height, PixelFormat.Format32bppPArgb)
            Using g As Graphics = Graphics.FromImage(bmp)
                g.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
                g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit
                g.Clear(Color.Transparent)

                Select Case idx
                ' Punkte (Kreise)
                    Case SteinIndexEnum.Punkt01 To SteinIndexEnum.Punkt09
                        DrawChineseDots(g, idx - SteinIndexEnum.Punkt01 + 1, size)

                ' Bambus
                    Case SteinIndexEnum.Bambus1 To SteinIndexEnum.Bambus9
                        DrawChineseBamboo(g, idx - SteinIndexEnum.Bambus1 + 1, size)

                ' Zeichen (萬)
                    Case SteinIndexEnum.Symbol1 To SteinIndexEnum.Symbol9
                        DrawChineseCharacter(g, "萬", idx - SteinIndexEnum.Symbol1 + 1, size)

                ' Drachen
                    Case SteinIndexEnum.DracheR
                        DrawChineseText(g, "中", size, Brushes.Red)
                    Case SteinIndexEnum.DracheG
                        DrawChineseText(g, "發", size, Brushes.Green)
                    Case SteinIndexEnum.DracheW
                        DrawChineseText(g, "□", size, Brushes.Blue) ' Weißer Drache = leeres Feld / blauer Rahmen

                ' Winde
                    Case SteinIndexEnum.WindOst
                        DrawChineseText(g, "東", size, Brushes.Black)
                    Case SteinIndexEnum.WindSüd
                        DrawChineseText(g, "南", size, Brushes.Black)
                    Case SteinIndexEnum.WindWst
                        DrawChineseText(g, "西", size, Brushes.Black)
                    Case SteinIndexEnum.WindNrd
                        DrawChineseText(g, "北", size, Brushes.Black)

                ' Blumen (Pflaume, Orchidee, Chrysantheme, Bambus)
                    Case SteinIndexEnum.BlütePf
                        DrawChineseText(g, "梅", size, Brushes.Magenta) ' Pflaume
                    Case SteinIndexEnum.BlüteOr
                        DrawChineseText(g, "蘭", size, Brushes.Purple) ' Orchidee
                    Case SteinIndexEnum.BlüteCt
                        DrawChineseText(g, "菊", size, Brushes.Orange) ' Chrysantheme
                    Case SteinIndexEnum.BlüteBa
                        DrawChineseText(g, "竹", size, Brushes.Green)  ' Bambus

                ' Jahreszeiten (Frühling, Sommer, Herbst, Winter)
                    Case SteinIndexEnum.JahrFrl
                        DrawChineseText(g, "春", size, Brushes.Green)
                    Case SteinIndexEnum.JahrSom
                        DrawChineseText(g, "夏", size, Brushes.OrangeRed)
                    Case SteinIndexEnum.JahrHer
                        DrawChineseText(g, "秋", size, Brushes.Brown)
                    Case SteinIndexEnum.JahrWin
                        DrawChineseText(g, "冬", size, Brushes.Blue)

                    Case Else
                        DrawChineseText(g, "？", size, Brushes.Gray)
                End Select
            End Using
            Return bmp
        End Function

        '
        ''' <summary>
        ''' Zeichnet Punkte-Symbole (klassische Kreise).
        ''' </summary>
        Private Sub DrawChineseDots(g As Graphics, number As Integer, size As Size)
            Dim r As Single = size.Width * 0.18F
            Dim positions As New List(Of PointF)

            ' Einfaches 3x3-Raster
            Dim cellW As Single = CSng(size.Width / 3)
            Dim cellH As Single = CSng(size.Height / 3)
            For row As Integer = 0 To 2
                For col As Integer = 0 To 2
                    positions.Add(New PointF(col * cellW + cellW / 2, row * cellH + cellH / 2))
                Next
            Next

            ' Farben klassisch Mahjong: Rot für 1, Blau/Grün gemischt für mehrere
            Dim colors() As Brush = {Brushes.Red, Brushes.Blue, Brushes.Green}

            For i As Integer = 0 To number - 1
                Dim pos As PointF = positions(i)
                g.FillEllipse(colors(i Mod colors.Length),
                          pos.X - r / 2, pos.Y - r / 2, r, r)
                g.DrawEllipse(Pens.Black, pos.X - r / 2, pos.Y - r / 2, r, r)
            Next
        End Sub

        '
        ''' <summary>
        ''' Zeichnet Bambusstäbe (grün, vertikal).
        ''' </summary>
        Private Sub DrawChineseBamboo(g As Graphics, number As Integer, size As Size)
            Dim barW As Single = CSng(size.Width / 5)
            Dim barH As Single = CSng(size.Height / 4)
            Dim spacing As Single = CSng(size.Width / 6)

            For i As Integer = 0 To number - 1
                Dim x As Single = CSng((i Mod 3) * spacing + size.Width / 3)
                Dim y As Single = CSng((i \ 3) * barH + size.Height / 8)
                Using br As New SolidBrush(Color.Green)
                    g.FillRectangle(br, x, y, barW, barH)
                End Using
                g.DrawRectangle(Pens.Black, x, y, barW, barH)
            Next
        End Sub

        '
        ''' <summary>
        ''' Zeichnet ein chinesisches Schriftzeichen (萬 für Characters).
        ''' </summary>
        Private Sub DrawChineseCharacter(g As Graphics, ch As String, number As Integer, size As Size)
            DrawChineseText(g, ch, size, Brushes.Red)
            ' Zahl als arabisch in Ecke setzen
            Using fnt As New Font("Arial", size.Height / 6.0F, FontStyle.Bold, GraphicsUnit.Pixel)
                g.DrawString(number.ToString(), fnt, Brushes.Black, 4, size.Height - CSng(size.Height / 5))
            End Using
        End Sub

        '
        ''' <summary>
        ''' Zeichnet ein einzelnes Schriftzeichen zentriert.
        ''' </summary>
        Private Sub DrawChineseText(g As Graphics, txt As String, size As Size, br As Brush)
            Using fnt As New Font("SimSun", CSng(size.Height / 2), FontStyle.Bold, GraphicsUnit.Pixel)
                Dim sf As New StringFormat With {.Alignment = StringAlignment.Center, .LineAlignment = StringAlignment.Center}
                g.DrawString(txt, fnt, br, New RectangleF(0, 0, size.Width, size.Height), sf)
            End Using
        End Sub

    End Module
End Namespace
