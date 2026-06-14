<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmSpielAnlegen
    Inherits System.Windows.Forms.Form

    'Das Formular überschreibt den Löschvorgang, um die Komponentenliste zu bereinigen.
    <System.Diagnostics.DebuggerNonUserCode()> _
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
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmSpielAnlegen))
        Me.lblInfoOben = New System.Windows.Forms.Label()
        Me.txtName = New System.Windows.Forms.TextBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.optSteineStream = New System.Windows.Forms.RadioButton()
        Me.optSteineFix = New System.Windows.Forms.RadioButton()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.optBasis152 = New System.Windows.Forms.RadioButton()
        Me.optBasis144 = New System.Windows.Forms.RadioButton()
        Me.Panel3 = New System.Windows.Forms.Panel()
        Me.optHalfSteinsetsCount04 = New System.Windows.Forms.RadioButton()
        Me.optHalfSteinsetsCount03 = New System.Windows.Forms.RadioButton()
        Me.optHalfSteinsetsCount02 = New System.Windows.Forms.RadioButton()
        Me.optHalfSteinsetsCount01 = New System.Windows.Forms.RadioButton()
        Me.lblInfoMitte = New System.Windows.Forms.Label()
        Me.lblInfoUnten = New System.Windows.Forms.Label()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.btnOK = New System.Windows.Forms.Button()
        Me.Num2UpDownSteineÜbereinander = New MahjongGK.Num2UpDown()
        Me.Num2UpDownSteineAufeinander = New MahjongGK.Num2UpDown()
        Me.Num2UpDownSteineNebeneinander = New MahjongGK.Num2UpDown()
        Me.Panel1.SuspendLayout()
        Me.Panel2.SuspendLayout()
        Me.Panel3.SuspendLayout()
        Me.SuspendLayout()
        '
        'lblInfoOben
        '
        Me.lblInfoOben.Location = New System.Drawing.Point(221, 57)
        Me.lblInfoOben.Name = "lblInfoOben"
        Me.lblInfoOben.Size = New System.Drawing.Size(355, 108)
        Me.lblInfoOben.TabIndex = 12
        Me.lblInfoOben.Text = resources.GetString("lblInfoOben.Text")
        '
        'txtName
        '
        Me.txtName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.txtName.Location = New System.Drawing.Point(15, 25)
        Me.txtName.MaxLength = 120
        Me.txtName.Name = "txtName"
        Me.txtName.Size = New System.Drawing.Size(544, 20)
        Me.txtName.TabIndex = 11
        Me.txtName.Text = "unbenannt"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(35, 129)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(96, 13)
        Me.Label6.TabIndex = 22
        Me.Label6.Text = "Lagen aufeinander"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(62, 102)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(69, 13)
        Me.Label5.TabIndex = 21
        Me.Label5.Text = "übereinander"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(11, 70)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(120, 26)
        Me.Label4.TabIndex = 20
        Me.Label4.Text = "STEINE nebeneinander" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "(Steine, nicht Felder)"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.Location = New System.Drawing.Point(12, 54)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(131, 13)
        Me.Label3.TabIndex = 16
        Me.Label3.Text = "Größe des Spielfeldes"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(12, 9)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(101, 13)
        Me.Label1.TabIndex = 23
        Me.Label1.Text = "Name des Spiels"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.Location = New System.Drawing.Point(13, 152)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(180, 13)
        Me.Label2.TabIndex = 24
        Me.Label2.Text = "Anzahl der Steine / Steinstrom"
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.optSteineStream)
        Me.Panel1.Controls.Add(Me.optSteineFix)
        Me.Panel1.Location = New System.Drawing.Point(16, 177)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(186, 52)
        Me.Panel1.TabIndex = 25
        '
        'optSteineStream
        '
        Me.optSteineStream.AutoSize = True
        Me.optSteineStream.Location = New System.Drawing.Point(9, 26)
        Me.optSteineStream.Name = "optSteineStream"
        Me.optSteineStream.Size = New System.Drawing.Size(149, 17)
        Me.optSteineStream.TabIndex = 1
        Me.optSteineStream.Text = "Kontinuierlicher Steinstrom"
        Me.optSteineStream.UseVisualStyleBackColor = True
        '
        'optSteineFix
        '
        Me.optSteineFix.AutoSize = True
        Me.optSteineFix.Checked = True
        Me.optSteineFix.Location = New System.Drawing.Point(9, 3)
        Me.optSteineFix.Name = "optSteineFix"
        Me.optSteineFix.Size = New System.Drawing.Size(139, 17)
        Me.optSteineFix.TabIndex = 0
        Me.optSteineFix.TabStop = True
        Me.optSteineFix.Text = "Fixe Anzahl von Steinen"
        Me.optSteineFix.UseVisualStyleBackColor = True
        '
        'Panel2
        '
        Me.Panel2.Controls.Add(Me.optBasis152)
        Me.Panel2.Controls.Add(Me.optBasis144)
        Me.Panel2.Location = New System.Drawing.Point(16, 238)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(186, 52)
        Me.Panel2.TabIndex = 26
        '
        'optBasis152
        '
        Me.optBasis152.AutoSize = True
        Me.optBasis152.Location = New System.Drawing.Point(9, 26)
        Me.optBasis152.Name = "optBasis152"
        Me.optBasis152.Size = New System.Drawing.Size(104, 17)
        Me.optBasis152.TabIndex = 1
        Me.optBasis152.Text = "Basis 152 Steine"
        Me.optBasis152.UseVisualStyleBackColor = True
        '
        'optBasis144
        '
        Me.optBasis144.AutoSize = True
        Me.optBasis144.Checked = True
        Me.optBasis144.Location = New System.Drawing.Point(9, 3)
        Me.optBasis144.Name = "optBasis144"
        Me.optBasis144.Size = New System.Drawing.Size(104, 17)
        Me.optBasis144.TabIndex = 0
        Me.optBasis144.TabStop = True
        Me.optBasis144.Text = "Basis 144 Steine"
        Me.optBasis144.UseVisualStyleBackColor = True
        '
        'Panel3
        '
        Me.Panel3.Controls.Add(Me.optHalfSteinsetsCount04)
        Me.Panel3.Controls.Add(Me.optHalfSteinsetsCount03)
        Me.Panel3.Controls.Add(Me.optHalfSteinsetsCount02)
        Me.Panel3.Controls.Add(Me.optHalfSteinsetsCount01)
        Me.Panel3.Location = New System.Drawing.Point(16, 302)
        Me.Panel3.Name = "Panel3"
        Me.Panel3.Size = New System.Drawing.Size(186, 100)
        Me.Panel3.TabIndex = 27
        '
        'optHalfSteinsetsCount04
        '
        Me.optHalfSteinsetsCount04.AutoSize = True
        Me.optHalfSteinsetsCount04.Location = New System.Drawing.Point(9, 72)
        Me.optHalfSteinsetsCount04.Name = "optHalfSteinsetsCount04"
        Me.optHalfSteinsetsCount04.Size = New System.Drawing.Size(99, 17)
        Me.optHalfSteinsetsCount04.TabIndex = 3
        Me.optHalfSteinsetsCount04.Text = "288/304 Steine"
        Me.optHalfSteinsetsCount04.UseVisualStyleBackColor = True
        '
        'optHalfSteinsetsCount03
        '
        Me.optHalfSteinsetsCount03.AutoSize = True
        Me.optHalfSteinsetsCount03.Location = New System.Drawing.Point(9, 49)
        Me.optHalfSteinsetsCount03.Name = "optHalfSteinsetsCount03"
        Me.optHalfSteinsetsCount03.Size = New System.Drawing.Size(99, 17)
        Me.optHalfSteinsetsCount03.TabIndex = 2
        Me.optHalfSteinsetsCount03.Text = "216/228 Steine"
        Me.optHalfSteinsetsCount03.UseVisualStyleBackColor = True
        '
        'optHalfSteinsetsCount02
        '
        Me.optHalfSteinsetsCount02.AutoSize = True
        Me.optHalfSteinsetsCount02.Checked = True
        Me.optHalfSteinsetsCount02.Location = New System.Drawing.Point(9, 26)
        Me.optHalfSteinsetsCount02.Name = "optHalfSteinsetsCount02"
        Me.optHalfSteinsetsCount02.Size = New System.Drawing.Size(99, 17)
        Me.optHalfSteinsetsCount02.TabIndex = 1
        Me.optHalfSteinsetsCount02.TabStop = True
        Me.optHalfSteinsetsCount02.Text = "144/152 Steine"
        Me.optHalfSteinsetsCount02.UseVisualStyleBackColor = True
        '
        'optHalfSteinsetsCount01
        '
        Me.optHalfSteinsetsCount01.AutoSize = True
        Me.optHalfSteinsetsCount01.Location = New System.Drawing.Point(9, 3)
        Me.optHalfSteinsetsCount01.Name = "optHalfSteinsetsCount01"
        Me.optHalfSteinsetsCount01.Size = New System.Drawing.Size(87, 17)
        Me.optHalfSteinsetsCount01.TabIndex = 0
        Me.optHalfSteinsetsCount01.Text = "72/76 Steine"
        Me.optHalfSteinsetsCount01.UseVisualStyleBackColor = True
        '
        'lblInfoMitte
        '
        Me.lblInfoMitte.Location = New System.Drawing.Point(221, 180)
        Me.lblInfoMitte.Name = "lblInfoMitte"
        Me.lblInfoMitte.Size = New System.Drawing.Size(355, 52)
        Me.lblInfoMitte.TabIndex = 28
        Me.lblInfoMitte.Text = "Diese Anzahl der Steine kann anschließend nicht mehr geändert werden." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Merke: die" &
    " fixe Anzahl von Steinen ist die maximale Anzahl von Steinen, die verlegt werden" &
    " kann. Weniger verlegen geht immer. "
        '
        'lblInfoUnten
        '
        Me.lblInfoUnten.Location = New System.Drawing.Point(221, 241)
        Me.lblInfoUnten.Name = "lblInfoUnten"
        Me.lblInfoUnten.Size = New System.Drawing.Size(355, 122)
        Me.lblInfoUnten.TabIndex = 29
        Me.lblInfoUnten.Text = resources.GetString("lblInfoUnten.Text")
        '
        'btnCancel
        '
        Me.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnCancel.Location = New System.Drawing.Point(459, 367)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(100, 32)
        Me.btnCancel.TabIndex = 30
        Me.btnCancel.Text = "Abbruch"
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'btnOK
        '
        Me.btnOK.Location = New System.Drawing.Point(353, 367)
        Me.btnOK.Name = "btnOK"
        Me.btnOK.Size = New System.Drawing.Size(100, 32)
        Me.btnOK.TabIndex = 31
        Me.btnOK.Text = "OK"
        Me.btnOK.UseVisualStyleBackColor = True
        '
        'Num2UpDownSteineÜbereinander
        '
        Me.Num2UpDownSteineÜbereinander.Location = New System.Drawing.Point(137, 93)
        Me.Num2UpDownSteineÜbereinander.MinimumSize = New System.Drawing.Size(65, 24)
        Me.Num2UpDownSteineÜbereinander.Name = "Num2UpDownSteineÜbereinander"
        Me.Num2UpDownSteineÜbereinander.Size = New System.Drawing.Size(65, 28)
        Me.Num2UpDownSteineÜbereinander.TabIndex = 19
        '
        'Num2UpDownSteineAufeinander
        '
        Me.Num2UpDownSteineAufeinander.Location = New System.Drawing.Point(137, 120)
        Me.Num2UpDownSteineAufeinander.MinimumSize = New System.Drawing.Size(65, 24)
        Me.Num2UpDownSteineAufeinander.Name = "Num2UpDownSteineAufeinander"
        Me.Num2UpDownSteineAufeinander.Size = New System.Drawing.Size(65, 28)
        Me.Num2UpDownSteineAufeinander.TabIndex = 18
        '
        'Num2UpDownSteineNebeneinander
        '
        Me.Num2UpDownSteineNebeneinander.Location = New System.Drawing.Point(137, 68)
        Me.Num2UpDownSteineNebeneinander.MinimumSize = New System.Drawing.Size(65, 24)
        Me.Num2UpDownSteineNebeneinander.Name = "Num2UpDownSteineNebeneinander"
        Me.Num2UpDownSteineNebeneinander.Size = New System.Drawing.Size(65, 28)
        Me.Num2UpDownSteineNebeneinander.TabIndex = 17
        '
        'frmSpielAnlegen
        '
        Me.AcceptButton = Me.btnOK
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.CancelButton = Me.btnCancel
        Me.ClientSize = New System.Drawing.Size(582, 402)
        Me.Controls.Add(Me.btnOK)
        Me.Controls.Add(Me.btnCancel)
        Me.Controls.Add(Me.lblInfoUnten)
        Me.Controls.Add(Me.lblInfoMitte)
        Me.Controls.Add(Me.Panel3)
        Me.Controls.Add(Me.Panel2)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Num2UpDownSteineÜbereinander)
        Me.Controls.Add(Me.Num2UpDownSteineAufeinander)
        Me.Controls.Add(Me.Num2UpDownSteineNebeneinander)
        Me.Controls.Add(Me.lblInfoOben)
        Me.Controls.Add(Me.txtName)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmSpielAnlegen"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "MahjongGK - Neues Spiel anlegen"
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        Me.Panel2.ResumeLayout(False)
        Me.Panel2.PerformLayout()
        Me.Panel3.ResumeLayout(False)
        Me.Panel3.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents lblInfoOben As Label
    Friend WithEvents txtName As TextBox
    Friend WithEvents Label6 As Label
    Friend WithEvents Label5 As Label
    Friend WithEvents Label4 As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents Num2UpDownSteineÜbereinander As Num2UpDown
    Friend WithEvents Num2UpDownSteineAufeinander As Num2UpDown
    Friend WithEvents Num2UpDownSteineNebeneinander As Num2UpDown
    Friend WithEvents Label1 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents Panel1 As Panel
    Friend WithEvents optSteineStream As RadioButton
    Friend WithEvents optSteineFix As RadioButton
    Friend WithEvents Panel2 As Panel
    Friend WithEvents optBasis152 As RadioButton
    Friend WithEvents optBasis144 As RadioButton
    Friend WithEvents Panel3 As Panel
    Friend WithEvents optHalfSteinsetsCount04 As RadioButton
    Friend WithEvents optHalfSteinsetsCount03 As RadioButton
    Friend WithEvents optHalfSteinsetsCount02 As RadioButton
    Friend WithEvents optHalfSteinsetsCount01 As RadioButton
    Friend WithEvents lblInfoMitte As Label
    Friend WithEvents lblInfoUnten As Label
    Friend WithEvents btnCancel As Button
    Friend WithEvents btnOK As Button
End Class
