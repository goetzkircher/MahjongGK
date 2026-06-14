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
        Me.TabPageNameSpeicherung = New System.Windows.Forms.TabPage()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.Panel4 = New System.Windows.Forms.Panel()
        Me.optOhneEinstufung = New System.Windows.Forms.RadioButton()
        Me.optProfi = New System.Windows.Forms.RadioButton()
        Me.optSchwer = New System.Windows.Forms.RadioButton()
        Me.optMittel = New System.Windows.Forms.RadioButton()
        Me.optLeicht = New System.Windows.Forms.RadioButton()
        Me.Panel3 = New System.Windows.Forms.Panel()
        Me.optSaveAußerhalb = New System.Windows.Forms.RadioButton()
        Me.optSaveEigeneSammlung = New System.Windows.Forms.RadioButton()
        Me.optSaveSpielesammlung = New System.Windows.Forms.RadioButton()
        Me.btnSave = New System.Windows.Forms.Button()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.txtAnmerkung = New System.Windows.Forms.TextBox()
        Me.lblName = New System.Windows.Forms.Label()
        Me.txtName = New System.Windows.Forms.TextBox()
        Me.lblToolboxSpieler = New System.Windows.Forms.Label()
        Me.TabPageGröße = New System.Windows.Forms.TabPage()
        Me.btnSteineReset = New System.Windows.Forms.Button()
        Me.lblSpielSizeOK = New System.Windows.Forms.Label()
        Me.btnChangeSize = New System.Windows.Forms.Button()
        Me.lblSpielSizeError = New System.Windows.Forms.Label()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.optChangeBottom = New System.Windows.Forms.RadioButton()
        Me.optChangeTopBottom = New System.Windows.Forms.RadioButton()
        Me.optChangeTop = New System.Windows.Forms.RadioButton()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.optChangeRight = New System.Windows.Forms.RadioButton()
        Me.optChangeLeftRight = New System.Windows.Forms.RadioButton()
        Me.optChangeLeft = New System.Windows.Forms.RadioButton()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.lblInfoGrenzen = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.lblToolboxEditor = New System.Windows.Forms.Label()
        Me.TabPageHintergrund = New System.Windows.Forms.TabPage()
        Me.ImageListToolBox = New System.Windows.Forms.ImageList(Me.components)
        Me.ToolTipToolBox = New System.Windows.Forms.ToolTip(Me.components)
        Me.tmrTxtNameAnmBeschrDebouncer = New System.Windows.Forms.Timer(Me.components)
        Me.tmrHideLblSpielSizeError = New System.Windows.Forms.Timer(Me.components)
        Me.tmrDelaySave = New System.Windows.Forms.Timer(Me.components)
        Me.Num2UpDownSteineÜbereinander = New MahjongGK.Num2UpDown()
        Me.Num2UpDownSteineAufeinander = New MahjongGK.Num2UpDown()
        Me.Num2UpDownSteineNebeneinander = New MahjongGK.Num2UpDown()
        Me.UctlToolboxHintergrund1 = New MahjongGK.UctlToolboxHintergrund()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Panel5 = New System.Windows.Forms.Panel()
        Me.optSteinFontSegeo = New System.Windows.Forms.RadioButton()
        Me.optSteinFontNoto = New System.Windows.Forms.RadioButton()
        Me.Panel6 = New System.Windows.Forms.Panel()
        Me.optSteinDesignDefault = New System.Windows.Forms.RadioButton()
        Me.optSteinDesignTest = New System.Windows.Forms.RadioButton()
        Me.optSteinDesignEditorSpezial = New System.Windows.Forms.RadioButton()
        Me.optSteinDesignUniGelb = New System.Windows.Forms.RadioButton()
        Me.optSteinDesignUniDunkel = New System.Windows.Forms.RadioButton()
        Me.Panel7 = New System.Windows.Forms.Panel()
        Me.optSteinSatzMedium = New System.Windows.Forms.RadioButton()
        Me.optSteinSatzLight = New System.Windows.Forms.RadioButton()
        Me.optSteinSatzDark = New System.Windows.Forms.RadioButton()
        Me.Panel8 = New System.Windows.Forms.Panel()
        Me.btnClose = New System.Windows.Forms.Button()
        Me.TabControlToolBox.SuspendLayout()
        Me.TabPageNameSpeicherung.SuspendLayout()
        Me.Panel4.SuspendLayout()
        Me.Panel3.SuspendLayout()
        Me.TabPageGröße.SuspendLayout()
        Me.Panel2.SuspendLayout()
        Me.Panel1.SuspendLayout()
        Me.TabPageHintergrund.SuspendLayout()
        Me.Panel5.SuspendLayout()
        Me.Panel6.SuspendLayout()
        Me.Panel7.SuspendLayout()
        Me.Panel8.SuspendLayout()
        Me.SuspendLayout()
        '
        'TabControlToolBox
        '
        Me.TabControlToolBox.Controls.Add(Me.TabPageNameSpeicherung)
        Me.TabControlToolBox.Controls.Add(Me.TabPageGröße)
        Me.TabControlToolBox.Controls.Add(Me.TabPageHintergrund)
        Me.TabControlToolBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TabControlToolBox.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TabControlToolBox.ImageList = Me.ImageListToolBox
        Me.TabControlToolBox.ItemSize = New System.Drawing.Size(75, 30)
        Me.TabControlToolBox.Location = New System.Drawing.Point(0, 0)
        Me.TabControlToolBox.Name = "TabControlToolBox"
        Me.TabControlToolBox.SelectedIndex = 0
        Me.TabControlToolBox.Size = New System.Drawing.Size(468, 533)
        Me.TabControlToolBox.TabIndex = 2
        '
        'TabPageNameSpeicherung
        '
        Me.TabPageNameSpeicherung.Controls.Add(Me.btnClose)
        Me.TabPageNameSpeicherung.Controls.Add(Me.Panel8)
        Me.TabPageNameSpeicherung.Controls.Add(Me.Label7)
        Me.TabPageNameSpeicherung.Controls.Add(Me.Panel4)
        Me.TabPageNameSpeicherung.Controls.Add(Me.Panel3)
        Me.TabPageNameSpeicherung.Controls.Add(Me.btnSave)
        Me.TabPageNameSpeicherung.Controls.Add(Me.Label2)
        Me.TabPageNameSpeicherung.Controls.Add(Me.txtAnmerkung)
        Me.TabPageNameSpeicherung.Controls.Add(Me.lblName)
        Me.TabPageNameSpeicherung.Controls.Add(Me.txtName)
        Me.TabPageNameSpeicherung.Controls.Add(Me.lblToolboxSpieler)
        Me.TabPageNameSpeicherung.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TabPageNameSpeicherung.ImageKey = "(Keine)"
        Me.TabPageNameSpeicherung.Location = New System.Drawing.Point(4, 34)
        Me.TabPageNameSpeicherung.Name = "TabPageNameSpeicherung"
        Me.TabPageNameSpeicherung.Size = New System.Drawing.Size(460, 495)
        Me.TabPageNameSpeicherung.TabIndex = 3
        Me.TabPageNameSpeicherung.Text = "Name, Beschreibung und Speicherung"
        Me.TabPageNameSpeicherung.UseVisualStyleBackColor = True
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(63, 156)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(161, 13)
        Me.Label7.TabIndex = 24
        Me.Label7.Text = "Schwierigkeitsgrad / Speicherort"
        '
        'Panel4
        '
        Me.Panel4.Controls.Add(Me.optOhneEinstufung)
        Me.Panel4.Controls.Add(Me.optProfi)
        Me.Panel4.Controls.Add(Me.optSchwer)
        Me.Panel4.Controls.Add(Me.optMittel)
        Me.Panel4.Controls.Add(Me.optLeicht)
        Me.Panel4.Location = New System.Drawing.Point(66, 180)
        Me.Panel4.Name = "Panel4"
        Me.Panel4.Size = New System.Drawing.Size(138, 75)
        Me.Panel4.TabIndex = 23
        '
        'optOhneEinstufung
        '
        Me.optOhneEinstufung.AutoSize = True
        Me.optOhneEinstufung.Checked = True
        Me.optOhneEinstufung.Location = New System.Drawing.Point(4, 4)
        Me.optOhneEinstufung.Name = "optOhneEinstufung"
        Me.optOhneEinstufung.Size = New System.Drawing.Size(104, 17)
        Me.optOhneEinstufung.TabIndex = 4
        Me.optOhneEinstufung.TabStop = True
        Me.optOhneEinstufung.Text = "Ohne Einstufung"
        Me.optOhneEinstufung.UseVisualStyleBackColor = True
        '
        'optProfi
        '
        Me.optProfi.AutoSize = True
        Me.optProfi.Location = New System.Drawing.Point(71, 50)
        Me.optProfi.Name = "optProfi"
        Me.optProfi.Size = New System.Drawing.Size(46, 17)
        Me.optProfi.TabIndex = 3
        Me.optProfi.TabStop = True
        Me.optProfi.Text = "Profi"
        Me.optProfi.UseVisualStyleBackColor = True
        '
        'optSchwer
        '
        Me.optSchwer.AutoSize = True
        Me.optSchwer.Location = New System.Drawing.Point(71, 27)
        Me.optSchwer.Name = "optSchwer"
        Me.optSchwer.Size = New System.Drawing.Size(61, 17)
        Me.optSchwer.TabIndex = 2
        Me.optSchwer.TabStop = True
        Me.optSchwer.Text = "Schwer"
        Me.optSchwer.UseVisualStyleBackColor = True
        '
        'optMittel
        '
        Me.optMittel.AutoSize = True
        Me.optMittel.Location = New System.Drawing.Point(4, 50)
        Me.optMittel.Name = "optMittel"
        Me.optMittel.Size = New System.Drawing.Size(50, 17)
        Me.optMittel.TabIndex = 1
        Me.optMittel.TabStop = True
        Me.optMittel.Text = "Mittel"
        Me.optMittel.UseVisualStyleBackColor = True
        '
        'optLeicht
        '
        Me.optLeicht.AutoSize = True
        Me.optLeicht.Location = New System.Drawing.Point(4, 27)
        Me.optLeicht.Name = "optLeicht"
        Me.optLeicht.Size = New System.Drawing.Size(54, 17)
        Me.optLeicht.TabIndex = 0
        Me.optLeicht.TabStop = True
        Me.optLeicht.Text = "Leicht"
        Me.optLeicht.UseVisualStyleBackColor = True
        '
        'Panel3
        '
        Me.Panel3.Controls.Add(Me.optSaveAußerhalb)
        Me.Panel3.Controls.Add(Me.optSaveEigeneSammlung)
        Me.Panel3.Controls.Add(Me.optSaveSpielesammlung)
        Me.Panel3.Location = New System.Drawing.Point(210, 180)
        Me.Panel3.Name = "Panel3"
        Me.Panel3.Size = New System.Drawing.Size(223, 80)
        Me.Panel3.TabIndex = 22
        '
        'optSaveAußerhalb
        '
        Me.optSaveAußerhalb.AutoSize = True
        Me.optSaveAußerhalb.Location = New System.Drawing.Point(4, 28)
        Me.optSaveAußerhalb.Name = "optSaveAußerhalb"
        Me.optSaveAußerhalb.Size = New System.Drawing.Size(203, 17)
        Me.optSaveAußerhalb.TabIndex = 2
        Me.optSaveAußerhalb.Text = "Speichern außerhalb von MahjongGK"
        Me.optSaveAußerhalb.UseVisualStyleBackColor = True
        '
        'optSaveEigeneSammlung
        '
        Me.optSaveEigeneSammlung.AutoSize = True
        Me.optSaveEigeneSammlung.Checked = True
        Me.optSaveEigeneSammlung.Location = New System.Drawing.Point(4, 5)
        Me.optSaveEigeneSammlung.Name = "optSaveEigeneSammlung"
        Me.optSaveEigeneSammlung.Size = New System.Drawing.Size(195, 17)
        Me.optSaveEigeneSammlung.TabIndex = 1
        Me.optSaveEigeneSammlung.TabStop = True
        Me.optSaveEigeneSammlung.Text = "Speichern in der eigenen Sammlung"
        Me.optSaveEigeneSammlung.UseVisualStyleBackColor = True
        '
        'optSaveSpielesammlung
        '
        Me.optSaveSpielesammlung.AutoSize = True
        Me.optSaveSpielesammlung.Location = New System.Drawing.Point(4, 51)
        Me.optSaveSpielesammlung.Name = "optSaveSpielesammlung"
        Me.optSaveSpielesammlung.Size = New System.Drawing.Size(183, 17)
        Me.optSaveSpielesammlung.TabIndex = 0
        Me.optSaveSpielesammlung.Text = "Speicherin in der Spielesammlung"
        Me.optSaveSpielesammlung.UseVisualStyleBackColor = True
        '
        'btnSave
        '
        Me.btnSave.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnSave.Location = New System.Drawing.Point(309, 444)
        Me.btnSave.Name = "btnSave"
        Me.btnSave.Size = New System.Drawing.Size(109, 32)
        Me.btnSave.TabIndex = 21
        Me.btnSave.Text = "Speichern"
        Me.btnSave.UseVisualStyleBackColor = True
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(63, 78)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(160, 13)
        Me.Label2.TabIndex = 14
        Me.Label2.Text = "Anmerkung (erscheinen nur hier)"
        '
        'txtAnmerkung
        '
        Me.txtAnmerkung.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.txtAnmerkung.Location = New System.Drawing.Point(66, 96)
        Me.txtAnmerkung.MaxLength = 250
        Me.txtAnmerkung.Multiline = True
        Me.txtAnmerkung.Name = "txtAnmerkung"
        Me.txtAnmerkung.Size = New System.Drawing.Size(380, 43)
        Me.txtAnmerkung.TabIndex = 13
        Me.txtAnmerkung.Text = "Zeile1" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Zeile2" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Zeile3"
        '
        'lblName
        '
        Me.lblName.AutoSize = True
        Me.lblName.Location = New System.Drawing.Point(63, 30)
        Me.lblName.Name = "lblName"
        Me.lblName.Size = New System.Drawing.Size(327, 13)
        Me.lblName.TabIndex = 10
        Me.lblName.Text = "Name (erscheint im Spiel/Editor oben mittig und in der Spielauswahl)"
        '
        'txtName
        '
        Me.txtName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.txtName.Location = New System.Drawing.Point(66, 48)
        Me.txtName.MaxLength = 120
        Me.txtName.Name = "txtName"
        Me.txtName.Size = New System.Drawing.Size(380, 20)
        Me.txtName.TabIndex = 9
        Me.txtName.Text = "123456789012345678901234567890"
        '
        'lblToolboxSpieler
        '
        Me.lblToolboxSpieler.Image = CType(resources.GetObject("lblToolboxSpieler.Image"), System.Drawing.Image)
        Me.lblToolboxSpieler.Location = New System.Drawing.Point(22, 22)
        Me.lblToolboxSpieler.Name = "lblToolboxSpieler"
        Me.lblToolboxSpieler.Size = New System.Drawing.Size(34, 31)
        Me.lblToolboxSpieler.TabIndex = 2
        '
        'TabPageGröße
        '
        Me.TabPageGröße.Controls.Add(Me.btnSteineReset)
        Me.TabPageGröße.Controls.Add(Me.lblSpielSizeOK)
        Me.TabPageGröße.Controls.Add(Me.btnChangeSize)
        Me.TabPageGröße.Controls.Add(Me.lblSpielSizeError)
        Me.TabPageGröße.Controls.Add(Me.Panel2)
        Me.TabPageGröße.Controls.Add(Me.Panel1)
        Me.TabPageGröße.Controls.Add(Me.lblInfoGrenzen)
        Me.TabPageGröße.Controls.Add(Me.Label6)
        Me.TabPageGröße.Controls.Add(Me.Label5)
        Me.TabPageGröße.Controls.Add(Me.Label4)
        Me.TabPageGröße.Controls.Add(Me.Label3)
        Me.TabPageGröße.Controls.Add(Me.lblToolboxEditor)
        Me.TabPageGröße.Controls.Add(Me.Num2UpDownSteineÜbereinander)
        Me.TabPageGröße.Controls.Add(Me.Num2UpDownSteineAufeinander)
        Me.TabPageGröße.Controls.Add(Me.Num2UpDownSteineNebeneinander)
        Me.TabPageGröße.ImageKey = "(Keine)"
        Me.TabPageGröße.Location = New System.Drawing.Point(4, 34)
        Me.TabPageGröße.Name = "TabPageGröße"
        Me.TabPageGröße.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPageGröße.Size = New System.Drawing.Size(460, 495)
        Me.TabPageGröße.TabIndex = 0
        Me.TabPageGröße.Text = "Größe"
        Me.TabPageGröße.UseVisualStyleBackColor = True
        '
        'btnSteineReset
        '
        Me.btnSteineReset.Location = New System.Drawing.Point(321, 98)
        Me.btnSteineReset.Name = "btnSteineReset"
        Me.btnSteineReset.Size = New System.Drawing.Size(75, 23)
        Me.btnSteineReset.TabIndex = 22
        Me.btnSteineReset.Text = "Reset"
        Me.btnSteineReset.UseVisualStyleBackColor = True
        '
        'lblSpielSizeOK
        '
        Me.lblSpielSizeOK.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblSpielSizeOK.ForeColor = System.Drawing.SystemColors.WindowText
        Me.lblSpielSizeOK.Location = New System.Drawing.Point(134, 229)
        Me.lblSpielSizeOK.Name = "lblSpielSizeOK"
        Me.lblSpielSizeOK.Size = New System.Drawing.Size(284, 29)
        Me.lblSpielSizeOK.TabIndex = 21
        Me.lblSpielSizeOK.Text = "Größenänderung übernommen"
        Me.lblSpielSizeOK.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.lblSpielSizeOK.Visible = False
        '
        'btnChangeSize
        '
        Me.btnChangeSize.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnChangeSize.Location = New System.Drawing.Point(248, 315)
        Me.btnChangeSize.Name = "btnChangeSize"
        Me.btnChangeSize.Size = New System.Drawing.Size(109, 32)
        Me.btnChangeSize.TabIndex = 20
        Me.btnChangeSize.Text = "Übernehmen"
        Me.btnChangeSize.UseVisualStyleBackColor = True
        '
        'lblSpielSizeError
        '
        Me.lblSpielSizeError.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblSpielSizeError.ForeColor = System.Drawing.Color.DarkRed
        Me.lblSpielSizeError.Location = New System.Drawing.Point(70, 215)
        Me.lblSpielSizeError.Name = "lblSpielSizeError"
        Me.lblSpielSizeError.Size = New System.Drawing.Size(284, 29)
        Me.lblSpielSizeError.TabIndex = 19
        Me.lblSpielSizeError.Text = "Die gewünschte Größenänderung " & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "kann nicht vorgenommen werden."
        Me.lblSpielSizeError.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.lblSpielSizeError.Visible = False
        '
        'Panel2
        '
        Me.Panel2.Controls.Add(Me.optChangeBottom)
        Me.Panel2.Controls.Add(Me.optChangeTopBottom)
        Me.Panel2.Controls.Add(Me.optChangeTop)
        Me.Panel2.Controls.Add(Me.Label9)
        Me.Panel2.Location = New System.Drawing.Point(70, 248)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(289, 50)
        Me.Panel2.TabIndex = 18
        '
        'optChangeBottom
        '
        Me.optChangeBottom.AutoSize = True
        Me.optChangeBottom.Location = New System.Drawing.Point(153, 22)
        Me.optChangeBottom.Name = "optChangeBottom"
        Me.optChangeBottom.Size = New System.Drawing.Size(145, 17)
        Me.optChangeBottom.TabIndex = 3
        Me.optChangeBottom.TabStop = True
        Me.optChangeBottom.Text = "nur unten vornehmen"
        Me.optChangeBottom.UseVisualStyleBackColor = True
        '
        'optChangeTopBottom
        '
        Me.optChangeTopBottom.AutoSize = True
        Me.optChangeTopBottom.Checked = True
        Me.optChangeTopBottom.Location = New System.Drawing.Point(78, 22)
        Me.optChangeTopBottom.Name = "optChangeTopBottom"
        Me.optChangeTopBottom.Size = New System.Drawing.Size(62, 17)
        Me.optChangeTopBottom.TabIndex = 2
        Me.optChangeTopBottom.TabStop = True
        Me.optChangeTopBottom.Text = "beides"
        Me.optChangeTopBottom.UseVisualStyleBackColor = True
        '
        'optChangeTop
        '
        Me.optChangeTop.AutoSize = True
        Me.optChangeTop.Location = New System.Drawing.Point(8, 22)
        Me.optChangeTop.Name = "optChangeTop"
        Me.optChangeTop.Size = New System.Drawing.Size(75, 17)
        Me.optChangeTop.TabIndex = 1
        Me.optChangeTop.TabStop = True
        Me.optChangeTop.Text = "nur oben"
        Me.optChangeTop.UseVisualStyleBackColor = True
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Location = New System.Drawing.Point(3, 5)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(245, 13)
        Me.Label9.TabIndex = 0
        Me.Label9.Text = "Größenänderung der Steine übereinander:"
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.optChangeRight)
        Me.Panel1.Controls.Add(Me.optChangeLeftRight)
        Me.Panel1.Controls.Add(Me.optChangeLeft)
        Me.Panel1.Controls.Add(Me.Label8)
        Me.Panel1.Location = New System.Drawing.Point(68, 161)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(289, 50)
        Me.Panel1.TabIndex = 17
        '
        'optChangeRight
        '
        Me.optChangeRight.AutoSize = True
        Me.optChangeRight.Location = New System.Drawing.Point(153, 22)
        Me.optChangeRight.Name = "optChangeRight"
        Me.optChangeRight.Size = New System.Drawing.Size(148, 17)
        Me.optChangeRight.TabIndex = 3
        Me.optChangeRight.TabStop = True
        Me.optChangeRight.Text = "nur rechts vornehmen"
        Me.optChangeRight.UseVisualStyleBackColor = True
        '
        'optChangeLeftRight
        '
        Me.optChangeLeftRight.AutoSize = True
        Me.optChangeLeftRight.Checked = True
        Me.optChangeLeftRight.Location = New System.Drawing.Point(78, 22)
        Me.optChangeLeftRight.Name = "optChangeLeftRight"
        Me.optChangeLeftRight.Size = New System.Drawing.Size(79, 17)
        Me.optChangeLeftRight.TabIndex = 2
        Me.optChangeLeftRight.TabStop = True
        Me.optChangeLeftRight.Text = "beidseitig"
        Me.optChangeLeftRight.UseVisualStyleBackColor = True
        '
        'optChangeLeft
        '
        Me.optChangeLeft.AutoSize = True
        Me.optChangeLeft.Location = New System.Drawing.Point(8, 22)
        Me.optChangeLeft.Name = "optChangeLeft"
        Me.optChangeLeft.Size = New System.Drawing.Size(73, 17)
        Me.optChangeLeft.TabIndex = 1
        Me.optChangeLeft.TabStop = True
        Me.optChangeLeft.Text = "nur links"
        Me.optChangeLeft.UseVisualStyleBackColor = True
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(4, 5)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(255, 13)
        Me.Label8.TabIndex = 0
        Me.Label8.Text = "Größenänderung der Steine nebeneinander:"
        '
        'lblInfoGrenzen
        '
        Me.lblInfoGrenzen.AutoSize = True
        Me.lblInfoGrenzen.Location = New System.Drawing.Point(35, 383)
        Me.lblInfoGrenzen.Name = "lblInfoGrenzen"
        Me.lblInfoGrenzen.Size = New System.Drawing.Size(467, 91)
        Me.lblInfoGrenzen.TabIndex = 16
        Me.lblInfoGrenzen.Text = resources.GetString("lblInfoGrenzen.Text")
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(164, 132)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(74, 13)
        Me.Label6.TabIndex = 15
        Me.Label6.Text = "aufeinander"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(163, 105)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(81, 13)
        Me.Label5.TabIndex = 14
        Me.Label5.Text = "übereinander"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(65, 79)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(204, 13)
        Me.Label4.TabIndex = 13
        Me.Label4.Text = "Anzahl der STEINE nebeneinander"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(66, 27)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(131, 13)
        Me.Label3.TabIndex = 9
        Me.Label3.Text = "Größe des Spielfeldes"
        '
        'lblToolboxEditor
        '
        Me.lblToolboxEditor.Image = CType(resources.GetObject("lblToolboxEditor.Image"), System.Drawing.Image)
        Me.lblToolboxEditor.Location = New System.Drawing.Point(22, 22)
        Me.lblToolboxEditor.Name = "lblToolboxEditor"
        Me.lblToolboxEditor.Size = New System.Drawing.Size(34, 31)
        Me.lblToolboxEditor.TabIndex = 1
        '
        'TabPageHintergrund
        '
        Me.TabPageHintergrund.Controls.Add(Me.UctlToolboxHintergrund1)
        Me.TabPageHintergrund.Location = New System.Drawing.Point(4, 34)
        Me.TabPageHintergrund.Name = "TabPageHintergrund"
        Me.TabPageHintergrund.Size = New System.Drawing.Size(460, 495)
        Me.TabPageHintergrund.TabIndex = 4
        Me.TabPageHintergrund.Text = "Hintergrund"
        Me.TabPageHintergrund.UseVisualStyleBackColor = True
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
        'tmrTxtNameAnmBeschrDebouncer
        '
        Me.tmrTxtNameAnmBeschrDebouncer.Interval = 250
        '
        'tmrHideLblSpielSizeError
        '
        Me.tmrHideLblSpielSizeError.Interval = 5000
        '
        'tmrDelaySave
        '
        Me.tmrDelaySave.Interval = 250
        '
        'Num2UpDownSteineÜbereinander
        '
        Me.Num2UpDownSteineÜbereinander.Location = New System.Drawing.Point(238, 97)
        Me.Num2UpDownSteineÜbereinander.MinimumSize = New System.Drawing.Size(65, 24)
        Me.Num2UpDownSteineÜbereinander.Name = "Num2UpDownSteineÜbereinander"
        Me.Num2UpDownSteineÜbereinander.Size = New System.Drawing.Size(65, 28)
        Me.Num2UpDownSteineÜbereinander.TabIndex = 12
        '
        'Num2UpDownSteineAufeinander
        '
        Me.Num2UpDownSteineAufeinander.Location = New System.Drawing.Point(238, 123)
        Me.Num2UpDownSteineAufeinander.MinimumSize = New System.Drawing.Size(65, 24)
        Me.Num2UpDownSteineAufeinander.Name = "Num2UpDownSteineAufeinander"
        Me.Num2UpDownSteineAufeinander.Size = New System.Drawing.Size(65, 28)
        Me.Num2UpDownSteineAufeinander.TabIndex = 11
        '
        'Num2UpDownSteineNebeneinander
        '
        Me.Num2UpDownSteineNebeneinander.Location = New System.Drawing.Point(238, 71)
        Me.Num2UpDownSteineNebeneinander.MinimumSize = New System.Drawing.Size(65, 24)
        Me.Num2UpDownSteineNebeneinander.Name = "Num2UpDownSteineNebeneinander"
        Me.Num2UpDownSteineNebeneinander.Size = New System.Drawing.Size(65, 28)
        Me.Num2UpDownSteineNebeneinander.TabIndex = 10
        '
        'UctlToolboxHintergrund1
        '
        Me.UctlToolboxHintergrund1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.UctlToolboxHintergrund1.Location = New System.Drawing.Point(0, 0)
        Me.UctlToolboxHintergrund1.Margin = New System.Windows.Forms.Padding(4)
        Me.UctlToolboxHintergrund1.Name = "UctlToolboxHintergrund1"
        Me.UctlToolboxHintergrund1.Padding = New System.Windows.Forms.Padding(0, 5, 0, 0)
        Me.UctlToolboxHintergrund1.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.UctlToolboxHintergrund1.Size = New System.Drawing.Size(460, 495)
        Me.UctlToolboxHintergrund1.TabIndex = 0
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(5, 5)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(190, 13)
        Me.Label1.TabIndex = 25
        Me.Label1.Text = "Für das Foto verwendetes SteinDesign"
        '
        'Panel5
        '
        Me.Panel5.Controls.Add(Me.optSteinFontSegeo)
        Me.Panel5.Controls.Add(Me.optSteinFontNoto)
        Me.Panel5.Location = New System.Drawing.Point(4, 22)
        Me.Panel5.Name = "Panel5"
        Me.Panel5.Size = New System.Drawing.Size(66, 53)
        Me.Panel5.TabIndex = 26
        '
        'optSteinFontSegeo
        '
        Me.optSteinFontSegeo.AutoSize = True
        Me.optSteinFontSegeo.Checked = True
        Me.optSteinFontSegeo.Location = New System.Drawing.Point(4, 4)
        Me.optSteinFontSegeo.Name = "optSteinFontSegeo"
        Me.optSteinFontSegeo.Size = New System.Drawing.Size(56, 17)
        Me.optSteinFontSegeo.TabIndex = 4
        Me.optSteinFontSegeo.TabStop = True
        Me.optSteinFontSegeo.Text = "Segeo"
        Me.optSteinFontSegeo.UseVisualStyleBackColor = True
        '
        'optSteinFontNoto
        '
        Me.optSteinFontNoto.AutoSize = True
        Me.optSteinFontNoto.Location = New System.Drawing.Point(4, 27)
        Me.optSteinFontNoto.Name = "optSteinFontNoto"
        Me.optSteinFontNoto.Size = New System.Drawing.Size(48, 17)
        Me.optSteinFontNoto.TabIndex = 0
        Me.optSteinFontNoto.TabStop = True
        Me.optSteinFontNoto.Text = "Noto"
        Me.optSteinFontNoto.UseVisualStyleBackColor = True
        '
        'Panel6
        '
        Me.Panel6.Controls.Add(Me.optSteinDesignDefault)
        Me.Panel6.Controls.Add(Me.optSteinDesignTest)
        Me.Panel6.Controls.Add(Me.optSteinDesignEditorSpezial)
        Me.Panel6.Controls.Add(Me.optSteinDesignUniGelb)
        Me.Panel6.Controls.Add(Me.optSteinDesignUniDunkel)
        Me.Panel6.Location = New System.Drawing.Point(76, 22)
        Me.Panel6.Name = "Panel6"
        Me.Panel6.Size = New System.Drawing.Size(110, 129)
        Me.Panel6.TabIndex = 27
        '
        'optSteinDesignDefault
        '
        Me.optSteinDesignDefault.AutoSize = True
        Me.optSteinDesignDefault.Checked = True
        Me.optSteinDesignDefault.Location = New System.Drawing.Point(4, 4)
        Me.optSteinDesignDefault.Name = "optSteinDesignDefault"
        Me.optSteinDesignDefault.Size = New System.Drawing.Size(59, 17)
        Me.optSteinDesignDefault.TabIndex = 4
        Me.optSteinDesignDefault.TabStop = True
        Me.optSteinDesignDefault.Text = "Default"
        Me.optSteinDesignDefault.UseVisualStyleBackColor = True
        '
        'optSteinDesignTest
        '
        Me.optSteinDesignTest.AutoSize = True
        Me.optSteinDesignTest.Location = New System.Drawing.Point(4, 96)
        Me.optSteinDesignTest.Name = "optSteinDesignTest"
        Me.optSteinDesignTest.Size = New System.Drawing.Size(46, 17)
        Me.optSteinDesignTest.TabIndex = 3
        Me.optSteinDesignTest.TabStop = True
        Me.optSteinDesignTest.Text = "Test"
        Me.optSteinDesignTest.UseVisualStyleBackColor = True
        '
        'optSteinDesignEditorSpezial
        '
        Me.optSteinDesignEditorSpezial.AutoSize = True
        Me.optSteinDesignEditorSpezial.Location = New System.Drawing.Point(4, 73)
        Me.optSteinDesignEditorSpezial.Name = "optSteinDesignEditorSpezial"
        Me.optSteinDesignEditorSpezial.Size = New System.Drawing.Size(84, 17)
        Me.optSteinDesignEditorSpezial.TabIndex = 2
        Me.optSteinDesignEditorSpezial.TabStop = True
        Me.optSteinDesignEditorSpezial.Text = "EdtorSpezial"
        Me.optSteinDesignEditorSpezial.UseVisualStyleBackColor = True
        '
        'optSteinDesignUniGelb
        '
        Me.optSteinDesignUniGelb.AutoSize = True
        Me.optSteinDesignUniGelb.Location = New System.Drawing.Point(4, 50)
        Me.optSteinDesignUniGelb.Name = "optSteinDesignUniGelb"
        Me.optSteinDesignUniGelb.Size = New System.Drawing.Size(91, 17)
        Me.optSteinDesignUniGelb.TabIndex = 1
        Me.optSteinDesignUniGelb.TabStop = True
        Me.optSteinDesignUniGelb.Text = "UniGelbBraun"
        Me.optSteinDesignUniGelb.UseVisualStyleBackColor = True
        '
        'optSteinDesignUniDunkel
        '
        Me.optSteinDesignUniDunkel.AutoSize = True
        Me.optSteinDesignUniDunkel.Location = New System.Drawing.Point(4, 27)
        Me.optSteinDesignUniDunkel.Name = "optSteinDesignUniDunkel"
        Me.optSteinDesignUniDunkel.Size = New System.Drawing.Size(75, 17)
        Me.optSteinDesignUniDunkel.TabIndex = 0
        Me.optSteinDesignUniDunkel.TabStop = True
        Me.optSteinDesignUniDunkel.Text = "UniDunkel"
        Me.optSteinDesignUniDunkel.UseVisualStyleBackColor = True
        '
        'Panel7
        '
        Me.Panel7.Controls.Add(Me.optSteinSatzMedium)
        Me.Panel7.Controls.Add(Me.optSteinSatzLight)
        Me.Panel7.Controls.Add(Me.optSteinSatzDark)
        Me.Panel7.Location = New System.Drawing.Point(192, 22)
        Me.Panel7.Name = "Panel7"
        Me.Panel7.Size = New System.Drawing.Size(70, 80)
        Me.Panel7.TabIndex = 28
        '
        'optSteinSatzMedium
        '
        Me.optSteinSatzMedium.AutoSize = True
        Me.optSteinSatzMedium.Location = New System.Drawing.Point(4, 28)
        Me.optSteinSatzMedium.Name = "optSteinSatzMedium"
        Me.optSteinSatzMedium.Size = New System.Drawing.Size(62, 17)
        Me.optSteinSatzMedium.TabIndex = 2
        Me.optSteinSatzMedium.Text = "Medium"
        Me.optSteinSatzMedium.UseVisualStyleBackColor = True
        '
        'optSteinSatzLight
        '
        Me.optSteinSatzLight.AutoSize = True
        Me.optSteinSatzLight.Checked = True
        Me.optSteinSatzLight.Location = New System.Drawing.Point(4, 5)
        Me.optSteinSatzLight.Name = "optSteinSatzLight"
        Me.optSteinSatzLight.Size = New System.Drawing.Size(48, 17)
        Me.optSteinSatzLight.TabIndex = 1
        Me.optSteinSatzLight.TabStop = True
        Me.optSteinSatzLight.Text = "Light"
        Me.optSteinSatzLight.UseVisualStyleBackColor = True
        '
        'optSteinSatzDark
        '
        Me.optSteinSatzDark.AutoSize = True
        Me.optSteinSatzDark.Location = New System.Drawing.Point(4, 51)
        Me.optSteinSatzDark.Name = "optSteinSatzDark"
        Me.optSteinSatzDark.Size = New System.Drawing.Size(48, 17)
        Me.optSteinSatzDark.TabIndex = 0
        Me.optSteinSatzDark.Text = "Dark"
        Me.optSteinSatzDark.UseVisualStyleBackColor = True
        '
        'Panel8
        '
        Me.Panel8.Controls.Add(Me.Panel5)
        Me.Panel8.Controls.Add(Me.Label1)
        Me.Panel8.Controls.Add(Me.Panel7)
        Me.Panel8.Controls.Add(Me.Panel6)
        Me.Panel8.Location = New System.Drawing.Point(66, 266)
        Me.Panel8.Name = "Panel8"
        Me.Panel8.Size = New System.Drawing.Size(309, 154)
        Me.Panel8.TabIndex = 29
        '
        'btnClose
        '
        Me.btnClose.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnClose.Location = New System.Drawing.Point(27, 444)
        Me.btnClose.Name = "btnClose"
        Me.btnClose.Size = New System.Drawing.Size(109, 32)
        Me.btnClose.TabIndex = 30
        Me.btnClose.Text = "Schließen"
        Me.btnClose.UseVisualStyleBackColor = True
        '
        'frmToolBox
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.ClientSize = New System.Drawing.Size(468, 533)
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
        Me.TabPageNameSpeicherung.ResumeLayout(False)
        Me.TabPageNameSpeicherung.PerformLayout()
        Me.Panel4.ResumeLayout(False)
        Me.Panel4.PerformLayout()
        Me.Panel3.ResumeLayout(False)
        Me.Panel3.PerformLayout()
        Me.TabPageGröße.ResumeLayout(False)
        Me.TabPageGröße.PerformLayout()
        Me.Panel2.ResumeLayout(False)
        Me.Panel2.PerformLayout()
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        Me.TabPageHintergrund.ResumeLayout(False)
        Me.Panel5.ResumeLayout(False)
        Me.Panel5.PerformLayout()
        Me.Panel6.ResumeLayout(False)
        Me.Panel6.PerformLayout()
        Me.Panel7.ResumeLayout(False)
        Me.Panel7.PerformLayout()
        Me.Panel8.ResumeLayout(False)
        Me.Panel8.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents TabControlToolBox As TabControl
    Friend WithEvents ImageListToolBox As ImageList
    Friend WithEvents ToolTipToolBox As ToolTip
    Friend WithEvents TabPageHintergrund As TabPage
    Friend WithEvents UctlToolboxHintergrund1 As UctlToolboxHintergrund
    Friend WithEvents TabPageNameSpeicherung As TabPage
    Friend WithEvents lblToolboxSpieler As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents txtAnmerkung As TextBox
    Friend WithEvents lblName As Label
    Friend WithEvents txtName As TextBox
    Friend WithEvents TabPageGröße As TabPage
    Friend WithEvents lblInfoGrenzen As Label
    Friend WithEvents Label6 As Label
    Friend WithEvents Label5 As Label
    Friend WithEvents Label4 As Label
    Friend WithEvents Num2UpDownSteineÜbereinander As Num2UpDown
    Friend WithEvents Num2UpDownSteineAufeinander As Num2UpDown
    Friend WithEvents Num2UpDownSteineNebeneinander As Num2UpDown
    Friend WithEvents Label3 As Label
    Friend WithEvents lblToolboxEditor As Label
    Friend WithEvents Panel1 As Panel
    Friend WithEvents Label8 As Label
    Friend WithEvents Panel2 As Panel
    Friend WithEvents optChangeBottom As RadioButton
    Friend WithEvents optChangeTopBottom As RadioButton
    Friend WithEvents optChangeTop As RadioButton
    Friend WithEvents Label9 As Label
    Friend WithEvents optChangeRight As RadioButton
    Friend WithEvents optChangeLeftRight As RadioButton
    Friend WithEvents optChangeLeft As RadioButton
    Friend WithEvents lblSpielSizeError As Label
    Friend WithEvents btnChangeSize As Button
    Friend WithEvents tmrTxtNameAnmBeschrDebouncer As Timer
    Friend WithEvents tmrHideLblSpielSizeError As Timer
    Friend WithEvents lblSpielSizeOK As Label
    Friend WithEvents btnSteineReset As Button
    Friend WithEvents btnSave As Button
    Friend WithEvents Panel3 As Panel
    Friend WithEvents optSaveEigeneSammlung As RadioButton
    Friend WithEvents optSaveSpielesammlung As RadioButton
    Friend WithEvents optSaveAußerhalb As RadioButton
    Friend WithEvents Panel4 As Panel
    Friend WithEvents optProfi As RadioButton
    Friend WithEvents optSchwer As RadioButton
    Friend WithEvents optMittel As RadioButton
    Friend WithEvents optLeicht As RadioButton
    Friend WithEvents Label7 As Label
    Friend WithEvents optOhneEinstufung As RadioButton
    Friend WithEvents tmrDelaySave As Timer
    Friend WithEvents Panel6 As Panel
    Friend WithEvents optSteinDesignDefault As RadioButton
    Friend WithEvents optSteinDesignTest As RadioButton
    Friend WithEvents optSteinDesignEditorSpezial As RadioButton
    Friend WithEvents optSteinDesignUniGelb As RadioButton
    Friend WithEvents optSteinDesignUniDunkel As RadioButton
    Friend WithEvents Panel5 As Panel
    Friend WithEvents optSteinFontSegeo As RadioButton
    Friend WithEvents optSteinFontNoto As RadioButton
    Friend WithEvents Label1 As Label
    Friend WithEvents Panel7 As Panel
    Friend WithEvents optSteinSatzMedium As RadioButton
    Friend WithEvents optSteinSatzLight As RadioButton
    Friend WithEvents optSteinSatzDark As RadioButton
    Friend WithEvents Panel8 As Panel
    Friend WithEvents btnClose As Button
End Class
