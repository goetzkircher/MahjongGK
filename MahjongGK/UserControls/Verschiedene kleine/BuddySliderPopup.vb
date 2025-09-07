Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

Imports System.ComponentModel
Imports System.Drawing.Drawing2D

Public Class BuddySliderPopup
    Inherits Form

    Private ReadOnly _slider As BuddySlider
    Private ReadOnly _margin As Padding = New Padding(8)

    Private _closeX As CloseGlyphControl
    Private Const CLOSE_MARGIN As Integer = 6
    Private Const CLOSE_SIZE As Integer = 16

    ' >>> NEU: Host-eigenes Event, das vom Child-Slider gespiegelt wird
    <Category("Behavior")>
    Public Event ValueChanged As EventHandler

    Public Sub New()
        Me.FormBorderStyle = FormBorderStyle.None
        Me.ShowInTaskbar = False
        Me.StartPosition = FormStartPosition.Manual
        Me.KeyPreview = True
        Me.TopMost = True
        Me.DoubleBuffered = True
        ' Me.BackColor = Color.Transparent

        _slider = New BuddySlider() With {.TabStop = True}
        Controls.Add(_slider)
        ' Close-Glyph anlegen und über den Slider legen
        _closeX = New CloseGlyphControl() With {
            .Size = New Size(CLOSE_SIZE, CLOSE_SIZE),
            .Anchor = AnchorStyles.Top Or AnchorStyles.Right
        }
        Controls.Add(_closeX)
        _closeX.BringToFront()
        AddHandler _closeX.Click, Sub() ClosePopup()

        UpdateLayoutToPreferred()

        ' >>> NEU: Event-Re-Raise
        AddHandler _slider.ValueChanged,
            Sub(sender, e) RaiseEvent ValueChanged(Me, e)

        AddHandler Me.Deactivate, AddressOf OnDeactivateClose
    End Sub

    <Browsable(False)>
    Public ReadOnly Property Slider As BuddySlider
        Get
            Return _slider
        End Get
    End Property

    ' >>> NEU: Eigenschaften durchreichen

    <Category("Behavior")>
    Public Property Minimum As Integer
        Get
            Return _slider.Minimum
        End Get
        Set(value As Integer)
            _slider.Minimum = value
        End Set
    End Property

    <Category("Behavior")>
    Public Property Maximum As Integer
        Get
            Return _slider.Maximum
        End Get
        Set(value As Integer)
            _slider.Maximum = value
        End Set
    End Property

    <Category("Behavior")>
    Public Property Value(Optional dontRaiseEvent As Boolean = False) As Integer
        Get
            Return _slider.Value(dontRaiseEvent)
        End Get
        Set(value As Integer)
            _slider.Value(dontRaiseEvent) = value
        End Set
    End Property

    Public Sub SetRangeAndValueSilently(min As Integer, max As Integer, v As Integer)
        _slider.SetRangeAndValueSilently(min, max, v)
    End Sub
    <Category("Layout")>
    Public ReadOnly Property PreferredWidth As Integer
        Get
            Return _slider.PreferredWidth
        End Get
    End Property

    <Category("Layout")>
    Public ReadOnly Property PreferredHeight As Integer
        Get
            Return _slider.PreferredHeight
        End Get
    End Property

    <Browsable(False)>
    Public ReadOnly Property PreferredSizeHost As Size
        Get
            Return _slider.GetPreferredSize(Size.Empty)
        End Get
    End Property

    ' ---- bestehende Show-Logik bleibt gleich ----
    Public Overloads Sub Show(anchor As Control, showAt As Point)
        If anchor Is Nothing Then Throw New ArgumentNullException(NameOf(anchor))
        UpdateLayoutToPreferred()

        Dim screenPt As Point = anchor.PointToScreen(showAt)
        Dim desiredLocation As New Point(screenPt.X, screenPt.Y)
        Dim wa As Rectangle = Screen.FromControl(anchor).WorkingArea
        Dim sz As Size = Me.Size

        If desiredLocation.X + sz.Width > wa.Right Then
            desiredLocation.X = Math.Max(wa.Left, wa.Right - sz.Width)
        End If
        If desiredLocation.Y + sz.Height > wa.Bottom Then
            Dim above As Integer = screenPt.Y - sz.Height
            desiredLocation.Y = If(above >= wa.Top, above, Math.Max(wa.Top, wa.Bottom - sz.Height))
        End If

        Me.Location = desiredLocation
        MyBase.Show()
        Activate()
        _slider.Focus()
    End Sub

    Public Sub ClosePopup()
        MyBase.Close()
    End Sub

    Private Sub UpdateLayoutToPreferred()
        Dim pref As Size = _slider.GetPreferredSize(Size.Empty)
        Me.ClientSize = New Size(pref.Width + _margin.Horizontal, pref.Height + _margin.Vertical)
        _slider.Bounds = New Rectangle(_margin.Left, _margin.Top, pref.Width, pref.Height)
        ' Close-X oben rechts im Client
        _closeX.Location = New Point(Me.ClientSize.Width - CLOSE_MARGIN - _closeX.Width,
                                     CLOSE_MARGIN)
        _closeX.BringToFront()
    End Sub

    Protected Overrides Sub OnKeyDown(e As KeyEventArgs)
        MyBase.OnKeyDown(e)
        If e.KeyCode = Keys.Escape Then ClosePopup()
    End Sub

    Private Sub OnDeactivateClose(sender As Object, e As EventArgs)
        ClosePopup()
    End Sub

    Protected Overrides Sub OnMouseWheel(e As MouseEventArgs)
        MyBase.OnMouseWheel(e)
        _slider.Focus()
        ' Slider verarbeitet MouseWheel selbst
    End Sub

    Protected Overrides ReadOnly Property CreateParams As CreateParams
        Get
            Const CS_DROPSHADOW As Integer = &H20000
            Dim cp As CreateParams = MyBase.CreateParams
            cp.ClassStyle = cp.ClassStyle Or CS_DROPSHADOW
            cp.ExStyle = cp.ExStyle Or &H80 ' WS_EX_TOOLWINDOW
            Return cp
        End Get
    End Property

    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        MyBase.OnPaint(e)
        Using pen As New Pen(Color.FromArgb(60, 0, 0, 0))
            Dim r As New Rectangle(0, 0, ClientSize.Width - 1, ClientSize.Height - 1)
            e.Graphics.DrawRectangle(pen, r)
        End Using
    End Sub
    Protected Overrides Sub OnResize(e As EventArgs)
        MyBase.OnResize(e)
        If _closeX IsNot Nothing Then
            _closeX.Location = New Point(Me.ClientSize.Width - CLOSE_MARGIN - _closeX.Width,
                                         CLOSE_MARGIN)
            _closeX.BringToFront()
        End If
    End Sub
    Protected Overrides Sub WndProc(ByRef m As Message)
        Const WM_NCLBUTTONDOWN As Integer = &HA1
        Const WM_LBUTTONDOWN As Integer = &H201
        Const WM_RBUTTONDOWN As Integer = &H204
        Const WM_MBUTTONDOWN As Integer = &H207

        Select Case m.Msg
            Case WM_NCLBUTTONDOWN, WM_LBUTTONDOWN, WM_RBUTTONDOWN, WM_MBUTTONDOWN
                Dim p As Point = PointToClient(Control.MousePosition)

                ' Klick auf das Close-X: nichts tun -> Child bekommt das Ereignis
                If _closeX IsNot Nothing AndAlso _closeX.Bounds.Contains(p) Then
                    Return
                End If

                ' Klick außerhalb des Sliders => schließen
                If Not _slider.Bounds.Contains(p) Then
                    ClosePopup()
                    Return
                End If
        End Select
        MyBase.WndProc(m)
    End Sub

