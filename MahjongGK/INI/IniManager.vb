'
' SPDX-License-Identifier: GPL-3.0-or-later
'############################################################################
'#                                                                          #
'#   Copyright © 2025–2026 Götz Kircher <mahjonggk@t-online.de>             #
'#                                                                          #
'#                     MahjongGK  -  Mahjong Solitär                        #
'#                                                                          #
'#   This program is free software: you can redistribute it and/or modify   #
'#   it under the terms of the GNU General Public License as published by   #
'#   the Free Software Foundation, either version 3 of the License, or      #
'#   at your option any later version.                                      #
'#                                                                          #
'#   This program is distributed in the hope that it will be useful,        #
'#   but WITHOUT ANY WARRANTY; without even the implied warranty of         #
'#   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the           #
'#   GNU General Public License for more details.                           #
'#   https://www.gnu.org/licenses/gpl-3.0.html                              #
'#                                                                          #   
'#   This project uses:                                                     #
'#   Magick.NET (Apache License 2.0 https://github.com/dlemstra/Magick.NET) #
'#   SVG.NET (MS-PL-Lizenz https://github.com/svg-net/SVG)                  #
'#                                                                          #
'############################################################################
'
'
Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

#Disable Warning IDE0079
#Disable Warning IDE1006

Imports System.Globalization
Imports System.IO
Imports System.Text
Imports System.Threading
Public Enum IniEvents
    None
    OnChangeValue
    OnWriteValue
    OnUpdate
End Enum

Public Class IniManager
    Implements IDisposable

    Private ReadOnly _fileFullPath As String
    Private _iniLines As New List(Of String)
    Private _raiseIniEvents As IniEvents
    Private _raiseIniEventsSic As IniEvents
    Public Property InitialisierungAktiv As Boolean
    ''' <summary>
    ''' Für Debugzwecke der Dateiname mit Extension der Ini.
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property Name As String

    Public ReadOnly Property FileFullPath As String
        Get
            Return _fileFullPath
        End Get
    End Property

    Public ReadOnly Property FileFullTmpPath As String
        Get
            Return _fileFullPath & ".tmp"
        End Get
    End Property

    Public Property IniLines As List(Of String)
        Get
            Return _iniLines
        End Get
        Set(value As List(Of String))
            _iniLines = value
        End Set
    End Property

    Public Sub New(fileName_ext As String)
        _fileFullPath = AppDataFileINI(fileName_ext)
        Name = fileName_ext
        'In die Liste in INI eintragen
        AllIniManagers.Add(Me)
        Try
            'sicherstellen, daß die nicht versehentlich zum UpDaten genommen wird.
            '(schwer zu findender Fehler)
            File.Delete(_fileFullPath & ".tmp")
        Catch ex As Exception

        End Try
        Load(loadTmpFile:=False)
    End Sub
    '

#Region "Verschiedene öffentliche Member"

    Public Sub LoadTmpFile()
        Load(loadTmpFile:=True)
    End Sub
    Public Sub LoadOrgFile()
        Load(loadTmpFile:=False)
    End Sub

    ''' <summary>
    ''' IniEvents hat die Werte: None, OnChangeValue, OnWriteValue, OnUpdate. Je nach Wert geben die Überladungen
    ''' WriteValue True oder False zurück. None gibt immer False zurück. Grundeinstellung bei der InitDragDropBitmaps.
    ''' 
    ''' </summary>
    ''' <returns></returns>
    Public Property RaiseIniEvents As IniEvents
        Get
            Return _raiseIniEvents
        End Get
        Set(value As IniEvents)
            _raiseIniEvents = value
        End Set
    End Property

    Public Sub BackupValueRaiseIniEvents(Optional newValue As IniEvents = CType(-1, IniEvents))

        _raiseIniEventsSic = _raiseIniEvents
        If newValue <> -1 Then
            _raiseIniEvents = newValue
        End If

    End Sub

    Public Sub RestoreValueRaiseIniEvents()
        _raiseIniEvents = _raiseIniEventsSic
    End Sub

    Public ReadOnly Property FullPath As String
        Get
            Return _fileFullPath
        End Get
    End Property

#End Region

#Region "Write/Read"

    ' Für die Standardwerte: 
    ' Decimal, Double, Single, Long, Integer,
    ' Boolean
    ' Color
    ' Size, Point, Rectangle
    ' SizeF, PointF, RectangleF
    ' String, Char
    ' Font
    ' gibt es passsende Überladungen, die gleich
    ' das richtige Format zurückgeben.
    ' Für alle anderen Werte sind die Konvertierungsfunktionen Cvt...
    ' zu ergänzen, die sich ganz unten am Ende des Moduls befinden,
    ' und und die Write- und Read Funktionen zu ergänzen.

