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
        Me.TabPageNameBild = New System.Windows.Forms.TabPage()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.btnCreteSpielbild = New System.Windows.Forms.Button()
        Me.btnSpielbildLöschen = New System.Windows.Forms.Button()
        Me.picSpielfeldPicture = New System.Windows.Forms.PictureBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.txtAnmerkung = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.txtBeschreibung = New System.Windows.Forms.TextBox()
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
        Me.Num2UpDownSteineÜbereinander = New MahjongGK.Num2UpDown()
        Me.Num2UpDownSteineAufeinander = New MahjongGK.Num2UpDown()
        Me.Num2UpDownSteineNebeneinander = New MahjongGK.Num2UpDown()
        Me.TabPageHintergrund = New System.Windows.Forms.TabPage()
        Me.UctlToolboxHintergrund1 = New MahjongGK.UctlToolboxHintergrund()
        Me.ImageListToolBox = New System.Windows.Forms.ImageList(Me.components)
        Me.ToolTipToolBox = New System.Windows.Forms.ToolTip(Me.components)
        Me.tmrTxtNameAnmBeschrDebouncer = New System.Windows.Forms.Timer(Me.components)
        Me.tmrHideLblSpielSizeError = New System.Windows.Forms.Timer(Me.components)
        Me.TabControlToolBox.SuspendLayout()
        Me.TabPageNameBild.SuspendLayout()
        CType(Me.picSpielfeldPicture, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TabPageGröße.SuspendLayout()
        Me.Panel2.SuspendLayout()
        Me.Panel1.SuspendLayout()
        Me.TabPageHintergrund.SuspendLayout()
        Me.SuspendLayout()
        '
        'TabControlToolBox
        '
        Me.TabControlToolBox.Controls.Add(Me.TabPageNameBild)
        Me.TabControlToolBox.Controls.Add(Me.TabPageGröße)
        Me.TabControlToolBox.Controls.Add(Me.TabPageHintergrund)
        Me.TabControlToolBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TabControlToolBox.ImageList = Me.ImageListToolBox
        Me.TabControlToolBox.ItemSize = New System.Drawing.Size(75, 30)
        Me.TabControlToolBox.Location = New System.Drawing.Point(0, 0)
        Me.TabControlToolBox.Name = "TabControlToolBox"
        Me.TabControlToolBox.SelectedIndex = 0
        Me.TabControlToolBox.Size = New System.Drawing.Size(468, 533)
        Me.TabControlToolBox.TabIndex = 2
        '
        'TabPageNameBild
        '
        Me.TabPageNameBild.Controls.Add(Me.Label10)
        Me.TabPageNameBild.Controls.Add(Me.Label11)
        Me.TabPageNameBild.Controls.Add(Me.btnCreteSpielbild)
        Me.TabPageNameBild.Controls.Add(Me.btnSpielbildLöschen)
        Me.TabPageNameBild.Controls.Add(Me.picSpielfeldPicture)
        Me.TabPageNameBild.Controls.Add(Me.Label2)
        Me.TabPageNameBild.Controls.Add(Me.txtAnmerkung)
        Me.TabPageNameBild.Controls.Add(Me.Label1)
        Me.TabPageNameBild.Controls.Add(Me.txtBeschreibung)
        Me.TabPageNameBild.Controls.Add(Me.lblName)
        Me.TabPageNameBild.Controls.Add(Me.txtName)
        Me.TabPageNameBild.Controls.Add(Me.lblToolboxSpieler)
        Me.TabPageNameBild.ImageKey = "(Keine)"
        Me.TabPageNameBild.Location = New System.Drawing.Point(4, 34)
        Me.TabPageNameBild.Name = "TabPageNameBild"
        Me.TabPageNameBild.Size = New System.Drawing.Size(460, 495)
        Me.TabPageNameBild.TabIndex = 3
        Me.TabPageNameBild.Text = "Name und Bild"
        Me.TabPageNameBild.UseVisualStyleBackColor = True
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Location = New System.Drawing.Point(174, 452)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(165, 26)
        Me.Label10.TabIndex = 26
        Me.Label10.Text = "Das Bild wird aus dem aktuellen" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Aussehen des Editors gewonnen."
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Location = New System.Drawing.Point(63, 241)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(170, 13)
        Me.Label11.TabIndex = 23
        Me.Label11.Text = "Bild (erscheint in der Spielauswahl)"
        '
        'btnCreteSpielbild
        '
        Me.btnCreteSpielbild.Location = New System.Drawing.Point(70, 454)
        Me.btnCreteSpielbild.Name = "btnCreteSpielbild"
        Me.btnCreteSpielbild.Size = New System.Drawing.Size(75, 23)
        Me.btnCreteSpielbild.TabIndex = 22
        Me.btnCreteSpielbild.Text = "erzeugen"
        Me.btnCreteSpielbild.UseVisualStyleBackColor = True
        '
        'btnSpielbildLöschen
        '
        Me.btnSpielbildLöschen.Location = New System.Drawing.Point(364, 454)
        Me.btnSpielbildLöschen.Name = "btnSpielbildLöschen"
        Me.btnSpielbildLöschen.Size = New System.Drawing.Size(75, 23)
        Me.btnSpielbildLöschen.TabIndex = 25
        Me.btnSpielbildLöschen.Text = "löschen"
        Me.btnSpielbildLöschen.UseVisualStyleBackColor = True
        '
        'picSpielfeldPicture
        '
        Me.picSpielfeldPicture.Location = New System.Drawing.Point(66, 264)
        Me.picSpielfeldPicture.Name = "picSpielfeldPicture"
        Me.picSpielfeldPicture.Size = New System.Drawing.Size(380, 165)
        Me.picSpielfeldPicture.TabIndex = 24
        Me.picSpielfeldPicture.TabStop = False
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(63, 146)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(160, 13)
        Me.Label2.TabIndex = 14
        Me.Label2.Text = "Anmerkung (erscheinen nur hier)"
        '
        'txtAnmerkung
        '
        Me.txtAnmerkung.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.txtAnmerkung.Location = New System.Drawing.Point(66, 164)
        Me.txtAnmerkung.MaxLength = 250
        Me.txtAnmerkung.Multiline = True
        Me.txtAnmerkung.Name = "txtAnmerkung"
        Me.txtAnmerkung.Size = New System.Drawing.Size(380, 43)
        Me.txtAnmerkung.TabIndex = 13
        Me.txtAnmerkung.Text = "Zeile1" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Zeile2" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Zeile3"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(63, 74)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(218, 13)
        Me.Label1.TabIndex = 12
        Me.Label1.Text = "Beschreibung (erscheint in der Spielauswahl)"
        '
        'txtBeschreibung
        '
        Me.txtBeschreibung.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.txtBeschreibung.Location = New System.Drawing.Point(66, 92)
        Me.txtBeschreibung.MaxLength = 250
        Me.txtBeschreibung.Multiline = True
        Me.txtBeschreibung.Name = "txtBeschreibung"
        Me.txtBeschreibung.Size = New System.Drawing.Size(380, 43)
        Me.txtBeschreibung.TabIndex = 11
        Me.txtBeschreibung.Text = "Zeile1" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Zeile2" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Zeile3"
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
        Me.btnChangeSize.Location = New System.Drawing.Point(69, 305)
        Me.btnChangeSize.Name = "btnChangeSize"
        Me.btnChangeSize.Size = New System.Drawing.Size(288, 32)
        Me.btnChangeSize.TabIndex = 20
        Me.btnChangeSize.Text = "Änderungen übernehmen"
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
        Me.optChangeBottom.Size = New System.Drawing.Size(126, 17)
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
        Me.optChangeTopBottom.Size = New System.Drawing.Size(56, 17)
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
        Me.optChangeTop.Size = New System.Drawing.Size(67, 17)
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
        Me.Label9.Size = New System.Drawing.Size(206, 13)
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
        Me.optChangeRight.Size = New System.Drawing.Size(128, 17)
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
        Me.optChangeLeftRight.Size = New System.Drawing.Size(69, 17)
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
        Me.optChangeLeft.Size = New System.Drawing.Size(64, 17)
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
        Me.Label8.Size = New System.Drawing.Size(215, 13)
        Me.Label8.TabIndex = 0
        Me.Label8.Text = "Größenänderung der Steine nebeneinander:"
        '
        'lblInfoGrenzen
        '
        Me.lblInfoGrenzen.AutoSize = True
        Me.lblInfoGrenzen.Location = New System.Drawing.Point(35, 383)
        Me.lblInfoGrenzen.Name = "lblInfoGrenzen"
        Me.lblInfoGrenzen.Size = New System.Drawing.Size(381, 91)
        Me.lblInfoGrenzen.TabIndex = 16
        Me.lblInfoGrenzen.Text = resources.GetString("lblInfoGrenzen.Text")
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(164, 132)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(63, 13)
        Me.Label6.TabIndex = 15
        Me.Label6.Text = "aufeinander"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(163, 105)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(69, 13)
        Me.Label5.TabIndex = 14
        Me.Label5.Text = "übereinander"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(65, 79)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(173, 13)
        Me.Label4.TabIndex = 13
        Me.Label4.Text = "Anzahl der STEINE nebeneinander"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(66, 27)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(110, 13)
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
        Me.TabPageNameBild.ResumeLayout(False)
        Me.TabPageNameBild.PerformLayout()
        CType(Me.picSpielfeldPicture, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TabPageGröße.ResumeLayout(False)
        Me.TabPageGröße.PerformLayout()
        Me.Panel2.ResumeLayout(False)
        Me.Panel2.PerformLayout()
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        Me.TabPageHintergrund.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents TabControlToolBox As TabControl
    Friend WithEvents ImageListToolBox As ImageList
    Friend WithEvents ToolTipToolBox As ToolTip
    Friend WithEvents TabPageHintergrund As TabPage
    Friend WithEvents UctlToolboxHintergrund1 As UctlToolboxHintergrund
    Friend WithEvents TabPageNameBild As TabPage
    Friend WithEvents lblToolboxSpieler As Label
    Friend WithEvents Label10 As Label
    Friend WithEvents Label11 As Label
    Friend WithEvents btnCreteSpielbild As Button
    Friend WithEvents Label2 As Label
    Friend WithEvents txtAnmerkung As TextBox
    Friend WithEvents Label1 As Label
    Friend WithEvents txtBeschreibung As TextBox
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
    Friend WithEvents btnSpielbildLöschen As Button
    Friend WithEvents picSpielfeldPicture As PictureBox
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
End Class
