Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

#Disable Warning IDE0079
#Disable Warning IDE1006

''' <summary>
''' Pfad: MahjongGK/Spielfeld/Container/TopSteinInfo
''' 
''' Hält die Daten der obersten vollständig sichtbaren Steine.
''' </summary>

Public Class TopSteinInfo
    Inherits TripleX

    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub New(x As Integer, y As Integer, z As Integer)
        MyBase.New(x, y, z)
    End Sub

    Public Overrides Function ToString() As String
        Return $"(X={x}, Y={y}, Z={z}, Valide={Valide}, SteinInfoIndex={SteinInfoIndex}, SteinIndex={SteinIndex}, Quadrant={Quadrant}, FreeSide={FreeSide})"
    End Function

    Public Overloads Function ToTriple() As Triple
        Return New Triple(Me.x, Me.y, Me.z, Me.Valide)
    End Function

    Public Property RenderRect As Rectangle

    Private _freeSide As FreeSide
    Public Property FreeSide As FreeSide
        Get
            Return _freeSide
        End Get
        Set(value As FreeSide)
            _freeSide = value
        End Set
    End Property

    Public ReadOnly Property HasAnyFreeSide As Boolean
        Get
            Return (_freeSide And 3) <> 0
        End Get
    End Property

    Public Property SteinStatus As SteinStatus

    Public Property SteinClickGruppe As Integer

End Class
