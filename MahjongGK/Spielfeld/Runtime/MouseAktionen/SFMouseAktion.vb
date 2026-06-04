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
#Disable Warning IDE0044

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
        Public Function ConsumeQuicktestHasMouseStateChange(rxContent As RectangleX) As MouseStateChanged
            'Wegen dem Mausrad ist das im _mouseWheelPoller angesiedelt.
            Return _mouseWheelPoller.ConsumeSchnelltestHasMouseStateChange(rxContent)
        End Function

        Public Sub CreateDragDropJobFromEditor(steinInfoIndex As Integer)
            If IsNothing(_sfd) Then
                Throw New Exception("Programmierfehler: DragDrop ohne sdf instanziert.")
            End If
            _dragDropStartFrom = DragDropStartFrom.Editor
            _dragDropJob = DragDropJob.FirstStep
            _steinInfoIndex = steinInfoIndex
            _steinSymbolEnum = _sfd.SFInf.SteinInfos(steinInfoIndex).SteinSymbolIndex
            _stockSteinIndex = -1
            _srcPos3D = _sfd.SFInf.SteinInfos(steinInfoIndex).Pos3D
            _isInit = InitDragDropBitmaps()
        End Sub

        Public Sub CreateDragDropJobFromStock(stockSteinIndex As Integer, steinSymbolEnum As SteinSymbol)
            If IsNothing(_sfd) Then
                Throw New Exception("Programmierfehler: DragDrop ohne sdf instanziert.")
            End If
            _steinInfoIndex = -1
            _stockSteinIndex = stockSteinIndex
            _dragDropStartFrom = DragDropStartFrom.Stock
            _dragDropJob = DragDropJob.FirstStep
            _steinSymbolEnum = steinSymbolEnum
            _srcPos3D = Nothing
            _isInit = InitDragDropBitmaps()
        End Sub

        Public Sub ClearLastMouseFlight()
            _isInit = False
        End Sub

        Private ReadOnly _sfd As SFDaten
        Private ReadOnly _mouseWheelPoller As MouseWheelPoller

        Private _bmpNormal As Bitmap = Nothing
        Private _bmpGhostLight As Bitmap = Nothing
        Private _bmpGhostMedium As Bitmap = Nothing
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
        ''' Der SteinSymbolIndex besimmt das Symbol auf der Oberseite des Steines.
        ''' </summary>
        Private _steinSymbolEnum As SteinSymbol
        Private _dragDropStartFrom As DragDropStartFrom
        Private _dragDropJob As DragDropJob
        Private _srcRect As Rectangle
        Private _srcPos3D As Triple
        Private _isInit As Boolean
        Private _IsDirty As Boolean

        Private _polling As Boolean
        Private _leftMousePressed As Boolean
        Private _leftMouseDown As Boolean
        Private _leftMouseReleased As Boolean
        Private _dragDropMoved As Boolean
        '
        ''' <summary>
        ''' Schließt _dragDropMoved ein!
        ''' </summary>
        Private _dragDropAktive As Boolean
        Private _endDragDrop As Boolean
        Private _mousePos As Point
        Private _MousePosOnStarttDragDrop As Point
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

            _hasSpecialBmps(specialBitmap) = bmp IsNot Nothing
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
                request = New TileRequest(_sfd.SFRun.AktRenderMode, INI.Tile_TileColors, _sfd.SFInf.SteinInfos(_steinInfoIndex).SteinSymbolIndex, SteinStatus.I01Normal, _sfd.SFLay.steinSize, INI.Tile_BasisSize, SteinGhost.None)
                _bmpNormal = TileFactory.GetTile(request)
                request = New TileRequest(_sfd.SFRun.AktRenderMode, INI.Tile_TileColors, _sfd.SFInf.SteinInfos(_steinInfoIndex).SteinSymbolIndex, SteinStatus.I01Normal, _sfd.SFLay.steinSize, INI.Tile_BasisSize, SteinGhost.Light)
                _bmpGhostLight = TileFactory.GetTileDeepCopy(request)
                '
                request = New TileRequest(_sfd.SFRun.AktRenderMode, INI.Tile_TileColors, _sfd.SFInf.SteinInfos(_steinInfoIndex).SteinSymbolIndex, SteinStatus.I01Normal, _sfd.SFLay.steinSize, INI.Tile_BasisSize, SteinGhost.Medium)
                _bmpGhostMedium = TileFactory.GetTile(request)

            ElseIf _steinSymbolEnum >= 0 Then
                'Stein aus dem Vorrat
                'Falls _steinSymbolEnum ungültig ist. wird die Errorgrafik erzeugt
                request = New TileRequest(_sfd.SFRun.AktRenderMode, INI.Tile_TileColors, _steinSymbolEnum, SteinStatus.I01Normal, _sfd.SFLay.steinSize, INI.Tile_BasisSize, SteinGhost.None)
                _bmpNormal = TileFactory.GetTile(request)
                request = New TileRequest(_sfd.SFRun.AktRenderMode, INI.Tile_TileColors, _steinSymbolEnum, SteinStatus.I01Normal, _sfd.SFLay.steinSize, INI.Tile_BasisSize, SteinGhost.Light)
                _bmpGhostLight = TileFactory.GetTileDeepCopy(request)
                '
                request = New TileRequest(_sfd.SFRun.AktRenderMode, INI.Tile_TileColors, _steinSymbolEnum, SteinStatus.I01Normal, _sfd.SFLay.steinSize, INI.Tile_BasisSize, SteinGhost.Medium)
                _bmpGhostMedium = TileFactory.GetTile(request)
            Else
                Return False
            End If

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
        '
        ''' <summary>
        ''' Stoppt alle laufenden Animationen
        ''' </summary>
        Public Sub Clear()

            _sfd.SFRun.MousePolling.AbortDragDrop()
            ClearSpecialBmps()
            ClearDoubleClickGhostBmp()
            _sfd.SFStock.AbortAndOrClearGapJob()

        End Sub

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
        Public Sub DoMouseDownDoubleklickAndDragDropAktion()

