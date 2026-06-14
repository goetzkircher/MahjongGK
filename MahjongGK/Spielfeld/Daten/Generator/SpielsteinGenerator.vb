
Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

#Disable Warning IDE0079
#Disable Warning IDE1006

Imports System.Xml.Serialization
Imports MahjongGK.Contracts

Namespace Spielfeld

    ' --- Öffentliche Enumerationen ---
    ' Die Enumerationen sind in einer zentralen Deklarationsdatei gespeichert,
    ' daher hier auskommentiert.

    'Public Enum GeneratorModi
    '    StoneStream 'für endlosen Steinstrom
    '    StoneSet 'für definierte Steinmengen
    'End Enum

    'Public Enum SteinRndGruppe
    '    Normal
    '    Flower
    '    Season
    'End Enum

    'Public Enum SteinSymbol
    '    Dummy
    '    Punkt1
    '    Punkt2
    '    Punkt3
    '    Punkt4
    '    Punkt5
    '    Punkt6
    '    Punkt7
    '    Punkt8
    '    Punkt9
    '    Bambus1
    '    Bambus2
    '    Bambus3
    '    Bambus4
    '    Bambus5
    '    Bambus6
    '    Bambus7
    '    Bambus8
    '    Bambus9
    '    Symbol1
    '    Symbol2
    '    Symbol3
    '    Symbol4
    '    Symbol5
    '    Symbol6
    '    Symbol7
    '    Symbol8
    '    Symbol9
    '    DrachenRot
    '    DrachenGrün
    '    DrachenWeiß
    '    WindOst
    '    WindSüd
    '    WindWest
    '    WindNord
    '    BlütePflaume
    '    BlüteOrchidee
    '    BlüteChrisantheme
    '    BlüteBambus
    '    JahrFrühling
    '    JahrSommer
    '    JahrHerbst
    '    JahrWinter
    'End Enum

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

    'Public Enum GeneratorModus
    '    None = -1
    '    StoneStream_Base144_Continuous = 0
    '    StoneStream_Base152_Continuous = 4
    '    StoneStream_Base144_Low = 1
    '    StoneStream_Base152_Low = 5
    '    StoneStream_Base144_Medium = 2
    '    StoneStream_Base152_Medium = 6
    '    StoneStream_Base144_Height = 3
    '    StoneStream_Base152_Height = 7
    '    StoneSet_072 = 8
    '    StoneSet_076 = 12
    '    StoneSet_144 = 9
    '    StoneSet_152 = 13
    '    StoneSet_216 = 10
    '    StoneSet_228 = 14
    '    StoneSet_288 = 11
    '    StoneSet_304 = 15
    'End Enum

    Public Enum SteinRndGruppe
        Normal
        Flower
        Season
    End Enum

    ''' <summary>
    ''' Pfad: MahjongGK/Spielfeld/Daten
    ''' 
    ''' Diese Klasse kapselt alles zur Erzeugung von Spielsteinen in zufälligen Zusammenstellungen
    ''' und der Verwaltung des Steinvorrates für den Editor.
    ''' Im Editor ist das Spielfeld zu sehen, ein freischwebendes Werkzeugfenster und oben eine 
    ''' Zeile mit Mahjongsteinen, die den sichtbaren Steinvorrat enthält.
    ''' Aus dieser Zeile kann der Spieler Steine sowohl entnehmen, als auch zurücklegen.
    ''' 
    ''' </summary>
    <Serializable> 'Die Klasse wird gespeichert mit der Xml.Serialization
    Public Class SpielsteinGenerator

#Region "Instanzierung und InitDragDropBitmaps"

        'Hinweis:
        'Im Spiel können Steine nur paarweise entnommen werden.
        'Im Editor können die Steine einzeln plaziert werden.
        'Das Spiel kann erst dann gespielt werden (gilt auch für testweises Spielen),
        'wenn sich auf dem Spielfeld nur Paare befinden. Das Editieren kann aber jederzeit
        'unterbrochen werden und der Editierungsstand gespeichert und wiederhergestellt werden.
        '
        'Ein Stein, der sich alleine auf dem Spielfeld befindet, nenne ich Strohwitwer,
        'der Paarstein, der noch im Steinvorrat ist, ist die Strohwitwe.

        Sub New()

        End Sub
        Sub New(generatorMode As GeneratorModus)

            Dim genmod As (isStoneSet As Boolean, isBase152 As Boolean, count As Integer) = GetValueFromGeneratorModi(generatorMode)

            DoSubNew(generatorMode, genmod.count + 1, genmod.isBase152)

        End Sub

        ''Sub New(visibleAreaMaxLength As Integer, generatorModus As GeneratorModi,
        ''        Optional halfSteinsetsCount As Integer = 2,
        ''        Optional imStoneSet152SteineErzeugen As Boolean = False)

        ''    DoSubNew(visibleAreaMaxLength, generatorModus, halfSteinsetsCount, imStoneSet152SteineErzeugen)

        ''End Sub

        Private Sub DoSubNew(generatorModus As GeneratorModus,
                             halfSteinsetsCount As Integer,
                             stoneSet152SteineErzeugen As Boolean)

            _GeneratorModus = generatorModus
            Me.HalfSteinsetsCount = halfSteinsetsCount
            Me.StoneSet152SteineErzeugen = stoneSet152SteineErzeugen
            StockNoSortAreaEndIndex = INI.SpielsteinGenerator_VorratNoSortAreaEndIndexDefault 'Default = 9

            'Die Werte werden nur beim SteinStrom benötigt.
            Dim min As Integer = If(stoneSet152SteineErzeugen, 152 \ 2 - 1, 144 \ 2 - 1)
            Dim delta As Integer = If(stoneSet152SteineErzeugen, 152, 144)
            StockMaxUBound = min + delta
            StockNachschubschwelle = min
            CheckAndRefillVorrat()
        End Sub
        '
#End Region

#Region "Deklarationen"

#End Region

