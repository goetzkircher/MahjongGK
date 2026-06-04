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
Imports MahjongGK.Contracts

#Disable Warning IDE0079
#Disable Warning IDE1006
#Disable Warning IDE0044

Namespace Spielfeld
    ''' <summary>
    ''' Pfad: MahjongGK/Spielfeld/Daten
    ''' Die Klasse hieß ursprünglich SpielFeldInfo und beinhaltet alle Spielfeld-Daten
    ''' darunter eine List(Of SteinInfo), die die SteinInfo für jeden Stein hält.
    ''' Das ist die Klasse, die gespeichert wird.
    ''' </summary>
    Public Class SFInfo

#Region "Konstruktor / InitDragDropBitmaps"

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

            TestEnumStein()

            If CheckSpielsize(spielSize) Then
                copySpielSizeToXyzValues(spielSize)
            Else
                Throw New Exception("Ungültige Spielfelddimensionen.")
            End If

            Me.SpielSizeInSteinen = spielSize

            ReDim ArrFB(xUBnd, yUBnd, zUBnd)

            SteinInfos = New List(Of SteinInfo)

            MakeDirtyTopSteinInfos()

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
                _xMax = .x * 2 '
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

        Private Sub CopyArrFbUboundsToSizeValues()

            If _arrFB Is Nothing Then
                Throw New Exception("Programmierfehler: arrFeldBeschr darf nicht Nothing sein.")
            End If

            Dim xUbnd As Integer = _arrFB.GetUpperBound(0)
            Dim yUbnd As Integer = _arrFB.GetUpperBound(1)
            Dim zUbnd As Integer = _arrFB.GetUpperBound(2)

            _xMin = 1
            _yMin = 1
            _zMin = 0

            _xMax = xUbnd - 1
            _yMax = yUbnd - 1
            _zMax = zUbnd

            _xUBnd = xUbnd
            _yUBnd = yUbnd
            _zUBnd = zUbnd

            SpielSizeInSteinen = GetSizeInSteinenFromUBounds(xUbnd, yUbnd, zUbnd)

            With SpielSizeInSteinen
                _xMaxSteine = .x
                _yMaxSteine = .y
                _zMaxSteine = .z
            End With

        End Sub

#End Region

#Region "Persistente Kerndaten"

        Public Property Version As String = "1"

        Public Property Name As String = "unbenannt"
        Public Property Beschreibung As String = "nicht festgelegt"
        Public Property Anmerkung As String = "keine"
        Public Property SpielInfo As String = "keine vorhanden"

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
        Private ReadOnly _sessionIdent As String = Guid.NewGuid.ToString

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

        Private _SpielSizeInSteinen As Triple
        Public Property SpielSizeInSteinen As Triple
            Get
                Return _SpielSizeInSteinen
            End Get
            Set(value As Triple)
                'Diese Konstruktion ist bedingt durch die Backdor, daß man in der XML
                'die Spielfeldgröße ändern kann, damit das vom Programm erkannt wird. 
                If _SpielSizeInSteinenFromXml Is Nothing Then
                    _SpielSizeInSteinenFromXml = value.DeepCopy
                End If
                _SpielSizeInSteinen = value
                copySpielSizeToXyzValues(value)
                MakeDirtyTopSteinInfos()
            End Set
        End Property
        '
        ''' <summary>
        ''' Wird in Load gebraucht um den Wert von SpielSizeInSteinen zu sichern,
        ''' bevor er bei der Zuweisung von arrFB überschrieben wird.
        ''' </summary>
        Private _SpielSizeInSteinenFromXml As Triple = Nothing
        '
        ''' <summary>
        ''' Im Normalfall ist diese Property Nothing und wird daher nicht serialisiert.
        ''' Es ist möglich in der Xml mit einem Fremdeditor die Werte von SpielSizeInSteinen
        ''' zu ändern. Werden ungültige Werte eingestellt, wird die Änderung nicht vorgenommen
        ''' und hier eine Fehlermeldung eingetragen und die Xml zurückgeschrieben.
        ''' </summary>
        ''' <returns></returns>
        Public Property ErrorMsg As String = Nothing
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

        Public ReadOnly Property IsValide As Boolean
            Get
                Return _xMax > 0 AndAlso _yMax > 0
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

        Private _SteinInfos As New List(Of SteinInfo)
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

#Region "arrFB Serialisierung / Laufzeitobjekte"

        ' Private _arrFB As Integer(,,)

        <XmlIgnore>
        Public Property ArrFB As Integer(,,)
        '    Get
        '        Return _arrFB
        '    End Get
        '    Set(value As Integer(,,))
        '        _arrFB = value
        '    End Set
        'End Property

        <XmlElement("ArrFB")>
        Public Property ArrFB_Anchors As FBCodec2x2.FBAnchors
            Get
                Return FBCodec2x2.ToAnchors(ArrFB, FB_INDEX_FACTOR)
            End Get
            Set(value As FBCodec2x2.FBAnchors)
                If value Is Nothing Then
                    ArrFB = Nothing
                Else
                    ArrFB = FBCodec2x2.FromAnchors(value, FB_INDEX_FACTOR)
                End If
            End Set
        End Property

        <XmlIgnore>
        Public Property Generator As SpielsteinGenerator = Nothing

        Public Property GeneratorValuesForXml As SpielsteinGeneratorValuesForXml = Nothing

#End Region

#Region "Undo/Redo"

        Public Property EditorUndo As New EditorUndoItems

        Private _undoText As String
        Private _undoStep As Integer = -1
        Private Const STEPSVISIBLE As Integer = 30

        Public ReadOnly Property HasUndoText As Boolean
            Get
                Return _undoStep >= 0
            End Get
        End Property
        '
        ''' <summary>
        ''' Text, der sowohl im Edtor als auch im Stock links mittig
        ''' in rxHeader ausgegeben wird und der nach STEPSVISIBLE Rendersteps
        ''' sich selber löscht.
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property ConsumeUndoText As String
            Get
                _undoStep -= 1
                If _undoStep < 0 Then
                    _undoText = String.Empty
                End If
                Return _undoText
            End Get
        End Property

        Public Sub SetUndoText(text As String)
            If Not String.IsNullOrEmpty(text) Then
                _undoStep = STEPSVISIBLE
                _undoText = text
            Else
                _undoStep = -1
                _undoText = String.Empty
            End If
        End Sub
        Public Sub SetUndoText(job As UnReDoJob)

            Select Case job
                Case UnReDoJob.EtoE
                    SetUndoText("Undo Edit->Edit")
                Case UnReDoJob.EtoS
                    SetUndoText("Undo Edit-Stock")
                Case UnReDoJob.FillStock
                    SetUndoText("Undo Füllen")
                Case UnReDoJob.None
                    SetUndoText("Nichts")
                Case UnReDoJob.SortStock
                    SetUndoText("Undo Sort")
                Case UnReDoJob.StoE
                    SetUndoText("Undo Stock-Editor")
                Case UnReDoJob.StoS
                    SetUndoText("Undo Stock-Stoc")
                Case Else
                    SetUndoText("???")
            End Select

        End Sub

#End Region

#Region "Information"

        Private _aktSelectableCount As Integer
        Private _aktRemovableCount As Integer

        Public Class SteinCountInfo
            Sub New()
            End Sub

            Public summeSteineGesamt As Integer
            Public summeSteineAktuell As Integer
            Public davonSelectable As Integer
            Public davonRemovable As Integer
            Public imVorrat As Integer
            Public xMax As Integer
            Public yMax As Integer
            Public zMax As Integer

            Public Function AnyValueChanged(oldSci As SteinCountInfo) As Boolean
                If oldSci Is Nothing Then Return True
                With oldSci
                    If .summeSteineGesamt <> Me.summeSteineGesamt Then Return True
                    If .summeSteineAktuell <> Me.summeSteineAktuell Then Return True
                    If .davonSelectable <> Me.davonSelectable Then Return True
                    If .davonRemovable <> Me.davonRemovable Then Return True
                    If .imVorrat <> Me.imVorrat Then Return True
                    If .xMax <> Me.xMax Then Return True
                    If .yMax <> Me.yMax Then Return True
                    If .zMax <> Me.zMax Then Return True
                    Return True
                End With
            End Function
        End Class

        Public Function AktSteinCountInfo() As SteinCountInfo
            ' das könnte überflüssig sein, da es ja sowieso laufend aufgerufen wird.
            ' UpdateTopSteinInfos()
            Dim sci As New SteinCountInfo
            With sci
                .summeSteineGesamt = If(SteinInfos IsNot Nothing, SteinInfos.Count, 0)
                .davonSelectable = _aktSelectableCount
                .davonRemovable = _aktRemovableCount
                For Each item As SteinInfo In SteinInfos
                    If Not item.IsRemoved Then
                        .summeSteineAktuell += 1
                    End If
                Next
                .xMax = xMax \ 2
                .yMax = yMax \ 2
                .zMax = zMax + 1
                .imVorrat = Generator.StockAktCount
            End With
            Return sci
        End Function

        ''Diese Properties gehörten eigentlich nach hierher, sind aber ganz 
        ''oben angesiedelt, damit sie in der Xml am Anfang sichtbar sind.
        ''Public Property Name As String = "unbenannt"
        ''Public Property Beschreibung As String = "nicht festgelegt"
        ''Public Property Anmerkung As String = "keine"
        ''
        <XmlIgnore>
        Public Property SpielfeldPicture As Bitmap
            Get
                If String.IsNullOrEmpty(SpielfeldPictureXml) Then
                    Return Nothing
                End If

                Dim bytes() As Byte = Convert.FromBase64String(SpielfeldPictureXml)

                Using ms As New MemoryStream(bytes)
                    Using img As Image = Image.FromStream(ms)
                        Return New Bitmap(img)
                    End Using
                End Using
            End Get
            Set(value As Bitmap)
                If value Is Nothing Then
                    SpielfeldPictureXml = String.Empty
                    Return
                End If

                Using ms As New MemoryStream()
                    value.Save(ms, Imaging.ImageFormat.Png)
                    SpielfeldPictureXml = Convert.ToBase64String(ms.ToArray(), Base64FormattingOptions.InsertLineBreaks)
                End Using
            End Set
        End Property

        Private _spielfeldPictureXml As String
        Public Property SpielfeldPictureXml As String
            Get
                Return _spielfeldPictureXml
            End Get
            Set(value As String)
                _spielfeldPictureXml = value
            End Set
        End Property

#End Region

#Region "SteinInfos - interne Verwaltung"

        Private Sub AddSteinInfoInternal(newSteinInfo As SteinInfo)

            _SteinInfos.Add(newSteinInfo)
            NotifySteinInfosChanged()

        End Sub

        Private Sub NotifySteinInfosChanged()

            SyncToggleFlag()
            MarkTopSearchDirty()
            MarkSteinCountChanged()
            MakeDirtyTopSteinInfos()

            Update_SteinInfos_RectQuadranten()

        End Sub

        Private Sub MarkTopSearchDirty()
            _indexTopSearchDirty = True
        End Sub

        Private Sub MarkSteinCountChanged()
            _SteinInfosCountChanged = True
        End Sub

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
        'TODO
        '
        '''' <summary>
        '''' Spielbar ist ein Spiel dann, wenn Daten vorhanden sind und wenn
        '''' wenn alle Paare auf dem Spielfeld vollständig sind, also keine
        '''' "StrohWitwen" oder "Witwer" vorhanden sind und den Werkstattsteinen
        '''' alle andere Grafiken zugeordnet wurden
        '''' </summary>
        '''' <returns></returns>
        'Public ReadOnly Property IsPlayable As Boolean
        '    Get
        '        If IsNothing(SteinInfos) OrElse SteinInfos.Count = 0 Then
        '            Return False
        '        End If

        '        Dim arrKlickGruppeCount(MJ_STEININDEX_MAX) As Integer
        '        Dim foundWerkstattStein As Boolean

        '        For Each stein As SteinInfo In SteinInfos
        '            arrKlickGruppeCount(stein.KlickGruppe) += 1
        '            If stein.IsWerkbankStein Then
        '                foundWerkstattStein = True
        '            End If
        '        Next

        '        If foundWerkstattStein Then
        '            Return False
        '        End If

        '        For idx As Integer = 0 To MJ_STEININDEX_MAX
        '            If (arrKlickGruppeCount(idx) Mod 2) <> 0 Then
        '                'Für 0 gilt: 0 Mod 2 = 0 -> braucht deshalb nicht abgefangen zu werden,
        '                'denn nicht vorhandene Steinpaare brauchen nicht berücksichtigt werden,
        '                'ausgenommen, es gibt überhaupt keine Steine. Das ist oben abgefangen.
        '                Return False
        '            End If
        '        Next

        '        Return True

        '    End Get

        'End Property

        Public ReadOnly Property IsEmpty As Boolean
            Get
                'If INI.Debug_StopRendering Then Stop
                If IsNothing(SteinInfos) Then Return True
                '' Das hier nicht! If SteinInfos.Count = 0 Then Return True
                'sonst wird ein leeres Spielfeld nicht angezeigt
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

