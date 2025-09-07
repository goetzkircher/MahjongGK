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


Imports System.Reflection

Public Module INI

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
    ''     Add
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

    Public ReadOnly BasisIni As IniManager
    Public ReadOnly Spieler1Ini As IniManager
    Public ReadOnly Spieler2Ini As IniManager


    Sub New()
        'Instanzen für verschiedene INIs hier anlegen
        BasisIni = New IniManager("Basis.ini")

        'Spieler1Ini = New IniManager("Spieler1.ini")
        'Spieler2Ini = New IniManager("Spieler2.ini")

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
    'Integer, Long, Single, Double, Decimal, Date, Enumerationen
    'Color, Point, PointF, Size, SizeF, Rectangle, RechtangleF
    'und Font

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
    'Public Property Kopier_VorlageBeispiel As TileSetInUse
    '    Get
    '        Dim [Default] As String = TileSetInUse.InternalSet.ToString
    '        Dim comment As String = Nothing
    '        Dim zRetVal As String = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
    '        Dim result As TileSetInUse
    '        If Not [Enum].TryParse(Of TileSetInUse)(zRetVal, True, result) Then
    '            result = TileSetInUse.InternalSet
    '        End If
    '        Return result
    '    End Get
    '    Set(value As TileSetInUse)
    '        BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value.ToString)
    '    End Set
    'End Property

#End Region

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
                                    "~Sie können diese Ini-Datei einfach löschen. Sie wird dann mit Default-Werten neu erzeugt." &
                                    "~Hinweis an Programmierer: Läuft das Programm in der IDE, ist im Hauptformular ganz links unten ein Button ""INI""." &
                                    "~Mit diesem Editor können Sie während der Laufzeit die INI ändern. Ein Teil der Änderungen bedürfen dennoch einen Neustart."

            Return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
        End Get
        Set(value As String)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
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
    Public Property IfRunningInIDE_InsertStoneIndex As Boolean
        Get
            If IsNothing(_IfRunningInIDE_InsertStoneIndex) Then
                Dim [Default] As Boolean = False
                Dim comment As String = Nothing
                _IfRunningInIDE_InsertStoneIndex = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
            End If

            'muss hier stehen, sonst wird der Wert nicht initialisiert und kann in der INI manuell nicht geändert werden.
            If Not Debugger.IsAttached() Then
                Return False
            End If

            Return CBool(_IfRunningInIDE_InsertStoneIndex)
        End Get
        Set(value As Boolean)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
            _IfRunningInIDE_InsertStoneIndex = Nothing
        End Set
    End Property

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
            Dim comment As String = "Nur in der IDE sichtbar gibt es in FrmMain unten zwei Buttons ""DwnLd"" und ""Gfx"", die" &
                                    "~den Windows Dateiexplorer mit dem Dowwnloadverzeichniss und dem Grafikverzeichniss öffnen." &
                                    "~Hier ist der lokale Pfad auf Ihrem Rechner einzugeben. Default: der Pfad auf meinem Rechner."

            Return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
        End Get
        Set(value As String)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
        End Set
    End Property
    Public Property IfRunningInIDE_Grafik16x16Directory_Other As String
        Get
            Dim [Default] As String = "C:\Users\goetz\Documents\Visual Studio\MahjongGK\Grafiken\Grafiken16x16_Andere"
            Dim comment As String = "Nur in der IDE sichtbar gibt es in FrmMain unten zwei Buttons ""DwnLd"" und ""Gfx"", die" &
                                    "~den Windows Dateiexplorer mit dem Dowwnloadverzeichniss und dem Grafikverzeichniss öffnen." &
                                    "~Hier ist der lokale Pfad auf Ihrem Rechner einzugeben. Default: der Pfad auf meinem Rechner."

            Return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
        End Get
        Set(value As String)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
        End Set
    End Property

    Public Property IfRunningInIDE_DownloadDirectory As String
        Get
            Dim [Default] As String = "C:\Users\goetz\Downloads\Vivaldi"
            Dim comment As String = "Gleiches Spiel mit dem Downloadverzeichnis." &
                                      "~Hinweis: Backslashes müssen verdoppelt werden, da sie Escape-Zeichen sind."
            Return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
        End Get
        Set(value As String)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
        End Set
    End Property

#End Region

