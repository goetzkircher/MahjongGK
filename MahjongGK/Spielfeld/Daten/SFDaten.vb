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
Namespace Spielfeld
    ''' <summary> 
    ''' Pfad: MahjongGK/Spielfeld/Daten
    ''' 
    ''' Zentrale Wurzelklasse für den gesamten globalen Laufzeitkontext des Spielfeldsystems. 
    ''' 
    ''' SFD hält keine eigentlichen persistenten Spielfelddaten, sondern die zur Laufzeit 
    ''' benötigten globalen Zustände und Teilobjekte, über die das System koordiniert wird. 
    ''' Dazu gehören insbesondere: 
    ''' 
    ''' - die aktuell aktiven bzw. sichtbaren Spielfelder, 
    ''' - globale Layout- und Anzeigebereiche, 
    ''' - globale Renderressourcen wie Backbuffer und Graphics, 
    ''' - editorbezogene Bedienzustände, 
    ''' - UI-Zustände und globale Laufzeitflags. 
    ''' 
    ''' SFDat ist damit die zentrale Zugriffsstelle für den nicht persistenten 
    ''' Anwendungszustand des Spielfeldbereichs. 
    ''' 
    ''' Wichtig: 
    ''' Fachliche Spielfeldinhalte gehören nicht nach SFDat, sondern in SFInf. 
    ''' Laufzeitwerte, die zu genau einem konkreten Spielfeld gehören, gehören 
    ''' bevorzugt in SFRun. 
    ''' </summary>
    Public Class SFDaten
        Implements IDisposable

#Region "Konstruktor"

        Public Sub New()

        End Sub

        Public Sub New(spielSize As Triple)
            SFInf = New SFInfo(Me, spielSize)
            SFCur = New SFCurrent(Me)
            SFRun = New SFRuntime(Me)
            SFLay = New SFLayout(Me)
            SFEdi = New SFEditor(Me)
            SFRen = New SFRender(Me)
            SFRenMan = New SFRenderManager(Me)
            SFUI = New SFUIFace(Me)
            'ZLVxxx   SFMouse = New SFMouseRuntime_zlv(Me)
            'ZLVSFAir = New SFAirflight(Me)
            SFMouse = New SFMouseAktion(Me)
            SFStock = New SFStockState(Me)
        End Sub

        Public Sub New(spielfeldinfo As SFInfo)
            SFInf = spielfeldinfo
            SFInf.SetOwner(Me)
            SFCur = New SFCurrent(Me)
            SFRun = New SFRuntime(Me)
            SFLay = New SFLayout(Me)
            SFEdi = New SFEditor(Me)
            SFRen = New SFRender(Me)
            SFRenMan = New SFRenderManager(Me)
            SFUI = New SFUIFace(Me)
            'ZLVxxx   SFMouse = New SFMouseRuntime_zlv(Me)
            'ZLV SFAir = New SFAirflight(Me)
            SFMouse = New SFMouseAktion(Me)
            SFStock = New SFStockState(Me)
        End Sub

#End Region

#Region "Tochterklassen"
        '
        Public Property SFInf As SFInfo
        Public Property SFCur As SFCurrent
        Public Property SFRun As SFRuntime
        Public Property SFLay As SFLayout
        Public Property SFEdi As SFEditor
        Public Property SFRen As SFRender
        Public Property SFRenMan As SFRenderManager
        Public Property SFUI As SFUIFace
        '
        ''ZLVxxx   Public Property SFMouse As SFMouseRuntime_zlv
        'Public Property SFAir As SFAirflight

        Public Property SFMouse As SFMouseAktion

        Public Property SFStock As SFStockState
#End Region

#Region "Dispose"

        Private _disposed As Boolean

        Public Sub Dispose() Implements IDisposable.Dispose
            If _disposed Then
                Return
            End If

            _disposed = True

            If SFLay IsNot Nothing Then
                SFLay.Dispose()
                SFLay = Nothing
            End If

            If SFRenMan IsNot Nothing Then
                SFRenMan.Dispose()
                SFRenMan = Nothing
            End If

            GC.SuppressFinalize(Me)

        End Sub

    End Class

#End Region

End Namespace