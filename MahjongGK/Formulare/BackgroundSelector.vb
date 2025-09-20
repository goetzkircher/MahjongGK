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
'
'
#Disable Warning IDE0079
#Disable Warning IDE1006



Imports System.Drawing.Drawing2D
Imports System.IO
Imports System.Text.RegularExpressions

Public Class BackgroundSelector
    Inherits Form

    ' ── Öffentliche API ──────────────────────────────────────────────────────────
    Private _selectedFile As String = Nothing

    ''' <summary>
    ''' Ausgewählte Originalbild-Datei.
    ''' Setzen akzeptiert entweder einen Vollpfad oder nur den Dateinamen.
    ''' Regeln:
    ''' - String.IsNullOrEmpty ⇒ Auswahl/Thumb werden geleert.
    ''' - Wird ein Vollpfad aus dem User/Factory-Verzeichnis übergeben, wird IsUserGrafik entsprechend gesetzt.
    ''' - Wird nur ein Dateiname übergeben, wird im aktuell aktiven Verzeichnis gesucht.
    ''' - Wird die Datei (und ihr Thumb *.thb.png) nicht gefunden, bleibt die Liste stehen, Vorschau wird geleert.
    ''' </summary>
    Public Property SelectedFile As String ' ''' GEÄNDERT: bidirektional
        Get
            Return _selectedFile
        End Get
        Set(value As String)
            Dim v As String = If(value, String.Empty).Trim()
            If v.Length = 0 Then
                _selectedFile = Nothing
                ' Auswahl und Preview leeren, Buttons passend setzen
                lst.ClearSelected()
                If pic IsNot Nothing AndAlso pic.Image IsNot Nothing Then
                    Dim old As Image = pic.Image : pic.Image = Nothing : old.Dispose()
                End If
                lblOrigSize.Text = "Original: –"
                UpdateSwitchButtons()
                Return
            End If

            ' Falls Vollpfad: Quelle (user/factory) ableiten
            If Path.IsPathRooted(v) Then
                Dim dir As String = Path.GetDirectoryName(v)
                If String.Equals(NormalizeDir(dir), NormalizeDir(_dirUser), StringComparison.OrdinalIgnoreCase) Then
                    If Not _useUser Then IsUserGrafik = True ' ''' NEU: über Property setzen (führt Refresh aus)
                ElseIf String.Equals(NormalizeDir(dir), NormalizeDir(_dirFactory), StringComparison.OrdinalIgnoreCase) Then
                    If _useUser Then IsUserGrafik = False
                End If
                ' In beiden Fällen: versuchen, in aktueller Liste zu selektieren
                _selectedFile = v
                UpdateSelectionFromCurrentState()
            Else
                ' Nur Dateiname → im aktiven Verzeichnis kombinieren
                Dim root As String = If(_useUser, _dirUser, _dirFactory)
                Dim full As String = If(String.IsNullOrEmpty(root), v, Path.Combine(root, v))
                _selectedFile = full
                UpdateSelectionFromCurrentState()
            End If
        End Set
    End Property

    ' NEU: Ausgewählter Render-Modus (statt CheckBox-Logik)
    Public Property SelectedMode As BackgroundImageRenderMode
        Get
            If radPreserve.Checked Then Return BackgroundImageRenderMode.PreserveOrgSize
            If radCoverCrop.Checked Then Return BackgroundImageRenderMode.CoverCrop
            If radStretch.Checked Then Return BackgroundImageRenderMode.Stretch
            Return BackgroundImageRenderMode.FitInside
        End Get
        Set(value As BackgroundImageRenderMode)
            Select Case value
                Case BackgroundImageRenderMode.PreserveOrgSize
                    radPreserve.Checked = True
                Case BackgroundImageRenderMode.CoverCrop
                    radCoverCrop.Checked = True
                Case BackgroundImageRenderMode.FitInside
                    radFitInside.Checked = True
                Case Else ' BackgroundImageRenderMode.Stretch und BackgroundImageRenderMode.None
                    radStretch.Checked = True
            End Select
        End Set
    End Property

    ' Optional: Externer Bildlader (z.B. Magick.NET)
    Private ReadOnly _customImageLoader As Func(Of String, Image) = Nothing

    ' ── Felder ──────────────────────────────────────────────────────────────────
    Private ReadOnly _startDir As String
    Private ReadOnly _items As New List(Of Item)()

    Private WithEvents split As SplitContainer
    Private WithEvents lst As ListBoxSelectOnWheel
    Private WithEvents picHost As Panel
    Private WithEvents pic As PictureBox
    Private WithEvents btnOk As Button
    Private WithEvents btnCancel As Button
    Private WithEvents pnlButtons As Panel
    Private WithEvents btnInfo1 As ButtonInfo
    Private WithEvents btnInfo2 As ButtonInfo
    ' Umschalt-Buttons
    Private WithEvents btnInterne As Button
    Private WithEvents btnEigene As Button
    ' Radiobuttons für RenderMode
    Private WithEvents radFitInside As RadioButton
    Private WithEvents radStretch As RadioButton
    Private WithEvents radCoverCrop As RadioButton
    Private WithEvents radPreserve As RadioButton
    Private WithEvents lblOrigSize As Label

    Private Const clientSizeWidth As Integer = 800
    Private Const clientSizeHeight As Integer = 400

    Private Const PreviewAspect As Double = 1.5 ' 1.5 : 1 (Querformat)
    Private ReadOnly _themer As Theme.ThemeManager

    ' Pfade & Umschaltzustand
    Private _dirFactory As String = String.Empty
    Private _dirUser As String = String.Empty
    Private _useUser As Boolean = False ' False = Interne, True = Eigene

    ''' <summary>
    ''' True = User-Verzeichnis aktiv, False = interne Fabrik-Grafiken.
    ''' Setzen aktualisiert Liste, Auswahl, Buttons und Vorschau gemäß Regeln.
    ''' </summary>
    Public Property IsUserGrafik As Boolean ' ''' NEU: bidirektional
        Get
            Return _useUser
        End Get
        Set(value As Boolean)
            If _useUser = value Then
                Return
            End If
            _useUser = value

            ' Buttons aktualisieren und Liste neu laden
            UpdateSwitchButtons()
            LoadFileList()

            ' Versuch: aktuelle _selectedFile (Dateiname) im neuen Verzeichnis selektieren
            UpdateSelectionFromCurrentState()
        End Set
    End Property

    ' ── Konstruktor ─────────────────────────────────────────────────────────────
    Public Sub New(ByVal startDirectory As String)
        _themer = New Theme.ThemeManager(Me, If(INI.Global_DarkMode, AppTheme.Dark, AppTheme.Light),
                                         useOwnerDrawTabs:=False, customizeProgressBar:=True, brightenAmount:=0)

        _startDir = If(startDirectory, String.Empty)

        Me.Text = "Hintergrundbilder festlegen"
        Me.StartPosition = FormStartPosition.CenterParent
        Me.MinimizeBox = False
        Me.MaximizeBox = False
        Me.ShowIcon = False
        Me.FormBorderStyle = FormBorderStyle.Sizable
        Me.AutoScaleMode = AutoScaleMode.Font
        Me.MinimumSize = New Size(700, 450)

        BuildUi()
        LoadFileList()
    End Sub

    ' ── UI ──────────────────────────────────────────────────────────────────────
    Private Sub BuildUi()
        split = New SplitContainer() With {
            .Dock = DockStyle.Fill,
            .FixedPanel = FixedPanel.Panel1,
            .IsSplitterFixed = False,
            .SplitterWidth = 6
        }

        ' Linke Seite: ListBox
        lst = New ListBoxSelectOnWheel() With {
            .Dock = DockStyle.Fill,
            .IntegralHeight = False
        }
        Dim pnlLeft As New Panel() With {.Dock = DockStyle.Fill, .Padding = New Padding(8, 8, 8, 48)}
        pnlLeft.Controls.Add(lst)

        ' Rechte Seite: Preview
        pic = New PictureBox() With {
            .SizeMode = PictureBoxSizeMode.Zoom,
            .BackColor = Color.Black
        }
        picHost = New Panel() With {
            .Dock = DockStyle.Fill,
            .BackColor = SystemColors.ControlDarkDark,
            .Padding = New Padding(8, 8, 8, 48)
        }
        picHost.Controls.Add(pic)

        split.Panel1MinSize = 160
        split.Panel1.Controls.Add(pnlLeft)
        split.Panel2.Controls.Add(picHost)

        ' Buttons unten (neues zweireihiges Panel)
        btnOk = New Button() With {.Text = "Übernehmen", .DialogResult = DialogResult.OK}
        btnCancel = New Button() With {.Text = "Abbrechen", .DialogResult = DialogResult.Cancel}
        Me.AcceptButton = btnOk
        Me.CancelButton = btnCancel

        ' Info-Buttons
        btnInfo1 = New ButtonInfo() With {
            .Size = New Size(26, 26),
            .AutoSquare = True,
            .InfoHeader = "Info Eigene Bilder",
            .InfoText = BackgroundZipManager.InfoText
        }

        Dim infoModes As New System.Text.StringBuilder()
        infoModes.AppendLine("Anzeigemodi für Hintergrundbilder:")
        infoModes.AppendLine("• Proportionen beibehalten: Bild wird eingepasst (Letterbox).")
        infoModes.AppendLine("• Dehnen/Strecken: Bild wird auf das Zielverhältnis verzerrt.")
        infoModes.AppendLine("• Ausschneiden (Cover): Bild wird so skaliert, dass es die Fläche füllt; Überstände werden mittig beschnitten.")
        infoModes.AppendLine("• Originalgröße bewahren: Bild wird höchstens verkleinert und zentriert auf die Fläche gesetzt.")
        btnInfo2 = New ButtonInfo() With {
            .Size = New Size(26, 26),
            .AutoSquare = True,
            .InfoHeader = "Info Bilddarstellung",
            .InfoText = infoModes.ToString()
        }

        ' Umschalter
        btnInterne = New Button() With {.Text = "Interne Hintergrundgrafiken"}
        btnEigene = New Button() With {.Text = "Eigene Hintergrundgrafiken"}

        ' Radiobuttons (deutsche Bezeichnungen)
        radFitInside = New RadioButton() With {.Text = "Proportionen beibehalten", .AutoSize = True}
        radStretch = New RadioButton() With {.Text = "Dehnen/Strecken", .AutoSize = True, .Checked = True} ' Standard
        radCoverCrop = New RadioButton() With {.Text = "Ausschneiden (Cover)", .AutoSize = True}
        radPreserve = New RadioButton() With {.Text = "Originalgröße bewahren", .AutoSize = True}

        ' Label für Originalgröße
        lblOrigSize = New Label() With {.AutoSize = True, .Text = "Original: –"}

        pnlButtons = New Panel() With {.Dock = DockStyle.Bottom, .Height = 80}
        pnlButtons.Controls.Add(btnOk)
        pnlButtons.Controls.Add(btnCancel)
        pnlButtons.Controls.Add(btnInfo1)
        pnlButtons.Controls.Add(btnInfo2)
        pnlButtons.Controls.Add(btnInterne)
        pnlButtons.Controls.Add(btnEigene)
        pnlButtons.Controls.Add(radFitInside)
        pnlButtons.Controls.Add(radStretch)
        pnlButtons.Controls.Add(radCoverCrop)
        pnlButtons.Controls.Add(radPreserve)
        pnlButtons.Controls.Add(lblOrigSize)

        Me.Controls.Add(split)
        Me.Controls.Add(pnlButtons)

        ' Startlayout
        Me.ClientSize = New Size(clientSizeWidth, clientSizeHeight)
        Me.FormBorderStyle = FormBorderStyle.FixedDialog
        split.SplitterDistance = 220

        ' Events
        AddHandler picHost.Resize, AddressOf PicHost_Resize
        AddHandler lst.SelectedIndexChanged, AddressOf Lst_SelectedIndexChanged
        AddHandler lst.DoubleClick, AddressOf Lst_DoubleClick
        AddHandler btnOk.Click, AddressOf BtnOk_Click
        AddHandler pnlButtons.Resize, AddressOf PnlButtons_Resize

        ' ''' GEÄNDERT: Umschalter leiten auf Properties, nicht direkte Felder
        AddHandler btnInterne.Click, Sub(_s, _e) IsUserGrafik = False
        AddHandler btnEigene.Click, Sub(_s, _e) If HasThumbs(_dirUser) Then IsUserGrafik = True

        ' Einmalig Layout anpassen
        PnlButtons_Resize(pnlButtons, EventArgs.Empty)
        PicHost_Resize(picHost, EventArgs.Empty)

        ' Pfade und Umschaltstatus setzen
        _dirFactory = INI.AppDataDirectory(AppDataSubDir.Hintergrundgrafiken)
        _dirUser = INI.AppDataDirectory(AppDataSubDir.Eigene_Hintergrundgrafiken)

        Dim userHas As Boolean = HasThumbs(_dirUser)
        _useUser = False ' Start: Interne anzeigen
        UpdateSwitchButtons() ' ''' NEU: initial Buttons passend setzen
    End Sub

    Private Sub PnlButtons_Resize(ByVal _s As Object, ByVal _e As EventArgs)
        Dim spacing As Integer = 8

        ' Höhen
        Dim modeH As Integer = 26
        Dim actionH As Integer = 26

        ' Größen
        btnCancel.Size = New Size(100, actionH)
        btnOk.Size = New Size(110, actionH)
        btnInfo1.Size = New Size(24, 24)
        btnInfo2.Size = New Size(24, 24)
        btnInterne.Size = New Size(180, actionH)
        btnEigene.Size = New Size(180, actionH)

        ' Zeilen
        Dim actionTop As Integer = pnlButtons.ClientSize.Height - actionH - spacing
        Dim modeTop As Integer = Math.Max(4, actionTop - modeH - spacing)

        ' Untere Reihe: rechts OK/Cancel
        btnCancel.Location = New Point(pnlButtons.ClientSize.Width - btnCancel.Width - spacing,
                                       actionTop + (actionH - btnCancel.Height) \ 2)
        btnOk.Location = New Point(btnCancel.Left - spacing - btnOk.Width, btnCancel.Top)
        btnInfo2.Location = New Point(btnOk.Left - spacing - btnInfo2.Width,
                                      actionTop + (actionH - btnInfo2.Height) \ 2)

        ' Untere Reihe: links Infos & Umschalter
        btnInfo1.Location = New Point(spacing, actionTop + (actionH - btnInfo1.Height) \ 2)
        btnInterne.Location = New Point(btnInfo1.Right + spacing, actionTop)
        btnEigene.Location = New Point(btnInterne.Right + spacing, actionTop)

        ' Obere Reihe: Radiobuttons
        Dim x As Integer = spacing
        Dim rbTop As Integer = modeTop + (modeH - radFitInside.Height) \ 2

        radFitInside.Location = New Point(x, rbTop) : x = radFitInside.Right + spacing
        radStretch.Location = New Point(x, rbTop) : x = radStretch.Right + spacing
        radCoverCrop.Location = New Point(x, rbTop) : x = radCoverCrop.Right + spacing
        radPreserve.Location = New Point(x, rbTop)

        ' Rechts: Originalgröße-Label
        Dim labelSize As Size = lblOrigSize.PreferredSize
        Dim labelTop As Integer = modeTop + (modeH - labelSize.Height) \ 2
        Dim labelLeft As Integer = pnlButtons.ClientSize.Width - spacing - labelSize.Width
        If labelLeft < radPreserve.Right + spacing Then labelLeft = radPreserve.Right + spacing
        lblOrigSize.Location = New Point(labelLeft, labelTop)
    End Sub

    ' ── Datenmodell ─────────────────────────────────────────────────────────────
    Private NotInheritable Class Item
        Public Property DisplayName As String = String.Empty ' Thumb-Anzeige
        Public Property FullPath As String = String.Empty    ' Originalbild (ohne .thb.png)
        Public Property ThumbPath As String = String.Empty   ' Pfad zum Thumb (*.thb.png)
        Public Overrides Function ToString() As String
            Return DisplayName
        End Function
    End Class

    ' ── Einlesen / Aufbereiten / Sortieren ─────────────────────────────────────
    Private Sub LoadFileList()
        _items.Clear()

        Dim root As String = If(_useUser, _dirUser, _dirFactory)
        If Not Directory.Exists(root) Then
            lst.DataSource = Nothing
            pic.Image = Nothing
            Return
        End If

        ' Nur vorhandene Thumbs (*.thb.png) auflisten; nur wenn Basisbild existiert
        Dim thumbs As IEnumerable(Of String) = Directory.EnumerateFiles(root, "*.thb.png", SearchOption.TopDirectoryOnly)

        For Each thb As String In thumbs
            Dim baseImg As String = thb.Substring(0, thb.Length - ".thb.png".Length)
            If File.Exists(baseImg) AndAlso IsImageFile(baseImg) Then
                Dim rawName As String = Path.GetFileNameWithoutExtension(baseImg)
                Dim disp As String = PrepareDisplayName(rawName)

                _items.Add(New Item With {
                    .DisplayName = disp,
                    .FullPath = baseImg,
                    .ThumbPath = thb
                })
            End If
        Next

        _items.Sort(Function(a As Item, b As Item) StringComparer.CurrentCultureIgnoreCase.Compare(a.DisplayName, b.DisplayName))

        lst.DataSource = Nothing
        lst.DataSource = _items

        If lst.Items IsNot Nothing AndAlso lst.Items.Count > 0 Then
            ' Bei bestehender Auswahl versuchen, passenden Eintrag zu halten
            If Not SelectItemByFileName(_selectedFile) Then
                lst.SelectedIndex = 0
            End If
        Else
            lst.SelectedIndex = -1
            If pic.Image IsNot Nothing Then
                Dim old As Image = pic.Image
                pic.Image = Nothing
                old.Dispose()
            End If
        End If
    End Sub

    ' rechts: Endung ist bereits weg; von rechts alles abschneiden, was nur "0-9 _ . , ' "" - und Whitespace" ist
    Private Shared Function PrepareDisplayName(ByVal raw As String) As String
        If raw Is Nothing Then Return String.Empty

        Dim s As String = raw.Replace("_", " ").Replace("-", " ")
        s = Regex.Replace(s, "[\s0-9_\.\,'""\-]+$", "")
        s = Regex.Replace(s, "\s*[\(\[\{][^\)\]\}]*[\)\]\}]\s*$", "")
        s = Regex.Replace(s, "\s+", " ").Trim()
        If s.Length = 0 Then s = raw.Trim()
        Return s
    End Function

    ' ── Vorschau 1.5:1 ─────────────────────────────────────────────────────────
    Private Sub PicHost_Resize(ByVal sender As Object, ByVal e As EventArgs)
        Dim host As Rectangle = picHost.ClientRectangle
        If host.Width <= 0 OrElse host.Height <= 0 Then Return

        Dim targetW As Double = CDbl(host.Width)
        Dim targetH As Double = targetW / PreviewAspect
        If targetH > host.Height Then
            targetH = host.Height
            targetW = targetH * PreviewAspect
        End If

        Dim x As Integer = CInt((host.Width - targetW) / 2)
        Dim y As Integer = CInt((host.Height - targetH) / 2)

        pic.Bounds = New Rectangle(x, y, CInt(targetW), CInt(targetH))
    End Sub

    ' ── Auswahl / Doppelklick ──────────────────────────────────────────────────
    Private Sub Lst_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs)
        Dim it As Item = TryCast(lst.SelectedItem, Item)
        If it Is Nothing Then
            If pic.Image IsNot Nothing Then
                Dim old As Image = pic.Image
                pic.Image = Nothing
                old.Dispose()
            End If
            lblOrigSize.Text = "Original: –" ' ''' NEU: Label leeren
            Return
        End If

        ' Vorschau setzen
        pic.Image = SafeLoadImage(it.ThumbPath)

        ' Originalgröße auslesen und anzeigen
        Dim sz As Size = GetImageSizeFast(it.FullPath)
        If sz.Width > 0 AndAlso sz.Height > 0 Then
            lblOrigSize.Text = $"Original: {sz.Width} × {sz.Height} px"
        Else
            lblOrigSize.Text = "Original: –"
        End If

        ' Aktuelle Auswahl in SelectedFile spiegeln (bidirektional)
        _selectedFile = it.FullPath ' ''' NEU: interner Spiegel, damit Getter stimmt

        ' Label neu positionieren (Textbreite kann sich ändern)
        PnlButtons_Resize(pnlButtons, EventArgs.Empty)
    End Sub

    Private Sub Lst_DoubleClick(ByVal sender As Object, ByVal e As EventArgs)
        If lst.SelectedItem Is Nothing Then Return
        BtnOk_Click(Me, EventArgs.Empty)
    End Sub

    ' ── OK / Cancel / Info ─────────────────────────────────────────────────────
    Private Sub BtnOk_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim it As Item = TryCast(lst.SelectedItem, Item)
        If it Is Nothing Then
            Me.DialogResult = DialogResult.None
            Return
        End If

        _selectedFile = it.FullPath ' Originalbild (nicht der Thumb)
        Me.Close()
    End Sub

    ' Umschalter: (jetzt über Property geregelt – Handler bleiben zur Bedienung)
    Private Sub BtnInterne_Click(sender As Object, e As EventArgs) Handles btnInterne.Click
        IsUserGrafik = False ' ''' GEÄNDERT
    End Sub

    Private Sub BtnEigene_Click(sender As Object, e As EventArgs) Handles btnEigene.Click
        If HasThumbs(_dirUser) Then IsUserGrafik = True ' ''' GEÄNDERT
    End Sub

    ' ── Bildlader (Thumb laden; WEBP-Fall bleibt für Vollbilder erhalten) ──────
    Private Function SafeLoadImage(ByVal fullpath As String) As Image
        Try
            If _customImageLoader IsNot Nothing Then
                Dim imgCustom As Image = _customImageLoader(fullpath)
                If imgCustom IsNot Nothing Then Return imgCustom
            End If

            Using fs As New FileStream(fullpath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
                Using original As Image = Image.FromStream(fs)
                    Return New Bitmap(original)
                End Using
            End Using

        Catch ex As Exception
            Return MakePlaceholder("Vorschau nicht möglich" & vbCrLf & ex.Message)
        End Try
    End Function

    Private Shared Function MakePlaceholder(ByVal text As String) As Image
        Dim w As Integer = 600
        Dim h As Integer = CInt(w / PreviewAspect)
        Dim bmp As New Bitmap(w, h)
        Using g As Graphics = Graphics.FromImage(bmp)
            g.SmoothingMode = SmoothingMode.AntiAlias
            g.Clear(Color.DimGray)
            Using br As New LinearGradientBrush(New Rectangle(0, 0, w, h), Color.FromArgb(40, Color.White), Color.FromArgb(10, Color.Black), 90.0F)
                g.FillRectangle(br, 0, 0, w, h)
            End Using
            Using pen As New Pen(Color.DarkSlateGray, 3.0F)
                g.DrawRectangle(pen, 1, 1, w - 3, h - 3)
            End Using
            Using f As New Font("Segoe UI", 14.0F, FontStyle.Bold, GraphicsUnit.Point)
                Dim sz As SizeF = g.MeasureString(text, f)
                g.DrawString(text, f, Brushes.WhiteSmoke, (w - sz.Width) / 2.0F, (h - sz.Height) / 2.0F)
            End Using
        End Using
        Return bmp
    End Function

    ' ── Helpers ────────────────────────────────────────────────────────────────
    Private Shared Function IsImageFile(pathname As String) As Boolean
        Dim ext As String = Path.GetExtension(pathname)
        If String.IsNullOrEmpty(ext) Then Return False
        If ext.StartsWith(".", StringComparison.Ordinal) Then ext = ext.Substring(1)
        Dim e As String = ext.ToLowerInvariant()
        Return (e = "jpg" OrElse e = "jpeg" OrElse e = "png" OrElse e = "bmp" OrElse e = "gif" OrElse e = "webp")
    End Function

    Private Shared Function HasThumbs(root As String) As Boolean
        If Not Directory.Exists(root) Then Return False
        Return Directory.EnumerateFiles(root, "*.thb.png", SearchOption.AllDirectories).Any()
    End Function

    ' Liest die Bildgröße schnell aus dem Dateikopf. Unterstützt PNG, JPEG, GIF, BMP, WEBP (VP8X/VP8L).
    ' Bei Fehler/Unbekannt → Fallback via Image.FromStream.
    Private Shared Function GetImageSizeFast(path As String) As Size
        Try
            If String.IsNullOrEmpty(path) OrElse Not File.Exists(path) Then Return Size.Empty

            Using fs As New FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
                Using br As New BinaryReader(fs)
                    ' Mindestens 12 Bytes vorab
                    Dim head As Byte() = br.ReadBytes(12)
                    fs.Position = 0

                    ' PNG
                    If head.Length >= 8 AndAlso head(0) = &H89 AndAlso head(1) = &H50 AndAlso head(2) = &H4E AndAlso head(3) = &H47 AndAlso head(4) = &HD AndAlso head(5) = &HA AndAlso head(6) = &H1A AndAlso head(7) = &HA Then
                        Return ReadPngSize(br)
                    End If

                    ' JPEG
                    If head.Length >= 2 AndAlso head(0) = &HFF AndAlso head(1) = &HD8 Then
                        Return ReadJpegSize(br)
                    End If

                    ' GIF
                    If head.Length >= 6 AndAlso
                   (head(0) = AscW("G"c) AndAlso head(1) = AscW("I"c) AndAlso head(2) = AscW("F"c) AndAlso head(3) = AscW("8"c) AndAlso (head(4) = AscW("7"c) OrElse head(4) = AscW("9"c)) AndAlso head(5) = AscW("a"c)) Then
                        Return ReadGifSize(br)
                    End If

                    ' BMP
                    If head.Length >= 2 AndAlso head(0) = AscW("B"c) AndAlso head(1) = AscW("M"c) Then
                        Return ReadBmpSize(br)
                    End If

                    ' WEBP (RIFF....WEBP)
                    If head.Length >= 12 AndAlso head(0) = AscW("R"c) AndAlso head(1) = AscW("I"c) AndAlso head(2) = AscW("F"c) AndAlso head(3) = AscW("F"c) AndAlso
                   head(8) = AscW("W"c) AndAlso head(9) = AscW("E"c) AndAlso head(10) = AscW("B"c) AndAlso head(11) = AscW("P"c) Then
                        Return ReadWebpSize(br)
                    End If
                End Using
            End Using

            ' Fallback
            Using fs As New FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
                Using img As Image = Image.FromStream(fs, useEmbeddedColorManagement:=True, validateImageData:=False)
                    Return img.Size
                End Using
            End Using

        Catch
            Return Size.Empty
        End Try
    End Function

    ' ---- PNG (IHDR) ----
    Private Shared Function ReadPngSize(br As BinaryReader) As Size
        br.BaseStream.Position = 8 ' nach Signatur
        Dim lenBE As Integer = ReadInt32BE(br) ' Länge des ersten Chunks
        Dim t As Byte() = br.ReadBytes(4)      ' Chunk-Typ
        If t.Length = 4 AndAlso t(0) = AscW("I"c) AndAlso t(1) = AscW("H"c) AndAlso t(2) = AscW("D"c) AndAlso t(3) = AscW("R"c) Then
            Dim w As Integer = ReadInt32BE(br)
            Dim h As Integer = ReadInt32BE(br)
            Return New Size(w, h)
        End If
        Return Size.Empty
    End Function

    ' ---- JPEG (SOF0/1/2/… Marker) ----
    Private Shared Function ReadJpegSize(br As BinaryReader) As Size
        Dim s As Stream = br.BaseStream
        s.Position = 2 ' nach FF D8
        While s.Position + 3 < s.Length
            ' Suche Marker 0xFF??
            Dim b As Integer = s.ReadByte()
            If b <> &HFF Then Continue While
            Dim m As Integer = s.ReadByte()
            If m = &HD9 OrElse m = &HDA Then Exit While ' EOI / SOS → danach heikel
            ' Marker ohne Length?
            If m >= &HD0 AndAlso m <= &HD7 Then Continue While ' RSTn
            If m = &H1 Then Continue While                    ' TEM
            ' Segmentlänge
            Dim segLen As Integer = ReadUInt16BE(br)
            If segLen < 2 OrElse s.Position + segLen - 2 > s.Length Then Exit While
            ' SOF?
            If (m >= &HC0 AndAlso m <= &HC3) OrElse (m >= &HC5 AndAlso m <= &HC7) OrElse (m >= &HC9 AndAlso m <= &HCB) OrElse (m >= &HCD AndAlso m <= &HCF) Then
                ' [Prec(1)][H(2)][W(2)]...
                Dim prec As Byte = br.ReadByte()
                Dim h As Integer = ReadUInt16BE(br)
                Dim w As Integer = ReadUInt16BE(br)
                Return New Size(w, h)
            Else
                ' Segment überspringen
                s.Position += (segLen - 2)
            End If
        End While
        Return Size.Empty
    End Function

    ' ---- GIF (Logical Screen Descriptor) ----
    Private Shared Function ReadGifSize(br As BinaryReader) As Size
        br.BaseStream.Position = 6 ' nach Header
        Dim w As Integer = ReadUInt16LE(br)
        Dim h As Integer = ReadUInt16LE(br)
        Return New Size(w, h)
    End Function

    ' ---- BMP (DIB Header) ----
    Private Shared Function ReadBmpSize(br As BinaryReader) As Size
        br.BaseStream.Position = 14 ' nach BITMAPFILEHEADER (14 Bytes)
        Dim dibSize As Integer = ReadInt32LE(br)
        If dibSize >= 40 AndAlso br.BaseStream.Position + 8 <= br.BaseStream.Length Then
            Dim w As Integer = ReadInt32LE(br)
            Dim h As Integer = Math.Abs(ReadInt32LE(br)) ' Top-Down negative Höhe
            Return New Size(w, h)
        ElseIf dibSize = 12 AndAlso br.BaseStream.Position + 4 <= br.BaseStream.Length Then
            Dim w As Integer = ReadUInt16LE(br)
            Dim h As Integer = ReadUInt16LE(br)
            Return New Size(w, h)
        End If
        Return Size.Empty
    End Function

    ' ---- WEBP (RIFF WEBP VP8X/VP8L) ----
    Private Shared Function ReadWebpSize(br As BinaryReader) As Size
        Dim s As Stream = br.BaseStream
        s.Position = 12 ' nach "RIFF....WEBP"
        While s.Position + 8 <= s.Length
            Dim fourCC As New String(br.ReadChars(4))
            Dim chunkLen As Integer = ReadInt32LE(br)
            If fourCC = "VP8X" AndAlso chunkLen >= 10 AndAlso s.Position + chunkLen <= s.Length Then
                Dim flags As Byte = br.ReadByte()
                br.ReadBytes(3) ' reserved
                Dim wMinus1 As Integer = ReadUInt24LE(br)
                Dim hMinus1 As Integer = ReadUInt24LE(br)
                Return New Size(wMinus1 + 1, hMinus1 + 1)
            ElseIf fourCC = "VP8L" AndAlso chunkLen >= 5 AndAlso s.Position + chunkLen <= s.Length Then
                Dim sig As Byte = br.ReadByte() ' 0x2F
                Dim bits As UInteger = CUInt(ReadUInt32LE(br))
                Dim w As Integer = CInt((bits And &H3FFFUI) + 1UI)
                Dim h As Integer = CInt(((bits >> 14) And &H3FFFUI) + 1UI)
                Return New Size(w, h)
            Else
                ' Unbekannt/VP8 (lossy) → überspringen (evtl. Fallback)
                s.Position += chunkLen + (chunkLen And 1) ' Padding auf even
            End If
        End While
        Return Size.Empty
    End Function

    ' ---- Lesehilfen ----
    Private Shared Function ReadInt32BE(br As BinaryReader) As Integer
        Dim b() As Byte = br.ReadBytes(4)
        If b.Length < 4 Then Return 0
        Return (CInt(b(0)) << 24) Or (CInt(b(1)) << 16) Or (CInt(b(2)) << 8) Or CInt(b(3))
    End Function

    Private Shared Function ReadUInt16BE(br As BinaryReader) As Integer
        Dim b() As Byte = br.ReadBytes(2)
        If b.Length < 2 Then Return 0
        Return (CInt(b(0)) << 8) Or CInt(b(1))
    End Function

    Private Shared Function ReadUInt16LE(br As BinaryReader) As Integer
        Dim b() As Byte = br.ReadBytes(2)
        If b.Length < 2 Then Return 0
        Return CInt(b(0)) Or (CInt(b(1)) << 8)
    End Function

    Private Shared Function ReadInt32LE(br As BinaryReader) As Integer
        Dim b() As Byte = br.ReadBytes(4)
        If b.Length < 4 Then Return 0
        Return CInt(b(0)) Or (CInt(b(1)) << 8) Or (CInt(b(2)) << 16) Or (CInt(b(3)) << 24)
    End Function

    Private Shared Function ReadUInt24LE(br As BinaryReader) As Integer
        Dim b() As Byte = br.ReadBytes(3)
        If b.Length < 3 Then Return 0
        Return CInt(b(0)) Or (CInt(b(1)) << 8) Or (CInt(b(2)) << 16)
    End Function

    Private Shared Function ReadUInt32LE(br As BinaryReader) As UInteger
        Dim b() As Byte = br.ReadBytes(4)
        If b.Length < 4 Then Return 0UI
        Return CUInt(b(0)) Or (CUInt(b(1)) << 8) Or (CUInt(b(2)) << 16) Or (CUInt(b(3)) << 24)
    End Function

    ' ── Aufräumen ───────────────────────────────────────────────────────────────
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            If pic IsNot Nothing AndAlso pic.Image IsNot Nothing Then pic.Image.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    ' ── NEUE/HILFSFUNKTIONEN (ohne Logikänderung) ──────────────────────────────

    ''' <summary>Aktualisiert die Enabled-Zustände der Umschalt-Buttons gemäß aktueller Quelle.</summary>
    Private Sub UpdateSwitchButtons()
        Dim userHas As Boolean = HasThumbs(_dirUser)
        If _useUser Then
            btnEigene.Enabled = False
            btnInterne.Enabled = True
        Else
            btnInterne.Enabled = False
            btnEigene.Enabled = userHas
        End If
    End Sub

    ''' <summary>Normalisiert ein Verzeichnis (ohne abschließenden Backslash).</summary>
    Private Shared Function NormalizeDir(dir As String) As String
        If String.IsNullOrEmpty(dir) Then Return String.Empty
        Return Path.GetFullPath(dir).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
    End Function

    ''' <summary>
    ''' Versucht, basierend auf _selectedFile (Dateiname oder Pfad) den passenden Listeneintrag
    ''' im aktuellen Verzeichnis zu selektieren und Vorschau zu laden.
    ''' Gelingt es nicht, wird die Vorschau geleert.
    ''' </summary>
    Private Sub UpdateSelectionFromCurrentState()
        If String.IsNullOrEmpty(_selectedFile) Then
            lst.ClearSelected()
            If pic IsNot Nothing AndAlso pic.Image IsNot Nothing Then
                Dim old As Image = pic.Image : pic.Image = Nothing : old.Dispose()
            End If
            lblOrigSize.Text = "Original: –"
            Return
        End If

        If Not SelectItemByFileName(_selectedFile) Then
            ' Nicht gefunden → Preview+Label leeren
            If pic IsNot Nothing AndAlso pic.Image IsNot Nothing Then
                Dim old As Image = pic.Image : pic.Image = Nothing : old.Dispose()
            End If
            lblOrigSize.Text = "Original: –"
        End If
    End Sub

    ''' <summary>
    ''' Selektiert in der Liste das Item mit identischem Dateinamen (ohne Pfad) zu <paramref name="fileOrPath"/>.
    ''' Gibt True zurück, wenn gefunden.
    ''' </summary>
    Private Function SelectItemByFileName(fileOrPath As String) As Boolean
        If lst.DataSource Is Nothing OrElse String.IsNullOrEmpty(fileOrPath) Then Return False
        Dim targetName As String = Path.GetFileNameWithoutExtension(fileOrPath)

        For i As Integer = 0 To _items.Count - 1
            Dim it As Item = _items(i)
            If String.Equals(Path.GetFileNameWithoutExtension(it.FullPath), targetName, StringComparison.OrdinalIgnoreCase) Then
                lst.SelectedIndex = i
                Return True
            End If
        Next
        Return False
    End Function

End Class
