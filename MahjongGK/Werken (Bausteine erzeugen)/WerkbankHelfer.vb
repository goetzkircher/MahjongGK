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

Namespace Umfeld
    Public Module WerkbankHelfer

        Public Function IsInsideEllipse(xAkt As Integer, x0 As Integer, xMax As Integer, yAkt As Integer, y0 As Integer, yMax As Integer) As Boolean

            Dim nxMax As Integer = (xMax - x0) \ 2
            Dim nxAkt As Integer = (xAkt - x0) \ 2
            Dim nyMax As Integer = (yMax - y0) \ 2
            Dim nyAkt As Integer = (yAkt - y0) \ 2

            Return IsInsideEllipse(nxMax, nyMax, nxAkt, nyAkt)

        End Function

        '
        ''' <summary>
        ''' Prüft, ob die Feldmitte (xAkt,yAkt) (0-basiert) innerhalb (inkl. Rand) der Ellipse liegt,
        ''' die im xMax × yMax-Raster zentriert ist und die größtmögliche inscribed Ellipse bildet.
        ''' </summary>
        Public Function IsInsideEllipse(xMax As Integer, yMax As Integer,
                                        xAkt As Integer, yAkt As Integer) As Boolean
            Const EPS As Double = 0.000000000001

            ' Gültigkeit prüfen
            If xMax <= 0 OrElse yMax <= 0 Then Return False
            If xAkt < 0 OrElse xAkt >= xMax OrElse yAkt < 0 OrElse yAkt >= yMax Then Return False

            ' Zentrum der Ellipse in Feldeinheiten
            Dim cx As Double = xMax / 2.0
            Dim cy As Double = yMax / 2.0

            ' Halbachsen: so gewählt, dass die Ellipse die äußersten Feldmittelpunkte tangiert
            Const SUBVAL As Double = 0.94
            Dim rx As Double = (xMax * SUBVAL) / 2.0
            Dim ry As Double = (yMax * SUBVAL) / 2.0

            ' Feldmitte
            Dim px As Double = xAkt + 0.5
            Dim py As Double = yAkt + 0.5

            ' Degenerationsfälle (xMax=1 und/oder yMax=1)
            If rx <= EPS AndAlso ry <= EPS Then
                ' 1×1-Fall: genau ein Feld
                Return (xMax = 1 AndAlso yMax = 1 AndAlso xAkt = 0 AndAlso yAkt = 0)
            ElseIf rx <= EPS Then
                ' „Vertikale Linie“ bei x=cx
                Return Math.Abs(px - cx) <= EPS AndAlso Math.Abs((py - cy) / ry) <= 1.0 + EPS
            ElseIf ry <= EPS Then
                ' „Horizontale Linie“ bei y=cy
                Return Math.Abs(py - cy) <= EPS AndAlso Math.Abs((px - cx) / rx) <= 1.0 + EPS
            End If

            ' Ellipsentest
            Dim dx As Double = (px - cx) / rx
            Dim dy As Double = (py - cy) / ry
            Return (dx * dx + dy * dy) <= 1.0 + EPS

        End Function

        '
        ''' <summary>
        ''' Approximierte signierte Distanz (in Feld-Einheiten) der Feldmitte (xAkt,yAkt)
        ''' zur Ellipse, die im xMax×yMax-Raster zentriert ist (inscribed).
        ''' Negativ = innerhalb, 0 ≈ Rand, positiv = außerhalb.
        ''' </summary>
        Public Function EllipseSignedDistance(xMax As Integer, yMax As Integer,
                                          xAkt As Integer, yAkt As Integer) As Double
            Const EPS As Double = 0.000000000001
            If xMax <= 0 OrElse yMax <= 0 Then Return Double.PositiveInfinity
            If xAkt < 0 OrElse xAkt >= xMax OrElse yAkt < 0 OrElse yAkt >= yMax Then Return Double.PositiveInfinity

            Dim cx As Double = xMax / 2.0
            Dim cy As Double = yMax / 2.0
            Dim rx As Double = (xMax - 1) / 2.0
            Dim ry As Double = (yMax - 1) / 2.0

            ' Degenerationen (schmale Fälle) simpel behandeln
            If rx <= EPS AndAlso ry <= EPS Then
                Dim px As Double = xAkt + 0.5, py As Double = yAkt + 0.5
                Dim d As Double = Math.Sqrt((px - cx) * (px - cx) + (py - cy) * (py - cy))
                Return d ' „Ring“ um einen Punkt
            ElseIf rx <= EPS Then
                Dim px As Double = xAkt + 0.5
                Return Math.Abs(px - cx) ' Band um vertikale Linie
            ElseIf ry <= EPS Then
                Dim py As Double = yAkt + 0.5
                Return Math.Abs(py - cy) ' Band um horizontale Linie
            End If

            ' Normierter Ellipsenwert F = q - 1
            Dim pxC As Double = (xAkt + 0.5) - cx
            Dim pyC As Double = (yAkt + 0.5) - cy
            Dim invRx2 As Double = 1.0 / (rx * rx)
            Dim invRy2 As Double = 1.0 / (ry * ry)

            Dim q As Double = pxC * pxC * invRx2 + pyC * pyC * invRy2
            Dim Fx As Double = 2.0 * pxC * invRx2
            Dim Fy As Double = 2.0 * pyC * invRy2
            Dim grad As Double = Math.Sqrt(Fx * Fx + Fy * Fy)

            If grad <= EPS Then
                ' Am Zentrum: klare „weit innen“-Situation → großer negativer Abstand
                Return -Math.Min(rx, ry)
            End If

            ' Signierte Normaldistanz (erstordentliche Approximation)
            Return (q - 1.0) / grad
        End Function

        '
        ''' <summary>
        ''' True, wenn (xAkt,yAkt) im Ring (Rand) der Ellipse liegt.
        ''' ringHalfWidth: Halbbreite des Rings in Feld-Einheiten (z. B. 0.5 ≈ 1 Zelle „dick“).
        ''' </summary>
        Public Function IsOnEllipseRing(xMax As Integer, yMax As Integer,
                                    xAkt As Integer, yAkt As Integer,
                                    ringHalfWidth As Double) As Boolean
            Const EPS As Double = 0.000000000001
            If ringHalfWidth < 0 Then Return False
            Dim d As Double = EllipseSignedDistance(xMax, yMax, xAkt, yAkt)
            Return Math.Abs(d) <= ringHalfWidth + EPS
        End Function

        '#################################################################################################
        'Anwendung (typischer U-Fall in Bildschirmkoordinaten)
        'Raster: xMax, yMax (0-basiert), Feldmitte = (x+0.5, y+0.5).
        'U-Form nach unten (weil Y nach unten wächst) axis:=Vertical, opensPositive:=True.
        'Scheitelpunkt in der Mitte oben vx = xMax / 2.0, vy nahe oberem Rand (z. B. vy = 2.0).
        'Öffnung wählen :  z. B. gewünschte Breite an yTarget = yMax - 1

        'Dim vx As Double = xMax / 2.0
        '        Dim vy As Double = 2.0
        '        Dim p As Double = ParabolaPFromWidthAtY(vy, yMax - 1, desiredWidth:=xMax * 0.9) ' 90% Breite am unteren Ende
        '        Dim thick As Double = 0.5 ' ≈ 1 Zelle Strichstärke

        '        If IsOnParabolaBand(xMax, yMax, x, y, vx, vy, p, thick,
        '                    axis:=ParabolaAxis.Vertical, opensPositive:=True) Then
        '        ' ... Zelle (x,y) liegt im U-Rand
        '        End If

        'Hinweise

        '„Öffnungswinkel“: Bei Parabeln ist das physikalisch der Krümmungsparameter p.
        'Größeres p ⇒ flacher (weiter offen), kleineres p ⇒ enger (steil).
        'Die Helfer ParabolaPFromWidthAtY/…AtX geben dir intuitiv p aus Ziel-Breite/Höhe.

        'Clipping/Längenbegrenzung: Für eine „U-Bügel-Länge“ kannst du zusätzlich z. B. y auf einen Bereich begrenzen (y ≥ vy und y ≤ yCut) oder analog bei horizontaler Parabel.

        'Strichstärke: ringHalfWidth = 0.5 ≈ eine Zelle „dick“; 1.0 ≈ zwei Zellen; feiner regelbar.


        Public Enum ParabolaAxis
            Vertical   ' (x - vx)^2 = 4 p * sgn * (y - vy)
            Horizontal ' (y - vy)^2 = 4 p * sgn * (x - vx)
        End Enum

        '
        ''' <summary>
        ''' Signierte Distanz (in Feld-Einheiten) der Feldmitte (x,y) zu einer Parabel.
        ''' Achse: Vertical (Standard) → U-Form; Horizontal → C-Form.
        ''' sgn: True = Öffnung in +Y bzw. +X-Richtung, False = in -Y bzw. -X.
        ''' Scheitelpunkt (vx,vy) in Feld-Einheiten; p > 0 steuert die Öffnung (größer = weiter geöffnet).
        ''' </summary>
        Public Function ParabolaSignedDistance(
                xMax As Integer, yMax As Integer, x As Integer, y As Integer,
                vx As Double, vy As Double,
                p As Double,
                Optional axis As ParabolaAxis = ParabolaAxis.Vertical,
                Optional opensPositive As Boolean = True
            ) As Double

            Const EPS As Double = 0.000000000001
            If x < 0 OrElse x >= xMax OrElse y < 0 OrElse y >= yMax Then
                Return Double.PositiveInfinity
            End If
            If p <= EPS Then
                ' p≈0 wäre degeneriert (zu „spitz“) → praktisch unbrauchbar
                Return Double.PositiveInfinity
            End If

            Dim px As Double = (x + 0.5) - vx
            Dim py As Double = (y + 0.5) - vy
            Dim s As Double = If(opensPositive, 1.0, -1.0)

            Dim F As Double, gx As Double, gy As Double

            If axis = ParabolaAxis.Vertical Then
                ' (x - vx)^2 - 4 p s * (y - vy) = 0
                F = px * px - 4.0 * p * s * py
                gx = 2.0 * px
                gy = -4.0 * p * s
            Else
                ' (y - vy)^2 - 4 p s * (x - vx) = 0
                F = py * py - 4.0 * p * s * px
                gx = -4.0 * p * s
                gy = 2.0 * py
            End If

            Dim grad As Double = Math.Sqrt(gx * gx + gy * gy)
            If grad <= EPS Then
                ' Nur äußerst selten (numerisch), fallback auf große Distanz
                Return Double.PositiveInfinity
            End If

            ' Erstordnungs-Approximation der orthogonalen Distanz
            Return F / grad
        End Function

        '
        ''' <summary>
        ''' True, wenn (x,y) im Parabel-Band liegt (Rand mit definierter Strichstärke).
        ''' ringHalfWidth: Halbbreite in Feld-Einheiten (z. B. 0.5 ≈ „1 Zelle dick“).
        ''' </summary>
        Public Function IsOnParabolaBand(
                xMax As Integer, yMax As Integer, x As Integer, y As Integer,
                vx As Double, vy As Double,
                p As Double,
                ringHalfWidth As Double,
                Optional axis As ParabolaAxis = ParabolaAxis.Vertical,
                Optional opensPositive As Boolean = True
            ) As Boolean

            Const EPS As Double = 0.000000000001
            If ringHalfWidth < 0 Then Return False

            Dim d As Double = ParabolaSignedDistance(xMax, yMax, x, y, vx, vy, p, axis, opensPositive)
            Return Math.Abs(d) <= ringHalfWidth + EPS
        End Function

        '
        ''' <summary>
        ''' Komfort-Helfer: Für gegebene gewünschte Breite an einem Ziel-y (vertikale Parabel)
        ''' den p-Wert berechnen. Gilt für yTarget jenseits des Scheitels (selbe Öffnungsrichtung!).
        ''' Formel: width = 2 * sqrt(4 p * |yTarget - vy|)  ⇒  p = width^2 / (16 * |yTarget - vy|)
        ''' </summary>
        Public Function ParabolaPFromWidthAtY(
                vy As Double, yTarget As Double, desiredWidth As Double
            ) As Double
            Dim dy As Double = Math.Abs(yTarget - vy)
            If dy <= 0 OrElse desiredWidth <= 0 Then Return Double.PositiveInfinity
            Return (desiredWidth * desiredWidth) / (16.0 * dy)
        End Function

        '
        ''' <summary>
        ''' Analog für horizontale Parabel: p aus gewünschter Höhe an Ziel-x.
        ''' </summary>
        Public Function ParabolaPFromHeightAtX(
                vx As Double, xTarget As Double, desiredHeight As Double
            ) As Double
            Dim dx As Double = Math.Abs(xTarget - vx)
            If dx <= 0 OrElse desiredHeight <= 0 Then Return Double.PositiveInfinity
            Return (desiredHeight * desiredHeight) / (16.0 * dx)
        End Function


        '
        ' Wenn du mir sagst, wo dein Scheitel (vx,vy) sitzen soll und welche Breite/Höhe
        ' du an welchem Rand willst, bau ich dir eine kleine Factory, die p und die Parameter
        ' pro Ebene z automatisch ableitet (für dein „Kegel-/Bügel“-Profil).
        'Du:
        'Das weis ich noch nicht, das muss ich ausprobieren.
        'Jeder Stein einer höheren Schicht braucht ja ein Fundament aus anderen Steinen auf der
        'darunter liegenden Schicht. Vielleicht ist der einfachste AirPlaneFlightPath, die unterste Schicht
        'zu berechnen und die nächsten Schichten einfach durchzuprobieren, d.h. durch alle Felder
        'zu interieren und mit der Funktion HasFundament zu testen.
        'ChatGPT:
        'Nachgedacht für 1m 46s

        'Klingt nach einem guten Plan. Hier hast du die Bausteine dafür:
        'eine schnelle HasFundament-Prüfung (2×2-Stein), plus eine Erosion (Morphologie)
        'als Turbo-Variante für ganze Ebenen.


        Public Enum SupportRule
            AllCells        ' alle 2×2 Zellen darunter müssen belegt sein (klassisches Fundament)
            AtLeastN        ' mindestens N Zellen darunter (Parameter)
            AnyRowOrCol     ' eine volle Zeile ODER volle Spalte darunter (Trägerbalken)
        End Enum

        '
        ''' <summary>
        ''' Prüft, ob ein 2×2-Stein mit linker oberer Ecke (x,y) auf Ebene z von der Ebene darunter (z-1)
        ''' ausreichend getragen wird. Das Grid ist ein 2D-Boolean-Array in Feld-Koordinaten (0-basiert):
        ''' grid(x,y) = True bedeutet: Feld ist von einem Stein belegt.
        ''' </summary>
        Public Function HasFundament(
        below As Boolean(,),
        x As Integer, y As Integer,
        Optional rule As SupportRule = SupportRule.AllCells,
        Optional nMin As Integer = 3
    ) As Boolean

            ' Bounds prüfen: 2×2 Rechteck muss vollständig im "below"-Raster liegen
            Dim w As Integer = below.GetUpperBound(0) + 1
            Dim h As Integer = below.GetUpperBound(1) + 1
            If x < 0 OrElse y < 0 OrElse x + 1 >= w OrElse y + 1 >= h Then Return False

            Dim b00 As Boolean = below(x, y)
            Dim b10 As Boolean = below(x + 1, y)
            Dim b01 As Boolean = below(x, y + 1)
            Dim b11 As Boolean = below(x + 1, y + 1)

            Select Case rule
                Case SupportRule.AllCells
                    Return b00 AndAlso b10 AndAlso b01 AndAlso b11

                Case SupportRule.AtLeastN
                    Dim cnt As Integer = CInt(b00) + CInt(b10) + CInt(b01) + CInt(b11)
                    Return cnt >= Math.Max(0, Math.Min(4, nMin))

                Case SupportRule.AnyRowOrCol
                    Dim row0 As Boolean = b00 AndAlso b10
                    Dim row1 As Boolean = b01 AndAlso b11
                    Dim col0 As Boolean = b00 AndAlso b01
                    Dim col1 As Boolean = b10 AndAlso b11
                    Return row0 OrElse row1 OrElse col0 OrElse col1

                Case Else
                    Return False
            End Select
        End Function

        '
        ''' <summary>
        ''' Morphologische Erosion mit 2×2-Kernel (alle vier müssen True sein).
        ''' Liefert die Menge aller 2×2-Top-Left-Positionen, die ein Fundament haben.
        ''' Top-Left-Koordinate (x,y) im Ergebnis entspricht einem legalen Platz für den 2×2-Stein.
        ''' </summary>
        Public Function Erode2x2AllCells(below As Boolean(,)) As Boolean(,)
            Dim w As Integer = below.GetUpperBound(0) + 1
            Dim h As Integer = below.GetUpperBound(1) + 1
            If w < 2 OrElse h < 2 Then
                Return New Boolean(Math.Max(0, w - 1), Math.Max(0, h - 1)) {} ' leeres Raster in gleicher Achsreihenfolge
            End If

            Dim topW As Integer = w - 1
            Dim topH As Integer = h - 1
            Dim top As Boolean(,) = New Boolean(topW - 1, topH - 1) {}

            For y As Integer = 0 To topH - 1
                For x As Integer = 0 To topW - 1
                    If below(x, y) AndAlso below(x + 1, y) AndAlso below(x, y + 1) AndAlso below(x + 1, y + 1) Then
                        top(x, y) = True
                    End If
                Next
            Next
            Return top
        End Function

        '
        ''' <summary>
        ''' Allgemeinere Erosion: mindestens nMin der 4 Zellen darunter müssen belegt sein.
        ''' Praktisch, wenn du „lockere“ Fundamente zulassen willst.
        ''' </summary>
        Public Function Erode2x2AtLeastN(below As Boolean(,), nMin As Integer) As Boolean(,)
            Dim w As Integer = below.GetUpperBound(0) + 1
            Dim h As Integer = below.GetUpperBound(1) + 1
            If w < 2 OrElse h < 2 Then
                Return New Boolean(Math.Max(0, w - 1), Math.Max(0, h - 1)) {}
            End If

            Dim need As Integer = Math.Max(0, Math.Min(4, nMin))
            Dim topW As Integer = w - 1
            Dim topH As Integer = h - 1
            Dim top As Boolean(,) = New Boolean(topW - 1, topH - 1) {}

            For y As Integer = 0 To topH - 1
                For x As Integer = 0 To topW - 1
                    Dim cnt As Integer =
                    CInt(below(x, y)) + CInt(below(x + 1, y)) +
                    CInt(below(x, y + 1)) + CInt(below(x + 1, y + 1))
                    If cnt >= need Then top(x, y) = True
                Next
            Next
            Return top
        End Function

        'So setzt du's ein
        'Unterste Schicht(z = 0) füllst du mit deiner Flächenfunktion (Ellipse, Parabel, …)
        'In Feld-/Stein-Koordinaten 0-basiert: base(x,y) = IsInsideEllipse(...).
        'Nächste Schicht(z = 1) : 
        'Entweder „brute force“: für jeden Kandidaten (x,y) prüfst du HasFundament(base, x, y).
        'Oder superschnell: layer1 = Erode2x2AllCells(base).
        '(Das liefert dir direkt alle validen Top-Left-Positionen für 2×2-Steine auf z=1.)
        'Weiter hoch: iterativ erodieren: layer(z) = Erode2x2AllCells(layer(z-1)) (oder AtLeastN,
        'wenn du lockerer bauen willst).
        'Der Effekt ist genau dein „Kegel/abgetrepptes Schrumpfen“.
        'Das Schöne: Du brauchst keine Offsets/Step-2-Gymnastik mehr.
        'Deine eigene Konvertierung von (idxX, idxY, idxZ) ins 0-basierte Feldraster
        'bleibt davor/danach – diese Helpers sind rein lokal auf dem belegten/freien Feld-Grid.
        'Wenn du magst, schreibe ich dir noch zwei kleine Helfer:
        'ToFieldGrid(stonePositions) ⇄ FromFieldGrid(layer) (Mapping zwischen deiner Objektliste und Boolean(,)).
        'Eine kompakte Pipeline: BuildLayers(base, maxZ, rule) die solange erodiert, bis eine Ebene leer wird.

    End Module


End Namespace