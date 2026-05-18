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
        Public Function ConsumeSchnelltestHasMouseStateChange() As MouseStateChanged
            'Wegen dem Mausrad ist das im _mouseWheelPoller angesiedelt.
            Return _mouseWheelPoller.ConsumeSchnelltestHasMouseStateChange()
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
            MakeDirty()
        End Sub

        Private Sub ClearSpecialBmps()
            For idx As Integer = 0 To SpecialBmps.UBnd
                _hasSpecialBmps(idx) = False
                _bmpSpecialBmps(idx) = Nothing
                _rectSpecialBmps(idx) = Rectangle.Empty
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
                        SetSpecialBmps(SpecialBmps.AtStockInsertPos, .BmpGapInsert, .RectGapInsert)
                    End If
                    If .HasBmpGapRemove Then
                        SetSpecialBmps(SpecialBmps.AtStockRemovePos, .BmpGapRemove, .RectGapRemove)
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

            If _leftMouseDoublKlick Then
                If _klickArea <> AirKlickArea.Editor Then
                    'nur im Editor zulässig.
                    _leftMouseDoublKlick = False
                End If
            End If

            If _leftMouseDoublKlick Then
                'Debug.Print("_leftMouseDoublKlick - " & Now.ToString)
                DoLeftMouseDoublKlick
                Exit Sub
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
            'und im Renderer aus PaintStock zugegriffen wird.
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
                            CreateDragDropJobFromEditor(steinInfoIndex) 'setzt _isInit
                            _sfd.SFRun.MousePolling.StartDragDrop()
                            DoRenderstep()
                        End If

                    ElseIf _klickArea = AirKlickArea.Stock Then

                        Dim sv As SFStockJobs.StockValues = _sfd.SFStock.GetSelectedSteinValues(_mousePos, getDropPosition:=False)
                        If sv.HasValueInsideStock Then
                            CreateDragDropJobFromStock(sv.StockSteinIdx, sv.SteinIndex, sv.RectStein)
                            _sfd.SFRun.MousePolling.StartDragDrop()
                            DoRenderstep()
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
                    ' Debug.Print($"Editor DragDropJob.Active {_mousePos}")
                    SetSpecialBmps(SpecialBmps.AtEditorSrcPos, _bmpGhost, _sfd.SFInf.SteinInfos(_steinInfoIndex).RectStein)
                    SetSpecialBmps(SpecialBmps.AtEditorMousePos, _bmpSelected, _mouseAnkerPosition.GetAktRectPlane(_mousePos))

                    Dim tpl As Triple = _sfd.SFInf.IsFundamentKandidat(_mousePos)
                    If tpl.IsValideYes Then
                        'mögliche Ablageposition auf dem Feld
                        SetSpecialBmps(SpecialBmps.AtEditorCanDropPos, _bmpGhost, _sfd.SFInf.GetSteinRenderRect(tpl))
                    Else
                        Dim sv As SFStockJobs.StockValues = _sfd.SFStock.GetSelectedSteinValues(_mousePos, getDropPosition:=True, leaveCenterBlank:=True)
                        If sv.HasValue Then
                            'mögliche Ablageposition im Vorrat
                            If Not (sv.StockSteinIdx = _stockSteinIndex OrElse sv.StockSteinIdx = _stockSteinIndex + 1) Then
                                'links und rechts von der Ausgangsposition ist eine Ablage sinnlos,
                                'da sie zur Ausgangsposition führt, daher die Einschränkung.
                                SetSpecialBmps(SpecialBmps.AtStockCanDropPos, _bmpGhost, sv.RectStein)
                            End If
                        ElseIf _klickArea = AirKlickArea.Stock Then
                            If _sfd.SFStock.StockAktCount = 0 Then
                                With _sfd.SFLay
                                    Dim rect As New Rectangle(.rxStock.Left, .rxStock.Top, .steinWidth, .steinHeight)
                                    SetSpecialBmps(SpecialBmps.AtStockCanDropPos, _bmpGhost, rect)
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
                        _sfd.SFInf.RemoveSteinFromSpielfeld(_steinInfoIndex)
                        _sfd.SFInf.AddSteinToSpielfeld(_steinTypEnum, tpl)
                        MakeDirty()
                    Else
                        'Stein zurück in den Vorrat
                        Dim sv As SFStockJobs.StockValues = _sfd.SFStock.GetSelectedSteinValues(_mousePos, getDropPosition:=True, leaveCenterBlank:=True)
                        If sv.HasValueInsideStock Then
                            _sfd.SFInf.RemoveSteinFromSpielfeld(_steinInfoIndex)
                            _sfd.SFStock.StartNewGapJob(idxGapInsert:=sv.StockSteinIdx, bmpGapInsert:=_bmpNormal, _steinTypEnum)
                        ElseIf sv.HasValueAfterEndOfStock Then
                            Dim st As SteinTyp = _sfd.SFInf.SteinInfos(_steinInfoIndex).SteinTypIndex
                            _sfd.SFInf.RemoveSteinFromSpielfeld(_steinInfoIndex)
                            _sfd.SFStock.AddAtStockEnd(st)
                            MakeDirty()
                        Else
                            Dim rect As New Rectangle(_sfd.SFLay.rxStock.Left, _sfd.SFLay.rxStock.Top, _sfd.SFLay.steinWidth, _sfd.SFLay.steinHeight)

                            If _sfd.SFStock.StockAktCount = 0 AndAlso rect.Contains(_mousePos) Then
                                Dim st As SteinTyp = _sfd.SFInf.SteinInfos(_steinInfoIndex).SteinTypIndex
                                _sfd.SFInf.RemoveSteinFromSpielfeld(_steinInfoIndex)
                                _sfd.SFStock.AddAtStockEnd(st)
                                SetSpecialBmps(SpecialBmps.AtStockCanDropPos, _bmpNormal, rect)
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

        Private Sub DoRenderstepStock(job As DragDropJob)
            Select Case job
                Case DragDropJob.FirstStep

                    Dim rect As Rectangle = _sfd.SFStock.GetRectFromStockSteinIdx(_stockSteinIndex, getDropPosition:=False)
                    SetSpecialBmps(SpecialBmps.AtStockSrcPos, _bmpGhost, rect)

                    'Dim sv As SFStockJobs.StockValues = _sfd.SFStock.GetSelectedSteinValues(_mousePos, getDropPosition:=True)
                    'If sv.HasValues Then
                    '    SetSpecialBmps(SpecialBmps.AtStockCanDropPos, _bmpGhost, sv.RectStein)
                    'End If

                    _mouseAnkerPosition = New MouseAnkerPosition(_rectSpecialBmps(SpecialBmps.AtStockSrcPos), _mousePos, _sfd.SFLay.rxStageUsed)

                Case DragDropJob.Active

                    Dim rect As Rectangle = _sfd.SFStock.GetRectFromStockSteinIdx(_stockSteinIndex, getDropPosition:=False)
                    SetSpecialBmps(SpecialBmps.AtStockSrcPos, _bmpGhost, rect)
                    If _klickArea = AirKlickArea.Stock OrElse _klickArea = AirKlickArea.StockScrollbar Then
                        SetSpecialBmps(SpecialBmps.AtStockMousePos, _bmpSelected, _mouseAnkerPosition.GetAktRectPlane(_mousePos))
                    Else
                        SetSpecialBmps(SpecialBmps.AtEditorMousePos, _bmpSelected, _mouseAnkerPosition.GetAktRectPlane(_mousePos))
                    End If

                    Dim tpl As Triple = _sfd.SFInf.IsFundamentKandidat(_mousePos)
                    If tpl.IsValideYes Then
                        'mögliche Ablageposition auf dem Feld
                        SetSpecialBmps(SpecialBmps.AtEditorCanDropPos, _bmpGhost, _sfd.SFInf.GetSteinRenderRect(tpl))
                    Else
                        Dim sv As SFStockJobs.StockValues = _sfd.SFStock.GetSelectedSteinValues(_mousePos, getDropPosition:=True, leaveCenterBlank:=True)
                        If sv.HasValue Then
                            'mögliche Ablageposition im Vorrat
                            If Not (sv.StockSteinIdx = _stockSteinIndex OrElse sv.StockSteinIdx = _stockSteinIndex + 1) Then
                                'links und rechts von der Ausgangsposition ist eine Ablage sinnlos,
                                'da sie zur Ausgangsposition führt, daher die Einschränkung.
                                SetSpecialBmps(SpecialBmps.AtStockCanDropPos, _bmpGhost, sv.RectStein)
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
                        If _sfd.SFStock.StockAktUBnd > _stockSteinIndex Then
                            'Entfernen mit Animation
                            _sfd.SFStock.StartNewGapJob(idxGapRemove:=_stockSteinIndex, bmpGapRemove:=_bmpGhost, steinTypRemove:=_steinTypEnum)
                        Else
                            'den allerletzten Stein rechts ohne Animation
                            _sfd.SFStock.GetSteinTypIndexAndRemove(_stockSteinIndex)
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

            Debug.Print("DoLeftMouseDoublKlick " & Now.ToString)

            Dim mousePos As Point = _sfd.SFRun.MousePolling.MousePos

            If Not _sfd.SFLay.rxStageUsed.Contains(mousePos) Then
                Exit Sub
            End If

            If _sfd.SFStock.StockAktCount = 0 Then
                Exit Sub
            End If

            Dim tpl As Triple = _sfd.SFInf.IsFundamentKandidat(mousePos)
            If tpl.IsValideYes Then
                'Debug.Print("DoMouseUpMoveAktion" & Now.ToString & " - " & mousePos.ToString)
                'mögliche Ablageposition auf dem Feld

                Dim st As SteinTyp = _sfd.SFStock.GetSteinTypIndexAndRemove(index:=0)
                _sfd.SFInf.AddSteinToSpielfeld(st, tpl)
            End If
        End Sub

