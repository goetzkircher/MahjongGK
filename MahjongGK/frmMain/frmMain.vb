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
'

Imports System.Drawing.Imaging
Imports System.IO
Imports System.Text.RegularExpressions
Imports MahjongGK.Contracts
Imports MahjongGK.Spielfeld
Imports TileFactory

#Disable Warning IDE0079
#Disable Warning IDE1006

Public Enum VisibleUserControl
    None = -1
    Spielfeld
    Spielauswahl
    Einstellungen
    About
    Hilfe
End Enum

Public Class frmMain

#Region "Einstiegspunkt und Tests während der Programmentwicklung"

    '######################################################################################################

    Private Sub Test1(value As Boolean)

        'SFMain.SFDat.SFInf.UpdateTopSteinInfos()
        Me.Hide()
        Dim c As New ColorPickerHSB
        c.ShowDialog()

        'Using frm As New FrmBackgroundSelector("C:\Users\goetz\Downloads\Vivaldi")
        '    frm.ShowDialog()
        'End Using
    End Sub

    Private Sub Test2()

        Dim lwp As String = "C:\Users\goetz\MahjongGK\Temporär\SpielfeldInfo"
        Dim dirInfo As New DirectoryInfo(lwp)

        AktVisibleUserControl = VisibleUserControl.Spielauswahl
        Dim arrFI() As FileInfo = dirInfo.GetFiles
        UctlSpielauswahlMain.Initialisierung(arrFI)
        'Using frm As New frmWebPTest
        '    frm.ShowDialog()
        'End Using
        UCtlSpielfeldMain.Invalidate()

    End Sub

    Private Sub Go()

        AktVisibleUserControl = VisibleUserControl.Spielfeld

        'Dim newSpielfeldInfo As New SFInfo(New Triple(MJ_STEINE_MAXX, MJ_STEINE_MAXY, MJ_STEINE_MAXZ))
        'Dim wbsSF As Werkstück = Umfeld.Werkstück_Pyramide(New Triple(MJ_STEINE_MAXX \ 2, MJ_STEINE_MAXY * 3 \ 2, MJ_STEINE_MAXZ), True, True, demoMode:=True) ', True, True)
        Dim newSpielfeldInfo As New SFInfo(New Triple(7, 4, 6))
        ' Dim wbsSF As Werkstück = Umfeld.Werkstück_Pyramide(New Triple(5, 5, 3), True, True, demoMode:=True) ', True, True)
        'Dim wbsSF As Werkstück = Umfeld.Werkstück_Rechteck(New Triple(2, 2, 1), demoMode:=True) ', True, True)

        'newSpielfeldInfo.AddWerkstückToSpielfeld(wbsSF, New Triple(1, 1, 0))
        Dim generator As New SpielsteinGenerator(GeneratorModus.StoneSet_072)
        '  generator.DebugStoneCountLimit = 16
        SFMain.CreateSpielfeld(newSpielfeldInfo, generator)

        'startet die Anzeige
        SFMain.RenderMode = RenderMode.Edit

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

    Private Sub GfxCompiler()
        Using frm As New frmGfxCompiler
            frm.ShowDialog()
        End Using
    End Sub

#End Region

    'Im Hauptformular sind In im Me.Load- Ereignis einige wenige Initialisierungen angesiedelt. 
    'Außerdem die Erstellung und Verwaltung des Hauptmenues, das dann die Befehle an die einzenen UserControls weitergibt,
    'sowie das Ein- und Aushängen (= sichtbar/unsichtbar machen) der verschiedenen UserControls In das Hauptformular,
    'alles also reine Verwaltungsarbeit.
    'Die Programmlogik ist komplett In den verschiedenen UserControls angesiedelt, von wo aus auf einen gemeinsamen Pool zugegriffen
    'wird, der sich überwiegend im Verzeichnis "Namespace Spielfeld" befindet.

