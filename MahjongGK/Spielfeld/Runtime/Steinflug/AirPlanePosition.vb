Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

'
''' <summary>
''' Pfad: MahjongGK/Runtime/Steinflug
'''
''' Bestimmt die aktuelle Renderposition eines an die Maus gekoppelten fliegenden Steins.
''' Der Maus-Ankerpunkt startet an der echten Klickposition im Stein und wandert bei
''' Mausbewegung weich auf einen lageabhängigen Sollwert.
''' </summary>
Public NotInheritable Class AirPlanePosition

    Private Const DELTASTEPS As Integer = 10

    Private ReadOnly _planeWidth As Integer
    Private ReadOnly _planeHeight As Integer
    Private ReadOnly _stockBoundaryY As Integer

    Private ReadOnly _targetBoardX As Double
    Private ReadOnly _targetBoardY As Double
    Private ReadOnly _targetStockX As Double
    Private ReadOnly _targetStockY As Double

    Private _lastMousePos As Point

    Private _anchorX As Double
    Private _anchorY As Double

    Private _targetAnchorX As Double
    Private _targetAnchorY As Double
    Private _stepsRemaining As Integer

    '
    ''' <summary>
    ''' Initialisiert die Positionssteuerung für einen an die Maus gekoppelten Stein.
    ''' </summary>
    ''' <param name="rectPlane">Aktuelle Position und Größe des Steins.</param>
    ''' <param name="ptMousePos">Mausposition beim Anklicken des Steins.</param>
    ''' <param name="rxStockUsed">Grenzrechteck, dessen Y-Wert die Umschaltgrenze definiert.</param>
    Public Sub New(rectPlane As Rectangle, ptMousePos As Point, rxStockUsed As RectangleX)

        _planeWidth = rectPlane.Width
        _planeHeight = rectPlane.Height
        _stockBoundaryY = rxStockUsed.Y

        _targetBoardX = CDbl(_planeWidth \ 4)
        _targetBoardY = CDbl(_planeHeight \ 4)

        _targetStockX = CDbl(_planeWidth \ 2)
        _targetStockY = CDbl(_planeHeight \ 2)

        _lastMousePos = ptMousePos

        'Initialer realer Ankerpunkt relativ zum Stein
        _anchorX = CDbl(ptMousePos.X - rectPlane.X)
        _anchorY = CDbl(ptMousePos.Y - rectPlane.Y)

        'Passenden Sollanker für die aktuelle Startlage setzen
        If rectPlane.Y >= _stockBoundaryY Then
            _targetAnchorX = _targetBoardX
            _targetAnchorY = _targetBoardY
        Else
            _targetAnchorX = _targetStockX
            _targetAnchorY = _targetStockY
        End If

        If Not IsAnchorAtTarget() Then
            _stepsRemaining = DELTASTEPS
        End If

    End Sub

    '
    ''' <summary>
    ''' Liefert die aktuelle Renderposition des fliegenden Steins.
    ''' Ein Anker-Verschiebungsschritt erfolgt nur dann, wenn sich die Maus
    ''' seit dem letzten Aufruf tatsächlich bewegt hat.
    ''' </summary>
    Public Function GetAktRectPlane(aktMousePos As Point) As Rectangle

        Dim mouseMoved As Boolean =
            (aktMousePos.X <> _lastMousePos.X) OrElse (aktMousePos.Y <> _lastMousePos.Y)

        If mouseMoved Then

            Dim rectTopWithCurrentAnchor As Integer =
                aktMousePos.Y - CInt(Math.Round(_anchorY, MidpointRounding.AwayFromZero))

            Dim newTargetAnchorX As Double
            Dim newTargetAnchorY As Double

            If rectTopWithCurrentAnchor >= _stockBoundaryY Then
                newTargetAnchorX = _targetBoardX
                newTargetAnchorY = _targetBoardY
            Else
                newTargetAnchorX = _targetStockX
                newTargetAnchorY = _targetStockY
            End If

            If (newTargetAnchorX <> _targetAnchorX) OrElse (newTargetAnchorY <> _targetAnchorY) Then
                _targetAnchorX = newTargetAnchorX
                _targetAnchorY = newTargetAnchorY
                _stepsRemaining = DELTASTEPS
            End If

            If _stepsRemaining > 0 Then
                StepAnchorTowardsTarget()
            End If

            _lastMousePos = aktMousePos

        End If

        Return New Rectangle(
            aktMousePos.X - CInt(Math.Round(_anchorX, MidpointRounding.AwayFromZero)),
            aktMousePos.Y - CInt(Math.Round(_anchorY, MidpointRounding.AwayFromZero)),
            _planeWidth,
            _planeHeight
        )

    End Function

    Private Sub StepAnchorTowardsTarget()

        If _stepsRemaining <= 0 Then
            Exit Sub
        End If

        _anchorX += (_targetAnchorX - _anchorX) / CDbl(_stepsRemaining)
        _anchorY += (_targetAnchorY - _anchorY) / CDbl(_stepsRemaining)

        _stepsRemaining -= 1

        If _stepsRemaining <= 0 Then
            _anchorX = _targetAnchorX
            _anchorY = _targetAnchorY
            _stepsRemaining = 0
        End If

    End Sub

    Private Function IsAnchorAtTarget() As Boolean
        Return (_anchorX = _targetAnchorX) AndAlso (_anchorY = _targetAnchorY)
    End Function

End Class
