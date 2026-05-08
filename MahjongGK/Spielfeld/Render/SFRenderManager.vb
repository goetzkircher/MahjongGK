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

'
#Disable Warning IDE0079
#Disable Warning IDE1006

Namespace Spielfeld
    ''' <summary>
    ''' Pfad: MahjongGK/Spielfeld/Render
    ''' </summary>
    Public Class SFRenderManager
        Implements IDisposable

        Public Sub New()

        End Sub

        Public Sub New(owner As SFDaten)
            _sfd = owner
        End Sub

        Private ReadOnly _sfd As SFDaten

        Public RenderTakt As New RenderingTaktgeber()

        Private _lastRectOutput As New Rectangle

        Private _initialisierungLäuft As Boolean

        Private _createScreenShot As Boolean

        Private _takteAussetzen As Integer

        Private _Spielbetrieb_PositionHistory As Integer = -1 'nicht möglicher Wert

        Public Sub PaintSpielfeld_CreateScreenShot()
            _createScreenShot = True
        End Sub

        ' „Wecker“ (10–15ms ist gut),  = 1 ergibt einen etwas stabileren Takt von 15 ms,
        ' hat aber Auswirkung auf alle Timer und kostet mehr Energie.
        ' Werte über 30 verlangsamen die Geschwindigkeit, was zu einer Vergröberung
        ' der Animation führt. (Die Gesamtlänge der Animation bleibt, die Schritte
        ' werde größer, bis es irgendwann kippt und die Anumationsdauer sich verlängert.) 
        Public WithEvents RenderTimer As New System.Windows.Forms.Timer With {
            .Interval = INI.Rendering_RenderTimerIntervalWorking}

        '  <DebuggerStepThrough>
        Private Sub RenderTimer_Tick(sender As Object, e As EventArgs) Handles RenderTimer.Tick

            'Bedingungen, unter denen nicht gerendert werden soll.

            If SFMain.VisibleUserControlEnum <> VisibleUserControl.Spielfeld Then
                Exit Sub
            End If

            Dim vuctl As Control = SFMain.VisibleUserControlControl

            If vuctl Is Nothing OrElse vuctl.IsDisposed Then
                Exit Sub
            End If

            If Not vuctl.Visible OrElse
                vuctl.Width = 0 OrElse
                vuctl.Height = 0 Then
                Exit Sub
            End If

            ' Der eigentliche Takt kommt hier von der Stopwatch im Scheduler:
            If RenderTakt.TryNextFrame() Then
                'und bewirkt hier, daß das AnzeigeControl Invalidate gesetzt wird,
                'was zur Folge hat, das das Paint Event des Controls aufgerufen wird.
                'Dieses wiederum ruft PaintUCtlSpielfeld in dieser Klasse auf.
                vuctl.Invalidate()
#If DEBUGFRAME Then
                        Deb ug.Print("TryNextFrame = True")
                    Else
                        Deb ug.Print("TryNextFrame = False")
#End If
            End If
        End Sub

        Private Sub SetRendertimerIntervall(Optional setWorking As Boolean = False)
            If setWorking Then
                RenderTimer.Interval = INI.Rendering_RenderTimerIntervalWorking
            Else
                If _sfd.SFRun.RenderingSkipCounter > INI.Rendering_RenderTimerFramesToPause Then
                    RenderTimer.Interval = INI.Rendering_RenderTimerIntervalPaused
                Else
                    RenderTimer.Interval = INI.Rendering_RenderTimerIntervalWorking
                End If
            End If
        End Sub

        ''' <summary>
        ''' Dieses Sub wird vom PaintEvent der Zeichenfläche (UCtlSpielfeld und UCtlEdtor) getaktet
        ''' aufgerufen. Es ist ein Verteiler, der entweder das Update des Spielfeldes oder das Zeichnen selber aufruft.
        ''' </summary>
        ''' <param name="e"></param>
        ''' <param name="rectOutput"></param>
        ''' <param name="timeDifferenzFaktor"></param>
        Public Sub PaintUCtlSpielfeld(e As PaintEventArgs, rectOutput As Rectangle, timeDifferenzFaktor As Double)

            '
            'Der frühestmögliche Zeitpunkt den AktRenderMode festzulegen.
            'Abgeleitet vom Rendermode.
            'Es gibt nur diese eine Stelle im Programm, wo der AktRenderMode
            'gesetzt wird. 
            'Manuelles Setzen nur direkt über SFMain.RenderMode.
            If SFMain.SFDatHasDataAndDoRender Then
                SFMain.SFDat.SFRun.SetAktRenderMode(CvtToAktRenderMode(SFMain.RenderMode))

                If SFMain.SFDat.SFRun.AktRenderMode = AktRenderMode.None Then
                    '... deshalb:
                    Try
                        If SFMain.SFDatHasDataAndDoRender Then
                            PaintBackbuffer(e.Graphics, rectOutput)
                        Else
                            RenderingStartScreen.PaintStartScreen(e.Graphics, rectOutput)
                        End If
                    Catch ex As Exception
                        If Debugger.IsAttached Then
                            Stop
                        End If
                        'ignorieren
                    End Try
                    Exit Sub
                End If
            Else
                PaintStartScreen(e.Graphics, rectOutput)
                Exit Sub
            End If

            If _takteAussetzen > 0 Then
                'Nötig, wenn die INI im laufendem Betrieb geändert wird, (was nur wärend der Programmentwicklung
                'möglich ist und auch dann nur, wenn das Programm in der IDE läuft), weil die Ini dann diverse
                'Events auslöst, die erst abgearbeitet werden müssen. Um Seiteneffekte zu vermeiden, muß das
                'Rendern ausgesetzt werden.
                'Die Taktzahl ist bewußt viel zu hoch angesetzt (etwa 1/2 Sekunde), damit erübrigt sich es,
                'darüber nachzudenken, ob es notwendig wird, die Taktzahl später nochmal zu erhöhen. 

                _takteAussetzen -= 1
                'Auskommentieren, wenn man den Zeitpunkt sehen will,
                'an dem die INI-Datei geändert wurde. Die Oberfläche friert dann kurz und schwarz eingefärbt ein.
                If _sfd.SFRun.Backbuffer_HasContent Then
                    PaintBackbuffer(e.Graphics, _sfd.SFLay.rxOutputUsed)
                Else
                    PaintStartScreen(e.Graphics, rectOutput)
                End If
                Exit Sub
            End If

