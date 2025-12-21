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
Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

#Disable Warning IDE0079
#Disable Warning IDE1006

Imports System.Runtime.CompilerServices

Namespace Spielfeld
    Public Module SpielfeldHelper


#Region "Sicherheitsgurt"

        Sub New()

            'Wenn die Enumeration "Stein" geändert wird, muß die Lockuptabelle GruppeLookup
            'in der #Region "Sonstige Helfer" angepasst werden.
            'Diese Prüfung erinnert daran :-)
            Dim enumLength As Integer = [Enum].GetValues(GetType(SteinIndexEnum)).Length
            If GruppeLookupNormal.Length <> enumLength Then
                Throw New InvalidOperationException(
                $"Programmierfehler! GruppeLookupNormal hat {GruppeLookupNormal.Length} Einträge, Enum SteinIndexEnum hat {enumLength}. Tabelle anpassen!")
            End If
        End Sub

#End Region

#Region "Bit- und Bit-Operationen"


        '' ''' <summary>
        '' ''' Gibt eine einzelne Ziffer aus einer Long-Zahl zurück.
        '' ''' </summary>
        '' ''' <param name="fb"></param>
        '' ''' <param name="divDigit"></param>
        '' ''' <returns></returns>
        '' <MethodImpl(MethodImplOptions.AggressiveInlining)>
        '' Private Function GetDigit(fb As Long, divDigit As Long) As Integer
        ''     Return CInt((fb \ divDigit) Mod 10)
        '' End Function
        '' '
        '' ''' <summary>
        '' ''' Setzt eine einzelne Ziffer in einer Long-Zahl
        '' ''' </summary>
        '' ''' <param name="fb"></param>
        '' ''' <param name="divDigit"></param>
        '' ''' <param name="digitVal"></param>
        '' ''' <returns></returns>
        '' <MethodImpl(MethodImplOptions.AggressiveInlining)>
        '' Private Function SetDigit(fb As Long, divDigit As Long, digitVal As Integer) As Long
        ''     If digitVal < 0 Then digitVal = 0
        ''     If digitVal > 9 Then digitVal = 9
        ''     fb -= ((fb \ divDigit) Mod 10) * divDigit
        ''     fb += CLng(digitVal) * divDigit
        ''     Return fb
        '' End Function
        '' '
        '' '
        '' 'Ich betrachte die Zehn- und Hunderttausenderstelle des Feldbeschreibers als Bitfeld,
        '' 'in dem 8 verschiedene Flags (BIT0 bis BIT7) gespeichert sind.
        '' '
        '' ' --- Bitfeld auslesen (8 Bits = 0–255) ---
        '' <MethodImpl(MethodImplOptions.AggressiveInlining)>
        '' Public Function GetBitfeld(ByVal zahl As Long) As Integer
        ''     Return CInt((zahl \ DIV_DIG5) Mod 100)
        '' End Function
        '  
        '' ' --- Einzelnes Bit setzen ---
        '' <MethodImpl(MethodImplOptions.AggressiveInlining)>
        '' Public Function SetBit(ByVal zahl As Long, ByVal bitMaske As Integer) As Long
        ''     Dim feld As Integer = GetBitfeld(zahl)
        ''     feld = feld Or bitMaske
        ''     Return (zahl - (GetBitfeld(zahl) * DIV_DIG5)) + (feld * DIV_DIG5)
        '' End Function
        '  
        '' ' --- Einzelnes Bit löschen ---
        '' <MethodImpl(MethodImplOptions.AggressiveInlining)>
        '' Public Function ClearBit(ByVal zahl As Long, ByVal bitMaske As Integer) As Long
        ''     Dim feld As Integer = GetBitfeld(zahl)
        ''     feld = feld And Not bitMaske
        ''     Return (zahl - (GetBitfeld(zahl) * DIV_DIG5)) + (feld * DIV_DIG5)
        '' End Function
        '  
        '' ' --- Prüfen, ob ein Bit gesetzt ist ---
        '' <MethodImpl(MethodImplOptions.AggressiveInlining)>
        '' Public Function IsBitSet(ByVal zahl As Long, ByVal bitMaske As Integer) As Boolean
        ''     Return (GetBitfeld(zahl) And bitMaske) <> 0
        '' End Function
        '  
        '' ' --- Direktwert ins Bitfeld schreiben (0–255) ---
        '' <MethodImpl(MethodImplOptions.AggressiveInlining)>
        '' Public Function SetBitfeld(ByVal zahl As Long, ByVal feldwert As Integer) As Long
        ''     Return (zahl - (GetBitfeld(zahl) * DIV_DIG5)) + ((feldwert And &HFF) * DIV_DIG5)
        '' End Function

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

#Region "Sonstige Helfer"

        ' Lookup-Tabelle.
        ' Muss aus der Reihenfolge der Enum-Deklarationen SteinIndexEnum
        ' und SteinClickGruppe gebaut sein!
        ' Hintergrund: Die Regel "Steine mit gleichem Symbol können paarweise
        ' entfernt werden" gilt meistens, aber nicht immer.
        ' Die Blüten und Jahressymbole sind optisch unterschiedlich, können
        ' aber paarweise entfernt werden. Deshalb gibt es die SteinClickGruppe
        ' und das hier ist die Übersetzungstabellevom SteinIndexEnum zur
        ' SteinClickGruppe
        ' Der Wert des Index selber ist egal, wichtig ist, daß Steine einer
        ' Klickgruppe den gleichen Wert haben.

        Private ReadOnly GruppeLookupNormal As Integer() = {
        0,  'Dummy
        1,  'Punkt1
        2,  'Punkt2
        3,  'Punkt3
        4,  'Punkt4
        5,  'Punkt5
        6,  'Punkt6
        7,  'Punkt7
        8,  'Punkt8
        9,  'Punkt9
        10, 'Bambus1
        11, 'Bambus2
        12, 'Bambus3
        13, 'Bambus4
        14, 'Bambus5
        15, 'Bambus6
        16, 'Bambus7
        17, 'Bambus8
        18, 'Bambus9
        19, 'Symbol1
        20, 'Symbol2
        21, 'Symbol3
        22, 'Symbol4
        23, 'Symbol5
        24, 'Symbol6
        25, 'Symbol7
        26, 'Symbol8
        27, 'Symbol9
        28, 'DrachenRot
        29, 'DrachenGrün
        30, 'DrachenWeiß
        31, 'WindOst
        32, 'WindSüd
        33, 'WindWest
        34, 'WindNord
        35, 'BlütePflaume
        35, 'BlüteOrchidee
        35, 'BlüteChrisantheme
        35, 'BlüteBambus
        36, 'JahrFrühling
        36, 'JahrSommer
        36, 'JahrHerbst
        36  'JahrWinterhttps://www.skmb.de/de/home/onlinebanking/nbf/banking/einzelauftrag.html?sp:ct=TUFJTkBwb3J0YWw%3D
    }

        Private ReadOnly GruppeLookupVereinfacht As Integer() = {
        0,  'Dummy
        1,  'Punkt1
        2,  'Punkt2
        3,  'Punkt3
        4,  'Punkt4
        5,  'Punkt5
        6,  'Punkt6
        7,  'Punkt7
        8,  'Punkt8
        9,  'Punkt9
        10, 'Bambus1
        11, 'Bambus2
        12, 'Bambus3
        13, 'Bambus4
        14, 'Bambus5
        15, 'Bambus6
        16, 'Bambus7
        17, 'Bambus8
        18, 'Bambus9
        19, 'Symbol1
        20, 'Symbol2
        21, 'Symbol3
        22, 'Symbol4
        23, 'Symbol5
        24, 'Symbol6
        25, 'Symbol7
        26, 'Symbol8
        27, 'Symbol9
        28, 'DrachenRot
        29, 'DrachenGrün
        30, 'DrachenWeiß
        31, 'WindOst
        31, 'WindSüd
        31, 'WindWest
        31, 'WindNord
        35, 'BlütePflaume
        35, 'BlüteOrchidee
        35, 'BlüteChrisantheme
        35, 'BlüteBambus
        36, 'JahrFrühling
        36, 'JahrSommer
        36, 'JahrHerbst
        36  'JahrWinter
    }

        ''' <summary>
        ''' Gibt die SteinklickGruppe aus dem SteinIndex zurück. 
        ''' Greift auf INI.Regeln_WindsInOneClickGroup zu.
        ''' </summary>
        ''' <param name="index"></param>
        ''' <returns></returns>
        Public Function GetSteinClickGruppe(index As SteinIndexEnum) As Integer
            If INI.Spielbetrieb_WindsAreInOneClickGroup Then
                Return GruppeLookupVereinfacht(index)
            Else
                Return GruppeLookupNormal(index)
            End If
        End Function
        '
        ''' <summary>
        ''' Gibt die SteinklickGruppe aus dem SteinIndex zurück unter Auswertung von windsInOneClickGroup.
        ''' </summary>
        ''' <param name="index"></param>
        ''' <param name="windsInOneClickGroup"></param>
        ''' <returns></returns>
        Public Function GetSteinClickGruppe(index As SteinIndexEnum, windsInOneClickGroup As Boolean) As Integer
            If windsInOneClickGroup Then
                Return GruppeLookupVereinfacht(index)
            Else
                Return GruppeLookupNormal(index)
            End If
        End Function

#End Region

    End Module

End Namespace