
Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

#Disable Warning IDE0079
#Disable Warning IDE1006
#Disable Warning IDE0031
#Disable Warning IDE0017

Namespace Spielfeld

    Public Enum AirRenderJob
        None
        Spielfeld
        Editor
        Stock
    End Enum

    Public Enum AirBitmapStyle
        Normal
        Selected
        Ghost
    End Enum
    'Hinweis: Die Begriffe "AirPlane" und "Plane" verwende ich Synonym,
    'Plane ist eine Instanz oder eine Variable
    'wie z.B. Dim plane As AirPlane.
    '

    '' Deklariert im SteinflugAnimator
    ''Public Enum AirplaneAnimation
    ''    None = 0
    ''    ScaleDown = 1
    ''    ScaleUp = 2
    ''    Rotate = 3
    ''    RotateShrink = 4
    ''    FlipX = 5
    ''    FlipY = 6
    ''    FlipShrink = 7
    ''    Pulse = 8
    ''    SlideLeft = 9
    ''    SlideUp = 10
    ''    ScaleSlide = 11
    ''    RotatePulse = 12
    ''End Enum

    '
    ''' <summary>
    ''' Die Klasse bündelt alle fliegenden Steine im Spiel.
    ''' Es können beliebig viele Steine gleichzeitig fliegen.
    ''' Wie alle SFxx-Klassen hat auch diese Klasse Zugriff auf alle anderen
    ''' SFxx Klassen.
    ''' </summary>
    Public Class SFAirflight

#Region "Deklarationen"

        Private ReadOnly _sfd As SFDaten
        Private ReadOnly _loAirPlanesStock As List(Of Airplane)
        Private ReadOnly _loAirPlanesSFld As List(Of Airplane)
        Private ReadOnly _loAirPlanesEdit As List(Of Airplane)

        Private _nextCenterPoint As Point
        Private _nextTopMost As Boolean
        Private _aktAirPlane As Airplane

#End Region

#Region "Konstruktor"

        Public Sub New()
        End Sub

        Public Sub New(owner As SFDaten)
            _sfd = owner
            _loAirPlanesStock = New List(Of Airplane)
            _loAirPlanesSFld = New List(Of Airplane)
            _loAirPlanesEdit = New List(Of Airplane)

        End Sub

#End Region

