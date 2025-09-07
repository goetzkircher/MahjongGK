Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

Imports System.ComponentModel

'
''' <summary>
''' KompassRose-UserControl mit Hover-Effekt.
''' Ressourcen je Richtung: "Compass" + {N,NO,O,SO,S,SW,W,NW} [+ "mover" | "sel"].
''' Mitte hat immer einen Punkt (Ressource "CompassCenter" oder generiert).
''' </summary>
Public Class KompassRose
    Inherits UserControl

    '--------------------------- Konstanten ---------------------------
    Private Const IMG_SIZE As Integer = 16
    Private Const DEFAULT_PREFIX As String = "Compass"
    Private Const SUFFIX_SELECTED As String = "sel"
    Private Const SUFFIX_MOVER As String = "mover"

    ' Abstand zwischen Zellen (0 = dicht an dicht)
    <Browsable(True), DefaultValue(0)>
    Public Property Spacing As Integer = 0

    ' Prefix konfigurierbar (falls du andere Namen nutzt)
    <Browsable(True), DefaultValue(DEFAULT_PREFIX)>
    Public Property ResourcePrefix As String = DEFAULT_PREFIX

    Private ReadOnly _pbox(2, 2) As PictureBox
    Private ReadOnly _mapPbToDir As New Dictionary(Of PictureBox, KompassEnum)

    Private _centerDotBmp As Bitmap = Nothing
    Private _direction As KompassEnum = KompassEnum.None
    Private _hoverDir As KompassEnum = KompassEnum.None

    '--------------------------- Enum ---------------------------


    '--------------------------- Ereignis ---------------------------
    Public Event DirectionChanged(sender As Object, direction As KompassEnum)

    '--------------------------- Property: Direction ---------------------------
    <Browsable(True), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)>
    Public Property Direction As KompassEnum
        Get
            Return _direction
        End Get
        Set(value As KompassEnum)
            _direction = value
            UpdateVisuals()
            ' Immer feuern – auch wenn derselbe Wert erneut zugewiesen wird.
            RaiseEvent DirectionChanged(Me, _direction)
        End Set
    End Property

    '--------------------------- ctor ---------------------------
    Public Sub New()
        MyBase.New()
        SetStyle(ControlStyles.AllPaintingInWmPaint Or
                 ControlStyles.OptimizedDoubleBuffer Or
                 ControlStyles.UserPaint, True)
        BackColor = Color.Transparent

        ' 3×3 PictureBoxes erstellen
        For row As Integer = 0 To 2
            For col As Integer = 0 To 2
                Dim pb As New PictureBox() With {
                    .SizeMode = PictureBoxSizeMode.CenterImage,
                    .Width = IMG_SIZE, .Height = IMG_SIZE,
                    .BackColor = Color.Transparent,
                    .TabStop = False
                }
                AddHandler pb.Click, AddressOf OnPbClick
                AddHandler pb.MouseEnter, AddressOf OnPbMouseEnter
                AddHandler pb.MouseLeave, AddressOf OnPbMouseLeave
                _pbox(row, col) = pb
                Controls.Add(pb)
            Next
        Next

        ' Mitte (1,1) – immer sichtbar mit Punkt (nicht klickbar)
        _pbox(1, 1).Enabled = False

        ' Positionen -> Richtungen
        _mapPbToDir(_pbox(0, 0)) = KompassEnum.NW
        _mapPbToDir(_pbox(0, 1)) = KompassEnum.N
        _mapPbToDir(_pbox(0, 2)) = KompassEnum.NO
        _mapPbToDir(_pbox(1, 0)) = KompassEnum.W
        ' _pbox(1,1) = Mitte (None) – kein Eintrag
        _mapPbToDir(_pbox(1, 2)) = KompassEnum.O
        _mapPbToDir(_pbox(2, 0)) = KompassEnum.SW
        _mapPbToDir(_pbox(2, 1)) = KompassEnum.S
        _mapPbToDir(_pbox(2, 2)) = KompassEnum.SO

        UpdateLayoutAndSize()
        UpdateVisuals()
    End Sub

    Protected Overrides Sub Dispose(disposing As Boolean)
        If disposing Then
            If _centerDotBmp IsNot Nothing Then
                _centerDotBmp.Dispose()
                _centerDotBmp = Nothing
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub

    '--------------------------- Layout ---------------------------
    Protected Overrides Sub OnSizeChanged(e As EventArgs)
        MyBase.OnSizeChanged(e)
        UpdateLayoutAndSize()
    End Sub

    Private Sub UpdateLayoutAndSize()
        Dim cell As Integer = IMG_SIZE
        Dim s As Integer = Spacing

        Dim totalW As Integer = 3 * cell + 2 * s
        Dim totalH As Integer = 3 * cell + 2 * s

        'Die <> durch < ersetzen, dann kann das Control größer sein. 
        If Width <> totalW Then Width = totalW
        If Height <> totalH Then Height = totalH

        Dim startX As Integer = (Width - totalW) \ 2
        Dim startY As Integer = (Height - totalH) \ 2

        For row As Integer = 0 To 2
            For col As Integer = 0 To 2
                Dim pb As PictureBox = _pbox(row, col)
                pb.Left = startX + col * (cell + s)
                pb.Top = startY + row * (cell + s)
                pb.Width = cell
                pb.Height = cell
            Next
        Next

        ' Mitte immer mit Punkt
        EnsureCenterDot()
        _pbox(1, 1).Image = _centerDotBmp
        _pbox(1, 1).Visible = True
    End Sub

    '--------------------------- Maus-Handling ---------------------------
    Private Sub OnPbClick(sender As Object, e As EventArgs)
        Dim pb As PictureBox = DirectCast(sender, PictureBox)
        Dim dir As KompassEnum
        If _mapPbToDir.TryGetValue(pb, dir) Then
            Me.Direction = dir
        End If
    End Sub

    Private Sub OnPbMouseEnter(sender As Object, e As EventArgs)
        Dim pb As PictureBox = DirectCast(sender, PictureBox)
        Dim dir As KompassEnum
        If _mapPbToDir.TryGetValue(pb, dir) Then
            _hoverDir = dir
            UpdateVisuals()
        End If
    End Sub

    Private Sub OnPbMouseLeave(sender As Object, e As EventArgs)
        ' Wenn der Mauszeiger das jeweilige Feld verlässt, Hover löschen,
        ' das nächste Feld setzt es in dessen MouseEnter sofort wieder.
        _hoverDir = KompassEnum.None
        UpdateVisuals()
    End Sub

    '--------------------------- Visuals ---------------------------
    Private Sub UpdateVisuals()
        ' Alle 8 Richtungen setzen: mover > sel > normal
        For Each kvp As KeyValuePair(Of PictureBox, KompassEnum) In _mapPbToDir
            Dim pb As PictureBox = kvp.Key
            Dim dir As KompassEnum = kvp.Value

            Dim img As Image
            If dir = _hoverDir Then
                img = GetDirectionBitmap(dir, state:="mover")
            ElseIf dir = _direction Then
                img = GetDirectionBitmap(dir, state:="sel")
            Else
                img = GetDirectionBitmap(dir, state:="")
            End If

            pb.Image = img
        Next

        ' Mitte (Punkt) beibehalten
        If _pbox(1, 1).Image Is Nothing Then
            EnsureCenterDot()
            _pbox(1, 1).Image = _centerDotBmp
        End If

        Invalidate()
    End Sub

    Private Function GetDirectionBitmap(dir As KompassEnum, state As String) As Image
        If dir = KompassEnum.None Then Return Nothing
        Dim name As String = ResourcePrefix & DirToToken(dir) & state
        Dim obj As Object = My.Resources.ResourceManager.GetObject(name)
        Dim bmp As Bitmap = TryCast(obj, Bitmap)

        If bmp IsNot Nothing Then Return bmp

        ' Fallback: 16×16 transparent
        Dim ph As New Bitmap(IMG_SIZE, IMG_SIZE)
        Using g As Graphics = Graphics.FromImage(ph)
            g.Clear(Color.Transparent)
        End Using
        Return ph
    End Function

    Private Shared Function DirToToken(dir As KompassEnum) As String
        Select Case dir
            Case KompassEnum.N : Return "N"
            Case KompassEnum.NO : Return "NO"
            Case KompassEnum.O : Return "O"
            Case KompassEnum.SO : Return "SO"
            Case KompassEnum.S : Return "S"
            Case KompassEnum.SW : Return "SW"
            Case KompassEnum.W : Return "W"
            Case KompassEnum.NW : Return "NW"
            Case Else : Return String.Empty
        End Select
    End Function

    '--------------------------- Center Dot ---------------------------
    Private Sub EnsureCenterDot()
        If _centerDotBmp IsNot Nothing Then Return

        ' 1) Versuche Ressource
        Dim obj As Object = My.Resources.ResourceManager.GetObject("CompassCenter")
        Dim bmp As Bitmap = TryCast(obj, Bitmap)

        ' 2) Fallback: kleinen schwarzen Punkt generieren
        If bmp Is Nothing Then
            bmp = New Bitmap(IMG_SIZE, IMG_SIZE)
            Using g As Graphics = Graphics.FromImage(bmp)
                g.Clear(Color.Transparent)
                Dim r As Integer = 2
                Dim cx As Integer = IMG_SIZE \ 2 - 1
                Dim cy As Integer = IMG_SIZE \ 2 - 1
                g.FillEllipse(Brushes.Black, cx - r, cy - r, 2 * r + 1, 2 * r + 1)
            End Using
        End If

        _centerDotBmp = bmp
    End Sub

End Class
