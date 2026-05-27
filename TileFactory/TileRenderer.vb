Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports TileFactory.TileColors

'Kein NameSpace. Es wird der StammSpaceName TileTactory verwendet.
'
''' <summary>
''' Rendert den neutralen Basiskörper eines Mahjong-Steines.
''' Prinzip:
''' - eine Grundform mit Face-Farbe und Rand in Tieffarbe
''' - diese Grundform mehrfach versetzt übereinander zeichnen
''' - darauf die Frontplatte setzen
''' </summary>
Friend Module TileRenderer

    '
    ''' <summary>
    ''' Rendert den neutralen Grundstein.
    ''' </summary>
    Public Sub RenderTileBase(g As Graphics,
                                     layout As TileLayout,
                                     colors As TileColors)

        RenderDepthStack(g, layout, colors)

    End Sub

    Public Function CreateBaseTileBitmap(request As TileRequest,
                                                layout As TileLayout,
                                                colors As TileColors) As Bitmap

        Dim bmpLayer As New Bitmap(layout.SteinRect.Width, layout.SteinRect.Height, Imaging.PixelFormat.Format32bppArgb)

        Using g As Graphics = Graphics.FromImage(bmpLayer)
            SetGraphicsMode(g, sourceOver:=True)
            g.Clear(Color.Transparent)
            RenderTileBase(g, layout, colors)
        End Using
        'TODO Wolken und Körnung eintragen

        Dim tex As FaceTexturValues = colors.GetLayerTexturValues

        If tex.outerCloudStrength = 0 AndAlso tex.outerGrainStrength = 0 Then
            Return bmpLayer
        Else
            Dim texturMap As Single(,) = CreateMaterialTextureMap(bmpLayer, tex, layout.FaktorBasisHeightToAktHeight)
            TileTextur.InsertTexturMap(bmpLayer, texturMap)
            Return bmpLayer
        End If

    End Function

    '
    ''' <summary>
    ''' Zeichnet die Tiefenlagen von hinten nach vorne.
    ''' </summary>
    Private Sub RenderDepthStack(g As Graphics,
                                       layout As TileLayout,
                                       colors As TileColors)

        Dim dx As Integer = layout.RightLayerDepth
        Dim dy As Integer = layout.BottomLayerDepth

        With colors
            Select Case layout.DepthLayerCount
                Case 1
                    RenderDepthBaseShape(g, layout, dx, dy, dx, dy, .GetColLayerLine, .GetColLayerMidUp, True)

                Case 2
                    RenderDepthBaseShape(g, layout, dx * 2, dy * 2, dx, dy, .GetColLayerLine, .GetColLayerDn, True)
                    RenderDepthBaseShape(g, layout, dx, dy, dx, dy, .GetColLayerLine, .GetColLayerMidUp, False)

                Case Else
                    RenderDepthBaseShape(g, layout, dx * 3, dy * 3, dx, dy, .GetColLayerLine, .GetColLayerDn, True)
                    RenderDepthBaseShape(g, layout, dx * 2, dy * 2, dx, dy, .GetColLayerLine, .GetColLayerMidDn, False)
                    RenderDepthBaseShape(g, layout, dx, dy, dx, dy, .GetColLayerLine, .GetColLayerMidUp, False)
            End Select
        End With
    End Sub

    '
    ''' <summary>
    ''' Zeichnet genau eine Tiefenlage.
    ''' Die Form besteht aus:
    ''' - Hauptfläche in faceColor
    ''' - rechts und unten Rand in layerColor
    ''' </summary>
    Private Sub RenderDepthBaseShape(g As Graphics,
                                    layout As TileLayout,
                                    offsetX As Integer,
                                    offsetY As Integer,
                                    layerDx As Integer,
                                    layerDy As Integer,
                                    layerLineColor As Color,
                                    layerColor As Color,
                                    isOutermostLayer As Boolean)

        Dim rcOuter As New Rectangle(
        layout.FaceOuterRect.Left + offsetX,
        layout.FaceOuterRect.Top + offsetY,
        layout.FaceOuterRect.Width,
        layout.FaceOuterRect.Height)

        If rcOuter.Width <= 0 OrElse rcOuter.Height <= 0 Then
            Return
        End If

        Dim rcInner As New Rectangle(
        rcOuter.Left - layerDx,
        rcOuter.Top - layerDy,
        rcOuter.Width,
        rcOuter.Height)

        Dim depthOutlineWidth As Single = layout.DepthOutlineWidth

        Dim rcOuterStroke As Rectangle = rcOuter
        If isOutermostLayer AndAlso depthOutlineWidth >= 2.0F Then
            rcOuterStroke = GetInsetRectangle(rcOuter, 1)
        End If

        Using gpOuterFill As GraphicsPath = CreateRoundedRectPath(rcOuter, layout.FaceOuterCornerRadius),
              gpOuterStroke As GraphicsPath = CreateRoundedRectPath(rcOuterStroke, layout.FaceOuterCornerRadius),
              brOuter As New SolidBrush(layerColor),
              pnOuter As New Pen(layerLineColor, depthOutlineWidth)

            g.FillPath(brOuter, gpOuterFill)
            g.DrawPath(pnOuter, gpOuterStroke)
        End Using

        'Using gpInner As GraphicsPath = CreateRoundedRectPath(rcInner, layout.FaceOuterCornerRadius),
        '  brInner As New SolidBrush(faceColor),
        '  pnInner As New Pen(layerLineColor, depthOutlineWidth)

        '    g.FillPath(brInner, gpInner)
        '    g.DrawPath(pnInner, gpInner)

        'End Using

    End Sub

    '

    '
    ''' <summary>
    ''' Erzeugt ein abgerundetes Rechteck.
    ''' </summary>
    Private Function CreateRoundedRectPath(rect As Rectangle, radius As Integer) As GraphicsPath

        Dim gp As New GraphicsPath()

        If rect.Width <= 0 OrElse rect.Height <= 0 Then
            Return gp
        End If

        If radius <= 0 Then
            gp.AddRectangle(rect)
            gp.CloseFigure()
            Return gp
        End If

        Dim d As Integer = radius * 4
        If d > rect.Width Then d = rect.Width
        If d > rect.Height Then d = rect.Height

        If d < 2 Then
            gp.AddRectangle(rect)
            gp.CloseFigure()
            Return gp
        End If

        Dim arc As New Rectangle(rect.X, rect.Y, d, d)

        gp.AddArc(arc, 180, 90)
        arc.X = rect.Right - d
        gp.AddArc(arc, 270, 90)
        arc.Y = rect.Bottom - d
        gp.AddArc(arc, 0, 90)
        arc.X = rect.X
        gp.AddArc(arc, 90, 90)

        gp.CloseFigure()
        Return gp

    End Function

    '
    ''' <summary>
    ''' Mischt zwei Farben.
    ''' </summary>
    Private Function Blend(c1 As Color, c2 As Color, ratio As Double) As Color

        If ratio < 0.0R Then ratio = 0.0R
        If ratio > 1.0R Then ratio = 1.0R

        Dim a As Integer = CInt(Math.Round(CInt(c1.A) + ((CInt(c2.A) - CInt(c1.A)) * ratio)))
        Dim r As Integer = CInt(Math.Round(CInt(c1.R) + ((CInt(c2.R) - CInt(c1.R)) * ratio)))
        Dim g As Integer = CInt(Math.Round(CInt(c1.G) + ((CInt(c2.G) - CInt(c1.G)) * ratio)))
        Dim b As Integer = CInt(Math.Round(CInt(c1.B) + ((CInt(c2.B) - CInt(c1.B)) * ratio)))

        a = ClampByteValue(a)
        r = ClampByteValue(r)
        g = ClampByteValue(g)
        b = ClampByteValue(b)

        Return Color.FromArgb(a, r, g, b)

    End Function

    '
    ''' <summary>
    ''' Hellt eine Farbe auf.
    ''' </summary>
    Private Function Lighten(source As Color, factor As Double) As Color

        If factor < 0.0R Then factor = 0.0R
        If factor > 1.0R Then factor = 1.0R

        Dim r As Integer = CInt(Math.Round(CInt(source.R) + ((255 - CInt(source.R)) * factor)))
        Dim g As Integer = CInt(Math.Round(CInt(source.G) + ((255 - CInt(source.G)) * factor)))
        Dim b As Integer = CInt(Math.Round(CInt(source.B) + ((255 - CInt(source.B)) * factor)))

        Return Color.FromArgb(source.A, ClampByteValue(r), ClampByteValue(g), ClampByteValue(b))

    End Function

    '
    ''' <summary>
    ''' Dunkelt eine Farbe ab.
    ''' </summary>
    Private Function Darken(source As Color, factor As Double) As Color

        If factor < 0.0R Then factor = 0.0R
        If factor > 1.0R Then factor = 1.0R

        Dim r As Integer = CInt(Math.Round(CInt(source.R) * (1.0R - factor)))
        Dim g As Integer = CInt(Math.Round(CInt(source.G) * (1.0R - factor)))
        Dim b As Integer = CInt(Math.Round(CInt(source.B) * (1.0R - factor)))

        Return Color.FromArgb(source.A, ClampByteValue(r), ClampByteValue(g), ClampByteValue(b))

    End Function

    '
    ''' <summary>
    ''' Begrenzt auf 0..255.
    ''' </summary>
    Private Function ClampByteValue(value As Integer) As Integer

        If value < 0 Then
            Return 0
        ElseIf value > 255 Then
            Return 255
        Else
            Return value
        End If

    End Function

    Private Function GetInsetRectangle(source As Rectangle, inset As Integer) As Rectangle

        If inset <= 0 Then
            Return source
        End If

        Dim left As Integer = source.Left + inset
        Dim top As Integer = source.Top + inset
        Dim width As Integer = source.Width - (2 * inset)
        Dim height As Integer = source.Height - (2 * inset)

        If width < 1 Then width = 1
        If height < 1 Then height = 1

        Return New Rectangle(left, top, width, height)

    End Function
End Module