#Region "Eigenschaften nicht persistent"
        '
        ''' <summary>
        ''' Verkürzt gewaltsam den Stock.
        ''' Funktioniert nur, wenn Debugger.IsAttached. und nur zum Debuggen gedacht.
        ''' Andernfalls ist der Aufruf unschädlich, aber wirkungslos.
        ''' </summary>
        Public WriteOnly Property DebugStoneCountLimit As Integer
            Set(value As Integer)
                If Debugger.IsAttached Then
                    If value > 0 AndAlso value < Stock.Count AndAlso Stock.Count > 0 Then
                        StockStopNachschub = True
                        Dim arrStock() As SteinSymbol = Stock.ToArray
                        ReDim Preserve arrStock(value - 1)
                        Stock.Clear()
                        Stock.AddRange(arrStock)
                        _hasUndoStockSnapshot = True
                    End If
                End If
            End Set
        End Property

        <XmlIgnore>
        Private _rndGenerator As Random

        Private Sub InitGeneratorRandomIfNeeded()

            If _rndGenerator Is Nothing Then
                _rndGenerator = CreateGeneratorRandom()
            End If

        End Sub
#End Region

#Region "Eigenschaften persistent"

        '
        ''' <summary>
        ''' Zur zukünftigen Nutzung
        ''' </summary>
        ''' <returns></returns>
        Public Property SchemaVersion As Integer = 1
        '
        ''' <summary>
        ''' Zur zukünftigen Nutzung
        ''' </summary>
        ''' <returns></returns>
        Public Property GeneratorVersion As Integer = 1
        '
        Private _GeneratorModus As GeneratorModus
        ''' <summary>
        ''' Im GeneratorModi.StoneStream gibt es einen endlosen Strom an Steinen, die aber in Portionen erzeugt
        ''' werden. Die Portionsgröße ergibt sich aus der aktuellen Anzahl von Steinen in der Vorrat und
        ''' der VorratskisteLength. Überprüft wird es bei der InitDragDropBitmaps und bei jeder Steinentnahme.
        ''' (Hier kommt noch die VorratskisteNachschubSchwelle ins Spiel).
        ''' Der GeneratorModus eines Spiellayoutes kann nicht mehr geändert werden.
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property GeneratorModus As GeneratorModus
            Get
                Return _GeneratorModus
            End Get
        End Property

        Public Property GeneratorModusForXml As GeneratorModus
            Get
                Return _GeneratorModus
            End Get
            Set(value As GeneratorModus)
                _GeneratorModus = value
            End Set
        End Property
        '
        ''' <summary>
        ''' Im GeneratorModi.StoneSet liefert das Programm immer ein vielfaches von halben"
        ''' Steinsätzen. Ein vollstädiger Steinsatz hat 144 oder 152 Steine. Default ist 2
        ''' </summary>
        ''' <returns></returns>
        Public Property HalfSteinsetsCount As Integer
        '
        ''' <summary>
        ''' In einem vollständigem Steinsatz kommen 144 Steine vor. Das Programm liefert die
        ''' Sätze in vielfachen von halben Steinsätzen, das ergibt aber im Satz 8 Steine zuviel.
        ''' Ist der Schalter ein, stimmt die Anzahl, aber bestimmte Steine kommen nur alternierend
        ''' in jedem zweitem Satz vor. (entweder Blumen oder Jahreszeiten). Default: True
        ''' </summary>
        ''' <returns></returns>
        Public Property StoneSet152SteineErzeugen As Boolean
        '
        ''' <summary>
        ''' Die Vorratskiste, in der sich die Steine befinden. 
        ''' </summary>
        ''' <returns></returns>
        Public Property Stock As List(Of SteinSymbol)
        Public ReadOnly Property StockAktCount As Integer
            Get
                If IsNothing(Stock) Then
                    Return -1
                Else
                    Return Stock.Count
                End If
            End Get
        End Property
        Public ReadOnly Property StockAktUBnd As Integer
            Get
                If IsNothing(Stock) Then
                    Return -1
                Else
                    Return Stock.Count - 1
                End If
            End Get
        End Property
        '
        ''' <summary>
        ''' Die Länge der Vorratskiste, d.h. die Anzahl maximal enthaltener Steine -1
        ''' </summary>
        ''' <returns></returns>
        Public Property StockMaxUBound As Integer
        '
        ''' <summary>
        ''' Unterschreitet die Anzahl der Steine in der Vorratskiste diesen Wert,
        ''' wird Nachschub erzeugt und an die Kiste angehängt.
        ''' </summary>
        ''' <returns></returns>
        Public Property StockNachschubschwelle As Integer
        '
        Private _VorratStopNachschub As Boolean
        ''' <summary>
        ''' Ist dieses Flag True, wird die Vorratskiste nicht nachgefüllt.
        ''' Wird es auf False zurückgestellt, wird sofort nachgefüllt.
        ''' </summary>
        ''' <returns></returns>
        <XmlIgnore>
        Public Property StockStopNachschub As Boolean
            Get
                Return _VorratStopNachschub
            End Get
            Set(value As Boolean)
                _VorratStopNachschub = value
                If Not value Then
                    CheckAndRefillVorrat()
                End If
            End Set
        End Property
        '
        Public Property StockStopNachschubXmlOnly As Boolean
            Get
                Return _VorratStopNachschub
            End Get
            Set(value As Boolean)
                _VorratStopNachschub = value
            End Set
        End Property
        '
        ''' <summary>
        ''' Die Vorratskiste kann jederzeit neu gemischt werden.
        ''' Davon ausgenommen ist der Bereich in der Vorratskiste
        ''' von Index = 0 bis NoSortAreaEndIndex.
        ''' Abschalten mit NoSortAreaEndIndex = -1 
        ''' </summary>
        ''' <returns></returns>
        Public Property StockNoSortAreaEndIndex As Integer
        ''
        '''' <summary>
        '''' Erstellt eine echte tiefe Kopie von SteinInfo.
        '''' Alle Wertetypen/Enums sind ohnehin kopiert; Referenzen werden manuell geklont.
        '''' </summary>
        'Public Function DeepCopy() As SFInfo

        '    ' Flache Kopie aller Felder (inkl. privater Backing Fields wie _AnimMaxStep, _AnimLoops)
        '    Dim c As SFInfo = DirectCast(Me.MemberwiseClone(), SFInfo)

        '    ' Tiefer Kopieren aller Referenztypen/Felder:
        '    ' Triple ist eine Klasse → explizit klonen

        '    ' Falls später weitere Referenztypen hinzukommen, hier analog tief kopieren.

        '    Return c

        '    'Antwort von ChatGPT, ob man das über Serialisierung/Deserialisierung
        '    'lösen könne:

        '    'Warum das besser ist als (De-)Serialisierung:
        '    '<XmlIgnore>-Felder würden bei XML-Serialisierung fehlen → unvollständige Kopie.
        '    'Keine Attribute / Type - Resolver nötig, kein Overhead.
        '    'Setter-Logiken(wie AnimMaxStep mit ×100) werden nicht versehentlich erneut ausgeführt
        '    '— die privaten Backing-Felder werden 1:1 kopiert, genau wie gewünscht.
        '    'Da Triple bereits eine saubere DeepCopy liefert, reicht das oben völlig aus.
        '    '
        '    'Die frühere Methode über den BinaryFormatter ist von Microsoft als depreceted eingestuft.
        '    '
        '    'Es ginge noch über den DataContractSerializer, das ist aber mit Vergabe vieler Attribute
        '    'versehen.
        'End Function

