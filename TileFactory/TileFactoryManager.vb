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

    Private Const CACHE_MODE_COUNT As Integer = 2
    Private Const CACHE_STATUS_COUNT As Integer = 9
    Private Const CACHE_SYMBOL_COUNT As Integer = 43
    Private Const CACHE_GHOST_COUNT As Integer = 3 'Transparent, LightOnly, LightTransparent
    Private Const CACHE_NORMAL_LENGTH As Integer = CACHE_MODE_COUNT * CACHE_STATUS_COUNT * CACHE_SYMBOL_COUNT
    Private Const CACHE_GHOST_LENGTH As Integer = CACHE_MODE_COUNT * CACHE_GHOST_COUNT * CACHE_SYMBOL_COUNT
    Private Const CACHE_LENGTH As Integer = CACHE_NORMAL_LENGTH + CACHE_GHOST_LENGTH

    'Grundsatzfrage: für wen gilt das ReadOnly?
    Private ReadOnly _tileCache(CACHE_LENGTH - 1) As Bitmap
    Private ReadOnly _tileCacheQueryCount(CACHE_LENGTH - 1) As Integer
    Private ReadOnly _tileCacheIsValide(CACHE_LENGTH - 1) As Boolean
    Private ReadOnly _tileGhostSteinStatus(CACHE_GHOST_LENGTH - 1) As SteinStatus

    Private _spielSteinSize As Size = Size.Empty
    Private _editSteinSize As Size = Size.Empty

    Private _spielLayout As TileLayout = Nothing
    Private _editLayout As TileLayout = Nothing

    Sub New()

    End Sub

    '
    ''' <summary>
    ''' Liefert einen Mahjongstein.
    ''' Besitzer ist das Cache!
    ''' </summary>
    Public Function GetTile(request As TileRequest) As Bitmap

        'Note: Auskommentierte Stop-Bedingung 1. und 2. Stein links oben im Tester
        '1.Stein links oben im Tester
        'If request.SteinStatus = SteinStatus.I01Normal AndAlso
        '    request.SteinSymbol = SteinSymbol.Bambus6 AndAlso
        '    request.SteinSymbolVersion = SteinSymbolVersion.Normal AndAlso
        '    request.SteinFrameVersion = SteinFrameVersion.Standard Then
        '    Stop
        'End If

        '2.Stein links oben im Tester
        'If request.SteinStatus = SteinStatus.I01Normal AndAlso
        '    request.SteinSymbol = SteinSymbol.WindSüd AndAlso
        '    request.SteinSymbolVersion = SteinSymbolVersion.Winde AndAlso
        '    request.SteinFrameVersion = SteinFrameVersion.Standard Then
        '    Stop
        'End If

        IfNecessaryClearArrCacheIsValid(request)

        ''Der cacheIndex gibt auch die immer gleichen gültigen Werte der immer gleichen Steine zurück
        Dim cacheIndex As Integer = TileFactoryManager.GetIndexCache(request)

        If request.SteinGhost <> SteinGhost.None Then
            'Für die Geister gibt es nur einen Cache pro SteinStatus.
            '(Was normal ausreichend ist)
            'Sollte der Status dennoch wechseln, wird hier der betreffende
            'Cache gelöscht.
            '
            Dim cacheGhostSteinStatusIdx As Integer = cacheIndex - CACHE_NORMAL_LENGTH
            If _tileGhostSteinStatus(cacheGhostSteinStatusIdx) <> request.SteinStatus Then
                _tileGhostSteinStatus(cacheGhostSteinStatusIdx) = request.SteinStatus
                _tileCacheIsValide(cacheIndex) = False
            End If

        End If

        If _tileCacheIsValide(cacheIndex) Then
            _tileCacheQueryCount(cacheIndex) += 1
            Return _tileCache(cacheIndex)
        End If

        _tileCache(cacheIndex)?.Dispose()
        _tileCache(cacheIndex) = Nothing
        _tileCacheQueryCount(cacheIndex) = 0
        'gibt immer eine Bitmap zurück, im Fehlerfall eine Rote.
        _tileCache(cacheIndex) = TileFactoryComposer.CreateTileBitmap(request)
        _tileCacheIsValide(cacheIndex) = True

        Return _tileCache(cacheIndex)

    End Function

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

        DisposeTileCacheAndClearOtherValues()
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

        For i As Integer = 0 To _tileCacheQueryCount.GetUpperBound(0)
            count += _tileCacheQueryCount(i)
        Next

        Return count

    End Function

    Private Function CountQueriesForMode(aktRenderMode As AktRenderMode) As Long

        Dim count As Long = 0

        For statusIndex As Integer = 0 To CACHE_STATUS_COUNT - 1
            For typeIndex As Integer = 0 To CACHE_SYMBOL_COUNT - 1
                Dim cacheIndex As Integer = ((GetModeIndex(aktRenderMode) * CACHE_STATUS_COUNT) + statusIndex) * CACHE_SYMBOL_COUNT + typeIndex
                count += _tileCacheQueryCount(cacheIndex)
            Next
        Next

        Return count

    End Function

    Private Function CountQueriesForModeAndStatus(aktRenderMode As AktRenderMode,
                                              steinStatus As SteinStatus) As Long

        Dim count As Long = 0
        Dim modeIndex As Integer = GetModeIndex(aktRenderMode)
        Dim statusIndex As Integer = GetStatusIndex(steinStatus)

        For typeIndex As Integer = 0 To CACHE_SYMBOL_COUNT - 1
            Dim cacheIndex As Integer = ((modeIndex * CACHE_STATUS_COUNT) + statusIndex) * CACHE_SYMBOL_COUNT + typeIndex
            count += _tileCacheQueryCount(cacheIndex)
        Next

        Return count

    End Function

    Private Function CountUsedAll() As Integer

        Dim count As Integer = 0

        For i As Integer = 0 To _tileCache.GetUpperBound(0)
            If _tileCache(i) IsNot Nothing Then
                count += 1
            End If
        Next

        Return count

    End Function

    Private Function CountUsedForMode(aktRenderMode As AktRenderMode) As Integer

        Dim count As Integer = 0

        For statusIndex As Integer = 0 To CACHE_STATUS_COUNT - 1
            For typeIndex As Integer = 0 To CACHE_SYMBOL_COUNT - 1
                Dim cacheIndex As Integer = ((GetModeIndex(aktRenderMode) * CACHE_STATUS_COUNT) + statusIndex) * CACHE_SYMBOL_COUNT + typeIndex
                If _tileCache(cacheIndex) IsNot Nothing Then
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

        For typeIndex As Integer = 0 To CACHE_SYMBOL_COUNT - 1
            Dim cacheIndex As Integer = ((modeIndex * CACHE_STATUS_COUNT) + statusIndex) * CACHE_SYMBOL_COUNT + typeIndex
            If _tileCache(cacheIndex) IsNot Nothing Then
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

    Private _tileColors_SessionIdent As String = String.Empty
    Private Sub IfNecessaryClearArrCacheIsValid(request As TileRequest)

        'Note: Auskommentierte Stop-Bedingung 1. und 2. Stein links oben im Tester
        ''1.Stein links oben im Tester
        'If request.SteinStatus = SteinStatus.I01Normal AndAlso
        '    request.SteinSymbol = SteinSymbol.Bambus6 AndAlso
        '    request.SteinSymbolVersion = SteinSymbolVersion.Normal AndAlso
        '    request.SteinFrameVersion = SteinFrameVersion.Standard Then
        '    Stop
        'End If

        '2.Stein links oben im Tester
        'If request.SteinStatus = SteinStatus.I01Normal AndAlso
        '    request.SteinSymbol = SteinSymbol.WindSüd AndAlso
        '    request.SteinSymbolVersion = SteinSymbolVersion.Winde AndAlso
        '    request.SteinFrameVersion = SteinFrameVersion.Standard Then
        '    Stop
        'End If

        ''VORSICHT FALLE:
        ' in folgenden wird auf eine Änderung der .TileColors.SessionIdent geprüft.
        ' Die unüberlegte Änderung einer SessionIdent, wie auch der Steingröße oder 
        ' anderer Werte TileColors, können zu sehr schwer zu findenden Fehlern führen,
        ' deren Ursache in der Änderung während des Renderns geschieht.
        ' (Im Tester innderhalb der Ausgabeschleife der 20 Steine.)
        ' Grund ist, daß die Änderung das Cache löscht, obwohl die Bitmaps noch geblittet
        ' werden bzw. noch Bestandteil einer PictureBox sind und Das Paint-Event noch darauf
        ' zugreift. Der Fehler tritt dann irgendwo in den Tiefen des Framework auf, ohne
        ' Bezug zum Auslöser.)
        ' Eine Änderung muss daher das vollständige Blitten betreffen, muss also bei der 
        ' ersten Bitmap oder vorher auftreten und nicht zwischendrin.
        ' Dann müssen stabile Verhältnisse herrschen bis zur Ausgabe des letzten Steines.
        ' NACHTRAG:
        ' Es gibt immer wieder Situationen, das daß Programm abstürzt, weil noch ein
        ' Zugriff auf das Cache erfolgt, obwohl noch keine neue Bitmap erzeugt wurde,
        ' das Cache aber gelöscht wurde.
        ' Die genaue Ursache zu finden ist wegen des Polling und des Zusammenhangs mit
        ' Mausbewegung und Größenänderung des Formulars nicht trivial.
        ' Deshalb habe ich umgestellt auf ein anderes Verfahren des Disposen.

        With request

