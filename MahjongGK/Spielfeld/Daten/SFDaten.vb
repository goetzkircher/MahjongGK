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
            SFMouse = New SFMouseRuntime(Me)
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
            SFMouse = New SFMouseRuntime(Me)
        End Sub


#End Region


#Region "Tochterklassen"
        '
        Public ReadOnly Property SFInf As SFInfo
        Public ReadOnly Property SFCur As SFCurrent
        Public ReadOnly Property SFRun As SFRuntime
        Public ReadOnly Property SFLay As SFLayout
        Public ReadOnly Property SFEdi As SFEditor
        Public ReadOnly Property SFRen As SFRender
        Public ReadOnly Property SFRenMan As SFRenderManager
        Public ReadOnly Property SFUI As SFUIFace
        '
        Public ReadOnly Property SFMouse As SFMouseRuntime
#End Region


#Region "UI - Toolbox / Sichtbarkeit / Tabzustand"
        'Verbleibt bis zur Nutzung noch hier gelagert.
        Public Property ToolboxIsVisible As Boolean

        Private _toolboxTabPageChanged As Boolean
        Private disposedValue As Boolean

        Public Property ToolboxTabPageChanged As Boolean
            Get
                If _toolboxTabPageChanged Then
                    _toolboxTabPageChanged = False
                    Return True
                Else
                    Return False
                End If
            End Get
            Set(value As Boolean)
                _toolboxTabPageChanged = value
            End Set
        End Property

#End Region


#Region "Dispose"


        Private _disposed As Boolean

        Public Sub Dispose() Implements IDisposable.Dispose
            If _disposed Then
                Return
            End If

            If Me.SFRun.MousePolling IsNot Nothing Then
                Me.SFRun.MousePolling.Dispose()
            End If

            _disposed = True

            GC.SuppressFinalize(Me)

        End Sub

    End Class

#End Region

End Namespace