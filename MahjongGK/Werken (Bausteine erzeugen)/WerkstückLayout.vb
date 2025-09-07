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
''' <summary>
''' Die Basisdaten, die zur Bausteinerzeugung im Edior benötigt werden.
''' </summary>
Public Class WerkstückLayout

    Sub New()

    End Sub

    Public Property Basisform As BasisformEnum
    Public Property NameBasisformForSaving As String
    Public Property FeldSizeXmax_MinValue As Integer
    Public Property FeldSizeYmax_MinValue As Integer
    Public Property FeldSizeXmax_MaxValue As Integer
    Public Property FeldSizeYmax_MaxValue As Integer
    Public Property FeldSizeZmax_MaxValue As Integer
    Public Property FeldSizeXmax_Value As Integer
    Public Property FeldSizeYmax_Value As Integer
    Public Property FeldSizeZmax_Value As Integer


End Class
