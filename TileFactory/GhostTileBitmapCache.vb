Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

Imports System.Drawing
Imports MahjongGK.Contracts.GlobalEnum

Public Module GhostTileBitmapCache

    Private Const MAX_CACHE_COUNT As Integer = 100

    Private ReadOnly _cache As New Dictionary(Of GhostTileCacheKey, CacheEntry)()
    Private _useCounter As Long

    Private _tileColorsRef As TileColors
    Private _tileColorsSessionIdent As Object
    Private _steinSize As Size
    Private _steinBasisSize As Size
    Private _isInitialized As Boolean

    Public Function GetGhostBitmap(request As TileRequest,
                                   createBitmap As Func(Of TileRequest, Bitmap)) As Bitmap

        If request Is Nothing Then
            Throw New ArgumentNullException(NameOf(request))
        End If

        If createBitmap Is Nothing Then
            Throw New ArgumentNullException(NameOf(createBitmap))
        End If

        EnsureValidCacheFrame(request)

        Dim key As GhostTileCacheKey = GhostTileCacheKey.FromRequest(request)

        Dim entry As CacheEntry = Nothing

        If _cache.TryGetValue(key, entry) Then
            _useCounter += 1
            entry.LastUsed = _useCounter
            Return entry.Bitmap
        End If

        Dim bmp As Bitmap = createBitmap(request)

        If bmp Is Nothing Then
            Throw New InvalidOperationException("createBitmap hat Nothing zurückgegeben.")
        End If

        _useCounter += 1

        _cache.Add(key, New CacheEntry With {
            .Bitmap = bmp,
            .LastUsed = _useCounter
        })

        If _cache.Count > MAX_CACHE_COUNT Then
            RemoveLeastRecentlyUsed()
        End If

        Return bmp

    End Function

    Public Sub Clear()

        For Each entry As CacheEntry In _cache.Values
            If entry.Bitmap IsNot Nothing Then
                entry.Bitmap.Dispose()
            End If
        Next

        _cache.Clear()

        _isInitialized = False
        _tileColorsRef = Nothing
        _tileColorsSessionIdent = Nothing
        _steinSize = Size.Empty
        _steinBasisSize = Size.Empty
        _useCounter = 0

    End Sub

    Private Sub EnsureValidCacheFrame(request As TileRequest)

        Dim mustClear As Boolean = False

        If Not _isInitialized Then
            mustClear = True
        ElseIf Not Object.ReferenceEquals(_tileColorsRef, request.TileColors) Then
            mustClear = True
        ElseIf Not Object.Equals(_tileColorsSessionIdent, request.TileColors.SessionIdent) Then
            mustClear = True
        ElseIf _steinSize <> request.SteinSize Then
            mustClear = True
        ElseIf _steinBasisSize <> request.SteinBasisSize Then
            mustClear = True
        End If

        If mustClear Then
            Clear()

            _tileColorsRef = request.TileColors
            _tileColorsSessionIdent = request.TileColors.SessionIdent
            _steinSize = request.SteinSize
            _steinBasisSize = request.SteinBasisSize
            _isInitialized = True
        End If

    End Sub

    Private Sub RemoveLeastRecentlyUsed()

        Dim oldestKey As GhostTileCacheKey = Nothing
        Dim oldestUse As Long = Long.MaxValue
        Dim found As Boolean = False

        For Each kvp As KeyValuePair(Of GhostTileCacheKey, CacheEntry) In _cache

            If kvp.Value.LastUsed < oldestUse Then
                oldestUse = kvp.Value.LastUsed
                oldestKey = kvp.Key
                found = True
            End If

        Next

        If found Then
            Dim entry As CacheEntry = _cache(oldestKey)

            If entry.Bitmap IsNot Nothing Then
                entry.Bitmap.Dispose()
            End If

            _cache.Remove(oldestKey)
        End If

    End Sub

    Private NotInheritable Class CacheEntry
        Public Property Bitmap As Bitmap
        Public Property LastUsed As Long
    End Class

    Private Structure GhostTileCacheKey
        Implements IEquatable(Of GhostTileCacheKey)

        Private ReadOnly _aktRenderMode As AktRenderMode
        Private ReadOnly _steinTyp As SteinTyp
        Private ReadOnly _steinStatus As SteinStatus
        Private ReadOnly _steinFrameVersion As SteinFrameVersion
        Private ReadOnly _steinWidth As Integer
        Private ReadOnly _steinHeight As Integer

        Public Sub New(request As TileRequest)

            _aktRenderMode = request.AktRenderMode
            _steinTyp = request.SteinTyp
            _steinStatus = request.SteinStatus
            _steinFrameVersion = request.SteinFrameVersion
            _steinWidth = request.SteinWidth
            _steinHeight = request.SteinHeight

        End Sub

        Public Shared Function FromRequest(request As TileRequest) As GhostTileCacheKey
            Return New GhostTileCacheKey(request)
        End Function

        Public Overloads Function Equals(other As GhostTileCacheKey) As Boolean _
            Implements IEquatable(Of GhostTileCacheKey).Equals

            Return _aktRenderMode = other._aktRenderMode AndAlso
                   _steinTyp = other._steinTyp AndAlso
                   _steinStatus = other._steinStatus AndAlso
                   _steinFrameVersion = other._steinFrameVersion AndAlso
                   _steinWidth = other._steinWidth AndAlso
                   _steinHeight = other._steinHeight

        End Function

        Public Overrides Function Equals(obj As Object) As Boolean

            If Not TypeOf obj Is GhostTileCacheKey Then
                Return False
            End If

            Return Equals(DirectCast(obj, GhostTileCacheKey))

        End Function

        Public Overrides Function GetHashCode() As Integer

            Dim h As Integer = 17

            AddIntToHash(h, CInt(_aktRenderMode))
            AddIntToHash(h, CInt(_steinTyp))
            AddIntToHash(h, CInt(_steinStatus))
            AddIntToHash(h, CInt(_steinFrameVersion))
            AddIntToHash(h, _steinWidth)
            AddIntToHash(h, _steinHeight)

            Return h

        End Function

        Private Shared Sub AddIntToHash(ByRef h As Integer, value As Integer)

            Dim uh As UInteger = CUInt(h)
            Dim uv As UInteger = CUInt(value)

            uh = ((uh << 5) - uh) Xor uv   ' h * 31 Xor value

            h = CInt(uh And &H7FFFFFFFUI)

        End Sub

    End Structure

End Module
