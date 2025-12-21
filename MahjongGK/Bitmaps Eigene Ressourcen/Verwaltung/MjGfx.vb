' WICHTIG: Hier keine Änderungen vornehmen, sie werden gelöscht. Änderungen nur in frmGfxCompiler.WriteMjGfxModule vornehmen!
Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On
Imports System.Drawing.Drawing2D
Imports System.Drawing.Imaging
Imports System.IO
Imports System.IO.Compression
Imports System.Xml.Serialization
Imports Svg

''' <summary>
''' Auto-generiertes Zugriffmodul auf GfxPack-Inhalte.
''' SVG wird bevorzugt. PNG-Auswahl erfolgt anhand der INTRINSISCHEN PNG-Pixelgröße (IHDR), nicht anhand q##.
''' ImageList_* Ordner erzeugen jeweils eine Funktion, die eine ImageList gefüllt zurückgibt.
''' Generiert am 2025-09-17 14:24:03.
''' </summary>
Public Module MjGfx

    ' ── Registrierte GfxPack-Dateien (relativ zum App-Basisverzeichnis) ───────────────
    Private ReadOnly s_packFiles As String() = {
        "Basis_Satz1.gfx.xml",
        "Basis_Satz2.gfx.xml"
    }

    Private s_inited As Boolean = False
    Private ReadOnly s_dict As New Dictionary(Of String, List(Of GfxEntry))(StringComparer.OrdinalIgnoreCase)
    ' Cache für PNG-Größen, Key = entry.DataGzBase64 (String-Referenz)
    Private ReadOnly s_pngSizeCache As New Dictionary(Of String, Size)(StringComparer.Ordinal)
    ' Für ImageList-Ordner: FolderTop → (BaseName → Kandidatenliste)
    Private ReadOnly s_folderMap As New Dictionary(Of String, Dictionary(Of String, List(Of GfxEntry)))(StringComparer.OrdinalIgnoreCase)

    ''' <summary>Einmaliges Laden der registrierten GfxPack-XMLs und Aufbau von s_dict / s_folderMap.</summary>
    Private Sub EnsureLoaded()
        If s_inited Then Return
        SyncLock s_dict
            If s_inited Then Return
            For Each rel As String In s_packFiles
                Dim fullPath As String = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Eigene Ressourcen", rel)
                If Not File.Exists(fullPath) Then Continue For
                Try
                    Dim ser As New XmlSerializer(GetType(GfxPack))
                    Using fs As FileStream = File.OpenRead(fullPath)
                        Dim p As GfxPack = DirectCast(ser.Deserialize(fs), GfxPack)
                        If p IsNot Nothing AndAlso p.Entries IsNot Nothing Then
                            For Each e As GfxEntry In p.Entries
                                If e Is Nothing OrElse String.IsNullOrWhiteSpace(e.Key) Then Continue For
                                ' 1) s_dict: Key → Varianten
                                Dim lst As List(Of GfxEntry) = Nothing
                                If Not s_dict.TryGetValue(e.Key, lst) Then
                                    lst = New List(Of GfxEntry)()
                                    s_dict(e.Key) = lst
                                End If
                                lst.Add(e)
                                ' 2) s_folderMap: nur Top-Ordner ImageList_* gruppieren
                                Dim top As String = ""
                                If Not String.IsNullOrEmpty(e.Folder) Then
                                    Dim parts() As String = e.Folder.Split(New Char() {Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar}, StringSplitOptions.RemoveEmptyEntries)
                                    If parts.Length > 0 Then top = parts(0)
                                End If
                                If Not String.IsNullOrEmpty(top) AndAlso top.StartsWith("ImageList_", StringComparison.OrdinalIgnoreCase) Then
                                    Dim groups As Dictionary(Of String, List(Of GfxEntry)) = Nothing
                                    If Not s_folderMap.TryGetValue(top, groups) Then
                                        groups = New Dictionary(Of String, List(Of GfxEntry))(StringComparer.OrdinalIgnoreCase)
                                        s_folderMap(top) = groups
                                    End If
                                    Dim bn As String = If(String.IsNullOrEmpty(e.BaseName), e.Key, e.BaseName)
                                    Dim gl As List(Of GfxEntry) = Nothing
                                    If Not groups.TryGetValue(bn, gl) Then
                                        gl = New List(Of GfxEntry)()
                                        groups(bn) = gl
                                    End If
                                    gl.Add(e)
                                End If
                            Next
                        End If
                    End Using
                Catch
                    ' korruptes Pack still überspringen
                End Try
            Next
            s_inited = True
        End SyncLock
    End Sub

    ''' <summary>Liefert Bitmap für Key in gewünschter Größe (SVG bevorzugt, sonst beste PNG nach IHDR-Maßen).</summary>
    Public Function MjGfx_GfxMain(key As String, width As Integer, Optional height As Integer = 0) As Bitmap
        If width <= 0 Then Throw New ArgumentOutOfRangeException(NameOf(width))
        If height <= 0 Then height = width
        EnsureLoaded()
        Dim list As List(Of GfxEntry) = Nothing
        If Not s_dict.TryGetValue(key, list) OrElse list Is Nothing OrElse list.Count = 0 Then Return Nothing
        Dim svg As GfxEntry = list.FirstOrDefault(Function(e) String.Equals(e.Kind, "svg", StringComparison.OrdinalIgnoreCase))
        If svg IsNot Nothing Then Return RenderSvgEntry(svg, width, height)
        Dim pngBest As GfxEntry = PickBestPngByIntrinsicSize(list, width, height)
        If pngBest Is Nothing Then Return Nothing
        Return RenderPngEntry(pngBest, width, height)
    End Function

    ''' <summary>Füllt das Label mit der Grafik gleichen Namens in der Größe des Labels</summary>
    Public Sub MjGfx_GfxMain(lbl As Label)
        lbl.Image = MjGfx_GfxMain(lbl.Name, lbl.Width, lbl.Height)
    End Sub

    ''' <summary>Füllt das Button mit der Grafik gleichen Namens in der Größe 16x16</summary>
    Public Sub MjGfx_GfxMain(btn As Button)
        btn.TextImageRelation = TextImageRelation.ImageBeforeText
        btn.Image = MjGfx_GfxMain(btn.Name, 16)
    End Sub

    Private Function PickBestEntryForSize(candidates As IEnumerable(Of GfxEntry), w As Integer, h As Integer) As GfxEntry
        Dim svg As GfxEntry = candidates.FirstOrDefault(Function(e) String.Equals(e.Kind, "svg", StringComparison.OrdinalIgnoreCase))
        If svg IsNot Nothing Then Return svg
        Return PickBestPngByIntrinsicSize(candidates, w, h)
    End Function

    Private Function PickBestPngByIntrinsicSize(candidates As IEnumerable(Of GfxEntry), desiredW As Integer, desiredH As Integer) As GfxEntry
        Dim best As GfxEntry = Nothing
        Dim bestScore As Integer = Integer.MaxValue
        Dim bestIsDownscale As Boolean = False
        For Each e As GfxEntry In candidates
            If Not String.Equals(e.Kind, "png", StringComparison.OrdinalIgnoreCase) Then Continue For
            Dim sz As Size = GetPngSize(e)
            If sz.Width <= 0 OrElse sz.Height <= 0 Then Continue For
            Dim dw As Integer = Math.Abs(sz.Width - desiredW)
            Dim dh As Integer = Math.Abs(sz.Height - desiredH)
            Dim score As Integer = Math.Max(dw, dh)
            Dim isDownscale As Boolean = (sz.Width >= desiredW AndAlso sz.Height >= desiredH)
            If score < bestScore Then
                best = e : bestScore = score : bestIsDownscale = isDownscale
            ElseIf score = bestScore Then
                If isDownscale AndAlso Not bestIsDownscale Then
                    best = e : bestIsDownscale = True
                ElseIf isDownscale = bestIsDownscale Then
                    Dim bsz As Size = If(best Is Nothing, Size.Empty, GetPngSize(best))
                    Dim a As Integer = sz.Width * sz.Height
                    Dim b As Integer = bsz.Width * bsz.Height
                    If a > b Then best = e
                End If
            End If
        Next
        If best Is Nothing Then
            best = candidates.FirstOrDefault(Function(x) String.Equals(x.Kind, "png", StringComparison.OrdinalIgnoreCase))
        End If
        Return best
    End Function

    Private Function GetPngSize(entry As GfxEntry) As Size
        If entry Is Nothing OrElse String.IsNullOrEmpty(entry.DataGzBase64) Then Return Size.Empty
        Dim key As String = entry.DataGzBase64
        Dim sz As Size = Size.Empty
        If s_pngSizeCache.TryGetValue(key, sz) Then Return sz
        Dim bytes As Byte() = InflateGZipFromBase64(entry.DataGzBase64)
        Dim w As Integer = 0, h As Integer = 0
        If TryReadPngIHDR(bytes, w, h) AndAlso w > 0 AndAlso h > 0 Then
            sz = New Size(w, h)
        Else
            Try
                Using ms As New MemoryStream(bytes, writable:=False)
                    Using img As Image = Image.FromStream(ms, useEmbeddedColorManagement:=True, validateImageData:=True)
                        sz = img.Size
                    End Using
                End Using
            Catch
                sz = Size.Empty
            End Try
        End If
        If sz <> Size.Empty Then s_pngSizeCache(key) = sz
        Return sz
    End Function

    Private Function TryReadPngIHDR(data As Byte(), ByRef width As Integer, ByRef height As Integer) As Boolean
        width = 0 : height = 0
        If data Is Nothing OrElse data.Length < 33 Then Return False
        If Not (data(0) = &H89 AndAlso data(1) = &H50 AndAlso data(2) = &H4E AndAlso data(3) = &H47 AndAlso data(4) = &HD AndAlso data(5) = &HA AndAlso data(6) = &H1A AndAlso data(7) = &HA) Then Return False
        Dim t0 As Integer = 8 + 4 ' skip length
        If data.Length < t0 + 4 + 8 Then Return False
        If Not (data(t0) = &H49 AndAlso data(t0 + 1) = &H48 AndAlso data(t0 + 2) = &H44 AndAlso data(t0 + 3) = &H52) Then Return False
        Dim p As Integer = t0 + 4
        width = (CInt(data(p)) << 24) Or (CInt(data(p + 1)) << 16) Or (CInt(data(p + 2)) << 8) Or CInt(data(p + 3))
        height = (CInt(data(p + 4)) << 24) Or (CInt(data(p + 5)) << 16) Or (CInt(data(p + 6)) << 8) Or CInt(data(p + 7))
        Return (width > 0 AndAlso height > 0)
    End Function

    Private Function RenderPngEntry(entry As GfxEntry, w As Integer, h As Integer) As Bitmap
        Dim bytes As Byte() = InflateGZipFromBase64(entry.DataGzBase64)
        Using ms As New MemoryStream(bytes, writable:=False)
            Using src As Image = Image.FromStream(ms, useEmbeddedColorManagement:=True, validateImageData:=True)
                Return ResizeImageCrisp(CType(src, Bitmap), w, h)
            End Using
        End Using
    End Function

    Private Function RenderSvgEntry(entry As GfxEntry, w As Integer, h As Integer) As Bitmap
        Dim bytes As Byte() = InflateGZipFromBase64(entry.DataGzBase64)
        Using ms As New MemoryStream(bytes, writable:=False)
            Dim doc As SvgDocument = SvgDocument.Open(Of SvgDocument)(ms)
            ' SVG rendert bereits in Zielgröße – wir schicken es trotzdem durch die zentrale Pipeline,
            ' damit Format (32bppArgb) & ggf. Aufhellung im DarkMode einheitlich angewendet werden.
            Dim tmp As Bitmap = doc.Draw(w, h)
            Try
                Return ResizeImageCrisp(tmp, w, h)
            Finally
                tmp.Dispose()
            End Try
        End Using
    End Function

    Private Function ResizeImageCrisp(src As Bitmap, w As Integer, h As Integer) As Bitmap
        If src Is Nothing Then Throw New ArgumentNullException(NameOf(src))
        If w <= 0 OrElse h <= 0 Then Throw New ArgumentOutOfRangeException("w/h")

        Dim dst As New Bitmap(w, h, Drawing.Imaging.PixelFormat.Format32bppArgb)
        ' DPI vom Quellbild übernehmen (falls sinnvoll)
        Try
            dst.SetResolution(src.HorizontalResolution, src.VerticalResolution)
        Catch
            ' exotic DPI -> Standard belassen
        End Try

        Using g As Graphics = Graphics.FromImage(dst)
            g.CompositingMode = CompositingMode.SourceOver
            g.CompositingQuality = CompositingQuality.HighQuality
            g.InterpolationMode = InterpolationMode.HighQualityBicubic
            g.PixelOffsetMode = PixelOffsetMode.HighQuality
            g.SmoothingMode = SmoothingMode.HighQuality

            Dim destRect As New Rectangle(0, 0, w, h)
            Dim srcRect As New Rectangle(0, 0, src.Width, src.Height)

            ' Optional: Aufhellung im DarkMode via ColorMatrix (Skalierung der RGB-Kanäle)
            Dim useBrighten As Boolean = False
            Dim cm As ColorMatrix = Nothing
            Dim ia As ImageAttributes = Nothing
            Try
                If INI.Global_DarkMode Then
                    Dim brighten As Single = Math.Max(0.0F, INI.Global_BrightenAmount)
                    If brighten > 0.0001F Then
                        Dim factor As Single = 1.0F + brighten
                        factor = Math.Min(factor, 3.0F) ' harte Kappe gegen Überstrahlung
                        cm = New ColorMatrix(New Single()() {
                            New Single() {factor, 0.0F, 0.0F, 0.0F, 0.0F},
                            New Single() {0.0F, factor, 0.0F, 0.0F, 0.0F},
                            New Single() {0.0F, 0.0F, factor, 0.0F, 0.0F},
                            New Single() {0.0F, 0.0F, 0.0F, 1.0F, 0.0F},
                            New Single() {0.0F, 0.0F, 0.0F, 0.0F, 1.0F}
                        })
                        ia = New ImageAttributes()
                        ia.SetColorMatrix(cm, ColorMatrixFlag.Default, ColorAdjustType.Bitmap)
                        useBrighten = True
                    End If
                End If
            Catch
                ' Falls INI nicht verfügbar oder andere Fehler → ohne Aufhellung zeichnen
                useBrighten = False
            End Try

            If useBrighten AndAlso ia IsNot Nothing Then
                g.DrawImage(src, destRect, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, GraphicsUnit.Pixel, ia)
            Else
                g.DrawImage(src, destRect, srcRect, GraphicsUnit.Pixel)
            End If
        End Using

        Return dst
    End Function

    Private Function InflateGZipFromBase64(b64 As String) As Byte()
        Dim gz As Byte() = Convert.FromBase64String(b64)
        Using cms As New MemoryStream(gz, writable:=False)
            Using gs As New GZipStream(cms, CompressionMode.Decompress, leaveOpen:=False)
                Using outMs As New MemoryStream()
                    gs.CopyTo(outMs)
                    Return outMs.ToArray()
                End Using
            End Using
        End Using
    End Function

    ''' <summary>Erzeugt eine ImageList aus dem angegebenen ImageList_-Top-Ordner in gewünschter Größe.</summary>
    Private Function BuildImageListForFolder(folderTop As String, width As Integer, height As Integer) As ImageList
        EnsureLoaded()
        If width <= 0 Then Throw New ArgumentOutOfRangeException(NameOf(width))
        If height <= 0 Then height = width
        Dim il As New ImageList With {
            .ColorDepth = ColorDepth.Depth32Bit,
            .ImageSize = New Size(width, height)
        }
        Dim groups As Dictionary(Of String, List(Of GfxEntry)) = Nothing
        If Not s_folderMap.TryGetValue(folderTop, groups) OrElse groups Is Nothing OrElse groups.Count = 0 Then
            Return il
        End If
        For Each bn As String In groups.Keys.OrderBy(Function(x) x, StringComparer.OrdinalIgnoreCase)
            Dim best As GfxEntry = PickBestEntryForSize(groups(bn), width, height)
            If best Is Nothing Then Continue For
            Dim bmp As Bitmap = Nothing
            If String.Equals(best.Kind, "svg", StringComparison.OrdinalIgnoreCase) Then
                bmp = RenderSvgEntry(best, width, height)
            Else
                bmp = RenderPngEntry(best, width, height)
            End If
            If bmp IsNot Nothing Then
                il.Images.Add(bn, bmp)
            End If
        Next
        Return il
    End Function

    ''' <summary>Gibt "architecture" als Bitmap zurück (SVG bevorzugt). Größe: width×height (height=0→quadratisch).</summary>
    Public Function MjGfx_architecture(width As Integer, Optional height As Integer = 0) As Bitmap
        Return MjGfx_GfxMain("architecture", width, height)
    End Function

    ''' <summary>Gibt "baseline_restore_black_24dp" als Bitmap zurück (SVG bevorzugt). Größe: width×height (height=0→quadratisch).</summary>
    Public Function MjGfx_baseline_restore_black_24dp(width As Integer, Optional height As Integer = 0) As Bitmap
        Return MjGfx_GfxMain("baseline_restore_black_24dp", width, height)
    End Function

    ''' <summary>Gibt "Cancel" als Bitmap zurück (SVG bevorzugt). Größe: width×height (height=0→quadratisch).</summary>
    Public Function MjGfx_Cancel(width As Integer, Optional height As Integer = 0) As Bitmap
        Return MjGfx_GfxMain("Cancel", width, height)
    End Function

    ''' <summary>Gibt "Checked" als Bitmap zurück (SVG bevorzugt). Größe: width×height (height=0→quadratisch).</summary>
    Public Function MjGfx_Checked(width As Integer, Optional height As Integer = 0) As Bitmap
        Return MjGfx_GfxMain("Checked", width, height)
    End Function

    ''' <summary>Gibt "CompassN" als Bitmap zurück (SVG bevorzugt). Größe: width×height (height=0→quadratisch).</summary>
    Public Function MjGfx_CompassN(width As Integer, Optional height As Integer = 0) As Bitmap
        Return MjGfx_GfxMain("CompassN", width, height)
    End Function

    ''' <summary>Gibt "CompassNmover" als Bitmap zurück (SVG bevorzugt). Größe: width×height (height=0→quadratisch).</summary>
    Public Function MjGfx_CompassNmover(width As Integer, Optional height As Integer = 0) As Bitmap
        Return MjGfx_GfxMain("CompassNmover", width, height)
    End Function

    ''' <summary>Gibt "CompassNO" als Bitmap zurück (SVG bevorzugt). Größe: width×height (height=0→quadratisch).</summary>
    Public Function MjGfx_CompassNO(width As Integer, Optional height As Integer = 0) As Bitmap
        Return MjGfx_GfxMain("CompassNO", width, height)
    End Function

    ''' <summary>Gibt "CompassNOmover" als Bitmap zurück (SVG bevorzugt). Größe: width×height (height=0→quadratisch).</summary>
    Public Function MjGfx_CompassNOmover(width As Integer, Optional height As Integer = 0) As Bitmap
        Return MjGfx_GfxMain("CompassNOmover", width, height)
    End Function

    ''' <summary>Gibt "CompassNOsel" als Bitmap zurück (SVG bevorzugt). Größe: width×height (height=0→quadratisch).</summary>
    Public Function MjGfx_CompassNOsel(width As Integer, Optional height As Integer = 0) As Bitmap
        Return MjGfx_GfxMain("CompassNOsel", width, height)
    End Function

    ''' <summary>Gibt "CompassNsel" als Bitmap zurück (SVG bevorzugt). Größe: width×height (height=0→quadratisch).</summary>
    Public Function MjGfx_CompassNsel(width As Integer, Optional height As Integer = 0) As Bitmap
        Return MjGfx_GfxMain("CompassNsel", width, height)
    End Function

    ''' <summary>Gibt "CompassNW" als Bitmap zurück (SVG bevorzugt). Größe: width×height (height=0→quadratisch).</summary>
    Public Function MjGfx_CompassNW(width As Integer, Optional height As Integer = 0) As Bitmap
        Return MjGfx_GfxMain("CompassNW", width, height)
    End Function

    ''' <summary>Gibt "CompassNWmover" als Bitmap zurück (SVG bevorzugt). Größe: width×height (height=0→quadratisch).</summary>
    Public Function MjGfx_CompassNWmover(width As Integer, Optional height As Integer = 0) As Bitmap
        Return MjGfx_GfxMain("CompassNWmover", width, height)
    End Function

    ''' <summary>Gibt "CompassNWsel" als Bitmap zurück (SVG bevorzugt). Größe: width×height (height=0→quadratisch).</summary>
    Public Function MjGfx_CompassNWsel(width As Integer, Optional height As Integer = 0) As Bitmap
        Return MjGfx_GfxMain("CompassNWsel", width, height)
    End Function

    ''' <summary>Gibt "CompassO" als Bitmap zurück (SVG bevorzugt). Größe: width×height (height=0→quadratisch).</summary>
    Public Function MjGfx_CompassO(width As Integer, Optional height As Integer = 0) As Bitmap
        Return MjGfx_GfxMain("CompassO", width, height)
    End Function

    ''' <summary>Gibt "CompassOmover" als Bitmap zurück (SVG bevorzugt). Größe: width×height (height=0→quadratisch).</summary>
    Public Function MjGfx_CompassOmover(width As Integer, Optional height As Integer = 0) As Bitmap
        Return MjGfx_GfxMain("CompassOmover", width, height)
    End Function

    ''' <summary>Gibt "CompassOsel" als Bitmap zurück (SVG bevorzugt). Größe: width×height (height=0→quadratisch).</summary>
    Public Function MjGfx_CompassOsel(width As Integer, Optional height As Integer = 0) As Bitmap
        Return MjGfx_GfxMain("CompassOsel", width, height)
    End Function

    ''' <summary>Gibt "CompassS" als Bitmap zurück (SVG bevorzugt). Größe: width×height (height=0→quadratisch).</summary>
    Public Function MjGfx_CompassS(width As Integer, Optional height As Integer = 0) As Bitmap
        Return MjGfx_GfxMain("CompassS", width, height)
    End Function

    ''' <summary>Gibt "CompassSE" als Bitmap zurück (SVG bevorzugt). Größe: width×height (height=0→quadratisch).</summary>
    Public Function MjGfx_CompassSE(width As Integer, Optional height As Integer = 0) As Bitmap
        Return MjGfx_GfxMain("CompassSE", width, height)
    End Function

    ''' <summary>Gibt "CompassSEsel" als Bitmap zurück (SVG bevorzugt). Größe: width×height (height=0→quadratisch).</summary>
    Public Function MjGfx_CompassSEsel(width As Integer, Optional height As Integer = 0) As Bitmap
        Return MjGfx_GfxMain("CompassSEsel", width, height)
    End Function

    ''' <summary>Gibt "CompassSmover" als Bitmap zurück (SVG bevorzugt). Größe: width×height (height=0→quadratisch).</summary>
    Public Function MjGfx_CompassSmover(width As Integer, Optional height As Integer = 0) As Bitmap
        Return MjGfx_GfxMain("CompassSmover", width, height)
    End Function

    ''' <summary>Gibt "CompassSOmover" als Bitmap zurück (SVG bevorzugt). Größe: width×height (height=0→quadratisch).</summary>
    Public Function MjGfx_CompassSOmover(width As Integer, Optional height As Integer = 0) As Bitmap
        Return MjGfx_GfxMain("CompassSOmover", width, height)
    End Function

    ''' <summary>Gibt "CompassSsel" als Bitmap zurück (SVG bevorzugt). Größe: width×height (height=0→quadratisch).</summary>
    Public Function MjGfx_CompassSsel(width As Integer, Optional height As Integer = 0) As Bitmap
        Return MjGfx_GfxMain("CompassSsel", width, height)
    End Function

    ''' <summary>Gibt "CompassSW" als Bitmap zurück (SVG bevorzugt). Größe: width×height (height=0→quadratisch).</summary>
    Public Function MjGfx_CompassSW(width As Integer, Optional height As Integer = 0) As Bitmap
        Return MjGfx_GfxMain("CompassSW", width, height)
    End Function

    ''' <summary>Gibt "CompassSWmover" als Bitmap zurück (SVG bevorzugt). Größe: width×height (height=0→quadratisch).</summary>
    Public Function MjGfx_CompassSWmover(width As Integer, Optional height As Integer = 0) As Bitmap
        Return MjGfx_GfxMain("CompassSWmover", width, height)
    End Function

    ''' <summary>Gibt "CompassSWsel" als Bitmap zurück (SVG bevorzugt). Größe: width×height (height=0→quadratisch).</summary>
    Public Function MjGfx_CompassSWsel(width As Integer, Optional height As Integer = 0) As Bitmap
        Return MjGfx_GfxMain("CompassSWsel", width, height)
    End Function

    ''' <summary>Gibt "CompassW" als Bitmap zurück (SVG bevorzugt). Größe: width×height (height=0→quadratisch).</summary>
    Public Function MjGfx_CompassW(width As Integer, Optional height As Integer = 0) As Bitmap
        Return MjGfx_GfxMain("CompassW", width, height)
    End Function

    ''' <summary>Gibt "CompassWmover" als Bitmap zurück (SVG bevorzugt). Größe: width×height (height=0→quadratisch).</summary>
    Public Function MjGfx_CompassWmover(width As Integer, Optional height As Integer = 0) As Bitmap
        Return MjGfx_GfxMain("CompassWmover", width, height)
    End Function

    ''' <summary>Gibt "CompassWsel" als Bitmap zurück (SVG bevorzugt). Größe: width×height (height=0→quadratisch).</summary>
    Public Function MjGfx_CompassWsel(width As Integer, Optional height As Integer = 0) As Bitmap
        Return MjGfx_GfxMain("CompassWsel", width, height)
    End Function

    ''' <summary>Gibt "contrast-circle-symbol" als Bitmap zurück (SVG bevorzugt). Größe: width×height (height=0→quadratisch).</summary>
    Public Function MjGfx_contrast_circle_symbol(width As Integer, Optional height As Integer = 0) As Bitmap
        Return MjGfx_GfxMain("contrast-circle-symbol", width, height)
    End Function

    ''' <summary>Gibt "contrast_Heller" als Bitmap zurück (SVG bevorzugt). Größe: width×height (height=0→quadratisch).</summary>
    Public Function MjGfx_contrast_Heller(width As Integer, Optional height As Integer = 0) As Bitmap
        Return MjGfx_GfxMain("contrast_Heller", width, height)
    End Function

    ''' <summary>Gibt "Dummy" als Bitmap zurück (SVG bevorzugt). Größe: width×height (height=0→quadratisch).</summary>
    Public Function MjGfx_Dummy(width As Integer, Optional height As Integer = 0) As Bitmap
        Return MjGfx_GfxMain("Dummy", width, height)
    End Function

    ''' <summary>Gibt "EditorFileCmd_Laden_Eigene" als Bitmap zurück (SVG bevorzugt). Größe: width×height (height=0→quadratisch).</summary>
    Public Function MjGfx_EditorFileCmd_Laden_Eigene(width As Integer, Optional height As Integer = 0) As Bitmap
        Return MjGfx_GfxMain("EditorFileCmd_Laden_Eigene", width, height)
    End Function

    ''' <summary>Gibt "EditorFileCmd_Speichern" als Bitmap zurück (SVG bevorzugt). Größe: width×height (height=0→quadratisch).</summary>
    Public Function MjGfx_EditorFileCmd_Speichern(width As Integer, Optional height As Integer = 0) As Bitmap
        Return MjGfx_GfxMain("EditorFileCmd_Speichern", width, height)
    End Function

    ''' <summary>Gibt "ellipse-vector-format" als Bitmap zurück (SVG bevorzugt). Größe: width×height (height=0→quadratisch).</summary>
    Public Function MjGfx_ellipse_vector_format(width As Integer, Optional height As Integer = 0) As Bitmap
        Return MjGfx_GfxMain("ellipse-vector-format", width, height)
    End Function

    ''' <summary>Gibt "ErrorGrafik" als Bitmap zurück (SVG bevorzugt). Größe: width×height (height=0→quadratisch).</summary>
    Public Function MjGfx_ErrorGrafik(width As Integer, Optional height As Integer = 0) As Bitmap
        Return MjGfx_GfxMain("ErrorGrafik", width, height)
    End Function

    ''' <summary>Gibt "FileOpen" als Bitmap zurück (SVG bevorzugt). Größe: width×height (height=0→quadratisch).</summary>
    Public Function MjGfx_FileOpen(width As Integer, Optional height As Integer = 0) As Bitmap
        Return MjGfx_GfxMain("FileOpen", width, height)
    End Function

    ''' <summary>Gibt "FileSave" als Bitmap zurück (SVG bevorzugt). Größe: width×height (height=0→quadratisch).</summary>
    Public Function MjGfx_FileSave(width As Integer, Optional height As Integer = 0) As Bitmap
        Return MjGfx_GfxMain("FileSave", width, height)
    End Function

    ''' <summary>Gibt "file_open" als Bitmap zurück (SVG bevorzugt). Größe: width×height (height=0→quadratisch).</summary>
    Public Function MjGfx_file_open(width As Integer, Optional height As Integer = 0) As Bitmap
        Return MjGfx_GfxMain("file_open", width, height)
    End Function

    ''' <summary>Gibt "FolderOpen" als Bitmap zurück (SVG bevorzugt). Größe: width×height (height=0→quadratisch).</summary>
    Public Function MjGfx_FolderOpen(width As Integer, Optional height As Integer = 0) As Bitmap
        Return MjGfx_GfxMain("FolderOpen", width, height)
    End Function

    ''' <summary>Gibt "handyman" als Bitmap zurück (SVG bevorzugt). Größe: width×height (height=0→quadratisch).</summary>
    Public Function MjGfx_handyman(width As Integer, Optional height As Integer = 0) As Bitmap
        Return MjGfx_GfxMain("handyman", width, height)
    End Function

    ''' <summary>Gibt "hexagon-geometrical-shape-outline" als Bitmap zurück (SVG bevorzugt). Größe: width×height (height=0→quadratisch).</summary>
    Public Function MjGfx_hexagon_geometrical_shape_outline(width As Integer, Optional height As Integer = 0) As Bitmap
        Return MjGfx_GfxMain("hexagon-geometrical-shape-outline", width, height)
    End Function

    ''' <summary>Gibt "HGrdEinfügen" als Bitmap zurück (SVG bevorzugt). Größe: width×height (height=0→quadratisch).</summary>
    Public Function MjGfx_HGrdEinfügen(width As Integer, Optional height As Integer = 0) As Bitmap
        Return MjGfx_GfxMain("HGrdEinfügen", width, height)
    End Function

    ''' <summary>Gibt "HGrdEinfügenGrün" als Bitmap zurück (SVG bevorzugt). Größe: width×height (height=0→quadratisch).</summary>
    Public Function MjGfx_HGrdEinfügenGrün(width As Integer, Optional height As Integer = 0) As Bitmap
        Return MjGfx_GfxMain("HGrdEinfügenGrün", width, height)
    End Function

    ''' <summary>Gibt "Horizontal" als Bitmap zurück (SVG bevorzugt). Größe: width×height (height=0→quadratisch).</summary>
    Public Function MjGfx_Horizontal(width As Integer, Optional height As Integer = 0) As Bitmap
        Return MjGfx_GfxMain("Horizontal", width, height)
    End Function

    ''' <summary>Gibt "Info16qBlau" als Bitmap zurück (SVG bevorzugt). Größe: width×height (height=0→quadratisch).</summary>
    Public Function MjGfx_Info16qBlau(width As Integer, Optional height As Integer = 0) As Bitmap
        Return MjGfx_GfxMain("Info16qBlau", width, height)
    End Function

    ''' <summary>Gibt "mode_standby" als Bitmap zurück (SVG bevorzugt). Größe: width×height (height=0→quadratisch).</summary>
    Public Function MjGfx_mode_standby(width As Integer, Optional height As Integer = 0) As Bitmap
        Return MjGfx_GfxMain("mode_standby", width, height)
    End Function

    ''' <summary>Gibt "MoveLocation" als Bitmap zurück (SVG bevorzugt). Größe: width×height (height=0→quadratisch).</summary>
    Public Function MjGfx_MoveLocation(width As Integer, Optional height As Integer = 0) As Bitmap
        Return MjGfx_GfxMain("MoveLocation", width, height)
    End Function

    ''' <summary>Gibt "MoveLocation32qBlau" als Bitmap zurück (SVG bevorzugt). Größe: width×height (height=0→quadratisch).</summary>
    Public Function MjGfx_MoveLocation32qBlau(width As Integer, Optional height As Integer = 0) As Bitmap
        Return MjGfx_GfxMain("MoveLocation32qBlau", width, height)
    End Function

    ''' <summary>Gibt "MoveLocation32qGrün" als Bitmap zurück (SVG bevorzugt). Größe: width×height (height=0→quadratisch).</summary>
    Public Function MjGfx_MoveLocation32qGrün(width As Integer, Optional height As Integer = 0) As Bitmap
        Return MjGfx_GfxMain("MoveLocation32qGrün", width, height)
    End Function

    ''' <summary>Gibt "octagon-outline-shape" als Bitmap zurück (SVG bevorzugt). Größe: width×height (height=0→quadratisch).</summary>
    Public Function MjGfx_octagon_outline_shape(width As Integer, Optional height As Integer = 0) As Bitmap
        Return MjGfx_GfxMain("octagon-outline-shape", width, height)
    End Function

    ''' <summary>Gibt "Pause734x652" als Bitmap zurück (SVG bevorzugt). Größe: width×height (height=0→quadratisch).</summary>
    Public Function MjGfx_Pause734x652(width As Integer, Optional height As Integer = 0) As Bitmap
        Return MjGfx_GfxMain("Pause734x652", width, height)
    End Function

    ''' <summary>Gibt "pentagon-outline-shape" als Bitmap zurück (SVG bevorzugt). Größe: width×height (height=0→quadratisch).</summary>
    Public Function MjGfx_pentagon_outline_shape(width As Integer, Optional height As Integer = 0) As Bitmap
        Return MjGfx_GfxMain("pentagon-outline-shape", width, height)
    End Function

    ''' <summary>Gibt "PfeilDn" als Bitmap zurück (SVG bevorzugt). Größe: width×height (height=0→quadratisch).</summary>
    Public Function MjGfx_PfeilDn(width As Integer, Optional height As Integer = 0) As Bitmap
        Return MjGfx_GfxMain("PfeilDn", width, height)
    End Function

    ''' <summary>Gibt "PfeilDnmover" als Bitmap zurück (SVG bevorzugt). Größe: width×height (height=0→quadratisch).</summary>
    Public Function MjGfx_PfeilDnmover(width As Integer, Optional height As Integer = 0) As Bitmap
        Return MjGfx_GfxMain("PfeilDnmover", width, height)
    End Function

    ''' <summary>Gibt "PfeilUp" als Bitmap zurück (SVG bevorzugt). Größe: width×height (height=0→quadratisch).</summary>
    Public Function MjGfx_PfeilUp(width As Integer, Optional height As Integer = 0) As Bitmap
        Return MjGfx_GfxMain("PfeilUp", width, height)
    End Function

    ''' <summary>Gibt "PfeilUpmover" als Bitmap zurück (SVG bevorzugt). Größe: width×height (height=0→quadratisch).</summary>
    Public Function MjGfx_PfeilUpmover(width As Integer, Optional height As Integer = 0) As Bitmap
        Return MjGfx_GfxMain("PfeilUpmover", width, height)
    End Function

    ''' <summary>Gibt "Punkt" als Bitmap zurück (SVG bevorzugt). Größe: width×height (height=0→quadratisch).</summary>
    Public Function MjGfx_Punkt(width As Integer, Optional height As Integer = 0) As Bitmap
        Return MjGfx_GfxMain("Punkt", width, height)
    End Function

    ''' <summary>Gibt "PunktMover" als Bitmap zurück (SVG bevorzugt). Größe: width×height (height=0→quadratisch).</summary>
    Public Function MjGfx_PunktMover(width As Integer, Optional height As Integer = 0) As Bitmap
        Return MjGfx_GfxMain("PunktMover", width, height)
    End Function

    ''' <summary>Gibt "PunktSel" als Bitmap zurück (SVG bevorzugt). Größe: width×height (height=0→quadratisch).</summary>
    Public Function MjGfx_PunktSel(width As Integer, Optional height As Integer = 0) As Bitmap
        Return MjGfx_GfxMain("PunktSel", width, height)
    End Function

    ''' <summary>Gibt "Redo" als Bitmap zurück (SVG bevorzugt). Größe: width×height (height=0→quadratisch).</summary>
    Public Function MjGfx_Redo(width As Integer, Optional height As Integer = 0) As Bitmap
        Return MjGfx_GfxMain("Redo", width, height)
    End Function

    ''' <summary>Gibt "Restart" als Bitmap zurück (SVG bevorzugt). Größe: width×height (height=0→quadratisch).</summary>
    Public Function MjGfx_Restart(width As Integer, Optional height As Integer = 0) As Bitmap
        Return MjGfx_GfxMain("Restart", width, height)
    End Function

    ''' <summary>Gibt "rhomb-outline" als Bitmap zurück (SVG bevorzugt). Größe: width×height (height=0→quadratisch).</summary>
    Public Function MjGfx_rhomb_outline(width As Integer, Optional height As Integer = 0) As Bitmap
        Return MjGfx_GfxMain("rhomb-outline", width, height)
    End Function

    ''' <summary>Gibt "Screenshot" als Bitmap zurück (SVG bevorzugt). Größe: width×height (height=0→quadratisch).</summary>
    Public Function MjGfx_Screenshot(width As Integer, Optional height As Integer = 0) As Bitmap
        Return MjGfx_GfxMain("Screenshot", width, height)
    End Function

    ''' <summary>Gibt "service_toolbox" als Bitmap zurück (SVG bevorzugt). Größe: width×height (height=0→quadratisch).</summary>
    Public Function MjGfx_service_toolbox(width As Integer, Optional height As Integer = 0) As Bitmap
        Return MjGfx_GfxMain("service_toolbox", width, height)
    End Function

    ''' <summary>Gibt "ShowSelectableChecked" als Bitmap zurück (SVG bevorzugt). Größe: width×height (height=0→quadratisch).</summary>
    Public Function MjGfx_ShowSelectableChecked(width As Integer, Optional height As Integer = 0) As Bitmap
        Return MjGfx_GfxMain("ShowSelectableChecked", width, height)
    End Function

    ''' <summary>Gibt "ShowSelectableUnChecked" als Bitmap zurück (SVG bevorzugt). Größe: width×height (height=0→quadratisch).</summary>
    Public Function MjGfx_ShowSelectableUnChecked(width As Integer, Optional height As Integer = 0) As Bitmap
        Return MjGfx_GfxMain("ShowSelectableUnChecked", width, height)
    End Function

    ''' <summary>Gibt "SliderPunkt" als Bitmap zurück (SVG bevorzugt). Größe: width×height (height=0→quadratisch).</summary>
    Public Function MjGfx_SliderPunkt(width As Integer, Optional height As Integer = 0) As Bitmap
        Return MjGfx_GfxMain("SliderPunkt", width, height)
    End Function

    ''' <summary>Gibt "Start" als Bitmap zurück (SVG bevorzugt). Größe: width×height (height=0→quadratisch).</summary>
    Public Function MjGfx_Start(width As Integer, Optional height As Integer = 0) As Bitmap
        Return MjGfx_GfxMain("Start", width, height)
    End Function

    ''' <summary>Gibt "Statistik" als Bitmap zurück (SVG bevorzugt). Größe: width×height (height=0→quadratisch).</summary>
    Public Function MjGfx_Statistik(width As Integer, Optional height As Integer = 0) As Bitmap
        Return MjGfx_GfxMain("Statistik", width, height)
    End Function

    ''' <summary>Gibt "Tip" als Bitmap zurück (SVG bevorzugt). Größe: width×height (height=0→quadratisch).</summary>
    Public Function MjGfx_Tip(width As Integer, Optional height As Integer = 0) As Bitmap
        Return MjGfx_GfxMain("Tip", width, height)
    End Function

    ''' <summary>Gibt "Tipps" als Bitmap zurück (SVG bevorzugt). Größe: width×height (height=0→quadratisch).</summary>
    Public Function MjGfx_Tipps(width As Integer, Optional height As Integer = 0) As Bitmap
        Return MjGfx_GfxMain("Tipps", width, height)
    End Function

    ''' <summary>Gibt "triangle-outline-variant" als Bitmap zurück (SVG bevorzugt). Größe: width×height (height=0→quadratisch).</summary>
    Public Function MjGfx_triangle_outline_variant(width As Integer, Optional height As Integer = 0) As Bitmap
        Return MjGfx_GfxMain("triangle-outline-variant", width, height)
    End Function

    ''' <summary>Gibt "Undo" als Bitmap zurück (SVG bevorzugt). Größe: width×height (height=0→quadratisch).</summary>
    Public Function MjGfx_Undo(width As Integer, Optional height As Integer = 0) As Bitmap
        Return MjGfx_GfxMain("Undo", width, height)
    End Function

    ''' <summary>Gibt "vector-circle" als Bitmap zurück (SVG bevorzugt). Größe: width×height (height=0→quadratisch).</summary>
    Public Function MjGfx_vector_circle(width As Integer, Optional height As Integer = 0) As Bitmap
        Return MjGfx_GfxMain("vector-circle", width, height)
    End Function

    ''' <summary>Gibt "Vertikal" als Bitmap zurück (SVG bevorzugt). Größe: width×height (height=0→quadratisch).</summary>
    Public Function MjGfx_Vertikal(width As Integer, Optional height As Integer = 0) As Bitmap
        Return MjGfx_GfxMain("Vertikal", width, height)
    End Function

    ''' <summary>Gibt "WerkbkFileCmd_Speichern" als Bitmap zurück (SVG bevorzugt). Größe: width×height (height=0→quadratisch).</summary>
    Public Function MjGfx_WerkbkFileCmd_Speichern(width As Integer, Optional height As Integer = 0) As Bitmap
        Return MjGfx_GfxMain("WerkbkFileCmd_Speichern", width, height)
    End Function

    ''' <summary>Gibt "WerkbkrFileCmd_Laden_Eigene" als Bitmap zurück (SVG bevorzugt). Größe: width×height (height=0→quadratisch).</summary>
    Public Function MjGfx_WerkbkrFileCmd_Laden_Eigene(width As Integer, Optional height As Integer = 0) As Bitmap
        Return MjGfx_GfxMain("WerkbkrFileCmd_Laden_Eigene", width, height)
    End Function

    ''' <summary>Gibt "WerkkbkFileCmd_Speichern" als Bitmap zurück (SVG bevorzugt). Größe: width×height (height=0→quadratisch).</summary>
    Public Function MjGfx_WerkkbkFileCmd_Speichern(width As Integer, Optional height As Integer = 0) As Bitmap
        Return MjGfx_GfxMain("WerkkbkFileCmd_Speichern", width, height)
    End Function

    ''' <summary>Gibt "Werkzeug" als Bitmap zurück (SVG bevorzugt). Größe: width×height (height=0→quadratisch).</summary>
    Public Function MjGfx_Werkzeug(width As Integer, Optional height As Integer = 0) As Bitmap
        Return MjGfx_GfxMain("Werkzeug", width, height)
    End Function

    ''' <summary>Gibt "WerkzeugAktiv" als Bitmap zurück (SVG bevorzugt). Größe: width×height (height=0→quadratisch).</summary>
    Public Function MjGfx_WerkzeugAktiv(width As Integer, Optional height As Integer = 0) As Bitmap
        Return MjGfx_GfxMain("WerkzeugAktiv", width, height)
    End Function

    ''' <summary>Gibt "WindsChecked" als Bitmap zurück (SVG bevorzugt). Größe: width×height (height=0→quadratisch).</summary>
    Public Function MjGfx_WindsChecked(width As Integer, Optional height As Integer = 0) As Bitmap
        Return MjGfx_GfxMain("WindsChecked", width, height)
    End Function

    ''' <summary>Gibt "WindsUnChecked" als Bitmap zurück (SVG bevorzugt). Größe: width×height (height=0→quadratisch).</summary>
    Public Function MjGfx_WindsUnChecked(width As Integer, Optional height As Integer = 0) As Bitmap
        Return MjGfx_GfxMain("WindsUnChecked", width, height)
    End Function

    ''' <summary>Erzeugt eine ImageList aus "ImageList_Mark" (Items alphabetisch nach BaseName) in gewünschter Größe.</summary>
    Public Function MjGfx_ImageList_Mark(width As Integer, Optional height As Integer = 0) As ImageList
        Return BuildImageListForFolder("ImageList_Mark", width, height)
    End Function

    ''' <summary>Erzeugt eine ImageList aus "ImageList_Toolbox" (Items alphabetisch nach BaseName) in gewünschter Größe.</summary>
    Public Function MjGfx_ImageList_Toolbox(width As Integer, Optional height As Integer = 0) As ImageList
        Return BuildImageListForFolder("ImageList_Toolbox", width, height)
    End Function

End Module
