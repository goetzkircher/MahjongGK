Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On
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

Imports System.Drawing.Imaging



#Disable Warning IDE0079
#Disable Warning IDE1006

Public Class frmMain


#Region "Einstiegspunkt und Tests während der Programmentwicklung"

    ''' <summary>
    ''' Hier ist der Einstieg ins Spiel
    ''' </summary>
    ''' <param name="startRenderingWithOrToggleTo"></param>
    Private Sub InitialisierungSpielfeldEditorAndWerkbank(startRenderingWithOrToggleTo As RenderingEnum)
        If Not _initialisierungSpielfeldEditorAndWerkbankIsDone Then

            'Hier werden die Anfangs-Spiele/Editor/Werkbanksdaten zugewiesen
            Spielfeld.TestDaten_Spielfeld_Methodenaufruf_zum_Debuggen()

            _initialisierungSpielfeldEditorAndWerkbankIsDone = True
        End If
        Select Case startRenderingWithOrToggleTo
            Case RenderingEnum.Spielfeld
                AktVisibleUserControl = VisibleUserControl.Spielfeld
                Spielfeld.AktRendering = RenderingEnum.Spielfeld

            Case RenderingEnum.Editor
                AktVisibleUserControl = VisibleUserControl.Editor
                Spielfeld.AktRendering = RenderingEnum.Editor
            Case RenderingEnum.Werkbank
                AktVisibleUserControl = VisibleUserControl.Werkbank
                Spielfeld.AktRendering = RenderingEnum.Werkbank
            Case Else
        End Select
    End Sub

    '######################################################################################################

    Private Sub Toggle()
        If Spielfeld.AktRendering = RenderingEnum.Spielfeld Then
            Spielfeld.AktRendering = RenderingEnum.Werkbank
        Else
            Spielfeld.AktRendering = RenderingEnum.Spielfeld
        End If
    End Sub

    Private Sub Test1(value As Boolean)
        Dim icr As New IniCommentReader(INI.AllIniManagersFullPath)
        Dim sb As New System.Text.StringBuilder
        sb.Append(icr.GetKommentar(NameOf(INI.Editor_UsingEditor), seperator:=CommentSeperator.CrLf))
        sb.Append(icr.GetKommentar(NameOf(INI.IfRunningInIDE_Grafik16x16Directory_Ressourcen), seperator:=CommentSeperator.Tilde))
        sb.Append(icr.GetKommentar(NameOf(INI.IfRunningInIDE_Grafik16x16Directory_Other), seperator:=CommentSeperator.Tilde))
        sb.Append(icr.GetKommentar(NameOf(INI.Rendering_BitmapHighQuality), seperator:=CommentSeperator.Tilde))
        sb.Append(icr.GetKommentar(NameOf(INI.Sonstiges_AppGrafikSatz), seperator:=CommentSeperator.Tilde))
        MessageBoxFormatiert.ShowInfo(sb.ToString, "Kommentare")
    End Sub

    Private Sub Test2()
        INI.Editor_UsingEditorAllowed = Not INI.Editor_UsingEditorAllowed
    End Sub

    Private Sub Go()
        InitialisierungSpielfeldEditorAndWerkbank(startRenderingWithOrToggleTo:=RenderingEnum.Spielfeld)
    End Sub

    Private Sub Parkplatz()

        'Using tst As New MahjongGKSymbolFactory.TileStyleTuner
        '    tst.ShowDialog()
        'End Using




        'SpielfeldTest_SpielsteinGenerator.RunAll()

        'Dim gen1 As New SpielsteinGenerator(visibleAreaMaxLength:=30, generatorMode:=GeneratorModi.StoneStream_Base152_Continuous)
        'Dim gen2 As New SpielsteinGenerator(visibleAreaMaxLength:=30, generatorMode:=GeneratorModi.StoneSet_144)

        'Dim stat As New Statistik(gen1.Vorrat, gen2.Vorrat)
        'MessageBoxFormatiert.ShowInfoMonoSpaced(stat.ToString(deltaProz144:=True), "Spielsteinverteilung")
        'MessageBoxFormatiert.ShowInfoMonoSpaced(Spielfeld.DebugKonstantenString, "Spielsteinvariable")
    End Sub



#End Region



    'Im Hauptformular sind In im Me.Load- Ereignis einige wenige Initialisierungen angesiedelt. 
    'Außerdem die Erstellung und Verwaltung des Hauptmenues, das dann die Befehle an die einzenen UserControls weitergibt,
    'sowie das Ein- und Aushängen (= sichtbar/unsichtbar machen) der verschiedenen UserControls In das Hauptformular,
    'alles also reine Verwaltungsarbeit.
    'Die Programmlogik ist komplett In den verschiedenen UserControls angesiedelt, von wo aus auf einen gemeinsamen Pool zugegriffen
    'wird, der sich überwiegend im Verzeichnis "Namespace Spielfeld" befindet.