#Region "Öffentliches"
        Public Sub Clear()
            _loAirPlanesSFld.Clear()
            _loAirPlanesEdit.Clear()
            _loAirPlanesStock.Clear()
        End Sub
        '
        ''' <summary>
        ''' Zum Aufruf aus dem RenderManager heraus zur Steuerung von doRender.
        ''' </summary>
        ''' <returns></returns>
        Public Function HasAnyRenderJob() As Boolean

            If _loAirPlanesSFld IsNot Nothing AndAlso _loAirPlanesSFld.Count > 0 Then
                Return True
            End If
            If _loAirPlanesEdit IsNot Nothing AndAlso _loAirPlanesEdit.Count > 0 Then
                Return True
            End If
            If _loAirPlanesStock IsNot Nothing AndAlso _loAirPlanesStock.Count > 0 Then
                Return True
            End If

            Return False

        End Function
        '
        ''' <summary>
        ''' zum Aufruf aus Paint_Spielfeld heraus.
        ''' </summary>
        ''' <returns></returns>
        Public Function HasSpielfeldRenderJob(steinInfoIndex As Integer, timeDifferenzFaktor As Double) As Boolean

            'das ist jetzt identisch dem HasEditorRenderJob, muss aber nicht so bleiben.

            If _loAirPlanesSFld Is Nothing OrElse _loAirPlanesSFld.Count = 0 Then
                Return False
            End If

            Dim aktPlane As Airplane = Nothing

            For Each plane As Airplane In _loAirPlanesSFld
                If plane.SteinInfoIndex = steinInfoIndex Then
                    aktPlane = plane
                    Exit For
                End If
            Next

            If aktPlane Is Nothing Then
                Return False
            End If

            Dim ncp As (pt As Point, toMost As Boolean) = aktPlane.GetNextCenterPoint(timeDifferenzFaktor)
            _nextCenterPoint = ncp.pt
            _nextTopMost = ncp.toMost

            If _nextCenterPoint.IsEmpty Then
                _loAirPlanesSFld.Remove(aktPlane)
                _aktAirPlane = Nothing
            Else
                _aktAirPlane = aktPlane
            End If

            Return Not _nextCenterPoint.IsEmpty

        End Function
        '
        ''' <summary>
        ''' zum Aufruf aus Paint_Editor heraus.
        ''' Hinweis: Sowohl für den Hinflug aus dem Steinorrat als für den Rückflug beim Abbruch den DragDrop
        ''' wird die SteinfeldInfo des Steines benötigt. Wenn diese dem Spielfeld hinzugefügt wird, wird
        ''' normalerweise der Stein beim nächsten Rendern geblittet. Nicht so, wenn ein Steinflug aktiv ist:
        ''' da zunächst gesucht wird, ob unter dem SteinfeldInfoIndex eine AnimationsBitmap vorhanden ist und,
        ''' wenn ja, wird diese geblittet und nicht die normale Bitmap.
        ''' </summary>
        ''' <returns></returns>
        Public Function HasEditorRenderJob(steinInfoIndex As Integer, timeDifferenzFaktor As Double) As Boolean

            If _loAirPlanesEdit Is Nothing OrElse _loAirPlanesEdit.Count = 0 Then
                Return False
            End If

            Dim aktPlane As Airplane = Nothing

            For Each plane As Airplane In _loAirPlanesEdit
                If plane.SteinInfoIndex = steinInfoIndex Then
                    aktPlane = plane
                    Exit For
                End If
            Next

            If aktPlane Is Nothing Then
                Return False
            End If
            Dim ncp As (pt As Point, toMost As Boolean) = aktPlane.GetNextCenterPoint(timeDifferenzFaktor)
            _nextCenterPoint = ncp.pt
            _nextTopMost = ncp.toMost

            If _nextCenterPoint.IsEmpty Then
                _loAirPlanesEdit.Remove(aktPlane)
                _aktAirPlane = Nothing
            Else
                _aktAirPlane = aktPlane
            End If

            Return Not _nextCenterPoint.IsEmpty

        End Function
        '
        ''' <summary>
        ''' Zum Aufruf aus Paint_Stock heraus.
        ''' </summary>
        ''' <returns></returns>
        Public Function HasStockRenderJob(stockIndex As Integer, timeDifferenzFaktor As Double) As Boolean

            If _loAirPlanesStock Is Nothing OrElse _loAirPlanesStock.Count = 0 Then
                Return False
            End If

            Dim aktPlane As Airplane = Nothing

            For Each plane As Airplane In _loAirPlanesStock
                If plane.StockIndex = stockIndex Then
                    aktPlane = plane
                    Exit For
                End If
            Next

            If aktPlane Is Nothing Then
                Return False
            End If

            Dim ncp As (pt As Point, toMost As Boolean) = aktPlane.GetNextCenterPoint(timeDifferenzFaktor)
            _nextCenterPoint = ncp.pt
            _nextTopMost = ncp.toMost

            If _nextCenterPoint.IsEmpty Then
                _loAirPlanesStock.Remove(aktPlane)
                _aktAirPlane = Nothing
            Else
                _aktAirPlane = aktPlane
            End If

            Return Not _nextCenterPoint.IsEmpty

        End Function

        Public Sub AddPlane(plane As Airplane)
            If plane.RenderJob = AirRenderJob.None Then
                Throw New Exception("Airplane falsch instanziert. _AirRenderJob = AirRenderJob.None darf nicht sein.")
            End If
            Select Case plane.RenderJob
                Case AirRenderJob.Spielfeld
                    _loAirPlanesSFld.Add(plane)
                Case AirRenderJob.Editor
                    _loAirPlanesEdit.Add(plane)
                Case AirRenderJob.Stock
                    _loAirPlanesStock.Add(plane)
                Case Else
                    Throw New Exception("Unbekannte Enumeration")
            End Select
        End Sub

        '
        ''' <summary>
        ''' Gibt den nächsten CenterPoint zurück, aber nur unnmittelbat nachdem einer der
        ''' Funktionen HasStockRenderJob, HasEditorRenderJob oder HasSpielfeldRenderJob
        ''' = True ist.
        ''' 
        ''' </summary>
        ''' <returns></returns>
        Public Function GetNextCenterPoint() As Point
            If _aktAirPlane Is Nothing Then
                Throw New Exception("Einer der Has...-Aufrufe in dieser Klasse wurde nicht gemacht.")
            End If
            Return _nextCenterPoint
        End Function

        '
        ''' <summary>
        ''' Die Standardfunktion für die nächste Bitmap in der Standardgröße samt zugehörgem Ausgabe-Rectangle.
        ''' Wenn das Ziel errreicht ist, ist hasValue = False.
        ''' </summary>
        ''' <param name="bitmapStyle"></param>
        ''' <returns></returns>
        Public Function GetNextRenderBitmap(bitmapStyle As AirBitmapStyle) As (bmp As Bitmap, rect As Rectangle, topMost As Boolean)
            If _aktAirPlane Is Nothing Then
                Throw New Exception("Einer der Has...-Aufrufe in dieser Klasse wurde nicht gemacht.")
            End If
            Return _aktAirPlane.GetNextRenderBitmap(_nextCenterPoint, bitmapStyle)
        End Function
        Public Function GetStartRenderBitmap(bitmapStyle As AirBitmapStyle) As (bmp As Bitmap, rect As Rectangle)
            If _aktAirPlane Is Nothing Then
                Throw New Exception("Einer der Has...-Aufrufe in dieser Klasse wurde nicht gemacht.")
            End If
            Return _aktAirPlane.GetStartRenderBitmap(bitmapStyle)
        End Function
        Public Function GetZielRenderBitmap(bitmapStyle As AirBitmapStyle) As (bmp As Bitmap, rect As Rectangle)
            If _aktAirPlane Is Nothing Then
                Throw New Exception("Einer der Has...-Aufrufe in dieser Klasse wurde nicht gemacht.")
            End If
            Return _aktAirPlane.GetZielRenderBitmap(bitmapStyle)
        End Function
