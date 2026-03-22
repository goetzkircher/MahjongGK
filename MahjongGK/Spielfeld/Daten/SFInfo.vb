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

Imports System.IO
Imports System.Text
Imports System.Xml
Imports System.Xml.Serialization

#Disable Warning IDE0079
#Disable Warning IDE1006

Namespace Spielfeld
    ''' <summary>
    ''' Pfad: MahjongGK/Spielfeld/Daten
    ''' </summary>
    Public Class SFInfo

#Region "Konstruktor / Initialisierung"

        Public Sub New()

        End Sub

        Public Sub New(owner As SFDaten)
            _sfd = owner
        End Sub

        Private _sfd As SFDaten

        Public Sub SetOwner(owner As SFDaten)
            _sfd = owner
        End Sub

        ''' <summary>
        ''' spielSize ist die Anzahl der Steine, die maximal nebeneinander und untereinander
        ''' auf dem Feld Platz haben. Z ist die Anzahl der Schichten übereinander.
        ''' </summary>
        ''' <param name="spielSize"></param>
        Public Sub New(owner As SFDaten, spielSize As Triple)
            _sfd = owner
            Initialisierung(spielSize)
        End Sub

        Public Sub New(spielsize As Triple)

            If Not CheckSpielsize(spielsize) Then
                Throw New Exception("Ungültige Spielfeldgröße.")
            End If

            Initialisierung(spielsize)

        End Sub

        Public Shared Function CheckSpielsize(spielsize As Triple) As Boolean
            With spielsize
                If .x < MJ_STEINE_MINX Then
                    Return False
                ElseIf .y < MJ_STEINE_MINY Then
                    Return False
                ElseIf .z < MJ_STEINE_MINZ Then
                    Return False
                    '
                ElseIf .x > MJ_STEINE_MAXX Then
                    Return False
                ElseIf .y < MJ_STEINE_MINY Then
                    Return False
                ElseIf .z > MJ_STEINE_MAXZ Then
                    Return False
                End If
            End With

            Return True

        End Function

        Private Sub Initialisierung(spielSize As Triple)

            If CheckSpielsize(spielSize) Then
                copySpielSizeToXyzValues(spielSize)
            Else
                Throw New Exception("Ungültige Spielfelddimensionen.")
            End If

            Me.SpielSize = spielSize

            ReDim arrFB(xUBnd, yUBnd, zUBnd)

            SteinInfos = New List(Of SteinInfo)

        End Sub

        Private Sub copySpielSizeToXyzValues(spielSize As Triple)

            With spielSize
                'Die äußerten Felder rings herum bleiben frei, d.h. es gilt:
                'For idx = 1 to ArrFB.UBound(1) -1
                '   ...
                'Next
                'Der Grund: Abfragen auf idx - 1 und idx + 1 brauchen nicht abgefangen werden.

                _xMin = 1
                _yMin = 1
                _zMin = 0
                _xMax = .x * 2
                _yMax = .y * 2
                _zMax = .z - 1

                _xUBnd = _xMax + 1
                _yUBnd = _yMax + 1
                _zUBnd = _zMax
                _xMaxSteine = .x
                _yMaxSteine = .y
                _zMaxSteine = _zMax + 1

            End With

        End Sub

#End Region

#Region "Persistente Kerndaten"

        Public Property Version As String = "1"

        Public Name As String

        <XmlElement(ElementName:="PersistentIdent")>
        Public _persistentIdent As String = Helfer.NewIdent
        '
        ''' <summary>
        ''' Eindeutige Identifikation (persistent).
        ''' </summary>
        Public ReadOnly Property PersistentIdent As String
            Get
                Return _persistentIdent
            End Get
        End Property
        '
        ''' <summary>
        ''' temporäre Ident,(nicht persistent) wird bei jedem Erzeugen der Spielfeldinfo erneut vergeben,
        ''' auch beim Laden der XML-Datei von Festplatte.
        ''' </summary>
        <XmlIgnore>
        Private _sessionIdent As String = Guid.NewGuid.ToString

        Public ReadOnly Property SessionIdent As String
            Get
                Return _sessionIdent
            End Get
        End Property

        Public Function HasSameSessionIdent(lastSessionIdent As String) As Boolean
            If String.IsNullOrEmpty(lastSessionIdent) Then
                Return False
            Else
                Return lastSessionIdent = _sessionIdent
            End If
        End Function

#End Region

#Region "Dimensionen / Bounds"

        Private _xMin As Integer
        Private _yMin As Integer
        Private _zMin As Integer
        Private _xMax As Integer
        Private _yMax As Integer
        Private _zMax As Integer

        Private _xUBnd As Integer
        Private _yUBnd As Integer
        Private _zUBnd As Integer
        Private _xMaxSteine As Integer
        Private _yMaxSteine As Integer
        Private _zMaxSteine As Integer

        Public Property SpielSize As Triple
        '
        Public ReadOnly Property xMin As Integer
            Get
                Return _xMin
            End Get
        End Property
        '
        Public ReadOnly Property yMin As Integer
            Get
                Return _yMin
            End Get
        End Property
        '
        Public ReadOnly Property zMin As Integer
            Get
                Return _zMin
            End Get
        End Property
        '
        Public ReadOnly Property xMax As Integer
            Get
                Return _xMax
            End Get
        End Property
        Public ReadOnly Property yMax As Integer
            Get
                Return _yMax
            End Get
        End Property
        Public ReadOnly Property zMax As Integer
            Get
                Return _zMax
            End Get
        End Property

        Public ReadOnly Property xUBnd As Integer
            Get
                Return _xUBnd
            End Get
        End Property
        Public ReadOnly Property yUBnd As Integer
            Get
                Return _yUBnd
            End Get
        End Property
        Public ReadOnly Property zUBnd As Integer
            Get
                Return _zUBnd
            End Get
        End Property

        Public ReadOnly Property xMaxSteine As Integer
            Get
                Return _xMaxSteine
            End Get
        End Property
        Public ReadOnly Property yMaxSteine As Integer
            Get
                Return _yMaxSteine
            End Get
        End Property
        Public ReadOnly Property zMaxSteine As Integer
            Get
                Return _zMaxSteine
            End Get
        End Property

        '
        ''' <summary>
        ''' Diese Property dient nur dazu in der Xml-Datei an dieser Stelle zu stehen,
        ''' damit man die Anzahl der folgenden SteinInfos sehen kann.
        ''' Sie wird beim Speichern gesetzt und muß eine Public Property sein,
        ''' sonst erscheint sie nicht in er Xml.
        ''' </summary>
        ''' <returns></returns>
        Public Property SummeSteinInfos As Integer
        '
        Public Property Speicherdatum As Date
        '
        ''' <summary>
        ''' Gibt die aktuellen 3D-Grenzwerte als Struktur zurück.
        ''' </summary>
        ''' <returns>Bounds3D mit x/y/z-Min/Max</returns>
        Public Function GetBounds3D() As Bounds3D

            Return New Bounds3D(Me.xMin, Me.xMax,
                        Me.yMin, Me.yMax,
                        Me.zMin, Me.zMax)

        End Function

#End Region

#Region "SteinInfos - Felder / Zugriff"

        Private _SteinInfos As List(Of SteinInfo) = New List(Of SteinInfo)
        'Private ReadOnly _SteinInfos As List(Of SteinInfo) = New List(Of SteinInfo)

        'Public ReadOnly Property SteinInfos As IReadOnlyList(Of SteinInfo)
        Public Property SteinInfos As List(Of SteinInfo)
            Get
                Return _SteinInfos
            End Get
            Set(value As List(Of SteinInfo))
                _SteinInfos = value
            End Set
        End Property

        Private _SteinInfosCountChanged As Boolean

        Public ReadOnly Property ConsumeSteinInfosCountChanged As Boolean
            Get
                If _SteinInfosCountChanged Then
                    _SteinInfosCountChanged = False
                    Return True
                Else
                    Return False
                End If
            End Get
        End Property

#End Region

#Region "SteinInfos - interne Verwaltung"

        Private Sub AddSteinInfoInternal(newSteinInfo As SteinInfo)

            _SteinInfos.Add(newSteinInfo)
            NotifySteinInfosChanged()

        End Sub

        Private Sub RemoveSteinInfoInternal(index As Integer)

            ' später

        End Sub

        Private Sub ClearSteinInfosInternal()

            ' später

        End Sub

        Private Sub RebuildSteinInfoIndices()

            ' später

        End Sub

        Private Sub NotifySteinInfosChanged()

            SyncToggleFlag()
            MarkTopSearchDirty()
            MarkSteinCountChanged()

            Update_SteinInfos_RectQuadranten()

        End Sub

        Private Sub MarkTopSearchDirty()
            _indexTopSearchDirty = True
        End Sub

        Private Sub MarkSteinCountChanged()
            _SteinInfosCountChanged = True
        End Sub

#End Region

#Region "Serialisierung / Laufzeitobjekte"

        <XmlIgnore>
        Public Property Generator As SpielsteinGenerator = Nothing

        Public Property GeneratorValuesForXml As SpielsteinGeneratorValuesForXml = Nothing

        <XmlIgnore>
        Public Property arrFB As Integer(,,)

        <XmlElement("ArrFB")>
        Public Property arrFB_Anchors As FBCodec2x2.FBAnchors
            Get
                Return FBCodec2x2.ToAnchors(arrFB, 1000)
            End Get
            Set(value As FBCodec2x2.FBAnchors)
                If value Is Nothing Then
                    arrFB = Nothing
                Else
                    arrFB = FBCodec2x2.FromAnchors(value, 1000)
                End If
            End Set
        End Property

#End Region

#Region "Hintergrunddaten / Toolbox-Werte"

        Public Property Toolbox_HGrdEditorUseSplFldValues As Boolean

        Private _hgrdSplFldColorIsInit As Boolean
        Private _toolbox_HGrdSplFldColor As Color
        Private _hgrdSplFldColor As Color

        Private _hgrdEditorColorIsInit As Boolean
        Private _toolbox_HGrdEditorColor As Color
        Private _hgrdEditorColor As Color

        Private _hgrdSplFldBitmapNameIsInit As Boolean
        Private _toolbox_HGrdSplFldBitmapName As String
        Private _hgrdSplFldBitmapName As String

        Private _hgrdEditorBitmapNameIsInit As Boolean
        Private _toolbox_HGrdEditorBitmapName As String
        Private _hgrdEditorBitmapName As String

        Private _hgrdSplFldBitmapIsUserGrafikIsInit As Boolean
        Private _toolbox_HGrdSplFldBitmapIsUserGrafik As Boolean
        Private _hgrdSplFldBitmapIsUserGrafik As Boolean

        Private _hgrdEditorBitmapIsUserGrafikIsInit As Boolean
        Private _toolbox_HGrdEditorBitmapIsUserGrafik As Boolean
        Private _hgrdEditorBitmapIsUserGrafik As Boolean

        Private _hgrdSplFldRenderModeIsInit As Boolean
        Private _toolbox_HGrdSplFldRenderMode As Images.BackgroundImageRenderMode
        Private _hgrdSplFldRenderMode As Images.BackgroundImageRenderMode

        Private _hgrdEditorRenderModeIsInit As Boolean
        Private _toolbox_HGrdEditorRenderMode As Images.BackgroundImageRenderMode
        Private _hgrdEditorRenderMode As Images.BackgroundImageRenderMode

        Private _hgrdEditorShowFramingIsInit As Boolean
        Private _toolbox_hgrdEditorShowFraming As Boolean
        Private _hgrdEditorShowFraming As Boolean

#End Region

