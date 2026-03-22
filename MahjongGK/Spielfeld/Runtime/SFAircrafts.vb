Namespace Spielfeld

    'Hinweis: Die Begriffe "AirPlane" und "Plane" verwende ich Synonym,
    'wie z.B. Dim plane As AirPlane.
    '
    ''' <summary>
    ''' Die Entnahmereihenfolge vom Stapel
    ''' Wenn x-Renderintervalle nichts passiert, wird doch entfernt.
    ''' Die Order gilt, bis der Stapel leer ist oder geändert wird.
    ''' </summary>
    Public Enum AirplaneRemovingOrder
        AllTogether
        OneEachRenderTick
        OneEachDelay
        '
        ''' <summary>
        ''' 
        ''' </summary>
        OnlyRemovablePairsUseDelay
        OnlyOneLongSideFreeUseDelay
    End Enum

    Public Enum AirplaneZOrder
        InsertLast
        InsertFirst
        InsertAnywhere
    End Enum

    Public Enum AirplaneModus
        ''' <summary>
        ''' Im Editor: 
        ''' Quelle Vorrat: MousePairedLeftUp
        ''' Quelle Spiel: MousePairedCenter
        ''' Im Spiel: FlyAnimated
        ''' </summary>
        Auto
        '
        ''' <summary>
        ''' Der Stein hängt an der Maus.
        ''' Wenn die Quelle der Steinvorrat ist: MousePairedLeftUp
        ''' Wenn die Quelle das Spiel ist: MousePairedCenter
        ''' </summary>
        MousePairedAuto
        '
        ''' <summary>
        ''' Der Stein hängt an der Maus.
        ''' Gekoppelt am Klickpunkt
        ''' </summary>
        MousePaired
        '
        ''' <summary>
        ''' Der Stein hängt an der Maus.
        ''' Gekoppelt mittig auf dem Stein
        ''' </summary>
        MousePairedCenter
        '
        ''' <summary>
        ''' Der Stein hängt an der Maus.
        ''' Gekoppelt mittig auf dem linkem oberen Quadranten
        ''' </summary>
        MousePairedLeftUp
        '
        ''' <summary>
        ''' Standardflug
        ''' </summary>
        FlyNotAnimated
        '
        ''' <summary>
        ''' Animierter Flug
        ''' </summary>
        FlyAnimated
    End Enum
    '
    ''' <summary>
    ''' Wohin der Stein fliegt
    ''' </summary>
    Public Enum AirportDst
        '
        ''' <summary>
        ''' Das ist der Standard
        ''' Im Editor fliegen die Steine zurück in den Vorrat.
        ''' Im Spiel fliegen sie in die Historybox oder lösen sich auf, wenn keine Historybox existiert.
        ''' Bei Undo geht es umgekehrt.
        ''' </summary>
        Auto
        '
        StockInsert
        StockFlyBack
        SpielAddTo
        HistoryInsertLeft
        HistoryInsertRight
        Melt

    End Enum
    '
    Public Enum AirportSrc
        Auto
        SpielWithHistory
        SpielWithoutHistory
        UndoSpielWithHistory
        UndoSpielWithoutHistory
        '
        ''' <summary>
        ''' Kann nicht automatisch erkannt werden.
        ''' </summary>
        ReDeSpielWithHistory
        '
        ''' <summary>
        ''' Kann nicht automatisch erkannt werden.
        ''' </summary>
        ReDoSpielWithoutHistory
        '
        StockToEdit
        StockToStock
        EditToEdit
        EditToStock
        FlyBack
    End Enum
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
    Public Class SFAircrafts

#Region "Konstruktor"

        Public Sub New()
            Clear()
        End Sub

        Public Sub New(owner As SFDaten)
            _sfd = owner
            Clear()
        End Sub

        Private ReadOnly _sfd As SFDaten

