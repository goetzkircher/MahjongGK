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

Imports MahjongGK.Contracts

Namespace Spielfeld

#Disable Warning IDE0079
#Disable Warning IDE1006

    ''' <summary>
    ''' Pfad: MahjongGK/Spielfeld/Main
    ''' </summary>
    Module SFMain

        ''' <summary>
        ''' Pfad: MahjongGK/Spielfeld/Main/SFMain
        ''' Hierüber kann auf alle SF...-Klassen zugegriffen werden.
        ''' </summary>
        Public SFDat As SFDaten

        Private _RenderMode As RenderMode '= RenderMode.NoDataLoaded 'Default
        '
        ''' <summary>
        ''' Hiermit wird gesteuert ob und was gerendert wird.
        ''' In SFRun gibt es Public ReadOnly Property AktRenderMode As AktRenderMode
        ''' Innerhalb aller Render-Funktionen ist diese Propery zu verwenden.
        ''' 
        ''' HINWEIS:
        ''' Wenn SFDat Nothing ist, knallt es. Nicht hier abfangen, sondern beim Aufrufer,
        ''' damit RenderMode nicht außer Takt kommt.
        ''' Abfangfrage über:
        ''' If SFMain.SFDatHasData Then
        ''' oder
        ''' If SFMain.SFDatHasNoData Then
        ''' </summary>
        ''' <returns></returns>
        Public Property RenderMode As RenderMode
            Get
                Return _RenderMode
            End Get
            Set(value As RenderMode)

                _RenderMode = value

                If value = RenderMode.Edit OrElse value = RenderMode.Spiel Then
                    If _visibleUserControlEnum <> VisibleUserControl.Spielfeld Then
                        'Wenn es knallt Hinweis lesen
                        SFDat.SFRenMan.RenderTimer.Stop()
                        If Debugger.IsAttached Then
                            Throw New Exception("Programmierfehler: RenderMode.Editor OrElse RenderMode.Spielfeld angefordert ohne gültiges VisibleUserControl")
                        End If
                        Exit Property
                    End If
                End If

                Select Case value
                        'Wenn es knallt Hinweis lesen
                    Case RenderMode.NoRendering 'Synonym: RenderMode.EndOfSpiel
                        'bewußte Umstellung ==> immer speichern 
                        If Not IsNothing(SFDat) Then
                            SFDat.SFInf.SaveLastUsedSpielfeld()
                            SFDat.SFRenMan.RenderTimer.Stop()
                        End If

                    Case RenderMode.Paused
                        SFDat.SFRenMan.RenderTimer.Stop()

                    Case RenderMode.Spiel, RenderMode.Edit
                        SFDat.SFRenMan.RenderTakt.Reset()
                        SFDat.SFRenMan.RenderTimer.Start()

                    Case Else
                        Stop 'Programmierfehler
                End Select

            End Set
        End Property
        '
        Private _VisibleUserControlControl As Control
        Private _visibleUserControlEnum As VisibleUserControl = VisibleUserControl.None
        '
        ''' <summary>
        ''' Das UserControl, auf das gezeichnet wird.
        ''' Wird von frmMain hier eingetragen.
        ''' Wird 
        ''' </summary>
        Public Sub SetVisibleUserControl(control As Control, visibleUserControlEnum As VisibleUserControl)

            _VisibleUserControlControl = control
            _visibleUserControlEnum = visibleUserControlEnum
        End Sub

        Public ReadOnly Property VisibleUserControlControl As Control
            Get
                Return _VisibleUserControlControl
            End Get
        End Property
        Public ReadOnly Property VisibleUserControlEnum As VisibleUserControl
            Get
                Return _visibleUserControlEnum
            End Get
        End Property

        '
        ''' <summary>
        ''' Erzeugt ein komplett neues leeres Spielfeld mit dem Dimensionen spielsize.
        ''' Sind die Dimensionen ungültig, wird False zurückgegeben.
        ''' Über SFMain.GetSpielfeldDimensionen_AsMsgString kann ein Hilfetext abgerufen werden.
        ''' Die Gültigkeit wird nur hier geprüft, sonst nirgends.
        ''' Über einen Trick kann man die Grenzen überwinden:
        ''' Ein beliebiges Spielfeld erzeugen und speichern.
        ''' In der Xml-Datei die Spielfeldgröße ändern (nur vergrößern zulässig) und wieder laden.
        ''' </summary>
        ''' <param name="spielsize"></param>
        ''' <returns></returns>
        Public Function CreateSpielfeld(spielsize As Triple) As Boolean

            CloseSpielfeld()

            If Not SFInfo.CheckSpielsize(spielsize) Then
                Return False
            End If

            SFDat = New SFDaten(spielsize)

            'Der RenderMode bleibt auf NoDataLoaded, auch wenn Daten geladen wurden, denn
            'Das Umschalten bewirkt das sofortige Rendern.
            'Public Property RenderMode As RenderMode muss explizit zu Start aufgerufen werden.

            Return True

        End Function

        Public Function CreateSpielfeld(spielfeldinfo As SFInfo) As Boolean

            CloseSpielfeld()

            If Not SFInfo.CheckSpielsize(spielfeldinfo.SpielSizeInSteinen) Then
                Return False
            End If

            SFDat = New SFDaten(spielfeldinfo)

            'Der RenderMode bleibt auf NoDataLoaded, auch wenn Daten geladen wurden, denn
            'Das Umschalten bewirkt das sofortige Rendern.
            'Public Property RenderMode As RenderMode muss explizit zu Start aufgerufen werden.

            Return True

        End Function

        Public Sub CloseSpielfeld()

            RenderMode = RenderMode.NoRendering

            CloseToolBox()

            If SFDat IsNot Nothing Then
                SFDatDisposeAndSetNothing(saveSpielFeld:=If(SFDat.SFInf.xMax = 0, False, True))
            End If
        End Sub

        Public ReadOnly Property SFDatHasDataAndDoRender As Boolean
            Get
                If SFDat Is Nothing Then
                    Return False
                ElseIf RenderMode = RenderMode.NoRendering Then
                    Return False
                Else
                    Return True
                End If
            End Get
        End Property

        '
        ''' <summary>
        ''' Gibt die minimalen und maximalen gültigen Werte der
        ''' Spielfelddimensionen zurück.
        ''' </summary>
        ''' <returns></returns>
        Public Function GetSpielfeldDimensionen_AsMsgString(Optional asErrormessage As Boolean = False) As String
            Dim sb As New System.Text.StringBuilder
            If asErrormessage Then
                sb.AppendLine("Ungültige Feldabmessungen!")
                sb.AppendLine("Gültig sind:")
            Else
                sb.AppendLine("Erlaubte Feldabmessungen:")
            End If
            sb.AppendLine($"{MJ_STEINE_MINX} bis {MJ_STEINE_MAXX} Steine nebeneinandern")
            sb.AppendLine($"{MJ_STEINE_MINY} bis {MJ_STEINE_MAXY} Steine übereinander")
            sb.AppendLine($"{MJ_STEINE_MINZ} bis {MJ_STEINE_MAXZ} Steine aufeinander")
            Return sb.ToString
        End Function

        Public Function LoadSpielfeld(path As String) As Boolean
            Throw New Exception("noch nicht implementiert")
        End Function
        Public Function LoadLastSpielfeld() As Boolean
            Throw New Exception("noch nicht implementiert")
        End Function

        ''' <summary>
        ''' Vorsicht Falle: Nicht verwenden zur Unterscheidung ob der Rendermode
        ''' nach Spielbeginn auf RenderMode.NoDataLoaded (zurück)gesetzt wurde,
        ''' da RenderMode = RenderMode.NoDataLoaded immer True zurückgibt.
        ''' Hierzu immer direkt auf keinSpielGeladen = SFDat.xMax = 0 prüfen.
        ''' </summary>
        ''' <returns></returns>
        Public Function SpielfeldIsEmpty() As Boolean
            With SFDat.SFInf
                If IsNothing(SFDat) Then
                    Return True
                ElseIf .xMax = 0 Then
                    Return True
                ElseIf RenderMode = RenderMode.NoRendering Then
                    Return True
                Else
                    Return False
                End If
            End With
        End Function
        Public Function SpielfeldIsNotEmpty() As Boolean
            Return Not SpielfeldIsEmpty()
        End Function
        Public Sub CloseToolBox()

            Dim frm As Form = INI.ToolBox_FormToolBox

            If frm Is Nothing Then
                INI.ToolBox_FormIsVisible = False
                Exit Sub
            End If

            Try
                If Not frm.IsDisposed Then
                    frm.Close()
                End If
            Catch
                If Not frm.IsDisposed Then
                    frm.Dispose()
                End If
            End Try

            INI.ToolBox_FormToolBox = Nothing
            INI.ToolBox_FormIsVisible = False

        End Sub

        'Ist das korrekt?. 
        '
        ''' <summary>
        ''' 
        ''' </summary>
        Public Sub SFDatDisposeAndSetNothing(saveSpielFeld As Boolean)

            If saveSpielFeld AndAlso SFDat IsNot Nothing Then
                SFDat.SFInf.SaveLastUsedSpielfeld()
            End If

            If SFDat IsNot Nothing Then

                SFDat.Dispose()
                SFDat = Nothing
            End If

            _RenderMode = RenderMode.NoRendering

        End Sub
        '

        '
        ''' <summary>
        ''' Disposed alles Notwendige in allen SF-Klassen und Abhängigen.
        ''' Von außen SFMain.CloseSpielfeld verwenden.
        ''' </summary>
        Private Sub Dispose()
            If SFDat IsNot Nothing Then
                SFDat.Dispose()
                SFDat = Nothing
            End If
        End Sub

    End Module
End Namespace