Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

Imports System.Drawing
Imports System.Text
Imports MahjongGK.Contracts.GlobalEnum

'Kein NameSpace. Es wird der StammSpaceName TileTactory verwendet.
'
''' <summary>
''' Hier ist die Verwaltung angesiedelt.
''' </summary>
Friend Class TileFactoryManager

    Private Const CACHE_MODE_COUNT As Integer = 2   'Spiel / Edit
    Private Const CACHE_STATUS_COUNT As Integer = 9 'I01 .. I09
    Private Const CACHE_TYPE_COUNT As Integer = 43  'SteinTyp 0 .. 42
    Private Const CACHE_LENGTH As Integer = CACHE_MODE_COUNT * CACHE_STATUS_COUNT * CACHE_TYPE_COUNT

    'Grundsatzfrage: für wen gilt das ReadOnly?
    Private ReadOnly _tileStandardCache(CACHE_LENGTH - 1) As Bitmap
    Private ReadOnly _tileStandardCacheQueryCount(CACHE_LENGTH - 1) As Long

    Private ReadOnly _tileSmallCache(CacheIndex.UBound) As Bitmap
    Private ReadOnly _tileSmallCacheQueryCount(CacheIndex.UBound) As Long
    Private ReadOnly _tileSmallCacheLastRequestHash(CacheIndex.UBound) As Integer

    Private _spielSteinSize As Size = Size.Empty
    Private _editSteinSize As Size = Size.Empty

    Private _spielLayout As TileLayout = Nothing
    Private _editLayout As TileLayout = Nothing

    Sub New()

    End Sub

    ' Ich habe den Fehler eingekreist.
    '
    ''' <summary>
    ''' Liefert einen Mahjongstein.
    ''' Besitzer ist das Cache!
    ''' </summary>
    Public Function GetTile(request As TileRequest) As Bitmap

        'Note: Auskommentierte Stop-Bedingung 1. und 2. Stein links oben im Tester
        '1.Stein links oben im Tester
        'If request.SteinStatus = SteinStatus.I01Normal AndAlso
        '    request.SteinTyp = SteinTyp.Bambus6 AndAlso
        '    request.SteinTypVersion = SteinTypVersion.Normal AndAlso
        '    request.SteinFrameVersion = SteinFrameVersion.Standard Then
        '    Stop
        'End If

        '2.Stein links oben im Tester
        'If request.SteinStatus = SteinStatus.I01Normal AndAlso
        '    request.SteinTyp = SteinTyp.WindSüd AndAlso
        '    request.SteinTypVersion = SteinTypVersion.Winde AndAlso
        '    request.SteinFrameVersion = SteinFrameVersion.Standard Then
        '    Stop
        'End If
        EnsureModeSize(request) 'Cleard ggf alle Caches, macht in diesem Fall absolut nichts, da sich die Steingröße und die TileColors nicht geändert haben.

        If request.SteinFrameVersion = SteinFrameVersion.Standard Then

            'Der cacheIndex gibt auch die immer gleichen gültigen Werte der immer gleichen Steine zurück
            Dim cacheIndex As Integer = TileFactoryManager.GetIndexStandardCache(request)

            Dim bmp As Bitmap = _tileStandardCache(cacheIndex)

            If bmp IsNot Nothing Then
                _tileStandardCacheQueryCount(cacheIndex) += 1
                'Die Bitmap ist nicht angelegt, bei der Zuweisung zu einer Picturebox knallt es.
                'Wie kann ich das hier abfangen?
                If bmp.Width <= 0 Then Stop
                Return bmp '
            End If

            bmp = TileFactoryComposer.CreateTileBitmap(request) 'gibt immer eine Bitmap zurück, im Fehlerfall eine Rote.

            _tileStandardCache(cacheIndex) = bmp

            Return bmp
        Else
            Dim cacheIndex As CacheIndex = request.TileColors.GetIndexSmallCache

            If CacheNeedsClear(cacheIndex, request.SteinTyp, request.SteinStatus) Then
                If _tileSmallCache(cacheIndex) IsNot Nothing Then
                    _tileSmallCache(cacheIndex).Dispose()
                    _tileSmallCache(cacheIndex) = Nothing
                End If
            End If

            Dim bmp As Bitmap = _tileSmallCache(cacheIndex)

            If bmp IsNot Nothing Then
                _tileSmallCacheQueryCount(cacheIndex) += 1
                Return bmp
            End If

            bmp = TileFactoryComposer.CreateTileBitmap(request)

            _tileSmallCache(cacheIndex) = bmp

            Return bmp

        End If

    End Function

    '
    ''' <summary>
    ''' Liefert den aktuell gemerkten Layoutdatensatz für Spiel oder Editor.
    ''' </summary>
    Public Function GetAktLayout(aktRenderMode As AktRenderMode) As TileLayout

        Select Case aktRenderMode
            Case AktRenderMode.Spiel
                Return _spielLayout

            Case AktRenderMode.Edit
                Return _editLayout

            Case Else
                Return Nothing
        End Select

    End Function

    '
    ''' <summary>
    ''' Räumt alle intern gehaltenen Ressourcen auf.
    ''' </summary>
    Public Sub DisposeAll()

        ClearAllCaches()
        DisposeAllLichtkarten()
    End Sub

    '
    ''' <summary>
    ''' Liefert Debug-Informationen zur aktuellen Cache-Belegung.
    ''' Nur aktiv, wenn ein Debugger angehängt ist.
    ''' </summary>
    Public Function DebugInfoString() As String

        If Not Debugger.IsAttached Then
            Return String.Empty
        End If

        Dim sb As New StringBuilder()

        Dim totalUsed As Integer = CountUsedAll()
        Dim usedSpiel As Integer = CountUsedForMode(AktRenderMode.Spiel)
        Dim usedEdit As Integer = CountUsedForMode(AktRenderMode.Edit)

        Dim totalQueries As Long = CountQueriesAll()
        Dim queriesSpiel As Long = CountQueriesForMode(AktRenderMode.Spiel)
        Dim queriesEdit As Long = CountQueriesForMode(AktRenderMode.Edit)

        sb.AppendLine("TileFactoryManager.DebugInfo")
        sb.AppendLine(New String("="c, 78))
        sb.AppendLine($"CacheSlots total   : {CACHE_LENGTH,6}")
        sb.AppendLine($"CacheSlots belegt  : {totalUsed,6}")
        sb.AppendLine($"CacheSlots frei    : {CACHE_LENGTH - totalUsed,6}")
        sb.AppendLine($"Abfragen total     : {totalQueries,6}")
        sb.AppendLine($"Abfragen Spiel     : {queriesSpiel,6}")
        sb.AppendLine($"Abfragen Edit      : {queriesEdit,6}")
        sb.AppendLine($"CacheMisses        : {totalUsed,6}")
        sb.AppendLine($"CacheHits          : {totalQueries - totalUsed,6}")
        sb.AppendLine()

        sb.AppendLine("Layouts / Größen")
        sb.AppendLine(New String("-"c, 78))
        sb.AppendLine($"Spiel Size         : {FormatSize(_spielSteinSize)}")
        sb.AppendLine($"Spiel Layout       : {If(_spielLayout Is Nothing, "Nothing", "vorhanden")}")
        sb.AppendLine($"Edit  Size         : {FormatSize(_editSteinSize)}")
        sb.AppendLine($"Edit  Layout       : {If(_editLayout Is Nothing, "Nothing", "vorhanden")}")
        sb.AppendLine()

        sb.AppendLine("Belegung und Abfragen nach SteinStatus")
        sb.AppendLine(New String("-"c, 78))
        AppendStatusLine(sb, SteinStatus.I01Normal)
        AppendStatusLine(sb, SteinStatus.I02Selected)
        AppendStatusLine(sb, SteinStatus.I03Selectable)
        AppendStatusLine(sb, SteinStatus.I04Removable)
        AppendStatusLine(sb, SteinStatus.I05Locked)
        AppendStatusLine(sb, SteinStatus.I06WerkstückStein)
        AppendStatusLine(sb, SteinStatus.I07MissingSecond)
        AppendStatusLine(sb, SteinStatus.I08WerkstückEinfügeFehler)
        AppendStatusLine(sb, SteinStatus.I09WerkstückZufallsgrafik)

        Return sb.ToString()

    End Function

    Private Sub AppendStatusLine(sb As StringBuilder, status As SteinStatus)

        Dim usedSpiel As Integer = CountUsedForModeAndStatus(AktRenderMode.Spiel, status)
        Dim usedEdit As Integer = CountUsedForModeAndStatus(AktRenderMode.Edit, status)
        Dim usedTotal As Integer = usedSpiel + usedEdit

        Dim queriesSpiel As Long = CountQueriesForModeAndStatus(AktRenderMode.Spiel, status)
        Dim queriesEdit As Long = CountQueriesForModeAndStatus(AktRenderMode.Edit, status)
        Dim queriesTotal As Long = queriesSpiel + queriesEdit

        sb.AppendLine($"{status,-28} Slots:{usedTotal,4}   Qry:{queriesTotal,6}   Spiel:{queriesSpiel,6}   Edit:{queriesEdit,6}")

    End Sub

    Private Function CountQueriesAll() As Long

        Dim count As Long = 0

        For i As Integer = 0 To _tileStandardCacheQueryCount.GetUpperBound(0)
            count += _tileStandardCacheQueryCount(i)
        Next

        Return count

    End Function

    Private Function CountQueriesForMode(aktRenderMode As AktRenderMode) As Long

        Dim count As Long = 0

        For statusIndex As Integer = 0 To CACHE_STATUS_COUNT - 1
            For typeIndex As Integer = 0 To CACHE_TYPE_COUNT - 1
                Dim cacheIndex As Integer = ((GetModeIndex(aktRenderMode) * CACHE_STATUS_COUNT) + statusIndex) * CACHE_TYPE_COUNT + typeIndex
                count += _tileStandardCacheQueryCount(cacheIndex)
            Next
        Next

        Return count

    End Function

    Private Function CountQueriesForModeAndStatus(aktRenderMode As AktRenderMode,
                                              steinStatus As SteinStatus) As Long

        Dim count As Long = 0
        Dim modeIndex As Integer = GetModeIndex(aktRenderMode)
        Dim statusIndex As Integer = GetStatusIndex(steinStatus)

        For typeIndex As Integer = 0 To CACHE_TYPE_COUNT - 1
            Dim cacheIndex As Integer = ((modeIndex * CACHE_STATUS_COUNT) + statusIndex) * CACHE_TYPE_COUNT + typeIndex
            count += _tileStandardCacheQueryCount(cacheIndex)
        Next

        Return count

    End Function

    Private Function CountUsedAll() As Integer

        Dim count As Integer = 0

        For i As Integer = 0 To _tileStandardCache.GetUpperBound(0)
            If _tileStandardCache(i) IsNot Nothing Then
                count += 1
            End If
        Next

        Return count

    End Function

    Private Function CountUsedForMode(aktRenderMode As AktRenderMode) As Integer

        Dim count As Integer = 0

        For statusIndex As Integer = 0 To CACHE_STATUS_COUNT - 1
            For typeIndex As Integer = 0 To CACHE_TYPE_COUNT - 1
                Dim cacheIndex As Integer = ((GetModeIndex(aktRenderMode) * CACHE_STATUS_COUNT) + statusIndex) * CACHE_TYPE_COUNT + typeIndex
                If _tileStandardCache(cacheIndex) IsNot Nothing Then
                    count += 1
                End If
            Next
        Next

        Return count

    End Function

    Private Function CountUsedForModeAndStatus(aktRenderMode As AktRenderMode,
                                           steinStatus As SteinStatus) As Integer

        Dim count As Integer = 0
        Dim modeIndex As Integer = GetModeIndex(aktRenderMode)
        Dim statusIndex As Integer = GetStatusIndex(steinStatus)

        For typeIndex As Integer = 0 To CACHE_TYPE_COUNT - 1
            Dim cacheIndex As Integer = ((modeIndex * CACHE_STATUS_COUNT) + statusIndex) * CACHE_TYPE_COUNT + typeIndex
            If _tileStandardCache(cacheIndex) IsNot Nothing Then
                count += 1
            End If
        Next

        Return count

    End Function

    Private Shared Function FormatSize(value As Size) As String

        If value = Size.Empty Then
            Return "Empty"
        End If

        Return $"{value.Width} x {value.Height}"

    End Function

    Private _tileColors As TileColors = Nothing
    Private Sub EnsureModeSize(request As TileRequest)

        'Note: Auskommentierte Stop-Bedingung 1. und 2. Stein links oben im Tester
        ''1.Stein links oben im Tester
        'If request.SteinStatus = SteinStatus.I01Normal AndAlso
        '    request.SteinTyp = SteinTyp.Bambus6 AndAlso
        '    request.SteinTypVersion = SteinTypVersion.Normal AndAlso
        '    request.SteinFrameVersion = SteinFrameVersion.Standard Then
        '    Stop
        'End If

        '2.Stein links oben im Tester
        'If request.SteinStatus = SteinStatus.I01Normal AndAlso
        '    request.SteinTyp = SteinTyp.WindSüd AndAlso
        '    request.SteinTypVersion = SteinTypVersion.Winde AndAlso
        '    request.SteinFrameVersion = SteinFrameVersion.Standard Then
        '    Stop
        'End If
        With request

            'Note: Überprüfung Änderung von TileColor DebugPrint-Ausgabe
#Const WithMsg = False
#If WithMsg Then
            Dim msg As String
            Dim cacheResetDone As Boolean = False

            If _tileColors Is Nothing Then
                _tileColors = .TileColors
                ClearAllCaches()
                cacheResetDone = True
                msg = "_tileColors Is Nothing "

            ElseIf .TileColors Is _tileColors Then
                ' gleiche Instanz
                If .TileColors.SessionIdent <> _tileColors.SessionIdent Then
                    _tileColors = .TileColors
                    ClearAllCaches()
                    cacheResetDone = True
                    msg = ".TileColors.SessionIdent <> _tileColors.SessionIdent"
                Else
                    msg = ".TileColors gleiche Instanz"
                End If
            Else
                ' neue Instanz
                _tileColors = .TileColors
                ClearAllCaches()
                cacheResetDone = True
                msg = ".TileColors neue Instanz"
            End If

            'If request.SteinStatus = SteinStatus.I01Normal AndAlso
            '    request.SteinTyp = SteinTyp.WindSüd AndAlso
            '    request.SteinTypVersion = SteinTypVersion.Winde AndAlso
            '    request.SteinFrameVersion = SteinFrameVersion.Standard Then
            '    Debug.Print(msg)
            'End If
            Debug.Print(msg & " - " & Now.ToString)

#Else
            Dim cacheResetDone As Boolean = False

            If _tileColors Is Nothing Then
                _tileColors = .TileColors
                ClearAllCaches()
                cacheResetDone = True

            ElseIf .TileColors Is _tileColors Then
                ' gleiche Instanz
                If .TileColors.SessionIdent <> _tileColors.SessionIdent Then
                    _tileColors = .TileColors
                    ClearAllCaches()
                    cacheResetDone = True
                End If
            Else
                ' neue Instanz
                _tileColors = .TileColors
                ClearAllCaches()
                cacheResetDone = True
            End If
#End If

            Select Case .AktRenderMode
                Case AktRenderMode.Spiel

                    If _spielSteinSize <> Size.Empty AndAlso _spielSteinSize <> .SteinSize Then
                        If Not cacheResetDone Then
                            ClearAllCaches()
                        End If
                    End If

                    _spielSteinSize = .SteinSize

                    If _spielLayout Is Nothing Then
                        _spielLayout = TileLayoutFactory.Create(.SteinSize, .SteinBasisSize)
                    End If

                Case AktRenderMode.Edit

                    If _editSteinSize <> Size.Empty AndAlso _editSteinSize <> .SteinSize Then
                        If Not cacheResetDone Then
                            ClearAllCaches()
                        End If

                    End If

                    _editSteinSize = .SteinSize

                    If _editLayout Is Nothing Then
                        _editLayout = TileLayoutFactory.Create(.SteinSize, .SteinBasisSize)
                    End If

                Case Else 'AktRenderMode.None

                    Throw New InvalidOperationException("Programmierfehler: Unbekannter AktRenderMode.")
            End Select
        End With
    End Sub

    Private Sub ClearAllCaches()

        Dim i As Integer

        For i = 0 To _tileStandardCache.GetUpperBound(0)
            If _tileStandardCache(i) IsNot Nothing Then
                _tileStandardCache(i).Dispose()
            End If
            _tileStandardCache(i) = Nothing
            _tileStandardCacheQueryCount(i) = 0
        Next

        For i = 0 To _tileSmallCache.GetUpperBound(0)
            If _tileSmallCache(i) IsNot Nothing Then
                _tileSmallCache(i).Dispose()
            End If
            _tileSmallCache(i) = Nothing
            _tileSmallCacheQueryCount(i) = 0
        Next

        _spielSteinSize = Size.Empty
        _editSteinSize = Size.Empty

        _spielLayout = Nothing
        _editLayout = Nothing

    End Sub

    Private Shared Function GetIndexStandardCache(request As TileRequest) As Integer

        With request
            Dim modeIndex As Integer = GetModeIndex(.AktRenderMode)
            Dim statusIndex As Integer = GetStatusIndex(.SteinStatus)
            Dim typeIndex As Integer = CInt(.SteinTyp)

            Return ((modeIndex * CACHE_STATUS_COUNT) + statusIndex) * CACHE_TYPE_COUNT + typeIndex
        End With
    End Function

    Private Shared Function GetModeIndex(aktRenderMode As AktRenderMode) As Integer

        Select Case aktRenderMode
            Case AktRenderMode.Spiel
                Return 0

            Case AktRenderMode.Edit
                Return 1

            Case Else
                Throw New InvalidOperationException("Programmierfehler: Für diesen AktRenderMode gibt es keinen Cacheindex.")
        End Select

    End Function

    Private Shared Function GetStatusIndex(steinStatus As SteinStatus) As Integer

        Select Case steinStatus
            Case SteinStatus.I01Normal
                Return 0
            Case SteinStatus.I02Selected
                Return 1
            Case SteinStatus.I03Selectable
                Return 2
            Case SteinStatus.I04Removable
                Return 3
            Case SteinStatus.I05Locked
                Return 4
            Case SteinStatus.I06WerkstückStein
                Return 5
            Case SteinStatus.I07MissingSecond
                Return 6
            Case SteinStatus.I08WerkstückEinfügeFehler
                Return 7
            Case SteinStatus.I09WerkstückZufallsgrafik
                Return 8
            Case Else
                Throw New InvalidOperationException("Programmierfehler: Dieser SteinStatus hat keinen Cacheindex.")
        End Select

    End Function

    Private Function CacheNeedsClear(
            mainCacheIndex As Integer,
            steinTyp As SteinTyp,
            steinStatus As SteinStatus) As Boolean

        Dim typValue As Integer = CInt(steinTyp)
        Dim statusValue As Integer = CInt(steinStatus)

        Dim currentHash As Integer = typValue Or (statusValue << 6)

        If currentHash <> _tileSmallCacheLastRequestHash(mainCacheIndex) Then
            _tileSmallCacheLastRequestHash(mainCacheIndex) = currentHash
            Return True
        Else
            Return False
        End If

    End Function

End Class