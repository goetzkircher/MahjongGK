Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

Imports System.ComponentModel

' Einfache, eigengezeichnete Slider-Komponente.
' - Eigenschaften: Minimum, Maximum, Value
' - Ereignis: ValueChanged
' - Bedienung: Mausziehen, PfeilLinks / PfeilRechts, Mausrad (Schrittweite 1)
' - Daumen-Grafik: My.Resources.SliderPunkt (16x16)
' - Schmaler Schatten unter Track und Daumen
' - Keine Fokus-/Tastatur-Zierränder gezeichnet

<DefaultEvent("ValueChanged")>
Public Class BuddySlider
    Inherits Control

    ' -------------------
    ' Felder & Konstanten
    ' -------------------
    Private _minimum As Integer = 0
    Private _maximum As Integer = 99
    Private _value As Integer = 50

    Private _dragging As Boolean = False
    Private _thumbSize As New Size(16, 16) ' Ressource ist 16x16
    Private ReadOnly _trackHeight As Integer = 6
    Private ReadOnly _trackPadding As Integer = 10 ' Platz links/rechts vom Track (Daumen-Hälfte + etwas Luft)

    Private ReadOnly _shadowColor As Color = Color.FromArgb(90, 0, 0, 0) ' schmaler Schatten
    Private ReadOnly _trackShadowOffset As Integer = 1
    Private ReadOnly _thumbShadowOffset As New Point(1, 1)



    ' -------------------
    ' Initialisierung
    ' -------------------
    Public Sub New()
        SetStyle(ControlStyles.AllPaintingInWmPaint Or
                 ControlStyles.OptimizedDoubleBuffer Or
                 ControlStyles.UserPaint Or
                 ControlStyles.ResizeRedraw Or
                 ControlStyles.Selectable, True)
        TabStop = True
        Size = New Size(160, 28)
    End Sub

    ' -------------------
    ' Öffentliche API
    ' -------------------
    <Category("Behavior"), DefaultValue(0)>
    Public Property Minimum As Integer
        Get
            Return _minimum
        End Get
        Set(ByVal value As Integer)
            If value > _maximum Then
                _maximum = value
            End If
            _minimum = value
            Me.Value = Clamp(Me.Value, _minimum, _maximum)
            Invalidate()
        End Set
    End Property

    <Category("Behavior"), DefaultValue(100)>
    Public Property Maximum As Integer
        Get
            Return _maximum
        End Get
        Set(ByVal value As Integer)
            If value < _minimum Then
                _minimum = value
            End If
            _maximum = value
            Me.Value = Clamp(Me.Value, _minimum, _maximum)
            Invalidate()
        End Set
    End Property

    <Category("Behavior"), DefaultValue(50)>
    Public Property Value(Optional dontRaiseEvent As Boolean = False) As Integer
        Get
            Return _value
        End Get
        Set(ByVal newValue As Integer)
            Dim nv As Integer = Clamp(newValue, _minimum, _maximum)
            If nv <> _value Then
                _value = nv
                Invalidate()
                If Not dontRaiseEvent Then
                    OnValueChanged(EventArgs.Empty)
                End If
            End If
        End Set
    End Property

    Public Sub SetRangeAndValueSilently(min As Integer, max As Integer, v As Integer)
        Me.Minimum = min
        Me.Maximum = max
        Me.Value(dontRaiseEvent:=True) = v
    End Sub


    ' Empfohlene PopUp-Größe
    Private Const DEF_PREF_WIDTH As Integer = 200
    Private Const DEF_PREF_HEIGHT As Integer = 32

    <Browsable(True), Category("Layout")>
    Public ReadOnly Property PreferredWidth As Integer
        Get
            Return Math.Max(DEF_PREF_WIDTH, _thumbSize.Width * 8)
        End Get
    End Property

    <Browsable(True), Category("Layout")>
    Public ReadOnly Property PreferredHeight As Integer
        Get
            ' genug Luft für Track + Schatten + 16x16 Daumen
            Return Math.Max(DEF_PREF_HEIGHT, _thumbSize.Height + 12)
        End Get
    End Property

    Public Overrides Function GetPreferredSize(proposedSize As Size) As Size
        Return New Size(PreferredWidth, PreferredHeight)
    End Function

    Public Event ValueChanged As EventHandler

    Protected Overridable Sub OnValueChanged(e As EventArgs)
        RaiseEvent ValueChanged(Me, e)
    End Sub

    ' -------------------
    ' Rendering
    ' -------------------
    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        MyBase.OnPaint(e)

        Dim g As Graphics = e.Graphics
        g.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias

        ' Layout berechnen
        Dim trackRect As Rectangle = GetTrackRect()
        Dim thumbRect As Rectangle = GetThumbRect(trackRect)

        ' --- Track-Schatten ---
        Dim trackShadow As New Rectangle(trackRect.X, trackRect.Y + _trackShadowOffset, trackRect.Width, trackRect.Height)
        Using brShad As New SolidBrush(_shadowColor)
            g.FillRoundedRect(brShad, trackShadow, CInt(trackRect.Height / 2))
        End Using

        ' --- Track ---
        Dim trackColor As Color = If(BackColor.GetBrightness() < 0.5F,
                                     Color.FromArgb(64, 255, 255, 255),
                                     Color.FromArgb(96, 0, 0, 0))
        Using br As New SolidBrush(trackColor)
            g.FillRoundedRect(br, trackRect, CInt(trackRect.Height / 2))
        End Using

        ' --- Progress (links bis Daumenmitte) ---
        Dim centerX As Integer = thumbRect.X + (thumbRect.Width \ 2)
        Dim progressRect As Rectangle = Rectangle.FromLTRB(trackRect.Left, trackRect.Top, Math.Max(trackRect.Left, Math.Min(centerX, trackRect.Right)), trackRect.Bottom)
        Dim progressColor As Color = If(BackColor.GetBrightness() < 0.5F,
                                        Color.FromArgb(160, 180, 220, 255),
                                        Color.FromArgb(180, 60, 120, 220))
        Using brProg As New SolidBrush(progressColor)
            g.FillRoundedRect(brProg, progressRect, CInt(trackRect.Height / 2))
        End Using

        ' --- Thumb-Schatten ---
        Dim thumbShadow As Rectangle = thumbRect
        thumbShadow.Offset(_thumbShadowOffset)
        Using brShad As New SolidBrush(_shadowColor)
            g.FillEllipse(brShad, thumbShadow)
        End Using

        '' ''' --- Thumb (Bitmap 16x16) ---
        ''Dim bmp As Bitmap = My.Resources.SliderPunkt
        ''g.DrawImage(bmp, thumbRect)

        ' --- Thumb (selbst gezeichnete Ellipse 16x16) ---
        Dim thumbColor As Color = Color.FromArgb(255, 83, 114, 145)
        Using br As New SolidBrush(thumbColor)
            g.FillEllipse(br, thumbRect)
        End Using

        ''thumbColor = Color.FromArgb(255, 59, 68, 82)
        ''Using pen As New Pen(thumbColor, 1)
        ''    g.DrawEllipse(pen, thumbRect)
        ''End Using

        thumbRect.Inflate(-3, -3)
        thumbColor = Color.FromArgb(255, 157, 179, 200)
        Using br As New SolidBrush(thumbColor)
            g.FillEllipse(br, thumbRect)
        End Using

        thumbRect.Inflate(-3, -3)
        thumbColor = Color.FromArgb(255, 203, 216, 228)
        Using br As New SolidBrush(thumbColor)
            g.FillEllipse(br, thumbRect)
        End Using


        ' Keine Fokus-Rechteck zeichnen (bewusst unterdrückt)
    End Sub

    ' -------------------
    ' Eingabe: Maus & Tastatur & Mausrad
    ' -------------------
    Protected Overrides Sub OnMouseDown(e As MouseEventArgs)
        MyBase.OnMouseDown(e)
        Focus()

        If e.Button = MouseButtons.Left Then
            Dim trackRect As Rectangle = GetTrackRect()
            Dim thumbRect As Rectangle = GetThumbRect(trackRect)

            If thumbRect.Contains(e.Location) Then
                _dragging = True
                Capture = True
            ElseIf trackRect.Contains(e.Location) Then
                ' Klick auf den Track setzt den Wert direkt an die Position und startet Drag
                Me.Value = PositionToValue(e.X, trackRect)
                _dragging = True
                Capture = True
            End If
        End If
    End Sub

    Protected Overrides Sub OnMouseMove(e As MouseEventArgs)
        MyBase.OnMouseMove(e)
        If _dragging Then
            Dim trackRect As Rectangle = GetTrackRect()
            Me.Value = PositionToValue(e.X, trackRect)
        End If
    End Sub

    Protected Overrides Sub OnMouseUp(e As MouseEventArgs)
        MyBase.OnMouseUp(e)
        If e.Button = MouseButtons.Left AndAlso _dragging Then
            _dragging = False
            Capture = False
        End If
    End Sub

    Protected Overrides Sub OnMouseWheel(e As MouseEventArgs)
        MyBase.OnMouseWheel(e)
        ' Mausrad: üblich -> Rad nach vorn = größerer Wert
        If e.Delta > 0 Then
            StepValue(+1)
        ElseIf e.Delta < 0 Then
            StepValue(-1)
        End If
    End Sub

    Protected Overrides Sub OnKeyDown(e As KeyEventArgs)
        MyBase.OnKeyDown(e)
        If e.KeyCode = Keys.Left Then
            StepValue(-1)
            e.Handled = True
        ElseIf e.KeyCode = Keys.Right Then
            StepValue(+1)
            e.Handled = True
        End If
    End Sub

    Protected Overrides Function IsInputKey(keyData As Keys) As Boolean
        ' Sorgt dafür, dass Pfeiltasten hier ankommen
        If keyData = Keys.Left OrElse keyData = Keys.Right Then
            Return True
        End If
        Return MyBase.IsInputKey(keyData)
    End Function

    ' -------------------
    ' Layout & Helfer
    ' -------------------
    Private Sub StepValue(delta As Integer)
        If delta > 0 Then
            If _value < _maximum Then Me.Value = _value + 1
        ElseIf delta < 0 Then
            If _value > _minimum Then Me.Value = _value - 1
        End If
    End Sub

    Private Function GetTrackRect() As Rectangle
        ' Track mittig in der Höhe ausrichten
        Dim leftPad As Integer = Math.Max(_trackPadding, _thumbSize.Width \ 2)
        Dim rightPad As Integer = Math.Max(_trackPadding, _thumbSize.Width \ 2)
        Dim width As Integer = Math.Max(10, Me.Width - leftPad - rightPad)
        Dim y As Integer = (Me.Height - _trackHeight) \ 2
        Return New Rectangle(leftPad, y, width, _trackHeight)
    End Function

    Private Function GetThumbRect(trackRect As Rectangle) As Rectangle
        ' X-Position entsprechend des Werts (Daumen zentriert auf Track)
        Dim x As Integer = ValueToPosition(Me.Value, trackRect)
        Dim cx As Integer = x - (_thumbSize.Width \ 2)
        Dim cy As Integer = (Me.Height - _thumbSize.Height) \ 2
        Return New Rectangle(cx, cy, _thumbSize.Width, _thumbSize.Height)
    End Function

    Private Function ValueToPosition(v As Integer, trackRect As Rectangle) As Integer
        If _maximum = _minimum Then Return trackRect.Left
        Dim t As Double = (CDbl(v - _minimum)) / CDbl(_maximum - _minimum) ' 0..1
        Dim x As Double = trackRect.Left + t * trackRect.Width
        Return CInt(Math.Round(x))
    End Function

    Private Function PositionToValue(px As Integer, trackRect As Rectangle) As Integer
        Dim x As Integer = Clamp(px, trackRect.Left, trackRect.Right)
        Dim t As Double = (CDbl(x - trackRect.Left)) / Math.Max(1.0R, CDbl(trackRect.Width))
        Dim v As Double = _minimum + t * CDbl(_maximum - _minimum)
        ' Schrittweite 1
        Return Clamp(CInt(Math.Round(v)), _minimum, _maximum)
    End Function

    Private Shared Function Clamp(v As Integer, lo As Integer, hi As Integer) As Integer
        If v < lo Then Return lo
        If v > hi Then Return hi
        Return v
    End Function

    ' Keine unschönen Fokus-Rechtecke anzeigen
    Protected Overrides ReadOnly Property ShowFocusCues As Boolean
        Get
            Return False
        End Get
    End Property

    ' Designerfreundlich: Mindestgröße
    Protected Overrides Sub OnResize(e As EventArgs)
        MyBase.OnResize(e)
        Dim minW As Integer = Math.Max(60, _thumbSize.Width * 4)
        Dim minH As Integer = Math.Max(_thumbSize.Height + 4, 24)
        If Width < minW Then Width = minW
        If Height < minH Then Height = minH
    End Sub
End Class

' --- kleine Grafik-Helfererweiterung: abgerundete Rechtecke füllen ---
Friend Module GraphicsExtensions
    <System.Runtime.CompilerServices.Extension()>
    Public Sub FillRoundedRect(g As Graphics, brush As Brush, rect As Rectangle, radius As Integer)
        Using gp As New Drawing2D.GraphicsPath()
            Dim r As Integer = Math.Max(0, radius)
            Dim d As Integer = r * 2
            Dim arc As New Rectangle(rect.X, rect.Y, d, d)

            ' oben links
            gp.AddArc(arc, 180, 90)
            ' oben rechts
            arc.X = rect.Right - d
            gp.AddArc(arc, 270, 90)
            ' unten rechts
            arc.Y = rect.Bottom - d
            gp.AddArc(arc, 0, 90)
            ' unten links
            arc.X = rect.Left
            gp.AddArc(arc, 90, 90)
            gp.CloseFigure()

            g.FillPath(brush, gp)
        End Using
    End Sub
End Module

