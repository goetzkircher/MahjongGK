Public Module GlobalEnum
    ''' <summary>
    ''' Die Enumerationen hier sind verbindlich.
    ''' Bei eventuellen Schreibfehlern gelten die Schreibweisen aus diesem Dokument.
    ''' </summary>

    Public Enum AktRenderMode
        None
        Spiel = 2
        Edit = 4
    End Enum
    '
    ''' <summary>
    ''' Grundsätzlicher Aufbau
    ''' Ein SteinDesign besteht aus drei Steinsätzen.
    ''' Es kann beliebig viele SteinDesigns geben, wie werden im
    ''' Unterverzeichnis SteinDesigns\SteinDesign.Name\und den drei Unterverzeichnissen für den Steinsatz gespeichert
    ''' </summary>
    Public Enum SteinDesign
        [Default]
        UniDunkel
        UniGelbBraun
        Test
    End Enum

    Public Enum SteinSatz
        Light
        Medium
        Dark
    End Enum
    '
    ''' <summary>
    ''' Weitere Fonts: siehe http://quivira-font.com/ 
    ''' und https://www.gnu.org/software/freefont/ranges/mahjong.html
    ''' </summary>
    Public Enum SteinFont
        Segoe
        Noto
    End Enum

    '
    ''' <summary>
    ''' Der Steinstatus bestimmt das farbliche Aussehen der Steine
    ''' teilweise mit zusätzlicher Beschriftung oder Durchkreuzung.
    ''' </summary>
    Public Enum SteinStatus
        ''' <summary>
        ''' Gibt Nothing zurück
        ''' </summary>
        I00Unsichtbar
        '
        ''' <summary>
        ''' Nicht klickbarer (nicht wählbarer) Stein (Das sind die meisten Steine 
        ''' während des Spielens und des Editierens)
        ''' </summary>
        I01Normal
        '
        ''' <summary>
        ''' 'Der ersten Stein eines Paares, den der Spieler angeklickt hat
        ''' </summary>
        I02Selected
        '
        ''' <summary>
        ''' 'einzeln klickbar (wählbar), aber ohne Paarstein
        ''' </summary>
        I03Selectable
        '
        ''' <summary>
        ''' 'klickbar und Teil eines gültigen Paars (oder mehrerer gültiger Steine)
        ''' </summary>
        I04Removable
        '
        ''' <summary>
        ''' Wenn kein Stein mehr entnehmbar ist. (kräftiges rotes Farbschema)
        ''' </summary>
        I05Locked
        '
        ''' <summary>
        ''' Im Editor: Steine aus einem Werkstück. 
        ''' (Ein Werkstück ist eine aus mehreren Steinen zusammen-
        ''' gesetzte Figur. Dessen Steine müssen ausgetauscht werden.)
        ''' </summary>
        I06WerkstückStein
        '
        ''' <summary>
        ''' Im Editor, wenn der Partnerstein fehlt. (d.h. der ist noch im Stock)
        ''' </summary>
        I07MissingSecond
        '
        ''' <summary>
        ''' Normalgrafik durchgekreuzt.
        ''' </summary>
        I08WerkstückEinfügeFehler
        '
        ''' <summary>
        ''' Normalgrafik mit halb so großem Symbol
        ''' </summary>
        I09WerkstückZufallsgrafik
        '
        ''' <summary>
        ''' Nothing zurückgeben
        ''' </summary>
        I10Reserve1
        '
        ''' <summary>
        ''' Nothing zurückgeben
        ''' </summary>
        I11Reserve2            ' 
    End Enum
    '
    ''' <summary>
    ''' Der Index auf die in Mahjong verwendeten Steine.
    ''' (In meinem erstem Satz gibt es für jeden Steinstatus 
    ''' ein Verzeichnis und dort für jeden Stein
    ''' eine PNG-Datei, die die Bizmap des Steines enthält.)
    ''' </summary>
    Public Enum SteinTyp
        '
        ''' <summary>
        ''' In der TileFactory andere Bedeutung!
        ''' </summary>
        ErrorSy = 0
        '
        '''' <summary>
        '''' Der Stein ohne Symbol. 
        '''' Gilt für die in der Tilefactory erzeugten Steine.
        '''' Hier wird ein Stein ohne Beschriftung zurückgegeben.
        '''' </summary>
        'NoSymbo = 0
        ''
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
    ''' <summary>
    ''' Hinweis: Es gibt keinen einzigen SteinTyp in 
    ''' mehreren SteinTypVersion gleichzeitig.
    ''' </summary>
    Public Enum SteinTypVersion
        '
        ''' <summary>
        ''' Die Standardversion
        ''' </summary>
        Normal
        '
        ''' <summary>
        ''' wie Normal, aber mit Bezeichnung N, S, W, O in der linken oberen Ecke
        ''' </summary>
        Winde
        '
        ''' <summary>
        ''' Anderes Farbschema
        ''' </summary>
        Blüten
        '
        ''' <summary>
        ''' Anderes Farbschema
        ''' </summary>
        JZeiten

    End Enum

    Public Enum SteinFrameVersion
        '
        ''' <summary>
        ''' Besitzer ist das Cache
        ''' </summary>
        Standard 'kommen direkt aus dem Cache
        '
        ''' <summary>
        ''' Besitzer ist auch das Cache, das Cache fasst aber nur eine einzige Bitmap,
        ''' die durch die nächste Anforderung überschrieben wird.
        ''' </summary>
        ''' '
        MouseSelected
        ''' <summary>
        ''' Besitzer ist auch das Cache, das Cache fasst aber nur eine einzige Bitmap.
        ''' </summary>
        MouseOver
        '
        ''' <summary>
        ''' Besitzer ist auch das Cache, das Cache fasst aber nur eine einzige Bitmap.
        ''' </summary>
        MouseCanDrop
        '
    End Enum
    Public Enum CacheIndex
        SpielStein
        EditorStein
        SpielMouseOver
        EditorMouseOver
        SpielSelected
        EditorSelected
        SpielCanDrop
        EditorCanDrop
        UBound = EditorCanDrop
    End Enum

    ''' <summary>
    ''' Gibt an, was zurückgegeben wird, wenn Hue = 360.
    ''' Hue = 0, Schwarz, Grau wie der B-Wert vorgibt,
    ''' Weiß oder Graus aus dem B-Wert 100-B 
    ''' </summary>
    Public Enum Hue360Mode
        '
        ''' <summary>
        ''' Behandelt Hue >= 360 als Hue = 0.
        ''' Das ist der Default
        ''' </summary>
        Hue0
        '
        ''' <summary>
        ''' 360 ergibt Schwarz
        ''' 361 ergibt Weiß
        ''' </summary>
        Black360White361
        '
        ''' <summary>
        ''' Gibt Schwarz zurück
        ''' </summary>
        Black
        '
        ''' <summary>
        ''' Gibt einen Grauwert aus dem B-Wert in Prozent zurück
        ''' </summary>
        Grey
        '
        ''' <summary>
        ''' Gibt Weiß zurück
        ''' </summary>
        White
        '
        ''' <summary>
        ''' Gibt einen Grauwert aus (100 - B) in Prozent zurück
        ''' </summary>
        GreyReverse
    End Enum

    Public Enum LightMap
        Diagonal
        Zentral
        RahmenXS
        RahmenS
        RahmenM
        RahmenL
        RahmenXL
    End Enum

    ''' <summary>
    ''' Alle echten Daten-Properties von TileColors.
    ''' AppRoot und die ...Xml-Hilfsproperties sind bewusst nicht enthalten.
    ''' </summary>
    Public Enum SteinValue
        SteinSatz
        HueBasisNormal
        HueBasisWinde
        HueBasisBlüten
        HueBasisJahrezeiten
        HueFaceFrame
        SatFaceFrame
        BrgFaceFrame
        HueSymbolOutline
        HueSymbolGradientTo
        HueSymbolGradientFrom
        SymbolGradientMode
        SymbolOutlineWidth
        SatI01Normal
        BrgI01Normal
        SatI02Selected
        BrgI02Selected
        SatI03Selectable
        BrgI03Selectable
        SatI04Removable
        BrgI04Removable
        SatI05Locked
        BrgI05Locked
        SatI06WerkstückStein
        BrgI06WerkstückStein
        SatI07MissingSecond
        BrgI07MissingSecond
        SatI08WerkstückEinfügeFehler
        BrgI08WerkstückEinfügeFehler
        SatI09WerkstückZufallsgrafik
        BrgI09WerkstückZufallsgrafik
        DeltaSatBasisToLayerUp
        DeltaBrgBasisToLayerUp
        DeltaSatBasisToLayerDn
        DeltaBrgBasisToLayerDn
        DeltaSatBasisKorrektur
        DeltaBrgBasisKorrektur
        FaceLichtkarte
        ShiftLayer
        ShiftFace
        Kommentar
        FaceBorderColor
    End Enum

End Module

