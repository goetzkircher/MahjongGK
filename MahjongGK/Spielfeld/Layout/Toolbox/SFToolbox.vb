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
Imports System.Drawing.Imaging
Imports MahjongGK.Spielfeld

#Disable Warning IDE0079
#Disable Warning IDE1006

Public Enum ToolboxJobAnswer
    DoNothing
    DoRendering
    UpdateSpielfeld
    UpdateSpielfeldX
    CreateSpielbildStep1DoHideGridSetCreateSpielbild
    CreateSpielbildStep2DoCreateSpielbild
End Enum

Public Enum ToolboxPollEvent
    None
    DoLoadSFInfoValues
    ChangedTabPage
    ChangedTexte
    ChangedSpielfeldPicture
    DoChangedSpielfeldSize
    ContextmenueEditor_SomethingChanged
    ContextmenueEditor_ShowTooltipChanged
    CreateSpielbildStep1SetHideGridSetCreateSpielbild
    CreateSpielbildStep2SetDoCreateSpielbild
End Enum

''' <summary>
''' Verarbeitung von Daten der Toolbox, die nicht innerhalb der Toolbox verarbeitet werden.
''' Dazu gehören auch Statusmeldungen, die nicht in der Toolbox angesiedelt sind,
''' sondern in frmMain unten oder Änderungen in den Kontextmenues.
''' </summary>
Public Class SFToolbox

    Public Sub New()

    End Sub

    Public Sub New(owner As SFDaten)
        _sfd = owner
    End Sub

    Private ReadOnly _sfd As SFDaten

    Public Sub SetConsumeTabpagePollEvent(value As ToolboxPollEvent)
        _toolboxPollEvent = value
    End Sub
    Private _toolboxPollEvent As ToolboxPollEvent
    '
    ''' <summary>
    ''' Im wesentlichen werden Änderungen in der Toolbox verarbeitet.
    ''' Hierhin wird nur gemeldet, daß etwas gemacht wurde.
    ''' Dennoch besteht hier die Möglichkeit noch weiter einzugreifen.
    ''' </summary>
    ''' <returns></returns>
    Public Function ConsumeToolboxPollEventJob() As ToolboxJobAnswer

        SetStatusDatenInFrmMain()

        Select Case ConsumeToolboxPollEvent
            Case ToolboxPollEvent.None
                Return ToolboxJobAnswer.DoNothing

            Case ToolboxPollEvent.ChangedTabPage
                Return ToolboxJobAnswer.DoNothing

            Case ToolboxPollEvent.DoLoadSFInfoValues
                'wird aus  FrmToolbox.load aufgerufen  
                frmMain.ToolBox?.SetSfdAndCopyValuesToForm(_sfd)
                Return ToolboxJobAnswer.DoNothing

            Case ToolboxPollEvent.ChangedTexte
                Return ToolboxJobAnswer.DoRendering

            Case ToolboxPollEvent.ChangedSpielfeldPicture
                Return ToolboxJobAnswer.DoNothing

            Case ToolboxPollEvent.DoChangedSpielfeldSize
                ChangeSize()
                Return ToolboxJobAnswer.UpdateSpielfeldX

            Case ToolboxPollEvent.ContextmenueEditor_SomethingChanged
                Return ToolboxJobAnswer.DoRendering

            Case ToolboxPollEvent.ContextmenueEditor_ShowTooltipChanged
                Return ToolboxJobAnswer.UpdateSpielfeldX

            Case ToolboxPollEvent.CreateSpielbildStep1SetHideGridSetCreateSpielbild

                Return ToolboxJobAnswer.CreateSpielbildStep1DoHideGridSetCreateSpielbild

            Case ToolboxPollEvent.CreateSpielbildStep2SetDoCreateSpielbild
                Return ToolboxJobAnswer.CreateSpielbildStep2DoCreateSpielbild

        End Select

        Return ToolboxJobAnswer.DoNothing

    End Function

    Private ReadOnly Property ConsumeToolboxPollEvent As ToolboxPollEvent
        Get
            If _toolboxPollEvent <> ToolboxPollEvent.None Then
                Dim value As ToolboxPollEvent = _toolboxPollEvent
                _toolboxPollEvent = ToolboxPollEvent.None
                Return value
            Else
                Return ToolboxPollEvent.None
            End If
        End Get
    End Property

    '##### Spielfeldgröße ########################################################################################
    '
    ''' <summary>
    ''' Wird von der Toolbox gesetzt.
    ''' </summary>
    ''' <returns></returns>
    Public Property OldStatusSpielfeldSize As ToolboxStatusSpielfeldSize
    '
    ''' <summary>
    ''' Wird von der Tollbox gesetzt.
    ''' </summary>
    ''' <returns></returns>
    Public Property NewStatusSpielfeldSize As ToolboxStatusSpielfeldSize

    Structure ToolboxStatusSpielfeldSize
        Public x As Integer
        Public y As Integer
        Public z As Integer
        Public changeLeft As Boolean
        Public changeLeftRight As Boolean
        Public changeRight As Boolean
        Public changeTop As Boolean
        Public changeTopBottom As Boolean
        Public changeBottom As Boolean
        Public sizeInSteinen As Triple
    End Structure

    ''' <summary>
    ''' Einfügen der Änderungen genau im Rendertakt.
    ''' </summary>
    Private Sub ChangeSize()

        Dim addOrRemoveLeft As Integer
        Dim addOrRemoveTop As Integer
        Dim addOrRemoveRight As Integer
        Dim addOrRemoveBottom As Integer
        Dim addOrRemoveLayer As Integer
        Dim deltaX As Integer
        Dim deltaY As Integer

        With OldStatusSpielfeldSize
            deltaX = NewStatusSpielfeldSize.x - .x 'das ist die Anzahl in Steinen, nicht in Spalten /Zeilen.
            deltaY = NewStatusSpielfeldSize.y - .y 'gebraucht wird in Spalten/Zeilen, daher gleich mal 2 nehmen.
            addOrRemoveLayer = NewStatusSpielfeldSize.z - .z
        End With

        If NewStatusSpielfeldSize.changeLeft Then
            addOrRemoveLeft = deltaX * 2 'Das ist in Spalten
            addOrRemoveRight = 0
        ElseIf NewStatusSpielfeldSize.changeLeftRight Then
            addOrRemoveLeft = deltaX  'Spalten verteilen
            addOrRemoveRight = deltaX
        Else 'changeRight
            addOrRemoveLeft = 0
            addOrRemoveRight = deltaX * 2 'Das ist in Spalten
        End If
        '
        If NewStatusSpielfeldSize.changeTop Then
            addOrRemoveTop = deltaY * 2
            addOrRemoveBottom = 0
        ElseIf NewStatusSpielfeldSize.changeTopBottom Then
            addOrRemoveTop = deltaY
            addOrRemoveBottom = deltaY
        Else 'changeRight
            addOrRemoveTop = 0
            addOrRemoveBottom = deltaY * 2
        End If

        'Da die Änderungen in der Toolbox bereits geprüft sind,
        'ist die IdPossible-Abfrage eigentlich überflüssig.
        If _sfd.SFInf.RedimArrFBIsPossible(
                                addOrRemoveLeft,
                                addOrRemoveTop,
                                addOrRemoveRight,
                                addOrRemoveBottom,
                                addOrRemoveLayer) Then

            _sfd.SFInf.ChangeSpielfeldSize(NewStatusSpielfeldSize.sizeInSteinen,
                                       addOrRemoveLeft,
                                        addOrRemoveTop,
                                        addOrRemoveRight,
                                        addOrRemoveBottom,
                                        addOrRemoveLayer)

        End If

    End Sub

    Public Sub ClearStatusDatenInFrmMain()
        Dim sci As New SFInfo.SteinCountInfo
        SetStatusDatenInFrmMain(sci)
    End Sub

    Private _oldSteinCountInfo As SFInfo.SteinCountInfo = Nothing
    Private Sub SetStatusDatenInFrmMain()
        Dim sci As SFInfo.SteinCountInfo = _sfd.SFInf.AktSteinCountInfo
        SetStatusDatenInFrmMain(sci)
    End Sub

    Private Sub SetStatusDatenInFrmMain(sci As SFInfo.SteinCountInfo)

        If Not sci.AnyValueChanged(_oldSteinCountInfo) Then
            Exit Sub
        End If

        _oldSteinCountInfo = sci

        With frmMain.ToolStripExMain
            Dim lblSize As ToolStripLabel = TryCast(.Items("stat_size"), ToolStripLabel)
            Dim lblGesamt As ToolStripLabel = TryCast(.Items("stat_total"), ToolStripLabel)
            Dim lblAktuell As ToolStripLabel = TryCast(.Items("stat_current"), ToolStripLabel)
            Dim lblWaehlbar As ToolStripLabel = TryCast(.Items("stat_sel"), ToolStripLabel)
            Dim lblPaare As ToolStripLabel = TryCast(.Items("stat_pairs"), ToolStripLabel)
            Dim lblStock As ToolStripLabel = TryCast(.Items("stat_stock"), ToolStripLabel)
            With sci
                lblGesamt.Text = $"Summe der Steine { .summeSteineGesamt} gesamt"
                lblAktuell.Text = $"{ .summeSteineAktuell} aktuell"
                lblWaehlbar.Text = $"{ .davonSelectable} wählbar"
                lblPaare.Text = $"{ .davonRemovable} entfernbar"
                lblSize.Text = $"Feldgröße: { .xMax}/{ .yMax}/{ .zMax}"
                If _sfd.SFRun.AktRenderMode = Contracts.GlobalEnum.AktRenderMode.Edit Then
                    lblStock.Text = $"{ .imVorrat} im Vorrat"
                End If
            End With
        End With
    End Sub

    Public Sub CreateSpielbild()
        If _sfd.SFRun.Backbuffer_HasContent Then
            'das ist praktisch immer der Fall, deshalb kein Else.
            If _sfd.SFLay.rxStageUsed IsNot Nothing Then
                'auch das ist immer der Fall,wenn CreateSpielbild nur aufgerufen wird,
                'wenn der Editor aktiv ist, also auch nur Notbremse ohne Else.
                '
                With _sfd.SFLay.rxStageUsed

                    Dim bmp As New Bitmap(.Width, .Height, PixelFormat.Format32bppArgb)

                    Using gfx As Graphics = Graphics.FromImage(bmp)

                        Dim dstRect As New Rectangle(0, 0, .Width, .Height)
                        Dim srcRect As New Rectangle(.Left, .Top, .Width, .Height)

                        gfx.DrawImage(_sfd.SFRun.Backbuffer, dstRect, srcRect, GraphicsUnit.Pixel)

                    End Using

                    frmMain.ToolBox?.SetSpielbild(bmp)

                End With

            End If
        End If
    End Sub

End Class
