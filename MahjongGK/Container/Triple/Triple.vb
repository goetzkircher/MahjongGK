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
'#   the Free Software Foundation, either version 3 of the License, or     #
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
'

Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

#Disable Warning IDE0079
#Disable Warning IDE1006


''' <summary>Klasse zum Kapseln von x, y und z-Koordinaten</summary>
<DebuggerDisplay("{DebuggerView}")>
<DebuggerStepThrough>
<Serializable>
Public Class Triple

    Public Event Changed(sender As Triple)

    Private _x As Integer
    Private _y As Integer
    Private _z As Integer
    Private _Valide As ValidePlaceEnum

    Public Property x As Integer
        Get
            Return _x
        End Get
        Set(value As Integer)
            If _x = value Then Return
            _x = value
            RaiseChanged()
        End Set
    End Property

    Public Property y As Integer
        Get
            Return _y
        End Get
        Set(value As Integer)
            If _y = value Then Return
            _y = value
            RaiseChanged()
        End Set
    End Property

    Public Property z As Integer
        Get
            Return _z
        End Get
        Set(value As Integer)
            If _z = value Then Return
            _z = value
            RaiseChanged()
        End Set
    End Property

    Public Property Valide As ValidePlaceEnum
        Get
            Return _Valide
        End Get
        Set(value As ValidePlaceEnum)
            If _Valide = value Then Return
            _Valide = value
            RaiseChanged()
        End Set
    End Property

    <DebuggerBrowsable(DebuggerBrowsableState.Never)>
    Private ReadOnly Property DebuggerView As String
        Get
            Return $"(X={x}, Y={y}, Z={z}, Valide={Valide})"
        End Get
    End Property

    Sub New()
    End Sub

    Sub New(x As Integer, y As Integer, z As Integer)
        _x = x : _y = y : _z = z
    End Sub

    Sub New(x As Integer, y As Integer, z As Integer, valide As ValidePlaceEnum)
        _x = x : _y = y : _z = z : _Valide = valide
    End Sub

    Sub New(tripl As Triple, valide As ValidePlaceEnum)
        _x = tripl.x : _y = tripl.y : _z = tripl.z : _Valide = valide
    End Sub

    Public ReadOnly Property DeepCopy As Triple
        Get
            Return New Triple(x, y, z, Valide)
        End Get
    End Property

    Public Function Contains(x As Integer, y As Integer, z As Integer) As Boolean
        Return Me.x = x AndAlso Me.y = y AndAlso Me.z = z
    End Function

    Public Function IsInsideSpielfeldBounds(arrFB(,,) As Integer) As Boolean
        Return ((x >= 1 AndAlso x <= arrFB.GetUpperBound(0) - 1) AndAlso
                (y >= 1 AndAlso y <= arrFB.GetUpperBound(1) - 1) AndAlso
                (z >= 0 AndAlso z <= arrFB.GetUpperBound(2)))
    End Function

    Public Function IsInsideLBoundUBound(arrFB(,,) As Integer) As Boolean
        Return ((x >= 0 AndAlso x <= arrFB.GetUpperBound(0)) AndAlso
                (y >= 0 AndAlso y <= arrFB.GetUpperBound(1)) AndAlso
                (z >= 0 AndAlso z <= arrFB.GetUpperBound(2)))
    End Function

    Public Sub CopyToArrFBMain(arrFB(,,) As Integer, fb As Integer)
        arrFB(x, y, z) = fb
    End Sub

    Public Function IsEqual(triple As Triple) As Boolean
        If IsNothing(triple) Then
            Return False
        Else
            Return x = triple.x AndAlso y = triple.y AndAlso z = triple.z
        End If
    End Function

    Public Overrides Function ToString() As String
        Return $"(X={x}, Y={y}, Z={z}, Valide={Valide})"
    End Function

    Private Sub RaiseChanged()
        RaiseEvent Changed(Me)
    End Sub

End Class
