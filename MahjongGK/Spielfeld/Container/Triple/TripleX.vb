Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

Imports MahjongGK.Spielfeld



#Disable Warning IDE0079
#Disable Warning IDE1006

''' <summary>
''' Pfad: MahjongGK/Spielfeld/Container/Triple
''' 
''' Erweiterung von Triple um steinspezifische Zusatzinformationen.
''' </summary>
<DebuggerStepThrough>
<Serializable>
Public Class TripleX
    Inherits Triple

    Private _Quadrant As Quadrant
    Private _SteinIndex As SteinIndexEnum
    Private _SteinInfoIndex As Integer
    Private _SteinInfo As SteinInfo

    Public Property Quadrant As Quadrant
        Get
            Return _Quadrant
        End Get
        Set(value As Quadrant)
            _Quadrant = value
        End Set
    End Property

    Public Property SteinIndex As SteinIndexEnum
        Get
            Return _SteinIndex
        End Get
        Set(value As SteinIndexEnum)
            _SteinIndex = value
        End Set
    End Property

    Public Property SteinInfoIndex As Integer
        Get
            Return _SteinInfoIndex
        End Get
        Set(value As Integer)
            _SteinInfoIndex = value
        End Set
    End Property

    Public Property SteinInfo As SteinInfo
        Get
            Return _SteinInfo
        End Get
        Set(value As SteinInfo)
            _SteinInfo = value
        End Set
    End Property

    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub New(x As Integer, y As Integer, z As Integer)
        MyBase.New(x, y, z)
    End Sub

    Public Sub New(x As Integer, y As Integer, z As Integer, valide As ValidePlace)
        MyBase.New(x, y, z, valide)
    End Sub

    Public Sub New(x As Integer,
                       y As Integer,
                       z As Integer,
                       valide As ValidePlace,
                       steinInfoIndex As Integer,
                       steinIndex As SteinIndexEnum,
                       quadrant As Quadrant,
                       steinInfo As SteinInfo)

        MyBase.New(x, y, z, valide)

        _SteinInfoIndex = steinInfoIndex
        _SteinIndex = steinIndex
        _Quadrant = quadrant
        _SteinInfo = steinInfo
    End Sub

    Public Sub New(tripl As Triple, valide As ValidePlace)
        MyBase.New(tripl, valide)
    End Sub

    Public Sub New(valide As ValidePlace)
        MyBase.New(valide)
    End Sub

    Public Sub New(tripl As Triple, Optional addX As Integer = 0, Optional addY As Integer = 0, Optional addZ As Integer = 0)
        MyBase.New(tripl, addX, addY, addZ)
    End Sub

    Public Sub New(triplx As TripleX)
        MyBase.New(triplx.x, triplx.y, triplx.z, triplx.Valide)

        _SteinInfoIndex = triplx.SteinInfoIndex
        _SteinIndex = triplx.SteinIndex
        _Quadrant = triplx.Quadrant
        Me.Tag = triplx.Tag
    End Sub

    Public ReadOnly Property DeepCopyX As TripleX
        Get
            Dim result As New TripleX(x, y, z, Valide, SteinInfoIndex, SteinIndex, Quadrant, SteinInfo)
            result.Tag = Me.Tag
            Return result
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return $"(X={x}, Y={y}, Z={z}, Valide={Valide}, SteinInfoIndex={SteinInfoIndex}, SteinIndex={SteinIndex}, Quadrant={Quadrant})"
    End Function

    Public Function ToTriple() As Triple
        Return New Triple(Me.x, Me.y, Me.z, Me.Valide)
    End Function

End Class

