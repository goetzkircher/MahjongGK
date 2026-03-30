
'Ich habe alles überarbeitet und in eine Klasse gepackt.
'Prüfe bitte auf Schlüssigkeit und Vollständigkeit.
'und schreibe Sub AuswertungKlickArea

Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

Imports MahjongGK.Spielfeld

#Disable Warning IDE0079
#Disable Warning IDE1006

''' <summary>
''' Pfad: MahjongGK\Spielfeld\Runtime\Steinflug
''' </summary>
Public Class AirJobSelector

#Region "Konstruktor"

    Sub New()

    End Sub
    '
    ''' <summary>
    ''' dragDropExpected = True, wenn hier eine DragDrop-Aktion erwartet wird,
    ''' ein Stein also an der Maus hängen wird.
    ''' dragDropExpected = True ist die Voraussetzung dazu, daß DragDropKandidatX
    ''' überhaupt True werden kann. Zweite Voraussetzung ist, daß der Klick in ein Feld
    ''' mit Steinen erfolgte, also wenn DragDropKandidat True ist. 
    ''' </summary>
    ''' <param name="sfd"></param>
    ''' <param name="steinInfoIndex"></param>
    Sub New(sfd As SFDaten, steinInfoIndex As Integer)
        _sfd = sfd
        _steinInfoIndex = steinInfoIndex
        _steinIndex = SteinIndexEnum.ErrorSy
        Initialisierung()
    End Sub
    Sub New(sfd As SFDaten, steinIndex As SteinIndexEnum)
        _sfd = sfd
        _steinIndex = steinIndex
        _steinInfoIndex = -1
        Initialisierung()
    End Sub

    Private _sfd As SFDaten

#End Region

#Region "öffentlicher Zugriff"

    Private _steinInfoIndex As Integer
    Public ReadOnly Property SteinInfoIndex As Integer
        Get
            Return _steinInfoIndex
        End Get
    End Property

    Private _steinIndex As SteinIndexEnum
    ''' <summary>
    ''' Wenn HasSteinInfoIndex False ist, wird SteinIndexEnum.ErrorSy zurückgegeben,
    ''' (SteinIndexEnum ist der Index auf die Bitmap des Steines) 
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property SteinIndex As SteinIndexEnum
        Get
            Return _steinIndex
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

    Private _isValideSteinInfoIndex As Boolean
    Public ReadOnly Property IsValideSteinInfoIndex As Boolean
        Get
            Return _isValideSteinInfoIndex
        End Get
    End Property

    Private _mousePos As Point
    Public ReadOnly Property MousePos As Point
        Get
            Return _mousePos
        End Get
    End Property

    Private _mousePressed As Boolean
    Public ReadOnly Property MousePressed As Boolean
        Get
            Return _mousePressed
        End Get
    End Property

    Private _isSpiel As Boolean
    Public ReadOnly Property IsSpiel As Boolean
        Get
            Return _isSpiel
        End Get
    End Property
    Public ReadOnly Property IsEditor As Boolean
        Get
            Return Not _isSpiel
        End Get
    End Property

    Private _hasAction As Boolean
    Public ReadOnly Property HasAction As Boolean
        Get
            Return _hasAction
        End Get
    End Property

    Private _klickArea As AirKlickArea
    Public ReadOnly Property KlickArea As AirKlickArea
        Get
            Return _klickArea
        End Get
    End Property

    Private _flightRoute As AirFlightRoute
    Public ReadOnly Property FlightRoute As AirFlightRoute
        Get
            Return _flightRoute
        End Get
    End Property

    Private _planeModus As AirplaneModus
    Public ReadOnly Property PlaneModus As AirplaneModus
        Get
            Return _planeModus
        End Get
    End Property

    Private _dstDecisionAtMouseUp As Boolean
    Public ReadOnly Property DstDecisionAtMouseUp As Boolean
        Get
            Return _dstDecisionAtMouseUp
        End Get
    End Property

    Private _removingCandidat As Boolean
    '
    ''' <summary>
    ''' Das ist ein Stein der mit allen vier Quadranten sichtbar ist.
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property RemovingCandidat As Boolean
        Get
            Return _removingCandidat
        End Get
    End Property

    '
    ''' <summary>
    ''' Wird aufgerufen, wenn die Animationsstrecke zu kurz ist.
    ''' Wirkt nur, wenn PlaneModus = AirplaneModus.FlyAnimated,
    ''' andernfall ist der Aufruf unschädlich.
    ''' </summary>
    Public Sub SetFlyAnimatedToNotAnimated()
        If _planeModus = AirplaneModus.FlyAnimated Then
            _planeModus = AirplaneModus.FlyNotAnimated
        End If
    End Sub

#End Region