#Region "Hintergrund - öffentliche Properties"
        '
        ''' <summary>
        ''' Nur für den Zugriff der Toolbox.
        ''' Für normale Zugriffe die Variante ohne das Präfix Toolbox_ verwenden.
        ''' </summary>
        Public Property Toolbox_HGrdSplFldColor As Color
            Get
                Return _toolbox_HGrdSplFldColor
            End Get
            Set(value As Color)
                _toolbox_HGrdSplFldColor = value
                _hgrdSplFldColorIsInit = False
                INI.Rendering_SetConsumeDoRendering()
            End Set
        End Property

        Public Property Toolbox_HGrdEditorColor As Color
            Get
                Return _toolbox_HGrdEditorColor
            End Get
            Set(value As Color)
                _toolbox_HGrdEditorColor = value
                _hgrdEditorColorIsInit = False
                INI.Rendering_SetConsumeDoRendering()
            End Set
        End Property

        Public Property ToolboxHGrdEditorShowFraming As Boolean
            Get
                Return _toolbox_hgrdEditorShowFraming
            End Get
            Set(value As Boolean)
                _toolbox_hgrdEditorShowFraming = value
                _hgrdEditorShowFramingIsInit = False
                INI.Rendering_SetConsumeDoRendering()
            End Set
        End Property

        Public ReadOnly Property HGrdEditorShowFraming As Boolean
            Get
                If _hgrdEditorShowFramingIsInit Then
                    Return _hgrdEditorShowFraming
                Else
                    _hgrdEditorShowFraming = ToolboxHGrdEditorShowFraming
                    _hgrdEditorShowFramingIsInit = True
                    Return _hgrdEditorShowFraming
                End If
            End Get
        End Property

        Public ReadOnly Property HGrdSplFldColor As Color
            Get
                If _hgrdSplFldColorIsInit Then
                    Return _hgrdSplFldColor
                Else
                    If Toolbox_HGrdSplFldColor.IsEmpty Then
                        _hgrdSplFldColor = INI.Toolbox_HGrdSplFldColorFallback
                        INI.Rendering_SetConsumeDoRendering()
                    Else
                        _hgrdSplFldColor = Toolbox_HGrdSplFldColor
                    End If
                    _hgrdSplFldColorIsInit = True
                    Return _hgrdSplFldColor
                End If
            End Get
        End Property

        Public Property Toolbox_HGrdSplFldBitmapName As String
            Get
                Return _toolbox_HGrdSplFldBitmapName
            End Get
            Set(value As String)
                _toolbox_HGrdSplFldBitmapName = value
                _hgrdSplFldBitmapNameIsInit = False
                INI.Rendering_SetConsumeDoRendering()
            End Set
        End Property

        Public ReadOnly Property HGrdSplFldBitmapName As String
            Get
                If _hgrdSplFldBitmapNameIsInit Then
                    Return _hgrdSplFldBitmapName
                Else
                    If String.IsNullOrEmpty(Toolbox_HGrdSplFldBitmapName) Then
                        _hgrdSplFldBitmapName = INI.Toolbox_HGrdSplFldBitmapNameFallback
                        INI.Rendering_SetConsumeDoRendering()
                    Else
                        _hgrdSplFldBitmapName = Toolbox_HGrdSplFldBitmapName
                    End If
                    _hgrdSplFldBitmapNameIsInit = True
                    Return _hgrdSplFldBitmapName
                End If
            End Get
        End Property

        Public Property Toolbox_HGrdSplFldBitmapIsUserGrafik As Boolean
            Get
                Return _toolbox_HGrdSplFldBitmapIsUserGrafik
            End Get
            Set(value As Boolean)
                _toolbox_HGrdSplFldBitmapIsUserGrafik = value
                _hgrdSplFldBitmapIsUserGrafikIsInit = False
                INI.Rendering_SetConsumeDoRendering()
            End Set
        End Property

        Public ReadOnly Property HGrdSplFldBitmapIsUserGrafik As Boolean
            Get
                If _hgrdSplFldBitmapIsUserGrafikIsInit Then
                    Return _hgrdSplFldBitmapIsUserGrafik
                Else
                    _hgrdSplFldBitmapIsUserGrafik = Toolbox_HGrdSplFldBitmapIsUserGrafik
                    _hgrdSplFldBitmapIsUserGrafikIsInit = True
                    Return _hgrdSplFldBitmapIsUserGrafik
                End If
            End Get
        End Property

        Public Property Toolbox_HGrdSplFldRenderMode As Images.BackgroundImageRenderMode
            Get
                Return _toolbox_HGrdSplFldRenderMode
            End Get
            Set(value As Images.BackgroundImageRenderMode)
                _toolbox_HGrdSplFldRenderMode = value
                _hgrdSplFldRenderModeIsInit = False
                INI.Rendering_SetConsumeDoRendering()
            End Set
        End Property

        Public ReadOnly Property HGrdSplFldRenderMode As Images.BackgroundImageRenderMode
            Get
                If _hgrdSplFldRenderModeIsInit Then
                    Return _hgrdSplFldRenderMode
                Else
                    If _toolbox_HGrdSplFldRenderMode = Images.BackgroundImageRenderMode.None Then
                        _hgrdSplFldRenderMode = INI.Toolbox_HGrdSplFldRenderModeFallback
                        INI.Rendering_SetConsumeDoRendering()
                    Else
                        _hgrdSplFldRenderMode = _toolbox_HGrdSplFldRenderMode
                    End If
                    _hgrdSplFldRenderModeIsInit = True
                    Return _hgrdSplFldRenderMode
                End If
            End Get
        End Property

#End Region

#Region "Hintergrund - Bildcache"

        Private _lastFullpath As String = String.Empty
        Private _lastResult As Boolean

        <XmlIgnore>
        Public Property BitmapUGrdSingleImgCache As Images.BackgroundSingleImageCache = Nothing

        Public ReadOnly Property HasBitmapUGrd As Boolean
            Get
                Dim fullpath As String
                Dim hgrdRenderMode As Images.BackgroundImageRenderMode

                If String.IsNullOrEmpty(HGrdSplFldBitmapName) Then
                    Return False
                Else
                    fullpath = AppDataFullPath(AppDataSubDir.Hintergrundgrafiken, HGrdSplFldBitmapName)
                    hgrdRenderMode = HGrdSplFldRenderMode
                End If

                If IsNothing(BitmapUGrdSingleImgCache) Then
                    If _lastResult = False AndAlso fullpath = _lastFullpath Then
                        'Die Datei war schon mal gesucht und nicht gefunden.
                        Return False
                    End If
                    _lastFullpath = String.Copy(fullpath)
                    If File.Exists(fullpath) Then
                        BitmapUGrdSingleImgCache = New Images.BackgroundSingleImageCache()
                        BitmapUGrdSingleImgCache.Load(AppDataFullPath(AppDataSubDir.Hintergrundgrafiken, HGrdSplFldBitmapName), hgrdRenderMode)
                        _lastResult = True
                        Return True
                    Else
                        _lastResult = False
                        Return False
                    End If
                Else
                    BitmapUGrdSingleImgCache.LoadIfPathChanged(fullpath, hgrdRenderMode)
                    Return True
                End If
            End Get
        End Property

        Public Function GetBitmapUGrd(size As Size) As Bitmap
            Return BitmapUGrdSingleImgCache.GetBitmap(size)
        End Function

        Public Function GetBitmapUGrdDominantColor() As Color
            Return BitmapUGrdSingleImgCache.BackgroundDominantColor
        End Function
#End Region

#Region "Zustand / Status"
        '
        ''' <summary>
        ''' Spielbar ist ein Spiel dann, wenn Daten vorhanden sind und wenn
        ''' wenn alle Paare auf dem Spielfeld vollständig sind, also keine
        ''' "StrohWitwen" oder "Witwer" vorhanden sind und den Werkstattsteinen
        ''' alle andere Grafiken zugeordnet wurden
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property IsPlayable As Boolean
            Get
                If IsNothing(SteinInfos) OrElse SteinInfos.Count = 0 Then
                    Return False
                End If

                Dim arrKlickGruppeCount(MJ_STEININDEX_MAX) As Integer
                Dim foundWerkstattStein As Boolean

                For Each stein As SteinInfo In SteinInfos
                    arrKlickGruppeCount(stein.KlickGruppe) += 1
                    If stein.IsWerkbankStein Then
                        foundWerkstattStein = True
                    End If
                Next

                If foundWerkstattStein Then
                    Return False
                End If

                For idx As Integer = 0 To MJ_STEININDEX_MAX
                    If (arrKlickGruppeCount(idx) Mod 2) <> 0 Then
                        'Für 0 gilt: 0 Mod 2 = 0 -> braucht deshalb nicht abgefangen zu werden,
                        'denn nicht vorhandene Steinpaare brauchen nicht berücksichtigt werden,
                        'ausgenommen, es gibt überhaupt keine Steine. Das ist oben abgefangen.
                        Return False
                    End If
                Next

                Return True

            End Get

        End Property

        Public ReadOnly Property IsEmpty As Boolean
            Get
                'If INI.Debug_StopRendering Then Stop
                If IsNothing(SteinInfos) Then Return True
                If SteinInfos.Count = 0 Then Return True
                If xMax = 0 Then Return True
                If yMax = 0 Then Return True
                Return False
            End Get
        End Property

        ''' <summary>
        ''' Vergleicht diese Instanz mit einer anderen SpielfeldInfo.
        ''' Es wird nur geprüft, ob beide SpielfeldInfo dieselbe Instanz sind.
        ''' spielfeldinfo_Other darf Nothing sein, dann sind sie unterschiedlich.
        ''' Vorsicht Falle: CompareSpielfeldInfo darf natürlich nicht aufgerufen
        ''' werden, wenn die eigene Instanz Nothing ist.
        ''' Das geht mit der anderen Überladung.
        ''' </summary>
        Public Function IsEqual(spielfeldinfo_Other As SFInfo) As Boolean
            Return Me Is spielfeldinfo_Other
        End Function

        ''' <summary>
        ''' Vergleicht zwei SpielfeldInfo-Instanzen.
        ''' Prüft:
        '''   - ob beide Nothing sind (--> Exception("Beide SpielfeldInfo sind Nothing (Initialisierungsfehler).")
        '''   - ob sie auf dieselbe Instanz zeigen
        ''' </summary>
        ''' <param name="spielfeldinfoA">Erste SpielfeldInfo</param>
        ''' <param name="spielfeldinfoB">Zweite SpielfeldInfo</param>
        ''' <returns>True, wenn beide dieselbe Instanz sind, False sonst.</returns>
        Public Shared Function IsEqual(spielfeldinfoA As SFInfo, spielfeldinfoB As SFInfo) As Boolean
            If spielfeldinfoA Is Nothing AndAlso spielfeldinfoB Is Nothing Then
                Throw New Exception("Beide SpielfeldInfo sind Nothing (Initialisierungsfehler).")
            End If

            Return spielfeldinfoA Is spielfeldinfoB
        End Function
        '

#End Region

