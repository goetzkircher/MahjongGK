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
Imports System.Globalization
Imports System.IO
Imports System.Reflection
Imports MahjongGK.Contracts
Imports MahjongGK.Contracts.GlobalEnum
Imports MahjongGK.Spielfeld
Imports TileFactory

Public Module INI

#Region "Kopf"

    '' Kopiervorlage für die Pfad/ Dateinamen-Enumeration
    '' (Die verwendeten Enumerationen sind in Shared/Enumerationen)
    ''
    '' ''' <summary>
    '' ''' Enumeration der verwendeten Unterverzeichnisse in "C:\Users\aktueller User\MahjongGK\SubDefault.value.ToString"
    '' ''' Verwendung: Entweder über Dim Path As String = INI.AppDataDefault(.....)
    '' ''' oder durch Nutzung (im Modul INI) einer Public Property Kopier_VorlageFürPfade As String
    '' ''' Die gewünschten Pfade werden automatisch angelegt.
    '' ''' </summary>
    '' Public Enum AppDataSubDir
    ''     None
    ''     INI
    ''     Steine
    '' End Enum
    '' ''' <summary>
    '' ''' Enumeration der verwendeten Unterverzeichnisse in "C:\Users\aktueller User\MahjongGK\SubDefault.value.ToString\SubSubDefault.value.ToString"
    '' ''' Verwendung wie AppDataSubDir
    '' ''' </summary>
    '' Public Enum AppDataSubSubDir
    ''     None
    ''     letztesSpiel
    ''     Layout
    '' End Enum

    '' ''' <summary>
    '' ''' Enumeration der verwendeten Dateinamen.
    '' ''' Die Endung mit einem Unterstrich abtrennen.
    '' ''' Die Endung muss 3 Zeichen lang sein.
    '' ''' </summary>
    '' Public Enum AppDataFileName
    ''     None
    ''     Steininfos_xml
    '' End Enum

    '' Public Enum AppDataTimeStamp
    ''     None
    ''     AddRenderBitmapTopZOrder
    ''     LookForLastTimeStamp
    '' End Enum

    '''' <summary>
    '''' In dieser Enum kann ein Pattern verschlüsselt werden.
    '''' Es gilt: 
    '''' _Q_ = ? (Question, Fragezeichen),
    '''' _N_ = # (Number),
    '''' _S_ = * (Stern, Star),
    '''' _D_ = . (Dot, Punkt).
    '''' _SD_ = *.
    '''' _SDS_ = *.*
    '''' Beispiel: Dateiname_S__D_ext --> Dateiname*.ext
    '''' </summary>
    'Public Enum AppDataFilePattern
    '    None
    '    Steininfos_xml
    'End Enum

    ' Zentrale Sammlung aller IniManager
    'Wird aus New IniManager("Basis.ini") heraus gefüllt.
    Public ReadOnly AllIniManagers As New List(Of IniManager)

    'Hinweis: In frmIniEditor muss _iniIniFileNames noch ergänzt werden, damit die Werte editiert werden können.

    Public ReadOnly BasisIni As IniManager
    Public ReadOnly ToolBoxIni As IniManager
    Public ReadOnly Rendering As IniManager

    Sub New()
        'Instanzen für verschiedene INIs hier anlegen
        BasisIni = New IniManager("Basis.ini")
        ToolBoxIni = New IniManager("ToolBox.ini")
        Rendering = New IniManager("Rendering.ini")

        'Spieler1Ini = New IniManager("Spieler1.ini")
        'Spieler2Ini = New IniManager("Spieler2.ini")

        IniCleanup.RemoveObsoleteIniKeys()

        AllIniManagersSetRaiseIniEvents(IniEvents.None)
        'Verhindert den Aufruf der Events hier in INI.
        AllIniManagersInitAllDefaultsAndSave()
        AllIniManagerSave()
        AllIniManagersSetRaiseIniEvents(IniEvents.OnWriteValue)
    End Sub
    '
    ''' <summary>
    ''' Wenn update True ist, wird die Ini neu eingelesen und alle Events ausgelößt. 
    ''' Der Wert von raiseIniEventsDefault bleibt stehen, bis er geändert wird.
    ''' (Derzeit in der BasisINI)
    ''' </summary>
    Public Sub Initialisierung(update As Boolean, raiseIniEventsDefault As IniEvents)

        If update Then
            UpDateIni(IniEvents.OnUpdate, readNewIniFromIniTmp:=False)
        End If

        AllIniManagersSetRaiseIniEvents(raiseIniEventsDefault)

        'Irgendwelcher weiterer Code ist hier nicht notwendig.
        'Sub New() wird aufgerufen, sobald der erste Zugriff auf INI erfolgt.
    End Sub

    ''' <summary>
    ''' Muss aus frmMain.FormClosing heraus aufgerufen werden, um sicherzustellen,
    ''' dass die INI-Daten alle gespeichert werden.
    ''' </summary>
    Public Sub DisposeIniManager()
        BasisIni.Dispose()
        'Spieler1Ini.Dispose()
        'Spieler2Ini.Dispose()
    End Sub

    'In den Properties muss dann nur Return BasisIni.ReadValue(... und
    'BasisIni.WriteValue(... angepasst werden, auf welche Instanz
    'zugegriffen werden soll.
    '
    'Die Verwaltung um Spieler1Ini und Spieler2Ini auszutauschen ist noch nicht geschrieben.
    '
    'Vorbemerkung:
    'Das Anlegen einer Property für jede Eigenschaft, die in die INI gespeichert wird,
    'lohnt sich meiner Erfahrung nach, da die Namen während der Programmentwicklung
    'selten so bleiben, wie sie angelegt wurden, und so die Namen in der INI automatisch
    'mit geändert werden.
    'Zudem lässt sich weitere Funktionalität gleich mit einbauen.
    'Das System hat auch Nachteile, aber für mich überwiegen die Vorteile.
    'Ich nutze das System seit es VB.Net gibt.
    '
    'EVENTS: Auch das ist möglich.
    '
    'Einfach ein Public Event Kopier_Vorlage_Event(value As String) 
    '(oder einem anderen As..) anlegen
    'Dann:  BasisIni.WritePath(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
    'Ändern: If BasisIni.WritePath(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value) then
    '             RaiseEvent  Kopier_Vorlage_Event(value)
    '        End If
    '        
    'Beim Empfänger mus das Event aboniert werden.
    'In der Form oder im UserControl:
    '
    'Private Sub Form_Load(sender As Object, e As EventArgs) Handles Me.Load
    '   AddHandler INI.Editor_UsingEditorAllowed_Changed, AddressOf SubInDerForm
    'End Sub
    '
    'Private Sub Form_Disposed(sender As Object, e As EventArgs) Handles Me.Disposed
    '   RemoveHandler INI.Editor_UsingEditorAllowed_Changed, AddressOf SubInDerForm
    'End Sub
    '
    'Private Sub SubInDerForm(value as String)
    '   ....
    'End Sub
    '
    'Wann events ausgelöst werden, kann gesteuert werden über die Enumeration
    '
    'Public Enum IniEvents
    '    None
    '    OnChangeValue
    '    OnWriteValue
    '    OnUpdate
    'End Enum

    '
    'WICHTIG:
    'Die Namen der Properties müssen alle mindestens einen Unterstrich haben!
    'Das vor dem (erstem) Unterstrich ist der Folder-Name, das dahinter der Key.
    'genauso WICHTIG:
    'Alle anderen Property, Function oder Sub im Modul dürfen KEINE Unterstriche haben.
    'Es kann zu schwer zu findende Seiteneffekten kommen, weil Automatismen integriert
    'sind, die nach dem Unterstrich selektieren.
    '
    'Die erzeugten Dateien sind normale txt-Dateien.
    'Hat während der Programmentwicklung den Vorteil, dass Werte geändert werden können,
    'obwohl die "Einstellungen" noch nicht programmiert sind.
    '
    'Wenn die Namen der Properties geändert werden oder Properties gelöscht werden,
    'entstehen verweiste Einträge. Diese werden automatisch entfernt, samt Kommentar und Wert.

    'Ich nutze das Modul INI auch, um wichtige globale Werte zu speichern, die
    'nicht über das Programmende hinaus gespeichert werden müssen, oder für
    'ReadOnly Properies, deren Wert abhängig ist von anderen gespeicherten
    'Ini-Werten.

    '
    ' Jedem Wert kann ein Kommentar hinzugefügt werden, der dann in der INI
    ' erscheint. Update und Löschung der Kommentare werden automatisch aktualisiert.
    ' Der Zeilentrenner für mehrzeilige Kommentare ist die Tilde "~".
    '----------------------------------
    ' Wrapper-Properties für Basisdaten
    '----------------------------------
    '
    'Es werden folgende Werte unterstützt:
    'Byte, Integer, Long, Single, Double, Decimal, Date, Enumerationen
    'Color, Point, PointF, Size, SizeF, Rectangle, RechtangleF
    'und Font

#End Region

#Region "Kopiervorlagen"

    '--------------------------
    '--- Strings für PFADE ----
    '--------------------------
    '    Pfade haben eine eigene Vorlage, da sie eine absolut/relativ-Konvertierung
    '    durchlaufen, um die Pfade zu unterschiedlichen Computern kompatibel zu machen.
    '    (Weitergeben der Dateien unterhalb des Programmverzeichnis, des Dokumenten-
    '    Verzeichnisses und der INI-Datei)
    '    Bitte weiterlesen, es gibt noch eine komfortable Alternative.

    ' ''' <summary>
    ' ''' Hinweis 1: Vor Verwendung prüfen, ob nicht AppDataFullPath verwendet werden kann.
    ' ''' Vorteil: Die Enumerationen  AppDataSubDir, AppDataSubSubDir, AppDataFileName
    ' ''' und AppDataFilePattern können verwendet werden. Es ist ein OpenFileDialog und andere
    ' ''' Funktionen integriert.
    ' ''' Hinweis 2: Die Pfade werden als relative Pfade gespeichert, sofern sie sich unterhalb
    ' ''' Environment.SpecialFolder.MyDocuments oder unterhalb des Programmverzeichnisses
    ' ''' befinden. (Ein Verzeichnis über MyDocuments und dort das Verzeichnis MahjongGK)
    ' ''' </summary>
    ' ''' <returns>Default: Environment.SpecialFolder.MyDocuments </returns>
    ' 'Public Property Kopier_VorlageFürPfade As String
    '    Get
    '        Dim [Default] As String = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
    '        Dim comment As String = Nothing
    '        Return BasisIni.ReadPath(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
    '    End Get
    '    Set(value As String)
    '        BasisIni.WritePath(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
    '    End Set
    'End Property

    '---------------------------
    '--- für normale STRINGS ---
    '---------------------------

    'Public Property Kopier_Vorlage As String
    '    Get
    '        Dim [Default] As String = Nothing
    '        Dim comment As String = Nothing
    '        Return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
    '    End Get
    '    Set(value As String)
    '        BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
    '    End Set
    'End Property

    ''Für zeitkritische häufige Abfragen
    'Private _Kopier_Vorlage As String = Nothing
    'Private _Kopier_Vorlage_Loaded As Boolean = False

    'Public Property Kopier_Vorlage As String
    '    Get
    '        If Not _Kopier_Vorlage_Loaded Then
    '            Dim [Default] As String = "" ' oder Nothing oder Wert
    '            Dim comment As String = Nothing
    '            _Kopier_Vorlage = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
    '            _Kopier_Vorlage_Loaded = True
    '        End If
    '        Return _Kopier_Vorlage
    '    End Get
    '    Set(value As String)
    '        BasisIni.WriteValue(
    '        FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name),
    '        value)
    '        ' Cache sofort aktualisieren, kein Re-Read:
    '        _Kopier_Vorlage = value
    '        _Kopier_Vorlage_Loaded = True
    '    End Set
    'End Property
    '
    '------------
    '--- CHAR ---
    '------------

    'Public Property Kopier_Vorlage As Char
    '    Get
    '        Dim [Default] As Char = ControlChars.NullChar
    '        Dim comment As String = Nothing
    '        Return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
    '    End Get
    '    Set(value As Char)
    '        BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
    '    End Set
    'End Property

    '--- Boolean ----

    'Public Property Kopier_Vorlage As Boolean
    '    Get
    '        Dim [Default] As Boolean = False
    '        Dim comment As String = Nothing
    '        Return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
    '    End Get
    '    Set(value As Boolean)
    '        BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value.ToString)
    '    End Set
    'End Property

    ''Für zeitkritische häufige Abfragen
    'Private _Kopier_Vorlage As Boolean?
    'Public Property Kopier_Vorlage As Boolean
    '    Get
    '        If IsNothing(_Kopier_Vorlage) Then
    '            Dim [Default] As Boolean = False
    '            Dim comment As String = Nothing
    '            _Kopier_Vorlage = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
    '        End If
    '        Return CBool(_Kopier_Vorlage)
    '    End Get
    '    Set(value As Boolean)
    '        BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value.ToString)
    '        _Kopier_Vorlage = Nothing
    '    End Set
    'End Property
    '
    '------------
    '--- BYTE ---
    '------------

    '    Public Property Kopier_Vorlage As Byte
    '        Get
    '            Dim [Default] As Byte = 0
    '            Dim comment As String = Nothing
    '            Return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
    '        End Get
    '        Set(value As Byte)
    '            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value.ToString)
    '        End Set
    '    End Property

    '---------------
    '--- INTEGER ---
    '---------------

    '    Public Property Kopier_Vorlage As Integer
    '        Get
    '            Dim [Default] As Integer = 0
    '            Dim comment As String = Nothing
    '            Return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
    '        End Get
    '        Set(value As Integer)
    '            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value.ToString)
    '        End Set
    '    End Property

    ''für zeitkritische Abfragen
    'Private _Kopier_Vorlage As Integer?
    'Public Property Kopier_Vorlage As Integer
    '    Get
    '        If Not _Kopier_Vorlage.HasValue Then
    '            Dim [Default] As Integer = 0
    '            Dim comment As String = Nothing
    '            _Kopier_Vorlage = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
    '        End If
    '        Return _Kopier_Vorlage.Value
    '    End Get
    '    Set(value As Integer)
    '        BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
    '        _Kopier_Vorlage = Nothing
    '    End Set
    'End Property
    '
    '------------
    '--- LONG ---
    '------------
    '
    'Public Property Kopier_Vorlage As Long
    '    Get
    '        Dim [Default] As Long = 0
    '        Dim comment As String = Nothing
    '        Return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
    '    End Get
    '    Set(value As Long)
    '        BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value.ToString)
    '    End Set
    'End Property
    '
    'für zeitkritische Abfragen
    'Private _Kopier_Vorlage_Long As Long?
    'Public Property Kopier_Vorlage As Long
    '    Get
    '        If Not _Kopier_Vorlage_Long.HasValue Then
    '            Dim [Default] As Long = 0
    '            Dim comment As String = Nothing
    '            _Kopier_Vorlage_Long = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
    '        End If
    '        Return _Kopier_Vorlage_Long.Value
    '    End Get
    '    Set(value As Long)
    '        BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
    '        _Kopier_Vorlage_Long = Nothing
    '    End Set
    'End Property
    '
    '--------------
    '--- SINGLE ---
    '--------------
    '
    'Public Property Kopier_Vorlage As Single
    '    Get
    '        Dim [Default] As Single = 0
    '        Dim comment As String = Nothing
    '        Return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
    '    End Get
    '    Set(value As Single)
    '        BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value.ToString)
    '    End Set
    'End Property

    'Private _Kopier_Vorlage As Single?
    'Public Property Kopier_Vorlage As Single
    '    Get
    '        If Not _Kopier_Vorlage.HasValue Then
    '            Dim [Default] As Single = 0
    '            Dim comment As String = Nothing
    '            _Kopier_Vorlage = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
    '        End If
    '        Return _Kopier_Vorlage.Value
    '    End Get
    '    Set(value As Single)
    '        BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
    '        _Kopier_Vorlage = Nothing
    '    End Set
    'End Property
    '
    '--------------
    '--- DOUBLE ---
    '--------------
    '
    'Public Property Kopier_Vorlage As Double
    '    Get
    '        Dim [Default] As Double = 0
    '        Dim comment As String = Nothing
    '        Return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
    '    End Get
    '    Set(value As Double)
    '        BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value.ToString)
    '    End Set
    'End Property

    'Private _Kopier_Vorlage As Double?
    'Public Property Kopier_Vorlage As Double
    '    Get
    '        If Not _Kopier_Vorlage.HasValue Then
    '            Dim [Default] As Double = 0
    '            Dim comment As String = Nothing
    '            _Kopier_Vorlage = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
    '        End If
    '        Return _Kopier_Vorlage.Value
    '    End Get
    '    Set(value As Double)
    '        BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
    '        _Kopier_Vorlage = Nothing
    '    End Set
    'End Property
    '
    '---------------
    '--- DECIMAL ---
    '---------------
    '
    'Public Property Kopier_Vorlage As Decimal
    '    Get
    '        Dim [Default] As Decimal = 0
    '        Dim comment As String = Nothing
    '        Return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
    '    End Get
    '    Set(value As Decimal)
    '        BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value.ToString)
    '    End Set
    'End Property

    'Private _Kopier_Vorlage As Decimal?
    'Public Property Kopier_Vorlage As Decimal
    '    Get
    '        If Not _Kopier_Vorlage.HasValue Then
    '            Dim [Default] As Decimal = 0
    '            Dim comment As String = Nothing
    '            _Kopier_Vorlage = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
    '        End If
    '        Return _Kopier_Vorlage.Value
    '    End Get
    '    Set(value As Decimal)
    '        BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
    '        _Kopier_Vorlage = Nothing
    '    End Set
    'End Property
    '
    '------------
    '--- DATE ---
    '------------

    'Public Property Kopier_Vorlage As Date
    '    Get
    '        Dim [Default] As Date = Date.MinValue
    '        Dim comment As String = Nothing
    '        Return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
    '    End Get
    '    Set(value As Date)
    '        WriteDate(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
    '    End Set
    'End Property
    '
    '-------------
    '--- COLOR ---
    '-------------
    '
    ''für zeitkritische Abfragen
    'Private _Kopier_Vorlage As Color
    'Public Property Kopier_Vorlage As Color
    '    Get
    '        If _Kopier_Vorlage.IsEmpty Then
    '            Dim [Default] As Color = Color.Black
    '            'alternativ
    '            'Dim [Default] As Color = IniManager.CvtHexStringToColor("FF000000")
    '            Dim comment As String = Nothing
    '            _Kopier_Vorlage = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
    '        End If
    '        Return _Kopier_Vorlage
    '    End Get
    '    Set(value As Color)
    '        BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
    '        _Kopier_Vorlage = Color.Empty
    '    End Set
    'End Property
    '
    ''für nicht zeitkritische Abfragen
    'Public Property Kopier_Vorlage2 As Color
    '    Get
    '        Dim [Default] As Color = Color.Black
    '        'alternativ
    '        'Dim [Default] As Color = IniManager.CvtHexStringToColor("FF000000")
    '        Dim comment As String = Nothing
    '        Return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
    '    End Get
    '    Set(value As Color)
    '        BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
    '    End Set
    'End Property
    '
    '--------------------
    '--- POINT POINTF ---
    '--------------------
    '
    'Public Property Kopier_Vorlage As Point 'Für PointF Point durch PontF ersetzten
    '    Get
    '        Dim [Default] As New Point(100, 100)
    '        Dim comment As String = Nothing
    '        Return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
    '    End Get
    '    Set(value As Point)
    '        BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
    '    End Set
    'End Property
    '
    '------------------
    '--- SIZE SIZEF ---
    '------------------
    '
    'Public Property Kopier_Vorlage As Size 'Für SizeF Size durch SizeF ersetzten.
    '    Get
    '        Dim [Default] As New Size(100,100)
    '        Dim comment As String = Nothing
    '        return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
    '    End Get
    '    Set(value As Size)
    '        BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
    '    End Set
    'End Property
    '
    '----------------------------
    '--- RECTANGLE RECTANGLEF ---
    '----------------------------
    '
    'Public Property Kopier_Vorlage As Rectangle 'Für RectangleF Rectangle durch RectangleF ersetzten.
    '    Get
    '        Dim [Default] As New Rectangle(0,0,100,100)
    '        Dim comment As String = Nothing
    '        Return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
    '    End Get
    '    Set(value As Rectangle)
    '        BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
    '    End Set
    'End Property
    '
    '----------------------------
    '--- PADDINGVALUES ----------
    '----------------------------
    '
    'für zeitkritische Abfragen:
    '
    'Private _Kopier_Vorlage As PaddingValues? ' Cache
    'Public Property Kopier_Vorlage As PaddingValues
    '    Get
    '        If Not _Kopier_Vorlage.HasValue Then
    ''Mögliche Schreibweise in der INI: Left=1,Top=1,Right=2,Bottom=2 oder L=1,T=... oder einfach 1,1,2,2 
    '            Dim [Default] As New PaddingValues(0, 0, 0, 0)
    '            Dim comment As String = Nothing
    '            _Kopier_Vorlage = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
    '        End If
    '        Return _Kopier_Vorlage.Value
    '    End Get
    '    Set(value As PaddingValues)
    '        BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
    '        _Kopier_Vorlage = Nothing   ' Cache ungültig machen
    '    End Set
    'End Property
    '
    'für nicht zeitkritische Abfragen
    'Public Property Kopier_Vorlage As PaddingValues 
    '    Get
    '        Dim [Default] As New PaddingValues(0,0,0,0)
    '        Dim comment As String = Nothing
    '        Return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
    '    End Get
    '    Set(value As PaddingValues)
    '        BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
    '    End Set
    'End Property
    '
    '--------------
    '--- TRIPLE ---
    '--------------
    '
    'für zeitkritische Abfragen:
    '
    'Private _Kopier_Vorlage As Triple? ' Cache
    'Public Property Kopier_Vorlage As Triple
    '    Get
    '        If Not _Kopier_Vorlage.HasValue Then
    '            Dim [Default] As New Triple()
    '            Dim comment As String = Nothing
    '            _Kopier_Vorlage = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
    '        End If
    '        Return _Kopier_Vorlage.Value
    '    End Get
    '    Set(value As Triple)
    '        BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
    '        _Kopier_Vorlage = Nothing   ' Cache ungültig machen
    '    End Set
    'End Property
    '
    'für nicht zeitkritische Abfragen
    'Public Property Kopier_Vorlage As Triple 
    '    Get
    '        Dim [Default] As New Triple()
    '        Dim comment As String = Nothing
    '        Return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
    '    End Get
    '    Set(value As Triple)
    '        BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
    '    End Set
    'End Property
    '
    '------------
    '--- FONT ---
    '------------
    '
    'Public Property Kopier_Vorlage As Font
    '    Get
    '        Dim [Default] As New Font("Arial", 8.25F, FontStyle.Regular)
    '        Dim comment As String = Nothing
    '        Return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
    '    End Get
    '    Set(value As Font)
    '        BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
    '    End Set
    'End Property
    '
    '------------
    '--- ENUM ---
    '------------
    '
    'Public Property Kopier_VorlageBeispiel As SteinSatz
    '    Get
    '        Dim [Default] As String = SteinSatz.InternalSet.ToString
    '        Dim comment As String = Nothing
    '        Dim zRetVal As String = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
    '        Dim result As SteinSatz
    '        If Not [Enum].TryParse(Of SteinSatz)(zRetVal, True, result) Then
    '            result = SteinSatz.InternalSet
    '        End If
    '        Return result
    '    End Get
    '    Set(value As SteinSatz)
    '        BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value.ToString)
    '    End Set
    'End Property