#Region "Spielfeld - AddRenderBitmapTopZOrder / Insert / Remove"

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
        Public Function AddSteinToSpielfeld(steinIndex As SteinSymbol, steinPos3D As Triple) As Boolean

            'Der steinInfoIndex wird hier gesichert, obwohl er gleichlautend ist mit dem
            'Index in SteinInfos. Grund: werden später Steine im Editor entfernt, verschieben sich die
            'Indexnummern in SteinInfos und da muss arrFB aktualisert werden. Dazu braucht man den
            '"alten" steinInfoIndex, eben diesen steinInfoIndex.
            Dim newSteinInfo As New SteinInfo(steinInfoIndex:=SteinInfos.Count, steinIndex, steinPos3D, _ArrFB, _sfd)

            If Not steinPos3D.IsInsideSpielfeldBounds(ArrFB) Then
                'Falsche Positionsangabe.
                'Kein Throw Nex Exception, sondern Anzeige auf dem Spielfeld
                'Fehlergrafik in die oberste Ebene als frei schwebender Stein.
                'Im fertig entwickeltem Programm sollte dieser Teil nicht mehr
                'aufgerufen werden :-)
                '
                'Linke obere Ecke der obersten Ebene
                Dim tpl As New Triple(1, 1, ArrFB.GetUpperBound(2))

                Do
                    Dim tplR As Triple = SearchPlace(tpl, direction:=Direction.Right)
                    Select Case tplR.Valide
                        Case ValidePlace.NoFundamentFound   'Zeilenende erreicht.
                            tpl.y += 2 'Eine Steinreihe tiefer weitersuchen
                            tpl.x = 1
                            If tpl.y > ArrFB.GetUpperBound(1) - 1 Then
                                'Die ganze oberste Ebene ist vollgepflastert mit Steinen.
                                'Da das ziemlich sicher nicht absichlich geschehen ist,
                                'unterstelle ich, das da viele Fehlergrafiken dabei sind
                                'und breche ab ohne weitere Prüfung und setze den
                                'Stein auf die Position links oben etwas versetzt,
                                'also optisch auf eine Ebene, die es garnicht gibt.
                                tpl.x = 2
                                tpl.y = 2

                                'SetStein rekursiv, aber mit jetzt gültigen Werten aufrufen
                                AddSteinToSpielfeld(SteinSymbol.ErrorSy, tpl)
                                Return False
                            End If

                        Case ValidePlace.Yes, ValidePlace.NoFundamentFound
                            ' FoundResult.NoFundament ist in diesem Fall OK, er wird zum freischwebendem Stein.
                            'SetStein rekursiv, aber mit jetzt gültigen Werten aufrufen
                            AddSteinToSpielfeld(SteinSymbol.ErrorSy, tplR)
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
                .SetArrFbReferenz(ArrFB)
            End With

            If Not steinPos3D.IsInsideSpielfeldBounds(ArrFB) Then
                'Falsche Positionsangabe.
                'Kein Throw Nex Exception, sondern Anzeige auf dem Spielfeld
                'Fehlergrafik in die oberste Ebene als frei schwebender Stein.
                'Im fertig entwickeltem Programm sollte dieser Teil nicht mehr
                'aufgerufen werden :-)
                '
                'Linke obere Ecke der obersten Ebene
                Dim tpl As New Triple(1, 1, ArrFB.GetUpperBound(2))

                Do
                    Dim tplR As Triple = SearchPlace(tpl, direction:=Direction.Right)
                    Select Case tplR.Valide
                        Case ValidePlace.NoFundamentFound   'Zeilenende erreicht.
                            tpl.y += 2 'Eine Steinreihe tiefer weitersuchen
                            tpl.x = 1
                            If tpl.y > ArrFB.GetUpperBound(1) - 1 Then
                                'Die ganze oberste Ebene ist vollgepflastert mit Steinen.
                                'Da das ziemlich sicher nicht absichlich geschehen ist,
                                'unterstelle ich, das da viele Fehlergrafiken dabei sind
                                'und breche ab ohne weitere Prüfung und setze den
                                'Stein auf die Position links oben etwas versetzt,
                                'also optisch auf eine Ebene, die es garnicht gibt.
                                tpl.x = 2
                                tpl.y = 2

                                'SetStein rekursiv, aber mit jetzt gültigen Werten aufrufen
                                AddSteinToSpielfeld(SteinSymbol.ErrorSy, tpl)
                                Return False
                            End If

                        Case ValidePlace.Yes, ValidePlace.NoFundamentFound
                            ' FoundResult.NoFundament ist in diesem Fall OK, er wird zum freischwebendem Stein.
                            'SetStein rekursiv, aber mit jetzt gültigen Werten aufrufen
                            AddSteinToSpielfeld(SteinSymbol.ErrorSy, tplR)
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

            If IsNothing(werkbankResult) OrElse IsNothing(werkbankResult.steinInfos) OrElse IsNothing(ArrFB) Then
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
                        'TODO
                        '.steinInfos(idx).SteinStatusUsed = SteinStatus.I08WerkstückEinfügeFehler
                        '.steinInfos(idx).SteinStatusIst = SteinStatus.I08WerkstückEinfügeFehler
                    Next
                End If

                For wbrZ As Integer = 0 To .arrFB.GetUpperBound(2)
                    For wbrX As Integer = 1 To .arrFB.GetUpperBound(0) - 1
                        For wbrY As Integer = 1 To .arrFB.GetUpperBound(1) - 1
                            If IsIndexQuadrant(.arrFB(wbrX, wbrY, wbrZ)) Then
                                Dim idx As Integer = GetSteinInfoIndex(.arrFB(wbrX, wbrY, wbrZ))
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
        ''' Entfernt den Stein vom Spielfeld (Nicht verwechseln mit IsRemoved, dem Flag) 
        ''' vollständig d.h. einschließlich aller Daten vom Spielfeld (SteinInfos und arrFB)
        ''' Hinweis: Um ihn nicht mehr anzuzeigen muß er nicht entfernt werden. Dazu wird
        ''' lediglich in arrFB IsRemoved gesetzt.
        ''' </summary>
        Public Sub RealRemoveSteinFromSpielfeld(steinInfoIndex As Integer)

            If steinInfoIndex >= SteinInfos.Count Then
                Throw New Exception($"Programmierfehler: Versuch mit dem nicht vorhandenen steinInfoIndex = {steinInfoIndex} einen Stein zu entfernen.")
            End If

            'den arrFB-Eintrag entfernen
            Dim tpl As Triple = SteinInfos(steinInfoIndex).Pos3D

            With tpl
                ArrFB(.x, .y, .z) = 0
                ArrFB(.x + 1, .y, .z) = 0
                ArrFB(.x, .y + 1, .z) = 0
                ArrFB(.x + 1, .y + 1, .z) = 0
            End With
            '
            'Die SteinInfo in SteinInfos entfernen
            SteinInfos.Remove(SteinInfos(steinInfoIndex))
            NotifySteinInfosChanged()
            '
            'In SteinInfo und im arrFB verschieben sich jetzt alle steinInfoIndex um -1.
            If steinInfoIndex >= SteinInfos.Count Then
                'Es wurde der letzte Stein, der UBound, entfernt
                'Die Reihenfolge stimmt also noch
                Exit Sub
            End If
            '
            For idx As Integer = steinInfoIndex To SteinInfos.Count - 1
                SteinInfos(idx).SteinInfoIndex = idx
                SetSteinInfoIndex(SteinInfos(idx).Pos3D, idx)
            Next

        End Sub
        '
        ''' <summary>
        ''' Entfernt den Stein vom Spielfeld (Nicht verwechseln mit IsRemoved, dem Flag) 
        ''' </summary>
        ''' <param name="steinInfo"></param>
        Public Sub RealRemoveSteinFromSpielfeld(steinInfo As SteinInfo)
            RealRemoveSteinFromSpielfeld(steinInfo.SteinInfoIndex)
        End Sub
        '
        ''' <summary>
        ''' Entfernt den Stein vom Spielfeld (Nicht verwechseln mit IsRemoved, dem Flag) 
        ''' </summary>
        ''' <param name="triple"></param>
        Public Sub RealRemoveSteinFromSpielfeld(triple As Triple)
            RealRemoveSteinFromSpielfeld(GetSteinInfoIndex(triple))
        End Sub
        '
        ''' <summary>
        ''' Entfernt den Stein vom Spielfeld (Nicht verwechseln mit IsRemoved, dem Flag) 
        ''' </summary>
        ''' <param name="tripleX"></param>
        Public Sub RealRemoveSteinFromSpielfeld(tripleX As TripleX)
            RealRemoveSteinFromSpielfeld(GetSteinInfoIndex(tripleX.ToTriple))
        End Sub
        '
        ''' <summary>
        ''' Entfernt den Stein vom Spielfeld (Nicht verwechseln mit IsRemoved, dem Flag) 
        ''' </summary>
        Public Sub RealRemoveLastSteinFromSpielfeld()
            If SteinInfos.Count = 0 Then
                Exit Sub
            Else
                RealRemoveSteinFromSpielfeld(steinInfoIndex:=SteinInfos.Count - 1)
            End If
        End Sub

#End Region

