'Diesen MousePoller hast du mir geschrieben. Er funktioniert auch, zeigt aber Merkwürdigkeiten.
'Deshalb schreibe ihn mir neu mit folgenden Bedingungen:
'

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
''' Variante B (Drag-in korrekt):
''' - Hardware-Zustände (L/R + Cursorpos) werden global sauber verfolgt.
''' - "Changed" wird nur gemeldet, wenn der Cursor effektiv über dem Target liegt,
'''   PLUS: beim Eintritt ins Target wird ein bereits gedrückter Button als "Changed" gemeldet.
'''
''' Zusätzlich:
''' - MouseEntered() / MouseLeft() (nicht rückstellend)
''' </summary>
Public NotInheritable Class MouseInputPoller
    Implements IDisposable

#Region "Win32 interop"

    <DllImport("user32.dll", SetLastError:=False)>
    Private Shared Function GetAncestor(hWnd As IntPtr, gaFlags As UInteger) As IntPtr
    End Function

    <DllImport("user32.dll", SetLastError:=False)>
    Private Shared Function GetCursorPos(ByRef lpPoint As Point) As Boolean
    End Function

    <DllImport("user32.dll", SetLastError:=False)>
    Private Shared Function ScreenToClient(hWnd As IntPtr, ByRef lpPoint As Point) As Integer
    End Function

    <DllImport("user32.dll", SetLastError:=False)>
    Private Shared Function GetAsyncKeyState(vKey As Integer) As Short
    End Function

    <DllImport("user32.dll", SetLastError:=False)>
    Private Shared Function WindowFromPoint(pt As Point) As IntPtr
    End Function

    <DllImport("user32.dll", SetLastError:=False)>
    Private Shared Function IsChild(hWndParent As IntPtr, hWnd As IntPtr) As Boolean
    End Function

    Private Const GA_ROOT As UInteger = 2UI

    Private Const VK_LBUTTON As Integer = &H1
    Private Const VK_RBUTTON As Integer = &H2

#End Region

#Region "Felder"

    Private ReadOnly _target As Control
    Private ReadOnly _sync As New Object()

    Private ReadOnly _overlayRoots As New HashSet(Of IntPtr)()

    Private _disposed As Boolean

    ' Client-Position nur "geführt", wenn inside=True
    Private _curPos As Point
    Private _prevPos As Point
    Private _delta As Point

    ' Hardware-Zustand global (immer korrekt, unabhängig vom inside)
    Private _leftDown As Boolean
    Private _rightDown As Boolean

    ' Ergebnisflags (nicht konsumierend)
    Private _leftChanged As Boolean
    Private _rightChanged As Boolean
    Private _posChanged As Boolean

    Private _inside As Boolean
    Private _entered As Boolean
    Private _left As Boolean

#End Region

#Region "Konstruktion / Dispose"

    Public Sub New(targetControl As Control)
        If targetControl Is Nothing Then Throw New ArgumentNullException(NameOf(targetControl))
        _target = targetControl


        Dim scr As Point = GetMouseScreenPosSafe()
        Dim insideNow As Boolean = IsCursorEffectivelyOverTarget(scr)

        _inside = insideNow
        _entered = False
        _left = False

        If insideNow AndAlso _target.IsHandleCreated Then
            _curPos = ScreenToClientSafe(scr, _target.Handle)
        Else
            _curPos = Point.Empty
        End If

        _prevPos = _curPos
        _delta = Point.Empty
        _posChanged = False

        _leftDown = IsKeyDown(VK_LBUTTON)
        _rightDown = IsKeyDown(VK_RBUTTON)

        _leftChanged = False
        _rightChanged = False

    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        _disposed = True
        GC.SuppressFinalize(Me)
    End Sub

#End Region

