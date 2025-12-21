
Imports System.Drawing.Drawing2D
Imports System.Drawing.Text

Namespace MjGDI
    Module GfxHelfer
        ''' <summary>
        ''' Wendet hochwertige Render-Settings auf ein vorhandenes Graphics an (z. B. im Paint-Event).
        ''' </summary>
        Public Sub GfxConfigureHighQuality(g As Graphics)
            g.SmoothingMode = SmoothingMode.AntiAlias
            g.InterpolationMode = InterpolationMode.HighQualityBicubic
            g.PixelOffsetMode = PixelOffsetMode.HighQuality
            g.CompositingQuality = CompositingQuality.HighQuality
            g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit
        End Sub


        ''' <summary>
        ''' Live-Resize: schnell und glatt genug. Stellt alle relevanten Modi.
        ''' </summary>
        Public Sub GfxConfigureForLiveResize(g As Graphics, src As Size, dest As Size)
            g.SmoothingMode = SmoothingMode.None
            g.CompositingQuality = CompositingQuality.HighSpeed
            g.PixelOffsetMode = PixelOffsetMode.Default
            g.InterpolationMode = GfxPickInterpolation(src, dest)
        End Sub

        ''' <summary>
        ''' Finaler 1:1- oder seltener Skalier-Blit (wenn du nach ResizeEnd
        ''' zur Sicherheit doch noch skalieren musst). Bevorzugt 1:1 „billig“.
        ''' </summary>
        Public Sub GfxConfigureForFinalBlit(g As Graphics, isExact1To1 As Boolean, src As Size, dest As Size)
            If isExact1To1 Then
                g.SmoothingMode = SmoothingMode.None
                g.CompositingQuality = CompositingQuality.HighSpeed
                g.PixelOffsetMode = PixelOffsetMode.Default
                g.InterpolationMode = InterpolationMode.NearestNeighbor
            Else
                ' Fallback, falls du wider Erwarten skalieren musst
                g.SmoothingMode = SmoothingMode.None
                g.CompositingQuality = CompositingQuality.HighSpeed
                g.PixelOffsetMode = PixelOffsetMode.Default
                g.InterpolationMode = GfxPickInterpolation(src, dest)
            End If
        End Sub

        ''' <summary>
        ''' Wählt einen praxistauglichen InterpolationMode anhand des Skalierungsfaktors.
        ''' Ziel: schnell beim Live-Resize, ausreichend glatt beim Downscale.
        ''' </summary>
        Public Function GfxPickInterpolation(src As Size, dest As Size) As InterpolationMode
            If src.Width <= 0 OrElse src.Height <= 0 OrElse dest.Width <= 0 OrElse dest.Height <= 0 Then
                Return InterpolationMode.NearestNeighbor
            End If

            Dim sx As Double = dest.Width / CDbl(src.Width)
            Dim sy As Double = dest.Height / CDbl(src.Height)
            Dim s As Double = Math.Min(sx, sy) ' konservativ

            If s >= 1.0R Then
                ' Upscale
                If s <= 1.25R Then
                    ' Sehr leichtes Upscale: Bilinear ist ok
                    Return InterpolationMode.Bilinear
                Else
                    ' Deutliches Upscale: Nearest ist am schnellsten (knackig), Bilinear weicher aber teurer
                    Return InterpolationMode.NearestNeighbor
                End If
            Else
                ' Downscale
                If s >= 0.75R Then
                    ' Leichtes Downscale: Bilinear reicht, schnell
                    Return InterpolationMode.Bilinear
                ElseIf s >= 0.4R Then
                    ' Mittleres Downscale: Bicubic ist sichtbar besser
                    Return InterpolationMode.Bicubic
                Else
                    ' Starkes Downscale: HighQualityBicubic bringt klaren Vorteil
                    Return InterpolationMode.HighQualityBicubic
                End If
            End If
        End Function


        ' internes Feld für den "global" gesicherten State
        Private _savedState As GraphicsState
        Private _hasState As Boolean

        ''' <summary>
        ''' Sichert den kompletten Zustand des Graphics und gibt ein Token zurück.
        ''' </summary>
        Public Function GfxPush(g As Graphics) As GraphicsState
            If g Is Nothing Then Throw New ArgumentNullException(NameOf(g))
            Return g.Save()
        End Function

        ''' <summary>
        ''' Stellt den zuvor mit GfxPush gesicherten Zustand wieder her.
        ''' </summary>
        Public Sub GfxPop(g As Graphics, state As GraphicsState)
            If g Is Nothing Then Throw New ArgumentNullException(NameOf(g))
            g.Restore(state)
        End Sub

        ''' <summary>
        ''' Sichert den Zustand im Modul-internen Speicher.
        ''' Achtung: nur ein State gleichzeitig.
        ''' </summary>
        Public Sub GfxPushStored(g As Graphics)
            If g Is Nothing Then Throw New ArgumentNullException(NameOf(g))
            _savedState = g.Save()
            _hasState = True
        End Sub

        ''' <summary>
        ''' Stellt den im Modul gespeicherten Zustand wieder her und löscht ihn.
        ''' </summary>
        Public Sub GfxPopStored(g As Graphics)
            If g Is Nothing Then Throw New InvalidOperationException("Kein gespeicherter State vorhanden.")
            g.Restore(_savedState)
            _hasState = False
        End Sub

        ''' <summary>
        ''' Prüfen, ob ein gespeicherter State vorhanden ist.
        ''' </summary>
        Public ReadOnly Property HasStoredState As Boolean
            Get
                Return _hasState
            End Get
        End Property

    End Module
End Namespace