#Region "Spielfeld - interne AddRenderBitmapTopZOrder/Remove-Helfer"

        'Private Function HandleInvalidInsertPosition(steinIndex As SteinSymbol, steinPos3D As Triple) As Boolean

        '    ' optional später aus AddSteinToSpielfeld herausziehen
        '    Return False

        'End Function

        Private Sub WriteSteinToArrFB(steinPos3D As Triple, steinInfoIndex As Integer)

            CopySteinIndexToSpielfeldPos3DAndSetOffsetXY(steinPos3D, steinInfoIndex)

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

                                Dim idx As Integer = GetSteinInfoIndex(.wbArrFB(wbrX, wbrY, wbrZ))
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

            If Not triple.IsInsideSpielfeldBounds(ArrFB) Then
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
            If ArrFB(infoFBx + 1, infoFBy + 1, infoFBz) <> 0 Then Return False
            '
            'Den FB genau unter dem infoFB
            If ArrFB(infoFBx, infoFBy + 1, infoFBz) <> 0 Then Return False

            'Den FB rechts vom infoFB 
            If ArrFB(infoFBx + 1, infoFBy, infoFBz) <> 0 Then Return False

            'Der infoFB 
            If ArrFB(infoFBx, infoFBy, infoFBz) <> 0 Then Return False
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
                If ArrFB(infoFBx + 1, infoFBy + 1, infoFBz) <> 0 Then Return False
                '
                'Den FB genau unter dem infoFB
                If ArrFB(infoFBx, infoFBy + 1, infoFBz) <> 0 Then Return False
                '
                'Den FB rechts vom infoFB 
                If ArrFB(infoFBx + 1, infoFBy, infoFBz) <> 0 Then Return False

                'Der infoFB selber 
                If ArrFB(infoFBx, infoFBy, infoFBz) <> 0 Then Return False

            Else
                'Auf der aktuellen Ebene müssen 4 Felder frei sein
                '
                'Den FB rechts unten vom infoFB
                If ArrFB(infoFBx + 1, infoFBy + 1, infoFBz) <> 0 Then Return False
                '
                'Den FB genau unter dem infoFB
                If ArrFB(infoFBx, infoFBy + 1, infoFBz) <> 0 Then Return False
                '
                'Den FB rechts vom infoFB 
                If ArrFB(infoFBx + 1, infoFBy, infoFBz) <> 0 Then Return False

                'Der infoFB selber 
                If ArrFB(infoFBx, infoFBy, infoFBz) <> 0 Then Return False

                'Alle Plätze des infoFB in der Ebene drunter müssen belegt sein.
                '(infoFBx, infoFBY und infoFBz sind die Koordinaten des abgefragten infoFB)
                '
                'genau eine Ebene tiefer gehen.
                infoFBz -= 1
                '
                'Den FB rechts unter dem infoFB
                If ArrFB(infoFBx + 1, infoFBy + 1, infoFBz) = 0 Then Return False
                '
                'Den FB genau unter dem infoFB
                If ArrFB(infoFBx, infoFBy + 1, infoFBz) = 0 Then Return False
                '
                'Den FB rechts vom infoFB 
                If ArrFB(infoFBx + 1, infoFBy, infoFBz) = 0 Then Return False

                'Der infoFB selber 
                If ArrFB(infoFBx, infoFBy, infoFBz) = 0 Then Return False

            End If

            Return True

        End Function

        Public Function IsRemovable(steinInfoIndex As Integer) As Boolean

            If steinInfoIndex < 0 Then
                Return False
            ElseIf IsNothing(_SteinInfos) Then
                Return False
            ElseIf _SteinInfos.Count = 0 Then
                Return False
            ElseIf steinInfoIndex >= _SteinInfos.Count Then
                Return False
            End If

            Dim si As SteinInfo = _SteinInfos(steinInfoIndex)

            '1. Regel:  Wenn der Stein auf der obersten Lage liegt, sind automatisch
            '           alle vier Quadranten sichbar.
            '           Es muss nur geprüft werden, ob link oder rechts kein Stein ist.
            With si.Pos3D
                If .z = zMax Then
                    'links
                    If ArrFB(.x - 1, .y, .z) = 0 AndAlso ArrFB(.x - 1, .y + 1, .z) = 0 Then
                        Return True
                    ElseIf ArrFB(.x + 2, .y, .z) = 0 AndAlso ArrFB(.x + 2, .y + 1, .z) = 0 Then
                        Return True
                    Else
                        Return False
                    End If
                End If
            End With
            '
            '2. Regel: Es müssen alle Felder über dem Stein frei sein
            '
            With si.Pos3D
                'über Quadrant LO
                If ArrFB(.x, .y, .z + 1) <> 0 Then Return False
                'RO
                If ArrFB(.x, .y + 1, .z + 1) <> 0 Then Return False
                'LU
                If ArrFB(.x + 1, .y, .z + 1) <> 0 Then Return False
                'RU
                If ArrFB(.x + 1, .y + 1, .z + 1) <> 0 Then Return False
                '
                '3. Regel: Es müssen alle Felder links oder rechts frei sein.
                '          (Wenn die frei sind, sind die darüber auch frei)
                'links
                If ArrFB(.x - 1, .y, .z) = 0 AndAlso ArrFB(.x - 1, .y + 1, .z) = 0 Then Return True
                'rechts
                If ArrFB(.x + 2, .y, .z) = 0 AndAlso ArrFB(.x + 2, .y + 1, .z) = 0 Then Return True
                '
                Return False

            End With

        End Function

        Public Function IsRemovable(triple As TripleX) As Boolean
            With triple
                Return IsRemovable(.x, .y, .z)
            End With
        End Function

        Public Function IsRemovable(x As Integer, y As Integer, z As Integer) As Boolean

            If x < xMin OrElse x > xMax OrElse y < yMin OrElse y > yMax OrElse z < zMin OrElse z > zMax Then
                Throw New Exception("Programmierfehler: versuchter Zugriff auf arrFeldBeschr außerhalb der Grenzen.")
            End If

            Return IsRemovable(GetSteinInfoIndex(x, y, z))

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
                    If Not tpl.IsInsideSpielfeldBounds(ArrFB) Then
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

                    If Not tpl.IsInsideSpielfeldBounds(ArrFB) Then
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

                Dim tpl2 As New Triple(tpl.x + addX, tpl.y + addY, tpl.z)

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
        ' Dim isRemoved As Boolean = (fb And FLAG_IsRemoved) <> 0
        ' Dim index As Integer = (fb >> FB_INDEX_SHIFT) - 1
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
        ' If isRemoved Then
        '    fb = fb Or FLAG_IsRemoved
        ' Else
        '    fb = fb And Not FLAG_IsRemoved
        ' End If
        '
        ' Dim flags As Integer = fb And FB_FLAG_MASK
        ' fb = ((newValue + 1) << FB_INDEX_SHIFT) Or flags
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
        ''' also in der Referenzsäule des Steins. Dort ist er um 1 erhöht und vier Bits
        ''' nah links verschoben gespeichert, damit der Wert immer ungleich 0 ist und die vier
        ''' niederwertigsten Stellen als Flagfeld frei bleiben.
        ''' 
        ''' In den vier anderen Feldern steht nicht der Index selbst, sondern nur eine
        ''' Offset-Information zur Referenzsäule. Über FLAG_XOffset und FLAG_YOffset
        ''' wird bestimmt, ob der Feldbeschreiber eine Spalte nach links und/oder eine
        ''' Zeile nach oben versetzt gelesen werden muss.
        ''' 
        ''' Dadurch kann von jedem der vier belegten Felder aus wieder auf das linke obere
        ''' Referenzfeld des Steins zugegriffen und dort der Steinindex gelesen werden.
        ''' 
        ''' Wird die Funktion auf ein leeres Feld angewendet, ist arrFB = 0. Dann sind
        ''' beide Offsets 0, es wird also erneut auf dasselbe Feld zugegriffen.
        ''' 
        ''' Vorsicht: IsIndexQuadrant arbeitet anders, weil dort kein Offset berücksichtigt
        ''' wird. 
        ''' Zum Iterieren durch das Feld deshalb IsIndexQuadrant oder GetSteinInfoIndexLO verwenden,
        ''' ausgenommen, es wird mit dem Toggleflag gearbeitet, das die bereits verarbeiteten
        ''' Quadranten dann ausblendet.
        ''' </summary>
        ''' <param name="x"></param>
        ''' <param name="y"></param>
        ''' <param name="z"></param>
        ''' <returns></returns> '
        Public Function GetSteinInfoIndex(x As Integer, y As Integer, z As Integer) As Integer
            Dim fb As Integer = ArrFB(x, y, z)
            Dim offsetX As Integer = If((fb And FLAG_XOffset) <> 0, 1, 0)
            Dim offsetY As Integer = If((fb And FLAG_YOffset) <> 0, 1, 0)
            Return (ArrFB(x - offsetX, y - offsetY, z) >> FB_INDEX_SHIFT) - 1
            'Auf ein unbelegtes Feld angewendet passiert folgendes:
            'offsetX und Offset Y sind 0, d.h. x und y werden nicht verändert.
            'Der Index ((arrFB(x - offsetX, y - offsetY, z) >> FB_INDEX_SHIFT)) ist auch 0
            '1 ab, gibt minus 1. ==> es muss nicht extra auf arrFB(x, y, z) = 0 geprüft werden,
            'ob ein Feld leer ist, es kann immer gleich nach dem SteinSymbolIndex gefragt werden.
        End Function

        ''
        '' Es gilt generell:
        '' Dim flags As Integer = fb And &HF
        '' Dim indexPlus1 As Integer = fb >> 4
        '' fb = (indexPlus1 << 4) Or flags

        '
        ''' <summary>
        ''' Diese Überladung holt des Index direkt aus dem arrFB unter Berücksichtigung der OffsetXY
        ''' Siehe die erste Überladung.
        ''' VORSICHT, zum Iterieren IsIndexQuadrant verwenden.
        ''' </summary>
        ''' <param name="tripl"></param>
        ''' <returns></returns>'
        Public Function GetSteinInfoIndex(tripl As Triple) As Integer
            'Weil zeitkritisch doppelter Code
            With tripl
                Dim fb As Integer = ArrFB(.x, .y, .z)
                Dim offsetX As Integer = If((fb And FLAG_XOffset) <> 0, 1, 0)
                Dim offsetY As Integer = If((fb And FLAG_YOffset) <> 0, 1, 0)
                Return (ArrFB(.x - offsetX, .y - offsetY, .z) >> FB_INDEX_SHIFT) - 1
            End With
        End Function

        ''' <summary>
        ''' Den Index und zur Prüfung, ob einer der Quadranten zu einem Stein gehört.
        ''' VORSICHT, zum Iterieren auf der Suche nach IndexStein IsIndexQuadrant verwenden.
        ''' 
        ''' </summary>
        ''' <param name="fb"></param>
        ''' <returns></returns>
        Public Shared Function GetSteinInfoIndex(fb As Integer) As Integer
            'Der Index wird um 1 erhöht gespeichert, weil index = 0 gleichbedeutend
            'wäre mit "freier Platz"
            Return (fb >> FB_INDEX_SHIFT) - 1
        End Function
        '
        ''' <summary>
        ''' Gibt den GetSteinInfoIndex nur, wenn (x,y,z) auf dden linken oberen
        ''' Quadranten zeigen. Geeignet zum vollständigen iterieren durch den ArrFB,
        ''' ohne den Ausschluss durch das ToggleFlag.
        ''' </summary>
        ''' <param name="x"></param>
        ''' <param name="y"></param>
        ''' <param name="z"></param>
        ''' <returns></returns>
        Public Function GetSteinInfoIndexLO(x As Integer, y As Integer, z As Integer) As Integer
            Dim fb As Integer = ArrFB(x, y, z)
            If (fb >> FB_INDEX_SHIFT) > 0 Then
                Return (ArrFB(x, y, z) >> FB_INDEX_SHIFT) - 1
            Else
                Return -1
            End If
        End Function

        ''' <summary>
        ''' Prüft, ob an der Stelle ein Felbbeschreiber FB ist, der einen SteinInfoIndex enthält.
        ''' Kann zum Iterieren durch das Feld benutzt werden.
        ''' </summary>
        ''' <param name="fb"></param>
        ''' <returns></returns>
        Public Shared Function IsIndexQuadrant(fb As Integer) As Boolean
            'Der Index wird um 1 erhöht gespeichert, weil index = 0 gleichbedeutend
            'wäre mit "freier Platz" hier wird die 1 nicht abgezogen!
            'so ist sichergestellt, daß nur True ist bei den linken oberen Quadranten
            'und die anderen drei Quadranten nicht berücksichtigt werden.
            Return (fb >> FB_INDEX_SHIFT) > 0
        End Function

        ''' <summary>
        ''' Prüft, ob an der Stelle ein Felbeschreiber FB ist, der einen SteinInfoIndex enthält.
        ''' Kann zum Iterieren durch das Feld benutzt werden.
        ''' </summary>
        Public Function IsIndexQuadrant(tripl As Triple) As Boolean
            'Der Index wird um 1 erhöht gespeichert, weil index = 0 gleichbedeutend
            'wäre mit "freier Platz" hier wird die 1 nicht abgezogen!
            'so ist sichergestellt, daß nur True ist bei den linken oberen Quadranten
            'und die anderen drei Quadranten nicht berücksichtigt werden.
            With tripl
                Dim fb As Integer = ArrFB(.x, .y, .z)
                Return (fb >> FB_INDEX_SHIFT) > 0
            End With
        End Function

        Public Function IsIndexQuadrant(x As Integer, y As Integer, z As Integer) As Boolean
            Return (ArrFB(x, y, z) >> FB_INDEX_SHIFT) > 0
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

            Dim fb As Integer = ArrFB(x, y, z)

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
            Dim fb As Integer = ArrFB(x, y, z)
            Dim offsetX As Integer = If((fb And FLAG_XOffset) <> 0, 1, 0)
            Dim offsetY As Integer = If((fb And FLAG_YOffset) <> 0, 1, 0)
            Return (ArrFB(x - offsetX, y - offsetY, z) And FLAG_ToggleFlag) <> 0
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
                Dim fb As Integer = ArrFB(.x, .y, .z)
                Dim offsetX As Integer = If((fb And FLAG_XOffset) <> 0, 1, 0)
                Dim offsetY As Integer = If((fb And FLAG_YOffset) <> 0, 1, 0)
                Return (ArrFB(.x - offsetX, .y - offsetY, .z) And FLAG_ToggleFlag) <> 0
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
                        Dim fb As Integer = ArrFB(x, y, z)
                        If fb <> 0 Then
                            'das erste gefundene belegte Feld auswerten
                            Dim offsetX As Integer = If((fb And FLAG_XOffset) <> 0, 1, 0)
                            Dim offsetY As Integer = If((fb And FLAG_YOffset) <> 0, 1, 0)
                            Return (ArrFB(x - offsetX, y - offsetY, z) And FLAG_ToggleFlag) <> 0
                        End If
                    Next
                Next
            Next
            Return False 'leeres Feld
        End Function

        Public Function GetIsRemoved(X As Integer, Y As Integer, Z As Integer) As Boolean
            'Weil zeitkritisch doppelter Code
            Dim fb As Integer = _ArrFB(X, Y, Z)
            Dim offsetX As Integer = If((fb And FLAG_XOffset) <> 0, 1, 0)
            Dim offsetY As Integer = If((fb And FLAG_YOffset) <> 0, 1, 0)
            Return (_ArrFB(X - offsetX, Y - offsetY, Z) And FLAG_IsRemoved) <> 0
        End Function

        Public Function GetIsRemoved(triple As Triple) As Boolean
            With triple
                'Weil zeitkritisch doppelter Code
                Dim fb As Integer = _ArrFB(.x, .y, .z)
                Dim offsetX As Integer = If((fb And FLAG_XOffset) <> 0, 1, 0)
                Dim offsetY As Integer = If((fb And FLAG_YOffset) <> 0, 1, 0)
                Return (_ArrFB(.x - offsetX, .y - offsetY, .z) And FLAG_IsRemoved) <> 0
            End With
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
            SetOffsetX(ArrFB(infoFBx + 1, infoFBy + 1, infoFBz), True)
            SetOffsetY(ArrFB(infoFBx + 1, infoFBy + 1, infoFBz), True)
            '
            'Der childFB genau unter dem infoFB zum infoFB geht mit OffsetX = 0 und OffsetY = -1
            SetOffsetX(ArrFB(infoFBx, infoFBy + 1, infoFBz), False)
            SetOffsetY(ArrFB(infoFBx, infoFBy + 1, infoFBz), True)
            '
            'Vom FB rechts vom infoFB zum infoFB: OffsetX = -1, OffsetY = 0
            SetOffsetX(ArrFB(infoFBx + 1, infoFBy, infoFBz), True)
            SetOffsetY(ArrFB(infoFBx + 1, infoFBy, infoFBz), False)

            'Der infoFB hat keinen Offset, daher beide 0 (Verweis auf sich selber)
            SetOffsetX(ArrFB(infoFBx, infoFBy, infoFBz), False)
            SetOffsetY(ArrFB(infoFBx, infoFBy, infoFBz), False)
            '
            SetSteinInfoIndex(ArrFB(infoFBx, infoFBy, infoFBz), SteinInfoIndex)

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

        Public Sub SetIsRemoved(x As Integer, y As Integer, z As Integer, value As Boolean)
            'Weil zeitkritisch doppelter Code
            Dim fb As Integer = ArrFB(x, y, z)
            Dim offsetX As Integer = If((fb And FLAG_XOffset) <> 0, 1, 0)
            Dim offsetY As Integer = If((fb And FLAG_YOffset) <> 0, 1, 0)
            If value Then
                ArrFB(x - offsetX, y - offsetY, z) = ArrFB(x - offsetX, y - offsetY, z) Or FLAG_IsRemoved
            Else
                ArrFB(x - offsetX, y - offsetY, z) = ArrFB(x - offsetX, y - offsetY, z) And Not FLAG_IsRemoved
            End If
        End Sub
        Public Sub SetIsRemoved(triple As Triple, value As Boolean)
            'Weil zeitkritisch doppelter Code
            With triple
                Dim fb As Integer = _ArrFB(.x, .y, .z)
                Dim offsetX As Integer = If((fb And FLAG_XOffset) <> 0, 1, 0)
                Dim offsetY As Integer = If((fb And FLAG_YOffset) <> 0, 1, 0)
                If value Then
                    _ArrFB(.x - offsetX, .y - offsetY, .z) = _ArrFB(.x - offsetX, .y - offsetY, .z) Or FLAG_IsRemoved
                Else
                    _ArrFB(.x - offsetX, .y - offsetY, .z) = _ArrFB(.x - offsetX, .y - offsetY, .z) And Not FLAG_IsRemoved
                End If
            End With
        End Sub

        '
        Public Sub SetToggleFlag(x As Integer, y As Integer, z As Integer, value As Boolean)
            'Weil zeitkritisch doppelter Code
            Dim fb As Integer = ArrFB(x, y, z)
            Dim offsetX As Integer = If((fb And FLAG_XOffset) <> 0, 1, 0)
            Dim offsetY As Integer = If((fb And FLAG_YOffset) <> 0, 1, 0)
            If value Then
                ArrFB(x - offsetX, y - offsetY, z) = ArrFB(x - offsetX, y - offsetY, z) Or FLAG_ToggleFlag
            Else
                ArrFB(x - offsetX, y - offsetY, z) = ArrFB(x - offsetX, y - offsetY, z) And Not FLAG_ToggleFlag
            End If
        End Sub
        '
        Public Sub SetToggleFlag(tripl As Triple, value As Boolean)
            'Weil zeitkritisch doppelter Code
            With tripl
                Dim fb As Integer = ArrFB(.x, .y, .z)
                Dim offsetX As Integer = If((fb And FLAG_XOffset) <> 0, 1, 0)
                Dim offsetY As Integer = If((fb And FLAG_YOffset) <> 0, 1, 0)
                If value Then
                    ArrFB(.x - offsetX, .y - offsetY, .z) = ArrFB(.x - offsetX, .y - offsetY, .z) Or FLAG_ToggleFlag
                Else
                    ArrFB(.x - offsetX, .y - offsetY, .z) = ArrFB(.x - offsetX, .y - offsetY, .z) And Not FLAG_ToggleFlag
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
                        If ArrFB(x, y, z) <> 0 Then
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

        Public Sub SetSteinInfoIndex(triple As Triple, value As Integer)
            With triple
                Dim flags As Integer = ArrFB(.x, .y, .z) And FB_FLAG_MASK
                'Der Index wird um 1 erhöht gespeichert, weil index = 0 gleichbedeutend
                'wäre mit "freier Platz".
                ArrFB(.x, .y, .z) = ((value + 1) << FB_INDEX_SHIFT) Or flags
            End With
        End Sub

        Public Shared Sub SetSteinInfoIndex(ByRef fb As Integer, value As Integer)
            Dim flags As Integer = fb And FB_FLAG_MASK
            'Der Index wird um 1 erhöht gespeichert, weil index = 0 gleichbedeutend
            'wäre mit "freier Platz".
            fb = ((value + 1) << FB_INDEX_SHIFT) Or flags
        End Sub

        '
        Public Sub ToggleToggleFlag(x As Integer, y As Integer, z As Integer)
            'Weil zeitkritisch doppelter Code
            Dim fb As Integer = ArrFB(x, y, z)
            Dim offsetX As Integer = If((fb And FLAG_XOffset) <> 0, 1, 0)
            Dim offsetY As Integer = If((fb And FLAG_YOffset) <> 0, 1, 0)
            ArrFB(x - offsetX, y - offsetY, z) = ArrFB(x - offsetX, y - offsetY, z) Xor FLAG_ToggleFlag
        End Sub
        '
        Public Sub ToggleToggleFlag(tripl As Triple)
            'Weil zeitkritisch doppelter Code
            With tripl
                Dim fb As Integer = ArrFB(.x, .y, .z)
                Dim offsetX As Integer = If((fb And FLAG_XOffset) <> 0, 1, 0)
                Dim offsetY As Integer = If((fb And FLAG_YOffset) <> 0, 1, 0)
                ArrFB(.x - offsetX, .y - offsetY, .z) = ArrFB(.x - offsetX, .y - offsetY, .z) Xor FLAG_ToggleFlag
            End With
        End Sub

        Public Shared Sub ToggleToggleFlag(ByRef fb As Integer)
            fb = fb Xor FLAG_ToggleFlag
        End Sub

        Private _tmpRemoveSteinAktive As Boolean
        Private _tmpFB(3) As Integer
        Private _tmpSteinInfoIndex As Integer = -1
        '
        ''' <summary>
        ''' Entfernt temporär einen Stein aus dem ArrFB
        ''' und setzt das Flag IsTmpRemoved im SteinInfo
        ''' </summary>
        ''' <param name="steinInfoIndex"></param>
        Public Sub TmpRealRemoveStein(steinInfoIndex As Integer)

            If _tmpRemoveSteinAktive Then
                Throw New Exception("TmpRealRemoveStein ist noch aktiv.")
            End If

            _tmpRemoveSteinAktive = True

            If steinInfoIndex < 0 Then
                _tmpSteinInfoIndex = -1
                Exit Sub
            End If
            If steinInfoIndex >= SteinInfos.Count Then
                _tmpSteinInfoIndex = -1
                Exit Sub
            End If

            _tmpSteinInfoIndex = steinInfoIndex
            _SteinInfos(steinInfoIndex).IsTmpRemoved = True

            With _SteinInfos(steinInfoIndex).Pos3D
                _tmpFB(0) = ArrFB(.x, .y, .z)
                _tmpFB(1) = ArrFB(.x + 1, .y, .z)
                _tmpFB(2) = ArrFB(.x, .y + 1, .z)
                _tmpFB(3) = ArrFB(.x + 1, .y + 1, .z)

                ArrFB(.x, .y, .z) = 0
                ArrFB(.x + 1, .y, .z) = 0
                ArrFB(.x, .y + 1, .z) = 0
                ArrFB(.x + 1, .y + 1, .z) = 0
            End With

        End Sub

        Public Sub TmpRealReturnRemovedStein()

            If Not _tmpRemoveSteinAktive Then
                Throw New Exception("TmpRealReturnRemovedStein aufgerufen ohne vorheriges TmpRealRemoveStein")
            End If
            '
            If _tmpSteinInfoIndex < 0 Then
                Exit Sub
            End If

            With _SteinInfos(_tmpSteinInfoIndex).Pos3D
                '
                'Die ToggleFlags syncronisieren
                Dim tglf As Boolean = GetToggleFlag(_SteinInfos(0).Pos3D)
                SetToggleFlag(_tmpFB(0), tglf)
                SetToggleFlag(_tmpFB(1), tglf)
                SetToggleFlag(_tmpFB(2), tglf)
                SetToggleFlag(_tmpFB(3), tglf)
                '
                'und zurück
                ArrFB(.x, .y, .z) = _tmpFB(0)
                ArrFB(.x + 1, .y, .z) = _tmpFB(1)
                ArrFB(.x, .y + 1, .z) = _tmpFB(2)
                ArrFB(.x + 1, .y + 1, .z) = _tmpFB(3)
            End With

            _SteinInfos(_tmpSteinInfoIndex).IsTmpRemoved = False

            _tmpRemoveSteinAktive = False

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

        Private Sub RebuildAllCaches()

            Update_SteinInfos_RectQuadranten()
            MarkTopSearchDirty()

        End Sub

#End Region

#Region "HitTest / Kandidaten / Mauslogik"

        Public Function GetTopSteinInfoIndexAtPoint(mousePos As Point) As Integer

            Dim tpl As Triple = GetTopSteinAtPoint(mousePos)

            If tpl.IsValideYes Then
                Return GetSteinInfoIndex(tpl)
            Else
                Return -1
            End If

        End Function

        Public Function GetTopSteinInfoIndexAtPointWithZ(mousePos As Point) As (idx As Integer, z As Integer)

            Dim tpl As Triple = GetTopSteinAtPoint(mousePos)

            If tpl.IsValideYes Then
                Dim rv As (idx As Integer, z As Integer)
                rv.idx = GetSteinInfoIndex(tpl)
                rv.idx = tpl.z + 1
                Return rv
            Else
                Return (-1, 0)
            End If

        End Function

        Public Function GetTopQuadrantAtPoint(mousePos As Point) As TripleX

            UpdateTopSearchIndex()

            For idx As Integer = 0 To _indexTopSearch.Length - 1

                Dim steinIndex As Integer = _indexTopSearch(idx)
                Dim stein As SteinInfo = _SteinInfos(steinIndex)

                If stein.IsRemoved Then
                    Continue For
                End If
                If stein.IsTmpRemoved Then
                    Continue For
                End If

                ' erster Treffer gewinnt
                ' If si.HitTest(pt) Then Return si
                Dim kandidat As TripleX = stein.IsTopQuadrant(mousePos)

                If kandidat.IsValideYes Then
                    Return kandidat
                End If

            Next

            Return New TripleX(ValidePlace.No)

        End Function

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
        Public Function GetTopSteinAtPoint(mousePos As Point) As TripleX

            UpdateTopSearchIndex()

            Dim kandidat As New TripleX(ValidePlace.NoKandidat) 'Default
            Dim retval As TripleX = Nothing

            For idx As Integer = 0 To _indexTopSearch.Length - 1

                Dim steinIndex As Integer = _indexTopSearch(idx)
                Dim stein As SteinInfo = _SteinInfos(steinIndex)

                If stein.IsRemoved Then
                    Continue For
                End If
                If stein.IsTmpRemoved Then
                    Continue For
                End If

                ' erster Treffer gewinnt
                ' If si.HitTest(pt) Then Return si
                kandidat = stein.IsTopQuadrant(mousePos)

                Dim okCounter As Integer

                With kandidat
                    If .IsValideYes Then
                        okCounter = 1
                        Select Case .Quadrant
                            Case Quadrant.LO
                                retval = kandidat
                                If stein.IsTopQuadrant(.x + 1, .y, .z) Then
                                    okCounter += 1
                                End If
                                If stein.IsTopQuadrant(.x, .y + 1, .z) Then
                                    okCounter += 1
                                End If
                                If stein.IsTopQuadrant(.x + 1, .y + 1, .z) Then
                                    okCounter += 1
                                End If

                            Case Quadrant.RO
                                retval = kandidat.DeepCopy(addX:=-1, addY:=0, addZ:=0)

                                If stein.IsTopQuadrant(.x - 1, .y, .z) Then
                                    okCounter += 1
                                End If
                                If stein.IsTopQuadrant(.x - 1, .y + 1, .z) Then
                                    okCounter += 1
                                End If
                                If stein.IsTopQuadrant(.x, .y + 1, .z) Then
                                    okCounter += 1
                                End If

                            Case Quadrant.LU
                                retval = kandidat.DeepCopy(addX:=0, addY:=-1, addZ:=0)
                                If stein.IsTopQuadrant(.x, .y - 1, .z) Then
                                    okCounter += 1
                                End If
                                If stein.IsTopQuadrant(.x + 1, .y - 1, .z) Then
                                    okCounter += 1
                                End If
                                If stein.IsTopQuadrant(.x + 1, .y, .z) Then
                                    okCounter += 1
                                End If

                            Case Quadrant.RU
                                retval = kandidat.DeepCopy(addX:=0, addY:=-1, addZ:=0)
                                If stein.IsTopQuadrant(.x - 1, .y - 1, .z) Then
                                    okCounter += 1
                                End If
                                If stein.IsTopQuadrant(.x, .y - 1, .z) Then
                                    okCounter += 1
                                End If
                                If stein.IsTopQuadrant(.x - 1, .y, .z) Then
                                    okCounter += 1
                                End If

                        End Select
                    End If
                End With

                If okCounter = 4 Then
                    Return retval
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
        ''' <param name="mousePos">Die aktuelle Mausposition</param>
        ''' <returns></returns>
        Public Function IsFundamentKandidat(mousePos As Point) As Triple

            UpdateTopSearchIndex()

            'skipSteinFromRect
            'Es geht um den Stein, der angeklickt wurde.
            'Das Problem ist folgendes: eigentlich müsste es heißen: skipSteinInfoIndex
            'Der steinInfoIndex ist auch bekannt

            Dim kandidat As New Triple(ValidePlace.NoKandidat) 'Default

            For Each stein As SteinInfo In SteinInfos

                If stein.IsRemoved Then
                    Continue For
                End If
                If stein.IsTmpRemoved Then
                    Continue For
                End If

                kandidat = stein.IsFundamentQuadrantKandidat(mousePos)

                If kandidat.IsValideYes Then
                    Exit For
                End If
            Next

            If kandidat.IsValideYes Then

                'Steht der kandidat auf dem rechten oder unteren Rand?
                'Dann sind nach rechts bzw. nach unten keine Felder frei.
                If Not kandidat.IsInsideSpielfeldBounds(ArrFB, excludeRightAndBottomMargin:=True) Then
                    Return New Triple(ValidePlace.NoKandidat)
                End If

                'kandidat ist der linke obere Quadrant des Steines.
                'Jetzt prüfen, ob die Plätze für anderen drei Quadranten frei sind
                'und ob darunter ein Fundament ist.

                Dim isBottom As Boolean = kandidat.z = 0
                Dim okCounter As Integer = 1

                With kandidat
                    'Quadrant rechts oben frei?
                    If ArrFB(.x + 1, .y, .z) = 0 Then
                        If isBottom Then 'kandidat steht auf der Grundfläche
                            okCounter += 1
                        Else
                            'Der Platz darunter belegt?
                            If ArrFB(.x + 1, .y, .z - 1) <> 0 Then
                                okCounter += 1
                            End If
                        End If
                    End If
                    '
                    'Quadrant unten drunter frei?
                    If ArrFB(.x, .y + 1, .z) = 0 Then
                        If isBottom Then 'kandidat steht auf der Grundfläche
                            okCounter += 1
                        Else
                            'Der Platz darunter belegt?
                            If ArrFB(.x, .y + 1, .z - 1) <> 0 Then
                                okCounter += 1
                            End If
                        End If
                    End If
                    '
                    'Quadrant rechts unten frei?
                    If ArrFB(.x + 1, .y + 1, .z) = 0 Then
                        If isBottom Then 'kandidat steht auf der Grundfläche
                            okCounter += 1
                        Else
                            'Der Platz darunter belegt?
                            If ArrFB(.x + 1, .y + 1, .z - 1) <> 0 Then
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
            Else 'kandidat.IsNotValideYes 
                'Wenn kein Stein gefunden wurde, die Grundfläche überprüfen.
                kandidat = GetPos3DOnGround(mousePos)
                If Not kandidat.IsInsideSpielfeldBounds(ArrFB, excludeRightAndBottomMargin:=True) Then
                    'Exclude...=True, weil kandidat den linken oberen Quadrant darstellt und auf der
                    'rechten Spalte und der unteren Zeile kein Platz für die anderen Quadranten ist.
                    Return New Triple(ValidePlace.NoKandidat)
                Else
                    'Prüfen, ob alle vier Plätze frei sind.
                    With kandidat
                        If ArrFB(.x, .y, .z) = 0 AndAlso
                            ArrFB(.x + 1, .y, .z) = 0 AndAlso
                            ArrFB(.x, .y + 1, .z) = 0 AndAlso
                            ArrFB(.x + 1, .y + 1, .z) = 0 Then
                            Return kandidat
                        Else
                            Return New Triple(ValidePlace.NoKandidat)
                        End If
                    End With
                End If
            End If
        End Function
        '
        Public Function GetSteinToolTipInfos(mousePos As Point) As TripleX()

            Dim ssi As New List(Of TripleX)

            Dim Kandidat As TripleX = GetTopQuadrantAtPoint(mousePos)

            If Kandidat.IsValideYes Then
                For idxZ As Integer = Kandidat.z To 0 Step -1
                    Dim idx As Integer = GetSteinInfoIndex(New TripleX(Kandidat.x, Kandidat.y, idxZ))

                    Dim aktSIE As TripleX
                    If idx = -1 Then
                        'Dann liegen Steine frei
                        aktSIE = New TripleX
                    Else
                        With SteinInfos(idx)
                            aktSIE = New TripleX(.X, .Y, idxZ, ValidePlace.Yes, .SteinInfoIndex, .SteinSymbolIndex, GetQuadrant(Kandidat.x, Kandidat.y, idxZ), SteinInfos(idx))
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

        ''' <summary>
        ''' Gibt den Mittelpunkt der Feldraster an.
        ''' </summary>
        ''' <param name="X">X-Wert des aktuellen arrFB-Steines</param>
        ''' <param name="Y">Y-Wert des aktuellen arrFB-Steines</param>
        ''' <returns></returns>
        Public Function GetFeldRasterCenter(X As Integer, Y As Integer) As Point
            With _sfd.SFLay
                Dim rasterX As Integer = .steinWidthHalf * (X - 1) + .steinWidthHalf
                Dim rasterY As Integer = .steinHeightHalf * (Y - 1) + .steinHeightHalf

                Return New Point(.rxStageUsed.Left + rasterX, .rxStageUsed.Top + rasterY)
            End With
        End Function

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
            Dim lay As SFLayout = _sfd.SFLay

            Dim rasterX As Integer = lay.steinWidthHalf * (X - 1)
            Dim rasterY As Integer = lay.steinHeightHalf * (Y - 1)

            Dim left As Integer = lay.rxStageUsed.Left + rasterX + lay.offset3DLeftSumme - (lay.offset3DLeftJeEbene * Z)
            Dim top As Integer = lay.rxStageUsed.Top + rasterY + lay.offset3DTopSumme - (lay.offset3DTopJeEbene * Z)

            Return New Rectangle(left, top, lay.steinWidth, lay.steinHeight)
        End Function

        Public Function GetSteinRenderRect(tripl As Triple) As Rectangle

            With tripl
                Dim lay As SFLayout = _sfd.SFLay

                Dim rasterX As Integer = lay.steinWidthHalf * (.x - 1)
                Dim rasterY As Integer = lay.steinHeightHalf * (.y - 1)

                Dim left As Integer = lay.rxStageUsed.Left + rasterX + lay.offset3DLeftSumme - (lay.offset3DLeftJeEbene * .z)
                Dim top As Integer = lay.rxStageUsed.Top + rasterY + lay.offset3DTopSumme - (lay.offset3DTopJeEbene * .z)

                Return New Rectangle(left, top, lay.steinWidth, lay.steinHeight)
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

#Region "TopSteinInfo"

        Private _arrTopSteinInfo() As TopSteinInfo = Nothing
        Private _arrTopSteinInfoIdxMaxInUse As Integer = -1

        'Enthält nur Indizes auf _arrTopSteinInfo,
        'nach SteinClickGruppe sortiert.
        Private _arrTopSteinInfoLookupByClickGruppe() As Integer = Nothing

        Private _topSteinInfoIsDirty As Boolean = True
        Private _lastWindsAreInOneClickGroup As Boolean
        Private _lastAktRenderMode As AktRenderMode
        Private _foundMissingSecond As Boolean
        '
        ''' <summary>
        ''' Gibt an, ob sich auf dem Spielfeld Steine gleicher SteinKlickGruppe
        ''' in ungerader Anzahl befinden. Wenn ja, ist das Spiel nicht spielbar.
        ''' Steht nur zur Verfügung, wenn:
        ''' INI.Spielbetrieb_ShowRemovableStones = True oder _sfd.SFRun.AktRenderMode =
        ''' AktRenderMode.Edit (Siehe SFInfo.UpdateTopSteinInfos)
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property FoundMissingSecond As Boolean
            Get
                Return _foundMissingSecond
            End Get
        End Property

        ''' <summary>
        ''' Prüft, ob der Stein an x/y/z in der aktuellen TopSteinInfo-Liste enthalten ist.
        ''' Voraussetzung: UpdateTopSteinInfos wurde zuvor aufgerufen.
        ''' </summary>
        Public Function IsTopStein(x As Integer, y As Integer, z As Integer) As Boolean
            Return ContainsTopStein(x, y, z)
        End Function

        '
        ''' <summary>
        ''' Liefert den TopSteinInfo-Eintrag zu x/y/z per binärer Suche.
        ''' Voraussetzung: UpdateTopSteinInfos wurde zuvor aufgerufen.
        ''' Gibt Nothing zurück, wenn kein Eintrag gefunden wurde.
        ''' </summary>
        Public Function GetTopSteinInfo(x As Integer, y As Integer, z As Integer) As TopSteinInfo

            If _arrTopSteinInfoIdxMaxInUse < 0 Then
                Return Nothing
            End If

            Dim low As Integer = 0
            Dim high As Integer = _arrTopSteinInfoIdxMaxInUse

            Do While low <= high

                Dim mid As Integer = low + ((high - low) \ 2)
                Dim item As TopSteinInfo = _arrTopSteinInfo(mid)

                Dim cmp As Integer = CompareTopSteinInfoToXYZ(item, x, y, z)

                If cmp = 0 Then
                    Return item
                End If

                If cmp < 0 Then
                    low = mid + 1
                Else
                    high = mid - 1
                End If

            Loop

            Return Nothing

        End Function
        Public Sub MakeDirtyTopSteinInfos()
            _topSteinInfoIsDirty = True
        End Sub '
        '
        ''' <summary>
        ''' Ruft UpdateTopSteinInfos auf, wenn zuvor MakeDirtyTopSteinInfos aufgerufen wurde oder sich
        ''' INI.Spielbetrieb_WindsAreInOneClickGroup und andere Bedingungen geändert haben. (siehe Code) Aufruf im
        ''' Normalfall am besten in SFRenderManager unmittelbar vor _sfd.SFAir.ConsumeAirplanePolling(), damit dort auf das Ergebnis
        ''' zugegriffen werden kann. Gibt True zurück, wenn UpdateTopSteinInfos aufgerufen wurde
        ''' </summary>
        Public Function ConsumeTopSteinInfosPolling() As Boolean

            If _lastWindsAreInOneClickGroup <> INI.Spielbetrieb_WindsAreInOneClickGroup Then
                _topSteinInfoIsDirty = True
                _lastWindsAreInOneClickGroup = INI.Spielbetrieb_WindsAreInOneClickGroup
            End If

            If _lastAktRenderMode <> _sfd.SFRun.AktRenderMode Then
                _topSteinInfoIsDirty = True
                _lastAktRenderMode = _sfd.SFRun.AktRenderMode
            End If

            If _topSteinInfoIsDirty Then
                _topSteinInfoIsDirty = False
                UpdateTopSteinInfos()
                Return True
            Else
                Return False
            End If

        End Function
        '
        ''' <summary>
        ''' Erzeugt/aktualisiert eine aktuelle Liste der vollständig sichtbaren Steine und deren Eigenschaften.
        ''' Aufruf nur dann nötig, wenn sich der Steinbestand oder eine der Regeln geändert hat.
        ''' (Siehe im Code von SFInfo.ConsumeTopSteinInfosPolling. Aufruf im Normalfall auch von dort.)
        ''' </summary>
        Public Sub UpdateTopSteinInfos()

            Dim showSelectableSteine As Boolean
            Dim showRemovableSteine As Boolean
            Dim renderModeEdit As Boolean = _sfd.SFRun.AktRenderMode = AktRenderMode.Edit

            If renderModeEdit Then
                showSelectableSteine = True
                showRemovableSteine = True
            Else 'AktRenderMode.Spiel (oder None, das kommt hier aber nicht an.)
                showSelectableSteine = INI.Spielbetrieb_ShowSelectableStones
                showRemovableSteine = INI.Spielbetrieb_ShowRemovableStones
            End If

            _foundMissingSecond = False

            'Die maximal mögliche Größe des Arrays festlegen.
            'Es sind maximal xMaxSteine * yMaxSteine vollständig sichtbar.
            Dim uBnd As Integer = xMaxSteine * yMaxSteine

            If _arrTopSteinInfo Is Nothing OrElse _arrTopSteinInfo.GetUpperBound(0) < uBnd Then
                ReDim _arrTopSteinInfo(uBnd)
            End If

            If _arrTopSteinInfoLookupByClickGruppe Is Nothing OrElse _arrTopSteinInfoLookupByClickGruppe.GetUpperBound(0) < uBnd Then
                ReDim _arrTopSteinInfoLookupByClickGruppe(uBnd)
            End If

            'Der Array wird nicht gelöscht, sondern nur logisch zurückgesetzt.
            _arrTopSteinInfoIdxMaxInUse = -1

            Dim z As Integer

            For x As Integer = xMin To xMax
                For y As Integer = yMin To yMax

                    If ArrFB(x, y, 0) = 0 Then
                        Continue For
                    End If

                    If GetIsRemoved(x, y, 0) Then
                        Continue For
                    End If

                    '
                    'Nicht nötig, ist ja aus dem arrFB entfernt.
                    ''If renderModeEdit Then
                    ''    If _SteinInfos(GetSteinInfoIndex(x, y, z)).IsTmpRemoved Then
                    ''        Continue For
                    ''    End If
                    ''End If

                    'Den obersten Stein suchen
                    If ArrFB(x, y, zMax) <> 0 Then
                        'Die oberste Ebene ist mit einem Stein belegt
                        z = zMax
                    Else
                        'Von unten nach oben den letzten Stein suchen
                        z = zMax
                        For idx As Integer = 1 To zMax '1 ist richtig, 0 bereits weiter oben aussortiert.
                            If ArrFB(x, y, idx) = 0 Then
                                z = idx - 1
                                Exit For
                            End If
                        Next
                    End If

                    If Not IsIndexQuadrant(x, y, z) Then
                        'Das ist nicht der linke obere Quadrant eines Steines
                        Continue For
                    End If

                    Dim steinIsKplVisible As Boolean

                    If z = zMax Then
                        steinIsKplVisible = True
                    Else
                        'Den gefundenen linken oberen Quadranten untersuchen
                        Dim okCounter As Integer = 1 'Ein oberster Quadrant ist gefunden.

                        'Sind auf den anderen drei Quadranten die Felder drüber frei?
                        If ArrFB(x + 1, y, z + 1) = 0 Then okCounter += 1
                        If ArrFB(x, y + 1, z + 1) = 0 Then okCounter += 1
                        If ArrFB(x + 1, y + 1, z + 1) = 0 Then okCounter += 1

                        steinIsKplVisible = (okCounter = 4)
                    End If

                    If steinIsKplVisible Then

                        Dim hasFreeSide As Integer = 0

                        'Linke Seite prüfen
                        If ArrFB(x - 1, y, z) = 0 AndAlso ArrFB(x - 1, y + 1, z) = 0 Then
                            hasFreeSide += 1
                        End If

                        'Rechte Seite prüfen
                        If ArrFB(x + 2, y, z) = 0 AndAlso ArrFB(x + 2, y + 1, z) = 0 Then
                            hasFreeSide += 2
                        End If

                        Dim tsi As New TopSteinInfo(x, y, z)

                        With tsi
                            .FreeSide = CType(hasFreeSide, FreeSide)
                            '    .RenderRect = GetSteinRenderRect(x, y, z)
                            .SteinInfoIndex = GetSteinInfoIndex(x, y, z)
                            .SteinInfo = _SteinInfos(.SteinInfoIndex)
                            .SteinSymbolIndex = .SteinInfo.SteinSymbolIndex
                            .SteinClickGruppe = GetSteinClickGruppe(.SteinSymbolIndex)

                            If showSelectableSteine OrElse showRemovableSteine Then
                                If (hasFreeSide And 3) <> 0 Then
                                    'Wenn showRemovableSteine, werden die I03Selectable
                                    'ggf in UpdateTopSteinInfoPairStatus hochgestuft.
                                    'Wenn Not showSelectableSteine, werden die übriggebliebenen
                                    'I03Selectable wieder runtergestuft in I01Normal
                                    .SteinStatus = SteinStatus.I03Selectable 'Default
                                Else
                                    .SteinStatus = SteinStatus.I01Normal
                                End If
                            Else
                                .SteinStatus = SteinStatus.I01Normal
                            End If

                            .Valide = ValidePlace.Yes
                        End With

                        _arrTopSteinInfoIdxMaxInUse += 1
                        _arrTopSteinInfo(_arrTopSteinInfoIdxMaxInUse) = tsi

                    End If

                Next
            Next

            If _arrTopSteinInfoIdxMaxInUse < 0 Then
                Return
            End If

            'Nur den tatsächlich benutzten Bereich sortieren.
            If _arrTopSteinInfoIdxMaxInUse > 0 Then
                Array.Sort(Of TopSteinInfo)(_arrTopSteinInfo,
                                            0,
                                            _arrTopSteinInfoIdxMaxInUse + 1,
                                             Comparer(Of TopSteinInfo).Create(AddressOf CompareTopSteinInfoXYZ))
            End If

            'Lookup-Array mit den Indizes auf _arrTopSteinInfo füllen
            For i As Integer = 0 To _arrTopSteinInfoIdxMaxInUse
                _arrTopSteinInfoLookupByClickGruppe(i) = i
            Next

            'Lookup nach SteinClickGruppe sortieren
            If _arrTopSteinInfoIdxMaxInUse > 0 Then
                Array.Sort(Of Integer)(_arrTopSteinInfoLookupByClickGruppe,
                                        0,
                                        _arrTopSteinInfoIdxMaxInUse + 1,
                                        Comparer(Of Integer).Create(AddressOf CompareTopSteinInfoLookupByClickGruppe))
            End If

            'Nun feststellen, welche anklickbaren Steine auch einen Partner haben.

            If showRemovableSteine Then
                UpdateTopSteinInfoPairStatus(onlyCount:=False)
                If Not showSelectableSteine Then
                    'die übriggebliebenen I03Selectable runterstufen
                    For idx As Integer = 0 To _arrTopSteinInfoIdxMaxInUse
                        If _arrTopSteinInfo(idx).SteinStatus = SteinStatus.I03Selectable Then
                            _arrTopSteinInfo(idx).SteinStatus = SteinStatus.I01Normal
                        End If
                    Next
                End If
            Else
                UpdateTopSteinInfoPairStatus(onlyCount:=True)
            End If

        End Sub

        '
        '
        ''' <summary>
        ''' Setzt bei allen I03Selectable-Steinen den Status auf I04Removable,
        ''' wenn innerhalb ihrer SteinClickGruppe mindestens zwei selektierbare
        ''' Topsteine vorhanden sind.
        ''' Voraussetzung: _arrTopSteinInfoLookupByClickGruppe ist sortiert.
        ''' </summary>
        Private Sub UpdateTopSteinInfoPairStatus(onlyCount As Boolean)

            Dim runStart As Integer = 0

            'Diese beiden Werte fallen hier als Abfallprodukt an:
            _aktSelectableCount = 0
            _aktRemovableCount = 0

            Do While runStart <= _arrTopSteinInfoIdxMaxInUse

                Dim firstTopIndex As Integer = _arrTopSteinInfoLookupByClickGruppe(runStart)
                Dim clickGruppe As Integer = _arrTopSteinInfo(firstTopIndex).SteinClickGruppe

                Dim runEnd As Integer = runStart + 1
                Do While runEnd <= _arrTopSteinInfoIdxMaxInUse

                    Dim nextTopIndex As Integer = _arrTopSteinInfoLookupByClickGruppe(runEnd)

                    If _arrTopSteinInfo(nextTopIndex).SteinClickGruppe <> clickGruppe Then
                        Exit Do
                    End If

                    runEnd += 1
                Loop

                Dim selectableCount As Integer = 0

                For idx As Integer = runStart To runEnd - 1
                    Dim tsiIndex As Integer = _arrTopSteinInfoLookupByClickGruppe(idx)
                    If _arrTopSteinInfo(tsiIndex).SteinStatus = SteinStatus.I03Selectable Then
                        selectableCount += 1
                    End If
                Next

                Dim removableCount As Integer = 0

                If selectableCount >= 2 Then
                    For idx As Integer = runStart To runEnd - 1
                        Dim tsiIndex As Integer = _arrTopSteinInfoLookupByClickGruppe(idx)
                        Dim tsi As TopSteinInfo = _arrTopSteinInfo(tsiIndex)

                        If tsi.SteinStatus = SteinStatus.I03Selectable Then
                            If Not onlyCount Then
                                tsi.SteinStatus = SteinStatus.I04Removable
                            End If
                            selectableCount -= 1
                            removableCount += 1
                        End If
                    Next
                End If

                runStart = runEnd

                _aktSelectableCount += selectableCount
                _aktRemovableCount += removableCount

            Loop

        End Sub

        '
        ''' <summary>
        ''' Prüft per binärer Suche, ob das sortierte Array den TopSteinInfo-Eintrag enthält.
        ''' Voraussetzung: UpdateTopSteinInfos wurde zuvor aufgerufen.
        ''' </summary>
        Private Function ContainsTopStein(x As Integer,
                                  y As Integer,
                                  z As Integer) As Boolean

            If _arrTopSteinInfoIdxMaxInUse < 0 Then
                Return False
            End If

            Dim low As Integer = 0
            Dim high As Integer = _arrTopSteinInfoIdxMaxInUse

            Do While low <= high

                Dim mid As Integer = low + ((high - low) \ 2)
                Dim item As TopSteinInfo = _arrTopSteinInfo(mid)

                Dim cmp As Integer = CompareTopSteinInfoToXYZ(item, x, y, z)

                If cmp = 0 Then
                    Return True
                End If

                If cmp < 0 Then
                    low = mid + 1
                Else
                    high = mid - 1
                End If

            Loop

            Return False

        End Function

        '
        ''' <summary>
        ''' Vergleich zweier TopSteinInfo-Objekte: zuerst X, dann Y, dann Z.
        ''' </summary>
        Private Function CompareTopSteinInfoXYZ(a As TopSteinInfo, b As TopSteinInfo) As Integer

            If a Is b Then
                Return 0
            End If

            If a Is Nothing Then
                Return -1
            End If

            If b Is Nothing Then
                Return 1
            End If

            If a.x < b.x Then Return -1
            If a.x > b.x Then Return 1

            If a.y < b.y Then Return -1
            If a.y > b.y Then Return 1

            If a.z < b.z Then Return -1
            If a.z > b.z Then Return 1

            Return 0

        End Function

        '
        ''' <summary>
        ''' Vergleich eines TopSteinInfo-Eintrags mit X/Y/Z.
        ''' </summary>
        Private Function CompareTopSteinInfoToXYZ(item As TopSteinInfo,
                                          x As Integer,
                                          y As Integer,
                                          z As Integer) As Integer

            If item Is Nothing Then
                Return -1
            End If

            If item.x < x Then Return -1
            If item.x > x Then Return 1

            If item.y < y Then Return -1
            If item.y > y Then Return 1

            If item.z < z Then Return -1
            If item.z > z Then Return 1

            Return 0

        End Function

        '
        ''' <summary>
        ''' Vergleicht zwei Indizes des Lookup-Arrays anhand der SteinClickGruppe.
        ''' Sekundärsortierung nach X/Y/Z nur für definierte Ordnung.
        ''' </summary>
        Private Function CompareTopSteinInfoLookupByClickGruppe(indexA As Integer,
                                                        indexB As Integer) As Integer

            Dim a As TopSteinInfo = _arrTopSteinInfo(indexA)
            Dim b As TopSteinInfo = _arrTopSteinInfo(indexB)

            If a Is b Then
                Return 0
            End If

            If a Is Nothing Then
                Return -1
            End If

            If b Is Nothing Then
                Return 1
            End If

            If a.SteinClickGruppe < b.SteinClickGruppe Then Return -1
            If a.SteinClickGruppe > b.SteinClickGruppe Then Return 1

            If a.SteinInfoIndex < b.SteinInfoIndex Then Return -1
            If a.SteinInfoIndex > b.SteinInfoIndex Then Return 1

            'wird nicht erreicht, da jeder SteininfoIndex nur einmal vorkommt.
            If a.x < b.x Then Return -1
            If a.x > b.x Then Return 1

            If a.y < b.y Then Return -1
            If a.y > b.y Then Return 1

            If a.z < b.z Then Return -1
            If a.z > b.z Then Return 1

            Return 0

        End Function

        '
        ''' <summary>
        ''' Prüft den gesamten Steinbestand darauf, ob jede SteinClickGruppe
        ''' in gerader Anzahl vorhanden ist.
        ''' True = es fehlt mindestens ein Partnerstein.
        ''' </summary>
        Private Sub UpdateFoundMissingSecond()

            Dim maxClickGruppe As Integer = [Enum].GetValues(GetType(SteinSymbol)).Length - 1
            Dim groupCount(maxClickGruppe) As Integer

            For x As Integer = xMin To xMax
                For y As Integer = yMin To yMax
                    For z As Integer = zMin To zMax

                        If ArrFB(x, y, z) = 0 Then
                            Continue For
                        End If

                        ''Hier kein Removed, sondern das gesamte Spielfeld betrachten
                        ''If GetIsRemoved(x, y, z) Then
                        ''    Continue For
                        ''End If

                        If Not IsIndexQuadrant(x, y, z) Then
                            Continue For
                        End If

                        Dim steinInfoIndex As Integer = GetSteinInfoIndex(x, y, z)
                        Dim clickGruppe As Integer = _SteinInfos(steinInfoIndex).KlickGruppe

                        groupCount(clickGruppe) += 1

                    Next
                Next
            Next

            _foundMissingSecond = False

            For i As Integer = 0 To groupCount.GetUpperBound(0)
                If (groupCount(i) And 1) <> 0 Then
                    'ungerade Zahl
                    _foundMissingSecond = True
                    Exit For
                End If
            Next

        End Sub