#Region "Initialisierungen"

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
    Private _frmToolBox As frmToolBox = Nothing
    Private _themer As Theme.ThemeManager

    Private _startupMainBounds As Rectangle = Rectangle.Empty
    Private _splash As frmSplash
    Sub New()

        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()

        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.

    End Sub

    Private Sub frmMain_Load(sender As Object, e As EventArgs) Handles Me.Load

        CheckIniFileWhenStartedInIde()

        'Starup 1) Startup-Bounds bestimmen (INI oder zentriert auf einem ausgewählten Screen)
        Dim iniRect As Rectangle = INI.Sonstiges_FrmMainStartupPosition

        If IsValidStartupRect(iniRect) Then
            _startupMainBounds = iniRect
        Else
            Dim scr As Screen = PickTargetScreen()
            _startupMainBounds = ComputeCenteredBoundsOnScreen(scr, Me.Size)
        End If

        'Startup 2) Splash über den geplanten Main-Bounds positionieren und anzeigen
        _splash = New frmSplash()
        _splash.PositionForMainTarget(_startupMainBounds)
        _splash.Show()
        _splash.Refresh()

        'Die Hintergrundgrafiken Auspacken, überprüfen, Thumbs anlegen .
        Dim bzm As New BackgroundZipManager
        bzm.InitializeOnStartup()

        ' ───────────────────────────────────────────────────────────────
        ' PRELOAD der nativen Magick-DLL:
        ' Magick.NET benötigt Magick.Native-Q8-x64.dll im Ordner
        '   runtimes\win-x64\native\
        ' Obwohl die DLL dort korrekt liegt, kann es je nach Windows-
        ' Loader zu 0x8007007E („Modul nicht gefunden“) kommen, wenn
        ' Magick.NET sie zu spät oder mit falschem Suchpfad lädt.
        ' 
        ' Lösung: Wir laden die DLL explizit per LoadLibrary, bevor
        ' Magick.NET das erste Mal verwendet wird. Zusätzlich hängen
        ' wir den Ordner in PATH, damit auch alle Abhängigkeiten
        ' gefunden werden.
        ' ───────────────────────────────────────────────────────────────
        Try
            StartupMagicNative.PreloadMagickNativeWithExpeption()
        Catch ex As Exception

        End Try

        'Aktualisierung der Titelzeile
        If My.Application.IsNetworkDeployed = True Then
            Me.Text &= " " & My.Application.Deployment.CurrentVersion.ToString
        Else
            Me.Text &= " " & Helfer.ReadClickOnceVersionFromManifest()
        End If
        'stellt ggf "[IDE]" der Titelzeile voran.
        Helfer.IsRunningInIDE(Me)

        'Prüfung, ob die Form auf den Bildschirm passt und ggf Anpassung. 
        Dim wa As Rectangle = Screen.FromControl(Me).WorkingArea
        Dim maxWidth As Integer = CInt(wa.Width * 0.8)
        Dim maxHeight As Integer = CInt(wa.Height * 0.8)

        If Me.Width > maxWidth Then Me.Width = maxWidth
        If Me.Height > maxHeight Then Me.Height = maxHeight

        Me.MinimumSize = Me.SizeFromClientSize(New Size(MJ_SPIELFELD_MIN_WIDTH, MJ_SPIELFELD_MIN_HEIGHT))
        Me.AutoScaleMode = AutoScaleMode.Dpi

        'Aktzelles Farbschema anwenden.
        'Muß geschehen, bevor die Controls ausgehängt werden, da sie dann nicht mehr erreichbar sind. 
        _themer = New Theme.ThemeManager(Me, If(INI.Global_DarkMode, AppTheme.Dark, AppTheme.Light),
                                   useOwnerDrawTabs:=False,        ' auf True, wenn Tabs farbig gezeichnet werden sollen
                                   customizeProgressBar:=True, brightenAmount:=0)

        '
        'Die UserControls müssen in der Reihenfolge der Enumeration VisibleUserControl
        'in die Liste eingefügt werden, damit sie später in der richtigen Reihenfolge
        'aufgerufen werden können.
        VisibleUserControls.Add(UCtlSpielfeldMain)
        UCtlSpielfeldMain.Parent = Nothing

        VisibleUserControls.Add(UctlSpielauswahlMain)
        UctlSpielauswahlMain.Parent = Nothing

        VisibleUserControls.Add(UCtlEinstellungenMain)
        UCtlEinstellungenMain.Parent = Nothing

        VisibleUserControls.Add(UCtlAboutMahjongGKMain)
        UCtlAboutMahjongGKMain.Parent = Nothing

        '
        'Hier weitere UserControls hinzufügen, die im TabControlMain
        'im Designer sichtbar sind.

        'Das TabControlMain entsorgen
        TabControlMain.Parent = Nothing
        TabControlMain.Dispose()
        '

        'und das erste UserControl anzeigen

        AktVisibleUserControl = VisibleUserControl.Spielfeld

        'Das Menue wird dynamisch erzeugt, damit es
        'übersichtlicher wird, als die statische Erzeugung im Designer.
        ' BuildMenu_ZLV(Me.MenuStripExMain)
        InitializeMainMenu()

        BuildBottomToolStrip()

        ' Startzustand setzen

        'Spielfeld.TestDaten_StatischesSpielfeld_EineEbenen

        If Debugger.IsAttached Then
            Me.KeyPreview = True
            MjDebug.Attach(Me)
        End If
        '
        'Die INI ist bereits initialisiert, das passiert beim allererstem Zugriff auf einen Wert automatisch.
        'hier geht es um eine Reinitialisierung mit Werten der IniEvents.
        INI.Initialisierung(update:=True, raiseIniEventsDefault:=IniEvents.OnChangeValue)

        '' Obsolet durch 1) Startup-Bounds bestimmen  Me.EnsureLocationVisibleOnAnyScreen()

        ' Startup 3) frmMain final positionieren & Splash schließen
        If IsValidStartupRect(iniRect) Then
            Me.StartPosition = FormStartPosition.Manual
            Me.Bounds = _startupMainBounds
            If INI.Sonstiges_FrmMainStartMaximized Then
                Me.WindowState = FormWindowState.Maximized
            End If
        Else
            Me.StartPosition = FormStartPosition.CenterScreen
            ' Windows legt Location dann selbst fest; Bounds schonen
        End If

        _splash.SetStatusText("")
        ' --- Splash bleibt noch 1 Sekunde stehen ---
        Dim splashkiller As New Timer() With {.Interval = 1000}
        AddHandler splashkiller.Tick,
            Sub(s2, e2)
                splashkiller.Stop()
                splashkiller.Dispose()
                If _splash IsNot Nothing Then
                    _splash.Close()
                    _splash.Dispose()
                    _splash = Nothing
                End If
            End Sub
        splashkiller.Start()

        TileFactoryINISettings.Tile_TextUseSegoeUISymbol = INI.Tile_TextUseSegoeUISymbol
        TileFactory.EnsureRuntimeTileColorsSynchronized(INI.Tile_DontOverwriteExistingTileColorsFiles)

        TileFactoryAPI.Initialisierung()
        INI.Tile_TileColors_Load()

    End Sub

    Private Sub frmMain_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        Spielfeld.SFMain.CloseSpielfeld()

        If Me.WindowState = Global.System.Windows.Forms.FormWindowState.Maximized Then
            'Wenn in diesem Fall das Rectangle nicht verkleinert wird, meint Windows einen
            'ungültigen (dritten) Bildschirm vor sich zu haben und schaltet um auf Standardausgabe.
            Dim rect As New Rectangle(Bounds.Location, Bounds.Size)
            rect.Inflate(-100, -100)
            INI.Sonstiges_FrmMainStartupPosition = rect
        Else
            INI.Sonstiges_FrmMainStartupPosition = Me.Bounds
        End If
        INI.Sonstiges_FrmMainStartMaximized = Me.WindowState = Global.System.Windows.Forms.FormWindowState.Maximized
        INI.AllIniManagerSave()
        TileFactory.TileFactoryAPI.DisposeAll()

    End Sub
    'Private Sub frmMain_Move(sender As Object, e As EventArgs) Handles Me.Move
    '    If Me.WindowState = FormWindowState.Normal Then
    '        INI.Sonstiges_FrmMainStartupPosition = Me.Bounds
    '    End If
    'End Sub

    'Helfer
    Private Shared Function PickTargetScreen() As Screen
        ' Nimm den Screen unter dem Mauszeiger (fühlt sich „richtig“ an).
        ' Fallback: Primärmonitor.
        Dim scr As Screen = Screen.FromPoint(Cursor.Position)
        If scr Is Nothing Then scr = Screen.PrimaryScreen
        Return scr
    End Function

    Private Shared Function ComputeCenteredBoundsOnScreen(scr As Screen, formSize As Size) As Rectangle
        Dim wa As Rectangle = scr.WorkingArea
        Dim w As Integer = Math.Min(formSize.Width, Math.Max(200, wa.Width))
        Dim h As Integer = Math.Min(formSize.Height, Math.Max(150, wa.Height))
        Dim x As Integer = wa.Left + (wa.Width - w) \ 2
        Dim y As Integer = wa.Top + (wa.Height - h) \ 2
        Return New Rectangle(x, y, w, h)
    End Function

    Private Shared Function IsValidStartupRect(r As Rectangle) As Boolean
        If r.IsEmpty OrElse r.Width <= 0 OrElse r.Height <= 0 Then Return False
        ' Rect muss vollständig in genau EINEM Screen liegen
        For Each scr As Screen In Screen.AllScreens
            If scr.Bounds.Contains(r) Then Return True
        Next
        Return False
    End Function

