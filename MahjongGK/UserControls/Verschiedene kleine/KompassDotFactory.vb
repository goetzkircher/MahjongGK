Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On
Imports System.Drawing.Drawing2D
Imports System.Drawing.Imaging

Public Module KompassDotFactory

#Region "Public API"

    Public Enum ControlStatus
        Normal
        MouseOver
        Selected
        Disabled
        NotVisible
    End Enum

    Public Function KompassDot(status As ControlStatus, Optional big As Boolean = False) As Bitmap
        Select Case status
            Case ControlStatus.Normal : Return GetDot(DotState.Normal, big)
            Case ControlStatus.MouseOver : Return GetDot(DotState.MouseOver, big)
            Case ControlStatus.Selected : Return GetDot(DotState.Selected, big)
            Case ControlStatus.Disabled : Return GetDot(DotState.Disabled, big)
            Case ControlStatus.NotVisible : Return KompassDotNotVisible(big)
            Case Else : Return GetDot(DotState.Normal, big)
        End Select
    End Function

    Public Function KompassDot(Optional big As Boolean = False) As Bitmap
        Return GetDot(DotState.Normal, big)
    End Function

    Public Function KompassDotMouseOver(Optional big As Boolean = False) As Bitmap
        Return GetDot(DotState.MouseOver, big)
    End Function

    Public Function KompassDotSelected(Optional big As Boolean = False) As Bitmap
        Return GetDot(DotState.Selected, big)
    End Function

    Public Function KompassDotDisabled(Optional big As Boolean = False) As Bitmap
        Return GetDot(DotState.Disabled, big)
    End Function

    Public Function KompassDotNotVisible(Optional big As Boolean = False) As Bitmap
        Dim sz As Integer = If(big, 24, 16)
        Return New Bitmap(sz, sz, PixelFormat.Format32bppPArgb) ' voll transparent
    End Function

    ' Orientierungspunkt – deutlich kleiner, neutral grau (Light/Dark angepasst)
    Public Function KompassDotCenter(Optional big As Boolean = False) As Bitmap
        Dim isDark As Boolean = INI.Global_DarkMode
        Dim size As Integer = If(big, 24, 16)
        Dim key As String = $"C-{If(isDark, "D", "L")}-{size}"

        Dim cached As Bitmap = Nothing
        If _cache.TryGetValue(key, cached) AndAlso cached IsNot Nothing Then
            Return CType(cached.Clone(), Bitmap)
        End If

        Dim outerDiameter As Single = If(size = 16, 6.0F, 9.0F) ' deutlich kleiner
        Dim strokeW As Single = If(size = 16, 1.0F, 1.2F)

        Dim baseDark As Color = If(isDark, Color.FromArgb(&HFF999999), Color.FromArgb(&HFF505050))
        Dim baseLight As Color = If(isDark, Color.FromArgb(&HFFE8E8E8), Color.FromArgb(&HFFBEBEBE))
        Dim rim As Color = If(isDark, Color.FromArgb(&HFF3A3A3A), Color.FromArgb(&HFF7A7A7A))

        Dim bmp As New Bitmap(size, size, PixelFormat.Format32bppPArgb)
        Using g As Graphics = Graphics.FromImage(bmp)
            g.SmoothingMode = SmoothingMode.AntiAlias
            g.PixelOffsetMode = PixelOffsetMode.HighQuality
            g.CompositingQuality = CompositingQuality.HighQuality

            Dim cx As Single = CSng(size) / 2.0F
            Dim cy As Single = cx
            Dim rect As New RectangleF(cx - outerDiameter / 2.0F, cy - outerDiameter / 2.0F, outerDiameter, outerDiameter)

            ' Füllung mit sanftem Radialglanz (neutral grau)
            Using gp As New GraphicsPath()
                gp.AddEllipse(rect)
                Using pgb As New PathGradientBrush(gp)
                    pgb.CenterPoint = New PointF(rect.Left + rect.Width * 0.4F, rect.Top + rect.Height * 0.4F)
                    pgb.CenterColor = baseLight
                    pgb.SurroundColors = {baseDark}
                    g.FillEllipse(pgb, rect)
                End Using
            End Using

            ' dünner Rand
            Using pen As New Pen(rim, strokeW)
                g.DrawEllipse(pen, rect)
            End Using
        End Using

        _cache(key) = bmp
        Return CType(bmp.Clone(), Bitmap)
    End Function