#End Region

#Region "statisch"
        '
        Public Shared ReadOnly NormalSteinsymbole As SteinSymbol() = {
            SteinSymbol.Punkt01, SteinSymbol.Punkt02, SteinSymbol.Punkt03, SteinSymbol.Punkt04, SteinSymbol.Punkt05,
            SteinSymbol.Punkt06, SteinSymbol.Punkt07, SteinSymbol.Punkt08, SteinSymbol.Punkt09,
            SteinSymbol.Bambus1, SteinSymbol.Bambus2, SteinSymbol.Bambus3, SteinSymbol.Bambus4, SteinSymbol.Bambus5,
            SteinSymbol.Bambus6, SteinSymbol.Bambus7, SteinSymbol.Bambus8, SteinSymbol.Bambus9,
            SteinSymbol.Symbol1, SteinSymbol.Symbol2, SteinSymbol.Symbol3, SteinSymbol.Symbol4, SteinSymbol.Symbol5,
            SteinSymbol.Symbol6, SteinSymbol.Symbol7, SteinSymbol.Symbol8, SteinSymbol.Symbol9,
            SteinSymbol.DracheR, SteinSymbol.DracheG, SteinSymbol.DracheW,
            SteinSymbol.WindOst, SteinSymbol.WindSüd, SteinSymbol.WindWst, SteinSymbol.WindNrd
        } ' = 34

        Public Shared ReadOnly FlowerSteinsymbole As SteinSymbol() = {
        SteinSymbol.BlütePf, SteinSymbol.BlüteOr, SteinSymbol.BlüteCt, SteinSymbol.BlüteBa
        } ' = 4

        Public Shared ReadOnly SeasonSteinsymbole As SteinSymbol() = {
        SteinSymbol.JahrFrl, SteinSymbol.JahrSom, SteinSymbol.JahrHer, SteinSymbol.JahrWin
        } ' = 4

#End Region

