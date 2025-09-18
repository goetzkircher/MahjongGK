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

#Disable Warning IDE0079
#Disable Warning IDE1006

Imports System.Drawing.Drawing2D
Imports System.Drawing.Imaging
'
Imports System.IO

Public Class UctlToolboxHintergrund



    Public Sub InitialisierungAndUpdate()
        CopyIniToControl()
    End Sub


    Private _aktSFI As New SpielfeldInfo
    Private _AktDstIsSpielfeld As Boolean

    Private Sub SetAktSpielfeldInfo()
        If INI.Global_AktVisibleUserControl = VisibleUserControl.Editor Then
            _aktSFI = Spielfeld.EditorSpielfeldInfo
            _AktDstIsSpielfeld = False

        ElseIf INI.Global_AktVisibleUserControl = VisibleUserControl.Werkbank Then
            _aktSFI = Spielfeld.EditorSpielfeldInfo
            _AktDstIsSpielfeld = False

        Else
            _aktSFI = Spielfeld.SpielerSpielfeldInfo
            _AktDstIsSpielfeld = True
        End If
    End Sub

    Private Sub CopyIniToControl()

        SetAktSpielfeldInfo()

        If _AktDstIsSpielfeld Then
            gbxAktSpiel.Text = "Daten aus aktuell geladenem Spielfeld"
        Else
            gbxAktSpiel.Text = "Daten aus aktuell geladenem Editor"
        End If

        Const nocol As String = "Keine Farbe gewählt"
        '
        With lblToolboxHGrdSpfldColor
            If _aktSFI.Toolbox_HGrdSpfldColor = Color.Empty Then
                .ForeColor = SystemColors.GrayText
                .BackColor = Color.Transparent
                .Text = nocol
            Else
                .Text = String.Empty
                .BackColor = _aktSFI.Toolbox_HGrdSpfldColor
            End If
        End With
        '
        With lblToolboxHGrdSpfldColorFallback
            If INI.Toolbox_HGrdSpfldColorFallback = Color.Empty Then
                .ForeColor = SystemColors.GrayText
                .BackColor = Color.Transparent
                .Text = nocol
            Else
                .Text = String.Empty
                .BackColor = INI.Toolbox_HGrdSpfldColorFallback
            End If
        End With
        '
        With lblToolboxHGrdEditorColor
            If _aktSFI.Toolbox_HGrdEditorColor = Color.Empty Then
                .ForeColor = SystemColors.GrayText
                .BackColor = Color.Transparent
                .Text = nocol
            Else
                .Text = String.Empty
                .BackColor = _aktSFI.Toolbox_HGrdEditorColor
            End If
        End With
        '
        With lblToolboxHGrdEditorColorFallback
            If INI.Toolbox_HGrdEditorColorFallback = Color.Empty Then
                .ForeColor = SystemColors.GrayText
                .BackColor = Color.Transparent
                .Text = nocol
            Else
                .Text = String.Empty
                .BackColor = INI.Toolbox_HGrdEditorColorFallback
            End If
        End With
        '

        picToolboxHGrdSpfld.Image = GetMiniThumb(_aktSFI.Toolbox_HGrdSpfldBitmapName, _aktSFI.Toolbox_HGrdSpfldBitmapIsUserGrafik)
        picToolboxHGrdSpfldFallback.Image = GetMiniThumb(INI.Toolbox_HGrdSpfldBitmapNameFallback, INI.Toolbox_HGrdSpfldBitmapIsUserGrafikFallback)

        picToolboxHGrdEditor.Image = GetMiniThumb(_aktSFI.Toolbox_HGrdEditorBitmapName, _aktSFI.Toolbox_HGrdEditorBitmapIsUserGrafik)
        picToolboxHGrdEditorFallback.Image = GetMiniThumb(INI.Toolbox_HGrdEditorBitmapNameFallback, INI.Toolbox_HGrdEditorBitmapIsUserGrafikFallback)

        lblToolboxHGrdSpfldRenderMode.Text = GetTextFromBirm(_aktSFI.Toolbox_HGrdSpfldRenderMode)
        lblToolboxHGrdEditorRenderMode.Text = GetTextFromBirm(_aktSFI.Toolbox_HGrdEditorRenderMode)

        lblToolboxHGrdSplfldRenderModeFallback.Text = GetTextFromBirm(INI.Toolbox_HGrdSpfldRenderModeFallback)
        lblToolboxHGrdEditorRenderModeFallback.Text = GetTextFromBirm(INI.Toolbox_HGrdEditorRenderModeFallBack)

        chkToolboxHGrdEditorUseSpfldEinstlg.Checked = _aktSFI.Toolbox_HGrdEditorUseSpfldValues
        chkToolboxHGrdEditorUseSpfldEinstlgFallback.Checked = INI.Toolbox_HGrdEditorUseSplfldEinstlgFallback

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
    Private Function GetMiniThumb(name As String, isUserGrafik As Boolean) As Bitmap
        ' Zielgröße für die PictureBox (Editor-Thumb; symmetrisch genug für beide)
        Dim maxW As Integer = picToolboxHGrdEditor.Width
        Dim maxH As Integer = picToolboxHGrdEditor.Height

        Try
            ' 1) Eingabe prüfen
            If String.IsNullOrWhiteSpace(name) Then
                ' NEU: neutrales, graues Platzhalterbild mit Hinweistext
                Return BuildNoSelectionMiniThumb(New Size(maxW, maxH), Me.Font, Me.BackColor)
            End If

            ' 2) Quellverzeichnis wählen
            Dim baseDir As String = If(isUserGrafik,
                                   INI.AppDataDirectory(AppDataSubDir.Eigene_Hintergrundgrafiken),
                                   INI.AppDataDirectory(AppDataSubDir.Hintergrundgrafiken))

            ' 3) Dateiname mit ".thb.png" erweitern
            Dim fileName As String = name & ".thb.png"

            ' 4) Vollständigen Pfad bilden
            Dim fullPath As String = Path.Combine(baseDir, fileName)

            ' 5) Existenz prüfen
            If Not File.Exists(fullPath) Then
                Return BuildErrorMiniThumb(New Size(maxW, maxH))
            End If

            ' 6) Laden (Handle-frei machen)
            Dim bmp As Bitmap = Nothing
            Try
                Using tmp As Image = Image.FromFile(fullPath)
                    bmp = New Bitmap(tmp)
                End Using
            Catch
                Return BuildErrorMiniThumb(New Size(maxW, maxH))
            End Try

            ' 7) Auf Ziel einpassen (nur schrumpfen)
            Dim tw As Integer = bmp.Width
            Dim th As Integer = bmp.Height

            Dim scaleW As Double = CDbl(maxW) / Math.Max(1, tw)
            Dim scaleH As Double = CDbl(maxH) / Math.Max(1, th)
            Dim scale As Double = Math.Min(1.0R, Math.Min(scaleW, scaleH)) ' Nur verkleinern

            Dim newW As Integer = CInt(Math.Floor(tw * scale))
            Dim newH As Integer = CInt(Math.Floor(th * scale))

            If newW <= 0 OrElse newH <= 0 Then
                bmp.Dispose()
                Return BuildErrorMiniThumb(New Size(maxW, maxH))
            End If

            If newW < tw OrElse newH < th Then
                Dim ok As Boolean = MjGDI.ShrinkBitmap(bmp, newW, newH, disposeSrc:=True, highQuality:=True, cvtToARGBBitmap:=True)
                If Not ok OrElse bmp Is Nothing Then
                    Return BuildErrorMiniThumb(New Size(maxW, maxH))
                End If
            End If

            ' 8) Ergebnis zurück
            Return bmp

        Catch
            Return BuildErrorMiniThumb(New Size(maxW, maxH))
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

    ' Helper: neutrales Platzhalter-Thumbnail "(kein Bild ausgewählt)"
    Private Function BuildNoSelectionMiniThumb(sz As Size, baseFont As Font, parentBack As Color) As Bitmap
        Dim w As Integer = Math.Max(1, sz.Width)
        Dim h As Integer = Math.Max(1, sz.Height)

        Dim bmp As New Bitmap(w, h, Imaging.PixelFormat.Format32bppPArgb)
        bmp.MakeTransparent()

        Using g As Graphics = Graphics.FromImage(bmp)
            g.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
            g.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic
            g.TextRenderingHint = Drawing.Text.TextRenderingHint.AntiAliasGridFit

            ' Rahmen wie bei Label mit BorderStyle.FixedSingle
            Using p As New Pen(SystemColors.ControlDark)
                g.DrawRectangle(p, 0, 0, w - 1, h - 1)
            End Using

            ' Text in Grautextfarbe
            Dim text As String = "Kein Bild gewählt"
            Using f As New Font(baseFont, baseFont.Style),
              br As New SolidBrush(SystemColors.GrayText)

                Dim r As New RectangleF(2.0F, 2.0F, w - 4.0F, h - 4.0F)
                Dim sf As New StringFormat() With {
                .Alignment = StringAlignment.Center,
                .LineAlignment = StringAlignment.Center,
                .Trimming = StringTrimming.EllipsisCharacter,
                .FormatFlags = StringFormatFlags.LineLimit
            }

                g.DrawString(text, f, br, r, sf)
            End Using
        End Using

        Return bmp

    End Function

    Private Function GetTextFromBirm(birm As BackgroundImageRenderMode) As String

        Select Case birm
            Case BackgroundImageRenderMode.CoverCrop
                Return "Modus: Ausschneiden (Cover)"
            Case BackgroundImageRenderMode.FitInside
                Return "Modus: Proportionen beibehalten"
            Case BackgroundImageRenderMode.None
                Return "Modus: Nicht ausgewählt"
            Case BackgroundImageRenderMode.PreserveOrgSize
                Return "Modus: Originalgröße behalten"
            Case BackgroundImageRenderMode.Stretch
                Return "Modus: Dehnen/Strecken"
            Case Else
                If Debugger.IsAttached Then
                    Stop 'programmierfehler
                    Return "Fehler"
                Else
                    Return "Modus: Fehler"
                End If
        End Select

    End Function

    Private Sub btnToolboxHGrdSpfldBitmapNameFallback_Click(sender As Object, e As EventArgs) Handles btnToolboxHGrdSpfldBitmapNameFallback.Click

    End Sub

    Private Sub btnToolboxHGrdSpfldBitmapClearFallback_Click(sender As Object, e As EventArgs) Handles btnToolboxHGrdSpfldBitmapClearFallback.Click
        CopyIniToControl()
    End Sub

    Private Sub btnToolboxHGrdEditorBitmapNameFallback_Click(sender As Object, e As EventArgs) Handles btnToolboxHGrdEditorBitmapNameFallback.Click

    End Sub

    Private Sub btnToolboxHGrdEditorBitmapClearFallback_Click(sender As Object, e As EventArgs) Handles btnToolboxHGrdEditorBitmapClearFallback.Click
        CopyIniToControl()
    End Sub

    Private Sub btnToolboxHGrdSpfldColorFallback_Click(sender As Object, e As EventArgs) Handles btnToolboxHGrdSpfldColorFallback.Click

    End Sub

    Private Sub btnToolboxHGrdSpfldColorClearFallback_Click(sender As Object, e As EventArgs) Handles btnToolboxHGrdSpfldColorClearFallback.Click
        CopyIniToControl()
    End Sub

    Private Sub btnToolboxHGrdEditorColorFallback_Click(sender As Object, e As EventArgs) Handles btnToolboxHGrdEditorColorFallback.Click

    End Sub

    Private Sub btnToolboxHGrdEditorColorClearFallback_Click(sender As Object, e As EventArgs) Handles btnToolboxHGrdEditorColorClearFallback.Click
        CopyIniToControl()
    End Sub

    ''' <summary>
    ''' Hin: die bisherige Farbe, zurück: die neue Farbe und True,
    ''' sofern eine neue Farbe gewählt. Wenn Abbrechen gewählt, False
    ''' </summary>
    ''' <param name="col"></param>
    ''' <returns></returns>
    Private Function GetColor(ByRef col As Color) As Boolean

    End Function




End Class
