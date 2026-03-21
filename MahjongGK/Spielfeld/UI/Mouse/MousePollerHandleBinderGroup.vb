
''' <summary>
''' Pfad: MahjongGK/Spielfeld/UI/Mouse
''' 
''' Verwaltet mehrere Overlay-Bindungen in einem Rutsch (Bequemlichkeit).
''' </summary>
Public NotInheritable Class MousePollerHandleBinderGroup
    Implements IDisposable

    Private ReadOnly _items As New List(Of MousePollerHandleBinder)()
    Private _disposed As Boolean

    Private Sub New()
    End Sub

    ''' <summary>
    ''' Erstellt eine Gruppe und bindet alle angegebenen Controls an den Poller.
    ''' </summary>
    Public Shared Function Bind(poller As MouseInputPoller, ParamArray overlays() As Control) As MousePollerHandleBinderGroup
        If poller Is Nothing Then Throw New ArgumentNullException(NameOf(poller))
        Dim g As New MousePollerHandleBinderGroup()
        For Each c As Control In overlays
            If c IsNot Nothing Then
                g._items.Add(New MousePollerHandleBinder(poller, c))
            End If
        Next
        Return g
    End Function

    Public Sub Dispose() Implements IDisposable.Dispose
        If _disposed Then Return
        _disposed = True
        For Each it As MousePollerHandleBinder In _items
            it.Dispose()
        Next
        _items.Clear()
        GC.SuppressFinalize(Me)
    End Sub
End Class