#Region "Verteiler"
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
            _leftMouseDown = _sfd.SFRun.MousePolling.LeftMouseDown
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
                frmMain.UCtlSpielfeldMain.ContextMenuStrip = New ContextMenueEditor(_sfd)
                Exit Sub
            End If

            If _leftMouseDoublKlick Then
                'falls durch den ersten Klick ein DragDroJob gestartet wurde:
                _sfd.SFRun.MousePolling.AbortDragDrop()

                If _klickArea = AirKlickArea.Editor Then
                    DoEditorLeftMouseDoublKlick()
                    Exit Sub
                ElseIf _klickArea = AirKlickArea.Stock Then
                    DoStockLeftMouseDoublKlick()
                    Exit Sub
                Else
                    'nur im Editor und im Vorrat zulässig.
                    _leftMouseDoublKlick = False
                End If
            End If

            DoStockScrollbar()

            If _klickArea = AirKlickArea.Spielfeld Then
                DoKlickSpiel(insideSpielfeld:=True)
                Exit Sub

            ElseIf _klickAreaGroup = AirKlickAreaGroup.Historybox Then
                DoKlickHistorboxes()
                Exit Sub

            ElseIf _klickAreaGroup = AirKlickAreaGroup.Button Then
                If _sfd.SFRun.AktRenderMode = AktRenderMode.Spiel Then

                ElseIf _sfd.SFRun.AktRenderMode = AktRenderMode.Edit Then
                    If _klickArea = AirKlickArea.Undo Then
                        DoEditorUndo()

                    ElseIf _klickArea = AirKlickArea.Redo Then
                        DoEditorRedo()
                    End If
                End If
                Exit Sub
            ElseIf _sfd.SFRun.AktRenderMode = AktRenderMode.Spiel Then
                DoKlickSpiel(insideSpielfeld:=False)
                Exit Sub
            End If

            'Hier werden die aktuellen Rechtangle des Stock festgelegt, auf die im folgenden
            'und im Renderer aus Paint_Stock zugegriffen wird.
            'muus genau hier zwischen der Scrollbar und Spielfels, Editor, Stock
            'stehen, da die Scrollbar die Werte ändert und dann der Startoffset
            'bei der Ausgabe falsch ist.
            _sfd.SFStock.SetRectsFromVisibleStockItems()

            If _klickArea = AirKlickArea.Editor OrElse _klickArea = AirKlickArea.Stock Then
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

                    If _klickArea = AirKlickArea.Editor Then
                        Dim steinInfoIndex As Integer = _sfd.SFInf.GetTopSteinInfoIndexAtPoint(_mousePos)
                        If steinInfoIndex >= 0 Then ' Andernflls Klick auf eine freie Fläche
                            If _sfd.SFRun.MousePolling.StartDragDrop() Then
                                CreateDragDropJobFromEditor(steinInfoIndex) 'setzt _isInit
                                DoRenderstepEditorOrStock()
                            End If
                        End If

                    ElseIf _klickArea = AirKlickArea.Stock Then

                        Dim sv As SFStockJobs.StockValues = _sfd.SFStock.GetSelectedSteinValues(_mousePos, getDropPosition:=False)
                        If sv.HasValueInsideStock Then
                            If _sfd.SFRun.MousePolling.StartDragDrop() Then
                                CreateDragDropJobFromStock(sv.StockSteinIdx, sv.SteinIndex)
                                DoRenderstepEditorOrStock()
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
                DoRenderstepEditorOrStock()

            ElseIf _endDragDrop Then
                _dragDropJob = DragDropJob.EndJob

                DoRenderstepEditorOrStock()
            End If

        End Sub

