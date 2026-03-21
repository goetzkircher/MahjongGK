'
' SPDX-License-Identifier: GPL-3.0-or-later
'###########################################################################
'#                                                                         #
'#   Copyright © 2025–2026 Götz Kircher <mahjonggk@t-online.de>            #
'#                                                                         #
'#                     MahjongGK  -  Mahjong Solitär                       #
'#                                                                         #
'#   This program is free software: you can redistribute it and/or modify  #
'#   it under the terms of the GNU General Public License as published by  #
'#   the Free Software Fundament, either version 3 of the License, or     #
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
'
Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

#Disable Warning IDE0079
#Disable Warning IDE1006


Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Text
Imports Microsoft.Win32.SafeHandles

Namespace MjDebug

    ''' <summary>
    ''' Umleitung der Debug-Ausgabe in ein Konsolenfenster 
    ''' Nur brauchbar, wenn man zwei Bildschirme hat.
    ''' Srollt nicht horizontal.
    ''' </summary>
    Module ConsoleHelper

        <DllImport("kernel32.dll", SetLastError:=True)>
        Private Function AllocConsole() As Boolean
        End Function

        <DllImport("kernel32.dll", SetLastError:=True)>
        Private Function FreeConsole() As Boolean
        End Function

        <DllImport("kernel32.dll", CharSet:=CharSet.Auto, SetLastError:=True)>
        Private Function CreateFile(
            lpFileName As String,
            dwDesiredAccess As UInteger,
            dwShareMode As UInteger,
            lpSecurityAttributes As IntPtr,
            dwCreationDisposition As UInteger,
            dwFlagsAndAttributes As UInteger,
            hTemplateFile As IntPtr) As IntPtr
        End Function

        Private Const GENERIC_READ As UInteger = &H80000000UI
        Private Const GENERIC_WRITE As UInteger = &H40000000UI
        Private Const FILE_SHARE_READ As UInteger = &H1
        Private Const FILE_SHARE_WRITE As UInteger = &H2
        Private Const OPEN_EXISTING As UInteger = 3
        Private Const FILE_ATTRIBUTE_NORMAL As UInteger = &H80

        Public Sub AttachConsole()
            If Not AllocConsole() Then
                Throw New System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error())
            End If

            ' StdOut auf Konsole umleiten
            Dim stdOutHandle As IntPtr = CreateFile("CONOUT$", GENERIC_WRITE, FILE_SHARE_WRITE Or FILE_SHARE_READ,
                                               IntPtr.Zero, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, IntPtr.Zero)
            If stdOutHandle = IntPtr.Zero OrElse stdOutHandle = New IntPtr(-1) Then
                Throw New System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error())
            End If

            Dim fsOut As New FileStream(New SafeFileHandle(stdOutHandle, True), FileAccess.Write)
            Dim swOut As New StreamWriter(fsOut, Encoding.UTF8) With {.AutoFlush = True}
            Console.SetOut(swOut)

            ' StdIn auf Konsole umleiten
            Dim stdInHandle As IntPtr = CreateFile("CONIN$", GENERIC_READ, FILE_SHARE_READ Or FILE_SHARE_WRITE,
                                              IntPtr.Zero, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, IntPtr.Zero)
            If stdInHandle = IntPtr.Zero OrElse stdInHandle = New IntPtr(-1) Then
                Throw New System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error())
            End If

            Dim fsIn As New FileStream(New SafeFileHandle(stdInHandle, True), FileAccess.Read)
            Dim srIn As New StreamReader(fsIn, Encoding.UTF8)
            Console.SetIn(srIn)

            ' StdErr auf Konsole umleiten (optional)
            Dim stdErrHandle As IntPtr = CreateFile("CONOUT$", GENERIC_WRITE, FILE_SHARE_WRITE Or FILE_SHARE_READ,
                                               IntPtr.Zero, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, IntPtr.Zero)
            If stdErrHandle = IntPtr.Zero OrElse stdErrHandle = New IntPtr(-1) Then
                Throw New System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error())
            End If

            Dim fsErr As New FileStream(New SafeFileHandle(stdErrHandle, True), FileAccess.Write)
            Dim swErr As New StreamWriter(fsErr, Encoding.UTF8) With {.AutoFlush = True}
            Console.SetError(swErr)

        End Sub

        Public Sub DetachConsole()
            FreeConsole()
        End Sub


    End Module

End Namespace



