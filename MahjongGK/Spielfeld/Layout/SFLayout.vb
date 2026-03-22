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

Namespace Spielfeld

#Disable Warning IDE0079
#Disable Warning IDE1006

    'Auf dem Controll tummeln sich rund um das zentrale Spielfeld von MahjongGK mehrere Fenster.
    'Im Editiermodus wird das ganze verkleinert und um zusätzliche Fenster weitert. 
    'Mehrere RectangleX dienen dazu dieses Layout zu beschreiben. 
    'Es kann dann bleiben, solange das Control seine Größe nicht ändert oder AktRendering 
    'nicht geändert wird. 

    'Es gibt folgende Ausgabebereiche

    '    Public rectxOutput As RectangleX                    'Gesamte Ausgabefläche - Basis der Berechnungen
    'Public rectxStock As RectangleX                     'Oben die Leiste des Steinvorrat beim Editieren.
    'Public rectxStockScrollbar As RectangleX            'Scrollbar des Steinvorrat
    'Public rectxBitmapUgrd As RectangleX                'Hält die UGHrdBitmap. Beim Spielen so groß wie outputRect
    'Public rectxHeader As RectangleX                    'Optional. Hält die Überschrift
    'Public rectxContent As RectangleX                   'UGrdRect ohne Header
    'Public rectxHistoryBoxLeft As RectangleX            'Optional
    'Public rectxHistoryBoxRight As RectangleX           'Optional
    'Public rectxHistoryBoxLeftScrollbar As RectangleX   'Optional
    'Public rectxHistoryBoxRightScrollbar As RectangleX  'Optional
    'Public rectxStageAvailable As RectangleX            'Der für Spielfeld/Editor zur Verfügung stehende Platz
    ''und Basis für die Interation.
    'Public rectxStageUsed As RectangleX                 'Der von Spielfeld/Editor genutzte Platz.

    ''Für alle außer rectxOutput, rectxStageUsed und den Scrollbars gibt es Paddings
    'Public INI.Rendering_PadStock As PaddingValues						
    '    Public INI.Rendering_PadBitmapUgrd As PaddingValues				
    '    Public INI.Rendering_PadHeader As PaddingValues				
    '    Public INI.Rendering_PadContent As PaddingValues				
    '    Public INI.Rendering_PadHistoryBoxLeft As PaddingValues			
    '    Public INI.Rendering_PadHistoryBoxRight As PaddingValues			
    '    Public INI.Rendering_PadStageAvailable As PaddingValues			

    '    'Diese Felder sind optional:
    '     Public INI.Rendering_UsePadHeader As Boolean
    '     Public INI.Rendering_UsePadHistoryBoxLeft As Boolean			
    '     Public INI.Rendering_UsePadHistoryBoxRight As Boolean	

    '     'Debughilfe:
    '     'Public INI.Rendering_DrawDebugLines As Boolean

    ''' <summary> 
    ''' Pfad: MahjongGK/Spielfeld/Layout
    ''' 
    ''' Hält die globalen Layout- und Flächeninformationen des Spielfeldbereichs. 
    ''' 
    ''' SFLay beschreibt, wie der verfügbare Anzeigebereich aktuell räumlich aufgeteilt ist. 
    ''' Dazu gehören typischerweise: 
    ''' 
    ''' - der gesamte Ausgabebereich, 
    ''' - Header-, Content- und Hintergrundbereiche, 
    ''' - Vorratsbereich und dessen Scrollbar/Markierung, 
    ''' - History-Bereiche, 
    ''' - der verfügbare und der tatsächlich genutzte Spielfeldbereich, 
    ''' - die aktuell verwendete Layout-Variante. 
    ''' 
    ''' SFLay enthält keine Spielfeldlogik und keine eigentlichen Renderressourcen,
    ''' sondern nur geometrische und organisatorische Informationen über die Aufteilung 
    ''' der sichtbaren Flächen. 
    ''' 
    ''' Diese Klasse beschreibt also nicht das Spielfeld selbst, 
    ''' sondern den äußeren Rahmen, in dem es angezeigt wird. 
    ''' </summary> 
    Public Class SFLayout
        Implements IDisposable

        Public Sub New()

        End Sub

        Public Sub New(owner As SFDaten)
            _sfd = owner
        End Sub

        Private ReadOnly _sfd As SFDaten

#Region "Render - Steinmaße / 3D-Offsets"

        Private _steinSize As Size

        Public Property steinSize As Size
            Get
                Return _steinSize
            End Get
            Set(value As Size)
                _steinSize = value
            End Set
        End Property

        ''' <summary>
        ''' Aktuelle Breite der Steine (immer gerade Zahl).
        ''' </summary>
        Public ReadOnly Property steinWidth As Integer
            Get
                Return _steinSize.Width
            End Get
        End Property

        ''' <summary>
        ''' Aktuelle Höhe der Steine (immer gerade Zahl).
        ''' </summary>
        Public ReadOnly Property steinHeight As Integer
            Get
                Return _steinSize.Height
            End Get
        End Property

        ''' <summary>
        ''' Halbe Breite der Steine.
        ''' </summary>
        Public ReadOnly Property steinWidthHalf As Integer
            Get
                Return _steinSize.Width \ 2
            End Get
        End Property

        ''' <summary>
        ''' Halbe Höhe der Steine.
        ''' </summary>
        Public ReadOnly Property steinHeightHalf As Integer
            Get
                Return _steinSize.Height \ 2
            End Get
        End Property

        Public Property offset3DLeftJeEbene As Integer
        Public Property offset3DTopJeEbene As Integer

        Public Property offset3DLeftSumme As Integer
        Public Property offset3DTopSumme As Integer

        Public Property steinWidthLastCreated As Integer
        Public Property steinHeightLastCreated As Integer

