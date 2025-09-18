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
        Me.GroupBox9 = New System.Windows.Forms.GroupBox()
        Me.TabControlToolboxHGrd = New System.Windows.Forms.TabControl()
        Me.TabPageAktSpiel = New System.Windows.Forms.TabPage()
        Me.gbxAktSpiel = New System.Windows.Forms.GroupBox()
        Me.chkToolboxHGrdEditorUseSpfldEinstlg = New System.Windows.Forms.CheckBox()
        Me.GroupBox3 = New System.Windows.Forms.GroupBox()
        Me.lblToolboxHGrdEditorColor = New System.Windows.Forms.Label()
        Me.lblToolboxHGrdSpfldColor = New System.Windows.Forms.Label()
        Me.btnToolboxHGrdEditorColorClear = New System.Windows.Forms.Button()
        Me.btnToolboxHGrdSpfldColorClear = New System.Windows.Forms.Button()
        Me.btnToolboxHGrdEditorColor = New System.Windows.Forms.Button()
        Me.btnToolboxHGrdSpfldColor = New System.Windows.Forms.Button()
        Me.GroupBox4 = New System.Windows.Forms.GroupBox()
        Me.lblToolboxHGrdEditorRenderMode = New System.Windows.Forms.Label()
        Me.lblToolboxHGrdSpfldRenderMode = New System.Windows.Forms.Label()
        Me.picToolboxHGrdEditor = New System.Windows.Forms.PictureBox()
        Me.picToolboxHGrdSpfld = New System.Windows.Forms.PictureBox()
        Me.btnToolboxHGrdEditorBitmapClear = New System.Windows.Forms.Button()
        Me.btnToolboxHGrdSpfldBitmapClear = New System.Windows.Forms.Button()
        Me.btnToolboxHGrdEditorBitmapName = New System.Windows.Forms.Button()
        Me.btnToolboxHGrdSpfldBitmapName = New System.Windows.Forms.Button()
        Me.btnInfoAktSpiel = New MahjongGK.ButtonInfo()
        Me.TabPageFallback = New System.Windows.Forms.TabPage()
        Me.gbxFallback = New System.Windows.Forms.GroupBox()
        Me.chkToolboxHGrdEditorUseSpfldEinstlgFallback = New System.Windows.Forms.CheckBox()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.lblToolboxHGrdEditorColorFallback = New System.Windows.Forms.Label()
        Me.lblToolboxHGrdSpfldColorFallback = New System.Windows.Forms.Label()
        Me.btnToolboxHGrdEditorColorClearFallback = New System.Windows.Forms.Button()
        Me.btnToolboxHGrdSpfldColorClearFallback = New System.Windows.Forms.Button()
        Me.btnToolboxHGrdEditorColorFallback = New System.Windows.Forms.Button()
        Me.btnToolboxHGrdSpfldColorFallback = New System.Windows.Forms.Button()
        Me.GroupBox5 = New System.Windows.Forms.GroupBox()
        Me.picToolboxHGrdSpfldFallback = New System.Windows.Forms.PictureBox()
        Me.lblToolboxHGrdEditorRenderModeFallback = New System.Windows.Forms.Label()
        Me.lblToolboxHGrdSplfldRenderModeFallback = New System.Windows.Forms.Label()
        Me.picToolboxHGrdEditorFallback = New System.Windows.Forms.PictureBox()
        Me.btnToolboxHGrdEditorBitmapClearFallback = New System.Windows.Forms.Button()
        Me.btnToolboxHGrdSpfldBitmapClearFallback = New System.Windows.Forms.Button()
        Me.btnToolboxHGrdEditorBitmapNameFallback = New System.Windows.Forms.Button()
        Me.btnToolboxHGrdSpfldBitmapNameFallback = New System.Windows.Forms.Button()
        Me.ButtonInfo1 = New MahjongGK.ButtonInfo()
        Me.GroupBox9.SuspendLayout()
        Me.TabControlToolboxHGrd.SuspendLayout()
        Me.TabPageAktSpiel.SuspendLayout()
        Me.gbxAktSpiel.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
        Me.GroupBox4.SuspendLayout()
        CType(Me.picToolboxHGrdEditor, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.picToolboxHGrdSpfld, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TabPageFallback.SuspendLayout()
        Me.gbxFallback.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.GroupBox5.SuspendLayout()
        CType(Me.picToolboxHGrdSpfldFallback, System.ComponentModel.ISupportInitialize).BeginInit()
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
        Me.gbxAktSpiel.Controls.Add(Me.chkToolboxHGrdEditorUseSpfldEinstlg)
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
        'chkToolboxHGrdEditorUseSpfldEinstlg
        '
        Me.chkToolboxHGrdEditorUseSpfldEinstlg.AutoSize = True
        Me.chkToolboxHGrdEditorUseSpfldEinstlg.Location = New System.Drawing.Point(195, 280)
        Me.chkToolboxHGrdEditorUseSpfldEinstlg.Name = "chkToolboxHGrdEditorUseSpfldEinstlg"
        Me.chkToolboxHGrdEditorUseSpfldEinstlg.Size = New System.Drawing.Size(83, 43)
        Me.chkToolboxHGrdEditorUseSpfldEinstlg.TabIndex = 5
        Me.chkToolboxHGrdEditorUseSpfldEinstlg.Text = "Editor nutzt " & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Spielfeldein-" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "stellungen"
        Me.chkToolboxHGrdEditorUseSpfldEinstlg.UseVisualStyleBackColor = True
        '
        'GroupBox3
        '
        Me.GroupBox3.Controls.Add(Me.lblToolboxHGrdEditorColor)
        Me.GroupBox3.Controls.Add(Me.lblToolboxHGrdSpfldColor)
        Me.GroupBox3.Controls.Add(Me.btnToolboxHGrdEditorColorClear)
        Me.GroupBox3.Controls.Add(Me.btnToolboxHGrdSpfldColorClear)
        Me.GroupBox3.Controls.Add(Me.btnToolboxHGrdEditorColor)
        Me.GroupBox3.Controls.Add(Me.btnToolboxHGrdSpfldColor)
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
        'lblToolboxHGrdSpfldColor
        '
        Me.lblToolboxHGrdSpfldColor.BackColor = System.Drawing.Color.Transparent
        Me.lblToolboxHGrdSpfldColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblToolboxHGrdSpfldColor.Cursor = System.Windows.Forms.Cursors.Default
        Me.lblToolboxHGrdSpfldColor.Location = New System.Drawing.Point(6, 51)
        Me.lblToolboxHGrdSpfldColor.Name = "lblToolboxHGrdSpfldColor"
        Me.lblToolboxHGrdSpfldColor.Size = New System.Drawing.Size(114, 35)
        Me.lblToolboxHGrdSpfldColor.TabIndex = 10
        Me.lblToolboxHGrdSpfldColor.Text = "(keine Farbe gewählt)"
        Me.lblToolboxHGrdSpfldColor.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
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
        'btnToolboxHGrdSpfldColorClear
        '
        Me.btnToolboxHGrdSpfldColorClear.Location = New System.Drawing.Point(77, 21)
        Me.btnToolboxHGrdSpfldColorClear.Name = "btnToolboxHGrdSpfldColorClear"
        Me.btnToolboxHGrdSpfldColorClear.Size = New System.Drawing.Size(43, 23)
        Me.btnToolboxHGrdSpfldColorClear.TabIndex = 8
        Me.btnToolboxHGrdSpfldColorClear.Text = "Clear"
        Me.btnToolboxHGrdSpfldColorClear.UseVisualStyleBackColor = True
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
        'btnToolboxHGrdSpfldColor
        '
        Me.btnToolboxHGrdSpfldColor.Location = New System.Drawing.Point(5, 21)
        Me.btnToolboxHGrdSpfldColor.Name = "btnToolboxHGrdSpfldColor"
        Me.btnToolboxHGrdSpfldColor.Size = New System.Drawing.Size(69, 23)
        Me.btnToolboxHGrdSpfldColor.TabIndex = 6
        Me.btnToolboxHGrdSpfldColor.Text = "Spielfeld"
        Me.btnToolboxHGrdSpfldColor.UseVisualStyleBackColor = True
        '
        'GroupBox4
        '
        Me.GroupBox4.Controls.Add(Me.lblToolboxHGrdEditorRenderMode)
        Me.GroupBox4.Controls.Add(Me.lblToolboxHGrdSpfldRenderMode)
        Me.GroupBox4.Controls.Add(Me.picToolboxHGrdEditor)
        Me.GroupBox4.Controls.Add(Me.picToolboxHGrdSpfld)
        Me.GroupBox4.Controls.Add(Me.btnToolboxHGrdEditorBitmapClear)
        Me.GroupBox4.Controls.Add(Me.btnToolboxHGrdSpfldBitmapClear)
        Me.GroupBox4.Controls.Add(Me.btnToolboxHGrdEditorBitmapName)
        Me.GroupBox4.Controls.Add(Me.btnToolboxHGrdSpfldBitmapName)
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
        'lblToolboxHGrdSpfldRenderMode
        '
        Me.lblToolboxHGrdSpfldRenderMode.AutoSize = True
        Me.lblToolboxHGrdSpfldRenderMode.Location = New System.Drawing.Point(15, 157)
        Me.lblToolboxHGrdSpfldRenderMode.Name = "lblToolboxHGrdSpfldRenderMode"
        Me.lblToolboxHGrdSpfldRenderMode.Size = New System.Drawing.Size(166, 13)
        Me.lblToolboxHGrdSpfldRenderMode.TabIndex = 8
        Me.lblToolboxHGrdSpfldRenderMode.Text = "lblToolboxHGrdSpfldRenderMode"
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
        'picToolboxHGrdSpfld
        '
        Me.picToolboxHGrdSpfld.BackColor = System.Drawing.SystemColors.ControlLight
        Me.picToolboxHGrdSpfld.Location = New System.Drawing.Point(15, 51)
        Me.picToolboxHGrdSpfld.Name = "picToolboxHGrdSpfld"
        Me.picToolboxHGrdSpfld.Size = New System.Drawing.Size(150, 103)
        Me.picToolboxHGrdSpfld.TabIndex = 6
        Me.picToolboxHGrdSpfld.TabStop = False
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
        'btnToolboxHGrdSpfldBitmapClear
        '
        Me.btnToolboxHGrdSpfldBitmapClear.Location = New System.Drawing.Point(122, 21)
        Me.btnToolboxHGrdSpfldBitmapClear.Name = "btnToolboxHGrdSpfldBitmapClear"
        Me.btnToolboxHGrdSpfldBitmapClear.Size = New System.Drawing.Size(43, 23)
        Me.btnToolboxHGrdSpfldBitmapClear.TabIndex = 4
        Me.btnToolboxHGrdSpfldBitmapClear.Text = "Clear"
        Me.btnToolboxHGrdSpfldBitmapClear.UseVisualStyleBackColor = True
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
        'btnToolboxHGrdSpfldBitmapName
        '
        Me.btnToolboxHGrdSpfldBitmapName.Location = New System.Drawing.Point(15, 21)
        Me.btnToolboxHGrdSpfldBitmapName.Name = "btnToolboxHGrdSpfldBitmapName"
        Me.btnToolboxHGrdSpfldBitmapName.Size = New System.Drawing.Size(104, 23)
        Me.btnToolboxHGrdSpfldBitmapName.TabIndex = 2
        Me.btnToolboxHGrdSpfldBitmapName.Text = "Spielfeld"
        Me.btnToolboxHGrdSpfldBitmapName.UseVisualStyleBackColor = True
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
        Me.btnInfoAktSpiel.InfoText = "Wenn Farben und Bilder hinterlegt sind, gelten die Bilder."
        Me.btnInfoAktSpiel.Location = New System.Drawing.Point(280, 280)
        Me.btnInfoAktSpiel.MinimumSize = New System.Drawing.Size(16, 16)
        Me.btnInfoAktSpiel.Name = "btnInfoAktSpiel"
        Me.btnInfoAktSpiel.Size = New System.Drawing.Size(26, 26)
        Me.btnInfoAktSpiel.TabIndex = 2
        Me.btnInfoAktSpiel.TabStop = False
        Me.btnInfoAktSpiel.Text = "ButtonInfo2"
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
        Me.gbxFallback.Controls.Add(Me.chkToolboxHGrdEditorUseSpfldEinstlgFallback)
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
        'chkToolboxHGrdEditorUseSpfldEinstlgFallback
        '
        Me.chkToolboxHGrdEditorUseSpfldEinstlgFallback.AutoSize = True
        Me.chkToolboxHGrdEditorUseSpfldEinstlgFallback.Location = New System.Drawing.Point(195, 280)
        Me.chkToolboxHGrdEditorUseSpfldEinstlgFallback.Name = "chkToolboxHGrdEditorUseSpfldEinstlgFallback"
        Me.chkToolboxHGrdEditorUseSpfldEinstlgFallback.Size = New System.Drawing.Size(83, 43)
        Me.chkToolboxHGrdEditorUseSpfldEinstlgFallback.TabIndex = 5
        Me.chkToolboxHGrdEditorUseSpfldEinstlgFallback.Text = "Editor nutzt" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Spielfeldein-" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "stellungen"
        Me.chkToolboxHGrdEditorUseSpfldEinstlgFallback.UseVisualStyleBackColor = True
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.lblToolboxHGrdEditorColorFallback)
        Me.GroupBox2.Controls.Add(Me.lblToolboxHGrdSpfldColorFallback)
        Me.GroupBox2.Controls.Add(Me.btnToolboxHGrdEditorColorClearFallback)
        Me.GroupBox2.Controls.Add(Me.btnToolboxHGrdSpfldColorClearFallback)
        Me.GroupBox2.Controls.Add(Me.btnToolboxHGrdEditorColorFallback)
        Me.GroupBox2.Controls.Add(Me.btnToolboxHGrdSpfldColorFallback)
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
        'lblToolboxHGrdSpfldColorFallback
        '
        Me.lblToolboxHGrdSpfldColorFallback.BackColor = System.Drawing.Color.Transparent
        Me.lblToolboxHGrdSpfldColorFallback.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblToolboxHGrdSpfldColorFallback.Cursor = System.Windows.Forms.Cursors.Default
        Me.lblToolboxHGrdSpfldColorFallback.Location = New System.Drawing.Point(6, 51)
        Me.lblToolboxHGrdSpfldColorFallback.Name = "lblToolboxHGrdSpfldColorFallback"
        Me.lblToolboxHGrdSpfldColorFallback.Size = New System.Drawing.Size(114, 35)
        Me.lblToolboxHGrdSpfldColorFallback.TabIndex = 10
        Me.lblToolboxHGrdSpfldColorFallback.Text = "(keine Farbe gewählt)"
        Me.lblToolboxHGrdSpfldColorFallback.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
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
        'btnToolboxHGrdSpfldColorClearFallback
        '
        Me.btnToolboxHGrdSpfldColorClearFallback.Location = New System.Drawing.Point(77, 21)
        Me.btnToolboxHGrdSpfldColorClearFallback.Name = "btnToolboxHGrdSpfldColorClearFallback"
        Me.btnToolboxHGrdSpfldColorClearFallback.Size = New System.Drawing.Size(43, 23)
        Me.btnToolboxHGrdSpfldColorClearFallback.TabIndex = 8
        Me.btnToolboxHGrdSpfldColorClearFallback.Text = "Clear"
        Me.btnToolboxHGrdSpfldColorClearFallback.UseVisualStyleBackColor = True
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
        'btnToolboxHGrdSpfldColorFallback
        '
        Me.btnToolboxHGrdSpfldColorFallback.Location = New System.Drawing.Point(5, 21)
        Me.btnToolboxHGrdSpfldColorFallback.Name = "btnToolboxHGrdSpfldColorFallback"
        Me.btnToolboxHGrdSpfldColorFallback.Size = New System.Drawing.Size(69, 23)
        Me.btnToolboxHGrdSpfldColorFallback.TabIndex = 6
        Me.btnToolboxHGrdSpfldColorFallback.Text = "Spielfeld"
        Me.btnToolboxHGrdSpfldColorFallback.UseVisualStyleBackColor = True
        '
        'GroupBox5
        '
        Me.GroupBox5.Controls.Add(Me.picToolboxHGrdSpfldFallback)
        Me.GroupBox5.Controls.Add(Me.lblToolboxHGrdEditorRenderModeFallback)
        Me.GroupBox5.Controls.Add(Me.lblToolboxHGrdSplfldRenderModeFallback)
        Me.GroupBox5.Controls.Add(Me.picToolboxHGrdEditorFallback)
        Me.GroupBox5.Controls.Add(Me.btnToolboxHGrdEditorBitmapClearFallback)
        Me.GroupBox5.Controls.Add(Me.btnToolboxHGrdSpfldBitmapClearFallback)
        Me.GroupBox5.Controls.Add(Me.btnToolboxHGrdEditorBitmapNameFallback)
        Me.GroupBox5.Controls.Add(Me.btnToolboxHGrdSpfldBitmapNameFallback)
        Me.GroupBox5.Location = New System.Drawing.Point(8, 19)
        Me.GroupBox5.Name = "GroupBox5"
        Me.GroupBox5.Size = New System.Drawing.Size(177, 344)
        Me.GroupBox5.TabIndex = 3
        Me.GroupBox5.TabStop = False
        Me.GroupBox5.Text = "Bilder"
        '
        'picToolboxHGrdSpfldFallback
        '
        Me.picToolboxHGrdSpfldFallback.BackColor = System.Drawing.SystemColors.ControlLight
        Me.picToolboxHGrdSpfldFallback.Location = New System.Drawing.Point(15, 51)
        Me.picToolboxHGrdSpfldFallback.Name = "picToolboxHGrdSpfldFallback"
        Me.picToolboxHGrdSpfldFallback.Size = New System.Drawing.Size(150, 102)
        Me.picToolboxHGrdSpfldFallback.TabIndex = 6
        Me.picToolboxHGrdSpfldFallback.TabStop = False
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
        'lblToolboxHGrdSplfldRenderModeFallback
        '
        Me.lblToolboxHGrdSplfldRenderModeFallback.AutoSize = True
        Me.lblToolboxHGrdSplfldRenderModeFallback.Location = New System.Drawing.Point(15, 157)
        Me.lblToolboxHGrdSplfldRenderModeFallback.Name = "lblToolboxHGrdSplfldRenderModeFallback"
        Me.lblToolboxHGrdSplfldRenderModeFallback.Size = New System.Drawing.Size(208, 13)
        Me.lblToolboxHGrdSplfldRenderModeFallback.TabIndex = 6
        Me.lblToolboxHGrdSplfldRenderModeFallback.Text = "lblToolboxHGrdSplfldRenderModeFallback"
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
        'btnToolboxHGrdSpfldBitmapClearFallback
        '
        Me.btnToolboxHGrdSpfldBitmapClearFallback.Location = New System.Drawing.Point(122, 21)
        Me.btnToolboxHGrdSpfldBitmapClearFallback.Name = "btnToolboxHGrdSpfldBitmapClearFallback"
        Me.btnToolboxHGrdSpfldBitmapClearFallback.Size = New System.Drawing.Size(43, 23)
        Me.btnToolboxHGrdSpfldBitmapClearFallback.TabIndex = 4
        Me.btnToolboxHGrdSpfldBitmapClearFallback.Text = "Clear"
        Me.btnToolboxHGrdSpfldBitmapClearFallback.UseVisualStyleBackColor = True
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
        'btnToolboxHGrdSpfldBitmapNameFallback
        '
        Me.btnToolboxHGrdSpfldBitmapNameFallback.Location = New System.Drawing.Point(15, 21)
        Me.btnToolboxHGrdSpfldBitmapNameFallback.Name = "btnToolboxHGrdSpfldBitmapNameFallback"
        Me.btnToolboxHGrdSpfldBitmapNameFallback.Size = New System.Drawing.Size(104, 23)
        Me.btnToolboxHGrdSpfldBitmapNameFallback.TabIndex = 2
        Me.btnToolboxHGrdSpfldBitmapNameFallback.Text = "Spielfeld"
        Me.btnToolboxHGrdSpfldBitmapNameFallback.UseVisualStyleBackColor = True
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
        Me.ButtonInfo1.InfoText = "Wenn Farben und Bilder hinterlegt sind, gelten die Bilder."
        Me.ButtonInfo1.Location = New System.Drawing.Point(280, 280)
        Me.ButtonInfo1.MinimumSize = New System.Drawing.Size(16, 16)
        Me.ButtonInfo1.Name = "ButtonInfo1"
        Me.ButtonInfo1.Size = New System.Drawing.Size(26, 26)
        Me.ButtonInfo1.TabIndex = 2
        Me.ButtonInfo1.TabStop = False
        Me.ButtonInfo1.Text = "ButtonInfo2"
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
        CType(Me.picToolboxHGrdSpfld, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TabPageFallback.ResumeLayout(False)
        Me.gbxFallback.ResumeLayout(False)
        Me.gbxFallback.PerformLayout()
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox5.ResumeLayout(False)
        Me.GroupBox5.PerformLayout()
        CType(Me.picToolboxHGrdSpfldFallback, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.picToolboxHGrdEditorFallback, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents lblToolboxHGrdSpfldColor As System.Windows.Forms.Label
    Friend WithEvents lblToolboxHGrdEditorColorFallback As System.Windows.Forms.Label
    Friend WithEvents lblToolboxHGrdSpfldColorFallback As System.Windows.Forms.Label
    Friend WithEvents GroupBox9 As GroupBox
    Friend WithEvents gbxAktSpiel As GroupBox
    Friend WithEvents chkToolboxHGrdEditorUseSpfldEinstlg As CheckBox
    Friend WithEvents GroupBox3 As GroupBox
    Friend WithEvents btnToolboxHGrdEditorColorClear As Button
    Friend WithEvents btnToolboxHGrdSpfldColorClear As Button
    Friend WithEvents btnToolboxHGrdEditorColor As Button
    Friend WithEvents btnToolboxHGrdSpfldColor As Button
    Friend WithEvents GroupBox4 As GroupBox
    Friend WithEvents btnToolboxHGrdEditorBitmapClear As Button
    Friend WithEvents btnToolboxHGrdSpfldBitmapClear As Button
    Friend WithEvents btnToolboxHGrdEditorBitmapName As Button
    Friend WithEvents btnToolboxHGrdSpfldBitmapName As Button
    Friend WithEvents btnInfoAktSpiel As ButtonInfo
    Friend WithEvents TabControlToolboxHGrd As TabControl
    Friend WithEvents TabPageAktSpiel As TabPage
    Friend WithEvents TabPageFallback As TabPage
    Friend WithEvents picToolboxHGrdSpfld As PictureBox
    Friend WithEvents picToolboxHGrdEditor As PictureBox
    Friend WithEvents gbxFallback As GroupBox
    Friend WithEvents chkToolboxHGrdEditorUseSpfldEinstlgFallback As CheckBox
    Friend WithEvents GroupBox2 As GroupBox
    Friend WithEvents btnToolboxHGrdEditorColorClearFallback As Button
    Friend WithEvents btnToolboxHGrdSpfldColorClearFallback As Button
    Friend WithEvents btnToolboxHGrdEditorColorFallback As Button
    Friend WithEvents btnToolboxHGrdSpfldColorFallback As Button
    Friend WithEvents GroupBox5 As GroupBox
    Friend WithEvents picToolboxHGrdEditorFallback As PictureBox
    Friend WithEvents picToolboxHGrdSpfldFallback As PictureBox
    Friend WithEvents btnToolboxHGrdEditorBitmapClearFallback As Button
    Friend WithEvents btnToolboxHGrdSpfldBitmapClearFallback As Button
    Friend WithEvents btnToolboxHGrdEditorBitmapNameFallback As Button
    Friend WithEvents btnToolboxHGrdSpfldBitmapNameFallback As Button
    Friend WithEvents ButtonInfo1 As ButtonInfo
    Friend WithEvents lblToolboxHGrdSplfldRenderModeFallback As Label
    Friend WithEvents lblToolboxHGrdEditorRenderMode As Label
    Friend WithEvents lblToolboxHGrdSpfldRenderMode As Label
    Friend WithEvents lblToolboxHGrdEditorRenderModeFallback As Label
    Friend WithEvents lblToolboxHGrdEditorColor As Label
End Class
