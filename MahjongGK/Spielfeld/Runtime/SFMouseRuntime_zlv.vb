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
Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

'
#Disable Warning IDE0079
#Disable Warning IDE1006

' ZLVxxx Namespace Spielfeld
' ZLVxxx     ''' <summary>
' ZLVxxx     ''' Pfad: MahjongGK/Spielfeld/Runtime
' ZLVxxx     ''' </summary>
' ZLVxxx     Public Class SFMouseRuntime_zlv
' ZLVxxx 
' ZLVxxx         Public Sub New()
' ZLVxxx 
' ZLVxxx         End Sub
' ZLVxxx 
' ZLVxxx         Private ReadOnly _sfd As SFDaten
' ZLVxxx 
' ZLVxxx         Public Sub New(owner As SFDaten)
' ZLVxxx             _sfd = owner
' ZLVxxx         End Sub
' ZLVxxx 
' ZLVxxx         Private Enum LeftMouseJob
' ZLVxxx             None
' ZLVxxx             StockScrollbar
' ZLVxxx             MoveSelectedStein
' ZLVxxx         End Enum
' ZLVxxx         '################################################
' ZLVxxx 
' ZLVxxx         Private _leftMouseJob As LeftMouseJob
' ZLVxxx 
' ZLVxxx         Public Sub UpdateMouseValues(outputRect As Rectangle, somethingMouseDone As Boolean, spielfeldIsUpdated As Boolean)
' ZLVxxx 
' ZLVxxx             If Not IsNothing(_sfd.SFRun.EditorStockValues) Then
' ZLVxxx                 With _sfd.SFRun.EditorStockValues
' ZLVxxx                     If .StockSelectedSteinJob = SelectedSteinJob.FlyBack_Done Then
' ZLVxxx                         .ClearGhostIdx() 'Ghost wird nicht mehr angezeigt.
' ZLVxxx                         _leftMouseJob = LeftMouseJob.None
' ZLVxxx                         .StockSelectedSteinJob = SelectedSteinJob.None
' ZLVxxx                     End If
' ZLVxxx                 End With
' ZLVxxx                 '
' ZLVxxx                 With _sfd.SFRun.MousePolling
' ZLVxxx                     If _leftMouseJob = LeftMouseJob.None Then
' ZLVxxx                         If _sfd.SFLay.rxStock?.Contains(.MousePos) Then
' ZLVxxx                             If .LeftMouseChanged Then
' ZLVxxx                                 If .LeftMouseDown Then
' ZLVxxx                                     With _sfd.SFRun.EditorStockValues
' ZLVxxx                                         If .GetIdxSelectedStockItemAndSetGhostValuesAndBitmaps(_sfd.SFRun.MousePolling.MousePos) >= 0 Then
' ZLVxxx                                             _leftMouseJob = LeftMouseJob.MoveSelectedStein
' ZLVxxx                                             .StockSelectedSteinJob = SelectedSteinJob.MouseMove
' ZLVxxx 
' ZLVxxx                                         End If
' ZLVxxx                                     End With
' ZLVxxx                                 End If
' ZLVxxx                             End If
' ZLVxxx                         End If
' ZLVxxx                     End If
' ZLVxxx                     '
' ZLVxxx                     If _leftMouseJob = LeftMouseJob.MoveSelectedStein Then
' ZLVxxx                         If .LeftMouseChanged Then
' ZLVxxx                             If .LeftMouseUp Then
' ZLVxxx                                 With _sfd.SFRun.EditorStockValues
' ZLVxxx                                     .StockSelectedSteinJob = SelectedSteinJob.ChoiseInsertOrFlyBack
' ZLVxxx                                     SetEditorStockChoiseInsertOrFlyBackValues()
' ZLVxxx                                     _leftMouseJob = LeftMouseJob.None
' ZLVxxx                                 End With
' ZLVxxx                             End If
' ZLVxxx                         End If
' ZLVxxx                     End If
' ZLVxxx                 End With
' ZLVxxx 
' ZLVxxx                 With _sfd.SFRun.EditorStockValues
' ZLVxxx                     If _leftMouseJob = LeftMouseJob.MoveSelectedStein Then
' ZLVxxx                         If .StockSelectedSteinJob = SelectedSteinJob.FlyBack_Done OrElse
' ZLVxxx                             .StockSelectedSteinJob = SelectedSteinJob.Insert_Done Then
' ZLVxxx 
' ZLVxxx                             .StockSelectedSteinJob = SelectedSteinJob.None
' ZLVxxx                             _leftMouseJob = LeftMouseJob.None
' ZLVxxx                         End If
' ZLVxxx                     End If
' ZLVxxx                 End With
' ZLVxxx             End If
' ZLVxxx             '
' ZLVxxx             With _sfd.SFRun.MousePolling
' ZLVxxx                 If _leftMouseJob = LeftMouseJob.None Then
' ZLVxxx                     If _sfd.SFLay.rxStockScrollbar?.Contains(.MousePos) Then
' ZLVxxx                         If .LeftMouseChanged Then
' ZLVxxx                             If .LeftMouseDown Then
' ZLVxxx                                 _sfd.SFRun.HScrollBarStock.HandleMouseDown(.MousePos.X, _sfd.SFLay.rxStockScrollbar.Y, _sfd.SFLay.rxStockScrollbar.ContentRect)
' ZLVxxx                                 _leftMouseJob = LeftMouseJob.StockScrollbar
' ZLVxxx                             End If
' ZLVxxx                         End If
' ZLVxxx                     End If
' ZLVxxx                 ElseIf _leftMouseJob = LeftMouseJob.StockScrollbar Then
' ZLVxxx                     If .LeftMouseDown Then
' ZLVxxx                         _sfd.SFRun.HScrollBarStock.HandleMouseDown(.MousePos.X, _sfd.SFLay.rxStockScrollbar.Y, _sfd.SFLay.rxStockScrollbar.ContentRect)
' ZLVxxx                     Else
' ZLVxxx                         _leftMouseJob = LeftMouseJob.None
' ZLVxxx                     End If
' ZLVxxx                 End If
' ZLVxxx             End With
' ZLVxxx 
' ZLVxxx         End Sub
' ZLVxxx 
' ZLVxxx         Public Sub SetEditorStockChoiseInsertOrFlyBackValues()
' ZLVxxx             Dim ptStart As New Point(_sfd.SFRun.MousePolling.MousePos.X - _sfd.SFRun.EditorStockDeltaMouseToGhost.X, _sfd.SFRun.MousePolling.MousePos.Y - _sfd.SFRun.EditorStockDeltaMouseToGhost.Y)
' ZLVxxx             _sfd.SFRun.EditorStockSteinFlugValues = New Airplanes_Flugweg(ptStart, ptZiel:=New Point(_sfd.SFRun.EditorStockGhostRect.Left, _sfd.SFRun.EditorStockGhostRect.Top), _sfd.SFLay.steinSize, AirFlugWeg.Zufall, INI.Rendering_SteinFlugGeschwindigkeit)
' ZLVxxx         End Sub
' ZLVxxx 
' ZLVxxx     End Class
' ZLVxxx End Namespace