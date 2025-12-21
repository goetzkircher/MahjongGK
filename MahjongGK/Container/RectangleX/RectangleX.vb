'Ich gebe dir nochmal die komplette Klasse RectangleX.
'Gib mir bitte die komplette geänderte #Region "Debughilfe (Zeichnen ins _gfx, pro-Kante, DashPattern)"


Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

Imports System.Drawing.Drawing2D
Imports System.Globalization

Imports Grafix = System.Drawing.Graphics

' RectangleX – kompakt, _gfx-only Rendering, Debug-Rahmen via Pen.DashPattern

#Const INCLUDE_Shrinkbitmap = False
#Const USE_InternalMaybeDebugDraw = False

''' <summary>
''' Erweiterung der Rectangle-Struktur um Padding (Abstände innen) und diverse Hilfsmethoden.
''' Außerdem sind Zeichenmethoden enthalten, die auf eine Grafix-Instanz zeichnen.
''' </summary>
Public Class RectangleX

#Region "Basis + Padding"

    Private _x As Integer
    Private _y As Integer
    Private _w As Integer
    Private _h As Integer

    Private _gfx As Grafix

    ''' <summary>Abstände innen (Padding) – Standard ist 0 auf allen Seiten.</summary>
    Public Property Padding As PaddingValues

#End Region

#Region "Konstruktoren"

    Public Enum Align
        Left = 1
        Center = 2
        Right = 4
        Top = 8
        Bottom = 16
    End Enum

    Public Sub New()
        Me.New(0, 0, 0, 0)
    End Sub

    Public Sub New(x As Integer, y As Integer, width As Integer, height As Integer)
        _x = x : _y = y : _w = width : _h = height
        Padding = New PaddingValues(0)
    End Sub

    Public Sub New(x As Integer, y As Integer, width As Integer, height As Integer, paddingAll As Integer)
        Me.New(x, y, width, height)
        Me.Padding = New PaddingValues(paddingAll)
    End Sub

    Public Sub New(x As Integer, y As Integer, width As Integer, height As Integer, padding As PaddingValues)
        Me.New(x, y, width, height)
        Me.Padding = padding
    End Sub

    Public Sub New(location As Point, size As Size)
        Me.New(location.X, location.Y, size.Width, size.Height)
        Padding = New PaddingValues(0)
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
        Padding = New PaddingValues(0)
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
        Set(ByVal value As Integer)
            _x = value
        End Set
    End Property

    Public Property Y As Integer
        Get
            Return _y
        End Get
        Set(ByVal value As Integer)
            _y = value
        End Set
    End Property

    Public Property Left As Integer
        Get
            Return _x
        End Get
        Set(ByVal value As Integer)
            _x = value
        End Set
    End Property

    Public Property Top As Integer
        Get
            Return _y
        End Get
        Set(ByVal value As Integer)
            _y = value
        End Set
    End Property

    Public Property Width As Integer
        Get
            Return _w
        End Get
        Set(ByVal value As Integer)
            _w = value
        End Set
    End Property

    Public Property Height As Integer
        Get
            Return _h
        End Get
        Set(ByVal value As Integer)
            _h = value
        End Set
    End Property

    Public Property Right As Integer
        Get
            Return _x + _w
        End Get
        Set(ByVal value As Integer)
            _w = value - _x
        End Set
    End Property

    Public Property Bottom As Integer
        Get
            Return _y + _h
        End Get
        Set(ByVal value As Integer)
            _h = value - _y
        End Set
    End Property

    Public Property Location As Point
        Get
            Return New Point(_x, _y)
        End Get
        Set(ByVal value As Point)
            _x = value.X
            _y = value.Y
        End Set
    End Property

    Public Property Size As Size
        Get
            Return New Size(_w, _h)
        End Get
        Set(ByVal value As Size)
            _w = value.Width
            _h = value.Height
        End Set
    End Property

    ' --- Inside-Varianten ---

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
        If rx Is Nothing Then Return Rectangle.Empty
        Return rx.ToRectangle()
    End Operator

    Public Shared Widening Operator CType(rect As Rectangle) As RectangleX
        Return New RectangleX(rect)
    End Operator

    ' Entspricht Rectangle.IsEmpty (nur bei 0/0/0/0 True)
    Public ReadOnly Property IsEmpty As Boolean
        Get
            Return _x = 0 AndAlso _y = 0 AndAlso _w = 0 AndAlso _h = 0
        End Get
    End Property

    Public Shared ReadOnly Property Empty As RectangleX
        Get
            Return New RectangleX(0, 0, 0, 0)
        End Get
    End Property


#End Region

#Region "Inc/Dec / Grow/Shrink"

    ' Inc/Dec / Grow/Shrink

    Public Sub IncX(Optional ByVal n As Integer = 1)
        _x += n
    End Sub

    Public Sub DecX(Optional ByVal n As Integer = 1)
        _x -= n
    End Sub

    Public Sub IncLeft(Optional ByVal n As Integer = 1)
        _x += n
        _w = Math.Max(0, _w - n)
    End Sub

    Public Sub DecLeft(Optional ByVal n As Integer = 1)
        _x -= n
        _w += n
    End Sub

    Public Sub IncY(Optional ByVal n As Integer = 1)
        _y += n
    End Sub

    Public Sub DecY(Optional ByVal n As Integer = 1)
        _y -= n
    End Sub

    Public Sub IncTop(Optional ByVal n As Integer = 1)
        _y += n
        _h = Math.Max(0, _h - n)
    End Sub

    Public Sub DecTop(Optional ByVal n As Integer = 1)
        _y -= n
        _h += n
    End Sub

    Public Sub IncRight(Optional ByVal n As Integer = 1)
        _w += n
    End Sub

    Public Sub DecRight(Optional ByVal n As Integer = 1)
        _w = Math.Max(0, _w - n)
    End Sub

    Public Sub IncBottom(Optional ByVal n As Integer = 1)
        _h += n
    End Sub

    Public Sub DecBottom(Optional ByVal n As Integer = 1)
        _h = Math.Max(0, _h - n)
    End Sub


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

    Public Shared Function FromRectangle(rect As Rectangle, Optional paddingAll As Integer = 0) As RectangleX
        Dim rx As New RectangleX(rect)
        If paddingAll <> 0 Then rx.Padding = New PaddingValues(paddingAll)
        Return rx
    End Function

    Public Shared Function FromRectangle(rect As Rectangle, padding As PaddingValues) As RectangleX
        Dim rx As New RectangleX(rect) : rx.Padding = padding : Return rx
    End Function

#End Region

#Region "Abgeleitete Rechtecke"

    Public ReadOnly Property BoundsRect As Rectangle
        Get
            Return New Rectangle(_x, _y, _w, _h)
        End Get
    End Property

    Public ReadOnly Property ContentRect As Rectangle
        Get
            Dim cx As Integer = _x + Padding.Left
            Dim cy As Integer = _y + Padding.Top
            Dim cw As Integer = Math.Max(0, _w - (Padding.Left + Padding.Right))
            Dim ch As Integer = Math.Max(0, _h - (Padding.Top + Padding.Bottom))
            Return New Rectangle(cx, cy, cw, ch)
        End Get
    End Property

    Private Function GetBaseRect(usePadding As Boolean) As Rectangle
        Return If(usePadding, ContentRect, BoundsRect)
    End Function

#End Region

#Region "Ableitungen – bestehende Rectangle / RectangleX"

    Public Function GetRectangleInside(marginAll As Integer, usePadding As Boolean) As Rectangle
        Return GetRectangleInside(marginAll, marginAll, marginAll, marginAll, usePadding)
    End Function

    Public Function GetRectangleInside(marginLeft As Integer,
                                       marginRight As Integer,
                                       marginTop As Integer,
                                       marginBottom As Integer,
                                       usePadding As Boolean) As Rectangle
        Dim r As Rectangle = GetBaseRect(usePadding)
        Dim nx As Integer = r.X + marginLeft
        Dim ny As Integer = r.Y + marginTop
        Dim nw As Integer = Math.Max(0, r.Width - Math.Max(0, marginLeft) - Math.Max(0, marginRight))
        Dim nh As Integer = Math.Max(0, r.Height - Math.Max(0, marginTop) - Math.Max(0, marginBottom))
        Return New Rectangle(nx, ny, nw, nh)
    End Function

    Public Function GetRectangleInside(width As Integer,
                                       height As Integer,
                                       align As Align,
                                       usePadding As Boolean
                                       ) As Rectangle

        Dim baseRect As Rectangle = GetBaseRect(usePadding)
        If baseRect.Width <= 0 OrElse baseRect.Height <= 0 Then
            Return New Rectangle(baseRect.X, baseRect.Y,
                                 Math.Max(0, baseRect.Width), Math.Max(0, baseRect.Height))
        End If

        Dim reqW As Integer = If(width <= 0, baseRect.Width, width)
        Dim reqH As Integer = If(height <= 0, baseRect.Height, height)

        Dim tgtW As Integer = Math.Min(reqW, baseRect.Width)
        Dim tgtH As Integer = Math.Min(reqH, baseRect.Height)

        Dim nx As Integer
        Dim ny As Integer

        Select Case align
            Case Align.Center
                nx = baseRect.X + (baseRect.Width - tgtW) \ 2
                ny = baseRect.Y + (baseRect.Height - tgtH) \ 2
            Case Align.Left
                nx = baseRect.X
                ny = baseRect.Y
            Case Align.Right
                nx = baseRect.X + baseRect.Width - tgtW
                ny = baseRect.Y + baseRect.Height - tgtH
        End Select

        Return New Rectangle(nx, ny, tgtW, tgtH)

    End Function

    Private Shared Function ResolvePaddings(current As PaddingValues,
                                            paddings As System.Nullable(Of PaddingValues)) As PaddingValues
        If paddings.HasValue Then Return paddings.Value
        Return current
    End Function

    Public Function GetRectangleXInside(marginAll As Integer,
                                        usePadding As Boolean,
                                        Optional paddings As System.Nullable(Of PaddingValues) = Nothing) As RectangleX
        Dim r As Rectangle = GetRectangleInside(marginAll, usePadding)
        Dim rx As New RectangleX(r) : rx.Padding = ResolvePaddings(Me.Padding, paddings)
        Return rx
    End Function

    Public Function GetRectangleXInside(marginLeft As Integer,
                                        marginRight As Integer,
                                        marginTop As Integer,
                                        marginBottom As Integer,
                                        usePadding As Boolean,
                                        Optional paddings As System.Nullable(Of PaddingValues) = Nothing) As RectangleX
        Dim r As Rectangle = GetRectangleInside(marginLeft, marginRight, marginTop, marginBottom, usePadding)
        Dim rx As New RectangleX(r) : rx.Padding = ResolvePaddings(Me.Padding, paddings)
        Return rx
    End Function

    Public Function GetRectangleXInside(width As Integer,
                                        height As Integer,
                                        align As Align,
                                        usePadding As Boolean,
                                        Optional paddings As System.Nullable(Of PaddingValues) = Nothing) As RectangleX
        Dim r As Rectangle = GetRectangleInside(width, height, align, usePadding)
        Dim rx As New RectangleX(r) : rx.Padding = ResolvePaddings(Me.Padding, paddings)
        Return rx
    End Function

#End Region

#Region "Zeichenmethoden (nur _gfx)"

    ''' <summary>Setzt die Grafix-Instanz für alle Zeichenmethoden.</summary>
    Public Sub SetGfx(gfx As Grafix)
        _gfx = gfx
    End Sub
    Public Sub ClearGfx()
        _gfx = Nothing
    End Sub

    Public ReadOnly Property HasGfx As Boolean
        Get
            Return _gfx IsNot Nothing
        End Get
    End Property
    Public Sub DrawRectangle(color As Color,
                             borderWidth As Integer,
                             usePadding As Boolean)
        If _gfx Is Nothing OrElse borderWidth <= 0 Then Exit Sub
        Dim r As Rectangle = GetBaseRect(usePadding)
        If r.Width <= 0 OrElse r.Height <= 0 Then Exit Sub

        Using pen As New Pen(color, CSng(borderWidth))
            pen.Alignment = PenAlignment.Inset
            _gfx.SmoothingMode = SmoothingMode.AntiAlias
            _gfx.DrawRectangle(pen, r)
        End Using

