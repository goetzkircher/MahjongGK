Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

Imports System.Drawing.Drawing2D
Imports System.Drawing.Imaging
Imports System.IO

Public Enum BackgroundImageRenderMode
    None
    FitInside
    Stretch
    CoverCrop
    PreserveOrgSize
End Enum

Public NotInheritable Class BackgroundSingleImageCache
    Implements IDisposable

    Sub New()
    End Sub

    ''' <summary>
    ''' Falls fullpath Nothing ist, wird nur ein parameterloses Sub New ausgeführt. 
    ''' </summary>
    ''' <param name="fullpath"></param>
    ''' <param name="mode"></param>
    Sub New(fullpath As String, mode As BackgroundImageRenderMode)
        If Not String.IsNullOrEmpty(fullpath) Then
            Load(fullpath, mode)
        End If
    End Sub

#Region "Felder/Status"

    Private _disposed As Boolean

    Private _fullPath As String = Nothing
    Private _original As Bitmap = Nothing
    Private _orgSize As Size = Size.Empty
    Private _mode As BackgroundImageRenderMode = BackgroundImageRenderMode.FitInside

    ' Hot-Cache
    Private _lastBitmap As Bitmap = Nothing
    Private _lastSize As Size = Size.Empty
    Private _lastBg As Color = Color.Empty
    Private _ownsLast As Boolean = False
    Private _isError As Boolean = False
    Private _lastError As String = String.Empty

#End Region

#Region "Öffentliche API"

    Public ReadOnly Property IsError As Boolean
        Get
            Return _isError
        End Get
    End Property

    Public ReadOnly Property LastErrorMessage As String
        Get
            Return _lastError
        End Get
    End Property

    Public ReadOnly Property CurrentPath As String
        Get
            Return _fullPath
        End Get
    End Property

    Public ReadOnly Property OriginalSize As Size
        Get
            Return _orgSize
        End Get
    End Property

    Public ReadOnly Property HasBitmap As Boolean
        Get
            Return _original IsNot Nothing AndAlso _orgSize.Width > 0 AndAlso _orgSize.Height > 0
        End Get
    End Property

    ' ── Load: wirft KEINE Exceptions mehr (außer ObjectDisposedException) ───────────
    Public Sub Load(fullpath As String, mode As BackgroundImageRenderMode)

        Dim normalized As String = Nothing
        Dim loaded As Bitmap = Nothing
        Dim errMsg As String = String.Empty

        ' 1) Pfad vorprüfen – KEIN Throw
        If String.IsNullOrWhiteSpace(fullpath) Then
            errMsg = "Pfad ist leer."
        Else
            ' 2) Normalisieren (wir fangen alles ab)
            Try
                normalized = NormalizePath(fullpath)
            Catch ex As Exception
                normalized = fullpath
                errMsg = $"Pfad ungültig: {ex.Message}"
            End Try

            ' 3) Laden über System.Drawing.Image.FromFile – KEIN Throw nach außen
            If String.IsNullOrEmpty(errMsg) Then
                loaded = LoadOriginalBitmapFromFile(normalized)
                If loaded Is Nothing Then
                    errMsg = "Unbekannter Fehler beim Laden."
                End If
            End If
        End If

        ' 4) Fallback: Fehlerbitmap erstellen, niemals Nothing behalten
        If Not String.IsNullOrEmpty(errMsg) Then
            Try
                loaded = CreateErrorBitmap(If(normalized, fullpath), errMsg)
            Catch ex2 As Exception
                ' Als ultimatives Fallback ein winziges 1x1 rotes Pixel
                loaded = New Bitmap(1, 1, Imaging.PixelFormat.Format32bppArgb)
                Using g As Graphics = Graphics.FromImage(loaded)
                    g.Clear(Color.DarkRed)
                End Using
            End Try
        End If

        ' 5) Zustand übernehmen
        EnsureNotDisposed()

        ClearHotCache_NoLock()
        If _original IsNot Nothing Then _original.Dispose()

        _fullPath = If(normalized, fullpath)
        _original = loaded
        _orgSize = If(_original IsNot Nothing, _original.Size, Size.Empty)
        _mode = If(mode = BackgroundImageRenderMode.None, BackgroundImageRenderMode.Stretch, mode)

        _isError = Not String.IsNullOrEmpty(errMsg)
        _lastError = If(_isError, errMsg, String.Empty)
    End Sub

    ' ── LoadIfPathChanged: ebenfalls ohne Throws (außer Disposed) ───────────────────
    Public Sub LoadIfPathChanged(fullpath As String, mode As BackgroundImageRenderMode)
        Dim normalized As String = fullpath
        Try
            normalized = NormalizePath(fullpath)
        Catch
            ' ignorieren – wir vergleichen notfalls den Rohwert
        End Try

        EnsureNotDisposed()
        Dim needLoad As Boolean = True
        If Not String.IsNullOrEmpty(_fullPath) AndAlso
           String.Equals(_fullPath, normalized, StringComparison.OrdinalIgnoreCase) AndAlso
           _mode = If(mode = BackgroundImageRenderMode.None, BackgroundImageRenderMode.Stretch, mode) Then
            needLoad = False
        End If

        If needLoad Then
            ' delegiert auf die robuste Load-Variante
            Load(fullpath, mode)
        End If
    End Sub

    ''' <summary>
    ''' Gibt Bitmap in exakt <paramref name="targetSize"/> zurück (gemäß Modus, der beim Load gesetzt wurde).
    ''' </summary>
    Public Function GetBitmap(targetSize As Size, Optional backgroundColor As Color? = Nothing) As Bitmap
        EnsureNotDisposed()
        If _original Is Nothing OrElse targetSize.Width <= 0 OrElse targetSize.Height <= 0 Then
            Return Nothing
        End If

        Dim bg As Color = If(backgroundColor.HasValue,
                             backgroundColor.Value,
                             If(_lastBg <> Color.Empty, _lastBg, ComputeDominantColor(_original)))

        If _lastBitmap IsNot Nothing AndAlso
           _lastSize.Equals(targetSize) AndAlso
           ((_mode <> BackgroundImageRenderMode.FitInside) OrElse _lastBg.ToArgb() = bg.ToArgb()) Then
            Return _lastBitmap
        End If

        Rebuild_NoLock(targetSize, _mode, bg)
        Return _lastBitmap
    End Function

    ' ── Invalidate: Fehlerstatus zurücksetzen ───────────────────────────────────────
    Public Sub Invalidate()
        ClearHotCache_NoLock()
        If _original IsNot Nothing Then _original.Dispose() : _original = Nothing
        _fullPath = Nothing
        _orgSize = Size.Empty
        _isError = False
        _lastError = String.Empty
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        If _disposed Then Return
        ClearHotCache_NoLock()
        If _original IsNot Nothing Then _original.Dispose() : _original = Nothing
        _disposed = True
        GC.SuppressFinalize(Me)
    End Sub

