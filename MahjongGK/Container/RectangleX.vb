Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

Imports System.Drawing.Drawing2D
Imports System.Globalization

Imports Grafix = System.Drawing.Graphics

'Zur Entstehung: Während ich das Layout in SpielfeldManager.CreateMainLayout entwickelt
'habe, ist mir die Notwendigkeit einer erweiterten Rectangle-Struktur klar geworden.
'Ergebnis ist diese Klasse. Entwicklungszeit von der Grundidee zur fertigen Klasse:
'Angefangen um 14:30 Uhr. Um 16 Uhr Denkpause mit Spaziergang zum Supermarkt Abendessen
'einkaufen, dann eine Stunde am Computer, anschließend Abendessen, dann wieder
'Computer. Fertig um 21:30. Die Klasse hat mit Kommentaren über 1.000 Zeilen.
'Ich habe so ein Tempo noch nicht erlebt.
'
'Wer es nachmachen will: Mit ChatGPT grundsätzliches Profil mit Vorstellungen und
'Anforderungen erarbeiten. Präzise beschreiben, was man haben will. Nicht zuviel auf
'einmal, ChatGPT vergisst sonst gerne einiges. ChatGPT macht selber Vorschläge,
'wenn man genau beschreibt, für was man die Klasse benötigt. Dann ist Entwicklung im
'Dialog möglich. Inzwischen merke ich an den Fehlern und Unsauberkeiten von ChatGPT
'wann Wissen "hinten rausfällt", dann lade ich die aktuelle Klasse wieder: "hier ist die
'aktuelle Version".
'Ich arbeite mit der kostenpflichtigen Version, mit der kostenlosen geht sowas nicht.
'Auf dem Laptop läuft eine weitere kostenlose Instanz um die Hauptinstanz nicht abzulenken. 

Public Class RectangleX

    '///////////////////////////////////////////////////////////////////////////////
    ' RectangleX
    '   - Erweiterung von System.Drawing.Rectangle mit Padding-Unterstützung
    '   - Verwendung wie ein normales Rectangle (X, Y, Width, Height, Location, Size)
    '   - Zusätzliche "Inside"-Properties berücksichtigen Padding (XInside, WidthInside, …)
    '
    ' Wichtigste Funktionen / Properties:
    '
    ' Konstruktoren:
    '   New(x, y, width, height [, paddingAll / padding As PaddingValues])
    '   New(rect As Rectangle [, paddingAll / padding As PaddingValues])
    '
    ' Factory:
    '   FromRectangle(rect As Rectangle [, paddingAll As Integer / padding As PaddingValues])
    '
    ' Operatoren:
    '   CType(rect As Rectangle) As RectangleX   ' implizit Rectangle -> RectangleX (Padding=0)
    '   CType(rx As RectangleX) As Rectangle     ' implizit RectangleX -> Rectangle
    '
    ' Dim sysRect As New Rectangle(10, 20, 200, 100)
    '
    '   1) Normale Operator-Umwandlung (Padding = 0)
    ' Dim rx1 As RectangleX = sysRect
    '
    '   2) Factory mit Padding.All = 5
    ' Dim rx2 As RectangleX = RectangleX.FromRectangle(sysRect, 5)
    '
    '   3) Factory mit individuellem Padding
    ' Dim rx3 As RectangleX = RectangleX.FromRectangle(sysRect,
    '                  New RectangleX.PaddingValues(5, 10, 5, 10))
    '
    'Veränderungs-Helfer:
    '       Inc*/Dec* für X, Y, Left, Top, Right, Bottom (Optional Schrittweite)
    '       GrowAll / ShrinkAll für gleichmäßiges Vergrößern/Verkleinern

    ' Properties:
    '   X, Y, Width, Height, Left, Top, Right, Bottom
    '   Location, Size
    '   ... jeweils auch als Inside-Variante (mit Padding)
    '   Padding As PaddingValues
    '
    ' Methoden – Ableitungen:
    '   GetRectangleInside(marginAll As Integer [, usePadding As Boolean])
    '   GetRectangleInside(left, right, top, bottom [, usePadding As Boolean])
    '   GetRectangleInside(width, height [, usePadding As Boolean])
    '   GetRectangleXInside(...)  ' wie oben, aber liefert RectangleX (mit Padding)
    '
    ' Zeichenmethoden:
    '   DrawRectangle(gfx, color, borderWidth [, usePadding])
    '   FillRectangle(gfx, color [, usePadding])
    '   DrawImage(gfx, bmp [, usePadding])
    '   DrawImageBuffered(gfx, bgc [, backgroundColor, usePadding])
    '   DrawString(gfx, text, font [, foreColor, usePadding])
    '   DrawStringCentered(...)
    '   DrawString(..., horizontalAlignment, verticalAlignment [, usePadding])
    '
    ' Utility:
    '   DeepCopy() As RectangleX
    '   ToXmlDataString() As String         ' z. B. "X,Y,W,H|PL,PT,PR,PB"
    '   FromXmlDataString(s As String)      ' rekonstruiert RectangleX aus String
    '
    ' Debugfunktionen (reversibles Overlay):
    '   • SetDebugHost(host As Control, Optional backColor As Color? = Nothing)
    '       – Muss einmal gesetzt werden (für Screen-Koordinaten via ControlPaint).
    '   • DrawContentDebug(doDraw As Boolean, style As Integer)
    '   • DrawBoundsDebug(doDraw As Boolean, style As Integer)
    '       – doDraw: globales INI-Flag vom SpielfeldManager
    '       – style: Linienmuster (8 Muster, zyklisch: style Mod 8), 1-px Rahmen
    '   • DrawDebugExclusive(Optional usePadding As Boolean? = Nothing,
    '                        Optional styleOverride As Integer? = Nothing,
    '                        Optional drawBounds As Boolean? = Nothing,
    '                        Optional drawContent As Boolean? = Nothing)
    '       – Zeichnet nur den Debug-Rahmen (z. B. für Platzhalter ohne normale Ausgabe)
    '   • ResetDebug() – setzt alle Debug-Einstellungen der Instanz zurück
    '
    ' Verhalten:
    '   • Wenn aktiviert, wird der XOR-ähnliche Rahmen am ENDE jeder Zeichenmethode
    '     (DrawRectangle/FillRectangle/DrawImage/…/DrawString) automatisch gezeichnet.
    '   • Reversibel: erneuter Aufruf mit gleichen Kanten löscht den Rahmen wieder.
    '   • Arbeitet in Screen-Koordinaten (ControlPaint.DrawReversibleLine).
    '
    ' Typischer Ablauf:
    '   • Beim Init/Resize/EditMode: SetDebugHost(Me),
    '       DrawContentDebug(INI.DebugLayoutContent, style1),
    '       DrawBoundsDebug(INI.DebugLayoutBounds, style2)
    '   • Im Paint: normale Zeichenaufrufe – Debugrahmen kommt automatisch hinterher.
    '   • Für Platzhalter: DrawDebugExclusive() verwenden.

    '///////////////////////////////////////////////////////////////////////////////


    'Für eigenständige Verwendung in anderen Projekten ggf auf True setzten
    'um die (vereinfachte) eingebaute Verkleinerungsmethode zu verwenden.
