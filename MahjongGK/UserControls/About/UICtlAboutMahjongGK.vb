Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On
Imports System.IO


''' <summary>
''' UserControl "Über MahjongGK".
''' Tabs: Über, Lizenz, Bibliotheken, Bilder, Dank.
''' Zeigt unten rechts dezent die App-Version (Application.ProductVersion).
''' Standardgröße: 1070 x 459 (kann frei skaliert werden, alles anchored).
''' </summary>
Public NotInheritable Class UICtlAboutMahjongGK
    Inherits UserControl

    ' Public API
    Private _showCloseButton As Boolean = True
    Public Property ShowCloseButton As Boolean
        Get
            Return _showCloseButton
        End Get
        Set(value As Boolean)
            _showCloseButton = value
            _btnClose.Visible = value
            PerformLayout()
        End Set
    End Property

    ' Core UI
    Private ReadOnly _tabs As New TabControl()
    Private ReadOnly _btnClose As New Button()
    Private ReadOnly _lblVersion As New Label()

    Private ReadOnly _pageAbout As New TabPage("About")
    Private ReadOnly _pageLicense As New TabPage("Lizenz")
    Private ReadOnly _pageLibs As New TabPage("Bibliotheken")
    Private ReadOnly _pageImages As New TabPage("Bilder")
    Private ReadOnly _pageThanks As New TabPage("Dank")

    ' Controls pro Tab
    Private ReadOnly _aboutText As New RichTextBox()
    Private ReadOnly _licText As New RichTextBox()
    Private ReadOnly _btnOpenLicense As New Button()

    Private ReadOnly _libsPanel As New TableLayoutPanel()
    Private ReadOnly _lnkMagick As New LinkLabel()
    Private ReadOnly _lblMagick As New Label()
    Private ReadOnly _lnkSvg As New LinkLabel()
    Private ReadOnly _lblSvg As New Label()

    Private ReadOnly _imgPanel As New TableLayoutPanel()
    Private ReadOnly _lnkPixabay As New LinkLabel()
    Private ReadOnly _lblPixabay As New Label()
    Private ReadOnly _lnkPixabayTerms As New LinkLabel()
    Private ReadOnly _lnkOpenAI As New LinkLabel()
    Private ReadOnly _lblSora As New Label()

    Private ReadOnly _thanksText As New RichTextBox()

    Public Sub New()
        MyBase.New()

        ' Design-Grundwerte
        Me.Name = "UICtlAboutMahjongGK"
        Me.Size = New Drawing.Size(1070, 459)
        Me.AutoScaleMode = AutoScaleMode.Font

        InitializeBaseLayout()
        InitializeAboutTab()
        InitializeLicenseTab()
        InitializeLibrariesTab()
        InitializeImagesTab()
        InitializeThanksTab()

        _tabs.TabPages.AddRange(New TabPage() {_pageAbout, _pageLicense, _pageLibs, _pageImages, _pageThanks})
        _tabs.SelectedIndex = 0

        ' Version klein unten rechts
        Dim v As String = Application.ProductVersion
        _lblVersion.Text = "Version " & v

        ' Layout an Größe koppeln
        AddHandler Me.SizeChanged, Sub(_s, __) RelayoutFooter()
    End Sub

    ' -------------------------------
    ' Hosting-Helfer
    ' -------------------------------

    ''' <summary>
    ''' Optionaler Helfer: In ein Panel hosten (Dock Fill).
    ''' </summary>
    Public Sub HostIn(parentControl As Control)
        If parentControl Is Nothing Then Return
        Me.Dock = DockStyle.Fill
        parentControl.Controls.Add(Me)
        Me.BringToFront()
    End Sub

    ' -------------------------------
    ' Init & Layout
    ' -------------------------------

    Private Sub InitializeBaseLayout()
        ' Tabs
        _tabs.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        _tabs.Location = New Drawing.Point(12, 12)
        _tabs.Size = New Drawing.Size(Me.ClientSize.Width - 24, Me.ClientSize.Height - 70)

        ' Close-Button (optional)
        _btnClose.Text = "Schließen"
        _btnClose.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        _btnClose.Size = New Drawing.Size(100, 28)
        AddHandler _btnClose.Click, Sub(_s, __)
                                        frmMain.AktVisibleUserControl = INI.Global_LastVisibleUserControl
                                    End Sub

        ' Version (rechts unten, dezent)
        _lblVersion.AutoSize = True
        _lblVersion.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right

        Me.Controls.Add(_tabs)
        Me.Controls.Add(_btnClose)
        Me.Controls.Add(_lblVersion)



        RelayoutFooter()
    End Sub

    Private Sub RelayoutFooter()
        ' Positionen dynamisch anpassen
        _btnClose.Location = New Drawing.Point(Me.ClientSize.Width - _btnClose.Width - 12, Me.ClientSize.Height - _btnClose.Height - 10)
        ' Label etwas links vom Close-Button
        Dim lblWidth As Integer = If(_lblVersion.PreferredWidth > 120, _lblVersion.PreferredWidth, 120)
        _lblVersion.Location = New Drawing.Point(_btnClose.Left - 10 - lblWidth, _btnClose.Top + 6)

        ' Tabs an freien Platz anpassen
        _tabs.Size = New Drawing.Size(Me.ClientSize.Width - 24, Me.ClientSize.Height - 70)
    End Sub

    Private Shared Sub PrepareReadOnlyBox(rtb As RichTextBox)
        rtb.BorderStyle = BorderStyle.None
        rtb.ReadOnly = True
        rtb.DetectUrls = True
        rtb.WordWrap = True
        rtb.ScrollBars = RichTextBoxScrollBars.Vertical
        rtb.Dock = DockStyle.Fill
        rtb.BackColor = SystemColors.Window
    End Sub

    Private Shared Sub OpenUrl(url As String)
        Try
            Dim psi As New ProcessStartInfo(url) With {.UseShellExecute = True}
            Process.Start(psi)
        Catch ex As Exception
            MessageBox.Show("Konnte Link nicht öffnen:" & Environment.NewLine & url,
                            "Link öffnen", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Shared Sub OpenLocalFile(path As String)
        Try
            If File.Exists(path) Then
                Dim psi As New ProcessStartInfo(path) With {.UseShellExecute = True}
                Process.Start(psi)
            Else
                MessageBox.Show("Datei nicht gefunden:" & Environment.NewLine & path,
                                "Datei öffnen", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        Catch ex As Exception
            MessageBox.Show("Konnte Datei nicht öffnen." & Environment.NewLine & ex.Message,
                            "Datei öffnen", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' -------------------------------
    ' Tabs
    ' -------------------------------

    Private Sub InitializeAboutTab()
        PrepareReadOnlyBox(_aboutText)
        _aboutText.Text =
"MahjongGK ist ein Open-Source-Projekt für ein Mahjong-Solitär-Spielfeld mit eigenem leistungsstarkem Editor, Renderer und umfangreicher Anpassbarkeit." & Environment.NewLine &
"Entwickelt in VB.NET (.NET Framework 4.8.1) mit Fokus auf saubere Architektur, flexible Erweiterbarkeit und klare Trennung von Daten, Logik und Darstellung." & Environment.NewLine & Environment.NewLine &
"Dieses Programm wurde nach bestem Wissen und Gewissen entwickelt. Die Nutzung erfolgt auf eigene Verantwortung; eine Haftung für Schäden jedweder Art ist ausgeschlossen." & Environment.NewLine & Environment.NewLine &
"Über den Entwickler:" & Environment.NewLine &
"MahjongGK ist ein Hobbyprojekt eines Ingenieurs im Ruhestand, dem Programmieren auch nach Jahrzehnten noch viel Freude bereitet." & Environment.NewLine & Environment.NewLine &
"Götz Kircher wünscht viel Spaß und Entspannung mit dem Programm." & Environment.NewLine &
"MahjongGK@t-online.de - Deutschland/Hessen/Mittelhessen/Marburg"

        Dim lnkRepo As New LinkLabel() With {
        .AutoSize = True,
        .Text = "Quellcode auf GitHub: glkmr/MahjongGK"
    }
        ' Link-Daten (Beschriftung bleibt schön)
        lnkRepo.Links.Clear()
        lnkRepo.Links.Add(22, "glkmr/MahjongGK".Length, "https://github.com/glkmr/MahjongGK")
        AddHandler lnkRepo.LinkClicked,
        Sub(_s, args)
            Dim target As String = TryCast(args.Link.LinkData, String)
            If Not String.IsNullOrEmpty(target) Then OpenUrl(target)
        End Sub

        Dim lnkLatest As New LinkLabel() With {
        .AutoSize = True,
        .Text = " Neueste Version: GitHub Releases"
    }
        lnkLatest.Links.Clear()
        lnkLatest.Links.Add((" Neueste Version: ").Length, ("GitHub Releases").Length, "https://github.com/glkmr/MahjongGK/releases/latest")
        AddHandler lnkLatest.LinkClicked,
                Sub(_s, args)
                    Dim url As String = TryCast(args.Link.LinkData, String)
                    If Not String.IsNullOrEmpty(url) Then OpenUrl(url)
                End Sub

        ' Untereinander anordnen (Text oben, LinkLabel unten)
        Dim host As New TableLayoutPanel() With {
        .Dock = DockStyle.Fill,
        .ColumnCount = 1,
        .RowCount = 2,
        .Padding = New Padding(0)
    }
        host.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))
        host.RowStyles.Add(New RowStyle(SizeType.AutoSize))
        host.Controls.Add(_aboutText, 0, 0)

        Dim linkRow As New FlowLayoutPanel() With {
        .Dock = DockStyle.Fill,
        .FlowDirection = FlowDirection.LeftToRight,
        .AutoSize = True,
        .Padding = New Padding(0, 6, 0, 0)
    }
        linkRow.Controls.Add(lnkRepo)
        'lnkLatest.Controls.AddRenderBitmapTopZOrder(lnkLatest)

        host.Controls.Add(linkRow, 0, 1)
        host.Controls.Add(lnkLatest, 1, 1)


        _pageAbout.Padding = New Padding(12)
        _pageAbout.Controls.Add(host)
    End Sub

    Private Sub InitializeLicenseTab()
        PrepareReadOnlyBox(_licText)
        _licText.Text =
"Copyright (C) 2025–" & Now.Year.ToString & " Götz Kircher" & Environment.NewLine &
"Lizenz: GNU General Public License v3.0 or later (GPL-3.0-or-later)" & Environment.NewLine &
"Vollständigen Lizenztext siehe Datei LICENSE im Programmverzeichnis." & Environment.NewLine &
"SPDX-License-Identifier: GPL-3.0-or-later"

        _btnOpenLicense.Text = "LICENSE öffnen"
        _btnOpenLicense.AutoSize = True
        _btnOpenLicense.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left
        AddHandler _btnOpenLicense.Click,
            Sub(_s, __)
                Dim baseDir As String = AppDomain.CurrentDomain.BaseDirectory
                Dim licPath As String = Path.Combine(baseDir, "LICENCE.txt")
                OpenLocalFile(licPath)
            End Sub

        Dim pnl As New TableLayoutPanel() With {
            .Dock = DockStyle.Fill,
            .ColumnCount = 1,
            .RowCount = 2
        }
        pnl.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))
        pnl.RowStyles.Add(New RowStyle(SizeType.AutoSize))
        pnl.Controls.Add(_licText, 0, 0)

        Dim btnHost As New FlowLayoutPanel() With {
            .Dock = DockStyle.Fill,
            .FlowDirection = FlowDirection.LeftToRight,
            .AutoSize = True,
            .Padding = New Padding(0, 6, 0, 0)
        }
        btnHost.Controls.Add(_btnOpenLicense)
        pnl.Controls.Add(btnHost, 0, 1)

        _pageLicense.Padding = New Padding(12)
        _pageLicense.Controls.Add(pnl)
    End Sub

    Private Sub InitializeLibrariesTab()

        _libsPanel.Dock = DockStyle.Fill
        _libsPanel.Padding = New Padding(4)
        _libsPanel.AutoScroll = True
        _libsPanel.ColumnCount = 2
        _libsPanel.RowCount = 2
        _libsPanel.ColumnStyles.Add(New ColumnStyle(SizeType.AutoSize))
        _libsPanel.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0F))

        _lnkMagick.Text = "Magick.NET (Apache-2.0)"
        AddHandler _lnkMagick.LinkClicked, Sub(_s, __) OpenUrl("https://github.com/dlemstra/Magick.NET")

        _lblMagick.Text = "Bildbearbeitung und WebP-Unterstützung"

        _lnkSvg.Text = "SVG.NET (MS-PL)"
        AddHandler _lnkSvg.LinkClicked, Sub(_s, __) OpenUrl("https://github.com/svg-net/SVG")
        _lblSvg.AutoSize = True
        _lblSvg.Text = "SVG-Darstellung"

        _libsPanel.Controls.Add(_lnkMagick, 0, 0)


        _lnkMagick.AutoSize = True
        _lblMagick.AutoSize = True
        _lnkSvg.AutoSize = True
        _lblSvg.AutoSize = True

        _libsPanel.Controls.Add(_lnkMagick, 0, 0)
        _libsPanel.Controls.Add(_lblMagick, 1, 0)
        _libsPanel.Controls.Add(_lnkSvg, 0, 1)
        _libsPanel.Controls.Add(_lblSvg, 1, 1)

        _pageLibs.Padding = New Padding(12)
        _pageLibs.Controls.Add(_libsPanel)
    End Sub

    Private Sub InitializeImagesTab()
        _imgPanel.Dock = DockStyle.Fill
        _imgPanel.Padding = New Padding(4)
        _imgPanel.AutoScroll = True
        _imgPanel.ColumnCount = 2
        _imgPanel.RowCount = 3
        _imgPanel.ColumnStyles.Add(New ColumnStyle(SizeType.AutoSize))
        _imgPanel.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0F))

        _lnkPixabay.Text = "Pixabay"
        AddHandler _lnkPixabay.LinkClicked, Sub(_s, __) OpenUrl("https://pixabay.com")
        _lblPixabay.Text = "Bilder im Programm-Ordner: Images\Photography\Pixabay\  (Original-Dateinamen mit Pixabay-ID)"

        _lnkPixabayTerms.Text = "Pixabay License"
        AddHandler _lnkPixabayTerms.LinkClicked, Sub(_s, __) OpenUrl("https://pixabay.com/service/terms/")

        _lnkOpenAI.Text = "OpenAI (Sora)"
        AddHandler _lnkOpenAI.LinkClicked, Sub(_s, __) OpenUrl("https://openai.com")
        _lblSora.Text = "KI-generierte Bilder im Programm-Ordner: Images\Photography\Sora\  (volle Nutzungsrechte beim Nutzer)"

        _lnkPixabayTerms.AutoSize = True
        _lnkPixabay.AutoSize = True
        _lblPixabay.AutoSize = True
        _lnkOpenAI.AutoSize = True
        _lblSora.AutoSize = True


        _imgPanel.Controls.Add(_lnkPixabay, 0, 0)
        _imgPanel.Controls.Add(_lblPixabay, 1, 0)
        _imgPanel.Controls.Add(_lnkPixabayTerms, 0, 1)
        _imgPanel.SetColumnSpan(_lnkPixabayTerms, 2)
        _imgPanel.Controls.Add(_lnkOpenAI, 0, 2)
        _imgPanel.Controls.Add(_lblSora, 1, 2)

        _pageImages.Padding = New Padding(12)
        _pageImages.Controls.Add(_imgPanel)
    End Sub

    Private Sub InitializeThanksTab()
        PrepareReadOnlyBox(_thanksText)
        _thanksText.Text =
"Ein besonderer Dank geht an:" & Environment.NewLine &
"- die Pixabay-Community für ihre freien Fotografien" & Environment.NewLine &
("- OpenAI für die Bereitstellung von ""Sora""") & Environment.NewLine &
"- die Entwickler von Magick.NET und SVG.NET" & Environment.NewLine &
"- alle, die Ideen, Feedback und Beiträge zu MahjongGK leisten."

        _pageThanks.Padding = New Padding(12)
        _pageThanks.Controls.Add(_thanksText)
    End Sub

End Class