#Region "Initialisierungen"

    Public Enum VisibleUserControl
        None = -1
        Spielfeld
        Editor
        Werkbank
        Einstellungen
        SpielfeldWählen
        Hilfe
        Über
    End Enum

    Private Enum Info
        AutoSave
    End Enum

    ''' <summary>
    ''' Das TabControlMain ist nur im Designer sichtbar, damit die UserControls
    ''' im Designer angezeigt werden können.
    ''' Zur Laufzeit wird es abgeschaltet, und das gerade benötigte UserControls
    ''' wird dem FrmMain zugeordnet. Die UserControls werden hier gespeichert
    ''' </summary>
    Private VisibleUserControls As New List(Of Control)
    Private _isRefreshing As Boolean 'bezieht sich auf das ToolStrip nach Änderungen in der Ini
    Private ReadOnly _cbToolTip As New ToolTip()
    Private _initialisierungSpielfeldEditorAndWerkbankIsDone As Boolean
    Private _frmToolBox As frmToolBox = Nothing

    Sub New()

        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()

        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.

    End Sub


    Private Sub frmMain_Load(sender As Object, e As EventArgs) Handles Me.Load

        'Aktualisierung der Titelzeile
        If My.Application.IsNetworkDeployed = True Then
            Me.Text &= " " & My.Application.Deployment.CurrentVersion.ToString
        Else
            Me.Text &= " " & Helper.ReadClickOnceVersionFromManifest()
        End If
        'stellt ggf "[IDE]" der Titelzeile voran.
        Helper.IsRunningInIDE(Me)

        'Prüfung, ob die Form auf den Bildschirm passt und ggf Anpassung. 
        Dim wa As Rectangle = Screen.FromControl(Me).WorkingArea
        Dim maxWidth As Integer = CInt(wa.Width * 0.8)
        Dim maxHeight As Integer = CInt(wa.Height * 0.8)

        If Me.Width > maxWidth Then Me.Width = maxWidth
        If Me.Height > maxHeight Then Me.Height = maxHeight

        Me.MinimumSize = Me.SizeFromClientSize(New Size(MJ_SPIELFELD_MIN_WIDTH, MJ_SPIELFELD_MIN_HEIGHT))
        Me.AutoScaleMode = AutoScaleMode.Dpi

        'Die UserControls müssen in der Reihenfolge der Enumeration VisibleUserControl
        'in die Liste eingefügt werden, damit sie später in der richtigen Reihenfolge
        'aufgerufen werden können.
        VisibleUserControls.Add(UCtlSpielfeldMain)
        UCtlSpielfeldMain.Parent = Nothing

        VisibleUserControls.Add(UCtlEditorMain)
        UCtlEditorMain.Parent = Nothing

        VisibleUserControls.Add(UCtlWerkbankMain)
        UCtlWerkbankMain.Parent = Nothing

        VisibleUserControls.Add(UCtlEinstellungenMain)
        UCtlEinstellungenMain.Parent = Nothing

        '
        'Hier weitere UserControls hinzufügen, die im TabControlMain
        'im Designer sichtbar sind.

        'Das TabControlMain entsorgen
        TabControlMain.Parent = Nothing
        TabControlMain.Dispose()

        'und das erste UserControl anzeigen

        AktVisibleUserControl = VisibleUserControl.Spielfeld


        AddHandler INI.EventsOnly_RefreshUINachIniÄnderung_Event, AddressOf RefreshUINachIniÄnderung
        AddHandler INI.Rendering_AktMaxSteineXYZ_Event, AddressOf RefreshToolStripExMainAktMaxSteineXYZ
        AddHandler INI.RuntimeOnly_AktRendering_Event, AddressOf UpdateGrpEditorButtons
        AddHandler INI.RuntimeOnly_ToolboxAktiv_Event, AddressOf ShowOrHideToolboxAndUpdateToolboxButton
        '()

        'Das Menue wird dynamisch erzeugt, damit es
        'übersichtlicher wird, als die statische Erzeugung im Designer.
        BuildMenu(Me.MenuStripExMain)

        BuildBottomToolStrip()

        ' Startzustand setzen

        'Spielfeld.TestDaten_StatischesSpielfeld_EineEbenen

        If Debugger.IsAttached Then
            Me.KeyPreview = True
            DebugStep.Attach(Me)
        End If
        '
        'Die INI ist bereits initialisiert, das passiert beim allererstem Zugriff auf einen Wert automatisch.
        'hier geht es um um eine Reinitialisierung mit Werfen der IniEvents.
        INI.Initialisierung(update:=True, raiseIniEventsDefault:=IniEvents.OnChangeValue)

        Me.EnsureLocationVisibleOnAnyScreen()

    End Sub

    Private Sub frmMain_Disposed(sender As Object, e As EventArgs) Handles Me.Disposed
        RemoveHandler INI.EventsOnly_RefreshUINachIniÄnderung_Event, AddressOf RefreshUINachIniÄnderung
        RemoveHandler INI.Rendering_AktMaxSteineXYZ_Event, AddressOf RefreshToolStripExMainAktMaxSteineXYZ
        RemoveHandler INI.RuntimeOnly_AktRendering_Event, AddressOf UpdateGrpEditorButtons
        RemoveHandler INI.RuntimeOnly_ToolboxAktiv_Event, AddressOf ShowOrHideToolboxAndUpdateToolboxButton
    End Sub

#End Region

