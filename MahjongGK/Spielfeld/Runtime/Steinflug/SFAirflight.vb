'Kannst du meiner Erinnerung auffrishen und mir einen Einstieg geben, wie die Klasse funktioniert?
'Ich habe die vor zwei Monaten angefangen zu programmieren und, dann die Tilefactory geschrieben und finde mich jetzt nicht mehr zurecht.

Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

#Disable Warning IDE0079
#Disable Warning IDE1006
#Disable Warning IDE0031
#Disable Warning IDE0017

Imports MahjongGK.Contracts.GlobalEnum

Namespace Spielfeld

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
        Implements IDisposable

#Region "Konstruktor"

        Public Sub New()
        End Sub

        Public Sub New(owner As SFDaten)
            _sfd = owner
            InitialisierungIndexToPlane(steinInfoCount:=250, stockCount:=250, growStepSteinInfo:=125, growStepStock:=125)
            RenderBuffer = New AirRenderBuffer(bufferSize:=10, growStep:=250)
        End Sub

        Private ReadOnly _sfd As SFDaten

#End Region

#Region "ReadOnly Properties"

        Public ReadOnly Property AktSteinInfoIndex As Integer
            Get
                Return _aktPlane.SteinInfoIndex
            End Get
        End Property

        ''' <summary>
        ''' Wenn HasSteinInfoIndex False ist, wird SteinTyp.ErrorSy zurückgegeben,
        ''' (SteinTyp ist der Index auf die Bitmap des Steines) 
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property AktSteinIndex As SteinTyp
            Get
                Return _aktPlane.SteinTypIndex
            End Get
        End Property

        Public ReadOnly Property AktSteinSize As Size
            Get
                Return _aktPlane.BmpOrg.Size
            End Get
        End Property

        Public ReadOnly Property AktHasSteinInfoIndex As Boolean
            Get
                Return _aktPlane.SteinInfoIndex >= 0
            End Get
        End Property

        Public ReadOnly Property AktHasSteinIndex As Boolean
            Get
                Return _aktPlane.SteinInfoIndex < 0
            End Get
        End Property

        '
        Private _JobSelector As AirJobSelector = Nothing
        Public ReadOnly Property AktJobSelector As AirJobSelector
            Get
                Return _aktPlane.JobSelector
            End Get
        End Property
        '
        ''' <summary>
        ''' Wie bewegt sich der Stein. 
        ''' Mauskoppelung, animiert, nicht animierte, auflösend. 
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property AktPlaneModus As AirplaneModus
            Get
                Return _aktPlane.PlaneModus
            End Get
        End Property
        '
        ''' <summary>
        ''' Was für eine Aktion soll ausgeführt werden.
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property AktFlightRoute As AirFlightRoute
            Get
                Return _aktPlane.FlightRoute
            End Get
        End Property
        '
        ''' <summary>
        ''' Wo wurde geklickt
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property AktKlickArea As AirKlickArea
            Get
                Return _aktPlane.KlickArea
            End Get
        End Property
        '
        ''' <summary>
        ''' Das Renderrechteck des Steines am Quellort (ggf Ghost)
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property AktSrcRect As Rectangle
            Get
                Return _aktPlane.SrcRect
            End Get
        End Property
        '
        ''' <summary>
        ''' Das Render-Rechteck des Steines am Zielort (ggf Ghost)
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property AktDstRect As Rectangle
        '
        ''' <summary>
        ''' Das aktuelle Render-Rechteck des Steines
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property AktRenderRect As Rectangle

        ''' <summary>
        ''' Das ist eine DeepCopy.
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property AktBmpOrg As Bitmap
            Get
                Return _aktPlane.BmpOrg
            End Get
        End Property
        '
        Public ReadOnly Property AktBmpGhost As Bitmap
            Get
                Return _aktPlane.BmpGhost
            End Get
        End Property
        '
        Public ReadOnly Property AktBmpSelected As Bitmap
            Get
                Return _aktPlane.BmpSelected
            End Get
        End Property
        '
        Public ReadOnly Property AktBmpPlacable As Bitmap
            Get
                Return _aktPlane.BmpPlacable
            End Get
        End Property

#End Region