#Region "Rendering"
    '  Public Enum TileSetInUse


    Public Property Rendering_TileSetInUse As TileSetInUse
        Get
            Dim [Default] As String = TileSetInUse.InternalSet.ToString
            Dim comment As String = "Das Programm ist vorgesehen für die Verwendung beliebiger und beliebig vieler Sätze an Mahjongsteinen" &
                                    "~in beliebigen Breiten/Höhenverhältnissen. Die Programmlogik ist noch nicht implementiert. Deshalb ist" &
                                    "~derzeit nur der Default möglich: ""InternalSet"" (Wenn implementiert, ändert sich dieser Text hier!)" &
                                    "~Wenn jemand Lust hat die Grafiken beizusteuern: MahjongGK@t-online.de"
            Dim zRetVal As String = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
            Dim result As TileSetInUse
            If Not [Enum].TryParse(Of TileSetInUse)(zRetVal, True, result) Then
                result = TileSetInUse.InternalSet
            End If
            Return result
        End Get
        Set(value As TileSetInUse)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value.ToString)
        End Set
    End Property

    Public Property Rendering_RenderTimerInterval As Integer
        Get
            Dim [Default] As Integer = 15
            Dim comment As String = "Normal 15 bis 20 (Einheit Millisekunden). Werte über 30 für schwache Rechner," &
                                    "~= 1 führt zu einem stabilerem Takt aller Timer auf dem Computer und zu etwas höherem Energieverbrauch." &
                                    "~Zu hohe Werte verlangsamen und verlängern die Animation. Default: 15"
            Return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
        End Get
        Set(value As Integer)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value.ToString)
        End Set
    End Property

    Private _Rendering_BitmapHighQuality As Boolean?
    Public Property Rendering_BitmapHighQuality As Boolean
        Get
            If IsNothing(_Rendering_BitmapHighQuality) Then
                Dim [Default] As Boolean = True
                Dim comment As String = "Wenn die Bildschirmausgabe auf langsamen Rechnern hakelt, versuchen Sie es mit False. Default = True."
                _Rendering_BitmapHighQuality = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
            End If
            Return CBool(_Rendering_BitmapHighQuality)
        End Get
        Set(value As Boolean)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
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

    Private _Rendering_OrgGrafikSizeWidth As Integer?
    Public Property Rendering_OrgGrafikSizeWidth As Integer
        Get
            If Not _Rendering_OrgGrafikSizeWidth.HasValue Then
                Dim [Default] As Integer = -1
                Dim comment As String = "Finger weg, wird vom Programm verwaltet."
                _Rendering_OrgGrafikSizeWidth = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
                _Rendering_OrgGrafikSizeWidth = Math.Max(60, Math.Min(600, _Rendering_OrgGrafikSizeWidth.Value))
            End If
            Return _Rendering_OrgGrafikSizeWidth.Value
        End Get
        Set(value As Integer)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
            _Rendering_OrgGrafikSizeWidth = Nothing
        End Set
    End Property

    Private _Rendering_OrgGrafikSizeHeight As Integer?
    Public Property Rendering_OrgGrafikSizeHeight As Integer
        Get
            If Not _Rendering_OrgGrafikSizeHeight.HasValue Then
                Dim [Default] As Integer = -1
                Dim comment As String = "Finger weg, wird vom Programm verwaltet."
                _Rendering_OrgGrafikSizeHeight = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
                _Rendering_OrgGrafikSizeHeight = Math.Max(60, Math.Min(600, _Rendering_OrgGrafikSizeHeight.Value))
            End If
            Return _Rendering_OrgGrafikSizeHeight.Value
        End Get
        Set(value As Integer)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
            _Rendering_OrgGrafikSizeHeight = Nothing
        End Set
    End Property

    Private _Rendering_OrgGrafikReferenceSizeWidth As Integer?
    Public Property Rendering_OrgGrafikReferenceSizeWidth As Integer
        Get
            If Not _Rendering_OrgGrafikReferenceSizeWidth.HasValue Then
                Dim [Default] As Integer = 198
                Dim comment As String = "Die Originalgröße der Grafiken bezieht das Programm aus den Grafiken selber. Die Referenzgröße bestimmt" &
                                        "~die maximale Größe der verwendeten Steine und das Seitenverhältniss. Default Breite: 198, Höhe: 252." &
                                        $"~Ist einer der Werte kleiner {MJ_GRAFIK_SRC_MIN_WIDTH_OR_HEIGHT}, werden die OrgGrafikSize-Werte genommen. Gültige Werte 0 bis {MJ_GRAFIK_SRC_MAX_WIDTH_OR_HEIGHT} Pixel." &
                                        "~Das Seitenverhältnis ist von 1:2 bis 2:1 begrenzt."

                _Rendering_OrgGrafikReferenceSizeWidth = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
                _Rendering_OrgGrafikReferenceSizeWidth = Math.Max(60, Math.Min(600, _Rendering_OrgGrafikReferenceSizeWidth.Value))
            End If
            Return _Rendering_OrgGrafikReferenceSizeWidth.Value
        End Get
        Set(value As Integer)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
            _Rendering_OrgGrafikReferenceSizeWidth = Nothing
        End Set
    End Property

    Private _Rendering_OrgGrafikReferenceSizeHeight As Integer?
    Public Property Rendering_OrgGrafikReferenceSizeHeight As Integer
        Get
            If Not _Rendering_OrgGrafikReferenceSizeHeight.HasValue Then
                Dim [Default] As Integer = 252
                Dim comment As String = Nothing
                _Rendering_OrgGrafikReferenceSizeHeight = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
                _Rendering_OrgGrafikReferenceSizeHeight = Math.Max(60, Math.Min(600, _Rendering_OrgGrafikReferenceSizeHeight.Value))
            End If
            Return _Rendering_OrgGrafikReferenceSizeHeight.Value
        End Get
        Set(value As Integer)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
            _Rendering_OrgGrafikReferenceSizeHeight = Nothing
        End Set
    End Property

    Private _Rendering_UseGrafikOrgSize As Boolean?
    Public Property Rendering_UseGrafikOrgSize As Boolean
        Get
            If IsNothing(_Rendering_UseGrafikOrgSize) Then
                Dim [Default] As Boolean = False
                Dim comment As String = "Wenn dieses Flag auf True steht, wird die maximale Größe und das Seitenverhältniss aus den Original-" &
                                        "~Abmessungen der Grafiken bezogen. Default: False"
                _Rendering_UseGrafikOrgSize = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
            End If
            Return CBool(_Rendering_UseGrafikOrgSize)
        End Get
        Set(value As Boolean)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
            _Rendering_UseGrafikOrgSize = Nothing
        End Set
    End Property

    Public ReadOnly Property Rendering_OrgGrafikUsedSizeWidth As Integer
        Get
            If Rendering_UseGrafikOrgSize Then
                Return Rendering_OrgGrafikSizeWidth
            Else
                Return Rendering_OrgGrafikReferenceSizeWidth
            End If
        End Get
    End Property

    Public ReadOnly Property Rendering_OrgGrafikUsedSizeHeight As Integer
        Get
            If Rendering_UseGrafikOrgSize Then
                Return Rendering_OrgGrafikSizeHeight
            Else
                Return Rendering_OrgGrafikReferenceSizeHeight
            End If
        End Get
    End Property


    Private _Rendering_Offset3DMaxX As Integer?
    Public Property Rendering_Offset3DMaxX As Integer
        Get
            If Not _Rendering_Offset3DMaxX.HasValue Then
                Dim [Default] As Integer = 30
                Dim comment As String = "Die Gesamt-Verschiebung eines 10 Steine hohen Stapels in X und Y Richtung in Pixel um den" &
                                        "~3D-Effekt zu erreichen, bei maximaler Steingröße. Erlaubt: -100 bis +100. Bei = 0 gibt es keinen" &
                                        "~3D-Effekt, wenn Offset3DMinPerLayerX/Y auch auf 0 steht. Default: 30"
                _Rendering_Offset3DMaxX = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
                _Rendering_Offset3DMaxX = Math.Max(-200, Math.Min(200, _Rendering_Offset3DMaxX.Value))
            End If
            Return _Rendering_Offset3DMaxX.Value
        End Get
        Set(value As Integer)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
            _Rendering_Offset3DMaxX = Nothing
        End Set
    End Property


    Private _Rendering_Offset3DMaxY As Integer?
    Public Property Rendering_Offset3DMaxY As Integer
        Get
            If Not _Rendering_Offset3DMaxY.HasValue Then
                Dim [Default] As Integer = 30
                Dim comment As String = Nothing
                _Rendering_Offset3DMaxY = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
                _Rendering_Offset3DMaxY = Math.Max(-60, Math.Min(60, _Rendering_Offset3DMaxY.Value))
            End If
            Return _Rendering_Offset3DMaxY.Value
        End Get
        Set(value As Integer)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
            _Rendering_Offset3DMaxY = Nothing
        End Set
    End Property

    Private _Rendering_Offset3DMinPerLayerX As Integer?
    Public Property Rendering_Offset3DMinPerLayerX As Integer
        Get
            If Not _Rendering_Offset3DMinPerLayerX.HasValue Then
                Dim [Default] As Integer = 1
                Dim comment As String = "Die Mindest-Verschiebung je Stein in Pixel, unabhängig von der Steingröße." &
                                        "~Erlaubt: 0 bis 5. Die 0 nur verwenden, wenn Rendering_Offset3DMaxX auch auf 0 steht. Default: 1"
                _Rendering_Offset3DMinPerLayerX = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
                _Rendering_Offset3DMinPerLayerX = Math.Max(0, Math.Min(5, _Rendering_Offset3DMinPerLayerX.Value))
            End If
            Return _Rendering_Offset3DMinPerLayerX.Value
        End Get
        Set(value As Integer)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
            _Rendering_Offset3DMinPerLayerX = Nothing
        End Set
    End Property


    Private _Rendering_Offset3DMinPerLayerY As Integer?
    Public Property Rendering_Offset3DMinPerLayerY As Integer
        Get
            If Not _Rendering_Offset3DMinPerLayerY.HasValue Then
                Dim [Default] As Integer = 1
                Dim comment As String = Nothing
                _Rendering_Offset3DMinPerLayerY = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
                _Rendering_Offset3DMinPerLayerY = Math.Max(0, Math.Min(5, _Rendering_Offset3DMinPerLayerY.Value))
            End If
            Return _Rendering_Offset3DMinPerLayerY.Value
        End Get
        Set(value As Integer)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
            _Rendering_Offset3DMinPerLayerY = Nothing
        End Set
    End Property

    '---------------------------
    Public ReadOnly Property Rendering_Offset3DFaktorAbsolutX As Double
        Get
            Try
                Return Math.Abs(Rendering_Offset3DMaxX / 10 / Rendering_OrgGrafikUsedSizeWidth)
            Catch ex As Exception
                Return 30 / 10 / 198
            End Try
        End Get
    End Property
    Public ReadOnly Property Rendering_Offset3DFaktorAbsolutY As Double
        Get
            Try
                Return Math.Abs(Rendering_Offset3DMaxY / 10 / Rendering_OrgGrafikUsedSizeHeight)
            Catch ex As Exception
                Return 30 / 10 / 252
            End Try
        End Get
    End Property

    Public ReadOnly Property Rendering_Offset3DFaktorSignX As Integer
        Get
            Try
                Return Math.Sign(Rendering_Offset3DMaxX)
            Catch ex As Exception
                Return 1
            End Try
        End Get
    End Property
    Public ReadOnly Property Rendering_Offset3DFaktorSignY As Integer
        Get
            Try
                Return Math.Sign(Rendering_Offset3DMaxY)
            Catch ex As Exception
                Return 1
            End Try
        End Get
    End Property


    Private _Rendering_RectOutputPaddingLeft As Integer?
    Public Property Rendering_RectOutputPaddingLeft As Integer
        Get
            If Not _Rendering_RectOutputPaddingLeft.HasValue Then
                Dim [Default] As Integer = 10
                Dim comment As String = "Breite des Innenrahmens um das Spielfeld. Erlaubt 0 bis 20. Default für alle Werte: 10"
                _Rendering_RectOutputPaddingLeft = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
                _Rendering_RectOutputPaddingLeft = Math.Max(0, Math.Min(20, _Rendering_RectOutputPaddingLeft.Value))
            End If
            Return _Rendering_RectOutputPaddingLeft.Value
        End Get
        Set(value As Integer)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
            _Rendering_RectOutputPaddingLeft = Nothing
        End Set
    End Property

    Private _Rendering_RectOutputPaddingRight As Integer?
    Public Property Rendering_RectOutputPaddingRight As Integer
        Get
            If Not _Rendering_RectOutputPaddingRight.HasValue Then
                Dim [Default] As Integer = 10
                Dim comment As String = Nothing
                _Rendering_RectOutputPaddingRight = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
                _Rendering_RectOutputPaddingRight = Math.Max(0, Math.Min(20, _Rendering_RectOutputPaddingRight.Value))
            End If
            Return _Rendering_RectOutputPaddingRight.Value
        End Get
        Set(value As Integer)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
            _Rendering_RectOutputPaddingRight = Nothing
        End Set
    End Property

    Private _Rendering_RectOutputPaddingTop As Integer?
    Public Property Rendering_RectOutputPaddingTop As Integer
        Get
            If Not _Rendering_RectOutputPaddingTop.HasValue Then
                Dim [Default] As Integer = 10
                Dim comment As String = Nothing
                _Rendering_RectOutputPaddingTop = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
                _Rendering_RectOutputPaddingTop = Math.Max(0, Math.Min(20, _Rendering_RectOutputPaddingTop.Value))
            End If
            Return _Rendering_RectOutputPaddingTop.Value
        End Get
        Set(value As Integer)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
            _Rendering_RectOutputPaddingTop = Nothing
        End Set
    End Property

    Private _Rendering_RectOutputPaddingBottom As Integer?
    Public Property Rendering_RectOutputPaddingBottom As Integer
        Get
            If Not _Rendering_RectOutputPaddingBottom.HasValue Then
                Dim [Default] As Integer = 10
                Dim comment As String = Nothing
                _Rendering_RectOutputPaddingBottom = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
                _Rendering_RectOutputPaddingBottom = Math.Max(0, Math.Min(20, _Rendering_RectOutputPaddingBottom.Value))
            End If
            Return _Rendering_RectOutputPaddingBottom.Value
        End Get
        Set(value As Integer)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
            _Rendering_RectOutputPaddingBottom = Nothing
        End Set
    End Property

    Public Event Rendering_AktMaxSteineXYZ_Event()
    Public Property Rendering_AktMaxSteineX As Integer
        Get
            Dim [Default] As Integer = 0
            Dim comment As String = "Finger weg, werden vom Programm verwaltet."
            Return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
        End Get
        Set(value As Integer)
            If BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value.ToString) Then
                RaiseEvent Rendering_AktMaxSteineXYZ_Event()
            End If
        End Set
    End Property

    Public Property Rendering_AktMaxSteineY As Integer
        Get
            Dim [Default] As Integer = 0
            Dim comment As String = Nothing
            Return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
        End Get
        Set(value As Integer)
            If BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value.ToString) Then
                RaiseEvent Rendering_AktMaxSteineXYZ_Event()
            End If
        End Set
    End Property

    Public Property Rendering_AktMaxSteineZ As Integer
        Get
            Dim [Default] As Integer = 0
            Dim comment As String = Nothing
            Return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
        End Get
        Set(value As Integer)
            If BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value.ToString) Then
                RaiseEvent Rendering_AktMaxSteineXYZ_Event()
            End If
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
            Dim comment As String = "Mit False läßt sich der Editor komplett abschalten. Default: True"
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

    Public Property Editor_VerhältnisNormalsteineZuSondersteine As Double
        Get
            Dim [Default] As Double = 17
            Dim comment As String = "Sondersteine sind die 4 Blumen und die 4 Jahreszeiten." &
                                    "~Hiermit wird gesteuert, auf wieviel Normalsteinpaare ein Sondersteinpaar kommt." &
                                    "~Sollen alle Steine gleichhäufig vorkommen, ist der Wert 17, sollen die Sondersteine" &
                                    "~nur halb so häufig vorkommen ist der Wert 34. Default: 17"
            Return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
        End Get
        Set(value As Double)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value.ToString)
        End Set
    End Property



    Public Property Editor_VorratNoSortAreaEndIndexDefault As Integer
        Get
            Dim [Default] As Integer = 9
            Dim comment As String = "Im Editor läßt sich die Vorratskiste jederzeit neu mischen. Davon ausgenommen sind die Steine bis zum" &
                                    "~hier angegebenem Index. Default: 9 (=10 Steine), abschalten mit -1"
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
    Public Property Editor_GeneratorModusDefault As GeneratorModi
        Get
            Dim [Default] As String = GeneratorModi.StoneSet_144.ToString
            Dim comment As String = Nothing
            Dim zRetVal As String = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
            Dim aRetVal As GeneratorModi = CType(System.Enum.Parse(aRetVal.GetType(), zRetVal), GeneratorModi)
            Return aRetVal
        End Get
        Set(value As GeneratorModi)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value.ToString)
        End Set
    End Property


    Public Property Editor_VorratMaxUBoundDefault As Integer
        Get
            Dim [Default] As Integer = MJ_STEINE_VORRATMAXDEFAULT
            Dim comment As String = "Die Anzahl der Steine in der Vorratskiste die ""pro Portion"" erzeugt werden. Default: " & MJ_STEINE_VORRATMAXDEFAULT.ToString

            'Rückgabe 
            Return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
        End Get
        Set(value As Integer)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value.ToString)
        End Set
    End Property

    Public Property Editor_VorratNachschubschwelleDefault As Integer
        Get
            Dim [Default] As Integer = MJ_STEINE_VORRATNACHSCHUBSCHWELLEDEFAULT
            Dim comment As String = "Unterschreitet die Anzahl der Steine in der Vorratskiste diesen Wert, wird Nachschub erzeugt. Default: " & MJ_STEINE_VORRATNACHSCHUBSCHWELLEDEFAULT.ToString
            'Rückgabe 
            Return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
        End Get
        Set(value As Integer)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value.ToString)
        End Set
    End Property

    Public Property Editor_SteinGeneratorDebugMode As Integer
        Get
            Dim [Default] As Integer = 0
            Dim comment As String = "Mit einer bliebigen Zahl <> 0 erzeugt der SteinGenerator immer wieder die gleichen Steinfolgen." &
                                    "~Zum Austesten gedacht. Default = 0 (normaler Spielbetrieb)"
            'Rückgabe 
            Return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
        End Get
        Set(value As Integer)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value.ToString)
        End Set
    End Property