#Region "Öffentliches"

        Private _hasUndoStockSnapshot As Boolean
        Private _undoStockSnapShot() As SteinSymbol
        Public Function ConsumeHasUndoStockSnapshot() As Boolean

            If _hasUndoStockSnapshot Then
                _hasUndoStockSnapshot = False
                Return True
            Else
                Return False
            End If
        End Function
        '
        ''' <summary>
        ''' Die Funktion ist zeitkritisch, deshalb wird das Original geliefert und keine Kopie.
        ''' Das Original darf nur aufbewahr werden und keinesfalls geändert werden.
        ''' </summary>
        ''' <returns></returns>
        Public Function UndoStocksnapShot() As SteinSymbol()
            Return _undoStockSnapShot
        End Function

        Public Function GetGroup(ByVal idx As SteinSymbol) As SteinRndGruppe
            Dim i As Integer
            For i = 0 To NormalSteinsymbole.Length - 1
                If NormalSteinsymbole(i) = idx Then Return SteinRndGruppe.Normal
            Next
            For i = 0 To FlowerSteinsymbole.Length - 1
                If FlowerSteinsymbole(i) = idx Then Return SteinRndGruppe.Flower
            Next
            For i = 0 To SeasonSteinsymbole.Length - 1
                If SeasonSteinsymbole(i) = idx Then Return SteinRndGruppe.Season
            Next
            ' Fallback: behandle Unbekanntes als Normal
            Return SteinRndGruppe.Normal

        End Function
        '
        ''' <summary>
        ''' Nach manuellem clearen des Vorrat kann er hiermit auch manuell
        ''' wieder gefüllt werden.
        ''' Im GeneratorModi.StoneSet wird der Vorrat nur einmalig aufgefüllt. 
        ''' </summary>
        Public Sub CheckAndRefillVorrat()

            Dim genmod As (isStoneSet As Boolean, isBase152 As Boolean, count As Integer) = GetValueFromGeneratorModi(_GeneratorModus)

            If genmod.isStoneSet Then
                If Stock Is Nothing Then
                    'Nur beim ersten Aufruf füllen
                    Stock = New List(Of SteinSymbol)()
                    Dim stones1 As List(Of SteinSymbol)
                    InitGeneratorRandomIfNeeded()

                    stones1 = BuildStoneSetStones(halfSetCount:=genmod.count + 1,
                                 isBase152:=genmod.isBase152,
                                 rnd:=_rndGenerator)

                    Stock.AddRange(stones1)
                    _hasUndoStockSnapshot = True
                    _undoStockSnapShot = Stock.ToArray()
                End If

                Exit Sub
            End If

            'Ab hier: GeneratorModus = GeneratorModi.StoneStream 

            If Stock Is Nothing Then
                Stock = New List(Of SteinSymbol)()
            End If

            If _VorratStopNachschub Then
                Exit Sub
            End If

            If Stock.Count > StockNachschubschwelle Then
                Exit Sub
            End If

            InitGeneratorRandomIfNeeded()

            Dim pairCount As Integer = (StockMaxUBound - Stock.Count + 1) \ 2

            Dim stones2 As List(Of SteinSymbol)

            stones2 = BuildStoneStreamPortion(pairCount:=pairCount,
                                         isBase152:=genmod.isBase152,
                                         rnd:=_rndGenerator)

            _hasUndoStockSnapshot = True
            _undoStockSnapShot = Stock.ToArray()
            Stock.AddRange(stones2)

        End Sub
        '
        ''' <summary>
        ''' Gibt den selektierten Stein zurück und löscht ihn im Vorrat.
        ''' Wenn index kleiner 0 OrElse Vorrat.Count = 0 OrElse index > Vorrat.Count - 1
        ''' dann wird die Fehlergrafik zurückgegeben.
        ''' </summary>
        ''' <param name="index"></param>
        ''' <returns></returns>
        Public Function GetSteinSymbolIndexAndRemove(index As Integer) As SteinSymbol
            If index < 0 OrElse Stock Is Nothing OrElse Stock.Count = 0 OrElse index > Stock.Count - 1 Then
                Return SteinSymbol.ErrorSy 'Die Fehlergrafik
            Else
                Dim retval As SteinSymbol = Stock(index)
                Stock.RemoveAt(index)  ' <- exakt diese Position löschen
                CheckAndRefillVorrat()
                Return retval
            End If
        End Function
        ''' <summary>
        ''' Gibt den selektierten Stein zurück und löscht ihn NICHT im Vorrat.
        ''' Wenn index kleiner 0 OrElse Vorrat.Count = 0 OrElse index > Vorrat.Count - 1
        ''' dann wird die Fehlergrafik zurückgegeben.
        ''' </summary>
        ''' <param name="index"></param>
        ''' <returns></returns>
        Public Function GetSteinSymbolIndexDontRemove(index As Integer) As SteinSymbol
            If index < 0 OrElse Stock Is Nothing OrElse Stock.Count = 0 OrElse index > Stock.Count - 1 Then
                Return SteinSymbol.ErrorSy 'Die Fehlergrafik
            Else
                Dim retval As SteinSymbol = Stock(index)
                Return retval
            End If
        End Function
        '
        ''' <summary>
        ''' Fügt den übergebenen Stein links vom index ein.
        ''' (Zurücklegen eines Steines vom Feld in den Vorrat.)
        ''' Ist index zu klein, wird ganz links eingefügt,
        ''' ist er zu groß ganz rechts.
        ''' Vorsicht: nur verwenden um entnommene Steine wieder hinzuzufügen,
        ''' sonst stimmt die Zusammensetzung der Steine nicht mehr.
        ''' </summary>
        ''' <param name="index"></param>
        ''' <param name="sie"></param>
        Public Sub InsertLeftFromSteinIdx(index As Integer, sie As SteinSymbol)

            If Stock.Count = 0 Then
                Stock.Add(sie)
            ElseIf index < 0 Then
                Stock.Insert(0, sie)
            ElseIf index >= Stock.Count Then
                Stock.Add(sie)
            Else
                Stock.Insert(index, sie)
            End If
        End Sub
        '
        ''' <summary>
        ''' Fügt ein Stein am Ende hinzu.
        ''' Vorsicht: nur verwenden um entnommene Steine wieder hinzuzufügen.
        ''' </summary>
        ''' <param name="sie"></param>
        Public Sub AddAtStockEnd(sie As SteinSymbol)
            Stock.Add(sie)
        End Sub

        Public Sub ShuffleStock(includeNoSortArea As Boolean)
            '
            If Stock.Count <= 1 Then
                'hier gibt es nichts zu mischen
                Exit Sub
            End If

            Dim idxTo As Integer = 0

            If Not includeNoSortArea Then

                If Stock.Count - 1 <= StockNoSortAreaEndIndex Then
                    'hier auch nicht
                    Exit Sub
                End If

                If Stock.Count - 1 <= StockNoSortAreaEndIndex + 2 Then
                    'weniger als 2 Steine hinter dem Index vorhanden
                    'hier gibt es nichts zu mischen
                    Exit Sub
                End If
                idxTo = StockNoSortAreaEndIndex + 1
            End If

            _hasUndoStockSnapshot = True
            _undoStockSnapShot = Stock.ToArray

            For idx1 As Integer = Stock.Count - 1 To idxTo Step -1
                Dim idx2 As Integer = GetZufallszahl(idxTo, idx1 + 1)
                Dim tmp As SteinSymbol = Stock(idx1)
                Stock(idx1) = Stock(idx2)
                Stock(idx2) = tmp
            Next

        End Sub
        Public Sub SortStockPur()
            _hasUndoStockSnapshot = True
            _undoStockSnapShot = Stock.ToArray
            Stock.Sort()
        End Sub
        '
        ''' <summary>
        ''' Gibt einen Array() As Boolean zurück, der an den Indexpositionen True enthält,
        ''' an denen im Stock ein Witwenstein steht.
        ''' </summary>
        ''' <returns></returns>
        Public Function GetArrayStockWidows() As Boolean()

            'Die Methode der Feststellung weicht von SortStockByPairs ab.
            '
            '
            If Stock.Count <= 0 Then
                'hier gibt es nichts zu tun
                Return Array.Empty(Of Boolean)
            End If

            ''Hier nicht nötig, sogar falsch, da der Stock nicht verändert wird.
            ''_hasUndoStockSnapshot = True
            ''_undoStockSnapShot = Stock.ToArray

            Dim arrSteine() As SteinSymbol = Stock.ToArray
            '
            'hier beidesmal wird rückwärts gesucht
            '(Der Grund warum SortStockByPairs anders gesucht wird, liegt in der Umsortierung.)
            For idx1 As Integer = arrSteine.GetUpperBound(0) To 0 Step -1
                If arrSteine(idx1) >= 0 Then
                    'Im SpielsteinGenerator wird generell windsInOneClickGroup:=False verwendet, damit das Spiel auch
                    'mit windsInOneClickGroup:=False gespielt werden kann. windsInOneClickGroup:=True soll eine
                    'Spielvereinfachung bedeuten und keine Editorvereinfachung, die zur Folge hätte, das diese
                    'so entwickelten Spiele nur mit windsInOneClickGroup:=True gespielt werden können.
                    '(Alleine die Prüfung, ob ein Spiel spielfähig ist, müssse an weitere Randbedingungen geknüpft werden.)
                    Dim klickgruppe As Integer = SFInfo.GetSteinClickGruppe(arrSteine(idx1), windsInOneClickGroup:=False)
                    For idx2 As Integer = arrSteine.GetUpperBound(0) To idx1 + 1 Step -1
                        If arrSteine(idx2) >= 0 Then
                            If klickgruppe = SFInfo.GetSteinClickGruppe(arrSteine(idx2), windsInOneClickGroup:=False) Then
                                arrSteine(idx1) = CType(-1, SteinSymbol) 'als entnommen Kennzeichnen
                                arrSteine(idx2) = CType(-1, SteinSymbol) 'als entnommen Kennzeichnen
                                Exit For
                            End If
                        End If
                    Next
                End If
            Next
            '
            'Übrig bleiben die Witwen
            Dim arrResult(Stock.Count - 1) As Boolean
            For idx As Integer = 0 To arrSteine.GetUpperBound(0)
                arrResult(idx) = arrSteine(idx) >= 0
            Next

            Return arrResult

        End Function
        '
        ''' <summary>
        ''' Sortiert den Vorrat so, daß alle Steinpaare zusammenstehen.
        ''' Gibt zurück, wieviele "Witwen" am Anfang stehen.
        ''' </summary>
        ''' <returns></returns>
        Public Function SortStockByPairs() As Integer

            If Stock.Count <= 1 Then
                'hier gibt es nichts zu sortieren
                Return Stock.Count
            End If
            '
            _hasUndoStockSnapshot = True
            _undoStockSnapShot = Stock.ToArray
            '
            'Die Idee ist, daß die Reihenfolge der Steine grundsätzlich erhalten bleibt.
            'd.h. aus einer Kopie der Liste der Steine wird von vorne ein Stein nach dem anderen
            'entnommen und von hinten der zugehörige Paarstein gesucht,
            '
            Dim arrSteine() As SteinSymbol = Stock.ToArray
            Stock.Clear()

            For idx1 As Integer = 0 To arrSteine.GetUpperBound(0)
                If arrSteine(idx1) >= 0 Then
                    Dim klickgruppe As Integer = SFInfo.GetSteinClickGruppe(arrSteine(idx1), windsInOneClickGroup:=False)
                    For idx2 As Integer = arrSteine.GetUpperBound(0) To idx1 + 1 Step -1
                        If arrSteine(idx2) >= 0 Then
                            If klickgruppe = SFInfo.GetSteinClickGruppe(arrSteine(idx2), windsInOneClickGroup:=False) Then
                                Stock.Add(arrSteine(idx1))
                                arrSteine(idx1) = CType(-1, SteinSymbol) 'als entnommen Kennzeichnen
                                Stock.Add(arrSteine(idx2))
                                arrSteine(idx2) = CType(-1, SteinSymbol) 'als entnommen Kennzeichnen
                                Exit For
                            End If
                        End If
                    Next
                End If
            Next

            'Die Witwen bleiben bei dieser Sortiermethode in arrSteine übrig.
            'Diese suchen
            Dim witwen As New List(Of SteinSymbol)
            For idx1 As Integer = 0 To arrSteine.GetUpperBound(0)
                If arrSteine(idx1) >= 0 Then
                    witwen.Add(arrSteine(idx1))
                End If
            Next

            If witwen.Count = 0 Then
                Return 0
            End If

            Stock.InsertRange(0, witwen)

            Return witwen.Count

        End Function

        Public Sub SeperateStockWidow()

            If Stock.Count <= 2 Then
                'hier gibt es nichts zu separieren.
                '(Wenn Stock.Count = 2, dann ist das entweder ein Steinpaar oder zwei Steinwitwen.
                'beides braucht nicht separiert werden. )
                Exit Sub
            End If

            '' wieder in SortStockByPairs erledigt
            ''_hasUndoStockSnapshot = True
            ''_undoStockSnapShot = Stock.ToArray

            Dim sicStock() As SteinSymbol = Stock.ToArray()
            'zunächste mit SortStockByPairs sortieren, um alle Witwen an den Anfang zu spülen.
            Dim witwenCount As Integer = SortStockByPairs()

            If witwenCount = 0 Then
                'Stock wiederherstellen
                Stock.Clear()
                Stock.InsertRange(0, sicStock)
                Exit Sub
            End If
            '
            'die Witwen separieren
            Dim witwen(witwenCount - 1) As SteinSymbol
            For idx As Integer = 0 To witwenCount - 1
                witwen(idx) = Stock(idx)
            Next
            '
            'Stock wiederherstellen
            Stock.Clear()
            Stock.InsertRange(0, sicStock)
            '
            'In dem wiederhergestelltem Stock sind die Witwen ja noch drin.
            'diese herauslöschen
            For idx1 As Integer = 0 To witwen.GetUpperBound(0)
                If Not RemoveFirstExactSymbol(Stock, witwen(idx1)) Then
                    Throw New Exception("unklarer Programmierfehler")
                    ''Nur Sicherheits-Fallback, eigentlich sollte das nicht nötig sein.
                    'Dim klickgruppe As Integer = SFInfo.GetSteinClickGruppe(witwen(idx1), windsInOneClickGroup:=False)

                    'For idx2 As Integer = 0 To Stock.Count - 1
                    '    If SFInfo.GetSteinClickGruppe(Stock(idx2), windsInOneClickGroup:=False) = klickgruppe Then
                    '        Stock.RemoveAt(idx2)
                    '        Exit For
                    '    End If
                    'Next

                End If
            Next
            '
            'und am Anfang wieder hinzu fügen
            Stock.InsertRange(0, witwen)

        End Sub

        Private Function RemoveFirstExactSymbol(stock As List(Of SteinSymbol),
                                               sie As SteinSymbol) As Boolean

            For idx As Integer = 0 To stock.Count - 1
                If stock(idx) = sie Then
                    stock.RemoveAt(idx)
                    Return True
                End If
            Next

            Return False

        End Function
#End Region

#Region "Statistik"

        ''' <summary>
        ''' Wertet den aktuellen Vorrat aus
        ''' Liefert je SteinSymbol einen Single:
        '''   Vorkomma  = absolute Anzahl des Steins in Vorrat,
        '''   Nachkomma = Anteil (0..0.999) auf 3 Nachkommastellen gerundet.
        ''' Sonderfall: Liegt der Anteil bei 1.000 (100 %), wird er auf 0.999 gesetzt.
        ''' Bei leerem Vorrat wird ein (MJ_STEININDEX_MAX+1)-Array aus 0.0F zurückgegeben.
        ''' </summary>
        Public Function Statistic() As Single()
            Dim maxIdx As Integer = MJ_STEININDEX_MAX
            Dim result(maxIdx) As Single

            ' Guards
            If Stock Is Nothing OrElse Stock.Count = 0 Then
                Return result
            End If

            ' Zählen
            Dim counts(maxIdx) As Integer
            Dim total As Integer = Stock.Count

            For i As Integer = 0 To total - 1
                Dim idx As Integer = CInt(Stock(i))
                If idx >= 0 AndAlso idx <= maxIdx Then
                    counts(idx) += 1
                End If
            Next

            ' In Single mit Anteil (3 Nachkommastellen) packen
            For idx As Integer = 0 To maxIdx
                Dim c As Integer = counts(idx)
                Dim frac As Single = 0.0F

                If c > 0 Then
                    Dim r As Double = CDbl(c) / CDbl(total)                 ' 0..1
                    Dim rr As Double = Math.Round(r, 3, MidpointRounding.AwayFromZero)
                    If rr >= 1.0R Then rr = 0.999R                          ' 100% → 0.999
                    frac = CSng(rr)
                End If

                result(idx) = CSng(c) + frac
            Next

            Return result
        End Function

        '
        ''' <summary>
        ''' Erzeugt eine kompakte String-Darstellung der Statistik.
        ''' Format je Eintrag: "idx [Name]: Anzahl (Anteil)"
        ''' Anteil ist der Faktor 0.000..0.999 (100% wird als 0.999 gezeigt).
        ''' </summary>
        ''' <param name="onlyNonZero">Nur Einträge mit Anzahl > 0 ausgeben.</param>
        ''' <param name="itemsPerLine">Wie viele Einträge pro Zeile.</param>
        ''' <param name="showEnumName">Enum-Namen zusätzlich anzeigen.</param>
        Public Function FormatStatisticString(Optional onlyNonZero As Boolean = True,
                                          Optional itemsPerLine As Integer = 9,
                                          Optional showEnumName As Boolean = True) As String

            Dim stats As Single() = Me.Statistic()
            Dim sb As New System.Text.StringBuilder(2048)
            Dim col As Integer = 0

            If stats Is Nothing OrElse stats.Length = 0 Then
                Return "(keine Daten)"
            End If

            For idx As SteinSymbol = 0 To CType(MJ_STEININDEX_MAX, SteinSymbol)
                Dim absCount As Integer
                Dim ratio As Single
                ParseStatisticValue(stats(idx), absCount, ratio)

                If (Not onlyNonZero) OrElse absCount > 0 Then
                    Dim label As String
                    If showEnumName Then
                        Dim name As String = CType(idx, SteinSymbol).ToString()
                        'label = String.Format("{0,2} {1,-16}", idx, name)
                        label = String.Format("{0,-8}", name)
                    Else
                        label = String.Format("{0,2}", idx)
                    End If

                    Dim piece As String = String.Format("{0}= {1,3} ({2:#0.0}%)", label, absCount, ratio * 100)

                    'Dim sollProzent As Single
                    'If StoneSet152SteineErzeugen Then
                    'Else

                    'End If

                    If col > 0 Then sb.Append("   ")
                    sb.Append(piece)
                    col += 1
                    If itemsPerLine = 9 Then
                        If idx = SteinSymbol.ErrorSy OrElse
                            idx = SteinSymbol.Punkt09 OrElse
                            idx = SteinSymbol.Bambus9 OrElse
                            idx = SteinSymbol.Symbol9 OrElse
                            idx = SteinSymbol.DracheW OrElse
                            idx = SteinSymbol.WindNrd OrElse
                            idx = SteinSymbol.BlüteBa OrElse
                            col >= itemsPerLine Then

                            sb.AppendLine()
                            col = 0
                        End If
                    Else
                        If idx = 0 OrElse col >= itemsPerLine Then
                            sb.AppendLine()
                            col = 0
                        End If
                    End If
                End If
            Next

            If col <> 0 Then sb.AppendLine()
            Return sb.ToString()
        End Function

        '
        ''' <summary>
        ''' Schreibt die Statistik kompakt ins Debug-Ausgabefenster.
        ''' </summary>
        ''' <param name="onlyNonZero">Nur Einträge mit Anzahl > 0 ausgeben.</param>
        ''' <param name="itemsPerLine">Wie viele Einträge pro Zeile.</param>
        ''' <param name="showEnumName">Enum-Namen zusätzlich anzeigen.</param>
        <Conditional("DEBUG")>
        Public Sub DebugPrintStatistic(Optional onlyNonZero As Boolean = False,
                                   Optional itemsPerLine As Integer = 9,
                                   Optional showEnumName As Boolean = True)
            Dim txt As String = FormatStatisticString(onlyNonZero, itemsPerLine, showEnumName)
            Debug.Print(txt)
        End Sub

        '
        ''' <summary>
        ''' Zerlegt einen Statistic()-Wert in absolute Anzahl und Anteil (gerundet auf 3 Nachkommastellen).
        ''' </summary>
        Public Shared Sub ParseStatisticValue(val As Single, ByRef absoluteCount As Integer, ByRef ratio As Single)
            absoluteCount = CInt(Math.Truncate(val))
            Dim frac As Double = CDbl(val) - Math.Truncate(CDbl(val))
            ratio = CSng(Math.Round(frac, 3, MidpointRounding.AwayFromZero))
            If ratio >= 1.0F Then ratio = 0.999F ' Sicherheitsnetz, sollte aus Statistic() schon so kommen
        End Sub

