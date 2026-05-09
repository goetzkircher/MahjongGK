Imports System.Drawing
Imports MahjongGK.Contracts

Friend Module ResLichtkarten

    Public Function Image_Lichtkarte(lichtkarte As LightMap, request As TileRequest) As Bitmap

        Select Case lichtkarte
            Case LightMap.Diagonal
                Return Image_LichtkarteDiagonal(request)

            Case LightMap.Zentral
                Return Image_LichtkarteZentral(request)

            Case LightMap.RahmenXS
                Return Image_LichtkarteRahmenXS(request)

            Case LightMap.RahmenS
                Return Image_LichtkarteRahmenS(request)

            Case LightMap.RahmenM
                Return Image_LichtkarteRahmenM(request)

            Case LightMap.RahmenL
                Return Image_LichtkarteRahmenL(request)

            Case LightMap.RahmenXL
                Return Image_LichtkarteRahmenXL(request)

            Case Else
                Throw New Exception("Programmierfehhler: ungültige Enum LightMap")
        End Select

    End Function

    Public Sub DisposeAllLichtkarten()

        DisposeLichtkartenDiagonal()
        DisposeLichtkartenZentral()
        DisposeLichtkarten_RahmenXS()
        DisposeLichtkarten_RahmenS()
        DisposeLichtkarten_RahmenM()
        DisposeLichtkarten_RahmenL()
        DisposeLichtkarten_RahmenXL()
    End Sub

End Module