#End Region

#Region "Editor und Stock"

        Private Sub DoStockScrollbar()

            If _aktHScrollbarJob = ScrollbarJob.MouseMove Then
                If _leftMouseDown Then
                    _sfd.SFRun.HScrollBarStock.HandleMousemMove(_mousePos)
                    MakeDirty()
                Else
                    _sfd.SFRun.HScrollBarStock.HandleMouseUp()
                    _aktHScrollbarJob = ScrollbarJob.None
                    MakeDirty()
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
            End If
        End Sub
        '
        Private Sub DoRenderstepEditorOrStock()

            If _dragDropJob = DragDropJob.FirstStep Then
                Select Case _dragDropStartFrom
                    Case DragDropStartFrom.Editor
                        DoRenderstepEditor(DragDropJob.FirstStep)

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
                    _sfd.SFInf.TmpRealRemoveStein(_steinInfoIndex)

                    'Die Originalposition des Steins, der wegen TmpRemoveStein nicht mehr vorhanden ist.
                    'Noch ohne Geist an der ursprünglichen stelle, dafür als Selected Bitmap
                    SetSpecialBmps(SpecialBmps.AtEditorSrcPos, _bmpGhostMedium, _sfd.SFInf.SteinInfos(_steinInfoIndex).RectStein, _sfd.SFInf.SteinInfos(_steinInfoIndex).Z)

                    'wird in Case DragDropJob.Active gebraucht
                    _mouseAnkerPosition = New MouseAnkerPosition(_rectSpecialBmps(SpecialBmps.AtEditorSrcPos), _mousePos, _sfd.SFLay.rxStageUsed)
                    _MousePosOnStarttDragDrop = _mousePos

                Case DragDropJob.Active
                    '
                    ' Debug.Print($"Editor DragDropJob.Active {_mousePos}")
                    SetSpecialBmps(SpecialBmps.AtEditorSrcPos, _bmpGhostLight, _sfd.SFInf.SteinInfos(_steinInfoIndex).RectStein, _sfd.SFInf.SteinInfos(_steinInfoIndex).Z)
                    SetSpecialBmps(SpecialBmps.AtEditorMousePos, _bmpGhostMedium, _mouseAnkerPosition.GetAktRectPlane(_mousePos), DUMMY)

                    Dim tpl As Triple = _sfd.SFInf.IsFundamentKandidat(_mousePos)
                    If tpl.IsValideYes Then
                        'mögliche Ablageposition auf dem Feld
                        SetSpecialBmps(SpecialBmps.AtEditorCanDropPos, _bmpGhostLight, _sfd.SFInf.GetSteinRenderRect(tpl), tpl.z)
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
                                SetSpecialBmps(SpecialBmps.AtStockCanDropPos, _bmpGhostLight, sv.RectStein, DUMMY)
                            End If
                        ElseIf _klickArea = AirKlickArea.Stock Then
                            If _sfd.SFStock.StockAktCount = 0 Then
                                With _sfd.SFLay
                                    Dim rect As New Rectangle(.rxStock.Left, .rxStock.Top, .steinWidth, .steinHeight)
                                    SetSpecialBmps(SpecialBmps.AtStockCanDropPos, _bmpGhostLight, rect, DUMMY)
                                End With
                            End If
                        End If
                    End If

                Case DragDropJob.EndJob
                    '()
                    Dim tplDst As Triple = _sfd.SFInf.IsFundamentKandidat(_mousePos)
                    '()
                    _sfd.SFInf.TmpRealReturnRemovedStein()
                    '()
                    If tplDst.IsValideYes Then
                        'Umsetzen eines Steines innerhalb des Editors.
                        'Hier ist der Grund, warum der Stein um ein Feld nach rechts und/oder unten 
                        'verschoben wird, wenn man ihn in einem anderen Quadranten als den linken
                        'oberen klickt. Daher nur zulassen, wenn die Maus wenigstens etwas bewegt wurde.
                        'Damit wird der erste Klick eines Doppelklicks eleminiert, denn
                        'andernfalls wurde die linke Maustaste auf dem Stein losgelassen 
                        'und damit soll das DragDrop abgebrochen werden statt es auszuführen.
                        If IsMouseMoved(_mousePos, _MousePosOnStarttDragDrop) Then
                            _sfd.SFStock.StartNewFadeOutJob(_steinInfoIndex, startAsGhost:=True)
                            With _sfd.SFInf
                                Dim tplSrc As Triple = .SteinInfos(_steinInfoIndex).Pos3D
                                .RealRemoveSteinFromSpielfeld(_steinInfoIndex)
                                .AddSteinToSpielfeld(_steinSymbolEnum, tplDst)
                                .EditorUndo.AddNewUndo(_steinSymbolEnum, tplSrc, tplDst)
                            End With
                            MakeDirty()
                        Else
                            _sfd.SFRun.MousePolling.AbortDragDrop()
                        End If
                    Else
                        'Stein zurück in den Vorrat
                        Dim svDst As SFStockJobs.StockValues = _sfd.SFStock.GetSelectedSteinValues(_mousePos, getDropPosition:=True, leaveCenterBlank:=False)
                        Dim tplSrc As Triple = _sfd.SFInf.SteinInfos(_steinInfoIndex).Pos3D
                        If svDst.HasValueInsideStock Then

                            _sfd.SFStock.StartNewFadeOutJob(_steinInfoIndex, startAsGhost:=True)
                            _sfd.SFInf.RealRemoveSteinFromSpielfeld(_steinInfoIndex)
                            _sfd.SFStock.StartNewGapJob(idxGapInsert:=svDst.StockSteinIdx, bmpGapInsert:=_bmpNormal, _steinSymbolEnum)
                            _sfd.SFInf.EditorUndo.AddNewUndo(_steinSymbolEnum, tplSrc, svDst.StockSteinIdx)
                            MakeDirty()
                        ElseIf svDst.HasValueAfterEndOfStock Then
                            _sfd.SFStock.StartNewFadeOutJob(_steinInfoIndex, startAsGhost:=True)
                            Dim st As SteinSymbol = _sfd.SFInf.SteinInfos(_steinInfoIndex).SteinSymbolIndex
                            _sfd.SFInf.RealRemoveSteinFromSpielfeld(_steinInfoIndex)
                            _sfd.SFStock.AddAtStockEnd(st)
                            _sfd.SFStock.NotifyStockInsert(ATEND)
                            _sfd.SFInf.EditorUndo.AddNewUndo(_steinSymbolEnum, tplSrc, svDst.StockSteinIdx)
                            MakeDirty()
                        Else
                            Dim rect As New Rectangle(_sfd.SFLay.rxStock.Left, _sfd.SFLay.rxStock.Top, _sfd.SFLay.steinWidth, _sfd.SFLay.steinHeight)

                            If _sfd.SFStock.StockAktCount = 0 AndAlso rect.Contains(_mousePos) Then
                                'Der Stock ist leer. Prüfen, ob die Maus ganz am Anfang auf dem Geiste-Symbol steht
                                _sfd.SFStock.StartNewFadeOutJob(_steinInfoIndex, startAsGhost:=True)
                                Dim st As SteinSymbol = _sfd.SFInf.SteinInfos(_steinInfoIndex).SteinSymbolIndex
                                _sfd.SFInf.RealRemoveSteinFromSpielfeld(_steinInfoIndex)
                                _sfd.SFStock.AddAtStockEnd(st)
                                _sfd.SFStock.NotifyStockInsert(ATEND)
                                SetSpecialBmps(SpecialBmps.AtStockCanDropPos, _bmpNormal, rect, DUMMY)
                                _sfd.SFInf.EditorUndo.AddNewUndo(_steinSymbolEnum, tplSrc, svDst.StockSteinIdx)
                                MakeDirty()
                            Else
                                'Ungültige Position =>= zurückfliegen in den Editor
                                Dim plane As New Airplane(_sfd, _steinInfoIndex, _mousePos, AirPointIs.CenterQuadrantLO, PlaneFlightPath.Direkt)   '= Airplane.CreateEditorFlyBackInsideEditorAirplane(_sfd, _steinInfoIndex, startPoint:=_mousePos)
                                _sfd.SFAir.AddPlane(plane)
                            End If

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
                    SetSpecialBmps(SpecialBmps.AtStockSrcPos, _bmpGhostLight, rect, DUMMY)

                    'Dim sv As SFStockJobs.StockValues = _sfd.SFStock.GetSelectedSteinValues(_mousePos, getDropPosition:=True)
                    'If sv.HasValues Then
                    '    SetSpecialBmps(SpecialBmps.AtStockCanDropPos, _bmpGhost, sv.RectStein)
                    'End If

                    _mouseAnkerPosition = New MouseAnkerPosition(_rectSpecialBmps(SpecialBmps.AtStockSrcPos), _mousePos, _sfd.SFLay.rxStageUsed)

                Case DragDropJob.Active

                    Dim rect As Rectangle = _sfd.SFStock.GetRectFromStockSteinIdx(_stockSteinIndex, getDropPosition:=False)

                    SetSpecialBmps(SpecialBmps.AtStockSrcPos, _bmpGhostLight, rect, DUMMY)
                    If _klickArea = AirKlickArea.Stock OrElse _klickArea = AirKlickArea.StockScrollbar Then
                        SetSpecialBmps(SpecialBmps.AtStockMousePos, _bmpGhostMedium, _mouseAnkerPosition.GetAktRectPlane(_mousePos), DUMMY)
                    Else
                        SetSpecialBmps(SpecialBmps.AtEditorMousePos, _bmpGhostMedium, _mouseAnkerPosition.GetAktRectPlane(_mousePos), DUMMY)
                    End If

                    Dim tpl As Triple = _sfd.SFInf.IsFundamentKandidat(_mousePos)
                    If tpl.IsValideYes Then
                        'mögliche Ablageposition auf dem Feld
                        SetSpecialBmps(SpecialBmps.AtEditorCanDropPos, _bmpGhostLight, _sfd.SFInf.GetSteinRenderRect(tpl), tpl.z)
                    Else
                        Dim sv As SFStockJobs.StockValues = _sfd.SFStock.GetSelectedSteinValues(_mousePos, getDropPosition:=True, leaveCenterBlank:=True)
                        If sv.HasValue Then
                            'mögliche Ablageposition im Vorrat
                            If Not (sv.StockSteinIdx = _stockSteinIndex OrElse sv.StockSteinIdx = _stockSteinIndex + 1) Then
                                'links und rechts von der Ausgangsposition ist eine Ablage sinnlos,
                                'da sie zur Ausgangsposition führt, daher die Einschränkung.
                                SetSpecialBmps(SpecialBmps.AtStockCanDropPos, _bmpGhostLight, sv.RectStein, DUMMY)
                            End If
                        End If
                    End If

                Case DragDropJob.EndJob
                    '()
                    Dim tplDst As Triple = _sfd.SFInf.IsFundamentKandidat(_mousePos)
                    '()
                    If tplDst.IsValideYes Then
                        'Ablegen eines Steines aus dem Vorrat
                        _sfd.SFInf.AddSteinToSpielfeld(_steinSymbolEnum, tplDst)
                        '

                        'Entfernen aus dem Vorrat
                        If _sfd.SFStock.StockAktUBnd > _stockSteinIndex Then
                            'Entfernen mit Animation
                            _sfd.SFStock.StartNewGapJob(idxGapRemove:=_stockSteinIndex, bmpGapRemove:=_bmpGhostLight, steinSymbolRemove:=_steinSymbolEnum, gapRemoveUseFirstRect:=True)
                        Else
                            'den allerletzten Stein rechts ohne Animation
                            _sfd.SFStock.GetSteinSymbolIndexAndRemove(_stockSteinIndex)
                            _sfd.SFStock.NotifyStockRemove(_stockSteinIndex)
                        End If
                        _sfd.SFInf.EditorUndo.AddNewUndo(_steinSymbolEnum, _stockSteinIndex, tplDst)
                    Else
                        'Stein im Vorrat umlagern
                        Dim sv As SFStockJobs.StockValues = _sfd.SFStock.GetSelectedSteinValues(_mousePos, getDropPosition:=True, leaveCenterBlank:=True)

                        If sv.HasValue Then ' das ist einschließlich sv.HasValueAfterEndOfStock 
                            If sv.StockSteinIdx = _stockSteinIndex OrElse sv.StockSteinIdx = _stockSteinIndex + 1 Then
                                'Stein würde an der Ausgangsposition zum liegen kommen
                                '==> nichts tun (Flugweg zur kurz zum Rückflug)
                            Else
                                _sfd.SFStock.StartNewGapJob(idxGapInsert:=sv.StockSteinIdx, bmpGapInsert:=_bmpNormal, _steinSymbolEnum,
                                                            idxGapRemove:=_stockSteinIndex, bmpGapRemove:=_bmpGhostLight, _steinSymbolEnum)
                                _sfd.SFInf.EditorUndo.AddNewUndo(_steinSymbolEnum, _stockSteinIndex, sv.StockSteinIdx)
                            End If
                        Else
                            ' Ungültige Position ==> zurückfliegen in den Vorrat
                            Dim ptCenter As Point = _sfd.SFStock.GetCenterPointFromStockSteinIdx(_stockSteinIndex)
                            Dim plane As New Airplane(_sfd, _stockSteinIndex, _steinSymbolEnum, _mousePos, AirPointIs.CenterQuadrantLO, ptCenter, AirPointIs.Center, PlaneFlightPath.Direkt)   '= Airplane.CreateEditorFlyBackInsideEditorAirplane(_sfd, _steinInfoIndex, startPoint:=_mousePos)
                            _sfd.SFAir.AddPlane(plane)
                        End If

                    End If

            End Select
        End Sub