#If USE_InternalMaybeDebugDraw Then
        InternalMaybeDebugDraw()
#End If

    End Sub

    Public Sub FillRectangle(color As Color,
                             usePadding As Boolean)
        If _gfx Is Nothing Then Exit Sub
        Dim r As Rectangle = GetBaseRect(usePadding)
        If r.Width <= 0 OrElse r.Height <= 0 Then Exit Sub

        Using br As New SolidBrush(color)
            _gfx.FillRectangle(br, r)
        End Using

#If USE_InternalMaybeDebugDraw Then
        InternalMaybeDebugDraw()
#End If

    End Sub

    Public Sub DrawImage(bmp As Bitmap,
                         usePadding As Boolean)
        If _gfx Is Nothing OrElse bmp Is Nothing Then Exit Sub
        Dim r As Rectangle = GetBaseRect(usePadding)
        If r.Width <= 0 OrElse r.Height <= 0 Then Exit Sub

#If INCLUDE_Shrinkbitmap Then
        If bmp.Width <> r.Width OrElse bmp.Height <> r.Height Then
            ShrinkBitmap(bmp, r.Width, r.Height, disposeSrc:=True, highQuality:=True, cvtToARGBBitmap:=True)
        End If
#Else
        If bmp.Width <> r.Width OrElse bmp.Height <> r.Height Then
            MjGDI.ShrinkBitmap(bmp, r.Width, r.Height, disposeSrc:=True, highQuality:=True, cvtToARGBBitmap:=True)
        End If
