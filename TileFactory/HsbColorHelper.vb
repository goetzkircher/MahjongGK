Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.Runtime.InteropServices

Public Module HsbColorHelper

    Private Const EPSILON As Double = 0.0000001R
    ''' <summary>
    ''' deltaHue in Grad, deltaSaturation und deltaBrightness in Prozentpunkten, 
    ''' alpha ebenfalls als Byte-Differenz auf Alpha -255..+255.
    ''' </summary>
    ''' <param name="bmpSrc"></param>
    ''' <param name="alpha"></param>
    ''' <param name="deltaHue"></param>
    ''' <param name="deltaSaturation"></param>
    ''' <param name="deltaBrightness"></param>
    ''' <param name="disposeBmpSrc"></param>
    ''' <returns></returns>
    Public Function HsbAdjustment(bmpSrc As Bitmap,
                              alpha As Decimal,
                              deltaHue As Decimal,
                              deltaSaturation As Decimal,
                              deltaBrightness As Decimal,
                              Optional disposeBmpSrc As Boolean = False) As Bitmap

        If bmpSrc Is Nothing Then
            Throw New ArgumentNullException(NameOf(bmpSrc))
        End If

        Dim width As Integer = bmpSrc.Width
        Dim height As Integer = bmpSrc.Height
        Dim rect As New Rectangle(0, 0, width, height)

        Dim src32 As Bitmap = Nothing
        Dim srcIsClone As Boolean = False

        If bmpSrc.PixelFormat = PixelFormat.Format32bppArgb OrElse
       bmpSrc.PixelFormat = PixelFormat.Format32bppPArgb Then

            src32 = bmpSrc
        Else
            src32 = New Bitmap(width, height, PixelFormat.Format32bppArgb)
            Using g As Graphics = Graphics.FromImage(src32)
                g.DrawImageUnscaled(bmpSrc, 0, 0)
            End Using
            srcIsClone = True
        End If

        Dim result As New Bitmap(width, height, PixelFormat.Format32bppArgb)

        Dim srcData As BitmapData = Nothing
        Dim dstData As BitmapData = Nothing

        Try
            srcData = src32.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb)
            dstData = result.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb)

            Dim srcStride As Integer = srcData.Stride
            Dim dstStride As Integer = dstData.Stride
            Dim srcBytes As Integer = Math.Abs(srcStride) * height
            Dim dstBytes As Integer = Math.Abs(dstStride) * height

            Dim srcBuffer(srcBytes - 1) As Byte
            Dim dstBuffer(dstBytes - 1) As Byte

            Marshal.Copy(srcData.Scan0, srcBuffer, 0, srcBytes)

            Dim dH As Double = CDbl(deltaHue)
            Dim dS As Double = CDbl(deltaSaturation) / 100.0R
            Dim dB As Double = CDbl(deltaBrightness) / 100.0R

            Dim onlyAlpha As Boolean =
            dH = 0.0R AndAlso dS = 0.0R AndAlso dB = 0.0R

            For y As Integer = 0 To height - 1

                Dim srcRow As Integer = y * srcStride
                Dim dstRow As Integer = y * dstStride

                For x As Integer = 0 To width - 1

                    Dim srcIndex As Integer = srcRow + x * 4
                    Dim dstIndex As Integer = dstRow + x * 4

                    Dim bByte As Byte = srcBuffer(srcIndex)
                    Dim gByte As Byte = srcBuffer(srcIndex + 1)
                    Dim rByte As Byte = srcBuffer(srcIndex + 2)
                    Dim aByte As Byte = srcBuffer(srcIndex + 3)

                    If onlyAlpha Then
                        dstBuffer(dstIndex) = bByte
                        dstBuffer(dstIndex + 1) = gByte
                        dstBuffer(dstIndex + 2) = rByte
                        If aByte > 0 Then
                            dstBuffer(dstIndex + 3) = CByte(alpha)
                        End If
                    Else
                        Dim r As Double = CDbl(rByte) / 255.0R
                        Dim g As Double = CDbl(gByte) / 255.0R
                        Dim b As Double = CDbl(bByte) / 255.0R

                        Dim maxValue As Double = Math.Max(r, Math.Max(g, b))
                        Dim minValue As Double = Math.Min(r, Math.Min(g, b))
                        Dim diff As Double = maxValue - minValue

                        Dim hue As Double
                        Dim saturation As Double
                        Dim brightness As Double = maxValue

                        If maxValue <= EPSILON Then
                            saturation = 0.0R
                        Else
                            saturation = diff / maxValue
                        End If

                        If diff <= EPSILON Then
                            hue = 0.0R
                        ElseIf Math.Abs(maxValue - r) <= EPSILON Then
                            hue = 60.0R * ((g - b) / diff)
                        ElseIf Math.Abs(maxValue - g) <= EPSILON Then
                            hue = 60.0R * (((b - r) / diff) + 2.0R)
                        Else
                            hue = 60.0R * (((r - g) / diff) + 4.0R)
                        End If

                        hue = NormalizeHue(hue + dH)
                        saturation = Clamp01(saturation + dS)
                        brightness = Clamp01(brightness + dB)

                        Dim c As Double = brightness * saturation
                        Dim hPrime As Double = hue / 60.0R
                        Dim xx As Double = c * (1.0R - Math.Abs((hPrime Mod 2.0R) - 1.0R))
                        Dim m As Double = brightness - c

                        Dim r1 As Double
                        Dim g1 As Double
                        Dim b1 As Double

                        If hPrime < 1.0R Then
                            r1 = c
                            g1 = xx
                            b1 = 0.0R
                        ElseIf hPrime < 2.0R Then
                            r1 = xx
                            g1 = c
                            b1 = 0.0R
                        ElseIf hPrime < 3.0R Then
                            r1 = 0.0R
                            g1 = c
                            b1 = xx
                        ElseIf hPrime < 4.0R Then
                            r1 = 0.0R
                            g1 = xx
                            b1 = c
                        ElseIf hPrime < 5.0R Then
                            r1 = xx
                            g1 = 0.0R
                            b1 = c
                        Else
                            r1 = c
                            g1 = 0.0R
                            b1 = xx
                        End If

                        dstBuffer(dstIndex) =
                        CByte(ClampToByte(CInt(Math.Round((b1 + m) * 255.0R, MidpointRounding.AwayFromZero))))
                        dstBuffer(dstIndex + 1) =
                        CByte(ClampToByte(CInt(Math.Round((g1 + m) * 255.0R, MidpointRounding.AwayFromZero))))
                        dstBuffer(dstIndex + 2) =
                        CByte(ClampToByte(CInt(Math.Round((r1 + m) * 255.0R, MidpointRounding.AwayFromZero))))
                        If aByte > 0 Then
                            dstBuffer(dstIndex + 3) = CByte(alpha)
                        End If
                    End If

                Next
            Next

            Marshal.Copy(dstBuffer, 0, dstData.Scan0, dstBytes)

        Finally
            If srcData IsNot Nothing Then
                src32.UnlockBits(srcData)
            End If

            If dstData IsNot Nothing Then
                result.UnlockBits(dstData)
            End If

            If srcIsClone Then
                src32.Dispose()
            End If

            If disposeBmpSrc Then
                bmpSrc.Dispose()
            End If
        End Try

        Return result

    End Function
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

End Module