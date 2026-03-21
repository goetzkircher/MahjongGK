'Ich will aus dieser Klasse die Property:
'Public Property Quadrant As Quadrant
'Public Property SteinIndex As SteinIndexEnum
'Public Property SteinInfoIndex As Integer
'herausnehmen und in eine ansonsten funktionsgleiche
'Klasse TriplX verlagern. Da ich wieder nicht weis,
'wie die Vererbung funktioniert, schreibe sie mir bitte.
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
'

Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

#Disable Warning IDE0079
#Disable Warning IDE1006

''' <summary>
''' Pfad: MahjongGK/Spielfeld/Container/Triple
''' 
''' Ergebnis z.B. der Suche nach einem freiem Platz für einen Stein.
''' Es gibt in Tripl: 
''' Property Valide As ValidePlace: Get und Set möglich
''' ReadOnly Property IsValide As Boolean: True bei Yes
''' und ReadOnly Property IsNotValide As Boolean: True bei ungleich Yes
''' (Meist reicht die Auskunft)
''' </summary>
Public Enum ValidePlace
    NotSet
    Yes
    No
    '                
    Occupied         'Der Platz ist belegt
    OutsideBorder    'Spielfeldrand erreicht.
    NoKandidat = No
    NoFundamentFound = No 'in der Schicht darunter gibt es keinen
    '                       Stein, auf den gebaut werden könnte.
End Enum

''' <summary>Klasse zum Kapseln von x, y und z-Koordinaten</summary>
<DebuggerStepThrough>
<DebuggerDisplay("{DebuggerView}")>
<Serializable>
Public Class Triple

    Private _x As Integer
    Private _y As Integer
    Private _z As Integer
    Private _Valide As ValidePlace

    Public Property x As Integer
        Get
            Return _x
        End Get
        Set(value As Integer)
            _x = value
        End Set
    End Property

    Public Property y As Integer
        Get
            Return _y
        End Get
        Set(value As Integer)
            _y = value
        End Set
    End Property

    Public Property z As Integer
        Get
            Return _z
        End Get
        Set(value As Integer)
            _z = value
        End Set
    End Property

    Public Property Valide As ValidePlace
        Get
            Return _Valide
        End Get
        Set(value As ValidePlace)
            _Valide = value
        End Set
    End Property

    Public ReadOnly Property IsValideYes As Boolean
        Get
            Return _Valide = ValidePlace.Yes
        End Get
    End Property

    Public ReadOnly Property IsNotValideYes As Boolean
        Get
            Return _Valide <> ValidePlace.Yes
        End Get
    End Property


    <DebuggerBrowsable(DebuggerBrowsableState.Never)>
    Private ReadOnly Property DebuggerView As String
        Get
            Return $"(X={x}, Y={y}, Z={z}, Valide={Valide})"
        End Get
    End Property

    Public Property Tag As Object

    Sub New()
    End Sub

    Sub New(x As Integer, y As Integer, z As Integer)
        _x = x : _y = y : _z = z
    End Sub

    Sub New(x As Integer, y As Integer, z As Integer, valide As ValidePlace)
        _x = x : _y = y : _z = z : _Valide = valide
    End Sub


    Sub New(tripl As Triple, valide As ValidePlace)
        _x = tripl.x : _y = tripl.y : _z = tripl.z : _Valide = valide
    End Sub

    Sub New(valide As ValidePlace)
        _Valide = valide
    End Sub
    '
    ''' <summary>
    ''' Kopiert das übergebene Triple und korrigiert es um die weiteren Angaben.
    ''' </summary>
    ''' <param name="tripl"></param>
    ''' <param name="addX"></param>
    ''' <param name="addY"></param>
    ''' <param name="addZ"></param>
    Sub New(tripl As Triple, Optional addX As Integer = 0, Optional addY As Integer = 0, Optional addZ As Integer = 0)
        _x = tripl.x + addX : _y = tripl.y + addY : _z = tripl.z + addZ : _Valide = tripl.Valide
    End Sub

    Public ReadOnly Property DeepCopy As Triple
        Get
            Return New Triple(x, y, z, Valide)
        End Get
    End Property



    Public Function IsEqual(triple As Triple) As Boolean
        If IsNothing(triple) Then
            Return False
        Else
            Return x = triple.x AndAlso y = triple.y AndAlso z = triple.z
        End If
    End Function
    Public Function IsEqual(x As Integer, y As Integer, z As Integer) As Boolean
        Return Me.x = x AndAlso Me.y = y AndAlso Me.z = z
    End Function

    Public ReadOnly Property IsEmpty As Boolean
        Get
            If x <> 0 Then Return False
            If y <> 0 Then Return False
            If z <> 0 Then Return False
            Return True
        End Get
    End Property
    Public ReadOnly Property IsNotEmpty As Boolean
        Get
            If x <> 0 Then Return True
            If y <> 0 Then Return True
            If z <> 0 Then Return True
            Return False
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return $"(X={x}, Y={y}, Z={z}, Valide={Valide})"
    End Function

    Public Function IsInsideSpielfeldBounds(arrFB(,,) As Integer, Optional excludeRightAndBottomMargin As Boolean = False) As Boolean
        Dim subtract As Integer = If(excludeRightAndBottomMargin = True, 2, 1)
        Return ((x >= 1 AndAlso x <= arrFB.GetUpperBound(0) - subtract) AndAlso
            (y >= 1 AndAlso y <= arrFB.GetUpperBound(1) - subtract) AndAlso
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

End Class
