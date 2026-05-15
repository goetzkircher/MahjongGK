Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On
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
Imports System.Drawing
Imports System.Drawing.Drawing2D

'
#Disable Warning IDE0079
#Disable Warning IDE1006

Public Module TileSymbolRenderer

    Public Function CreateSymbolLayerBitmap(request As TileRequest,
                                               layout As TileLayout,
                                               colors As TileColors) As Bitmap

        If request Is Nothing Then Throw New ArgumentNullException(NameOf(request))
        If layout Is Nothing Then Throw New ArgumentNullException(NameOf(layout))
        If colors Is Nothing Then Throw New ArgumentNullException(NameOf(colors))

        Dim bmp As New Bitmap(layout.SteinRect.Width, layout.SteinRect.Height, Imaging.PixelFormat.Format32bppArgb)

        Dim srv As TileColors.SymbolRenderValues = colors.GetSymbolRenderValues
        Dim trv As TileColors.TextRenderValues = colors.GetTextRenderValues

        Using gfx As Graphics = Graphics.FromImage(bmp)

            SetGraphicsMode(gfx, sourceOver:=False)
            gfx.Clear(Color.Transparent)

            If Not srv.DonotInsertSymbol Then
                Dim tr As RectangleF = GetTargetRect(layout, colors)
                RenderSymbolIntoRect(gfx, tr, srv)

                If trv.HasText Then
                    'Dim rect As Rectangle
                    'With colors
                    '    '                          10 =  10 Pixel Verschiebung auf dem Basisstein als Basis.
                    '    Dim left As Integer = CInt(10 * layout.FaktorBasisWidthToAktWidth * colors.FaktorTextOffsetLeft)
                    '    Dim top As Integer = CInt(10 * layout.FaktorBasisHeightToAktHeight * colors.FaktorTextOffsetTop)

                    '    rect = New Rectangle(
                    '    CInt(layout.FaceInnerRect.Left * (.FaktorTextOffsetLeft + 2.0F)),
                    '    CInt(layout.FaceInnerRect.Top * (.FaktorTextOffsetTop + 2.0F)),
                    '    CInt(layout.FaceInnerRect.Width * 0.24R),
                    '    CInt(layout.FaceInnerRect.Height * 0.16R))
                    'End With

                    '

                    Dim pt As New Point(trv.Left, trv.Top)
                    RenderTextAtPoint(gfx, pt, trv.FontSize, colors, trv)
                    'If trv.UseUgrd AndAlso trv.TextUgrdDiameter > 0 Then
                    '    RenderTextAtPoint(gfx, pt, trv.FontSize, colors, trv)
                    'Else
                    '    RenderTextAtPoint(gfx, pt, trv.Text, trv.FontFamilyName, trv.FontStyle, trv.TextColor, trv.FontSize)
                    'End If

                End If

                'Note Auskommentiert. Schreibt einen Erzeugungszähler in den Mahjongstein
                'Static count As Integer
                'count += 1
                'RenderSymbolIntoRect(gfx, tr, "   " & count.ToString & "   ", "Arial", FontStyle.Regular, Color.Black, faktorSymbolSize:=1)

            End If
        End Using

        Return bmp

    End Function

    Public Sub RenderSymbolIntoRect(
       gfx As Graphics,
       targetRect As RectangleF,
       srv As TileColors.SymbolRenderValues)

        With srv

            Dim faktorSymbolSize As Decimal = .FaktorSymbolSize
            If .RenderSymbolHalfSize Then
                With targetRect
                    targetRect.Inflate(New SizeF(.Width / -8, .Height / -8))
                End With
            End If
            If .UseGradient Then
                RenderSymbolIntoRect(gfx, targetRect, .Symbol, .FontFamilyName, .FontStyle, .GradientColorFrom, .GradientColorTo, .SymbolGradientMode, .OutlineColor, .FaktorOutlineWidth, faktorSymbolSize)
            Else
                RenderSymbolIntoRect(gfx, targetRect, .Symbol, .FontFamilyName, .FontStyle, .NormalColor, faktorSymbolSize)
            End If

            If .InsertErrorCross Then
                'With targetRect
                '    targetRect.Inflate(New SizeF(.Width / -4, .Height / -4))
                'End With
                RenderSymbolIntoRect(gfx, targetRect, "✘", "Segoe UI", FontStyle.Regular, Color.Black, faktorSymbolSize:=1)
            End If

        End With
    End Sub

    '
    ''' <summary>
    ''' Rendert ein Unicode-Zeichen mittig in ein beliebiges Zielrechteck.
    ''' Einfarbig
    ''' </summary>
    Private Sub RenderSymbolIntoRect(
        g As Graphics,
        targetRect As RectangleF,
        symbolText As String,
        fontFamilyName As String,
        fontStyle As FontStyle,
        symbolColor As Color,
        faktorSymbolSize As Decimal)

        If String.IsNullOrWhiteSpace(symbolText) Then
            Exit Sub
        End If

        Try

            Dim drawRect As RectangleF = RectangleF.FromLTRB(
                CSng(targetRect.Left),
                CSng(targetRect.Top),
                CSng(targetRect.Right),
                CSng(targetRect.Bottom))

            Dim fontToUse As Font = CreateBestFitFont(
                g:=g,
                text:=symbolText,
                rect:=drawRect,
                preferredFontFamilyName:=fontFamilyName,
                fontStyle:=fontStyle,
                faktorSymbolSize)

            Using fontToDispose As Font = fontToUse,
                  brText As New SolidBrush(symbolColor),
                  sf As New StringFormat()

                sf.Alignment = StringAlignment.Center
                sf.LineAlignment = StringAlignment.Center
                sf.FormatFlags = StringFormatFlags.NoClip
                sf.Trimming = StringTrimming.None

                g.DrawString(symbolText, fontToDispose, brText, drawRect, sf)

            End Using

        Finally

        End Try

    End Sub
    Private Sub RenderSymbolIntoRect(g As Graphics,
                             targetRect As RectangleF,
                             symbolText As String,
                             fontFamilyName As String,
                             fontStyle As FontStyle,
                             colorFrom As Color,
                             colorTo As Color,
                             gradientMode As LinearGradientMode,
                             outlineColor As Color,
                             outlineWidth As Single,
                             faktorSymbolSize As Decimal)

        If String.IsNullOrEmpty(symbolText) Then
            Exit Sub
        End If

        Try

            Using fontToUse As Font = CreateBestFitFont(
                g:=g,
                text:=symbolText,
                rect:=targetRect,
                preferredFontFamilyName:=fontFamilyName,
                fontStyle:=fontStyle,
                faktorSymbolSize:=faktorSymbolSize)

                Using sf As New StringFormat()
                    sf.Alignment = StringAlignment.Center
                    sf.LineAlignment = StringAlignment.Center
                    sf.FormatFlags = StringFormatFlags.NoClip
                    sf.Trimming = StringTrimming.None

                    Using gp As New GraphicsPath()

                        Dim emSize As Single = CSng(g.DpiY * fontToUse.SizeInPoints / 72.0F)

                        gp.AddString(symbolText,
                                     fontToUse.FontFamily,
                                     CInt(fontToUse.Style),
                                     emSize,
                                     targetRect,
                                     sf)

                        Using br As New LinearGradientBrush(targetRect, colorFrom, colorTo, gradientMode)
                            g.FillPath(br, gp)
                        End Using

                        If outlineWidth > 0.0F AndAlso outlineColor.A > 0 Then
                            Using pn As New Pen(outlineColor, outlineWidth)
                                pn.LineJoin = LineJoin.Miter
                                g.DrawPath(pn, gp)
                            End Using
                        End If

                    End Using
                End Using
            End Using
        Finally

        End Try

    End Sub

    Private Sub RenderTextAtPoint(
        g As Graphics,
        targetPoint As Point,
        symbolText As String,
        fontFamilyName As String,
        fontStyle As FontStyle,
        symbolColor As Color,
        fontsize As Single)

        If String.IsNullOrWhiteSpace(symbolText) Then
            Exit Sub
        End If

        Try

            Dim fontFamily As FontFamily = SymbolFontManager.ResolveFontFamily(fontFamilyName)

            Using font As New Font(fontFamily, fontsize, fontStyle, GraphicsUnit.Pixel),
                  brText As New SolidBrush(symbolColor)

                g.DrawString(symbolText, font, brText, targetPoint)

            End Using

        Finally

        End Try

    End Sub

    Private Sub RenderTextAtPoint(
            g As Graphics,
            targetPoint As Point,
            fontsize As Single,
            colors As TileColors,
            trv As TileColors.TextRenderValues)

        Dim bmp As Bitmap = GetTileTextBitmap(trv)

        Dim oldCompositingMode As Drawing2D.CompositingMode = g.CompositingMode

        Try
            g.CompositingMode = Drawing2D.CompositingMode.SourceOver
            g.DrawImageUnscaled(bmp, targetPoint)
        Finally
            g.CompositingMode = oldCompositingMode
        End Try

    End Sub
    '
    ''' <summary>
    ''' Sucht eine brauchbare Fontgröße, die möglichst groß in die SymbolRect passt.
    ''' </summary>
    Private Function CreateBestFitFont(
        g As Graphics,
        text As String,
        rect As RectangleF,
        preferredFontFamilyName As String,
        fontStyle As FontStyle,
        faktorSymbolSize As Decimal) As Font

        'Hinweis: Mit dem Faktor muß außerhalb der Messung multipliziert werden,
        '         die testSize damit zu multiplizieren führt zum falschem Ergebnis. 
        If g Is Nothing Then Throw New ArgumentNullException(NameOf(g))

        Dim fontFamily As FontFamily = SymbolFontManager.ResolveFontFamily(preferredFontFamilyName)

        Dim startSize As Single = rect.Height * 0.9F
        If startSize < 6.0F Then startSize = 6.0F

        Dim factor As Single = CSng(faktorSymbolSize)

        'Die Konstante vergrößert das Segeo-Symbol auf die Defaultgröße
        factor *= 1.15F

        If factor <= 0.5F Then factor = 0.5F

        Dim testSize As Single = startSize

        Do While testSize >= 4.0F

            Using candidate As New Font(fontFamily, testSize, fontStyle, GraphicsUnit.Pixel)

                Dim measured As SizeF = g.MeasureString(text, candidate)

                If measured.Width <= rect.Width AndAlso measured.Height <= rect.Height Then
                    Exit Do
                End If

            End Using

            testSize -= 1.0F

        Loop

        Return New Font(fontFamily, testSize * factor, fontStyle, GraphicsUnit.Pixel)

    End Function

    Private Function GetTargetRect(layout As TileLayout, colors As TileColors) As RectangleF

        Dim outlineWidth As Single = CSng(layout.FaktorBasisWidthToAktWidth) * colors.GetFaktorSymbolOutlineWidth

        Dim drawRect As RectangleF

        With layout.SymbolRect

            'Die beiden < 1-Faktoren zentrieren das Segeo-Symbol bei den OffsetTop und OffsetLeft-
            'Faktoren von 1 auf die Defaultposition.
            Dim addY As Integer = CInt(.Width * -0.35 * colors.GetFaktorSymbolOffsetTop / 20)
            Dim addX As Integer = CInt(.Height * 0.6 * colors.GetFaktorSymbolOffsetLeft / 20)

            drawRect = RectangleF.FromLTRB(
                CSng(.Left + addX),
                CSng(.Top + addY),
                CSng(.Right + addX),
                CSng(.Bottom + addY))
        End With

        Return drawRect

    End Function

End Module