#End Region

#Region "BasisIni"

#Region "Hinweis"

    'Hinweis:
    'Die Reihenfolge beachten.
    'Aus ihr ergibt sich die Reihenfolge der Werte in der INI.
    '
    ''' <summary>
    ''' Dies ist die erste Property und damit der erste Eintrag
    ''' </summary>
    ''' <returns></returns>
    Public Property Hinweis_Bitte_lesen As String
        Get
            Dim [Default] As String = "Ende des Hinweis."
            Dim comment As String = "Sie können hier Programmeinstellungen direkt ändern, bzw. Einstellungen ändern, die sich sonst nirgens ändern lassen." &
                                    "~Sorgfältig arbeiten! Bei Fehlern arbeitet das Programm unvorhersehbar. Nicht bei laufendem Programm ändern." &
                                    "~Sie können diese Ini-Datei einfach löschen. Sie wird dann mit Satz1-Werten neu erzeugt." &
                                    "~Hinweis an Programmierer: Läuft das Programm in der IDE, ist im Hauptformular ganz links unten ein Button ""INI""." &
                                    "~Mit diesem Editor können Sie während der Laufzeit die INI ändern. Ein Teil der Änderungen bedürfen dennoch einen Neustart."

            Return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
        End Get
        Set(value As String)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
        End Set
    End Property

#End Region

#Region "Debughilfen"

    Private _Debug_StopRenderingOnce As Boolean
    ''' <summary>
    ''' Ein Flag, das das Rendern einmalig anhält, wenn es True gesetzt wird.
    ''' </summary>
    ''' <returns></returns>
    Public Property Debug_StopRenderingOnce As Boolean
        Get
            If Not _Debug_StopRenderingOnce Then
                Return False
            Else
                _Debug_StopRenderingOnce = False
                Return True
            End If
        End Get
        Set(value As Boolean)
            _Debug_StopRenderingOnce = value
        End Set
    End Property
    '
    ''' <summary>
    ''' Ein Flag, das das Rendern anhält, wenn es True gesetzt wird.
    ''' </summary>
    ''' <returns></returns>
    Public Property Debug_StopRendering As Boolean
#End Region

#Region "Global"

    Public Property Global_UseSystemDarkMode As Boolean
        Get
            Dim [Default] As Boolean = True

            If Debugger.IsAttached Then
                [Default] = False
            End If

            Dim comment As String = Nothing
            Return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
        End Get
        Set(value As Boolean)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value.ToString)
        End Set
    End Property

    'Für zeitkritische häufige Abfragen
    Private _Global_DarkMode As Boolean?
    Public Property Global_DarkMode As Boolean
        Get
            If IsNothing(_Global_DarkMode) Then
                If Global_UseSystemDarkMode Then
                    _Global_DarkMode = Theme.WindowsTheme.IsAppDarkMode()
                Else
                    Dim [Default] As Boolean = False
                    Dim comment As String = "Darkmode True/False. Nur wirksam, wenn Global_UseSystemDarkMode auf False steht."
                    _Global_DarkMode = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
                End If
            End If
            Return CBool(_Global_DarkMode)
        End Get
        Set(value As Boolean)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value.ToString)
            _Global_DarkMode = Nothing
            _Rendering_ConsumeDoRendering = True
        End Set
    End Property

    ' Globaler Standard-Aufhellungswert (0.0 … 1.0). Praxis: 0.05 … 0.30.
    'Public Property Global_BrightenAmountDefault As Single = 0.4F

    Private _Global_BrightenAmount As Single?
    Public Property Global_BrightenAmount As Single
        Get
            If Not _Global_BrightenAmount.HasValue Then
                Dim [Default] As Single = 0.4F
                Dim comment As String = "Für den Darkmode gibt es nur zum Teil gesonderte Grafiken. Andere werden aufgehellt." &
                                        "~Sinnvolle Werte: 0.2 bis 0.5, Satz1 0.4"
                _Global_BrightenAmount = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
                _Global_BrightenAmount = Math.Max(0.2F, Math.Min(0.6F, CSng(_Global_BrightenAmount)))
            End If
            Return _Global_BrightenAmount.Value
        End Get
        Set(value As Single)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
            _Global_BrightenAmount = Nothing
        End Set
    End Property

    Public Property Global_AktVisibleUserControl As VisibleUserControl
        Get
            Dim [Default] As String = VisibleUserControl.Spielfeld.ToString
            Dim comment As String = "Werden beide vom Programm verwaltet."
            Dim zRetVal As String = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
            Dim result As VisibleUserControl
            If Not [Enum].TryParse(Of VisibleUserControl)(zRetVal, True, result) Then
                result = VisibleUserControl.Spielfeld
            End If
            Return result
        End Get
        Set(value As VisibleUserControl)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value.ToString)
        End Set
    End Property
    Public Property Global_LastVisibleUserControl As VisibleUserControl
        Get
            Dim [Default] As String = VisibleUserControl.Spielfeld.ToString
            Dim comment As String = Nothing
            Dim zRetVal As String = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
            Dim result As VisibleUserControl
            If Not [Enum].TryParse(Of VisibleUserControl)(zRetVal, True, result) Then
                result = VisibleUserControl.Spielfeld
            End If
            Return result
        End Get
        Set(value As VisibleUserControl)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value.ToString)
        End Set
    End Property

#End Region

