Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

Imports System.Drawing
Imports System.Drawing.Drawing2D

'
''' <summary>
''' Erzeugt Status-Overlays transparent über dem Stein.
''' </summary>
Friend NotInheritable Class TileOverlayFactory

    Private Sub New()
    End Sub

    Public Shared Function CreateFrameOverlayBitmap(request As TileRequest,
                                                     layout As TileLayout,
                                                     colors As TileColors) As Bitmap

        If request Is Nothing Then Throw New ArgumentNullException(NameOf(request))
        If layout Is Nothing Then Throw New ArgumentNullException(NameOf(layout))

        Dim bmp As New Bitmap(layout.SteinRect.Width, layout.SteinRect.Height, Imaging.PixelFormat.Format32bppArgb)

        Using g As Graphics = Graphics.FromImage(bmp)

            g.Clear(Color.Transparent)
            SetGraphicsMode(g, sourceOver:=True)

            RenderFrameOverlayLightMapColor1(g, request, layout, colors)

        End Using

        Return bmp

    End Function

    Private Shared Sub RenderFrameOverlayLightMapColor1(g As Graphics,
                                                request As TileRequest,
                                                layout As TileLayout,
                                                colors As TileColors)

        Dim overlayColor As Color = colors.GetColFaceFrame

        Dim rc As Rectangle = layout.FaceFrameRect
        '  rc.Inflate(-2, -2)

        Dim diameter As Integer = layout.FaceFrameCornerRadius * 2
        If colors.GetFaktorFaceFrameRadius <> 0 Then
            Dim addX As Integer = CInt(100 * layout.FaktorBasisWidthToAktWidth * colors.GetFaktorFaceFrameRadius)
            diameter += addX
            ' Debug.Print($"diameter {diameter} addX{addX} Faktor {colors.FaktorFaceFrameRadius}")

        End If
        Using gp As GraphicsPath = CreateRoundedRectPath(rc, Math.Max(2, diameter)),
              pn As New Pen(Color.FromArgb(220, overlayColor), Math.Max(2, layout.FaceFrameThickness))

            g.DrawPath(pn, gp)

        End Using

    End Sub
    '
    ''' <summary>
    ''' Abgerundetes Rechteck.
    ''' </summary>
    Private Shared Function CreateRoundedRectPath(rect As Rectangle, diameter As Integer) As GraphicsPath

        Dim gp As New GraphicsPath()

        If rect.Width <= 0 OrElse rect.Height <= 0 Then
            Return gp
        End If

        If diameter <= 0 Then
            gp.AddRectangle(rect)
            gp.CloseFigure()
            Return gp
        End If

        If diameter > rect.Width Then diameter = rect.Width
        If diameter > rect.Height Then diameter = rect.Height

        If diameter < 1 Then
            gp.AddRectangle(rect)
            gp.CloseFigure()
            Return gp
        End If

        Dim arc As New Rectangle(rect.X, rect.Y, diameter, diameter)

        gp.AddArc(arc, 180, 90)
        arc.X = rect.Right - diameter
        gp.AddArc(arc, 270, 90)
        arc.Y = rect.Bottom - diameter
        gp.AddArc(arc, 0, 90)
        arc.X = rect.X
        gp.AddArc(arc, 90, 90)

        gp.CloseFigure()
        Return gp

    End Function

End Class
