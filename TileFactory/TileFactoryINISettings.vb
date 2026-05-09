Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

#Disable Warning IDE0079
#Disable Warning IDE1006

Public Module TileFactoryINISettings
    Private _tile_TextUseSegoeUISymbol As Boolean
    Public Property Tile_TextUseSegoeUISymbol As Boolean
        Get
            Return _tile_TextUseSegoeUISymbol
        End Get
        Set(value As Boolean)
            _tile_TextUseSegoeUISymbol = value
        End Set
    End Property
End Module