#Const INCLUDE_Shrinkbitmap = False

#Region "PaddingValues"

    ''' <summary>
    ''' ' RectangleX
    '''   - Rechteck wie System.Drawing.Rectangle, erweitert um Padding
    '''   - Umwandlung Rectangle RectangleX per Operator CType
    '''   - Inside-Properties berücksichtigen Padding (XInside, WidthInside, …)
    '''   - Ableitungen: GetRectangleInside(...) / GetRectangleXInside(...)
    '''   - Zeichenmethoden: DrawRectangle, FillRectangle, DrawImage, DrawString, …
    '''   - Utility: DeepCopy, ToDataString, FromDataString
    '''   - Inc*/Dec* für X, Y, Left, Top, Right, Bottom (Optional Schrittweite)
    '''   - GrowAll / ShrinkAll für gleichmäßiges Vergrößern/Verkleinern
    ''' </summary>
    Public Structure PaddingValues
        Public Property Left As Integer
        Public Property Top As Integer
        Public Property Right As Integer
        Public Property Bottom As Integer

        Public Sub New(all As Integer)
            Left = all : Top = all : Right = all : Bottom = all
        End Sub

        Public Sub New(left As Integer, top As Integer, right As Integer, bottom As Integer)
            Me.Left = left : Me.Top = top : Me.Right = right : Me.Bottom = bottom
        End Sub

        Public Property All As Integer
            Get
                If Left = Top AndAlso Top = Right AndAlso Right = Bottom Then Return Left
                Return 0
            End Get
            Set(value As Integer)
                Left = value : Top = value : Right = value : Bottom = value
            End Set
        End Property

        Public Function Horizontal() As Integer
            Return Left + Right
        End Function
        Public Function Vertical() As Integer
            Return Top + Bottom
        End Function

        Public Overrides Function ToString() As String
            Return $"L{Left},T{Top},R{Right},B{Bottom}"
        End Function
    End Structure

#End Region

#Region "Basis + Padding"

    Private _x As Integer
    Private _y As Integer
    Private _w As Integer
    Private _h As Integer

    Public Property Padding As PaddingValues

    ' ─────────────────────────────────────────────────────────────────────────────
    ' Zentrale Schaltstelle für die Zeichnen-Fläche (Bounds vs. Content/Padding)
    ' ─────────────────────────────────────────────────────────────────────────────

    ' Default: ohne Padding zeichnen
    Private _usePaddingForDrawing As Boolean = False

    ''' <summary>
    ''' Globaler Default dieser Instanz: True = ContentRect (mit Padding), False = BoundsRect.
    ''' Alle Zeichenmethoden verwenden diesen Default, wenn kein Override übergeben wird.
    ''' </summary>
    Public Property UsePaddingForDrawing As Boolean
        Get
            Return _usePaddingForDrawing
        End Get
        Set(value As Boolean)
            _usePaddingForDrawing = value
        End Set
    End Property

    ''' <summary>Fluent: Setzt Zeichnen auf ContentRect (mit Padding) und gibt Me zurück.</summary>
    Public Function UsePadding() As RectangleX
        _usePaddingForDrawing = True
        Return Me
    End Function

    ''' <summary>Fluent: Setzt Zeichnen auf BoundsRect (ohne Padding) und gibt Me zurück.</summary>
    Public Function UseBounds() As RectangleX
        _usePaddingForDrawing = False
        Return Me
    End Function

#End Region

#Region "Konstruktoren (wie Rectangle) + Overloads mit Padding"

    Public Sub New()
        Me.New(0, 0, 0, 0)
    End Sub

    Public Sub New(x As Integer, y As Integer, width As Integer, height As Integer)
        _x = x : _y = y : _w = width : _h = height
        Padding = New PaddingValues(0)
    End Sub

    ' + paddingAll
    Public Sub New(x As Integer, y As Integer, width As Integer, height As Integer, paddingAll As Integer)
        Me.New(x, y, width, height)
        Me.Padding = New PaddingValues(paddingAll)
    End Sub

    ' + PaddingValues
    Public Sub New(x As Integer, y As Integer, width As Integer, height As Integer, padding As PaddingValues)
        Me.New(x, y, width, height)
        Me.Padding = padding
    End Sub

    Public Sub New(location As Point, size As Size)
        Me.New(location.X, location.Y, size.Width, size.Height)
    End Sub

    Public Sub New(location As Point, size As Size, paddingAll As Integer)
        Me.New(location.X, location.Y, size.Width, size.Height)
        Me.Padding = New PaddingValues(paddingAll)
    End Sub

    Public Sub New(location As Point, size As Size, padding As PaddingValues)
        Me.New(location.X, location.Y, size.Width, size.Height)
        Me.Padding = padding
    End Sub

    Public Sub New(rect As Rectangle)
        Me.New(rect.X, rect.Y, rect.Width, rect.Height)
    End Sub

    Public Sub New(rect As Rectangle, paddingAll As Integer)
        Me.New(rect)
        Me.Padding = New PaddingValues(paddingAll)
    End Sub

    Public Sub New(rect As Rectangle, padding As PaddingValues)
        Me.New(rect)
        Me.Padding = padding
    End Sub

#End Region

#Region "Wie Rectangle – Properties & Konvertierung"

    Public Property X As Integer
        Get
            Return _x
        End Get
        Set(value As Integer)
            _x = value
        End Set
    End Property

    Public Property Y As Integer
        Get
            Return _y
        End Get
        Set(value As Integer)
            _y = value
        End Set
    End Property
    Public Property Left As Integer
        Get
            Return _x
        End Get
        Set(value As Integer)
            _x = value
        End Set
    End Property

    Public Property Top As Integer
        Get
            Return _y
        End Get
        Set(value As Integer)
            _y = value
        End Set
    End Property
    Public Property Width As Integer
        Get
            Return _w
        End Get
        Set(value As Integer)
            _w = value
        End Set
    End Property

    Public Property Height As Integer
        Get
            Return _h
        End Get
        Set(value As Integer)
            _h = value
        End Set
    End Property

    Public Property Right As Integer
        Get
            Return _x + _w
        End Get
        Set(value As Integer)
            _w = value - _x
        End Set
    End Property

    Public Property Bottom As Integer
        Get
            Return _y + _h
        End Get
        Set(value As Integer)
            _h = value - _y
        End Set
    End Property

    Public Property Location As Point
        Get
            Return New Point(_x, _y)
        End Get
        Set(value As Point)
            _x = value.X
            _y = value.Y
        End Set
    End Property

    Public Property Size As Size
        Get
            Return New Size(_w, _h)
        End Get
        Set(value As Size)
            _w = value.Width
            _h = value.Height
        End Set
    End Property

    '--- Varianten Inside (berücksichtigen Padding => ContentRect) ---

    Public ReadOnly Property XInside As Integer
        Get
            Return ContentRect.X
        End Get
    End Property

    Public ReadOnly Property YInside As Integer
        Get
            Return ContentRect.Y
        End Get
    End Property
    Public ReadOnly Property LeftInside As Integer
        Get
            Return ContentRect.Left
        End Get
    End Property

    Public ReadOnly Property TopInside As Integer
        Get
            Return ContentRect.Top
        End Get
    End Property

    Public ReadOnly Property WidthInside As Integer
        Get
            Return ContentRect.Width
        End Get
    End Property

    Public ReadOnly Property HeightInside As Integer
        Get
            Return ContentRect.Height
        End Get
    End Property

    Public ReadOnly Property RightInside As Integer
        Get
            Return ContentRect.Right
        End Get
    End Property

    Public ReadOnly Property BottomInside As Integer
        Get
            Return ContentRect.Bottom
        End Get
    End Property

    Public ReadOnly Property LocationInside As Point
        Get
            Return ContentRect.Location
        End Get
    End Property

    Public ReadOnly Property SizeInside As Size
        Get
            Return ContentRect.Size
        End Get
    End Property

    Public Function ToRectangle() As Rectangle
        Return New Rectangle(_x, _y, _w, _h)
    End Function

    Public Shared Widening Operator CType(rx As RectangleX) As Rectangle
        If rx Is Nothing Then
            Return Rectangle.Empty
        End If
        Return rx.ToRectangle()
    End Operator

    Public Shared Widening Operator CType(rect As Rectangle) As RectangleX
        Return New RectangleX(rect)
    End Operator

#End Region

#Region "Increment/Decrement / Grow/Shrink Helfer"

    '--- X / Left ---
    Public Sub IncX(Optional n As Integer = 1)
        _x += n
    End Sub

    Public Sub DecX(Optional n As Integer = 1)
        _x -= n
    End Sub

    Public Sub IncLeft(Optional n As Integer = 1)
        _x += n
        _w = Math.Max(0, _w - n)   ' Breite darf nicht negativ werden
    End Sub

    Public Sub DecLeft(Optional n As Integer = 1)
        _x -= n
        _w += n
    End Sub

    '--- Y / Top ---
    Public Sub IncY(Optional n As Integer = 1)
        _y += n
    End Sub

    Public Sub DecY(Optional n As Integer = 1)
        _y -= n
    End Sub

    Public Sub IncTop(Optional n As Integer = 1)
        _y += n
        _h = Math.Max(0, _h - n)   ' Höhe darf nicht negativ werden
    End Sub

    Public Sub DecTop(Optional n As Integer = 1)
        _y -= n
        _h += n
    End Sub

    '--- Right ---
    Public Sub IncRight(Optional n As Integer = 1)
        _w += n
    End Sub

    Public Sub DecRight(Optional n As Integer = 1)
        _w = Math.Max(0, _w - n)
    End Sub

    '--- Bottom ---
    Public Sub IncBottom(Optional n As Integer = 1)
        _h += n
    End Sub

    Public Sub DecBottom(Optional n As Integer = 1)
        _h = Math.Max(0, _h - n)
    End Sub

    '--- All ---
    ' ShrinkAll: alle Seiten +n nach innen -> kleineres Rechteck
    ' GrowAll:   alle Seiten -n nach außen -> größeres Rechteck
    Public Sub ShrinkAll(Optional n As Integer = 1)
        _x += n : _y += n
        _w = Math.Max(0, _w - 2 * n)
        _h = Math.Max(0, _h - 2 * n)
    End Sub

    Public Sub GrowAll(Optional n As Integer = 1)
        _x -= n : _y -= n
        _w += 2 * n
        _h += 2 * n
    End Sub

#End Region

#Region "Factory-Helper"

    ''' <summary>
    ''' Baut aus einem Rectangle ein RectangleX mit optionalem Padding.All.
    ''' </summary>
    Public Shared Function FromRectangle(rect As Rectangle,
                                         Optional paddingAll As Integer = 0) As RectangleX
        Dim rx As New RectangleX(rect)
        If paddingAll <> 0 Then
            rx.Padding = New PaddingValues(paddingAll)
        End If
        Return rx
    End Function

    ''' <summary>
    ''' Baut aus einem Rectangle ein RectangleX mit explizitem Padding.
    ''' </summary>
    Public Shared Function FromRectangle(rect As Rectangle,
                                         padding As PaddingValues) As RectangleX
        Dim rx As New RectangleX(rect)
        rx.Padding = padding
        Return rx
    End Function

#End Region


#Region "Abgeleitete Rechtecke"

    ''' <summary>Außere Abmessungen ohne Padding.</summary>
    Public ReadOnly Property BoundsRect As Rectangle
        Get
            Return New Rectangle(_x, _y, _w, _h)
        End Get
    End Property

    ''' <summary>Innenbereich unter Berücksichtigung von Padding (nicht negativ).</summary>
    Public ReadOnly Property ContentRect As Rectangle
        Get
            Dim cx As Integer = _x + Padding.Left
            Dim cy As Integer = _y + Padding.Top
            Dim cw As Integer = Math.Max(0, _w - Padding.Horizontal())
            Dim ch As Integer = Math.Max(0, _h - Padding.Vertical())
            Return New Rectangle(cx, cy, cw, ch)
        End Get
    End Property

    ''' <summary>
    ''' Auswahl der Zeichenfläche: Nothing ⇒ Instanz-Default (UsePaddingForDrawing),
    ''' True ⇒ ContentRect, False ⇒ BoundsRect.
    ''' </summary>
    Private Function GetBaseRect(Optional usePadding As Boolean? = Nothing) As Rectangle
        Dim usePad As Boolean = If(usePadding.HasValue, usePadding.Value, _usePaddingForDrawing)
        Return If(usePad, ContentRect, BoundsRect)
    End Function

#End Region

#Region "Ableitungen: Rectangle (bestehend)"

    Public Function GetRectangleInside(marginAll As Integer,
                                       Optional usePadding As Boolean = False) As Rectangle
        Return GetRectangleInside(marginAll, marginAll, marginAll, marginAll, usePadding)
    End Function

    Public Function GetRectangleInside(marginLeft As Integer,
                                       marginRight As Integer,
                                       marginTop As Integer,
                                       marginBottom As Integer,
                                       Optional usePadding As Boolean = False) As Rectangle
        Dim r As Rectangle = GetBaseRect(usePadding)
        Dim nx As Integer = r.X + marginLeft
        Dim ny As Integer = r.Y + marginTop
        Dim nw As Integer = Math.Max(0, r.Width - Math.Max(0, marginLeft) - Math.Max(0, marginRight))
        Dim nh As Integer = Math.Max(0, r.Height - Math.Max(0, marginTop) - Math.Max(0, marginBottom))
        Return New Rectangle(nx, ny, nw, nh)
    End Function

    Public Function GetRectangleInside(width As Integer,
                                       height As Integer,
                                       Optional usePadding As Boolean = False) As Rectangle
        Dim baseRect As Rectangle = GetBaseRect(usePadding)
        If baseRect.Width <= 0 OrElse baseRect.Height <= 0 Then
            Return New Rectangle(baseRect.X, baseRect.Y, Math.Max(0, baseRect.Width), Math.Max(0, baseRect.Height))
        End If

        Dim reqW As Integer = If(width <= 0, baseRect.Width, width)
        Dim reqH As Integer = If(height <= 0, baseRect.Height, height)

        Dim tgtW As Integer = Math.Min(reqW, baseRect.Width)
        Dim tgtH As Integer = Math.Min(reqH, baseRect.Height)

        Dim nx As Integer = baseRect.X + (baseRect.Width - tgtW) \ 2
        Dim ny As Integer = baseRect.Y + (baseRect.Height - tgtH) \ 2

        Return New Rectangle(nx, ny, tgtW, tgtH)
    End Function

#End Region

#Region "Ableitungen: RectangleX (mit Padding-Übernahme)"

    ' Hinweis: paddings := Nothing -> aktuelles Padding übernehmen.
    ' In VB: Nullable(Of PaddingValues) geht mit der Kurzform "PaddingValues?".
    Private Shared Function ResolvePaddings(current As PaddingValues, paddings As System.Nullable(Of PaddingValues)) As PaddingValues
        If paddings.HasValue Then Return paddings.Value
        Return current
    End Function

    Public Function GetRectangleXInside(marginAll As Integer,
                                        Optional usePadding As Boolean = False,
                                        Optional paddings As System.Nullable(Of PaddingValues) = Nothing) As RectangleX
        Dim r As Rectangle = GetRectangleInside(marginAll, usePadding)
        Dim rx As New RectangleX(r)
        rx.Padding = ResolvePaddings(Me.Padding, paddings)
        Return rx
    End Function

    Public Function GetRectangleXInside(marginLeft As Integer,
                                        marginRight As Integer,
                                        marginTop As Integer,
                                        marginBottom As Integer,
                                        Optional usePadding As Boolean = False,
                                        Optional paddings As System.Nullable(Of PaddingValues) = Nothing) As RectangleX
        Dim r As Rectangle = GetRectangleInside(marginLeft, marginRight, marginTop, marginBottom, usePadding)
        Dim rx As New RectangleX(r)
        rx.Padding = ResolvePaddings(Me.Padding, paddings)
        Return rx
    End Function

    Public Function GetRectangleXInside(width As Integer,
                                        height As Integer,
                                        Optional usePadding As Boolean = False,
                                        Optional paddings As System.Nullable(Of PaddingValues) = Nothing) As RectangleX
        Dim r As Rectangle = GetRectangleInside(width, height, usePadding)
        Dim rx As New RectangleX(r)
        rx.Padding = ResolvePaddings(Me.Padding, paddings)
        Return rx
    End Function

#End Region

#Region "Zeichenmethoden"

#Region "Zeichenmethoden"

    Public Sub DrawRectangle(gfx As Grafix,
                         color As Color,
                         borderWidth As Integer,
                         Optional usePadding As Boolean? = Nothing)
        If gfx Is Nothing OrElse borderWidth <= 0 Then Exit Sub
        Dim r As Rectangle = GetBaseRect(usePadding)
        If r.Width <= 0 OrElse r.Height <= 0 Then Exit Sub

        Using pen As New Pen(color, CSng(borderWidth))
            pen.Alignment = PenAlignment.Inset
            gfx.SmoothingMode = SmoothingMode.AntiAlias
            gfx.DrawRectangle(pen, r)
        End Using

        InternalMaybeDebugDraw()
    End Sub

    Public Sub FillRectangle(gfx As Grafix,
                         color As Color,
                         Optional usePadding As Boolean? = Nothing)
        If gfx Is Nothing Then Exit Sub
        Dim r As Rectangle = GetBaseRect(usePadding)
        If r.Width <= 0 OrElse r.Height <= 0 Then Exit Sub

        Using br As New SolidBrush(color)
            gfx.FillRectangle(br, r)
        End Using

        InternalMaybeDebugDraw()
    End Sub

    Public Sub DrawImage(gfx As Grafix,
                     bmp As Bitmap,
                     Optional usePadding As Boolean? = Nothing)
        If gfx Is Nothing OrElse bmp Is Nothing Then Exit Sub
        Dim r As Rectangle = GetBaseRect(usePadding)
        If r.Width <= 0 OrElse r.Height <= 0 Then Exit Sub

        If bmp.Width <> r.Width OrElse bmp.Height <> r.Height Then

#If INCLUDE_Shrinkbitmap Then
            ShrinkBitmap(bmp, r.Width, r.Height, disposeSrc:=True, highQuality:=True, cvtToARGBBitmap:=True)
#Else
            MjGDI.ShrinkBitmap(bmp, r.Width, r.Height, disposeSrc:=True, highQuality:=True, cvtToARGBBitmap:=True)
#End If
        End If

        gfx.DrawImageUnscaled(bmp, r.Location)
        InternalMaybeDebugDraw()
    End Sub

    Public Sub DrawImageBuffered(gfx As Grafix,
                             bgc As BackgroundSingleImageCache,
                             Optional backgroundColor As Color? = Nothing,
                             Optional usePadding As Boolean? = Nothing)
        If gfx Is Nothing Then Exit Sub
        Dim r As Rectangle = GetBaseRect(usePadding)
        If r.Width <= 0 OrElse r.Height <= 0 Then Exit Sub

        Dim bmp As Bitmap = Nothing
        If bgc IsNot Nothing Then
            bmp = bgc.GetBitmap(r.Size, If(backgroundColor.HasValue, CType(backgroundColor, Color), CType(Nothing, Color?)))
        End If

        If bmp Is Nothing Then
            If backgroundColor.HasValue Then
                Using br As New SolidBrush(backgroundColor.Value)
                    gfx.FillRectangle(br, r)
                End Using
            End If
            Exit Sub
        End If

        If bmp.Width <> r.Width OrElse bmp.Height <> r.Height Then

#If INCLUDE_Shrinkbitmap Then
            ShrinkBitmap(bmp, r.Width, r.Height, disposeSrc:=True, highQuality:=True, cvtToARGBBitmap:=True)
#Else
            MjGDI.ShrinkBitmap(bmp, r.Width, r.Height, disposeSrc:=True, highQuality:=True, cvtToARGBBitmap:=True)
#End If
        End If

        gfx.DrawImageUnscaled(bmp, r.Location)
        InternalMaybeDebugDraw()
    End Sub

    ' Wrapper ohne eigenen Debug-Hook (die Langform macht das bereits)
    Public Sub DrawString(gfx As Grafix,
                      text As String,
                      fnt As Font,
                      Optional foreColor As Color = Nothing,
                      Optional usePadding As Boolean? = Nothing)
        DrawString(gfx, text, fnt, foreColor,
               horizontalAlignment:=StringAlignment.Near,
               verticalAlignment:=StringAlignment.Near,
               usePadding:=usePadding)
    End Sub

    Public Sub DrawStringCentered(gfx As Grafix,
                              text As String,
                              fnt As Font,
                              Optional foreColor As Color = Nothing,
                              Optional usePadding As Boolean? = Nothing)
        DrawString(gfx, text, fnt, foreColor,
               horizontalAlignment:=StringAlignment.Center,
               verticalAlignment:=StringAlignment.Center,
               usePadding:=usePadding)
    End Sub

    Public Sub DrawString(gfx As Grafix,
                      text As String,
                      fnt As Font,
                      Optional foreColor As Color = Nothing,
                      Optional horizontalAlignment As StringAlignment = StringAlignment.Near,
                      Optional verticalAlignment As StringAlignment = StringAlignment.Near,
                      Optional usePadding As Boolean? = Nothing)
        If gfx Is Nothing OrElse fnt Is Nothing OrElse String.IsNullOrEmpty(text) Then Exit Sub
        Dim r As Rectangle = GetBaseRect(usePadding)
        If r.Width <= 0 OrElse r.Height <= 0 Then Exit Sub

        If foreColor = Nothing Then foreColor = Color.Black
        Using br As New SolidBrush(foreColor)
            Dim fmt As New StringFormat(StringFormatFlags.NoClip)
            fmt.Alignment = horizontalAlignment
            fmt.LineAlignment = verticalAlignment
            fmt.Trimming = StringTrimming.EllipsisCharacter
            gfx.TextRenderingHint = Drawing.Text.TextRenderingHint.ClearTypeGridFit
            gfx.DrawString(text, fnt, br, RectangleF.op_Implicit(r), fmt)
        End Using

        InternalMaybeDebugDraw()
    End Sub

#End Region

#End Region

#Region "DeepCopy & String-Pack/Unpack (schlank)"

    Public Function DeepCopy() As RectangleX
        Dim copy As New RectangleX(_x, _y, _w, _h)
        copy.Padding = New PaddingValues(Padding.Left, Padding.Top, Padding.Right, Padding.Bottom)
        Return copy

    End Function

    ''' <summary>Gibt kompakten String: X,Y,W,H|PL,PT,PR,PB</summary>
    Public Overrides Function ToString() As String
        Return ToXmlDataString()
    End Function

    Public Function ToXmlDataString() As String
        Dim ci As CultureInfo = Globalization.CultureInfo.InvariantCulture
        Return String.Format(ci, "{0},{1},{2},{3}|{4},{5},{6},{7}",
                             _x, _y, _w, _h,
                             Padding.Left, Padding.Top, Padding.Right, Padding.Bottom)
    End Function

    Public Shared Function FromXmlDataString(s As String) As RectangleX
        Dim rx As RectangleX = Nothing
        If Not TryParseXmlDataString(s, rx) Then Return Nothing
        Return rx
    End Function

    Public Shared Function TryParseXmlDataString(s As String, ByRef result As RectangleX) As Boolean

        result = Nothing

        If String.IsNullOrWhiteSpace(s) Then
            Return False
        End If

        Dim parts() As String = s.Trim().Split("|"c)
        If parts.Length <> 2 Then
            Return False
        End If

        Dim x As Integer, y As Integer, w As Integer, h As Integer
        Dim pl As Integer, pt As Integer, pr As Integer, pb As Integer
        If Not Parse4(parts(0), x, y, w, h) Then
            Return False
        End If
        If Not Parse4(parts(1), pl, pt, pr, pb) Then
            Return False
        End If

        Dim rx As New RectangleX(x, y, w, h)
        rx.Padding = New PaddingValues(pl, pt, pr, pb)
        result = rx

        Return True

    End Function

    Private Shared Function Parse4(csv As String,
                                   ByRef a As Integer, ByRef b As Integer,
                                   ByRef c As Integer, ByRef d As Integer) As Boolean
        If csv Is Nothing Then
            Return False
        End If
        Dim p As String() = csv.Split(","c)
        If p.Length <> 4 Then
            Return False
        End If
        Dim ci As CultureInfo = Globalization.CultureInfo.InvariantCulture
        Return Integer.TryParse(p(0), Globalization.NumberStyles.Integer, ci, a) AndAlso
               Integer.TryParse(p(1), Globalization.NumberStyles.Integer, ci, b) AndAlso
               Integer.TryParse(p(2), Globalization.NumberStyles.Integer, ci, c) AndAlso
               Integer.TryParse(p(3), Globalization.NumberStyles.Integer, ci, d)
    End Function

