Public Class MouseWheelPoller

    Private _pendingSteps As Integer
    Private _remainder As Integer

    Public Sub Attach(ctrl As Control)

        If ctrl Is Nothing Then
            Throw New ArgumentNullException(NameOf(ctrl))
        End If

        AddHandler ctrl.MouseWheel, AddressOf OnMouseWheel

    End Sub

    Public Sub Detach(ctrl As Control)

        If ctrl Is Nothing Then
            Return
        End If

        RemoveHandler ctrl.MouseWheel, AddressOf OnMouseWheel

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
    Public Function ConsumeSchnelltestHasMouseStateChange() As Boolean

        Dim aktState As New MouseStateSnapshot(
        Control.MousePosition,
        Control.MouseButtons,
        Control.ModifierKeys,
        _mouseWheelSerial)

        If Not _hasLastMouseState Then
            _lastMouseState = aktState
            _hasLastMouseState = True
            Return True
        End If

        If aktState.MousePosScreen <> _lastMouseState.MousePosScreen Then
            _lastMouseState = aktState
            Return True
        End If

        If aktState.Buttons <> _lastMouseState.Buttons Then
            _lastMouseState = aktState
            Return True
        End If

        If aktState.ModifierKeys <> _lastMouseState.ModifierKeys Then
            _lastMouseState = aktState
            Return True
        End If

        If aktState.MouseWheelSerial <> _lastMouseState.MouseWheelSerial Then
            _lastMouseState = aktState
            Return True
        End If

        Return False

    End Function

#End Region

End Class