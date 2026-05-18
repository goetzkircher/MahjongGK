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

Public Class SFStockJobs

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
    '
    ''' <summary>
    ''' Reine Weiterleitung nach _sfd.SFInf.Generator.DebugStoneCountLimit.
    ''' Kürzt den Vorrat. Nur innerhalb der IDE wirksam.
    ''' Aufruf außerhalb unschädlich.
    ''' </summary>
    Public WriteOnly Property DebugStoneCountLimit As Integer
        Set(value As Integer)
            _sfd.SFInf.Generator.DebugStoneCountLimit = value
        End Set
    End Property

    Private _lastSteineCount As Integer = -1
    Private _msgSteineCountChangedAt As Integer
    '
    ''' <summary>
    ''' True wid einen Renderstep nach dem Ereignis zurückgegeben 
    ''' </summary>
    ''' <returns></returns>
    Public Function ConsumeSteineCountGrow() As Boolean

        If _lastSteineCount < SteineCount Then
            _msgSteineCountChangedAt = INI.RenderCounter_GetValue + 1
        End If
        _lastSteineCount = SteineCount

        Return _msgSteineCountChangedAt >= INI.RenderCounter_GetValue

    End Function

    ''' <summary>
    ''' Weiterleitung mit anschließedem UpdateStockValues.
    ''' Gibt den selektierten Stein zurück und löscht ihn im Vorrat.
    ''' Wenn index kleiner 0 OrElse Vorrat.Count = 0 OrElse index > Vorrat.Count - 1
    ''' dann wird die Fehlergrafik zurückgegeben.
    ''' </summary>
    ''' <param name="index"></param>
    ''' <returns></returns>
    Public Function GetSteinTypIndexAndRemove(index As Integer) As SteinTyp
        Dim st As SteinTyp = _sfd.SFInf.Generator.GetSteinTypIndexAndRemove(index)
        UpdateStockValues()
        Return st
    End Function
    '
    ''' <summary>
    ''' Weiterleitung
    ''' Gibt den selektierten Stein zurück und löscht ihn NICHT im Vorrat.
    ''' Wenn index kleiner 0 OrElse Vorrat.Count = 0 OrElse index > Vorrat.Count - 1
    ''' dann wird die Fehlergrafik zurückgegeben.
    ''' </summary>
    ''' <param name="index"></param>
    ''' <returns></returns>
    Public Function GetSteinTypIndexDontRemove(index As Integer) As SteinTyp
        Return _sfd.SFInf.Generator.GetSteinTypIndexDontRemove(index)
    End Function
    '
    ''' <summary>
    ''' Weiterleitung mit anschließedem UpdateStockValues.
    ''' Fügt den übergebenen Stein links vom index ein.
    ''' (Zurücklegen eines Steines vom Feld in den Vorrat.)
    ''' Ist index zu klein, wird ganz links eingefügt,
    ''' ist er zu groß ganz rechts.
    ''' Vorsicht: nur verwenden um entnommene Steine wieder hinzuzufügen.
    ''' </summary>
    ''' <param name="index"></param>
    ''' <param name="st"></param>
    Public Sub InsertLeftFromSteinIdx(index As Integer, st As SteinTyp)
        _sfd.SFInf.Generator.InsertLeftFromSteinIdx(index, st)
        UpdateStockValues()
    End Sub
    '
    ''' <summary>
    ''' Weiterleitung mit anschließedem UpdateStockValues.
    ''' Fügt ein Stein am Ende hinzu.
    ''' Vorsicht: nur verwenden um entnommene Steine wieder hinzuzufügen,
    ''' sonst stimmt die Zusammensetzung des Vorrates nicht mehr.
    ''' </summary>
    ''' <param name="st"></param>
    Public Sub AddAtStockEnd(st As SteinTyp)
        _sfd.SFInf.Generator.AddAtStockEnd(st)
        UpdateStockValues()
    End Sub

    Public ReadOnly Property StockAktCount As Integer
        Get
            Return _sfd.SFInf.Generator.StockAktCount
        End Get
    End Property
    Public ReadOnly Property StockAktUBnd As Integer
        Get
            Return _sfd.SFInf.Generator.StockAktUBnd
        End Get
    End Property
    '
    ''' <summary>
    ''' Die Länge der Vorratskiste, d.h. die Anzahl maximal enthaltener Steine -1
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property StockMaxUBound As Integer
        Get
            Return _sfd.SFInf.Generator.StockMaxUBound
        End Get
    End Property

    '
    ''' <summary>
    ''' Weiterleitung
    ''' </summary>
    Public Sub ShuffleStock()
        _sfd.SFInf.Generator.ShuffleStock()
    End Sub

    Public Sub UpdateStockValues()

        If IsNothing(_sfd.SFLay.rxStock) Then
            Exit Sub
        End If

        SteineCount = _sfd.SFInf.Generator.Stock.Count
        SteineUBound = SteineCount - 1

        If SteineCount = 0 Then
            'Die Scrollbar zurücksetzen
            _sfd.SFRun.HScrollBarStock.SetRange(0)
            Return
        End If

        'Aufrunden, der letzte Stein kann angeschnitten sein.
        SteineVisibleMaxCount = _sfd.SFLay.rxStock.Width \ _sfd.SFLay.steinWidth + 1
        If SteineVisibleMaxCount > SteineCount Then
            SteineVisibleMaxCount = SteineCount
        End If

        If SteineCount >= SteineVisibleMaxCount Then
            SteineVisibleAktCount = SteineVisibleMaxCount
        Else
            SteineVisibleAktCount = SteineCount
        End If

        'TODO stimmt nicht
        SteinVisibleAktFistIdx = 0
        SteinVisibleAktLastIdx = SteineVisibleAktCount - 1

        With _sfd.SFRun.HScrollBarStock
            .SetRange(SteineUBound) 'Minimum = 0 : Maximum = steineUBound
            .SetSmallChange(1)
            .SetPageSize(SteineVisibleMaxCount - 1)
        End With
    End Sub

    Public Structure StockValues

        Public HasValue As Boolean
        Public HasValueAfterEndOfStock As Boolean
        Public HasValueInsideStock As Boolean
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
    ''' in den StockValues, st braucht also nicht gesichert werden.
    ''' </summary>
    ''' <param name="mousePos"></param>
    ''' <returns></returns>
    Public Function GetSelectedSteinValues(mousePos As Point, Optional getDropPosition As Boolean = False, Optional leaveCenterBlank As Boolean = False) As StockValues

        Dim sv As New StockValues
        sv.MousePos = mousePos

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

            Dim ok As Boolean = False
            If leaveCenterBlank Then
                If rectStein.Contains(mousePos) Then
                    Dim delta As Integer = rectStein.Width \ 6
                    If mousePos.X >= rectStein.Left + delta AndAlso mousePos.X <= rectStein.Right - delta Then
                        ok = True
                    End If
                End If
            Else
                ok = rectStein.Contains(mousePos)
            End If

            If ok Then

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
                    .HasValue = True
                    .HasValueInsideStock = True
                End With

                Exit For

            End If
        Next

        If Not sv.HasValue Then 'AndAlso SteinVisibleAktLastIdx = _sfd.SFInf.Generator.StockAktUBnd  Then
            Dim rectStein As Rectangle = GetRectFromStockSteinIdx(SteinVisibleAktLastIdx, getDropPosition)
            If Not rectStein.IsEmpty Then
                Dim rect As Rectangle
                With rectStein
                    rect = New Rectangle(.Left + .Width, .Top, .Width, .Height)
                End With

                If rect.Contains(mousePos) Then
                    With sv
                        .HasValue = True
                        .HasAnkerPos = False
                        .HasValueAfterEndOfStock = True
                        .HasValueInsideStock = False
                        .SteinIndex = SteinTyp.ErrorSy
                        .StockSteinIdx = _sfd.SFInf.Generator.StockAktUBnd + 1
                        .RectStein = rect
                    End With
                End If
            End If
        End If

        Return sv

    End Function

    Private _stockRectsNormal() As Rectangle = Array.Empty(Of Rectangle)()
    Private _stockRectsDrop() As Rectangle = Array.Empty(Of Rectangle)()
    Private _stockRectFirstIdx As Integer
    Private _stockRectLastIdx As Integer

    Private _IdxGapInsert As Integer = -1
    Private _IdxGapRemove As Integer = -1
    Private _WidthGapInsert As Integer
    Private _WidthGapRemove As Integer

    Private _gapJobIsWorking As Boolean
    Private _hasBmpGapInsert As Boolean
    Private _hasBmpGapRemove As Boolean
    Private _bmpGapInsert As Bitmap
    Private _bmpGapRemove As Bitmap
    Private _rectGapInsert As Rectangle
    Private _rectGapRemove As Rectangle
    Private _steinTypInsert As SteinTyp
    Private _steinTypRemove As SteinTyp

    ''' <summary>
    ''' Wenn GapJobIsWorking sollte am Beginn der Auswertung der Mausaktionen stehen
    ''' und die weitere Auswertung abbrechen, damit keine instabilen Zustände entstehen.
    ''' Die GabJob-Animation dauert nur wenige Renderschritte. 
    ''' (INI.Editor_SpaceFramesToOpenOrClose)
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property GapJobIsWorking As Boolean
        Get
            Return _gapJobIsWorking
        End Get
    End Property
    '
    ''' <summary>
    ''' Wenn True, kann BmpGapInsert und RectGapInsert abgerufen werden und der Renderung
    ''' zugeführt werden.
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property HasBmpGapInsert As Boolean
        Get
            Return _hasBmpGapInsert
        End Get
    End Property

    Public ReadOnly Property HasBmpGapRemove As Boolean
        Get
            Return _hasBmpGapRemove
        End Get
    End Property

    Public ReadOnly Property BmpGapInsert As Bitmap
        Get
            Return _bmpGapInsert
        End Get
    End Property

    Public ReadOnly Property BmpGapRemove As Bitmap
        Get
            Return _bmpGapRemove
        End Get

    End Property
    Public ReadOnly Property RectGapInsert As Rectangle
        Get
            Return _rectGapInsert
        End Get
    End Property

    Public ReadOnly Property RectGapRemove As Rectangle
        Get
            Return _rectGapRemove
        End Get

    End Property

    Private Sub ClearGapJob()

        _IdxGapInsert = -1
        _WidthGapInsert = 0
        _bmpGapInsert = Nothing
        _rectGapInsert = Rectangle.Empty
        _hasBmpGapInsert = False
        _IdxGapRemove = -1
        _WidthGapRemove = 0
        _bmpGapRemove = Nothing
        _rectGapRemove = Rectangle.Empty
        _hasBmpGapRemove = False
        _gapJobIsWorking = False

    End Sub
    '
    '''' <summary>
    '''' Führt das Einfügen und/oder Entfernen der Steine dann durch.
    '''' </summary>
    'Private Sub InsertOrRemoveSteinToStock()

    '    If _IdxGapInsert >= 0 AndAlso _IdxGapRemove >= 0 Then
    '        If _IdxGapInsert > _IdxGapRemove Then
    '            'der weiter hinten stehende Stein muss zuerst bearbeitet werden, 
    '            'da sich sonst der Index des anderen ändert.
    '            _sfd.SFInf.Generator.InsertLeftFromSteinIdx(_IdxGapInsert, _steinTypInsert)
    '            _sfd.SFInf.Generator.GetSteinTypIndexAndRemove(_IdxGapRemove)
    '        Else
    '            _sfd.SFInf.Generator.GetSteinTypIndexAndRemove(_IdxGapRemove)
    '            _sfd.SFInf.Generator.InsertLeftFromSteinIdx(_IdxGapInsert, _steinTypInsert)
    '        End If
    '    ElseIf _IdxGapInsert >= 0 Then
    '        _sfd.SFInf.Generator.InsertLeftFromSteinIdx(_IdxGapInsert, _steinTypInsert)

    '    ElseIf _IdxGapRemove >= 0 Then
    '        _sfd.SFInf.Generator.GetSteinTypIndexAndRemove(_IdxGapRemove)
    '    End If

    'End Sub
    '
    ''' <summary>
    ''' Ein Gap-Job öffnet oder schließt eine oder zwei Lücken im Vorrat, in die
    ''' ein Stein eingefügt oder entfernt wird.
    ''' 
    ''' </summary>
    Public Sub StartNewGapJob(Optional idxGapInsert As Integer = -1,
                              Optional bmpGapInsert As Bitmap = Nothing,
                              Optional steinTypInsert As SteinTyp = SteinTyp.ErrorSy,
                              Optional idxGapRemove As Integer = -1,
                              Optional bmpGapRemove As Bitmap = Nothing,
                              Optional steinTypRemove As SteinTyp = SteinTyp.ErrorSy)

        If (idxGapInsert < 0 AndAlso idxGapRemove < 0) OrElse (bmpGapInsert Is Nothing AndAlso bmpGapRemove Is Nothing) Then
            Throw New Exception("Programmierfehler: StartNewGapJob erfordert entweder Werte für GapInsert oder GapRemove oder beides.")
        End If
        If idxGapInsert >= 0 AndAlso bmpGapInsert Is Nothing Then
            Throw New Exception("Programmierfehler: StartNewGapJob erfordert bmpGapInsert oder idxGapInsert muss -1 sein.")
        End If
        If idxGapRemove >= 0 AndAlso bmpGapRemove Is Nothing Then
            Throw New Exception("Programmierfehler: StartNewGapJob erfordert bmpGapRemove oder idxGapRemove muss -1 sein.")
        End If

        If idxGapInsert >= 0 AndAlso idxGapRemove >= 0 AndAlso idxGapInsert = idxGapRemove Then
            Throw New Exception("Programmierfehler: idxGapInsert und idxGapRemove dürfen nicht gleich sein.")
        End If

        ClearGapJob()

        If idxGapInsert >= 0 Then
            _IdxGapInsert = idxGapInsert
            _bmpGapInsert = bmpGapInsert
            _steinTypInsert = steinTypInsert
            _hasBmpGapInsert = True
            'Die _rect... werden in SetRectsFromVisibleStock zugewiesen,
            'da st sich laufend ändern können.
        End If

        If idxGapRemove >= 0 Then
            _IdxGapRemove = idxGapRemove
            _bmpGapRemove = bmpGapRemove
            _steinTypRemove = steinTypRemove
            _hasBmpGapRemove = True
            GetSteinTypIndexAndRemove(_IdxGapRemove)

        End If

        SetRectsFromVisibleStock(startNewGapJob:=True)

    End Sub

    Public Sub ContinueGapJob()
        SetRectsFromVisibleStock(startNewGapJob:=False)
    End Sub

    Public Sub SetRectsFromVisibleStock()
        SetRectsFromVisibleStock(startNewGapJob:=False)
    End Sub

    '
    ''' <summary>
    ''' 
    ''' notwendigen Gap-Werte aus den vier Properties _IdxGapInsert, _IdxGapRemove, _WidthGapInsert und _WidthGapRemove 
    ''' Wenn startNewGapJob = TRue gesetzt wird, muss vorher _IdxGapInsert und/oder _IdxGapRemove
    ''' und die zugehörigen GapInsertJob und(oder GapRemoveJob gesetzt wwerden.
    ''' (nicht benötigte auf None stellen) WidthGapX setzt das Programm selber.
    ''' Nach der automatischen Durchführung der Jobs wird alles rückgestellt.
    ''' </summary>
    Private Sub SetRectsFromVisibleStock(startNewGapJob As Boolean)

        If startNewGapJob Then
            _gapJobIsWorking = True
            Dim steinWidth As Integer = _sfd.SFLay.steinWidth
            _WidthGapInsert = 0
            _WidthGapRemove = steinWidth
        End If

        If _gapJobIsWorking Then
            Dim steinWidth As Integer = _sfd.SFLay.steinWidth
            Dim frames As Integer = INI.Editor_SpaceFramesToOpenOrClose
            Dim steps As Integer = steinWidth \ frames
            Dim endCount As Integer

            If _IdxGapInsert >= 0 Then
                _WidthGapInsert += steps
                If _WidthGapInsert > steinWidth Then
                    _WidthGapInsert = steinWidth
                    endCount += 1
                End If
            Else
                endCount += 1
            End If
            If _IdxGapRemove >= 0 Then
                _WidthGapRemove -= steps
                If _WidthGapRemove < 0 Then
                    _WidthGapRemove = 0
                    endCount += 1
                End If
            Else
                endCount += 1
            End If
            '

            SetRectFromStockSteinIdx(_IdxGapInsert, _WidthGapInsert, _IdxGapRemove, _WidthGapRemove)

            If endCount = 2 Then
                If _IdxGapInsert >= 0 Then
                    _sfd.SFStock.InsertLeftFromSteinIdx(_IdxGapInsert, _steinTypInsert)
                End If
                ClearGapJob()
            End If

        Else
            SetRectFromStockSteinIdx(_IdxGapInsert, _WidthGapInsert, _IdxGapRemove, _WidthGapRemove)
        End If

        If _hasBmpGapInsert Then
            _rectGapInsert = GetRectFromStockSteinIdx(_IdxGapInsert, getDropPosition:=True)
        End If
        If _hasBmpGapRemove Then
            _rectGapRemove = GetRectFromStockSteinIdx(_IdxGapRemove, getDropPosition:=True)
        End If

    End Sub

    '
    ''' <summary>
    ''' Muss einmal pro Renderung aufgerufen werden, anschließend können die Rectangle über
    ''' GetRectFromStockSteinIdx(stockSteinIdx As Integer) abgerufen werden.
    ''' (Alternatov die Überladung)
    ''' </summary>
    Private Sub SetRectFromStockSteinIdx(idxGapInsert As Integer, widthGapInsert As Integer, idxGapRemove As Integer, widthGapRemove As Integer)

        ' idxGapX = -1 schaltet ab, da "If idx = idxGapInsert Then" nie erfüllt wird.

        With _sfd.SFLay

            Dim offsetLeft As Integer = _sfd.SFStock.OffsetLeft
            Dim steinWidth As Integer = .steinWidth
            Dim steinHeight As Integer = .steinHeight
            Dim stockTop As Integer = .rxStock.Top
            Dim stockLeft As Integer = .rxStock.Left
            Dim stockWidth As Integer = .rxStockScrollbar.Width
            Dim maxCount As Integer = _sfd.SFStock.SteineCount

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

                If idx = idxGapInsert Then
                    extraOffset += widthGapInsert
                End If

                If idx = idxGapRemove Then
                    extraOffset += widthGapRemove
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
                        Dim newLeft As Integer = .Left - (.Width \ 2) 'Das ist Anfangs eine negative Zahl
                        If newLeft > 0 Then
                            newLeft = .Left - .Width
                        End If

                        _stockRectsDrop(idxAkt) = New Rectangle(newLeft, .Top, .Width, .Height)
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

        If stockSteinIdx < _stockRectFirstIdx OrElse stockSteinIdx > _stockRectLastIdx + 1 Then
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

#Region "Scrollbar"

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

#End Region

End Class
