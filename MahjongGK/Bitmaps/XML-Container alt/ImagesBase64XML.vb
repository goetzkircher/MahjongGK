'
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
'
Imports System.Drawing.Drawing2D
Imports System.Drawing.Imaging
Imports System.IO
Imports System.Xml.Serialization

#Disable Warning IDE0079
#Disable Warning IDE1006

Namespace Images

    Public Class ImagesBase64XML

        ''' <summary>
        ''' Das sind die Images in den xml-Dateien
        ''' </summary>
        Public ImagesB64(42) As String

        <XmlIgnore>
        Private ReadOnly _OriginalBitmaps(42) As Bitmap

        <XmlIgnore>
        Private ReadOnly _ScaledBitmaps(42) As Bitmap

        <XmlIgnore>
        Private _ScaledWidth As Integer = -1

        <XmlIgnore>
        Private _ScaledHeight As Integer = -1

        <XmlIgnore>
        Private _LastQualityFlag As Boolean = False

        Public Property ImagesBase64(idx As Integer) As String
            Get
                Return ImagesB64(idx)
            End Get
            Set(value As String)
                ImagesB64(idx) = value
            End Set
        End Property

        ''Public ReadOnly Property BitmapWidth As Integer
        ''    Get
        ''        If IsNothing(_OriginalBitmaps(0)) Then
        ''            Return -1
        ''        Else
        ''            Return _OriginalBitmaps(0).Width
        ''        End If
        ''    End Get
        ''End Property
        ''Public ReadOnly Property BitmapHeight As Integer
        ''    Get
        ''        If IsNothing(_OriginalBitmaps(0)) Then
        ''            Return -1
        ''        Else
        ''            Return _OriginalBitmaps(0).Height
        ''        End If
        ''    End Get
        ''End Property


        ''' <summary>
        ''' Skaliert alle Bilder auf eine neue Zielgröße.
        ''' Die Dummybitmaps werden nur während der Entwicklungszeit benötigt,
        ''' um zu überprüfen, ob eine Grafik auf unsichtbar geschaltet ist
        ''' und um Programmierfehler zu entdecken.
        ''' </summary>
        Public Sub CreateImages(width As Integer, height As Integer, qualität As Boolean, idxSteinStatus As Integer, createDummyBmps As Boolean)

            If width <= 0 OrElse height <= 0 Then
                Exit Sub
            End If

            If _ScaledWidth = width AndAlso _ScaledHeight = height AndAlso qualität = _LastQualityFlag Then
                Exit Sub
            End If

            If createDummyBmps Then
                If IfRunningInIDE_ShowAllStones Then
                    For idxStein As Integer = 0 To 42

                        _ScaledBitmaps(idxStein)?.Dispose()
                        _ScaledBitmaps(idxStein) = New Bitmap(width, height, PixelFormat.Format32bppArgb)

                        Using gfx As Graphics = Graphics.FromImage(_ScaledBitmaps(idxStein))
                            gfx.Clear(Color.FromArgb(128, 128, 128, 128))

                            ' Durchmesser = 860% der kleineren Kantenlänge
                            Dim d As Single = 0.6F * Math.Min(width, height)

                            ' Mittelpunkt der Grafik
                            Dim centerX As Single = width / 2.0F * 0.85F
                            Dim centerY As Single = height / 2.0F * 0.85F

                            ' Linke obere Ecke des umschreibenden Rechtecks berechnen
                            Dim x As Single = centerX - d / 2.0F
                            Dim y As Single = centerY - d / 2.0F

                            ' Kreis zeichnen
                            If idxSteinStatus = SteinStatus.I00Unsichtbar Then
                                'Das sind die nicht mehr sichtbaren Steine, also die entnommenen Steine,
                                'die zu Kontrollzwecke innerhalb der IDE sichtbar gemacht werden können.
                                Using pen As New Pen(Color.Red, 3)
                                    gfx.DrawEllipse(pen, x, y, d, d)
                                End Using

                            ElseIf idxSteinStatus = SteinStatus.I10Reserve1 Then
                                'Das gilt auch für die Reservesteine
                                Using pen As New Pen(Color.Green, 3)
                                    gfx.DrawEllipse(pen, x, y, d, d)
                                End Using

                            ElseIf idxSteinStatus = SteinStatus.I11Reserve2 Then
                                Using pen As New Pen(Color.Blue, 3)
                                    gfx.DrawEllipse(pen, x, y, d, d)
                                End Using
                            End If


                        End Using
                    Next

                    Exit Sub
                Else
                    For i As Integer = 0 To 42
                        _ScaledBitmaps(i)?.Dispose()
                        _ScaledBitmaps(i) = Nothing
                    Next
                End If
            End If



            For idxStein As Integer = 0 To 42

                Dim img As Bitmap = GetOriginalBitmap(idxStein)

                'Hier ist die tiefst-mögliche Stelle im Programm zur Feststellung der
                'Abmessung der Steine.
                If idxStein = 1 AndAlso idxSteinStatus = 1 Then 'die Erste der normalen Grafiken
                    '                                            (idxSteinStatus = 0 sind generierte Grafiken
                    '                                            und idxStein = 0 ist die Fehlergrafik)
                    INI.Rendering_OrgGrafikSizeWidth = img.Width
                    INI.Rendering_OrgGrafikSizeHeight = img.Height
                End If


                If img IsNot Nothing Then
                    _ScaledBitmaps(idxStein)?.Dispose()
                    _ScaledBitmaps(idxStein) = ResizeBitmap(img, width, height, qualität)

                    If idxSteinStatus = SteinStatus.I08WerkstückEinfügeFehler Then
                        SteinOverlays.InsertGrafik(_ScaledBitmaps(idxStein), "Error")
                        ' SteinOverlays.InsertOverlayGrafik(_ScaledBitmaps(idxStein), OverlayStyle.Warnung)
                    ElseIf idxSteinStatus = SteinStatus.I09WerkstückZufallsgrafik Then
                        SteinOverlays.InsertGrafik(_ScaledBitmaps(idxStein), "NoGo")
                        'SteinOverlays.InsertOverlayGrafik(_ScaledBitmaps(idxStein), OverlayStyle.Blockiert)
                    End If

                    'If idxStein = 0 Then
                    '    InsertErrorGrafik(_ScaledBitmaps(idxStein))
                    'End If


                End If
            Next


            _ScaledWidth = width
            _ScaledHeight = height
            _LastQualityFlag = qualität

        End Sub

        ''' <summary>
        ''' Gibt ein skaliertes Bild zurück (nach vorherigem Aufruf von CreateImages).
        ''' </summary>
        Public Function GetImage(idx As Integer) As Bitmap
            If idx < 0 OrElse idx > 42 Then
                Return Nothing
            End If
            Return _ScaledBitmaps(idx)
        End Function

        Public Function GetScaledBitmaps() As Bitmap()
            Return _ScaledBitmaps
        End Function

        ''' <summary>
        ''' Gibt das Originalbild zurück (nicht skaliert).
        ''' </summary>
        Public Function GetOriginalBitmap(idx As Integer) As Bitmap
            If idx < 0 OrElse idx > 42 Then
                Return Nothing
            End If
            If _OriginalBitmaps(idx) Is Nothing AndAlso Not String.IsNullOrEmpty(ImagesB64(idx)) Then
                Try

                    Using ms As New IO.MemoryStream(Convert.FromBase64String(ImagesB64(idx)))
                        Dim bf As New Runtime.Serialization.Formatters.Binary.BinaryFormatter
                        Dim img As Image = CType(bf.Deserialize(ms), Image)
                        _OriginalBitmaps(idx) = New Bitmap(img)  ' Direkt als Bitmap speichern
                        img.Dispose()
                    End Using
                Catch
                    ' Fehlerhafte Grafik ersetzen
                    _OriginalBitmaps(idx) = New Bitmap(198, 252)
                    Using g As Graphics = Graphics.FromImage(_OriginalBitmaps(idx))
                        g.Clear(Color.Fuchsia)
                    End Using
                End Try
            End If
            Return _OriginalBitmaps(idx)
        End Function

        Public Function GetOriginalBitmaps() As Bitmap()
            Return _OriginalBitmaps
        End Function
        '
        ''' <summary>
        ''' Bild skalieren mit einstellbarer Qualität.
        ''' </summary>
        Private Function ResizeBitmap(src As Image, targetWidth As Integer, targetHeight As Integer, highQuality As Boolean) As Bitmap
            Dim dest As New Bitmap(targetWidth, targetHeight, PixelFormat.Format32bppPArgb)
            Using g As Graphics = Graphics.FromImage(dest)
                g.InterpolationMode = If(highQuality, InterpolationMode.HighQualityBicubic, InterpolationMode.HighQualityBilinear)
                g.PixelOffsetMode = PixelOffsetMode.HighQuality
                g.SmoothingMode = SmoothingMode.HighSpeed
                g.CompositingQuality = CompositingQuality.HighSpeed
                g.DrawImage(src, 0, 0, targetWidth, targetHeight)
            End Using
            Return dest
        End Function



        ''' <summary>
        ''' Läd die Xml mit den Images der Spielsteine.
        ''' </summary>
        Public Shared Function Load(ByVal zLwpD As String) As ImagesBase64XML
            Try
                Dim zString As String = FileIO.FileSystem.ReadAllText(zLwpD)
                Dim xs As New XmlSerializer(GetType(ImagesBase64XML))
                Dim Daten As ImagesBase64XML = CType(xs.Deserialize(New StringReader(zString)), ImagesBase64XML)
                Return Daten
            Catch ex As Exception
                Return Nothing
            End Try
        End Function




    End Class

End Namespace