#End Region

        Private _modus As AirplaneModus
        Private _portSrcEnum As AirportSrc
        Private _portDstEnum As AirportDst
        Private _idxStockOrHistorySrc As Integer
        Private _idxStockOrHistoryDst As Integer
        Private _f1bSrc As Triple
        Private _f2bSrc As Triple
        Private _f1bDst As Triple
        Private _f2bDst As Triple
        Private _zorder As AirplaneZOrder
        Private _animation As AirplaneAnimation
        Private _aktMousePos As Point

        Public Sub Rendertakt()

        End Sub
        '
        ''' <summary>
        ''' Wird nichts angegeben gilt AirplaneModus.Auto
        ''' Im Editor: 
        ''' Quelle Vorrat: MousePairedLeftUp
        ''' Quelle Spiel: MousePairedCenter
        ''' Im Spiel: FlyAnimated
        ''' </summary>
        ''' <param name="modus"></param>
        Public Sub SetModus(modus As AirplaneModus)

        End Sub
        '
        ''' <summary>
        ''' Wird nichts angegeben, holt sich die Klasse die aktuelle Position selber.
        ''' </summary>
        ''' <param name="aktMousePos"></param>
        Public Sub SetAktMousePos(aktMousePos As Point)

        End Sub
        '
        ''' <summary>
        ''' Derzeit gib es hier nur den Mode Auto.
        ''' Wird nichts angegeben, gilt der Stein unter dem Cursor.
        ''' Gibt es hier keinen, wird die Bearbeitung abgebrochen
        ''' </summary>
        ''' <param name="planeSrc"></param>
        Public Sub SetAirportSrc(planeSrc As AirportSrc)

        End Sub

        Public Sub SetAirportSrc(idxStockOrHistorySrc As Integer)

        End Sub
        Public Sub SetAirportSrc(fbSrc As Triple)

        End Sub
        '
        ''' <summary>
        ''' Hier werden gleich zwei Steine aufgenommen
        ''' </summary>
        ''' <param name="fb1Src"></param>
        ''' <param name="fb2Src"></param>
        Public Sub SetAirportSrc(fb1Src As Triple, fb2Src As Triple)

        End Sub

        ''' <summary>
        ''' Wenn nichts angegeben wird, gilt .auto:
        ''' Im Editor fliegen die Steine zurück in den Vorrat.
        ''' Im Spiel fliegen sie in die Historybox oder lösen sich auf, wenn keine Historybox existiert.
        ''' Bei Undo geht es umgekehrt.
        ''' </summary>
        Public Sub SetAirportDst(planeDst As AirportDst)

        End Sub

        Public Sub SetAirportDst(idxStockOrHistoryDst As Integer)

        End Sub
        Public Sub SetAirportDst(fbSrc As Triple)

        End Sub
        Public Sub SetAirportDst(fb1Src As Triple, fb2Src As Triple)

        End Sub

        Public Sub setZOrder(zorder As AirplaneZOrder)

        End Sub

        Public Sub SetAnimation(animation As AirplaneAnimation)

        End Sub
        '
        ''' <summary>
        ''' Zuerst die benötigten Setxx- Methoden aufrufen. Sie stehen alle
        ''' auf Default und werden nach dem Adden auf Default zurückgesetzt.
        ''' Gibt die Funktion False zurück, werden keine Flugdaten angelegt.
        ''' (typischerweise, wenn unter der MousePosition kein Stein ist.)
        ''' Das False ist also nur zur Kentnissnahme.
        ''' </summary>
        Public Function AddAirplane() As Boolean

            Dim plane As New Aircraft(_sfd)
            With plane
                .planeModus = _modus
                .portSrcEnum = _portSrcEnum
                .portDstEnum = _portDstEnum
                .idxStockOrHistorySrc = _idxStockOrHistorySrc
                .idxStockOrHistoryDst = _idxStockOrHistoryDst
                .f1bSrc = _f1bSrc
                .f2bSrc = _f2bSrc
                .f1bDst = _f1bDst
                .f2bDst = _f2bDst
                .zorder = _zorder
                .animation = _animation
                .aktMousePos = _aktMousePos

            End With
            If plane.Initialisierung Then
                _planes.Add(plane)
                Return True
            Else
                plane.Dispose()
                plane = Nothing
                Return False
            End If

            Clear()

        End Function

        Private Sub Clear()
            _modus = AirplaneModus.Auto
            _portSrcEnum = AirportSrc.Auto
            _portDstEnum = AirportDst.Auto
            _idxStockOrHistorySrc = -1
            _idxStockOrHistoryDst = -1
            _f1bSrc = New Triple
            _f2bSrc = New Triple
            _f1bDst = New Triple
            _f2bDst = New Triple
            _zorder = AirplaneZOrder.InsertLast
            _animation = AirplaneAnimation.None
            _aktMousePos = New Point
        End Sub

#Region "Class Airplane (Steinflug- Daten)"

        Private _planes As List(Of Aircraft)

#End Region

        Public Sub Dispose()

            If _planes IsNot Nothing Then
                For Each plane As Aircraft In _planes
                    plane.Dispose()
                Next
            End If
        End Sub

    End Class

End Namespace
