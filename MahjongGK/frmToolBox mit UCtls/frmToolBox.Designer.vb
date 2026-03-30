<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmToolBox
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmToolBox))
        Me.TabControlToolBox = New System.Windows.Forms.TabControl()
        Me.TabPageSpieler = New System.Windows.Forms.TabPage()
        Me.KompassRoseX1 = New MahjongGK.KompassRoseX()
        Me.KompassRose1 = New MahjongGK.KompassRose()
        Me.lblToolboxSpieler = New System.Windows.Forms.Label()
        Me.TabPageEditor = New System.Windows.Forms.TabPage()
        Me.lblToolboxEditor = New System.Windows.Forms.Label()
        Me.MenuStrip_Editor = New System.Windows.Forms.MenuStrip()
        Me.tsmiEditor_Datei = New System.Windows.Forms.ToolStripMenuItem()
        Me.TabPageHintergrund = New System.Windows.Forms.TabPage()
        Me.UctlToolboxHintergrund1 = New MahjongGK.UctlToolboxHintergrund()
        Me.ImageListToolBox = New System.Windows.Forms.ImageList(Me.components)
        Me.ToolTipToolBox = New System.Windows.Forms.ToolTip(Me.components)
        Me.TabControlToolBox.SuspendLayout()
        Me.TabPageSpieler.SuspendLayout()
        Me.TabPageEditor.SuspendLayout()
        Me.MenuStrip_Editor.SuspendLayout()
        Me.TabPageHintergrund.SuspendLayout()
        Me.SuspendLayout()
        '
        'TabControlToolBox
        '
        Me.TabControlToolBox.Controls.Add(Me.TabPageSpieler)
        Me.TabControlToolBox.Controls.Add(Me.TabPageEditor)
        Me.TabControlToolBox.Controls.Add(Me.TabPageHintergrund)
        Me.TabControlToolBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TabControlToolBox.ImageList = Me.ImageListToolBox
        Me.TabControlToolBox.ItemSize = New System.Drawing.Size(75, 30)
        Me.TabControlToolBox.Location = New System.Drawing.Point(0, 0)
        Me.TabControlToolBox.Margin = New System.Windows.Forms.Padding(4)
        Me.TabControlToolBox.Name = "TabControlToolBox"
        Me.TabControlToolBox.SelectedIndex = 0
        Me.TabControlToolBox.Size = New System.Drawing.Size(585, 666)
        Me.TabControlToolBox.TabIndex = 2
        '
        'TabPageSpieler
        '
        Me.TabPageSpieler.Controls.Add(Me.KompassRoseX1)
        Me.TabPageSpieler.Controls.Add(Me.KompassRose1)
        Me.TabPageSpieler.Controls.Add(Me.lblToolboxSpieler)
        Me.TabPageSpieler.ImageKey = "(Keine)"
        Me.TabPageSpieler.Location = New System.Drawing.Point(4, 34)
        Me.TabPageSpieler.Margin = New System.Windows.Forms.Padding(4)
        Me.TabPageSpieler.Name = "TabPageSpieler"
        Me.TabPageSpieler.Size = New System.Drawing.Size(577, 628)
        Me.TabPageSpieler.TabIndex = 3
        Me.TabPageSpieler.Text = "Spieler"
        Me.TabPageSpieler.UseVisualStyleBackColor = True
        '
        'KompassRoseX1
        '
        Me.KompassRoseX1.Direction = MahjongGK.KompassRoseX.KompassXEnum.None
        Me.KompassRoseX1.Location = New System.Drawing.Point(76, 142)
        Me.KompassRoseX1.Margin = New System.Windows.Forms.Padding(4)
        Me.KompassRoseX1.MinimumSize = New System.Drawing.Size(107, 98)
        Me.KompassRoseX1.Name = "KompassRoseX1"
        Me.KompassRoseX1.Size = New System.Drawing.Size(173, 160)
        Me.KompassRoseX1.TabIndex = 4
        '
        'KompassRose1
        '
        Me.KompassRose1.BackColor = System.Drawing.Color.Transparent
        Me.KompassRose1.Direction = MahjongGK.KompassEnum.None
        Me.KompassRose1.Location = New System.Drawing.Point(131, 389)
        Me.KompassRose1.Margin = New System.Windows.Forms.Padding(4)
        Me.KompassRose1.Name = "KompassRose1"
        Me.KompassRose1.Size = New System.Drawing.Size(48, 48)
        Me.KompassRose1.TabIndex = 3
        '
        'lblToolboxSpieler
        '
        Me.lblToolboxSpieler.Cursor = System.Windows.Forms.Cursors.Default
        Me.lblToolboxSpieler.Image = CType(resources.GetObject("lblToolboxSpieler.Image"), System.Drawing.Image)
        Me.lblToolboxSpieler.Location = New System.Drawing.Point(27, 58)
        Me.lblToolboxSpieler.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblToolboxSpieler.Name = "lblToolboxSpieler"
        Me.lblToolboxSpieler.Size = New System.Drawing.Size(43, 39)
        Me.lblToolboxSpieler.TabIndex = 2
        '
        'TabPageEditor
        '
        Me.TabPageEditor.Controls.Add(Me.lblToolboxEditor)
        Me.TabPageEditor.Controls.Add(Me.MenuStrip_Editor)
        Me.TabPageEditor.ImageKey = "(Keine)"
        Me.TabPageEditor.Location = New System.Drawing.Point(4, 34)
        Me.TabPageEditor.Margin = New System.Windows.Forms.Padding(4)
        Me.TabPageEditor.Name = "TabPageEditor"
        Me.TabPageEditor.Padding = New System.Windows.Forms.Padding(4)
        Me.TabPageEditor.Size = New System.Drawing.Size(577, 628)
        Me.TabPageEditor.TabIndex = 0
        Me.TabPageEditor.Text = "Editor"
        Me.TabPageEditor.UseVisualStyleBackColor = True
        '
        'lblToolboxEditor
        '
        Me.lblToolboxEditor.Cursor = System.Windows.Forms.Cursors.Default
        Me.lblToolboxEditor.Image = CType(resources.GetObject("lblToolboxEditor.Image"), System.Drawing.Image)
        Me.lblToolboxEditor.Location = New System.Drawing.Point(27, 55)
        Me.lblToolboxEditor.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblToolboxEditor.Name = "lblToolboxEditor"
        Me.lblToolboxEditor.Size = New System.Drawing.Size(43, 39)
        Me.lblToolboxEditor.TabIndex = 1
        '
        'MenuStrip_Editor
        '
        Me.MenuStrip_Editor.ImageScalingSize = New System.Drawing.Size(20, 20)
        Me.MenuStrip_Editor.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsmiEditor_Datei})
        Me.MenuStrip_Editor.Location = New System.Drawing.Point(4, 4)
        Me.MenuStrip_Editor.Name = "MenuStrip_Editor"
        Me.MenuStrip_Editor.Size = New System.Drawing.Size(569, 28)
        Me.MenuStrip_Editor.TabIndex = 0
        Me.MenuStrip_Editor.Text = "MenuStrip1"
        '
        'tsmiEditor_Datei
        '
        Me.tsmiEditor_Datei.Name = "tsmiEditor_Datei"
        Me.tsmiEditor_Datei.Size = New System.Drawing.Size(59, 24)
        Me.tsmiEditor_Datei.Text = "Datei"
        '
        'TabPageHintergrund
        '
        Me.TabPageHintergrund.Controls.Add(Me.UctlToolboxHintergrund1)
        Me.TabPageHintergrund.Location = New System.Drawing.Point(4, 34)
        Me.TabPageHintergrund.Margin = New System.Windows.Forms.Padding(4)
        Me.TabPageHintergrund.Name = "TabPageHintergrund"
        Me.TabPageHintergrund.Size = New System.Drawing.Size(577, 628)
        Me.TabPageHintergrund.TabIndex = 4
        Me.TabPageHintergrund.Text = "Hintergrund"
        Me.TabPageHintergrund.UseVisualStyleBackColor = True
        '
        'UctlToolboxHintergrund1
        '
        Me.UctlToolboxHintergrund1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.UctlToolboxHintergrund1.Location = New System.Drawing.Point(0, 0)
        Me.UctlToolboxHintergrund1.Margin = New System.Windows.Forms.Padding(5)
        Me.UctlToolboxHintergrund1.Name = "UctlToolboxHintergrund1"
        Me.UctlToolboxHintergrund1.Padding = New System.Windows.Forms.Padding(0, 6, 0, 0)
        Me.UctlToolboxHintergrund1.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.UctlToolboxHintergrund1.Size = New System.Drawing.Size(577, 628)
        Me.UctlToolboxHintergrund1.TabIndex = 0
        '
        'ImageListToolBox
        '
        Me.ImageListToolBox.ImageStream = CType(resources.GetObject("ImageListToolBox.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.ImageListToolBox.TransparentColor = System.Drawing.Color.Transparent
        Me.ImageListToolBox.Images.SetKeyName(0, "Spieler.png")
        Me.ImageListToolBox.Images.SetKeyName(1, "SpielerAktiv.png")
        Me.ImageListToolBox.Images.SetKeyName(2, "Editor.png")
        Me.ImageListToolBox.Images.SetKeyName(3, "EditorAktiv.png")
        Me.ImageListToolBox.Images.SetKeyName(4, "Werkbank.png")
        Me.ImageListToolBox.Images.SetKeyName(5, "WerkbankAktiv.png")
        Me.ImageListToolBox.Images.SetKeyName(6, "MoveLocation16q.png")
        Me.ImageListToolBox.Images.SetKeyName(7, "MoveLocation16qGrün.png")
        '
        'ToolTipToolBox
        '
        Me.ToolTipToolBox.BackColor = System.Drawing.Color.Chartreuse
        Me.ToolTipToolBox.IsBalloon = True
        Me.ToolTipToolBox.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info
        '
        'frmToolBox
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(120.0!, 120.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.ClientSize = New System.Drawing.Size(585, 666)
        Me.Controls.Add(Me.TabControlToolBox)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.KeyPreview = True
        Me.Margin = New System.Windows.Forms.Padding(4)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmToolBox"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.Manual
        Me.Text = "Werkzeugkiste - Editor/Werkbank"
        Me.TopMost = True
        Me.TabControlToolBox.ResumeLayout(False)
        Me.TabPageSpieler.ResumeLayout(False)
        Me.TabPageEditor.ResumeLayout(False)
        Me.TabPageEditor.PerformLayout()
        Me.MenuStrip_Editor.ResumeLayout(False)
        Me.MenuStrip_Editor.PerformLayout()
        Me.TabPageHintergrund.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents TabControlToolBox As TabControl
    Friend WithEvents TabPageEditor As TabPage
    Friend WithEvents MenuStrip_Editor As MenuStrip
    Friend WithEvents tsmiEditor_Datei As ToolStripMenuItem
    Friend WithEvents ImageListToolBox As ImageList
    Friend WithEvents lblToolboxEditor As Label
    Friend WithEvents ToolTipToolBox As ToolTip
    Friend WithEvents TabPageHintergrund As TabPage
    Friend WithEvents UctlToolboxHintergrund1 As UctlToolboxHintergrund
    Friend WithEvents TabPageSpieler As TabPage
    Friend WithEvents KompassRoseX1 As KompassRoseX
    Friend WithEvents KompassRose1 As KompassRose
    Friend WithEvents lblToolboxSpieler As Label
End Class
