Imports MahjongGK.Contracts
Imports MahjongGK.Spielfeld

Public Class frmSpielAnlegen
    Private Sub frmSpielAnlegen_Load(sender As Object, e As EventArgs) Handles Me.Load

        SetOptEnDisableAndTexte()

        With lblInfoOben
            .Text = .Text.Replace("xx", GlobalConstants.MJ_STEINE_MAXX.ToString)
            .Text = .Text.Replace("yy", GlobalConstants.MJ_STEINE_MAXY.ToString)
            .Text = .Text.Replace("zz", GlobalConstants.MJ_STEINE_MAXZ.ToString)
        End With

        With Num2UpDownSteineNebeneinander
            .MinValue = 1
            .MaxValue = GlobalConstants.MJ_STEINE_MAXX
            .Value = 12
        End With

        With Num2UpDownSteineAufeinander
            .MinValue = 1
            .MaxValue = GlobalConstants.MJ_STEINE_MAXY
            .Value = 6
        End With

        With Num2UpDownSteineÜbereinander
            .MinValue = 1
            .MaxValue = GlobalConstants.MJ_STEINE_MAXZ
            .Value = 4
        End With
    End Sub

    Private Sub btnOK_Click(sender As Object, e As EventArgs) Handles btnOK.Click
        Me.DialogResult = DialogResult.OK
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Me.DialogResult = DialogResult.Cancel
    End Sub

    Private Sub opt_CheckedChanged(sender As Object, e As EventArgs) _
        Handles optBasis144.CheckedChanged,
                optBasis152.CheckedChanged,
                optSteineFix.CheckedChanged,
                optSteineStream.CheckedChanged,
                optHalfSteinsetsCount01.CheckedChanged,
                optHalfSteinsetsCount02.CheckedChanged,
                optHalfSteinsetsCount03.CheckedChanged,
                optHalfSteinsetsCount04.CheckedChanged

        SetOptEnDisableAndTexte()

    End Sub

    Private Sub SetOptEnDisableAndTexte()

        Dim enabled As Boolean = optSteineFix.Checked
        optHalfSteinsetsCount01.Enabled = enabled
        optHalfSteinsetsCount02.Enabled = enabled
        optHalfSteinsetsCount03.Enabled = enabled
        optHalfSteinsetsCount04.Enabled = enabled

        If optBasis144.Checked Then
            optHalfSteinsetsCount01.Text = " 72 Steine"
            optHalfSteinsetsCount02.Text = "144 Steine"
            optHalfSteinsetsCount03.Text = "216 Steine"
            optHalfSteinsetsCount04.Text = "288 Steine"
        Else
            optHalfSteinsetsCount01.Text = " 76 Steine"
            optHalfSteinsetsCount02.Text = "152 Steine"
            optHalfSteinsetsCount03.Text = "228 Steine"
            optHalfSteinsetsCount04.Text = "304 Steine"
        End If

    End Sub

    Public ReadOnly Property GetGeneratorModus As GeneratorModus
        Get
            Dim count As Integer
            If optHalfSteinsetsCount01.Checked Then
                count = 0
            ElseIf optHalfSteinsetsCount02.Checked Then
                count = 1
            ElseIf optHalfSteinsetsCount03.Checked Then
                count = 2
            Else 'If optHalfSteinsetsCount04.Checked Then
                count = 3
            End If

            Return SpielsteinGenerator.GetGeneratorModus(optSteineFix.Checked, optBasis152.Checked, count)
        End Get
    End Property

    Public ReadOnly Property GetName As String
        Get
            Return txtName.Text.Trim
        End Get
    End Property

    Public ReadOnly Property GetSpielfeldSize As Triple
        Get
            Return New Triple(Num2UpDownSteineNebeneinander.Value, Num2UpDownSteineÜbereinander.Value, Num2UpDownSteineAufeinander.Value)
        End Get
    End Property
End Class