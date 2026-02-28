Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On
Imports System.Threading

Public Class SteinInfoCollection
    Implements IEnumerable(Of SteinInfo)

    Private ReadOnly _items As List(Of SteinInfo)
    Private _renderVersion As Integer


    Public Sub New()
        _items = New List(Of SteinInfo)()
        _renderVersion = 0
    End Sub

    Public ReadOnly Property RenderVersion As Integer
        Get
            Return Threading.Volatile.Read(_renderVersion)
        End Get
    End Property

    Public ReadOnly Property Items As List(Of SteinInfo)
        Get
            Return _items
        End Get
    End Property


    Public ReadOnly Property Count As Integer
        Get
            Return _items.Count
        End Get
    End Property

    Default Public ReadOnly Property Item(index As Integer) As SteinInfo
        Get
            Return _items(index)
        End Get
    End Property

    Public Sub Add(item As SteinInfo)
        If item Is Nothing Then Throw New ArgumentNullException(NameOf(item))

        Wire(item)
        _items.Add(item)
        MarkDirty()
    End Sub

    Public Sub AddRange(items As IEnumerable(Of SteinInfo))
        If items Is Nothing Then Throw New ArgumentNullException(NameOf(items))

        For Each si As SteinInfo In items
            If si Is Nothing Then Continue For
            Wire(si)
            _items.Add(si)
        Next

        MarkDirty()
    End Sub

    Public Sub Clear()
        For Each si As SteinInfo In _items
            Unwire(si)
        Next
        _items.Clear()
        MarkDirty()
    End Sub

    Public Function Remove(item As SteinInfo) As Boolean
        If item Is Nothing Then Return False
        Dim ok As Boolean = _items.Remove(item)
        If ok Then
            Unwire(item)
            MarkDirty()
        End If
        Return ok
    End Function

    Public Sub RemoveAt(index As Integer)
        Dim si As SteinInfo = _items(index)
        _items.RemoveAt(index)
        Unwire(si)
        MarkDirty()
    End Sub

    Public Sub MarkDirty()
        Interlocked.Increment(_renderVersion)
    End Sub

    Private Sub Wire(item As SteinInfo)
        item.SetDirtyNotifier(AddressOf MarkDirty)
        ' SteinInfo kümmert sich intern um Postion3D.Changed (subscribe/unsubscribe),
        ' sobald Postion3D gesetzt ist.
        ' Optional: Wenn Postion3D bereits gesetzt ist, einmalig "dirty" zählt schon über Add().
    End Sub

    Private Sub Unwire(item As SteinInfo)
        item.SetDirtyNotifier(Nothing)
    End Sub

    Public Function GetEnumerator() As IEnumerator(Of SteinInfo) _
     Implements IEnumerable(Of SteinInfo).GetEnumerator

        Return _items.GetEnumerator()
    End Function

    Private Function IEnumerable_GetEnumerator() As IEnumerator _
        Implements IEnumerable.GetEnumerator

        Return GetEnumerator()
    End Function

End Class

