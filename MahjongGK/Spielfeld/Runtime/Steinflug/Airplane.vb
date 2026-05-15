Imports MahjongGK.Contracts.GlobalEnum
Imports TileFactory

Namespace Spielfeld

    ''' <summary>
    ''' Klasse, die für den Steinflug einen einzelnen fliegenden Stein hält.
    ''' Sie muss aktiv entsorgt werden, indem Dispose aufgerufen wird.
    ''' </summary>
    Public Class Airplane
        Implements IDisposable

#Region "Konstruktor"

        Sub New()

        End Sub

        Public Shared Function CreateEditorDragDropAirplane(sfd As SFDaten, steinInfoIndex As Integer) As Airplane
            Return New Airplane(sfd, steinInfoIndex, dragDrop:=True)
        End Function
        Public Shared Function CreateEditorFlyBackInsideEditorAirplane(sfd As SFDaten, steinInfoIndex As Integer) As Airplane
            Dim plane As New Airplane(sfd, steinInfoIndex, dragDrop:=False)
            Dim ptZiel As Point = sfd.SFInf.SteinInfos(steinInfoIndex).RectStein.Location
            Dim ptStart As Point = sfd.SFRun.MousePolling.MousePos
            Dim pfp As New AirplanesFlightPath(ptStart, ptZiel, sfd.SFLay.steinSize, PlaneFlightPath.Zufall, 20, ptStartIsMouseAnkerPos:=True)
            plane.PlanesFlightPath = pfp
            Return plane
        End Function

        '
        ''' <summary>
        ''' Überladung für eine Stein aus dem Spielfeld für DragDrop
        ''' </summary>
        ''' <param name="sfd"></param>
        ''' <param name="steinInfoIndex"></param>
        Sub New(sfd As SFDaten, steinInfoIndex As Integer, dragDrop As Boolean)
            _sfd = sfd
            _dragDropAktiv = dragDrop
            _dragDropFirstStep = dragDrop
            _steinInfoIndex = steinInfoIndex
            _steinTypIndex = sfd.SFInf.SteinInfos(steinInfoIndex).SteinTypIndex
            _stockIndex = -1
            _srcPos3D = sfd.SFInf.SteinInfos(steinInfoIndex).Pos3D
            Initialisierung(srcRect:=Nothing) 'holt sich srcRect aus den SteinInfos
            InitRenderBitmapBuffer()
        End Sub
        '
        ''' <summary>
        ''' Überladung für eine Stein aus dem Vorrat für DragDrop
        ''' </summary>
        ''' <param name="sfd"></param>
        ''' <param name="stockIndex"></param>
        ''' <param name="steinIndex"></param>
        ''' <param name="srcRect"></param>
        Sub New(sfd As SFDaten, stockIndex As Integer, steinIndex As SteinTyp, srcRect As Rectangle, dragDrop As Boolean)
            _sfd = sfd
            _steinInfoIndex = -1
            _stockIndex = stockIndex
            _dragDropAktiv = dragDrop
            _dragDropFirstStep = dragDrop
            _steinTypIndex = steinIndex
            _srcPos3D = Nothing
            Initialisierung(srcRect)
        End Sub

        Private ReadOnly _sfd As SFDaten

#End Region

#Region "AirplanesFlightPath"

        Private _PlanesFlightPath As AirplanesFlightPath = Nothing
        Public Property PlanesFlightPath As AirplanesFlightPath
            Get
                Return _PlanesFlightPath
            End Get
            Set(value As AirplanesFlightPath)
                _PlanesFlightPath = value
            End Set
        End Property

#End Region

