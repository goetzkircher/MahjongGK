Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On
Imports System.Drawing.Imaging

Namespace Spielfeld
    '
    ''' <summary>
    ''' Ermittelt aus einer Hintergrund-Bitmap eine dominante Farbe und leitet daraus
    ''' Overlay-Farben für Normal, MouseOver, MouseDown und Selected ab.
    ''' 
    ''' Die Klasse besitzt die übergebene Bitmap nicht, sondern wertet sie nur aus.
    ''' </summary>
    Public NotInheritable Class OverlayColorPalette

        '
        ''' <summary>
        ''' Die aus der Bitmap bestimmte dominante Hintergrundfarbe.
        ''' </summary>
        Public ReadOnly Property DominantBackgroundColor As Color

        '
        ''' <summary>
        ''' Grundfarbe für das Overlay.
        ''' </summary>
        Public ReadOnly Property ColorNormal As Color

        '
        ''' <summary>
        ''' Farbe für MouseOver.
        ''' </summary>
        Public ReadOnly Property ColorMouseOver As Color

        '
        ''' <summary>
        ''' Farbe für MouseDown.
        ''' </summary>
        Public ReadOnly Property ColorMouseDown As Color

        '
        ''' <summary>
        ''' Farbe für Selected.
        ''' </summary>
        Public ReadOnly Property ColorSelected As Color

        '
        ''' <summary>
        ''' Leerer Konstruktor. Es wird zunächst eine neutrale Palette erzeugt.
        ''' </summary>
        Public Sub New()

            Me.New(Color.Gray)

        End Sub

        '
        ''' <summary>
        ''' Konstruktor mit direkter Vorgabe einer Hintergrund-Bitmap.
        ''' </summary>
        Public Sub New(ByVal bmpBackground As Bitmap)

            Me.New(DominantColorFromBitmap(bmpBackground))

        End Sub

        '
        ''' <summary>
        ''' Konstruktor mit direkter Vorgabe einer Hintergrundfarbe.
        ''' </summary>
        Public Sub New(ByVal backgroundColor As Color)

            DominantBackgroundColor = backgroundColor

            Dim normalColor As Color = GetOverlayColorFromBackground(backgroundColor)
            Dim bright As Single = backgroundColor.GetBrightness()

            ColorNormal = normalColor

            If bright < 0.5F Then
                ColorMouseOver = BlendWith(normalColor, Color.White, 0.18F)
                ColorMouseDown = BlendWith(normalColor, Color.Black, 0.22F)
                ColorSelected = BlendWith(normalColor, Color.White, 0.32F)
            Else
                ColorMouseOver = BlendWith(normalColor, Color.Black, 0.18F)
                ColorMouseDown = BlendWith(normalColor, Color.Black, 0.32F)
                ColorSelected = BlendWith(normalColor, Color.Black, 0.12F)
            End If

        End Sub

        '
        ''' <summary>
        ''' Erzeugt aus einer Hintergrund-Bitmap eine neue Palette.
        ''' </summary>
        Public Shared Function FromBitmap(ByVal bmpBackground As Bitmap) As OverlayColorPalette

            Return New OverlayColorPalette(bmpBackground)

        End Function

        '
        ''' <summary>
        ''' Erzeugt aus einer Hintergrundfarbe eine neue Palette.
        ''' </summary>
        Public Shared Function FromColor(ByVal backgroundColor As Color) As OverlayColorPalette

            Return New OverlayColorPalette(backgroundColor)

        End Function

        '
        ''' <summary>
        ''' Gibt eine neue Palette auf Basis einer anderen Bitmap zurück.
        ''' </summary>
        Public Function WithBackgroundBitmap(ByVal bmpBackground As Bitmap) As OverlayColorPalette

            Return New OverlayColorPalette(bmpBackground)

        End Function

        '
        ''' <summary>
        ''' Gibt eine neue Palette auf Basis einer anderen Hintergrundfarbe zurück.
        ''' </summary>
        Public Function WithBackgroundColor(ByVal backgroundColor As Color) As OverlayColorPalette

            Return New OverlayColorPalette(backgroundColor)

        End Function

        '
        ''' <summary>
        ''' Bestimmt aus einer Bitmap eine dominante Farbe.
        ''' </summary>
        Public Shared Function DominantColorFromBitmap(ByVal bmpSrc As Bitmap) As Color

            If bmpSrc Is Nothing Then
                Return Color.Black
            End If

            Dim bmp As Bitmap = bmpSrc

            If bmp.PixelFormat <> PixelFormat.Format32bppArgb Then
                bmp = New Bitmap(bmpSrc.Width, bmpSrc.Height, PixelFormat.Format32bppArgb)
                Using g As Graphics = Graphics.FromImage(bmp)
                    g.DrawImage(bmpSrc, 0, 0, bmpSrc.Width, bmpSrc.Height)
                End Using
            End If

            Const stepXY As Integer = 4
            Const Q As Integer = 16

            Dim counts(Q - 1, Q - 1, Q - 1) As Double
            Dim sumR(Q - 1, Q - 1, Q - 1) As Double
            Dim sumG(Q - 1, Q - 1, Q - 1) As Double
            Dim sumB(Q - 1, Q - 1, Q - 1) As Double

            Dim rect As Rectangle = New Rectangle(0, 0, bmp.Width, bmp.Height)
            Dim data As BitmapData = bmp.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb)

            Try
                Dim stride As Integer = data.Stride
                Dim basePtr As IntPtr = data.Scan0

                For y As Integer = 0 To bmp.Height - 1 Step stepXY
                    Dim row As Integer = y * stride

                    For x As Integer = 0 To bmp.Width - 1 Step stepXY
                        Dim p As Integer = row + x * 4

                        Dim bb As Byte = Runtime.InteropServices.Marshal.ReadByte(basePtr, p + 0)
                        Dim gg As Byte = Runtime.InteropServices.Marshal.ReadByte(basePtr, p + 1)
                        Dim rr As Byte = Runtime.InteropServices.Marshal.ReadByte(basePtr, p + 2)
                        Dim aa As Byte = Runtime.InteropServices.Marshal.ReadByte(basePtr, p + 3)

                        If aa < 16 Then
                            Continue For
                        End If

                        Dim h As Double
                        Dim s As Double
                        Dim v As Double
                        RgbToHsv(CInt(rr), CInt(gg), CInt(bb), h, s, v)

                        Dim sat As Double = 0.3 + 0.7 * s
                        Dim lum As Double = 0.6 + 0.4 * v
                        Dim w As Double = (aa / 255.0) * sat * lum

                        If w <= 0.0 Then
                            Continue For
                        End If

                        Dim ri As Integer = rr >> 4
                        Dim gi As Integer = gg >> 4
                        Dim bi As Integer = bb >> 4

                        counts(ri, gi, bi) += w
                        sumR(ri, gi, bi) += rr * w
                        sumG(ri, gi, bi) += gg * w
                        sumB(ri, gi, bi) += bb * w
                    Next
                Next

                Dim br As Integer = 0
                Dim bg As Integer = 0
                Dim bbMax As Integer = 0
                Dim best As Double = -1.0

                For ri As Integer = 0 To Q - 1
                    For gi As Integer = 0 To Q - 1
                        For bi As Integer = 0 To Q - 1
                            If counts(ri, gi, bi) > best Then
                                best = counts(ri, gi, bi)
                                br = ri
                                bg = gi
                                bbMax = bi
                            End If
                        Next
                    Next
                Next

                If best <= 0.0 Then
                    Return Color.Black
                End If

                Dim mr As Integer = CInt(Math.Round(sumR(br, bg, bbMax) / best))
                Dim mg As Integer = CInt(Math.Round(sumG(br, bg, bbMax) / best))
                Dim mb As Integer = CInt(Math.Round(sumB(br, bg, bbMax) / best))

                Return Color.FromArgb(255,
                                  ClampByte(mr),
                                  ClampByte(mg),
                                  ClampByte(mb))

            Finally
                bmp.UnlockBits(data)

                If Not Object.ReferenceEquals(bmp, bmpSrc) Then
                    bmp.Dispose()
                End If
            End Try

        End Function

        '
        ''' <summary>
        ''' Leitet aus einer Hintergrundfarbe die eigentliche Overlay-Grundfarbe ab.
        ''' </summary>
        Private Shared Function GetOverlayColorFromBackground(ByVal backColor As Color) As Color

            If IsNearlyGray(backColor, 12) Then
                Return GetBlackOrWhiteContrast(backColor)
            End If

            Dim result As Color = GetComplementaryColorHue(backColor)
            Dim brightness As Single = backColor.GetBrightness()

            If brightness < 0.35F Then
                result = BlendWith(result, Color.White, 0.22F)
            ElseIf brightness > 0.7F Then
                result = BlendWith(result, Color.Black, 0.22F)
            End If

            Return result

        End Function

        '
        ''' <summary>
        ''' Liefert bei nahezu grauen Farben stattdessen eine klare Kontrastfarbe.
        ''' </summary>
        Private Shared Function GetBlackOrWhiteContrast(ByVal c As Color) As Color

            If c.GetBrightness() < 0.5F Then
                Return Color.White
            Else
                Return Color.Black
            End If

        End Function

        '
        ''' <summary>
        ''' Prüft, ob eine Farbe nahezu grau ist.
        ''' </summary>
        Private Shared Function IsNearlyGray(ByVal c As Color,
                                         ByVal tolerance As Integer) As Boolean

            Dim drg As Integer = Math.Abs(CInt(c.R) - CInt(c.G))
            Dim drb As Integer = Math.Abs(CInt(c.R) - CInt(c.B))
            Dim dgb As Integer = Math.Abs(CInt(c.G) - CInt(c.B))

            Return drg <= tolerance AndAlso
               drb <= tolerance AndAlso
               dgb <= tolerance

        End Function

        '
        ''' <summary>
        ''' Liefert die Komplementärfarbe über die Hue-Drehung.
        ''' Bei Grauwerten bleibt die Farbe unverändert.
        ''' </summary>
        Private Shared Function GetComplementaryColorHue(ByVal c As Color) As Color

            Dim s As Single = c.GetSaturation()

            If s <= 0.001F Then
                Return c
            End If

            Dim h As Single = c.GetHue()
            Dim l As Single = c.GetBrightness()

            h += 180.0F
            If h >= 360.0F Then
                h -= 360.0F
            End If

            Return ColorFromHSL(h, s, l, c.A)

        End Function

        '
        ''' <summary>
        ''' Mischt zwei Farben.
        ''' amount = 0 ergibt colorA, amount = 1 ergibt colorB.
        ''' </summary>
        Private Shared Function BlendWith(ByVal colorA As Color,
                                      ByVal colorB As Color,
                                      ByVal amount As Single) As Color

            If amount <= 0.0F Then
                Return colorA
            End If

            If amount >= 1.0F Then
                Return colorB
            End If

            Dim r As Integer = CInt(CSng(colorA.R) + (CSng(colorB.R) - CSng(colorA.R)) * amount)
            Dim g As Integer = CInt(CSng(colorA.G) + (CSng(colorB.G) - CSng(colorA.G)) * amount)
            Dim b As Integer = CInt(CSng(colorA.B) + (CSng(colorB.B) - CSng(colorA.B)) * amount)

            Return Color.FromArgb(colorA.A, ClampByte(r), ClampByte(g), ClampByte(b))

        End Function

        '
        ''' <summary>
        ''' Wandelt HSL in Color um.
        ''' </summary>
        Private Shared Function ColorFromHSL(ByVal h As Single,
                                         ByVal s As Single,
                                         ByVal l As Single,
                                         ByVal a As Integer) As Color

            If s <= 0.001F Then
                Dim gray As Integer = ClampByte(CInt(l * 255.0F))
                Return Color.FromArgb(a, gray, gray, gray)
            End If

            Dim q As Single
            If l < 0.5F Then
                q = l * (1.0F + s)
            Else
                q = l + s - l * s
            End If

            Dim p As Single = 2.0F * l - q
            Dim hk As Single = h / 360.0F

            Dim tr As Single = hk + (1.0F / 3.0F)
            Dim tg As Single = hk
            Dim tb As Single = hk - (1.0F / 3.0F)

            Dim r As Integer = ClampByte(CInt(HueToRGB(p, q, tr) * 255.0F))
            Dim g As Integer = ClampByte(CInt(HueToRGB(p, q, tg) * 255.0F))
            Dim b As Integer = ClampByte(CInt(HueToRGB(p, q, tb) * 255.0F))

            Return Color.FromArgb(a, r, g, b)

        End Function

        '
        ''' <summary>
        ''' Hilfsfunktion für HSL nach RGB.
        ''' </summary>
        Private Shared Function HueToRGB(ByVal p As Single,
                                     ByVal q As Single,
                                     ByVal t As Single) As Single

            If t < 0.0F Then
                t += 1.0F
            End If

            If t > 1.0F Then
                t -= 1.0F
            End If

            If t < (1.0F / 6.0F) Then
                Return p + (q - p) * 6.0F * t
            End If

            If t < 0.5F Then
                Return q
            End If

            If t < (2.0F / 3.0F) Then
                Return p + (q - p) * ((2.0F / 3.0F) - t) * 6.0F
            End If

            Return p

        End Function

        '
        ''' <summary>
        ''' RGB nach HSV.
        ''' h = 0..360, s = 0..1, v = 0..1
        ''' </summary>
        Private Shared Sub RgbToHsv(ByVal r As Integer,
                                ByVal g As Integer,
                                ByVal b As Integer,
                                ByRef h As Double,
                                ByRef s As Double,
                                ByRef v As Double)

            Dim rf As Double = r / 255.0
            Dim gf As Double = g / 255.0
            Dim bf As Double = b / 255.0

            Dim maxC As Double = Math.Max(rf, Math.Max(gf, bf))
            Dim minC As Double = Math.Min(rf, Math.Min(gf, bf))
            Dim delta As Double = maxC - minC

            v = maxC

            If maxC <= 0.0 Then
                s = 0.0
                h = 0.0
                Exit Sub
            End If

            s = delta / maxC

            If delta <= 0.0 Then
                h = 0.0
                Exit Sub
            End If

            If maxC = rf Then
                h = 60.0 * (((gf - bf) / delta) Mod 6.0)
            ElseIf maxC = gf Then
                h = 60.0 * (((bf - rf) / delta) + 2.0)
            Else
                h = 60.0 * (((rf - gf) / delta) + 4.0)
            End If

            If h < 0.0 Then
                h += 360.0
            End If

        End Sub

        '
        ''' <summary>
        ''' Begrenzt einen Integer auf den Byte-Bereich.
        ''' </summary>
        Private Shared Function ClampByte(ByVal value As Integer) As Integer

            If value < 0 Then
                Return 0
            End If

            If value > 255 Then
                Return 255
            End If

            Return value

        End Function

    End Class
End Namespace