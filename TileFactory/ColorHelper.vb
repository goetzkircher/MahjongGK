Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.Globalization

Friend Module ColorHelper

    Private Const EPSILON As Double = 0.0000001R

    ''' <summary>
    ''' Liefert den Farbton einer Farbe als Single von 0 bis 360 Grad. Bei Graustufen ist der Rückgabewert 0.
    ''' </summary>
    Public Function GetHue(color As Color) As Single

        Dim hsb As (hue As Double, saturation As Double, brightness As Double) = GetHsb01(color)
        Dim result As Single = CSng(Math.Round(hsb.hue, MidpointRounding.AwayFromZero))

        If result >= 360 Then
            result = 0
        End If

        Return result

    End Function

    ''' <summary>
    ''' Liefert die Sättigung einer Farbe im HSB-Farbraum als Integer von 0 bis 100.
    ''' </summary>
    Public Function GetSaturation(color As Color) As Integer

        Dim hsb As (hue As Double, saturation As Double, brightness As Double) = GetHsb01(color)
        Dim saturation100 As Integer =
            CInt(Math.Round(hsb.saturation * 100.0R, MidpointRounding.AwayFromZero))

        Return ClampToPercent(saturation100)

    End Function

    ''' <summary>
    ''' Liefert die Brightness einer Farbe im HSB-Farbraum als Integer von 0 bis 100.
    ''' </summary>
    Public Function GetBrightness(color As Color) As Integer

        Dim hsb As (hue As Double, saturation As Double, brightness As Double) = GetHsb01(color)
        Dim brightness100 As Integer =
            CInt(Math.Round(hsb.brightness * 100.0R, MidpointRounding.AwayFromZero))

        Return ClampToPercent(brightness100)

    End Function

    ''' <summary>
    ''' Liefert Sättigung und Brightness als Tupel (jeweils 0 bis 100).
    ''' </summary>
    Public Function GetHSB(color As Color) As (hue As Single, saturation As Integer, brightness As Integer)

        Dim hue As Single = GetHue(color)
        Dim saturation As Integer = GetSaturation(color)
        Dim brightness As Integer = GetBrightness(color)
        Dim result As (hue As Single, saturation As Integer, brightness As Integer) = (hue, saturation, brightness)

        Return result

    End Function

    ''' <summary>
    ''' Erzeugt eine Color aus HSB.
    ''' hue: 0 bis 360 Grad
    ''' saturation: 0 bis 100 Prozent
    ''' brightness: 0 bis 100 Prozent
    ''' Werte außerhalb des Bereichs werden angepasst.
    ''' Alpha ist immer 255.
    ''' </summary>
    Public Function ColorFrmHSB(hue As Single,
                                saturation As Integer,
                                brightness As Integer) As Color

        Dim hueNorm As Double = NormalizeHue(CDbl(hue))
        Dim satNorm As Double = CDbl(ClampToPercent(saturation)) / 100.0R
        Dim briNorm As Double = CDbl(ClampToPercent(brightness)) / 100.0R

        Return ColorFromHsb(255, hueNorm, satNorm, briNorm)

    End Function

    ''' <summary>
    ''' Alternative mit korrekter Schreibweise.
    ''' </summary>
    Public Function ColorFromHSB(hue As Single,
                                 saturation As Integer,
                                 brightness As Integer) As Color

        Return ColorFrmHSB(hue, saturation, brightness)

    End Function

    ''' <summary>
    ''' Erzeugt eine Farbnuance mit absolut gesetzter Sättigung und Brightness.
    ''' Hue und Alpha bleiben erhalten.
    ''' Wertebereich für absoluteSaturation und absoluteBrightness: 0 bis 100.
    ''' </summary>
    Public Function GetColorNuanceAbsolute(color As Color,
                                           absoluteSaturation As Integer,
                                           absoluteBrightness As Integer) As Color

        Dim hsb As (hue As Double, saturation As Double, brightness As Double) = GetHsb01(color)
        Dim saturation As Double = CDbl(ClampToPercent(absoluteSaturation)) / 100.0R
        Dim brightness As Double = CDbl(ClampToPercent(absoluteBrightness)) / 100.0R

        Return ColorFromHsb(color.A, hsb.hue, saturation, brightness)

    End Function

    ''' <summary>
    ''' Erzeugt eine Farbnuance relativ zur Ausgangsfarbe.
    ''' Hue und Alpha bleiben erhalten.
    ''' deltaSaturation und deltaBrightness dürfen negativ sein.
    ''' Das Ergebnis wird jeweils auf 0 bis 100 begrenzt.
    ''' </summary>
    Public Function GetColorNuanceDelta(color As Color,
                                        deltaSaturation As Integer,
                                        deltaBrightness As Integer) As Color

        Dim newSaturation As Integer = GetSaturation(color) + deltaSaturation
        Dim newBrightness As Integer = GetBrightness(color) + deltaBrightness

        Return GetColorNuanceAbsolute(color, newSaturation, newBrightness)

    End Function

    ''' <summary>
    ''' Alternative mit gemeinsamem Funktionsnamen.
    ''' </summary>
    Public Function GetColorNuance(color As Color,
                                   valueSaturation As Integer,
                                   valueBrightness As Integer,
                                   mode As ColorNuanceMode) As Color

        Select Case mode
            Case ColorNuanceMode.Absolute
                Return GetColorNuanceAbsolute(color, valueSaturation, valueBrightness)

            Case ColorNuanceMode.Delta
                Return GetColorNuanceDelta(color, valueSaturation, valueBrightness)

            Case Else
                Throw New ArgumentOutOfRangeException(NameOf(mode))
        End Select

    End Function

    Public Enum ColorNuanceMode
        Absolute
        Delta
    End Enum

    ''' <summary>
    ''' Liefert HSB-Komponenten als Double:
    ''' hue: 0..360
    ''' saturation: 0..1
    ''' brightness: 0..1
    ''' </summary>
    Private Function GetHsb01(color As Color) As (hue As Double, saturation As Double, brightness As Double)

        Dim r As Double = CDbl(color.R) / 255.0R
        Dim g As Double = CDbl(color.G) / 255.0R
        Dim b As Double = CDbl(color.B) / 255.0R

        Dim maxValue As Double = Math.Max(r, Math.Max(g, b))
        Dim minValue As Double = Math.Min(r, Math.Min(g, b))
        Dim delta As Double = maxValue - minValue

        Dim hue As Double
        Dim saturation As Double
        Dim brightness As Double = maxValue

        If maxValue <= EPSILON Then
            saturation = 0.0R
        Else
            saturation = delta / maxValue
        End If

        If delta <= EPSILON Then
            hue = 0.0R
        ElseIf Math.Abs(maxValue - r) <= EPSILON Then
            hue = 60.0R * ((g - b) / delta)
        ElseIf Math.Abs(maxValue - g) <= EPSILON Then
            hue = 60.0R * (((b - r) / delta) + 2.0R)
        Else
            hue = 60.0R * (((r - g) / delta) + 4.0R)
        End If

        hue = NormalizeHue(hue)

        Dim result As (hue As Double, saturation As Double, brightness As Double) =
            (hue, Clamp01(saturation), Clamp01(brightness))

        Return result

    End Function

    ''' <summary>
    ''' Erzeugt eine Color aus HSB-Komponenten.
    ''' hue: 0..360
    ''' saturation: 0..1
    ''' brightness: 0..1
    ''' </summary>
    Private Function ColorFromHsb(alpha As Integer,
                                  hue As Double,
                                  saturation As Double,
                                  brightness As Double) As Color

        Dim h As Double = NormalizeHue(hue)
        Dim s As Double = Clamp01(saturation)
        Dim v As Double = Clamp01(brightness)

        Dim c As Double = v * s
        Dim hPrime As Double = h / 60.0R
        Dim x As Double = c * (1.0R - Math.Abs((hPrime Mod 2.0R) - 1.0R))
        Dim m As Double = v - c

        Dim r1 As Double
        Dim g1 As Double
        Dim b1 As Double

        If hPrime < 1.0R Then
            r1 = c
            g1 = x
            b1 = 0.0R
        ElseIf hPrime < 2.0R Then
            r1 = x
            g1 = c
            b1 = 0.0R
        ElseIf hPrime < 3.0R Then
            r1 = 0.0R
            g1 = c
            b1 = x
        ElseIf hPrime < 4.0R Then
            r1 = 0.0R
            g1 = x
            b1 = c
        ElseIf hPrime < 5.0R Then
            r1 = x
            g1 = 0.0R
            b1 = c
        Else
            r1 = c
            g1 = 0.0R
            b1 = x
        End If

        Dim aInt As Integer = ClampToByte(alpha)
        Dim rInt As Integer = ClampToByte(CInt(Math.Round((r1 + m) * 255.0R, MidpointRounding.AwayFromZero)))
        Dim gInt As Integer = ClampToByte(CInt(Math.Round((g1 + m) * 255.0R, MidpointRounding.AwayFromZero)))
        Dim bInt As Integer = ClampToByte(CInt(Math.Round((b1 + m) * 255.0R, MidpointRounding.AwayFromZero)))

        Return Color.FromArgb(aInt, rInt, gInt, bInt)

    End Function

    Private Function NormalizeHue(hue As Double) As Double

        Dim result As Double = hue Mod 360.0R

        If result < 0.0R Then
            result += 360.0R
        End If

        If result >= 360.0R Then
            result -= 360.0R
        End If

        Return result

    End Function

    Private Function Clamp01(value As Double) As Double

        If value < 0.0R Then
            Return 0.0R
        End If

        If value > 1.0R Then
            Return 1.0R
        End If

        Return value

    End Function

    Private Function ClampToPercent(value As Integer) As Integer

        If value < 0 Then
            Return 0
        End If

        If value > 100 Then
            Return 100
        End If

        Return value

    End Function

    Private Function ClampToByte(value As Integer) As Integer

        If value < 0 Then
            Return 0
        End If

        If value > 255 Then
            Return 255
        End If

        Return value

    End Function

    Public Function InterpolateColor(
                            color1 As Color,
                            color2 As Color,
                            t As Double) As Color

        'If t < 0.0R Then t = 0.0R
        'If t > 1.0R Then t = 1.0R

        Dim a As Integer = CInt(Math.Round(color1.A + (color2.A - color1.A) * t))
        Dim r As Integer = CInt(Math.Round(color1.R + (color2.R - color1.R) * t))
        Dim g As Integer = CInt(Math.Round(color1.G + (color2.G - color1.G) * t))
        Dim b As Integer = CInt(Math.Round(color1.B + (color2.B - color1.B) * t))

        Return Color.FromArgb(a, r, g, b)
    End Function

    Public Function TryParseArgbHexColor(text As String, ByRef result As Color) As Boolean

        result = Color.Empty

        If text Is Nothing Then Return False

        Dim s As String = text.Trim()

        If s.StartsWith("#", StringComparison.Ordinal) Then
            s = s.Substring(1)
        End If

        If s.StartsWith("&H", StringComparison.OrdinalIgnoreCase) Then
            s = s.Substring(2)
        End If

        If s.Length <> 8 Then Return False

        Dim value As UInteger
        If Not UInteger.TryParse(s, NumberStyles.HexNumber, CultureInfo.InvariantCulture, value) Then
            Return False
        End If

        Dim raw As Integer = BitConverter.ToInt32(BitConverter.GetBytes(value), 0)
        result = Color.FromArgb(raw)

        Return True

    End Function

    ''' <summary>
    ''' Ändert Sättigung und Helligkeit einer Bitmap in einem Durchgang.
    ''' satFaktor: 1.0 = unverändert, 0.0 = Graustufen, größer 1.0 = stärker gesättigt.
    ''' brgFactor:
    '''   modusMultiplikation=True:  1.0 = unverändert, größer = heller, kleiner = dunkler.
    '''   modusMultiplikation=False: 0.0 = unverändert, positiv = heller, negativ = dunkler.
    ''' Alpha bleibt unverändert.
    ''' </summary>
    Public Function ChangeSaturationAndBrightness(bmpSrc As Bitmap,
                                                  satFaktor As Decimal,
                                                  brgFactor As Decimal,
                                                  modusMultiplikation As Boolean) As Bitmap

        If bmpSrc Is Nothing Then
            Throw New ArgumentNullException(NameOf(bmpSrc))
        End If

        If satFaktor < 0D Then
            Throw New ArgumentOutOfRangeException(NameOf(satFaktor), "Der Sättigungsfaktor darf nicht negativ sein.")
        End If

        If modusMultiplikation AndAlso brgFactor < 0D Then
            Throw New ArgumentOutOfRangeException(NameOf(brgFactor), "Der Helligkeitsfaktor darf nicht negativ sein.")
        End If

        Dim s As Single = CSng(satFaktor)
        Dim b As Single = CSng(brgFactor)

        Const lumR As Single = 0.3086F
        Const lumG As Single = 0.6094F
        Const lumB As Single = 0.082F

        Dim sr As Single = (1.0F - s) * lumR
        Dim sg As Single = (1.0F - s) * lumG
        Dim sb As Single = (1.0F - s) * lumB

        Dim satMatrix As New ColorMatrix(
            New Single()() {
                New Single() {sr + s, sg, sb, 0.0F, 0.0F},
                New Single() {sr, sg + s, sb, 0.0F, 0.0F},
                New Single() {sr, sg, sb + s, 0.0F, 0.0F},
                New Single() {0.0F, 0.0F, 0.0F, 1.0F, 0.0F},
                New Single() {0.0F, 0.0F, 0.0F, 0.0F, 1.0F}
            })

        Dim brgMatrix As ColorMatrix

        If modusMultiplikation Then

            brgMatrix = New ColorMatrix(
                New Single()() {
                    New Single() {b, 0.0F, 0.0F, 0.0F, 0.0F},
                    New Single() {0.0F, b, 0.0F, 0.0F, 0.0F},
                    New Single() {0.0F, 0.0F, b, 0.0F, 0.0F},
                    New Single() {0.0F, 0.0F, 0.0F, 1.0F, 0.0F},
                    New Single() {0.0F, 0.0F, 0.0F, 0.0F, 1.0F}
                })

        Else

            brgMatrix = New ColorMatrix(
                New Single()() {
                    New Single() {1.0F, 0.0F, 0.0F, 0.0F, 0.0F},
                    New Single() {0.0F, 1.0F, 0.0F, 0.0F, 0.0F},
                    New Single() {0.0F, 0.0F, 1.0F, 0.0F, 0.0F},
                    New Single() {0.0F, 0.0F, 0.0F, 1.0F, 0.0F},
                    New Single() {b, b, b, 0.0F, 1.0F}
                })

        End If

        Dim combinedMatrix As ColorMatrix = MultiplyColorMatrices(satMatrix, brgMatrix)

        Dim bmpDst As New Bitmap(bmpSrc.Width, bmpSrc.Height, PixelFormat.Format32bppArgb)

        Using ia As New ImageAttributes()
            ia.SetColorMatrix(combinedMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap)

            Using g As Graphics = Graphics.FromImage(bmpDst)
                g.DrawImage(
                    bmpSrc,
                    New Rectangle(0, 0, bmpSrc.Width, bmpSrc.Height),
                    0,
                    0,
                    bmpSrc.Width,
                    bmpSrc.Height,
                    GraphicsUnit.Pixel,
                    ia)
            End Using
        End Using

        Return bmpDst

    End Function

    Private Function MultiplyColorMatrices(a As ColorMatrix,
                                       b As ColorMatrix) As ColorMatrix

        Dim result(4)() As Single

        For row As Integer = 0 To 4

            result(row) = New Single(4) {}

            For col As Integer = 0 To 4

                Dim value As Single = 0.0F

                For k As Integer = 0 To 4
                    value += a(row, k) * b(k, col)
                Next

                result(row)(col) = value

            Next

        Next

        Return New ColorMatrix(result)

    End Function

End Module