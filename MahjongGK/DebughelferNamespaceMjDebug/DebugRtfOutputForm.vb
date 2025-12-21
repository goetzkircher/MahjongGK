Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On
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


Namespace MjDebug

#Disable Warning IDE0079
#Disable Warning IDE1006


    Public Class DebugRtfOutputForm
        Inherits Form

        Public ReadOnly RichTextOutput As New RichTextBox With {
        .Dock = DockStyle.Fill,
        .Font = New Font("Consolas", 10),
        .ReadOnly = True,
        .WordWrap = False,
        .BackColor = Color.Black,
        .ForeColor = Color.White
    }

        Public Sub New()
            Me.Text = "Debug-Ausgabe mit Highlights"
            Me.Size = New Size(900, 600)
            Me.StartPosition = FormStartPosition.Manual
            Me.TopMost = True
            Me.Controls.Add(RichTextOutput)
        End Sub

        Public Sub AppendColoredText(text As String, foreColor As Color, Optional backColor As Color = Nothing)
            If Me.InvokeRequired Then
                Me.Invoke(Sub() AppendColoredText(text, foreColor, backColor))
                Return
            End If

            Dim start As Integer = RichTextOutput.TextLength
            RichTextOutput.AppendText(text)
            Dim [end] As Integer = RichTextOutput.TextLength

            RichTextOutput.Select(start, [end] - start)
            RichTextOutput.SelectionColor = foreColor
            If backColor <> Color.Empty Then
                RichTextOutput.SelectionBackColor = backColor
            Else
                RichTextOutput.SelectionBackColor = RichTextOutput.BackColor
            End If
            RichTextOutput.SelectionLength = 0
            RichTextOutput.SelectionStart = RichTextOutput.TextLength
            RichTextOutput.ScrollToCaret()
        End Sub

        Public Sub ClearText()
            If Me.InvokeRequired Then
                Me.Invoke(Sub() ClearText())
            Else
                RichTextOutput.Clear()
            End If
        End Sub
    End Class

End Namespace