Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

Imports System.Drawing
Imports MahjongGK.Contracts
Imports MahjongGK.Contracts.GlobalEnum

'
''' <summary>
''' Container für die drei Hsb-Werte im Integerformat
''' H = 0..359
''' S = 0..100
''' B = 0..100
''' </summary>
Public Class HsbInteger
    '

    '
    'Die drei Farbwert-Properties sind Readonly um sie vor
    'versehentlichen Zuweisungen zu schützen.

    Public Sub New()

    End Sub

    Public Sub New(h As Integer, s As Integer, b As Integer, Optional normaliszeTo361 As Boolean = False)

        Me._h = NormalizeHue(h, normaliszeTo361)
        Me._s = ClampPercent(s)
        Me._b = ClampPercent(b)
    End Sub

    Public Sub New(src As HsbSingle)

        If src Is Nothing Then Throw New ArgumentNullException(NameOf(src))

        Me._h = NormalizeHue(CInt(Math.Round(src.H)), normaliszeTo361:=False)
        Me._s = ClampPercent(CInt(Math.Round(src.S * 100.0F)))
        Me._b = ClampPercent(CInt(Math.Round(src.B * 100.0F)))
    End Sub

    Private _h As Integer
    Private _s As Integer
    Private _b As Integer
    '
    ''' <summary>
    ''' Gibt den normalisierten Hue zurück.
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property H As Integer
        Get
            Return NormalizeHue(_h, normaliszeTo361:=False)
        End Get
    End Property
    '
    ''' <summary>
    ''' Gibt die geclampte Sättigung zurück.
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property S As Integer
        Get
            Return ClampPercent(_s)
        End Get
    End Property
    '
    ''' <summary>
    ''' Gibt die geklampte Helligkeit zurück.
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property B As Integer
        Get
            Return ClampPercent(_b)
        End Get
    End Property

    Public Sub AddH(h As Integer)
        _h += NormalizeHue(h, normaliszeTo361:=True)
    End Sub
    Public Sub AddS(s As Integer)
        _h += ClampPercent(s)
    End Sub
    Public Sub AddB(b As Integer)
        _h += ClampPercent(b)
    End Sub
    Public Sub SetH(h As Integer)
        _h = NormalizeHue(h, normaliszeTo361:=True)
    End Sub
    Public Sub SetS(s As Integer)
        _h = ClampPercent(s)
    End Sub
    Public Sub SetB(b As Integer)
        _h = ClampPercent(b)
    End Sub

    Public Function DeepCopy() As HsbInteger
        Return New HsbInteger(H, S, B)
    End Function

    Public Function ToColor() As Color
        Return ToColor(_h, _s, _b)
    End Function
    '
    ''' <summary>
    ''' Wie ToColor, nur der Wert hue = 360 wird als Schwarz, Weiß oder Grauwert interpretiert.
    ''' </summary>
    ''' <param name="hueBlack"></param>
    ''' <returns></returns>
    Public Function ToColorB360(Optional hueBlack As Hue360Mode = Hue360Mode.Hue0) As Color

        If _h < 360 Then
            Return ToColor(_h, _s, _b)
        End If

        Dim gray As Integer

        Select Case hueBlack

            Case Hue360Mode.Hue0
                Return ToColor(0, _s, _b)

            Case Hue360Mode.Black360White361
                If _h = 360 Then
                    Return Color.Black
                Else
                    Return Color.White
                End If

            Case Hue360Mode.Black
                Return Color.Black

            Case Hue360Mode.White
                Return Color.White

            Case Hue360Mode.Grey
                gray = (_b * 255 + 50) \ 100

            Case Hue360Mode.GreyReverse
                gray = 255 - ((_b * 255 + 50) \ 100)
            Case Else
                Throw New ArgumentOutOfRangeException(NameOf(hueBlack))
        End Select

        gray = Math.Max(0, Math.Min(255, gray))

        Return Color.FromArgb(255, gray, gray, gray)

    End Function
    '

    Private Function ToColor(valueH As Integer, valueS As Integer, valueB As Integer) As Color

        Dim hue As Integer = valueH Mod 360
        If hue < 0 Then hue += 360

        Dim satPercent As Integer = Clamp(valueS, 0, 100)
        Dim briPercent As Integer = Clamp(valueB, 0, 100)

        Dim sat As Double = CDbl(satPercent) / 100.0
        Dim bri As Double = CDbl(briPercent) / 100.0

        If sat = 0.0 Then
            Dim gray As Integer = CInt(Math.Round(bri * 255.0))
            gray = Clamp(gray, 0, 255)
            Return Color.FromArgb(gray, gray, gray)
        End If

        Dim hSection As Double = CDbl(hue) / 60.0
        Dim sector As Integer = CInt(Math.Floor(hSection))
        Dim fraction As Double = hSection - Math.Floor(hSection)

        Dim p As Double = bri * (1.0 - sat)
        Dim q As Double = bri * (1.0 - sat * fraction)
        Dim t As Double = bri * (1.0 - sat * (1.0 - fraction))

        Dim r As Double
        Dim g As Double
        Dim bl As Double

        Select Case sector
            Case 0
                r = bri
                g = t
                bl = p
            Case 1
                r = q
                g = bri
                bl = p
            Case 2
                r = p
                g = bri
                bl = t
            Case 3
                r = p
                g = q
                bl = bri
            Case 4
                r = t
                g = p
                bl = bri
            Case Else
                r = bri
                g = p
                bl = q
        End Select

        Return Color.FromArgb(
            Clamp(CInt(Math.Round(r * 255.0)), 0, 255),
            Clamp(CInt(Math.Round(g * 255.0)), 0, 255),
            Clamp(CInt(Math.Round(bl * 255.0)), 0, 255))

    End Function

    Private Shared Function Clamp(value As Integer, minValue As Integer, maxValue As Integer) As Integer
        If value < minValue Then Return minValue
        If value > maxValue Then Return maxValue
        Return value
    End Function

    Public Sub SetFromColor(col As Color)

        Dim r As Double = CDbl(col.R) / 255.0
        Dim g As Double = CDbl(col.G) / 255.0
        Dim bl As Double = CDbl(col.B) / 255.0

        Dim maxValue As Double = Math.Max(r, Math.Max(g, bl))
        Dim minValue As Double = Math.Min(r, Math.Min(g, bl))
        Dim delta As Double = maxValue - minValue

        Dim hue As Double
        Dim saturation As Double
        Dim brightness As Double = maxValue

        If delta = 0.0 Then
            hue = 0.0
        ElseIf maxValue = r Then
            hue = 60.0 * (((g - bl) / delta) Mod 6.0)
        ElseIf maxValue = g Then
            hue = 60.0 * (((bl - r) / delta) + 2.0)
        Else
            hue = 60.0 * (((r - g) / delta) + 4.0)
        End If

        If hue < 0.0 Then hue += 360.0

        If maxValue = 0.0 Then
            saturation = 0.0
        Else
            saturation = delta / maxValue
        End If

        Me._h = CInt(Math.Round(hue)) Mod 360
        Me._s = Clamp(CInt(Math.Round(saturation * 100.0)), 0, 100)
        Me._b = Clamp(CInt(Math.Round(brightness * 100.0)), 0, 100)
    End Sub

    Public Shared Function NormalizeHue(value As Integer, normaliszeTo361 As Boolean) As Integer
        If normaliszeTo361 AndAlso value > 361 Then value = 361
        Dim modulo As Integer = If(normaliszeTo361, 362, 360)
        Dim result As Integer = value Mod modulo
        If result < 0 Then result += modulo
        Return result
    End Function

    Public Shared Function ClampPercent(value As Integer) As Integer
        If value < 0 Then Return 0
        If value > 100 Then Return 100
        Return value
    End Function

End Class