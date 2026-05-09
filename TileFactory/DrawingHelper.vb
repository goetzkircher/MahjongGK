Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

Imports System.Drawing
Imports System.Drawing.Drawing2D

'Namespace TileFactory Ist im StammNameSpace definiert

'
Friend Module DrawingHelper

    ''' <summary>
    ''' Stellt die Graphics-Instanz auf definierte Qualität ein.
    ''' 
    ''' sourceOver=True:
    '''   Alpha-Überlagerung. Transparente Pixel bleiben transparent.
    ''' 
    ''' sourceOver=False:
    '''   Deckendes Kopieren. Auch transparente Quellpixel überschreiben Zielpixel.
    ''' 
    ''' fastQuality=True:
    '''   Schnellere, einfachere Render-Einstellungen.
    ''' 
    ''' fastQuality=False:
    '''   Höhere Qualität für Linien, Bitmaps und Text.
    ''' </summary>
    Public Sub SetGraphicsMode(g As Graphics,
                               sourceOver As Boolean,
                               Optional fastQuality As Boolean = False)

        If g Is Nothing Then
            Throw New ArgumentNullException(NameOf(g))
        End If

        'Immer in Pixeln arbeiten.
        g.PageUnit = GraphicsUnit.Pixel
        g.PageScale = 1.0F

        If fastQuality Then
            'Schneller, ohne Kantenglättung.
            g.SmoothingMode = SmoothingMode.None

            'Schnellere Bitmap-Skalierung.
            g.InterpolationMode = InterpolationMode.Low

            'Einfache Pixelplatzierung.
            g.PixelOffsetMode = PixelOffsetMode.None

            'Schnelleres Alpha-Compositing.
            g.CompositingQuality = CompositingQuality.HighSpeed

            'Schnellere, einfache Textdarstellung.
            g.TextRenderingHint = Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit
        Else
            'Glättet Linien, Kurven und Pfadkanten.
            g.SmoothingMode = SmoothingMode.AntiAlias

            'Hohe Qualität beim Skalieren von Bitmaps.
            g.InterpolationMode = InterpolationMode.HighQualityBicubic

            'Optimiert die Pixelpositionierung.
            g.PixelOffsetMode = PixelOffsetMode.HighQuality

            'Hohe Qualität beim Mischen halbtransparenter Pixel.
            g.CompositingQuality = CompositingQuality.HighQuality

            'Saubere Glyphen/Textausgabe und Schutz vor problematischen Systemdefaults.
            g.TextRenderingHint = Drawing.Text.TextRenderingHint.AntiAliasGridFit
        End If

        If sourceOver Then
            'Quelle wird unter Beachtung des Alphakanals über das Ziel gezeichnet.
            g.CompositingMode = CompositingMode.SourceOver
        Else
            'Quelle wird direkt und deckend in das Ziel kopiert.
            g.CompositingMode = CompositingMode.SourceCopy
        End If

    End Sub
End Module
