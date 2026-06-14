Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

#Disable Warning IDE0079
#Disable Warning IDE1006
#Disable Warning IDE0140

Imports System.Drawing.Drawing2D
Imports System.Drawing.Text

''' <summary>
''' Zweispaltiger Farbwähler wie die gewünschte HTML-Optik:
''' • Gruppen-Überschrift mit farbigem Kreis (●) vor dem Titel
''' • Darunter Farben als Kacheln: Farbbalken oben, Name zentriert unten
''' • Umbruch nur an Gruppen-Überschriften (keine Gruppe wird geteilt)
''' • Selektierte Farbe: Name fett + Auswahlrahmen
''' • Dark-/Light-Mode via INI.Global_DarkMode
''' </summary>
Public NotInheritable Class ColorPickerNamedColors
    Inherits Form

    ' ──  Parameter ──────────────────────────────────────────────────
    Private Const LBLWIDTH As Integer = 200   ' Kachelbreite
    Private Const LBLHEIGHT As Integer = 70   ' Kachelhöhe inkl. Text
    Private Const BTNWIDTH As Integer = 85    ' Button-Breite   

    Private _SelectedColor As Color
    Private _pendingCenterSelection As Boolean = False
    Private _initialColor As Color

    ' ── Steuerelemente ─────────────────────────────────────────────────────────
    Private ReadOnly _grid As TwoColumnColorPanel = New TwoColumnColorPanel()
    Private ReadOnly _btnOk As Button = New Button()
    Private ReadOnly _btnCancel As Button = New Button()
    Private ReadOnly _btnCopyName As Button = New Button()
    Private ReadOnly _btnCopyHex As Button = New Button()
    Private ReadOnly _btnCopyDez As Button = New Button()

    ' ── Tabellengesteuerte Reihenfolge ───────────────────────────────────────────
    ' Format:
    '   #HEADER|<Titel>|<R,G,B>   -> Gruppen-Überschrift mit Kreisfarbe
    '   <KnownColorName>          -> Farbnamen exakt wie in System.Drawing.KnownColor
    '
    ' Du kannst Zeilen frei verschieben. Alles, was hier NICHT steht,
    ' wird am Ende in die Sammelgruppe "Nicht einsortiert (automatisch ergänzt)" gelegt.
    Private Shared ReadOnly OrderLines As String() = {
    "#HEADER|Rot|255,0,0",
    "Red",
    "DarkRed",
    "Firebrick",
    "IndianRed",
    "Crimson",
    "Tomato",
    "Salmon",
    "DarkSalmon",
    "LightSalmon",
    "RosyBrown",
    "Brown",
    "Maroon",
    "MistyRose",
    "LightPink",
    "Pink",
    "HotPink",
    "DeepPink",
               _
    "#HEADER|Orange|255,128,0",
    "OrangeRed",
    "Orange",
    "DarkOrange",
    "Coral",
    "LightCoral",
    "Sienna",
    "SaddleBrown",
    "Chocolate",
    "SandyBrown",
    "Peru",
    "BurlyWood",
    "Tan",
    "PeachPuff",
    "Moccasin",
    "NavajoWhite",
    "PapayaWhip",
    "BlanchedAlmond",
    "Bisque",
    "AntiqueWhite",
                   _
    "#HEADER|Gelb|255,215,0",
    "Gold",
    "Goldenrod",
    "DarkGoldenrod",
    "Khaki",
    "DarkKhaki",
    "Yellow",
    "LightYellow",
    "LemonChiffon",
    "LightGoldenrodYellow",
    "PaleGoldenrod",
    "Wheat",
    "Cornsilk",
               _
    "#HEADER|Grün|0,170,0",
    "Green",
    "DarkGreen",
    "ForestGreen",
    "SeaGreen",
    "MediumSeaGreen",
    "DarkSeaGreen",
    "YellowGreen",
    "Olive",
    "DarkOliveGreen",
    "OliveDrab",
    "Lime",
    "LimeGreen",
    "GreenYellow",
    "Chartreuse",
    "LawnGreen",
    "SpringGreen",
    "MediumSpringGreen",
    "PaleGreen",
    "LightGreen",
                 _
    "#HEADER|Cyan|0,170,170",
    "Aqua",
    "Cyan",
    "DarkCyan",
    "Teal",
    "DarkCyan",
    "LightSeaGreen",
    "Turquoise",
    "MediumTurquoise",
    "Aquamarine",
    "MediumAquamarine",
    "DarkTurquoise",
    "PaleTurquoise",
    "CadetBlue",
    "PowderBlue",
    "LightCyan",
    "Azure",
    "MintCream",
    "Honeydew",
               _
    "#HEADER|Blau|0,90,255",
    "Blue",
    "MediumBlue",
    "DarkBlue",
    "Navy",
    "MidnightBlue",
    "RoyalBlue",
    "CornflowerBlue",
    "DodgerBlue",
    "DeepSkyBlue",
    "SkyBlue",
    "LightBlue",
    "LightSkyBlue",
    "SteelBlue",
    "LightSteelBlue",
    "SlateBlue",
    "DarkSlateBlue",
    "MediumSlateBlue",
    "AliceBlue",
    "GhostWhite",
                 _
    "#HEADER|Magenta/Violett|200,0,200",
    "Fuchsia",
    "Magenta",
    "Purple",
    "Indigo",
    "BlueViolet",
    "DarkViolet",
    "DarkMagenta",
    "DarkOrchid",
    "Orchid",
    "MediumOrchid",
    "Violet",
    "Plum",
    "Thistle",
    "MediumPurple",
    "Lavender",
    "LavenderBlush",
    "MediumVioletRed",
    "PaleVioletRed",
                    _
    "#HEADER|Sonstige (Grauwerte u.A.)|192,192,192",
    "Black",
    "DimGray",
    "Gray",
    "DarkGray",
    "SlateGray",
    "DarkSlateGray",
    "LightSlateGray",
    "Silver",
    "LightGray",
    "Gainsboro",
    "WhiteSmoke",
    "White",
    "Snow",
    "FloralWhite",
    "Ivory",
    "Linen",
    "OldLace",
    "Seashell",
    "Beige"
}

    ' ── Konstruktor ────────────────────────────────────────────────────────────
    Public Sub New()

        Me.Text = "Farbwähler (benannte Farben)"
        Me.MinimumSize = New Size(500, 500)
        Me.KeyPreview = True
        Me.Icon = My.Resources.MahjongGK

        ApplyTheme()

        ' Panel konfigurieren
        _grid.Dock = DockStyle.Fill
        _grid.TileSize = New Size(LBLWIDTH, LBLHEIGHT)
        _grid.GroupHeaderHeight = LBLHEIGHT
        _grid.GroupGap = 12
        _grid.PaddingAll = 10
        _grid.HGap = 16
        _grid.VGap = 8
        AddHandler _grid.SelectionChanged,
             Sub()
                 Me._SelectedColor = _grid.SelectedColor
                 Dim hasSel As Boolean = (Me._SelectedColor <> Color.Empty)
                 _btnCopyName.Enabled = hasSel
                 _btnCopyHex.Enabled = hasSel
                 _btnCopyDez.Enabled = hasSel
             End Sub

        ' Buttons unten
        Dim pnlButtons As Panel = New Panel() With {.Dock = DockStyle.Bottom, .Height = 50}
        _btnOk.Text = "Übernehmen" : _btnOk.DialogResult = DialogResult.OK
        _btnCancel.Text = "Abbrechen" : _btnCancel.DialogResult = DialogResult.Cancel
        AddHandler _btnOk.Click, Sub() Me._SelectedColor = _grid.SelectedColor
        AddHandler pnlButtons.Resize,
             Sub(sender As Object, e As EventArgs)
                 Dim pad As Integer = 10

                 ' Größen
                 _btnCancel.Size = New Size(BTNWIDTH, 30)
                 _btnOk.Size = New Size(BTNWIDTH, 30)
                 _btnCopyName.Size = New Size(BTNWIDTH, 30)
                 _btnCopyHex.Size = New Size(BTNWIDTH, 30)
                 _btnCopyDez.Size = New Size(BTNWIDTH, 30)

                 ' Rechts: Abbrechen / Übernehmen
                 _btnCancel.Location = New Point(pnlButtons.ClientSize.Width - _btnCancel.Width - pad,
                                                 (pnlButtons.ClientSize.Height - _btnCancel.Height) \ 2)
                 _btnOk.Location = New Point(_btnCancel.Left - _btnOk.Width - 8, _btnCancel.Top)

                 ' Links: Copy-Buttons
                 Dim y As Integer = (pnlButtons.ClientSize.Height - _btnCopyName.Height) \ 2
                 Dim x As Integer = pad
                 _btnCopyName.Location = New Point(x, y)
                 x += _btnCopyName.Width + 8
                 _btnCopyHex.Location = New Point(x, y)
                 x += _btnCopyHex.Width + 8
                 _btnCopyDez.Location = New Point(x, y)
             End Sub

        pnlButtons.Controls.Add(_btnOk)
        pnlButtons.Controls.Add(_btnCancel)

        ' Copy-Buttons links
        _btnCopyName.Text = "Copy as Name"
        _btnCopyHex.Text = "Copy as Hex"
        _btnCopyDez.Text = "Copy as Dez"

        _btnCopyName.TabIndex = 10
        _btnCopyHex.TabIndex = 11
        _btnCopyDez.TabIndex = 12

        ' Default: erst aktivieren, wenn eine Auswahl existiert
        _btnCopyName.Enabled = False
        _btnCopyHex.Enabled = False
        _btnCopyDez.Enabled = False

        AddHandler _btnCopyName.Click,
            Sub(_s As Object, _e As EventArgs)
                Dim n As String = "Color." & _grid.SelectedName
                If n IsNot Nothing AndAlso n.Length > 0 Then
                    Try : Clipboard.SetText(n) : Catch : End Try
                End If
            End Sub

        AddHandler _btnCopyHex.Click,
            Sub(_s As Object, _e As EventArgs)
                Dim c As Color = _grid.SelectedColor
                If c <> Color.Empty Then
                    Try : Clipboard.SetText(ToRgbHex(c)) : Catch : End Try
                End If
            End Sub

        AddHandler _btnCopyDez.Click,
            Sub(_s As Object, _e As EventArgs)
                Dim c As Color = _grid.SelectedColor
                If c <> Color.Empty Then
                    Try : Clipboard.SetText(ToRgbDez(c)) : Catch : End Try
                End If
            End Sub

        pnlButtons.Controls.Add(_btnCopyName)
        pnlButtons.Controls.Add(_btnCopyHex)
        pnlButtons.Controls.Add(_btnCopyDez)

        ' ... Setup von _grid und Buttons ...

        ' Form zusammensetzen
        Me.Controls.Add(_grid)
        Me.Controls.Add(pnlButtons)

        ' Daten laden gemäß Tabelle
        Dim groups As List(Of ColorGroup) = BuildGroupsFromTable()
        _grid.SetGroups(groups)

        AddHandler Me.KeyDown,
            Sub(_s As Object, e As KeyEventArgs)
                Select Case e.KeyCode
                    Case Keys.Enter : _btnOk.PerformClick()
                    Case Keys.Escape : _btnCancel.PerformClick()
                End Select
            End Sub
    End Sub

    ''' <summary>Ergebnisfarbe nach OK. Bei Cancel bleibt der anfängliche Wert unverändert.</summary>
    Public Property SelectedColor As Color
        Get
            Return _SelectedColor
        End Get
        Set(value As Color)
            _SelectedColor = value
            _initialColor = value
            If _initialColor <> Color.Empty Then
                _grid.SelectClosest(_initialColor)
                ' erst nach Anzeige zentrieren
                _pendingCenterSelection = True
            End If
        End Set
    End Property

    ' Nach ShowDialog wird OnShown ausgelöst – hier zentrieren wir wirklich
    Protected Overrides Sub OnShown(e As EventArgs)
        MyBase.OnShown(e)
        If _pendingCenterSelection Then
            _pendingCenterSelection = False
            _grid.ScrollSelectionIntoView(True)   ' True = möglichst vertikal zentrieren
        End If
    End Sub

    Private Sub ApplyTheme()
        Dim dark As Boolean
        Try
            dark = INI.Global_DarkMode
        Catch ex As Exception
            dark = False
        End Try

        If dark Then
            Me.BackColor = Color.FromArgb(24, 24, 24)
            Me.ForeColor = Color.Gainsboro
        Else
            Me.BackColor = SystemColors.Window
            Me.ForeColor = SystemColors.WindowText
        End If
    End Sub

    ' ── Datenmodell ────────────────────────────────────────────────────────────

    <DebuggerDisplay("{Name} = {Color}")>
    Private NotInheritable Class NamedColorItem
        Public ReadOnly Property Color As Color
        Public ReadOnly Property Name As String
        Public Sub New(c As Color, name As String)
            Me.Color = c
            Me.Name = name
        End Sub
    End Class

    Private NotInheritable Class ColorGroup
        Public ReadOnly Property Title As String
        Public ReadOnly Property TitleCircleColor As Color
        Public ReadOnly Property Items As List(Of NamedColorItem)
        Public Sub New(title As String, circle As Color, items As IEnumerable(Of NamedColorItem))
            Me.Title = title
            Me.TitleCircleColor = circle
            Me.Items = items.ToList()
        End Sub
    End Class

    ''' <summary>
    ''' Baut die Gruppen exakt gemäß <see cref="OrderLines"/> auf.
    ''' Unbekannte/vergessene KnownColors werden automatisch in eine Abschlussgruppe einsortiert.
    ''' </summary>
    Private Function BuildGroupsFromTable() As List(Of ColorGroup)
        Dim groups As List(Of ColorGroup) = New List(Of ColorGroup)()
        Dim current As ColorGroup = Nothing

        ' HashSet aller bekannten (nicht-systemischen) KnownColors
        Dim knownAll As KnownColor() = CType([Enum].GetValues(GetType(KnownColor)), KnownColor())
        Dim nonSystem As HashSet(Of String) = New HashSet(Of String)(StringComparer.InvariantCultureIgnoreCase)
        Dim kc As KnownColor
        For Each kc In knownAll
            Dim c As Color = Color.FromKnownColor(kc)
            If c.IsSystemColor Then Continue For
            If c.A < 255 Then Continue For
            nonSystem.Add(kc.ToString())
        Next

        ' Wir sammeln, was in der Tabelle verwendet wurde
        Dim used As HashSet(Of String) = New HashSet(Of String)(StringComparer.InvariantCultureIgnoreCase)

        ' Parser für #HEADER|Titel|R,G,B
        Dim line As String
        For Each line In OrderLines
            If String.IsNullOrWhiteSpace(line) Then Continue For

            If line.StartsWith("#HEADER|", StringComparison.InvariantCultureIgnoreCase) Then
                ' neuen Header anlegen
                Dim parts As String() = line.Split("|"c)
                Dim title As String = If(parts.Length > 1, parts(1).Trim(), "Gruppe")
                Dim circle As Color = Color.Silver

                If parts.Length > 2 Then
                    Dim rgbParts As String() = parts(2).Split(","c)
                    If rgbParts.Length = 3 Then
                        Dim r As Integer, g As Integer, b As Integer
                        If Integer.TryParse(rgbParts(0).Trim(), r) AndAlso
                       Integer.TryParse(rgbParts(1).Trim(), g) AndAlso
                       Integer.TryParse(rgbParts(2).Trim(), b) Then
                            circle = Color.FromArgb(Math.Max(0, Math.Min(255, r)),
                                                Math.Max(0, Math.Min(255, g)),
                                                Math.Max(0, Math.Min(255, b)))
                        End If
                    End If
                End If

                current = New ColorGroup(title, circle, Enumerable.Empty(Of NamedColorItem)())
                groups.Add(current)

            Else
                ' Farbnamenzeile
                Dim name As String = line.Trim()
                If name.Length = 0 Then Continue For

                If Not nonSystem.Contains(name) Then
                    ' Unbekannt, überspringen (oder du wirfst absichtlich eine Exception)
                    Continue For
                End If

                used.Add(name)

                ' Farbe erzeugen und anhängen
                Dim c As Color = Color.FromKnownColor(CType([Enum].Parse(GetType(KnownColor), name, True), KnownColor))
                If current Is Nothing Then
                    ' Falls Tabelle mit Farbe beginnt, erzeugen wir eine Default-Gruppe
                    current = New ColorGroup("Ohne Überschrift", Color.Silver, Enumerable.Empty(Of NamedColorItem)())
                    groups.Add(current)
                End If

                current.Items.Add(New NamedColorItem(c, name))
            End If
        Next

        ' Fehlende Farben automatisch in Sammelgruppe hängen (damit nichts verloren geht)
        Dim missing As List(Of String) = nonSystem.Where(Function(n As String) Not used.Contains(n)).OrderBy(Function(n As String) n, StringComparer.InvariantCultureIgnoreCase).ToList()
        If missing.Count > 0 Then
            Dim fallbackTitle As String = "Nicht einsortiert (automatisch ergänzt)"
            Dim fallback As ColorGroup = New ColorGroup(fallbackTitle, Color.Gray, Enumerable.Empty(Of NamedColorItem)())
            Dim n As String
            For Each n In missing
                Dim col As Color = Color.FromKnownColor(CType([Enum].Parse(GetType(KnownColor), n, True), KnownColor))
                fallback.Items.Add(New NamedColorItem(col, n))
            Next
            groups.Add(fallback)
        End If

        ' Leere Gruppen entfernen (falls jemand Header ohne Items übrig lässt)
        Dim cleaned As List(Of ColorGroup) = groups.Where(Function(g As ColorGroup) g.Items IsNot Nothing AndAlso g.Items.Count > 0).ToList()
        Return cleaned
    End Function

    Private Function BuildNamedColors() As List(Of NamedColorItem)
        Dim list As List(Of NamedColorItem) = New List(Of NamedColorItem)(capacity:=200)
        Dim known() As KnownColor = CType([Enum].GetValues(GetType(KnownColor)), KnownColor())
        Dim kc As KnownColor
        For Each kc In known
            Dim c As Color = Color.FromKnownColor(kc)
            If c.IsSystemColor Then Continue For
            If c.A < 255 Then Continue For ' Transparent etc. raus
            list.Add(New NamedColorItem(c, kc.ToString()))
        Next

        Dim ordered As List(Of NamedColorItem) =
            list.OrderBy(Function(x As NamedColorItem) x.Name, StringComparer.InvariantCultureIgnoreCase).ToList()
        Return ordered
    End Function

    ''' <summary>Gruppiert Farben entlang des HSV-Farbrads + Neutrale.</summary>
    Private Function Groupify(items As List(Of NamedColorItem)) As List(Of ColorGroup)
        Dim Hue As Func(Of Color, Double) =
            Function(c As Color) As Double
                Dim h As Double = c.GetHue()
                If h < 0 Then h = 0
                If h > 360 Then h = 360
                Return h
            End Function

        Dim Sat As Func(Of Color, Double) =
            Function(c As Color) As Double
                Dim r As Double = CDbl(c.R) / 255.0
                Dim g As Double = CDbl(c.G) / 255.0
                Dim b As Double = CDbl(c.B) / 255.0
                Dim maxc As Double = Math.Max(r, Math.Max(g, b))
                Dim minc As Double = Math.Min(r, Math.Min(g, b))
                If maxc = 0 Then Return 0
                If maxc = minc Then Return 0
                Return (maxc - minc) / maxc
            End Function

        Dim ValF As Func(Of Color, Double) =
            Function(c As Color) As Double
                Dim v As Integer = Math.Max(c.R, Math.Max(c.G, c.B))
                Return CDbl(v) / 255.0
            End Function

        Dim neutrals As List(Of NamedColorItem) =
            items.Where(Function(i As NamedColorItem)
                            Dim s As Double = Sat(i.Color)
                            Dim v As Double = ValF(i.Color)
                            Return s <= 0.08 OrElse v <= 0.08 OrElse v >= 0.96
                        End Function).ToList()

        Dim chroma As List(Of NamedColorItem) = items.Except(neutrals).ToList()

        Dim groups As New List(Of ColorGroup) From {
            MakeRangeGroup("Rot", Color.FromArgb(255, 0, 0), chroma, 345, 360, 0, 15, Hue),
            MakeRangeGroup("Orange", Color.FromArgb(255, 128, 0), chroma, 15, 45, Hue),
            MakeRangeGroup("Gelb", Color.FromArgb(255, 215, 0), chroma, 45, 75, Hue),
            MakeRangeGroup("Grün", Color.FromArgb(0, 170, 0), chroma, 75, 165, Hue),
            MakeRangeGroup("Cyan", Color.FromArgb(0, 170, 170), chroma, 165, 195, Hue),
            MakeRangeGroup("Blau", Color.FromArgb(0, 90, 255), chroma, 195, 255, Hue),
            MakeRangeGroup("Magenta", Color.FromArgb(200, 0, 200), chroma, 255, 345, Hue)
        }

        If neutrals.Count > 0 Then
            Dim orderedNeutrals As IEnumerable(Of NamedColorItem) =
                neutrals.OrderBy(Function(i As NamedColorItem) i.Name, StringComparer.InvariantCultureIgnoreCase)
            groups.Add(New ColorGroup("Neutral (Schwarz–Grau–Weiß)", Color.Silver, orderedNeutrals))
        End If

        ' Leere entfernen
        Dim nonEmpty As List(Of ColorGroup) =
            groups.Where(Function(g As ColorGroup) g.Items.Count > 0).ToList()
        Return nonEmpty
    End Function

    Private Function MakeRangeGroup(title As String,
                                    circle As Color,
                                    pool As IEnumerable(Of NamedColorItem),
                                    lo As Double,
                                    hi As Double,
                                    hueFunc As Func(Of Color, Double)) As ColorGroup

        Dim take As IEnumerable(Of NamedColorItem) =
            pool.Where(Function(i As NamedColorItem)
                           Dim h As Double = hueFunc(i.Color)
                           If lo <= hi Then
                               Return h >= lo AndAlso h < hi
                           Else
                               ' Wrap-around
                               Return (h >= lo AndAlso h <= 360.0R) OrElse (h >= 0.0R AndAlso h < hi)
                           End If
                       End Function).
                 OrderBy(Function(i As NamedColorItem) i.Name, StringComparer.InvariantCultureIgnoreCase)

        Return New ColorGroup(title, circle, take)
    End Function

    Private Function MakeRangeGroup(title As String,
                                    circle As Color,
                                    pool As IEnumerable(Of NamedColorItem),
                                    lo1 As Double,
                                    hi1 As Double,
                                    lo2 As Double,
                                    hi2 As Double,
                                    hueFunc As Func(Of Color, Double)) As ColorGroup

        Dim take As IEnumerable(Of NamedColorItem) =
            pool.Where(Function(i As NamedColorItem)
                           Dim h As Double = hueFunc(i.Color)
                           Dim in1 As Boolean = (h >= lo1 AndAlso h < hi1)
                           Dim in2 As Boolean = (h >= lo2 AndAlso h < hi2)
                           Return in1 OrElse in2
                       End Function).
                 OrderBy(Function(i As NamedColorItem) i.Name, StringComparer.InvariantCultureIgnoreCase)

        Return New ColorGroup(title, circle, take)
    End Function

    ' --- Format-Helper ---
    Private Shared Function ToRgbHex(ByVal c As Color) As String
        Return "#" & c.R.ToString("X2") & c.G.ToString("X2") & c.B.ToString("X2")
    End Function

    Private Shared Function ToRgbDez(ByVal c As Color) As String
        Return c.R.ToString() & "," & c.G.ToString() & "," & c.B.ToString()
    End Function

    ' ── Zeichenpanel (2 Spalten, Umbruch nur an Gruppen; runde Farbfelder; Text scrollt mit) ──
    Private NotInheritable Class TwoColumnColorPanel
        Inherits Panel

        ' --- Hilfstypen (statt Tuples) ---
        Private NotInheritable Class LayoutEntry
            Public Property GroupIndex As Integer
            Public Property ItemIndex As Integer   ' -1 = Header
            Public Property Bounds As Rectangle
            Public Property IsHeader As Boolean
        End Class

        Private NotInheritable Class Selection
            Public Property GroupIndex As Integer = -1
            Public Property ItemIndex As Integer = -1
            Public Sub SetTo(g As Integer, i As Integer)
                Me.GroupIndex = g
                Me.ItemIndex = i
            End Sub
            Public Function IsValid() As Boolean
                Return (GroupIndex >= 0 AndAlso ItemIndex >= 0)
            End Function
        End Class

        ' --- Ereignisse/öffentliche Properties ---
        Public Event SelectionChanged As EventHandler

        Public Property TileSize As Size = New Size(LBLWIDTH, LBLHEIGHT)
        Public Property GroupHeaderHeight As Integer = LBLHEIGHT
        Public Property GroupGap As Integer = 12
        Public Property PaddingAll As Integer = 10
        Public Property HGap As Integer = 16
        Public Property VGap As Integer = 8

        Public ReadOnly Property SelectedColor As Color
            Get
                If _selected.IsValid() Then
                    Return _groups(_selected.GroupIndex).Items(_selected.ItemIndex).Color
                End If
                Return Color.Empty
            End Get
        End Property
        Public ReadOnly Property SelectedName As String
            Get
                If _selected IsNot Nothing AndAlso _selected.IsValid() Then
                    Return _groups(_selected.GroupIndex).Items(_selected.ItemIndex).Name
                End If
                Return String.Empty
            End Get
        End Property

        ' --- Felder ---
        Private _groups As List(Of ColorGroup) = New List(Of ColorGroup)()
        Private _selected As Selection = New Selection()
        Private ReadOnly _layout As List(Of LayoutEntry) = New List(Of LayoutEntry)()

        Public Sub New()
            Me.DoubleBuffered = True
            Me.SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.UserPaint Or ControlStyles.OptimizedDoubleBuffer, True)
            Me.AutoScroll = True
            Me.TabStop = True
        End Sub

        Public Sub SetGroups(groups As List(Of ColorGroup))
            _groups = If(groups, New List(Of ColorGroup)())
            _selected = New Selection()
            RebuildLayout()
            Me.Invalidate()
        End Sub

        Public Sub SelectFirst()
            Dim g As Integer
            For g = 0 To _groups.Count - 1
                If _groups(g).Items.Count > 0 Then
                    _selected.SetTo(g, 0)
                    RaiseEvent SelectionChanged(Me, EventArgs.Empty)
                    Me.Invalidate()
                    Exit For
                End If
            Next
        End Sub

        Public Sub SelectClosest(target As Color)
            If _groups.Count = 0 Then Return

            Dim bestG As Integer = -1
            Dim bestI As Integer = -1
            Dim bestScore As Integer = Integer.MaxValue

            Dim g As Integer
            For g = 0 To _groups.Count - 1
                Dim i As Integer
                For i = 0 To _groups(g).Items.Count - 1
                    Dim c As Color = _groups(g).Items(i).Color
                    Dim dR As Integer = CInt(Math.Abs(CInt(c.R) - CInt(target.R)))
                    Dim dG As Integer = CInt(Math.Abs(CInt(c.G) - CInt(target.G)))
                    Dim dB As Integer = CInt(Math.Abs(CInt(c.B) - CInt(target.B)))
                    Dim s As Integer = dR * dR + dG * dG + dB * dB
                    If s < bestScore Then
                        bestScore = s
                        bestG = g
                        bestI = i
                    End If
                Next
            Next

            If bestG >= 0 Then
                _selected.SetTo(bestG, bestI)
                EnsureVisible(_selected)
                RaiseEvent SelectionChanged(Me, EventArgs.Empty)
                Me.Invalidate()
            End If
        End Sub

        Protected Overrides Sub OnSizeChanged(e As EventArgs)
            MyBase.OnSizeChanged(e)
            RebuildLayout()
            Me.Invalidate()
        End Sub

        Private Sub RebuildLayout()
            _layout.Clear()
            If _groups.Count = 0 Then
                Me.AutoScrollMinSize = Size.Empty
                Return
            End If

            Dim pad As Integer = Me.PaddingAll
            Dim colW As Integer = Me.TileSize.Width
            Dim xCol0 As Integer = pad
            Dim xCol1 As Integer = pad + colW + Me.HGap

            ' --- Gruppenhöhen vorab berechnen (Header + Items) ---
            Dim groupHeights As List(Of Integer) = New List(Of Integer)(_groups.Count)
            Dim idx As Integer
            For idx = 0 To _groups.Count - 1
                Dim itemCount As Integer = _groups(idx).Items.Count
                Dim h As Integer = Me.GroupHeaderHeight + Me.VGap + (itemCount * (Me.TileSize.Height + Me.VGap)) + Me.GroupGap
                groupHeights.Add(h)
            Next

            ' Ziel: echte 2-Spalten-Aufteilung immer erzwingen (Umbruch NUR an Überschrift).
            Dim totalContentHeight As Integer = 0
            For Each h As Integer In groupHeights
                totalContentHeight += h
            Next
            Dim halfTarget As Integer = (totalContentHeight + 1) \ 2  ' balanciert

            Dim yCol0 As Integer = pad
            Dim yCol1 As Integer = pad
            Dim currentCol As Integer = 0
            Dim accCol0 As Integer = 0

            Dim g As Integer
            For g = 0 To _groups.Count - 1
                Dim grp As ColorGroup = _groups(g)
                Dim grpH As Integer = groupHeights(g)

                ' Entscheiden, ob wir für Balance in Spalte 2 wechseln (nur an Gruppen-Grenze)
                If currentCol = 0 AndAlso (accCol0 > 0) AndAlso (accCol0 + grpH) > halfTarget Then
                    currentCol = 1
                End If

                Dim x As Integer = If(currentCol = 0, xCol0, xCol1)
                Dim y As Integer = If(currentCol = 0, yCol0, yCol1)

                ' Header
                ' Header
                Dim hdr As LayoutEntry = New LayoutEntry() With {
                    .GroupIndex = g,
                    .ItemIndex = -1,
                    .Bounds = New Rectangle(x, y, colW, Me.GroupHeaderHeight),
                    .IsHeader = True
}
                _layout.Add(hdr)
                y += Me.GroupHeaderHeight
                y += Me.VGap            ' <-- NEU: Abstand wie zwischen Kacheln

                ' Items
                Dim i As Integer
                For i = 0 To grp.Items.Count - 1
                    Dim le As LayoutEntry = New LayoutEntry() With {
                    .GroupIndex = g,
                    .ItemIndex = i,
                    .Bounds = New Rectangle(x, y, colW, Me.TileSize.Height),
                    .IsHeader = False
                }
                    _layout.Add(le)
                    y += Me.TileSize.Height + Me.VGap
                Next
                y += Me.GroupGap

                If currentCol = 0 Then
                    yCol0 = y
                    accCol0 += grpH
                Else
                    yCol1 = y
                End If
            Next

            Dim totalW As Integer = pad + (colW * 2) + Me.HGap + pad
            Dim totalH As Integer = Math.Max(yCol0, yCol1) + pad
            Me.AutoScrollMinSize = New Size(totalW, totalH)
        End Sub

        Protected Overrides Sub OnPaint(e As PaintEventArgs)
            MyBase.OnPaint(e)

            ' Scroll-Translation: Für GDI+ (DrawString etc.) genügt die Welt-Transformation
            e.Graphics.TranslateTransform(Me.AutoScrollPosition.X, Me.AutoScrollPosition.Y)
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic

            Dim dark As Boolean
            Try
                dark = INI.Global_DarkMode
            Catch ex As Exception
                dark = False
            End Try

            Dim back As Color = If(dark, Color.FromArgb(24, 24, 24), SystemColors.Window)
            Dim fore As Color = If(dark, Color.Gainsboro, SystemColors.WindowText)
            Dim frame As Color = If(dark, Color.FromArgb(60, 60, 60), Color.FromArgb(200, 200, 200))

            e.Graphics.Clear(back)

            Using fontName As Font = New Font(Me.Font, FontStyle.Regular),
              fontNameBold As Font = New Font(Me.Font, FontStyle.Bold),
              fontHeader As Font = New Font(Me.Font.FontFamily, Me.Font.Size + 0.5F, FontStyle.Bold),
              penFrame As Pen = New Pen(Color.FromArgb(110, frame))

                Dim sfHeader As StringFormat = New StringFormat() With {
                .LineAlignment = StringAlignment.Center,
                .Alignment = StringAlignment.Near,
                .Trimming = StringTrimming.EllipsisCharacter,
                .FormatFlags = StringFormatFlags.NoWrap
            }
                Dim sfCenter As StringFormat = New StringFormat() With {
                .LineAlignment = StringAlignment.Center,
                .Alignment = StringAlignment.Center,
                .Trimming = StringTrimming.EllipsisCharacter
            }

                Dim entry As LayoutEntry
                For Each entry In _layout
                    Dim rc As Rectangle = entry.Bounds

                    If entry.IsHeader Then
                        Dim grp As ColorGroup = _groups(entry.GroupIndex)
                        Dim dotSize As Integer = 14
                        Dim dotY As Integer = rc.Top + ((rc.Height - dotSize) \ 2)
                        Using brDot As SolidBrush = New SolidBrush(grp.TitleCircleColor)
                            e.Graphics.FillEllipse(brDot, rc.Left, dotY, dotSize, dotSize)
                        End Using
                        Dim textRect As RectangleF = New RectangleF(rc.Left + dotSize + 6, rc.Top, rc.Width - (dotSize + 6), rc.Height)
                        Using brText As SolidBrush = New SolidBrush(fore)
                            e.Graphics.DrawString(grp.Title, fontHeader, brText, textRect, sfHeader)
                        End Using

                    Else
                        Dim grp As ColorGroup = _groups(entry.GroupIndex)
                        Dim itm As NamedColorItem = grp.Items(entry.ItemIndex)

                        ' Farbfeld (abgerundet)
                        Dim colorHeight As Integer = Math.Max(18, CInt(entry.Bounds.Height * 0.58R))
                        Dim rcColor As Rectangle = New Rectangle(rc.Left, rc.Top, rc.Width, colorHeight)
                        Dim radius As Integer = 6
                        Using gp As GraphicsPath = CreateRoundedRect(rcColor, radius),
                          brFill As SolidBrush = New SolidBrush(itm.Color)
                            e.Graphics.FillPath(brFill, gp)
                            e.Graphics.DrawPath(penFrame, gp)
                        End Using

                        ' Text darunter (GDI+ -> scrollt mit)
                        Dim rcText As RectangleF = New RectangleF(rc.Left + 4, rcColor.Bottom + 4, rc.Width - 8, Math.Max(16, rc.Bottom - (rcColor.Bottom + 4)))
                        Dim isSel As Boolean = (_selected.GroupIndex = entry.GroupIndex AndAlso _selected.ItemIndex = entry.ItemIndex)
                        Dim useFont As Font = If(isSel, fontNameBold, fontName)
                        Dim labelText As String = itm.Name & "  " & ColorPickerNamedColors.ToRgbHex(itm.Color)
                        Using brText As SolidBrush = New SolidBrush(fore)
                            e.Graphics.DrawString(labelText, useFont, brText, rcText, sfCenter)
                        End Using

                        If isSel Then
                            Using penSel As Pen = New Pen(Color.FromArgb(180, 0, 120, 215), 2.0F)
                                Dim selRect As Rectangle = Rectangle.Inflate(rc, -1, -1)
                                e.Graphics.DrawRectangle(penSel, selRect)
                            End Using
                        End If
                    End If
                Next
            End Using
        End Sub

        ' --- HitTest/Scroll ---
        Private Function HitTest(pClient As Point) As LayoutEntry
            Dim p As Point = New Point(pClient.X - Me.AutoScrollPosition.X, pClient.Y - Me.AutoScrollPosition.Y)
            Dim entry As LayoutEntry
            For Each entry In _layout
                If (Not entry.IsHeader) AndAlso entry.Bounds.Contains(p) Then
                    Return entry
                End If
            Next
            Return Nothing
        End Function

        Private Sub EnsureVisible(sel As Selection)
            If Not sel.IsValid() Then Return
            Dim entry As LayoutEntry = _layout.FirstOrDefault(Function(t As LayoutEntry) (Not t.IsHeader) AndAlso t.GroupIndex = sel.GroupIndex AndAlso t.ItemIndex = sel.ItemIndex)
            If entry Is Nothing Then Return
            Dim rc As Rectangle = entry.Bounds
            rc.Offset(Me.AutoScrollPosition.X, Me.AutoScrollPosition.Y)
            Me.ScrollControlIntoView(New DummyProxy(rc))
        End Sub

        ' --- Maus/Tastatur ---
        Protected Overrides Sub OnMouseDown(e As MouseEventArgs)
            MyBase.OnMouseDown(e)
            If e.Button <> MouseButtons.Left Then Return
            Dim hit As LayoutEntry = HitTest(e.Location)
            If hit IsNot Nothing Then
                _selected.SetTo(hit.GroupIndex, hit.ItemIndex)
                RaiseEvent SelectionChanged(Me, EventArgs.Empty)
                Me.Invalidate()
            End If
            Me.Focus()
        End Sub

        ' --- Doppelklick übernimmt ---
        Protected Overrides Sub OnMouseDoubleClick(e As MouseEventArgs)
            MyBase.OnMouseDoubleClick(e)
            If e.Button <> MouseButtons.Left Then Return

            Dim hit As LayoutEntry = HitTest(e.Location)
            If hit Is Nothing Then Return

            ' Auswahl setzen
            _selected.SetTo(hit.GroupIndex, hit.ItemIndex)
            RaiseEvent SelectionChanged(Me, EventArgs.Empty)
            Me.Invalidate()

            ' Parent-Form ermitteln und OK auslösen
            Dim frm As Form = TryCast(Me.FindForm(), Form)
            If frm Is Nothing Then Return

            Dim okBtn As Button = FindOkButtonRecursive(frm)
            If okBtn IsNot Nothing Then
                okBtn.PerformClick()
            Else
                ' Fallback: direkt DialogResult setzen und schließen
                frm.DialogResult = DialogResult.OK
                frm.Close()
            End If
        End Sub

        ' Sucht in der gesamten Control-Hierarchie nach einem Button mit DialogResult.OK
        Private Shared Function FindOkButtonRecursive(parent As Control) As Button
            Dim i As Integer
            For i = 0 To parent.Controls.Count - 1
                Dim btn As Button = TryCast(parent.Controls(i), Button)
                If (btn IsNot Nothing) AndAlso (btn.DialogResult = DialogResult.OK) Then
                    Return btn
                End If
                Dim deeper As Button = FindOkButtonRecursive(parent.Controls(i))
                If deeper IsNot Nothing Then Return deeper
            Next
            Return Nothing
        End Function

        Protected Overrides Sub OnKeyDown(e As KeyEventArgs)
            MyBase.OnKeyDown(e)
            If _groups.Count = 0 Then Return

            If Not _selected.IsValid() Then
                SelectFirst()
                e.Handled = True
                Return
            End If

            Dim g As Integer = _selected.GroupIndex
            Dim i As Integer = _selected.ItemIndex

            Select Case e.KeyCode
                Case Keys.Up
                    If i > 0 Then
                        _selected.SetTo(g, i - 1)
                    Else
                        Dim pg As Integer
                        For pg = g - 1 To 0 Step -1
                            If _groups(pg).Items.Count > 0 Then
                                _selected.SetTo(pg, _groups(pg).Items.Count - 1)
                                Exit For
                            End If
                        Next
                    End If

                Case Keys.Down
                    If i < _groups(g).Items.Count - 1 Then
                        _selected.SetTo(g, i + 1)
                    Else
                        Dim ng As Integer
                        For ng = g + 1 To _groups.Count - 1
                            If _groups(ng).Items.Count > 0 Then
                                _selected.SetTo(ng, 0)
                                Exit For
                            End If
                        Next
                    End If

                Case Keys.Left, Keys.Right
                    Dim here As LayoutEntry = _layout.FirstOrDefault(Function(t As LayoutEntry) (Not t.IsHeader) AndAlso t.GroupIndex = g AndAlso t.ItemIndex = i)
                    If here IsNot Nothing Then
                        Dim midY As Integer = here.Bounds.Top + (here.Bounds.Height \ 2)
                        Dim candidates As IEnumerable(Of LayoutEntry)
                        If e.KeyCode = Keys.Left Then
                            candidates =
                            _layout.Where(Function(t As LayoutEntry) (Not t.IsHeader) AndAlso t.Bounds.Right <= here.Bounds.Left - 1 AndAlso
                                                          Math.Abs(((t.Bounds.Top + (t.Bounds.Height \ 2)) - midY)) < 9999).
                                    OrderByDescending(Function(t As LayoutEntry) t.Bounds.Right)
                        Else
                            candidates =
                            _layout.Where(Function(t As LayoutEntry) (Not t.IsHeader) AndAlso t.Bounds.Left >= here.Bounds.Right + 1 AndAlso
                                                          Math.Abs(((t.Bounds.Top + (t.Bounds.Height \ 2)) - midY)) < 9999).
                                    OrderBy(Function(t As LayoutEntry) t.Bounds.Left)
                        End If
                        Dim best As LayoutEntry = candidates.FirstOrDefault()
                        If best IsNot Nothing Then
                            _selected.SetTo(best.GroupIndex, best.ItemIndex)
                        End If
                    End If

                Case Keys.Home
                    Dim gi As Integer
                    For gi = 0 To _groups.Count - 1
                        If _groups(gi).Items.Count > 0 Then
                            _selected.SetTo(gi, 0)
                            Exit For
                        End If
                    Next

                Case Keys.End
                    Dim gi As Integer
                    For gi = _groups.Count - 1 To 0 Step -1
                        If _groups(gi).Items.Count > 0 Then
                            _selected.SetTo(gi, _groups(gi).Items.Count - 1)
                            Exit For
                        End If
                    Next

                Case Else
                    Return
            End Select

            EnsureVisible(_selected)
            RaiseEvent SelectionChanged(Me, EventArgs.Empty)
            Me.Invalidate()
            e.Handled = True
        End Sub

        ' --- Rounded Rect Helper ---
        Private Shared Function CreateRoundedRect(r As Rectangle, radius As Integer) As GraphicsPath
            Dim gp As GraphicsPath = New GraphicsPath()
            Dim d As Integer = radius * 2
            gp.AddArc(r.Left, r.Top, d, d, 180, 90)
            gp.AddArc(r.Right - d, r.Top, d, d, 270, 90)
            gp.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90)
            gp.AddArc(r.Left, r.Bottom - d, d, d, 90, 90)
            gp.CloseFigure()
            Return gp
        End Function

        ' In TwoColumnColorPanel
        Public Sub ScrollSelectionIntoView(Optional ByVal centerVertically As Boolean = True)
            If _groups Is Nothing OrElse _groups.Count = 0 Then Return
            If _selected Is Nothing OrElse Not _selected.IsValid() Then Return

            Dim entry As LayoutEntry = _layout.FirstOrDefault(
        Function(t As LayoutEntry) (Not t.IsHeader) AndAlso
                                   t.GroupIndex = _selected.GroupIndex AndAlso
                                   t.ItemIndex = _selected.ItemIndex)
            If entry Is Nothing Then Return

            Dim viewportH As Integer = Me.ClientSize.Height
            Dim itemH As Integer = entry.Bounds.Height
            Dim itemTop As Integer = entry.Bounds.Top

            Dim desiredY As Integer
            If centerVertically AndAlso viewportH > itemH Then
                desiredY = itemTop - ((viewportH - itemH) \ 2)
            Else
                ' Alternativ: knapp oberhalb anzeigen (Header/Kachel-Padding berücksichtigen)
                desiredY = Math.Max(0, itemTop - Me.PaddingAll)
            End If

            ' Zielbereich begrenzen
            Dim maxY As Integer = Math.Max(0, Me.AutoScrollMinSize.Height - viewportH)
            If desiredY < 0 Then desiredY = 0
            If desiredY > maxY Then desiredY = maxY

            ' X unverändert lassen
            Dim currentX As Integer = Math.Abs(Me.AutoScrollPosition.X)
            Me.AutoScrollPosition = New Point(currentX, desiredY)
            Me.Invalidate()
        End Sub

        ' --- Dummy für ScrollControlIntoView (Bounds „shadows“, nicht Overrides) ---
        Private NotInheritable Class DummyProxy
            Inherits Control
            Private ReadOnly _bounds As Rectangle
            Public Sub New(bounds As Rectangle)
                _bounds = bounds
                Me.SetBounds(bounds.X, bounds.Y, bounds.Width, bounds.Height)
                Me.Visible = False
            End Sub
            Public Shadows ReadOnly Property Bounds As Rectangle
                Get
                    Return _bounds
                End Get
            End Property
        End Class
    End Class
End Class

