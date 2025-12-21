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
'
Imports System.Drawing.Imaging

#Disable Warning IDE0079
#Disable Warning IDE1006
'
Namespace Spielfeld

    Public Class SpielfeldDatenClass

        Sub New()

        End Sub

        ''' <summary>
        ''' Eine Enumeration, mit der das aktuelle UserControl, auf das gezeichnet wird,
        ''' indentifiniert werden kann.
        ''' </summary>
        ''' <returns></returns>
        Public Property VisibleUserControlEnum As VisibleUserControl
        Public Property VisibleUserControlUCtl As Control = Nothing
        '
        ''' <summary>
        ''' Das aktuelle Spielfeld As SpielfeldInfo.
        ''' Das Setzen startet die Animation, wenn vorher von irgendwo anders
        ''' Spielfeld.PaintLimiterModul.PaintSpielfeld_GivePermissionIfPossible = True gesetzt wurde.
        ''' (Dadurch wird Spielfeld.SpielfeldVerwaltung.UpdateSpielfeld_GivePermissionIfPossible
        ''' im Rendertakt aufgerufen, bis aktSpielfeld ungleich Nothing ist.)
        ''' Wird aktSpielfeld = Nothing gesetzt, schaltet sich die Animation auch ab,
        ''' aber mit einer Fehlermeldung direkt auf dem Bildschirm
        ''' </summary>
        Public Property AktSpielfeldInfo As SpielfeldInfo = Nothing
        Public Property SpielfeldSpielfeldInfo As SpielfeldInfo = Nothing
        Public Property WerkbankSpielfeldInfo As SpielfeldInfo = Nothing
        Public Property AktSpielfeldInfo_Ident As String = Nothing
        Public Property LastSpielfeldInfo_Ident As String = Nothing

        Private _aktRendering As RenderingEnum
        Private _consumeRenderingChanged As Boolean
        Public Property AktRendering As RenderingEnum
            Get
                Return _aktRendering
            End Get
            Set(value As RenderingEnum)
                If value <> _aktRendering Then
                    _lastRendering = _aktRendering
                    _aktRendering = value
                    _consumeRenderingChanged = True
                    frmMain.UpdateSpielfeldEditorWerkbankButtons()
                End If
            End Set
        End Property

        Private _lastRendering As RenderingEnum
        Public ReadOnly Property LastRendering() As RenderingEnum
            Get
                Return _lastRendering
            End Get
        End Property

        ''' <summary>
        ''' Gibt einmalig True zurück, wenn Aktrendering geändert wurde.
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property ConsumeRenderingChanged() As Boolean
            Get
                Dim retval As Boolean = _consumeRenderingChanged
                _consumeRenderingChanged = False
                Return retval
            End Get
        End Property


        Public xMin As Integer = 1
        Public yMin As Integer = 1
        Public zMin As Integer = 0
        Public xMax As Integer
        Public yMax As Integer
        Public zMax As Integer
        '

        Public Property xMaxSteine As Integer
        Public Property yMaxSteine As Integer
        Public Property zMaxSteine As Integer

        ''' <summary>
        ''' Gesamte Ausgabefläche wie vom Paint
        ''' </summary>
        Public rxOutput As RectangleX = Nothing
        '
        ''' <summary>
        ''' Oben die Leiste des Steinvorrat beim Editieren.
        ''' </summary>
        Public rxStock As RectangleX = Nothing
        '
        ''' <summary>
        ''' Scrollbar des Steinvorrat
        ''' </summary>
        Public rxStockScrollbar As RectangleX = Nothing
        '
        ''' <summary>
        ''' Hält die UGHrdBitmap. Beim Spielen so groß wie outputRect
        ''' Im Editor rxStageAvailable
        ''' In der Werkbank wie outputRect
        ''' </summary>
        Public rxBitmapUgrd As RectangleX = Nothing
        '
        ''' <summary>
        ''' Hält die Überschrift
        ''' </summary>
        Public rxHeader As RectangleX = Nothing
        '
        ''' <summary>
        ''' UGrdRect ohne Header
        ''' </summary>
        Public rxContent As RectangleX = Nothing
        '
        ''' <summary>
        ''' innerhalb rectxContent
        ''' </summary>
        Public rxHistoryBoxLeftContainer As RectangleX = Nothing
        '
        ''' <summary>
        ''' innerhalb rxHistoryBoxRightContainer
        ''' </summary>
        Public rxHistoryBoxRightContainer As RectangleX = Nothing
        ''' <summary>
        ''' Optional innerhalb rxHistoryBoxLeftContainer
        ''' </summary>
        Public rxHistoryBoxLeft As RectangleX = Nothing
        '
        ''' <summary>
        ''' Optional innerhalb rectxContent
        ''' </summary>
        Public rxHistoryBoxRight As RectangleX = Nothing
        '
        ''' <summary>
        ''' innerhalb rxHistoryBoxLeftContainer
        ''' </summary>
        Public rxHistoryBoxLeftScrollbar As RectangleX = Nothing
        '
        ''' <summary>
        ''' innerhalb rxHistoryBoxRightContainer
        ''' </summary>
        Public rxHistoryBoxRightScrollbar As RectangleX = Nothing
        '
        ''' <summary>
        ''' Der für Spielfeld/Editor zur Verfügung stehende Platz
        ''' für das Spielfeld. Nötig für die Berechnung der Steingröße.
        ''' Wenn dieser Wert nicht berechnet wird, kommt es zur Endlosschleife
        ''' in der Berechnung der Steingröße.
        ''' Wird zum Rendern nicht benötigt, nur zur Kontrolle.
        ''' </summary>
        Public rxStageAvailable As RectangleX = Nothing
        '
        ''' <summary>
        ''' Der von Spielfeld/Editor genutzte Platz des Spielfeldes. Wird nicht in CreateMainLayout
        ''' fesgelegt, sondern in UpdateSpielfeld.
        ''' </summary>
        Public rxStageUsed As RectangleX = Nothing

        Public Property Backbuffer As Bitmap                'Das Zeichenbrett
        Public Property BackBufferGfx As Graphics           'Der Zeichenknecht
        Public Property Backbuffer_HasContent As Boolean
        Public Sub CreateBackbufferAndGfx()
            If rxOutput Is Nothing OrElse rxOutput.IsEmpty Then
                Throw New Exception("Programmierfehler: CreateBackbufferAndGfx vor CreateMainLayout aufgerufen.")
            End If
            'Nur auf Änderungen reagieren, zeitkritisch!
            If Backbuffer Is Nothing Then
                Backbuffer = New Bitmap(rxOutput.Width, rxOutput.Height, PixelFormat.Format32bppArgb)
                BackBufferGfx = Graphics.FromImage(Backbuffer)
            ElseIf Backbuffer.Size <> rxOutput.Size Then
                Backbuffer.Dispose()
                BackBufferGfx.Dispose()
                Backbuffer = New Bitmap(rxOutput.Width, rxOutput.Height, PixelFormat.Format32bppArgb)
                BackBufferGfx = Graphics.FromImage(Backbuffer)
            End If
            BackBufferGfx.Clear(Color.Silver)
            Backbuffer_HasContent = False
        End Sub
        ''' <summary>
        ''' Das ist die aktuelle Breite der Steine
        ''' </summary>
        Public steinWidth As Integer
        '
        ''' <summary>
        ''' Das ist die aktuelle Höhe der Steine
        ''' </summary>
        Public steinHeight As Integer
        ''' <summary>
        ''' Das ist die halbe aktuelle Breite der Steine
        ''' </summary>
        Public steinWidthHalf As Integer
        '
        ''' <summary>
        ''' Das ist die halbe aktuelle Höhe der Steine
        ''' </summary>
        Public steinHeightHalf As Integer

        Public offset3DLeftJeEbene As Integer
        Public offset3DTopJeEbene As Integer

        Public offset3DLeftSumme As Integer
        Public offset3DTopSumme As Integer
        '
        Public steinWidthLastCreated As Integer
        Public steinHeightLastCreated As Integer

        Public ResizePolling As New ResizeActivityPoller(frmMain.UCtlSpielfeldMain)
        Public ResizingIsAktiv As Boolean
        Public MousePolling As New MouseInputPoller(frmMain.UCtlSpielfeldMain)
        Public ReadOnly StartscreenBackgroundImageCache As New BackgroundSingleImageCache(Rendering_StartscreenBitmapFullpath, BackgroundImageRenderMode.Stretch)
        Public StartscreenBackgroundImage As Bitmap = Nothing
        Public AnimationIsWorking As Boolean
        Public DragDropIsWorking As Boolean

        Public RenderingDoneCounter As Integer
        Public RenderingSkipCounter As Integer

        Public ReadOnly DebugLabels As New DebugLabelPlacer()

        Public ToolboxIsVisible As Boolean

        Private _toolboxTabPageChanged As Boolean
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

    End Class

End Namespace