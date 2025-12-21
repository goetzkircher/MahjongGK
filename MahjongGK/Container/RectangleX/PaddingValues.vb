'
' SPDX-License-Identifier: GPL-3.0-or-later
'###########################################################################
'#                                                                         #
'#   Copyright © 2025–2026 Götz Kircher <MahjongGK@t-online.de>            #
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
Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On
'
#Disable Warning IDE0079
#Disable Warning IDE1006

''' <summary>
''' ' RectangleX
'''   - Rechteck wie System.Drawing.Rectangle, erweitert um Padding
'''   - Umwandlung Rectangle RectangleX per Operator CType
'''   - Inside-Properties berücksichtigen Padding (XInside, WidthInside, …)
'''   - Ableitungen: GetRectangleInside(...) / GetRectangleXInside(...)
'''   - Zeichenmethoden: DrawRectangle, FillRectangle, DrawImage, DrawString, …
'''   - Utility: DeepCopy, ToDataString, FromDataString
'''   - Inc*/Dec* für X, Y, Left, Top, Right, Bottom (Optional Schrittweite)
'''   - GrowAll / ShrinkAll für gleichmäßiges Vergrößern/Verkleinern
''' </summary>
Public Structure PaddingValues
    Public Property Left As Integer
    Public Property Top As Integer
    Public Property Right As Integer
    Public Property Bottom As Integer

    Public Sub New(all As Integer)
        Left = all : Top = all : Right = all : Bottom = all
    End Sub

    Public Sub New(left As Integer, top As Integer, right As Integer, bottom As Integer)
        Me.Left = left : Me.Top = top : Me.Right = right : Me.Bottom = bottom
    End Sub

    Public Property All As Integer
        Get
            If Left = Top AndAlso Top = Right AndAlso Right = Bottom Then Return Left
            Return 0
        End Get
        Set(value As Integer)
            Left = value : Top = value : Right = value : Bottom = value
        End Set
    End Property

    Public Function Horizontal() As Integer
        Return Left + Right
    End Function
    Public Function Vertical() As Integer
        Return Top + Bottom
    End Function

    Public Overrides Function ToString() As String
        Return $"L{Left},T{Top},R{Right},B{Bottom}"
    End Function

    Public Function ToDataString() As String
        ' Kompakt als "L,T,R,B"
        Return $"{Left},{Top},{Right},{Bottom}"
    End Function

    Public Shared Function FromDataString(data As String) As PaddingValues
        If String.IsNullOrWhiteSpace(data) Then Return Empty

        Dim parts As String() = data.Split(","c)
        If parts.Length <> 4 Then Return Empty

        Dim l, t, r, b As Integer
        If Integer.TryParse(parts(0), l) AndAlso
           Integer.TryParse(parts(1), t) AndAlso
           Integer.TryParse(parts(2), r) AndAlso
           Integer.TryParse(parts(3), b) Then
            Return New PaddingValues(l, t, r, b)
        End If

        Return Empty
    End Function
    Public Shared ReadOnly Property Empty As PaddingValues
        Get
            Return New PaddingValues(0, 0, 0, 0)
        End Get
    End Property

    Public ReadOnly Property IsEmpty As Boolean
        Get
            Return Left = 0 AndAlso Top = 0 AndAlso Right = 0 AndAlso Bottom = 0
        End Get
    End Property

End Structure
