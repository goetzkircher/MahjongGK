Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On
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
#Disable Warning IDE0079
#Disable Warning IDE1006
'
Namespace Umfeld
    '
    ''' <summary>
    ''' Hier stehen Basisbausteine, wie Linie, Diagonale, Pyramiden oder ähnliches. 
    ''' </summary>
    Public Module Werkstücke

        Private ReadOnly _rnd As New Random

        'Hinweis 1: Die Werkstatt nicht in den Namespace Spielfeld verschieben,
        'das gibt garantiert Ärger, weil übliche Deklarationen kollidieren können.
        'Diese Fehler sind schwer zu finden. (Stichwort: die 3D-Koordinaten)

        'Hinweis 2: Alle Member der Werkbank sind auch in SpielfeldInfo zu finden,
        '           von dort stammen sie, Copy-Paste :-) also nicht verwechseln.
        '           Das Problem war, daß Werkbank und SpielfeldInfo auf unabhängige
        '           Instanzen der SpielfeldInfoValues im zugreifen müssen und mein
        '           Konzept das nicht hergab. Eine Umstellung des Konzeptes auf
        '           Klassen hätte das für mich völlig unübersichtlich gemacht,
        '           deshalb die Copy-Paste-Lösung in die Werkstatt.
        '           (Man muss dazu wissen: Ich kenne zwar Vb.Net von Anfang ,
        '           habe aber zwischen 2010 und 2020 keine einzige Zeile programmiert,
        '           da ich beruflich was anderes gemacht habe. Erst als Rentner habe
        '           ich als Hobby wieder angefangen. Da fehlen mir 10 Jahre Vb.Net
        '           Entwickung, die nachzuholen mir schwer fällt.)
        '
        'Hinweis 3: In der WerkBank beim Adden von Steines ist es egal, welche
        '           SteinIndexEnum genommen wird.
        '           Die WerkBank trägt immer den SteinStatus.WerkstückEinfügeFehler ein und
        '           setzt das Flag IsWerkBankStein.
        '           Vor dem Adden zum Spielfeld muss der Baustein durch den
        '           Spielsteinaustauscher gejagt werden, der dann die Steine
        '           ersetzt durch Steine aus dem aktuellem SteinVorrat.
        '           Das kann auch später nach dem Adden erfolgen, dann müssen aber
        '           alle Steine des Spielfeldes ausgetauscht werden, um sicherzustellen
        '           daß das Spielfeld in sich konsistent ist.
        '
        'Hinweis 4: bausteinSize As Triple gibt die Anzahl der Steine an, die
        '           die Nebeninander, Übereinander und Aufeinander Stehen,
        '           nicht die Dimensionen. Darau ergeben sich dann die Werte
        '           .xMin, .xMax, .yMin, .yMax, .zMin, .zMax
        '           Das sind NICHT LBound und UBounf, zumindest in der x/y-Ebene nicht,
        '           weil eine leere Spalte/Zeile um das Spielfeld gelegt ist,
        '           um keine Ausnahmefälle für das Zugreifen auf die Felder 
        '           x+1, x-1, y+1 und y-1 zu haben. (Die ja immer unbelegt sind
        '           und deshalb immer den Wert = 0 = unbelegt zurückgegeben.)
        '
        '           In AddWerkstückToSpielfeld sind die steinPos3D aber
        '           SpielfeldKoordinaten und die Unterscheiden sich erheblich,
        '           weil jeder Stein 2 mal 2 Felder belegt.
        '
        Public Function Werkstück_Grundgerüst(bausteinSize As Triple, demoMode As Boolean) As Werkstück

            Dim wb As New Werkbank(bausteinSize)

            With wb
                For idxZ As Integer = .zMin To .zMax
                    For idxX As Integer = .xMin To .xMax
                        For idxY As Integer = .yMin To .yMax
                            Dim tplQuestion As New Triple(idxX, idxY, idxZ)
                            Dim steinPos3D As Triple = wb.IsValidePlace(tplQuestion)
                            If steinPos3D.Valide = ValidePlace.Yes Then
                                'Stein mit zufälliger SteinIndexEnum setzen.
                                wb.AddSteinToSpielfeld(CType(_rnd.Next(1, 43), SteinIndexEnum), steinPos3D)
                            End If
                        Next
                    Next
                Next
            End With

            Dim result As (steinInfos As List(Of Spielfeld.SteinInfo), arrFB As Integer(,,)) = wb.ResultAsSteinInfosArrFB

            Return wb.ResultAsWerkstück(demoMode)

        End Function

        Public Function Werkstück_Rechteck(bausteinSize As Triple, demoMode As Boolean) As Werkstück

            Dim wb As New Werkbank(bausteinSize)

            With wb
                For idxZ As Integer = .zMin To .zMin + 1
                    For idxX As Integer = .xMin To .xMax Step 2
                        For idxY As Integer = .yMin To .yMax Step 2
                            If idxZ = 1 And idxX > 4 Then
                                Continue For
                            End If
                            Dim tplQuestion As New Triple(idxX, idxY, idxZ)
                            Dim steinPos3D As Triple = wb.IsValidePlace(tplQuestion)
                            If steinPos3D.Valide = ValidePlace.Yes Then
                                wb.AddSteinToSpielfeld(CType(_rnd.Next(1, 43), SteinIndexEnum), steinPos3D)
                            End If
                        Next
                    Next
                Next
            End With

            Dim result As (steinInfos As List(Of Spielfeld.SteinInfo), arrFB As Integer(,,)) = wb.ResultAsSteinInfosArrFB

            Return wb.ResultAsWerkstück(demoMode)

        End Function

        Public Function Werkstück_Pyramide(bausteinSize As Triple, halfStepX As Boolean, halfStepY As Boolean, demoMode As Boolean) As Werkstück

            Dim wb As New Werkbank(bausteinSize)

            Dim mulX As Integer = If(halfStepX, 1, 2)
            Dim mulY As Integer = If(halfStepY, 1, 2)

            With wb
                For idxZ As Integer = .zMin To .zMax
                    For idxX As Integer = .xMin + idxZ * mulX To .xMax - idxZ * mulX Step 2
                        For idxY As Integer = .yMin + idxZ * mulY To .yMax - idxZ * mulY Step 2
                            Dim tplQuestion As New Triple(idxX, idxY, idxZ)
                            Dim tplAnswer As Triple = wb.IsValidePlace(tplQuestion)
                            If tplAnswer.Valide = ValidePlace.Yes Then
                                wb.AddSteinToSpielfeld(CType(_rnd.Next(1, 43), SteinIndexEnum), tplAnswer)
                            End If
                        Next
                    Next
                Next
            End With

            Dim result As (steinInfos As List(Of Spielfeld.SteinInfo), arrFB As Integer(,,)) = wb.ResultAsSteinInfosArrFB

            Return wb.ResultAsWerkstück(demoMode)

        End Function

        Public Function Werkstück_Ellipse(bausteinSize As Triple, demoMode As Boolean) As Werkstück

            Dim wb As New Werkbank(bausteinSize)

            Dim xFrom As Integer
            Dim xTo As Integer
            Dim yFrom As Integer
            Dim yTo As Integer

            With wb
                .xMax += 1
                .yMax += 1

                For idxZ As Integer = .zMin To .zMax

                    Dim offset As Integer = idxZ ' If (idxZ = 0, 0, 1) Then

                    xFrom = .xMin + offset
                    xTo = .xMax - offset

                    For idxX As Integer = xFrom To xTo Step 2

                        yFrom = .yMin + offset
                        yTo = .yMax - offset

                        For idxY As Integer = yFrom To yTo Step 2
                            If Umfeld.IsInsideEllipse(idxX, xFrom, xTo, idxY, yFrom, yTo) Then

                                Dim tplQuestion As New Triple(idxX, idxY, idxZ)
                                Dim tplAnswer As Triple = wb.IsValidePlace(tplQuestion)

                                If tplAnswer.Valide = ValidePlace.Yes Then
                                    'Das ist hier nur eine Sicherheitsgurt.
                                    '(Schützt vor Programmierfehlern, die sichtbar werden, weil der Stein dann fehlt.)
                                    wb.AddSteinToSpielfeld(CType(_rnd.Next(1, 43), SteinIndexEnum), tplAnswer)
                                End If
                            End If
                        Next
                    Next
                Next
            End With

            Dim result As (steinInfos As List(Of Spielfeld.SteinInfo), arrFB As Integer(,,)) = wb.ResultAsSteinInfosArrFB

            Return wb.ResultAsWerkstück(demoMode)

        End Function

    End Module

End Namespace