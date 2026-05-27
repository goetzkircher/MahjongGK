Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On
Imports System.Drawing

#Disable Warning IDE0079
#Disable Warning IDE1006
#Disable Warning IDE0031
#Disable Warning IDE0017

Friend Module TileTextRenderer

    Private _yCorrectionBitmap() As Bitmap
    Private _yCorrectionHash() As Integer
    Private _yCorrectionIsInit As Boolean
    Private Const UBND15 As Integer = 15 ' = txs.txtSymb.Length * 2 - 1
    Private Const UBND7 As Integer = 7

    Public Function GetTileTextBitmap(trv As TileColors.TextRenderValues) As Bitmap

        If Not _yCorrectionIsInit Then
            ReDim _yCorrectionBitmap(UBND15)
            ReDim _yCorrectionHash(UBND15)
            _yCorrectionIsInit = True
        End If

        Dim idxCache As Integer = -1

        Dim symbolText As String = trv.Text
        Select Case symbolText.Length
            Case 1
                'trv.Symbols.txtSymb ist geprüft 8 Zeichen lang 
                idxCache = trv.Symbols.txtSymb.IndexOf(symbolText)
            Case 2
                idxCache = trv.Symbols.txtSymb.IndexOf(symbolText.Substring(0, 1))
                If idxCache >= 0 Then
                    Dim idx As Integer = trv.Symbols.txtSymb.IndexOf(symbolText.Substring(1, 1))
                    If idx = UBND7 Then
                        idxCache += UBND7 + 1
                    Else
                        idxCache = -1
                    End If
                End If

            Case Else
                Throw New Exception("Programmierfehler: Maximal zwei Zeichen erlaubt.")
        End Select

        If idxCache = -1 Then
            Throw New Exception("Programmierfehler: Zeichen stammt nicht aus diesem Zeichensatz.")
        End If

        Dim found As Boolean = True
        Dim hash As Integer = GetTextCacheHash(trv)

        If _yCorrectionHash(idxCache) = 0 Then
            found = False
        Else
            If _yCorrectionHash(idxCache) <> hash Then
                'Irgendwas hat sich geändert ==> alle Zeichen neu vermessen.
                ReDim _yCorrectionHash(UBND15)
                found = False
            End If
        End If

        If found Then
            Return _yCorrectionBitmap(idxCache)
        End If
        '
        DisposeYCorrectedBitmap(idxCache)

        Dim newHash As Integer = GetTextCacheHash(trv)

        Dim textUgrdDiameter As Single = trv.TextUgrdDiameter
        Dim bmpSize As Integer = CInt(Math.Ceiling(trv.TextUgrdDiameter))
        Dim fontSize As Single = trv.FontSize

        If bmpSize <= 0 Then
            Dim fontFamily As FontFamily = SymbolFontManager.ResolveFontFamily(trv.FontFamilyName)
            Using font As New Font(fontFamily, fontSize, trv.FontStyle, GraphicsUnit.Pixel)

                textUgrdDiameter = GetTextUgrdDiameter(symbolText, font)
                bmpSize = CInt(textUgrdDiameter)
            End Using
        End If

        Using tmpBmp As New Bitmap(bmpSize, bmpSize, Imaging.PixelFormat.Format32bppArgb)

            Using gTmp As Graphics = Graphics.FromImage(tmpBmp),
                  brText As New SolidBrush(Color.White)

                gTmp.Clear(Color.Transparent)

                gTmp.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
                gTmp.TextRenderingHint = Drawing.Text.TextRenderingHint.AntiAliasGridFit

                Dim fontFamily As FontFamily = SymbolFontManager.ResolveFontFamily(trv.FontFamilyName)

                Using font As New Font(fontFamily, fontSize, trv.FontStyle, GraphicsUnit.Pixel)

                    Using sf As New StringFormat(StringFormat.GenericTypographic)

                        sf.Alignment = StringAlignment.Center
                        sf.LineAlignment = StringAlignment.Center
                        sf.FormatFlags = sf.FormatFlags Or StringFormatFlags.NoClip

                        Dim rect As New RectangleF(0.0F, 0.0F, textUgrdDiameter, textUgrdDiameter)

                        gTmp.DrawString(symbolText, font, brText, rect, sf)

                    End Using

                End Using

            End Using

            Dim bounds As Rectangle = GetAlphaBoundsFast(tmpBmp)

            If bounds.IsEmpty Then
                ' Zeichen erzeugt keine sichtbaren Pixel. (Leerzeichen)
                Dim result2 As New Bitmap(bmpSize, bmpSize, Imaging.PixelFormat.Format32bppArgb)
                Using gResult As Graphics = Graphics.FromImage(result2)
                    gResult.Clear(Color.Transparent)
                End Using
                Return result2
            End If

            Dim visibleCenterY As Single = CSng(bounds.Top + bounds.Bottom - 1) / 2.0F
            Dim wantedCenterY As Single = textUgrdDiameter / 2.0F
            Dim yCorrection As Single = wantedCenterY - visibleCenterY

            If symbolText = "▲" Then
                yCorrection *= 1.9F
            End If

            Dim result As New Bitmap(bmpSize, bmpSize, Imaging.PixelFormat.Format32bppArgb)

            Using gResult As Graphics = Graphics.FromImage(result),
                  brCircle As New SolidBrush(trv.TextUgrdColor),
                  brText As New SolidBrush(trv.TextColor)

                gResult.Clear(Color.Transparent)

                gResult.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
                gResult.TextRenderingHint = Drawing.Text.TextRenderingHint.AntiAliasGridFit

                Dim circleRect As New RectangleF(0.0F, 0.0F, textUgrdDiameter, textUgrdDiameter)

                If trv.UseUgrd AndAlso textUgrdDiameter > 0 Then

                    If trv.TextUgrdCenterColor.A = 0 Then
                        gResult.FillEllipse(brCircle, circleRect)
                    Else
                        Dim circlePath As New Drawing2D.GraphicsPath()
                        circlePath.AddEllipse(circleRect)
                        Using pathBrush As New Drawing2D.PathGradientBrush(circlePath)

                            pathBrush.CenterColor = trv.TextUgrdCenterColor
                            pathBrush.SurroundColors = {trv.TextUgrdColor}

                            gResult.FillEllipse(pathBrush, circleRect)

                        End Using
                        If trv.TextUgrdBorderColor.A > 0 Then
                            Using pn As New Pen(trv.TextUgrdBorderColor, 1.0F)
                                With circleRect
                                    gResult.DrawEllipse(pn, New RectangleF(.Left, .Top, .Width - 1, .Height - 1))
                                End With
                            End Using
                        End If
                    End If
                End If

                Dim fontFamily As FontFamily = SymbolFontManager.ResolveFontFamily(trv.FontFamilyName)

                Using font As New Font(fontFamily, fontSize, trv.FontStyle, GraphicsUnit.Pixel)

                    Using sf As New StringFormat(StringFormat.GenericTypographic)

                        sf.Alignment = StringAlignment.Center
                        sf.LineAlignment = StringAlignment.Center
                        sf.FormatFlags = sf.FormatFlags Or StringFormatFlags.NoClip

                        Dim textRect As RectangleF = circleRect
                        textRect.Y += yCorrection

                        gResult.DrawString(symbolText, font, brText, textRect, sf)

                    End Using

                End Using

            End Using

            _yCorrectionBitmap(idxCache) = result
            _yCorrectionHash(idxCache) = newHash

            Return result

        End Using
    End Function

    Private Function GetAlphaBoundsFast(bmp As Bitmap) As Rectangle

        Dim rect As New Rectangle(0, 0, bmp.Width, bmp.Height)

        Dim data As Imaging.BitmapData = bmp.LockBits(
        rect,
        Imaging.ImageLockMode.ReadOnly,
        Imaging.PixelFormat.Format32bppArgb)

        Try
            Dim stride As Integer = data.Stride
            Dim bytesCount As Integer = Math.Abs(stride) * bmp.Height
            Dim buffer(bytesCount - 1) As Byte

            Runtime.InteropServices.Marshal.Copy(data.Scan0, buffer, 0, bytesCount)

            Dim left As Integer = bmp.Width
            Dim top As Integer = bmp.Height
            Dim right As Integer = -1
            Dim bottom As Integer = -1

            For y As Integer = 0 To bmp.Height - 1

                Dim rowOffset As Integer = y * stride

                For x As Integer = 0 To bmp.Width - 1

                    Dim alphaIndex As Integer = rowOffset + x * 4 + 3

                    If buffer(alphaIndex) <> 0 Then

                        If x < left Then left = x
                        If x > right Then right = x
                        If y < top Then top = y
                        If y > bottom Then bottom = y

                    End If

                Next

            Next

            If right < left OrElse bottom < top Then
                Return Rectangle.Empty
            End If

            Return Rectangle.FromLTRB(left, top, right + 1, bottom + 1)

        Finally
            bmp.UnlockBits(data)
        End Try

    End Function

    Private Function AdjustRect(gfx As Graphics,
                            symbolText As String,
                            font As Font,
                            rect As Rectangle) As Rectangle

        If gfx Is Nothing Then Throw New ArgumentNullException(NameOf(gfx))
        If font Is Nothing Then Throw New ArgumentNullException(NameOf(font))

        If String.IsNullOrEmpty(symbolText) Then
            Return rect
        End If

        Dim textSize As SizeF = gfx.MeasureString(symbolText, font)

        Dim neededWidth As Integer = CInt(Math.Ceiling(textSize.Width))
        Dim neededHeight As Integer = CInt(Math.Ceiling(textSize.Height))

        If rect.Width >= neededWidth AndAlso rect.Height >= neededHeight Then
            Return rect
        End If

        Dim newWidth As Integer = Math.Max(rect.Width, neededWidth)
        Dim newHeight As Integer = Math.Max(rect.Height, neededHeight)

        'Zentrisch um das bisherige Rechteck vergrößern.
        Dim dx As Integer = (newWidth - rect.Width) \ 2
        Dim dy As Integer = (newHeight - rect.Height) \ 2

        Return New Rectangle(rect.Left - dx,
                         rect.Top - dy,
                         newWidth,
                         newHeight)

    End Function

    Private Function GetTextUgrdDiameter(symbolText As String,
                                     font As Font) As Integer

        If font Is Nothing Then Throw New ArgumentNullException(NameOf(font))

        If String.IsNullOrEmpty(symbolText) Then
            Return Math.Max(1, CInt(Math.Ceiling(font.GetHeight())))
        End If

        Using bmpMeasure As New Bitmap(1, 1)
            Using gfxMeasure As Graphics = Graphics.FromImage(bmpMeasure)
                Using sf As New StringFormat(StringFormat.GenericTypographic)

                    sf.Alignment = StringAlignment.Center
                    sf.LineAlignment = StringAlignment.Center
                    sf.FormatFlags = sf.FormatFlags Or StringFormatFlags.NoClip

                    Dim size As SizeF = gfxMeasure.MeasureString(symbolText,
                                                             font,
                                                             PointF.Empty,
                                                             sf)

                    Dim needed As Single = Math.Max(size.Width, size.Height)

                    Return Math.Max(1, CInt(Math.Ceiling(needed)))

                End Using
            End Using
        End Using

    End Function
    Public Sub DisposeYCorrectedBitmap()
        For idx As Integer = 0 To UBND15
            If _yCorrectionBitmap(idx) IsNot Nothing Then
                _yCorrectionBitmap(idx).Dispose()
                _yCorrectionBitmap(idx) = Nothing
            End If
        Next
    End Sub

    Public Sub DisposeYCorrectedBitmap(idxCache As Integer)
        If _yCorrectionBitmap(idxCache) IsNot Nothing Then
            _yCorrectionBitmap(idxCache).Dispose()
            _yCorrectionBitmap(idxCache) = Nothing
        End If
    End Sub

    Public Function GetTextCacheHash(trv As TileColors.TextRenderValues) As Integer

        Dim h As Integer = 17

        AddIntToHash(h, If(trv.Text Is Nothing, 0, trv.Text.GetHashCode()))
        AddIntToHash(h, If(trv.FontFamilyName Is Nothing, 0, trv.FontFamilyName.GetHashCode()))
        AddIntToHash(h, CInt(trv.FontStyle))
        AddIntToHash(h, trv.FaktorTextSize.GetHashCode())
        AddIntToHash(h, trv.TextUgrdDiameter.GetHashCode())
        AddIntToHash(h, trv.FontSize.GetHashCode)
        AddIntToHash(h, trv.SteinSize.Width)
        AddIntToHash(h, trv.SteinSize.Height)
        AddIntToHash(h, trv.TextColor.ToArgb())
        AddIntToHash(h, trv.TextUgrdColor.ToArgb())
        AddIntToHash(h, trv.TextUgrdBorderColor.ToArgb())
        AddIntToHash(h, trv.TextUgrdCenterColor.ToArgb())

        Return h

    End Function

    Private Sub AddIntToHash(ByRef h As Integer, value As Integer)
        Dim tmp As Long = CLng(h) * 31L
        tmp = tmp Xor CLng(value)

        h = CInt(tmp And &H7FFFFFFFL)

    End Sub

End Module
