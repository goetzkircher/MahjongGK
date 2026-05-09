
Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Globalization
Imports System.IO
Imports System.Reflection
Imports System.Security.Cryptography
Imports System.Text
Imports System.Windows.Forms
Imports System.Xml
Imports System.Xml.Serialization
Imports MahjongGK.Contracts.GlobalEnum

#Disable Warning IDE0079
#Disable Warning IDE1006
#Disable Warning IDE0031
#Disable Warning IDE0017
''' <summary>
''' Container mit allen Werten, die einen Mahjongstein beschreiben.
''' Die Klasse ist in MahjongGK.Contracts angesiedelt.
''' </summary>
Public Class TileColors

    ''' <summary>
    ''' Änderung der Zulässigkeit der Debug-Einstellung nur 
    ''' in diesem Programm.
    ''' </summary>
    Private ReadOnly DEBUGAPPNAME As String = "MahjongGKTileFactoryTester"

    Public Function DeepCopy() As TileColors

        Dim copy As TileColors = DirectCast(Me.MemberwiseClone(), TileColors)

        If Me._shiftLightMapLookup IsNot Nothing Then
            copy._shiftLightMapLookup = CType(Me._shiftLightMapLookup.Clone(), Integer(,))
        End If

        If Me._dShiftLightMapLookup IsNot Nothing Then
            copy._dShiftLightMapLookup = CType(Me._dShiftLightMapLookup.Clone(), Integer(,,))
        End If

        If Me._shiftLayerLookup IsNot Nothing Then
            copy._shiftLayerLookup = CType(Me._shiftLayerLookup.Clone(), Integer(,,))
        End If

        ' Neue temporäre Ident, weil es ein neues Objekt ist.
        copy._sessionIdent = Guid.NewGuid().ToString()

        Return copy

    End Function

