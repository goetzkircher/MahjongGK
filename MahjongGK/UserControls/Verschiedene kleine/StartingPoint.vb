Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

'
''' <summary>
''' Auswahl-Control für den Startpunkt auf einem 9x9 Raster (16x16 Zellen).
''' Aktive Punkte: Ecken, Kantenmitten, Center sowie CenterLO/RO/RU/LU.
''' Hover = PunktMover, Selektiert = PunktSel, sonst Punkt.
''' </summary>
Public Class StartingPoint
    Inherits UserControl

    ' Ressourcen-Grafiken:
    Private ReadOnly _bmpNormal As Bitmap = My.Resources.Punkt
    Private ReadOnly _bmpSel As Bitmap = My.Resources.PunktSel
    Private ReadOnly _bmpMover As Bitmap = My.Resources.PunktMover

    Private Const CELL As Integer = 16
    Private Const GRID As Integer = 9

    Public Enum PositionEnum
        EckeLO
        EckeRO
        EckeRU
        EckeLU
        MitteO
        MitteR
        MitteU
        MitteL
        CenterLO
        CenterRO
        CenterRU
        CenterLU
        Center
    End Enum

    Private ReadOnly _map As New Dictionary(Of PictureBox, PositionEnum)
    Private _selected As PositionEnum = PositionEnum.Center

    '
    ''' <summary>Feuert, wenn sich die Auswahl ändert.</summary>
    Public Event StartingPointChanged(ByVal newValue As PositionEnum)

    '
    ''' <summary>Aktuell gewählter Startpunkt.</summary>
    Public Property SelectedStartingPoint As PositionEnum
        Get
            Return _selected
        End Get
        Set(value As PositionEnum)
            If _selected <> value Then
                _selected = value
                UpdateVisuals()
                RaiseEvent StartingPointChanged(_selected)
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

        ' Alle Zellen erzeugen (tight packed)
        Dim coordsToPos As New Dictionary(Of Point, PositionEnum) From {
            {New Point(0, 0), PositionEnum.EckeLO},
            {New Point(8, 0), PositionEnum.EckeRO},
            {New Point(8, 8), PositionEnum.EckeRU},
            {New Point(0, 8), PositionEnum.EckeLU},
                                                   _
            {New Point(4, 0), PositionEnum.MitteO},
            {New Point(8, 4), PositionEnum.MitteR},
            {New Point(4, 8), PositionEnum.MitteU},
            {New Point(0, 4), PositionEnum.MitteL},
                                                   _
            {New Point(4, 4), PositionEnum.Center},
                                                   _
            {New Point(2, 2), PositionEnum.CenterLO},
            {New Point(6, 2), PositionEnum.CenterRO},
            {New Point(6, 6), PositionEnum.CenterRU},
            {New Point(2, 6), PositionEnum.CenterLU}
        }

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

                Dim key As New Point(x, y)
                If coordsToPos.ContainsKey(key) Then
                    Dim pos As PositionEnum = coordsToPos(key)
                    _map(pb) = pos
                    pb.Image = If(pos = _selected, _bmpSel, _bmpNormal)
                    pb.Cursor = Cursors.Hand
                    AddHandler pb.MouseEnter, AddressOf Pb_MouseEnter
                    AddHandler pb.MouseLeave, AddressOf Pb_MouseLeave
                    AddHandler pb.Click, AddressOf Pb_Click
                Else
                    pb.Enabled = False
                End If
            Next
        Next

        ResumeLayout(False)
    End Sub

    Private Sub Pb_Click(sender As Object, e As EventArgs)
        Dim pb As PictureBox = DirectCast(sender, PictureBox)
        Dim pos As PositionEnum = _map(pb)
        SelectedStartingPoint = pos
    End Sub

    Private Sub Pb_MouseEnter(sender As Object, e As EventArgs)
        Dim pb As PictureBox = DirectCast(sender, PictureBox)
        Dim pos As PositionEnum = _map(pb)
        If _selected = pos Then
            pb.Image = _bmpSel
        Else
            pb.Image = _bmpMover
        End If
    End Sub

    Private Sub Pb_MouseLeave(sender As Object, e As EventArgs)
        Dim pb As PictureBox = DirectCast(sender, PictureBox)
        Dim pos As PositionEnum = _map(pb)
        If _selected = pos Then
            pb.Image = _bmpSel
        Else
            pb.Image = _bmpNormal
        End If
    End Sub

    Private Sub UpdateVisuals()
        For Each kvp As KeyValuePair(Of PictureBox, PositionEnum) In _map
            kvp.Key.Image = If(kvp.Value = _selected, _bmpSel, _bmpNormal)
        Next
    End Sub
End Class