#Region "Spielfeld - Add / Insert / Remove"

        ''' <summary>
        ''' Setzt einen Stein auf das Spielfeld und Added einen Stein mit Basisinformationen
        ''' zu den SteinInfos. Wenn OK, gibt die Funktion True zurück.
        ''' Sind die Koordinaten des infoFBTriple ungültig, wird der Stein als Stein.Dummy
        ''' (Fehler-Grafik) auf die erste gefundene Position als frei schwebender Stein in
        ''' der obersten Ebene geparkt. Ungültig heißt, es würde ein OutOfRange-Error entstehen.
        ''' Das ist ein Sicherheitsgurt während der Programmentwicklung.
        ''' Die Funktion gibt dann False zurück.
        ''' </summary>
        ''' <param name="steinPos3D"></param>
        ''' <returns></returns>
        Public Function AddSteinToSpielfeld(steinIndex As SteinIndexEnum, steinPos3D As Triple, Optional tmpDebug As Integer = 0) As Boolean

            'Der steinInfoIndex wird hier gesichert, obwohl er gleichlautend ist mit dem
            'Index in SteinInfos. Grund: werden später Steine im Editor entfernt, verschieben sich die
            'Indexnummern in SteinInfos und da muss arrFB aktualisert werden. Dazu braucht man den
            '"alten" steinInfoIndex, eben diesen steinInfoIndex.
            Dim newSteinInfo As New SteinInfo(steinInfoIndex:=SteinInfos.Count, steinIndex, steinPos3D, tmpDebug)

            If Not steinPos3D.IsInsideSpielfeldBounds(arrFB) Then
                'Falsche Positionsangabe.
                'Kein Throw Nex Exception, sondern Anzeige auf dem Spielfeld
                'Fehlergrafik in die oberste Ebene als frei schwebender Stein.
                'Im fertig entwickeltem Programm sollte dieser Teil nicht mehr
                'aufgerufen werden :-)
                '
                'Linke obere Ecke der obersten Ebene
                Dim tpl As New Triple(1, 1, arrFB.GetUpperBound(2))

                Do
                    Dim tplR As Triple = SearchPlace(tpl, direction:=Direction.Right)
                    Select Case tplR.Valide
                        Case ValidePlace.NoFundamentFound   'Zeilenende erreicht.
                            tpl.y += 2 'Eine Steinreihe tiefer weitersuchen
                            tpl.x = 1
                            If tpl.y > arrFB.GetUpperBound(1) - 1 Then
                                'Die ganze oberste Ebene ist vollgepflastert mit Steinen.
                                'Da das ziemlich sicher nicht absichlich geschehen ist,
                                'unterstelle ich, das da viele Fehlergrafiken dabei sind
                                'und breche ab ohne weitere Prüfung und setze den
                                'Stein auf die Position links oben etwas versetzt,
                                'also optisch auf eine Ebene, die es garnicht gibt.
                                tpl.x = 2
                                tpl.y = 2

                                'SetStein rekursiv, aber mit jetzt gültigen Werten aufrufen
                                AddSteinToSpielfeld(SteinIndexEnum.ErrorSy, tpl)
                                Return False
                            End If

                        Case ValidePlace.Yes, ValidePlace.NoFundamentFound
                            ' FoundResult.NoFundament ist in diesem Fall OK, er wird zum freischwebendem Stein.
                            'SetStein rekursiv, aber mit jetzt gültigen Werten aufrufen
                            AddSteinToSpielfeld(SteinIndexEnum.ErrorSy, tplR)
                            Return False
                    End Select
                Loop
            End If
            '
            'Hier ist jetzt die Normal -Routine
            '
            'SteinInfos.Count ist der Index, den der Stein in SteinInfos haben wird.
            CopySteinIndexToSpielfeldPos3DAndSetOffsetXY(steinPos3D, SteinInfoIndex:=SteinInfos.Count)
            '() nicht trennen, der Index muss stimmen. Späterer Zugriff über
            'Dim aktSteininfo As SteinInfo = SteinInfos(indexSteinInfo)
            AddSteinInfoInternal(newSteinInfo)

            Return True

        End Function

        Public Function AddSteinToSpielfeld(wsSteinInfo As SteinInfo, steinPos3D As Triple, Optional tmpDebug As Integer = 0) As Boolean

            'unbedingt von wsSteinInfo eine tiefe Kopie machen!
            'Wird ein Werkstück mehrfach eingefügt, sind plötzlich Zwillinge
            'oder Drillinge auf dem Spielfeld, da die Referenzen auf die
            'gleiche SteinInfo zeigen.

            Dim siDc As SteinInfo = wsSteinInfo.DeepCopy

            With siDc
                .SteinInfoIndex = SteinInfos.Count
                .Pos3D = steinPos3D
            End With

            If Not steinPos3D.IsInsideSpielfeldBounds(arrFB) Then
                'Falsche Positionsangabe.
                'Kein Throw Nex Exception, sondern Anzeige auf dem Spielfeld
                'Fehlergrafik in die oberste Ebene als frei schwebender Stein.
                'Im fertig entwickeltem Programm sollte dieser Teil nicht mehr
                'aufgerufen werden :-)
                '
                'Linke obere Ecke der obersten Ebene
                Dim tpl As New Triple(1, 1, arrFB.GetUpperBound(2))

                Do
                    Dim tplR As Triple = SearchPlace(tpl, direction:=Direction.Right)
                    Select Case tplR.Valide
                        Case ValidePlace.NoFundamentFound   'Zeilenende erreicht.
                            tpl.y += 2 'Eine Steinreihe tiefer weitersuchen
                            tpl.x = 1
                            If tpl.y > arrFB.GetUpperBound(1) - 1 Then
                                'Die ganze oberste Ebene ist vollgepflastert mit Steinen.
                                'Da das ziemlich sicher nicht absichlich geschehen ist,
                                'unterstelle ich, das da viele Fehlergrafiken dabei sind
                                'und breche ab ohne weitere Prüfung und setze den
                                'Stein auf die Position links oben etwas versetzt,
                                'also optisch auf eine Ebene, die es garnicht gibt.
                                tpl.x = 2
                                tpl.y = 2

                                'SetStein rekursiv, aber mit jetzt gültigen Werten aufrufen
                                AddSteinToSpielfeld(SteinIndexEnum.ErrorSy, tpl)
                                Return False
                            End If

                        Case ValidePlace.Yes, ValidePlace.NoFundamentFound
                            ' FoundResult.NoFundament ist in diesem Fall OK, er wird zum freischwebendem Stein.
                            'SetStein rekursiv, aber mit jetzt gültigen Werten aufrufen
                            AddSteinToSpielfeld(SteinIndexEnum.ErrorSy, tplR)
                            Return False
                    End Select
                Loop
            End If
            '
            'Hier ist jetzt die Normal -Routine
            '
            'SteinInfos.Count ist der Index, den der Stein in SteinInfos haben wird.
            CopySteinIndexToSpielfeldPos3DAndSetOffsetXY(steinPos3D, SteinInfoIndex:=SteinInfos.Count)
            '() nicht trennen, der Index muss stimmen. Späterer Zugriff über
            'Dim aktSteininfo As SteinInfo = SteinInfos(indexSteinInfo)
            AddSteinInfoInternal(siDc)

            Return True

        End Function

        '
        ''' <summary>
        ''' Fügt das werkbankResult in das Spielfeld ein.Es muss vorher geprüft werden mit IsValideArea
        ''' ob das überhaupt möglich ist.
        ''' Reihenfolge 1. IsValideArea 2. mit dem SpielfeldSteineAustauscher die Steine ändern.
        ''' (Kann auch später insgesamt geschehen) 2./3. AddWerkbankResultToSpielfeld.
        ''' Es gibt aber noch ein Sicherheitsnetz: Gibt es beim Einfügen doch Probleme,
        ''' wird umgeschaltet und alle 
        ''' </summary>
        ''' <param name="werkbankResult"></param>
        ''' <param name="insertAt"></param>
        Public Sub AddWerkbankResultToSpielfeld(werkbankResult As (steinInfos As List(Of SteinInfo), arrFB As Integer(,,)), insertAt As Triple)

            If IsNothing(werkbankResult) OrElse IsNothing(werkbankResult.steinInfos) OrElse IsNothing(arrFB) Then
                If Debugger.IsAttached Then
                    Stop 'Programmierfehler
                Else
                    Exit Sub
                End If
            End If

            If werkbankResult.steinInfos.Count = 0 Then
                Exit Sub
            End If

            'Nochmalige Prüfung (oder erstmalige Prüfung), da ich während
            'der Werkstücke nicht den Dienstweg gehe.
            Dim vp As ValidePlace = IsValideArea(werkbankResult, insertAt)

            With werkbankResult
                If vp <> ValidePlace.Yes Then
                    For idx As Integer = 0 To .steinInfos.Count - 1
                        .steinInfos(idx).IsWerkbankStein = True 'wieder einschalten (ob man's noch gebrauchen kann, weis ich derzeit nicht)
                        .steinInfos(idx).SteinStatusUsed = SteinStatus.I08WerkstückEinfügeFehler
                        .steinInfos(idx).SteinStatusIst = SteinStatus.I08WerkstückEinfügeFehler
                    Next
                End If

                For wbrZ As Integer = 0 To .arrFB.GetUpperBound(2)
                    For wbrX As Integer = 1 To .arrFB.GetUpperBound(0) - 1
                        For wbrY As Integer = 1 To .arrFB.GetUpperBound(1) - 1
                            If IsIndexQuadrant(.arrFB(wbrX, wbrY, wbrZ)) Then
                                Dim idx As Integer = GetIndexStein(.arrFB(wbrX, wbrY, wbrZ))
                                Dim newPlace As New Triple(insertAt.x + wbrX - 1, insertAt.y + wbrY - 1, insertAt.z + wbrZ)
                                If IsFreePlace(newPlace) Then
                                    AddSteinToSpielfeld(.steinInfos(idx), newPlace)
                                End If
                            End If
                        Next
                    Next
                Next
            End With

        End Sub
        '
        ''' <summary>
        ''' Fügt den Werkstück in das Spielfeld ein.Es muss vorher geprüft werden mit IsValideArea
        ''' ob das überhaupt möglich ist.
        ''' Reihenfolge 1. IsValideArea 2. mit dem Werkstück (hat den SpielfeldSteineAustauscher integriert) die Steine ändern.
        ''' (Kann auch später insgesamt geschehen) 2./3. AddWerkstückToSpielfeld.
        ''' </summary>
        ''' <param name="wbb"></param>
        ''' <param name="insertAt"></param>
        Public Sub AddWerkstückToSpielfeld(wbb As Werkstück, insertAt As Triple)

            Dim werkstück As (steinInfos As List(Of SteinInfo), arrFB As Integer(,,))
            werkstück.steinInfos = wbb.SteinInfos
            werkstück.arrFB = wbb.ArrFB
            AddWerkbankResultToSpielfeld(werkstück, insertAt)

        End Sub

        ''' <summary>
        ''' Entfernt einen Stein vollständig, d.h. einschließlich aller Daten vom Spielfeld
        ''' und aus steininfos
        ''' Hinweis: Um ihn nicht mehr anzuzeigen muß er nicht entfernt werden. Dazu wird
        ''' lediglich in der SteinInfo IsRemoved gesetzt.
        ''' </summary>
        ''' <param name="arrFB"></param>
        ''' <param name="steininfos"></param>
        ''' <param name="steininfo"></param>
        Public Sub RemoveSteinFromSpielfeld(arrFB(,,) As Integer, steininfos As SFInfo, steininfo As SteinInfo)

            ' später

        End Sub

        Public Sub RemoveLastSteinFromSpielfeld(arrFB(,,) As Integer, steininfos As SFInfo)

            ' später

        End Sub

#End Region

#Region "Spielfeld - interne Add/Remove-Helfer"

        Private Function HandleInvalidInsertPosition(steinIndex As SteinIndexEnum, steinPos3D As Triple) As Boolean

            ' optional später aus AddSteinToSpielfeld herausziehen
            Return False

        End Function

        Private Sub WriteSteinToArrFB(steinPos3D As Triple, steinInfoIndex As Integer)

            CopySteinIndexToSpielfeldPos3DAndSetOffsetXY(steinPos3D, steinInfoIndex)

        End Sub

        Private Sub DeleteSteinFromArrFB(steinPos3D As Triple)

            ' später

        End Sub

#End Region

