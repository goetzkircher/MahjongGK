''
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

Namespace Spielfeld

    ''' <summary>
    ''' Pfad: MahjongGK/Spielfeld/Render
    ''' </summary>
    Public Class RenderingTaktgeber

        ' ================================================
        ' Render-Architektur mit (einigermaßem) stabilem Takt
        ' ================================================
        ' - Der WinForms-Timer ist nur ein "Wecker" (Polling).
        ' - Der eigentliche Takt kommt von Stopwatch im FrameScheduler.
        ' - Vor jedem Invalidate() wird geprüft, ob ein Frame fällig ist.
        ' - OnPaint zeichnet IMMER → kein Flackern, keine "leeren" Frames.
        ' - TimeDifferenzFaktor gibt an, um wie viel der aktuelle Frame
        '   vom Sollwert (FRAME_MS) abweicht.
        '
        ' Typische Einstellungen:
        '   FRAME_MS = 25 ms (≈ 40 FPS)
        '   MAX_FACTOR = 2.0  (Animation max. doppelt so schnell)
        '   Timer.Interval = 5–10 ms (Pollingauflösung)
        '
        ' Ablauf:
        '   1) RenderTimer_Tick → Scheduler.TryNextFrame()
        '        → falls True → CurrentControl.Invalidate()
        '   2) OnPaint(...) → immer zeichnen, Faktor vom Scheduler
        '   3) Bewegungen/Animationen an TimeDifferenzFaktor koppeln
        '
        ' Vorteil:
        '   - Flackerfrei, da immer gezeichnet wird.
        '   - Stabiler, FPS-unabhängiger Animationsfluss.
        '   - Läuft auf langsamen und schnellen Rechnern gleichmäßig.
        ' ================================================


        Public Const FRAME_MS_SOLL As Double = 25.0   ' Ziel-FPS ≈ 40
        Public Const FRAME_MS_VGL As Double = 20.0    ' etwas langsamer, sonst ist TimeDifferenzFaktor immer > 1
        Public Const MAX_FACTOR As Double = 2.5       ' Kappen gegen Sprünge

        Private ReadOnly sw As Stopwatch = Stopwatch.StartNew()
        Private lastFrameMs As Double = 0.0
        Private _factor As Double = 1.0

        ' Long pauses = weicher Neustart
        Private Const LONG_PAUSE As Double = FRAME_MS_SOLL * MAX_FACTOR * 3

        Public Sub Reset()
            sw.Restart()
            lastFrameMs = 0.0
            _factor = 1.0
        End Sub

        ''' <summary>
        ''' Liefert True, wenn ein neuer Frame fällig ist. Setzt dabei Factor.
        ''' </summary>
        <DebuggerStepThrough>
        Public Function TryNextFrame() As Boolean

            Dim nowMs As Double = sw.Elapsed.TotalMilliseconds

            If lastFrameMs = 0.0 Then
                lastFrameMs = nowMs
                _factor = 1.0
                Return True
            End If

            Dim delta As Double = nowMs - lastFrameMs

            If delta > LONG_PAUSE Then
                _factor = 1.0
                lastFrameMs = nowMs
                Return True
            End If

            If delta < FRAME_MS_VGL Then
                Return False ' Noch nicht fällig
            End If

            _factor = Math.Min(delta / FRAME_MS_SOLL, MAX_FACTOR)

            lastFrameMs = nowMs

            Return True

        End Function

        Public ReadOnly Property TimeDifferenzFaktor As Double
            Get
                Return _factor
            End Get
        End Property

    End Class

End Namespace