#End Region

#Region "VisibleUserControl"

    '' --- Properties, Felder, Enums ---
    'Private menuEnableBindings As New List(Of Tuple(Of ToolStripMenuItem, Func(Of Boolean)))
    'Private menuVisibleBindings As New List(Of Tuple(Of ToolStripMenuItem, Func(Of Boolean)))

    Private Sub ChangeVisibleControl(ctrl As VisibleUserControl)
        AktVisibleUserControl = ctrl
    End Sub

    Private _aktVisibleUserControlValue As VisibleUserControl = VisibleUserControl.None
    ' Property mit automatischem Menü-Refresh

    ''' <summary>
    ''' Im Unterschied zu: AktVisibleUserControl As VisibleUserControl prüft die
    ''' Set-Variante, ob sich das VisibleUserControl geändert hat und gibt True 
    ''' zurück, wenn ja.
    ''' </summary>
    ''' <param name="value"></param>
    ''' <returns></returns>
    Public Function SetAktVisibleUserControl(value As VisibleUserControl) As Boolean
        If _aktVisibleUserControlValue = value Then
            Return False
        Else
            AktVisibleUserControl = value
            Return True
        End If
    End Function

    Public Property AktVisibleUserControl As VisibleUserControl
        Get
            Return _aktVisibleUserControlValue
        End Get

        Set(value As VisibleUserControl)

            If value = _aktVisibleUserControlValue Then
                Return
            End If

            SuspendLayout()

            ' Menü soll ganz oben
            MenuStripExMain.BringToFront()
            ' ToolStrip soll ganz unten
            ToolStripExMain.BringToFront()
            ' Panel in die Mitte
            PanelFrmMainUGrd.BringToFront()

            'den Namen des bisherigen sichern, um das bisherige von überall her
            'wieder Rückstellen zu können (z.B. aus UICtlAboutMahjongGK heraus)
            INI.Global_LastVisibleUserControl = _aktVisibleUserControlValue

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
                '
                'auch zur globalen Verwendung. 
                INI.Global_AktVisibleUserControl = _aktVisibleUserControlValue

            Catch ex As Exception
                If Debugger.IsAttached Then
                    Stop 'Das UserControl existiert noch nicht.
                End If
                Exit Property
            End Try

            'RefreshMenuStates()

            Spielfeld.SFMain.SetVisibleUserControl(VisibleUserControls(value), value)

            ResumeLayout(True)

        End Set
    End Property

#End Region

#Region "ToolStrip unten InitDragDropBitmaps"

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
            .Height = 30
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

            'ToolStripExMain.Items.AddRenderBitmapTopZOrder(MkBtnText("dbg_explorer", "Explorer",
            'Sub() Process.Start("explorer.exe", AppDataDirectory),
            'If(INI.Sonstiges_ShowToolTips, "Öffnet AppData-Verzeichnis", Nothing)))

            ToolStripExMain.Items.Add(MkBtnText("dbg_gfxCompiler", "Gfx",
            Sub() GfxCompiler(),
            If(INI.Sonstiges_ShowToolTips, "Den GfxCompiler aufrufen (Grafiken einsammeln und einbinden)", Nothing)))

            ToolStripExMain.Items.Add(MkBtnText("dbg_go", "Go",
            Sub() Go(),
            If(INI.Sonstiges_ShowToolTips, "frmMain Sub Go ausführen", Nothing)))

            ToolStripExMain.Items.Add(MkBtnText("dbg_test1", "T1",
            Sub() Test1(True),
            If(INI.Sonstiges_ShowToolTips, "frmMain Sub Test1 ausführen", Nothing)))

            ToolStripExMain.Items.Add(MkBtnText("dbg_test2", "T2",
            Sub() Test2(),
            If(INI.Sonstiges_ShowToolTips, "frmMain Sub Test2 ausführen", Nothing)))

            '  ToolStripExMain.Items.AddRenderBitmapTopZOrder(New ToolStripSeparator())

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
                ("GfxCompiler 16x16 Ressou", INI.IfRunningInIDE_Grafik16x16Directory_Ressourcen),
                ("GfxCompiler 16x16 Andere", INI.IfRunningInIDE_Grafik16x16Directory_Other)
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
            ToolStripExMain.Items.Add(MkBtnImg("grpEditor_player", Theme.GetResBmp(AppGrafikName.Spieler.ToString), Sub() DoSpielfeld(),
                If(INI.Sonstiges_ShowToolTips, "Ruft das Spielfeld auf.", Nothing)))
            ToolStripExMain.Items.Add(MkBtnImg("grpEditor_editor", Theme.GetResBmp(AppGrafikName.Editor.ToString), Sub() DoEditor(),
                If(INI.Sonstiges_ShowToolTips, "Ruft den Editor auf.", Nothing)))
            ToolStripExMain.Items.Add(MkBtnImg("grpEditor_toolbox", Theme.GetResBmp(AppGrafikName.Werkzeug.ToString), Sub() DoToolBox(),
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
        'ToolStripExMain.Items.AddRenderBitmapTopZOrder(MkLbl("stat_title", "Steine:"))
        'ToolStripExMain.Items.AddRenderBitmapTopZOrder(New ToolStripSeparator())
        ToolStripExMain.Items.Add(MkLbl("stat_total", "Summe Steine: 000"))
        ToolStripExMain.Items.Add(New ToolStripSeparator())
        ToolStripExMain.Items.Add(MkLbl("stat_current", "Aktuell: 000"))
        ToolStripExMain.Items.Add(New ToolStripSeparator())
        ToolStripExMain.Items.Add(MkLbl("stat_sel", "wählbar: 00"))
        ToolStripExMain.Items.Add(New ToolStripSeparator())
        ToolStripExMain.Items.Add(MkLbl("stat_pairs", "Paare: 00"))
        ToolStripExMain.Items.Add(New ToolStripSeparator())
        ToolStripExMain.Items.Add(MkLbl("stat_stock", "im Vorrat: 000"))
        ToolStripExMain.Items.Add(New ToolStripSeparator())

        ' =======================
        ' Rechte Gruppe (Alignment = Right)
        ' Einfüge-Reihenfolge = von rechts nach links
        ' =======================

        ToolStripExMain.Items.Add(MkBtnImgRight("act_screenshot", Theme.GetResBmp(AppGrafikName.Screenshot.ToString), Sub() DoTakeScreenShot(),
        If(INI.Sonstiges_ShowToolTips, "Erzeugt einen ScreenShot vom Spielfeld/Editor/Werkbank.", Nothing)))

        ToolStripExMain.Items.Add(MkBtnImgRight("act_statistik", Theme.GetResBmp(AppGrafikName.Statistik.ToString), Sub() DoStatistik(),
            If(INI.Sonstiges_ShowToolTips, "Statistische Daten zum Spielfeld", Nothing)))

        ToolStripExMain.Items.Add(MkSepRight())

        ' '' 4) Label "Zeige:"
        ''ToolStripExMain.Items.AddRenderBitmapTopZOrder(MkLblRight("act_show_lbl", "Zeige:"))

        ToolStripExMain.Items.Add(MkBtnImgTextRight("act_tip1", Theme.GetResBmp(AppGrafikName.Tip.ToString), "",
            Sub() DoTipEinzel(),
            If(INI.Sonstiges_ShowToolTips, "Schalter: Zeigt permanent wählbare Steine an.", Nothing)))

        Dim chkWinds As ToolStripButton = MkToggleImgRight("opt_winds_onegrp",
            Theme.GetResBmp(AppGrafikName.WindsChecked.ToString), Theme.GetResBmp(AppGrafikName.WindsUnChecked.ToString),
            INI.Spielbetrieb_WindsAreInOneClickGroup,
            Sub(checked)
                If _isRefreshing Then Return
                INI.Spielbetrieb_WindsAreInOneClickGroup = checked
                ' ggf. Regeln neu anwenden/Refresh
            End Sub,
            If(INI.Sonstiges_ShowToolTips, "Schalter vereinfachte Spielregel: Alle Winde können untereinander Paare bilden.", Nothing))

        ToolStripExMain.Items.Add(chkWinds)

        ToolStripExMain.Items.Add(MkSepRight())

        ToolStripExMain.Items.Add(MkBtnImgTextRight("act_wählbar", Theme.GetResBmp(AppGrafikName.Tipps.ToString), "",
            Sub() DoTipAlle(),
            If(INI.Sonstiges_ShowToolTips, "Tip: Zeigt alle wählbaren Paare an.", Nothing)))

        Dim chkSel As ToolStripButton = MkToggleImgRight("opt_show_sel",
                Theme.GetResBmp(AppGrafikName.ShowSelectableChecked.ToString), Theme.GetResBmp(AppGrafikName.ShowSelectableUnChecked.ToString),
                INI.Spielbetrieb_ShowSelectableStones,
                Sub(checked)
                    If _isRefreshing Then Return
                    INI.Spielbetrieb_ShowSelectableStones = checked
                    ' ggf. Refresh/Neuzeichnen hier
                End Sub,
                If(INI.Sonstiges_ShowToolTips, "Tip: Zeigt alle wählbaren Steine an.", Nothing))
        ToolStripExMain.Items.Add(chkSel)

        ToolStripExMain.Items.Add(MkSepRight())

        ToolStripExMain.Items.Add(MkBtnImgRight("act_restart", Theme.GetResBmp(AppGrafikName.Restart.ToString), Sub() DoReDo(),
            If(INI.Sonstiges_ShowToolTips, "Stell das Spielfeld auf die Ausgangsstellung zurück", Nothing)))

        ToolStripExMain.Items.Add(MkBtnImgRight("act_redo", Theme.GetResBmp(AppGrafikName.Redo.ToString), Sub() DoReDo(),
            If(INI.Sonstiges_ShowToolTips, "Arbeitet wieder vorwärts, solange das noch möglich ist", Nothing)))

        ToolStripExMain.Items.Add(MkBtnImgRight("act_undo", Theme.GetResBmp(AppGrafikName.Undo.ToString), Sub() DoUndo(),
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
        .ImageAlign = ContentAlignment.MiddleLeft,
            .Alignment = ToolStripItemAlignment.Right
        }
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
                btnShowSel.Image = If(btnShowSel.Checked, Theme.GetResBmp(AppGrafikName.ShowSelectableChecked.ToString), Theme.GetResBmp(AppGrafikName.ShowSelectableUnChecked.ToString))
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
                btnWinds.Image = If(btnWinds.Checked, Theme.GetResBmp(AppGrafikName.WindsChecked.ToString), Theme.GetResBmp(AppGrafikName.WindsUnChecked.ToString))
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

    Public Sub DoSpielfeld()
        'TODO SFD - Anpassung
        If SFMain.RenderMode = RenderMode.NoRendering Then
            MsgBox("Kein Spielfeld geladen", MsgBoxStyle.Information)
        Else
            SFMain.RenderMode = RenderMode.Spiel
        End If
        UpdateSpielfeldEditorButtons()
    End Sub

    Private Sub DoEditor()
        If SFMain.RenderMode = RenderMode.NoRendering Then
            MsgBox("Kein Spielfeld geladen", MsgBoxStyle.Information)
        Else
            SFMain.RenderMode = RenderMode.Edit
        End If
        UpdateSpielfeldEditorButtons()
    End Sub

    Public Sub DoToolBox()
        If SFMain.RenderMode = RenderMode.NoRendering Then
            MsgBox("Kein Spielfeld geladen", MsgBoxStyle.Information)
        Else
            ShowOrHideToolboxAndUpdateToolboxButton()
        End If
    End Sub

    Private Sub DoTakeScreenShot()
        If SFMain.RenderMode = RenderMode.NoRendering Then
            MsgBox("Kein Spielfeld geladen", MsgBoxStyle.Information)
        Else
            SFMain.SFDat.SFRenMan.PaintSpielfeld_CreateScreenShot()
            MsgBox("Screnshot erzeugt.", MsgBoxStyle.Information)
        End If
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

    ''' <summary>
    ''' Aktualisiert die Statuswerte (Gesamt, Aktuell, Wählbar, Paare).
    ''' </summary>
    Public Sub UpdateStatus(gesamt As Integer, aktuell As Integer, waehlbar As Integer, paare As Integer, maxSteine As Triple)
        Dim lblSize As ToolStripLabel = TryCast(ToolStripExMain.Items("stat_size"), ToolStripLabel)
        Dim lblGesamt As ToolStripLabel = TryCast(ToolStripExMain.Items("stat_total"), ToolStripLabel)
        Dim lblAktuell As ToolStripLabel = TryCast(ToolStripExMain.Items("stat_current"), ToolStripLabel)
        Dim lblWaehlbar As ToolStripLabel = TryCast(ToolStripExMain.Items("stat_sel"), ToolStripLabel)
        Dim lblPaare As ToolStripLabel = TryCast(ToolStripExMain.Items("stat_pairs"), ToolStripLabel)

        If lblGesamt IsNot Nothing Then lblGesamt.Text = $"{gesamt} Gesamt"
        If lblAktuell IsNot Nothing Then lblAktuell.Text = $"{aktuell} Aktuell"
        If lblWaehlbar IsNot Nothing Then lblWaehlbar.Text = $"{waehlbar} wählbar"
        If lblPaare IsNot Nothing Then lblPaare.Text = $"{paare} Paare"
        If lblSize IsNot Nothing Then lblSize.Text = $"Feldgröße: {maxSteine.x }/{maxSteine.y}/{maxSteine.z }"
    End Sub

    Public Sub UpdateSpielfeldEditorButtons()

        If Not INI.Editor_UsingEditorAllowed Then
            Exit Sub
        End If

        Dim btnPlayer As ToolStripButton = TryCast(ToolStripExMain.Items("grpEditor_player"), ToolStripButton)

        If btnPlayer Is Nothing Then
            'Formular noch nicht initialisiert. Die Anderen sind dann auch Nothing
            Exit Sub
        End If

        Dim btnEditor As ToolStripButton = TryCast(ToolStripExMain.Items("grpEditor_editor"), ToolStripButton)
        'TODO SFD-Anpassung
        Select Case Spielfeld.SFMain.RenderMode
            Case RenderMode.NoRendering, RenderMode.Paused
                btnPlayer.Image = Theme.GetResBmp(AppGrafikName.Spieler.ToString)
                btnEditor.Image = Theme.GetResBmp(AppGrafikName.Editor.ToString)

            Case RenderMode.Edit
                btnPlayer.Image = Theme.GetResBmp(AppGrafikName.Spieler.ToString)
                btnEditor.Image = Theme.GetResBmp(AppGrafikName.EditorAktiv.ToString)

            Case RenderMode.Spiel
                btnPlayer.Image = Theme.GetResBmp(AppGrafikName.SpielerAktiv.ToString)
                btnEditor.Image = Theme.GetResBmp(AppGrafikName.Editor.ToString)
        End Select

    End Sub

    Public Sub ShowOrHideToolboxAndUpdateToolboxButton()

        If Not INI.Editor_UsingEditorAllowed Then
            Exit Sub
        End If

        ''Sicherstellen, daß Daten für Spielfeld/Editor/Werkbank geladen sind.
        'Spielfeld.EnsureSpielfeldInfoAreAvailable(startRenderingWithOrToggleTo:=AktRenderMode.Edit)

        ' 2) Button sicher ermitteln
        Dim btnToolBox As ToolStripButton = Nothing
        Dim item As ToolStripItem = Nothing
        If ToolStripExMain.Items.ContainsKey("grpEditor_toolbox") Then
            item = ToolStripExMain.Items("grpEditor_toolbox")
            btnToolBox = TryCast(item, ToolStripButton)
        End If

        If Not INI.ToolBox_VolatilFormIsVisible Then

            If btnToolBox IsNot Nothing Then
                btnToolBox.Image = Theme.GetResBmp(AppGrafikName.WerkzeugAktiv.ToString)
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

            Try
                ' 4) Als Owned-Form von frmMain anzeigen → bleibt über frmMain, aber nicht über anderen Apps
                _frmToolBox.Show(Me)       ' Owner setzen
                _frmToolBox.BringToFront() ' sicher nach vorne holen
                ' Optional:
                ' _frmToolBox.Activate()

                ' 5) Maus-Overlay registrieren (falls noch nicht geschehen)
                'sonst wird die Toolbox von der Maus abgefangen und Spielfeld bekommt keine Mausbewegungen mehr,
                'wenn ein Stein unter der Toolbox hindurch gezogen wird.
                ' Spielfeld.SFD.MousePolling.RegisterOverlay(_frmToolBox.Handle) 'ist jetzt in frmToolBox
            Catch ex As Exception
                'ist bereits aktiv
            End Try

        Else
            If btnToolBox IsNot Nothing Then
                btnToolBox.Image = Theme.GetResBmp(AppGrafikName.Werkzeug.ToString)
                btnToolBox.Checked = False
            End If

            If _frmToolBox IsNot Nothing AndAlso Not _frmToolBox.IsDisposed Then
                _frmToolBox.Hide()
            End If
        End If

        INI.ToolBox_VolatilFormIsVisible = Not INI.ToolBox_VolatilFormIsVisible

    End Sub

    Public ReadOnly Property ToolBox As frmToolBox
        Get
            Return _frmToolBox
        End Get
    End Property

