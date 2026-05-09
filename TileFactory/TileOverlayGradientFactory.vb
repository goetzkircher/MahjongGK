Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.Runtime.InteropServices

'
''' <summary>
''' Erzeugt eine transparente Bitmap mit einer skalierten LightMap
''' für die Oberseite eines Mahjong-Steines.
''' </summary>
Friend NotInheritable Class TileOverlayGradientFactory

    Public Shared Function CreateGradientOverlayBitmap(request As TileRequest,
                                                   layout As TileLayout,
                                                   colors As TileColors) As Bitmap

        If request Is Nothing Then Throw New ArgumentNullException(NameOf(request))
        If layout Is Nothing Then Throw New ArgumentNullException(NameOf(layout))
        If colors Is Nothing Then Throw New ArgumentNullException(NameOf(colors))

        Dim bmpResult As New Bitmap(layout.FaceOuterRect.Width,
                                layout.FaceOuterRect.Height,
                                PixelFormat.Format32bppArgb)

        'Wichtig:
        'Wenn Ressouren.Image_LichtkarteDiagonal(request) eine gecachte Bitmap liefert,
        'darf sie hier NICHT disposed werden. (wird sonst jedesmal neu erzeugt)
        'Siehe bmpLichtkarte.Dispose() am Funktionsende
        '
        Dim bmpLichtkarte As Bitmap = ResLichtkarten.Image_Lichtkarte(colors.GetFaceLightMap, request)

        'bmpLichtkarte.Save($"C:\Users\goetz\Documents\_tmp2\LightMap {Now.Ticks.ToString.Substring(8, 5)} {bmpLichtkarte.Width.ToString}x{bmpLichtkarte.Height.ToString}.png")

        If bmpLichtkarte.Width <> bmpResult.Width OrElse bmpLichtkarte.Height <> bmpResult.Height Then
            Throw New InvalidOperationException("bmpLichtkarte und bmpResult müssen gleich groß sein.")
        End If

        Dim texturMap As Single(,) = CreateMaterialTextureMap(bmpLichtkarte, colors.GetFaceTexturValues, layout.FaktorBasisHeightToAktHeight)

        Dim basisCol As Color = colors.GetColSteinBasisColor
        Dim rc As New Rectangle(0, 0, bmpResult.Width, bmpResult.Height)

        Dim srcData As BitmapData = Nothing
        Dim dstData As BitmapData = Nothing

        Try
            srcData = bmpLichtkarte.LockBits(rc, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb)
            dstData = bmpResult.LockBits(rc, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb)

            Dim srcStride As Integer = srcData.Stride
            Dim dstStride As Integer = dstData.Stride

            Dim srcByteCount As Integer = srcStride * srcData.Height
            Dim dstByteCount As Integer = dstStride * dstData.Height

            Dim srcBytes(srcByteCount - 1) As Byte
            Dim dstBytes(dstByteCount - 1) As Byte

            Marshal.Copy(srcData.Scan0, srcBytes, 0, srcByteCount)

            For y As Integer = 0 To rc.Height - 1

                Dim srcRow As Integer = y * srcStride
                Dim dstRow As Integer = y * dstStride

                For x As Integer = 0 To rc.Width - 1

                    Dim srcIndex As Integer = srcRow + (x * 4)
                    Dim dstIndex As Integer = dstRow + (x * 4)

                    'Format32bppArgb in Memory: B, G, R, A
                    Dim srcB As Integer = CInt(srcBytes(srcIndex))
                    Dim srcG As Integer = CInt(srcBytes(srcIndex + 1))
                    Dim srcR As Integer = CInt(srcBytes(srcIndex + 2))
                    Dim srcA As Integer = CInt(srcBytes(srcIndex + 3))

                    'Sicherheitshalber Mittelwert, auch wenn die LightMap grau ist
                    Dim gray As Integer = colors.GetShiftLightMapLookupValue((srcR + srcG + srcB) \ 3)
                    gray = CInt(gray * texturMap(x, y))
                    If gray > 255 Then gray = 255

                    dstBytes(dstIndex) = CByte((gray * CInt(basisCol.B)) \ 255)
                    dstBytes(dstIndex + 1) = CByte((gray * CInt(basisCol.G)) \ 255)
                    dstBytes(dstIndex + 2) = CByte((gray * CInt(basisCol.R)) \ 255)
                    dstBytes(dstIndex + 3) = CByte((srcA * CInt(basisCol.A)) \ 255)

                Next
            Next

            Marshal.Copy(dstBytes, 0, dstData.Scan0, dstByteCount)

        Finally
            If dstData IsNot Nothing Then
                bmpResult.UnlockBits(dstData)
            End If

            If srcData IsNot Nothing Then
                bmpLichtkarte.UnlockBits(srcData)
            End If
        End Try
        '
        Return bmpResult

    End Function

End Class