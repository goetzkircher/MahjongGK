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
Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

#Disable Warning IDE0079
#Disable Warning IDE1006


'Zentrale Deklaration programmweit geltender Enumerationen

''' <summary>
''' Enumeration der verwendeten Unterverzeichnisse in "C:\Users\aktueller User\MahjongGK\SubDefault.value.ToString"
''' Verwendung: Entweder über Dim Path As String = INI.AppDataDefault(.....)
''' oder durch Nutzung (im Modul INI) einer Public Property Kopier_VorlageFürPfade As String
''' Die gewünschten Pfade werden automatisch angelegt.
''' </summary>
Public Enum AppDataSubDir
    None
    INI
    Steine198x252
    Steine198x252Layout
    Grafiken
    Diverses
End Enum
''' <summary>
''' Enumeration der verwendeten Unterverzeichnisse in "C:\Users\aktueller User\MahjongGK\SubDefault.value.ToString\SubSubDefault.value.ToString"
''' Verwendung wie AppDataSubDir
''' </summary>
Public Enum AppDataSubSubDir
    None
    letztesSpiel
    Layout
    Diverses_ScreenShots
    Grafiken_AppGrafiken16x16
End Enum

''' <summary>
''' Enumeration der verwendeten Dateinamen.
''' Die Endung mit einem Unterstrich abtrennen.
''' Die Endung muss 3 Zeichen lang sein.
''' </summary>
Public Enum AppDataFileName
    None
    Steininfos_xml
    ScreeenShot_png
End Enum

Public Enum AppDataTimeStamp
    None
    Add
    LookForLastTimeStamp
End Enum

''' <summary>
''' In dieser Enum kann ein Pattern verschlüsselt werden.
''' Es gilt: 
''' _Q_ = ? (Question, Fragezeichen),
''' _N_ = # (Number),
''' _S_ = * (Stern, Star),
''' _D_ = . (Dot, Punkt).
''' _SD_ = *.
''' _SDS_ = *.*
''' Beispiel: Dateiname_S__D_ext --> Dateiname*.ext
''' </summary>
Public Enum AppDataFilePattern
    None
    Steininfos_xml
End Enum

''' <summary>
''' Auf englisch, weil in der INI
''' Die verwendeten Steinsätze
''' </summary>
Public Enum TileSetInUse
    InternalSet
End Enum
'
''' <summary>
''' Für die Grafiken 16x16 im Programm.
''' Der Default-Datensatz ist in Me.Ressources hinterlegt.
''' </summary>
Public Enum AppGrafikSatz
    [Default] ' Ressourcen
    Satz1     ' Beispiel; echte Namen später
    Satz2
    ' ...
End Enum
'
''' <summary>
''' 
''' </summary>
Public Enum AppGrafikName
    ErrorGrafik ' Notnagel; MUSS in Ressourcen vorhanden sein
    Editor
    EditorAktiv
    PfeilDn
    PfeilUp
    Redo
    Restart
    Screenshot
    ShowSelectableChecked
    ShowSelectableUnChecked
    Spieler
    SpielerAktiv
    Statistik
    Tip
    Tipps
    Undo
    Werkbank
    WerkbankAktiv
    Werkzeug
    WerkzeugAktiv
    WindsChecked
    WindsUnChecked
End Enum

Public Enum IniEvents
    None
    OnChangeValue
    OnWriteValue
    OnUpdate
End Enum


''' <summary>
''' Enumeration der Steine
''' </summary>
Public Enum SteinIndexEnum
    ErrorSy
    Punkt01
    Punkt02
    Punkt03
    Punkt04
    Punkt05
    Punkt06
    Punkt07
    Punkt08
    Punkt09
    Bambus1
    Bambus2
    Bambus3
    Bambus4
    Bambus5
    Bambus6
    Bambus7
    Bambus8
    Bambus9
    Symbol1
    Symbol2
    Symbol3
    Symbol4
    Symbol5
    Symbol6
    Symbol7
    Symbol8
    Symbol9
    DracheR
    DracheG
    DracheW
    WindOst
    WindSüd
    WindWst
    WindNrd
    BlütePf
    BlüteOr
    BlüteCt
    BlüteBa
    JahrFrl
    JahrSom
    JahrHer
    JahrWin