#Disable Warning IDE0079 ' Unnötige Unterdrückung entfernen
#Disable Warning IDE0051 ' Nicht verwendete private Member entfernen

    '
    '--- String
    Public Function WriteValue(folderAndKey As (folder As String, key As String), ByVal value As String) As Boolean
        Return WriteValueToINI(folderAndKey, value)
    End Function

    Public ReadOnly Property ReadValue(folderAndKey As (folder As String, key As String), [default] As String, comment As String) As String
        Get
            Try
                Return ReadValueFromINI(folderAndKey, [default], comment)
            Catch ex As Exception
                Return [default]
            End Try
        End Get
    End Property
    '
    'Sonderfall String als Path
    '--- String
    Public Function WritePath(folderAndKey As (folder As String, key As String), ByVal value As String) As Boolean
        Return WriteValueToINI(folderAndKey, CvtPathToRelPath(value))
    End Function

    Public ReadOnly Property ReadPath(folderAndKey As (folder As String, key As String), [default] As String, comment As String) As String
        Get
            Try
                Return CvtRelPathToPath(ReadValueFromINI(folderAndKey, CvtPathToRelPath([default]), comment))
            Catch ex As Exception
                Return [default]
            End Try
        End Get
    End Property

    '
    '--- Char
    Public Function WriteValue(folderAndKey As (folder As String, key As String), ByVal value As Char) As Boolean
        Return WriteValueToINI(folderAndKey, value.ToString)
    End Function

    Public ReadOnly Property ReadValue(folderAndKey As (folder As String, key As String), [default] As Char, comment As String) As Char
        Get
            Try
                Dim s As String = ReadValueFromINI(folderAndKey, [default].ToString, comment)
                If String.IsNullOrEmpty(s) Then
                    Return [default]
                Else
                    Return s(0) 'nur das erste Zeichen
                End If
            Catch ex As Exception
                Return [default]
            End Try
        End Get
    End Property
    '
    '--- Decimal
    Public Function WriteValue(folderAndKey As (folder As String, key As String), ByVal value As Decimal) As Boolean
        Return WriteValueToINI(folderAndKey, CvtDecimalToString(value))
    End Function
    Public ReadOnly Property ReadValue(folderAndKey As (folder As String, key As String), [default] As Decimal, comment As String) As Decimal
        Get
            Try
                Return CvtStringToDecimal(ReadValueFromINI(folderAndKey, CvtDecimalToString([default]), comment), [default])
            Catch ex As Exception
                Return [default]
            End Try

        End Get
    End Property
    '
    '--- Double
    Public Function WriteValue(folderAndKey As (folder As String, key As String), ByVal value As Double) As Boolean
        Return WriteValueToINI(folderAndKey, CvtDoubleToString(value))
    End Function
    Public ReadOnly Property ReadValue(folderAndKey As (folder As String, key As String), [default] As Double, comment As String) As Double

        Get
            Try
                Return CvtStringToDouble(ReadValueFromINI(folderAndKey, CvtDoubleToString([default]), comment), [default])
            Catch ex As Exception
                Return [default]
            End Try

        End Get
    End Property
    '
    ' --- Single
    Public Function WriteValue(folderAndKey As (folder As String, key As String), ByVal value As Single) As Boolean
        Return WriteValueToINI(folderAndKey, CvtSingleToString(value))
    End Function
    Public ReadOnly Property ReadValue(folderAndKey As (folder As String, key As String), [default] As Single, comment As String) As Single
        Get
            Try
                Return CvtStringToSingle(ReadValueFromINI(folderAndKey, CvtSingleToString([default]), comment), [default])
            Catch ex As Exception
                Return [default]
            End Try

        End Get
    End Property
    '
    'Long
    Public Function WriteValue(folderAndKey As (folder As String, key As String), ByVal value As Long) As Boolean
        Return WriteValueToINI(folderAndKey, CvtLongToString(value))
    End Function

    Public ReadOnly Property ReadValue(folderAndKey As (folder As String, key As String), [default] As Long, comment As String) As Long
        Get
            Try
                Return CvtStringToLong(ReadValueFromINI(folderAndKey, CvtLongToString([default]), comment), [default])
            Catch ex As Exception
                Return [default]
            End Try

        End Get
    End Property
    '
    ' Integer
    Public Function WriteValue(folderAndKey As (folder As String, key As String), ByVal value As Integer) As Boolean
        Return WriteValueToINI(folderAndKey, CvtIntegerToString(value))
    End Function

    Public ReadOnly Property ReadValue(folderAndKey As (folder As String, key As String), [default] As Integer, comment As String) As Integer
        Get
            Try
                Return CvtStringToInteger(ReadValueFromINI(folderAndKey, CvtIntegerToString([default]), comment), [default])
            Catch ex As Exception
                Return [default]
            End Try

        End Get
    End Property

    ' byte
    Public Function WriteValue(folderAndKey As (folder As String, key As String), ByVal value As Byte) As Boolean
        Return WriteValueToINI(folderAndKey, CvtIntegerToString(value))
    End Function

    Public ReadOnly Property ReadValue(folderAndKey As (folder As String, key As String), [default] As Byte, comment As String) As Byte
        Get
            Try
                Dim value As Integer = CvtStringToInteger(ReadValueFromINI(folderAndKey, CvtIntegerToString([default]), comment), [default])
                If value < Byte.MinValue OrElse value > Byte.MaxValue Then
                    Return [default]
                Else
                    Return CByte(value)
                End If
            Catch ex As Exception
                Return [default]
            End Try

        End Get
    End Property
    '
    '--- Boolean
    Public Function WriteValue(folderAndKey As (folder As String, key As String), ByVal value As Boolean) As Boolean
        Return WriteValueToINI(folderAndKey, CvtBooleanToString(value))
    End Function
    Public ReadOnly Property ReadValue(folderAndKey As (folder As String, key As String), [default] As Boolean, comment As String) As Boolean
        Get
            Try
                Return CvtStringToBoolean(ReadValueFromINI(folderAndKey, CvtBooleanToString([default]), comment), [default])
            Catch ex As Exception
                Return [default]
            End Try

        End Get
    End Property

    '
    '--- Color
    Public Function WriteValue(folderAndKey As (folder As String, key As String), ByVal value As Color) As Boolean
        Return WriteValueToINI(folderAndKey, CvtColorToHexString(value))
    End Function

    Public ReadOnly Property ReadValue(folderAndKey As (folder As String, key As String), [default] As Color, comment As String) As Color
        Get
            Try
                Return CvtHexStringToColor(ReadValueFromINI(folderAndKey, CvtColorToHexString([default]), comment), [default])
            Catch ex As Exception
                Return [default]
            End Try
        End Get
    End Property
    '
    '--- Date
    Public Function WriteValue(folderAndKey As (folder As String, key As String), ByVal value As Date) As Boolean
        Return WriteValueToINI(folderAndKey, CvtDateToString(value))
    End Function

    Public ReadOnly Property ReadValue(folderAndKey As (folder As String, key As String), [default] As Date, comment As String) As Date
        Get
            Try
                Return CvtStringToDate(ReadValueFromINI(folderAndKey, CvtDateToString([default]), comment), [default])
            Catch ex As Exception
                Return [default]
            End Try
        End Get
    End Property
    '
    '--- Point
    Public Function WriteValue(folderAndKey As (folder As String, key As String), value As Point) As Boolean
        Return WriteValueToINI(folderAndKey, CvtPointToString(value))
    End Function

    Public ReadOnly Property ReadValue(folderAndKey As (folder As String, key As String), [default] As Point, comment As String) As Point
        Get
            Try
                Return CvtStringToPoint(ReadValueFromINI(folderAndKey, CvtPointToString([default]), comment), [default])
            Catch ex As Exception
                Return [default]
            End Try
        End Get
    End Property
    '
    '--- PointF
    Public Function WriteValue(folderAndKey As (folder As String, key As String), value As PointF) As Boolean
        Return WriteValueToINI(folderAndKey, CvtPointFToString(value))
    End Function

    Public ReadOnly Property ReadValue(folderAndKey As (folder As String, key As String), [default] As PointF, comment As String) As PointF
        Get
            Try
                Return CvtStringToPointF(ReadValueFromINI(folderAndKey, CvtPointFToString([default]), comment), [default])
            Catch ex As Exception
                Return [default]
            End Try
        End Get
    End Property
    '
    '--- Size
    Public Function WriteValue(folderAndKey As (folder As String, key As String), value As Size) As Boolean
        Return WriteValueToINI(folderAndKey, CvtSizeToString(value))
    End Function

    Public ReadOnly Property ReadValue(folderAndKey As (folder As String, key As String), [default] As Size, comment As String) As Size
        Get
            Try
                Return CvtStringToSize(ReadValueFromINI(folderAndKey, CvtSizeToString([default]), comment), [default])
            Catch ex As Exception
                Return [default]
            End Try
        End Get
    End Property
    '
    '--- SizeF
    Public Function WriteValue(folderAndKey As (folder As String, key As String), value As SizeF) As Boolean
        Return WriteValueToINI(folderAndKey, CvtSizeFToString(value))
    End Function

    Public ReadOnly Property ReadValue(folderAndKey As (folder As String, key As String), [default] As SizeF, comment As String) As SizeF
        Get
            Try
                Return CvtStringToSizeF(ReadValueFromINI(folderAndKey, CvtSizeFToString([default]), comment), [default])
            Catch ex As Exception
                Return [default]
            End Try
        End Get
    End Property
    '
    '--- Rectangle
    Public Function WriteValue(folderAndKey As (folder As String, key As String), value As Rectangle) As Boolean
        Return WriteValueToINI(folderAndKey, CvtRectToString(value))
    End Function
    Public ReadOnly Property ReadValue(folderAndKey As (folder As String, key As String), [default] As Rectangle, comment As String) As Rectangle
        Get
            Try
                Return CvtStringToRect(ReadValueFromINI(folderAndKey, CvtRectToString([default]), comment), [default])
            Catch ex As Exception
                Return [default]
            End Try
        End Get
    End Property
    '
    '--- RectangleF
    Public Function WriteValue(folderAndKey As (folder As String, key As String), value As RectangleF) As Boolean
        Return WriteValueToINI(folderAndKey, CvtRectFToString(value))
    End Function
    Public ReadOnly Property ReadValue(folderAndKey As (folder As String, key As String), [default] As RectangleF, comment As String) As RectangleF
        Get
            Try
                Return CvtStringToRectF(ReadValueFromINI(folderAndKey, CvtRectFToString([default]), comment), [default])
            Catch ex As Exception
                Return [default]
            End Try
        End Get
    End Property
    '
    '--- Font
    Public Function WriteValue(folderAndKey As (folder As String, key As String), value As Font) As Boolean
        Return WriteValueToINI(folderAndKey, CvtFontToString(value))
    End Function
    Public ReadOnly Property ReadValue(folderAndKey As (folder As String, key As String), [default] As Font, comment As String) As Font
        Get
            Try
                Return CvtStringToFont(ReadValueFromINI(folderAndKey, CvtFontToString([default]), comment), [default])
            Catch ex As Exception
                Return [default]
            End Try
        End Get
    End Property

    '
    '--- PaddingValues
    Public Function WriteValue(folderAndKey As (folder As String, key As String), value As PaddingValues) As Boolean
        Return WriteValueToINI(folderAndKey, CvtPaddingToString(value))
    End Function

    Public ReadOnly Property ReadValue(folderAndKey As (folder As String, key As String), [default] As PaddingValues, comment As String) As PaddingValues
        Get
            Try
                Dim raw As String = ReadValueFromINI(folderAndKey, CvtPaddingToString([default]), comment)
                Return CvtStringToPadding(raw, [default])
            Catch ex As Exception
                Return [default]
            End Try
        End Get
    End Property

    '
    '--- Tripl
    Public Function WriteValue(folderAndKey As (folder As String, key As String), value As Triple) As Boolean
        Return WriteValueToINI(folderAndKey, CvtTripleToString(value))
    End Function
    Public ReadOnly Property ReadValue(folderAndKey As (folder As String, key As String), [default] As Triple, comment As String) As Triple
        Get
            Try
                Return CvtStringToTriple(ReadValueFromINI(folderAndKey, CvtTripleToString([default]), comment), [default])
            Catch ex As Exception
                Return [default]
            End Try
        End Get
    End Property

#End Region

