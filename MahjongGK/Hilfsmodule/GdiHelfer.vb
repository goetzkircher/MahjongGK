
Imports System.Drawing.Drawing2D
Imports System.Drawing.Imaging
Imports System.IO
Imports System.Runtime.InteropServices
Imports ImageMagick

Namespace MjGDI
    Public Module GdiHelfer

        ''' <summary>
        ''' Erzeugt eine neue aufgehellte Bitmap-Kopie (additiver Offset auf RGB).
        ''' amount: wird aus der INI bezogen. Original bleibt unverändert.
        ''' </summary>
        Public Function BrightenImage(src As Image, Optional deepCopy As Boolean = False) As Image
            Return BrightenImage(src, INI.Global_BrightenAmount, deepCopy)
        End Function
        ''' <summary>
        ''' Erzeugt eine neue aufgehellte Bitmap-Kopie (additiver Offset auf RGB).
        ''' amount: 0.0 … 1.0  (Praxis: 0.05 … 0.30). Original bleibt unverändert.
        ''' </summary>
        Public Function BrightenImage(src As Image, amount As Single, Optional deepCopy As Boolean = False) As Image
            If src Is Nothing Then Return Nothing

            If amount <= 0.0001F Then
                ' Keine Änderung gewünscht → saubere Kopie zurückgeben
                Return New Bitmap(src)
            End If
            If amount > 1.0F Then amount = 1.0F

            Dim dst As Image
            Dim w As Integer = src.Width
            Dim h As Integer = src.Height

            If deepCopy Then
                dst = New Bitmap(w, h, PixelFormat.Format32bppArgb)
            Else
                dst = src
            End If

            Using g As Graphics = Graphics.FromImage(dst)
                Using ia As New ImageAttributes()
                    ' ColorMatrix mit additivem Offset (amount) auf R, G, B
                    Dim cm As New ColorMatrix(New Single()() {
                    New Single() {1.0F, 0.0F, 0.0F, 0.0F, 0.0F},
                    New Single() {0.0F, 1.0F, 0.0F, 0.0F, 0.0F},
                    New Single() {0.0F, 0.0F, 1.0F, 0.0F, 0.0F},
                    New Single() {0.0F, 0.0F, 0.0F, 1.0F, 0.0F},
                    New Single() {amount, amount, amount, 0.0F, 1.0F}
                })
                    ia.SetColorMatrix(cm)
                    g.DrawImage(src, New Rectangle(0, 0, w, h), 0, 0, w, h, GraphicsUnit.Pixel, ia)
                End Using
            End Using

            Return dst
        End Function

        '──────────────────────────────────────────────────────────────────────────────
        ' 1) Nicht-destruktiv: neue Bitmap mit KONSTANTEM Alpha erzeugen (schnell)
        '    Setzt den Ausgabekanal Alpha = alpha (0..255), unabhängig vom Quell-Alpha.
        '    Hinweis: Das Originalbild bleibt unverändert.
        '──────────────────────────────────────────────────────────────────────────────
        Public Function WithConstantAlpha(src As Bitmap, alpha As Byte, Optional disposeSrc As Boolean = False) As Bitmap
            If src Is Nothing Then Throw New ArgumentNullException(NameOf(src))
            Dim rect As New Rectangle(0, 0, src.Width, src.Height)

            Dim dst As New Bitmap(src.Width, src.Height, PixelFormat.Format32bppArgb)
            Using g As Graphics = Graphics.FromImage(dst)
                Dim a As Single = CSng(alpha) / 255.0F

                ' ColorMatrix: Alpha = 0 * oldA + a  →  konstanter Alpha
                Dim cm As New ColorMatrix(New Single()() {
                    New Single() {1, 0, 0, 0, 0},
                    New Single() {0, 1, 0, 0, 0},
                    New Single() {0, 0, 1, 0, 0},
                    New Single() {0, 0, 0, 0, 0},   ' kein Beitrag von oldA
                    New Single() {0, 0, 0, a, 1}    ' + a als Offset → konstanter Alpha
                })

                Using ia As New ImageAttributes()
                    ia.SetColorMatrix(cm, ColorMatrixFlag.Default, ColorAdjustType.Bitmap)
                    g.DrawImage(src, rect, 0, 0, src.Width, src.Height, GraphicsUnit.Pixel, ia)
                End Using
            End Using

            If disposeSrc Then src.Dispose()

            Return dst

        End Function

        '──────────────────────────────────────────────────────────────────────────────
        ' 2) Destruktiv/In-place: alle Alpha-Bytes direkt auf einen Wert setzen
        '    Achtung: Mutiert die übergebene Bitmap. Arbeitet in 32bppArgb.
        '──────────────────────────────────────────────────────────────────────────────
        Public Sub SetAlphaInPlace(ByRef bmp As Bitmap, alpha As Byte)
            If bmp Is Nothing Then Throw New ArgumentNullException(NameOf(bmp))

            ' Bei falschem Format auf 32bppArgb klonen und Referenz beim Aufrufer austauschen
            If bmp.PixelFormat <> PixelFormat.Format32bppArgb Then
                Dim clone As New Bitmap(bmp.Width, bmp.Height, PixelFormat.Format32bppArgb)
                Using g As Graphics = Graphics.FromImage(clone)
                    g.DrawImage(bmp, 0, 0, bmp.Width, bmp.Height)
                End Using
                bmp.Dispose()
                bmp = clone
            End If

            Dim rect As New Rectangle(0, 0, bmp.Width, bmp.Height)
            Dim data As BitmapData = bmp.LockBits(rect, ImageLockMode.ReadWrite, bmp.PixelFormat)
            Try
                Dim stride As Integer = data.Stride
                Dim w As Integer = bmp.Width
                Dim h As Integer = bmp.Height
                Dim bytes As Integer = Math.Abs(stride) * h
                Dim buf(bytes - 1) As Byte

                Marshal.Copy(data.Scan0, buf, 0, bytes)

                ' 32bppArgb: BGRA → Alpha liegt an jedem 4. Byte, Offset 3
                Dim y As Integer, x As Integer, idx As Integer
                For y = 0 To h - 1
                    idx = y * stride + 3
                    For x = 0 To w - 1
                        buf(idx) = alpha
                        idx += 4
                    Next
                Next

                Marshal.Copy(buf, 0, data.Scan0, bytes)
            Finally
                bmp.UnlockBits(data)
            End Try
        End Sub
        Public Function WithConstantAlpha(src As Bitmap, alpha As Byte) As Bitmap
            If src Is Nothing Then Throw New ArgumentNullException(NameOf(src))
            Dim dst As New Bitmap(src.Width, src.Height, PixelFormat.Format32bppArgb)
            Using g As Graphics = Graphics.FromImage(dst)
                Dim a As Single = alpha / 255.0F
                Dim cm As New ColorMatrix(New Single()() {
                    New Single() {1, 0, 0, 0, 0},
                    New Single() {0, 1, 0, 0, 0},
                    New Single() {0, 0, 1, 0, 0},
                    New Single() {0, 0, 0, 0, 0},   ' oldA ignorieren
                    New Single() {0, 0, 0, a, 1}    ' Alpha = a (konstant)
                })
                Using ia As New ImageAttributes()
                    ia.SetColorMatrix(cm)
                    g.DrawImage(src, New Rectangle(0, 0, src.Width, src.Height), 0, 0, src.Width, src.Height, GraphicsUnit.Pixel, ia)
                End Using
            End Using
            Return dst
        End Function

        ' ============================================================================
        '  Utils.ImageTools
        '  ───────────────────────────────────────────────────────────────────────────
        '  Sammlung von Bildbearbeitungs-Funktionen für System.Drawing.Bitmap.
        '
        '  ENTHALTENE FUNKTIONEN:
        '  • Brighten(ByRef bmp, amount, Optional deepCopy:=False)
        '       → Helligkeit erhöhen (additiver Offset auf RGB).
        '         deepCopy=False: in-place; True: neue Bitmap + alte entsorgt.
        '
        '  • AdjustGamma(ByRef bmp, gamma, Optional deepCopy:=False)
        '       → Gamma-Korrektur (0.5 … 2.2 üblich, 1.0 = neutral).
        '
        '  • AdjustContrast(ByRef bmp, contrast, Optional deepCopy:=False)
        '       → Kontrast anpassen (-1.0 … +1.0, 0 = neutral).
        '
        '  • Sharpen(ByRef bmp, Optional strength:=1.0F, Optional deepCopy:=False)
        '       → Bitmap schärfen (3x3-Kernel, Stärke 0.0 … 2.0).
        '
        '  • ShrinkBitmap(ByRef bmp, newWidth, newHeight,
        '                 Optional highQuality:=True,
        '                 Optional disposeSrc:=False)
        '       → Bitmap skalieren/verkleinern:
        '         - Eine Dimension darf 0 sein, wird proportional berechnet.
        '         - TileFlipXY als Standard → saubere Ränder bei Bicubic.
        '         - disposeSrc=True → Quell-Bitmap wird entsorgt.
        '
        '  • SavePng(bmp, filePath)
        '       → Speichert immer als PNG.
        '
        '  • LoadBitmap(filePath) As Bitmap
        '       → Lädt Bild als neue 32bppARGB-Bitmap (keine Dateisperre).
        '
        '  INTERN:
        '  • PixelOp-Delegate für ByRef-Pixeloperationen.
        '  • ApplyPerPixelInPlace, ApplyLutInPlace/ToNew → Low-Level Helfer.
        '  • ConvolveToNew → generischer 3x3-Kernel (z. B. für Sharpen).
        '  • ComputeTargetSize → berechnet fehlende Dimension proportional.
        '
        '  HINWEISE:
        '  • Immer Option Strict/Explicit aktiv (wie im Modul gesetzt).
        '  • Für große Downscales ggf. stufenweise arbeiten + Sharpen.
        '  • Alle Methoden sind GC-sicher; Dispose-Optionen für sofortiges
        '    Freigeben von Bitmaps vorhanden.
        ' ============================================================================


        '────────────────────────────────────────────────────────────────────────
        ' Delegate erlaubt ByRef-Ausgaben für per-Pixel-Operationen
        '────────────────────────────────────────────────────────────────────────
        Private Delegate Sub PixelOp(
                    b As Integer, g As Integer, r As Integer, a As Integer,
                    ByRef nb As Byte, ByRef ng As Byte, ByRef nr As Byte, ByRef na As Byte)

        '────────────────────────────────────────────────────────────────────────
        ' Hilfsfunktionen: Formate & Utilities
        '────────────────────────────────────────────────────────────────────────
        Private Function IsIndexedFormat(pf As PixelFormat) As Boolean
            Return (pf And PixelFormat.Indexed) = PixelFormat.Indexed
        End Function

        Private Function IsLockBitsFriendly(pf As PixelFormat) As Boolean
            Select Case pf
                Case PixelFormat.Format32bppArgb, PixelFormat.Format32bppPArgb, PixelFormat.Format24bppRgb
                    Return True
                Case Else
                    Return False
            End Select
        End Function

        Private Sub CopyImageTo(ByVal src As Image, ByVal dst As Bitmap)
            Using g As Graphics = Graphics.FromImage(dst)
                g.DrawImage(src, 0, 0, dst.Width, dst.Height)
            End Using
        End Sub

        '────────────────────────────────────────────────────────────────────────
        ' 1) Brighten: additiver Offset auf RGB (amount: 0.0 .. 1.0)
        ' deepCopy=False → in-place; deepCopy=True → ersetzt & (autom.) entsorgt
        '────────────────────────────────────────────────────────────────────────
        Public Function Brighten(ByRef bmp As Bitmap, amount As Single, Optional deepCopy As Boolean = False) As Boolean
            If bmp Is Nothing Then Return False
            If amount <= 0.0001F Then Return False
            If amount > 1.0F Then amount = 1.0F

            If deepCopy Then
                Dim oldRef As Bitmap = bmp
                bmp = BrightenToNew(oldRef, amount)
                oldRef.Dispose()
                Return True
            Else
                Return BrightenInPlace(bmp, amount)
            End If
        End Function

        Private Function BrightenToNew(src As Image, amount As Single) As Bitmap
            Dim w As Integer = src.Width, h As Integer = src.Height
            Dim dst As New Bitmap(w, h, PixelFormat.Format32bppArgb)
            dst.SetResolution(src.HorizontalResolution, src.VerticalResolution)

            Using g As Graphics = Graphics.FromImage(dst)
                Using ia As New ImageAttributes()
                    Dim cm As New ColorMatrix(New Single()() {
                                New Single() {1, 0, 0, 0, 0},
                                New Single() {0, 1, 0, 0, 0},
                                New Single() {0, 0, 1, 0, 0},
                                New Single() {0, 0, 0, 1, 0},
                                New Single() {amount, amount, amount, 0, 1}
                            })
                    ia.SetColorMatrix(cm, ColorMatrixFlag.Default, ColorAdjustType.Bitmap)
                    g.DrawImage(src, New Rectangle(0, 0, w, h), 0, 0, w, h, GraphicsUnit.Pixel, ia)
                End Using
            End Using
            Return dst
        End Function

        Private Function BrightenInPlace(bmp As Bitmap, amount As Single) As Boolean
            Dim pf As PixelFormat = bmp.PixelFormat
            Dim offs As Integer = CInt(Math.Round(amount * 255.0F))
            If offs = 0 Then Return False

            If IsLockBitsFriendly(pf) Then
                Return ApplyPerPixelInPlace(bmp,
                            Sub(b As Integer, g As Integer, r As Integer, a As Integer,
                                ByRef nb As Byte, ByRef ng As Byte, ByRef nr As Byte, ByRef na As Byte)
                                Dim bb As Integer = b + offs : If bb > 255 Then bb = 255
                                Dim gg As Integer = g + offs : If gg > 255 Then gg = 255
                                Dim rr As Integer = r + offs : If rr > 255 Then rr = 255
                                nb = CByte(bb) : ng = CByte(gg) : nr = CByte(rr) : na = CByte(a)
                            End Sub)
            Else
                Using tmp As New Bitmap(bmp.Width, bmp.Height, PixelFormat.Format32bppArgb)
                    tmp.SetResolution(bmp.HorizontalResolution, bmp.VerticalResolution)
                    CopyImageTo(bmp, tmp)
                    BrightenInPlace(tmp, amount)
                    Using g As Graphics = Graphics.FromImage(bmp)
                        g.DrawImage(tmp, 0, 0, bmp.Width, bmp.Height)
                    End Using
                End Using
                Return True
            End If
        End Function

        '────────────────────────────────────────────────────────────────────────
        ' 2) Gamma-Korrektur: gamma > 0 (typ. 0.5 .. 2.2); 1.0 = neutral
        '────────────────────────────────────────────────────────────────────────
        Public Function AdjustGamma(ByRef bmp As Bitmap, gamma As Single, Optional deepCopy As Boolean = False) As Boolean
            If bmp Is Nothing Then Return False
            If gamma <= 0 Then Return False
            If Math.Abs(gamma - 1.0F) < 0.0001F Then Return False

            Dim lut(255) As Byte
            Dim inv As Single = 1.0F / gamma
            For i As Integer = 0 To 255
                Dim v As Single = CSng(Math.Pow(i / 255.0F, inv)) * 255.0F
                If v < 0 Then v = 0
                If v > 255 Then v = 255
                lut(i) = CByte(v)
            Next

            If deepCopy Then
                Dim oldRef As Bitmap = bmp
                bmp = ApplyLutToNew(oldRef, lut)
                oldRef.Dispose()
                Return True
            Else
                Return ApplyLutInPlace(bmp, lut)
            End If
        End Function

        '────────────────────────────────────────────────────────────────────────
        ' 3) Kontrast: contrast ∈ [-1.0 .. +1.0]; 0 = neutral
        ' new = (old - 128) * (1 + contrast) + 128
        '────────────────────────────────────────────────────────────────────────
        Public Function AdjustContrast(ByRef bmp As Bitmap, contrast As Single, Optional deepCopy As Boolean = False) As Boolean
            If bmp Is Nothing Then Return False
            If contrast < -1.0F Then contrast = -1.0F
            If contrast > +1.0F Then contrast = +1.0F
            If Math.Abs(contrast) < 0.0001F Then Return False

            Dim factor As Single = 1.0F + contrast
            Dim lut(255) As Byte
            For i As Integer = 0 To 255
                Dim v As Single = (i - 128.0F) * factor + 128.0F
                If v < 0 Then v = 0
                If v > 255 Then v = 255
                lut(i) = CByte(v)
            Next

            If deepCopy Then
                Dim oldRef As Bitmap = bmp
                bmp = ApplyLutToNew(oldRef, lut)
                oldRef.Dispose()
                Return True
            Else
                Return ApplyLutInPlace(bmp, lut)
            End If
        End Function

        '────────────────────────────────────────────────────────────────────────
        ' 4) Schärfen: 3x3-Kernel (0 -1 0 / -1 5 -1 / 0 -1 0) mit Stärke
        '────────────────────────────────────────────────────────────────────────
        Public Function Sharpen(ByRef bmp As Bitmap, Optional strength As Single = 1.0F, Optional deepCopy As Boolean = False) As Boolean
            If bmp Is Nothing Then Return False
            If strength < 0F Then strength = 0F
            If strength > 2.0F Then strength = 2.0F
            If strength < 0.0001F Then Return False

            Dim k00 As Single = 0, k01 As Single = -1 * strength, k02 As Single = 0
            Dim k10 As Single = -1 * strength, k11 As Single = 5 * strength, k12 As Single = -1 * strength
            Dim k20 As Single = 0, k21 As Single = -1 * strength, k22 As Single = 0

            If deepCopy Then
                Dim oldRef As Bitmap = bmp
                bmp = ConvolveToNew(oldRef, k00, k01, k02, k10, k11, k12, k20, k21, k22)
                oldRef.Dispose()
                Return True
            Else
                ' In-place via temporäre Quelle
                Dim srcCopy As New Bitmap(bmp.Width, bmp.Height, PixelFormat.Format32bppArgb)
                srcCopy.SetResolution(bmp.HorizontalResolution, bmp.VerticalResolution)
                CopyImageTo(bmp, srcCopy)
                Using g As Graphics = Graphics.FromImage(bmp)
                    Using filtered As Bitmap = ConvolveToNew(srcCopy, k00, k01, k02, k10, k11, k12, k20, k21, k22)
                        g.DrawImage(filtered, 0, 0, bmp.Width, bmp.Height)
                    End Using
                End Using
                srcCopy.Dispose()
                Return True
            End If
        End Function



        ' ─────────────────────────────────────────────────────────────────────────
        ' 5) ShrinkBitmap:
        '    - 0 bei einer Dimension erlaubt → proportional berechnen
        ' ─────────────────────────────────────────────────────────────────────────

        Private Sub ComputeTargetSize(srcW As Integer, srcH As Integer, ByRef newW As Integer, ByRef newH As Integer)
            If newW <= 0 AndAlso newH > 0 Then
                newW = CInt(Math.Max(1, Math.Round(newH * (srcW / CDbl(srcH)))))
            ElseIf newH <= 0 AndAlso newW > 0 Then
                newH = CInt(Math.Max(1, Math.Round(newW * (srcH / CDbl(srcW)))))
            End If
        End Sub

        '────────────────────────────────────────────────────────────────────────
        ' Auto-Heuristik für Mehrstufen-Downscaling
        ' r = min(newW/origW, newH/origH)
        ' - r ≥ 0.75  → 1 Schritt (direkt)
        ' - sonst     → steps = ceil( log(r) / log(perStep) ), perStep ≈ 0.6
        ' Clamp: 2..maxSteps
        '────────────────────────────────────────────────────────────────────────
        Private Function ComputeAutoSteps(origW As Integer,
                                          origH As Integer,
                                          newW As Integer,
                                          newH As Integer,
                                          Optional perStep As Double = 0.6,
                                          Optional maxSteps As Integer = 6) As Integer
            If origW <= 0 OrElse origH <= 0 OrElse newW <= 0 OrElse newH <= 0 Then Return 1
            Dim rW As Double = newW / CDbl(origW)
            Dim rH As Double = newH / CDbl(origH)
            Dim r As Double = Math.Min(rW, rH)

            If r >= 0.75 Then Return 1 ' kleine Änderung → direkt
            If perStep <= 0.05 OrElse perStep >= 0.95 Then perStep = 0.6

            Dim steps As Integer = CInt(Math.Ceiling(Math.Log(r) / Math.Log(perStep)))
            If steps < 2 Then steps = 2
            If steps > maxSteps Then steps = maxSteps
            Return steps
        End Function


        ''' <summary>
        ''' Verkleinert oder vergrößert eine Bitmap.
        ''' - Eine Dimension darf 0 sein → wird aus der übergebenen Bitmap proportional berechnet.
        ''' - Beide ≤ 0 → Abbruch (False).
        ''' - TileFlipXY ist Standard → keine Randartefakte bei Bicubic.
        ''' </summary>
        Public Function ShrinkBitmap(ByRef bmp As Bitmap,
                                     newWidth As Integer,
                                     newHeight As Integer,
                                     Optional disposeSrc As Boolean = False,
                                     Optional highQuality As Boolean = True,
                                     Optional cvtToARGBBitmap As Boolean = True
                                    ) As Boolean

            If bmp Is Nothing Then Return False

            If cvtToARGBBitmap AndAlso Not bmp.PixelFormat = PixelFormat.Format32bppPArgb Then
                'umstellen auf Argb-Format
                Dim nbmp As New Bitmap(bmp.Width, bmp.Height, PixelFormat.Format32bppArgb)
                Dim gfx As Graphics = Graphics.FromImage(nbmp)
                gfx.DrawImage(bmp, New Point(0, 0))
                gfx.Dispose()
                bmp = nbmp
            End If

            ' fehlende Dimension aus Proportionen ableiten
            ComputeTargetSize(bmp.Width, bmp.Height, newWidth, newHeight)

            ' ungültig?
            If newWidth <= 0 AndAlso newHeight <= 0 Then Return False
            If newWidth <= 0 OrElse newHeight <= 0 Then Return False

            ' Nichts zu tun?, aber True zurückliefern
            If bmp.Width = newWidth AndAlso bmp.Height = newHeight Then Return True

            Dim oldRef As Bitmap = bmp
            Dim dst As New Bitmap(newWidth, newHeight, PixelFormat.Format32bppArgb)
            dst.SetResolution(oldRef.HorizontalResolution, oldRef.VerticalResolution)

            Using g As Graphics = Graphics.FromImage(dst)
                If highQuality Then
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic
                    g.SmoothingMode = SmoothingMode.HighQuality
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality
                    g.CompositingQuality = CompositingQuality.HighQuality
                Else
                    g.InterpolationMode = InterpolationMode.NearestNeighbor
                End If

                ' Standard: TileFlipXY an den Rändern
                Using ia As New ImageAttributes()
                    ia.SetWrapMode(WrapMode.TileFlipXY)
                    g.DrawImage(
                        oldRef,
                        New Rectangle(0, 0, newWidth, newHeight),
                        0, 0, oldRef.Width, oldRef.Height,
                        GraphicsUnit.Pixel,
                        ia
                    )
                End Using
            End Using

            bmp = dst

            If disposeSrc Then oldRef.Dispose()

            Return True

        End Function

        ''' <summary>
        ''' ShrinkBitmap mit Zwischenschritten (steps).
        ''' - steps &gt; 1: genau so viele Stufen.
        ''' - steps &lt;= 1: Auto-Heuristik (ComputeAutoSteps) wird verwendet.
        '''   → Bei geringer Verkleinerung bleibt es bei einem Schritt.
        ''' </summary>
        Public Function ShrinkBitmap(ByRef bmp As Bitmap,
                                     newWidth As Integer,
                                     newHeight As Integer,
                                     steps As Integer,
                                     Optional disposeSrc As Boolean = False,
                                     Optional highQuality As Boolean = True,
                                     Optional cvtToARGBBitmap As Boolean = True) As Boolean
            If bmp Is Nothing Then Return False

            If cvtToARGBBitmap AndAlso Not bmp.PixelFormat = PixelFormat.Format32bppPArgb Then
                'umstellen auf Argb-Format
                Dim nbmp As New Bitmap(bmp.Width, bmp.Height, PixelFormat.Format32bppArgb)
                Dim gfx As Graphics = Graphics.FromImage(nbmp)
                gfx.DrawImage(bmp, New Point(0, 0))
                gfx.Dispose()
                bmp = nbmp
            End If

            ' Zielmaße ggf. proportional ergänzen
            ComputeTargetSize(bmp.Width, bmp.Height, newWidth, newHeight)
            If newWidth <= 0 OrElse newHeight <= 0 Then Return False

            ' Auto-Heuristik einschalten, wenn steps <= 1
            If steps < 1 Then
                Dim autoSteps As Integer = ComputeAutoSteps(bmp.Width, bmp.Height, newWidth, newHeight)
                If autoSteps <= 1 Then
                    ' kleine Änderung → ein Schritt reicht
                    Return ShrinkBitmap(bmp, newWidth, newHeight, highQuality, disposeSrc)
                Else
                    steps = autoSteps
                End If
            End If

            ' Schon Zielgröße?
            If bmp.Width = newWidth AndAlso bmp.Height = newHeight Then Return False

            ' Mehrstufige Verkleinerung
            Dim origW As Integer = bmp.Width
            Dim origH As Integer = bmp.Height

            Dim curBmp As Bitmap = bmp
            For i As Integer = 1 To steps
                Dim t As Double = i / steps
                Dim stepW As Integer
                Dim stepH As Integer
                If i < steps Then
                    stepW = CInt(Math.Max(1, Math.Round(origW + (newWidth - origW) * t)))
                    stepH = CInt(Math.Max(1, Math.Round(origH + (newHeight - origH) * t)))
                Else
                    stepW = newWidth
                    stepH = newHeight
                End If

                Dim tmp As New Bitmap(stepW, stepH, PixelFormat.Format32bppArgb)
                tmp.SetResolution(curBmp.HorizontalResolution, curBmp.VerticalResolution)

                Using g As Graphics = Graphics.FromImage(tmp)
                    If highQuality Then
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic
                        g.SmoothingMode = SmoothingMode.HighQuality
                        g.PixelOffsetMode = PixelOffsetMode.HighQuality
                        g.CompositingQuality = CompositingQuality.HighQuality
                    Else
                        g.InterpolationMode = InterpolationMode.NearestNeighbor
                    End If

                    Using ia As New ImageAttributes()
                        ia.SetWrapMode(WrapMode.TileFlipXY) ' saubere Ränder in jeder Stufe
                        g.DrawImage(curBmp,
                                    New Rectangle(0, 0, stepW, stepH),
                                    0, 0, curBmp.Width, curBmp.Height,
                                    GraphicsUnit.Pixel,
                                    ia)
                    End Using
                End Using

                ' Vorherige Instanz entsorgen:
                If i > 1 OrElse disposeSrc Then
                    curBmp.Dispose()
                End If

                curBmp = tmp
            Next

            bmp = curBmp
            Return True
        End Function


        '────────────────────────────────────────────────────────────────────────
        ' 6) Speichern (PNG) & Laden (ohne Dateisperre)
        '────────────────────────────────────────────────────────────────────────
        Public Sub SavePng(bmp As Image, filePath As String)
            If bmp Is Nothing Then Throw New ArgumentNullException(NameOf(bmp))
            If String.IsNullOrWhiteSpace(filePath) Then Throw New ArgumentException("filePath leer", NameOf(filePath))
            Dim dir As String = Path.GetDirectoryName(filePath)
            If Not String.IsNullOrEmpty(dir) AndAlso Not Directory.Exists(dir) Then
                Directory.CreateDirectory(dir)
            End If
            bmp.Save(filePath, ImageFormat.Png)
        End Sub

        ''' <summary>
        ''' Der Unterschied zu LoadImage ist 1.) keine Unterstützung von Bildern im webp-Format.
        ''' 2.) Wirft Exceptions, wenn useErrorBitmap auf False steht.
        ''' requestedSize ist die Größe der Fehlergrafik. Defaulu 256 x 170
        ''' </summary>
        ''' <param name="filePath"></param>
        ''' <returns></returns>
        Public Function LoadBitmap(filePath As String,
                           Optional useErrorBitmap As Boolean = False,
                           Optional requestedSize As Size = Nothing) As Bitmap
            Try
                If String.IsNullOrWhiteSpace(filePath) Then
                    If useErrorBitmap Then
                        Return CreateErrorBitmap(filePath, requestedSize, "Pfad leer.")
                    Else
                        Throw New ArgumentException("filePath leer", NameOf(filePath))
                    End If
                End If

                Using fs As New FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read)
                    Using img As Image = Image.FromStream(fs)
                        Dim bmp As New Bitmap(img.Width, img.Height, PixelFormat.Format32bppArgb)
                        bmp.SetResolution(img.HorizontalResolution, img.VerticalResolution)
                        CopyImageTo(img, bmp)
                        Return bmp
                    End Using
                End Using

            Catch ex As Exception
                If useErrorBitmap Then
                    Return CreateErrorBitmap(filePath,
                                     If(requestedSize = Nothing, New Size(256, 170), requestedSize),
                                     ex.Message)
                Else
                    Throw
                End If
            End Try
        End Function

        '────────────────────────────────────────────────────────────────────────
        ' Gemeinsame Low-Level-Helfer
        '────────────────────────────────────────────────────────────────────────
        Private Function ApplyLutInPlace(bmp As Bitmap, lut() As Byte) As Boolean
            Dim pf As PixelFormat = bmp.PixelFormat
            If IsLockBitsFriendly(pf) Then
                Return ApplyPerPixelInPlace(bmp,
                            Sub(b As Integer, g As Integer, r As Integer, a As Integer,
                                ByRef nb As Byte, ByRef ng As Byte, ByRef nr As Byte, ByRef na As Byte)
                                nb = lut(b) : ng = lut(g) : nr = lut(r) : na = CByte(a)
                            End Sub)
            Else
                Using tmp As New Bitmap(bmp.Width, bmp.Height, PixelFormat.Format32bppArgb)
                    tmp.SetResolution(bmp.HorizontalResolution, bmp.VerticalResolution)
                    CopyImageTo(bmp, tmp)
                    ApplyLutInPlace(tmp, lut)
                    Using g As Graphics = Graphics.FromImage(bmp)
                        g.DrawImage(tmp, 0, 0, bmp.Width, bmp.Height)
                    End Using
                End Using
                Return True
            End If
        End Function

        Private Function ApplyLutToNew(src As Image, lut() As Byte) As Bitmap
            Dim w As Integer = src.Width, h As Integer = src.Height
            Dim dst As New Bitmap(w, h, PixelFormat.Format32bppArgb)
            dst.SetResolution(src.HorizontalResolution, src.VerticalResolution)

            Using g As Graphics = Graphics.FromImage(dst)
                g.DrawImage(src, 0, 0, w, h)
            End Using

            ApplyLutInPlace(dst, lut)
            Return dst
        End Function

        Private Function ApplyPerPixelInPlace(bmp As Bitmap, pxOp As PixelOp) As Boolean
            Dim rect As New Rectangle(0, 0, bmp.Width, bmp.Height)
            Dim data As BitmapData = Nothing
            Try
                data = bmp.LockBits(rect, ImageLockMode.ReadWrite, bmp.PixelFormat)
                Dim stride As Integer = data.Stride
                Dim height As Integer = data.Height
                Dim width As Integer = data.Width
                Dim ptr As IntPtr = data.Scan0
                Dim bytes As Integer = Math.Abs(stride) * height
                Dim buffer(bytes - 1) As Byte
                Marshal.Copy(ptr, buffer, 0, bytes)

                Select Case bmp.PixelFormat
                    Case PixelFormat.Format32bppArgb, PixelFormat.Format32bppPArgb
                        For y As Integer = 0 To height - 1
                            Dim row As Integer = y * stride
                            For x As Integer = 0 To width - 1
                                Dim i As Integer = row + x * 4
                                Dim b As Integer = buffer(i + 0)
                                Dim g As Integer = buffer(i + 1)
                                Dim r As Integer = buffer(i + 2)
                                Dim a As Integer = buffer(i + 3)
                                Dim nb As Byte, ng As Byte, nr As Byte, na As Byte
                                pxOp(b, g, r, a, nb, ng, nr, na)
                                buffer(i + 0) = nb : buffer(i + 1) = ng : buffer(i + 2) = nr : buffer(i + 3) = na
                            Next
                        Next

                    Case PixelFormat.Format24bppRgb
                        For y As Integer = 0 To height - 1
                            Dim row As Integer = y * stride
                            For x As Integer = 0 To width - 1
                                Dim i As Integer = row + x * 3
                                Dim b As Integer = buffer(i + 0)
                                Dim g As Integer = buffer(i + 1)
                                Dim r As Integer = buffer(i + 2)
                                Dim nb As Byte, ng As Byte, nr As Byte, na As Byte
                                pxOp(b, g, r, 255, nb, ng, nr, na)
                                buffer(i + 0) = nb : buffer(i + 1) = ng : buffer(i + 2) = nr
                            Next
                        Next

                    Case Else
                        ' Nicht unterstützt → 32bpp-Umweg
                        bmp.UnlockBits(data)
                        data = Nothing
                        Using tmp As New Bitmap(bmp.Width, bmp.Height, PixelFormat.Format32bppArgb)
                            tmp.SetResolution(bmp.HorizontalResolution, bmp.VerticalResolution)
                            CopyImageTo(bmp, tmp)
                            ApplyPerPixelInPlace(tmp, pxOp)
                            Using g As Graphics = Graphics.FromImage(bmp)
                                g.DrawImage(tmp, 0, 0, bmp.Width, bmp.Height)
                            End Using
                        End Using
                        Return True
                End Select

                Marshal.Copy(buffer, 0, ptr, bytes)
                Return True
            Finally
                If data IsNot Nothing Then
                    bmp.UnlockBits(data)
                End If
            End Try
        End Function

        Private Function ConvolveToNew(src As Bitmap,
                                               k00 As Single, k01 As Single, k02 As Single,
                                               k10 As Single, k11 As Single, k12 As Single,
                                               k20 As Single, k21 As Single, k22 As Single) As Bitmap

            Dim w As Integer = src.Width, h As Integer = src.Height
            Dim srcPf As PixelFormat = If(IsLockBitsFriendly(src.PixelFormat), src.PixelFormat, PixelFormat.Format32bppArgb)

            Dim srcWork As Bitmap
            If srcPf = src.PixelFormat Then
                srcWork = src
            Else
                srcWork = New Bitmap(w, h, srcPf)
                srcWork.SetResolution(src.HorizontalResolution, src.VerticalResolution)
                CopyImageTo(src, srcWork)
            End If

            Dim dst As New Bitmap(w, h, srcPf)
            dst.SetResolution(src.HorizontalResolution, src.VerticalResolution)

            Dim rect As New Rectangle(0, 0, w, h)
            Dim sData As BitmapData = Nothing, dData As BitmapData = Nothing
            Try
                sData = srcWork.LockBits(rect, ImageLockMode.ReadOnly, srcPf)
                dData = dst.LockBits(rect, ImageLockMode.WriteOnly, srcPf)

                Dim sStride As Integer = sData.Stride
                Dim dStride As Integer = dData.Stride
                Dim bpp As Integer = If(srcPf = PixelFormat.Format24bppRgb, 3, 4)

                Dim sBuf(Math.Abs(sStride) * h - 1) As Byte
                Dim dBuf(Math.Abs(dStride) * h - 1) As Byte
                Marshal.Copy(sData.Scan0, sBuf, 0, sBuf.Length)

                For y As Integer = 0 To h - 1
                    For x As Integer = 0 To w - 1
                        Dim accR As Single = 0, accG As Single = 0, accB As Single = 0
                        For ky As Integer = -1 To 1
                            Dim yy As Integer = Math.Min(Math.Max(y + ky, 0), h - 1)
                            Dim row As Integer = yy * sStride
                            For kx As Integer = -1 To 1
                                Dim xx As Integer = Math.Min(Math.Max(x + kx, 0), w - 1)
                                Dim idx As Integer = row + xx * bpp
                                Dim b As Integer = sBuf(idx + 0)
                                Dim g As Integer = sBuf(idx + 1)
                                Dim r As Integer = sBuf(idx + 2)
                                Dim k As Single
                                If ky = -1 AndAlso kx = -1 Then k = k00
                                If ky = -1 AndAlso kx = 0 Then k = k01
                                If ky = -1 AndAlso kx = 1 Then k = k02
                                If ky = 0 AndAlso kx = -1 Then k = k10
                                If ky = 0 AndAlso kx = 0 Then k = k11
                                If ky = 0 AndAlso kx = 1 Then k = k12
                                If ky = 1 AndAlso kx = -1 Then k = k20
                                If ky = 1 AndAlso kx = 0 Then k = k21
                                If ky = 1 AndAlso kx = 1 Then k = k22
                                accB += b * k
                                accG += g * k
                                accR += r * k
                            Next
                        Next
                        Dim di As Integer = y * dStride + x * bpp
                        Dim nb As Integer = CInt(Math.Round(accB)) : If nb < 0 Then nb = 0 : If nb > 255 Then nb = 255
                        Dim ng As Integer = CInt(Math.Round(accG)) : If ng < 0 Then ng = 0 : If ng > 255 Then ng = 255
                        Dim nr As Integer = CInt(Math.Round(accR)) : If nr < 0 Then nr = 0 : If nr > 255 Then nr = 255
                        dBuf(di + 0) = CByte(nb)
                        dBuf(di + 1) = CByte(ng)
                        dBuf(di + 2) = CByte(nr)
                        If bpp = 4 Then
                            ' Alpha aus Quelle beibehalten (Zentrumspixel)
                            Dim si As Integer = y * sStride + x * 4
                            dBuf(di + 3) = sBuf(si + 3)
                        End If
                    Next
                Next

                Marshal.Copy(dBuf, 0, dData.Scan0, dBuf.Length)
            Finally
                If sData IsNot Nothing Then srcWork.UnlockBits(sData)
                If dData IsNot Nothing Then dst.UnlockBits(dData)
                If Not Object.ReferenceEquals(srcWork, src) Then srcWork.Dispose()
            End Try

            Return dst
        End Function


        Private Const DEFAULT_STEPS As Integer = -1 'automatische Festlegung

        ''' <summary>
        ''' Lädt ein Bild (WebP inkl.) in Originalgröße als Bitmap.
        ''' </summary>
        Public Function LoadBitmapViaMagick(path As String) As Bitmap
            Return LoadBitmapViaMagick(path, Size.Empty)
        End Function

        ''' <summary>
        ''' Lädt ein Bild (WebP inkl.) und skaliert es optional auf die gewünschte Größe.
        ''' Fehler liefern eine rote Fehler-Bitmap mit dem Dateinamen.
        ''' </summary>
        Public Function LoadBitmapViaMagick(path As String, size As Size) As Bitmap
            ' 1) Vorbedingungen: keine Exceptions mehr, stattdessen Fehlergrafik
            If String.IsNullOrWhiteSpace(path) Then
                Return CreateErrorBitmap(path, size, "Pfad ist leer.")
            End If

            If Not File.Exists(path) Then
                Return CreateErrorBitmap(path, size, "Datei nicht gefunden.")
            End If

            Dim src As Bitmap = Nothing

            Try
                ' 2) Ursprungsbitmap laden (immer über Magick)
                src = LoadBitmapCore(path)
                If src Is Nothing Then
                    Return CreateErrorBitmap(path, size, "Bild konnte nicht geladen werden.")
                End If

                ' 3) Zielgröße berechnen
                Dim target As Size = ComputeTargetSize(src.Size, size)

                ' 4) Keine Skalierung nötig → direkt zurückgeben
                If target.Width = src.Width AndAlso target.Height = src.Height Then
                    ' src NICHT entsorgen – wird als Ergebnis zurückgegeben
                    Return src
                End If

                ' 5) Verkleinern mit deiner Routine
                Dim work As Bitmap = src ' ShrinkBitmap kann (bei disposeSrc:=True) src entsorgen/ersetzen
                Dim ok As Boolean = False
                Try
                    ok = MjGDI.ShrinkBitmap(work,
                                    newWidth:=target.Width,
                                    newHeight:=target.Height,
                                    steps:=DEFAULT_STEPS,
                                    disposeSrc:=True,        ' src darf entsorgt werden
                                    highQuality:=True,
                                    cvtToARGBBitmap:=True)
                Catch
                    ok = False
                End Try

                If ok AndAlso work IsNot Nothing Then
                    ' work enthält das Ergebnis. src wurde (falls nötig) bereits entsorgt.
                    Return work
                End If

                ' 6) Fallback-Skalierung (falls ShrinkBitmap fehlschlägt)
                Dim fallback As New Bitmap(target.Width, target.Height, PixelFormat.Format32bppArgb)
                Using g As Graphics = Graphics.FromImage(fallback)
                    g.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic
                    g.PixelOffsetMode = Drawing2D.PixelOffsetMode.HighQuality
                    g.SmoothingMode = Drawing2D.SmoothingMode.HighQuality
                    g.CompositingQuality = Drawing2D.CompositingQuality.HighQuality
                    g.DrawImage(src, New Rectangle(Point.Empty, target))
                End Using

                ' src hier explizit entsorgen (nur wenn wir nicht vorher zurückgegeben haben)
                src.Dispose()
                Return fallback

            Catch ex As Exception
                ' Irgendein anderer Fehler → Fehlergrafik
                If src IsNot Nothing Then
                    Try : src.Dispose() : Catch : End Try
                End If
                Return CreateErrorBitmap(path, size, ex.Message)
            End Try
        End Function


        ' ────────────────────────────────────────────────────────────────
        ' Interna
        ' ────────────────────────────────────────────────────────────────

        ''' <summary>
        ''' Lädt beliebige Bildformate (inkl. WebP) über Magick.NET und liefert ein Bitmap.
        ''' </summary>
        Private Function LoadBitmapCore(path As String) As Bitmap
            Using mi As New MagickImage(path)
                Dim bytes() As Byte = mi.ToByteArray(MagickFormat.Png)
                Using ms As New MemoryStream(bytes, writable:=False)
                    Using temp As New Bitmap(ms)
                        Return New Bitmap(temp)
                    End Using
                End Using
            End Using
        End Function

        ''' <summary>
        ''' Berechnet die Zielgröße aus Vorgabe und Original.
        ''' </summary>
        Private Function ComputeTargetSize(original As Size, requested As Size) As Size
            If requested.Width <= 0 AndAlso requested.Height <= 0 Then
                Return original
            End If

            If requested.Width > 0 AndAlso requested.Height > 0 Then
                Return requested
            End If

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
        ''' Erzeugt eine rote Fehler-Bitmap mit Dateinamen und optional Fehlermeldung.
        ''' </summary>
        Private Function CreateErrorBitmap(fullpath As String, requested As Size, Optional message As String = Nothing) As Bitmap
            Dim w As Integer = If(requested.Width > 0, requested.Width, 256)
            Dim h As Integer = If(requested.Height > 0, requested.Height, 170)

            Dim bmp As New Bitmap(w, h, PixelFormat.Format32bppArgb)
            Using g As Graphics = Graphics.FromImage(bmp)
                g.Clear(Color.Red)

                Using font As New Font("Segoe UI", 9, FontStyle.Bold)
                    Dim nameOnly As String = ""
                    Try
                        nameOnly = Path.GetFileName(If(fullpath, ""))
                    Catch
                        nameOnly = "(unbekannt)"
                    End Try

                    Dim text As String = If(String.IsNullOrEmpty(nameOnly), "(ohne Name)", nameOnly)
                    If Not String.IsNullOrEmpty(message) Then
                        text &= vbCrLf & message
                    End If

                    Dim rect As New RectangleF(4, 4, w - 8, h - 8)
                    Dim fmt As New StringFormat With {.Alignment = StringAlignment.Near, .LineAlignment = StringAlignment.Near}
                    g.DrawString(text, font, Brushes.White, rect, fmt)
                End Using
            End Using
            Return bmp
        End Function

    End Module


End Namespace