#End Region

        ''Wird vermutlich nicht gebrauct.
        ''#Region "Strohwitwen"

        ''        Private ReadOnly _VorratStrohWitwen As New List(Of SteinSymbol)

        ''        ''' <summary>
        ''        ''' Entfernt alls Paare aus dem Vorrat, sodaß anschließend nur noch Strohwitwen darin sind.
        ''        ''' Gibt die Anzahl der Strohwitwen zurück.
        ''        ''' ACHTUNG: Setzt VorratStopNachschub auf True. Muss manuell rückgestellt werden,
        ''        ''' dann wird der Vorrat auch sofort wieder aufgefüllt. (für "weiter Editieren")
        ''        ''' Die Strohwitwen bleiben drin(!) und stehen ganz am Anfang.
        ''        ''' </summary>
        ''        ''' <param name="windsAreInOneClickGroup"></param>
        ''        ''' <returns></returns>
        ''        Public Function RemovePaareFromVorrat(windsAreInOneClickGroup As Boolean) As Integer
        ''            CreateVorratStrohWitwen(windsAreInOneClickGroup)
        ''            Stock = _VorratStrohWitwen
        ''            StockStopNachschub = True
        ''            Return Stock.Count
        ''        End Function

        ''        Public ReadOnly Property VorratStrohWitwen(windsAreInOneClickGroup As Boolean) As List(Of SteinSymbol)
        ''            Get
        ''                CreateVorratStrohWitwen(windsAreInOneClickGroup)
        ''                Return _VorratStrohWitwen
        ''            End Get
        ''        End Property

        ''        Public ReadOnly Property VorratHasStrohWitwen(windsAreInOneClickGroup As Boolean) As Boolean
        ''            Get
        ''                CreateVorratStrohWitwen(windsAreInOneClickGroup)
        ''                Return _VorratStrohWitwen.Count > 0
        ''            End Get
        ''        End Property

        ''        Private Sub CreateVorratStrohWitwen(windsAreInOneClickGroup As Boolean)

        ''            _VorratStrohWitwen.Clear()
        ''            If Stock Is Nothing OrElse Stock.Count = 0 Then
        ''                Exit Sub
        ''            End If

        ''            ' Enum ist lückenlos 0..MJ_STEININDEX_MAX
        ''            Dim counts(MJ_STEININDEX_MAX) As Integer

        ''            For i As Integer = 0 To Stock.Count - 1
        ''                Dim idx As Integer = SFInfo.GetSteinClickGruppe(CType(Stock(i), SteinSymbol), windsAreInOneClickGroup)
        ''                If idx >= 0 AndAlso idx <= MJ_STEININDEX_MAX Then
        ''                    counts(idx) += 1
        ''                End If
        ''            Next

        ''            ' ---- Sonderfall Fehlergrafik (0): bei Vorkommen immer genau einmal aufnehmen
        ''            If counts(0) > 0 Then
        ''                _VorratStrohWitwen.Add(CType(0, SteinSymbol))
        ''            End If

        ''            ' ---- Alle anderen: nur bei ungerader Häufigkeit je einmal aufnehmen
        ''            For idx As Integer = 1 To MJ_STEININDEX_MAX
        ''                If (counts(idx) And 1) <> 0 Then
        ''                    _VorratStrohWitwen.Add(CType(idx, SteinSymbol))
        ''                End If
        ''            Next
        ''        End Sub

        ''#End Region

#Region "Hilfsfunktionen GeneratorModi"
        ''' <summary>
        ''' Wandelt die Angaben in die entsprechende Enumeration um.
        ''' </summary>
        ''' <param name="isStoneSet"></param>
        ''' <param name="isBase152"></param>
        ''' <param name="count">Darf die Werte 0, 1, 2 und 3 annehmen. (sonst Exception)</param>
        ''' <returns></returns>
        Public Shared Function GetGeneratorModus(isStoneSet As Boolean,
                                         isBase152 As Boolean,
                                         count As Integer) As GeneratorModus

            If count < 0 OrElse count > 3 Then
                Throw New ArgumentOutOfRangeException(NameOf(count), "count muss 0–3 sein.")
            End If

            Dim value As Integer = 0

            ' Bit 3: StoneSet oder Stream
            If isStoneSet Then value = value Or &H8

            ' Bit 2: Base152 oder Base144
            If isBase152 Then value = value Or &H4

            ' Bits 1..0: Count
            value = value Or (count And &H3)

            Return CType(value, GeneratorModus)

        End Function

        ''' <summary>
        ''' Die Gegenfunktion zu GetGeneratorModi
        ''' </summary>
        ''' <param name="genmod"></param>
        ''' <returns></returns>
        Public Shared Function GetValueFromGeneratorModi(genmod As GeneratorModus) As (isStoneSet As Boolean,
                                                                                isBase152 As Boolean,
                                                                                count As Integer)

            Dim value As Integer = CInt(genmod)
            If value < 0 OrElse value > 15 Then
                Throw New ArgumentOutOfRangeException(NameOf(genmod),
                                              "Ungültiger GeneratorModus: " & genmod.ToString())
            End If

            Dim isStoneSet As Boolean = (value And &H8) <> 0
            Dim isBase152 As Boolean = (value And &H4) <> 0
            Dim count As Integer = value And &H3

            Return (isStoneSet, isBase152, count)

        End Function

#End Region

#Region "Klassen für die Steinerzeugung - vereinfacht"

        Private Shared Function CreateGeneratorRandom() As Random

            Dim iniSeed As Integer = INI.SpielsteinGenerator_DebugMode

            If iniSeed = 0 Then
                Return New Random()
            Else
                Return New Random(iniSeed + 3000)
            End If

        End Function

        Private Shared Function BuildStoneSetStones(halfSetCount As Integer,
                                            isBase152 As Boolean,
                                            rnd As Random) As List(Of SteinSymbol)

            If halfSetCount < 1 OrElse halfSetCount > 4 Then
                Throw New ArgumentOutOfRangeException(NameOf(halfSetCount),
                                              "halfSetCount muss 1 bis 4 sein.")
            End If

            If rnd Is Nothing Then
                Throw New ArgumentNullException(NameOf(rnd))
            End If

            Dim stones As New List(Of SteinSymbol)

            Dim startWithFlowers As Boolean = (rnd.Next(2) = 0)

            For halfSetIdx As Integer = 0 To halfSetCount - 1

                AddNormalHalfSet(stones)

                If isBase152 Then

                    AddSpecialSingles(stones, FlowerSteinsymbole, rnd)
                    AddSpecialSingles(stones, SeasonSteinsymbole, rnd)

                Else

                    Dim addFlowers As Boolean

                    If startWithFlowers Then
                        addFlowers = ((halfSetIdx And 1) = 0)
                    Else
                        addFlowers = ((halfSetIdx And 1) <> 0)
                    End If

                    If addFlowers Then
                        AddSpecialSingles(stones, FlowerSteinsymbole, rnd)
                    Else
                        AddSpecialSingles(stones, SeasonSteinsymbole, rnd)
                    End If

                End If

            Next

            ShuffleInPlace(stones, rnd)

            Return stones

        End Function

        Private Shared Sub AddNormalHalfSet(stones As List(Of SteinSymbol))

            '34 normale Steintypen, je ein Paar = 68 Steine
            For idx As Integer = 0 To NormalSteinsymbole.Length - 1
                Dim sie As SteinSymbol = NormalSteinsymbole(idx)
                stones.Add(sie)
                stones.Add(sie)
            Next

        End Sub

        Private Shared Sub AddSpecialSingles(stones As List(Of SteinSymbol),
                                     source() As SteinSymbol,
                                     rnd As Random)

            'Blumen/Jahreszeiten sind keine identischen Paare.
            'Sie bilden über SFInfo.GetSteinClickGruppe() eine gemeinsame Klickgruppe.
            Dim arr() As SteinSymbol = CType(source.Clone(), SteinSymbol())
            ShuffleInPlace(arr, rnd)

            For idx As Integer = 0 To arr.Length - 1
                stones.Add(arr(idx))
            Next

        End Sub

        Private Shared Function BuildStoneStreamPortion(pairCount As Integer,
                                                isBase152 As Boolean,
                                                rnd As Random) As List(Of SteinSymbol)

            If pairCount < 1 Then
                Return New List(Of SteinSymbol)
            End If

            If rnd Is Nothing Then
                Throw New ArgumentNullException(NameOf(rnd))
            End If

            Dim stones As New List(Of SteinSymbol)(pairCount * 2)

            For pairIdx As Integer = 1 To pairCount

                Dim group As SteinRndGruppe = DrawStreamGroup(isBase152, rnd)

                Select Case group

                    Case SteinRndGruppe.Normal
                        Dim sie As SteinSymbol = NormalSteinsymbole(rnd.Next(NormalSteinsymbole.Length))
                        stones.Add(sie)
                        stones.Add(sie)

                    Case SteinRndGruppe.Flower
                        AddRandomSpecialPair(stones, FlowerSteinsymbole, rnd)

                    Case SteinRndGruppe.Season
                        AddRandomSpecialPair(stones, SeasonSteinsymbole, rnd)

                    Case Else
                        Throw New InvalidOperationException("Unbekannte SteinRndGruppe.")

                End Select

            Next

            ShuffleInPlace(stones, rnd)

            Return stones

        End Function

        Private Shared Function DrawStreamGroup(isBase152 As Boolean,
                                        rnd As Random) As SteinRndGruppe

            'Base144-HalfSet:
            '34 Normalpaare + 1 Blumen-Paar + 1 Jahreszeiten-Paar = 36 Paare
            '
            'Base152-HalfSet:
            '34 Normalpaare + 2 Blumen-Paare + 2 Jahreszeiten-Paare = 38 Paare

            Dim normalWeight As Integer = 34
            Dim flowerWeight As Integer
            Dim seasonWeight As Integer

            If isBase152 Then
                flowerWeight = 2
                seasonWeight = 2
            Else
                flowerWeight = 1
                seasonWeight = 1
            End If

            Dim sum As Integer = normalWeight + flowerWeight + seasonWeight
            Dim n As Integer = rnd.Next(sum)

            If n < normalWeight Then
                Return SteinRndGruppe.Normal
            End If

            n -= normalWeight

            If n < flowerWeight Then
                Return SteinRndGruppe.Flower
            End If

            Return SteinRndGruppe.Season

        End Function

        Private Shared Sub AddRandomSpecialPair(stones As List(Of SteinSymbol),
                                        source() As SteinSymbol,
                                        rnd As Random)

            Dim idx1 As Integer = rnd.Next(source.Length)
            Dim idx2 As Integer = rnd.Next(source.Length - 1)

            If idx2 >= idx1 Then
                idx2 += 1
            End If

            stones.Add(source(idx1))
            stones.Add(source(idx2))

        End Sub

        Private Shared Sub ShuffleInPlace(Of T)(list As IList(Of T),
                                        rnd As Random)

            For idx1 As Integer = list.Count - 1 To 1 Step -1
                Dim idx2 As Integer = rnd.Next(idx1 + 1)

                Dim tmp As T = list(idx1)
                list(idx1) = list(idx2)
                list(idx2) = tmp
            Next

        End Sub

#End Region
    End Class

End Namespace