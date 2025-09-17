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
'
Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

#Disable Warning IDE0079
#Disable Warning IDE1006


Imports System.Runtime.InteropServices
Namespace MjDebug
    Module DebugStep

        ' ##########################################################
        ' Modul DebugStep – Steuerung von Debug-Ausgaben per Tastatur
        '
        ' Steuerung:
        '   Strg LINKS  = Einzelschritt (ein WaitForStep() wird freigegeben)
        '   Shift LINKS = Dauerlauf einschalten (alle WaitForStep() kehren sofort zurück)
        '   Strg RECHTS = Dauerlauf ausschalten (zurück zum Einzelschritt)
        '
        ' Verwendung:
        '   1. Im Hauptformular (frmMain) im Load-Ereignis einmalig aufrufen:
        '
        '          Private Sub frmMain_Load(...) Handles MyBase.Load
        '              DebugStep.Attach(Me)
        '          End Sub
        '
        '   2. An beliebigen Stellen im Test-/Debugcode:
        '
        '          DebugStep.WaitForStep()
        '
        '      → Das Programm wartet hier, bis die Strg- oder Shift-Steuerung
        '        den Ablauf freigibt.
        '
        ' Hinweise:
        '   - KeyPreview wird im Attach automatisch auf True gesetzt,
        '     damit das Formular die Tastendrücke vor den Controls erhält.
        '   - Der Dauerlauf (Shift LINKS) bleibt aktiv, bis er mit Strg RECHTS
        '     wieder abgeschaltet wird.
        '
        ' ##########################################################

        ' --- WinAPI für Links/Rechts-Erkennung ---
        <DllImport("user32.dll")>
        Private Function GetKeyState(vKey As Integer) As Short
        End Function
        Private Const VK_LCONTROL As Integer = &HA2
        Private Const VK_RCONTROL As Integer = &HA3
        Private Const VK_LSHIFT As Integer = &HA0
        Private Const VK_RSHIFT As Integer = &HA1
        Private Function IsKeyDown(vk As Integer) As Boolean
            Return (GetKeyState(vk) And &H8000S) <> 0
        End Function

        Private _goNext As Boolean
        Private _attached As Boolean
        Private _pureLctrlArmed As Boolean
        Private _runToEnd As Boolean
        Private _lastCtrlSideLeft As Boolean

        Public ReadOnly Property IsRunToEnd As Boolean
            Get
                Return _runToEnd
            End Get
        End Property

        Public Sub Attach(host As Form)

            If _attached Then
                Return
            End If

            host.KeyPreview = True

            AddHandler host.KeyDown,
            Sub(s, e)
                Select Case e.KeyCode

                    Case Keys.ControlKey
                        ' WinForms liefert nur ControlKey → Seite per WinAPI feststellen
                        _lastCtrlSideLeft = Not IsKeyDown(VK_RCONTROL) AndAlso IsKeyDown(VK_LCONTROL)
                        If _lastCtrlSideLeft Then
                            ' Strg LINKS gedrückt → Kandidat für Einzelschritt (bei KeyUp)
                            _pureLctrlArmed = True
                        Else
                            ' Strg RECHTS gedrückt → Dauerlauf AUS (sofort)
                            _runToEnd = False
                            _pureLctrlArmed = False
                        End If
                        e.Handled = True
                        e.SuppressKeyPress = True

                    Case Keys.ShiftKey
                        ' Seite prüfen (hier kann WinForms meist L/R liefern, wir gehen aber sicher)
                        Dim leftShift As Boolean = Not IsKeyDown(VK_RSHIFT) AndAlso IsKeyDown(VK_LSHIFT)
                        If leftShift Then
                            _runToEnd = True      ' Dauerlauf EIN (permanent)
                            _goNext = True        ' evtl. wartende Stelle freigeben
                        End If
                        e.Handled = True
                        e.SuppressKeyPress = True

                    Case Else
                        _pureLctrlArmed = False
                End Select
            End Sub

            AddHandler host.KeyUp,
            Sub(s, e)
                Select Case e.KeyCode

                    Case Keys.ControlKey
                        ' Nur wenn es wirklich die LINKE Strg war und „pures“ Strg (ohne andere Tasten)
                        If _pureLctrlArmed AndAlso _lastCtrlSideLeft Then
                            _goNext = True
                        End If
                        _pureLctrlArmed = False
                        e.Handled = True
                        e.SuppressKeyPress = True

                    Case Keys.ShiftKey
                        ' Shift LINKS lässt Dauerlauf an (permanent) – nichts weiter tun
                        e.Handled = True
                        e.SuppressKeyPress = True

                    Case Else
                        _pureLctrlArmed = False
                End Select
            End Sub

            ' Bei Fokusverlust nur das Arming zurücksetzen, Dauerlauf bleibt bestehen
            AddHandler host.Deactivate, Sub() _pureLctrlArmed = False

            _attached = True
        End Sub

        ' Im Debug-Code aufrufen, um auf Eingabe zu warten
        Public Sub WaitForStep()

            If _runToEnd Then
                Return
            End If

            _goNext = False
            Do
                Application.DoEvents()
                Threading.Thread.Sleep(10)
            Loop Until _goNext
        End Sub

    End Module
End Namespace