#End If

        _gfx.DrawImageUnscaled(bmp, r.Location)
#If USE_InternalMaybeDebugDraw Then
        InternalMaybeDebugDraw()
#End If

    End Sub

    Public Sub DrawImageBuffered(bgc As BackgroundSingleImageCache,
                                 usePadding As Boolean,
                                 Optional backgroundColor As Color? = Nothing)

        If _gfx Is Nothing Then Exit Sub
        Dim r As Rectangle = GetBaseRect(usePadding)
        If r.Width <= 0 OrElse r.Height <= 0 Then Exit Sub

        Dim bmp As Bitmap = Nothing
        If bgc IsNot Nothing Then
            bmp = bgc.GetBitmap(r.Size, If(backgroundColor.HasValue, CType(backgroundColor, Color), CType(Nothing, Color?)))
        End If

        If bmp Is Nothing Then
            If backgroundColor.HasValue Then
                Using br As New SolidBrush(backgroundColor.Value)
                    _gfx.FillRectangle(br, r)
                End Using
            End If
            Exit Sub
        End If

#If INCLUDE_Shrinkbitmap Then
        If bmp.Width <> r.Width OrElse bmp.Height <> r.Height Then
            ShrinkBitmap(bmp, r.Width, r.Height, disposeSrc:=True, highQuality:=True, cvtToARGBBitmap:=True)
        End If
