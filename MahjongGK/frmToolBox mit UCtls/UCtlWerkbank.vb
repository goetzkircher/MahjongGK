Public Class UCtlWerkbank

    Private _wb As Werkbank = Nothing
    Private _rnd As New Random

    Private _xAkt As Integer
    Private _yAkt As Integer
    Private _zAkt As Integer

    Private _skipEvents As Boolean = True

    Public ReadOnly Property ResultAsSteinInfosArrFB As (steinInfos As List(Of SteinInfo), arrFB As Integer(,,))
        Get
            If Not IsNothing(_wb) Then
                Return _wb.ResultAsSteinInfosArrFB
            Else
                Return Nothing
            End If
        End Get
    End Property
    '
    ''' <summary>
    ''' Das Werkstück zum Einbauen (demomode = False)
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property ResultAsWerkstück As Werkstück
        Get
            If Not IsNothing(_wb) Then
                Return _wb.ResultAsWerkstück(demoMode:=False)
            Else
                Return Nothing
            End If
        End Get
    End Property


#Region "Werkbank Bearbeitung"

    Private _aktBasisformEnum As BasisformEnum

    Public Sub SetBasisformEnum(aktBasisformEnum As BasisformEnum)



        _aktBasisformEnum = aktBasisformEnum
        ToolBox_AktBasisform = aktBasisformEnum

        If _aktBasisformEnum = BasisformEnum.Zufall Then
            _aktBasisformEnum = CType(_rnd.Next(BasisformEnum.Linie, BasisformEnum.Zufall), BasisformEnum)
        End If

        num2UpDnX.MinValue = 1
        num2UpDnY.MinValue = 1
        num2UpDnZ.MinValue = 1

        num2UpDnX.MaxValue = INI.ToolBox_FeldSizeXmax()
        num2UpDnY.MaxValue = INI.ToolBox_FeldSizeYmax()
        num2UpDnZ.MaxValue = INI.ToolBox_FeldSizeZmax()

        num2UpDnX.Value = INI.ToolBox_FeldSizeX(aktBasisformEnum)
        num2UpDnY.Value = INI.ToolBox_FeldSizeY(aktBasisformEnum)
        num2UpDnZ.Value = INI.ToolBox_FeldSizeZ(aktBasisformEnum)


        _skipEvents = False 'ab jetzt Events ncht mehr abfangen
        WerkbankMainjob()

    End Sub


    Private Sub num2UpDnWerkbankFeldSize_ValueChanged() _
        Handles num2UpDnX.ValueChanged,
                num2UpDnY.ValueChanged,
                num2UpDnZ.ValueChanged

        WerkbankMainjob()
    End Sub


    Private Sub KompassRoseXWerkbankRichtung_DirectionClick(direction As KompassRoseX.KompassXEnum) _
        Handles KompassRoseRichtung.DirectionClick
        WerkbankMainjob()
    End Sub

    Private Sub chkWerkbankVersatz_Click(sender As Object, e As EventArgs) _
        Handles chkZusatzfunktion.Click
        WerkbankMainjob()
    End Sub

    Private Sub WerkbankMainjob()

        If _skipEvents Then
            'Vorab-Events, die abgefangen werden müssen
            Exit Sub
        End If

        _xAkt = num2UpDnX.Value
        _yAkt = num2UpDnY.Value
        _zAkt = num2UpDnZ.Value


        ' Das Polling läuft bereits Spielfeld.TaktgeberModul.PaintSpielfeld_ReInitialisierung()


        Dim wb As Werkbank = Nothing

        'With wb
        '    For idxZ As Integer = .zMin To .zMax
        '        For idxX As Integer = .xMin To .xMax
        '            For idxY As Integer = .yMin To .yMax
        '                Dim tplQuestion As New Triple(idxX, idxY, idxZ)
        '                Dim steinPos3D As Triple = wb.IsValidePlace(tplQuestion)
        '                If steinPos3D.Valide = ValidePlaceEnum.Yes Then
        '                    'Stein mit zufälliger SteinIndexEnum setzen.
        '                    wb.AddSteinToSpielfeld(CType(_rnd.Next(1, 43), SteinIndexEnum), steinPos3D)
        '                End If
        '            Next
        '        Next
        '    Next
        'End With

        ''    Entweder:
        ''    Dim result As (steinInfos As List(Of SteinInfo), arrFB As Integer(,,)) = wb.ResultAsSteinInfosArrFB
        ''    Oder:
        ''    Dim result As  Werkstück = wb.ResultAsWerkstück(demoMode)
        ''    Return result

        ''End Function




        Select Case _aktBasisformEnum
            '
            Case BasisformEnum.Linie


                EnDisable("Länge", Nothing, "Höhe", Nothing, "Richtung", reducedKompass:=True)
                wb = CreateLine()

            '
            Case BasisformEnum.Winkel
                EnDisable("Länge", Nothing, "Höhe", "Steine versetzen", "Richtung", reducedKompass:=True)
                ' TODO
            '
            Case BasisformEnum.UForm
                EnDisable("Länge", "Breite", "Höhe", "Steine versetzen", "Richtung", reducedKompass:=True)
                ' TODO
            '
            Case BasisformEnum.Rechteck
                EnDisable("Länge", "Breite", "Höhe", Nothing, "Richtung", reducedKompass:=True)
                wb = CreateRechteck()
            '
            Case BasisformEnum.Kreis
                EnDisable("Länge", "Breite", "Höhe", Nothing, Nothing, reducedKompass:=True)
                ' TODO
            '
            Case BasisformEnum.Pyramide
                EnDisable("Länge", "Breite", "Höhe", "drehen", Nothing, reducedKompass:=True)
                ' TODO
            '
            Case BasisformEnum.Kegel
                EnDisable("Länge", "Breite", "Höhe", "Steine versetzen", "Richtung", reducedKompass:=True)
                ' TODO
            '
            '==================================================================
            Case BasisformEnum.Zufall
                ' kommt hier nicht vor, da bereits abgefangen und geändert.
        End Select

        If Not IsNothing(wb) Then
            'während der Entwicklung kommt das öfter vor.
            Dim ws As Werkstück = wb.ResultAsWerkstück(demoMode:=True)

            Spielfeld.SFD.WerkbankSpielfeldInfo = New SpielfeldInfo(ws.WerkstückSize, SpielfeldOrEditorMode.Editor)
            Spielfeld.SFD.WerkbankSpielfeldInfo.AddWerkstückToSpielfeld(ws, New Triple(1, 1, 0))

        End If

        INI.Debug_StopRendering = True

    End Sub