#Region "Editor"

    Public Event Editor_UsingEditorAllowed_Changed(value As Boolean)
    ''' <summary>
    ''' Gibt an, ob der Anwender den Editor verwenden darf.
    ''' </summary>
    ''' <returns>True, wenn der Editor den Editor verwenden darf.</returns>
    ''' <remarks>Standardmäßig ist der Editor aktiv.</remarks>
    Public Property Editor_UsingEditorAllowed As Boolean
        Get
            Dim [Default] As Boolean = True
            Dim comment As String = "Mit False läßt sich der Editor komplett abschalten. Satz1: True"
            Return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
        End Get
        Set(value As Boolean)
            If BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value) Then
                RaiseEvent Editor_UsingEditorAllowed_Changed(value)
            End If
        End Set
    End Property

    Public Property Editor_UsingEditor As Boolean
        Get
            Dim [Default] As Boolean = True
            Dim comment As String = "Finger weg, wird vom Programm verwaltet."
            Return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
        End Get
        Set(value As Boolean)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
        End Set
    End Property

    Public Property Editor_MessageFont As Font
        Get
            Dim [Default] As New Font("Arial", 10.0F, FontStyle.Regular)
            Dim comment As String = Nothing
            Return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
        End Get
        Set(value As Font)
            _Rendering_ConsumeDoRendering = True
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
        End Set
    End Property

    Public Property Editor_MessageAlpha As Integer
        Get
            Dim [Default] As Integer = 128
            Dim comment As String = "Der Alpha-Wert des Untergrundes der Message. Default 128"
            Dim value As Integer = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)

            If value > 255 Then Return 255
            If value < 0 Then Return 0
            Return value

        End Get
        Set(value As Integer)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value.ToString)
            _Rendering_ConsumeDoRendering = True
        End Set
    End Property

    Public Property Editor_MessageGray As Integer
        Get
            Dim [Default] As Integer = 200
            Dim comment As String = "Der Grau-Wert des Untergrundes der Message. Default: 200"
            Dim value As Integer = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
            If value > 255 Then Return 255
            If value < 0 Then Return 0
            Return value
        End Get
        Set(value As Integer)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value.ToString)
            _Rendering_ConsumeDoRendering = True
        End Set
    End Property

    Private _Editor_SortSpacerWidth As Integer?
    Public Property Editor_SortSpacerWidth As Integer
        Get
            If Not _Editor_SortSpacerWidth.HasValue Then
                Dim [Default] As Integer = 10
                Dim comment As String = "Breite des Zwischenraumes im Steinvorrat ab welchem Stein neu gemischt wird."
                _Editor_SortSpacerWidth = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
                'muss hier das .Value dahinter?
                If _Editor_SortSpacerWidth < 0 Then _Editor_SortSpacerWidth = 0
                If _Editor_SortSpacerWidth > 100 Then _Editor_SortSpacerWidth = 100
            End If
            Return _Editor_SortSpacerWidth.Value
        End Get
        Set(value As Integer)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
            _Editor_SortSpacerWidth = Nothing
        End Set
    End Property

    Private _Editor_SpaceFramesToOpenOrClose As Integer?
    Public Property Editor_SpaceFramesToOpenOrClose As Integer
        Get
            If Not _Editor_SpaceFramesToOpenOrClose.HasValue Then
                Dim [Default] As Integer = 10
                Dim comment As String = "Anzahl der Frames um die Lücke für einen Stein zu öffnen oder zu schließen. Default = 10"
                _Editor_SpaceFramesToOpenOrClose = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
                'muss hier das .Value dahinter?
                If _Editor_SpaceFramesToOpenOrClose < 0 Then _Editor_SpaceFramesToOpenOrClose = 0
                If _Editor_SpaceFramesToOpenOrClose > 100 Then _Editor_SpaceFramesToOpenOrClose = 100
            End If
            Return _Editor_SpaceFramesToOpenOrClose.Value
        End Get
        Set(value As Integer)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
            _Editor_SpaceFramesToOpenOrClose = Nothing
        End Set
    End Property

    Private _Editor_SortMinIndex As Integer?
    Public Property Editor_SortMinIndex As Integer
        Get
            If Not _Editor_SortMinIndex.HasValue Then
                Dim [Default] As Integer = 10
                Dim comment As String = "Index, ab dem die Steine im Vorrat gemischt werden. Default: 10, Maximum 100, Abschalten durch 0."
                _Editor_SortMinIndex = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
                'muss hier das .Value dahinter?
                If _Editor_SortMinIndex < 0 Then _Editor_SortMinIndex = 0
                If _Editor_SortMinIndex > 100 Then _Editor_SortMinIndex = 100
            End If
            Return _Editor_SortMinIndex.Value
        End Get
        Set(value As Integer)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
            _Editor_SortMinIndex = Nothing
        End Set
    End Property

    Private _Editor_HScrollbarColor As Color
    Public Property Editor_HScrollbarColor As Color
        Get
            If _Editor_HScrollbarColor.IsEmpty Then
                Dim [Default] As Color = IniManager.CvtHexStringToColor("FFFCFCDF")
                Dim comment As String = "Default: ""FFFCFCDF"" (ergibt intensive Cremefarbe)"
                _Editor_HScrollbarColor = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
            End If
            Return _Editor_HScrollbarColor
        End Get
        Set(value As Color)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
            _Editor_HScrollbarColor = Color.Empty
            _Rendering_ConsumeDoRendering = True
        End Set
    End Property
    '

    '
    Private _Editor_ShowGrid As Boolean?
    Public Property Editor_ShowGrid As Boolean
        Get
            If IsNothing(_Editor_ShowGrid) Then
                Dim [Default] As Boolean = True
                Dim comment As String = Nothing
                _Editor_ShowGrid = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
            End If
            Return CBool(_Editor_ShowGrid)
        End Get
        Set(value As Boolean)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value.ToString)
            _Editor_ShowGrid = Nothing
        End Set
    End Property

    Private _Editor_GridColorOutside As Color
    Public Property Editor_GridColorOutside As Color
        Get
            If _Editor_GridColorOutside.IsEmpty Then
                Dim [Default] As Color = Color.Black 'IniManager.CvtHexStringToColor("FFFCFCDF")
                Dim comment As String = ""
                _Editor_GridColorOutside = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
            End If
            Return _Editor_GridColorOutside
        End Get
        Set(value As Color)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
            _Editor_GridColorOutside = Color.Empty
            _Rendering_ConsumeDoRendering = True
        End Set
    End Property

    Private _Editor_GridColorInside As Color
    Public Property Editor_GridColorInside As Color
        Get
            If _Editor_GridColorInside.IsEmpty Then
                Dim [Default] As Color = Color.Gray 'IniManager.CvtHexStringToColor("FFFCFCDF")
                Dim comment As String = ""
                _Editor_GridColorInside = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
            End If
            Return _Editor_GridColorInside
        End Get
        Set(value As Color)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
            _Editor_GridColorInside = Color.Empty
            _Rendering_ConsumeDoRendering = True
        End Set
    End Property

    Private _Editor_ShowFrmTooltipSteinInfo As Boolean?
    Public Property Editor_ShowFrmTooltipSteinInfo As Boolean
        Get
            If IsNothing(_Editor_ShowFrmTooltipSteinInfo) Then
                Dim [Default] As Boolean = True
                Dim comment As String = "Das kleine Formular im Editor, das an die Maus gekoppelt ist."
                _Editor_ShowFrmTooltipSteinInfo = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
            End If
            Return CBool(_Editor_ShowFrmTooltipSteinInfo)
        End Get
        Set(value As Boolean)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value.ToString)
            _Editor_ShowFrmTooltipSteinInfo = Nothing
        End Set
    End Property

    Public Property SpielsteinGenerator_VerhältnisNormalsteineZuSondersteine As Double
        Get
            Dim [Default] As Double = 17
            Dim comment As String = "Sondersteine sind die 4 Blumen und die 4 Jahreszeiten." &
                                    "~Hiermit wird gesteuert, auf wieviel Normalsteinpaare ein Sondersteinpaar kommt." &
                                    "~Sollen alle Steine gleichhäufig vorkommen, ist der Wert 17, sollen die Sondersteine" &
                                    "~nur halb so häufig vorkommen ist der Wert 34. Satz1: 17"
            Return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
        End Get
        Set(value As Double)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value.ToString)
        End Set
    End Property

    Public Property SpielsteinGenerator_VorratNoSortAreaEndIndexDefault As Integer
        Get
            Dim [Default] As Integer = 9
            Dim comment As String = "Im Editor läßt sich die Vorratskiste jederzeit neu mischen. Davon ausgenommen sind die Steine bis zum" &
                                    "~hier angegebenem Index. Satz1: 9 (=10 Steine), abschalten mit -1"
            'Rückgabe 
            Return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
        End Get
        Set(value As Integer)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value.ToString)
        End Set
    End Property

    ' Die eENUM durch den Namen der Enumeration ersetzten.
    ' eENUM ist der Name der Enumeration (Kommt vier mal vor)
    ' eENUM.DEFAULT eben der Default (kommt einmal vor)
    '
    Public Property SpielsteinGenerator_GeneratorModusDefault As GeneratorModus
        Get
            Dim [Default] As String = GeneratorModus.StoneSet_144.ToString
            Dim comment As String = Nothing
            Dim zRetVal As String = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
            Dim aRetVal As GeneratorModus = CType(System.Enum.Parse(aRetVal.GetType(), zRetVal), GeneratorModus)
            Return aRetVal
        End Get
        Set(value As GeneratorModus)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value.ToString)
        End Set
    End Property

    Public Property SpielsteinGenerator_VorratMaxUBoundDefault As Integer
        Get
            Dim [Default] As Integer = MJ_STEINE_VORRATMAXDEFAULT
            Dim comment As String = "Die Anzahl der Steine in der Vorratskiste die ""pro Portion"" erzeugt werden. Satz1: " & MJ_STEINE_VORRATMAXDEFAULT.ToString

            'Rückgabe 
            Return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
        End Get
        Set(value As Integer)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value.ToString)
        End Set
    End Property

    Public Property SpielsteinGenerator_VorratNachschubschwelleDefault As Integer
        Get
            Dim [Default] As Integer = MJ_STEINE_VORRATNACHSCHUBSCHWELLEDEFAULT
            Dim comment As String = "Unterschreitet die Anzahl der Steine in der Vorratskiste diesen Wert, wird Nachschub erzeugt. Satz1: " & MJ_STEINE_VORRATNACHSCHUBSCHWELLEDEFAULT.ToString
            'Rückgabe 
            Return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
        End Get
        Set(value As Integer)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value.ToString)
        End Set
    End Property

    Public Property SpielsteinGenerator_DebugMode As Integer
        Get
            Dim [Default] As Integer = 0
            Dim comment As String = "Mit einer bliebigen Zahl <> 0 erzeugt der SteinGenerator immer wieder die gleichen Steinfolgen." &
                                    "~Zum Austesten gedacht. Satz1 = 0 (normaler Spielbetrieb)"
            'Rückgabe 
            Return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
        End Get
        Set(value As Integer)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value.ToString)
        End Set
    End Property

#End Region

#Region "Spielbetrieb"

    ''' <summary>
    ''' Gibt an, ob das Spiel automatisch gespeichert werden soll.
    ''' </summary>
    ''' <returns>True, wenn AutoSave aktiv ist.</returns>
    ''' <remarks>Standardmäßig ist AutoSave aktiv.</remarks>
    Public Property Spielbetrieb_AutoSave As Boolean
        Get
            Dim [Default] As Boolean = True
            Dim comment As String = ""
            Return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
        End Get
        Set(value As Boolean)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
        End Set
    End Property

    Private _Spielbetrieb_WindsAreInOneClickGroup As Boolean?
    Public Property Spielbetrieb_WindsAreInOneClickGroup As Boolean
        Get
            If IsNothing(_Spielbetrieb_WindsAreInOneClickGroup) Then
                Dim [Default] As Boolean = False
                Dim comment As String = "Wenn True ist das eine Spielregelvereinfachung:" &
                                        "~Die 4 Winde können in beliebiger Kombination paarweise entnommen werden."
                _Spielbetrieb_WindsAreInOneClickGroup = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
            End If
            Return CBool(_Spielbetrieb_WindsAreInOneClickGroup)
        End Get
        Set(value As Boolean)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value.ToString)
            _Spielbetrieb_WindsAreInOneClickGroup = Nothing
        End Set
    End Property

    Private _Spielbetrieb_ShowSelectableStones As Boolean?
    Public Property Spielbetrieb_ShowSelectableStones As Boolean
        Get
            If IsNothing(_Spielbetrieb_ShowSelectableStones) Then
                Dim [Default] As Boolean = True
                Dim comment As String = "Wenn True werden alle selektierbaren Steine in anderer Farbe dargestellt."
                _Spielbetrieb_ShowSelectableStones = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
            End If
            Return CBool(_Spielbetrieb_ShowSelectableStones)
        End Get
        Set(value As Boolean)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value.ToString)
            _Spielbetrieb_ShowSelectableStones = Nothing
        End Set
    End Property

    Private _Spielbetrieb_ShowRemovableStones As Boolean?
    Public Property Spielbetrieb_ShowRemovableStones As Boolean
        Get
            If IsNothing(_Spielbetrieb_ShowRemovableStones) Then
                Dim [Default] As Boolean = True
                Dim comment As String = "Wenn True werden alle entnehmbaren Steine in anderer Farbe dargestellt."
                _Spielbetrieb_ShowRemovableStones = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
            End If
            Return CBool(_Spielbetrieb_ShowRemovableStones)
        End Get
        Set(value As Boolean)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value.ToString)
            _Spielbetrieb_ShowRemovableStones = Nothing
        End Set
    End Property

    Private _Spielbetrieb_PositionHistory As Integer?
    Public Property Spielbetrieb_PositionHistory As Integer
        Get
            If Not _Spielbetrieb_PositionHistory.HasValue Then
                Dim [Default] As Integer = 0
                Dim comment As String = "0 = ohne sichtbare History, 1 = kleine Historybox links unten, 2 = links vom Spielfeld, 3 = rechts vom Spielfeld. Default:  = 1"
                _Spielbetrieb_PositionHistory = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
            End If
            Return _Spielbetrieb_PositionHistory.Value
        End Get
        Set(value As Integer)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
            _Spielbetrieb_PositionHistory = Nothing
        End Set
    End Property

    Private _Spielbetrieb_UseUnDoReDoSpielfeld As Boolean?
    Public Property Spielbetrieb_UseUnDoReDoSpielfeld As Boolean
        Get
            If IsNothing(_Spielbetrieb_UseUnDoReDoSpielfeld) Then
                Dim [Default] As Boolean = True
                Dim comment As String = "Die Buttons auf dem Spielfeld einschalten"
                _Spielbetrieb_UseUnDoReDoSpielfeld = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
            End If
            Return CBool(_Spielbetrieb_UseUnDoReDoSpielfeld)
        End Get
        Set(value As Boolean)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
            _Spielbetrieb_UseUnDoReDoSpielfeld = Nothing
        End Set
    End Property

#End Region

#Region "BackGroundImage"

    Private _BackGroundImage_MaxDistortionRatio As Double?
    Public Property BackGroundImage_MaxDistortionRatio As Double
        Get
            If Not _BackGroundImage_MaxDistortionRatio.HasValue Then
                Dim [Default] As Double = 0.1
                Dim comment As String = "Ab dieser Abweichung des erforderlichen Breiten/Höhen-Verhältnisses wird das Bild nicht mehr gestaucht," &
                                        "~sondern in einen Untergrund mit der dominanten Hauptfarbe des Bildes kopiert. Satz1: 0.1, gültig: 0 bis 3"
                _BackGroundImage_MaxDistortionRatio = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
            End If
            Return Math.Max(0, Math.Min(_BackGroundImage_MaxDistortionRatio.Value, 3))
        End Get
        Set(value As Double)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
            _BackGroundImage_MaxDistortionRatio = Nothing
        End Set
    End Property

    Private _BackGroundImage_ColorSampleStep As Integer?
    Public Property BackGroundImage_ColorSampleStep As Integer
        Get
            If Not _BackGroundImage_ColorSampleStep.HasValue Then
                Dim [Default] As Integer = 30
                Dim comment As String = "Schrittweite in Pixeln bei der Berechnung der dominanten Hauptfarbe. Satz1: 10, erlaubt 1 bis 30"
                _BackGroundImage_ColorSampleStep = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
                _BackGroundImage_ColorSampleStep = Math.Max(1, Math.Min(30, _BackGroundImage_ColorSampleStep.Value))
            End If
            Return _BackGroundImage_ColorSampleStep.Value
        End Get
        Set(value As Integer)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
            _BackGroundImage_ColorSampleStep = Nothing
        End Set
    End Property

    'Tipps für die INI-Defaults
    'BackgroundImage_minSaturation = 0.10 (unter 10 % gilt „grau/entsättigt“)
    'BackgroundImage_minWeightForGray = 0.30 (graue/helle Bereiche zählen min. 30 %)
    'BackgroundImage_luminanceBoost = 0.50 (helle Pixel moderat bevorzugen)
    'Wenn die Rückgabefarbe immer noch etwas zu dunkel ist, erhöhe zuerst
    'BackgroundImage_minWeightForGray (z. B. 0.45) oder den luminanceBoost (z. B. 0.8).
    'Bei sehr „pastelligen“ Motiven kann auch minSaturation leicht runter (z. B. 0.07),
    'damit Pastellfarben nicht vorschnell als „grau“ gelten.

    Private _BackGroundImage_MinSaturation As Double?
    Public Property BackGroundImage_MinSaturation As Double
        Get
            If Not _BackGroundImage_MinSaturation.HasValue Then
                Dim [Default] As Double = 0.1
                Dim comment As String = "Stellschrauben zur Berechnung der Hintegrundfarbe, wenn die Bilder nicht gedehnt werden und kein Auschnitt verwendet wird." &
                                        "~Pixel unterhalb dieser Sättigung gelten als ""entsättigt"" und werden nicht auf 0 abgewertet, sondern auf einen Mindestanteil." &
                                        "~Gültige Werte: 0 bis 1, Satz1 0.1"

                _BackGroundImage_MinSaturation = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
            End If
            Return Math.Max(0, Math.Min(_BackGroundImage_MinSaturation.Value, 3))
        End Get
        Set(value As Double)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
            _BackGroundImage_MinSaturation = Nothing
        End Set
    End Property

    Private _BackGroundImage_MinWeightForGray As Double?
    Public Property BackGroundImage_MinWeightForGray As Double
        Get
            If Not _BackGroundImage_MinWeightForGray.HasValue Then
                Dim [Default] As Double = 0.3
                Dim comment As String = "Mindestgewicht für graue/entsättigte Pixel (z. B. 0.3 = 30 %), damit helle Graubereiche nicht ""verschwinden""." &
                                        "~"
                _BackGroundImage_MinWeightForGray = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
            End If
            Return Math.Max(0, Math.Min(_BackGroundImage_MinWeightForGray.Value, 3))
        End Get
        Set(value As Double)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
            _BackGroundImage_MinWeightForGray = Nothing
        End Set
    End Property

    Private _BackGroundImage_LuminanceBoost As Double?
    Public Property BackGroundImage_LuminanceBoost As Double
        Get
            If Not _BackGroundImage_LuminanceBoost.HasValue Then
                Dim [Default] As Double = 0.5
                Dim comment As String = "Verstärkt die Gewichtung hellerer Pixel. 0 = aus. Typisch 0.3–0.8. Satz1: 0.5"
                _BackGroundImage_LuminanceBoost = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
            End If
            Return Math.Max(0, Math.Min(_BackGroundImage_LuminanceBoost.Value, 3))
        End Get
        Set(value As Double)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
            _BackGroundImage_LuminanceBoost = Nothing
        End Set
    End Property

    Private _BackGroundImage_Brighten As Double?
    Public Property BackGroundImage_Brighten As Double
        Get
            If Not _BackGroundImage_Brighten.HasValue Then
                Dim [Default] As Double = 1
                Dim comment As String = "Aufhellung des Endergebnisses. Erlaubt: 0.1 (Abdunklung) über 1 (keine Aufhellung) bis 2. Satz1: 1"
                _BackGroundImage_Brighten = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
            End If
            Return Math.Max(0.1, Math.Min(_BackGroundImage_Brighten.Value, 2))
        End Get
        Set(value As Double)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
            _BackGroundImage_Brighten = Nothing
        End Set
    End Property

    Private _BackGroundImage_HeightQuality As Boolean?
    Public Property BackGroundImage_HeightQuality As Boolean
        Get
            If IsNothing(_BackGroundImage_HeightQuality) Then
                Dim [Default] As Boolean = False
                Dim comment As String = "Auf langsamen Rechnern False setzten. Satz1: True"
                _BackGroundImage_HeightQuality = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
            End If
            Return CBool(_BackGroundImage_HeightQuality)
        End Get
        Set(value As Boolean)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
            _BackGroundImage_HeightQuality = Nothing
        End Set
    End Property

#End Region

#Region "InfoMessageBox"

    Public Property InfoMessageBox_FontHeader As Font
        Get
            Dim [Default] As New Font("Segoe UI", 12.0F, FontStyle.Bold)
            Dim comment As String = "Andere serifenlose Standardschriften: Arial, Segoe UI, Calibri, Tahoma, Verdana, sans-serif. Satz1 Segoe UI;12;Bold"
            Return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
        End Get
        Set(value As Font)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
        End Set
    End Property

    Public Property InfoMessageBox_FontMessage As Font
        Get
            Dim [Default] As New Font("Segoe UI", 12.0F, FontStyle.Regular)
            Dim comment As String = Nothing
            Return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
        End Get
        Set(value As Font)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
        End Set
    End Property

    Public Property InfoMessageBox_FontHeaderMonoSpaced As Font
        Get
            Dim [Default] As New Font("Cascadia Mono", 12.0F, FontStyle.Bold)
            Dim comment As String = "Andere diktengleiche Fonts: Cascadia Mono, Consolas, Lucida Console, Courier New, monospace. Satz1: Cascadia Mono;12;Bold"
            Return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
        End Get
        Set(value As Font)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
        End Set
    End Property

    Public Property InfoMessageBox_FontMessageMonoSpaced As Font
        Get
            Dim [Default] As New Font("Cascadia Mono", 12.0F, FontStyle.Regular)
            Dim comment As String = Nothing
            Return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
        End Get
        Set(value As Font)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
        End Set
    End Property

    Public Property InfoMessageBox_FormBackColor As Color
        Get
            Dim [Default] As Color = Color.Silver
            'alternativ
            'Dim [Default] As Color = IniManager.CvtHexStringToColor("FF000000")
            Dim comment As String = Nothing
            Return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
        End Get
        Set(value As Color)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
        End Set
    End Property
    Public Property InfoMessageBox_TextBackColor As Color
        Get
            Dim [Default] As Color = Color.Beige
            'alternativ
            'Dim [Default] As Color = IniManager.CvtHexStringToColor("FF000000")
            Dim comment As String = Nothing
            Return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
        End Get
        Set(value As Color)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
        End Set
    End Property

#End Region