End Enum



'
' StoneStream
' |Base144              
' ||Count HightBit
' |||Count LowBit
' ||||
' 0000 = 0   = GeneratorModi.StoneStream_Base144_Continuous    unendlicher Steinstrom mit 144er Steinverteilung
' 0001 = 1   = GeneratorModi.StoneStream_Base144_Low           Der zahlenmäßige Wert von Low, Medium und High 
' 0010 = 2   = GeneratorModi.StoneStream_Base144_Medium        ist in der INI hinterlegt und fei definierbar.
' 0011 = 3   = GeneratorModi.StoneStream_Base144_Height
' |Base152
' ||Count HightBit
' |||Count LowBit
' ||||  
' 0100 = 4    = GeneratorModi.StoneStream_Base152_Continuous   unendlicher Steinstrom mit 152er Steinverteilung 
' 0101 = 5    = GeneratorModi.StoneStream_Base152_Low
' 0110 = 6    = GeneratorModi.StoneStream_Base152_Medium
' 0111 = 7    = GeneratorModi.StoneStream_Base152_Heigh
'
' StoneSet
' |Base144 
' ||Count HightBit
' |||Count LowBit
' ||||
' 1000 = 8    = GeneratorModi.StoneSet_072  = 1 * 72
' 1001 = 9    = GeneratorModi.StoneSet_144  = 2 * 72
' 1010 = 10   = GeneratorModi.StoneSet_216  = 3 * 72
' 1011 = 11   = GeneratorModi.StoneSet_288  = 4 * 72
' |Base152
' ||Count HightBit
' |||Count LowBit
' ||||
' 1100 = 12   = GeneratorModi.StoneSet_076  = 1 * 76
' 1101 = 13   = GeneratorModi.StoneSet_152  = 2 * 76
' 1110 = 14   = GeneratorModi.StoneSet_228  = 3 * 76
' 1111 = 15   = GeneratorModi.StoneSet_304  = 4 * 76
'
' Konvertierung siehe: (im Spielsteingenerator)
' Public Function GetGeneratorModi(isStoneSet As Boolean,
'                                     isBase152 As Boolean,
'                                     count As Integer) As GeneratorModi
' Public Function GetValueFromGeneratorModi(genmod As GeneratorModi) As (isStoneSet As Boolean,
'                                                                            isBase152 As Boolean,
'                                                                            count As Integer)

Public Enum GeneratorModi
    StoneStream_Base144_Continuous = 0
    StoneStream_Base152_Continuous = 4
    StoneStream_Base144_Low = 1
    StoneStream_Base152_Low = 5
    StoneStream_Base144_Medium = 2
    StoneStream_Base152_Medium = 6
    StoneStream_Base144_Height = 3
    StoneStream_Base152_Height = 7
    StoneSet_072 = 8
    StoneSet_076 = 12
    StoneSet_144 = 9
    StoneSet_152 = 13
    StoneSet_216 = 10
    StoneSet_228 = 14
    StoneSet_288 = 11
    StoneSet_304 = 15
End Enum




Public Enum SteinRndGruppe
    Normal
    Flower
    Season
End Enum

Public Enum SteinStatus
    ''' <summary>
    ''' Wenn das Programm innerhalb der IDE läuft, kann das Programmverhalten
    ''' über den Schalter "unsichtbare Steine sichtbar machen" geändert werden.
    ''' Es werden dann halbtransparente graue Steine mit Rotem Kreis und Indexnummer
    ''' angezeigt.
    ''' </summary>
    Unsichtbar          ' nicht sichtbar (Geistergrafik möglich)
    Normal
    Selected
    ClickableOne        ' einzeln klickbar
    ClickablePartOfPair ' klickbar und Teil eines gültigen Paars
    Locked
    NotUnsed            ' nur für Schwierigkeitslevel-Auswahl
    MissingSecond       ' im Editor, wenn Partnerstein fehlt
    WerkstückEinfügeFehler
    WerkstückZufallsgrafik
    Reserve1            ' Geistergrafik mit grünem Kreis
    Reserve2            ' Geistergrafik mit blauem Kreis
