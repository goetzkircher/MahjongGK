Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On
Imports System.Drawing.Drawing2D
Imports System.IO

Namespace MahjongGKSymbolFactory

    '
    ''' <summary>
    ''' Bild-Export: Supersampling, Downscale und Batch-Save.
    ''' </summary>
    Public NotInheritable Class TileExport
        Private Sub New()
        End Sub

        '
        ''' <summary>
        ''' Rendert alle 42 Steine mit Symbolen und speichert sie als PNGs in einen Zielordner.
        ''' Nutzt Supersampling (scaleFactor), um gestochen scharfe Ergebnisse zu bekommen.
        ''' </summary>
        ''' <param name="targetFolder">Zielordner (wird angelegt).</param>
        ''' <param name="baseSize">Zielgröße pro Stein (z. B. 198x252).</param>
        ''' <param name="preset">Körperstil (Klassisch/Modern/StarkTexturiert).</param>
        ''' <param name="superSample">Renderfaktor (z. B. 2 für 2×).</param>
        Public Shared Sub SaveAllTilesAsPng(targetFolder As String,
                                            baseSize As Size,
                                            preset As TilePreset,
                                            Optional superSample As Integer = 2)

            If Not Directory.Exists(targetFolder) Then
                Directory.CreateDirectory(targetFolder)
            End If

            ' Symbole in Super-Sample-Größe generieren
            Dim hiSize As New Size(baseSize.Width * superSample, baseSize.Height * superSample)
            Dim symbols As List(Of Bitmap) = MahjongSymbolFactory.GenerateAllSymbols(hiSize)

            ' Steine rendern
            Dim tiles As List(Of Bitmap) =
                SetRenderer.RenderSetWithSeries(symbols, hiSize, preset)

            ' Downscale & speichern
            For i As Integer = 0 To tiles.Count - 1
                Dim si As SteinIndexEnum = CType(i, SteinIndexEnum)
                Dim hiBmp As Bitmap = tiles(i)
                Using loBmp As Bitmap = DownscaleHighQuality(hiBmp, baseSize)
                    Dim filename As String = Path.Combine(targetFolder, $"{si}.png")
                    loBmp.Save(filename, Imaging.ImageFormat.Png)
                End Using
                hiBmp.Dispose()
                symbols(i).Dispose()
            Next
        End Sub

        '
        ''' <summary>
        ''' Exportiert drei komplette Sätze in Unterordnern „Classic“, „Modern“, „Textured“.
        ''' </summary>
        Public Shared Sub SaveAllThreeVariants(baseSize As Size,
                                               Optional superSample As Integer = 2)
            Dim rootFolder As String = INI.AppDataDirectory()
            Dim classicDir As String = Path.Combine(rootFolder, "Classic")
            Dim modernDir As String = Path.Combine(rootFolder, "Modern")
            Dim texturedDir As String = Path.Combine(rootFolder, "Textured")

            SaveAllTilesAsPng(classicDir, baseSize, TilePreset.Klassisch, superSample)
            SaveAllTilesAsPng(modernDir, baseSize, TilePreset.ModernGlatt, superSample)
            SaveAllTilesAsPng(texturedDir, baseSize, TilePreset.StarkTexturiert, superSample)

        End Sub

        '
        ''' <summary>
        ''' Hochwertiges Herunterskalieren (Lanczos-ähnlich via HighQualityBicubic + korrekte Sampling-Einstellungen).
        ''' </summary>
        Private Shared Function DownscaleHighQuality(src As Bitmap, targetSize As Size) As Bitmap
            Dim dst As New Bitmap(targetSize.Width, targetSize.Height, Imaging.PixelFormat.Format32bppPArgb)
            Using g As Graphics = Graphics.FromImage(dst)
                g.SmoothingMode = SmoothingMode.HighQuality
                g.InterpolationMode = InterpolationMode.HighQualityBicubic
                g.PixelOffsetMode = PixelOffsetMode.HighQuality
                g.CompositingQuality = CompositingQuality.HighQuality
                g.DrawImage(src, New Rectangle(Point.Empty, targetSize),
                            New Rectangle(0, 0, src.Width, src.Height), GraphicsUnit.Pixel)
            End Using
            Return dst
        End Function

    End Class

End Namespace