#End Region

#Region "Menüverwaltung"

    Private Structure StoneMenuSelection

        Public Sub New(font As SteinFont,
                       design As SteinDesign,
                       satz As SteinSatz)

            Me.Font = font
            Me.Design = design
            Me.Satz = satz

        End Sub

        Public ReadOnly Font As SteinFont
        Public ReadOnly Design As SteinDesign
        Public ReadOnly Satz As SteinSatz

    End Structure

    Private Structure StoneDesignMenuSelection

        Public Sub New(font As SteinFont,
                       design As SteinDesign)

            Me.Font = font
            Me.Design = design

        End Sub

        Public ReadOnly Font As SteinFont
        Public ReadOnly Design As SteinDesign

    End Structure

    Private ReadOnly _stoneFontItems As New System.Collections.Generic.List(Of ToolStripMenuItem)
    Private ReadOnly _stoneDesignItems As New System.Collections.Generic.List(Of ToolStripMenuItem)
    Private ReadOnly _stoneDesignLeafItems As New System.Collections.Generic.List(Of ToolStripMenuItem)

    Private Sub InitializeMainMenu()

        Dim mnuSpielOeffnen As New ToolStripMenuItem("&Spiel öffnen (Einzeln)")
        Dim mnuSpielauswählen As New ToolStripMenuItem("S&piel auswählen (Übersicht)")
        Dim mnuSteinDesign As New ToolStripMenuItem("S&tein-Design")
        Dim mnuEinstellungen As New ToolStripMenuItem("&Einstellungen")
        Dim mnuAbout As New ToolStripMenuItem("&About")
        Dim mnuHilfe As New ToolStripMenuItem("&Hilfe")

        Dim mnuNeuesSpielOeffnen As New ToolStripMenuItem("&Neues Spiel öffnen")
        Dim mnuLetztesSpielOeffnen As New ToolStripMenuItem("&Letztes Spiel öffnen")
        Dim mnuNeuesSpielAnlegen As New ToolStripMenuItem("N&eues Spiel anlegen")

        Dim mnuSegoe As New ToolStripMenuItem("&Segoe")
        Dim mnuNoto As New ToolStripMenuItem("&Noto")

        mnuSegoe.Tag = SteinFont.Segoe
        mnuNoto.Tag = SteinFont.Noto

        _stoneFontItems.Add(mnuSegoe)
        _stoneFontItems.Add(mnuNoto)

        Me.MenuStripExMain.SuspendLayout()

        Try
            Me.MenuStripExMain.Items.Clear()
            _stoneFontItems.Clear()
            _stoneDesignItems.Clear()
            _stoneDesignLeafItems.Clear()

            mnuSegoe.Tag = SteinFont.Segoe
            mnuNoto.Tag = SteinFont.Noto

            _stoneFontItems.Add(mnuSegoe)
            _stoneFontItems.Add(mnuNoto)

            AddHandler mnuNeuesSpielOeffnen.Click,
            Sub(sender As Object, e As EventArgs)
                Go()
                'Me.AktVisibleUserControl = VisibleUserControl.SpielfeldWählen
            End Sub

            AddHandler mnuLetztesSpielOeffnen.Click,
            Sub(sender As Object, e As EventArgs)
                OpenLastSpiel()
            End Sub

            AddHandler mnuNeuesSpielAnlegen.Click,
            Sub(sender As Object, e As EventArgs)
                OpenNewSpiel()
            End Sub

            mnuSpielOeffnen.DropDownItems.Add(mnuNeuesSpielOeffnen)
            mnuSpielOeffnen.DropDownItems.Add(mnuLetztesSpielOeffnen)
            mnuSpielOeffnen.DropDownItems.Add(New ToolStripSeparator())
            mnuSpielOeffnen.DropDownItems.Add(mnuNeuesSpielAnlegen)

            AddHandler mnuSpielOeffnen.Click,
            Sub(sender As Object, e As EventArgs)
                Me.AktVisibleUserControl = VisibleUserControl.Spielfeld
            End Sub

            AddHandler mnuSpielauswählen.Click,
            Sub(sender As Object, e As EventArgs)
                Me.AktVisibleUserControl = VisibleUserControl.Spielauswahl
            End Sub

            '  mnuSpielauswählen.DropDownItems.Add(mnuSpielauswählen)

            BuildStoneDesignSubMenu(parentMenu:=mnuSegoe, steinFont:=SteinFont.Segoe)
            BuildStoneDesignSubMenu(parentMenu:=mnuNoto, steinFont:=SteinFont.Noto)

            mnuSteinDesign.DropDownItems.Add(mnuSegoe)
            mnuSteinDesign.DropDownItems.Add(mnuNoto)

            AddHandler mnuSteinDesign.DropDownOpening,
            Sub(sender As Object, e As EventArgs)
                UpdateStoneDesignMenuChecks()
            End Sub

            AddHandler mnuEinstellungen.Click,
            Sub(sender As Object, e As EventArgs)
                Me.AktVisibleUserControl = VisibleUserControl.Einstellungen
            End Sub

            AddHandler mnuAbout.Click,
            Sub(sender As Object, e As EventArgs)
                Me.AktVisibleUserControl = VisibleUserControl.About
            End Sub

            AddHandler mnuHilfe.Click,
            Sub(sender As Object, e As EventArgs)
                Me.AktVisibleUserControl = VisibleUserControl.Hilfe
            End Sub

            Me.MenuStripExMain.Items.AddRange(
            New ToolStripItem() {
                mnuSpielOeffnen,
                mnuSpielauswählen,
                mnuSteinDesign,
                mnuEinstellungen,
                mnuAbout,
                mnuHilfe
            })

            UpdateStoneDesignMenuChecks()

        Finally
            Me.MenuStripExMain.ResumeLayout()
        End Try

    End Sub

    Private Sub BuildStoneDesignSubMenu(parentMenu As ToolStripMenuItem,
                                        steinFont As SteinFont)

        For Each currentDesign As SteinDesign In [Enum].GetValues(GetType(SteinDesign))

            If Not INI.Tile_AllowTileColorsTestFiles Then
                If currentDesign.ToString.StartsWith("Test") Then
                    Continue For
                End If
            End If

            Dim mnuDesign As New ToolStripMenuItem(GetEnumDisplayText(currentDesign)) With {
                .Tag = New StoneDesignMenuSelection(
                    font:=steinFont,
                    design:=currentDesign)
            }

            _stoneDesignItems.Add(mnuDesign)

            For Each currentSatz As SteinSatz In [Enum].GetValues(GetType(SteinSatz))

                Dim mnuSatz As New ToolStripMenuItem(GetEnumDisplayText(currentSatz)) With {
                    .Tag = New StoneMenuSelection(
                        font:=steinFont,
                        design:=currentDesign,
                        satz:=currentSatz)
                }

                AddHandler mnuSatz.Click, AddressOf OnStoneDesignLeafClick

                mnuDesign.DropDownItems.Add(mnuSatz)
                _stoneDesignLeafItems.Add(mnuSatz)

            Next

            parentMenu.DropDownItems.Add(mnuDesign)

        Next

    End Sub

    Private Sub OnStoneDesignLeafClick(sender As Object, e As EventArgs)

        Dim clickedItem As ToolStripMenuItem = DirectCast(sender, ToolStripMenuItem)
        Dim selection As StoneMenuSelection = CType(clickedItem.Tag, StoneMenuSelection)

        INI.Tile_SteinFont = selection.Font
        INI.Tile_SteinDesign = selection.Design
        INI.Tile_SteinSatz = selection.Satz
        INI.Tile_TileColors_Load()

        UpdateStoneDesignMenuChecks()

    End Sub

    Private Sub UpdateStoneDesignMenuChecks()

        For Each currentItem As ToolStripMenuItem In _stoneFontItems

            Dim currentFont As SteinFont = CType(currentItem.Tag, SteinFont)

            currentItem.Checked = (INI.Tile_SteinFont = currentFont)

        Next

        For Each currentItem As ToolStripMenuItem In _stoneDesignItems

            Dim selection As StoneDesignMenuSelection = CType(currentItem.Tag, StoneDesignMenuSelection)

            currentItem.Checked =
                (INI.Tile_SteinFont = selection.Font) AndAlso
                (INI.Tile_SteinDesign = selection.Design)

        Next

        For Each currentItem As ToolStripMenuItem In _stoneDesignLeafItems

            Dim selection As StoneMenuSelection = CType(currentItem.Tag, StoneMenuSelection)

            currentItem.Checked =
                (INI.Tile_SteinFont = selection.Font) AndAlso
                (INI.Tile_SteinDesign = selection.Design) AndAlso
                (INI.Tile_SteinSatz = selection.Satz)

        Next

    End Sub

    Private Shared Function GetEnumDisplayText(value As [Enum]) As String

        Return value.ToString().Replace("_"c, " "c)

    End Function

    Private Sub OpenLastSpiel()

    End Sub

    Private Sub OpenNewSpiel()

    End Sub

    Private Sub OpenNewBackGround(save As Boolean)

    End Sub

