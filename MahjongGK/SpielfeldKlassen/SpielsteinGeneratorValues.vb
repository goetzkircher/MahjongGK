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

'
Imports MahjongGK.Spielfeld



#Disable Warning IDE0079
#Disable Warning IDE1006


Public Class SpielsteinGeneratorValues

    Sub New()

    End Sub

    Public Property SchemaVersion As Integer = 1
    Public Property GeneratorVersion As Integer = 1

    Public Property GeneratorModusForXml As GeneratorModus
    Public Property HalfSteinsetsCount As Integer
    Public Property StoneSet152SteineErzeugen As Boolean
    Public Property Vorrat As List(Of SteinIndexEnum)
    Public Property VorratMaxUBound As Integer
    Public Property VorratNachschubschwelle As Integer
    Public Property VorratStopNachschub As Boolean
    '
    Public Property VorratStopNachschubXmlOnly As Boolean
    Public Property VorratSelectedIndex As Integer = -1 'Kein Stein ausgewählt
    Public Property VorratNoSortAreaEndIndex As Integer

    Public Sub CopySpielsteinGeneratorValues_To_SpielsteinGenerator(gen As SpielsteinGenerator)

        With gen
            SchemaVersion = .SchemaVersion
            GeneratorVersion = .GeneratorVersion
            GeneratorModusForXml = .GeneratorModusForXml
            HalfSteinsetsCount = .HalfSteinsetsCount
            StoneSet152SteineErzeugen = .StoneSet152SteineErzeugen
            Vorrat = .Vorrat
            VorratMaxUBound = .VorratMaxUBound
            VorratNachschubschwelle = .VorratNachschubschwelle
            VorratStopNachschub = .VorratStopNachschub
            VorratStopNachschubXmlOnly = .VorratStopNachschubXmlOnly
            VorratSelectedIndex = .VorratSelectedIndex
            VorratNoSortAreaEndIndex = .VorratNoSortAreaEndIndex
        End With

    End Sub
    Public Sub CopySpielsteinGenerator_To_SpielsteinGeneratorValues(gen As SpielsteinGenerator)
        With gen
            SchemaVersion = .SchemaVersion
            GeneratorVersion = .GeneratorVersion
            GeneratorModusForXml = .GeneratorModusForXml
            HalfSteinsetsCount = .HalfSteinsetsCount
            StoneSet152SteineErzeugen = .StoneSet152SteineErzeugen
            Vorrat = .Vorrat
            VorratMaxUBound = .VorratMaxUBound
            VorratNachschubschwelle = .VorratNachschubschwelle
            VorratStopNachschub = .VorratStopNachschub
            VorratStopNachschubXmlOnly = .VorratStopNachschubXmlOnly
            VorratSelectedIndex = .VorratSelectedIndex
            VorratNoSortAreaEndIndex = .VorratNoSortAreaEndIndex
        End With
    End Sub

End Class