#Else
        If bmp.Width <> r.Width OrElse bmp.Height <> r.Height Then
            MjGDI.ShrinkBitmap(bmp, r.Width, r.Height, disposeSrc:=True, highQuality:=True, cvtToARGBBitmap:=True)
        End If
#End If

        _gfx.DrawImageUnscaled(bmp, r.Location)
#If USE_InternalMaybeDebugDraw Then
        InternalMaybeDebugDraw()
#End If

    End Sub

    Public Sub DrawString(text As String,
                          fnt As Font,
                           usePadding As Boolean,
                          Optional foreColor As Color = Nothing
                         )
        DrawString(text, fnt, usePadding, foreColor, StringAlignment.Near, StringAlignment.Near)
    End Sub

    Public Sub DrawStringCentered(text As String,
                                  fnt As Font,
                                  usePadding As Boolean,
                                  Optional foreColor As Color = Nothing
                                  )
        DrawString(text, fnt, usePadding, foreColor, StringAlignment.Center, StringAlignment.Center)
    End Sub

    Public Sub DrawString(text As String,
                          fnt As Font,
                          usePadding As Boolean,
                          Optional foreColor As Color = Nothing,
                          Optional horizontalAlignment As StringAlignment = StringAlignment.Near,
                          Optional verticalAlignment As StringAlignment = StringAlignment.Near
                          )
        If _gfx Is Nothing OrElse fnt Is Nothing OrElse String.IsNullOrEmpty(text) Then Exit Sub
        Dim r As Rectangle = GetBaseRect(usePadding)
        If r.Width <= 0 OrElse r.Height <= 0 Then Exit Sub

        If foreColor = Nothing Then foreColor = Color.Black
        Using br As New SolidBrush(foreColor)
            Using fmt As New StringFormat(StringFormatFlags.NoClip)
                fmt.Alignment = horizontalAlignment
                fmt.LineAlignment = verticalAlignment
                fmt.Trimming = StringTrimming.EllipsisCharacter
                _gfx.TextRenderingHint = Drawing.Text.TextRenderingHint.ClearTypeGridFit
                _gfx.DrawString(text, fnt, br, RectangleF.op_Implicit(r), fmt)
            End Using
        End Using