#Region "API"

    ''' <summary>
    ''' Aktualisiert Zustand. True, wenn seit dem letzten Poll() etwas geändert hat (Variante B).
    ''' </summary>
    Public Function Poll() As Boolean

        If _disposed Then Return False


        Dim anyChange As Boolean = False

        ' --- Screenpos + inside ---
        Dim scr As Point = GetMouseScreenPosSafe()
        Dim insideNow As Boolean = IsCursorEffectivelyOverTarget(scr)

        Dim enteredNow As Boolean = (Not _inside) AndAlso insideNow
        Dim leftNow As Boolean = _inside AndAlso (Not insideNow)

        _entered = enteredNow
        _left = leftNow

        If _entered OrElse _left Then anyChange = True

        ' --- Buttons global lesen + globalen Wechsel bestimmen ---
        Dim lNow As Boolean = IsKeyDown(VK_LBUTTON)
        Dim rNow As Boolean = IsKeyDown(VK_RBUTTON)

        Dim leftChangedGlobal As Boolean = (lNow <> _leftDown)
        Dim rightChangedGlobal As Boolean = (rNow <> _rightDown)

        ' Hardwarezustand IMMER fortschreiben (global korrekt)
        _leftDown = lNow
        _rightDown = rNow

        ' --- Position nur bei inside auswerten ---
        If insideNow AndAlso _target.IsHandleCreated Then
            Dim newPos As Point = ScreenToClientSafe(scr, _target.Handle)

            _prevPos = _curPos
            _curPos = newPos

            Dim dx As Integer = _curPos.X - _prevPos.X
            Dim dy As Integer = _curPos.Y - _prevPos.Y
            _delta = New Point(dx, dy)

            _posChanged = (dx <> 0 OrElse dy <> 0)
            If _posChanged Then anyChange = True
        Else
            _prevPos = _curPos
            _delta = Point.Empty
            _posChanged = False
        End If

        'originaler Code:
        '' ' --- Changed-Meldung nur bei inside, PLUS Drag-in-Sync beim Eintritt ---
        ''If insideNow Then
        ''    _leftChanged = leftChangedGlobal OrElse (enteredNow AndAlso lNow)
        ''    _rightChanged = rightChangedGlobal OrElse (enteredNow AndAlso rNow)

        ''    If _leftChanged OrElse _rightChanged Then
        ''        anyChange = True
        ''    End If
        ''Else
        ''    _leftChanged = False
        ''    _rightChanged = False
        ''End If
        '
        'geänderter Code
        _leftChanged = leftChangedGlobal OrElse (enteredNow AndAlso lNow)
        _rightChanged = rightChangedGlobal OrElse (enteredNow AndAlso rNow)

        If _leftChanged OrElse _rightChanged Then
            anyChange = True
        End If
        'Ende Änderung


        _inside = insideNow

        Return anyChange

    End Function

    ' keine der folgenden Funktionen ist rückstellend!

    Public Function LeftMouseChanged() As Boolean

        Return _leftChanged

    End Function

    Public Function LeftMousePressed() As Boolean

        Return _leftDown

    End Function
    Public Function LeftMouseReleased() As Boolean

        Return Not _leftDown

    End Function

    Public Function RightMouseChanged() As Boolean

        Return _rightChanged

    End Function

    Public Function RightMousePressed() As Boolean

        Return _rightDown

    End Function
    Public Function RightMouseReleased() As Boolean

        Return Not _rightDown

    End Function

    Public Function MousePosChanged() As Boolean

        Return _posChanged

    End Function

    Public Function MousePos() As Point
        Return _curPos
    End Function
    '
    ''' <summary>
    ''' Gibt den Point aus den X-Wert und übergebenem y zurück
    ''' </summary>
    ''' <param name="y"></param>
    ''' <returns></returns>
    Public Function MousePosX(y As Integer) As Point
        Return New Point(_curPos.X, y)
    End Function
    '
    ''' <summary>
    ''' Gibt den Point aus dem Y-Wert und übergebenem X-Wert zurück.
    ''' </summary>
    ''' <param name="x"></param>
    ''' <returns></returns>
    Public Function MousePosY(x As Integer) As Point
        Return New Point(x, _curPos.Y)
    End Function

    Public Function MousePosDelta() As Point

        Return _delta

    End Function

    Public Function IsInsideTarget() As Boolean

        Return _inside

    End Function

    ''' <summary>True, wenn seit letztem Poll() ein Enter (outside-&gt;inside) stattgefunden hat.</summary>
    Public Function MouseEntered() As Boolean

        Return _entered

    End Function

    ''' <summary>True, wenn seit letztem Poll() ein Leave (inside-&gt;outside) stattgefunden hat.</summary>
    Public Function MouseLeft() As Boolean

        Return _left

    End Function

#End Region

#Region "Overlay API"

    Public Sub RegisterOverlay(ctrl As Control)
        If ctrl Is Nothing Then Return
        Dim h As IntPtr = ctrl.Handle
        If h <> IntPtr.Zero Then

            _overlayRoots.Add(h)

        End If
    End Sub

    Public Sub RegisterOverlay(hWnd As IntPtr)
        If hWnd = IntPtr.Zero Then Return

        _overlayRoots.Add(hWnd)

    End Sub

    Public Sub UnregisterOverlay(hWnd As IntPtr)
        If hWnd = IntPtr.Zero Then Return

        _overlayRoots.Remove(hWnd)

    End Sub

#End Region

#Region "HitTest / Helper"

    Private Function IsCursorOverTargetScreen(screenPos As Point) As Boolean
        If _target Is Nothing Then Return False
        If Not _target.IsHandleCreated Then Return False
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

        ' 2b) Overlay „durchsichtig“ behandeln: Root oder Kind davon
        Dim hRoot As IntPtr = GetAncestor(h, GA_ROOT)
        For Each ov As IntPtr In _overlayRoots
            If h = ov OrElse hRoot = ov OrElse IsChild(ov, h) Then
                Return True
            End If
        Next

        Return False
    End Function

    Private Shared Function GetMouseScreenPosSafe() As Point
        Dim p As Point
        If GetCursorPos(p) Then
            Return p
        End If
        Return Point.Empty
    End Function

    Private Shared Function ScreenToClientSafe(screenPos As Point, hwnd As IntPtr) As Point
        Dim p As Point = screenPos
        Call ScreenToClient(hwnd, p)
        Return p
    End Function

    Private Shared Function IsKeyDown(vKey As Integer) As Boolean
        Return (GetAsyncKeyState(vKey) And &H8000S) <> 0
    End Function

#End Region

End Class