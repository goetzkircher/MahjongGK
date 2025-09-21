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
        Me.Panel5 = New System.Windows.Forms.Panel()
        Me.pnlmoveFrei = New System.Windows.Forms.GroupBox()
        Me.Label29 = New System.Windows.Forms.Label()
        Me.Label30 = New System.Windows.Forms.Label()
        Me.pnlmoveKreis = New System.Windows.Forms.Panel()
        Me.GroupBox8 = New System.Windows.Forms.GroupBox()
        Me.Label27 = New System.Windows.Forms.Label()
        Me.Label28 = New System.Windows.Forms.Label()
        Me.pnlmoveZufall = New System.Windows.Forms.Panel()
        Me.GroupBox7 = New System.Windows.Forms.GroupBox()
        Me.Label25 = New System.Windows.Forms.Label()
        Me.Label26 = New System.Windows.Forms.Label()
        Me.pnlmoveRechteck = New System.Windows.Forms.Panel()
        Me.GroupBox6 = New System.Windows.Forms.GroupBox()
        Me.Label23 = New System.Windows.Forms.Label()
        Me.Label24 = New System.Windows.Forms.Label()
        Me.pnlmoveKegel = New System.Windows.Forms.Panel()
        Me.GroupBox5 = New System.Windows.Forms.GroupBox()
        Me.Label21 = New System.Windows.Forms.Label()
        Me.Label22 = New System.Windows.Forms.Label()
        Me.pnlmovePyramide = New System.Windows.Forms.Panel()
        Me.GroupBox4 = New System.Windows.Forms.GroupBox()
        Me.Label19 = New System.Windows.Forms.Label()
        Me.Label20 = New System.Windows.Forms.Label()
        Me.pnlmoveUForm = New System.Windows.Forms.Panel()
        Me.GroupBox3 = New System.Windows.Forms.GroupBox()
        Me.Label17 = New System.Windows.Forms.Label()
        Me.Label18 = New System.Windows.Forms.Label()
        Me.pnlmoveWinkel = New System.Windows.Forms.Panel()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.Label15 = New System.Windows.Forms.Label()
        Me.Label16 = New System.Windows.Forms.Label()
        Me.lblInfi1 = New System.Windows.Forms.Label()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.pnlmoveLinie = New System.Windows.Forms.Panel()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.txtNameBasisformForSaving = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.txtNameBasisform = New System.Windows.Forms.TextBox()
        Me.lblToolboxWerkbank = New System.Windows.Forms.Label()
        Me.MenuStrip_Werkbk = New System.Windows.Forms.MenuStrip()
        Me.tsmiWerkbk_Datei = New System.Windows.Forms.ToolStripMenuItem()
        Me.num2UpDnFeldSizeZmax = New MahjongGK.Num2UpDown()
        Me.num2UpDnFeldSizeYmax = New MahjongGK.Num2UpDown()
        Me.num2UpDnFeldSizeXmax = New MahjongGK.Num2UpDown()
        Me.TabPageHintergrund = New System.Windows.Forms.TabPage()
        Me.UctlToolboxHintergrund1 = New MahjongGK.UctlToolboxHintergrund()
        Me.ImageListToolBox = New System.Windows.Forms.ImageList(Me.components)
        Me.ToolTipToolBox = New System.Windows.Forms.ToolTip(Me.components)
        Me.ButtonTooltip1 = New MahjongGK.ButtonTooltip()
        Me.TabControlToolBox.SuspendLayout()
        Me.TabPageSpieler.SuspendLayout()
        Me.TabPageEditor.SuspendLayout()
        Me.MenuStrip_Editor.SuspendLayout()
        Me.TabPagePositionierer.SuspendLayout()
        Me.TabPageWerkbank.SuspendLayout()
        Me.Panel5.SuspendLayout()
        Me.pnlmoveFrei.SuspendLayout()
        Me.pnlmoveKreis.SuspendLayout()
        Me.GroupBox8.SuspendLayout()
        Me.pnlmoveZufall.SuspendLayout()
        Me.GroupBox7.SuspendLayout()
        Me.pnlmoveRechteck.SuspendLayout()
        Me.GroupBox6.SuspendLayout()
        Me.pnlmoveKegel.SuspendLayout()
        Me.GroupBox5.SuspendLayout()
        Me.pnlmovePyramide.SuspendLayout()
        Me.GroupBox4.SuspendLayout()
        Me.pnlmoveUForm.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
        Me.pnlmoveWinkel.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.pnlmoveLinie.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
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
        Me.TabControlToolBox.Size = New System.Drawing.Size(1610, 553)
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
        Me.TabPageSpieler.Size = New System.Drawing.Size(1602, 515)
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
        Me.TabPageEditor.Size = New System.Drawing.Size(1602, 515)
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
        Me.MenuStrip_Editor.Size = New System.Drawing.Size(1596, 25)
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
        Me.TabPagePositionierer.Size = New System.Drawing.Size(1602, 515)
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
        Me.TabPageWerkbank.Controls.Add(Me.ButtonTooltip1)
        Me.TabPageWerkbank.Controls.Add(Me.Panel5)
        Me.TabPageWerkbank.Controls.Add(Me.pnlmoveKreis)
        Me.TabPageWerkbank.Controls.Add(Me.pnlmoveZufall)
        Me.TabPageWerkbank.Controls.Add(Me.pnlmoveRechteck)
        Me.TabPageWerkbank.Controls.Add(Me.pnlmoveKegel)
        Me.TabPageWerkbank.Controls.Add(Me.pnlmovePyramide)
        Me.TabPageWerkbank.Controls.Add(Me.pnlmoveUForm)
        Me.TabPageWerkbank.Controls.Add(Me.pnlmoveWinkel)
        Me.TabPageWerkbank.Controls.Add(Me.lblInfi1)
        Me.TabPageWerkbank.Controls.Add(Me.Label8)
        Me.TabPageWerkbank.Controls.Add(Me.Label5)
        Me.TabPageWerkbank.Controls.Add(Me.pnlmoveLinie)
        Me.TabPageWerkbank.Controls.Add(Me.Label4)
        Me.TabPageWerkbank.Controls.Add(Me.txtNameBasisformForSaving)
        Me.TabPageWerkbank.Controls.Add(Me.Label3)
        Me.TabPageWerkbank.Controls.Add(Me.txtNameBasisform)
        Me.TabPageWerkbank.Controls.Add(Me.lblToolboxWerkbank)
        Me.TabPageWerkbank.Controls.Add(Me.MenuStrip_Werkbk)
        Me.TabPageWerkbank.Controls.Add(Me.num2UpDnFeldSizeZmax)
        Me.TabPageWerkbank.Controls.Add(Me.num2UpDnFeldSizeYmax)
        Me.TabPageWerkbank.Controls.Add(Me.num2UpDnFeldSizeXmax)
        Me.TabPageWerkbank.Location = New System.Drawing.Point(4, 34)
        Me.TabPageWerkbank.Name = "TabPageWerkbank"
        Me.TabPageWerkbank.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPageWerkbank.Size = New System.Drawing.Size(1602, 515)
        Me.TabPageWerkbank.TabIndex = 1
        Me.TabPageWerkbank.Text = "Werkbank"
        Me.TabPageWerkbank.UseVisualStyleBackColor = True
        '
        'Panel5
        '
        Me.Panel5.Controls.Add(Me.pnlmoveFrei)
        Me.Panel5.Location = New System.Drawing.Point(1257, 252)
        Me.Panel5.Name = "Panel5"
        Me.Panel5.Size = New System.Drawing.Size(300, 240)
        Me.Panel5.TabIndex = 23
        '
        'pnlmoveFrei
        '
        Me.pnlmoveFrei.Controls.Add(Me.Label29)
        Me.pnlmoveFrei.Controls.Add(Me.Label30)
        Me.pnlmoveFrei.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlmoveFrei.Location = New System.Drawing.Point(0, 0)
        Me.pnlmoveFrei.Name = "pnlmoveFrei"
        Me.pnlmoveFrei.Size = New System.Drawing.Size(300, 240)
        Me.pnlmoveFrei.TabIndex = 0
        Me.pnlmoveFrei.TabStop = False
        Me.pnlmoveFrei.Text = "frei"
        '
        'Label29
        '
        Me.Label29.AutoSize = True
        Me.Label29.Location = New System.Drawing.Point(179, 33)
        Me.Label29.Name = "Label29"
        Me.Label29.Size = New System.Drawing.Size(37, 13)
        Me.Label29.TabIndex = 15
        Me.Label29.Text = "Länge"
        '
        'Label30
        '
        Me.Label30.AutoSize = True
        Me.Label30.Location = New System.Drawing.Point(23, 33)
        Me.Label30.Name = "Label30"
        Me.Label30.Size = New System.Drawing.Size(50, 13)
        Me.Label30.TabIndex = 14
        Me.Label30.Text = "Richtung"
        '
        'pnlmoveKreis
        '
        Me.pnlmoveKreis.Controls.Add(Me.GroupBox8)
        Me.pnlmoveKreis.Location = New System.Drawing.Point(1257, 6)
        Me.pnlmoveKreis.Name = "pnlmoveKreis"
        Me.pnlmoveKreis.Size = New System.Drawing.Size(300, 240)
        Me.pnlmoveKreis.TabIndex = 22
        '
        'GroupBox8
        '
        Me.GroupBox8.Controls.Add(Me.Label27)
        Me.GroupBox8.Controls.Add(Me.Label28)
        Me.GroupBox8.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupBox8.Location = New System.Drawing.Point(0, 0)
        Me.GroupBox8.Name = "GroupBox8"
        Me.GroupBox8.Size = New System.Drawing.Size(300, 240)
        Me.GroupBox8.TabIndex = 0
        Me.GroupBox8.TabStop = False
        Me.GroupBox8.Text = "Kreis"
        '
        'Label27
        '
        Me.Label27.AutoSize = True
        Me.Label27.Location = New System.Drawing.Point(179, 33)
        Me.Label27.Name = "Label27"
        Me.Label27.Size = New System.Drawing.Size(37, 13)
        Me.Label27.TabIndex = 15
        Me.Label27.Text = "Länge"
        '
        'Label28
        '
        Me.Label28.AutoSize = True
        Me.Label28.Location = New System.Drawing.Point(23, 33)
        Me.Label28.Name = "Label28"
        Me.Label28.Size = New System.Drawing.Size(50, 13)
        Me.Label28.TabIndex = 14
        Me.Label28.Text = "Richtung"
        '
        'pnlmoveZufall
        '
        Me.pnlmoveZufall.Controls.Add(Me.GroupBox7)
        Me.pnlmoveZufall.Location = New System.Drawing.Point(951, 252)
        Me.pnlmoveZufall.Name = "pnlmoveZufall"
        Me.pnlmoveZufall.Size = New System.Drawing.Size(300, 240)
        Me.pnlmoveZufall.TabIndex = 21
        '
        'GroupBox7
        '
        Me.GroupBox7.Controls.Add(Me.Label25)
        Me.GroupBox7.Controls.Add(Me.Label26)
        Me.GroupBox7.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupBox7.Location = New System.Drawing.Point(0, 0)
        Me.GroupBox7.Name = "GroupBox7"
        Me.GroupBox7.Size = New System.Drawing.Size(300, 240)
        Me.GroupBox7.TabIndex = 0
        Me.GroupBox7.TabStop = False
        Me.GroupBox7.Text = "Zufall"
        '
        'Label25
        '
        Me.Label25.AutoSize = True
        Me.Label25.Location = New System.Drawing.Point(179, 33)
        Me.Label25.Name = "Label25"
        Me.Label25.Size = New System.Drawing.Size(37, 13)
        Me.Label25.TabIndex = 15
        Me.Label25.Text = "Länge"
        '
        'Label26
        '
        Me.Label26.AutoSize = True
        Me.Label26.Location = New System.Drawing.Point(23, 33)
        Me.Label26.Name = "Label26"
        Me.Label26.Size = New System.Drawing.Size(50, 13)
        Me.Label26.TabIndex = 14
        Me.Label26.Text = "Richtung"
        '
        'pnlmoveRechteck
        '
        Me.pnlmoveRechteck.Controls.Add(Me.GroupBox6)
        Me.pnlmoveRechteck.Location = New System.Drawing.Point(951, 6)
        Me.pnlmoveRechteck.Name = "pnlmoveRechteck"
        Me.pnlmoveRechteck.Size = New System.Drawing.Size(300, 240)
        Me.pnlmoveRechteck.TabIndex = 20
        '
        'GroupBox6
        '
        Me.GroupBox6.Controls.Add(Me.Label23)
        Me.GroupBox6.Controls.Add(Me.Label24)
        Me.GroupBox6.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupBox6.Location = New System.Drawing.Point(0, 0)
        Me.GroupBox6.Name = "GroupBox6"
        Me.GroupBox6.Size = New System.Drawing.Size(300, 240)
        Me.GroupBox6.TabIndex = 0
        Me.GroupBox6.TabStop = False
        Me.GroupBox6.Text = "Rechteck"
        '
        'Label23
        '
        Me.Label23.AutoSize = True
        Me.Label23.Location = New System.Drawing.Point(179, 33)
        Me.Label23.Name = "Label23"
        Me.Label23.Size = New System.Drawing.Size(37, 13)
        Me.Label23.TabIndex = 15
        Me.Label23.Text = "Länge"
        '
        'Label24
        '
        Me.Label24.AutoSize = True
        Me.Label24.Location = New System.Drawing.Point(23, 33)
        Me.Label24.Name = "Label24"
        Me.Label24.Size = New System.Drawing.Size(50, 13)
        Me.Label24.TabIndex = 14
        Me.Label24.Text = "Richtung"
        '
        'pnlmoveKegel
        '
        Me.pnlmoveKegel.Controls.Add(Me.GroupBox5)
        Me.pnlmoveKegel.Location = New System.Drawing.Point(645, 252)
        Me.pnlmoveKegel.Name = "pnlmoveKegel"
        Me.pnlmoveKegel.Size = New System.Drawing.Size(300, 240)
        Me.pnlmoveKegel.TabIndex = 19
        '
        'GroupBox5
        '
        Me.GroupBox5.Controls.Add(Me.Label21)
        Me.GroupBox5.Controls.Add(Me.Label22)
        Me.GroupBox5.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupBox5.Location = New System.Drawing.Point(0, 0)
        Me.GroupBox5.Name = "GroupBox5"
        Me.GroupBox5.Size = New System.Drawing.Size(300, 240)
        Me.GroupBox5.TabIndex = 0
        Me.GroupBox5.TabStop = False
        Me.GroupBox5.Text = "Kegel"
        '
        'Label21
        '
        Me.Label21.AutoSize = True
        Me.Label21.Location = New System.Drawing.Point(179, 33)
        Me.Label21.Name = "Label21"
        Me.Label21.Size = New System.Drawing.Size(37, 13)
        Me.Label21.TabIndex = 15
        Me.Label21.Text = "Länge"
        '
        'Label22
        '
        Me.Label22.AutoSize = True
        Me.Label22.Location = New System.Drawing.Point(23, 33)
        Me.Label22.Name = "Label22"
        Me.Label22.Size = New System.Drawing.Size(50, 13)
        Me.Label22.TabIndex = 14
        Me.Label22.Text = "Richtung"
        '
        'pnlmovePyramide
        '
        Me.pnlmovePyramide.Controls.Add(Me.GroupBox4)
        Me.pnlmovePyramide.Location = New System.Drawing.Point(339, 252)
        Me.pnlmovePyramide.Name = "pnlmovePyramide"
        Me.pnlmovePyramide.Size = New System.Drawing.Size(300, 240)
        Me.pnlmovePyramide.TabIndex = 18
        '
        'GroupBox4
        '
        Me.GroupBox4.Controls.Add(Me.Label19)
        Me.GroupBox4.Controls.Add(Me.Label20)
        Me.GroupBox4.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupBox4.Location = New System.Drawing.Point(0, 0)
        Me.GroupBox4.Name = "GroupBox4"
        Me.GroupBox4.Size = New System.Drawing.Size(300, 240)
        Me.GroupBox4.TabIndex = 0
        Me.GroupBox4.TabStop = False
        Me.GroupBox4.Text = "Pyramide"
        '
        'Label19
        '
        Me.Label19.AutoSize = True
        Me.Label19.Location = New System.Drawing.Point(179, 33)
        Me.Label19.Name = "Label19"
        Me.Label19.Size = New System.Drawing.Size(37, 13)
        Me.Label19.TabIndex = 15
        Me.Label19.Text = "Länge"
        '
        'Label20
        '
        Me.Label20.AutoSize = True
        Me.Label20.Location = New System.Drawing.Point(23, 33)
        Me.Label20.Name = "Label20"
        Me.Label20.Size = New System.Drawing.Size(50, 13)
        Me.Label20.TabIndex = 14
        Me.Label20.Text = "Richtung"
        '
        'pnlmoveUForm
        '
        Me.pnlmoveUForm.Controls.Add(Me.GroupBox3)
        Me.pnlmoveUForm.Location = New System.Drawing.Point(645, 6)
        Me.pnlmoveUForm.Name = "pnlmoveUForm"
        Me.pnlmoveUForm.Size = New System.Drawing.Size(300, 240)
        Me.pnlmoveUForm.TabIndex = 17
        '
        'GroupBox3
        '
        Me.GroupBox3.Controls.Add(Me.Label17)
        Me.GroupBox3.Controls.Add(Me.Label18)
        Me.GroupBox3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupBox3.Location = New System.Drawing.Point(0, 0)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Size = New System.Drawing.Size(300, 240)
        Me.GroupBox3.TabIndex = 0
        Me.GroupBox3.TabStop = False
        Me.GroupBox3.Text = "U-Form"
        '
        'Label17
        '
        Me.Label17.AutoSize = True
        Me.Label17.Location = New System.Drawing.Point(179, 33)
        Me.Label17.Name = "Label17"
        Me.Label17.Size = New System.Drawing.Size(37, 13)
        Me.Label17.TabIndex = 15
        Me.Label17.Text = "Länge"
        '
        'Label18
        '
        Me.Label18.AutoSize = True
        Me.Label18.Location = New System.Drawing.Point(23, 33)
        Me.Label18.Name = "Label18"
        Me.Label18.Size = New System.Drawing.Size(50, 13)
        Me.Label18.TabIndex = 14
        Me.Label18.Text = "Richtung"
        '
        'pnlmoveWinkel
        '
        Me.pnlmoveWinkel.Controls.Add(Me.GroupBox2)
        Me.pnlmoveWinkel.Location = New System.Drawing.Point(339, 6)
        Me.pnlmoveWinkel.Name = "pnlmoveWinkel"
        Me.pnlmoveWinkel.Size = New System.Drawing.Size(300, 240)
        Me.pnlmoveWinkel.TabIndex = 16
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.Label15)
        Me.GroupBox2.Controls.Add(Me.Label16)
        Me.GroupBox2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupBox2.Location = New System.Drawing.Point(0, 0)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(300, 240)
        Me.GroupBox2.TabIndex = 0
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Winkel"
        '
        'Label15
        '
        Me.Label15.AutoSize = True
        Me.Label15.Location = New System.Drawing.Point(179, 33)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(37, 13)
        Me.Label15.TabIndex = 15
        Me.Label15.Text = "Länge"
        '
        'Label16
        '
        Me.Label16.AutoSize = True
        Me.Label16.Location = New System.Drawing.Point(23, 33)
        Me.Label16.Name = "Label16"
        Me.Label16.Size = New System.Drawing.Size(50, 13)
        Me.Label16.TabIndex = 14
        Me.Label16.Text = "Richtung"
        '
        'lblInfi1
        '
        Me.lblInfi1.Image = Global.MahjongGK.My.Resources.Resources.Info16qBlau
        Me.lblInfi1.Location = New System.Drawing.Point(15, 401)
        Me.lblInfi1.Name = "lblInfi1"
        Me.lblInfi1.Size = New System.Drawing.Size(16, 16)
        Me.lblInfi1.TabIndex = 14
        Me.ToolTipToolBox.SetToolTip(Me.lblInfi1, resources.GetString("lblInfi1.ToolTip"))
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(34, 403)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(277, 13)
        Me.Label8.TabIndex = 13
        Me.Label8.Text = "Die Zusammenstellung der Symbole ist nicht zum Spielen!"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(67, 97)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(219, 26)
        Me.Label5.TabIndex = 10
        Me.Label5.Text = "Größe des Grundrisses (in Steinen)" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Nebeneinander - Übereinander - Aufeinander"
        '
        'pnlmoveLinie
        '
        Me.pnlmoveLinie.Controls.Add(Me.GroupBox1)
        Me.pnlmoveLinie.Location = New System.Drawing.Point(14, 161)
        Me.pnlmoveLinie.Name = "pnlmoveLinie"
        Me.pnlmoveLinie.Size = New System.Drawing.Size(300, 240)
        Me.pnlmoveLinie.TabIndex = 9
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.Label7)
        Me.GroupBox1.Controls.Add(Me.Label6)
        Me.GroupBox1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupBox1.Location = New System.Drawing.Point(0, 0)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(300, 240)
        Me.GroupBox1.TabIndex = 0
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Linien"
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(179, 33)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(37, 13)
        Me.Label7.TabIndex = 15
        Me.Label7.Text = "Länge"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(23, 33)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(50, 13)
        Me.Label6.TabIndex = 14
        Me.Label6.Text = "Richtung"
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
        Me.MenuStrip_Werkbk.Size = New System.Drawing.Size(1596, 25)
        Me.MenuStrip_Werkbk.TabIndex = 0
        Me.MenuStrip_Werkbk.Text = "MenuStrip2"
        '
        'tsmiWerkbk_Datei
        '
        Me.tsmiWerkbk_Datei.Name = "tsmiWerkbk_Datei"
        Me.tsmiWerkbk_Datei.Size = New System.Drawing.Size(50, 21)
        Me.tsmiWerkbk_Datei.Text = "Datei"
        '
        'num2UpDnFeldSizeZmax
        '
        Me.num2UpDnFeldSizeZmax.Location = New System.Drawing.Point(226, 130)
        Me.num2UpDnFeldSizeZmax.MinimumSize = New System.Drawing.Size(65, 24)
        Me.num2UpDnFeldSizeZmax.Name = "num2UpDnFeldSizeZmax"
        Me.num2UpDnFeldSizeZmax.Size = New System.Drawing.Size(65, 28)
        Me.num2UpDnFeldSizeZmax.TabIndex = 12
        '
        'num2UpDnFeldSizeYmax
        '
        Me.num2UpDnFeldSizeYmax.Location = New System.Drawing.Point(148, 130)
        Me.num2UpDnFeldSizeYmax.MinimumSize = New System.Drawing.Size(65, 24)
        Me.num2UpDnFeldSizeYmax.Name = "num2UpDnFeldSizeYmax"
        Me.num2UpDnFeldSizeYmax.Size = New System.Drawing.Size(65, 28)
        Me.num2UpDnFeldSizeYmax.TabIndex = 11
        '
        'num2UpDnFeldSizeXmax
        '
        Me.num2UpDnFeldSizeXmax.Location = New System.Drawing.Point(70, 130)
        Me.num2UpDnFeldSizeXmax.MinimumSize = New System.Drawing.Size(65, 24)
        Me.num2UpDnFeldSizeXmax.Name = "num2UpDnFeldSizeXmax"
        Me.num2UpDnFeldSizeXmax.Size = New System.Drawing.Size(65, 28)
        Me.num2UpDnFeldSizeXmax.TabIndex = 5
        '
        'TabPageHintergrund
        '
        Me.TabPageHintergrund.Controls.Add(Me.UctlToolboxHintergrund1)
        Me.TabPageHintergrund.Location = New System.Drawing.Point(4, 34)
        Me.TabPageHintergrund.Name = "TabPageHintergrund"
        Me.TabPageHintergrund.Size = New System.Drawing.Size(1602, 515)
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
        Me.UctlToolboxHintergrund1.Size = New System.Drawing.Size(1602, 515)
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
        'ButtonTooltip1
        '
        Me.ButtonTooltip1.AccessibleDescription = "Zeigt einen Hinweis-Tooltip."
        Me.ButtonTooltip1.AccessibleName = "Info"
        Me.ButtonTooltip1.BackColor = System.Drawing.SystemColors.Control
        Me.ButtonTooltip1.Cursor = System.Windows.Forms.Cursors.Default
        Me.ButtonTooltip1.DarkMode = False
        Me.ButtonTooltip1.InfoHeader = "Info"
        Me.ButtonTooltip1.InfoText = "Rechtklick auf die Pfeile öffnet BuddySlider"
        Me.ButtonTooltip1.Location = New System.Drawing.Point(298, 132)
        Me.ButtonTooltip1.MinimumSize = New System.Drawing.Size(16, 16)
        Me.ButtonTooltip1.Name = "ButtonTooltip1"
        Me.ButtonTooltip1.ShowOnFocus = False
        Me.ButtonTooltip1.Size = New System.Drawing.Size(26, 26)
        Me.ButtonTooltip1.TabIndex = 24
        Me.ButtonTooltip1.TabStop = False
        Me.ButtonTooltip1.Text = "ButtonTooltip1"
        '
        'frmToolBox
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1610, 553)
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
        Me.Panel5.ResumeLayout(False)
        Me.pnlmoveFrei.ResumeLayout(False)
        Me.pnlmoveFrei.PerformLayout()
        Me.pnlmoveKreis.ResumeLayout(False)
        Me.GroupBox8.ResumeLayout(False)
        Me.GroupBox8.PerformLayout()
        Me.pnlmoveZufall.ResumeLayout(False)
        Me.GroupBox7.ResumeLayout(False)
        Me.GroupBox7.PerformLayout()
        Me.pnlmoveRechteck.ResumeLayout(False)
        Me.GroupBox6.ResumeLayout(False)
        Me.GroupBox6.PerformLayout()
        Me.pnlmoveKegel.ResumeLayout(False)
        Me.GroupBox5.ResumeLayout(False)
        Me.GroupBox5.PerformLayout()
        Me.pnlmovePyramide.ResumeLayout(False)
        Me.GroupBox4.ResumeLayout(False)
        Me.GroupBox4.PerformLayout()
        Me.pnlmoveUForm.ResumeLayout(False)
        Me.GroupBox3.ResumeLayout(False)
        Me.GroupBox3.PerformLayout()
        Me.pnlmoveWinkel.ResumeLayout(False)
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.pnlmoveLinie.ResumeLayout(False)
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
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
    Friend WithEvents num2UpDnFeldSizeXmax As Num2UpDown
    Friend WithEvents Label3 As Label
    Friend WithEvents ToolTipToolBox As ToolTip
    Friend WithEvents Label4 As Label
    Friend WithEvents txtNameBasisformForSaving As TextBox
    Friend WithEvents pnlmoveLinie As Panel
    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents num2UpDnFeldSizeZmax As Num2UpDown
    Friend WithEvents num2UpDnFeldSizeYmax As Num2UpDown
    Friend WithEvents Label5 As Label
    Friend WithEvents Label7 As Label
    Friend WithEvents Label6 As Label
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
    Friend WithEvents pnlmoveKegel As Panel
    Friend WithEvents GroupBox5 As GroupBox
    Friend WithEvents Label21 As Label
    Friend WithEvents Label22 As Label
    Friend WithEvents pnlmovePyramide As Panel
    Friend WithEvents GroupBox4 As GroupBox
    Friend WithEvents Label19 As Label
    Friend WithEvents Label20 As Label
    Friend WithEvents pnlmoveUForm As Panel
    Friend WithEvents GroupBox3 As GroupBox
    Friend WithEvents Label17 As Label
    Friend WithEvents Label18 As Label
    Friend WithEvents pnlmoveWinkel As Panel
    Friend WithEvents GroupBox2 As GroupBox
    Friend WithEvents Label15 As Label
    Friend WithEvents Label16 As Label
    Friend WithEvents Panel5 As Panel
    Friend WithEvents pnlmoveFrei As GroupBox
    Friend WithEvents Label29 As Label
    Friend WithEvents Label30 As Label
    Friend WithEvents pnlmoveKreis As Panel
    Friend WithEvents GroupBox8 As GroupBox
    Friend WithEvents Label27 As Label
    Friend WithEvents Label28 As Label
    Friend WithEvents pnlmoveZufall As Panel
    Friend WithEvents GroupBox7 As GroupBox
    Friend WithEvents Label25 As Label
    Friend WithEvents Label26 As Label
    Friend WithEvents pnlmoveRechteck As Panel
    Friend WithEvents GroupBox6 As GroupBox
    Friend WithEvents Label23 As Label
    Friend WithEvents Label24 As Label
    Friend WithEvents TabPageSpieler As TabPage
    Friend WithEvents lblToolboxSpieler As Label
    Friend WithEvents KompassRose1 As KompassRose
    Friend WithEvents KompassRoseX1 As KompassRoseX
    Friend WithEvents Positionierer1 As Positionierer
    Friend WithEvents TabPageHintergrund As TabPage
    Friend WithEvents UctlToolboxHintergrund1 As UctlToolboxHintergrund
    Friend WithEvents ButtonTooltip1 As ButtonTooltip
End Class