#End Region

#Region "Internals"

    Private Enum DotState
        Normal
        MouseOver
        Selected
        Disabled
    End Enum

    ' Cache pro (Mode/State/Size) + Center-Varianten
    Private ReadOnly _cache As New Dictionary(Of String, Bitmap)(StringComparer.Ordinal)

    Private Structure Palette
        Public Normal As Color
        Public MouseOver As Color
        Public Selected As Color
        Public Disabled As Color
        Public RimLight As Color
        Public RimDark As Color
        Public Shadow As Color
        Public Specular As Color
        Public Gloss As Color
        Public BackgroundLift As Integer
        ' Fog-Overlay für Disabled (verstärktes „abgeschaltet“)
        Public DisabledFog As Color
    End Structure

    Private Function GetLightPalette() As Palette
#Disable Warning IDE0017 ' Initialisierung von Objekten vereinfachen
        Dim p As New Palette
#Enable Warning IDE0017 ' Initialisierung von Objekten vereinfachen
        ' Vorgaben
        p.Normal = Color.FromArgb(&HFF404040)
        p.Selected = Color.FromArgb(&HFF008000)
        p.MouseOver = Color.FromArgb(&HFF0066CC)
        ' Deutlich abgeschaltet: grauer Grundton
        p.Disabled = Color.FromArgb(&HFF9C9C9C)
        p.RimLight = Color.FromArgb(&HFFFFFFFF)
        p.RimDark = Color.FromArgb(&HFF2A2A2A)
        p.Shadow = Color.FromArgb(90, 0, 0, 0)
        p.Specular = Color.FromArgb(170, 255, 255, 255)
        p.Gloss = Color.FromArgb(80, 255, 255, 255)
        p.DisabledFog = Color.FromArgb(110, 245, 245, 245) ' milchig auf hell
        p.BackgroundLift = 0
        Return p
    End Function

    Private Function GetDarkPalette() As Palette
#Disable Warning IDE0017 ' Initialisierung von Objekten vereinfachen
        Dim p As New Palette