#Const WithMsg = False

#If WithMsg Then
            Dim msg As String
            Dim cacheResetDone As Boolean = False

            If _tileColors_SessionIdent = String.Empty Then
                _tileColors_SessionIdent = String.Copy(.TileColors.SessionIdent)
                SetCacheInvalide()
                cacheResetDone = True
                msg = ".TileColors neue Instanz"

            ElseIf _tileColors_SessionIdent <> .TileColors.SessionIdent Then
                ' neue Instanz
                _tileColors_SessionIdent = String.Copy(.TileColors.SessionIdent)
                SetCacheInvalide()
                cacheResetDone = True
                msg = ".TileColors neue Instanz ==> lösche Cache <=="
            Else
                'gleiche Instanz
                cacheResetDone = False
                msg = ".TileColors gleiche Instanz"
            End If

            Debug.Print(msg & " - " & Now.ToString)

#Else
            Dim cacheResetDone As Boolean = False

            If _tileColors_SessionIdent = String.Empty Then
                _tileColors_SessionIdent = String.Copy(.TileColors.SessionIdent)
                SetCacheInvalide()
                cacheResetDone = True

            ElseIf _tileColors_SessionIdent <> .TileColors.SessionIdent Then
                ' neue Instanz
                _tileColors_SessionIdent = String.Copy(.TileColors.SessionIdent)
                SetCacheInvalide()
                cacheResetDone = True
            Else
                'gleiche Instanz
                cacheResetDone = False
            End If