#If USE_InternalMaybeDebugDraw Then
        InternalMaybeDebugDraw()
#End If
    End Sub

#End Region

#Region "DeepCopy & String-Pack/Unpack"

    Public Function DeepCopy() As RectangleX
        Dim copy As New RectangleX(_x, _y, _w, _h)
        copy.Padding = New PaddingValues(Padding.Left, Padding.Top, Padding.Right, Padding.Bottom)
        Return copy
    End Function

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
        If String.IsNullOrWhiteSpace(s) Then Return False

        Dim parts() As String = s.Trim().Split("|"c)
        If parts.Length <> 2 Then Return False

        Dim x As Integer, y As Integer, w As Integer, h As Integer
        Dim pl As Integer, pt As Integer, pr As Integer, pb As Integer
        If Not Parse4(parts(0), x, y, w, h) Then Return False
        If Not Parse4(parts(1), pl, pt, pr, pb) Then Return False

        Dim rx As New RectangleX(x, y, w, h)
        rx.Padding = New PaddingValues(pl, pt, pr, pb)
        result = rx
        Return True
    End Function

    Private Shared Function Parse4(csv As String,
                                   ByRef a As Integer, ByRef b As Integer,
                                   ByRef c As Integer, ByRef d As Integer) As Boolean
        If csv Is Nothing Then Return False
        Dim p As String() = csv.Split(","c)
        If p.Length <> 4 Then Return False
        Dim ci As CultureInfo = Globalization.CultureInfo.InvariantCulture
        Return Integer.TryParse(p(0), Globalization.NumberStyles.Integer, ci, a) AndAlso
               Integer.TryParse(p(1), Globalization.NumberStyles.Integer, ci, b) AndAlso
               Integer.TryParse(p(2), Globalization.NumberStyles.Integer, ci, c) AndAlso
               Integer.TryParse(p(3), Globalization.NumberStyles.Integer, ci, d)
    End Function

#End Region

