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
                   steinSymbol As SteinSymbol,
                   steinStatus As SteinStatus,
                   steinSize As Size,
                   steinBasisSize As Size,
                   steinGhost As SteinGhost)

        Me.AktRenderMode = aktRenderMode
        Me.TileColors = tileColors
        Me.SteinSymbol = steinSymbol
        Me.SteinStatus = steinStatus
        Me.SteinSymbolVersion = GetSteinSymbolVersionFromSteinSymbol(steinSymbol)
        Me.SteinSize = steinSize
        Me.SteinBasisSize = steinBasisSize
        Me._steinGhost = steinGhost

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
        Me.SteinSymbol = tilerequest.SteinSymbol
        Me.SteinStatus = tilerequest.SteinStatus
        Me.SteinSymbolVersion = tilerequest.SteinSymbolVersion
        Me.SteinSize = tilerequest.SteinSize
        Me.SteinBasisSize = tilerequest.SteinBasisSize
        Me._steinGhost = tilerequest.SteinGhost

    End Sub

    Private Sub CheckSize()
        If SteinWidth < 6 OrElse SteinWidth > 500 OrElse SteinHeight < 6 OrElse SteinHeight > 500 Then
            Throw New Exception("Ungültige Steinabmessungen in TileRenderRequest. Gültig: 50 bis 500 für Breite und Höhe.")
        End If
    End Sub

    Public ReadOnly Property TileColors As TileColors 'Hier muss nur geprüft werden, ob sich die Instanz geändert hat.
    Public ReadOnly Property AktRenderMode As AktRenderMode 'Enumeration
    Public ReadOnly Property SteinSymbol As SteinSymbol 'Enumeration
    Public ReadOnly Property SteinStatus As SteinStatus 'Enumeration
    '
    ''' <summary>
    ''' SteinSymbolVersion wird immer aus SteinSymbol abgeleitet.
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property SteinSymbolVersion As SteinSymbolVersion 'Enumeration

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

    Private ReadOnly _steinGhost As SteinGhost
    Public ReadOnly Property SteinGhost As SteinGhost
        Get
            Return _steinGhost
        End Get
    End Property

    Private Shared ReadOnly SteinSymbolToSteinSymbolVersionLookup As Integer() = {
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
    ''' SteinSymbolVersion leitet sich aus dem SteinSymbol ab.
    ''' Hier die Umwandlung.
    ''' </summary>
    ''' <param name="Index"></param>
    ''' <returns></returns>
    Public Shared Function GetSteinSymbolVersionFromSteinSymbol(Index As SteinSymbol) As SteinSymbolVersion
        Return CType(SteinSymbolToSteinSymbolVersionLookup(CInt(Index)), SteinSymbolVersion)
    End Function

    Public Function SomethingChanged(request As TileRequest) As Boolean

        If request Is Nothing Then
            Return True
        End If

        If Not Object.ReferenceEquals(Me.TileColors, request.TileColors) Then Return True
        If Me.TileColors.SessionIdent <> request.TileColors.SessionIdent Then Return True

        If Me.AktRenderMode <> request.AktRenderMode Then Return True
        If Me.SteinSymbol <> request.SteinSymbol Then Return True
        If Me.SteinStatus <> request.SteinStatus Then Return True
        '' If Me.SteinSymbolVersion <> request.SteinSymbolVersion Then Return True 'wird abgeleitet
        If Me.SteinWidth <> request.SteinWidth Then Return True
        If Me.SteinHeight <> request.SteinHeight Then Return True
        If Me.SteinGhost <> SteinGhost Then Return True

        Return False

    End Function

    Public Shared Function DeepCopy(source As TileRequest) As TileRequest

        If source Is Nothing Then
            Return Nothing
        End If

        Return New TileRequest(source)

    End Function

End Class