#End Region

#Region "Lookup-Tabelle"

        Private Sub TestEnumStein()

            'Wenn die Enumeration "Stein" geändert wird, muß die Lockuptabelle GruppeLookup
            'angepasst werden.
            'Diese Prüfung erinnert daran :-)
            Dim enumLength As Integer = [Enum].GetValues(GetType(SteinSymbol)).Length

            If GruppeLookupNormal.Length <> enumLength Then
                Throw New InvalidOperationException(
            $"Programmierfehler! GruppeLookupNormal hat {GruppeLookupNormal.Length} Einträge, Enum SteinSymbol hat {enumLength}. Tabelle anpassen!")
            End If

            If GruppeLookupVereinfacht.Length <> enumLength Then
                Throw New InvalidOperationException(
            $"Programmierfehler! GruppeLookupVereinfacht hat {GruppeLookupVereinfacht.Length} Einträge, Enum SteinSymbol hat {enumLength}. Tabelle anpassen!")
            End If
        End Sub

        ' Lookup-Tabelle.
        ' Muss aus der Reihenfolge der Enum-Deklarationen SteinSymbol
        ' und SteinClickGruppe gebaut sein!
        ' Hintergrund: Die Regel "Steine mit gleichem Symbol können paarweise
        ' entfernt werden" gilt meistens, aber nicht immer.
        ' Die Blüten und Jahressymbole sind optisch unterschiedlich, können
        ' aber paarweise entfernt werden. Deshalb gibt es die SteinClickGruppe
        ' und das hier ist die Übersetzungstabellevom SteinSymbol zur
        ' SteinClickGruppe
        ' Der Wert des Index selber ist egal, wichtig ist, daß Steine einer
        ' Klickgruppe den gleichen Wert haben.

        Private Shared ReadOnly GruppeLookupNormal As Integer() = {
        0,  'Dummy
        1,  'Punkt1
        2,  'Punkt2
        3,  'Punkt3
        4,  'Punkt4
        5,  'Punkt5
        6,  'Punkt6
        7,  'Punkt7
        8,  'Punkt8
        9,  'Punkt9
        10, 'Bambus1
        11, 'Bambus2
        12, 'Bambus3
        13, 'Bambus4
        14, 'Bambus5
        15, 'Bambus6
        16, 'Bambus7
        17, 'Bambus8
        18, 'Bambus9
        19, 'Symbol1
        20, 'Symbol2
        21, 'Symbol3
        22, 'Symbol4
        23, 'Symbol5
        24, 'Symbol6
        25, 'Symbol7
        26, 'Symbol8
        27, 'Symbol9
        28, 'DrachenRot
        29, 'DrachenGrün
        30, 'DrachenWeiß
        31, 'WindOst
        32, 'WindSüd
        33, 'WindWest
        34, 'WindNord
        35, 'BlütePflaume
        35, 'BlüteOrchidee
        35, 'BlüteChrisantheme
        35, 'BlüteBambus
        36, 'JahrFrühling
        36, 'JahrSommer
        36, 'JahrHerbst
        36  'JahrWinter
    }

        Private Shared ReadOnly GruppeLookupVereinfacht As Integer() = {
        0,  'Dummy
        1,  'Punkt1
        2,  'Punkt2
        3,  'Punkt3
        4,  'Punkt4
        5,  'Punkt5
        6,  'Punkt6
        7,  'Punkt7
        8,  'Punkt8
        9,  'Punkt9
        10, 'Bambus1
        11, 'Bambus2
        12, 'Bambus3
        13, 'Bambus4
        14, 'Bambus5
        15, 'Bambus6
        16, 'Bambus7
        17, 'Bambus8
        18, 'Bambus9
        19, 'Symbol1
        20, 'Symbol2
        21, 'Symbol3
        22, 'Symbol4
        23, 'Symbol5
        24, 'Symbol6
        25, 'Symbol7
        26, 'Symbol8
        27, 'Symbol9
        28, 'DrachenRot
        29, 'DrachenGrün
        30, 'DrachenWeiß
        31, 'WindOst
        31, 'WindSüd
        31, 'WindWest
        31, 'WindNord
        35, 'BlütePflaume
        35, 'BlüteOrchidee
        35, 'BlüteChrisantheme
        35, 'BlüteBambus
        36, 'JahrFrühling
        36, 'JahrSommer
        36, 'JahrHerbst
        36  'JahrWinter
    }

        ''' <summary>
        ''' Gibt die SteinklickGruppe aus dem SteinSymbolIndex zurück. 
        ''' Greift auf INI.Spielbetrieb_WindsAreInOneClickGroup zu.
        ''' </summary>
        ''' <param name="index"></param>
        ''' <returns></returns>
        Public Shared Function GetSteinClickGruppe(index As SteinSymbol) As Integer
            If INI.Spielbetrieb_WindsAreInOneClickGroup Then
                Return GruppeLookupVereinfacht(index)
            Else
                Return GruppeLookupNormal(index)
            End If
        End Function
        '
        ''' <summary>
        ''' Gibt die SteinklickGruppe aus dem SteinSymbolIndex zurück unter Auswertung von windsInOneClickGroup.
        ''' </summary>
        ''' <param name="index"></param>
        ''' <param name="windsInOneClickGroup"></param>
        ''' <returns></returns>
        Public Shared Function GetSteinClickGruppe(index As SteinSymbol, windsInOneClickGroup As Boolean) As Integer
            If windsInOneClickGroup Then
                Return GruppeLookupVereinfacht(index)
            Else
                Return GruppeLookupNormal(index)
            End If
        End Function