#Region "Initialisierung und Auswertung"

    ''' <summary>
    ''' Sammelt aus sfd As SFDaten und steinInfoIndex As Integer alle notwendigen
    ''' Daten zusammen und füllt alle ReadOnly-Properties.
    ''' </summary>
    Private Sub Initialisierung()

        'Intern bitte nicht auf die Properties zugreifen, sondern
        'auf die privaten _-Varianten (Wegen der Zusatzinformation _Unterstrich)
        _mousePos = _sfd.SFRun.MousePolling.MousePos
        _mousePressed = _sfd.SFRun.MousePolling.LeftMouseDown
        _isValideSteinInfoIndex = _steinInfoIndex >= 0
        _isSpiel = _sfd.SFLay.rxStock Is Nothing

        _klickArea = AirGetKlickArea(_mousePos, _sfd.SFLay)

        AuswertungKlickArea()

    End Sub

    Private Sub AuswertungKlickArea()

        ' Gegeben sind:
        ' _mousePressed
        ' _isValideSteinInfoIndex 
        ' _isSpiel
        ' _klickArea
        '
        ' Gesucht und zuzuweisen sind:
        ' Die Abhängigkeiten stehen in den Summarys der Enumerationen weiter oben.
        ' _airplaneModus
        ' _dstDecisionAtMouseUp
        ' _removingCandidat (als vorläufige Einstufung)

        _hasAction = False
        _flightRoute = AirFlightRoute.NoFlightRoute
        _planeModus = AirplaneModus.NotSet
        _dstDecisionAtMouseUp = False
        _removingCandidat = False

        Select Case _klickArea

            Case AirKlickArea.Undo

                If _mousePressed Then
                    _hasAction = True
                    _flightRoute = AirFlightRoute.Undo
                    _planeModus = AirplaneModus.FlyAnimated
                End If

            Case AirKlickArea.HistoryBoxSmall,
                 AirKlickArea.HistoryBoxLeft,
                 AirKlickArea.HistoryBoxRight

                If _mousePressed AndAlso _isValideSteinInfoIndex Then
                    _hasAction = True
                    _flightRoute = AirFlightRoute.Undo
                    _planeModus = AirplaneModus.FlyAnimated
                End If

            Case AirKlickArea.Redo

                If _mousePressed Then
                    _hasAction = True
                    _flightRoute = AirFlightRoute.Redo
                    _planeModus = AirplaneModus.FlyAnimated
                End If

            Case AirKlickArea.Spielfeld

                If _mousePressed AndAlso _isValideSteinInfoIndex Then

                    _hasAction = True
                    If _sfd.SFLay.rxHistoryBoxLeft IsNot Nothing OrElse
                               _sfd.SFLay.rxHistoryBoxRight IsNot Nothing OrElse
                                _sfd.SFLay.rxHistoryBoxSmall IsNot Nothing Then

                        _flightRoute = AirFlightRoute.SpielfeldToHistory
                        _planeModus = AirplaneModus.FlyAnimated
                    Else
                        _flightRoute = AirFlightRoute.SpielfeldToHistoryNotVisible
                        _planeModus = AirplaneModus.GoOutTheWindow
                    End If
                    _dstDecisionAtMouseUp = False
                    _removingCandidat = True

                End If
            Case AirKlickArea.Editor
                If _mousePressed AndAlso _isValideSteinInfoIndex Then
                    _hasAction = True
                    _flightRoute = AirFlightRoute.EditfeldToUnKnown
                    _planeModus = AirplaneModus.MousePaired
                    _dstDecisionAtMouseUp = True
                    _removingCandidat = _sfd.SFInf.IsRemovable(_steinInfoIndex)
                End If

            Case AirKlickArea.Stock

                If _mousePressed AndAlso Not _isSpiel AndAlso _isValideSteinInfoIndex Then
                    _hasAction = True
                    _flightRoute = AirFlightRoute.StockToUnKnown
                    _planeModus = AirplaneModus.MousePaired
                    _dstDecisionAtMouseUp = True
                End If

            Case AirKlickArea.StockScrollbar,
                 AirKlickArea.HistoryBoxLeftScrollbar,
                 AirKlickArea.HistoryBoxRightScrollbar

                _hasAction = False
                _flightRoute = AirFlightRoute.NoEvaluation
                _planeModus = AirplaneModus.NotSet
                _dstDecisionAtMouseUp = False
                _removingCandidat = False

            Case AirKlickArea.None,
                 AirKlickArea.Content,
                 AirKlickArea.Header,
                 AirKlickArea.Outside

                _hasAction = False
                _flightRoute = AirFlightRoute.NoFlightRoute
                _planeModus = AirplaneModus.NotSet
                _dstDecisionAtMouseUp = False
                _removingCandidat = False

            Case Else

                _hasAction = False
                _flightRoute = AirFlightRoute.NoFlightRoute
                _planeModus = AirplaneModus.NotSet
                _dstDecisionAtMouseUp = False
                _removingCandidat = False

        End Select

    End Sub

#End Region

#Region "Helfer"

#End Region

End Class
