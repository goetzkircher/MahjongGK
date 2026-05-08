Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

Imports System.Runtime.InteropServices

'
''' <summary>
''' Pfad: MahjongGK/Spielfeld/UI/Mouse
''' 
''' Minimaler, instanzbasierter Maus-Poller (WinForms, .NET Framework 4.8.x).
''' 
''' Grundidee:
''' - Die physische Maus (Position, linke/rechte Taste) wird global gelesen.
''' - Ein MouseDown wird aber nur dann als "gültig für das Target" akzeptiert,
'''   wenn der Druck effektiv über dem Target begonnen hat.
''' - Das dazugehörige MouseUp wird danach auch außerhalb des Targets noch
'''   zugelassen, damit Drag-/Klicksequenzen sauber beendet werden können.
''' 
''' Damit werden insbesondere herausgefiltert:
''' - Klicks außerhalb des Target-Controls
''' - Klicks auf andere eigene Forms
''' - Klicks auf Menüs / ContextMenus
''' - Klicks auf fremde Programme, die über dem eigenen Fenster liegen
''' 
''' Wichtige Begriffe:
''' - Raw / Hardwarezustand:
'''   physischer Zustand der Maustaste, global gelesen
''' - Valid / gültige Klicksequenz:
'''   nur dann aktiv, wenn MouseDown auf dem Target begonnen hat
''' 
''' Die Methoden LeftMouseDown / RightMouseDown liefern daher bewusst NICHT
''' den rohen Hardwarezustand, sondern den gültigen, auf das Target bezogenen Zustand.
''' 
''' Zusatz:
''' - MouseEntered() / MouseLeft() sind nicht konsumierend.
''' - Consume...Pressed/Released sind konsumierende Flankenabfragen.
''' </summary>
Public NotInheritable Class MouseInputPoller
    ' Diese Klasse besitzt keine eigenen unmanaged Ressourcen,
    ' keine Timer, keine Event-Subscriptions und keine Handles,
    ' die sie selbst erzeugt oder freigeben müsste.
    ' Daher ist in dieser Form kein Dispose nötig.

#Region "Win32 interop"

    '
    ''' <summary>
    ''' Liefert das übergeordnete Fenster zu einem Handle.
    ''' Hier benutzt, um zum Root-Fenster eines Overlay-Controls zu kommen.
    ''' </summary>
    <DllImport("user32.dll", SetLastError:=False)>
    Private Shared Function GetAncestor(hWnd As IntPtr, gaFlags As UInteger) As IntPtr
    End Function

    '
    ''' <summary>
    ''' Liefert die aktuelle Cursorposition in Screen-Koordinaten.
    ''' </summary>
    <DllImport("user32.dll", SetLastError:=False)>
    Private Shared Function GetCursorPos(ByRef lpPoint As Point) As Boolean
    End Function

    '
    ''' <summary>
    ''' Wandelt einen Screen-Punkt in Client-Koordinaten eines Fensters um.
    ''' </summary>
    <DllImport("user32.dll", SetLastError:=False)>
    Private Shared Function ScreenToClient(hWnd As IntPtr, ByRef lpPoint As Point) As Integer
    End Function

    '
    ''' <summary>
    ''' Liest den aktuellen Hardwarezustand einer Taste.
    ''' Bit 15 gesetzt => Taste ist aktuell gedrückt.
    ''' </summary>
    <DllImport("user32.dll", SetLastError:=False)>
    Private Shared Function GetAsyncKeyState(vKey As Integer) As Short
    End Function

    '
    ''' <summary>
    ''' Liefert das Fenster/Control, das an einem Screen-Punkt aktuell "oben liegt".
    ''' Genau das ist hier wichtig, damit nicht nur geometrisch,
    ''' sondern auch visuell/oberflächenbezogen geprüft wird.
    ''' </summary>
    <DllImport("user32.dll", SetLastError:=False)>
    Private Shared Function WindowFromPoint(pt As Point) As IntPtr
    End Function

    '
    ''' <summary>
    ''' Prüft, ob ein Handle Kind eines anderen Fensters ist.
    ''' </summary>
    <DllImport("user32.dll", SetLastError:=False)>
    Private Shared Function IsChild(hWndParent As IntPtr, hWnd As IntPtr) As Boolean
    End Function

    Private Const GA_ROOT As UInteger = 2UI

    Private Const VK_LBUTTON As Integer = &H1
    Private Const VK_RBUTTON As Integer = &H2

#End Region

#Region "Felder"

    '
    ''' <summary>
    ''' True, solange ein fachlich freigegebenes DragDrop aktiv ist.
    ''' </summary>
    Private _dragDropActive As Boolean

    '
    ''' <summary>
    ''' Konsumierende Endflanke für DragDrop.
    ''' </summary>
    Private _dragEndedEdge As Boolean

    '
    ''' <summary>
    ''' True genau in dem ConsumeAirplanePolling-Durchlauf, in dem sich die Maus
    ''' während eines aktiven DragDrop bewegt hat.
    ''' </summary>
    Private _dragMoved As Boolean
    '
    ''' <summary>
    ''' Das Ziel-Control, für das die Maus logisch ausgewertet wird.
    ''' </summary>
    Private ReadOnly _target As Control

    '
    ''' <summary>
    ''' Reserviert für spätere Synchronisierung. Derzeit nicht aktiv benutzt.
    ''' Kann bleiben oder entfernt werden.
    ''' </summary>
    Private ReadOnly _sync As New Object()

    '
    ''' <summary>
    ''' Overlay-Roots, die logisch wie "durchlässig" behandelt werden sollen.
    ''' Beispiel: eigene transparente Hilfsfenster oder definierte Overlay-Controls.
    ''' 
    ''' Wenn der Cursor darüber liegt, soll dies trotzdem noch als "über dem Target"
    ''' gelten, sofern das Overlay bewusst registriert wurde.
    ''' </summary>
    Private ReadOnly _overlayRoots As New HashSet(Of IntPtr)()

    '
    ''' <summary>
    ''' Aktuelle Mausposition im Client-Koordinatensystem des Targets.
    ''' Wird nur dann aktiv geführt, wenn der Cursor effektiv über dem Target liegt.
    ''' </summary>
    Private _curPos As Point

    '
    ''' <summary>
    ''' Vorherige Client-Position.
    ''' </summary>
    Private _prevPos As Point

    '
    ''' <summary>
    ''' Positionsänderung seit dem letzten ConsumeAirplanePolling.
    ''' </summary>
    Private _delta As Point

    '
    ''' <summary>
    ''' Roher globaler Hardwarezustand der linken Taste.
    ''' Achtung: Das ist NICHT die Target-Logik, sondern nur "physisch gedrückt".
    ''' </summary>
    Private _leftDown As Boolean

    '
    ''' <summary>
    ''' Roher globaler Hardwarezustand der rechten Taste.
    ''' </summary>
    Private _rightDown As Boolean

    '
    ''' <summary>
    ''' Nicht konsumierende Änderungsflags für links/rechts.
    ''' Diese melden nur Änderungen, die für das Target logisch relevant sind.
    ''' </summary>
    Private _leftChanged As Boolean
    Private _rightChanged As Boolean

    '
    ''' <summary>
    ''' Nicht konsumierendes Änderungsflag für die Position.
    ''' </summary>
    Private _posChanged As Boolean

    '
    ''' <summary>
    ''' True, wenn der Cursor aktuell effektiv über dem Target ist.
    ''' "Effektiv" heißt:
    ''' - geometrisch im Bereich des Controls
    ''' - und nicht von einem fremden/anderen Fenster verdeckt,
    '''   außer es ist ein registriertes Overlay
    ''' </summary>
    Private _inside As Boolean

    '
    ''' <summary>
    ''' Flanke outside -> inside seit letztem ConsumeAirplanePolling.
    ''' Nicht konsumierend.
    ''' </summary>
    Private _entered As Boolean

    '
    ''' <summary>
    ''' Flanke inside -> outside seit letztem ConsumeAirplanePolling.
    ''' Nicht konsumierend.
    ''' </summary>
    Private _left As Boolean

    '
    ''' <summary>
    ''' Konsumierende Flankenflags für gültige Klicksequenzen.
    ''' Werden nur gesetzt, wenn die Sequenz für das Target gültig ist.
    ''' </summary>
    Private _leftPressedEdge As Boolean
    Private _leftReleasedEdge As Boolean
    Private _rightPressedEdge As Boolean
    Private _rightReleasedEdge As Boolean

    '
    ''' <summary>
    ''' True, solange eine gültige linke Klicksequenz aktiv ist.
    ''' 
    ''' Bedeutet:
    ''' - Die linke Maustaste wurde physisch gedrückt
    ''' - und dieses MouseDown begann effektiv über dem Target
    ''' - damit gehört das spätere MouseUp ebenfalls logisch zum Target
    ''' </summary>
    Private _leftValidDownActive As Boolean

    '
    ''' <summary>
    ''' Analog zur linken Taste für rechts.
    ''' </summary>
    Private _rightValidDownActive As Boolean

#End Region

#Region "Konstruktion"

    '
    ''' <summary>
    ''' Erstellt einen Poller für ein bestimmtes Ziel-Control.
    ''' Der Startzustand wird aus der aktuellen realen Mauslage abgeleitet.
    ''' </summary>
    ''' <param name="targetControl">Das Ziel-Control, für das die Maus logisch ausgewertet wird.</param>
    Public Sub New(targetControl As Control)

        If targetControl Is Nothing Then
            Throw New ArgumentNullException(NameOf(targetControl))
        End If

        _target = targetControl

        ' Aktuelle echte Mausposition im Screen holen
        Dim scr As Point = GetMouseScreenPosSafe()

        ' Prüfen, ob der Cursor beim Start effektiv über dem Target liegt
        Dim insideNow As Boolean = IsCursorEffectivelyOverTarget(scr)

        _inside = insideNow
        _entered = False
        _left = False

        ' Nur wenn der Cursor aktuell wirklich auf dem Target ist,
        ' wird eine sinnvolle Client-Position geführt.
        If insideNow AndAlso _target.IsHandleCreated Then
            _curPos = ScreenToClientSafe(scr, _target.Handle)
        Else
            _curPos = Point.Empty
        End If

        _prevPos = _curPos
        _delta = Point.Empty
        _posChanged = False

        ' Rohzustand der Tasten beim Erzeugen übernehmen
        _leftDown = IsKeyDown(VK_LBUTTON)
        _rightDown = IsKeyDown(VK_RBUTTON)

        _leftChanged = False
        _rightChanged = False

        _leftPressedEdge = False
        _leftReleasedEdge = False
        _rightPressedEdge = False
        _rightReleasedEdge = False

        ' Beim Start wird absichtlich keine gültige Klicksequenz angenommen.
        ' Auch wenn die Taste physisch schon gedrückt wäre, wissen wir nicht,
        ' ob das Down auf dem Target begonnen hat.
        _leftValidDownActive = False
        _rightValidDownActive = False

        _dragDropActive = False
        _dragEndedEdge = False
        _dragMoved = False

    End Sub