#End Region

#Region "Privates"
#End Region

    End Class
End Namespace

'Ab hier ZLV
' '''        Implements IDisposable

'    '    '#Region "Konstruktor"

'    '    '        Public Sub New()
'    '    '        End Sub

'    '    '        Public Sub New(owner As SFDaten)
'    '    '            _sfd = owner
'    '    '            InitialisierungIndexToPlane(steinInfoCount:=250, stockCount:=250, growStepSteinInfo:=125, growStepStock:=125)
'    '    '            RenderBuffer = New AirRenderBuffer(bufferSize:=10, growStep:=250)
'    '    '        End Sub

'    '    '        Private ReadOnly _sfd As SFDaten

'    '    '#End Region

'    '    '#Region "ReadOnly Properties"

'    '    '        Public ReadOnly Property AktSteinInfoIndex As Integer
'    '    '            Get
'    '    '                Return _aktPlane.SteinInfoIndex
'    '    '            End Get
'    '    '        End Property

'    '    '        ''' <summary>
'    '    '        ''' Wenn HasSteinInfoIndex False ist, wird SteinSymbol.ErrorSy zurückgegeben,
'    '    '        ''' (SteinSymbol ist der Index auf die Bitmap des Steines) 
'    '    '        ''' </summary>
'    '    '        ''' <returns></returns>
'    '    '        Public ReadOnly Property AktSteinIndex As SteinSymbol
'    '    '            Get
'    '    '                Return _aktPlane.SteinSymbolIndex
'    '    '            End Get
'    '    '        End Property

'    '    '        Public ReadOnly Property AktSteinSize As Size
'    '    '            Get
'    '    '                Return _aktPlane.BmpOrg.Size
'    '    '            End Get
'    '    '        End Property

'    '    '        Public ReadOnly Property AktHasSteinInfoIndex As Boolean
'    '    '            Get
'    '    '                Return _aktPlane.SteinInfoIndex >= 0
'    '    '            End Get
'    '    '        End Property

'    '    '        Public ReadOnly Property AktHasSteinIndex As Boolean
'    '    '            Get
'    '    '                Return _aktPlane.SteinInfoIndex < 0
'    '    '            End Get
'    '    '        End Property

