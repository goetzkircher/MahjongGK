Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

'Verwendung:
' irgendwo nach Erzeugen der Form:
' Einzelnes Fenster:
' Dim _toolboxBinder As New OverlayHandleBinder(_mouse, Me.ToolboxForm)

' oder mehrere Fenster auf einmal:
' Dim _binderGroup As OverlayBinderGroup =
'    OverlayBinderGroup.Bind(_mouse, Me.ToolboxForm, Me.PropertiesDock, Me.ColorPicker)

''' <summary>
''' Pfad: MahjongGK/Spielfeld/UI/Mouse
''' 
''' Koppelt ein Overlay-(TopLevel-)Fenster (oder beliebiges Control) an den MouseInputPoller:
''' - Registriert das aktuelle Handle bei HandleCreated
''' - Deregistriert bei HandleDestroyed
''' - Berücksichtigt auch "schon vorhandenes" Handle beim Start
''' </summary>
Public NotInheritable Class MousePollerHandleBinder
    Implements IDisposable

    Private ReadOnly _poller As MouseInputPoller
    Private ReadOnly _ctrl As Control
    Private _lastHandle As IntPtr
    Private _disposed As Boolean

    Public Sub New(poller As MouseInputPoller, overlay As Control)
        If poller Is Nothing Then Throw New ArgumentNullException(NameOf(poller))
        If overlay Is Nothing Then Throw New ArgumentNullException(NameOf(overlay))

        _poller = poller
        _ctrl = overlay

        AddHandler _ctrl.HandleCreated, AddressOf OnHandleCreated
        AddHandler _ctrl.HandleDestroyed, AddressOf OnHandleDestroyed

        ' Falls das Handle bereits existiert (z. B. wenn das Fenster schon gezeigt wurde)
        If _ctrl.IsHandleCreated Then
            _lastHandle = _ctrl.Handle
            _poller.RegisterOverlay(_lastHandle)
        End If
    End Sub

    Private Sub OnHandleCreated(sender As Object, e As EventArgs)
        If _disposed Then Return
        _lastHandle = _ctrl.Handle
        If _lastHandle <> IntPtr.Zero Then
            _poller.RegisterOverlay(_lastHandle)
        End If
    End Sub

    Private Sub OnHandleDestroyed(sender As Object, e As EventArgs)
        If _disposed Then Return
        If _lastHandle <> IntPtr.Zero Then
            _poller.UnregisterOverlay(_lastHandle)
            _lastHandle = IntPtr.Zero
        End If
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        If _disposed Then Return
        _disposed = True

        RemoveHandler _ctrl.HandleCreated, AddressOf OnHandleCreated
        RemoveHandler _ctrl.HandleDestroyed, AddressOf OnHandleDestroyed

        If _lastHandle <> IntPtr.Zero Then
            _poller.UnregisterOverlay(_lastHandle)
            _lastHandle = IntPtr.Zero
        End If

        GC.SuppressFinalize(Me)
    End Sub
End Class