#Region "Sonstiges"

    Public Property Sonstiges_ScreenshotMaxCount As Integer
        Get
            Dim [Default] As Integer = 20
            Dim comment As String = "Screenshots werden, mit einem Zeitstempel versehen, gespeichert im Verzeichnis:~" &
                                    AppDataFullPath(AppDataSubDir.Diverses, AppDataSubSubDir.Diverses_ScreenShots) &
                                    "~Sind dort mehr als hier angegeben, werden die Ältesten beim nächsten ScreenShot gelöscht. Satz1 = 20"

            Return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
        End Get
        Set(value As Integer)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value.ToString)
        End Set
    End Property

    Public Property Sonstiges_ShowToolTips As Boolean
        Get
            Dim [Default] As Boolean = True
            Dim comment As String = Nothing
            Return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
        End Get
        Set(value As Boolean)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value.ToString)
        End Set
    End Property

    Public Property Sonstiges_AppGrafikSatz As AppGrafikSatz
        Get
            Dim [Default] As String = AppGrafikSatz.Satz1.ToString
            Dim comment As String = "Das Programm ist vorgesehen für die Verwendung beliebiger und beliebig vieler Sätze an 16x16 Grafiken" &
                                    "~für die verschiedenen Buttons im Programm. Die Programmlogik ist noch nicht implementiert und es mangelt" &
                                    "~an Grafiken. Derzeit ist nur der Satz1 möglich: ""Satz1"" (Wenn implementiert, ändert sich dieser Text hier!)" &
                                    "~Wenn jemand Lust hat die Grafiken beizusteuern: MahjongGK@t-online.de"
            Dim zRetVal As String = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
            Dim result As AppGrafikSatz
            If Not [Enum].TryParse(Of AppGrafikSatz)(zRetVal, True, result) Then
                result = AppGrafikSatz.Satz1
            End If
            Return result
        End Get
        Set(value As AppGrafikSatz)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value.ToString)
        End Set
    End Property
    Public Property Sonstiges_FrmMainStartupPosition As Rectangle
        Get
            Dim [Default] As New Rectangle
            Dim comment As String = Nothing
            Return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
        End Get
        Set(value As Rectangle)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
        End Set
    End Property

    Public Property Sonstiges_FrmMainStartMaximized As Boolean
        Get
            Dim [Default] As Boolean = False
            Dim comment As String = "Der WindowsState beim Programmende, der beim nächsten Start wiederhergestellt wird."
            Return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
        End Get
        Set(value As Boolean)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value.ToString)
        End Set
    End Property
#End Region

#Region "IfRunningInIDE_... Properties"

    Private _IfRunningInIDE_ShowAllStones As Boolean?
    ''' <summary>
    ''' Gibt außerhalb der IDE immer False zurück
    ''' </summary>
    ''' <returns></returns>
    Public Property IfRunningInIDE_ShowAllStones As Boolean
        Get

            If IsNothing(_IfRunningInIDE_ShowAllStones) Then
                Dim [Default] As Boolean = False
                Dim comment As String = Nothing
                _IfRunningInIDE_ShowAllStones = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
            End If
            'musss hier stehen, sonst wird der Wert nicht initialisiert und kann in der INI manuell nicht geändert werden.
            If Not Debugger.IsAttached() Then
                Return False
            End If

            Return CBool(_IfRunningInIDE_ShowAllStones)
        End Get
        Set(value As Boolean)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
            _IfRunningInIDE_ShowAllStones = Nothing
        End Set
    End Property

    Private _IfRunningInIDE_InsertStoneIndex As Boolean?
    ''' <summary>
    ''' Gibt außerhalb der IDE immer False zurück
    ''' </summary>
    ''' <returns></returns>
    Public Property IfRunningInIDE_ShowErrorMsgInsteadOfException As Boolean
        Get
            Dim [Default] As Boolean = False
            Dim comment As String = Nothing
            Dim retval As Boolean = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
            'muss hier stehen, sonst wird der Wert nicht initialisiert und kann in der INI manuell nicht geändert werden.
            If Not Debugger.IsAttached() Then
                Return False
            Else
                Return retval
            End If
        End Get
        Set(value As Boolean)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
        End Set
    End Property

    Public Property IfRunningInIDE_IniEditorDarkmode As Boolean
        Get
            Dim [Default] As Boolean = True
            Dim comment As String = Nothing
            Return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
        End Get
        Set(value As Boolean)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value.ToString)
        End Set
    End Property

    Public Property IfRunningInIDE_Grafik16x16Directory_Ressourcen As String
        Get
            Dim [Default] As String = "C:\Users\goetz\Documents\Visual Studio\MahjongGK\Grafiken\Grafiken16x16_Ressourcen"
            Dim comment As String = "Nur in der IDE sichtbar gibt es in FrmMain unten zwei Buttons ""DwnLd"" und ""GfxCompiler"", die" &
                                    "~den Windows Dateiexplorer mit dem Dowwnloadverzeichniss und dem Grafikverzeichniss öffnen." &
                                    "~Hier ist der lokale Pfad auf Ihrem Rechner einzugeben. Satz1: der Pfad auf meinem Rechner."

            Return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
        End Get
        Set(value As String)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
        End Set
    End Property
    Public Property IfRunningInIDE_Grafik16x16Directory_Other As String
        Get
            Dim [Default] As String = "C:\Users\goetz\Documents\Visual Studio\MahjongGK\Grafiken\Grafiken16x16_Andere"
            Dim comment As String = "Nur in der IDE sichtbar gibt es in FrmMain unten zwei Buttons ""DwnLd"" und ""GfxCompiler"", die" &
                                    "~den Windows Dateiexplorer mit dem Dowwnloadverzeichniss und dem Grafikverzeichniss öffnen." &
                                    "~Hier ist der lokale Pfad auf Ihrem Rechner einzugeben. Satz1: der Pfad auf meinem Rechner."

            Return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
        End Get
        Set(value As String)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
        End Set
    End Property

    Public Property IfRunningInIDE_DownloadDirectory As String
        Get
            Dim [Default] As String = "C:\Users\goetz\Downloads\Vivaldi"
            Dim comment As String = "Gleiches Spielfeld mit dem Downloadverzeichnis." &
                                      "~Hinweis: Backslashes müssen verdoppelt werden, da sie Escape-Zeichen sind."
            Return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
        End Get
        Set(value As String)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
        End Set
    End Property

#End Region

#Region "GfxCompiler"

    'für nicht zeitkritische Abfragen
    Public Property GfxCompiler_GhostUGrdColor As Color
        Get
            Dim [Default] As Color = IniManager.CvtHexStringToColor("FFC0C0C0")
            Dim comment As String = Nothing
            Return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
        End Get
        Set(value As Color)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
        End Set
    End Property
    Public Property GfxCompiler_GhostBasisColor(steinStatus As SteinStatus) As Color
        Get
            Dim [Default] As Color
            Select Case steinStatus
                Case SteinStatus.I00Unsichtbar
                    [Default] = IniManager.CvtHexStringToColor("FFC0C0C0")
                Case SteinStatus.I01Normal
                    [Default] = IniManager.CvtHexStringToColor("FFFCFCDF")
                Case SteinStatus.I02Selected
                    [Default] = IniManager.CvtHexStringToColor("FFFFFF00")
                Case SteinStatus.I03Selectable
                    [Default] = IniManager.CvtHexStringToColor("FFCAD9FF")
                Case SteinStatus.I04Removable
                    [Default] = IniManager.CvtHexStringToColor("FFFFFFFF")
                Case SteinStatus.I05Locked
                    [Default] = IniManager.CvtHexStringToColor("FFD8D8D8")
                Case SteinStatus.I06WerkstückStein
                    [Default] = IniManager.CvtHexStringToColor("FF000000")
                Case SteinStatus.I07MissingSecond
                    [Default] = IniManager.CvtHexStringToColor("FFFFBF00")
                Case SteinStatus.I08WerkstückEinfügeFehler
                    [Default] = IniManager.CvtHexStringToColor("FFFF0000")
                Case SteinStatus.I09WerkstückZufallsgrafik
                    [Default] = IniManager.CvtHexStringToColor("FF000000")
                Case SteinStatus.I10Reserve1
                    [Default] = IniManager.CvtHexStringToColor("FF00FF00")
                Case SteinStatus.I11Reserve2
                    [Default] = IniManager.CvtHexStringToColor("FFFF00FF")

            End Select
            Dim comment As String = Nothing
            Return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name, steinStatus.ToString), [Default], comment)
        End Get
        Set(value As Color)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name, steinStatus.ToString), value)
        End Set
    End Property

    Public Property GfxCompiler_GhostTransparenzUnsichtbar As Byte
        Get
            Dim [Default] As Byte = 200 '
            Dim comment As String = Nothing
            Return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
        End Get
        Set(value As Byte)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value.ToString)
        End Set
    End Property
    Public Property GfxCompiler_GhostTransparenzAndere As Byte
        Get
            Dim [Default] As Byte = 200 '
            Dim comment As String = Nothing
            Return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
        End Get
        Set(value As Byte)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value.ToString)
        End Set
    End Property
    Public Property GfxCompiler_GhostWitdth As Integer
        Get
            Dim [Default] As Integer = 200
            Dim comment As String = Nothing
            Return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
        End Get
        Set(value As Integer)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value.ToString)
        End Set
    End Property
    Public Property GfxCompiler_GhostHeight As Integer
        Get
            Dim [Default] As Integer = 250
            Dim comment As String = Nothing
            Return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
        End Get
        Set(value As Integer)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value.ToString)
        End Set
    End Property

    Public Property GfxCompiler_GhostCurvatureFaktor As Double
        Get
            Dim [Default] As Double = 0.06
            Dim comment As String = Nothing
            Return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
        End Get
        Set(value As Double)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value.ToString)
        End Set
    End Property

    Public Property GfxCompiler_GhostShadowRightFaktor As Double
        Get
            Dim [Default] As Double = 0.05
            Dim comment As String = Nothing
            Return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
        End Get
        Set(value As Double)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value.ToString)
        End Set
    End Property
    Public Property GfxCompiler_GhostShadowBottomFaktor As Double
        Get
            Dim [Default] As Double = 0.05
            Dim comment As String = Nothing
            Return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
        End Get
        Set(value As Double)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value.ToString)
        End Set
    End Property
    Public Property GfxCompiler_GhostPaddingFaktor As Double
        Get
            Dim [Default] As Double = 0.05
            Dim comment As String = Nothing
            Return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
        End Get
        Set(value As Double)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value.ToString)
        End Set
    End Property

    Public Property GfxCompiler_GhostFrameStrokeWidthFaktor As Double
        Get
            Dim [Default] As Double = 0.02
            Dim comment As String = Nothing
            Return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
        End Get
        Set(value As Double)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value.ToString)
        End Set
    End Property

#End Region

#End Region

#Region "ToolboxINI"

#Region "ColorPickerHSB"

    Public Property ColorPickerHSB_Toolbox_PickerBackColor As Color
        Get
            Dim [Default] As Color = Color.Empty
            'alternativ
            'Dim [Default] As Color = IniManager.CvtHexStringToColor("FF000000")
            Dim comment As String = Nothing
            Return ToolBoxIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
        End Get
        Set(value As Color)
            ToolBoxIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
        End Set
    End Property

    Public Property ColorPickerHSB_Toolbox_SavedColorsString As String
        Get
            Dim [Default] As String = Nothing
            Dim comment As String = Nothing
            Return ToolBoxIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
        End Get
        Set(value As String)
            ToolBoxIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
        End Set
    End Property

#End Region

#Region "Toolbox"
    Private _ToolBox_FormToolBox As Form
    Public Property ToolBox_FormToolBox As Form
        Get
            Return _ToolBox_FormToolBox
        End Get
        Set(value As Form)
            _ToolBox_FormToolBox = value
            If value Is Nothing Then
                _ToolBox_FormIsVisible = False
            End If
        End Set
    End Property

    Private _ToolBox_FormIsVisible As Boolean
    Public Property ToolBox_FormIsVisible As Boolean
        Get
            If ToolBox_FormToolBox Is Nothing Then
                _ToolBox_FormIsVisible = False
            End If
            Return _ToolBox_FormIsVisible
        End Get
        Set(value As Boolean)
            _ToolBox_FormIsVisible = value
        End Set
    End Property
    Private _ToolBox_TabPageChangedConsume As Boolean

    Public Property ToolBox_ConsumeTabPageChanged As Boolean
        Get
            If _ToolBox_TabPageChangedConsume Then
                _ToolBox_TabPageChangedConsume = False
                Return True
            Else
                Return False
            End If
        End Get
        Set(value As Boolean)
            _ToolBox_TabPageChangedConsume = value
        End Set
    End Property
    '
    ''' <summary>
    ''' Speicherung der aktuellen Position der Form Toolbox
    ''' </summary>
    ''' <returns></returns>
    Public Property ToolBox_Rectangle As Rectangle
        Get
            Dim [Default] As New Rectangle(100, 100, frmToolBox.MJ_FRMTOOLBOX_WIDTH, frmToolBox.MJ_FRMTOOLBOX_HEIGHT) '
            Dim comment As String = Nothing
            Dim rc As Rectangle = ToolBoxIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)

            ' --- Größe absichern 
            rc.Width = frmToolBox.MJ_FRMTOOLBOX_WIDTH
            rc.Height = frmToolBox.MJ_FRMTOOLBOX_HEIGHT

            ' Arbeitsbereich des Monitors ermitteln, der dem Rechteck am nächsten ist
            Dim wa As Rectangle = Screen.FromRectangle(rc).WorkingArea

            ' Falls zu groß: auf Arbeitsbereich kappen
            If rc.Width > wa.Width Then rc.Width = wa.Width
            If rc.Height > wa.Height Then rc.Height = wa.Height

            ' Position so klemmen, dass das Rechteck komplett innerhalb des Arbeitsbereichs liegt
            If rc.Right > wa.Right Then rc.X = wa.Right - rc.Width
            If rc.Bottom > wa.Bottom Then rc.Y = wa.Bottom - rc.Height
            If rc.X < wa.Left Then rc.X = wa.Left
            If rc.Y < wa.Top Then rc.Y = wa.Top

            Return rc
        End Get
        Set(value As Rectangle)
            ToolBoxIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
        End Set
    End Property

    Public Property ToolBox_FeldSizeXmax() As Integer
        Get
            Dim [Default] As Integer = 30
            Dim comment As String = $"Maximale Anzahl der Steine nebeneinander. Gültig: 1 bis {MJ_STEINE_MAXX}, Satz1 = 30"
            Dim value As Integer = ToolBoxIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
            Return Math.Max(1, Math.Min(MJ_STEINE_MAXX, value))
        End Get
        Set(value As Integer)
            ToolBoxIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value.ToString)
        End Set
    End Property
    Public Property ToolBox_FeldSizeYmax() As Integer
        Get
            Dim [Default] As Integer = 15
            Dim comment As String = $"Maximale Anzahl der Steine übereinander. Gültig: 1 bis {MJ_STEINE_MAXY}, Satz1 = 15"
            Dim value As Integer = ToolBoxIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
            Return Math.Max(1, Math.Min(MJ_STEINE_MAXY, value))
        End Get
        Set(value As Integer)
            ToolBoxIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value.ToString)
        End Set
    End Property
    Public Property ToolBox_FeldSizeZmax() As Integer
        Get
            Dim [Default] As Integer = 10
            Dim comment As String = $"Maximale Anzahl der Steine aufeinander. Gültig: 1 bis {MJ_STEINE_MAXZ}, Satz1 = 10"
            Dim value As Integer = ToolBoxIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
            Return Math.Max(1, Math.Min(MJ_STEINE_MAXZ, value))
        End Get
        Set(value As Integer)
            ToolBoxIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value.ToString)
        End Set
    End Property
    Public Property ToolBox_FeldSizeX(bform As BasisformEnum) As Integer
        Get
            Dim [Default] As Integer = 10
            Dim comment As String = Nothing
            If bform = 0 Then
                comment = "Die jeweiligen aktuellen Werte der Basisformen in der Toolbox. Satz1 für X = 10, Y = 10, Z = 5"
            End If
            Dim value As Integer = ToolBoxIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name, bform.ToString), [Default], comment)
            Return Math.Max(1, Math.Min(MJ_STEINE_MAXX, value))
        End Get
        Set(value As Integer)
            ToolBoxIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name, bform.ToString), value.ToString)
        End Set
    End Property
    Public Property ToolBox_FeldSizeY(bform As BasisformEnum) As Integer
        Get
            Dim [Default] As Integer = 10
            Dim comment As String = Nothing
            Dim value As Integer = ToolBoxIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name, bform.ToString), [Default], comment)
            Return Math.Max(1, Math.Min(MJ_STEINE_MAXX, value))
        End Get
        Set(value As Integer)
            ToolBoxIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name, bform.ToString), value.ToString)
        End Set
    End Property
    Public Property ToolBox_FeldSizeZ(bform As BasisformEnum) As Integer
        Get
            Dim [Default] As Integer = 5
            Dim comment As String = Nothing
            Dim value As Integer = ToolBoxIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name, bform.ToString), [Default], comment)
            Return Math.Max(1, Math.Min(MJ_STEINE_MAXZ, value))
        End Get
        Set(value As Integer)
            ToolBoxIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name, bform.ToString), value.ToString)
        End Set
    End Property
    Public Property ToolBox_FeldSizeDefaultX(bform As BasisformEnum) As Integer
        Get
            Dim [Default] As Integer = 10
            Select Case bform
                Case BasisformEnum.Kegel
                    [Default] = 5
                Case BasisformEnum.Kreis
                    [Default] = 5
                Case BasisformEnum.Linie
                    [Default] = 8
                Case BasisformEnum.Pyramide
                    [Default] = 5
                Case BasisformEnum.Rechteck
                    [Default] = 4
                Case BasisformEnum.UForm
                    [Default] = 4
                Case BasisformEnum.Winkel
                    [Default] = 5
                Case BasisformEnum.Zufall
                    [Default] = 6
            End Select
            Dim comment As String = Nothing
            If bform = 0 Then
                comment = "Die jeweiligen aktuellen Werte der Basisformen in der Toolbox. Satz1 für X = 10, Y = 10, Z = 5"
            End If
            Dim value As Integer = ToolBoxIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name, bform.ToString), [Default], comment)
            Return Math.Max(1, Math.Min(MJ_STEINE_MAXX, value))
        End Get
        Set(value As Integer)
            ToolBoxIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name, bform.ToString), value.ToString)
        End Set
    End Property
    Public Property ToolBox_FeldSizeDefauktY(bform As BasisformEnum) As Integer
        Get
            Dim [Default] As Integer = 10
            Select Case bform
                Case BasisformEnum.Kegel
                    [Default] = 5
                Case BasisformEnum.Kreis
                    [Default] = 5
                Case BasisformEnum.Linie
                    [Default] = 8
                Case BasisformEnum.Pyramide
                    [Default] = 5
                Case BasisformEnum.Rechteck
                    [Default] = 6
                Case BasisformEnum.UForm
                    [Default] = 4
                Case BasisformEnum.Winkel
                    [Default] = 6
                Case BasisformEnum.Zufall
                    [Default] = 6
            End Select
            Dim comment As String = Nothing
            Dim value As Integer = ToolBoxIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name, bform.ToString), [Default], comment)
            Return Math.Max(1, Math.Min(MJ_STEINE_MAXX, value))
        End Get
        Set(value As Integer)
            ToolBoxIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name, bform.ToString), value.ToString)
        End Set
    End Property
    Public Property ToolBox_FeldSizeDefauktZ(bform As BasisformEnum) As Integer
        Get
            Dim [Default] As Integer = 5
            Select Case bform
                Case BasisformEnum.Kegel
                    [Default] = 4
                Case BasisformEnum.Kreis
                    [Default] = 1
                Case BasisformEnum.Linie
                    [Default] = 3
                Case BasisformEnum.Pyramide
                    [Default] = 5
                Case BasisformEnum.Rechteck
                    [Default] = 1
                Case BasisformEnum.UForm
                    [Default] = 1
                Case BasisformEnum.Winkel
                    [Default] = 1
                Case BasisformEnum.Zufall
                    [Default] = 4
            End Select

            Dim comment As String = Nothing
            Dim value As Integer = ToolBoxIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name, bform.ToString), [Default], comment)
            Return Math.Max(1, Math.Min(MJ_STEINE_MAXZ, value))
        End Get
        Set(value As Integer)
            ToolBoxIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name, bform.ToString), value.ToString)
        End Set
    End Property

    Public Property ToolBox_NameBasisformForSaving(bform As BasisformEnum) As String
        Get
            Dim [Default] As String = bform.ToString & "_1"
            Dim comment As String = Nothing
            Return ToolBoxIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name, bform.ToString), [Default], comment)
        End Get
        Set(value As String)
            ToolBoxIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name, bform.ToString), value.ToString)
        End Set
    End Property

    Public Property ToolBox_AktBasisform As BasisformEnum
        Get
            Dim [Default] As String = BasisformEnum.Linie.ToString
            Dim comment As String = Nothing
            Dim zRetVal As String = ToolBoxIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
            Dim result As BasisformEnum
            If Not [Enum].TryParse(Of BasisformEnum)(zRetVal, True, result) Then
                result = BasisformEnum.Linie
            End If
            Return result
        End Get
        Set(value As BasisformEnum)
            ToolBoxIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value.ToString)
        End Set
    End Property

    Public Property Toolbox_HGrdSplFldColorFallback As Color
        Get
            Dim [Default] As Color = Color.Empty
            Dim comment As String = Nothing
            Dim col As Color = ToolBoxIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
            If col = Color.Empty Then
                Return Rendering_BackgroundColor
            Else
                Return col
            End If
        End Get
        Set(value As Color)
            ToolBoxIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
        End Set
    End Property

    Public Property Toolbox_HGrdSplFldBitmapNameFallback As String
        Get
            Dim [Default] As String = "wallpaperInv-2070678.jpg" 'Soll
            Dim comment As String = "Default: wallpaperInv-2070678.jpg"
            Return ToolBoxIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
        End Get
        Set(value As String)
            ToolBoxIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
        End Set
    End Property

    Public Property Toolbox_HGrdSplFldBitmapIsUserGrafikFallback As Boolean
        Get
            Dim [Default] As Boolean = False
            Dim comment As String = Nothing
            Return ToolBoxIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
        End Get
        Set(value As Boolean)
            ToolBoxIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value.ToString)
        End Set
    End Property

    Public Property Toolbox_HGrdSplFldRenderModeFallback As Images.BackgroundImageRenderMode
        Get
            Dim [Default] As String = Images.BackgroundImageRenderMode.None.ToString
            Dim comment As String = Nothing
            Dim zRetVal As String = ToolBoxIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
            Dim result As Images.BackgroundImageRenderMode
            If Not [Enum].TryParse(Of Images.BackgroundImageRenderMode)(zRetVal, True, result) Then
                result = Images.BackgroundImageRenderMode.Stretch
            End If
            Return result
        End Get
        Set(value As Images.BackgroundImageRenderMode)
            ToolBoxIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value.ToString)
        End Set
    End Property

