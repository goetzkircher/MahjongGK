Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

Imports System.ComponentModel
Imports System.Drawing.Drawing2D
Imports System.Drawing.Imaging

<DefaultEvent("Click")>
Public Class ButtonInfo
    Inherits Control

    ' ─────────────────────────────────────────────────────────────────────────────
    ' Felder
    ' ─────────────────────────────────────────────────────────────────────────────
    Private _infoText As String = "Dummytext. InfoText zuweisen."
    Private _infoHeader As String = "Info"
    Private _darkMode As Boolean = False
    Private _hover As Boolean = False
    Private _pressed As Boolean = False
    Private _autoSquare As Boolean = True

    ' Cache für gerenderte Symbole (pro Größe & DarkMode)
    Private Shared ReadOnly _iconCache As New Dictionary(Of String, Bitmap)(StringComparer.Ordinal)

    ' ─────────────────────────────────────────────────────────────────────────────
    ' Konstruktor
    ' ─────────────────────────────────────────────────────────────────────────────
    Public Sub New()

        Me.SetStyle(ControlStyles.AllPaintingInWmPaint Or
            ControlStyles.OptimizedDoubleBuffer Or
            ControlStyles.UserPaint Or
            ControlStyles.ResizeRedraw, True)

        Me.TabStop = False
        Me.Cursor = Cursors.Hand
        Me.MinimumSize = New Size(16, 16)
        Me.Size = New Size(26, 26)
        Me.BackColor = SystemColors.Control

        ' DarkMode Standard aus globaler INI beziehen (falls vorhanden)
        Try
            ' Erwartet: INI.Global_DarkMode As Boolean in deinem Projekt
            _darkMode = INI.Global_DarkMode
        Catch
            _darkMode = False
        End Try

        Me.AccessibleName = "Info"
        Me.AccessibleDescription = "Zeigt Informationen."
    End Sub

    ' ─────────────────────────────────────────────────────────────────────────────
    ' Öffentliche Eigenschaften
    ' ─────────────────────────────────────────────────────────────────────────────
    <Category("Appearance"), Description("Text, der in der MessageBox angezeigt wird.")>
    <Editor(GetType(System.ComponentModel.Design.MultilineStringEditor),
        GetType(System.Drawing.Design.UITypeEditor))>
    Public Property InfoText As String
        Get
            Return _infoText
        End Get
        Set(value As String)
            If value Is Nothing Then value = String.Empty
            _infoText = value
        End Set
    End Property

    <Category("Appearance"), Description("Titel/Überschrift der MessageBox.")>
    Public Property InfoHeader As String
        Get
            Return _infoHeader
        End Get
        Set(value As String)
            If value Is Nothing Then value = String.Empty
            _infoHeader = value
        End Set
    End Property

    <Category("Behavior"), Description("Erzwingt DarkMode-Darstellung (überschreibt INI.Global_DarkMode).")>
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

    <Category("Layout"), Description("Hält den Button quadratisch (Breite = Höhe).")>
    Public Property AutoSquare As Boolean
        Get
            Return _autoSquare
        End Get
        Set(value As Boolean)
            _autoSquare = value
            Me.Invalidate()
        End Set
    End Property

    ' ─────────────────────────────────────────────────────────────────────────────
    ' Layout: quadratisch halten
    ' ─────────────────────────────────────────────────────────────────────────────
    Protected Overrides Sub OnResize(e As EventArgs)
        MyBase.OnResize(e)
        If _autoSquare Then
            Dim s As Integer = Math.Max(Me.Width, Me.Height)
            If s <> Me.Width OrElse s <> Me.Height Then
                ' setze auf Quadrat mit maximaler Kante (wir bleiben „groß genug“)
                Me.Size = New Size(s, s)
            End If
        End If
        Me.Invalidate()
    End Sub

    ' ─────────────────────────────────────────────────────────────────────────────
    ' Interaktion
    ' ─────────────────────────────────────────────────────────────────────────────
    Protected Overrides Sub OnMouseEnter(e As EventArgs)
        _hover = True
        Me.Invalidate()
        MyBase.OnMouseEnter(e)
    End Sub

    Protected Overrides Sub OnMouseLeave(e As EventArgs)
        _hover = False
        _pressed = False
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
            ' Click auslösen
            Me.OnClick(EventArgs.Empty)
            ' Standardaktion: MessageBox anzeigen
            If Not String.IsNullOrEmpty(_infoText) Then
                MessageBoxFormatiert.Show(_infoText, If(String.IsNullOrEmpty(_infoHeader), "Info", _infoHeader), MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        End If
        MyBase.OnMouseUp(e)
    End Sub

    ' ─────────────────────────────────────────────────────────────────────────────
    ' Rendering
    ' ─────────────────────────────────────────────────────────────────────────────
    Protected Overrides Sub OnPaintBackground(e As PaintEventArgs)
        e.Graphics.Clear(Me.BackColor)
    End Sub

    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        MyBase.OnPaint(e)

        Dim g As Graphics = e.Graphics
        g.SmoothingMode = SmoothingMode.AntiAlias
        g.InterpolationMode = InterpolationMode.HighQualityBicubic
        g.PixelOffsetMode = PixelOffsetMode.HighQuality

        Dim rect As Rectangle = Me.ClientRectangle
        If rect.Width <= 0 OrElse rect.Height <= 0 Then Return

        ' Hintergrund (leichtes Hover/Pressed)
        Dim bg As Color
        Dim border As Color
        If _darkMode Then
            bg = If(_pressed, Color.FromArgb(80, 255, 255, 255),
                 If(_hover, Color.FromArgb(40, 255, 255, 255), Color.FromArgb(20, 255, 255, 255)))
            border = Color.FromArgb(120, 255, 255, 255)
        Else
            Const COL As Integer = 128
            bg = If(_pressed, Color.FromArgb(90, COL, COL, COL),
                 If(_hover, Color.FromArgb(40, COL, COL, COL), Color.FromArgb(20, COL, COL, COL)))
            border = Color.FromArgb(150, COL, COL, COL)
        End If

        Using br As New SolidBrush(bg)
            g.FillRectangle(br, rect)
        End Using

        ' ''Der Rahmen um das Icon ist deaktiviert, sieht m.E. nicht gut aus
        ''Using pen As New Pen(border, 1.0F)
        ''    g.DrawRectangle(pen, 0, 0, rect.Width - 1, rect.Height - 1)
        ''End Using

        ' Info-Icon zentral zeichnen
        Dim iconPadding As Integer = Math.Max(2, CInt(Math.Min(rect.Width, rect.Height) * 0.12))
        Dim iconSize As Integer = Math.Max(12, Math.Min(rect.Width, rect.Height) - 2 * iconPadding)
        Dim icon As Bitmap = GetInfoIcon(iconSize, _darkMode)
        Try
            Dim x As Integer = rect.Left + (rect.Width - icon.Width) \ 2
            Dim y As Integer = rect.Top + (rect.Height - icon.Height) \ 2
            g.DrawImage(icon, x, y, icon.Width, icon.Height)
        Finally
            icon.Dispose() ' wir bekommen eine Clone()-Kopie
        End Try
    End Sub

    ' ─────────────────────────────────────────────────────────────────────────────
    ' Icon-Rendering (mit Cache)
    ' ─────────────────────────────────────────────────────────────────────────────
    Private Shared Function GetInfoIcon(ByVal size As Integer, ByVal dark As Boolean) As Bitmap

        If size < 12 Then size = 12
        Dim key As String = size.ToString() & If(dark, "D", "L")

        Dim cached As Bitmap = Nothing
        If _iconCache.TryGetValue(key, cached) Then
            Return DirectCast(cached.Clone(), Bitmap)
        End If

        Dim bmp As New Bitmap(size, size, PixelFormat.Format32bppArgb)
        Using g As Graphics = Graphics.FromImage(bmp)
            g.SmoothingMode = SmoothingMode.AntiAlias
            g.InterpolationMode = InterpolationMode.HighQualityBicubic
            g.PixelOffsetMode = PixelOffsetMode.HighQuality

            Dim fill As Color
            Dim border As Color
            Dim glyph As Color = Color.White

            If dark Then
                fill = Color.DodgerBlue
                'border = Color.FromArgb(220, Color.WhiteSmoke)
                border = Color.SteelBlue
            Else
                fill = Color.RoyalBlue
                border = Color.Black
            End If

            Dim pad As Single = CSng(Math.Max(1.0, size * 0.1F))
            Dim rc As New RectangleF(pad, pad, size - 2 * pad, size - 2 * pad)

            Using br As New SolidBrush(fill)
                g.FillEllipse(br, rc)
            End Using
            Using pen As New Pen(border, CSng(Math.Max(1.0, size * 0.1F)))
                g.DrawEllipse(pen, rc)
            End Using

            ' Randbreite wie beim Kreis
            Dim penW As Single = CSng(Math.Max(1.0F, size * 0.06F))

            ' Innenbereich des Kreises für das Glyph
            Dim glyphMargin As Single = CSng(Math.Max(1.0F, size * 0.04F))
            Dim rcInner As RectangleF = rc
            rcInner.Inflate(-penW / 2.0F, -penW / 2.0F)   ' innerhalb der Kreislinie
            rcInner.Inflate(-glyphMargin, -glyphMargin)   ' Sicherheitsrand

            ' === Glyph „i“ ================================================================
            Using brGlyph As New SolidBrush(glyph)
                ' "1 Gerätepixel" (DPI-korrekt)
                Dim px As Single = CSng(96.0F / g.DpiY)

                ' Proportionen + Mindestabstände
                Dim minGap As Single = Math.Max(2.0F * px, rcInner.Height * 0.06F) ' Punkt↔Stiel & Stiel↔Kreis
                Dim dotD As Single = Math.Max(2.0F * px, rcInner.Height * 0.2F)  ' Punkt ~20% der Innenhöhe
                Dim stemW As Single = Math.Max(2.0F * px, rcInner.Width * 0.16F)  ' Stielbreite ~16%
                Dim wantStemH As Single = Math.Max(4.0F * px, rcInner.Height * 0.46F)

                ' --- Punkt (leicht oberhalb Mitte), dann ALLES 1px nach oben schieben ---
                Dim dotCenterY As Single = rcInner.Top + rcInner.Height * 0.3F
                Dim dotX As Single = rcInner.Left + (rcInner.Width - dotD) / 2.0F
                Dim dotY As Single = dotCenterY - dotD / 2.0F

                ' 1 px nach oben (optischer Wunsch)
                dotY -= px

                ' Clamp: Punkt innerhalb rcInner
                If dotY < rcInner.Top Then dotY = rcInner.Top
                If dotY + dotD > rcInner.Bottom - (minGap + px) Then
                    dotY = rcInner.Bottom - (minGap + px) - dotD
                End If

                g.FillEllipse(brGlyph, dotX, dotY, dotD, dotD)

                ' --- Stiel ---
                Dim yTop As Single = dotY + dotD + minGap
                yTop -= px ' gesamter Block leicht nach oben

                Dim yBottomMax As Single = rcInner.Bottom - minGap
                Dim stemH As Single = Math.Min(wantStemH, Math.Max(0.0F, yBottomMax - yTop))

                ' Untere Kappe 1 px länger
                If yTop + stemH + px <= yBottomMax Then
                    stemH += px
                End If
                If stemH < stemW Then stemH = stemW

                Dim stemX As Single = rcInner.Left + (rcInner.Width - stemW) / 2.0F
                Dim capD As Single = stemW
                If stemH < capD Then capD = stemH

                g.FillEllipse(brGlyph, stemX, yTop, capD, capD)                                ' obere Kappe
                Dim midH As Single = Math.Max(0.0F, stemH - capD)
                If midH > 0.0F Then
                    g.FillRectangle(brGlyph, stemX, yTop + capD / 2.0F, stemW, midH)          ' Mittelstück
                End If
                g.FillEllipse(brGlyph, stemX, yTop + stemH - capD, capD, capD)                ' untere Kappe
            End Using


        End Using

        _iconCache(key) = bmp
        Return DirectCast(bmp.Clone(), Bitmap)
    End Function

    ' ─────────────────────────────────────────────────────────────────────────────
    ' Hilfen
    ' ─────────────────────────────────────────────────────────────────────────────
    Private Function ParentBackColor() As Color
        Dim p As Control = Me.Parent
        If p IsNot Nothing Then Return p.BackColor
        Return SystemColors.Control
    End Function

    Protected Overrides Sub Dispose(disposing As Boolean)
        MyBase.Dispose(disposing)
        If disposing Then
            For Each kvp As KeyValuePair(Of String, Bitmap) In _iconCache
                kvp.Value.Dispose()
            Next
            _iconCache.Clear()
        End If
    End Sub
End Class

