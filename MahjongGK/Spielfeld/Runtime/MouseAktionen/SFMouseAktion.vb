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
        AtStockInsertPos
        AtStockRemovePos
        UBnd = AtStockRemovePos
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
        '
        ''' <summary>
        ''' Reine Weiterleitung
        ''' </summary>
        ''' <returns></returns>
        Public Function ConsumeQuicktestHasMouseStateChange(rxStageUsed As RectangleX, rxStock As RectangleX, rxStockScrollbar As Rectangle) As MouseStateChanged
            'Wegen dem Mausrad ist das im _mouseWheelPoller angesiedelt.
            Return _mouseWheelPoller.ConsumeSchnelltestHasMouseStateChange(rxStageUsed, rxStock, rxStockScrollbar)
        End Function

        Public Sub CreateDragDropJobFromEditor(steinInfoIndex As Integer)
            If IsNothing(_sfd) Then
                Throw New Exception("Programmierfehler: DragDrop ohne sdf instanziert.")
            End If
            _dragDropStartFrom = DragDropStartFrom.Editor
            _dragDropJob = DragDropJob.FirstStep
            _steinInfoIndex = steinInfoIndex
            _steinTypEnum = _sfd.SFInf.SteinInfos(steinInfoIndex).SteinTypIndex
            _stockSteinIndex = -1
            _srcPos3D = _sfd.SFInf.SteinInfos(steinInfoIndex).Pos3D
            _isInit = InitDragDropBitmaps()
        End Sub

        Public Sub CreateDragDropJobFromStock(stockSteinIndex As Integer, steinTypEnum As SteinTyp, srcRect As Rectangle)
            If IsNothing(_sfd) Then
                Throw New Exception("Programmierfehler: DragDrop ohne sdf instanziert.")
            End If
            _steinInfoIndex = -1
            _stockSteinIndex = stockSteinIndex
            _dragDropStartFrom = DragDropStartFrom.Stock
            _dragDropJob = DragDropJob.FirstStep
            _steinTypEnum = steinTypEnum
            _srcPos3D = Nothing
            _isInit = InitDragDropBitmaps()
        End Sub

        Public Sub ClearLastMouseFlight()
            _isInit = False
        End Sub

        Private ReadOnly _sfd As SFDaten
        Private ReadOnly _mouseWheelPoller As MouseWheelPoller

        Private _bmpNormal As Bitmap = Nothing
        Private _bmpGhost As Bitmap = Nothing
        Private _bmpSelected As Bitmap = Nothing
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
        Private _stockSteinIndex As Integer
        '
        ''' <summary>
        ''' Der SteinTypIndex besimmt das Symbol auf der Oberseite des Steines.
        ''' </summary>
        Private _steinTypEnum As SteinTyp
        Private _dragDropStartFrom As DragDropStartFrom
        Private _dragDropJob As DragDropJob
        Private _srcRect As Rectangle
        Private _srcPos3D As Triple
        Private _isInit As Boolean
        Private _IsDirty As Boolean

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
        Private _leftMouseDoublKlick As Boolean

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
        ''' <summary>
        ''' Die Z-Ebene wird nicht benötigt für Bitmaps, die auf dem Stock angezeigt werden.
        ''' </summary>
        Private Const DUMMY As Integer = 999
        'eine Zahl größer als die maximal denkbare Länge des Steinvorrates
        Private Const ATEND As Integer = 99999

        Private _hasSpecialBmps(SpecialBmps.UBnd) As Boolean
        Private _bmpSpecialBmps(SpecialBmps.UBnd) As Bitmap
        Private _rectSpecialBmps(SpecialBmps.UBnd) As Rectangle
        Private _zEbeneSpecialBmp(SpecialBmps.UBnd) As Integer
        Public Function GetSpecialBmps(specialBitmap As SpecialBmps) As (has As Boolean, bmp As Bitmap, rect As Rectangle, zEbene As Integer)
            Dim values As (has As Boolean, bmp As Bitmap, rect As Rectangle, zEbene As Integer)
            values.has = _hasSpecialBmps(specialBitmap)
            values.bmp = _bmpSpecialBmps(specialBitmap)
            values.rect = _rectSpecialBmps(specialBitmap)
            values.zEbene = _zEbeneSpecialBmp(specialBitmap)
            Return values
        End Function

        Private Sub SetSpecialBmps(specialBitmap As SpecialBmps, bmp As Bitmap, rect As Rectangle, zEbene As Integer)
            _hasSpecialBmps(specialBitmap) = True
            _bmpSpecialBmps(specialBitmap) = bmp
            _rectSpecialBmps(specialBitmap) = rect
            _zEbeneSpecialBmp(specialBitmap) = zEbene
            MakeDirty()
        End Sub

        Private Sub ClearSpecialBmps()
            For idx As Integer = 0 To SpecialBmps.UBnd
                _hasSpecialBmps(idx) = False
                'Kein Dispose. Besitzer aller Bitmaps ist die TileFactory
                _bmpSpecialBmps(idx) = Nothing
                _rectSpecialBmps(idx) = Rectangle.Empty
                _zEbeneSpecialBmp(idx) = -1
            Next
        End Sub
        Private Sub MakeDirty()
            _IsDirty = True
        End Sub

        Private Function InitDragDropBitmaps() As Boolean
            '
            'Bitmaps Eigentum der TileFactory ==> kein Dispose!

            Dim request As TileRequest = Nothing

            If _steinInfoIndex >= 0 Then
                'Stein aus dem Feld
                request = New TileRequest(_sfd.SFRun.AktRenderMode, INI.Tile_TileColors, _sfd.SFInf.SteinInfos(_steinInfoIndex).SteinTypIndex, SteinStatus.I01Normal, SteinFrameVersion.Standard, _sfd.SFLay.steinSize, INI.Tile_BasisSize)
                _bmpNormal = TileFactory.GetTile(request)
            ElseIf _steinTypEnum >= 0 Then
                'Stein aus dem Vorrat
                'Falls _steinTypEnum ungültig ist. wird die Errorgrafik erzeugt
                request = New TileRequest(_sfd.SFRun.AktRenderMode, INI.Tile_TileColors, _steinTypEnum, SteinStatus.I01Normal, SteinFrameVersion.Standard, _sfd.SFLay.steinSize, INI.Tile_BasisSize)
                _bmpNormal = TileFactory.GetTile(request)
            Else
                Return False
            End If

            request.SetSteinFrameVersion(SteinStatus.I01Normal, SteinFrameVersion.Standard, ghost:=True)
            _bmpGhost = TileFactory.GetTileDeepCopy(request)
            '
            request.SetSteinFrameVersion(SteinStatus.I02Selected, SteinFrameVersion.MouseSelected, ghost:=False)
            _bmpSelected = TileFactory.GetTile(request)

            Return True

        End Function

        Private _lastGapJobIsWorking As Boolean
        Public Function GapJobIsWorking() As Boolean
            Dim retval As Boolean = _sfd.SFStock.GapJobIsWorking
            If retval = False AndAlso _lastGapJobIsWorking = True Then
                _lastGapJobIsWorking = False
                Return True
            Else
                _lastGapJobIsWorking = retval
                Return retval
            End If
        End Function

        Public Function ConsumeMousAktionIsDirty() As Boolean

            Dim isDirty As Boolean = _IsDirty
            _IsDirty = False
            Return isDirty

        End Function

        '
        ''' <summary>
        ''' Das ist der Hauptaufruf, wenn irgenwas geklickt oder gezogen wird, also DragDrop.
        ''' Aufruf aus dem RenderManager.PaintUCtlSpielfeld heraus.
        ''' </summary>
        Public Sub DoMouseDownAndDragDropAktion()
            '
            ClearSpecialBmps()

            If _sfd.SFStock.GapJobIsWorking Then
                With _sfd.SFStock
                    .ContinueGapJob()
                    If .HasBmpGapInsert Then
                        SetSpecialBmps(SpecialBmps.AtStockInsertPos, .BmpGapInsert, .RectGapInsert, DUMMY)
                    End If
                    If .HasBmpGapRemove Then
                        SetSpecialBmps(SpecialBmps.AtStockRemovePos, .BmpGapRemove, .RectGapRemove, DUMMY)
                    End If
                End With
                MakeDirty()
                Exit Sub
            End If

            'alle in der Region benötigten Werte lokal laden.
            _leftMousePressed = _sfd.SFRun.MousePolling.ConsumeLeftMousePressed()
            _leftMouseReleased = _sfd.SFRun.MousePolling.ConsumeLeftMouseReleased()
            _leftMouseDoublKlick = _sfd.SFRun.MousePolling.ConsumeLeftMouseDoubleClick()
            _mousePos = _sfd.SFRun.MousePolling.MousePos
            _klickArea = AirGetKlickArea(_mousePos, _sfd.SFLay)
            _klickAreaGroup = AirGetKlickAreaGroup(_klickArea)
            _klickIsDragDropKandidat = AirIsDragDropKandidat(_klickArea)
            _dragDropMoved = _sfd.SFRun.MousePolling.DragDropMoved
            _dragDropAktive = _sfd.SFRun.MousePolling.DragDropActive
            _endDragDrop = _sfd.SFRun.MousePolling.ConsumeEndDragDrop
            _wheelSteps = _mouseWheelPoller.PollWheelStep()

            If _sfd.SFRun.MousePolling.ConsumeRightMousePressed() Then
                frmMain.UCtlSpielfeldMain.ContextMenuStrip = New ContextMenueEditor
                Exit Sub
            End If

            If _leftMouseDoublKlick Then
                If _klickArea = AirKlickArea.Editor Then
                    DoLeftMouseDoublKlick()
                    Exit Sub
                Else
                    'nur im Editor zulässig.
                    _leftMouseDoublKlick = False
                End If
            End If

            If _klickAreaGroup = AirKlickAreaGroup.Scrollbar Then

                If _wheelSteps <> 0 Then

                    _sfd.SFRun.HScrollBarStock.HandleMouseWheel(_wheelSteps * -3)
                    _wheelSteps = 0
                    MakeDirty()
                End If

                If _leftMousePressed Then
                    If _aktHScrollbarJob = ScrollbarJob.None Then
                        _sfd.SFRun.HScrollBarStock.HandleMouseDown(_mousePos)
                        _aktHScrollbarJob = ScrollbarJob.MouseMove
                        MakeDirty()
                    End If

                ElseIf _leftMouseReleased Then

                    _sfd.SFRun.HScrollBarStock.HandleMouseUp()
                    _aktHScrollbarJob = ScrollbarJob.None
                    MakeDirty()
                Else
                    If _aktHScrollbarJob = ScrollbarJob.MouseMove Then
                        _sfd.SFRun.HScrollBarStock.HandleMousemMove(_mousePos)
                        MakeDirty()

                    End If
                End If
                If _IsDirty Then
                    If _sfd.SFStock.StockAktCount = 0 Then
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
            'und im Renderer aus Paint_Stock zugegriffen wird.
            'muus genau hier zwischen der Scrollbar und Spielfels, Editor, Stock
            'stehen, da die Scrollbar die Werte ändert und dann der Startoffset
            'bei der Ausgabe falsch ist.
            _sfd.SFStock.SetRectsFromVisibleStock()

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
                            If _sfd.SFRun.MousePolling.StartDragDrop() Then
                                CreateDragDropJobFromEditor(steinInfoIndex) 'setzt _isInit
                                DoRenderstep()
                            End If
                        End If

                    ElseIf _klickArea = AirKlickArea.Stock Then

                        Dim sv As SFStockJobs.StockValues = _sfd.SFStock.GetSelectedSteinValues(_mousePos, getDropPosition:=False)
                        If sv.HasValueInsideStock Then
                            If _sfd.SFRun.MousePolling.StartDragDrop() Then
                                CreateDragDropJobFromStock(sv.StockSteinIdx, sv.SteinIndex, sv.RectStein)
                                DoRenderstep()
                            End If
                        End If
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

        End Sub

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

        ''' <summary>
        ''' In beiden Subs, DoRenderstepStock und DoRenderstepEditor, werden Steine von einem zum
        ''' anderen transportiert. Ausschlaggebend, wo die Aktion verarbeitet wird, ist der Startpunkt, das Drag
        ''' </summary>
        Private Sub DoRenderstepEditor(job As DragDropJob)
            Select Case job
                Case DragDropJob.FirstStep
                    '()
                    _sfd.SFInf.TmpRemoveStein(_steinInfoIndex)

                    'Die Originalposition des Steins, der wegen TmpRemoveStein nicht mehr vorhanden ist.
                    'Noch ohne Geist an der ursprünglichen stelle, dafür als Selected Bitmap
                    SetSpecialBmps(SpecialBmps.AtEditorSrcPos, _bmpSelected, _sfd.SFInf.SteinInfos(_steinInfoIndex).RectStein, _sfd.SFInf.SteinInfos(_steinInfoIndex).Z)

                    'wird in Case DragDropJob.Active gebraucht
                    _mouseAnkerPosition = New MouseAnkerPosition(_rectSpecialBmps(SpecialBmps.AtEditorSrcPos), _mousePos, _sfd.SFLay.rxStageUsed)

                Case DragDropJob.Active
                    '
                    ' Debug.Print($"Editor DragDropJob.Active {_mousePos}")
                    SetSpecialBmps(SpecialBmps.AtEditorSrcPos, _bmpGhost, _sfd.SFInf.SteinInfos(_steinInfoIndex).RectStein, _sfd.SFInf.SteinInfos(_steinInfoIndex).Z)
                    SetSpecialBmps(SpecialBmps.AtEditorMousePos, _bmpSelected, _mouseAnkerPosition.GetAktRectPlane(_mousePos), DUMMY)

                    Dim tpl As Triple = _sfd.SFInf.IsFundamentKandidat(_mousePos)
                    If tpl.IsValideYes Then
                        'mögliche Ablageposition auf dem Feld
                        SetSpecialBmps(SpecialBmps.AtEditorCanDropPos, _bmpGhost, _sfd.SFInf.GetSteinRenderRect(tpl), tpl.z)
                    Else
                        Dim sv As SFStockJobs.StockValues = _sfd.SFStock.GetSelectedSteinValues(_mousePos, getDropPosition:=True, leaveCenterBlank:=True)
                        'If sv.StockSteinIdx = 0 Then
                        '    Stop
                        'End If
                        If sv.HasValue Then
                            'mögliche Ablageposition im Vorrat
                            If Not (sv.StockSteinIdx = _stockSteinIndex OrElse sv.StockSteinIdx = _stockSteinIndex + 1) OrElse _sfd.SFStock.SteinVisibleAktFistIdx = 0 Then
                                'links und rechts von der Ausgangsposition ist eine Ablage sinnlos,
                                'da sie zur Ausgangsposition führt, daher die Einschränkung.
                                SetSpecialBmps(SpecialBmps.AtStockCanDropPos, _bmpGhost, sv.RectStein, DUMMY)
                            End If
                        ElseIf _klickArea = AirKlickArea.Stock Then
                            If _sfd.SFStock.StockAktCount = 0 Then
                                With _sfd.SFLay
                                    Dim rect As New Rectangle(.rxStock.Left, .rxStock.Top, .steinWidth, .steinHeight)
                                    SetSpecialBmps(SpecialBmps.AtStockCanDropPos, _bmpGhost, rect, DUMMY)
                                End With
                            End If
                        End If
                    End If

                Case DragDropJob.EndJob
                    '()
                    Dim tpl As Triple = _sfd.SFInf.IsFundamentKandidat(_mousePos)
                    '()
                    _sfd.SFInf.TmpReturnRemovedStein()
                    '()
                    If tpl.IsValideYes Then
                        'Umsetzen eines Steines innerhalb des Editors
                        _sfd.SFStock.StartNewFadeOutJob(_steinInfoIndex, startAsGhost:=True)
                        _sfd.SFInf.RemoveSteinFromSpielfeld(_steinInfoIndex)
                        _sfd.SFInf.AddSteinToSpielfeld(_steinTypEnum, tpl)
                        MakeDirty()
                    Else
                        'Stein zurück in den Vorrat
                        Dim sv As SFStockJobs.StockValues = _sfd.SFStock.GetSelectedSteinValues(_mousePos, getDropPosition:=True, leaveCenterBlank:=False)

                        If sv.HasValueInsideStock Then
                            _sfd.SFStock.StartNewFadeOutJob(_steinInfoIndex, startAsGhost:=True)
                            _sfd.SFInf.RemoveSteinFromSpielfeld(_steinInfoIndex)
                            _sfd.SFStock.StartNewGapJob(idxGapInsert:=sv.StockSteinIdx, bmpGapInsert:=_bmpNormal, _steinTypEnum)
                        ElseIf sv.HasValueAfterEndOfStock Then
                            _sfd.SFStock.StartNewFadeOutJob(_steinInfoIndex, startAsGhost:=True)
                            Dim st As SteinTyp = _sfd.SFInf.SteinInfos(_steinInfoIndex).SteinTypIndex
                            _sfd.SFInf.RemoveSteinFromSpielfeld(_steinInfoIndex)
                            _sfd.SFStock.AddAtStockEnd(st)
                            _sfd.SFStock.NotifyStockInsert(ATEND)
                            MakeDirty()
                        Else
                            Dim rect As New Rectangle(_sfd.SFLay.rxStock.Left, _sfd.SFLay.rxStock.Top, _sfd.SFLay.steinWidth, _sfd.SFLay.steinHeight)

                            If _sfd.SFStock.StockAktCount = 0 AndAlso rect.Contains(_mousePos) Then
                                _sfd.SFStock.StartNewFadeOutJob(_steinInfoIndex, startAsGhost:=True)
                                Dim st As SteinTyp = _sfd.SFInf.SteinInfos(_steinInfoIndex).SteinTypIndex
                                _sfd.SFInf.RemoveSteinFromSpielfeld(_steinInfoIndex)
                                _sfd.SFStock.AddAtStockEnd(st)
                                _sfd.SFStock.NotifyStockInsert(ATEND)
                                SetSpecialBmps(SpecialBmps.AtStockCanDropPos, _bmpNormal, rect, DUMMY)
                                MakeDirty()
                            Else
                                'With _sfd.SFLay
                                '    Dim rect As New Rectangle(.rxStock.Left, .rxStock.Top, .steinWidth, .steinHeight)
                                '    SetSpecialBmps(SpecialBmps.AtStockCanDropPos, _bmpGhost, rect)
                                'End With
                                'ElseIf _klickArea = AirKlickArea.Stock AndAlso _sfd.SFStock.StockAktCount = 1 Then
                                '    With _sfd.SFLay
                                '        Dim rect As New Rectangle(.rxStock.Left + .steinWidth, .rxStock.Top, .steinWidth, .steinHeight)
                                '        SetSpecialBmps(SpecialBmps.AtStockCanDropPos, _bmpGhost, rect)
                                '    End With
                            End If
                            ' Ungültige Position ==> zurückfliegen
                        End If
                    End If

            End Select
        End Sub
        '
        ''' <summary>
        ''' In beiden Subs, DoRenderstepStock und DoRenderstepEditor, werden Steine von einem zum
        ''' anderen transportiert. Ausschlaggebend, wo die Aktion verarbeitet wird, ist der Startpunkt, das Drag.
        ''' </summary>
        ''' <param name="job"></param>
        Private Sub DoRenderstepStock(job As DragDropJob)
            Select Case job
                Case DragDropJob.FirstStep

                    Dim rect As Rectangle = _sfd.SFStock.GetRectFromStockSteinIdx(_stockSteinIndex, getDropPosition:=False)
                    SetSpecialBmps(SpecialBmps.AtStockSrcPos, _bmpGhost, rect, DUMMY)

                    'Dim sv As SFStockJobs.StockValues = _sfd.SFStock.GetSelectedSteinValues(_mousePos, getDropPosition:=True)
                    'If sv.HasValues Then
                    '    SetSpecialBmps(SpecialBmps.AtStockCanDropPos, _bmpGhost, sv.RectStein)
                    'End If

                    _mouseAnkerPosition = New MouseAnkerPosition(_rectSpecialBmps(SpecialBmps.AtStockSrcPos), _mousePos, _sfd.SFLay.rxStageUsed)

                Case DragDropJob.Active

                    Dim rect As Rectangle = _sfd.SFStock.GetRectFromStockSteinIdx(_stockSteinIndex, getDropPosition:=False)
                    SetSpecialBmps(SpecialBmps.AtStockSrcPos, _bmpGhost, rect, 99)
                    If _klickArea = AirKlickArea.Stock OrElse _klickArea = AirKlickArea.StockScrollbar Then
                        SetSpecialBmps(SpecialBmps.AtStockMousePos, _bmpSelected, _mouseAnkerPosition.GetAktRectPlane(_mousePos), DUMMY)
                    Else
                        SetSpecialBmps(SpecialBmps.AtEditorMousePos, _bmpSelected, _mouseAnkerPosition.GetAktRectPlane(_mousePos), DUMMY)
                    End If

                    Dim tpl As Triple = _sfd.SFInf.IsFundamentKandidat(_mousePos)
                    If tpl.IsValideYes Then
                        'mögliche Ablageposition auf dem Feld
                        SetSpecialBmps(SpecialBmps.AtEditorCanDropPos, _bmpGhost, _sfd.SFInf.GetSteinRenderRect(tpl), tpl.z)
                    Else
                        Dim sv As SFStockJobs.StockValues = _sfd.SFStock.GetSelectedSteinValues(_mousePos, getDropPosition:=True, leaveCenterBlank:=True)
                        If sv.HasValue Then
                            'mögliche Ablageposition im Vorrat
                            If Not (sv.StockSteinIdx = _stockSteinIndex OrElse sv.StockSteinIdx = _stockSteinIndex + 1) Then
                                'links und rechts von der Ausgangsposition ist eine Ablage sinnlos,
                                'da sie zur Ausgangsposition führt, daher die Einschränkung.
                                SetSpecialBmps(SpecialBmps.AtStockCanDropPos, _bmpGhost, sv.RectStein, DUMMY)
                            End If
                        End If
                    End If

                Case DragDropJob.EndJob
                    '()
                    Dim tpl As Triple = _sfd.SFInf.IsFundamentKandidat(_mousePos)
                    '()
                    If tpl.IsValideYes Then
                        'Ablegen eines Steines aus dem Vorrat
                        _sfd.SFInf.AddSteinToSpielfeld(_steinTypEnum, tpl)
                        '

                        'Entfernen aus dem Vorrat
                        If _sfd.SFStock.StockAktUBnd > _stockSteinIndex Then
                            'Entfernen mit Animation
                            _sfd.SFStock.StartNewGapJob(idxGapRemove:=_stockSteinIndex, bmpGapRemove:=_bmpGhost, steinTypRemove:=_steinTypEnum, gapRemoveUseFirstRect:=True)
                        Else
                            'den allerletzten Stein rechts ohne Animation
                            _sfd.SFStock.GetSteinTypIndexAndRemove(_stockSteinIndex)
                            _sfd.SFStock.NotifyStockRemove(_stockSteinIndex)
                        End If
                    Else
                        'Stein im Vorrat umlagern
                        Dim sv As SFStockJobs.StockValues = _sfd.SFStock.GetSelectedSteinValues(_mousePos, getDropPosition:=True, leaveCenterBlank:=True)

                        If sv.HasValue Then ' das ist einschließlich sv.HasValueAfterEndOfStock 
                            If sv.StockSteinIdx = _stockSteinIndex OrElse sv.StockSteinIdx = _stockSteinIndex + 1 Then
                                'Stein würde an der Ausgangsposition zum liegen kommen
                                '==> nichts tun (Flugweg zur kurz zum Rückflug)
                            Else
                                _sfd.SFStock.StartNewGapJob(idxGapInsert:=sv.StockSteinIdx, bmpGapInsert:=_bmpNormal, _steinTypEnum,
                                                            idxGapRemove:=_stockSteinIndex, bmpGapRemove:=_bmpGhost, _steinTypEnum)
                            End If
                        Else
                            ' Ungültige Position ==> zurückfliegen
                            '   Stop
                        End If

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

        Private Sub DoLeftMouseDoublKlick()

            'Debug.Print("DoLeftMouseDoublKlick " & Now.ToString)

            Dim mousePos As Point = _sfd.SFRun.MousePolling.MousePos

            If Not _sfd.SFLay.rxStageUsed.Contains(mousePos) Then
                Exit Sub
            End If

            If _sfd.SFStock.StockAktCount = 0 AndAlso INI.Editor_DoubleClickRemoveStein = False Then
                Exit Sub
            End If
            If INI.Editor_DoubleClickRemoveStein Then
                Dim steinInfoIndex As Integer = _sfd.SFInf.GetTopSteinInfoIndexAtPoint(_mousePos)
                If steinInfoIndex >= 0 Then ' Andernflls Klick auf eine freie Fläche
                    Dim st As SteinTyp = _sfd.SFInf.SteinInfos(steinInfoIndex).SteinTypIndex
                    Dim request As New TileRequest(_sfd.SFRun.AktRenderMode, INI.Tile_TileColors, _sfd.SFInf.SteinInfos(steinInfoIndex).SteinTypIndex, SteinStatus.I01Normal, SteinFrameVersion.Standard, _sfd.SFLay.steinSize, INI.Tile_BasisSize)
                    Dim bmp As Bitmap = TileFactory.GetTile(request)
                    _sfd.SFStock.StartNewFadeOutJob(_steinInfoIndex, startAsGhost:=False)
                    _sfd.SFInf.RemoveSteinFromSpielfeld(steinInfoIndex)
                    _sfd.SFStock.StartNewGapJob(idxGapInsert:=_sfd.SFStock.SteinVisibleAktFistIdx, bmpGapInsert:=bmp, st)
                End If

            ElseIf INI.Editor_DoubleClickSetStein Then
                Dim tpl As Triple = _sfd.SFInf.IsFundamentKandidat(mousePos)
                If tpl.IsValideYes Then
                    'Debug.Print("DoDoubleClickGhostPosition" & Now.ToString & " - " & mousePos.ToString)
                    'mögliche Ablageposition auf dem Feld

                    Dim st As SteinTyp = _sfd.SFStock.GetSteinTypIndexDontRemove(index:=_sfd.SFStock.SteinVisibleAktFistIdx)
                    Dim request As New TileRequest(_sfd.SFRun.AktRenderMode, INI.Tile_TileColors, st, SteinStatus.I01Normal, SteinFrameVersion.Standard, _sfd.SFLay.steinSize, INI.Tile_BasisSize)
                    Dim bmp As Bitmap = TileFactory.GetTile(request)
                    _sfd.SFStock.StartNewGapJob(idxGapRemove:=_sfd.SFStock.SteinVisibleAktFistIdx, bmpGapRemove:=bmp, steinTypRemove:=st)

                    _sfd.SFInf.AddSteinToSpielfeld(st, tpl)
                End If
            End If

        End Sub

#Region "DoDoubleClickGhostPosition"

        Private _hasDoubleClickGhostBmp As Boolean
        Private _zEbeneDoubleClickGhostBmp As Integer
        Private _bmpDoubleClickGhostBmp As Bitmap
        Private _rectDoubleClickGhostBmp As Rectangle
        Private _DoubleClickGhostIsAktive As Boolean

        Public Function GetDoubleClickGhostBmp() As (has As Boolean, bmp As Bitmap, rect As Rectangle, z As Integer)
            Dim values As (has As Boolean, bmp As Bitmap, rect As Rectangle, z As Integer)
            values.has = _hasDoubleClickGhostBmp
            values.bmp = _bmpDoubleClickGhostBmp
            values.rect = _rectDoubleClickGhostBmp
            values.z = _zEbeneDoubleClickGhostBmp
            Return values
        End Function

        Private Sub SetDoubleClickGhostBmp(bmp As Bitmap, rect As Rectangle, zEbene As Integer)
            _hasDoubleClickGhostBmp = True
            _bmpDoubleClickGhostBmp = bmp
            _rectDoubleClickGhostBmp = rect
            _zEbeneDoubleClickGhostBmp = zEbene
            _DoubleClickGhostIsAktive = True
            MakeDirty()
        End Sub

        Public Sub ClearDoubleClickGhostBmp()
            _hasDoubleClickGhostBmp = False
            _bmpDoubleClickGhostBmp = Nothing
            _rectDoubleClickGhostBmp = Rectangle.Empty
            _zEbeneDoubleClickGhostBmp = -1
            _DoubleClickGhostIsAktive = False
        End Sub

        Public ReadOnly Property DoubleClickGhostIsAktive As Boolean
            Get
                Return _DoubleClickGhostIsAktive
            End Get
        End Property
        '
        ''' <summary>
        ''' Hier wird das reine Bewegen der Maus ohne gedückte Maustasten verarbeitet. 
        ''' </summary>
        Public Sub DoDoubleClickGhostPosition()

            'ClearDoubleClickGhostBmp()
            If Not INI.Editor_DoubleClickSetStein Then
                Exit Sub
            End If

            Dim mousePos As Point = _sfd.SFRun.MousePolling.MousePos

            If Not _sfd.SFLay.rxStageUsed.Contains(mousePos) Then
                Exit Sub
            End If

            If _sfd.SFStock.StockAktCount = 0 Then
                Exit Sub
            End If

            Dim tpl As Triple = _sfd.SFInf.IsFundamentKandidat(mousePos)

            If tpl.IsValideYes Then
                'Debug.Print("DoDoubleClickGhostPosition" & Now.ToString & " - " & mousePos.ToString)
                'mögliche Ablageposition auf dem Feld

                Dim st As SteinTyp = _sfd.SFStock.GetSteinTypIndexDontRemove(index:=0)

                Dim request As New TileRequest(_sfd.SFRun.AktRenderMode, INI.Tile_TileColors, st, SteinStatus.I01Normal, SteinFrameVersion.Standard, _sfd.SFLay.steinSize, INI.Tile_BasisSize, ghost:=True)
                Dim bmp As Bitmap = TileFactory.GetTile(request)

                SetDoubleClickGhostBmp(bmp, _sfd.SFInf.GetSteinRenderRect(tpl), zEbene:=tpl.z)
            End If

        End Sub

#End Region
    End Class
End Namespace