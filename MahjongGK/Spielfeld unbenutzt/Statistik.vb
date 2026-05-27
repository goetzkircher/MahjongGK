Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

Imports MahjongGK.Contracts

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
#Disable Warning IDE0079
#Disable Warning IDE1006
'
''' <summary>
''' Berechnet Statistik über Mahjong-Steine im Vorrat
''' </summary>
Public Class Statistik

    Private ReadOnly _werteVorrat(,) As Single = Nothing
    Private ReadOnly _anzahlVorrat As Integer

    Private ReadOnly _werteSpielfeld(,) As Single = Nothing
    Private ReadOnly _anzahlSpielfeld As Integer

    Public Enum StatistikResult
        SumAbsVorrat
        ProzImVorrat
        SollProz144
        SollProz152
    End Enum

    Public Sub New(steineVorrat As List(Of SteinSymbol), steineSpielfeld As List(Of SteinSymbol))

        If steineVorrat Is Nothing Then
            _anzahlVorrat = 0
        Else
            _anzahlVorrat = steineVorrat.Count
        End If

        If steineSpielfeld Is Nothing Then
            _anzahlSpielfeld = 0
        Else
            _anzahlSpielfeld = steineSpielfeld.Count
        End If

        If _anzahlVorrat = 0 And _anzahlSpielfeld = 0 Then
            Exit Sub
        End If

        If _anzahlVorrat > 0 Then
            _werteVorrat = WerteBerechnen(steineVorrat)
        End If

        If _anzahlSpielfeld > 0 Then
            _werteSpielfeld = WerteBerechnen(steineSpielfeld)
        End If

    End Sub

    Private Function WerteBerechnen(steine As List(Of SteinSymbol)) As Single(,)

        Dim werte(42, [Enum].GetValues(GetType(StatistikResult)).Length - 1) As Single

        ' 1. Absolut zählen
        Dim counts(42) As Integer
        Dim sumcount As Integer
        For Each sie As SteinSymbol In steine
            If sie <> SteinSymbol.ErrorSy Then
                counts(CInt(sie)) += 1
                sumcount += 1
            End If
        Next
        werte(0, StatistikResult.SumAbsVorrat) = sumcount

        ' 2. Prozente im Vorrat
        For idx As Integer = 1 To MJ_STEININDEX_MAX

            Dim abs As Integer = counts(idx)
            werte(idx, StatistikResult.SumAbsVorrat) = abs
            If steine.Count > 0 Then
                werte(idx, StatistikResult.ProzImVorrat) = CSng(abs / steine.Count * 100.0!)
            End If
        Next

        ' 3. Soll-Verteilungen berechnen
        ' Normale Steine: 34 * 4 = 136
        ' Sondersteine:   8 * 1 oder * 2 = 8 oder 16
        For idx As Integer = 1 To MJ_STEININDEX_MAX

            Dim isNormal As Boolean = (idx <= CInt(SteinSymbol.WindNrd))
            Dim soll144 As Single
            Dim soll152 As Single

            If isNormal Then
                soll144 = MJ_STEIN_VERTEILUNG_FAKTOR_144_NORMAL * 100
                soll152 = MJ_STEIN_VERTEILUNG_FAKTOR_152_NORMAL * 100
            Else
                soll144 = MJ_STEIN_VERTEILUNG_FAKTOR_144_SONDER * 100
                soll152 = MJ_STEIN_VERTEILUNG_FAKTOR_152_SONDER * 100
            End If

            werte(idx, StatistikResult.SollProz144) = soll144
            werte(idx, StatistikResult.SollProz152) = soll152
        Next

        Return werte

    End Function

    '' ''' <summary>
    '' ''' Zugriff auf einzelne Werte
    '' ''' </summary>
    ''Public ReadOnly Property WertVorrat(stein As SteinSymbol, res As StatistikResult) As Single
    ''    Get
    ''        Return _werteVorrat(CInt(stein), CInt(res))
    ''    End Get
    ''End Property

    Public Function Abweichungen(arrDeltaProzent() As Single) As (maxAbw As Single, avgAbw As Single, stdAbw As Single)

        Dim n As Integer = MJ_STEINE_SUMMESTEINE042_ALLE
        Dim sumAbs As Double = 0
        Dim sumSquares As Double = 0
        Dim maxAbs As Double = 0

        ' Elemente 1..n auswerten (Index 0 ist reserviert)
        For i As Integer = 1 To n
            Dim val As Double = arrDeltaProzent(i)
            Dim absVal As Double = Math.Abs(val)

            sumAbs += absVal
            sumSquares += val * val

            If absVal > maxAbs Then maxAbs = absVal
        Next

        Dim avgAbs As Double = sumAbs / n
        ' Standardabweichung = Wurzel(Mittelwert der Quadrate)
        Dim stdDev As Double = Math.Sqrt(sumSquares / n)

        Return (CSng(maxAbs), CSng(avgAbs), CSng(stdDev))

    End Function

    Public Overloads Function ToString(res As StatistikResult) As String
        Dim sb As New System.Text.StringBuilder
        For i As Integer = 1 To MJ_STEININDEX_MAX
            sb.AppendLine($"{DirectCast(i, SteinSymbol),-8}  {_werteVorrat(i, res):F3}")
        Next
        Return sb.ToString()
    End Function

    ''' <summary>
    ''' Gibt Tabelle als String zurück
    ''' </summary>
    Public Overloads Function ToString(deltaProz144 As Boolean, Optional arrSpielfeld() As SteinSymbol = Nothing) As String

        If _anzahlVorrat = 0 AndAlso _anzahlSpielfeld = 0 Then
            Return "Es sind weder Vorratsdaten, noch Spielfelddaten vorhanden. (In Beiden, Stock und Spielfeld, ist die Anzahl der Steine = 0)"
        End If

        Dim sb As New System.Text.StringBuilder

        If _anzahlVorrat > 0 Then
            sb.Append($"Summe der Steine im Stock: {_werteVorrat(0, StatistikResult.SumAbsVorrat)} Steine. ")

            If deltaProz144 Then
                sb.AppendLine($"Vergleichsbasis: Spielfeld mit {MJ_STEINE_SUMMESTEINE144_ALLE} Steinen.")
            Else
                sb.AppendLine($"Vergleichsbasis: Spielfeld mit {MJ_STEINE_SUMMESTEINE152_ALLE} Steinen.")
            End If

            sb.AppendLine()

            Dim arrDeltaProzent() As Single = ToStringMontage(sb, _werteVorrat, deltaProz144)

            Dim abw As (maxAbw As Single, avgAbw As Single, stdAbw As Single) = Abweichungen(arrDeltaProzent)
            sb.Append($"  In Summe: Maximale Abweichung {abw.maxAbw:F2}% Standardabweichung {abw.stdAbw:F2}%")
            sb.AppendLine($" Anzahl der Steine im Stock: {_anzahlSpielfeld} Steine. ")

        End If

        If _anzahlVorrat > 0 AndAlso _anzahlSpielfeld > 0 Then
            sb.AppendLine()
            sb.AppendLine(New String("-"c, 230))
            sb.AppendLine()
        End If

        If _anzahlSpielfeld > 0 Then

            If deltaProz144 Then
                sb.AppendLine($"Vergleichsbasis: Spielfeld mit {MJ_STEINE_SUMMESTEINE144_ALLE} Steinen.")
            Else
                sb.AppendLine($"Vergleichsbasis: Spielfeld mit {MJ_STEINE_SUMMESTEINE152_ALLE} Steinen.")
            End If

            sb.AppendLine()

            Dim arrDeltaProzent() As Single = ToStringMontage(sb, _werteSpielfeld, deltaProz144)

            Dim abw As (maxAbw As Single, avgAbw As Single, stdAbw As Single) = Abweichungen(arrDeltaProzent)
            sb.Append($"  In Summe: Maximale Abweichung {abw.maxAbw:F2}% Standardabweichung {abw.stdAbw:F2}%")
            sb.AppendLine($" Anzahl der Steine im Stock: {_anzahlSpielfeld } Steine.")

        End If

        sb.AppendLine()
        sb.AppendLine(New String("-"c, 230))
        sb.AppendLine()

        Return sb.ToString()

    End Function

    Private Function ToStringMontage(sb As System.Text.StringBuilder, werte(,) As Single, deltaProz144 As Boolean) As Single()

        Dim arrDeltaProzent(MJ_STEINE_SUMMESTEINE042_ALLE) As Single

        Dim header1 As String = "│ Stein       Anzahl Prozent Prozent Differenz"
        Dim header2 As String = "│                      ist     soll           "

        For pass As Integer = 1 To 5
            sb.Append(header1)
        Next
        sb.AppendLine(("|"))
        For pass As Integer = 1 To 5
            sb.Append(header2)
        Next
        sb.AppendLine(("|"))

        ' Sortierte Reihenfolge 
        'Hinweis SteinSymbol.ErrorSy dient hier als Platzhalter
        Dim reihenfolge As SteinSymbol() =
        {
            SteinSymbol.Punkt01, SteinSymbol.Bambus1, SteinSymbol.Symbol1, SteinSymbol.DracheR, SteinSymbol.BlütePf,
            SteinSymbol.Punkt02, SteinSymbol.Bambus2, SteinSymbol.Symbol2, SteinSymbol.DracheG, SteinSymbol.BlüteOr,
            SteinSymbol.Punkt03, SteinSymbol.Bambus3, SteinSymbol.Symbol3, SteinSymbol.DracheW, SteinSymbol.BlüteCt,
            SteinSymbol.Punkt04, SteinSymbol.Bambus4, SteinSymbol.Symbol4, SteinSymbol.ErrorSy, SteinSymbol.BlüteBa,
            SteinSymbol.Punkt05, SteinSymbol.Bambus5, SteinSymbol.Symbol5, SteinSymbol.ErrorSy, SteinSymbol.ErrorSy,
            SteinSymbol.Punkt06, SteinSymbol.Bambus6, SteinSymbol.Symbol6, SteinSymbol.WindOst, SteinSymbol.JahrFrl,
            SteinSymbol.Punkt07, SteinSymbol.Bambus7, SteinSymbol.Symbol7, SteinSymbol.WindSüd, SteinSymbol.JahrSom,
            SteinSymbol.Punkt08, SteinSymbol.Bambus8, SteinSymbol.Symbol8, SteinSymbol.WindWst, SteinSymbol.JahrHer,
            SteinSymbol.Punkt09, SteinSymbol.Bambus9, SteinSymbol.Symbol9, SteinSymbol.WindNrd, SteinSymbol.JahrWin
        }

        Dim count As Integer
        For Each sie As SteinSymbol In reihenfolge
            'If CType(sie, SteinSymbol) = SteinSymbol.Punkt06 Then
            '    Stop
            'End If
            Dim sumAbsVorrat As Single = werte(CInt(sie), StatistikResult.SumAbsVorrat)
            Dim prozImVorrat As Single = werte(CInt(sie), StatistikResult.ProzImVorrat)
            Dim prozSoll As Single

            If deltaProz144 Then
                prozSoll = werte(sie, StatistikResult.SollProz144)
            Else
                prozSoll = werte(sie, StatistikResult.SollProz152)
            End If

            arrDeltaProzent(sie) = prozImVorrat - prozSoll

            If sie = SteinSymbol.ErrorSy Then
                sb.Append($"{"│",-46}")
            Else
                sb.Append($"│ {sie.ToString & ":",-8} = {sumAbsVorrat,3} ~ {prozImVorrat,6:F2}% {prozSoll,6:F2}% {"Δ"}={prozImVorrat - prozSoll,7:+0.00;-0.00;0.00}% ")
            End If

            count += 1
            If count = 5 Then
                sb.AppendLine("│")
                count = 0
            End If
        Next

        sb.AppendLine()

        Return arrDeltaProzent

    End Function

End Class
