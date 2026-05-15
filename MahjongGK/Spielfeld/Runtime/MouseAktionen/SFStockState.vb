Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

Imports MahjongGK.Contracts.GlobalEnum
Imports MahjongGK.Spielfeld

#Disable Warning IDE0079
#Disable Warning IDE1006
#Disable Warning IDE0031
#Disable Warning IDE0017

Public Enum GapJob
    None
    Open
    Close
End Enum

Public Class SFStockState

    Sub New()

    End Sub

    Sub New(owner As SFDaten)

        _sfd = owner

    End Sub

    Private _sfd As SFDaten
    Public ReadOnly Property IsEmpty As Boolean
        Get
            Return SteineCount = 0
        End Get
    End Property

    Private Property SteineCount As Integer
    Private Property SteineUBound As Integer
    Private Property SteineVisibleMaxCount As Integer
    Private Property SteineVisibleAktCount As Integer
    Private Property HScrollBarLastValue As Integer
    Public Property OffsetLeft As Integer
    Public Property SteinVisibleAktFistIdx As Integer
    Public Property SteinVisibleAktLastIdx As Integer

    Public Property LastSteinWidth As Integer
    Public Property LastRxStockWidth As Integer

    Public Sub ClearAndSetStartvalues()

        If IsNothing(_sfd.SFLay.rxStock) Then
            Return
        End If

        SteineCount = _sfd.SFInf.Generator.Stock.Count
        SteineUBound = SteineCount - 1

        If SteineCount = 0 Then
            'Die Scrollbar zurücksetzen
            _sfd.SFRun.HScrollBarStock.SetRange(0)
            Return
        End If

        UpdateSteineVisibleMaxCount()

        If SteineCount >= SteineVisibleMaxCount Then
            SteineVisibleAktCount = SteineVisibleMaxCount
        Else
            SteineVisibleAktCount = SteineCount
        End If

        SteinVisibleAktFistIdx = 0
        SteinVisibleAktLastIdx = SteineVisibleAktCount - 1

        With _sfd.SFRun.HScrollBarStock
            .SetRange(SteineUBound) 'Minimum = 0 : Maximum = steineUBound
            .SetSmallChange(1)
            .SetPageSize(SteineVisibleMaxCount - 1)
        End With
    End Sub

    Public Sub UpdateSteineVisibleMaxCount()
        'Aufrunden, der letzte Stein kann angeschnitten sein.
        SteineVisibleMaxCount = _sfd.SFLay.rxStock.Width \ _sfd.SFLay.steinWidth + 1

        With _sfd.SFRun.HScrollBarStock
            .SetPageSize(SteineVisibleMaxCount - 1)
        End With
    End Sub '

    Public Structure StockValues

        Public HasValues As Boolean
        Public IsWaiting As Boolean
        Public HasAnkerPos As Boolean

        Public MousePos As Point
        Public AnkerPos As Point
        Public StockSteinIdx As Integer
        Public SteinIndex As SteinTyp
        Public RectStein As Rectangle

    End Structure
    '
    ''' <summary>
    ''' Bevor die Funktion aufgerufen wird, muss SetRectsFromVisibleStock (oder die
    ''' Überladung) aufgerufen werden da auf GetRectFromStockSteinIdx zugegriffen wird.
    ''' Während der Animation um Platz zus schaffen bzw. zu schließen wird der Wert
    ''' IsWaitimg = True zurückgegeben. Dann muss im nächsten Renderschritt die
    ''' Anfrage wiederholt werden. Die usprüngliche Mausposition ist die MousePos
    ''' in den StockValues, sie braucht also nicht gesichert werden.
    ''' </summary>
    ''' <param name="mousePos"></param>
    ''' <returns></returns>
    Public Function GetSelectedSteinValues(mousePos As Point, Optional getDropPosition As Boolean = False) As StockValues

        Dim sv As New StockValues
        sv.HasValues = False
        sv.IsWaiting = False
        sv.MousePos = mousePos

        If _gapJobIsWorking Then
            sv.IsWaiting = True
            Return sv
        End If

        If IsEmpty Then
            Return sv
        End If

        If SteinVisibleAktFistIdx = -1 Then
            Return sv
        End If

        If Not _sfd.SFLay.rxStock.Contains(mousePos) Then
            Return sv
        End If

        For aktIdx As Integer = SteinVisibleAktFistIdx To SteinVisibleAktLastIdx

            Dim rectStein As Rectangle = GetRectFromStockSteinIdx(aktIdx, getDropPosition)

            If rectStein.IsEmpty Then
                Continue For
            End If

            If rectStein.Contains(mousePos) Then

                With sv
                    If getDropPosition Then
                        .AnkerPos = Point.Empty
                        .HasAnkerPos = False
                    Else
                        .AnkerPos = New Point(mousePos.X - rectStein.Left, mousePos.Y - rectStein.Top)
                        .HasAnkerPos = True
                    End If
                    .StockSteinIdx = aktIdx
                    .SteinIndex = _sfd.SFInf.Generator.Stock(aktIdx)
                    .RectStein = rectStein
                    .HasValues = True
                End With

                Return sv

            End If

        Next

        Return sv

    End Function

    Private _stockRectsNormal() As Rectangle = Array.Empty(Of Rectangle)()
    Private _stockRectsDrop() As Rectangle = Array.Empty(Of Rectangle)()
    Private _stockRectFirstIdx As Integer
    Private _stockRectLastIdx As Integer

    Public Property IdxGap1 As Integer = -1
    Public Property IdxGap2 As Integer = -1
    Public Property WidthGap1 As Integer
    Public Property WidthGap2 As Integer
    Public Property Gap1Job As GapJob
    Public Property Gap2Job As GapJob
    '
    '
    Private _gapJobIsWorking As Boolean
    '
    ''' <summary>
    ''' 
    ''' notwendigen Gap-Werte aus den vier Properties IdxGap1, IdxGap2, WidthGap1 und WidthGap2 
    ''' Wenn startNewGapJob = TRue gesetzt wird, muss vorher IdxGap1 und/oder IdxGap2
    ''' und die zugehörigen Gap1Job und(oder Gap2Job gesetzt wwerden.
    ''' (nicht benötigte auf None stellen) WidthGapX setzt das Programm selber.
    ''' Nach der automatischen Durchführung der Jobs wird alles rückgestellt.
    ''' </summary>
    Public Sub SetRectsFromVisibleStock(Optional startNewGapJob As Boolean = False)

        If startNewGapJob AndAlso (Gap1Job <> GapJob.None OrElse Gap2Job <> GapJob.None) Then
            _gapJobIsWorking = True
            Dim steinWidth As Integer = _sfd.SFLay.steinWidth
            If Gap1Job = GapJob.Close Then
                WidthGap1 = steinWidth
            Else
                WidthGap1 = 0
            End If
            If Gap2Job = GapJob.Close Then
                WidthGap2 = steinWidth
            Else
                WidthGap2 = 0
            End If
        End If

        If _gapJobIsWorking Then
            Dim steinWidth As Integer = _sfd.SFLay.steinWidth
            Dim frames As Integer = INI.Editor_SpaceFramesToOpenOrClose
            Dim steps As Integer = steinWidth \ frames
            Dim endCount As Integer
            If Gap1Job = GapJob.Close Then
                WidthGap1 -= steps
                If WidthGap1 < 0 Then
                    WidthGap1 = 0
                    endCount += 1
                End If
            ElseIf Gap1Job = GapJob.Open Then
                WidthGap1 += steps
                If WidthGap1 > steinWidth Then
                    WidthGap1 = steinWidth
                    endCount += 1
                End If
            Else
                endCount += 1
            End If
            '
            If Gap2Job = GapJob.Close Then
                WidthGap2 -= steps
                If WidthGap2 < 0 Then
                    WidthGap2 = 0
                    endCount += 1
                End If
            ElseIf Gap2Job = GapJob.Open Then
                WidthGap2 += steps
                If WidthGap2 > steinWidth Then
                    WidthGap2 = steinWidth
                    endCount += 1
                End If
            Else
                endCount += 1
            End If

            SetRectFromStockSteinIdx(IdxGap1, WidthGap1, IdxGap2, WidthGap2)

            If endCount = 2 Then
                _gapJobIsWorking = False
                IdxGap1 = -1
                IdxGap2 = -1
                Gap1Job = GapJob.None
                Gap2Job = GapJob.None
                WidthGap1 = 0
                WidthGap2 = 0
            End If

        Else
            SetRectFromStockSteinIdx(IdxGap1, WidthGap1, IdxGap2, WidthGap2)
        End If

    End Sub

    '
    ''' <summary>
    ''' Muss einmal pro Renderung aufgerufen werden, anschließend können die Rectangle über
    ''' GetRectFromStockSteinIdx(stockSteinIdx As Integer) abgerufen werden.
    ''' (Alternatov die Überladung)
    ''' </summary>
    ''' <param name="idxGap1"></param>
    ''' <param name="widthGap1"></param>
    ''' <param name="idxGap2"></param>
    ''' <param name="widthGap2"></param>
    Public Sub SetRectFromStockSteinIdx(Optional idxGap1 As Integer = -1,
                                        Optional widthGap1 As Integer = 0,
                                        Optional idxGap2 As Integer = -1,
                                        Optional widthGap2 As Integer = 0)

        ' idxGapX = -1 schaltet ab, da "If idx = idxGap1 Then" nie erfüllt wird.

        With _sfd.SFLay

            Dim offsetLeft As Integer = _sfd.SFStock.OffsetLeft
            Dim steinWidth As Integer = .steinWidth
            Dim steinHeight As Integer = .steinHeight
            Dim stockTop As Integer = .rxStock.Top
            Dim stockWidth As Integer = .rxStockScrollbar.Width
            Dim maxCount As Integer = _sfd.SFInf.Generator.Stock.Count

            Dim sortMinIdx As Integer = INI.Editor_SortMinIndex
            Dim sortSpacer As Integer = INI.Editor_SortSpacerWidth

            _stockRectFirstIdx = SteinVisibleAktFistIdx
            _stockRectLastIdx = Math.Min(SteinVisibleAktLastIdx, maxCount - 1)

            If _stockRectLastIdx < _stockRectFirstIdx Then
                _stockRectsNormal = Array.Empty(Of Rectangle)()
                Return
            End If

            Dim count As Integer = _stockRectLastIdx - _stockRectFirstIdx + 1
            ReDim _stockRectsNormal(count - 1)
            ReDim _stockRectsDrop(count - 1)

            Dim xOffset As Integer = 0
            Dim extraOffset As Integer = 0

            For idx As Integer = _stockRectFirstIdx To _stockRectLastIdx

                If idx = sortMinIdx AndAlso idx > 0 Then
                    extraOffset += sortSpacer
                End If

                If idx = idxGap1 Then
                    extraOffset += widthGap1
                End If

                If idx = idxGap2 Then
                    extraOffset += widthGap2
                End If

                Dim left As Integer = offsetLeft + xOffset + extraOffset

                If left > stockWidth Then
                    _stockRectLastIdx = idx - 1
                    Exit For
                End If

                Dim idxAkt As Integer = idx - _stockRectFirstIdx

                _stockRectsNormal(idxAkt) = New Rectangle(left, stockTop, steinWidth, steinHeight)

                With _stockRectsNormal(idxAkt)
                    If idxAkt = 0 Then
                        'Um einen halben Stein nach links verschoben.
                        '(Der Stein wird an der Indexposition eingefügt, alle Steine einschließlich
                        'der Indexposition um eins nach rechts verschoben.)
                        _stockRectsDrop(idxAkt) = New Rectangle(.Left - (.Width \ 2), .Top, .Width, .Height)
                    Else
                        'Dadurch steht der Geisterstein auch zwischen Lücken mittig
                        Dim center As Integer = (.Right - _stockRectsNormal(idxAkt - 1).Left) \ 2
                        Dim left2 As Integer = _stockRectsNormal(idxAkt - 1).Left + center - (.Width \ 2)
                        _stockRectsDrop(idxAkt) = New Rectangle(left2, .Top, .Width, .Height)
                    End If
                End With

                xOffset += steinWidth

            Next

        End With

    End Sub

    Public Function GetRectFromStockSteinIdx(stockSteinIdx As Integer, Optional getDropPosition As Boolean = False) As Rectangle

        If stockSteinIdx < _stockRectFirstIdx OrElse stockSteinIdx > _stockRectLastIdx Then
            Return Rectangle.Empty
        End If

        Dim arrIdx As Integer = stockSteinIdx - _stockRectFirstIdx

        If arrIdx < 0 OrElse arrIdx >= _stockRectsNormal.Length Then
            Return Rectangle.Empty
        End If
        If getDropPosition Then
            Return _stockRectsDrop(arrIdx)
        Else
            Return _stockRectsNormal(arrIdx)
        End If

    End Function

    ''' <summary>
    ''' Die Scrollbar wird im Rendertakt aufgerufen, (also je Frame genau einmal)
    ''' und der aktuelle Wert wird hierher übermittelt.
    ''' Im Anschluß werden die aktuellen Steine gerendert.
    ''' </summary>
    ''' <param name="value"></param>
    Public Sub SetHScrollBarValue(value As Integer)
        If HScrollBarLastValue = value Then
            Return
        End If

        Dim delta As Integer = value - HScrollBarLastValue
        HScrollBarLastValue = value

        Select Case delta
            Case < 0
                MoveLeft(-delta)
            Case > 0
                MoveRight(delta)
        End Select
    End Sub

    Public Sub MoveRight(count As Integer)

        Dim newFirstIdx As Integer = SteinVisibleAktFistIdx + count
        Dim maxFirstIdx As Integer = SteineCount - SteineVisibleAktCount

        If newFirstIdx <= maxFirstIdx Then
            SteinVisibleAktFistIdx = newFirstIdx
            SteinVisibleAktLastIdx = SteinVisibleAktFistIdx + SteineVisibleAktCount - 1
        Else
            SteinVisibleAktFistIdx = maxFirstIdx
            SteinVisibleAktLastIdx = SteineUBound
        End If
        AdjustRightEdgeForLastStone()

    End Sub

    Public Sub MoveLeft(count As Integer)

        Dim newFirstIdx As Integer = SteinVisibleAktFistIdx - count

        If newFirstIdx >= 0 Then
            SteinVisibleAktFistIdx = newFirstIdx
            SteinVisibleAktLastIdx = SteinVisibleAktFistIdx + SteineVisibleAktCount - 1
        Else
            SteinVisibleAktFistIdx = 0
            SteinVisibleAktLastIdx = SteineVisibleAktCount - 1
        End If
        AdjustRightEdgeForLastStone()

    End Sub

    Private Sub AdjustRightEdgeForLastStone() 'Name kannst du ändern

        If SteineCount <= 0 Then
            OffsetLeft = 0
            Return
        End If

        If SteinVisibleAktLastIdx <> SteineUBound Then
            OffsetLeft = 0
            Return
        End If

        OffsetLeft = GetOffsetLeft()

    End Sub

    Private Function GetOffsetLeft() As Integer

        Dim value1 As Integer = (SteinVisibleAktLastIdx - SteinVisibleAktFistIdx + 1) * _sfd.SFLay.steinWidth
        Dim value2 As Integer = value1 - _sfd.SFLay.rxStock.Width

        If value2 > 0 Then
            Return -value2
        Else
            Return 0
        End If

    End Function
End Class
