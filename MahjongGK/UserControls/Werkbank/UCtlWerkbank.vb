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
Imports MahjongGK.Spielfeld

#Disable Warning IDE0079
#Disable Warning IDE1006

Public Class UCtlWerkbank

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

    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        ' Immer malen, Factor kommt vom Scheduler (Stopwatch-basiert):
        Dim factor As Double = RenderingTaktgeberModul.FrameScheduler.TimeDifferenzFaktor

        Spielfeld.PaintSpielfeld_Paint(VisibleUserControl.Werkbank,
                                       e,
                                       New Rectangle(200, 100, Me.Width - 200 - 50, Me.Height - 100 - 50),
                                       factor)

#If DEBUGFRAME Then
        If stopwatch.IsRunning Then
            Debug.WriteLine("Zeit seit letztem Paint: " & stopwatch.ElapsedMilliseconds & " ms, factor=" & factor.ToString("0.00"))
        End If
#End If
        stopwatch.Restart()

    End Sub

#End Region


End Class