#Region "Mausaktionen und Größenänderung der Form auswerten und Rendern ggf verkürzen."

            Dim somethingDoneMousePoll As Boolean = _sfd.SFRun.MousePolling.Poll
            Dim somethingDoneResizePoll As Boolean = _sfd.SFRun.ResizePolling.Poll
            Dim somethingDoneTopSteinInfosPoll As Boolean = _sfd.SFInf.ConsumeTopSteinInfosPolling()
            '()
            Dim somethingDoneTopAirPlanePoll As Boolean = _sfd.SFAir.ConsumeAirplanePolling()

            Dim somethingDoneBitmapUGrd As Boolean = False
            If _sfd.SFInf.BitmapUGrdSingleImgCache IsNot Nothing Then
                somethingDoneBitmapUGrd = _sfd.SFInf.BitmapUGrdSingleImgCache.ConsumeBitmapChanged
            End If
            _sfd.SFLay.BitmapUGrdChanged = somethingDoneBitmapUGrd

            'If _sfd.ToolboxTabPageChanged Then
            '    Stop
            'End If

            Dim updateSpielfeld As Boolean = False
            Dim doRendering As Boolean = False

            If _Spielbetrieb_PositionHistory <> INI.Spielbetrieb_PositionHistory Then
                updateSpielfeld = True
                doRendering = True
                _Spielbetrieb_PositionHistory = INI.Spielbetrieb_PositionHistory
                SetRendertimerIntervall(setWorking:=True)
            End If

            If _sfd.SFLay.BitmapUGrdChanged Then
                updateSpielfeld = True
                doRendering = True
            End If

            If Not doRendering Then
                If somethingDoneResizePoll Then
                    'Auswertung des Resizing hat Priorität
                    '(Bei Größenänderung braucht das MousePolling nicht ausgewertet zu werden
                    'da die Maus beschäftigt ist)
                    If _sfd.SFRun.ResizePolling.ConsumeResizeStarted Then
                        _sfd.SFRun.ResizingIsAktiv = True
                    End If
                    If _sfd.SFRun.ResizePolling.ConsumeResizeEnded Then
                        _sfd.SFRun.ResizingIsAktiv = False
                    End If
                ElseIf somethingDoneMousePoll Then
                    doRendering = True
                ElseIf somethingDoneTopAirPlanePoll Then
                    doRendering = True
                ElseIf somethingDoneTopSteinInfosPoll Then
                    doRendering = True
                End If

                SetRendertimerIntervall(somethingDoneMousePoll)

                If _sfd.SFRun.ResizingIsAktiv Then
                    If _sfd.SFRun.Backbuffer_HasContent Then
                        PaintBackbuffer(e.Graphics, _sfd.SFLay.rxOutputUsed)
                    Else
                        PaintStartScreen(e.Graphics, rectOutput)
                    End If
                    Exit Sub
                End If
            End If

            Dim aktRenderModeOrSteinBasisSizeChanged As Boolean

            'Die Consume... Abfragen dürfen nicht übersprungen werden,
            'also kein ElseIf...
            If SFMain.SFDat.SFRun.ConsumeAktRenderModeChanged Then
                aktRenderModeOrSteinBasisSizeChanged = True
                updateSpielfeld = True
                doRendering = True
            End If

            If INI.ToolBox_ConsumeTabPageChanged Then
                'Sonst schaltet die Spielfeld/Editor-Umschaltung nicht, wenn die Toolbox auf frmMain steht.
                updateSpielfeld = True
                doRendering = True
            End If

            If _sfd.SFRun.ConsumeSessionIdentChanged Then
                updateSpielfeld = True
                doRendering = True
            End If

            If rectOutput <> _lastRectOutput Then
                updateSpielfeld = True
                doRendering = True
                _lastRectOutput = rectOutput
            End If

            'Die Consume... Abfragen dürfen nicht übersprungen werden,
            'also kein ElseIf...
            If INI.Tile_ConsumeSteinSatzOrBasisSizeChanged Then
                updateSpielfeld = True
                doRendering = True
                aktRenderModeOrSteinBasisSizeChanged = True
            End If

            If _sfd.SFInf.ConsumeSteinInfosCountChanged Then
                doRendering = True
            End If

            If doRendering = False Then
                If somethingDoneMousePoll Then
                    doRendering = True
                ElseIf somethingDoneBitmapUGrd Then
                    doRendering = True
                ElseIf _createScreenShot Then
                    doRendering = True
                ElseIf _initialisierungLäuft Then
                    doRendering = True
                ElseIf _takteAussetzen > 0 Then
                    doRendering = True
                    'ZLVxxx  ElseIf _sfd.SFRun.EditorStockSteinFlugValues?.IsFlying Then
                    'ZLVxxx      doRendering = True
                End If
            End If

            If Not doRendering Then
                If _sfd.SFRun.Backbuffer_HasContent Then
                    PaintBackbuffer(e.Graphics, _sfd.SFLay.rxOutputUsed)
                Else
                    PaintStartScreen(e.Graphics, rectOutput)
                End If
                Exit Sub
            End If

            _sfd.SFRun.RenderingSkipCounter = 0
            _sfd.SFRun.RenderingDoneCounter += 1

