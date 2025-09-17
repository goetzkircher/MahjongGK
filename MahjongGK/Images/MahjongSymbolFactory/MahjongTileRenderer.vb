Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On
Imports System.Drawing.Drawing2D
Imports System.Drawing.Imaging

Namespace MahjongGKSymbolFactory

    ' ''' 
    ' '' <summary>
    ' '' Rendert einen Mahjong-Ziegel als Bitmap.
    ' '' </summary>
    ' ''' <param name="sizePx">Zielgröße in Pixeln, z.B. 198x252.</param>
    ' ''' <param name="symbol">Transparente Symbolgrafik (wird skaliert/zentriert). Darf Nothing sein.</param>
    ' ''' <param name="style">Optionaler Stil; wenn Nothing, wird Default basierend auf sizePx verwendet.</param>
    ' ''' <returns>ARGB-Bitmap mit weichem Schlagschatten und Transparenz.</returns>
    Public NotInheritable Class MahjongTileRenderer

        Private Sub New()

        End Sub

        Public Shared Function RenderTile(sizePx As Size,
                                          symbol As Bitmap,
                                          Optional style As TileStyle? = Nothing) As Bitmap

            '    Debug.Print("Renderer ns: " & GetType(MahjongTileRenderer).FullName)

#Disable Warning IDE0030 ' COALESCE-Ausdruck verwenden
            Dim st As TileStyle = If(style.HasValue, style.Value, TileStyle.CreateDefault(sizePx))
