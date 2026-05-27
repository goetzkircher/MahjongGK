Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

Imports MahjongGK.Contracts.GlobalEnum
Imports MahjongGK.Spielfeld
Imports TileFactory

#Disable Warning IDE0079
#Disable Warning IDE1006
#Disable Warning IDE0031
#Disable Warning IDE0017
#Disable Warning IDE0044

Public Class SFStockJobs

#Region "Instanzierung"

    Sub New()

    End Sub

    Sub New(owner As SFDaten)

        _sfd = owner

    End Sub

    Private _sfd As SFDaten

#End Region

#Region "Properties und Weiterleitungen"

    Public ReadOnly Property IsEmpty As Boolean
        Get
            Return SteineCount = 0
        End Get
    End Property

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
    ''' True wird einen Renderstep nach dem Ereignis zurückgegeben 
    ''' </summary>
    ''' <returns></returns>
    Public Function ConsumeSteineCountGrow() As Boolean

        If _lastSteineCount < SteineCount Then
            _msgSteineCountChangedAt = INI.Volatil_RenderCounterValue + 1
        End If
        _lastSteineCount = SteineCount

        Return _msgSteineCountChangedAt >= INI.Volatil_RenderCounterValue

    End Function

    ''' <summary>
    ''' Weiterleitung mit anschließedem UpdateScrollbar.
    ''' Gibt den selektierten Stein zurück und löscht ihn im Vorrat.
    ''' Wenn index kleiner 0 OrElse Vorrat.Count = 0 OrElse index > Vorrat.Count - 1
    ''' dann wird die Fehlergrafik zurückgegeben.
    ''' </summary>
    ''' <param name="index"></param>
    ''' <returns></returns>
    Public Function GetSteinSymbolIndexAndRemove(index As Integer) As SteinSymbol
        Dim st As SteinSymbol = _sfd.SFInf.Generator.GetSteinSymbolIndexAndRemove(index)
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
    Public Function GetSteinSymbolIndexDontRemove(index As Integer) As SteinSymbol
        Return _sfd.SFInf.Generator.GetSteinSymbolIndexDontRemove(index)
    End Function
    '
    ''' <summary>
    ''' Weiterleitung mit anschließedem UpdateScrollbar.
    ''' Fügt den übergebenen Stein links vom index ein.
    ''' (Zurücklegen eines Steines vom Feld in den Vorrat.)
    ''' Ist index zu klein, wird ganz links eingefügt,
    ''' ist er zu groß ganz rechts.
    ''' Vorsicht: nur verwenden um entnommene Steine wieder hinzuzufügen,
    ''' sonst stimmt die Zusammensetzung der Steine nicht mehr.
    ''' </summary>
    ''' <param name="index"></param>
    ''' <param name="st"></param>
    Public Sub InsertLeftFromSteinIdx(index As Integer, st As SteinSymbol)
        _sfd.SFInf.Generator.InsertLeftFromSteinIdx(index, st)
    End Sub
    '
    ''' <summary>
    ''' Weiterleitung mit anschließedem UpdateScrollbar.
    ''' Fügt ein Stein am Ende hinzu.
    ''' Vorsicht: nur verwenden um entnommene Steine wieder hinzuzufügen,
    ''' sonst stimmt die Zusammensetzung des Vorrates nicht mehr.
    ''' </summary>
    ''' <param name="st"></param>
    Public Sub AddAtStockEnd(st As SteinSymbol)
        _sfd.SFInf.Generator.AddAtStockEnd(st)
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
    Public Sub ShuffleStock(includeNoSortArea As Boolean)
        _sfd.SFInf.Generator.ShuffleStock(includeNoSortArea)
    End Sub

    Public Sub SortStockPur()
        _sfd.SFInf.Generator.SortStockPur()
    End Sub
    Public Sub SortStockByPairs()
        _sfd.SFInf.Generator.SortStockByPairs()
    End Sub
    Public Sub SeperateStockWidow()
        _sfd.SFInf.Generator.SeperateStockWidow()
    End Sub

    Public Function GetArrayStockWidows() As Boolean()
        Return _sfd.SFInf.Generator.GetArrayStockWidows
    End Function
#End Region

#Region "GapJob und StockValues"

    Public Structure StockValues

        Public HasValue As Boolean
        Public HasValueAfterEndOfStock As Boolean
        Public HasValueInsideStock As Boolean
        Public HasAnkerPos As Boolean

        Public MousePos As Point
        Public AnkerPos As Point
        Public StockSteinIdx As Integer
        Public SteinIndex As SteinSymbol
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

        Dim sv As New StockValues With {
            .StockSteinIdx = -1,
            .MousePos = mousePos
        }

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
                        .SteinIndex = SteinSymbol.ErrorSy
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
    Private _WidthGapInsert As Single
    Private _WidthGapRemove As Single

    Private _gapJobIsWorking As Boolean
    Private _hasBmpGapInsert As Boolean
    Private _hasBmpGapRemove As Boolean
    Private _bmpGapInsert As Bitmap
    Private _bmpGapRemove As Bitmap
    Private _rectGapInsert As Rectangle
    Private _rectGapRemove As Rectangle
    Private _steinSymbolInsert As SteinSymbol
    Private _steinSymbolRemove As SteinSymbol
    Private _rectGapRemoveUseFirstRect As Boolean
    Private _rectGapRemoveFirstRectSelected As Boolean

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

    Public Sub DoGgfAbortAndClearGapJob()
        '
        'Bei sehr schnellem Klicken (Undo/Redo) kann es vorkommen, daß noch während der Animation
        'bereits eine neue einläuft. Da das Einfügen erst am Ende der Animation erfolgt,
        'kann die Animation zwar abgebrochen werden, das Einfügen muss aber durchgeführt werden.
        If _IdxGapInsert >= 0 Then
            With _sfd.SFStock
                .InsertLeftFromSteinIdx(_IdxGapInsert, _steinSymbolInsert)
                _sfd.SFStock.NotifyStockInsert(_IdxGapInsert)
            End With
        End If

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
        _rectGapRemoveUseFirstRect = False
        _rectGapRemoveFirstRectSelected = False
    End Sub
    '

    '
    ''' <summary>
    ''' Ein Gap-Job öffnet oder schließt eine oder zwei Lücken im Vorrat, in die
    ''' ein Stein eingefügt oder entfernt wird.
    ''' 
    ''' </summary>
    Public Sub StartNewGapJob(Optional idxGapInsert As Integer = -1,
                              Optional bmpGapInsert As Bitmap = Nothing,
                              Optional steinSymbolInsert As SteinSymbol = SteinSymbol.ErrorSy,
                              Optional idxGapRemove As Integer = -1,
                              Optional bmpGapRemove As Bitmap = Nothing,
                              Optional steinSymbolRemove As SteinSymbol = SteinSymbol.ErrorSy,
                              Optional gapRemoveUseFirstRect As Boolean = False)

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

        DoGgfAbortAndClearGapJob()

        _rectGapRemoveUseFirstRect = gapRemoveUseFirstRect

        If idxGapInsert >= 0 Then
            _IdxGapInsert = idxGapInsert
            _bmpGapInsert = bmpGapInsert
            _steinSymbolInsert = steinSymbolInsert
            _hasBmpGapInsert = True
            'Die _rect... werden in SetRectsFromVisibleStock zugewiesen,
            'da st sich laufend ändern können.
        End If

        If idxGapRemove >= 0 Then
            _IdxGapRemove = idxGapRemove
            _bmpGapRemove = bmpGapRemove
            _steinSymbolRemove = steinSymbolRemove
            _hasBmpGapRemove = True
            GetSteinSymbolIndexAndRemove(_IdxGapRemove)
            _sfd.SFStock.NotifyStockRemove(_IdxGapRemove)
        End If

        SetRectsFromVisibleStockItems(startNewGapJob:=True)

    End Sub

    Public Sub ContinueGapJob()
        SetRectsFromVisibleStockItems(startNewGapJob:=False)
    End Sub

    Public Sub SetRectsFromVisibleStockItems()
        SetRectsFromVisibleStockItems(startNewGapJob:=False)
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
    Private Sub SetRectsFromVisibleStockItems(startNewGapJob As Boolean)

        If startNewGapJob Then
            _gapJobIsWorking = True
            Dim steinWidth As Integer = _sfd.SFLay.steinWidth
            _WidthGapInsert = 0
            _WidthGapRemove = steinWidth
        End If

        If _gapJobIsWorking Then
            Dim steinWidth As Integer = _sfd.SFLay.steinWidth
            Dim steps As Integer = INI.Editor_AnimationSteps
            Dim deltaWidth As Single = CSng(steinWidth / steps)
            '                                            
            Dim endCount As Integer

            If _IdxGapInsert >= 0 Then
                _WidthGapInsert += deltaWidth
                If _WidthGapInsert > steinWidth Then
                    _WidthGapInsert = 0 'nicht = steinWidth, denn dann wird die Lücke
                    '                    und der neue Stein für einen Renderstep angezeigt.
                    endCount += 1
                End If
            Else
                endCount += 1
            End If
            If _IdxGapRemove >= 0 Then
                _WidthGapRemove -= deltaWidth
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
                    With _sfd.SFStock
                        .InsertLeftFromSteinIdx(_IdxGapInsert, _steinSymbolInsert)
                        _sfd.SFStock.NotifyStockInsert(_IdxGapInsert)
                    End With
                    _IdxGapInsert = -1 'sonst wird doppelt eingefügt
                End If
                DoGgfAbortAndClearGapJob()
            End If

        Else
            SetRectFromStockSteinIdx(_IdxGapInsert, _WidthGapInsert, _IdxGapRemove, _WidthGapRemove)
        End If

        If _hasBmpGapInsert Then
            _rectGapInsert = GetRectFromStockSteinIdx(_IdxGapInsert, getDropPosition:=True)
        End If
        If _hasBmpGapRemove Then
            If _rectGapRemoveUseFirstRect Then
                If Not _rectGapRemoveFirstRectSelected Then
                    _rectGapRemove = GetRectFromStockSteinIdx(_IdxGapRemove, getDropPosition:=True)
                    _rectGapRemoveFirstRectSelected = True
                End If
            Else
                _rectGapRemove = GetRectFromStockSteinIdx(_IdxGapRemove, getDropPosition:=True)
            End If
        End If

    End Sub

    '
    ''' <summary>
    ''' Muss einmal pro Renderung aufgerufen werden, anschließend können die Rectangle über
    ''' GetRectFromStockSteinIdx(stockSteinIdx As Integer) abgerufen werden.
    ''' (Alternatov die Überladung)
    ''' </summary>
    Private Sub SetRectFromStockSteinIdx(idxGapInsert As Integer, widthGapInsert As Single, idxGapRemove As Integer, widthGapRemove As Single)

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
                    extraOffset += CInt(widthGapInsert)
                End If

                If idx = idxGapRemove Then
                    extraOffset += CInt(widthGapRemove)
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

    Public Function GetCenterPointFromStockSteinIdx(stockSteinIdx As Integer) As Point

        Dim rect As Rectangle = GetRectFromStockSteinIdx(stockSteinIdx)

        With rect
            Return New Point(.X + .Width \ 2, .Y + .Height \ 2)
        End With

    End Function
#End Region

#Region "FadeOutJob"

    Private _fadeOutSteinSymbolEnum As SteinSymbol
    Private _fadeOutHasValues As Boolean = False
    Private _fadeOutStartAsGhost As SteinGhost
    Private _fadeOutAktStep As Integer
    Private _fadeOutMaxStep As Integer
    Private _fadeOutAlpha As Single
    Private _fadeOutRect As Rectangle
    Private _fadeOutZEbene As Integer

    Public Sub StartNewFadeOutJob(steinInfoIndex As Integer, startAsGhost As Boolean)
        With _sfd.SFInf.SteinInfos(steinInfoIndex)
            _sfd.SFStock.StartNewFadeOutJob(.SteinSymbolIndex, .RectStein, .Z, SteinGhost.None)
        End With
    End Sub
    ''' <summary>
    ''' Das Fadeout bezieht sich auf den Editor, ist aber hier angesiedelt,
    ''' da es aus dieser Klasse heraus angestoßen wird.
    ''' (Verschieben von Steinen aus dem Editor in den Vorrat.)
    ''' </summary>
    Public Sub StartNewFadeOutJob(steinSymbolEnum As SteinSymbol, rect As Rectangle, zEbene As Integer, startAsGhost As SteinGhost)
        _fadeOutSteinSymbolEnum = steinSymbolEnum
        _fadeOutStartAsGhost = startAsGhost
        _fadeOutHasValues = True
        _fadeOutMaxStep = INI.Editor_AnimationSteps + 1 'Siehe Kommentar in ConsumeHasFadeOutJob
        _fadeOutAktStep = 0
        _fadeOutRect = rect
        _fadeOutZEbene = zEbene
    End Sub

    '
    ''' <summary>
    ''' Wird bei jedem Renderschritt aus Paint_Editor heraus aufgerufen.
    ''' Zählt nach jedem Aufruf weiter, bis die Animation zu ende ist
    ''' und gibt dann False zurück.
    ''' Berechnet auch den neunen Alpha-Wert für das verblassen.
    ''' </summary>
    Public Function ConsumeHasFadeOutJob(zEbene As Integer) As Boolean

        If _fadeOutHasValues AndAlso zEbene = _fadeOutZEbene Then
            _fadeOutAktStep += 1
            If _fadeOutAktStep > _fadeOutMaxStep Then
                _fadeOutHasValues = False
                Return False
            Else
                _fadeOutAlpha = CSng(_fadeOutMaxStep - _fadeOutAktStep) / CSng(_fadeOutMaxStep)
                _fadeOutAlpha = Math.Max(0.0F, Math.Min(1.0F, _fadeOutAlpha))
                If _fadeOutAlpha < 0.05 Then ' Es bringt nichts eine praktisch unsichtbare Bitmap noch zu blitten.
                    '                        'deshalb _fadeOutMaxStep in StartNewFadeOutJob um 1 erhöht
                    '                        'damit INI.Editor_AnimationSteps ausgeführt werden.
                    _fadeOutHasValues = False
                    Return False
                Else
                    Return True
                End If
            End If
        Else
            Return False
        End If
    End Function
    '
    '
    ''' <summary>
    ''' Für die Abfrage aus dem RenderManager heraus.
    ''' Reine Abfrage
    ''' </summary>
    ''' <returns></returns>
    Public Function IsFadeOutAktive() As Boolean
        Return _fadeOutHasValues
    End Function
    '
    ''' <summary>
    ''' Aufrufen, nachdem ConsumeHasFadeOutJob True zurückgegeben hat.
    ''' Dann die Bitmap blitten und Disposen.
    ''' </summary>
    ''' <returns></returns>
    Public Function GetFadeOutBitmap() As Bitmap

        Dim request As New TileRequest(_sfd.SFRun.AktRenderMode, INI.Tile_TileColors, _fadeOutSteinSymbolEnum, SteinStatus.I01Normal, _sfd.SFLay.steinSize, INI.Tile_BasisSize, _fadeOutStartAsGhost)

        Dim bmpSrc As Bitmap = TileFactoryAPI.GetTile(request) 'Cache-Bitmap, nicht disposen
        Dim bmpFade As New Bitmap(bmpSrc.Width, bmpSrc.Height, Imaging.PixelFormat.Format32bppArgb)

        Using g As Graphics = Graphics.FromImage(bmpFade)

            Dim cm As New Imaging.ColorMatrix()
            cm.Matrix00 = 1.0F
            cm.Matrix11 = 1.0F
            cm.Matrix22 = 1.0F
            cm.Matrix33 = _fadeOutAlpha
            cm.Matrix44 = 1.0F

            Using ia As New Imaging.ImageAttributes()
                ia.SetColorMatrix(cm, Imaging.ColorMatrixFlag.Default, Imaging.ColorAdjustType.Bitmap)

                g.DrawImage(bmpSrc,
                            New Rectangle(0, 0, bmpSrc.Width, bmpSrc.Height),
                            0, 0, bmpSrc.Width, bmpSrc.Height,
                            GraphicsUnit.Pixel,
                            ia)
            End Using

        End Using

        Return bmpFade

    End Function

    Public Function GetFadeOutRect() As Rectangle
        Return _fadeOutRect
    End Function