#End Region

            If _createScreenShot Then
                'Ich gehe davon aus, daß der Backpuffer Inhalt hat, denn es ist sehr unwahrscheinlich,
                'daß der Screenshot angefordert wird, wenn noch nie etwas gezeichnet wurde.
                _createScreenShot = False
                Try
                    _sfd.SFRun.Backbuffer.Save(AppDataFullPath(AppDataSubDir.Diverses,
                                                        AppDataSubSubDir.Diverses_ScreenShots,
                                                        AppDataFileName.ScreeenShot_png,
                                                        AppDataTimeStamp.Add,
                                                        maxFiles:=INI.Sonstiges_ScreenshotMaxCount), Imaging.ImageFormat.Png)
                Catch ex As Exception
                    'dann gibt es keinen Screenshot
                End Try
            End If

            If _initialisierungLäuft And _sfd.SFRun.AktRenderMode <> RenderingEnum.None Then
                'Es muss sichergestellt sein, daß _initialisierungLäuft korrekt rückgestellt wird
                'deshalb wird hier die Prüfung vorab durchgeführt, die in UpdateSpielfeld
                'auch durchgeführt wird, aber ohne Möglichkeit im Fehlerfall das Flag stehen zu lassen.
                Select Case _sfd.SFRun.AktRenderMode
                    Case AktRenderMode.Spiel
                        _initialisierungLäuft = False
                        updateSpielfeld = False
                        _sfd.SFLay.UpdateSpielfeldLayout(rectOutput, aktRenderModeOrSteinBasisSizeChanged)
                        'ZLVxxx  _sfd.SFMouse.UpdateMouseValues(rectOutput, somethingDoneMousePoll, spielfeldIsUpdated:=True)
                        _sfd.SFRen.RenderingDistributor(e.Graphics, rectOutput, timeDifferenzFaktor)
                        Exit Sub

                    Case AktRenderMode.Edit
                        _initialisierungLäuft = False
                        updateSpielfeld = False
                        _sfd.SFLay.UpdateSpielfeldLayout(rectOutput, aktRenderModeOrSteinBasisSizeChanged)
                        'ZLVxxx  _sfd.SFMouse.UpdateMouseValues(rectOutput, somethingDoneMousePoll, spielfeldIsUpdated:=True)
                        _sfd.SFRen.RenderingDistributor(e.Graphics, rectOutput, timeDifferenzFaktor)
                        Exit Sub

                End Select
            End If

            If updateSpielfeld Then
                _sfd.SFLay.UpdateSpielfeldLayout(rectOutput, aktRenderModeOrSteinBasisSizeChanged)
                'ZLVxxx  _sfd.SFMouse.UpdateMouseValues(rectOutput, somethingDoneMousePoll, spielfeldIsUpdated:=True)
                _sfd.SFRen.RenderingDistributor(e.Graphics, rectOutput, timeDifferenzFaktor)
                updateSpielfeld = False
                Exit Sub
            End If

            If doRendering Then
                'ZLVxxx   _sfd.SFMouse.UpdateMouseValues(rectOutput, somethingDoneMousePoll, spielfeldIsUpdated:=False)
                _sfd.SFRen.RenderingDistributor(e.Graphics, rectOutput, timeDifferenzFaktor)
            End If
        End Sub

        Private Sub PaintBackbuffer(PaintEventGfx As Graphics, rectOutput As Rectangle)

            _sfd.SFRun.RenderingSkipCounter += 1
            _sfd.SFRun.RenderingDoneCounter = 0

            If _sfd.SFRun.Backbuffer_HasContent Then
                MjGDI.GfxPushStored(PaintEventGfx) 'wäre aktuell entbehrlich, nur zur Sicherheit für zukünftige Änderungen.
                MjGDI.GfxConfigureForLiveResize(PaintEventGfx, _sfd.SFRun.Backbuffer.Size, rectOutput.Size)
                Dim scaled As Boolean
                If rectOutput.Size = _sfd.SFRun.Backbuffer.Size Then
                    PaintEventGfx.DrawImageUnscaledAndClipped(_sfd.SFRun.Backbuffer, rectOutput)
                    scaled = False
                Else
                    PaintEventGfx.DrawImage(_sfd.SFRun.Backbuffer, rectOutput)
                    scaled = True
                End If
                If INI.Rendering_DrawRenderingSkipDoneMarker Then
                    'unterer rechter Marker, .
                    DrawRenderingSkipDoneMarker(PaintEventGfx, rectOutput.Width, rectOutput.Height, _sfd.SFRun.RenderingSkipCounter, "Skip", scaled)
                End If
                MjGDI.GfxPopStored(PaintEventGfx)
            Else
                PaintEventGfx.DrawImageUnscaledAndClipped(RenderingStartScreen.StartscreenBackgroundImageCache.GetBitmap(rectOutput.Size), rectOutput)
            End If
        End Sub
        '

        Private _disposed As Boolean
        Public Sub Dispose() Implements IDisposable.Dispose
            If Not _disposed Then
                If RenderTimer IsNot Nothing Then
                    RemoveHandler RenderTimer.Tick, AddressOf RenderTimer_Tick
                    RenderTimer.Stop()
                    RenderTimer.Dispose()
                    RenderTimer = Nothing
                End If
                _disposed = True
            End If
            GC.SuppressFinalize(Me)
        End Sub
    End Class
End Namespace