#Region "Lesen und schreiben - zentrale Routinen"

    '
    ''' <summary>
    ''' Liefert die Zeilennummer der Folder-Überschrift "[Folder]" oder -1.
    ''' </summary>
    Private Function FindFolderLine(folder As String) As Integer
        Dim search As String = "[" & folder & "]"
        For i As Integer = 0 To _iniLines.Count - 1
            If _iniLines(i).Equals(search, StringComparison.OrdinalIgnoreCase) Then
                Return i
            End If
        Next
        Return -1
    End Function

    '
    ''' <summary>
    ''' Liefert die Zeilennummer der Key-Zeile "key=..." innerhalb des Folders oder -1.
    ''' </summary>
    Private Function FindKeyLine(folderAndKey As (folder As String, key As String)) As Integer
        Dim folderLine As Integer = FindFolderLine(folderAndKey.folder)
        If folderLine < 0 Then Return -1

        For i As Integer = folderLine + 1 To _iniLines.Count - 1
            Dim l As String = _iniLines(i)
            If l.StartsWith("[") Then Return -1 ' nächster Abschnitt erreicht
            If l.StartsWith(folderAndKey.key & "=", StringComparison.OrdinalIgnoreCase) Then
                Return i
            End If
        Next
        Return -1
    End Function

    ' ---------- Kommentar-Handling (neu) ----------

    '
    ''' <summary>
    ''' Baut aus dem Kommentartext (Zeilentrenner "~") die kommentierten Zeilen inkl. Spacer.
    ''' Leere oder Nothing -> leere Liste (kein Kommentar).
    ''' </summary>
    Private Function BuildCommentBlockLines(comment As String) As List(Of String)
        Dim result As New List(Of String)
        If String.IsNullOrWhiteSpace(comment) Then
            Return result
        End If

        ' Abstand zum vorherigen Eintrag (auskommentierte Leerzeile)
        result.Add(";")

        ' Mehrzeilig via "~"
        Dim parts As String() = comment.Split(New String() {"~"}, StringSplitOptions.None)
        For Each p As String In parts
            ' Keine Farben/Meta – einfach als ";"-Kommentar schreiben
            result.Add(";" & p.TrimEnd)
        Next
        Return result
    End Function

    '
    ''' <summary>
    ''' Findet die zum Key gehörige Kommentar-Blockregion (zusammenhängende ";"-Zeilen direkt über dem Key).
    ''' Gibt (start,end) zurück – inkl. Spacer falls vorhanden. Nichts gefunden -> (-1,-1).
    ''' </summary>
    Private Function FindCommentBlockForKey(keyLine As Integer, folderLine As Integer) As (start As Integer, [end] As Integer)
        If keyLine <= 0 Then Return (-1, -1)

        Dim first As Integer = keyLine - 1
        ' rückwärts alle Kommentarzeilen aufsammeln
        While first > folderLine
            Dim s As String = _iniLines(first)
            If s.TrimStart().StartsWith(";", StringComparison.Ordinal) Then
                first -= 1
            Else
                Exit While
            End If
        End While

        first += 1 ' auf die erste Kommentarzeile vor dem Key setzen
        Dim last As Integer = keyLine - 1

        If first > last Then
            Return (-1, -1)
        End If

        ' Sicherheit: prüfen, ob es wirklich Kommentar ist
        If Not _iniLines(first).TrimStart().StartsWith(";", StringComparison.Ordinal) Then
            Return (-1, -1)
        End If

        Return (first, last)
    End Function

    '
    ''' <summary>
    ''' Stellt sicher, dass der Kommentarblock über dem Key dem gewünschten Kommentar entspricht.
    ''' - Bei leerem gewünschtem Kommentar wird ein bestehender Block entfernt.
    ''' - Bei abweichendem Kommentar wird ersetzt.
    ''' - Bei fehlendem Kommentar wird eingefügt.
    ''' </summary>
    Private Sub EnsureCommentForKey(desiredComment As String,
                                keyLine As Integer,
                                folderLine As Integer)

        Dim desired As List(Of String) = BuildCommentBlockLines(desiredComment)
        Dim region As (start As Integer, [end] As Integer) = FindCommentBlockForKey(keyLine, folderLine)

        If desired.Count = 0 Then
            ' Kommentar soll nicht existieren -> vorhandenen entfernen
            If region.start <> -1 Then
                _iniLines.RemoveRange(region.start, region.[end] - region.start + 1)
                MarkChanged()
            End If
            Return
        End If

        If region.start = -1 Then
            ' Kein Kommentar vorhanden -> einfügen vor dem Key
            _iniLines.InsertRange(keyLine, desired)
            MarkChanged()
            Return
        End If

        ' Vergleich: ist der Block identisch?
        Dim count As Integer = region.[end] - region.start + 1
        Dim equal As Boolean = (count = desired.Count)
        If equal Then
            equal = True 'Default
            For i As Integer = 0 To count - 1
                If Not String.Equals(_iniLines(region.start + i), desired(i), StringComparison.Ordinal) Then
                    equal = False
                    Exit For
                End If
            Next
        End If

        If Not equal Then
            ' Ersetzen
            _iniLines.RemoveRange(region.start, count)
            _iniLines.InsertRange(region.start, desired)
            MarkChanged()
        End If
    End Sub

    ' ---------- Lesen / Schreiben mit Kommentarpflege ----------

    '
    ''' <summary>
    ''' Liest einen Wert aus – oder legt ihn (inkl. Kommentar, wenn initialisierungAktiv=True) mit Default neu an.
    ''' </summary>
    Private Function ReadValueFromINI(folderAndKey As (folder As String, key As String),
                                  defaultValue As String,
                                  comment As String) As String

        Dim idx As Integer = FindKeyLine(folderAndKey)

        If idx < 0 Then
            ' Key existiert nicht -> neu anlegen (falls InitDragDropBitmaps gewünscht inkl. Kommentar)
            WriteValueToINI(folderAndKey, defaultValue, comment)
            Return defaultValue
        End If

        ' Beim Initialisierungslauf Kommentare synchronisieren (ändern/löschen/anlegen)
        If InitialisierungAktiv Then
            Dim folderLine As Integer = FindFolderLine(folderAndKey.folder)
            If folderLine >= 0 Then
                EnsureCommentForKey(comment, idx, folderLine)
                ' Achtung: EnsureCommentForKey kann vor idx eingefügt/entfernt haben -> keyLine evtl. verschoben.
                ' Für das reine Lesen ist das egal, wir lesen die Zeile neu:
                idx = FindKeyLine(folderAndKey)
            End If
        End If

        Dim line As String = _iniLines(idx)
        Dim pos As Integer = line.IndexOf("="c)
        If pos < 0 OrElse pos = line.Length - 1 Then
            Return ""
        End If

        Return CvtINIValueToStringValue(line.Substring(pos + 1), defaultValue)

    End Function

    '
    ''' <summary>
    ''' Schreibt einen Wert (fügt an, falls er nicht existiert). Beim Initialisierungslauf werden Kommentare gepflegt.
    ''' Gibt True zurück, wenn sich er Wert geändert hat UND _DontRaiseChangedEvent auf False steht.
    ''' Steht _DontRaiseChangedEvent auf True, wird immer False zurüchkegeben.
    ''' </summary>
    Private Function WriteValueToINI(folderAndKey As (folder As String, key As String),
                                value As String,
                                Optional comment As String = Nothing) As Boolean

        value = CvtStringValueToINIValue(value)

        Dim folderLine As Integer = FindFolderLine(folderAndKey.folder)

        ' Folder nicht vorhanden -> neu anlegen
        If folderLine < 0 Then
            _iniLines.Add("[" & folderAndKey.folder & "]")
            If InitialisierungAktiv Then
                Dim block As List(Of String) = BuildCommentBlockLines(comment)
                If block.Count > 0 Then
                    _iniLines.AddRange(block)
                End If
            End If
            _iniLines.Add($"{folderAndKey.key}={value}")
            MarkChanged()
            Return Announce(True)
        End If

        ' Schlüssel im Folder suchen
        For i As Integer = folderLine + 1 To _iniLines.Count - 1
            Dim l As String = _iniLines(i)

            ' Leere oder Kommentarzeilen überspringen (gehören zu vorherigen Keys)
            If String.IsNullOrWhiteSpace(l) OrElse l.TrimStart().StartsWith(";") Then
                Continue For
            End If

            If l.StartsWith("[") Then
                ' nächster Abschnitt erreicht -> hier einfügen
                Dim insertAt As Integer = i
                If InitialisierungAktiv Then
                    Dim block As List(Of String) = BuildCommentBlockLines(comment)
                    If block.Count > 0 Then
                        _iniLines.InsertRange(insertAt, block)
                        insertAt += block.Count
                    End If
                End If
                _iniLines.Insert(insertAt, $"{folderAndKey.key}={value}")
                MarkChanged()
                Return Announce(True)
            End If

            If l.StartsWith(folderAndKey.key & "=", StringComparison.OrdinalIgnoreCase) Then
                ' Key existiert -> Wert ersetzen
                Dim newValue As String = $"{folderAndKey.key}={value}"
                Dim changed As Boolean
                'abr nur, wenn er sich geändert hat.
                If String.Compare(_iniLines(i), newValue, ignoreCase:=False) <> 0 Then
                    _iniLines(i) = newValue
                    MarkChanged()
                    changed = True
                Else
                    changed = False
                End If

                ' Kommentare ggf. pflegen
                If InitialisierungAktiv Then
                    EnsureCommentForKey(comment, i, folderLine)
                End If

                Return Announce(changed)

            End If
        Next

        ' Ende des Folders erreicht -> am Schluss anhängen
        Dim appendAt As Integer = _iniLines.Count
        If InitialisierungAktiv Then
            Dim block As List(Of String) = BuildCommentBlockLines(comment)
            If block.Count > 0 Then
                _iniLines.AddRange(block)
            End If
        End If
        _iniLines.Add($"{folderAndKey.key}={value}")
        MarkChanged()

        Return Announce(True)

    End Function

    Private Function Announce(changed As Boolean) As Boolean

        Select Case _raiseIniEvents
            Case IniEvents.None
                Return False
            Case IniEvents.OnChangeValue
                Return changed
            Case IniEvents.OnUpdate
                Return True
            Case IniEvents.OnWriteValue
                Return True
            Case Else
                Stop 'Programmierfehler
                Return False
        End Select

    End Function

#End Region