End Enum

Public Enum VisiblePart
    none ' nicht sichtbar
    OL ' oben links, linke oberstes Viertel sichtbar
    OM ' oben Mitte, obere Hälfte sichtbar
    [OR] ' oben rechts, rechtes oberstes Viertel sichtbar
    ML ' Mitte links, linke Hälfte sichtbar
    MM ' Mitte Mitte, alles sichtbar
    MR ' Mitte rechts, rechte Hälfte sichtbar
    UL ' unten links, linkes unterstes Viertel sichtbar
    UM ' unten Mitte, untere Hälfte sichtbar
    UR ' unten rechts, rechtes unterstes Viertel sichtbar
End Enum
'
''' <summary>
''' Richtung der Suche nach einem freiem Platz für einen
''' Stein ausgehend von einer Ausgangsstellung.
''' </summary>
Public Enum Direction
    Up
    UpRight
    Right
    DownRight
    Down
    DownLeft
    Left
    UpLeft
    RightUp = UpRight
    RightDown = DownRight
    LeftDown = DownLeft
    LeftUp = UpLeft
    LBnd = Up    'um in einer ForNext-Schleife alle Himmelsrichtungen durchzugehen.
    UBnd = UpLeft
End Enum

Public Enum Move
    NoMove
    LeftOrUp
    RightOrDown
End Enum


