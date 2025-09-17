Imports System.IO
Imports System.Runtime.InteropServices

Module StartupMagicNative
    <DllImport("kernel32", SetLastError:=True, CharSet:=CharSet.Unicode)>
    Private Function LoadLibrary(lpFileName As String) As IntPtr
    End Function

    Public Sub PreloadMagickNativeWithExpeption()

        Dim nativeFull As String = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                      "runtimes", "win-x64", "native", "Magick.Native-Q8-x64.dll")
        Dim h As IntPtr = LoadLibrary(nativeFull)
        If h = IntPtr.Zero Then
            Dim err As Integer = Marshal.GetLastWin32Error()
            Throw New System.ComponentModel.Win32Exception(err,
                "LoadLibrary auf Magick.Native-Q8-x64.dll fehlgeschlagen: " & nativeFull)
        End If
        ' Optional zusätzlich den Ordner in den Suchpfad hängen (schadet nicht):
        Dim oldPath As String = Environment.GetEnvironmentVariable("PATH")
        Environment.SetEnvironmentVariable("PATH",
            Path.GetDirectoryName(nativeFull) & ";" & oldPath)
    End Sub

    ''' <summary>
    ''' Versucht, die native Magick-DLL vorzubereiten.
    ''' Gibt Nothing zurück, wenn alles in Ordnung ist,
    ''' ansonsten eine rote Fehler-Bitmap mit Text.
    ''' </summary>
    Public Function PreloadMagickNativeWithBitmapErrorMessage() As Bitmap
        Try
            Dim nativeFull As String = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                          "runtimes", "win-x64", "native", "Magick.Native-Q8-x64.dll")

            If Not File.Exists(nativeFull) Then
                Return CreateErrorBitmap("Magick.Native fehlt!" & vbCrLf & nativeFull)
            End If

            Dim h As IntPtr = LoadLibrary(nativeFull)
            If h = IntPtr.Zero Then
                Dim err As Integer = Marshal.GetLastWin32Error()
                Return CreateErrorBitmap("LoadLibrary-Fehler " & err & vbCrLf & Path.GetFileName(nativeFull))
            End If

            ' optional zusätzlich PATH ergänzen
            Dim oldPath As String = Environment.GetEnvironmentVariable("PATH")
            Environment.SetEnvironmentVariable("PATH", Path.GetDirectoryName(nativeFull) & ";" & oldPath)

            Return Nothing ' alles ok
        Catch ex As Exception
            Return CreateErrorBitmap("PreloadMagickNativeWithExpeption-Exception:" & vbCrLf & ex.Message)
        End Try
    End Function

    Private Function CreateErrorBitmap(msg As String) As Bitmap
        Dim bmp As New Bitmap(400, 200)
        Using g As Graphics = Graphics.FromImage(bmp)
            g.Clear(Color.Red)
            Using f As New Font("Arial", 10, FontStyle.Bold)
                g.DrawString(msg, f, Brushes.White, New RectangleF(5, 5, bmp.Width - 10, bmp.Height - 10))
            End Using
        End Using
        Return bmp
    End Function
End Module
