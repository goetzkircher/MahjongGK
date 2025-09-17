Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

' … deine ThemePalette bleibt wie gepostet …

Namespace Theme

    Public NotInheritable Class ThemedRenderer
        Inherits ToolStripProfessionalRenderer

        Private ReadOnly P As ThemePalette

        Public Sub New(palette As ThemePalette)
            MyBase.New(New ThemedColorTable(palette))
            P = palette
            ' Wichtig: sonst werden Systemfarben verwendet
            Me.RoundedEdges = False
        End Sub

        Protected Overrides Sub OnRenderToolStripBackground(e As ToolStripRenderEventArgs)
            ' Sicherstellen, dass auch ohne Gradient überall MenuBack liegt
            Using b As Brush = New SolidBrush(P.MenuBack)
                e.Graphics.FillRectangle(b, e.AffectedBounds)
            End Using
        End Sub

        Protected Overrides Sub OnRenderMenuItemBackground(e As ToolStripItemRenderEventArgs)
            Dim it As ToolStripItem = e.Item
            Dim r As Rectangle = New Rectangle(Point.Empty, it.Bounds.Size)

            If it.Selected OrElse it.Pressed Then
                Using b As Brush = New SolidBrush(P.MenuSelBack)
                    e.Graphics.FillRectangle(b, r)
                End Using
                Using pen As Pen = New Pen(P.MenuSelBorder)
                    Dim rr As Rectangle = New Rectangle(r.X, r.Y, r.Width - 1, r.Height - 1)
                    e.Graphics.DrawRectangle(pen, rr)
                End Using
            Else
                Using b As Brush = New SolidBrush(P.MenuBack)
                    e.Graphics.FillRectangle(b, r)
                End Using
            End If
        End Sub

        Protected Overrides Sub OnRenderItemText(e As ToolStripItemTextRenderEventArgs)
            ' Textfarbe konsistent setzen (selected/disabled)
            If Not e.Item.Enabled Then
                e.TextColor = P.DisabledFore
            ElseIf e.Item.Selected OrElse e.Item.Pressed Then
                e.TextColor = P.SelectionFore
            Else
                e.TextColor = P.MenuFore
            End If
            e.TextFont = e.Item.Font
            MyBase.OnRenderItemText(e)
        End Sub

        Protected Overrides Sub OnRenderSeparator(e As ToolStripSeparatorRenderEventArgs)
            Dim r As Rectangle = New Rectangle(0, 0, e.Item.Width, e.Item.Height)
            Using b As Brush = New SolidBrush(P.MenuBack)
                e.Graphics.FillRectangle(b, r)
            End Using
            Dim y As Integer = CInt(Math.Truncate(r.Height / 2))
            Using pen As Pen = New Pen(P.Border)
                e.Graphics.DrawLine(pen, 2, y, r.Width - 2, y)
            End Using
        End Sub

        Protected Overrides Sub OnRenderArrow(e As ToolStripArrowRenderEventArgs)
            ' Pfeilfarbe für Untermenüs
            e.ArrowColor = If(e.Item.Enabled, If(e.Item.Selected, P.SelectionFore, P.MenuFore), P.DisabledFore)
            MyBase.OnRenderArrow(e)
        End Sub

        Protected Overrides Sub OnRenderImageMargin(e As ToolStripRenderEventArgs)
            ' Bildspalte einfärben
            Using b As Brush = New SolidBrush(P.MenuBack)
                e.Graphics.FillRectangle(b, e.AffectedBounds)
            End Using
        End Sub
    End Class

    ' Zusätzlich: alle DropDown-Ebenen rekursiv stylen
    Friend Module ToolStripStylingHelpers

        Public Sub StyleToolStripRecursive(ts As ToolStrip, p As ThemePalette)
            If ts Is Nothing Then Return
            ts.RenderMode = ToolStripRenderMode.Professional
            ts.Renderer = New ThemedRenderer(p)
            ts.BackColor = p.MenuBack
            ts.ForeColor = p.MenuFore

            For Each it As ToolStripItem In ts.Items
                it.ForeColor = p.MenuFore

                Dim ddi As ToolStripDropDownItem = TryCast(it, ToolStripDropDownItem)
                If ddi IsNot Nothing AndAlso ddi.HasDropDownItems Then
                    StyleDropDownRecursive(ddi.DropDown, p)
                End If
            Next
        End Sub

        Public Sub StyleDropDownRecursive(dd As ToolStripDropDown, p As ThemePalette)
            If dd Is Nothing Then Return
            dd.Renderer = New ThemedRenderer(p)
            dd.BackColor = p.MenuBack
            dd.ForeColor = p.MenuFore

            For Each it As ToolStripItem In dd.Items
                it.ForeColor = p.MenuFore
                Dim ddi As ToolStripDropDownItem = TryCast(it, ToolStripDropDownItem)
                If ddi IsNot Nothing AndAlso ddi.HasDropDownItems Then
                    StyleDropDownRecursive(ddi.DropDown, p)
                End If
            Next
        End Sub


    End Module

End Namespace
