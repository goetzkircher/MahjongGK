Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On
Imports System.IO

Namespace MahjongGKSymbolFactory
    ' 
    ''' <summary>
    ''' Kleines WinForms-Tool zum Live-Tuning der Mahjong-Ziegel-Optik.
    ''' </summary>
    Public Class TileStyleTuner
        Inherits Form

        Private ReadOnly cboPreset As New ComboBox()
        Private ReadOnly cboMuster As New ComboBox()
        Private ReadOnly txtIni As New TextBox()
        Private ReadOnly txtSection As New TextBox()
        Private ReadOnly btnBrowseIni As New Button()
        Private ReadOnly btnLoad As New Button()
        Private ReadOnly btnSave As New Button()
        Private ReadOnly btnExport As New Button()
        Private ReadOnly pic As New PictureBox()
        Private ReadOnly lblSize As New Label()
        Private ReadOnly nudW As New NumericUpDown()
        Private ReadOnly nudH As New NumericUpDown()

        ' Slider
        Private ReadOnly trCorner As New TrackBar()
        Private ReadOnly trEdge As New TrackBar()
        Private ReadOnly trShadow As New TrackBar()
        Private ReadOnly trFaceInset As New TrackBar()
        Private ReadOnly trGloss As New TrackBar()
        Private ReadOnly trNoise As New TrackBar()
        Private ReadOnly trSymShadow As New TrackBar()
        Private ReadOnly trSymScale As New TrackBar()
        Private ReadOnly trTintMinA As New TrackBar()
        Private ReadOnly trTintBoost As New TrackBar()
        Private ReadOnly trEdgeLightAlpha As New TrackBar()
        Private ReadOnly trEdgeDarkAlpha As New TrackBar()
        Private ReadOnly trEdgeLightW As New TrackBar()
        Private ReadOnly trEdgeDarkW As New TrackBar()

        ' Color Buttons
        Private ReadOnly btnFaceTint As New Button()
        Private ReadOnly btnBodyTop As New Button()
        Private ReadOnly btnBodyBottom As New Button()
        Private ReadOnly btnEdgeLight As New Button()
        Private ReadOnly btnEdgeDark As New Button()
        Private ReadOnly btnIvoryLight As New Button()
        Private ReadOnly btnIvoryDark As New Button()

        Private ReadOnly lblCorner As New Label()
        Private ReadOnly lblEdge As New Label()
        Private ReadOnly lblShadow As New Label()
        Private ReadOnly lblFaceInset As New Label()
        Private ReadOnly lblGloss As New Label()
        Private ReadOnly lblNoise As New Label()
        Private ReadOnly lblSymShadow As New Label()
        Private ReadOnly lblSymScale As New Label()
        Private ReadOnly lblTintMinA As New Label()
        Private ReadOnly lblTintBoost As New Label()
        Private ReadOnly lblEdgeLightAlpha As New Label()
        Private ReadOnly lblEdgeDarkAlpha As New Label()
        Private ReadOnly lblEdgeLightW As New Label()
        Private ReadOnly lblEdgeDarkW As New Label()

        Private currentSettings As New TileRenderSettings()
        Private currentPreset As TilePreset = TilePreset.Klassisch
        Private currentMuster As SteinIndexEnum = SteinIndexEnum.Punkt06
        Private baseSize As New Size(198, 252)
        Private watcher As TileRenderSettingsWatcher

        ' 
        ''' <summary>
        ''' Konstruktor: UI aufbauen.
        ''' </summary>
        Public Sub New()
            Me.Text = "Mahjong TileStyle Tuner"
            Me.StartPosition = FormStartPosition.CenterScreen
            Me.ClientSize = New Size(1120, 700)
            Me.MinimumSize = New Size(980, 620)
            Me.DoubleBuffered = True

            ' Preset-Auswahl
            cboPreset.DropDownStyle = ComboBoxStyle.DropDownList
            cboPreset.Items.AddRange(New Object() {"Klassisch", "ModernGlatt", "StarkTexturiert"})
            cboPreset.SelectedIndex = 0
            AddHandler cboPreset.SelectedIndexChanged, Sub(sender As Object, e As EventArgs)
                                                           currentPreset = CType(cboPreset.SelectedIndex, TilePreset)
                                                           RenderPreview()
                                                       End Sub
            ' Muster-Auswahl
            cboMuster.DropDownStyle = ComboBoxStyle.DropDownList
            cboMuster.Items.AddRange(New Object() {
                    SteinIndexEnum.ErrorSy.ToString,
                    SteinIndexEnum.Punkt01.ToString,
                    SteinIndexEnum.Punkt02.ToString,
                    SteinIndexEnum.Punkt03.ToString,
                    SteinIndexEnum.Punkt04.ToString,
                    SteinIndexEnum.Punkt05.ToString,
                    SteinIndexEnum.Punkt06.ToString,
                    SteinIndexEnum.Punkt07.ToString,
                    SteinIndexEnum.Punkt08.ToString,
                    SteinIndexEnum.Punkt09.ToString,
                    SteinIndexEnum.Bambus1.ToString,
                    SteinIndexEnum.Bambus2.ToString,
                    SteinIndexEnum.Bambus3.ToString,
                    SteinIndexEnum.Bambus4.ToString,
                    SteinIndexEnum.Bambus5.ToString,
                    SteinIndexEnum.Bambus6.ToString,
                    SteinIndexEnum.Bambus7.ToString,
                    SteinIndexEnum.Bambus8.ToString,
                    SteinIndexEnum.Bambus9.ToString,
                    SteinIndexEnum.Symbol1.ToString,
                    SteinIndexEnum.Symbol2.ToString,
                    SteinIndexEnum.Symbol3.ToString,
                    SteinIndexEnum.Symbol4.ToString,
                    SteinIndexEnum.Symbol5.ToString,
                    SteinIndexEnum.Symbol6.ToString,
                    SteinIndexEnum.Symbol7.ToString,
                    SteinIndexEnum.Symbol8.ToString,
                    SteinIndexEnum.Symbol9.ToString,
                    SteinIndexEnum.DracheR.ToString,
                    SteinIndexEnum.DracheG.ToString,
                    SteinIndexEnum.DracheW.ToString,
                    SteinIndexEnum.WindOst.ToString,
                    SteinIndexEnum.WindSüd.ToString,
                    SteinIndexEnum.WindWst.ToString,
                    SteinIndexEnum.WindNrd.ToString,
                    SteinIndexEnum.BlütePf.ToString,
                    SteinIndexEnum.BlüteOr.ToString,
                    SteinIndexEnum.BlüteCt.ToString,
                    SteinIndexEnum.BlüteBa.ToString,
                    SteinIndexEnum.JahrFrl.ToString,
                    SteinIndexEnum.JahrSom.ToString,
                    SteinIndexEnum.JahrHer.ToString,
                    SteinIndexEnum.JahrWin.ToString
                })

            cboMuster.SelectedIndex = 7

            AddHandler cboMuster.SelectedIndexChanged, Sub(sender As Object, e As EventArgs)
                                                           currentMuster = CType(cboMuster.SelectedIndex, SteinIndexEnum)
                                                           RenderPreview()
                                                       End Sub

            ' INI Pfad / Sektion
            txtIni.Text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "TileRender.ini")
            txtSection.Text = "Preset.Classic"
            btnBrowseIni.Text = "…"
            btnLoad.Text = "Load INI"
            btnSave.Text = "Save INI"

            AddHandler btnBrowseIni.Click, AddressOf OnBrowseIni
            AddHandler btnLoad.Click, AddressOf OnLoadIni
            AddHandler btnSave.Click, AddressOf OnSaveIni

            ' Größe
            lblSize.Text = "Size:"
            nudW.Minimum = 64 : nudW.Maximum = 8192 : nudW.Value = baseSize.Width
            nudH.Minimum = 64 : nudH.Maximum = 8192 : nudH.Value = baseSize.Height
            AddHandler nudW.ValueChanged, Sub()
                                              baseSize = New Size(CInt(nudW.Value), CInt(nudH.Value))
                                              RenderPreview()
                                          End Sub
            AddHandler nudH.ValueChanged, Sub()
                                              baseSize = New Size(CInt(nudW.Value), CInt(nudH.Value))
                                              RenderPreview()
                                          End Sub

            ' PictureBox
            pic.BackColor = Color.DimGray
            pic.SizeMode = PictureBoxSizeMode.CenterImage

            ' Slider Setup
            SetupTrack(trCorner, 2, 40, 14)
            SetupTrack(trEdge, 2, 30, 10)
            SetupTrack(trShadow, 8, 64, 24)
            SetupTrack(trFaceInset, 2, 24, 8)
            SetupTrack(trGloss, 0, 255, 80)
            SetupTrack(trNoise, 0, 255, 18)
            SetupTrack(trSymShadow, 0, 255, 110)
            SetupTrack(trSymScale, 50, 100, 86) ' % (0.50..1.00)
            SetupTrack(trTintMinA, 0, 128, 28)    ' Untergrenze-Alpha (0..128 reicht erfahrungsgemäß)
            SetupTrack(trTintBoost, 100, 200, 180) ' als Prozent (100..200)
            SetupTrack(trEdgeLightAlpha, 0, 255, If(currentSettings.EdgeLightAlpha > 0, currentSettings.EdgeLightAlpha, CInt(currentSettings.EdgeLight.A)))
            SetupTrack(trEdgeDarkAlpha, 0, 255, If(currentSettings.EdgeDarkAlpha > 0, currentSettings.EdgeDarkAlpha, CInt(currentSettings.EdgeDark.A)))
            SetupTrack(trEdgeLightW, 0, 16, CInt(Math.Round(currentSettings.EdgeLightWidthPx))) ' 0 = auto
            SetupTrack(trEdgeDarkW, 0, 20, CInt(Math.Round(currentSettings.EdgeDarkWidthPx)))


            ' Labels
            lblCorner.Text = "CornerRadius / Eckenradius"
            lblEdge.Text = "EdgeWidth / Kantenbreite"
            lblShadow.Text = "ShadowSize / Schattengröße"
            lblFaceInset.Text = "FaceInset / Flächeneinzug"
            lblGloss.Text = "GlossOpacity / Glanz-Deckkraft"
            lblNoise.Text = "NoiseOpacity / Rausch-Deckkraft"
            lblSymShadow.Text = "SymbolShadowOpacity / Symbolschatten-Deckkraft"
            lblSymScale.Text = "SymbolScale / Symbolskalierung (%)"
            lblTintMinA.Text = "FaceTint Min Alpha / Flächenfarbe Mindesttransparenz"
            lblTintBoost.Text = "FaceTint Boost Max (%) / Flächenfarbe Max Verstärkung"
            lblEdgeLightAlpha.Text = "trEdgeLightAlpha außer Funktion"
            lblEdgeDarkAlpha.Text = "trEdgeDarkAlpha außer Funktion"
            lblEdgeLightW.Text = "trEdgeLightW außer Funktion"
            lblEdgeDarkW.Text = "trEdgeDarkW außer Funktion"


            btnFaceTint.Name = "btnFaceTint"
            btnBodyTop.Name = "btnBodyTop"
            btnBodyBottom.Name = "btnBodyBottom"
            btnEdgeLight.Name = "btnEdgeLight"
            btnEdgeDark.Name = "btnEdgeDark"
            btnIvoryLight.Name = "btnIvoryLight"
            btnIvoryDark.Name = "btnIvoryDark"

            ' Color Buttons
            InitColorButton(btnFaceTint, "Flächenfarbe", currentSettings.FaceTint)
            InitColorButton(btnBodyTop, "Körper oben", currentSettings.BodyTop)
            InitColorButton(btnBodyBottom, "Körper unten", currentSettings.BodyBottom)
            InitColorButton(btnEdgeLight, "Kanten hell", currentSettings.EdgeLight)
            InitColorButton(btnEdgeDark, "Kanten dunkel", currentSettings.EdgeDark)
            InitColorButton(btnIvoryLight, "Fläche Elfenbein hell", currentSettings.FaceIvoryLight)
            InitColorButton(btnIvoryDark, "Fläche Elfenbein dunkel", currentSettings.FaceIvoryDark)

            ' Export
            btnExport.Text = "Export: Alle 42 als PNG…"
            AddHandler btnExport.Click, AddressOf OnExportAll

            ' Layout
            BuildLayout()

            ' Slider Events → Settings aktualisieren + Preview
            AddHandler trCorner.ValueChanged, Sub()
                                                  currentSettings.CornerRadius = trCorner.Value
                                                  RenderPreview()
                                              End Sub
            AddHandler trEdge.ValueChanged, Sub()
                                                currentSettings.EdgeWidth = trEdge.Value
                                                RenderPreview()
                                            End Sub
            AddHandler trShadow.ValueChanged, Sub()
                                                  currentSettings.ShadowSize = trShadow.Value
                                                  RenderPreview()
                                              End Sub
            AddHandler trFaceInset.ValueChanged, Sub()
                                                     currentSettings.FaceInset = trFaceInset.Value
                                                     RenderPreview()
                                                 End Sub
            AddHandler trGloss.ValueChanged, Sub()
                                                 currentSettings.GlossOpacity = trGloss.Value
                                                 RenderPreview()
                                             End Sub
            AddHandler trNoise.ValueChanged, Sub()
                                                 currentSettings.NoiseOpacity = trNoise.Value
                                                 RenderPreview()
                                             End Sub
            AddHandler trSymShadow.ValueChanged, Sub()
                                                     currentSettings.SymbolShadowOpacity = trSymShadow.Value
                                                     RenderPreview()
                                                 End Sub
            AddHandler trSymScale.ValueChanged, Sub()
                                                    currentSettings.SymbolScale = trSymScale.Value / 100.0F
                                                    RenderPreview()
                                                End Sub

            AddHandler trTintMinA.ValueChanged, Sub()
                                                    currentSettings.FaceTintMinAlpha = trTintMinA.Value
                                                    RenderPreview()
                                                End Sub

            AddHandler trTintBoost.ValueChanged, Sub()
                                                     currentSettings.FaceTintBoostMax = trTintBoost.Value / 100.0F
                                                     RenderPreview()
                                                 End Sub
            AddHandler trEdgeLightAlpha.ValueChanged, Sub()
                                                          currentSettings.EdgeLightAlpha = trEdgeLightAlpha.Value
                                                          RenderPreview()
                                                      End Sub
            AddHandler trEdgeDarkAlpha.ValueChanged, Sub()
                                                         currentSettings.EdgeDarkAlpha = trEdgeDarkAlpha.Value
                                                         RenderPreview()
                                                     End Sub
            AddHandler trEdgeLightW.ValueChanged, Sub()
                                                      currentSettings.EdgeLightWidthPx = trEdgeLightW.Value
                                                      RenderPreview()
                                                  End Sub
            AddHandler trEdgeDarkW.ValueChanged, Sub()
                                                     currentSettings.EdgeDarkWidthPx = trEdgeDarkW.Value
                                                     RenderPreview()
                                                 End Sub

            lblCorner.BringToFront()
            lblEdge.BringToFront()
            lblShadow.BringToFront()
            lblFaceInset.BringToFront()
            lblGloss.BringToFront()
            lblNoise.BringToFront()
            lblSymShadow.BringToFront()
            lblSymScale.BringToFront()
            lblTintBoost.BringToFront()
            lblTintMinA.BringToFront()
            lblEdgeLightAlpha.BringToFront()
            lblEdgeDarkAlpha.BringToFront()
            lblEdgeLightW.BringToFront()
            lblEdgeDarkW.BringToFront()

            'legt die INI-Datei an, falls noch nicht geschehen

            Dim cif As New CreateIniFileTileStylePresetsDefault
            txtIni.Text = cif.FullPath

            ' Initial: Settings aus Preset übernehmen
            Dim st0 As TileStyle = TileStyle.CreatePreset(currentPreset, baseSize)
            currentSettings = TileSettingsFromStyle(st0)
            SyncUiFromSettings()
            RenderPreview()
        End Sub

        ' 
        ''' <summary>
        ''' Trackbar Grundsetup.
        ''' </summary>
        Private Sub SetupTrack(tb As TrackBar, minV As Integer, maxV As Integer, val As Integer)
            tb.Minimum = minV : tb.Maximum = maxV : tb.Value = Math.Min(Math.Max(val, minV), maxV)
            tb.TickStyle = TickStyle.None : tb.SmallChange = 1 : tb.LargeChange = 2
        End Sub

        ' 
        ''' <summary>
        ''' Colorbutton initialisieren (zeigt Farbe, öffnet ColorDialog).
        ''' </summary>
        Private Sub InitColorButton(btn As Button, title As String, c As Color)
            btn.Text = title
            btn.BackColor = c
            btn.ForeColor = If((CInt(c.R) + c.G + c.B) / 3 < 128, Color.White, Color.Black)
            AddHandler btn.Click,
            Sub()
                Using cd As New ColorDialog()
                    cd.FullOpen = True : cd.Color = btn.BackColor
                    If cd.ShowDialog(Me) = DialogResult.OK Then
                        If btn Is btnFaceTint Then
                            Dim keepA As Integer = Math.Max(currentSettings.FaceTintMinAlpha, Math.Min(160, currentSettings.FaceTintMinAlpha + 20))
                            btn.BackColor = Color.FromArgb(keepA, cd.Color)
                        Else
                            btn.BackColor = cd.Color
                        End If
                        btn.ForeColor = If((CInt(cd.Color.R) + cd.Color.G + cd.Color.B) / 3 < 64, Color.White, Color.Black)
                        ApplyColorToSettings(btn)
                        RenderPreview()
                    End If
                End Using
            End Sub
        End Sub

        ' 
        ''' <summary>
        ''' Mappt Button->Setting.
        ''' </summary>
        Private Sub ApplyColorToSettings(btn As Button)
            Select Case btn.Name
                Case btnFaceTint.Name : currentSettings.FaceTint = btn.BackColor
                Case btnBodyTop.Name : currentSettings.BodyTop = btn.BackColor
                Case btnBodyBottom.Name : currentSettings.BodyBottom = btn.BackColor
                Case btnEdgeLight.Name : currentSettings.EdgeLight = btn.BackColor
                Case btnEdgeDark.Name : currentSettings.EdgeDark = btn.BackColor
                Case btnIvoryLight.Name : currentSettings.FaceIvoryLight = btn.BackColor
                Case btnIvoryDark.Name : currentSettings.FaceIvoryDark = btn.BackColor
            End Select
        End Sub

        ' 
        ''' <summary>
        ''' UI-Layout zusammensetzen.
        ''' </summary>
        Private Sub BuildLayout()
            'Dim pad As Integer = 8

            Dim pnlTop As New Panel With {.Dock = DockStyle.Top, .Height = 80}
            Me.Controls.Add(pnlTop)

            ' Reihe 1: Preset, INI, Section, Size
            Dim left As Integer = 0
            cboPreset.SetBounds(left, 12, 90, 24)
            '
            left += 90 + 5
            cboMuster.SetBounds(left, 12, 70, 24)
            '
            left += 70 + 5
            txtIni.SetBounds(left, 12, 300, 24)
            '
            left += 300 + 2
            btnBrowseIni.SetBounds(left, 10, 24, 24)
            '
            left += 24 + 5
            txtSection.SetBounds(left, 12, 90, 24)
            '
            left += 90 + 5
            btnLoad.SetBounds(left, 10, 75, 24)
            '
            left += 75 + 5
            btnSave.SetBounds(left, 10, 75, 24)
            '
            lblSize.SetBounds(10, 45, 40, 24)
            nudW.SetBounds(55, 45, 70, 24)
            nudH.SetBounds(130, 45, 70, 24)
            btnExport.SetBounds(220, 45, 220, 26)

            pnlTop.Controls.AddRange({cboPreset, cboMuster, txtIni, btnBrowseIni, txtSection, btnLoad, btnSave, lblSize, nudW, nudH, btnExport})

            ' Links: Steuerbereich (wir machen ihn etwas breiter, z.B. 520 px)
            Dim pnlControl As New Panel With {.Dock = DockStyle.Left, .Width = 550}
            Me.Controls.Add(pnlControl)

            ' Subspalten im Steuerbereich:
            Dim pnlLeft As New Panel With {.Parent = pnlControl, .Width = 350, .Dock = DockStyle.Left}   ' Slider
            Dim pnlRight As New Panel With {.Parent = pnlControl, .Dock = DockStyle.Right}                ' Buttons rechts

            ' --- SLIDER (links) ---
            Dim y As Integer = 10
            AddRow(pnlLeft, lblCorner, trCorner, y) : y += 46
            AddRow(pnlLeft, lblEdge, trEdge, y) : y += 46
            AddRow(pnlLeft, lblShadow, trShadow, y) : y += 46
            AddRow(pnlLeft, lblFaceInset, trFaceInset, y) : y += 46
            AddRow(pnlLeft, lblGloss, trGloss, y) : y += 46
            AddRow(pnlLeft, lblNoise, trNoise, y) : y += 46
            AddRow(pnlLeft, lblSymShadow, trSymShadow, y) : y += 46
            AddRow(pnlLeft, lblSymScale, trSymScale, y) : y += 56

            ' NEU: EdgeLight/EdgeDark Slider UNTER die bisherigen
            AddRow(pnlLeft, lblEdgeLightAlpha, trEdgeLightAlpha, y) : y += 46
            AddRow(pnlLeft, lblEdgeDarkAlpha, trEdgeDarkAlpha, y) : y += 46
            AddRow(pnlLeft, lblEdgeLightW, trEdgeLightW, y) : y += 46
            AddRow(pnlLeft, lblEdgeDarkW, trEdgeDarkW, y) : y += 56

            ' --- BUTTONS (rechts) ---
            Dim btns() As Button = {btnFaceTint, btnBodyTop, btnBodyBottom, btnEdgeLight, btnEdgeDark, btnIvoryLight, btnIvoryDark}
            Dim yb As Integer = 40
            Dim btnW As Integer = 155     ' „jetzige Breite“
            Dim btnH As Integer = 30
            For Each b As Button In btns
                b.SetBounds(10, yb, btnW, btnH)
                pnlRight.Controls.Add(b)
                yb += btnH + 8
            Next

            ' Rechts: Preview über den restlichen Platz
            'pic.Dock = DockStyle.Right
            With pic
                .Left = 550
                .Top = 100
                .Width = 400
                .Height = 400
                .Anchor = AnchorStyles.Left Or AnchorStyles.Top
            End With
            Me.Controls.Add(pic)

            ' Topbar (Preset/INI/Export) bleibt wie gehabt


        End Sub

        Private Sub AddRow(parent As Control, lbl As Label, tr As TrackBar, ByRef y As Integer)
            lbl.AutoSize = True
            lbl.SetBounds(15, y + 42, 180, 22)
            tr.SetBounds(10, y + 22, 320, 32)
            parent.Controls.Add(lbl)
            parent.Controls.Add(tr)
        End Sub

        ' 
        ''' <summary>
        ''' Settings -> UI spiegeln.
        ''' </summary>
        Private Sub SyncUiFromSettings()
            trCorner.Value = Clamp(CInt(Math.Round(currentSettings.CornerRadius)), trCorner.Minimum, trCorner.Maximum)
            trEdge.Value = Clamp(CInt(Math.Round(currentSettings.EdgeWidth)), trEdge.Minimum, trEdge.Maximum)
            trShadow.Value = Clamp(CInt(Math.Round(currentSettings.ShadowSize)), trShadow.Minimum, trShadow.Maximum)
            trFaceInset.Value = Clamp(CInt(Math.Round(currentSettings.FaceInset)), trFaceInset.Minimum, trFaceInset.Maximum)
            trGloss.Value = Clamp(currentSettings.GlossOpacity, trGloss.Minimum, trGloss.Maximum)
            trNoise.Value = Clamp(currentSettings.NoiseOpacity, trNoise.Minimum, trNoise.Maximum)
            trSymShadow.Value = Clamp(currentSettings.SymbolShadowOpacity, trSymShadow.Minimum, trSymShadow.Maximum)
            trSymScale.Value = Clamp(CInt(Math.Round(currentSettings.SymbolScale * 100.0F)), trSymScale.Minimum, trSymScale.Maximum)
            trTintMinA.Value = Clamp(currentSettings.FaceTintMinAlpha, trTintMinA.Minimum, trTintMinA.Maximum)
            trTintBoost.Value = Clamp(CInt(Math.Round(currentSettings.FaceTintBoostMax * 100.0F)), trTintBoost.Minimum, trTintBoost.Maximum)

            btnFaceTint.BackColor = currentSettings.FaceTint
            btnBodyTop.BackColor = currentSettings.BodyTop
            btnBodyBottom.BackColor = currentSettings.BodyBottom
            btnEdgeLight.BackColor = currentSettings.EdgeLight
            btnEdgeDark.BackColor = currentSettings.EdgeDark
            btnIvoryLight.BackColor = currentSettings.FaceIvoryLight
            btnIvoryDark.BackColor = currentSettings.FaceIvoryDark
        End Sub

        Private Function Clamp(v As Integer, mn As Integer, mx As Integer) As Integer
            If v < mn Then Return mn
            If v > mx Then Return mx
            Return v
        End Function

        ''' <summary>
        ''' Settings -> TileStyle, Render 1 Beispiel-Stein (z. B. Punkt05).
        ''' </summary>
        Private Sub RenderPreview()


            Try
                Dim stBase As MahjongGKSymbolFactory.TileStyle = TileStyle.CreatePreset(currentPreset, baseSize)
                Dim st As TileStyle = currentSettings.ToTileStyle(stBase)
                Debug.Print($"FaceTint={st.FaceTint} BodyTop={st.BodyTop} BodyBottom={st.BodyBottom}")

                ' Ein Beispielsymbol bauen (hier: Punkt05 in Zielgröße)
                Dim sym As Bitmap = MahjongSymbolFactory.GenerateSymbolChinese(currentMuster, baseSize)
                Dim tile As Bitmap = MahjongTileRenderer.RenderTile(baseSize, sym, st)
                ' in PictureBox darstellen
                pic.Image?.Dispose()
                pic.Image = CType(tile.Clone(), Bitmap)
                pic.Refresh()
                tile.Dispose()
                sym.Dispose()
            Catch ex As Exception
                ' Für Debug:
                ' MessageBox.Show(Me, ex.Message, "RenderPreview Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try

        End Sub

        ' 
        ''' <summary>
        ''' Aus Preset-Style Settings „abgreifen“.
        ''' </summary>
        Private Function TileSettingsFromStyle(st As TileStyle) As TileRenderSettings
            Dim s As New TileRenderSettings() With {
            .CornerRadius = st.CornerRadius,
            .EdgeWidth = st.EdgeWidth,
            .ShadowSize = st.ShadowSize,
            .FaceInset = st.FaceInset,
            .FaceTint = st.FaceTint,
            .BodyTop = st.BodyTop,
            .BodyBottom = st.BodyBottom,
            .EdgeLight = st.EdgeLight,
            .EdgeDark = st.EdgeDark,
            .FaceIvoryLight = st.FaceIvoryLight,
            .FaceIvoryDark = st.FaceIvoryDark,
            .GlossOpacity = st.GlossOpacity,
            .NoiseOpacity = st.NoiseOpacity,
            .SymbolShadowOpacity = st.SymbolShadowOpacity,
            .SymbolScale = st.SymbolScale
        }
            Return s
        End Function

        ' 
        ''' <summary>
        ''' INI auswählen.
        ''' </summary>
        Private Sub OnBrowseIni(sender As Object, e As EventArgs)
            Using sfd As New SaveFileDialog()
                sfd.Filter = "INI|*.ini|Alle Dateien|*.*"
                sfd.FileName = Path.GetFileName(txtIni.Text)
                sfd.InitialDirectory = Path.GetDirectoryName(txtIni.Text)
                If sfd.ShowDialog(Me) = DialogResult.OK Then
                    txtIni.Text = sfd.FileName
                End If
            End Using
        End Sub

        ' 
        ''' <summary>
        ''' Aus INI laden (Sektion in txtSection).
        ''' </summary>
        Private Sub OnLoadIni(sender As Object, e As EventArgs)
            Try
                Dim ini As New IniFile(txtIni.Text)
                Dim s As New TileRenderSettings()
                s.LoadFromIni(ini, txtSection.Text)
                currentSettings = s
                SyncUiFromSettings()
                RenderPreview()

                ' Watcher für Hot-Reload
                watcher?.GetType() ' noop
                watcher = New TileRenderSettingsWatcher(txtIni.Text, txtSection.Text)
                AddHandler watcher.Reloaded, Sub(ns As TileRenderSettings)
                                                 currentSettings = ns
                                                 SyncUiFromSettings()
                                                 RenderPreview()
                                             End Sub
            Catch ex As Exception
                MessageBox.Show(Me, ex.Message, "INI laden", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Sub

        ' 
        ''' <summary>
        ''' In INI speichern (Sektion in txtSection).
        ''' </summary>
        Private Sub OnSaveIni(sender As Object, e As EventArgs)
            Try
                Dim ini As New IniFile(txtIni.Text)
                currentSettings.SaveToIni(ini, txtSection.Text)
                MessageBox.Show(Me, "Gespeichert.", "INI", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Catch ex As Exception
                MessageBox.Show(Me, ex.Message, "INI speichern", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Sub

        ' 
        ''' <summary>
        ''' Batch-Export aller 42 Steine nach Ordner (fragt Zielordner ab).
        ''' </summary>
        Private Sub OnExportAll(sender As Object, e As EventArgs)
            Using fbd As New FolderBrowserDialog()
                fbd.Description = "Zielordner für PNG-Export wählen"
                If fbd.ShowDialog(Me) <> DialogResult.OK Then Return

                Dim stBase As TileStyle = TileStyle.CreatePreset(currentPreset, baseSize)
                Dim st As TileStyle = currentSettings.ToTileStyle(stBase)

                ' Wir nutzen den SetRenderer mit Serienfarben des Presets:
                Dim hiSize As Size = baseSize ' hier ohne Supersampling; alternativ: Size(base*2)
                Dim symbols As List(Of Bitmap) = MahjongSymbolFactory.GenerateAllSymbols(hiSize)
                Dim tiles As List(Of Bitmap) = SetRenderer.RenderSetWithSeries(symbols, hiSize, CType(cboPreset.SelectedIndex, TilePreset))

                ' Alle speichern:
                Try
                    For i As Integer = 0 To tiles.Count - 1
                        Dim si As SteinIndexEnum = CType(i, SteinIndexEnum)
                        Dim fp As String = Path.Combine(fbd.SelectedPath, $"{si}.png")
                        tiles(i).Save(fp, Imaging.ImageFormat.Png)
                    Next
                    MessageBox.Show(Me, "Export abgeschlossen.", "PNG Export", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Catch ex As Exception
                    MessageBox.Show(Me, ex.Message, "Export-Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Finally
                    For Each b As Bitmap In tiles : b.Dispose() : Next
                    For Each b As Bitmap In symbols : b.Dispose() : Next
                End Try
            End Using
        End Sub

        ' 
        ''' <summary>
        ''' Einstiegspunkt für Standalone-Test.
        ''' </summary>
        <STAThread>
        Public Shared Sub Main()
            Application.EnableVisualStyles()
            Application.SetCompatibleTextRenderingDefault(False)
            Application.Run(New TileStyleTuner())
        End Sub

    End Class
End Namespace