#End Region

#End Region

#Region "Rendering.ini"

    Private _Rendering_RenderTimerIntervalWorking As Integer?
    Public Property Rendering_RenderTimerIntervalWorking As Integer
        Get
            If IsNothing(_Rendering_RenderTimerIntervalWorking) Then
                Dim [Default] As Integer = 15
                Dim comment As String = "I01Normal 15 bis 20 (Einheit Millisekunden). Werte über 30 für schwache Rechner," &
                                    "~= 1 führt zu einem stabilerem Takt aller Timer auf dem Computer und zu etwas höherem Energieverbrauch." &
                                    "~Zu hohe Werte verlangsamen und verlängern die Animation. Satz1: 15"
                _Rendering_RenderTimerIntervalWorking = Rendering.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
            End If

            Return _Rendering_RenderTimerIntervalWorking.Value
        End Get
        Set(value As Integer)
            Rendering.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value.ToString)
            _Rendering_RenderTimerIntervalWorking = Nothing
        End Set
    End Property

    Private _Rendering_RenderTimerIntervalPaused As Integer?
    Public Property Rendering_RenderTimerIntervalPaused As Integer
        Get
            If IsNothing(_Rendering_RenderTimerIntervalPaused) Then
                Dim [Default] As Integer = 500
                Dim comment As String = "Das Programm rendert nur, wenn sich etwas geändert hat. Wenn nicht, wird die letzte Änderung geblittet." &
                                        "~Geschieht längere Zeit keine Aktion, wird das Blitten verlangsamt auf die hier eingestelltn Intervall. Default: alle 500 ms"
                _Rendering_RenderTimerIntervalPaused = Rendering.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
            End If

            Return _Rendering_RenderTimerIntervalPaused.Value
        End Get
        Set(value As Integer)
            Rendering.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value.ToString)
            _Rendering_RenderTimerIntervalPaused = Nothing
        End Set
    End Property

    Private _Rendering_RenderTimerFramesToPause As Integer?
    Public Property Rendering_RenderTimerFramesToPause As Integer
        Get
            If IsNothing(_Rendering_RenderTimerFramesToPause) Then
                Dim [Default] As Integer = 300
                Dim comment As String = "Anzahl der Frames während der keine Aktion stattfindet bis zum Umschalten auf den IntervallPaused." &
                                        "~Der Wert muss größer sein, als die Steps der längsten Animation, sonst friert diese ein. Default: 300"
                _Rendering_RenderTimerFramesToPause = Rendering.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
            End If

            Return _Rendering_RenderTimerFramesToPause.Value
        End Get
        Set(value As Integer)
            Rendering.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value.ToString)
            _Rendering_RenderTimerFramesToPause = Nothing
        End Set
    End Property

    Private _Rendering_BitmapHighQuality As Boolean?
    Public Property Rendering_BitmapHighQuality As Boolean
        Get
            If IsNothing(_Rendering_BitmapHighQuality) Then
                Dim [Default] As Boolean = True
                Dim comment As String = "Wenn die Bildschirmausgabe auf langsamen Rechnern hakelt, versuchen Sie es mit False. Satz1 = True."
                _Rendering_BitmapHighQuality = Rendering.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
            End If
            Return CBool(_Rendering_BitmapHighQuality)
        End Get
        Set(value As Boolean)
            Rendering.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
            _Rendering_BitmapHighQuality = Nothing
        End Set
    End Property

    Public ReadOnly Property Rendering_InterpolationMode As Drawing2D.InterpolationMode
        Get
            If IsNothing(_Rendering_BitmapHighQuality) Then
                'Initialisieren
                Dim dummy As Boolean = Rendering_BitmapHighQuality
            End If

            If _Rendering_BitmapHighQuality Then
                Return Drawing2D.InterpolationMode.HighQualityBicubic
            Else
                Return Drawing2D.InterpolationMode.HighQualityBilinear
            End If
        End Get
    End Property

    Private _Rendering_HScrollbarHeight As Integer?
    Public Property Rendering_HScrollbarHeightVScrollbarWitdh As Integer
        Get
            If Not _Rendering_HScrollbarHeight.HasValue Then
                Dim [Default] As Integer = 20
                Dim comment As String = "Höhe der horizontalen Scrollbar im Editormodus und Breite der vertikalen Scrollbars. Default = 20"
                _Rendering_HScrollbarHeight = Rendering.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
                _Rendering_HScrollbarHeight = Math.Max(10, Math.Min(30, _Rendering_HScrollbarHeight.Value))
            End If
            Return _Rendering_HScrollbarHeight.Value
        End Get
        Set(value As Integer)
            Rendering.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
            _Rendering_HScrollbarHeight = Nothing
        End Set
    End Property

    Private _Rendering_StockMarkHeight As Integer?
    Public Property Rendering_StockMarkHeight As Integer
        Get
            If Not _Rendering_StockMarkHeight.HasValue Then
                Dim [Default] As Integer = 10
                Dim comment As String = "Höhe der Markierungslsite im Editormodus. Default = 10"
                _Rendering_StockMarkHeight = Rendering.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
                _Rendering_StockMarkHeight = Math.Max(5, Math.Min(20, _Rendering_StockMarkHeight.Value))
            End If
            Return _Rendering_StockMarkHeight.Value
        End Get
        Set(value As Integer)
            Rendering.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
            _Rendering_StockMarkHeight = Nothing
        End Set
    End Property

    Private _Rendering_ConsumeDoRendering As Boolean = True
    '
    ''' <summary>
    ''' Kann von überall heraus aufgerufen werden. Wird bei der Abfrage ob
    ''' neu gerendert werden soll oder die bisherige Bitmap geblittet werden soll.
    ''' </summary>
    Public Sub Rendering_SetConsumeDoRendering()
        _Rendering_ConsumeDoRendering = True
    End Sub
    '

    Private _Rendering_BackgroundColorDarkMode As Color
    Public Property Rendering_BackgroundColorDarkMode As Color
        Get
            If _Rendering_BackgroundColorDarkMode.IsEmpty Then
                Dim [Default] As Color = IniManager.CvtHexStringToColor("FFC0C0C0")
                Dim comment As String = Nothing
                _Rendering_BackgroundColorDarkMode = Rendering.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
            End If
            Return _Rendering_BackgroundColorDarkMode
        End Get
        Set(value As Color)
            Rendering.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
            _Rendering_BackgroundColorDarkMode = Color.Empty
            _Rendering_ConsumeDoRendering = True
        End Set
    End Property

    Private _Rendering_BackgroundColorLightMode As Color
    Public Property Rendering_BackgroundColorLightMode As Color
        Get
            If _Rendering_BackgroundColorLightMode.IsEmpty Then
                Dim [Default] As Color = IniManager.CvtHexStringToColor("FF606060")
                Dim comment As String = "Fallbackwerte. Werden benutzt, solage mit der Toolbox nichts anderes festgelegt wurde."
                _Rendering_BackgroundColorLightMode = Rendering.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
            End If
            Return _Rendering_BackgroundColorLightMode
        End Get
        Set(value As Color)
            Rendering.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
            _Rendering_BackgroundColorLightMode = Color.Empty
            _Rendering_ConsumeDoRendering = True
        End Set
    End Property
    '
    ''' <summary>
    ''' Bereits selektiert nach Light/Darkmode
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property Rendering_BackgroundColor As Color
        Get
            If Global_DarkMode Then
                Return Rendering_BackgroundColorDarkMode
            Else
                Return Rendering_BackgroundColorLightMode
            End If
        End Get
    End Property
    '
    Private _Rendering_MouseOverSteinRahmenColor As Color
    Public Property Rendering_MouseOverSteinRahmenColor As Color
        Get
            If _Rendering_MouseOverSteinRahmenColor.IsEmpty Then
                Dim [Default] As Color = IniManager.CvtHexStringToColor("FF9EDFA3")
                Dim comment As String = ""
                _Rendering_MouseOverSteinRahmenColor = Rendering.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
            End If
            Return _Rendering_MouseOverSteinRahmenColor
        End Get
        Set(value As Color)
            Rendering.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
            _Rendering_MouseOverSteinRahmenColor = Color.Empty
            _Rendering_ConsumeDoRendering = True
        End Set
    End Property

    Private _Rendering_SelectedSteinRahmenColor As Color
    Public Property Rendering_SelectedSteinRahmenColor As Color
        Get
            If _Rendering_SelectedSteinRahmenColor.IsEmpty Then
                Dim [Default] As Color = IniManager.CvtHexStringToColor("FF3FA35C")
                Dim comment As String = ""
                _Rendering_SelectedSteinRahmenColor = Rendering.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
            End If
            Return _Rendering_SelectedSteinRahmenColor
        End Get
        Set(value As Color)
            Rendering.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
            _Rendering_SelectedSteinRahmenColor = Color.Empty
            _Rendering_ConsumeDoRendering = True
        End Set
    End Property

    Private _Rendering_PlaceableSteinRahmenColor As Color
    Public Property Rendering_PlaceableSteinRahmenColor As Color
        Get
            If _Rendering_PlaceableSteinRahmenColor.IsEmpty Then
                Dim [Default] As Color = IniManager.CvtHexStringToColor("FF1F5F3A")
                Dim comment As String = ""
                _Rendering_PlaceableSteinRahmenColor = Rendering.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
            End If
            Return _Rendering_PlaceableSteinRahmenColor
        End Get
        Set(value As Color)
            Rendering.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
            _Rendering_PlaceableSteinRahmenColor = Color.Empty
            _Rendering_ConsumeDoRendering = True
        End Set
    End Property

    '
    Private _Rendering_SteinFlugGeschwindigkeit As Double?
    Public Property Rendering_SteinFlugGeschwindigkeit As Double
        Get
            If Not _Rendering_SteinFlugGeschwindigkeit.HasValue Then
                Dim [Default] As Double = 36
                Dim comment As String = "Die Anzahl der Pixel je Frame, die der Stein weiterkommt. Default: 36"
                _Rendering_SteinFlugGeschwindigkeit = Rendering.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
            End If
            Return _Rendering_SteinFlugGeschwindigkeit.Value
        End Get
        Set(value As Double)
            Rendering.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
            _Rendering_SteinFlugGeschwindigkeit = Nothing
        End Set
    End Property
    '

    Private _Rendering_StartscreenBitmapNameLightMode As String = Nothing
    Private _Rendering_StartscreenBitmapNameLightMode_Loaded As Boolean = False

    Public Property Rendering_StartscreenBitmapNameLightMode As String
        Get
            If Not _Rendering_StartscreenBitmapNameLightMode_Loaded Then
                Dim [Default] As String = "MahjongGK_Light.jpg" ' oder Nothing oder Wert
                Dim comment As String = Nothing
                _Rendering_StartscreenBitmapNameLightMode = Rendering.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
                _Rendering_StartscreenBitmapNameLightMode_Loaded = True
            End If
            Return _Rendering_StartscreenBitmapNameLightMode
        End Get
        Set(value As String)
            Rendering.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
            ' Cache sofort aktualisieren, kein Re-Read:
            _Rendering_StartscreenBitmapNameLightMode = value
            _Rendering_StartscreenBitmapNameLightMode_Loaded = True
        End Set
    End Property
    '
    Private _Rendering_StartscreenBitmapNameDarkMode As String = Nothing
    Private _Rendering_StartScreenBitmapNameDarkMode_Loaded As Boolean = False
    Public Property Rendering_StartScreenBitmapNameDarkMode As String
        Get
            If Not _Rendering_StartScreenBitmapNameDarkMode_Loaded Then
                Dim [Default] As String = "MahjongGK_Dark.jpg" ' oder Nothing oder Wert
                Dim comment As String = Nothing
                _Rendering_StartscreenBitmapNameDarkMode = Rendering.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
                _Rendering_StartScreenBitmapNameDarkMode_Loaded = True
            End If
            Return _Rendering_StartscreenBitmapNameDarkMode
        End Get
        Set(value As String)
            Rendering.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
            ' Cache sofort aktualisieren, kein Re-Read:
            _Rendering_StartscreenBitmapNameDarkMode = value
            _Rendering_StartScreenBitmapNameDarkMode_Loaded = True
        End Set
    End Property
    '
    ''' <summary>
    ''' Gibt den kompletten Pfad in Abhängigkeit vom DarkMode zurück, ggf Nothing.
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property Rendering_StartscreenBitmapFullpath As String
        Get
            If Global_DarkMode Then
                If String.IsNullOrEmpty(Rendering_StartScreenBitmapNameDarkMode) Then
                    Return Nothing
                Else
                    Return AppDataFullPath(AppDataSubDir.Hintergrundgrafiken, Rendering_StartScreenBitmapNameDarkMode)
                End If
            Else
                If String.IsNullOrEmpty(Rendering_StartscreenBitmapNameLightMode) Then
                    Return Nothing
                Else
                    Return AppDataFullPath(AppDataSubDir.Hintergrundgrafiken, Rendering_StartscreenBitmapNameLightMode)
                End If
            End If
        End Get
    End Property

    Public ReadOnly Property Rendering_HasStartscreenBitmapFullpath As Boolean
        Get
            Return Not (String.IsNullOrEmpty(Rendering_StartscreenBitmapNameLightMode) AndAlso String.IsNullOrEmpty(Rendering_StartScreenBitmapNameDarkMode))
        End Get
    End Property

    Private _Rendering_DrawRenderRect As Boolean?
    Public Property Rendering_DrawRenderRect As Boolean
        Get
            'If Debugger.IsAttached Then
            '    Return True
            'Else
            '    Return False
            'End If
            If IsNothing(_Rendering_DrawRenderRect) Then
                Dim [Default] As Boolean = False ' Debugger.IsAttached
                Dim comment As String = "Für die Programmentwicklung zur Kontrolle der Lage der rxRectangle. Nur innerhalb der IDE verwendbar."
                _Rendering_DrawRenderRect = Rendering.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
            End If
            Return CBool(_Rendering_DrawRenderRect)
        End Get
        Set(value As Boolean)
            Rendering.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
            _Rendering_DrawRenderRect = Nothing
            _Rendering_ConsumeDoRendering = True
        End Set
    End Property

    '
    'für zeitkritische Abfragen
    Private _Rendering_HeaderHeight As Integer?
    Public Property Rendering_HeaderHeight As Integer
        Get
            If Not _Rendering_HeaderHeight.HasValue Then
                Dim [Default] As Integer = 30
                Dim comment As String = Nothing
                _Rendering_HeaderHeight = Rendering.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
            End If
            Return _Rendering_HeaderHeight.Value
        End Get
        Set(value As Integer)
            Rendering.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
            _Rendering_HeaderHeight = Nothing
            _Rendering_ConsumeDoRendering = True
        End Set
    End Property
    '

    '
    Private _Rendering_PaddingStageAvailable As PaddingValues? ' Cache
    Public Property Rendering_PaddingStageAvailable As PaddingValues
        Get
            If Not _Rendering_PaddingStageAvailable.HasValue Then
                'Mögliche Schreibweise in der INI: Left=1,Top=1,Right=2,Bottom=2 oder L=1,T=... oder einfach 1,1,2,2 
                Dim [Default] As New PaddingValues(0)
                Dim comment As String = ""
                _Rendering_PaddingStageAvailable = Rendering.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
            End If
            Return _Rendering_PaddingStageAvailable.Value
        End Get
        Set(value As PaddingValues)
            Rendering.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
            _Rendering_PaddingStageAvailable = Nothing   ' Cache ungültig machen
        End Set
    End Property

    Private _Rendering_DrawRenderingSkipDoneMarker As Boolean?
    Public Property Rendering_DrawRenderingSkipDoneMarker As Boolean
        Get
            If IsNothing(_Rendering_DrawRenderingSkipDoneMarker) Then
                Dim [Default] As Boolean = Debugger.IsAttached
                Dim comment As String = "Zeichnet rechts unten im Spielfeld ein Feld ein, an dem erkennbar ist, ob neu gerendert wurde. Default: Debugger.IsAttached"
                _Rendering_DrawRenderingSkipDoneMarker = Rendering.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
            End If
            Return CBool(_Rendering_DrawRenderingSkipDoneMarker)
        End Get
        Set(value As Boolean)
            Rendering.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
            _Rendering_DrawRenderingSkipDoneMarker = Nothing
            _Rendering_ConsumeDoRendering = True
        End Set
    End Property

