Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

Imports System.IO
Imports System.Xml
Imports System.Xml.Serialization
Imports Svg ' aus Svg.NET (C#-Projekt in Lösung)
Public Enum AktGfxBasis
    Satz1
    Satz2
End Enum

Public NotInheritable Class ImageContainer
    Private ReadOnly _root As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                                                    "Visual Studio", "MahjongGK", "Grafiken", "Integrierte")
    Private _basis As AktGfxBasis
    Private _pack As GfxPack

    ' Cache: Key = $"{key}|{w}x{h}|dm={0/1}|dpi={dpiFaktor}"
    Private ReadOnly _bmpCache As New Dictionary(Of String, Bitmap)(StringComparer.OrdinalIgnoreCase)

    ' INI-Spiegel (zur Laufzeit konstant)
    Private ReadOnly _isDark As Boolean = INI.Global_DarkMode
    Private ReadOnly _brighten As Single = INI.Global_BrightenAmount

    Private Sub New()
    End Sub

    Public Shared ReadOnly Property Instance As ImageContainer = New ImageContainer()

    Public Sub LoadBasis(basis As AktGfxBasis)
        _bmpCache.Clear()
        _basis = basis
        Dim fileName As String = $"Basis_{basis}.gfx.xml"
        Dim full As String = Path.Combine(_root, fileName)
        If Not File.Exists(full) Then Throw New FileNotFoundException("GfxCompiler-Basis-XML fehlt.", full)

        Dim ser As New XmlSerializer(GetType(GfxPack))
        Using fs As FileStream = File.OpenRead(full)
            _pack = DirectCast(ser.Deserialize(fs), GfxPack)
        End Using
        If _pack Is Nothing OrElse _pack.Entries Is Nothing Then
            Throw New InvalidDataException("GfxPack leer/ungültig.")
        End If
    End Sub

    ''' <summary>
    ''' Liefert Bitmap für einen Logiknamen.
    ''' name kann "…##q" enthalten. Bei ##q wird dpiFaktor beachtet.
    ''' sizeW/H: wenn 0 → Default 100; wenn nur eine Seite 0 → 1:1.
    ''' SVG hat Vorrang vor PNG.
    ''' </summary>
    Public Function GetBitmap(name As String, sizeW As Integer, sizeH As Integer, Optional dpiFaktor As Single = 1.0F) As Bitmap
        If _pack Is Nothing Then Throw New InvalidOperationException("ImageContainer.LoadBasis(...) zuerst aufrufen.")

        Dim q As Integer? = Nothing
        Dim baseKey As String = StripSizeQ(name, q)

        ' Größe ermitteln
        Dim w As Integer = sizeW
        Dim h As Integer = sizeH
        If w <= 0 AndAlso h <= 0 Then
            w = 100 : h = 100
        ElseIf w <= 0 Xor h <= 0 Then
            ' 1:1 falls Verhältnis unbekannt
            If w <= 0 Then w = h Else h = w
        End If

        ' Bei ##q darf dpiFaktor wirken
        If q.HasValue AndAlso dpiFaktor > 0.0001F AndAlso Math.Abs(dpiFaktor - 1.0F) > 0.0001F Then
            w = CInt(Math.Round(w * dpiFaktor))
            h = CInt(Math.Round(h * dpiFaktor))
        End If

        Dim cacheKey As String = $"{baseKey}|{w}x{h}|dm={(If(_isDark, 1, 0))}|dpi={dpiFaktor.ToString(System.Globalization.CultureInfo.InvariantCulture)}"
        Dim bmp As Bitmap = Nothing
        If _bmpCache.TryGetValue(cacheKey, bmp) Then Return bmp

        ' 1) Versuche SVG mit passender/nahe Größe (bei mehrfachen Einträgen)
        Dim svgEntry As GfxEntry = BestEntry(_pack.Entries, baseKey, preferSvg:=True)
        If svgEntry IsNot Nothing AndAlso svgEntry.Kind = "svg" Then
            bmp = RenderSvgEntry(svgEntry, w, h)
        End If

        ' 2) Sonst PNG/BMP
        If bmp Is Nothing Then
            Dim pngEntry As GfxEntry = BestEntry(_pack.Entries, baseKey, preferSvg:=False)
            If pngEntry Is Nothing Then Throw New KeyNotFoundException($"Grafik '{baseKey}' nicht gefunden.")
            Dim raw As Byte() = InflateGZip(pngEntry.DataGzBase64)
            Using ms As New MemoryStream(raw)
                Using tmp As Bitmap = DirectCast(Image.FromStream(ms), Bitmap)
                    bmp = ResizeBitmap(tmp, w, h)
                End Using
            End Using
        End If

        ' Dark-Anpassung, falls gewünscht
        If _isDark AndAlso _brighten > 0 Then
            Dim b2 As Bitmap = MaybeBrighten(bmp, _brighten)
            If Not Object.ReferenceEquals(b2, bmp) Then bmp = b2
        End If

        _bmpCache(cacheKey) = bmp
        Return bmp
    End Function

    Private Function BestEntry(entries As IEnumerable(Of GfxEntry), baseKey As String, preferSvg As Boolean) As GfxEntry
        ' Suche erst strikt nach baseKey, bevorzugt SVG
        Dim exact As List(Of GfxEntry) = entries.Where(Function(e) String.Equals(e.Key, baseKey, StringComparison.OrdinalIgnoreCase)).ToList()
        If exact.Count > 0 Then
            Dim p As GfxEntry = If(preferSvg, exact.FirstOrDefault(Function(e) e.Kind = "svg"), exact.FirstOrDefault(Function(e) e.Kind <> "svg"))
            If p Is Nothing Then p = exact.First()
            Return p
        End If

        ' Optional: lockere Suche (falls im Pack z.B. nur Größenvarianten mit Key==baseKey existieren sollten)
        ' Hier nicht nötig, weil wir Key bereits ohne ##q speichern.
        Return Nothing
    End Function

    Private Function RenderSvgEntry(entry As GfxEntry, w As Integer, h As Integer) As Bitmap
        Dim raw As Byte() = InflateGZip(entry.DataGzBase64)
        Dim svgText As String = System.Text.Encoding.UTF8.GetString(raw)

        Dim doc As Svg.SvgDocument
        Using sr As New StringReader(svgText)
            Dim xrSettings As New XmlReaderSettings() With {
            .DtdProcessing = DtdProcessing.Ignore
        }
            Using xr As XmlReader = XmlReader.Create(sr, xrSettings)
                doc = SvgDocument.Open(Of SvgDocument)(xr)
            End Using
        End Using

        Dim bmp As New Bitmap(w, h, Imaging.PixelFormat.Format32bppPArgb)
        Using g As Graphics = Graphics.FromImage(bmp)
            g.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
            doc.Draw(g, New SizeF(w, h))
        End Using
        Return bmp
    End Function


End Class

