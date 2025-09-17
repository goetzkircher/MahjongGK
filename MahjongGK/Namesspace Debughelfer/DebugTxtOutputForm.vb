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

#Disable Warning IDE0079
#Disable Warning IDE1006

Namespace MjDebug
    ''' <summary>
    ''' Ein Formular mit einer Textbox, die als Debug-Ausgabe verwendet werden kann.
    ''' </summary>  
    Public Class DebugTxtOutputForm
        Inherits Form

        Public ReadOnly TextBoxOutput As New TextBox With {
        .Multiline = True,
        .ScrollBars = ScrollBars.Both,
        .Dock = DockStyle.Fill,
        .Font = New Font("Consolas", 10),
        .WordWrap = False
    }

        Public Sub New()
            Me.Text = "Debug-Ausgabe"
            Me.Size = New Size(900, 600)
            Me.StartPosition = FormStartPosition.Manual
            Me.TopMost = True
            Me.Controls.Add(TextBoxOutput)
        End Sub

        Public Sub AppendText(text As String)
            If Me.InvokeRequired Then
                Me.Invoke(Sub() AppendText(text))
            Else
                TextBoxOutput.AppendText(text & Environment.NewLine)
            End If
        End Sub

        Public Sub SetText(text As String)
            If Me.InvokeRequired Then
                Me.Invoke(Sub() SetText(text))
            Else
                TextBoxOutput.Text = text
            End If
        End Sub
    End Class
End Namespace