#End Region

#Region "API"

    '
    ''' <summary>
    ''' Aktualisiert den Zustand.
    ''' 
    ''' Rückgabe:
    ''' True, wenn sich seit dem letzten ConsumeAirplanePolling() etwas geändert hat,
    ''' das für die weitere Auswertung relevant sein kann.
    ''' </summary>
    Public Function Poll() As Boolean

        Dim anyChange As Boolean = False
        _dragMoved = False

        ' ---------------------------------------------------------
        ' 1) Aktuelle Screen-Position und inside-Zustand bestimmen
        ' ---------------------------------------------------------
        Dim scr As Point = GetMouseScreenPosSafe()
        Dim insideNow As Boolean = IsCursorEffectivelyOverTarget(scr)

        ' Enter-/Leave-Flanken zum bisherigen Zustand
        Dim enteredNow As Boolean = (Not _inside) AndAlso insideNow
        Dim leftNow As Boolean = _inside AndAlso (Not insideNow)

        _entered = enteredNow
        _left = leftNow

        If _entered OrElse _left Then
            anyChange = True
        End If

        ' ---------------------------------------------------------
        ' 2) Aktuellen rohen Hardwarezustand der Tasten lesen
        ' ---------------------------------------------------------
        Dim lNow As Boolean = IsKeyDown(VK_LBUTTON)
        Dim rNow As Boolean = IsKeyDown(VK_RBUTTON)

        ' Flanke = hat sich der rohe Hardwarezustand geändert?
        Dim leftChangedGlobal As Boolean = (lNow <> _leftDown)
        Dim rightChangedGlobal As Boolean = (rNow <> _rightDown)

        ' Änderungsflags pro ConsumeAirplanePolling neu aufbauen
        _leftChanged = False
        _rightChanged = False

        ' ---------------------------------------------------------
        ' 3) Linke Taste logisch filtern
        ' ---------------------------------------------------------
        If leftChangedGlobal Then

            If lNow Then
                ' -----------------------------
                ' Linkes MouseDown
                ' -----------------------------
                ' Nur akzeptieren, wenn der Druck effektiv über dem Target begann.
                ' Alles andere wird als Start einer gültigen Sequenz verworfen.
                If insideNow Then
                    _leftValidDownActive = True
                    _leftPressedEdge = True
                    _leftChanged = True
                Else
                    _leftValidDownActive = False
                End If

            Else
                ' -----------------------------
                ' Linkes MouseUp
                ' -----------------------------
                ' Das MouseUp wird auch außerhalb zugelassen,
                ' aber nur wenn zuvor ein gültiges MouseDown aktiv war.
                If _leftValidDownActive Then
                    _leftReleasedEdge = True
                    _leftChanged = True

                    If _dragDropActive Then
                        _dragEndedEdge = True
                        _dragDropActive = False
                    End If
                End If

                ' Nach dem Release ist die gültige Sequenz beendet.
                _leftValidDownActive = False
            End If

        End If

        ' ---------------------------------------------------------
        ' 4) Rechte Taste logisch filtern
        ' ---------------------------------------------------------
        If rightChangedGlobal Then

            If rNow Then
                ' Rechtes MouseDown nur akzeptieren,
                ' wenn es effektiv über dem Target beginnt.
                If insideNow Then
                    _rightValidDownActive = True
                    _rightPressedEdge = True
                    _rightChanged = True
                Else
                    _rightValidDownActive = False
                End If

            Else
                ' Rechtes MouseUp auch außerhalb zulassen,
                ' aber nur für eine gültig gestartete Sequenz.
                If _rightValidDownActive Then
                    _rightReleasedEdge = True
                    _rightChanged = True
                End If

                _rightValidDownActive = False
            End If

        End If

        ' ---------------------------------------------------------
        ' 5) Rohzustand immer global fortschreiben
        ' ---------------------------------------------------------
        ' Sehr wichtig:
        ' Die Hardwarezustände müssen immer korrekt aktuell bleiben,
        ' auch wenn die Flanke logisch verworfen wurde.
        _leftDown = lNow
        _rightDown = rNow

        ' ---------------------------------------------------------
        ' 6) Position nur dann auswerten, wenn der Cursor effektiv
        '    über dem Target liegt
        ' ---------------------------------------------------------
        If insideNow AndAlso _target.IsHandleCreated Then

            Dim newPos As Point = ScreenToClientSafe(scr, _target.Handle)

            _prevPos = _curPos
            _curPos = newPos

            Dim dx As Integer = _curPos.X - _prevPos.X
            Dim dy As Integer = _curPos.Y - _prevPos.Y
            _delta = New Point(dx, dy)

            _posChanged = (dx <> 0 OrElse dy <> 0)

            If _posChanged Then
                anyChange = True
            End If

        Else
            ' Außerhalb keine neue Client-Position führen
            _prevPos = _curPos
            _delta = Point.Empty
            _posChanged = False
        End If

        If _dragDropActive AndAlso _posChanged Then
            _dragMoved = True
        End If
        ' ---------------------------------------------------------
        ' 7) Wiedereintritt ins Target während einer gültigen Sequenz
        ' ---------------------------------------------------------
        ' Wenn z.B. links gültig auf dem Target gedrückt wurde,
        ' der Cursor danach das Target verlässt und später wieder eintritt,
        ' darf dies erneut als relevante Änderung gemeldet werden.
        '
        ' ABER:
        ' Nur für gültig gestartete Sequenzen, nicht für beliebiges
        ' "von außen gedrückt hereingezogen".
        If enteredNow Then

            If _leftValidDownActive AndAlso lNow Then
                _leftChanged = True
            End If

            If _rightValidDownActive AndAlso rNow Then
                _rightChanged = True
            End If

        End If

        If _leftChanged OrElse _rightChanged Then
            anyChange = True
        End If

        ' Bisherigen inside-Zustand auf den aktuellen Stand bringen
        _inside = insideNow

        Return anyChange

    End Function

    ' -------------------------------------------------------------
    ' Nicht konsumierende Zustandsabfragen
    ' -------------------------------------------------------------

    '
    ''' <summary>
    ''' True, wenn seit dem letzten ConsumeAirplanePolling() eine für das Target relevante
    ''' Änderung der linken Taste festgestellt wurde.
    ''' Nicht konsumierend.
    ''' </summary>
    Public Function LeftMouseChanged() As Boolean

        Return _leftChanged

    End Function

    '
    ''' <summary>
    ''' True, solange eine gültige linke Klicksequenz aktiv ist.
    ''' 
    ''' Achtung:
    ''' Das ist NICHT einfach der rohe Hardwarezustand,
    ''' sondern der logisch für das Target gültige Zustand.
    ''' </summary>
    Public Function LeftMouseDown() As Boolean

        Return _leftValidDownActive

    End Function

    '
    ''' <summary>
    ''' Inverse der gültigen linken Klicksequenz.
    ''' </summary>
    Public Function LeftMouseUp() As Boolean

        Return Not _leftValidDownActive

    End Function

    '
    ''' <summary>
    ''' True, wenn seit dem letzten ConsumeAirplanePolling() eine für das Target relevante
    ''' Änderung der rechten Taste festgestellt wurde.
    ''' Nicht konsumierend.
    ''' </summary>
    Public Function RightMouseChanged() As Boolean

        Return _rightChanged

    End Function

    '
    ''' <summary>
    ''' True, solange eine gültige rechte Klicksequenz aktiv ist.
    ''' </summary>
    Public Function RightMouseDown() As Boolean

        Return _rightValidDownActive

    End Function

    '
    ''' <summary>
    ''' Inverse der gültigen rechten Klicksequenz.
    ''' </summary>
    Public Function RightMouseUp() As Boolean

        Return Not _rightValidDownActive

    End Function

    ' -------------------------------------------------------------
    ' Konsumierende Flankenabfragen
    ' -------------------------------------------------------------

    '
    ''' <summary>
    ''' Liefert einmalig die gültige Press-Flanke der linken Taste.
    ''' Danach wird das Flag zurückgesetzt.
    ''' </summary>
    Public Function ConsumeLeftMousePressed() As Boolean

        Dim result As Boolean = _leftPressedEdge
        _leftPressedEdge = False
        Return result

    End Function

    '
    ''' <summary>
    ''' Liefert einmalig die gültige Release-Flanke der linken Taste.
    ''' Danach wird das Flag zurückgesetzt.
    ''' </summary>
    Public Function ConsumeLeftMouseReleased() As Boolean

        Dim result As Boolean = _leftReleasedEdge
        _leftReleasedEdge = False
        Return result

    End Function

    '
    ''' <summary>
    ''' Liefert einmalig die gültige Press-Flanke der rechten Taste.
    ''' Danach wird das Flag zurückgesetzt.
    ''' </summary>
    Public Function ConsumeRightMousePressed() As Boolean

        Dim result As Boolean = _rightPressedEdge
        _rightPressedEdge = False
        Return result

    End Function

    '
    ''' <summary>
    ''' Liefert einmalig die gültige Release-Flanke der rechten Taste.
    ''' Danach wird das Flag zurückgesetzt.
    ''' </summary>
    Public Function ConsumeRightMouseReleased() As Boolean

        Dim result As Boolean = _rightReleasedEdge
        _rightReleasedEdge = False
        Return result

    End Function

    ' -------------------------------------------------------------
    ' Positionsabfragen
    ' -------------------------------------------------------------

    '
    ''' <summary>
    ''' True, wenn sich die geführte Mausposition seit dem letzten ConsumeAirplanePolling()
    ''' geändert hat.
    ''' Nur relevant, solange der Cursor effektiv über dem Target ist.
    ''' </summary>
    Public Function MousePosChanged() As Boolean

        Return _posChanged

    End Function

    '
    ''' <summary>
    ''' Aktuelle geführte Mausposition im Client-System des Targets.
    ''' </summary>
    Public Function MousePos() As Point

        Return _curPos

    End Function

    '
    ''' <summary>
    ''' Gibt einen Point mit aktuellem X und übergebenem Y zurück.
    ''' Praktisch für Hilfskonstruktionen beim Rendern oder Testen.
    ''' </summary>
    Public Function MousePosX(y As Integer) As Point

        Return New Point(_curPos.X, y)

    End Function

    '
    ''' <summary>
    ''' Gibt einen Point mit übergebenem X und aktuellem Y zurück.
    ''' </summary>
    Public Function MousePosY(x As Integer) As Point

        Return New Point(x, _curPos.Y)

    End Function

    '
    ''' <summary>
    ''' Positionsdifferenz seit dem letzten ConsumeAirplanePolling().
    ''' </summary>
    Public Function MousePosDelta() As Point

        Return _delta

    End Function

    ' -------------------------------------------------------------
    ' Bereichs- / Enter-Leave-Abfragen
    ' -------------------------------------------------------------

    '
    ''' <summary>
    ''' True, wenn der Cursor aktuell effektiv über dem Target liegt.
    ''' </summary>
    Public Function IsInsideTarget() As Boolean

        Return _inside

    End Function

    '
    ''' <summary>
    ''' True, wenn seit dem letzten ConsumeAirplanePolling() ein Übergang
    ''' von außerhalb nach innerhalb stattgefunden hat.
    ''' Nicht konsumierend.
    ''' </summary>
    Public Function MouseEntered() As Boolean

        Return _entered

    End Function

    '
    ''' <summary>
    ''' True, wenn seit dem letzten ConsumeAirplanePolling() ein Übergang
    ''' von innerhalb nach außerhalb stattgefunden hat.
    ''' Nicht konsumierend.
    ''' </summary>
    Public Function MouseLeft() As Boolean

        Return _left

    End Function

    ' -------------------------------------------------------------
    ' Drag-Drop-Abfragen
    ' --------chrome://vivaldi-webui/startpage?section=Speed-dials&background-color=#111112-----------------------------------------------------

    '
    ''' <summary>
    ''' Startet ein DragDrop.
    ''' Voraussetzung: gültiges linkes MouseDown ist aktiv.
    ''' </summary>
    Public Sub StartDragDrop()

        If _dragDropActive Then
            Throw New InvalidOperationException("Programmierfehler: DragDrop ist bereits aktiv.")
        End If

        If Not _leftValidDownActive Then
            Throw New InvalidOperationException("Programmierfehler: StartDragDrop ohne gültiges linkes MouseDown.")
        End If

        _dragDropActive = True
        _dragEndedEdge = False
        _dragMoved = False

    End Sub

    '
    ''' <summary>
    ''' Bricht ein aktives DragDrop sofort ab.
    ''' Das spätere MouseUp liefert dann kein DragDrop-Ende mehr.
    ''' </summary>
    Public Sub AbortDragDrop()

        _dragDropActive = False
        _dragEndedEdge = False
        _dragMoved = False

    End Sub

    '
    ''' <summary>
    ''' True genau in dem ConsumeAirplanePolling-Durchlauf, in dem sich die Maus
    ''' während eines aktiven DragDrop bewegt hat.
    ''' </summary>
    Public Function DragDropMoved() As Boolean

        Return _dragMoved

    End Function

    ''' <summary>
    ''' True in jedem Durchlauf, ausgenommen
    ''' ConsumeEndDragDrop
    ''' </summary>
    Public Function DragDropActive() As Boolean

        Return _dragDropActive

    End Function

    '
    ''' <summary>
    ''' Liefert einmalig das Ende eines aktiven DragDrop.
    ''' </summary>
    Public Function ConsumeEndDragDrop() As Boolean

        Dim result As Boolean = _dragEndedEdge
        _dragEndedEdge = False
        Return result

    End Function

    ' -------------------------------------------------------------
    ' Optionale Raw-Abfragen
    ' -------------------------------------------------------------
    ' Diese beiden Methoden liefern den reinen Hardwarezustand.
    ' Damit bleibt die Trennung sauber:
    ' - LeftMouseDown()  = gültig fürs Target
    ' - LeftMouseDownRaw() = physisch gedrückt

    '
    ''' <summary>
    ''' Roher Hardwarezustand der linken Maustaste.
    ''' Ohne Target-Filterung.
    ''' </summary>
    Public Function LeftMouseDownRaw() As Boolean

        Return _leftDown

    End Function

    '
    ''' <summary>
    ''' Roher Hardwarezustand der rechten Maustaste.
    ''' Ohne Target-Filterung.
    ''' </summary>
    Public Function RightMouseDownRaw() As Boolean

        Return _rightDown

    End Function

#End Region

#Region "Overlay API"

    '
    ''' <summary>
    ''' Registriert ein Control als Overlay-Root.
    ''' Ein Klick/Pointer über diesem Overlay kann damit weiterhin
    ''' logisch als "über dem Target" gelten.
    ''' </summary>
    Public Sub RegisterOverlay(ctrl As Control)

        If ctrl Is Nothing Then Return

        Dim h As IntPtr = ctrl.Handle
        If h <> IntPtr.Zero Then
            _overlayRoots.Add(h)
        End If

    End Sub

    '
    ''' <summary>
    ''' Registriert direkt ein Fensterhandle als Overlay-Root.
    ''' </summary>
    Public Sub RegisterOverlay(hWnd As IntPtr)

        If hWnd = IntPtr.Zero Then Return

        _overlayRoots.Add(hWnd)

    End Sub

    '
    ''' <summary>
    ''' Entfernt ein Overlay-Root wieder aus der Berücksichtigung.
    ''' </summary>
    Public Sub UnregisterOverlay(hWnd As IntPtr)

        If hWnd = IntPtr.Zero Then Return

        _overlayRoots.Remove(hWnd)

    End Sub

