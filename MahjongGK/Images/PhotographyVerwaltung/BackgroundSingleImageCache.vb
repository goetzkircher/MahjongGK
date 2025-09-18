
' ────────────────────────────────────────────────────────────────────────────────
'  BackgroundSingleImageCache (stabile Variante)
'  • Keine Size.Empty-Aufrufe an den Cache beim normalen Rendern
'  • Guard gegen {Width<=0, Height<=0}
'  • InsideRect ("Cover + Crop") erzeugt stets exakt Zielgröße
'  • Hot-Cache vermeidet unnötige Rebuilds
' ────────────────────────────────────────────────────────────────────────────────

Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

#Disable Warning IDE0079
#Disable Warning IDE1006

Imports System.Drawing.Drawing2D
Imports System.Drawing.Imaging
Imports System.IO

Public Enum BackgroundImageRenderMode
    ''' <summary>
    ''' None wird als Stretch verarbeitet, wenn damit LoadBitmap aufgerufen wird.
    ''' </summary>
    None                ' nichts ausgewählt 
    FitInside           ' Proportionen erhalten, Letterbox
    Stretch             ' Verzerren erlaubt
    CoverCrop           ' Cover + zentrierter Zuschnitt
    PreserveOrgSize     ' Nur verkleinern, zentriert platzieren
End Enum


''' <summary>
''' Host/Adapter für genau ein Hintergrundbild auf Basis eines gemeinsamen <see cref="BackgroundImageCache"/>.
''' 
''' Modi (exklusiv):
''' 1) FitInside (Default): Proportionen erhalten, Letterbox (Untergrundfarbe aus Cache).
''' 2) Stretch: Verzerren erlaubt, Zielverhältnis wird erzwungen.
''' 3) CoverCrop: "Cover + zentrierter Crop".
''' 4) PreserveOrgSize: Nur verkleinern (nie vergrößern), zentriert auf Zielgröße (transparenter Hintergrund).
''' 
''' WICHTIG: Cache-verwaltete Bitmaps NICHT entsorgen. Host-eigene Bitmaps (CoverCrop/PreserveOrgSize) werden intern verwaltet.
''' </summary>
Public NotInheritable Class BackgroundSingleImageCache

    ' Hinweise / Integration

    'FitInside und Stretch rufen beide den Cache direkt auf:

    'GetFromCache(size, preserve:=True) ⇒ FitInside (Letterbox)

    'GetFromCache(size, preserve:=False) ⇒ Stretch (verzerren)

    'CoverCrop & PreserveOrgSize erzeugen Host-eigene Bitmaps (setzen _ownsLast=True),
    'damit du sie sauber disposen kannst, wenn die Größe/der Modus wechselt.

    'Falls dein BackgroundImageCache.LoadImage(...) kein backgroundColor-Parameter hat,
    'ersetze die beiden Stellen In CoverCrop/PreserveOrgSize einfach durch die dreiparametrige
    'Variante ohne Hintergrund (oder nutze Color.Transparent nur, wenn die Überladung vorhanden ist).

#Region "Felder & Status"

    Private ReadOnly _cache As BackgroundImageCache

    Private _fullPath As String = Nothing
    Private _hasBitmap As Boolean = False
    Private _orgSize As Size = Size.Empty

    ' Hot-Cache der letzten Lieferung
    Private _lastSize As Size = Size.Empty
    Private _lastBitmap As Bitmap = Nothing
    Private _ownsLast As Boolean = False


    Private _mode As BackgroundImageRenderMode = BackgroundImageRenderMode.FitInside

#End Region

#Region "Konstruktion"

    Public Sub New()
        ' leer (Designer/Fabrik)
    End Sub

    Public Sub New(cache As BackgroundImageCache)
        If cache Is Nothing Then Throw New ArgumentNullException(NameOf(cache))
        _cache = cache
    End Sub

#End Region

