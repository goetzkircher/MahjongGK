Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

Imports System.Drawing
Imports System.Drawing.Text
Imports System.IO
Imports System.Windows.Forms

Friend Module SymbolFontManager

    Private ReadOnly _privateFonts As New PrivateFontCollection()
    Private ReadOnly _syncRoot As New Object()

    Private _isInitialized As Boolean = False
    Private _notoFamily As FontFamily = Nothing

    Private Sub EnsureInitialized()

        If _isInitialized Then
            Exit Sub
        End If

        SyncLock _syncRoot

            If _isInitialized Then
                Exit Sub
            End If

            Dim fontPath As String =
                Path.Combine(Application.StartupPath, "Fonts", "NotoSansSymbols2-Regular.ttf")

            If File.Exists(fontPath) Then
                _privateFonts.AddFontFile(fontPath)

                If _privateFonts.Families.Length > 0 Then
                    _notoFamily = _privateFonts.Families(0)
                End If
            End If

            _isInitialized = True

        End SyncLock

    End Sub

    Public Function ResolveFontFamily(preferredFontFamilyName As String) As FontFamily

        EnsureInitialized()

        Dim preferredNormalized As String = NormalizeFontName(preferredFontFamilyName)

        '1.) Wunschfont bevorzugen
        If IsNotoName(preferredNormalized) Then
            If _notoFamily IsNot Nothing Then
                Return _notoFamily
            End If
        Else
            Dim installedPreferred As FontFamily = FindInstalledFontFamily(preferredFontFamilyName)
            If installedPreferred IsNot Nothing Then
                Return installedPreferred
            End If
        End If

        '2.) Fallbacks
        If _notoFamily IsNot Nothing Then
            Return _notoFamily
        End If

        Dim segoe As FontFamily = FindInstalledFontFamily("Segoe UI Symbol")
        If segoe IsNot Nothing Then
            Return segoe
        End If

        Dim arialUnicode As FontFamily = FindInstalledFontFamily("Arial Unicode MS")
        If arialUnicode IsNot Nothing Then
            Return arialUnicode
        End If

        Return FontFamily.GenericSansSerif

    End Function

    Public Function ResolveFontFamilyName(preferredFontFamilyName As String) As String

        Dim ff As FontFamily = ResolveFontFamily(preferredFontFamilyName)
        Return ff.Name

    End Function

    Public Function IsPrivateNotoLoaded() As Boolean

        EnsureInitialized()
        Return _notoFamily IsNot Nothing

    End Function

    Private Function FindInstalledFontFamily(fontFamilyName As String) As FontFamily

        If String.IsNullOrWhiteSpace(fontFamilyName) Then
            Return Nothing
        End If

        For Each ff As FontFamily In FontFamily.Families
            If String.Equals(ff.Name, fontFamilyName, StringComparison.OrdinalIgnoreCase) Then
                Return ff
            End If
        Next

        Return Nothing

    End Function

    Private Function NormalizeFontName(value As String) As String

        If value Is Nothing Then
            Return String.Empty
        End If

        Dim s As String = value.Trim().ToLowerInvariant()
        s = s.Replace(" ", String.Empty)
        s = s.Replace("-", String.Empty)

        Return s

    End Function

    Private Function IsNotoName(normalizedName As String) As Boolean

        Return normalizedName = "notosanssymbols2"

    End Function

End Module