#End If

            Select Case .AktRenderMode
                Case AktRenderMode.Spiel
                    'Ein Wechel zwischen AktRenderMode.Spiel und AktRenderMode.Edit braucht
                    'nicht festgestellt werden, da sich die Steingrößen immer gleichzeitig ändern.
                    If _spielSteinSize <> Size.Empty AndAlso _spielSteinSize <> .SteinSize Then
                        If Not cacheResetDone Then
                            SetCacheInvalide()
                        End If
                    End If

                    _spielSteinSize = .SteinSize

                    If _spielLayout Is Nothing Then
                        'Wird in ClearAllCaches zurückgesetzt.
                        _spielLayout = TileLayoutFactory.Create(.SteinSize, .SteinBasisSize)
                    End If

                Case AktRenderMode.Edit

                    If _editSteinSize <> Size.Empty AndAlso _editSteinSize <> .SteinSize Then
                        If Not cacheResetDone Then
                            SetCacheInvalide()
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

    Private Sub DisposeTileCacheAndClearOtherValues()

        For idx As Integer = 0 To _tileCache.GetUpperBound(0)
            _tileCache(idx)?.Dispose()
            _tileCache(idx) = Nothing
            _tileCacheQueryCount(idx) = 0
            _tileCacheIsValide(idx) = False
        Next

        _spielSteinSize = Size.Empty
        _editSteinSize = Size.Empty

        _spielLayout = Nothing
        _editLayout = Nothing

    End Sub

    Private Sub SetCacheInvalide()

        For idx As Integer = 0 To _tileCache.GetUpperBound(0)
            _tileCacheIsValide(idx) = False
        Next

        _spielSteinSize = Size.Empty
        _editSteinSize = Size.Empty

        _spielLayout = Nothing
        _editLayout = Nothing

        For idx As Integer = 0 To _tileGhostSteinStatus.GetUpperBound(0)
            _tileGhostSteinStatus(idx) = CType(-1, SteinStatus)
        Next

    End Sub

    Private Shared Function GetIndexCache(request As TileRequest) As Integer

        With request
            Dim modeIndex As Integer = GetModeIndex(.AktRenderMode)
            Dim symbolIndex As Integer = CInt(.SteinSymbol)

            If .SteinGhost = SteinGhost.None Then
                Dim statusIndex As Integer = GetStatusIndex(.SteinStatus)

                Return ((modeIndex * CACHE_STATUS_COUNT) + statusIndex) *
                   CACHE_SYMBOL_COUNT + symbolIndex
            Else
                Dim ghostIndex As Integer = CInt(.SteinGhost) - 1
                'Transparent=0, LightOnly=1, LightTransparent=2

                Return CACHE_NORMAL_LENGTH +
                   ((modeIndex * CACHE_GHOST_COUNT) + ghostIndex) *
                   CACHE_SYMBOL_COUNT + symbolIndex
            End If
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

End Class