'    '    '        '
'    '    '        Private _JobSelector As AirJobSelector = Nothing
'    '    '        Public ReadOnly Property AktJobSelector As AirJobSelector
'    '    '            Get
'    '    '                Return _aktPlane.JobSelector
'    '    '            End Get
'    '    '        End Property
'    '    '        '
'    '    '        ''' <summary>
'    '    '        ''' Wie bewegt sich der Stein. 
'    '    '        ''' Mauskoppelung, animiert, nicht animierte, auflösend. 
'    '    '        ''' </summary>
'    '    '        ''' <returns></returns>
'    '    '        Public ReadOnly Property AktPlaneModus As AirplaneModus
'    '    '            Get
'    '    '                Return _aktPlane.PlaneModus
'    '    '            End Get
'    '    '        End Property
'    '    '        '
'    '    '        ''' <summary>
'    '    '        ''' Was für eine Aktion soll ausgeführt werden.
'    '    '        ''' </summary>
'    '    '        ''' <returns></returns>
'    '    '        Public ReadOnly Property AktFlightRoute As AirFlightRoute
'    '    '            Get
'    '    '                Return _aktPlane.FlightRoute
'    '    '            End Get
'    '    '        End Property
'    '    '        '
'    '    '        ''' <summary>
'    '    '        ''' Wo wurde geklickt
'    '    '        ''' </summary>
'    '    '        ''' <returns></returns>
'    '    '        Public ReadOnly Property AktKlickArea As AirKlickArea
'    '    '            Get
'    '    '                Return _aktPlane.KlickArea
'    '    '            End Get
'    '    '        End Property
'    '    '        '
'    '    '        ''' <summary>
'    '    '        ''' Das Renderrechteck des Steines am Quellort (ggf Ghost)
'    '    '        ''' </summary>
'    '    '        ''' <returns></returns>
'    '    '        Public ReadOnly Property AktSrcRect As Rectangle
'    '    '            Get
'    '    '                Return _aktPlane.SrcRect
'    '    '            End Get
'    '    '        End Property
'    '    '        '
'    '    '        ''' <summary>
'    '    '        ''' Das Render-Rechteck des Steines am Zielort (ggf Ghost)
'    '    '        ''' </summary>
'    '    '        ''' <returns></returns>
'    '    '        Public ReadOnly Property AktDstRect As Rectangle
'    '    '        '
'    '    '        ''' <summary>
'    '    '        ''' Das aktuelle Render-Rechteck des Steines
'    '    '        ''' </summary>
'    '    '        ''' <returns></returns>
'    '    '        Public ReadOnly Property AktRenderRect As Rectangle

'    '    '        ''' <summary>
'    '    '        ''' Das ist eine DeepCopy.
'    '    '        ''' </summary>
'    '    '        ''' <returns></returns>
'    '    '        Public ReadOnly Property AktBmpOrg As Bitmap
'    '    '            Get
'    '    '                Return _aktPlane.BmpOrg
'    '    '            End Get
'    '    '        End Property
'    '    '        '
'    '    '        Public ReadOnly Property AktBmpGhost As Bitmap
'    '    '            Get
'    '    '                Return _aktPlane.BmpGhost
'    '    '            End Get
'    '    '        End Property
'    '    '        '
'    '    '        Public ReadOnly Property AktBmpSelected As Bitmap
'    '    '            Get
'    '    '                Return _aktPlane.BmpSelected
'    '    '            End Get
'    '    '        End Property
'    '    '        '
'    '    '        Public ReadOnly Property AktBmpPlacable As Bitmap
'    '    '            Get
'    '    '                Return _aktPlane.BmpPlacable
'    '    '            End Get
'    '    '        End Property

'    '    '#End Region

'    '    '#Region "IndexToPlane-Verwaltung"
'    '    '        '
'    '    '        ''' <summary>
'    '    '        ''' Direkte Zuordnung: SteinInfoIndex -> AirPlane.
'    '    '        ''' Nicht zugeordnete Einträge sind Nothing.
'    '    '        ''' </summary>
'    '    '        Private _planeFromSteinInfoIndex() As Airplane = Nothing
'    '    '        '
'    '    '        ''' <summary>
'    '    '        ''' Direkte Zuordnung: StockIndex -> AirPlane.
'    '    '        ''' Nicht zugeordnete Einträge sind Nothing.
'    '    '        ''' </summary>
'    '    '        Private _planeFromStockIndex() As Airplane = Nothing
'    '    '        '
'    '    '        ''' <summary>
'    '    '        ''' Vergrößerungsschritt für SteinInfoIndex -> AirPlane.
'    '    '        ''' </summary>
'    '    '        Private _growStepSteinInfoIndexToPlane As Integer = 64
'    '    '        '
'    '    '        ''' <summary>
'    '    '        ''' Vergrößerungsschritt für StockIndex -> AirPlane.
'    '    '        ''' </summary>
'    '    '        Private _growStepStockIndexToPlane As Integer = 32
'    '    '        '
'    '    '        ''' <summary>
'    '    '        ''' Initialisiert die direkten Index->Plane-Zuordnungen.
'    '    '        ''' Muss genau einmal beim Erzeugen von SFAirFlight aufgerufen werden.
'    '    '        ''' steinInfoCount ist die maximal bekannte oder zunächst erwartete Anzahl.
'    '    '        ''' Falls später größere Indizes benötigt werden, werden die Arrays automatisch erweitert.
'    '    '        ''' stockCount ist die anfänglich erwartete Größe des Stock-Bereichs.
'    '    '        ''' </summary>
'    '    '        Private Sub InitialisierungIndexToPlane(steinInfoCount As Integer,
'    '    '                                                stockCount As Integer,
'    '    '                                                growStepSteinInfo As Integer,
'    '    '                                                growStepStock As Integer)