#End Region

#Region "Spielfeld"

    Private _Spielfeld_DrawBackgroundBitmap As Boolean?
    Public Property Spielfeld_DrawBackgroundBitmap As Boolean
        Get
            If IsNothing(_Spielfeld_DrawBackgroundBitmap) Then
                Dim [Default] As Boolean = False
                Dim comment As String = Nothing
                _Spielfeld_DrawBackgroundBitmap = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
            End If
            Return CBool(_Spielfeld_DrawBackgroundBitmap)
        End Get
        Set(value As Boolean)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
            _Spielfeld_DrawBackgroundBitmap = Nothing
        End Set
    End Property

    Private _Spielfeld_BackgroundBitmapIndex As Integer?
    Public Property Spielfeld_BackgroundBitmapIndex As Integer
        Get
            If Not _Spielfeld_BackgroundBitmapIndex.HasValue Then
                Dim [Default] As Integer = 0
                Dim comment As String = Nothing
                _Spielfeld_BackgroundBitmapIndex = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
            End If
            Return _Spielfeld_BackgroundBitmapIndex.Value
        End Get
        Set(value As Integer)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
            _Spielfeld_BackgroundBitmapIndex = Nothing
        End Set
    End Property

    Private _Spielfeld_DrawBackgroundColor As Boolean?
    Public Property Spielfeld_DrawBackgroundColor As Boolean
        Get
            If IsNothing(_Spielfeld_DrawBackgroundColor) Then
                Dim [Default] As Boolean = True
                Dim comment As String = Nothing
                _Spielfeld_DrawBackgroundColor = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
            End If
            Return CBool(_Spielfeld_DrawBackgroundColor)
        End Get
        Set(value As Boolean)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
            _Spielfeld_DrawBackgroundColor = Nothing
        End Set
    End Property

    Private _Spielfeld_BackgroundColor As Color
    Public Property Spielfeld_BackgroundColor As Color
        Get
            If _Spielfeld_BackgroundColor.IsEmpty Then
                Dim [Default] As Color = IniManager.CvtHexStringToColor("FFC0C0C0")
                Dim comment As String = Nothing
                _Spielfeld_BackgroundColor = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
            End If
            Return _Spielfeld_BackgroundColor
        End Get
        Set(value As Color)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
            _Spielfeld_BackgroundColor = Color.Empty
        End Set
    End Property


    Private _Spielfeld_DrawFraming As Boolean?
    Public Property Spielfeld_DrawFraming As Boolean
        Get
            If IsNothing(_Spielfeld_DrawFraming) Then
                Dim [Default] As Boolean = True
                Dim comment As String = Nothing
                _Spielfeld_DrawFraming = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
            End If
            Return CBool(_Spielfeld_DrawFraming)
        End Get
        Set(value As Boolean)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
            _Spielfeld_DrawFraming = Nothing
        End Set
    End Property

    Private _Spielfeld_FramingColor As Color
    Public Property Spielfeld_FramingColor As Color
        Get
            If _Spielfeld_FramingColor.IsEmpty Then
                Dim [Default] As Color = Color.Black
                Dim comment As String = Nothing
                _Spielfeld_FramingColor = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
            End If
            Return _Spielfeld_FramingColor
        End Get
        Set(value As Color)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
            _Spielfeld_FramingColor = Color.Empty
        End Set
    End Property

    Private _Spielfeld_FramingThickness As Single?
    Public Property Spielfeld_FramingThickness As Single
        Get
            If Not _Spielfeld_FramingThickness.HasValue Then
                Dim [Default] As Single = 2
                Dim comment As String = Nothing
                _Spielfeld_FramingThickness = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
            End If
            Return _Spielfeld_FramingThickness.Value
        End Get
        Set(value As Single)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
            _Spielfeld_FramingThickness = Nothing
        End Set
    End Property

    Private _Spielfeld_DrawRenderRect As Boolean?
    Public Property Spielfeld_DrawRenderRect As Boolean
        Get
            If IsNothing(_Spielfeld_DrawRenderRect) Then
                Dim [Default] As Boolean = Debugger.IsAttached
                Dim comment As String = Nothing
                _Spielfeld_DrawRenderRect = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
            End If
            Return CBool(_Spielfeld_DrawRenderRect)
        End Get
        Set(value As Boolean)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
            _Spielfeld_DrawRenderRect = Nothing
        End Set
    End Property

    Private _Spielfeld_RenderRectColor As Color
    Public Property Spielfeld_RenderRectColor As Color
        Get
            If _Spielfeld_RenderRectColor.IsEmpty Then
                Dim [Default] As Color = Color.Black
                Dim comment As String = Nothing
                _Spielfeld_RenderRectColor = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
            End If
            Return _Spielfeld_RenderRectColor
        End Get
        Set(value As Color)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
            _Spielfeld_RenderRectColor = Color.Empty
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


    Public Property Spielbetrieb_WindsAreInOneClickGroup As Boolean
        Get
            Dim [Default] As Boolean = False
            Dim comment As String = "Wenn True ist das eine starke Spielregelvereinfachung:" &
                                    "~Die 4 Winde können in beliebiger Kombination paarweise entnommen werden."
            Return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
        End Get
        Set(value As Boolean)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value.ToString)
        End Set
    End Property
    Public Property Spielbetrieb_ShowSelectableStones As Boolean
        Get
            Dim [Default] As Boolean = False
            Dim comment As String = "Wenn True werden alle selektierbaren Steine in anderer Farbe dargestellt. Default: False"
            Return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
        End Get
        Set(value As Boolean)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value.ToString)
        End Set
    End Property

