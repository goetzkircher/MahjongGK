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

Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

Imports System.IO

#Disable Warning IDE0079
#Disable Warning IDE1006

Namespace Images

    Public Module BitmapContainer

        Private Const UBND_BITMAPCONTAINER As Integer = 11
        Private ReadOnly _bitmapcontainer(UBND_BITMAPCONTAINER) As ImagesBase64XML

        Private _IsInit As Boolean
        Public ReadOnly Property IsInit As Boolean
            Get
                Return _IsInit
            End Get
        End Property


        Public Sub ChangeImagesSize(steinWidth As Integer, steinHeight As Integer)
            If BitmapContainer.IsInit Then
                BitmapContainer.CreateImages(steinWidth, steinHeight, INI.Rendering_BitmapHighQuality)
            Else
                BitmapContainer.Initialisierung(steinWidth, steinHeight, INI.Rendering_BitmapHighQuality)
            End If
        End Sub

        Public Sub Initialisierung(steinWidth As Integer, steinHeight As Integer, highQuality As Boolean)

            Dim zLwP As String = Path.Combine(Application.StartupPath, "Images", "XML-Container")
            Dim zLwPD As String = Path.Combine(zLwP, "MahjongImagesSrcBase64_00_Normal.xml")
            _bitmapcontainer(SteinStatus.I01Normal) = ImagesBase64XML.Load(zLwPD)


            'Basieren auf dem Normalstein
            _bitmapcontainer(SteinStatus.I08WerkstückEinfügeFehler) = ImagesBase64XML.Load(zLwPD)
            _bitmapcontainer(SteinStatus.I09WerkstückZufallsgrafik) = ImagesBase64XML.Load(zLwPD)

            zLwPD = Path.Combine(zLwP, "MahjongImagesSrcBase64_01_Selected.xml")
            _bitmapcontainer(SteinStatus.I02Selected) = ImagesBase64XML.Load(zLwPD)

            zLwPD = Path.Combine(zLwP, "MahjongImagesSrcBase64_02_Selectable.xml")
            _bitmapcontainer(SteinStatus.I03Selectable) = ImagesBase64XML.Load(zLwPD)

            zLwPD = Path.Combine(zLwP, "MahjongImagesSrcBase64_03_Removable.xml")
            _bitmapcontainer(SteinStatus.I04Removable) = ImagesBase64XML.Load(zLwPD)

            zLwPD = Path.Combine(zLwP, "MahjongImagesSrcBase64_04_Locked.xml")
            _bitmapcontainer(SteinStatus.I05Locked) = ImagesBase64XML.Load(zLwPD)

            zLwPD = Path.Combine(zLwP, "MahjongImagesSrcBase64_05_NotUsed.xml")
            _bitmapcontainer(SteinStatus.I06NotUnsed) = ImagesBase64XML.Load(zLwPD)

            zLwPD = Path.Combine(zLwP, "MahjongImagesSrcBase64_06_MissingSecond.xml")
            _bitmapcontainer(SteinStatus.I07MissingSecond) = ImagesBase64XML.Load(zLwPD)



            CreateImages(steinWidth, steinHeight, highQuality)

            _IsInit = True

        End Sub

        Public Sub CreateImages(steinWidth As Integer, steinHeight As Integer, highQuality As Boolean)

            For idxSteinStatus As Integer = 0 To _bitmapcontainer.GetUpperBound(0)

                Dim createDummyBmps As Boolean = False
                Select Case idxSteinStatus
                    Case SteinStatus.I00Unsichtbar, SteinStatus.I10Reserve1, SteinStatus.I11Reserve2
                        _bitmapcontainer(idxSteinStatus) = New ImagesBase64XML
                        createDummyBmps = True
                End Select
                _bitmapcontainer(idxSteinStatus).CreateImages(steinWidth, steinHeight, highQuality, idxSteinStatus, createDummyBmps)
            Next
        End Sub

        Public Function GetBitmap(steinstatus As SteinStatus, idx As Integer) As Bitmap
            Return _bitmapcontainer(steinstatus).GetImage(idx)
        End Function

        Public Function GetScaledBitmaps(steinstatus As SteinStatus) As Bitmap()
            Return _bitmapcontainer(steinstatus).GetScaledBitmaps
        End Function

        Public Function GetOriginalBitmaps(steinstatus As SteinStatus) As Bitmap()
            Return _bitmapcontainer(steinstatus).GetOriginalBitmaps
        End Function

    End Module

End Namespace