#End Region

#Region "Debughilfe"

    ' Reversibles (XOR-ähnliches) Zeichnen über ControlPaint.* in Screen-Koordinaten.
    ' Achtung: funktioniert außerhalb DoubleBuffering, ideal als temporäres Debug-Overlay.

    Private _dbgHost As Control = Nothing
    Private _dbgBackColor As Color? = Nothing
    Private _dbgDrawContent As Boolean = False
    Private _dbgStyleContent As Integer = 0
    Private _dbgDrawBounds As Boolean = False
    Private _dbgStyleBounds As Integer = 0

    ''' <summary>Host-Control für Screen-Koordinaten setzen; BackColor optional überschreiben.</summary>
    Public Sub SetDebugHost(host As Control, Optional backColor As Color? = Nothing)
        _dbgHost = host
        _dbgBackColor = backColor
    End Sub

    ''' <summary>Debug-Rahmen für ContentRect (mit Padding) aktivieren/parametrieren.</summary>
    Public Sub DrawContentDebug(doDraw As Boolean, style As Integer)
        _dbgDrawContent = doDraw
        _dbgStyleContent = style
    End Sub

    ''' <summary>Debug-Rahmen für BoundsRect (ohne Padding) aktivieren/parametrieren.</summary>
    Public Sub DrawBoundsDebug(doDraw As Boolean, style As Integer)
        _dbgDrawBounds = doDraw
        _dbgStyleBounds = style
    End Sub

    ''' <summary>Alle Debug-Einstellungen dieser Instanz zurücksetzen.</summary>
    Public Sub ResetDebug()
        _dbgDrawContent = False : _dbgStyleContent = 0
        _dbgDrawBounds = False : _dbgStyleBounds = 0
        _dbgBackColor = Nothing
    End Sub

    ' Acht zyklische 1-Bit-Linienmuster (style darf beliebig groß/negativ sein)
    Private Shared ReadOnly _dbgLinePatterns As Byte() = {
        &HFF,  ' 11111111  (durchgehend)
        &HF0,  ' 11110000
        &HCC,  ' 11001100
        &HAA,  ' 10101010  (gepunktet)
        &HEE,  ' 11101110
        &H11,  ' 00010001
        &HBD,  ' 10111101
        &HE7   ' 11100111
        }

    ' Am Ende jeder Zeichenmethode aufrufen.
    Private Sub InternalMaybeDebugDraw()
        If _dbgHost Is Nothing Then Exit Sub

        If _dbgDrawBounds Then
            Dim rb As Rectangle = BoundsRect
            If rb.Width > 0 AndAlso rb.Height > 0 Then
                DrawReversibleRectWithPattern(_dbgHost.RectangleToScreen(rb), _dbgStyleBounds)
            End If
        End If

        If _dbgDrawContent Then
            Dim rc As Rectangle = ContentRect
            If rc.Width > 0 AndAlso rc.Height > 0 Then
                DrawReversibleRectWithPattern(_dbgHost.RectangleToScreen(rc), _dbgStyleContent)
            End If
        End If
    End Sub

    ' Zeichnet einen 1-px-Rahmen mit Muster in Screen-Koordinaten; zweiter Aufruf löscht ihn wieder.
    Private Sub DrawReversibleRectWithPattern(screenRect As Rectangle, style As Integer)
        Dim backCol As Color = If(_dbgBackColor.HasValue, _dbgBackColor.Value,
                              If(_dbgHost IsNot Nothing, _dbgHost.BackColor, SystemColors.Control))

        Dim pat As Byte = _dbgLinePatterns(Math.Abs(style) Mod _dbgLinePatterns.Length)

        Dim w As Integer = Math.Max(1, screenRect.Width)
        Dim h As Integer = Math.Max(1, screenRect.Height)
        Dim x0 As Integer = screenRect.Left
        Dim y0 As Integer = screenRect.Top
        Dim x1 As Integer = screenRect.Right - 1
        Dim y1 As Integer = screenRect.Bottom - 1

        ' Obere Kante
        For i As Integer = 0 To w - 1
            If ((pat >> (i And 7)) And 1) = 1 Then
                ControlPaint.DrawReversibleLine(New Point(x0 + i, y0), New Point(x0 + i + 1, y0), backCol)
            End If
        Next
        ' Rechte Kante
        For i As Integer = 0 To h - 1
            If ((pat >> (i And 7)) And 1) = 1 Then
                ControlPaint.DrawReversibleLine(New Point(x1, y0 + i), New Point(x1, y0 + i + 1), backCol)
            End If
        Next
        ' Untere Kante
        For i As Integer = 0 To w - 1
            If ((pat >> (i And 7)) And 1) = 1 Then
                ControlPaint.DrawReversibleLine(New Point(x1 - i, y1), New Point(x1 - i - 1, y1), backCol)
            End If
        Next
        ' Linke Kante
        For i As Integer = 0 To h - 1
            If ((pat >> (i And 7)) And 1) = 1 Then
                ControlPaint.DrawReversibleLine(New Point(x0, y1 - i), New Point(x0, y1 - i - 1), backCol)
            End If
        Next
    End Sub

    ''' <summary>
    ''' Nur den Debug-Rahmen zeichnen (z. B. für Platzhalter), ohne normale Ausgabe.
    ''' Nutzt die gesetzten Flags/Styles; optionales Override pro Aufruf.
    ''' </summary>
    Public Sub DrawDebugExclusive(Optional usePadding As Boolean? = Nothing,
                              Optional styleOverride As Integer? = Nothing,
                              Optional drawBounds As Boolean? = Nothing,
                              Optional drawContent As Boolean? = Nothing)
        If _dbgHost Is Nothing Then Exit Sub

        ' Aktuellen Zustand sichern
        Dim oldB As Boolean = _dbgDrawBounds, oldBS As Integer = _dbgStyleBounds
        Dim oldC As Boolean = _dbgDrawContent, oldCS As Integer = _dbgStyleContent

        ' Optional überschreiben
        If drawBounds.HasValue Then _dbgDrawBounds = drawBounds.Value
        If drawContent.HasValue Then _dbgDrawContent = drawContent.Value
        If styleOverride.HasValue Then
            _dbgStyleBounds = styleOverride.Value
            _dbgStyleContent = styleOverride.Value
        End If

        ' Zeichnen
        If _dbgDrawBounds Then
            Dim rb As Rectangle = BoundsRect
            If rb.Width > 0 AndAlso rb.Height > 0 Then
                DrawReversibleRectWithPattern(_dbgHost.RectangleToScreen(rb), _dbgStyleBounds)
            End If
        End If
        If _dbgDrawContent Then
            Dim rc As Rectangle = If(If(usePadding.HasValue, usePadding.Value, _usePaddingForDrawing), ContentRect, BoundsRect)
            If rc.Width > 0 AndAlso rc.Height > 0 Then
                DrawReversibleRectWithPattern(_dbgHost.RectangleToScreen(rc), _dbgStyleContent)
            End If
        End If

        ' Zustand zurück
        _dbgDrawBounds = oldB : _dbgStyleBounds = oldBS
        _dbgDrawContent = oldC : _dbgStyleContent = oldCS
    End Sub