#End Region

#Region "InfoMessageBox"

    Public Property InfoMessageBox_FontHeader As Font
        Get
            Dim [Default] As New Font("Segoe UI", 12.0F, FontStyle.Bold)
            Dim comment As String = "Andere serifenlose Standardschriften: Arial, Segoe UI, Calibri, Tahoma, Verdana, sans-serif. Default Segoe UI;12;Bold"
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
            Dim comment As String = "Andere diktengleiche Fonts: Cascadia Mono, Consolas, Lucida Console, Courier New, monospace. Default: Cascadia Mono;12;Bold"
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
                                    "~Sind dort mehr als hier angegeben, werden die Ältesten beim nächsten ScreenShot gelöscht. Default = 20"


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
            Dim [Default] As String = AppGrafikSatz.Default.ToString
            Dim comment As String = "Das Programm ist vorgesehen für die Verwendung beliebiger und beliebig vieler Sätze an 16x16 Grafiken" &
                                    "~für die verschiedenen Buttons im Programm. Die Programmlogik ist noch nicht implementiert und es mangelt" &
                                    "~an Grafiken. Derzeit ist nur der Default möglich: ""Default"" (Wenn implementiert, ändert sich dieser Text hier!)" &
                                    "~Wenn jemand Lust hat die Grafiken beizusteuern: MahjongGK@t-online.de"
            Dim zRetVal As String = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
            Dim result As AppGrafikSatz
            If Not [Enum].TryParse(Of AppGrafikSatz)(zRetVal, True, result) Then
                result = AppGrafikSatz.Default
            End If
            Return result
        End Get
        Set(value As AppGrafikSatz)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value.ToString)
        End Set
    End Property