#End Region

#Region "INI Testen"
    Private Sub CheckIniFileWhenStartedInIde()

        If Not Debugger.IsAttached Then
            Return
        End If

        Dim fullPath As String = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "Visual Studio",
            "MahjongGK",
            "MahjongGK",
            "INI",
            "INI.vb")

        CheckIniManagers(fullPath)

    End Sub

    Private Sub CheckIniManagers(fullPath As String)

        If Not File.Exists(fullPath) Then
            MessageBox.Show("INI-Datei nicht gefunden:" & Environment.NewLine & fullPath,
                            "Schwerer Fehler in der INI",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error)
            End
        End If

        Dim text As String = File.ReadAllText(fullPath)

        Dim propertyRegex As New Regex(
            "(?<header>^\s*(?:Public|Friend|Private)\s+(?:Shared\s+)?Property\s+(?<name>\[?[A-Za-z_][A-Za-z0-9_]*\]?).*?$)(?<body>.*?^\s*End\s+Property\b)",
            RegexOptions.Multiline Or RegexOptions.Singleline)

        For Each propertyMatch As Match In propertyRegex.Matches(text)

            Dim propertyName As String = propertyMatch.Groups("name").Value
            propertyName = propertyName.Trim("["c, "]"c)

            Dim body As String = propertyMatch.Groups("body").Value

            Dim getMatch As Match = Regex.Match(body,
                                                "^\s*Get\b(?<getbody>.*?)(^\s*End\s+Get\b)",
                                                RegexOptions.Multiline Or RegexOptions.Singleline)

            Dim setMatch As Match = Regex.Match(body,
                                                "^\s*Set\b.*?$(?<setbody>.*?)(^\s*End\s+Set\b)",
                                                RegexOptions.Multiline Or RegexOptions.Singleline)

            If Not getMatch.Success Then
                Continue For
            End If

            Dim getterText As String = getMatch.Groups("getbody").Value
            Dim setterText As String = If(setMatch.Success, setMatch.Groups("setbody").Value, "")

            Dim readManager As String = GetSingleIniManager(getterText, "ReadValue", propertyName)
            Dim writeManager As String = GetSingleIniManager(setterText, "WriteValue", propertyName)

            If readManager = "" Then
                Continue For
            End If

            If writeManager = "" Then
                IniFatal(propertyName,
                         "Im Getter steht " & readManager & ".ReadValue(...), aber im Setter fehlt die passende WriteValue-Funktion.")
            End If

            If readManager <> writeManager Then
                IniFatal(propertyName,
                         "Getter und Setter greifen auf unterschiedliche INI-Manager zu:" &
                         Environment.NewLine &
                         "Getter: " & readManager & ".ReadValue(...)" &
                         Environment.NewLine &
                         "Setter: " & writeManager & ".WriteValue(...)")
            End If

        Next

    End Sub

    Private Function GetSingleIniManager(sourceText As String,
                                         functionName As String,
                                         propertyName As String) As String

        Dim qualifiedRegex As New Regex(
            "(?<manager>[A-Za-z_][A-Za-z0-9_]*)\s*\.\s*" & functionName & "\s*\(",
            RegexOptions.Multiline)

        Dim unqualifiedRegex As New Regex(
            "(?<!\.)\b" & functionName & "\s*\(",
            RegexOptions.Multiline)

        If unqualifiedRegex.IsMatch(sourceText) Then
            IniFatal(propertyName,
                     functionName & "(...) ist nicht mit einem INI-Manager qualifiziert.")
        End If

        Dim managers As New List(Of String)

        For Each m As Match In qualifiedRegex.Matches(sourceText)

            Dim manager As String = m.Groups("manager").Value

            If Not managers.Contains(manager) Then
                managers.Add(manager)
            End If

        Next

        If managers.Count = 0 Then
            Return ""
        End If

        If managers.Count > 1 Then
            IniFatal(propertyName,
                     "Innerhalb derselben Property werden mehrere INI-Manager für " & functionName & "(...) verwendet: " &
                     String.Join(", ", managers))
        End If

        Return managers(0)

    End Function

    Private Sub IniFatal(propertyName As String, detail As String)

        MessageBox.Show("Schwerer Fehler in der INI, zuerst beseitigen." &
                        Environment.NewLine &
                        "PropertyName: " & propertyName &
                        Environment.NewLine &
                        Environment.NewLine &
                        detail,
                        "INI-Prüfung",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error)

    End Sub

#End Region

End Class