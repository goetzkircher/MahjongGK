Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

'
''' <summary>
''' Pfad: MahjongGK/Spielfeld/Rendering
'''
''' Puffert zusätzliche Renderausgaben, die nach dem normalen Spielfeld
''' bzw. nach dem Stock gerendert werden sollen, damit die Z-Order
''' unabhängig von der eigentlichen Steinreihenfolge korrekt bleibt.
'''
''' Arbeitsweise:
''' - AddRenderBitmapTopZOrder(...) hängt einen Eintrag hinten an.
''' - GetNextRenderBitmap(...) liefert die Einträge in Einfügereihenfolge.
''' - Sobald kein Eintrag mehr vorhanden ist, werden die Indizes zurückgesetzt.
''' - Vor dem ersten AddRenderBitmapTopZOrder des nächsten Durchlaufs werden alte Referenzen
'''   intern vollständig gelöscht.
''' </summary>
Public NotInheritable Class AirRenderBuffer

#Region "Felder"

    Private _renderRects() As Rectangle = Nothing
    Private _renderBitmaps() As Bitmap = Nothing

    Private _nextFreeIndex As Integer
    Private _nextReadIndex As Integer

    Private ReadOnly _growStep As Integer

    Private _needsClear As Boolean

#End Region

#Region "Konstruktor"

    '
    ''' <summary>
    ''' </summary>
    Public Sub New(bufferSize As Integer, growStep As Integer)

        ReDim _renderRects(bufferSize - 1)
        ReDim _renderBitmaps(bufferSize - 1)

        _growStep = growStep

        _nextFreeIndex = 0
        _nextReadIndex = 0

        _needsClear = False

    End Sub

#End Region

#Region "Status"

    '
    ''' <summary>
    ''' True, wenn mindestens eine Renderbitmap vorhanden ist.
    ''' </summary>
    Public ReadOnly Property RenderBitmapIsAvailable As Boolean
        Get
            Return _nextFreeIndex > 0
        End Get
    End Property

#End Region

#Region "Internes Löschen"

    '
    ''' <summary>
    ''' Löscht intern alle noch vorhandenen Referenzen.
    ''' Es werden keine Bitmaps disposed, sondern nur die Verweise entfernt.
    ''' </summary>
    Private Sub ClearInternal()

        Dim i As Integer

        For i = 0 To _renderBitmaps.GetUpperBound(0)
            _renderBitmaps(i) = Nothing
        Next

        _nextFreeIndex = 0
        _nextReadIndex = 0

        _needsClear = False

    End Sub

#End Region

#Region "Hinzufügen"

    '
    ''' <summary>
    ''' Hängt einen neuen Renderdatensatz hinten an.
    ''' Es werden immer gültige Werte übergeben.
    ''' </summary>
    Public Sub AddRenderBitmapTopZOrder(rect As Rectangle, bmp As Bitmap, Optional insertAtPreviousPos As Boolean = False)

        If _needsClear Then
            ClearInternal()
        End If

        If _nextFreeIndex > _renderRects.GetUpperBound(0) Then
            ReDim Preserve _renderRects(_renderRects.GetUpperBound(0) + _growStep)
            ReDim Preserve _renderBitmaps(_renderBitmaps.GetUpperBound(0) + _growStep)
        End If

        If insertAtPreviousPos AndAlso _nextFreeIndex > 0 Then
            _renderRects(_nextFreeIndex) = _renderRects(_nextFreeIndex - 1)
            _renderBitmaps(_nextFreeIndex) = _renderBitmaps(_nextFreeIndex - 1)
            _renderRects(_nextFreeIndex - 1) = rect
            _renderBitmaps(_nextFreeIndex - 1) = bmp
        Else
            _renderRects(_nextFreeIndex) = rect
            _renderBitmaps(_nextFreeIndex) = bmp
        End If

        _nextFreeIndex += 1

    End Sub

#End Region

#Region "Auslesen"

    '
    ''' <summary>
    ''' Liefert den nächsten Renderdatensatz.
    ''' Wenn kein Eintrag mehr vorhanden ist, werden die Indizes sofort
    ''' zurückgesetzt. Die eigentlichen Referenzen werden erst vor dem
    ''' nächsten AddRenderBitmapTopZOrder vollständig gelöscht.
    ''' </summary>
    Public Function GetNextRenderBitmap() As (found As Boolean, bmp As Bitmap, rect As Rectangle)

        If _nextReadIndex < _nextFreeIndex Then
            _nextReadIndex += 1
            Return (True, _renderBitmaps(_nextReadIndex - 1), _renderRects(_nextReadIndex - 1))
        End If

        _nextFreeIndex = 0
        _nextReadIndex = 0
        _needsClear = True

        Return (False, Nothing, Rectangle.Empty)

    End Function

#End Region

End Class