#Region "HauptMenue"

    ' --- Properties, Felder, Enums ---
    Private menuEnableBindings As New List(Of Tuple(Of ToolStripMenuItem, Func(Of Boolean)))
    Private menuVisibleBindings As New List(Of Tuple(Of ToolStripMenuItem, Func(Of Boolean)))


    Private Sub ChangeVisibleControl(ctrl As VisibleUserControl)
        AktVisibleUserControl = ctrl
    End Sub

    Private _aktVisibleUserControlValue As VisibleUserControl = VisibleUserControl.None
    ' Property mit automatischem Menü-Refresh
    Private Property AktVisibleUserControl As VisibleUserControl
        Get
            Return _aktVisibleUserControlValue
        End Get

        Set(value As VisibleUserControl)

            SuspendLayout()

            'Ein eventuell noch laufendes Spielfeld abschalten
            Spielfeld.PaintSpielfeld_DeInitialisierung()


            'Das bisherige UserControl aushängen. Beim Initialisieren ist das bisherige 
            'VisibleUserControl.None
            Try
                If _aktVisibleUserControlValue <> VisibleUserControl.None Then
                    If Not IsNothing(VisibleUserControls(_aktVisibleUserControlValue)) Then
                        VisibleUserControls(_aktVisibleUserControlValue).Parent = Nothing
                    End If
                End If
            Catch ex As Exception

            End Try

            Try
                'Und das neue Control einhängen.
                VisibleUserControls(value).Parent = Me.PanelFrmMainUGrd
                _aktVisibleUserControlValue = value

            Catch ex As Exception
                If Debugger.IsAttached Then
                    Stop 'Das UserControl existiert noch nicht.
                End If
            End Try

            RefreshMenuStates()

            'Erläuterung siehe PaintLimiterErläuterung.txt
            If value = VisibleUserControl.Spielfeld OrElse
                value = VisibleUserControl.Editor OrElse
                value = VisibleUserControl.Werkbank Then

                Spielfeld.PaintSpielfeld_Initialisierung(VisibleUserControls(value), value)
            End If

            ResumeLayout(True)

        End Set
    End Property

    ' --- Spezial-Label für rechtsbündige Menüs ---
    Private Class ToolStripSpringLabel
        Inherits ToolStripLabel
        Protected Overrides Sub OnLayout(e As LayoutEventArgs)
            MyBase.OnLayout(e)
            If Me.Owner IsNot Nothing Then
                Dim springSpace As Integer = Me.Owner.DisplayRectangle.Width
                For Each item As ToolStripItem In Me.Owner.Items
                    If item IsNot Me AndAlso item.Alignment = ToolStripItemAlignment.Left Then
                        springSpace -= item.Width
                    End If
                Next
                Me.Width = Math.Max(springSpace, 0)
            End If
        End Sub
    End Class

    ' --- Menüaufbau ---
    Private Sub BuildMenu(ms As MenuStrip)

        ms.Items.Clear()
        menuEnableBindings.Clear()
        menuVisibleBindings.Clear()

        ' === Datei ===
        Dim mnuDatei As New ToolStripMenuItem("Datei")
        mnuDatei.DropDownItems.Add(CreateMenuItem("Letzten Spielstand laden", Sub() SpielstandLoad(False)))
        mnuDatei.DropDownItems.Add(CreateMenuItem("Spielstand laden", Sub() SpielstandLoad(True)))
        mnuDatei.DropDownItems.Add(New ToolStripSeparator())
        mnuDatei.DropDownItems.Add(CreateMenuItem("Spielstand speichern", Sub() SpielstandSave(False)))
        mnuDatei.DropDownItems.Add(CreateMenuItem("Spielstand speichern unter", Sub() SpielstandSave(True)))
        mnuDatei.DropDownItems.Add(CreateCheckMenuItem("Automatisch speichern",
                                                       INI.Spielbetrieb_AutoSave,
                                                       Sub(sender)
                                                           Dim itm As ToolStripMenuItem = DirectCast(sender, ToolStripMenuItem)
                                                           INI.Spielbetrieb_AutoSave = itm.Checked
                                                       End Sub))
        mnuDatei.DropDownItems.Add(CreateMenuItem("Info", Sub() ShowInfo(Info.AutoSave)))

        ' === Spiel ===
        Dim mnuSpiel As New ToolStripMenuItem("Spiel")
        mnuSpiel.DropDownItems.Add(CreateMenuItem("Spielen",
                                                  Sub() ChangeVisibleControl(VisibleUserControl.Spielfeld),
                                                   Function() As Boolean
                                                       Return AktVisibleUserControl <> VisibleUserControl.Spielfeld
                                                   End Function))

        mnuSpiel.DropDownItems.Add(CreateMenuItem("Spielfeld wählen",
                                                  Sub() ChangeVisibleControl(VisibleUserControl.SpielfeldWählen),
                                                  Function() As Boolean
                                                      Return AktVisibleUserControl <> VisibleUserControl.SpielfeldWählen
                                                  End Function))

        mnuSpiel.DropDownItems.Add(CreateMenuItem("Spielfeld zufällig wählen", Sub() SelectRandomSpielfeld()))


        Dim mnuEditor As New ToolStripMenuItem("Editor")

        Dim editorItem As ToolStripMenuItem = CreateMenuItem("Editor",
                                                         Sub() ChangeVisibleControl(VisibleUserControl.Editor),
                                                         Function() As Boolean
                                                             Return AktVisibleUserControl <> VisibleUserControl.Editor
                                                         End Function)



        ' Hier wird geprüft, ob der Editor verwendet werden darf
        menuVisibleBindings.Add(New Tuple(Of ToolStripMenuItem, Func(Of Boolean))(
                                                    editorItem,
                                                    Function() As Boolean
                                                        Return INI.Editor_UsingEditorAllowed
                                                    End Function
                                                    ))
        mnuEditor.DropDownItems.Add(editorItem)

        ' === Einstellungen ===
        Dim mnuEinstellungen As New ToolStripMenuItem("Einstellungen")
        AddHandler mnuEinstellungen.Click, Sub() ChangeVisibleControl(VisibleUserControl.Einstellungen)

        menuEnableBindings.Add(New Tuple(Of ToolStripMenuItem, Func(Of Boolean))(
                                            mnuEinstellungen,
                                            Function() As Boolean
                                                Return AktVisibleUserControl <> VisibleUserControl.Einstellungen
                                            End Function))

        ' === Rechtsbündiger Teil ===
        Dim spring As New ToolStripSpringLabel() With {.AutoSize = False}

        ' === Hilfe ===
        Dim mnuHilfe As New ToolStripMenuItem("Hilfe")
        mnuHilfe.DropDownItems.Add(CreateMenuItem("Hilfe",
                                                  Sub() ChangeVisibleControl(VisibleUserControl.Hilfe),
                                                   Function() As Boolean
                                                       Return AktVisibleUserControl <> VisibleUserControl.Hilfe
                                                   End Function))

        mnuHilfe.DropDownItems.Add(CreateMenuItem("Über",
                                                  Sub() ChangeVisibleControl(VisibleUserControl.Über),
                                                   Function() As Boolean
                                                       Return AktVisibleUserControl <> VisibleUserControl.Über
                                                   End Function))

        If INI.Editor_UsingEditorAllowed Then
            ' --- Menüs hinzufügen ---
            ms.Items.AddRange({mnuDatei, mnuSpiel, mnuEditor, mnuEinstellungen, spring, mnuHilfe})
        Else
            ms.Items.AddRange({mnuDatei, mnuSpiel, mnuEinstellungen, spring, mnuHilfe})
        End If

    End Sub

    ' --- Hilfsfunktionen ---
    Private Function CreateMenuItem(text As String, action As Action, Optional enabledCondition As Func(Of Boolean) = Nothing) As ToolStripMenuItem
        Dim itm As New ToolStripMenuItem(text)
        AddHandler itm.Click, Sub(sender, e) action()
        If enabledCondition IsNot Nothing Then
            menuEnableBindings.Add(New Tuple(Of ToolStripMenuItem, Func(Of Boolean))(itm, enabledCondition))

        End If
        Return itm
    End Function

    Private Function CreateCheckMenuItem(text As String, isChecked As Boolean, onCheckedChanged As Action(Of Object)) As ToolStripMenuItem
        Dim itm As New ToolStripMenuItem(text) With {.CheckOnClick = True, .Checked = isChecked}
        AddHandler itm.CheckedChanged,
            Sub(sender, e)
                onCheckedChanged(sender)
            End Sub
        Return itm
    End Function

    Private Sub RefreshMenuStates()
        For Each bindingtpl As Tuple(Of ToolStripMenuItem, Func(Of Boolean)) In menuEnableBindings
            'Debug.WriteLine($"Type von Item2: {bindingtpl.Item2.GetType().FullName}")
            'Das hier geht nicht, die IDE meckert
            'Der Wert vom Typ "Func(Of Boolean)" kann nicht in "Boolean" konvertiert werden.
            'bindingtpl.Item1.Enabled = bindingtpl.Item2()
            'Nach langer Suche:
            Dim func As Func(Of Boolean) = bindingtpl.Item2
            Dim result As Boolean = func()
            bindingtpl.Item1.Enabled = result
        Next
        For Each bindingtpl As Tuple(Of ToolStripMenuItem, Func(Of Boolean)) In menuVisibleBindings
            'Problem wie oben.
            Dim func As Func(Of Boolean) = bindingtpl.Item2
            Dim result As Boolean = func()
            bindingtpl.Item1.Enabled = result
        Next
    End Sub

    ' --- Platzhalter-Subs ---
    Private Sub SpielstandLoad(forceDialog As Boolean)
        MessageBox.Show("SpielstandLoad(" & forceDialog & ")")
    End Sub

    Private Sub SpielstandSave(forceDialog As Boolean)
        MessageBox.Show("SpielstandSave(" & forceDialog & ")")
    End Sub

    Private Sub ShowInfo(infoType As Info)
        MessageBox.Show("Info: " & infoType.ToString())
    End Sub



    Private Sub SelectRandomSpielfeld()
        MessageBox.Show("Zufälliges Spielfeld wählen")
    End Sub



