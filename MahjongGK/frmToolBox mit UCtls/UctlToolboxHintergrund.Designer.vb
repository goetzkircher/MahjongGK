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
        Me.chkDrawRenderRect = New System.Windows.Forms.CheckBox()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.chkToolboxHGrdEditorShowFraming = New System.Windows.Forms.CheckBox()
        Me.btnInfoAktSpiel = New MahjongGK.ButtonInfo()
        Me.GroupBox3 = New System.Windows.Forms.GroupBox()
        Me.ButtonTooltip1 = New MahjongGK.ButtonTooltip()
        Me.lblToolboxHGrdSplFldColor = New System.Windows.Forms.Label()
        Me.btnToolboxHGrdSplFldColorClear = New System.Windows.Forms.Button()
        Me.btnToolboxHGrdSplFldColor = New System.Windows.Forms.Button()
        Me.GroupBox4 = New System.Windows.Forms.GroupBox()
        Me.lblToolboxHGrdSplFldRenderMode = New System.Windows.Forms.Label()
        Me.picToolboxHGrdSplFld = New System.Windows.Forms.PictureBox()
        Me.btnToolboxHGrdSplFldBitmapClear = New System.Windows.Forms.Button()
        Me.btnToolboxHGrdSplFldBitmapName = New System.Windows.Forms.Button()
        Me.TabPageFallback = New System.Windows.Forms.TabPage()
        Me.gbxFallback = New System.Windows.Forms.GroupBox()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.ButtonTooltip2 = New MahjongGK.ButtonTooltip()
        Me.lblToolboxHGrdSplFldColorFallback = New System.Windows.Forms.Label()
        Me.btnToolboxHGrdSplFldColorClearFallback = New System.Windows.Forms.Button()
        Me.btnToolboxHGrdSplFldColorFallback = New System.Windows.Forms.Button()
        Me.GroupBox5 = New System.Windows.Forms.GroupBox()
        Me.picToolboxHGrdSplFldFallback = New System.Windows.Forms.PictureBox()
        Me.lblToolboxHGrdSplFldRenderModeFallback = New System.Windows.Forms.Label()
        Me.btnToolboxHGrdSplFldBitmapClearFallback = New System.Windows.Forms.Button()
        Me.btnToolboxHGrdSplFldBitmapNameFallback = New System.Windows.Forms.Button()
        Me.ButtonInfo1 = New MahjongGK.ButtonInfo()
        Me.gbxAktSpielOnlyIDE = New System.Windows.Forms.GroupBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.lblDominantBackgroundColor = New System.Windows.Forms.Label()
        Me.lblColorMouseOver = New System.Windows.Forms.Label()
        Me.lblColorMouseDown = New System.Windows.Forms.Label()
        Me.lblColorSelected = New System.Windows.Forms.Label()
        Me.lblColorNormal = New System.Windows.Forms.Label()
        Me.GroupBox9.SuspendLayout()
        Me.TabControlToolboxHGrd.SuspendLayout()
        Me.TabPageAktSpiel.SuspendLayout()
        Me.gbxAktSpiel.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
        Me.GroupBox4.SuspendLayout()
        CType(Me.picToolboxHGrdSplFld, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TabPageFallback.SuspendLayout()
        Me.gbxFallback.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.GroupBox5.SuspendLayout()
        CType(Me.picToolboxHGrdSplFldFallback, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.gbxAktSpielOnlyIDE.SuspendLayout()
        Me.SuspendLayout()
        '
        'GroupBox9
        '
        Me.GroupBox9.Controls.Add(Me.TabControlToolboxHGrd)
        Me.GroupBox9.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupBox9.Location = New System.Drawing.Point(0, 6)
        Me.GroupBox9.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.GroupBox9.Name = "GroupBox9"
        Me.GroupBox9.Padding = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.GroupBox9.Size = New System.Drawing.Size(480, 557)
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
        Me.TabControlToolboxHGrd.Location = New System.Drawing.Point(4, 19)
        Me.TabControlToolboxHGrd.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.TabControlToolboxHGrd.Name = "TabControlToolboxHGrd"
        Me.TabControlToolboxHGrd.SelectedIndex = 0
        Me.TabControlToolboxHGrd.Size = New System.Drawing.Size(472, 534)
        Me.TabControlToolboxHGrd.TabIndex = 18
        '
        'TabPageAktSpiel
        '
        Me.TabPageAktSpiel.Controls.Add(Me.gbxAktSpiel)
        Me.TabPageAktSpiel.Location = New System.Drawing.Point(4, 36)
        Me.TabPageAktSpiel.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.TabPageAktSpiel.Name = "TabPageAktSpiel"
        Me.TabPageAktSpiel.Padding = New System.Windows.Forms.Padding(13, 12, 13, 12)
        Me.TabPageAktSpiel.Size = New System.Drawing.Size(464, 494)
        Me.TabPageAktSpiel.TabIndex = 0
        Me.TabPageAktSpiel.Text = "Aktuelles Spielfeld"
        Me.TabPageAktSpiel.UseVisualStyleBackColor = True
        '
        'gbxAktSpiel
        '
        Me.gbxAktSpiel.Controls.Add(Me.gbxAktSpielOnlyIDE)
        Me.gbxAktSpiel.Controls.Add(Me.GroupBox1)
        Me.gbxAktSpiel.Controls.Add(Me.GroupBox3)
        Me.gbxAktSpiel.Controls.Add(Me.GroupBox4)
        Me.gbxAktSpiel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.gbxAktSpiel.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.gbxAktSpiel.Location = New System.Drawing.Point(13, 12)
        Me.gbxAktSpiel.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.gbxAktSpiel.Name = "gbxAktSpiel"
        Me.gbxAktSpiel.Padding = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.gbxAktSpiel.Size = New System.Drawing.Size(438, 470)
        Me.gbxAktSpiel.TabIndex = 17
        Me.gbxAktSpiel.TabStop = False
        Me.gbxAktSpiel.Text = "Aktuell geladenes Spielfeld"
        '
        'chkDrawRenderRect
        '
        Me.chkDrawRenderRect.AutoSize = True
        Me.chkDrawRenderRect.Location = New System.Drawing.Point(6, 23)
        Me.chkDrawRenderRect.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.chkDrawRenderRect.Name = "chkDrawRenderRect"
        Me.chkDrawRenderRect.Size = New System.Drawing.Size(371, 38)
        Me.chkDrawRenderRect.TabIndex = 8
        Me.chkDrawRenderRect.Text = "Kein Hintergrundbild, sondern die Aufteilung anzeigen" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "(Alle RectangleX)"
        Me.chkDrawRenderRect.UseVisualStyleBackColor = True
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.chkToolboxHGrdEditorShowFraming)
        Me.GroupBox1.Controls.Add(Me.btnInfoAktSpiel)
        Me.GroupBox1.Location = New System.Drawing.Point(255, 186)
        Me.GroupBox1.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Padding = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.GroupBox1.Size = New System.Drawing.Size(172, 91)
        Me.GroupBox1.TabIndex = 7
        Me.GroupBox1.TabStop = False
        '
        'chkToolboxHGrdEditorShowFraming
        '
        Me.chkToolboxHGrdEditorShowFraming.AutoSize = True
        Me.chkToolboxHGrdEditorShowFraming.Location = New System.Drawing.Point(8, 17)
        Me.chkToolboxHGrdEditorShowFraming.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.chkToolboxHGrdEditorShowFraming.Name = "chkToolboxHGrdEditorShowFraming"
        Me.chkToolboxHGrdEditorShowFraming.Size = New System.Drawing.Size(159, 38)
        Me.chkToolboxHGrdEditorShowFraming.TabIndex = 6
        Me.chkToolboxHGrdEditorShowFraming.Text = "Spielfeldumrahmung" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "anzeigen (nur Editor)"
        Me.chkToolboxHGrdEditorShowFraming.UseVisualStyleBackColor = True
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
        Me.btnInfoAktSpiel.Location = New System.Drawing.Point(119, 53)
        Me.btnInfoAktSpiel.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.btnInfoAktSpiel.MinimumSize = New System.Drawing.Size(21, 20)
        Me.btnInfoAktSpiel.Name = "btnInfoAktSpiel"
        Me.btnInfoAktSpiel.Size = New System.Drawing.Size(35, 35)
        Me.btnInfoAktSpiel.TabIndex = 2
        Me.btnInfoAktSpiel.TabStop = False
        Me.btnInfoAktSpiel.Text = "ButtonInfo2"
        '
        'GroupBox3
        '
        Me.GroupBox3.Controls.Add(Me.ButtonTooltip1)
        Me.GroupBox3.Controls.Add(Me.lblToolboxHGrdSplFldColor)
        Me.GroupBox3.Controls.Add(Me.btnToolboxHGrdSplFldColorClear)
        Me.GroupBox3.Controls.Add(Me.btnToolboxHGrdSplFldColor)
        Me.GroupBox3.Location = New System.Drawing.Point(255, 23)
        Me.GroupBox3.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Padding = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.GroupBox3.Size = New System.Drawing.Size(172, 155)
        Me.GroupBox3.TabIndex = 4
        Me.GroupBox3.TabStop = False
        Me.GroupBox3.Text = "Farben"
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
        Me.ButtonTooltip1.Location = New System.Drawing.Point(119, 114)
        Me.ButtonTooltip1.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.ButtonTooltip1.MinimumSize = New System.Drawing.Size(21, 20)
        Me.ButtonTooltip1.Name = "ButtonTooltip1"
        Me.ButtonTooltip1.ShowOnFocus = False
        Me.ButtonTooltip1.Size = New System.Drawing.Size(35, 35)
        Me.ButtonTooltip1.TabIndex = 12
        Me.ButtonTooltip1.TabStop = False
        Me.ButtonTooltip1.Text = "ButtonTooltip1"
        '
        'lblToolboxHGrdSplFldColor
        '
        Me.lblToolboxHGrdSplFldColor.BackColor = System.Drawing.Color.Transparent
        Me.lblToolboxHGrdSplFldColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblToolboxHGrdSplFldColor.Cursor = System.Windows.Forms.Cursors.Default
        Me.lblToolboxHGrdSplFldColor.Location = New System.Drawing.Point(8, 63)
        Me.lblToolboxHGrdSplFldColor.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblToolboxHGrdSplFldColor.Name = "lblToolboxHGrdSplFldColor"
        Me.lblToolboxHGrdSplFldColor.Size = New System.Drawing.Size(151, 43)
        Me.lblToolboxHGrdSplFldColor.TabIndex = 10
        Me.lblToolboxHGrdSplFldColor.Text = "(keine Farbe gewählt)"
        Me.lblToolboxHGrdSplFldColor.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'btnToolboxHGrdSplFldColorClear
        '
        Me.btnToolboxHGrdSplFldColorClear.Location = New System.Drawing.Point(103, 26)
        Me.btnToolboxHGrdSplFldColorClear.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.btnToolboxHGrdSplFldColorClear.Name = "btnToolboxHGrdSplFldColorClear"
        Me.btnToolboxHGrdSplFldColorClear.Size = New System.Drawing.Size(57, 28)
        Me.btnToolboxHGrdSplFldColorClear.TabIndex = 8
        Me.btnToolboxHGrdSplFldColorClear.Text = "Clear"
        Me.btnToolboxHGrdSplFldColorClear.UseVisualStyleBackColor = True
        '
        'btnToolboxHGrdSplFldColor
        '
        Me.btnToolboxHGrdSplFldColor.Location = New System.Drawing.Point(7, 26)
        Me.btnToolboxHGrdSplFldColor.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.btnToolboxHGrdSplFldColor.Name = "btnToolboxHGrdSplFldColor"
        Me.btnToolboxHGrdSplFldColor.Size = New System.Drawing.Size(92, 28)
        Me.btnToolboxHGrdSplFldColor.TabIndex = 6
        Me.btnToolboxHGrdSplFldColor.Text = "Spielfeld"
        Me.btnToolboxHGrdSplFldColor.UseVisualStyleBackColor = True
        '
        'GroupBox4
        '
        Me.GroupBox4.Controls.Add(Me.lblToolboxHGrdSplFldRenderMode)
        Me.GroupBox4.Controls.Add(Me.picToolboxHGrdSplFld)
        Me.GroupBox4.Controls.Add(Me.btnToolboxHGrdSplFldBitmapClear)
        Me.GroupBox4.Controls.Add(Me.btnToolboxHGrdSplFldBitmapName)
        Me.GroupBox4.Location = New System.Drawing.Point(11, 23)
        Me.GroupBox4.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.GroupBox4.Name = "GroupBox4"
        Me.GroupBox4.Padding = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.GroupBox4.Size = New System.Drawing.Size(236, 260)
        Me.GroupBox4.TabIndex = 3
        Me.GroupBox4.TabStop = False
        Me.GroupBox4.Text = "Bilder"
        '
        'lblToolboxHGrdSplFldRenderMode
        '
        Me.lblToolboxHGrdSplFldRenderMode.AutoSize = True
        Me.lblToolboxHGrdSplFldRenderMode.Location = New System.Drawing.Point(20, 193)
        Me.lblToolboxHGrdSplFldRenderMode.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblToolboxHGrdSplFldRenderMode.Name = "lblToolboxHGrdSplFldRenderMode"
        Me.lblToolboxHGrdSplFldRenderMode.Size = New System.Drawing.Size(227, 17)
        Me.lblToolboxHGrdSplFldRenderMode.TabIndex = 8
        Me.lblToolboxHGrdSplFldRenderMode.Text = "lblToolboxHGrdSplFldRenderMode"
        '
        'picToolboxHGrdSplFld
        '
        Me.picToolboxHGrdSplFld.BackColor = System.Drawing.SystemColors.ControlLight
        Me.picToolboxHGrdSplFld.Location = New System.Drawing.Point(20, 63)
        Me.picToolboxHGrdSplFld.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.picToolboxHGrdSplFld.Name = "picToolboxHGrdSplFld"
        Me.picToolboxHGrdSplFld.Size = New System.Drawing.Size(200, 127)
        Me.picToolboxHGrdSplFld.TabIndex = 6
        Me.picToolboxHGrdSplFld.TabStop = False
        '
        'btnToolboxHGrdSplFldBitmapClear
        '
        Me.btnToolboxHGrdSplFldBitmapClear.Location = New System.Drawing.Point(163, 26)
        Me.btnToolboxHGrdSplFldBitmapClear.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.btnToolboxHGrdSplFldBitmapClear.Name = "btnToolboxHGrdSplFldBitmapClear"
        Me.btnToolboxHGrdSplFldBitmapClear.Size = New System.Drawing.Size(57, 28)
        Me.btnToolboxHGrdSplFldBitmapClear.TabIndex = 4
        Me.btnToolboxHGrdSplFldBitmapClear.Text = "Clear"
        Me.btnToolboxHGrdSplFldBitmapClear.UseVisualStyleBackColor = True
        '
        'btnToolboxHGrdSplFldBitmapName
        '
        Me.btnToolboxHGrdSplFldBitmapName.Location = New System.Drawing.Point(20, 26)
        Me.btnToolboxHGrdSplFldBitmapName.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.btnToolboxHGrdSplFldBitmapName.Name = "btnToolboxHGrdSplFldBitmapName"
        Me.btnToolboxHGrdSplFldBitmapName.Size = New System.Drawing.Size(139, 28)
        Me.btnToolboxHGrdSplFldBitmapName.TabIndex = 2
        Me.btnToolboxHGrdSplFldBitmapName.Text = "Spielfeld"
        Me.btnToolboxHGrdSplFldBitmapName.UseVisualStyleBackColor = True
        '
        'TabPageFallback
        '
        Me.TabPageFallback.Controls.Add(Me.gbxFallback)
        Me.TabPageFallback.Location = New System.Drawing.Point(4, 36)
        Me.TabPageFallback.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.TabPageFallback.Name = "TabPageFallback"
        Me.TabPageFallback.Padding = New System.Windows.Forms.Padding(13, 12, 13, 12)
        Me.TabPageFallback.Size = New System.Drawing.Size(464, 343)
        Me.TabPageFallback.TabIndex = 1
        Me.TabPageFallback.Text = "Fallback"
        Me.TabPageFallback.UseVisualStyleBackColor = True
        '
        'gbxFallback
        '
        Me.gbxFallback.Controls.Add(Me.GroupBox2)
        Me.gbxFallback.Controls.Add(Me.GroupBox5)
        Me.gbxFallback.Controls.Add(Me.ButtonInfo1)
        Me.gbxFallback.Dock = System.Windows.Forms.DockStyle.Fill
        Me.gbxFallback.Location = New System.Drawing.Point(13, 12)
        Me.gbxFallback.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.gbxFallback.Name = "gbxFallback"
        Me.gbxFallback.Padding = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.gbxFallback.Size = New System.Drawing.Size(438, 319)
        Me.gbxFallback.TabIndex = 18
        Me.gbxFallback.TabStop = False
        Me.gbxFallback.Text = "Fallback (Wenn das aktuelles Spielfeld keine Werte hat)"
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.Label1)
        Me.GroupBox2.Controls.Add(Me.ButtonTooltip2)
        Me.GroupBox2.Controls.Add(Me.lblToolboxHGrdSplFldColorFallback)
        Me.GroupBox2.Controls.Add(Me.btnToolboxHGrdSplFldColorClearFallback)
        Me.GroupBox2.Controls.Add(Me.btnToolboxHGrdSplFldColorFallback)
        Me.GroupBox2.Location = New System.Drawing.Point(255, 23)
        Me.GroupBox2.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Padding = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.GroupBox2.Size = New System.Drawing.Size(172, 282)
        Me.GroupBox2.TabIndex = 4
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Farben"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(8, 193)
        Me.Label1.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(108, 32)
        Me.Label1.TabIndex = 14
        Me.Label1.Text = "Auch HGrdFarbe" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "der Werkbank"
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
        Me.ButtonTooltip2.Location = New System.Drawing.Point(119, 114)
        Me.ButtonTooltip2.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.ButtonTooltip2.MinimumSize = New System.Drawing.Size(21, 20)
        Me.ButtonTooltip2.Name = "ButtonTooltip2"
        Me.ButtonTooltip2.ShowOnFocus = False
        Me.ButtonTooltip2.Size = New System.Drawing.Size(35, 35)
        Me.ButtonTooltip2.TabIndex = 13
        Me.ButtonTooltip2.TabStop = False
        Me.ButtonTooltip2.Text = "ButtonTooltip2"
        '
        'lblToolboxHGrdSplFldColorFallback
        '
        Me.lblToolboxHGrdSplFldColorFallback.BackColor = System.Drawing.Color.Transparent
        Me.lblToolboxHGrdSplFldColorFallback.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblToolboxHGrdSplFldColorFallback.Cursor = System.Windows.Forms.Cursors.Default
        Me.lblToolboxHGrdSplFldColorFallback.Location = New System.Drawing.Point(8, 63)
        Me.lblToolboxHGrdSplFldColorFallback.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblToolboxHGrdSplFldColorFallback.Name = "lblToolboxHGrdSplFldColorFallback"
        Me.lblToolboxHGrdSplFldColorFallback.Size = New System.Drawing.Size(151, 43)
        Me.lblToolboxHGrdSplFldColorFallback.TabIndex = 10
        Me.lblToolboxHGrdSplFldColorFallback.Text = "(keine Farbe gewählt)"
        Me.lblToolboxHGrdSplFldColorFallback.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'btnToolboxHGrdSplFldColorClearFallback
        '
        Me.btnToolboxHGrdSplFldColorClearFallback.Location = New System.Drawing.Point(103, 26)
        Me.btnToolboxHGrdSplFldColorClearFallback.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.btnToolboxHGrdSplFldColorClearFallback.Name = "btnToolboxHGrdSplFldColorClearFallback"
        Me.btnToolboxHGrdSplFldColorClearFallback.Size = New System.Drawing.Size(57, 28)
        Me.btnToolboxHGrdSplFldColorClearFallback.TabIndex = 8
        Me.btnToolboxHGrdSplFldColorClearFallback.Text = "Clear"
        Me.btnToolboxHGrdSplFldColorClearFallback.UseVisualStyleBackColor = True
        '
        'btnToolboxHGrdSplFldColorFallback
        '
        Me.btnToolboxHGrdSplFldColorFallback.Location = New System.Drawing.Point(7, 26)
        Me.btnToolboxHGrdSplFldColorFallback.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.btnToolboxHGrdSplFldColorFallback.Name = "btnToolboxHGrdSplFldColorFallback"
        Me.btnToolboxHGrdSplFldColorFallback.Size = New System.Drawing.Size(92, 28)
        Me.btnToolboxHGrdSplFldColorFallback.TabIndex = 6
        Me.btnToolboxHGrdSplFldColorFallback.Text = "Spielfeld"
        Me.btnToolboxHGrdSplFldColorFallback.UseVisualStyleBackColor = True
        '
        'GroupBox5
        '
        Me.GroupBox5.Controls.Add(Me.picToolboxHGrdSplFldFallback)
        Me.GroupBox5.Controls.Add(Me.lblToolboxHGrdSplFldRenderModeFallback)
        Me.GroupBox5.Controls.Add(Me.btnToolboxHGrdSplFldBitmapClearFallback)
        Me.GroupBox5.Controls.Add(Me.btnToolboxHGrdSplFldBitmapNameFallback)
        Me.GroupBox5.Location = New System.Drawing.Point(11, 23)
        Me.GroupBox5.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.GroupBox5.Name = "GroupBox5"
        Me.GroupBox5.Padding = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.GroupBox5.Size = New System.Drawing.Size(236, 423)
        Me.GroupBox5.TabIndex = 3
        Me.GroupBox5.TabStop = False
        Me.GroupBox5.Text = "Bilder"
        '
        'picToolboxHGrdSplFldFallback
        '
        Me.picToolboxHGrdSplFldFallback.BackColor = System.Drawing.SystemColors.ControlLight
        Me.picToolboxHGrdSplFldFallback.Location = New System.Drawing.Point(20, 63)
        Me.picToolboxHGrdSplFldFallback.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.picToolboxHGrdSplFldFallback.Name = "picToolboxHGrdSplFldFallback"
        Me.picToolboxHGrdSplFldFallback.Size = New System.Drawing.Size(200, 126)
        Me.picToolboxHGrdSplFldFallback.TabIndex = 6
        Me.picToolboxHGrdSplFldFallback.TabStop = False
        '
        'lblToolboxHGrdSplFldRenderModeFallback
        '
        Me.lblToolboxHGrdSplFldRenderModeFallback.AutoSize = True
        Me.lblToolboxHGrdSplFldRenderModeFallback.Location = New System.Drawing.Point(20, 193)
        Me.lblToolboxHGrdSplFldRenderModeFallback.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblToolboxHGrdSplFldRenderModeFallback.Name = "lblToolboxHGrdSplFldRenderModeFallback"
        Me.lblToolboxHGrdSplFldRenderModeFallback.Size = New System.Drawing.Size(274, 16)
        Me.lblToolboxHGrdSplFldRenderModeFallback.TabIndex = 6
        Me.lblToolboxHGrdSplFldRenderModeFallback.Text = "lblToolboxHGrdSplFldRenderModeFallback"
        '
        'btnToolboxHGrdSplFldBitmapClearFallback
        '
        Me.btnToolboxHGrdSplFldBitmapClearFallback.Location = New System.Drawing.Point(163, 26)
        Me.btnToolboxHGrdSplFldBitmapClearFallback.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.btnToolboxHGrdSplFldBitmapClearFallback.Name = "btnToolboxHGrdSplFldBitmapClearFallback"
        Me.btnToolboxHGrdSplFldBitmapClearFallback.Size = New System.Drawing.Size(57, 28)
        Me.btnToolboxHGrdSplFldBitmapClearFallback.TabIndex = 4
        Me.btnToolboxHGrdSplFldBitmapClearFallback.Text = "Clear"
        Me.btnToolboxHGrdSplFldBitmapClearFallback.UseVisualStyleBackColor = True
        '
        'btnToolboxHGrdSplFldBitmapNameFallback
        '
        Me.btnToolboxHGrdSplFldBitmapNameFallback.Location = New System.Drawing.Point(20, 26)
        Me.btnToolboxHGrdSplFldBitmapNameFallback.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.btnToolboxHGrdSplFldBitmapNameFallback.Name = "btnToolboxHGrdSplFldBitmapNameFallback"
        Me.btnToolboxHGrdSplFldBitmapNameFallback.Size = New System.Drawing.Size(139, 28)
        Me.btnToolboxHGrdSplFldBitmapNameFallback.TabIndex = 2
        Me.btnToolboxHGrdSplFldBitmapNameFallback.Text = "Spielfeld"
        Me.btnToolboxHGrdSplFldBitmapNameFallback.UseVisualStyleBackColor = True
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
        Me.ButtonInfo1.Location = New System.Drawing.Point(373, 356)
        Me.ButtonInfo1.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.ButtonInfo1.MinimumSize = New System.Drawing.Size(21, 20)
        Me.ButtonInfo1.Name = "ButtonInfo1"
        Me.ButtonInfo1.Size = New System.Drawing.Size(35, 35)
        Me.ButtonInfo1.TabIndex = 2
        Me.ButtonInfo1.TabStop = False
        Me.ButtonInfo1.Text = "ButtonInfo2"
        '
        'gbxAktSpielOnlyIDE
        '
        Me.gbxAktSpielOnlyIDE.Controls.Add(Me.lblColorNormal)
        Me.gbxAktSpielOnlyIDE.Controls.Add(Me.lblColorSelected)
        Me.gbxAktSpielOnlyIDE.Controls.Add(Me.lblColorMouseDown)
        Me.gbxAktSpielOnlyIDE.Controls.Add(Me.lblColorMouseOver)
        Me.gbxAktSpielOnlyIDE.Controls.Add(Me.lblDominantBackgroundColor)
        Me.gbxAktSpielOnlyIDE.Controls.Add(Me.Label7)
        Me.gbxAktSpielOnlyIDE.Controls.Add(Me.Label6)
        Me.gbxAktSpielOnlyIDE.Controls.Add(Me.Label5)
        Me.gbxAktSpielOnlyIDE.Controls.Add(Me.Label4)
        Me.gbxAktSpielOnlyIDE.Controls.Add(Me.Label3)
        Me.gbxAktSpielOnlyIDE.Controls.Add(Me.Label2)
        Me.gbxAktSpielOnlyIDE.Controls.Add(Me.chkDrawRenderRect)
        Me.gbxAktSpielOnlyIDE.Location = New System.Drawing.Point(11, 290)
        Me.gbxAktSpielOnlyIDE.Name = "gbxAktSpielOnlyIDE"
        Me.gbxAktSpielOnlyIDE.Size = New System.Drawing.Size(416, 172)
        Me.gbxAktSpielOnlyIDE.TabIndex = 9
        Me.gbxAktSpielOnlyIDE.TabStop = False
        Me.gbxAktSpielOnlyIDE.Text = "Anzeige nur in der IDE"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(7, 69)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(134, 17)
        Me.Label2.TabIndex = 9
        Me.Label2.Text = "OverlayColorPalette"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(7, 96)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(99, 34)
        Me.Label3.TabIndex = 10
        Me.Label3.Text = "DominantBack" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "groundColor"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(7, 140)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(86, 17)
        Me.Label4.TabIndex = 11
        Me.Label4.Text = "ColorNormal"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(207, 96)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(114, 17)
        Me.Label5.TabIndex = 12
        Me.Label5.Text = "ColorMouseOver"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(207, 117)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(118, 17)
        Me.Label6.TabIndex = 13
        Me.Label6.Text = "ColorMouseDown"
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(207, 140)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(96, 17)
        Me.Label7.TabIndex = 14
        Me.Label7.Text = "ColorSelected"
        '
        'lblDominantBackgroundColor
        '
        Me.lblDominantBackgroundColor.BackColor = System.Drawing.Color.LightGray
        Me.lblDominantBackgroundColor.Location = New System.Drawing.Point(113, 100)
        Me.lblDominantBackgroundColor.Name = "lblDominantBackgroundColor"
        Me.lblDominantBackgroundColor.Size = New System.Drawing.Size(65, 34)
        Me.lblDominantBackgroundColor.TabIndex = 15
        '
        'lblColorMouseOver
        '
        Me.lblColorMouseOver.BackColor = System.Drawing.Color.LightGray
        Me.lblColorMouseOver.Location = New System.Drawing.Point(327, 96)
        Me.lblColorMouseOver.Name = "lblColorMouseOver"
        Me.lblColorMouseOver.Size = New System.Drawing.Size(65, 17)
        Me.lblColorMouseOver.TabIndex = 16
        '
        'lblColorMouseDown
        '
        Me.lblColorMouseDown.BackColor = System.Drawing.Color.LightGray
        Me.lblColorMouseDown.Location = New System.Drawing.Point(327, 118)
        Me.lblColorMouseDown.Name = "lblColorMouseDown"
        Me.lblColorMouseDown.Size = New System.Drawing.Size(65, 17)
        Me.lblColorMouseDown.TabIndex = 17
        '
        'lblColorSelected
        '
        Me.lblColorSelected.BackColor = System.Drawing.Color.LightGray
        Me.lblColorSelected.Location = New System.Drawing.Point(327, 140)
        Me.lblColorSelected.Name = "lblColorSelected"
        Me.lblColorSelected.Size = New System.Drawing.Size(65, 17)
        Me.lblColorSelected.TabIndex = 18
        '
        'lblColorNormal
        '
        Me.lblColorNormal.BackColor = System.Drawing.Color.LightGray
        Me.lblColorNormal.Location = New System.Drawing.Point(113, 140)
        Me.lblColorNormal.Name = "lblColorNormal"
        Me.lblColorNormal.Size = New System.Drawing.Size(65, 17)
        Me.lblColorNormal.TabIndex = 19
        '
        'UctlToolboxHintergrund
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.GroupBox9)
        Me.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.Name = "UctlToolboxHintergrund"
        Me.Padding = New System.Windows.Forms.Padding(0, 6, 0, 0)
        Me.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Size = New System.Drawing.Size(480, 563)
        Me.GroupBox9.ResumeLayout(False)
        Me.TabControlToolboxHGrd.ResumeLayout(False)
        Me.TabPageAktSpiel.ResumeLayout(False)
        Me.gbxAktSpiel.ResumeLayout(False)
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.GroupBox3.ResumeLayout(False)
        Me.GroupBox4.ResumeLayout(False)
        Me.GroupBox4.PerformLayout()
        CType(Me.picToolboxHGrdSplFld, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TabPageFallback.ResumeLayout(False)
        Me.gbxFallback.ResumeLayout(False)
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.GroupBox5.ResumeLayout(False)
        Me.GroupBox5.PerformLayout()
        CType(Me.picToolboxHGrdSplFldFallback, System.ComponentModel.ISupportInitialize).EndInit()
        Me.gbxAktSpielOnlyIDE.ResumeLayout(False)
        Me.gbxAktSpielOnlyIDE.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents lblToolboxHGrdSplFldColor As System.Windows.Forms.Label
    Friend WithEvents lblToolboxHGrdSplFldColorFallback As System.Windows.Forms.Label
    Friend WithEvents GroupBox9 As GroupBox
    Friend WithEvents gbxAktSpiel As GroupBox
    Friend WithEvents GroupBox3 As GroupBox
    Friend WithEvents btnToolboxHGrdSplFldColorClear As Button
    Friend WithEvents btnToolboxHGrdSplFldColor As Button
    Friend WithEvents GroupBox4 As GroupBox
    Friend WithEvents btnToolboxHGrdSplFldBitmapClear As Button
    Friend WithEvents btnToolboxHGrdSplFldBitmapName As Button
    Friend WithEvents btnInfoAktSpiel As ButtonInfo
    Friend WithEvents TabControlToolboxHGrd As TabControl
    Friend WithEvents TabPageAktSpiel As TabPage
    Friend WithEvents TabPageFallback As TabPage
    Friend WithEvents picToolboxHGrdSplFld As PictureBox
    Friend WithEvents gbxFallback As GroupBox
    Friend WithEvents GroupBox2 As GroupBox
    Friend WithEvents btnToolboxHGrdSplFldColorClearFallback As Button
    Friend WithEvents btnToolboxHGrdSplFldColorFallback As Button
    Friend WithEvents GroupBox5 As GroupBox
    Friend WithEvents picToolboxHGrdSplFldFallback As PictureBox
    Friend WithEvents btnToolboxHGrdSplFldBitmapClearFallback As Button
    Friend WithEvents btnToolboxHGrdSplFldBitmapNameFallback As Button
    Friend WithEvents ButtonInfo1 As ButtonInfo
    Friend WithEvents lblToolboxHGrdSplFldRenderModeFallback As Label
    Friend WithEvents lblToolboxHGrdSplFldRenderMode As Label
    Friend WithEvents chkToolboxHGrdEditorShowFraming As CheckBox
    Friend WithEvents ButtonTooltip1 As ButtonTooltip
    Friend WithEvents ButtonTooltip2 As ButtonTooltip
    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents chkDrawRenderRect As CheckBox
    Friend WithEvents Label1 As Label
    Friend WithEvents gbxAktSpielOnlyIDE As GroupBox
    Friend WithEvents Label2 As Label
    Friend WithEvents Label6 As Label
    Friend WithEvents Label5 As Label
    Friend WithEvents Label4 As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents Label7 As Label
    Friend WithEvents lblColorNormal As Label
    Friend WithEvents lblColorSelected As Label
    Friend WithEvents lblColorMouseDown As Label
    Friend WithEvents lblColorMouseOver As Label
    Friend WithEvents lblDominantBackgroundColor As Label
End Class