#Enable Warning IDE0017 ' Initialisierung von Objekten vereinfachen
        p.Normal = Color.FromArgb(&HFFD6D6D6)
        p.Selected = Color.FromArgb(&HFF33CC33)
        p.MouseOver = Color.FromArgb(&HFF66B2FF)
        ' Deutlich abgeschaltet auf dark: dunkler, entsättigt
        p.Disabled = Color.FromArgb(&HFF5B5B5B)
        p.RimLight = Color.FromArgb(&HFFFFFFFF)
        p.RimDark = Color.FromArgb(&HFF141414)
        p.Shadow = Color.FromArgb(120, 0, 0, 0)
        p.Specular = Color.FromArgb(180, 255, 255, 255)
        p.Gloss = Color.FromArgb(90, 255, 255, 255)
        p.DisabledFog = Color.FromArgb(100, 0, 0, 0) ' grauer Schleier auf dunkel
        p.BackgroundLift = 6
        Return p
    End Function

    Private Function GetDot(state As DotState, big As Boolean) As Bitmap
        Dim isDark As Boolean = INI.Global_DarkMode
        Dim size As Integer = If(big, 24, 16)
        Dim key As String = (If(isDark, "D", "L")) & "-" & CInt(state).ToString() & "-" & size.ToString()

        Dim bmp As Bitmap = Nothing
        If _cache.TryGetValue(key, bmp) AndAlso bmp IsNot Nothing Then
            Return CType(bmp.Clone(), Bitmap)
        End If

        Dim pal As Palette = If(isDark, GetDarkPalette(), GetLightPalette())
        Dim baseColor As Color
        Dim isDisabled As Boolean = (state = DotState.Disabled)

        Select Case state
            Case DotState.Normal : baseColor = pal.Normal
            Case DotState.MouseOver : baseColor = pal.MouseOver
            Case DotState.Selected : baseColor = pal.Selected
            Case DotState.Disabled : baseColor = pal.Disabled
            Case Else : baseColor = pal.Normal
        End Select

        Dim rendered As Bitmap = RenderDot(size, baseColor, pal, isDisabled)

        _cache(key) = rendered
        Return CType(rendered.Clone(), Bitmap)
    End Function

    Private Function RenderDot(size As Integer, baseColor As Color, pal As Palette, isDisabled As Boolean) As Bitmap
        Dim bmp As New Bitmap(size, size, PixelFormat.Format32bppPArgb)

        Using g As Graphics = Graphics.FromImage(bmp)
            g.SmoothingMode = SmoothingMode.AntiAlias
            g.PixelOffsetMode = PixelOffsetMode.HighQuality
            g.CompositingQuality = CompositingQuality.HighQuality

            Dim pad As Single = If(size >= 24, 1.5F, 1.0F)
            Dim rect As New RectangleF(pad, pad, size - 2 * pad, size - 2 * pad)

            ' Für Disabled: zusätzlich den Farbkontrast reduzieren (sanft grauen)
            Dim fillColor As Color = If(isDisabled, Blend(baseColor, Color.FromArgb(&HFFB0B0B0), 0.55F), baseColor)

            ' 1) Körper – Radialverlauf
            Using path As New GraphicsPath()
                path.AddEllipse(rect)
                Using pgb As New PathGradientBrush(path)
                    pgb.CenterPoint = New PointF(rect.Left + rect.Width * 0.35F, rect.Top + rect.Height * 0.35F)
                    Dim centerC As Color = Lighten(fillColor, CByte(30 + pal.BackgroundLift))
                    Dim surroundC As Color = Darken(fillColor, 30)
                    If isDisabled Then
                        centerC = Blend(centerC, Color.Gray, 0.35F)
                        surroundC = Blend(surroundC, Color.Gray, 0.35F)
                    End If
                    pgb.CenterColor = centerC
                    pgb.SurroundColors = {surroundC}
                    g.FillEllipse(pgb, rect)
                End Using
            End Using

            ' 2) Innen-Schatten
            Dim inset As Single = If(size >= 24, 1.4F, 1.0F)
            Dim innerRect As New RectangleF(rect.X + inset, rect.Y + inset, rect.Width - 2 * inset, rect.Height - 2 * inset)
            Using gp2 As New GraphicsPath()
                gp2.AddEllipse(innerRect)
                Using pgb2 As New PathGradientBrush(gp2)
                    Dim sh As Color = pal.Shadow
                    If isDisabled Then sh = Color.FromArgb(Math.Min(180, sh.A + 30), sh) ' etwas stärker
                    pgb2.CenterColor = Color.FromArgb(0, sh)
                    pgb2.SurroundColors = {sh}
                    g.FillEllipse(pgb2, innerRect)
                End Using
            End Using

            ' 3) Glanzkappe
            Dim glossH As Single = innerRect.Height * 0.55F
            Dim glossRect As New RectangleF(innerRect.X, innerRect.Y, innerRect.Width, glossH)
            Using pathGloss As New GraphicsPath()
                pathGloss.AddEllipse(glossRect)
                Using brushGloss As New PathGradientBrush(pathGloss)
                    Dim glossColor As Color = If(isDisabled, Color.FromArgb(CInt(pal.Gloss.A * 0.5), pal.Gloss), pal.Gloss)
                    brushGloss.CenterPoint = New PointF(glossRect.Left + glossRect.Width * 0.5F, glossRect.Top + glossRect.Height * 0.35F)
                    brushGloss.CenterColor = glossColor
                    brushGloss.SurroundColors = {Color.FromArgb(0, 255, 255, 255)}
                    g.FillEllipse(brushGloss, glossRect)
                End Using
            End Using

            ' 4) Spekular-Highlight
            Dim specW As Single = If(size >= 24, 5.0F, 3.0F)
            Dim specH As Single = If(size >= 24, 3.0F, 2.0F)
            Dim specRect As New RectangleF(innerRect.Left + innerRect.Width * 0.22F,
                                            innerRect.Top + innerRect.Height * 0.18F,
                                            specW, specH)
            Dim specCol As Color = If(isDisabled, Color.FromArgb(CInt(BlendAlpha(pal.Specular.A, 0.45F)), pal.Specular), pal.Specular)
            Using specBrush As New SolidBrush(specCol)
                g.FillEllipse(specBrush, specRect)
            End Using

            ' 5) Außenkontur – oben hell, unten dunkel (bei Disabled abgeschwächt)
            Dim penW As Single = If(size >= 24, 1.3F, 1.0F)
            Dim rimLight As Color = If(isDisabled, Color.FromArgb(CInt(BlendAlpha(pal.RimLight.A, 0.45F)), pal.RimLight), pal.RimLight)
            Dim rimDark As Color = If(isDisabled, Color.FromArgb(CInt(BlendAlpha(pal.RimDark.A, 0.45F)), pal.RimDark), pal.RimDark)
            Using penLight As New Pen(rimLight, penW),
                  penDark As New Pen(rimDark, penW)
                ''g.DrawArc(penLight, RectangleFToRectangle(rect), 200.0F, 160.0F)
                ''g.DrawArc(penDark, RectangleFToRectangle(rect), 20.0F, 160.0F)
                g.DrawArc(penLight, RectangleFToRectangle(rect), 0F, 360.0F)
            End Using

            ' 6) Disabled-Fog – deutlicher „disabled“-Eindruck
            If isDisabled Then
                Using fog As New SolidBrush(pal.DisabledFog)
                    g.FillEllipse(fog, rect)
                End Using
            End If

            'Markierung, daß diese Bitmap nicht aufgehellt werden soll.
            'siehe DARK_MARKER im ThemeManager
            bmp.SetPixel(0, 0, Color.FromArgb(1, 127, 128, 129))

        End Using

        Return bmp
    End Function

    ' ── Helpers ────────────────────────────────────────────────────────────────

    Private Function RectangleFToRectangle(r As RectangleF) As Rectangle
        Return New Rectangle(CInt(Math.Round(r.X)), CInt(Math.Round(r.Y)),
                             CInt(Math.Round(r.Width)), CInt(Math.Round(r.Height)))
    End Function

    Private Function ClampByte(v As Integer) As Byte
        If v < 0 Then Return 0
        If v > 255 Then Return 255
        Return CByte(v)
    End Function

    Private Function Lighten(c As Color, amount As Byte) As Color
        Dim r As Byte = ClampByte(CInt(c.R) + amount)
        Dim g As Byte = ClampByte(CInt(c.G) + amount)
        Dim b As Byte = ClampByte(CInt(c.B) + amount)
        Return Color.FromArgb(c.A, r, g, b)
    End Function

    Private Function Darken(c As Color, amount As Byte) As Color
        Dim r As Byte = ClampByte(CInt(c.R) - amount)
        Dim g As Byte = ClampByte(CInt(c.G) - amount)
        Dim b As Byte = ClampByte(CInt(c.B) - amount)
        Return Color.FromArgb(c.A, r, g, b)
    End Function

    Private Function Blend(a As Color, b As Color, t As Single) As Color
        If t < 0.0F Then t = 0.0F
        If t > 1.0F Then t = 1.0F
        Dim ia As Single = 1.0F - t
        Dim rr As Integer = CInt(a.R * ia + b.R * t)
        Dim gg As Integer = CInt(a.G * ia + b.G * t)
        Dim bb As Integer = CInt(a.B * ia + b.B * t)
        Dim aa As Integer = CInt(a.A * ia + b.A * t)
        Return Color.FromArgb(ClampByte(aa), ClampByte(rr), ClampByte(gg), ClampByte(bb))
    End Function

    Private Function BlendAlpha(alpha As Integer, factor As Single) As Integer
        Dim v As Integer = CInt(Math.Round(alpha * Math.Max(0.0F, Math.Min(1.0F, factor))))
        If v < 0 Then v = 0
        If v > 255 Then v = 255
        Return v
    End Function

#End Region

End Module