#End Region

#Region "ToolStrip unten Initialisierung"


    ' --- Aufruf z. B. im Load-Event:
    ' Private Sub frmMain_Load(...) Handles MyBase.Load
    '     InitBottomToolStrip()
    ' End Sub

    Private Sub BuildBottomToolStrip()
        ' Basis
        With ToolStripExMain
            .Dock = DockStyle.Bottom
            .GripStyle = ToolStripGripStyle.Hidden
            .AutoSize = False
            .Height = 28
            .CanOverflow = False
            .LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow
            .ImageScalingSize = New Size(16, 16)
            .ShowItemToolTips = INI.Sonstiges_ShowToolTips
            .SuspendLayout()
            .Items.Clear()
        End With

        ' =======================
        ' Debug-Gruppe (links) – nur wenn Debugger.IsAttached
        ' =======================
        If Debugger.IsAttached Then
            ToolStripExMain.Items.Add(MkBtnText("dbg_ini", "INI",
            Sub() INI.IniEditieren(),
            If(INI.Sonstiges_ShowToolTips, "INI während der Laufzeit bearbeiten", Nothing)))

            'ToolStripExMain.Items.Add(MkBtnText("dbg_explorer", "Explorer",
            'Sub() Process.Start("explorer.exe", AppDataDirectory),
            'If(INI.Sonstiges_ShowToolTips, "Öffnet AppData-Verzeichnis", Nothing)))

            ToolStripExMain.Items.Add(MkBtnText("dbg_go", "Go",
            Sub() Go(),
            If(INI.Sonstiges_ShowToolTips, "frmMain Sub Go ausführen", Nothing)))

            ToolStripExMain.Items.Add(MkBtnText("dbg_toggle", "Tgl",
            Sub() Toggle(),
            If(INI.Sonstiges_ShowToolTips, "frmMain Sub Toggle ausführen", Nothing)))

            ToolStripExMain.Items.Add(MkBtnText("dbg_test1", "T1",
            Sub() Test1(True),
            If(INI.Sonstiges_ShowToolTips, "frmMain Sub Test1 ausführen", Nothing)))

            ToolStripExMain.Items.Add(MkBtnText("dbg_test2", "T2",
            Sub() Test2(),
            If(INI.Sonstiges_ShowToolTips, "frmMain Sub Test2 ausführen", Nothing)))

            '  ToolStripExMain.Items.Add(New ToolStripSeparator())

            ' ---- Dateiexplorer aufrufen ----
            Dim cbo1 As New ToolStripComboBox("dbg_fileexpl") With {
                .DropDownStyle = ComboBoxStyle.DropDownList,
                .AutoSize = False,
                .Width = 50
            }
            'Anzeige, Farbe(ohne #), optionales Thema/Query
            Dim entries As (Label As String, Query As String)() = {
                ("AppDataDir", AppDataDirectory),
                ("Download", INI.IfRunningInIDE_DownloadDirectory),
                ("Gfx 16x16 Ressou", INI.IfRunningInIDE_Grafik16x16Directory_Ressourcen),
                ("Gfx 16x16 Andere", INI.IfRunningInIDE_Grafik16x16Directory_Other)
            }

            ' Items befüllen
            For Each e As (Label As String, Query As String) In entries
                cbo1.Items.Add(e.Label)
            Next
            If cbo1.Items.Count > 0 Then cbo1.SelectedIndex = 0
            If INI.Sonstiges_ShowToolTips Then
                cbo1.ToolTipText = "Windows Dateiexplorer mit vorgegebenem Pfad."
            End If

            AddHandler cbo1.SelectedIndexChanged,
                Sub()
                    Dim idx As Integer = cbo1.SelectedIndex
                    If idx >= 0 AndAlso idx < entries.Length Then
                        Dim e As (Label As String, Query As String) = entries(idx)
                        Process.Start("explorer.exe", e.Query)
                    End If
                End Sub

            ToolStripExMain.Items.Add(cbo1)



            ' ---- Icon-Kategorien → Material Icons mit Farbvorgabe öffnen ----
            Dim cbo2 As New ToolStripComboBox("dbg_iconcats") With {
                .DropDownStyle = ComboBoxStyle.DropDownList,
                .AutoSize = False,
                .Width = 50
            }
            ' Anzeige, Farbe (ohne #), optionales Thema/Query
            Dim entries2 As (Label As String, Hex As String, Query As String)() = {
                ("Neutral (Anthrazit)", "404040", ""),
                ("Checked (Schwarz)", "000000", ""),
                ("UnChecked (Grau)", "A0A0A0", ""),
                ("Error/Stop (Rot)", "C00000", "error"),
                ("Warnung (Orange)", "CC6600", "warning"),
                ("Aktiv (Grün)", "008000", ""),
                ("MouseOver (Blau)", "0066CC", ""),
                ("Tipps/Info (Blau)", "0066CC", ""),
                ("Optionen (Grau-Blau)", "3A6E7F", ""),
                ("Hellgrau", "B0B0B0", ""),
                ("Weiß", "FFFFFF", ""),
                ("Schwarz", "000000", "")
            }

            ' Items befüllen
            For Each e As (Label As String, Hex As String, Query As String) In entries2
                cbo2.Items.Add(e.Label)
            Next
            If cbo2.Items.Count > 0 Then cbo2.SelectedIndex = 0
            If INI.Sonstiges_ShowToolTips Then
                cbo2.ToolTipText = "Google Material Icons mit vorgewählter Farbe und optionalem Thema öffnen"
            End If

            AddHandler cbo2.SelectedIndexChanged,
                Sub()
                    Dim idx As Integer = cbo2.SelectedIndex
                    If idx >= 0 AndAlso idx < entries2.Length Then
                        Dim e As (Label As String, Hex As String, Query As String) = entries2(idx)
                        Dim url As String = BuildMaterialIconsUrl(e.Hex, e.Query)
                        OpenUrlInBrowser(url)
                    End If
                End Sub

            ToolStripExMain.Items.Add(cbo2)
            ToolStripExMain.Items.Add(New ToolStripSeparator())

            AttachToolStripComboOverflowTooltip(cbo1)
            AttachToolStripComboOverflowTooltip(cbo2)

        End If

        ' =======================
        ' Editor-Gruppe (links), nur wenn Allowed
        ' =======================
        If INI.Editor_UsingEditorAllowed Then
            ToolStripExMain.Items.Add(MkBtnImg("grpEditor_player", GetAppGrafik(AppGrafikName.Spieler), Sub() DoSpielfeld(),
            If(INI.Sonstiges_ShowToolTips, "Ruft das Spiel auf.", Nothing)))
            ToolStripExMain.Items.Add(MkBtnImg("grpEditor_editor", GetAppGrafik(AppGrafikName.Editor), Sub() DoEditor(),
            If(INI.Sonstiges_ShowToolTips, "Ruft den Editor auf.", Nothing)))
            ToolStripExMain.Items.Add(MkBtnImg("grpEditor_werkbank", GetAppGrafik(AppGrafikName.Werkbank), Sub() DoWerkbank(),
            If(INI.Sonstiges_ShowToolTips, "Ruft die Werkbank auf.", Nothing)))
            ToolStripExMain.Items.Add(MkBtnImg("grpEditor_toolbox", GetAppGrafik(AppGrafikName.Werkzeug), Sub() DoToolBox(),
            If(INI.Sonstiges_ShowToolTips, "Ruft die Werkzeugkiste auf.", Nothing)))
            ToolStripExMain.Items.Add(New ToolStripSeparator())

            ' Enabled-Zustand abhängig von UsingEditor
            SetGroupEnabled("grpEditor", INI.Editor_UsingEditor)
        End If


        ' =======================
        ' Status-Gruppe (links)
        ' =======================
        ToolStripExMain.Items.Add(MkLbl("stat_size", "Feldgröße: 00/00/00"))
        ToolStripExMain.Items.Add(New ToolStripSeparator())
        'ToolStripExMain.Items.Add(MkLbl("stat_title", "Steine:"))
        'ToolStripExMain.Items.Add(New ToolStripSeparator())
        ToolStripExMain.Items.Add(MkLbl("stat_total", "Summe Steine: 000"))
        ToolStripExMain.Items.Add(New ToolStripSeparator())
        ToolStripExMain.Items.Add(MkLbl("stat_current", "Aktuell: 000"))
        ToolStripExMain.Items.Add(New ToolStripSeparator())
        ToolStripExMain.Items.Add(MkLbl("stat_sel", "wählbar: 00"))
        ToolStripExMain.Items.Add(New ToolStripSeparator())
        ToolStripExMain.Items.Add(MkLbl("stat_pairs", "Paare: 00"))
        ToolStripExMain.Items.Add(New ToolStripSeparator())



        ' =======================
        ' Rechte Gruppe (Alignment = Right)
        ' Einfüge-Reihenfolge = von rechts nach links
        ' =======================

        ToolStripExMain.Items.Add(MkBtnImgRight("act_screenshot", GetAppGrafik(AppGrafikName.Screenshot), Sub() DoTakeScreenShot(),
        If(INI.Sonstiges_ShowToolTips, "Erzeugt einen ScreenShot vom Spiel/Editor/Werkbank.", Nothing)))

        ToolStripExMain.Items.Add(MkBtnImgRight("act_statistik", GetAppGrafik(AppGrafikName.Statistik), Sub() DoStatistik(),
            If(INI.Sonstiges_ShowToolTips, "Statistische Daten zum Spiel", Nothing)))

        ToolStripExMain.Items.Add(MkSepRight())

        ' '' 4) Label "Zeige:"
        ''ToolStripExMain.Items.Add(MkLblRight("act_show_lbl", "Zeige:"))

        ToolStripExMain.Items.Add(MkBtnImgTextRight("act_tip1", GetAppGrafik(AppGrafikName.Tip), "",
            Sub() DoTipEinzel(),
            If(INI.Sonstiges_ShowToolTips, "Schalter: Zeigt permanent wählbare Steine an.", Nothing)))


        Dim chkWinds As ToolStripButton = MkToggleImgRight("opt_winds_onegrp",
            GetAppGrafik(AppGrafikName.WindsChecked), GetAppGrafik(AppGrafikName.WindsUnChecked),
            INI.Spielbetrieb_WindsAreInOneClickGroup,
            Sub(checked)
                If _isRefreshing Then Return
                INI.Spielbetrieb_WindsAreInOneClickGroup = checked
                ' ggf. Regeln neu anwenden/Refresh
            End Sub,
            If(INI.Sonstiges_ShowToolTips, "Schalter vereinfachte Spielregel: Alle Winde können untereinander Paare bilden.", Nothing))

        ToolStripExMain.Items.Add(chkWinds)

        ToolStripExMain.Items.Add(MkSepRight())

        ToolStripExMain.Items.Add(MkBtnImgTextRight("act_wählbar", GetAppGrafik(AppGrafikName.Tipps), "",
            Sub() DoTipAlle(),
            If(INI.Sonstiges_ShowToolTips, "Tip: Zeigt alle wählbaren Paare an.", Nothing)))

        Dim chkSel As ToolStripButton = MkToggleImgRight("opt_show_sel",
                GetAppGrafik(AppGrafikName.ShowSelectableChecked), GetAppGrafik(AppGrafikName.ShowSelectableUnChecked),
                INI.Spielbetrieb_ShowSelectableStones,
                Sub(checked)
                    If _isRefreshing Then Return
                    INI.Spielbetrieb_ShowSelectableStones = checked
                    ' ggf. Refresh/Neuzeichnen hier
                End Sub,
                If(INI.Sonstiges_ShowToolTips, "Tip: Zeigt alle wählbaren Steine an.", Nothing))
        ToolStripExMain.Items.Add(chkSel)


        ToolStripExMain.Items.Add(MkSepRight())

        ToolStripExMain.Items.Add(MkBtnImgRight("act_restart", GetAppGrafik(AppGrafikName.Restart), Sub() DoReDo(),
            If(INI.Sonstiges_ShowToolTips, "Stell das Spiel auf die Ausgangsstellung zurück", Nothing)))

        ToolStripExMain.Items.Add(MkBtnImgRight("act_redo", GetAppGrafik(AppGrafikName.Redo), Sub() DoReDo(),
            If(INI.Sonstiges_ShowToolTips, "Arbeitet wieder vorwärts, solange das noch möglich ist", Nothing)))

        ToolStripExMain.Items.Add(MkBtnImgRight("act_undo", GetAppGrafik(AppGrafikName.Undo), Sub() DoUndo(),
            If(INI.Sonstiges_ShowToolTips, "Setzt das letzte Steinpaar wieder auf das Spielfeld", Nothing)))

        ToolStripExMain.Items.Add(MkSepRight())

        ToolStripExMain.ResumeLayout()
        ToolStripExMain.PerformLayout()
    End Sub

    ' --- Helper: Labels ---
    Private Function MkLbl(name As String, text As String) As ToolStripLabel
        Return New ToolStripLabel(text) With {.Name = name}
    End Function

    Private Function MkLblRight(name As String, text As String) As ToolStripLabel
        Return New ToolStripLabel(text) With {.Name = name, .Alignment = ToolStripItemAlignment.Right}
    End Function

    ' --- Helper: Buttons (Text / Image / Image+Text) ---
    Private Function MkBtnText(name As String, caption As String, onClick As Action, tip As String) As ToolStripButton
        Dim b As New ToolStripButton(caption) With {.Name = name, .DisplayStyle = ToolStripItemDisplayStyle.Text}
        If tip IsNot Nothing Then b.ToolTipText = tip
        AddHandler b.Click, Sub() onClick()
        Return b
    End Function

    Private Function MkBtnImg(name As String, img As Image, onClick As Action, tip As String) As ToolStripButton
        Dim b As New ToolStripButton() With {.Name = name, .Image = img, .DisplayStyle = ToolStripItemDisplayStyle.Image}
        If tip IsNot Nothing Then b.ToolTipText = tip
        AddHandler b.Click, Sub() onClick()
        Return b
    End Function

    Private Function MkBtnImgRight(name As String, img As Image, onClick As Action, tip As String) As ToolStripButton
        Dim b As ToolStripButton = MkBtnImg(name, img, onClick, tip)
        b.Alignment = ToolStripItemAlignment.Right
        Return b
    End Function

    Private Function MkBtnImgTextRight(name As String, img As Image, caption As String, onClick As Action, tip As String) As ToolStripButton
        Dim b As New ToolStripButton(caption) With {
        .Name = name,
        .Image = img,
        .DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
        .ImageAlign = ContentAlignment.MiddleLeft
    }
        b.Alignment = ToolStripItemAlignment.Right
        If tip IsNot Nothing Then b.ToolTipText = tip
        AddHandler b.Click, Sub() onClick()
        Return b
    End Function

    ' --- Helper: Toggle-Buttons mit Bildwechsel (als „Checkbox mit Grafik“) ---
    Private Function MkToggleImgRight(name As String,
                                  imgUnchecked As Image,
                                  imgChecked As Image,
                                  initialChecked As Boolean,
                                  onChanged As Action(Of Boolean),
                                  tip As String) As ToolStripButton
        Dim b As New ToolStripButton() With {
        .Name = name,
        .CheckOnClick = True,
        .Checked = initialChecked,
        .Image = If(initialChecked, imgChecked, imgUnchecked),
        .Alignment = ToolStripItemAlignment.Right,
        .DisplayStyle = ToolStripItemDisplayStyle.Image
    }
        If tip IsNot Nothing Then b.ToolTipText = tip
        AddHandler b.CheckedChanged,
        Sub()
            If _isRefreshing Then Return 'richtig? (bin mir unsicher)
            b.Image = If(b.Checked, imgChecked, imgUnchecked)
            onChanged(b.Checked)
        End Sub
        Return b
    End Function

    ' --- Helper: Separator rechts ---
    Private Function MkSepRight() As ToolStripSeparator
        Return New ToolStripSeparator() With {.Alignment = ToolStripItemAlignment.Right}
    End Function

    ' --- Gruppe aktivieren/deaktivieren (per Namenspräfix) ---
    Private Sub SetGroupEnabled(prefix As String, enabled As Boolean)
        For Each it As ToolStripItem In ToolStripExMain.Items
            If it.Name IsNot Nothing AndAlso it.Name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) Then
                it.Enabled = enabled
            End If
        Next
    End Sub

    ' --- API: von außen Editor-Gruppe schalten (und später erweitern) ---
    Public Sub UsingEditor(enabled As Boolean)
        SetGroupEnabled("grpEditor", enabled)
        ' hier ggf. eigene Ergänzungen (Ansichten umschalten etc.)
    End Sub

    ' --- Platzhaltergrafik 16x16 ---
    Private Function DummyGrafik() As Image
        Dim bmp As New Bitmap(16, 16, PixelFormat.Format32bppPArgb)
        Using g As Graphics = Graphics.FromImage(bmp)
            g.Clear(Color.Transparent)
            ' kleiner Rahmen + Diagonale, damit man "etwas" sieht
            Using p As New Pen(Color.Gray)
                g.DrawRectangle(p, 0, 0, 15, 15)
                g.DrawLine(p, 0, 15, 15, 0)
            End Using
        End Using
        Return bmp
    End Function


    ''' <summary>
    ''' Zieht nach INI-Änderungen die UI-Zustände im ToolStrip nach:
    ''' - ToolTips zeigen/unterdrücken
    ''' - Toggle-Buttons (grafische "Checkboxen") setzen
    ''' - Editor-Gruppe aktivieren/deaktivieren
    ''' </summary>
    Public Sub RefreshUINachIniÄnderung()
        If ToolStripExMain Is Nothing Then Exit Sub

        _isRefreshing = True
        Try
            ' 1) ToolTips (global für ToolStrip-Items)
            ToolStripExMain.ShowItemToolTips = INI.Sonstiges_ShowToolTips

            ' 2) Toggle: "alle wählbaren Steine anzeigen"
            Dim btnShowSel As ToolStripButton = TryCast(ToolStripExMain.Items("opt_show_sel"), ToolStripButton)
            If btnShowSel IsNot Nothing Then
                btnShowSel.Checked = INI.Spielbetrieb_ShowSelectableStones
                ' Bild entsprechend dem Zustand (hier Dummy – bei dir echte Grafiken einsetzen)
                btnShowSel.Image = If(btnShowSel.Checked, GetAppGrafik(AppGrafikName.ShowSelectableChecked), GetAppGrafik(AppGrafikName.ShowSelectableUnChecked))
                If INI.Sonstiges_ShowToolTips Then
                    btnShowSel.ToolTipText = "Zeigt alle wählbaren Steine an."
                Else
                    btnShowSel.ToolTipText = Nothing
                End If
            End If

            ' 3) Toggle: "Winde bilden gemeinsame Paargruppe"
            Dim btnWinds As ToolStripButton = TryCast(ToolStripExMain.Items("opt_winds_onegrp"), ToolStripButton)
            If Not IsNothing(btnWinds) Then
                btnWinds.Checked = INI.Spielbetrieb_WindsAreInOneClickGroup
                btnWinds.Image = If(btnWinds.Checked, GetAppGrafik(AppGrafikName.WindsChecked), GetAppGrafik(AppGrafikName.WindsUnChecked))
                If INI.Sonstiges_ShowToolTips Then
                    btnWinds.ToolTipText = "Vereinfachte Spielregel: Alle Winde können Paare bilden."
                Else
                    btnWinds.ToolTipText = Nothing
                End If
            End If

            ' 4) Editor-Gruppe (Allowed = Neustartpflicht, daher hier nur Enabled)
            UsingEditor(INI.Editor_UsingEditor)

            ' 5) Optional: ToolTips der restlichen Items je nach Setting unterdrücken/setzen
            If Not INI.Sonstiges_ShowToolTips Then
                ' Wer mag, kann hier weitere ToolTips leeren:
                ' SafeSetTip("act_undo", Nothing)
                ' SafeSetTip("act_redo", Nothing)
                ' ...
            End If

            ToolStripExMain.PerformLayout()

        Finally
            _isRefreshing = False
        End Try
    End Sub

    ' Hilfsfunktion: ToolTipText sicher setzen (falls Item existiert)
    Private Sub SafeSetTip(itemName As String, tip As String)
        Dim it As ToolStripItem = ToolStripExMain.Items(itemName)
        If it IsNot Nothing Then it.ToolTipText = tip
    End Sub

    Private Sub OpenUrlInBrowser(url As String)
        Try
            Dim psi As New ProcessStartInfo(url) With {.UseShellExecute = True}
            Process.Start(psi)
        Catch
            ' Fallback
            Process.Start("explorer.exe", url)
        End Try
    End Sub

    Private Function BuildMaterialIconsUrl(hexNoHash As String, query As String) As String
        Dim clean As String = hexNoHash.Trim().TrimStart("#"c)
        Dim baseUrl As String = $"https://fonts.google.com/icons?icon.size=16&icon.color=%23{clean}"
        If Not String.IsNullOrWhiteSpace(query) Then
            Dim q As String = Uri.EscapeDataString(query.Trim())
            ' Material Icons nutzt die Such-Query im Pfad-Param "q"
            baseUrl &= $"&q={q}"
        End If
        ' Voreinstellung irgendeines Icons bleibt optional; Suche überschreibt die Ansicht
        Return baseUrl
    End Function

    Private Function BuildMaterialIconsUrl(hexNoHash As String) As String
        ' Verwendet dein Muster; nur die Farbe wird ersetzt.
        ' icon.size=16 bleibt, Farbe als %23RRGGBB kodiert.
        Dim clean As String = hexNoHash.Trim().TrimStart("#"c)
        Return $"https://fonts.google.com/icons?icon.size=16&icon.color=%23{clean}&selected=Material+Symbols+Outlined:search:FILL@0;wght@400;GRAD@0;opsz@20"
    End Function

    Private Sub AttachComboOverflowTooltip(cb As ComboBox)
        ' 1) DropDown-Liste auf Maximalbreite einstellen (bei Öffnen)
        AddHandler cb.DropDown, Sub(_s, _e) RecalcDropDownWidth(cb)

        ' 2) Tooltip zeigen, wenn der ausgewählte Text im Edit-Bereich abgeschnitten ist
        AddHandler cb.MouseMove,
        Sub(sender As Object, e As MouseEventArgs)
            Dim c As ComboBox = DirectCast(sender, ComboBox)
            Dim txt As String = If(c.SelectedItem IsNot Nothing, c.GetItemText(c.SelectedItem), c.Text)
            Dim w As Integer = TextRenderer.MeasureText(txt, c.Font).Width
            Dim visible As Integer = c.ClientSize.Width - 6            ' kleiner Innenabzug
            _cbToolTip.SetToolTip(c, If(w > visible, txt, Nothing))
        End Sub
    End Sub

    Private Sub RecalcDropDownWidth(cb As ComboBox)
        Dim maxW As Integer = cb.DropDownWidth
        Using g As Graphics = cb.CreateGraphics()
            For Each it As Object In cb.Items
                Dim s As String = cb.GetItemText(it)
                Dim w As Integer = TextRenderer.MeasureText(g, s, cb.Font).Width
                If w > maxW Then maxW = w
            Next
        End Using
        ' Platz für Scrollbar + Puffer
        maxW += SystemInformation.VerticalScrollBarWidth + 12
        cb.DropDownWidth = Math.Min(maxW, 3000)
    End Sub

    Public Sub AttachToolStripComboOverflowTooltip(tscb As ToolStripComboBox)
        Dim cb As ComboBox = tscb.ComboBox

        ' DropDown-Liste bei Öffnen auf maximal nötige Breite bringen
        AddHandler cb.DropDown, Sub(_s, _e) RecalcDropDownWidth(cb)

        ' Tooltip zeigen, wenn der ausgewählte Text im Edit-Bereich abgeschnitten ist
        AddHandler cb.MouseMove,
        Sub(sender As Object, e As MouseEventArgs)
            Dim txt As String = If(cb.SelectedItem IsNot Nothing, cb.GetItemText(cb.SelectedItem), cb.Text)
            Dim w As Integer = TextRenderer.MeasureText(txt, cb.Font).Width
            Dim visible As Integer = cb.ClientSize.Width - 6
            _cbToolTip.SetToolTip(cb, If(w > visible, txt, Nothing))
        End Sub
    End Sub

    'Private Sub RecalcDropDownWidth(cb As ComboBox)
    '    Dim maxW As Integer = cb.DropDownWidth
    '    Using g As Graphics = cb.CreateGraphics()
    '        For Each it As Object In cb.Items
    '            Dim s As String = cb.GetItemText(it)
    '            Dim w As Integer = TextRenderer.MeasureText(g, s, cb.Font).Width
    '            If w > maxW Then maxW = w
    '        Next
    '    End Using
    '    maxW += SystemInformation.VerticalScrollBarWidth + 12
    '    cb.DropDownWidth = Math.Min(maxW, 3000)
    'End Sub
