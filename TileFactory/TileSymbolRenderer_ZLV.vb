'Ich zeig dir hier die Klasse, in der der Font gebraucht wird.

'Option Compare Text
'Option Explicit On
'Option Infer Off
'Option Strict On

'Imports System.Drawing
'Imports System.Drawing.Drawing2D
'Imports MahjongGK.Contracts

''
'''' <summary>
'''' Rendert Mahjong-Symbole in die Symbolfläche eines Steins.
'''' </summary>
'Friend NotInheritable Class TileSymbolRenderer

'    Private Sub New()
'    End Sub

'    '
'    ''' <summary>
'    ''' Rendert ein Unicode-Zeichen mittig in die SymbolRect.
'    ''' </summary>
'    Public Shared Sub RenderSymbol(
'            g As Graphics,
'            layout As TileLayout,
'            colors As TileColors,
'            symbolText As String,
'            fontFamilyName As String,
'            fontStyle As FontStyle)

'        Dim outlineWidth As Single = CSng(layout.FaktorBasisWidthToAktWidth) * colors.FaktorSymbolOutlineWidth

'        Dim drawRect As RectangleF

'        With layout.SymbolRect

'            Dim offtop As Decimal = colors.FaktorSymbolOffsetTop
'            Dim offleft As Decimal = colors.FaktorSymbolOffsetLeft
'            'Note: Anpassung SteinFont.Noto OffsetTop
'            If colors.SteinFont = SteinFont.Noto Then
'                offtop += 3.5D
'            End If
'            '
'            '       10 =  0 Pixel Verschiebung auf dem Basisstein als Basis.
'            Dim addX As Integer = CInt(10 * layout.FaktorBasisWidthToAktWidth * offleft)
'            Dim addY As Integer = CInt(10 * layout.FaktorBasisHeightToAktHeight * offtop)

'            drawRect = RectangleF.FromLTRB(
'                CSng(.Left + addX),
'                CSng(.Top + addY),
'                CSng(.Right + addX),
'                CSng(.Bottom + addY))
'        End With

'        Dim faktor As Decimal = colors.GetFaktorSymbolSize
'        'Note: Anpassung SteinFont.Noto GetFaktorSymbolSize
'        If colors.SteinFont = SteinFont.Noto Then
'            'um das ist Noto ungefähr kleiner als Segeo 
'            faktor *= 1.55D
'        End If

'        RenderSymbolIntoRect(
'           g:=g,
'           targetRect:=drawRect,
'           symbolText:=symbolText,
'           fontFamilyName:=fontFamilyName,
'           fontStyle:=FontStyle.Regular,
'           colorFrom:=colors.GetColSymbolFrom,
'           colorTo:=colors.GetColSymbolTo,
'           gradientMode:=colors.GetSymbolGradientMode,
'           outlineColor:=colors.GetColSymbolOutline,
'           outlineWidth:=outlineWidth,
'           faktorSymbolSize:=faktor)

'    End Sub

'    '
'    ''' <summary>
'    ''' Sucht eine brauchbare Fontgröße, die möglichst groß in die SymbolRect passt.
'    ''' </summary>
'    Private Shared Function CreateBestFitFont(
'        g As Graphics,
'        text As String,
'        rect As RectangleF,
'        preferredFontFamilyName As String,
'        fontStyle As FontStyle,
'        faktorSymbolSize As Decimal) As Font

'        'Hinweis: Mit dem Faktor muß außerhalb der Messung multipliziert werden,
'        '         die testSize damit zu multiplizieren führt zum falschem Ergebnis. 
'        If g Is Nothing Then Throw New ArgumentNullException(NameOf(g))

'        Dim fontFamily As FontFamily = SymbolFontManager.ResolveFontFamily(preferredFontFamilyName)

'        Dim startSize As Single = rect.Height * 0.9F
'        If startSize < 6.0F Then startSize = 6.0F

'        Dim factor As Single = CSng(faktorSymbolSize)
'        If factor <= 0.5F Then factor = 0.5F

'        Dim testSize As Single = startSize

'        Do While testSize >= 4.0F

'            Using candidate As New Font(fontFamily, testSize, fontStyle, GraphicsUnit.Pixel)

'                Dim measured As SizeF = g.MeasureString(text, candidate)

'                If measured.Width <= rect.Width AndAlso measured.Height <= rect.Height Then
'                    Exit Do
'                End If

'            End Using

'            testSize -= 1.0F

'        Loop