#End Region

#Region "Toolbox"
    '
    ''' <summary>
    ''' Speicherung der aktuellen Position der Form Toolbox
    ''' </summary>
    ''' <returns></returns>
    Public Property ToolBox_Rectangle As Rectangle
        Get
            Dim [Default] As New Rectangle(100, 100, frmToolBox.MJ_FRMTOOLBOX_WIDTH, frmToolBox.MJ_FRMTOOLBOX_HEIGHT) '
            Dim comment As String = Nothing
            Dim rc As Rectangle = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)

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
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
        End Set
    End Property


    Public Property ToolBox_FeldSizeXmax() As Integer
        Get
            Dim [Default] As Integer = 30
            Dim comment As String = $"Maximale Anzahl der Steine nebeneinander. Gültig: 1 bis {MJ_STEINE_MAXX_SIDEBYSIDE}, Default = 30"
            Dim value As Integer = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
            Return Math.Max(1, Math.Min(MJ_STEINE_MAXX_SIDEBYSIDE, value))
        End Get
        Set(value As Integer)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value.ToString)
        End Set
    End Property
    Public Property ToolBox_FeldSizeYmax() As Integer
        Get
            Dim [Default] As Integer = 15
            Dim comment As String = $"Maximale Anzahl der Steine übereinander. Gültig: 1 bis {MJ_STEINE_MAXY_OVERANOTHER}, Default = 15"
            Dim value As Integer = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
            Return Math.Max(1, Math.Min(MJ_STEINE_MAXY_OVERANOTHER, value))
        End Get
        Set(value As Integer)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value.ToString)
        End Set
    End Property
    Public Property ToolBox_FeldSizeZmax() As Integer
        Get
            Dim [Default] As Integer = 10
            Dim comment As String = $"Maximale Anzahl der Steine aufeinander. Gültig: 1 bis {MJ_STEINE_MAXZ_LAYER}, Default = 10"
            Dim value As Integer = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
            Return Math.Max(1, Math.Min(MJ_STEINE_MAXZ_LAYER, value))
        End Get
        Set(value As Integer)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value.ToString)
        End Set
    End Property
    Public Property ToolBox_FeldSizeX(bform As BasisformEnum) As Integer
        Get
            Dim [Default] As Integer = 10
            Dim comment As String = Nothing
            If bform = 0 Then
                comment = "Die jeweiligen aktuellen Werte der Basisformen in der Toolbox. Default für X = 10, Y = 10, Z = 5"
            End If
            Dim value As Integer = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name, bform.ToString), [Default], comment)
            Return Math.Max(1, Math.Min(MJ_STEINE_MAXX_SIDEBYSIDE, value))
        End Get
        Set(value As Integer)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name, bform.ToString), value.ToString)
        End Set
    End Property
    Public Property ToolBox_FeldSizeY(bform As BasisformEnum) As Integer
        Get
            Dim [Default] As Integer = 10
            Dim comment As String = Nothing
            Dim value As Integer = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name, bform.ToString), [Default], comment)
            Return Math.Max(1, Math.Min(MJ_STEINE_MAXX_SIDEBYSIDE, value))
        End Get
        Set(value As Integer)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name, bform.ToString), value.ToString)
        End Set
    End Property
    Public Property ToolBox_FeldSizeZ(bform As BasisformEnum) As Integer
        Get
            Dim [Default] As Integer = 5
            Dim comment As String = Nothing
            Dim value As Integer = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name, bform.ToString), [Default], comment)
            Return Math.Max(1, Math.Min(MJ_STEINE_MAXZ_LAYER, value))
        End Get
        Set(value As Integer)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name, bform.ToString), value.ToString)
        End Set
    End Property


    Public Property ToolBox_NameBasisformForSaving(bform As BasisformEnum) As String
        Get
            Dim [Default] As String = bform.ToString & "_1"
            Dim comment As String = Nothing
            Return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name, bform.ToString), [Default], comment)
        End Get
        Set(value As String)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name, bform.ToString), value.ToString)
        End Set
    End Property

    Public Property ToolBox_AktBasisform As BasisformEnum
        Get
            Dim [Default] As String = BasisformEnum.Pyramide.ToString
            Dim comment As String = Nothing
            Dim zRetVal As String = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
            Dim result As BasisformEnum
            If Not [Enum].TryParse(Of BasisformEnum)(zRetVal, True, result) Then
                result = BasisformEnum.Pyramide
            End If
            Return result
        End Get
        Set(value As BasisformEnum)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value.ToString)
        End Set
    End Property