#Enable Warning IDE0030 ' COALESCE-Ausdruck verwenden

            ' Arbeitsfläche inkl. Rand für Shadow
            Dim pad As Integer = CInt(Math.Ceiling(st.ShadowSize * 1.6F))
            Dim bmp As New Bitmap(sizePx.Width + pad * 2, sizePx.Height + pad * 2, PixelFormat.Format32bppPArgb)

            Using g As Graphics = Graphics.FromImage(bmp)
                ConfigureHighQuality(g)

                Dim outer As New RectangleF(pad, pad, sizePx.Width, sizePx.Height)
                Dim pathOuter As GraphicsPath = RoundedRect(outer, st.CornerRadius)

                ' 1) Schatten
                DrawShadow(g, pathOuter, st)

                ' 2) Außenkörper (sanfter Vertikalverlauf)
                Using lg As New LinearGradientBrush(outer, st.BodyTop, st.BodyBottom, LinearGradientMode.Vertical)
                    g.FillPath(lg, pathOuter)
                End Using


                ' 3) Oberseite (Face) mit Ivory-Verlauf + FaceTint
                Dim faceRect As RectangleF = InflateRect(outer, -st.EdgeWidth - st.FaceInset)

                Dim facePath As GraphicsPath = RoundedRect(faceRect, Math.Max(2.0F, st.CornerRadius * 0.65F))
                Using lgFace As New LinearGradientBrush(faceRect, st.FaceIvoryLight, st.FaceIvoryDark, LinearGradientMode.Vertical)
                    g.FillPath(lgFace, facePath)
                End Using

                ' --- Sichtbare, adaptive Tönung (hell/pastell bleibt sichtbar) ---
                Dim aSrc As Integer = st.FaceTint.A
                Dim tintL As Single = (0.2126F * st.FaceTint.R + 0.7152F * st.FaceTint.G + 0.0722F * st.FaceTint.B) / 255.0F
                Dim boost As Single = 1.0F + (Math.Max(1.0F, st.FaceTintBoostMax) - 1.0F) * Math.Min(1.0F, Math.Max(0.0F, tintL))
                Dim minA As Integer = Math.Max(0, Math.Min(255, st.FaceTintMinAlpha))
                ' Deckel leicht oberhalb, damit Glanz/Noise nicht erdrückt werden:
                Dim effA As Integer = CInt(Math.Min(160.0F, Math.Max(minA, aSrc * boost)))
                Using tintBr As New SolidBrush(Color.FromArgb(effA, st.FaceTint))
                    g.FillPath(tintBr, facePath)
                End Using


                ' 4) Papierkorn / leichtes Rauschen
                If st.NoiseOpacity > 0 Then
                    Using noise As Bitmap = MakeNoise(faceRect.Size.ToSize(), st.NoiseOpacity, seed:=123)
                        g.SetClip(facePath)
                        g.DrawImage(noise, faceRect.Location)
                        g.ResetClip()
                    End Using
                End If

                ' 5) Glanzlicht (oben)
                If st.GlossOpacity > 0 Then
                    DrawGloss(g, facePath, faceRect, st.GlossOpacity)
                End If

                '6) Kanten abrunden
                ' Kantenabrundung – nutzt Farben/Regler aus st (EdgeLight/EdgeDark, WidthPx, Alpha …)
                ApplyFaceRoundover(g, facePath, faceRect,
                   edgeWidth:=Math.Max(6.0F, st.EdgeWidth * 0.6F),
                   maxHighlight:=80,
                   maxShadow:=60)

                ' 7) Symbol platzieren (zentriert & skaliert) + sanfter Symbol-Schatten
                If symbol IsNot Nothing Then
                    DrawSymbol(g, symbol, faceRect, st)
                End If

                ' 8) Dezente Innenkontur
                Using penInside As New Pen(Color.FromArgb(80, 0, 0, 0), 1.0F)
                    g.DrawPath(penInside, facePath)
                End Using

                pathOuter.Dispose()
                facePath.Dispose()

                '"btnFaceTint" 
                '"btnBodyTop"  
                '"btnBodyBottom"
                '"btnEdgeLight" 
                '"btnEdgeDark" :
                '"btnIvoryLight"
                '"btnIvoryDark" 

                Using dbg As New SolidBrush(Color.FromArgb(255, st.FaceTint))
                    g.FillRectangle(dbg, 0, 40, 20, 40)
                End Using
                Using dbg As New SolidBrush(Color.FromArgb(255, st.BodyTop))
                    g.FillRectangle(dbg, 0, 80, 20, 40)
                End Using
                Using dbg As New SolidBrush(Color.FromArgb(255, st.BodyBottom))
                    g.FillRectangle(dbg, 0, 120, 20, 40)
                End Using
                Using dbg As New SolidBrush(Color.FromArgb(255, st.EdgeLight))
                    g.FillRectangle(dbg, 0, 160, 20, 40)
                End Using
                Using dbg As New SolidBrush(Color.FromArgb(255, st.EdgeDark))
                    g.FillRectangle(dbg, 0, 200, 20, 40)
                End Using
                Using dbg As New SolidBrush(Color.FromArgb(255, st.FaceIvoryLight))
                    g.FillRectangle(dbg, 0, 240, 20, 40)
                End Using
                Using dbg As New SolidBrush(Color.FromArgb(255, st.FaceIvoryDark))
                    g.FillRectangle(dbg, 0, 280, 20, 40)
                End Using

            End Using

            Return bmp

        End Function

        '
        ''' <summary>
        ''' Erzeugt aus 42 Symbolbitmaps einen kompletten Satz von gerenderten Ziegeln.
        ''' </summary>
        ''' <param name="symbols">Liste/Array mit 42 Symbolen (0..41). Null-Einträge werden übersprungen.</param>
        ''' <param name="sizePx">Zielgröße, z.B. 198x252.</param>
        ''' <param name="faceTint">Einheitlicher Farbton der Oberseite (je nach Satz variieren).</param>
        ''' <returns>Liste der fertigen Steinbitmaps in gleicher Reihenfolge.</returns>
        Public Shared Function RenderSet(symbols As IList(Of Bitmap),
                                         sizePx As Size,
                                         faceTint As Color) As List(Of Bitmap)
            Dim st As TileStyle = TileStyle.CreateDefault(sizePx)
            st.FaceTint = faceTint

            Dim result As New List(Of Bitmap)(symbols.Count)
            For i As Integer = 0 To symbols.Count - 1
                Dim bmp As Bitmap = RenderTile(sizePx, symbols(i), st)
                result.Add(bmp)
            Next
            Return result
        End Function

        ' ============================
        '  Helfer
        ' ============================

        Private Shared Sub ConfigureHighQuality(g As Graphics)
            g.SmoothingMode = SmoothingMode.AntiAlias
            g.InterpolationMode = InterpolationMode.HighQualityBicubic
            g.PixelOffsetMode = PixelOffsetMode.HighQuality
            g.CompositingQuality = CompositingQuality.HighQuality
        End Sub

        Private Shared Function RoundedRect(r As RectangleF, radius As Single) As GraphicsPath
            Dim gp As New GraphicsPath()
            Dim maxRadius As Single = Math.Max(0.0F, Math.Min(radius, Math.Min(r.Width, r.Height) * 0.5F))
            If maxRadius <= 0.5F Then
                gp.AddRectangle(r)
                Return gp
            End If

            Dim d As Single = maxRadius * 2.0F
            Dim arc As New RectangleF(r.X, r.Y, d, d)
            gp.AddArc(arc, 180, 90)
            arc.X = r.Right - d
            gp.AddArc(arc, 270, 90)
            arc.Y = r.Bottom - d
            gp.AddArc(arc, 0, 90)
            arc.X = r.X
            gp.AddArc(arc, 90, 90)
            gp.CloseFigure()
            Return gp
        End Function

        Private Shared Function InflateRect(r As RectangleF, delta As Single) As RectangleF
            Return New RectangleF(r.X + delta, r.Y + delta, r.Width - 2 * delta, r.Height - 2 * delta)
        End Function

        Private Shared Sub DrawShadow(g As Graphics, path As GraphicsPath, st As TileStyle)
            ' Weicher Drop-Schatten (mehrfaches Füllen versetzter, wachsender Pfade)
            Dim bounds As RectangleF = path.GetBounds()

            ' Versatz (Richtung rechts/unten):
            Dim offset As Single = Math.Max(1.5F, st.ShadowSize * 0.35F)

            ' Schrittzahl an ShadowSize koppeln:
            Dim steps As Integer = Math.Max(8, CInt(st.ShadowSize * 1.2F)) ' z.B. 24 bei ShadowSize=20

            ' Startrechteck: nur versetzt, noch NICHT aufgebläht
            Dim r As New RectangleF(bounds.X + offset, bounds.Y + offset, bounds.Width, bounds.Height)

            ' Schatten-Rundung: separat regelbar
            Dim shadowCornerRadius As Single = Math.Max(0.0F, st.CornerRadius * 0.45F)

            ' Max-Alpha
            Dim maxA As Integer = 90

            For i As Integer = steps To 1 Step -1
                ' Wachsender „Weichheits“-Rand:
                Dim grow As Single = i * 0.55F
                ' Quadratisch abfallende Alpha-Kurve:
                Dim a As Integer = CInt(maxA * (i / CSng(steps)) ^ 2)
                If a <= 0 Then Continue For

                ' EINMALIG aufblähen: Ausgang r + grow
                Dim rr As New RectangleF(r.X - grow, r.Y - grow, r.Width + 2 * grow, r.Height + 2 * grow)

                Using gp As GraphicsPath = RoundedRect(rr, shadowCornerRadius + grow * 0.25F)
                    Using sb As New SolidBrush(Color.FromArgb(a, 0, 0, 0))
                        g.FillPath(sb, gp)
                    End Using
                End Using
            Next
        End Sub


        Private Shared Sub DrawGloss(g As Graphics, facePath As GraphicsPath, faceRect As RectangleF, glossOpacity As Integer)
            Dim glossRect As New RectangleF(faceRect.X, faceRect.Y, faceRect.Width, faceRect.Height * 0.55F)
            Using gp As New GraphicsPath()
                gp.AddEllipse(glossRect)
                Using pth As New PathGradientBrush(gp)
                    pth.CenterColor = Color.FromArgb(Math.Max(0, Math.Min(255, glossOpacity)), 255, 255, 255)
                    pth.SurroundColors = {Color.FromArgb(0, 255, 255, 255)}
                    Dim gs As GraphicsState = g.Save()
                    Try
                        g.SetClip(facePath)
                        g.FillPath(pth, gp)
                    Finally
                        g.Restore(gs)
                    End Try
                End Using
            End Using
        End Sub

        Private Shared Sub DrawSymbol(g As Graphics, symbol As Bitmap, faceRect As RectangleF, st As TileStyle)
            If symbol Is Nothing Then Return

            ' Zielrechteck für das Symbol berechnen (Seitenverhältnis erhalten)
            Dim maxW As Single = faceRect.Width * st.SymbolScale
            Dim maxH As Single = faceRect.Height * st.SymbolScale
            Dim scale As Single = Math.Min(maxW / symbol.Width, maxH / symbol.Height)
            Dim w As Single = symbol.Width * scale
            Dim h As Single = symbol.Height * scale
            Dim x As Single = faceRect.X + (faceRect.Width - w) / 2.0F
            Dim y As Single = faceRect.Y + (faceRect.Height - h) / 2.0F
            Dim dest As New RectangleF(x, y, w, h)

            ' Symbol-Schatten (weiche Kante via leichte Mehrfachzeichnung)
#Disable Warning IDE0017 ' Initialisierung von Objekten vereinfachen
            If st.SymbolShadowOpacity > 0 Then
                Using ia As New ImageAttributes()
                    Dim cm As New ColorMatrix()
                    cm.Matrix33 = Math.Min(1.0F, st.SymbolShadowOpacity / 255.0F) ' Alpha
                    ia.SetColorMatrix(cm, ColorMatrixFlag.Default, ColorAdjustType.Bitmap)

                    Dim off As Single = Math.Max(1.0F, Math.Min(3.0F, Math.Min(dest.Width, dest.Height) / 120.0F * 2.0F))
                    Dim shadowRect As New RectangleF(dest.X + off, dest.Y + off, dest.Width, dest.Height)
                    g.DrawImage(symbol, Rectangle.Round(shadowRect), 0, 0, symbol.Width, symbol.Height, GraphicsUnit.Pixel, ia)
                End Using
            End If
#Enable Warning IDE0017 ' Initialisierung von Objekten vereinfachen

            ' Symbol selbst (hochqualitativ)
            Using ia2 As New ImageAttributes()
                ia2.SetWrapMode(WrapMode.TileFlipXY)
                g.DrawImage(symbol, Rectangle.Round(dest), 0, 0, symbol.Width, symbol.Height, GraphicsUnit.Pixel, ia2)
            End Using
        End Sub

        Private Shared Function MakeNoise(size As Size, opacity As Integer, Optional seed As Integer = 0) As Bitmap
            Dim bmp As New Bitmap(Math.Max(1, size.Width), Math.Max(1, size.Height), PixelFormat.Format32bppPArgb)
            Dim rnd As Random = If(seed = 0, New Random(), New Random(seed))
            Dim data As BitmapData = bmp.LockBits(New Rectangle(Point.Empty, bmp.Size), ImageLockMode.WriteOnly, bmp.PixelFormat)
            Try
                Dim stride As Integer = data.Stride
                Dim height As Integer = data.Height
                Dim width As Integer = data.Width
                Dim ptr As IntPtr = data.Scan0
                Dim bytes As Integer = Math.Abs(stride) * height
                Dim raw(bytes - 1) As Byte

                For y As Integer = 0 To height - 1
                    Dim row As Integer = y * stride
                    For x As Integer = 0 To width - 1
                        ' Subtiles, leicht warmes Rauschen
                        Dim n As Integer = rnd.Next(0, 256)
                        Dim a As Byte = CByte(Math.Max(0, Math.Min(255, opacity)))
                        raw(row + x * 4 + 0) = CByte(Math.Min(230 + ((n And &H1F) Mod 25), 255))  ' B
                        raw(row + x * 4 + 1) = CByte(Math.Min(232 + ((n >> 3) And &H1F), 255))     ' G
                        raw(row + x * 4 + 2) = CByte(Math.Min(235 + ((n >> 5) And &H1F), 255))     ' R
                        raw(row + x * 4 + 3) = a                                    ' A
                    Next
                Next

                Runtime.InteropServices.Marshal.Copy(raw, 0, ptr, bytes)
            Finally
                bmp.UnlockBits(data)
            End Try
            Return bmp
        End Function

        '
        ''' <summary>
        ''' Simuliert abgerundete Kanten der Oberseite (Face) durch innere Highlights und Schatten.
        ''' </summary>
        Private Shared Sub ApplyFaceRoundover(g As Graphics, facePath As GraphicsPath, faceRect As RectangleF,
                                      Optional edgeWidth As Single = 8.0F,
                                      Optional maxHighlight As Integer = 80,
                                      Optional maxShadow As Integer = 70)
            ' 1) Ambient-„Innenschatten“ rundum: dunkler Saum direkt an der Innenkante → wirkt gewölbt
            '    Center transparent, Rand leicht dunkel → Dunkel sitzt an den Kanten (innen).
            Using pgb As New PathGradientBrush(facePath)
                pgb.CenterColor = Color.FromArgb(0, 0, 0, 0)
                pgb.SurroundColors = {Color.FromArgb(Math.Max(0, Math.Min(255, CInt(maxShadow * 0.6))), 0, 0, 0)}
                ' FocusScales drücken den dunklen Bereich an die Kanten (je kleiner, desto schmaler der Saum)
                Dim fx As Single = Math.Max(0.0F, 1.0F - (edgeWidth / Math.Max(1.0F, faceRect.Width * 0.5F)) * 1.4F)
                Dim fy As Single = Math.Max(0.0F, 1.0F - (edgeWidth / Math.Max(1.0F, faceRect.Height * 0.5F)) * 1.4F)
                pgb.FocusScales = New PointF(fx, fy)
                g.FillPath(pgb, facePath)
            End Using

            ' 2) Richtungslicht: oben/links zarter „Glanzring“, unten/rechts zarter „Schattenring“
            '    Wir verwenden einen Inset-Pen und clippen auf Dreiecks-Halbräume (oben-links / unten-rechts).
            Dim w As Single = Math.Max(1.0F, edgeWidth) ' Ringbreite
            Using penHi As New Pen(Color.FromArgb(maxHighlight, 255, 255, 255), w),
          penSh As New Pen(Color.FromArgb(maxShadow, 0, 0, 0), Math.Max(1.0F, w * 0.9F))
                penHi.Alignment = PenAlignment.Inset
                penSh.Alignment = PenAlignment.Inset

                ' Top-Left-Clip für Highlight
                ' Richtungslicht – Highlight:
                Dim gs1 As GraphicsState = g.Save()
                Try
                    Using clipTL As Region = CreateDiagonalRegion(faceRect, topLeft:=True)
                        g.Clip = clipTL
                        g.DrawPath(penHi, facePath)
                    End Using
                Finally
                    g.Restore(gs1)
                End Try

                ' Bottom-Right-Clip für Schatten
                ' Richtungslicht – Schatten:
                Dim gs2 As GraphicsState = g.Save()
                Try
                    Using clipBR As Region = CreateDiagonalRegion(faceRect, topLeft:=False)
                        g.Clip = clipBR
                        g.DrawPath(penSh, facePath)
                    End Using
                Finally
                    g.Restore(gs2)
                End Try
            End Using

            ' 3) Feiner „Feather“-Saum für Extra-Rundheit (mehrere dünne, halbtransparente Inset-Strokes)
            Dim steps As Integer = Math.Max(3, CInt(edgeWidth / 2.0F))
            For i As Integer = 1 To steps
                Dim t As Single = i / CSng(steps)
                Dim aHi As Integer = CInt(maxHighlight * (1.0F - t) * 0.35F)
                Dim aSh As Integer = CInt(maxShadow * (1.0F - t) * 0.35F)
                Using penHi2 As New Pen(Color.FromArgb(aHi, 255, 255, 255), Math.Max(1.0F, w * (1.0F - t) * 0.6F)),
                  penSh2 As New Pen(Color.FromArgb(aSh, 0, 0, 0), Math.Max(1.0F, w * (1.0F - t) * 0.6F))
                    penHi2.Alignment = PenAlignment.Inset
                    penSh2.Alignment = PenAlignment.Inset
                    ' oben/links leicht verstärken
                    Using clipTL As Region = CreateDiagonalRegion(faceRect, topLeft:=True)
                        Dim gs2 As GraphicsState = g.Save()
                        g.DrawPath(penHi2, facePath)
                        g.Restore(gs2)
                    End Using
                    ' unten/rechts leicht verstärken
                    Using clipBR As Region = CreateDiagonalRegion(faceRect, topLeft:=False)
                        Dim gs1 As GraphicsState = g.Save()
                        g.DrawPath(penSh2, facePath)
                        g.Restore(gs1)
                    End Using
                End Using
            Next
        End Sub

        '
        ''' <summary>
        ''' Dreiecks-Halbraum als Clip: True = oben/links, False = unten/rechts (an der Diagonalen geteilt).
        ''' </summary>
        Private Shared Function CreateDiagonalRegion(r As RectangleF, topLeft As Boolean) As Region
            Dim gp As New Drawing2D.GraphicsPath()
            If topLeft Then
                ' Dreieck über der Hauptdiagonalen: (Left,Top)-(Right,Top)-(Left,Bottom)
                gp.AddPolygon({
            New PointF(r.Left, r.Top),
            New PointF(r.Right, r.Top),
            New PointF(r.Left, r.Bottom)
        })
            Else
                ' Gegendreieck: (Right,Bottom)-(Right,Top)-(Left,Bottom)
                gp.AddPolygon({
            New PointF(r.Right, r.Bottom),
            New PointF(r.Right, r.Top),
            New PointF(r.Left, r.Bottom)
        })
            End If
            Dim reg As New Region(gp)
            gp.Dispose()
            ' auf Face beschränken (Sicherheit, falls größer)
            reg.Intersect(New Region(r))
            Return reg
        End Function




        Private Shared Function InflateRoundedRect(r As RectangleF, inflate As Single, cornerRadius As Single) As GraphicsPath
            Dim rr As New RectangleF(r.X - inflate, r.Y - inflate, r.Width + 2 * inflate, r.Height + 2 * inflate)
            Return RoundedRect(rr, cornerRadius + inflate)
        End Function



    End Class

End Namespace