End Class


Friend Class CloseGlyphControl
    Inherits Control

    Private _hover As Boolean
    Private _down As Boolean

    Public Sub New()
        SetStyle(ControlStyles.AllPaintingInWmPaint Or
                 ControlStyles.UserPaint Or
                 ControlStyles.OptimizedDoubleBuffer Or
                 ControlStyles.AllPaintingInWmPaint, True)
        Me.Size = New Size(16, 16)
        Me.BackColor = SystemColors.Control
        Me.TabStop = False
    End Sub

    Protected Overrides Sub OnMouseEnter(e As EventArgs)
        MyBase.OnMouseEnter(e)
        _hover = True
        Cursor = Cursors.Hand
        Invalidate()
    End Sub

    Protected Overrides Sub OnMouseLeave(e As EventArgs)
        MyBase.OnMouseLeave(e)
        _hover = False
        _down = False
        Cursor = Cursors.Default
        Invalidate()
    End Sub

    Protected Overrides Sub OnMouseDown(e As MouseEventArgs)
        MyBase.OnMouseDown(e)
        If e.Button = MouseButtons.Left Then
            _down = True
            Invalidate()
        End If
    End Sub

    Protected Overrides Sub OnMouseUp(e As MouseEventArgs)
        MyBase.OnMouseUp(e)
        If _down AndAlso ClientRectangle.Contains(e.Location) Then
            _down = False
            Invalidate()
            OnClick(EventArgs.Empty) ' Click auslösen
        Else
            _down = False
            Invalidate()
        End If
    End Sub

    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        MyBase.OnPaint(e)
        Dim g As Graphics = e.Graphics
        g.SmoothingMode = SmoothingMode.AntiAlias

        ' dezente runde Hover/Down-Fläche
        Dim bg As Color =
            If(_down, Color.FromArgb(80, 0, 0, 0),
            If(_hover, Color.FromArgb(40, 0, 0, 0), Color.Transparent))
        If bg.A > 0 Then
            Using br As New SolidBrush(bg)
                g.FillEllipse(br, ClientRectangle)
            End Using
        End If

        ' X zeichnen (leicht eingerückt)
        Dim inset As Integer = 5
        Dim r As Rectangle = Rectangle.Inflate(ClientRectangle, -inset, -inset)
        Using penX As New Pen(Color.FromArgb(200, 40, 40, 40), 2)
            g.DrawLine(penX, r.Left, r.Top, r.Right, r.Bottom)
            g.DrawLine(penX, r.Right, r.Top, r.Left, r.Bottom)
        End Using
    End Sub
End Class
