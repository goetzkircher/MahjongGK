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

#Disable Warning IDE0079
#Disable Warning IDE1006

Imports System.Drawing.Drawing2D
Imports System.Drawing.Imaging
'
Imports System.IO

Public Class UctlToolboxHintergrund
    Public Sub InitialisierungAndUpdate()
        If Not Debugger.IsAttached Then
            chkDrawRenderRect.Visible = False
        End If
        m_iZuweisungAktiv = +1
        CopyIniToControl()
        m_iZuweisungAktiv -= 1
    End Sub

    Private Const NOCOLSEL As String = "Keine Farbe gewählt"

    Private m_iZuweisungAktiv As Integer = 0

    Private Sub CopyIniToControl()
        'TODO SFD-Anpassung
        '' '
        ''With lblToolboxHGrdSplFldColor
        ''    If Spielfeld.SFD.SpielfeldSpielfeldInfo.Toolbox_HGrdSplFldColor = Color.Empty Then
        ''        .ForeColor = SystemColors.GrayText
        ''        .BackColor = Color.Transparent
        ''        .Text = NOCOLSEL
        ''    Else
        ''        .Text = String.Empty
        ''        .BackColor = Spielfeld.SFD.SpielfeldSpielfeldInfo.Toolbox_HGrdSplFldColor
        ''    End If
        ''End With
        '' '
        ''With lblToolboxHGrdSplFldColorFallback
        ''    If INI.Toolbox_HGrdSplFldColorFallback = Color.Empty Then
        ''        .ForeColor = SystemColors.GrayText
        ''        .BackColor = Color.Transparent
        ''        .Text = NOCOLSEL
        ''    Else
        ''        .Text = String.Empty
        ''        .BackColor = INI.Toolbox_HGrdSplFldColorFallback
        ''    End If
        ''End With

        ''chkDrawRenderRect.Checked = INI.Rendering_DrawRenderRect

        ''picToolboxHGrdSplFld.Image = GetMiniThumb(Spielfeld.SFD.SpielfeldSpielfeldInfo.Toolbox_HGrdSplFldBitmapName, Spielfeld.SFD.SpielfeldSpielfeldInfo.Toolbox_HGrdSplFldBitmapIsUserGrafik)
        ''picToolboxHGrdSplFldFallback.Image = GetMiniThumb(INI.Toolbox_HGrdSplFldBitmapNameFallback, INI.Toolbox_HGrdSplFldBitmapIsUserGrafikFallback)
        ''lblToolboxHGrdSplFldRenderMode.Text = GetTextFromBirm(Spielfeld.SFD.SpielfeldSpielfeldInfo.Toolbox_HGrdSplFldRenderMode)
        ''lblToolboxHGrdSplFldRenderModeFallback.Text = GetTextFromBirm(INI.Toolbox_HGrdSplFldRenderModeFallback)

        ''  'INI.Toolbox_HGrdEditorColorFallback  
        ''  'INI.Toolbox_HGrdSplFldBitmapFallback  
        ''  'INI.Toolbox_HGrdEditorBitmapFallback 
        ''  'INI.Toolbox_HGrdSplFldBitmapIsUserGrafikFallback  
        ''  'INI.Toolbox_HGrdEditorBitmapIsUserGrafikFallback  

    End Sub

    Private Sub CopyControlToIni()
        'INI.Toolbox_HGrdSplFldColorFallback  
        'INI.Toolbox_HGrdEditorColorFallback  
        'INI.Toolbox_HGrdSplFldBitmapFallback  
        'INI.Toolbox_HGrdEditorBitmapFallback 
        'INI.Toolbox_HGrdSplFldBitmapIsUserGrafikFallback  
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
        Dim maxW As Integer = picToolboxHGrdSplFld.Width
        Dim maxH As Integer = picToolboxHGrdSplFld.Height

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

    Private Function GetTextFromBirm(birm As Images.BackgroundImageRenderMode) As String

        Select Case birm
            Case Images.BackgroundImageRenderMode.CoverCrop
                Return "Modus: Ausschneiden (Cover)"
            Case Images.BackgroundImageRenderMode.FitInside
                Return "Modus: Proportionen beibehalten"
            Case Images.BackgroundImageRenderMode.None
                Return "Modus: Nicht ausgewählt"
            Case Images.BackgroundImageRenderMode.PreserveOrgSize
                Return "Modus: Originalgröße behalten"
            Case Images.BackgroundImageRenderMode.Stretch
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

    ''TODO SFD-Anpassung
    ''Private Sub btnToolboxHGrdSplFldBitmapClear_Click(sender As Object, e As EventArgs) Handles btnToolboxHGrdSplFldBitmapClear.Click
    ''    With Spielfeld.SFD.SpielfeldSpielfeldInfo
    ''        .Toolbox_HGrdSplFldBitmapIsUserGrafik = False
    ''        .Toolbox_HGrdSplFldBitmapName = String.Empty
    ''        .Toolbox_HGrdSplFldRenderMode = BackgroundImageRenderMode.None
    ''    End With
    ''    With picToolboxHGrdSplFld
    ''        .Image = BuildNoSelectionMiniThumb(New Size(.Width, .Height), Me.Font, Me.BackColor)
    ''    End With
    ''End Sub
    ''Private Sub btnToolboxHGrdSplFldBitmapClearFallback_Click(sender As Object, e As EventArgs) Handles btnToolboxHGrdSplFldBitmapClearFallback.Click
    ''    INI.Toolbox_HGrdSplFldBitmapIsUserGrafikFallback = False
    ''    INI.Toolbox_HGrdSplFldBitmapNameFallback = String.Empty
    ''    INI.Toolbox_HGrdSplFldRenderModeFallback = BackgroundImageRenderMode.None
    ''    With picToolboxHGrdSplFldFallback
    ''        .Image = BuildNoSelectionMiniThumb(New Size(.Width, .Height), Me.Font, Me.BackColor)
    ''    End With
    ''End Sub

    ''Private Sub btnToolboxHGrdSplFldBitmapName_Click(sender As Object, e As EventArgs) Handles btnToolboxHGrdSplFldBitmapName.Click

    ''    With Spielfeld.SFD.SpielfeldSpielfeldInfo
    ''        Using frm As New BackgroundSelector(If(.Toolbox_HGrdSplFldBitmapIsUserGrafik,
    ''                                  INI.AppDataDirectory(AppDataSubDir.Eigene_Hintergrundgrafiken),
    ''                                  INI.AppDataDirectory(AppDataSubDir.Hintergrundgrafiken)))
    ''            frm.IsUserGrafik = .Toolbox_HGrdSplFldBitmapIsUserGrafik
    ''            frm.SelectedFile = .Toolbox_HGrdSplFldBitmapName
    ''            frm.SelectedMode = .Toolbox_HGrdSplFldRenderMode
    ''            If frm.ShowDialog = DialogResult.OK Then
    ''                .Toolbox_HGrdSplFldBitmapIsUserGrafik = frm.IsUserGrafik
    ''                .Toolbox_HGrdSplFldBitmapName = frm.SelectedFile
    ''                With picToolboxHGrdSplFld
    ''                    .Image = GetMiniThumb(frm.SelectedFile, frm.IsUserGrafik)
    ''                    .Refresh()
    ''                End With
    ''                lblToolboxHGrdSplFldRenderMode.Text = GetTextFromBirm(frm.SelectedMode)
    ''                .Toolbox_HGrdSplFldRenderMode = frm.SelectedMode
    ''            End If
    ''        End Using
    ''    End With
    ''End Sub

    ''Private Sub btnToolboxHGrdSplFldBitmapNameFallback_Click(sender As Object, e As EventArgs) Handles btnToolboxHGrdSplFldBitmapNameFallback.Click
    ''    Using frm As New BackgroundSelector(If(INI.Toolbox_HGrdSplFldBitmapIsUserGrafikFallback,
    ''                                  INI.AppDataDirectory(AppDataSubDir.Eigene_Hintergrundgrafiken),
    ''                                  INI.AppDataDirectory(AppDataSubDir.Hintergrundgrafiken)))
    ''        frm.IsUserGrafik = INI.Toolbox_HGrdSplFldBitmapIsUserGrafikFallback
    ''        frm.SelectedFile = INI.Toolbox_HGrdSplFldBitmapNameFallback
    ''        frm.SelectedMode = INI.Toolbox_HGrdSplFldRenderModeFallback
    ''        If frm.ShowDialog = DialogResult.OK Then
    ''            INI.Toolbox_HGrdSplFldBitmapIsUserGrafikFallback = frm.IsUserGrafik
    ''            INI.Toolbox_HGrdSplFldBitmapNameFallback = frm.SelectedFile
    ''            With picToolboxHGrdSplFldFallback
    ''                .Image = GetMiniThumb(frm.SelectedFile, frm.IsUserGrafik)
    ''                .Refresh()
    ''            End With
    ''            lblToolboxHGrdSplFldRenderModeFallback.Text = GetTextFromBirm(frm.SelectedMode)
    ''            INI.Toolbox_HGrdSplFldRenderModeFallback = frm.SelectedMode
    ''        End If
    ''    End Using
    ''End Sub

    ''Private Sub btnToolboxHGrdSplFldColorClear_Click(sender As Object, e As EventArgs) Handles btnToolboxHGrdSplFldColorClear.Click
    ''    Spielfeld.SFD.SpielfeldSpielfeldInfo.Toolbox_HGrdSplFldColor = Color.Empty
    ''    ClearColor(lblToolboxHGrdSplFldColor)
    ''End Sub
    ''Private Sub btnToolboxHGrdSplFldColorClearFallback_Click(sender As Object, e As EventArgs) Handles btnToolboxHGrdSplFldColorClearFallback.Click
    ''    INI.Toolbox_HGrdSplFldColorFallback = Color.Empty
    ''    lblToolboxHGrdSplFldColorFallback.BackColor = Color.Transparent
    ''End Sub

    ''Private Sub btnToolboxHGrdSplFldColor_Click(sender As Object, e As EventArgs) Handles btnToolboxHGrdSplFldColor.Click
    ''    If GetColor(lblToolboxHGrdSplFldColor) Then
    ''        Spielfeld.SFD.SpielfeldSpielfeldInfo.Toolbox_HGrdSplFldColor = lblToolboxHGrdSplFldColor.BackColor
    ''    End If
    ''End Sub
    ''Private Sub btnToolboxHGrdSplFldColorFallback_Click(sender As Object, e As EventArgs) Handles btnToolboxHGrdSplFldColorFallback.Click
    ''    If GetColor(lblToolboxHGrdSplFldColorFallback) Then
    ''        INI.Toolbox_HGrdSplFldColorFallback = lblToolboxHGrdSplFldColorFallback.BackColor
    ''    End If
    ''End Sub

    Private Sub chkToolboxHGrdEditorShowFraming_CheckedChanged(sender As Object, e As EventArgs) Handles chkToolboxHGrdEditorShowFraming.CheckedChanged

    End Sub

    Private Function GetColor(lbl As Label) As Boolean

        Dim ctrlDown As Boolean = (Control.ModifierKeys And Keys.Control) = Keys.Control

        If ctrlDown Then
            Using frm As New ColorPickerNamedColors()
                With frm
                    If lbl.Text = NOCOLSEL Then
                        .SelectedColor = Color.Empty
                    Else
                        .SelectedColor = lbl.BackColor
                    End If
                    Dim dgr As DialogResult = .ShowDialog
                    If dgr <> DialogResult.OK Then
                        Return False
                    Else
                        lbl.BackColor = .SelectedColor
                        lbl.Text = String.Empty
                        lbl.Refresh()
                        Return True
                    End If
                End With
            End Using
        Else
            Using frm As New ColorPickerHSB
                With frm
                    .SavedColorsString = INI.ColorPickerHSB_Toolbox_SavedColorsString
                    .PickerBackColor = INI.ColorPickerHSB_Toolbox_PickerBackColor
                    .SelectedColor = lbl.BackColor

                    Dim dgr As DialogResult = .ShowDialog

                    INI.ColorPickerHSB_Toolbox_SavedColorsString = .SavedColorsString
                    INI.ColorPickerHSB_Toolbox_PickerBackColor = .PickerBackColor

                    If dgr <> DialogResult.OK Then
                        Return False
                    Else
                        lbl.BackColor = .SelectedColor
                        lbl.Text = String.Empty
                        lbl.Refresh()
                        Return True
                    End If
                End With
            End Using
        End If

    End Function

    Private Sub ClearColor(lbl As Label)
        With lbl
            .ForeColor = SystemColors.GrayText
            .BackColor = Color.Transparent
            .Text = NOCOLSEL
            .Refresh()
        End With
    End Sub

    Private Sub chkDrawRenderRect_CheckedChanged(sender As Object, e As EventArgs) Handles chkDrawRenderRect.CheckedChanged
        If m_iZuweisungAktiv <> 0 Then
            Exit Sub
        End If

        INI.Rendering_DrawRenderRect = chkDrawRenderRect.Checked
    End Sub
End Class
