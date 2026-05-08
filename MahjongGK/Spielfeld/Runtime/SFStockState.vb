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
Imports MahjongGK.Contracts.GlobalEnum

'
#Disable Warning IDE0079
#Disable Warning IDE1006

Namespace Spielfeld
    Public Enum SelectedSteinJob
        None
        MouseMove
        ChoiseInsertOrFlyBack
        Insert_Do
        Insert_Done
        FlyBack_Do
        FlyBack_Done
    End Enum
    ''' <summary>
    ''' Pfad: MahjongGK/Spielfeld/Runtime
    ''' </summary>
    Public Class SFStockState

        ''' <summary>
        ''' Setzt auch alle Startwerte.
        ''' </summary>
        Sub New(owner As SFDaten)

            _sfd = owner

            If IsNothing(_sfd.SFLay.rxStock) Then
                Return
            End If

            SteineCount = _sfd.SFInf.Generator.Stock.Count
            SteineUBound = SteineCount - 1

            If SteineCount = 0 Then
                'Die Scrollbar zurücksetzen
                _sfd.SFRun.HScrollBarStock.SetRange(0)
                Return
            End If

            UpdateSteineVisibleMaxCount()

            If SteineCount >= SteineVisibleMaxCount Then
                SteineVisibleAktCount = SteineVisibleMaxCount
            Else
                SteineVisibleAktCount = SteineCount
            End If

            SteinVisibleAktFistIdx = 0
            SteinVisibleAktLastIdx = SteineVisibleAktCount - 1

            With _sfd.SFRun.HScrollBarStock
                .SetRange(SteineUBound) 'Minimum = 0 : Maximum = steineUBound
                .SetSmallChange(1)
                .SetPageSize(SteineVisibleMaxCount - 1)
            End With

        End Sub

        Private _sfd As SFDaten
        Public ReadOnly Property IsEmpty As Boolean
            Get
                Return SteineCount = 0
            End Get
        End Property

        Private Property SteineCount As Integer
        Private Property SteineUBound As Integer
        Private Property SteineVisibleMaxCount As Integer
        Private Property SteineVisibleAktCount As Integer
        Private Property HScrollBarLastValue As Integer
        Public Property OffsetLeft As Integer
        Public Property SteinVisibleAktFistIdx As Integer
        Public Property SteinVisibleAktLastIdx As Integer

        Public Property LastSteinWidth As Integer
        Public Property LastRxStockWidth As Integer

        Private _ghostIdx As Integer = -1
        Private _selectedStockSteinIdx As Integer = -1
        Private _moveSelectedSteinIndex As SteinTyp
        Private _moveBmpGhost As Bitmap = Nothing
        Private _moveBmpSelected As Bitmap = Nothing
        Private _moveBmpCanDrop As Bitmap = Nothing
        Private _selectedMouseAnkerPos As Point
        Private _hasSelectedMouseAnkerPos As Boolean

        '
        ''' <summary>
        ''' Keine automatische Rückstellung.
        ''' </summary>
        ''' <returns></returns>
        Public Property StockSelectedSteinJob As SelectedSteinJob

        '
        ''' <summary>
        ''' -1, wenn kein Index gegeben.
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property GhostIdx As Integer
            Get
                Return _ghostIdx
            End Get
        End Property
        '
        ''' <summary>
        ''' -1, wenn kein Index gegeben.
        ''' Der Index ist identisch mit dem GhostIdx mit einem Unterschied:
        ''' der GhostIdx wird über ClearGhostIdx gelöscht,
        ''' SelectedSteinIdx über ClearSelectedSteinIdx.
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property SelectedStockSteinIdx As Integer
            Get
                Return _selectedStockSteinIdx
            End Get
        End Property

        Public Sub UpdateSteineVisibleMaxCount()
            'Aufrunden, der letzte Stein kann angeschnitten sein.
            SteineVisibleMaxCount = _sfd.SFLay.rxStock.Width \ _sfd.SFLay.steinWidth + 1

            With _sfd.SFRun.HScrollBarStock
                .SetPageSize(SteineVisibleMaxCount - 1)
            End With
        End Sub '

        Public Sub ClearGhostIdx()
            _ghostIdx = -1
        End Sub
        Public Sub ClearSelectedStockSteinIdx()
            _selectedStockSteinIdx = -1
        End Sub
        '
        ''' <summary>
        ''' Nothing, wenn kein Ghost gegeben
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property MoveBmpGhost As Bitmap
            Get
                Return _moveBmpGhost
            End Get
        End Property
        Public ReadOnly Property MoveBmpSelected As Bitmap
            Get
                Return _moveBmpSelected
            End Get
        End Property
        Public ReadOnly Property MoveBmpPlaceable As Bitmap
            Get
                Return _moveBmpCanDrop
            End Get
        End Property

        Public ReadOnly Property MoveSelectedSteinIndex As SteinTyp
            Get
                Return _moveSelectedSteinIndex
            End Get
        End Property

        ''' <summary>
        ''' Gibt den aktuellen Index oder -1 für "nichts vorhanden" zurück.
        ''' Setzt gleichzeitig _SelectedMouseAnkerPos, _ghostIdx, _ghostBmp
        ''' </summary>
        ''' <param name="mousePos"></param>
        ''' <returns></returns>
        Public Function GetIdxSelectedStockItemAndSetGhostValuesAndBitmaps(mousePos As Point) As Integer

            _hasSelectedMouseAnkerPos = False
            _moveBmpGhost?.Dispose() : _moveBmpGhost = Nothing
            _moveBmpCanDrop?.Dispose() : _moveBmpCanDrop = Nothing
            _moveBmpSelected?.Dispose() : _moveBmpSelected = Nothing
            _ghostIdx = -1

            If IsEmpty Then
                Return -1
            End If
            If SteinVisibleAktFistIdx = -1 Then
                Return -1
            End If

            If Not _sfd.SFLay.rxStock.Contains(mousePos) Then
                Return -1
            End If

            Dim aktXMin As Integer = OffsetLeft
            Dim aktXMax As Integer = OffsetLeft + _sfd.SFLay.steinWidth

            For aktIdx As Integer = SteinVisibleAktFistIdx To SteinVisibleAktLastIdx
                If mousePos.X >= aktXMin And mousePos.X <= aktXMax Then
                    _selectedMouseAnkerPos = New Point(mousePos.X - aktXMin, mousePos.Y - _sfd.SFLay.rxStock.Y)
                    _hasSelectedMouseAnkerPos = True
                    _ghostIdx = aktIdx
                    _selectedStockSteinIdx = aktIdx
                    _moveSelectedSteinIndex = _sfd.SFInf.Generator.Stock(_ghostIdx)

                    Dim request As New TileFactory.TileRequest(_sfd.SFRun.AktRenderMode, INI.Tile_TileColors, steinTyp:=_moveSelectedSteinIndex, SteinStatus.I01Normal, SteinFrameVersion.Standard, _sfd.SFLay.steinSize, INI.Tile_BasisSize, ghost:=True)
                    _moveBmpGhost = TileFactory.GetTile(request)
                    request.SetSteinFrameVersion(SteinFrameVersion.MouseCanDrop, ghost:=False)
                    _moveBmpCanDrop = TileFactory.GetTile(request)
                    request.SetSteinFrameVersion(SteinFrameVersion.MouseSelected, ghost:=False)
                    _moveBmpSelected = TileFactory.GetTile(request)

                    Return aktIdx
                Else
                    aktXMin += _sfd.SFLay.steinWidth
                    aktXMax += _sfd.SFLay.steinWidth
                End If
            Next

            Return -1

        End Function

        Public Property ShowStockGhost As Boolean

        Public Sub ClearAndSetStartvalues()

        End Sub

        '
        ''' <summary>
        ''' Das ist die Position relativ zur oberen linken Ecke des Steines,
        ''' an den der Stein mit der Maus gezogen wird.
        ''' Wird von GetIdxSelectedStockItem gesetzt.
        ''' Achtung: Point.Empty kann nicht verwendet werden, da (0,0) ein gültiger Wert ist. 
        ''' Deshalb immer HasSelectedMouseAnkerPos abfragen.
        ''' Gültig bis zum nächsten GetIdxSelectedStockItem.
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property SelectedMouseAnkerPos As Point
            Get
                Return _selectedMouseAnkerPos
            End Get
        End Property

        Public ReadOnly Property HasSelectedMouseAnkerPos As Boolean
            Get
                Return _hasSelectedMouseAnkerPos
            End Get
        End Property

        ''' <summary>
        ''' Die Scrollbar wird im Rendertakt aufgerufen, (also je Frame genau einmal)
        ''' und der aktuelle Wert wird hierher übermittelt.
        ''' Im Anschluß werden die aktuellen Steine gerendert.
        ''' </summary>
        ''' <param name="value"></param>
        Public Sub SetHScrollBarValue(value As Integer)
            If HScrollBarLastValue = value Then
                Return
            End If

            Dim delta As Integer = value - HScrollBarLastValue
            HScrollBarLastValue = value

            Select Case delta
                Case < 0
                    MoveLeft(-delta)
                Case > 0
                    MoveRight(delta)
            End Select
        End Sub

        Public Sub MoveRight(count As Integer)

            Dim newFirstIdx As Integer = SteinVisibleAktFistIdx + count
            Dim maxFirstIdx As Integer = SteineCount - SteineVisibleAktCount

            If newFirstIdx <= maxFirstIdx Then
                SteinVisibleAktFistIdx = newFirstIdx
                SteinVisibleAktLastIdx = SteinVisibleAktFistIdx + SteineVisibleAktCount - 1
            Else
                SteinVisibleAktFistIdx = maxFirstIdx
                SteinVisibleAktLastIdx = SteineUBound
            End If
            AdjustRightEdgeForLastStone()

        End Sub

        Public Sub MoveLeft(count As Integer)

            Dim newFirstIdx As Integer = SteinVisibleAktFistIdx - count

            If newFirstIdx >= 0 Then
                SteinVisibleAktFistIdx = newFirstIdx
                SteinVisibleAktLastIdx = SteinVisibleAktFistIdx + SteineVisibleAktCount - 1
            Else
                SteinVisibleAktFistIdx = 0
                SteinVisibleAktLastIdx = SteineVisibleAktCount - 1
            End If
            AdjustRightEdgeForLastStone()

        End Sub

        Private Sub AdjustRightEdgeForLastStone() 'Name kannst du ändern

            If SteineCount <= 0 Then
                OffsetLeft = 0
                Return
            End If

            If SteinVisibleAktLastIdx <> SteineUBound Then
                OffsetLeft = 0
                Return
            End If

            OffsetLeft = GetOffsetLeft()

        End Sub

        Private Function GetOffsetLeft() As Integer

            Dim value1 As Integer = (SteinVisibleAktLastIdx - SteinVisibleAktFistIdx + 1) * _sfd.SFLay.steinWidth
            Dim value2 As Integer = value1 - _sfd.SFLay.rxStock.Width

            If value2 > 0 Then
                Return -value2
            Else
                Return 0
            End If

        End Function

    End Class
End Namespace