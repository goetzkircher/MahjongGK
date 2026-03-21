Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On
Imports System.Drawing.Drawing2D
Imports Grafix = System.Drawing.Graphics   ' Alias, damit die Signatur exakt "Grafix" verwenden kann

' ─────────────────────────────────────────────────────────────────────────────
' HScrollRenderer  (mit Pfeilbuttons + Auto-Repeat)
'   - keine Events für Value; Polling via ConsumeValueChanged()
'   - Mausbedienung: Hover, Pressed, Drag, Page-Scroll, MouseWheel
'   - Pfeilbuttons links/rechts (SmallChange), gedrückt halten = Auto-Repeat
'   - Auto-Repeat per PumpAutoRepeat() (zeitgesteuert, fürs Polling)
' ─────────────────────────────────────────────────────────────────────────────
''' <summary>
''' Pfad: MahjongGK/Spielfeld/Render
''' </summary>
Public Class HScrollRenderer

    ' Monotone Uhr (startet einmalig beim Laden)
    '(Monoton heißt, die Zeit springt nicht bei Änderungen der Systemzeit.)
    Private ReadOnly _clock As Stopwatch = Stopwatch.StartNew()
    Private Function NowMs() As Long
        Return _clock.ElapsedMilliseconds
    End Function

    ' ── öffentlicher Zustand / API ────────────────────────────────────────────
    Private _minimum As Integer = 0
    Private _maximum As Integer = 0
    Private _value As Integer = 0
    Private _smallChange As Integer = 1
    Private _largeChange As Integer = 10
    Private _valueChanged As Boolean = False

    ' Layout
    Private Const ArrowWidth As Integer = 22
    Private Const ThumbMinPx As Integer = 24

    ' Maus-Zustand
    Private _isDragging As Boolean = False
    Private _dragStartX As Integer = 0
    Private _dragStartValue As Integer = 0
    Private _isHoverThumb As Boolean = False
    Private _isPressedThumb As Boolean = False
    Private _hoverLeft As Boolean = False
    Private _hoverRight As Boolean = False
    Private _pressedLeft As Boolean = False
    Private _pressedRight As Boolean = False

    ' Auto-Repeat (auch für Page-Scroll, wenn gewünscht)
    Private Enum RepeatAction
        None
        ArrowLeft
        ArrowRight
        PageLeft
        PageRight
    End Enum

    Private _repeat As RepeatAction = RepeatAction.None
    Private _repeatNextMs As Long = 0
    Private Const RepeatInitialDelayMs As Integer = 400
    Private Const RepeatIntervalMs As Integer = 40

    ' Geometrie-Cache
    Private _lastThumbRect As Rectangle = Rectangle.Empty
    Private _lastTrackRect As Rectangle = Rectangle.Empty
    Private _lastLeftArrow As Rectangle = Rectangle.Empty
    Private _lastRightArrow As Rectangle = Rectangle.Empty
    Private _rectHScrollbar As Rectangle = Rectangle.Empty
    Private _colorschemeEnabled As ScrollColorScheme
    Private _colorschemeDisabled As ScrollColorScheme
    Private _isInit As Boolean = False


    Sub New()

    End Sub

    Public Sub New(rectHScrollbar As Rectangle, basisColor As Color)
        If rectHScrollbar.Width <= 0 OrElse rectHScrollbar.Height <= 0 Then
            Throw New Exception("HScrollRenderer rectHScrollbar.Width <= 0 OrElse rectHScrollbar.Height <= 0")
        End If
        _rectHScrollbar = rectHScrollbar
        _colorschemeEnabled = BuildScheme(basisColor, enabled:=True)
        _colorschemeDisabled = BuildScheme(basisColor, enabled:=False)
        _isInit = True
    End Sub


    Public Property RectHScrollbar As Rectangle
        Get
            Return _rectHScrollbar
        End Get
        Set(value As Rectangle)
            _rectHScrollbar = value
        End Set
    End Property

    ' ── Bereich / Value / Schritte ───────────────────────────────────────────
    ''' <summary>
    ''' minimum wird auf 0 gesetzt, value wird angepasst, wenn > maximum
    ''' </summary>
    ''' <param name="maximum"></param>
    Public Sub SetRange(maximum As Integer)
        If _maximum <> maximum Then
            If maximum < 0 Then
                maximum = 0
            End If
            _minimum = 0
            _maximum = maximum
            If _value > maximum Then
                _value = maximum
            End If
            CoerceValue(_value)
        End If
    End Sub

    Public Sub SetRange(minimum As Integer, maximum As Integer, Optional adjustValue As Boolean = False)
        If maximum < minimum Then maximum = minimum
        _minimum = minimum
        _maximum = maximum
        CoerceValue(_value)
    End Sub

    Public Sub SetPageSize(pageSize As Integer)
        If pageSize < 1 Then pageSize = 1
        _largeChange = pageSize
        CoerceValue(_value)
    End Sub

    Public Sub SetSmallChange(stepWidth As Integer)
        If stepWidth < 1 Then stepWidth = 1
        _smallChange = stepWidth
    End Sub

    Public Function GetValue() As Integer
        Return _value
    End Function

    Public Sub SetValue(v As Integer)
        CoerceValue(v)
    End Sub

    Public Sub ScrollBy(delta As Integer)
        CoerceValue(_value + delta)
    End Sub

    Public Sub ScrollPageLeft()
        CoerceValue(_value - _largeChange)
    End Sub

    Public Sub ScrollPageRight()
        CoerceValue(_value + _largeChange)
    End Sub

    ''' <summary>True, wenn sich der Wert seit der letzten Abfrage geändert hat (wird dabei zurückgestellt).</summary>
    Public Function ConsumeValueChanged() As Boolean
        Dim wasChanged As Boolean = _valueChanged
        _valueChanged = False
        Return wasChanged
    End Function

    ' ── Zeichnen ─────────────────────────────────────────────────────────────
    Public Sub PaintHScroll(gfx As Grafix, enabled As Boolean)

        If Not _isInit Then
            Throw New Exception("HScrollRenderer.PaintHScroll not Init")
        End If

        Dim scheme As ScrollColorScheme = If(enabled, _colorschemeEnabled, _colorschemeDisabled)

        ' Pfeil- und Trackrechtecke berechnen
        _lastLeftArrow = New Rectangle(_rectHScrollbar.Left, _rectHScrollbar.Top, ArrowWidth, _rectHScrollbar.Height)
        _lastRightArrow = New Rectangle(_rectHScrollbar.Right - ArrowWidth, _rectHScrollbar.Top, ArrowWidth, _rectHScrollbar.Height)
        _lastTrackRect = Rectangle.FromLTRB(_lastLeftArrow.Right, _rectHScrollbar.Top, _lastRightArrow.Left, _rectHScrollbar.Bottom)

        ' Track
        Using bg As New SolidBrush(scheme.Track)
            gfx.FillRectangle(bg, _lastTrackRect)
        End Using
        Using topPen As New Pen(scheme.TrackHighlight),
              botPen As New Pen(scheme.TrackShadow)
            gfx.DrawLine(topPen, _lastTrackRect.Left, _lastTrackRect.Top, _lastTrackRect.Right - 1, _lastTrackRect.Top)
            gfx.DrawLine(botPen, _lastTrackRect.Left, _lastTrackRect.Bottom - 1, _lastTrackRect.Right - 1, _lastTrackRect.Bottom - 1)
        End Using

        ' Thumb
        _lastThumbRect = ComputeThumbRect(_lastTrackRect)
        If _lastThumbRect.Width > 0 Then
            ' Schatten
            Dim shadow As New Rectangle(_lastThumbRect.X, _lastThumbRect.Bottom - 2, _lastThumbRect.Width, 2)
            Using sb As New SolidBrush(scheme.ThumbShadow)
                gfx.FillRectangle(sb, shadow)
            End Using

            ' Statusfarben
            Dim inner As Rectangle = Inflate(_lastThumbRect, -1, -1)
            Dim topCol As Color = scheme.ThumbGradTop
            Dim botCol As Color = scheme.ThumbGradBottom
            Dim borderCol As Color = scheme.ThumbBorder
            If _isPressedThumb Then
                topCol = scheme.ThumbGradTopPressed : botCol = scheme.ThumbGradBottomPressed : borderCol = scheme.ThumbBorderPressed
            ElseIf _isHoverThumb Then
                topCol = scheme.ThumbGradTopHover : botCol = scheme.ThumbGradBottomHover : borderCol = scheme.ThumbBorderHover
            End If

            Using lg As New LinearGradientBrush(inner, topCol, botCol, LinearGradientMode.Vertical),
                  bd As New Pen(borderCol)
                gfx.FillRectangle(lg, inner)
                gfx.DrawRectangle(bd, Rectangle.FromLTRB(_lastThumbRect.Left, _lastThumbRect.Top, _lastThumbRect.Right - 1, _lastThumbRect.Bottom - 1))
            End Using

            ' Gripper
            Dim gy As Integer = CInt(inner.Top + inner.Height \ 2)
            Dim gx0 As Integer = inner.Left + Math.Max(4, inner.Width \ 4)
            Dim gx1 As Integer = inner.Right - Math.Max(4, inner.Width \ 4)
            Using pen1 As New Pen(scheme.GripperLight),
                  pen2 As New Pen(scheme.GripperDark)
                gfx.DrawLine(pen2, gx0, gy, gx1, gy)
                gfx.DrawLine(pen1, gx0, gy - 1, gx1, gy - 1)
                gfx.DrawLine(pen1, gx0, gy + 1, gx1, gy + 1)
            End Using
        End If

        ' Pfeilbuttons zeichnen
        DrawArrowButton(gfx, _lastLeftArrow, scheme, leftArrow:=True, enabled:=enabled, hover:=_hoverLeft, pressed:=_pressedLeft)
        DrawArrowButton(gfx, _lastRightArrow, scheme, leftArrow:=False, enabled:=enabled, hover:=_hoverRight, pressed:=_pressedRight)
    End Sub

    ' ── Maus-Steuerung (wertet nur Status aus, keine Fremd-Invalidates) ──────
    Public Function HandleMouseDown(x As Integer, y As Integer, hostRect As Rectangle) As Boolean
        Dim anyChange As Boolean = False

        ' Pfeile zuerst
        If _lastLeftArrow.Contains(x, y) Then
            _pressedLeft = True : _repeat = RepeatAction.ArrowLeft
            _repeatNextMs = NowMs() + RepeatInitialDelayMs
            ScrollBy(-_smallChange) : anyChange = True
            Return anyChange
        End If
        If _lastRightArrow.Contains(x, y) Then
            _pressedRight = True : _repeat = RepeatAction.ArrowRight
            _repeatNextMs = NowMs() + RepeatInitialDelayMs
            ScrollBy(+_smallChange) : anyChange = True
            Return anyChange
        End If

        ' Track
        If Not _lastTrackRect.Contains(x, y) Then Return False

        If _lastThumbRect.Contains(x, y) Then
            _isDragging = True
            _isPressedThumb = True
            _dragStartX = x
            _dragStartValue = _value
            anyChange = True
        Else
            If x < _lastThumbRect.Left Then
                ScrollPageLeft() : _repeat = RepeatAction.PageLeft : _repeatNextMs = NowMs() + RepeatInitialDelayMs
                anyChange = True
            ElseIf x > _lastThumbRect.Right Then
                ScrollPageRight() : _repeat = RepeatAction.PageRight : _repeatNextMs = NowMs() + RepeatInitialDelayMs
                anyChange = True
            End If
        End If
        Return anyChange
    End Function

    Public Function HandleMouseMove(x As Integer, y As Integer, hostRect As Rectangle) As Boolean
        Dim anyChange As Boolean = False

        ' Hover-Stati Pfeile
        Dim hl As Boolean = _lastLeftArrow.Contains(x, y)
        Dim hr As Boolean = _lastRightArrow.Contains(x, y)
        If hl <> _hoverLeft Then _hoverLeft = hl : anyChange = True
        If hr <> _hoverRight Then _hoverRight = hr : anyChange = True

        ' Hover Thumb
        Dim ht As Boolean = _lastThumbRect.Contains(x, y)
        If ht <> _isHoverThumb Then _isHoverThumb = ht : anyChange = True

        If Not _isDragging Then Return anyChange

        ' Dragging → Wert anpassen
        Dim track As Rectangle = _lastTrackRect
        Dim rangeSpan As Integer = Math.Max(1, (_maximum - _minimum))
        Dim page As Integer = Math.Min(_largeChange, rangeSpan)
        Dim thumbLen As Integer = ThumbPixelLength(track.Width)
        Dim movablePx As Integer = Math.Max(1, track.Width - thumbLen)
        Dim dx As Integer = x - _dragStartX
        Dim valuePerPx As Double = (rangeSpan - page + 1) / Math.Max(1, CDbl(movablePx))
        Dim newVal As Integer = CInt(Math.Round(_dragStartValue + dx * valuePerPx, MidpointRounding.AwayFromZero))
        Dim before As Integer = _value
        CoerceValue(newVal)
        If _value <> before Then anyChange = True

        Return anyChange
    End Function

    Public Function HandleMouseUp() As Boolean
        Dim changed As Boolean = _isDragging OrElse _isPressedThumb OrElse _pressedLeft OrElse _pressedRight
        _isDragging = False
        _isPressedThumb = False
        _pressedLeft = False
        _pressedRight = False
        _repeat = RepeatAction.None
        Return changed
    End Function

    Public Function HandleMouseLeave() As Boolean
        Dim changed As Boolean = _isDragging OrElse _isHoverThumb OrElse _isPressedThumb OrElse _hoverLeft OrElse _hoverRight OrElse _pressedLeft OrElse _pressedRight
        _isDragging = False
        _isHoverThumb = False
        _isPressedThumb = False
        _hoverLeft = False
        _hoverRight = False
        _pressedLeft = False
        _pressedRight = False
        _repeat = RepeatAction.None
        Return changed
    End Function

    Public Function HandleMouseWheel(delta As Integer) As Boolean
        If delta = 0 Then Return False
        Dim steps As Integer = Math.Max(1, Math.Abs(delta) \ 120)
        Dim dir As Integer = If(delta > 0, -1, +1) ' nach oben = nach links
        Dim before As Integer = _value
        CoerceValue(_value + dir * steps * _smallChange)
        Return _value <> before
    End Function

    ''' <summary>
    ''' Auto-Repeat-Pumpe. Aus dem Polling heraus regelmäßig aufrufen.
    ''' Rückgabe True, wenn sich etwas geändert hat (neu zeichnen).
    ''' </summary>
    Public Function PumpAutoRepeat() As Boolean

        If _repeat = RepeatAction.None Then
            Return False
        End If

        Dim aktMs As Long = NowMs()

        If aktMs < _repeatNextMs Then
            Return False
        End If

        Select Case _repeat
            Case RepeatAction.ArrowLeft : ScrollBy(-_smallChange)
            Case RepeatAction.ArrowRight : ScrollBy(+_smallChange)
            Case RepeatAction.PageLeft : ScrollPageLeft()
            Case RepeatAction.PageRight : ScrollPageRight()
        End Select

        _repeatNextMs = aktMs + RepeatIntervalMs
        Return True

    End Function

    ' ── interne Helfer ───────────────────────────────────────────────────────
    Private Sub CoerceValue(v As Integer)
        Dim rangeSpan As Integer = Math.Max(0, _maximum - _minimum)
        Dim page As Integer = Math.Min(_largeChange, Math.Max(1, rangeSpan + 1))
        Dim maxVal As Integer = _maximum - page + 1
        If maxVal < _minimum Then maxVal = _minimum

        Dim nv As Integer = Math.Min(Math.Max(v, _minimum), maxVal)
        If nv <> _value Then
            _value = nv
            _valueChanged = True
        End If
    End Sub

    Private Function ComputeThumbRect(track As Rectangle) As Rectangle
        Dim span As Integer = Math.Max(0, _maximum - _minimum)
        Dim page As Integer = Math.Min(_largeChange, span + 1)
        If track.Width <= 0 Then Return Rectangle.Empty

        Dim thumbLen As Integer = ThumbPixelLength(track.Width)
        Dim movablePx As Integer = Math.Max(1, track.Width - thumbLen)
        Dim maxVal As Integer = Math.Max(_minimum, _maximum - page + 1)
        Dim denom As Integer = Math.Max(1, maxVal - _minimum)
        Dim t As Double = (_value - _minimum) / CDbl(denom)   ' 0..1
        Dim thumbX As Integer = track.Left + CInt(Math.Round(t * movablePx, MidpointRounding.AwayFromZero))

        Return New Rectangle(thumbX, track.Top, thumbLen, track.Height)
    End Function

    Private Function ThumbPixelLength(trackWidth As Integer) As Integer
        Dim span As Integer = Math.Max(0, _maximum - _minimum)
        Dim page As Integer = Math.Min(_largeChange, span + 1)
        If span <= 0 Then Return Math.Max(ThumbMinPx, CInt(trackWidth * 0.5))

        Dim fraction As Double = page / CDbl(span + 1)
        Dim px As Integer = CInt(Math.Round(trackWidth * fraction, MidpointRounding.AwayFromZero))
        Return Math.Max(ThumbMinPx, Math.Min(px, Math.Max(ThumbMinPx, CInt(trackWidth * 0.9))))
    End Function

    Private Function Inflate(r As Rectangle, dx As Integer, dy As Integer) As Rectangle
        Return Rectangle.FromLTRB(r.Left + dx, r.Top + dy, r.Right - dx, r.Bottom - dy)
    End Function

    Private Sub DrawArrowButton(gfx As Grafix, r As Rectangle, scheme As ScrollColorScheme, leftArrow As Boolean, enabled As Boolean, hover As Boolean, pressed As Boolean)
        ' Hintergrund
        Dim topCol As Color = scheme.BtnGradTop
        Dim botCol As Color = scheme.BtnGradBottom
        Dim borderCol As Color = scheme.BtnBorder
        If pressed Then
            topCol = scheme.BtnGradTopPressed : botCol = scheme.BtnGradBottomPressed : borderCol = scheme.BtnBorderPressed
        ElseIf hover Then
            topCol = scheme.BtnGradTopHover : botCol = scheme.BtnGradBottomHover : borderCol = scheme.BtnBorderHover
        End If

        Dim inner As Rectangle = Inflate(r, -1, -1)
        Using lg As New LinearGradientBrush(inner, topCol, botCol, LinearGradientMode.Vertical),
              bd As New Pen(borderCol)
            gfx.FillRectangle(lg, inner)
            gfx.DrawRectangle(bd, Rectangle.FromLTRB(r.Left, r.Top, r.Right - 1, r.Bottom - 1))
        End Using

        ' Pfeilsymbol (einfaches Dreieck)
        Dim cx As Integer = r.Left + r.Width \ 2
        Dim cy As Integer = r.Top + r.Height \ 2
        Dim w As Integer = Math.Max(6, r.Width \ 3)
        Dim h As Integer = Math.Max(8, r.Height \ 3)

        Dim pts As Point()
        If leftArrow Then
            pts = {New Point(cx + w \ 2, cy - h \ 2),
                    New Point(cx + w \ 2, cy + h \ 2),
                    New Point(cx - w \ 2, cy)}
        Else
            pts = {New Point(cx - w \ 2, cy - h \ 2),
                    New Point(cx - w \ 2, cy + h \ 2),
                    New Point(cx + w \ 2, cy)}
        End If

        Dim arrowCol As Color = If(enabled, scheme.BtnGlyph, WithAlpha(scheme.BtnGlyph, 120))
        Using br As New SolidBrush(arrowCol), pn As New Pen(WithAlpha(Color.Black, 60))
            gfx.FillPolygon(br, pts)
            gfx.DrawPolygon(pn, pts)
        End Using
    End Sub

    ' ── Farbschema ───────────────────────────────────────────────────────────
    Private Structure ScrollColorScheme
        ' Track
        Public Track As Color
        Public TrackHighlight As Color
        Public TrackShadow As Color

        ' Thumb
        Public ThumbBorder As Color
        Public ThumbGradTop As Color
        Public ThumbGradBottom As Color
        Public ThumbShadow As Color
        Public GripperLight As Color
        Public GripperDark As Color
        Public ThumbBorderHover As Color
        Public ThumbGradTopHover As Color
        Public ThumbGradBottomHover As Color
        Public ThumbBorderPressed As Color
        Public ThumbGradTopPressed As Color
        Public ThumbGradBottomPressed As Color

        ' Buttons
        Public BtnGradTop As Color
        Public BtnGradBottom As Color
        Public BtnBorder As Color
        Public BtnGradTopHover As Color
        Public BtnGradBottomHover As Color
        Public BtnBorderHover As Color
        Public BtnGradTopPressed As Color
        Public BtnGradBottomPressed As Color
        Public BtnBorderPressed As Color
        Public BtnGlyph As Color
    End Structure

    Private Function BuildScheme(baseColor As Color, enabled As Boolean) As ScrollColorScheme
        Dim lum As Double = Luminance(baseColor)
        Dim isDarkBase As Boolean = lum < 0.5

        ' Track
        Dim track As Color = If(isDarkBase, Lighten(Desaturate(baseColor, 0.25), 0.08), Darken(Desaturate(baseColor, 0.25), 0.08))
        Dim trackHi As Color = If(isDarkBase, Lighten(track, 0.08), Lighten(track, 0.02))
        Dim trackLo As Color = If(isDarkBase, Darken(track, 0.12), Darken(track, 0.18))

        ' Thumb (normal / Hover / Pressed)
        Dim thumbTop As Color = If(isDarkBase, Lighten(baseColor, 0.2), Darken(baseColor, 0.12))
        Dim thumbBot As Color = If(isDarkBase, Lighten(baseColor, 0.08), Darken(baseColor, 0.24))
        Dim thumbBorder As Color = If(isDarkBase, Darken(baseColor, 0.35), Darken(baseColor, 0.45))
        Dim thumbShadow As Color = WithAlpha(Blend(Color.Black, baseColor, 0.85), 60)

        Dim thumbTopHover As Color = If(isDarkBase, Lighten(baseColor, 0.28), Darken(baseColor, 0.18))
        Dim thumbBotHover As Color = If(isDarkBase, Lighten(baseColor, 0.16), Darken(baseColor, 0.3))
        Dim thumbBorderHover As Color = If(isDarkBase, Darken(baseColor, 0.42), Darken(baseColor, 0.52))

        Dim thumbTopPressed As Color = If(isDarkBase, Lighten(baseColor, 0.12), Darken(baseColor, 0.28))
        Dim thumbBotPressed As Color = If(isDarkBase, Lighten(baseColor, 0.02), Darken(baseColor, 0.4))
        Dim thumbBorderPressed As Color = If(isDarkBase, Darken(baseColor, 0.5), Darken(baseColor, 0.6))

        ' Buttons (in Anlehnung an Thumb, aber etwas neutraler)
        Dim btnTop As Color = If(isDarkBase, Lighten(Desaturate(baseColor, 0.3), 0.15), Darken(Desaturate(baseColor, 0.3), 0.1))
        Dim btnBot As Color = If(isDarkBase, Lighten(Desaturate(baseColor, 0.3), 0.05), Darken(Desaturate(baseColor, 0.3), 0.22))
        Dim btnBorder As Color = If(isDarkBase, Darken(baseColor, 0.45), Darken(baseColor, 0.55))
        Dim btnTopHover As Color = If(isDarkBase, Lighten(btnTop, 0.08), Darken(btnTop, 0.08))
        Dim btnBotHover As Color = If(isDarkBase, Lighten(btnBot, 0.08), Darken(btnBot, 0.08))
        Dim btnBorderHover As Color = If(isDarkBase, Darken(btnBorder, 0.05), Darken(btnBorder, 0.05))
        Dim btnTopPressed As Color = If(isDarkBase, Darken(btnTop, 0.12), Darken(btnTop, 0.18))
        Dim btnBotPressed As Color = If(isDarkBase, Darken(btnBot, 0.18), Darken(btnBot, 0.26))
        Dim btnBorderPressed As Color = If(isDarkBase, Darken(btnBorder, 0.12), Darken(btnBorder, 0.12))
        Dim btnGlyph As Color = If(isDarkBase, WithAlpha(Color.White, 220), WithAlpha(Color.Black, 220))

        If Not enabled Then
            track = WithAlpha(Desaturate(track, 0.6), 180)
            thumbTop = WithAlpha(Desaturate(thumbTop, 0.6), 160)
            thumbBot = WithAlpha(Desaturate(thumbBot, 0.6), 160)
            thumbBorder = WithAlpha(Desaturate(thumbBorder, 0.6), 160)
            trackHi = WithAlpha(trackHi, 160)
            trackLo = WithAlpha(trackLo, 160)
            thumbShadow = WithAlpha(thumbShadow, 30)

            ' Buttons „entkräftet“
            btnTop = WithAlpha(Desaturate(btnTop, 0.6), 150)
            btnBot = WithAlpha(Desaturate(btnBot, 0.6), 150)
            btnBorder = WithAlpha(Desaturate(btnBorder, 0.6), 150)
            btnTopHover = btnTop : btnBotHover = btnBot : btnBorderHover = btnBorder
            btnTopPressed = btnTop : btnBotPressed = btnBot : btnBorderPressed = btnBorder
            btnGlyph = WithAlpha(btnGlyph, 120)

            thumbTopHover = thumbTop : thumbBotHover = thumbBot : thumbBorderHover = thumbBorder
            thumbTopPressed = thumbTop : thumbBotPressed = thumbBot : thumbBorderPressed = thumbBorder
        End If

        Dim s As ScrollColorScheme
        s.Track = track : s.TrackHighlight = trackHi : s.TrackShadow = trackLo

        s.ThumbBorder = thumbBorder : s.ThumbGradTop = thumbTop : s.ThumbGradBottom = thumbBot
        s.ThumbShadow = thumbShadow : s.GripperLight = WithAlpha(Color.White, 140) : s.GripperDark = WithAlpha(Color.Black, 140)
        s.ThumbBorderHover = thumbBorderHover : s.ThumbGradTopHover = thumbTopHover : s.ThumbGradBottomHover = thumbBotHover
        s.ThumbBorderPressed = thumbBorderPressed : s.ThumbGradTopPressed = thumbTopPressed : s.ThumbGradBottomPressed = thumbBotPressed

        s.BtnGradTop = btnTop : s.BtnGradBottom = btnBot : s.BtnBorder = btnBorder
        s.BtnGradTopHover = btnTopHover : s.BtnGradBottomHover = btnBotHover : s.BtnBorderHover = btnBorderHover
        s.BtnGradTopPressed = btnTopPressed : s.BtnGradBottomPressed = btnBotPressed : s.BtnBorderPressed = btnBorderPressed
        s.BtnGlyph = btnGlyph

        Return s
    End Function

    ' ── Farb-Utilities ───────────────────────────────────────────────────────
    Private Function WithAlpha(c As Color, a As Integer) As Color
        Return Color.FromArgb(Math.Max(0, Math.Min(255, a)), c.R, c.G, c.B)
    End Function

    Private Function Clamp01(x As Double) As Double
        If x < 0.0 Then Return 0.0
        If x > 1.0 Then Return 1.0
        Return x
    End Function

    Private Function Luminance(c As Color) As Double
        Return (0.2126 * (c.R / 255.0)) + (0.7152 * (c.G / 255.0)) + (0.0722 * (c.B / 255.0))
    End Function

    Private Function Blend(fg As Color, bg As Color, alpha As Double) As Color
        alpha = Clamp01(alpha)
        Dim ia As Double = 1.0 - alpha
        Dim r As Integer = CInt(fg.R * alpha + bg.R * ia)
        Dim g As Integer = CInt(fg.G * alpha + bg.G * ia)
        Dim b As Integer = CInt(fg.B * alpha + bg.B * ia)
        Return Color.FromArgb(255, r, g, b)
    End Function

    Private Function Desaturate(c As Color, amount As Double) As Color
        amount = Clamp01(amount)
        Dim gray As Integer = CInt(0.299 * c.R + 0.587 * c.G + 0.114 * c.B)
        Dim r As Integer = CInt(c.R + (gray - c.R) * amount)
        Dim g As Integer = CInt(c.G + (gray - c.G) * amount)
        Dim b As Integer = CInt(c.B + (gray - c.B) * amount)
        Return Color.FromArgb(c.A, r, g, b)
    End Function

    Private Function Lighten(c As Color, amount As Double) As Color
        amount = Clamp01(amount)
        Dim r As Integer = CInt(c.R + (255 - c.R) * amount)
        Dim g As Integer = CInt(c.G + (255 - c.G) * amount)
        Dim b As Integer = CInt(c.B + (255 - c.B) * amount)
        Return Color.FromArgb(c.A, r, g, b)
    End Function

    Private Function Darken(c As Color, amount As Double) As Color
        amount = Clamp01(amount)
        Dim r As Integer = CInt(c.R * (1.0 - amount))
        Dim g As Integer = CInt(c.G * (1.0 - amount))
        Dim b As Integer = CInt(c.B * (1.0 - amount))
        Return Color.FromArgb(c.A, r, g, b)
    End Function

End Class
