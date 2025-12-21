'Hier ist die aktuelle Version des MouseInputPoller.
'Die Mausbewegung wird nur erfasst, wenn die Maus sich innerhalb des UserControls befindet.
'Ist das UserControl von einem anderem Programm überdeckt, wird die Mausbewegung dennoch erfasst
'und mein Renderer tritt in aktion. Läst sich das Ändern?


Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

Imports System.Runtime.InteropServices

''' <summary>
''' Instanzbasierter, ereignisloser Maus-Poller (WinForms, .NET Framework 4.8.x).
''' - Poll() zyklisch aufrufen; True bei Mausaktivität (Move/Buttons/Wheel)
''' - Detailinfos via konsumierende Getter (selbstrückstellend)
''' - Mausrad via IMessageFilter (app-weit), aber nur gezählt, wenn Cursor über dem Ziel-Control
''' - Enthält Methoden, um den Cursor tatsächlich zu bewegen (Client-Koordinaten)
''' </summary>
Public NotInheritable Class MouseInputPoller
    Implements IDisposable

    ' Verwendung:
    ' Feld im Form/UserControl:
    '    Private _mouse As MouseInputPoller
    '
    '' z.B. nach HandleCreated:
    '_mouse = New MouseInputPoller(Me.RenderSurfaceControl)
    '
    '' im Render-Takt:
    'If _mouse.Poll() Then
    '    Dim d As Size = _mouse.ConsumeCursorMovedDelta()
    '    Dim wV As Integer = _mouse.ConsumeWheelDeltaVertical()
    '    Dim wH As Integer = _mouse.ConsumeWheelDeltaHorizontal()
    '    ' ... reagieren & rendern ...
    '    End If
    '
    '' beim Schließen:
    '_mouse.Dispose()

#Region "Win32 interop"

    <DllImport("user32.dll")>
    Private Shared Function GetAncestor(hWnd As IntPtr, gaFlags As UInteger) As IntPtr
    End Function

    <DllImport("user32.dll")>
    Private Shared Function GetCursorPos(ByRef lpPoint As Point) As Boolean
    End Function

    <DllImport("user32.dll")>
    Private Shared Function ScreenToClient(hWnd As IntPtr, ByRef lpPoint As Point) As Integer
    End Function

    <DllImport("user32.dll")>
    Private Shared Function ClientToScreen(hWnd As IntPtr, ByRef lpPoint As Point) As Integer
    End Function

    <DllImport("user32.dll")>
    Private Shared Function SetCursorPos(X As Integer, Y As Integer) As Boolean
    End Function

    <DllImport("user32.dll")>
    Private Shared Function GetAsyncKeyState(vKey As Integer) As Short
    End Function

    <DllImport("user32.dll")>
    Private Shared Function WindowFromPoint(pt As Point) As IntPtr
    End Function

    <DllImport("user32.dll")>
    Private Shared Function IsChild(hWndParent As IntPtr, hWnd As IntPtr) As Boolean
    End Function


    Private Const GA_ROOT As UInteger = 2UI

    Private Const VK_LBUTTON As Integer = &H1
    Private Const VK_RBUTTON As Integer = &H2
    Private Const VK_MBUTTON As Integer = &H4
    Private Const VK_XBUTTON1 As Integer = &H5
    Private Const VK_XBUTTON2 As Integer = &H6

#End Region

#Region "Konstruktion / Lebenszyklus"

    Private ReadOnly _target As Control
    Private ReadOnly _sync As New Object
    Private ReadOnly _filter As WheelFilter ' je Instanz
    Private _disposed As Boolean

    ' registrierte Top-Level/Root-Fenster der Overlays 
    ' z.B. für ToolTip, ContextMenu, eigene Form o.ä.
    Private ReadOnly _overlayRoots As New HashSet(Of IntPtr)()

    Public Sub New(targetControl As Control)
        If targetControl Is Nothing Then Throw New ArgumentNullException(NameOf(targetControl))
        _target = targetControl
        _filter = New WheelFilter(AddressOf IsCursorOverTargetScreen)
        Application.AddMessageFilter(_filter)
        ResetStateInternal()
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        If _disposed Then Return
        SyncLock _sync
            Application.RemoveMessageFilter(_filter)
            _disposed = True
        End SyncLock
        GC.SuppressFinalize(Me)
    End Sub
#End Region

#Region "Interner Zustand"
    Private _curPosClient As Point
    Private _prevPosClient As Point
    Private _curInside As Boolean
    Private _prevInside As Boolean

    Private _btnL As Boolean, _btnR As Boolean, _btnM As Boolean, _btnX1 As Boolean, _btnX2 As Boolean
    Private _btnL_prev As Boolean, _btnR_prev As Boolean, _btnM_prev As Boolean, _btnX1_prev As Boolean, _btnX2_prev As Boolean

    ' selbstrückstellend
    Private _pressedL As Boolean, _pressedR As Boolean, _pressedM As Boolean, _pressedX1 As Boolean, _pressedX2 As Boolean
    Private _releasedL As Boolean, _releasedR As Boolean, _releasedM As Boolean, _releasedX1 As Boolean, _releasedX2 As Boolean

    Private _deltaSinceConsume As Size
    Private _wheelDeltaV As Integer
    Private _wheelDeltaH As Integer

    Private _changedSinceLastPoll As Boolean
    Private _lastActivityStamp As Long

#End Region

#Region "Öffentliche API"

    ' Haupt-Poll
    Public Function Poll() As Boolean
        If _disposed Then Return False
        SyncLock _sync
            Dim anyChange As Boolean = False

            ' 1) Position & Inside
            Dim scr As Point = GetCursorPosScreen()
            Dim insideNow As Boolean = IsCursorEffectivelyOverTarget(scr)
            Dim cli As Point =
                If(_target IsNot Nothing AndAlso _target.IsHandleCreated,
                   ScreenToClientSafe(scr, _target.Handle),
                   Point.Empty)

            If insideNow Then
                If cli <> _curPosClient Then
                    Dim dx As Integer = cli.X - _curPosClient.X
                    Dim dy As Integer = cli.Y - _curPosClient.Y
                    _deltaSinceConsume = New Size(_deltaSinceConsume.Width + dx, _deltaSinceConsume.Height + dy)
                    anyChange = True
                End If
            End If

            _prevPosClient = _curPosClient
            _curPosClient = cli
            _prevInside = _curInside
            _curInside = insideNow
            If _curInside <> _prevInside Then anyChange = True

            ' 2) Buttons (AsyncKeyState)
            Dim lNow As Boolean = (GetAsyncKeyState(VK_LBUTTON) And &H8000S) <> 0
            Dim rNow As Boolean = (GetAsyncKeyState(VK_RBUTTON) And &H8000S) <> 0
            Dim mNow As Boolean = (GetAsyncKeyState(VK_MBUTTON) And &H8000S) <> 0
            Dim x1Now As Boolean = (GetAsyncKeyState(VK_XBUTTON1) And &H8000S) <> 0
            Dim x2Now As Boolean = (GetAsyncKeyState(VK_XBUTTON2) And &H8000S) <> 0

            If _curInside Then
                EdgeUpdate(lNow, _btnL, _pressedL, _releasedL, anyChange)
                EdgeUpdate(rNow, _btnR, _pressedR, _releasedR, anyChange)
                EdgeUpdate(mNow, _btnM, _pressedM, _releasedM, anyChange)
                EdgeUpdate(x1Now, _btnX1, _pressedX1, _releasedX1, anyChange)
                EdgeUpdate(x2Now, _btnX2, _pressedX2, _releasedX2, anyChange)
            End If

            _btnL = lNow : _btnR = rNow : _btnM = mNow : _btnX1 = x1Now : _btnX2 = x2Now

            ' 3) Wheel (vom instanzgebundenen Filter)
            If _curInside Then
                Dim dV As Integer = _filter.ConsumeWheel()
                Dim dH As Integer = _filter.ConsumeWheelH()
                If dV <> 0 Then _wheelDeltaV += dV : anyChange = True
                If dH <> 0 Then _wheelDeltaH += dH : anyChange = True
            Else
                ' Zähler im Filter zurücksetzen, aber ignorieren
                Call _filter.ConsumeWheel()
                Call _filter.ConsumeWheelH()
            End If

            _changedSinceLastPoll = anyChange
            If anyChange Then _lastActivityStamp = Stopwatch.GetTimestamp()
            Return anyChange
        End SyncLock
    End Function

    Public ReadOnly Property HasMouseActivitySinceLastPoll As Boolean
        Get
            Return _changedSinceLastPoll
        End Get
    End Property

    Public Function GetMillisSinceLastMouseActivity() As Long
        Dim dticks As Long = Stopwatch.GetTimestamp() - _lastActivityStamp
        Return CLng(1000.0 * dticks / Stopwatch.Frequency)
    End Function

    ' Position / Bereich
    ''' <summary>Aktuelle Cursor-Position in Client-Koordinaten des Ziel-Controls.</summary>
    Public Function GetCursorPosClient() As Point
        SyncLock _sync
            Return _curPosClient
        End SyncLock
    End Function

    ''' <summary>True, wenn der Cursor im Clientbereich des Ziel-Controls liegt.</summary>
    Public Function IsCursorInsideClient() As Boolean
        SyncLock _sync
            Return _curInside
        End SyncLock
    End Function

    ' Bewegungen (selbstrückstellend)
    ''' <summary>Delta der seit dem letzten Abfragen stattgefundenen Cursorbewegung (Client-Koords). Selbstrückstellend.</summary>
    Public Function ConsumeCursorMovedDelta() As Size
        SyncLock _sync
            Dim d As Size = _deltaSinceConsume
            _deltaSinceConsume = Size.Empty
            Return d
        End SyncLock
    End Function

    ' Mausrad (selbstrückstellend)
    ''' <summary>Vertikales Wheel-Delta (Vielfache von 120). Selbstrückstellend.</summary>
    Public Function ConsumeWheelDeltaVertical() As Integer
        SyncLock _sync
            Dim d As Integer = _wheelDeltaV
            _wheelDeltaV = 0
            Return d
        End SyncLock
    End Function

    ''' <summary>Horizontales Wheel-Delta (Vielfache von 120). Selbstrückstellend.</summary>
    Public Function ConsumeWheelDeltaHorizontal() As Integer
        SyncLock _sync
            Dim d As Integer = _wheelDeltaH
            _wheelDeltaH = 0
            Return d
        End SyncLock
    End Function

    ' Buttons
    Public Function IsMouseButtonDown(btn As MouseButtons) As Boolean
        SyncLock _sync
            Select Case btn
                Case MouseButtons.Left : Return _btnL
                Case MouseButtons.Right : Return _btnR
                Case MouseButtons.Middle : Return _btnM
                Case MouseButtons.XButton1 : Return _btnX1
                Case MouseButtons.XButton2 : Return _btnX2
                Case Else : Return False
            End Select
        End SyncLock
    End Function

    Public Function ConsumeMouseButtonPressed(btn As MouseButtons) As Boolean
        SyncLock _sync
            Select Case btn
                Case MouseButtons.Left
                    Dim b As Boolean = _pressedL : _pressedL = False : Return b
                Case MouseButtons.Right
                    Dim b As Boolean = _pressedR : _pressedR = False : Return b
                Case MouseButtons.Middle
                    Dim b As Boolean = _pressedM : _pressedM = False : Return b
                Case MouseButtons.XButton1
                    Dim b As Boolean = _pressedX1 : _pressedX1 = False : Return b
                Case MouseButtons.XButton2
                    Dim b As Boolean = _pressedX2 : _pressedX2 = False : Return b
                Case Else
                    Return False
            End Select
        End SyncLock
    End Function

    Public Function ConsumeMouseButtonReleased(btn As MouseButtons) As Boolean
        SyncLock _sync
            Select Case btn
                Case MouseButtons.Left
                    Dim b As Boolean = _releasedL : _releasedL = False : Return b
                Case MouseButtons.Right
                    Dim b As Boolean = _releasedR : _releasedR = False : Return b
                Case MouseButtons.Middle
                    Dim b As Boolean = _releasedM : _releasedM = False : Return b
                Case MouseButtons.XButton1
                    Dim b As Boolean = _releasedX1 : _releasedX1 = False : Return b
                Case MouseButtons.XButton2
                    Dim b As Boolean = _releasedX2 : _releasedX2 = False : Return b
                Case Else
                    Return False
            End Select
        End SyncLock
    End Function

    ' Sammelabfrage (selbstrückstellend)
    Public Function ConsumeAnyMouseActivity(
        Optional ByRef moveDelta As Size = Nothing,
        Optional ByRef wheelVertical As Integer = 0,
        Optional ByRef wheelHorizontal As Integer = 0) As Boolean

        Dim any As Boolean = False
        Dim d As Size = ConsumeCursorMovedDelta() : If d <> Size.Empty Then any = True
        Dim wv As Integer = ConsumeWheelDeltaVertical() : If wv <> 0 Then any = True
        Dim wh As Integer = ConsumeWheelDeltaHorizontal() : If wh <> 0 Then any = True
        moveDelta = d : wheelVertical = wv : wheelHorizontal = wh

        If PeekAnyEdge() Then any = True
        Return any
    End Function

    ' Cursor tatsächlich bewegen (aktiv)
    ''' <summary>Setzt den System-Cursor auf eine Client-Position des Ziel-Controls (intern nach Screen konvertiert).</summary>
    Public Sub SetCursorPosClient(targetPosClient As Point)
        SyncLock _sync
            If _target Is Nothing OrElse Not _target.IsHandleCreated Then Exit Sub
            Dim scr As Point = ClientToScreenSafe(targetPosClient, _target.Handle)
            Call SetCursorPos(scr.X, scr.Y)
        End SyncLock
    End Sub

    ''' <summary>Bewegt den Cursor relativ zur aktuellen Client-Position um delta (Client-Koords).</summary>
    Public Sub MoveCursorByClientDelta(delta As Size)
        SyncLock _sync
            If _target Is Nothing OrElse Not _target.IsHandleCreated Then Exit Sub
            Dim currentClient As Point = _curPosClient
            Dim newClient As New Point(currentClient.X + delta.Width, currentClient.Y + delta.Height)
            Dim scr As Point = ClientToScreenSafe(newClient, _target.Handle)
            Call SetCursorPos(scr.X, scr.Y)
        End SyncLock
    End Sub