#End Region

#Region "Doubleklick Editor und Stock"

        Private Sub DoEditorLeftMouseDoublKlick()

            'Debug.Print("DoEditorLeftMouseDoublKlick " & Now.ToString)

            If _sfd.SFStock.StockAktCount = 0 AndAlso INI.Editor_DoubleClickRemoveStein = False Then
                Exit Sub
            End If
            If INI.Editor_DoubleClickRemoveStein Then
                'Stein aus dem Editor entfernen
                Dim steinInfoIndex As Integer = _sfd.SFInf.GetTopSteinInfoIndexAtPoint(_mousePos)
                If steinInfoIndex >= 0 Then ' Andernfalls Klick auf eine freie Fläche
                    Dim tplSrc As Triple = _sfd.SFInf.SteinInfos(steinInfoIndex).Pos3D
                    Dim steinSymbol As SteinSymbol = _sfd.SFInf.SteinInfos(steinInfoIndex).SteinSymbolIndex
                    Dim request As New TileRequest(_sfd.SFRun.AktRenderMode, INI.Tile_TileColors, _sfd.SFInf.SteinInfos(steinInfoIndex).SteinSymbolIndex, SteinStatus.I01Normal, _sfd.SFLay.steinSize, INI.Tile_BasisSize, SteinGhost.Medium)
                    Dim bmp As Bitmap = TileFactory.GetTile(request)
                    _sfd.SFStock.StartNewFadeOutJob(steinInfoIndex, startAsGhost:=False)
                    _sfd.SFStock.StartNewGapJob(idxGapInsert:=_sfd.SFStock.SteinVisibleAktFistIdx, bmpGapInsert:=bmp, steinSymbol)
                    _sfd.SFInf.RealRemoveSteinFromSpielfeld(steinInfoIndex)
                    _sfd.SFInf.EditorUndo.AddNewUndo(steinSymbol, tplSrc, _sfd.SFStock.SteinVisibleAktFistIdx)
                    'Mit dem Steinflug an dieser Stelle, das funktioniert so nicht,
                    'denn dadurch, daß der Stein entfernt wurde, wird er in Haupt-Auswertungsschleife
                    'in SfRender.Paint_Editor
                    'nicht mehr erreicht, es kann also keine Animation ablaufen.
                End If

            ElseIf INI.Editor_DoubleClickSetStein Then
                'Stein dem Editor hinzufügen
                Dim tplDst As Triple = _sfd.SFInf.IsFundamentKandidat(_mousePos)
                If tplDst.IsValideYes Then
                    'mögliche Ablageposition auf dem Feld

                    Dim stockSteinIndex As Integer = _sfd.SFStock.SteinVisibleAktFistIdx
                    Dim steinSymbol As SteinSymbol = _sfd.SFStock.GetSteinSymbolIndexDontRemove(stockSteinIndex)
                    Dim request As New TileRequest(_sfd.SFRun.AktRenderMode, INI.Tile_TileColors, steinSymbol, SteinStatus.I01Normal, _sfd.SFLay.steinSize, INI.Tile_BasisSize, SteinGhost.Medium)
                    Dim bmp As Bitmap = TileFactory.GetTile(request)
                    _sfd.SFStock.StartNewGapJob(idxGapRemove:=_sfd.SFStock.SteinVisibleAktFistIdx, bmpGapRemove:=bmp, steinSymbolRemove:=steinSymbol)

                    _sfd.SFInf.AddSteinToSpielfeld(steinSymbol, tplDst)
                    Dim steinInfoIndex As Integer = _sfd.SFInf.GetSteinInfoIndex(tplDst)
                    _sfd.SFInf.EditorUndo.AddNewUndo(steinSymbol, _sfd.SFStock.SteinVisibleAktFistIdx, tplDst)
                    'Auf Steinflug verzichtet.
                End If
            End If

        End Sub

        Private Sub DoStockLeftMouseDoublKlick()
            If _sfd.SFStock.StockAktCount = 0 AndAlso INI.Editor_DoubleClickRemoveStein = False Then
                Exit Sub
            End If
            Dim sv As SFStockJobs.StockValues = _sfd.SFStock.GetSelectedSteinValues(_mousePos, getDropPosition:=False)
            If sv.HasValueInsideStock Then
                If sv.StockSteinIdx = 0 Then
                    'Stein würde an der Ausgangsposition zum liegen kommen
                    '==> nichts tun (Flugweg zur kurz zum Rückflug)
                Else
                    _sfd.SFStock.StartNewGapJob(idxGapInsert:=0, bmpGapInsert:=_bmpNormal, _steinSymbolEnum,
                                               idxGapRemove:=sv.StockSteinIdx, bmpGapRemove:=_bmpGhostMedium, _steinSymbolEnum, gapRemoveUseFirstRect:=True)
                    _sfd.SFInf.EditorUndo.AddNewUndo(_steinSymbolEnum, sv.StockSteinIdx, 0)
                End If
            End If
        End Sub

        '##########################################

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
        Public Sub DoGhostPositionForDoubleKlick()

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
                'Debug.Print("DoGhostPositionForDoubleKlick" & Now.ToString & " - " & mousePos.ToString)
                'mögliche Ablageposition auf dem Feld

                Dim st As SteinSymbol = _sfd.SFStock.GetSteinSymbolIndexDontRemove(index:=0)
                Dim steinGhost As SteinGhost = If(INI.Editor_ShowDoubleclickGhost, SteinGhost.Light, SteinGhost.Dark)
                Dim request As New TileRequest(_sfd.SFRun.AktRenderMode, INI.Tile_TileColors, st, SteinStatus.I01Normal, _sfd.SFLay.steinSize, INI.Tile_BasisSize, steinGhost)
                Dim bmp As Bitmap = TileFactory.GetTile(request)

                SetDoubleClickGhostBmp(bmp, _sfd.SFInf.GetSteinRenderRect(tpl), zEbene:=tpl.z)
            End If

        End Sub

#End Region

#Region "Undo und Redo Editor und Stock"
        Private Sub DoEditorUndo()
            _sfd.SFInf.SetUndoText(job:=UnReDoJob.EtoE)
        End Sub
        Private Sub DoEditorRedo()

        End Sub
#End Region

#Region "Spielfeld"

        Private Sub DoKlickSpiel(insideSpielfeld As Boolean)
            If insideSpielfeld Then
            Else
            End If
        End Sub

        Private Sub DoKlickHistorboxes()

        End Sub
        Private Sub DoRenderstepHistorboxes()

        End Sub

#End Region

#Region "Helfer"

        Private Function IsMouseMoved(aktMousePos As Point, lastMousePos As Point) As Boolean

            Debug.Print($"AktMousePos: {aktMousePos}, LastMousePos: {lastMousePos} NowMs: {Now.Millisecond}")

            If lastMousePos = Point.Empty Then
                Return True
            End If

            Const delta As Integer = 1

            If Math.Abs(aktMousePos.X - lastMousePos.X) > delta Then Return True
            If Math.Abs(aktMousePos.Y - lastMousePos.Y) > delta Then Return True

            Return False

        End Function

#End Region

    End Class
End Namespace