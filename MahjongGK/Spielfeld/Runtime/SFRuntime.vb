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

Imports System.Drawing.Imaging
Imports MahjongGK.Contracts.GlobalEnum

Namespace Spielfeld
    ''' <summary>
    ''' Pfad: MahjongGK/Spielfeld/
    ''' 
    ''' Hält den nicht persistenten, laufzeitabhängigen Zustand eines konkreten Spielfelds. 
    ''' 
    ''' SFRun enthält alle Werte, die logisch zu genau einem bestimmten Spielfeld gehören, 
    ''' aber nicht mit gespeichert werden sollen. Dazu zählen insbesondere: 
    ''' 
    ''' - laufzeitabhängige Such- und Sortierindizes, 
    ''' - Dirty-Flags und Rebuild-Marker, 
    ''' - feldbezogene Werte, die beim nächsten Rendern oder Aktualisieren ausgewertet werden, 
    ''' - weitere Caches oder Hilfsdaten, die nur für dieses konkrete Spielfeld gelten. 
    ''' 
    ''' SFRun ist damit die Brücke zwischen Fachmodell und Darstellung: 
    ''' Die eigentlichen Spielfelddaten bleiben in SFInf, 
    ''' die daraus abgeleiteten Laufzeitwerte liegen in SFRun. 
    ''' 
    ''' Faustregel: 
    ''' Gehört ein Wert zu genau diesem einen Spielfeld, aber nicht ins XML, 
    ''' dann ist SFRun meist der richtige Ort.
    '''</summary> 
    Public Class SFRuntime

        Public Sub New()

        End Sub

        Public Sub New(owner As SFDaten)
            _sfd = owner
        End Sub

        Private ReadOnly _sfd As SFDaten

#Region "Rendering-Modus / Zustandswechsel"

        Private _aktRenderMode As AktRenderMode
        Private _lastAktRenderMode As AktRenderMode
        Private _consumeAktRenderModeChanged As Boolean
        '
        ''' <summary>
        ''' ReadOnly, weil es vom Programm gesetzt wird aus einem der 
        ''' Folgeaufrufe des Paint-Events der AusgabeControls.
        ''' "manuelle Änderung des RenderMode nur über SFMain.RenderMod.
        ''' </summary>
        Public ReadOnly Property AktRenderMode As AktRenderMode
            Get
                Return _aktRenderMode
            End Get
        End Property
        '
        ''' <summary>
        ''' Wird vom Programm gesetzt aus einem der Folgeaufrufe des 
        ''' Paint-Events der AusgabeControls.
        ''' "manuelle Änderung des RenderMode nur über SFMain.RenderMod.
        ''' </summary>
        ''' <param name="value"></param>
        Public Sub SetAktRenderMode(value As AktRenderMode)
            If value <> _aktRenderMode Then
                _lastAktRenderMode = _aktRenderMode
                _aktRenderMode = value
                _consumeAktRenderModeChanged = True
                frmMain.UpdateSpielfeldEditorButtons()
            End If
        End Sub

        Public ReadOnly Property LastAktRenderMode As AktRenderMode
            Get
                Return _lastAktRenderMode
            End Get
        End Property

        ''' <summary>
        ''' Gibt einmalig True zurück, wenn AktRenderMode geändert wurde.
        ''' </summary>
        Public ReadOnly Property ConsumeAktRenderModeChanged As Boolean
            Get
                Dim retval As Boolean = _consumeAktRenderModeChanged
                _consumeAktRenderModeChanged = False
                Return retval
            End Get
        End Property

#End Region

#Region "Current / aktive Spielfelder"
        ''' <summary>
        ''' Wenn die sich ändert, wurde "On the Fly" ein anderes
        ''' Spiel geladen.
        ''' </summary>
        ''' <returns></returns>
        Public Property AktSpielfeldInfo_Ident As String = Nothing
        Public Property LastSpielfeldInfo_Ident As String = Nothing

#End Region

