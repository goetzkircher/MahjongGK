
Public Enum MouseStateChanged
    None
    MouseMovedWhileLeftMouseReleased
    AllOtherMouseEvents
End Enum

Public Class MouseWheelPoller

    Private _pendingSteps As Integer
    Private _remainder As Integer
    Private _ctrl As Control

    Public Sub Attach(ctrl As Control)

        If ctrl Is Nothing Then
            Throw New ArgumentNullException(NameOf(ctrl))
        End If

        AddHandler ctrl.MouseWheel, AddressOf OnMouseWheel

        _ctrl = ctrl
    End Sub

    Public Sub Detach(ctrl As Control)

        If ctrl Is Nothing Then
            Return
        End If

        RemoveHandler ctrl.MouseWheel, AddressOf OnMouseWheel

        _ctrl = Nothing

    End Sub

    Private Sub OnMouseWheel(sender As Object, e As MouseEventArgs)

        'Auch feine Mausräder / Touchpads können kleinere Delta-Werte liefern.
        _remainder += e.Delta

        While Math.Abs(_remainder) >= SystemInformation.MouseWheelScrollDelta

            If _remainder > 0 Then
                _pendingSteps += 1
                _remainder -= SystemInformation.MouseWheelScrollDelta
            Else
                _pendingSteps -= 1
                _remainder += SystemInformation.MouseWheelScrollDelta
            End If

        End While

        _mouseWheelSerial += 1

    End Sub

    Public Function PollWheelStep() As Integer

        If _pendingSteps > 0 Then
            _pendingSteps -= 1
            Return 1
        End If

        If _pendingSteps < 0 Then
            _pendingSteps += 1
            Return -1
        End If

        Return 0

    End Function

#Region "Schnelltest"
    '
    'Reiner Check, ob sich irgendwas geändert hat.
    'Der Schnelltest ist völlig unabhängig von dem anderem Code dieser Klasse,
    'ausgenommen daß OnMouseWheel  _mouseWheelSerial += 1 ausführt.
    '(Weshalb die Funktion hier angesiedelt ist)

    Private Structure MouseStateSnapshot

        Public Sub New(mousePosScreen As Point,
                   buttons As MouseButtons,
                   modifierKeys As Keys,
                   mouseWheelSerial As Integer)

            Me.MousePosScreen = mousePosScreen
            Me.Buttons = buttons
            Me.ModifierKeys = modifierKeys
            Me.MouseWheelSerial = mouseWheelSerial

        End Sub

        Public ReadOnly MousePosScreen As Point
        Public ReadOnly Buttons As MouseButtons
        Public ReadOnly ModifierKeys As Keys
        Public ReadOnly MouseWheelSerial As Integer

    End Structure

    Private _lastMouseState As MouseStateSnapshot
    Private _hasLastMouseState As Boolean = False
    Private _mouseWheelSerial As Integer
    '
    ''' <summary>
    ''' Gibt True zurück, wenn sich seit dem letztem Aufruf die MausPosition,
    ''' der Status der linken oder rechten Maustaste oder der Status 
    ''' der Strg- oder Alt-Taste oder das Scrollrad geändert hat.
    ''' </summary>
    Public Function ConsumeSchnelltestHasMouseStateChange(rxStageUsed As RectangleX, rxStock As RectangleX, rxStockScrollbar As RectangleX) As MouseStateChanged

        Dim aktState As New MouseStateSnapshot(
        Control.MousePosition,
        Control.MouseButtons,
        Control.ModifierKeys,
        _mouseWheelSerial)

        Dim found As Boolean
        If _ctrl IsNot Nothing Then
            Dim mpc As Point = _ctrl.PointToClient(aktState.MousePosScreen)

            If rxStageUsed?.Contains(mpc) Then
                found = True
            ElseIf rxStock?.Contains(mpc) Then
                found = True
            ElseIf rxStockscrollbar?.Contains(mpc) Then
                found = True
            End If
        End If

        If Not found Then
            Return MouseStateChanged.None
        End If

        If Not _hasLastMouseState Then
            _lastMouseState = aktState
            _hasLastMouseState = True
            Return MouseStateChanged.AllOtherMouseEvents
        End If

        'Tastenänderungen immer als relevantes Ereignis melden.
        If aktState.Buttons <> _lastMouseState.Buttons Then
            _lastMouseState = aktState
            Return MouseStateChanged.AllOtherMouseEvents
        End If

        'Modifier-Änderungen immer als relevantes Ereignis melden.
        If aktState.ModifierKeys <> _lastMouseState.ModifierKeys Then
            _lastMouseState = aktState
            Return MouseStateChanged.AllOtherMouseEvents
        End If

        'Mausradänderungen immer als relevantes Ereignis melden.
        If aktState.MouseWheelSerial <> _lastMouseState.MouseWheelSerial Then
            _lastMouseState = aktState
            Return MouseStateChanged.AllOtherMouseEvents
        End If

        'Reine Positionsänderung gesondert bewerten.
        If aktState.MousePosScreen <> _lastMouseState.MousePosScreen Then

            Dim leftMouseIsReleased As Boolean =
            (aktState.Buttons And MouseButtons.Left) = MouseButtons.None

            _lastMouseState = aktState

            If leftMouseIsReleased Then
                Return MouseStateChanged.MouseMovedWhileLeftMouseReleased
            End If

            Return MouseStateChanged.AllOtherMouseEvents

        End If

        Return MouseStateChanged.None

    End Function

#End Region

End Class