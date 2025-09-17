'
' SPDX-License-Identifier: GPL-3.0-or-later
'###########################################################################
'#                                                                         #
'#   Copyright © 2025–2026 Götz Kircher <MahjongGk@t-online.de>            #
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
Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On
Imports System.Drawing.Drawing2D
Imports System.Drawing.Imaging
'
Imports System.IO

Public Class UctlToolboxHintergrund

    Private Sub UctlToolboxHintergrund_Load(sender As Object, e As EventArgs) Handles Me.Load

    End Sub

    Private Sub btnToolboxHGrdSpfldBitmapFallback_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub btnToolboxHGrdSpfldBitmapClearFallback_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub btnToolboxHGrdEditorBitmapFallback_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub btnToolboxHGrdEditorBitmapClearFallback_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub btnToolboxHGrdSpfldColorFallback_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub btnToolboxHGrdSpfldColorClearFallback_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub btnToolboxHGrdEditorColorFallback_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub btnToolboxHGrdEditorColorClearFallback_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub btnToolboxHGrdSpfldBitmap_Click(sender As Object, e As EventArgs) Handles btnToolboxHGrdSpfldBitmap.Click

    End Sub

    Private Sub btnToolboxHGrdSpfldBitmapClear_Click(sender As Object, e As EventArgs) Handles btnToolboxHGrdSpfldBitmapClear.Click

    End Sub

    Private Sub btnToolboxHGrdEditorBitmap_Click(sender As Object, e As EventArgs) Handles btnToolboxHGrdEditorBitmap.Click

    End Sub

    Private Sub btnToolboxHGrdEditorBitmapClear_Click(sender As Object, e As EventArgs) Handles btnToolboxHGrdEditorBitmapClear.Click

    End Sub

    Private Sub btnToolboxHGrdSpfldColor_Click(sender As Object, e As EventArgs) Handles btnToolboxHGrdSpfldColor.Click

    End Sub

    Private Sub btnToolboxHGrdSpfldColorClear_Click(sender As Object, e As EventArgs) Handles btnToolboxHGrdSpfldColorClear.Click

    End Sub

    Private Sub btnToolboxHGrdEditorColor_Click(sender As Object, e As EventArgs) Handles btnToolboxHGrdEditorColor.Click

    End Sub

    Private Sub btnToolboxHGrdEditorColorClear_Click(sender As Object, e As EventArgs) Handles btnToolboxHGrdEditorColorClear.Click

    End Sub

    Public Sub InitialisierungAndUpdate()
        CopyIniToControl()
    End Sub


    Private Sub CopyIniToControl()
        '
        With lblToolboxHGrdSpfldColor
            If INI.Toolbox_HGrdSpfldColor = Color.Empty Then
                .ForeColor = SystemColors.GrayText
                .BackColor = Color.Transparent
                .Text = "(keine Farbe gewählt)"
            Else
                .Text = String.Empty
                .BackColor = INI.Toolbox_HGrdSpfldColor
            End If
        End With
        '
        With lblToolboxHGrdSpfldColorFallback
            If INI.Toolbox_HGrdSpfldColorFallback = Color.Empty Then
                .ForeColor = SystemColors.GrayText
                .BackColor = Color.Transparent
                .Text = "(keine Farbe gewählt)"
            Else
                .Text = String.Empty
                .BackColor = INI.Toolbox_HGrdSpfldColorFallback
            End If
        End With
        '
        With lblToolboxHGrdEditorColor
            If INI.Toolbox_HGrdEditorColor = Color.Empty Then
                .ForeColor = SystemColors.GrayText
                .BackColor = Color.Transparent
                .Text = "(keine Farbe gewählt)"
            Else
                .Text = String.Empty
                .BackColor = INI.Toolbox_HGrdEditorColor
            End If
        End With
        '
        With lblToolboxHGrdEditorColorFallback
            If INI.Toolbox_HGrdEditorColorFallback = Color.Empty Then
                .ForeColor = SystemColors.GrayText
                .BackColor = Color.Transparent
                .Text = "(keine Farbe gewählt)"
            Else
                .Text = String.Empty
                .BackColor = INI.Toolbox_HGrdEditorColorFallback
            End If
        End With
        '





        'INI.Toolbox_HGrdEditorColorFallback  
        'INI.Toolbox_HGrdSpfldBitmapFallback  
        'INI.Toolbox_HGrdEditorBitmapFallback 
        'INI.Toolbox_HGrdSpfldBitmapIsUserGrafikFallback  
        'INI.Toolbox_HGrdEditorBitmapIsUserGrafikFallback  

    End Sub

    Private Sub CopyControlToIni()
        'INI.Toolbox_HGrdSpfldColorFallback  
        'INI.Toolbox_HGrdEditorColorFallback  
        'INI.Toolbox_HGrdSpfldBitmapFallback  
        'INI.Toolbox_HGrdEditorBitmapFallback 
        'INI.Toolbox_HGrdSpfldBitmapIsUserGrafikFallback  
        'INI.Toolbox_HGrdEditorBitmapIsUserGrafikFallback  

    End Sub

    ' HINWEIS:
    ' - Erwartet vorhandene INI.AppDataDirectory(AppDataSubDir.*)
    ' - Erwartet die vom Nutzer bereitgestellte ShrinkBitmap(ByRef bmp, newW, newH, ...)
    ' - Rückgabebitmap gehört dem Aufrufer (Dispose nicht vergessen)

    ''' <summary>
    ''' Liefert ein 150x100-geeignetes Mini-Thumb als <see cref="Bitmap"/>.
    ''' Quelle: 
    '''  - isUserGrafik = False → INI.AppDataDirectory(AppDataSubDir.Hintergrundgrafiken)
    '''  - isUserGrafik = True  → INI.AppDataDirectory(AppDataSubDir.Eigene_Hintergrundgrafiken)
    ''' Es wird – falls nötig – das Postfix ".thb.png" an <paramref name="name"/> angehängt.
    ''' Existiert die Datei nicht oder tritt ein Fehler auf, wird eine neutrale Fehlerminiatur erzeugt.
    ''' </summary>
    Public Function GetMiniThumb(name As String, isUserGrafik As Boolean) As Bitmap
        ' Zielgröße für die PictureBox
        Const MaxW As Integer = 150
        Const MaxH As Integer = 100

        Try
            ' 1) Eingabe prüfen
            If String.IsNullOrWhiteSpace(name) Then
                Return BuildErrorMiniThumb(New Size(MaxW, MaxH))
            End If

            ' 2) Quellverzeichnis wählen
            Dim baseDir As String = If(isUserGrafik,
            INI.AppDataDirectory(AppDataSubDir.Eigene_Hintergrundgrafiken),
            INI.AppDataDirectory(AppDataSubDir.Hintergrundgrafiken))

            ' 3) Dateiname mit ".thb.png" erweitern
            Dim fileName As String = name & ".thb.png"

            ' 4) Vollständigen Pfad bilden.
            Dim fullPath As String = Path.Combine(baseDir, fileName)

            ' 5) Existenz prüfen
            If Not File.Exists(fullPath) Then
                Return BuildErrorMiniThumb(New Size(MaxW, MaxH))
            End If

            ' 6) Laden
            Dim bmp As Bitmap = Nothing
            Try
                ' Wichtig: Image.FromFile hält Datei-Handle; daher in Memory kopieren
                Using tmp As Image = Image.FromFile(fullPath)
                    bmp = New Bitmap(tmp) ' nun losgelöst von der Datei
                End Using
            Catch
                ' Laden fehlgeschlagen -> Fehlerminiatur
                Return BuildErrorMiniThumb(New Size(MaxW, MaxH))
            End Try

            ' 7) Auf 150x100 einpassen (Seitenverhältnis beibehalten, nur Schrumpfen)
            Dim tw As Integer = bmp.Width
            Dim th As Integer = bmp.Height

            Dim scaleW As Double = CDbl(MaxW) / Math.Max(1, tw)
            Dim scaleH As Double = CDbl(MaxH) / Math.Max(1, th)
            Dim scale As Double = Math.Min(1.0R, Math.Min(scaleW, scaleH)) ' Nur verkleinern, nie vergrößern

            Dim newW As Integer = CInt(Math.Floor(tw * scale))
            Dim newH As Integer = CInt(Math.Floor(th * scale))

            If newW <= 0 OrElse newH <= 0 Then
                bmp.Dispose()
                Return BuildErrorMiniThumb(New Size(MaxW, MaxH))
            End If

            ' Nur schrumpfen, wenn nötig
            If newW < tw OrElse newH < th Then
                ' Verwendet die vom Nutzer bereitgestellte Schrumpf-Funktion (in-place über ByRef)
                Dim ok As Boolean = MjGDI.ShrinkBitmap(bmp, newW, newH, disposeSrc:=True, highQuality:=True, cvtToARGBBitmap:=True)
                If Not ok OrElse bmp Is Nothing Then
                    ' Falls etwas schiefging, sichere Fehlerminiatur zurückgeben
                    Return BuildErrorMiniThumb(New Size(MaxW, MaxH))
                End If
            End If

            ' 8) Ergebnis zurück
            Return bmp

        Catch
            ' Unerwarteter Fehler → sichere Fehlerminiatur
            Return BuildErrorMiniThumb(New Size(MaxW, MaxH))
        End Try
    End Function


    ''' <summary>
    ''' Erzeugt eine neutrale 150x100 (oder angegebene Größe) Fehlerminiatur:
    ''' neutralgrauer Hintergrund, zentriertes SystemIcons.Warning.
    ''' </summary>
    Private Function BuildErrorMiniThumb(sz As Size) As Bitmap
        Dim bmp As New Bitmap(Math.Max(1, sz.Width), Math.Max(1, sz.Height), PixelFormat.Format32bppArgb)

        Using g As Graphics = Graphics.FromImage(bmp),
          iconBmp As Bitmap = SystemIcons.Warning.ToBitmap()

            g.SmoothingMode = SmoothingMode.AntiAlias
            g.InterpolationMode = InterpolationMode.HighQualityBicubic
            g.PixelOffsetMode = PixelOffsetMode.HighQuality
            g.CompositingQuality = CompositingQuality.HighQuality

            ' Neutralgrau (nicht zu dunkel, nicht zu hell)
            Using bg As New SolidBrush(Color.FromArgb(216, 216, 216))
                g.FillRectangle(bg, 0, 0, bmp.Width, bmp.Height)
            End Using

            ' Icon proportional maximal ~60% der kürzeren Kante
            Dim maxIcon As Integer = CInt(Math.Floor(Math.Min(bmp.Width, bmp.Height) * 0.6R))
            maxIcon = Math.Max(16, maxIcon)

            ' SystemIcons.Warning hat typ. 32x32 / 48x48 – proportional einpassen
            Dim scale As Double = Math.Min(CDbl(maxIcon) / iconBmp.Width, CDbl(maxIcon) / iconBmp.Height)
            Dim iw As Integer = CInt(Math.Floor(iconBmp.Width * scale))
            Dim ih As Integer = CInt(Math.Floor(iconBmp.Height * scale))

            Dim x As Integer = (bmp.Width - iw) \ 2
            Dim y As Integer = (bmp.Height - ih) \ 2

            g.DrawImage(iconBmp, New Rectangle(x, y, iw, ih))
        End Using

        Return bmp
    End Function

    Private Sub lblToolboxHGrdSpfldColor_Click(sender As Object, e As EventArgs) Handles lblToolboxHGrdSpfldColor.Click

    End Sub

    Private Sub lblToolboxHGrdEditorColor_Click(sender As Object, e As EventArgs)

    End Sub
End Class
