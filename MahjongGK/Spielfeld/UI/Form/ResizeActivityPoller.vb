Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

''' <summary>
''' Pfad: MahjongGK/Spielfeld/UI/Form
''' 
''' Poll-basierter Detector für Größenänderungen (WinForms).
''' Erkennt ResizeStart / Resizing / ResizeEnd rein über ClientSize-Änderungen.
''' - Aufruf: zyklisch Poll() aus deiner Render-/Update-Schleife
''' - "Konsumierende" Getter: ConsumeResizeStarted/Ended, ConsumeSizeChanged
''' - ResizeEnd wird nach IdleMs ohne weitere Größenänderung gemeldet.
''' </summary>
Public NotInheritable Class ResizeActivityPoller

    Private ReadOnly _target As Control
    Private ReadOnly _idleMs As Integer

    Private _lastClientSize As Size
    Private _lastChangeStamp As Long ' TickCount (ms)
    Private _inResize As Boolean

    ' konsumierende Flags
    Private _started As Boolean
    Private _ended As Boolean
    Private _sizeChanged As Boolean
    Private _delta As Size

    ''' <param name="target">Form/UserControl, dessen ClientSize beobachtet wird</param>
    ''' <param name="idleMs">Zeit ohne weitere Änderung, nach der ResizeEnd feuert</param>
    Public Sub New(target As Control, Optional idleMs As Integer = 120)
        If target Is Nothing Then Throw New ArgumentNullException(NameOf(target))
        _target = target
        _idleMs = Math.Max(16, idleMs) ' mindestens ~1 Frame
        _lastClientSize = SafeClientSize()
        _lastChangeStamp = Environment.TickCount
        _inResize = False
        ClearEdgeFlags()
    End Sub

    ''' <summary>
    ''' Zyklisch aufrufen. Liefert True, wenn sich seit dem letzten Aufruf etwas Relevantes getan hat.
    ''' </summary>
    Public Function Poll() As Boolean
        If _target.IsDisposed OrElse Not _target.IsHandleCreated Then
            ' Handle weg – alles zurücksetzen
            ResetState()
            Return False
        End If

        Dim nowMs As Integer = Environment.TickCount
        Dim current As Size = SafeClientSize()

        If current <> _lastClientSize Then
            ' Größe hat sich geändert → „lebt“
            _delta = New Size(current.Width - _lastClientSize.Width, current.Height - _lastClientSize.Height)
            _sizeChanged = True
            _lastClientSize = current

            If Not _inResize Then
                _inResize = True
                _started = True
            End If

            _lastChangeStamp = nowMs
            Return True
        End If

        ' Keine weitere Änderung → evtl. Ende erreicht?
        If _inResize Then
            Dim idleFor As Integer = CInt(nowMs - _lastChangeStamp)
            If idleFor >= _idleMs Then
                _inResize = False
                _ended = True
                Return True
            End If
        End If

        Return False
    End Function

    ' ---------- Konsumierende Getter ----------

    ''' <summary>true genau einmal nach Erkennen des Resize-Beginns; reset nach Abfrage</summary>
    Public Function ConsumeResizeStarted() As Boolean
        Dim v As Boolean = _started
        _started = False
        Return v
    End Function

    ''' <summary>true solange eine Größenänderung aktiv ist (nicht konsumierend)</summary>
    Public ReadOnly Property IsResizing As Boolean
        Get
            Return _inResize
        End Get
    End Property

    ''' <summary>true genau einmal nach Erkennen des Resize-Endes; reset nach Abfrage</summary>
    Public Function ConsumeResizeEnded() As Boolean
        Dim v As Boolean = _ended
        _ended = False
        Return v
    End Function

    ''' <summary>true, wenn seit letztem Poll eine Größenänderung passiert ist; reset nach Abfrage</summary>
    Public Function ConsumeSizeChanged(ByRef newClientSize As Size, ByRef delta As Size) As Boolean
        If _sizeChanged Then
            newClientSize = _lastClientSize
            delta = _delta
            _sizeChanged = False
            _delta = Size.Empty
            Return True
        End If
        newClientSize = _lastClientSize
        delta = Size.Empty
        Return False
    End Function

    ''' <summary>Aktuelle ClientSize (zuletzt beobachtet)</summary>
    Public ReadOnly Property CurrentClientSize As Size
        Get
            Return _lastClientSize
        End Get
    End Property

    ' ---------- intern ----------

    Private Sub ResetState()
        _lastClientSize = Size.Empty
        _lastChangeStamp = Environment.TickCount
        _inResize = False
        ClearEdgeFlags()
        _delta = Size.Empty
    End Sub

    Private Sub ClearEdgeFlags()
        _started = False
        _ended = False
        _sizeChanged = False
    End Sub

    Private Function SafeClientSize() As Size
        ' Falls target (kurzzeitig) keine Handle-Infos liefert, auf aktuelle Properties gehen
        ' Bei Form: ClientSize; bei UserControl: ebenfalls ClientSize
        Return _target.ClientSize
    End Function

End Class

