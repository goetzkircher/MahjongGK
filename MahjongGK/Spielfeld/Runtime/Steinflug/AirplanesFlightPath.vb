
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

Public Enum AirPointIs
    Center
    LeftUp
    CenterQuadrantLO
    CenterStein
End Enum

'
''' <summary>
''' Pfad: MahjongGK\Spielfeld\Runtime\Steinflug
''' </summary>
Public NotInheritable Class AirplanesFlightPath

    Sub New()

    End Sub

    Private Shared ReadOnly _rnd As Random = New Random()

    Private ReadOnly _ptStart As Point
    Private ReadOnly _ptZiel As Point
    Private ReadOnly _steinSize As Size
    Private ReadOnly _flightPath As PlaneFlightPath

    '
    ''' <summary>
    ''' Geschwindigkeit in Pixel pro Soll-Frame.
    ''' Der tatsächliche Fortschritt pro Render-Schritt ergibt sich aus:
    ''' Geschwindigkeit * timeDifferenzFaktor
    ''' </summary>
    Private ReadOnly _speedPerFrame As Double

    Private ReadOnly _startF As PointF
    Private ReadOnly _zielF As PointF
    Private ReadOnly _control1 As PointF
    Private ReadOnly _control2 As PointF

    Private ReadOnly _steinWidthHalf As Integer
    Private ReadOnly _steinHeightHalf As Integer
    '
    ''' <summary>
    ''' Geschätzte Pfadlänge in Pixel.
    ''' </summary>
    Private ReadOnly _pathLength As Double

    '
    ''' <summary>
    ''' Fortschritt auf dem Pfad: 0.0 bis 1.0
    ''' </summary>
    Private _progress As Double

    Private _isFlying As Boolean
    Private _lastPoint As Point

    '
    ''' <summary>
    ''' Erzeugt eine Flugroute eines Mahjongsteins von Start nach Ziel.
    ''' speedPerFrame = Pixel pro Soll-Frame.
    ''' </summary>
    Public Sub New(ptStart As Point,
                   startPointIs As AirPointIs,
                   ptZiel As Point,
                   zielPointIs As AirPointIs,
                   steinSize As Size,
                   flightPath As PlaneFlightPath,
                   speedPerFrame As Double
                   )

        _steinWidthHalf = steinSize.Width \ 2
        _steinHeightHalf = steinSize.Height \ 2
        _steinSize = steinSize

        '
        'Auf Steinmitte umrechnen, damit die Flugroute immer entlang der Steinmitte verläuft.
        '(bei sich drehenden Steinen wichtig)
        Select Case startPointIs
            Case AirPointIs.Center, AirPointIs.CenterStein
                _ptStart = ptStart

            Case AirPointIs.CenterQuadrantLO
                With ptStart
                    'auf die Mitte umrechnen
                    _ptStart = New Point(.X + _steinWidthHalf \ 2, .Y + _steinHeightHalf \ 2)
                End With

            Case AirPointIs.LeftUp
                With ptStart
                    _ptStart = New Point(.X + _steinWidthHalf, .Y + _steinHeightHalf)
                End With
            Case Else
                Throw New Exception("Unbekannte Enumeration.")
        End Select

        Select Case zielPointIs
            Case AirPointIs.Center, AirPointIs.CenterStein
                _ptZiel = ptZiel

            Case AirPointIs.CenterQuadrantLO
                With ptZiel
                    'auf die Mitte umrechnen
                    _ptZiel = New Point(.X + _steinWidthHalf \ 2, .Y + _steinHeightHalf \ 2)
                End With

            Case AirPointIs.LeftUp
                With ptZiel
                    _ptZiel = New Point(.X + _steinWidthHalf, .Y + _steinHeightHalf)
                End With

            Case Else
                Throw New Exception("Unbekannte Enumeration.")
        End Select

        If speedPerFrame <= 10.0 Then
            _speedPerFrame = 10.0
        Else
            _speedPerFrame = speedPerFrame
        End If

        If flightPath = PlaneFlightPath.Zufall Then
            _flightPath = CType(_rnd.Next(0, 6), PlaneFlightPath)
        Else
            _flightPath = flightPath
        End If

        _startF = New PointF(CSng(_ptStart.X), CSng(_ptStart.Y))
        _zielF = New PointF(CSng(_ptZiel.X), CSng(_ptZiel.Y))

        ComputeControlPoints(_control1, _control2)

        _pathLength = Math.Max(1.0, EstimatedPathLength())

        _progress = 0.0
        _isFlying = True
        _lastPoint = ptStart

    End Sub

    Public ReadOnly Property RectStart As Rectangle
        Get
            With _steinSize
                Return New Rectangle(_ptStart.X - .Width \ 2, _ptStart.Y - .Height \ 2, .Width, .Height)
            End With
        End Get
    End Property
    Public ReadOnly Property RectZiel As Rectangle
        Get
            With _steinSize
                Return New Rectangle(_ptZiel.X - .Width \ 2, _ptZiel.Y - .Height \ 2, .Width, .Height)
            End With
        End Get
    End Property

    '
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
    '' Im Bedarfsfall können die Propierties aktiviert werden.
    '
    ''Public ReadOnly Property PtCenterStart As Point
    ''    Get
    ''        Return _ptStart
    ''    End Get
    ''End Property

    ''Public ReadOnly Property PtCenterZiel As Point
    ''    Get
    ''        Return _ptZiel
    ''    End Get
    ''End Property
    ''Public ReadOnly Property PtLeftUpStart As Point
    ''    Get
    ''        With _ptStart
    ''            Return New Point(.X - _steinWidthHalf, .Y - _steinHeightHalf)
    ''        End With
    ''    End Get
    ''End Property

    ''Public ReadOnly Property PtLeftUpZiel As Point
    ''    Get
    ''        With _ptZiel
    ''            Return New Point(.X - _steinWidthHalf, .Y - _steinHeightHalf)
    ''        End With
    ''    End Get
    ''End Property

    ''Public ReadOnly Property SteinSize As Size
    ''    Get
    ''        Return _steinSize
    ''    End Get
    ''End Property

    ''Public ReadOnly Property AirPlaneFlightPath As PlaneFlightPath
    ''    Get
    ''        Return _flightPath
    ''    End Get
    ''End Property

    ''Public ReadOnly Property SpeedPerFrame As Double
    ''    Get
    ''        Return _speedPerFrame
    ''    End Get
    ''End Property

    ''''' <summary>
    ''''' Geschätzte Anzahl der Render-Schritte bei Soll-Frame-Zeit.
    ''''' Mit kleinem Sicherheitsaufschlag. (+2)
    ''''' Die tatsächliche Anzahl ist abhängig von den Werten des timeDifferenzFaktor.
    ''''' </summary>
    ''Public ReadOnly Property RenderCountEstimate As Integer
    ''    Get
    ''        Return Math.Max(1, CInt(Math.Ceiling(_pathLength / _speedPerFrame)) + 2)
    ''    End Get
    ''End Property

    ''Public ReadOnly Property CurrentCenterPoint As Point
    ''    Get
    ''        Return _lastPoint
    ''    End Get
    ''End Property
    ''Public ReadOnly Property CurrentLeftUpPoint As Point
    ''    Get
    ''        With _lastPoint
    ''            Return New Point(.X - _steinWidthHalf, .Y - _steinHeightHalf)
    ''        End With
    ''    End Get
    ''End Property

    '
    ''' <summary>
    ''' Liefert die nächste Center-Position für den aktuellen Render-Schritt.
    ''' Das ist die Basisfunktion, die von allen GetNext... Funktionen aufgerufen wird.
    ''' </summary>
    Public Function GetNextCenterPoint(ByVal timeDifferenzFaktor As Double) As (HasValue As Boolean, pt As Point)

        If Not _isFlying Then
            Return (False, Point.Empty)
        End If

        Dim pixelSchritt As Double = _speedPerFrame * timeDifferenzFaktor

        If pixelSchritt <= 0.0 Then
            Return (False, Point.Empty)
        End If

        Dim deltaT As Double = pixelSchritt / _pathLength
        _progress += deltaT

        If _progress >= 1.0 Then
            _progress = 1.0
            _lastPoint = _ptZiel
            _isFlying = False
            Return (True, _ptZiel)
        End If

        Dim pt As PointF = EvalPoint(_progress)

        _lastPoint = New Point(CInt(Math.Round(pt.X)), CInt(Math.Round(pt.Y)))
        Return (True, _lastPoint)

    End Function

    '
    ''' <summary>
    ''' Setzt den Flug auf den Anfang zurück.
    ''' </summary>
    Public Sub Reset()
        _progress = 0.0
        _isFlying = False
        _lastPoint = _ptStart
    End Sub

    '
    ''' <summary>
    ''' Optional: setzt den Flug sofort ins Ziel.
    ''' </summary>
    Public Sub FinishNow()
        _progress = 1.0
        _isFlying = True
        _lastPoint = _ptZiel
    End Sub
    ''' <summary>
    ''' Optional: bricht den Flug sofort ab.
    ''' </summary>
    Public Sub Abort()
        _isFlying = False
    End Sub

    Private Sub ComputeControlPoints(ByRef control1 As PointF, ByRef control2 As PointF)

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

        Select Case _flightPath

            Case PlaneFlightPath.Direkt
                control1 = New PointF(_startF.X + dx / 3.0F, _startF.Y + dy / 3.0F)
                control2 = New PointF(_startF.X + dx * 2.0F / 3.0F, _startF.Y + dy * 2.0F / 3.0F)

            Case PlaneFlightPath.KurveOben
                control1 = New PointF(_startF.X + dx / 3.0F, _startF.Y + dy / 3.0F - bogenHoehe)
                control2 = New PointF(_startF.X + dx * 2.0F / 3.0F, _startF.Y + dy * 2.0F / 3.0F - bogenHoehe)

            Case PlaneFlightPath.KurveUnten
                control1 = New PointF(_startF.X + dx / 3.0F, _startF.Y + dy / 3.0F + bogenHoehe)
                control2 = New PointF(_startF.X + dx * 2.0F / 3.0F, _startF.Y + dy * 2.0F / 3.0F + bogenHoehe)

            Case PlaneFlightPath.BogenLinks
                control1 = New PointF(_startF.X + dx / 3.0F + nx * bogenHoehe, _startF.Y + dy / 3.0F + ny * bogenHoehe)
                control2 = New PointF(_startF.X + dx * 2.0F / 3.0F + nx * bogenHoehe, _startF.Y + dy * 2.0F / 3.0F + ny * bogenHoehe)

            Case PlaneFlightPath.BogenRechts
                control1 = New PointF(_startF.X + dx / 3.0F - nx * bogenHoehe, _startF.Y + dy / 3.0F - ny * bogenHoehe)
                control2 = New PointF(_startF.X + dx * 2.0F / 3.0F - nx * bogenHoehe, _startF.Y + dy * 2.0F / 3.0F - ny * bogenHoehe)

            Case PlaneFlightPath.ZickZack
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

    Private Function EstimatedPathLength() As Double

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
