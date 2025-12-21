Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

''' <summary>
''' Platziert Debug-Text-Rechtecke kollisionsfrei auf einer Zeichenfläche.
''' Pro Frame einmal Start(surface) aufrufen; anschließend für jedes Label
''' Request(preferredPoint, size) verwenden. Reservierte Bereiche werden gemerkt.
''' </summary>
Public NotInheritable Class DebugLabelPlacer

    Private _surface As Rectangle
    Private ReadOnly _occupied As New List(Of Rectangle)(64)

    ''' <summary>Horiz./Vert. Abstand zwischen Labels in Pixeln.</summary>
    Public Property Gap As Integer = 2

    ''' <summary>Zusätzlicher Rand an der rechten/unteren Fläche, bevor umgebrochen wird.</summary>
    Public Property Margin As Integer = 2

    ''' <summary>Setzt die Ziel-Zeichenfläche und leert die Belegung (pro Frame aufrufen).</summary>
    Public Sub Start(surface As Rectangle)
        _surface = surface
        _occupied.Clear()
    End Sub

    ''' <summary>Merkt einen bereits belegten Bereich vor (optional).</summary>
    Public Sub Reserve(area As Rectangle)
        If area.Width <= 0 OrElse area.Height <= 0 Then Exit Sub
        Dim r As Rectangle = ConfineToSurface(area)
        If r.Width > 0 AndAlso r.Height > 0 Then _occupied.Add(r)
    End Sub

    ''' <summary>
    ''' Liefert einen kollisionsfreien Ziel-Rect für ein Label.
    ''' Vorgehen: vom gewünschten Punkt aus zeilenweise nach unten,
    ''' danach in die nächste "Spalte" nach rechts usw.
    ''' </summary>
    Public Function Request(preferredTopLeft As Point, size As Size) As Rectangle
        Dim w As Integer = Math.Max(1, size.Width)
        Dim h As Integer = Math.Max(1, size.Height)

        ' Startposition in die Fläche zwingen
        Dim startX As Integer = Math.Max(_surface.Left, Math.Min(preferredTopLeft.X, _surface.Right - w))
        Dim startY As Integer = Math.Max(_surface.Top, Math.Min(preferredTopLeft.Y, _surface.Bottom - h))

        Dim x As Integer = startX
        Dim y As Integer = startY

        Dim stepY As Integer = h + Gap
        Dim stepX As Integer = w + Gap

        ' Einfache, robuste Suche: rasterartig nach unten, dann Spalte nach rechts.
        Do
            Dim cand As New Rectangle(x, y, w, h)
            cand = ConfineToSurface(cand)
            If cand.Width <= 0 OrElse cand.Height <= 0 Then
                ' Kein Platz mehr in dieser Spalte -> nächste Spalte
                x += stepX
                y = _surface.Top
                If x + w + Margin > _surface.Right Then Exit Do
                Continue Do
            End If

            If Not IntersectsAny(cand) Then
                _occupied.Add(cand)
                Return cand
            End If

            ' Nächste Zeile innerhalb der Spalte
            y += stepY
            If y + h + Margin > _surface.Bottom Then
                ' Umbruch in nächste Spalte
                x += stepX
                y = _surface.Top
                If x + w + Margin > _surface.Right Then Exit Do
            End If
        Loop

        ' Fallback: oben links in die Fläche (kann im Extremfall überlappen)
        Dim fb As New Rectangle(_surface.Left, _surface.Top, w, h)
        _occupied.Add(fb)
        Return fb
    End Function

    Private Function IntersectsAny(r As Rectangle) As Boolean
        For i As Integer = 0 To _occupied.Count - 1
            If _occupied(i).IntersectsWith(r) Then Return True
        Next
        Return False
    End Function

    Private Function ConfineToSurface(r As Rectangle) As Rectangle
        Dim x As Integer = Math.Max(_surface.Left, r.Left)
        Dim y As Integer = Math.Max(_surface.Top, r.Top)
        Dim right As Integer = Math.Min(_surface.Right, r.Right)
        Dim bottom As Integer = Math.Min(_surface.Bottom, r.Bottom)
        Dim w As Integer = Math.Max(0, right - x)
        Dim h As Integer = Math.Max(0, bottom - y)
        Return New Rectangle(x, y, w, h)
    End Function

End Class
