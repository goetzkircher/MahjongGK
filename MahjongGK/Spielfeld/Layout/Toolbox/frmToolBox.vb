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
'
#Disable Warning IDE0079
#Disable Warning IDE1006

Imports System.ComponentModel
Imports MahjongGK.Contracts
Imports MahjongGK.Helfer
Imports MahjongGK.Spielfeld
Imports Microsoft.VisualBasic.FileIO
'
''' <summary>
''' Wichtiger Hinweis zur Toolbox:
''' Wenn ein anderes Spiel geladen wird, muss die vorherige Toolbox geschlossen werden.
''' Dazu gibt es Im Modul SFMain das Sub SFMain.CloseToolBox, die aber auch über
''' CloseSpielfeld aufgerufen wird.
''' 
''' </summary>
Public Class frmToolBox

    Public Const MJ_FRMTOOLBOX_WIDTH As Integer = 424 'aktuelle Maße eintragen
    Public Const MJ_FRMTOOLBOX_HEIGHT As Integer = 572 'wird in der INi gebraucht.
    'Public Const MJ_FRMTOOLBOX_PADDING_LEFT As Integer = 15
    'Public Const MJ_FRMTOOLBOX_PADDING_BOTTOM As Integer = 20
    'Public Const MJ_FRMTOOLBOX_PANEL_ As Integer = 15

#Region "Config für Icons"

    'Icons aus Ressourcen nach dem Schema "Enumeration_Wert"
    '(z. B. EditorFileCmd_Speichern, Basisform_Kreis).
    'Wenn ein Icon fehlt, wird einfach keins gesetzt.
    'Ich habe eine kleine Hilfsfunktion eingebaut, die das Bitmap
    'aus My.Resources holt und auf 16×16 skaliert (anpassbar).

    Private Const ICON_SIZE As Integer = 16
    Private Const SCALE_ICONS As Boolean = True

#End Region

    Public Shared FrmToolboxWidth As Integer = frmToolBox.Width
    Public Shared FrmToolboxHeight As Integer = frmToolBox.Height
    Public _zuweisungAktiv As Integer = 1

#Region "Form-Events"
    Private Sub frmToolBox_Load(sender As Object, e As EventArgs) Handles Me.Load

        Me.Location = INI.ToolBox_Rectangle.Location
        ' Me.Size = DpiScaleSize(MJ_FRMTOOLBOX_WIDTH, MJ_FRMTOOLBOX_HEIGHT)

        'TabControlToolBox.SelectedTab = TabPageWerkbank
        ' lblImageSpieler.Image = MjGfx_HGrdEinfügenGrün(32)

        Me.EnsureLocationVisibleOnAnyScreen()
        UpDateSelectedTabIcon()

        Helfer.DisableAllTabStops(Me)

        INI.ToolBox_VolatilFormToolBox = Me
        ''TODO SFD-Anpassung
        ''_toolboxBinder = New MousePollerHandleBinder(Spielfeld.SFD.MousePolling, Me)

        lblSpielSizeOK.Location = lblSpielSizeError.Location
        optSaveSpielesammlung.Visible = INI.Editor_SaveToSpielesammlungAllowed
        optSteinDesignTest.Visible = INI.Tile_AllowTileColorsTestFiles

        _zuweisungAktiv = 0
        '()
    End Sub
    Private Sub frmToolBox_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        frmMain.ShowOrHideToolboxAndUpdateToolboxButton()
        e.Cancel = True
    End Sub
    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        frmMain.ShowOrHideToolboxAndUpdateToolboxButton()
    End Sub
    Private Function DpiScale(ByVal value As Integer) As Integer
        Return CInt(Math.Round(value * Me.DeviceDpi / 96.0F))
    End Function

    Private Function DpiScaleSize(ByVal width As Integer,
                                  ByVal height As Integer) As Size
        Return New Size(DpiScale(width), DpiScale(height))
    End Function
    Private Sub frmToolBox_Shown(sender As Object, e As EventArgs) Handles Me.Shown

        With lblInfoGrenzen
            .Text = .Text.Replace("xx", GlobalConstants.MJ_STEINE_MAXX.ToString)
            .Text = .Text.Replace("yy", GlobalConstants.MJ_STEINE_MAXY.ToString)
            .Text = .Text.Replace("zz", GlobalConstants.MJ_STEINE_MAXZ.ToString)
        End With

        Num2UpDownSteineNebeneinander.MinValue = 1
        Num2UpDownSteineNebeneinander.MaxValue = GlobalConstants.MJ_STEINE_MAXX
        Num2UpDownSteineAufeinander.MinValue = 1
        Num2UpDownSteineAufeinander.MaxValue = GlobalConstants.MJ_STEINE_MAXY
        Num2UpDownSteineÜbereinander.MinValue = 1
        Num2UpDownSteineÜbereinander.MaxValue = GlobalConstants.MJ_STEINE_MAXZ

        UctlToolboxHintergrund1.InitialisierungAndUpdate()
        SFMain.SFDat.SFTool.SetConsumeTabpagePollEvent(ToolboxPollEvent.DoLoadSFInfoValues)

    End Sub
    Private Sub frmToolBox_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        If e.CloseReason = CloseReason.UserClosing Then
            e.Cancel = True     ' Schließen verhindern
            Me.Hide()
            INI.ToolBox_VolatilFormIsVisible = False
        End If
        INI.ToolBox_VolatilFormToolBox = Nothing
    End Sub

    Private Sub frmToolBox_LocationChanged(sender As Object, e As EventArgs) Handles Me.LocationChanged
        INI.ToolBox_Rectangle = New Rectangle(Me.Location, Me.Size)
    End Sub

#End Region

#Region "Verwaltung"

#Region "Icon-Lader"
    ''' <summary>
    ''' Lädt optional ein Icon aus My.Resources via Schlüssel "EnumName_Wert".
    ''' Skaliert auf ICON_SIZE falls konfiguriert.
    ''' </summary>
    Private Shared Function GetEnumIcon(enumName As String, valueName As String) As Image
        Return MjGfx_GfxMain($"{enumName}_{valueName}", ICON_SIZE)
    End Function
#End Region

#Region "Helpers"
    ' Aus Enum-Namen lesbare Beschriftung machen: "SpeichernUnter" → "Speichern unter"
    Private Shared Function CaptionFromEnumName(name As String) As String
        If String.IsNullOrWhiteSpace(name) Then Return String.Empty

        Dim sb As New Text.StringBuilder()
        sb.Append(name(0))
        For i As Integer = 1 To name.Length - 1
            Dim ch As Char = name(i)
            If Char.IsUpper(ch) Then
                sb.Append(" ")
                sb.Append(Char.ToLowerInvariant(ch))
            Else
                sb.Append(ch)
            End If
        Next
        ' Erste Worte groß
        Dim txt As String = sb.ToString().Trim()
        'alternativ:        Return Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(txt)
        Return txt
    End Function
#End Region

#Region "Panelhandling"

    Private Sub TabControlToolBox_SelectedIndexChanged(sender As Object, e As EventArgs) Handles TabControlToolBox.SelectedIndexChanged
        ''TODO SFD-Anpassung
        'If TabControlToolBox.SelectedTab Is TabPageNameSpeicherung Then
        '    SFMain.RenderMode = RenderMode.Spiel
        '    SFMain.SFDat.SFTool.SetConsumeTabpagePollEvent(ToolboxPollEvent.ChangedTabPage)

        'ElseIf TabControlToolBox.SelectedTab Is TabPageGröße Then
        '    SFMain.RenderMode = RenderMode.Edit
        '    SFMain.SFDat.SFTool.SetConsumeTabpagePollEvent(ToolboxPollEvent.ChangedTabPage)

        'ElseIf TabControlToolBox.SelectedTab Is TabPageHintergrund Then
        '    UctlToolboxHintergrund1.InitialisierungAndUpdate()

        'Else
        '    If Debugger.IsAttached Then
        '        Stop 'Programmierfehler: hinzugefügte TabPag auch hier hinzufügen.
        '    End If
        'End If

        'UpDateSelectedTabIcon()

    End Sub

    Private Sub UpDateSelectedTabIcon()

        ' Grafik des aktiven Reiters auf "aktiv" stellen, indem die passende Grafik geladen wird.
        '
        ' Die Lösung funktioniert nur dann zuverlässig, wenn folgende Voraussetzungen erfüllt sind:
        ' Bilder in der ImageList liegen in Paaren hintereinander
        ' Das „normale“ (inaktive) Symbol steht immer auf einer geraden Indexnummer:
        ' 0 = Spieler, 2 = Editor, 4 = Werkbank, …
        ' Das „aktive“ Symbol folgt direkt danach auf der ungeraden Indexnummer:
        ' 1 = SpielerAktiv, 3 = EditorAktiv, 5 = WerkbankAktiv, …
        ' 🔑 Damit ergibt sich immer das Schema „Basis-Icon auf gerade, Aktiv-Icon = Basis+1“.
        ' Die TabPages müssen im Designer oder per Code ihren ImageIndex zugewiesen bekommen
        ' Es reicht nicht, den ImageKey zu setzen, da der Umschaltcode ausschließlich mit den numerischen Indexwerten arbeitet (ImageIndex).
        ' Beispiel im Designer:
        ' TabPage „Editor“ → ImageIndex = 2
        ' TabPage „Werkbank“ → ImageIndex = 4

        ' Alle Tabs auf "inaktiv" setzen (Basis-Icon)
        For Each tp As TabPage In TabControlToolBox.TabPages
            Dim x As Integer = tp.ImageIndex
            ' wenn aktuell Aktiv-Icon (ungerade), dann -1 auf Basis-Icon
            If x Mod 2 = 1 Then tp.ImageIndex = x - 1
        Next

        ' Aktiven Tab auf "aktiv" setzen (Aktiv-Icon)
        With TabControlToolBox.SelectedTab
            Dim x As Integer = .ImageIndex
            ' wenn Basis-Icon (gerade), dann +1 auf Aktiv-Icon
            If x Mod 2 = 0 Then .ImageIndex = x + 1
        End With
    End Sub

#End Region

#End Region

#Region "Verarbeitung"

    Private _sfd As SFDaten
    Private _oldStatusSpielfeldSize As SFToolbox.ToolboxStatusSpielfeldSize
    Private Sub btnChangeSize_Click(sender As Object, e As EventArgs) Handles btnChangeSize.Click
        'Die Verarbeitung der Größenänderung muß im Rendertrakt eingetaktet
        'geschehen, weil es sonst zu Zugriffen auf die bereits geänderten Daten kommen kann,
        'was zum Programmabsturz führen kann. (geänderter arrFB)
        'Siehe auch ChangeSizePossible
        btnChangeSize.Enabled = False
        If ChangeSizePossible(GetStatusSpielfeldSize) Then
            With _sfd.SFTool
                .OldStatusSpielfeldSize = _oldStatusSpielfeldSize.DeepCopy
                .NewStatusSpielfeldSize = GetStatusSpielfeldSize()
                .SetConsumeTabpagePollEvent(ToolboxPollEvent.DoChangedSpielfeldSize)
            End With
            _oldStatusSpielfeldSize = GetStatusSpielfeldSize()
        End If
        btnChangeSize.Enabled = True
    End Sub

    Public Sub SetSfdAndCopyValuesToForm(sfd As SFDaten)
        _zuweisungAktiv += 1
        _sfd = sfd
        With _sfd.SFInf
            txtName.Text = .Name
            txtAnmerkung.Text = .Anmerkung
            '
            Num2UpDownSteineNebeneinander.Value = .SpielSizeInSteinen.x
            Num2UpDownSteineÜbereinander.Value = .SpielSizeInSteinen.y
            Num2UpDownSteineAufeinander.Value = .SpielSizeInSteinen.z
        End With
        _oldStatusSpielfeldSize = GetStatusSpielfeldSize()

        CopySfInfoToOptSteinDesign()
        _zuweisungAktiv -= 1
    End Sub

    Private Function GetStatusSpielfeldSize() As SFToolbox.ToolboxStatusSpielfeldSize
        Dim values As New SFToolbox.ToolboxStatusSpielfeldSize
        With values
            .x = Num2UpDownSteineNebeneinander.Value
            .y = Num2UpDownSteineÜbereinander.Value
            .z = Num2UpDownSteineAufeinander.Value
            .changeLeft = optChangeLeft.Checked
            .changeLeftRight = optChangeLeftRight.Checked
            .changeRight = optChangeRight.Checked
            .changeTop = optChangeTop.Checked
            .changeTopBottom = optChangeTopBottom.Checked
            .changeBottom = optChangeBottom.Checked
            .sizeInSteinen = New Triple(.x, .y, .z)
        End With
        Return values
    End Function

    Private Function ChangeSizePossible(val As SFToolbox.ToolboxStatusSpielfeldSize) As Boolean

        Dim addOrRemoveLeft As Integer
        Dim addOrRemoveTop As Integer
        Dim addOrRemoveRight As Integer
        Dim addOrRemoveBottom As Integer
        Dim addOrRemoveLayer As Integer
        Dim deltaX As Integer
        Dim deltaY As Integer

        With _oldStatusSpielfeldSize
            deltaX = val.x - .x 'das ist die Anzahl in Steinen, nicht in Spalten /Zeilen.
            deltaY = val.y - .y 'gebraucht wird in Spalten/Zeilen, daher gleich mal 2 nehmen.
            addOrRemoveLayer = val.z - .z
        End With

        If deltaX = 0 AndAlso deltaY = 0 AndAlso addOrRemoveLayer = 0 Then
            Return False
        End If

        If val.changeLeft Then
            addOrRemoveLeft = deltaX * 2 'Das ist in Spalten
            addOrRemoveRight = 0
        ElseIf val.changeLeftRight Then
            addOrRemoveLeft = deltaX  'Spalten verteilen
            addOrRemoveRight = deltaX
        Else 'changeRight
            addOrRemoveLeft = 0
            addOrRemoveRight = deltaX * 2 'Das ist in Spalten
        End If
        '
        If val.changeTop Then
            addOrRemoveTop = deltaY * 2
            addOrRemoveBottom = 0
        ElseIf val.changeTopBottom Then
            addOrRemoveTop = deltaY
            addOrRemoveBottom = deltaY
        Else 'changeRight
            addOrRemoveTop = 0
            addOrRemoveBottom = deltaY * 2
        End If

        Dim ok As Boolean

        If _sfd.SFInf.RedimArrFBIsPossible(
                                addOrRemoveLeft,
                                addOrRemoveTop,
                                addOrRemoveRight,
                                addOrRemoveBottom,
                                addOrRemoveLayer) Then

            ''Die eigentliche Änderung kann hier nicht durchgeführt werden,
            ''weil sie zeitlich unbestimmt durchgeführt wird und daher
            ''irgenwann im Rendertakt auftritt und das zum unbestimten
            'Zustand führt und zu Zugriffen auf einen arrFB, der sich bereits geändert
            ''haben könnte.
            ''Deshalb wird dieser Code in _sfd.SFTool ausgeführt,
            ''Ausgelößt durch  _sfd.SFTool.SetConsumeTabpagePollEvent(ToolboxPollEvent.DoChangedSpielfeldSize)
            ''_sfd.SFInf.ChangeSpielfeldSize(val.sizeInSteinen,
            ''                           addOrRemoveLeft,
            ''                            addOrRemoveTop,
            ''                            addOrRemoveRight,
            ''                            addOrRemoveBottom,
            ''                            addOrRemoveLayer)

            lblSpielSizeOK.Visible = True
            ok = True

        Else

            lblSpielSizeError.Visible = True
            ok = False
        End If

        tmrHideLblSpielSizeError.Start()

        Return ok
    End Function

    Private Sub tmrHideLblSpielSizeError_Tick(sender As Object, e As EventArgs) Handles tmrHideLblSpielSizeError.Tick
        tmrHideLblSpielSizeError.Stop()
        lblSpielSizeError.Visible = False
        lblSpielSizeOK.Visible = False
    End Sub

    '##### Texte ########################################################################################

    Private Sub txtNameAnmBeschr_TextChanged(sender As Object, e As EventArgs) _
        Handles txtName.TextChanged,
                txtAnmerkung.TextChanged

        If _zuweisungAktiv <> 0 Then Exit Sub

        With tmrTxtNameAnmBeschrDebouncer
            .Stop()
            .Start()
        End With

    End Sub

    Private Sub tmrTxtNameAnmBeschrDebouncer_Tick(sender As Object, e As EventArgs) _
        Handles tmrTxtNameAnmBeschrDebouncer.Tick

        tmrTxtNameAnmBeschrDebouncer.Stop()

        With _sfd.SFInf
            .Name = txtName.Text
            .Anmerkung = txtAnmerkung.Text
            _sfd.SFTool.SetConsumeTabpagePollEvent(ToolboxPollEvent.ChangedTexte)
        End With
    End Sub

    Private Sub btnSteineReset_Click(sender As Object, e As EventArgs) Handles btnSteineReset.Click
        _zuweisungAktiv += 1
        With _oldStatusSpielfeldSize
            Num2UpDownSteineNebeneinander.Value = .x
            Num2UpDownSteineÜbereinander.Value = .y
            Num2UpDownSteineAufeinander.Value = .z
        End With
        _zuweisungAktiv -= 1
    End Sub

    '#############################################################################################

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        'zuerst ein "Foto" des Spielfeldes erzeugen
        CopyOptSteinDesignToSFInf()
        _sfd.SFTool.SetConsumeTabpagePollEvent(ToolboxPollEvent.CreateSpielbildStep1SetHideGridSetCreateSpielbild)
        'Das dauert zwei Renderungen plus Vorlaufzeit = drei Renderungen plus Reserve
        '= vier Renderungen = mindestens 160 ms. Gewählt: 250 ms.
        tmrDelaySave.Interval = 250
        tmrDelaySave.Start()
    End Sub

    Private Sub tmrDelaySave_Tick(sender As Object, e As EventArgs) Handles tmrDelaySave.Tick
        tmrDelaySave.Stop()
        DoSave()
    End Sub

    Private Sub DoSave()

        Dim fullpath As String

        If optSaveEigeneSammlung.Checked Then
            fullpath = INI.BasisIni.AppDataFullPathSpiel(AppDataSubDir.Spiele, AppDataSubSubDir.EigeneSpiele, _sfd.SFInf.FileName)
            _sfd.SFInf.SpielStärke = GetSpielGrad()
            _sfd.SFInf.Save(fullpath)
            MsgBox("gespeichert", MsgBoxStyle.Information)

        ElseIf optSaveSpielesammlung.Checked Then
            _sfd.SFInf.SpielStärke = GetSpielGrad()
            fullpath = INI.BasisIni.AppDataFullPathSpiel(AppDataSubDir.Spiele, AppDataSubSubDir.Spielesammlung, _sfd.SFInf.FileName)
            _sfd.SFInf.Save(fullpath)
            MsgBox("gespeichert", MsgBoxStyle.Information)

        ElseIf optSaveAußerhalb.Checked Then
            Dim sfd As New SaveFileDialog
            With sfd
                .DefaultExt = "xml"
                .FileName = _sfd.SFInf.FileName
                .Filter = "xml|xml"
                .InitialDirectory = SpecialDirectories.MyDocuments
                If .ShowDialog = DialogResult.OK Then
                    _sfd.SFInf.Save(.FileName)
                End If
            End With
        End If
    End Sub

    Private Function GetSpielGrad() As SpielStärke
        If optLeicht.Checked Then
            Return SpielStärke.Leicht
        ElseIf optMittel.Checked Then
            Return SpielStärke.Mittel
        ElseIf optSchwer.Checked Then
            Return SpielStärke.Schwer
        ElseIf optProfi.Checked Then
            Return SpielStärke.Profi
        Else
            Return SpielStärke.Ohne
        End If
    End Function

    Private Sub CopyOptSteinDesignToSFInf()

        Dim sf As SteinFont, sd As SteinDesign, ss As SteinSatz

        If optSteinFontNoto.Checked Then
            sf = SteinFont.Noto
        Else
            sf = SteinFont.Segoe
        End If

        If optSteinDesignDefault.Checked Then
            sd = SteinDesign.Default

        ElseIf optSteinDesignUniDunkel.Checked Then
            sd = SteinDesign.UniDunkel

        ElseIf optSteinDesignUniGelb.Checked Then
            sd = SteinDesign.UniGelbBraun

        ElseIf optSteinDesignEditorSpezial.Checked Then
            sd = SteinDesign.EditorSpezial

        ElseIf optSteinDesignTest.Checked Then
            sd = SteinDesign.Test
        Else
            sd = SteinDesign.Default
        End If

        _sfd.SFInf.SetDesignForSpielfeldPicture(sf, sd, ss)

    End Sub

    Private Sub CopySfInfoToOptSteinDesign()

        Dim sf As SteinFont = _sfd.SFInf.SpielfeldPictureSteinFont
        Dim sd As SteinDesign = _sfd.SFInf.SpielfeldPictureSteinDesign
        Dim ss As SteinSatz = _sfd.SFInf.SpielfeldPictureSteinSatz

        If Not _sfd.SFInf.SpielfeldPictureSteinIsInit Then
            _sfd.SFInf.SpielfeldPictureSteinFont = INI.Tile_SteinFont
            _sfd.SFInf.SpielfeldPictureSteinDesign = INI.Tile_SteinDesign
            _sfd.SFInf.SpielfeldPictureSteinSatz = INI.Tile_SteinSatz
            sf = INI.Tile_SteinFont
            sd = INI.Tile_SteinDesign
            ss = INI.Tile_SteinSatz
        End If

        Select Case sf
            Case SteinFont.Noto : optSteinFontNoto.Checked = True
            Case Else : optSteinFontSegeo.Checked = True
        End Select

        Select Case sd
            Case SteinDesign.Default
                optSteinDesignDefault.Checked = True

            Case SteinDesign.UniDunkel
                optSteinDesignUniDunkel.Checked = True

            Case SteinDesign.UniGelbBraun
                optSteinDesignUniGelb.Checked = True

            Case SteinDesign.EditorSpezial
                optSteinDesignEditorSpezial.Checked = True
            Case SteinDesign.Test
                optSteinDesignTest.Checked = True
            Case Else
                optSteinDesignDefault.Checked = True

        End Select

        Select Case ss
            Case SteinSatz.Light
                optSteinSatzLight.Checked = True
            Case SteinSatz.Dark
                optSteinSatzDark.Checked = True
            Case Else
                optSteinSatzMedium.Checked = True
        End Select

    End Sub

#End Region

End Class