#End Region

#Region "Hilfsroutinen"
    Private Sub ResetStateInternal()
        _curPosClient = Point.Empty
        _prevPosClient = Point.Empty
        _curInside = False
        _prevInside = False

        _btnL = False : _btnR = False : _btnM = False : _btnX1 = False : _btnX2 = False
        _btnL_prev = False : _btnR_prev = False : _btnM_prev = False : _btnX1_prev = False : _btnX2_prev = False

        _pressedL = False : _pressedR = False : _pressedM = False : _pressedX1 = False : _pressedX2 = False
        _releasedL = False : _releasedR = False : _releasedM = False : _releasedX1 = False : _releasedX2 = False

        _deltaSinceConsume = Size.Empty
        _wheelDeltaV = 0
        _wheelDeltaH = 0
        _changedSinceLastPoll = False
        _lastActivityStamp = Stopwatch.GetTimestamp()
    End Sub

    Private Shared Sub EdgeUpdate(now As Boolean, ByRef prev As Boolean,
                                  ByRef pressedEdge As Boolean, ByRef releasedEdge As Boolean,
                                  ByRef anyChange As Boolean)
        If now <> prev Then
            If now Then
                pressedEdge = True
            Else
                releasedEdge = True
            End If
            anyChange = True
        End If
        prev = now
    End Sub

    Private Function PeekAnyEdge() As Boolean
        SyncLock _sync
            Return _pressedL OrElse _pressedR OrElse _pressedM OrElse _pressedX1 OrElse _pressedX2 _
                OrElse _releasedL OrElse _releasedR OrElse _releasedM OrElse _releasedX1 OrElse _releasedX2
        End SyncLock
    End Function

    Private Shared Function GetCursorPosScreen() As Point
        Dim p As Point
        If GetCursorPos(p) Then
            Return p
        Else
            Return Point.Empty
        End If
    End Function

    Private Shared Function ScreenToClientSafe(screenPos As Point, hwnd As IntPtr) As Point
        Dim p As Point = screenPos
        Call ScreenToClient(hwnd, p)
        Return p
    End Function

    Private Shared Function ClientToScreenSafe(clientPos As Point, hwnd As IntPtr) As Point
        Dim p As Point = clientPos
        Call ClientToScreen(hwnd, p)
        Return p
    End Function

    Private Function IsCursorOverTargetScreen(screenPos As Point) As Boolean
        Dim r As Rectangle = _target.RectangleToScreen(_target.ClientRectangle)
        Return r.Contains(screenPos)
    End Function

    Private Function IsCursorEffectivelyOverTarget(screenPos As Point) As Boolean
        ' 1) Grob: Punkt muss im Screen-Rect des Ziel-Controls liegen
        If Not IsCursorOverTargetScreen(screenPos) Then Return False

        ' 2) Fein: wirklich oberstes Fenster an der Position bestimmen
        Dim h As IntPtr = WindowFromPoint(screenPos)
        If h = IntPtr.Zero Then Return False

        Dim targetH As IntPtr = _target.Handle

        ' 2a) Ziel selbst oder sein Kind?
        If h = targetH OrElse IsChild(targetH, h) Then Return True

        ' 2b) Eigene Overlays „durchsichtig“ behandeln:
        '     – Gleiches Handle ODER ein Kind eines registrierten Overlay-Roots
        '     – zusätzlich: wenn WindowFromPoint auf ein Kind zeigt, betrachten wir dessen Root
        Dim hRoot As IntPtr = GetAncestor(h, GA_ROOT)
        For Each ov As IntPtr In _overlayRoots
            If h = ov OrElse hRoot = ov OrElse IsChild(ov, h) Then
                Return True
            End If
        Next

        ' 2c) Sonst ist es ein fremdes Fenster → verdeckt
        Return False
    End Function


    ''' <summary>
    ''' Fügt ein Overlay-Handle hinzu, das die Mausaktivität durchlässt.
    ''' Z.B. für Popup-Fenster o.ä., die zum Ziel-Control gehören.
    ''' (Tookbox, ContextMenu, ToolTip, eigene Form o.ä.)
    ''' </summary>
    Public Sub RegisterOverlay(ctrl As Control)
        If ctrl Is Nothing Then Return
        Dim h As IntPtr = ctrl.Handle
        If h <> IntPtr.Zero Then _overlayRoots.Add(h)
    End Sub

    Public Sub RegisterOverlay(hWnd As IntPtr)
        If hWnd <> IntPtr.Zero Then _overlayRoots.Add(hWnd)
    End Sub

    Public Sub UnregisterOverlay(hWnd As IntPtr)
        If hWnd <> IntPtr.Zero Then _overlayRoots.Remove(hWnd)
    End Sub

#End Region

#Region "Innerer MessageFilter (instanzgebunden)"
    ''' <summary>
    ''' Sammelt Wheel-Deltas nur, wenn der Cursor über dem Ziel-Control liegt (Abfrage via Delegate).
    ''' </summary>
    Private NotInheritable Class WheelFilter
        Implements IMessageFilter

        Private Const WM_MOUSEWHEEL As Integer = &H20A
        Private Const WM_MOUSEHWHEEL As Integer = &H20E

        Private _wheelSum As Integer
        Private _wheelHSum As Integer

        Friend Property IsCursorOverTargetFunc As Func(Of Point, Boolean)

        Friend Sub New(isOver As Func(Of Point, Boolean))
            IsCursorOverTargetFunc = isOver
        End Sub

        <DebuggerStepThrough>
        Public Function PreFilterMessage(ByRef m As Message) As Boolean Implements IMessageFilter.PreFilterMessage
            If m.Msg = WM_MOUSEWHEEL OrElse m.Msg = WM_MOUSEHWHEEL Then
                Dim wp As Long = ToInt64(m.WParam)
                Dim lp As Long = ToInt64(m.LParam)

                ' Wheel-Delta: HIWORD(wParam) als SIGNED SHORT
                Dim delta As Integer = HiWordI64(wp)

                ' Screen-Koordinaten: LOWORD/HIWORD(lParam) als SIGNED SHORT
                Dim x As Integer = LoWordI64(lp)
                Dim y As Integer = HiWordI64(lp)
                Dim scr As New Point(x, y)

                If IsCursorOverTargetFunc Is Nothing OrElse IsCursorOverTargetFunc(scr) Then
                    If m.Msg = WM_MOUSEWHEEL Then
                        Threading.Interlocked.Add(_wheelSum, delta)
                    Else
                        Threading.Interlocked.Add(_wheelHSum, delta)
                    End If
                End If
            End If
            Return False
        End Function


        Friend Function ConsumeWheel() As Integer
            Return Threading.Interlocked.Exchange(_wheelSum, 0)
        End Function

        Friend Function ConsumeWheelH() As Integer
            Return Threading.Interlocked.Exchange(_wheelHSum, 0)
        End Function
    End Class

    ' ── Word-Extraktion (signiert) ──────────────────────────────────────────────
    Private Shared Function LoWordI64(value As Long) As Short
        Dim w As Integer = CInt(value And &HFFFFL)
        If (w And &H8000) <> 0 Then w -= &H10000
        Return CShort(w)
    End Function

    Private Shared Function HiWordI64(value As Long) As Short
        Dim w As Integer = CInt((value >> 16) And &HFFFFL)
        If (w And &H8000) <> 0 Then w -= &H10000
        Return CShort(w)
    End Function

    Private Shared Function ToInt64(ptr As IntPtr) As Long
        Return ptr.ToInt64()
    End Function

#End Region

End Class