Public Enum PositionEnum
    '
    ''' <summary>
    ''' Die linke obere Ecke des Spielfeldes
    ''' </summary>
    EckeLO
    '
    ''' <summary>
    ''' Die rechte obere Ecke des Spielfeld
    ''' </summary>
    EckeRO
    '
    ''' <summary>
    ''' Die rechte untere Ecke des Spielfeldes
    ''' </summary>
    EckeRU
    '
    ''' <summary>
    ''' Die linke untere Ecke des Spielfeldes
    ''' </summary>
    EckeLU
    '
    ''' <summary>
    ''' Die Mitte oben des Spielfeldes
    ''' </summary>
    MitteO
    '
    ''' <summary>
    ''' Die Mitte rechts des Spielfeldes
    ''' </summary>
    MitteR
    '
    ''' <summary>
    ''' Die Mitte unten des Spielfeldes
    ''' </summary>
    MitteU
    '
    ''' <summary>
    ''' Die Mitte Links des Spielfeldes
    ''' </summary>
    MitteL
    '
    ''' <summary>
    ''' Vom Zentrum nach links oben mittig
    ''' </summary>
    CenterLO
    '
    ''' <summary>
    ''' Vom Zentrum nach rechts oben mittig
    ''' </summary>
    CenterRO
    '
    ''' <summary>
    ''' Vom Zentrum nach rechts unten mittig
    ''' </summary>
    CenterRU
    '
    ''' <summary>
    ''' Vom Zentrum nach links unten mittig
    ''' </summary>
    CenterLU
    '
    ''' <summary>
    ''' Für For-Next Schleifen um geradzahlige Schleifenwerte zu erreichen.
    ''' (Paarweise Steinverlegung)
    ''' </summary>
    UBndWithoutCenter = CenterLU
    '
    ''' <summary>
    ''' Die Mitte rechts des Spielfeldes
    ''' </summary>
    Center
    '
    ''' <summary>
    ''' Für For-Next Schleifen
    ''' </summary>
    LBnd = EckeLO
    '
    ''' <summary>
    ''' Für For-Next Schleifen
    ''' </summary>
    UBnd = Center

End Enum

Public Enum Animation
    None
    Erscheinen
    Verblassen
    Wachsen
    ErscheinenOhneUGrd
    VerschwindenUniUGrdfarbe
    Schrumpfen
    TaumelSchrumpfen
    WachsenPauseVerblassen
    DehnSchrumpfen
    DehnRotierSchrumpfen
    DehnTaumelSchrumpfen
    'ZweiBilderSchrumpfen
    'ZweiBilderDehnSchrumpfen
    'ZweiBilderDehnRotierSchrumpfen
    'ZweiBilderDehnTaumelSchrumpfen
    'ZweiBilderDehnZufallsanimation
    'ZweiBilderErscheinenOhneUGrd
    'ZweiBilderVerschwindenUniUGrdfarbe
    Test
End Enum

Public Enum Verdeckt
    Keine = 0  '0000
    LinksOben = 1  '0001
    RechtsOben = 2  '0010
    LinksUnten = 4  '0100
    RechtsUnten = 8  '1000
    Alle = 15 '1111
    OhneLinksOben = 14 '1110
    OhneRechtsOben = 13 '1101
    OhneLinksUnten = 11 '1011 
    OhneRechtsUnten = 7  '0111
    NurLinks = 5  '0101
    NurRechts = 10 '1010
    NurOben = 3  '0011
    NurUnten = 12 '1100
    LinksObenRechtsUnten = 6 '0110
    LinksUntenRechtsOben = 9 '1001
End Enum

'Die Umkehrung ist diese Enumeration

Public Enum Sichtbar
    Keine = 15 '1111
    LinksOben = 14 '1110
    RechtsOben = 13 '1101
    LinksUnten = 11 '1011
    RechtsUnten = 7  '0111
    Alle = 0  '0000
    OhneLinksOben = 1  '0001
    OhneRechtsOben = 2  '0010
    OhneLinksUnten = 4  '0100 
    OhneRechtsUnten = 8  '1000
    NurLinks = 10 '1010
    NurRechts = 5  '0101
    NurOben = 12 '1100
    NurUnten = 3  '0011
    LinksObenRechtsUnten = 9 '1001
    LinksUntenRechtsOben = 6 '0110
End Enum

Public Enum Quadrant
    LO = 1
    RO = 2
    LU = 4
    RU = 8
End Enum

Public Enum UpdateSrc
    Initialisierung
    PaintSpielfeld_UpdteSpielfeld_IsSet
    PaintEvent
End Enum

''' <summary>
''' Ergebnis der Suche nach einem freiem Platz für einen Stein.
''' (gebraucht in TriplR) Default ist Yes. (nur so ist es möglich
''' mit ungleich Yes auf "Nicht Valide" zu schließen, ohne genauer
''' hinzugucken, aus welchem Grund.
''' </summary>
Public Enum ValidePlaceEnum
    NotSet
    Yes
    NoFundamentFound 'in der Schicht darunter gibt es keinen
    '                 Stein, auf den gebaut werden könnte.
    Occupied         'Der Platz ist belegt
    OutsideBorder    'Spielfeldrand erreicht.
End Enum

Public Enum RenderingEnum
    None
    Spielfeld
    Editor
    Werkbank
End Enum

#Region "Enums und Konstanten"
' Datei-Menü getrennt für Editor/Werkbank (unterschiedlich erweiterbar)
Public Enum EditorFileCmd
    LadenInterne
    LadenEigene
    Speichern
    SpeichernUnter
End Enum

Public Enum WerkbkFileCmd
    LadenInterne
    LadenEigene
    Speichern
    Speichern_unter
End Enum

' Werkbank: Basisformen
Public Enum BasisformEnum
    Linie
    Winkel
    UForm
    Rechteck
    Kreis
    Pyramide
    Kegel
    Zufall
End Enum

' Platzhalter
Public Enum PlatzhalterEditor
    Item1
    Item2
End Enum

Public Enum PlatzhalterWerkbank
    Item1
    Item2
End Enum


#End Region

Public Enum KompassEnum
    None
    N
    NO
    O
    SO
    S
    SW
    W
    NW
End Enum