#Region "Platzprüfung / Feldlogik"

        ''' <summary>
        ''' Wie ValidePlace, nur wird ein von der Werkstatt 
        ''' erstellter Baustein Stein für Stein geprüft.
        ''' </summary>
        ''' <returns></returns>
        Public Function IsValideArea(werkbankResult As (wbSteinInfos As List(Of SteinInfo), wbArrFB As Integer(,,)), insertAt As Triple) As ValidePlace
            'Anmerkung: Ich hatte in der Deklaration von werkbankResult steinInfos und arrFB stehen.
            '           Mußte ich dann ändern in wbSteinInfos und wbArrFB, weil die Intellisenze
            '           die Werte von SteinInfos und arrFB aus dem Modul SpielfeldDaten anzeigt,
            '           was irritierend ist.

            With werkbankResult

                For wbrZ As Integer = 0 To .wbArrFB.GetUpperBound(2)
                    For wbrX As Integer = 1 To .wbArrFB.GetUpperBound(0) - 1
                        For wbrY As Integer = 1 To .wbArrFB.GetUpperBound(1) - 1
                            If IsIndexQuadrant(.wbArrFB(wbrX, wbrY, wbrZ)) Then

                                Dim idx As Integer = GetIndexStein(.wbArrFB(wbrX, wbrY, wbrZ))
                                Dim newPlace As New Triple(insertAt.x + wbrX - 1, insertAt.y + wbrY - 1, insertAt.z + wbrZ)
                                Dim tpl As Triple = IsValidePlace(newPlace)

                                'If tpl.Valide <> ValidePlace.Yes Then
                                '    Stop
                                'End If

                                Select Case tpl.Valide
                                    Case ValidePlace.NoFundamentFound
                                        'Hier nur die unterste Ebene prüfen, die Ebenen drüber haben
                                        'kein Fundament, das entsteht ja erst.
                                        '(Es wird entstehen, denn die Werkbank hat dafür gesorgt.)
                                        If wbrZ = 0 Then
                                            'Wenn die unterste Ebene des Werkbankresult nicht auf der untersten Ebene 
                                            'eingefügt wird, dann hat der Stein kein Fundament.
                                            '(Bei wbrZ = 0 And insertAt.z = 0 gibt es kein NoFundamentFound)
                                            Return tpl.Valide
                                        Else
                                            'bei allen anderen Steinen in den anderen Ebenen fehlt das Fundament,
                                            'das wird ja vom Werkbankresult erst hinzugefügt.
                                            'Nichts machen, nächsten Stein prüfen oder Ende der Prüfung
                                        End If
                                    Case ValidePlace.Occupied
                                        Return tpl.Valide

                                    Case ValidePlace.OutsideBorder
                                        Return tpl.Valide

                                    Case ValidePlace.Yes
                               'Nichts machen, nächsten Stein prüfen oder Ende der Prüfung

                                    Case ValidePlace.NotSet
                                        If Debugger.IsAttached Then
                                            Stop ' Programmierfehler
                                        End If
                                End Select

                            End If
                        Next
                    Next
                Next

            End With

            Return ValidePlace.Yes

        End Function

        ''' <summary>
        ''' Eine Kombination aus IsInsideSpielfeldBounds, IsFreePlace und HasFundament.
        ''' Valide = ValidePlace.Yes, heist, der Stein kann dort abgelegt werden.
        ''' </summary>
        ''' <param name="triple"></param>
        ''' <returns></returns>
        Public Function IsValidePlace(triple As Triple) As Triple

            If Not triple.IsInsideSpielfeldBounds(arrFB) Then
                Return New Triple(triple, ValidePlace.OutsideBorder)
            Else
                If IsFreePlace(triple) Then
                    If HasFundament(triple) Then
                        Return New Triple(triple, ValidePlace.Yes)
                    Else
                        Return New Triple(triple, ValidePlace.NoFundamentFound)
                    End If
                Else
                    Return New Triple(triple, ValidePlace.Occupied)
                End If
            End If

        End Function

        ''' <summary>
        ''' Prüft, ob der Platz frei ist
        ''' </summary>
        ''' <param name="infoFBTriple"></param>
        ''' <returns></returns>
        Public Function IsFreePlace(infoFBTriple As Triple) As Boolean

            Dim infoFBx As Integer = infoFBTriple.x
            Dim infoFBy As Integer = infoFBTriple.y
            Dim infoFBz As Integer = infoFBTriple.z

            If infoFBx < xMin Then Return False
            If infoFBy < yMin Then Return False

            'Bereits bei Gleichheit wird False zurück gemeldet, weil nach der
            'Position des InfoFB gefragt wird. Seine Childs sind auf dieser Position
            'bereits außerhalb.
            If infoFBx >= xMax Then Return False
            If infoFBy >= yMax Then Return False

            If infoFBz > zMax Then Return False
            '
            'die obere Zeile
            'Den FB rechts unter dem infoFB
            If arrFB(infoFBx + 1, infoFBy + 1, infoFBz) <> 0 Then Return False
            '
            'Den FB genau unter dem infoFB
            If arrFB(infoFBx, infoFBy + 1, infoFBz) <> 0 Then Return False

            'Den FB rechts vom infoFB 
            If arrFB(infoFBx + 1, infoFBy, infoFBz) <> 0 Then Return False

            'Der infoFB 
            If arrFB(infoFBx, infoFBy, infoFBz) <> 0 Then Return False
            '
            Return True

        End Function
        '

        ''' <summary>
        ''' Prüft, ob der Stein auf der Grundfläche, oder vollständig 
        ''' auf anderen Steinen stehen würde.
        ''' </summary>
        ''' <param name="infoFBTriple">Kann Nothing sein</param>
        ''' <returns></returns>
        Public Function HasFundament(infoFBTriple As Triple) As Boolean

            If IsNothing(infoFBTriple) Then
                Return False
            End If

            'Im Unterschied zu IsFreePlace wird hier zusätzlich in der Ebene
            'unter dem Stein gesucht.
            Dim infoFBx As Integer = infoFBTriple.x
            Dim infoFBy As Integer = infoFBTriple.y
            Dim infoFBz As Integer = infoFBTriple.z

            If infoFBx < xMin Then Return False
            If infoFBy < yMin Then Return False

            'Bereits bei Gleichheit wird False zurück gemeldet, weil nach der
            'Position des InfoFB gefragt wird. Seine Childs sind auf dieser Position
            'bereits außerhalb.
            If infoFBx >= xMax Then Return False
            If infoFBy >= yMax Then Return False

            If infoFBz > zMax Then Return False

            If infoFBz = 0 Then
                'Das ist die Grundfläche des Spieles.
                'Hier müssen die 4 Felder frei sein

                'Den FB rechts unten vom infoFB
                If arrFB(infoFBx + 1, infoFBy + 1, infoFBz) <> 0 Then Return False
                '
                'Den FB genau unter dem infoFB
                If arrFB(infoFBx, infoFBy + 1, infoFBz) <> 0 Then Return False
                '
                'Den FB rechts vom infoFB 
                If arrFB(infoFBx + 1, infoFBy, infoFBz) <> 0 Then Return False

                'Der infoFB selber 
                If arrFB(infoFBx, infoFBy, infoFBz) <> 0 Then Return False

            Else
                'Auf der aktuellen Ebene müssen 4 Felder frei sein
                '
                'Den FB rechts unten vom infoFB
                If arrFB(infoFBx + 1, infoFBy + 1, infoFBz) <> 0 Then Return False
                '
                'Den FB genau unter dem infoFB
                If arrFB(infoFBx, infoFBy + 1, infoFBz) <> 0 Then Return False
                '
                'Den FB rechts vom infoFB 
                If arrFB(infoFBx + 1, infoFBy, infoFBz) <> 0 Then Return False

                'Der infoFB selber 
                If arrFB(infoFBx, infoFBy, infoFBz) <> 0 Then Return False

                'Alle Plätze des infoFB in der Ebene drunter müssen belegt sein.
                '(infoFBx, infoFBY und infoFBz sind die Koordinaten des abgefragten infoFB)
                '
                'genau eine Ebene tiefer gehen.
                infoFBz -= 1
                '
                'Den FB rechts unter dem infoFB
                If arrFB(infoFBx + 1, infoFBy + 1, infoFBz) = 0 Then Return False
                '
                'Den FB genau unter dem infoFB
                If arrFB(infoFBx, infoFBy + 1, infoFBz) = 0 Then Return False
                '
                'Den FB rechts vom infoFB 
                If arrFB(infoFBx + 1, infoFBy, infoFBz) = 0 Then Return False

                'Der infoFB selber 
                If arrFB(infoFBx, infoFBy, infoFBz) = 0 Then Return False

            End If

            Return True

        End Function

        ''' <summary>
        ''' Incrementiert die X- und Y-Koordinaten des Triple in die vorgegebene (Himmels-) Richtung.
        ''' </summary>
        Public Function IncDirection(triple As Triple, direction As Direction, Optional [Step] As Integer = 1) As Triple

            'DeepCopy erstellen um sicherzustellen, dass der Ausgangswert unverändert bleibt.
            Dim newTriple As Triple = triple.DeepCopy

            With newTriple
                Select Case direction
                    Case Direction.Up
                        .y -= [Step]

                    Case Direction.UpRight
                        .y -= [Step]
                        .x += [Step]

                    Case Direction.Right
                        .x += [Step]

                    Case Direction.DownRight
                        .y += [Step]
                        .x += [Step]

                    Case Direction.Down
                        .y += [Step]

                    Case Direction.DownLeft
                        .y += [Step]
                        .x -= [Step]

                    Case Direction.Left
                        .x -= [Step]

                    Case Direction.UpLeft
                        .y -= [Step]
                        .x -= [Step]

                End Select
            End With

            Return newTriple

        End Function

        '
        ''' <summary>
        ''' Sucht von der Startposition (Koordinaten in infoFBTriple)
        ''' in die angegebene  Richtung nach dem nächsten freiem Platz.
        ''' Wenn z = 0 (also auf der Grundfläche) wird jeder freie Platz zurückgegeben.
        ''' Wenn z > 0 nur Positionen, wo der Stein vollständig auf anderen Steinen
        ''' steht. 
        ''' Ob was gefunden wurde und wenn nicht, warum, steht in Triple.fr As SearchResult
        ''' </summary>
        ''' <param name="infoFBTriple"></param>
        ''' <param name="direction"></param>
        ''' <returns></returns>
        Public Function SearchPlace(infoFBTriple As Triple, direction As Direction) As Triple

            'DeepCopy um Seiteneffekte zu verhindern
            Dim tpl As Triple = infoFBTriple.DeepCopy

            'nicht mögliche Startpositionen anpassen
            If tpl.x < 1 Then tpl.x = 1
            If tpl.y < 1 Then tpl.y = 1

            If tpl.z = 0 Then
                Do
                    If Not tpl.IsInsideSpielfeldBounds(arrFB) Then
                        Return New Triple(tpl, ValidePlace.OutsideBorder)
                    Else
                        If IsFreePlace(tpl) Then
                            Return New Triple(tpl, ValidePlace.Yes)
                        End If
                    End If
                    tpl = IncDirection(tpl, direction)
                Loop
            Else
                Dim fFoundFreePlace As Boolean = False
                Do

                    If Not tpl.IsInsideSpielfeldBounds(arrFB) Then
                        If fFoundFreePlace Then
                            Return New Triple(tpl, ValidePlace.NoFundamentFound)
                        Else
                            Return New Triple(tpl, ValidePlace.OutsideBorder)
                        End If
                    Else
                        Dim fFound As Boolean = IsFreePlace(tpl)

                        If fFound And Not fFoundFreePlace Then
                            fFoundFreePlace = True
                        End If

                        If fFound AndAlso HasFundament(tpl) Then
                            Return New Triple(tpl, ValidePlace.Yes)
                        End If

                    End If

                    tpl = IncDirection(tpl, direction)
                Loop
            End If

        End Function
        '
        ''' <summary>
        ''' Zunächst wie die erste Überladung von SearchPlace, als die Suche nach einem freienm Platz.
        ''' Wenn einer gefunden wurde, wird der Platz verschoben nach Angabe von moveX und moveX
        ''' unter Berücksichtigung von stepHalberStein
        ''' </summary>
        ''' <param name="infoFBTriple"></param>
        ''' <param name="direction"></param>
        ''' <param name="moveX"></param>
        ''' <param name="moveY"></param>
        ''' <param name="stepHalberStein"></param>
        ''' <returns></returns>
        Public Function SearchPlace(infoFBTriple As Triple, direction As Direction, moveX As Move, moveY As Move, stepHalberStein As Boolean) As Triple

            Dim tpl As Triple = SearchPlace(infoFBTriple, direction)
            Dim add As Integer = If(stepHalberStein, 1, 2)

            Dim addX As Integer

            Do
                Select Case moveX
                    Case Move.LeftOrUp
                        addX = -add
                    Case Move.RightOrDown
                        addX = add
                    Case Else
                        addX = 0
                End Select

                Dim addY As Integer

                Select Case moveY
                    Case Move.LeftOrUp
                        addY = -add
                    Case Move.RightOrDown
                        addY = add
                    Case Else
                        addY = 0
                End Select

                Dim tpl2 As New Triple(infoFBTriple.x + addX, infoFBTriple.y + addY, infoFBTriple.z)

                Dim tpl3 As Triple = IsValidePlace(tpl2)

                Select Case tpl3.Valide
                    Case ValidePlace.Yes
                        Return tpl3
                    Case ValidePlace.NoFundamentFound
                    'nächster Versuch
                    Case ValidePlace.OutsideBorder
                        Return tpl3
                End Select
            Loop

        End Function

#End Region

#Region "Positionshilfen"

        ''' <summary>
        ''' Gibt die die Koordinaten der Spielfeldmitte in einem Triple zurück.
        ''' Brauchbar um den den ersten Stein zu verlegen
        ''' Wird Ebene zu hoch angegeben, wird die oberste Ebene eingestellt.
        ''' Ist der Platz belegt, ist Found = FoundResult.NoFundamentFound 
        ''' </summary>
        ''' <param name="ebene"></param>
        ''' <returns></returns>
        Public Function GetSpielfeldCenter(ebene As Integer) As Triple

            If ebene > zMax Then
                ebene = zMax
            End If

            Dim tpl As New Triple(xMax \ 2, yMax \ 2, ebene)

            If IsFreePlace(tpl) Then
                If HasFundament(tpl) Then
                    tpl.Valide = ValidePlace.Yes
                Else
                    tpl.Valide = ValidePlace.NoFundamentFound
                End If
            Else
                tpl.Valide = ValidePlace.NoFundamentFound
            End If

            Return tpl

        End Function
        '
        '
        ''' <summary>
        ''' Gibt absolute Positionen im Spielfeld ohne Belegt-Prüfung zurück.
        ''' </summary>
        ''' <param name="position"></param>
        ''' <param name="ebene"></param>
        ''' <returns></returns>
        Public Function GetPositionImSpielfeld(position As PositionEnum, ebene As Integer) As Triple
            Return GetPositionImSpielfeld(position, New Triple(xMin, yMin, ebene), xMax \ 2, yMax \ 2)
        End Function
        '
        ''' <summary>
        ''' Gibt relative Positionen im Spielfeld ohne Belegt-Prüfung zurück.
        ''' Das Feld im Feld ist definiert durch startPosition, steineCountX und steineCountY 
        ''' </summary>
        ''' <param name="position"></param>
        ''' <param name="startPosition"></param>
        ''' <param name="steineCountX"></param>
        ''' <param name="steineCountY"></param>
        ''' <returns></returns>
        Public Function GetPositionImSpielfeld(position As PositionEnum, startPosition As Triple, steineCountX As Integer, steineCountY As Integer) As Triple

            Dim ebene As Integer = Math.Min(startPosition.z, zMax)

            Dim leftL As Integer = startPosition.x
            Dim leftM As Integer = steineCountX + leftL - 1
            Dim leftR As Integer = steineCountX * 2 + leftL - 2

            Dim topO As Integer = startPosition.y
            Dim topM As Integer = steineCountY + topO - 1
            Dim topU As Integer = steineCountY * 2 + topO - 2

            Dim deltaLeft As Integer = (leftM - leftL) \ 2
            Dim deltaTop As Integer = (topM - topO) \ 2

            Dim x As Integer
            Dim y As Integer

            Select Case position
                Case PositionEnum.EckeLO
                    x = leftL : y = topO

                Case PositionEnum.EckeRO
                    x = leftR : y = topO

                Case PositionEnum.EckeLU
                    x = leftL : y = topU

                Case PositionEnum.EckeRU
                    x = leftR : y = topU

            '-------------------
                Case PositionEnum.MitteL
                    x = leftL : y = topM

                Case PositionEnum.MitteR
                    x = leftR : y = topM

                Case PositionEnum.MitteO
                    x = leftM : y = topO

                Case PositionEnum.MitteU
                    x = leftM : y = topU

            '------------------- 
                Case PositionEnum.Center
                    x = leftM : y = topM

            '-------------------
                Case PositionEnum.CenterLO
                    x = leftL + deltaLeft : y = topO + deltaTop

                Case PositionEnum.CenterRO
                    x = leftR - deltaLeft : y = topO + deltaTop

                Case PositionEnum.CenterLU
                    x = leftL + deltaLeft : y = topU - deltaTop

                Case PositionEnum.CenterRU
                    x = leftR - deltaLeft : y = topU - deltaTop

            End Select

            Return IsValidePlace(New Triple(x, y, ebene))

        End Function

