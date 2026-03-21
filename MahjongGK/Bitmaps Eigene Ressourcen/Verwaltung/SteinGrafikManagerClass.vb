'###############################################################################
'# SteinGrafikManager – zentrale Verwaltung
'###############################################################################
Imports System.Drawing.Drawing2D
Imports System.Drawing.Imaging
Imports System.IO
Imports System.IO.Compression
Imports System.Xml.Serialization

Namespace Images

    Public Class SteinGrafikManagerClass

        Public Sub New()

        End Sub

        ' --------- Konfiguration / Zustand ---------
        ' Root gemäß Vorgabe: Programmverzeichnis\ "Eigene Ressourcen"
        Private ReadOnly _root As String = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Bitmaps Eigene Ressourcen")

        Private _currentSatz As SteinSatz = SteinSatz.None
        Private _pack As SteinPack = Nothing

        ' Fehlertext von der letzten Operation (Preload, etc.)
        Public ReadOnly Property LastError As String
            Get
                Return _lastError
            End Get
        End Property
        Private _lastError As String = ""

        ' Basis-Bilder (Referenzgröße aus XML) – unabhängig von Rendering/Size
        ' Key: $"{status}|{index}" (OrdinalIgnoreCase)
        Private ReadOnly _baseCache As New Dictionary(Of String, Bitmap)(StringComparer.OrdinalIgnoreCase)

        ' Für jede Rendering-Gruppe ein Größen-gebundener Cache [Status, Index] → Bitmap (skaliert)
        ' und die zugehörige zuletzt verwendete Size
        Private NotInheritable Class RenderCache
            Public LastSize As Size = Size.Empty
            ' 2D-Array: (SteinStatusCount, SteinIndexCount)
            Public Bitmaps As Bitmap(,) = Nothing
            Public Sub Clear()
                If Bitmaps IsNot Nothing Then
                    Dim sCount As Integer = Bitmaps.GetLength(0)
                    Dim iCount As Integer = Bitmaps.GetLength(1)
                    For s As Integer = 0 To sCount - 1
                        For i As Integer = 0 To iCount - 1
                            Dim b As Bitmap = Bitmaps(s, i)
                            If b IsNot Nothing Then
                                b.Dispose()
                                Bitmaps(s, i) = Nothing
                            End If
                        Next
                    Next
                End If
                LastSize = Size.Empty
                Bitmaps = Nothing
            End Sub
        End Class

        Private ReadOnly _rcSpielfeld As New RenderCache()
        Private ReadOnly _rcEditor As New RenderCache()

        Private Shared ReadOnly SteinStatusCount As Integer = [Enum].GetValues(GetType(SteinStatus)).Length
        Private Shared ReadOnly SteinIndexCount As Integer = [Enum].GetValues(GetType(SteinIndexEnum)).Length

        '###############################################################################
        '# Öffentliche API
        '###############################################################################

        ''' <summary>
        ''' Lädt/wechsel den SteinSatz. Liest XML ein und leert sämtliche Caches.
        ''' Optional: Basis-Bitmaps (Referenzgröße) sofort dekodieren (kein Vorrats-Scaling!).
        ''' satz = SteinSatz.None löscht alles und gibt True zurück.
        ''' </summary>
        Public Function PreloadSteinSatz(satz As SteinSatz, Optional decodeAllBasePNGs As Boolean = True) As Boolean

            _lastError = ""

            If satz = SteinSatz.None Then
                _currentSatz = satz
                Rendering_OrgGrafikSteinsatz = satz
                Rendering_OrgGrafikSizeWidth = -1
                Rendering_OrgGrafikSizeHeight = -1
                ' Alle Caches leeren
                _pack = Nothing
                _baseCache.Clear()
                ClearAllCaches()
                Return True '
            End If

            Try
                Dim dir As String = _root 'Path.Combine(_root, satz.ToString())
                If Not Directory.Exists(dir) Then
                    Throw New DirectoryNotFoundException($"Steinsatz-Ordner fehlt: {dir}")
                End If

                Dim xmlFile As String = Path.Combine(dir, $"Steine_{satz}.xml")
                If Not File.Exists(xmlFile) Then
                    Throw New FileNotFoundException("Stein-XML fehlt.", xmlFile)
                End If

                Dim ser As New XmlSerializer(GetType(SteinPack))
                Using fs As FileStream = File.OpenRead(xmlFile)
                    _pack = DirectCast(ser.Deserialize(fs), SteinPack)
                End Using

                If _pack Is Nothing OrElse _pack.Entries Is Nothing OrElse _pack.RefWidth <= 0 OrElse _pack.RefHeight <= 0 Then
                    Throw New InvalidDataException("SteinPack leer/ungültig.")
                End If

                _currentSatz = satz
                Rendering_OrgGrafikSteinsatz = satz
                Rendering_OrgGrafikSizeWidth = _pack.RefWidth
                Rendering_OrgGrafikSizeHeight = _pack.RefHeight

                ' Alle Caches leeren
                ClearAllCaches()

                ' Optional: alle Basis-PNGs (Ref-Größe) jetzt dekodieren (schneller bei späterem Zugriff).
                _baseCache.Clear()
                If decodeAllBasePNGs Then
                    Dim statuses As SteinStatus() = CType([Enum].GetValues(GetType(SteinStatus)), SteinStatus())
                    Dim indices As SteinIndexEnum() = CType([Enum].GetValues(GetType(SteinIndexEnum)), SteinIndexEnum())

                    For Each st As SteinStatus In statuses
                        For Each idx As SteinIndexEnum In indices
                            Dim baseBmp As Bitmap = ResolveBaseBitmap(st, idx) ' dekodiert oder Ghost
                            Dim key As String = MakeKey(st, idx)
                            If Not _baseCache.ContainsKey(key) Then
                                _baseCache.Add(key, baseBmp)
                            Else
                                ' sollte nicht vorkommen; im Zweifel ersetzen
                                _baseCache(key)?.Dispose()
                                _baseCache(key) = baseBmp
                            End If
                        Next
                    Next
                Else
                    ' Lazy: _baseCache bleibt leer; ResolveBaseBitmap() dekodiert bei Bedarf
                End If

                Return True

            Catch ex As Exception
                _lastError = ex.Message
                _pack = Nothing
                _baseCache.Clear()
                ClearAllCaches()
                Return False
            End Try
        End Function

        ''' <summary>
        ''' Liefert (zeitkritisch) die Bitmap gemäß Regeln.
        ''' Regeln:
        ''' 1) keine Vorerzeugung: Größenabhängige Bitmaps nur on-demand.
        ''' 2) Größenwechsel je RenderingEnum → Cache dieser Gruppe leeren.
        ''' </summary>
        Public Function GetStein(index As SteinIndexEnum, status As SteinStatus, size As Size, aktRendering As AktRenderMode) As Bitmap


            ' Muss ein Satz geladen sein.
            If _pack Is Nothing Then
                Throw New Exception("Kein Steinsatz geladen. Zuerst PreloadSteinSatz(...) aufrufen.")
            End If

            ' Ziel-Rendercache wählen
            Dim rc As RenderCache = SelectRenderCache(aktRendering)
            If rc Is Nothing Then Return Nothing

            ' Regel 4 – Größenwechsel → Cache dieser Gruppe leeren
            If size.IsEmpty OrElse size.Width <= 0 OrElse size.Height <= 0 Then
                Throw New Exception("Ungültige Zielgröße.")
            End If

            If rc.LastSize <> size Then
                rc.Clear()
                rc.LastSize = size
                rc.Bitmaps = New Bitmap(SteinStatusCount - 1, SteinIndexCount - 1) {}
            End If

            ' Render-Cache-Hit?
            Dim cached As Bitmap = rc.Bitmaps(CInt(status), CInt(index))
            If cached IsNot Nothing Then Return cached

            ' Basis-Bitmap (Ref-Größe) holen (vorab dekodiert oder lazy)
            Dim baseBmp As Bitmap = GetBaseBitmap(status, index)
            If baseBmp Is Nothing Then baseBmp = RenderGhost(_pack.RefWidth, _pack.RefHeight, index.ToString())

            ' Skalieren 
            Dim scaled As Bitmap = ScaleBitmap(baseBmp, size)

            ' In den Render-Cache legen
            rc.Bitmaps(CInt(status), CInt(index)) = scaled
            Return scaled

        End Function


        ''' <summary>
        ''' Liefert die Referenzgröße des geladenen Satzes (w, h). New Site, wenn nichts geladen
        ''' </summary>
        Public Function GetOrgBitmapSize() As Size
            If _pack Is Nothing Then Return New Size
            Return New Size(_pack.RefWidth, _pack.RefHeight)
        End Function

        Public ReadOnly Property GetOrgBitmapsAreLoaded As Boolean
            Get
                Return _pack Is Nothing
            End Get
        End Property

        Public ReadOnly Property GetCurrentSteinsatz As SteinSatz
            Get
                Return _currentSatz
            End Get
        End Property


        '###############################################################################
        '# Hilfsfunktionen (privat)
        '###############################################################################

        Private Sub ClearAllCaches()
            _rcSpielfeld.Clear()
            _rcEditor.Clear()

            ' Basis-Bitmaps entsorgen
            For Each kvp As KeyValuePair(Of String, Bitmap) In _baseCache
                kvp.Value?.Dispose()
            Next
            _baseCache.Clear()
        End Sub

        Private Shared Function MakeKey(status As SteinStatus, index As SteinIndexEnum) As String
            Return status.ToString() & "|" & index.ToString()
        End Function

        Private Function SelectRenderCache(r As AktRenderMode) As RenderCache
            Select Case r
                Case AktRenderMode.Spiel : Return _rcSpielfeld
                Case AktRenderMode.Edit : Return _rcEditor
                Case Else : Return Nothing
            End Select
        End Function

        Private Function GetBaseBitmap(status As SteinStatus, index As SteinIndexEnum) As Bitmap
            Dim key As String = MakeKey(status, index)
            Dim bmp As Bitmap = Nothing
            If _baseCache.TryGetValue(key, bmp) Then
                Return bmp
            End If

            ' Lazy: dekodieren/erzeugen und in _baseCache hinterlegen
            bmp = ResolveBaseBitmap(status, index)
            _baseCache(key) = bmp
            Return bmp
        End Function

        ''' <summary>
        ''' Sucht den XML-Eintrag und dekodiert PNG (Ref-Größe) oder erzeugt Ghost.
        ''' Gibt IMMER ein Bitmap zurück (nie Nothing).
        ''' </summary>
        Private Function ResolveBaseBitmap(status As SteinStatus, index As SteinIndexEnum) As Bitmap
            ' Schutz, falls _pack fehlt
            If _pack Is Nothing OrElse _pack.Entries Is Nothing Then
                Return RenderGhost(200, 250, index.ToString()) ' defensive Defaults
            End If

            Dim sStatus As String = status.ToString()
            Dim sIndex As String = index.ToString()
            Dim entry As SteinEntry = _pack.Entries.FirstOrDefault(Function(e) e.Status = sStatus AndAlso e.Index = sIndex)

            If entry Is Nothing OrElse String.IsNullOrWhiteSpace(entry.PngGzBase64) Then
                Return RenderGhost(_pack.RefWidth, _pack.RefHeight, index.ToString())
            End If

            Dim raw As Byte() = InflateGZip(entry.PngGzBase64)
            Using ms As New MemoryStream(raw)
                Return DirectCast(Image.FromStream(ms), Bitmap)
            End Using
        End Function

        Private Shared Function InflateGZip(b64 As String) As Byte()
            Dim gz() As Byte = Convert.FromBase64String(b64)
            Using ms As New MemoryStream(gz),
                  gzStream As New GZipStream(ms, CompressionMode.Decompress),
                  outMs As New MemoryStream()
                gzStream.CopyTo(outMs)
                Return outMs.ToArray()
            End Using
        End Function

        Private Shared Function ScaleBitmap(src As Bitmap, targetSize As Size) As Bitmap
            If src Is Nothing Then Throw New ArgumentNullException(NameOf(src))
            If targetSize.Width <= 0 OrElse targetSize.Height <= 0 Then Throw New ArgumentOutOfRangeException(NameOf(targetSize))

            Dim dst As New Bitmap(targetSize.Width, targetSize.Height, PixelFormat.Format32bppPArgb)
            Using g As Graphics = Graphics.FromImage(dst)
                g.PixelOffsetMode = PixelOffsetMode.HighQuality
                g.InterpolationMode = InterpolationMode.HighQualityBicubic
                g.SmoothingMode = SmoothingMode.HighQuality
                g.CompositingQuality = CompositingQuality.HighQuality
                g.DrawImage(src, New Rectangle(Point.Empty, targetSize))
            End Using
            Return dst
        End Function

        Private Shared Function RenderGhost(w As Integer, h As Integer, labelText As String) As Bitmap
            Dim bmp As New Bitmap(w, h, PixelFormat.Format32bppPArgb)
            Using g As Graphics = Graphics.FromImage(bmp)
                g.SmoothingMode = SmoothingMode.AntiAlias
                g.Clear(Color.Transparent)
                Using bg As New SolidBrush(Color.FromArgb(128, 160, 160, 160))
                    g.FillRectangle(bg, 0, 0, w, h)
                End Using
                Dim r As Integer = Math.Min(w, h) - 10
                Using pen As New Pen(Color.Red, Math.Max(2, r \ 20))
                    g.DrawEllipse(pen, (w - r) \ 2, (h - r) \ 2, r, r)
                End Using
                Using f As New Font("Segoe UI", Math.Max(8, Math.Min(w, h) \ 6), FontStyle.Bold, GraphicsUnit.Pixel),
                          sb As New SolidBrush(Color.FromArgb(220, 40, 40, 40))
                    Dim sz As SizeF = g.MeasureString(labelText, f)
                    g.DrawString(labelText, f, sb, (w - sz.Width) / 2.0F, (h - sz.Height) / 2.0F)
                End Using
            End Using
            Return bmp
        End Function
    End Class
End Namespace