#Region "InitDragDropBitmaps"

        Private Sub Initialisierung(srcRect As Rectangle)
            '
            'Da beliebig viele Airplanes gleichzeitig fliegen können, müssen die
            'Bitmaps Eigentum der Planes sein, d.h. es müssen DeepCopies gemacht werden.
            DisposeOwnedBitmaps()

            Dim request As TileRequest

            If _steinInfoIndex >= 0 Then
                'Stein aus dem Feld
                request = New TileRequest(_sfd.SFRun.AktRenderMode, INI.Tile_TileColors, _sfd.SFInf.SteinInfos(_steinInfoIndex).SteinTypIndex, SteinStatus.I02Selected, SteinFrameVersion.Standard, _sfd.SFLay.steinSize, INI.Tile_BasisSize)
                _bmpOrg = TileFactory.GetTileDeepCopy(request)
            Else
                'Stein aus dem Vorrat
                'Falls _steinTypIndex ungültig ist. wird die Errorgrafik erzeugt
                request = New TileRequest(_sfd.SFRun.AktRenderMode, INI.Tile_TileColors, _steinTypIndex, SteinStatus.I02Selected, SteinFrameVersion.Standard, _sfd.SFLay.steinSize, INI.Tile_BasisSize)
                _bmpOrg = TileFactory.GetTileDeepCopy(request)
            End If

            request.SetSteinFrameVersion(SteinFrameVersion.Standard, ghost:=True)
            _bmpGhost = TileFactory.GetTileDeepCopy(request)
            '
            request.SetSteinFrameVersion(SteinFrameVersion.MouseSelected, ghost:=False)
            _bmpSelected = TileFactory.GetTileDeepCopy(request)

            request.SetSteinFrameVersion(SteinFrameVersion.MouseCanDrop, ghost:=False)
            _bmpCanDrop = TileFactory.GetTileDeepCopy(request)

            If _steinInfoIndex >= 0 Then
                _JobSelector = New AirJobSelector(_sfd, _steinInfoIndex)
                _srcRect = _sfd.SFInf.SteinInfos(_steinInfoIndex).RectStein
            Else
                _JobSelector = New AirJobSelector(_sfd, _steinTypIndex)
                _srcRect = srcRect
            End If

        End Sub

        Public Sub SetDstRect(steinInfoIndex As Integer)

        End Sub

#End Region

