'
' SPDX-License-Identifier: GPL-3.0-or-later
'###########################################################################
'#                                                                         #
'#   Copyright © 2025–2026 Götz Kircher <MahjongGK@t-online.de>            #
'#                                                                         #
'#                     MahjongGK  -  Mahjong Solitär                       #
'#                                                                         #
'#   This program is free software: you can redistribute it and/or modify  #
'#   it under the terms of the GNU General Public License as published by  #
'#   the Free Software Foundation, either version 3 of the License, or     #
'#   at your option any later version.                                     #
'#                                                                         #
'#   This program is distributed in the hope that it will be useful,       #
'#   but WITHOUT ANY WARRANTY; without even the implied warranty of        #
'#   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the          #
'#   GNU General Public License for more details.                          #
'#   https://www.gnu.org/licenses/gpl-3.0.html                             #
'#                                                                         #
'###########################################################################
'
Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On
'
#Disable Warning IDE0079
#Disable Warning IDE1006



Public Class frmWebPTest
    Inherits Form

    Public Class frmToolBox
        Inherits Form

        Public Sub New()
            Me.Text = "Toolbox – DPI-Test"
            Me.Size = New Size(400, 300)
            Debug.WriteLine($"Start-DPI: {Me.DeviceDpi}")
        End Sub

        ' Variante A: Event
        Private Sub frmToolBox_DpiChanged(sender As Object, e As DpiChangedEventArgs) Handles Me.DpiChanged
            Debug.WriteLine($"DPI: {e.DeviceDpiOld} -> {e.DeviceDpiNew}")
            ' Optional: Bounds = e.SuggestedRectangle
        End Sub

        ' Variante B: Win32-Fallback
        Protected Overrides Sub WndProc(ByRef m As Message)
            Const WM_DPICHANGED As Integer = &H2E0
            If m.Msg = WM_DPICHANGED Then
                Dim newDpi As Integer = CInt(m.WParam.ToInt32() And &HFFFF)
                Debug.WriteLine($"[WM_DPICHANGED] Neue DPI = {newDpi}")
            End If
            MyBase.WndProc(m)
        End Sub
    End Class

    ''Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    ''    Try
    ''        Dim path As String = "C:\Users\goetz\Documents\Visual Studio\MahjongGK\Eigene Ressourcen Quelle\Andere\Satz1\HGrdSplFld\SplFldUGrd02Light.webp"

    ''        Using img As New MagickImage(path)
    ''            ' Konvertieren in System.Drawing.Bitmap
    ''            Using ms As New IO.MemoryStream()
    ''                img.Write(ms, MagickFormat.Bmp) ' temporär als BMP ins MemoryStream
    ''                ms.Position = 0
    ''                Dim bmp As New Bitmap(ms)

    ''                ' Setze als Hintergrund
    ''                Me.BackgroundImage = bmp
    ''                Me.BackgroundImageLayout = ImageLayout.Stretch
    ''            End Using
    ''        End Using

    ''    Catch ex As Exception
    ''        MessageBox.Show("Fehler beim Laden des WebP: " & ex.Message)
    ''    End Try
    ''End Sub

    ''Private Sub InitializeComponent()
    ''    Me.SuspendLayout()
    ''    '
    ''    'frmWebPTest
    ''    '
    ''    Me.ClientSize = New System.Drawing.Size(660, 500)
    ''    Me.Name = "frmWebPTest"
    ''    Me.ResumeLayout(False)

    ''End Sub



    ''<DllImport("kernel32", SetLastError:=True, CharSet:=CharSet.Unicode)>
    ''Private Shared Function LoadLibrary(lpFileName As String) As IntPtr
    ''End Function
    ''<DllImport("kernel32")>
    ''Private Shared Function FreeLibrary(h As IntPtr) As Boolean
    ''End Function

    ''Private Sub TestNativeLoad()
    ''    Dim p As String = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "runtimes\win-x64\native\Magick.Native-Q8-x64.dll")
    ''    Dim h As IntPtr = LoadLibrary(p)
    ''    If h = IntPtr.Zero Then
    ''        Dim err As Integer = Marshal.GetLastWin32Error()
    ''        MessageBox.Show("LoadLibrary failed, Win32Error=" & err)
    ''    Else
    ''        FreeLibrary(h)
    ''        MessageBox.Show("LoadLibrary OK")
    ''    End If
    ''End Sub
End Class

