Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

'Positionierer auf 9×9 Raster (16×16 px je Zelle):
'StartingPoint (13 selektierbare Punkte) bleibt wie gehabt: Ecken, Kantenmitten, Center und die vier „CenterLO/RO/RU/LU“.
'KompassRose wird in das 9×9 eingebaut: die 8 Pfeile sitzen direkt rings um das Center in den 8 Nachbarzellen.
'Wichtig: Kein Kompass-Center-Punkt. Der funktionale Punkt ist der StartingPoint-Center.
'Pfeile = Moment-Buttons: jeder Klick sendet ein Event mit Richtung und Δ (StepX/StepY).
'Punkt-Zellen: Hover = PunktMover, Selektiert = PunktSel, sonst Punkt.
'Events:
'StartingPointChanged(newValue As PositionEnum)
'DirectionClicked(direction As Kompass, dx As Integer, dy As Integer, newOffsetX As Integer, newOffsetY As Integer)
'OffsetsChanged(offsetX As Integer, offsetY As Integer)

Public Class Positionierer
    Inherits UserControl

    Private Const LINE_THICK As Integer = 2

    ' ---------- Enums ----------
    Public Enum Kompass
        None
        N
        NO
        O
        SO
        S
        SW
        W
        NW
    End Enum

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

    ' ---------- Events ----------
    Public Event StartingPointChanged(newValue As PositionEnum)
    Public Event DirectionClicked(direction As Kompass, dx As Integer, dy As Integer, newOffsetX As Integer, newOffsetY As Integer)
    Public Event DirectionIncrement(dx As Integer, dy As Integer)
    Public Event OffsetsChanged(offsetX As Integer, offsetY As Integer)

    ' ---------- Public Properties ----------
    Public Property AutoUpdateOffsets As Boolean = True

    Private _offsetX As Integer
    Public Property OffsetX As Integer
        Get
            Return _offsetX
        End Get
        Set(value As Integer)
            If _offsetX <> value Then
                _offsetX = value
                RaiseEvent OffsetsChanged(_offsetX, _offsetY)
            End If
        End Set
    End Property

    Private _offsetY As Integer
    Public Property OffsetY As Integer
        Get
            Return _offsetY
        End Get
        Set(value As Integer)
            If _offsetY <> value Then
                _offsetY = value
                RaiseEvent OffsetsChanged(_offsetX, _offsetY)
            End If
        End Set
    End Property

    Private _stepX As Integer = 1
    Public Property StepX As Integer
        Get
            Return _stepX
        End Get
        Set(value As Integer)
            _stepX = Math.Max(1, Math.Abs(value))
        End Set
    End Property

    Private _stepY As Integer = 1
    Public Property StepY As Integer
        Get
            Return _stepY
        End Get
        Set(value As Integer)
            _stepY = Math.Max(1, Math.Abs(value))
        End Set
    End Property

    ' Auto-Repeat Tuning
    Private _initialRepeatDelayMs As Integer = 350
    Public Property InitialRepeatDelayMs As Integer
        Get
            Return _initialRepeatDelayMs
        End Get
        Set(value As Integer)
            _initialRepeatDelayMs = Math.Max(0, value)
        End Set
    End Property

    Private _repeatIntervalMs As Integer = 80
    Public Property RepeatIntervalMs As Integer
        Get
            Return _repeatIntervalMs
        End Get
        Set(value As Integer)
            _repeatIntervalMs = Math.Max(10, value)
        End Set
    End Property

    ' ---------- Ressourcen ----------
    Private ReadOnly _pt As Bitmap = KompassDotFactory.KompassDot  'My.Resources.Punkt
    Private ReadOnly _ptSel As Bitmap = KompassDotFactory.KompassDotSelected  'My.Resources.PunktSel
    Private ReadOnly _ptMover As Bitmap = KompassDotFactory.KompassDotMouseOver  'My.Resources.PunktMover

    Private Function ArrowNormal(d As Kompass) As Bitmap
        If d = Kompass.None Then Return Nothing
        Dim resName As String = "Compass" & d.ToString()
        Return Theme.GetResBmp(resName)
    End Function

    Private Function ArrowHover(d As Kompass) As Bitmap
        If d = Kompass.None Then Return Nothing
        Dim resName As String = "Compass" & d.ToString() & "mover"
        Return Theme.GetResBmp(resName)
    End Function
    Private Function ArrowSel(d As Kompass) As Bitmap
        If d = Kompass.None Then Return Nothing
        Dim resName As String = "Compass" & d.ToString() & "mover"
        Return Theme.GetResBmp(resName)
    End Function


    ' ---------- Layout ----------
    Private Const CELL As Integer = 16
    Private Const GRID As Integer = 7
    Private Const C As Integer = 3 ' center index 0..8

    Private ReadOnly _startMap As New Dictionary(Of PictureBox, PositionEnum)
    Private ReadOnly _arrowMap As New Dictionary(Of Kompass, PictureBox)
    Private _selected As PositionEnum = PositionEnum.Center

    ' Auto-Repeat intern
    Private ReadOnly _delayTimer As New Timer()
    Private ReadOnly _repeatTimer As New Timer()
    Private _heldDir As Kompass = Kompass.None
    Private _heldPB As PictureBox = Nothing

    Public Sub New()
        DoubleBuffered = True
        SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.UserPaint Or ControlStyles.OptimizedDoubleBuffer, True)
        Size = New Size(GRID * CELL, GRID * CELL)
        BuildGrid()

        ' Timer verdrahten
        AddHandler _delayTimer.Tick, AddressOf DelayTimer_Tick
        AddHandler _repeatTimer.Tick, AddressOf RepeatTimer_Tick
    End Sub

    ' ---------- Public API ----------
    Public Property SelectedStartingPoint As PositionEnum
        Get
            Return _selected
        End Get
        Set(value As PositionEnum)
            If _selected <> value Then
                _selected = value
                UpdateStartPointVisuals()
                RaiseEvent StartingPointChanged(_selected)
            End If
        End Set
    End Property

    Public Sub SetOffsets(x As Integer, y As Integer)
        Dim changed As Boolean = False
        If _offsetX <> x Then _offsetX = x : changed = True
        If _offsetY <> y Then _offsetY = y : changed = True
        If changed Then RaiseEvent OffsetsChanged(_offsetX, _offsetY)
    End Sub

    Public Sub ResetOffsets()
        SetOffsets(0, 0)
    End Sub

    ' ---------- Aufbau ----------
    Private Sub BuildGrid()
        SuspendLayout()

        Dim MAX As Integer = GRID - 1   ' = 6

        ' StartingPoint Punkte (Ecken)
        AddPoint(0, 0, PositionEnum.EckeLO)
        AddPoint(MAX, 0, PositionEnum.EckeRO)      ' (6,0)
        AddPoint(MAX, MAX, PositionEnum.EckeRU)    ' (6,6)
        AddPoint(0, MAX, PositionEnum.EckeLU)      ' (0,6)

        ' Kantenmitten
        AddPoint(C, 0, PositionEnum.MitteO)        ' (3,0)
        AddPoint(MAX, C, PositionEnum.MitteR)      ' (6,3)
        AddPoint(C, MAX, PositionEnum.MitteU)      ' (3,6)
        AddPoint(0, C, PositionEnum.MitteL)        ' (0,3)

        ' Center
        AddPoint(C, C, PositionEnum.Center)        ' (3,3)

        ' Vier „Center-Quadranten“-Punkte:
        AddPoint(1, 1, PositionEnum.CenterLO)
        AddPoint(5, 1, PositionEnum.CenterRO)
        AddPoint(5, 5, PositionEnum.CenterRU)
        AddPoint(1, 5, PositionEnum.CenterLU)

        ' Kompass-Pfeile direkt um das Center (keine Center-Grafik!)
        AddArrow(C, C - 1, Kompass.N)          ' (3,2)
        AddArrow(C + 1, C - 1, Kompass.NO)     ' (4,2)
        AddArrow(C + 1, C, Kompass.O)          ' (4,3)
        AddArrow(C + 1, C + 1, Kompass.SO)     ' (4,4)
        AddArrow(C, C + 1, Kompass.S)          ' (3,4)
        AddArrow(C - 1, C + 1, Kompass.SW)     ' (2,4)
        AddArrow(C - 1, C, Kompass.W)          ' (2,3)
        AddArrow(C - 1, C - 1, Kompass.NW)     ' (2,2)

        ResumeLayout(False)
        UpdateStartPointVisuals()
    End Sub

    Private Sub Positionierer_Paint(sender As Object, e As PaintEventArgs) Handles Me.Paint

        'Die Ecken liegen bei:
        ' (0,0)
        ' (6,0)
        ' (6,6)
        ' (0,6)

        Dim x As Integer, y As Integer

        Dim width As Integer = CELL * 6
        Dim height As Integer = CELL * 6
        Dim left As Integer = CELL \ 2
        Dim top As Integer = CELL \ 2

        Using penLine As New Pen(Color.FromArgb(120, Me.ForeColor), CSng(LINE_THICK))
            Dim rect As New Rectangle(left, top, width, height)
            e.Graphics.DrawRectangle(penLine, rect)
        End Using


    End Sub


    Private Sub AddPoint(x As Integer, y As Integer, pos As PositionEnum)
        Dim pb As New PictureBox() With {
            .SizeMode = PictureBoxSizeMode.CenterImage,
            .Width = CELL, .Height = CELL,
            .Left = x * CELL, .Top = y * CELL,
            .BackColor = Color.Transparent,
            .Cursor = Cursors.Default,
            .Image = _pt
        }
        Controls.Add(pb)
        _startMap(pb) = pos

        AddHandler pb.MouseEnter, Sub(s, e)
                                      Dim p As PictureBox = DirectCast(s, PictureBox)
                                      Dim thisPos As PositionEnum = _startMap(p)
                                      p.Image = If(thisPos = _selected, _ptSel, _ptMover)
                                  End Sub
        AddHandler pb.MouseLeave, Sub(s, e)
                                      Dim p As PictureBox = DirectCast(s, PictureBox)
                                      Dim thisPos As PositionEnum = _startMap(p)
                                      p.Image = If(thisPos = _selected, _ptSel, _pt)
                                  End Sub
        AddHandler pb.Click, Sub(s, e)
                                 Dim p As PictureBox = DirectCast(s, PictureBox)
                                 Dim thisPos As PositionEnum = _startMap(p)
                                 If _selected <> thisPos Then
                                     _selected = thisPos
                                     UpdateStartPointVisuals()
                                     RaiseEvent StartingPointChanged(_selected)
                                 End If
                             End Sub
    End Sub

    Private Sub AddArrow(x As Integer, y As Integer, dir As Kompass)
        Dim pb As New PictureBox() With {
            .SizeMode = PictureBoxSizeMode.CenterImage,
            .Width = CELL, .Height = CELL,
            .Left = x * CELL, .Top = y * CELL,
            .BackColor = Color.Transparent,
            .Cursor = Cursors.Default,
            .Image = ArrowNormal(dir)
        }
        Controls.Add(pb)
        _arrowMap(dir) = pb

        AddHandler pb.MouseEnter, Sub(s, e) If _heldDir <> dir Then pb.Image = ArrowHover(dir)
        AddHandler pb.MouseLeave, Sub(s, e)
                                      If _heldDir = dir Then
                                          ' Bei gedrückt und raus: wir stoppen die Wiederholung
                                          StopRepeat()
                                      End If
                                      pb.Image = ArrowNormal(dir)
                                  End Sub

        ' Auto-Repeat: Down → sofort 1x feuern + Delay → Repeat
        AddHandler pb.MouseDown, Sub(s, e)
                                     If e.Button <> MouseButtons.Left Then Return
                                     StartRepeat(dir, pb)
                                 End Sub

        AddHandler pb.MouseUp, Sub(s, e)
                                   If e.Button <> MouseButtons.Left Then Return
                                   StopRepeat()
                                   ' MouseUp: zeige Hover, falls Cursor noch drauf liegt
                                   If pb.ClientRectangle.Contains(pb.PointToClient(Control.MousePosition)) Then
                                       pb.Image = ArrowHover(dir)
                                   Else
                                       pb.Image = ArrowNormal(dir)
                                   End If
                               End Sub

        ' Falls Capture verloren geht (z. B. Alt-Tab)
        AddHandler pb.MouseCaptureChanged, Sub(s, e) If _heldDir = dir Then StopRepeat()
    End Sub

    ' ---------- Auto-Repeat-Logik ----------
    Private Sub StartRepeat(dir As Kompass, pb As PictureBox)
        ' bereits aktiv? erst stoppen
        StopRepeat()

        _heldDir = dir
        _heldPB = pb

        ' Sofort EINEN Schritt ausführen
        pb.Image = ArrowSel(dir)
        FireDirection(dir)

        ' Delay starten, danach Intervalltimer
        _delayTimer.Interval = InitialRepeatDelayMs
        _delayTimer.Start()
    End Sub

    Private Sub DelayTimer_Tick(sender As Object, e As EventArgs)
        _delayTimer.Stop()
        If _heldDir <> Kompass.None Then
            _repeatTimer.Interval = RepeatIntervalMs
            _repeatTimer.Start()
        End If
    End Sub

    Private Sub RepeatTimer_Tick(sender As Object, e As EventArgs)
        If _heldDir = Kompass.None OrElse _heldPB Is Nothing Then
            StopRepeat()
            Return
        End If
        _heldPB.Image = ArrowSel(_heldDir)
        FireDirection(_heldDir)
    End Sub

    Private Sub StopRepeat()
        _delayTimer.Stop()
        _repeatTimer.Stop()

        If _heldPB IsNot Nothing Then
            Dim over As Boolean = _heldPB.ClientRectangle.Contains(_heldPB.PointToClient(Control.MousePosition))
            _heldPB.Image = If(over, ArrowHover(_heldDir), ArrowNormal(_heldDir))
        End If

        _heldDir = Kompass.None
        _heldPB = Nothing
    End Sub

    Private Sub FireDirection(dir As Kompass)
        Dim dx As Integer, dy As Integer
        DirToDelta(dir, dx, dy)

        dx *= StepX : dy *= StepY

        Dim nx As Integer = _offsetX
        Dim ny As Integer = _offsetY
        If AutoUpdateOffsets Then
            OffsetX = _offsetX + dx ' nutzt Property -> OffsetsChanged
            OffsetY = _offsetY + dy
            nx = _offsetX : ny = _offsetY
        End If

        RaiseEvent DirectionClicked(dir, dx, dy, nx, ny)

        RaiseEvent DirectionIncrement(dx, dy)

    End Sub

    ' ---------- Helpers ----------
    Private Sub UpdateStartPointVisuals()
        For Each kvp As KeyValuePair(Of PictureBox, PositionEnum) In _startMap
            kvp.Key.Image = If(kvp.Value = _selected, _ptSel, _pt)
        Next
    End Sub

    Private Sub DirToDelta(dir As Kompass, ByRef dx As Integer, ByRef dy As Integer)
        dx = 0 : dy = 0
        Select Case dir
            Case Kompass.N : dy = -1
            Case Kompass.S : dy = 1
            Case Kompass.O : dx = 1
            Case Kompass.W : dx = -1
            Case Kompass.NO : dx = 1 : dy = -1
            Case Kompass.NW : dx = -1 : dy = -1
            Case Kompass.SO : dx = 1 : dy = 1
            Case Kompass.SW : dx = -1 : dy = 1
        End Select
    End Sub

    Protected Overrides Sub OnResize(e As EventArgs)
        MyBase.OnResize(e)
        Dim needW As Integer = GRID * CELL
        Dim needH As Integer = GRID * CELL
        If Width <> needW OrElse Height <> needH Then
            Size = New Size(needW, needH)
        End If
    End Sub


    Protected Overrides Sub Dispose(disposing As Boolean)
        If disposing Then
            RemoveHandler _delayTimer.Tick, AddressOf DelayTimer_Tick
            RemoveHandler _repeatTimer.Tick, AddressOf RepeatTimer_Tick
            _delayTimer.Dispose()
            _repeatTimer.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub


End Class
