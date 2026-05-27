Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

Imports System.Drawing

'
''' <summary>
''' Zentrale Erzeugung von Layoutdaten.
''' Hier kann später ohne Eingriff in den Rendercode ein Cache ergänzt werden.
''' </summary>
Friend Module TileLayoutFactory
    '
    ''' <summary>
    ''' Erzeugt ein Layout direkt aus Breite und Höhe.
    ''' </summary>
    Public Function Create(steinSize As Size, steinBasisSize As Size) As TileLayout

        Return New TileLayout(steinSize, steinBasisSize)

    End Function

    '
    ''' <summary>
    ''' Erzeugt ein Layout aus einem RenderRequest.
    ''' </summary>
    Public Function Create(request As TileRequest) As TileLayout

        If request Is Nothing Then Throw New ArgumentNullException(NameOf(request))

        Return Create(request.SteinSize, request.SteinBasisSize)

    End Function

End Module
