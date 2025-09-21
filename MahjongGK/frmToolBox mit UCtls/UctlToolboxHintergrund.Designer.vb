<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class UctlToolboxHintergrund
    Inherits System.Windows.Forms.UserControl

    'UserControl überschreibt den Löschvorgang, um die Komponentenliste zu bereinigen.
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(UctlToolboxHintergrund))
        Me.GroupBox9 = New System.Windows.Forms.GroupBox()
        Me.TabControlToolboxHGrd = New System.Windows.Forms.TabControl()
        Me.TabPageAktSpiel = New System.Windows.Forms.TabPage()
        Me.gbxAktSpiel = New System.Windows.Forms.GroupBox()
        Me.chkToolboxHGrdEditorShowFraming = New System.Windows.Forms.CheckBox()
        Me.chkToolboxHGrdEditorUseSplFldEinstlg = New System.Windows.Forms.CheckBox()
        Me.GroupBox3 = New System.Windows.Forms.GroupBox()
        Me.lblToolboxHGrdEditorColor = New System.Windows.Forms.Label()
        Me.lblToolboxHGrdSplFldColor = New System.Windows.Forms.Label()
        Me.btnToolboxHGrdEditorColorClear = New System.Windows.Forms.Button()
        Me.btnToolboxHGrdSplFldColorClear = New System.Windows.Forms.Button()
        Me.btnToolboxHGrdEditorColor = New System.Windows.Forms.Button()
        Me.btnToolboxHGrdSplFldColor = New System.Windows.Forms.Button()
        Me.GroupBox4 = New System.Windows.Forms.GroupBox()
        Me.lblToolboxHGrdEditorRenderMode = New System.Windows.Forms.Label()
        Me.lblToolboxHGrdSplFldRenderMode = New System.Windows.Forms.Label()
        Me.picToolboxHGrdEditor = New System.Windows.Forms.PictureBox()
        Me.picToolboxHGrdSplFld = New System.Windows.Forms.PictureBox()
        Me.btnToolboxHGrdEditorBitmapClear = New System.Windows.Forms.Button()
        Me.btnToolboxHGrdSplFldBitmapClear = New System.Windows.Forms.Button()
        Me.btnToolboxHGrdEditorBitmapName = New System.Windows.Forms.Button()
        Me.btnToolboxHGrdSplFldBitmapName = New System.Windows.Forms.Button()
        Me.TabPageFallback = New System.Windows.Forms.TabPage()
        Me.gbxFallback = New System.Windows.Forms.GroupBox()
        Me.chkToolboxHGrdEditorUseSplFldEinstlgFallback = New System.Windows.Forms.CheckBox()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.lblToolboxHGrdEditorColorFallback = New System.Windows.Forms.Label()
        Me.lblToolboxHGrdSplFldColorFallback = New System.Windows.Forms.Label()
        Me.btnToolboxHGrdEditorColorClearFallback = New System.Windows.Forms.Button()
        Me.btnToolboxHGrdSplFldColorClearFallback = New System.Windows.Forms.Button()
        Me.btnToolboxHGrdEditorColorFallback = New System.Windows.Forms.Button()
        Me.btnToolboxHGrdSplFldColorFallback = New System.Windows.Forms.Button()
        Me.GroupBox5 = New System.Windows.Forms.GroupBox()
        Me.picToolboxHGrdSplFldFallback = New System.Windows.Forms.PictureBox()
        Me.lblToolboxHGrdEditorRenderModeFallback = New System.Windows.Forms.Label()
        Me.lblToolboxHGrdSplFldRenderModeFallback = New System.Windows.Forms.Label()
        Me.picToolboxHGrdEditorFallback = New System.Windows.Forms.PictureBox()
        Me.btnToolboxHGrdEditorBitmapClearFallback = New System.Windows.Forms.Button()
        Me.btnToolboxHGrdSplFldBitmapClearFallback = New System.Windows.Forms.Button()
        Me.btnToolboxHGrdEditorBitmapNameFallback = New System.Windows.Forms.Button()
        Me.btnToolboxHGrdSplFldBitmapNameFallback = New System.Windows.Forms.Button()
        Me.btnInfoAktSpiel = New MahjongGK.ButtonInfo()
        Me.ButtonInfo1 = New MahjongGK.ButtonInfo()
        Me.ButtonTooltip1 = New MahjongGK.ButtonTooltip()
        Me.ButtonTooltip2 = New MahjongGK.ButtonTooltip()
        Me.GroupBox9.SuspendLayout()
        Me.TabControlToolboxHGrd.SuspendLayout()
        Me.TabPageAktSpiel.SuspendLayout()
        Me.gbxAktSpiel.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
        Me.GroupBox4.SuspendLayout()
        CType(Me.picToolboxHGrdEditor, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.picToolboxHGrdSplFld, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TabPageFallback.SuspendLayout()
        Me.gbxFallback.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.GroupBox5.SuspendLayout()
        CType(Me.picToolboxHGrdSplFldFallback, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.picToolboxHGrdEditorFallback, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'GroupBox9
        '
        Me.GroupBox9.Controls.Add(Me.TabControlToolboxHGrd)
        Me.GroupBox9.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupBox9.Location = New System.Drawing.Point(0, 5)
        Me.GroupBox9.Name = "GroupBox9"
        Me.GroupBox9.Size = New System.Drawing.Size(360, 495)
        Me.GroupBox9.TabIndex = 3
        Me.GroupBox9.TabStop = False
        Me.GroupBox9.Text = "Hintergrundbilder und Farben"
        '
        'TabControlToolboxHGrd
        '
        Me.TabControlToolboxHGrd.Controls.Add(Me.TabPageAktSpiel)
        Me.TabControlToolboxHGrd.Controls.Add(Me.TabPageFallback)
        Me.TabControlToolboxHGrd.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TabControlToolboxHGrd.ItemSize = New System.Drawing.Size(100, 32)
        Me.TabControlToolboxHGrd.Location = New System.Drawing.Point(3, 16)
        Me.TabControlToolboxHGrd.Name = "TabControlToolboxHGrd"
        Me.TabControlToolboxHGrd.SelectedIndex = 0
        Me.TabControlToolboxHGrd.Size = New System.Drawing.Size(354, 476)
        Me.TabControlToolboxHGrd.TabIndex = 18
        '
        'TabPageAktSpiel
        '
        Me.TabPageAktSpiel.Controls.Add(Me.gbxAktSpiel)
        Me.TabPageAktSpiel.Location = New System.Drawing.Point(4, 36)
        Me.TabPageAktSpiel.Name = "TabPageAktSpiel"
        Me.TabPageAktSpiel.Padding = New System.Windows.Forms.Padding(10)
        Me.TabPageAktSpiel.Size = New System.Drawing.Size(346, 436)
        Me.TabPageAktSpiel.TabIndex = 0
        Me.TabPageAktSpiel.Text = "Aktuelles Spiel"
        Me.TabPageAktSpiel.UseVisualStyleBackColor = True
        '
        'gbxAktSpiel
        '
        Me.gbxAktSpiel.Controls.Add(Me.chkToolboxHGrdEditorShowFraming)
        Me.gbxAktSpiel.Controls.Add(Me.chkToolboxHGrdEditorUseSplFldEinstlg)
        Me.gbxAktSpiel.Controls.Add(Me.GroupBox3)
        Me.gbxAktSpiel.Controls.Add(Me.GroupBox4)
        Me.gbxAktSpiel.Controls.Add(Me.btnInfoAktSpiel)
        Me.gbxAktSpiel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.gbxAktSpiel.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.gbxAktSpiel.Location = New System.Drawing.Point(10, 10)
        Me.gbxAktSpiel.Name = "gbxAktSpiel"
        Me.gbxAktSpiel.Size = New System.Drawing.Size(326, 416)
        Me.gbxAktSpiel.TabIndex = 17
        Me.gbxAktSpiel.TabStop = False
        Me.gbxAktSpiel.Text = "Aktuell geladenes Spiel"
        '
        'chkToolboxHGrdEditorShowFraming
        '
        Me.chkToolboxHGrdEditorShowFraming.AutoSize = True
        Me.chkToolboxHGrdEditorShowFraming.Location = New System.Drawing.Point(196, 329)
        Me.chkToolboxHGrdEditorShowFraming.Name = "chkToolboxHGrdEditorShowFraming"
        Me.chkToolboxHGrdEditorShowFraming.Size = New System.Drawing.Size(123, 30)
        Me.chkToolboxHGrdEditorShowFraming.TabIndex = 6
        Me.chkToolboxHGrdEditorShowFraming.Text = "Spielfeldumrahmung" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "anzeigen (nur Editor)"
        Me.chkToolboxHGrdEditorShowFraming.UseVisualStyleBackColor = True
        '
        'chkToolboxHGrdEditorUseSplFldEinstlg
        '
        Me.chkToolboxHGrdEditorUseSplFldEinstlg.AutoSize = True
        Me.chkToolboxHGrdEditorUseSplFldEinstlg.Location = New System.Drawing.Point(196, 280)
        Me.chkToolboxHGrdEditorUseSplFldEinstlg.Name = "chkToolboxHGrdEditorUseSplFldEinstlg"
        Me.chkToolboxHGrdEditorUseSplFldEinstlg.Size = New System.Drawing.Size(83, 43)
        Me.chkToolboxHGrdEditorUseSplFldEinstlg.TabIndex = 5
        Me.chkToolboxHGrdEditorUseSplFldEinstlg.Text = "Editor nutzt " & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Spielfeldein-" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "stellungen"
        Me.chkToolboxHGrdEditorUseSplFldEinstlg.UseVisualStyleBackColor = True
        '
        'GroupBox3
        '
        Me.GroupBox3.Controls.Add(Me.ButtonTooltip1)
        Me.GroupBox3.Controls.Add(Me.lblToolboxHGrdEditorColor)
        Me.GroupBox3.Controls.Add(Me.lblToolboxHGrdSplFldColor)
        Me.GroupBox3.Controls.Add(Me.btnToolboxHGrdEditorColorClear)
        Me.GroupBox3.Controls.Add(Me.btnToolboxHGrdSplFldColorClear)
        Me.GroupBox3.Controls.Add(Me.btnToolboxHGrdEditorColor)
        Me.GroupBox3.Controls.Add(Me.btnToolboxHGrdSplFldColor)
        Me.GroupBox3.Location = New System.Drawing.Point(191, 19)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Size = New System.Drawing.Size(129, 255)
        Me.GroupBox3.TabIndex = 4
        Me.GroupBox3.TabStop = False
        Me.GroupBox3.Text = "Farben"
        '
        'lblToolboxHGrdEditorColor
        '
        Me.lblToolboxHGrdEditorColor.BackColor = System.Drawing.Color.Transparent
        Me.lblToolboxHGrdEditorColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblToolboxHGrdEditorColor.Cursor = System.Windows.Forms.Cursors.Default
        Me.lblToolboxHGrdEditorColor.Location = New System.Drawing.Point(7, 212)
        Me.lblToolboxHGrdEditorColor.Name = "lblToolboxHGrdEditorColor"
        Me.lblToolboxHGrdEditorColor.Size = New System.Drawing.Size(114, 35)
        Me.lblToolboxHGrdEditorColor.TabIndex = 11
        Me.lblToolboxHGrdEditorColor.Text = "(keine Farbe gewählt)"
        Me.lblToolboxHGrdEditorColor.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblToolboxHGrdSplFldColor
        '
        Me.lblToolboxHGrdSplFldColor.BackColor = System.Drawing.Color.Transparent
        Me.lblToolboxHGrdSplFldColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblToolboxHGrdSplFldColor.Cursor = System.Windows.Forms.Cursors.Default
        Me.lblToolboxHGrdSplFldColor.Location = New System.Drawing.Point(6, 51)
        Me.lblToolboxHGrdSplFldColor.Name = "lblToolboxHGrdSplFldColor"
        Me.lblToolboxHGrdSplFldColor.Size = New System.Drawing.Size(114, 35)
        Me.lblToolboxHGrdSplFldColor.TabIndex = 10
        Me.lblToolboxHGrdSplFldColor.Text = "(keine Farbe gewählt)"
        Me.lblToolboxHGrdSplFldColor.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'btnToolboxHGrdEditorColorClear
        '
        Me.btnToolboxHGrdEditorColorClear.Location = New System.Drawing.Point(77, 183)
        Me.btnToolboxHGrdEditorColorClear.Name = "btnToolboxHGrdEditorColorClear"
        Me.btnToolboxHGrdEditorColorClear.Size = New System.Drawing.Size(43, 23)
        Me.btnToolboxHGrdEditorColorClear.TabIndex = 9
        Me.btnToolboxHGrdEditorColorClear.Text = "Clear"
        Me.btnToolboxHGrdEditorColorClear.UseVisualStyleBackColor = True
        '
        'btnToolboxHGrdSplFldColorClear
        '
        Me.btnToolboxHGrdSplFldColorClear.Location = New System.Drawing.Point(77, 21)
        Me.btnToolboxHGrdSplFldColorClear.Name = "btnToolboxHGrdSplFldColorClear"
        Me.btnToolboxHGrdSplFldColorClear.Size = New System.Drawing.Size(43, 23)
        Me.btnToolboxHGrdSplFldColorClear.TabIndex = 8
        Me.btnToolboxHGrdSplFldColorClear.Text = "Clear"
        Me.btnToolboxHGrdSplFldColorClear.UseVisualStyleBackColor = True
        '
        'btnToolboxHGrdEditorColor
        '
        Me.btnToolboxHGrdEditorColor.Location = New System.Drawing.Point(6, 183)
        Me.btnToolboxHGrdEditorColor.Name = "btnToolboxHGrdEditorColor"
        Me.btnToolboxHGrdEditorColor.Size = New System.Drawing.Size(69, 23)
        Me.btnToolboxHGrdEditorColor.TabIndex = 7
        Me.btnToolboxHGrdEditorColor.Text = "Editor"
        Me.btnToolboxHGrdEditorColor.UseVisualStyleBackColor = True
        '
        'btnToolboxHGrdSplFldColor
        '
        Me.btnToolboxHGrdSplFldColor.Location = New System.Drawing.Point(5, 21)
        Me.btnToolboxHGrdSplFldColor.Name = "btnToolboxHGrdSplFldColor"
        Me.btnToolboxHGrdSplFldColor.Size = New System.Drawing.Size(69, 23)
        Me.btnToolboxHGrdSplFldColor.TabIndex = 6
        Me.btnToolboxHGrdSplFldColor.Text = "Spielfeld"
        Me.btnToolboxHGrdSplFldColor.UseVisualStyleBackColor = True
        '
        'GroupBox4
        '
        Me.GroupBox4.Controls.Add(Me.lblToolboxHGrdEditorRenderMode)
        Me.GroupBox4.Controls.Add(Me.lblToolboxHGrdSplFldRenderMode)
        Me.GroupBox4.Controls.Add(Me.picToolboxHGrdEditor)
        Me.GroupBox4.Controls.Add(Me.picToolboxHGrdSplFld)
        Me.GroupBox4.Controls.Add(Me.btnToolboxHGrdEditorBitmapClear)
        Me.GroupBox4.Controls.Add(Me.btnToolboxHGrdSplFldBitmapClear)
        Me.GroupBox4.Controls.Add(Me.btnToolboxHGrdEditorBitmapName)
        Me.GroupBox4.Controls.Add(Me.btnToolboxHGrdSplFldBitmapName)
        Me.GroupBox4.Location = New System.Drawing.Point(8, 19)
        Me.GroupBox4.Name = "GroupBox4"
        Me.GroupBox4.Size = New System.Drawing.Size(177, 344)
        Me.GroupBox4.TabIndex = 3
        Me.GroupBox4.TabStop = False
        Me.GroupBox4.Text = "Bilder"
        '
        'lblToolboxHGrdEditorRenderMode
        '
        Me.lblToolboxHGrdEditorRenderMode.AutoSize = True
        Me.lblToolboxHGrdEditorRenderMode.Location = New System.Drawing.Point(15, 317)
        Me.lblToolboxHGrdEditorRenderMode.Name = "lblToolboxHGrdEditorRenderMode"
        Me.lblToolboxHGrdEditorRenderMode.Size = New System.Drawing.Size(169, 13)
        Me.lblToolboxHGrdEditorRenderMode.TabIndex = 9
        Me.lblToolboxHGrdEditorRenderMode.Text = "lblToolboxHGrdEditorRenderMode"
        '
        'lblToolboxHGrdSplFldRenderMode
        '
        Me.lblToolboxHGrdSplFldRenderMode.AutoSize = True
        Me.lblToolboxHGrdSplFldRenderMode.Location = New System.Drawing.Point(15, 157)
        Me.lblToolboxHGrdSplFldRenderMode.Name = "lblToolboxHGrdSplFldRenderMode"
        Me.lblToolboxHGrdSplFldRenderMode.Size = New System.Drawing.Size(171, 13)
        Me.lblToolboxHGrdSplFldRenderMode.TabIndex = 8
        Me.lblToolboxHGrdSplFldRenderMode.Text = "lblToolboxHGrdSplFldRenderMode"
        '
        'picToolboxHGrdEditor
        '
        Me.picToolboxHGrdEditor.BackColor = System.Drawing.SystemColors.ControlLight
        Me.picToolboxHGrdEditor.Location = New System.Drawing.Point(18, 212)
        Me.picToolboxHGrdEditor.Name = "picToolboxHGrdEditor"
        Me.picToolboxHGrdEditor.Size = New System.Drawing.Size(150, 102)
        Me.picToolboxHGrdEditor.TabIndex = 7
        Me.picToolboxHGrdEditor.TabStop = False
        '
        'picToolboxHGrdSplFld
        '
        Me.picToolboxHGrdSplFld.BackColor = System.Drawing.SystemColors.ControlLight
        Me.picToolboxHGrdSplFld.Location = New System.Drawing.Point(15, 51)
        Me.picToolboxHGrdSplFld.Name = "picToolboxHGrdSplFld"
        Me.picToolboxHGrdSplFld.Size = New System.Drawing.Size(150, 103)
        Me.picToolboxHGrdSplFld.TabIndex = 6
        Me.picToolboxHGrdSplFld.TabStop = False
        '
        'btnToolboxHGrdEditorBitmapClear
        '
        Me.btnToolboxHGrdEditorBitmapClear.Location = New System.Drawing.Point(128, 183)
        Me.btnToolboxHGrdEditorBitmapClear.Name = "btnToolboxHGrdEditorBitmapClear"
        Me.btnToolboxHGrdEditorBitmapClear.Size = New System.Drawing.Size(40, 23)
        Me.btnToolboxHGrdEditorBitmapClear.TabIndex = 5
        Me.btnToolboxHGrdEditorBitmapClear.Text = "Clear"
        Me.btnToolboxHGrdEditorBitmapClear.UseVisualStyleBackColor = True
        '
        'btnToolboxHGrdSplFldBitmapClear
        '
        Me.btnToolboxHGrdSplFldBitmapClear.Location = New System.Drawing.Point(122, 21)
        Me.btnToolboxHGrdSplFldBitmapClear.Name = "btnToolboxHGrdSplFldBitmapClear"
        Me.btnToolboxHGrdSplFldBitmapClear.Size = New System.Drawing.Size(43, 23)
        Me.btnToolboxHGrdSplFldBitmapClear.TabIndex = 4
        Me.btnToolboxHGrdSplFldBitmapClear.Text = "Clear"
        Me.btnToolboxHGrdSplFldBitmapClear.UseVisualStyleBackColor = True
        '
        'btnToolboxHGrdEditorBitmapName
        '
        Me.btnToolboxHGrdEditorBitmapName.Location = New System.Drawing.Point(15, 183)
        Me.btnToolboxHGrdEditorBitmapName.Name = "btnToolboxHGrdEditorBitmapName"
        Me.btnToolboxHGrdEditorBitmapName.Size = New System.Drawing.Size(107, 23)
        Me.btnToolboxHGrdEditorBitmapName.TabIndex = 3
        Me.btnToolboxHGrdEditorBitmapName.Text = "Editor"
        Me.btnToolboxHGrdEditorBitmapName.UseVisualStyleBackColor = True
        '
        'btnToolboxHGrdSplFldBitmapName
        '
        Me.btnToolboxHGrdSplFldBitmapName.Location = New System.Drawing.Point(15, 21)
        Me.btnToolboxHGrdSplFldBitmapName.Name = "btnToolboxHGrdSplFldBitmapName"
        Me.btnToolboxHGrdSplFldBitmapName.Size = New System.Drawing.Size(104, 23)
        Me.btnToolboxHGrdSplFldBitmapName.TabIndex = 2
        Me.btnToolboxHGrdSplFldBitmapName.Text = "Spielfeld"
        Me.btnToolboxHGrdSplFldBitmapName.UseVisualStyleBackColor = True
        '
        'TabPageFallback
        '
        Me.TabPageFallback.Controls.Add(Me.gbxFallback)
        Me.TabPageFallback.Location = New System.Drawing.Point(4, 36)
        Me.TabPageFallback.Name = "TabPageFallback"
        Me.TabPageFallback.Padding = New System.Windows.Forms.Padding(10)
        Me.TabPageFallback.Size = New System.Drawing.Size(346, 436)
        Me.TabPageFallback.TabIndex = 1
        Me.TabPageFallback.Text = "Fallback"
        Me.TabPageFallback.UseVisualStyleBackColor = True
        '
        'gbxFallback
        '
        Me.gbxFallback.Controls.Add(Me.chkToolboxHGrdEditorUseSplFldEinstlgFallback)
        Me.gbxFallback.Controls.Add(Me.GroupBox2)
        Me.gbxFallback.Controls.Add(Me.GroupBox5)
        Me.gbxFallback.Controls.Add(Me.ButtonInfo1)
        Me.gbxFallback.Dock = System.Windows.Forms.DockStyle.Fill
        Me.gbxFallback.Location = New System.Drawing.Point(10, 10)
        Me.gbxFallback.Name = "gbxFallback"
        Me.gbxFallback.Size = New System.Drawing.Size(326, 416)
        Me.gbxFallback.TabIndex = 18
        Me.gbxFallback.TabStop = False
        Me.gbxFallback.Text = "Fallback (Wenn das aktuelles Spiel keine Werte hat)"
        '
        'chkToolboxHGrdEditorUseSplFldEinstlgFallback
        '
        Me.chkToolboxHGrdEditorUseSplFldEinstlgFallback.AutoSize = True
        Me.chkToolboxHGrdEditorUseSplFldEinstlgFallback.Location = New System.Drawing.Point(196, 280)
        Me.chkToolboxHGrdEditorUseSplFldEinstlgFallback.Name = "chkToolboxHGrdEditorUseSplFldEinstlgFallback"
        Me.chkToolboxHGrdEditorUseSplFldEinstlgFallback.Size = New System.Drawing.Size(83, 43)
        Me.chkToolboxHGrdEditorUseSplFldEinstlgFallback.TabIndex = 5
        Me.chkToolboxHGrdEditorUseSplFldEinstlgFallback.Text = "Editor nutzt" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Spielfeldein-" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "stellungen"
        Me.chkToolboxHGrdEditorUseSplFldEinstlgFallback.UseVisualStyleBackColor = True
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.ButtonTooltip2)
        Me.GroupBox2.Controls.Add(Me.lblToolboxHGrdEditorColorFallback)
        Me.GroupBox2.Controls.Add(Me.lblToolboxHGrdSplFldColorFallback)
        Me.GroupBox2.Controls.Add(Me.btnToolboxHGrdEditorColorClearFallback)
        Me.GroupBox2.Controls.Add(Me.btnToolboxHGrdSplFldColorClearFallback)
        Me.GroupBox2.Controls.Add(Me.btnToolboxHGrdEditorColorFallback)
        Me.GroupBox2.Controls.Add(Me.btnToolboxHGrdSplFldColorFallback)
        Me.GroupBox2.Location = New System.Drawing.Point(191, 19)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(129, 255)
        Me.GroupBox2.TabIndex = 4
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Farben"
        '
        'lblToolboxHGrdEditorColorFallback
        '
        Me.lblToolboxHGrdEditorColorFallback.BackColor = System.Drawing.Color.Transparent
        Me.lblToolboxHGrdEditorColorFallback.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblToolboxHGrdEditorColorFallback.Cursor = System.Windows.Forms.Cursors.Default
        Me.lblToolboxHGrdEditorColorFallback.Location = New System.Drawing.Point(7, 212)
        Me.lblToolboxHGrdEditorColorFallback.Name = "lblToolboxHGrdEditorColorFallback"
        Me.lblToolboxHGrdEditorColorFallback.Size = New System.Drawing.Size(113, 35)
        Me.lblToolboxHGrdEditorColorFallback.TabIndex = 11
        Me.lblToolboxHGrdEditorColorFallback.Text = "(keine Farbe gewählt)"
        Me.lblToolboxHGrdEditorColorFallback.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblToolboxHGrdSplFldColorFallback
        '
        Me.lblToolboxHGrdSplFldColorFallback.BackColor = System.Drawing.Color.Transparent
        Me.lblToolboxHGrdSplFldColorFallback.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblToolboxHGrdSplFldColorFallback.Cursor = System.Windows.Forms.Cursors.Default
        Me.lblToolboxHGrdSplFldColorFallback.Location = New System.Drawing.Point(6, 51)
        Me.lblToolboxHGrdSplFldColorFallback.Name = "lblToolboxHGrdSplFldColorFallback"
        Me.lblToolboxHGrdSplFldColorFallback.Size = New System.Drawing.Size(114, 35)
        Me.lblToolboxHGrdSplFldColorFallback.TabIndex = 10
        Me.lblToolboxHGrdSplFldColorFallback.Text = "(keine Farbe gewählt)"
        Me.lblToolboxHGrdSplFldColorFallback.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'btnToolboxHGrdEditorColorClearFallback
        '
        Me.btnToolboxHGrdEditorColorClearFallback.Location = New System.Drawing.Point(79, 183)
        Me.btnToolboxHGrdEditorColorClearFallback.Name = "btnToolboxHGrdEditorColorClearFallback"
        Me.btnToolboxHGrdEditorColorClearFallback.Size = New System.Drawing.Size(40, 23)
        Me.btnToolboxHGrdEditorColorClearFallback.TabIndex = 9
        Me.btnToolboxHGrdEditorColorClearFallback.Text = "Clear"
        Me.btnToolboxHGrdEditorColorClearFallback.UseVisualStyleBackColor = True
        '
        'btnToolboxHGrdSplFldColorClearFallback
        '
        Me.btnToolboxHGrdSplFldColorClearFallback.Location = New System.Drawing.Point(77, 21)
        Me.btnToolboxHGrdSplFldColorClearFallback.Name = "btnToolboxHGrdSplFldColorClearFallback"
        Me.btnToolboxHGrdSplFldColorClearFallback.Size = New System.Drawing.Size(43, 23)
        Me.btnToolboxHGrdSplFldColorClearFallback.TabIndex = 8
        Me.btnToolboxHGrdSplFldColorClearFallback.Text = "Clear"
        Me.btnToolboxHGrdSplFldColorClearFallback.UseVisualStyleBackColor = True
        '
        'btnToolboxHGrdEditorColorFallback
        '
        Me.btnToolboxHGrdEditorColorFallback.Location = New System.Drawing.Point(6, 183)
        Me.btnToolboxHGrdEditorColorFallback.Name = "btnToolboxHGrdEditorColorFallback"
        Me.btnToolboxHGrdEditorColorFallback.Size = New System.Drawing.Size(69, 23)
        Me.btnToolboxHGrdEditorColorFallback.TabIndex = 7
        Me.btnToolboxHGrdEditorColorFallback.Text = "Editor"
        Me.btnToolboxHGrdEditorColorFallback.UseVisualStyleBackColor = True
        '
        'btnToolboxHGrdSplFldColorFallback
        '
        Me.btnToolboxHGrdSplFldColorFallback.Location = New System.Drawing.Point(5, 21)
        Me.btnToolboxHGrdSplFldColorFallback.Name = "btnToolboxHGrdSplFldColorFallback"
        Me.btnToolboxHGrdSplFldColorFallback.Size = New System.Drawing.Size(69, 23)
        Me.btnToolboxHGrdSplFldColorFallback.TabIndex = 6
        Me.btnToolboxHGrdSplFldColorFallback.Text = "Spielfeld"
        Me.btnToolboxHGrdSplFldColorFallback.UseVisualStyleBackColor = True
        '
        'GroupBox5
        '
        Me.GroupBox5.Controls.Add(Me.picToolboxHGrdSplFldFallback)
        Me.GroupBox5.Controls.Add(Me.lblToolboxHGrdEditorRenderModeFallback)
        Me.GroupBox5.Controls.Add(Me.lblToolboxHGrdSplFldRenderModeFallback)
        Me.GroupBox5.Controls.Add(Me.picToolboxHGrdEditorFallback)
        Me.GroupBox5.Controls.Add(Me.btnToolboxHGrdEditorBitmapClearFallback)
        Me.GroupBox5.Controls.Add(Me.btnToolboxHGrdSplFldBitmapClearFallback)
        Me.GroupBox5.Controls.Add(Me.btnToolboxHGrdEditorBitmapNameFallback)
        Me.GroupBox5.Controls.Add(Me.btnToolboxHGrdSplFldBitmapNameFallback)
        Me.GroupBox5.Location = New System.Drawing.Point(8, 19)
        Me.GroupBox5.Name = "GroupBox5"
        Me.GroupBox5.Size = New System.Drawing.Size(177, 344)
        Me.GroupBox5.TabIndex = 3
        Me.GroupBox5.TabStop = False
        Me.GroupBox5.Text = "Bilder"
        '
        'picToolboxHGrdSplFldFallback
        '
        Me.picToolboxHGrdSplFldFallback.BackColor = System.Drawing.SystemColors.ControlLight
        Me.picToolboxHGrdSplFldFallback.Location = New System.Drawing.Point(15, 51)
        Me.picToolboxHGrdSplFldFallback.Name = "picToolboxHGrdSplFldFallback"
        Me.picToolboxHGrdSplFldFallback.Size = New System.Drawing.Size(150, 102)
        Me.picToolboxHGrdSplFldFallback.TabIndex = 6
        Me.picToolboxHGrdSplFldFallback.TabStop = False
        '
        'lblToolboxHGrdEditorRenderModeFallback
        '
        Me.lblToolboxHGrdEditorRenderModeFallback.AutoSize = True
        Me.lblToolboxHGrdEditorRenderModeFallback.Location = New System.Drawing.Point(15, 317)
        Me.lblToolboxHGrdEditorRenderModeFallback.Name = "lblToolboxHGrdEditorRenderModeFallback"
        Me.lblToolboxHGrdEditorRenderModeFallback.Size = New System.Drawing.Size(209, 13)
        Me.lblToolboxHGrdEditorRenderModeFallback.TabIndex = 8
        Me.lblToolboxHGrdEditorRenderModeFallback.Text = "lblToolboxHGrdEditorRenderModeFallback"
        '
        'lblToolboxHGrdSplFldRenderModeFallback
        '
        Me.lblToolboxHGrdSplFldRenderModeFallback.AutoSize = True
        Me.lblToolboxHGrdSplFldRenderModeFallback.Location = New System.Drawing.Point(15, 157)
        Me.lblToolboxHGrdSplFldRenderModeFallback.Name = "lblToolboxHGrdSplFldRenderModeFallback"
        Me.lblToolboxHGrdSplFldRenderModeFallback.Size = New System.Drawing.Size(211, 13)
        Me.lblToolboxHGrdSplFldRenderModeFallback.TabIndex = 6
        Me.lblToolboxHGrdSplFldRenderModeFallback.Text = "lblToolboxHGrdSplFldRenderModeFallback"
        '
        'picToolboxHGrdEditorFallback
        '
        Me.picToolboxHGrdEditorFallback.BackColor = System.Drawing.SystemColors.ControlLight
        Me.picToolboxHGrdEditorFallback.Location = New System.Drawing.Point(18, 212)
        Me.picToolboxHGrdEditorFallback.Name = "picToolboxHGrdEditorFallback"
        Me.picToolboxHGrdEditorFallback.Size = New System.Drawing.Size(150, 102)
        Me.picToolboxHGrdEditorFallback.TabIndex = 7
        Me.picToolboxHGrdEditorFallback.TabStop = False
        '
        'btnToolboxHGrdEditorBitmapClearFallback
        '
        Me.btnToolboxHGrdEditorBitmapClearFallback.Location = New System.Drawing.Point(128, 183)
        Me.btnToolboxHGrdEditorBitmapClearFallback.Name = "btnToolboxHGrdEditorBitmapClearFallback"
        Me.btnToolboxHGrdEditorBitmapClearFallback.Size = New System.Drawing.Size(40, 23)
        Me.btnToolboxHGrdEditorBitmapClearFallback.TabIndex = 5
        Me.btnToolboxHGrdEditorBitmapClearFallback.Text = "Clear"
        Me.btnToolboxHGrdEditorBitmapClearFallback.UseVisualStyleBackColor = True
        '
        'btnToolboxHGrdSplFldBitmapClearFallback
        '
        Me.btnToolboxHGrdSplFldBitmapClearFallback.Location = New System.Drawing.Point(122, 21)
        Me.btnToolboxHGrdSplFldBitmapClearFallback.Name = "btnToolboxHGrdSplFldBitmapClearFallback"
        Me.btnToolboxHGrdSplFldBitmapClearFallback.Size = New System.Drawing.Size(43, 23)
        Me.btnToolboxHGrdSplFldBitmapClearFallback.TabIndex = 4
        Me.btnToolboxHGrdSplFldBitmapClearFallback.Text = "Clear"
        Me.btnToolboxHGrdSplFldBitmapClearFallback.UseVisualStyleBackColor = True
        '
        'btnToolboxHGrdEditorBitmapNameFallback
        '
        Me.btnToolboxHGrdEditorBitmapNameFallback.Location = New System.Drawing.Point(15, 183)
        Me.btnToolboxHGrdEditorBitmapNameFallback.Name = "btnToolboxHGrdEditorBitmapNameFallback"
        Me.btnToolboxHGrdEditorBitmapNameFallback.Size = New System.Drawing.Size(107, 23)
        Me.btnToolboxHGrdEditorBitmapNameFallback.TabIndex = 3
        Me.btnToolboxHGrdEditorBitmapNameFallback.Text = "Editor"
        Me.btnToolboxHGrdEditorBitmapNameFallback.UseVisualStyleBackColor = True
        '
        'btnToolboxHGrdSplFldBitmapNameFallback
        '
        Me.btnToolboxHGrdSplFldBitmapNameFallback.Location = New System.Drawing.Point(15, 21)
        Me.btnToolboxHGrdSplFldBitmapNameFallback.Name = "btnToolboxHGrdSplFldBitmapNameFallback"
        Me.btnToolboxHGrdSplFldBitmapNameFallback.Size = New System.Drawing.Size(104, 23)
        Me.btnToolboxHGrdSplFldBitmapNameFallback.TabIndex = 2
        Me.btnToolboxHGrdSplFldBitmapNameFallback.Text = "Spielfeld"
        Me.btnToolboxHGrdSplFldBitmapNameFallback.UseVisualStyleBackColor = True
        '
        'btnInfoAktSpiel
        '
        Me.btnInfoAktSpiel.AccessibleDescription = "Zeigt Informationen."
        Me.btnInfoAktSpiel.AccessibleName = "Info"
        Me.btnInfoAktSpiel.AutoSquare = True
        Me.btnInfoAktSpiel.BackColor = System.Drawing.SystemColors.Control
        Me.btnInfoAktSpiel.Cursor = System.Windows.Forms.Cursors.Hand
        Me.btnInfoAktSpiel.DarkMode = True
        Me.btnInfoAktSpiel.InfoHeader = "Info"
        Me.btnInfoAktSpiel.InfoText = resources.GetString("btnInfoAktSpiel.InfoText")
        Me.btnInfoAktSpiel.Location = New System.Drawing.Point(280, 289)
        Me.btnInfoAktSpiel.MinimumSize = New System.Drawing.Size(16, 16)
        Me.btnInfoAktSpiel.Name = "btnInfoAktSpiel"
        Me.btnInfoAktSpiel.Size = New System.Drawing.Size(26, 26)
        Me.btnInfoAktSpiel.TabIndex = 2
        Me.btnInfoAktSpiel.TabStop = False
        Me.btnInfoAktSpiel.Text = "ButtonInfo2"
        '
        'ButtonInfo1
        '
        Me.ButtonInfo1.AccessibleDescription = "Zeigt Informationen."
        Me.ButtonInfo1.AccessibleName = "Info"
        Me.ButtonInfo1.AutoSquare = True
        Me.ButtonInfo1.BackColor = System.Drawing.SystemColors.Control
        Me.ButtonInfo1.Cursor = System.Windows.Forms.Cursors.Hand
        Me.ButtonInfo1.DarkMode = True
        Me.ButtonInfo1.InfoHeader = "Info"
        Me.ButtonInfo1.InfoText = "Generelle Regel:" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Wenn Farben UND Bilder hinterlegt sind, gelten die Bilder."
        Me.ButtonInfo1.Location = New System.Drawing.Point(280, 289)
        Me.ButtonInfo1.MinimumSize = New System.Drawing.Size(16, 16)
        Me.ButtonInfo1.Name = "ButtonInfo1"
        Me.ButtonInfo1.Size = New System.Drawing.Size(26, 26)
        Me.ButtonInfo1.TabIndex = 2
        Me.ButtonInfo1.TabStop = False
        Me.ButtonInfo1.Text = "ButtonInfo2"
        '
        'ButtonTooltip1
        '
        Me.ButtonTooltip1.AccessibleDescription = "Zeigt einen Hinweis-Tooltip."
        Me.ButtonTooltip1.AccessibleName = "Info"
        Me.ButtonTooltip1.BackColor = System.Drawing.SystemColors.Control
        Me.ButtonTooltip1.Cursor = System.Windows.Forms.Cursors.Default
        Me.ButtonTooltip1.DarkMode = False
        Me.ButtonTooltip1.InfoHeader = "Info"
        Me.ButtonTooltip1.InfoText = "Gedrückte Strg-Taste = benannte Farben"
        Me.ButtonTooltip1.Location = New System.Drawing.Point(89, 93)
        Me.ButtonTooltip1.MinimumSize = New System.Drawing.Size(16, 16)
        Me.ButtonTooltip1.Name = "ButtonTooltip1"
        Me.ButtonTooltip1.ShowOnFocus = False
        Me.ButtonTooltip1.Size = New System.Drawing.Size(26, 26)
        Me.ButtonTooltip1.TabIndex = 12
        Me.ButtonTooltip1.TabStop = False
        Me.ButtonTooltip1.Text = "ButtonTooltip1"
        '
        'ButtonTooltip2
        '
        Me.ButtonTooltip2.AccessibleDescription = "Zeigt einen Hinweis-Tooltip."
        Me.ButtonTooltip2.AccessibleName = "Info"
        Me.ButtonTooltip2.BackColor = System.Drawing.SystemColors.Control
        Me.ButtonTooltip2.Cursor = System.Windows.Forms.Cursors.Default
        Me.ButtonTooltip2.DarkMode = False
        Me.ButtonTooltip2.InfoHeader = "Info"
        Me.ButtonTooltip2.InfoText = "Gedrückte Strg-Taste = benannte Farben"
        Me.ButtonTooltip2.Location = New System.Drawing.Point(89, 93)
        Me.ButtonTooltip2.MinimumSize = New System.Drawing.Size(16, 16)
        Me.ButtonTooltip2.Name = "ButtonTooltip2"
        Me.ButtonTooltip2.ShowOnFocus = False
        Me.ButtonTooltip2.Size = New System.Drawing.Size(26, 26)
        Me.ButtonTooltip2.TabIndex = 13
        Me.ButtonTooltip2.TabStop = False
        Me.ButtonTooltip2.Text = "ButtonTooltip2"
        '
        'UctlToolboxHintergrund
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.GroupBox9)
        Me.Name = "UctlToolboxHintergrund"
        Me.Padding = New System.Windows.Forms.Padding(0, 5, 0, 0)
        Me.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Size = New System.Drawing.Size(360, 500)
        Me.GroupBox9.ResumeLayout(False)
        Me.TabControlToolboxHGrd.ResumeLayout(False)
        Me.TabPageAktSpiel.ResumeLayout(False)
        Me.gbxAktSpiel.ResumeLayout(False)
        Me.gbxAktSpiel.PerformLayout()
        Me.GroupBox3.ResumeLayout(False)
        Me.GroupBox4.ResumeLayout(False)
        Me.GroupBox4.PerformLayout()
        CType(Me.picToolboxHGrdEditor, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.picToolboxHGrdSplFld, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TabPageFallback.ResumeLayout(False)
        Me.gbxFallback.ResumeLayout(False)
        Me.gbxFallback.PerformLayout()
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox5.ResumeLayout(False)
        Me.GroupBox5.PerformLayout()
        CType(Me.picToolboxHGrdSplFldFallback, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.picToolboxHGrdEditorFallback, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents lblToolboxHGrdSplFldColor As System.Windows.Forms.Label
    Friend WithEvents lblToolboxHGrdEditorColorFallback As System.Windows.Forms.Label
    Friend WithEvents lblToolboxHGrdSplFldColorFallback As System.Windows.Forms.Label
    Friend WithEvents GroupBox9 As GroupBox
    Friend WithEvents gbxAktSpiel As GroupBox
    Friend WithEvents chkToolboxHGrdEditorUseSplFldEinstlg As CheckBox
    Friend WithEvents GroupBox3 As GroupBox
    Friend WithEvents btnToolboxHGrdEditorColorClear As Button
    Friend WithEvents btnToolboxHGrdSplFldColorClear As Button
    Friend WithEvents btnToolboxHGrdEditorColor As Button
    Friend WithEvents btnToolboxHGrdSplFldColor As Button
    Friend WithEvents GroupBox4 As GroupBox
    Friend WithEvents btnToolboxHGrdEditorBitmapClear As Button
    Friend WithEvents btnToolboxHGrdSplFldBitmapClear As Button
    Friend WithEvents btnToolboxHGrdEditorBitmapName As Button
    Friend WithEvents btnToolboxHGrdSplFldBitmapName As Button
    Friend WithEvents btnInfoAktSpiel As ButtonInfo
    Friend WithEvents TabControlToolboxHGrd As TabControl
    Friend WithEvents TabPageAktSpiel As TabPage
    Friend WithEvents TabPageFallback As TabPage
    Friend WithEvents picToolboxHGrdSplFld As PictureBox
    Friend WithEvents picToolboxHGrdEditor As PictureBox
    Friend WithEvents gbxFallback As GroupBox
    Friend WithEvents chkToolboxHGrdEditorUseSplFldEinstlgFallback As CheckBox
    Friend WithEvents GroupBox2 As GroupBox
    Friend WithEvents btnToolboxHGrdEditorColorClearFallback As Button
    Friend WithEvents btnToolboxHGrdSplFldColorClearFallback As Button
    Friend WithEvents btnToolboxHGrdEditorColorFallback As Button
    Friend WithEvents btnToolboxHGrdSplFldColorFallback As Button
    Friend WithEvents GroupBox5 As GroupBox
    Friend WithEvents picToolboxHGrdEditorFallback As PictureBox
    Friend WithEvents picToolboxHGrdSplFldFallback As PictureBox
    Friend WithEvents btnToolboxHGrdEditorBitmapClearFallback As Button
    Friend WithEvents btnToolboxHGrdSplFldBitmapClearFallback As Button
    Friend WithEvents btnToolboxHGrdEditorBitmapNameFallback As Button
    Friend WithEvents btnToolboxHGrdSplFldBitmapNameFallback As Button
    Friend WithEvents ButtonInfo1 As ButtonInfo
    Friend WithEvents lblToolboxHGrdSplFldRenderModeFallback As Label
    Friend WithEvents lblToolboxHGrdEditorRenderMode As Label
    Friend WithEvents lblToolboxHGrdSplFldRenderMode As Label
    Friend WithEvents lblToolboxHGrdEditorRenderModeFallback As Label
    Friend WithEvents lblToolboxHGrdEditorColor As Label
    Friend WithEvents chkToolboxHGrdEditorShowFraming As CheckBox
    Friend WithEvents ButtonTooltip1 As ButtonTooltip
    Friend WithEvents ButtonTooltip2 As ButtonTooltip
End Class
