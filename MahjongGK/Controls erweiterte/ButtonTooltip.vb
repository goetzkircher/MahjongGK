Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

Imports System.ComponentModel
Imports System.Drawing.Drawing2D
Imports System.Drawing.Imaging

<DefaultEvent("Click")>
Public Class ButtonTooltip
    Inherits Control

    ' ──────────────────────────────────────────────────────────────────────────
    ' Felder
    ' ──────────────────────────────────────────────────────────────────────────
    Private _infoText As String = "Dummytext. InfoText zuweisen."
    Private _infoHeader As String = "Info"
    Private _darkMode As Boolean = False
    Private _showOnFocus As Boolean = False
    Private _hover As Boolean
    Private _pressed As Boolean
    Private _autoSquare As Boolean = True

    Private ReadOnly _tt As New ToolTip()               ' eigener Tooltip
    Private _autoShowOnHover As Boolean = True
    Private _toolTipIcon As ToolTipIcon = ToolTipIcon.Info
    Private _toolTipDurationMs As Integer = 4000        ' Show()-Dauer

    ' Cache für gerenderte Symbole (pro Größe & DarkMode-ignoriert)
    Private Shared ReadOnly _iconCache As New Dictionary(Of String, Bitmap)(StringComparer.Ordinal)

    ' ──────────────────────────────────────────────────────────────────────────
    ' Konstruktor
    ' ──────────────────────────────────────────────────────────────────────────
    Public Sub New()
        Me.SetStyle(ControlStyles.AllPaintingInWmPaint Or
                    ControlStyles.OptimizedDoubleBuffer Or
                    ControlStyles.UserPaint Or
                    ControlStyles.ResizeRedraw, True)

        Me.TabStop = False
        Me.Cursor = Cursors.Default
        Me.MinimumSize = New Size(16, 16)
        Me.Size = New Size(26, 26)
        Me.BackColor = SystemColors.Control

        ' DarkMode aus globaler INI (falls vorhanden)
        Try
            _darkMode = INI.Global_DarkMode
        Catch
            _darkMode = False
        End Try

        ' Tooltip-Defaults
        _tt.IsBalloon = True
        _tt.ToolTipIcon = _toolTipIcon
        _tt.ToolTipTitle = _infoHeader
        _tt.InitialDelay = 400
        _tt.ReshowDelay = 100
        _tt.UseFading = True
        _tt.UseAnimation = True
        _tt.InitialDelay = HoverDelayMs
        _tt.ReshowDelay = Math.Max(100, HoverDelayMs \ 3)
        _tt.AutoPopDelay = Math.Max(1000, _toolTipDurationMs)
        _tt.ShowAlways = False
        _tt.SetToolTip(Me, _infoText) ' Standard-Hover-Tooltip

        ' Systemverhalten (Standard) vorbereiten
        _tt.SetToolTip(Me, _infoText)

        Me.AccessibleName = "Info"
        Me.AccessibleDescription = "Zeigt einen Hinweis-Tooltip."
    End Sub

    Protected Overrides Sub Dispose(disposing As Boolean)
        If disposing Then
            _tt.Dispose()
            For Each kvp As KeyValuePair(Of String, Bitmap) In _iconCache
                kvp.Value.Dispose()
            Next
            _iconCache.Clear()
        End If
        MyBase.Dispose(disposing)
    End Sub

    ' ──────────────────────────────────────────────────────────────────────────
    ' Öffentlich sichtbare Eigenschaften
    ' ──────────────────────────────────────────────────────────────────────────
    <Category("Appearance"),
     Description("Text, der im Tooltip angezeigt wird.")>
    <Editor(GetType(System.ComponentModel.Design.MultilineStringEditor),
            GetType(System.Drawing.Design.UITypeEditor))>
    Public Property InfoText As String
        Get
            Return _infoText
        End Get
        Set(value As String)
            If value Is Nothing Then value = String.Empty
            _infoText = value
            _tt.SetToolTip(Me, _infoText)
        End Set
    End Property

    <Category("Appearance"),
     Description("Titel/Überschrift des Tooltips.")>
    Public Property InfoHeader As String
        Get
            Return _infoHeader
        End Get
        Set(value As String)
            If value Is Nothing Then value = String.Empty
            _infoHeader = value
            _tt.ToolTipTitle = _infoHeader
        End Set
    End Property

    <Category("Behavior"),
 Description("Zeigt den Tooltip an, wenn das Control den Fokus erhält (Tastatur/Maus).")>
    <DefaultValue(True)>
    Public Property ShowOnFocus As Boolean
        Get
            Return _showOnFocus
        End Get
        Set(value As Boolean)
            _showOnFocus = value
        End Set
    End Property


    <Category("Behavior"),
     Description("Erzwingt DarkMode-Darstellung (nur Hover/Pressed-Hintergrund).")>
    Public Property DarkMode As Boolean
        Get
            Return _darkMode
        End Get
        Set(value As Boolean)
            If _darkMode <> value Then
                _darkMode = value
                Me.Invalidate()
            End If
        End Set
    End Property

    <Category("Behavior"),
     Description("Zeigt den Tooltip automatisch beim Überfahren mit der Maus an.")>
    <DefaultValue(True)>
    Public Property AutoShowOnHover As Boolean
        Get
            Return _autoShowOnHover
        End Get
        Set(value As Boolean)
            _autoShowOnHover = value
        End Set
    End Property

    <Category("Behavior"),
     Description("Balloon-Optik des Tooltips.")>
    <DefaultValue(True)>
    Public Property ToolTipIsBalloon As Boolean
        Get
            Return _tt.IsBalloon
        End Get
        Set(value As Boolean)
            _tt.IsBalloon = value
        End Set
    End Property

    <Category("Behavior"),
     Description("Icon des Tooltips.")>
    <DefaultValue(GetType(ToolTipIcon), "Info")>
    Public Property ToolTipIcon As ToolTipIcon
        Get
            Return _toolTipIcon
        End Get
        Set(value As ToolTipIcon)
            _toolTipIcon = value
            _tt.ToolTipIcon = value
        End Set
    End Property

    <Category("Behavior"),
     Description("Anzeigedauer des sofort eingeblendeten Tooltips in Millisekunden.")>
    <DefaultValue(4000)>
    Public Property ToolTipDurationMs As Integer
        Get
            Return _toolTipDurationMs
        End Get
        Set(value As Integer)
            _toolTipDurationMs = Math.Max(500, value)
            _tt.AutoPopDelay = _toolTipDurationMs
        End Set
    End Property
    <Category("Behavior"),
    Description("Verzögerung (ms) bis der Tooltip beim Hover erscheint.")>
    <DefaultValue(330)>
    Public Property HoverDelayMs As Integer = 330


    <Category("Layout"),
     Description("Hält den Button quadratisch (Breite = Höhe).")>
    <DefaultValue(True)>
    Public Property AutoSquare As Boolean
        Get
            Return _autoSquare
        End Get
        Set(value As Boolean)
            _autoSquare = value
            Me.Invalidate()
        End Set
    End Property

    ' ──────────────────────────────────────────────────────────────────────────
    ' Layout
    ' ──────────────────────────────────────────────────────────────────────────
    Protected Overrides Sub OnResize(e As EventArgs)
        MyBase.OnResize(e)
        If _autoSquare Then
            Dim s As Integer = Math.Max(Me.Width, Me.Height)
            If s <> Me.Width OrElse s <> Me.Height Then
                Me.Size = New Size(s, s)
            End If
        End If
        Me.Invalidate()
    End Sub

    ' ──────────────────────────────────────────────────────────────────────────
    ' Interaktion: Hover zeigt Tooltip (optional), Klick zeigt „sofort“-Tooltip
    ' ──────────────────────────────────────────────────────────────────────────
    Protected Overrides Sub OnMouseEnter(e As EventArgs)
        _hover = True
        Me.Invalidate()
        ' Nur Titel/Icon aktualisieren, Anzeige übernimmt der ToolTip nach InitialDelay automatisch
        _tt.ToolTipTitle = _infoHeader
        _tt.ToolTipIcon = _toolTipIcon
        MyBase.OnMouseEnter(e)
    End Sub

    Protected Overrides Sub OnMouseLeave(e As EventArgs)
        _hover = False
        _pressed = False
        _tt.Hide(Me)
        Me.Invalidate()
        MyBase.OnMouseLeave(e)
    End Sub

    Protected Overrides Sub OnMouseDown(e As MouseEventArgs)
        If e.Button = MouseButtons.Left Then
            _pressed = True
            Me.Invalidate()
        End If
        MyBase.OnMouseDown(e)
    End Sub

    Protected Overrides Sub OnMouseUp(e As MouseEventArgs)
        If _pressed AndAlso e.Button = MouseButtons.Left Then
            _pressed = False
            Me.Invalidate()
            If Not String.IsNullOrEmpty(_infoText) Then
                _tt.ToolTipTitle = _infoHeader
                _tt.ToolTipIcon = _toolTipIcon
                _tt.Show(_infoText, Me, Me.Width \ 2, Me.Height, _toolTipDurationMs)
            End If
            Me.OnClick(EventArgs.Empty)
        End If
        MyBase.OnMouseUp(e)
    End Sub


    Protected Overrides Sub OnGotFocus(e As EventArgs)
        MyBase.OnGotFocus(e)
        If _showOnFocus AndAlso Not String.IsNullOrEmpty(_infoText) Then
            _tt.ToolTipTitle = _infoHeader
            _tt.ToolTipIcon = _toolTipIcon
            _tt.Show(_infoText, Me, Me.Width \ 2, Me.Height, _toolTipDurationMs)
        End If
    End Sub

    Protected Overrides Sub OnLostFocus(e As EventArgs)
        _tt.Hide(Me)
        MyBase.OnLostFocus(e)
    End Sub

    ' ──────────────────────────────────────────────────────────────────────────
    ' Rendering
    ' ──────────────────────────────────────────────────────────────────────────
    Protected Overrides Sub OnPaintBackground(e As PaintEventArgs)
        Dim bg As Color = If(Me.Parent IsNot Nothing, Me.Parent.BackColor, SystemColors.Control)
        e.Graphics.Clear(bg)
    End Sub


    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        MyBase.OnPaint(e)

        Dim g As Graphics = e.Graphics
        g.SmoothingMode = SmoothingMode.AntiAlias
        g.InterpolationMode = InterpolationMode.HighQualityBicubic
        g.PixelOffsetMode = PixelOffsetMode.HighQuality

        Dim rect As Rectangle = Me.ClientRectangle
        If rect.Width <= 0 OrElse rect.Height <= 0 Then Return

        ' Hover/Pressed-Overlay
        Dim overlay As Color
        Dim borderRect As Rectangle = New Rectangle(0, 0, rect.Width - 1, rect.Height - 1)

        If _darkMode Then
            overlay = If(_pressed, Color.FromArgb(80, 255, 255, 255),
                      If(_hover, Color.FromArgb(40, 255, 255, 255), Color.FromArgb(20, 255, 255, 255)))
        Else
            Const COL As Integer = 128
            overlay = If(_pressed, Color.FromArgb(90, COL, COL, COL),
                      If(_hover, Color.FromArgb(40, COL, COL, COL), Color.FromArgb(20, COL, COL, COL)))
        End If

        Using br As New SolidBrush(overlay)
            g.FillRectangle(br, rect)
        End Using

        ' 'Der Rahmen um das Icon ist deaktivier, sieht m.E. nicht gut aus.
        ''Using pen As New Pen(Color.FromArgb(150, 128, 128, 128), 1.0F)
        ''    g.DrawRectangle(pen, borderRect)
        ''End Using

        ' Info-Icon gemäß Vorgabe:
        ' - 2 px äußerer Rand (Kreislinie)
        ' - innen weiß
        ' - Glyph „i“ blau
        Dim iconPadding As Integer = Math.Max(2, CInt(Math.Min(rect.Width, rect.Height) * 0.12))
        Dim iconSize As Integer = Math.Max(12, Math.Min(rect.Width, rect.Height) - 2 * iconPadding)
        Dim icon As Bitmap = GetInfoIcon_WhiteInsideBlueGlyph(iconSize)
        Try
            Dim x As Integer = rect.Left + (rect.Width - icon.Width) \ 2
            Dim y As Integer = rect.Top + (rect.Height - icon.Height) \ 2
            g.DrawImage(icon, x, y, icon.Width, icon.Height)
        Finally
            icon.Dispose()
        End Try
    End Sub

    ' ──────────────────────────────────────────────────────────────────────────
    ' Icon: 2 px Rand, innen weiß, „i“ in Blau  (Proportionen wie bei ButtonInfo)
    ' ──────────────────────────────────────────────────────────────────────────
    Private Shared Function GetInfoIcon_WhiteInsideBlueGlyph(ByVal size As Integer) As Bitmap
        If size < 12 Then size = 12
        Dim key As String = "WB-" & size.ToString()

        Dim cached As Bitmap = Nothing
        If _iconCache.TryGetValue(key, cached) Then
            Return DirectCast(cached.Clone(), Bitmap)
        End If

        Dim bmp As New Bitmap(size, size, PixelFormat.Format32bppArgb)
        Using g As Graphics = Graphics.FromImage(bmp)
            g.SmoothingMode = SmoothingMode.AntiAlias
            g.InterpolationMode = InterpolationMode.HighQualityBicubic
            g.PixelOffsetMode = PixelOffsetMode.HighQuality

            Dim borderColor As Color = Color.RoyalBlue
            Dim glyph As Color = Color.RoyalBlue
            Dim fill As Color = Color.White

            ' äußerer Kreis: 2 px Rand
            Dim pad As Single = 2.0F
            Dim rc As New RectangleF(pad, pad, size - 2 * pad, size - 2 * pad)

            Using brFill As New SolidBrush(fill)
                g.FillEllipse(brFill, rc)
            End Using
            Using pen As New Pen(borderColor, 2.0F)
                g.DrawEllipse(pen, rc)
            End Using

            ' Innenbereich (für das Glyph) definieren
            Dim rcInner As RectangleF = rc
            rcInner.Inflate(-2.0F, -2.0F) ' etwas Luft zum Rand

            ' === Glyph „i“ (wie in ButtonInfo, nur mit blauer Farbe) ==========
            Using brGlyph As New SolidBrush(glyph)
                Dim px As Single = CSng(96.0F / g.DpiY)

                Dim minGap As Single = Math.Max(2.0F * px, rcInner.Height * 0.06F)
                Dim dotD As Single = Math.Max(2.0F * px, rcInner.Height * 0.2F)
                Dim stemW As Single = Math.Max(2.0F * px, rcInner.Width * 0.16F)
                Dim wantStemH As Single = Math.Max(4.0F * px, rcInner.Height * 0.46F)

                Dim dotCenterY As Single = rcInner.Top + rcInner.Height * 0.3F
                Dim dotX As Single = rcInner.Left + (rcInner.Width - dotD) / 2.0F
                Dim dotY As Single = dotCenterY - dotD / 2.0F
                dotY -= px

                If dotY < rcInner.Top Then dotY = rcInner.Top
                If dotY + dotD > rcInner.Bottom - (minGap + px) Then
                    dotY = rcInner.Bottom - (minGap + px) - dotD
                End If
                g.FillEllipse(brGlyph, dotX, dotY, dotD, dotD)

                Dim yTop As Single = dotY + dotD + minGap
                yTop -= px

                Dim yBottomMax As Single = rcInner.Bottom - minGap
                Dim stemH As Single = Math.Min(wantStemH, Math.Max(0.0F, yBottomMax - yTop))
                If yTop + stemH + px <= yBottomMax Then
                    stemH += px
                End If
                If stemH < stemW Then stemH = stemW

                Dim stemX As Single = rcInner.Left + (rcInner.Width - stemW) / 2.0F
                Dim capD As Single = stemW
                If stemH < capD Then capD = stemH

                g.FillEllipse(brGlyph, stemX, yTop, capD, capD)
                Dim midH As Single = Math.Max(0.0F, stemH - capD)
                If midH > 0.0F Then
                    g.FillRectangle(brGlyph, stemX, yTop + capD / 2.0F, stemW, midH)
                End If
                g.FillEllipse(brGlyph, stemX, yTop + stemH - capD, capD, capD)
            End Using
        End Using

        _iconCache(key) = bmp
        Return DirectCast(bmp.Clone(), Bitmap)
    End Function

End Class