#Region "Properties"

    Public Sub New()
        _hashLastStorage = GetMyHash()
    End Sub
    '
    ''' <summary>
    ''' Holt sich aus tileRenderRequest die Werte, die auch per 
    ''' SetSteinMainDescriptor übergeben werden könnten
    ''' </summary>
    ''' <param name="tileRenderRequest"></param>
    Public Sub New(tileRenderRequest As TileRequest)
        With tileRenderRequest
            _aktRenderMode = .AktRenderMode
            _steinStatus = .SteinStatus
            _steinType = .SteinTyp
            _steinTypVersion = .SteinTypVersion
            _steinFrameVersion = .SteinFrameVersion
            _steinSize = .SteinSize
            _steinBasisSize = .SteinBasisSize
        End With
        _hashLastStorage = GetMyHash()
    End Sub

#Region "Header"

    Private _steinSize As Size
    Public WriteOnly Property SteinSize As Size
        Set(value As Size)
            _steinSize = value
        End Set
    End Property

    Private _steinBasisSize As Size
    Public WriteOnly Property SteinBasisSize As Size
        Set(value As Size)
            _steinBasisSize = value
        End Set
    End Property
    ''' <summary>
    ''' Setzte eine neue SessionIdent
    ''' </summary>
    Public Sub SetNewIdent()
        _sessionIdent = Guid.NewGuid().ToString()
    End Sub
    '
    Private _steinDesign As SteinDesign = CType(-1, SteinDesign)

    ''' <summary>
    ''' Das Steindesign wird ausschlielich zum Laden und speichern
    ''' der Klasse benötigt und wird nicht gespeichert.
    ''' (Festlegung des Pfades der TileColors)
    ''' </summary>
    Public WriteOnly Property SteinDesign As SteinDesign
        Set(value As SteinDesign)
            _steinDesign = value
        End Set
    End Property
    '
    Private _steinSatz As SteinSatz = CType(-1, SteinSatz)
    '
    ''' <summary>
    ''' Der Steinsatz wird ausschließlich zum Laden und Speichern des Datensatzes
    ''' verwendet und nicht gespeichert. (Festlegung des Dateinamen der TileColors)
    ''' </summary>
    Public WriteOnly Property SteinSatz As SteinSatz
        Set(value As SteinSatz)
            _steinSatz = value
        End Set
    End Property

    Private _useDevelopmentPath As Boolean?
    Public WriteOnly Property UseDevelopmentPath As Boolean?
        Set(value As Boolean?)
            _useDevelopmentPath = value
        End Set
    End Property

    Public Property Version As String = "1"

    Private _hashLastStorage As String

    ''' <summary>
    ''' Gibt True zurück, wenn seit dem Laden bzw. dem letzten
    ''' Speichern Daten geändert wurden.
    ''' Wird beim Speichern rückgestellt.
    ''' </summary>
    ''' <returns></returns>
    Public Function IsDirtySinceLastStorage() As Boolean
        Dim hash As String = GetMyHash()
        Return hash <> _hashLastStorage
    End Function

    '
    ''' <summary>
    ''' temporäre Ident,(nicht persistent) wird bei jedem Erzeugen erneut vergeben,
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

#Region "Integer-Werte aus tbXxx"

    Public Property HueBasisBlüten As Integer = 350
    Public Property HueBasisJZeiten As Integer = 90
    Public Property HueBasisNormal As Integer = 42
    Public Property HueBasisWinde As Integer = 210
    Public Property HueFaceFrame As Integer = 0
    Public Property HueSymbolGradientFrom As Integer = 0
    Public Property HueSymbolGradientTo As Integer = 210
    Public Property HueSymbolOutline As Integer = 0

#End Region

#Region "Integer und Boolean-Werte numXxx, chkkXxx und optXxx (Copy-Paste-Region aus Form1)"

    'Hinweise: chkBoxen und optButtons müssen in
    'TileColorsFormMapper.GetControlName einzeln angeführt werden,
    'da beide vom Typ Boolean sind und sonst nicht unterschieden werden können.
    '
    ' StatusLoadingOK wird True, sobald die Daten von Festplatte geladen wurden.
    ' (Unterscheidung, ob Defaultinstanz oder korrekt von Festplatte geladen.)

    'Hier befinden sich einige Werte drin, die keine Farben sind.
    'z.B. FaktorBasisWidthToAktWidth das ist so, weil sie in der FileFaktory
    'anfallen und nicht in TileLayout.

    'Hier in dieser Region wird alles angesiedelt,

    'ab hierher aus der Zwischenablage überschreiben
    '=================================================================

    Private ReadOnly _arrCopyNonColorValues() As String = {
        "AlphaGhost",
        "DBrgGhost",
        "DHueGhost",
        "DSatGhost",
        "FaktorSymbolOffsetLeft",
        "FaktorSymbolOffsetTop",
        "FaktorSymbolOutlineWidth",
        "FaktorSymbolSize",
        "FaktorTextOffsetLeft",
        "FaktorTextOffsetTop",
        "FaktorTextSize",
        "InsertFaceFramAlways",
        "InsertFaceFrameNever",
        "InsertFaceFramOnlyMouseOver",
        "InsertFaceFramOnlyNormaleOne",
        "InsertTextBlumen",
        "InsertTextDrachen",
        "InsertTextJZeiten",
        "InsertTextWinde",
        "TextFontStyleBold",
        "TextSymbole",
        "TileBasisHeight",
        "TileBasisWidth"
    }

    Private ReadOnly _arrCopyColorValues() As String = {
        "BrgFaceFrameBlüten",
        "BrgFaceFrameJZeiten",
        "BrgFaceFrameNormal",
        "BrgFaceFrameWinde",
        "BrgI01NormalBlüten",
        "BrgI01NormalJZeiten",
        "BrgI01NormalNormal",
        "BrgI01NormalWinde",
        "BrgI02SelectedBlüten",
        "BrgI02SelectedJZeiten",
        "BrgI02SelectedNormal",
        "BrgI02SelectedWinde",
        "BrgI03SelectableBlüten",
        "BrgI03SelectableJZeiten",
        "BrgI03SelectableNormal",
        "BrgI03SelectableWinde",
        "BrgI04RemovableBlüten",
        "BrgI04RemovableJZeiten",
        "BrgI04RemovableNormal",
        "BrgI04RemovableWinde",
        "BrgI05LockedBlüten",
        "BrgI05LockedJZeiten",
        "BrgI05LockedNormal",
        "BrgI05LockedWinde",
        "BrgI06WerkstückSteinBlüten",
        "BrgI06WerkstückSteinJZeiten",
        "BrgI06WerkstückSteinNormal",
        "BrgI06WerkstückSteinWinde",
        "BrgI07MissingSecondBlüten",
        "BrgI07MissingSecondJZeiten",
        "BrgI07MissingSecondNormal",
        "BrgI07MissingSecondWinde",
        "BrgI08WerkstückEinfügeFehlerBlüten",
        "BrgI08WerkstückEinfügeFehlerJZeiten",
        "BrgI08WerkstückEinfügeFehlerNormal",
        "BrgI08WerkstückEinfügeFehlerWinde",
        "BrgI09WerkstückZufallsgrafikBlüten",
        "BrgI09WerkstückZufallsgrafikJZeiten",
        "BrgI09WerkstückZufallsgrafikNormal",
        "BrgI09WerkstückZufallsgrafikWinde",
        "BrgLayerDnBlüten",
        "BrgLayerDnJZeiten",
        "BrgLayerDnNormal",
        "BrgLayerDnWinde",
        "BrgLayerLineBlüten",
        "BrgLayerLineJZeiten",
        "BrgLayerLineNormal",
        "BrgLayerLineWinde",
        "BrgLayerMidDnBlüten",
        "BrgLayerMidDnJZeiten",
        "BrgLayerMidDnNormal",
        "BrgLayerMidDnWinde",
        "BrgLayerMidUpBlüten",
        "BrgLayerMidUpJZeiten",
        "BrgLayerMidUpNormal",
        "BrgLayerMidUpWinde",
        "BrgLayerUpBlüten",
        "BrgLayerUpJZeiten",
        "BrgLayerUpNormal",
        "BrgLayerUpWinde",
        "BrgSymbolGradientFrom",
        "BrgSymbolGradientTo",
        "BrgSymbolOutline",
        "DBrgSummenÄnderungAdd",
        "DBrgSummenÄnderungMul",
        "DeltaHueLayerBlüten",
        "DeltaHueLayerJZeiten",
        "DeltaHueLayerNormal",
        "DeltaHueLayerWinde",
        "DSatSummenÄnderung",
        "DShiftNormalBlüten",
        "DShiftNormalJZeiten",
        "DShiftNormalNormal",
        "DShiftNormalWinde",
        "DShiftRemovableBlüten",
        "DShiftRemovableJZeiten",
        "DShiftRemovableNormal",
        "DShiftRemovableWinde",
        "DShiftSelectableBlüten",
        "DShiftSelectableJZeiten",
        "DShiftSelectableNormal",
        "DShiftSelectableWinde",
        "DShiftSelectedBlüten",
        "DShiftSelectedJZeiten",
        "DShiftSelectedNormal",
        "DShiftSelectedWinde",
        "FaceACloudSteinStatusRemovable",
        "FaceACloudSteinStatusSelectable",
        "FaceACloudSteinStatusSelected",
        "FaceAGrainSteinStatusNormal",
        "FaceAGrainSteinStatusRemovable",
        "FaceAGrainSteinStatusSelectable",
        "FaceAGrainSteinStatusSelected",
        "FaceFrameEditorCanDrop",
        "FaceFrameEditorMouseOver",
        "FaceFrameEditorMouseSelected",
        "FaceICloudSteinStatusNormal",
        "FaceICloudSteinStatusRemovable",
        "FaceICloudSteinStatusSelectable",
        "FaceICloudSteinStatusSelected",
        "FaceIGrainSteinStatusNormal",
        "FaceIGrainSteinStatusRemovable",
        "FaceIGrainSteinStatusSelectable",
        "FaceIGrainSteinStatusSelected",
        "FaceLightMapNormal",
        "FaceLightMapRemovable",
        "FaceLightMapSelectable",
        "FaceLightMapSelected",
        "FaktorFaceFrameRadius",
        "FaktorOutlineIsZeroIfTileWidthLowerAs",
        "HueBasisBlüten",
        "HueBasisJZeiten",
        "HueBasisNormal",
        "HueBasisWinde",
        "HueFaceFrame",
        "HueSymbolGradientFrom",
        "HueSymbolGradientTo",
        "HueSymbolOutline",
        "LayerACloud",
        "LayerAGrain",
        "SatFaceFrameBlüten",
        "SatFaceFrameJZeiten",
        "SatFaceFrameNormal",
        "SatFaceFrameWinde",
        "SatI01NormalBlüten",
        "SatI01NormalJZeiten",
        "SatI01NormalNormal",
        "SatI01NormalWinde",
        "SatI02SelectedBlüten",
        "SatI02SelectedJZeiten",
        "SatI02SelectedNormal",
        "SatI02SelectedWinde",
        "SatI03SelectableBlüten",
        "SatI03SelectableJZeiten",
        "SatI03SelectableNormal",
        "SatI03SelectableWinde",
        "SatI04RemovableBlüten",
        "SatI04RemovableJZeiten",
        "SatI04RemovableNormal",
        "SatI04RemovableWinde",
        "SatI05LockedBlüten",
        "SatI05LockedJZeiten",
        "SatI05LockedNormal",
        "SatI05LockedWinde",
        "SatI06WerkstückSteinBlüten",
        "SatI06WerkstückSteinJZeiten",
        "SatI06WerkstückSteinNormal",
        "SatI06WerkstückSteinWinde",
        "SatI07MissingSecondBlüten",
        "SatI07MissingSecondJZeiten",
        "SatI07MissingSecondNormal",
        "SatI07MissingSecondWinde",
        "SatI08WerkstückEinfügeFehlerBlüten",
        "SatI08WerkstückEinfügeFehlerJZeiten",
        "SatI08WerkstückEinfügeFehlerNormal",
        "SatI08WerkstückEinfügeFehlerWinde",
        "SatI09WerkstückZufallsgrafikBlüten",
        "SatI09WerkstückZufallsgrafikJZeiten",
        "SatI09WerkstückZufallsgrafikNormal",
        "SatI09WerkstückZufallsgrafikWinde",
        "SatLayerDnBlüten",
        "SatLayerDnJZeiten",
        "SatLayerDnNormal",
        "SatLayerDnWinde",
        "SatLayerLineBlüten",
        "SatLayerLineJZeiten",
        "SatLayerLineNormal",
        "SatLayerLineWinde",
        "SatLayerMidDnBlüten",
        "SatLayerMidDnJZeiten",
        "SatLayerMidDnNormal",
        "SatLayerMidDnWinde",
        "SatLayerMidUpBlüten",
        "SatLayerMidUpJZeiten",
        "SatLayerMidUpNormal",
        "SatLayerMidUpWinde",
        "SatLayerUpBlüten",
        "SatLayerUpJZeiten",
        "SatLayerUpNormal",
        "SatLayerUpWinde",
        "SatSymbolGradientFrom",
        "SatSymbolGradientTo",
        "SatSymbolOutLine",
        "ShiftLayerNormalBlüten",
        "ShiftLayerNormalJZeiten",
        "ShiftLayerNormalNormal",
        "ShiftLayerNormalWinde",
        "ShiftLayerRemovableBlüten",
        "ShiftLayerRemovableJZeiten",
        "ShiftLayerRemovableNormal",
        "ShiftLayerRemovableWinde",
        "ShiftLayerSelectableBlüten",
        "ShiftLayerSelectableJZeiten",
        "ShiftLayerSelectableNormal",
        "ShiftLayerSelectableWinde",
        "ShiftLayerSelectedBlüten",
        "ShiftLayerSelectedJZeiten",
        "ShiftLayerSelectedNormal",
        "ShiftLayerSelectedWinde",
        "ShiftLightMapSteinStatusNormal",
        "ShiftLightMapSteinStatusRemovable",
        "ShiftLightMapSteinStatusSelectable",
        "ShiftLightMapSteinStatusSelected",
        "SymbolColBlauerDrache",
        "SymbolColBlüten",
        "SymbolColGrünerDrache",
        "SymbolColJZeiten",
        "SymbolColRoterDrache",
        "SymbolColWinde",
        "SymbolGradientMode"
    }

    Public Property CloudGrainKoppeln As Boolean = False
    Public Property CopyAll As Boolean = False
    Public Property CopyAllSegeoToAllNoto As Boolean = False
    Public Property CopyColors As Boolean = False
    Public Property CopyNonColors As Boolean = False
    Public Property CopyNotoMediumToLightAndDark As Boolean = False
    Public Property CopySegeoMediumToLightAndDark As Boolean = False
    Public Property CopySegeoMediumToNotoMedium As Boolean = False
    Public Property DebugFaceFrameEditorCanDrop As Boolean = False
    Public Property DebugFaceFrameEditorMouseOver As Boolean = True
    Public Property DebugFaceFrameEditorMouseSelected As Boolean = False
    Public Property DebugShowFaceFrameMouse As Boolean = False
    Public Property DeltaHueLayerBlüten As Integer = 0
    Public Property DeltaHueLayerJZeiten As Integer = 10
    Public Property DeltaHueLayerNormal As Integer = 0
    Public Property DeltaHueLayerWinde As Integer = 0
    Public Property DonotInsertSymbol As Boolean = False
    Private _DShiftNormalBlüten As Integer = 0
    Public Property DShiftNormalBlüten As Integer
        Get
            Return _DShiftNormalBlüten
        End Get
        Set(value As Integer)
            _DShiftNormalBlüten = value
            CreateDShiftNormalBlüten(_DShiftNormalBlüten)
        End Set
    End Property

    Private _DShiftNormalJZeiten As Integer = 0
    Public Property DShiftNormalJZeiten As Integer
        Get
            Return _DShiftNormalJZeiten
        End Get
        Set(value As Integer)
            _DShiftNormalJZeiten = value
            CreateDShiftNormalJZeiten(_DShiftNormalJZeiten)
        End Set
    End Property

    Private _DShiftNormalNormal As Integer = 0
    Public Property DShiftNormalNormal As Integer
        Get
            Return _DShiftNormalNormal
        End Get
        Set(value As Integer)
            _DShiftNormalNormal = value
            CreateDShiftNormalNormal(_DShiftNormalNormal)
        End Set
    End Property

    Private _DShiftNormalWinde As Integer = 0
    Public Property DShiftNormalWinde As Integer
        Get
            Return _DShiftNormalWinde
        End Get
        Set(value As Integer)
            _DShiftNormalWinde = value
            CreateDShiftNormalWinde(_DShiftNormalWinde)
        End Set
    End Property

    Private _DShiftRemovableBlüten As Integer = 0
    Public Property DShiftRemovableBlüten As Integer
        Get
            Return _DShiftRemovableBlüten
        End Get
        Set(value As Integer)
            _DShiftRemovableBlüten = value
            CreateDShiftRemovableBlüten(_DShiftRemovableBlüten)
        End Set
    End Property

    Private _DShiftRemovableJZeiten As Integer = 0
    Public Property DShiftRemovableJZeiten As Integer
        Get
            Return _DShiftRemovableJZeiten
        End Get
        Set(value As Integer)
            _DShiftRemovableJZeiten = value
            CreateDShiftRemovableJZeiten(_DShiftRemovableJZeiten)
        End Set
    End Property

    Private _DShiftRemovableNormal As Integer = 0
    Public Property DShiftRemovableNormal As Integer
        Get
            Return _DShiftRemovableNormal
        End Get
        Set(value As Integer)
            _DShiftRemovableNormal = value
            CreateDShiftRemovableNormal(_DShiftRemovableNormal)
        End Set
    End Property

    Private _DShiftRemovableWinde As Integer = 0
    Public Property DShiftRemovableWinde As Integer
        Get
            Return _DShiftRemovableWinde
        End Get
        Set(value As Integer)
            _DShiftRemovableWinde = value
            CreateDShiftRemovableWinde(_DShiftRemovableWinde)
        End Set
    End Property

    Private _DShiftSelectableBlüten As Integer = 0
    Public Property DShiftSelectableBlüten As Integer
        Get
            Return _DShiftSelectableBlüten
        End Get
        Set(value As Integer)
            _DShiftSelectableBlüten = value
            CreateDShiftSelectableBlüten(_DShiftSelectableBlüten)
        End Set
    End Property

    Private _DShiftSelectableJZeiten As Integer = 0
    Public Property DShiftSelectableJZeiten As Integer
        Get
            Return _DShiftSelectableJZeiten
        End Get
        Set(value As Integer)
            _DShiftSelectableJZeiten = value
            CreateDShiftSelectableJZeiten(_DShiftSelectableJZeiten)
        End Set
    End Property

    Private _DShiftSelectableNormal As Integer = 0
    Public Property DShiftSelectableNormal As Integer
        Get
            Return _DShiftSelectableNormal
        End Get
        Set(value As Integer)
            _DShiftSelectableNormal = value
            CreateDShiftSelectableNormal(_DShiftSelectableNormal)
        End Set
    End Property

    Private _DShiftSelectableWinde As Integer = 0
    Public Property DShiftSelectableWinde As Integer
        Get
            Return _DShiftSelectableWinde
        End Get
        Set(value As Integer)
            _DShiftSelectableWinde = value
            CreateDShiftSelectableWinde(_DShiftSelectableWinde)
        End Set
    End Property

    Private _DShiftSelectedBlüten As Integer = 0
    Public Property DShiftSelectedBlüten As Integer
        Get
            Return _DShiftSelectedBlüten
        End Get
        Set(value As Integer)
            _DShiftSelectedBlüten = value
            CreateDShiftSelectedBlüten(_DShiftSelectedBlüten)
        End Set
    End Property

    Private _DShiftSelectedJZeiten As Integer = 0
    Public Property DShiftSelectedJZeiten As Integer
        Get
            Return _DShiftSelectedJZeiten
        End Get
        Set(value As Integer)
            _DShiftSelectedJZeiten = value
            CreateDShiftSelectedJZeiten(_DShiftSelectedJZeiten)
        End Set
    End Property

    Private _DShiftSelectedNormal As Integer = 0
    Public Property DShiftSelectedNormal As Integer
        Get
            Return _DShiftSelectedNormal
        End Get
        Set(value As Integer)
            _DShiftSelectedNormal = value
            CreateDShiftSelectedNormal(_DShiftSelectedNormal)
        End Set
    End Property

    Private _DShiftSelectedWinde As Integer = 0
    Public Property DShiftSelectedWinde As Integer
        Get
            Return _DShiftSelectedWinde
        End Get
        Set(value As Integer)
            _DShiftSelectedWinde = value
            CreateDShiftSelectedWinde(_DShiftSelectedWinde)
        End Set
    End Property

    Public Property FaceACloudSteinStatusNormal As Decimal = 0D
    Public Property FaceACloudSteinStatusRemovable As Decimal = 0D
    Public Property FaceACloudSteinStatusSelectable As Decimal = 0D
    Public Property FaceACloudSteinStatusSelected As Decimal = 0D
    Public Property FaceAGrainSteinStatusNormal As Decimal = 0D
    Public Property FaceAGrainSteinStatusRemovable As Decimal = 0D
    Public Property FaceAGrainSteinStatusSelectable As Decimal = 0D
    Public Property FaceAGrainSteinStatusSelected As Decimal = 0D
    Public Property SatFaceFrameBlüten As Integer = 100
    Public Property BrgFaceFrameBlüten As Integer = 100
    Public Property FaceFrameEditorCanDrop As String = "FF3CB371"
    Public Property FaceFrameEditorMouseOver As String = "FFFFFFFF"
    Public Property FaceFrameEditorMouseSelected As String = "FFADFF2F"
    Public Property BrgFaceFrameJZeiten As Integer = 100
    Public Property SatFaceFrameJZeiten As Integer = 90
    Public Property BrgFaceFrameNormal As Integer = 100
    Public Property SatFaceFrameNormal As Integer = 100
    Public Property BrgFaceFrameWinde As Integer = 30
    Public Property SatFaceFrameWinde As Integer = 95
    Public Property FaceICloudSteinStatusNormal As Decimal = 0D
    Public Property FaceICloudSteinStatusRemovable As Decimal = 0D
    Public Property FaceICloudSteinStatusSelectable As Decimal = 0D
    Public Property FaceICloudSteinStatusSelected As Decimal = 0D
    Public Property FaceIGrainSteinStatusNormal As Decimal = 0D
    Public Property FaceIGrainSteinStatusRemovable As Decimal = 0D
    Public Property FaceIGrainSteinStatusSelectable As Decimal = 0D
    Public Property FaceIGrainSteinStatusSelected As Decimal = 0D
    Public Property FaceUseInnerAreaSteinStatusNormal As Boolean = False
    Public Property FaceUseInnerAreaSteinStatusRemovable As Boolean = False
    Public Property FaceUseInnerAreaSteinStatusSelectable As Boolean = False
    Public Property FaceUseInnerAreaSteinStatusSelected As Boolean = False
    Public Property FaktorFaceFrameRadius As Decimal = 0D
    Public Property FaktorOutlineIsZeroIfTileWidthLowerAs As Decimal = 100D
    Public Property FaktorSymbolOffsetLeft As Decimal = 1D
    Public Property FaktorSymbolOffsetTop As Decimal = 1D
    Public Property FaktorSymbolOutlineWidth As Decimal = 0D
    Public Property FaktorSymbolSize As Decimal = 1D
    Public Property FaktorTextOffsetLeft As Decimal = 1D
    Public Property FaktorTextOffsetTop As Decimal = 1D
    Public Property FaktorTextSize As Decimal = 1D
    Public Property DBrgGhost As Integer = 0
    Public Property DSatGhost As Integer = 0
    Public Property DHueGhost As Integer = 0
    Public Property AlphaGhost As Integer = 255
    Public Property GhostUseFastMethode As Boolean = True
    Public Property SatI01NormalBlüten As Integer = 35
    Public Property BrgI01NormalBlüten As Integer = 88
    Public Property SatI01NormalJZeiten As Integer = 30
    Public Property BrgI01NormalJZeiten As Integer = 85
    Public Property SatI01NormalNormal As Integer = 18
    Public Property BrgI01NormalNormal As Integer = 92
    Public Property SatI01NormalWinde As Integer = 40
    Public Property BrgI01NormalWinde As Integer = 78
    Public Property BrgI02SelectedBlüten As Integer = 15
    Public Property SatI02SelectedBlüten As Integer = -20
    Public Property BrgI02SelectedJZeiten As Integer = 15
    Public Property SatI02SelectedJZeiten As Integer = -20
    Public Property BrgI02SelectedNormal As Integer = 100
    Public Property SatI02SelectedNormal As Integer = -20
    Public Property BrgI02SelectedWinde As Integer = 15
    Public Property SatI02SelectedWinde As Integer = -20
    Public Property BrgI03SelectableBlüten As Integer = 10
    Public Property SatI03SelectableBlüten As Integer = 10
    Public Property BrgI03SelectableJZeiten As Integer = 10
    Public Property SatI03SelectableJZeiten As Integer = 10
    Public Property SatI03SelectableNormal As Integer = 10
    Public Property BrgI03SelectableNormal As Integer = 10
    Public Property BrgI03SelectableWinde As Integer = 10
    Public Property SatI03SelectableWinde As Integer = 10
    Public Property SatI04RemovableBlüten As Integer = -15
    Public Property BrgI04RemovableBlüten As Integer = 5
    Public Property SatI04RemovableJZeiten As Integer = -15
    Public Property BrgI04RemovableJZeiten As Integer = 5
    Public Property SatI04RemovableNormal As Integer = -15
    Public Property BrgI04RemovableNormal As Integer = 5
    Public Property SatI04RemovableWinde As Integer = -15
    Public Property BrgI04RemovableWinde As Integer = 5
    Public Property BrgI05LockedBlüten As Integer = 0
    Public Property SatI05LockedBlüten As Integer = 0
    Public Property BrgI05LockedJZeiten As Integer = 0
    Public Property SatI05LockedJZeiten As Integer = 0
    Public Property SatI05LockedNormal As Integer = 0
    Public Property BrgI05LockedNormal As Integer = 0
    Public Property BrgI05LockedWinde As Integer = 0
    Public Property SatI05LockedWinde As Integer = 0
    Public Property BrgI06WerkstückSteinBlüten As Integer = 0
    Public Property SatI06WerkstückSteinBlüten As Integer = 0
    Public Property BrgI06WerkstückSteinJZeiten As Integer = 0
    Public Property SatI06WerkstückSteinJZeiten As Integer = 0
    Public Property SatI06WerkstückSteinNormal As Integer = 0
    Public Property BrgI06WerkstückSteinNormal As Integer = 0
    Public Property BrgI06WerkstückSteinWinde As Integer = 0
    Public Property SatI06WerkstückSteinWinde As Integer = 0
    Public Property SatI07MissingSecondBlüten As Integer = 0
    Public Property BrgI07MissingSecondBlüten As Integer = 0
    Public Property SatI07MissingSecondJZeiten As Integer = 0
    Public Property BrgI07MissingSecondJZeiten As Integer = 0
    Public Property SatI07MissingSecondNormal As Integer = 0
    Public Property BrgI07MissingSecondNormal As Integer = 0
    Public Property BrgI07MissingSecondWinde As Integer = 0
    Public Property SatI07MissingSecondWinde As Integer = 0
    Public Property SatI08WerkstückEinfügeFehlerBlüten As Integer = 0
    Public Property BrgI08WerkstückEinfügeFehlerBlüten As Integer = 0
    Public Property SatI08WerkstückEinfügeFehlerJZeiten As Integer = 0
    Public Property BrgI08WerkstückEinfügeFehlerJZeiten As Integer = 0
    Public Property SatI08WerkstückEinfügeFehlerNormal As Integer = 0
    Public Property BrgI08WerkstückEinfügeFehlerNormal As Integer = 0
    Public Property BrgI08WerkstückEinfügeFehlerWinde As Integer = 0
    Public Property SatI08WerkstückEinfügeFehlerWinde As Integer = 0
    Public Property SatI09WerkstückZufallsgrafikBlüten As Integer = 0
    Public Property BrgI09WerkstückZufallsgrafikBlüten As Integer = 0
    Public Property SatI09WerkstückZufallsgrafikJZeiten As Integer = 0
    Public Property BrgI09WerkstückZufallsgrafikJZeiten As Integer = 0
    Public Property BrgI09WerkstückZufallsgrafikNormal As Integer = 0
    Public Property SatI09WerkstückZufallsgrafikNormal As Integer = 0
    Public Property SatI09WerkstückZufallsgrafikWinde As Integer = 0
    Public Property BrgI09WerkstückZufallsgrafikWinde As Integer = 0
    Public Property InsertFaceFramAlways As Boolean = False
    Public Property InsertFaceFrameNever As Boolean = False
    Public Property InsertFaceFramOnlyMouseOver As Boolean = True
    Public Property InsertFaceFramOnlyNormaleOne As Boolean = False
    Public Property InsertTextBlumen As Boolean = False
    Public Property InsertTextDrachen As Boolean = False
    Public Property InsertTextJZeiten As Boolean = False
    Public Property InsertTextWinde As Boolean = False
    Public Property LayerACloud As Decimal = 0D
    Public Property LayerAGrain As Decimal = 0D
    Public Property SatLayerDnBlüten As Integer = 100
    Public Property BrgLayerDnBlüten As Integer = 70
    Public Property SatLayerDnJZeiten As Integer = 100
    Public Property BrgLayerDnJZeiten As Integer = 70
    Public Property SatLayerDnNormal As Integer = 100
    Public Property BrgLayerDnNormal As Integer = 70
    Public Property SatLayerDnWinde As Integer = 100
    Public Property BrgLayerDnWinde As Integer = 70
    Public Property SatLayerLineBlüten As Integer = 100
    Public Property BrgLayerLineBlüten As Integer = 60
    Public Property SatLayerLineJZeiten As Integer = 100
    Public Property BrgLayerLineJZeiten As Integer = 60
    Public Property SatLayerLineNormal As Integer = 100
    Public Property BrgLayerLineNormal As Integer = 60
    Public Property SatLayerLineWinde As Integer = 100
    Public Property BrgLayerLineWinde As Integer = 60
    Public Property SatLayerMidDnBlüten As Integer = 100
    Public Property BrgLayerMidDnBlüten As Integer = 60
    Public Property SatLayerMidDnJZeiten As Integer = 100
    Public Property BrgLayerMidDnJZeiten As Integer = 60
    Public Property SatLayerMidDnNormal As Integer = 100
    Public Property BrgLayerMidDnNormal As Integer = 60
    Public Property SatLayerMidDnWinde As Integer = 100
    Public Property BrgLayerMidDnWinde As Integer = 60
    Public Property SatLayerMidUpBlüten As Integer = 100
    Public Property BrgLayerMidUpBlüten As Integer = 70
    Public Property SatLayerMidUpJZeiten As Integer = 100
    Public Property BrgLayerMidUpJZeiten As Integer = 70
    Public Property SatLayerMidUpNormal As Integer = 100
    Public Property BrgLayerMidUpNormal As Integer = 70
    Public Property SatLayerMidUpWinde As Integer = 100
    Public Property BrgLayerMidUpWinde As Integer = 70
    Public Property SatLayerUpBlüten As Integer = 100
    Public Property BrgLayerUpBlüten As Integer = 80
    Public Property SatLayerUpJZeiten As Integer = 100
    Public Property BrgLayerUpJZeiten As Integer = 80
    Public Property SatLayerUpNormal As Integer = 100
    Public Property BrgLayerUpNormal As Integer = 80
    Public Property SatLayerUpWinde As Integer = 100
    Public Property BrgLayerUpWinde As Integer = 80
    Private _ShiftLayerNormalBlüten As Integer = 0
    Public Property ShiftLayerNormalBlüten As Integer
        Get
            Return _ShiftLayerNormalBlüten
        End Get
        Set(value As Integer)
            _ShiftLayerNormalBlüten = value
            CreateShiftLayerNormalBlüten(_ShiftLayerNormalBlüten)
        End Set
    End Property

    Private _ShiftLayerNormalJZeiten As Integer = 0
    Public Property ShiftLayerNormalJZeiten As Integer
        Get
            Return _ShiftLayerNormalJZeiten
        End Get
        Set(value As Integer)
            _ShiftLayerNormalJZeiten = value
            CreateShiftLayerNormalJZeiten(_ShiftLayerNormalJZeiten)
        End Set
    End Property

    Private _ShiftLayerNormalNormal As Integer = 0
    Public Property ShiftLayerNormalNormal As Integer
        Get
            Return _ShiftLayerNormalNormal
        End Get
        Set(value As Integer)
            _ShiftLayerNormalNormal = value
            CreateShiftLayerNormalNormal(_ShiftLayerNormalNormal)
        End Set
    End Property

    Private _ShiftLayerNormalWinde As Integer = 0
    Public Property ShiftLayerNormalWinde As Integer
        Get
            Return _ShiftLayerNormalWinde
        End Get
        Set(value As Integer)
            _ShiftLayerNormalWinde = value
            CreateShiftLayerNormalWinde(_ShiftLayerNormalWinde)
        End Set
    End Property

    Private _ShiftLayerRemovableBlüten As Integer = 0
    Public Property ShiftLayerRemovableBlüten As Integer
        Get
            Return _ShiftLayerRemovableBlüten
        End Get
        Set(value As Integer)
            _ShiftLayerRemovableBlüten = value
            CreateShiftLayerRemovableBlüten(_ShiftLayerRemovableBlüten)
        End Set
    End Property

    Private _ShiftLayerRemovableJZeiten As Integer = 0
    Public Property ShiftLayerRemovableJZeiten As Integer
        Get
            Return _ShiftLayerRemovableJZeiten
        End Get
        Set(value As Integer)
            _ShiftLayerRemovableJZeiten = value
            CreateShiftLayerRemovableJZeiten(_ShiftLayerRemovableJZeiten)
        End Set
    End Property

    Private _ShiftLayerRemovableNormal As Integer = 0
    Public Property ShiftLayerRemovableNormal As Integer
        Get
            Return _ShiftLayerRemovableNormal
        End Get
        Set(value As Integer)
            _ShiftLayerRemovableNormal = value
            CreateShiftLayerRemovableNormal(_ShiftLayerRemovableNormal)
        End Set
    End Property

    Private _ShiftLayerRemovableWinde As Integer = 0
    Public Property ShiftLayerRemovableWinde As Integer
        Get
            Return _ShiftLayerRemovableWinde
        End Get
        Set(value As Integer)
            _ShiftLayerRemovableWinde = value
            CreateShiftLayerRemovableWinde(_ShiftLayerRemovableWinde)
        End Set
    End Property

    Private _ShiftLayerSelectableBlüten As Integer = 0
    Public Property ShiftLayerSelectableBlüten As Integer
        Get
            Return _ShiftLayerSelectableBlüten
        End Get
        Set(value As Integer)
            _ShiftLayerSelectableBlüten = value
            CreateShiftLayerSelectableBlüten(_ShiftLayerSelectableBlüten)
        End Set
    End Property

    Private _ShiftLayerSelectableJZeiten As Integer = 0
    Public Property ShiftLayerSelectableJZeiten As Integer
        Get
            Return _ShiftLayerSelectableJZeiten
        End Get
        Set(value As Integer)
            _ShiftLayerSelectableJZeiten = value
            CreateShiftLayerSelectableJZeiten(_ShiftLayerSelectableJZeiten)
        End Set
    End Property

    Private _ShiftLayerSelectableNormal As Integer = 0
    Public Property ShiftLayerSelectableNormal As Integer
        Get
            Return _ShiftLayerSelectableNormal
        End Get
        Set(value As Integer)
            _ShiftLayerSelectableNormal = value
            CreateShiftLayerSelectableNormal(_ShiftLayerSelectableNormal)
        End Set
    End Property

    Private _ShiftLayerSelectableWinde As Integer = 0
    Public Property ShiftLayerSelectableWinde As Integer
        Get
            Return _ShiftLayerSelectableWinde
        End Get
        Set(value As Integer)
            _ShiftLayerSelectableWinde = value
            CreateShiftLayerSelectableWinde(_ShiftLayerSelectableWinde)
        End Set
    End Property

    Private _ShiftLayerSelectedBlüten As Integer = 0
    Public Property ShiftLayerSelectedBlüten As Integer
        Get
            Return _ShiftLayerSelectedBlüten
        End Get
        Set(value As Integer)
            _ShiftLayerSelectedBlüten = value
            CreateShiftLayerSelectedBlüten(_ShiftLayerSelectedBlüten)
        End Set
    End Property

    Private _ShiftLayerSelectedJZeiten As Integer = 0
    Public Property ShiftLayerSelectedJZeiten As Integer
        Get
            Return _ShiftLayerSelectedJZeiten
        End Get
        Set(value As Integer)
            _ShiftLayerSelectedJZeiten = value
            CreateShiftLayerSelectedJZeiten(_ShiftLayerSelectedJZeiten)
        End Set
    End Property

    Private _ShiftLayerSelectedNormal As Integer = 0
    Public Property ShiftLayerSelectedNormal As Integer
        Get
            Return _ShiftLayerSelectedNormal
        End Get
        Set(value As Integer)
            _ShiftLayerSelectedNormal = value
            CreateShiftLayerSelectedNormal(_ShiftLayerSelectedNormal)
        End Set
    End Property

    Private _ShiftLayerSelectedWinde As Integer = 0
    Public Property ShiftLayerSelectedWinde As Integer
        Get
            Return _ShiftLayerSelectedWinde
        End Get
        Set(value As Integer)
            _ShiftLayerSelectedWinde = value
            CreateShiftLayerSelectedWinde(_ShiftLayerSelectedWinde)
        End Set
    End Property

    Private _ShiftLightMapSteinStatusNormal As Integer = 70
    Public Property ShiftLightMapSteinStatusNormal As Integer
        Get
            Return _ShiftLightMapSteinStatusNormal
        End Get
        Set(value As Integer)
            _ShiftLightMapSteinStatusNormal = value
            CreateShiftLightMapSteinStatusNormal(_ShiftLightMapSteinStatusNormal)
        End Set
    End Property

    Private _ShiftLightMapSteinStatusRemovable As Integer = 60
    Public Property ShiftLightMapSteinStatusRemovable As Integer
        Get
            Return _ShiftLightMapSteinStatusRemovable
        End Get
        Set(value As Integer)
            _ShiftLightMapSteinStatusRemovable = value
            CreateShiftLightMapSteinStatusRemovable(_ShiftLightMapSteinStatusRemovable)
        End Set
    End Property

    Private _ShiftLightMapSteinStatusSelectable As Integer = 60
    Public Property ShiftLightMapSteinStatusSelectable As Integer
        Get
            Return _ShiftLightMapSteinStatusSelectable
        End Get
        Set(value As Integer)
            _ShiftLightMapSteinStatusSelectable = value
            CreateShiftLightMapSteinStatusSelectable(_ShiftLightMapSteinStatusSelectable)
        End Set
    End Property

    Private _ShiftLightMapSteinStatusSelected As Integer = 70
    Public Property ShiftLightMapSteinStatusSelected As Integer
        Get
            Return _ShiftLightMapSteinStatusSelected
        End Get
        Set(value As Integer)
            _ShiftLightMapSteinStatusSelected = value
            CreateShiftLightMapSteinStatusSelected(_ShiftLightMapSteinStatusSelected)
        End Set
    End Property

    Public Property StatusLoadingOK As Boolean = False
    Private _SteinFont As SteinFont = SteinFont.Segoe
    <XmlIgnore>
    Public Property SteinFont As SteinFont
        Get
            Return _SteinFont
        End Get
        Set(value As SteinFont)
            _SteinFont = value
        End Set
    End Property

    Public Property DSatSummenÄnderung As Decimal = 1D
    Public Property DBrgSummenÄnderungAdd As Decimal = 0D
    Public Property DBrgSummenÄnderungMul As Decimal = 1D
    Public Property DBrgSummenÄnderungMultiplikation As Boolean = True
    Public Property SymbolColBlauerDrache As String = "00FFFFFF"
    Public Property SymbolColBlüten As String = "00FFFFFF"
    Public Property SymbolColGrünerDrache As String = "00FFFFFF"
    Public Property SymbolColJZeiten As String = "00FFFFFF"
    Public Property SymbolColRoterDrache As String = "00FFFFFF"
    Public Property SymbolColWinde As String = "00FFFFFF"
    Public Property BrgSymbolGradientFrom As Integer = 100
    Public Property SatSymbolGradientFrom As Integer = 100
    Public Property SatSymbolGradientTo As Integer = 100
    Public Property BrgSymbolGradientTo As Integer = 100
    Public Property SymbolGradientToKoppeln As Boolean = False
    Public Property SatSymbolOutLine As Integer = 100
    Public Property BrgSymbolOutline As Integer = 100
    Public Property TextColor As String = "FF000000"
    Public Property TextFontStyleBold As Boolean = False
    Public Property TextSymbole As String = "NSOW▲✿✱!"
    Public Property TileBasisHeight As Integer = 242
    Public Property TileBasisWidth As Integer = 200
    Public Property TilesSpaceBetweenHeight As Integer = 0
    Public Property TilesSpaceBetweenWidth As Integer = 0
    Public Property UsedFontNotoSansSymbol2 As Boolean
        Get
            Return _SteinFont = SteinFont.Noto
        End Get
        Set(value As Boolean)
            _SteinFont = If(value, SteinFont.Noto, SteinFont.Segoe)
        End Set
    End Property

    Public Property UsedFontSegeoUISymbol As Boolean
        Get
            Return _SteinFont = SteinFont.Segoe
        End Get
        Set(value As Boolean)
            _SteinFont = If(value, SteinFont.Segoe, SteinFont.Noto)
        End Set
    End Property

    '=================================================================
    'bis hierher aus der Zwischenablage überschreiben

#End Region

#Region "ComboBox / Enum"

    <XmlIgnore>
    Public Property SymbolGradientMode As LinearGradientMode = LinearGradientMode.BackwardDiagonal
    Public ReadOnly Property GetSymbolGradientMode As LinearGradientMode
        Get
            Return SymbolGradientMode
        End Get
    End Property

    <XmlElement("SymbolGradientMode")>
    Public Property SymbolGradientModeXml As String
        Get
            Return Me.SymbolGradientMode.ToString()
        End Get
        Set(ByVal value As String)
            Dim parsed As LinearGradientMode

            If [Enum].TryParse(Of LinearGradientMode)(value, True, parsed) Then
                Me.SymbolGradientMode = parsed
            Else
                If Debugger.IsAttached Then
                    Throw New InvalidOperationException(
                    $"Ungültiger LinearGradientMode-Wert in XML: '{value}'.")
                End If

                Me.SymbolGradientMode = LinearGradientMode.BackwardDiagonal
            End If
        End Set
    End Property

    <XmlIgnore>
    Public Property FaceLightMapNormal As LightMap = LightMap.RahmenXL

    <XmlElement("FaceLightMapNormal")>
    Public Property FaceLightMapNormalXml As String
        Get
            Return Me.FaceLightMapNormal.ToString()
        End Get
        Set(ByVal value As String)
            Dim parsed As LightMap

            If [Enum].TryParse(Of LightMap)(value, True, parsed) Then
                Me.FaceLightMapNormal = parsed
            Else
                If Debugger.IsAttached Then
                    Throw New InvalidOperationException(
                    $"Ungültiger LightMap-Wert in XML: '{value}'.")
                End If

                Me.FaceLightMapNormal = LightMap.Zentral
            End If
        End Set
    End Property

    <XmlIgnore>
    Public Property FaceLightMapSelected As LightMap = LightMap.RahmenXS

    <XmlElement("FaceLightMapSelected")>
    Public Property FaceLightMapSelectedXml As String
        Get
            Return Me.FaceLightMapSelected.ToString()
        End Get
        Set(ByVal value As String)
            Dim parsed As LightMap

            If [Enum].TryParse(Of LightMap)(value, True, parsed) Then
                Me.FaceLightMapSelected = parsed
            Else
                If Debugger.IsAttached Then
                    Throw New InvalidOperationException(
                    $"Ungültiger LightMap-Wert in XML: '{value}'.")
                End If

                Me.FaceLightMapSelected = LightMap.Zentral
            End If
        End Set
    End Property

    <XmlIgnore>
    Public Property FaceLightMapSelectable As LightMap = LightMap.RahmenS

    <XmlElement("FaceLightMapSelectable")>
    Public Property FaceLightMapSelectableXml As String
        Get
            Return Me.FaceLightMapSelectable.ToString()
        End Get
        Set(ByVal value As String)
            Dim parsed As LightMap

            If [Enum].TryParse(Of LightMap)(value, True, parsed) Then
                Me.FaceLightMapSelectable = parsed
            Else
                If Debugger.IsAttached Then
                    Throw New InvalidOperationException(
                    $"Ungültiger LightMap-Wert in XML: '{value}'.")
                End If

                Me.FaceLightMapSelectable = LightMap.Zentral
            End If
        End Set
    End Property

    <XmlIgnore>
    Public Property FaceLightMapRemovable As LightMap = LightMap.RahmenXL

    <XmlElement("FaceLightMapRemovable")>
    Public Property FaceLightMapRemovableXml As String
        Get
            Return Me.FaceLightMapRemovable.ToString()
        End Get
        Set(ByVal value As String)
            Dim parsed As LightMap

            If [Enum].TryParse(Of LightMap)(value, True, parsed) Then
                Me.FaceLightMapRemovable = parsed
            Else
                If Debugger.IsAttached Then
                    Throw New InvalidOperationException(
                    $"Ungültiger LightMap-Wert in XML: '{value}'.")
                End If

                Me.FaceLightMapRemovable = LightMap.Zentral
            End If
        End Set
    End Property

#End Region

#Region "ColorBox / Color"

#End Region

#Region "Sonstige Property"

#End Region

    ''' <summary>
    ''' Liefert einen stabilen AutoHash über alle beschreibenden Public-Properties.
    ''' ReadOnly-Properties wie AppRoot gehen bewusst nicht ein.
    ''' XML-Hilfsproperties mit Suffix "Xml" gehen ebenfalls nicht ein,
    ''' damit z. B. eine Color nicht doppelt gezählt wird.
    ''' </summary>
    Public Function GetMyHash() As String

        Dim props As PropertyInfo() =
            GetType(TileColors).GetProperties(BindingFlags.Instance Or BindingFlags.Public)

        Array.Sort(props,
                   Function(left As PropertyInfo, right As PropertyInfo) As Integer
                       Return StringComparer.Ordinal.Compare(left.Name, right.Name)
                   End Function)

        Dim parts As New List(Of String)()

        For Each prop As PropertyInfo In props
            If Not prop.CanRead Then Continue For
            If Not prop.CanWrite Then Continue For
            If prop.GetIndexParameters().Length <> 0 Then Continue For
            If IsXmlHelperProperty(prop) Then Continue For
            If prop.Name = "SteinDesign" Then Continue For
            If prop.Name = "SteinSatz" Then Continue For
            If prop.Name = "StatusLoadingOK" Then Continue For
            If prop.Name = "DonotInsertSymbol" Then Continue For

            Dim rawValue As Object = prop.GetValue(Me, Nothing)
            Dim valueText As String = ConvertPropertyValueToInvariantString(rawValue)

            parts.Add(prop.Name & "=" & valueText)
        Next

        Dim payload As String = String.Join(vbLf, parts.ToArray())
        Dim payloadBytes As Byte() = Encoding.UTF8.GetBytes(payload)

        Using sha As SHA256 = SHA256.Create()
            Dim hashBytes As Byte() = sha.ComputeHash(payloadBytes)
            Dim sb As New StringBuilder(hashBytes.Length * 2)

            For Each b As Byte In hashBytes
                sb.Append(b.ToString("x2", CultureInfo.InvariantCulture))
            Next

            Return sb.ToString()
        End Using

    End Function

    Public Sub CompareTileColors(tileColors As TileColors)

        If tileColors Is Nothing Then
            Throw New ArgumentNullException(NameOf(tileColors))
        End If

        Dim props As PropertyInfo() =
        GetType(TileColors).GetProperties(BindingFlags.Instance Or BindingFlags.Public)

        Array.Sort(props,
               Function(left As PropertyInfo, right As PropertyInfo) As Integer
                   Return StringComparer.Ordinal.Compare(left.Name, right.Name)
               End Function)

        Dim lines As New List(Of String)()

        lines.Add("Hash Me        : " & Me.GetMyHash())
        lines.Add("Hash Vergleich : " & tileColors.GetMyHash())
        lines.Add(String.Empty)

        For Each prop As PropertyInfo In props
            If Not prop.CanRead Then Continue For
            If Not prop.CanWrite Then Continue For
            If prop.GetIndexParameters().Length <> 0 Then Continue For
            If IsXmlHelperProperty(prop) Then Continue For
            If prop.Name = "SteinDesign" Then Continue For
            If prop.Name = "SteinSatz" Then Continue For

            Dim myRawValue As Object = prop.GetValue(Me, Nothing)
            Dim otherRawValue As Object = prop.GetValue(tileColors, Nothing)

            Dim myValueText As String = ConvertPropertyValueToInvariantString(myRawValue)
            Dim otherValueText As String = ConvertPropertyValueToInvariantString(otherRawValue)

            If StringComparer.Ordinal.Compare(myValueText, otherValueText) <> 0 Then
                lines.Add(prop.Name)
                lines.Add("  Me        = " & myValueText)
                lines.Add("  Vergleich = " & otherValueText)
                lines.Add(String.Empty)
            End If
        Next

        If lines.Count = 3 Then
            lines.Add("Keine Unterschiede.")
        End If

        Clipboard.SetText(String.Join(Environment.NewLine, lines.ToArray()))

    End Sub
#End Region

#Region "Abruf der Werte"

    Private _aktRenderMode As AktRenderMode
    Private _steinStatus As SteinStatus
    Private _steinType As SteinTyp
    Private _steinTypVersion As SteinTypVersion
    Private _steinFrameVersion As SteinFrameVersion
    '
    ''' <summary>
    ''' Der aktuelle SteinStatus minus eins. (also ohne die unsichtbaren Steine) 
    ''' Wird zum schnellem Zugriff auf die Lightmap-Lookup gebraucht.
    ''' </summary>
    Private _idxSteinStatusForLookup As Integer
    '

    Public Sub SetSteinMainDescriptor(tileRequest As TileRequest)
        With tileRequest
            _aktRenderMode = .AktRenderMode
            _steinStatus = .SteinStatus
            _steinType = .SteinTyp
            _steinTypVersion = .SteinTypVersion
            _steinFrameVersion = .SteinFrameVersion
            _steinSize = .SteinSize
            _steinBasisSize = .SteinBasisSize

            _idxSteinStatusForLookup = GetSteinStatusForLookup(_steinStatus)
        End With
    End Sub
    '
    ''' <summary>
    ''' Die Funktion nicht laufend aus den Lookup-Abfragen aufrufen, da diese Abfragen zeitkritisch sind.
    ''' _idxSteinStatusForLookup nutzen.
    ''' </summary>
    ''' <param name="steinStatus"></param>
    ''' <returns></returns>
    Private Function GetSteinStatusForLookup(steinStatus As SteinStatus) As Integer

        'Der Index auf die Lookuptabelle wird auf die Steinstati
        'Normal, Selected, Selectable, Removable begrenzt.
        'Alle anderen werden auf "Normal" umgebogen
        Dim idxSteinStatusForLookup As Integer = steinStatus - 1
        If idxSteinStatusForLookup > 3 Then
            idxSteinStatusForLookup = 0
        End If

        Return idxSteinStatusForLookup

    End Function

    Public Function GetSteinFrameVersion() As SteinFrameVersion
        Return _steinFrameVersion
    End Function

    Public Function GetSymbolFontFamilyName() As String
        If _SteinFont = SteinFont.Segoe Then
            Return "Segoe UI Symbol"
        Else
            Return "Noto Sans Symbols 2"
        End If
    End Function

    Public Function GetColSteinBasisColorHueSatBrg() As (hue As Integer, sat As Integer, brg As Integer)

        Dim hue As Integer
        Dim sat As Integer
        Dim brg As Integer

        Select Case _steinStatus
            Case SteinStatus.I01Normal
                Select Case _steinTypVersion
                    Case SteinTypVersion.Normal
                        hue = HueBasisNormal
                        sat = SatI01NormalNormal
                        brg = BrgI01NormalNormal
                    Case SteinTypVersion.Blüten
                        hue = HueBasisBlüten
                        sat = SatI01NormalBlüten
                        brg = BrgI01NormalBlüten
                    Case SteinTypVersion.Winde
                        hue = HueBasisWinde
                        sat = SatI01NormalWinde
                        brg = BrgI01NormalWinde
                    Case SteinTypVersion.JZeiten
                        hue = HueBasisJZeiten
                        sat = SatI01NormalJZeiten
                        brg = BrgI01NormalJZeiten
                End Select
            Case SteinStatus.I02Selected
                Select Case _steinTypVersion
                    Case SteinTypVersion.Normal
                        hue = HueBasisNormal
                        sat = AddAndClamp(SatI02SelectedNormal, SatI01NormalNormal)
                        brg = AddAndClamp(BrgI02SelectedNormal, BrgI01NormalNormal)
                    Case SteinTypVersion.Blüten
                        hue = HueBasisBlüten
                        sat = AddAndClamp(SatI02SelectedBlüten, SatI01NormalNormal)
                        brg = AddAndClamp(BrgI02SelectedBlüten, BrgI01NormalNormal)
                    Case SteinTypVersion.Winde
                        hue = HueBasisWinde
                        sat = AddAndClamp(SatI02SelectedWinde, SatI01NormalNormal)
                        brg = AddAndClamp(BrgI02SelectedWinde, BrgI01NormalNormal)
                    Case SteinTypVersion.JZeiten
                        hue = HueBasisJZeiten
                        sat = AddAndClamp(SatI02SelectedJZeiten, SatI01NormalNormal)
                        brg = AddAndClamp(BrgI02SelectedJZeiten, BrgI01NormalNormal)
                End Select
            Case SteinStatus.I03Selectable
                Select Case _steinTypVersion
                    Case SteinTypVersion.Normal
                        hue = HueBasisNormal
                        sat = AddAndClamp(SatI03SelectableNormal, SatI01NormalNormal)
                        brg = AddAndClamp(BrgI03SelectableNormal, BrgI01NormalNormal)
                    Case SteinTypVersion.Blüten
                        hue = HueBasisBlüten
                        sat = AddAndClamp(SatI03SelectableBlüten, SatI01NormalNormal)
                        brg = AddAndClamp(BrgI03SelectableBlüten, BrgI01NormalNormal)
                    Case SteinTypVersion.Winde
                        hue = HueBasisWinde
                        sat = AddAndClamp(SatI03SelectableWinde, SatI01NormalNormal)
                        brg = AddAndClamp(BrgI03SelectableWinde, BrgI01NormalNormal)
                    Case SteinTypVersion.JZeiten
                        hue = HueBasisJZeiten
                        sat = AddAndClamp(SatI03SelectableJZeiten, SatI01NormalNormal)
                        brg = AddAndClamp(BrgI03SelectableJZeiten, BrgI01NormalNormal)
                End Select
            Case SteinStatus.I04Removable
                Select Case _steinTypVersion
                    Case SteinTypVersion.Normal
                        hue = HueBasisNormal
                        sat = AddAndClamp(SatI04RemovableNormal, SatI01NormalNormal)
                        brg = AddAndClamp(BrgI04RemovableNormal, BrgI01NormalNormal)
                    Case SteinTypVersion.Blüten
                        hue = HueBasisBlüten
                        sat = AddAndClamp(SatI04RemovableBlüten, SatI01NormalNormal)
                        brg = AddAndClamp(BrgI04RemovableBlüten, BrgI01NormalNormal)
                    Case SteinTypVersion.Winde
                        hue = HueBasisWinde
                        sat = AddAndClamp(SatI04RemovableWinde, SatI01NormalNormal)
                        brg = AddAndClamp(BrgI04RemovableWinde, BrgI01NormalNormal)
                    Case SteinTypVersion.JZeiten
                        hue = HueBasisJZeiten
                        sat = AddAndClamp(SatI04RemovableJZeiten, SatI01NormalNormal)
                        brg = AddAndClamp(BrgI04RemovableJZeiten, BrgI01NormalNormal)
                End Select
            Case SteinStatus.I05Locked
                Select Case _steinTypVersion
                    Case SteinTypVersion.Normal
                        hue = HueBasisNormal
                        sat = AddAndClamp(SatI05LockedNormal, SatI01NormalNormal)
                        brg = AddAndClamp(BrgI05LockedNormal, BrgI01NormalNormal)
                    Case SteinTypVersion.Blüten
                        hue = HueBasisBlüten
                        sat = AddAndClamp(SatI05LockedBlüten, SatI01NormalNormal)
                        brg = AddAndClamp(BrgI05LockedBlüten, BrgI01NormalNormal)
                    Case SteinTypVersion.Winde
                        hue = HueBasisWinde
                        sat = AddAndClamp(SatI05LockedWinde, SatI01NormalNormal)
                        brg = AddAndClamp(BrgI05LockedWinde, BrgI01NormalNormal)
                    Case SteinTypVersion.JZeiten
                        hue = HueBasisJZeiten
                        sat = AddAndClamp(SatI05LockedJZeiten, SatI01NormalNormal)
                        brg = AddAndClamp(BrgI05LockedJZeiten, BrgI01NormalNormal)
                End Select
            Case SteinStatus.I06WerkstückStein
                Select Case _steinTypVersion
                    Case SteinTypVersion.Normal
                        hue = HueBasisNormal
                        sat = AddAndClamp(SatI06WerkstückSteinNormal, SatI01NormalNormal)
                        brg = AddAndClamp(BrgI06WerkstückSteinNormal, BrgI01NormalNormal)
                    Case SteinTypVersion.Blüten
                        hue = HueBasisBlüten
                        sat = AddAndClamp(SatI06WerkstückSteinBlüten, SatI01NormalNormal)
                        brg = AddAndClamp(BrgI06WerkstückSteinBlüten, BrgI01NormalNormal)
                    Case SteinTypVersion.Winde
                        hue = HueBasisWinde
                        sat = AddAndClamp(SatI06WerkstückSteinWinde, SatI01NormalNormal)
                        brg = AddAndClamp(BrgI06WerkstückSteinWinde, BrgI01NormalNormal)
                    Case SteinTypVersion.JZeiten
                        hue = HueBasisJZeiten
                        sat = AddAndClamp(SatI06WerkstückSteinJZeiten, SatI01NormalNormal)
                        brg = AddAndClamp(BrgI06WerkstückSteinJZeiten, BrgI01NormalNormal)
                End Select
            Case SteinStatus.I07MissingSecond
                Select Case _steinTypVersion
                    Case SteinTypVersion.Normal
                        hue = HueBasisNormal
                        sat = AddAndClamp(SatI07MissingSecondNormal, SatI01NormalNormal)
                        brg = AddAndClamp(BrgI07MissingSecondNormal, BrgI01NormalNormal)
                    Case SteinTypVersion.Blüten
                        hue = HueBasisBlüten
                        sat = AddAndClamp(SatI07MissingSecondBlüten, SatI01NormalNormal)
                        brg = AddAndClamp(BrgI07MissingSecondBlüten, BrgI01NormalNormal)
                    Case SteinTypVersion.Winde
                        hue = HueBasisWinde
                        sat = AddAndClamp(SatI07MissingSecondWinde, SatI01NormalNormal)
                        brg = AddAndClamp(BrgI07MissingSecondWinde, BrgI01NormalNormal)
                    Case SteinTypVersion.JZeiten
                        hue = HueBasisJZeiten
                        sat = AddAndClamp(SatI07MissingSecondJZeiten, SatI01NormalNormal)
                        brg = AddAndClamp(BrgI07MissingSecondJZeiten, BrgI01NormalNormal)
                End Select

            Case SteinStatus.I08WerkstückEinfügeFehler
                Select Case _steinTypVersion
                    Case SteinTypVersion.Normal
                        hue = HueBasisNormal
                        sat = AddAndClamp(SatI08WerkstückEinfügeFehlerNormal, SatI01NormalNormal)
                        brg = AddAndClamp(BrgI08WerkstückEinfügeFehlerNormal, BrgI01NormalNormal)
                    Case SteinTypVersion.Blüten
                        hue = HueBasisBlüten
                        sat = AddAndClamp(SatI08WerkstückEinfügeFehlerBlüten, SatI01NormalNormal)
                        brg = AddAndClamp(BrgI08WerkstückEinfügeFehlerBlüten, BrgI01NormalNormal)
                    Case SteinTypVersion.Winde
                        hue = HueBasisWinde
                        sat = AddAndClamp(SatI08WerkstückEinfügeFehlerWinde, SatI01NormalNormal)
                        brg = AddAndClamp(BrgI08WerkstückEinfügeFehlerWinde, BrgI01NormalNormal)
                    Case SteinTypVersion.JZeiten
                        hue = HueBasisJZeiten
                        sat = AddAndClamp(SatI08WerkstückEinfügeFehlerJZeiten, SatI01NormalNormal)
                        brg = AddAndClamp(BrgI08WerkstückEinfügeFehlerJZeiten, BrgI01NormalNormal)
                End Select
            Case SteinStatus.I09WerkstückZufallsgrafik
                Select Case _steinTypVersion
                    Case SteinTypVersion.Normal
                        hue = HueBasisNormal
                        sat = AddAndClamp(SatI09WerkstückZufallsgrafikNormal, SatI01NormalNormal)
                        brg = AddAndClamp(BrgI09WerkstückZufallsgrafikNormal, BrgI01NormalNormal)
                    Case SteinTypVersion.Blüten
                        hue = HueBasisBlüten
                        sat = AddAndClamp(SatI09WerkstückZufallsgrafikBlüten, SatI01NormalNormal)
                        brg = AddAndClamp(BrgI09WerkstückZufallsgrafikBlüten, BrgI01NormalNormal)
                    Case SteinTypVersion.Winde
                        hue = HueBasisWinde
                        sat = AddAndClamp(SatI09WerkstückZufallsgrafikWinde, SatI01NormalNormal)
                        brg = AddAndClamp(BrgI09WerkstückZufallsgrafikWinde, BrgI01NormalNormal)
                    Case SteinTypVersion.JZeiten
                        hue = HueBasisJZeiten
                        sat = AddAndClamp(SatI09WerkstückZufallsgrafikJZeiten, SatI01NormalNormal)
                        brg = AddAndClamp(BrgI09WerkstückZufallsgrafikJZeiten, BrgI01NormalNormal)
                    Case Else
                        Throw New Exception("Ungültiger Steinstatus")
                End Select
        End Select
        Return (hue, sat, brg)

    End Function

    Public Function GetColSteinBasisColor() As Color
        Dim hsb As (hue As Integer, sat As Integer, brg As Integer) = GetColSteinBasisColorHueSatBrg()
        Return New HsbInteger(hsb.hue, hsb.sat, hsb.brg, normaliszeTo361:=True).ToColorB360(Hue360Mode.Black360White361)
    End Function

    Public Function GetColLayerUp() As Color

        Dim hue As Integer
        Dim sat As Integer
        Dim brg As Integer

        Select Case _steinTypVersion
            Case SteinTypVersion.Normal
                hue = DeltaHueLayerNormal
                sat = SatLayerUpNormal
                brg = BrgLayerUpNormal
            Case SteinTypVersion.Blüten
                hue = DeltaHueLayerBlüten
                sat = SatLayerUpBlüten
                brg = BrgLayerUpBlüten
            Case SteinTypVersion.Winde
                hue = DeltaHueLayerWinde
                sat = SatLayerUpWinde
                brg = BrgLayerUpWinde
            Case SteinTypVersion.JZeiten
                hue = DeltaHueLayerJZeiten
                sat = SatLayerUpJZeiten
                brg = BrgLayerUpJZeiten
        End Select

        Dim hsb As (hue As Integer, sat As Integer, brg As Integer) = GetColSteinBasisColorHueSatBrg()
        Dim col As Color = New HsbInteger(AddHue(hsb.hue, hue), sat, brg, normaliszeTo361:=True).ToColorB360(Hue360Mode.Black360White361)
        Return GetShiftedLayerColor(col)

    End Function
    Public Function GetColLayerMidUp() As Color

        Dim hue As Integer
        Dim sat As Integer
        Dim brg As Integer

        Select Case _steinTypVersion
            Case SteinTypVersion.Normal
                hue = DeltaHueLayerNormal
                sat = SatLayerMidUpNormal
                brg = BrgLayerMidUpNormal
            Case SteinTypVersion.Blüten
                hue = DeltaHueLayerBlüten
                sat = SatLayerMidUpBlüten
                brg = BrgLayerMidUpBlüten
            Case SteinTypVersion.Winde
                hue = DeltaHueLayerWinde
                sat = SatLayerMidUpWinde
                brg = BrgLayerMidUpWinde
            Case SteinTypVersion.JZeiten
                hue = DeltaHueLayerJZeiten
                sat = SatLayerMidUpJZeiten
                brg = BrgLayerMidUpJZeiten
        End Select

        Dim hsb As (hue As Integer, sat As Integer, brg As Integer) = GetColSteinBasisColorHueSatBrg()
        Dim col As Color = New HsbInteger(AddHue(hsb.hue, hue), sat, brg, normaliszeTo361:=True).ToColorB360(Hue360Mode.Black360White361)
        Return GetShiftedLayerColor(col)

    End Function

    Public Function GetColLayerMidDn() As Color

        Dim hue As Integer
        Dim sat As Integer
        Dim brg As Integer

        Select Case _steinTypVersion
            Case SteinTypVersion.Normal
                hue = DeltaHueLayerNormal
                sat = SatLayerMidDnNormal
                brg = BrgLayerMidDnNormal
            Case SteinTypVersion.Blüten
                hue = DeltaHueLayerBlüten
                sat = SatLayerMidDnBlüten
                brg = BrgLayerMidDnBlüten
            Case SteinTypVersion.Winde
                hue = DeltaHueLayerWinde
                sat = SatLayerMidDnWinde
                brg = BrgLayerMidDnWinde
            Case SteinTypVersion.JZeiten
                hue = DeltaHueLayerJZeiten
                sat = SatLayerMidDnJZeiten
                brg = BrgLayerMidDnJZeiten
        End Select

        Dim hsb As (hue As Integer, sat As Integer, brg As Integer) = GetColSteinBasisColorHueSatBrg()

        Dim col As Color = New HsbInteger(AddHue(hsb.hue, hue), sat, brg, normaliszeTo361:=True).ToColorB360(Hue360Mode.Black360White361)
        Return GetShiftedLayerColor(col)

    End Function

    Public Function GetColLayerDn() As Color

        Dim hue As Integer
        Dim sat As Integer
        Dim brg As Integer

        Select Case _steinTypVersion
            Case SteinTypVersion.Normal
                hue = DeltaHueLayerNormal
                sat = SatLayerDnNormal
                brg = BrgLayerDnNormal
            Case SteinTypVersion.Blüten
                hue = DeltaHueLayerBlüten
                sat = SatLayerDnBlüten
                brg = BrgLayerDnBlüten
            Case SteinTypVersion.Winde
                hue = DeltaHueLayerWinde
                sat = SatLayerDnWinde
                brg = BrgLayerDnWinde
            Case SteinTypVersion.JZeiten
                hue = DeltaHueLayerJZeiten
                sat = SatLayerDnJZeiten
                brg = BrgLayerDnJZeiten
        End Select

        Dim hsb As (hue As Integer, sat As Integer, brg As Integer) = GetColSteinBasisColorHueSatBrg()

        Dim col As Color = New HsbInteger(AddHue(hsb.hue, hue), sat, brg, normaliszeTo361:=True).ToColorB360(Hue360Mode.Black360White361)
        Return GetShiftedLayerColor(col)

    End Function

    Public Function GetColLayerLine() As Color

        Dim hue As Integer
        Dim sat As Integer
        Dim brg As Integer

        Select Case _steinTypVersion
            Case SteinTypVersion.Normal
                hue = DeltaHueLayerNormal
                sat = SatLayerLineNormal
                brg = BrgLayerLineNormal
            Case SteinTypVersion.Blüten
                hue = DeltaHueLayerBlüten
                sat = SatLayerLineBlüten
                brg = BrgLayerLineBlüten
            Case SteinTypVersion.Winde
                hue = DeltaHueLayerWinde
                sat = SatLayerLineWinde
                brg = BrgLayerLineWinde
            Case SteinTypVersion.JZeiten
                hue = DeltaHueLayerJZeiten
                sat = SatLayerLineJZeiten
                brg = BrgLayerLineJZeiten
        End Select

        Dim hsb As (hue As Integer, sat As Integer, brg As Integer) = GetColSteinBasisColorHueSatBrg()
        Dim col As Color = New HsbInteger(AddHue(hsb.hue, hue), sat, brg, normaliszeTo361:=True).ToColorB360(Hue360Mode.Black360White361)
        Return GetShiftedLayerColor(col)

    End Function

    Public Function GetColSymbolFrom() As Color
        Return New HsbInteger(HueSymbolGradientFrom, SatSymbolGradientFrom, BrgSymbolGradientFrom, normaliszeTo361:=True).ToColorB360(Hue360Mode.Black360White361)
    End Function
    Public Function GetColSymbolTo() As Color
        Return New HsbInteger(HueSymbolGradientTo, SatSymbolGradientTo, BrgSymbolGradientTo, normaliszeTo361:=True).ToColorB360(Hue360Mode.Black360White361)
    End Function
    Public Function GetColSymbolOutline() As Color
        Return New HsbInteger(HueSymbolOutline, SatSymbolOutLine, BrgSymbolOutline, normaliszeTo361:=True).ToColorB360(Hue360Mode.Black360White361)
    End Function

    Public Function GetFaktorFaceFrameRadius() As Decimal
        Dim faktor As Decimal = FaktorFaceFrameRadius
        If SteinFont = SteinFont.Segoe Then
            faktor *= 1D
        Else
            faktor *= 1D
        End If
        Return faktor
    End Function
    Public Function GetFaktorSymbolOffsetLeft() As Decimal
        Dim faktor As Decimal = FaktorSymbolOffsetLeft
        If SteinFont = SteinFont.Segoe Then
            faktor *= 0.35D
        Else
            faktor *= 1D
        End If
        Return faktor
    End Function
    Public Function GetFaktorSymbolOffsetTop() As Decimal
        Dim faktor As Decimal = FaktorSymbolOffsetTop
        If SteinFont = SteinFont.Segoe Then
            faktor *= -0.24D
        Else
            faktor *= 1D
        End If
        Return faktor
    End Function
    Public Function GetFaktorSymbolOutlineWidth() As Decimal

        If _steinSize.Width < FaktorOutlineIsZeroIfTileWidthLowerAs Then
            Return 0
        Else
            Return FaktorSymbolOutlineWidth
        End If

    End Function
    Public Function GetFaktorSymbolSize() As Decimal
        Dim faktor As Decimal = FaktorSymbolSize
        If SteinFont = SteinFont.Segoe Then
            faktor *= 1.13D
        Else
            faktor *= 1D
        End If
        Return faktor
    End Function
    Public Function GetFaktorTextOffsetLeft() As Decimal
        Dim faktor As Decimal = FaktorTextOffsetLeft
        If SteinFont = SteinFont.Segoe Then
            faktor *= 2.24D
        Else
            faktor *= 1D
        End If
        Return faktor
    End Function
    Public Function GetFaktorTextOffsetTop() As Decimal
        Dim faktor As Decimal = FaktorTextOffsetTop
        If SteinFont = SteinFont.Segoe Then
            faktor *= 1.81D
        Else
            faktor *= 1D
        End If
        Return faktor
    End Function
    Public Function GetFaktorTextSize() As Decimal
        Dim faktor As Decimal = FaktorTextSize
        If SteinFont = SteinFont.Segoe Then
            faktor *= 1D
        Else
            faktor *= 1D
        End If
        Return faktor
    End Function

    Public Function GetTileBasisWidth() As Integer
        Dim value As Integer = TileBasisWidth
        Return value And Not 1
    End Function

    Public Function GetTileBasisHeight() As Integer
        Dim value As Integer = TileBasisHeight
        Return value And Not 1
    End Function

    Public Function GetTileBasisSize() As Size
        Return New Size(GetTileBasisWidth, GetTileBasisHeight)
    End Function

    Public Function GetColFaceFrame() As Color

        'GetDrawingFaceFrame beachten

        Dim sat As Integer
        Dim brg As Integer

        If DrawingFaceFrameDebugModePossible() AndAlso DebugShowFaceFrameMouse Then
            If DebugFaceFrameEditorMouseOver Then
                Dim col As Color
                If TryParseArgbHexColor(FaceFrameEditorMouseOver, col) Then
                    Return col
                Else
                    Return Color.Black
                End If
            ElseIf DebugFaceFrameEditorMouseSelected Then
                Dim col As Color
                If TryParseArgbHexColor(FaceFrameEditorMouseSelected, col) Then
                    Return col
                Else
                    Return Color.Black
                End If
            ElseIf DebugFaceFrameEditorCanDrop Then
                Dim col As Color
                If TryParseArgbHexColor(FaceFrameEditorCanDrop, col) Then
                    Return col
                Else
                    Return Color.Black
                End If
            Else
                If Debugger.IsAttached Then
                    Throw New Exception("DrawingFaceFrameDebugModePossible: Dreimal False in GetColFaceFrame")
                Else
                    Dim col As Color
                    If TryParseArgbHexColor(FaceFrameEditorMouseOver, col) Then
                        Return col
                    Else
                        Return Color.Black
                    End If
                End If
            End If
        End If

        If _steinFrameVersion = SteinFrameVersion.Standard Then

            Select Case _steinTypVersion
                Case SteinTypVersion.Normal
                    sat = SatFaceFrameNormal
                    brg = BrgFaceFrameNormal
                Case SteinTypVersion.Blüten
                    sat = SatFaceFrameBlüten
                    brg = BrgFaceFrameBlüten
                Case SteinTypVersion.Winde
                    sat = SatFaceFrameWinde
                    brg = BrgFaceFrameWinde
                Case SteinTypVersion.JZeiten
                    sat = SatFaceFrameJZeiten
                    brg = BrgFaceFrameJZeiten
            End Select

            Return New HsbInteger(HueFaceFrame, sat, brg, normaliszeTo361:=True).ToColorB360
        Else

            Select Case _steinFrameVersion
                Case SteinFrameVersion.MouseOver
                    Dim col As Color
                    If TryParseArgbHexColor(FaceFrameEditorMouseOver, col) Then
                        Return col
                    Else
                        Return Color.Black
                    End If

                Case SteinFrameVersion.MouseSelected
                    Dim col As Color
                    If TryParseArgbHexColor(FaceFrameEditorMouseSelected, col) Then
                        Return col
                    Else
                        Return Color.Black
                    End If

                Case SteinFrameVersion.MouseCanDrop
                    Dim col As Color
                    If TryParseArgbHexColor(FaceFrameEditorCanDrop, col) Then
                        Return col
                    Else
                        Return Color.Black
                    End If

            End Select

        End If

        Throw New Exception("Ungültige Enumeration in GetColFaceFrame")

    End Function

    Public Function GetInsertFaceFrame() As Boolean

        'GetColFaceFrame beachten
        If DrawingFaceFrameDebugModePossible() AndAlso DebugShowFaceFrameMouse Then
            'unabhängig der Auswahl True zurückgeben,
            'weil die Farben in GetColFaceFrame umgebogen werden.
            Return True
        End If

        If InsertFaceFramAlways Then
            Return True
        ElseIf InsertFaceFrameNever Then
            Return False
        ElseIf InsertFaceFramOnlyMouseOver Then
            If _steinFrameVersion = SteinFrameVersion.Standard Then
                Return False
            Else
                Return True
            End If
        ElseIf InsertFaceFramOnlyNormaleOne Then
            If _steinFrameVersion = SteinFrameVersion.Standard Then
                Return True
            Else
                Return False
            End If
        Else
            If Debugger.IsAttached Then
                Throw New Exception("Dreimal False in GetInsertFaceFrame")
            Else
                Return True
            End If
        End If

    End Function

    Private _drawingFaceFrameDebugModePossible As Boolean?

    Private Function DrawingFaceFrameDebugModePossible() As Boolean

        If Not _drawingFaceFrameDebugModePossible.HasValue Then
            If My.Application.Info.AssemblyName = DEBUGAPPNAME Then
                _drawingFaceFrameDebugModePossible = True
            Else
                _drawingFaceFrameDebugModePossible = False
            End If
        End If

        Return _drawingFaceFrameDebugModePossible.Value

    End Function

    Public Function GetIndexSmallCache() As CacheIndex

        Select Case _aktRenderMode
            Case AktRenderMode.Spiel
                Select Case _steinFrameVersion
                    Case SteinFrameVersion.Standard
                        Return CacheIndex.SpielStein

                    Case SteinFrameVersion.MouseOver
                        Return CacheIndex.SpielMouseOver

                    Case SteinFrameVersion.MouseSelected
                        Return CacheIndex.SpielSelected

                    Case SteinFrameVersion.MouseCanDrop
                        Return CacheIndex.SpielCanDrop
                End Select
            Case AktRenderMode.Edit
                Select Case _steinFrameVersion
                    Case SteinFrameVersion.Standard
                        Return CacheIndex.EditorStein

                    Case SteinFrameVersion.MouseOver
                        Return CacheIndex.EditorMouseOver

                    Case SteinFrameVersion.MouseSelected
                        Return CacheIndex.EditorSelected

                    Case SteinFrameVersion.MouseCanDrop
                        Return CacheIndex.EditorCanDrop
                End Select
            Case Else
        End Select
        '
        Throw New Exception("Ungültige Enumeration in GetIndexSmallCache")
        '
    End Function

    Public Structure SymbolRenderValues
        Public DonotInsertSymbol As Boolean
        Public RenderSymbolHalfSize As Boolean
        Public InsertErrorCross As Boolean
        Public Symbol As String
        Public FontFamilyName As String
        Public FontStyle As FontStyle
        Public FaktorSymbolSize As Decimal
        Public UseGradient As Boolean
        Public SymbolGradientMode As LinearGradientMode
        Public GradientColorFrom As Color
        Public GradientColorTo As Color
        Public NormalColor As Color
        Public UseOutline As Boolean
        Public OutlineColor As Color
        Public FaktorOutlineWidth As Single

    End Structure
    Public Function GetSymbolRenderValues() As SymbolRenderValues

        Dim srv As New SymbolRenderValues

        With srv
            .Symbol = ResolveSymbolText(_steinType)

            .DonotInsertSymbol = DonotInsertSymbol

            Dim coltext As String

            Select Case _steinTypVersion
                Case SteinTypVersion.Blüten
                    coltext = SymbolColBlüten

                Case SteinTypVersion.JZeiten
                    coltext = SymbolColJZeiten

                Case SteinTypVersion.Winde
                    coltext = SymbolColWinde

                Case SteinTypVersion.Normal
                    Select Case _steinType
                        Case SteinTyp.DracheG
                            coltext = SymbolColGrünerDrache

                        Case SteinTyp.DracheR
                            coltext = SymbolColRoterDrache

                        Case SteinTyp.DracheW
                            coltext = SymbolColBlauerDrache

                        Case Else
                            coltext = "00FFFFFF"

                    End Select
                Case Else
                    coltext = "00FFFFFF"

            End Select

            If Not coltext.StartsWith("00") Then
                Dim col As Color
                .UseGradient = False
                If TryParseArgbHexColor(coltext, col) Then
                    .NormalColor = col
                Else
                    .NormalColor = Color.Black
                End If
            Else
                .GradientColorFrom = GetColSymbolFrom()
                .GradientColorTo = GetColSymbolTo()
                If ColorsAreEqual(.GradientColorFrom, .GradientColorTo) Then
                    .UseGradient = False
                    .NormalColor = .GradientColorFrom
                Else
                    .UseGradient = True
                    .SymbolGradientMode = SymbolGradientMode
                End If
            End If

            .OutlineColor = GetColSymbolOutline()
            .FaktorOutlineWidth = GetFaktorSymbolOutlineWidth()

            If .OutlineColor.A > 0 AndAlso .FaktorOutlineWidth > 0 Then
                'Outline funktioniert nur mit Gradient.
                '==> ggf umstellen
                .UseOutline = True
                If Not .UseGradient Then
                    .UseGradient = True
                    .GradientColorFrom = .NormalColor
                    .GradientColorTo = .NormalColor
                End If
            Else
                .UseOutline = False
            End If

            .FaktorSymbolSize = GetFaktorSymbolSize()
            .FontStyle = FontStyle.Regular

            .FontFamilyName = GetSymbolFontFamilyName()

            Select Case _steinStatus
                Case SteinStatus.I08WerkstückEinfügeFehler
                    .RenderSymbolHalfSize = True
                    .InsertErrorCross = True
                Case SteinStatus.I09WerkstückZufallsgrafik
                    .RenderSymbolHalfSize = True
            End Select

        End With

        Return srv

    End Function

    Private ReadOnly _symbolTextBySteinTyp As _
        New Dictionary(Of SteinTyp, String) From {
            {SteinTyp.WindOst, U(&H1F000)},
            {SteinTyp.WindSüd, U(&H1F001)},
            {SteinTyp.WindWst, U(&H1F002)},
            {SteinTyp.WindNrd, U(&H1F003)},
                                           _
            {SteinTyp.DracheR, U(&H1F004)},
            {SteinTyp.DracheG, U(&H1F005)},
            {SteinTyp.DracheW, U(&H1F006)},
                                           _
            {SteinTyp.Symbol1, U(&H1F007)},
            {SteinTyp.Symbol2, U(&H1F008)},
            {SteinTyp.Symbol3, U(&H1F009)},
            {SteinTyp.Symbol4, U(&H1F00A)},
            {SteinTyp.Symbol5, U(&H1F00B)},
            {SteinTyp.Symbol6, U(&H1F00C)},
            {SteinTyp.Symbol7, U(&H1F00D)},
            {SteinTyp.Symbol8, U(&H1F00E)},
            {SteinTyp.Symbol9, U(&H1F00F)},
                                           _
            {SteinTyp.Bambus1, U(&H1F010)},
            {SteinTyp.Bambus2, U(&H1F011)},
            {SteinTyp.Bambus3, U(&H1F012)},
            {SteinTyp.Bambus4, U(&H1F013)},
            {SteinTyp.Bambus5, U(&H1F014)},
            {SteinTyp.Bambus6, U(&H1F015)},
            {SteinTyp.Bambus7, U(&H1F016)},
            {SteinTyp.Bambus8, U(&H1F017)},
            {SteinTyp.Bambus9, U(&H1F018)},
                                           _
            {SteinTyp.Punkt01, U(&H1F019)},
            {SteinTyp.Punkt02, U(&H1F01A)},
            {SteinTyp.Punkt03, U(&H1F01B)},
            {SteinTyp.Punkt04, U(&H1F01C)},
            {SteinTyp.Punkt05, U(&H1F01D)},
            {SteinTyp.Punkt06, U(&H1F01E)},
            {SteinTyp.Punkt07, U(&H1F01F)},
            {SteinTyp.Punkt08, U(&H1F020)},
            {SteinTyp.Punkt09, U(&H1F021)},
                                           _
            {SteinTyp.BlütePf, U(&H1F022)},
            {SteinTyp.BlüteOr, U(&H1F023)},
            {SteinTyp.BlüteCt, U(&H1F024)},
            {SteinTyp.BlüteBa, U(&H1F025)},
                                           _
            {SteinTyp.JahrFrl, U(&H1F026)},
            {SteinTyp.JahrSom, U(&H1F027)},
            {SteinTyp.JahrHer, U(&H1F028)},
            {SteinTyp.JahrWin, U(&H1F029)}
        }

    Private Function U(codePoint As Integer) As String
        Return Char.ConvertFromUtf32(codePoint)
    End Function

    Private Function ResolveSymbolText(steinTyp As SteinTyp) As String

        Dim result As String = Nothing

        If _symbolTextBySteinTyp.TryGetValue(steinTyp, result) Then
            Return result
        End If

        Return String.Empty

    End Function

    Public Structure TextRenderValues
        Public HasText As Boolean
        Public Text As String
        Public FontFamilyName As String
        Public FontStyle As FontStyle
        Public FaktorTextSize As Decimal
        Public TextColor As Color
    End Structure

    Public Function GetTextRenderValues() As TextRenderValues

        'Zeichensatz für Arial "NSOW▲*+!"
        'Zeichensatz für "NSOW▲✿✱!"
        Dim trv As New TextRenderValues

        Dim useArial As Boolean = False 'Default

        With trv
            Dim txtSymb As String = TextSymbole
            If String.IsNullOrEmpty(txtSymb) OrElse txtSymb.Length <> 8 Then
                txtSymb = "NSOWDBJ!"
                useArial = True
            ElseIf Not TileFactoryINISettings.Tile_TextUseSegoeUISymbol Then
                txtSymb = "NSOWDBJ!"
                useArial = True
            End If

            .Text = String.Empty 'Default

            Select Case _steinTypVersion
                Case SteinTypVersion.Blüten
                    If InsertTextBlumen Then .Text = txtSymb.Substring(5, 1)
                Case SteinTypVersion.JZeiten
                    If InsertTextJZeiten Then .Text = txtSymb.Substring(6, 1)
                Case SteinTypVersion.Winde
                    useArial = True
                    Select Case _steinType
                        Case SteinTyp.WindOst
                            If InsertTextWinde Then .Text = txtSymb.Substring(3, 1)
                        Case SteinTyp.WindNrd
                            If InsertTextWinde Then .Text = txtSymb.Substring(0, 1)
                        Case SteinTyp.WindSüd
                            If InsertTextWinde Then .Text = txtSymb.Substring(1, 1)
                        Case SteinTyp.WindWst
                            If InsertTextWinde Then .Text = txtSymb.Substring(3, 1)
                    End Select
                Case SteinTypVersion.Normal
                    Select Case _steinType
                        Case SteinTyp.DracheG
                            If InsertTextDrachen Then .Text = txtSymb.Substring(4, 1)
                        Case SteinTyp.DracheR
                            If InsertTextDrachen Then .Text = txtSymb.Substring(4, 1)
                        Case SteinTyp.DracheW
                            If InsertTextDrachen Then .Text = txtSymb.Substring(4, 1)
                    End Select
            End Select

            If _steinStatus = SteinStatus.I07MissingSecond Then
                .Text &= txtSymb.Substring(7, 1)
            End If

            Dim col As Color
            If TryParseArgbHexColor(TextColor, col) Then
                .TextColor = col
            Else
                .TextColor = Color.Black
            End If

            .FaktorTextSize = GetFaktorTextSize()
            If useArial Then
                .FontFamilyName = "Arial"
            Else
                If _steinTypVersion = SteinTypVersion.Winde Then
                    'Das sind die Buchstaben, die in Arial besser sind
                    .FontFamilyName = "Arial"
                Else
                    .FontFamilyName = "Segoe UI Symbol" '"Segoe UI Emoji"
                End If
            End If

            'If _steinTypVersion = SteinTypVersion.Winde Then
            '    .FontStyle = FontStyle.Bold
            'Else
            '    .FontStyle = FontStyle.Regular
            'End If
            .FontStyle = If(TextFontStyleBold, FontStyle.Bold, FontStyle.Regular)

            .HasText = .Text <> String.Empty

            If DonotInsertSymbol Then
                .HasText = False
            End If

        End With

        Return trv

    End Function

    '
    ''' <summary>
    ''' Spezialfall der Addition.
    ''' Wenn hue >= 360 ist, ist damit schwarz (360) oder weiß (361 oder größer) gemeint.
    ''' Diese Werte werden unverändert zurückgegeben. Alle anderen werden um deltaHue
    ''' geändert und dann ggf. normalisiert.
    ''' </summary>
    ''' <param name="hue"></param>
    ''' <param name="deltaHue"></param>
    ''' <returns></returns>
    Private Function AddHue(hue As Integer, deltaHue As Integer) As Integer

        If hue >= 360 Then
            Return hue
        End If

        hue += deltaHue
        If hue > 360 Then
            hue -= 360
        ElseIf hue < 0 Then
            hue += 360
        End If

        Return hue

    End Function

    'Public Function GetFaceLightMap() As LightMap
    '    Select Case _steinTypVersion
    '        Case SteinTypVersion.Normal
    '            Return FaceLightMapNormal
    '        Case SteinTypVersion.Winde
    '            Return FaceLightMapSelected
    '        Case SteinTypVersion.Blüten
    '            Return FaceLightMapSelectable
    '        Case SteinTypVersion.JZeiten
    '            Return FaceLightMapRemovable
    '        Case Else
    '            Return FaceLightMapNormal
    '    End Select
    'End Function

    Public Function GetFaceLightMap() As LightMap
        Select Case _steinStatus
            Case SteinStatus.I01Normal
                Return FaceLightMapNormal
            Case SteinStatus.I02Selected
                Return FaceLightMapSelected
            Case SteinStatus.I03Selectable
                Return FaceLightMapSelectable
            Case SteinStatus.I04Removable
                Return FaceLightMapRemovable
            Case Else
                Return FaceLightMapNormal
        End Select
    End Function

    Public Structure FaceTexturValues
        Public useDifferentInnerArea As Boolean
        Public innerCloudStrength As Single
        Public innerGrainStrength As Single
        Public outerCloudStrength As Single
        Public outerGrainStrength As Single
        Public seed As Integer?
        Public steinStatus As SteinStatus
        Public Shadows ReadOnly Property ToString As String
            Get
                Dim s As String
                If seed.HasValue Then
                    s = CBool(seed.Value).ToString
                Else
                    s = "Nothing"
                End If

                Return $"OuterCloud={outerCloudStrength}, OuterGrain={outerGrainStrength}, InnerCloud={innerCloudStrength}, InnerGrain={innerGrainStrength}, UseInner={useDifferentInnerArea}, seed={s}"
            End Get
        End Property

    End Structure
    Public Function GetFaceTexturValues() As FaceTexturValues

        Dim ftv As New FaceTexturValues

        'Note: Debughelfer: negativen Seed zuweisen für immer gleiche Muster
        'ftv.seed = -1

        ftv.steinStatus = _steinStatus 'nur zum Debuggen

        With ftv
            Select Case _steinStatus
                Case SteinStatus.I02Selected
                    .outerCloudStrength = FaceACloudSteinStatusSelected
                    .outerGrainStrength = FaceAGrainSteinStatusSelected
                    .innerCloudStrength = FaceICloudSteinStatusSelected
                    .innerGrainStrength = FaceIGrainSteinStatusSelected
                    .useDifferentInnerArea = FaceUseInnerAreaSteinStatusSelected

                Case SteinStatus.I03Selectable
                    .outerCloudStrength = FaceACloudSteinStatusSelectable
                    .outerGrainStrength = FaceAGrainSteinStatusSelectable
                    .innerCloudStrength = FaceICloudSteinStatusSelectable
                    .innerGrainStrength = FaceIGrainSteinStatusSelectable
                    .useDifferentInnerArea = FaceUseInnerAreaSteinStatusSelectable

                Case SteinStatus.I04Removable
                    .outerCloudStrength = FaceACloudSteinStatusRemovable
                    .outerGrainStrength = FaceAGrainSteinStatusRemovable
                    .innerCloudStrength = FaceICloudSteinStatusRemovable
                    .innerGrainStrength = FaceIGrainSteinStatusRemovable
                    .useDifferentInnerArea = FaceUseInnerAreaSteinStatusRemovable

                Case Else
                    'SteinStatus.I01Normal und alle anderen
                    .outerCloudStrength = FaceACloudSteinStatusNormal
                    .outerGrainStrength = FaceAGrainSteinStatusNormal
                    .innerCloudStrength = FaceICloudSteinStatusNormal
                    .innerGrainStrength = FaceIGrainSteinStatusNormal
                    .useDifferentInnerArea = FaceUseInnerAreaSteinStatusNormal

            End Select
        End With

        Return ftv

    End Function

    Public Function GetLayerTexturValues() As FaceTexturValues

        Dim ftv As New FaceTexturValues

        'Note: Debughelfer: negativen Seed zuweisen für immer gleiche Muster
        'ftv.seed = -1

        ftv.steinStatus = _steinStatus 'nur zum Debuggen
        With ftv

            .outerCloudStrength = LayerACloud
            .outerGrainStrength = LayerAGrain
            .innerCloudStrength = 0
            .innerGrainStrength = 0
            .useDifferentInnerArea = False

        End With

        Return ftv

    End Function