'        Return New Font(fontFamily, testSize * factor, fontStyle, GraphicsUnit.Pixel)

'    End Function

'    '
'    ''' <summary>
'    ''' Rendert ein Unicode-Zeichen mittig in ein beliebiges Zielrechteck.
'    ''' Einfarbig
'    ''' </summary>
'    Public Shared Sub RenderSymbolIntoRect(
'        g As Graphics,
'        targetRect As Rectangle,
'        symbolText As String,
'        fontFamilyName As String,
'        fontStyle As FontStyle,
'        symbolColor As Color,
'        faktorSymbolSize As Decimal)

'        If String.IsNullOrWhiteSpace(symbolText) Then
'            Exit Sub
'        End If

'        Try

'            Dim drawRect As RectangleF = RectangleF.FromLTRB(
'                CSng(targetRect.Left),
'                CSng(targetRect.Top),
'                CSng(targetRect.Right),
'                CSng(targetRect.Bottom))

'            Dim fontToUse As Font = CreateBestFitFont(
'                g:=g,
'                text:=symbolText,
'                rect:=drawRect,
'                preferredFontFamilyName:=fontFamilyName,
'                fontStyle:=fontStyle,
'                faktorSymbolSize)

'            Using fontToDispose As Font = fontToUse,
'                  brText As New SolidBrush(symbolColor),
'                  sf As New StringFormat()

'                sf.Alignment = StringAlignment.Near
'                sf.LineAlignment = StringAlignment.Center
'                sf.FormatFlags = StringFormatFlags.NoWrap
'                sf.Trimming = StringTrimming.None

'                g.DrawString(symbolText, fontToDispose, brText, drawRect, sf)

'            End Using

'        Finally

'        End Try

'    End Sub
'    Public Shared Sub RenderSymbolIntoRect(g As Graphics,
'                             targetRect As RectangleF,
'                             symbolText As String,
'                             fontFamilyName As String,
'                             fontStyle As FontStyle,
'                             colorFrom As Color,
'                             colorTo As Color,
'                             gradientMode As LinearGradientMode,
'                             outlineColor As Color,
'                             outlineWidth As Single,
'                             faktorSymbolSize As Decimal)

'        If String.IsNullOrEmpty(symbolText) Then
'            Exit Sub
'        End If

'        Try

'            Using fontToUse As Font = CreateBestFitFont(
'                g:=g,
'                text:=symbolText,
'                rect:=targetRect,
'                preferredFontFamilyName:=fontFamilyName,
'                fontStyle:=fontStyle,
'                faktorSymbolSize:=faktorSymbolSize)

'                Using sf As New StringFormat()
'                    sf.Alignment = StringAlignment.Center
'                    sf.LineAlignment = StringAlignment.Center
'                    sf.FormatFlags = StringFormatFlags.NoClip

'                    Using gp As New GraphicsPath()

'                        Dim emSize As Single = CSng(g.DpiY * fontToUse.SizeInPoints / 72.0F)

'                        gp.AddString(symbolText,
'                                     fontToUse.FontFamily,
'                                     CInt(fontToUse.Style),
'                                     emSize,
'                                     targetRect,
'                                     sf)

'                        Using br As New LinearGradientBrush(targetRect, colorFrom, colorTo, gradientMode)
'                            g.FillPath(br, gp)
'                        End Using

'                        If outlineWidth > 0.0F AndAlso outlineColor.A > 0 Then
'                            Using pn As New Pen(outlineColor, outlineWidth)
'                                pn.LineJoin = LineJoin.Miter
'                                g.DrawPath(pn, gp)
'                            End Using
'                        End If

'                    End Using
'                End Using
'            End Using
'        Finally

'        End Try

'    End Sub

'    'Public Shared Function ResolveSymbolText(steinTyp As SteinTyp) As String

'    '    Select Case steinTyp

'    '        Case SteinTyp.WindOst
'    '            Return Char.ConvertFromUtf32(&H1F000)

'    '        Case SteinTyp.WindSüd
'    '            Return Char.ConvertFromUtf32(&H1F001)

'    '        Case SteinTyp.WindWst
'    '            Return Char.ConvertFromUtf32(&H1F002)

'    '        Case SteinTyp.WindNrd
'    '            Return Char.ConvertFromUtf32(&H1F003)

'    '        Case SteinTyp.DracheR
'    '            Return Char.ConvertFromUtf32(&H1F004)

