Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

Imports System.Drawing
Imports System.Globalization

Public Module TileColorConvert

    ''' <summary>
    ''' Wandelt eine Color in einen lesbaren ARGB-Hexstring um: AARRGGBB
    ''' Beispiel: FF3366CC
    ''' </summary>
    Public Function ColorToArgbHex(value As Color) As String
        Return value.A.ToString("X2", CultureInfo.InvariantCulture) &
               value.R.ToString("X2", CultureInfo.InvariantCulture) &
               value.G.ToString("X2", CultureInfo.InvariantCulture) &
               value.B.ToString("X2", CultureInfo.InvariantCulture)
    End Function

    ''' <summary>
    ''' Liest einen ARGB-Hexstring im Format AARRGGBB.
    ''' Erlaubt zusätzlich #AARRGGBB, undZchHAARRGGBB und 0xAARRGGBB.
    ''' </summary>
    Public Function ParseArgbHex(text As String) As Color

        If text Is Nothing Then
            Throw New ArgumentNullException(NameOf(text))
        End If

        Dim s As String = text.Trim()

        If s.StartsWith("#", StringComparison.Ordinal) Then
            s = s.Substring(1)
        End If

        If s.StartsWith("&H", StringComparison.OrdinalIgnoreCase) Then
            s = s.Substring(2)
        End If

        If s.StartsWith("0x", StringComparison.OrdinalIgnoreCase) Then
            s = s.Substring(2)
        End If

        If s.Length <> 8 Then
            Throw New FormatException("ARGB-Hex muss genau 8 Hex-Zeichen haben, z. B. FF3366CC.")
        End If

        Dim argb As Integer = Integer.Parse(s, NumberStyles.HexNumber, CultureInfo.InvariantCulture)

        Dim a As Integer = (argb >> 24) And &HFF
        Dim r As Integer = (argb >> 16) And &HFF
        Dim g As Integer = (argb >> 8) And &HFF
        Dim b As Integer = argb And &HFF

        Return Color.FromArgb(a, r, g, b)

    End Function

End Module