#End Region

#Region "Layout - globale Ausgabebereiche"

        ''' <summary>
        ''' Gesamte Ausgabefläche wie vom Paint.
        ''' </summary>
        Public Property rxOutput As RectangleX = Nothing

        ''' <summary>
        ''' Oben die Leiste des Steinvorrats beim Editieren.
        ''' </summary>
        Public Property rxStock As RectangleX = Nothing

        ''' <summary>
        ''' Scrollbar des Steinvorrats.
        ''' </summary>
        Public Property rxStockScrollbar As RectangleX = Nothing

        ''' <summary>
        ''' Markierung des Steinvorrats.
        ''' </summary>
        Public Property rxStockMark As RectangleX = Nothing

        ''' <summary>
        ''' Hält die UGrd-Bitmap.
        ''' Beim Spielen so groß wie rxOutput,
        ''' im Editor rxStageAvailable,
        ''' in der Werkbank wie rxOutput.
        ''' </summary>
        Public Property rxBitmapUgrd As RectangleX = Nothing

        ''' <summary>
        ''' Hält die Überschrift.
        ''' </summary>
        Public Property rxHeader As RectangleX = Nothing

        ''' <summary>
        ''' UGrd-Rechteck ohne Header.
        ''' </summary>
        Public Property rxContent As RectangleX = Nothing

        ''' <summary>
        ''' Optional innerhalb des linken History-Containers.
        ''' </summary>
        Public Property rxHistoryBoxLeft As RectangleX = Nothing

        ''' <summary>
        ''' Optional innerhalb des rechten History-Containers.
        ''' </summary>
        Public Property rxHistoryBoxRight As RectangleX = Nothing

        Public Property rxHistoryBoxLeftScrollbar As RectangleX = Nothing
        Public Property rxHistoryBoxRightScrollbar As RectangleX = Nothing

        Public Property rxUndo As RectangleX = Nothing
        Public Property rxRedo As RectangleX = Nothing

        Private Property BitmapUGrdFingerprint As Spielfeld.BitmapFingerprint

        Private _bitmapsUndoRedo(1, 3) As Bitmap 'Ist automatisch mit Nothigg initialisiert

        Public Enum UndoRedoBmp
            Undo
            Redo
        End Enum

        Public Enum UndoRedoMode
            Normal
            MouseOver
            MouseDown
            Selected
        End Enum

        Public Property BitmapUnRe(d0 As UndoRedoBmp, d1 As UndoRedoMode) As Bitmap
            Get
                Return _bitmapsUndoRedo(d0, d1)
            End Get
            Set(value As Bitmap)
                If _bitmapsUndoRedo(d0, d1) IsNot Nothing Then
                    _bitmapsUndoRedo(d0, d1).Dispose()
                    _bitmapsUndoRedo(d0, d1) = Nothing 'hier nich unbedingt nötig, aber sauberer.
                    '1.) muss die Bitmap hier auch noch auf Nothing gesetzt werden? 
                End If
                _bitmapsUndoRedo(d0, d1) = value
            End Set
        End Property

        '2.) Die Eigenschaft ist in SFLayout angesiedelt.
        'Was muss die Klasse bereitstellen?
        'Die letze _bitmapUndo bleibt im Speicher. 
        'Nach jetzigem Planungsstand ist dann aber Programmende.
        'Doch lieber so programmieren, daß es keine Seiteneffekte gibt, wenn sich das ändert.

        ''' <summary>
        ''' Der für Spielfeld/Editor verfügbare Platz zur Berechnung der Steingröße.
        ''' Wird zum Rendern nicht direkt benötigt, sondern als Rechenbasis.
        ''' </summary>
        Public Property rxStageAvailable As RectangleX = Nothing

        ''' <summary>
        ''' Der aktuell vom Spielfeld/Editor tatsächlich genutzte Bereich.
        ''' </summary>
        Public Property rxStageUsed As RectangleX = Nothing

        ''' <summary>
        ''' Die aktuelle Aufteilung des Anzeigebereichs.
        ''' </summary>
        Public Property AktLayout As Layout

