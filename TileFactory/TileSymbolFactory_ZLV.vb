'Option Compare Text
'Option Explicit On
'Option Infer Off
'Option Strict On

'Imports System.Drawing
'Imports MahjongGK.Contracts
'Imports MahjongGK.Contracts.GlobalEnum

''
'''' <summary>
'''' Erzeugt die Symbolschicht eines Steines.
'''' </summary>
'Friend NotInheritable Class TileSymbolFactory

'    Private Sub New()
'    End Sub

'    '
'    ''' <summary>
'    ''' Transparente Bitmap mit Symbolinhalt.
'    ''' </summary>
'    Public Shared Function CreateSymbolLayerBitmap(request As TileRequest,
'                                               layout As TileLayout,
'                                               colors As TileColors) As Bitmap

'        If request Is Nothing Then Throw New ArgumentNullException(NameOf(request))
'        If layout Is Nothing Then Throw New ArgumentNullException(NameOf(layout))
'        If colors Is Nothing Then Throw New ArgumentNullException(NameOf(colors))

'        Dim bmp As New Bitmap(layout.SteinRect.Width, layout.SteinRect.Height, Imaging.PixelFormat.Format32bppArgb)

'        Dim srv As TileColors.SymbolRenderValues = colors.GetSymbolRenderValues
'        Dim trv As TileColors.TextRenderValues = colors.GetTextRenderValues

'        Using g As Graphics = Graphics.FromImage(bmp)

'            SetGraphicsMode(g, sourceOver:=False)
'            g.Clear(Color.Transparent)

'            If srv.DonotInsertSymbol Then
'                Return bmp
'            Else

'            End If

'            Dim done As Boolean
'            Select Case request.SteinStatus
'                Case SteinStatus.I08WerkstückEinfügeFehler
'                    RenderTileSymbolHalfSize(g, request, layout, colors)
'                    RenderErrorCross(g, layout, colors)

'                Case SteinStatus.I05Locked
'                    RenderTileSymbolNormal(g, request, layout, colors)
'                    RenderErrorCross(g, layout, colors)

'                Case SteinStatus.I09WerkstückZufallsgrafik
'                    RenderTileSymbolHalfSize(g, request, layout, colors)

'                Case SteinStatus.I07MissingSecond
'                    RenderTileSymbolNormal(g, request, layout, colors)

'                    Dim labelText As String = ResolveWindCornerLabel(request.SteinTyp)
'                    If labelText = String.Empty Then
'                        labelText = "♊"
'                    Else
'                        labelText = "♊ " & labelText
'                    End If

'                    RenderLabelText(g, request, layout, labelText, colors)
'                    done = True

'                Case Else
'                    RenderTileSymbolNormal(g, request, layout, colors)
'            End Select

'            If Not done AndAlso request.SteinTypVersion = SteinTypVersion.Winde Then
'                Dim labelText As String = ResolveWindCornerLabel(request.SteinTyp)
'                RenderLabelText(g, request, layout, labelText, colors)
'            End If

'        End Using

'        Return bmp

'    End Function

'    '
'    ''' <summary>
'    ''' Symbol normal groß.
'    ''' </summary>
'    Private Shared Sub RenderTileSymbolNormal(g As Graphics,
'                                          request As TileRequest,
'                                          layout As TileLayout,
'                                          colors As TileColors)

'        Dim trv As TileColors.TextRenderValues = colors.GetTextRenderValues

'        Dim symbolText As String = ResolveSymbolText(request.SteinTyp)

'        If String.IsNullOrWhiteSpace(symbolText) Then
'            Return
'        End If

'        TileSymbolRenderer.RenderSymbol(
'            g:=g,
'            layout:=layout,
'            colors:=colors,
'            symbolText:=symbolText,
'            fontFamilyName:=colors.GetSymbolFontFamilyName,
'            fontStyle:=FontStyle.Regular)

'        '
'        'N Auskommentiert. Schreibt einen Erzeugungszähler in den Mahjongstein
'        'Static count As Integer
'        'count += 1
'        'TileSymbolRenderer.RenderSymbol(
'        '    g:=g,
'        '    layout:=layout,
'        '    colors:=colors,
'        '    symbolText:="   " & count.ToString & "   ",
'        '    fontFamilyName:="Arial",
'        '    fontStyle:=FontStyle.Regular)

