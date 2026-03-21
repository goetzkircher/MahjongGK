
'Pfad: MahjongGK/Spielfeld/Basis
'
'##############################################################################
'Gespeichert als ...\SFKlassen\SFStructures
'##############################################################################
'
Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

''' <summary>
''' Hält die Min und Max-Werte des Spielfeldes.
''' </summary>
Public Structure Bounds3D

    Public xMin As Integer
    Public xMax As Integer
    Public yMin As Integer
    Public yMax As Integer
    Public zMin As Integer
    Public zMax As Integer

    Public Sub New(ByVal xMin As Integer, ByVal xMax As Integer,
                   ByVal yMin As Integer, ByVal yMax As Integer,
                   ByVal zMin As Integer, ByVal zMax As Integer)

        Me.xMin = xMin
        Me.xMax = xMax
        Me.yMin = yMin
        Me.yMax = yMax
        Me.zMin = zMin
        Me.zMax = zMax

    End Sub

End Structure