#End Region

        Public Sub UpdateSpielfeldLayout(outputRect As Rectangle, aktRenderModeChanged As Boolean, Optional forceUpdate As Boolean = False)

            Dim saveToTmpVerz As Boolean = False

            'Auf eine Änderung von AktRendering reagieren
            Dim changed As Boolean = False
            'True: es gibt ein gültiges Rect, d.h. es wurde bereits gerendert
            Dim createFrozenBitmap As Boolean = (rxOutput IsNot Nothing AndAlso Not rxOutput.IsEmpty)

            If aktRenderModeChanged Then
                changed = True
            End If
            '
            '
            'Die Zuweisungen RectangleX = Rectangle sind erlaubt 
            'wegen des Widening Operator in RectangleX, der automatisch
            'die Konvertierung vornimmt.
            If changed Or forceUpdate Then
                rxOutput = outputRect
            Else
                If rxOutput = outputRect Then
                    Exit Sub
                Else
                    rxOutput = outputRect
                End If
            End If

            If _sfd.SFInf.IsEmpty Then
                Exit Sub
            End If

            If changed AndAlso _sfd.SFRun.AktRenderMode <> AktRenderMode.None Then
                ' Prüfen, ob gespeichert werden soll ins Temporär-Verzeichnis.
                ' Annahme: _sfd.AktSpielfeldInfo.Ident ist die "neu beobachtete" ID.
                Dim identNew As String = _sfd.SFInf.PersistentIdent

                If String.IsNullOrEmpty(_sfd.SFRun.AktSpielfeldInfo_Ident) Then
                    ' Erstes Mal: Akt noch leer -> einmalig speichern
                    _sfd.SFRun.AktSpielfeldInfo_Ident = identNew
                    ' Last bleibt Nothing/leer
                    saveToTmpVerz = True

                ElseIf identNew = _sfd.SFRun.AktSpielfeldInfo_Ident Then
                    ' Keine Änderung
                    saveToTmpVerz = False

                ElseIf identNew = _sfd.SFRun.LastSpielfeldInfo_Ident Then
                    ' Sonderfall: Hin- und Her zwischen zwei bekannten IDs
                    ' (Aktuelle Daten von Spielfeld/Editor und Werkbank)
                    ' -> NICHT speichern, nur die Historie spiegeln/aktualisieren
                    Dim tmp As String = _sfd.SFRun.AktSpielfeldInfo_Ident
                    _sfd.SFRun.AktSpielfeldInfo_Ident = _sfd.SFRun.LastSpielfeldInfo_Ident
                    _sfd.SFRun.LastSpielfeldInfo_Ident = tmp
                    saveToTmpVerz = False

                Else
                    ' ECHTER Wechsel auf eine neue, bisher unbekannte ID
                    _sfd.SFRun.LastSpielfeldInfo_Ident = _sfd.SFRun.AktSpielfeldInfo_Ident
                    _sfd.SFRun.AktSpielfeldInfo_Ident = identNew
                    saveToTmpVerz = True
                End If
            End If

            INI.RaiseAllIniEvents() 'ZLV ?

            If _sfd.SFRun.AktRenderMode = AktRenderMode.Spiel Then
                'Es ist übersichtlicher die einzelnen Layouts jeweils komplett gesondert zu
                'bearbeiten und nicht nach Gemeinsamkeiten zu sortieren.
                'spätestens bei Änderungen geht es einfacher.
                'Daher zunächst, ganz übersichtlich und kurz:
                '
                Select Case INI.Spielbetrieb_PositionHistory
                    Case 0
                        AktLayout = Layout.SplfldWithHeaderAndHistNone
                    Case 2
                        AktLayout = Layout.SplfldWithHeaderAndHistRight
                    Case Else
                        AktLayout = Layout.SplfldWithHeaderAndHistLeft
                End Select

            ElseIf _sfd.SFRun.AktRenderMode = AktRenderMode.Edit Then
                AktLayout = Layout.EditorWithHeader

                'Initialisierung den Spielsteingenerators
                With _sfd.SFInf
                    If IsNothing(.Generator) Then
                        If IsNothing(.GeneratorValuesForXml) Then
                            'TODO SFD-Anpassung so umbauen, daß der Default nur genommen wird, wenn nichts Anderes festgelegt wurde
                            If INI.SpielsteinGenerator_GeneratorModusDefault = GeneratorModus.None Then
                                .Generator = New SpielsteinGenerator(INI.SpielsteinGenerator_GeneratorModusDefault)
                            Else
                                .Generator = New SpielsteinGenerator(INI.SpielsteinGenerator_GeneratorModusDefault)
                            End If
                        Else
                            .Generator = New SpielsteinGenerator
                            'Das sind die bisherigen Werte, die beim Speichern von Spielfeldinfo gespeichert werden.
                            .GeneratorValuesForXml.CopySpielsteinGeneratorValues_To_SpielsteinGenerator(.Generator)
                        End If
                    End If
                End With

            Else ' _sfd.sflay.AktRendering = RenderingEnum.None Then
                AktLayout = Layout.None
            End If

            'Die Steinabmessungen werden interativ berechnet.
            'Sie sind abhängig von der Feldgröße in Steinen, der Feldgröße in Pixeln
            'und der Feldhöhe in Pixelverschiebungen je Stein und Ebene und ob oben die Vorratssteine sind.
            'Zusätzlich fließen noch die Paddings rings um das Ausgaberechteck in die Berechnung ein.
            '
            'Startwerte:
            steinSize = New Size(INI.Images_OrgGrafikSizeWidth, INI.Images_OrgGrafikSizeHeight)
            CreateMainLayoutIterativStep() 'Berechnung des Layouts mit dieser Steingröße.
            'Daraus ergibt sich eine neue Steingröße.
            'die "+ 2" ergeben sich aus: Wert aufrunden + 1.
            steinSize = New Size(rxStageAvailable.Width \ _sfd.SFInf.xMaxSteine + 2, rxStageAvailable.Height \ _sfd.SFInf.yMaxSteine + 2)

            '
            Dim summeWidth As Integer
            Dim summeHeight As Integer

            Dim steinSizeIteration As Size
            Dim shrink As Integer = -1
            '
            'Die steinSize wird jetzt so lange proportional schrittweise verkleinert, bis es passt.
            Do
                shrink += 1

                steinSizeIteration = GetNewSteinSize(shrink)

                offset3DLeftJeEbene = CInt(Math.Max(INI.Rendering_Offset3DFaktorAbsolutX * steinSizeIteration.Width, CDbl(INI.Rendering_Offset3DMinPerLayerX)))
                offset3DTopJeEbene = CInt(Math.Max(INI.Rendering_Offset3DFaktorAbsolutY * steinSizeIteration.Height, CDbl(INI.Rendering_Offset3DMinPerLayerY)))

                offset3DLeftSumme = offset3DLeftJeEbene * _sfd.SFInf.zMax
                offset3DTopSumme = offset3DTopJeEbene * _sfd.SFInf.zMax

                summeWidth = steinSizeIteration.Width * _sfd.SFInf.xMaxSteine + offset3DLeftSumme
                summeHeight = steinSizeIteration.Height * _sfd.SFInf.yMaxSteine + offset3DTopSumme

                steinSize = New Size(steinSizeIteration.Width, steinSizeIteration.Height)

                '_sfd.rxStageAvailable wird hier berechnet in Abhängigkeit von _sfd.steinSize und aller anderen Variablen
                CreateMainLayoutIterativStep()

                If summeWidth <= rxStageAvailable.WidthInside AndAlso summeHeight <= rxStageAvailable.HeightInside Then
                    Exit Do
                End If
            Loop

            'Zusätzlicher Aufruf außerhalb der Schreife zum Debuggen:
            'CreateMainLayoutIterativStep(fDoStop:=True) 

            'getestet mit einem Spielfeld mit 75 X 25 X 25 Steinen
            'und einem RenderRect mit 344 x 194 Pixeln
            'und einem MJ_SPIELFELD_MIN_WIDTH MJ_SPIELFELD_MIN_HEIGHT von 600 x 400 Pixeln.
            'Das läuft.
            ''Auf das Spielfeld passen 75 x 25 x 25 = 46,875 Steine (und der Computer schafft die Anzeige.) 
            '
            rxStageUsed = rxStageAvailable.GetRectangleXInside(summeWidth, summeHeight, align:=RectangleX.Align.Center, usePadding:=True)

            rxStageUsed?.SetDrawBoundsAndContentDebug(Style)
            ' Initialisierung der RectangleX-Felder/Eigenschaften mit Debug-Namen
            ' Nur für Debug/Diagnose
            RectangleXDebugBinder.BindNames(_sfd.SFLay)

            If steinWidthLastCreated <> steinWidth OrElse steinHeightLastCreated <> steinHeight Then
                ' BitmapContainer.ChangeImagesSize(steinWidth, steinHeight)

                steinWidthLastCreated = steinWidth
                steinHeightLastCreated = steinHeight
            End If

            offset3DLeftJeEbene *= INI.Rendering_Offset3DFaktorSignX
            offset3DTopJeEbene *= INI.Rendering_Offset3DFaktorSignY

            If offset3DLeftJeEbene < 0 Then
                offset3DLeftSumme = 0
            End If
            If offset3DTopJeEbene < 0 Then
                offset3DTopSumme = 0
            End If

            _sfd.SFInf.Update_SteinInfos_RectQuadranten()

            'With _sfd.AktSpielfeldInfo
            '    If IsNothing(.BitmapUGrdImgCache) Then
            '        If .HasBitmapUGrd Then
            '            .BitmapUGrdImgCache = New BackgroundSingleImageCache(BackgroundBitmapCache)
            '            .BitmapUGrdImgCache.LoadBitmap(.BitmapUGrdFullpath, BackgroundSingleImageCache.BackgroundRenderMode.CoverCrop)
            '        Else
            '            With _sfd.AktSpielfeldInfo
            '                Dim bitmapUGrdFullpath As String = IO.Path.Combine(AppDataDirectory(AppDataSubDir.Hintergrundgrafiken),
            '                If(INI.Global_DarkMode, "water_3007467.jpg", "watercolor_2323195.jpg"))
            '                .BitmapUGrdFullpath = bitmapUGrdFullpath
            '                .BitmapUGrdImgCache = New BackgroundSingleImageCache(BackgroundBitmapCache)
            '                .BitmapUGrdImgCache.LoadBitmap(bitmapUGrdFullpath, BackgroundSingleImageCache.BackgroundRenderMode.PreserveOrgSize)
            '            End With
            '        End If
            '    End If
            'End With

            Select Case AktLayout
                Case Layout.None
                    _sfd.SFRun.HScrollBarStock = Nothing
                Case Layout.SplfldWithHeaderAndHistNone
                    _sfd.SFRun.HScrollBarStock = Nothing
                Case Layout.SplfldWithHeaderAndHistRight
                    _sfd.SFRun.HScrollBarStock = Nothing
                Case Layout.SplfldWithHeaderAndHistLeft
                    _sfd.SFRun.HScrollBarStock = Nothing
                Case Layout.EditorWithHeader

                    If IsNothing(_sfd.SFRun.HScrollBarStock) Then
                        _sfd.SFRun.HScrollBarStock = New HScrollRenderer(rxStockScrollbar.ToRectangle, INI.Editor_HScrollbarColor)
                    Else
                        _sfd.SFRun.HScrollBarStock.RectHScrollbar = rxStockScrollbar.ToRectangle
                    End If
                    If IsNothing(_sfd.SFRun.EditorStockValues) Then
                        _sfd.SFRun.EditorStockValues = New SFStockState(_sfd)
                    End If

                    With _sfd.SFRun.EditorStockValues
                        If .LastSteinWidth <> steinWidth OrElse
                            .LastRxStockWidth <> rxStockScrollbar.WidthInside Then

                            .LastSteinWidth = steinWidth
                            .LastRxStockWidth = rxStockScrollbar.WidthInside
                            .ClearAndSetStartvalues()
                            .UpdateSteineVisibleMaxCount()
                        End If
                    End With
                Case Else
                    Stop 'Programmierfehler: Enum hinzugekommen
            End Select

            If _sfd.SFRun.AktRenderMode = AktRenderMode.Edit Then
                If INI.Editor_ShowFrmSteinStackInfo Then
                    If IsNothing(_sfd.SFRun.EditorFrmSteinStackInfo) Then
                        _sfd.SFRun.EditorFrmSteinStackInfo = New frmSteinStackInfo(frmMain, offsetRight:=18, offsetUp:=12)
                    End If
                End If
            Else
                If _sfd.SFRun.EditorFrmSteinStackInfo IsNot Nothing Then
                    _sfd.SFRun.EditorFrmSteinStackInfo.Dispose()
                    _sfd.SFRun.EditorFrmSteinStackInfo = Nothing
                End If
            End If

            DisposeUndoRedoBitmaps()

            'Prüfen, ob sich der Hintergrund geändert hat?
            Dim createNew As Boolean = True

            If (rxUndo IsNot Nothing OrElse rxRedo IsNot Nothing) AndAlso createNew Then

                Dim UndoRedoPalette As OverlayColorPalette

                If Not IsNothing(_sfd.SFLay.rxBitmapUgrd) Then
                    If _sfd.SFInf.HasBitmapUGrd Then
                        UndoRedoPalette = New OverlayColorPalette(_sfd.SFInf.BitmapUGrdSingleImgCache.GetBitmap(rxBitmapUgrd.Size))
                    Else
                        UndoRedoPalette = New OverlayColorPalette(_sfd.SFInf.HGrdSplFldColor)
                    End If
                Else
                    UndoRedoPalette = New OverlayColorPalette(INI.Rendering_BackgroundColor)
                End If

                Dim bmpUndo As Bitmap = Images.UndoRedoBitmapFactory.CreateUndoRedoBitmap(Images.UndoRedoGlyph.Undo, UndoRedoPalette.ColorNormal, rxUndo.Size.Width)
                Dim bmpRedo As Bitmap = Images.UndoRedoBitmapFactory.CreateUndoRedoBitmap(Images.UndoRedoGlyph.Redo, UndoRedoPalette.ColorNormal, rxUndo.Size.Width)

                BitmapUnRe(UndoRedoBmp.Undo, UndoRedoMode.Normal) = bmpUndo
                BitmapUnRe(UndoRedoBmp.Redo, UndoRedoMode.Normal) = bmpRedo

                BitmapUnRe(UndoRedoBmp.Undo, UndoRedoMode.MouseOver) = Spielfeld.RenderHelper.DrawOverlay_Außenrahmen3D(bmpUndo, rahmenbreite:=2, UndoRedoPalette.ColorMouseOver, copyBitmap:=True)
                BitmapUnRe(UndoRedoBmp.Redo, UndoRedoMode.MouseOver) = Spielfeld.RenderHelper.DrawOverlay_Außenrahmen3D(bmpRedo, rahmenbreite:=2, UndoRedoPalette.ColorMouseOver, copyBitmap:=True)

                BitmapUnRe(UndoRedoBmp.Undo, UndoRedoMode.MouseDown) = Spielfeld.RenderHelper.DrawOverlay_Außenrahmen3D(bmpUndo, rahmenbreite:=2, UndoRedoPalette.ColorMouseDown, copyBitmap:=True)
                BitmapUnRe(UndoRedoBmp.Redo, UndoRedoMode.MouseDown) = Spielfeld.RenderHelper.DrawOverlay_Außenrahmen3D(bmpRedo, rahmenbreite:=2, UndoRedoPalette.ColorMouseDown, copyBitmap:=True)

                BitmapUnRe(UndoRedoBmp.Undo, UndoRedoMode.Selected) = Spielfeld.RenderHelper.DrawOverlay_Außenrahmen3D(bmpUndo, rahmenbreite:=2, UndoRedoPalette.ColorSelected, copyBitmap:=True)
                BitmapUnRe(UndoRedoBmp.Redo, UndoRedoMode.Selected) = Spielfeld.RenderHelper.DrawOverlay_Außenrahmen3D(bmpRedo, rahmenbreite:=2, UndoRedoPalette.ColorSelected, copyBitmap:=True)

            End If

            If saveToTmpVerz Then
                _sfd.SFInf.SaveLastUsedSpielfeld()
            End If
        End Sub

        ''' <summary>
        ''' Liefert eine neue Bitmap-Größe ausgehend von Original/Referenzmaßen,
        ''' indem vom längeren Maß der Wert <paramref name="shrink"/> abgezogen wird.
        ''' Das Seitenverhältnis bleibt erhalten. Ergebnisse werden auf die nächstkleinere
        ''' gerade Integerzahl abgerundet. Unterschreitet eine Seite das Mindestmaß,
        ''' werden **beide** Seiten auf das Mindestmaß gesetzt.
        ''' </summary>
        Public Function GetNewSteinSize(shrink As Integer) As Size
            ' --- 1) Ausgangsmaße bestimmen (Double) ------------------------------------------
            '     a) Falls UseGrafikOrgSize = True -> Originalmaße verwenden
            '     b) Sonst Referenzmaße; wenn eine Seite < SRC_MIN, dann auf Original ausweichen
            '     c) Beide Seiten am Ende auf SRC_MAX deckeln

            Dim baseW As Double
            Dim baseH As Double

            If INI.Images_UseGrafikOrgSize Then
                baseW = CDbl(INI.Images_OrgGrafikSizeWidth)
                baseH = CDbl(INI.Images_OrgGrafikSizeHeight)
            Else
                Dim refW As Integer = INI.Images_OrgGrafikReferenceSizeWidth
                Dim refH As Integer = INI.Images_OrgGrafikReferenceSizeHeight

                If refW < MJ_GRAFIK_SRC_MIN_WIDTH_OR_HEIGHT OrElse refH < MJ_GRAFIK_SRC_MIN_WIDTH_OR_HEIGHT Then
                    ' Referenz zu klein -> Original nehmen
                    baseW = CDbl(INI.Images_OrgGrafikSizeWidth)
                    baseH = CDbl(INI.Images_OrgGrafikSizeHeight)
                ElseIf refW > MJ_GRAFIK_SRC_MAX_WIDTH_OR_HEIGHT OrElse refH > MJ_GRAFIK_SRC_MAX_WIDTH_OR_HEIGHT Then
                    ' Referenz zu groß -> Original nehmen
                    baseW = CDbl(INI.Images_OrgGrafikSizeWidth)
                    baseH = CDbl(INI.Images_OrgGrafikSizeHeight)
                Else
                    baseW = CDbl(refW)
                    baseH = CDbl(refH)
                End If
            End If

            ' --- 2) Seitenverhältnis und längere Seite bestimmen ----------------------------
            '      Hinweis: baseW/baseH sind jetzt valide (>= SRC_MIN, <= SRC_MAX) oder wie Original
            Dim ratio As Double ' width / height
            If baseH > 0 Then
                ratio = baseW / baseH
            Else
                ' Sicherheitsnetz; sollte praktisch nie auftreten
                ratio = 1.0R
            End If

            Dim widthIsLonger As Boolean = (baseW >= baseH)
            Dim longSide As Double = If(widthIsLonger, baseW, baseH)

            ' --- 3) Ziel-Längenseite um shrink verkleinern ----------------------------------
            '      (Kann theoretisch <= 0 werden; wird durch Mindestmaß später abgefangen.)
            Dim targetLong As Double = longSide - CDbl(shrink)
            If targetLong < 0 Then targetLong = 0

            ' --- 4) Zweite Seite proportional berechnen -------------------------------------
            Dim targetW As Double
            Dim targetH As Double

            If widthIsLonger Then
                ' Breite war die längere Seite
                targetW = targetLong
                ' H = W / (W/H) = W / ratio
                If ratio > 0 Then
                    targetH = targetW / ratio
                Else
                    targetH = targetW ' Fallback
                End If
            Else
                ' Höhe war die längere Seite
                targetH = targetLong
                ' W = H * (W/H) = H * ratio
                targetW = targetH * ratio
            End If

            ' --- 5) Auf nächste gerade Integerzahl ABRUNDEN ---------------------------------
            Dim iW As Integer = FloorToEvenInt(targetW)
            Dim iH As Integer = FloorToEvenInt(targetH)

            ' --- 6) Mindestmaß-Prüfung -------------------------------------------------------
            ' Wenn eine Seite unter das Mindestmaß fällt, beide auf Mindestmaß setzen.
            If iW < MJ_GRAFIK_STEIN_MIN_WIDTH_OR_HEIGHT OrElse iH < MJ_GRAFIK_STEIN_MIN_WIDTH_OR_HEIGHT Then
                iW = MJ_GRAFIK_STEIN_MIN_WIDTH_OR_HEIGHT
                iH = MJ_GRAFIK_STEIN_MIN_WIDTH_OR_HEIGHT
            End If

            Return New Size(iW, iH)

        End Function

        ''' <summary>
        ''' Rundet einen Double-Wert ab und liefert die nächstkleinere gerade Integerzahl.
        ''' Beispiel: 101,9 -> 100; 100,0 -> 100; 2,0 -> 2; 1,2 -> 0.
        ''' </summary>
        Private Function FloorToEvenInt(value As Double) As Integer
            Dim n As Integer = CInt(Math.Floor(value))
            If (n And 1) <> 0 Then
                n -= 1 ' auf die nächstkleinere gerade Zahl
            End If
            Return n
        End Function

        ''' <summary>
        ''' Setzt die einzelnen Bereiche, außer splfldUsedRect
        ''' </summary>
        Private Sub CreateMainLayoutIterativStep(Optional fDoStop As Boolean = False)

            ' 1.) _sfd.rxOutput ist gegeben.
            ' 2.) Der Renderer fragt _sfd.sflay.AktRendering nicht ab. 
            '     Er fragt ab, ob ein rx... Nothing ist, deshalb sind alle nicht benötigten rx.. Nothing zu setzen.
            '     daher zunächst:

            rxStock = Nothing
            rxStockScrollbar = Nothing
            rxStockMark = Nothing
            rxBitmapUgrd = Nothing
            rxHeader = Nothing
            rxContent = Nothing
            rxHistoryBoxLeft = Nothing
            rxHistoryBoxRight = Nothing
            rxHistoryBoxLeftScrollbar = Nothing
            rxHistoryBoxRightScrollbar = Nothing
            rxStageAvailable = Nothing
            rxStageUsed = Nothing
            rxUndo = Nothing
            rxRedo = Nothing
            '
            '
            ' Überschreibung zum Debuggen
            '_sfd.AktLayout = Layout.SplfldWithHeaderAndHistNone
            '
            '
            'Auf gehts,
            Dim histboxContainerWidth As Integer = 2 * steinWidth +
                INI.Rendering_HScrollbarHeightVScrollbarWitdh

            Const UNDOW As Integer = 32
            Const UNDOH As Integer = 32

            Const UNDOMARGIN As Integer = 10
            Const UNDOMARGINRIGHT As Integer = 32

            Select Case AktLayout
                Case Layout.SplfldWithHeaderAndHistNone
                    rxBitmapUgrd = New RectangleX(rxOutput)
                    rxHeader = rxBitmapUgrd.GetRectangleXInside(width:=-1, INI.Rendering_HeaderHeight, align:=RectangleX.Align.Top)
                    rxContent = rxBitmapUgrd.GetRectangleXInside(width:=-1, height:=-1, align:=RectangleX.Align.Center)
                    rxContent.IncTop(rxHeader.Height + 1)

                    rxStageAvailable = rxContent.GetRectangleXInside(width:=-1, height:=-1, align:=RectangleX.Align.Left, usePadding:=True, INI.Rendering_PaddingStageAvailable)
                    If INI.Spielbetrieb_UseUnDoReDoSpielfeld Then
                        rxUndo = New RectangleX
                        rxUndo.Width = UNDOW
                        rxUndo.Height = UNDOH
                        rxUndo.Left = rxContent.Right - UNDOW - UNDOMARGIN - UNDOW - UNDOMARGINRIGHT
                        rxUndo.Top = rxContent.Bottom - UNDOH - UNDOMARGIN
                        '
                        rxRedo = New RectangleX
                        rxRedo.Width = UNDOW
                        rxRedo.Height = UNDOH
                        rxRedo.Left = rxContent.Right - UNDOW - UNDOMARGINRIGHT
                        rxRedo.Top = rxContent.Bottom - UNDOH - UNDOMARGIN
                    End If
                Case Layout.SplfldWithHeaderAndHistLeft
                    rxBitmapUgrd = New RectangleX(rxOutput)
                    rxHeader = rxBitmapUgrd.GetRectangleXInside(width:=-1, INI.Rendering_HeaderHeight, align:=RectangleX.Align.Top)
                    rxContent = rxBitmapUgrd.GetRectangleXInside(width:=-1, height:=-1, align:=RectangleX.Align.Center)
                    rxContent.IncTop(rxHeader.Height + 1)

                    rxHistoryBoxLeft = rxContent.GetRectangleXInside(steinWidth * 2, height:=-1, align:=RectangleX.Align.Left)
                    rxHistoryBoxLeftScrollbar = rxContent.GetRectangleXInside(INI.Rendering_HScrollbarHeightVScrollbarWitdh, height:=-1, align:=RectangleX.Align.Left)
                    rxHistoryBoxLeftScrollbar.IncLeft(steinWidth * 2 + 1)

                    rxStageAvailable = rxContent.GetRectangleXInside(width:=-1, height:=-1, align:=RectangleX.Align.Left, usePadding:=True, INI.Rendering_PaddingStageAvailable)
                    rxStageAvailable.IncLeft(steinWidth * 2 + INI.Rendering_HScrollbarHeightVScrollbarWitdh + 2)

                    If INI.Spielbetrieb_UseUnDoReDoSpielfeld Then
                        rxUndo = New RectangleX
                        rxUndo.Width = UNDOW
                        rxUndo.Height = UNDOH
                        rxUndo.Left = rxContent.Right - UNDOW - UNDOMARGIN - UNDOH - UNDOMARGINRIGHT
                        rxUndo.Top = rxContent.Bottom - UNDOH - UNDOMARGIN
                        '
                        rxRedo = New RectangleX
                        rxRedo.Width = UNDOW
                        rxRedo.Height = UNDOH
                        rxRedo.Left = rxContent.Right - UNDOW - UNDOMARGINRIGHT
                        rxRedo.Top = rxContent.Bottom - UNDOH - UNDOMARGIN
                    End If
                Case Layout.SplfldWithHeaderAndHistRight
                    rxBitmapUgrd = New RectangleX(rxOutput)
                    rxHeader = rxBitmapUgrd.GetRectangleXInside(width:=-1, INI.Rendering_HeaderHeight, align:=RectangleX.Align.Top)
                    rxContent = rxBitmapUgrd.GetRectangleXInside(width:=-1, height:=-1, align:=RectangleX.Align.Center)
                    rxContent.IncTop(rxHeader.Height + 1)

                    rxHistoryBoxRightScrollbar = rxContent.GetRectangleXInside(INI.Rendering_HScrollbarHeightVScrollbarWitdh, height:=-1, align:=RectangleX.Align.Right)
                    rxHistoryBoxRight = rxContent.GetRectangleXInside(steinWidth * 2, height:=-1, align:=RectangleX.Align.Right)
                    rxHistoryBoxRight.MoveLeft(INI.Rendering_HScrollbarHeightVScrollbarWitdh + 1)

                    rxStageAvailable = rxContent.GetRectangleXInside(width:=-1, height:=-1, align:=RectangleX.Align.Right, usePadding:=True, INI.Rendering_PaddingStageAvailable)
                    rxStageAvailable.DecRight(steinWidth * 2 + INI.Rendering_HScrollbarHeightVScrollbarWitdh + 2)

                    If INI.Spielbetrieb_UseUnDoReDoSpielfeld Then
                        rxUndo = New RectangleX
                        rxUndo.Width = UNDOW
                        rxUndo.Height = UNDOH
                        rxUndo.Left = rxContent.Left + UNDOMARGIN
                        rxUndo.Top = rxContent.Bottom - UNDOH - UNDOMARGIN
                        '
                        rxRedo = New RectangleX
                        rxRedo.Width = UNDOW
                        rxRedo.Height = UNDOH
                        rxRedo.Left = rxContent.Left + UNDOMARGIN + UNDOW + UNDOMARGIN
                        rxRedo.Top = rxContent.Bottom - UNDOH - UNDOMARGIN
                    End If

                Case Layout.EditorWithHeader

                    'If fDoStop Then Stop

                    rxBitmapUgrd = New RectangleX(rxOutput)
                    rxHeader = rxBitmapUgrd.GetRectangleXInside(width:=-1, INI.Rendering_HeaderHeight, align:=RectangleX.Align.Top)
                    rxContent = rxBitmapUgrd.GetRectangleXInside(width:=-1, height:=-1, align:=RectangleX.Align.Center)
                    rxContent.IncTop(rxHeader.Height + 1)

                    rxStockMark = rxContent.GetRectangleXInside(width:=-1, height:=INI.Rendering_StockMarkHeight, align:=RectangleX.Align.Top)

                    rxStock = rxContent.GetRectangleXInside(width:=-1, height:=steinHeight, align:=RectangleX.Align.Top)
                    rxStock.MoveDown(INI.Rendering_StockMarkHeight + 1)

                    rxStockScrollbar = rxContent.GetRectangleXInside(width:=-1, height:=INI.Rendering_HScrollbarHeightVScrollbarWitdh, align:=RectangleX.Align.Top)
                    rxStockScrollbar.MoveDown(INI.Rendering_StockMarkHeight + steinHeight + 2)

                    rxStageAvailable = rxContent.GetRectangleXInside(width:=-1, height:=-1, align:=RectangleX.Align.Top, usePadding:=True, INI.Rendering_PaddingStageAvailable)
                    rxStageAvailable.IncTop(INI.Rendering_StockMarkHeight + steinHeight + INI.Rendering_HScrollbarHeightVScrollbarWitdh + 3)
                    ' rxStageAvailable.MoveDown(200)

                    If INI.Spielbetrieb_UseUnDoReDoSpielfeld Then
                        rxUndo = New RectangleX
                        rxUndo.Width = UNDOW
                        rxUndo.Height = UNDOH
                        rxUndo.Left = rxContent.Right - UNDOW - UNDOMARGIN - UNDOW - UNDOMARGINRIGHT
                        rxUndo.Top = rxContent.Bottom - UNDOH - UNDOMARGIN
                        '
                        rxRedo = New RectangleX
                        rxRedo.Width = UNDOW
                        rxRedo.Height = UNDOH
                        rxRedo.Left = rxContent.Right - UNDOW - UNDOMARGINRIGHT
                        rxRedo.Top = rxContent.Bottom - UNDOH - UNDOMARGIN
                    End If
                Case Else
                    Stop
            End Select

            If INI.Rendering_DrawRenderRect Then
                rxBitmapUgrd = Nothing
                '
                rxOutput?.SetDrawBoundsAndContentDebug(Style(reset:=True))
                rxStock?.SetDrawBoundsAndContentDebug(Style)
                rxStockScrollbar?.SetDrawBoundsAndContentDebug(Style)
                rxStockMark?.SetDrawBoundsAndContentDebug(Style)
                rxBitmapUgrd?.SetDrawBoundsAndContentDebug(Style)
                rxHeader?.SetDrawBoundsAndContentDebug(Style)
                rxContent?.SetDrawBoundsAndContentDebug(Style)
                rxHistoryBoxLeft?.SetDrawBoundsAndContentDebug(Style)
                rxHistoryBoxRight?.SetDrawBoundsAndContentDebug(Style)
                rxHistoryBoxLeftScrollbar?.SetDrawBoundsAndContentDebug(Style)
                rxHistoryBoxRightScrollbar?.SetDrawBoundsAndContentDebug(Style)
                rxStageAvailable?.SetDrawBoundsAndContentDebug(Style)
                rxUndo?.SetDrawBoundsAndContentDebug(Style)
                rxRedo?.SetDrawBoundsAndContentDebug(Style)

            End If

            If rxStageAvailable Is Nothing Then
                Stop 'Programmierfehler
                '_sfd.rxStageAvailable muß zugewiesen werden
            End If

        End Sub

        Private Function Style(Optional reset As Boolean = False) As Integer
            'monotoner Vorwärtszähler, der die verschiedenen Linienvarianten
            'der DrawDebug-Linien und Farben in RectangleX verteilt.
            Static value As Integer = -1
            If reset Then value = -1
            value += 1
            Return value
        End Function

        Private Sub DisposeUndoRedoBitmaps()

            For d1 As Integer = UndoRedoBmp.Undo To UndoRedoBmp.Redo
                For d2 As Integer = UndoRedoMode.Normal To UndoRedoMode.Selected
                    If _bitmapsUndoRedo(d1, d2) IsNot Nothing Then
                        _bitmapsUndoRedo(d1, d2).Dispose()
                        _bitmapsUndoRedo(d1, d2) = Nothing
                    End If
                Next
            Next
        End Sub

        Private _disposed As Boolean
        Protected Overridable Sub Dispose(disposing As Boolean)

            If Not _disposed Then

                If disposing Then
                    DisposeUndoRedoBitmaps()
                End If

                _disposed = True

            End If

        End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub

    End Class

End Namespace