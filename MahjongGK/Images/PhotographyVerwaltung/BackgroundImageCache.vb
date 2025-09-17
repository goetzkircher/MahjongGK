Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

Imports System.Drawing.Imaging
Imports System.IO
Imports System.Runtime.InteropServices
Imports ImageMagick

''' <summary>
''' Bild-Cache für Hintergrundbilder mit Größenanpassung, optionaler Proportionserhaltung
''' (Letter/Pillarbox) und automatischer Hintergrundfarbe (dominante Farbe).
''' 
''' <para>
''' WICHTIG: Die von <see cref="LoadImage"/> zurückgegebenen <see cref="Bitmap"/>-Objekte
''' gehören dem Cache. Bitte NICHT entsorgen. Wenn du eine eigene Lebensdauer benötigst,
''' nutze <c>DirectCast(returned.Clone(), Bitmap)</c>.
''' </para>
''' 
''' <para>
''' Thread-safe: Interner Zugriff wird per <c>SyncLock</c> serialisiert.
''' </para>
''' </summary>
Public NotInheritable Class BackgroundImageCache
    Implements IDisposable

    ''' <summary>
    ''' Interner Cache-Eintrag: Originalbild + zuletzt erzeugte, parametrisierte
    ''' Resized-Variante. Für jeden Pfad existiert genau ein <see cref="Entry"/>.
    ''' </summary>
    Private NotInheritable Class Entry
        ''' <summary>Unverändertes Originalbild (via Magick geladen).</summary>
        Public Property Original As Bitmap
        ''' <summary>Letzte gebaute Zielbitmap (abhängig von Größe/Optionen).</summary>
        Public Property Resized As Bitmap
        ''' <summary>Zuletzt berechnete Zielgröße.</summary>
        Public Property ResizedSize As Size
        ''' <summary>Flag: Proportionen erhalten (Letter/Pillarbox) ja/nein.</summary>
        Public Property ResizedPreserve As Boolean
        ''' <summary>Verwendete Hintergrundfarbe (bei Letter/Pillarbox).</summary>
        Public Property ResizedBg As Color
    End Class

    ' Schlüssel = normalisierter Vollpfad, Wert = Entry
    Private ReadOnly _map As New Dictionary(Of String, Entry)(StringComparer.OrdinalIgnoreCase)

    ' Serialisiert alle Lese/Schreibzugriffe auf _map und Entries
    Private ReadOnly _gate As New Object()

    Private _disposed As Boolean

    Private Const DEFAULT_STEPS As Integer = 4

    ' ───────────────── Öffentliche API ─────────────────

    ''' <summary>
    ''' Lädt ein Bild in Originalgröße und cached es (keine Skalierung).
    ''' </summary>
    ''' <param name="path">Dateipfad (relativ oder absolut).</param>
    ''' <returns>Referenz auf das gecachte Original-Bitmap (nicht entsorgen!).</returns>
    Public Function LoadImage(path As String) As Bitmap
        Return LoadImage(path, Size.Empty, preserveOrgProportion:=False, backgroundColor:=Color.Transparent)
    End Function

    ''' <summary>
    ''' Lädt ein Bild und skaliert auf die gewünschte Zielgröße (ggf. verzerrt).
    ''' </summary>
    ''' <param name="path">Dateipfad (relativ oder absolut).</param>
    ''' <param name="size">Zielgröße; 0 in einer Dimension → proportional berechnet.</param>
    ''' <returns>Referenz auf gecachte, erzeugte Ziel-Bitmap (nicht entsorgen!).</returns>
    Public Function LoadImage(path As String, size As Size) As Bitmap
        Return LoadImage(path, size, preserveOrgProportion:=False, backgroundColor:=Color.Transparent)
    End Function

    ''' <summary>
    ''' Lädt ein Bild, skaliert auf Ziel und ermittelt die Hintergrundfarbe automatisch
    ''' aus der dominanten Bildfarbe (Histogramm, Sampling über INI konfigurierbar).
    ''' </summary>
    ''' <param name="path">Dateipfad.</param>
    ''' <param name="size">Zielgröße; 0 in einer Dimension → proportional berechnet.</param>
    ''' <param name="preserveOrgProportion">True → Letter/Pillarbox statt Verzerrung (Schwelle via INI).</param>
    ''' <returns>Referenz auf gecachte Ziel-Bitmap (nicht entsorgen!).</returns>
    ''' <exception cref="ObjectDisposedException">Wenn bereits disposed.</exception>
    ''' <exception cref="ArgumentNullException">Wenn Pfad leer/weißleer.</exception>
    Public Function LoadImage(path As String,
                              size As Size,
                              preserveOrgProportion As Boolean) As Bitmap
        If _disposed Then Throw New ObjectDisposedException(NameOf(BackgroundImageCache))
        If String.IsNullOrWhiteSpace(path) Then Throw New ArgumentNullException(NameOf(path))

        Dim full As String = GetNormalizedFullPath(path)

        SyncLock _gate
            ' Entry erzeugen oder existierenden holen
            Dim e As Entry = Nothing
            If Not _map.TryGetValue(full, e) Then
                e = New Entry() With {
                    .Original = LoadOriginalBitmap(full), ' via Magick + Fallback ErrorBitmap
                    .Resized = Nothing,
                    .ResizedSize = Size.Empty,
                    .ResizedPreserve = False,
                    .ResizedBg = Color.Empty
                }
                _map(full) = e
            End If

            ' dominante Hauptfarbe (Sampling-Raster via INI; dunkle/entsättigte Pixel werden geringer gewichtet)
            'Entsättigte Pixel = Pixel mit niedriger Sättigung (unter minSaturation, z. B. 0,1).
            'Sie werden mit einem kleineren Gewicht in das Histogramm eingerechnet (satWeight < 1).
            'Damit dominieren nicht versehentlich große graue Flächen (z. B. Hintergrund oder Nebel) die Bestimmung der „dominanten“ Farbe.
            Dim stepXY As Integer = INI.BackGroundImage_ColorSampleStep
            Dim autoBg As Color = ComputeDominantColor(e.Original, stepXY, minAlpha:=16)

            Return LoadImage_Internal(e, size, preserveOrgProportion, autoBg)
        End SyncLock
    End Function

    ''' <summary>
    ''' Lädt ein Bild, skaliert auf Ziel und nutzt eine explizit vorgegebene Hintergrundfarbe
    ''' für Letter/Pillarbox (wenn <paramref name="preserveOrgProportion"/> aktiv und Verzerrung über Schwelle).
    ''' </summary>
    ''' <param name="path">Dateipfad.</param>
    ''' <param name="size">Zielgröße; 0 in einer Dimension → proportional berechnet.</param>
    ''' <param name="preserveOrgProportion">True → Letter/Pillarbox statt Verzerrung (Schwelle via INI).</param>
    ''' <param name="backgroundColor">Hintergrundfarbe für Ränder.</param>
    ''' <returns>Referenz auf gecachte Ziel-Bitmap (nicht entsorgen!).</returns>
    ''' <exception cref="ObjectDisposedException">Wenn bereits disposed.</exception>
    ''' <exception cref="ArgumentNullException">Wenn Pfad leer/weißleer.</exception>
    Public Function LoadImage(path As String,
                              size As Size,
                              preserveOrgProportion As Boolean,
                              backgroundColor As Color) As Bitmap
        If _disposed Then Throw New ObjectDisposedException(NameOf(BackgroundImageCache))
        If String.IsNullOrWhiteSpace(path) Then Throw New ArgumentNullException(NameOf(path))

        Dim full As String = GetNormalizedFullPath(path)

        SyncLock _gate
            Dim e As Entry = Nothing
            If Not _map.TryGetValue(full, e) Then
                e = New Entry() With {
                    .Original = LoadOriginalBitmap(full),
                    .Resized = Nothing,
                    .ResizedSize = Size.Empty,
                    .ResizedPreserve = False,
                    .ResizedBg = Color.Empty
                }
                _map(full) = e
            End If

            Return LoadImage_Internal(e, size, preserveOrgProportion, backgroundColor)
        End SyncLock
    End Function

    ' ───────────── Ergänzung: entsorgbare Kopien anfordern ─────────────

    ''' <summary>
    ''' Liefert eine entsorgbare Kopie des Originalbildes (keine Skalierung).
    ''' Besitzer ist der Aufrufer → <c>Dispose()</c> nicht vergessen.
    ''' </summary>
    Public Function LoadImageClone(path As String) As Bitmap
        Dim ref As Bitmap = LoadImage(path)
        Return SafeClone(ref)
    End Function

    ''' <summary>
    ''' Liefert eine entsorgbare Kopie in gewünschter Größe (ggf. verzerrt).
    ''' Besitzer ist der Aufrufer → <c>Dispose()</c> nicht vergessen.
    ''' </summary>
    Public Function LoadImageClone(path As String, size As Size) As Bitmap
        Dim ref As Bitmap = LoadImage(path, size)
        Return SafeClone(ref)
    End Function

    ''' <summary>
    ''' Liefert eine entsorgbare Kopie in gewünschter Größe, optional mit
    ''' Proportionserhalt (Letter/Pillarbox, Hintergrundfarbe automatisch).
    ''' Besitzer ist der Aufrufer → <c>Dispose()</c> nicht vergessen.
    ''' </summary>
    Public Function LoadImageClone(path As String,
                               size As Size,
                               preserveOrgProportion As Boolean) As Bitmap
        Dim ref As Bitmap = LoadImage(path, size, preserveOrgProportion)
        Return SafeClone(ref)
    End Function

    ''' <summary>
    ''' Liefert eine entsorgbare Kopie in gewünschter Größe, optional mit
    ''' Proportionserhalt (Letter/Pillarbox) und expliziter Hintergrundfarbe.
    ''' Besitzer ist der Aufrufer → <c>Dispose()</c> nicht vergessen.
    ''' </summary>
    Public Function LoadImageClone(path As String,
                               size As Size,
                               preserveOrgProportion As Boolean,
                               backgroundColor As Color) As Bitmap
        Dim ref As Bitmap = LoadImage(path, size, preserveOrgProportion, backgroundColor)
        Return SafeClone(ref)
    End Function

    ''' <summary>
    ''' Try-Variante: gibt bei Erfolg True zurück und liefert eine entsorgbare Kopie.
    ''' Bei Fehler False und <paramref name="result"/> = Nothing (keine Exceptions).
    ''' </summary>
    Public Function TryLoadImageClone(path As String,
                                  size As Size,
                                  preserveOrgProportion As Boolean,
                                  backgroundColor As Color,
                                  ByRef result As Bitmap) As Boolean
        result = Nothing
        Try
            Dim ref As Bitmap = LoadImage(path, size, preserveOrgProportion, backgroundColor)
            result = SafeClone(ref)
            Return True
        Catch
            result = Nothing
            Return False
        End Try
    End Function


    ''' <summary>
    ''' Entfernt den Cache-Eintrag (und entsorgt Bitmaps) für den angegebenen Pfad.
    ''' </summary>
    ''' <param name="path">Dateipfad; leer → ignoriert.</param>
    Public Sub Invalidate(path As String)
        If _disposed Then Return
        If String.IsNullOrWhiteSpace(path) Then Return

        Dim full As String = GetNormalizedFullPath(path)

        SyncLock _gate
            Dim e As Entry = Nothing
            If _map.TryGetValue(full, e) Then
                If e.Resized IsNot Nothing Then e.Resized.Dispose() : e.Resized = Nothing
                If e.Original IsNot Nothing Then e.Original.Dispose() : e.Original = Nothing
                _map.Remove(full)
            End If
        End SyncLock
    End Sub

    ''' <summary>
    ''' Leert den gesamten Cache und entsorgt alle Bitmaps.
    ''' </summary>
    Public Sub Clear()
        If _disposed Then Return
        SyncLock _gate
            For Each e As Entry In _map.Values
                If e.Resized IsNot Nothing Then e.Resized.Dispose()
                If e.Original IsNot Nothing Then e.Original.Dispose()
            Next
            _map.Clear()
        End SyncLock
    End Sub

    ''' <summary>
    ''' Entsorgt den Cache (implizit <see cref="Clear"/>). Danach sind alle
    ''' Methodenaufrufe ungültig und werfen <see cref="ObjectDisposedException"/>.
    ''' </summary>
    Public Sub Dispose() Implements IDisposable.Dispose
        If _disposed Then Return
        Clear()
        _disposed = True
        GC.SuppressFinalize(Me)
    End Sub

    ' ───────────────── Interna ─────────────────

    ''' <summary>
    ''' Kernlogik: Zielgröße bestimmen, Cache-Treffer prüfen und ggf. neu rendern.
    ''' </summary>
    Private Function LoadImage_Internal(e As Entry,
                                        size As Size,
                                        preserveOrgProportion As Boolean,
                                        backgroundColor As Color) As Bitmap
        ' Zielgröße (Width/Height 0 → proportional ergänzen) aus Original ableiten
        Dim target As Size = ComputeTargetSize(e.Original.Size, size)

        ' Originalgröße? → Original zurückgeben (kein Clone: Besitz bleibt beim Cache)
        If target.Width = e.Original.Width AndAlso target.Height = e.Original.Height Then
            Return e.Original
        End If

        ' Prüfen, ob die bereits gebaute Variante für dieselben Parameter wiederverwendbar ist
        Dim mustRebuild As Boolean =
            (e.Resized Is Nothing) OrElse
            (Not e.ResizedSize.Equals(target)) OrElse
            (e.ResizedPreserve <> preserveOrgProportion) OrElse
            (e.ResizedBg.ToArgb() <> backgroundColor.ToArgb())

        If mustRebuild Then
            ' Alte Zielbitmap weg (um GDI-Handles/Memory freizugeben)
            If e.Resized IsNot Nothing Then
                e.Resized.Dispose()
                e.Resized = Nothing
            End If

            Dim built As Bitmap

            ' Verzerrungsprüfung: Nur wenn preserve=true und die INI-Schwelle überschritten wird,
            ' wird wirklich Letter/Pillarbox gerechnet – ansonsten direkte Skalierung.
            If preserveOrgProportion AndAlso WouldDistort(e.Original.Size, target) Then
                ' proportional einpassen (Innenpassung)
                Dim fit As Size = FitInside(e.Original.Size, target)

                ' Hochwertige Skalierung (erst Portierung via ShrinkBitmap, sonst GDI+ Fallback)
                Dim scaled As Bitmap = ScaleBitmapHighQuality(e.Original, fit)

                ' Endbild in Zielgröße mit Hintergrundleiste(n)
                built = New Bitmap(target.Width, target.Height, PixelFormat.Format32bppArgb)
                Using g As Graphics = Graphics.FromImage(built)
                    Using bg As New SolidBrush(backgroundColor)
                        g.FillRectangle(bg, 0, 0, built.Width, built.Height)
                    End Using
                    If INI.BackGroundImage_HeightQuality Then
                        ' GDI+ Render-Qualität hochsetzen
                        g.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic
                        g.PixelOffsetMode = Drawing2D.PixelOffsetMode.HighQuality
                        g.SmoothingMode = Drawing2D.SmoothingMode.HighQuality
                        g.CompositingQuality = Drawing2D.CompositingQuality.HighQuality
                    Else
                        g.InterpolationMode = Drawing2D.InterpolationMode.NearestNeighbor
                    End If
                    ' Zentrierte Einpassung
                    Dim x As Integer = (target.Width - fit.Width) \ 2
                    Dim y As Integer = (target.Height - fit.Height) \ 2
                    g.DrawImage(scaled, New Rectangle(x, y, fit.Width, fit.Height))
                End Using
                scaled.Dispose()
            Else
                ' exakte Zielgröße (Verzerrung erlaubt oder unterhalb Schwelle)
                built = ScaleBitmapHighQuality(e.Original, target)
            End If

            ' Cache-Status aktualisieren
            e.Resized = built
            e.ResizedSize = target
            e.ResizedPreserve = preserveOrgProportion
            e.ResizedBg = backgroundColor
        End If

        Return e.Resized
    End Function

    ''' <summary>
    ''' Versucht, einen Normalpfad zu erzeugen (für Dictionary-Schlüssel).
    ''' </summary>
    Private Shared Function GetNormalizedFullPath(fullpath As String) As String
        Try
            Return Path.GetFullPath(fullpath)
        Catch
            ' Fallback: notfalls den Eingabewert nutzen (z. B. bei exotischen Pfaden)
            Return fullpath
        End Try
    End Function

    ''' <summary>
    ''' Lädt das Originalbitmap über ImageMagick (robust gegenüber diversen Formaten).
    ''' Bei Fehlern wird eine rote Fehlergrafik erzeugt (damit der Cache nie Nothing speichert).
    ''' </summary>
    Private Shared Function LoadOriginalBitmap(path As String) As Bitmap
        Try
            ' MagickImage kann (je nach Format) Farbprofile/DPI mitnehmen;
            ' wir serialisieren als PNG in Memory, um ein detachtes System.Drawing.Bitmap zu erhalten.
            Using mi As New MagickImage(path)
                Dim bytes() As Byte = mi.ToByteArray(MagickFormat.Png)
                Using ms As New MemoryStream(bytes, writable:=False)
                    Using temp As New Bitmap(ms)
                        Return New Bitmap(temp) ' detach vom Stream/Handle
                    End Using
                End Using
            End Using
        Catch ex As Exception
            ' Keine Exception nach außen: lieber verwertbare Fehlergrafik zurückgeben
            Return CreateErrorBitmap(path, Size.Empty, ex.Message)
        End Try
    End Function

    ''' <summary>
    ''' Leitet aus Original- und Wunschgröße die tatsächlich zu rendernde Zielgröße ab.
    ''' </summary>
    Private Shared Function ComputeTargetSize(original As Size, requested As Size) As Size


        If requested.Width <= 0 AndAlso requested.Height <= 0 Then
            Return original
        End If
        If requested.Width > 0 AndAlso requested.Height > 0 Then
            Return requested
        End If

        ' Eine Dimension fehlt → proportional ergänzen
        Dim ow As Integer = Math.Max(1, original.Width)
        Dim oh As Integer = Math.Max(1, original.Height)

        If requested.Width <= 0 AndAlso requested.Height > 0 Then
            Dim w As Integer = CInt(Math.Round(ow * (requested.Height / CDbl(oh))))
            Return New Size(Math.Max(1, w), requested.Height)
        End If

        If requested.Height <= 0 AndAlso requested.Width > 0 Then
            Dim h As Integer = CInt(Math.Round(oh * (requested.Width / CDbl(ow))))
            Return New Size(requested.Width, Math.Max(1, h))
        End If

        Return original
    End Function

    ''' <summary>
    ''' Prüft, ob die gewünschte Zielgröße eine „zu starke“ Verzerrung erzwingen würde.
    ''' Schwelle über <c>INI.BackGroundImage_MaxDistortionRatio</c> (z. B. 0.10 = 10 %).
    ''' </summary>
    Private Shared Function WouldDistort(original As Size, target As Size) As Boolean
        Dim sx As Double = target.Width / Math.Max(1.0, original.Width)
        Dim sy As Double = target.Height / Math.Max(1.0, original.Height)
        Dim ratio As Double = Math.Max(sx, sy) / Math.Max(0.000001, Math.Min(sx, sy))
        Dim distortion As Double = ratio - 1.0
        Dim threshold As Double = GetDistortionThresholdFromIni()
        Return distortion > threshold
    End Function

    ''' <summary>
    ''' Liest die Verzerrungs-Schwelle aus der INI (Fallback 0.1).
    ''' </summary>
    Private Shared Function GetDistortionThresholdFromIni() As Double
        Try
            Dim v As Double = INI.BackGroundImage_MaxDistortionRatio
            If Double.IsNaN(v) OrElse v < 0 Then Return 0.1
            Return v
        Catch
            Return 0.1
        End Try
    End Function

    ''' <summary>
    ''' Berechnet die größte Innenpassung (fit-inside) ohne Verzerrung.
    ''' </summary>
    Private Shared Function FitInside(original As Size, target As Size) As Size
        Dim sx As Double = target.Width / Math.Max(1.0, original.Width)
        Dim sy As Double = target.Height / Math.Max(1.0, original.Height)
        Dim s As Double = Math.Min(sx, sy)
        Dim w As Integer = Math.Max(1, CInt(Math.Round(original.Width * s)))
        Dim h As Integer = Math.Max(1, CInt(Math.Round(original.Height * s)))
        Return New Size(w, h)
    End Function

    ''' <summary>
    ''' Hochwertige Skalierung: bevorzugt deine <c>MjGDI.ShrinkBitmap</c> (mit stufenweisem Downscale),
    ''' bei Fehlern Fallback auf GDI+ HighQualityBicubic.
    ''' </summary>
    Private Shared Function ScaleBitmapHighQuality(src As Bitmap, target As Size) As Bitmap
        ' Work-Kopie anlegen (ShrinkBitmap kann das übergebene Bitmap übernehmen/entsorgen)
        Dim work As New Bitmap(src)
        Dim ok As Boolean = False
        Try
            ok = MjGDI.ShrinkBitmap(work,
                                    newWidth:=target.Width,
                                    newHeight:=target.Height,
                                    steps:=If(INI.BackGroundImage_HeightQuality, DEFAULT_STEPS, 1),
                                    disposeSrc:=True,     ' gibt Ressourcen frei
                                    highQuality:=INI.BackGroundImage_HeightQuality,
                                    cvtToARGBBitmap:=True)
        Catch
            ok = False
        End Try

        If ok AndAlso work IsNot Nothing Then
            Return work
        End If

        ' Fallback: einmaliges, hochwertiges Resampling
        Dim bmp As New Bitmap(target.Width, target.Height, PixelFormat.Format32bppArgb)
        Using g As Graphics = Graphics.FromImage(bmp)
            g.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic
            g.PixelOffsetMode = Drawing2D.PixelOffsetMode.HighQuality
            g.SmoothingMode = Drawing2D.SmoothingMode.HighQuality
            g.CompositingQuality = Drawing2D.CompositingQuality.HighQuality
            g.DrawImage(src, New Rectangle(0, 0, bmp.Width, bmp.Height))
        End Using
        Return bmp
    End Function

    ''' <summary>
    ''' Erzeugt eine rote Fehler-Bitmap mit Dateiname und optionaler Fehlermeldung.
    ''' Wird verwendet, wenn Laden via Magick fehlschlägt, damit der Cache nie Nothing hält.
    ''' </summary>
    Private Shared Function CreateErrorBitmap(fullpath As String, requested As Size, Optional message As String = Nothing) As Bitmap
        Dim w As Integer = If(requested.Width > 0, requested.Width, 256)
        Dim h As Integer = If(requested.Height > 0, requested.Height, 64)
        Dim bmp As New Bitmap(w, h, PixelFormat.Format32bppArgb)
        Using g As Graphics = Graphics.FromImage(bmp)
            g.Clear(Color.Red)
            Using font As New Font("Segoe UI", 10, FontStyle.Bold)
                Dim text As String = Path.GetFileName(fullpath)
                If Not String.IsNullOrEmpty(message) Then text &= vbCrLf & message
                Dim rect As New RectangleF(4, 4, w - 8, h - 8)
                Dim fmt As New StringFormat With {.Alignment = StringAlignment.Near, .LineAlignment = StringAlignment.Near}
                g.DrawString(text, font, Brushes.White, rect, fmt)
            End Using
        End Using
        Return bmp
    End Function

    ' ───────────── Durchschnittsfarbe (schnell, Alpha-gewichtet), Sampling stepXY ─────────────

    ''' <summary>
    ''' Schnelle Durchschnittsfarbe (Alpha-gewichtet) über grobes Raster.
    ''' Praktisch für einfache Hintergrundableitung, hier nicht direkt von außen verwendet.
    ''' </summary>
    Private Shared Function ComputeAverageColor(src As Bitmap, stepXY As Integer) As Color
        If src Is Nothing Then Return Color.Black
        Dim stepX As Integer = Math.Max(1, stepXY)
        Dim stepY As Integer = Math.Max(1, stepXY)

        ' Für schnellen Zugriff in 32bppArgb konvertieren (LockBits)
        Dim bmp As Bitmap = src
        If bmp.PixelFormat <> PixelFormat.Format32bppArgb Then
            bmp = New Bitmap(src.Width, src.Height, PixelFormat.Format32bppArgb)
            Using g As Graphics = Graphics.FromImage(bmp)
                g.DrawImage(src, 0, 0, src.Width, src.Height)
            End Using
        End If

        Dim rect As New Rectangle(0, 0, bmp.Width, bmp.Height)
        Dim data As BitmapData = bmp.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb)
        Try
            Dim stride As Integer = data.Stride
            Dim basePtr As IntPtr = data.Scan0

            Dim sumR As Double = 0, sumG As Double = 0, sumB As Double = 0, sumA As Double = 0

            For y As Integer = 0 To bmp.Height - 1 Step stepY
                Dim row As Integer = y * stride
                For x As Integer = 0 To bmp.Width - 1 Step stepX
                    Dim p As Integer = row + x * 4
                    Dim b As Byte = Runtime.InteropServices.Marshal.ReadByte(basePtr, p + 0)
                    Dim g As Byte = Runtime.InteropServices.Marshal.ReadByte(basePtr, p + 1)
                    Dim r As Byte = Runtime.InteropServices.Marshal.ReadByte(basePtr, p + 2)
                    Dim a As Byte = Runtime.InteropServices.Marshal.ReadByte(basePtr, p + 3)

                    Dim aw As Double = a / 255.0 ' Alpha-Gewichtung
                    sumR += r * aw : sumG += g * aw : sumB += b * aw : sumA += aw
                Next
            Next

            If sumA <= 0.000001 Then Return Color.Black

            Dim ar As Integer = CInt(Math.Round(sumR / sumA))
            Dim ag As Integer = CInt(Math.Round(sumG / sumA))
            Dim ab As Integer = CInt(Math.Round(sumB / sumA))
            Return Color.FromArgb(255,
                                  Math.Min(255, Math.Max(0, ar)),
                                  Math.Min(255, Math.Max(0, ag)),
                                  Math.Min(255, Math.Max(0, ab)))
        Finally
            bmp.UnlockBits(data)
            If Not Object.ReferenceEquals(bmp, src) Then bmp.Dispose()
        End Try
    End Function

    ' ───────────── Dominante Hauptfarbe (Histogramm-basiert) ─────────────

    ''' <summary>
    ''' Ermittelt die dominante Hauptfarbe über ein quantisiertes RGB-Histogramm (16×16×16).
    ''' Pixel mit geringer Sättigung werden nicht komplett abgewertet, sondern auf einen
    ''' konfigurierbaren Mindestanteil gesetzt. Zusätzlich werden hellere Pixel (V in HSV)
    ''' optional stärker gewichtet (luminanceBoost).
    ''' 
    ''' Stellschrauben (INI):
    ''' • INI.BackgroundImage_minSaturation      ∈ [0..1], Default 0.10
    ''' • INI.BackgroundImage_minWeightForGray   ∈ [0..1], Default 0.30
    ''' • INI.BackgroundImage_luminanceBoost     ≥ 0,    Default 0.50
    ''' </summary>
    Private Function ComputeDominantColor(src As Bitmap,
                                             stepXY As Integer,
                                             Optional minAlpha As Byte = 16) As Color
        If src Is Nothing Then Return Color.Black

        ' --- INI-Werte robust einlesen und clampen ---
        Dim minSat As Double = 0.1
        Dim grayFloor As Double = 0.3
        Dim lumBoost As Double = 0.5
        Try
            minSat = Clamp01(INI.BackGroundImage_MinSaturation)
        Catch
            minSat = 0.1
        End Try
        Try
            grayFloor = Clamp01(INI.BackGroundImage_MinWeightForGray)
        Catch
            grayFloor = 0.3
        End Try
        Try
            lumBoost = Math.Max(0.0, INI.BackGroundImage_LuminanceBoost)
        Catch
            lumBoost = 0.5
        End Try

        Dim stepX As Integer = Math.Max(1, stepXY)
        Dim stepY As Integer = Math.Max(1, stepXY)

        ' Für LockBits in 32bppArgb konvertieren (Kopie nur bei Bedarf)
        Dim bmp As Bitmap = src
        If bmp.PixelFormat <> PixelFormat.Format32bppArgb Then
            bmp = New Bitmap(src.Width, src.Height, PixelFormat.Format32bppArgb)
            Using g As Graphics = Graphics.FromImage(bmp)
                g.DrawImage(src, 0, 0, src.Width, src.Height)
            End Using
        End If

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

            For y As Integer = 0 To bmp.Height - 1 Step stepY
                Dim row As Integer = y * stride
                For x As Integer = 0 To bmp.Width - 1 Step stepX
                    Dim p As Integer = row + x * 4
                    Dim bb As Byte = Marshal.ReadByte(basePtr, p + 0)
                    Dim gg As Byte = Marshal.ReadByte(basePtr, p + 1)
                    Dim rr As Byte = Marshal.ReadByte(basePtr, p + 2)
                    Dim aa As Byte = Marshal.ReadByte(basePtr, p + 3)

                    If aa < minAlpha Then Continue For

                    ' HSV für Sättigung + Luminanz
                    Dim h As Double, s As Double, v As Double
                    RgbToHsv(rr, gg, bb, h, s, v)

                    ' --- Sättigungs-Gewichtung mit Mindestgewicht für graue Pixel ---
                    ' s ∈ [0..1], minSat ∈ [0..1]
                    ' satFactor: 1.0 bei s >= minSat; darunter linear Richtung grayFloor
                    Dim satFactor As Double
                    If s >= minSat Then
                        satFactor = 1.0
                    Else
                        Dim t As Double = s / Math.Max(0.000001, minSat) ' 0..1
                        satFactor = grayFloor + (1.0 - grayFloor) * t    ' ∈ [grayFloor..1]
                    End If

                    ' --- Luminanz-Boost: hellere Pixel bevorzugen ---
                    ' v ∈ [0..1], symmetrisch um 0.5: v=0.5 → Faktor ~1
                    ' lumFactor wird auf [0.2..2.0] begrenzt, um Extreme zu vermeiden.
                    Dim lumFactor As Double
                    If lumBoost <= 0.0 Then
                        lumFactor = 1.0
                    Else
                        lumFactor = 1.0 + lumBoost * (v - 0.5) * 2.0 ' v<0.5 -> <1, v>0.5 -> >1
                        lumFactor = Math.Max(0.2, Math.Min(2.0, lumFactor))
                    End If

                    Dim w As Double = (aa / 255.0) * satFactor * lumFactor
                    If w <= 0 Then Continue For

                    Dim rBin As Integer = (rr >> 4) ' 0..15
                    Dim gBin As Integer = (gg >> 4)
                    Dim bBin As Integer = (bb >> 4)

                    counts(rBin, gBin, bBin) += w
                    sumR(rBin, gBin, bBin) += rr * w
                    sumG(rBin, gBin, bBin) += gg * w
                    sumB(rBin, gBin, bBin) += bb * w
                Next
            Next

            ' Dominanten Bin wählen (maximales Gewicht)
            Dim bestR As Integer = 0, bestG As Integer = 0, bestB As Integer = 0
            Dim bestCount As Double = -1.0
            For r As Integer = 0 To Q - 1
                For g As Integer = 0 To Q - 1
                    For b As Integer = 0 To Q - 1
                        If counts(r, g, b) > bestCount Then
                            bestCount = counts(r, g, b)
                            bestR = r : bestG = g : bestB = b
                        End If
                    Next
                Next
            Next

            If bestCount <= 0 Then Return Color.Black

            ' Mittelwertfarbe des dominanten Bins (robuster als Bin-Zentrum)
            Dim mr As Integer = CInt(Math.Round(sumR(bestR, bestG, bestB) / bestCount))
            Dim mg As Integer = CInt(Math.Round(sumG(bestR, bestG, bestB) / bestCount))
            Dim mb As Integer = CInt(Math.Round(sumB(bestR, bestG, bestB) / bestCount))

            Dim rescol As Color = Color.FromArgb(255,
                              Math.Max(0, Math.Min(255, mr)),
                              Math.Max(0, Math.Min(255, mg)),
                              Math.Max(0, Math.Min(255, mb)))
            If INI.BackGroundImage_Brighten <> 1 Then
                rescol = Brighten(rescol, INI.BackGroundImage_Brighten)
            End If

            Return rescol

        Finally
            bmp.UnlockBits(data)
            If Not Object.ReferenceEquals(bmp, src) Then bmp.Dispose()
        End Try
    End Function

    ' --- kleine Hilfsfunktion zum Clamp ---
    Private Shared Function Clamp01(v As Double) As Double
        If Double.IsNaN(v) Then Return 0.0
        If v < 0.0 Then Return 0.0
        If v > 1.0 Then Return 1.0
        Return v
    End Function

    ''' <summary>
    ''' RGB→HSV-Umrechnung; h in [0..360], s/v in [0..1].
    ''' </summary>
    Private Sub RgbToHsv(r As Byte, g As Byte, b As Byte,
                                ByRef h As Double, ByRef s As Double, ByRef v As Double)
        Dim rf As Double = r / 255.0, gf As Double = g / 255.0, bf As Double = b / 255.0
        Dim maxV As Double = Math.Max(rf, Math.Max(gf, bf))
        Dim minV As Double = Math.Min(rf, Math.Min(gf, bf))
        v = maxV
        Dim d As Double = maxV - minV
        s = If(maxV = 0, 0, d / maxV)
        If d = 0 Then
            h = 0
        Else
            If maxV = rf Then
                h = 60 * (((gf - bf) / d) Mod 6)
            ElseIf maxV = gf Then
                h = 60 * (((bf - rf) / d) + 2)
            Else
                h = 60 * (((rf - gf) / d) + 4)
            End If
            If h < 0 Then h += 360
        End If
    End Sub
    ''' <summary>
    ''' HSV → RGB-Umrechnung. h in [0..360], s/v in [0..1].
    ''' Rückgabe über ByRef r,g,b (0..255).
    ''' </summary>
    Private Sub HsvToRgb(h As Double, s As Double, v As Double,
                            ByRef r As Integer, ByRef g As Integer, ByRef b As Integer)

        If s <= 0.000001 Then
            ' Grauwert (keine Sättigung)
            Dim gv As Integer = CInt(Math.Round(v * 255))
            gv = Math.Max(0, Math.Min(255, gv))
            r = gv : g = gv : b = gv
            Return
        End If

        ' Hue in [0..360)
        h = h Mod 360.0
        If h < 0 Then h += 360.0

        Dim sector As Double = h / 60.0
        Dim i As Integer = CInt(Math.Floor(sector))
        Dim f As Double = sector - i

        Dim p As Double = v * (1.0 - s)
        Dim q As Double = v * (1.0 - s * f)
        Dim t As Double = v * (1.0 - s * (1.0 - f))

        Dim rf As Double, gf As Double, bf As Double
        Select Case i Mod 6
            Case 0 : rf = v : gf = t : bf = p
            Case 1 : rf = q : gf = v : bf = p
            Case 2 : rf = p : gf = v : bf = t
            Case 3 : rf = p : gf = q : bf = v
            Case 4 : rf = t : gf = p : bf = v
            Case Else : rf = v : gf = p : bf = q
        End Select

        r = Math.Max(0, Math.Min(255, CInt(Math.Round(rf * 255))))
        g = Math.Max(0, Math.Min(255, CInt(Math.Round(gf * 255))))
        b = Math.Max(0, Math.Min(255, CInt(Math.Round(bf * 255))))
    End Sub


    Private Function Brighten(c As Color, factor As Double) As Color
        Dim h, s, v As Double
        RgbToHsv(c.R, c.G, c.B, h, s, v)
        v = Math.Min(1.0, v * factor)
        Dim r, g, b As Integer
        HsvToRgb(h, s, v, r, g, b)
        Return Color.FromArgb(255, r, g, b)
    End Function

    ' ───────────── Interna ─────────────

    ''' <summary>
    ''' Erstellt eine sichere, detachte Kopie in 32bppArgb und übernimmt DPI.
    ''' </summary>
    Private Function SafeClone(src As Bitmap) As Bitmap
        If src Is Nothing Then Throw New ArgumentNullException(NameOf(src))
        Dim clone As New Bitmap(src.Width, src.Height, PixelFormat.Format32bppArgb)
        clone.SetResolution(src.HorizontalResolution, src.VerticalResolution)
        Using g As Graphics = Graphics.FromImage(clone)
            g.InterpolationMode = Drawing2D.InterpolationMode.NearestNeighbor ' 1:1
            g.PixelOffsetMode = Drawing2D.PixelOffsetMode.HighQuality
            g.SmoothingMode = Drawing2D.SmoothingMode.None
            g.CompositingQuality = Drawing2D.CompositingQuality.HighQuality
            g.DrawImageUnscaled(src, 0, 0)
        End Using
        Return clone
    End Function
End Class
