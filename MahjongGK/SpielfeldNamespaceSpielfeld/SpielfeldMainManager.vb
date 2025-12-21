'
' SPDX-License-Identifier: GPL-3.0-or-later
'###########################################################################
'#                                                                         #
'#   Copyright © 2025–2026 Götz Kircher <MahjongGK@t-online.de>            #
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
Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On
'
#Disable Warning IDE0079
#Disable Warning IDE1006

Namespace Spielfeld
    ''' <summary>
    ''' Hauptmodul im Namespace Spielfeld.
    ''' Hier sind Methoden zusammengefaßt, die für das Spielfeld insgesamt zuständig sind.
    ''' </summary>
    Public Module SpielfeldMainManager

        ''' <summary>
        ''' Stellt sicher, daß in Spielfeld.SFD ein SpielfeldInfo zur Verfügung steht,
        ''' damit die aktuelle Rendering-Einstellung (startRenderingWithOrToggleTo)
        ''' passend sind.
        ''' </summary>
        ''' <param name="startRenderingWithOrToggleTo"></param>
        Public Sub EnsureSpielfeldInfoAreAvailable(startRenderingWithOrToggleTo As RenderingEnum)

            Select Case startRenderingWithOrToggleTo
                Case RenderingEnum.Spielfeld, RenderingEnum.Editor
                    If SFD.SpielfeldSpielfeldInfo Is Nothing Then
                        frmMain.InitialisierungSpielfeldEditorAndWerkbank(startRenderingWithOrToggleTo)
                    End If

                Case RenderingEnum.Werkbank
                    If SFD.WerkbankSpielfeldInfo Is Nothing Then
                        frmMain.InitialisierungSpielfeldEditorAndWerkbank(startRenderingWithOrToggleTo)
                    End If
                Case Else
            End Select
            'vorläufiger Code, bis SpielfeldEditor und Werkbank fertig sind
        End Sub


    End Module
End Namespace