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
#Disable Warning IDE0079
#Disable Warning IDE1006


Imports System.ComponentModel
Imports MahjongGK.MjMix


Public Class frmToolBox


    Public Const MJ_FRMTOOLBOX_WIDTH As Integer = 400
    Public Const MJ_FRMTOOLBOX_HEIGHT As Integer = 580
    Public Const MJ_FRMTOOLBOX_PADDING_LEFT As Integer = 15
    Public Const MJ_FRMTOOLBOX_PADDING_BOTTOM As Integer = 20
    Public Const MJ_FRMTOOLBOX_PANEL_ As Integer = 15

    Private _themer As Theme.ThemeManager
    Private _aktVisibleUserControl As VisibleUserControl

    Private _toolboxBinder As OverlayHandleBinder

    Private _aktBasisformEnum As BasisformEnum

#Region "Maps für Enable/Disable"
    Private ReadOnly _mapEditorFile As New Dictionary(Of EditorFileCmd, ToolStripMenuItem)
    Private ReadOnly _mapWerkbkFile As New Dictionary(Of WerkbkFileCmd, ToolStripMenuItem)
    Private ReadOnly _mapBasisform As New Dictionary(Of BasisformEnum, ToolStripMenuItem)
    Private ReadOnly _mapPlatzEditor As New Dictionary(Of PlatzhalterEditor, ToolStripMenuItem)
    Private ReadOnly _mapPlatzWerkbk As New Dictionary(Of PlatzhalterWerkbank, ToolStripMenuItem)
#End Region

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

#Region "Form-Events"
    Private Sub frmToolBox_Load(sender As Object, e As EventArgs) Handles Me.Load


        Me.Location = INI.ToolBox_Rectangle.Location
        Me.Width = MJ_FRMTOOLBOX_WIDTH
        Me.Height = MJ_FRMTOOLBOX_HEIGHT


        TabControlToolBox.SelectedTab = TabPageWerkbank
        ' lblImageSpieler.Image = MjGfx_HGrdEinfügenGrün(32)

        Me.EnsureLocationVisibleOnAnyScreen()
        BuildEditorMenu()
        BuildWerkbkMenu()
        UpDateSelectedTabIcon()

        _themer = New Theme.ThemeManager(Me, If(INI.Global_DarkMode, AppTheme.Dark, AppTheme.Light),
                                   useOwnerDrawTabs:=True,        ' auf True, wenn Tabs farbig gezeichnet werden sollen
                                   customizeProgressBar:=True, INI.Global_BrightenAmount)
        _themer.RegisterToolTip(ToolTipToolBox)
        _themer.RegisterImageList(ImageListToolBox)

        MjMix.DisableAllTabStops(Me)

        DoBasisformen(ToolBox_AktBasisform)

        _toolboxBinder = New OverlayHandleBinder(Spielfeld.SFD.MousePolling, Me)

    End Sub

    Private Sub frmToolBox_Shown(sender As Object, e As EventArgs) Handles Me.Shown

        UctlToolboxHintergrund1.InitialisierungAndUpdate()
    End Sub
    Private Sub frmToolBox_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        If e.CloseReason = CloseReason.UserClosing Then
            e.Cancel = True     ' Schließen verhindern
        End If
    End Sub

    Private Sub frmToolBox_LocationChanged(sender As Object, e As EventArgs) Handles Me.LocationChanged
        INI.ToolBox_Rectangle = New Rectangle(Me.Location, Me.Size)
    End Sub

#End Region

#Region "Verwaltung"

#Region "Builder – EDITOR"
    Private Sub BuildEditorMenu()
        MenuStrip_Editor.SuspendLayout()
        MenuStrip_Editor.Items.Clear()
        _mapEditorFile.Clear()
        _mapPlatzEditor.Clear()

        ' Datei
        Dim mDatei As New ToolStripMenuItem("Datei")
        For Each cmd As EditorFileCmd In [Enum].GetValues(GetType(EditorFileCmd))