#Region "Interne Hilfen"

    Private Sub ResetLast()
        If _ownsLast AndAlso _lastBitmap IsNot Nothing Then
            _lastBitmap.Dispose()
        End If
        _lastBitmap = Nothing
        _lastSize = Size.Empty
        _ownsLast = False
    End Sub

    ''' <summary>
    ''' Liefert eine Bitmap exakt in <paramref name="target"/> aus dem Cache.
    ''' preserve:=True → FitInside/Letterbox; preserve:=False → Stretch (verzerren).
    ''' Hintergrundfarbe bestimmt der Cache (z. B. Dominant-Color).
    ''' </summary>
    Private Function GetFromCache(target As Size, preserve As Boolean) As Bitmap
        Return _cache.LoadImage(_fullPath, target, preserve)
    End Function

#End Region

#Region "Öffentliche API"

    ''' <summary>
    ''' Lädt (registriert) ein Bild und setzt den exklusiven Render-Modus.
    ''' </summary>
    Public Sub LoadBitmap(fullpath As String, mode As BackgroundImageRenderMode)
        ' Pfad prüfen / zurücksetzen
        If String.IsNullOrWhiteSpace(fullpath) OrElse Not File.Exists(fullpath) Then
            If Not String.IsNullOrEmpty(_fullPath) Then _cache.Invalidate(_fullPath)
            _fullPath = Nothing : _hasBitmap = False : _orgSize = Size.Empty
            ResetLast()
            Return
        End If

        _fullPath = fullpath
        _mode = If(mode = BackgroundImageRenderMode.None, BackgroundImageRenderMode.Stretch, mode)

        ' Originalgröße einmalig ermitteln
        Using probe As Bitmap = _cache.LoadImageClone(_fullPath)
            _orgSize = If(probe IsNot Nothing, probe.Size, Size.Empty)
        End Using

        ResetLast()
        _hasBitmap = (_orgSize.Width > 0 AndAlso _orgSize.Height > 0)
    End Sub

    ''' <summary>
    ''' Liefert eine Bitmap mit exakt der angeforderten Größe entsprechend aktuellem Modus.
    ''' </summary>
    Public Function GetBitmap(size As Size) As Bitmap
        If Not _hasBitmap Then Return Nothing

        If size.Width <= 0 OrElse size.Height <= 0 Then
            If _lastBitmap IsNot Nothing Then Return _lastBitmap
            Return Nothing
        End If

        Dim effectiveSize As Size = size

        ' Hot-Cache-Hit?
        If _lastBitmap IsNot Nothing AndAlso _lastSize.Equals(effectiveSize) Then
            Return _lastBitmap
        End If

        ' Modus-/Größenwechsel → evtl. Host-Bitmap entsorgen
        If _lastBitmap IsNot Nothing AndAlso Not _lastSize.Equals(effectiveSize) Then
            ResetLast()
        End If

        Select Case _mode

            Case BackgroundImageRenderMode.FitInside
                ' Proportionen erhalten, Letterbox vom Cache
                _lastBitmap = GetFromCache(effectiveSize, preserve:=True)
                _lastSize = effectiveSize
                _ownsLast = False
                Return _lastBitmap

            Case BackgroundImageRenderMode.Stretch
                ' Verzerren erlaubt, exakt Zielverhältnis
                _lastBitmap = GetFromCache(effectiveSize, preserve:=False)
                _lastSize = effectiveSize
                _ownsLast = False
                Return _lastBitmap

            Case BackgroundImageRenderMode.CoverCrop
                ' Cover + zentrierter Crop auf Zielgröße
                Dim scaleW As Double = CDbl(effectiveSize.Width) / Math.Max(1, _orgSize.Width)
                Dim scaleH As Double = CDbl(effectiveSize.Height) / Math.Max(1, _orgSize.Height)
                Dim scale As Double = Math.Max(scaleW, scaleH)
                Dim coverW As Integer = Math.Max(1, CInt(Math.Ceiling(_orgSize.Width * scale)))
                Dim coverH As Integer = Math.Max(1, CInt(Math.Ceiling(_orgSize.Height * scale)))

                Dim src As Bitmap = _cache.LoadImage(_fullPath,
                                                     New Size(coverW, coverH),
                                                     preserveOrgProportion:=False,
                                                     backgroundColor:=Color.Transparent)
                If src Is Nothing Then Return Nothing

                Dim cropX As Integer = Math.Max(0, (src.Width - effectiveSize.Width) \ 2)
                Dim cropY As Integer = Math.Max(0, (src.Height - effectiveSize.Height) \ 2)

                Dim result As New Bitmap(effectiveSize.Width, effectiveSize.Height, PixelFormat.Format32bppArgb)
                Using g As Graphics = Graphics.FromImage(result)
                    g.CompositingMode = CompositingMode.SourceOver
                    g.CompositingQuality = CompositingQuality.HighQuality
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality
                    g.SmoothingMode = SmoothingMode.HighQuality

                    g.DrawImage(src,
                                New Rectangle(0, 0, effectiveSize.Width, effectiveSize.Height),
                                New Rectangle(cropX, cropY, effectiveSize.Width, effectiveSize.Height),
                                GraphicsUnit.Pixel)
                End Using

                _lastBitmap = result
                _lastSize = effectiveSize
                _ownsLast = True
                Return _lastBitmap

            Case BackgroundImageRenderMode.PreserveOrgSize
                ' Nur verkleinern, zentriert auf transparente Zielbitmap
                Dim fitW As Double = CDbl(effectiveSize.Width) / Math.Max(1, _orgSize.Width)
                Dim fitH As Double = CDbl(effectiveSize.Height) / Math.Max(1, _orgSize.Height)
                Dim scale As Double = Math.Min(1.0R, Math.Min(fitW, fitH)) ' nie > 1.0
                Dim drawW As Integer = Math.Max(1, CInt(Math.Floor(_orgSize.Width * scale)))
                Dim drawH As Integer = Math.Max(1, CInt(Math.Floor(_orgSize.Height * scale)))

                Dim content As Bitmap = _cache.LoadImage(_fullPath,
                                                         New Size(drawW, drawH),
                                                         preserveOrgProportion:=False,
                                                         backgroundColor:=Color.Transparent)
                If content Is Nothing Then Return Nothing

                Dim result As New Bitmap(effectiveSize.Width, effectiveSize.Height, PixelFormat.Format32bppArgb)
                Using g As Graphics = Graphics.FromImage(result)
                    g.CompositingMode = CompositingMode.SourceOver
                    g.CompositingQuality = CompositingQuality.HighQuality
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality
                    g.SmoothingMode = SmoothingMode.HighQuality

                    g.Clear(Color.Transparent)

                    Dim dx As Integer = (effectiveSize.Width - drawW) \ 2
                    Dim dy As Integer = (effectiveSize.Height - drawH) \ 2

                    g.DrawImage(content,
                                New Rectangle(dx, dy, drawW, drawH),
                                New Rectangle(0, 0, drawW, drawH),
                                GraphicsUnit.Pixel)
                End Using

                _lastBitmap = result
                _lastSize = effectiveSize
                _ownsLast = True
                Return _lastBitmap

            Case Else
                Return Nothing
        End Select
    End Function

    Public Function GetBitmap(width As Integer, height As Integer) As Bitmap
        Return GetBitmap(New Size(Math.Max(0, width), Math.Max(0, height)))
    End Function

    Public Function GetAktSize(Optional size As Size = Nothing) As Size
        If Not _hasBitmap Then Return Size.Empty
        If _lastBitmap IsNot Nothing Then Return _lastSize
        If Not size.IsEmpty Then
            Dim dummy As Bitmap = GetBitmap(size)
            Return If(_lastBitmap IsNot Nothing, _lastSize, Size.Empty)
        End If
        Return Size.Empty
    End Function

    Public Sub Invalidate()
        If Not String.IsNullOrEmpty(_fullPath) Then
            _cache.Invalidate(_fullPath)
        End If
        _fullPath = Nothing
        _hasBitmap = False
        _orgSize = Size.Empty
        ResetLast()
    End Sub

    Public Function HasBitmap() As Boolean
        Return _hasBitmap
    End Function

#End Region

End Class