#End Region

#Region "RuntimeOnly"

    Public Event RuntimeOnly_AktRendering_Event()
    Private _RuntimeOnly_AktRendering As RenderingEnum
    Public Property RuntimeOnly_AktRendering As RenderingEnum
        Get
            Return _RuntimeOnly_AktRendering
        End Get
        Set(value As RenderingEnum)
            _RuntimeOnly_AktRendering = value
            RaiseEvent RuntimeOnly_AktRendering_Event()
        End Set
    End Property

    Public Event RuntimeOnly_ToolboxAktiv_Event()
    Private _RuntimeOnly_ToolboxAktiv As Boolean
    Public Property RuntimeOnly_ToolboxAktiv As Boolean
        Get
            Return _RuntimeOnly_ToolboxAktiv
        End Get
        Set(value As Boolean)
            _RuntimeOnly_ToolboxAktiv = value
            RaiseEvent RuntimeOnly_ToolboxAktiv_Event()
        End Set
    End Property

#End Region

#Region "EventsOnly"

    Public Event EventsOnly_RefreshUINachIniÄnderung_Event()

    Public Sub EventsOnly_RefreshUINachIniÄnderung()
        RaiseEvent Rendering_AktMaxSteineXYZ_Event()
        RaiseEvent EventsOnly_RefreshUINachIniÄnderung_Event()
    End Sub