#Region "IndexToPlane-Verwaltung"
        '
        ''' <summary>
        ''' Direkte Zuordnung: SteinInfoIndex -> AirPlane.
        ''' Nicht zugeordnete Einträge sind Nothing.
        ''' </summary>
        Private _planeFromSteinInfoIndex() As Airplane = Nothing
        '
        ''' <summary>
        ''' Direkte Zuordnung: StockIndex -> AirPlane.
        ''' Nicht zugeordnete Einträge sind Nothing.
        ''' </summary>
        Private _planeFromStockIndex() As Airplane = Nothing
        '
        ''' <summary>
        ''' Vergrößerungsschritt für SteinInfoIndex -> AirPlane.
        ''' </summary>
        Private _growStepSteinInfoIndexToPlane As Integer = 64
        '
        ''' <summary>
        ''' Vergrößerungsschritt für StockIndex -> AirPlane.
        ''' </summary>
        Private _growStepStockIndexToPlane As Integer = 32
        '
        ''' <summary>
        ''' Initialisiert die direkten Index->Plane-Zuordnungen.
        ''' Muss genau einmal beim Erzeugen von SFAirFlight aufgerufen werden.
        ''' steinInfoCount ist die maximal bekannte oder zunächst erwartete Anzahl.
        ''' Falls später größere Indizes benötigt werden, werden die Arrays automatisch erweitert.
        ''' stockCount ist die anfänglich erwartete Größe des Stock-Bereichs.
        ''' </summary>
        Private Sub InitialisierungIndexToPlane(steinInfoCount As Integer,
                                                stockCount As Integer,
                                                growStepSteinInfo As Integer,
                                                growStepStock As Integer)

            If steinInfoCount < 0 Then
                Throw New ArgumentOutOfRangeException(NameOf(steinInfoCount))
            End If

            If stockCount < 0 Then
                Throw New ArgumentOutOfRangeException(NameOf(stockCount))
            End If

            If growStepSteinInfo <= 0 Then
                Throw New ArgumentOutOfRangeException(NameOf(growStepSteinInfo))
            End If

            If growStepStock <= 0 Then
                Throw New ArgumentOutOfRangeException(NameOf(growStepStock))
            End If

            _growStepSteinInfoIndexToPlane = growStepSteinInfo
            _growStepStockIndexToPlane = growStepStock

            If steinInfoCount = 0 Then
                _planeFromSteinInfoIndex = Array.Empty(Of Airplane)()
            Else
                ReDim _planeFromSteinInfoIndex(steinInfoCount - 1)
            End If

            If stockCount = 0 Then
                _planeFromStockIndex = Array.Empty(Of Airplane)()
            Else
                ReDim _planeFromStockIndex(stockCount - 1)
            End If

        End Sub
        '
        ''' <summary>
        ''' Stellt sicher, daß das Array SteinInfoIndex->Plane den angegebenen Index aufnehmen kann.
        ''' </summary>
        Private Sub EnsureSteinInfoIndexCapacity(requiredIndex As Integer)

            If requiredIndex < 0 Then
                Throw New ArgumentOutOfRangeException(NameOf(requiredIndex))
            End If

            If _planeFromSteinInfoIndex Is Nothing Then
                Throw New Exception("Programmierfehler: _planeFromSteinInfoIndex wurde nicht initialisiert.")
            End If

            If requiredIndex < _planeFromSteinInfoIndex.Length Then
                Exit Sub
            End If

            Dim newLength As Integer = _planeFromSteinInfoIndex.Length

            If newLength = 0 Then
                newLength = _growStepSteinInfoIndexToPlane
            End If

            Do While requiredIndex >= newLength
                newLength += _growStepSteinInfoIndexToPlane
            Loop

            ReDim Preserve _planeFromSteinInfoIndex(newLength - 1)

        End Sub
        '
        ''' <summary>
        ''' Stellt sicher, daß das Array StockIndex->Plane den angegebenen Index aufnehmen kann.
        ''' </summary>
        Private Sub EnsureStockIndexCapacity(requiredIndex As Integer)

            If requiredIndex < 0 Then
                Throw New ArgumentOutOfRangeException(NameOf(requiredIndex))
            End If

            If _planeFromStockIndex Is Nothing Then
                Throw New Exception("Programmierfehler: _planeFromStockIndex wurde nicht initialisiert.")
            End If

            If requiredIndex < _planeFromStockIndex.Length Then
                Exit Sub
            End If

            Dim newLength As Integer = _planeFromStockIndex.Length

            If newLength = 0 Then
                newLength = _growStepStockIndexToPlane
            End If

            Do While requiredIndex >= newLength
                newLength += _growStepStockIndexToPlane
            Loop

            ReDim Preserve _planeFromStockIndex(newLength - 1)

        End Sub
        '
        ''' <summary>
        ''' Registriert ein AirPlane in der passenden direkten Zuordnungstabelle.
        ''' Genau einer der beiden Indizes muß gültig sein.
        ''' </summary>
        Private Sub RegisterPlaneIndex(plane As Airplane)

            If plane Is Nothing Then
                Throw New ArgumentNullException(NameOf(plane))
            End If

            Dim steinInfoIndex As Integer = plane.SteinInfoIndex
            Dim stockIndex As Integer = plane.StockIndex

            If steinInfoIndex >= 0 Then

                If stockIndex >= 0 Then
                    Throw New Exception("Programmierfehler: Ein AirPlane darf nicht gleichzeitig SteinInfoIndex und StockIndex besitzen.")
                End If

                EnsureSteinInfoIndexCapacity(steinInfoIndex)

                If _planeFromSteinInfoIndex(steinInfoIndex) IsNot Nothing Then
                    Throw New Exception("Programmierfehler: SteinInfoIndex ist bereits einem anderen AirPlane zugeordnet.")
                End If

                _planeFromSteinInfoIndex(steinInfoIndex) = plane
                Exit Sub
            End If

            If stockIndex >= 0 Then

                EnsureStockIndexCapacity(stockIndex)

                If _planeFromStockIndex(stockIndex) IsNot Nothing Then
                    Throw New Exception("Programmierfehler: StockIndex ist bereits einem anderen AirPlane zugeordnet.")
                End If

                _planeFromStockIndex(stockIndex) = plane
                Exit Sub
            End If

            Throw New Exception("Programmierfehler: Ein AirPlane muß entweder einen gültigen SteinInfoIndex oder einen gültigen StockIndex besitzen.")

        End Sub
        '
        ''' <summary>
        ''' Entfernt ein AirPlane aus der passenden direkten Zuordnungstabelle.
        ''' </summary>
        Private Sub UnregisterPlaneIndex(plane As Airplane)

            If plane Is Nothing Then
                Exit Sub
            End If

            Dim steinInfoIndex As Integer = plane.SteinInfoIndex
            Dim stockIndex As Integer = plane.StockIndex

            If steinInfoIndex >= 0 Then

                If _planeFromSteinInfoIndex IsNot Nothing AndAlso
                   steinInfoIndex < _planeFromSteinInfoIndex.Length AndAlso
                   Object.ReferenceEquals(_planeFromSteinInfoIndex(steinInfoIndex), plane) Then

                    _planeFromSteinInfoIndex(steinInfoIndex) = Nothing
                End If

                Exit Sub
            End If

            If stockIndex >= 0 Then

                If _planeFromStockIndex IsNot Nothing AndAlso
                   stockIndex < _planeFromStockIndex.Length AndAlso
                   Object.ReferenceEquals(_planeFromStockIndex(stockIndex), plane) Then

                    _planeFromStockIndex(stockIndex) = Nothing
                End If

            End If

        End Sub
        '
        ''' <summary>
        ''' Liefert das zugeordnete AirPlane zu einem SteinInfoIndex.
        ''' Bei ungültigem Index oder fehlender Zuordnung wird Nothing zurückgegeben.
        ''' Verwendung:
        ''' Dim plane As AirPlane = GetPlaneFromSteinInfoIndex(steinInfoIndex)
        ''' If plane Is Nothing Then
        '''     Stein normal rendern
        ''' Else
        '''     Plane-Position / Ghost / Ziel etc. rendern
        ''' End If
        ''' </summary>
        Public Function GetPlaneFromSteinInfoIndex(steinInfoIndex As Integer) As Airplane

            If steinInfoIndex < 0 Then
                Return Nothing
            End If

            If _planeFromSteinInfoIndex Is Nothing Then
                Return Nothing
            End If

            If steinInfoIndex >= _planeFromSteinInfoIndex.Length Then
                Return Nothing
            End If

            Dim plane As Airplane = _planeFromSteinInfoIndex(steinInfoIndex)

            If plane IsNot Nothing Then
                plane.IncRenderCallsCounter()
            End If

            Return plane

        End Function
        '
        ''' <summary>
        ''' Liefert das zugeordnete AirPlane zu einem StockIndex.
        ''' Bei ungültigem Index oder fehlender Zuordnung wird Nothing zurückgegeben.
        ''' Verwendung:
        ''' Dim plane As AirPlane = GetPlaneFromStockIndex(stockIndex)
        ''' If plane Is Nothing Then
        '''     Stein normal rendern
        ''' Else
        '''     Plane-Position / Ghost / Ziel etc. rendern
        ''' End If
        ''' </summary>
        Public Function GetPlaneFromStockIndex(stockIndex As Integer) As Airplane

            If stockIndex < 0 Then
                Return Nothing
            End If

            If _planeFromStockIndex Is Nothing Then
                Return Nothing
            End If

            If stockIndex >= _planeFromStockIndex.Length Then
                Return Nothing
            End If

            Dim plane As Airplane = _planeFromStockIndex(stockIndex)

            If plane IsNot Nothing Then
                plane.IncRenderCallsCounter()
            End If

            Return plane

        End Function

