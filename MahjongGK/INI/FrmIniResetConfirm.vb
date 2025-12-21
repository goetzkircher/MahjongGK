Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

Public NotInheritable Class FrmIniResetConfirm
    Inherits Form

    Public Enum IniResetChoice
        AlleIni
        AktiveIni
        AktiveIniRestart
        Abbrechen
    End Enum

    Private ReadOnly _lbl As Label
    Private ReadOnly _pic As PictureBox
    Private ReadOnly _btnAlle As Button
    Private ReadOnly _btnAktive As Button
    Private ReadOnly _btnAktiveAppExit As Button
    Private ReadOnly _btnCancel As Button
    Private _result As IniResetChoice = IniResetChoice.Abbrechen

    Private Sub New()
        ' Form-Basics
        Me.FormBorderStyle = FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.ShowInTaskbar = False
        Me.StartPosition = FormStartPosition.CenterParent
        Me.Text = "Bestätigung"
        Me.Padding = New Padding(12)
        Me.AutoScaleMode = AutoScaleMode.Font
        Me.AutoSize = True
        Me.AutoSizeMode = AutoSizeMode.GrowAndShrink

        ' Icon/Fragezeichen
        _pic = New PictureBox() With {
            .SizeMode = PictureBoxSizeMode.CenterImage,
            .Size = New Size(40, 40),
            .Image = SystemIcons.Question.ToBitmap()
        }

        ' Text
        _lbl = New Label() With {
            .AutoSize = True,
            .MaximumSize = New Size(520, 0), ' Zeilenumbruch ab ~520px
            .Text = vbCrLf & vbCrLf & "INI mit Defaultwerten neu aufbauen?"
        }

        ' Buttons
        _btnAlle = New Button() With {.Text = "Alle INI", .AutoSize = True}
        _btnAktive = New Button() With {.Text = "Aktive INI", .AutoSize = True}
        _btnAktiveAppExit = New Button() With {.Text = "Alle INI + Ende", .AutoSize = True}
        _btnCancel = New Button() With {.Text = "Abbrechen", .AutoSize = True}

        ' DialogResult-Mapping (Yes/No/OK/Cancel optional, aber wir geben eigenes Enum zurück)
        AddHandler _btnAlle.Click, Sub()
                                       _result = IniResetChoice.AlleIni
                                       Me.DialogResult = DialogResult.Yes
                                       Me.Close()
                                   End Sub
        AddHandler _btnAktiveAppExit.Click, Sub()
                                                _result = IniResetChoice.AktiveIniRestart
                                                Me.DialogResult = DialogResult.OK
                                                Me.Close()
                                            End Sub
        AddHandler _btnAktive.Click, Sub()
                                         _result = IniResetChoice.AktiveIni
                                         Me.DialogResult = DialogResult.No
                                         Me.Close()
                                     End Sub
        AddHandler _btnCancel.Click, Sub()
                                         _result = IniResetChoice.Abbrechen
                                         Me.DialogResult = DialogResult.Cancel
                                         Me.Close()
                                     End Sub

        ' Accept/Cancel (Enter/ESC)
        Me.AcceptButton = _btnAktive   ' sinnvolle Default-Wahl
        Me.CancelButton = _btnCancel

        ' Layout: oben Icon + Text, unten Buttons rechts
        Dim tlp As New TableLayoutPanel() With {
            .ColumnCount = 2,
            .RowCount = 2,
            .Dock = DockStyle.Fill,
            .AutoSize = True
        }
        tlp.ColumnStyles.Add(New ColumnStyle(SizeType.AutoSize))
        tlp.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0!))
        tlp.RowStyles.Add(New RowStyle(SizeType.AutoSize))
        tlp.RowStyles.Add(New RowStyle(SizeType.AutoSize))

        ' Zeile 0: Icon + Text
        tlp.Controls.Add(_pic, 0, 0)
        tlp.Controls.Add(_lbl, 1, 0)
        tlp.SetCellPosition(_pic, New TableLayoutPanelCellPosition(0, 0))
        tlp.SetCellPosition(_lbl, New TableLayoutPanelCellPosition(1, 0))
        _pic.Margin = New Padding(0, 2, 12, 0)
        _lbl.Margin = New Padding(0, 0, 0, 0)

        ' Button-Panel
        Dim pnlButtons As New FlowLayoutPanel() With {
            .FlowDirection = FlowDirection.LeftToRight,
            .Dock = DockStyle.Fill,
            .AutoSize = True,
            .Padding = New Padding(0),
            .Margin = New Padding(0, 10, 0, 0)
        }
        pnlButtons.Controls.AddRange(New Control() {_btnAktive, _btnAktiveAppExit, _btnAlle, _btnCancel})

        ' Zeile 1: (leer für Icon-Spalte) + Buttons
        tlp.Controls.Add(New Panel() With {.Width = 1, .Height = 1}, 0, 1)
        tlp.Controls.Add(pnlButtons, 1, 1)

        Me.Controls.Add(tlp)

        ' Mindestbreite für harmonische Optik
        Me.MinimumSize = New Size(380, Me.MinimumSize.Height)
    End Sub
    '
    'Geänderter Name wegen Namenskollision zwischen ShowDialog-(Shared) und Form.ShowDialog (Instanz).
    Public Shared Function ShowTheDialog(owner As IWin32Window) As IniResetChoice
        Using f As New FrmIniResetConfirm()
            f.Icon = Nothing ' wie MessageBox: kein Fenster-Icon nötig
            If owner Is Nothing Then
                f.ShowDialog()
            Else
                f.ShowDialog(owner)
            End If
            Return f._result
        End Using
    End Function

    Public Shared Function ShowTheDialog() As IniResetChoice
        Return ShowTheDialog(TryCast(Form.ActiveForm, IWin32Window))
    End Function

End Class