#End Region

#Region "Renderkern"

    Private Sub Rebuild_NoLock(target As Size, mode As BackgroundImageRenderMode, bg As Color)
        If _ownsLast AndAlso _lastBitmap IsNot Nothing Then
            _lastBitmap.Dispose()
        End If
        _lastBitmap = Nothing
        _ownsLast = False

        Select Case mode
            Case BackgroundImageRenderMode.Stretch
                _lastBitmap = ScaleHighQuality(_original, target)
                _ownsLast = True

            Case BackgroundImageRenderMode.FitInside
                Dim fit As Size = FitInside(_orgSize, target)
                Using scaled As Bitmap = ScaleHighQuality(_original, fit)
                    Dim result As New Bitmap(target.Width, target.Height, PixelFormat.Format32bppArgb)
                    Using g As Graphics = Graphics.FromImage(result)
                        g.CompositingQuality = CompositingQuality.HighQuality
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic
                        g.PixelOffsetMode = PixelOffsetMode.HighQuality
                        g.SmoothingMode = SmoothingMode.HighQuality
                        Using sb As New SolidBrush(bg)
                            g.FillRectangle(sb, 0, 0, result.Width, result.Height)
                        End Using
                        Dim dx As Integer = (target.Width - fit.Width) \ 2
                        Dim dy As Integer = (target.Height - fit.Height) \ 2
                        g.DrawImage(scaled, New Rectangle(dx, dy, fit.Width, fit.Height))
                    End Using
                    _lastBitmap = result
                End Using
                _ownsLast = True
                _lastBg = bg

            Case BackgroundImageRenderMode.CoverCrop
                Dim cover As Size = CoverSize(_orgSize, target)
                Using coverBmp As Bitmap = ScaleHighQuality(_original, cover)
                    Dim cx As Integer = Math.Max(0, (coverBmp.Width - target.Width) \ 2)
                    Dim cy As Integer = Math.Max(0, (coverBmp.Height - target.Height) \ 2)
                    Dim result As New Bitmap(target.Width, target.Height, PixelFormat.Format32bppArgb)
                    Using g As Graphics = Graphics.FromImage(result)
                        g.CompositingQuality = CompositingQuality.HighQuality
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic
                        g.PixelOffsetMode = PixelOffsetMode.HighQuality
                        g.SmoothingMode = SmoothingMode.HighQuality
                        g.DrawImage(coverBmp,
                                    New Rectangle(0, 0, target.Width, target.Height),
                                    New Rectangle(cx, cy, target.Width, target.Height),
                                    GraphicsUnit.Pixel)
                    End Using
                    _lastBitmap = result
                End Using
                _ownsLast = True

            Case BackgroundImageRenderMode.PreserveOrgSize
                Dim fit As Size = FitInsideNoUpscale(_orgSize, target)
                Using content As Bitmap = If(fit.Equals(_orgSize),
                                             New Bitmap(_original),
                                             ScaleHighQuality(_original, fit))
                    Dim result As New Bitmap(target.Width, target.Height, PixelFormat.Format32bppArgb)
                    Using g As Graphics = Graphics.FromImage(result)
                        g.CompositingQuality = CompositingQuality.HighQuality
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic
                        g.PixelOffsetMode = PixelOffsetMode.HighQuality
                        g.SmoothingMode = SmoothingMode.HighQuality
                        g.Clear(Color.Transparent)
                        Dim dx As Integer = (target.Width - fit.Width) \ 2
                        Dim dy As Integer = (target.Height - fit.Height) \ 2
                        g.DrawImage(content, New Rectangle(dx, dy, fit.Width, fit.Height))
                    End Using
                    _lastBitmap = result
                End Using
                _ownsLast = True

            Case Else
                _lastBitmap = ScaleHighQuality(_original, target)
                _ownsLast = True
        End Select

        _lastSize = target
    End Sub