#End Region

#Region "FB - Lesen"

        ' Kopiervorlagen der Funktionen und Methoden dieser Region als InlineCode
        '
        ' Lesen:
        ' Dim offsetX As Integer = If((fb And FLAG_XOffset) <> 0, 1, 0)
        ' Dim offsetY As Integer = If((fb And FLAG_YOffset) <> 0, 1, 0)
        ' Dim toggleFlag As Boolean = (fb And FLAG_ToggleFlag) <> 0
        ' Dim index As Integer = fb \ 1000
        '
        ' Schreiben:
        ' If value0or1 <> 0 Then fb = fb Or FLAG_XOffset Else fb = fb And Not FLAG_XOffset
        ' If value0or1 <> 0 Then fb = fb Or FLAG_YOffset Else fb = fb And Not FLAG_YOffset
        '
        ' If toggleValue Then
        '    fb = fb Or FLAG_ToggleFlag
        ' Else
        '    fb = fb And Not FLAG_ToggleFlag
        ' End If
        '
        ' Dim flags As Integer = fb And &HFF
        ' fb = newValue * 1000 + flags
        '
        ' fb = fb Xor FLAG_ToggleFlag ' Toggle
        '
        '
        ''' <summary>
        ''' Diese Überladung holt den Steinindex direkt aus arrFB.
        ''' 
        ''' Ein Stein belegt vier Felder in arrFB. Alle vier Felder müssen ungleich 0 sein,
        ''' damit sie als belegt erkannt werden.
        ''' 
        ''' Der eigentliche Steinindex steht nur im linken oberen Feld des 2x2-Blocks,
        ''' also in der Referenzsäule des Steins. Dort ist er um 1 erhöht und mit 1000
        ''' multipliziert gespeichert, damit der Wert immer ungleich 0 ist und die drei
        ''' niederwertigsten Stellen als Flagfeld frei bleiben.
        ''' 
        ''' In den drei anderen Feldern steht nicht der Index selbst, sondern nur eine
        ''' Offset-Information zur Referenzsäule. Über FLAG_XOffset und FLAG_YOffset
        ''' wird bestimmt, ob der Feldbeschreiber eine Spalte nach links und/oder eine
        ''' Zeile nach oben versetzt gelesen werden muss.
        ''' 
        ''' Dadurch kann von jedem der vier belegten Felder aus wieder auf das linke obere
        ''' Referenzfeld des Steins zugegriffen und dort der Steinindex gelesen werden.
        ''' 
        ''' Wird die Funktion auf ein leeres Feld angewendet, ist arrFB = 0. Dann sind
        ''' beide Offsets 0, es wird also erneut auf dasselbe Feld zugegriffen. Da dort
        ''' ebenfalls 0 steht, ergibt (0 \ 1000) - 1 den Wert -1.
        ''' 
        ''' Vorsicht: IsIndexQuadrant arbeitet anders, weil dort kein Offset berücksichtigt
        ''' wird. Zum Iterieren durch das Feld deshalb IsIndexQuadrant verwenden.
        ''' </summary>
        ''' <param name="x"></param>
        ''' <param name="y"></param>
        ''' <param name="z"></param>
        ''' <returns></returns> '
        Public Function GetIndexStein(x As Integer, y As Integer, z As Integer) As Integer
            Dim fb As Integer = arrFB(x, y, z)
            Dim offsetX As Integer = If((fb And FLAG_XOffset) <> 0, 1, 0)
            Dim offsetY As Integer = If((fb And FLAG_YOffset) <> 0, 1, 0)
            Return (arrFB(x - offsetX, y - offsetY, z) \ 1000) - 1
            'Auf ein unbelegtes Feld angewendet passiert folgendes:
            'offsetX und Offset Y sind 0, d.h. x und y werden nicht verändert. 
            'Der Index  (arrFB(x - offsetX, y - offsetY, z) \ 1000) ist auch 0
            '1 ab, gibt minus 1. ==> es muss nicht extra auf arrFB(x, y, z) = 0 geprüft werden,
            'ob ein Feld leer ist, es kann immer gleich nach dem SteinIndex gefragt werden.
        End Function
        '
        ''' <summary>
        ''' Diese Überladung holt des Index direkt aus dem arrFB unter Berücksichtigung der OffsetXY
        ''' Siehe die erste Überladung.
        ''' VORSICHT, zum Iterieren IsIndexQuadrant verwenden.
        ''' </summary>
        ''' <param name="tripl"></param>
        ''' <returns></returns>'
        Public Function GetIndexStein(tripl As Triple) As Integer
            'Weil zeitkritisch doppelter Code
            With tripl
                Dim fb As Integer = arrFB(.x, .y, .z)
                Dim offsetX As Integer = If((fb And FLAG_XOffset) <> 0, 1, 0)
                Dim offsetY As Integer = If((fb And FLAG_YOffset) <> 0, 1, 0)
                Return (arrFB(.x - offsetX, .y - offsetY, .z) \ 1000) - 1
            End With
        End Function

        ''' <summary>
        ''' Den Index und zur Prüfung, ob einer der Quadranten zu einem Stein gehört.
        ''' VORSICHT, zum Iterieren auf der Suche nach IndexStein IsIndexQuadrant verwenden.
        ''' 
        ''' </summary>
        ''' <param name="fb"></param>
        ''' <returns></returns>
        Public Shared Function GetIndexStein(fb As Integer) As Integer
            'Der Index wird um 1 erhöht gespeichert, weil index = 0 gleichbedeutend
            'wäre mit "freier Platz"
            Return (fb \ 1000) - 1  ' Wert ab dritter Dezimalstelle (ab Hunderterstelle)
        End Function

        ''' <summary>
        ''' Prüft, ob an der Stelle ein Felbbeschreiber FB ist, der einen SteinIndex enthält.
        ''' Kann zum Iterieren durch das Feld benutzt werden.
        ''' </summary>
        ''' <param name="fb"></param>
        ''' <returns></returns>
        Public Shared Function IsIndexQuadrant(fb As Integer) As Boolean
            'Der Index wird um 1 erhöht gespeichert, weil index = 0 gleichbedeutend
            'wäre mit "freier Platz" hier wird die 1 nicht abgezogen!
            'so ist sichergestellt, daß nur True ist bei den linken oberen Quadranten
            'und die anderen drei Quadranten nicht berücksichtigt werden.
            'Quadranten 
            Return (fb \ 1000) > 0  ' Wert ab dritter Dezimalstelle
        End Function

        ''' <summary>
        ''' Prüft, ob an der Stelle ein Felbeschreiber FB ist, der einen SteinIndex enthält.
        ''' Kann zum Iterieren durch das Feld benutzt werden.
        ''' </summary>
        Public Function IsIndexQuadrant(tripl As Triple) As Boolean
            'Der Index wird um 1 erhöht gespeichert, weil index = 0 gleichbedeutend
            'wäre mit "freier Platz" hier wird die 1 nicht abgezogen!
            'so ist sichergestellt, daß nur True ist bei den linken oberen Quadranten
            'und die anderen drei Quadranten nicht berücksichtigt werden.
            'Quadranten 
            With tripl
                Dim fb As Integer = arrFB(.x, .y, .z)
                Return (fb \ 1000) > 0  ' Wert ab dritter Dezimalstelle
            End With

        End Function

        Public Function GetQuadrant(x As Integer, y As Integer, z As Integer) As Quadrant

            ''Dim fb As Integer = arrFB(x, y, z)
            ''If fb = 0 Then
            ''    Return Quadrant.none
            ''End If
            ''Dim offsetX As Integer = If((fb And FLAG_XOffset) <> 0, 1, 0)
            ''Dim offsetY As Integer = If((fb And FLAG_YOffset) <> 0, 1, 0)

            ''If offsetX = 0 AndAlso offsetY = 0 Then
            ''    Return Quadrant.LO
            ''ElseIf offsetX = 1 AndAlso offsetY = 0 Then
            ''    Return Quadrant.RO
            ''ElseIf offsetX = 0 AndAlso offsetY = 1 Then
            ''    Return Quadrant.LU
            ''Else
            ''    Return Quadrant.RU
            ''End If
            '
            'obiger Code optimiert:

            Dim fb As Integer = arrFB(x, y, z)

            If fb = 0 Then
                Return Quadrant.none
            End If

            Dim idx As Integer = 0
            If (fb And FLAG_XOffset) <> 0 Then idx += 1
            If (fb And FLAG_YOffset) <> 0 Then idx += 2

            Return CType(1 << idx, Quadrant)

        End Function

        Public Shared Function GetOffsetX(fb As Integer) As Integer
            Return If((fb And FLAG_XOffset) <> 0, 1, 0)
        End Function

        Public Shared Function GetOffsetY(fb As Integer) As Integer
            Return If((fb And FLAG_YOffset) <> 0, 1, 0)
        End Function
        '
        ''' <summary>
        ''' Holt den Wert des ToggleFlags koorigiert um den OffsetXY direkt aus dem arrFB 
        ''' </summary>
        ''' <param name="x"></param>
        ''' <param name="y"></param>
        ''' <param name="z"></param>
        ''' <returns></returns> '
        Public Function GetToggleFlag(x As Integer, y As Integer, z As Integer) As Boolean
            'Weil zeitkritisch doppelter Code
            Dim fb As Integer = arrFB(x, y, z)
            Dim offsetX As Integer = If((fb And FLAG_XOffset) <> 0, 1, 0)
            Dim offsetY As Integer = If((fb And FLAG_YOffset) <> 0, 1, 0)
            Return (arrFB(x - offsetX, y - offsetY, z) And FLAG_ToggleFlag) <> 0
        End Function
        '
        ''' <summary>
        ''' Holt den Wert des ToggleFlags koorigiert um den OffsetXY direkt aus dem arrFB
        ''' </summary>
        ''' <param name="tripl"></param>
        ''' <returns></returns> '
        Public Function GetToggleFlag(tripl As Triple) As Boolean
            'Weil zeitkritisch doppelter Code
            With tripl
                Dim fb As Integer = arrFB(.x, .y, .z)
                Dim offsetX As Integer = If((fb And FLAG_XOffset) <> 0, 1, 0)
                Dim offsetY As Integer = If((fb And FLAG_YOffset) <> 0, 1, 0)
                Return (arrFB(.x - offsetX, .y - offsetY, .z) And FLAG_ToggleFlag) <> 0
            End With
        End Function
        '
        ''' <summary>
        ''' Seperate Gewinnung des Wertes.
        ''' </summary>
        ''' <param name="fb"></param>
        ''' <returns></returns>
        Public Function GetToggleFlag(fb As Integer) As Boolean
            Return (fb And FLAG_ToggleFlag) <> 0
        End Function
        '
        ''' <summary>
        ''' Zum Syncronisieren von ToggleFlag und VergleichsToggleFlag muss das
        ''' VergleichsToggleFlag auf den Zustand im Feld gesetz werden. (sicherheitshalber)
        ''' Dazu wird das erste Toggleflag im Feld gesucht und ausgelesen.
        ''' </summary>
        ''' <returns></returns> '
        Public Function GetFirstToggleFlagValue() As Boolean

            For z As Integer = 0 To zMax 'auf der untersten Ebene beginnen. (da wird immer was gefunden)
                For x As Integer = 0 To xMax
                    For y As Integer = 0 To yMax
                        Dim fb As Integer = arrFB(x, y, z)
                        If fb <> 0 Then
                            'das erste gefundene belegte Feld auswerten
                            Dim offsetX As Integer = If((fb And FLAG_XOffset) <> 0, 1, 0)
                            Dim offsetY As Integer = If((fb And FLAG_YOffset) <> 0, 1, 0)
                            Return (arrFB(x - offsetX, y - offsetY, z) And FLAG_ToggleFlag) <> 0
                        End If
                    Next
                Next
            Next
            Return False 'leeres Feld
        End Function

#End Region

#Region "FB - Schreiben"

        ''' <summary>
        ''' Setzt die X-Y-OffsetWerte der childFB des Feldbeschreibers und den Index des Feldbeschreibers selber. 
        ''' </summary>
        ''' <param name="infoFBTriple"></param>
        Public Sub CopySteinIndexToSpielfeldPos3DAndSetOffsetXY(infoFBTriple As Triple, SteinInfoIndex As Integer)
            '
            Dim infoFBx As Integer = infoFBTriple.x
            Dim infoFBy As Integer = infoFBTriple.y
            Dim infoFBz As Integer = infoFBTriple.z

            'Vom childFB rechts unter dem infoFB geht es zum infoFB, indem von beiden Koordinaten
            '1 subtrahiert wird. Daher:
            SetOffsetX(arrFB(infoFBx + 1, infoFBy + 1, infoFBz), True)
            SetOffsetY(arrFB(infoFBx + 1, infoFBy + 1, infoFBz), True)
            '
            'Der childFB genau unter dem infoFB zum infoFB geht mit OffsetX = 0 und OffsetY = -1
            SetOffsetX(arrFB(infoFBx, infoFBy + 1, infoFBz), False)
            SetOffsetY(arrFB(infoFBx, infoFBy + 1, infoFBz), True)
            '
            'Vom FB rechts vom infoFB zum infoFB: OffsetX = -1, OffsetY = 0
            SetOffsetX(arrFB(infoFBx + 1, infoFBy, infoFBz), True)
            SetOffsetY(arrFB(infoFBx + 1, infoFBy, infoFBz), False)

            'Der infoFB hat keinen Offset, daher beide 0 (Verweis auf sich selber)
            SetOffsetX(arrFB(infoFBx, infoFBy, infoFBz), False)
            SetOffsetY(arrFB(infoFBx, infoFBy, infoFBz), False)
            '
            SetIndexStein(arrFB(infoFBx, infoFBy, infoFBz), SteinInfoIndex)

        End Sub

        'Hinweis: Die Setter-Prozeduren sind ByRef, damit fb geändert wird.
        Public Sub SetOffsetX(ByRef fb As Integer, value0or1 As Integer)
            If value0or1 <> 0 Then
                fb = fb Or FLAG_XOffset
            Else
                fb = fb And Not FLAG_XOffset
            End If
        End Sub

        Public Sub SetOffsetX(ByRef fb As Integer, value As Boolean)
            If value Then
                fb = fb Or FLAG_XOffset
            Else
                fb = fb And Not FLAG_XOffset
            End If
        End Sub

        Public Sub SetOffsetY(ByRef fb As Integer, value0or1 As Integer)
            If value0or1 <> 0 Then
                fb = fb Or FLAG_YOffset
            Else
                fb = fb And Not FLAG_YOffset
            End If
        End Sub

        Public Sub SetOffsetY(ByRef fb As Integer, value As Boolean)
            If value Then
                fb = fb Or FLAG_YOffset
            Else
                fb = fb And Not FLAG_YOffset
            End If
        End Sub
        '
        Public Sub SetToggleFlag(x As Integer, y As Integer, z As Integer, value As Boolean)
            'Weil zeitkritisch doppelter Code
            Dim fb As Integer = arrFB(x, y, z)
            Dim offsetX As Integer = If((fb And FLAG_XOffset) <> 0, 1, 0)
            Dim offsetY As Integer = If((fb And FLAG_YOffset) <> 0, 1, 0)
            If value Then
                arrFB(x - offsetX, y - offsetY, z) = arrFB(x - offsetX, y - offsetY, z) Or FLAG_ToggleFlag
            Else
                arrFB(x - offsetX, y - offsetY, z) = arrFB(x - offsetX, y - offsetY, z) And Not FLAG_ToggleFlag
            End If
        End Sub
        '
        Public Sub SetToggleFlag(tripl As Triple, value As Boolean)
            'Weil zeitkritisch doppelter Code
            With tripl
                Dim fb As Integer = arrFB(.x, .y, .z)
                Dim offsetX As Integer = If((fb And FLAG_XOffset) <> 0, 1, 0)
                Dim offsetY As Integer = If((fb And FLAG_YOffset) <> 0, 1, 0)
                If value Then
                    arrFB(.x - offsetX, .y - offsetY, .z) = arrFB(.x - offsetX, .y - offsetY, .z) Or FLAG_ToggleFlag
                Else
                    arrFB(.x - offsetX, .y - offsetY, .z) = arrFB(.x - offsetX, .y - offsetY, .z) And Not FLAG_ToggleFlag
                End If
            End With
        End Sub

        Public Shared Sub SetToggleFlag(ByRef fb As Integer, value As Boolean)
            If value Then
                fb = fb Or FLAG_ToggleFlag
            Else
                fb = fb And Not FLAG_ToggleFlag
            End If
        End Sub

        Public Sub ClearAllToggleFlags()
            For x As Integer = xMin To xMax
                For y As Integer = yMin To yMax
                    For z As Integer = zMin To zMax
                        If arrFB(x, y, z) <> 0 Then
                            SetToggleFlag(x, y, z, False)
                        End If
                    Next
                Next
            Next
        End Sub
        ''' <summary>
        ''' Wenn ein Stein zum Spielfeld hinzugefügt wird muss das Toggleflag syncronisiert
        ''' werden, sonst wird der Stein nicht angezeigt.
        ''' Es wird einfach der Zustand des ersten Toggleflags kopiert.
        ''' </summary>
        ''' <param name="steinPos3D"></param>
        Public Sub SyncToggleFlag(steinPos3D As Triple)

            If SteinInfos.Count <= 1 Then
                Exit Sub
            End If

            SetToggleFlag(steinPos3D, GetToggleFlag(SteinInfos(0).Pos3D))

        End Sub
        '
        ''' <summary>
        ''' Syncronisiert das letzte mit dem erstem Toggleflag, also der
        ''' zuletzt hinzugefügte Stein mit dem erstem Stein.
        ''' </summary>
        Public Sub SyncToggleFlag()

            If SteinInfos.Count <= 1 Then
                Exit Sub
            End If

            SetToggleFlag(SteinInfos(SteinInfos.Count - 1).Pos3D, GetToggleFlag(SteinInfos(0).Pos3D))

        End Sub

        Public Shared Sub SetIndexStein(ByRef fb As Integer, value As Integer)
            ' Wert ab vierten Dezimalstelle setzen (ab Tausenderstelle)
            ' Flags in den unteren Bits behalten
            Dim flags As Integer = fb And &HFF  ' Bits 0-15, Dezimal 0 bi 255
            'Der Index wird um 1 erhöht gespeichert, weil index = 0 gleichbedeutend
            'wäre mit "freier Platz" (zumindest, wenn flags = 0, was möglich ist.)
            fb = (value + 1) * 1000 + flags
        End Sub
        '
        Public Sub ToggleToggleFlag(x As Integer, y As Integer, z As Integer)
            'Weil zeitkritisch doppelter Code
            Dim fb As Integer = arrFB(x, y, z)
            Dim offsetX As Integer = If((fb And FLAG_XOffset) <> 0, 1, 0)
            Dim offsetY As Integer = If((fb And FLAG_YOffset) <> 0, 1, 0)
            arrFB(x - offsetX, y - offsetY, z) = arrFB(x - offsetX, y - offsetY, z) Xor FLAG_ToggleFlag
        End Sub
        '
        Public Sub ToggleToggleFlag(tripl As Triple)
            'Weil zeitkritisch doppelter Code
            With tripl
                Dim fb As Integer = arrFB(.x, .y, .z)
                Dim offsetX As Integer = If((fb And FLAG_XOffset) <> 0, 1, 0)
                Dim offsetY As Integer = If((fb And FLAG_YOffset) <> 0, 1, 0)
                arrFB(.x - offsetX, .y - offsetY, .z) = arrFB(.x - offsetX, .y - offsetY, .z) Xor FLAG_ToggleFlag
            End With
        End Sub

        Public Shared Sub ToggleToggleFlag(ByRef fb As Integer)
            fb = fb Xor FLAG_ToggleFlag
        End Sub

#End Region

#Region "TopSearch-Index"

        Private _indexTopSearch As Integer()
        Private _indexTopSearchDirty As Boolean = True

        Private Sub UpdateTopSearchIndex()

            If Not _indexTopSearchDirty Then
                Return
            End If

            Dim count As Integer = _SteinInfos.Count

            If count = 0 Then
                _indexTopSearch = Array.Empty(Of Integer)()
                _indexTopSearchDirty = False
                Return
            End If

            Dim arr(count - 1) As Integer

            Dim i As Integer
            For i = 0 To count - 1
                arr(i) = i
            Next

            Array.Sort(arr, AddressOf CompareSteinTopSearch)

            _indexTopSearch = arr
            _indexTopSearchDirty = False

        End Sub

        Private Function CompareSteinTopSearch(indexA As Integer, indexB As Integer) As Integer

            Dim a As SteinInfo = _SteinInfos(indexA)
            Dim b As SteinInfo = _SteinInfos(indexB)

            Dim cmp As Integer

            cmp = b.Pos3D.z.CompareTo(a.Pos3D.z)
            If cmp <> 0 Then
                Return cmp
            End If

            cmp = b.Pos3D.y.CompareTo(a.Pos3D.y)
            If cmp <> 0 Then
                Return cmp
            End If

            Return b.Pos3D.x.CompareTo(a.Pos3D.x)

        End Function

#End Region

#Region "Rebuild / Update"

        Public Sub Update_SteinInfos_RectQuadranten()
            'Nicht updaten, wenn SFInfo gesondert erzeugt und befüllt wird.
            '(mit den Werkbank/Werkstückroutinen)
            If Not IsNothing(_sfd) Then
                If _SteinInfos.Count > 0 Then
                    For Each item As SteinInfo In _SteinInfos
                        item.Update_RectQuadranten(_sfd)
                    Next
                End If
            End If
        End Sub

        Private Sub Update_WindsAreInOneClickGroup(enabled As Boolean)

            ' ... Später

        End Sub

        Private Sub RebuildAllCaches()

            Update_SteinInfos_RectQuadranten()
            MarkTopSearchDirty()

        End Sub

#End Region

#Region "HitTest / Kandidaten / Mauslogik"

        '
        ''' <summary>
        ''' Da der 3D-Effekt die Steine nach oben und nach links verschiebt,muss man in der
        ''' rechten unteren Ecke mit der höchsten Z-Ebene anfangen und dann zeilenweise das
        ''' Feld abarbeiten. Der erste Treffer gewinnt.
        ''' Da hierzu eine andere Sortierung der SteinInfos As List(Of SteinInfo) notwendig
        ''' ist, die ursprüngliche Sortierung wegen des Zugriffs über den arrFB gebraucht
        ''' wird, gibt es den _indexTopSearch.
        ''' </summary>
        ''' <param name="mousePos"></param>
        ''' <returns></returns>
        Public Function FindTopSteinAtPoint(mousePos As Point) As TripleX

            UpdateTopSearchIndex()

            Dim Kandidat As TripleX = New TripleX(ValidePlace.NoKandidat) 'Default

            For idx As Integer = 0 To _indexTopSearch.Length - 1

                Dim steinIndex As Integer = _indexTopSearch(idx)
                Dim stein As SteinInfo = _SteinInfos(steinIndex)

                If stein.IsRemoved Then
                    Continue For
                End If

                ' erster Treffer gewinnt
                ' If si.HitTest(pt) Then Return si
                Kandidat = stein.IsTopQuadrant(mousePos, arrFB, _sfd)
                If Kandidat.IsValideYes Then
                    Return Kandidat
                End If
            Next

            Return New TripleX(ValidePlace.No)

        End Function

        ''' <summary>
        ''' Prüft, ob an der Mausposition ein Stein abgelegt werden kann.
        ''' Gibt ein Triple zurück, das mit IsValideYes oder IsNotValideYes
        ''' abgefragt werden kann.
        ''' Wenn Yes, dann enthält das Tiple die Ablegekoordinaten für den Stein.
        ''' Beim Ablegen auf das Feld (nicht auf den Vorrat) ist immer der linke
        ''' obere Quadrant der "Führungsquadrant". (Der Mauszeiger steht auf dem 
        ''' abzulegende Stein immer auf diesem Quadranten) 
        ''' </summary>
        ''' <param name="mousePos"></param>
        ''' <returns></returns>
        Public Function IsFundamentKandidat(mousePos As Point) As Triple

            Dim kandidat As New Triple(ValidePlace.NoKandidat) 'Default
            For Each stein As SteinInfo In SteinInfos
                kandidat = stein.IsFundamentPartKandidat(mousePos, arrFB)
                If kandidat.IsValideYes Then
                    Exit For
                End If
            Next

            If kandidat.IsValideYes Then

                'Steht der Kandidat auf dem rechten oder unteren Rand?
                'Dann sind nach rechts bzw. nach unten keine Felder frei.
                If Not kandidat.IsInsideSpielfeldBounds(arrFB, excludeRightAndBottomMargin:=True) Then
                    Return New Triple(ValidePlace.NoKandidat)
                End If

                'Kandidat ist der linke obere Quadrant des Steines.
                'Jetzt prüfen, ob die Plätze für anderen drei Quadranten frei sind
                'und ob darunter ein Fundament ist.

                Dim isBottom As Boolean = kandidat.z = 0
                Dim okCounter As Integer = 1

                With kandidat
                    'Quadrant rechts oben frei?
                    If arrFB(.x + 1, .y, .z) = 0 Then
                        If isBottom Then 'Kandidat steht auf der Grundfläche
                            okCounter += 1
                        Else
                            'Der Platz darunter belegt?
                            If arrFB(.x + 1, .y, .z - 1) <> 0 Then
                                okCounter += 1
                            End If
                        End If
                    End If
                    '
                    'Quadrant unten drunter frei?
                    If arrFB(.x, .y + 1, .z) = 0 Then
                        If isBottom Then 'Kandidat steht auf der Grundfläche
                            okCounter += 1
                        Else
                            'Der Platz darunter belegt?
                            If arrFB(.x, .y + 1, .z - 1) <> 0 Then
                                okCounter += 1
                            End If
                        End If
                    End If
                    '
                    'Quadrant rechts unten frei?
                    If arrFB(.x + 1, .y + 1, .z) = 0 Then
                        If isBottom Then 'Kandidat steht auf der Grundfläche
                            okCounter += 1
                        Else
                            'Der Platz darunter belegt?
                            If arrFB(.x + 1, .y + 1, .z - 1) <> 0 Then
                                okCounter += 1
                            End If
                        End If
                    End If
                    '
                    '
                    If okCounter = 4 Then
                        'alle vier Plätze sind frei und darunter befindet sich ein Fundament.
                        Return kandidat
                    Else
                        Return New Triple(ValidePlace.NoKandidat)
                    End If

                End With
            Else 'Kandidat.IsNotValideYes 
                'Wenn kein Stein gefunden wurde, die Grundfläche überprüfen.
                kandidat = GetPos3DOnGround(mousePos)
                If Not kandidat.IsInsideSpielfeldBounds(arrFB, excludeRightAndBottomMargin:=True) Then
                    'Exclude...=True, weil Kandidat den linken oberen Quadrant darstellt und auf der
                    'rechten Spalte und der unteren Zeile kein Platz für die anderen Quadranten ist.
                    Return New Triple(ValidePlace.NoKandidat)
                Else
                    'Prüfen, ob die anderen drei Plätze frei sind.
                    With kandidat
                        If arrFB(.x + 1, .y, .z) = 0 AndAlso
                            arrFB(.x, .y + 1, .z) = 0 AndAlso
                            arrFB(.x + 1, .y + 1, .z) = 0 Then
                            Return kandidat
                        Else
                            Return New Triple(ValidePlace.NoKandidat)
                        End If
                    End With
                End If
            End If
        End Function

        Public Function GetSteinStackInfos(mousePos As Point) As TripleX()

            Dim ssi As New List(Of TripleX)

            Dim Kandidat As TripleX = FindTopSteinAtPoint(mousePos)

            If Kandidat.IsValideYes Then
                For idxZ As Integer = Kandidat.z To 0 Step -1
                    Dim idx As Integer = GetIndexStein(New TripleX(Kandidat.x, Kandidat.y, idxZ))

                    Dim aktSIE As TripleX
                    If idx = -1 Then
                        aktSIE = New TripleX
                    Else
                        With SteinInfos(idx)
                            aktSIE = New TripleX(.X, .Y, idxZ, ValidePlace.Yes, .SteinInfoIndex, .SteinIndex, GetQuadrant(Kandidat.x, Kandidat.y, idxZ), SteinInfos(idx))
                        End With
                    End If
                    ssi.Add(aktSIE)
                Next
            End If

            Return ssi.ToArray

        End Function

#End Region

#Region "Render-Geometrie"

        ''' <summary>
        ''' Gibt den Left-Wert zur Anzeige des aktuellen arrFB-Steines an.
        ''' </summary>
        ''' <param name="X">X-Wert des aktuellen arrFB-Steines</param>
        ''' <param name="Z">Z-Wert des aktuellen arrFB-Steines</param>
        ''' <returns></returns>
        Public Function GetSteinRenderLeft(X As Integer, Z As Integer) As Integer
            With _sfd.SFLay
                Dim rasterX As Integer = .steinWidthHalf * (X - 1)
                Dim shift3D As Integer = .offset3DLeftSumme - (.offset3DLeftJeEbene * Z)

                Return .rxStageUsed.Left + rasterX + shift3D
            End With
        End Function
        '
        ''' <summary>
        ''' Gibt den Top-Wert zur Anzeige des aktuellen arrFB-Steines an.
        ''' </summary>
        ''' <param name="Y">Y-Wert des aktuellen arrFB-Steines</param>
        ''' <param name="Z">Z-Wert des aktuellen arrFB-Steines</param>
        ''' <returns></returns>
        Public Function GetSteinRenderTop(Y As Integer, Z As Integer) As Integer
            With _sfd.SFLay
                Dim rasterY As Integer = .steinHeightHalf * (Y - 1)
                Dim shift3D As Integer = .offset3DTopSumme - (.offset3DTopJeEbene * Z)

                Return .rxStageUsed.Top + rasterY + shift3D
            End With
        End Function
        'Ich habe noch Varianten hinzugefügt:
        '
        ''' <summary>
        '''  Gibt den Point-Wert zur Anzeige des aktuellen arrFB-Steines an.
        ''' </summary>
        ''' <param name="X"></param>
        ''' <param name="Y"></param>
        ''' <param name="Z"></param>
        ''' <returns></returns>
        Public Function GetSteinRenderPoint(X As Integer, Y As Integer, Z As Integer) As Point

            Return New Point(GetSteinRenderLeft(X, Z), GetSteinRenderTop(Y, Z))

        End Function

        Public Function GetSteinRenderPoint(tripl As Triple) As Point

            With tripl
                Return New Point(GetSteinRenderLeft(.x, .z), GetSteinRenderTop(.y, .z))
            End With

        End Function

        ''' <summary>
        '''  Gibt den Rectangle-Wert zur Anzeige des aktuellen arrFB-Steines an.
        ''' </summary>
        ''' <param name="X"></param>
        ''' <param name="Y"></param>
        ''' <param name="Z"></param>
        ''' <returns></returns>
        Public Function GetSteinRenderRect(X As Integer, Y As Integer, Z As Integer) As Rectangle

            Return New Rectangle(GetSteinRenderLeft(X, Z), GetSteinRenderTop(Y, Z), _sfd.SFLay.steinWidth, _sfd.SFLay.steinHeight)

        End Function

        Public Function GetSteinRenderRect(tripl As Triple) As Rectangle

            With tripl
                Return New Rectangle(GetSteinRenderLeft(.x, .z), GetSteinRenderTop(.y, .z), _sfd.SFLay.steinWidth, _sfd.SFLay.steinHeight)
            End With

        End Function

        '
        ''' <summary>
        ''' Rechnet eine Bildschirm-X-Position auf den X-Index der Grundfläche zurück.
        ''' Umkehrung von SteinOnStange_Left(X, 0).
        ''' </summary>
        ''' <param name="mouseX"></param>
        ''' <returns>
        ''' X-Index der Grundfläche oder -1, wenn mouseX außerhalb liegt.
        ''' </returns>
        Private Function GetGroundIndexX(mouseX As Integer) As Integer

            ' Umkehrung von:
            ' SteinOnStange_Left(X, 0) =
            ' rxStageUsed.Left + offset3DLeftSumme + steinWidthHalf * (X - 1)

            Dim relX As Integer = mouseX - _sfd.SFLay.rxStageUsed.Left - _sfd.SFLay.offset3DLeftSumme

            If relX < 0 Then
                Return -1
            End If

            Dim result As Integer = (relX \ _sfd.SFLay.steinWidthHalf) + 1

            If result < xMin OrElse result > xMax Then
                Return -1
            End If

            Return result

        End Function

        '
        ''' <summary>
        ''' Rechnet eine Bildschirm-Y-Position auf den Y-Index der Grundfläche zurück.
        ''' Umkehrung von SteinOnStange_Top(Y, 0).
        ''' </summary>
        ''' <param name="mouseY"></param>
        ''' <returns>
        ''' Y-Index der Grundfläche oder -1, wenn mouseY außerhalb liegt.
        ''' </returns>
        Private Function GetGroundIndexY(mouseY As Integer) As Integer

            ' Umkehrung von:
            ' SteinOnStange_Top(Y, 0) =
            ' rxStageUsed.Top + offset3DTopSumme + steinHeightHalf * (Y - 1)

            Dim relY As Integer = mouseY - _sfd.SFLay.rxStageUsed.Top - _sfd.SFLay.offset3DTopSumme

            If relY < 0 Then
                Return -1
            End If

            Dim result As Integer = (relY \ _sfd.SFLay.steinHeightHalf) + 1

            If result < yMin OrElse result > yMax Then
                Return -1
            End If

            Return result

        End Function

        '
        ''' <summary>
        ''' Gibt die Pos3D auf der Grundfläche zurück.
        ''' Wenn unter der Maus kein Stein gefunden wurde, ist unter der Maus
        ''' logischerweise die Grundfläche. Deren Pos3D gibt es hier.
        ''' (Wegen der 3D-Verschiebung kann nicht einfach die Z-Koordinate
        ''' nach oben verschoben werden. Es muss vorher nach Steinen gesucht werden.)
        ''' </summary>
        ''' <param name="mousePos"></param>
        ''' <returns></returns>
        Public Function GetPos3DOnGround(mousePos As Point) As Triple

            Dim xResult As Integer = GetGroundIndexX(mousePos.X)
            If xResult = -1 Then
                Return New Triple(ValidePlace.No)
            End If

            Dim yResult As Integer = GetGroundIndexY(mousePos.Y)
            If yResult = -1 Then
                Return New Triple(ValidePlace.No)
            End If

            Return New Triple(xResult, yResult, 0, ValidePlace.Yes)

        End Function

#End Region

#Region "Statistik"

        Public Sub ShowMessageBoxStatistik()
            Dim steineVorrat As New List(Of SteinIndexEnum)
            Dim steineSpielfeld As New List(Of SteinIndexEnum)

            If SteinInfos IsNot Nothing AndAlso SteinInfos.Count > 0 Then
                'Steine einsammeln
                For Each item As SteinInfo In SteinInfos
                    steineSpielfeld.Add(item.SteinIndex)
                Next
            End If
        End Sub

#End Region

#Region "Debug / Prüfungen"

        ''' <summary>
        ''' Die Funktion arbeitet nur, wenn Debugger.IsAttached, also nur innerhalb der IDE.
        ''' Prüft, ob alle Randfelder auf 0 sind. Wenn nicht, ist ein Stein falsch
        ''' eingetragen worden. Programmierfehler! Die Funktion wirft dann eine Exception,
        ''' </summary>
        Public Sub CheckRand()

            If Not Debugger.IsAttached Then
                Exit Sub
            End If

            ' Prüft den Rand in X- und Y-Richtung für alle Z-Ebenen
            For z As Integer = 0 To zMax
                ' Linker Rand (x=0)
                For y As Integer = 0 To yMax + 1
                    If arrFB(0, y, z) <> 0 Then
                        Throw New Exception($"Programmierfehler: Linker Rand ist nicht 0 in x=0 y={y} z={z}")
                    End If
                Next

                ' Rechter Rand (x = xMax+1)
                For y As Integer = 0 To yMax + 1
                    If arrFB(xMax + 1, y, z) <> 0 Then
                        Throw New Exception($"Programmierfehler: Rechter Rand ist nicht 0 in x={xMax + 1} y={y} z={z}")
                    End If
                Next

                ' Oberer Rand (y = 0)
                For x As Integer = 0 To xMax + 1
                    If arrFB(x, 0, z) <> 0 Then
                        Throw New Exception($"Programmierfehler: Oberer Rand ist nicht 0 in x={x} y=0 z={z}")
                    End If
                Next

                ' Unterer Rand (y = yMax+1)
                For x As Integer = 0 To xMax + 1
                    If arrFB(x, yMax + 1, z) <> 0 Then
                        Throw New Exception($"Programmierfehler: Unterer Rand ist nicht 0 in x={x} y={yMax + 1} z={z}")
                    End If
                Next
            Next

        End Sub

        Private Sub CheckSteinInfoIndices()

            If Not Debugger.IsAttached Then
                Exit Sub
            End If

            If IsNothing(_SteinInfos) Then
                Exit Sub
            End If

            If _SteinInfos.Count = 0 Then
                Exit Sub
            End If

            For idx As Integer = 0 To _SteinInfos.Count - 1
                If _SteinInfos(idx).SteinInfoIndex <> idx Then
                    Throw New Exception($"Programmierfehler: Konsistenz der Steininfos stimmt nicht SteinInfos({idx}).SteinInfoIndex hat den Wert {_SteinInfos(idx).SteinInfoIndex} anstelle {idx}")
                End If
            Next
        End Sub

        Private Sub CheckArrFBConsistency()

            ' später

        End Sub

        ''' <summary>
        ''' Erstellt ein neues arrFB mit größeren Dimensionen und kopiert
        ''' die bisherigen Werte hinein.
        ''' 
        ''' Es wird nur dann tatsächlich vergrößert, wenn:
        ''' - keine neue Dimension kleiner als die bisherige ist
        ''' - mindestens eine neue Dimension größer ist
        ''' 
        ''' Andernfalls wird das ursprüngliche Array unverändert zurückgegeben.
        ''' </summary>
        ''' <param name="arrFB">Das bestehende 3D-Array.</param>
        ''' <param name="xUBnd">Neue obere Grenze der X-Dimension.</param>
        ''' <param name="yUBnd">Neue obere Grenze der Y-Dimension.</param>
        ''' <param name="zUBnd">Neue obere Grenze der Z-Dimension.</param>
        ''' <returns>
        ''' Das vergrößerte Array oder, falls keine zulässige Vergrößerung vorliegt,
        ''' das unveränderte Ursprungsarray.
        ''' </returns>
        Private Function RedimArrFB(ByVal arrFB(,,) As Integer,
                                   ByVal xUBnd As Integer,
                                   ByVal yUBnd As Integer,
                                   ByVal zUBnd As Integer) As Integer(,,)

            If arrFB Is Nothing Then
                Return New Integer(xUBnd, yUBnd, zUBnd) {}
            End If

            Dim oldXUBnd As Integer = arrFB.GetUpperBound(0)
            Dim oldYUBnd As Integer = arrFB.GetUpperBound(1)
            Dim oldZUBnd As Integer = arrFB.GetUpperBound(2)

            'Nicht arbeiten, wenn irgendeine Dimension kleiner wird
            If xUBnd < oldXUBnd OrElse
               yUBnd < oldYUBnd OrElse
               zUBnd < oldZUBnd Then
                Return arrFB
            End If

            'Nicht arbeiten, wenn alles gleich geblieben ist
            If xUBnd = oldXUBnd AndAlso
               yUBnd = oldYUBnd AndAlso
               zUBnd = oldZUBnd Then
                Return arrFB
            End If

            Dim arrNew(xUBnd, yUBnd, zUBnd) As Integer

            For x As Integer = 0 To oldXUBnd
                For y As Integer = 0 To oldYUBnd
                    For z As Integer = 0 To oldZUBnd
                        arrNew(x, y, z) = arrFB(x, y, z)
                    Next
                Next
            Next

            Return arrNew

        End Function

