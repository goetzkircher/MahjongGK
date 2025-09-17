Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On
Namespace Spielfeld
    '
    ''' <summary>
    ''' Platzierungs-Helper ohne Koordinatensystem-Wechsel (0-basiert, Schritt = 1).
    ''' Liefert TopLeft und RU; optional: Iteration über alle belegten Zellen für IsFreePlace().
    ''' </summary>
    Public Module PlacementSameGrid

        '---------------------------
        ' Public API
        '---------------------------

        ''' <summary>
        ''' Berechnet die Ziel-Position (linke obere Ecke) im Spielfeld so, dass
        ''' der gewählte Referenzpunkt (im Quellrechteck) deckungsgleich auf der
        ''' gewählten Startposition liegt. Offsets werden addiert.
        ''' Rückgabe: (TopLeft, RU, Fits). Fits = False, wenn der Bereich außerhalb der Grenzen liegt.
        ''' </summary>
        Public Function ComputePlacementTopLeftSameGrid(srcLO As Point,
                                                    srcRU As Point,
                                                    refPoint As ReferencePoint.RefPoint,
                                                    startPoint As StartingPoint.PositionEnum,
                                                    xMax As Integer,
                                                    yMax As Integer,
                                                    offsetX As Integer,
                                                    offsetY As Integer) _
                                                    As (topLeft As Point, ru As Point, fits As Boolean)

            ' 1) Sanity & Normalisierung
            Dim norm As (lo As Point, ru As Point) = NormalizeRect(srcLO, srcRU)
            Dim lo As Point = norm.lo
            Dim ru As Point = norm.ru

            ' 2) Anchor / Referenzpunkt innerhalb des Quellrechtecks
            Dim anchor As Point = GetRefAnchor(lo, ru, refPoint)

            ' 3) Zielzelle für Startpunkt (gleiches Raster)
            Dim startCell As Point = GetStartCell(startPoint, xMax, yMax)

            ' 4) TopLeft so verschieben, dass Anchor auf Start fällt (plus Offsets)
            Dim anchorRel As New Point(anchor.X - lo.X, anchor.Y - lo.Y)
            Dim targetTopLeft As New Point(startCell.X - anchorRel.X + offsetX,
                                       startCell.Y - anchorRel.Y + offsetY)

            ' 5) Bounds prüfen (inkl. RU-Ecke)
            Dim w As Integer = (ru.X - lo.X) + 1
            Dim h As Integer = (ru.Y - lo.Y) + 1
            Dim targetRU As New Point(targetTopLeft.X + w - 1,
                                  targetTopLeft.Y + h - 1)

            Dim fits As Boolean =
            targetTopLeft.X >= 0 AndAlso targetTopLeft.Y >= 0 AndAlso
            targetRU.X <= xMax AndAlso targetRU.Y <= yMax

            Return (If(fits, targetTopLeft, New Point(-1, -1)),
                targetRU,
                fits)
        End Function

        ''' <summary>
        ''' Überladung mit Breite/Höhe statt srcRU.
        ''' width/height sind Anzahl Zellen.
        ''' </summary>
        Public Function ComputePlacementTopLeftSameGrid(srcLO As Point,
                                                    width As Integer,
                                                    height As Integer,
                                                    refPoint As ReferencePoint.RefPoint,
                                                    startPoint As StartingPoint.PositionEnum,
                                                    xMax As Integer,
                                                    yMax As Integer,
                                                    offsetX As Integer,
                                                    offsetY As Integer) _
                                                    As (topLeft As Point, ru As Point, fits As Boolean)
            Dim srcRU As New Point(srcLO.X + width - 1, srcLO.Y + height - 1)
            Return ComputePlacementTopLeftSameGrid(srcLO, srcRU, refPoint, startPoint, xMax, yMax, offsetX, offsetY)
        End Function

        '---------------------------
        ' Iteration über belegte Zellen
        '---------------------------

        '
        ''' <summary>
        ''' Enumeriert alle Zellen innerhalb [topLeft..ru] (inklusive) auf Ebene z.
        ''' Ideal zum sequentiellen Prüfen via IsFreePlace(x, y, z).
        ''' </summary>
        Public Iterator Function CellsInArea(topLeft As Point,
                                         ru As Point,
                                         z As Integer) As IEnumerable(Of (x As Integer, y As Integer, z As Integer))
            Dim lo As Point = NormalizeRect(topLeft, ru).lo
            Dim hi As Point = NormalizeRect(topLeft, ru).ru
            For y As Integer = lo.Y To hi.Y
                For x As Integer = lo.X To hi.X
                    Yield (x, y, z)
                Next
            Next
        End Function

        '
        ''' <summary>
        ''' Wie CellsInArea, aber Rückgabe als Point (ohne Z), falls Z getrennt gehandhabt wird.
        ''' </summary>
        Public Iterator Function CellsInArea2D(topLeft As Point,
                                           ru As Point) As IEnumerable(Of Point)
            Dim lo As Point = NormalizeRect(topLeft, ru).lo
            Dim hi As Point = NormalizeRect(topLeft, ru).ru
            For y As Integer = lo.Y To hi.Y
                For x As Integer = lo.X To hi.X
                    Yield New Point(x, y)
                Next
            Next
        End Function

        '---------------------------
        ' Convenience
        '---------------------------

        Public Function ToRectangle(topLeft As Point, ru As Point) As Rectangle
            Dim n As (lo As Point, ru As Point) = NormalizeRect(topLeft, ru)
            Return Rectangle.FromLTRB(n.lo.X, n.lo.Y, n.ru.X + 1, n.ru.Y + 1)
        End Function

        '---------------------------
        ' Helpers (privat)
        '---------------------------

        Private Function NormalizeRect(a As Point, b As Point) As (lo As Point, ru As Point)
            Dim loX As Integer = Math.Min(a.X, b.X)
            Dim loY As Integer = Math.Min(a.Y, b.Y)
            Dim ruX As Integer = Math.Max(a.X, b.X)
            Dim ruY As Integer = Math.Max(a.Y, b.Y)
            Return (New Point(loX, loY), New Point(ruX, ruY))
        End Function

        Private Function GetRefAnchor(lo As Point,
                                  ru As Point,
                                  refPoint As ReferencePoint.RefPoint) As Point
            Dim left As Integer = lo.X
            Dim top As Integer = lo.Y
            Dim right As Integer = ru.X
            Dim bottom As Integer = ru.Y
            Dim cx As Integer = (left + right) \ 2
            Dim cy As Integer = (top + bottom) \ 2

            Select Case refPoint
                Case ReferencePoint.RefPoint.EckeLO : Return New Point(left, top)
                Case ReferencePoint.RefPoint.EckeRO : Return New Point(right, top)
                Case ReferencePoint.RefPoint.EckeRU : Return New Point(right, bottom)
                Case ReferencePoint.RefPoint.EckeLU : Return New Point(left, bottom)
                Case ReferencePoint.RefPoint.Center : Return New Point(cx, cy)
                Case Else : Return New Point(cx, cy)
            End Select
        End Function

        Private Function GetStartCell(startPoint As StartingPoint.PositionEnum,
                                  xMax As Integer,
                                  yMax As Integer) As Point
            If xMax < 0 OrElse yMax < 0 Then Return New Point(0, 0)

            Dim minX As Integer = 0
            Dim minY As Integer = 0
            Dim maxX As Integer = xMax
            Dim maxY As Integer = yMax
            Dim midX As Integer = (minX + maxX) \ 2
            Dim midY As Integer = (minY + maxY) \ 2

            ' halbe Strecken
            Dim qxL As Integer = (minX + midX) \ 2
            Dim qxR As Integer = (midX + maxX) \ 2
            Dim qyO As Integer = (minY + midY) \ 2
            Dim qyU As Integer = (midY + maxY) \ 2

            Select Case startPoint
                Case StartingPoint.PositionEnum.EckeLO : Return New Point(minX, minY)
                Case StartingPoint.PositionEnum.EckeRO : Return New Point(maxX, minY)
                Case StartingPoint.PositionEnum.EckeRU : Return New Point(maxX, maxY)
                Case StartingPoint.PositionEnum.EckeLU : Return New Point(minX, maxY)

                Case StartingPoint.PositionEnum.MitteO : Return New Point(midX, minY)
                Case StartingPoint.PositionEnum.MitteR : Return New Point(maxX, midY)
                Case StartingPoint.PositionEnum.MitteU : Return New Point(midX, maxY)
                Case StartingPoint.PositionEnum.MitteL : Return New Point(minX, midY)

                Case StartingPoint.PositionEnum.Center : Return New Point(midX, midY)

                Case StartingPoint.PositionEnum.CenterLO : Return New Point(qxL, qyO)
                Case StartingPoint.PositionEnum.CenterRO : Return New Point(qxR, qyO)
                Case StartingPoint.PositionEnum.CenterRU : Return New Point(qxR, qyU)
                Case StartingPoint.PositionEnum.CenterLU : Return New Point(qxL, qyU)

                Case Else : Return New Point(midX, midY)
            End Select
        End Function

    End Module

End Namespace