'    End Sub

'    '
'    ''' <summary>
'    ''' Symbol halb so groß für Zufallsgrafik.
'    ''' </summary>
'    Private Shared Sub RenderTileSymbolHalfSize(g As Graphics,
'                                            request As TileRequest,
'                                            layout As TileLayout,
'                                            colors As TileColors)

'        Dim symbolText As String = ResolveSymbolText(request.SteinTyp)

'        If String.IsNullOrWhiteSpace(symbolText) Then
'            Return
'        End If

'        Dim halfRect As Rectangle = ScaleRectAroundCenter(layout.SymbolRect, 0.5R)

'        TileSymbolRenderer.RenderSymbolIntoRect(
'            g:=g,
'            targetRect:=halfRect,
'            symbolText:=symbolText,
'            symbolColor:=colors.GetColSymbolFrom,  ichtig?
'            fontFamilyName:=colors.GetSymbolFontFamilyName,
'            fontStyle:=FontStyle.Regular,
'            faktorSymbolSize:=1)

'    End Sub

'    '
'    ''' <summary>
'    ''' Zeichnet ein Fehlerkreuz über den Stein.
'    ''' </summary>
'    Private Shared Sub RenderErrorCross(g As Graphics, layout As TileLayout, colors As TileColors)

'        TileSymbolRenderer.RenderSymbol(
'            g:=g,
'            layout:=layout,
'            colors:=colors,
'            symbolText:="✘",
'            fontFamilyName:=colors.GetSymbolFontFamilyName,
'            fontStyle:=FontStyle.Regular)

'    End Sub

'    '
'    ''' <summary>
'    ''' Bei SteinTypVersion.Winde zusätzliche Kennzeichnung links oben.
'    ''' </summary>
'    Private Shared Sub RenderLabelText(g As Graphics,
'                                             request As TileRequest,
'                                             layout As TileLayout,
'                                             labelText As String,
'                                             colors As TileColors)

'        If String.IsNullOrWhiteSpace(labelText) Then
'            Exit Sub
'        End If

'
'   
'
'
'
'        Den Text links oben eintragen
'        'If colors.TextDontInsert Then
'        '    Exit Sub
'        'End If

'        Dim rect As Rectangle
'        With colors
'            '                          10 =  10 Pixel Verschiebung auf dem Basisstein als Basis.
'            Dim addX As Integer = CInt(10 * layout.FaktorBasisWidthToAktWidth * colors.FaktorTextOffsetLeft)
'            Dim addY As Integer = CInt(10 * layout.FaktorBasisHeightToAktHeight * colors.FaktorTextOffsetTop)

'            rect = New Rectangle(
'            CInt(layout.FaceInnerRect.Left * (.FaktorTextOffsetLeft + 2.0F)),
'            CInt(layout.FaceInnerRect.Top * (.FaktorTextOffsetTop + 2.0F)),
'            CInt(layout.FaceInnerRect.Width * 0.24R),
'            CInt(layout.FaceInnerRect.Height * 0.16R))
'        End With

'        TileSymbolRenderer.RenderSymbolIntoRect(
'            g:=g,
'            targetRect:=rect,
'            symbolText:=labelText,
'            symbolColor:=colors.GetColText,
'            fontFamilyName:="Segoe UI",
'            fontStyle:=FontStyle.Bold,
'            faktorSymbolSize:=colors.FaktorTextSize)

'    End Sub

'    '
'    ''' <summary>
'    ''' Skaliert ein Rechteck um seinen Mittelpunkt.
'    ''' </summary>
'    Private Shared Function ScaleRectAroundCenter(source As Rectangle, factor As Double) As Rectangle

'        Dim centerX As Double = source.Left + (source.Width / 2.0R)
'        Dim centerY As Double = source.Top + (source.Height / 2.0R)

'        Dim newWidth As Integer = Math.Max(1, CInt(Math.Round(source.Width * factor)))
'        Dim newHeight As Integer = Math.Max(1, CInt(Math.Round(source.Height * factor)))

'        Dim newLeft As Integer = CInt(Math.Round(centerX - (newWidth / 2.0R)))
'        Dim newTop As Integer = CInt(Math.Round(centerY - (newHeight / 2.0R)))

'        Return New Rectangle(newLeft, newTop, newWidth, newHeight)

'    End Function

'End Class