#Disable Warning IDE0017 ' Initialisierung von Objekten vereinfachen
            Dim tsmi As New ToolStripMenuItem(CaptionFromEnumName(cmd.ToString())) With {
                .Name = "tsmiEditor_" & cmd.ToString(),
                .Tag = cmd,
                .ShowShortcutKeys = False
            }
#Enable Warning IDE0017 ' Initialisierung von Objekten vereinfachen
            ' Icon: "EditorFileCmd_<Wert>"
            tsmi.Image = GetEnumIcon(GetType(EditorFileCmd).Name, cmd.ToString())
            AddHandler tsmi.Click, AddressOf OnEditorFileClick
            mDatei.DropDownItems.Add(tsmi)
            _mapEditorFile(cmd) = tsmi
        Next
        MenuStrip_Editor.Items.Add(mDatei)

        ' PlatzhalterEditor
        Dim mPlatz As New ToolStripMenuItem("PlatzhalterEditor")
        For Each it As PlatzhalterEditor In [Enum].GetValues(GetType(PlatzhalterEditor))
#Disable Warning IDE0017 ' Initialisierung von Objekten vereinfachen
            Dim tsmi As New ToolStripMenuItem(CaptionFromEnumName(it.ToString())) With {
                .Name = "tsmiEditor_" & it.ToString(),
                .Tag = it,
                .ShowShortcutKeys = False
            }
#Enable Warning IDE0017 ' Initialisierung von Objekten vereinfachen
            ' Icon: "PlatzhalterEditor_<Wert>"
            tsmi.Image = GetEnumIcon(GetType(PlatzhalterEditor).Name, it.ToString())
            AddHandler tsmi.Click, AddressOf OnPlatzhalterEditorClick
            mPlatz.DropDownItems.Add(tsmi)
            _mapPlatzEditor(it) = tsmi
        Next
        MenuStrip_Editor.Items.Add(mPlatz)

        MenuStrip_Editor.ResumeLayout()
    End Sub
#End Region

#Region "Builder – WERKBANK"
    Private Sub BuildWerkbkMenu()

        MenuStrip_Werkbk.SuspendLayout()
        MenuStrip_Werkbk.Items.Clear()
        _mapWerkbkFile.Clear()
        _mapBasisform.Clear()
        _mapPlatzWerkbk.Clear()

        ' Datei
        Dim mDatei As New ToolStripMenuItem("Datei")
        For Each cmd As WerkbkFileCmd In [Enum].GetValues(GetType(WerkbkFileCmd))
#Disable Warning IDE0017 ' Initialisierung von Objekten vereinfachen
            Dim tsmi As New ToolStripMenuItem(CaptionFromEnumName(cmd.ToString())) With {
                .Name = "tsmiWerkBk_" & cmd.ToString(),
                .Tag = cmd,
                .ShowShortcutKeys = False
            }
#Enable Warning IDE0017 ' Initialisierung von Objekten vereinfachen
            ' Icon: "WerkbkFileCmd_<Wert>"
            tsmi.Image = GetEnumIcon(GetType(WerkbkFileCmd).Name, cmd.ToString())
            AddHandler tsmi.Click, AddressOf OnWerkbkFileClick
            mDatei.DropDownItems.Add(tsmi)
            _mapWerkbkFile(cmd) = tsmi
        Next
        MenuStrip_Werkbk.Items.Add(mDatei)

        ' Basisformen
        Dim mBasis As New ToolStripMenuItem("Basisformen")
        For Each b As BasisformEnum In [Enum].GetValues(GetType(BasisformEnum))
#Disable Warning IDE0017 ' Initialisierung von Objekten vereinfachen
            Dim tsmi As New ToolStripMenuItem(CaptionFromEnumName(b.ToString())) With {
                .Name = "tsmiWerkBk_Basisform_" & b.ToString(),
                .Tag = b,
                .ShowShortcutKeys = False
            }