#End Region

#Region "Hilfsfunktionen (Loader, Skalierung, Farben)"

    Private Shared Function NormalizePath(p As String) As String
        Try
            Return Path.GetFullPath(p)
        Catch
            Return p
        End Try
    End Function

    ' Neu: Laden über System.Drawing.Image.FromFile für jpg/png/bmp/gif/tiff
    Private Shared Function LoadOriginalBitmapFromFile(path As String) As Bitmap
        Try
            ' Image.FromFile hält die Datei gelockt, daher sofort in neuen Bitmap klonen.
            Using img As Image = Image.FromFile(path)
                ' Ergebnis in 32bpp ARGB klonen, um einheitliches Format zu haben.
                Dim bmp As New Bitmap(img.Width, img.Height, PixelFormat.Format32bppArgb)
                Using g As Graphics = Graphics.FromImage(bmp)
                    g.CompositingMode = CompositingMode.SourceCopy
                    g.CompositingQuality = CompositingQuality.HighQuality
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality
                    g.SmoothingMode = SmoothingMode.HighQuality
                    g.DrawImage(img, New Rectangle(0, 0, bmp.Width, bmp.Height))
                End Using
                bmp.SetResolution(img.HorizontalResolution, img.VerticalResolution)
                Return bmp
            End Using
        Catch ex As Exception
            Return CreateErrorBitmap(path, ex.Message)
        End Try
    End Function

    Private Shared Function CreateErrorBitmap(fullpath As String, Optional message As String = Nothing) As Bitmap
        Dim w As Integer = 500, h As Integer = 200
        Dim bmp As New Bitmap(w, h, PixelFormat.Format32bppArgb)
        Using g As Graphics = Graphics.FromImage(bmp)
            g.Clear(Color.DarkRed)
            Using f As New Font("Segoe UI", 9.0F, FontStyle.Bold, GraphicsUnit.Point)
                Dim txt As String = Path.GetFileName(fullpath)
                If Not String.IsNullOrEmpty(message) Then txt &= vbCrLf & message
                g.DrawString(txt, f, Brushes.White, New RectangleF(4, 4, w - 8, h - 8))
            End Using
        End Using
        Return bmp
    End Function

    ' Hochwertiges Resampling in 32bpp ARGB, DPI übernommen
    Private Shared Function ScaleHighQuality(src As Bitmap, target As Size) As Bitmap
        Dim tw As Integer = Math.Max(1, target.Width)
        Dim th As Integer = Math.Max(1, target.Height)

        Dim bmp As New Bitmap(tw, th, PixelFormat.Format32bppArgb)
        bmp.SetResolution(src.HorizontalResolution, src.VerticalResolution)

        Using g As Graphics = Graphics.FromImage(bmp)
            g.CompositingMode = CompositingMode.SourceOver
            g.CompositingQuality = CompositingQuality.HighQuality
            g.InterpolationMode = InterpolationMode.HighQualityBicubic
            g.PixelOffsetMode = PixelOffsetMode.HighQuality
            g.SmoothingMode = SmoothingMode.HighQuality
            g.DrawImage(src, New Rectangle(0, 0, tw, th))
        End Using

        Return bmp
    End Function

    ' FitInside, aber niemals vergrößern (Scale <= 1.0)
    Private Shared Function FitInsideNoUpscale(original As Size, target As Size) As Size
        Dim ow As Double = Math.Max(1, original.Width)
        Dim oh As Double = Math.Max(1, original.Height)
        Dim sx As Double = target.Width / ow
        Dim sy As Double = target.Height / oh
        Dim s As Double = Math.Min(sx, sy)
        s = Math.Min(1.0R, s) ' nie hochskalieren

        Dim w As Integer = Math.Max(1, CInt(Math.Floor(ow * s)))
        Dim h As Integer = Math.Max(1, CInt(Math.Floor(oh * s)))
        Return New Size(w, h)
    End Function

    ' Cover-Größe: skaliert so, dass Ziel komplett bedeckt wird (zentrierter Crop möglich)
    Private Shared Function CoverSize(original As Size, target As Size) As Size
        Dim ow As Double = Math.Max(1, original.Width)
        Dim oh As Double = Math.Max(1, original.Height)
        Dim sx As Double = target.Width / ow
        Dim sy As Double = target.Height / oh
        Dim s As Double = Math.Max(sx, sy)

        Dim w As Integer = Math.Max(1, CInt(Math.Ceiling(ow * s)))
        Dim h As Integer = Math.Max(1, CInt(Math.Ceiling(oh * s)))
        Return New Size(w, h)
    End Function

    ' Dominante Farbe (unverändert)
    Private Shared Function ComputeDominantColor(src As Bitmap) As Color
        If src Is Nothing Then Return Color.Black

        ' Für LockBits ggf. in 32bpp ARGB konvertieren
        Dim bmp As Bitmap = src
        If bmp.PixelFormat <> PixelFormat.Format32bppArgb Then
            bmp = New Bitmap(src.Width, src.Height, PixelFormat.Format32bppArgb)
            Using g As Graphics = Graphics.FromImage(bmp)
                g.DrawImage(src, 0, 0, src.Width, src.Height)
            End Using
        End If

        Const stepXY As Integer = 4
        Const Q As Integer = 16 ' 4 Bit pro Kanal

        Dim counts(Q - 1, Q - 1, Q - 1) As Double
        Dim sumR(Q - 1, Q - 1, Q - 1) As Double
        Dim sumG(Q - 1, Q - 1, Q - 1) As Double
        Dim sumB(Q - 1, Q - 1, Q - 1) As Double

        Dim rect As New Rectangle(0, 0, bmp.Width, bmp.Height)
        Dim data As BitmapData = bmp.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb)

        Try
            Dim stride As Integer = data.Stride
            Dim basePtr As IntPtr = data.Scan0

            For y As Integer = 0 To bmp.Height - 1 Step stepXY
                Dim row As Integer = y * stride
                For x As Integer = 0 To bmp.Width - 1 Step stepXY
                    Dim p As Integer = row + x * 4
                    Dim bb As Byte = System.Runtime.InteropServices.Marshal.ReadByte(basePtr, p + 0)
                    Dim gg As Byte = System.Runtime.InteropServices.Marshal.ReadByte(basePtr, p + 1)
                    Dim rr As Byte = System.Runtime.InteropServices.Marshal.ReadByte(basePtr, p + 2)
                    Dim aa As Byte = System.Runtime.InteropServices.Marshal.ReadByte(basePtr, p + 3)
                    If aa < 16 Then Continue For

                    ' H/S/V zur Gewichtung
                    Dim h As Double, s As Double, v As Double
                    RgbToHsv(CInt(rr), CInt(gg), CInt(bb), h, s, v)

                    ' leichte Bevorzugung von gesättigt/hell
                    Dim sat As Double = 0.3 + 0.7 * s
                    Dim lum As Double = 0.6 + 0.4 * v
                    Dim w As Double = (aa / 255.0) * sat * lum
                    If w <= 0 Then Continue For

                    Dim ri As Integer = rr >> 4
                    Dim gi As Integer = gg >> 4
                    Dim bi As Integer = bb >> 4
                    counts(ri, gi, bi) += w
                    sumR(ri, gi, bi) += rr * w
                    sumG(ri, gi, bi) += gg * w
                    sumB(ri, gi, bi) += bb * w
                Next
            Next

            ' Dominantes Bin suchen
            Dim br As Integer = 0, bg As Integer = 0, bbMax As Integer = 0
            Dim best As Double = -1.0R
            For ri As Integer = 0 To Q - 1
                For gi As Integer = 0 To Q - 1
                    For bi As Integer = 0 To Q - 1
                        If counts(ri, gi, bi) > best Then
                            best = counts(ri, gi, bi)
                            br = ri : bg = gi : bbMax = bi
                        End If
                    Next
                Next
            Next
            If best <= 0 Then Return Color.Black

            Dim mr As Integer = CInt(Math.Round(sumR(br, bg, bbMax) / best))
            Dim mg As Integer = CInt(Math.Round(sumG(br, bg, bbMax) / best))
            Dim mb As Integer = CInt(Math.Round(sumB(br, bg, bbMax) / best))

            Return Color.FromArgb(255,
                              Math.Max(0, Math.Min(255, mr)),
                              Math.Max(0, Math.Min(255, mg)),
                              Math.Max(0, Math.Min(255, mb)))
        Finally
            bmp.UnlockBits(data)
            If Not Object.ReferenceEquals(bmp, src) Then bmp.Dispose()
        End Try
    End Function

    ' RGB → HSV; h in [0..360], s/v in [0..1]
    Private Shared Sub RgbToHsv(r As Integer, g As Integer, b As Integer,
                            ByRef h As Double, ByRef s As Double, ByRef v As Double)

        Dim rf As Double = Math.Max(0, Math.Min(255, r)) / 255.0
        Dim gf As Double = Math.Max(0, Math.Min(255, g)) / 255.0
        Dim bf As Double = Math.Max(0, Math.Min(255, b)) / 255.0

        Dim maxV As Double = Math.Max(rf, Math.Max(gf, bf))
        Dim minV As Double = Math.Min(rf, Math.Min(gf, bf))
        v = maxV

        Dim d As Double = maxV - minV
        s = If(maxV <= 0.0, 0.0, d / maxV)

        If d = 0.0 Then
            h = 0.0
            Return
        End If

        If maxV = rf Then
            h = 60.0 * (((gf - bf) / d) Mod 6.0)
        ElseIf maxV = gf Then
            h = 60.0 * (((bf - rf) / d) + 2.0)
        Else
            h = 60.0 * (((rf - gf) / d) + 4.0)
        End If

        If h < 0.0 Then h += 360.0
    End Sub

    ' Proportionen erhalten: größtmögliche Innenpassung in die Zielgröße
    Private Shared Function FitInside(original As Size, target As Size) As Size
        Dim ow As Double = Math.Max(1, original.Width)
        Dim oh As Double = Math.Max(1, original.Height)
        Dim sx As Double = target.Width / ow
        Dim sy As Double = target.Height / oh
        Dim s As Double = Math.Min(sx, sy)

        Dim w As Integer = Math.Max(1, CInt(Math.Round(ow * s)))
        Dim h As Integer = Math.Max(1, CInt(Math.Round(oh * s)))
        Return New Size(w, h)
    End Function

#End Region

#Region "Aufräumen"

    Private Sub ClearHotCache_NoLock()
        If _ownsLast AndAlso _lastBitmap IsNot Nothing Then
            _lastBitmap.Dispose()
        End If
        _lastBitmap = Nothing
        _lastSize = Size.Empty
        _lastBg = Color.Empty
        _ownsLast = False
    End Sub

    Private Sub EnsureNotDisposed()
        If _disposed Then Throw New ObjectDisposedException(Me.GetType().Name)
    End Sub

#End Region

End Class
