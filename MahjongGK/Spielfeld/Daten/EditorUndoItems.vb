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
Imports MahjongGK.Contracts.GlobalEnum

'
#Disable Warning IDE0079
#Disable Warning IDE1006

Public Class EditorUndoItems

    Sub New()

    End Sub

    Public Property Items As List(Of EditorUndoItem) = New List(Of EditorUndoItem)

    ''' <summary>
    ''' Verschieben eines Steines innerhalb des Editors.
    ''' </summary>
    Sub AddNewUndo(SteinSymbol As SteinSymbol, EditTplSrc As Triple, EditTplDst As Triple)
        Dim Undo As New EditorUndoItem(SteinSymbol, EditTplSrc, EditTplDst)
        AddUndo(Undo)
    End Sub
    '
    ''' <summary>
    ''' Entnehmen eines Steines aus dem Editor.
    ''' </summary>
    Sub AddNewUndo(SteinSymbol As SteinSymbol, EditTplSrc As Triple, StockIdxDst As Integer)
        Dim Undo As New EditorUndoItem(SteinSymbol, EditTplSrc, StockIdxDst)
        AddUndo(Undo)
    End Sub
    '
    ''' <summary>
    ''' Hinzufügen eines Steines zum Feld.
    ''' </summary>
    Sub AddNewUndo(SteinSymbol As SteinSymbol, StockIdxSrc As Integer, EditTplDst As Triple)
        Dim Undo As New EditorUndoItem(SteinSymbol, StockIdxSrc, EditTplDst)
        AddUndo(Undo)
    End Sub
    '
    ''' <summary>
    ''' Verschieben eines Steines innerhalb des Vorrat.
    ''' </summary>
    Sub AddNewUndo(SteinSymbol As SteinSymbol, StockIdxSrc As Integer, StockIdxDst As Integer)
        Dim Undo As New EditorUndoItem(SteinSymbol, StockIdxSrc, StockIdxDst)
        AddUndo(Undo)
    End Sub
    '
    ''' <summary>
    ''' Snapshot des Vorraten vor dem Sortieren und vor dem Auffüllen.
    ''' </summary>
    Sub AddNewUndo(job As UnReDoJob, StockSnapshot() As SteinSymbol)
        If job <> UnReDoJob.FillStock AndAlso job <> UnReDoJob.SortStock Then
            Throw New Exception("Nur UnReDoJob.FillStock und UnReDoJob.SortStock erlaubt.")
        End If
        Dim Undo As New EditorUndoItem(job, StockSnapshot)
        AddUndo(Undo)
    End Sub

    Private Sub AddUndo(undo As EditorUndoItem)

        If Items Is Nothing Then
            Items = New List(Of EditorUndoItem)
        End If
        Items.Add(undo)

    End Sub
End Class
