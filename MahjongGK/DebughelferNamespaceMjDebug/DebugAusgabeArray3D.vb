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


Namespace MjDebug

    ''' <summary>
    ''' Hier werden 3 verschiedene Methoden genutzt um Ausgaben sichtbar zu machen.
    ''' In einer TextBox, einer RtfBox und in einem gesondertem Fenster, der Console.
    ''' Gut brauchbar sind alle drei nur auf einem Computer mit zwei oder mehr Bildschirmen.
    ''' Die Methoden können als Vorlage für weitere Nutzung dienen.
    ''' Der Aufruf erfolgt von beliebiger Stelle im Programm über DebugHilfen.Print3DArrayToRtfOutputForm(...),
    ''' die beiden anderen Methoden analog.
    ''' </summary>
    Public Module DebugAusgabeArray3D

        Private Const SPALTENBREITE As Integer = 6

        ''rechtsbündig
        'Private Function Col(val As integer) As String
        '    Return String.Format("{0," & SPALTENBREITE & "}", val)
        'End Function
        ''rechtsbündig
        'Private Function ColBracket(val As Integer) As String
        '    Return String.Format("[{0," & (SPALTENBREITE - 2) & "}]", val)
        'End Function
        '
        'linksbündig
        Private Function Col(val As Integer) As String
            Return String.Format("{0,-" & SPALTENBREITE & "}", val)
        End Function
        '
        'linksbündig
        Private Function ColBracket(val As Integer) As String
            Return String.Format("[{0,-" & (SPALTENBREITE - 2) & "}]", val)
        End Function

        '=== RTF-Ausgabe ===
        Private dbgRtfForm As MjDebug.DebugRtfOutputForm = Nothing

        Public Sub Print3DArrayToRtfOutputForm(arr As Integer(,,), Optional highlightList As List(Of (Integer, Integer, Integer)) = Nothing)

            If Not Debugger.IsAttached Then Return

            If dbgRtfForm Is Nothing OrElse dbgRtfForm.IsDisposed Then
                dbgRtfForm = New DebugRtfOutputForm()
                dbgRtfForm.Show()
            Else
                dbgRtfForm.ClearText()
                dbgRtfForm.BringToFront()
                dbgRtfForm.Focus()
            End If

            For z As Integer = 0 To arr.GetLength(2) - 1
                dbgRtfForm.AppendColoredText($"=== Ebene Z = {z} ===" & Environment.NewLine, Color.LightGreen)

                dbgRtfForm.AppendColoredText("Y\X".PadLeft(6), Color.White)
                For x As Integer = 0 To arr.GetLength(0) - 1
                    dbgRtfForm.AppendColoredText(Col(x), Color.White)
                Next
                dbgRtfForm.AppendColoredText(Environment.NewLine, Color.White)
                dbgRtfForm.AppendColoredText(New String("-"c, 6 + arr.GetLength(0) * SPALTENBREITE) & Environment.NewLine, Color.Gray)

                For y As Integer = 0 To arr.GetLength(1) - 1
                    dbgRtfForm.AppendColoredText($"{y,6}", Color.White)
                    For x As Integer = 0 To arr.GetLength(0) - 1
                        Dim isHighlight As Boolean = highlightList IsNot Nothing AndAlso highlightList.Contains((x, y, z))
                        If isHighlight Then
                            dbgRtfForm.AppendColoredText(Col(arr(x, y, z)), Color.Yellow, Color.DarkRed)
                        Else
                            dbgRtfForm.AppendColoredText(Col(arr(x, y, z)), Color.White)
                        End If
                    Next
                    dbgRtfForm.AppendColoredText(Environment.NewLine, Color.White)
                Next
                dbgRtfForm.AppendColoredText(Environment.NewLine, Color.White)
            Next
        End Sub

        '=== Textbox-Ausgabe ===
        Private dbgTxtForm As DebugTxtOutputForm = Nothing

        Public Sub Print3DArrayToTxtOutputForm(AktSpielfeldInfo As SpielfeldInfo, Optional highlightXyz As Triple = Nothing)
            If Not Debugger.IsAttached Then Return

            If dbgTxtForm Is Nothing OrElse dbgTxtForm.IsDisposed Then
                dbgTxtForm = New DebugTxtOutputForm()
                dbgTxtForm.Show()
            End If

            Dim sb As New System.Text.StringBuilder()
            With Spielfeld.SFD.AktSpielfeldInfo
                For z As Integer = 0 To .arrFB.GetLength(2) - 1
                    sb.AppendLine($"=== Ebene Z = {z} ===")

                    sb.Append("Y\X".PadLeft(7))
                    For x As Integer = 0 To .arrFB.GetLength(0) - 1
                        sb.Append(Col(x))
                    Next
                    sb.AppendLine()
                    sb.AppendLine(New String("-"c, 6 + .arrFB.GetLength(0) * SPALTENBREITE))

                    For y As Integer = 0 To .arrFB.GetLength(1) - 1
                        sb.Append($"   {y,2} |")
                        For x As Integer = 0 To .arrFB.GetLength(0) - 1
                            Dim isHighlight As Boolean = highlightXyz IsNot Nothing AndAlso highlightXyz.Contains(x, y, z)

                            Dim fb As Integer = .arrFB(x, y, z)
                            If fb >= 100 Then
                                'Hier in diesem Feld sind die Flags (derzeit, Programmversion 0) immer 0
                                'der Index steht hier mit 100 multipliziert.
                                'deshalb zum sichtbar machen den SteinIndex addieren.
                                fb += .SteinInfos((fb \ 1000) - 1).SteinIndex
                            End If

                            If isHighlight Then
                                sb.Append(ColBracket(fb))
                            Else
                                sb.Append(Col(fb))
                            End If
                        Next
                        sb.AppendLine()
                    Next
                    sb.AppendLine()
                Next
            End With
            dbgTxtForm.SetText(sb.ToString())
            dbgTxtForm.BringToFront()
            dbgTxtForm.Focus()
        End Sub

        '=== Konsolen-Ausgabe ===
        Public Sub Print3DArrayToConsole(arr As Integer(,,))
            If Not Debugger.IsAttached Then Exit Sub

            ConsoleHelper.AttachConsole()

            For z As Integer = 0 To arr.GetLength(2) - 1
                Console.WriteLine($"=== Ebene Z = {z} ===")

                Console.Write("Y\X".PadLeft(6))
                For x As Integer = 0 To arr.GetLength(0) - 1
                    Console.Write(Col(x))
                Next
                Console.WriteLine()
                Console.WriteLine(New String("-"c, 6 + arr.GetLength(0) * SPALTENBREITE))

                For y As Integer = 0 To arr.GetLength(1) - 1
                    Console.Write($"{y,6}")
                    For x As Integer = 0 To arr.GetLength(0) - 1
                        Console.Write(Col(arr(x, y, z)))
                    Next
                    Console.WriteLine()
                Next

                Console.WriteLine()
            Next

            Console.WriteLine("Drücke eine Taste, um die Konsole zu schließen...")
            Console.ReadKey()

            ConsoleHelper.DetachConsole()
        End Sub

    End Module


End Namespace
