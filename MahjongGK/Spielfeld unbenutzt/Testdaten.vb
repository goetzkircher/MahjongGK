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



Namespace Spielfeld
    '
    ''' <summary>
    ''' Hier sind Testdaten drin, und Code, den ich zur Erstellung dieser Testdaten
    ''' bei der Programmentwicklung gebraucht habe, die aber im fertigem Programm ohne
    ''' Bedeutung sind.
    ''' </summary>
    Module Testdaten

        Public Sub TestDaten_Spielfeld_3_x_3_x_1()

            '  TODO
            Dim newSpielfeldInfo As New SFDaten(New Triple(3, 3, 1))

            With newSpielfeldInfo.SFInf
                Dim centerXyz As Triple = .GetSpielfeldCenter(0)
                .AddSteinToSpielfeld(SteinIndexEnum.Bambus1, centerXyz)

                Dim tplr As Triple
                tplr = .SearchPlace(centerXyz, Direction.Left)
                If tplr.Valide = ValidePlace.Yes Then
                    .AddSteinToSpielfeld(SteinIndexEnum.Bambus2, tplr)
                End If

                tplr = .SearchPlace(centerXyz, Direction.LeftUp)
                If tplr.Valide = ValidePlace.Yes Then
                    .AddSteinToSpielfeld(SteinIndexEnum.Bambus3, tplr)
                End If

                tplr = .SearchPlace(centerXyz, Direction.RightDown)
                If tplr.Valide = ValidePlace.Yes Then
                    .AddSteinToSpielfeld(SteinIndexEnum.Bambus4, tplr)
                End If

                tplr = .SearchPlace(tplr, Direction.Up)
                If tplr.Valide = ValidePlace.Yes Then
                    .AddSteinToSpielfeld(SteinIndexEnum.Bambus5, tplr)
                End If

                tplr = .SearchPlace(tplr, Direction.Up)
                If tplr.Valide = ValidePlace.Yes Then
                    .AddSteinToSpielfeld(SteinIndexEnum.Bambus6, tplr)
                End If

            End With




            'DebugHilfen.Print3DArrayToTxtOutputForm(SFD.AktSpielfeldInfo)
            '   DebugShowArrFBMain(arrFBMain, 0)

        End Sub


    End Module
End Namespace
