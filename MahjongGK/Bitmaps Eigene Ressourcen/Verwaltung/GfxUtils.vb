Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On
Imports System.Drawing.Drawing2D
Imports System.Drawing.Imaging
Imports System.IO
Imports System.IO.Compression
Imports System.Text.RegularExpressions

Friend Module GfxUtils

    Private ReadOnly rxSizeQ As New Regex("(?<base>.+?)(?<size>\d{2,3})q$", RegexOptions.Compiled)

    Public Function StripSizeQ(nameWithOptSize As String, ByRef q As Integer?) As String
        Dim m As Match = rxSizeQ.Match(nameWithOptSize)
        If m.Success Then
            Dim s As String = m.Groups("size").Value
            Dim val As Integer
            If Integer.TryParse(s, val) Then
                q = val
            End If
            Return m.Groups("base").Value
        End If
        q = Nothing
        Return nameWithOptSize
    End Function

    Public Function InflateGZip(base64Gz As String) As Byte()
        Dim gzBytes() As Byte = Convert.FromBase64String(base64Gz)
        Using ms As New MemoryStream(gzBytes),
              gz As New GZipStream(ms, CompressionMode.Decompress),
              outMs As New MemoryStream()
            gz.CopyTo(outMs)
            Return outMs.ToArray()
        End Using
    End Function

    Public Function DeflateGZip(data As Byte()) As String
        Using outMs As New MemoryStream()
            Using gz As New GZipStream(outMs, CompressionMode.Compress, True)
                gz.Write(data, 0, data.Length)
            End Using
            Return Convert.ToBase64String(outMs.ToArray())
        End Using
    End Function

    Public Function ResizeBitmap(src As Bitmap, targetW As Integer, targetH As Integer) As Bitmap
        Dim bmp As New Bitmap(targetW, targetH, PixelFormat.Format32bppPArgb)
        bmp.SetResolution(src.HorizontalResolution, src.VerticalResolution)
        Using g As Graphics = Graphics.FromImage(bmp)
            g.InterpolationMode = InterpolationMode.HighQualityBicubic
            g.PixelOffsetMode = PixelOffsetMode.HighQuality
            g.SmoothingMode = SmoothingMode.HighQuality
            g.CompositingQuality = CompositingQuality.HighQuality
            g.DrawImage(src, New Rectangle(0, 0, targetW, targetH),
                        New Rectangle(0, 0, src.Width, src.Height), GraphicsUnit.Pixel)
        End Using
        Return bmp
    End Function

    Public Function MaybeBrighten(img As Bitmap, brightenAmount As Single) As Bitmap
        If brightenAmount <= 0.0001F Then Return img
        ' einfache Brightness-Matrix
        Dim cm As New Imaging.ColorMatrix(New Single()() {
            New Single() {1, 0, 0, 0, 0},
            New Single() {0, 1, 0, 0, 0},
            New Single() {0, 0, 1, 0, 0},
            New Single() {0, 0, 0, 1, 0},
            New Single() {brightenAmount, brightenAmount, brightenAmount, 0, 1}
        })
        Dim ia As New Imaging.ImageAttributes()
        ia.SetColorMatrix(cm)

        Dim clone As New Bitmap(img.Width, img.Height, PixelFormat.Format32bppPArgb)
        clone.SetResolution(img.HorizontalResolution, img.VerticalResolution)
        Using g As Graphics = Graphics.FromImage(clone)
            g.DrawImage(img, New Rectangle(0, 0, img.Width, img.Height), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, ia)
        End Using
        Return clone
    End Function

    ''' <summary>
    ''' liefert einen ByteArray mit dem Inhalt eine PNG.
    ''' </summary>
    ''' <param name="path"></param>
    ''' <returns></returns>
    Public Function ReadBytesFromFile(path As String) As Byte()
        Using fs As FileStream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read)
            Using ms As New MemoryStream()
                fs.CopyTo(ms)
                Return ms.ToArray()
            End Using
        End Using
    End Function

    Public Function ReadTextFromFileUtf8NoBom(path As String) As String
        Dim bytes() As Byte = ReadBytesFromFile(path)
        Return System.Text.Encoding.UTF8.GetString(bytes)
    End Function
    Public Function ReadBytesFromBitmap(bmp As Bitmap) As Byte()
        If bmp Is Nothing Then Throw New ArgumentNullException(NameOf(bmp))

        Using ms As New MemoryStream()
            ' Standardmäßig in den ms als PNG speichern (verlustfrei, unterstützt Transparenz)
            bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png)
            Return ms.ToArray()
        End Using
    End Function

End Module

