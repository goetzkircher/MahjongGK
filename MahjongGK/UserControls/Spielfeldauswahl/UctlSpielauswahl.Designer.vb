<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class UctlSpielauswahl
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
        Me.hsb = New System.Windows.Forms.HScrollBar()
        Me.vsb = New System.Windows.Forms.VScrollBar()
        Me.SuspendLayout()
        '
        'hsb
        '
        Me.hsb.Location = New System.Drawing.Point(61, 37)
        Me.hsb.Name = "hsb"
        Me.hsb.Size = New System.Drawing.Size(534, 17)
        Me.hsb.TabIndex = 0
        '
        'vsb
        '
        Me.vsb.Location = New System.Drawing.Point(212, 84)
        Me.vsb.Name = "vsb"
        Me.vsb.Size = New System.Drawing.Size(17, 174)
        Me.vsb.TabIndex = 1
        '
        'UctlSpielauswahl
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.vsb)
        Me.Controls.Add(Me.hsb)
        Me.Name = "UctlSpielauswahl"
        Me.Size = New System.Drawing.Size(634, 331)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents hsb As HScrollBar
    Friend WithEvents vsb As VScrollBar
End Class
