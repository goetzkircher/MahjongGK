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
Imports MahjongGK.Contracts.GlobalEnum
Imports TileFactory

'
#Disable Warning IDE0079
#Disable Warning IDE1006

Namespace Spielfeld

    '----------------------------------------
    ''' <summary>
    ''' Enumeration für Public Function GetSpecialBmps(specialBitmap As SpecialBmps)
    ''' und Private Sub SetSpecialBmps(specialBitmap As SpecialBmps, bmp As Bitmap, rect As Rectangle)
    ''' Die Funktion ist im Renderer an verschiedenen Stellen im Einsatz um einzelne Mahjongsteine
    ''' und Geistersteine zu blitten, die hier festgelegt werden.
    ''' </summary>
    Public Enum SpecialBmps
        AtEditorSrcPos
        AtEditorMousePos
        AtEditorCanDropPos
        AtStockSrcPos
        AtStockMousePos
        AtStockCanDropPos
        UBnd = AtStockCanDropPos
    End Enum

    ''' <summary>
    ''' Pfad: MahjongGK/Spielfeld/Runtime
    ''' Hier sind alle Funktionen drin, die Folge vom Mausaktionen sind. 
    ''' </summary>
    Public Class SFMouseAktion

        Sub New()

        End Sub
        Public Sub New(owner As SFDaten)
            _sfd = owner
            _mouseWheelPoller = New MouseWheelPoller()
            _mouseWheelPoller.Attach(frmMain.UCtlSpielfeldMain)
        End Sub

        Public Function ConsumeSchnelltestHasMouseStateChange() As Boolean
            'Wegen dem Mausrad ist das im _mouseWheelPoller angesiedelt.
            Return _mouseWheelPoller.ConsumeSchnelltestHasMouseStateChange
        End Function

        Public Sub CreateDragDropJobFromEditor(steinInfoIndex As Integer)
            If IsNothing(_sfd) Then
                Throw New Exception("Programmierfehler: DragDrop ohne sdf instanziert.")
            End If
            _dragDropStartFrom = DragDropStartFrom.Editor
            _dragDropJob = DragDropJob.FirstStep
            _steinInfoIndex = steinInfoIndex
            _steinTypIndex = _sfd.SFInf.SteinInfos(steinInfoIndex).SteinTypIndex
            _stockIndex = -1
            _srcPos3D = _sfd.SFInf.SteinInfos(steinInfoIndex).Pos3D
            _isInit = InitDragDropBitmaps(srcRect:=Nothing) 'holt sich srcRect aus den SteinInfos
        End Sub

        Public Sub CreateDragDropJobFromStock(stockIndex As Integer, steinIndex As SteinTyp, srcRect As Rectangle)
            If IsNothing(_sfd) Then
                Throw New Exception("Programmierfehler: DragDrop ohne sdf instanziert.")
            End If
            _steinInfoIndex = -1
            _stockIndex = stockIndex
            _dragDropStartFrom = DragDropStartFrom.Stock
            _dragDropJob = DragDropJob.FirstStep
            _steinTypIndex = steinIndex
            _srcPos3D = Nothing
            _isInit = InitDragDropBitmaps(srcRect)
        End Sub

        Public Sub ClearLastMouseFlight()
            _isInit = False
        End Sub

        Private ReadOnly _sfd As SFDaten
        Private ReadOnly _mouseWheelPoller As MouseWheelPoller

        Private _bmpOrg As Bitmap = Nothing
        Private _bmpGhost As Bitmap = Nothing
        Private _bmpSelected As Bitmap = Nothing
        Private _bmpCanDrop As Bitmap = Nothing
        '
        ''' <summary>
        ''' Der SteinInfoIndex zeigt auf den SteinInfi-Datensatz des Steies im Editor.
        ''' </summary>
        Private _steinInfoIndex As Integer
        '
        ''' <summary>
        ''' Der Stockindex Zeigt auf den Stein im Vorrat.
        ''' Der Index ist nur während des DtagDrop gültig.
        ''' </summary>
        Private _stockIndex As Integer
        '
        ''' <summary>
        ''' Der SteinTypIndex besimmt das Symbol auf der Oberseite des Steines.
        ''' </summary>
        Private _steinTypIndex As SteinTyp
        Private _dragDropStartFrom As DragDropStartFrom
        Private _dragDropJob As DragDropJob
        Private _srcRect As Rectangle
        Private _srcPos3D As Triple
        Private _isInit As Boolean
        Private _hasRenderJob As Boolean

        Private _polling As Boolean
        Private _leftMousePressed As Boolean
        Private _leftMouseReleased As Boolean
        Private _dragDropMoved As Boolean
        '
        ''' <summary>
        ''' Schließt _dragDropMoved ein!
        ''' </summary>
        Private _dragDropAktive As Boolean
        Private _endDragDrop As Boolean
        Private _mousePos As Point
        Private _klickArea As AirKlickArea
        Private _klickAreaGroup As AirKlickAreaGroup
        Private _klickIsDragDropKandidat As Boolean

        Private _mouseAnkerPosition As MouseAnkerPosition
        Private _aktHScrollbarJob As ScrollbarJob

        Private _wheelSteps As Integer

        Private Enum DragDropJob
            FirstStep
            Active
            EndJob
        End Enum
        Private Enum ScrollbarJob
            None
            MouseDown
            MouseMove
            MouseUp
        End Enum

        Private Enum DragDropStartFrom
            Editor
            Stock
        End Enum
        '

        Private _hasSpecialBmps(SpecialBmps.UBnd) As Boolean
        Private _bmpSpecialBmps(SpecialBmps.UBnd) As Bitmap
        Private _rectSpecialBmps(SpecialBmps.UBnd) As Rectangle
        Public Function GetSpecialBmps(specialBitmap As SpecialBmps) As (has As Boolean, bmp As Bitmap, rect As Rectangle)
            Dim values As (has As Boolean, bmp As Bitmap, rect As Rectangle)
            values.has = _hasSpecialBmps(specialBitmap)
            values.bmp = _bmpSpecialBmps(specialBitmap)
            values.rect = _rectSpecialBmps(specialBitmap)
            Return values
        End Function

        Private Sub SetSpecialBmps(specialBitmap As SpecialBmps, bmp As Bitmap, rect As Rectangle)
            _hasSpecialBmps(specialBitmap) = True
            _bmpSpecialBmps(specialBitmap) = bmp
            _rectSpecialBmps(specialBitmap) = rect
            _hasRenderJob = True
        End Sub

        Private Sub ClearSpecialBmps()
            For idx As Integer = 0 To SpecialBmps.UBnd
                _hasSpecialBmps(idx) = False
                _bmpSpecialBmps(idx) = Nothing
                _rectSpecialBmps(idx) = Rectangle.Empty
            Next
        End Sub

        Private Function InitDragDropBitmaps(srcRect As Rectangle) As Boolean
            '
            'Bitmaps Eigentum der TileFactory ==> kein Dispose!

            Dim request As TileRequest = Nothing

            If _steinInfoIndex >= 0 Then
                'Stein aus dem Feld
                request = New TileRequest(_sfd.SFRun.AktRenderMode, INI.Tile_TileColors, _sfd.SFInf.SteinInfos(_steinInfoIndex).SteinTypIndex, SteinStatus.I02Selected, SteinFrameVersion.Standard, _sfd.SFLay.steinSize, INI.Tile_BasisSize)
                _bmpOrg = TileFactory.GetTileDeepCopy(request)
            ElseIf _steinTypIndex >= 0 Then
                'Stein aus dem Vorrat
                'Falls _steinTypIndex ungültig ist. wird die Errorgrafik erzeugt
                request = New TileRequest(_sfd.SFRun.AktRenderMode, INI.Tile_TileColors, _steinTypIndex, SteinStatus.I02Selected, SteinFrameVersion.Standard, _sfd.SFLay.steinSize, INI.Tile_BasisSize)
                _bmpOrg = TileFactory.GetTileDeepCopy(request)
            Else
                Return False
            End If

            request.SetSteinFrameVersion(SteinFrameVersion.Standard, ghost:=True)
            _bmpGhost = TileFactory.GetTileDeepCopy(request)
            '
            request.SetSteinFrameVersion(SteinFrameVersion.MouseSelected, ghost:=False)
            _bmpSelected = TileFactory.GetTileDeepCopy(request)

            request.SetSteinFrameVersion(SteinFrameVersion.MouseCanDrop, ghost:=False)
            _bmpCanDrop = TileFactory.GetTileDeepCopy(request)

            Return True

        End Function

        '
        ''' <summary>
        ''' Das ist zugleich Frage und Auslöser die Antwort zu finden, also der Hauptaufruf, der alle
        ''' Vorbereitungsarbeiten macht, einschließlich der Feststellung, daß die Maus einen Stein geklickt hat und
        ''' daher DragDrop begonnen werden soll. Aufruf aus dem RenderManager.PaintUCtlSpielfeld heraus.
        ''' </summary>
        ''' <returns></returns>
        Public Function ConsumeMouseAktionHasRenderJob() As Boolean

            '
            'Default
            ClearSpecialBmps()

            'alle in der Region benötigten Werte lokal laden.
            _leftMousePressed = _sfd.SFRun.MousePolling.ConsumeLeftMousePressed()
            _leftMouseReleased = _sfd.SFRun.MousePolling.ConsumeLeftMouseReleased()
            _mousePos = _sfd.SFRun.MousePolling.MousePos
            _klickArea = AirGetKlickArea(_mousePos, _sfd.SFLay)
            _klickAreaGroup = AirGetKlickAreaGroup(_klickArea)
            _klickIsDragDropKandidat = AirIsDragDropKandidat(_klickArea)
            _dragDropMoved = _sfd.SFRun.MousePolling.DragDropMoved
            _dragDropAktive = _sfd.SFRun.MousePolling.DragDropActive
            _endDragDrop = _sfd.SFRun.MousePolling.ConsumeEndDragDrop

            _wheelSteps = _mouseWheelPoller.PollWheelStep()

            If _klickAreaGroup = AirKlickAreaGroup.Scrollbar Then

                If _wheelSteps <> 0 Then

                    _sfd.SFRun.HScrollBarStock.HandleMouseWheel(_wheelSteps * -3)
                    _wheelSteps = 0
                    _hasRenderJob = True
                End If

                If _leftMousePressed Then
                    If _aktHScrollbarJob = ScrollbarJob.None Then
                        _sfd.SFRun.HScrollBarStock.HandleMouseDown(_mousePos)
                        _aktHScrollbarJob = ScrollbarJob.MouseMove
                        _hasRenderJob = True
                    End If

                ElseIf _leftMouseReleased Then

                    _sfd.SFRun.HScrollBarStock.HandleMouseUp()
                    _aktHScrollbarJob = ScrollbarJob.None
                    _hasRenderJob = True
                Else
                    If _aktHScrollbarJob = ScrollbarJob.MouseMove Then
                        _sfd.SFRun.HScrollBarStock.HandleMousemMove(_mousePos)
                        _hasRenderJob = True

                    End If
                End If
                If _hasRenderJob Then
                    If _sfd.SFInf.Generator.StockAktCount = 0 Then
                        _sfd.SFRun.HScrollBarStock.SetRange(0)
                    Else
                        _sfd.SFStock.SetHScrollBarValue(_sfd.SFRun.HScrollBarStock.GetValue)
                    End If
                End If

            ElseIf _klickAreaGroup = AirKlickAreaGroup.Historybox Then
                DoKlickHistorboxes()

            ElseIf _klickAreaGroup = AirKlickAreaGroup.Button Then
                DoKlickButtons()
            End If

            'Hier werden die aktuellen Rechtangle des Stock festgelegt, auf die im folgenden
            'und im Renderer aus PaintStock zugegriffen wird.
            'muus genau hier zwischen der Scrollbar und Spielfels, Editor, Stock
            'stehen, da die Scrollbar die Werte ändert und dann der Startoffset
            'bei der Ausgabe falsch ist.
            _sfd.SFStock.SetRectsFromVisibleStock(startNewGapJob:=False)

            If _klickArea = AirKlickArea.Spielfeld OrElse
                 _klickArea = AirKlickArea.Editor OrElse
                  _klickArea = AirKlickArea.Stock Then
                '----------------------------------------------------
                ' DragDrop
                '----------------------------------------------------
                If Not _dragDropAktive AndAlso _leftMousePressed Then

                    _dragDropJob = DragDropJob.FirstStep

                    'DragDrop kann nur einmalig stattfinden.
                    'Auch während eines DragDrop bringt ein
                    'Buttonklick nichts.
                    'Vermutlich tritt das nur ein, wenn sehr schnell
                    'hintereinander geklickt wird.
                    '==> einfach ignorieren.

                    If _klickArea = AirKlickArea.Spielfeld Then
                        DoKlickSpiel()

                    ElseIf _klickArea = AirKlickArea.Editor Then
                        Dim steinInfoIndex As Integer = _sfd.SFInf.GetTopSteinInfoIndexAtPoint(_mousePos)
                        If steinInfoIndex >= 0 Then ' Andernflls Klick auf eine freie Fläche
                            CreateDragDropJobFromEditor(steinInfoIndex) 'setzt _isInit
                            _sfd.SFRun.MousePolling.StartDragDrop()
                            DoRenderstep()
                        End If

                    ElseIf _klickArea = AirKlickArea.Stock Then

                        Dim sv As SFStockState.StockValues = _sfd.SFStock.GetSelectedSteinValues(_mousePos, getDropPosition:=True)
                        If sv.HasValues Then
                            CreateDragDropJobFromStock(sv.StockSteinIdx, sv.SteinIndex, sv.RectStein)
                            _sfd.SFRun.MousePolling.StartDragDrop()
                            DoRenderstep()
                        End If
                        'Dim idxSelStockItem As Integer = _sfd.SFRun.SFStock.GetIdxSelectedStockItem(_mousePos)
                    End If
                End If
            End If

            If _dragDropMoved Then
                _dragDropJob = DragDropJob.Active
                'DragDrop ist nur im Editor und im Vorrat möglich.
                'In DoKlickEditor oder DoKlickStock wurde das
                'DragDrop initialisiert, jetzt muss es gestartet werden,
                'weiterlaufen oder enden.
                DoRenderstep()

            ElseIf _endDragDrop Then
                _dragDropJob = DragDropJob.EndJob

                DoRenderstep()
            End If

            Dim retval As Boolean = _hasRenderJob
            _hasRenderJob = False
            Return retval

        End Function

        Private Sub DoRenderstep()

            If _dragDropJob = DragDropJob.FirstStep Then
                Select Case _dragDropStartFrom
                    Case DragDropStartFrom.Editor
                        DoRenderstepEditor(DragDropJob.FirstStep)
                        'DoRenderstepEditor(DragDropJob.FirstStep)
                        'Dim steinInfoIndex As Integer = _sfd.SFInf.GetTopSteinInfoIndexAtPoint(_mousePos)

                        'If steinInfoIndex < 0 Then
                        '    'Klick auf eine freie Fläche
                        '    Exit Sub
                        'Else
                        '    CreateDragDropJobFromEditor(steinInfoIndex)
                        'End If
                    Case DragDropStartFrom.Stock
                        DoRenderstepStock(DragDropJob.FirstStep)

                End Select

            ElseIf _dragDropJob = DragDropJob.Active Then
                Select Case _dragDropStartFrom
                    Case DragDropStartFrom.Editor
                        DoRenderstepEditor(DragDropJob.Active)

                    Case DragDropStartFrom.Stock
                        DoRenderstepStock(DragDropJob.Active)

                End Select

            ElseIf _dragDropJob = DragDropJob.EndJob Then
                Select Case _dragDropStartFrom
                    Case DragDropStartFrom.Editor
                        DoRenderstepEditor(DragDropJob.EndJob)

                    Case DragDropStartFrom.Stock
                        DoRenderstepStock(DragDropJob.EndJob)

                End Select

            End If

        End Sub

        Private Sub DoRenderstepEditor(job As DragDropJob)
            Select Case job
                Case DragDropJob.FirstStep
                    '()
                    _sfd.SFInf.TmpRemoveStein(_steinInfoIndex)

                    'Die Originalposition des Steins, der wegen TmpRemoveStein nicht mehr vorhanden ist.
                    'Noch ohne Geist an der ursprünglichen stelle, dafür als Selected Bitmap
                    SetSpecialBmps(SpecialBmps.AtEditorSrcPos, _bmpSelected, _sfd.SFInf.SteinInfos(_steinInfoIndex).RectStein)

                    'wird in Case DragDropJob.Active gebraucht
                    _mouseAnkerPosition = New MouseAnkerPosition(_rectSpecialBmps(SpecialBmps.AtEditorSrcPos), _mousePos, _sfd.SFLay.rxStageUsed)

                Case DragDropJob.Active
                    '
                    SetSpecialBmps(SpecialBmps.AtEditorSrcPos, _bmpGhost, _sfd.SFInf.SteinInfos(_steinInfoIndex).RectStein)
                    SetSpecialBmps(SpecialBmps.AtEditorMousePos, _bmpSelected, _mouseAnkerPosition.GetAktRectPlane(_mousePos))

                    Dim tpl As Triple = _sfd.SFInf.IsFundamentKandidat(_mousePos)
                    If tpl.IsValideYes Then
                        'mögliche Ablageposition auf dem Feld
                        SetSpecialBmps(SpecialBmps.AtEditorCanDropPos, _bmpGhost, _sfd.SFInf.GetSteinRenderRect(tpl))
                    Else
                        Dim sv As SFStockState.StockValues = _sfd.SFStock.GetSelectedSteinValues(_mousePos, getDropPosition:=True)
                        If sv.HasValues Then
                            'mögliche Ablageposition im Vorrat
                            SetSpecialBmps(SpecialBmps.AtStockCanDropPos, _bmpGhost, sv.RectStein)
                        End If
                    End If

                Case DragDropJob.EndJob
                    '()
                    Dim tpl As Triple = _sfd.SFInf.IsFundamentKandidat(_mousePos)
                    '()
                    _sfd.SFInf.TmpReturnRemovedStein()
                    '()
                    If tpl.IsValideYes Then
                        If _steinInfoIndex >= 0 Then
                            'Umsetzen eines Steines innerhalb des Editors
                            _sfd.SFInf.RemoveSteinFromSpielfeld(_steinInfoIndex)
                            _sfd.SFInf.AddSteinToSpielfeld(_steinTypIndex, tpl)
                        Else
                            Stop
                        End If
                    Else
                        'Ungültige Position ==> zurückfliegen
                        'Stein zurück in den Vorrat
                        Dim sv As SFStockState.StockValues = _sfd.SFStock.GetSelectedSteinValues(_mousePos, getDropPosition:=True)
                        If sv.HasValues Then
                            _sfd.SFInf.RemoveSteinFromSpielfeld(_steinInfoIndex)
                            _sfd.SFInf.Generator.InsertLeftFromSelectedStein(sv.StockSteinIdx, _steinTypIndex)
                        Else
                            ' Ungültige Position ==> zurückfliegen
                        End If
                    End If

            End Select
        End Sub

        Private Sub DoRenderstepStock(job As DragDropJob)
            Select Case job
                Case DragDropJob.FirstStep

                    Dim sv As SFStockState.StockValues = _sfd.SFStock.GetSelectedSteinValues(_mousePos, getDropPosition:=True)
                    If sv.HasValues Then

                        SetSpecialBmps(SpecialBmps.AtStockSrcPos, _bmpSelected, sv.RectStein)
                    End If

                    _mouseAnkerPosition = New MouseAnkerPosition(_rectSpecialBmps(SpecialBmps.AtStockSrcPos), _mousePos, _sfd.SFLay.rxStageUsed)

                    SetSpecialBmps(SpecialBmps.AtStockSrcPos, _bmpSelected, sv.RectStein)

                Case DragDropJob.Active
                    '

                    _bmpSpecialBmps(SpecialBmps.AtStockSrcPos) = _bmpGhost
                    '  _rectSpecialBmps(SpecialBmps.AtStockSrcPos) = _stockValuesSrcKlickAreaStock.RectStein
                    _hasSpecialBmps(SpecialBmps.AtStockSrcPos) = True

                    _bmpSpecialBmps(SpecialBmps.AtStockMousePos) = _bmpSelected
                    _rectSpecialBmps(SpecialBmps.AtStockMousePos) = _mouseAnkerPosition.GetAktRectPlane(_mousePos)
                    _hasSpecialBmps(SpecialBmps.AtStockMousePos) = True

                    Dim tpl As Triple = _sfd.SFInf.IsFundamentKandidat(_mousePos)
                    If tpl.IsValideYes Then
                        Dim rect As Rectangle = _sfd.SFInf.GetSteinRenderRect(tpl)
                        _bmpSpecialBmps(SpecialBmps.AtStockCanDropPos) = _bmpGhost
                        _rectSpecialBmps(SpecialBmps.AtStockCanDropPos) = rect
                        _hasSpecialBmps(SpecialBmps.AtStockCanDropPos) = True
                    End If
                    _hasRenderJob = True

                Case DragDropJob.EndJob
                    '()
                    Dim tpl As Triple = _sfd.SFInf.IsFundamentKandidat(_mousePos)
                    '()
                    _sfd.SFInf.TmpReturnRemovedStein()
                    '()
                    If tpl.IsValideYes Then
                        If _steinInfoIndex >= 0 Then
                            'Umsetzen eines Steines innerhalb des Editors
                            _sfd.SFInf.RemoveSteinFromSpielfeld(_steinInfoIndex)
                            _sfd.SFInf.AddSteinToSpielfeld(_steinTypIndex, tpl)
                        Else
                            'Stein aus dem Vorrat ablegen

                        End If
                    Else
                        'Ungültige Position ==> zurückfliegen

                    End If

            End Select
        End Sub

        Private Sub DoKlickSpiel()

        End Sub

        Private Sub DoKlickHistorboxes()

        End Sub
        Private Sub DoRenderstepHistorboxes()

        End Sub

        '----------------------------------------------------------------------

        Private Sub DoKlickButtons()

        End Sub
    End Class
End Namespace