#End Region

#Region "ShiftXxx"

    Private Sub CreateShiftLightMapSteinStatusNormal(shift As Integer)
        CreateShiftLightMap(index:=SteinStatus.I01Normal, shift)
    End Sub
    Private Sub CreateShiftLightMapSteinStatusSelected(shift As Integer)
        CreateShiftLightMap(index:=SteinStatus.I02Selected, shift)
    End Sub
    Private Sub CreateShiftLightMapSteinStatusSelectable(shift As Integer)
        CreateShiftLightMap(index:=SteinStatus.I03Selectable, shift)
    End Sub
    Private Sub CreateShiftLightMapSteinStatusRemovable(shift As Integer)
        CreateShiftLightMap(index:=SteinStatus.I04Removable, shift)
    End Sub
    '
    '
    Private Sub CreateShiftLayerNormalNormal(shift As Integer)
        CreateShiftLayer(SteinStatus.I01Normal, SteinTypVersion.Normal, shift)
    End Sub
    Private Sub CreateShiftLayerNormalBlüten(shift As Integer)
        CreateShiftLayer(SteinStatus.I01Normal, SteinTypVersion.Blüten, shift)
    End Sub
    Private Sub CreateShiftLayerNormalWinde(shift As Integer)
        CreateShiftLayer(SteinStatus.I01Normal, SteinTypVersion.Winde, shift)
    End Sub
    Private Sub CreateShiftLayerNormalJZeiten(shift As Integer)
        CreateShiftLayer(SteinStatus.I01Normal, SteinTypVersion.JZeiten, shift)
    End Sub
    '
    '
    Private Sub CreateShiftLayerSelectedNormal(shift As Integer)
        CreateShiftLayer(SteinStatus.I02Selected, SteinTypVersion.Normal, shift)
    End Sub
    Private Sub CreateShiftLayerSelectedBlüten(shift As Integer)
        CreateShiftLayer(SteinStatus.I02Selected, SteinTypVersion.Blüten, shift)
    End Sub
    Private Sub CreateShiftLayerSelectedWinde(shift As Integer)
        CreateShiftLayer(SteinStatus.I02Selected, SteinTypVersion.Winde, shift)
    End Sub
    Private Sub CreateShiftLayerSelectedJZeiten(shift As Integer)
        CreateShiftLayer(SteinStatus.I02Selected, SteinTypVersion.JZeiten, shift)
    End Sub
    '
    '
    Private Sub CreateShiftLayerSelectableNormal(shift As Integer)
        CreateShiftLayer(SteinStatus.I03Selectable, SteinTypVersion.Normal, shift)
    End Sub
    Private Sub CreateShiftLayerSelectableBlüten(shift As Integer)
        CreateShiftLayer(SteinStatus.I03Selectable, SteinTypVersion.Blüten, shift)
    End Sub
    Private Sub CreateShiftLayerSelectableWinde(shift As Integer)
        CreateShiftLayer(SteinStatus.I03Selectable, SteinTypVersion.Winde, shift)
    End Sub
    Private Sub CreateShiftLayerSelectableJZeiten(shift As Integer)
        CreateShiftLayer(SteinStatus.I03Selectable, SteinTypVersion.JZeiten, shift)
    End Sub
    '
    '
    Private Sub CreateShiftLayerRemovableNormal(shift As Integer)
        CreateShiftLayer(SteinStatus.I04Removable, SteinTypVersion.Normal, shift)
    End Sub
    Private Sub CreateShiftLayerRemovableBlüten(shift As Integer)
        CreateShiftLayer(SteinStatus.I04Removable, SteinTypVersion.Blüten, shift)
    End Sub
    Private Sub CreateShiftLayerRemovableWinde(shift As Integer)
        CreateShiftLayer(SteinStatus.I04Removable, SteinTypVersion.Winde, shift)
    End Sub
    Private Sub CreateShiftLayerRemovableJZeiten(shift As Integer)
        CreateShiftLayer(SteinStatus.I04Removable, SteinTypVersion.JZeiten, shift)
    End Sub
    '
    '
    Private Sub CreateDShiftNormalBlüten(shift As Integer)
        CreateDShift(steinStatus:=SteinStatus.I01Normal, steinTypVersion:=SteinTypVersion.Blüten, shift)
    End Sub
    Private Sub CreateDShiftNormalJZeiten(shift As Integer)
        CreateDShift(steinStatus:=SteinStatus.I01Normal, steinTypVersion:=SteinTypVersion.JZeiten, shift)
    End Sub
    Private Sub CreateDShiftNormalNormal(shift As Integer)
        CreateDShift(steinStatus:=SteinStatus.I01Normal, steinTypVersion:=SteinTypVersion.Normal, shift)
    End Sub
    Private Sub CreateDShiftNormalWinde(shift As Integer)
        CreateDShift(steinStatus:=SteinStatus.I01Normal, steinTypVersion:=SteinTypVersion.Winde, shift)
    End Sub
    Private Sub CreateDShiftRemovableBlüten(shift As Integer)
        CreateDShift(steinStatus:=SteinStatus.I04Removable, steinTypVersion:=SteinTypVersion.Blüten, shift)
    End Sub
    Private Sub CreateDShiftRemovableJZeiten(shift As Integer)
        CreateDShift(steinStatus:=SteinStatus.I04Removable, steinTypVersion:=SteinTypVersion.JZeiten, shift)
    End Sub
    Private Sub CreateDShiftRemovableNormal(shift As Integer)
        CreateDShift(steinStatus:=SteinStatus.I04Removable, steinTypVersion:=SteinTypVersion.Normal, shift)
    End Sub
    Private Sub CreateDShiftRemovableWinde(shift As Integer)
        CreateDShift(steinStatus:=SteinStatus.I04Removable, steinTypVersion:=SteinTypVersion.Winde, shift)
    End Sub
    Private Sub CreateDShiftSelectableBlüten(shift As Integer)
        CreateDShift(steinStatus:=SteinStatus.I03Selectable, steinTypVersion:=SteinTypVersion.Blüten, shift)
    End Sub
    Private Sub CreateDShiftSelectableJZeiten(shift As Integer)
        CreateDShift(steinStatus:=SteinStatus.I03Selectable, steinTypVersion:=SteinTypVersion.JZeiten, shift)
    End Sub
    Private Sub CreateDShiftSelectableNormal(shift As Integer)
        CreateDShift(steinStatus:=SteinStatus.I03Selectable, steinTypVersion:=SteinTypVersion.Normal, shift)
    End Sub
    Private Sub CreateDShiftSelectableWinde(shift As Integer)
        CreateDShift(steinStatus:=SteinStatus.I03Selectable, steinTypVersion:=SteinTypVersion.Winde, shift)
    End Sub
    Private Sub CreateDShiftSelectedBlüten(shift As Integer)
        CreateDShift(steinStatus:=SteinStatus.I02Selected, steinTypVersion:=SteinTypVersion.Blüten, shift)
    End Sub
    Private Sub CreateDShiftSelectedJZeiten(shift As Integer)
        CreateDShift(steinStatus:=SteinStatus.I02Selected, steinTypVersion:=SteinTypVersion.JZeiten, shift)
    End Sub
    Private Sub CreateDShiftSelectedNormal(shift As Integer)
        CreateDShift(steinStatus:=SteinStatus.I02Selected, steinTypVersion:=SteinTypVersion.Normal, shift)
    End Sub
    Private Sub CreateDShiftSelectedWinde(shift As Integer)
        CreateDShift(steinStatus:=SteinStatus.I02Selected, steinTypVersion:=SteinTypVersion.Winde, shift)
    End Sub

    Private _shiftLightMapLookup(3, 255) As Integer
    Private _dShiftLightMapLookup(3, 3, 255) As Integer
    Private _shiftLayerLookup(3, 3, 255) As Integer

    Private Sub CreateShiftLightMap(index As Integer, shift As Integer)
        Dim arr() As Integer = CreateLinearGrayLookup(shift)
        For idx As Integer = 0 To 255
            _shiftLightMapLookup(index - 1, idx) = arr(idx)
        Next
    End Sub
    Private Sub CreateShiftLayer(steinStatus As SteinStatus, steinTypVersion As SteinTypVersion, shift As Integer)
        Dim idxSteinStatus As Integer = GetSteinStatusForLookup(steinStatus)
        Dim idxSteinTypVersion As Integer = steinTypVersion

        Dim arr() As Integer = CreateLinearGrayLookup(shift)
        For idxLookUp As Integer = 0 To 255
            _shiftLayerLookup(idxSteinStatus, idxSteinTypVersion, idxLookUp) = arr(idxLookUp)
        Next

    End Sub

    Private Sub CreateDShift(steinStatus As SteinStatus, steinTypVersion As SteinTypVersion, shift As Integer)

        Dim idxSteinStatus As Integer = GetSteinStatusForLookup(steinStatus)
        Dim idxSteinTypVersion As Integer = steinTypVersion

        Dim arr() As Integer = CreateLinearGrayLookup(shift)
        For idxLookUp As Integer = 0 To 255
            _dShiftLightMapLookup(idxSteinStatus, idxSteinTypVersion, idxLookUp) = arr(idxLookUp)
        Next

    End Sub

    Private Function CreateLinearGrayLookup(shift As Integer) As Integer()

        Dim result(255) As Integer
        Dim shiftClamped As Integer = Math.Max(-100, Math.Min(100, shift))

        Dim y0 As Double
        Dim y255 As Double

        If shiftClamped >= 0 Then
            y0 = shiftClamped / 100.0R * 255.0R
            y255 = 255.0R
        Else
            y0 = 0.0R
            y255 = 255.0R + (shiftClamped / 100.0R * 255.0R)
        End If

        For i As Integer = 0 To 255
            Dim value As Integer = CInt(Math.Round(
            y0 + ((y255 - y0) * i / 255.0R),
            MidpointRounding.AwayFromZero))

            If value < 0 Then
                value = 0
            ElseIf value > 255 Then
                value = 255
            End If

            result(i) = value
        Next

        Return result

    End Function
    '
    '
    ''' <summary>
    ''' Direkter Zugriff auf die aktuelle LookUp.
    ''' </summary>
    ''' <param name="inValue"></param>
    ''' <returns></returns>
    Public Function GetShiftLightMapLookupValue(inValue As Integer) As Integer

        Dim newInValue As Integer = _shiftLightMapLookup(_idxSteinStatusForLookup, inValue)

        Return _dShiftLightMapLookup(_idxSteinStatusForLookup, _idxSteinStatusForLookup, newInValue)

    End Function

    Private Function GetShiftedLayerColor(color As Color) As Color

        Dim a As Integer = CInt(color.A)
        Dim r As Integer = CInt(color.R)
        Dim g As Integer = CInt(color.G)
        Dim b As Integer = CInt(color.B)

        Dim oldMax As Integer = Math.Max(r, Math.Max(g, b))
        If oldMax <= 0 Then
            Return color
        End If

        Dim newMax As Integer = CInt(_shiftLayerLookup(_idxSteinStatusForLookup, _steinTypVersion, oldMax))

        Dim rNew As Integer = (r * newMax + (oldMax \ 2)) \ oldMax
        Dim gNew As Integer = (g * newMax + (oldMax \ 2)) \ oldMax
        Dim bNew As Integer = (b * newMax + (oldMax \ 2)) \ oldMax

        Return Color.FromArgb(a, rNew, gNew, bNew)

    End Function

#End Region

#Region "Load/Save - Syncronisation"

    Private Const MAX_BACKUP_FILES As Integer = 20
    Private Shared _runtimeTileColorsSynchronized As Boolean = False

    Private Shared Function IsTileFactoryTester() As Boolean

        Return String.Equals(My.Application.Info.AssemblyName,
                             "MahjongGKTileFactoryTester",
                             StringComparison.Ordinal)

    End Function

    Public Shared Function EnsureRuntimeTileColorsSynchronized(dontOverwriteExistingTileColorsFiles As Boolean) As Boolean

        If IsTileFactoryTester() Then
            Return True
        End If

        If _runtimeTileColorsSynchronized Then
            Return True
        End If

        If Not TrySynchronizeRuntimeTileColors(dontOverwriteExistingTileColorsFiles) Then
            Return False
        End If

        _runtimeTileColorsSynchronized = True
        Return True

    End Function

    Private Shared Function TrySynchronizeRuntimeTileColors(dontOverwriteExistingTileColorsFiles As Boolean) As Boolean

        Dim sourceRoot As String = GetRuntimeTileColorsRoot()
        Dim targetRoot As String = GetMahjongGKUserTileColorsRoot()

        Try
            If Not Directory.Exists(sourceRoot) Then
                Return True
            End If

            Directory.CreateDirectory(targetRoot)

            Dim sourceFiles() As String =
                Directory.GetFiles(sourceRoot, "*.xml", SearchOption.AllDirectories)

            For Each sourceFile As String In sourceFiles

                Dim relativePart As String =
                    sourceFile.Substring(sourceRoot.Length).TrimStart(Path.DirectorySeparatorChar,
                                                                      Path.AltDirectorySeparatorChar)

                Dim targetFile As String = Path.Combine(targetRoot, relativePart)
                Dim targetFolder As String = Path.GetDirectoryName(targetFile)

                If String.IsNullOrWhiteSpace(targetFolder) Then
                    Throw New InvalidOperationException("Zielverzeichnis konnte nicht bestimmt werden: " & targetFile)
                End If

                Directory.CreateDirectory(targetFolder)

                If MustCopyTileColorFile(sourceFile, targetFile, dontOverwriteExistingTileColorsFiles) Then
                    File.Copy(sourceFile, targetFile, True)

                    ' Quellzeit übernehmen, damit der Vergleich stabil bleibt.
                    File.SetLastWriteTimeUtc(targetFile, File.GetLastWriteTimeUtc(sourceFile))
                End If
            Next

            Return True

        Catch ex As Exception When TypeOf ex Is IOException OrElse
                                   TypeOf ex Is UnauthorizedAccessException OrElse
                                   TypeOf ex Is InvalidOperationException OrElse
                                   TypeOf ex Is ArgumentException OrElse
                                   TypeOf ex Is NotSupportedException

            Dim sb As New StringBuilder
            sb.AppendLine("Die Mahjong-Steinbeschreibungen konnten nicht in den Benutzerordner kopiert werden.")
            sb.AppendLine("Quellordner:")
            sb.AppendLine(sourceRoot)
            sb.AppendLine()
            sb.AppendLine("Zielordner:")
            sb.AppendLine(targetRoot)
            sb.AppendLine()
            sb.AppendLine("Grund: " & ex.Message)
            sb.AppendLine()
            sb.AppendLine("Der Programmstart wird abgebrochen.")

            MsgBox(sb.ToString(), MsgBoxStyle.Critical Or MsgBoxStyle.OkOnly, "MahjongGK")
            Return False
        End Try

    End Function

    Private Shared Function MustCopyTileColorFile(sourceFile As String,
                                                  targetFile As String,
                                                  dontOverwriteExistingTileColorsFiles As Boolean) As Boolean

        If Not File.Exists(targetFile) Then
            Return True
        End If

        If dontOverwriteExistingTileColorsFiles Then
            Return False
        End If

        Dim sourceInfo As New FileInfo(sourceFile)
        Dim targetInfo As New FileInfo(targetFile)

        If sourceInfo.Length <> targetInfo.Length Then
            Return True
        End If

        If sourceInfo.LastWriteTimeUtc <> targetInfo.LastWriteTimeUtc Then
            Return True
        End If

        Return False

    End Function

    Public Shared Function FileExists(steinDesign As SteinDesign,
                                steinSatz As SteinSatz,
                                steinFont As SteinFont,
                                Optional useDevelopmentPath As Boolean = False) As Boolean
        Dim fullPath As String =
          GetFullPathOnlyForLoading(steinDesign, steinSatz, steinFont, useDevelopmentPath)

        Return System.IO.File.Exists(fullPath)

    End Function
    Public Shared Function Load(steinDesign As SteinDesign,
                                steinSatz As SteinSatz,
                                steinFont As SteinFont,
                                Optional useDevelopmentPath As Boolean = False) As TileColors

        If useDevelopmentPath AndAlso Not IsTileFactoryTester() Then
            Throw New InvalidOperationException("Unerlaubter Aufruf. UseDevelopmentPath = True ist nur aus dem MahjongGKTileFactoryTester erlaubt.")
        End If

        Dim fullPath As String =
            GetFullPathOnlyForLoading(steinDesign, steinSatz, steinFont, useDevelopmentPath)

        If String.IsNullOrWhiteSpace(fullPath) Then
            Return DefaultTileColors(steinDesign, steinSatz, steinFont, useDevelopmentPath)
        End If

        If Not File.Exists(fullPath) Then
            Return DefaultTileColors(steinDesign, steinSatz, steinFont, useDevelopmentPath)
        End If

        Try
            Dim serializer As New XmlSerializer(GetType(TileColors))

            Using fs As New FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read)
                Dim result As TileColors = CType(serializer.Deserialize(fs), TileColors)

                If result Is Nothing Then
                    Dim sb As New StringBuilder
                    sb.AppendLine("Folgende Mahjong-Steinbeschreibung konnte nicht geladen werden:")
                    sb.AppendLine(fullPath)
                    sb.AppendLine("Grund: unbekannt.")
                    sb.AppendLine()
                    sb.AppendLine("Es wird ein Standard-Design geladen.")

                    MsgBox(sb.ToString(), MsgBoxStyle.Exclamation Or MsgBoxStyle.OkOnly, "MahjongGK")
                    Return DefaultTileColors(steinDesign, steinSatz, steinFont, useDevelopmentPath)
                End If

                'sicherheitshalber immer rückstellen. Nur zu Debugzwecken im FileFactoryTester da.
                result.DonotInsertSymbol = False
                result.StatusLoadingOK = True

                Return result
            End Using

        Catch ex As Exception When TypeOf ex Is IOException OrElse
                                   TypeOf ex Is UnauthorizedAccessException OrElse
                                   TypeOf ex Is InvalidOperationException

            Dim sb As New StringBuilder
            sb.AppendLine("Folgende Mahjong-Steinbeschreibung konnte nicht geladen werden:")
            sb.AppendLine(fullPath)
            sb.AppendLine("Grund: " & ex.Message)
            sb.AppendLine()
            sb.AppendLine("Es wird ein Standard-Design geladen.")

            MsgBox(sb.ToString(), MsgBoxStyle.Exclamation Or MsgBoxStyle.OkOnly, "MahjongGK")
            Return DefaultTileColors(steinDesign, steinSatz, steinFont, useDevelopmentPath)
        End Try

    End Function

    '
    ''' <summary>
    ''' Die Funktion Save, und damit GetFullPathOnlyForSaving darf ausschließlich aus dem Programm
    ''' MahjongGKTileFactoryTester heraus aufgerufen werden.
    ''' </summary>
    ''' <param name="useDevelopmentPath"></param>
    ''' <param name="insertDateTime"></param>
    ''' <returns></returns>
    Public Function GetFullPathOnlyForSaving(useDevelopmentPath As Boolean,
                                             Optional insertDateTime As Boolean = False) As String

        If Not IsTileFactoryTester() Then
            Throw New InvalidOperationException("Unerlaubter Aufruf. Nur aus MahjongGKTileFactoryTester erlaubt.")
        End If

        If CInt(_steinDesign) = -1 Then Throw New InvalidOperationException("Programmierfehler: SteinDesign nicht zugewiesen.")
        If CInt(_steinSatz) = -1 Then Throw New InvalidOperationException("Programmierfehler: SteinSatz nicht zugewiesen.")
        If CInt(_SteinFont) = -1 Then Throw New InvalidOperationException("Programmierfehler: SteinFont nicht zugewiesen.")

        'Bei Änderungen die andere Überladung auch ändern!

        Dim rootPath As String

        If useDevelopmentPath Then
            rootPath = GetTesterTileColorsRoot()
        Else
            rootPath = GetProjectTileColorsRoot()
        End If

        Dim pathPart As String = Path.Combine(rootPath, _steinDesign.ToString())

        Dim namePart As String = _SteinFont.ToString() & "-"

        If insertDateTime Then
            namePart &= Date.Now.ToString("yyyy-MM-dd-HH-mm-ss-fff", CultureInfo.InvariantCulture) & "-"
        End If

        namePart &= _steinSatz.ToString()
        namePart &= ".xml"

        Try
            Directory.CreateDirectory(pathPart)

        Catch ex As Exception When TypeOf ex Is IOException OrElse
                                   TypeOf ex Is UnauthorizedAccessException OrElse
                                   TypeOf ex Is ArgumentException OrElse
                                   TypeOf ex Is NotSupportedException

            Dim sb As New StringBuilder
            sb.AppendLine("Folgendes Verzeichnis konnte nicht angelegt werden:")
            sb.AppendLine(pathPart)
            sb.AppendLine("Grund: " & ex.Message)
            sb.AppendLine()
            sb.AppendLine("Der Vorgang wird abgebrochen.")

            MsgBox(sb.ToString(), MsgBoxStyle.Critical Or MsgBoxStyle.OkOnly, "MahjongGK")
            Return String.Empty
        End Try

        Return Path.Combine(pathPart, namePart)

    End Function
    '
    ''' <summary>
    ''' Nur für Infozwecke!
    ''' </summary>
    ''' <param name="steinDesign"></param>
    ''' <param name="steinSatz"></param>
    ''' <param name="steinFont"></param>
    ''' <param name="useDevelopmentPath"></param>
    ''' <returns></returns>
    Public Shared Function GetFullPathOnlyForSaving(steinDesign As SteinDesign,
                                steinSatz As SteinSatz,
                                steinFont As SteinFont,
                                useDevelopmentPath As Boolean) As String

        If Not IsTileFactoryTester() Then
            Throw New InvalidOperationException("Unerlaubter Aufruf. Nur aus MahjongGKTileFactoryTester erlaubt.")
        End If

        'Bei Änderungen die andere Überladung auch ändern!

        Dim rootPath As String

        If useDevelopmentPath Then
            rootPath = GetTesterTileColorsRoot()
        Else
            rootPath = GetProjectTileColorsRoot()
        End If

        Dim pathPart As String = Path.Combine(rootPath, steinDesign.ToString())

        Dim namePart As String = steinFont.ToString() & "-"

        namePart &= steinSatz.ToString()
        namePart &= ".xml"

        Return Path.Combine(pathPart, namePart)

    End Function

    Public Shared Function GetFullPathOnlyForLoading(steinDesign As SteinDesign,
                                                     steinSatz As SteinSatz,
                                                     steinFont As SteinFont,
                                                     useDevelopmentPath As Boolean) As String

        If CInt(steinFont) = -1 Then
            Throw New Exception("steinFont nicht zugewiesen.")
        End If
        Dim rootPath As String

        If useDevelopmentPath Then
            rootPath = GetTesterTileColorsRoot()

        ElseIf IsTileFactoryTester() Then
            ' Der TileFactoryTester lädt bei useDevelopmentPath = False direkt aus dem Projektordner.
            rootPath = GetProjectTileColorsRoot()

        Else
            ' MahjongGK lädt aus dem Benutzerordner.
            rootPath = GetMahjongGKUserTileColorsRoot()
        End If

        Dim pathPart As String = Path.Combine(rootPath, steinDesign.ToString())

        Dim namePart As String = steinFont.ToString() & "-"

        ''Das hier kommt nie vor. Die Varianten mit Datum sind ein automatisches Backup
        ''nach Änderungen, die nur im TileFaktoryTester möglich sind, und das
        ''derzeit nur zum manuellen Restore gedacht ist.
        ''If insertDateTime Then
        ''    namePart &= Date.Now.ToString("yyyy-MM-dd-HH-mm-ss-fff", CultureInfo.InvariantCulture) & "-"
        ''End If

        namePart &= steinSatz.ToString()
        namePart &= ".xml"

        Try
            Directory.CreateDirectory(pathPart)

        Catch ex As Exception When TypeOf ex Is IOException OrElse
                                   TypeOf ex Is UnauthorizedAccessException OrElse
                                   TypeOf ex Is ArgumentException OrElse
                                   TypeOf ex Is NotSupportedException

            Dim sb As New StringBuilder
            sb.AppendLine("Auf folgendes Verzeichnis konnte nicht zugegriffen werden:")
            sb.AppendLine(pathPart)
            sb.AppendLine("Grund: " & ex.Message)
            sb.AppendLine()
            sb.AppendLine("Es wird ein Standard-Design geladen.")

            MsgBox(sb.ToString(), MsgBoxStyle.Critical Or MsgBoxStyle.OkOnly, "MahjongGK")
            Return String.Empty
        End Try

        Return Path.Combine(pathPart, namePart)

    End Function

    Public Shared Function DefaultTileColors(steinDesign As SteinDesign,
                                             steinSatz As SteinSatz,
                                             steinFont As SteinFont,
                                             useDevelopmentPath As Boolean) As TileColors
        Dim result As New TileColors()

        With result
            .SteinDesign = steinDesign
            .SteinSatz = steinSatz
            .SteinFont = steinFont
            .UseDevelopmentPath = useDevelopmentPath
            ._hashLastStorage = .GetMyHash()
            .StatusLoadingOK = False
        End With

        Return result
    End Function

    Public Sub Save(useDevelopmentPath As Boolean)

        Dim filePath As String = String.Empty
        Dim tempPath As String = String.Empty
        Dim backupPath As String = String.Empty

        Try
            Me.UseDevelopmentPath = useDevelopmentPath

            filePath = Me.GetFullPathOnlyForSaving(useDevelopmentPath, insertDateTime:=False)

            If String.IsNullOrWhiteSpace(filePath) Then
                Exit Sub
            End If

            tempPath = filePath & ".tmp"

            Dim serializer As New XmlSerializer(GetType(TileColors))
            Dim settings As New XmlWriterSettings() With {
                .Indent = True,
                .Encoding = New UTF8Encoding(False)
            }

            Using fs As New FileStream(tempPath, FileMode.Create, FileAccess.Write, FileShare.None)
                Using writer As XmlWriter = XmlWriter.Create(fs, settings)
                    serializer.Serialize(writer, Me)
                End Using
            End Using

            If File.Exists(filePath) Then
                backupPath = EnsureUniqueBackupFilePath(Me.GetFullPathOnlyForSaving(useDevelopmentPath, insertDateTime:=True))

                ' tempPath, filePath und backupPath liegen im gleichen Ordner.
                File.Replace(tempPath, filePath, backupPath)

                DeleteOldBackups(Path.GetDirectoryName(filePath),
                                 _SteinFont,
                                 _steinSatz,
                                 MAX_BACKUP_FILES)
            Else
                File.Move(tempPath, filePath)
            End If

            Me._hashLastStorage = Me.GetMyHash()

        Catch ex As Exception When TypeOf ex Is IOException OrElse
                                   TypeOf ex Is UnauthorizedAccessException OrElse
                                   TypeOf ex Is InvalidOperationException OrElse
                                   TypeOf ex Is ArgumentException OrElse
                                   TypeOf ex Is NotSupportedException

            Dim sb As New StringBuilder
            sb.AppendLine("Folgende Mahjong-Steinbeschreibung konnte nicht gespeichert werden:")
            sb.AppendLine(filePath)
            sb.AppendLine("Grund: " & ex.Message)
            sb.AppendLine()
            sb.AppendLine("Der Vorgang wird abgebrochen.")

            MsgBox(sb.ToString(), MsgBoxStyle.Critical Or MsgBoxStyle.OkOnly, "MahjongGK")

        Finally
            If Not String.IsNullOrWhiteSpace(tempPath) AndAlso File.Exists(tempPath) Then
                Try
                    File.Delete(tempPath)
                Catch
                    ' absichtlich leer
                End Try
            End If
        End Try

    End Sub

    Private Shared Function EnsureUniqueBackupFilePath(candidatePath As String) As String

        If String.IsNullOrWhiteSpace(candidatePath) Then
            Throw New ArgumentException("Backup-Pfad ist leer.", NameOf(candidatePath))
        End If

        If Not File.Exists(candidatePath) Then
            Return candidatePath
        End If

        Dim folderPath As String = Path.GetDirectoryName(candidatePath)
        Dim fileName As String = Path.GetFileName(candidatePath)
        Dim extension As String = Path.GetExtension(fileName)
        Dim baseName As String = Path.GetFileNameWithoutExtension(fileName)

        Dim lastDashPos As Integer = baseName.LastIndexOf("-"c)

        If lastDashPos < 0 Then
            Throw New InvalidOperationException("Backup-Dateiname hat ein unerwartetes Format: " & fileName)
        End If

        Dim leftPart As String = baseName.Substring(0, lastDashPos)
        Dim rightPart As String = baseName.Substring(lastDashPos) ' inkl. "-SteinSatz"

        Dim count As Integer = 1

        Do
            Dim newFileName As String =
                leftPart & "-" &
                count.ToString("00", CultureInfo.InvariantCulture) &
                rightPart &
                extension

            Dim newPath As String = Path.Combine(folderPath, newFileName)

            If Not File.Exists(newPath) Then
                Return newPath
            End If

            count += 1
        Loop

    End Function

    Private Shared Sub DeleteOldBackups(folderPath As String,
                                        steinFont As SteinFont,
                                        steinSatz As SteinSatz,
                                        maxBackupFiles As Integer)

        If maxBackupFiles < 0 Then
            Throw New ArgumentOutOfRangeException(NameOf(maxBackupFiles))
        End If

        If String.IsNullOrWhiteSpace(folderPath) Then
            Exit Sub
        End If

        If Not Directory.Exists(folderPath) Then
            Exit Sub
        End If

        Dim di As New DirectoryInfo(folderPath)
        Dim searchPattern As String = steinFont.ToString() & "-*-" & steinSatz.ToString() & ".xml"
        Dim allFiles() As FileInfo = di.GetFiles(searchPattern)
        Dim backupFiles As New List(Of FileInfo)

        For Each fi As FileInfo In allFiles
            If IsBackupFileName(fi.Name, steinFont, steinSatz) Then
                backupFiles.Add(fi)
            End If
        Next

        ' Neueste zuerst.
        backupFiles.Sort(
            Function(x As FileInfo, y As FileInfo) As Integer
                Return StringComparer.OrdinalIgnoreCase.Compare(y.Name, x.Name)
            End Function)

        If backupFiles.Count <= maxBackupFiles Then
            Exit Sub
        End If

        For i As Integer = maxBackupFiles To backupFiles.Count - 1
            backupFiles(i).Delete()
        Next

    End Sub

    Private Shared Function IsBackupFileName(candidateFileName As String,
                                             steinFont As SteinFont,
                                             steinSatz As SteinSatz) As Boolean

        If String.IsNullOrWhiteSpace(candidateFileName) Then Return False

        Dim prefix As String = steinFont.ToString() & "-"
        Dim suffix As String = "-" & steinSatz.ToString() & ".xml"

        If Not candidateFileName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) Then
            Return False
        End If

        If Not candidateFileName.EndsWith(suffix, StringComparison.OrdinalIgnoreCase) Then
            Return False
        End If

        Dim middleLength As Integer = candidateFileName.Length - prefix.Length - suffix.Length

        If middleLength <= 0 Then
            Return False
        End If

        Dim middle As String = candidateFileName.Substring(prefix.Length, middleLength)
        Dim timeLength As Integer = "yyyy-MM-dd-HH-mm-ss-fff".Length

        Dim timePart As String
        Dim counterPart As String = String.Empty

        If middle.Length = timeLength Then
            timePart = middle

        ElseIf middle.Length > timeLength AndAlso middle(timeLength) = "-"c Then
            timePart = middle.Substring(0, timeLength)
            counterPart = middle.Substring(timeLength + 1)

            If counterPart.Length = 0 Then
                Return False
            End If

        Else
            Return False
        End If

        Dim dt As DateTime
        Dim okDate As Boolean =
            DateTime.TryParseExact(timePart,
                                   "yyyy-MM-dd-HH-mm-ss-fff",
                                   CultureInfo.InvariantCulture,
                                   DateTimeStyles.None,
                                   dt)

        If Not okDate Then
            Return False
        End If

        If counterPart.Length = 0 Then
            Return True
        End If

        Dim n As Integer
        Return Integer.TryParse(counterPart,
                                NumberStyles.None,
                                CultureInfo.InvariantCulture,
                                n)

    End Function

    Private Shared Function GetTesterTileColorsRoot() As String

        Return Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            "MahjongGKTileFactoryTester",
            "SteinDesigns")

    End Function

    Private Shared Function GetMahjongGKUserTileColorsRoot() As String

        Return Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            "MahjongGK",
            "SteinDesigns")

    End Function

    Public Shared Function GetProjectTileColorsRoot() As String

        Return Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "Visual Studio",
            "MahjongGK",
            "MahjongGK",
            "Spielfeld",
            "TileColors")

    End Function

    Public Shared Function GetMahjongGKVbProjFullPath() As String

        Return Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "Visual Studio",
            "MahjongGK",
            "MahjongGK",
            "MahjongGK.vbproj")

    End Function
    Private Shared Function GetRuntimeTileColorsRoot() As String

        Return Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "Spielfeld",
            "TileColors")

    End Function

