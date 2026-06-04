Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

Public NotInheritable Class ResizeActivityPoller

    Private ReadOnly _target As Control
    Private ReadOnly _idleMs As Integer

    Private _lastClientSize As Size
    Private _lastChangeStamp As Long
    Private _hasValidSize As Boolean
    Private _inResize As Boolean

    Private _started As Boolean
    Private _ended As Boolean
    Private _sizeChanged As Boolean
    Private _delta As Size

    Public Sub New(target As Control, Optional idleMs As Integer = 120)

        If target Is Nothing Then
            Throw New ArgumentNullException(NameOf(target))
        End If

        _target = target
        _idleMs = Math.Max(16, idleMs)

        _lastClientSize = SafeClientSize()
        _lastChangeStamp = Environment.TickCount
        _hasValidSize = True
        _inResize = False

        ClearEdgeFlags()
        _delta = Size.Empty

    End Sub

    Public Function Poll() As Boolean

        If _target.IsDisposed OrElse Not _target.IsHandleCreated Then
            ResetState()
            Return False
        End If

        Dim nowMs As Long = Environment.TickCount
        Dim current As Size = SafeClientSize()

        If Not _hasValidSize Then
            _lastClientSize = current
            _lastChangeStamp = nowMs
            _hasValidSize = True
            _inResize = False
            ClearEdgeFlags()
            _delta = Size.Empty
            Return False
        End If

        If current <> _lastClientSize Then

            'Ein altes, noch nicht konsumiertes ResizeEnd ist ab jetzt ungültig.
            _ended = False
            'sicherheitshalber auch: 
            _started = False

            _delta = New Size(current.Width - _lastClientSize.Width,
                      current.Height - _lastClientSize.Height)

            _lastClientSize = current
            _lastChangeStamp = nowMs
            _sizeChanged = True

            If Not _inResize Then
                _inResize = True
                _started = True
            End If

            Return True

        End If

        If _inResize Then
            Dim idleFor As Long = nowMs - _lastChangeStamp

            If idleFor >= _idleMs Then
                _inResize = False
                _ended = True
                Return True
            End If
        End If

        Return False

    End Function

    Public Function ConsumeResizeStarted() As Boolean
        Dim v As Boolean = _started
        _started = False
        Return v
    End Function

    Public ReadOnly Property IsResizing As Boolean
        Get
            Return _inResize
        End Get
    End Property

    Public Function ConsumeResizeEnded() As Boolean
        Dim v As Boolean = _ended
        _ended = False
        Return v
    End Function

    Public Function ConsumeSizeChanged(ByRef newClientSize As Size,
                                       ByRef delta As Size) As Boolean

        newClientSize = _lastClientSize

        If _sizeChanged Then
            delta = _delta
            _sizeChanged = False
            _delta = Size.Empty
            Return True
        End If

        delta = Size.Empty
        Return False

    End Function

    Public ReadOnly Property CurrentClientSize As Size
        Get
            Return _lastClientSize
        End Get
    End Property

    Private Sub ResetState()

        _hasValidSize = False
        _lastClientSize = Size.Empty
        _lastChangeStamp = Environment.TickCount
        _inResize = False
        _delta = Size.Empty

        ClearEdgeFlags()

    End Sub

    Private Sub ClearEdgeFlags()

        _started = False
        _ended = False
        _sizeChanged = False

    End Sub

    Private Function SafeClientSize() As Size
        Return _target.ClientSize
    End Function

End Class