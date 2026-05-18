Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports MahjongGK.Contracts.GlobalEnum

'Namespace TileFactory Das ist der StammNameSpace

Public Module TileFactoryAPI

    'Private GHOSTTRANSPARENCY As Single = 0.35 '' Alpha-Faktor: 1.0 = deckend, 0.0 = unsichtbar
    'Private GHOSTLIGHTEN As Single = 0.35 'RGB aufhellen  0.0 = unverändert, 1.0 = weiß

    Public Sub EnsureRuntimeTileColorsSynchronized(dontOverwriteExistingTileColorsFiles As Boolean)
        TileColors.EnsureRuntimeTileColorsSynchronized(dontOverwriteExistingTileColorsFiles)
    End Sub

    Private _manager As TileFactoryManager = Nothing

    '
    ''' <summary>
    ''' Aufruf aus frmMain.Load heraus.
    ''' </summary>
    Public Sub Initialisierung()

    End Sub

    '
    ''' <summary>
    ''' Räumt die TileFactory am Programmende auf.
    ''' Aus frmMain.Closing aufrufen.
    ''' </summary>
    Public Sub DisposeAll()

        If _manager IsNot Nothing Then
            _manager.DisposeAll()
            _manager = Nothing
        End If

    End Sub

    '
    ''' <summary>
    ''' Liefert eine gecachte Steinbitmap für Spiel oder Editor. 
    ''' Existiert der Stein noch nicht im Cache, wird er bei Bedarf
    ''' erzeugt und anschließend im Cache gehalten.
    ''' </summary>
    ''' <remarks>
    ''' Besitzer der zurückgegebenen Bitmap ist immer das Cache.
    ''' Die Bitmap darf vom Aufrufer weder verändert noch disposed
    ''' werden. Aufgeräumt wird ausschließlich intern
    ''' beim Größenwechsel und beim Programmende über
    ''' TileFactoryAPI.DisposeAll
    ''' </remarks>
    Public Function GetTile(request As TileRequest) As Bitmap

        If request Is Nothing Then
            Throw New ArgumentNullException(NameOf(request))
        End If

        If _manager Is Nothing Then
            _manager = New TileFactoryManager
        End If

        Select Case request.SteinStatus
            Case SteinStatus.I00Unsichtbar,
                 SteinStatus.I10Reserve1,
                 SteinStatus.I11Reserve2

                Return Nothing
        End Select

        If request.AktRenderMode = AktRenderMode.None Then
            Return Nothing
        End If

        If Not request.Ghost Then
            Dim bmp As Bitmap = _manager.GetTile(request)
            Return bmp
        Else

            'Geister gibt es immer nur zum Normalstatus
            request.SetSteinStatusToI01Normal()

            Dim bmpGhost As Bitmap =
                GhostTileBitmapCache.GetGhostBitmap(request,
                Function(req As TileRequest) _manager.GetTile(req))

            Return bmpGhost

        End If

    End Function

    Public Function GetTileDeepCopy(request As TileRequest) As Bitmap
        Dim bmp As Bitmap = GetTile(request)
        With bmp
            Return .Clone(New Rectangle(0, 0, .Width, .Height), .PixelFormat)
        End With
    End Function

    '
    ''' <summary>
    ''' Liefert den aktuell gemerkten Layoutdatensatz für Spiel oder Editor.
    ''' </summary>
    ''' <remarks>
    ''' Der Layoutdatensatz gehört der TileFactory-Verwaltung.
    ''' Er wird intern gehalten und bei Größenwechsel zusammen mit den Caches
    ''' verworfen und neu aufgebaut.
    ''' </remarks>
    Public Function GetAktLayout(aktRenderMode As AktRenderMode) As TileLayout

        If _manager Is Nothing Then
            Throw New InvalidOperationException("TileFactoryAPI.Initialisierung(useGoogleFont) muss vorher aufgerufen werden.")
        End If

        Return _manager.GetAktLayout(aktRenderMode)

    End Function

    Public Function GetLayout(steinSize As Size, steinBasisSize As Size) As TileLayout

        If _manager Is Nothing Then
            Throw New InvalidOperationException("TileFactoryAPI.Initialisierung(useGoogleFont) muss vorher aufgerufen werden.")
        End If

        Return TileLayoutFactory.Create(steinSize, steinBasisSize)

    End Function

    '
    ''' <summary>
    ''' Liefert die effektive Schatten-/Tiefengröße für einen Stein.
    ''' Ohne Symbol-Font-Berechnung.
    ''' </summary>
    Public Function GetShadowSize(steinSize As Size, steinBasisSize As Size) As Size
        Return TileLayout.GetShadowSize(steinSize, steinBasisSize)
    End Function

    Private Function GetManager() As TileFactoryManager

        If _manager Is Nothing Then
            Throw New InvalidOperationException("TileFactoryAPI.Initialisierung(useGoogleFont) muss vor dem ersten Zugriff aufgerufen werden.")
        End If

        Return _manager

    End Function

    Public Function DebugInfoString() As String

        If _manager Is Nothing Then
            Return String.Empty
        End If

        Return _manager.DebugInfoString()

    End Function

#Region "Helfer"

    Private Function CreateRoundedRectPath(rect As Rectangle, radius As Integer) As GraphicsPath

        Dim gp As New GraphicsPath()

        If rect.Width <= 0 OrElse rect.Height <= 0 Then
            Return gp
        End If

        If radius <= 0 Then
            gp.AddRectangle(rect)
            gp.CloseFigure()
            Return gp
        End If

        Dim d As Integer = radius * 2
        If d > rect.Width Then d = rect.Width
        If d > rect.Height Then d = rect.Height

        If d < 2 Then
            gp.AddRectangle(rect)
            gp.CloseFigure()
            Return gp
        End If

        Dim arc As New Rectangle(rect.X, rect.Y, d, d)

        gp.AddArc(arc, 180, 90)
        arc.X = rect.Right - d
        gp.AddArc(arc, 270, 90)
        arc.Y = rect.Bottom - d
        gp.AddArc(arc, 0, 90)
        arc.X = rect.X
        gp.AddArc(arc, 90, 90)

        gp.CloseFigure()
        Return gp

    End Function

    '
    Public Function DeepCopyTransparenceFast(bmpSrc As Bitmap, transparence As Single, lighten As Single) As Bitmap

        Dim bmpDst As Bitmap = New Bitmap(bmpSrc.Width, bmpSrc.Height, bmpSrc.PixelFormat) ' CType(bmpSrc.Clone(), Bitmap)
        Using gfx As Graphics = Graphics.FromImage(bmpDst)

            Using ia As New Imaging.ImageAttributes()

                'Wie erweitere ich diese Matrix um die Bitmap heller zu machen?
                Dim cm As New Imaging.ColorMatrix()
                cm.Matrix33 = transparence  ' Alpha-Faktor: 1.0 = deckend, 0.0 = unsichtbar

                cm.Matrix40 = lighten        ' Rot aufhellen
                cm.Matrix41 = lighten        ' Grün aufhellen
                cm.Matrix42 = lighten        ' Blau aufhellen

                ia.SetColorMatrix(cm)

                gfx.DrawImage(bmpSrc, New Rectangle(0, 0, bmpSrc.Width, bmpSrc.Height), 0, 0, bmpSrc.Width, bmpSrc.Height, GraphicsUnit.Pixel, ia)
            End Using
        End Using

        Return bmpDst

    End Function

#End Region

End Module