#End Region

#Region "Bitmap-Utility"

#If INCLUDE_Shrinkbitmap Then

    Private Function ShrinkBitmap(ByRef bmp As Bitmap,
                                 newWidth As Integer,
                                 newHeight As Integer,
                                 Optional disposeSrc As Boolean = False,
                                 Optional highQuality As Boolean = True,
                                 Optional cvtToARGBBitmap As Boolean = True
                                ) As Boolean
        If bmp Is Nothing Then
            Return False
        End If
        If newWidth <= 0 OrElse newHeight <= 0 Then
            Return False
        End If
        If bmp.Width = newWidth AndAlso bmp.Height = newHeight Then
            Return False
        End If

        Dim pf As PixelFormat = If(cvtToARGBBitmap, PixelFormat.Format32bppArgb, bmp.PixelFormat)
        Dim nb As New Bitmap(newWidth, newHeight, pf)

        Using g As Graphics = Graphics.FromImage(nb)
            If highQuality Then
                g.InterpolationMode = InterpolationMode.HighQualityBicubic
                g.SmoothingMode = SmoothingMode.HighQuality
                g.PixelOffsetMode = PixelOffsetMode.HighQuality
                g.CompositingQuality = CompositingQuality.HighQuality
            Else
                g.InterpolationMode = InterpolationMode.NearestNeighbor
                g.SmoothingMode = SmoothingMode.None
                g.PixelOffsetMode = PixelOffsetMode.None
                g.CompositingQuality = CompositingQuality.HighSpeed
            End If
            g.DrawImage(bmp, New Rectangle(0, 0, newWidth, newHeight))
        End Using

        Dim old As Bitmap = bmp
        bmp = nb

        If disposeSrc Then
            Try : old.Dispose() : Catch : End Try
        End If

        Return True

    End Function

#End If

#End Region

End Class