#End Region

#Region "Tile"

    'alles was 'Stein' heißt bezieht sich auf die alten Steine,
    'die als png-Dateien vorliegen/vorlagen (will ich will alles ausbauen),
    'alles was Tile heißt, auf die neuen Steine, die komplett bei Gebrauch
    'erzeugt werden in der TileFactory.
    '
    '
    '
    'Wichtig für Rendering_AktTileColor:
    'In der Tilefactory gilt: wenn sich die Instanz geändert hat,
    'wid angenommen, irgendwelche Farben hätten sich geändert und
    'das Cache wird neu aufgebaut. ===> Die Instanz nur ändern, 
    'wenn sich wirklich was ändern.
    '()

    Private _Tile_AktTileColor As TileColors
    Public ReadOnly Property Tile_TileColors As TileColors
        Get
            Return _Tile_AktTileColor
        End Get
    End Property
    '
    '
    ''' <summary>
    ''' Läd die aktuelle Instanz der TileColors von der Festplatte.
    ''' Es wird der Satz geladen, der durch
    ''' INI.Tile_SteinDesign und INI.Tile_SteinSatz
    ''' definiert ist.
    ''' </summary>
    Public Sub Tile_TileColors_Load()

        If INI.Tile_SteinDesign.ToString.StartsWith("Test") Then
            If Not Tile_AllowTileColorsTestFiles Then
                INI.Tile_SteinDesign = SteinDesign.Default
            End If
        End If

        Dim fullpath As String = TileColors.GetFullPathOnlyForLoading(INI.Tile_SteinDesign, INI.Tile_SteinSatz, INI.Tile_SteinFont, useDevelopmentPath:=False)

        If File.Exists(fullpath) Then
            Try
                _Tile_AktTileColor = TileColors.Load(INI.Tile_SteinDesign, INI.Tile_SteinSatz, INI.Tile_SteinFont)
            Catch ex As Exception
                _Tile_AktTileColor = New TileColors
            End Try
        Else
            INI.Tile_SteinDesign = SteinDesign.Default
            INI.Tile_SteinSatz = SteinSatz.Medium
            INI.Tile_SteinFont = SteinFont.Segoe
            Try
                _Tile_AktTileColor = TileColors.Load(INI.Tile_SteinDesign, INI.Tile_SteinSatz, INI.Tile_SteinFont)
            Catch ex As Exception
                _Tile_AktTileColor = New TileColors
            End Try
        End If

        _tile_BasisSize = _Tile_AktTileColor.GetTileBasisSize
        If _tile_BasisSize.Width < 120 OrElse _tile_BasisSize.Height < 120 Then
            _tile_BasisSize = New Size(200, 242)
        End If

        _tile_BasisSizeChanged = True

    End Sub

    Private _tile_BasisSize As Size
    Private _tile_BasisSizeChanged As Boolean
    Private _tile_lastBasisSize As Size

    Public ReadOnly Property Tile_BasisSize As Size
        Get
            Return _tile_BasisSize
        End Get
    End Property
    '
    ''' <summary>
    ''' Gibt auch True zurück, wenn ein neuer Steinsatz geladen wurde.
    ''' </summary>
    ''' <returns></returns>
    Public Function Tile_ConsumeSteinSatzOrBasisSizeChanged() As Boolean
        Dim retval As Boolean = _tile_BasisSizeChanged
        If _tile_lastBasisSize <> _tile_BasisSize Then
            retval = True
            _tile_lastBasisSize = _tile_BasisSize
        End If
        _tile_BasisSizeChanged = False
        Return retval
    End Function

    Public Property Tile_SteinDesign As SteinDesign
        Get
            Dim [Default] As String = SteinDesign.Default.ToString
            Dim comment As String = Nothing
            Dim zRetVal As String = Rendering.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
            Dim result As SteinDesign
            If Not [Enum].TryParse(Of SteinDesign)(zRetVal, True, result) Then
                result = SteinDesign.Default
            End If
            Return result
        End Get
        Set(value As SteinDesign)
            Rendering.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value.ToString)
        End Set
    End Property

    Public Property Tile_SteinSatz As SteinSatz
        Get
            Dim [Default] As String = SteinSatz.Medium.ToString
            Dim comment As String = Nothing
            Dim zRetVal As String = Rendering.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
            Dim result As SteinSatz
            If Not [Enum].TryParse(Of SteinSatz)(zRetVal, True, result) Then
                result = SteinSatz.Medium
            End If
            Return result
        End Get
        Set(value As SteinSatz)
            Rendering.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value.ToString)
        End Set
    End Property

    Public Property Tile_SteinFont As SteinFont
        Get
            Dim [Default] As String = SteinFont.Segoe.ToString
            Dim comment As String = Nothing
            Dim zRetVal As String = Rendering.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
            Dim result As SteinFont
            If Not [Enum].TryParse(Of SteinFont)(zRetVal, True, result) Then
                result = SteinFont.Segoe
            End If
            Return result
        End Get
        Set(value As SteinFont)
            Rendering.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value.ToString)
        End Set
    End Property

    Public Property Tile_DontOverwriteExistingTileColorsFiles As Boolean
        Get
            Dim [Default] As Boolean = False
            Dim comment As String = "Das ist eine Hintertüre. Normal werden (manuell) geänderte TileColorFiles (Die" &
                                    "~stehen in: [Benutzer]\MahjongGK\SteinDesigns) beim Programmstart überschrieben."
            Return Rendering.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
        End Get
        Set(value As Boolean)
            Rendering.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value.ToString)
        End Set
    End Property

    Public Property Tile_AllowTileColorsTestFiles As Boolean
        Get
            Dim [Default] As Boolean = Debugger.IsAttached
            Dim comment As String = "Ermöglicht es auf ausgewählten Rechnern testweise neue TileColors.xml auszuprobieren." &
                                    "~Es müssen aber auch andere Daten vorhanden sein, sonst erscheint SteinDesign.Default."
            Return Rendering.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
        End Get
        Set(value As Boolean)
            Rendering.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value.ToString)
        End Set
    End Property

    Public Property Tile_TextUseSegoeUISymbol As Boolean
        Get
            Dim [Default] As Boolean = False
            Dim comment As String = "In der linken oberen Ecke der Steine stehen (nicht immer) Buchstaben oder Symbole" &
                                    "~Per Default werden diese in Arial geschrieben. Ist dieser Schalter ein, werden" &
                                    "~""echte"" Symbole genommmen und in andererem Font gerendert. Ausprobieren." &
                                    "~Wird erst nach Neustart des Programms wirksam."
            Return Rendering.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
        End Get
        Set(value As Boolean)
            Rendering.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value.ToString)
        End Set
    End Property

    ''Private _Tile_SpaceBetweenWidth As Double?
    ''Public Property Tile_SpaceBetweenWidth As Double
    ''    Get
    ''        If Not _Tile_SpaceBetweenWidth.HasValue Then
    ''            Dim [Default] As Double = 10
    ''            Dim comment As String = $"Zwischenraum zwischen den Steinen bezogen auf eine Steinbreite von {GlobalConstants.MJ_GRAFIK_STEIN_BASIS_WIDTH} Pixel." &
    ''                                    "~Steine darunter werden etwas sichtbar. Möglicher Werte: 0 bis 10 Pixel. DEfault 0 Pixel"
    ''            _Tile_SpaceBetweenWidth = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
    ''            If _Tile_SpaceBetweenWidth > 0 Then
    ''                _Tile_SpaceBetweenWidth /= GlobalConstants.MJ_GRAFIK_STEIN_BASIS_WIDTH_DBL
    ''            End If
    ''        End If
    ''        Return _Tile_SpaceBetweenWidth.Value
    ''    End Get
    ''    Set(value As Double)
    ''        BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
    ''        _Tile_SpaceBetweenWidth = Nothing
    ''    End Set
    ''End Property

    ''Private _Tile_SpaceBetweenHeight As Double?
    ''Public Property Tile_SpaceBetweenHeight As Double
    ''    Get
    ''        If Not _Tile_SpaceBetweenHeight.HasValue Then
    ''            Dim [Default] As Double = 10
    ''            Dim comment As String = $"Wie vor für die Höhe bezogen auf {GlobalConstants.MJ_GRAFIK_STEIN_BASIS_HEIGHT} Pixel."
    ''            _Tile_SpaceBetweenHeight = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
    ''            If _Tile_SpaceBetweenHeight > 0 Then
    ''                _Tile_SpaceBetweenHeight /= GlobalConstants.MJ_GRAFIK_STEIN_BASIS_HEIGHT_DBL
    ''            End If
    ''        End If
    ''        Return _Tile_SpaceBetweenHeight.Value
    ''    End Get
    ''    Set(value As Double)
    ''        BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
    ''        _Tile_SpaceBetweenHeight = Nothing
    ''    End Set
    ''End Property
#End Region

#Region "Nicht gespeicherte Daten"

#End Region

#Region "Ini Editieren"

    ''' <summary>
    ''' Zum Editieren der INI während des laufenden Betriebes.
    ''' 
    ''' </summary>
    Public Sub IniEditieren()

        Dim sicRendermode As RenderMode = SFMain.RenderMode
        If SFMain.SFDatHasDataAndDoRender Then
            SFMain.RenderMode = RenderMode.Paused
        End If
        Using frm As New FrmIniEditor()

            frm.ShowDialog()

        End Using
        If SFMain.SFDatHasDataAndDoRender Then
            SFMain.RenderMode = sicRendermode
        End If
    End Sub

#End Region