'    '    '            If steinInfoCount < 0 Then
'    '    '                Throw New ArgumentOutOfRangeException(NameOf(steinInfoCount))
'    '    '            End If

'    '    '            If stockCount < 0 Then
'    '    '                Throw New ArgumentOutOfRangeException(NameOf(stockCount))
'    '    '            End If

'    '    '            If growStepSteinInfo <= 0 Then
'    '    '                Throw New ArgumentOutOfRangeException(NameOf(growStepSteinInfo))
'    '    '            End If

'    '    '            If growStepStock <= 0 Then
'    '    '                Throw New ArgumentOutOfRangeException(NameOf(growStepStock))
'    '    '            End If

'    '    '            _growStepSteinInfoIndexToPlane = growStepSteinInfo
'    '    '            _growStepStockIndexToPlane = growStepStock

'    '    '            If steinInfoCount = 0 Then
'    '    '                _planeFromSteinInfoIndex = Array.Empty(Of Airplane)()
'    '    '            Else
'    '    '                ReDim _planeFromSteinInfoIndex(steinInfoCount - 1)
'    '    '            End If

'    '    '            If stockCount = 0 Then
'    '    '                _planeFromStockIndex = Array.Empty(Of Airplane)()
'    '    '            Else
'    '    '                ReDim _planeFromStockIndex(stockCount - 1)
'    '    '            End If

'    '    '        End Sub
'    '    '        '
'    '    '        ''' <summary>
'    '    '        ''' Stellt sicher, daß das Array SteinInfoIndex->Plane den angegebenen Index aufnehmen kann.
'    '    '        ''' </summary>
'    '    '        Private Sub EnsureSteinInfoIndexCapacity(requiredIndex As Integer)

'    '    '            If requiredIndex < 0 Then
'    '    '                Throw New ArgumentOutOfRangeException(NameOf(requiredIndex))
'    '    '            End If

'    '    '            If _planeFromSteinInfoIndex Is Nothing Then
'    '    '                Throw New Exception("Programmierfehler: _planeFromSteinInfoIndex wurde nicht initialisiert.")
'    '    '            End If

'    '    '            If requiredIndex < _planeFromSteinInfoIndex.Length Then
'    '    '                Exit Sub
'    '    '            End If

'    '    '            Dim newLength As Integer = _planeFromSteinInfoIndex.Length

'    '    '            If newLength = 0 Then
'    '    '                newLength = _growStepSteinInfoIndexToPlane
'    '    '            End If

'    '    '            Do While requiredIndex >= newLength
'    '    '                newLength += _growStepSteinInfoIndexToPlane
'    '    '            Loop

'    '    '            ReDim Preserve _planeFromSteinInfoIndex(newLength - 1)

'    '    '        End Sub
'    '    '        '
'    '    '        ''' <summary>
'    '    '        ''' Stellt sicher, daß das Array StockIndex->Plane den angegebenen Index aufnehmen kann.
'    '    '        ''' </summary>
'    '    '        Private Sub EnsureStockIndexCapacity(requiredIndex As Integer)

'    '    '            If requiredIndex < 0 Then
'    '    '                Throw New ArgumentOutOfRangeException(NameOf(requiredIndex))
'    '    '            End If

'    '    '            If _planeFromStockIndex Is Nothing Then
'    '    '                Throw New Exception("Programmierfehler: _planeFromStockIndex wurde nicht initialisiert.")
'    '    '            End If

'    '    '            If requiredIndex < _planeFromStockIndex.Length Then
'    '    '                Exit Sub
'    '    '            End If

'    '    '            Dim newLength As Integer = _planeFromStockIndex.Length

'    '    '            If newLength = 0 Then
'    '    '                newLength = _growStepStockIndexToPlane
'    '    '            End If

'    '    '            Do While requiredIndex >= newLength
'    '    '                newLength += _growStepStockIndexToPlane
'    '    '            Loop