#Region "Konvertierungen"

    'In die INI werden nur einzeilige Strings geschrieben, hier sind die Konvertierungsroutinen.
    'Den "zurück"-Konvertierungen wird immer ein Default mitgegeben.
    'Das ist als Sicherungsnetz zu verstehen für den Fall, daß der Anwender in der INI herumpfuscht
    'und Werte nicht mehr lesbar sind.

    Public Function CvtIntegerToString(value As Integer) As String
        Return value.ToString(CultureInfo.InvariantCulture)
    End Function

    Public Function CvtStringToInteger(s As String, [default] As Integer) As Integer
        Dim result As Integer
        If Integer.TryParse(s, NumberStyles.Integer,
                            CultureInfo.InvariantCulture, result) Then
            Return result
        End If
        Return [default]
    End Function

    Public Function CvtLongToString(value As Long) As String
        Return value.ToString(CultureInfo.InvariantCulture)
    End Function

    Public Function CvtStringToLong(s As String, [default] As Long) As Long
        Dim result As Long
        If Long.TryParse(s, NumberStyles.Integer,
                         CultureInfo.InvariantCulture, result) Then
            Return result
        End If
        Return [default]
    End Function

    '
    ' === Boolean ===
    ' Speicherung als "True"/"False" (invariant)
    '
    Public Function CvtBooleanToString(value As Boolean) As String
        Return value.ToString(CultureInfo.InvariantCulture)
    End Function

    Public Function CvtStringToBoolean(s As String, [default] As Boolean) As Boolean
        Dim result As Boolean
        If Boolean.TryParse(s, result) Then
            Return result
        End If
        Return [default]
    End Function

    Private Const COLOR_EMPTY_STRING As String = "EMPTY"

    ''' <summary>
    ''' Wandelt eine <see cref="Color"/> in einen String um.
    ''' Color.Empty wird als "EMPTY" kodiert; ansonsten AARRGGBB (Großbuchstaben).
    ''' </summary>
    Public Shared Function CvtColorToHexString(color As Color) As String
        If color.IsEmpty Then
            Return COLOR_EMPTY_STRING
        End If

        ' AARRGGBB
        Return color.A.ToString("X2", CultureInfo.InvariantCulture) &
               color.R.ToString("X2", CultureInfo.InvariantCulture) &
               color.G.ToString("X2", CultureInfo.InvariantCulture) &
               color.B.ToString("X2", CultureInfo.InvariantCulture)
    End Function

    ''' <summary>
    ''' Überladung ohne Default. (nötig, um den Default festzulegen)
    ''' Der Default des Default ist Color.Empty
    ''' </summary>
    ''' <param name="hex"></param>
    ''' <returns></returns>
    Public Shared Function CvtHexStringToColor(hex As String) As Color
        Return CvtHexStringToColor(hex, Color.Empty)
    End Function
    ''' <summary>
    ''' Wandelt einen String nach AARRGGBB / RRGGBB / "EMPTY" in eine <see cref="Color"/>.
    ''' Bei Fehlern wird <paramref name="[default]"/> zurückgegeben.
    ''' </summary>
    Public Shared Function CvtHexStringToColor(hex As String, [default] As Color) As Color
        Try
            If String.IsNullOrWhiteSpace(hex) Then
                Return [default]
            End If

            Dim s As String = hex.Trim()

            ' Sonderfall "EMPTY"
            If s.Equals(COLOR_EMPTY_STRING, StringComparison.OrdinalIgnoreCase) Then
                Return Color.Empty
            End If

            ' optionale Präfixe entfernen
            If s.StartsWith("#") Then s = s.Substring(1)
            If s.StartsWith("0x", StringComparison.OrdinalIgnoreCase) Then s = s.Substring(2)

            Select Case s.Length
                Case 8
                    ' AARRGGBB
                    Dim val As Integer = Integer.Parse(s, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture)
                    Dim a As Integer = (val >> 24) And &HFF
                    Dim r As Integer = (val >> 16) And &HFF
                    Dim g As Integer = (val >> 8) And &HFF
                    Dim b As Integer = val And &HFF
                    Return Color.FromArgb(a, r, g, b)

                Case 6
                    ' RRGGBB => Alpha=FF
                    Dim val As Integer = Integer.Parse(s, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture)
                    Dim r As Integer = (val >> 16) And &HFF
                    Dim g As Integer = (val >> 8) And &HFF
                    Dim b As Integer = val And &HFF
                    Return Color.FromArgb(&HFF, r, g, b)

                Case Else
                    ' ungültige Länge
                    Return [default]
            End Select

        Catch
            Return [default]
        End Try
    End Function

    '==========================================================
    ' Zahlen (Single/Double/Decimal) – InvariantCulture
    '==========================================================
    Public Function CvtSingleToString(value As Single) As String
        Return value.ToString(CultureInfo.InvariantCulture)
    End Function

    Public Function CvtStringToSingle(s As String, [default] As Single) As Single
        Dim result As Single
        If Single.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, result) Then
            Return result
        End If
        Return [default]
    End Function

    Public Function CvtDoubleToString(value As Double) As String
        Return value.ToString(CultureInfo.InvariantCulture)
    End Function

    Public Function CvtStringToDouble(s As String, [default] As Double) As Double
        Dim result As Double
        If Double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, result) Then
            Return result
        End If
        Return [default]
    End Function

    Public Function CvtDecimalToString(value As Decimal) As String
        Return value.ToString(CultureInfo.InvariantCulture)
    End Function

    Public Function CvtStringToDecimal(s As String, [default] As Decimal) As Decimal
        Dim result As Decimal
        If Decimal.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, result) Then
            Return result
        End If
        Return [default]
    End Function

    '==========================================================
    ' Datum – ISO-8601 Roundtrip ("o") statt "s"
    '==========================================================
    Public Function CvtDateToString(d As Date) As String
        ' "o" bewahrt Millisekunden und Kind (UTC/Local/Unspecified)
        Return d.ToString("o", CultureInfo.InvariantCulture)
    End Function

    Public Function CvtStringToDate(s As String, [default] As Date) As Date
        Dim dt As Date
        If Date.TryParseExact(s, "o", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, dt) Then
            Return dt
        End If
        Return [default]
    End Function

    ' Alternative über Ticks – robust gegen Kultur
    Public Function CvtDateToTicksString(d As Date) As String
        Return d.Ticks.ToString(CultureInfo.InvariantCulture)
    End Function

    Public Function CvtTicksStringToDate(s As String) As Date
        Dim ticks As Long
        If Long.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, ticks) Then
            Return New Date(ticks)
        End If
        ' Fallback: jetzt ist 0 sinnvoller als Exception
        Return New Date(0)
    End Function

    '==========================================================
    ' Font – Semikolon-Format "Name;Size;Style"
    '   (liest zusätzlich alte Komma-Variante)
    '==========================================================
    Public Function CvtFontToString(f As Font) As String
        ' Beispiel: "Arial;8.25;Bold"
        Return String.Format(CultureInfo.InvariantCulture, "{0};{1};{2}", f.Name, f.Size, f.Style)
    End Function

    Public Function CvtStringToFont(s As String, defaultFont As Font) As Font
        Try
            If String.IsNullOrWhiteSpace(s) Then Return defaultFont

            Dim name As String, sizeStr As String, styleStr As String

            If s.Contains(";"c) Then
                ' Neues Format: Name;Size;Style
                Dim parts() As String = s.Split(";"c)
                If parts.Length < 3 Then Return defaultFont
                name = parts(0).Trim()
                sizeStr = parts(1).Trim()
                styleStr = parts(2).Trim()
            Else
                ' Legacy: "Name,8,25,Bold" oder "Name,12,Bold"
                Dim parts() As String = s.Split(","c)
                Select Case parts.Length
                    Case >= 4
                        name = parts(0).Trim()
                        sizeStr = parts(1).Trim() & "."c & parts(2).Trim() ' 8 + 25 -> 8.25 (invariant)
                        styleStr = parts(3).Trim()
                    Case 3
                        name = parts(0).Trim()
                        sizeStr = parts(1).Trim()
                        styleStr = parts(2).Trim()
                    Case Else
                        Return defaultFont
                End Select
            End If

            Dim sizeVal As Single
            If Not Single.TryParse(sizeStr, NumberStyles.Float, CultureInfo.InvariantCulture, sizeVal) Then
                Return defaultFont
            End If

            Dim style As FontStyle
            If Not [Enum].TryParse(styleStr, True, style) Then
                ' Zahl zulassen (z. B. "1")
                Dim styleInt As Integer
                If Integer.TryParse(styleStr, NumberStyles.Integer, CultureInfo.InvariantCulture, styleInt) Then
                    style = CType(styleInt, FontStyle)
                Else
                    style = defaultFont.Style
                End If
            End If

            Return New Font(name, sizeVal, style)
        Catch
            Return defaultFont
        End Try
    End Function

    ''' <summary>Triple ⇒ "x;y;z;Valide" (leer bei Nothing)</summary>
    Public Shared Function CvtTripleToString(value As Triple) As String
        If value Is Nothing Then Return String.Empty
        Return $"{value.x};{value.y};{value.z};{CInt(value.Valide)}"
    End Function

    ''' <summary>"x;y;z;Valide" ⇒ Triple (bei Fehlern/leer: Default)</summary>
    Public Shared Function CvtStringToTriple(s As String, [default] As Triple) As Triple
        If String.IsNullOrWhiteSpace(s) Then Return [default]
        Try
            Dim p() As String = s.Split(";"c)
            If p.Length >= 4 Then
                Dim xi As Integer = Integer.Parse(p(0).Trim())
                Dim yi As Integer = Integer.Parse(p(1).Trim())
                Dim zi As Integer = Integer.Parse(p(2).Trim())
                Dim v As ValidePlace = CType(Integer.Parse(p(3).Trim()), ValidePlace)
                Return New Triple(xi, yi, zi, v)
            End If
        Catch
            ' ignorieren → Default
        End Try
        Return [default]
    End Function

    '==========================================================
    ' Point/Size/Rectangle – Semikolon-Format; int-basiert
    '   (liest zusätzlich alte Komma-Variante)
    '==========================================================
    Public Function CvtPointToString(p As Point) As String
        Return String.Format(CultureInfo.InvariantCulture, "{0};{1}", p.X, p.Y)
    End Function

    Public Function CvtStringToPoint(s As String, [default] As Point) As Point
        Try
            Dim parts() As String = SplitSmart2(s)
            Dim x As Integer = Integer.Parse(parts(0), NumberStyles.Integer, CultureInfo.InvariantCulture)
            Dim y As Integer = Integer.Parse(parts(1), NumberStyles.Integer, CultureInfo.InvariantCulture)
            Return New Point(x, y)
        Catch
            Return [default]
        End Try
    End Function

    Public Function CvtSizeToString(sz As Size) As String
        Return String.Format(CultureInfo.InvariantCulture, "{0};{1}", sz.Width, sz.Height)
    End Function

    Public Function CvtStringToSize(s As String, [default] As Size) As Size
        Try
            Dim parts() As String = SplitSmart2(s)
            Dim w As Integer = Integer.Parse(parts(0), NumberStyles.Integer, CultureInfo.InvariantCulture)
            Dim h As Integer = Integer.Parse(parts(1), NumberStyles.Integer, CultureInfo.InvariantCulture)
            Return New Size(w, h)
        Catch
            Return [default]
        End Try
    End Function

    Public Function CvtRectToString(r As Rectangle) As String
        Return String.Format(CultureInfo.InvariantCulture, "{0};{1};{2};{3}", r.X, r.Y, r.Width, r.Height)
    End Function

    Public Function CvtStringToRect(s As String, [default] As Rectangle) As Rectangle
        Try
            Dim parts As String() = SplitSmart4(s)
            Dim x As Integer = Integer.Parse(parts(0), NumberStyles.Integer, CultureInfo.InvariantCulture)
            Dim y As Integer = Integer.Parse(parts(1), NumberStyles.Integer, CultureInfo.InvariantCulture)
            Dim w As Integer = Integer.Parse(parts(2), NumberStyles.Integer, CultureInfo.InvariantCulture)
            Dim h As Integer = Integer.Parse(parts(3), NumberStyles.Integer, CultureInfo.InvariantCulture)
            Return New Rectangle(x, y, w, h)
        Catch
            Return [default]
        End Try
    End Function

    '==========================================================
    ' PointF/SizeF/RectangleF – Semikolon-Format; float-basiert
    '==========================================================
    Public Function CvtPointFToString(p As PointF) As String
        Return String.Format(CultureInfo.InvariantCulture, "{0};{1}", p.X, p.Y)
    End Function

    Public Function CvtStringToPointF(s As String, [default] As PointF) As PointF
        Try
            Dim parts() As String = SplitSmart2(s)
            Dim x As Single = Single.Parse(parts(0), NumberStyles.Float, CultureInfo.InvariantCulture)
            Dim y As Single = Single.Parse(parts(1), NumberStyles.Float, CultureInfo.InvariantCulture)
            Return New PointF(x, y)
        Catch
            Return [default]
        End Try
    End Function

    Public Function CvtSizeFToString(sz As SizeF) As String
        Return String.Format(CultureInfo.InvariantCulture, "{0};{1}", sz.Width, sz.Height)
    End Function

    Public Function CvtStringToSizeF(s As String, [default] As SizeF) As SizeF
        Try
            Dim parts() As String = SplitSmart2(s)
            Dim w As Single = Single.Parse(parts(0), NumberStyles.Float, CultureInfo.InvariantCulture)
            Dim h As Single = Single.Parse(parts(1), NumberStyles.Float, CultureInfo.InvariantCulture)
            Return New SizeF(w, h)
        Catch
            Return [default]
        End Try
    End Function

    Public Function CvtRectFToString(r As RectangleF) As String
        Return String.Format(CultureInfo.InvariantCulture, "{0};{1};{2};{3}", r.X, r.Y, r.Width, r.Height)
    End Function

    Public Function CvtStringToRectF(s As String, [default] As RectangleF) As RectangleF
        Try
            Dim parts() As String = SplitSmart4(s)
            Dim x As Single = Single.Parse(parts(0), NumberStyles.Float, CultureInfo.InvariantCulture)
            Dim y As Single = Single.Parse(parts(1), NumberStyles.Float, CultureInfo.InvariantCulture)
            Dim w As Single = Single.Parse(parts(2), NumberStyles.Float, CultureInfo.InvariantCulture)
            Dim h As Single = Single.Parse(parts(3), NumberStyles.Float, CultureInfo.InvariantCulture)
            Return New RectangleF(x, y, w, h)
        Catch
            Return [default]
        End Try
    End Function

    '
    ''' <summary>
    ''' Serialisiert PaddingValues kompakt als "L,T,R,B".
    ''' </summary>
    Public Shared Function CvtPaddingToString(p As PaddingValues) As String
        ' Falls deine Structure bereits ToDataString besitzt, kannst du 1:1 delegieren:
        ' Return p.ToDataString()
        Return $"{p.Left},{p.Top},{p.Right},{p.Bottom}"
    End Function

    '
    ''' <summary>
    ''' Wandelt einen String in PaddingValues zurück.
    ''' Akzeptiert:
    '''  - "L,T,R,B" (z. B. "10,5,10,5", Semikolon auch erlaubt)
    '''  - benannte Felder "L=10,T=5,R=10,B=5" (Reihenfolge egal)
    '''  - "Empty" oder Leerstring → 0,0,0,0
    ''' Fehler → liefert [default].
    ''' </summary>
    Public Shared Function CvtStringToPadding(s As String, [default] As PaddingValues) As PaddingValues
        Try
            If String.IsNullOrWhiteSpace(s) Then Return [default]

            Dim txt As String = s.Trim()
            If txt.Equals("Empty", StringComparison.OrdinalIgnoreCase) Then
                Return New PaddingValues(0, 0, 0, 0)
            End If

            txt = txt.Replace(";", ",")

            ' Benannte Felder?
            If txt.Contains("="c) Then
                Dim l As Integer, t As Integer, r As Integer, b As Integer
                Dim seenL As Boolean, seenT As Boolean, seenR As Boolean, seenB As Boolean

                For Each part As String In txt.Split(","c)
                    Dim kv() As String = part.Split({"="c}, 2)
                    If kv.Length <> 2 Then Continue For
                    Dim name As String = kv(0).Trim().ToUpperInvariant()
                    Dim valStr As String = kv(1).Trim()
                    Dim val As Integer
                    If Not Integer.TryParse(valStr, val) Then Return [default]

                    Select Case name
                        Case "L", "LEFT" : l = val : seenL = True
                        Case "T", "TOP" : t = val : seenT = True
                        Case "R", "RIGHT" : r = val : seenR = True
                        Case "B", "BOTTOM" : b = val : seenB = True
                    End Select
                Next

                If seenL AndAlso seenT AndAlso seenR AndAlso seenB Then
                    Return New PaddingValues(l, t, r, b)
                Else
                    Return [default]
                End If
            End If

            ' Einfache Liste "L,T,R,B"
            Dim parts As String() = txt.Split(","c)
            If parts.Length <> 4 Then Return [default]

            Dim li, ti, ri, bi As Integer
            If Integer.TryParse(parts(0).Trim(), li) AndAlso
               Integer.TryParse(parts(1).Trim(), ti) AndAlso
               Integer.TryParse(parts(2).Trim(), ri) AndAlso
               Integer.TryParse(parts(3).Trim(), bi) Then
                Return New PaddingValues(li, ti, ri, bi)
            End If

            Return [default]
        Catch
            Return [default]
        End Try
    End Function
    '==========================================================
    ' Kleine Helfer zum robusten Splitten (Semikolon bevorzugt;
    ' Komma als Legacy-Fallback)
    '==========================================================
    Private Function SplitSmart2(s As String) As String()
        If s Is Nothing Then Throw New ArgumentNullException(NameOf(s))
        Dim parts As String()
        If s.Contains(";"c) Then
            parts = s.Split(";"c)
        Else
            parts = s.Split(","c)
        End If
        If parts.Length < 2 Then Throw New FormatException("Expected 2 parts.")
        Return New String() {parts(0).Trim(), parts(1).Trim()}
    End Function

    Private Function SplitSmart4(s As String) As String()
        If s Is Nothing Then Throw New ArgumentNullException(NameOf(s))
        Dim parts As String()
        If s.Contains(";"c) Then
            parts = s.Split(";"c)
        Else
            parts = s.Split(","c)
        End If
        If parts.Length < 4 Then Throw New FormatException("Expected 4 parts.")
        Return New String() {parts(0).Trim(), parts(1).Trim(), parts(2).Trim(), parts(3).Trim()}
    End Function

#End Region

#Region "String"

    ' ---- Helfer ----

    ' Darf roh (unquoted) geschrieben werden?
    Private Function IsSafePlain(s As String) As Boolean
        If s Is Nothing OrElse s.Length = 0 Then Return True
        If s.StartsWith(" "c) OrElse s.EndsWith(" "c) Then Return False
        For Each ch As Char In s
            Dim code As Integer = AscW(ch)
            If ch = "\"c OrElse ch = """"c Then Return False
            If code < 32 Then
                Return False ' Steuerzeichen \0..\x1F
            End If
        Next
        Return True
    End Function

    ' Sichtbar-escapen: \n \r \t \\ \" \0, sonst \uXXXX für Steuerzeichen
    Private Function EscapeForIni(s As String) As String
        If s Is Nothing Then Return ""
        Dim sb As New StringBuilder(s.Length + 8)
        For Each ch As Char In s
            Select Case ch
                Case ControlChars.Lf : sb.Append("\n")
                Case ControlChars.Cr : sb.Append("\r")
                Case ControlChars.Tab : sb.Append("\t")
                Case ControlChars.NullChar : sb.Append("\0")
                Case "\"c : sb.Append("\\")
                Case """"c : sb.Append("\""")
                Case Else
                    Dim code As Integer = AscW(ch)
                    If code < 32 Then
                        sb.Append("\u").Append(code.ToString("X4", CultureInfo.InvariantCulture))
                    Else
                        sb.Append(ch)
                    End If
            End Select
        Next
        Return sb.ToString()
    End Function

    ' Unescape: versteht quoted ("...") + \n \r \t \0 \\ \" \uXXXX
    Private Function UnescapeFromIni(quotedContent As String) As String

        Dim sb As New StringBuilder(quotedContent.Length)
        Dim i As Integer = 0
        While i < quotedContent.Length
            Dim ch As Char = quotedContent(i)
            If ch <> "\"c Then
                sb.Append(ch)
                i += 1
                Continue While
            End If

            ' Escape-Sequenz
            i += 1
            If i >= quotedContent.Length Then
                ' einzelner Backslash am Ende – roh übernehmen
                sb.Append("\"c)
                Exit While
            End If

            Select Case quotedContent(i)
                Case "n"c : sb.Append(ControlChars.Lf) : i += 1
                Case "r"c : sb.Append(ControlChars.Cr) : i += 1
                Case "t"c : sb.Append(ControlChars.Tab) : i += 1
                Case "0"c : sb.Append(ControlChars.NullChar) : i += 1
                Case "\"c : sb.Append("\"c) : i += 1
                Case """"c : sb.Append(""""c) : i += 1
                Case "u"c
                    ' \uXXXX (4 Hex-Zeichen)
                    If i + 4 < quotedContent.Length Then
                        Dim hex As String = quotedContent.Substring(i + 1, 4)
                        Dim code As Integer
                        If Integer.TryParse(hex, NumberStyles.HexNumber, CultureInfo.InvariantCulture, code) Then
                            sb.Append(ChrW(code))
                            i += 5
                        Else
                            sb.Append("\u")
                            i += 1
                        End If
                    Else
                        sb.Append("\u")
                        i += 1
                    End If
                Case Else
                    ' unbekanntes Escape – roh übernehmen
                    sb.Append("\"c).Append(quotedContent(i))
                    i += 1
            End Select
        End While
        Return sb.ToString()
    End Function

    ' ---- Kernfunktionen ----

    Public Function CvtStringValueToINIValue(value As String) As String
        If value Is Nothing Then
            Return ""
        End If
        If IsSafePlain(value) Then
            Return value
        End If
        Return """" & EscapeForIni(value) & """"
    End Function

    Public Function CvtINIValueToStringValue(stored As String, [default] As String) As String
        If stored Is Nothing Then
            Return If([default], "")
        End If
        If stored.Length >= 2 AndAlso stored(0) = """"c AndAlso stored(stored.Length - 1) = """"c Then
            Dim inner As String = stored.Substring(1, stored.Length - 2)
            Try
                Return UnescapeFromIni(inner)
            Catch
                Return If([default], "")
            End Try
        End If
        ' Unquoted -> roh zurückgeben
        Return stored

    End Function

#End Region

#Region "Pfade und Dateinamen"

    Private Const REL_PREFIX_APPROOT As String = "®"
    Private Const REL_PREFIX_USERROOT As String = "©"
    '
    ' Das ist das Verzeichnis eine Ebene unterhalb von "Dokumente" 
    ' und dort das Verzeichnis MahjongGK.
    ' Gleichzeitig ist hier der zentrale Ort im Programm, wo der
    ' Speicherpfad aller Daten festgelegt wird. (Ausgenommen:
    ' Für "speichern unter ..." wird ein Pfad vorgeschlagen aus diesem
    ' Bereich, den der Anwender aber ändern kann.")
    ' Eine Änderung hier wirkt sich auf das gesamte Programm aus.
    Public ReadOnly Property AppRoot As String
        Get
            Dim appname As String = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name
            If appname = "MahjongGK" Then
                Return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), appname)
            Else
                'In anderen Projekten als MahjongGK verwende ich:
                Return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Anwendungsdaten II", appname)
            End If
        End Get
    End Property

    '
    'Das ist das Verzeichnis "Dokumente" des aktuellen Benutzers.
    Private ReadOnly Property UserRoot As String =
                      Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)

    ' Merkt sich, ob wir schon im Notfallmodus sind
    Private _useFallback As Boolean? = Nothing  ' Nothing = noch nicht entschieden
    Private disposedValue As Boolean

    Public Function AppDataDirectory(Optional subdir As AppDataSubDir = AppDataSubDir.None,
                                     Optional subsubdir As AppDataSubSubDir = AppDataSubSubDir.None,
                                     Optional sub3Dir As String = Nothing) As String

        Return AppDataDirectory(
           If(subdir <> AppDataSubDir.None, subdir.ToString, String.Empty),
           AppDataSubSubDirToString(subsubdir),
           sub3Dir
           )

    End Function

    Public Function AppDataDirectory(Optional subdir As String = Nothing,
                                     Optional subsubdir As String = Nothing,
                                     Optional sub3Dir As String = Nothing) As String

        Dim aktpath As String = If(_useFallback.HasValue AndAlso _useFallback.Value,
                                   GetFallbackRoot(),
                                   AppRoot)

        If Not String.IsNullOrEmpty(subdir) Then
            aktpath = Path.Combine(aktpath, subdir)
        End If
        If Not String.IsNullOrEmpty(subsubdir) Then
            aktpath = Path.Combine(aktpath, subsubdir)
        End If
        If Not String.IsNullOrEmpty(sub3Dir) Then
            aktpath = Path.Combine(aktpath, sub3Dir)
        End If
        'zum testen
        'aktpath = "C:\?:\invalid\path" 'wirft "ungültiger Pfadnahme" ,

        Try
            Directory.CreateDirectory(aktpath)
        Catch ex As Exception
            If Not _useFallback.HasValue Then
                ' Benutzer fragen (nur beim ersten Mal)
                Dim msg As String =
                    "Der Speicherordner für Spieldaten konnte nicht erstellt werden:" & vbCrLf &
                    aktpath & vbCrLf & vbCrLf &
                    "Fehler: " & ex.Message & vbCrLf & vbCrLf &
                    "Soll im Notfallmodus weitergespielt werden?" & vbCrLf & vbCrLf &
                    "Die Speicherung erfolgt dann nur temporär und kann jederzeit verloren gehen. " &
                    "Andernfalls wird das Programm beendet."

                Dim result As DialogResult = MessageBox.Show(msg,
                                System.Reflection.Assembly.GetExecutingAssembly().GetName().Name _ 'Programmname
                                & " - Speicherfehler",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Warning,
                                MessageBoxDefaultButton.Button2)

                If result = DialogResult.Yes Then
                    _useFallback = True
                    aktpath = GetFallbackRoot()
                Else
                    Environment.Exit(1)
                End If

            ElseIf _useFallback.Value Then
                aktpath = GetFallbackRoot()
            Else
                ' Benutzer hatte "Nein" gesagt -> sofort beenden
                Environment.Exit(1)
            End If

            ' Fallback-Ordner erstellen
            Try
                Directory.CreateDirectory(aktpath)
            Catch
                MessageBox.Show("Auch der Notfall-Speicherpfad konnte nicht erstellt werden." & vbCrLf &
                                "Das Spielfeld wird beendet.",
                                "Kritischer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Environment.Exit(1)
            End Try
        End Try

        Return aktpath

    End Function

    Private Function GetFallbackRoot() As String
        Return Path.Combine(Path.GetTempPath(),
                            System.Reflection.Assembly.GetExecutingAssembly().GetName().Name)
    End Function

    '
    Public ReadOnly Property AppDataFileINI(dateiname_ext As String) As String
        Get
            'Dim path As String = path.Combine(AppDataFolder, "\Allgemeine-Einstellungen.ini")
            Return Path.Combine(AppDataDirectory(AppDataSubDir.INI.ToString), dateiname_ext)
        End Get
    End Property

    ''' <summary>
    ''' Montiert den kompletten Pfad aus den Enumerationen und fügt ggf. den aktuellen Zeitstempel hinzu.
    ''' Bei timestamp = AppDataTimeStamp.LookForLastTimeStamp wird nach der jüngsten Datei gesucht.
    ''' Gibt es keine Datei, wird String.Empty zurückgegeben!
    ''' maxFiles arbeitet nur in Verbindung mit timestamp = AppDataTimeStamp.AddRenderBitmapTopZOrder und räumt
    ''' "on the Fly" auf, indem alle Dateien über maxFiles hinaus gelöscht werden.
    ''' </summary>
    Public Function AppDataFullPath(filename As AppDataFileName,
                                    Optional timestamp As AppDataTimeStamp = AppDataTimeStamp.None,
                                    Optional maxFiles As Integer = Integer.MaxValue) As String

        Return AppDataFullPath(
            String.Empty,
            String.Empty,
            If(filename <> AppDataFileName.None, filename.ToString, String.Empty),
            timestamp, maxFiles
            )

    End Function

    ''' <summary>
    ''' Montiert den kompletten Pfad aus den Enumerationen und fügt ggf. den aktuellen Zeitstempel hinzu.
    ''' Bei timestamp = AppDataTimeStamp.LookForLastTimeStamp wird nach der jüngsten Datei gesucht.
    ''' Gibt es keine Datei, wird String.Empty zurückgegeben!
    ''' maxFiles arbeitet nur in Verbindung mit timestamp = AppDataTimeStamp.AddRenderBitmapTopZOrder und räumt
    ''' "on the Fly" auf, indem alle Dateien über maxFiles hinaus gelöscht werden.
    ''' </summary>
    Public Function AppDataFullPath(subdir As AppDataSubDir,
                                    filename As AppDataFileName,
                                    Optional timestamp As AppDataTimeStamp = AppDataTimeStamp.None,
                                    Optional maxFiles As Integer = Integer.MaxValue) As String

        Return AppDataFullPath(
            If(subdir <> AppDataSubDir.None, subdir.ToString, String.Empty),
            String.Empty,
            If(filename <> AppDataFileName.None, filename.ToString, String.Empty),
            timestamp,
            maxFiles
            )

    End Function

    ''' <summary>
    ''' Montiert den kompletten Pfad aus den Enumerationen und fügt ggf. den aktuellen Zeitstempel hinzu.
    ''' Bei timestamp = AppDataTimeStamp.LookForLastTimeStamp wird nach der jüngsten Datei gesucht.
    ''' Gibt es keine Datei, wird String.Empty zurückgegeben!
    ''' maxFiles arbeitet nur in Verbindung mit timestamp = AppDataTimeStamp.AddRenderBitmapTopZOrder und räumt
    ''' "on the Fly" auf, indem alle Dateien über maxFiles hinaus gelöscht werden.
    ''' </summary>
    Public Function AppDataFullPath(subdir As AppDataSubDir,
                                    subsubdir As AppDataSubSubDir,
                                    filename As AppDataFileName,
                                    Optional timestamp As AppDataTimeStamp = AppDataTimeStamp.None,
                                    Optional maxFiles As Integer = Integer.MaxValue) As String

        Return AppDataFullPath(
            If(subdir <> AppDataSubDir.None, subdir.ToString, String.Empty),
            AppDataSubSubDirToString(subsubdir),
            If(filename <> AppDataFileName.None, filename.ToString, String.Empty),
            timestamp,
            maxFiles
            )

    End Function
    '
    ''' <summary>
    ''' Überladung speziell zu Speicherung der Spiele.
    ''' </summary>
    ''' <param name="subdir"></param>
    ''' <param name="subsubdir"></param>
    ''' <param name="filename"></param>
    ''' <returns></returns>
    Public Function AppDataFullPathSpiel(subdir As AppDataSubDir,
                                   subsubdir As AppDataSubSubDir,
                                   filename As String) As String

        Dim basePath As String = AppDataDirectory(
            If(subdir <> AppDataSubDir.None, subdir.ToString, String.Empty),
            AppDataSubSubDirToString(subsubdir))

        If String.IsNullOrWhiteSpace(filename) Then
            filename = Guid.NewGuid.ToString
        End If

        If Not filename.EndsWith(".xml", StringComparison.OrdinalIgnoreCase) Then
            filename &= ".xml"
        End If

        Dim fullpath As String = Path.Combine(basePath, filename)

        Return fullpath

    End Function

    '
    ''' <summary>
    ''' Montiert den kompletten Pfad aus den Enumerationen und fügt ggf. den aktuellen Zeitstempel hinzu.
    ''' Bei timestamp = AppDataTimeStamp.LookForLastTimeStamp wird nach der jüngsten Datei gesucht.
    ''' Gibt es keine Datei, wird String.Empty zurückgegeben!
    ''' maxFiles arbeitet nur in Verbindung mit timestamp = AppDataTimeStamp.AddRenderBitmapTopZOrder und räumt
    ''' "on the Fly" auf, indem alle Dateien über maxFiles hinaus gelöscht werden.
    ''' </summary>
    ''' <param name="subdir"></param>
    ''' <param name="subsubdir"></param>
    ''' <param name="filename"></param>
    ''' <param name="timestamp"></param>
    ''' <returns></returns>
    Public Function AppDataFullPath(Optional subdir As String = Nothing,
                                    Optional subsubdir As String = Nothing,
                                    Optional filename As String = Nothing,
                                    Optional timestamp As AppDataTimeStamp = AppDataTimeStamp.None,
                                    Optional maxFiles As Integer = Integer.MaxValue) As String

        Dim basePath As String = AppDataDirectory(subdir, subsubdir)

        ' Kein Dateiname -> nur Verzeichnis
        If String.IsNullOrEmpty(filename) Then
            Return basePath
        End If

        'den Unterstrich in der Enumeration ändern. 
        If filename.Length > 3 AndAlso filename(filename.Length - 4) = "_"c Then
            filename = filename.Substring(0, filename.Length - 4) &
                "." &
                filename.Substring(filename.Length - 3)
        End If

        ' Normale Kombination
        Dim fullpath As String = Path.Combine(basePath, filename)

        If timestamp = AppDataTimeStamp.Add Then
            fullpath = CreateFullPathWithTimeStamp(fullpath, maxFiles)
        ElseIf timestamp = AppDataTimeStamp.LookForLastTimeStamp Then
            fullpath = GetFullpathFromLastTimeStamp(fullpath)
        End If

        Return fullpath

    End Function

    ''' <summary>
    ''' Wandelt einen absoluten Pfad in einen relativen Pfad zu _appRoot oder _userRoot um.
    ''' Liegt der Pfad außerhalb beider Wurzeln, wird der Originalpfad zurückgegeben.
    ''' Relative Pfade erhalten das Präfix ® oder ©.
    ''' </summary>
    Public Function CvtPathToRelPath(path As String) As String
        Dim absPath As String = IO.Path.GetFullPath(path)
        Dim rootApp As String = IO.Path.GetFullPath(AppRoot)
        Dim rootUser As String = IO.Path.GetFullPath(UserRoot)

        ' Prüfen auf _appRoot
        If String.Equals(absPath, rootApp, StringComparison.OrdinalIgnoreCase) OrElse
               absPath.StartsWith(rootApp & IO.Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase) Then
            Dim rel As String = absPath.Substring(rootApp.Length)
            If rel.StartsWith(IO.Path.DirectorySeparatorChar) OrElse rel.StartsWith(IO.Path.AltDirectorySeparatorChar) Then
                rel = rel.Substring(1)
            End If
            Return REL_PREFIX_APPROOT & rel
        End If

        ' Prüfen auf _userRoot
        If String.Equals(absPath, rootUser, StringComparison.OrdinalIgnoreCase) OrElse
                absPath.StartsWith(rootUser & IO.Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase) Then
            Dim rel As String = absPath.Substring(rootUser.Length)
            If rel.StartsWith(IO.Path.DirectorySeparatorChar) OrElse rel.StartsWith(IO.Path.AltDirectorySeparatorChar) Then
                rel = rel.Substring(1)
            End If
            Return REL_PREFIX_USERROOT & rel
        End If

        ' Pfad außerhalb der Wurzeln -> Original zurückgeben
        Return path
    End Function

    ''' <summary>
    ''' Wandelt einen Pfad aus der INI zurück in einen absoluten Pfad.
    ''' Erkennt automatisch das Präfix ® (AppRoot) oder © (UserRoot) und hängt die richtige Wurzel davor.
    ''' </summary>
    Public Function CvtRelPathToPath(relPath As String) As String
        If String.IsNullOrEmpty(relPath) Then Return AppRoot

        If relPath.StartsWith(REL_PREFIX_APPROOT) Then
            Dim rel As String = relPath.Substring(REL_PREFIX_APPROOT.Length)
            Return Path.Combine(AppRoot, rel)
        ElseIf relPath.StartsWith(REL_PREFIX_USERROOT) Then
            Dim rel As String = relPath.Substring(REL_PREFIX_USERROOT.Length)
            Return Path.Combine(UserRoot, rel)
        Else
            ' Kein Präfix -> Pfad unverändert
            Return relPath
        End If
    End Function

    ' Letzte Datei anhand Timestamp finden
    Private Function GetFullpathFromLastTimeStamp(fullPath As String) As String
        Dim files As List(Of (Datum As DateTime, FullPath As String)) = GetSortedTimeStampFiles(fullPath)
        If files.Count > 0 Then
            Return files(0).FullPath  ' Jüngste zuerst
        Else
            Return String.Empty
        End If
    End Function

    ''' <summary>
    ''' Aus dem fullPath (einschließich Dateiname) wird der Dateiname separiert und das Pattern
    ''' eines Timestamp vorangestellt und im Verzeichnis alle Dateien gesucht, die diesem
    ''' Pattern entsprechen. Anschließend wird ein Dialog aufgerufen und der Anwender kann wählen.
    ''' </summary>
    ''' <param name="fullPath"></param>
    ''' <param name="header"></param>
    ''' <returns></returns>
    Public Function GetFullpathFromSelectedTimeStamp(fullPath As String, Optional header As String = Nothing) As String

        Dim files As List(Of (Datum As DateTime, FullPath As String)) = GetSortedTimeStampFiles(fullPath)
        If files.Count = 0 Then Return String.Empty

        Using frm As New Form With {
            .Text = If(String.IsNullOrEmpty(header), "Datei mit Zeitstempel auswählen", header),
            .Width = 500,
            .Height = 300,
            .StartPosition = FormStartPosition.CenterScreen
        }
            Dim lb As New ListBox With {
            .Dock = DockStyle.Fill
        }
            Dim found As Boolean
            ' Datum/Uhrzeit + Original-Dateiname anzeigen
            For Each entry As (Datum As DateTime, FullPath As String) In files
                Dim origName As String = Path.GetFileName(entry.FullPath).Substring(20) ' skip yyyy-mm-dd-hh-mm-ss_
                lb.Items.Add($"{entry.Datum:yyyy-MM-dd HH:mm:ss}  {origName}")
                found = True
            Next

            If Not found Then
                MsgBox("Es gibt hier (noch) keine Dateien", MsgBoxStyle.Information, If(String.IsNullOrEmpty(header), "Datei mit Zeitstempel auswählen", header))
                Return String.Empty

            End If

            frm.Controls.Add(lb)

            Dim btnOK As New Button With {
            .Text = "OK",
            .Dock = DockStyle.Bottom
        }
            frm.Controls.Add(btnOK)

            AddHandler btnOK.Click, Sub() frm.DialogResult = DialogResult.OK

            If frm.ShowDialog() = DialogResult.OK AndAlso lb.SelectedIndex >= 0 Then
                Return files(lb.SelectedIndex).FullPath
            End If
        End Using

        Return String.Empty

    End Function

    ' Hilfsfunktion: Timestamp-Dateien sortiert zurückgeben
    Private Function GetSortedTimeStampFiles(fullPath As String) As List(Of (Datum As DateTime, FullPath As String))
        Dim result As New List(Of (Datum As DateTime, FullPath As String))
        If String.IsNullOrWhiteSpace(fullPath) Then Return result

        Dim dir As String = Path.GetDirectoryName(fullPath)
        Dim fileName As String = Path.GetFileName(fullPath)
        If Not Directory.Exists(dir) Then Return result

        Dim pattern As String = "????-??-??-??-??-??_" & fileName

        Dim tmp As New List(Of (Datum As DateTime, FullPath As String))

        For Each File As String In Directory.GetFiles(dir, pattern)
            Dim baseName As String = Path.GetFileName(File)
            Dim tsPart As String = baseName.Substring(0, 19) ' yyyy-MM-dd-HH-mm-ss
            Dim ts As DateTime
            If DateTime.TryParseExact(tsPart, "yyyy-MM-dd-HH-mm-ss",
                                  CultureInfo.InvariantCulture,
                                  DateTimeStyles.None, ts) Then
                tmp.Add((ts, File))
            End If
        Next

        ' Sortierung: jüngste zuerst
        result = tmp.OrderByDescending(Function(x) x.Datum).ToList()
        Return result
    End Function

    ' Neue Datei mit Timestamp erzeugen
    Private Function CreateFullPathWithTimeStamp(fullPath As String, Optional maxFiles As Integer = Integer.MaxValue) As String

        If String.IsNullOrWhiteSpace(fullPath) Then
            Return String.Empty
        End If

        Dim dir As String = Path.GetDirectoryName(fullPath)
        Dim fileName As String = Path.GetFileName(fullPath)
        Dim timeStamp As String = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss")
        Dim newFilePath As String = Path.Combine(dir, $"{timeStamp}_{fileName}")

        ' Optionales Aufräumen
        If maxFiles > 0 Then
            Dim files As List(Of (Datum As DateTime, FullPath As String)) = GetSortedTimeStampFiles(fullPath)
            If files.Count >= maxFiles Then
                ' Älteste löschen, bis die Anzahl passt
                For Each oldFile As (Datum As DateTime, FullPath As String) In files.Skip(maxFiles - 1)
                    Try
                        File.Delete(oldFile.FullPath)
                    Catch ex As Exception
                        ' Ignorieren, wenn nicht löschbar
                    End Try
                Next
            End If
        End If

        Return newFilePath

    End Function

    ''' <summary>
    ''' Aus dem fullPath (einschließlich Dateiname) wird das Verzeichnis ermittelt und alle Dateien,
    ''' die auf das angegebene Pattern passen, alphabetisch sortiert angezeigt. 
    ''' Der Benutzer kann eine Datei auswählen.
    ''' </summary>
    ''' <returns>Vollständiger Pfad der gewählten Datei oder String.Empty</returns>
    Public Function GetFullpathFromSelectedFile(path As String,
                                            pattern As String,
                                            Optional header As String = Nothing) As String

        Dim files As List(Of String) = GetSortedFiles(path, pattern)
        If files.Count = 0 Then Return String.Empty

        Using frm As New Form With {
        .Text = If(String.IsNullOrEmpty(header), "Datei auswählen", header),
        .Width = 500,
        .Height = 300,
        .StartPosition = FormStartPosition.CenterScreen
    }
            Dim lb As New ListBox With {
            .Dock = DockStyle.Fill
        }
            Dim found As Boolean

            For Each f As String In files
                lb.Items.Add(IO.Path.GetFileName(f))
                found = True
            Next

            If Not found Then
                MsgBox("Es gibt hier (noch) keine Dateien", MsgBoxStyle.Information,
                   If(String.IsNullOrEmpty(header), "Datei auswählen", header))
                Return String.Empty
            End If

            frm.Controls.Add(lb)

            Dim btnOK As New Button With {
            .Text = "OK",
            .Dock = DockStyle.Bottom
        }
            frm.Controls.Add(btnOK)

            AddHandler btnOK.Click, Sub() frm.DialogResult = DialogResult.OK

            If frm.ShowDialog() = DialogResult.OK AndAlso lb.SelectedIndex >= 0 Then
                Return files(lb.SelectedIndex)
            End If
        End Using

        Return String.Empty

    End Function

    ''' <summary>
    ''' Holt alphabetisch sortierte Dateien im angegebenen Verzeichnis, die auf das Pattern passen.
    ''' </summary>
    Private Function GetSortedFiles(path As String, pattern As String) As List(Of String)
        Dim result As New List(Of String)
        If String.IsNullOrWhiteSpace(path) Then Return result

        If Not Directory.Exists(path) Then Return result

        result = Directory.GetFiles(path, pattern).OrderBy(Function(x) x).ToList()
        Return result
    End Function

    Public Function AppDataSubSubDirToString(adss As AppDataSubSubDir) As String
        Dim name As String = adss.ToString()

        ' Sonderfall: None → leer
        If adss = AppDataSubSubDir.None Then
            Return String.Empty
        End If

        ' Alle AppDataSubDir-Namen durchgehen
        For Each ads As AppDataSubDir In [Enum].GetValues(GetType(AppDataSubDir))
            If ads = AppDataSubDir.None Then Continue For

            Dim prefix As String = ads.ToString() & "_"
            If name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) Then
                ' Alles nach dem Unterstrich zurückgeben
                Return name.Substring(prefix.Length)
            End If
        Next

        ' Kein Präfix gefunden → vollständiger Name
        Return name
    End Function

#End Region

#Region "Load Save einschließlich der verzögerten Speicherung"

    Private changed As Boolean = False
    Private saveCts As CancellationTokenSource
    Private ReadOnly saveDebounceMs As Integer = 500

    'da wird etwas zugewiesen – nur eben kein „sichtbarer Inhalt“,
    'sondern ein anonymer Platzhalter-Object, der als Schloss dient,
    'damit andere Threads den Code zwischen SyncLock ind End SyncLock
    'nicht ausführen.
    Private ReadOnly gate As New Object()

    ' Muss threadsicher aufgerufen werden können
    Private Sub MarkChanged()

        SyncLock gate
            changed = True

            ' alte CTS abbrechen & verwerfen (Dispose später im Task)
            Dim oldCts As CancellationTokenSource = saveCts
            If oldCts IsNot Nothing Then
                Try : oldCts.Cancel()
                Catch : End Try
            End If

            ' neue CTS anlegen
            saveCts = New CancellationTokenSource()
            Dim localCts As CancellationTokenSource = saveCts

            ' Entprell-Aufgabe starten
            Task.Run(
                Async Function()
                    Try
                        Await Task.Delay(saveDebounceMs, localCts.Token).ConfigureAwait(False)

                        ' Nur speichern, wenn diese CTS noch die aktuelle ist
                        Dim isCurrent As Boolean
                        SyncLock gate
                            isCurrent = Object.ReferenceEquals(localCts, saveCts)
                        End SyncLock

                        If isCurrent AndAlso changed Then
                            ' Optional: changed vor/nach Save setzen – je nach Logik
                            Save() ' oder: Await SaveAsync().ConfigureAwait(False)
                            SyncLock gate
                                changed = False
                            End SyncLock
                        End If

                    Catch ex As TaskCanceledException
                        ' Abbruch = neue Änderung kam -> nichts tun
                    Finally
                        localCts.Dispose()
                    End Try
                End Function)
        End SyncLock
    End Sub

    ' Beispiel: beim Beenden oder Dispose aufrufen
    Public Sub Flush()

        Dim cts As CancellationTokenSource = Nothing
        SyncLock gate
            cts = saveCts
        End SyncLock

        ' laufende Debounce-Phase abbrechen und direkt speichern
        If cts IsNot Nothing Then
            Try
                cts.Cancel()
            Finally
                cts.Dispose()
            End Try
        End If

        If changed Then
            Save()
            SyncLock gate
                changed = False
            End SyncLock
        End If
    End Sub

    Public Sub CopyOrgFileToTmpFile()

        Dim lo As New List(Of String)
        If File.Exists(_fileFullPath) Then
            For Each l As String In File.ReadAllLines(_fileFullPath)
                Dim trimmed As String = l.Trim()
                If trimmed <> "" Then
                    lo.Add(trimmed)
                End If
            Next
        End If

        Try
            Using sw As New StreamWriter(_fileFullPath & ".tmp", False, Encoding.UTF8)
                For Each l As String In lo
                    sw.WriteLine(l)
                Next
            End Using
            changed = False
        Catch ex As Exception
            'TODO Logging/Fehlermeldung 
        End Try

    End Sub

    ''' <summary>
    ''' Speichert die Datei sofort, sofern Änderungen gemacht wurden
    ''' </summary>
    Public Sub Save(Optional alwaysSave As Boolean = False)
        If Not changed And Not alwaysSave Then
            Return
        End If

        Try
            Using sw As New StreamWriter(_fileFullPath, False, Encoding.UTF8)
                For Each l As String In _iniLines
                    sw.WriteLine(l)
                Next
            End Using
            changed = False
        Catch ex As Exception
            'TODO Logging/Fehlermeldung 
        End Try
    End Sub

    ''' <summary>
    ''' Lädt die Datei oder erzeugt neue.
    ''' </summary>
    Private Sub Load(loadTmpFile As Boolean)

        _iniLines.Clear()

        ' Je nach Flag richtigen Pfad zusammensetzen
        Dim path As String = _fileFullPath & If(loadTmpFile, ".tmp", String.Empty)

        If File.Exists(path) Then
            For Each l As String In File.ReadAllLines(path)
                Dim trimmed As String = l.Trim()
                If trimmed <> "" Then
                    _iniLines.Add(trimmed)
                End If
            Next
        End If

    End Sub

#End Region

#Region "Dispose"

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                Flush()
            End If
            disposedValue = True
        End If
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        Dispose(disposing:=True)
        GC.SuppressFinalize(Me)
    End Sub

#End Region

End Class

