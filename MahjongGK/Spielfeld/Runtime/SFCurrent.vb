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

Namespace Spielfeld

    ''' <summary>
    ''' Pfad: MahjongGK/Spielfeld/Runtime
    ''' 
    ''' Hält die aktuell ausgewählten, aktiven oder referenzierten Spielfeldinstanzen. 
    ''' 
    ''' SFCur bündelt, welche Spielfelder im Moment im Anwendungskontext relevant sind. 
    ''' Dazu gehören typischerweise: 
    ''' 
    ''' - das aktuell aktive bzw. sichtbare Spielfeld, 
    ''' - das eigentliche Spiel-Spielfeld, 
    ''' - das Werkbank-Spielfeld, 
    ''' - zugehörige Identifikations- und Vergleichswerte, 
    ''' - ggf. Session- oder Renderbezüge zwischen mehreren Spielfeldinstanzen. 
    ''' 
    ''' SFCur enthält selbst keine Spielfeldlogik, sondern verwaltet nur die Referenzen 
    ''' auf die aktuell beteiligten SFInf-Instanzen. 
    ''' 
    ''' Zweck dieser Klasse ist es, den globalen Auswahl- und Umschaltzustand 
    ''' zwischen mehreren Spielfeldern klar von Render-, Layout- und Editorzuständen zu trennen. 
    ''' </summary> 
    Public Class SFCurrent

        Private ReadOnly _sfd As SFDaten

        Public Sub New(owner As SFDaten)
            _sfd = owner
        End Sub

    End Class

End Namespace
