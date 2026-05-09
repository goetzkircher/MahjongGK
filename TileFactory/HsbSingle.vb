Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

Imports System.Drawing

''' <summary>
''' Echter HSB/HSV-Farbwert:
''' H = 0..359
''' S = 0..1
''' B = 0..1
''' </summary>
Public Class HsbSingle
    '
    'Die drei Farbwert-Properties sind Readonly um sie vor
    'versehentlichen Zuweisungen zu schützen.
    Public Sub New()

    End Sub

    Public Sub New(h As Single, s As Single, b As Single)
        Me._h = h
        Me._s = s
        Me._b = b
    End Sub

    Public Sub New(src As HsbInteger)
        If src Is Nothing Then Throw New ArgumentNullException(NameOf(src))

        Me._h = NormalizeHue(src.H)
        Me._s = Clamp01(CSng(src.S) / 100.0F)
        Me._b = Clamp01(CSng(src.B) / 100.0F)
    End Sub

    Private _h As Single
    Private _s As Single
    Private _b As Single

    Public ReadOnly Property H As Single
        Get
            Return NormalizeHue(_h)
        End Get
    End Property
    Public ReadOnly Property S As Single
        Get
            Return Clamp01(_s)
        End Get
    End Property
    Public ReadOnly Property B As Single
        Get
            Return Clamp01(_b)
        End Get
    End Property

    Public Sub AddH(h As Single)
        _h += h
    End Sub
    Public Sub AddS(s As Single)
        _h += s
    End Sub
    Public Sub AddB(b As Single)
        _h += b
    End Sub
    Public Sub SetH(h As Single)
        _h = h
    End Sub
    Public Sub SetS(s As Single)
        _h = s
    End Sub
    Public Sub SetB(b As Single)
        _h = b
    End Sub
    Public Function GetColor() As Color

        Dim hue As Double = NormalizeHue(Me.H)
        Dim sat As Double = Clamp01(Me.S)
        Dim bri As Double = Clamp01(Me.B)

        If sat <= 0.0 Then
            Dim gray As Integer = ClampByte(CInt(Math.Round(bri * 255.0)))
            Return Color.FromArgb(gray, gray, gray)
        End If

        Dim hSection As Double = hue / 60.0
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
            ClampByte(CInt(Math.Round(r * 255.0))),
            ClampByte(CInt(Math.Round(g * 255.0))),
            ClampByte(CInt(Math.Round(bl * 255.0))))
    End Function

    Public Sub SetFromColor(col As Color)
        Dim r As Double = CDbl(col.R) / 255.0
        Dim g As Double = CDbl(col.G) / 255.0
        Dim bl As Double = CDbl(col.B) / 255.0

        Dim maxValue As Double = Math.Max(r, Math.Max(g, bl))
        Dim minValue As Double = Math.Min(r, Math.Min(g, bl))
        Dim delta As Double = maxValue - minValue

        Dim hue As Double
        Dim sat As Double
        Dim bri As Double = maxValue

        If delta <= 0.0 Then
            hue = 0.0
        ElseIf maxValue = r Then
            hue = 60.0 * ((g - bl) / delta)
        ElseIf maxValue = g Then
            hue = 60.0 * (((bl - r) / delta) + 2.0)
        Else
            hue = 60.0 * (((r - g) / delta) + 4.0)
        End If

        If hue < 0.0 Then
            hue += 360.0
        End If

        If maxValue <= 0.0 Then
            sat = 0.0
        Else
            sat = delta / maxValue
        End If

        Me._h = CSng(NormalizeHue(CSng(hue)))
        Me._s = CSng(Clamp01(CSng(sat)))
        Me._b = CSng(Clamp01(CSng(bri)))
    End Sub

    Private Shared Function NormalizeHue(value As Single) As Single
        Dim result As Single = CSng(value Mod 360.0)
        If result < 0.0 Then result += 360.0F
        If result >= 360.0 Then result -= 360.0F
        Return result
    End Function

    Private Shared Function ClampByte(value As Integer) As Integer
        If value < 0 Then Return 0
        If value > 255 Then Return 255
        Return value
    End Function
    Private Shared Function NormalizeHue(value As Integer) As Single
        Dim result As Integer = value Mod 360
        If result < 0 Then result += 360
        Return CSng(result)
    End Function

    Private Shared Function Clamp01(value As Single) As Single
        If value < 0.0F Then Return 0.0F
        If value > 1.0F Then Return 1.0F
        Return value
    End Function
End Class