#Region "Debughilfe (Zeichnen ins _gfx, pro-Kante, DashPattern)"

    ' --- Debug-Füllflächen: Alpha + Farbpalette (6 Farben) -----------------------
    Public Const DBG_ALPHA02 As Integer = 30     ' 0..255 – Vollflächen-Füllung (ContentRect)
    Public Const DBG_ALPHA01 As Integer = 30  ' 0..255 – zweite Füllung, wenn „Pattern da ist“

    ' 6 feste Basisfarben – Auswahl über _dbgStyle (rotierend)
    Private Shared ReadOnly _dbgFillPalette As Color() = {
        Color.Red, Color.Cyan, Color.Green, Color.Magenta, Color.Blue, Color.Yellow
    }

    Private Shared Function GetDebugFillColor(style As Integer, alpha As Integer) As Color
        Dim idx As Integer = Math.Abs(style) Mod _dbgFillPalette.Length
        Dim base As Color = _dbgFillPalette(idx)
        Dim a As Integer = Math.Max(0, Math.Min(255, alpha))
        Return Color.FromArgb(a, base)
    End Function

    ' Zeichnet die (halbtransparente) Vollflächen-Füllung für den ContentRect.
    ' wodurch die Pattern-Präsenz (Rahmen gezeichnet) visuell dunkler hervortritt.

    'Ich habe FillDebugContent geändert. Jetzt ändere du es bitte so, daß 
    Private Sub FillDebugContent(style As Integer)

        If _gfx Is Nothing Then Exit Sub

        Dim br As Rectangle = BoundsRect
        Dim cr As Rectangle = ContentRect

        ' 1) Erste Lage (immer, sobald Debug aktiv)
        Using bbr As New SolidBrush(GetDebugFillColor(style, DBG_ALPHA01))
            _gfx.FillRectangle(bbr, br)
        End Using

        ' 2) Zweite Lage, der Innenraum. Nur wenn er sich in der Größe des Außenraumes unterscheidet
        If br <> cr Then
            Using bcr As New SolidBrush(GetDebugFillColor(style, DBG_ALPHA02))
                _gfx.FillRectangle(bcr, cr)
            End Using
        End If
    End Sub
    ' ---------------------------------------------------------------------------

    <Flags>
    Private Enum EdgeMask As Integer
        None = 0
        Top = 1
        Right = 2
        Bottom = 4
        Left = 8
        All = Top Or Right Or Bottom Or Left
    End Enum

    Private _debugName As String = Nothing

    ''' <summary>Nur für Debugzwecke. Wird vom Binder gesetzt.</summary>
    Friend Sub __SetDebugName(name As String)
        _debugName = name
    End Sub

    ''' <summary>Gelesener Anzeigename für DrawDebug.</summary>
    Public ReadOnly Property DebugName As String
        Get
            Return _debugName
        End Get
    End Property

    ' Host nur für API-Kompatibilität vorhanden; Screen-Koordinaten werden NICHT verwendet
    Private _dbgHost As Control = Nothing
    Private _dbgBackColor As Color? = Nothing     ' ungenutzt, bleibt für API

    Private _dbgDrawContent As Boolean = False    ' Steuert NUR noch den Content-Rahmen
    Private _dbgDrawBounds As Boolean = False     ' Steuert den Bounds-Rahmen
    Private _dbgStyle As Integer = 0              ' Bestimmt die Farbe (6er-Rotation)

    ''' <summary>API-Stub; wird für Screen/XOR nicht mehr benötigt.</summary>
    Public Sub SetDebugHost(host As Control, Optional backColor As Color? = Nothing)
        _dbgHost = host
        _dbgBackColor = backColor
    End Sub

    Public Sub SetDrawBoundsAndContentDebug(style As Integer)
        _dbgDrawBounds = True
        _dbgStyle = style
        _dbgDrawContent = True
    End Sub
    Public Sub SetDrawContentDebug(style As Integer)
        _dbgDrawContent = True
        _dbgStyle = style
    End Sub
    Public Sub SetDrawBoundsDebug(style As Integer)
        _dbgDrawBounds = True : _dbgStyle = style
    End Sub

    Public Sub ResetDebug()
        _dbgDrawContent = False
        _dbgStyle = 0
        _dbgDrawBounds = False
        _dbgBackColor = Nothing
    End Sub

    ' Farbe aus INI – Fallback Magenta
    Private Shared Function DebugLineColor() As Color
        Try
            Return Color.Black ' INI.Rendering_DebugDrawLinesColor
        Catch
            Return Color.Magenta
        End Try
    End Function

    ' Dash-Pattern pro Stil (1px-Pens); bewusst simpel & performant
    Private Shared Function GetDashArray(style As Integer) As Single()
        Select Case style Mod 10
            Case 0 : Return New Single() {6, 6}
            Case 1 : Return New Single() {12, 12}
            Case 2 : Return New Single() {24, 24}
            Case 3 : Return New Single() {32, 32}
            Case 4 : Return New Single() {12, 6, 6, 6}
            Case 5 : Return New Single() {18, 6, 6, 6}
            Case 6 : Return New Single() {24, 6, 6, 6}
            Case 7 : Return New Single() {32, 6, 6, 6}
            Case 8 : Return New Single() {32, 12}
            Case 9 : Return New Single() {48, 24}
        End Select
        Return New Single() {6, 6}
    End Function

    ' Zeichnet einen gepunkteten/gestrichelten 1-px Rahmen ins _gfx
    Private Sub DrawPatternRect(r As Rectangle, style As Integer, edges As EdgeMask)
        If _gfx Is Nothing OrElse r.Width <= 0 OrElse r.Height <= 0 Then Exit Sub

        Dim dash() As Single = GetDashArray(style)

        Using pen As New Pen(DebugLineColor(), 1.0F)
            pen.Alignment = PenAlignment.Inset
            pen.DashStyle = DashStyle.Custom
            pen.DashPattern = dash
            pen.DashCap = DashCap.Flat
            _gfx.SmoothingMode = SmoothingMode.None  ' Pixel-präzise

            Dim x0 As Integer = r.Left
            Dim y0 As Integer = r.Top
            Dim x1 As Integer = r.Right - 1
            Dim y1 As Integer = r.Bottom - 1

            If (edges And EdgeMask.Top) <> 0 Then _gfx.DrawLine(pen, x0, y0, x1, y0)
            If (edges And EdgeMask.Right) <> 0 Then _gfx.DrawLine(pen, x1, y0, x1, y1)
            If (edges And EdgeMask.Bottom) <> 0 Then _gfx.DrawLine(pen, x1, y1, x0, y1)
            If (edges And EdgeMask.Left) <> 0 Then _gfx.DrawLine(pen, x0, y1, x0, y0)
        End Using
    End Sub

    ' Am Ende jeder Zeichenmethode – Debug-Ausgabe
    Private Sub InternalMaybeDebugDraw()
        If _gfx Is Nothing Then Exit Sub
        If Not Debugger.IsAttached Then Exit Sub

        Dim rb As Rectangle = BoundsRect
        Dim rc As Rectangle = ContentRect

        Dim drawB As Boolean = _dbgDrawBounds AndAlso rb.Width > 0 AndAlso rb.Height > 0
        Dim drawC As Boolean = _dbgDrawContent AndAlso rc.Width > 0 AndAlso rc.Height > 0

        ' Wenn keinerlei Debug-Rahmen aktiv ist, sparen wir uns jegliche Arbeit.
        If Not drawB AndAlso Not drawC Then Exit Sub

        ' Kantenmasken (vermeidet Doppellinien)
        Dim maskB As EdgeMask = EdgeMask.All
        Dim maskC As EdgeMask = EdgeMask.All
        If drawB AndAlso drawC Then
            If rc.Left = rb.Left Then maskC = maskC And Not EdgeMask.Left
            If rc.Top = rb.Top Then maskC = maskC And Not EdgeMask.Top
            If rc.Right = rb.Right Then maskC = maskC And Not EdgeMask.Right
            If rc.Bottom = rb.Bottom Then maskC = maskC And Not EdgeMask.Bottom
        End If

        ' --- NEU: ContentRect IMMER füllen, sobald Debug aktiv ist ---
        ' patternPresent := es wird irgendein Rahmen gezeichnet (Bounds oder Content)
        Dim patternPresent As Boolean = (drawB AndAlso maskB <> EdgeMask.None) OrElse (drawC AndAlso maskC <> EdgeMask.None)
        FillDebugContent(_dbgStyle)

        ' --- Rahmen zeichnen (wie gehabt) ---
        If drawC AndAlso maskC <> EdgeMask.None Then
            DrawPatternRect(rc, _dbgStyle, maskC)
        End If
        If drawB AndAlso maskB <> EdgeMask.None Then
            DrawPatternRect(rb, _dbgStyle, maskB)
        End If
    End Sub

    ''' <summary>Zeichnet nur die Debug-Overlays (z. B. Platzhalter), ohne normale Ausgabe.</summary>
    Public Sub DrawDebug(Optional drawBounds As Boolean? = Nothing,
                         Optional drawContent As Boolean? = Nothing,
                         Optional styleBounds As Integer? = Nothing)
        If _gfx Is Nothing Then Exit Sub
        If Not Debugger.IsAttached Then Return

        ' Zustand sichern
        Dim oldDrawB As Boolean = _dbgDrawBounds
        Dim oldDrawC As Boolean = _dbgDrawContent
        Dim oldStyle As Integer = _dbgStyle

        If drawBounds.HasValue Then _dbgDrawBounds = drawBounds.Value
        If drawContent.HasValue Then _dbgDrawContent = drawContent.Value
        If styleBounds.HasValue Then _dbgStyle = styleBounds.Value

        Dim rb As Rectangle = BoundsRect
        Dim rc As Rectangle = ContentRect

        Dim drawB As Boolean = _dbgDrawBounds AndAlso rb.Width > 0 AndAlso rb.Height > 0
        Dim drawC As Boolean = _dbgDrawContent AndAlso rc.Width > 0 AndAlso rc.Height > 0

        ' Füllung: immer, sobald irgendein Debug aktiv ist
        FillDebugContent(_dbgStyle)

        ' Rahmen
        Dim maskB As EdgeMask = EdgeMask.All
        Dim maskC As EdgeMask = EdgeMask.All
        If drawB AndAlso drawC Then
            If rc.Left = rb.Left Then maskC = maskC And Not EdgeMask.Left
            If rc.Top = rb.Top Then maskC = maskC And Not EdgeMask.Top
            If rc.Right = rb.Right Then maskC = maskC And Not EdgeMask.Right
            If rc.Bottom = rb.Bottom Then maskC = maskC And Not EdgeMask.Bottom
        End If

        Dim nameDone As Boolean = False
        If drawC AndAlso maskC <> EdgeMask.None Then
            DrawPatternRect(rc, _dbgStyle, maskC)
            DrawDebugName(rc)
            nameDone = True
        End If
        If drawB AndAlso maskB <> EdgeMask.None Then
            DrawPatternRect(rb, _dbgStyle, maskB)
            If Not nameDone Then DrawDebugName(rb)
        End If

        ' Zustand zurück
        _dbgDrawBounds = oldDrawB
        _dbgDrawContent = oldDrawC
        _dbgStyle = oldStyle
    End Sub

    ' Debug-Label oben links – benutzt deinen zentralen Placer (SFD.DebugLabels)
    Private Sub DrawDebugName(r As Rectangle)
        Dim txt As String = _debugName
        If String.IsNullOrEmpty(txt) OrElse _gfx Is Nothing Then Exit Sub

        Dim sizePx As Size = TextRenderer.MeasureText(txt, New Font("Segoe UI", 9.0F))
        Dim preferred As New Point(r.Left + 4, r.Top + 3)
        Dim target As Rectangle = Spielfeld.SFD.DebugLabels.Request(preferred, sizePx)

        _gfx.TextRenderingHint = Drawing.Text.TextRenderingHint.ClearTypeGridFit
        Using f As New Font("Segoe UI", 9.0F, FontStyle.Regular, GraphicsUnit.Point)
            Using fbShadow As New SolidBrush(Color.FromArgb(96, Color.Black))
                _gfx.DrawString(txt, f, fbShadow, CSng(target.X + 1), CSng(target.Y + 1))
            End Using
            Using fb As New SolidBrush(Color.Black)
                _gfx.DrawString(txt, f, fb, CSng(target.X), CSng(target.Y))
            End Using
        End Using
    End Sub

#End Region


#Region "Bitmap-Utility (optional)"

#If INCLUDE_Shrinkbitmap Then

    Private Function ShrinkBitmap(ByRef bmp As Bitmap,
                                  newWidth As Integer,
                                  newHeight As Integer,
                                  Optional disposeSrc As Boolean = False,
                                  Optional highQuality As Boolean = True,
                                  Optional cvtToARGBBitmap As Boolean = True) As Boolean
        If bmp Is Nothing Then Return False
        If newWidth <= 0 OrElse newHeight <= 0 Then Return False
        If bmp.Width = newWidth AndAlso bmp.Height = newHeight Then Return False

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
        If disposeSrc Then Try : old.Dispose() : Catch : End Try
        Return True
    End Function
#End If

#End Region

End Class