#End Region

#Region "Ini Editieren"

    ''' <summary>
    ''' Zum Editieren der INI während des laufenden Betriebes.
    ''' 
    ''' </summary>
    Public Sub IniEditieren()

        Spielfeld.PaintSpielfeld_BeginPause()

        Using f As New FrmIniEditor()

            f.ShowDialog()

            If f.IniFileChanged Then
                Spielfeld.PaintSpielfeld_EndPause(startIniUpdate:=True, raiseIniEvents:=IniEvents.OnUpdate)
            Else
                Spielfeld.PaintSpielfeld_EndPause(startIniUpdate:=False)
            End If

        End Using

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
    ''' maxFiles arbeitet nur in Verbindung mit timestamp = AppDataTimeStamp.Add und räumt
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
    ''' maxFiles arbeitet nur in Verbindung mit timestamp = AppDataTimeStamp.Add und räumt
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
    ''' maxFiles arbeitet nur in Verbindung mit timestamp = AppDataTimeStamp.Add und räumt
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
    ''' maxFiles arbeitet nur in Verbindung mit timestamp = AppDataTimeStamp.Add und räumt
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
    ''' maxFiles arbeitet nur in Verbindung mit timestamp = AppDataTimeStamp.Add und räumt
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
    ''' maxFiles arbeitet nur in Verbindung mit timestamp = AppDataTimeStamp.Add und räumt
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

