Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

Imports System.ComponentModel
Imports System.Drawing.Drawing2D

' ──────────────────────────────────────────────────────────────────────────────
'  KompassRoseX
'  • Rechteckiges UserControl, das 16 Himmelsrichtungen + Center als Dots zeigt
'  • Dots auf 3 Ringen: Außen (N,O,S,W), Mitte (NW,NO,SW,SO), Innen (alle übrigen)
'  • MouseOver/Selected/Disabled/NotVisible, Direction-Property, Clear/Reset
'  • Automatische Wahl 16x16/24x24 je nach Controlgröße
'  • Orientierungslinien vom Zentrum zu jedem Dot
'  • Mindestgröße wird berechnet, sodass Linien zwischen den Ringen „durchpassen“
' ──────────────────────────────────────────────────────────────────────────────

<DefaultEvent("DirectionChanged")>
Public Class KompassRoseX
    Inherits UserControl

#Region "Enums & Events"

    Public Enum KompassXEnum
        None    ' nichts ausgewählt
        Center  ' Center wurde geklickt
        N
        NNO
        NO
        NOO
        O
        SOO
        SO
        SSO
        S
        SSW
        SW
        SWW
        W
        NWW
        NW
        NNW
    End Enum

    ''' <summary>Ausgelöst nur bei neuer Selektion.</summary>
    Public Event DirectionChanged(direction As KompassXEnum)

#End Region

#Region "Konstanten (Layout, Größen)"

    Public Shadows Const DEFAULTSIZE As Integer = 150

    ' Ab welcher kürzeren Kante auf 24x24 umgeschaltet wird
    Private Const USE24x24_AT As Integer = 170

    ' Ringe (Faktoren bezogen auf nutzbaren Max-Radius)
    Private Const RADIUS_AUSSEN As Double = 0.9
    Private Const RADIUS_MITTE As Double = 0.9 '0.72
    Private Const RADIUS_INNEN As Double = 0.5

    ' Orientierungslinien + Innenabstand
    Private Const LINE_THICK As Integer = 2
    Private Shadows Const PADDING As Integer = 4

#End Region

#Region "Felder (Status, Mappings)"

    Private ReadOnly _pbByDir As New Dictionary(Of KompassXEnum, PictureBox)()
    Private ReadOnly _dirByPb As New Dictionary(Of PictureBox, KompassXEnum)()

    Private ReadOnly _disabled As New HashSet(Of KompassXEnum)()
    Private ReadOnly _notVisible As New HashSet(Of KompassXEnum)()

    Private _direction As KompassXEnum = KompassXEnum.None

    Private Class DotImages
        Public Property [Normal] As Bitmap
        Public Property [Over] As Bitmap
        Public Property [Selected] As Bitmap
        Public Property [Disabled] As Bitmap
        Public Property [NotVisible] As Bitmap
    End Class

    Private _dot16 As DotImages
    Private _dot24 As DotImages
    Private _center16 As Bitmap
    Private _center24 As Bitmap

    Private _use24 As Boolean

    ' (Nur informativ – Positionierung erfolgt explizit)
    Private Shared ReadOnly RING_OUTER As KompassXEnum() =
        {KompassXEnum.N, KompassXEnum.O, KompassXEnum.S, KompassXEnum.W}
    Private Shared ReadOnly RING_MID As KompassXEnum() =
        {KompassXEnum.NW, KompassXEnum.NO, KompassXEnum.SW, KompassXEnum.SO}
    Private Shared ReadOnly RING_INNER As KompassXEnum() =
        {KompassXEnum.NNO, KompassXEnum.NOO, KompassXEnum.SOO, KompassXEnum.SSO,
         KompassXEnum.SSW, KompassXEnum.SWW, KompassXEnum.NWW, KompassXEnum.NNW}

#End Region

#Region "Konstruktor & Grundsetup"

    Public Sub New()
        MyBase.New()
        Me.DoubleBuffered = True
        Me.SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.UserPaint Or ControlStyles.OptimizedDoubleBuffer, True)

        Me.Size = New Size(DEFAULTSIZE, DEFAULTSIZE)
        Dim ms0 As Integer = ComputeMinSize(detect24:=False)
        Me.MinimumSize = New Size(ms0, ms0)

        LoadBitmaps()
        BuildDots()
        LayoutDots()
    End Sub

    Protected Overrides Sub Dispose(disposing As Boolean)
        If disposing Then
            DisposeImages(_dot16)
            DisposeImages(_dot24)
            If _center16 IsNot Nothing Then _center16.Dispose()
            If _center24 IsNot Nothing Then _center24.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    Private Sub DisposeImages(d As DotImages)
        If d Is Nothing Then Return
        If d.Normal IsNot Nothing Then d.Normal.Dispose()
        If d.Over IsNot Nothing Then d.Over.Dispose()
        If d.Selected IsNot Nothing Then d.Selected.Dispose()
        If d.Disabled IsNot Nothing Then d.Disabled.Dispose()
        If d.NotVisible IsNot Nothing Then d.NotVisible.Dispose()
    End Sub

#End Region

#Region "Öffentliche API"

    ''' <summary>
    ''' Der aktuell ausgewählte Punkt. Setter wirft Exception bei Disabled/NotVisible.
    ''' </summary>
    <Browsable(True), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)>
    Public Property Direction As KompassXEnum
        Get
            Return _direction
        End Get
        Set(ByVal value As KompassXEnum)
            If value <> KompassXEnum.None AndAlso IsDisabledOrNotVisible(value) Then
                Throw New InvalidOperationException($"KompassRoseX.Direction: {value} ist deaktiviert oder unsichtbar.")
            End If
            ApplySelection(value, raiseEvt:=False)
        End Set
    End Property

    ''' <summary>Richtungen als Disabled (funktionslos) markieren.</summary>
    Public Sub DisableDirection(ByVal disabled() As KompassXEnum)
        _disabled.Clear()
        If disabled IsNot Nothing Then
            For Each d As KompassXEnum In disabled
                If d <> KompassXEnum.None Then _disabled.Add(d)
            Next
        End If
        RefreshVisualsAll()
        If _direction <> KompassXEnum.None AndAlso _disabled.Contains(_direction) Then
            ApplySelection(KompassXEnum.None, raiseEvt:=False)
        End If
    End Sub

    ''' <summary>Richtungen als NotVisible (unsichtbar und funktionslos) markieren.</summary>
    Public Sub NotVisibleDirection(ByVal notvisible() As KompassXEnum)
        _notVisible.Clear()
        If notvisible IsNot Nothing Then
            For Each d As KompassXEnum In notvisible
                If d <> KompassXEnum.None Then _notVisible.Add(d) ' FIX
            Next
        End If
        RefreshVisualsAll()
        If _direction <> KompassXEnum.None AndAlso _notVisible.Contains(_direction) Then
            ApplySelection(KompassXEnum.None, raiseEvt:=False)
        End If
    End Sub

    ''' <summary>True, falls Richtung deaktiviert oder unsichtbar ist.</summary>
    Public Function IsDisabledOrNotVisible(ByVal direction As KompassXEnum) As Boolean
        Return _disabled.Contains(direction) OrElse _notVisible.Contains(direction)
    End Function

    ''' <summary>Auswahl löschen; Disabled/NotVisible bleiben erhalten.</summary>
    Public Sub Clear()
        ApplySelection(KompassXEnum.None, raiseEvt:=False)
        RefreshVisualsAll()
    End Sub

    ''' <summary>Vollständiger Reset inkl. Entfernen von Disabled/NotVisible.</summary>
    Public Sub Reset()
        _disabled.Clear()
        _notVisible.Clear()
        ApplySelection(KompassXEnum.None, raiseEvt:=False)
        RefreshVisualsAll()
    End Sub

    ''' <summary>
    ''' Berechnetes Minimum, damit die Linien zwischen den Ringen "durchpassen".
    ''' (Für 16×16-Betrieb. Die tatsächliche MinimumSize wird dynamisch gesetzt.)
    ''' </summary>
    Public ReadOnly Property ControlMinSize As Integer
        Get
            Return ComputeMinSize(detect24:=False)
        End Get
    End Property

#End Region

#Region "Layout / Painting"

    Protected Overrides Sub OnResize(ByVal e As EventArgs)
        MyBase.OnResize(e)

        Dim shorter As Integer = Math.Min(Me.Width, Me.Height)
        Dim use24Now As Boolean = (shorter >= USE24x24_AT)

        If use24Now <> _use24 Then
            _use24 = use24Now
            Dim ms As Integer = ComputeMinSize(detect24:=_use24)
            Me.MinimumSize = New Size(ms, ms)
            ApplyImagesAll()
        End If

        LayoutDots()
        Me.Invalidate()
    End Sub

    Protected Overrides Sub OnPaint(ByVal e As PaintEventArgs)
        MyBase.OnPaint(e)

        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias

        Dim cx As Integer = Me.ClientSize.Width \ 2
        Dim cy As Integer = Me.ClientSize.Height \ 2
        Dim pCenter As New Point(cx, cy)

        Dim dotSize As Integer = If(_use24, 24, 16)
        Dim maxR As Double = (Math.Min(Me.ClientSize.Width, Me.ClientSize.Height) / 2.0) - (dotSize / 2.0) - PADDING

        Using penLine As New Pen(Color.FromArgb(120, Me.ForeColor), CSng(LINE_THICK))

            Dim r As Integer = CInt(maxR * RADIUS_AUSSEN)
            Using penSel As New Pen(Color.FromArgb(180, Me.ForeColor), CSng(2))
                e.Graphics.DrawEllipse(penSel, cx - r, cy - r, r * 2, r * 2)
            End Using

            r = CInt(maxR * RADIUS_INNEN)
            Using penSel As New Pen(Color.FromArgb(180, Me.ForeColor), CSng(2))
                e.Graphics.DrawEllipse(penSel, cx - r, cy - r, r * 2, r * 2)
            End Using

            For Each kvp As KeyValuePair(Of KompassXEnum, PictureBox) In _pbByDir
                Dim pb As PictureBox = kvp.Value
                If kvp.Key = KompassXEnum.Center Then
                    Continue For
                End If
                If pb.Visible Then

                    Dim pDot As New Point(pb.Left + (pb.Width \ 2), pb.Top + (pb.Height \ 2))
                    e.Graphics.DrawLine(penLine, pCenter, pDot)
                End If
            Next
        End Using

        ' Center-Selektion (Ring)
        If _direction = KompassXEnum.Center Then
            Dim r As Integer = CInt(maxR * 0.18)
            Using penSel As New Pen(Color.FromArgb(180, Me.ForeColor), CSng(2))
                e.Graphics.DrawEllipse(penSel, cx - r, cy - r, r * 2, r * 2)
            End Using
        End If
    End Sub

    Private Sub LayoutDots()
        If _pbByDir.Count = 0 Then Return

        Dim dotSize As Integer = If(_use24, 24, 16)
        Dim cx As Integer = Me.ClientSize.Width \ 2
        Dim cy As Integer = Me.ClientSize.Height \ 2
        Dim maxR As Double = (Math.Min(Me.ClientSize.Width, Me.ClientSize.Height) / 2.0) - (dotSize / 2.0) - PADDING

        ' Center
        PlaceDot(KompassXEnum.Center, cx, cy, dotSize)

        ' Außenring N,O,S,W (0,90,180,270°)
        PlacePolar(KompassXEnum.N, RADIUS_AUSSEN, 0.0, cx, cy, maxR, dotSize)
        PlacePolar(KompassXEnum.O, RADIUS_AUSSEN, 90.0, cx, cy, maxR, dotSize)
        PlacePolar(KompassXEnum.S, RADIUS_AUSSEN, 180.0, cx, cy, maxR, dotSize)
        PlacePolar(KompassXEnum.W, RADIUS_AUSSEN, 270.0, cx, cy, maxR, dotSize)

        ' Mittelring: NW (315°), NO (45°), SW (225°), SO (135°)
        PlacePolar(KompassXEnum.NW, RADIUS_MITTE, 315.0, cx, cy, maxR, dotSize)
        PlacePolar(KompassXEnum.NO, RADIUS_MITTE, 45.0, cx, cy, maxR, dotSize)
        PlacePolar(KompassXEnum.SW, RADIUS_MITTE, 225.0, cx, cy, maxR, dotSize)
        PlacePolar(KompassXEnum.SO, RADIUS_MITTE, 135.0, cx, cy, maxR, dotSize)

        ' Innenring (22.5°-Offsets)
        PlacePolar(KompassXEnum.NNO, RADIUS_INNEN, 22.5, cx, cy, maxR, dotSize)
        PlacePolar(KompassXEnum.NOO, RADIUS_INNEN, 67.5, cx, cy, maxR, dotSize)
        PlacePolar(KompassXEnum.SOO, RADIUS_INNEN, 112.5, cx, cy, maxR, dotSize)
        PlacePolar(KompassXEnum.SSO, RADIUS_INNEN, 157.5, cx, cy, maxR, dotSize)
        PlacePolar(KompassXEnum.SSW, RADIUS_INNEN, 202.5, cx, cy, maxR, dotSize)
        PlacePolar(KompassXEnum.SWW, RADIUS_INNEN, 247.5, cx, cy, maxR, dotSize)
        PlacePolar(KompassXEnum.NWW, RADIUS_INNEN, 292.5, cx, cy, maxR, dotSize)
        PlacePolar(KompassXEnum.NNW, RADIUS_INNEN, 337.5, cx, cy, maxR, dotSize)
    End Sub

    Private Sub PlaceDot(ByVal dir As KompassXEnum, ByVal x As Integer, ByVal y As Integer, ByVal dotSize As Integer)
        Dim pb As PictureBox = _pbByDir(dir)
        pb.Size = New Size(dotSize, dotSize)
        pb.Location = New Point(x - (dotSize \ 2), y - (dotSize \ 2))
    End Sub

    Private Sub PlacePolar(ByVal dir As KompassXEnum, ByVal rf As Double, ByVal degCWFromNorth As Double,
                           ByVal cx As Integer, ByVal cy As Integer, ByVal maxR As Double, ByVal dotSize As Integer)
        Dim rad As Double = rf * maxR
        Dim theta As Double = (90.0 - degCWFromNorth) * Math.PI / 180.0
        Dim x As Integer = CInt(cx + (rad * Math.Cos(theta)))
        Dim y As Integer = CInt(cy - (rad * Math.Sin(theta)))
        PlaceDot(dir, x, y, dotSize)
    End Sub

#End Region

#Region "Interne Helfer (Bilder, Dots, Status)"

    Private Sub LoadBitmaps()
        ' Nutzt deine bereits vorhandenen Routinen:
        _dot16 = New DotImages() With {
            .Normal = KompassDot(False),
            .Over = KompassDotMouseOver(False),
            .Selected = KompassDotSelected(False),
            .Disabled = KompassDotDisabled(False),
            .NotVisible = KompassDotNotVisible(False)
        }
        _dot24 = New DotImages() With {
            .Normal = KompassDot(True),
            .Over = KompassDotMouseOver(True),
            .Selected = KompassDotSelected(True),
            .Disabled = KompassDotDisabled(True),
            .NotVisible = KompassDotNotVisible(True)
        }
        _center16 = KompassDotCenter(False)
        _center24 = KompassDotCenter(True)

        _use24 = (Math.Min(Me.Width, Me.Height) >= USE24x24_AT)
    End Sub

    Private Function CurrentDotImages() As DotImages
        If _use24 Then
            Return _dot24
        Else
            Return _dot16
        End If
    End Function

    Private Function CurrentCenterBitmap() As Bitmap
        If _use24 Then
            Return _center24
        Else
            Return _center16
        End If
    End Function

    Private Sub BuildDots()
        Me.SuspendLayout()

        ' Enum.GetValues -> Object; mit CType nach KompassXEnum casten (Option Strict)
        Dim values As Array = [Enum].GetValues(GetType(KompassXEnum))
        For Each obj As Object In values
            Dim dir As KompassXEnum = CType(obj, KompassXEnum)

            ' FIX: Für None kein PictureBox anlegen
            If dir = KompassXEnum.None Then Continue For

            Dim pb As New PictureBox() With {
        .SizeMode = PictureBoxSizeMode.CenterImage,
        .BackColor = Color.Transparent,
        .TabStop = False
    }
            _pbByDir(dir) = pb
            _dirByPb(pb) = dir
            AddHandler pb.MouseEnter, AddressOf Pb_MouseEnter
            AddHandler pb.MouseLeave, AddressOf Pb_MouseLeave
            AddHandler pb.Click, AddressOf Pb_Click
            Me.Controls.Add(pb)
        Next

        ApplyImagesAll()
        Me.ResumeLayout()
    End Sub

    Private Sub ApplyImagesAll()
        Dim imgs As DotImages = CurrentDotImages()
        For Each kv As KeyValuePair(Of KompassXEnum, PictureBox) In _pbByDir
            Dim dir As KompassXEnum = kv.Key
            If dir = KompassXEnum.None Then
                Continue For   ' FIX
            End If

            Dim pb As PictureBox = kv.Value
            If dir = KompassXEnum.Center Then
                pb.Image = CurrentCenterBitmap()
                pb.Visible = Not _notVisible.Contains(dir)
            Else
                Dim img As Bitmap
                If _notVisible.Contains(dir) Then
                    img = imgs.NotVisible
                ElseIf _disabled.Contains(dir) Then
                    img = imgs.Disabled
                ElseIf dir = _direction Then
                    img = imgs.Selected
                Else
                    img = imgs.Normal
                End If
                pb.Image = img
                pb.Visible = Not _notVisible.Contains(dir)
            End If
        Next
    End Sub

    Private Sub RefreshVisualsAll()
        ApplyImagesAll()
        Me.Invalidate()
    End Sub

    Private Sub SetDotVisual(ByVal dir As KompassXEnum, ByVal state As ControlStatus)
        If dir = KompassXEnum.Center Then
            Dim pbC As PictureBox = _pbByDir(dir)
            pbC.Image = CurrentCenterBitmap()
            pbC.Visible = Not _notVisible.Contains(dir)
            Return
        End If

        Dim imgs As DotImages = CurrentDotImages()
        Dim pb As PictureBox = _pbByDir(dir)

        If _notVisible.Contains(dir) Then
            pb.Image = imgs.NotVisible
            pb.Visible = False
            Return
        End If

        pb.Visible = True

        Select Case state
            Case ControlStatus.Disabled
                pb.Image = imgs.Disabled
            Case ControlStatus.Selected
                pb.Image = imgs.Selected
            Case ControlStatus.MouseOver
                pb.Image = imgs.Over
            Case ControlStatus.Normal
                pb.Image = imgs.Normal
            Case ControlStatus.NotVisible
                pb.Image = imgs.NotVisible
                pb.Visible = False
        End Select
    End Sub

#End Region

#Region "Interaktion"

    Private Sub Pb_MouseEnter(ByVal sender As Object, ByVal e As EventArgs)
        Dim pb As PictureBox = DirectCast(sender, PictureBox)
        Dim dir As KompassXEnum = _dirByPb(pb)
        If dir = KompassXEnum.Center Then Return
        If IsDisabledOrNotVisible(dir) Then Return
        If dir = _direction Then Return
        SetDotVisual(dir, ControlStatus.MouseOver)
    End Sub

    Private Sub Pb_MouseLeave(ByVal sender As Object, ByVal e As EventArgs)
        Dim pb As PictureBox = DirectCast(sender, PictureBox)
        Dim dir As KompassXEnum = _dirByPb(pb)
        If dir = KompassXEnum.Center Then Return
        If IsDisabledOrNotVisible(dir) Then Return
        If dir = _direction Then
            SetDotVisual(dir, ControlStatus.Selected)
        Else
            SetDotVisual(dir, ControlStatus.Normal)
        End If
    End Sub

    Private Sub Pb_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim pb As PictureBox = DirectCast(sender, PictureBox)
        Dim dir As KompassXEnum = _dirByPb(pb)
        If IsDisabledOrNotVisible(dir) Then Return
        ApplySelection(dir, raiseEvt:=True)
    End Sub

    Private Sub ApplySelection(ByVal newDir As KompassXEnum, ByVal raiseEvt As Boolean)
        If newDir = _direction Then
            Return
        End If

        If _direction <> KompassXEnum.None AndAlso _pbByDir.ContainsKey(_direction) Then
            If Not IsDisabledOrNotVisible(_direction) AndAlso _direction <> KompassXEnum.Center Then
                SetDotVisual(_direction, ControlStatus.Normal)
            End If
        End If

        _direction = newDir

        If _direction <> KompassXEnum.None Then
            If _direction = KompassXEnum.Center Then
                _pbByDir(_direction).Image = CurrentCenterBitmap()
            Else
                SetDotVisual(_direction, ControlStatus.Selected)
            End If
        End If

        Me.Invalidate()

        If raiseEvt Then
            RaiseEvent DirectionChanged(_direction)
        End If
    End Sub

#End Region

#Region "Mindestgröße rechnerisch bestimmen"

    ''' <summary>
    ''' Mindestkantenlänge, sodass der Abstand zwischen den Ringen ≥ (DotRadius + 3).
    ''' </summary>
    Private Function ComputeMinSize(ByVal detect24 As Boolean) As Integer
        Dim dot As Integer = If(detect24, 24, 16)
        Dim dotR As Double = dot / 2.0
        Dim needGap As Double = dotR + 3.0

        Dim sizeNeeded As Integer
        Dim minDiff As Double = Math.Min(RADIUS_MITTE - RADIUS_INNEN, RADIUS_AUSSEN - RADIUS_MITTE)
        If minDiff <> 0 Then
            Dim minHalf As Double = (needGap / minDiff) + (dot / 2.0) + PADDING
            sizeNeeded = CInt(Math.Ceiling(2 * minHalf))
        End If
        sizeNeeded += 2 ' Sicherheitsaufschlag
        If sizeNeeded < 80 Then sizeNeeded = 80
        Return sizeNeeded
    End Function

#End Region

#Region "ControlStatus"

    Public Enum ControlStatus
        Normal
        MouseOver
        Selected
        Disabled
        NotVisible
    End Enum

#End Region

End Class
