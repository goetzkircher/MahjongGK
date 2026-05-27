Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

Imports System.Drawing
Imports MahjongGK.Contracts.GlobalEnum

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

        'Basisbitmaps erstellen
        Dim baseBmp As Bitmap = TileRenderer.CreateBaseTileBitmap(request, layout, colors)
        Dim symbolBmp As Bitmap = TileSymbolRenderer.CreateSymbolLayerBitmap(request, layout, colors)

        Dim faceBmp As Bitmap = TileFaceWithoutSymbolFactory.CreateFaceWithoutSymbolBitmap(request, layout, colors)

        'Summenänderung:
        Dim sumAdjustFaceValues As TileColors.AdjustAlphaSatBrgColorizeValues = colors.GetSumAdjustValues(faceValues:=True)
        If sumAdjustFaceValues.HasValues Then
            faceBmp = AdjustAlphaSatBrgColorizeProzent(faceBmp, sumAdjustFaceValues)
            If request.SteinGhost = SteinGhost.None Then
                'Im Normalfall bleibt das Symbol unverändert, nur der Alpha-Wert wird angepasst,
                'denn andernfall deckt das Symbol eine eventuelle Transparenz zu.
                If sumAdjustFaceValues.alphaAbsolut <> 255 Then
                    Dim adjustAlpha As New TileColors.AdjustAlphaSatBrgColorizeValues
                    adjustAlpha.alphaAbsolut = sumAdjustFaceValues.alphaAbsolut
                    symbolBmp = AdjustAlphaSatBrgColorizeProzent(symbolBmp, adjustAlpha)
                End If

            Else
                'im Geisterfall wird es komplett wie das Face behandelt.
                symbolBmp = AdjustAlphaSatBrgColorizeProzent(symbolBmp, sumAdjustFaceValues)
            End If
        End If

        Dim sumAdjustLayerValues As TileColors.AdjustAlphaSatBrgColorizeValues = colors.GetSumAdjustValues(faceValues:=False)
        If sumAdjustLayerValues.HasValues Then
            baseBmp = AdjustAlphaSatBrgColorizeProzent(baseBmp, sumAdjustLayerValues)
        End If

        If request.SteinGhost <> SteinGhost.None Then
            'Geisterzeugung
            Dim ghostAdjustLayerValues As TileColors.AdjustAlphaSatBrgColorizeValues = colors.GetCvtToGhostValues(request.SteinGhost, faceValues:=False)
            If ghostAdjustLayerValues.HasValues Then
                baseBmp = AdjustAlphaSatBrgColorizeProzent(baseBmp, ghostAdjustLayerValues)
            End If
            Dim ghostAdjustFaceValues As TileColors.AdjustAlphaSatBrgColorizeValues = colors.GetCvtToGhostValues(request.SteinGhost, faceValues:=True)
            If ghostAdjustFaceValues.HasValues Then
                faceBmp = AdjustAlphaSatBrgColorizeProzent(faceBmp, ghostAdjustFaceValues)
                symbolBmp = AdjustAlphaSatBrgColorizeProzent(symbolBmp, ghostAdjustFaceValues)
            End If
        End If

        Try

            Dim bmpResult As New Bitmap(layout.SteinSize.Width, layout.SteinSize.Height, Imaging.PixelFormat.Format32bppArgb)

            Using g As Graphics = Graphics.FromImage(bmpResult)

                g.Clear(Color.Transparent)
                SetGraphicsMode(g, sourceOver:=True)

                g.DrawImageUnscaled(baseBmp, 0, 0)
                g.DrawImageUnscaled(faceBmp, layout.FaceOuterRect.Left, layout.FaceOuterRect.Top)
                g.DrawImageUnscaled(symbolBmp, 0, 0)

            End Using

            bmpResult.Tag = Now.ToLongTimeString

            Debug.Print($"CreateBitmap {request.SteinSymbol} {request.SteinGhost} {Now}")
            Return bmpResult

        Finally
            baseBmp?.Dispose()
            symbolBmp?.Dispose()
            faceBmp?.Dispose()
        End Try

    End Function

End Module