#End Region

#Region "AktPlane-Verwaltung"

        Public Property RenderBuffer As AirRenderBuffer

        Private _planes As List(Of Airplane) = Nothing

        Private _aktPlane As Airplane = Nothing
        '
        '''
        Public ReadOnly Property AktPlane As Airplane
            Get
                Return _aktPlane
            End Get
        End Property

        Private _aktPlaneIndex As Integer = -1
        '
        '
        ''' <summary>
        ''' Wird intern oder extern von ConsumeAirplanePolling aufgerufen.
        ''' </summary>
        Public Sub ResetNextPlane()
            _aktPlaneIndex = -1
        End Sub
        '
        ''' <summary>
        ''' Wird in einer DoLoop aufgerufen, bis False zurückgegeben wird.
        ''' Solange True, kann über AktPlane zugegriften werden.
        ''' Von ConsumeAirplanePolling (aus dem Renderer heraus) wird immer zuerst zurückgesetzt,
        ''' sodaß alle Planes durchlaufen werden.
        ''' </summary>
        ''' <returns></returns>
        Public Function SetNextPlane() As Boolean

            If _planes Is Nothing OrElse _planes.Count = 0 Then
                _aktPlane = Nothing
                Return False
            End If

            _aktPlaneIndex += 1

            If _aktPlaneIndex >= _planes.Count Then
                _aktPlane = Nothing
                Return False
            End If

            _aktPlane = _planes(_aktPlaneIndex)
            Return True

        End Function
        '
        ''' <summary>
        ''' Fügt ein Airplane an einer definierten Position in die Z-Order-Liste ein.
        ''' </summary>
        Public Sub AddPlane(plane As Airplane, insertAt As AirplaneInsertAt)

            If plane Is Nothing Then
                Throw New ArgumentNullException(NameOf(plane))
            End If

            If _planes Is Nothing Then
                _planes = New List(Of Airplane)
            End If

            If plane.DragDropAktiv Then
                For Each p As Airplane In _planes
                    If p.DragDropAktiv Then
                        Throw New Exception("Adden von Airplane abgebrochen. DragDropAktiv einer weiteren Instanz ist noch True.")
                    End If
                Next
            End If

            Dim insertIndex As Integer = GetInsertIndex(insertAt)

            _planes.Insert(insertIndex, plane)

            RegisterPlaneIndex(plane)

            MakeDirty()

        End Sub
        '
        ''' <summary>
        ''' Entfernt den AktPlane, nachdem der Auftrag abgearbeitet ist.
        ''' </summary>
        Private Sub RemoveAktPlane()

            If _planes Is Nothing Then Exit Sub

            If _aktPlaneIndex < 0 OrElse _aktPlaneIndex >= _planes.Count Then
                _aktPlane = Nothing
                Exit Sub
            End If

            Dim plane As Airplane = _planes(_aktPlaneIndex)

            UnregisterPlaneIndex(plane)
            _planes.RemoveAt(_aktPlaneIndex)
            _aktPlane = Nothing

            'Wichtig:
            'Da SetNextPlane beim nächsten Aufruf zuerst +1 rechnet,
            'muss hier um 1 zurückgesetzt werden, damit kein Plane
            'übersprungen wird.
            _aktPlaneIndex -= 1

            If _planes.Count = 0 Then
                _planes = Nothing
                _aktPlaneIndex = -1
            End If

        End Sub

#End Region

#Region "Polling"

        Private _polling As Boolean
        Private _leftMousePressed As Boolean
        Private _leftMouseReleased As Boolean
        Private _dragDropMoved As Boolean
        '
        ''' <summary>
        ''' Schließt _dragDropMoved ein!
        ''' </summary>
        Private _dragDropAktive As Boolean
        Private _endDragDrop As Boolean
        Private _mousePos As Point
        Private _klickArea As AirKlickArea
        Private _klickAreaGroup As AirKlickAreaGroup
        Private _klickIsDragDropKandidat As Boolean

        Private _isDirty As Boolean

        Private Enum DragDropJob
            FirstStep
            Active
            EndJob
        End Enum

        Public Sub Polling_Dispose()
            _polling = False
            Dispose() ' Disposed die Planes.
        End Sub
        '

        Public Function ConsumeAirplanePolling() As Boolean

            'alle in der Region benötigten Werte lokal laden.
            _leftMousePressed = _sfd.SFRun.MousePolling.ConsumeLeftMousePressed()
            _leftMouseReleased = _sfd.SFRun.MousePolling.ConsumeLeftMouseReleased()
            _mousePos = _sfd.SFRun.MousePolling.MousePos
            _klickArea = AirGetKlickArea(_mousePos, _sfd.SFLay)
            _klickAreaGroup = AirGetKlickAreaGroup(_klickArea)
            _klickIsDragDropKandidat = AirIsDragDropKandidat(_klickArea)
            _dragDropMoved = _sfd.SFRun.MousePolling.DragDropMoved
            _dragDropAktive = _sfd.SFRun.MousePolling.DragDropActive
            _endDragDrop = _sfd.SFRun.MousePolling.ConsumeEndDragDrop

            If Not IsDragDropAktiv() AndAlso _leftMousePressed Then
                'DragDrop kann nur einmalig stattfinden.
                'Auch während eines DragDrop bringt ein
                'Buttonklick nichts.
                'Vermutlich tritt das nur ein, wenn sehr schnell
                'hintereinander geklickt wird.
                '==> einfach ignorieren.
                If _klickArea = AirKlickArea.Spielfeld Then
                    DoKlickSpiel()

                ElseIf _klickArea = AirKlickArea.Editor Then
                    DoKlickEditor()

                ElseIf _klickArea = AirKlickArea.Stock Then

                ElseIf _klickAreaGroup = AirKlickAreaGroup.Historybox Then
                    DoKlickHistorboxes()

                ElseIf _klickAreaGroup = AirKlickAreaGroup.Button Then
                    DoKlickButtons()
                End If
            End If
            '
            DoRenderstep()

            If _isDirty Then
                _isDirty = False
                Return True
            Else
                Return False
            End If

        End Function

        Private _stop As Boolean
        Private Sub DoRenderstep()

            ResetNextPlane()

            Do
                If Not SetNextPlane() Then
                    Exit Do
                End If

                If _aktPlane.DragDropAktiv Then
                    If _aktPlane.ConsumeDragDropFirstStep Then
                        Select Case _aktPlane.KlickArea
                            Case AirKlickArea.Editor
                                DoRenderstepEditor(DragDropJob.FirstStep)

                            Case AirKlickArea.Stock
                                DoRenderstepStock(DragDropJob.FirstStep)

                            Case AirKlickArea.Spielfeld
                                DoRenderstepSpiel(DragDropJob.FirstStep)

                        End Select

                    ElseIf _dragDropAktive Then
                        Select Case _aktPlane.KlickArea
                            Case AirKlickArea.Editor
                                DoRenderstepEditor(DragDropJob.Active)

                            Case AirKlickArea.Stock
                                DoRenderstepStock(DragDropJob.Active)

                            Case AirKlickArea.Spielfeld
                                DoRenderstepSpiel(DragDropJob.Active)

                        End Select

                    ElseIf _endDragDrop Then
                        Select Case _aktPlane.KlickArea
                            Case AirKlickArea.Editor
                                DoRenderstepEditor(DragDropJob.EndJob)

                            Case AirKlickArea.Stock
                                DoRenderstepStock(DragDropJob.EndJob)

                            Case AirKlickArea.Spielfeld
                                DoRenderstepSpiel(DragDropJob.EndJob)

                        End Select

                    End If
                Else 'Not AktPlane.DragDropAktiv 

                End If
            Loop
        End Sub

        Private Sub DoKlickSpiel()

        End Sub
        Private Sub DoRenderstepSpiel(job As DragDropJob)
            With _aktPlane
                Select Case job
                    Case DragDropJob.FirstStep
                        .AddRenderSteinInfoIndexZOrder(.SrcRect, .BmpGhost)

                    Case DragDropJob.Active
                        .AddRenderSteinInfoIndexZOrder(.SrcRect, .BmpGhost)

                    Case DragDropJob.EndJob
                        'Neue Instanz zum Ablegen oder zurückfliegen erstellen

                        'alte Instanz löschen
                        RemoveAktPlane()
                End Select
            End With
            MakeDirty()
        End Sub
        '----------------------------------------------------------------------
        Private Sub DoKlickEditor()

            Dim steinInfoIndex As Integer = _sfd.SFInf.GetTopSteinInfoIndexAtPoint(_mousePos)

            If steinInfoIndex < 0 Then
                'Klick auf eine freie Fläche
                Exit Sub
            End If
            '
            If _sfd.SFRun.MousePolling.StartDragDrop() Then
                'Die Deklaration erzeugt auch die benötigten Bitmaps, holt sich das Ausgaberechteck usw.
                Dim plane As Airplane = Airplane.CreateEditorDragDropAirplane(_sfd, steinInfoIndex)
                plane.MouseAnkerPosition = New MouseAnkerPosition(plane.SrcRect, _mousePos, _sfd.SFLay.rxStageUsed)
            End If

            '

        End Sub

        Private _aktValideMouseTripl As New Triple(valide:=ValidePlace.NotSet)

        Private Sub DoRenderstepEditor(job As DragDropJob)
            With _aktPlane
                Select Case job
                    Case DragDropJob.FirstStep
                        '()
                        _sfd.SFInf.TmpRemoveStein(.SteinInfoIndex)

                        .AddRenderSteinInfoIndexZOrder(.SrcRect, .BmpGhost)
                        'Andernfalls blitzt der Geist einen Rendertakt lang auf.
                        RenderBuffer.AddRenderBitmapTopZOrder(.MouseAnkerPosition.GetAktRectPlane(_mousePos), .BmpSelected)

                    Case DragDropJob.Active
                        ' Z-Order-Problem
                        '.AddRenderSteinInfoIndexZOrder(.SrcRect, .BmpGhost)

                        RenderBuffer.AddRenderBitmapTopZOrder(.SrcRect, .BmpGhost)
                        RenderBuffer.AddRenderBitmapTopZOrder(.MouseAnkerPosition.GetAktRectPlane(_mousePos), .BmpSelected)

                        Dim tpl As Triple = _sfd.SFInf.IsFundamentKandidat(_mousePos)
                        If tpl.IsValideYes Then
                            Dim rect As Rectangle = _sfd.SFInf.GetSteinRenderRect(tpl)
                            RenderBuffer.AddRenderBitmapTopZOrder(rect, .BmpGhost, insertAtPreviousPos:=True)
                            _aktValideMouseTripl = tpl
                        Else
                            _aktValideMouseTripl.Valide = ValidePlace.NotSet
                        End If

                    Case DragDropJob.EndJob
                        '()
                        _sfd.SFInf.TmpReturnRemovedStein()

                        If _aktValideMouseTripl.IsValideYes Then
                            If .HasSteinInfoIndex Then
                                'Umsetzen innerhalb des Editors
                                _sfd.SFInf.RemoveSteinFromSpielfeld(.SteinInfoIndex)
                                _sfd.SFInf.AddSteinToSpielfeld(.SteinTypIndex, _aktValideMouseTripl)
                                'alte Instanz löschen
                                RemoveAktPlane()
                            Else
                                'Stein aus dem Vorrat ablegen
                                Stop
                                RemoveAktPlane()
                            End If
                        Else
                            'Ungültige Position ==> zurückfliegen
                            Stop
                            RemoveAktPlane()
                            'Die Deklaration erzeugt auch die benötigten Bitmaps, holt sich das Ausgaberechteck usw.
                            Dim plane As Airplane = Airplane.CreateEditorFlyBackInsideEditorAirplane(_sfd, .SteinInfoIndex)

                            Try
                                '    _sfd.SFAir.AddPlane(plane, AirplaneInsertAt.First)

                            Catch ex As Exception
                                'es gibt bereits eine DragDrop-Instanz
                                If Debugger.IsAttached Then
                                    Stop 'Programmierfehler
                                End If
                                'Andernfalls ignorieren und abbrechen.
                                Exit Sub
                            End Try

                            '
                        End If

                End Select
            End With
            MakeDirty()
        End Sub

        '----------------------------------------------------------------------

        Private Sub DoRenderstepStock(job As DragDropJob)
            With _aktPlane
                Select Case job
                    Case DragDropJob.FirstStep
                        .AddRenderSteinInfoIndexZOrder(.SrcRect, .BmpGhost)

                    Case DragDropJob.Active
                        .AddRenderSteinInfoIndexZOrder(.SrcRect, .BmpGhost)

                    Case DragDropJob.EndJob
                        'Neue Instanz zum Ablegen oder zurückfliegen erstellen

                        'alte Instanz löschen
                        RemoveAktPlane()
                End Select
            End With
            MakeDirty()
        End Sub

        '----------------------------------------------------------------------
        '
        Private Sub DoKlickStock()

        End Sub
        Private Sub DoKlickHistorboxes()

        End Sub
        Private Sub DoRenderstepHistorboxes()

        End Sub

        '----------------------------------------------------------------------

        Private Sub DoKlickButtons()

        End Sub
        '

        Private Sub MakeDirty()
            _isDirty = True
        End Sub