'    '        Case SteinTyp.DracheG
'    '            Return Char.ConvertFromUtf32(&H1F005)

'    '        Case SteinTyp.DracheW
'    '            Return Char.ConvertFromUtf32(&H1F006)

'    '        Case SteinTyp.Symbol1
'    '            Return Char.ConvertFromUtf32(&H1F007)
'    '        Case SteinTyp.Symbol2
'    '            Return Char.ConvertFromUtf32(&H1F008)
'    '        Case SteinTyp.Symbol3
'    '            Return Char.ConvertFromUtf32(&H1F009)
'    '        Case SteinTyp.Symbol4
'    '            Return Char.ConvertFromUtf32(&H1F00A)
'    '        Case SteinTyp.Symbol5
'    '            Return Char.ConvertFromUtf32(&H1F00B)
'    '        Case SteinTyp.Symbol6
'    '            Return Char.ConvertFromUtf32(&H1F00C)
'    '        Case SteinTyp.Symbol7
'    '            Return Char.ConvertFromUtf32(&H1F00D)
'    '        Case SteinTyp.Symbol8
'    '            Return Char.ConvertFromUtf32(&H1F00E)
'    '        Case SteinTyp.Symbol9
'    '            Return Char.ConvertFromUtf32(&H1F00F)

'    '        Case SteinTyp.Bambus1
'    '            Return Char.ConvertFromUtf32(&H1F010)
'    '        Case SteinTyp.Bambus2
'    '            Return Char.ConvertFromUtf32(&H1F011)
'    '        Case SteinTyp.Bambus3
'    '            Return Char.ConvertFromUtf32(&H1F012)
'    '        Case SteinTyp.Bambus4
'    '            Return Char.ConvertFromUtf32(&H1F013)
'    '        Case SteinTyp.Bambus5
'    '            Return Char.ConvertFromUtf32(&H1F014)
'    '        Case SteinTyp.Bambus6
'    '            Return Char.ConvertFromUtf32(&H1F015)
'    '        Case SteinTyp.Bambus7
'    '            Return Char.ConvertFromUtf32(&H1F016)
'    '        Case SteinTyp.Bambus8
'    '            Return Char.ConvertFromUtf32(&H1F017)
'    '        Case SteinTyp.Bambus9
'    '            Return Char.ConvertFromUtf32(&H1F018)

'    '        Case SteinTyp.Punkt01
'    '            Return Char.ConvertFromUtf32(&H1F019)
'    '        Case SteinTyp.Punkt02
'    '            Return Char.ConvertFromUtf32(&H1F01A)
'    '        Case SteinTyp.Punkt03
'    '            Return Char.ConvertFromUtf32(&H1F01B)
'    '        Case SteinTyp.Punkt04
'    '            Return Char.ConvertFromUtf32(&H1F01C)
'    '        Case SteinTyp.Punkt05
'    '            Return Char.ConvertFromUtf32(&H1F01D)
'    '        Case SteinTyp.Punkt06
'    '            Return Char.ConvertFromUtf32(&H1F01E)
'    '        Case SteinTyp.Punkt07
'    '            Return Char.ConvertFromUtf32(&H1F01F)
'    '        Case SteinTyp.Punkt08
'    '            Return Char.ConvertFromUtf32(&H1F020)
'    '        Case SteinTyp.Punkt09
'    '            Return Char.ConvertFromUtf32(&H1F021)

'    '        Case SteinTyp.BlütePf
'    '            Return Char.ConvertFromUtf32(&H1F022)
'    '        Case SteinTyp.BlüteOr
'    '            Return Char.ConvertFromUtf32(&H1F023)
'    '        Case SteinTyp.BlüteCt
'    '            Return Char.ConvertFromUtf32(&H1F024)
'    '        Case SteinTyp.BlüteBa
'    '            Return Char.ConvertFromUtf32(&H1F025)

'    '        Case SteinTyp.JahrFrl
'    '            Return Char.ConvertFromUtf32(&H1F026)
'    '        Case SteinTyp.JahrSom
'    '            Return Char.ConvertFromUtf32(&H1F027)
'    '        Case SteinTyp.JahrHer
'    '            Return Char.ConvertFromUtf32(&H1F028)
'    '        Case SteinTyp.JahrWin
'    '            Return Char.ConvertFromUtf32(&H1F029)

'    '        Case SteinTyp.ErrorSy
'    '            Return "?"

'    '        Case Else
'    '            Return String.Empty

'    '    End Select

'    'End Function
'End Class
