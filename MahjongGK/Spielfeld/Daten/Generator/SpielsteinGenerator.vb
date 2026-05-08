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

    'Public Enum SteinTyp
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

#Region "Instanzierung und Initialisierung"

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
            StockNoSortAreaEndIndex = INI.SpielsteinGenerator_VorratNoSortAreaEndIndexDefault 'Default = 10
            StockMaxUBound = INI.SpielsteinGenerator_VorratMaxUBoundDefault 'Default = MJ_STEINE_MAXCOUNT
            StockNachschubschwelle = INI.SpielsteinGenerator_VorratNachschubschwelleDefault ' Default = 100
            CheckAndRefillVorrat()
        End Sub
        '
        ''' <summary>
        ''' Initialisiert den Zufallsgenerator.
        ''' Mehrfaches aufrufen ist schadlos.
        ''' </summary>
        Private Sub InitGenerator()
            If _packGen Is Nothing Then
                Dim cfg As New GeneratorConfig(k:=0.1)
                Dim stats As New PackStats()
                _packGen = New PackGenerator(cfg, stats)
            End If
        End Sub

#End Region

#Region "Deklarationen"

        Private _packGen As PackGenerator = Nothing

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
        ''' der VorratskisteLength. Überprüft wird es bei der Initialisierung und bei jeder Steinentnahme.
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
        Public Property Stock As List(Of SteinTyp)
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
        '
        ''' <summary>
        ''' Erstellt eine echte tiefe Kopie von SteinInfo.
        ''' Alle Wertetypen/Enums sind ohnehin kopiert; Referenzen werden manuell geklont.
        ''' </summary>
        Public Function DeepCopy() As SFInfo

            ' Flache Kopie aller Felder (inkl. privater Backing Fields wie _AnimMaxStep, _AnimLoops)
            Dim c As SFInfo = DirectCast(Me.MemberwiseClone(), SFInfo)

            ' Tiefer Kopieren aller Referenztypen/Felder:
            ' Triple ist eine Klasse → explizit klonen

            ' Falls später weitere Referenztypen hinzukommen, hier analog tief kopieren.

            Return c

            'Antwort von ChatGPT, ob man das über Serialisierung/Deserialisierung
            'lösen könne:

            'Warum das besser ist als (De-)Serialisierung:
            '<XmlIgnore>-Felder würden bei XML-Serialisierung fehlen → unvollständige Kopie.
            'Keine Attribute / Type - Resolver nötig, kein Overhead.
            'Setter-Logiken(wie AnimMaxStep mit ×100) werden nicht versehentlich erneut ausgeführt
            '— die privaten Backing-Felder werden 1:1 kopiert, genau wie gewünscht.
            'Da Triple bereits eine saubere DeepCopy liefert, reicht das oben völlig aus.
            '
            'Die frühere Methode über den BinaryFormatter ist von Microsoft als depreceted eingestuft.
            '
            'Es ginge noch über den DataContractSerializer, das ist aber mit Vergabe vieler Attribute
            'versehen.
        End Function

#End Region

#Region "statisch"
        '
        Public Shared ReadOnly NORMALS As SteinTyp() = {
            SteinTyp.Punkt01, SteinTyp.Punkt02, SteinTyp.Punkt03, SteinTyp.Punkt04, SteinTyp.Punkt05,
            SteinTyp.Punkt06, SteinTyp.Punkt07, SteinTyp.Punkt08, SteinTyp.Punkt09,
            SteinTyp.Bambus1, SteinTyp.Bambus2, SteinTyp.Bambus3, SteinTyp.Bambus4, SteinTyp.Bambus5,
            SteinTyp.Bambus6, SteinTyp.Bambus7, SteinTyp.Bambus8, SteinTyp.Bambus9,
            SteinTyp.Symbol1, SteinTyp.Symbol2, SteinTyp.Symbol3, SteinTyp.Symbol4, SteinTyp.Symbol5,
            SteinTyp.Symbol6, SteinTyp.Symbol7, SteinTyp.Symbol8, SteinTyp.Symbol9,
            SteinTyp.DracheR, SteinTyp.DracheG, SteinTyp.DracheW,
            SteinTyp.WindOst, SteinTyp.WindSüd, SteinTyp.WindWst, SteinTyp.WindNrd
        } ' = 34

        Public Shared ReadOnly FLOWERS As SteinTyp() = {
        SteinTyp.BlütePf, SteinTyp.BlüteOr, SteinTyp.BlüteCt, SteinTyp.BlüteBa
        } ' = 4

        Public Shared ReadOnly SEASONS As SteinTyp() = {
        SteinTyp.JahrFrl, SteinTyp.JahrSom, SteinTyp.JahrHer, SteinTyp.JahrWin
        } ' = 4

#End Region

#Region "Öffentliches"

        Public Function GetGroup(ByVal idx As SteinTyp) As SteinRndGruppe
            Dim i As Integer
            For i = 0 To NORMALS.Length - 1
                If NORMALS(i) = idx Then Return SteinRndGruppe.Normal
            Next
            For i = 0 To FLOWERS.Length - 1
                If FLOWERS(i) = idx Then Return SteinRndGruppe.Flower
            Next
            For i = 0 To SEASONS.Length - 1
                If SEASONS(i) = idx Then Return SteinRndGruppe.Season
            Next
            ' Fallback: behandle Unbekanntes als Normal
            Return SteinRndGruppe.Normal

        End Function
        '
        ''' <summary>
        ''' Nach manuellem clearen des Vorrat kann er hiermit auch manuell
        ''' wieder gefüllt werden.
        ''' Im GeneratorModi.StoneSet wird der Vorrat nur aufgefüllt, 
        ''' wenn refillStoneSet true ist und Vorrat.Count = 0
        ''' _VorratStopNachschub wird bei refillStoneSet = True nicht berücksichtigt.
        ''' </summary>
        Public Sub CheckAndRefillVorrat(Optional refillStoneSet As Boolean = False)

            If Stock Is Nothing Then Stock = New List(Of SteinTyp)()

            Dim genmod As (isStoneSet As Boolean, isBase152 As Boolean, count As Integer) = GetValueFromGeneratorModi(_GeneratorModus)

            If _VorratStopNachschub AndAlso Not refillStoneSet AndAlso Not genmod.isStoneSet Then
                Exit Sub
            End If

            If genmod.isStoneSet Then
                If Stock.Count > 0 AndAlso Not refillStoneSet Then
                    Exit Sub
                End If
            Else 'GeneratorModus = GeneratorModi.StoneStream 
                If Stock.Count > StockNachschubschwelle Then
                    Exit Sub
                End If
            End If

            InitGenerator()

            Dim stones As List(Of SteinTyp)

            If genmod.isStoneSet Then
                '-- endlicher Vorrat aus N Sets)
                Dim stoneSet As New StoneSet(setsCount:=HalfSteinsetsCount, StoneSet152SteineErzeugen)
                _packGen.UseStoneSet(stoneSet)
                stones = _packGen.BuildPacksetOfStones()
            Else
                '_GeneratorModus = GeneratorModi.StoneStream 
                Dim steinpaare As Integer = (StockMaxUBound - Stock.Count) \ 2

                stones = _packGen.BuildPortionStones(steinpaare)
            End If

            Stock.AddRange(stones)

        End Sub
        '
        ''' <summary>
        ''' Gibt den selektierten Stein zurück und löscht ihn im Vorrat.
        ''' Wenn index kleiner 0 OrElse Vorrat.Count = 0 OrElse index > Vorrat.Count - 1
        ''' dann wird die Fehlergrafik zurückgegeben.
        ''' </summary>
        ''' <param name="index"></param>
        ''' <returns></returns>
        Public Function GetSelectedStein(index As Integer) As SteinTyp
            If index < 0 OrElse Stock Is Nothing OrElse Stock.Count = 0 OrElse index > Stock.Count - 1 Then
                Return SteinTyp.ErrorSy 'Die Fehlergrafik
            Else
                Dim retval As SteinTyp = Stock(index)
                Stock.RemoveAt(index)  ' <- exakt diese Position löschen
                Return retval
            End If
        End Function
        '
        ''' <summary>
        ''' Fügt den übergebenen Stein links vom index ein.
        ''' (Zurücklegen eines Steines vom Feld in den Vorrat.)
        ''' Ist index zu klein, wird ganz links eingefügt,
        ''' ist er zu groß ganz rechts.
        ''' </summary>
        ''' <param name="index"></param>
        ''' <param name="sie"></param>
        Public Sub InsertLeftFromSelectedStein(index As Integer, sie As SteinTyp)

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

        Public Sub ShuffleVorrat()
            '
            If Stock.Count <= 1 Then
                'hier gibt es nichts zu mischen
                Exit Sub
            End If

            If Stock.Count - 1 <= StockNoSortAreaEndIndex Then
                'hier auch nicht
                Exit Sub
            End If

            If Stock.Count - 1 <= StockNoSortAreaEndIndex + 2 Then
                'weniger als 2 Steine hinter dem Index vorhanden
                'hier gibt es nichts zu mischen
                Exit Sub
            End If

            Dim idxTo As Integer = StockNoSortAreaEndIndex + 1
            Dim idx1 As Integer

            For idx1 = Stock.Count - 1 To idxTo Step -1
                Dim idx2 As Integer = GetZufallszahl(idxTo, idx1 + 1)
                Dim tmp As SteinTyp = Stock(idx1)
                Stock(idx1) = Stock(idx2)
                Stock(idx2) = tmp
            Next

        End Sub

#End Region

#Region "Statistik"

        ''' <summary>
        ''' Wertet den aktuellen Vorrat aus
        ''' Liefert je SteinTyp einen Single:
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

            For idx As SteinTyp = 0 To CType(MJ_STEININDEX_MAX, SteinTyp)
                Dim absCount As Integer
                Dim ratio As Single
                ParseStatisticValue(stats(idx), absCount, ratio)

                If (Not onlyNonZero) OrElse absCount > 0 Then
                    Dim label As String
                    If showEnumName Then
                        Dim name As String = CType(idx, SteinTyp).ToString()
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
                        If idx = SteinTyp.ErrorSy OrElse
                            idx = SteinTyp.Punkt09 OrElse
                            idx = SteinTyp.Bambus9 OrElse
                            idx = SteinTyp.Symbol9 OrElse
                            idx = SteinTyp.DracheW OrElse
                            idx = SteinTyp.WindNrd OrElse
                            idx = SteinTyp.BlüteBa OrElse
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