#End Region

#Region "HitTest / Helper"

    '
    ''' <summary>
    ''' Grobprüfung:
    ''' Liegt der Screen-Punkt geometrisch im Screen-Rechteck des Target-Controls?
    ''' 
    ''' Diese Prüfung genügt allein nicht, weil das Control verdeckt sein könnte.
    ''' </summary>
    Private Function IsCursorOverTargetScreen(screenPos As Point) As Boolean

        If _target Is Nothing Then Return False
        If Not _target.IsHandleCreated Then Return False

        Dim r As Rectangle = _target.RectangleToScreen(_target.ClientRectangle)
        Return r.Contains(screenPos)

    End Function

    '
    ''' <summary>
    ''' Feine "liegt wirklich auf dem Target"-Prüfung.
    ''' 
    ''' Logik:
    ''' 1) Geometrisch im Bereich des Target?
    ''' 2) Welches Fenster/Control liegt an dieser Stelle wirklich oben?
    ''' 3) Ist das das Target, ein Kind davon oder ein bewusst registriertes Overlay?
    ''' 
    ''' Nur dann gilt der Cursor als effektiv über dem Target.
    ''' </summary>
    Private Function IsCursorEffectivelyOverTarget(screenPos As Point) As Boolean

        ' 1) Grobe Geometrieprüfung
        If Not IsCursorOverTargetScreen(screenPos) Then
            Return False
        End If

        ' 2) Tatsächlich oberstes Fenster an der Position
        Dim h As IntPtr = WindowFromPoint(screenPos)
        If h = IntPtr.Zero Then
            Return False
        End If

        Dim targetH As IntPtr = _target.Handle

        ' 2a) Das Target selbst oder ein Kind davon?
        If h = targetH OrElse IsChild(targetH, h) Then
            Return True
        End If

        ' 2b) Registrierte Overlays wie "durchlässig" behandeln
        Dim hRoot As IntPtr = GetAncestor(h, GA_ROOT)

        For Each ov As IntPtr In _overlayRoots
            If h = ov OrElse hRoot = ov OrElse IsChild(ov, h) Then
                Return True
            End If
        Next

        ' Alles andere gilt als nicht über dem Target
        Return False

    End Function

    '
    ''' <summary>
    ''' Holt die Mausposition sicher in Screen-Koordinaten.
    ''' Falls die API wider Erwarten fehlschlägt, wird Point.Empty geliefert.
    ''' </summary>
    Private Shared Function GetMouseScreenPosSafe() As Point

        Dim p As Point
        If GetCursorPos(p) Then
            Return p
        End If

        Return Point.Empty

    End Function

    '
    ''' <summary>
    ''' Wandelt Screen-Koordinaten in Client-Koordinaten des angegebenen Fensters um.
    ''' </summary>
    Private Shared Function ScreenToClientSafe(screenPos As Point, hwnd As IntPtr) As Point

        Dim p As Point = screenPos
        Call ScreenToClient(hwnd, p)
        Return p

    End Function

    '
    ''' <summary>
    ''' True, wenn die angegebene Taste physisch gerade gedrückt ist.
    ''' </summary>
    Private Shared Function IsKeyDown(vKey As Integer) As Boolean

        Return (GetAsyncKeyState(vKey) And &H8000S) <> 0

    End Function

#End Region

End Class