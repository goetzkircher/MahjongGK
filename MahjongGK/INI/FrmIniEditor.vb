' ===== FrmIniEditorSimple.vb (no highlight, native RTB undo) =====
Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

Imports System.Drawing.Imaging
Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Text
Imports System.Text.RegularExpressions

''' <summary>
''' Ein einfacher, fokussierter Editor für die Bearbeitung der Basis.ini im Projekt MahjongGK.
''' Damit können keine neuen Einträge erstellt werden (Das geht über die Properties im Modul INI)
''' Es geht um das Anpassen/Ändern bestehender Werte, solange kein UCtlEigenschaften programmiert ist.
''' Er kann nur innerhalb der IDE aufgerufen werden links unten im FrmMain Klick auf "INI"</summary>
Public Class FrmIniEditor
    Inherits Form


    'INI Editor(MahjongGK)

    'Features
    'Laden & Speichern
    'Öffnet automatisch Basis.ini im AppData-Pfad (...\MahjongGK\INI\Basis.ini).
    'Speichert in UTF-8 mit BOM.
    'Syntax-Highlight beim Laden
    'Sektionen [Abschnitt] → fett, grün
    'Keys Name = ... → orange
    'Equals = → grün
    'Values → weiß/schwarz (abhängig vom Schema)
    'Kommentare ;… nur am Zeilenanfang → grün
    'Rehighlight
    'Menüpunkt zum Neuanwenden der Highlight-Regeln, z. B. nach größeren Änderungen.
    'Dark/Light-Umschalter löst automatisch ein Rehighlight aus.
    'Dark / Light Mode
    'Umschaltbar über das Menü, passt Farben des Editors und der Syntax an.
    'Colors-Menü
    'Unterpunkte Section, Comment, Key, Equals, Value
    'Klick = Schreib-/Auswahlfarbe auf aktuellen Schemawert setzen
    'Mit Farbswatches als optische Hilfe
    'Menüpunkt Font: öffnet den Windows-Fontdialog
    'Undo/Redo
    'Nutzt die native RichTextBox-Funktion.
    'Keine künstlichen Einträge durch Highlighting.
    'Kontextmenü
    'Ausschneiden, Kopieren, Einfügen (plain text), Löschen, Alles auswählen.
    'Nutzung
    'Änderungen an Werten vornehmen → Speichern (Strg+S).
    'Bei Bedarf Rehighlight ausführen (Menü Colors → Rehighlight).
    'Dark/Light Mode nach Geschmack umschalten.
    'Schriftart jederzeit per Menü ändern.


    Private ReadOnly _rtb As New RichTextBox()
    Private ReadOnly _menu As New MenuStrip()

    Private ReadOnly _miDark As New ToolStripMenuItem("Darkmode") With {.CheckOnClick = True}
    Private ReadOnly _miUndo As New ToolStripMenuItem("DoUndo")
    Private ReadOnly _miRedo As New ToolStripMenuItem("Redo")
    Private ReadOnly _miCancel As New ToolStripMenuItem("Abbrechen")
    Private ReadOnly _miSave As New ToolStripMenuItem("Speichern")
    Private ReadOnly _miWordWrap As New ToolStripMenuItem("Wordwrap") With {.CheckOnClick = True}

    ' --- Colors-Menü ---
    Private ReadOnly _miColors As New ToolStripMenuItem("Colors")
    Private ReadOnly _miColSection As New ToolStripMenuItem("Selection") ' Sektion
    Private ReadOnly _miColComment As New ToolStripMenuItem("Comment")
    Private ReadOnly _miColKey As New ToolStripMenuItem("Key")
    Private ReadOnly _miColEquals As New ToolStripMenuItem("Equals")
    Private ReadOnly _miColValue As New ToolStripMenuItem("Value")
    Private ReadOnly _miFont As New ToolStripMenuItem("Font")

    Private ReadOnly _cboKeys As New ComboBox
    Private ReadOnly _cboSektion As New ComboBox

    Private ReadOnly _fontDlg As New FontDialog() With {.ShowEffects = True}

    Private ReadOnly _cms As New ContextMenuStrip()

    Private ReadOnly _lblMark As New Label() With {
        .Text = "➤",
        .AutoSize = True,
        .Font = New Font(SystemFonts.DefaultFont, FontStyle.Bold)
    }

    Private _darkMode As Boolean = INI.IfRunningInIDE_IniEditorDarkmode
    Private _scheme As ColorScheme = ColorScheme.DarkDefault

    Private _iniFullLoadPath As String = String.Empty
    Private _iniFullSavePath As String = String.Empty
    Private _isDirty As Boolean

    <DllImport("user32.dll", CharSet:=CharSet.Auto, EntryPoint:="SendMessageW")>
    Private Shared Function SendMessageMargins(hWnd As IntPtr, msg As Integer, wParam As IntPtr, lParam As IntPtr) As IntPtr
    End Function

    Private Const EM_SETMARGINS As Integer = &HD3
    Private Const EC_LEFTMARGIN As Integer = &H1
    Private Const EC_RIGHTMARGIN As Integer = &H2

    Public Sub New()

        Me.Text = "INI Editor"
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.Icon = My.Resources.MahjongGK
        Me.KeyPreview = True


        ' Zielgröße 1400x650 – an Arbeitsbereich anpassen
        Dim work As Rectangle = Screen.PrimaryScreen.WorkingArea
        Dim target As New Size(1400, 650)
        Dim w As Integer = Math.Min(target.Width, Math.Max(800, work.Width - 40))
        Dim h As Integer = Math.Min(target.Height, Math.Max(450, work.Height - 80))
        Me.Size = New Size(w, h)
        Me.MinimumSize = New Size(700, 400)

        ConfigureMenu()
        ConfigureRichTextBox()
        ConfigureContextMenu()

        Me.SuspendLayout()
        Controls.Add(_menu)

        Controls.Add(_rtb)

        Controls.Add(_lblMark)

        ' Sicherstellen, dass der MenuStrip beim Docking „zuerst“ berücksichtigt wird:
        ' (Index 0 = ganz oben in der Z-Order)
        Controls.SetChildIndex(_menu, 1)
        Controls.SetChildIndex(_rtb, 0)

        Me.ResumeLayout(True)
        Me.PerformLayout()

        _lblMark.BringToFront()


        AddHandler _rtb.TextChanged, AddressOf Rtb_TextChanged
        AddHandler Me.FormClosing, AddressOf Frm_FormClosing
        AddHandler Me.Shown, AddressOf Frm_Shown
        AddHandler _rtb.HandleCreated, Sub(_s, _e) SetRtbMargins(18, 5)

        ' Pfad zur INI-Datei ermitteln (mit Fallback)
        Try
            _iniFullLoadPath = Path.Combine(AppDataDirectory(AppDataSubDir.INI), "Basis.ini")
            _iniFullSavePath = _iniFullLoadPath & ".tmp"
        Catch
            Dim fallbackDir As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MahjongGK", "INI")
            Directory.CreateDirectory(fallbackDir)
            _iniFullLoadPath = Path.Combine(fallbackDir, "Basis.ini")
            _iniFullSavePath = _iniFullLoadPath & ".tmp"
        End Try

        ApplyScheme(_scheme)
    End Sub

    Dim _IniFileChanged As Boolean
    ReadOnly Property IniFileChanged As Boolean
        Get
            Return _IniFileChanged
        End Get
    End Property

    Private Sub ConfigureMenu()

        _menu.Dock = DockStyle.Top
        _menu.GripStyle = ToolStripGripStyle.Hidden

        AddHandler _miDark.CheckedChanged, Sub() ToggleDarkLight(_miDark.Checked)
        AddHandler _miUndo.Click, Sub() If _rtb.CanUndo Then _rtb.Undo()
        AddHandler _miRedo.Click, Sub() If _rtb.CanRedo Then _rtb.Redo()
        AddHandler _miCancel.Click, Sub()
                                        Me.DialogResult = DialogResult.Cancel
                                        Me.Close()
                                    End Sub
        AddHandler _miSave.Click, AddressOf Save_Click
        AddHandler _miWordWrap.CheckedChanged, Sub() _rtb.WordWrap = _miWordWrap.Checked

        _menu.Items.AddRange(New ToolStripItem() {
            _miDark, New ToolStripSeparator(),
            _miUndo, _miRedo, New ToolStripSeparator(),
            _miCancel, _miSave, New ToolStripSeparator(),
            _miWordWrap
        })

        _miSave.ShortcutKeys = Keys.Control Or Keys.S
        _miUndo.ShortcutKeys = Keys.Control Or Keys.Z
        _miRedo.ShortcutKeys = Keys.Control Or Keys.Y

        ' Colors-Untermenü zusammenbauen
        _miColors.DropDownItems.AddRange(New ToolStripItem() {
            _miColSection,
            _miColComment,
            _miColKey,
            _miColEquals,
            _miColValue,
            New ToolStripSeparator(), ' <-- das ist der Trennstrich
            _miFont
        })



        AddHandler _miColSection.Click, Sub() SetTypingColor(_scheme.SectionColor)
        AddHandler _miColComment.Click, Sub() SetTypingColor(_scheme.CommentColor)
        AddHandler _miColKey.Click, Sub() SetTypingColor(_scheme.KeyColor)
        AddHandler _miColEquals.Click, Sub() SetTypingColor(_scheme.EqualsColor)
        AddHandler _miColValue.Click, Sub() SetTypingColor(_scheme.ValueColor)

        AddHandler _miFont.Click,
    Sub()
        _fontDlg.Font = _rtb.Font
        If _fontDlg.ShowDialog(Me) = DialogResult.OK Then
            _rtb.Font = _fontDlg.Font
            ' ggf. Margins neu setzen
            ' SetRtbMargins(10,0)
            RefreshHighlight()
        End If
    End Sub

        AddHandler _cboKeys.SelectedIndexChanged, Sub() JumpToSelectedKey()
        AddHandler _cboSektion.SelectedIndexChanged, Sub() JumpToSelectedSection()

        ' Colors-Menü im Hauptmenü einhängen
        _menu.Items.Add(New ToolStripSeparator())
        _menu.Items.Add(_miColors)

        RefreshColorSwatches()

        _cboKeys.DropDownStyle = ComboBoxStyle.DropDownList
        _cboKeys.Width = 450

        _cboSektion.DropDownStyle = ComboBoxStyle.DropDownList
        _cboSektion.Width = 300

        ' In ToolStripControlHost packen
        Dim host1 As New ToolStripControlHost(_cboKeys)
        host1.Alignment = ToolStripItemAlignment.Right  ' Ganz rechts platzieren
        Dim host2 As New ToolStripControlHost(_cboSektion)
        host2.Alignment = ToolStripItemAlignment.Right  ' Ganz rechts platzieren

        _menu.Items.Add(New ToolStripSeparator())
        ' Ins MenuStrip einfügen
        _menu.Items.Add(host1)
        _menu.Items.Add(host2)

    End Sub

    Private Sub ConfigureRichTextBox()
        _rtb.Dock = DockStyle.Fill
        _rtb.BorderStyle = BorderStyle.None
        _rtb.HideSelection = False
        _rtb.AcceptsTab = True
        _rtb.WordWrap = False ' Standard laut Vorgabe
        _rtb.ScrollBars = RichTextBoxScrollBars.Both
        _rtb.DetectUrls = False
        _rtb.Font = PickEditorFont()

        ' Einmal z. B. in Form_Load:
        AddHandler _rtb.VScroll, AddressOf HideMarkerOnScroll
        AddHandler _rtb.HScroll, AddressOf HideMarkerOnScroll
        AddHandler _rtb.MouseWheel, AddressOf HideMarkerOnScroll
        AddHandler _rtb.Resize, AddressOf HideMarkerOnScroll   ' bei Größenänderung sicherheitshalber
        AddHandler _rtb.KeyDown, AddressOf HideMarkerOnScroll  ' PgUp/PgDn/Arrow scrollen oft mit

    End Sub

    Private Shared Function PickEditorFont() As Font
        ' Bevorzugt Cascadia Mono 11, sonst Consolas, sonst Courier New
        Dim families As HashSet(Of String) = FontFamily.Families.Select(Function(ff) ff.Name).ToHashSet(StringComparer.OrdinalIgnoreCase)
        If families.Contains("Cascadia Mono") Then Return New Font("Cascadia Mono", 14.0F, FontStyle.Regular, GraphicsUnit.Point)
        If families.Contains("Consolas") Then Return New Font("Consolas", 14.0F, FontStyle.Regular, GraphicsUnit.Point)
        Return New Font("Courier New", 14.0F, FontStyle.Regular, GraphicsUnit.Point)
    End Function

    Private Sub ConfigureContextMenu()
        Dim miCut As New ToolStripMenuItem("Ausschneiden")
        Dim miCopy As New ToolStripMenuItem("Kopieren")
        Dim miPaste As New ToolStripMenuItem("Einfügen")
        Dim miDelete As New ToolStripMenuItem("Löschen")
        Dim miSelectAll As New ToolStripMenuItem("Alles auswählen")

        AddHandler miCut.Click, Sub() _rtb.Cut()
        AddHandler miCopy.Click, Sub() _rtb.Copy()
        AddHandler miPaste.Click, Sub() _rtb.Paste()
        AddHandler miDelete.Click, Sub() If _rtb.SelectionLength > 0 Then _rtb.SelectedText = String.Empty
        AddHandler miSelectAll.Click, Sub() _rtb.SelectAll()

        _cms.Items.AddRange(New ToolStripItem() {miCut, miCopy, miPaste, miDelete, New ToolStripSeparator(), miSelectAll})
        _rtb.ContextMenuStrip = _cms
    End Sub

    Private Sub Frm_Shown(sender As Object, e As EventArgs)
        LoadIniFile(hlight:=True, _darkMode)
        _rtb.Focus()
    End Sub

    Private Sub LoadIniFile(hlight As Boolean, darkmode As Boolean)
        Try
            If File.Exists(_iniFullLoadPath) Then
                Using fs As New FileStream(_iniFullLoadPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
                    Using sr As New StreamReader(fs, True)
                        If hlight Then
                            Dim cs As ColorScheme
                            If darkmode Then
                                cs = ColorScheme.DarkDefault
                            Else
                                cs = ColorScheme.LightDefault
                            End If
                            _rtb.Rtf = Highlight(sr.ReadToEnd, cs, _rtb.Font.Name, _rtb.Font.Size)
                        Else
                            _rtb.Text = sr.ReadToEnd()
                        End If
                    End Using
                End Using
                _isDirty = False
            Else
                MessageBox.Show(Me, _iniFullLoadPath & " Nicht gefunden, Programmabbruch.",
                            "Ende", MessageBoxButtons.OK, MessageBoxIcon.Information)
                _isDirty = False
                Me.DialogResult = DialogResult.Abort
            End If
        Catch ex As Exception
            MessageBox.Show(Me, "Fehler beim Lesen der INI:" & Environment.NewLine & ex.Message,
                            "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

        FillCboKeys(String.Empty)
        FillCboSections()

    End Sub

    Private Sub Save_Click(sender As Object, e As EventArgs)
        ShowTransientMenuText(_miSave, "gespeichert")
        SaveIniFile()
    End Sub

    Private Sub SaveIniFile()

        If String.IsNullOrWhiteSpace(_rtb.Text) Then
            Try
                IO.File.Delete(_iniFullSavePath)
                IO.File.Delete(_iniFullLoadPath)
                Application.DoEvents()
                _isDirty = False
                _IniFileChanged = True
                Me.DialogResult = DialogResult.OK
            Catch ex As Exception

            End Try
            Exit Sub
        End If

        If _isDirty Then
            Try
                Directory.CreateDirectory(Path.GetDirectoryName(_iniFullSavePath))
                Using sw As New StreamWriter(_iniFullSavePath, append:=False, encoding:=New UTF8Encoding(encoderShouldEmitUTF8Identifier:=True))
                    sw.Write(_rtb.Text)
                End Using
                _isDirty = False
                _IniFileChanged = True
                Me.DialogResult = DialogResult.OK
            Catch ex As Exception
                MessageBox.Show(Me, "Fehler beim Speichern der INI:" & Environment.NewLine & ex.Message,
                                "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub

    Private Sub Frm_FormClosing(sender As Object, e As FormClosingEventArgs)
        If _isDirty Then
            _IniFileChanged = True
            Dim ans As DialogResult = MessageBox.Show(Me, "Änderungen speichern?", "INI Editor",
                                      MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question)
            If ans = DialogResult.Cancel Then
                e.Cancel = True
                Return
            ElseIf ans = DialogResult.Yes Then
                SaveIniFile()
            End If
        End If
    End Sub

    Private Sub Rtb_TextChanged(sender As Object, e As EventArgs)
        _isDirty = True
    End Sub

    Private Sub ToggleDarkLight(isDark As Boolean)
        _darkMode = isDark
        _scheme = If(isDark, ColorScheme.DarkDefault, ColorScheme.LightDefault)
        ApplyScheme(_scheme)
        _rtb.Rtf = Highlight(_rtb.Text, _scheme, _rtb.Font.Name, _rtb.Font.Size)
    End Sub

    Private Sub ApplyScheme(cs As ColorScheme)
        _lblMark.BackColor = cs.BackColor
        _lblMark.ForeColor = cs.KeyColor
        _cboKeys.BackColor = cs.BackColor
        _cboKeys.ForeColor = cs.KeyColor
        _cboKeys.Font = _rtb.Font
        _cboSektion.BackColor = cs.BackColor
        _cboSektion.ForeColor = cs.KeyColor
        _cboSektion.Font = _rtb.Font
        _rtb.BackColor = cs.BackColor
        _rtb.ForeColor = cs.ValueColor
        Me.BackColor = cs.BackColor
        _menu.BackColor = cs.MenuBack
        _menu.ForeColor = cs.MenuFore
        For Each it As ToolStripItem In _menu.Items
            it.ForeColor = cs.MenuFore
        Next
        RefreshColorSwatches()
    End Sub

    Public Structure ColorScheme
        Public BackColor As Color
        Public MenuBack As Color
        Public MenuFore As Color
        Public SectionColor As Color ' (aktuell ungenutzt)
        Public CommentColor As Color ' (aktuell ungenutzt)
        Public KeyColor As Color     ' (aktuell ungenutzt)
        Public EqualsColor As Color  ' (aktuell ungenutzt)
        Public ValueColor As Color

        Public Shared ReadOnly DarkDefault As ColorScheme = New ColorScheme With {
            .BackColor = Color.Black,
            .MenuBack = Color.FromArgb(&HFF222222),
            .MenuFore = Color.White,
            .SectionColor = Color.FromArgb(&HFF00FF00),
            .CommentColor = Color.FromArgb(&HFF00FF00),
            .KeyColor = Color.FromArgb(&HFFFF6600),
            .EqualsColor = Color.FromArgb(&HFF00FF00),
            .ValueColor = Color.FromArgb(&HFFFFFFFF)
        }

        Public Shared ReadOnly LightDefault As ColorScheme = New ColorScheme With {
            .BackColor = Color.White,
            .MenuBack = SystemColors.Control,
            .MenuFore = SystemColors.ControlText,
            .SectionColor = Color.Blue,
            .CommentColor = Color.FromArgb(0, 128, 0),
            .KeyColor = Color.FromArgb(180, 60, 0),
            .EqualsColor = Color.FromArgb(0, 128, 0),
            .ValueColor = Color.Black
        }
    End Structure

    Public Shared Function ShowIniEditor(owner As IWin32Window) As DialogResult
        Using f As New FrmIniEditor()
            Return f.ShowDialog(owner)
        End Using
    End Function

    Private Sub SetRtbMargins(leftPx As Integer, rightPx As Integer)
        If _rtb Is Nothing OrElse _rtb.IsDisposed OrElse Not _rtb.IsHandleCreated Then Return
        ' MAKELONG(right, left)
        Dim wParam As IntPtr = CType(EC_LEFTMARGIN Or EC_RIGHTMARGIN, IntPtr)
        Dim lParam As IntPtr = CType(((rightPx And &HFFFF) << 16) Or (leftPx And &HFFFF), IntPtr)
        SendMessageMargins(_rtb.Handle, EM_SETMARGINS, wParam, lParam)
    End Sub

    Private Sub RefreshHighlight()
        With _rtb
            .Rtf = Highlight(.Text, _scheme, _rtb.Font.Name, _rtb.Font.Size)
        End With
    End Sub

    Private Sub SetTypingColor(c As Color)
        If _rtb.SelectionLength > 0 Then
            _rtb.SelectionColor = c
        Else
            _rtb.SelectionColor = c
        End If
        _rtb.Focus()
    End Sub

    Private Function MakeColorSwatch(col As Color) As Image
        Dim bmp As New Bitmap(16, 16, PixelFormat.Format32bppPArgb)
        Using g As Graphics = Graphics.FromImage(bmp)
            g.Clear(col)
            g.DrawRectangle(Pens.Black, 0, 0, bmp.Width - 1, bmp.Height - 1)
        End Using
        Return bmp
    End Function

    Private Sub RefreshColorSwatches()
        _miColSection.Image = MakeColorSwatch(_scheme.SectionColor)
        _miColComment.Image = MakeColorSwatch(_scheme.CommentColor)
        _miColKey.Image = MakeColorSwatch(_scheme.KeyColor)
        _miColEquals.Image = MakeColorSwatch(_scheme.EqualsColor)
        _miColValue.Image = MakeColorSwatch(_scheme.ValueColor)
    End Sub

    Private Sub FillCboKeys(selection As String)

        _cboKeys.Items.Clear()
        Dim skipBefore As Boolean = Not String.IsNullOrEmpty(selection)
        If selection = "[Alle Sektionen]" Then
            skipBefore = False
        End If

        Dim skipAfter As Boolean = False

        ' Text aus RichTextBox zeilenweise durchgehen
        For Each line As String In _rtb.Lines
            Dim trimmed As String = line.Trim()

            ' Leere Zeilen, Kommentare und Folder überspringen
            If trimmed.Length = 0 Then
                Continue For
            End If

            If trimmed.StartsWith(";") Then
                Continue For
            End If

            If skipBefore Then
                If trimmed.StartsWith(selection) Then
                    skipAfter = True
                    skipBefore = False
                End If
                Continue For
            ElseIf skipAfter Then
                If trimmed.StartsWith("[") Then
                    Exit For
                End If
            End If

            If trimmed.StartsWith("[") Then
                Continue For
            End If

            ' Key = Value trennen
            Dim parts() As String = trimmed.Split({"="c}, 2)
            If parts.Length >= 1 Then
                Dim key As String = parts(0).Trim()
                If key.Length > 0 Then
                    _cboKeys.Items.Add(key)
                End If
            End If
        Next

        _cboKeys.Sorted = True

    End Sub

    Private Sub FillCboSections()


        _cboSektion.Items.Clear()
        _cboSektion.Items.Add("[Alle Sektionen]")
        ' Text aus RichTextBox zeilenweise durchgehen
        For Each line As String In _rtb.Lines
            Dim trimmed As String = line.Trim()

            ' Leere Zeilen, Kommentare und Keys überspringen
            If trimmed.Length = 0 Then
                Continue For
            End If
            If trimmed.Length > 0 AndAlso trimmed.StartsWith("[") Then
                _cboSektion.Items.Add(trimmed)
            End If
        Next

        _cboSektion.Sorted = True
    End Sub

    ' --- Win32 für exaktes Scrollen/Ermitteln der ersten sichtbaren Zeile ---

    Private Const EM_GETFIRSTVISIBLELINE As Integer = &HCE
    Private Const EM_LINESCROLL As Integer = &HB6

    <DllImport("user32.dll", CharSet:=CharSet.Auto)>
    Private Shared Function SendMessage(hWnd As IntPtr, msg As Integer, wParam As IntPtr, lParam As IntPtr) As IntPtr
    End Function

    Private Sub JumpToSelectedKey()
        JumpToSelectedItem(True)
    End Sub

    Private Sub JumpToSelectedSection()
        JumpToSelectedItem(False)
        FillCboKeys(_cboSektion.Text.Trim())
    End Sub

    Private Sub JumpToSelectedItem(isKey As Boolean)

        Dim item As String
        If isKey Then
            item = _cboKeys.Text.Trim()
        Else
            item = _cboSektion.Text.Trim()
        End If

        If item.Length = 0 OrElse _rtb.TextLength = 0 Then Exit Sub

        Dim lines() As String = _rtb.Lines
        For i As Integer = 0 To lines.Length - 1
            Dim raw As String = lines(i)
            Dim ls As String = raw.TrimStart()

            If isKey Then
                If ls.StartsWith(";") Then Continue For
                If ls.StartsWith("[") Then Continue For
            Else
                If Not ls.StartsWith("[") Then Continue For
            End If

            Dim eqPos As Integer
            If isKey Then
                eqPos = ls.IndexOf("="c)
                If eqPos <= 0 Then
                    Continue For
                End If
            Else
                eqPos = ls.Length
            End If

            Dim leftKey As String = ls.Substring(0, eqPos).Trim()
            If leftKey.Equals(item, StringComparison.OrdinalIgnoreCase) Then
                Dim lineStart As Integer = _rtb.GetFirstCharIndexFromLine(i)
                If lineStart < 0 Then Exit Sub

                ' Caret ANS ENDE DER ZEILE setzen
                Dim lineEnd As Integer = lineStart + raw.Length
                _rtb.Select(lineEnd, 0)

                ' Erst sichtbar machen …
                _rtb.ScrollToCaret()

                ' … dann exakt vertikal zentrieren (mit leichtem Offset nach unten)
                Dim caretPt As Point = CenterCaretVertically(_rtb, offsetLinesBelowCenter:=1)
                ShowLabelMark(caretPt, korrekturOffsetY:=42)
                _rtb.Focus()
                Exit Sub
            End If
        Next
    End Sub


    ' Zentriert die aktuelle Caret-Position vertikal im sichtbaren Bereich.
    ' offsetLinesBelowCenter: >0 = Zeile etwas UNTER die Mitte schieben (mehr Platz für Kommentar oben)
    Private Function CenterCaretVertically(rtb As RichTextBox, Optional offsetLinesBelowCenter As Integer = 0) As Point

        If rtb.ClientSize.Height <= 0 Then
            Return New Point
        End If

        Dim caretIdx As Integer = rtb.SelectionStart
        Dim caretPt As Point = rtb.GetPositionFromCharIndex(caretIdx)

        Dim lineHeight As Integer = Math.Max(1, rtb.Font.Height)
        Dim targetY As Integer = (rtb.ClientSize.Height \ 2) + (offsetLinesBelowCenter * lineHeight)

        ' Differenz in Pixel -> in Zeilen runden
        Dim deltaPixels As Integer = caretPt.Y - targetY
        Dim dLines As Integer = CInt(Math.Round(deltaPixels / CDbl(lineHeight), MidpointRounding.AwayFromZero))

        If dLines <> 0 Then
            ' Positive dLines -> runterscrollen (Text nach oben), negative -> hochscrollen
            SendMessage(rtb.Handle, EM_LINESCROLL, IntPtr.Zero, CType(dLines, IntPtr))

            ' Feintuning: einmal nachjustieren falls noch deutlich daneben
            caretPt = rtb.GetPositionFromCharIndex(caretIdx)
            deltaPixels = caretPt.Y - targetY
            If Math.Abs(deltaPixels) > lineHeight \ 2 Then
                Dim adjust As Integer = If(deltaPixels > 0, 1, -1)
                SendMessage(rtb.Handle, EM_LINESCROLL, IntPtr.Zero, CType(adjust, IntPtr))
            End If

        End If

        Return caretPt

    End Function

    ''' <summary>
    ''' Positioniert das Label auf das selektierte Wort.
    ''' Unsichtbar Schalten durch Übergabe eines leeren pt.
    ''' </summary>
    ''' <param name="pt"></param>
    ''' <param name="left"></param>
    ''' <param name="korrekturOffsetX"></param>
    ''' <param name="korrekturOffsetY"></param>
    Private Sub ShowLabelMark(pt As Point, Optional left As Integer = -1, Optional korrekturOffsetX As Integer = 0, Optional korrekturOffsetY As Integer = 0)

        If pt.IsEmpty Then
            _lblMark.Visible = False
        Else
            _lblMark.Top = pt.Y + korrekturOffsetY
            If left < 0 Then
                _lblMark.Left = korrekturOffsetX
            ElseIf left >= 0 Then
                _lblMark.Left = left + korrekturOffsetX
            Else
                _lblMark.Left = pt.X + korrekturOffsetX
            End If
            _lblMark.Visible = True
        End If

    End Sub

    Private Sub HideMarkerOnScroll(sender As Object, e As EventArgs)
        ShowLabelMark(New Point)
    End Sub
    '###############################################################################################

    Public Shared Function Highlight(text As String,
                                 schema As ColorScheme,
                                 Optional fontName As String = "Cascadia Mono",
                                 Optional fontSizePt As Single = 11.0F) As String
        ' schema: erwartet ein Record/Struct mit .ValueColor, .KeyColor, .EqualsColor, .CommentColor, .SectionColor
        ' Build RTF: colortbl Indexe -> 1: Value, 2: Key, 3: '=', 4: Comment, 5: Section
        Dim src As String = If(text, String.Empty)
        Dim sb As New StringBuilder(src.Length * 2 + 256)

        sb.Append("{\rtf1\ansi\deff0")
        sb.Append("{\fonttbl{\f0 ").Append(EscapeRtf(fontName)).Append(";}}")
        sb.Append("{\colortbl;")
        sb.Append(RtfColor(schema.ValueColor))   ' \cf1
        sb.Append(RtfColor(schema.KeyColor))     ' \cf2
        sb.Append(RtfColor(schema.EqualsColor))  ' \cf3
        sb.Append(RtfColor(schema.CommentColor)) ' \cf4
        sb.Append(RtfColor(schema.SectionColor)) ' \cf5
        sb.Append("}")

        sb.Append("\viewkind4\uc1\f0\fs").Append(CInt(fontSizePt * 2)).Append(" ")

        ' Zeilen normalisieren
        Dim lines() As String = src.Replace(vbCrLf, vbLf).Replace(vbCr, vbLf).Split(ChrW(10))

        Dim reSection As New Regex("^\s*\[[^\]\r\n]+\]\s*$", RegexOptions.Compiled)
        ' Kommentar nur, wenn ; erstes Nicht-Leerzeichen ist:
        Dim reCommentOnly As New Regex("^\s*;.*$", RegexOptions.Compiled)

        ' Alt (problematisch für Umlaute):
        ' Dim reKey As New Regex("^\s*([A-Za-z0-9_\.\(\)]+)\s*(=)\s*(.*)$", RegexOptions.Compiled)

        ' Neu (Unicode-freundlich, erlaubt Umlaute und Bindestrich):
        Dim reKey As New Regex("^\s*([-\p{L}\p{M}\p{N}_\.\(\)]+)\s*(=)\s*(.*)$", RegexOptions.Compiled)


        For Each ln As String In lines
            ' 1) Section
            If reSection.IsMatch(ln) Then
                sb.Append("\b\cf5 ").Append(EscapeRtf(ln.TrimEnd())).Append("\b0\cf1\par")
                Continue For
            End If

            ' 2) Kommentar-Zeile (Semikolon am Zeilenanfang, evtl. nach Whitespaces)
            If reCommentOnly.IsMatch(ln) Then
                sb.Append("\cf4 ").Append(EscapeRtf(ln.TrimEnd())).Append("\cf1\par")
                Continue For
            End If

            ' 3) Key = Value (Achtung: Semikola IM Value bleiben Value!)
            Dim m As Match = reKey.Match(ln)
            If m.Success Then
                ' key
                sb.Append("\cf2 ").Append(EscapeRtf(m.Groups(1).Value))
                ' =
                sb.Append("\cf3 ").Append(EscapeRtf(m.Groups(2).Value))
                ' value (ganzer Rest, inkl. evtl. Semikolons)
                sb.Append("\cf1 ").Append(EscapeRtf(m.Groups(3).Value.TrimEnd()))
                sb.Append("\par")
            Else
                ' 4) sonst: normaler Text
                sb.Append("\cf1 ").Append(EscapeRtf(ln.TrimEnd())).Append("\par")
            End If
        Next

        sb.Append("}")
        Return sb.ToString()
    End Function

    ' --- Helfer ---

    Private Shared Function RtfColor(c As Color) As String
        Return "\red" & c.R & "\green" & c.G & "\blue" & c.B & ";"
    End Function

    Private Shared Function EscapeRtf(s As String) As String
        If String.IsNullOrEmpty(s) Then Return String.Empty
        Dim sb As New StringBuilder(s.Length + 16)
        For Each ch As Char In s
            Select Case ch
                Case "\"c : sb.Append("\\")
                Case "{"c : sb.Append("\{")
                Case "}"c : sb.Append("\}")
                Case CChar(vbTab) : sb.Append("\tab ")
                Case Else
                    Dim code As Integer = AscW(ch)
                    If code < 32 OrElse code > 126 Then
                        sb.Append("\u").Append(code).Append("?")
                    Else
                        sb.Append(ch)
                    End If
            End Select
        Next
        Return sb.ToString()
    End Function

    Private Sub InitializeComponent()
        Me.SuspendLayout()
        '
        'FrmIniEditorSimple
        '
        Me.ClientSize = New System.Drawing.Size(284, 261)
        Me.Name = "FrmIniEditorSimple"
        Me.ResumeLayout(False)

    End Sub
    '
    ''' <summary>
    ''' Bequemer Fire-and-Forget-Wrapper, falls du nicht "Await" benutzen willst.
    ''' </summary>
    <DebuggerStepThrough>
    Public Sub ShowTransientMenuText(item As ToolStripMenuItem,
                                     tempText As String,
                                     Optional durationMs As Integer = 1000)
        Dim unused As Task = ShowTransientMenuTextAsync(item, tempText, durationMs)
    End Sub
    '
    ''' <summary>
    ''' Zeigt für kurze Zeit einen Ersatz-Text auf einem ToolStripMenuItem
    ''' (z.B. "gespeichert") und stellt danach den Originaltext wieder her.
    ''' Blockiert die UI nicht.
    ''' </summary>
    ''' <param name="item">Das betroffene Menü-Item.</param>
    ''' <param name="tempText">Der temporär anzuzeigende Text.</param>
    ''' <param name="durationMs">Anzeigedauer in Millisekunden (Default: 1000ms).</param>
    <DebuggerStepThrough>
    Public Async Function ShowTransientMenuTextAsync(item As ToolStripMenuItem,
                                                     tempText As String,
                                                     Optional durationMs As Integer = 1000) As Task
        If item Is Nothing Then Return

        Dim originalText As String = item.Text
        Try
            item.Text = tempText
            Dim owner As ToolStrip = item.Owner
            If owner IsNot Nothing Then
                owner.Invalidate()
                owner.Update()
            End If

            Await Task.Delay(durationMs).ConfigureAwait(True)
        Finally
            item.Text = originalText
            Dim owner As ToolStrip = item.Owner
            If owner IsNot Nothing Then
                owner.Invalidate()
                owner.Update()
            End If
        End Try
    End Function


End Class