#Region "Initialisierung"


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


#Region "Initialisierung der Default-Werte"

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
                    entry.Prop.SetValue(Nothing, CoerceForProperty(entry.Prop, entry.Value), Nothing)
                Next

                ' 5b) Indexierte Werte zurückschreiben
                For Each entry As (Prop As PropertyInfo, IndexArgs As Object(), Value As Object) In newValuesIndexed
                    entry.Prop.SetValue(Nothing, CoerceForProperty(entry.Prop, entry.Value), entry.IndexArgs)
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
    Private Function GetIniPropertiesWithUnderscore() As IEnumerable(Of PropertyInfo)

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

    ' --- NEU: Helfer, der Werte auf den Property-Typ formt (inkl. Enum & Nullable(Of Enum)) ---
    Private Function CoerceForProperty(prop As PropertyInfo, value As Object) As Object
        If value Is Nothing Then Return Nothing

        Dim targetType As Type = prop.PropertyType
        Dim isNullable As Boolean =
            targetType.IsGenericType AndAlso targetType.GetGenericTypeDefinition() Is GetType(Nullable(Of ))
        Dim nonNullType As Type = If(isNullable, Nullable.GetUnderlyingType(targetType), targetType)

        ' Bereits passend?
        If nonNullType.IsAssignableFrom(value.GetType()) Then
            Return value
        End If

        ' ENUMS (auch Flags, auch Nullable(Of Enum))
        If nonNullType.IsEnum Then
            ' String → Enum.Parse (IgnoreCase, erlaubt auch "A, B" bei <Flags>-Enums)
            If TypeOf value Is String Then
                Dim s As String = DirectCast(value, String)
                If String.IsNullOrWhiteSpace(s) Then
                    Return If(isNullable, Nothing, Activator.CreateInstance(nonNullType))
                End If
                Return [Enum].Parse(nonNullType, s, ignoreCase:=True)
            End If

            ' Numerisch → Enum.ToObject
            If IsNumeric(value) Then
                Dim raw As Long = Convert.ToInt64(value, Globalization.CultureInfo.InvariantCulture)
                Return [Enum].ToObject(nonNullType, raw)
            End If

            ' Fallback: Wenn der Wert sich z. B. als anderes Enum mit gleicher Basiszahl darstellt
            Try
                Dim raw As Long = Convert.ToInt64(value, Globalization.CultureInfo.InvariantCulture)
                Return [Enum].ToObject(nonNullType, raw)
            Catch
                ' Ignorieren → weiter unten allgemeiner Fallback
            End Try
        End If

        ' Alle anderen (nur wenn sinnvoll konvertierbar; Color/Point/... lassen wir unangetastet)
        Try
            If isNullable Then
                Return Convert.ChangeType(value, nonNullType, Globalization.CultureInfo.InvariantCulture)
            Else
                Return Convert.ChangeType(value, targetType, Globalization.CultureInfo.InvariantCulture)
            End If
        Catch
            ' Wenn nicht wandelbar, Original zurückgeben (SetValue würde sonst selbst aussteigen)
            Return value
        End Try
    End Function

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
    Private Function GetIniIndexedPropertiesWithEnumIndex() As IEnumerable(Of PropertyInfo)
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
    Private Iterator Function EnumerateIndexArgTuples(indexParamTypes As Type()) As IEnumerable(Of Object())
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

