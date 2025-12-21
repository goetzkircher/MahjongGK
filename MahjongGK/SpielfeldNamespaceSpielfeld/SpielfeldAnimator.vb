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


Imports System.Drawing.Drawing2D
Imports System.Drawing.Imaging

Namespace Spielfeld

    'Private rnd As New Random()

    'Public Sub SetRandomAnimation(ByRef feld As FeldBeschreiber)
    '    feld.AnimID = CType(rnd.Next(1, 13), AnimationType) ' 1–12
    '    feld.AktStep = 0
    '    feld.MaxStep = 350 ' oder je nach Effekt
    'End Sub

    ''' <summary>
    ''' Hier sind alle Funktionen zur Animation angesiedelt einschließlich deren Verwaltung.
    ''' </summary>
    Public Module SpielfeldAnimator

        'Grundregeln aller Funktionen:
        'bmp ist das Original.
        'aktStep ist 0…maxStep.
        'Jede Funktion gibt ein neues Bitmap zurück (nicht das Original verändern).
        'Transparenz bleibt erhalten.
        'Alle Funktionen sind so geschrieben, dass sie sich sowohl für Erscheinen als auch für
        'Verschwinden anpassen lassen (Parameter-Logik umkehren).

        'Animations-Set (12 Stück)

        Public Enum AnimationType
            None = 0
            ScaleDown = 1
            ScaleUp = 2
            Rotate = 3
            RotateShrink = 4
            FlipX = 5
            FlipY = 6
            FlipShrink = 7
            Pulse = 8
            SlideLeft = 9
            SlideUp = 10
            ScaleSlide = 11
            RotatePulse = 12
        End Enum

        Public Function RunAnimation(animType As AnimationType, bmp As Bitmap, aktStep As Integer, maxStep As Integer) As Bitmap
            Select Case animType
                Case AnimationType.ScaleDown : Return Animation_ScaleDown(bmp, aktStep, maxStep)
                Case AnimationType.ScaleUp : Return Animation_ScaleUp(bmp, aktStep, maxStep)
                Case AnimationType.Rotate : Return Animation_Rotate(bmp, aktStep, maxStep)
                Case AnimationType.RotateShrink : Return Animation_RotateShrink(bmp, aktStep, maxStep)
                Case AnimationType.FlipX : Return Animation_FlipX(bmp, aktStep, maxStep)
                Case AnimationType.FlipY : Return Animation_FlipY(bmp, aktStep, maxStep)
                Case AnimationType.FlipShrink : Return Animation_FlipShrink(bmp, aktStep, maxStep)
                Case AnimationType.Pulse : Return Animation_Pulse(bmp, aktStep, maxStep)
                Case AnimationType.SlideLeft : Return Animation_SlideLeft(bmp, aktStep, maxStep)
                Case AnimationType.SlideUp : Return Animation_SlideUp(bmp, aktStep, maxStep)
                Case AnimationType.ScaleSlide : Return Animation_ScaleSlide(bmp, aktStep, maxStep)
                Case AnimationType.RotatePulse : Return Animation_RotatePulse(bmp, aktStep, maxStep)
                Case Else
                    Return bmp ' keine Animation
            End Select
        End Function



        ''' <summary>
        ''' Einfaches Schrumpfen
        ''' </summary>
        ''' <param name="bmp"></param>
        ''' <param name="aktStep"></param>
        ''' <param name="maxStep"></param>
        ''' <returns></returns>
        Public Function Animation_ScaleDown(bmp As Bitmap, aktStep As Integer, maxStep As Integer) As Bitmap
            Dim factor As Single = CSng(1.0F - (aktStep / maxStep))
            Return ScaleBitmap(bmp, factor, factor)
        End Function
        '
        ''' <summary>
        ''' Einfaches Wachsen
        ''' </summary>
        ''' <param name="bmp"></param>
        ''' <param name="aktStep"></param>
        ''' <param name="maxStep"></param>
        ''' <returns></returns>
        Public Function Animation_ScaleUp(bmp As Bitmap, aktStep As Integer, maxStep As Integer) As Bitmap
            Dim factor As Single = CSng(aktStep / maxStep)
            Return ScaleBitmap(bmp, factor, factor)
        End Function
        '
        ''' <summary>
        ''' Rotation
        ''' </summary>
        ''' <param name="bmp"></param>
        ''' <param name="aktStep"></param>
        ''' <param name="maxStep"></param>
        ''' <returns></returns>
        Public Function Animation_Rotate(bmp As Bitmap, aktStep As Integer, maxStep As Integer) As Bitmap
            Dim angle As Single = CSng(360.0F * (aktStep / maxStep))
            Return RotateBitmap(bmp, angle, 1.0F)
        End Function
        '
        ''' <summary>
        ''' Dehnen und Schrumpfen
        ''' </summary>
        ''' <param name="bmp"></param>
        ''' <param name="aktStep"></param>
        ''' <param name="maxStep"></param>
        ''' <returns></returns>
        Public Function Animation_RotateShrink(bmp As Bitmap, aktStep As Integer, maxStep As Integer) As Bitmap
            Dim progress As Single = CSng(aktStep / maxStep)
            Dim angle As Single = 360.0F * progress
            Dim scale As Single = 1.0F - progress
            Return RotateBitmap(bmp, angle, scale)
        End Function
        '
        ''' <summary>
        ''' Horizontaler Überschlag
        ''' </summary>
        ''' <param name="bmp"></param>
        ''' <param name="aktStep"></param>
        ''' <param name="maxStep"></param>
        ''' <returns></returns>
        Public Function Animation_FlipX(bmp As Bitmap, aktStep As Integer, maxStep As Integer) As Bitmap
            Dim factorX As Single = CSng(Math.Abs(Math.Cos((aktStep / maxStep) * Math.PI)))
            Return ScaleBitmap(bmp, factorX, 1.0F)
        End Function
        '
        ''' <summary>
        ''' Vertikaler Überschlag
        ''' </summary>
        ''' <param name="bmp"></param>
        ''' <param name="aktStep"></param>
        ''' <param name="maxStep"></param>
        ''' <returns></returns>
        Public Function Animation_FlipY(bmp As Bitmap, aktStep As Integer, maxStep As Integer) As Bitmap
            Dim factorY As Single = CSng(Math.Abs(Math.Cos((aktStep / maxStep) * Math.PI)))
            Return ScaleBitmap(bmp, 1.0F, factorY)
        End Function
        '
        ''' <summary>
        ''' Flip plus Schrumpfen
        ''' </summary>
        ''' <param name="bmp"></param>
        ''' <param name="aktStep"></param>
        ''' <param name="maxStep"></param>
        ''' <returns></returns>
        Public Function Animation_FlipShrink(bmp As Bitmap, aktStep As Integer, maxStep As Integer) As Bitmap
            Dim p As Single = CSng(aktStep / maxStep)
            Dim factorX As Single = CSng(Math.Abs(Math.Cos(p * Math.PI)))
            Dim scale As Single = 1.0F - p
            Return ScaleBitmap(bmp, factorX * scale, scale)
        End Function
        '
        ''' <summary>
        ''' Pulsieren
        ''' </summary>
        ''' <param name="bmp"></param>
        ''' <param name="aktStep"></param>
        ''' <param name="maxStep"></param>
        ''' <returns></returns>
        Public Function Animation_Pulse(bmp As Bitmap, aktStep As Integer, maxStep As Integer) As Bitmap
            Dim factor As Single = CSng(1.0F + 0.2F * Math.Sin((aktStep / maxStep) * Math.PI * 2))
            Return ScaleBitmap(bmp, factor, factor)
        End Function
        '
        ''' <summary>
        ''' Seitliches Wegschieben
        ''' </summary>
        ''' <param name="bmp"></param>
        ''' <param name="aktStep"></param>
        ''' <param name="maxStep"></param>
        ''' <returns></returns>
        Public Function Animation_SlideLeft(bmp As Bitmap, aktStep As Integer, maxStep As Integer) As Bitmap
            Dim offsetX As Integer = CInt((aktStep / maxStep) * bmp.Width)
            Return TranslateBitmap(bmp, -offsetX, 0)
        End Function
        '
        ''' <summary>
        ''' Nach oben wegschieben
        ''' </summary>
        ''' <param name="bmp"></param>
        ''' <param name="aktStep"></param>
        ''' <param name="maxStep"></param>
        ''' <returns></returns>
        Public Function Animation_SlideUp(bmp As Bitmap, aktStep As Integer, maxStep As Integer) As Bitmap
            Dim offsetY As Integer = CInt((aktStep / maxStep) * bmp.Height)
            Return TranslateBitmap(bmp, 0, -offsetY)
        End Function
        '
        ''' <summary>
        ''' Schrumpfen plus Wegschieben
        ''' </summary>
        ''' <param name="bmp"></param>
        ''' <param name="aktStep"></param>
        ''' <param name="maxStep"></param>
        ''' <returns></returns>
        Public Function Animation_ScaleSlide(bmp As Bitmap, aktStep As Integer, maxStep As Integer) As Bitmap
            Dim p As Single = CSng(aktStep / maxStep)
            Dim scale As Single = 1.0F - p
            Dim offsetX As Integer = CInt(p * bmp.Width)
            Return ScaleTranslateBitmap(bmp, scale, scale, offsetX, 0)
        End Function
        '
        ''' <summary>
        ''' Drehen und pulsieren
        ''' </summary>
        ''' <param name="bmp"></param>
        ''' <param name="aktStep"></param>
        ''' <param name="maxStep"></param>
        ''' <returns></returns>
        Public Function Animation_RotatePulse(bmp As Bitmap, aktStep As Integer, maxStep As Integer) As Bitmap
            Dim p As Single = CSng(aktStep / maxStep)
            Dim angle As Single = 360.0F * p
            Dim scale As Single = CSng(1.0F + 0.1F * Math.Sin(p * Math.PI * 4))
            Return RotateBitmap(bmp, angle, scale)
        End Function
        '
        '
        ''########
        'Toolkit Hilfsfunktionen

        Private Function ScaleBitmap(src As Bitmap, scaleX As Single, scaleY As Single) As Bitmap
            Dim w As Integer = Math.Max(1, CInt(src.Width * scaleX))
            Dim h As Integer = Math.Max(1, CInt(src.Height * scaleY))
            Dim newBmp As New Bitmap(w, h, PixelFormat.Format32bppPArgb)
            newBmp.MakeTransparent()
            Using g As Graphics = Graphics.FromImage(newBmp)
                g.InterpolationMode = INI.Rendering_InterpolationMode
                g.DrawImage(src, 0, 0, w, h)
            End Using
            Return newBmp
        End Function

        Private Function RotateBitmap(src As Bitmap, angle As Single, scale As Single) As Bitmap
            Dim w As Integer = src.Width
            Dim h As Integer = src.Height
            Dim newBmp As New Bitmap(w, h, PixelFormat.Format32bppPArgb)
            newBmp.MakeTransparent()
            Using g As Graphics = Graphics.FromImage(newBmp)
                g.InterpolationMode = INI.Rendering_InterpolationMode
                g.TranslateTransform(w / 2.0F, h / 2.0F)
                g.RotateTransform(angle)
                g.ScaleTransform(scale, scale)
                g.TranslateTransform(-w / 2.0F, -h / 2.0F)
                g.DrawImage(src, 0, 0, w, h)
            End Using
            Return newBmp
        End Function

        Private Function TranslateBitmap(src As Bitmap, offsetX As Integer, offsetY As Integer) As Bitmap
            Dim newBmp As New Bitmap(src.Width, src.Height, PixelFormat.Format32bppPArgb)
            newBmp.MakeTransparent()
            Using g As Graphics = Graphics.FromImage(newBmp)
                g.InterpolationMode = INI.Rendering_InterpolationMode
                g.DrawImage(src, offsetX, offsetY, src.Width, src.Height)
            End Using
            Return newBmp
        End Function

        Private Function ScaleTranslateBitmap(src As Bitmap, scaleX As Single, scaleY As Single, offsetX As Integer, offsetY As Integer) As Bitmap
            Dim w As Integer = Math.Max(1, CInt(src.Width * scaleX))
            Dim h As Integer = Math.Max(1, CInt(src.Height * scaleY))
            Dim newBmp As New Bitmap(src.Width, src.Height, PixelFormat.Format32bppPArgb)
            newBmp.MakeTransparent()
            Using g As Graphics = Graphics.FromImage(newBmp)
                g.InterpolationMode = INI.Rendering_InterpolationMode
                Dim x As Integer = (src.Width - w) \ 2 + offsetX
                Dim y As Integer = (src.Height - h) \ 2 + offsetY
                g.DrawImage(src, x, y, w, h)
            End Using
            Return newBmp
        End Function


#Region "Wegberechnungen"


        ''' <summary>
        ''' Punkt einer animierten Bahn zwischen pStart und pZiel.
        ''' bulge (Ausbeulung) = 0  -> gerade Linie.
        ''' bulge > 0  -> Krümmung "oben/rechts".
        ''' bulge kleiner 0 -> Krümmung "unten/links".
        ''' Ergebnis wird ins rectBorder geklemmt.
        ''' Für bulge ist ein Wert im Bereich ±(5–30 % der Streckenlänge) ist sinnvoll.
        ''' Berechnung über Bulge_Bezier mit Faktor 0.05 bis 0.3
        ''' </summary>
        Public Function AnimationPathPoint_Bezier(aktStep As Integer,
                                                 maxStep As Integer,
                                                 pStart As Point,
                                                 pZiel As Point,
                                                 rectBorder As Rectangle,
                                                 bulge As Integer) As Point

            ' t in [0..1] berechnen (robust gegen Grenzfälle)
            Dim t As Double
            If maxStep <= 0 Then
                t = 1.0
            Else
                Dim tt As Double = CDbl(aktStep) / CDbl(maxStep)
                If tt < 0.0 Then
                    t = 0.0
                ElseIf tt > 1.0 Then
                    t = 1.0
                Else
                    t = tt
                End If
            End If

            Dim sx As Double = pStart.X
            Dim sy As Double = pStart.Y
            Dim ex As Double = pZiel.X
            Dim ey As Double = pZiel.Y

            Dim dx As Double = ex - sx
            Dim dy As Double = ey - sy
            Dim len As Double = Math.Sqrt(dx * dx + dy * dy)

            Dim x As Double
            Dim y As Double

            If bulge = 0 OrElse len = 0.0 Then
                ' Gerade: lineare Interpolation
                x = sx + t * dx
                y = sy + t * dy
            Else
                ' Quadratische Bezierkurve mit Kontrollpunkt in der Mitte,
                ' um ausbeulung Pixel entlang der Senkrechten (dy, -dx) verschoben.
                Dim nx As Double = dy / len     ' Senkrechte: (dy, -dx) –> bei +ausbeulung: oben/rechts
                Dim ny As Double = -dx / len

                Dim mx As Double = (sx + ex) * 0.5
                Dim my As Double = (sy + ey) * 0.5

                Dim cx As Double = mx + CDbl(bulge) * nx
                Dim cy As Double = my + CDbl(bulge) * ny

                Dim omt As Double = 1.0 - t
                ' Quadratische Bezier-Formel
                x = (omt * omt) * sx + 2.0 * omt * t * cx + (t * t) * ex
                y = (omt * omt) * sy + 2.0 * omt * t * cy + (t * t) * ey
            End If

            ' Auf rectBorder klemmen (Right/Bottom sind exklusiv)
            Dim xi As Integer = CInt(Math.Round(x))
            Dim yi As Integer = CInt(Math.Round(y))

            If xi < rectBorder.Left Then xi = rectBorder.Left
            If yi < rectBorder.Top Then yi = rectBorder.Top

            Dim maxX As Integer = rectBorder.Right - 1
            Dim maxY As Integer = rectBorder.Bottom - 1

            If xi > maxX Then xi = maxX
            If yi > maxY Then yi = maxY

            Return New Point(xi, yi)

        End Function

        ''' <summary>
        ''' Berechnet eine sinnvolle Ausbeulung anhand der Distanz von Start und Ziel vor.
        ''' Der faktor ist der prozentualer Anteil der Streckenlänge (1 = 100%)
        ''' Siehe Function AnimationPathPoint_Bezier
        ''' </summary>
        ''' <param name="pStart">Startpunkt</param>
        ''' <param name="pZiel">Zielpunkt</param>
        ''' <param name="faktor">Prozentualer Anteil der Streckenlänge (Standard 0.15 = 15%)</param>
        ''' <param name="richtung">
        '''   +1 = Ausbeulung nach oben/rechts
        '''   -1 = Ausbeulung nach unten/links
        '''    0 = eine Gerade. 
        ''' </param>
        Public Function Bulge_Bezier(pStart As Point,
                                     pZiel As Point,
                                     faktor As Double,
                                     richtung As Integer) As Integer

            Dim dx As Double = pZiel.X - pStart.X
            Dim dy As Double = pZiel.Y - pStart.Y
            Dim len As Double = Math.Sqrt(dx * dx + dy * dy)

            If len <= 0 Then
                Return 0
            End If

            Dim value As Double = len * faktor
            Return CInt(Math.Round(value)) * Math.Sign(richtung)

        End Function


        Public Enum AnimStartPos
            EckeLinksOben
            EckeRechtsOben
            EckeLinksUnten
            EckeRechtsUnten
        End Enum



        ''' <summary>
        ''' Spiralbahn ("SnailShell"/"Spiral") zwischen Ecke und Mitte.
        ''' shrink=True  -> von Ecke nach Mitte.
        ''' shrink=False -> von Mitte nach Ecke.
        ''' drehwinkel >0 = Uhrzeigersinn, drehwinkel kleiner 0 = Gegenuhrzeigersinn.
        ''' </summary>
        Public Function AnimationPathPoint_SnailShell(aktStep As Integer,
                                                      maxStep As Integer,
                                                      rectOutside As Rectangle,
                                                      pos As AnimStartPos,
                                                      drehwinkel As Integer,
                                                      shrink As Boolean) As Point
            ' Normales t ∈ [0..1]
            Dim t As Double
            If maxStep <= 0 Then
                t = 1.0
            Else
                t = CDbl(Math.Max(0, Math.Min(maxStep, aktStep))) / CDbl(maxStep)
            End If

            ' Easing (SmoothStep)
            Dim te As Double = t * t * (3 - 2 * t)

            ' Mittelpunkt
            Dim cx As Double = rectOutside.Left + rectOutside.Width / 2.0
            Dim cy As Double = rectOutside.Top + rectOutside.Height / 2.0

            ' Eckpunkt
            Dim ex As Integer, ey As Integer
            Select Case pos
                Case AnimStartPos.EckeLinksOben
                    ex = rectOutside.Left : ey = rectOutside.Top
                Case AnimStartPos.EckeRechtsOben
                    ex = rectOutside.Right - 1 : ey = rectOutside.Top
                Case AnimStartPos.EckeLinksUnten
                    ex = rectOutside.Left : ey = rectOutside.Bottom - 1
                Case AnimStartPos.EckeRechtsUnten
                    ex = rectOutside.Right - 1 : ey = rectOutside.Bottom - 1
            End Select

            Dim dx0 As Double = ex - cx
            Dim dy0 As Double = ey - cy
            Dim r0 As Double = Math.Sqrt(dx0 * dx0 + dy0 * dy0)

            If r0 <= 0.0001 Then
                Return New Point(CInt(Math.Round(cx)), CInt(Math.Round(cy)))
            End If

            ' Gesamtwinkel
            Const Deg2Rad As Double = Math.PI / 180.0
            Dim totalAngle As Double = Math.Abs(drehwinkel) * Deg2Rad
            Dim sign As Integer = If(drehwinkel = 0, 1, Math.Sign(drehwinkel))

            ' Sehr kleiner Winkel -> Gerade
            If totalAngle < (1.0 * Deg2Rad) Then
                Dim xL As Double, yL As Double
                If shrink Then
                    ' Ecke → Mitte
                    xL = ex + (cx - ex) * te
                    yL = ey + (cy - ey) * te
                Else
                    ' Mitte → Ecke
                    xL = cx + (ex - cx) * te
                    yL = cy + (ey - cy) * te
                End If
                Return ClampToRect(New Point(CInt(Math.Round(xL)), CInt(Math.Round(yL))), rectOutside)
            End If

            ' Startwinkel (Orientierung vom Mittelpunkt zur Ecke)
            Dim theta0 As Double = Math.Atan2(dy0, dx0)

            Dim theta As Double
            Dim r As Double

            If shrink Then
                ' Ecke → Mitte
                theta = theta0 + sign * te * totalAngle
                r = r0 * (1 - te)
            Else
                ' Mitte → Ecke
                theta = theta0 - sign * (1 - te) * totalAngle
                r = r0 * te
            End If

            Dim x As Double = cx + r * Math.Cos(theta)
            Dim y As Double = cy + r * Math.Sin(theta)

            Return ClampToRect(New Point(CInt(Math.Round(x)), CInt(Math.Round(y))), rectOutside)
        End Function

        Private Function ClampToRect(p As Point, r As Rectangle) As Point
            Dim x As Integer = p.X
            Dim y As Integer = p.Y
            If x < r.Left Then x = r.Left
            If y < r.Top Then y = r.Top
            Dim maxX As Integer = r.Right - 1
            Dim maxY As Integer = r.Bottom - 1
            If x > maxX Then x = maxX
            If y > maxY Then y = maxY
            Return New Point(x, y)
        End Function


#End Region

        ''' <summary>
        ''' Zeichnet ein Bild zentriert um <paramref name="center"/> rotiert und optional skaliert.
        ''' shrink:=True  → Start in Vollgröße, skaliert bis 0 bei |winkel|=|maxwinkel|.
        ''' shrink:=False → Start bei 0, wächst bis Vollgröße bei |winkel|=|maxwinkel|.
        ''' Die Rotation selbst entspricht <paramref name="winkel"/> (in Grad).
        ''' Kein temporäres Erzeugen/Rotieren von Bitmaps → keine Flacker-/Performance-Probleme.
        ''' </summary>
        Public Sub DrawRotatedCenteredScaled(g As Graphics,
                                             img As Image,
                                             center As PointF,
                                             baseSize As Size,
                                             winkel As Single,
                                             maxwinkel As Single,
                                             Optional shrink As Boolean = False,
                                             Optional easeScale As Boolean = True)

            If img Is Nothing OrElse baseSize.Width <= 0 OrElse baseSize.Height <= 0 Then Exit Sub

            ' Fortschritt p ∈ [0..1] aus Winkel ableiten (robust gegen 0/negativ)
            Dim denom As Single = Math.Abs(maxwinkel)
            Dim p As Double = If(denom <= 0.0001F, 1.0R, Math.Min(1.0R, CDbl(Math.Abs(winkel) / denom)))

            ' Easing für die Skalierung sorgt für sanftes Starten/Stoppen der Größenänderung.
            Dim pe As Double = If(easeScale, p * p * (3 - 2 * p), p)

            ' Skalenfaktor s bestimmen
            Dim s As Double = If(shrink, 1.0R - pe, pe)

            ' Ganz kleinen Maßstab als "unsichtbar" behandeln (vermeidet GDI-Probleme)
            If s <= 0.001R Then Exit Sub

            ' Render-Qualitäten (nur hier lokal ändern)
            Dim oldSmoothing As SmoothingMode = g.SmoothingMode
            Dim oldInterp As InterpolationMode = g.InterpolationMode
            Dim oldPixel As PixelOffsetMode = g.PixelOffsetMode
            g.SmoothingMode = SmoothingMode.HighQuality
            g.InterpolationMode = INI.Rendering_InterpolationMode
            g.PixelOffsetMode = PixelOffsetMode.Half

            ' Transform: Mittelpunkt → Rotation → Skalierung → Bild links/oben auf (-w/2, -h/2)
            Dim w As Single = CSng(baseSize.Width)
            Dim h As Single = CSng(baseSize.Height)

            Dim m As New Matrix()
            m.Translate(center.X, center.Y, MatrixOrder.Append)
            m.Rotate(winkel, MatrixOrder.Append)
            m.Scale(CSng(s), CSng(s), MatrixOrder.Append)
            m.Translate(-w / 2.0F, -h / 2.0F, MatrixOrder.Append)

            Dim oldTransform As Drawing2D.Matrix = g.Transform
            g.Transform = m

            ' Zeichnen in Basisgröße – Skalierung erledigt die Transform
            g.DrawImage(img, New RectangleF(0, 0, w, h))

            ' Aufräumen
            g.Transform = oldTransform
            g.SmoothingMode = oldSmoothing
            g.InterpolationMode = oldInterp
            g.PixelOffsetMode = oldPixel
            m.Dispose()
        End Sub

        ''' <summary>
        ''' Liefert die rotierte Bounding-Box-Größe eines breite x höhe Rechtecks bei Rotation um angleDeg (um den Mittelpunkt).
        ''' </summary>
        Public Function GetRotatedSize(size As Size, angleDeg As Double) As Size
            Dim w As Double = size.Width
            Dim h As Double = size.Height
            Dim a As Double = angleDeg * Math.PI / 180.0
            Dim c As Double = Math.Cos(a)
            Dim s As Double = Math.Sin(a)
            ' Achs-parallele Umhüllung:
            Dim rotW As Double = Math.Abs(w * c) + Math.Abs(h * s)
            Dim rotH As Double = Math.Abs(w * s) + Math.Abs(h * c)
            ' +0.5 für saubere Rundung auf Pixelränder
            Return New Size(CInt(Math.Floor(rotW + 0.5)), CInt(Math.Floor(rotH + 0.5)))
        End Function

        ''' <summary>
        ''' Kleinste quadratische Bounding-Box, die ein  breite x höhe Rechteck bei beliebiger Rotation
        ''' (um den Mittelpunkt) vollständig enthält. Entspricht der Rechteck-Diagonale.
        ''' </summary>
        Public Function GetRotatedSizeMaximum(size As Size) As Size
            Dim w As Double = size.Width
            Dim h As Double = size.Height
            Dim diag As Double = Math.Sqrt(w * w + h * h)

            ' Ceiling = sicherheitsorientiert (niemals zu klein abrunden)
            Dim side As Integer = CInt(Math.Ceiling(diag))
            Return New Size(side, side)
        End Function

        ''' <summary>
        ''' Berechnet die linke obere Ecke der (vorgedrehten) Bitmap, deren Mittelpunkt auf center liegen soll.
        ''' Optionales Clamping sorgt dafür, dass die ganze Bitmap innerhalb rectBorder liegt.
        ''' </summary>
        Public Function GetRotatedTopLeft(center As PointF,
                                          sizeStein As Size,
                                          angleDeg As Double,
                                          Optional rectBorder As Rectangle = Nothing,
                                          Optional clampInside As Boolean = False) As Point

            Dim rotSize As Size = GetRotatedSize(sizeStein, angleDeg)

            Dim x As Double = center.X - rotSize.Width / 2.0
            Dim y As Double = center.Y - rotSize.Height / 2.0

            If clampInside AndAlso rectBorder <> Rectangle.Empty Then
                ' So klemmen, dass die GESAMTE Bitmap im Rechteck bleibt:
                Dim minX As Integer = rectBorder.Left
                Dim minY As Integer = rectBorder.Top
                Dim maxX As Integer = rectBorder.Right - rotSize.Width   ' Right/Bottom sind exklusiv
                Dim maxY As Integer = rectBorder.Bottom - rotSize.Height

                If x < minX Then x = minX
                If y < minY Then y = minY
                If x > maxX Then x = maxX
                If y > maxY Then y = maxY
            End If

            ' Runden: Floor vermeidet 1px-Wackler stärker als Round
            Return New Point(CInt(Math.Floor(x)), CInt(Math.Floor(y)))
        End Function

    End Module
End Namespace