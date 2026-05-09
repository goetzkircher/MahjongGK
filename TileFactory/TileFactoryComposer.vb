Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

Imports System.Drawing

'
''' <summary>
''' Setzt die einzelnen Render-Ebenen zu einer Endbitmap zusammen.
''' </summary>
Friend Module TileFactoryComposer

    '
    ''' <summary>
    ''' Erzeugt die Endbitmap eines Steines.
    ''' Cache folgt später.
    ''' </summary>
    Public Function CreateTileBitmap(request As TileRequest) As Bitmap

        Dim layout As New TileLayout(request.SteinSize, request.SteinBasisSize)
        Dim colors As TileColors = request.TileColors

        Dim baseBmp As Bitmap = Nothing
        Dim symbolBmp As Bitmap = Nothing
        Dim overlayBmp As Bitmap = Nothing
        Dim gradientBmp As Bitmap = Nothing
        Dim resultBmp As Bitmap = Nothing

        Try
            baseBmp = TileRenderer.CreateBaseTileBitmap(request, layout, colors)
            symbolBmp = TileSymbolRenderer.CreateSymbolLayerBitmap(request, layout, colors)
            gradientBmp = TileOverlayGradientFactory.CreateGradientOverlayBitmap(request, layout, colors)

            If colors.GetInsertFaceFrame Then
                overlayBmp = TileOverlayFactory.CreateFrameOverlayBitmap(request, layout, colors)
            End If

            resultBmp = ComposeLayers(
                layout,
                colors,
                baseBmp:=baseBmp,
                gradientBmp:=gradientBmp,
                overlayBmp:=overlayBmp,
                symbolBmp:=symbolBmp)

            resultBmp.Tag = Now.ToLongTimeString

        Finally
            If baseBmp IsNot Nothing Then baseBmp.Dispose()
            If symbolBmp IsNot Nothing Then symbolBmp.Dispose()
            If overlayBmp IsNot Nothing Then overlayBmp.Dispose()
            If gradientBmp IsNot Nothing Then gradientBmp.Dispose()
        End Try

        Return resultBmp

    End Function

    '
    ''' <summary>
    ''' Fügt die drei Ebenen zusammen.
    ''' </summary>
    Private Function ComposeLayers(layout As TileLayout,
                               colors As TileColors,
                               baseBmp As Bitmap,
                               gradientBmp As Bitmap,
                               overlayBmp As Bitmap,
                               symbolBmp As Bitmap) As Bitmap

        Dim brgSummenÄnderung As Decimal
        Dim satSummenÄnderung As Decimal
        Dim changeSatOrBrg As Boolean

        If colors.DBrgSummenÄnderungMultiplikation Then
            brgSummenÄnderung = colors.DBrgSummenÄnderungMul
            changeSatOrBrg = brgSummenÄnderung <> 1D
        Else
            brgSummenÄnderung = colors.DBrgSummenÄnderungAdd
            changeSatOrBrg = brgSummenÄnderung <> 0D
        End If

        satSummenÄnderung = colors.DSatSummenÄnderung
        If satSummenÄnderung <> 1D Then
            changeSatOrBrg = True
        End If

        Dim bmpResult As New Bitmap(layout.SteinSize.Width,
                             layout.SteinSize.Height,
                             Imaging.PixelFormat.Format32bppArgb)

        Using g As Graphics = Graphics.FromImage(bmpResult)

            Dim ok As Boolean = False

            g.Clear(Color.Transparent)
            SetGraphicsMode(g, sourceOver:=True)

            If Not changeSatOrBrg Then

                If baseBmp IsNot Nothing Then
                    g.DrawImageUnscaled(baseBmp, 0, 0)
                    ok = True
                End If

                If gradientBmp IsNot Nothing Then
                    g.DrawImageUnscaled(gradientBmp, layout.FaceOuterRect.Left, layout.FaceOuterRect.Top)
                    ok = True
                End If

                If overlayBmp IsNot Nothing Then
                    g.DrawImageUnscaled(overlayBmp, 0, 0)
                    ok = True
                End If

            Else

                Using ia As Imaging.ImageAttributes =
                        CreateSaturationAndBrightnessImageAttributes(satSummenÄnderung,
                                                                     brgSummenÄnderung,
                                                                     colors.DBrgSummenÄnderungMultiplikation)

                    If baseBmp IsNot Nothing Then
                        DrawImageAdjusted(g, baseBmp, 0, 0, ia)
                        ok = True
                    End If

                    If gradientBmp IsNot Nothing Then
                        DrawImageAdjusted(g,
                                      gradientBmp,
                                      layout.FaceOuterRect.Left,
                                      layout.FaceOuterRect.Top,
                                      ia)
                        ok = True
                    End If

                    If overlayBmp IsNot Nothing Then
                        DrawImageAdjusted(g, overlayBmp, 0, 0, ia)
                        ok = True
                    End If

                End Using

            End If

            ' Symbol bewusst NICHT anpassen.
            If symbolBmp IsNot Nothing Then
                g.DrawImageUnscaled(symbolBmp, 0, 0)
            End If

            If Not ok Then
                g.Clear(Color.Red)
            End If

        End Using

        Return bmpResult

    End Function

    Private Sub DrawImageAdjusted(g As Graphics,
                              bmp As Bitmap,
                              x As Integer,
                              y As Integer,
                              ia As Imaging.ImageAttributes)

        If bmp Is Nothing Then
            Return
        End If

        Dim destRect As New Rectangle(x, y, bmp.Width, bmp.Height)

        g.DrawImage(
            bmp,
            destRect,
            0,
            0,
            bmp.Width,
            bmp.Height,
            GraphicsUnit.Pixel,
            ia)

    End Sub

    Private Function CreateSaturationAndBrightnessImageAttributes(satFaktor As Decimal,
                                                              brgFactor As Decimal,
                                                              modusMultiplikation As Boolean) As Imaging.ImageAttributes

        Dim cm As Imaging.ColorMatrix =
            CreateSaturationAndBrightnessColorMatrix(satFaktor, brgFactor, modusMultiplikation)

        Dim ia As New Imaging.ImageAttributes()

        ia.SetColorMatrix(cm,
                          Imaging.ColorMatrixFlag.Default,
                          Imaging.ColorAdjustType.Bitmap)

        Return ia

    End Function

    Private Function CreateSaturationAndBrightnessColorMatrix(satFaktor As Decimal,
                                                          brgFactor As Decimal,
                                                          modusMultiplikation As Boolean) As Imaging.ColorMatrix

        If satFaktor < 0D Then
            Throw New ArgumentOutOfRangeException(NameOf(satFaktor))
        End If

        If modusMultiplikation AndAlso brgFactor < 0D Then
            Throw New ArgumentOutOfRangeException(NameOf(brgFactor))
        End If

        Dim s As Single = CSng(satFaktor)
        Dim b As Single = CSng(brgFactor)

        Const lumR As Single = 0.3086F
        Const lumG As Single = 0.6094F
        Const lumB As Single = 0.082F

        Dim sr As Single = (1.0F - s) * lumR
        Dim sg As Single = (1.0F - s) * lumG
        Dim sb As Single = (1.0F - s) * lumB

        Dim m00 As Single = sr + s
        Dim m01 As Single = sg
        Dim m02 As Single = sb

        Dim m10 As Single = sr
        Dim m11 As Single = sg + s
        Dim m12 As Single = sb

        Dim m20 As Single = sr
        Dim m21 As Single = sg
        Dim m22 As Single = sb + s

        Dim addB As Single = 0.0F

        If modusMultiplikation Then

            m00 *= b
            m01 *= b
            m02 *= b

            m10 *= b
            m11 *= b
            m12 *= b

            m20 *= b
            m21 *= b
            m22 *= b

        Else

            addB = b

        End If

        Return New Imaging.ColorMatrix(
            New Single()() {
                New Single() {m00, m01, m02, 0.0F, 0.0F},
                New Single() {m10, m11, m12, 0.0F, 0.0F},
                New Single() {m20, m21, m22, 0.0F, 0.0F},
                New Single() {0.0F, 0.0F, 0.0F, 1.0F, 0.0F},
                New Single() {addB, addB, addB, 0.0F, 1.0F}
            })

    End Function

End Module
