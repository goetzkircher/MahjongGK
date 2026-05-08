Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On
Imports System.Drawing.Drawing2D
Imports System.Drawing.Imaging

Namespace Images
    Public Enum UndoRedoSymbol
        Undo = 0
        Redo = 1
    End Enum

    Public Module UndoRedoBitmapFactory

        '
        ''' <summary>
        ''' Erzeugt ein Undo- oder Redo-Symbol als Bitmap.
        ''' Transparent im Hintergrund, 32x32 standardmäßig.
        ''' Die Schattierung wird aus der Basisfarbe automatisch abgeleitet.
        ''' </summary>
        Public Function CreateUndoRedoBitmap(ByVal symbol As UndoRedoSymbol,
                                            ByVal baseColor As Color,
                                            Optional ByVal size As Integer = 32) As Bitmap

            If size < 16 Then
                Throw New ArgumentOutOfRangeException(NameOf(size), "Die Symbolgröße sollte mindestens 16 Pixel betragen.")
            End If

            Dim bmp As Bitmap = New Bitmap(size, size, PixelFormat.Format32bppPArgb)

            Using g As Graphics = Graphics.FromImage(bmp)

                g.SmoothingMode = SmoothingMode.AntiAlias
                g.InterpolationMode = InterpolationMode.HighQualityBicubic
                g.PixelOffsetMode = PixelOffsetMode.HighQuality
                g.CompositingQuality = CompositingQuality.HighQuality
                g.Clear(Color.Transparent)

                If symbol = UndoRedoSymbol.Redo Then
                    g.TranslateTransform(CSng(size), 0.0F)
                    g.ScaleTransform(-1.0F, 1.0F)
                End If

                DrawUndoSymbol(g, size, baseColor)

                g.ResetTransform()

            End Using

            Return bmp

        End Function

        '
        ''' <summary>
        ''' Zeichnet das eigentliche Undo-Symbol.
        ''' Redo entsteht durch horizontales Spiegeln.
        ''' </summary>
        Private Sub DrawUndoSymbol(ByVal g As Graphics,
                              ByVal size As Integer,
                              ByVal baseColor As Color)

            Dim darkColor As Color = baseColor 'BlendWith(baseColor, Color.Black, 0.34F)
            Dim lightColor As Color = baseColor 'BlendWith(baseColor, Color.White, 0.18F)

            Dim lineWidthOuter As Single = Math.Max(3.6F, CSng(size) * 0.115F)
            Dim lineWidthInner As Single = Math.Max(2.2F, CSng(size) * 0.075F)

            Dim shaftPath As GraphicsPath = BuildUndoShaftPath(size)
            Dim headPath As GraphicsPath = BuildUndoHeadPath(size)

            Using penOuter As Pen = New Pen(darkColor, lineWidthOuter),
              penInner As Pen = New Pen(lightColor, lineWidthInner),
              brushHeadOuter As SolidBrush = New SolidBrush(darkColor),
              brushHeadInner As SolidBrush = New SolidBrush(lightColor)

                penOuter.StartCap = LineCap.Round
                penOuter.EndCap = LineCap.Round
                penOuter.LineJoin = LineJoin.Round

                penInner.StartCap = LineCap.Round
                penInner.EndCap = LineCap.Round
                penInner.LineJoin = LineJoin.Round

                ' Schatten-/Außenkontur
                g.DrawPath(penOuter, shaftPath)
                g.FillPath(brushHeadOuter, headPath)

                ' Heller Kern
                g.DrawPath(penInner, shaftPath)

                Dim innerHeadPath As GraphicsPath = BuildUndoHeadPathInner(size)
                Using innerHeadPath
                    g.FillPath(brushHeadInner, innerHeadPath)
                End Using

            End Using

            shaftPath.Dispose()
            headPath.Dispose()

        End Sub

        '
        ''' <summary>
        ''' Bogen/Schaft des Undo-Pfeils.
        ''' </summary>
        Private Function BuildUndoShaftPath(ByVal size As Integer) As GraphicsPath

            Dim s As Single = CSng(size)

            Dim p0 As PointF = New PointF(s * 0.79F, s * 0.72F)
            Dim c1 As PointF = New PointF(s * 0.78F, s * 0.36F)
            Dim c2 As PointF = New PointF(s * 0.53F, s * 0.27F)
            Dim p1 As PointF = New PointF(s * 0.31F, s * 0.42F)

            Dim gp As GraphicsPath = New GraphicsPath()
            gp.AddBezier(p0, c1, c2, p1)

            Return gp

        End Function

        '
        ''' <summary>
        ''' Äußere Pfeilspitze.
        ''' </summary>
        Private Function BuildUndoHeadPath(ByVal size As Integer) As GraphicsPath

            Dim s As Single = CSng(size)

            Dim tip As PointF = New PointF(s * 0.11F, s * 0.42F)
            Dim upper As PointF = New PointF(s * 0.34F, s * 0.23F)
            Dim inner As PointF = New PointF(s * 0.28F, s * 0.42F)
            Dim lower As PointF = New PointF(s * 0.34F, s * 0.61F)

            Dim gp As GraphicsPath = New GraphicsPath()
            gp.AddPolygon(New PointF() {tip, upper, inner, lower})

            Return gp

        End Function

        '
        ''' <summary>
        ''' Innere, etwas kleinere Pfeilspitze für den helleren Kern.
        ''' </summary>
        Private Function BuildUndoHeadPathInner(ByVal size As Integer) As GraphicsPath

            Dim s As Single = CSng(size)

            Dim tip As PointF = New PointF(s * 0.15F, s * 0.42F)
            Dim upper As PointF = New PointF(s * 0.31F, s * 0.28F)
            Dim inner As PointF = New PointF(s * 0.27F, s * 0.42F)
            Dim lower As PointF = New PointF(s * 0.31F, s * 0.56F)

            Dim gp As GraphicsPath = New GraphicsPath()
            gp.AddPolygon(New PointF() {tip, upper, inner, lower})

            Return gp

        End Function

        '
        ''' <summary>
        ''' Mischt zwei Farben.
        ''' amount = 0 -> colorA
        ''' amount = 1 -> colorB
        ''' </summary>
        Private Function BlendWith(ByVal colorA As Color,
                          ByVal colorB As Color,
                          ByVal amount As Single) As Color

            If amount <= 0.0F Then Return colorA
            If amount >= 1.0F Then Return colorB

            Dim r As Integer = CInt(CSng(colorA.R) + (CSng(colorB.R) - CSng(colorA.R)) * amount)
            Dim g As Integer = CInt(CSng(colorA.G) + (CSng(colorB.G) - CSng(colorA.G)) * amount)
            Dim b As Integer = CInt(CSng(colorA.B) + (CSng(colorB.B) - CSng(colorA.B)) * amount)

            Return Color.FromArgb(255, r, g, b)

        End Function

        Private Function ClampByte(ByVal value As Integer) As Integer
            Return Math.Max(0, Math.Min(255, value))
        End Function

    End Module
End Namespace