#End Region

#Region "Erzeugung"



    Private Function Create() As Werkbank

        Dim wb As Werkbank = Nothing
        Dim x, y, z As Integer

        x = num2UpDnX.Value
        y = num2UpDnY.Value
        z = num2UpDnZ.Value


        Return wb

    End Function

    Private Function CreateLine() As Werkbank


        Dim wb As Werkbank = Nothing
        Dim x, y, z As Integer

        z = num2UpDnZ.Value

        Select Case KompassRoseRichtung.DirectionDiagonal
            Case KompassRoseX.KompassXDiagonalEnum.None
                Return New Werkbank(New Triple(1, 1, 1))

            Case KompassRoseX.KompassXDiagonalEnum.Center
                Return New Werkbank(New Triple(1, 1, 1))

            Case KompassRoseX.KompassXDiagonalEnum.N_S
                x = 1
                y = num2UpDnX.Value
                wb = New Werkbank(New Triple(x, y, z))
                With wb
                    For idxZ As Integer = .zMin To .zMax
                        For idxX As Integer = .xMin To .xMax Step 2
                            For idxY As Integer = .yMin To .yMax Step 2
                                Dim tplQuestion As New Triple(idxX, idxY, idxZ)
                                Dim steinPos3D As Triple = wb.IsValidePlace(tplQuestion)
                                If steinPos3D.Valide = ValidePlaceEnum.Yes Then
                                    wb.AddSteinToSpielfeld(CType(_rnd.Next(1, 43), SteinIndexEnum), steinPos3D)
                                End If
                            Next
                        Next
                    Next
                End With

            Case KompassRoseX.KompassXDiagonalEnum.W_O
                x = num2UpDnX.Value
                y = 1
                wb = New Werkbank(New Triple(x, y, z))
                With wb
                    For idxZ As Integer = .zMin To .zMax
                        For idxX As Integer = .xMin To .xMax Step 2
                            For idxY As Integer = .yMin To .yMax Step 2
                                Dim tplQuestion As New Triple(idxX, idxY, idxZ)
                                Dim steinPos3D As Triple = wb.IsValidePlace(tplQuestion)
                                If steinPos3D.Valide = ValidePlaceEnum.Yes Then
                                    wb.AddSteinToSpielfeld(CType(_rnd.Next(1, 43), SteinIndexEnum), steinPos3D)
                                End If
                            Next
                        Next
                    Next
                End With

            Case KompassRoseX.KompassXDiagonalEnum.NW_SO
                x = num2UpDnX.Value
                y = num2UpDnX.Value
                wb = New Werkbank(New Triple(x, y, z))
                With wb
                    For idxZ As Integer = .zMin To .zMax
                        For idxX As Integer = .xMin To .xMax Step 2
                            Dim tplQuestion As New Triple(idxX, idxX, idxZ)
                            Dim steinPos3D As Triple = wb.IsValidePlace(tplQuestion)
                            If steinPos3D.Valide = ValidePlaceEnum.Yes Then
                                wb.AddSteinToSpielfeld(CType(_rnd.Next(1, 43), SteinIndexEnum), steinPos3D)
                            End If
                        Next
                    Next
                End With
            Case KompassRoseX.KompassXDiagonalEnum.NO_SW
                x = num2UpDnX.Value
                y = num2UpDnX.Value
                wb = New Werkbank(New Triple(x, y, z))
                With wb
                    For idxZ As Integer = .zMin To .zMax
                        For idxX As Integer = .xMin To .xMax Step 2
                            Dim tplQuestion As New Triple(idxX, .xMax - idxX, idxZ)
                            Dim steinPos3D As Triple = wb.IsValidePlace(tplQuestion)
                            If steinPos3D.Valide = ValidePlaceEnum.Yes Then
                                wb.AddSteinToSpielfeld(CType(_rnd.Next(1, 43), SteinIndexEnum), steinPos3D)
                            End If
                        Next
                    Next
                End With
        End Select


        Return wb

    End Function

    Private Function CreateRechteck() As Werkbank


        Dim wb As Werkbank = Nothing
        Dim x, y, z As Integer

        z = num2UpDnZ.Value

        Select Case KompassRoseRichtung.DirectionDiagonal
            Case KompassRoseX.KompassXDiagonalEnum.None
                Return New Werkbank(New Triple(1, 1, 1))

            Case KompassRoseX.KompassXDiagonalEnum.Center
                Return New Werkbank(New Triple(1, 1, 1))

            Case KompassRoseX.KompassXDiagonalEnum.N_S
                x = num2UpDnX.Value
                y = num2UpDnY.Value
                wb = New Werkbank(New Triple(x, y, z))
                With wb
                    For idxZ As Integer = .zMin To .zMax
                        For idxX As Integer = .xMin To .xMax Step 2
                            For idxY As Integer = .yMin To .yMax Step 2
                                Dim tplQuestion As New Triple(idxX, idxY, idxZ)
                                Dim steinPos3D As Triple = wb.IsValidePlace(tplQuestion)
                                If steinPos3D.Valide = ValidePlaceEnum.Yes Then
                                    wb.AddSteinToSpielfeld(CType(_rnd.Next(1, 43), SteinIndexEnum), steinPos3D)
                                End If
                            Next
                        Next
                    Next
                End With

            Case KompassRoseX.KompassXDiagonalEnum.W_O
                x = num2UpDnX.Value
                y = num2UpDnY.Value
                wb = New Werkbank(New Triple(x, y, z))
                With wb
                    For idxZ As Integer = .zMin To .zMax
                        For idxX As Integer = .xMin To .xMax Step 2
                            For idxY As Integer = .yMin To .yMax Step 2
                                Dim tplQuestion As New Triple(idxX, idxY, idxZ)
                                Dim steinPos3D As Triple = wb.IsValidePlace(tplQuestion)
                                If steinPos3D.Valide = ValidePlaceEnum.Yes Then
                                    wb.AddSteinToSpielfeld(CType(_rnd.Next(1, 43), SteinIndexEnum), steinPos3D)
                                End If
                            Next
                        Next
                    Next
                End With

            Case KompassRoseX.KompassXDiagonalEnum.NW_SO
                x = num2UpDnX.Value
                y = num2UpDnY.Value
                wb = New Werkbank(New Triple(x, y, z))
                With wb
                    For idxZ As Integer = .zMin To .zMax
                        For idxX As Integer = .xMin To .xMax Step 2
                            Dim tplQuestion As New Triple(idxX, idxX, idxZ)
                            Dim steinPos3D As Triple = wb.IsValidePlace(tplQuestion)
                            If steinPos3D.Valide = ValidePlaceEnum.Yes Then
                                wb.AddSteinToSpielfeld(CType(_rnd.Next(1, 43), SteinIndexEnum), steinPos3D)
                            End If
                        Next
                    Next
                End With
            Case KompassRoseX.KompassXDiagonalEnum.NO_SW
                x = num2UpDnX.Value
                y = num2UpDnY.Value
                wb = New Werkbank(New Triple(x, y, z))
                With wb
                    For idxZ As Integer = .zMin To .zMax
                        For idxX As Integer = .xMin To .xMax Step 2
                            Dim tplQuestion As New Triple(idxX, .xMax - idxX, idxZ)
                            Dim steinPos3D As Triple = wb.IsValidePlace(tplQuestion)
                            If steinPos3D.Valide = ValidePlaceEnum.Yes Then
                                wb.AddSteinToSpielfeld(CType(_rnd.Next(1, 43), SteinIndexEnum), steinPos3D)
                            End If
                        Next
                    Next
                End With
        End Select


        Return wb

    End Function

    Private Function CreatePyramide() As Werkbank

        Dim wb As Werkbank = Nothing
        Dim x, y, z As Integer

        x = num2UpDnX.Value
        y = num2UpDnY.Value
        z = num2UpDnZ.Value


        Return wb

    End Function

#End Region



#Region "Helper"

    Private Sub EnDisable(x As String, y As String, z As String, zfkt As String, r As String, reducedKompass As Boolean)

        gbxX.Text = x
        gbxY.Text = y
        gbxZ.Text = z
        chkZusatzfunktion.Text = zfkt
        gbxRichtung.Text = r

        gbxX.Visible = Not String.IsNullOrEmpty(x)
        gbxY.Visible = Not String.IsNullOrEmpty(y)
        gbxZ.Visible = Not String.IsNullOrEmpty(z)
        gbxZusatzfunktion.Visible = Not String.IsNullOrEmpty(zfkt)
        gbxRichtung.Visible = Not String.IsNullOrEmpty(r)

        If Not gbxX.Visible Then _xAkt = 0
        If Not gbxY.Visible Then _yAkt = 0
        If Not gbxZ.Visible Then _zAkt = 0
        If Not gbxZusatzfunktion.Visible Then chkZusatzfunktion.Checked = False



        lblMsg.Text = Nothing

        KompassRoseRichtung.SetVisibleXDirectionsXXX(Not reducedKompass)

    End Sub



#End Region

End Class