'    '    '            ReDim Preserve _planeFromStockIndex(newLength - 1)

'    '    '        End Sub
'    '    '        '
'    '    '        ''' <summary>
'    '    '        ''' Registriert ein AirPlane in der passenden direkten Zuordnungstabelle.
'    '    '        ''' Genau einer der beiden Indizes muß gültig sein.
'    '    '        ''' </summary>
'    '    '        Private Sub RegisterPlaneIndex(plane As Airplane)

'    '    '            If plane Is Nothing Then
'    '    '                Throw New ArgumentNullException(NameOf(plane))
'    '    '            End If

'    '    '            Dim steinInfoIndex As Integer = plane.SteinInfoIndex
'    '    '            Dim stockIndex As Integer = plane.StockIndex

'    '    '            If steinInfoIndex >= 0 Then

'    '    '                If stockIndex >= 0 Then
'    '    '                    Throw New Exception("Programmierfehler: Ein AirPlane darf nicht gleichzeitig SteinInfoIndex und StockIndex besitzen.")
'    '    '                End If

'    '    '                EnsureSteinInfoIndexCapacity(steinInfoIndex)

'    '    '                If _planeFromSteinInfoIndex(steinInfoIndex) IsNot Nothing Then
'    '    '                    Throw New Exception("Programmierfehler: SteinInfoIndex ist bereits einem anderen AirPlane zugeordnet.")
'    '    '                End If

'    '    '                _planeFromSteinInfoIndex(steinInfoIndex) = plane
'    '    '                Exit Sub
'    '    '            End If

'    '    '            If stockIndex >= 0 Then

'    '    '                EnsureStockIndexCapacity(stockIndex)

'    '    '                If _planeFromStockIndex(stockIndex) IsNot Nothing Then
'    '    '                    Throw New Exception("Programmierfehler: StockIndex ist bereits einem anderen AirPlane zugeordnet.")
'    '    '                End If

'    '    '                _planeFromStockIndex(stockIndex) = plane
'    '    '                Exit Sub
'    '    '            End If

'    '    '            Throw New Exception("Programmierfehler: Ein AirPlane muß entweder einen gültigen SteinInfoIndex oder einen gültigen StockIndex besitzen.")

'    '    '        End Sub
'    '    '        '
'    '    '        ''' <summary>
'    '    '        ''' Entfernt ein AirPlane aus der passenden direkten Zuordnungstabelle.
'    '    '        ''' </summary>
'    '    '        Private Sub UnregisterPlaneIndex(plane As Airplane)

'    '    '            If plane Is Nothing Then
'    '    '                Exit Sub
'    '    '            End If

'    '    '            Dim steinInfoIndex As Integer = plane.SteinInfoIndex
'    '    '            Dim stockIndex As Integer = plane.StockIndex

'    '    '            If steinInfoIndex >= 0 Then

'    '    '                If _planeFromSteinInfoIndex IsNot Nothing AndAlso
'    '    '                   steinInfoIndex < _planeFromSteinInfoIndex.Length AndAlso
'    '    '                   Object.ReferenceEquals(_planeFromSteinInfoIndex(steinInfoIndex), plane) Then

'    '    '                    _planeFromSteinInfoIndex(steinInfoIndex) = Nothing
'    '    '                End If

'    '    '                Exit Sub
'    '    '            End If

'    '    '            If stockIndex >= 0 Then

'    '    '                If _planeFromStockIndex IsNot Nothing AndAlso
'    '    '                   stockIndex < _planeFromStockIndex.Length AndAlso
'    '    '                   Object.ReferenceEquals(_planeFromStockIndex(stockIndex), plane) Then

'    '    '                    _planeFromStockIndex(stockIndex) = Nothing
'    '    '                End If

'    '    '            End If

'    '    '        End Sub
'    '    '        '
'    '    '        ''' <summary>
'    '    '        ''' Liefert das zugeordnete AirPlane zu einem SteinInfoIndex.
'    '    '        ''' Bei ungültigem Index oder fehlender Zuordnung wird Nothing zurückgegeben.
'    '    '        ''' Verwendung:
'    '    '        ''' Dim plane As AirPlane = GetPlaneFromSteinInfoIndex(steinInfoIndex)
'    '    '        ''' If plane Is Nothing Then
'    '    '        '''     Stein normal rendern
'    '    '        ''' Else
'    '    '        '''     Plane-Position / Ghost / Ziel etc. rendern
'    '    '        ''' End If
'    '    '        ''' </summary>
'    '    '        Public Function GetPlaneFromSteinInfoIndex(steinInfoIndex As Integer) As Airplane

