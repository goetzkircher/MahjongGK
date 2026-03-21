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

Namespace Spielfeld
    ''' <summary>
    ''' Pfad: MahjongGK/Spielfeld/Runtime
    ''' </summary>
    Public Class SFMouseRuntime

        Public Sub New()

        End Sub

        Private ReadOnly _sfd As SFDaten

        Public Sub New(owner As SFDaten)
            _sfd = owner
        End Sub

        Private Enum LeftMouseJob
            None
            StockScrollbar
            MoveSelectedStein
        End Enum
        '################################################

        Private _leftMouseJob As LeftMouseJob


        Public Sub UpdateMouseValues(outputRect As Rectangle, somethingMouseDone As Boolean, spielfeldIsUpdated As Boolean)


            If Not IsNothing(_sfd.SFRun.EditorStockValues) Then
                With _sfd.SFRun.EditorStockValues
                    If .StockSelectedSteinJob = SelectedSteinJob.FlyBack_Done Then
                        .ClearGhostIdx() 'Ghost wird nicht mehr angezeigt.
                        _leftMouseJob = LeftMouseJob.None
                        .StockSelectedSteinJob = SelectedSteinJob.None
                    End If
                End With
                '
                With _sfd.SFRun.MousePolling
                    If _leftMouseJob = LeftMouseJob.None Then
                        If _sfd.SFLay.rxStock?.Contains(.MousePos) Then
                            If .LeftMouseChanged Then
                                If .LeftMousePressed Then
                                    With _sfd.SFRun.EditorStockValues
                                        If .GetIdxSelectedStockItemAndSetGhostValuesAndBitmaps(_sfd.SFRun.MousePolling.MousePos) >= 0 Then
                                            _leftMouseJob = LeftMouseJob.MoveSelectedStein
                                            .StockSelectedSteinJob = SelectedSteinJob.MouseMove

                                        End If
                                    End With
                                End If
                            End If
                        End If
                    End If
                    '
                    If _leftMouseJob = LeftMouseJob.MoveSelectedStein Then
                        If .LeftMouseChanged Then
                            If .LeftMouseReleased Then
                                With _sfd.SFRun.EditorStockValues
                                    .StockSelectedSteinJob = SelectedSteinJob.ChoiseInsertOrFlyBack
                                    SetEditorStockChoiseInsertOrFlyBackValues()
                                    _leftMouseJob = LeftMouseJob.None
                                End With
                            End If
                        End If
                    End If
                End With

                With _sfd.SFRun.EditorStockValues
                    If _leftMouseJob = LeftMouseJob.MoveSelectedStein Then
                        If .StockSelectedSteinJob = SelectedSteinJob.FlyBack_Done OrElse
                            .StockSelectedSteinJob = SelectedSteinJob.Insert_Done Then

                            .StockSelectedSteinJob = SelectedSteinJob.None
                            _leftMouseJob = LeftMouseJob.None
                        End If
                    End If
                End With
            End If
            '
            With _sfd.SFRun.MousePolling
                If _leftMouseJob = LeftMouseJob.None Then
                    If _sfd.SFLay.rxStockScrollbar?.Contains(.MousePos) Then
                        If .LeftMouseChanged Then
                            If .LeftMousePressed Then
                                _sfd.SFRun.HScrollBarStock.HandleMouseDown(.MousePos.X, _sfd.SFLay.rxStockScrollbar.Y, _sfd.SFLay.rxStockScrollbar.ContentRect)
                                _leftMouseJob = LeftMouseJob.StockScrollbar
                            End If
                        End If
                    End If
                ElseIf _leftMouseJob = LeftMouseJob.StockScrollbar Then
                    If .LeftMousePressed Then
                        _sfd.SFRun.HScrollBarStock.HandleMouseDown(.MousePos.X, _sfd.SFLay.rxStockScrollbar.Y, _sfd.SFLay.rxStockScrollbar.ContentRect)
                    Else
                        _leftMouseJob = LeftMouseJob.None
                    End If
                End If
            End With

        End Sub

        Public Sub SetEditorStockChoiseInsertOrFlyBackValues()
            Dim ptStart As New Point(_sfd.SFRun.MousePolling.MousePos.X - _sfd.SFRun.EditorStockDeltaMouseToGhost.X, _sfd.SFRun.MousePolling.MousePos.Y - _sfd.SFRun.EditorStockDeltaMouseToGhost.Y)
            _sfd.SFRun.EditorStockSteinFlugValues = New SteinFlugValues(ptStart, ptZiel:=New Point(_sfd.SFRun.EditorStockGhostRect.Left, _sfd.SFRun.EditorStockGhostRect.Top), _sfd.SFLay.steinSize, FlugWeg.Zufall, INI.Rendering_SteinFlugGeschwindigkeit)
        End Sub

    End Class
End Namespace