#End Region

#Region "Load / Save"

        Public Shared Function OpenWithDialog(sub1Dir As AppDataSubDir, sub2Dir As AppDataSubSubDir, sub3Dir As String, filename As String) As SFInfo
            Dim fullpath As String = Path.Combine(INI.AppDataDirectory(sub1Dir, sub2Dir, sub3Dir), filename)
            Return Load(fullpath)
        End Function
        Public Shared Function OpenWithDialog(sub1Dir As AppDataSubDir, sub2Dir As AppDataSubSubDir, sub3Dir As BasisformEnum, filename As String, Optional header As String = "Spiel laden") As SFInfo
            Dim path As String = IO.Path.Combine(INI.AppDataDirectory(sub1Dir, sub2Dir, sub3Dir.ToString), filename)
            Dim fullpath As String = INI.AppDataFullPathWithOpenFileDialog(path, "*.*", header)
            If Not String.IsNullOrEmpty(fullpath) Then
                Return Load(path)
            Else
                Return New SFInfo
            End If
        End Function
        Public Shared Function Load(sub1Dir As AppDataSubDir, sub2Dir As AppDataSubSubDir, sub3Dir As String, filename As String) As SFInfo
            Dim fullpath As String = Path.Combine(INI.AppDataDirectory(sub1Dir, sub2Dir, sub3Dir), filename)
            Return Load(fullpath)
        End Function
        Public Shared Function Load(sub1Dir As AppDataSubDir, sub2Dir As AppDataSubSubDir, sub3Dir As BasisformEnum, filename As String) As SFInfo
            Dim fullpath As String = Path.Combine(INI.AppDataDirectory(sub1Dir, sub2Dir, sub3Dir.ToString), filename)
            Return Load(fullpath)
        End Function
        '
        ''' <summary>
        ''' Läd die jüngste SpielfeldInfo aus einem temporärem Verzeichniss
        ''' </summary>
        ''' <returns></returns>
        Public Shared Function LoadLastUsedSpielfeld() As SFInfo
            Dim subsubdir As AppDataSubSubDir = AppDataSubSubDir.Temporär_SpielfeldInfo

            Dim fullpath As String = INI.AppDataFullPath(AppDataSubDir.Temporär, subsubdir, AppDataFileName.Spielfeldinfo_xml, AppDataTimeStamp.LookForLastTimeStamp, maxFiles:=20)
            Return Load(fullpath)
        End Function

        ''' <summary>
        ''' zLwpD = myIO.GetPathInAnwendungsdatenII("Suche", GetType(IndexData).Name + ".xml")
        ''' </summary>
        ''' <returns></returns>
        Public Shared Function Load(fullpath As String) As SFInfo

            If IO.File.Exists(fullpath) Then
                Try
                    Using reader As New IO.StreamReader(fullpath)
                        Dim serializer As New Xml.Serialization.XmlSerializer(GetType(SFInfo))
                        Dim daten As SFInfo = CType(serializer.Deserialize(reader), SFInfo)

                        'Die Dimensionen des ArrFB ändern (nur vergrößern möglich),
                        'falls die Spielfeldgröße geändert wurde. (Das ist eine Hintertüre)
                        Dim size As Triple = daten.SpielSize
                        Dim xUbnd As Integer = (size.x * 2) + 1
                        Dim yUbnd As Integer = (size.y * 2) + 1
                        Dim zUBnd As Integer = size.z
                        daten.arrFB = daten.RedimArrFB(daten.arrFB, xUbnd, yUbnd, zUBnd)

                        daten.copySpielSizeToXyzValues(daten.SpielSize)

                        Return daten

                    End Using
                Catch
                    ' Fehler beim Laden – neue Instanz zurückgeben
                    Return New SFInfo
                End Try
            End If

            Return New SFInfo()

        End Function

        Public Function SpielfeldInfoFileExists(sub1Dir As AppDataSubDir, sub2Dir As AppDataSubSubDir, sub3Dir As String, filename As String) As Boolean
            Dim fullpath As String = Path.Combine(INI.AppDataDirectory(sub1Dir, sub2Dir, sub3Dir), filename)
            Return File.Exists(fullpath)
        End Function
        Public Function SpielfeldInfoFileExists(sub1Dir As AppDataSubDir, sub2Dir As AppDataSubSubDir, sub3Dir As BasisformEnum, filename As String) As Boolean
            Dim fullpath As String = Path.Combine(INI.AppDataDirectory(sub1Dir, sub2Dir, sub3Dir.ToString), filename)
            Return File.Exists(fullpath)
        End Function

        Public Sub save(sub1Dir As AppDataSubDir, sub2Dir As AppDataSubSubDir, sub3Dir As String, filename As String)
            Dim fullpath As String = Path.Combine(INI.AppDataDirectory(sub1Dir, sub2Dir, sub3Dir), filename)
            Save(fullpath)
        End Sub
        Public Sub save(sub1Dir As AppDataSubDir, sub2Dir As AppDataSubSubDir, sub3Dir As BasisformEnum, filename As String)
            Dim fullpath As String = Path.Combine(INI.AppDataDirectory(sub1Dir, sub2Dir, sub3Dir.ToString), filename)
            Save(fullpath)
        End Sub
        '
        ''' <summary>
        ''' Speichert mit Zeitstempel in einem er beiden temporärem Verzeichnissen.
        ''' </summary>
        Public Sub SaveLastUsedSpielfeld()
            Dim subsubdir As AppDataSubSubDir = AppDataSubSubDir.Temporär_SpielfeldInfo
            Dim fullpath As String = INI.AppDataFullPath(AppDataSubDir.Temporär, subsubdir, AppDataFileName.Spielfeldinfo_xml, AppDataTimeStamp.Add, maxFiles:=20)
            Save(fullpath)
        End Sub
        Public Sub Save(fullpath As String)
            ' Defensive: Null-Propagation falls SteinInfos Nothing ist
            SummeSteinInfos = If(SteinInfos IsNot Nothing, SteinInfos.Count, 0)
            Speicherdatum = Now
            '
            If Not IsNothing(Generator) Then
                GeneratorValuesForXml = New SpielsteinGeneratorValuesForXml
                GeneratorValuesForXml.CopySpielsteinGenerator_To_SpielsteinGeneratorValues(Generator)
            End If

            Dim ser As New XmlSerializer(GetType(SFInfo)) ' explizit, nicht Me.GetType()
            Dim settings As New XmlWriterSettings With {
            .Indent = True,
            .Encoding = New UTF8Encoding(encoderShouldEmitUTF8Identifier:=False)
        }

            Try
                Using fs As New FileStream(fullpath, FileMode.Create, FileAccess.Write, FileShare.None)
                    Using xw As XmlWriter = XmlWriter.Create(fs, settings)
                        ser.Serialize(xw, Me)
                    End Using
                End Using
            Catch ex As InvalidOperationException
                ' Diese Exception kommt typischerweise bei Serialisierungsproblemen (nicht-public, fehlender Ctor, etc.)
                Throw New InvalidOperationException(
                "XML-Serialisierung fehlgeschlagen. Prüfe: Public-Klassen/-Properties, parameterloser Public-Konstruktor, " &
                "konkrete Collection-Typen und Initialisierung der Collections.", ex)
            Catch ex As Exception
                Throw ' nicht verschlucken – hochreichen
            End Try
        End Sub
#End Region

    End Class
End Namespace