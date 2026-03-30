
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
Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On
'
#Disable Warning IDE0079
#Disable Warning IDE1006
'
''' <summary>
''' Pfad: MahjongGK\Spielfeld\Runtime\Steinflug
''' </summary>
Public NotInheritable Class Airplanes_Flugweg

    Sub New()

    End Sub

    Private Shared ReadOnly _rnd As Random = New Random()

    Private ReadOnly _ptStart As Point
    Private ReadOnly _ptZiel As Point
    Private ReadOnly _steinSize As Size
    Private ReadOnly _weg As AirFlugWeg

    '
    ''' <summary>
    ''' Geschwindigkeit in Pixel pro Soll-Frame.
    ''' Der tatsächliche Fortschritt pro Render-Schritt ergibt sich aus:
    ''' Geschwindigkeit * timeDifferenzFaktor
    ''' </summary>
    Private ReadOnly _geschwindigkeitProFrame As Double

    Private ReadOnly _startF As PointF
    Private ReadOnly _zielF As PointF
    Private ReadOnly _control1 As PointF
    Private ReadOnly _control2 As PointF

    '
    ''' <summary>
    ''' Geschätzte Pfadlänge in Pixel.
    ''' </summary>
    Private ReadOnly _pathLength As Double

    '
    ''' <summary>
    ''' Fortschritt auf dem Pfad: 0.0 bis 1.0
    ''' </summary>
    Private _t As Double

    Private _isFinished As Boolean
    Private _lastPoint As Point

    '
    ''' <summary>
    ''' Erzeugt einen Flug eines Mahjongsteins von Start nach Ziel.
    ''' geschwindigkeitProFrame = Pixel pro Soll-Frame.
    ''' </summary>
    Public Sub New(ByVal ptStart As Point,
                   ByVal ptZiel As Point,
                   ByVal steinSize As Size,
                   ByVal weg As AirFlugWeg,
                   ByVal geschwindigkeitProFrame As Double)

        _ptStart = ptStart
        _ptZiel = ptZiel
        _steinSize = steinSize

        If geschwindigkeitProFrame <= 0.0 Then
            _geschwindigkeitProFrame = 1.0
        Else
            _geschwindigkeitProFrame = geschwindigkeitProFrame
        End If

        If weg = AirFlugWeg.Zufall Then
            _weg = CType(_rnd.Next(0, 6), AirFlugWeg)
        Else
            _weg = weg
        End If

        _startF = New PointF(CSng(ptStart.X), CSng(ptStart.Y))
        _zielF = New PointF(CSng(ptZiel.X), CSng(ptZiel.Y))

        Dim c1 As PointF
        Dim c2 As PointF
        BerechneSteuerpunkte(c1, c2)
        _control1 = c1
        _control2 = c2

        _pathLength = Math.Max(1.0, SchaetzePfadlaenge())

        _t = 0.0
        _isFinished = False
        _lastPoint = ptStart
    End Sub

    Public ReadOnly Property PtStart As Point
        Get
            Return _ptStart
        End Get
    End Property

    Public ReadOnly Property PtZiel As Point
        Get
            Return _ptZiel
        End Get
    End Property

    Public ReadOnly Property SteinSize As Size
        Get
            Return _steinSize
        End Get
    End Property

    Public ReadOnly Property Weg As AirFlugWeg
        Get
            Return _weg
        End Get
    End Property

    Public ReadOnly Property GeschwindigkeitProFrame As Double
        Get
            Return _geschwindigkeitProFrame
        End Get
    End Property

    ''' <summary>
    ''' Geschätzte Anzahl der Render-Schritte bei Soll-Frame-Zeit.
    ''' Mit kleinem Sicherheitsaufschlag. (+2)
    ''' Die tatsächliche Anzahl ist abhängig von den Werten des timeDifferenzFaktor.
    ''' </summary>
    Public ReadOnly Property RenderCountEstimate As Integer
        Get
            Return Math.Max(1, CInt(Math.Ceiling(_pathLength / _geschwindigkeitProFrame)) + 2)
        End Get
    End Property

    Public ReadOnly Property IsFinished As Boolean
        Get
            Return _isFinished
        End Get
    End Property
    '
    ''' <summary>
    ''' Gibt Not IsFinished zurück.
    ''' Gibt True zurück, solange noch neue Positionswerte geliefert werden können.
    ''' Sobald das Ziel erreicht ist, wird False zurückgegeben.
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property IsFlying As Boolean
        Get
            Return Not _isFinished
        End Get
    End Property

    Public ReadOnly Property CurrentPoint As Point
        Get
            Return _lastPoint
        End Get
    End Property

    '
    ''' <summary>
    ''' Liefert die nächste Position für den aktuellen Render-Schritt.
    ''' timeDifferenzFaktor = tatsächliche Zeit / Soll-Frame-Zeit
    ''' Beispiel:
    ''' Soll = 25 ms
    ''' Ist = 50 ms
    ''' Faktor = 2.0
    ''' </summary>
    Public Function GetNextPoint(ByVal timeDifferenzFaktor As Double) As Point

        If _isFinished Then
            Return _ptZiel
        End If

        If timeDifferenzFaktor <= 0.0 Then
            Return _lastPoint
        End If

        Dim pixelSchritt As Double = _geschwindigkeitProFrame * timeDifferenzFaktor

        If pixelSchritt <= 0.0 Then
            Return _lastPoint
        End If

        Dim deltaT As Double = pixelSchritt / _pathLength
        _t += deltaT

        If _t >= 1.0 Then
            _t = 1.0
            _lastPoint = _ptZiel
            _isFinished = True
            Return _lastPoint
        End If

        Dim pt As PointF = EvalPoint(_t)

        _lastPoint = New Point(CInt(Math.Round(pt.X)), CInt(Math.Round(pt.Y)))
        Return _lastPoint
    End Function

    '
    ''' <summary>
    ''' Setzt den Flug auf den Anfang zurück.
    ''' </summary>
    Public Sub Reset()
        _t = 0.0
        _isFinished = False
        _lastPoint = _ptStart
    End Sub

    '
    ''' <summary>
    ''' Optional: setzt den Flug sofort ins Ziel.
    ''' </summary>
    Public Sub FinishNow()
        _t = 1.0
        _isFinished = True
        _lastPoint = _ptZiel
    End Sub

    Private Sub BerechneSteuerpunkte(ByRef control1 As PointF, ByRef control2 As PointF)

        Dim dx As Single = _zielF.X - _startF.X
        Dim dy As Single = _zielF.Y - _startF.Y
        Dim dist As Single = Distanz(_startF, _zielF)

        If dist < 1.0F Then
            control1 = _startF
            control2 = _zielF
            Exit Sub
        End If

        Dim nx As Single = -dy / dist
        Dim ny As Single = dx / dist

        Dim bogenHoehe As Single = Math.Max(40.0F, dist * 0.25F)
        Dim zickZackHoehe As Single = Math.Max(25.0F, dist * 0.18F)

        Select Case _weg

            Case AirFlugWeg.Direkt
                control1 = New PointF(_startF.X + dx / 3.0F, _startF.Y + dy / 3.0F)
                control2 = New PointF(_startF.X + dx * 2.0F / 3.0F, _startF.Y + dy * 2.0F / 3.0F)

            Case AirFlugWeg.KurveOben
                control1 = New PointF(_startF.X + dx / 3.0F, _startF.Y + dy / 3.0F - bogenHoehe)
                control2 = New PointF(_startF.X + dx * 2.0F / 3.0F, _startF.Y + dy * 2.0F / 3.0F - bogenHoehe)

            Case AirFlugWeg.KurveUnten
                control1 = New PointF(_startF.X + dx / 3.0F, _startF.Y + dy / 3.0F + bogenHoehe)
                control2 = New PointF(_startF.X + dx * 2.0F / 3.0F, _startF.Y + dy * 2.0F / 3.0F + bogenHoehe)

            Case AirFlugWeg.BogenLinks
                control1 = New PointF(_startF.X + dx / 3.0F + nx * bogenHoehe, _startF.Y + dy / 3.0F + ny * bogenHoehe)
                control2 = New PointF(_startF.X + dx * 2.0F / 3.0F + nx * bogenHoehe, _startF.Y + dy * 2.0F / 3.0F + ny * bogenHoehe)

            Case AirFlugWeg.BogenRechts
                control1 = New PointF(_startF.X + dx / 3.0F - nx * bogenHoehe, _startF.Y + dy / 3.0F - ny * bogenHoehe)
                control2 = New PointF(_startF.X + dx * 2.0F / 3.0F - nx * bogenHoehe, _startF.Y + dy * 2.0F / 3.0F - ny * bogenHoehe)

            Case AirFlugWeg.ZickZack
                control1 = New PointF(_startF.X + dx * 0.25F + nx * zickZackHoehe, _startF.Y + dy * 0.25F + ny * zickZackHoehe)
                control2 = New PointF(_startF.X + dx * 0.75F - nx * zickZackHoehe, _startF.Y + dy * 0.75F - ny * zickZackHoehe)

            Case Else
                control1 = New PointF(_startF.X + dx / 3.0F, _startF.Y + dy / 3.0F)
                control2 = New PointF(_startF.X + dx * 2.0F / 3.0F, _startF.Y + dy * 2.0F / 3.0F)

        End Select
    End Sub

    Private Function EvalPoint(ByVal t As Double) As PointF

        Dim u As Double = 1.0 - t
        Dim tt As Double = t * t
        Dim uu As Double = u * u
        Dim uuu As Double = uu * u
        Dim ttt As Double = tt * t

        Dim x As Double =
            uuu * _startF.X +
            3.0 * uu * t * _control1.X +
            3.0 * u * tt * _control2.X +
            ttt * _zielF.X

        Dim y As Double =
            uuu * _startF.Y +
            3.0 * uu * t * _control1.Y +
            3.0 * u * tt * _control2.Y +
            ttt * _zielF.Y

        Return New PointF(CSng(x), CSng(y))
    End Function

    Private Function SchaetzePfadlaenge() As Double

        Const SEGMENTS As Integer = 32

        Dim sum As Double = 0.0
        Dim prev As PointF = EvalPoint(0.0)

        Dim i As Integer
        For i = 1 To SEGMENTS
            Dim t As Double = CDbl(i) / CDbl(SEGMENTS)
            Dim cur As PointF = EvalPoint(t)
            sum += Distanz(prev, cur)
            prev = cur
        Next

        Return Math.Max(1.0, sum)
    End Function

    Private Shared Function Distanz(ByVal p1 As PointF, ByVal p2 As PointF) As Single
        Dim dx As Double = p2.X - p1.X
        Dim dy As Double = p2.Y - p1.Y
        Return CSng(Math.Sqrt(dx * dx + dy * dy))
    End Function

End Class
