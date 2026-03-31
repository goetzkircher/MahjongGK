'
'##############################################################################
'Gespeichert als ...\SFKlassen\SFEnums
'##############################################################################

''' <summary>
''' Pfad: MahjongGK/Spielfeld/Basis
''' </summary>
Public Module SFEnums

    ''' <summary>
    ''' Hinweis: Bei Änderungen RenderMode, SpielMode und CvtRenderToSpielMode
    ''' aufeinander abstimmen. Die sind voneinander abhängig.
    ''' </summary>
    Public Enum RenderMode
        '
        ''' <summary>
        ''' Entweder tatsächlch keine Daten geladen oder
        ''' den Rendermode auf diesen Mode gesetzt.
        ''' Unterscheidbar durch: Keine_Daten_geladen = _sfd.SFInfo.xMax = 0  
        ''' </summary>
        NoRendering = 0
        '
        ''' <summary>
        ''' Das ist eine bewußte Pause, nicht die,die durch Untätigkeit 
        ''' geschaltet wird und sich automatisch ausschaltet. 
        ''' </summary>
        Paused = 1
        '
        ''' <summary>
        ''' Das Spielfeld wird gerendert. Das ist das Startsignal, nachdem
        ''' ein Spiel geladen oder neu angelegt wurde. Es kann also angelegt,
        ''' dann per Programmcode erzeugt werden und dann durch ändern des
        ''' RenderMode gestartet werden.
        ''' </summary>
        Spiel = 2
        '
        ''' <summary>
        ''' Wie Spielfeld, nur auf den EditorMode bezogen.
        ''' </summary>
        Edit = 4

    End Enum
    '
    ''' <summary>
    ''' Der Rendermode mit eingeschränktem Wertebereich:
    ''' Paused und NoDataLoad sind zusammengefasst zu None.
    ''' Spiel und Edit bleiben.
    ''' </summary>
    Public Enum AktRenderMode
        None
        Spiel = 2
        Edit = 4
    End Enum

    ''' <summary>
    ''' Wandelt RenderMode per Bitmaske in SpielMode um.
    ''' Paused und NoDataLoaded werden zu None.
    ''' </summary>
    Public Function CvtToAktRenderMode(ByVal rm As RenderMode) As AktRenderMode

        Return CType(CInt(rm) And 6, AktRenderMode)

    End Function

    'Zentrale Deklaration programmweit geltender Enumerationen

#Region "Stein-Enums"

    ''' <summary>
    ''' Die Unterverzeichnisse In "C:\Users\goetz\Documents\Visual Studio\MahjongGK\Eigene Ressourcen Quelle\Mahjongsteine"
    ''' enthalten jeweils einen kompletten Satz Steine und sie heißen nach dieser Enum.
    ''' </summary>
    Public Enum SteinSatz
        None
        Satz1
        Satz2
    End Enum

    ''' <summary>
    ''' Im Verzeichnis des aktiven Steinsatzes gibt es für jeden Stein
    ''' einen Unterordner, der den Status des Steins beschreibt.
    ''' Diese Enum enthält die möglichen Stati.
    ''' </summary>
    Public Enum SteinStatus
        ''' <summary>
        ''' Wenn das Programm innerhalb der IDE läuft, kann das Programmverhalten
        ''' über den Schalter "unsichtbare Steine sichtbar machen" geändert werden.
        ''' Es werden dann halbtransparente graue Steine mit Rotem Kreis und Indexnummer
        ''' angezeigt.
        ''' </summary>
        I00Unsichtbar          ' nicht sichtbar (Geistergrafik möglich)
        I01Normal
        I02Selected           'hier ohne Bedeutung. Der ersten Stein eines Paares, den der Spieler angeklickt hat
        I03Selectable        ' einzeln klickbar, aber ohne Paarstein
        I04Removable        ' klickbar und Teil eines gültigen Paars (oder noch weiterer gültiger Steine)
        I05Locked
        I06NotUnsed            ' nur für Schwierigkeitslevel-Auswahl
        I07MissingSecond       ' im Editor, wenn Partnerstein fehlt
        I08WerkstückEinfügeFehler
        I09WerkstückZufallsgrafik
        I10Reserve1            ' Geistergrafik
        I11Reserve2            ' Geistergrafik
    End Enum
    '
    ''' <summary>
    ''' Der Index auf die in Mahjong verwendeten Steine.
    ''' In jedem Verzeichnis des Steinstatus gibt es für jeden Stein
    ''' eine PNG-Datei, die die Bizmap des Steines enthält.
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

#End Region
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

    Public Enum GeneratorModus
        None = -1
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
        none
        '
        ''' <summary>
        ''' Auch _RectQuadrant(0)
        ''' </summary>
        LO = 1
        '
        ''' <summary>
        ''' Auch _RectQuadrant(1)
        ''' </summary>
        RO = 2
        '
        ''' <summary>
        ''' Auch _RectQuadrant(2)
        ''' </summary>
        LU = 4
        '
        ''' <summary>
        ''' Auch _RectQuadrant(3)
        ''' </summary>
        RU = 8
    End Enum

    Public Enum SichtbarResult
        None
        Full
        Only3DArea
    End Enum

    Public Enum UpdateSrc
        Initialisierung
        PaintSpielfeld_UpdteSpielfeld_IsSet
        PaintEvent
    End Enum

    'ZLV
    Public Enum RenderingEnum
        None
        Spielfeld
        Editor
    End Enum

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

    Public Enum Layout
        None
        SplfldWithHeaderAndHistNone
        SplfldWithHeaderAndSmallHist
        SplfldWithHeaderAndHistLeft
        SplfldWithHeaderAndHistRight
        EditorWithHeader
    End Enum

    Public Enum FreeSide
        None        '0000
        Left = 1    '0001
        Right = 2   '0010
        Both = 3    '0011
    End Enum

End Module