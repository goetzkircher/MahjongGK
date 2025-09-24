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



Namespace Spielfeld
    '
    ''' <summary>
    ''' Hier sind Testdaten drin, und Code, den ich zur Erstellung dieser Testdaten
    ''' bei der Programmentwicklung gebraucht habe, die aber im fertigem Programm ohne
    ''' Bedeutung sind.
    ''' </summary>
    Module Testdaten

        Public Sub TestDaten_Spielfeld_3_x_3_x_1()

            Dim newSpielfeldInfo As New SpielfeldInfo(New Triple(3, 3, 1), SpielfeldOrEditorMode.Spielfeld)

            With newSpielfeldInfo
                Dim centerXyz As Triple = .GetSpielfeldCenter(0)
                .AddSteinToSpielfeld(SteinIndexEnum.Bambus1, centerXyz)

                Dim tplr As Triple
                tplr = .SearchPlace(centerXyz, Direction.Left)
                If tplr.Valide = ValidePlaceEnum.Yes Then
                    .AddSteinToSpielfeld(SteinIndexEnum.Bambus2, tplr)
                End If

                tplr = .SearchPlace(centerXyz, Direction.LeftUp)
                If tplr.Valide = ValidePlaceEnum.Yes Then
                    .AddSteinToSpielfeld(SteinIndexEnum.Bambus3, tplr)
                End If

                tplr = .SearchPlace(centerXyz, Direction.RightDown)
                If tplr.Valide = ValidePlaceEnum.Yes Then
                    .AddSteinToSpielfeld(SteinIndexEnum.Bambus4, tplr)
                End If

                tplr = .SearchPlace(tplr, Direction.Up)
                If tplr.Valide = ValidePlaceEnum.Yes Then
                    .AddSteinToSpielfeld(SteinIndexEnum.Bambus5, tplr)
                End If

                tplr = .SearchPlace(tplr, Direction.Up)
                If tplr.Valide = ValidePlaceEnum.Yes Then
                    .AddSteinToSpielfeld(SteinIndexEnum.Bambus6, tplr)
                End If

            End With




            'DebugHilfen.Print3DArrayToTxtOutputForm(SFD.AktSpielfeldInfo)
            '   DebugShowArrFBMain(arrFBMain, 0)

        End Sub
        Public Sub TestDaten_Spielfeld_Methodenaufruf_zum_Debuggen()

            Dim newSpielfeld As New SpielfeldInfo(New Triple(20, 6, 10), SpielfeldOrEditorMode.Spielfeld)
            Dim newWerkbank As New SpielfeldInfo(New Triple(5, 5, 10), SpielfeldOrEditorMode.Editor)

            SFD.SpielerSpielfeldInfo = newSpielfeld
            SFD.WerkbankSpielfeldInfo = newWerkbank

            ' Das Polling läuft bereits Spielfeld.TaktgeberModul.PaintSpielfeld_ReInitialisierung()

            Dim wbsSF As Werkstück = Umfeld.Werkstück_Rechteck(New Triple(5, 5, 10), demoMode:=True) ', True, True)
            'Dim wbs As Werkstück = Umfeld.Werkstück_Rechteck(New Triple(5, 5, 25))
            Dim wbsWB As Werkstück = Umfeld.Werkstück_Pyramide(New Triple(5, 5, 10), True, True, demoMode:=True)
            Dim wbsEd As Werkstück = Umfeld.Werkstück_Pyramide(New Triple(10, 5, 4), True, True, demoMode:=True)

            newSpielfeld.AddWerkstückToSpielfeld(wbsWB, New Triple(1, 1, 0))
            newWerkbank.AddWerkstückToSpielfeld(wbsSF, New Triple(1, 1, 0))

            'With newSpielfeldInfo

            '    Dim tplr As New Triple

            '    For idx As Integer = PositionEnum.LBnd To PositionEnum.UBnd
            '        Dim tpl As Triple = .GetPositionImSpielfeld(DirectCast(idx, PositionEnum), New Triple(5, 5, 0), 5, 5)
            '        .AddSteinToSpielfeld(DirectCast(idx + 1, SteinIndexEnum), tpl, tmpDebug:=idx)
            '        DebugStep.WaitForStep()
            '    Next
            'End With
        End Sub

        'DebugHilfen.Print3DArrayToTxtOutputForm(newSpielfeldInfo)


    End Module
End Namespace
