Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On
Imports System.Drawing.Imaging
Imports System.Runtime.InteropServices

Namespace Spielfeld

    Public Module BitmapFingerprintModul
        '
        ''' <summary>
        ''' Kompakte Signatur einer Bitmap zur groben Änderungsprüfung.
        ''' </summary>
        Public Structure BitmapFingerprint
            Implements IEquatable(Of BitmapFingerprint)

            Public Width As Integer
            Public Height As Integer
            Public PixelFormatValue As Integer
            Public SampleHash As Long

            Public Overloads Function Equals(ByVal other As BitmapFingerprint) As Boolean _
                        Implements IEquatable(Of BitmapFingerprint).Equals

                Return Width = other.Width AndAlso
               Height = other.Height AndAlso
               PixelFormatValue = other.PixelFormatValue AndAlso
               SampleHash = other.SampleHash

            End Function

            Public Overrides Function Equals(ByVal obj As Object) As Boolean

                If TypeOf obj IsNot BitmapFingerprint Then
                    Return False
                End If

                Return Equals(DirectCast(obj, BitmapFingerprint))

            End Function

            Public Overrides Function GetHashCode() As Integer

                Dim h As Integer = Width
                h = h Xor (Height << 3)
                h = h Xor (PixelFormatValue << 7)
                h = h Xor SampleHash.GetHashCode()

                Return h

            End Function

            Public Shared Operator =(ByVal a As BitmapFingerprint,
                             ByVal b As BitmapFingerprint) As Boolean
                Return a.Equals(b)
            End Operator

            Public Shared Operator <>(ByVal a As BitmapFingerprint,
                              ByVal b As BitmapFingerprint) As Boolean
                Return Not a.Equals(b)
            End Operator

        End Structure

        Public Function CreateBitmapFingerprint(ByVal bmpSrc As Bitmap) As BitmapFingerprint

            Dim fp As BitmapFingerprint

            If bmpSrc Is Nothing Then
                fp.Width = 0
                fp.Height = 0
                fp.PixelFormatValue = 0
                fp.SampleHash = 0
                Return fp
            End If

            fp.Width = bmpSrc.Width
            fp.Height = bmpSrc.Height
            fp.PixelFormatValue = CInt(bmpSrc.PixelFormat)

            Dim bmp As Bitmap = bmpSrc

            If bmp.PixelFormat <> PixelFormat.Format32bppArgb Then
                bmp = New Bitmap(bmpSrc.Width, bmpSrc.Height, PixelFormat.Format32bppArgb)
                Using g As Graphics = Graphics.FromImage(bmp)
                    g.DrawImage(bmpSrc, 0, 0, bmpSrc.Width, bmpSrc.Height)
                End Using
            End If

            Dim rect As New Rectangle(0, 0, bmp.Width, bmp.Height)
            Dim data As BitmapData = bmp.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb)

            Try
                Dim hash64 As Long = 17L
                Dim stride As Integer = data.Stride
                Dim basePtr As IntPtr = data.Scan0

                ' 5x5 Stützpunkte gleichmäßig über die Bitmap verteilt
                Const gridX As Integer = 5
                Const gridY As Integer = 5

                Dim maxX As Integer = Math.Max(0, bmp.Width - 1)
                Dim maxY As Integer = Math.Max(0, bmp.Height - 1)

                For gy As Integer = 0 To gridY - 1
                    Dim y As Integer
                    If gridY = 1 Then
                        y = maxY \ 2
                    Else
                        y = CInt((CDbl(gy) / CDbl(gridY - 1)) * maxY)
                    End If

                    For gx As Integer = 0 To gridX - 1
                        Dim x As Integer
                        If gridX = 1 Then
                            x = maxX \ 2
                        Else
                            x = CInt((CDbl(gx) / CDbl(gridX - 1)) * maxX)
                        End If

                        Dim p As Integer = y * stride + x * 4

                        Dim bb As Integer = CInt(Marshal.ReadByte(basePtr, p + 0))
                        Dim gg As Integer = CInt(Marshal.ReadByte(basePtr, p + 1))
                        Dim rr As Integer = CInt(Marshal.ReadByte(basePtr, p + 2))
                        Dim aa As Integer = CInt(Marshal.ReadByte(basePtr, p + 3))

                        Dim argb As Long = (CLng(aa) << 24) Or
                                           (CLng(rr) << 16) Or
                                           (CLng(gg) << 8) Or
                                           CLng(bb)

                        Dim argb64 As Long = CLng(argb) And &HFFFFFFFFL ' unsigned 32-bit value
                        hash64 = (hash64 * 31L) Xor argb64
                    Next
                Next

                ' Zusatzprobe aus dem Zentrum, etwas gröber gemittelt
                Dim cx As Integer = bmp.Width \ 2
                Dim cy As Integer = bmp.Height \ 2

                Dim sumR As Integer = 0
                Dim sumG As Integer = 0
                Dim sumB As Integer = 0
                Dim sumA As Integer = 0
                Dim count As Integer = 0

                For oy As Integer = -1 To 1
                    Dim y As Integer = cy + oy
                    If y < 0 OrElse y >= bmp.Height Then
                        Continue For
                    End If

                    For ox As Integer = -1 To 1
                        Dim x As Integer = cx + ox
                        If x < 0 OrElse x >= bmp.Width Then
                            Continue For
                        End If

                        Dim p As Integer = y * stride + x * 4

                        sumB += CInt(Marshal.ReadByte(basePtr, p + 0))
                        sumG += CInt(Marshal.ReadByte(basePtr, p + 1))
                        sumR += CInt(Marshal.ReadByte(basePtr, p + 2))
                        sumA += CInt(Marshal.ReadByte(basePtr, p + 3))
                        count += 1
                    Next
                Next

                If count > 0 Then
                    Dim avgB As Integer = sumB \ count
                    Dim avgG As Integer = sumG \ count
                    Dim avgR As Integer = sumR \ count
                    Dim avgA As Integer = sumA \ count

                    Dim avgArgb As Long = (CLng(avgA) << 24) Or
                                          (CLng(avgR) << 16) Or
                                          (CLng(avgG) << 8) Or
                                          CLng(avgB)

                    hash64 = (hash64 * 31L) Xor (CLng(avgArgb) And &HFFFFFFFFL) ' unsigned 32-bit value
                End If

                fp.SampleHash = CInt(hash64 And &HFFFFFFFFL)
                Return fp

            Finally
                bmp.UnlockBits(data)

                If Not Object.ReferenceEquals(bmp, bmpSrc) Then
                    bmp.Dispose()
                End If
            End Try

        End Function

        Private Function MixFingerprintHash(ByVal hash As Long,
                                            ByVal value As Long) As Long

            Dim h As ULong = CULng(hash)
            Dim v As ULong = CULng(value)

            ' RotateLeft um 7 Bit und anschließend XOR
            h = ((h << 7) Or (h >> 57)) Xor v

            ' Positiv halten, damit nichts mit Vorzeichen irritiert
            Return CLng(h And &H7FFFFFFFFFFFFFFFUL)

        End Function

    End Module

End Namespace