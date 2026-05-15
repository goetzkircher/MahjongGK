Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

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
''
#Disable Warning IDE0079
#Disable Warning IDE1006

'''' <summary>
'''' Pfad: MahjongGK/Spielfeld/Runtime/Steinflug
'''' </summary>
''Public Class MouseAnkerVerschiebung

'    Private _srcX As Double
'    Private _srcY As Double
'    Private _dstX As Double
'    Private _dstY As Double
'    Private _stepX As Double
'    Private _stepY As Double

'    Public Sub New()

'    End Sub

'    '
'    ''' <summary>
'    ''' Verschiebt schrittweise die Ankerposition der Maus auf die Mitte
'    ''' des linken oberen Quadranten des mit der Maus gezogenen Steins.
'    ''' Wenn der Stein dort festgehalten wird, ist das Ablegen einfacher,
'    ''' da die Maus immer mittig über dem linken oberen Quadranten steht
'    ''' und dann keine Fallunterscheidungen gemacht werden müssen, auf welchem
'    ''' Quadranten die Maus aktuell steht.
'    ''' </summary>
'    ''' <param name="mouseAnkerPos">Die aktuelle Mausposition auf dem Stein</param>
'    Public Sub New(owner As SFDaten, mouseAnkerPos As Point)

'        _sfd = owner

'        Dim steps As Integer = 10

'        _srcX = CDbl(mouseAnkerPos.X)
'        _srcY = CDbl(mouseAnkerPos.Y)

'        _dstX = CDbl(_sfd.SFLay.steinWidthHalf \ 2)
'        _dstY = CDbl(_sfd.SFLay.steinHeightHalf \ 2)

'        _stepX = (_dstX - _srcX) / CDbl(steps)
'        _stepY = (_dstY - _srcY) / CDbl(steps)

'        'ZLVxxx  owner.SFRun.EditorStockMouseAnkerVerschiebung_HasValue = True
'    End Sub

'    Private _sfd As SFDaten

'    ''' <summary>
'    ''' Gibt die neue Ankerverschiebung zurück. Beim Rendern solange dem bisherigen
'    ''' Wert zuweisen, bis EditorStockMouseAnkerVerschiebung_HasValue = False.
'    ''' </summary>
'    ''' <returns></returns>
'    Public Function NewMouseAnkerPos() As Point

'        Dim finishedX As Boolean
'        Dim finishedY As Boolean

'        _srcX += _stepX
'        _srcY += _stepY

'        If _stepX >= 0.0R Then
'            finishedX = (_srcX >= _dstX)
'        Else
'            finishedX = (_srcX <= _dstX)
'        End If

'        If _stepY >= 0.0R Then
'            finishedY = (_srcY >= _dstY)
'        Else
'            finishedY = (_srcY <= _dstY)
'        End If

'        If finishedX Then
'            _srcX = _dstX
'        End If

'        If finishedY Then
'            _srcY = _dstY
'        End If

'        If finishedX AndAlso finishedY Then
'            'ZLVxxx _sfd.SFRun.EditorStockMouseAnkerVerschiebung_HasValue = False
'        End If

'        Return New Point(CInt(Math.Round(_srcX)), CInt(Math.Round(_srcY)))

'    End Function

'End Class