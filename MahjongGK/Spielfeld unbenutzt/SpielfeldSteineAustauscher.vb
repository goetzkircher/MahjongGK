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

#Disable Warning IDE0079
#Disable Warning IDE1006

''' <summary>
''' Die Funktionalität kann später noch ergänzt werden
''' insbesondere darauf, daß keine gleichen Steine 
''' in der untersten und zweituntersten Schicht genau 
''' oder versetzt aufeinander liegen.
''' </summary>
Public Class SpielfeldSteineAustauscher

    Sub New()

    End Sub
    '
    ''' <summary>
    ''' KomplettJob, d.h. alle Steine werden ausgetauscht.
    ''' Das Flag SteinInfo.IsWerkstattStein wird zurückgestellt.
    ''' </summary>
    ''' <param name="steininfos"></param>
    ''' <param name="vorrat"></param>
    Sub New(steininfos As List(Of Spielfeld.SteinInfo), vorrat As List(Of SteinIndexEnum))
        Me._steininfos = steininfos
        Me.vorrat = vorrat
        _werkstattJob = False
        DoJobKompletJob()
    End Sub
    '
    ''' <summary>
    ''' WerkbankJob, d.h. nur die Steine, die von der Werkbank hinzugefügt wurden,
    ''' werden ausgetauscht. Die Werkbank setzt das Flag SteinInfo.IsWerkstattStein.
    ''' 1. SteineNeeded aufrufen 2. Steine organisieren 3. Steine mit SetSteinVorrat
    ''' übergeben 4. ResultOK abfragen, 5. ggf ChangedSteinInfos wieder abrufen.
    ''' Der entsprechenden Steine sind dann geändert und die Flags rückgestellt.
    ''' Oder: Als Werkstück aus der Werkbank abholen. Der Werkstück
    ''' hat die gleichen Möglichkeiten.
    ''' </summary>
    ''' <param name="steininfos"></param>
    Sub New(steininfos As List(Of Spielfeld.SteinInfo))
        Me._steininfos = steininfos
        _werkstattJob = True
    End Sub

    Private ReadOnly _steininfos As List(Of Spielfeld.SteinInfo)
    Private vorrat As List(Of SteinIndexEnum)

    Private _resultOK As Boolean = False
    Private ReadOnly _werkstattJob As Boolean

    ''' <summary>
    ''' Das Ergebnis ist im KomplettJob immer OK, wenn weder steininfos noch 
    ''' Vorrat Nothing sind und beide die gleiche Anzahl von Elementen haben.
    ''' Im Werkstattjob immer abfragen.
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property ResultOK As Boolean
        Get
            Return _resultOK
        End Get
    End Property
    '
    ''' <summary>
    ''' Gibt die geänderte  List(Of SteinInfo) zurück.
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property ChangedSteinInfos As List(Of Spielfeld.SteinInfo)
        Get
            Return _steininfos
        End Get
    End Property

    Private Sub DoJobKompletJob()

        If IsNothing(_steininfos) Then _resultOK = False
        If IsNothing(vorrat) Then _resultOK = False
        If _steininfos.Count = 0 Then _resultOK = False
        If vorrat.Count = 0 Then _resultOK = False
        If vorrat.Count <> _steininfos.Count Then _resultOK = False

        If _resultOK = False Then
            Exit Sub
        End If

        For idx As Integer = 0 To _steininfos.Count - 1
            With _steininfos(idx)
                .SteinIndex = vorrat(idx)
                .SteinStatusIst = SteinStatus.I01Normal
                .IsWerkbankStein = False
            End With
        Next

        _resultOK = True

    End Sub
    '
    ''' <summary>
    ''' Gibt die Anzahl der Steine zurück, ausgetauscht werden müssen. 
    ''' </summary>
    ''' <returns></returns>
    Public Function SteineNeeded() As Integer
        If _werkstattJob Then
            Dim iCount As Integer
            For idx As Integer = 0 To _steininfos.Count - 1
                If _steininfos(idx).IsWerkbankStein Then
                    iCount += 1
                End If
            Next
            Return iCount
        Else
            Return _steininfos.Count
        End If
    End Function
    '
    ''' <summary>
    ''' Übergibt den Steinvorrat und startet den Austauschvorgang.
    ''' Vorher CountSteineNeeded aufrufen und die genaue Anzahl von 
    ''' Steinen im Vorrat übergeben.
    ''' </summary>
    ''' <param name="vorrat"></param>
    Public Sub SetSteinVorrat(vorrat As List(Of SteinIndexEnum))
        Me.vorrat = vorrat
        DoJobWerkstattJob()
    End Sub

    Private Sub DoJobWerkstattJob()
        Dim idxSchlepp As Integer = 0
        Try
            For idx As Integer = 0 To _steininfos.Count - 1
                With _steininfos(idx)
                    If _steininfos(idx).IsWerkbankStein Then
                        .SteinIndex = vorrat(idxSchlepp)
                        idxSchlepp += 1
                        .SteinStatusIst = SteinStatus.I01Normal
                        .IsWerkbankStein = False
                    End If
                End With
            Next

        Catch ex As Exception
            _resultOK = False
            Exit Sub
        End Try

        _resultOK = True

    End Sub

End Class
