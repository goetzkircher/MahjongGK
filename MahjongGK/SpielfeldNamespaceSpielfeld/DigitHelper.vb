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

Imports System.Runtime.CompilerServices

Namespace Spielfeld
    ''' <summary>
    ''' zeitkritische Methoden, die nur Zahlen manipulieren, ohne auf andere Daten zuzugreifen
    ''' </summary>
    Public Module DigitHelper


        Private ReadOnly _divisor(18) As Long

        Sub New()
            _divisor(0) = 0
            Dim val As Long = 1
            For i As Integer = 1 To 18
                _divisor(i) = val
                val *= 10
            Next
        End Sub

        '
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetDigit(ByVal value As Long, ByVal digitPos As Integer) As Integer
            Return CInt((value \ _divisor(digitPos)) Mod 10)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function SetDigit(ByVal value As Long, ByVal digitPos As Integer, ByVal digitValue As Integer) As Long
            Dim divisor As Long = _divisor(digitPos)
            Dim currentDigit As Integer = CInt((value \ divisor) Mod 10)
            Return value + CLng((digitValue - currentDigit)) * divisor
        End Function

        ''' <summary>
        ''' Inkrementiert einen Zähler-Digit zyklisch von 0..9 (Modulo 10),
        ''' ohne Übertrag auf höhere Stelle.
        ''' </summary>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function IncDigitMod10(ByVal value As Long, ByVal digitPos As Integer) As Long
            Dim divisor As Long = _divisor(digitPos)
            Dim currentDigit As Integer = CInt((value \ divisor) Mod 10)
            Dim newDigit As Integer = (currentDigit + 1) Mod 10
            Return value - CLng(currentDigit) * divisor + CLng(newDigit) * divisor
        End Function

    End Module
End Namespace