#End Region

#Region "Statistik"

        Public Sub ShowMessageBoxStatistik()
            Dim steineVorrat As New List(Of SteinSymbol)
            Dim steineSpielfeld As New List(Of SteinSymbol)

            If SteinInfos IsNot Nothing AndAlso SteinInfos.Count > 0 Then
                'Steine einsammeln
                For Each item As SteinInfo In SteinInfos
                    steineSpielfeld.Add(item.SteinSymbolIndex)
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
                    If ArrFB(0, y, z) <> 0 Then
                        Throw New Exception($"Programmierfehler: Linker Rand ist nicht 0 in x=0 y={y} z={z}")
                    End If
                Next

                ' Rechter Rand (x = xMax+1)
                For y As Integer = 0 To yMax + 1
                    If ArrFB(xMax + 1, y, z) <> 0 Then
                        Throw New Exception($"Programmierfehler: Rechter Rand ist nicht 0 in x={xMax + 1} y={y} z={z}")
                    End If
                Next

                ' Oberer Rand (y = 0)
                For x As Integer = 0 To xMax + 1
                    If ArrFB(x, 0, z) <> 0 Then
                        Throw New Exception($"Programmierfehler: Oberer Rand ist nicht 0 in x={x} y=0 z={z}")
                    End If
                Next

                ' Unterer Rand (y = yMax+1)
                For x As Integer = 0 To xMax + 1
                    If ArrFB(x, yMax + 1, z) <> 0 Then
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

