Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On
Imports System.Drawing.Imaging
Imports System.Runtime.InteropServices

'
''' <summary>
''' Ermittelt aus einer Hintergrund-Bitmap im Bereich der Buttons eine mittlere Hintergrundfarbe
''' und leitet daraus Overlay-Farben für Normal, MouseOver, MouseDown und Selected ab.
'''
''' Die Klasse besitzt die übergebene Bitmap nicht, sondern wertet sie nur aus.
''' </summary>
Public NotInheritable Class OverlayColorPalette

    Private ReadOnly _averageBackgroundColor As Color
    Private ReadOnly _sampleRect As Rectangle

    Private ReadOnly _colorNormal As Color
    Private ReadOnly _colorMouseOver As Color
    Private ReadOnly _colorMouseDown As Color
    Private ReadOnly _colorSelected As Color

    '
    ''' <summary>
    ''' Gemittelte Hintergrundfarbe im ausgewerteten Bereich.
    ''' </summary>
    Public ReadOnly Property AverageBackgroundColor As Color
        Get
            Return _averageBackgroundColor
        End Get
    End Property

    '
    ''' <summary>
    ''' Alias zu AverageBackgroundColor.
    ''' </summary>
    Public ReadOnly Property DominantBackgroundColor As Color
        Get
            Return _averageBackgroundColor
        End Get
    End Property

    '
    ''' <summary>
    ''' Ausgewerteter Bereich innerhalb der Bitmap.
    ''' </summary>
    Public ReadOnly Property SampleRect As Rectangle
        Get
            Return _sampleRect
        End Get
    End Property

    '
    ''' <summary>
    ''' Grundfarbe für das Overlay.
    ''' </summary>
    Public ReadOnly Property ColorNormal As Color
        Get
            Return _colorNormal
        End Get
    End Property

    '
    ''' <summary>
    ''' Farbe für MouseOver.
    ''' </summary>
    Public ReadOnly Property ColorMouseOver As Color
        Get
            Return _colorMouseOver
        End Get
    End Property

    '
    ''' <summary>
    ''' Farbe für MouseDown.
    ''' </summary>
    Public ReadOnly Property ColorMouseDown As Color
        Get
            Return _colorMouseDown
        End Get
    End Property

    '
    ''' <summary>
    ''' Farbe für Selected.
    ''' </summary>
    Public ReadOnly Property ColorSelected As Color
        Get
            Return _colorSelected
        End Get
    End Property

    '
    ''' <summary>
    ''' Leerer Konstruktor. Erzeugt eine neutrale Standardpalette.
    ''' </summary>
    Public Sub New()

        Dim bg As Color = Color.FromArgb(255, 128, 128, 128)

        _averageBackgroundColor = bg
        _sampleRect = Rectangle.Empty

        BuildPalette(bg, _colorNormal, _colorMouseOver, _colorMouseDown, _colorSelected)

    End Sub

    '
    ''' <summary>
    ''' Konstruktor mit Hintergrund-Bitmap und Auswertebereich der Buttons.
    ''' </summary>
    Public Sub New(ByVal bmpUGrd As Bitmap, ByVal rectButtons As Rectangle)

        Dim avg As Color = AverageColorFromBitmap(bmpUGrd, rectButtons)

        _averageBackgroundColor = avg
        _sampleRect = rectButtons

        BuildPalette(avg, _colorNormal, _colorMouseOver, _colorMouseDown, _colorSelected)

    End Sub

    '
    ''' <summary>
    ''' Konstruktor mit direkter Vorgabe einer Hintergrundfarbe.
    ''' </summary>
    Public Sub New(ByVal backgroundColor As Color)

        Dim bg As Color = Color.FromArgb(255, backgroundColor.R, backgroundColor.G, backgroundColor.B)

        _averageBackgroundColor = bg
        _sampleRect = Rectangle.Empty

        BuildPalette(bg, _colorNormal, _colorMouseOver, _colorMouseDown, _colorSelected)

    End Sub

    '
    ''' <summary>
    ''' Bestimmt die mittlere Farbe aus einer Bitmap im angegebenen Rechteck.
    ''' Transparente Pixel werden entsprechend ihrer Alpha-Deckung gewichtet.
    ''' </summary>
    Public Shared Function AverageColorFromBitmap(ByVal bmpSrc As Bitmap,
                                                  ByVal sampleRect As Rectangle) As Color

        If bmpSrc Is Nothing Then
            Return Color.Black
        End If

        If bmpSrc.Width <= 0 OrElse bmpSrc.Height <= 0 Then
            Return Color.Black
        End If

        Dim imgRect As Rectangle = New Rectangle(0, 0, bmpSrc.Width, bmpSrc.Height)
        Dim rect As Rectangle = Rectangle.Intersect(imgRect, sampleRect)

        If rect.Width <= 0 OrElse rect.Height <= 0 Then
            Return Color.Black
        End If

        Dim bmp As Bitmap = bmpSrc

        If bmp.PixelFormat <> PixelFormat.Format32bppArgb Then
            bmp = New Bitmap(bmpSrc.Width, bmpSrc.Height, PixelFormat.Format32bppArgb)

            Using g As Graphics = Graphics.FromImage(bmp)
                g.DrawImage(bmpSrc, 0, 0, bmpSrc.Width, bmpSrc.Height)
            End Using
        End If

        Dim data As BitmapData = bmp.LockBits(imgRect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb)

        Try
            Dim stride As Integer = data.Stride
            Dim absStride As Integer = Math.Abs(stride)

            Dim raw(absStride * bmp.Height - 1) As Byte
            Marshal.Copy(data.Scan0, raw, 0, raw.Length)

            Dim sumR As Double = 0.0
            Dim sumG As Double = 0.0
            Dim sumB As Double = 0.0
            Dim sumW As Double = 0.0

            Dim stepXY As Integer = 1

            If rect.Width * rect.Height > 6000 Then
                stepXY = 2
            End If

            If rect.Width * rect.Height > 24000 Then
                stepXY = 3
            End If

            Dim y As Integer
            For y = rect.Top To rect.Bottom - 1 Step stepXY

                Dim row As Integer
                If stride >= 0 Then
                    row = y * stride
                Else
                    row = (bmp.Height - 1 - y) * absStride
                End If

                Dim x As Integer
                For x = rect.Left To rect.Right - 1 Step stepXY

                    Dim p As Integer = row + x * 4

                    Dim bb As Byte = raw(p + 0)
                    Dim gg As Byte = raw(p + 1)
                    Dim rr As Byte = raw(p + 2)
                    Dim aa As Byte = raw(p + 3)

                    If aa = 0 Then
                        Continue For
                    End If

                    Dim w As Double = CDbl(aa) / 255.0

                    sumR += CDbl(rr) * w
                    sumG += CDbl(gg) * w
                    sumB += CDbl(bb) * w
                    sumW += w

                Next
            Next

            If sumW <= 0.0 Then
                Return Color.Black
            End If

            Dim r As Integer = CInt(Math.Round(sumR / sumW))
            Dim g As Integer = CInt(Math.Round(sumG / sumW))
            Dim b As Integer = CInt(Math.Round(sumB / sumW))

            Return Color.FromArgb(255, ClampByte(r), ClampByte(g), ClampByte(b))

        Finally
            bmp.UnlockBits(data)

            If Not Object.ReferenceEquals(bmp, bmpSrc) Then
                bmp.Dispose()
            End If
        End Try

    End Function

    '
    ''' <summary>
    ''' Baut die Palette ausschließlich über den mittleren Grauwert und konstante RGB-Deltas auf.
    ''' </summary>
    Private Shared Sub BuildPalette(ByVal bg As Color,
                                    ByRef colorNormal As Color,
                                    ByRef colorMouseOver As Color,
                                    ByRef colorMouseDown As Color,
                                    ByRef colorSelected As Color)

        Dim gray As Integer = (CInt(bg.R) + CInt(bg.G) + CInt(bg.B)) \ 3
        '
        '                                   'Original: Sanfter:
        Dim dNormal As Integer = 110        '110        90
        Dim dMouseOver As Integer = 150     '150        130
        Dim dMouseDown As Integer = 70      '70         55
        Dim dSelected As Integer = 190      '190        170

        If gray < 128 Then 'Original 128, Testen mit 140
            colorNormal = ShiftRgb(bg, dNormal)
            colorMouseOver = ShiftRgb(bg, dMouseOver)
            colorMouseDown = ShiftRgb(bg, dMouseDown)
            colorSelected = ShiftRgb(bg, dSelected)
        Else
            colorNormal = ShiftRgb(bg, -dNormal)
            colorMouseOver = ShiftRgb(bg, -dMouseOver)
            colorMouseDown = ShiftRgb(bg, -dMouseDown)
            colorSelected = ShiftRgb(bg, -dSelected)
        End If

    End Sub

    '
    ''' <summary>
    ''' Verschiebt alle drei RGB-Kanäle um denselben Betrag.
    ''' Dadurch bleibt der Farbcharakter grob erhalten.
    ''' </summary>
    Private Shared Function ShiftRgb(ByVal c As Color, ByVal delta As Integer) As Color

        Dim r As Integer = ClampByte(CInt(c.R) + delta)
        Dim g As Integer = ClampByte(CInt(c.G) + delta)
        Dim b As Integer = ClampByte(CInt(c.B) + delta)

        Return Color.FromArgb(255, r, g, b)

    End Function

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