#Region "Strohwitwen"

        Private ReadOnly _VorratStrohWitwen As New List(Of SteinTyp)

        ''' <summary>
        ''' Entfernt alls Paare aus dem Vorrat, sodaß anschließend nur noch Strohwitwen darin sind.
        ''' Gibt die Anzahl der Strohwitwen zurück.
        ''' ACHTUNG: Setzt VorratStopNachschub auf True. Muss manuell rückgestellt werden,
        ''' dann wird der Vorrat auch sofort wieder aufgefüllt. (für "weiter Editieren")
        ''' Die Strohwitwen bleiben drin(!) und stehen ganz am Anfang.
        ''' </summary>
        ''' <param name="windsAreInOneClickGroup"></param>
        ''' <returns></returns>
        Public Function RemovePaareFromVorrat(windsAreInOneClickGroup As Boolean) As Integer
            CreateVorratStrohWitwen(windsAreInOneClickGroup)
            Stock = _VorratStrohWitwen
            StockStopNachschub = True
            Return Stock.Count
        End Function

        Public ReadOnly Property VorratStrohWitwen(windsAreInOneClickGroup As Boolean) As List(Of SteinTyp)
            Get
                CreateVorratStrohWitwen(windsAreInOneClickGroup)
                Return _VorratStrohWitwen
            End Get
        End Property

        Public ReadOnly Property VorratHasStrohWitwen(windsAreInOneClickGroup As Boolean) As Boolean
            Get
                CreateVorratStrohWitwen(windsAreInOneClickGroup)
                Return _VorratStrohWitwen.Count > 0
            End Get
        End Property

        Private Sub CreateVorratStrohWitwen(windsAreInOneClickGroup As Boolean)

            _VorratStrohWitwen.Clear()
            If Stock Is Nothing OrElse Stock.Count = 0 Then
                Exit Sub
            End If

            ' Enum ist lückenlos 0..MJ_STEININDEX_MAX
            Dim counts(MJ_STEININDEX_MAX) As Integer

            For i As Integer = 0 To Stock.Count - 1
                Dim idx As Integer = SFInfo.GetSteinClickGruppe(CType(Stock(i), SteinTyp), windsAreInOneClickGroup)
                If idx >= 0 AndAlso idx <= MJ_STEININDEX_MAX Then
                    counts(idx) += 1
                End If
            Next

            ' ---- Sonderfall Fehlergrafik (0): bei Vorkommen immer genau einmal aufnehmen
            If counts(0) > 0 Then
                _VorratStrohWitwen.Add(CType(0, SteinTyp))
            End If

            ' ---- Alle anderen: nur bei ungerader Häufigkeit je einmal aufnehmen
            For idx As Integer = 1 To MJ_STEININDEX_MAX
                If (counts(idx) And 1) <> 0 Then
                    _VorratStrohWitwen.Add(CType(idx, SteinTyp))
                End If
            Next
        End Sub

#End Region

#Region "Hilfsfunktionen GeneratorModi"
        ''' <summary>
        ''' Wandelt die Angaben in die entsprechende Enumeration um.
        ''' </summary>
        ''' <param name="isStoneSet"></param>
        ''' <param name="isBase152"></param>
        ''' <param name="count">Darf die Werte 0, 1, 2 und 3 annehmen. (sonst Exception)</param>
        ''' <returns></returns>
        Public Shared Function GetGeneratorModi(isStoneSet As Boolean,
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

            Dim isStoneSet As Boolean = (value And &H8) <> 0
            Dim isBase152 As Boolean = (value And &H4) <> 0
            Dim count As Integer = value And &H3

            Return (isStoneSet, isBase152, count)

        End Function

#End Region

#Region "Klassen für die Steinerzeugung"

        'Anmerkung von ChatGPT zu den Zufallsgeneratoren
        'Du initialisierst mehrere Random mit fixen Seeds (gut für Reproduzierbarkeit) und an zwei Stellen mit
        'Guid.NewGuid().GetHashCode() (gut für Streuung). Das ist bewusst gemischt – nur Hinweis: deterministische
        'Runs sind damit nicht 100% reproduzierbar. Wenn Du deterministisch testen willst, gib überall Random aus
        'GeneratorConfig rein.

        Public Class GeneratorConfig

            Public Sub New(k As Double)
                Me.K = k

                Dim iniSeed As Integer = INI.SpielsteinGenerator_DebugMode

                If iniSeed = 0 Then
                    ' Normalbetrieb: EINMAL echten Zufall nehmen
                    ' und daraus unterschiedliche Seeds ableiten.
                    Dim baseRnd As New Random()
                    RndSelect = New Random(baseRnd.Next()) 'oder Random(Guid.NewGuid().GetHashCode())
                    RndShuffle = New Random(baseRnd.Next())
                Else
                    ' Debug/Test: deterministisch und unterscheidbar
                    RndSelect = New Random(iniSeed + 1000)
                    RndShuffle = New Random(iniSeed + 2000)
                End If

            End Sub

            ' Verhältnis Normal : (Flower+Season) in PACKS => 17 : 1
            Public Property RatioNormalToSpecial As Double = INI.SpielsteinGenerator_VerhältnisNormalsteineZuSondersteine
            ' Aufteilung des Special-Anteils: 50:50
            Public Property SpecialSplitFlower As Double = 0.5
            ' Sanfter Regler
            Public Property K As Double
            ' Kappung
            Public Property MinWeight As Double = 0.000001
            Public Property MaxWeight As Double = 1000000.0
            ' RNG
            Public Property RndSelect As Random
            Public Property RndShuffle As Random
        End Class

        ' --- Statistik für gewichteten Modus -----------------------------------------
        Public Class PackStats
            Public Property TotalPacks As Integer
            Public ReadOnly NormalPackCount As Integer() = New Integer(NORMALS.Length - 1) {}
            Public Property FlowerPackCount As Integer
            Public Property SeasonPackCount As Integer
        End Class

        Public Structure Pack
            Public ReadOnly A As SteinTyp
            Public ReadOnly B As SteinTyp
            Public Sub New(ByVal a As SteinTyp, ByVal b As SteinTyp)
                Me.A = a
                Me.B = b
            End Sub
        End Structure

        ' --- StoneSet (endlicher Vorrat) --------------------------------------------------

        Public Class StoneSet

            Private ReadOnly _packs As List(Of Pack)

            Private _cursor As Integer

            Private _toggleFlag As Boolean
            Private ReadOnly _startRndFlag As Boolean
            Private ReadOnly _stoneSet152 As Boolean
            Private ReadOnly _rndShuffleInPlace As Random

            Public Sub New(ByVal setsCount As Integer, ByVal stoneSet152 As Boolean)

                Dim iniSeed As Integer = INI.SpielsteinGenerator_DebugMode

                If iniSeed = 0 Then
                    _rndShuffleInPlace = New Random(Guid.NewGuid().GetHashCode())
                Else
                    _rndShuffleInPlace = New Random(iniSeed + 4000)
                End If

                _packs = BuildStoneSets(setsCount)
                ShuffleInPlace(_packs)
                _cursor = 0
                _stoneSet152 = stoneSet152
                _startRndFlag = GetZufallTrueFalse()

            End Sub

            Public Function RemainingPacks() As Integer
                Return _packs.Count - _cursor
            End Function

            Public Function TryDrawPack(ByRef pack As Pack) As Boolean
                If _cursor >= _packs.Count Then
                    pack = NothingPack()
                    Return False
                End If
                pack = _packs(_cursor)
                _cursor += 1
                Return True
            End Function

            Private Shared Function NothingPack() As Pack
                Return New Pack(NORMALS(0), NORMALS(0))
            End Function

            Private Function BuildStoneSets(ByVal setsCount As Integer) As List(Of Pack)
                Dim packs As New List(Of Pack)
                Dim s As Integer
                For s = 1 To setsCount
                    ' Normale: pro Typ 4 Steine => 2 Packs (identische Paare)
                    Dim i As Integer
                    For i = 0 To NORMALS.Length - 1
                        ' ergibt 34 Steinpaare, also 68 Steine.
                        Dim idx As SteinTyp = NORMALS(i)
                        packs.Add(New Pack(idx, idx))
                        packs.Add(New Pack(idx, idx))
                    Next

                    If _stoneSet152 Then
                        AddFlowers(packs)
                        AddSeasons(packs)
                    Else
                        'im StoneSet144 werden abwechselnd Steine geliefert.
                        'Grund: Im vollständigem Satz gibt es 144 Einzelsteine =
                        '2 x 68 = 136 Steine + 2 x die je 4 Steine AddSeasons ODER AddFlowers = 144 
                        '2 x 68 = 136 Steine + 2 x die je 4 Steine AddSeasons UND AddFlowers = 152  
                        If _startRndFlag Then
                            If _toggleFlag Then
                                AddSeasons(packs)
                            Else
                                AddFlowers(packs)
                            End If
                        Else
                            If _toggleFlag Then
                                AddFlowers(packs)
                            Else
                                AddSeasons(packs)
                            End If
                        End If
                    End If

                    _toggleFlag = Not _toggleFlag

                Next
                Return packs
            End Function

            Private Sub AddFlowers(packs As List(Of Pack))
                ' Flowers: 4 Einzelsteine => 2 Packs
                Dim arr As SteinTyp() = CType(FLOWERS.Clone(), SteinTyp())
                ShuffleInPlace(arr)
                packs.Add(New Pack(arr(0), arr(1)))
                packs.Add(New Pack(arr(2), arr(3)))
            End Sub

            Private Sub AddSeasons(packs As List(Of Pack))
                ' Seasons: 4 Einzelsteine => 2 Packs
                Dim arr As SteinTyp() = CType(SEASONS.Clone(), SteinTyp())
                ShuffleInPlace(arr)
                packs.Add(New Pack(arr(0), arr(1)))
                packs.Add(New Pack(arr(2), arr(3)))
            End Sub

            Private Sub ShuffleInPlace(Of T)(ByVal list As IList(Of T))
                Dim i As Integer
                For i = list.Count - 1 To 1 Step -1
                    Dim j As Integer = _rndShuffleInPlace.Next(i + 1)
                    Dim tmp As T = list(i)
                    list(i) = list(j)
                    list(j) = tmp
                Next
            End Sub

            Private Sub ShuffleInPlace(Of T)(ByVal arr As T())
                Dim i As Integer
                For i = arr.Length - 1 To 1 Step -1
                    Dim j As Integer = _rndShuffleInPlace.Next(i + 1)
                    Dim tmp As T = arr(i)
                    arr(i) = arr(j)
                    arr(j) = tmp
                Next
            End Sub
        End Class

        ' --- Generator (gewichteter Modus ODER Stoneset) ----------------------------------

        Public Class PackGenerator

            Private ReadOnly _cfg As GeneratorConfig
            Private ReadOnly _stats As PackStats
            Private _stoneSet As StoneSet ' optional

            Public Sub New(ByVal cfg As GeneratorConfig, ByVal stats As PackStats)
                _cfg = cfg
                _stats = stats
            End Sub

            Public Sub UseStoneSet(ByVal shoe As StoneSet)
                _stoneSet = shoe
            End Sub

            ''' <summary>
            ''' Im Modus StoneSet werden alle Steine zurückgegeben, 
            ''' im kontinuierlichem Modus so viele Steine, wie in
            ''' INI.Spielfeld_StandardMengeSteinPaareJePortion angegebn.
            ''' </summary>
            ''' <returns></returns>
            Public Function BuildPacksetOfStones() As List(Of SteinTyp)

                If _stoneSet IsNot Nothing Then
                    Return BuildPortionStones(9999)
                Else
                    Return BuildPortionStones(0)
                End If
            End Function

            Public Function BuildPortionStones(ByVal packsPerPortion As Integer) As List(Of SteinTyp)

                If packsPerPortion = 0 Then
                    Throw New Exception("packsPerPortion darf nicht 0 sein.(Hinweis: UseStoneSet mit überprüfen.)")
                End If

                Dim portionPacks As New List(Of Pack)
                Dim pass As Integer
                For pass = 1 To packsPerPortion
                    Dim p As Pack
                    If _stoneSet IsNot Nothing Then
                        If Not _stoneSet.TryDrawPack(p) Then Exit For
                    Else
                        p = DrawOnePackStoneStream()
                    End If
                    portionPacks.Add(p)
                Next

                Dim tiles As New List(Of SteinTyp)(portionPacks.Count * 2)
                Dim k As Integer
                For k = 0 To portionPacks.Count - 1
                    tiles.Add(portionPacks(k).A)
                    tiles.Add(portionPacks(k).B)
                Next

                ' Anzeige-Mischen
                ShuffleInPlace(tiles, _cfg.RndShuffle)
                Return tiles
            End Function

            ' ------------ Gewichtete Ziehung (wenn kein Shoe gesetzt) ------------------

            Private Structure Kandidat
                Public Kind As SteinRndGruppe
                Public NormalIndex As Integer ' 0..33, sonst -1 für Flower/Season
                Public Weight As Double
            End Structure

            Private Function DrawOnePackStoneStream() As Pack
                ' 34 Normal-Typen + FlowerPack + SeasonPack = 36 Kandidaten
                Dim Kandidats As New List(Of Kandidat)(NORMALS.Length + 2)

                ' Basisgewichte so, dass Summe Normal ~ 17 und Special ~ 1
                Dim baseNormal As Double = (_cfg.RatioNormalToSpecial) / CDbl(NORMALS.Length) ' 17/34=0,5
                Dim baseFlower As Double = 1.0R * _cfg.SpecialSplitFlower                                     ' 0,5
                Dim baseSeason As Double = 1.0R * (1.0R - _cfg.SpecialSplitFlower)                           ' 0,5

                Dim t As Integer = Math.Max(1, _stats.TotalPacks)
                Dim targetPerSlot As Double = CDbl(_stats.TotalPacks) * (1.0R / 36.0R)

                ' Normale
                Dim i As Integer
                For i = 0 To NORMALS.Length - 1
                    Dim ist As Integer = _stats.NormalPackCount(i)
                    Dim deficit As Double = targetPerSlot - ist
                    Dim w As Double = baseNormal * (1.0R + _cfg.K * deficit)
                    w = Clamp(w, _cfg.MinWeight, _cfg.MaxWeight)
                    Kandidats.Add(New Kandidat With {.Kind = SteinRndGruppe.Normal, .NormalIndex = i, .Weight = w})
                Next

                ' Flower
                Dim istF As Integer = _stats.FlowerPackCount
                Dim wF As Double = baseFlower * (1.0R + _cfg.K * (targetPerSlot - istF))
                Kandidats.Add(New Kandidat With {.Kind = SteinRndGruppe.Flower, .NormalIndex = -1, .Weight = Clamp(wF, _cfg.MinWeight, _cfg.MaxWeight)})

                ' Season
                Dim istS As Integer = _stats.SeasonPackCount
                Dim wS As Double = baseSeason * (1.0R + _cfg.K * (targetPerSlot - istS))
                Kandidats.Add(New Kandidat With {.Kind = SteinRndGruppe.Season, .NormalIndex = -1, .Weight = Clamp(wS, _cfg.MinWeight, _cfg.MaxWeight)})

                ' Roulette
                Dim choice As Kandidat = StoneStreamChoice(Kandidats, _cfg.RndSelect)

                ' Pack + Statistik
                Select Case choice.Kind
                    Case SteinRndGruppe.Normal
                        Dim idx As SteinTyp = NORMALS(choice.NormalIndex)
                        _stats.NormalPackCount(choice.NormalIndex) += 1
                        _stats.TotalPacks += 1
                        Return New Pack(idx, idx)

                    Case SteinRndGruppe.Flower
                        Dim a As SteinTyp = RandomOf(FLOWERS, _cfg.RndSelect)
                        Dim b As SteinTyp = RandomOf(FLOWERS, _cfg.RndSelect)
                        _stats.FlowerPackCount += 1
                        _stats.TotalPacks += 1
                        Return New Pack(a, b)

                    Case SteinRndGruppe.Season
                        Dim a2 As SteinTyp = RandomOf(SEASONS, _cfg.RndSelect)
                        Dim b2 As SteinTyp = RandomOf(SEASONS, _cfg.RndSelect)
                        _stats.SeasonPackCount += 1
                        _stats.TotalPacks += 1
                        Return New Pack(a2, b2)

                    Case Else
                        Throw New InvalidOperationException("Unbekannter Kandidat.")
                End Select
            End Function

            ' --- Utilities -------------------------------------------------------------

            Private Shared Function Clamp(ByVal v As Double, ByVal lo As Double, ByVal hi As Double) As Double
                If v < lo Then Return lo
                If v > hi Then Return hi
                Return v
            End Function

            Private Shared Function RandomOf(ByVal arr As SteinTyp(), ByVal rnd As Random) As SteinTyp
                Return arr(rnd.Next(arr.Length))
            End Function

            Private Shared Sub ShuffleInPlace(Of T)(ByVal list As IList(Of T), ByVal rnd As Random)
                Dim i As Integer
                For i = list.Count - 1 To 1 Step -1
                    Dim j As Integer = rnd.Next(i + 1)
                    Dim tmp As T = list(i)
                    list(i) = list(j)
                    list(j) = tmp
                Next
            End Sub

            Private Shared Function StoneStreamChoice(ByVal Kandidats As List(Of Kandidat), ByVal rnd As Random) As Kandidat
                Dim sum As Double = 0.0R
                Dim i As Integer
                For i = 0 To Kandidats.Count - 1
                    sum += Kandidats(i).Weight
                Next
                Dim u As Double = rnd.NextDouble() * sum
                Dim acc As Double = 0.0R
                For i = 0 To Kandidats.Count - 1
                    acc += Kandidats(i).Weight
                    If u < acc Then Return Kandidats(i)
                Next
                Return Kandidats(Kandidats.Count - 1) ' Fallback
            End Function
        End Class

    End Class

#End Region

End Namespace