#End Region

#Region "Helper"

    Private Shared Function ConvertPropertyValueToInvariantString(value As Object) As String

        If value Is Nothing Then
            Return String.Empty
        End If

        If TypeOf value Is Color Then
            Return ColorToArgbHex(DirectCast(value, Color))
        End If

        Dim formattable As IFormattable = TryCast(value, IFormattable)

        If formattable IsNot Nothing Then
            Return formattable.ToString(Nothing, CultureInfo.InvariantCulture)
        End If

        Return value.ToString()

    End Function

    Private Shared Function IsXmlHelperProperty(prop As PropertyInfo) As Boolean
        Return prop.Name.EndsWith("Xml", StringComparison.Ordinal)
    End Function

    ''' <summary>
    ''' Wandelt eine Color in einen lesbaren ARGB-Hexstring um: AARRGGBB
    ''' Beispiel: FF3366CC
    ''' </summary>
    Public Shared Function ColorToArgbHex(value As Color) As String
        Return value.A.ToString("X2", CultureInfo.InvariantCulture) &
               value.R.ToString("X2", CultureInfo.InvariantCulture) &
               value.G.ToString("X2", CultureInfo.InvariantCulture) &
               value.B.ToString("X2", CultureInfo.InvariantCulture)
    End Function

    ''' <summary>
    ''' Liest einen ARGB-Hexstring im Format AARRGGBB.
    ''' Erlaubt zusätzlich #AARRGGBB, Und-ZeichenHAARRGGBB und 0xAARRGGBB.
    ''' </summary>
    Public Shared Function ParseArgbHex(text As String) As Color

        If text Is Nothing Then
            Throw New ArgumentNullException(NameOf(text))
        End If

        Dim s As String = text.Trim()

        If s.Length = 0 Then
            Return Color.Empty
        End If

        If s.StartsWith("#", StringComparison.Ordinal) Then
            s = s.Substring(1)
        End If

        If s.StartsWith("&H", StringComparison.OrdinalIgnoreCase) Then
            s = s.Substring(2)
        End If

        If s.StartsWith("0x", StringComparison.OrdinalIgnoreCase) Then
            s = s.Substring(2)
        End If

        If s.Length <> 8 Then
            Throw New FormatException("ARGB-Hex muss genau 8 Hex-Zeichen haben, z. B. FF3366CC.")
        End If

        Dim argb As Integer = Integer.Parse(s, NumberStyles.HexNumber, CultureInfo.InvariantCulture)

        Dim a As Integer = (argb >> 24) And &HFF
        Dim r As Integer = (argb >> 16) And &HFF
        Dim g As Integer = (argb >> 8) And &HFF
        Dim b As Integer = argb And &HFF

        Return Color.FromArgb(a, r, g, b)

    End Function

    Private Function TryParseArgbHexColor(text As String, ByRef result As Color) As Boolean

        result = Color.Empty

        If text Is Nothing Then Return False

        Dim s As String = text.Trim()

        If s.StartsWith("#", StringComparison.Ordinal) Then
            s = s.Substring(1)
        End If

        If s.StartsWith("&H", StringComparison.OrdinalIgnoreCase) Then
            s = s.Substring(2)
        End If

        If s.Length <> 8 Then Return False

        Dim value As UInteger
        If Not UInteger.TryParse(s, NumberStyles.HexNumber, CultureInfo.InvariantCulture, value) Then
            Return False
        End If

        Dim raw As Integer = BitConverter.ToInt32(BitConverter.GetBytes(value), 0)
        result = Color.FromArgb(raw)

        Return True

    End Function

    Private Function AddAndClamp(value1 As Integer, value2 As Integer) As Integer

        Dim value As Integer = value1 + value2
        If value < 0 Then
            Return 0
        ElseIf value > 100 Then
            Return 100
        Else
            Return value
        End If
    End Function

    Private Function ColorsAreEqual(col1 As Color, col2 As Color) As Boolean
        If col1.A <> col2.A Then Return False
        If col1.R <> col2.R Then Return False
        If col1.G <> col2.G Then Return False
        If col1.B <> col2.B Then Return False
        Return True
    End Function
    '
    ''' <summary>
    ''' Importiert entweder die ColorValues oder die NonColorValues aus der übergebenen
    ''' TileColors in diese Instanz.
    ''' </summary>
    ''' <param name="tileColors"></param>
    ''' <param name="copyColorValues">True: die ColorValues, False: die NonColorValues</param>
    Public Sub ImportPartOfTileColors(tileColors As TileColors, copyColorValues As Boolean)

        If tileColors Is Nothing Then
            Throw New ArgumentNullException(NameOf(tileColors))
        End If

        Dim myType As Type = Me.GetType()

        Dim arr() As String
        If copyColorValues Then
            arr = _arrCopyColorValues
        Else
            arr = _arrCopyNonColorValues
        End If

        For Each propName As String In arr

            'findet alle Public Properties
            Dim pi As Reflection.PropertyInfo = myType.GetProperty(propName)

            'findet Public, Private, Friend und Protected
            'Dim pi As Reflection.PropertyInfo =
            '    myType.GetProperty(propName,
            '                       Reflection.BindingFlags.Instance Or
            '                       Reflection.BindingFlags.Public Or
            '                       Reflection.BindingFlags.NonPublic)

            If pi Is Nothing Then
                Throw New MissingMemberException(myType.FullName, propName)
            End If

            If Not pi.CanRead OrElse Not pi.CanWrite Then
                Throw New InvalidOperationException(
                    $"Property kann nicht kopiert werden: {propName}")
            End If

            Dim value As Object = pi.GetValue(tileColors, Nothing)

            If pi.PropertyType Is GetType(String) Then

                If value Is Nothing Then
                    pi.SetValue(Me, Nothing, Nothing)
                Else
                    pi.SetValue(Me, String.Copy(DirectCast(value, String)), Nothing)
                End If

            Else
                pi.SetValue(Me, value, Nothing)

            End If

        Next

        'gelten jetzt als geladen
        tileColors.StatusLoadingOK = True
        Me.StatusLoadingOK = True

    End Sub
#End Region

End Class
