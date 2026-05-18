Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

Imports System.Drawing
Imports MahjongGK.Contracts.GlobalEnum

'
''' <summary>
''' Beschreibt vollständig, wie ein Stein gerendert werden soll.
''' Noch ohne Cache-Logik.
''' </summary>
Public NotInheritable Class TileRequest

    Public Sub New()

    End Sub
    '
    ''' <summary>
    ''' Es werden immer Bitmaps aus dem Cache zurückgeliefert.
    ''' Besitzer ist das Cache.
    ''' </summary>
    Public Sub New(aktRenderMode As AktRenderMode,
                   tileColors As TileColors,
                   steinTyp As SteinTyp,
                   steinStatus As SteinStatus,
                   steinFrameVersion As SteinFrameVersion,
                   steinSize As Size,
                   steinBasisSize As Size,
                   Optional ghost As Boolean = False)

        Me.AktRenderMode = aktRenderMode
        Me.TileColors = tileColors
        Me.SteinTyp = steinTyp
        Me.SteinStatus = steinStatus
        Me.SteinTypVersion = GetSteinTypVersionFromSteinTyp(steinTyp)
        Me._steinFrameVersion = steinFrameVersion
        Me.SteinSize = steinSize
        Me.SteinBasisSize = steinBasisSize
        Me._ghost = ghost

        CheckSize()

        If IsNothing(Me.TileColors) Then
            Throw New Exception("Programmierfehler: tileColors darf in TileRequest nicht Nothing sein.")
        End If

        tileColors.SetSteinMainDescriptor(Me)

    End Sub

    Public Sub New(tilerequest As TileRequest)

        If tilerequest Is Nothing Then
            Throw New ArgumentNullException(NameOf(tilerequest))
        End If

        Me.AktRenderMode = tilerequest.AktRenderMode
        'Von den TileColors kommt fast immer die selbe Instanz. Wenn eine andere Instanz kommt,
        'heißt das, daß irgendwelche Werte sich geändert haben. (anderes Layout)
        Me.TileColors = tilerequest.TileColors 'TileColors also bewusst keine echte DeepCopy, sondern eine Referenzkopie.
        Me.SteinTyp = tilerequest.SteinTyp
        Me.SteinStatus = tilerequest.SteinStatus
        Me.SteinTypVersion = tilerequest.SteinTypVersion
        Me._steinFrameVersion = tilerequest.SteinFrameVersion
        Me.SteinSize = tilerequest.SteinSize
        Me.SteinBasisSize = tilerequest.SteinBasisSize
        Me._ghost = tilerequest.Ghost

    End Sub

    Private Sub CheckSize()
        If SteinWidth < 6 OrElse SteinWidth > 500 OrElse SteinHeight < 6 OrElse SteinHeight > 500 Then
            Throw New Exception("Ungültige Steinabmessungen in TileRenderRequest. Gültig: 50 bis 500 für Breite und Höhe.")
        End If
    End Sub

    Public ReadOnly Property TileColors As TileColors 'Hier muss nur geprüft werden, ob sich die Instanz geändert hat.
    Public ReadOnly Property AktRenderMode As AktRenderMode 'Enumeration
    Public ReadOnly Property SteinTyp As SteinTyp 'Enumeration
    Public ReadOnly Property SteinStatus As SteinStatus 'Enumeration
    '
    ''' <summary>
    ''' SteinTypVersion wird immer aus SteinTyp abgeleitet.
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property SteinTypVersion As SteinTypVersion 'Enumeration

    Private _steinFrameVersion As SteinFrameVersion
    Public ReadOnly Property SteinFrameVersion As SteinFrameVersion 'Enumeration
        Get
            Return _steinFrameVersion
        End Get
    End Property
    Public Sub SetSteinFrameVersion(steinStatus As SteinStatus, SteinFrameVersion As SteinFrameVersion, ghost As Boolean)
        _SteinStatus = steinStatus
        _steinFrameVersion = SteinFrameVersion
        _ghost = ghost
    End Sub

    Public Sub SetSteinStatusToI01Normal()
        _SteinStatus = SteinStatus.I01Normal
    End Sub
    Public ReadOnly Property SteinSize As Size

    Public ReadOnly Property SteinWidth As Integer
        Get
            Return SteinSize.Width
        End Get
    End Property
    Public ReadOnly Property SteinHeight As Integer
        Get
            Return SteinSize.Height
        End Get
    End Property

    Public ReadOnly Property SteinBasisSize As Size
    Public ReadOnly Property SteinBasisWidth As Integer
        Get
            Return SteinBasisSize.Width
        End Get
    End Property
    Public ReadOnly Property SteinBasisHeight As Integer
        Get
            Return SteinBasisSize.Height
        End Get
    End Property

    Private _ghost As Boolean
    Public ReadOnly Property Ghost As Boolean
        Get
            Return _ghost
        End Get
    End Property

    Private Shared ReadOnly SteinTypToSteinTypVersionLookup As Integer() = {
      0,  'ErrorSy
      0,  'Punkt01
      0,  'Punkt02
      0,  'Punkt03
      0,  'Punkt04
      0,  'Punkt05
      0,  'Punkt06
      0,  'Punkt07
      0,  'Punkt08
      0,  'Punkt09
      0,  'Bambus1
      0,  'Bambus2
      0,  'Bambus3
      0,  'Bambus4
      0,  'Bambus5
      0,  'Bambus6
      0,  'Bambus7
      0,  'Bambus8
      0,  'Bambus9
      0,  'Symbol1
      0,  'Symbol2
      0,  'Symbol3
      0,  'Symbol4
      0,  'Symbol5
      0,  'Symbol6
      0,  'Symbol7
      0,  'Symbol8
      0,  'Symbol9
      0,  'DracheR
      0,  'DracheG
      0,  'DracheW
      1,  'WindOst
      1,  'WindSüd
      1,  'WindWst
      1,  'WindNrd
      2,  'BlütePf
      2,  'BlüteOr
      2,  'BlüteCt
      2,  'BlüteBa
      3,  'JahrFrl
      3,  'JahrSom
      3,  'JahrHer
      3   'JahrWin
  }
    '
    ''' <summary>
    ''' SteinTypVersion leitet sich aus dem SteinTyp ab.
    ''' Hier die Umwandlung.
    ''' </summary>
    ''' <param name="Index"></param>
    ''' <returns></returns>
    Public Shared Function GetSteinTypVersionFromSteinTyp(Index As SteinTyp) As SteinTypVersion
        Return CType(SteinTypToSteinTypVersionLookup(CInt(Index)), SteinTypVersion)
    End Function

    Public Function SomethingChanged(request As TileRequest) As Boolean

        If request Is Nothing Then
            Return True
        End If

        If Not Object.ReferenceEquals(Me.TileColors, request.TileColors) Then Return True
        If Me.TileColors.SessionIdent <> request.TileColors.SessionIdent Then Return True

        If Me.AktRenderMode <> request.AktRenderMode Then Return True
        If Me.SteinTyp <> request.SteinTyp Then Return True
        If Me.SteinStatus <> request.SteinStatus Then Return True
        '' If Me.SteinTypVersion <> request.SteinTypVersion Then Return True 'wird abgeleitet
        If Me.SteinFrameVersion <> request.SteinFrameVersion Then Return True
        If Me.SteinWidth <> request.SteinWidth Then Return True
        If Me.SteinHeight <> request.SteinHeight Then Return True
        '  If Me.Ghost <> request.Ghost Then Return True

        Return False

    End Function

    Public Shared Function DeepCopy(source As TileRequest) As TileRequest

        If source Is Nothing Then
            Return Nothing
        End If

        Return New TileRequest(source)

    End Function

End Class
