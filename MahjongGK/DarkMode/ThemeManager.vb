Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

Imports System.Drawing.Imaging
Imports System.Runtime.InteropServices

Namespace Theme
    Public NotInheritable Class ThemeManager
        Private ReadOnly _root As Control
        Private ReadOnly _theme As AppTheme
        Private ReadOnly _p As ThemePalette
        Private ReadOnly _useOwnerDrawTabs As Boolean
        Private ReadOnly _customizeProgressBar As Boolean

        ' NEU: Aufhellung 0..255 (nur Darkmode)
        Private ReadOnly _brightenAmount As Single
        Private Const MAX_ICON_SIZE As Integer = 48

        'Markierung, daß diese Bitmap nicht aufgehellt werden soll.
        Private Shared ReadOnly DARK_MARKER As Color = Color.FromArgb(1, 127, 128, 129)

        ' Verhindert Mehrfach-Bearbeitung derselben Image-Instanz
        Private ReadOnly _processedImages As New HashSet(Of Integer)()

        Public Sub New(root As Control,
                   theme As AppTheme,
                   Optional useOwnerDrawTabs As Boolean = False,
                   Optional customizeProgressBar As Boolean = True,
                   Optional brightenAmount As Single = 0.4)
            _root = root
            _theme = theme
            _p = ThemePalette.ForTheme(theme)
            _useOwnerDrawTabs = useOwnerDrawTabs
            _customizeProgressBar = customizeProgressBar
            _brightenAmount = Math.Min(255, Math.Max(0, brightenAmount))

            ApplyAll()
            AddHandler _root.ControlAdded, AddressOf Root_ControlAdded
        End Sub

        Public Sub ApplyAll()
            _root.BackColor = _p.Background
            _root.ForeColor = _p.Foreground
            ApplyRecursive(_root)
            _root.Invalidate(True)
        End Sub

        Public Sub RegisterToolTip(tt As ToolTip)
            If tt Is Nothing Then Return
            tt.BackColor = _p.ToolTipBack
            tt.ForeColor = _p.ToolTipFore
            tt.OwnerDraw = False
        End Sub

        ' OPTIONAL: explizite Registrierung einer ImageList (falls nicht über Controls erreichbar)
        Public Sub RegisterImageList(il As ImageList)
            If il Is Nothing Then Return
            If _theme = AppTheme.Dark AndAlso _brightenAmount > 0 Then
                BrightenImageListIfNeeded(il)
            End If
        End Sub

        Private Sub Root_ControlAdded(sender As Object, e As ControlEventArgs)
            ApplyRecursive(e.Control)
        End Sub

        Private Sub ApplyRecursive(c As Control)
            ApplyOne(c)
            For Each child As Control In c.Controls
                ApplyRecursive(child)
            Next
        End Sub

        Private Sub ApplyOne(c As Control)
            ' Grundfarben (alles außer ToolStrip/ContextMenuStrip)
            If TypeOf c IsNot ToolStrip AndAlso TypeOf c IsNot ContextMenuStrip Then
                SafeSetColors(c, _p.ControlBack, _p.ControlFore)
            End If

            ' Per-Typ-Feinheiten (— wie in deiner vorherigen Version —)
            If TypeOf c Is Button Then
                Dim b As Button = DirectCast(c, Button)
                b.FlatStyle = FlatStyle.Flat
                b.FlatAppearance.BorderColor = _p.Border
                b.FlatAppearance.MouseOverBackColor = _p.AccentHover
                b.FlatAppearance.MouseDownBackColor = _p.Accent
                b.BackColor = _p.ControlBack : b.ForeColor = _p.ControlFore

            ElseIf TypeOf c Is Label Then
                c.ForeColor = _p.ControlFore
                If DirectCast(c, Label).BackColor <> Color.Transparent Then c.BackColor = _p.ControlBack

            ElseIf TypeOf c Is GroupBox Then
                c.ForeColor = _p.ControlFore : c.BackColor = _p.Background

            ElseIf TypeOf c Is Panel Then
                c.BackColor = _p.Background

            ElseIf TypeOf c Is TextBox Then
                Dim tb As TextBox = DirectCast(c, TextBox)
                tb.BackColor = _p.ControlBack : tb.ForeColor = _p.ControlFore : tb.BorderStyle = BorderStyle.FixedSingle

            ElseIf TypeOf c Is RichTextBox Then
                Dim r As RichTextBox = DirectCast(c, RichTextBox)
                r.BackColor = _p.ControlBack : r.ForeColor = _p.ControlFore : r.BorderStyle = BorderStyle.FixedSingle

            ElseIf TypeOf c Is ComboBox Then
                Dim cb As ComboBox = DirectCast(c, ComboBox)
                cb.BackColor = _p.ControlBack : cb.ForeColor = _p.ControlFore : cb.FlatStyle = FlatStyle.Flat

            ElseIf TypeOf c Is ListBox Then
                Dim lb As ListBox = DirectCast(c, ListBox)
                lb.BackColor = _p.ControlBack : lb.ForeColor = _p.ControlFore : lb.BorderStyle = BorderStyle.FixedSingle

            ElseIf TypeOf c Is NumericUpDown Then
                Dim nu As NumericUpDown = DirectCast(c, NumericUpDown)
                nu.BackColor = _p.ControlBack : nu.ForeColor = _p.ControlFore : nu.BorderStyle = BorderStyle.FixedSingle

            ElseIf TypeOf c Is Num2UpDown Then
                Dim nu As Num2UpDown = DirectCast(c, Num2UpDown)
                nu.BackColor = _p.ControlBack : nu.ForeColor = _p.ControlFore : nu.BorderStyle = BorderStyle.FixedSingle

            ElseIf TypeOf c Is ProgressBar Then
                Dim pb As ProgressBar = DirectCast(c, ProgressBar)
                If _customizeProgressBar Then
                    SetWindowTheme(pb.Handle, "", "")
                    pb.ForeColor = _p.Accent : pb.BackColor = _p.ControlBack : pb.Style = ProgressBarStyle.Continuous
                End If

            ElseIf TypeOf c Is SplitContainer Then
                Dim sp As SplitContainer = DirectCast(c, SplitContainer)
                sp.BackColor = _p.Background : sp.Panel1.BackColor = _p.Background : sp.Panel2.BackColor = _p.Background

            ElseIf TypeOf c Is TabControl Then
                Dim tc As TabControl = DirectCast(c, TabControl)
                tc.BackColor = _p.Background : tc.ForeColor = _p.ControlFore
                If _useOwnerDrawTabs Then
                    tc.DrawMode = TabDrawMode.OwnerDrawFixed
                    RemoveHandler tc.DrawItem, AddressOf TabControl_DrawItem
                    AddHandler tc.DrawItem, AddressOf TabControl_DrawItem
                End If

            ElseIf TypeOf c Is ToolStrip Then
                StyleToolStrip(DirectCast(c, ToolStrip))

            ElseIf TypeOf c Is ContextMenuStrip Then
                StyleContextMenu(DirectCast(c, ContextMenuStrip))
            End If

            'Es stimmt was nicht. Wenn ArrowNormal das erste mal aufgerufen wird,
            'liefert es die aufgehellte Bitmap für jeden d As Kompass.
            'Fahre ich dann mit der Maus über den Punkt, wird die Funktion beim Verlassen wieder aufgerufen,
            'das zweite mal mit einem bestimmten d As Kompass. Beim zweiten mal wird das Original geliefert,
            'INI.Global_DarkMode hat sich nicht geändert. (geht auch nicht, wird einmalig beim Programmstart
            'festgelegt) Private Function ArrowNormal(d As Kompass) As Bitmap
            'Nachgedacht für 3m 26s
            'Guter Hinweis – das Verhalten („erst hell, nach Hover wieder Original“) passt zu einem Doppel-System:
            'Du hast jetzt überall Theme.GetResBmp(...) (die Bilder kommen bereits hell aus dem Cache),
            'und zusätzlich hellt d'ein ThemeManager in ApplyOne/StyleToolStrip die Bilder an Controls nochmal
            '(bzw. entscheidet dort, ob nicht aufgehellt wird). Je nach Reihenfolge bekommst du dann mal das bereits
            'aufgehellte Cache-Bild, mal das „unveränderte“ (weil der ThemeManager es in diesem Fall überspringt).
            'Ergebnis: inkonsistent.
            'Kurzlösung(empfohlen)
            'Wenn du die Bilder schon über Theme.GetResImg/GetResBmp hellst, dann
            'deaktiviere im ThemeManager die Laufzeit-Aufhellung für Control-Images und ImageLists
            '' ' === NEU: Bilder aufhellen (Darkmode) ===================================
            ''If _theme = AppTheme.Dark AndAlso _brightenAmount > 0 Then
            ''    ' 1) Controls mit Image-Eigenschaft (PictureBox, Button, …)
            ''    TryBrightenControlImage(c)

            ''    ' 2) Controls mit ImageList-Eigenschaft (Buttons, TabControl, ListView, …)
            ''    TryBrightenControlImageList(c)
            ''End If
            ''und in StyleToolStrip/StyleContextMenu die Stellen,
            ''wo du it.Image = BrightenImageIfNeeded(it.Image) machst, ebenfalls entfernen.
            ''Lass die Helligkeit nur noch über den Cache laufen (konsistent).
        End Sub

        ' --- Images an Controls -----------------------------------------------------

        Private Sub TryBrightenControlImage(ctrl As Control)
            Dim pi As Reflection.PropertyInfo =
        ctrl.GetType().GetProperty("Image", Reflection.BindingFlags.Public Or Reflection.BindingFlags.Instance)

            If pi Is Nothing Then Return
            If Not GetType(Image).IsAssignableFrom(pi.PropertyType) Then Return

            Dim img As Image = TryCast(pi.GetValue(ctrl, Nothing), Image)
            Dim bright As Image = BrightenImageIfNeeded(img)
            If Not Object.ReferenceEquals(bright, img) Then
                pi.SetValue(ctrl, bright, Nothing)
            End If
        End Sub


        Private Sub TryBrightenControlImageList(ctrl As Control)
            ' 1) Generische ImageList-Property (z. B. TabControl.ImageList)
            Dim pi As System.Reflection.PropertyInfo =
        ctrl.GetType().GetProperty("ImageList",
            Reflection.BindingFlags.Public Or Reflection.BindingFlags.Instance)

            If pi IsNot Nothing AndAlso GetType(ImageList).IsAssignableFrom(pi.PropertyType) Then
                Dim il As ImageList = TryCast(pi.GetValue(ctrl, Nothing), ImageList)
                If il IsNot Nothing Then BrightenImageListIfNeeded(il)
            End If

            ' 2) Control-spezifische ImageLists sauber mitnehmen
            Dim lv As ListView = TryCast(ctrl, ListView)
            If lv IsNot Nothing Then
                If lv.SmallImageList IsNot Nothing Then BrightenImageListIfNeeded(lv.SmallImageList)
                If lv.LargeImageList IsNot Nothing Then BrightenImageListIfNeeded(lv.LargeImageList)
                If lv.StateImageList IsNot Nothing Then BrightenImageListIfNeeded(lv.StateImageList)
                Return
            End If

            Dim tv As TreeView = TryCast(ctrl, TreeView)
            If tv IsNot Nothing AndAlso tv.ImageList IsNot Nothing Then
                BrightenImageListIfNeeded(tv.ImageList)
                Return
            End If

            Dim tc As TabControl = TryCast(ctrl, TabControl)
            If tc IsNot Nothing AndAlso tc.ImageList IsNot Nothing Then
                BrightenImageListIfNeeded(tc.ImageList)
                Return
            End If
        End Sub


        ' --- ToolStrip / ContextMenuStrip ------------------------------------------

        Private Sub StyleToolStrip(ts As ToolStrip)
            ts.Renderer = New ThemedRenderer(_p)
            ts.BackColor = _p.MenuBack
            ts.ForeColor = _p.MenuFore

            For Each it As ToolStripItem In ts.Items
                it.ForeColor = _p.MenuFore

                ''Siehe Kommentar in ApplyOne
                '' Bilder ggf. aufhellen
                'If _theme = AppTheme.Dark AndAlso _brightenAmount > 0 AndAlso it.Image IsNot Nothing Then
                '    Dim newImg As Image = BrightenImageIfNeeded(it.Image)
                '    If Not Object.ReferenceEquals(newImg, it.Image) Then it.Image = newImg
                'End If

                ' Auch Untermenüs einfärben
                Dim ddi As ToolStripDropDownItem = TryCast(it, ToolStripDropDownItem)
                If ddi IsNot Nothing AndAlso ddi.HasDropDownItems Then
                    StyleDropDownRecursive(ddi.DropDown)
                End If
            Next

            ' Falls ein ContextMenuStrip dranhängt, gleich mit stylen
            If ts.ContextMenuStrip IsNot Nothing Then StyleContextMenu(ts.ContextMenuStrip)
        End Sub

        Private Sub StyleContextMenu(cms As ContextMenuStrip)
            cms.Renderer = New ThemedRenderer(_p)
            cms.BackColor = _p.MenuBack
            cms.ForeColor = _p.MenuFore

            For Each it As ToolStripItem In cms.Items
                it.ForeColor = _p.MenuFore

                '' Doppelte Aufhellung
                ''If _theme = AppTheme.Dark AndAlso _brightenAmount > 0 AndAlso it.Image IsNot Nothing Then
                ''    Dim newImg As Image = BrightenImageIfNeeded(it.Image)
                ''    If Not Object.ReferenceEquals(newImg, it.Image) Then it.Image = newImg
                ''End If

                Dim ddi As ToolStripDropDownItem = TryCast(it, ToolStripDropDownItem)
                If ddi IsNot Nothing AndAlso ddi.HasDropDownItems Then
                    StyleDropDownRecursive(ddi.DropDown)
                End If
            Next
        End Sub


        ' NEW: rekursiv alle DropDown-Ebenen stylen
        Private Sub StyleDropDownRecursive(dd As ToolStripDropDown)
            If dd Is Nothing Then Return
            dd.Renderer = New ThemedRenderer(_p)
            dd.BackColor = _p.MenuBack
            dd.ForeColor = _p.MenuFore

            For Each it As ToolStripItem In dd.Items
                it.ForeColor = _p.MenuFore
                Dim ddi As ToolStripDropDownItem = TryCast(it, ToolStripDropDownItem)
                If ddi IsNot Nothing AndAlso ddi.HasDropDownItems Then
                    StyleDropDownRecursive(ddi.DropDown)
                End If
            Next
        End Sub

        ' --- ImageList --------------------------------------------------------------

        Private Sub BrightenImageListIfNeeded(il As ImageList)
            If il Is Nothing OrElse il.Images Is Nothing OrElse il.Images.Count = 0 Then Return

            Dim count As Integer = il.Images.Count
            Dim keys(count - 1) As String
            For i As Integer = 0 To count - 1
                Try
                    keys(i) = il.Images.Keys(i)
                Catch
                    keys(i) = Nothing
                End Try
            Next

            For i As Integer = 0 To count - 1
                Dim img As Image = il.Images(i)
                Dim newImg As Image = BrightenImageIfNeeded(img)
                If Not Object.ReferenceEquals(newImg, img) Then
                    il.Images(i) = newImg
                    If keys(i) IsNot Nothing Then il.Images.SetKeyName(i, keys(i))
                End If
            Next
        End Sub


        ' --- Brighten Kernlogik -----------------------------------------------------

        Private Function BrightenImageIfNeeded(img As Image) As Image
            If img Is Nothing Then Return img

            ' Nur im Dark-Theme & wenn >0
            If _theme <> AppTheme.Dark OrElse _brightenAmount <= 0 Then Return img

            ' Schon bearbeitet? (gegen Mehrfach-Verarbeitung)
            Dim hash As Integer = RuntimeHelpersGetHashCode(img)
            If _processedImages.Contains(hash) Then Return img

            ' Sicherstellen, dass wir NICHT die Resource-Instanz mutieren:
            ' -> immer eigener 32bpp-Clone
            Dim bmp As Bitmap = TryCast(img, Bitmap)
            Dim clone As Bitmap
            If bmp Is Nothing Then
                clone = New Bitmap(img.Width, img.Height, PixelFormat.Format32bppArgb)
                clone.SetResolution(img.HorizontalResolution, img.VerticalResolution)
                Using g As Graphics = Graphics.FromImage(clone)
                    g.DrawImage(img, 0, 0, img.Width, img.Height)
                End Using
            Else
                clone = DirectCast(bmp.Clone(New Rectangle(0, 0, bmp.Width, bmp.Height), PixelFormat.Format32bppArgb), Bitmap)
                clone.SetResolution(bmp.HorizontalResolution, bmp.VerticalResolution)
            End If

            ' Dark-Marker (0,0) -> nicht aufhellen, Clone wieder verwerfen
            Dim marker As Color = clone.GetPixel(0, 0)
            If marker.A = DARK_MARKER.A AndAlso marker.R = DARK_MARKER.R AndAlso marker.G = DARK_MARKER.G AndAlso marker.B = DARK_MARKER.B Then
                _processedImages.Add(hash)
                clone.Dispose()
                Return img
            End If

            ' In-place auf dem Clone (deepCopy:=False) – effizient, kein weiterer Müll
            MjGDI.Brighten(clone, _brightenAmount, deepCopy:=False)

            _processedImages.Add(hash)
            Return clone
        End Function


        ' RuntimeHelpers.GetHashCode ohne Abhängigkeit
        Private Shared Function RuntimeHelpersGetHashCode(obj As Object) As Integer
            Return obj.GetHashCode()
        End Function

        ' --- restliche Helfer (Zeichnen/Styles) ------------------------------------

        Private Sub TabControl_DrawItem(sender As Object, e As DrawItemEventArgs)
            Dim tc As TabControl = DirectCast(sender, TabControl)
            Dim page As TabPage = tc.TabPages(e.Index)
            Dim selected As Boolean = (e.State And DrawItemState.Selected) = DrawItemState.Selected

            Using bg As Brush = New SolidBrush(If(selected, _p.SelectionBack, _p.ControlBack))
                e.Graphics.FillRectangle(bg, e.Bounds)
            End Using
            Using br As Brush = New SolidBrush(If(selected, _p.SelectionFore, _p.ControlFore))
                Dim sf As New StringFormat() With {.Alignment = StringAlignment.Center, .LineAlignment = StringAlignment.Center}
                e.Graphics.DrawString(page.Text, tc.Font, br, e.Bounds, sf)
            End Using
            Using p As New Pen(_p.Border)
                e.Graphics.DrawRectangle(p, New Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width - 1, e.Bounds.Height - 1))
            End Using
        End Sub

        Private Shared Sub SafeSetColors(c As Control, back As Color, fore As Color)
            If c.BackColor <> Color.Transparent Then c.BackColor = back
            c.ForeColor = fore
        End Sub

#Region "Win32 ProgressBar Hack"
        <DllImport("uxtheme.dll", CharSet:=CharSet.Unicode)>
        Private Shared Function SetWindowTheme(hWnd As IntPtr, pszSubAppName As String, pszSubIdList As String) As Integer
        End Function
#End Region
    End Class

End Namespace