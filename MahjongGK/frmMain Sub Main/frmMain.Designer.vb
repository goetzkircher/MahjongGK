<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmMain
    Inherits System.Windows.Forms.Form

    'Das Formular überschreibt den Löschvorgang, um die Komponentenliste zu bereinigen.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Wird vom Windows Form-Designer benötigt.
    Private components As System.ComponentModel.IContainer

    'Hinweis: Die folgende Prozedur ist für den Windows Form-Designer erforderlich.
    'Das Bearbeiten ist mit dem Windows Form-Designer möglich.  
    'Das Bearbeiten mit dem Code-Editor ist nicht möglich.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMain))
        Me.PanelFrmMainUGrd = New System.Windows.Forms.Panel()
        Me.TabControlMain = New System.Windows.Forms.TabControl()
        Me.TabPageSpielfeld = New System.Windows.Forms.TabPage()
        Me.UCtlSpielfeldMain = New MahjongGK.UCtlSpielfeld()
        Me.TabPageEinstellungen = New System.Windows.Forms.TabPage()
        Me.UCtlEinstellungenMain = New MahjongGK.UctlEinstellungen()
        Me.TabPageEditor = New System.Windows.Forms.TabPage()
        Me.UCtlEditorMain = New MahjongGK.UCtlEditor()
        Me.TabPageWerkbank = New System.Windows.Forms.TabPage()
        Me.UCtlWerkbankMain = New MahjongGK.UCtlWerkbank()
        Me.ToolStripExMain = New MahjongGK.ToolStripEx()
        Me.ToolStripLabel1 = New System.Windows.Forms.ToolStripLabel()
        Me.ToolTipMain = New System.Windows.Forms.ToolTip(Me.components)
        Me.MenuStripExMain = New MahjongGK.MenuStripEx()
        Me.MenueStripMainToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.PanelFrmMainUGrd.SuspendLayout()
        Me.TabControlMain.SuspendLayout()
        Me.TabPageSpielfeld.SuspendLayout()
        Me.TabPageEinstellungen.SuspendLayout()
        Me.TabPageEditor.SuspendLayout()
        Me.TabPageWerkbank.SuspendLayout()
        Me.ToolStripExMain.SuspendLayout()
        Me.MenuStripExMain.SuspendLayout()
        Me.SuspendLayout()
        '
        'PanelFrmMainUGrd
        '
        Me.PanelFrmMainUGrd.Controls.Add(Me.TabControlMain)
        Me.PanelFrmMainUGrd.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PanelFrmMainUGrd.Location = New System.Drawing.Point(0, 0)
        Me.PanelFrmMainUGrd.Name = "PanelFrmMainUGrd"
        Me.PanelFrmMainUGrd.Size = New System.Drawing.Size(1084, 491)
        Me.PanelFrmMainUGrd.TabIndex = 5
        '
        'TabControlMain
        '
        Me.TabControlMain.Controls.Add(Me.TabPageSpielfeld)
        Me.TabControlMain.Controls.Add(Me.TabPageEinstellungen)
        Me.TabControlMain.Controls.Add(Me.TabPageEditor)
        Me.TabControlMain.Controls.Add(Me.TabPageWerkbank)
        Me.TabControlMain.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TabControlMain.Location = New System.Drawing.Point(0, 0)
        Me.TabControlMain.Name = "TabControlMain"
        Me.TabControlMain.SelectedIndex = 0
        Me.TabControlMain.Size = New System.Drawing.Size(1084, 491)
        Me.TabControlMain.TabIndex = 2
        '
        'TabPageSpielfeld
        '
        Me.TabPageSpielfeld.Controls.Add(Me.UCtlSpielfeldMain)
        Me.TabPageSpielfeld.Location = New System.Drawing.Point(4, 22)
        Me.TabPageSpielfeld.Name = "TabPageSpielfeld"
        Me.TabPageSpielfeld.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPageSpielfeld.Size = New System.Drawing.Size(1076, 465)
        Me.TabPageSpielfeld.TabIndex = 0
        Me.TabPageSpielfeld.Text = "Spielfeld"
        Me.TabPageSpielfeld.UseVisualStyleBackColor = True
        '
        'UCtlSpielfeldMain
        '
        Me.UCtlSpielfeldMain.Dock = System.Windows.Forms.DockStyle.Fill
        Me.UCtlSpielfeldMain.Location = New System.Drawing.Point(3, 3)
        Me.UCtlSpielfeldMain.Name = "UCtlSpielfeldMain"
        Me.UCtlSpielfeldMain.Size = New System.Drawing.Size(1070, 459)
        Me.UCtlSpielfeldMain.TabIndex = 0
        '
        'TabPageEinstellungen
        '
        Me.TabPageEinstellungen.Controls.Add(Me.UCtlEinstellungenMain)
        Me.TabPageEinstellungen.Location = New System.Drawing.Point(4, 22)
        Me.TabPageEinstellungen.Name = "TabPageEinstellungen"
        Me.TabPageEinstellungen.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPageEinstellungen.Size = New System.Drawing.Size(777, 271)
        Me.TabPageEinstellungen.TabIndex = 1
        Me.TabPageEinstellungen.Text = "Einstellungen"
        Me.TabPageEinstellungen.UseVisualStyleBackColor = True
        '
        'UCtlEinstellungenMain
        '
        Me.UCtlEinstellungenMain.Dock = System.Windows.Forms.DockStyle.Fill
        Me.UCtlEinstellungenMain.Location = New System.Drawing.Point(3, 3)
        Me.UCtlEinstellungenMain.Name = "UCtlEinstellungenMain"
        Me.UCtlEinstellungenMain.Size = New System.Drawing.Size(771, 265)
        Me.UCtlEinstellungenMain.TabIndex = 0
        '
        'TabPageEditor
        '
        Me.TabPageEditor.Controls.Add(Me.UCtlEditorMain)
        Me.TabPageEditor.Location = New System.Drawing.Point(4, 22)
        Me.TabPageEditor.Name = "TabPageEditor"
        Me.TabPageEditor.Size = New System.Drawing.Size(777, 271)
        Me.TabPageEditor.TabIndex = 2
        Me.TabPageEditor.Text = "Editor"
        Me.TabPageEditor.UseVisualStyleBackColor = True
        '
        'UCtlEditorMain
        '
        Me.UCtlEditorMain.Dock = System.Windows.Forms.DockStyle.Fill
        Me.UCtlEditorMain.Location = New System.Drawing.Point(0, 0)
        Me.UCtlEditorMain.Name = "UCtlEditorMain"
        Me.UCtlEditorMain.Size = New System.Drawing.Size(777, 271)
        Me.UCtlEditorMain.TabIndex = 0
        '
        'TabPageWerkbank
        '
        Me.TabPageWerkbank.Controls.Add(Me.UCtlWerkbankMain)
        Me.TabPageWerkbank.Location = New System.Drawing.Point(4, 22)
        Me.TabPageWerkbank.Name = "TabPageWerkbank"
        Me.TabPageWerkbank.Size = New System.Drawing.Size(777, 271)
        Me.TabPageWerkbank.TabIndex = 3
        Me.TabPageWerkbank.Text = "Werkbank"
        Me.TabPageWerkbank.UseVisualStyleBackColor = True
        '
        'UCtlWerkbankMain
        '
        Me.UCtlWerkbankMain.Dock = System.Windows.Forms.DockStyle.Fill
        Me.UCtlWerkbankMain.Location = New System.Drawing.Point(0, 0)
        Me.UCtlWerkbankMain.Name = "UCtlWerkbankMain"
        Me.UCtlWerkbankMain.Size = New System.Drawing.Size(777, 271)
        Me.UCtlWerkbankMain.TabIndex = 0
        '
        'ToolStripExMain
        '
        Me.ToolStripExMain.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.ToolStripExMain.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripLabel1})
        Me.ToolStripExMain.Location = New System.Drawing.Point(0, 466)
        Me.ToolStripExMain.Name = "ToolStripExMain"
        Me.ToolStripExMain.Size = New System.Drawing.Size(1084, 25)
        Me.ToolStripExMain.TabIndex = 6
        Me.ToolStripExMain.Text = "ToolStripEx1"
        '
        'ToolStripLabel1
        '
        Me.ToolStripLabel1.Name = "ToolStripLabel1"
        Me.ToolStripLabel1.Size = New System.Drawing.Size(102, 22)
        Me.ToolStripLabel1.Text = "ToolStripExMain"
        '
        'MenuStripExMain
        '
        Me.MenuStripExMain.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MenueStripMainToolStripMenuItem})
        Me.MenuStripExMain.Location = New System.Drawing.Point(0, 0)
        Me.MenuStripExMain.Name = "MenuStripExMain"
        Me.MenuStripExMain.Size = New System.Drawing.Size(1084, 25)
        Me.MenuStripExMain.TabIndex = 7
        Me.MenuStripExMain.Text = "MenuStripEx1"
        '
        'MenueStripMainToolStripMenuItem
        '
        Me.MenueStripMainToolStripMenuItem.Name = "MenueStripMainToolStripMenuItem"
        Me.MenueStripMainToolStripMenuItem.Size = New System.Drawing.Size(129, 21)
        Me.MenueStripMainToolStripMenuItem.Text = "MenueStripExMain"
        '
        'frmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1084, 491)
        Me.Controls.Add(Me.ToolStripExMain)
        Me.Controls.Add(Me.MenuStripExMain)
        Me.Controls.Add(Me.PanelFrmMainUGrd)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MainMenuStrip = Me.MenuStripExMain
        Me.Name = "frmMain"
        Me.Text = "MahjongGK - by Götz Kircher - Version: "
        Me.PanelFrmMainUGrd.ResumeLayout(False)
        Me.TabControlMain.ResumeLayout(False)
        Me.TabPageSpielfeld.ResumeLayout(False)
        Me.TabPageEinstellungen.ResumeLayout(False)
        Me.TabPageEditor.ResumeLayout(False)
        Me.TabPageWerkbank.ResumeLayout(False)
        Me.ToolStripExMain.ResumeLayout(False)
        Me.ToolStripExMain.PerformLayout()
        Me.MenuStripExMain.ResumeLayout(False)
        Me.MenuStripExMain.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents PanelFrmMainUGrd As Panel
    Friend WithEvents TabControlMain As TabControl
    Friend WithEvents TabPageSpielfeld As TabPage
    Friend WithEvents UCtlSpielfeldMain As UCtlSpielfeld
    Friend WithEvents TabPageEinstellungen As TabPage
    Friend WithEvents UCtlEinstellungenMain As UctlEinstellungen
    Friend WithEvents TabPageEditor As TabPage
    Friend WithEvents UCtlEditorMain As UCtlEditor
    Friend WithEvents TabPageWerkbank As TabPage
    Friend WithEvents UCtlWerkbankMain As UCtlWerkbank
    Friend WithEvents ToolStripExMain As ToolStripEx
    Friend WithEvents ToolTipMain As ToolTip
    Friend WithEvents ToolStripLabel1 As ToolStripLabel
    Friend WithEvents MenuStripExMain As MenuStripEx
    Friend WithEvents MenueStripMainToolStripMenuItem As ToolStripMenuItem
End Class
