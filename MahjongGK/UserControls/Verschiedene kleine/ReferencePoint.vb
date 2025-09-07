Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

'
''' <summary>
''' Auswahl-Control für einen Referenzpunkt (Ecken + Center) auf einem 3x3 Raster.
''' Felder sind 16x16 px; Hover = PunktMover, Selektiert = PunktSel, sonst Punkt.
''' </summary>
Public Class ReferencePoint

    Inherits UserControl

    ' Ressourcen-Grafiken:
    Private ReadOnly _bmpNormal As Bitmap = My.Resources.Punkt
    Private ReadOnly _bmpSel As Bitmap = My.Resources.PunktSel
    Private ReadOnly _bmpMover As Bitmap = My.Resources.PunktMover

    Private Const CELL As Integer = 16
    Private Const GRID As Integer = 3

    Public Enum RefPoint
        None
        EckeLO
        EckeRO
        EckeRU
        EckeLU
        Center
    End Enum

    Private ReadOnly _map As New Dictionary(Of PictureBox, RefPoint)
    Private _selected As RefPoint = RefPoint.None

    '
    ''' <summary>Feuert, wenn sich die Auswahl ändert.</summary>
    Public Event ReferencePointChanged(ByVal newValue As RefPoint)

    '
    ''' <summary>Aktuell gewählter Referenzpunkt.</summary>
    Public Property SelectedReferencePoint As RefPoint
        Get
            Return _selected
        End Get
        Set(value As RefPoint)
            If _selected <> value Then
                _selected = value
                UpdateVisuals()
                RaiseEvent ReferencePointChanged(_selected)
            End If
        End Set
    End Property

    Public Sub New()
        DoubleBuffered = True
        SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.UserPaint Or ControlStyles.OptimizedDoubleBuffer, True)
        Size = New Size(GRID * CELL, GRID * CELL)
        CreateGrid()
    End Sub

    Private Sub CreateGrid()
        SuspendLayout()

        ' 3x3 PictureBoxes, aber nur Ecken + Mitte sind aktiv
        For y As Integer = 0 To GRID - 1
            For x As Integer = 0 To GRID - 1
                Dim pb As New PictureBox() With {
                    .SizeMode = PictureBoxSizeMode.CenterImage,
                    .Width = CELL,
                    .Height = CELL,
                    .Left = x * CELL,
                    .Top = y * CELL,
                    .BackColor = Color.Transparent
                }
                Controls.Add(pb)

                Dim rp As RefPoint = RefPoint.None
                If x = 0 AndAlso y = 0 Then rp = RefPoint.EckeLO
                If x = GRID - 1 AndAlso y = 0 Then rp = RefPoint.EckeRO
                If x = GRID - 1 AndAlso y = GRID - 1 Then rp = RefPoint.EckeRU
                If x = 0 AndAlso y = GRID - 1 Then rp = RefPoint.EckeLU
                If x = 1 AndAlso y = 1 Then rp = RefPoint.Center

                If rp <> RefPoint.None Then
                    _map(pb) = rp
                    pb.Image = _bmpNormal
                    AddHandler pb.MouseEnter, AddressOf Pb_MouseEnter
                    AddHandler pb.MouseLeave, AddressOf Pb_MouseLeave
                    AddHandler pb.Click, AddressOf Pb_Click
                    pb.Cursor = Cursors.Hand
                Else
                    pb.Enabled = False
                End If
            Next
        Next

        ResumeLayout(False)
    End Sub

    Private Sub Pb_Click(sender As Object, e As EventArgs)
        Dim pb As PictureBox = DirectCast(sender, PictureBox)
        Dim rp As RefPoint = _map(pb)
        SelectedReferencePoint = rp
    End Sub

    Private Sub Pb_MouseEnter(sender As Object, e As EventArgs)
        Dim pb As PictureBox = DirectCast(sender, PictureBox)
        Dim rp As RefPoint = _map(pb)
        If _selected = rp Then
            pb.Image = _bmpSel
        Else
            pb.Image = _bmpMover
        End If
    End Sub

    Private Sub Pb_MouseLeave(sender As Object, e As EventArgs)
        Dim pb As PictureBox = DirectCast(sender, PictureBox)
        Dim rp As RefPoint = _map(pb)
        If _selected = rp Then
            pb.Image = _bmpSel
        Else
            pb.Image = _bmpNormal
        End If
    End Sub

    Private Sub UpdateVisuals()
        For Each kvp As KeyValuePair(Of PictureBox, RefPoint) In _map
            kvp.Key.Image = If(kvp.Value = _selected, _bmpSel, _bmpNormal)
        Next
    End Sub
End Class