#End Region

#Region "Helfer"

        Private ReadOnly _rnd As New Random()
        '

        Public Function IsDragDropAktiv() As Boolean
            If _planes Is Nothing Then
                Return False
            End If
            For Each p As Airplane In _planes
                If p.DragDropAktiv Then
                    Return True
                End If
            Next
            Return False
        End Function

        '
        ''' <summary>
        ''' Ermittelt den gültigen Einfügeindex abhängig von der gewünschten Position
        ''' und der aktuellen Listenlänge.
        ''' </summary>
        Private Function GetInsertIndex(insertAt As AirplaneInsertAt) As Integer

            Dim count As Integer = _planes.Count

            Select Case insertAt

                Case AirplaneInsertAt.First
                    Return 0

                Case AirplaneInsertAt.Second
                    If count = 0 Then
                        Return 0
                    Else
                        Return 1
                    End If

                Case AirplaneInsertAt.BeforeLast
                    If count <= 1 Then
                        Return 0
                    Else
                        Return count - 1
                    End If

                Case AirplaneInsertAt.Last
                    Return count

                Case AirplaneInsertAt.Random
                    If count = 0 Then
                        Return 0
                    Else
                        Return _rnd.Next(0, count + 1)
                    End If

                Case Else
                    Throw New ArgumentOutOfRangeException(NameOf(insertAt))

            End Select

        End Function

#End Region

#Region "Dispose"

        Private _disposed As Boolean

        Public Sub Dispose() Implements IDisposable.Dispose

            If Not _disposed Then
                If _planes IsNot Nothing Then
                    For Each plane As Airplane In _planes
                        plane.Dispose()
                    Next
                End If

                GC.SuppressFinalize(Me)
                _disposed = True
            End If
        End Sub

#End Region

    End Class

End Namespace