#Region "DragDrop-Zustandswerte und Subs"

        Private _dragDropAktiv As Boolean
        Private _dragDropFirstStep As Boolean
        Private _renderCallsCounter As Integer
        Private _animationMaxSteps As Integer
        '
        ''' <summary>
        ''' Zur Bestimmung des aktuellen RenderRectangle aus der
        ''' aktuellen Mausposition.
        ''' Hat keinerlei Bezüge innerhalb AirPlane. Wird hier
        ''' nur eingehängt, damit die diesem AirPlane zugeordnete
        ''' Instanz der AirPlanePosition leicht im Zugriff ist.
        ''' </summary>
        ''' <returns></returns>
        Public Property MouseAnkerPosition As MouseAnkerPosition = Nothing
        '
        ''' <summary>
        ''' Wird über den Konstruktor gesetzt
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property DragDropAktiv As Boolean
            Get
                Return _dragDropAktiv
            End Get
        End Property
        '
        ''' <summary>
        ''' Wird über den Konstruktor gesetzt und dient beim Pollig im Rendern
        ''' dazu die Initialisierungsphase zu erkennen.
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property ConsumeDragDropFirstStep As Boolean
            Get
                If _dragDropFirstStep Then
                    _dragDropFirstStep = False
                    Return True
                Else
                    Return False
                End If
            End Get
        End Property
        '
        ''' <summary>
        ''' Wird jedesmal hochgezählt, wenn der Renderer GetPlaneFromSteinInfoIndex
        ''' oder GetPlaneFromStockIndex aufruft. (In SFAirflight #Region "IndexToPlane-Verwaltung")
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property RenderCallsCounter As Integer
            Get
                Return _renderCallsCounter
            End Get
        End Property

        Public Sub IncRenderCallsCounter()
            _renderCallsCounter += 1
        End Sub
        ''
        '''' <summary>
        '''' Die Anzahl der Animationsschritte bis zu Fertigstellung.
        '''' </summary>
        '''' <returns></returns>
        'Public Property AnimationMaxSteps As Integer
        '    Get
        '        Return _animationMaxSteps
        '    End Get
        '    Set(value As Integer)
        '        _animationMaxSteps = value
        '    End Set
        'End Property
        '
        '''' <summary>
        '''' Wenn AnimationMaxSteps nicht gesetzt ist immer False.
        '''' Wenn gesetzt, True wenn RenderCallsCounter >= AnimationMaxSteps
        '''' </summary>
        '''' <returns></returns>
        'Public Function AnimationIsRedy() As Boolean
        '    If _animationMaxSteps = 0 Then
        '        Return False
        '    End If
        '    Return _renderCallsCounter >= _animationMaxSteps
        'End Function
#End Region

#Region "ReadOnly-Properties"

        Private _bmpOrg As Bitmap = Nothing
        Private _bmpGhost As Bitmap = Nothing
        Private _bmpSelected As Bitmap = Nothing
        Private _bmpCanDrop As Bitmap = Nothing
        Private _steinInfoIndex As Integer
        Private _stockIndex As Integer
        Private _steinTypIndex As SteinTyp
        Private _srcRect As Rectangle
        Private _srcPos3D As Triple
        'ja

        Public ReadOnly Property SteinInfoIndex As Integer
            Get
                Return _steinInfoIndex
            End Get
        End Property

        ''' <summary>
        ''' Wenn HasSteinInfoIndex False ist, wird SteinTyp.ErrorSy zurückgegeben,
        ''' (SteinTyp ist der Index auf die Bitmap des Steines) 
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property SteinTypIndex As SteinTyp
            Get
                Return _steinTypIndex
            End Get
        End Property

        Public ReadOnly Property StockIndex As Integer
            Get
                Return _stockIndex
            End Get
        End Property

        Public ReadOnly Property SteinSize As Size
            Get
                Return _bmpOrg.Size
            End Get
        End Property

        Public ReadOnly Property HasSteinInfoIndex As Boolean
            Get
                Return _steinInfoIndex >= 0
            End Get
        End Property

        Public ReadOnly Property HasSteinIndex As Boolean
            Get
                Return _steinInfoIndex < 0
            End Get
        End Property

        '
        Private _JobSelector As AirJobSelector = Nothing
        Public ReadOnly Property JobSelector As AirJobSelector
            Get
                Return _JobSelector
            End Get
        End Property
        '
        ''' <summary>
        ''' Wie bewegt sich der Stein. 
        ''' Mauskoppelung, animiert, nicht animierte, auflösend. 
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property PlaneModus As AirplaneModus
            Get
                Return _JobSelector.PlaneModus
            End Get
        End Property
        '
        ''' <summary>
        ''' Was für eine Aktion soll ausgeführt werden.
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property FlightRoute As AirFlightRoute
            Get
                Return _JobSelector.FlightRoute
            End Get
        End Property
        '
        ''' <summary>
        ''' Wo wurde geklickt
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property KlickArea As AirKlickArea
            Get
                Return _JobSelector.KlickArea
            End Get
        End Property
        '
        Public ReadOnly Property SrcPos3D As Triple
            Get
                Return _srcPos3D
            End Get
        End Property

        ''' <summary>
        ''' Das Renderrechteck des Steines am Quellort (ggf Ghost)
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property SrcRect As Rectangle
            Get
                Return _srcRect
            End Get
        End Property

        ''' <summary>
        ''' Das ist eine DeepCopy.
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property BmpOrg As Bitmap
            Get
                Return _bmpOrg
            End Get
        End Property
        '
        Public ReadOnly Property BmpGhost As Bitmap
            Get
                Return _bmpGhost
            End Get
        End Property
        '
        Public ReadOnly Property BmpSelected As Bitmap
            Get
                Return _bmpSelected
            End Get
        End Property
        '
        Public ReadOnly Property BmpPlacable As Bitmap
            Get
                Return _bmpCanDrop
            End Get
        End Property

#End Region

#Region "Verwaltung zu rendernder Bitmaps"

        '
        ''' <summary>
        ''' Anfangsgröße des Puffers.
        ''' Im Normalfall reichen wenige Einträge.
        ''' </summary>
        Private Const RENDERBITMAPBUFFERSTARTSIZE As Integer = 3

        '
        ''' <summary>
        ''' Vergrößerungsschritt des Puffers.
        ''' Dient als Reserve, falls künftig doch mehr Renderbitmaps
        ''' pro Airplane benötigt werden.
        ''' </summary>
        Private Const RENDERBITMAPBUFFERGROWSTEP As Integer = 10

        Private _renderRect() As Rectangle = Nothing
        Private _renderBitmap() As Bitmap = Nothing

        Private _nextFreeRenderBitmapIndex As Integer
        Private _nextReadRenderBitmapIndex As Integer

        Private _renderBitmapNeedsClear As Boolean

        '
        ''' <summary>
        ''' Solange True, wie noch eine zu rendernde Bitmap vorhanden ist.
        ''' </summary>
        Public ReadOnly Property RenderBitmapIsAvailable As Boolean
            Get
                Return _nextFreeRenderBitmapIndex > 0
            End Get
        End Property

        '
        ''' <summary>
        ''' Muss einmal beim Erzeugen der Instanz aufgerufen werden.
        ''' </summary>
        Private Sub InitRenderBitmapBuffer()

            ReDim _renderRect(RENDERBITMAPBUFFERSTARTSIZE - 1)
            ReDim _renderBitmap(RENDERBITMAPBUFFERSTARTSIZE - 1)

            _nextFreeRenderBitmapIndex = 0
            _nextReadRenderBitmapIndex = 0

            _renderBitmapNeedsClear = False

        End Sub

        '
        ''' <summary>
        ''' Löscht intern alle Bitmap-Referenzen.
        ''' Es wird nichts disposed, sondern nur auf Nothing gesetzt.
        ''' </summary>
        Private Sub ClearRenderBitmapBufferInternal()

            Dim i As Integer

            For i = 0 To _renderBitmap.GetUpperBound(0)
                _renderBitmap(i) = Nothing
            Next

            _nextFreeRenderBitmapIndex = 0
            _nextReadRenderBitmapIndex = 0

            _renderBitmapNeedsClear = False

        End Sub

        '
        ''' <summary>
        ''' Gibt den nächsten Render-Eintrag zurück.
        ''' Die Ausgabereihenfolge entspricht der Einfügereihenfolge.
        ''' Wenn kein Eintrag mehr vorhanden ist, werden die Indizes
        ''' zurückgesetzt. Die eigentlichen Referenzen werden erst vor
        ''' dem nächsten Add vollständig gelöscht.
        ''' </summary>
        ''' <returns>
        ''' True, wenn ein Eintrag vorhanden war sowie die Bitmap mit Rectangle, sonst False.
        ''' </returns>
        Public Function NextRenderBitmap() As (found As Boolean, bmp As Bitmap, rect As Rectangle)

            If _nextReadRenderBitmapIndex < _nextFreeRenderBitmapIndex Then
                _nextReadRenderBitmapIndex += 1
                Return (True, _renderBitmap(_nextReadRenderBitmapIndex), _renderRect(_nextReadRenderBitmapIndex))
            Else
                _nextFreeRenderBitmapIndex = 0
                _nextReadRenderBitmapIndex = 0
                _renderBitmapNeedsClear = True

                Return (False, Nothing, Rectangle.Empty)
            End If

        End Function

        '
        ''' <summary>
        ''' Fügt eine zu rendernde Bitmap mit Zielrechteck ein.
        ''' Der Eintrag wird hinten angehängt.
        ''' </summary>
        Public Sub AddRenderSteinInfoIndexZOrder(rect As Rectangle, bmp As Bitmap)

            If _renderBitmapNeedsClear Then
                ClearRenderBitmapBufferInternal()
            End If

            If _nextFreeRenderBitmapIndex > _renderRect.GetUpperBound(0) Then
                ReDim Preserve _renderRect(_renderRect.GetUpperBound(0) + RENDERBITMAPBUFFERGROWSTEP)
                ReDim Preserve _renderBitmap(_renderBitmap.GetUpperBound(0) + RENDERBITMAPBUFFERGROWSTEP)
            End If

            _renderRect(_nextFreeRenderBitmapIndex) = rect
            _renderBitmap(_nextFreeRenderBitmapIndex) = bmp

            _nextFreeRenderBitmapIndex += 1

        End Sub

#End Region

#Region "Start"

        Private _planeStepNumber As Integer
        '
        ''' <summary>
        ''' Gibt den aktuellen Animationsschritt zurück.
        ''' Solange StartPlane nicht aufgerufen wurde, ist der Wert 0.
        ''' </summary>
        Public ReadOnly Property PlaneStepNumber As Integer
            Get
                Return _planeStepNumber
            End Get
        End Property

        '
        ''' <summary>
        ''' Startet den Flug.
        ''' Der erste gültige Animationsschritt ist danach 1.
        ''' </summary>
        Public Sub StartPlane()
            _planeStepNumber = 1
        End Sub

        '
        ''' <summary>
        ''' Liefert den aktuellen Animationsschritt und erhöht ihn anschließend
        ''' für den nächsten Abruf.
        ''' Solange StartPlane nicht aufgerufen wurde, wird 0 zurückgegeben.
        ''' </summary>
        Public Function GetAndIncrementPlaneStepNumber() As Integer

            If _planeStepNumber <= 0 Then
                Return 0
            End If

            Dim result As Integer = _planeStepNumber
            _planeStepNumber += 1
            Return result

        End Function

#End Region

#Region "Helfer"

        'Dim dist As Double = GetFlightDistance(pt1, pt2)

        'Dim stepsVisible As Integer = GetFlightStepCount(pt1, pt2, FlightStepMode.Euclidean)
        'Dim stepsRaster As Integer = GetFlightStepCount(pt1, pt2, FlightStepMode.Raster)

        ''' <summary>
        ''' Liefert die geometrische Strecke zwischen zwei Punkten in Pixel.
        ''' </summary>
        Public Function GetFlightDistance(pt1 As Point, pt2 As Point) As Double
            Dim dx As Integer = pt2.X - pt1.X
            Dim dy As Integer = pt2.Y - pt1.Y

            Return Math.Sqrt(CDbl(dx * dx) + CDbl(dy * dy))
        End Function

        ''' <summary>
        ''' Liefert die Anzahl der maximal möglichen Renderschritte zwischen zwei Punkten.
        ''' </summary>
        Public Function GetFlightStepCount(pt1 As Point,
                                           pt2 As Point,
                                           mode As AirFlightStepMode) As Integer

            Dim dx As Integer = Math.Abs(pt2.X - pt1.X)
            Dim dy As Integer = Math.Abs(pt2.Y - pt1.Y)

            Select Case mode
                Case AirFlightStepMode.Euclidean
                    Return CInt(Math.Ceiling(Math.Sqrt(CDbl(dx * dx) + CDbl(dy * dy))))

                Case AirFlightStepMode.Raster
                    Return Math.Max(dx, dy)

                Case Else
                    Throw New ArgumentOutOfRangeException(NameOf(mode))
            End Select

        End Function

#End Region

#Region "Dispose"

        Private _disposed As Boolean

        Private Sub ThrowIfDisposed()
            If _disposed Then
                Throw New ObjectDisposedException(Me.GetType().Name)
            End If
        End Sub

        Private Sub DisposeOwnedBitmaps()

            If _bmpOrg IsNot Nothing Then
                _bmpOrg.Dispose()
                _bmpOrg = Nothing
            End If

            If _bmpGhost IsNot Nothing Then
                _bmpGhost.Dispose()
                _bmpGhost = Nothing
            End If

            If _bmpSelected IsNot Nothing Then
                _bmpSelected.Dispose()
                _bmpSelected = Nothing
            End If

            If _bmpCanDrop IsNot Nothing Then
                _bmpCanDrop.Dispose()
                _bmpCanDrop = Nothing
            End If

        End Sub

        Public Sub Dispose() Implements IDisposable.Dispose

            If _disposed Then
                Return
            End If
            DisposeOwnedBitmaps()

            GC.SuppressFinalize(Me)

            _disposed = True

        End Sub

#End Region

    End Class

End Namespace