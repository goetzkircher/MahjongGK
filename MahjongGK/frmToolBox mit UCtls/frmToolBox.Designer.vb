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
        Me.TabPagePositionierer = New System.Windows.Forms.TabPage()
        Me.Positionierer1 = New MahjongGK.Positionierer()
        Me.positioniererOffsetX = New MahjongGK.Num2UpDown()
        Me.positioniererOffsetY = New MahjongGK.Num2UpDown()
        Me.positioniererBtnReset = New System.Windows.Forms.Button()
        Me.positioniererBtnUndoStep = New System.Windows.Forms.Button()
        Me.positioniererBtnReDoStep = New System.Windows.Forms.Button()
        Me.positioniererBtnUnDoEinbau = New System.Windows.Forms.Button()
        Me.positioniererBtnAbbrechen = New System.Windows.Forms.Button()
        Me.positioniererBtnÜbernehmen = New System.Windows.Forms.Button()
        Me.positioniererBtnEinbau = New System.Windows.Forms.Button()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.lblToolboxPositionierer = New System.Windows.Forms.Label()
        Me.PositioniererReferencePoint = New MahjongGK.ReferencePoint()
        Me.TabPageWerkbank = New System.Windows.Forms.TabPage()
        Me.UCtlWerkbank1 = New MahjongGK.UCtlWerkbank()
        Me.lblInfi1 = New System.Windows.Forms.Label()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.txtNameBasisformForSaving = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.txtNameBasisform = New System.Windows.Forms.TextBox()
        Me.lblToolboxWerkbank = New System.Windows.Forms.Label()
        Me.MenuStrip_Werkbk = New System.Windows.Forms.MenuStrip()
        Me.tsmiWerkbk_Datei = New System.Windows.Forms.ToolStripMenuItem()
        Me.TabPageHintergrund = New System.Windows.Forms.TabPage()
        Me.UctlToolboxHintergrund1 = New MahjongGK.UctlToolboxHintergrund()
        Me.ImageListToolBox = New System.Windows.Forms.ImageList(Me.components)
        Me.ToolTipToolBox = New System.Windows.Forms.ToolTip(Me.components)
        Me.TabControlToolBox.SuspendLayout()
        Me.TabPageSpieler.SuspendLayout()
        Me.TabPageEditor.SuspendLayout()
        Me.MenuStrip_Editor.SuspendLayout()
        Me.TabPagePositionierer.SuspendLayout()
        Me.TabPageWerkbank.SuspendLayout()
        Me.MenuStrip_Werkbk.SuspendLayout()
        Me.TabPageHintergrund.SuspendLayout()
        Me.SuspendLayout()
        '
        'TabControlToolBox
        '
        Me.TabControlToolBox.Controls.Add(Me.TabPageSpieler)
        Me.TabControlToolBox.Controls.Add(Me.TabPageEditor)
        Me.TabControlToolBox.Controls.Add(Me.TabPagePositionierer)
        Me.TabControlToolBox.Controls.Add(Me.TabPageWerkbank)
        Me.TabControlToolBox.Controls.Add(Me.TabPageHintergrund)
        Me.TabControlToolBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TabControlToolBox.ImageList = Me.ImageListToolBox
        Me.TabControlToolBox.ItemSize = New System.Drawing.Size(75, 30)
        Me.TabControlToolBox.Location = New System.Drawing.Point(0, 0)
        Me.TabControlToolBox.Name = "TabControlToolBox"
        Me.TabControlToolBox.SelectedIndex = 0
        Me.TabControlToolBox.Size = New System.Drawing.Size(384, 541)
        Me.TabControlToolBox.SizeMode = System.Windows.Forms.TabSizeMode.Fixed
        Me.TabControlToolBox.TabIndex = 2
        '
        'TabPageSpieler
        '
        Me.TabPageSpieler.Controls.Add(Me.KompassRoseX1)
        Me.TabPageSpieler.Controls.Add(Me.KompassRose1)
        Me.TabPageSpieler.Controls.Add(Me.lblToolboxSpieler)
        Me.TabPageSpieler.ImageKey = "(Keine)"
        Me.TabPageSpieler.Location = New System.Drawing.Point(4, 34)
        Me.TabPageSpieler.Name = "TabPageSpieler"
        Me.TabPageSpieler.Size = New System.Drawing.Size(375, 515)
        Me.TabPageSpieler.TabIndex = 3
        Me.TabPageSpieler.Text = "Spieler"
        Me.TabPageSpieler.UseVisualStyleBackColor = True
        '
        'KompassRoseX1
        '
        Me.KompassRoseX1.Direction = MahjongGK.KompassRoseX.KompassXEnum.None
        Me.KompassRoseX1.Location = New System.Drawing.Point(57, 115)
        Me.KompassRoseX1.MinimumSize = New System.Drawing.Size(80, 80)
        Me.KompassRoseX1.Name = "KompassRoseX1"
        Me.KompassRoseX1.Size = New System.Drawing.Size(130, 130)
        Me.KompassRoseX1.TabIndex = 4
        '
        'KompassRose1
        '
        Me.KompassRose1.BackColor = System.Drawing.Color.Transparent
        Me.KompassRose1.Direction = MahjongGK.KompassEnum.None
        Me.KompassRose1.Location = New System.Drawing.Point(98, 316)
        Me.KompassRose1.Name = "KompassRose1"
        Me.KompassRose1.Size = New System.Drawing.Size(48, 48)
        Me.KompassRose1.TabIndex = 3
        '
        'lblToolboxSpieler
        '
        Me.lblToolboxSpieler.Cursor = System.Windows.Forms.Cursors.Default
        Me.lblToolboxSpieler.Image = CType(resources.GetObject("lblToolboxSpieler.Image"), System.Drawing.Image)
        Me.lblToolboxSpieler.Location = New System.Drawing.Point(20, 47)
        Me.lblToolboxSpieler.Name = "lblToolboxSpieler"
        Me.lblToolboxSpieler.Size = New System.Drawing.Size(32, 32)
        Me.lblToolboxSpieler.TabIndex = 2
        '
        'TabPageEditor
        '
        Me.TabPageEditor.Controls.Add(Me.lblToolboxEditor)
        Me.TabPageEditor.Controls.Add(Me.MenuStrip_Editor)
        Me.TabPageEditor.ImageKey = "(Keine)"
        Me.TabPageEditor.Location = New System.Drawing.Point(4, 34)
        Me.TabPageEditor.Name = "TabPageEditor"
        Me.TabPageEditor.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPageEditor.Size = New System.Drawing.Size(375, 515)
        Me.TabPageEditor.TabIndex = 0
        Me.TabPageEditor.Text = "Editor"
        Me.TabPageEditor.UseVisualStyleBackColor = True
        '
        'lblToolboxEditor
        '
        Me.lblToolboxEditor.Cursor = System.Windows.Forms.Cursors.Default
        Me.lblToolboxEditor.Image = CType(resources.GetObject("lblToolboxEditor.Image"), System.Drawing.Image)
        Me.lblToolboxEditor.Location = New System.Drawing.Point(20, 45)
        Me.lblToolboxEditor.Name = "lblToolboxEditor"
        Me.lblToolboxEditor.Size = New System.Drawing.Size(32, 32)
        Me.lblToolboxEditor.TabIndex = 1
        '
        'MenuStrip_Editor
        '
        Me.MenuStrip_Editor.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsmiEditor_Datei})
        Me.MenuStrip_Editor.Location = New System.Drawing.Point(3, 3)
        Me.MenuStrip_Editor.Name = "MenuStrip_Editor"
        Me.MenuStrip_Editor.Size = New System.Drawing.Size(369, 25)
        Me.MenuStrip_Editor.TabIndex = 0
        Me.MenuStrip_Editor.Text = "MenuStrip1"
        '
        'tsmiEditor_Datei
        '
        Me.tsmiEditor_Datei.Name = "tsmiEditor_Datei"
        Me.tsmiEditor_Datei.Size = New System.Drawing.Size(50, 21)
        Me.tsmiEditor_Datei.Text = "Datei"
        '
        'TabPagePositionierer
        '
        Me.TabPagePositionierer.Controls.Add(Me.Positionierer1)
        Me.TabPagePositionierer.Controls.Add(Me.positioniererOffsetX)
        Me.TabPagePositionierer.Controls.Add(Me.positioniererOffsetY)
        Me.TabPagePositionierer.Controls.Add(Me.positioniererBtnReset)
        Me.TabPagePositionierer.Controls.Add(Me.positioniererBtnUndoStep)
        Me.TabPagePositionierer.Controls.Add(Me.positioniererBtnReDoStep)
        Me.TabPagePositionierer.Controls.Add(Me.positioniererBtnUnDoEinbau)
        Me.TabPagePositionierer.Controls.Add(Me.positioniererBtnAbbrechen)
        Me.TabPagePositionierer.Controls.Add(Me.positioniererBtnÜbernehmen)
        Me.TabPagePositionierer.Controls.Add(Me.positioniererBtnEinbau)
        Me.TabPagePositionierer.Controls.Add(Me.Label13)
        Me.TabPagePositionierer.Controls.Add(Me.Label12)
        Me.TabPagePositionierer.Controls.Add(Me.Label11)
        Me.TabPagePositionierer.Controls.Add(Me.Label10)
        Me.TabPagePositionierer.Controls.Add(Me.lblToolboxPositionierer)
        Me.TabPagePositionierer.Controls.Add(Me.PositioniererReferencePoint)
        Me.TabPagePositionierer.Location = New System.Drawing.Point(4, 34)
        Me.TabPagePositionierer.Name = "TabPagePositionierer"
        Me.TabPagePositionierer.Size = New System.Drawing.Size(375, 515)
        Me.TabPagePositionierer.TabIndex = 2
        Me.TabPagePositionierer.Text = "Positionierer"
        Me.TabPagePositionierer.UseVisualStyleBackColor = True
        '
        'Positionierer1
        '
        Me.Positionierer1.AutoUpdateOffsets = True
        Me.Positionierer1.InitialRepeatDelayMs = 350
        Me.Positionierer1.Location = New System.Drawing.Point(81, 164)
        Me.Positionierer1.Name = "Positionierer1"
        Me.Positionierer1.OffsetX = 0
        Me.Positionierer1.OffsetY = 0
        Me.Positionierer1.RepeatIntervalMs = 80
        Me.Positionierer1.SelectedStartingPoint = MahjongGK.Positionierer.PositionEnum.Center
        Me.Positionierer1.Size = New System.Drawing.Size(112, 112)
        Me.Positionierer1.StepX = 1
        Me.Positionierer1.StepY = 1
        Me.Positionierer1.TabIndex = 23
        '
        'positioniererOffsetX
        '
        Me.positioniererOffsetX.Location = New System.Drawing.Point(249, 217)
        Me.positioniererOffsetX.MinimumSize = New System.Drawing.Size(65, 24)
        Me.positioniererOffsetX.Name = "positioniererOffsetX"
        Me.positioniererOffsetX.Size = New System.Drawing.Size(65, 28)
        Me.positioniererOffsetX.TabIndex = 22
        Me.positioniererOffsetX.UseArrowRightLeft = True
        '
        'positioniererOffsetY
        '
        Me.positioniererOffsetY.Location = New System.Drawing.Point(249, 183)
        Me.positioniererOffsetY.MinimumSize = New System.Drawing.Size(65, 24)
        Me.positioniererOffsetY.Name = "positioniererOffsetY"
        Me.positioniererOffsetY.Size = New System.Drawing.Size(65, 28)
        Me.positioniererOffsetY.TabIndex = 21
        '
        'positioniererBtnReset
        '
        Me.positioniererBtnReset.Location = New System.Drawing.Point(258, 264)
        Me.positioniererBtnReset.Name = "positioniererBtnReset"
        Me.positioniererBtnReset.Size = New System.Drawing.Size(56, 23)
        Me.positioniererBtnReset.TabIndex = 20
        Me.positioniererBtnReset.Text = "Reset"
        Me.positioniererBtnReset.UseVisualStyleBackColor = True
        '
        'positioniererBtnUndoStep
        '
        Me.positioniererBtnUndoStep.Image = CType(resources.GetObject("positioniererBtnUndoStep.Image"), System.Drawing.Image)
        Me.positioniererBtnUndoStep.Location = New System.Drawing.Point(126, 332)
        Me.positioniererBtnUndoStep.Name = "positioniererBtnUndoStep"
        Me.positioniererBtnUndoStep.Size = New System.Drawing.Size(95, 23)
        Me.positioniererBtnUndoStep.TabIndex = 15
        Me.positioniererBtnUndoStep.Text = "UnDo Step"
        Me.positioniererBtnUndoStep.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.positioniererBtnUndoStep.UseVisualStyleBackColor = True
        '
        'positioniererBtnReDoStep
        '
        Me.positioniererBtnReDoStep.Image = CType(resources.GetObject("positioniererBtnReDoStep.Image"), System.Drawing.Image)
        Me.positioniererBtnReDoStep.Location = New System.Drawing.Point(227, 332)
        Me.positioniererBtnReDoStep.Name = "positioniererBtnReDoStep"
        Me.positioniererBtnReDoStep.Size = New System.Drawing.Size(87, 23)
        Me.positioniererBtnReDoStep.TabIndex = 14
        Me.positioniererBtnReDoStep.Text = "ReDo Step"
        Me.positioniererBtnReDoStep.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.positioniererBtnReDoStep.UseVisualStyleBackColor = True
        '
        'positioniererBtnUnDoEinbau
        '
        Me.positioniererBtnUnDoEinbau.Image = CType(resources.GetObject("positioniererBtnUnDoEinbau.Image"), System.Drawing.Image)
        Me.positioniererBtnUnDoEinbau.Location = New System.Drawing.Point(22, 332)
        Me.positioniererBtnUnDoEinbau.Name = "positioniererBtnUnDoEinbau"
        Me.positioniererBtnUnDoEinbau.Size = New System.Drawing.Size(98, 23)
        Me.positioniererBtnUnDoEinbau.TabIndex = 13
        Me.positioniererBtnUnDoEinbau.Text = "UnDo Einbau"
        Me.positioniererBtnUnDoEinbau.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.positioniererBtnUnDoEinbau.UseVisualStyleBackColor = True
        '
        'positioniererBtnAbbrechen
        '
        Me.positioniererBtnAbbrechen.Image = CType(resources.GetObject("positioniererBtnAbbrechen.Image"), System.Drawing.Image)
        Me.positioniererBtnAbbrechen.Location = New System.Drawing.Point(227, 303)
        Me.positioniererBtnAbbrechen.Name = "positioniererBtnAbbrechen"
        Me.positioniererBtnAbbrechen.Size = New System.Drawing.Size(87, 23)
        Me.positioniererBtnAbbrechen.TabIndex = 12
        Me.positioniererBtnAbbrechen.Text = "Abbrechen"
        Me.positioniererBtnAbbrechen.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.positioniererBtnAbbrechen.UseVisualStyleBackColor = True
        '
        'positioniererBtnÜbernehmen
        '
        Me.positioniererBtnÜbernehmen.Image = CType(resources.GetObject("positioniererBtnÜbernehmen.Image"), System.Drawing.Image)
        Me.positioniererBtnÜbernehmen.Location = New System.Drawing.Point(126, 303)
        Me.positioniererBtnÜbernehmen.Name = "positioniererBtnÜbernehmen"
        Me.positioniererBtnÜbernehmen.Size = New System.Drawing.Size(95, 23)
        Me.positioniererBtnÜbernehmen.TabIndex = 11
        Me.positioniererBtnÜbernehmen.Text = "Übernehmen"
        Me.positioniererBtnÜbernehmen.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.positioniererBtnÜbernehmen.UseVisualStyleBackColor = True
        '
        'positioniererBtnEinbau
        '
        Me.positioniererBtnEinbau.Image = CType(resources.GetObject("positioniererBtnEinbau.Image"), System.Drawing.Image)
        Me.positioniererBtnEinbau.Location = New System.Drawing.Point(22, 303)
        Me.positioniererBtnEinbau.Name = "positioniererBtnEinbau"
        Me.positioniererBtnEinbau.Size = New System.Drawing.Size(98, 23)
        Me.positioniererBtnEinbau.TabIndex = 10
        Me.positioniererBtnEinbau.Text = "Start Einbau"
        Me.positioniererBtnEinbau.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.positioniererBtnEinbau.UseVisualStyleBackColor = True
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.Location = New System.Drawing.Point(78, 119)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(128, 13)
        Me.Label13.TabIndex = 9
        Me.Label13.Text = "Bezugspunkt im Spielfeld:"
        '
        'Label12
        '
        Me.Label12.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.Label12.AutoSize = True
        Me.Label12.Location = New System.Drawing.Point(19, 456)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(305, 39)
        Me.Label12.TabIndex = 8
        Me.Label12.Text = "Der Bezugspunkt im Baustein ist eine der Ecken oder die Mitte." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Er wird in Deckun" &
    "g gebracht mit dem Bezugspunkt im Spielfeld." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Letzterer läßt sich mit den Pfeile" &
    "n schrittweise verschieben."
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Location = New System.Drawing.Point(78, 40)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(129, 13)
        Me.Label11.TabIndex = 7
        Me.Label11.Text = "Bezugspunkt im Baustein:"
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Location = New System.Drawing.Point(19, 15)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(283, 13)
        Me.Label10.TabIndex = 4
        Me.Label10.Text = "Positioniert den aktuellen Baustein der Werkbank im Editor"
        '
        'lblToolboxPositionierer
        '
        Me.lblToolboxPositionierer.Image = CType(resources.GetObject("lblToolboxPositionierer.Image"), System.Drawing.Image)
        Me.lblToolboxPositionierer.Location = New System.Drawing.Point(20, 45)
        Me.lblToolboxPositionierer.Name = "lblToolboxPositionierer"
        Me.lblToolboxPositionierer.Size = New System.Drawing.Size(32, 32)
        Me.lblToolboxPositionierer.TabIndex = 3
        '
        'PositioniererReferencePoint
        '
        Me.PositioniererReferencePoint.Location = New System.Drawing.Point(119, 59)
        Me.PositioniererReferencePoint.Name = "PositioniererReferencePoint"
        Me.PositioniererReferencePoint.SelectedReferencePoint = MahjongGK.ReferencePoint.RefPoint.None
        Me.PositioniererReferencePoint.Size = New System.Drawing.Size(48, 48)
        Me.PositioniererReferencePoint.TabIndex = 6
        '
        'TabPageWerkbank
        '
        Me.TabPageWerkbank.Controls.Add(Me.UCtlWerkbank1)
        Me.TabPageWerkbank.Controls.Add(Me.lblInfi1)
        Me.TabPageWerkbank.Controls.Add(Me.Label8)
        Me.TabPageWerkbank.Controls.Add(Me.Label4)
        Me.TabPageWerkbank.Controls.Add(Me.txtNameBasisformForSaving)
        Me.TabPageWerkbank.Controls.Add(Me.Label3)
        Me.TabPageWerkbank.Controls.Add(Me.txtNameBasisform)
        Me.TabPageWerkbank.Controls.Add(Me.lblToolboxWerkbank)
        Me.TabPageWerkbank.Controls.Add(Me.MenuStrip_Werkbk)
        Me.TabPageWerkbank.Location = New System.Drawing.Point(4, 34)
        Me.TabPageWerkbank.Name = "TabPageWerkbank"
        Me.TabPageWerkbank.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPageWerkbank.Size = New System.Drawing.Size(376, 503)
        Me.TabPageWerkbank.TabIndex = 1
        Me.TabPageWerkbank.Text = "Werkbank"
        Me.TabPageWerkbank.UseVisualStyleBackColor = True
        '
        'UCtlWerkbank1
        '
        Me.UCtlWerkbank1.Location = New System.Drawing.Point(44, 94)
        Me.UCtlWerkbank1.Name = "UCtlWerkbank1"
        Me.UCtlWerkbank1.Size = New System.Drawing.Size(280, 390)
        Me.UCtlWerkbank1.TabIndex = 15
        '
        'lblInfi1
        '
        Me.lblInfi1.Image = Global.MahjongGK.My.Resources.Resources.Info16qBlau
        Me.lblInfi1.Location = New System.Drawing.Point(27, 480)
        Me.lblInfi1.Name = "lblInfi1"
        Me.lblInfi1.Size = New System.Drawing.Size(16, 17)
        Me.lblInfi1.TabIndex = 14
        Me.ToolTipToolBox.SetToolTip(Me.lblInfi1, resources.GetString("lblInfi1.ToolTip"))
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(46, 484)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(319, 13)
        Me.Label8.TabIndex = 13
        Me.Label8.Text = "Die Zusammenstellung der Symbole ist nicht zum Spielen gedacht."
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(154, 46)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(111, 13)
        Me.Label4.TabIndex = 8
        Me.Label4.Text = "Name zum Speichern:"
        '
        'txtNameBasisformForSaving
        '
        Me.txtNameBasisformForSaving.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtNameBasisformForSaving.Location = New System.Drawing.Point(157, 62)
        Me.txtNameBasisformForSaving.MaxLength = 250
        Me.txtNameBasisformForSaving.Name = "txtNameBasisformForSaving"
        Me.txtNameBasisformForSaving.Size = New System.Drawing.Size(153, 22)
        Me.txtNameBasisformForSaving.TabIndex = 7
        Me.txtNameBasisformForSaving.TabStop = False
        Me.txtNameBasisformForSaving.Text = "12345"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(67, 46)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(86, 13)
        Me.Label3.TabIndex = 6
        Me.Label3.Text = "Name Basisform:"
        '
        'txtNameBasisform
        '
        Me.txtNameBasisform.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtNameBasisform.Location = New System.Drawing.Point(70, 62)
        Me.txtNameBasisform.MaxLength = 250
        Me.txtNameBasisform.Name = "txtNameBasisform"
        Me.txtNameBasisform.ReadOnly = True
        Me.txtNameBasisform.Size = New System.Drawing.Size(73, 22)
        Me.txtNameBasisform.TabIndex = 3
        Me.txtNameBasisform.TabStop = False
        Me.txtNameBasisform.Text = "txtNameBasisform"
        '
        'lblToolboxWerkbank
        '
        Me.lblToolboxWerkbank.Image = CType(resources.GetObject("lblToolboxWerkbank.Image"), System.Drawing.Image)
        Me.lblToolboxWerkbank.Location = New System.Drawing.Point(20, 45)
        Me.lblToolboxWerkbank.Name = "lblToolboxWerkbank"
        Me.lblToolboxWerkbank.Size = New System.Drawing.Size(32, 32)
        Me.lblToolboxWerkbank.TabIndex = 2
        '
        'MenuStrip_Werkbk
        '
        Me.MenuStrip_Werkbk.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsmiWerkbk_Datei})
        Me.MenuStrip_Werkbk.Location = New System.Drawing.Point(3, 3)
        Me.MenuStrip_Werkbk.Name = "MenuStrip_Werkbk"
        Me.MenuStrip_Werkbk.Size = New System.Drawing.Size(370, 25)
        Me.MenuStrip_Werkbk.TabIndex = 0
        Me.MenuStrip_Werkbk.Text = "MenuStrip2"
        '
        'tsmiWerkbk_Datei
        '
        Me.tsmiWerkbk_Datei.Name = "tsmiWerkbk_Datei"
        Me.tsmiWerkbk_Datei.Size = New System.Drawing.Size(50, 21)
        Me.tsmiWerkbk_Datei.Text = "Datei"
        '
        'TabPageHintergrund
        '
        Me.TabPageHintergrund.Controls.Add(Me.UctlToolboxHintergrund1)
        Me.TabPageHintergrund.Location = New System.Drawing.Point(4, 34)
        Me.TabPageHintergrund.Name = "TabPageHintergrund"
        Me.TabPageHintergrund.Size = New System.Drawing.Size(375, 515)
        Me.TabPageHintergrund.TabIndex = 4
        Me.TabPageHintergrund.Text = "Hintergrund"
        Me.TabPageHintergrund.UseVisualStyleBackColor = True
        '
        'UctlToolboxHintergrund1
        '
        Me.UctlToolboxHintergrund1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.UctlToolboxHintergrund1.Location = New System.Drawing.Point(0, 0)
        Me.UctlToolboxHintergrund1.Name = "UctlToolboxHintergrund1"
        Me.UctlToolboxHintergrund1.Padding = New System.Windows.Forms.Padding(0, 5, 0, 0)
        Me.UctlToolboxHintergrund1.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.UctlToolboxHintergrund1.Size = New System.Drawing.Size(375, 515)
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
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(384, 541)
        Me.Controls.Add(Me.TabControlToolBox)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.KeyPreview = True
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
        Me.TabPagePositionierer.ResumeLayout(False)
        Me.TabPagePositionierer.PerformLayout()
        Me.TabPageWerkbank.ResumeLayout(False)
        Me.TabPageWerkbank.PerformLayout()
        Me.MenuStrip_Werkbk.ResumeLayout(False)
        Me.MenuStrip_Werkbk.PerformLayout()
        Me.TabPageHintergrund.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents TabControlToolBox As TabControl
    Friend WithEvents TabPageEditor As TabPage
    Friend WithEvents TabPageWerkbank As TabPage
    Friend WithEvents MenuStrip_Editor As MenuStrip
    Friend WithEvents tsmiEditor_Datei As ToolStripMenuItem
    Friend WithEvents MenuStrip_Werkbk As MenuStrip
    Friend WithEvents tsmiWerkbk_Datei As ToolStripMenuItem
    Friend WithEvents ImageListToolBox As ImageList
    Friend WithEvents lblToolboxEditor As Label
    Friend WithEvents lblToolboxWerkbank As Label
    Friend WithEvents txtNameBasisform As TextBox
    Friend WithEvents Label3 As Label
    Friend WithEvents ToolTipToolBox As ToolTip
    Friend WithEvents Label4 As Label
    Friend WithEvents txtNameBasisformForSaving As TextBox
    Friend WithEvents Label8 As Label
    Friend WithEvents lblInfi1 As Label
    Friend WithEvents TabPagePositionierer As TabPage
    Friend WithEvents lblToolboxPositionierer As Label
    Friend WithEvents Label10 As Label
    Friend WithEvents Label12 As Label
    Friend WithEvents Label11 As Label
    Friend WithEvents PositioniererReferencePoint As ReferencePoint
    Friend WithEvents Label13 As Label
    Friend WithEvents positioniererBtnReDoStep As Button
    Friend WithEvents positioniererBtnUnDoEinbau As Button
    Friend WithEvents positioniererBtnAbbrechen As Button
    Friend WithEvents positioniererBtnÜbernehmen As Button
    Friend WithEvents positioniererBtnEinbau As Button
    Friend WithEvents positioniererBtnUndoStep As Button
    Friend WithEvents positioniererBtnReset As Button
    Friend WithEvents positioniererOffsetY As Num2UpDown
    Friend WithEvents positioniererOffsetX As Num2UpDown
    Friend WithEvents TabPageSpieler As TabPage
    Friend WithEvents lblToolboxSpieler As Label
    Friend WithEvents KompassRose1 As KompassRose
    Friend WithEvents KompassRoseX1 As KompassRoseX
    Friend WithEvents Positionierer1 As Positionierer
    Friend WithEvents TabPageHintergrund As TabPage
    Friend WithEvents UctlToolboxHintergrund1 As UctlToolboxHintergrund
    Friend WithEvents UCtlWerkbank1 As UCtlWerkbank
End Class
