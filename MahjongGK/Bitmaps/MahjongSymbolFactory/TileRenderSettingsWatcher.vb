
Imports System.IO

Namespace MahjongGKSymbolFactory
    '
    ''' <summary>
    ''' Beobachter für INI-Datei. Lädt Sektion neu und feuert Reloaded.
    ''' </summary>
    Public NotInheritable Class TileRenderSettingsWatcher
        Public Event Reloaded(newSettings As TileRenderSettings)

        Private ReadOnly _iniPath As String
        Private ReadOnly _section As String
        Private ReadOnly _fsw As FileSystemWatcher
        Private ReadOnly _debounceMs As Integer = 150
        Private _lastTick As Long

        Public Sub New(iniPath As String, section As String)
            _iniPath = iniPath
            _section = section

            Dim dir As String = Path.GetDirectoryName(_iniPath)
            Dim file As String = Path.GetFileName(_iniPath)

            _fsw = New FileSystemWatcher(dir, file) With {
            .NotifyFilter = NotifyFilters.LastWrite Or NotifyFilters.Size Or NotifyFilters.Attributes
        }
            AddHandler _fsw.Changed, AddressOf OnChanged
            AddHandler _fsw.Created, AddressOf OnChanged
            AddHandler _fsw.Renamed, AddressOf OnChanged
            _fsw.EnableRaisingEvents = True
        End Sub

        Private Sub OnChanged(sender As Object, e As FileSystemEventArgs)
            Dim nowTick As Long = Environment.TickCount  'Environment.TickCount64
            If nowTick - _lastTick < CULng(_debounceMs) Then Return
            _lastTick = nowTick

            Try
                Dim ini As New IniFile(_iniPath)
                Dim s As New TileRenderSettings()
                s.LoadFromIni(ini, _section)
                RaiseEvent Reloaded(s)
            Catch
                ' stillschweigend – oder Debug.Print
            End Try
        End Sub
    End Class
End Namespace