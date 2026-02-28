<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UCtlWerkbank
    Inherits System.Windows.Forms.UserControl

    'UserControl überschreibt den Löschvorgang, um die Komponentenliste zu bereinigen.
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
        Me.chkZusatzfunktion = New System.Windows.Forms.CheckBox()
        Me.gbxX = New System.Windows.Forms.GroupBox()
        Me.gbxY = New System.Windows.Forms.GroupBox()
        Me.gbxZ = New System.Windows.Forms.GroupBox()
        Me.gbxRichtung = New System.Windows.Forms.GroupBox()
        Me.gbxZusatzfunktion = New System.Windows.Forms.GroupBox()
        Me.lblMsg = New System.Windows.Forms.Label()
        Me.KompassRoseRichtung = New MahjongGK.KompassRoseX()
        Me.num2UpDnZ = New MahjongGK.Num2UpDown()
        Me.num2UpDnY = New MahjongGK.Num2UpDown()
        Me.num2UpDnX = New MahjongGK.Num2UpDown()
        Me.ButtonTooltip1 = New MahjongGK.ButtonTooltip()
        Me.gbxX.SuspendLayout()
        Me.gbxY.SuspendLayout()
        Me.gbxZ.SuspendLayout()
        Me.gbxRichtung.SuspendLayout()
        Me.gbxZusatzfunktion.SuspendLayout()
        Me.SuspendLayout()
        '
        'chkZusatzfunktion
        '
        Me.chkZusatzfunktion.AutoSize = True
        Me.chkZusatzfunktion.Location = New System.Drawing.Point(46, 22)
        Me.chkZusatzfunktion.Name = "chkZusatzfunktion"
        Me.chkZusatzfunktion.Size = New System.Drawing.Size(114, 17)
        Me.chkZusatzfunktion.TabIndex = 39
        Me.chkZusatzfunktion.Text = "chkZusatzfunktion"
        Me.chkZusatzfunktion.UseVisualStyleBackColor = True
        '
        'gbxX
        '
        Me.gbxX.Controls.Add(Me.num2UpDnX)
        Me.gbxX.Controls.Add(Me.ButtonTooltip1)
        Me.gbxX.Location = New System.Drawing.Point(13, 10)
        Me.gbxX.Name = "gbxX"
        Me.gbxX.Size = New System.Drawing.Size(168, 47)
        Me.gbxX.TabIndex = 40
        Me.gbxX.TabStop = False
        Me.gbxX.Text = "gbxX"
        '
        'gbxY
        '
        Me.gbxY.Controls.Add(Me.num2UpDnY)
        Me.gbxY.Location = New System.Drawing.Point(13, 63)
        Me.gbxY.Name = "gbxY"
        Me.gbxY.Size = New System.Drawing.Size(168, 47)
        Me.gbxY.TabIndex = 41
        Me.gbxY.TabStop = False
        Me.gbxY.Text = "gbxY"
        '
        'gbxZ
        '
        Me.gbxZ.Controls.Add(Me.num2UpDnZ)
        Me.gbxZ.Location = New System.Drawing.Point(13, 116)
        Me.gbxZ.Name = "gbxZ"
        Me.gbxZ.Size = New System.Drawing.Size(168, 47)
        Me.gbxZ.TabIndex = 42
        Me.gbxZ.TabStop = False
        Me.gbxZ.Text = "gbxZ"
        '
        'gbxRichtung
        '
        Me.gbxRichtung.Controls.Add(Me.KompassRoseRichtung)
        Me.gbxRichtung.Location = New System.Drawing.Point(13, 222)
        Me.gbxRichtung.Name = "gbxRichtung"
        Me.gbxRichtung.Size = New System.Drawing.Size(200, 148)
        Me.gbxRichtung.TabIndex = 43
        Me.gbxRichtung.TabStop = False
        Me.gbxRichtung.Text = "Richtung"
        '
        'gbxZusatzfunktion
        '
        Me.gbxZusatzfunktion.Controls.Add(Me.chkZusatzfunktion)
        Me.gbxZusatzfunktion.Location = New System.Drawing.Point(13, 169)
        Me.gbxZusatzfunktion.Name = "gbxZusatzfunktion"
        Me.gbxZusatzfunktion.Size = New System.Drawing.Size(168, 47)
        Me.gbxZusatzfunktion.TabIndex = 44
        Me.gbxZusatzfunktion.TabStop = False
        Me.gbxZusatzfunktion.Text = "Zuastzfunktion"
        '
        'lblMsg
        '
        Me.lblMsg.Location = New System.Drawing.Point(188, 17)
        Me.lblMsg.Name = "lblMsg"
        Me.lblMsg.Size = New System.Drawing.Size(88, 146)
        Me.lblMsg.TabIndex = 46
        Me.lblMsg.Text = "lblMsg"
        '
        'KompassRoseRichtung
        '
        Me.KompassRoseRichtung.Direction = MahjongGK.KompassRoseX.KompassXEnum.None
        Me.KompassRoseRichtung.Location = New System.Drawing.Point(38, 15)
        Me.KompassRoseRichtung.MinimumSize = New System.Drawing.Size(80, 80)
        Me.KompassRoseRichtung.Name = "KompassRoseRichtung"
        Me.KompassRoseRichtung.Size = New System.Drawing.Size(130, 130)
        Me.KompassRoseRichtung.TabIndex = 37
        '
        'num2UpDnZ
        '
        Me.num2UpDnZ.Location = New System.Drawing.Point(40, 13)
        Me.num2UpDnZ.MinimumSize = New System.Drawing.Size(65, 24)
        Me.num2UpDnZ.Name = "num2UpDnZ"
        Me.num2UpDnZ.Size = New System.Drawing.Size(65, 28)
        Me.num2UpDnZ.TabIndex = 35
        '
        'num2UpDnY
        '
        Me.num2UpDnY.Location = New System.Drawing.Point(47, 13)
        Me.num2UpDnY.MinimumSize = New System.Drawing.Size(65, 24)
        Me.num2UpDnY.Name = "num2UpDnY"
        Me.num2UpDnY.Size = New System.Drawing.Size(65, 28)
        Me.num2UpDnY.TabIndex = 34
        '
        'num2UpDnX
        '
        Me.num2UpDnX.Location = New System.Drawing.Point(47, 13)
        Me.num2UpDnX.MinimumSize = New System.Drawing.Size(65, 24)
        Me.num2UpDnX.Name = "num2UpDnX"
        Me.num2UpDnX.Size = New System.Drawing.Size(65, 28)
        Me.num2UpDnX.TabIndex = 32
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
        Me.ButtonTooltip1.Location = New System.Drawing.Point(133, 15)
        Me.ButtonTooltip1.MinimumSize = New System.Drawing.Size(16, 16)
        Me.ButtonTooltip1.Name = "ButtonTooltip1"
        Me.ButtonTooltip1.ShowOnFocus = False
        Me.ButtonTooltip1.Size = New System.Drawing.Size(26, 26)
        Me.ButtonTooltip1.TabIndex = 36
        Me.ButtonTooltip1.TabStop = False
        Me.ButtonTooltip1.Text = "ButtonTooltip1"
        '
        'UCtlWerkbank
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.lblMsg)
        Me.Controls.Add(Me.gbxZusatzfunktion)
        Me.Controls.Add(Me.gbxRichtung)
        Me.Controls.Add(Me.gbxZ)
        Me.Controls.Add(Me.gbxY)
        Me.Controls.Add(Me.gbxX)
        Me.Name = "UCtlWerkbank"
        Me.Size = New System.Drawing.Size(280, 390)
        Me.gbxX.ResumeLayout(False)
        Me.gbxY.ResumeLayout(False)
        Me.gbxZ.ResumeLayout(False)
        Me.gbxRichtung.ResumeLayout(False)
        Me.gbxZusatzfunktion.ResumeLayout(False)
        Me.gbxZusatzfunktion.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents chkZusatzfunktion As CheckBox
    Friend WithEvents KompassRoseRichtung As KompassRoseX
    Friend WithEvents ButtonTooltip1 As ButtonTooltip
    Friend WithEvents num2UpDnZ As Num2UpDown
    Friend WithEvents num2UpDnY As Num2UpDown
    Friend WithEvents num2UpDnX As Num2UpDown
    Friend WithEvents gbxX As GroupBox
    Friend WithEvents gbxY As GroupBox
    Friend WithEvents gbxZ As GroupBox
    Friend WithEvents gbxRichtung As GroupBox
    Friend WithEvents gbxZusatzfunktion As GroupBox
    Friend WithEvents lblMsg As Label
End Class