#End Region

#Region "Scrollbar"

    Private Enum StockChangeKind
        Insert
        Remove
    End Enum

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

    Public Sub UpdateScrollbar()

        If IsNothing(_sfd.SFLay.rxStock) Then
            Exit Sub
        End If

        RecalcScrollbarBasis()
        NormalizeVisibleRange()
        SyncScrollbarToVisibleRange()

    End Sub

    Public Sub NotifyStockInsert(insertIdx As Integer)

        RecalcScrollbarBasis()

        If insertIdx < SteinVisibleAktFistIdx Then
            SteinVisibleAktFistIdx += 1
        End If

        NormalizeVisibleRange()
        SyncScrollbarToVisibleRange()

    End Sub

    Public Sub NotifyStockRemove(removeIdx As Integer)

        RecalcScrollbarBasis()

        If removeIdx < SteinVisibleAktFistIdx Then
            SteinVisibleAktFistIdx -= 1
        End If

        NormalizeVisibleRange()
        SyncScrollbarToVisibleRange()

    End Sub

    Private Sub RecalcScrollbarBasis()

        SteineCount = _sfd.SFInf.Generator.Stock.Count
        SteineUBound = SteineCount - 1

        If SteineCount <= 0 Then
            SteineVisibleMaxCount = 0
            SteineVisibleAktCount = 0
            Return
        End If

        'Aufrunden: der letzte Stein darf angeschnitten sichtbar sein.
        SteineVisibleMaxCount = _sfd.SFLay.rxStock.Width \ _sfd.SFLay.steinWidth + 1

        If SteineVisibleMaxCount > SteineCount Then
            SteineVisibleMaxCount = SteineCount
        End If

        SteineVisibleAktCount = SteineVisibleMaxCount

    End Sub

    Private Sub NormalizeVisibleRange()

        If SteineCount <= 0 Then
            SteinVisibleAktFistIdx = 0
            SteinVisibleAktLastIdx = -1
            OffsetLeft = 0

            With _sfd.SFRun.HScrollBarStock
                .SetRange(0)
                .SetSmallChange(1)
                .SetPageSize(1)
                .SetValue(0)
                .ConsumeValueChanged()
            End With

            HScrollBarLastValue = 0
        Else

            If SteineVisibleAktCount <= 0 Then
                SteineVisibleAktCount = 1
            End If

            If SteineVisibleAktCount > SteineCount Then
                SteineVisibleAktCount = SteineCount
            End If

            Dim maxFirstIdx As Integer = SteineCount - SteineVisibleAktCount

            If maxFirstIdx < 0 Then
                maxFirstIdx = 0
            End If

            If SteinVisibleAktFistIdx < 0 Then
                SteinVisibleAktFistIdx = 0
            ElseIf SteinVisibleAktFistIdx > maxFirstIdx Then
                SteinVisibleAktFistIdx = maxFirstIdx
            End If

            SteinVisibleAktLastIdx = SteinVisibleAktFistIdx + SteineVisibleAktCount - 1

            If SteinVisibleAktLastIdx > SteineUBound Then
                SteinVisibleAktLastIdx = SteineUBound
            End If

            AdjustRightEdgeForLastStone()
        End If

        With _sfd.SFRun.HScrollBarStock
            If SteineCount <= 0 Then
                .AktVisibleSteinNumberFrom = 0
                .AktVisibleSteinNumberTo = 0
            Else
                .AktVisibleSteinNumberFrom = SteinVisibleAktFistIdx + 1
                .AktVisibleSteinNumberTo = SteinVisibleAktLastIdx + 1
            End If
        End With

    End Sub

    Private Sub SyncScrollbarToVisibleRange()

        If SteineCount <= 0 Then
            HScrollBarLastValue = 0
            Return
        End If

        With _sfd.SFRun.HScrollBarStock
            .SetRange(SteineUBound)
            .SetSmallChange(1)
            .SetPageSize(SteineVisibleMaxCount)
            .SetValue(SteinVisibleAktFistIdx)

            'Wichtig:
            'Der Owner synchronisiert hier absichtlich.
            'Diese Änderung darf nicht später als Benutzer-Scrollen fehlinterpretiert werden.
            .ConsumeValueChanged()
        End With

        HScrollBarLastValue = SteinVisibleAktFistIdx

    End Sub

    Public Sub SetHScrollBarValue(value As Integer)

        If SteineCount <= 0 Then
            Return
        End If

        If HScrollBarLastValue = value Then
            Return
        End If

        HScrollBarLastValue = value

        SteinVisibleAktFistIdx = value

        NormalizeVisibleRange()
        SyncScrollbarToVisibleRange()

    End Sub

    Public Sub MoveRight(count As Integer)

        If count <= 0 Then
            Return
        End If

        SteinVisibleAktFistIdx += count
        NormalizeVisibleRange()
        SyncScrollbarToVisibleRange()

    End Sub

    Public Sub MoveLeft(count As Integer)

        If count <= 0 Then
            Return
        End If

        SteinVisibleAktFistIdx -= count
        NormalizeVisibleRange()
        SyncScrollbarToVisibleRange()

    End Sub

    Private Sub AdjustRightEdgeForLastStone()

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

        Dim visibleCount As Integer = SteinVisibleAktLastIdx - SteinVisibleAktFistIdx + 1

        If visibleCount <= 0 Then
            Return 0
        End If

        Dim value1 As Integer = visibleCount * _sfd.SFLay.steinWidth
        Dim value2 As Integer = value1 - _sfd.SFLay.rxStock.Width

        If value2 > 0 Then
            Return -value2
        End If

        Return 0

    End Function

#End Region

End Class
