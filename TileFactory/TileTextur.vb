Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

Imports System.Drawing
Imports TileFactory.TileColors

Public Module TileTextur

    Private ReadOnly _seedRnd As New Random()

    Private Const SCHWELLWERT As Integer = 250

    Public Function CreateMaterialTextureMap(lightMap As Bitmap, texVal As FaceTexturValues, faktorTileBasisHeightToAktHeight As Double) As Single(,)

        'Debug.Print(texVal.ToString)

        With texVal
            If lightMap Is Nothing Then Throw New ArgumentNullException(NameOf(lightMap))
            If lightMap.Width <= 0 Then Throw New ArgumentOutOfRangeException(NameOf(lightMap))
            If lightMap.Height <= 0 Then Throw New ArgumentOutOfRangeException(NameOf(lightMap))

            If .innerCloudStrength < 0.0F Then Throw New ArgumentOutOfRangeException(NameOf(.innerCloudStrength))
            If .innerGrainStrength < 0.0F Then Throw New ArgumentOutOfRangeException(NameOf(.innerGrainStrength))
            If .outerCloudStrength < 0.0F Then Throw New ArgumentOutOfRangeException(NameOf(.outerCloudStrength))
            If .outerGrainStrength < 0.0F Then Throw New ArgumentOutOfRangeException(NameOf(.outerGrainStrength))

            Dim width As Integer = lightMap.Width
            Dim height As Integer = lightMap.Height

            Dim innerRect As Rectangle
            If .useDifferentInnerArea Then
                innerRect = FindInnerAreaFromLightMap(lightMap)
                If innerRect.IsEmpty Then
                    .useDifferentInnerArea = False
                End If
            End If

            Dim result(width - 1, height - 1) As Single

            Dim masterSeed As Integer

            If .seed.HasValue Then
                masterSeed = .seed.Value
            Else
                masterSeed = _seedRnd.Next()
            End If

            Dim innerRnd As New Random(masterSeed)
            Dim outerRnd As New Random(masterSeed Xor &H13579BDF)
            Dim grainRnd As New Random(masterSeed Xor &H2468ACE)
            Dim innerIsZero As Boolean
            Dim outerIsZero As Boolean

            Dim innerCloudMap(,) As Single
            If .innerCloudStrength = 0 AndAlso .innerGrainStrength = 0 Then
                ReDim innerCloudMap(width - 1, height - 1)
                innerIsZero = True
            Else
                innerCloudMap = CreateCloudMap(width, height, faktorTileBasisHeightToAktHeight, innerRnd)
            End If

            Dim outerCloudMap(,) As Single
            If .outerCloudStrength = 0 AndAlso .outerGrainStrength = 0 Then
                ReDim outerCloudMap(width - 1, height - 1)
                outerIsZero = True
            Else
                outerCloudMap = CreateCloudMap(width, height, faktorTileBasisHeightToAktHeight, outerRnd)
            End If

            'If texVal.steinStatus = SteinStatus.I02Selected Then
            '    Stop
            'End If

            For y As Integer = 0 To height - 1
                For x As Integer = 0 To width - 1

                    Dim cloud As Single
                    Dim grain As Single
                    Dim cloudStrength As Single
                    Dim grainStrength As Single

                    If .useDifferentInnerArea Then
                        If innerRect.Contains(x, y) Then
                            If innerIsZero Then
                                result(x, y) = 1
                                Continue For
                            Else
                                cloud = innerCloudMap(x, y)
                                cloudStrength = .innerCloudStrength
                                grainStrength = .innerGrainStrength
                            End If
                        Else
                            If outerIsZero Then
                                result(x, y) = 1
                                Continue For
                            Else
                                cloud = outerCloudMap(x, y)
                                cloudStrength = .outerCloudStrength
                                grainStrength = .outerGrainStrength
                            End If
                        End If
                    Else
                        cloud = outerCloudMap(x, y)
                        cloudStrength = .outerCloudStrength
                        grainStrength = .outerGrainStrength
                    End If

                    grain = CSng((grainRnd.NextDouble() - 0.5R) * 2.0R)

                    Dim factor As Single = 1.0F + cloud * cloudStrength + grain * grainStrength

                    'If factor < 0.65F Then factor = 0.65F
                    'If factor > 1.35F Then factor = 1.35F

                    If factor < 0.5F Then factor = 0.5F
                    If factor > 1.5F Then factor = 1.5F
                    result(x, y) = factor

                Next
            Next

            Return result

        End With
    End Function

    Private Function FindInnerAreaFromLightMap(lightMap As Bitmap) As Rectangle

        Dim centerX As Integer = lightMap.Width \ 2
        Dim centerY As Integer = lightMap.Height \ 2

        Dim left As Integer = centerX
        Dim right As Integer = centerX
        Dim top As Integer = centerY
        Dim bottom As Integer = centerY

        For x As Integer = centerX To 0 Step -1
            If lightMap.GetPixel(x, centerY).R < SCHWELLWERT Then
                left = x + 1
                Exit For
            End If
        Next

        For x As Integer = centerX To lightMap.Width - 1
            If lightMap.GetPixel(x, centerY).R < SCHWELLWERT Then
                right = x - 1
                Exit For
            End If
        Next

        For y As Integer = centerY To 0 Step -1
            If lightMap.GetPixel(centerX, y).R < SCHWELLWERT Then
                top = y + 1
                Exit For
            End If
        Next

        For y As Integer = centerY To lightMap.Height - 1
            If lightMap.GetPixel(centerX, y).R < SCHWELLWERT Then
                bottom = y - 1
                Exit For
            End If
        Next

        If right < left OrElse bottom < top Then
            Return Rectangle.Empty
            'Throw New InvalidOperationException("Innenfläche konnte aus der Lightmap nicht bestimmt werden.")
        End If

        Return Rectangle.FromLTRB(left, top, right + 1, bottom + 1)

    End Function

    Private Function CreateCloudMap(width As Integer,
                                height As Integer,
                                faktorTileBasisHeightToAktHeight As Double,
                                rnd As Random) As Single(,)

        Dim cloud1(,) As Single = CreateValueNoise(width, height, 18, faktorTileBasisHeightToAktHeight, rnd)
        Dim cloud2(,) As Single = CreateValueNoise(width, height, 37, faktorTileBasisHeightToAktHeight, rnd)
        Dim cloud3(,) As Single = CreateValueNoise(width, height, 73, faktorTileBasisHeightToAktHeight, rnd)

        Dim result(width - 1, height - 1) As Single

        For y As Integer = 0 To height - 1
            For x As Integer = 0 To width - 1

                Dim cloud As Single =
                cloud1(x, y) * 0.55F +
                cloud2(x, y) * 0.3F +
                cloud3(x, y) * 0.15F

                result(x, y) = (cloud - 0.5F) * 2.0F

            Next
        Next

        Return result

    End Function

    Private Function CreateValueNoise(width As Integer,
                                  height As Integer,
                                  cellSize As Integer,
                                  faktorTileBasisHeightToAktHeight As Double,
                                  Rnd As Random) As Single(,)

        cellSize = CInt(CDbl(cellSize) * faktorTileBasisHeightToAktHeight)

        If cellSize < 2 Then cellSize = 2

        Dim gridWidth As Integer = CInt(Math.Ceiling(width / CDbl(cellSize))) + 2
        Dim gridHeight As Integer = CInt(Math.Ceiling(height / CDbl(cellSize))) + 2

        Dim grid(gridWidth - 1, gridHeight - 1) As Single

        For gy As Integer = 0 To gridHeight - 1
            For gx As Integer = 0 To gridWidth - 1
                grid(gx, gy) = CSng(Rnd.NextDouble())
            Next
        Next

        Dim map(width - 1, height - 1) As Single

        For y As Integer = 0 To height - 1
            Dim fy As Single = CSng(y / CSng(cellSize))
            Dim gy0 As Integer = CInt(Math.Floor(fy))
            Dim ty As Single = SmoothStep(fy - gy0)

            For x As Integer = 0 To width - 1
                Dim fx As Single = CSng(x / CSng(cellSize))
                Dim gx0 As Integer = CInt(Math.Floor(fx))
                Dim tx As Single = SmoothStep(fx - gx0)

                Dim v00 As Single = grid(gx0, gy0)
                Dim v10 As Single = grid(gx0 + 1, gy0)
                Dim v01 As Single = grid(gx0, gy0 + 1)
                Dim v11 As Single = grid(gx0 + 1, gy0 + 1)

                Dim a As Single = Lerp(v00, v10, tx)
                Dim b As Single = Lerp(v01, v11, tx)

                map(x, y) = Lerp(a, b, ty)
            Next
        Next

        Return map

    End Function

    Private Function SmoothStep(t As Single) As Single

        If t < 0.0F Then Return 0.0F
        If t > 1.0F Then Return 1.0F

        Return t * t * (3.0F - 2.0F * t)

    End Function

    Private Function Lerp(a As Single,
                      b As Single,
                      t As Single) As Single

        Return a + (b - a) * t

    End Function

    Public Sub InsertTexturMap(ByRef bmpLayer As Bitmap, ByVal texturMap As Single(,))

        If bmpLayer Is Nothing Then
            Throw New ArgumentNullException(NameOf(bmpLayer))
        End If

        If texturMap Is Nothing Then
            Throw New ArgumentNullException(NameOf(texturMap))
        End If

        Dim width As Integer = bmpLayer.Width
        Dim height As Integer = bmpLayer.Height

        If texturMap.GetLength(0) <> width OrElse texturMap.GetLength(1) <> height Then
            Throw New ArgumentException("bmpLayer und texturMap müssen gleich groß sein.")
        End If

        If bmpLayer.PixelFormat <> Imaging.PixelFormat.Format32bppArgb Then
            Dim converted As New Bitmap(width, height, Imaging.PixelFormat.Format32bppArgb)

            Using g As Graphics = Graphics.FromImage(converted)
                g.DrawImageUnscaled(bmpLayer, 0, 0)
            End Using

            bmpLayer.Dispose()
            bmpLayer = converted
        End If

        Dim rect As New Rectangle(0, 0, width, height)

        Dim data As Imaging.BitmapData =
            bmpLayer.LockBits(rect,
                              Imaging.ImageLockMode.ReadWrite,
                              Imaging.PixelFormat.Format32bppArgb)

        Try
            Dim stride As Integer = data.Stride
            Dim byteCount As Integer = Math.Abs(stride) * height
            Dim buffer(byteCount - 1) As Byte

            Runtime.InteropServices.Marshal.Copy(data.Scan0, buffer, 0, byteCount)

            For y As Integer = 0 To height - 1

                Dim rowOffset As Integer = y * stride

                For x As Integer = 0 To width - 1

                    Dim p As Integer = rowOffset + x * 4

                    Dim a As Byte = buffer(p + 3)

                    If a <> 0 Then

                        Dim f As Single = texturMap(x, y)

                        If f < 0.0F Then
                            f = 0.0F
                        End If

                        Dim b As Integer = CInt(buffer(p) * f)
                        Dim g As Integer = CInt(buffer(p + 1) * f)
                        Dim r As Integer = CInt(buffer(p + 2) * f)

                        If b > 255 Then b = 255
                        If g > 255 Then g = 255
                        If r > 255 Then r = 255

                        buffer(p) = CByte(b)
                        buffer(p + 1) = CByte(g)
                        buffer(p + 2) = CByte(r)

                        ' Alpha bleibt unverändert.
                    End If

                Next

            Next

            Runtime.InteropServices.Marshal.Copy(buffer, 0, data.Scan0, byteCount)

        Finally
            bmpLayer.UnlockBits(data)
        End Try

    End Sub
End Module