#Enable Warning IDE0017 ' Initialisierung von Objekten vereinfachen
            ' Icon: "Basisform_<Wert>"
            tsmi.Image = GetEnumIcon(GetType(BasisformEnum).Name, b.ToString())
            AddHandler tsmi.Click, AddressOf OnBasisformClick
            mBasis.DropDownItems.Add(tsmi)
            _mapBasisform(b) = tsmi
        Next
        MenuStrip_Werkbk.Items.Add(mBasis)

        ' PlatzhalterWerkbank
        Dim mPlatz As New ToolStripMenuItem("PlatzhalterWerkbank")
        For Each it As PlatzhalterWerkbank In [Enum].GetValues(GetType(PlatzhalterWerkbank))
#Disable Warning IDE0017 ' Initialisierung von Objekten vereinfachen
            Dim tsmi As New ToolStripMenuItem(CaptionFromEnumName(it.ToString())) With {
                .Name = "tsmiWerkBk_" & it.ToString(),
                .Tag = it,
                .ShowShortcutKeys = False
            }
#Enable Warning IDE0017 ' Initialisierung von Objekten vereinfachen
            ' Icon: "PlatzhalterWerkbank_<Wert>"
            tsmi.Image = GetEnumIcon(GetType(PlatzhalterWerkbank).Name, it.ToString())
            AddHandler tsmi.Click, AddressOf OnPlatzhalterWerkbkClick
            mPlatz.DropDownItems.Add(tsmi)
            _mapPlatzWerkbk(it) = tsmi
        Next
        MenuStrip_Werkbk.Items.Add(mPlatz)

        MenuStrip_Werkbk.ResumeLayout()
    End Sub
#End Region

#Region "Icon-Lader"
    ''' <summary>
    ''' Lädt optional ein Icon aus My.Resources via Schlüssel "EnumName_Wert".
    ''' Skaliert auf ICON_SIZE falls konfiguriert.
    ''' </summary>
    Private Shared Function GetEnumIcon(enumName As String, valueName As String) As Image
        Return MjGfx_GfxMain($"{enumName}_{valueName}", ICON_SIZE)
    End Function
#End Region

#Region "Klick-Delegation → Enum-Methoden"
    ' Editor/Datei
    Private Sub OnEditorFileClick(sender As Object, e As EventArgs)
        Dim cmd As EditorFileCmd = CType(DirectCast(sender, ToolStripMenuItem).Tag, EditorFileCmd)
        DoEditorFile(cmd)
    End Sub

    ' Werkbank/Datei
    Private Sub OnWerkbkFileClick(sender As Object, e As EventArgs)
        Dim cmd As WerkbkFileCmd = CType(DirectCast(sender, ToolStripMenuItem).Tag, WerkbkFileCmd)
        DoWerkbkFile(cmd)
    End Sub

    ' Werkbank/Basisformen
    Private Sub OnBasisformClick(sender As Object, e As EventArgs)
        Dim b As BasisformEnum = CType(DirectCast(sender, ToolStripMenuItem).Tag, BasisformEnum)
        DoBasisformen(b)
    End Sub

    ' Platzhalter
    Private Sub OnPlatzhalterEditorClick(sender As Object, e As EventArgs)
        Dim it As PlatzhalterEditor = CType(DirectCast(sender, ToolStripMenuItem).Tag, PlatzhalterEditor)
        DoPlatzhalterEditor(it)
    End Sub

    Private Sub OnPlatzhalterWerkbkClick(sender As Object, e As EventArgs)
        Dim it As PlatzhalterWerkbank = CType(DirectCast(sender, ToolStripMenuItem).Tag, PlatzhalterWerkbank)
        DoPlatzhalterWerkbank(it)
    End Sub
#End Region

