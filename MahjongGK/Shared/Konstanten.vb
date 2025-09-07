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


Public Module Konstanten

    Public Const MJ_STEININDEX_MAX As Integer = 42

    'Die minimal mögliche Große von frmMain wird hierau bezogen.
    Public Const MJ_SPIELFELD_MIN_WIDTH As Integer = 600
    Public Const MJ_SPIELFELD_MIN_HEIGHT As Integer = 400

    'Das ist die Mindestfeldgröße, die für die Funktionen
    'Vorlagen_Pyramide und Vorlagen_Spielfeldaufteilung benötigt werden. 
    Public Const MJ_STEINE_SIDEBYSIDE_MIN As Integer = 5
    Public Const MJ_STEINE_OVERANOTHER_MIN As Integer = 5

    'Nach meinen Recherchen nach haben Mahjong-Layouts haben typischerweise
    'max. 15 bis 20 Steine nebeneinander und 8–12 Steine übereinander.
    '
    'Die hier angegebenen Grenzen stellen sicher, daß das Feld noch angezeigt
    'werden kann. Ob das sinnvoll ist, ist eine andere Frage. 
    'Und ob der Rechner ein derart großes und vollgepflastertes Feld noch
    'verdauen kann, ist wieder eine andere Frage.

    'Steine nebeneinander
    Public Const MJ_STEINE_MAXX_SIDEBYSIDE As Integer = 60
    Public Const MJ_STEINE_MAXY_OVERANOTHER As Integer = 20
    Public Const MJ_STEINE_MAXZ_LAYER As Integer = 10

    ''' <summary>
    ''' Theoretischer Wert zur Begrenzung der maximal erzeugten Steine.
    ''' </summary>
    Public Const MJ_STEINE_MAXCOUNT As Integer = MJ_STEINE_MAXX_SIDEBYSIDE * MJ_STEINE_MAXY_OVERANOTHER * MJ_STEINE_MAXZ_LAYER
    Public Const MJ_STEINE_VORRATMAXDEFAULT As Integer = 500
    Public Const MJ_STEINE_VORRATNACHSCHUBSCHWELLEDEFAULT As Integer = 250



    ''ergibt bei 30 Steinen nebeneinander 15 Steine übereinander
    'Public Const MJ_STEINE_OVERANOTHER_MAX As Integer =
    '            (MJ_STEINE_SIDEBYSIDE_MAX * MJ_GRAFIK_ORG_WIDTH * MJ_SPIELFELD_MIN_HEIGHT) \
    '            (MJ_GRAFIK_ORG_HEIGHT * MJ_SPIELFELD_MIN_WIDTH)
    'Hinweis zu obiger Formel: Es wird hier mit reinen Integer-Operationen gearbeitet.
    'Hier kann es ganz schnell passieren, daß ein Zwischenergebnis klein wird und das
    'Endergebnis durch die Rundung des Zwischenergenisses zerschossen, oder erschossen (=0)
    'wird. Daher zuerst alles multiplizieren und dann erst dividieren. 

    Public Const MJ_STEINE_SUMMESTEINE042_ALLE As Integer = 42
    Public Const MJ_STEINE_SUMMESTEINE042_NORMAL As Integer = 34
    Public Const MJ_STEINE_SUMMESTEINE042_SONDER As Integer = 8

    Public Const MJ_STEINE_SUMMESTEINE144_ALLE As Integer = 4 * MJ_STEINE_SUMMESTEINE042_NORMAL + MJ_STEINE_SUMMESTEINE042_SONDER
    Public Const MJ_STEINE_SUMMESTEINE144_NORMAL As Integer = 4 * MJ_STEINE_SUMMESTEINE042_NORMAL
    Public Const MJ_STEINE_SUMMESTEINE144_SONDER As Integer = MJ_STEINE_SUMMESTEINE042_SONDER

    Public Const MJ_STEINE_SUMMESTEINE152_ALLE As Integer = 4 * MJ_STEINE_SUMMESTEINE042_NORMAL + 2 * MJ_STEINE_SUMMESTEINE042_SONDER
    Public Const MJ_STEINE_SUMMESTEINE152_NORMAL As Integer = 4 * MJ_STEINE_SUMMESTEINE042_NORMAL
    Public Const MJ_STEINE_SUMMESTEINE152_SONDER As Integer = 2 * MJ_STEINE_SUMMESTEINE042_SONDER


    Public Const MJ_STEIN_VERTEILUNG_FAKTOR_144_NORMAL As Single = MJ_STEINE_SUMMESTEINE144_NORMAL / MJ_STEINE_SUMMESTEINE144_ALLE * 1 / MJ_STEINE_SUMMESTEINE042_NORMAL
    Public Const MJ_STEIN_VERTEILUNG_FAKTOR_144_SONDER As Single = MJ_STEINE_SUMMESTEINE144_SONDER / MJ_STEINE_SUMMESTEINE144_ALLE * 1 / MJ_STEINE_SUMMESTEINE042_SONDER
    Public Const MJ_STEIN_VERTEILUNG_FAKTOR_152_NORMAL As Single = MJ_STEINE_SUMMESTEINE152_NORMAL / MJ_STEINE_SUMMESTEINE152_ALLE * 1 / MJ_STEINE_SUMMESTEINE042_NORMAL
    Public Const MJ_STEIN_VERTEILUNG_FAKTOR_152_SONDER As Single = MJ_STEINE_SUMMESTEINE152_SONDER / MJ_STEINE_SUMMESTEINE152_ALLE * 1 / MJ_STEINE_SUMMESTEINE042_SONDER


    '
    '
    '
    Public Const MJ_GRAFIK_SRC_MAX_WIDTH_OR_HEIGHT As Integer = 600

    'Das sind "Notbremswerte", für den Spielbetrieb unbrauchbar klein.
    Public Const MJ_GRAFIK_SRC_MIN_WIDTH_OR_HEIGHT As Integer = 6
    Public Const MJ_GRAFIK_STEIN_MIN_WIDTH_OR_HEIGHT As Integer = 6

    'Public Const MJ_GRAFIK_FAKTOR_H_TO_W As Double = MJ_GRAFIK_ORG_WIDTH / MJ_GRAFIK_ORG_HEIGHT
    'Public Const MJ_GRAFIK_FAKTOR_W_TO_H As Double = MJ_GRAFIK_ORG_HEIGHT / MJ_GRAFIK_ORG_WIDTH

    Public Const MJ_MARGIN_ABSOLUT_LEFT As Integer = 10 'geradzahlige Werte nehmen
    Public Const MJ_MARGIN_ABSOLUT_TOP As Integer = 10  'für alle 4
    Public Const MJ_MARGIN_ABSOLUT_RIGHT As Integer = 10
    Public Const MJ_MARGIN_ABSOLUT_BOTTOM As Integer = 10

    'Die Hälfte. Wird benötigt um den Rahmen (falls er gezeichnet wird)
    'um das Spielfeld mittig auf den Rand zu zeichnen
    Public Const MJ_MARGIN_ABSOLUT_LEFT_HALF As Single = MJ_MARGIN_ABSOLUT_LEFT / 2
    Public Const MJ_MARGIN_ABSOLUT_TOP_HALF As Single = MJ_MARGIN_ABSOLUT_TOP / 2
    Public Const MJ_MARGIN_ABSOLUT_RIGHT_HALF As Single = MJ_MARGIN_ABSOLUT_RIGHT / 2
    Public Const MJ_MARGIN_ABSOLUT_BOTTOM_HALF As Single = MJ_MARGIN_ABSOLUT_BOTTOM / 2

    Public Const MJ_COLOR_BG_DEFAULT As Integer = &HFFD0D0D0 'helleres Grau

    ''Das ist die Verschiebung je Ebene der Steine bei maximaler Steingröße
    'Public Const MJ_OFFSET3D_MAX_LEFT As Double = 3
    'Public Const MJ_OFFSET3D_MAX_TOP As Double = 3
    'Public Const MJ_OFFSET3D_MIN_LEFT As Double = 1
    'Public Const MJ_OFFSET3D_MIN_TOP As Double = 1

    'Public Const MJ_OFFSET3D_PADDING_LEFTRIGHT As Integer = 10
    'Public Const MJ_OFFSET3D_PADDING_TOPBOTTOM As Integer = 10

    'Public Const MJ_OFFSET3DFAKTOR_MAX_LEFT As Double = MJ_OFFSET3D_MAX_LEFT / MJ_GRAFIK_ORG_WIDTH
    'Public Const MJ_OFFSET3DFAKTOR_MAX_TOP As Double = MJ_OFFSET3D_MAX_TOP / MJ_GRAFIK_ORG_HEIGHT



    Public Const DIV_DIG1 As Integer = 1                 ' 10^0
    Public Const DIV_DIG2 As Integer = 10                ' 10^1
    Public Const DIV_DIG3 As Integer = 100               ' 10^2
    Public Const DIV_DIG4 As Integer = 1000              ' 10^3
    Public Const DIV_DIG5 As Integer = 10000             ' 10^4
    Public Const DIV_DIG6 As Integer = 100000            ' 10^5
    Public Const DIV_DIG7 As Integer = 1000000            ' 10^6
    Public Const DIV_DIG8 As Integer = 10000000           ' 10^7
    Public Const DIV_DIG9 As Integer = 100000000          ' 10^8
    Public Const DIV_DIG10 As Long = 1000000000L        ' 10^9
    Public Const DIV_DIG11 As Long = 10000000000L       ' 10^10
    Public Const DIV_DIG12 As Long = 100000000000L      ' 10^11
    Public Const DIV_DIG13 As Long = 1000000000000L     ' 10^12
    Public Const DIV_DIG14 As Long = 10000000000000L    ' 10^13
    Public Const DIV_DIG17 As Long = 100000000000000000L    ' 10^17
    Public Const DIV_DIG18 As Long = 1000000000000000000L   ' 10^18


    ' Konstanten für einzelne Bits (Bitmasken)
    Public Const BIT0 As Integer = 1       ' Einerstelle Bit 0
    Public Const BIT1 As Integer = 2       ' Einerstelle Bit 1
    Public Const BIT2 As Integer = 4       ' Einerstelle Bit 2
    Public Const BIT3 As Integer = 8       ' Einerstelle Bit 3

    Public Const BIT4 As Integer = 16      ' Zehnerstelle Bit 0
    Public Const BIT5 As Integer = 32      ' Zehnerstelle Bit 1
    Public Const BIT6 As Integer = 64      ' Zehnerstelle Bit 2
    Public Const BIT7 As Integer = 128     ' Zehnerstelle Bit 3

    'Flags:
    Public Const FLAG_XOffset As Integer = BIT0
    Public Const FLAG_YOffset As Integer = BIT1
    Public Const FLAG_ToggleFlag As Integer = BIT2

    Public Const FLAG_Frei3 As Integer = BIT3
    Public Const FLAG_Frei4 As Integer = BIT4
    Public Const FLAG_Frei5 As Integer = BIT5
    Public Const FLAG_Frei6 As Integer = BIT6
    Public Const FLAG_Frei7 As Integer = BIT7

    '
    ''' <summary>
    ''' Tilde („ungefähr“, „ähnlich“, Statistik. (Verteilungen: X ~ N(0,1))
    ''' </summary>
    Public Const CHAR_UNGEFÄHR1 As Char = "~"c
    '
    ''' <summary>
    ''' Doppelte Tilde (Näherungswerte, Rundungen, z. B. π ≈ 3,14)
    ''' </summary>
    Public Const CHAR_UNGEFÄHR2 As Char = "≈"c
    '
    ''' <summary>
    ''' VORSICHT: NICHT DIKTENGLEICH! (fehlt in den meisten Schriften, daher Fallback in eine Standardschrift. -> Layout wird zerschossen.)
    ''' kongruent / entspricht (Geometrie (Dreiecke), Entsprechung, Ähnlichkeit)
    ''' </summary>
    Public Const CHAR_ENTSPRICHT As Char = "≅"c

    Public Const CHAR_DELTA As Char = "Δ"c
    '
    'Public Const CHAR_ As Char = ""c
    'Public Const CHAR_ As Char = ""c
    'Public Const CHAR_ As Char = ""c


    '--- Debug-Ausgabe
    Public Function DebugKonstantenString() As String

        Dim konst As New Dictionary(Of String, Object) From {
                                                             _
        {"MJ_STEININDEX_MAX", MJ_STEININDEX_MAX},
        {"MJ_SPIELFELD_MIN_WIDTH", MJ_SPIELFELD_MIN_WIDTH},
        {"MJ_SPIELFELD_MIN_HEIGHT", MJ_SPIELFELD_MIN_HEIGHT},
        {"MJ_STEINE_SIDEBYSIDE_MIN", MJ_STEINE_SIDEBYSIDE_MIN},
        {"MJ_STEINE_OVERANOTHER_MIN", MJ_STEINE_OVERANOTHER_MIN},
        {"MJ_STEINE_SIDEBYSIDE_MAX", MJ_STEINE_MAXX_SIDEBYSIDE},
        {"MJ_STEINE_OVERANOTHER_MAX", MJ_STEINE_MAXY_OVERANOTHER},
                                                                  _
        {"MJ_STEINE_SUMMESTEINE042_ALLE", MJ_STEINE_SUMMESTEINE042_ALLE},
        {"MJ_STEINE_SUMMESTEINE042_NORMAL", MJ_STEINE_SUMMESTEINE042_NORMAL},
        {"MJ_STEINE_SUMMESTEINE042_SONDER", MJ_STEINE_SUMMESTEINE042_SONDER},
                                                                             _
        {"MJ_STEINE_SUMMESTEINE144_ALLE", MJ_STEINE_SUMMESTEINE144_ALLE},
        {"MJ_STEINE_SUMMESTEINE144_NORMAL", MJ_STEINE_SUMMESTEINE144_NORMAL},
        {"MJ_STEINE_SUMMESTEINE144_SONDER", MJ_STEINE_SUMMESTEINE144_SONDER},
                                                                             _
        {"MJ_STEINE_SUMMESTEINE152_ALLE", MJ_STEINE_SUMMESTEINE152_ALLE},
        {"MJ_STEINE_SUMMESTEINE152_NORMAL", MJ_STEINE_SUMMESTEINE152_NORMAL},
        {"MJ_STEINE_SUMMESTEINE152_SONDER", MJ_STEINE_SUMMESTEINE152_SONDER},
                                                                             _
        {"MJ_STEIN_VERTEILUNG_FAKTOR_144_NORMAL", MJ_STEIN_VERTEILUNG_FAKTOR_144_NORMAL},
        {"MJ_STEIN_VERTEILUNG_FAKTOR_144_SONDER", MJ_STEIN_VERTEILUNG_FAKTOR_144_SONDER},
        {"MJ_STEIN_VERTEILUNG_FAKTOR_152_NORMAL", MJ_STEIN_VERTEILUNG_FAKTOR_152_NORMAL},
        {"MJ_STEIN_VERTEILUNG_FAKTOR_152_SONDER", MJ_STEIN_VERTEILUNG_FAKTOR_152_SONDER},
                                                                                         _
        {"MJ_MARGIN_ABSOLUT_LEFT", MJ_MARGIN_ABSOLUT_LEFT},
        {"MJ_MARGIN_ABSOLUT_TOP", MJ_MARGIN_ABSOLUT_TOP},
        {"MJ_MARGIN_ABSOLUT_RIGHT", MJ_MARGIN_ABSOLUT_RIGHT},
        {"MJ_MARGIN_ABSOLUT_BOTTOM", MJ_MARGIN_ABSOLUT_BOTTOM},
                                                               _
        {"MJ_MARGIN_ABSOLUT_LEFT_HALF", MJ_MARGIN_ABSOLUT_LEFT_HALF},
        {"MJ_MARGIN_ABSOLUT_TOP_HALF", MJ_MARGIN_ABSOLUT_TOP_HALF},
        {"MJ_MARGIN_ABSOLUT_RIGHT_HALF", MJ_MARGIN_ABSOLUT_RIGHT_HALF},
        {"MJ_MARGIN_ABSOLUT_BOTTOM_HALF", MJ_MARGIN_ABSOLUT_BOTTOM_HALF},
                                                                         _
        {"MJ_COLOR_BG_DEFAULT", MJ_COLOR_BG_DEFAULT}
    }


        Dim sb As New System.Text.StringBuilder
        Dim value As String
        For Each kvp As KeyValuePair(Of String, Object) In konst 'kvp = KeyValuePair 
            If kvp.Key.Contains("COLOR") Then
                ' Hex-Wert mit führendem &H ausgeben
                value = $"{kvp.Key} = {kvp.Value} (&H{kvp.Value:X8})"
            Else
                value = $"{kvp.Key} = {kvp.Value}"
            End If
            Debug.Print(value)
            sb.AppendLine(value)
        Next
        Return sb.ToString
    End Function

End Module