#End Region

#Region "ToolStrip unten Event-Verarbeitung"

    Private Sub DoSpielfeld()
        InitialisierungSpielfeldEditorAndWerkbank(startRenderingWithOrToggleTo:=RenderingEnum.Spielfeld)
    End Sub

    Private Sub DoEditor()
        InitialisierungSpielfeldEditorAndWerkbank(startRenderingWithOrToggleTo:=RenderingEnum.Editor)
    End Sub

    Private Sub DoWerkbank()
        InitialisierungSpielfeldEditorAndWerkbank(startRenderingWithOrToggleTo:=RenderingEnum.Werkbank)
    End Sub

    Private Sub DoToolBox()
        'Das lößt das Event RuntimeOnly_ToolboxAktiv_Event aus, das  UpdateGrpEditorButtons
        'aufruft, das die Toolbox öffnet oder schließt.
        'Klingt wie durch die Brust ins Auge, har aber den Vorteil von überall aus nur durch
        'Setzen oder Löschen des Flags RuntimeOnly_ToolboxAktiv die Toolbox zu öffnen oder zu schließen.
        INI.RuntimeOnly_ToolboxAktiv = Not INI.RuntimeOnly_ToolboxAktiv
    End Sub

    Private Sub DoTakeScreenShot()
        Spielfeld.PaintSpielfeld_CreateScreenShot()
        MsgBox("Screnshot erzeugt.", MsgBoxStyle.Information)
    End Sub

    Private Sub DoTipEinzel()

    End Sub

    Private Sub DoTipAlle()

    End Sub

    Private Sub DoReDo()

    End Sub

    Private Sub DoUndo()

    End Sub

    Private Sub DoStatistik()

    End Sub

    Private Sub RefreshToolStripExMainAktMaxSteineXYZ()

        Dim lblSize As ToolStripLabel = TryCast(ToolStripExMain.Items("stat_size"), ToolStripLabel)

        lblSize.Text = $"Feldgröße: {INI.Rendering_AktMaxSteineX}/{INI.Rendering_AktMaxSteineY}/{INI.Rendering_AktMaxSteineZ}"

    End Sub

    ''' <summary>
    ''' Aktualisiert die Statuswerte (Gesamt, Aktuell, Wählbar, Paare).
    ''' </summary>
    Public Sub UpdateStatus(gesamt As Integer, aktuell As Integer, waehlbar As Integer, paare As Integer)
        Dim lblGesamt As ToolStripLabel = TryCast(ToolStripExMain.Items("stat_total"), ToolStripLabel)
        Dim lblAktuell As ToolStripLabel = TryCast(ToolStripExMain.Items("stat_current"), ToolStripLabel)
        Dim lblWaehlbar As ToolStripLabel = TryCast(ToolStripExMain.Items("stat_sel"), ToolStripLabel)
        Dim lblPaare As ToolStripLabel = TryCast(ToolStripExMain.Items("stat_pairs"), ToolStripLabel)

        If lblGesamt IsNot Nothing Then lblGesamt.Text = $"{gesamt} Gesamt"
        If lblAktuell IsNot Nothing Then lblAktuell.Text = $"{aktuell} Aktuell"
        If lblWaehlbar IsNot Nothing Then lblWaehlbar.Text = $"{waehlbar} wählbar"
        If lblPaare IsNot Nothing Then lblPaare.Text = $"{paare} Paare"
    End Sub




    Private Sub UpdateGrpEditorButtons()

        If Not INI.Editor_UsingEditorAllowed Then
            Exit Sub
        End If

        Dim btnPlayer As ToolStripButton = TryCast(ToolStripExMain.Items("grpEditor_player"), ToolStripButton)
        Dim btnEditor As ToolStripButton = TryCast(ToolStripExMain.Items("grpEditor_editor"), ToolStripButton)
        Dim btnWerkbank As ToolStripButton = TryCast(ToolStripExMain.Items("grpEditor_werkbank"), ToolStripButton)

        Select Case INI.RuntimeOnly_AktRendering
            Case RenderingEnum.None
                btnPlayer.Image = GetAppGrafik(AppGrafikName.Spieler)
                btnEditor.Image = GetAppGrafik(AppGrafikName.Editor)
                btnWerkbank.Image = GetAppGrafik(AppGrafikName.Werkbank)

            Case RenderingEnum.Editor
                btnPlayer.Image = GetAppGrafik(AppGrafikName.Spieler)
                btnEditor.Image = GetAppGrafik(AppGrafikName.EditorAktiv)
                btnWerkbank.Image = GetAppGrafik(AppGrafikName.Werkbank)

            Case RenderingEnum.Spielfeld
                btnPlayer.Image = GetAppGrafik(AppGrafikName.SpielerAktiv)
                btnEditor.Image = GetAppGrafik(AppGrafikName.Editor)
                btnWerkbank.Image = GetAppGrafik(AppGrafikName.Werkbank)

            Case RenderingEnum.Werkbank
                btnPlayer.Image = GetAppGrafik(AppGrafikName.Spieler)
                btnEditor.Image = GetAppGrafik(AppGrafikName.Editor)
                btnWerkbank.Image = GetAppGrafik(AppGrafikName.WerkbankAktiv)
        End Select

    End Sub

    'Das ist diese Methode, sie ist angesiedelt in FrmMain.
    'Sie wird per Event aus der INI heraus aufgerufen.
    '(Dort gibt es eine Sektion [OnlyRuntime] mit einem Key ToolboxAktiv As Boolean,
    'der von überall heraus geschaltet werden kann.)
    Private Sub ShowOrHideToolboxAndUpdateToolboxButton()
        ' 1) Immer auf dem UI-Thread ausführen (INI-Event könnte im Hintergrund kommen)
        If Me.InvokeRequired Then
            Me.BeginInvoke(New MethodInvoker(AddressOf ShowOrHideToolboxAndUpdateToolboxButton))
            Return
        End If

        If Not INI.Editor_UsingEditorAllowed Then Exit Sub

        ' 2) Button sicher ermitteln
        Dim btnToolBox As ToolStripButton = Nothing
        Dim item As ToolStripItem = Nothing
        If ToolStripExMain.Items.ContainsKey("grpEditor_toolbox") Then
            item = ToolStripExMain.Items("grpEditor_toolbox")
            btnToolBox = TryCast(item, ToolStripButton)
        End If

        If INI.RuntimeOnly_ToolboxAktiv Then
            If btnToolBox IsNot Nothing Then
                btnToolBox.Image = GetAppGrafik(AppGrafikName.WerkzeugAktiv)
                btnToolBox.Checked = True
            End If

            ' 3) Instanz sicherstellen
            If _frmToolBox Is Nothing OrElse _frmToolBox.IsDisposed Then
                _frmToolBox = New frmToolBox() With {
                .ShowInTaskbar = False,
                .TopMost = False   ' Relativ zu Owner statt absolut
            }
                ' Empfehlung in frmToolBox:
                ' Private Sub frmToolBox_FormClosing(...) Handles Me.FormClosing
                '   If e.CloseReason = UserClosing Then e.Cancel = True : Me.Hide()
                ' End Sub
            End If

            ' 4) Als Owned-Form von frmMain anzeigen → bleibt über frmMain, aber nicht über anderen Apps
            _frmToolBox.Show(Me)       ' Owner setzen
            _frmToolBox.BringToFront() ' sicher nach vorne holen
            ' Optional:
            ' _frmToolBox.Activate()

        Else
            If btnToolBox IsNot Nothing Then
                btnToolBox.Image = GetAppGrafik(AppGrafikName.Werkzeug)
                btnToolBox.Checked = False
            End If

            If _frmToolBox IsNot Nothing AndAlso Not _frmToolBox.IsDisposed Then
                _frmToolBox.Hide()
            End If
        End If
    End Sub



#End Region


End Class