#Region "AppDataFullPath-Funktionen"

    '
    ' Hier ist die vom Programm genutzte Logik zur Bereitstellung von Pfaden
    ' mit und ohne Dateinamen angesiedelt.
    '
    ' Der Vorteil meines Ansatzes:
    ' Alle internen Zugriffe auf Dateien arbeiten immer mit absoluten Pfaden ==> kein Risiko,
    ' dass irgendwo ein falscher relativer Pfad verwendet wird.
    ' INI-Dateien speichern nur relativ ==> Portabilität zwischen Rechnern ist gegeben.
    ' Alle Daten im Verzeichnis "C:\Users\aktueller Anwender\MahjongGK"
    ' und alle vom User im Verzeichnis  "C:\Users\aktueller Anwender\Dokumente\???"
    ' gespeicherten Daten können portiert werden.

    ' Die Konvertierungslogik ist klar getrennt von den normalen Pfadzugriffen

    ' Hinweis:
    ' Hier werden die Enumerationen AppDataSubDir und AppDataSubSubDir verwendet,
    ' sie sind im Modul INI
    ' Aufbau:
    ' ''' <summary>
    ' ''' Enumeration der verwendeten Unterverzeichnisse in "C:\Users\aktueller User\MahjongGK\SubDefault.value.ToString"
    ' ''' Verwendung: Entweder über Dim Path As String = INI.AppDataDefault(.....)
    ' ''' oder durch Nutzung (im Modul INI) einer Public Property Kopier_VorlageFürPfade As String
    ' ''' Die gewünschten Pfade werden automatisch angelegt.
    ' ''' </summary>
    ' Public Enum AppDataSubDir
    '     None
    '     ...
    '     ...
    ' End Enum
    ' ''' <summary>
    ' ''' Enumeration der verwendeten Unterverzeichnisse in "C:\Users\aktueller User\MahjongGK\SubDefault.value.ToString\SubSubDefault.value.ToString"
    ' ''' </summary>
    ' Public Enum AppDataSubSubDir
    '     None
    '     ...
    '     ...
    ' End Enum
    '
    ''' <summary>
    ''' Gibt ein Verzeichnis gemäß der Enumerationen im AppDataDirectory zurück.
    ''' </summary>
    ''' <param name="subdir"></param>
    ''' <param name="subsubdir"></param>
    ''' <returns></returns>
    Public Function AppDataDirectory(Optional subdir As AppDataSubDir = Nothing,
                                     Optional subsubdir As AppDataSubSubDir = Nothing,
                                     Optional sub3Dir As String = Nothing) As String

        Return BasisIni.AppDataDirectory(subdir, subsubdir, sub3Dir)

    End Function
    ''' <summary>
    ''' Montiert den kompletten Pfad aus den Enumerationen und fügt ggf. den aktuellen Zeitstempel hinzu.
    ''' Bei timestamp = AppDataTimeStamp.LookForLastTimeStamp wird nach der jüngsten Datei gesucht.
    ''' Gibt es keine Datei, wird String.Empty zurückgegeben!
    ''' maxFiles arbeitet nur in Verbindung mit timestamp = AppDataTimeStamp.AddRenderBitmapTopZOrder und räumt
    ''' "on the Fly" auf, indem alle Dateien über maxFiles hinaus gelöscht werden.
    ''' </summary>
    Public Function AppDataFullPath(filename As AppDataFileName,
                                 Optional timestamp As AppDataTimeStamp = AppDataTimeStamp.None,
                                 Optional maxFiles As Integer = Integer.MaxValue) As String
        '
        'Ruft die gleiche Überladung in BasisIni auf.
        Return BasisIni.AppDataFullPath(filename, timestamp, maxFiles)

    End Function
    '
    ''' <summary>
    ''' Montiert den kompletten Pfad aus den Enumerationen und fügt ggf. den aktuellen Zeitstempel hinzu.
    ''' Bei timestamp = AppDataTimeStamp.LookForLastTimeStamp wird nach der jüngsten Datei gesucht.
    ''' Gibt es keine Datei, wird String.Empty zurückgegeben!
    ''' maxFiles arbeitet nur in Verbindung mit timestamp = AppDataTimeStamp.AddRenderBitmapTopZOrder und räumt
    ''' "on the Fly" auf, indem alle Dateien über maxFiles hinaus gelöscht werden.
    ''' </summary>
    Public Function AppDataFullPath(subdir As AppDataSubDir,
                                    filename As AppDataFileName,
                                    Optional timestamp As AppDataTimeStamp = AppDataTimeStamp.None,
                                    Optional maxFiles As Integer = Integer.MaxValue) As String
        '
        'Ruft die gleiche Überladung in BasisIni auf.
        Return BasisIni.AppDataFullPath(subdir, filename, timestamp, maxFiles)

    End Function

    ''' <summary>
    ''' Montiert den kompletten Pfad aus und fügt ggf. den aktuellen Zeitstempel hinzu.
    ''' Bei timestamp = AppDataTimeStamp.LookForLastTimeStamp wird nach der jüngsten Datei gesucht.
    ''' Gibt es keine Datei, wird String.Empty zurückgegeben!
    ''' maxFiles arbeitet nur in Verbindung mit timestamp = AppDataTimeStamp.AddRenderBitmapTopZOrder und räumt
    ''' "on the Fly" auf, indem alle Dateien über maxFiles hinaus gelöscht werden.
    ''' </summary>
    Public Function AppDataFullPath(subdir As AppDataSubDir,
                                    filename As String,
                                    Optional timestamp As AppDataTimeStamp = AppDataTimeStamp.None,
                                    Optional maxFiles As Integer = Integer.MaxValue) As String
        '

        Return BasisIni.AppDataFullPath(If(subdir <> AppDataSubDir.None, subdir.ToString, String.Empty),
            String.Empty,
            filename,
            timestamp,
            maxFiles
            )

    End Function

    '
    ''' <summary>
    ''' Montiert den kompletten Pfad aus den Enumerationen und fügt ggf. den aktuellen Zeitstempel hinzu.
    ''' Bei timestamp = AppDataTimeStamp.LookForLastTimeStamp wird nach der jüngsten Datei gesucht.
    ''' Gibt es keine Datei, wird String.Empty zurückgegeben!
    ''' maxFiles arbeitet nur in Verbindung mit timestamp = AppDataTimeStamp.AddRenderBitmapTopZOrder und räumt
    ''' "on the Fly" auf, indem alle Dateien über maxFiles hinaus gelöscht werden.
    ''' </summary>
    Public Function AppDataFullPath(subdir As AppDataSubDir,
                                    subsubdir As AppDataSubSubDir,
                                    filename As AppDataFileName,
                                    Optional timestamp As AppDataTimeStamp = AppDataTimeStamp.None,
                                    Optional maxFiles As Integer = Integer.MaxValue) As String
        '
        'Ruft die gleiche Überladung in BasisIni auf.
        Return BasisIni.AppDataFullPath(subdir, subsubdir, filename, timestamp, maxFiles)

    End Function
    '
    ''' <summary>
    ''' Montiert den kompletten Pfad aus den Enumerationen und fügt ggf. den aktuellen Zeitstempel hinzu.
    ''' Bei timestamp = AppDataTimeStamp.LookForLastTimeStamp wird nach der jüngsten Datei gesucht.
    ''' Gibt es keine Datei, wird String.Empty zurückgegeben!
    ''' maxFiles arbeitet nur in Verbindung mit timestamp = AppDataTimeStamp.AddRenderBitmapTopZOrder und räumt
    ''' "on the Fly" auf, indem alle Dateien über maxFiles hinaus gelöscht werden.
    ''' </summary>
    Public Function AppDataFullPath(subdir As AppDataSubDir,
                                    subsubdir As AppDataSubSubDir) As String
        '
        'Ruft die gleiche Überladung in BasisIni auf.
        Return BasisIni.AppDataFullPath(subdir, subsubdir, AppDataFileName.None, maxFiles:=Integer.MaxValue)

    End Function
    '
    ''' <summary>
    ''' Montiert den kompletten Pfad aus und fügt ggf. den aktuellen Zeitstempel hinzu.
    ''' Bei timestamp = AppDataTimeStamp.LookForLastTimeStamp wird nach der jüngsten Datei gesucht.
    ''' Gibt es keine Datei, wird String.Empty zurückgegeben!
    ''' maxFiles arbeitet nur in Verbindung mit timestamp = AppDataTimeStamp.AddRenderBitmapTopZOrder und räumt
    ''' "on the Fly" auf, indem alle Dateien über maxFiles hinaus gelöscht werden.
    ''' </summary>
    Public Function AppDataFullPath(subdir As AppDataSubDir,
                                    subsubdir As AppDataSubSubDir,
                                    filename As String,
                                    Optional timestamp As AppDataTimeStamp = AppDataTimeStamp.None,
                                    Optional maxFiles As Integer = Integer.MaxValue) As String
        '
        Return BasisIni.AppDataFullPath(
            If(subdir <> AppDataSubDir.None, subdir.ToString, String.Empty),
            BasisIni.AppDataSubSubDirToString(subsubdir),
            filename,
            timestamp,
            maxFiles
            )

    End Function
    '
    '-------------------------------
    '
    '
    Public Function AppDataFullPathWithOpenFileDialog(subdir As String,
                                    pattern As String,
                                    Optional header As String = Nothing) As String
        '
        Dim path As String = BasisIni.AppDataFullPath(subdir)

        Return BasisIni.GetFullpathFromSelectedFile(path, pattern, header)

    End Function
    Public Function AppDataFullPathWithOpenFileDialog(subdir As AppDataSubDir,
                                    pattern As AppDataFilePattern,
                                    Optional header As String = Nothing) As String
        '
        Dim path As String = BasisIni.AppDataFullPath(subdir, AppDataFileName.None, AppDataTimeStamp.None)

        Return BasisIni.GetFullpathFromSelectedFile(path, PatternFromEnum(pattern), header)

    End Function
    '
    Public Function AppDataFullPathWithOpenFileDialog(subdir As AppDataSubDir,
                                    pattern As String,
                                    Optional header As String = Nothing) As String
        '
        Dim path As String = BasisIni.AppDataFullPath(subdir, AppDataFileName.None, AppDataTimeStamp.None)

        Return BasisIni.GetFullpathFromSelectedFile(path, pattern, header)

    End Function
    '
    Public Function AppDataFullPathWithOpenFileDialog(subdir As AppDataSubDir,
        subsubdir As AppDataSubSubDir,
        pattern As AppDataFilePattern,
                                    Optional header As String = Nothing) As String
        '
        Dim path As String = BasisIni.AppDataFullPath(subdir, subsubdir, AppDataFileName.None, AppDataTimeStamp.None)

        Return BasisIni.GetFullpathFromSelectedFile(path, PatternFromEnum(pattern), header)

    End Function
    '
    Public Function AppDataFullPathWithOpenFileDialog(subdir As AppDataSubDir,
                                    subsubdir As AppDataSubSubDir,
                                    pattern As String,
                                    Optional header As String = Nothing) As String
        '
        Dim path As String = BasisIni.AppDataFullPath(subdir, subsubdir, AppDataFileName.None, AppDataTimeStamp.None)

        Return BasisIni.GetFullpathFromSelectedFile(path, pattern, header)

    End Function

    Public Function PatternFromEnum(value As AppDataFilePattern) As String
        Dim s As String = value.ToString()
        Return s.Replace("_D_", ".") _
            .Replace("_Q_", "?") _
            .Replace("_N_", "#") _
            .Replace("_S_", "*") _
            .Replace("_SD_", "*.") _
            .Replace("_SDS_", "*.*")

    End Function

#End Region

#Region "Interne Verwaltung"

#Region "Hilfsfunktionen"
    ''' <summary>
    ''' Hängt die Enumeration als Text an den key.
    ''' </summary>
    ''' <param name="funktionsname"></param>
    ''' <param name="enumToString"></param>
    ''' <returns></returns>
    Private Function FolderAndKeyFrom(funktionsname As String, enumToString As String) As (folder As String, key As String)

        Dim fakf As (folder As String, key As String) = FolderAndKeyFrom(funktionsname)

        If Not String.IsNullOrEmpty(enumToString) Then
            fakf.key &= "_" & enumToString
        End If
        Return fakf

    End Function

    Private Function FolderAndKeyFrom(funktionsname As String) As (folder As String, key As String)

        ' Getter-/Setter-Präfixe entfernen
        If funktionsname.StartsWith("get_") Then
            funktionsname = funktionsname.Substring(4)
        ElseIf funktionsname.StartsWith("set_") Then
            funktionsname = funktionsname.Substring(4)
        End If

        ' Position des Unterstrichs suchen
        Dim iZeiPos As Integer = funktionsname.IndexOf("_"c)

        If iZeiPos < 0 Then
            ' Kein Unterstrich -> Sonderfall
            Return ("UnknownFolderFromFunktionsname", funktionsname)
        Else
            ' Alles vor dem Unterstrich = Folder
            ' Alles nach dem Unterstrich = Key
            Dim folder As String = funktionsname.Substring(0, iZeiPos)
            Dim key As String = funktionsname.Substring(iZeiPos + 1)
            Return (folder, key)
        End If
    End Function

#End Region

