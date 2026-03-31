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

#Disable Warning IDE0079
#Disable Warning IDE1006

Imports System.Runtime.CompilerServices

Namespace Spielfeld
    ''' <summary>
    ''' Pfad: MahjongGK/Spielfeld/Helfermodule
    ''' </summary>
    Public Module SpielfeldHelper

#Region "Sicherheitsgurt"

#End Region

#Region "Zufallsfunktionen"

        Private ReadOnly _zufall As New Random()
        '
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetZufallszahl(minInclusive As Integer, maxExclusive As Integer, Optional maxInclusive As Boolean = False) As Integer
            If maxInclusive Then
                Return _zufall.Next(minInclusive, maxExclusive + 1)
            Else
                Return _zufall.Next(minInclusive, maxExclusive)
            End If
        End Function

        Public Function GetZufallTrueFalse() As Boolean
            'Die "2" ist ausgeschlossen. Next(0, 2) muss ich mir vorstellen wie:
            'von 0.00000000 bis 1.99999999 ergibt abgeschnitten (Floor) 0 oder 1.
            'Verteilung True zu False ist 50:50
            Return _zufall.Next(0, 2) = 1
        End Function
        '
        Public Function GetZufallMove() As Move
            Return CType(_zufall.Next(0, 3), Move)
        End Function

        Public Function GetZufall0To9() As Integer
            Return _zufall.Next(0, 10)
        End Function

        ''' <summary>
        ''' Gibt eine Zufallszahl größer als 0 und kleiner als 1 zurück
        ''' </summary>
        ''' <returns></returns>
        Public Function GetZufallDouble0To1() As Double
            Return _zufall.NextDouble
        End Function

        Public Function GetZufallDirection() As Direction
            Return CType(_zufall.Next(0, 8), Direction)
        End Function

#End Region

    End Module

End Namespace