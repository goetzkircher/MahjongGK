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
    ''' Pfad: MahjongGK/Spielfeld/Editor
    ''' 
    ''' Hält alle editorbezogenen Laufzeit- und Bedienzustände. 
    ''' 
    ''' SFEdi bündelt die Daten, die ausschließlich oder überwiegend im Editorbetrieb 
    ''' benötigt werden. Dazu gehören insbesondere: 
    ''' 
    ''' - Werte des Steinvorrats, 
    ''' - Ghost-/Drag-&amp;-Drop-Daten, 
    ''' - Flug- und Mausankerzustände, 
    ''' - editorbezogene Zwischenzustände, 
    ''' - ausstehende Platzierungsaktionen, 
    ''' - weitere Hilfsdaten für die Bearbeitung des Spielfelds. 
    ''' 
    ''' SFEdi enthält keine eigentlichen persistenten Spielfelddaten, 
    ''' sondern ausschließlich editorbezogene Interaktionszustände. 
    ''' 
    ''' Zweck dieser Klasse ist die klare Trennung zwischen: 
    ''' - dem fachlichen Spielfeldmodell, 
    ''' - dem globalen UI-/Renderzustand 
    ''' - und den temporären Zuständen des Editorbetriebs. 
    ''' </summary> 
    Public Class SFEditor

        Public Sub New()

        End Sub

        Public Sub New(owner As SFDaten)
            _sfd = owner
        End Sub

        Private ReadOnly _sfd As SFDaten



    End Class
End Namespace