Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

Imports System.ComponentModel

Public Class Num2UpDown
    Inherits UserControl

    Private Const HARD_MIN As Integer = 0
    Private Const HARD_MAX As Integer = 99

    Private _resPfeilDn As String = "CompassS" '"PfeilDn"
    Private _resPfeilUp As String = "CompassN" '"PfeilUp"

    ' --- Buddy-Slider (Popup) ---
    Private _buddy As BuddySliderPopup ' on-demand erzeugt

    Private ReadOnly _picLeft As New PictureBox()
    Private ReadOnly _lbl As New Label()
    Private ReadOnly _picRight As New PictureBox()

    Private _hoverLeft As Boolean
    Private _hoverRight As Boolean

    Private _value As Integer = 0
    Private _minValue As Integer = HARD_MIN
    Private _maxValue As Integer = HARD_MAX

    Public Event ValueChanged()

    Public Sub New()

        MyBase.New()
        Me.MinimumSize = New Size(65, 24)
        Me.Size = New Size(65, 28)
        Me.DoubleBuffered = True
        Me.TabStop = True

        ' Pfeil links (dekrement)
        _picLeft.Size = New Size(16, 16)
        _picLeft.SizeMode = PictureBoxSizeMode.CenterImage
        _picLeft.BackColor = Color.Transparent
        _picLeft.Cursor = Cursors.Hand
        _picLeft.Image = GetResBitmap(_resPfeilDn, hover:=False)
        AddHandler _picLeft.MouseClick, Sub(sender As Object, e As MouseEventArgs)
                                            If e.Button = MouseButtons.Left Then
                                                IncrementValue(-1)
                                            End If
                                        End Sub
        AddHandler _picLeft.MouseEnter, Sub(_s, __) SetHover(leftSide:=True, hover:=True)
        AddHandler _picLeft.MouseLeave, Sub(_s, __) SetHover(leftSide:=True, hover:=False)

        ' Label in der Mitte
        _lbl.AutoSize = False
        _lbl.TextAlign = ContentAlignment.MiddleCenter
        _lbl.Font = New Font("Segoe UI", 10.0F, FontStyle.Regular, GraphicsUnit.Point)
        _lbl.ForeColor = SystemColors.ControlText
        _lbl.BackColor = Color.Transparent
        _lbl.Text = FormatTwoDigits(_value)
        AddHandler _lbl.MouseWheel, AddressOf OnMouseWheelForwardFocus
        AddHandler _lbl.Click, Sub(_s, __) Me.Focus()

        ' Pfeil rechts (inkrement)
        _picRight.Size = New Size(16, 16)
        _picRight.SizeMode = PictureBoxSizeMode.CenterImage
        _picRight.BackColor = Color.Transparent
        _picRight.Cursor = Cursors.Hand
        _picRight.Image = GetResBitmap(_resPfeilUp, hover:=False)
        AddHandler _picRight.MouseClick, Sub(sender As Object, e As MouseEventArgs)
                                             If e.Button = MouseButtons.Left Then
                                                 IncrementValue(1)
                                             End If
                                         End Sub
        AddHandler _picRight.MouseEnter, Sub(_s, __) SetHover(leftSide:=False, hover:=True)
        AddHandler _picRight.MouseLeave, Sub(_s, __) SetHover(leftSide:=False, hover:=False)
        AddHandler _picLeft.MouseUp, AddressOf OnArrowMouseUp
        AddHandler _picRight.MouseUp, AddressOf OnArrowMouseUp
        AddHandler _lbl.MouseUp, AddressOf OnArrowMouseUp
        AddHandler Me.MouseUp, AddressOf OnArrowMouseUp


        Controls.Add(_picLeft)
        Controls.Add(_lbl)
        Controls.Add(_picRight)

        PerformLayout()

    End Sub


    ' 1) Zentrale Methode
    Private Sub ApplyArrowImages()
        _picLeft.Image = GetResBitmap(_resPfeilDn, hover:=False)
        _picRight.Image = GetResBitmap(_resPfeilUp, hover:=False)
        _picLeft.Invalidate()
        _picRight.Invalidate()
    End Sub

    ' 2) Property setzt nur noch Namen + Apply
    <Browsable(True), Category("Behavior"), DefaultValue(False), RefreshProperties(RefreshProperties.Repaint)>
    Public Property UseArrowRightLeft As Boolean
        Get
            Return _resPfeilDn = "CompassW"
        End Get
        Set(value As Boolean)
            If value Then
                _resPfeilDn = "CompassW"
                _resPfeilUp = "CompassO"
            Else
                _resPfeilDn = "CompassS"
                _resPfeilUp = "CompassN"
            End If
            If IsHandleCreated Then ApplyArrowImages()
        End Set
    End Property

    ' 3) Nach dem Designer-Setzen sicher anwenden
    Protected Overrides Sub OnCreateControl()
        MyBase.OnCreateControl()
        ApplyArrowImages()
    End Sub

    <Browsable(True), Category("Behavior"), DefaultValue(True)>
    Public Property ClampValue As Boolean = True

    <Browsable(True), Category("Behavior"), DefaultValue(0)>
    Public Property Value As Integer
        Get
            Return _value
        End Get
        Set(ByVal v As Integer)
            ' zuerst hart clampen auf 0..99
            v = Math.Min(HARD_MAX, Math.Max(HARD_MIN, v))

            If ClampValue Then
                ' Werte einfach auf den Bereich begrenzen
                v = Math.Min(_maxValue, Math.Max(_minValue, v))
            Else
                ' harte Prüfung mit Exception
                If v < _minValue OrElse v > _maxValue Then
                    Throw New ArgumentOutOfRangeException(
                    NameOf(Value),
                    $"Wert {v} liegt außerhalb von Min={_minValue} und Max={_maxValue}."
                )
                End If
            End If

            _value = v
            _lbl.Text = FormatTwoDigits(_value)
            RaiseEvent ValueChanged() ' IMMER feuern
        End Set
    End Property

    <Browsable(True), Category("Behavior"), DefaultValue(HARD_MIN)>
    Public Property MinValue As Integer
        Get
            Return _minValue
        End Get
        Set(value As Integer)
            ValidateBound(value, NameOf(MinValue))
            If value > _maxValue Then
                If ClampValue Then
                    _maxValue = value
                Else
                    Throw New ArgumentOutOfRangeException(NameOf(MinValue), "MinValue darf nicht größer als MaxValue sein.")
                End If
            End If
            _minValue = value
            If _value < _minValue Then Me.Value = _minValue
        End Set
    End Property

    <Browsable(True), Category("Behavior"), DefaultValue(HARD_MAX)>
    Public Property MaxValue As Integer
        Get
            Return _maxValue
        End Get
        Set(value As Integer)
            ValidateBound(value, NameOf(MaxValue))
            If value < _minValue Then
                If ClampValue Then
                    _minValue = value
                Else
                    Throw New ArgumentOutOfRangeException(NameOf(MaxValue), "MaxValue darf nicht kleiner als MinValue sein.")
                End If
            End If
            _maxValue = value
            If _value > _maxValue Then Me.Value = _maxValue
        End Set
    End Property

    Public Sub Increment()
        If _value < _maxValue Then
            Me.Value = _value + 1
        End If
    End Sub

    ''' <summary>
    ''' Positive Werte Incrementieren, negative Decrementieren, 0 macht nichts.
    ''' Wenn das Ergebnis die Grenzen Minimum/Maximum überschreiten würde, passiert nichts.
    ''' </summary>
    ''' <param name="incdec"></param>
    Public Sub Increment(incdec As Integer)

        If incdec <> 0 Then
            If incdec < 0 Then
                incdec = Math.Abs(incdec)
                If _value >= _minValue + incdec Then
                    Me.Value = _value - incdec
                End If
            Else
                If _value <= _maxValue - incdec Then
                    Me.Value = _value + incdec
                End If
            End If
        End If

    End Sub

    Public Sub Decrement()
        If _value > _minValue Then
            Me.Value = _value - 1
        End If
    End Sub


    Private Shared Sub ValidateBound(b As Integer, paramName As String)
        If b < HARD_MIN OrElse b > HARD_MAX Then
            Throw New ArgumentOutOfRangeException(paramName, $"Wert muss zwischen {HARD_MIN} und {HARD_MAX} liegen.")
        End If
    End Sub

    ' --- MouseOver Bildwechsel ---
    Private Sub SetHover(leftSide As Boolean, hover As Boolean)
        If leftSide Then
            _hoverLeft = hover
            _picLeft.Image = GetResBitmap(_resPfeilDn, _hoverLeft)
        Else
            _hoverRight = hover
            _picRight.Image = GetResBitmap(_resPfeilUp, _hoverRight)
        End If
    End Sub

    Private Shared Function GetResBitmap(baseName As String, hover As Boolean) As Bitmap
        Dim name As String = baseName & If(hover, "mover", String.Empty)
        Dim bmp As Bitmap = If(Theme.GetResBmp(name), Theme.GetResBmp(baseName))
        Return bmp
    End Function
    ' -----------------------------

    ' Tastatur: Up/Down
    Protected Overrides Function IsInputKey(keyData As Keys) As Boolean
        If keyData = Keys.Up OrElse keyData = Keys.Down Then Return True
        Return MyBase.IsInputKey(keyData)
    End Function

    Protected Overrides Sub OnKeyDown(e As KeyEventArgs)
        MyBase.OnKeyDown(e)
        If e.KeyCode = Keys.Down Then
            IncrementValue(-1) : e.Handled = True
        ElseIf e.KeyCode = Keys.Up Then
            IncrementValue(+1) : e.Handled = True
        End If
    End Sub

    ' Mausrad (wenn fokussiert)
    Protected Overrides Sub OnMouseWheel(e As MouseEventArgs)
        MyBase.OnMouseWheel(e)
        If Me.Focused Then IncrementValue(If(e.Delta > 0, +1, -1))
    End Sub

    Private Sub OnMouseWheelForwardFocus(sender As Object, e As MouseEventArgs)
        OnMouseWheel(e)
    End Sub

    Private Sub IncrementValue(stepBy As Integer)
        If stepBy > 0 Then
            Increment()
        ElseIf stepBy < 0 Then
            Decrement()
        End If
        Me.Focus()
    End Sub


    Private Shared Function FormatTwoDigits(n As Integer) As String
        If n < HARD_MIN Then n = HARD_MIN
        If n > HARD_MAX Then n = HARD_MAX
        Return n.ToString("00")
    End Function

    Protected Overrides Sub OnLayout(levent As LayoutEventArgs)
        MyBase.OnLayout(levent)
        Dim w As Integer = Me.ClientSize.Width
        Dim h As Integer = Me.ClientSize.Height

        Dim centerY As Integer = (h - _picLeft.Height) \ 2
        _picLeft.Location = New Point(2, Math.Max(0, centerY)) '+ 1)
        _picRight.Location = New Point(w - _picRight.Width - 2, Math.Max(0, centerY)) '- 1)

        Dim lblLeft As Integer = _picLeft.Right
        Dim lblRight As Integer = _picRight.Left
        Dim lblWidth As Integer = Math.Max(0, lblRight - lblLeft)
        _lbl.Bounds = New Rectangle(lblLeft, 0, lblWidth, h)
    End Sub

    Protected Overrides Sub OnPaint(pe As PaintEventArgs)
        MyBase.OnPaint(pe)
        'If Me.Focused Then ControlPaint.DrawFocusRectangle(pe.Graphics, Me.ClientRectangle)
    End Sub

#Region "Buddy-Slider (Popup)"

    Private Sub OnArrowMouseUp(sender As Object, e As MouseEventArgs)
        If e.Button = MouseButtons.Right Then
            ShowBuddySlider(DirectCast(sender, Control), New Point(e.X, e.Y))
        End If
    End Sub

    Private Sub ShowBuddySlider(anchor As Control, localPt As Point)

        Dim addHdl As Boolean
        If _buddy Is Nothing OrElse _buddy.IsDisposed Then
            _buddy = New BuddySliderPopup()
            addHdl = True
        End If

        ' Bereich und Startwert synchronisieren
        _buddy.SetRangeAndValueSilently(Me.MinValue, Me.MaxValue, Me.Value)

        If addHdl Then
            'Dann erst AddHandler, sonst ist Me.Value auf Maximum
            AddHandler _buddy.ValueChanged, Sub(_s, __)
                                                ' Live-Übernahme; clampen macht Property Value schon selbst
                                                'AddHandler lößt bereits das erste Event aus ==>  Me.Value wird überschrieben
                                                'durch den Startwert des Buddy.
                                                Me.Value = _buddy.Value
                                            End Sub
        End If

        ' Position relativ zum angeklickten Pfeil etwas unterhalb anzeigen
        Dim showAt As Point = localPt
        showAt.Y = anchor.Height + 2

        ' links/rechts leicht versetzen, damit der Popupkörper nicht über dem Finger/Maus liegt
        If anchor Is _picLeft Then
            showAt.X = Math.Max(0, anchor.Width \ 2 - _buddy.PreferredWidth \ 2)
        Else
            showAt.X = Math.Min(anchor.Width - _buddy.PreferredWidth, anchor.Width \ 2 - _buddy.PreferredWidth \ 2)
        End If

        ' Fallback: wenn zu schmal, an 0 andocken
        If showAt.X < 0 Then showAt.X = 0

        ' Anzeigen; AutoClose greift bei Fokusverlust/Klick außerhalb
        _buddy.Show(anchor, showAt)
    End Sub
    ' --- Ende Buddy-Slider ---

#End Region

    ''Protected Overrides Sub Dispose(disposing As Boolean)
    ''    Try
    ''        If disposing Then
    ''            If _buddy IsNot Nothing Then
    ''                _buddy.Close()
    ''                _buddy.Dispose()
    ''                _buddy = Nothing
    ''            End If
    ''        End If
    ''    Finally
    ''        MyBase.Dispose(disposing)
    ''    End Try
    ''End Sub

End Class

