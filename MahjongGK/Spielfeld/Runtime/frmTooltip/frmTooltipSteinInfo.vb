Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On
Imports MahjongGK.Contracts
Imports MahjongGK.Contracts.GlobalEnum

''Verwendung:
''Private _frmSteinInfo As frmTooltipSteinInfo
''Private Sub InitSteinInfoForm()
''    _frmSteinInfo = New frmTooltipSteinInfo(Me,
''                                          offsetRight:=18,
''                                          offsetUp:=12)
''End Sub
''
'' Beim Rendern dann nur noch:
'' _frmSteinInfo.UpdateInfo(arrTripl, ptMousePos, rectStageAvailable)
''
'' Ausblenden:
'' _frmSteinInfo.HideInfo()

'
''' <summary>
''' Pfad: MahjongGK\Spielfeld\Runtime\Tooltip\
''' Kleine rahmenlose Info-Form zur Anzeige der unter dem Mauszeiger
''' gestapelten Mahjong-Steine.
'''
''' Alle übergebenen Koordinaten beziehen sich auf die Owner-Form.
''' Intern wird nach Bildschirmkoordinaten umgerechnet.
''' </summary>
Public NotInheritable Class frmTooltipSteinInfo
    Inherits Form

    Private Const MaxZeilen As Integer = MJ_STEINE_MAXZ
    Private Const InnenRandX As Integer = 8
    Private Const InnenRandY As Integer = 6
    Private Const ZeilenAbstand As Integer = 2
    Private Const RahmenBreite As Integer = 1

    Private ReadOnly _ownerForm As Form
    Private ReadOnly _offsetRight As Integer
    Private ReadOnly _offsetUp As Integer

    Private ReadOnly _fontMono As Font
    Private ReadOnly _brushBack As SolidBrush
    Private ReadOnly _penBorder As Pen

    Private _anzeigeTexte() As String
    Private _lineHeight As Integer
    Private _maxTextWidth As Integer

    Private _lastMousePosOwner As Point
    Private _lastRectStageAvailableOwner As Rectangle
    Private _lastInfoKeys() As SteinInfoKey
    Private _lastVisibleState As Boolean

    '
    ''' <summary>
    ''' Schlüssel für die Änderungsprüfung.
    ''' Relevant sind nur SteinInfoIndex und Quadrant.
    ''' </summary>
    Private Structure SteinInfoKey

        Public SteinInfoIndex As Integer
        Public Quadrant As Quadrant

        Public Sub New(steinInfoIndexValue As Integer,
                       quadrantValue As Quadrant)

            SteinInfoIndex = steinInfoIndexValue
            Quadrant = quadrantValue

        End Sub

    End Structure

    '
    ''' <summary>
    ''' Erzeugt die Info-Form.
    ''' </summary>
    Public Sub New(ownerForm As Form,
                   offsetRight As Integer,
                   offsetUp As Integer)

        If ownerForm Is Nothing Then
            Throw New ArgumentNullException(NameOf(ownerForm))
        End If

        _ownerForm = ownerForm
        _offsetRight = Math.Max(0, offsetRight)
        _offsetUp = Math.Max(0, offsetUp)

        _fontMono = New Font(FontFamily.GenericMonospace, 9.0F, FontStyle.Regular, GraphicsUnit.Point)
        _brushBack = New SolidBrush(Color.FromArgb(245, 255, 255, 235))
        _penBorder = New Pen(Color.Black, RahmenBreite)

        _lineHeight = TextRenderer.MeasureText("Mg",
                                               _fontMono,
                                               New Size(Integer.MaxValue, Integer.MaxValue),
                                               TextFormatFlags.NoPadding).Height

        ReDim _anzeigeTexte(-1)

        _lastMousePosOwner = New Point(Integer.MinValue, Integer.MinValue)
        _lastRectStageAvailableOwner = Rectangle.Empty
        _lastInfoKeys = Nothing
        _lastVisibleState = False

        Me.FormBorderStyle = FormBorderStyle.None
        Me.ShowInTaskbar = False
        Me.StartPosition = FormStartPosition.Manual
        Me.ControlBox = False
        Me.MinimizeBox = False
        Me.MaximizeBox = False
        Me.DoubleBuffered = True
        Me.Owner = ownerForm

        Me.SetStyle(ControlStyles.AllPaintingInWmPaint Or
                    ControlStyles.OptimizedDoubleBuffer Or
                    ControlStyles.ResizeRedraw Or
                    ControlStyles.UserPaint, True)

        Me.Size = New Size(40, 20)
        Me.Visible = False

    End Sub

    '
    ''' <summary>
    ''' Aktualisiert Inhalt, Position und Sichtbarkeit in einem Schritt.
    ''' Alle Koordinaten beziehen sich auf die Owner-Form.
    ''' </summary>
    ''' <param name="arrTriplX">Anzuzeigende Steine oder Nothing/leer.</param>
    ''' <param name="mousePosOwner">Mausposition relativ zur Owner-Form.</param>
    ''' <param name="rectStageAvailableOwner">Verfügbarer Bereich relativ zur Owner-Form.</param>
    Public Sub UpdateInfo(arrTriplX() As TripleX,
                          mousePosOwner As Point,
                          rectStageAvailableOwner As Rectangle)

        Dim hasText As Boolean = (arrTriplX IsNot Nothing AndAlso arrTriplX.Length > 0)
        Dim mouseChanged As Boolean = (mousePosOwner <> _lastMousePosOwner)
        Dim rectChanged As Boolean = (rectStageAvailableOwner <> _lastRectStageAvailableOwner)
        Dim textChanged As Boolean = Not InfoKeysEqual10(_lastInfoKeys, arrTriplX)

        If Not hasText Then
            If Me.Visible Then
                Me.Hide()
                _lastVisibleState = False
            End If

            _lastMousePosOwner = mousePosOwner
            _lastRectStageAvailableOwner = rectStageAvailableOwner
            CopyInfoKeys10(arrTriplX, _lastInfoKeys)
            Return
        End If

        If textChanged Then
            BuildDisplayText(arrTriplX)
            RecalcSize()
        End If

        If textChanged OrElse mouseChanged OrElse rectChanged Then
            PositioniereForm(mousePosOwner, rectStageAvailableOwner)
        End If

        If textChanged Then
            Me.Invalidate()
        End If

        If Not Me.Visible Then
            Me.Show()
            _lastVisibleState = True
        End If

        _lastMousePosOwner = mousePosOwner
        _lastRectStageAvailableOwner = rectStageAvailableOwner
        CopyInfoKeys10(arrTriplX, _lastInfoKeys)

    End Sub

    '
    ''' <summary>
    ''' Blendet die Form aus.
    ''' </summary>
    Public Sub HideInfo()

        If Me.Visible Then
            Me.Hide()
            _lastVisibleState = False
        End If

    End Sub

    '
    ''' <summary>
    ''' Baut die anzuzeigenden Texte auf.
    ''' </summary>
    Private Sub BuildDisplayText(arrTriplX() As TripleX)

        Dim count As Integer = Math.Min(arrTriplX.Length, MaxZeilen)

        ReDim _anzeigeTexte(count - 1)

        Dim i As Integer
        Dim maxWidth As Integer = 0
        Dim isDebugAttached As Boolean = System.Diagnostics.Debugger.IsAttached

        For i = 0 To count - 1
            Dim s As String = TriplText(arrTriplX(i), isDebugAttached)
            _anzeigeTexte(i) = s

            Dim w As Integer = TextRenderer.MeasureText(s,
                                                       _fontMono,
                                                       New Size(Integer.MaxValue, Integer.MaxValue),
                                                       TextFormatFlags.NoPadding).Width
            If w > maxWidth Then
                maxWidth = w
            End If
        Next

        _maxTextWidth = maxWidth

    End Sub

    '
    ''' <summary>
    ''' Berechnet die Formgröße aus den aktuellen Texten.
    ''' </summary>
    Private Sub RecalcSize()

        Dim count As Integer = _anzeigeTexte.Length

        If count <= 0 Then
            If Me.Width <> 40 OrElse Me.Height <> 20 Then
                Me.Size = New Size(40, 20)
            End If
            Return
        End If

        Dim width As Integer = _maxTextWidth + InnenRandX * 2 + RahmenBreite * 2
        Dim height As Integer = InnenRandY * 2 + RahmenBreite * 2 + count * _lineHeight + (count - 1) * ZeilenAbstand

        If Me.Width <> width OrElse Me.Height <> height Then
            Me.Size = New Size(width, height)
        End If

    End Sub

    '
    ''' <summary>
    ''' Positioniert die Form relativ zur Maus.
    ''' Normalfall:
    ''' - rechts der Maus
    ''' - oberhalb der Maus
    ''' Falls erforderlich wird auf links bzw. unten gewechselt.
    ''' </summary>
    Private Sub PositioniereForm(mousePosOwner As Point,
                                 rectStageAvailableOwner As Rectangle)

        If rectStageAvailableOwner.Width <= 0 OrElse rectStageAvailableOwner.Height <= 0 Then
            Return
        End If

        Dim rectScreen As Rectangle = OwnerRectToScreen(rectStageAvailableOwner)
        Dim mouseScreen As Point = _ownerForm.PointToScreen(mousePosOwner)

        Dim formWidth As Integer = Me.Width
        Dim formHeight As Integer = Me.Height

        Dim left As Integer = mouseScreen.X + _offsetRight
        Dim top As Integer = mouseScreen.Y - _offsetUp - formHeight

        If left + formWidth > rectScreen.Right Then
            left = mouseScreen.X - _offsetRight - formWidth
        End If

        If top < rectScreen.Top Then
            top = mouseScreen.Y + _offsetUp
        End If

        If left < rectScreen.Left Then
            left = rectScreen.Left
        End If

        If left + formWidth > rectScreen.Right Then
            left = rectScreen.Right - formWidth
        End If

        If top < rectScreen.Top Then
            top = rectScreen.Top
        End If

        If top + formHeight > rectScreen.Bottom Then
            top = rectScreen.Bottom - formHeight
        End If

        If Me.Left <> left OrElse Me.Top <> top Then
            Me.Location = New Point(left, top)
        End If

    End Sub

    '
    ''' <summary>
    ''' Wandelt ein Rectangle von Owner-Koordinaten in Bildschirmkoordinaten um.
    ''' </summary>
    Private Function OwnerRectToScreen(rc As Rectangle) As Rectangle

        Dim pt As Point = _ownerForm.PointToScreen(New Point(rc.Left, rc.Top))
        Return New Rectangle(pt.X, pt.Y, rc.Width, rc.Height)

    End Function

    '
    ''' <summary>
    ''' Vergleicht maximal 10 Array-Einträge über SteinInfoIndex und Quadrant.
    ''' </summary>
    Private Shared Function InfoKeysEqual10(lastInfoKeys() As SteinInfoKey,
                                            arrTripl() As TripleX) As Boolean

        If lastInfoKeys Is Nothing OrElse arrTripl Is Nothing Then
            Return (lastInfoKeys Is Nothing AndAlso arrTripl Is Nothing)
        End If

        Dim len1 As Integer = Math.Min(lastInfoKeys.Length, MaxZeilen)
        Dim len2 As Integer = Math.Min(arrTripl.Length, MaxZeilen)

        If len1 <> len2 Then
            Return False
        End If

        Dim i As Integer
        For i = 0 To len1 - 1
            If lastInfoKeys(i).SteinInfoIndex <> arrTripl(i).SteinInfoIndex Then
                Return False
            End If

            If lastInfoKeys(i).Quadrant <> arrTripl(i).Quadrant Then
                Return False
            End If
        Next

        Return True

    End Function

    '
    ''' <summary>
    ''' Kopiert maximal 10 Schlüsselwerte für die Änderungsprüfung.
    ''' </summary>
    Private Shared Sub CopyInfoKeys10(source() As TripleX,
                                      ByRef target() As SteinInfoKey)

        If source Is Nothing OrElse source.Length = 0 Then
            target = Nothing
            Return
        End If

        Dim len As Integer = Math.Min(source.Length, MaxZeilen)

        ReDim target(len - 1)

        Dim i As Integer
        For i = 0 To len - 1
            target(i) = New SteinInfoKey(source(i).SteinInfoIndex,
                                         source(i).Quadrant)
        Next

    End Sub

    '
    ''' <summary>
    ''' Wandelt einen Tripl-Eintrag in den Anzeigetext um.
    ''' </summary>
    Private Shared Function TriplText(value As TripleX,
                                      isDebugAttached As Boolean) As String

        Dim quadrantText As String = GetQuadrantText(value.Quadrant)
        Dim steinText As String = GetSteinText(value.SteinTypIndex)

        ''If isDebugAttached Then
        ''    Return value.SteinInfoIndex.ToString().PadLeft(3) & " " & quadrantText & " " & steinText
        ''Else
        ''    Return quadrantText & " " & steinText
        ''End If

        Return (value.SteinInfoIndex + 1).ToString().PadLeft(3) & " " & quadrantText & " " & steinText

    End Function

    '
    ''' <summary>
    ''' Wandelt einen Quadranten-Enum in Anzeigetext um.
    ''' </summary>
    Private Shared Function GetQuadrantText(value As Quadrant) As String

        Select Case value
            Case Quadrant.none : Return "none"
            Case Quadrant.LO : Return "LO"
            Case Quadrant.RO : Return "RO"
            Case Quadrant.LU : Return "LU"
            Case Quadrant.RU : Return "RU"
            Case Else
                Return value.ToString()
        End Select

    End Function

    '
    ''' <summary>
    ''' Wandelt einen Stein-Enum in Anzeigetext um.
    ''' </summary>
    Private Shared Function GetSteinText(value As SteinTyp) As String

        Select Case value
            Case SteinTyp.ErrorSy : Return "Error"
            Case SteinTyp.Punkt01 : Return "Punkt 01"
            Case SteinTyp.Punkt02 : Return "Punkt 02"
            Case SteinTyp.Punkt03 : Return "Punkt 03"
            Case SteinTyp.Punkt04 : Return "Punkt 04"
            Case SteinTyp.Punkt05 : Return "Punkt 05"
            Case SteinTyp.Punkt06 : Return "Punkt 06"
            Case SteinTyp.Punkt07 : Return "Punkt 07"
            Case SteinTyp.Punkt08 : Return "Punkt 08"
            Case SteinTyp.Punkt09 : Return "Punkt 09"

            Case SteinTyp.Bambus1 : Return "Bambus 1"
            Case SteinTyp.Bambus2 : Return "Bambus 2"
            Case SteinTyp.Bambus3 : Return "Bambus 3"
            Case SteinTyp.Bambus4 : Return "Bambus 4"
            Case SteinTyp.Bambus5 : Return "Bambus 5"
            Case SteinTyp.Bambus6 : Return "Bambus 6"
            Case SteinTyp.Bambus7 : Return "Bambus 7"
            Case SteinTyp.Bambus8 : Return "Bambus 8"
            Case SteinTyp.Bambus9 : Return "Bambus 9"

            Case SteinTyp.Symbol1 : Return "Symbol 1"
            Case SteinTyp.Symbol2 : Return "Symbol 2"
            Case SteinTyp.Symbol3 : Return "Symbol 3"
            Case SteinTyp.Symbol4 : Return "Symbol 4"
            Case SteinTyp.Symbol5 : Return "Symbol 5"
            Case SteinTyp.Symbol6 : Return "Symbol 6"
            Case SteinTyp.Symbol7 : Return "Symbol 7"
            Case SteinTyp.Symbol8 : Return "Symbol 8"
            Case SteinTyp.Symbol9 : Return "Symbol 9"

            Case SteinTyp.DracheR : Return "roter Drache"
            Case SteinTyp.DracheG : Return "grüner Drache"
            Case SteinTyp.DracheW : Return "weißer Drache"

            Case SteinTyp.WindOst : Return "Ostwind"
            Case SteinTyp.WindSüd : Return "Südwind"
            Case SteinTyp.WindWst : Return "Westwind"
            Case SteinTyp.WindNrd : Return "Nordwind"

            Case SteinTyp.BlütePf : Return "Pfaume"
            Case SteinTyp.BlüteOr : Return "Orchidee"
            Case SteinTyp.BlüteCt : Return "Chrysantheme"
            Case SteinTyp.BlüteBa : Return "Bambus"

            Case SteinTyp.JahrFrl : Return "Frühling"
            Case SteinTyp.JahrSom : Return "Sommer"
            Case SteinTyp.JahrHer : Return "Herbst"
            Case SteinTyp.JahrWin : Return "Winter"

            Case Else
                Return value.ToString()
        End Select

    End Function

    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        MyBase.OnPaint(e)

        Dim g As Graphics = e.Graphics

        Dim rc As Rectangle = Me.ClientRectangle
        rc.Width -= 1
        rc.Height -= 1

        g.FillRectangle(_brushBack, rc)
        g.DrawRectangle(_penBorder, rc)

        If _anzeigeTexte Is Nothing OrElse _anzeigeTexte.Length = 0 Then
            Return
        End If

        Dim y As Integer = InnenRandY + RahmenBreite
        Dim i As Integer

        For i = 0 To _anzeigeTexte.Length - 1
            Dim rcText As New Rectangle(InnenRandX + RahmenBreite,
                                        y,
                                        Me.ClientSize.Width - (InnenRandX + RahmenBreite) * 2,
                                        _lineHeight)

            TextRenderer.DrawText(g,
                                  _anzeigeTexte(i),
                                  _fontMono,
                                  rcText,
                                  Color.Black,
                                  TextFormatFlags.Left Or TextFormatFlags.VerticalCenter Or TextFormatFlags.NoPadding)

            y += _lineHeight + ZeilenAbstand
        Next

    End Sub

    Protected Overrides ReadOnly Property ShowWithoutActivation As Boolean
        Get
            Return True
        End Get
    End Property

    Protected Overrides ReadOnly Property CreateParams As CreateParams
        Get
            Dim cp As CreateParams = MyBase.CreateParams
            cp.ExStyle = cp.ExStyle Or &H80
            cp.ExStyle = cp.ExStyle Or &H8000000
            Return cp
        End Get
    End Property

    Protected Overrides Sub Dispose(disposing As Boolean)
        Try
            If disposing Then
                _fontMono.Dispose()
                _brushBack.Dispose()
                _penBorder.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

End Class