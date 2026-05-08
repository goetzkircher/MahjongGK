Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

Imports MahjongGK.Contracts

Namespace Spielfeld
    Public Module ArrFBResizeHelper

        '
        ''' <summary>
        ''' Prüft, ob arrFB auf die angegebene Zielgröße gebracht werden kann.
        ''' Diese Überladung verändert nur die UBounds:
        ''' X nur rechts, Y nur unten, Z nur oben.
        ''' </summary>
        Public Function RedimArrFBIsPossible(ByVal arrFB(,,) As Integer,
                                             ByVal sizeInSteinen As Triple,
                                             ByVal backdorIsOpen As Boolean) As Boolean

            If Not IsTargetSizeValid(sizeInSteinen, backdorIsOpen) Then
                Return False
            End If

            If arrFB Is Nothing Then
                Return False
            End If

            Dim xUbndNew As Integer = (sizeInSteinen.x * 2) + 1
            Dim yUbndNew As Integer = (sizeInSteinen.y * 2) + 1
            Dim zUbndNew As Integer = sizeInSteinen.z

            Dim xUbndOld As Integer = arrFB.GetUpperBound(0)
            Dim yUbndOld As Integer = arrFB.GetUpperBound(1)
            Dim zUbndOld As Integer = arrFB.GetUpperBound(2)

            'Wenn verkleinert wird, darf nichts abgeschnitten werden.
            Dim x As Integer
            Dim y As Integer
            Dim z As Integer

            For z = 0 To zUbndOld
                For y = 0 To yUbndOld
                    For x = 0 To xUbndOld
                        If arrFB(x, y, z) <> 0 Then

                            If x > xUbndNew Then Return False
                            If y > yUbndNew Then Return False
                            If z > zUbndNew Then Return False

                            'Randfreiheit muss im neuen Feld erhalten bleiben
                            If x = 0 OrElse x = xUbndNew Then Return False
                            If y = 0 OrElse y = yUbndNew Then Return False
                        End If
                    Next
                Next
            Next

            Return True
        End Function

        '
        ''' <summary>
        ''' Prüft, ob arrFB um die angegebenen Spalten/Zeilen/Ebenen
        ''' vergrößert oder verkleinert werden kann.
        ''' Die addOrRemove-Werte gelten direkt in arrFB-Feldern.
        ''' </summary>
        Public Function RedimArrFBIsPossible(ByVal arrFB(,,) As Integer,
                                             ByVal addOrRemoveLeft As Integer,
                                             ByVal addOrRemoveTop As Integer,
                                             ByVal addOrRemoveRight As Integer,
                                             ByVal addOrRemoveBottom As Integer,
                                             ByVal addOrRemoveLayer As Integer) As Boolean

            If arrFB Is Nothing Then
                Return False
            End If

            Dim xUbndOld As Integer = arrFB.GetUpperBound(0)
            Dim yUbndOld As Integer = arrFB.GetUpperBound(1)
            Dim zUbndOld As Integer = arrFB.GetUpperBound(2)

            Dim xUbndNew As Integer = xUbndOld + addOrRemoveLeft + addOrRemoveRight
            Dim yUbndNew As Integer = yUbndOld + addOrRemoveTop + addOrRemoveBottom
            Dim zUbndNew As Integer = zUbndOld + addOrRemoveLayer

            Dim sizeNew As Triple = GetSizeInSteinenFromUBounds(xUbndNew, yUbndNew, zUbndNew)

            If Not IsTargetSizeValid(sizeNew, False) Then
                Return False
            End If

            Return CanMoveAllOccupiedCells(arrFB,
                                           addOrRemoveLeft,
                                           addOrRemoveTop,
                                           xUbndNew,
                                           yUbndNew,
                                           zUbndNew)
        End Function

        '
        ''' <summary>
        ''' Erstellt ein neues arrFB mit der angegebenen Zielgröße.
        ''' Diese Überladung verändert nur die UBounds:
        ''' X nur rechts, Y nur unten, Z nur oben.
        ''' Gibt Nothing zurück, wenn die Änderung unzulässig ist.
        ''' </summary>
        Public Function RedimArrFB(ByVal arrFB(,,) As Integer,
                                   ByVal sizeInSteinen As Triple,
                                   ByVal backdorIsOpen As Boolean) As Integer(,,)

            If Not RedimArrFBIsPossible(arrFB, sizeInSteinen, backdorIsOpen) Then
                Return Nothing
            End If

            Dim xUbndNew As Integer = (sizeInSteinen.x * 2) + 1
            Dim yUbndNew As Integer = (sizeInSteinen.y * 2) + 1
            Dim zUbndNew As Integer = sizeInSteinen.z

            Dim arrNew(xUbndNew, yUbndNew, zUbndNew) As Integer

            If arrFB Is Nothing Then
                Return arrNew
            End If

            CopyArrFBToNewArray(arrFB,
                                arrNew,
                                0,
                                0)

            Return arrNew
        End Function

        '
        ''' <summary>
        ''' Erstellt ein neues arrFB mit relativer Änderung an links/oben/rechts/unten/z.
        ''' Die addOrRemove-Werte gelten direkt in arrFB-Feldern.
        ''' Gibt Nothing zurück, wenn die Änderung unzulässig ist.
        ''' </summary>
        Public Function RedimArrFB(ByVal arrFB(,,) As Integer,
                                   ByVal addOrRemoveLeft As Integer,
                                   ByVal addOrRemoveTop As Integer,
                                   ByVal addOrRemoveRight As Integer,
                                   ByVal addOrRemoveBottom As Integer,
                                   ByVal addOrRemoveLayer As Integer) As Integer(,,)

            If arrFB Is Nothing Then
                Return Nothing
            End If

            If Not RedimArrFBIsPossible(arrFB,
                                        addOrRemoveLeft,
                                        addOrRemoveTop,
                                        addOrRemoveRight,
                                        addOrRemoveBottom,
                                        addOrRemoveLayer) Then
                Return Nothing
            End If

            Dim xUbndOld As Integer = arrFB.GetUpperBound(0)
            Dim yUbndOld As Integer = arrFB.GetUpperBound(1)
            Dim zUbndOld As Integer = arrFB.GetUpperBound(2)

            Dim xUbndNew As Integer = xUbndOld + addOrRemoveLeft + addOrRemoveRight
            Dim yUbndNew As Integer = yUbndOld + addOrRemoveTop + addOrRemoveBottom
            Dim zUbndNew As Integer = zUbndOld + addOrRemoveLayer

            Dim arrNew(xUbndNew, yUbndNew, zUbndNew) As Integer

            CopyArrFBToNewArray(arrFB,
                                arrNew,
                                addOrRemoveLeft,
                                addOrRemoveTop)

            Return arrNew
        End Function

        '
        ''' <summary>
        ''' Verkleinert arrFB auf die kleinstmögliche Größe, ohne belegte Felder
        ''' zu verlieren, und unter Einhaltung aller Rand- und Mindestbedingungen.
        ''' Intern wird RedimArrFB(...addOrRemove...) verwendet.
        ''' </summary>
        Public Function ArrFBTrimm(ByVal arrFB(,,) As Integer) As Integer(,,)

            If arrFB Is Nothing Then
                Return Nothing
            End If

            Dim xUbndOld As Integer = arrFB.GetUpperBound(0)
            Dim yUbndOld As Integer = arrFB.GetUpperBound(1)
            Dim zUbndOld As Integer = arrFB.GetUpperBound(2)

            Dim minXOcc As Integer = Integer.MaxValue
            Dim maxXOcc As Integer = Integer.MinValue
            Dim minYOcc As Integer = Integer.MaxValue
            Dim maxYOcc As Integer = Integer.MinValue
            Dim maxZOcc As Integer = Integer.MinValue

            Dim hasOccupied As Boolean = False

            Dim x As Integer
            Dim y As Integer
            Dim z As Integer

            For z = 0 To zUbndOld
                For y = 0 To yUbndOld
                    For x = 0 To xUbndOld
                        If arrFB(x, y, z) <> 0 Then
                            hasOccupied = True

                            If x < minXOcc Then minXOcc = x
                            If x > maxXOcc Then maxXOcc = x

                            If y < minYOcc Then minYOcc = y
                            If y > maxYOcc Then maxYOcc = y

                            If z > maxZOcc Then maxZOcc = z
                        End If
                    Next
                Next
            Next

            Dim xUbndMin As Integer = (MJ_STEINE_MINX * 2) + 1
            Dim yUbndMin As Integer = (MJ_STEINE_MINY * 2) + 1
            Dim zUbndMin As Integer = MJ_STEINE_MINZ

            Dim addOrRemoveLeft As Integer
            Dim addOrRemoveTop As Integer
            Dim addOrRemoveRight As Integer
            Dim addOrRemoveBottom As Integer
            Dim addOrRemoveLayer As Integer

            If Not hasOccupied Then
                'Kein Inhalt: einfach auf Mindestgröße verkleinern.
                addOrRemoveLeft = 0
                addOrRemoveTop = 0
                addOrRemoveRight = xUbndMin - xUbndOld
                addOrRemoveBottom = yUbndMin - yUbndOld
                addOrRemoveLayer = zUbndMin - zUbndOld
            Else
                '--------------------------------------------------
                ' Links/oben so weit wie möglich entfernen,
                ' damit das erste belegte Feld jeweils auf 1 liegt.
                '--------------------------------------------------
                addOrRemoveLeft = 1 - minXOcc
                addOrRemoveTop = 1 - minYOcc

                '--------------------------------------------------
                ' Mindest-UBounds aus belegtem Bereich bestimmen:
                ' links/rechts bzw. oben/unten je 1 Randfeld frei.
                '--------------------------------------------------
                Dim xUbndNeeded As Integer = (maxXOcc - minXOcc) + 2
                Dim yUbndNeeded As Integer = (maxYOcc - minYOcc) + 2
                Dim zUbndNeeded As Integer = maxZOcc

                If xUbndNeeded < xUbndMin Then xUbndNeeded = xUbndMin
                If yUbndNeeded < yUbndMin Then yUbndNeeded = yUbndMin
                If zUbndNeeded < zUbndMin Then zUbndNeeded = zUbndMin

                '--------------------------------------------------
                ' Rechte / untere / obere Z-Grenze passend setzen.
                ' newUBound = oldUBound + addLeft + addRight
                '--------------------------------------------------
                addOrRemoveRight = xUbndNeeded - xUbndOld - addOrRemoveLeft
                addOrRemoveBottom = yUbndNeeded - yUbndOld - addOrRemoveTop
                addOrRemoveLayer = zUbndNeeded - zUbndOld
            End If

            Return RedimArrFB(arrFB,
                      addOrRemoveLeft,
                      addOrRemoveTop,
                      addOrRemoveRight,
                      addOrRemoveBottom,
                      addOrRemoveLayer)

        End Function
        '
        ''' <summary>
        ''' Prüft Mindest- und ggf. Maximalgrößen.
        ''' </summary>
        Private Function IsTargetSizeValid(ByVal sizeInSteinen As Triple,
                                           ByVal backdorIsOpen As Boolean) As Boolean

            If sizeInSteinen Is Nothing Then
                Return False
            End If

            If sizeInSteinen.x < MJ_STEINE_MINX Then Return False
            If sizeInSteinen.y < MJ_STEINE_MINY Then Return False
            If sizeInSteinen.z < MJ_STEINE_MINZ Then Return False

            If Not backdorIsOpen Then
                If sizeInSteinen.x > MJ_STEINE_MAXX Then Return False
                If sizeInSteinen.y > MJ_STEINE_MAXY Then Return False
                If sizeInSteinen.z > MJ_STEINE_MAXZ Then Return False
            End If

            Return True
        End Function

        '
        ''' <summary>
        ''' Wandelt UBounds in die Spielgröße in Steinen um.
        ''' </summary>
        Private Function GetSizeInSteinenFromUBounds(ByVal xUbnd As Integer,
                                                 ByVal yUbnd As Integer,
                                                 ByVal zUbnd As Integer) As Triple

            'xUbnd = sizeX * 2 + 1  =>  sizeX = (xUbnd - 1) \ 2
            'yUbnd = sizeY * 2 + 1  =>  sizeY = (yUbnd - 1) \ 2
            Return New Triple((xUbnd - 1) \ 2,
                              (yUbnd - 1) \ 2,
                              zUbnd)
        End Function

        '
        ''' <summary>
        ''' Prüft, ob alle belegten Felder nach der Verschiebung noch
        ''' innerhalb des neuen Feldes liegen und nicht auf dem Außenrand.
        ''' </summary>
        Private Function CanMoveAllOccupiedCells(ByVal arrFB(,,) As Integer,
                                             ByVal shiftX As Integer,
                                             ByVal shiftY As Integer,
                                             ByVal xUbndNew As Integer,
                                             ByVal yUbndNew As Integer,
                                             ByVal zUbndNew As Integer) As Boolean

            Dim xUbndOld As Integer = arrFB.GetUpperBound(0)
            Dim yUbndOld As Integer = arrFB.GetUpperBound(1)
            Dim zUbndOld As Integer = arrFB.GetUpperBound(2)

            Dim x As Integer
            Dim y As Integer
            Dim z As Integer

            For z = 0 To zUbndOld
                For y = 0 To yUbndOld
                    For x = 0 To xUbndOld

                        If arrFB(x, y, z) <> 0 Then

                            Dim xNew As Integer = x + shiftX
                            Dim yNew As Integer = y + shiftY
                            Dim zNew As Integer = z

                            If xNew < 0 OrElse xNew > xUbndNew Then Return False
                            If yNew < 0 OrElse yNew > yUbndNew Then Return False
                            If zNew < 0 OrElse zNew > zUbndNew Then Return False

                            If xNew = 0 OrElse xNew = xUbndNew Then Return False
                            If yNew = 0 OrElse yNew = yUbndNew Then Return False
                        End If
                    Next
                Next
            Next

            Return True
        End Function

        '
        ''' <summary>
        ''' Kopiert das alte arrFB in das neue Array.
        ''' Nur X/Y werden verschoben. Z bleibt auf gleicher Ebenennummer.
        ''' </summary>
        Private Sub CopyArrFBToNewArray(ByVal arrOld(,,) As Integer,
                                    ByVal arrNew(,,) As Integer,
                                    ByVal shiftX As Integer,
                                    ByVal shiftY As Integer)

            Dim xUbndOld As Integer = arrOld.GetUpperBound(0)
            Dim yUbndOld As Integer = arrOld.GetUpperBound(1)
            Dim zUbndOld As Integer = arrOld.GetUpperBound(2)

            Dim xUbndNew As Integer = arrNew.GetUpperBound(0)
            Dim yUbndNew As Integer = arrNew.GetUpperBound(1)
            Dim zUbndNew As Integer = arrNew.GetUpperBound(2)

            Dim x As Integer
            Dim y As Integer
            Dim z As Integer

            For z = 0 To zUbndOld
                For y = 0 To yUbndOld
                    For x = 0 To xUbndOld

                        Dim xNew As Integer = x + shiftX
                        Dim yNew As Integer = y + shiftY
                        Dim zNew As Integer = z

                        If xNew >= 0 AndAlso xNew <= xUbndNew AndAlso
                           yNew >= 0 AndAlso yNew <= yUbndNew AndAlso
                           zNew >= 0 AndAlso zNew <= zUbndNew Then

                            arrNew(xNew, yNew, zNew) = arrOld(x, y, z)
                        End If
                    Next
                Next
            Next
        End Sub

    End Module
End Namespace