#Region "InitDragDropBitmaps"

    ''' <summary>
    ''' Schaltet InitialisierungAktiv für alle IniManager um.
    ''' </summary>
    Public Sub AllIniManagersSetInitialisierungAktiv(value As Boolean)
        For Each m As IniManager In AllIniManagers
            m.InitialisierungAktiv = value
        Next
    End Sub

    Public Sub AllIniManagersSetRaiseIniEvents(value As IniEvents)
        For Each m As IniManager In AllIniManagers
            m.RaiseIniEvents = value
        Next
    End Sub

    Public Sub AllIniManagerSave()
        For Each m As IniManager In AllIniManagers
            m.Save()
        Next
    End Sub

    Public Sub AllIniManagersSaveIniFileIfNotExisting()
        For Each m As IniManager In AllIniManagers
            Dim fullpath As String = m.FileFullPath
            If Not IO.File.Exists(fullpath) Then
                m.Save(alwaysSave:=True)
            End If
        Next
    End Sub

    Public Sub AllIniManagersBackupRaiseIniEvents()
        For Each m As IniManager In AllIniManagers
            m.BackupValueRaiseIniEvents()
        Next
    End Sub
    Public Sub AllIniManagersRestoreRaiseIniEvents()
        For Each m As IniManager In AllIniManagers
            m.RestoreValueRaiseIniEvents()
        Next
    End Sub

    Public Function AllIniManagersFullPath() As String()
        Dim lo As New List(Of String)
        For Each m As IniManager In AllIniManagers
            lo.Add(m.FileFullPath)
        Next

        Return lo.ToArray

    End Function

    Public Property AllIniManagersIniLines As List(Of String)()
        Get
            Dim idx As Integer = -1
            For Each m As IniManager In AllIniManagers
                idx += 1
            Next
            Dim arr(idx) As List(Of String)
            idx = -1
            For Each m As IniManager In AllIniManagers
                idx += 1
                arr(idx) = m.IniLines
            Next

            Return arr
        End Get

        Set(value As List(Of String)())
            Dim idx As Integer = -1
            For Each m As IniManager In AllIniManagers
                idx += 1
                m.IniLines = value(idx)
            Next
        End Set
    End Property

#Region "InitDragDropBitmaps der Default-Werte"

    ' Liefert alle Werte eines Enum-Typs als Object()-Array
    Private Function GetEnumValues(t As Type) As Object()
        Dim arr As System.Array = [Enum].GetValues(t)
        Dim result(arr.Length - 1) As Object
        arr.CopyTo(result, 0)
        Return result
    End Function

    ' Prüft, ob alle Typen Enums sind
    Private Function AreAllEnums(types As Type()) As Boolean
        For Each tp As Type In types ' <- Typ ergänzt
            If Not tp.IsEnum Then Return False
        Next
        Return True
    End Function

    ' Kartesisches Produkt der Enumwerte der Parametertypen
    Private Iterator Function EnumArgumentCombinations(paramTypes As Type()) As IEnumerable(Of Object())
        If paramTypes Is Nothing OrElse paramTypes.Length = 0 Then
            Yield New Object() {}
            Return
        End If

        Dim valueSets As New List(Of Object())()

        For Each pt As Type In paramTypes
            valueSets.Add(GetEnumValues(pt))
        Next

        Dim indices(valueSets.Count - 1) As Integer
        Dim lengths(valueSets.Count - 1) As Integer
        For i As Integer = 0 To valueSets.Count - 1 '  
            lengths(i) = valueSets(i).Length
        Next

        Dim done As Boolean = (valueSets.Count = 0)
        While Not done
            Dim args(valueSets.Count - 1) As Object
            For i As Integer = 0 To valueSets.Count - 1
                args(i) = valueSets(i)(indices(i))
            Next
            Yield args

            Dim pos As Integer = valueSets.Count - 1
            While pos >= 0
                indices(pos) += 1
                If indices(pos) < lengths(pos) Then
                    Exit While
                Else
                    indices(pos) = 0
                    pos -= 1
                End If
            End While
            If pos < 0 Then done = True
        End While
    End Function

    Public Sub AllIniManagersInitAllDefaultsAndSave()

        AllIniManagersSetInitialisierungAktiv(True)

        ' Alle öffentlichen Shared Member des Moduls INI holen
        Dim allMembers As MemberInfo() =
        GetType(INI).GetMembers(BindingFlags.Public Or BindingFlags.Static)

        ' Filtern (nur Member mit "_" und keine get_/set_-Accessor)
        Dim gefilterte As List(Of MemberInfo) =
        allMembers.
        Where(Function(m) m.Name.Contains("_"c) AndAlso
                          Not m.Name.StartsWith("get_", System.StringComparison.OrdinalIgnoreCase) AndAlso
                          Not m.Name.StartsWith("set_", System.StringComparison.OrdinalIgnoreCase) AndAlso
                          (TypeOf m Is PropertyInfo OrElse TypeOf m Is MethodInfo)).
        OrderBy(Function(m) m.Name).
        ToList()

        If gefilterte.Count = 0 Then
            AllIniManagersSetInitialisierungAktiv(False)
            Exit Sub
        End If

        ' Aufruf
        For Each member As MemberInfo In gefilterte
            Try
                If TypeOf member Is PropertyInfo Then
                    Dim p As PropertyInfo = DirectCast(member, PropertyInfo)
                    If Not p.CanRead Then
                        Continue For
                    End If

                    Dim indexParams As ParameterInfo() = p.GetIndexParameters()
                    If indexParams Is Nothing OrElse indexParams.Length = 0 Then
                        ' Normale (parameterlose) Property
                        p.GetValue(Nothing, Nothing)
                    Else
                        ' Index-Property: nur wenn alle Parameter Enums sind
                        Dim paramTypes As Type() = indexParams.Select(Function(pi) pi.ParameterType).ToArray()
                        If AreAllEnums(paramTypes) Then
                            For Each args As Object() In EnumArgumentCombinations(paramTypes) ' <- Typ ergänzt
                                p.GetValue(Nothing, args)
                            Next
                        End If
                    End If

                ElseIf TypeOf member Is MethodInfo Then
                    Dim mi As MethodInfo = DirectCast(member, MethodInfo)
                    Dim pars As ParameterInfo() = mi.GetParameters()

                    If pars Is Nothing OrElse pars.Length = 0 Then
                        mi.Invoke(Nothing, Nothing)
                    Else
                        Dim paramTypes As Type() = pars.Select(Function(pi) pi.ParameterType).ToArray()
                        If AreAllEnums(paramTypes) Then
                            For Each args As Object() In EnumArgumentCombinations(paramTypes) ' <- Typ ergänzt
                                mi.Invoke(Nothing, args)
                            Next
                        End If
                    End If
                End If

            Catch ex As Exception
                ' Defensives Logging
                Console.WriteLine("Fehler beim Aufruf von " & member.Name & ": " & ex.Message)
            End Try
        Next

        AllIniManagersSetInitialisierungAktiv(False)
        AllIniManagersSaveIniFileIfNotExisting()
    End Sub

#End Region

#Region "UpDateIni"

    '  'geschrieben von ChatGPT 5
    ' ──────────────────────────────────────────────────────────────────────────────
    '   ARCHITEKTUR & REGELN FÜR DAS INI-SYSTEM
    ' ──────────────────────────────────────────────────────────────────────────────
    '
    ' • Alle Properties, die mit der INI synchronisiert werden sollen, müssen
    '   im Namen mindestens einen Unterstrich "_" enthalten.
    '
    ' • Zwei Property-Arten werden automatisch berücksichtigt:
    '   1) Normale (nicht-indexierte) Properties → via GetIniPropertiesWithUnderscore()
    '   2) Enum-indexierte Properties → via GetIniIndexedPropertiesWithEnumIndex()
    '      (z. B. ToolBox_FeldSizeX(Basisform))
    '
    ' • Typenvielfalt:
    '   - Unterstützt werden alle gängigen ValueTypes (Integer, Double, Date, Enum, Color,
    '     Point, Size, Rectangle, …), Strings, Fonts sowie weitere ICloneable-Typen.
    '   - Enum-Werte können in der INI als Name ("Linie") oder als Zahl gespeichert sein;
    '     CoerceForProperty wandelt sie automatisch korrekt zurück.
    '
    ' • Caches & Events:
    '   - ResetIniCaches leert alle internen Cache-Felder (Shared "_...").
    '   - RaiseAllIniEvents löst alle registrierten Events neu aus.
    '   - ResetIniCachesAndRaiseAllIniEvents kombiniert beide Schritte.
    '
    ' • Arbeitsweise von UpDateIni:
    '   1) Neue *.tmp laden (ggf. Org→Tmp kopieren).
    '   2) Alle relevanten Properties auslesen und ihre Werte merken.
    '   3) Org-INI laden.
    '   4) Gemerkte Werte über die Property-Setter zurückschreiben
    '      (→ dadurch BasisIni.WriteValue + Events).
    '   5) Alles abspeichern und Caches/Events zurücksetzen.
    '
    ' • Sicherheit:
    '   - RaiseIniEvents wird während des Kopiervorgangs deaktiviert und
    '     danach auf den ursprünglichen Zustand zurückgesetzt.
    '   - Fehler führen zum Abbruch und zur Wiederherstellung des Originalzustands.
    '
    ' Damit gilt:
    '   Wenn eine neue Property den Grundregeln entspricht (Unterstrich im Namen,
    '   Indexer optional als Enum), wird sie automatisch mitgezogen.
    '
    ' ──────────────────────────────────────────────────────────────────────────────
    '

    'geschrieben von ChatGPT 5
    'Reflexion auf Modul-Properties: GetType(INI) funktioniert, da ein Module als statische
    'Klasse kompiliert wird. Wir filtern sauber auf Public Static, ohne Indexer,
    'mit CanRead/CanWrite, und mit Unterstrich im Namen.

    'Event-Auslösung: Die Setter deiner Properties rufen BasisIni.WriteValue(...) auf.
    'Ob Events feuern, steuert BasisIni.RaiseIniEvents → deshalb schalten wir erst auf raiseIE,
    'nachdem die Org-INI geladen ist, und schreiben dann zurück.

    'Sicherheit/Recovery: RaiseIniEvents wird In jedem Fall wieder auf den gesicherten Zustand
    'zurückgesetzt (auch bei Exceptions).

    'Keine Listenpflege nötig: Neue Properties mit Unterstrich werden automatisch berücksichtigt.

    'Typenvielfalt: Da wir über die Properties selbst schreiben/lesen, sind alle von dir
    'unterstützten Typen (Integer, Double, Date, Enum, Color, Point, …) automatisch abgedeckt.

    ''' <summary>
    ''' Aktualisiert die INI-Werte und löst je nach <paramref name="raiseIE"/> die Events aus.
    ''' Szenario A: <paramref name="readNewIniFromIniTmp"/> = False → Org-Ini wird nach Tmp kopiert (interne Aktualisierung).
    ''' Szenario B: <paramref name="readNewIniFromIniTmp"/> = True  → Extern bereitgestellte Tmp-Ini wird übernommen.(vom IniEditor)
    ''' </summary>
    ''' <param name="raiseIE">Steuert, wann BasisIni.WriteValue True liefert (und damit Events feuert).</param>
    ''' <param name="readNewIniFromIniTmp">Wenn True: vorhandene Tmp-Ini als Quelle verwenden, sonst Org→Tmp kopieren.</param>
    Public Sub UpDateIni(raiseIE As IniEvents, readNewIniFromIniTmp As Boolean)

        For Each manager As IniManager In AllIniManagers
            ' ... (unverändert bis zur Liste) ...

            manager.BackupValueRaiseIniEvents()
            manager.RaiseIniEvents = IniEvents.None

            ' --- zwei Container: einfache Props & indexierte Enum-Props ---
            Dim newValuesSimple As New List(Of (Prop As PropertyInfo, Value As Object))()
            Dim newValuesIndexed As New List(Of (Prop As PropertyInfo, IndexArgs As Object(), Value As Object))()

            Try

                If Not readNewIniFromIniTmp Then
                    ' Szenario A: Org → Tmp kopieren
                    manager.CopyOrgFileToTmpFile()
                Else
                    ' Szenario B: Tmp ist bereits bereitgestellt
                    ' 1) Tmp laden
                    manager.LoadTmpFile()
                End If

                ' Caches zurücksetzen, damit die Properties neu aus der (Tmp-)INI lesen
                ResetIniCaches()

                ' 2a) Einfache (nicht-indexierte) INI-Properties lesen/merken
                For Each p As PropertyInfo In GetIniPropertiesWithUnderscore()

                    Dim cur As Object = p.GetValue(Nothing, Nothing)
                    newValuesSimple.Add((p, CloneIfNeeded(cur)))
                Next

                ' 2b) Indexierte INI-Properties mit Enum-Index lesen/merken
                For Each p As PropertyInfo In GetIniIndexedPropertiesWithEnumIndex()
                    Dim idxTypes As Type() =
                     p.GetIndexParameters().Select(Function(pi) pi.ParameterType).ToArray()

                    For Each idxArgs As Object() In EnumerateIndexArgTuples(idxTypes)
                        Dim cur As Object = p.GetValue(Nothing, idxArgs)
                        newValuesIndexed.Add((p,
                              DirectCast(idxArgs.Clone(), Object()),
                              CloneIfNeeded(cur)))
                    Next
                Next

                ' 3) Zur Org-INI zurück
                manager.LoadOrgFile()

                ' 4) Event-Steuerung setzen
                manager.RaiseIniEvents = raiseIE

                ' 5a) Einfache Werte zurückschreiben
                For Each entry As (Prop As PropertyInfo, Value As Object) In newValuesSimple
                    entry.Prop.SetValue(Nothing, entry.Value, Nothing)
                Next

                ' 5b) Indexierte Werte zurückschreiben
                For Each entry As (Prop As PropertyInfo, IndexArgs As Object(), Value As Object) In newValuesIndexed
                    entry.Prop.SetValue(Nothing, entry.Value, entry.IndexArgs)
                Next

            Catch
                manager.RestoreValueRaiseIniEvents()
                Throw
            End Try

            manager.RestoreValueRaiseIniEvents()
            manager.Save(alwaysSave:=True)
        Next

        ResetIniCachesAndRaiseAllIniEvents()
    End Sub

    ''' <summary>
    ''' Setzt alle Cache-Felder (Private, Shared) dieses Moduls zurück,
    ''' deren Namen mit "_" beginnen und mindestens einen weiteren "_" enthalten.
    ''' Nullable-Typen -> Nothing; Strukturen -> .Empty falls vorhanden, sonst Default(0).
    ''' </summary>
    Public Sub ResetIniCaches(Optional useEmptyForStructs As Boolean = True)

        Dim t As Type = GetType(INI) ' <— falls das Modul anders heißt, hier anpassen
        Dim flags As BindingFlags = BindingFlags.NonPublic Or BindingFlags.Static Or BindingFlags.DeclaredOnly

        Dim cleared As Integer = 0

        For Each f As FieldInfo In t.GetFields(flags)
            Dim n As String = f.Name
            If n.StartsWith("_", StringComparison.Ordinal) AndAlso n.IndexOf("_"c, 1) >= 0 Then
                Dim ft As Type = f.FieldType
                Dim newVal As Object = Nothing

                If ft.IsGenericType AndAlso ft.GetGenericTypeDefinition() Is GetType(Nullable(Of )) Then
                    ' Nullable(Of T) -> Nothing (HasValue=False)
                    newVal = Nothing

                ElseIf ft.IsValueType Then
                    If useEmptyForStructs Then
                        ' Versuche .Empty (Size, SizeF, Point, PointF, Rectangle, RectangleF)
                        Dim emptyField As FieldInfo = ft.GetField("Empty", BindingFlags.Public Or BindingFlags.Static)
                        If emptyField IsNot Nothing Then
                            newVal = emptyField.GetValue(Nothing)
                        Else
                            newVal = Activator.CreateInstance(ft) ' Default(0)
                        End If
                    Else
                        newVal = Activator.CreateInstance(ft) ' Default(0)
                    End If

                Else
                    ' Referenztypen (falls vorhanden) -> Nothing
                    newVal = Nothing
                End If

                f.SetValue(Nothing, newVal)
                cleared += 1
            End If
        Next

#If DEBUG Then
        Debug.WriteLine($"ResetIniCaches(): cleared {cleared} fields.")
#End If
    End Sub

    ''' <summary>
    ''' Löst alle (parameterlosen) Events im Modul INI aus, indem alle registrierten Handler aufgerufen werden.
    ''' </summary>
    ''' <returns>Anzahl aufgerufener Handler.</returns>
    Public Function RaiseAllIniEvents() As Integer
        Dim t As Type = GetType(INI) ' ggf. anpassen
        Dim flags As BindingFlags =
            BindingFlags.Public Or BindingFlags.NonPublic Or BindingFlags.Static Or BindingFlags.DeclaredOnly

        Dim handlersCalled As Integer = 0
        Dim eventCount As Integer = 0

        For Each ev As EventInfo In t.GetEvents(flags)
            eventCount += 1

            ' VB.NET-Backing-Field heißt i.d.R. "<EventName>Event"
            Dim backing As FieldInfo =
                t.GetField(ev.Name & "Event", BindingFlags.NonPublic Or BindingFlags.Static Or BindingFlags.IgnoreCase)

            If backing Is Nothing Then
#If DEBUG Then
                ' Debug.WriteLine($"RaiseAllIniEvents: Kein Backing-Field für Event '{ev.Name}'.")
#End If
                Continue For
            End If

            Dim multicast As [Delegate] = TryCast(backing.GetValue(Nothing), [Delegate])
            If multicast Is Nothing Then Continue For

            For Each d As [Delegate] In multicast.GetInvocationList()
                Try
                    ' Parameterlos auslösen:
                    d.DynamicInvoke()
                    handlersCalled += 1
                Catch ex As TargetInvocationException
#If DEBUG Then
                    Debug.WriteLine($"Event '{ev.Name}' Handler '{d.Method.Name}' warf: {ex.InnerException?.Message}")
#End If
                End Try
            Next
        Next

#If DEBUG Then
        ' Debug.WriteLine($"RaiseAllIniEvents: {eventCount} Events; {handlersCalled} Handler aufgerufen.")
#End If

        Return handlersCalled
    End Function

    ''' <summary>
    ''' Komfort: Zuerst alle INI-Caches leeren, dann alle Events feuern.
    ''' </summary>
    Public Sub ResetIniCachesAndRaiseAllIniEvents()
        ResetIniCaches() ' Deine bereits vorhandene Methode
        RaiseAllIniEvents()
    End Sub

    ''' <summary>
    ''' Liefert alle öffentlichen, statischen, parameterlosen INI-Properties aus diesem Modul,
    ''' die sowohl get als auch set besitzen und mindestens einen Unterstrich "_" im Namen haben.
    ''' </summary>
    Public Function GetIniPropertiesWithUnderscore() As IEnumerable(Of PropertyInfo)

        Dim flags As BindingFlags = BindingFlags.Public Or BindingFlags.Static Or BindingFlags.DeclaredOnly

        ' Hinweis: Module werden als statische Klassen kompiliert; GetType(INI) ist gültig.
        Dim props() As PropertyInfo = GetType(INI).GetProperties(flags)

        Return props.
            Where(Function(p) p.CanRead AndAlso p.CanWrite).
            Where(Function(p) p.GetIndexParameters().Length = 0).
            Where(Function(p) p.Name.Contains("_"))

    End Function
    '
    ''' <summary>
    ''' Liefert für gegebene Werte eine eigenständige Kopie zurück,
    ''' wo nötig. ValueTypes (Strukturen) werden automatisch kopiert.
    ''' Unterstützte Typen:
    ''' - Integer, Long, Single, Double, Decimal, Date, Enumerationen (ValueTypes)
    ''' - Color, Point, PointF, Size, SizeF, Rectangle, RectangleF (ValueTypes)
    ''' - Font (Referenztyp) → wird geklont
    ''' Andere unveränderliche Typen (z.B. String) werden unverändert zurückgegeben.
    ''' </summary>
    Public Function CloneIfNeeded(value As Object) As Object
        If value Is Nothing Then Return Nothing

        ' 1) Spezialfall: Font (Referenztyp, veränderlich) → klonen
        If TypeOf value Is Font Then
            Return CType(value, Font).Clone()
        End If

        Dim t As Type = value.GetType()

        ' 2) Alle ValueTypes (Structs) sind per Zuweisung Kopien:
        '    Dazu gehören: Integer, Long, Single, Double, Decimal, Boolean,
        '    Date (DateTime), Enums, Color, Point, PointF, Size, SizeF,
        '    Rectangle, RectangleF … usw.
        If t.IsValueType Then
            Return value
        End If

        ' 3) String ist unveränderlich → kann geteilt werden
        If TypeOf value Is String Then
            Return value
        End If

        ' 4) Falls ein anderer Referenztyp ICloneable unterstützt, vorsichtig nutzen
        Dim clonable As ICloneable = TryCast(value, ICloneable)
        If clonable IsNot Nothing Then
            Try
                Return clonable.Clone()
            Catch
                ' Falls das Clone fehlschlägt, einfach Fallback
            End Try
        End If

        ' 5) Fallback: Referenz zurückgeben (für deine Liste nicht relevant)
        Return value
    End Function

    ' ───────────────────────────
    ' Hilfsfunktionen
    ' ───────────────────────────

    Private Function IsIntegralType(t As Type) As Boolean
        Return t Is GetType(Byte) OrElse t Is GetType(SByte) _
            OrElse t Is GetType(Short) OrElse t Is GetType(UShort) _
            OrElse t Is GetType(Integer) OrElse t Is GetType(UInteger) _
            OrElse t Is GetType(Long) OrElse t Is GetType(ULong)
    End Function

    ''' <summary>
    ''' True, wenn value ein endlicher Float/Decimal/Integer ist, der EXAKT ganzzahlig ist.
    ''' Gibt dann den Int64-Wert (ohne Runden) in result zurück.
    ''' </summary>
    Private Function TryAsExactWholeInt64(value As Object, ByRef result As Long) As Boolean
        If value Is Nothing Then Return False

        Try
            If TypeOf value Is Double Then
                Dim d As Double = DirectCast(value, Double)
                If Double.IsNaN(d) OrElse Double.IsInfinity(d) Then Return False
                If d <> Math.Truncate(d) Then Return False
                result = CLng(d)
                Return True

            ElseIf TypeOf value Is Single Then
                Dim f As Single = DirectCast(value, Single)
                If Single.IsNaN(f) OrElse Single.IsInfinity(f) Then Return False
                If f <> Math.Truncate(f) Then Return False
                result = CLng(f)
                Return True

            ElseIf TypeOf value Is Decimal Then
                Dim m As Decimal = DirectCast(value, Decimal)
                If m <> Decimal.Truncate(m) Then Return False
                result = Decimal.ToInt64(m)
                Return True

            ElseIf IsIntegralType(value.GetType()) Then
                result = System.Convert.ToInt64(value, CultureInfo.InvariantCulture)
                Return True
            End If
        Catch
            Return False
        End Try

        Return False
    End Function
    ' --- NEU: Helfer, der Werte auf den Property-Typ formt (inkl. Enum & Nullable(Of Enum)) ---
    ' Coerce nur, wenn die Umwandlung nachweislich verlustfrei ist.

    ' --- NEU: prüft, ob ALLE Typen Enums (oder Nullable(Of Enum)) sind
    Private Function AreAllEnumOrNullableEnum(types As Type()) As Boolean
        For Each t As Type In types
            If t.IsGenericType AndAlso t.GetGenericTypeDefinition() Is GetType(Nullable(Of )) Then
                t = Nullable.GetUnderlyingType(t)
            End If
            If t Is Nothing OrElse Not t.IsEnum Then Return False
        Next
        Return True
    End Function

    ' --- NEU: liefert alle INI-Properties mit Unterstrich, die INDEXER sind und deren Index-Parameter (alle) Enum/Nullable(Of Enum) sind
    Public Function GetIniIndexedPropertiesWithEnumIndex() As IEnumerable(Of PropertyInfo)
        Dim flags As BindingFlags = BindingFlags.Public Or BindingFlags.Static Or BindingFlags.DeclaredOnly
        Return GetType(INI).GetProperties(flags).
        Where(Function(p) p.CanRead AndAlso p.CanWrite).
        Where(Function(p) p.Name.Contains("_")).
        Where(Function(p)
                  Dim idx As ParameterInfo() = p.GetIndexParameters()
                  If idx Is Nothing OrElse idx.Length = 0 Then Return False
                  Return AreAllEnumOrNullableEnum(idx.Select(Function(pi) pi.ParameterType).ToArray())
              End Function)
    End Function

    ' --- NEU: kartesisches Produkt der Enumwerte über mehrere Index-Parameter (meist 1 Param).
    Public Iterator Function EnumerateIndexArgTuples(indexParamTypes As Type()) As IEnumerable(Of Object())
        Dim lists As New List(Of Object())()
        '
        For Each t As Type In indexParamTypes
            Dim isNullable As Boolean = t.IsGenericType AndAlso t.GetGenericTypeDefinition() Is GetType(Nullable(Of ))
            Dim nonNull As Type = If(isNullable, Nullable.GetUnderlyingType(t), t)
            Dim vals As System.Array = [Enum].GetValues(nonNull)
            ' Für Nullable(Of Enum) könntest du hier auch Nothing anbieten – in deinem Szenario nicht nötig.
            Dim objs(vals.Length - 1) As Object
            Dim i As Integer = 0
            For Each v As Object In vals
                objs(i) = v
                i += 1
            Next
            lists.Add(objs)
        Next

        ' kartesisches Produkt
        Dim idxCount As Integer = lists.Count
        Dim counters(idxCount - 1) As Integer
        Dim lengths As Integer() = lists.Select(Function(a) a.Length).ToArray()

        If idxCount = 0 Then
            Yield Array.Empty(Of Object)()
            Return
        End If

        While True
            Dim pick(idxCount - 1) As Object
            For i As Integer = 0 To idxCount - 1
                pick(i) = lists(i)(counters(i))
            Next
            Yield pick

            Dim pos As Integer = idxCount - 1
            While pos >= 0
                counters(pos) += 1
                If counters(pos) < lengths(pos) Then Exit While
                counters(pos) = 0
                pos -= 1
            End While
            If pos < 0 Then Exit While
        End While
    End Function

#End Region
#End Region
#End Region

End Module

