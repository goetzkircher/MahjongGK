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
    ''' Debug-Hilfsmodul zum Anzeigen von Bitmap-Arrays in einem Fenster.
    ''' </summary>
    Public Module DebugBitmaps
        Public Sub DebugShowBitmaps(ByVal bmp As Bitmap)
            If bmp Is Nothing Then Return
            Dim arrBmp As Bitmap() = {bmp}
            Dim f As New DebugArrayBitmapsForm(arrBmp)
            f.Show()                  ' nicht-modal anzeigen
            f.BringToFront()
        End Sub

        ' --- Aufrufer: einfach diese Prozedur verwenden ---
        Public Sub DebugShowArrayBitmaps(ByVal arrBmp() As Bitmap)
            If arrBmp Is Nothing OrElse arrBmp.Length = 0 Then Return

            Dim f As New DebugArrayBitmapsForm(arrBmp)
            f.Show()                  ' nicht-modal anzeigen
            f.BringToFront()
        End Sub


        ' --- Internes Debug-Fenster ---
        Friend Class DebugArrayBitmapsForm
            Inherits Form

            Public Sub New(images As Bitmap())
                Me.Text = $"Debug – Array Bitmaps ({images.Length})"
                Me.StartPosition = FormStartPosition.Manual
                Me.Location = New Point(0, 0)           ' linke obere Bildschirmecke
                Me.Size = New Size(250, 850)            ' gewünschte Fenstergröße
                Me.FormBorderStyle = FormBorderStyle.SizableToolWindow
                Me.KeyPreview = True


                Dim flow As New DoubleBufferedFlowPanel With {
                    .Dock = DockStyle.Fill,
                    .AutoScroll = True,
                    .FlowDirection = FlowDirection.TopDown,
                    .WrapContents = False,
                    .Padding = New Padding(10),
                    .BackColor = Color.White
                }

                ' Bitmaps als PictureBoxes hinzufügen
                For i As Integer = 0 To images.Length - 1
                    Dim bmp As Bitmap = images(i)
                    If bmp Is Nothing Then Continue For

                    Dim pb As New PictureBox With {
                .Image = bmp,
                .SizeMode = PictureBoxSizeMode.AutoSize
            }

                    ' Container nur für den Abstand unten (10px)
                    Dim holder As New Panel With {
                .AutoSize = True,
                .AutoSizeMode = AutoSizeMode.GrowAndShrink,
                .Margin = New Padding(0, 0, 0, 10)
            }
                    holder.Controls.Add(pb)
                    flow.Controls.Add(holder)
                Next

                Me.Controls.Add(flow)
            End Sub
        End Class


        ' --- Double-buffered FlowLayoutPanel zur Flacker-Minimierung ---
        Friend Class DoubleBufferedFlowPanel
            Inherits FlowLayoutPanel
            Public Sub New()
                Me.DoubleBuffered = True
                Me.ResizeRedraw = True
            End Sub
        End Class

    End Module

End Namespace