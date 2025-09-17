Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

Imports Microsoft.Win32

Namespace Theme
    ''' <summary>
    ''' Hier ist eine robuste, Strict-konforme Lösung zum Auslesen des Windows-Darkmodes über die Registry.
    ''' Sie unterscheidet zwischen App-Modus (entscheidend für WinForms) und System-Modus (Taskleiste, Systemoberfläche).
    ''' </summary>
    Public Module WindowsTheme

        'Anwendung:
        'If WindowsTheme.IsAppDarkMode() Then
        '' Darkmode aktivieren
        'Else
        '' Lightmode aktivieren
        'End If

        ' Quelle: HKCU\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize
        Private Const PERSONALIZE_KEY As String = "Software\Microsoft\Windows\CurrentVersion\Themes\Personalize"
        Private Const V_APPS_USE_LIGHT As String = "AppsUseLightTheme"
        Private Const V_SYSTEM_USE_LIGHT As String = "SystemUsesLightTheme"

        Public Enum ThemeMode
            Unknown = 0
            Light = 1
            Dark = 2
        End Enum

        ''' <summary>
        ''' True, wenn Windows für Apps (WinForms/WPF/UWP) den Darkmode bevorzugt.
        ''' Fallback (Key fehlt): Light = False.
        ''' </summary>
        Public Function IsAppDarkMode() As Boolean
            Return GetThemeModeApp() = ThemeMode.Dark
        End Function

        ''' <summary>
        ''' True, wenn Windows systemweit (Taskleiste, Shell) Darkmode bevorzugt.
        ''' Fallback (Key fehlt): Light = False.
        ''' </summary>
        Public Function IsSystemDarkMode() As Boolean
            Return GetThemeModeSystem() = ThemeMode.Dark
        End Function

        ''' <summary>
        ''' Liefert den App-Theme-Modus (entscheidend für Anwendungen).
        ''' </summary>
        Public Function GetThemeModeApp() As ThemeMode
            Return GetThemeModeInternal(V_APPS_USE_LIGHT)
        End Function

        ''' <summary>
        ''' Liefert den System-Theme-Modus (Taskleiste/Startmenü).
        ''' </summary>
        Public Function GetThemeModeSystem() As ThemeMode
            Return GetThemeModeInternal(V_SYSTEM_USE_LIGHT)
        End Function

        Private Function GetThemeModeInternal(valueName As String) As ThemeMode
            ' Interpretation: *UsesLightTheme = 1 → Light, = 0 → Dark
            Try
                Using k As RegistryKey = Registry.CurrentUser.OpenSubKey(PERSONALIZE_KEY, writable:=False)
                    Dim raw As Object = Nothing
                    If k IsNot Nothing Then raw = k.GetValue(valueName, 1) ' Standard: Light
                    Dim i As Integer = SafeToInt(raw, 1)
                    If i = 0 Then
                        Return ThemeMode.Dark
                    Else
                        Return ThemeMode.Light
                    End If
                End Using
            Catch
                ' Kein Zugriff / Alt-Windows → unbekannt, behandel wie Light
                Return ThemeMode.Unknown
            End Try
        End Function

        Private Function SafeToInt(value As Object, fallback As Integer) As Integer
            If value Is Nothing Then Return fallback
            If TypeOf value Is Integer Then Return CInt(value)
            If TypeOf value Is Long Then Return CInt(CLng(value))
            If TypeOf value Is UInteger Then Return CInt(CUInt(value))
            If TypeOf value Is String Then
                Dim tmp As Integer
                If Integer.TryParse(CStr(value), tmp) Then Return tmp
            End If
            Return fallback
        End Function
    End Module
End Namespace