'    '    '            If steinInfoIndex < 0 Then
'    '    '                Return Nothing
'    '    '            End If

'    '    '            If _planeFromSteinInfoIndex Is Nothing Then
'    '    '                Return Nothing
'    '    '            End If

'    '    '            If steinInfoIndex >= _planeFromSteinInfoIndex.Length Then
'    '    '                Return Nothing
'    '    '            End If

'    '    '            Dim plane As Airplane = _planeFromSteinInfoIndex(steinInfoIndex)

'    '    '            If plane IsNot Nothing Then
'    '    '                plane.IncRenderCallsCounter()
'    '    '            End If

'    '    '            Return plane

'    '    '        End Function
'    '    '        '
'    '    '        ''' <summary>
'    '    '        ''' Liefert das zugeordnete AirPlane zu einem StockIndex.
'    '    '        ''' Bei ungültigem Index oder fehlender Zuordnung wird Nothing zurückgegeben.
'    '    '        ''' Verwendung:
'    '    '        ''' Dim plane As AirPlane = GetPlaneFromStockIndex(stockIndex)
'    '    '        ''' If plane Is Nothing Then
'    '    '        '''     Stein normal rendern
'    '    '        ''' Else
'    '    '        '''     Plane-Position / Ghost / Ziel etc. rendern
'    '    '        ''' End If
'    '    '        ''' </summary>
'    '    '        Public Function GetPlaneFromStockIndex(stockIndex As Integer) As Airplane

'    '    '            If stockIndex < 0 Then
'    '    '                Return Nothing
'    '    '            End If

'    '    '            If _planeFromStockIndex Is Nothing Then
'    '    '                Return Nothing
'    '    '            End If

'    '    '            If stockIndex >= _planeFromStockIndex.Length Then
'    '    '                Return Nothing
'    '    '            End If

'    '    '            Dim plane As Airplane = _planeFromStockIndex(stockIndex)

'    '    '            If plane IsNot Nothing Then
'    '    '                plane.IncRenderCallsCounter()
'    '    '            End If

'    '    '            Return plane

'    '    '        End Function

'    '    '#End Region

'    '    '#Region "AktPlane-Verwaltung"

'    '    '        Public Property RenderBuffer As AirRenderBuffer

'    '    '        Private _planes As List(Of Airplane) = Nothing

'    '    '        Private _aktPlane As Airplane = Nothing
'    '    '        '
'    '    '        '''
'    '    '        Public ReadOnly Property AktPlane As Airplane
'    '    '            Get
'    '    '                Return _aktPlane
'    '    '            End Get
'    '    '        End Property

'    '    '        Private _aktPlaneIndex As Integer = -1
'    '    '        '
'    '    '        '
'    '    '        ''' <summary>
'    '    '        ''' Wird intern oder extern von ConsumeAirplanePolling aufgerufen.
'    '    '        ''' </summary>
'    '    '        Public Sub ResetNextPlane()
'    '    '            _aktPlaneIndex = -1
'    '    '        End Sub
'    '    '        '
'    '    '        ''' <summary>
'    '    '        ''' Wird in einer DoLoop aufgerufen, bis False zurückgegeben wird.
'    '    '        ''' Solange True, kann über AktPlane zugegriften werden.
'    '    '        ''' Von ConsumeAirplanePolling (aus dem Renderer heraus) wird immer zuerst zurückgesetzt,
'    '    '        ''' sodaß alle Planes durchlaufen werden.
'    '    '        ''' </summary>
'    '    '        ''' <returns></returns>
'    '    '        Public Function SetNextPlane() As Boolean

'    '    '            If _planes Is Nothing OrElse _planes.Count = 0 Then
'    '    '                _aktPlane = Nothing
'    '    '                Return False
'    '    '            End If

'    '    '            _aktPlaneIndex += 1

'    '    '            If _aktPlaneIndex >= _planes.Count Then
'    '    '                _aktPlane = Nothing
'    '    '                Return False
'    '    '            End If

'    '    '            _aktPlane = _planes(_aktPlaneIndex)
'    '    '            Return True

'    '    '        End Function
'    '    '        Public Sub AddPlane(plane As Airplane)
'    '    '            AddPlane(plane, insertAt:=AirplaneInsertAt.Last)
'    '    '        End Sub

