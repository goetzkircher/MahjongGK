Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

Public Enum AppTheme
    Light = 0
    Dark = 1
End Enum

Namespace Theme
    Public NotInheritable Class ThemePalette
        Public ReadOnly Background As Color
        Public ReadOnly Foreground As Color
        Public ReadOnly ControlBack As Color
        Public ReadOnly ControlFore As Color
        Public ReadOnly Border As Color
        Public ReadOnly Accent As Color
        Public ReadOnly AccentHover As Color
        Public ReadOnly SelectionBack As Color
        Public ReadOnly SelectionFore As Color
        Public ReadOnly MenuBack As Color
        Public ReadOnly MenuFore As Color
        Public ReadOnly MenuSelBack As Color
        Public ReadOnly MenuSelBorder As Color
        Public ReadOnly ToolTipBack As Color
        Public ReadOnly ToolTipFore As Color
        Public ReadOnly DisabledFore As Color
        Public ReadOnly LinkColor As Color

        Private Sub New(bg As Color, fg As Color,
                    ctlBg As Color, ctlFg As Color,
                    border As Color, accent As Color, accentHover As Color,
                    selBack As Color, selFore As Color,
                    menuBack As Color, menuFore As Color, menuSel As Color, menuSelBorder As Color,
                    ttBack As Color, ttFore As Color,
                    disabledFore As Color, linkColor As Color)
            Background = bg : Foreground = fg
            ControlBack = ctlBg : ControlFore = ctlFg
            Me.Border = border
            Me.Accent = accent
            Me.AccentHover = accentHover
            SelectionBack = selBack : SelectionFore = selFore
            Me.MenuBack = menuBack
            Me.MenuFore = menuFore
            MenuSelBack = menuSel : Me.MenuSelBorder = menuSelBorder
            ToolTipBack = ttBack : ToolTipFore = ttFore
            Me.DisabledFore = disabledFore
            Me.LinkColor = linkColor
        End Sub

        Public Shared Function ForTheme(t As AppTheme) As ThemePalette
            If t = AppTheme.Dark Then

                '1. Ruhiges, kontrastreiches Dark
                '    Return New ThemePalette(
                '    Color.FromArgb(32, 32, 36),  ' Background
                '    Color.FromArgb(235, 235, 235), ' Foreground
                '    Color.FromArgb(46, 46, 52),  ' ControlBack
                '    Color.FromArgb(230, 230, 230), ' ControlFore
                '    Color.FromArgb(70, 70, 78),  ' Border
                '    Color.FromArgb(0, 120, 215), ' Accent
                '    Color.FromArgb(0, 150, 245), ' AccentHover
                '    Color.FromArgb(60, 90, 150), ' SelectionBack
                '    Color.White,                  ' SelectionFore
                '    Color.FromArgb(40, 40, 46),  ' MenuBack
                '    Color.FromArgb(235, 235, 235), ' MenuFore
                '    Color.FromArgb(62, 62, 70),  ' MenuSelBack
                '    Color.FromArgb(90, 90, 100), ' MenuSelBorder
                '    Color.FromArgb(48, 48, 54),  ' ToolTipBack
                '    Color.FromArgb(240, 240, 240), ' ToolTipFore
                '    Color.FromArgb(150, 150, 155), ' DisabledFore
                '    Color.FromArgb(86, 156, 214)  ' LinkColor
                ')

                '2. Variante „Sanft“ (angepasst
                Return New ThemePalette(
                Color.FromArgb(32, 32, 36),   ' Background = &H202024
                Color.FromArgb(235, 235, 235), ' Foreground
                Color.FromArgb(32, 32, 36),   ' ControlBack (näher an Background, = &H26262A)
                Color.FromArgb(230, 230, 230), ' ControlFore
                Color.FromArgb(64, 64, 72),   ' Border (leicht angepasst, damit erkennbar bleibt)
                Color.FromArgb(0, 120, 215),  ' Accent
                Color.FromArgb(0, 150, 245),  ' AccentHover
                Color.FromArgb(60, 90, 150),  ' SelectionBack
                Color.White,                  ' SelectionFore
                Color.FromArgb(40, 40, 46),   ' MenuBack
                Color.FromArgb(235, 235, 235), ' MenuFore
                Color.FromArgb(62, 62, 70),   ' MenuSelBack
                Color.FromArgb(90, 90, 100),  ' MenuSelBorder
                Color.FromArgb(48, 48, 54),   ' ToolTipBack
                Color.FromArgb(240, 240, 240), ' ToolTipFore
                Color.FromArgb(150, 150, 155), ' DisabledFore
                Color.FromArgb(86, 156, 214)  ' LinkColor
            )

            Else
                ' Helles, leicht gedämpftes Light
                Return New ThemePalette(
                Color.White,  ' Background
                Color.FromArgb(30, 30, 30),     ' Foreground
                Color.White,                     ' ControlBack
                Color.FromArgb(30, 30, 30),     ' ControlFore
                Color.FromArgb(200, 200, 210),  ' Border
                Color.FromArgb(0, 120, 215),    ' Accent
                Color.FromArgb(0, 140, 235),    ' AccentHover
                Color.FromArgb(204, 228, 247),  ' SelectionBack
                Color.Black,                    ' SelectionFore
                Color.FromArgb(250, 250, 252),  ' MenuBack
                Color.FromArgb(20, 20, 20),     ' MenuFore
                Color.FromArgb(230, 238, 250),  ' MenuSelBack
                Color.FromArgb(200, 210, 230),  ' MenuSelBorder
                Color.FromArgb(255, 255, 230),  ' ToolTipBack
                Color.FromArgb(20, 20, 20),     ' ToolTipFore
                Color.FromArgb(140, 140, 150),  ' DisabledFore
                Color.FromArgb(0, 102, 204)     ' LinkColor
            )
            End If
        End Function
    End Class

    ' ToolStrip/ContextMenuStrip Renderer
    Public NotInheritable Class ThemedColorTable
        Inherits ProfessionalColorTable

        Private ReadOnly P As ThemePalette

        Public Sub New(palette As ThemePalette)
            P = palette
            Me.UseSystemColors = False
        End Sub

        Public Overrides ReadOnly Property ToolStripDropDownBackground As Color
            Get
                Return P.MenuBack
            End Get
        End Property
        Public Overrides ReadOnly Property ImageMarginGradientBegin As Color
            Get
                Return P.MenuBack
            End Get
        End Property
        Public Overrides ReadOnly Property ImageMarginGradientMiddle As Color
            Get
                Return P.MenuBack
            End Get
        End Property
        Public Overrides ReadOnly Property ImageMarginGradientEnd As Color
            Get
                Return P.MenuBack
            End Get
        End Property
        Public Overrides ReadOnly Property MenuItemSelected As Color
            Get
                Return P.MenuSelBack
            End Get
        End Property
        Public Overrides ReadOnly Property MenuItemBorder As Color
            Get
                Return P.MenuSelBorder
            End Get
        End Property
        Public Overrides ReadOnly Property MenuBorder As Color
            Get
                Return P.Border
            End Get
        End Property
        Public Overrides ReadOnly Property ToolStripBorder As Color
            Get
                Return P.Border
            End Get
        End Property
        Public Overrides ReadOnly Property ToolStripGradientBegin As Color
            Get
                Return P.MenuBack
            End Get
        End Property
        Public Overrides ReadOnly Property ToolStripGradientMiddle As Color
            Get
                Return P.MenuBack
            End Get
        End Property
        Public Overrides ReadOnly Property ToolStripGradientEnd As Color
            Get
                Return P.MenuBack
            End Get
        End Property
    End Class

    'Public NotInheritable Class ThemedRenderer
    '    Inherits ToolStripProfessionalRenderer

    '    Public Sub New(palette As ThemePalette)
    '        MyBase.New(New ThemedColorTable(palette))
    '    End Sub
    'End Class

End Namespace