#Region "Current / Session / Versionskontrolle"

        Private ReadOnly _renderversionSpielfeld As Integer = -1
        Private ReadOnly _renderversionWerkbank As Integer = -1

        Private _sessionIdentSpielfeld As String = "irgendwas1"
        Private ReadOnly _sessionIdentWerkbank As String = "irgendwas2"

        ''' <summary>
        ''' Analog RenderVersionChanged.
        ''' </summary>
        Public ReadOnly Property ConsumeSessionIdentChanged As Boolean
            Get
                Dim result As Boolean

                If _sessionIdentSpielfeld <> _sfd.SFInf.SessionIdent Then
                    _sessionIdentSpielfeld = _sfd.SFInf.SessionIdent
                    result = True
                End If

                Return result
            End Get
        End Property

#End Region

#Region "Render - Backbuffer / Graphics"

        Public Property Backbuffer As Bitmap
        Public Property BackBufferGfx As Graphics
        Public Property Backbuffer_HasContent As Boolean

        Public Sub CreateBackbufferAndGfx()
            With _sfd.SFLay.rxOutputUsed
                If _sfd.SFLay.rxOutputUsed Is Nothing OrElse _sfd.SFLay.rxOutputUsed.IsEmpty Then
                    Throw New Exception("Programmierfehler: CreateBackbufferAndGfx vor CreateMainLayoutIterativStep aufgerufen.")
                End If

                If Backbuffer Is Nothing Then
                    Backbuffer = New Bitmap(.Width, .Height, PixelFormat.Format32bppArgb)
                    BackBufferGfx = Graphics.FromImage(Backbuffer)

                ElseIf Backbuffer.Size <> .Size Then
                    Backbuffer.Dispose()
                    BackBufferGfx.Dispose()

                    Backbuffer = New Bitmap(.Width, .Height, PixelFormat.Format32bppArgb)
                    BackBufferGfx = Graphics.FromImage(Backbuffer)
                End If
            End With

            BackBufferGfx.Clear(Color.Silver)
            Backbuffer_HasContent = False

        End Sub

#End Region

#Region "Render - Cache / Startscreen / Debug"

        Public ReadOnly Property DebugLabels As New DebugLabelPlacer()

#End Region

#Region "Runtime Flags / Aktivitätsstatus"

        Public Property AnimationIsWorking As Boolean
        Public Property DragDropIsWorking As Boolean
        Public Property ResizingIsAktiv As Boolean

#End Region

#Region "Runtime Zähler / Statistik"

        Public Property RenderingDoneCounter As Integer
        Public Property RenderingSkipCounter As Integer

#End Region

#Region "Polling / Eingabe / Beobachter"

        Public Property ResizePolling As New ResizeActivityPoller(frmMain.UCtlSpielfeldMain, idleMs:=250)
        Public Property MousePolling As New MouseInputPoller(frmMain.UCtlSpielfeldMain)

#End Region

#Region "UI - Renderer / Controls / Formulare"

        Public Property HScrollBarStock As HScrollRenderer = Nothing

        Public Property EditorFrmTooltipSteinInfo As frmTooltipSteinInfo = Nothing

#End Region

#Region "Editor - ausstehende Aktionen"

        ''' <summary>
        ''' Hält die nächste Ablageposition, wenn IsValideYes.
        ''' </summary>
        Public Property EditorSteinDoPlaceAtPosTriple As New Triple

        Public Property EditorSteinDoPlace_SteinIndex As SteinSymbol

#End Region

#Region "Helfer / spätere Auslagerung"

        'Hier später optionale Hilfsmethoden unterbringen, z. B.:
        '
        'Private Sub ResetRenderState()
        'Private Sub ResetEditorState()
        'Private Sub ResetLayoutState()
        'Private Sub ResetCurrentSelection()
        '
        'Noch nicht nötig, aber dies ist der sinnvolle Sammelpunkt.

#End Region

    End Class
End Namespace