'    '    '        '
'    '    '        ''' <summary>
'    '    '        ''' Fügt ein Airplane an einer definierten Position in die Z-Order-Liste ein.
'    '    '        ''' </summary>
'    '    '        Public Sub AddPlane(plane As Airplane, insertAt As AirplaneInsertAt)

'    '    '            If plane Is Nothing Then
'    '    '                Throw New ArgumentNullException(NameOf(plane))
'    '    '            End If

'    '    '            If _planes Is Nothing Then
'    '    '                _planes = New List(Of Airplane)
'    '    '            End If

'    '    '            Dim insertIndex As Integer = GetInsertIndex(insertAt)

'    '    '            _planes.Insert(insertIndex, plane)

'    '    '            RegisterPlaneIndex(plane)

'    '    '            MakeDirty()

'    '    '        End Sub
'    '    '        '
'    '    '        ''' <summary>
'    '    '        ''' Entfernt den AktPlane, nachdem der Auftrag abgearbeitet ist.
'    '    '        ''' </summary>
'    '    '        Private Sub RemoveAktPlane()

'    '    '            If _planes Is Nothing Then Exit Sub

'    '    '            If _aktPlaneIndex < 0 OrElse _aktPlaneIndex >= _planes.Count Then
'    '    '                _aktPlane = Nothing
'    '    '                Exit Sub
'    '    '            End If

'    '    '            Dim plane As Airplane = _planes(_aktPlaneIndex)

'    '    '            UnregisterPlaneIndex(plane)
'    '    '            _planes.RemoveAt(_aktPlaneIndex)
'    '    '            _aktPlane = Nothing

'    '    '            'Wichtig:
'    '    '            'Da SetNextPlane beim nächsten Aufruf zuerst +1 rechnet,
'    '    '            'muss hier um 1 zurückgesetzt werden, damit kein Plane
'    '    '            'übersprungen wird.
'    '    '            _aktPlaneIndex -= 1

'    '    '            If _planes.Count = 0 Then
'    '    '                _planes = Nothing
'    '    '                _aktPlaneIndex = -1
'    '    '            End If

'    '    '        End Sub

'    '    '        Private Sub MakeDirty()
'    '    '            _isDirty = True
'    '    '        End Sub

'    '    '        Private _isDirty As Boolean

'    '    '#End Region

'    '    '#Region "Helfer"

'    '    '        Private ReadOnly _rnd As New Random()
'    '    '        '
'    '    '        ''' <summary>
'    '    '        ''' Ermittelt den gültigen Einfügeindex abhängig von der gewünschten Position
'    '    '        ''' und der aktuellen Listenlänge.
'    '    '        ''' </summary>
'    '    '        Private Function GetInsertIndex(insertAt As AirplaneInsertAt) As Integer

'    '    '            Dim count As Integer = _planes.Count

'    '    '            Select Case insertAt

'    '    '                Case AirplaneInsertAt.First
'    '    '                    Return 0

'    '    '                Case AirplaneInsertAt.Second
'    '    '                    If count = 0 Then
'    '    '                        Return 0
'    '    '                    Else
'    '    '                        Return 1
'    '    '                    End If

'    '    '                Case AirplaneInsertAt.BeforeLast
'    '    '                    If count <= 1 Then
'    '    '                        Return 0
'    '    '                    Else
'    '    '                        Return count - 1
'    '    '                    End If

'    '    '                Case AirplaneInsertAt.Last
'    '    '                    Return count

'    '    '                Case AirplaneInsertAt.Random
'    '    '                    If count = 0 Then
'    '    '                        Return 0
'    '    '                    Else
'    '    '                        Return _rnd.Next(0, count + 1)
'    '    '                    End If

'    '    '                Case Else
'    '    '                    Throw New ArgumentOutOfRangeException(NameOf(insertAt))

'    '    '            End Select

'    '    '        End Function

'    '    '#End Region

'    '    '#Region "Dispose"

'    '    '        Private _disposed As Boolean

'    '    '        Public Sub Dispose() Implements IDisposable.Dispose

'    '    '            If Not _disposed Then
'    '    '                If _planes IsNot Nothing Then
'    '    '                    For Each plane As Airplane In _planes
'    '    '                        plane.Dispose()
'    '    '                    Next
'    '    '                End If

'    '    '                GC.SuppressFinalize(Me)
'    '    '                _disposed = True
'    '    '            End If
'    '    '        End Sub

'    '    '#End Region

'    '    '    End Class

'End Namespace