#Region "Enable/Disable – exakt pro Enum"
    Public Sub DoEditorFile_Enabled(cmd As EditorFileCmd, flag As Boolean)
        If _mapEditorFile.TryGetValue(cmd, Nothing) Then _mapEditorFile(cmd).Enabled = flag
    End Sub

    Public Sub DoWerkBkFile_Enabled(cmd As WerkbkFileCmd, flag As Boolean)
        If _mapWerkbkFile.TryGetValue(cmd, Nothing) Then _mapWerkbkFile(cmd).Enabled = flag
    End Sub

    Public Sub DoBasisformen_Enabled(bform As BasisformEnum, flag As Boolean)
        If _mapBasisform.TryGetValue(bform, Nothing) Then _mapBasisform(bform).Enabled = flag
    End Sub

    Public Sub DoPlatzhalterEditor_Enabled(item As PlatzhalterEditor, flag As Boolean)
        If _mapPlatzEditor.TryGetValue(item, Nothing) Then _mapPlatzEditor(item).Enabled = flag
    End Sub

    Public Sub DoPlatzhalterWerkbank_Enabled(item As PlatzhalterWerkbank, flag As Boolean)
        If _mapPlatzWerkbk.TryGetValue(item, Nothing) Then _mapPlatzWerkbk(item).Enabled = flag
    End Sub
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

        If TabControlToolBox.SelectedTab Is TabPageSpieler Then
            Spielfeld.SFD.ToolboxTabPageChanged = True
            Spielfeld.SFD.AktRendering = RenderingEnum.Spielfeld

        ElseIf TabControlToolBox.SelectedTab Is TabPageEditor Then
            Spielfeld.SFD.ToolboxTabPageChanged = True
            Spielfeld.SFD.AktRendering = RenderingEnum.Editor

        ElseIf TabControlToolBox.SelectedTab Is TabPagePositionierer Then
            Spielfeld.SFD.ToolboxTabPageChanged = True
            Spielfeld.SFD.AktRendering = RenderingEnum.Editor

        ElseIf TabControlToolBox.SelectedTab Is TabPageWerkbank Then
            Spielfeld.SFD.ToolboxTabPageChanged = True
            Spielfeld.SFD.AktRendering = RenderingEnum.Werkbank

        ElseIf TabControlToolBox.SelectedTab Is TabPageHintergrund Then
            UctlToolboxHintergrund1.InitialisierungAndUpdate()

        Else
            If Debugger.IsAttached Then
                Stop 'Programmierfehler: hinzugefügte TabPag auch hier hinzufügen.
            End If
        End If

        UpDateSelectedTabIcon()

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

#Region "Enum-Aktionen – hier die Verarbeitung der Menü-Aufrufe"
    ' Editor/Datei – EIN EINZIGER Einstiegspunkt, sauber per Select Case
    Private Sub DoEditorFile(cmd As EditorFileCmd)
        Select Case cmd
            Case EditorFileCmd.LadenInterne
                ' TODO
            Case EditorFileCmd.LadenEigene
                ' TODO
            Case EditorFileCmd.Speichern
                ' TODO
            Case EditorFileCmd.SpeichernUnter
                ' TODO
        End Select
    End Sub

    ' Werkbank/Datei
    Private Sub DoWerkbkFile(cmd As WerkbkFileCmd)
        Select Case cmd
            Case WerkbkFileCmd.LadenInterne
                ' TODO
            Case WerkbkFileCmd.LadenEigene
                ' TODO
            Case WerkbkFileCmd.Speichern
                ' TODO
            Case WerkbkFileCmd.Speichern_unter
                ' TODO
        End Select
    End Sub

    ' Werkbank/Basisformen 
    Private Sub DoBasisformen(bform As BasisformEnum)

        txtNameBasisform.Text = bform.ToString
        txtNameBasisformForSaving.Text = INI.ToolBox_NameBasisformForSaving(bform)

        INI.Debug_StopRenderingOnce = True

        UCtlWerkbank1.SetBasisformEnum(bform)

    End Sub

    ' Platzhalter
    Private Sub DoPlatzhalterEditor(item As PlatzhalterEditor)
        Select Case item
            Case PlatzhalterEditor.Item1
                ' TODO
            Case PlatzhalterEditor.Item2
                ' TODO
        End Select
    End Sub

    Private Sub DoPlatzhalterWerkbank(item As PlatzhalterWerkbank)
        Select Case item
            Case PlatzhalterWerkbank.Item1
                ' TODO
            Case PlatzhalterWerkbank.Item2
                ' TODO
        End Select
    End Sub

#End Region

#Region "Helfer"

    Private Sub frmToolBox_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing

        frmMain.ShowOrHideToolboxAndUpdateToolboxButton()

    End Sub




#End Region

#End Region



End Class