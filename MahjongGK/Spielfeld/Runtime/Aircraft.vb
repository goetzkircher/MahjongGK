Namespace Spielfeld

    ''' <summary>
    ''' Klasse, die für den Steinflug einen einzelnen fliegenden Stein hält.
    ''' </summary>
    Public Class Aircraft
        Implements IDisposable

        Private _disposed As Boolean
        Private ReadOnly _sfd As SFDaten

        Public Sub New(owner As SFDaten)
            _sfd = owner
        End Sub

        Public Property SrcRect As Rectangle
        Public Property DstRect As Rectangle

        Public Property aktMousePos As Point = New Point

        Public Property planeModus As AirplaneModus
        Public Property portSrcEnum As AirportSrc
        Public Property portDstEnum As AirportDst
        Public Property idxStockOrHistorySrc As Integer
        Public Property idxStockOrHistoryDst As Integer
        Public Property f1bSrc As Triple
        Public Property f2bSrc As Triple
        Public Property f1bDst As Triple
        Public Property f2bDst As Triple
        Public Property zorder As AirplaneZOrder
        Public Property animation As AirplaneAnimation
        '
        ' bmpOrg muss eine Kopie sein, damit alle Bitmaps gleich behandelt werden können.
        Public Property bmpOrg As Bitmap
        Public Property bmpGhost As Bitmap
        Public Property bmpSelected As Bitmap
        Public Property bmpPlacable As Bitmap

        Public Function Initialisierung() As Boolean

            ThrowIfDisposed()

            ' Falls Initialisierung mehrfach aufgerufen werden kann:
            DisposeOwnedBitmaps()

            ' Original-Bitmap nur lesen, nicht besitzen:
            'Dim bmpOriginal As Bitmap = _sfd....

            ' Daraus neue Bitmaps erzeugen:
            'bmpGhost = ...
            'bmpSelected = ...
            'bmpPlacable = ...

        End Function

        Private Sub ThrowIfDisposed()
            If _disposed Then
                Throw New ObjectDisposedException(Me.GetType().Name)
            End If
        End Sub

        Private Sub DisposeOwnedBitmaps()

            If bmpOrg IsNot Nothing Then
                bmpOrg.Dispose()
                bmpOrg = Nothing
            End If

            If bmpGhost IsNot Nothing Then
                bmpGhost.Dispose()
                bmpGhost = Nothing
            End If

            If bmpSelected IsNot Nothing Then
                bmpSelected.Dispose()
                bmpSelected = Nothing
            End If

            If bmpPlacable IsNot Nothing Then
                bmpPlacable.Dispose()
                bmpPlacable = Nothing
            End If

        End Sub

        Protected Overridable Sub Dispose(disposing As Boolean)

            If _disposed Then
                Return
            End If

            If disposing Then
                DisposeOwnedBitmaps()
            End If

            _disposed = True

        End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            Dispose(disposing:=True)
            GC.SuppressFinalize(Me)
        End Sub

    End Class

End Namespace