#End Region

#Region "RedimArrFB"

        '
        ''' <summary>
        ''' Prüft, ob arrFB auf die angegebene Zielgröße gebracht werden kann.
        ''' Diese Überladung verändert nur die UBounds:
        ''' X nur rechts, Y nur unten, Z nur oben.
        ''' </summary>
        Public Function RedimArrFBIsPossible(ByVal arrFB(,,) As Integer,
                                         ByVal sizeInSteinen As Triple,
                                         ByVal backdorIsOpen As Boolean) As Boolean

            If Not IsTargetSizeValid(sizeInSteinen, backdorIsOpen) Then
                Return False
            End If

            If arrFB Is Nothing Then
                Return False
            End If

            Dim xUbndNew As Integer = (sizeInSteinen.x * 2) + 1
            Dim yUbndNew As Integer = (sizeInSteinen.y * 2) + 1
            Dim zUbndNew As Integer = sizeInSteinen.z

            Dim xUbndOld As Integer = arrFB.GetUpperBound(0)
            Dim yUbndOld As Integer = arrFB.GetUpperBound(1)
            Dim zUbndOld As Integer = arrFB.GetUpperBound(2)

            'Wenn verkleinert wird, darf nichts abgeschnitten werden.
            Dim x As Integer
            Dim y As Integer
            Dim z As Integer

            For z = 0 To zUbndOld
                For y = 0 To yUbndOld
                    For x = 0 To xUbndOld
                        If arrFB(x, y, z) <> 0 Then

                            If x > xUbndNew Then Return False
                            If y > yUbndNew Then Return False
                            If z > zUbndNew Then Return False

                            'Randfreiheit muss im neuen Feld erhalten bleiben
                            If x = 0 OrElse x = xUbndNew Then Return False
                            If y = 0 OrElse y = yUbndNew Then Return False
                        End If
                    Next
                Next
            Next

            Return True
        End Function

        Public Function RedimArrFBIsPossible(ByVal addOrRemoveLeft As Integer,
                                             ByVal addOrRemoveTop As Integer,
                                             ByVal addOrRemoveRight As Integer,
                                             ByVal addOrRemoveBottom As Integer,
                                             ByVal addOrRemoveLayer As Integer) As Boolean

            Return RedimArrFBIsPossible(ArrFB,
                                        addOrRemoveLeft,
                                        addOrRemoveTop,
                                        addOrRemoveRight,
                                        addOrRemoveBottom,
                                        addOrRemoveLayer)
        End Function
        '
        ''' <summary>
        ''' Prüft, ob arrFB um die angegebenen Spalten/Zeilen/Ebenen
        ''' vergrößert oder verkleinert werden kann.
        ''' Die addOrRemove-Werte gelten direkt in arrFB-Feldern.
        ''' Verkleinerung mit negativen Werten.
        ''' </summary>
        Public Function RedimArrFBIsPossible(ByVal arrFB(,,) As Integer,
                                     ByVal addOrRemoveLeft As Integer,
                                     ByVal addOrRemoveTop As Integer,
                                     ByVal addOrRemoveRight As Integer,
                                     ByVal addOrRemoveBottom As Integer,
                                     ByVal addOrRemoveLayer As Integer) As Boolean

            If arrFB Is Nothing Then
                Return False
            End If

            Dim xUbndOld As Integer = arrFB.GetUpperBound(0)
            Dim yUbndOld As Integer = arrFB.GetUpperBound(1)
            Dim zUbndOld As Integer = arrFB.GetUpperBound(2)

            Dim xUbndNew As Integer = xUbndOld + addOrRemoveLeft + addOrRemoveRight
            Dim yUbndNew As Integer = yUbndOld + addOrRemoveTop + addOrRemoveBottom
            Dim zUbndNew As Integer = zUbndOld + addOrRemoveLayer

            Dim sizeNew As Triple = GetSizeInSteinenFromUBounds(xUbndNew, yUbndNew, zUbndNew)

            If Not IsTargetSizeValid(sizeNew, False) Then
                Return False
            End If

            Return CanMoveAllOccupiedCells(arrFB,
                                   addOrRemoveLeft,
                                   addOrRemoveTop,
                                   xUbndNew,
                                   yUbndNew,
                                   zUbndNew)
        End Function

        Public Sub ChangeSpielfeldSize(ByVal sizeInSteinen As Triple)

            Dim xUbndNew As Integer = (sizeInSteinen.x * 2) + 1
            Dim yUbndNew As Integer = (sizeInSteinen.y * 2) + 1
            Dim zUbndNew As Integer = sizeInSteinen.z

            Dim arrNew(xUbndNew, yUbndNew, zUbndNew) As Integer

            If ArrFB Is Nothing Then
                ArrFB = arrNew
                Exit Sub
            End If

            CopyArrFBToNewArray(ArrFB,
                        arrNew,
                        0,
                        0)

            ArrFB = arrNew
            SpielSizeInSteinen = sizeInSteinen
            SyncronisationArrfbToSteinInfos()

        End Sub

        '
        ''' <summary>
        ''' Erstellt ein neues arrFB mit relativer Änderung an links/oben/rechts/unten/z.
        ''' Die addOrRemove-Werte gelten direkt in arrFB-Feldern.
        ''' Gibt Nothing zurück, wenn die Änderung unzulässig ist.
        ''' (ruft RedimArrFBIsPossible auf)
        ''' </summary>
        Public Sub ChangeSpielfeldSize(ByVal sizeInSteinen As Triple,
                           ByVal addOrRemoveLeft As Integer,
                           ByVal addOrRemoveTop As Integer,
                           ByVal addOrRemoveRight As Integer,
                           ByVal addOrRemoveBottom As Integer,
                           ByVal addOrRemoveLayer As Integer)

            If ArrFB Is Nothing Then
                Exit Sub
            End If

            If Not RedimArrFBIsPossible(ArrFB,
                                addOrRemoveLeft,
                                addOrRemoveTop,
                                addOrRemoveRight,
                                addOrRemoveBottom,
                                addOrRemoveLayer) Then
                Exit Sub
            End If

            Dim xUbndOld As Integer = ArrFB.GetUpperBound(0)
            Dim yUbndOld As Integer = ArrFB.GetUpperBound(1)
            Dim zUbndOld As Integer = ArrFB.GetUpperBound(2)

            Dim xUbndNew As Integer = xUbndOld + addOrRemoveLeft + addOrRemoveRight
            Dim yUbndNew As Integer = yUbndOld + addOrRemoveTop + addOrRemoveBottom
            Dim zUbndNew As Integer = zUbndOld + addOrRemoveLayer

            Dim arrNew(xUbndNew, yUbndNew, zUbndNew) As Integer

            CopyArrFBToNewArray(ArrFB,
                        arrNew,
                        addOrRemoveLeft,
                        addOrRemoveTop)

            ArrFB = arrNew
            'Die einzelnen SteinInfos brauchen die Referenz auf den ArrFB
            If SteinInfos IsNot Nothing Then
                For Each st As SteinInfo In SteinInfos
                    st.SetArrFbReferenz(ArrFB)
                Next
            End If
            SpielSizeInSteinen = sizeInSteinen
            SyncronisationArrfbToSteinInfos()

        End Sub

        '
        ''' <summary>
        ''' Prüft Mindest- und ggf. Maximalgrößen.
        ''' </summary>
        Private Function IsTargetSizeValid(ByVal sizeInSteinen As Triple,
                                           ByVal backdorIsOpen As Boolean) As Boolean

            If sizeInSteinen Is Nothing Then
                Return False
            End If

            If sizeInSteinen.x < MJ_STEINE_MINX Then Return False
            If sizeInSteinen.y < MJ_STEINE_MINY Then Return False
            If sizeInSteinen.z < MJ_STEINE_MINZ Then Return False

            If Not backdorIsOpen Then
                If sizeInSteinen.x > MJ_STEINE_MAXX Then Return False
                If sizeInSteinen.y > MJ_STEINE_MAXY Then Return False
                If sizeInSteinen.z > MJ_STEINE_MAXZ Then Return False
            End If

            Return True
        End Function

        '
        ''' <summary>
        ''' Wandelt UBounds in die Spielgröße in Steinen um.
        ''' </summary>
        Private Function GetSizeInSteinenFromUBounds(ByVal xUbnd As Integer,
                                                     ByVal yUbnd As Integer,
                                                     ByVal zUbnd As Integer) As Triple

            Return New Triple((xUbnd - 1) \ 2,
                              (yUbnd - 1) \ 2,
                               zUbnd + 1)
        End Function

        '
        ''' <summary>
        ''' Prüft, ob alle belegten Felder nach der Verschiebung noch
        ''' innerhalb des neuen Feldes liegen und nicht auf dem Außenrand.
        ''' </summary>
        Private Function CanMoveAllOccupiedCells(ByVal arrFB(,,) As Integer,
                                         ByVal shiftX As Integer,
                                         ByVal shiftY As Integer,
                                         ByVal xUbndNew As Integer,
                                         ByVal yUbndNew As Integer,
                                         ByVal zUbndNew As Integer) As Boolean

            Dim xUbndOld As Integer = arrFB.GetUpperBound(0)
            Dim yUbndOld As Integer = arrFB.GetUpperBound(1)
            Dim zUbndOld As Integer = arrFB.GetUpperBound(2)

            Dim x As Integer
            Dim y As Integer
            Dim z As Integer

            For z = 0 To zUbndOld
                For y = 0 To yUbndOld
                    For x = 0 To xUbndOld

                        If arrFB(x, y, z) <> 0 Then

                            Dim xNew As Integer = x + shiftX
                            Dim yNew As Integer = y + shiftY
                            Dim zNew As Integer = z

                            If xNew < 0 OrElse xNew > xUbndNew Then Return False
                            If yNew < 0 OrElse yNew > yUbndNew Then Return False
                            If zNew < 0 OrElse zNew > zUbndNew Then Return False

                            If xNew = 0 OrElse xNew = xUbndNew Then Return False
                            If yNew = 0 OrElse yNew = yUbndNew Then Return False
                        End If
                    Next
                Next
            Next

            Return True

        End Function

        ''' <summary>
        ''' Kopiert das alte arrFB in das neue Array.
        ''' Nur X/Y werden verschoben. Z bleibt auf gleicher Ebenennummer.
        ''' shiftX und shiftY sind die Spalten und Zeilen, die neu hinzugekommen/entfernt wurden.
        ''' Es gibt auch Spalten und Zeilen, die rechts und unten hinzugekommen sind/entfernt wurden,
        ''' arrNew ist auf jeden Fall so groß, daß alles hineinpasst, welhalb die
        ''' Einschränkung mit ok zulässig ist.
        ''' </summary>
        Private Sub CopyArrFBToNewArray(ByVal arrOld(,,) As Integer,
                          ByVal arrNew(,,) As Integer,
                          ByVal shiftX As Integer,
                          ByVal shiftY As Integer)

            Dim xUbndOld As Integer = arrOld.GetUpperBound(0)
            Dim yUbndOld As Integer = arrOld.GetUpperBound(1)
            Dim xUbndNew As Integer = arrNew.GetUpperBound(0)
            Dim yUbndNew As Integer = arrNew.GetUpperBound(1)

            Dim zUbnd As Integer = Math.Min(arrOld.GetUpperBound(2), arrNew.GetUpperBound(2))

            For z As Integer = 0 To zUbnd
                For y As Integer = 0 To yUbndOld
                    For x As Integer = 0 To xUbndOld
                        Dim newX As Integer = x + shiftX
                        Dim newY As Integer = y + shiftY
                        Dim ok As Boolean = True

                        If newX < 0 Then ok = False
                        If newY < 0 Then ok = False
                        If newX > xUbndNew Then ok = False
                        If newY > yUbndNew Then ok = False

                        If ok Then
                            arrNew(newX, newY, z) = arrOld(x, y, z)
                        End If
                    Next
                Next
            Next

        End Sub
        '

        ''' <summary>
        ''' Wenn Spalten und Zeilen links und/oder unten angefügt werden,
        ''' muss die Steinposition in den ArrFB korrigiert werden.
        ''' </summary>
        Private Sub SyncronisationArrfbToSteinInfos()
            'dann muss die Steinposition in den Steininfos korrigiert werden.

            Dim xUbndNew As Integer = ArrFB.GetUpperBound(0)
            Dim yUbndNew As Integer = ArrFB.GetUpperBound(1)
            Dim zUbndNew As Integer = ArrFB.GetUpperBound(2)

            Dim x As Integer
            Dim y As Integer
            Dim z As Integer

            For z = 0 To zUbndNew
                For y = 0 To yUbndNew
                    For x = 0 To xUbndNew
                        Dim idxSI As Integer = GetSteinInfoIndexLO(x, y, z)
                        If idxSI >= 0 Then
                            SteinInfos(idxSI).Pos3D = New Triple(x, y, z)
                        End If
                    Next
                Next
            Next
        End Sub

#End Region

#Region "Load / Save"

        Public Shared Function OpenWithDialog(sub1Dir As AppDataSubDir, sub2Dir As AppDataSubSubDir, sub3Dir As String, filename As String) As SFInfo
            Dim fullpath As String = Path.Combine(INI.AppDataDirectory(sub1Dir, sub2Dir, sub3Dir), filename)
            Return Load(fullpath)
        End Function
        Public Shared Function OpenWithDialog(sub1Dir As AppDataSubDir, sub2Dir As AppDataSubSubDir, sub3Dir As BasisformEnum, filename As String, Optional header As String = "Spielfeld laden") As SFInfo
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

                        With daten

                            'Die Dimensionen des ArrFB ändern,
                            'falls die Spielfeldgröße geändert wurde. (Das ist eine Hintertüre)
                            If ._SpielSizeInSteinenFromXml IsNot Nothing AndAlso
                               ._SpielSizeInSteinen IsNot Nothing AndAlso
                               ._SpielSizeInSteinenFromXml.IsNotEqual(._SpielSizeInSteinen) Then
                                If .RedimArrFBIsPossible(.ArrFB, ._SpielSizeInSteinenFromXml, backdorIsOpen:=True) Then
                                    .ChangeSpielfeldSize(._SpielSizeInSteinenFromXml)
                                    .ErrorMsg = Nothing
                                Else
                                    .ErrorMsg = "Redimensionierung nicht möglich: Zielgröße aus XML (" &
                                    ._SpielSizeInSteinenFromXml.ToString &
                                    ") ist mit aktuellem Spielfeld nicht vereinbar." &
                                    "(prüfen, ob Steine des alten Spielfeldes außerhalb des neuen Spielfeldes liegen.)"
                                End If
                            End If
                            '
                            'Die einzelnen SteinInfos brauchen die Referenz auf den ArrFB
                            If ._ArrFB IsNot Nothing AndAlso .SteinInfos IsNot Nothing Then
                                For Each st As SteinInfo In .SteinInfos
                                    st.SetArrFbReferenz(.ArrFB)
                                Next
                            End If
                        End With

                        Return daten

                    End Using
                Catch
                    ' Fehler beim Laden – neue Instanz zurückgeben
                    ' TODO Fehlerbehandlung
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
                "konkrete Collection-Typen und InitDragDropBitmaps der Collections.", ex)
            Catch ex As Exception
                Throw ' nicht verschlucken – hochreichen
            End Try
        End Sub
#End Region

    End Class
End Namespace