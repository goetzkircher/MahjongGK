Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

Imports System.Globalization
Imports System.Runtime.InteropServices

Namespace MahjongGKSymbolFactory
    '
    ''' <summary>
    ''' Win32-INI Wrapper (GetPrivateProfileString / WritePrivateProfileString).
    ''' </summary>
    Public NotInheritable Class IniFile
        Private ReadOnly _path As String

        <DllImport("kernel32", CharSet:=CharSet.Unicode, SetLastError:=True)>
        Private Shared Function GetPrivateProfileString(lpAppName As String, lpKeyName As String, lpDefault As String,
                                                       lpReturnedString As System.Text.StringBuilder, nSize As Integer,
                                                       lpFileName As String) As Integer
        End Function

        <DllImport("kernel32", CharSet:=CharSet.Unicode, SetLastError:=True)>
        Private Shared Function WritePrivateProfileString(lpAppName As String, lpKeyName As String, lpString As String,
                                                         lpFileName As String) As Integer
        End Function

        Public Sub New(path As String)
            _path = path
        End Sub

        Public Function ReadString(section As String, key As String, [default] As String) As String
            Dim sb As New System.Text.StringBuilder(1024)
            Dim n As Integer = GetPrivateProfileString(section, key, [default], sb, sb.Capacity, _path)
            Return If(n > 0, sb.ToString(), [default])
        End Function

        Public Function ReadSingle(section As String, key As String, [default] As Single) As Single
            Dim s As String = ReadString(section, key, [default].ToString(CultureInfo.InvariantCulture))
            Dim v As Single
            If Single.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, v) Then Return v
            Return [default]
        End Function

        Public Function ReadInt(section As String, key As String, [default] As Integer) As Integer
            Dim s As String = ReadString(section, key, [default].ToString(CultureInfo.InvariantCulture))
            Dim v As Integer
            If Integer.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, v) Then Return v
            Return [default]
        End Function

        Public Function ReadBool(section As String, key As String, [default] As Boolean) As Boolean
            Dim s As String = ReadString(section, key, If([default], "true", "false")).Trim().ToLowerInvariant()
            If s = "1" OrElse s = "true" OrElse s = "yes" OrElse s = "on" Then Return True
            If s = "0" OrElse s = "false" OrElse s = "no" OrElse s = "off" Then Return False
            Return [default]
        End Function

        Public Function ReadColor(section As String, key As String, [default] As Color) As Color
            ' ARGB als Hex: e.g. FFCCAA00  oder als R,G,B[,A]
            Dim s As String = ReadString(section, key, ColorToString([default]))
            Dim c As Color
            If TryParseColor(s, c) Then Return c
            Return [default]
        End Function

        Public Sub WriteString(section As String, key As String, value As String)
            WritePrivateProfileString(section, key, value, _path)
        End Sub

        Public Shared Function ColorToString(c As Color) As String
            Return $"{c.A:X2}{c.R:X2}{c.G:X2}{c.B:X2}" ' ARGB hex
        End Function

        Private Shared Function TryParseColor(s As String, ByRef c As Color) As Boolean
            s = s.Trim()
            If s.Length = 8 AndAlso System.Text.RegularExpressions.Regex.IsMatch(s, "^[0-9A-Fa-f]{8}$") Then
                Dim a As Byte = Convert.ToByte(s.Substring(0, 2), 16)
                Dim r As Byte = Convert.ToByte(s.Substring(2, 2), 16)
                Dim g As Byte = Convert.ToByte(s.Substring(4, 2), 16)
                Dim b As Byte = Convert.ToByte(s.Substring(6, 2), 16)
                c = Color.FromArgb(a, r, g, b) : Return True
            End If
            ' R,G,B[,A]
            Dim parts() As String = s.Split(","c)
            If parts.Length = 3 OrElse parts.Length = 4 Then
                Dim r, g, b, a As Integer
                If Integer.TryParse(parts(0), r) AndAlso Integer.TryParse(parts(1), g) AndAlso Integer.TryParse(parts(2), b) Then
                    a = If(parts.Length = 4 AndAlso Integer.TryParse(parts(3), a), a, 255)
                    c = Color.FromArgb(Math.Max(0, Math.Min(255, a)),
                                       Math.Max(0, Math.Min(255, r)),
                                       Math.Max(0, Math.Min(255, g)),
                                       Math.Max(0, Math.Min(255, b)))
                    Return True
                End If
            End If
            Return False
        End Function
    End Class
End Namespace

