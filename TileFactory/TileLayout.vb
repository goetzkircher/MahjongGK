Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

Imports System.Drawing

Public NotInheritable Class TileLayout

    '
    ' Relative Größe des Symbol-Rechtecks bezogen auf FaceInnerRect.
    ' 1.00 = maximal, kleinere Werte erzeugen größere Ränder.
    '
    Private Const SYMBOLRECT_SCALE As Double = 1 '0.78R
    Friend Const SYMBOLCONTENT_PADDING_SCALE As Double = 0.02R
    Private Const SYMBOLRECT_ZENTRIERVERSCHIEBUNG As Double = 0.02R
    '
    ' Schwellwerte für die Anzahl der Tiefenlagen.
    ' Bezug ist immer die kleinere Steinseite.
    '
    Private Const DEPTHLAYERS_THRESHOLD_1 As Integer = 60  'darunter nur noch 2 Lagen
    Private Const DEPTHLAYERS_THRESHOLD_2 As Integer = 25   'darunter nur noch 1 Lage

    '
    ' Ab dieser Größe können 2px-Konturen sinnvoll sein.
    '
    Private Const OUTLINEWIDTH_THRESHOLD As Integer = 150

    Private Const FACEFRAME_INSET_SCALE As Double = 0.07R '0.055R
    Private Const FACEFRAME_THICKNESS_SCALE As Double = 0.025R

    '
    ''' <summary>
    ''' Anzahl der zu zeichnenden Tiefenlagen.
    ''' 3 / 2 / 1 je nach Größe.
    ''' </summary>
    Public ReadOnly Property DepthLayerCount As Integer
        Get
            Dim minSide As Integer = Math.Min(Me.SteinSize.Width, Me.SteinSize.Height)

            If minSide < DEPTHLAYERS_THRESHOLD_2 Then
                Return 1
            ElseIf minSide < DEPTHLAYERS_THRESHOLD_1 Then
                Return 2
            Else
                Return 3
            End If
        End Get
    End Property

    ''
    ''' <summary>
    ''' Konturstärke der schwarzen Linien in den Tiefenlagen.
    ''' </summary>
    Public ReadOnly Property DepthOutlineWidth As Single
        Get
            Dim minSide As Integer = Math.Min(Me.SteinSize.Width, Me.SteinSize.Height)

            If minSide >= OUTLINEWIDTH_THRESHOLD Then
                Return 2.0F
            Else
                Return 1.0F
            End If
        End Get
    End Property

    Public ReadOnly Property SteinSize As Size
    Public ReadOnly Property SteinBasisSize As Size
    Public ReadOnly Property SteinRect As Rectangle
    Public ReadOnly Property BodyRect As Rectangle

    '
    ''' <summary>
    ''' Gesamte rechnerische Tiefe rechts.
    ''' </summary>
    Public ReadOnly Property RightDepth As Integer

    '
    ''' <summary>
    ''' Gesamte rechnerische Tiefe unten.
    ''' </summary>
    Public ReadOnly Property BottomDepth As Integer

    '
    ''' <summary>
    ''' Gleich breite Schattenlage rechts.
    ''' </summary>
    Public ReadOnly Property RightLayerDepth As Integer

    '
    ''' <summary>
    ''' Gleich breite Schattenlage unten.
    ''' </summary>
    Public ReadOnly Property BottomLayerDepth As Integer

    '
    ''' <summary>
    ''' Restpixel, die nicht in die Schattenlagen gehen, sondern in die Oberfläche.
    ''' </summary>
    Public ReadOnly Property RightDepthRest As Integer

    '
    ''' <summary>
    ''' Restpixel, die nicht in die Schattenlagen gehen, sondern in die Oberfläche.
    ''' </summary>
    Public ReadOnly Property BottomDepthRest As Integer

    Public ReadOnly Property FaceOuterRect As Rectangle
    Public ReadOnly Property FaceInnerRect As Rectangle
    Public ReadOnly Property FaceFrameRect As Rectangle
    Public ReadOnly Property FaceInfoLabelRect As Rectangle
    Public ReadOnly Property SymbolRect As Rectangle

    Public ReadOnly Property FrameThickness As Integer

    Public ReadOnly Property BodyCornerRadius As Integer
    Public ReadOnly Property FaceOuterCornerRadius As Integer
    Public ReadOnly Property FaceInnerCornerRadius As Integer
    Public ReadOnly Property FaceFrameCornerRadius As Integer
    Public ReadOnly Property FaceFrameThickness As Integer

    Public ReadOnly Property ShadowWidth As Integer
    Public ReadOnly Property ShadowHeight As Integer
    '
    ''' <summary>
    ''' Effektive Schatten-/Tiefengröße zum Stapeln.
    ''' Entspricht genau der Summe der tatsächlich verwendeten Lagen.
    ''' </summary>
    Public ReadOnly Property ShadowSize As Size
        Get
            Return New Size(Me.ShadowWidth, Me.ShadowHeight)
        End Get
    End Property
    '
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property FaktorBasisWidthToAktWidth As Double

    Public ReadOnly Property FaktorBasisHeightToAktHeight As Double

    ''' <summary>
    ''' Liefert die effektive Schatten-/Tiefengröße für einen Stein.
    ''' Diese Werte können direkt als Stapeloffset verwendet werden.
    ''' </summary>
    Public Shared Function GetShadowSize(steinSize As Size, steinBasisSize As Size) As Size
        Dim layout As New TileLayout(steinSize, steinBasisSize)
        Return layout.ShadowSize

    End Function

    Public Sub New(steinSize As Size, steinBasisSize As Size)

        Me.SteinSize = steinSize
        Me.SteinBasisSize = steinBasisSize
        Me.SteinRect = New Rectangle(0, 0, steinSize.Width, steinSize.Height)
        Me.BodyRect = Me.SteinRect

        Me.RightDepth = Math.Max(1, CInt(Math.Round(steinSize.Width * 0.131313R)))
        Me.BottomDepth = Math.Max(1, CInt(Math.Round(steinSize.Height * 0.087302R)))

        Me.FrameThickness = Math.Max(1, CInt(Math.Round(Math.Min(steinSize.Width, steinSize.Height) * 0.018R)))

        Me.BodyCornerRadius = Math.Max(3, CInt(Math.Round(Math.Min(steinSize.Width, steinSize.Height) * 0.048R)))
        Me.FaceOuterCornerRadius = Math.Max(3, CInt(Math.Round(Me.BodyCornerRadius * 0.88R)))
        Me.FaceInnerCornerRadius = Math.Max(2, CInt(Math.Round(Me.FaceOuterCornerRadius * 0.82R)))

        'Ein Wert > 0 erzeugt einen Abstand zwischen den Steinen.
        'Der Wert ist nur schwer hier reinzubringen.
        Dim faceOuterLeft As Integer = 0 ' CInt(FaktorBasisWidthToAktWidth * [Anzahl der Pixel bei 200 Pixel breite])
        Dim faceOuterTop As Integer = 0 ' CInt(FaktorBasisHeightToAktHeight * [Anzahl der Pixel]])

        'Schatten zuerst festlegen
        Me.RightLayerDepth = ComputeLayerDepth(totalDepth:=Me.RightDepth, layerCount:=Me.DepthLayerCount)
        Me.BottomLayerDepth = ComputeLayerDepth(totalDepth:=Me.BottomDepth, layerCount:=Me.DepthLayerCount)

        Me.ShadowWidth = Me.RightLayerDepth * Me.DepthLayerCount
        Me.ShadowHeight = Me.BottomLayerDepth * Me.DepthLayerCount

        'Oberfläche ist das, was übrig bleibt
        Dim faceOuterWidth As Integer = steinSize.Width - faceOuterLeft - Me.ShadowWidth
        Dim faceOuterHeight As Integer = steinSize.Height - faceOuterTop - Me.ShadowHeight

        If faceOuterWidth < 1 Then faceOuterWidth = 1
        If faceOuterHeight < 1 Then faceOuterHeight = 1

        Me.FaceOuterRect = New Rectangle(faceOuterLeft, faceOuterTop, faceOuterWidth, faceOuterHeight)

        Dim faceInnerLeft As Integer = Me.FaceOuterRect.Left + Me.FrameThickness
        Dim faceInnerTop As Integer = Me.FaceOuterRect.Top + Me.FrameThickness
        If faceInnerLeft < 1 Then faceInnerLeft = 1 'sonst wirkt der Faktor zur Verschiebung nicht.
        If faceInnerTop < 1 Then faceInnerTop = 1

        Dim faceInnerWidth As Integer = Me.FaceOuterRect.Width - (2 * Me.FrameThickness)
        Dim faceInnerHeight As Integer = Me.FaceOuterRect.Height - (2 * Me.FrameThickness)

        If faceInnerWidth < 1 Then faceInnerWidth = 1
        If faceInnerHeight < 1 Then faceInnerHeight = 1

        Me.FaceInnerRect = New Rectangle(faceInnerLeft, faceInnerTop, faceInnerWidth, faceInnerHeight)

        Dim faceFrameInset As Integer =
                    Math.Max(Me.FrameThickness + 2,
                    CInt(Math.Round(Math.Min(Me.FaceInnerRect.Width,
                    Me.FaceInnerRect.Height) * FACEFRAME_INSET_SCALE)))

        Dim faceFrameLeft As Integer = Me.FaceInnerRect.Left + faceFrameInset
        Dim faceFrameTop As Integer = Me.FaceInnerRect.Top + faceFrameInset
        Dim faceFrameWidth As Integer = Me.FaceInnerRect.Width - (2 * faceFrameInset)
        Dim faceFrameHeight As Integer = Me.FaceInnerRect.Height - (2 * faceFrameInset)

        If faceFrameWidth < 1 Then faceFrameWidth = 1
        If faceFrameHeight < 1 Then faceFrameHeight = 1

        Me.FaceFrameRect = New Rectangle(faceFrameLeft, faceFrameTop, faceFrameWidth, faceFrameHeight)
        Me.FaceFrameThickness = Math.Max(2, CInt(Math.Round(Math.Min(steinSize.Width, steinSize.Height) * FACEFRAME_THICKNESS_SCALE)))
        Me.FaceInfoLabelRect = New Rectangle(faceFrameLeft + CInt(faceFrameLeft * 0.35F), faceFrameTop + CInt(faceFrameTop * 0.2F), CInt(faceFrameWidth * 0.4R), CInt(faceFrameHeight * 0.16R))

        Dim faceFrameRadiusReduction As Integer = (faceFrameInset + 1) \ 2
        Me.FaceFrameCornerRadius = Math.Max(2, Me.FaceInnerCornerRadius - faceFrameRadiusReduction)

        Dim symbolWidth As Integer = CInt(Math.Round(Me.FaceInnerRect.Width * SYMBOLRECT_SCALE))
        Dim symbolHeight As Integer = CInt(Math.Round(Me.FaceInnerRect.Height * SYMBOLRECT_SCALE))

        If symbolWidth < 1 Then symbolWidth = 1
        If symbolHeight < 1 Then symbolHeight = 1

        If symbolWidth > Me.FaceInnerRect.Width Then symbolWidth = Me.FaceInnerRect.Width
        If symbolHeight > Me.FaceInnerRect.Height Then symbolHeight = Me.FaceInnerRect.Height

        Dim symbolLeft As Integer = Me.FaceInnerRect.Left + ((Me.FaceInnerRect.Width - symbolWidth) \ 2)
        Dim symbolTop As Integer = Me.FaceInnerRect.Top + ((Me.FaceInnerRect.Height - symbolHeight) \ 2)
        Dim zvW As Integer = CInt(Math.Round(steinSize.Width * SYMBOLRECT_ZENTRIERVERSCHIEBUNG * 0, mode:=MidpointRounding.AwayFromZero))
        Dim zvH As Integer = CInt(Math.Round(steinSize.Height * SYMBOLRECT_ZENTRIERVERSCHIEBUNG, 0, mode:=MidpointRounding.AwayFromZero))

        Me.SymbolRect = New Rectangle(symbolLeft + zvW, symbolTop + zvH, symbolWidth, symbolHeight)

        FaktorBasisWidthToAktWidth = CDbl(steinSize.Width) / CDbl(steinBasisSize.Width)
        FaktorBasisHeightToAktHeight = CDbl(steinSize.Height) / CDbl(steinBasisSize.Height)

    End Sub

    Private Shared Function ComputeLayerDepth(totalDepth As Integer, layerCount As Integer) As Integer

        If layerCount <= 0 Then
            Return 1
        End If

        Dim value As Integer = totalDepth \ layerCount

        If value < 1 Then
            value = 1
        End If

        Return value

    End Function

End Class