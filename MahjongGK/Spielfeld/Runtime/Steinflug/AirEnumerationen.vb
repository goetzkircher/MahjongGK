''' <summary>
'''PfadMahjongGK\Spielfeld\Runtime\Steinflug\AirEnumerationen.vb
''' </summary>
Public Module AirEnumerationen
    Public Enum AirplaneModus
        '
        NotSet
        '
        ''' <summary>
        ''' Der Stein hängt an der Maus.
        ''' Der Ankerpunkt auf dem Stein wandert auf dem Spielfeld
        ''' in die Mitte des linken oberen Quadranten und im Vorrat
        ''' mittig in den Stein. (Diese Positionen sind am geeignetsten 
        ''' um mit der darunter teilweise sichtbaren Geistergrafik, die
        ''' mögliche Ablagepositionen anzeigt, konsitent spielen zu können.)
        ''' </summary>
        MousePaired
        '
        ''' <summary>
        ''' Standardflug.
        ''' Wenn FlyAnimated festgestellt wurde, wird später
        ''' die Streckenlänge geprüft und ggf umgeschaltet.
        ''' Wird zunächst nicht berücksichtigt.
        ''' </summary>
        FlyNotAnimated
        '
        ''' <summary>
        ''' Animierter Flug
        ''' </summary>
        FlyAnimated
        '
        ''' <summary>
        ''' Animiert verblassen 
        ''' </summary>
        GoOutTheWindow

    End Enum

    ''' <summary>
    ''' Hinweis: o.B. heißt "ohne Bedeutung"
    ''' </summary>
    Public Enum AirFlightRoute
        '
        NotSet
        '
        ''' <summary>
        ''' _mousePressed= True
        ''' _isValideSteinInfoIndex= o.B. 
        ''' _isSpiel= o.B.
        ''' _klickArea= Undo, HistoryBoxLeft, HistoryBoxRight, HistoryBoxSmall
        ''' _planeModus= FlyAnimated
        ''' _dstDecisionAtMouseUp= False
        ''' _removingCandidat= False
        ''' Beschreibung: Ob nur das letzte Steinpaar oder mehrere Steinpaare Undoed werden,
        ''' entscheidet sich anhand der SteinInfo(steinInfoIndex) außerhalb der Klasse.
        ''' </summary>
        Undo
        '
        ''' <summary>
        ''' _mousePressed= True
        ''' _isValideSteinInfoIndex= o.B. 
        ''' _isSpiel= o.B.
        ''' _klickArea= Redo
        ''' _planeModus= FlyAnimated
        ''' _dstDecisionAtMouseUp= False
        ''' _removingCandidat= False
        ''' Beschreibung: Redo nach einer Sicherheitsabfrage.
        ''' </summary>
        Redo

        ''' <summary>
        ''' _mousePressed= True
        ''' _isValideSteinInfoIndex= True
        ''' _isSpiel= True
        ''' _klickArea= StageUsed und nicht (HistoryBoxLeft oder HistoryBoxRight oder HistoryBoxSmall)
        ''' _planeModus= GoOutTheWindow
        ''' _dstDecisionAtMouseUp= False
        ''' _removingCandidat= True
        ''' Beschreibung: Klick auf einen Stein. Kandidat zum Entnehmen (deshalb _removingCandidat= True)
        ''' </summary>
        SpielfeldToHistoryNotVisible
        '
        ''' <summary>
        ''' _mousePressed= True
        ''' _isValideSteinInfoIndex= True
        ''' _isSpiel= True
        ''' _klickArea= StageUsed und (HistoryBoxLeft oder HistoryBoxRight oder HistoryBoxSmall)
        ''' _planeModus= FlyAnimated
        ''' _dstDecisionAtMouseUp= False
        ''' _removingCandidat= True
        ''' Beschreibung: Klick auf einen Stein. Kandidat zum Entnehmen (deshalb _removingCandidat= True)
        ''' </summary>
        SpielfeldToHistory
        '

        ''' <summary>
        ''' _mousePressed= True
        ''' _isValideSteinInfoIndex= o.B. 
        ''' _isSpiel= False
        ''' _klickArea= Stock
        ''' _airplaneModus=MousePaired
        ''' _dstDecisionAtMouseUp=True
        ''' _removingCandidat= False
        ''' Beschreibung: Stein wird dem Vorrat entnommen und hängt an der Maus.
        ''' Es ist noch unklar, ob er an anderer Stelle im Vorrat oder im Spielfeld
        ''' abgelegt wird, oder ob er zurückfliegt.
        ''' </summary>
        StockToUnKnown
        '
        ''' <summary>
        ''' _mousePressed= True
        ''' _isValideSteinInfoIndex= o.B.
        ''' _isSpiel= False
        ''' _klickArea= StageUsed
        ''' _airplaneModus= MousePaired
        ''' _dstDecisionAtMouseUp= True
        ''' _removingCandidat= False
        ''' Beschreibung: Stein wird dem Spielfeld entnommen und hängt an der Maus.
        ''' Es ist noch unklar, ob er an anderer Stelle im Spielfeld oder im Vorrat
        ''' abgelegt wird, oder ob er zurückfliegt.
        ''' </summary>
        EditfeldToUnKnown
        '
        ''' <summary>
        ''' _mousePressed= True
        ''' _isValideSteinInfoIndex=o.B. 
        ''' _isSpiel=o.B.
        ''' _klickArea= StockScrollbar oder HistoryBoxLeftScrollbar oder HistoryBoxRightScrollbar 
        ''' _airplaneModus= NotSet
        ''' _dstDecisionAtMouseUp= False
        ''' _removingCandidat= False
        ''' Beschreibung: Die Scrollbars werden hier nicht ausgewertet.
        ''' </summary>
        NoEvaluation = -1
        '
        ''' <summary>
        ''' _mousePressed=
        ''' _isValideSteinInfoIndex= 
        ''' _isSpiel=
        ''' _klickArea= 
        ''' _airplaneModus= NotSet
        ''' _dstDecisionAtMouseUp= False
        ''' _removingCandidat= False
        ''' Beschreibung: Nichts gefunden. Klick auf Header, leerem StageUsed, leerem Stock. 
        ''' </summary>
        NoFlightRoute = -2

    End Enum

    ''Public Enum AirKlickArea
    ''    None
    ''    Content
    ''    Stock
    ''    StockScrollbar
    ''    HistoryBoxLeft
    ''    HistoryBoxRight
    ''    HistoryBoxSmall
    ''    HistoryBoxLeftScrollbar
    ''    HistoryBoxRightScrollbar
    ''    StageSpielfeld
    ''    StageEditor
    ''    Undo
    ''    Redo
    ''    Header
    ''    Outside
    ''End Enum

    Public Enum AirKlickAreaGroup
        None
        Spielfeld
        Editor
        Historybox
        Button
        Scrollbar
        Outside
    End Enum

    Public Enum AircraftAnimation
        None = 0
        ScaleDown = 1
        ScaleUp = 2
        Rotate = 3
        RotateShrink = 4
        FlipX = 5
        FlipY = 6
        FlipShrink = 7
        Pulse = 8
        SlideLeft = 9
        SlideUp = 10
        ScaleSlide = 11
        RotatePulse = 12
    End Enum

    Public Enum AirKlickArea
        None
        '
        Content
        '
        ''' <summary>
        ''' Kann generell Ziel eines Steinfluges sein.
        ''' Kann Ziel eines Edit-Steinfluges sein.
        ''' </summary>
        Stock
        '
        ''' <summary>
        ''' Ist eine Scrollbar.
        ''' </summary>
        StockScrollbar
        '
        ''' <summary>
        ''' Kann generell Ziel eines Steinfluges sein.
        ''' Kann Ziel eines Spiel-Steinfluges sein.
        ''' Ist eine HistoryBox.
        ''' </summary>
        HistoryBoxLeft
        '
        ''' <summary>
        ''' Kann generell Ziel eines Steinfluges sein.
        ''' Kann Ziel eines Spiel-Steinfluges sein.
        ''' Ist eine HistoryBox.
        ''' </summary>
        HistoryBoxRight
        '
        ''' <summary>
        ''' Kann generell Ziel eines Steinfluges sein.
        ''' Kann Ziel eines Spiel-Steinfluges sein.
        ''' Ist eine HistoryBox.
        ''' </summary>
        HistoryBoxSmall
        '
        ''' <summary>
        ''' Ist eine Scrollbar.
        ''' </summary>
        HistoryBoxLeftScrollbar
        '
        ''' <summary>
        ''' Ist eine Scrollbar.
        ''' </summary>
        HistoryBoxRightScrollbar
        '
        ''' <summary>
        ''' Kann generell Ziel eines Steinfluges sein.
        ''' Kann Ziel eines Spiel-Steinfluges sein.
        ''' </summary>
        Spielfeld
        '
        ''' <summary>
        ''' Kann generell Ziel eines Steinfluges sein.
        ''' Kann Ziel eines Edit-Steinfluges sein.
        ''' </summary>
        Editor
        '
        ''' <summary>
        ''' Ist ein Button.
        ''' </summary>
        Undo
        '
        ''' <summary>
        ''' Ist ein Button.
        ''' </summary>
        Redo
        '
        Header
        '
        ''' <summary>
        ''' Wird derzeit nicht erreicht.
        ''' </summary>
        Outside
    End Enum

    Public Enum PlaneFlightPath
        Direkt = 0
        KurveOben = 1
        KurveUnten = 2
        BogenLinks = 3
        BogenRechts = 4
        ZickZack = 5
        Zufall = 6
    End Enum

    Public Enum AirAnimStartPos
        EckeLinksOben
        EckeRechtsOben
        EckeLinksUnten
        EckeRechtsUnten
    End Enum
    Public Enum AirFlightStepMode
        ''' <summary>
        ''' Echte geometrische Fluglänge (Pythagoras).
        ''' Gut für sichtbare Flugstrecke und Rotationsentscheidung.
        ''' </summary>
        Euclidean = 0

        ''' <summary>
        ''' Diskrete Raster-Schrittzahl wie bei einer Linienabtastung.
        ''' (Anzahl der Schritte in der Hauptachse = die längere Achse
        ''' bei der Zählung xs Schritte in X-Richung und ys Schritte in Y-Richtung.)
        ''' </summary>
        Raster = 1
    End Enum

    Public Enum AirplaneInsertAt
        First
        Second
        BeforeLast
        Last
        Random
    End Enum

End Module