#Region "DoMouseUpMoveAktion"

        Private _hasAloneGhostBmp As Boolean
        Private _bmpAloneGhostBmp As Bitmap
        Private _rectAloneGhostBmp As Rectangle

        Public Function GetAloneGhostBmp() As (has As Boolean, bmp As Bitmap, rect As Rectangle)
            Dim values As (has As Boolean, bmp As Bitmap, rect As Rectangle)
            values.has = _hasAloneGhostBmp
            values.bmp = _bmpAloneGhostBmp
            values.rect = _rectAloneGhostBmp
            Return values
        End Function

        Private Sub SetAloneGhostBmp(bmp As Bitmap, rect As Rectangle)
            _hasAloneGhostBmp = True
            _bmpAloneGhostBmp = bmp
            _rectAloneGhostBmp = rect
            MakeDirty()
        End Sub

        Private Sub ClearAloneGhostBmp()
            _hasAloneGhostBmp = False
            _bmpAloneGhostBmp = Nothing
            _rectAloneGhostBmp = Rectangle.Empty
        End Sub

        '
        ''' <summary>
        ''' Hier wird das reine Bewegen der Maus ohne gedückte Maustasten verarbeitet. 
        ''' </summary>
        Public Sub DoMouseUpMoveAktion()

            ClearAloneGhostBmp()

            If _sfd.SFRun.MousePolling.LeftMouseDown OrElse _sfd.SFRun.MousePolling.RightMouseDown Then
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
                'Debug.Print("DoMouseUpMoveAktion" & Now.ToString & " - " & mousePos.ToString)
                'mögliche Ablageposition auf dem Feld

                Dim st As SteinTyp = _sfd.SFStock.GetSteinTypIndexDontRemove(index:=0)

                Dim request As New TileRequest(_sfd.SFRun.AktRenderMode, INI.Tile_TileColors, st, SteinStatus.I01Normal, SteinFrameVersion.Standard, _sfd.SFLay.steinSize, INI.Tile_BasisSize, ghost:=True)
                Dim bmp As Bitmap = TileFactory.GetTile(request)

                SetAloneGhostBmp(bmp, _sfd.SFInf.GetSteinRenderRect(tpl))
            End If

        End Sub

#End Region
    End Class
End Namespace