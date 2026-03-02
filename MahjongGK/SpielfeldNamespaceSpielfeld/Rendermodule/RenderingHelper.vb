Imports System.Drawing.Drawing2D

Namespace Spielfeld

    Module RenderingHelper
        ' ─────────────────────────────────────────────────────────────────────────────
        ' Kleiner, kontrastreicher Marker unten rechts als Debughilfe.
        ' Zeigt RenderingDoneCounter und RenderingSkipCounter
        ' (ändert NICHT den Backpuffer)
        ' wenn RenderingDoneCounter = 1, blitzt die Markierung rot auf.
        ' damit ist sichtbar, ob mindestens 1 Rendering durchgeführt wurde.
        ' ─────────────────────────────────────────────────────────────────────────────
        Private ReadOnly _penWhite As New Pen(Color.FromArgb(240, 255, 255, 255), 1.0F)
        Private ReadOnly _penBlack As New Pen(Color.FromArgb(240, 0, 0, 0), 1.0F)
        Private ReadOnly _brushDot As New SolidBrush(Color.FromArgb(220, 0, 0, 0))

        Public Sub DrawRenderingSkipDoneMarker(g As Graphics, surfaceW As Integer, surfaceH As Integer, counter As Integer, srcText As String, Optional scaled As Boolean = False)

            If surfaceW <= 100 OrElse surfaceH <= 100 Then Exit Sub

            Dim w As Integer = 130               ' Markergröße
            Dim h As Integer = 20
            Dim x As Integer = surfaceW - w - 10
            Dim y As Integer = surfaceH - h - 10

            Dim r As New Rectangle(x, y, w, h)

            ' Schnell zeichnen, keine Extras
            Dim oldSmoothing As SmoothingMode = g.SmoothingMode
            Dim oldPixel As PixelOffsetMode = g.PixelOffsetMode
            g.SmoothingMode = SmoothingMode.None
            g.PixelOffsetMode = PixelOffsetMode.Half

            ' Doppelrahmen (weiß außen, schwarz innen) -> auf jedem Hintergrund sichtbar
            g.DrawRectangle(_penWhite, r)
            Dim r2 As New Rectangle(r.X + 1, r.Y + 1, r.Width - 3, r.Height - 3)
            g.DrawRectangle(_penBlack, r2)
            r2.Width -= 1 : r2.Height -= 1

            ' Diagonaler Punkt (Mini-Dreieck) als Füllung
            '  • schnell, null Allokation, gut erkennbar
            Using fnt As New Font("Arial", 12, FontStyle.Regular, GraphicsUnit.Pixel)

                If srcText = "Done" Then 'alternativ If SFD.RenderingDoneCounter >= 1 Then
                    g.FillRectangle(Brushes.LightGreen, r2)
                Else
                    If scaled Then
                        g.FillRectangle(Brushes.Yellow, r2)
                    Else
                        g.FillRectangle(Brushes.Magenta, r2)
                    End If
                End If

                g.DrawString(SFD.AktRendering.ToString & " - " & srcText & " - " & counter.ToString, fnt, Brushes.Black, r.X + 4, r.Y + 2)

            End Using

            g.SmoothingMode = oldSmoothing
            g.PixelOffsetMode = oldPixel
        End Sub

        ''    Public Sub DrawRuntimeOnly_AktRendering(g As Graphics, surfaceW As Integer, surfaceH As Integer)
        ''        If surfaceW <= 100 OrElse surfaceH <= 100 Then Exit Sub


        ''        Dim w As Integer = 75               ' Markergröße
        ''        Dim h As Integer = 20
        ''        Dim x As Integer = surfaceW - w - 10
        ''        Dim y As Integer = 10

        ''        Dim r As New Rectangle(x, y, w, h)

        ''        Schnell zeichnen, keine Extras
        ''        Dim oldSmoothing As SmoothingMode = g.SmoothingMode
        ''        Dim oldPixel As PixelOffsetMode = g.PixelOffsetMode
        ''        g.SmoothingMode = SmoothingMode.None
        ''        g.PixelOffsetMode = PixelOffsetMode.Half

        ''        Doppelrahmen(weiß außen, schwarz innen) -> auf jedem Hintergrund sichtbar
        ''        g.DrawRectangle(_penWhite, r)
        ''        Dim r2 As New Rectangle(r.X + 1, r.Y + 1, r.Width - 2, r.Height - 2)
        ''        g.DrawRectangle(_penBlack, r2)

        ''        Diagonaler Punkt(Mini - Dreieck) als Füllung
        ''          • schnell, null Allokation, gut erkennbar
        ''        Using fnt As New Font("Arial", 12, FontStyle.Regular, GraphicsUnit.Pixel)

        ''            If SFD.RenderingDoneCounter >= 1 Then
        ''                g.FillRectangle(Brushes.LightGreen, r2)
        ''            Else
        ''                g.FillRectangle(Brushes.Magenta, r2)
        ''            End If

        ''            g.DrawString(INI.RuntimeOnly_AktRendering.ToString, fnt, Brushes.Black, r.X + 4, r.Y + 2)

        ''        End Using

        ''        g.SmoothingMode = oldSmoothing
        ''        g.PixelOffsetMode = oldPixel
        ''    End Sub

        ''' <summary>
        ''' Fehlende Werte werden aus der INI bezogen.
        ''' (INI.Editor_Message...)
        ''' </summary>
        ''' <param name="gfx"></param>
        ''' <param name="msg"></param>
        ''' <param name="rect"></param>
        Public Sub WriteMessageCentered(
            gfx As Graphics,
            msg As String,
            rect As Rectangle)
            WriteMessageCentered(gfx, msg, INI.Editor_MessageFont, INI.Editor_MessageAlpha, INI.Editor_MessageGray, rect)
        End Sub
        ''' <summary>
        ''' Schriftfarbe ist Schwarz, alpha und gray beziehen sich auf den Untergrund
        ''' </summary>
        ''' <param name="gfx"></param>
        ''' <param name="msg"></param>
        ''' <param name="fnt"></param>
        ''' <param name="alpha"></param>
        ''' <param name="gray"></param>
        ''' <param name="rect"></param>
        Public Sub WriteMessageCentered(
            gfx As Graphics,
            msg As String,
            fnt As Font,
            alpha As Integer,
            gray As Integer,
            rect As Rectangle)

            If gfx Is Nothing Then Throw New ArgumentNullException(NameOf(gfx))
            If fnt Is Nothing Then Throw New ArgumentNullException(NameOf(fnt))
            If msg Is Nothing Then msg = String.Empty

            If alpha < 0 OrElse alpha > 255 Then Throw New ArgumentOutOfRangeException(NameOf(alpha))
            If gray < 0 OrElse gray > 255 Then Throw New ArgumentOutOfRangeException(NameOf(gray))

            Dim backgroundColor As Color = Color.FromArgb(alpha, gray, gray, gray)
            Dim fontColor As Color = Color.FromArgb(255, Color.Black)

            ' Hintergrund
            Using bgBrush As New SolidBrush(backgroundColor)
                gfx.FillRectangle(bgBrush, rect)
            End Using

            ' Zentrierformat
            Using sf As New StringFormat()
                sf.Alignment = StringAlignment.Center
                sf.LineAlignment = StringAlignment.Center
                sf.FormatFlags = StringFormatFlags.NoWrap

                Using textBrush As New SolidBrush(fontColor)
                    gfx.TextRenderingHint = Drawing.Text.TextRenderingHint.ClearTypeGridFit
                    gfx.DrawString(msg, fnt, textBrush, rect, sf)
                End Using
            End Using

        End Sub

        Public Sub WriteMessageCentered(
            gfx As Graphics,
            msg As String,
            fnt As Font,
            fntColor As Color,
            bgColor As Color,
            rect As Rectangle)

            If gfx Is Nothing Then Throw New ArgumentNullException(NameOf(gfx))
            If fnt Is Nothing Then Throw New ArgumentNullException(NameOf(fnt))
            If msg Is Nothing Then msg = String.Empty

            ' Hintergrund
            Using bgBrush As New SolidBrush(bgColor)
                gfx.FillRectangle(bgBrush, rect)
            End Using

            ' Zentrierformat
            Using sf As New StringFormat()
                sf.Alignment = StringAlignment.Center
                sf.LineAlignment = StringAlignment.Center
                sf.FormatFlags = StringFormatFlags.NoWrap

                Using textBrush As New SolidBrush(fntColor)
                    gfx.TextRenderingHint = Drawing.Text.TextRenderingHint.ClearTypeGridFit
                    gfx.DrawString(msg, fnt, textBrush, rect, sf)
                End Using
            End Using

        End Sub
    End Module
End Namespace