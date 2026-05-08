Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On
Imports MahjongGK.Spielfeld

'
' SPDX-License-Identifier: GPL-3.0-or-later
'###########################################################################
'#                                                                         #
'#   Copyright © 2025–2026 Götz Kircher <mahjonggk@t-online.de>            #
'#                                                                         #
'#                     MahjongGK  -  Mahjong Solitär                       #
'#                                                                         #
'#   This program is free software: you can redistribute it and/or modify  #
'#   it under thechrome://vivaldi-webui/startpage?section=Speed-dials&background-color=#f7f7f7 terms of the GNU General Public License as published by  #
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

#Disable Warning IDE0079
#Disable Warning IDE1006

''' <summary>
''' Pfad: MahjongGK/Spielfeld/Render
''' </summary>
Public Class UCtlSpielfeld

#Region "Code, der nicht entfernt werden darf (nicht zum TestCode gehörend)"

    Public Sub New()
        InitializeComponent()
        Me.Dock = DockStyle.Fill
        Me.DoubleBuffered = True
        Me.SetStyle(ControlStyles.UserPaint Or
                    ControlStyles.AllPaintingInWmPaint Or
                    ControlStyles.OptimizedDoubleBuffer Or
                    ControlStyles.ResizeRedraw, True)
        ' Optional, wenn wirklich der kompletten Bereich selber gezeichnet wird:
        Me.SetStyle(ControlStyles.Opaque, True)
        Me.UpdateStyles()

    End Sub

    ' Ich fülle komplett selbst -> Hintergrund NICHT automatisch löschen
    Protected Overrides Sub OnPaintBackground(pevent As PaintEventArgs)
        ' absichtlich leer
    End Sub

    Private stopwatch As New Stopwatch
    <DebuggerStepThrough>
    Protected Overrides Sub OnPaint(e As PaintEventArgs)

        If Helfer.IsInDesigner(Me) Then
            ' Im Designer NICHTS mehr rendern, nur optional füllen
            e.Graphics.Clear(SystemColors.Control)
            Return
        End If

        If SFMain.SFDatHasDataAndDoRender Then
            Dim factor As Double = SFMain.SFDat.SFRenMan.RenderTakt.TimeDifferenzFaktor

            SFMain.SFDat.SFRenMan.PaintUCtlSpielfeld(e, New Rectangle(0, 0, Me.Width, Me.Height), factor)
        Else
            RenderingStartScreen.PaintStartScreen(e.Graphics, New Rectangle(0, 0, Me.Width, Me.Height))
        End If
#If DEBUGFRAME Then
        If stopwatch.IsRunning Then
            Debug.WriteLine("Zeit seit letztem Paint: " & stopwatch.ElapsedMilliseconds & " ms, factor=" & factor.ToString("0.00"))
        End If
#End If
        stopwatch.Restart()

    End Sub

#End Region

End Class
