Imports System.Drawing
Imports System.IO
Imports MahjongGK.Contracts.GlobalEnum

Friend Module ResLichtkarteZentral

    Private _imageLichtkarteOriginal As Bitmap = Nothing

    Private _imageLichtkarteSpiel As Bitmap = Nothing
    Private _imageLichtkarteSpielSize As Size = Size.Empty

    Private _imageLichtkarteEdit As Bitmap = Nothing
    Private _imageLichtkarteEditSize As Size = Size.Empty

    '
    ''' <summary>
    ''' Liefert die auf die aktuelle Steingröße heruntergerechnete LightMap.
    ''' Besitzer der zurückgegebenen Bitmap ist das Ressourcenmodul.
    ''' Die Bitmap darf von außen nicht disposed werden.
    ''' </summary>
    Public Function Image_LichtkarteZentral(request As TileRequest) As Bitmap

        Select Case request.AktRenderMode
            Case AktRenderMode.Spiel
                Dim targetSize As Size = TileFactoryAPI.GetAktLayout(AktRenderMode.Spiel).FaceOuterRect.Size
                If _imageLichtkarteSpiel Is Nothing OrElse _imageLichtkarteSpielSize <> targetSize Then
                    ReplaceScaledCache(_imageLichtkarteSpiel, _imageLichtkarteSpielSize, targetSize)
                    _imageLichtkarteSpiel = CreateScaledLightMap(targetSize)
                End If

                Return _imageLichtkarteSpiel

            Case AktRenderMode.Edit
                Dim targetSize As Size = TileFactoryAPI.GetAktLayout(AktRenderMode.Edit).FaceOuterRect.Size
                If _imageLichtkarteEdit Is Nothing OrElse _imageLichtkarteEditSize <> targetSize Then
                    ReplaceScaledCache(_imageLichtkarteEdit, _imageLichtkarteEditSize, targetSize)
                    _imageLichtkarteEdit = CreateScaledLightMap(targetSize)
                End If

                Return _imageLichtkarteEdit

            Case AktRenderMode.None
                Dim layout As TileLayout = TileFactoryAPI.GetLayout(request.SteinSize, request.SteinBasisSize)

                Return CreateScaledLightMap(layout.FaceOuterRect.Size)

            Case Else

                Return Nothing 'wird nicht erreicht
        End Select

    End Function

    '
    ''' <summary>
    ''' Räumt alle vom Modul gehaltenen Bitmaps auf.
    ''' </summary>
    Public Sub DisposeLichtkartenZentral()

        DisposeBitmap(_imageLichtkarteSpiel)
        _imageLichtkarteSpielSize = Size.Empty

        DisposeBitmap(_imageLichtkarteEdit)
        _imageLichtkarteEditSize = Size.Empty

        DisposeBitmap(_imageLichtkarteOriginal)

    End Sub

    Private Sub ReplaceScaledCache(ByRef cacheBitmap As Bitmap,
                                   ByRef cacheSize As Size,
                                   newSize As Size)

        DisposeBitmap(cacheBitmap)
        cacheSize = newSize

    End Sub

    Private Function CreateScaledLightMap(targetSize As Size) As Bitmap

        Dim source As Bitmap = GetOriginalLightMap()

        If source.Width = targetSize.Width AndAlso source.Height = targetSize.Height Then
            Return New Bitmap(source)
        End If

        Return ScaleBitmapTwoStage(source, targetSize)

    End Function

    Private Function GetOriginalLightMap() As Bitmap

        If _imageLichtkarteOriginal Is Nothing Then
            _imageLichtkarteOriginal = LoadImageLichtkarte()
        End If

        Return _imageLichtkarteOriginal

    End Function

    Private Function LoadImageLichtkarte() As Bitmap

        Dim base64 As String = _
            "iVBORw0KGgoAAAANSUhEUgAAAZAAAAH0CAYAAAAT2nuAAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsIA" &
            "AA7CARUoSoAAAAAZdEVYdFNvZnR3YXJlAFBhaW50Lk5FVCA1LjEuMTITAUd0AAAAuGVYSWZJSSoACAAAAAUAGgEFAAEAAABKAAAA" &
            "GwEFAAEAAABSAAAAKAEDAAEAAAACAAAAMQECABEAAABaAAAAaYcEAAEAAABsAAAAAAAAAPJ2AQDoAwAA8nYBAOgDAABQYWludC5O" &
            "RVQgNS4xLjEyAAADAACQBwAEAAAAMDIzMAGgAwABAAAAAQAAAAWgBAABAAAAlgAAAAAAAAACAAEAAgAEAAAAUjk4AAIABwAEAAAA" &
            "MDEwMAAAAACDfy8cctDT3wAA/rxJREFUeF7U/cuvbul25gm981t7x47biYgT4WO7bBelLApnAiZLhqQSpRMkxE1QDUqiUYCEBKJR" &
            "1eS/KYk2Eh0aFK1CAtFCNAqqUpVp0nlxUs6r0z4+vsU5Jy57rY/GWc/yb//iGe/81o5wFjzSqznfMZ7xjPFe5pzfba99rP/fwt1f" &
            "+At/4Z2f//mf/95xHN978eLFB5fL5YMXL168//Dw8NHLly8/uVwuH6+1PllrvXscx3trrffWWq+u1+vL6/X63nEcH97d3b13d3f3" &
            "7nEcr9ZaL4/juFtrrfv7++v1el3X6/Wy1rp7tL+4XC7hHI91XK/X6/Xh4eG61rocx5G2rtfr9fgZ1uPxZ521Hg/r+qhzvV6vx/qZ" &
            "47rWOi6XyzqO43h4eFiP2vGth4eH5HvSvVwuTxNzvV6Py+Vyfcz/cLlckHpdHx4e1lprxX5/f78ebdfoXK/XyGWcC+NY1+v1qa71" &
            "M60V+2OedRzHulwuGcO6u7t7yvkYvx7neD3mPR614r+mDtT/NEeP/eOJBDxqk0dcH36G9fDwsC5/NkHXxFwulzfGfblc1t3d3VPd" &
            "qX1lAzyO48BagHt9XJOnOUqf8yWt6/V6PeDP/njiZn5gO1JH1jl+NiN1Ph6j+UR8XLf18PCwXr9+nb33Rj2PY7tmH19/VsQbyTBf" &
            "TzU+ahyZF/Ob7THuyHgyZ8dxXDlX4T/WsjI/Dw8P6ziOdXd3R84bMa9fv37Kn/V8vO4yjxn/kb32qH39s+Gv6+NevV6v19drrYfH" &
            "2h/WWg/X6/VnN5mfnV+v1+vrh4eHr4/j+Ppyubxea72+XC5fHcfx9fV6/er+/v7Lh4eHr47j+Mla64+v1+sPr9frPz+O448eHh4+" &
            "P47ji+v1+vlxHD96/fr1569fv/7pP/2n//Snn3/++ZdrrdcZ339W+ObO+xeDy1rrnV//9V9/9emnn37w4sWLD+/u7r53uVx+8M47" &
            "7/zC/f39r661/sLd3d3H1+v147XW+y9evPgAD4t3juN4cRzHi+v1enlc2OPh4eG4XC7H5XI57u7ujvV448pFzZsEL7p2EXrzH483" &
            "nPDu7+/fsBPaxN/Qjy7t1JvAm1NqSZ3UXH92s30jB2/u5lPH9cZP7nocR7Otx1ysMfp4sL3hczxBm/mMu+KmOWnSTnhe2j4hb+EB" &
            "exzHG2MipvqiwfHE/vr162/oBLQ37cDjMf/AHuJ1kea6WF+02Gd84lodbV7jSz18KE/zYF2e22/7wrynHl7DTZf91Nb4nBfMyZXz" &
            "eRzH9eHhYd3f318fH+7X4zgejuN4vdb6aq31xXEcX6y1fnq9Xn+y1vr8/v7+T67X64/WWv/4OI5/eL1e/9la6/e+/PLLH/7pn/7p" &
            "H3z++ec//eEPf/j1enyA/4tCX50/H7z8S3/pL33vX/6X/+Wf++CDD37pxYsXf+Fyufzi3d3dz7948eJfeuedd35hrfWLL168+N79" &
            "/f2Hx3G8Oo7jxcPDw4tMPBcmyI0q59lw3HhtM1PT54m5v79/svFmQdCfuMvjKztuqnDTp+b18cZqhB8wLvm0Ud/QXI8X5eXxwZPm" &
            "m0V0XBdztnFTL1zmuru7eyP++jjO169/9sKJPubIcVeja4p+uAFjEx9kHl6+fPlGnPNyvwXJF72JwzqjOd1sw+Fx4cZKDfN49N5L" &
            "rY4/sJcSd39//8a+T17WGHDOHx4fQrEzLxt55nJf70C99Cd/cq4yBtbH66np0m5f5jtjWI8vCDlOX4ecB+6H9c05uT6+q7l/fMfx" &
            "1Vrri+v1+vnDw8M/fXh4+O2vv/76t1+/fv3/uV6vf+fLL7/8Z3/37/7dP7y/v//iKfmfI765K75bvPiN3/iN733/+9//Vz777LP/" &
            "/MPDw796d3f3X3z33Xf/lRcvXvzScRzfv7u7++Byubx655137tZaRz5SyOLy1VgmPpt84VV5m3y3aCQutraxyMVivmFvnPBc08ID" &
            "LpspfG4qg7kWPno48MoxG3A9jivvgI7Ht/Rff/31k5Y3a97WJ9a1sQXRYQsyRxkn48NNPeGFux7Hl1ysl9xWU5A5pgZ9CzeO3CiT" &
            "m/B6UIf6iWWdmd+Mn/rxUY+g/6qb/OXxhYnXnLH2Ja5pL+yn9XjTe/369fZdYuI4v8zLnKkjMeH4AXV53LOZL+4B5kj+5PNYyA2H" &
            "Nl4XBrWpRx/n1bU8PH6cmxx8oZT5SEzmN3OROU+NmbugrGnewTw8PDx8dX9///nXX3/9e/f393/vcrn8va+//vo3Hx4e/tZXX331" &
            "D3/zN3/zj9Zaf3bBfcf45kx+B/jlX/7l9//iX/yLv/zJJ5/8pQ8//PBff++99/5r77333q9eLpefe+eddz56+fLlO5fL5fLy5cv1" &
            "8uXLtbBR7+7unibq66+/Xl999dU69G4gG+A4jvXixYv18uXLp1i2LNyLFy+eFpCbvG0kbh6eJ282QY4GN5ZxxU3Fubkx44uNetyI" &
            "vkCP8nC6XC5PD9um6YuC8+KaPLaphqUbV+zXx4uH9cTHcbcxhseYVdYr/dyEGDvVHh/1w+F+jM11HPgoJLr0cTxLD5sJrNUavqGw" &
            "HttSQ25UbSxZp+j6AeI9QQ2O2zW3ubviBhrtrFVryWENttSdvK4l59HzvjxwX2BMwDxE4pg7Y84+T948WNbjw4WamQ/aE//69ev1" &
            "9ddfP90Lw2UL7/7+/uE4jp++fv36977++uu/8/Dw8Deu1+t/9Pr169/64osv/uHf/bt/90/Wd/wR1zfvoN8Or/76X//r/7lf/MVf" &
            "/G9+9NFHv/Hy5ctfe//99/+VDz/88JO7u7tXeWC8884763i8+b948WJd9Wrt9evX6/Xr1+urr75ar1+/Xi9evFivXr1ad3d369Wr" &
            "V+vdd999WhBu7mwMb8ALHiK8eWWzXfQRD/25QKyZC4AIlzXFvnAxBi0fc3CTsYbYUgN1L3irvB5vpNGxdmB/8oXDOI85POotxaR/" &
            "xav+pZsd+Zy/zB3hXMnPecoLCPI5dtbcxpo+b4KOSR7OWS7opf1IhBdf2nPGavvC/sr8ZRzJl1e6sadmgrVFk/XHxxo4r3z45Bql" &
            "Bucp9Xp+ppjYY1uol7pBGxvnZhW98Dg+5mAM5zf92DgPUwx1nf/hcb2yZq9fv15ffvnl0z3x/v5+ffnll+unP/3p+vLLL5/23P39" &
            "/bq7u1v39/f3X3/99Z8+PDz8zldfffX/fnh4+L/f3d39P47j+O3/8D/8D//4Kdm3xDdX7vl48Ru/8Rvff//99/9Ln3zyyX/zgw8+" &
            "+DdevXr1X/7www9/8f3333/v1atXx6tXr1YeHrmZ393dPR1X2ZDhhfPq1asney5aL2xbeOpx4wTh+HyVDRLbwmbMORtvHC2Wvmx8" &
            "XkjciB5PtHnR8KYRP/O4H9sDHoLOwz5jfCQ3NyffFDgHjCGcIxrhehyslbbsBd9EDdZtP28UHAv1o+s6J03COsljX/ocO31G6nnx" &
            "4sVTDPcJz92oYdvCw5AaS+vmHIlra+d1oi+46IWda6LtAa/gG4f1ek1jd470c3NO3+NJn5pXXNvMEVDbeyr++8d3IeG8fv36aZz3" &
            "9/frq6++Wj/5yU+eHiIP+HiY/NevX3+x1vq9169f//3L5fIfXa/X/9tPfvKTv/XP/tk/+90/+IM/+OqpqLfAmzPxPNz9lb/yVz7+" &
            "7LPP/vWPPvror3/wwQf/7Q8++OC/9OrVq0/ff//9l++///567733nh4AL1++XHePHy3lfGEDXq/XlQfNu++++/SOw4uRDboen7bc" &
            "NG2xsuDTxnEc4QUlopvY5Fja+C2WY46PNxA+VLm5eIyPdo7T3ORhnaw9iI21txxLrwxZq28K8a9HjfCCaN7d3T2NifHOH0QzyMUV" &
            "uIal8XistAVtzOSGk37qt45rXeVBRSTe85GYpsdaU0O4vGY4btcZ0H7Fg9L5HZvrM/HZywvzFE5sXo9Wv2uJDsdAXSK8pXwtB3nR" &
            "pJ+IDvuJY01rM0+cr9ivj3PAhwfvdTzm4y08LNb94zuWtHAeHh7u11p/8tVXX/3Ww8PD/+Ply5f/wR/+4R/+x7/927/9o/WznyE/" &
            "G9+8wm7Ar/zKr7z/a7/2a7/62Wef/Y9evnz5P3jvvff+0ve+973vv/feey9fvny5Pv744/Xq1as33jUceLufDZB3FnlYvP/++08P" &
            "mfW42TM5mSBPNjdMFowLeIfvQrh5CGouLDZv6uSSl/7d4zuqcFhD4q96xZp6MjeJ482EuQ98VJI+xxPNC75Ib3V4Dtx3/ayTOem/" &
            "lldTmfdwg5yznqz7Q/lo0DkDnofjPWG0Obhq39jHB2L0mw41eJOLjsG5CpgnddHOuXMNl/JRbNbBY7QWEQ5roF6rN9jNT+rgPgmy" &
            "dlfsN89dfNxT5LRGX8DzgDUv1bNKDZxXxwWcb89lYjJfD3pILLxAzgOBc0utzGds1Hr9+FXA4wNkrbXWF1988frh4eGPX7x48Te/" &
            "/PLL//NXX331f11r/e3f/u3f/vyp+Bvx5uqf4/irf/WvfvpLv/RL//1PPvnkf/ruu+/+11+9evXzH3zwwd0HH3yw3n///fXOO++s" &
            "9957b7333nvrnXfeWUs33fv7+/XixYv1zjvvrA8//HB9+OGH65133nljgR4en6R8smaSOIGZpFU2cltYPkSuuFFQkzF5BxQeF40x" &
            "x3E8fTx3ffzlWMbsutjPDdZ1pMa0IHkZn7Ekfq319K7vGL7cXbgRRN815Ghu4+c8D/rr4w00D1XWyCP1U3PGHz/nww/FwLXb38Ac" &
            "6zH2Hh/BBcnPm1/2YtYv8czfXkxQM33PZ2Ja3FG+n1r6uC7jiD81Zu2ZNzmpl3Pv4eTlmK+P+yRa6VsvSFz2iP2Jpz61Fq7jcGOz" &
            "Rny25zxH52ha3CvX8j0Kc3lOU0va7mbPvWV+fK45LT5rx/fweE9dj/fgr776aj3+w9s/fPfdd/+jly9f/u//+T//5/+Xf/bP/tk/" &
            "Xs/4ov3PZuscd3/9r//1f+373//+//CTTz75tz/99NO//PHHH7/3/vvvH+++++56+fLl08Pg7u5uvfvuu+vFixdPhT88LsTLly/X" &
            "9773vfXRRx+t9957b909/sw0Xw6FnwcIvzRqk5YY2xLzUBbXyGQH5LULjYsTTt7pZLEnrRxzIVA7Y4yfuY1cyKlpYSPn1fwxvIqy" &
            "RjCNLfB5+LwxZAz50UKOzp26M97MH+eD65L5YmwDx5pzjzP+1Jp1e9AXu6mN65D1vb+/r3OXnJz/W+aT4wmfNs4LW/M7X8ZiW2Ct" &
            "K246bFesVeKpk1jmX3iRwX3y9ddfv8EJYvMYW+3ZG15b1zvlYa2eDx6zfz2utiYBz7meV+y78LKfmkbWgfnbesYXvjVZO9f28f75" &
            "xYsXL37r3Xff/T9er9f/w2/+5m/+nVv/lXu/O30TL//aX/tr//qnn376v/zkk0/+zU8++eRXfvCDH7z43ve+t169erXee++9p4+i" &
            "LpfL00dSx+PneOtxEd5555312Wefrffff3+9fPly3T/+kiC/Ivjqq6+eWm5GeRfy9ddfP/Vjy1uzn/70p+urr75a18dXTmmcyFU2" &
            "Bic2Nt/E2gXChYs/cd5k5IaXm2U2PjdIi4mNGollPYnhDbBppR/u0kXgOOobV30+zjpZy4Rom5f58HyucoPNWKZ3KJnz6Ec7F1Li" &
            "UzvroHZuJLw4iazf0rtDtvhSa+JcM+fd9aR+z810fujjIOp6DLFzLTmO+FNTy+mxeF+0Whri83WxMCdXfFx4HeYlyJp4XVgf7esx" &
            "T94ZkxtkrmgP99C7WY4/fvIXarzgRU04OW91xp/GBw9zsNbs43BfvXr1T1+9evV/ur+//9/9wR/8wf/r93//90//MeJ8Zf8ZXvzG" &
            "b/zGX/nss8/+1x9++OG/9XM/93Offf/73z8++eSTp4+pPvjgg6cvwI/HjyMyyLXWevfdd9dHH320Pvjgg/XOO+88PQi+/PLLN95l" &
            "/PSnP32j/+WXX64//dM/XZ9//vn6oz/6ozdi0nIhZ3IfHl8R8xVt6gi8eFwo3sjIoy3H3JTCy6ITzH3gZuRXqPFnodOPRmwXPTwY" &
            "Yx91Oa7Y0piDY2ZejoPxyRtOarh//DjIOoyln3ocUzb7FfOdC4SavDmscjO/6PuBzPsDXh1nHEQ06X/AxwFEal9Yu9h5XKopYyQY" &
            "T1Ar7RZwHs9ATmKyJtxTmRfyHJv1yjwHvMFFmzG2cY7sZ22eS/JSq7UX5ofnGZ/XKS1ajqc/HI6dGraHe+jeQO34mOOqj7Fca1o4" &
            "OV+Ptede+vLly99///33/4Pr9frv/dZv/dZ/ePZOpO/SR3zwwQevfv3Xf/3f+Pmf//l/9+OPP/7vfvjhhz///e9//+njp/fff//p" &
            "4ZGf3F5xQ3nx4sX64IMP1scff7zee++9dblcnt5h5J1H3kn85Cc/Wff39+uLL75Yf/qnf7r++I//eP3oRz9af/AHf7B+/OMfP73D" &
            "8CSvYRPkJk0/FyuIL0cjMV7oq25orGmX78DNPNyMaeFiCZca7hOZd+pQOxdC/NQilz7aWSMRbvjJlXOOh/qsJT7X5htCq8k1OKft" &
            "ieEFtsoDiPbAdRkcD8+jlWMQDucndboO1j2BOQ3OyRrqa/3EtTk0z0gcY9YQx2PL5dpZf9bSc9NqmsA1SEsOrgUbwfweE+u1Bsca" &
            "f2zc+zyntjWzN22zn5r86Pnh4WG9fPnyRy9fvvz3X7x48b/9oz/6o7/xu7/7u+M7kXHH/eAHP3jnV3/1V/8bv/ALv/Dvfvrpp/+j" &
            "jz/++OMPPvhgfe9733v6ruP9999f77///ro8fjx1wSvwV69erY8++mh9/PHH6+XLl+vh8VXjF198sX76058+PTh++tOfri+++GJ9" &
            "8cUX6w/+4A/WH/7hH67f//3fX3/yJ3+yvvjii6dXKgEnLv3YeDzKr74WbkI5PpSfjdrGeObzqxPmsmZycsM0Do9LOpzf+ILo0U49" &
            "P0CWxhUecZR3SInJPMRm7YDzZVCLiOZV3ws1bvg8v+KiYW2cD59z7toctT04rUX6tLW6XYPt7O84rD3HpsEja8vepb3ltJ97w5wp" &
            "frLT57VqyPyyDo4tWty71HKfcG7Xw/qpkXzW9T5goy/gHC+92LH2tXynkqPj+O4j/jS+G3n9+vV65513/uDdd9/997/88st/73d+" &
            "53f+n28kBf7sg1jh137t137tBz/4wb/zcz/3c//mZ5999v2PP/54ffDBB+uDDz5Y77777tMvrV48/imRfE54HMd69erV+v73v78+" &
            "/fTT9erVq6d3HPf39+snP/nJev369frJT36yfvzjH68vvvhi/ehHP1q/8zu/s37nd35n/dN/+k/XD3/4w/X111+/cXPOdxoedFr6" &
            "3Eyc7DbBBCfW8CJn83qhm4/IeFwDbY5Ln7ls87nBmhzLo89dFzlBbs45MnZhbNZuWkHsZ7FTo86h2mhvvDYe8gKvuZEHT8D6rBtw" &
            "zprfseyzdvpX2TP2B5O+fYzzXo4/43eOhXrcGn/K7xq4V2kLrGMu9VzDBI5jlRoN17RKrl0drnPd+I70LK/n6fEe+t5a61969erV" &
            "ww9+8IO/9cMf/vDHTySgPkD+2l/7a//q9773vf/NJ5988m999tlnn3300UdHPqrKwyO/vHrx+GdGro9vhV6+fLk+/fTT9emnn64X" &
            "L148fUyVL7vz8MjxH/2jf7T+/t//++sf/sN/uP74j/943T/+U/yFv4WVB0PbKJ5AT8ZRbm5sntiAse5bg/70CW60Fh9bYK2WY2oE" &
            "b4b5TojnmWfX5Jy+MRH0p782N5aj/OKq5XY+2tx2N6umRVjL3Jz72GKneTLPHO9BrluL4TFryjmwfkDb5PcYeGQMa27XnPUda/8O" &
            "rsGxbY8FtLnFHw3HeQ83uBbrBu3e5ZyrzDPraP6r3mmY3/I6J8cZ38PDw3G9Xr93HMcvXi6XP/z+97//2z/60Y9++kZge4B8+umn" &
            "P/iVX/mV/9nHH3/8b3/66ae/9P3vf//y4YcfPr3j8MMjX5zn4fHZZ5+tTz/9dL18+XJ99dVXTx9D5eOqHH/0ox+tf/AP/sH6O3/n" &
            "76zf+73fW18//hP8fLT1NX7m56MnMX3aj/KqzI3x1lr60u2shW+bdc3hzWLKR1vOrTP5zWs3iMnnvERsiaGtnVPfNWXjUsearoHx" &
            "jiGngRdT0OJj57nrYR27mnLO+Vqllvi8FoxxrjTO41ks+45rXJ6369HXpnUI3qgmtNxnMYbjWj9H1r/KugTXk5txbGc6q6xFbIZr" &
            "S1xq2dVjDv1Tjdl7j5/ufHi9Xt97+fLl73z55Zf/8KuvvnrjFfsbD5Dvf//7H/7lv/yX/ycffPDB/+qjjz761U8++eTF9773vTce" &
            "HPmp7oFfW71+/XpdLpf12WefrR/84Afr5cuXTx9ZXa/Xp4dG3oX88Ic/XH/7b//t9Zu/+Zvrxz/+8Xrx4sX6+vFfSvLjquPxglsa" &
            "ZCbQkxXQ75tVgydw4k0aO3uOF71SJMfxu9rZd0zzW8eaOy3qrXLxWItIv+WlnxvbGsSBG136rVnP+RrM2cXZ7jyNd8axjXbXsuvH" &
            "RrgfOC42Hhvia9cj/U3jGNbHMYydzgnX5OuZsIb1z8ZFONYc97N/b9H1GMjLPcSxTTPXrK8d+mPj9Y3+3cPDww9evHjxzi/90i/9" &
            "v3/3d3/3h0/BfoD8+q//+r/x7rvv/jvvvPPOf/3jjz9+lZ/e5mOrd9555+lfDB+PN4UU9umnn66f+7mfe+Nnuq8f/z3GT3/606cv" &
            "y//RP/pH6z/5T/6T9U/+yT95elh89fgn27/88ss3NkHO7/GT0OQ9yoXEm3TjhdvAicvR59Zqvl1LPS2em+LWxrFGt9mnGMObdmke" &
            "aGv6zTY1auXoc84J0bhNd9eCVrM12Ccc15rjrEnE3sbtdfMxHOc2x/2mM/WtPR2Ntq+MVvOUy9ymH84F96imYbv9mXejzbWPhvVp" &
            "W3jANLQxrqLZ4m1jP+d+cBDX6/XVWuvTu7u7f/7ZZ5/95g9/+MOnP8D49AD59V//9X/pww8//J+///77/8NPPvnkk48++uj48MMP" &
            "v/EAuXv8Y4gs4sMPP1y/8Au/sN5///318Pib4uv1ur788sv14x//eD08PKzPP/98/eN//I/X3/gbf2P93u/93nqNf1bPdxz5PG89" &
            "/hSXF1Nra/jM+JYLjhrm7Oz0s8/NZq4btdnnZnfbbdqlTc05CVrMGl65OXdszLGGuW/tVv+UmxvcrcEcN3Jyzth2NI4yH7Hl3I2x" &
            "PDZ+bNlXXMuGfKdFTcM+37S8BwzXFxuPQa5pw+OknrVj43kaa/X+WOVjQPedO4jNfHO4zjzynHUZzbZk9xh9c/dYdmicaKaFw/76" &
            "2X36w8vlcvfy5cu/+/u///v/ZD3+uZPsyHe+973v/dX33nvvv/Pee+/9/MuXL4933nnn6YHhP0cR8YfH/4UrH3PF9vrxHwHmXchP" &
            "f/rT9Tu/8zvrP/6P/+P1x3/8x+v6+JOyL7744ulh0yYnH2n55sMabF+aqBbjPNSgDnnU4DE1ps8bocHcZ3XZZk36D30eesXPFxPL" &
            "+PiseZQbOX1tjhgz+dka99DbZ46B42/agefKjX7H2pY5dT7ON2/mzkWEm3G3OvmAoG44BLXcOL5V6jI3Ywk3NrdVbjQtF2Eez10L" &
            "a2ga5BBNg76W81p+3sqxND3WcJQHC8d4BurucpDDPM7BuLZ3psb4aHBcvA887rm7r7/++q/c39//j3/1V3/1lxN/t372k91f+eCD" &
            "D/4Xr169+m+99957H77//vsr7z7yZ0ryU93jcRHuHv9y6scff7x+4Rd+4enVz5eP/+nJT37yk6cv0H/rt35r/c2/+TfXn/zJnzw9" &
            "YPKuI4sZ3SCD8KCmi4ct8Tmn5i0+5yT3QJ3Om+bN4HhvQPLaGNO3PY35iPDtz7GNg33DHNdDjlt85hjeA463rXHd4m/z6Nj0qWs0" &
            "zWBXP+2B14aIvc1zG0OLdd+t+Yij7PfANzOjaeaGFf/ZsdWzNjrmk8fzpTm74LtWa1vzKGtiXs6n/THVMeHWufb5Dle8gDjD4336" &
            "vZcvX354d3f3t7/88svf/uqrrx4ua627jz766C9eLpe/drlcPnn33Xff+NMkfGjwhn//+Fd1P/nkk/XBBx+s9fiz23z/kX/78Q/+" &
            "wT9Yf+tv/a31ox/9aH2NP0XyGv/6MQUSHpgXis1otmA30Vzso9wcGlzLblFoD585Lnh4tA3KuPS90clrm44c9lubOKmTtbdchi/g" &
            "dbJWBPO6Rtc31W9Esx134JwHjml10L70TpDj4vzS3rjWn2Bdxt+qEbCWFktd55pATaPtmyWu81iH72YXYqPt65Z+2rg/nKv1DXNo" &
            "n+BrfDcfrU1ovJy3fffw8HDc39//6osXL/57v/Irv/KDtda6/PIv//IH1+v1Lx/H8SsvX758kY+t0i54MucBsh4fFvkzJRnQ68d/" &
            "7/GTn/xkff311+uP/uiP1m/91m+t3/u931v3j384MQ8ZfzxBeGCpYb3FE5yT7QkJbH/bRtg38QLazWmbZQ1zsUr828J1H2VjGa7l" &
            "Vkx6zm/sbhyTzYiGj+E3newr70PeoIlJp8E6bN7L1GNNri1wDDUnTrPx6PNjeCCaa7R8gcfknM3mFuSc4472Ue43jcdz9wnnd9/c" &
            "CS2euRy7q8X9tt8bjuNYX3zxxXs//elP/6tffPHFL6+11uUHP/jBz7/zzjv/1XfeeefD/C+Cr169evrFVXuA5FdRH3/88XrnnXee" &
            "foKbj6bu7+/X559/vv7e3/t76x//43+8jsdfWMWXh8c0QLfk50LRZh3zjDZBzrnjtAu7tbfFNFb6/I5tPdbn2la5yYb7Ns2xRrMZ" &
            "u/jA+Va5KFyb2wTznOvATY82ny/VZA77PAZ+0DDOGsRuX5s/6bQcttHeYs/2VebQPN6gV7mOWw0NjeNz1xK/5zD5myY5POd9zL41" &
            "1OdxTmAcj7Yz/1SLa6AvOMq6MA//1uH9/f2vvHz58td+/ud//r3Lq1ev/tL1ev0vHMfxKh9Xhex/MZxJ+/rrr9f3vve99cEHHzwV" &
            "nI+t8qX4P//n/3z9/b//95/+SGIeOm3S2wRlQB58Yjj4BvKYZ8oV2+QzXBv5vnACL+wZJo7tzMH8Hod536ZZx/kI2qdNyr5tAcdN" &
            "3jQf5LQa3RjTGtfzKK/cY1/4OJLj5b5veyQ5rJVjy7eUi2NZRZOIj7HWyDk1bZ8axxa0Ojkv1CYvrfGoFVDD42uIZtMP2HfNxK08" &
            "IhznDDxWj+Msh+c8aOMlJ3H5tez1Z580fbbW+i+/++67H16O4/jVu7u7X3jvvfcu+Rfm+cOISbDwa6M8DD766KP16tWr9fr163Uc" &
            "x9Nf2f3666/Xn/7pn67f/d3fXX/yJ3+y1lpPf0k3BQQs1BPkgbp/Czgxt4K5fTTsT2xbLG6k3UITjLfeZE+fdnPIPcOO41zMd8Yh" &
            "Wp0+3jJnTXuVenJsNfEidj07G9Hsrt83tjWMv4G+xLSbrXWtSXu7sZsfNN1bmmtcww3WceHmfHfDi93+o9x0CdcTtJp3IHcaW5vr" &
            "54DjO7uveA4mpC5qp93d3T09Ex5/LPXu9Xr91ev1+tnlOI5/+e7u7nsLv9u+Pr5CylNn4f/nfXj8We177733ZP/xj3/8xhfjP/zh" &
            "D9d/+p/+p08fa+VdR/RXeSJe8DNh2hNn/jQhsWeRGMdJ5rn9V/00zjoTmnZi824uOswTm3Hg70Yxv2vj7/9j9/dYLT4xb4P27nRB" &
            "L7nj45yydtdEXt4RB5675GMd0aU268q5+0T2a3z2J+fSDTD2+NYN6899klycWzaOM3Uw58RNDp6H5zqoMY2NHI+LvID1uIXLdpQ9" &
            "lb1ATvyu3+OgLWAMbQbnoPmpv8p+8FjJs9/1Mp/zxG9b0GpdsOcNAeswXEvqv16vd/f39//aBx988Bcvd3d3v3h3d/eubzhJkI+s" &
            "8iC4v79/+vch67GQ+8dfXj08PKwf//jH63d/93fX559//vTASKHZBLvGDUS7J4Scnc12njMX/Wy3oPGdi0f6feHx3I32aGWzMra1" &
            "VgePOW/9Cc5Bvas+I166aHNuHsfBHLz4XFs0yU27FR4HazCP5+k7l8dgH89bP23SZYudfp9PSI6Je4vd+dwYE3CN/MDcXetneSdQ" &
            "t3HT574k1zGtb/6t/abH64QIx/Zb8Jwx0b76r2R/4XK5/Fcu1+v1F4/jeJEFvJb/h5d/Eff+/v7pjyny4fHV4/8i+Md//Mfrn/yT" &
            "f7K++OKLNybBN4ejvCrxxfJY6DcGM8U3NLv51LDPfqItojmB9Q+Mt9l5nr71eBG2+mjLkTkNazC22XPOGlirfWm2MbaNhRw31mP9" &
            "i25O1maOwHXQ7jzMN9lbTMOkbx/5Hk+bU/aNSdf9yUa7bZPPdbkZsXFNzjSaThBf409x5PCatwbjp3tDWtaj8Qw+0NJv9udgGtOk" &
            "xzm//9kfx/3gyy+//MuX4zg+vbu7u+SL8wP/Zej9438yco//iOQ4jqf/XTDvTtK++uqr9Ud/9EfrD//wD9eBhxEL3S1AbARjbuGv" &
            "MiHTpKxhwrhAwaEb05SDR2t7HA2+SKKzqyXcwHmD3ZwZqdF5Wr6GFttiwuO730t51W1+9KZGLfLZp3Z8rjF65txSBzGtyXNATevH" &
            "5jG7RsJ71fvMc0Tuc+E587XAOlvt1OGRdvPYlq4/z4tzJyavvDlunjvvVf9eLmDdPOeRftbGWvyuft24t1xv1pZ+1nymeX9//879" &
            "/f1fvNzd3X10d3d35Ce7Bz5+sODD458ueffdd9fx+KDJT3dfP/7/Hj/84Q/Xl19+uQ58c78eC58mNEdOpCeJHE9s8+8QbbalWnY+" &
            "b2rGrBsm3xrM0VrgmyLjXdOZprWJ2I9yMTs3Y2wLbG8XAuNbjbbxAmM95lzKw8l+a1vLPPfdlvYYQR41Jlg3596X9HmdGMu5Wpvr" &
            "zBrWauNzrRNcD9fHe5x8x+e89R1DxO8xuZadxirXOfu7+SE4F20cQbR43Vj/DNZs+slBzoH9hrW5XK/XX7xcLpenL9ATzI+rcrHn" &
            "YZH/QCox+Qjr66+/Xp9//vn64Q9/+PTu5UH/NWzO09rmZGOMOUTzs+02A/NMx5xTJxNJf9BszZ4+22Snj3U0DkEfFv8bceTwSE32" &
            "W4z55LWag2xOaliX/rOHAWF/s/mmRZ2cO9/Uom8wZ25SjjOol9gW03xnjWO0zzy3xLUa7WN/4UblPI27NmMn337GNkwajXPRd2+O" &
            "40285Sb/OTUGR/lOyP0J00NmyulxkOd5eLz3v385juO9tdaRh0cWd+GXV2x3j39ckZw8XH7yk5+szz///GnQmbQMIkVwUqcWHvm0" &
            "BbvN2FrTmPzkHXpX8lxd8povm4uLbY43gy/q1prOcxrB/OaRf5QbT/zTOWOnFr95zJl+6uQ+ND9tqnWXMzbD/in2KOO3ru1nHPdp" &
            "57plnH5wOo6tcW6pIX1eO+1VdOz52NzXQY6uwTkZN4Fx6RvJdadfOLKGhvjJYcwU18DrzZjsO+zye1zmeT0e2zuX6/X6ThYvP8PN" &
            "R1Lr8X8IvOKL9cvlsl6+fLkWvhsJ5/PPP3/68jyaSXj3+H+mX8qXeeuxQD7AvAjRbBNHOwb3xiRQj1xvhsTF3vzeDLv8brloG6K9" &
            "MB+cw6UvswjOW/rmszFuaeOzvmls01pExx8DUD+vvmMn15y02GlzzbGtcvFlfpKTNVHP83E2DseH1zQaNznbHLPfxmlO+i2/ayWa" &
            "nbGxu85L+Rw9PupxbybXlDM3bMO8qTFHw6EXt+GxFuqtk08bAo49x9aC5CfXaHGxL6zHgT3Ee6gb/dY6Q+YguP7smfDychzHXQbj" &
            "QvJQyYOFxfpVxOvHP9uefzTIItcwobGbYzv9Bm27vLbt+oT1Ao/lbIOtkzyBdc98Lb8voqblC3kH+qnpC4ttB/KmeDfGGo2T/tnR" &
            "oJ/nbazkhcNz890uw8PFum6NfwbGsz/VGPBmlDjCcZM/yPVkTfMC23jPmdD03N9p3JJjB+Z3TsPj29msF3vDlGunwbXZIbzr9Xpc" &
            "fsb/M9Er3kryC/J8hBXkAZMHyVdffbU+//zz9dVXX71hX5uCYufA2gDNvRVNa5fDwES9MZZ2o+BFHezG9Fz7Fa+ggymXa3Ed5vLY" &
            "9Nz/LppvgrZPY+C5OfH53Db2V7nYXBMba2adfhfV4tzi271DcLwfOEHbH4ZrYi7miN03Ucc5xjURzENOmys2gzWxNq/hGZ7LX2X8" &
            "bbzmBKnb90TyHBNMtolPOK/nm3A/OPQR5Bt6IV3x4HDjAyW8Kz7WyjuV/O+D7eHRCg6nFTbZzVnDwD1RuxaNpmM4tuk4dy4y2sO1" &
            "dgPHO3EC57voYwbGh8uPiwyOozVjshvWOWuJ8dzStt7ixtD2WMOUO/a0zKW50WiNe4O82Kzl5jobaHec9c40HGvb2lynx+N4vKfd" &
            "PGbCuoTrYXyOvJfQ3nC94cFMuPaFeyvvi+HuxjLBc2R4fDs4Ptps1uG4Ljnxw4P9PCCa/wF//uSrr75al/JFeRsoB+kC3Q9oN6fl" &
            "y3lqWvollSfKC8+aaTcanxd/mnPQ71wTpjnIeTTbq1rW6eb6OG+J81jIp+6Uw37qME/guDTWdgZrflu4Do8jfXOnOhzXtHhMjGOt" &
            "eQucp63vKtdaMNVj0JZcjpmaYf/EIZfnjnH85L9Fx/0g9zg2w3VMSB7vE3POwLhpTOROuMTpJyTfdeRPmeQdyFVP1CT++uuv3xiY" &
            "N+JSsbQRjOeG9qSZ0za/NdczHiKTrenSnpjW7GNsO9/FMd552sPD7aIbHXUD5/L4HOvzXYsGa7DPWq5jlYfnLpa2wLlbM6hp7fjJ" &
            "m3TWULdrD48xzj/pr1KHtQxzmS/tjG/7ws3U89W4tqVN8xS+YZ/zRNN5Jn5grjktvnEaz2g8zx99k86tD7EzjXAuhz7fIql9lLX0" &
            "/UeEjsd/OJiBcWM3TAXSnubNZv/EoybjaMt5i4uPrdndT/MGb/HtArCv6bjPz8b9OTn1nd+2S/llCHm07doO5nKct/iN7FuPl7HN" &
            "tpvfgOe0TbWE79z0NxvXjbwJ1HfcBOZOazVO/MwT5+uMR+2sUx4g/IVX7O1eFLhm698C12qfYdutccEuX/wT2hwEO02C/jav1LEv" &
            "cC5qPl0FeSDwS3O+C0njQ4QLmf8HJIV4cOG5EDb6p5aN2V5he2Nxg4ZDMJaTyzzOT72lRTbHfuvFl1xByxewTs4D48xjzeZ4TaJr" &
            "O/2uz7rRcEx042N95KZ2z13TjG7sV/183FyC+kbyeR6p4364tDdOYjkPfEHm76Si+4B/mBtf4FxpGQd5XFvqkM8awvG6NC3qEeZS" &
            "q8WE1/bLKrU4zvXwyOb6mS8x62SvrGF/BVOt8a3h42He44isp9fBY2EzJnvAel0/a7vQmY+kfM4+332kXR9/xst3KUmU43PbpHE2" &
            "SYx1vHWyELQznjYvEDV9HmTCCecg5yivAlpdtntOomEtgzHPbVO8azFosxZtbksXTGy88LJXW17u1fWoke+JnMvNa9/0gx3nVjvX" &
            "b9cc67qpadt0TnDcDYkzZ9ILN9fdLfvU40y7lo/RvUapwTbXlz7HkRzGVM8EajimxXFt1xDT6rJm0zcnNsL5Wy7i6QESpO8FcmsP" &
            "kZwHR9lcvOA5oPRtj07jtkZkXOY8p7lWXgDR5rn7PqcekT6PrqPp07e0fkSLpb21aa0M8zg/01jdbJ/4zZ581tqBsdR3a2Mzx83c" &
            "5FvDu02ee+2iGfg6JM+1ER4H4Rj2bTOck5zm4/itx/F4fKvMHTmtVtdkm+uyLXGJJRLT5nMNte1aYgLnp92wDo+3NsNzv0pN1/wK" &
            "i4a2OdvA3Az7z9ouxj733QxuDuJa/sS829rkDlq/ndNmTdvZ90Z1vOMMa07zETDvmT59rHNqjjnzMY+P9MfW5il1eR7XjXubzTzn" &
            "aefm2U6wRms0vq9X870mOzT+1ALfK6LTuESryXHxNX2v4wTrsbUaiLZf6DvTuGVvJccEj91oeq7H/Qm7XI4P9+lXWDFc8XFVEI5F" &
            "WJh9xpl/icNJuSXWYMw0MZO+62hzkfPW6FvllVj82TzcpI131pwjMI+5eO6WWGqcwfwpb+s7d+B12/Gom+aYC37efIf/LdGgvuuL" &
            "j5rNTluzt3i3FsvxNS5jzJ0wzR9zOleOeYCkBdRjXDjUa9qtWfdS/tQKjw1N7yjfgZrX7PSRs258eBw3zG2OPLeG453zVuy41g4u" &
            "LGz3kVU4DR740qCDo7xFp6/huZOwVI9jObGXcpMxl/4zH3WYY5fP/t15LhhflFNNU+MFSO20py/INjc36+WctTQN91ub0OqmbvOx" &
            "b465u9byT/FBs9HOung0cr1Qz7lbHa0ZtjfupHHoy3DzW6ztOTfI59jWMB/puw6eO0+zNTDPLTGuydjdU61PP8e9w+S/pf7G4Vgc" &
            "+/QdCB8c/n7D33Os8hbOi7zwUGoDp+2CL9XaBnDRuwHZN9XouAnkOpYa9E3Neoz1DclccvIK2nzGsFHbczD5d7wWs4unfaqxcXeN" &
            "+fhF+FQDX6l6rly7dahFbqtzgv2OveDFhb/Yb/nCdT3WdX770w9yDV43fwOL5wRjeR27vl1Og/VecJ/g/aqNK/otX0Pz+17EPEa4" &
            "zD3x2nht8ziS+zL8Z3aZkwmMWdBzHvqb3bjwi28nMe7xBxUvevt4HMfi/2q4ygLQ5sI4GIL6XjwvQuN5sskjJ4362bAEeU2D+c1Z" &
            "ZdyxNd6ZzeOPjxfNUX61McVZn37qea04f02POq1PGzU8Dtp5XOWG3GK8f+IjzLFe055+bhn/VBt1HDP5lvwZ12X4Yn5pvbl3XRf5" &
            "PAaNy3HTTxt9gXkN8SXH2Z5tjWj9NI75qgcU7ez7xTH12Wcs2wP+mkdgDtFsgePM9dhX0eNaxu5xGW/8jNeCDmwTljZtRmPyncVO" &
            "9gnUm2qe6m7nLb991m4+xxuODSdrYztjHM+bpjmMncA489hv87uGm3qzWc9a5Oxs0b7ob1I5p/tndjbXypyuxzG2Bc2Xc17Utp/V" &
            "u/Qw8LVNXYNxU37nMofvkt3MZ9yu7ZC62GdM0+I4A54Hzn02n+18yuF8TZtH28233+dByzH1jfifvkRfKISNAdw0tGejUINwMdFg" &
            "W+VdCI+TrrlT7Wze/G6MC7fFkNP85LRGH8+ds82LdZrdej6f9HYt/CmO9pxnfto8Wc9IvN/tpk2+2NvHQfa3unY1p93hi/hr+UWf" &
            "a212xuSc42adrNearUVnB19ThjXddryG6dr0fE9jtEbzO3f0bY/GFQ9J6zFml+MMk1az2287/Qvz0fhBW+eJ0zRYD/2X5iCa3cK8" &
            "UFuhgXOdNS6sEfvxuDkarJdm3wRyeZw276F3Ow3heUNbx/Hu7zDF20a7W+qbLmRqNd3dHO1uFMQUQ1vzu3bnaH03+s2xj+O9bN6J" &
            "tEau4y76QUPLnTjHWvM5sIYb62j82Pyw4A07nBbvFk7yNoRrTtMhWGPLGzvROBMaZ7LtmrmB57jF3XJPPrMR9F9o8AbNxdDgQp2w" &
            "6U1xboZt6Vt/ytHQNrMbwX47N79hV9+u37R3m6LB/KbtnFMLt8F69rm1G+PZDarFMM65dzHftjWc5bHfMCe1t7jdzSMx8TXEH1gj" &
            "cba5Frdou27WFN2mv4O5t/DPYD1rN9utmGL5lQB5ma/Y6CfavavhKN8fGbdoNP/Tz3jj5Fv+4HryZwOM2JjUbeI1HXLY900x9uPk" &
            "ogs4dsJc1mlN+lss7fQ7fmqMT/O4G28H6ztnWhvn1L+1TXF5pZ0fYZBLMOY59U374bmNujl/jq550XC9hnVuRctpHc9j4myLvcG6" &
            "Lf4551Oeiz6aNK7l45xb4bpZi7Vssz+2xrPtVvBFL8eZ9Wscw2NiLbuaJr2LvxjnMUH5pQAfIgsb73ryjw9bYc7VwIlaGy37fUG7" &
            "7+ZYHwNzfD41xlrLNtdqfaPlcItvx7F247OfC9nzRX/Tt601zkGQfZBfrLQ8QdPydx2eYzfXRG3i0K+wrB2Odbxn/f3GBOs0bdva" &
            "uW2en6DdNFr8c1vijOvJzf8o18gq9wqCPt+jXFPs0/lU1y14m9g2nqV3L5mPNsa10SB2Y6PdWm98hGUBT3yCyT1wwViLPl+cjdts" &
            "k59gnzxvtBYb7lmLlj+TpkbLdQx/Fbi159RD7hQz1dM40XCfeag32a3LuaI2QW47mru0D1wTbdZpNbk+9idN8o7hIUK/25k9R8Nx" &
            "E6zXNK/6fzmatnP4BuJPLBjPObC2dRvMj6bPd3C+XW7ncuyO03DGmeye44V14D3YoI0aU56Avpz7mHPrXG5ZhPUYzI0QtM3hvm+6" &
            "jmGO8Pk2NZzGnfq00d4mljXZFz/7sZE7jcdjTcsDmbGEtWyPL+Nx/hw956yn2ZwjLRy/Sra/xbhZu+mEx75v0lNd9jEHc/lVf2I8" &
            "B20euTddK+0tB7lphOOpHx+vKdbpHC3fUT4GXcONizcsvoiMzdc262Yt4TPW9afx2mCb5smcNieNbx3CvHbeajeHuvY7vtVtu9vC" &
            "C33iuRrscx2tZzz9CmuVdxyxU/Dy+FaJ/9AmxdPOOMIDWWWwHox1Jr6ROprebrHi84X/HFB36UFLTsvrFp/raRza1uMcNN5Oy831" &
            "Jn66iYXrONpsd4uuc8Vn/qEHght1c97qYD/nUz7rBPZHo9VDf7Saj2PneTiJc/xFN3PDNx7fA5Z0d7WztqU9b87UrMPcZ22Cxxiw" &
            "LoMxTT/z4xoad5W14bnHO8V7TmI3r7Vb0GLO4sd/B5L+ExGLaxx41dA2HeHFtN9x02B8NFgLEQ03cxpoP8qCOs792BLrZh1v4qax" &
            "q5/HXc6pEVzXSYO5fB5M+gH9eSC4OW9qSrzn0/vAuT3Prr/loz8c9skLPG+7uWM/+m2MrbX88U3zOMVHo80h63ZskJtseOSHy9yu" &
            "y3BtruPM1jSXeOnT12xT8zy0I7XoIzznq7yYX2UtWh0tpzHd+3e4LD04rvjF1YSpqLz74K+1DBYZUGcagDdcO7qeNeQLWl6C77KY" &
            "mxu8bZa2+Tm/63E8vJjJd83WWrohXPT3cQjGco5cO+u3jWixDU2PvtZcD+1u9lv3OeB8O3fTot1c1tRiW/07WNe2aU4ayPeDZKfJ" &
            "+PTP9lurNdr2ke/z9Het1ctc5FH7bN3dwiOfoO2CPUkfOZzDnPs+0ebZ9fjcrfmMs5xGdJ7eTngyYzPOJpw+g7Zr+ZeTLV/wVLBu" &
            "3N4k5E4g3zz3WWfyecNOjRrETmMC/axlF+t63IzJZn7Onf9tW5sHt50/tUwtHPKDaT/vagpsdy7Gk7/TNcegjdw21gk739K4XOst" &
            "IN86rc7WPKYz/sShjseyi4nNPtqbz3Fn4APD94lgsjO3m3kGbdTf1dHw9BGWJ8B9wsWmcZHONkoWKT4ead/FTxvCem+rYY6P9rNN" &
            "eRu38VqbYukzzNtxjV2cffG3+bLfca1NGmmuc6ppstPnuu2/pbEWauYVvt9lOhd1Jp8b0XzHjXNuDcb6eCkfm/CmE3t4BLWnI9u0" &
            "Luk7dpV3eITj07I2LUfTodYO8ZPbNH3Dtp/2CY6Z9K1xa+4Jl6XP1VyEbTu0yYm9LYhzudnnjWG+4whzbtFo57e0KS83tzkeG33W" &
            "sG3pwmFc07C20WJb3tibL/bmp27rtxaOa5zmbeHimC6S1tocTq1p5pz29lHRND+eK/qdx7loaxpuTZM1kMubLGt0/ML9JOA6pLnG" &
            "pu0azE2Oxr2UdxETt2kGjr8VjU+bc/EBTL951m0co2kaznfGDS4xcNGv+n8/shjpk8PvO+7u7taLFy/eeLWVInYbxwsZLuO90EGz" &
            "xW5/bN7QzE//DqnJtTMvNdmf2k7PeWm3jjWtk36bY/JZT8sTrutlnH2OmziG/c4RjcaPf+nVsnO32KmRR/30qRV//rsDcz0G6rpv" &
            "bdZD2Gce/bv41OBarEtOwIdm22uTzmTzOwXC3NjWsM5HqTdoOlwjwzf/nDMH+y0v853ZXU/LSx77roHNc7RwvRzl7/wdB/6YYsMV" &
            "/wqdNnMo7IG5YPJybnBQQfLwZ8LkTS2xDfYnJvWat4PzcsxtsQjWF5/5jgm4wEtxXAuvQeMSzs31iobX3TFunA/zo287W8sVWzCd" &
            "p8+YtDY3jmFs7PS38zb31glYy8QJqMO6W56dljm7Fr7jPb6so2E9t2D6AY5rYG5q+FqmL+eOo91cxgfkkNv8jp3uRYHvp9by/Jo/" &
            "4Xh8ANzf379xT49udFp9x3G8cd8l543vQDwJD4//4cn9/f2TvQ0u8MIELmyX0740/zKsxXEzE4175o8e6zHMdWMcOYwNMj/WSGN8" &
            "y9vqcV3us7kOwnmCqx5crb74cu5aXZtrnHQC+6jjc9t2eWxvMZeyz6gTtItzyudmTdaT/KyP52eg9tTIbTGuN+B4HXPohmRfa66h" &
            "IdxL+ZUZ56ehzfV61PTaTXAsbdwD0XKNqZ31tj3jHPQRyUX7Vb+StY/HVeaaetfr9c9+hTWBP2Vl8oCD8iLxSDsnsWl5wR1vfgPt" &
            "LZ7nrCGIn3E+b1xevM6b87PxeRPZHzQt9yeQ37icE44pYN812kfOhNSyg3NYz76pcY48X9Hccc9a00k/89r8roNxU00t1mgasU25" &
            "yHeu2NzMMWwzn9qtfxbL+5TbWWz0m43xTYv15Ri0Oq6PN+E2fznfYaptB9d1BuawncexUg/YDxJPatq0+K0garAwt2ZvOPMH1CQ8" &
            "LudvmPyul7yzZl4wbZbwfCHzvPEdd3nGv1CPjnHGs5+8CeZOcc7p8wNzwjGT0+bQLb7plWMaX/063nV5DFObOI43z7V/m+axHGX9" &
            "aEvMczTIXXqHYH+zWWfhO5n2ziRcHhtaLudlvO8p8dve7jvU3PGI9J0zPrZwcmz1O8b5Lu2GkcE5KPZ8nNQeKizG8bEb5rVj22xT" &
            "Lmu5ERM3/enYtBZeXbBxk65Bp/ndpz3z4bfBsXlNW55oHo81WiN2NtqaHjXdEuOLlzpTTKuD/qbR9HYt4255zJvOd/VOemmG/eS0" &
            "fqslY+LN03U51vHmOsaYeNSxnvvWo26rO9r5kYJzXcp3bLExn3PTxvrYJi5j6E9rN/mlcbLedo+Nfw0PDSL3JsfQxhbYztinB0gm" &
            "k4g4+2fNDxfHubhVJpm22N0PksMDPLBBGsy1bnDmbwg3NbAe+lqbQA2/kuLNPkfnZe5JtzXHs02aazM+54ot6zhpOH8bX2IY3+B6" &
            "rOna2M40Gjd8HxuP+9k8cxvM9wsBnk99t0nbPh7N93o1cB0X4nneNKkd+L5EO/20+TyY6mjzZdh/1qzpObkF0xiafZW9ZnvzBZfL" &
            "440opKVNzEW4XC7r5cuXTwPiN/kPDw9P8V5QToQ3UvrhsYYrno6syYNhjWxH+YOIrsG6AfOwxpbPMTknnJdouThfjqV/6UHPY3jp" &
            "Wye+gPpruKDTj48PMeq7buZlLvNcQ3I1LmHthqbBOpy/1eKYpnfRx2LWmfrR8VzGd+CBkH3X8p/pT1zmTmziE7ew/6fvQ1lrYqjj" &
            "2pyXcU3fNo4pNueO32O74F1L/DswN3/Us8r4PUbnJc9jJneVvRT4HsT7Gf3Xx/s3YU3Wad02huM4fvYfSvGjKBdIsDgO3nwmbmBM" &
            "Kz5ITXw4Tbk4URkH9Zin5d+hjYcTbF3XatuuNRy6SHbNaLU7Ji05LicfdeQmRhuPbra7lqUXHYxra7hD07DNGtZvPNutEU6D4zyf" &
            "7Uh/WwfmyrnXxTwi9mNzs4ieYX1qpV2GG7Hz0N72qnGUhwN9Hg/rcw760m9oY7UWuT6fdIPmb/dk18v84bomax9lj63h/smYHG1/" &
            "46/xPhlx0TZRJqetgTzfDNg3N32i2Y/yZVDszuuLsV2Uzhl4YQLHUo9+c1te8nJ0rVPN1iDsdy7qOU8a32WQyxhrt9zhrfLQsLab" &
            "9dvReh6DtVodtzaOkef2Nx9b8k91nPnN9fjMaS1w/JnG2dgab4K5DeQ4R+peuF5tpy9wXuvafwbyeXyu1nQ/Y3+H+PlgSVybk+k+" &
            "v8Mb/5DwjDwVcUs8fYlp7RY+beY2/lFuJqydfcY3W4uZ2s4fzYkT+y6PayXPIOdMj7zGtc15bKOd+uxPnMvjjZ//V/rUnM/jiJbz" &
            "h294nC1PYL+PRupLTKuH9qkW+qcbTvxTDvrZrOF8Pj+LTd6FuXUdjcs+b4ZE6mgvchJH7q6FwyPRuMxnPXKJNg7CMenv5s6gfZeP" &
            "esQ038GFhOvmc83ATynyfLRtlYcQec4dnuHYY7hAUudzmnXYkrtdLEbsk9bOPmlznZbmgXlba3l9vqvJuWxrPMeQ12rjGMLzDT86" &
            "5Extl88cwzXbd6bNY0PsrIGatrXm+nOMbhtj0zY3yD5zjPsN5vncOadm/yqvqKmTc3/MNfFsc37DtTnGHPPDObuHWjNjbtf+rhnW" &
            "IZyf3Cnu6QFCx1ToFX8jq6EVznPmaFzn5AYgPFDyb13QW/TMP27IYb0z3V1jHsI8NvpTq+Omi4awbhv31MJteZq2Y8hPzLX83NB5" &
            "3Mxr+cn7Ns11M+dUi3PbZ3+Laa1xMm6PPzaCtqZnbrMH9F/KO4TnNsdOeTx/7jM28dShnRo8t9+x1rgFroN9Hn0vDVgj/R6/0Wy3" &
            "4LKGSZrQHjiGiyR/irs+/kogG8xf4hrRt49jSOODaQL56U9+c+w747i+FrNb7MBxsS29iqfdcC437wfmo99HN8Z7DzhfG7svGLdL" &
            "2TPWcpv8Z2NhYz2XMtf0uUb7ct5usm3cRtMPmgbtXhP6HEO/bYZraG3Ht+/QnrRG7iOeP84vY4mp3/TMsy2gfeJMmO5bXqt2bRDT" &
            "OJ4D3vfZnt6BUDQb8cDmTmJyD/yNKsbSvxvctXxktjRQwrxVfkoXMJa1089xpsXGcXNMnC/nSTy1qEfQzxhzuVjhNJ07/JzWa8ef" &
            "XjMmeq7XtTKmcVhDdGNvse0G2XxE/C3GeRxrUINaOQ8eHh7GnGxH+SVT4DzcOwvry+sgfvJ8zlqy5uxPvjZG6tPXQB3bqH8p73wT" &
            "w7nyHObv7nF8zkdNXh+x83rJ+JPL2vwLyS1Hi1nDvSjxPhqslY2w76K1JJ8ca+XcvolPTHl4/s07BRCSE3DRuHj2E9YwXBht5rA/" &
            "NfJdz6HNsIuNbTfZtE3n2dBu4Z3ZaGctboHt1qAOYc5Oi35rNN+Ox7711+bVWMB8jp3O3T/zeQ+087Q2t43ndiuP3IZDN/HYEpdz" &
            "czzP3muMdWu+wOf20zchMVNNDVNdjPW9gb4J9jNu0kub9sYE32utxxawv8vVfKnfx4Dz/40v0YmpuKv+NtZzcGjTOgebF8LHtLYg" &
            "1nIL543J2NyY2dqDp+mEE7S5Iq/FUNM50p/qdoxz2Mc+c7fm+qZGjvlTHa2F22BuWhu/+e6f8Wl3fp5f8O6WvLM5vgw3fcfumsdt" &
            "NH7jxt5izhpjmr77sTFnbOaEx9bytvzkn+lOHOahnci1TnvLb3/T9/jMn86d6zloD6xV1uNpt/rm5pt3isnbQU4QRa3j5Jfh4xG2" &
            "BuuknU3QbkMsTXaOz1mwxm91TXr02R/QnzVgvayDtdza3ib2jN98trFvn9ulPLTdPBf2p92ik48LaJt0vU48jw4fJtPNpfVvaWd8" &
            "5qJ9Gk+zO9a+yU+dVd7RtMZ6J22ux25tphZ+yxEda5rHRnvOCfOp3c6nMbFm5/A9240+n5tD8CFi3hv/kNDvKPyQ4IBi5wAZ72RO" &
            "7ILsnzjMl3rMZZ9atDcdLxZjW7/FkNvOqcO54sYgmIufZe9y2x4d2oPYeJzin9usQ1vL4/6uPYfb2jQm1+U+7cFO96Lv2aYXWG7m" &
            "7Pipb5VXqz7PMeNKfdlbfnHSxs6c5LTcHr/HQFib/MZz2/ncqOW45Y9pyhxw/tKoSS3ns8YuD2MnmOP4pkObfQbH6Jhv3LXyEOBG" &
            "b190R8D2Bg8gfRY0wQNNXJtsD5SxeYrSbx5tzM9zxvqccFyOrMm2ptls1uAaNP8qc77Ly/imFTSNFm9+OEubc2rM41yXMu+MCxxD" &
            "v+vikfmZw63FuDbH2m4NN+dkjPP51SjPHe9G3cSYY/7EaXWmsR7G0N7qJtxv8Iti399Y24LmNBbX7LHE1/gB8xm0n9VunUlzd59u" &
            "MR5fy3m5XP7sZ7wsLuSr/rpuWhB//otExgUeIG1sDbHb77g3BlR+5cNFXuWdVkAO46bNwBqCa/kHQqtoWyN9LhqP5joH83q8Sxti" &
            "qt12+x0z1WK7z8kz37FGfG0cjnO/wfmob5/j2ny6v4br4Sx+8jGedbnvPe44t5YzcfaltXfE07sN4lI+NSDnlnMjY2Ve2jkXnpeF" &
            "mpq/tTZPAeeDfMYljxs1Wk7ndiPaOIzYcmRM7Mfw69PHWv5sEuJYReiq/wrRAyBcVPgBJwyFPGkm91H+7wieB67Hi+G4ScONiO2i" &
            "C4d8ajZ7szWfeS1nwHF6zFPbgXPHdXCd7k8tNbf5SiPsi4ZtrMva1mnxu2bYz8b18QuX+H3eOLZNjeNte8K8nHsc9sfOMTOXuQFj" &
            "eU23upqO84RLmE9Ncto5wf1MpH/Ri8/4WlvlAWA/4xuOzdrbz7lxo936ge8T7i88+BrnwH2Z8/PG38IKmrgnixzCsdT2ZKXfLr6l" &
            "yfLishFTXYY1nIsLNsU0O8eYc4+7aTQdn08arTnnlGfCTofHiUeb58Bcx92qbx/1re062p6b9Dlmgr7Ep+9z3uwZc2tzbemzlrTW" &
            "d3w02KjXMGn7GESz5Wb+My3r8p5kG9HuVcxFuC4eG6fZGNN4getpmDRocz82gtwcHUO7bfTRn3n9xl/jJbhQaff3909t91PeaZJS" &
            "iDevL2Yi/DYQc5+LXbwn0PbGcfM4mxZtiWGsNa2/sy1cyFkT+gj613DTcXONjvGrZMdNY6X/uc169PHBkb5j0zIXthH2X8rn3PY7" &
            "j/Vt4zh4JDfnzfY2jaDdtdvv1tag6ViTujnf4cy/wLluflUU3q4+x9nGcTJvjr7OHE9t30ftbzDPY2m8VeYlHHKJ6/X6zS/R1/AZ" &
            "+sKDhN+HtIdHQyuKvlXeoRg7ezvf1WatjM0PxPDY6Guc2HzRtGbYby6Pjp/45Nk/4ZYL/axNsbR7fgjHmGuf7a1Zd4oPz8db2gUP" &
            "EcadzemUo2n4nDbmaZzntCnedvePzZ4zz/ZJy7xdM8+17NC0DObY4YyXHG3Mzs++feRMsG7LYe4Ox/H4DwnTaY1k25qPE7HKE9S2" &
            "1g9aPt/gA3J93KHlYPzkZ5sWv3HPmnM0NF7jux+Q2zi58dxyIwqsaR/tz4n3OY/02zb5ll4sBNZ2THQ8N7e0Scc8cwPaHT/p3MKh" &
            "tn3PHSPjct4Qe/Mzjnq31sLx+hONpm07EVu71yzYz/yt79ytTtbkPmGNCWc+57Of18yV312TlEIIXmhZSH8skXj7PDHMY3hBzji7" &
            "BYltAn0cdzu6bus3v30tj7kTdn7mudOvX4KsJzn0OcZ1Nnsa7fY33XXysVj8jiFaHvtyfgtyMawydtbKcbfc3LctNufWS5+xDbQ3" &
            "Xeu1nK1Rm/bLcBPe8afxtEYdcj3H1j1r4bb8thGtbxsx6axyn2r3tqkF/htlt8DzH9DG+2ZDeK0mxx7H4/9IeGjiTQwo2hbKCx0f" &
            "49umCPgT4fAnMI91bM8i7nhtsZs2EbvzMYZ9z0/juDFX4pvOxGdMmvVuaa3OXTz3h2syd2oN5rhNnEmj1eP96Xl0nukG0bRbSwyP" &
            "1HHftlaTm8dEu21pDbHb79hJm3U4Lr4Wx7YDdX3MudfJjfl3vPg4nglnORk/3X99fwpcT8NkbzB3qmf97KHVb+YW8WDbwI9ysdHn" &
            "c+dwoX4KJ583YNBqWuW7lTQ+NBzLPPbFf+iVP2OmnNQn32gased44E9lXMpPENfjnEZnqqfVMtVEXjs/y5W5jp98j6NppI7JT3s7" &
            "Zz961nGOy/CLPJ/z6Hqc382ajmGcc7RczW6Oba5nyhM/+2c3tOSxjjWp0Wol7Gs5m75t3JOX8v2Vc0z6zMHYnFuv1c+a+NAJ/L0z" &
            "46Kz03YegrUyjrqOffp3IFPBBDmGi3Ph4exy0Dbd3MNpk8hY+1JP45AXriewjYexPrZGPm3m2dcWjj42PkQ412kcy6QRn8cdfnzU" &
            "ZI6WzzmtMfFdm2Pcb83xl8cLxTW1ttO3JvnWbNxdc7y1wgnMsVY4t8Yw1rbjsQZq+v7BMXrMjUM4lxs51DbIYbzrYWzr037gRRC5" &
            "jqPNc8VYcsIjl/xd3z7b7CMn+ZtvDdfpyotTdtyCK/4RYQbZbk60B/azsAYX6jhv1KNsRNvPcOgV1A7Ms4aHquvf1WOOGzm38LMR" &
            "2c44sdHvGLf1jIeIm2H/rnlMZ/EeJ+08srXaPNYWx1jrEOS3OnJu2ObcriGcSW9J03VYz/YL7gXmm2ub+7FZ66xFJ8dcj7S5BTxv" &
            "cMzu2M6X7pO0tfvGGtYgdmvz3Gj8M7imxHJNjKdHXoL989zL46taDsQDjM9/6qQlpI7hARjO13y8IH2hh8cjwfxHeai0GIM1Tm26" &
            "oMlpNrdcwOa2scfuj9x2bYeJ53yt3YqmH8ROv3Oz3+o9s+fc9jTvLaLZDOeYNJ23NeqcxZljLveOeeaazxw+J5rdNy/rZ//yP4Cy" &
            "To6+doOpRtrO4Phb7bRN99sW42Ye+ZPNc9vO05/y2H69XteFf57ESAA3iG/y8VuYvBbzHDDe9aSm+FhP8rjvI9FsqywKGzm2xd44" &
            "sXtupsZxXE5uxOEkxrHMn6Pjn4O3vQAarBVb20dNN/2Mn/NAv2Gb+c0/+eI32hhyzLmvJ/vT5zpa59aWGGq2vULQxxt6qyfwmroO" &
            "x9iefG09V5lX25vvFrgG1sljq71xWs3PqdF1OI9hzVaPNexzW9B9uguZYMR31X8mdS1fQLFoa7o/wbycx87NupAztti9qaljfuNy" &
            "szY/NSdd1tE2v2Ma4mM8+S2WMY3bbEHWlSB3QtuwznXmt81+xvLYztPavLU2zVXTdNvp70CO4xxvm/u2Te2W+Wh79lDe+PzDh8D7" &
            "YenLWudhjnDIZdthFzc1Y2fzMefsn83vDuTw2BrztDmkhs9ty3G6h/v4xh9TZBFZ5PaQOMqX7tYIP6B9AuPIn2LD5Rg4cOu4T7ub" &
            "tenzv7mw3q2tae6wi/VHU2dwLbT7SE6OmRfnct+2xHBemcNj4jjpZ1y7STGnY1s+53Bt5pnDI9fC+dyaP/Wzv0PjWXPKab5j2xqw" &
            "ES3etkmHfoMagdfcOsyVNXFezwP1J13yeGSsubfq7nyufWq7+Y09OYzY/PAgPO/1r/G2Ni3CehT1v+EIyDuD8zGX6yQ/yEMtDzw3" &
            "x0z5ct76tDWtnNN20a8wnJst8EKR41rcpjiDdutb023SSd92++07kNf5Pf9p1Au8+Z3juc1au+b94RY7+eZa85bc4eRoPWrYZ52A" &
            "No+DsVOM4Vzh5XxXT9bUa0s0zalxDJ4n6hktP3mOaf3kbOdsE9/tVl64DWc82py3/imTy+XydBN2UQv/PsMfZbVXwdRshTR+jt6s" &
            "zTdpketzazZOswftAZV6WFM7Bo7fIX4/VOJr8c/RJmfHj49ryTxtXsnhHJFvnYB707G0tVjq715YMHfObWtoNxP6nIf1uGbaPT/m" &
            "Mhc5Czf6M244seec893AWt1azYmZ9NvRObJuAfWYP7bYXYP55O4a44lmWzfk9vw1ZP8s5Zls1m3wtUQ4xnPO/byQc631sy/R2y8B" &
            "pkngJuRm4UAYG6182UY9IrHUop0TQD9rtn/K04489wSewXmm/BnLrrW6FmpyXa5/ag3Rcn4eW4um7ZOGudPHU1zjHFk/89I35aFv" &
            "0o3N/lX2VsC4dnRr8+L8tu3G02LIIaxrHzktj/MbLaZpZS6vj59WkOdYg7Z2DcS+UM8a5ic+ouUkfG9cw95g/R5/4Dj316NOYpk7" &
            "dsNztxsP58O1mUP4GRGNp19hBSymFWUbEz2JYvMbLdb5mp3xtjXft23BtMC3wrqHFq9dQN58vrG673qsNflz3nxuXFM22silVuM1" &
            "nRZrjdasGTvH55iGnS+gHvPy2HJ6rBkn4Rr9xbTPncf546P91tbqC5jDcdYIz7Dd/WCyr7JeO+5Sre7TZvgeEE57mB2b+WYsER1f" &
            "1z5vfeIsD32pk7jqhzM59xjjOw58hBVjkEmgaBNqk7j0xSV5HOQqgzpDeGyTvbWGY7PojNv1m/YZz/5VXrns8lnPG5GYNBqH5y0/" &
            "z1tr8e3m1nSazc3xPNo2tWmep2Zdj2fis790kdIe2Nc02vlRHlJpzdZ0Dfp4c2POxiVYA3NO9x3GtBpta9pBuw4C61vTNsYRvOZ4" &
            "7Xm+btEy59DarY2u4xpc29pwG8y95l+ityJCyPGK//+8LYxtGXw08xaogTxPhBs5PLoZO24aL8DLyccerfmm0uJo59Hcs4vecYxf" &
            "WjtzmcN6Lcct+VosfWc4Sj2tOWZtfqFCXjjmTnb6b1mL5zTWbjt97Ls1P89Zt2ufNM0h7I9m0K7/gHPcajGci1z3g2gT5Ph8ap4z" &
            "xxKTPddc8zOX++TzQcGxTdrk52jO2jzwiBZL3YV833iApDnB9fF/I3z9+vU3HgQswsmpufOb21o4Pk6NOXxsjRvHG8i69vHCoL/F" &
            "m2MftbihvbmZgzb7gtguw79IbxqJse3WxnkMXL9j3G7heF5ia/3JTljfbeKyz3Ov3SprNunFxnjHkkuNxgvXtqm1uT2Ldx38mbq5" &
            "buE4JxvvS9RkDO2uh7anm2G5vho/3ORbZUy+ofOcejlvdU9xbsH0MMiR93R/dfEcMO/Tz3hZSOAHiL9E8WCtwf6ByU7ffrdmb9jZ" &
            "qcPjBOebWuMatjV+02J/ip3iG5ewz7xjuHBbrG3uTxeXGzmGubu4Y6jd7Uw3zTeTxDmecD8267Zztxa7ay1f6xPWcAznIEf6J+w0" &
            "zeP5FNNieUMkHONG+Ebv+1uLCXZ+6tHv80P3RWJX0w5nfs6b565h0juOx/8P5KJfxkQ4gfnYigNtg4qWLzz2GWNe3tmYTxv5qZm1" &
            "W/uifyUbkLPKR2wZbzSv+LcuiWV903wklqAvyHx7jBxHYuk3GMc8l8tlvXjxYt3d3a21yRfNdk4tjt/+1jiOZrPf57tcHrf5zOk4" &
            "I/zsKWpax19yM542XqiMPcp3AZfyIiuNdbAFzLmwJ1v+1to4g5xPdiP7y/7kiX2qzf3EJYY22pmntZYr+QLXdMGvSH2fOfRfKrR1" &
            "iQ7PzbN/Oiefa9W4HNu1/LcOsfvdiMfO8cafuKf/D+SifyhoJOgobx/XUDD9LtyDDSd9N6LZgl0c/alnQtPhkTrmZi7td6xjbJti" &
            "djauozWDnc1o+ehja/kmTes2X8ANTVjD+d3I9Xn63v+cR68lET/7rcVHHvkNky821uw8RxnTpNfgOJ9Hq7UzTPxjs5aOb5j48Z2B" &
            "962l+bKv1Rmk7/1jOH7S4jHwfdbwg+GMvzY5GPs0Ng+S50TsaQ0c9DT41gJrux43a7I++nd69k1wHh/Z2jsea7gO2mhvY+ExrW2M" &
            "tuAtlmgcj9Pn5jDex3ZOxM62yoUbOH/O2axn/i7G42JLfsbsYlkvW9NmMxw7NcZbj/HUsfbZeatr5w+cy63VE43W4pvisn+4j+i/" &
            "FS1Xy5k2zW80GncN//YkIM/XvW/0Dcx5C5pe+vVfortx4JfydtEaefU9FWjd2AzXYU32zYn+GjYNOV5Y1sWYS3l1n3PrUI/n1PS5" &
            "bS3mKB+JraFm1kRMvFY/x/mcOpsWG3nmuu6Wg3gOL/pnR9fFeGs2vnPx3PG2Uc+ajHHutjcnrvuOab6mabuPre3GkLhwmk7jW98w" &
            "hzkc476vN+fnmFl/MOVw7cS1fLRk8MbebvINycXchmti3/xLCEHEk8Cf7R36Nv+Kz9L5uXGaJ9TFhBNQM42x1nD/lraL8cQ2e1v4" &
            "2O3j+HMkdr415Gdero1zTXGMP+OQGz71n6ND2Be/53mnGZ2d3pQvuj7az3PruG8N53d/1864nps2DmvstHb9Zm8c21JjWvKzVsew" &
            "GZPumQ71qNvOeWTcKtdaxkS+Yxkfn/tujet7Lu+RDdZsuvabFyRXw+UoDwR+mR3RTNYV7zaCJu6CdkVPfm8M+lsex8TvTeZxWWNq" &
            "ngPWwPP0E8N6zvJRg3ltiz2NNVHHdXAO+J3XdLQmsauPHPMJ2nikdnsR8100ajZwDpjXtRDx72CdnMdODts62T+rfBowaU46rtE+" &
            "nns9HOd5ajma322apxZrWCu2iWM/81ywFye0fEHur1M/Mcyz0+CN3TW7kdNyEu7H1tqFD438Q0G/daJgis6ExnZ9/Hci7ae+HoDP" &
            "aXsqTBvTsN1ca3mj7VrToJ2/xprgWG4G+9wax/VzTNzgL168eOPXIreOPTl53GGqy418Y7JNjeOlvnmtncEc9nPexmpO8zWQN43L" &
            "aNo8392QWoxtU2txtNPfGv3kOW7iHbipWjs+xjedW2DdVge1+KtNw3yuS/q+VzIun+ZwbFnfts4NHgPRdIiJH1/a05foITGY4pOP" &
            "A5yS5iZmOPdTUfpIKzZvlNhtcx0Gx/Xw+IfdkrNNaDSp6wltF3+Lc9+85g+n5bjqj9OFy37mLq19RxU7fYxx/ozf+4KxrIPNa5b8" &
            "l/JOyjGMo59gjMfQ+AHt3gfxTfHcv5wTNtZiUD/HXZ6m4/y+pto7zrPmtaLPOchhbdShLm3sJyZHageuIzaDtna90h6fc+/aLeC1" &
            "whpyrdme5j0YP+fNujmPv+0T52KfGuTkYXnVg+9JOcSzPouKPzbyp5gWm8YJo3+HKYZ52U9jzNS8we1/bps0iPA4T+Q41jzHmN94" &
            "t/Djn+qwLX0eiWZbGzs3NTHlbKDPMc9tbf7OfMzldZvqM29qjuHR2oF9TTP2C77DCHZ1pW43avum5zW2/gTnnmKaLWh10c7msZy1" &
            "3RwEzsmj7Tx3Lmrb9m0bc6ddYuBmW+WCfQp4nIRwDNveSIZXv8ZUYDDVY26Ou8VyTTmSn/7uVVvTmriTvmtkzK4x3loe53N0qW17" &
            "fLZNbZd35yNnlT21tGen1mCfY6LZtHfz4ta0W9utP/OZRzS+NRufPudl33bm4fVB3gXvaHPNx0aua5q0PH7fD1pt5LPf8u7s/jjY" &
            "+sxtPcZQJ7mcm+fsW9twvDm0t3zku994T2N+gyXwVQHfttDOhfQNnucuOJhuDi6Y4GQSzJFjazsfmzcM43Yal/IRRWBubD6a02Ae" &
            "uU3fcJy12DgPbf6tQ/7Ud47WokfQP9V11ry2acznmJ09vvjbHiDXeamRa2LHbSDfa9Tim5b7sZ21M67HwM/4uX7mUZ/nRMvRzpt+" &
            "4pfm3drObTu1mo01xNfWyEc2187mPEG7vxJX/cKK5/HzHk8ch74DcUELf+IjzUIuoIExUzHNFrT60i7lc2Bz0w/Xk3xW/9LiMDdz" &
            "mG/sbK3edj7FcB7MnR7sE6LHZhtrcGyOieG59Wxv+myObXU4hr6AecjZ6bpvXpofAM7RYtj3+CecXTNube6Zv6HV1+LYv+IFp6/3" &
            "lrvpke/WxsG943ZLzrZmQXxtLDxPv+nb7npYK2Etx9keZA3aPXvpHyp6XATX0Gt6vV77O5AUFfABkr+LxUE3cfaXFmHKE1An8IR5" &
            "8i7lBurJnc6Ts+WxbbcxyWUdPKct/aYVnm1nOo3fYjmOKe7M3xr17TOHRyManBueO0di2KbaL9gr9DlXeMwx5SHHGm6Ob/xJj7ac" &
            "s28dx5Hr/DvbrbEB7wEt1uD1zrqX4tq4XDtjznjJPeVgbRzLKvucuVpzzKV8zOd6EpNz9ps2ebzntnPCetRZm/v3Gw+QVjh900Tm" &
            "z7znZ7ztHUu02N8V6xwBa8v5boNwM7QFTt85snmcj3HWok47N5d2f87a4pyP2NnepqWGM72lV/Ocz8b1uXnu22eNCS0udvrX8JA+" &
            "a87RGteTzbxwc2zfKzgfxzPZppxBrjH6rUGekRzUtj6v5dTCj6/Cd62uybaWLzkZk7hLeRg59ih/IDMczwE1XBfHwDHS7+ZaAuqy" &
            "HvPZty9oOrSzb/ukecmTjxPkG78HGeFsDiZynBObSw36WRNt1DWPYJ707aNO02SM4Q22Sp3U5VzwoiI3/Wa/4ue61jyG/zuZ8Uar" &
            "gevMdWUNgfW9Bta2PXB9l+HBxXPW6xqtFxvH5rVosK5hTddln2NpZ9xR5p6avPFa39zYPUbnc0ziwk1efgF+0Tv+lpM6hvcqzzk+" &
            "a+4+2eA4PQ5yqN0eZol9biMO3R89DnPX5mNV8qjhPufctVkrnKW58zwSnKv1yH3jP5QKnNgtweln4C0+mIpaw4b2Zpgm13nOwDge" &
            "c97sBPO7him29d3Is6/xrLl0UbLt5s1tgmNp28Xb3ri22bfjxOb94RjHE/aTY/uOY777bvETx7Be5qSZ67h2Q4i/vdpuzbEBb4rx" &
            "nTXCvrRW0yoPC98jrG9Yz/mmvOEantc18M6Qe+kq6x+ba5nqbDU1UHsN+yRgnote4H3z5Z7A4DaoabAucIKLa/23XeAJ5ja9CamH" &
            "fTf6GmJvcS3GPOchzEnLvJHT5vM52o5vnBbb+uTR32y3Nuu6NbtBn8frOPcnjcafdCYf63Bdzus+4/muovkZuzv/Nkfmc7sV1rU9" &
            "51PjHHoud/VMN15r3gLXmiPPV/mOiDh7qDa9xiMyFvOPA9+B8Kn+UP6JPoPY6G+bwDCHiMZFv5meQA1OHMfCRrjO1ojENx8RH4+t" &
            "xp3G2tTn+aN+O/cGPosjWr0tvqHp39LCt87kN2/CTifj9B4JHNtyNVvs9O9a41mDIK/tC3Psow7PW+zky7n3iv07mB+09SDP/rM5" &
            "yHlgrnnHcP343PcXc8ijzc18gv7gWj5mDqxtf2COec4XW+bl6W9hsZFMJIEnlf6drzUjGnlltMoDwWg6E3eC65raLQivxdxSV4s/" &
            "yisjotVpm+05vxWNO9mcs9mfNqHeXTZu0yN2c+p4N69J7Hzx4hg2wv0Gx/vGt+M2W7Ozz3m+e/wfKZeuK8dbo7X4qddiGqb1OvTn" &
            "OVwfeZ63tMkebWrFTljLNrYzeF81WLPxUstR9qv96VsvnN19JKD+tBaXy2X/EVbITtqK3/WDaXBuXrSmYZvt7NPvXFNzrO2GfY6z" &
            "P2i5aLfGzt58BNcxPPPTyPX6M3aKby3c3QV+Vld8LTc3d2v07Wpwy/jZmr/1Jzg+5zvfLf2mYd5F7+rNtZYx5aD+1KxrrZxnLX2k" &
            "vvOu8i7EOZ2PPtun2q3H+FVqvuq/x24xq9TEGpjPMH/SsG9Cu7/z4UFcWJhFp4KWFjRHJ0mfce234R5g4sJrNbhWapjvvmPXsHj2" &
            "L4xpimGsNRyTui7lgiaX9dvPmMbJOXMxJ3O4UZMc5uC5YwzHTY1cxqa1+Wjxrd7Gb2PmPNk2xU4x1vaYXBNjzGOO5k+f59a1jf1b" &
            "GnWt2cbb+taxj/yA9xLzPd6zcYXL2li767XNsC33CN90XW/gutr/vx60sSVu8luD575v0x6E7/v70/9ISCNvkMGhxfLPRo3wPZhL" &
            "uVl6cPQtbZwgdXqQ5CV/fK4n8GZp5wZzxc/jWUz69AVeJK/RKrmck2vT1qDZY6OfLbrOZc2m69yMp45bwPE0veY7g8dHvamF659+" &
            "LswDzx1v3zq55hhzDD8vn/ZIEF94HIfz0Z7mPluQOGu51oB+6sfnOPPSN8f2ad/EF3+rn3qOn1oD7V5r15I6uA7x8xjE76Ox8x+b" &
            "64j9nC/U8TRjFDbJRbsfWwpZWkwXwHP7OcE7JIaxse945pvnvuvzwjaQ77y0++hcjskcT42wXuMY9DPGa9o4u3jzCNp4YXmeHR+O" &
            "9Xf25ms5pr5hrbM4+g3aHNt07Z/a5M/8rsdxnP0aK1rRI27JxZw8p21qk27s1mn8NayXOTkSjdfs9ofjvtsa7qlEeNaLbWrmBdO9" &
            "nRxrWO/a/pTJlDw3dv5CK/20Y9hwgYvhZnJePkiaXmzcNPSRM+mSSzA+R+eytvO0c188jmNrc2MbtTOmnXYbk/2BfW7Os8qm3HGd" &
            "w7acX4Z/me2WHBOPPnNcS7OvYe+4WdvN8zCdM4f7k96Uu9kzH/7HgfYbh+oJruXFzTTnsbuuwDpc/52m4wj7HLOD41qMfZN/h+yv" &
            "6d7nY2IC53czeL1m/Vap9Sjzm5hv7JCQDRbK84WPs5hoacCtKNroI8hL82DcEpdjG88q9TVM2q01OMYXjrnhtHzU8zn70bCNrcH+" &
            "6Dh21xLHeGtMuQjzrNH4rTVfs93SnG/Sod9onF2sr7XANVnjzL7wYqO9+2BsMNlpS8t6tfV3802waUaHzRzGTphquQUTNzbqTeu2" &
            "Bn7AhwcxcXO0JnHgftC016AfO/3E9Wcfub15w2/n6fvcggGLsQ797PNoe87bZoxvasYU13xTjG3mr+GVQc53MO+Mv4NrPWsE57lp" &
            "Bdbw+vBGYg1ynMPHNPfdDNsazxrO1XjEZN/B+tPxDM7LWm6pa+KwDrczDteaXMbbb4759OfcezT2MzhmuqlOuCXedZjjseS8PUDa" &
            "NbQ0/4zhg4U26yZv03V9RrQuqwQEtDtZS7gg/BzN5veNaNcYl/Pgio/dqM2bFmMJ55k4q2wQwzrUSk1Nn/Cc3OpLn0c382l3Pzbi" &
            "0MOiIRptrK5n2mcep1uLo34D4223343cNI7ffMdMnMZlP7adr9kajpN5zfVie2Ldt33nI3Y21uBz69k2aTYbfTn3tW2/tehr52dH" &
            "5ozdY2rjJ/zwWKrXes7vcyP2N/4WVoxMnmRuTJJ+vgtxHHVpawVSr/nDafrxkbP09KXuZXOjy9H8qaZV5s01+nhLo9auTbXdojXZ" &
            "rUXers9ju/iafs53c5O5bM1cxxnMn/NWK89brmZjDHnsh9fOyW1x9DV/bJO/tYnbbG5tHG0OokeOfe37LvcT1/6W19nYCWoZuxjm" &
            "aTzbHZNj4zfQl3jOFcdqPvvmuhHu+9pYqYVJc5xE6c/5Ra/kr+VnhuTu9Nlv/qA9EFJH4NiW/wzmZWyOTd8+5nJe9z2epmHE7nFN" &
            "Mc0Wu8E5NpKz5W3+2FqeIDHWa9pT8wXSzl1XO2c+5mz5yclxmhvyGbOzUc+63DP2Md9Z7nbe4pt+463hhhNc9BBp57t8k52NOrtY" &
            "1uJ41mMkdpp355g41mu5Fu49Oy1r+hq23/lyvquBeKznz0RcYIKu+rUVN27Q4sJpzWi2pUGnFjZqejNQg5NOHOUfLnqibIsG9ViH" &
            "fWdo3NTMFg7nNj7rND2ek8uxuxZy3Vp9U9vpJfZMK7VGh3PiHJMG+Zm39GNzHYT3XJr3B9vl8T9ds5/nyZccBP2MNTe8XU3UYU5y" &
            "qB94TtxabTu4Ntozx9MaBNZg/lbXVKP7k20pZ5vLVkPgfsbqa89j9jh5JBJvH+Nz7vVcpT7qTb5LFqshg8uDI/9xVB4iHtid/s8A" &
            "6vDB47hpkaNx4CbPh1ga+Y4LnLPV6DE12yqL537bTMEx3KTcjLM49xmXftMNms+xzjG1BnNaO4PHvTZ1xdd0zZ14a+DG7qMb672U" &
            "V91noEYbY/zUd/yukWf9na/VFC5hnRxbjLktl0HOpOl8qzygV6nd96qm0+xTbtuX7i/ME5/h/NY/ylot5WE8dcKjLfamG9/TqjQn" &
            "zwPfwPlguOhzOceu4absvLZPcM2u3ZxmvxXWod3+9q+UzZnarbxDN6jkbPl2aHnt4+bhBe16do2g/Tk6jG8wp8Uau5gp1jb3V3mF" &
            "Z61czLe0IP1oUpv+XbsVjOE4PJ6zFr410/fRjbDPWs7lOKNxGtd925zz0MeL1rUe/V7T6UHCc8baljbpsDVwThve+FMmrTAifT88" &
            "+EDIQNoFlNim6dxreCK2us787Ds/kdjUTZs5U9tdYIR98dt2Fuu+7WeY4myL3Rdpa5k7xp21W7Djxt40W0w403o17tTCeRs0HdsW" &
            "rrmJZw1zdvC1vIbYpjvNH2N9D6CtxcfW7i3WJjI/bg2xk0PuZJv4q7xrWYXjvGntXhleu2fdUof1nbvF0r/jZF2e/px7jHQSFLR/" &
            "t8huZyDH/EMPJ/q9aNaxlsdgXba2sOT6OMVahzz27XMj2GeOlivHWxsx2QnH38L/88Rz6jDHsY1jNI6vozXwiCmfr7GcE+l7LzLO" &
            "Ma6x8R0THMO+d9xl8+qY1561nIt2+78LWH/K5f5kO4PHeZRrd5U1Is7u29O8sm9fg/1X/kt0FhFQ9KL/F7kV6gSE/eZe9epqt+Ec" &
            "12pf0onfbenVkJvrnRo1mJ8+2p2jNSL9xjF30pzsO19D7NNecdwtPvdbY43mn+G5vHBbXONMaLW2FuzmtGnlfDrm3HM3+azR9MzJ" &
            "tevGmInTGuNcd/LxGDSO4fuMtQPXYh/PzZ00l2pvnPjj4zjsI9p4WYvrahptroPYzXnjHYgDcsyNjg+QVTZOwElq54xnbmpR8ygP" &
            "rDXkMVKjbY3PsTp/Q/it5vhz9PlkI2wj3zb2W1ya642fvOfA83iUX/+cgevI2twW9No4sq679b2lnuA53DO41jNwvBnrpXyRHHh+" &
            "mIc21tDOm635Y6e+eQT7Z+scbs651m19w7c9NoLaOzD3BNfc2tsgNVPD47bd58zteWk+6hnxt3bTA+QYvhSKjZPs70X8hfsqF3p0" &
            "WChxffwHiOw3tNglPvOtskmJ2M/AvM7F+bI+x0+/+azNemy+qbq1eOc3nzGpmTA3fj+IWyP/eKzf68N+9lLirc+c1CYcZ7SYIL4z" &
            "jv3OwxoMz01+tBI4juM+A+faGrRHz7nIafal/+aBc+EYzlFyug7noK/VRlgr80Nt5mj5drncJ6jV5sBjb2jzQVA359wn3oNtX8Zm" &
            "7fZ9WMCcaz3+CquJB9xMvPCOx1/+LG2axlvYJDmnvdlYtDWsvcOOQx035nHO1qJn3ecicxS0PLSzT7trXoN2uziC+G3LcZf/uWCt" &
            "jPc63Arr7MDamYMah/7lM+uyBrVcw64fPY95Gjfzr/IRhP2Bc8T2XOzq9TljaON5q6fVNfHcvFbtj0ZSKzqt5uc25ub80Jd62jzl" &
            "xh57OPER1KXGGRLn+3/s7jftp3+JvkphsVEg/CR90L8PWWXRHUu7/bRx0tJfw6ajtm0596JSn7E8Z6x9hHNNNbQ4cnhssE7G0HK2" &
            "cZrDY5o1rRHER461pmaedRvPvp3dWjnnkXBMG7/PzWXM1JbWhZqsg3z32/ktjTlpo1YbS2rLNc9XplNzrvAJ2ukzj9jlYGv8xDgv" &
            "14PNcW085uz6zpkjQa4fIvS1GOcwZ+o7ttnX8Hy4cGJI4kZhYOwummiJCMY2Dfe96IHPqXWURU/zmKnzNrVTJ/rUZGzAPLFzzgnO" &
            "N5HNZ7ttrtP2aS5aHG2xs45p7AR5bg2xT/pn9TGOte5yGo3rXGlTjfS1ltpWuQ553UWvabW93drki73lCKaYW3i3wrHOk/M13B9a" &
            "vqZHO+eu8cLl0WgagW30cY3Tn3Iw3udu5lHfvqDZ2n1p5R2IE67NBn4bWJv52gYldvZWd/MxTxa4XWgTPNlneQnPmf1TfuegDu2M" &
            "83mrJ2C8tRzLfmuM/S4RzVxMuxz2uy7XfGvjHiFck+PcwvHNpcWfoXF2N4amnXNf1y1mauTtYqg7wdeJYa2m6xzmm3sM9x/z29H3" &
            "R4MazEM/76vWYC7XRzu5Pm/9NdxLWp05ur43+DT46Wlwc1K0xbSCYs/xMny268mMrQ2kDZh+1sW+W4uhnWP3Z6mx82i0MQVTHttv" &
            "0XD9rqdp3DIOa068wNypGdY1f2qO8fE5rb3YiFbAvuPNnTiNb9uOF2RNvbaObTrX8nA2L+ft4fccn6/VtKV3seS0Y4NzthZ4Xd1W" &
            "uQ6nFm67PyXe+4m1kJcjueY4N88N2rnOR/k1W9P0uDi2pzot4BY4OAPMT3tfvHjxjRtrWyCDdk8+4QEY1nbeW/K3o1tb2KDpr80F" &
            "3uB8re42F+ZPsUvxzd9swU73bdC0XH/jTLiFZ1023+zcmo5twW6NGmifeO1iJuyLhrWajjm0TbUfJ3OV81s4tr0NrGU92l3Twrw0" &
            "PnX8IOA8ek6nXAZ9bR+ak3PvhSlHs9+yByYcx+M7kIVBpyD2488X5vzV1dLfx/INloONL40DSizBwYTLtt7iIm35ozEdExfNh4eH" &
            "N+JZ9xWbL/PI+XS9wVQ3c+TIWsKZwFiu6UW/AskLAf47H9bT6m9zmUae62MdbI1LvdR6FheeuYZ1GsfrFzifY61LDuc+XNea1sZB" &
            "LeZyjQHz+PyCL8hpD283fu6rFt9yJd8q10e0iGhMfcL5mJO+wLnNTz1tbMFkyzGxHlfgPOZT+6q/Pcg8btZnjtjd57gDcthP6y+l" &
            "y0K1fgbk3+fvYia/uUZib2mOYb8d2wVK++T3OY/0N43GoT3gAk4Xwq1IHW2jGJPv1vzO0c5bOwM3c7CLd94G12CeNewPpniCPvNd" &
            "A5vns8Ex1ifP5+a75cbfNN2/1d4aHzA7/mTP/ua1MumQG78x2ZoG1yhxbd4Yl3P+RLzlXEXDjbG0p08dcgI+GMg1L7her2/+jDew" &
            "jQXSnwcIkzrhru9mNM3WDNrNpd1+boC2GcyfmnnTwtKWozdQ4/qcfINzOG32YKfTeGf8Vp/7Z/YzZFysyc18HnPeuPGZs2uOYSw1" &
            "7WvNe5DX33OaY1iHfeaQ1+o5i2u2Fkdezm0nnnOtthjnaH1fJ7fyDMfzGmz/tqhp+Zxa1mczv9lWeWE2cWML3vgVFuF+cOCdR951" &
            "xP42cHHUaX2iPSnbYlqnYarB/gbW4ZpYS87ja5quY+Kt8oqhcQ+9VaU9R2tTJ/O5hgvQfPsM10E4ljUazsVad7WER7vnkdwcW5zh" &
            "+FXqCs9c57Kd/antxm599n1kXGzRzlh8jTGWtozT421gXPZc4H2eo3P5BS155hvkTHNJn2PcGsyhHuMYT9vOP/UJa7A/zW/TO7IX" &
            "aKSAA9lvDw8WsdssO19DCm25Dnw2Tl+rP7DdHMeaT3Cs3rDcFNMFR8QezTRfENP8tTF5LIR1HO+4XND0mdPyGOS4hmB3QTWbMdVl" &
            "fvres85hLesY0TKXfeeM/wy8Fngz9/qssienxtgW5/N2ZN5VxkffBM4bj56n+Jpm0+AYJ/h65Zhi49yvMje+xunPfHhO3JqdODbf" &
            "l8TfcNW/K3Gt5C3NGzmu7RKjYRuDjNh2nFWS2240juPZ98LTv9Pzheg81KDO5CfPeZjPse5n0e1v3Mmfc2+EwBqEczgX0fKyP7XG" &
            "c3/yPQfOxXNr054LNfZ2Mwscy/Od7Ti5+ZgbvmGOtXi0f6fXsNNqsBbjWiPH12e7drhOzBMdHg3mcW7amdux5rJNMI/8FrvrO26q" &
            "gzHec21vM75pfnPXnKAlWZvJYDNv2gyBn4ZNJxvfdttaHuZzjsZtzTmnTWhb8x/lYqGdccSZplvTMCZ/szfbesb8Tm2KuwW7fcpj" &
            "zm/Rtt913aIRtBjHu087/WncM97vjDHMmWzGczmuY2p+Rd2uiTa+yd5qYZ9ofvOm/lHy20/bGXeC/S22xZ/5Deq2Wle+A3nq6ImU" &
            "88vjTfo6/Ew3/Pv7+3V/f//0aiBtwhW/4srf0aJvleJjM8/2xo0GtWynLfV5HNPYHB+EG834w6GNPmqER5955jdb0zU4tvDHDXTD" &
            "K1fXau1w3V+buW4w1/1Jl9rOz5rNa3D8lGeHzHXAeUnLz4Cv5S/2EtFx7uh4/Vz/Kh+VTZpBdKnF8fv+EX/Qxk9bxj35A8+7Y47H" &
            "eXzx4sU3xh1uG0vQ+Iyb6rJW2xu+3timX2x5rM3nPPQZjokebU8r6UEuCbgRiW0ahCeOtgPfZ7A13i029s8wcajVdM/izpr5OSfM" &
            "tX3yN9zC+651m6/Zvi1a3czTcjbeBHI9N7a5ltas+V2i6bZz522cCRwHbzBu5hJn/oap9tjcmt8PTaLpWy/jzf3KHD9sW3OM2+5+" &
            "ewbyk8sPG8K5m2/C06+wliaGhbP/8PCwjvJkbQV4II2Tvl+VWJ+wljXJ49ELMcURZxzm+DZt0mCeXbuFR45hbuMYjRPbmYZ9jmtt" &
            "wlms95a59k1ofMc0/amZfyumHIZ9O96uT1iPtl0zJh/7vg89Ny85Qfq5v0TfsYzjfdAaTbc9PNgPn3Be36vIafygxU330tTFenMe" &
            "f4sjvvEo9qLx6MWcEpBHkE9da0yTEx+PtNvW4LqmPOYRqXlqE6a4hbf1zkueN2FA3TOEEz7zuSbaebR9d06b9Vuu58Lx1nazjxfL" &
            "rbBm0z9D49M2gX5fi4Q1nc/nrsU8+qzp+KnxRkUwfm3uHwtc3+SmNnFaHYHt/MfS8VPDfIN1ENMYjeTKeXTO4snl2Jud5xf9pYSG" &
            "+OrfwiLBAS6CMV54a8TvgbcarB0/j4yZMPnO4oKp1rWp9xbdpQVjv+mEZ99UD2G7/VwXj9dcYsdlzqZhrm0N1myNvJyfwRrWm9C4" &
            "ZzHrJI7H1oyJ33y0tfO2D831kTjjT22KMWy3jhsfEvbFljE38L60AzUn2B9tX0OE63Wt1ozNY/S542wzr9WYe8XTR1h5dcBXCbFb" &
            "cCGJn8LtYeCkU0GNb84OO/0G1si8hMfp8ZJDe7j0m0s0v9dj6cu1ac0IzkXzN1sw2QPWSlsDuW638o4y5ikm9h0c47lc5Qtkc1o+" &
            "29yI5rPt2FyH4fvcXOvZzvjmdzPfyPUcTo5pHk+zE+a11jS9Vk3rUq7T9HO0RrvPMJY2t8mfOtbwJXr4t8LxrTVuMN3LeZ+9HLo5" &
            "XvDP6xPA44FffjRxFkDwbaALDZptyd4WzrB/0j1D6vT8TIvLuGyGNo8NmRfmajmXbmr8A4jW9wZoYA5yzHf/OdjFeq2eC9d9hrZ/" &
            "mgbXYGrmG812CxLnXNTzGCYe+7YTO1+bHyK18MbieWYN3tscZ8vDWPrbuTnN72NrHLPH4jiPk+Oz5nSt7eakYbKf1TrFJfd61Gj3" &
            "d8df/Q7EzoW3TeSEl4eCxZ3INg9y2nRE01xFa2ly45tiCXI8FjfnjT0bgBpTDda03Rup6dnXsKvV/qbTajv0SxHXEP02BvOoZ54b" &
            "kb7XsY01zTURU55V9qjndG3iXcNZS0zTCDIO7jdzrec2zQP3ArWbFnlsjZPzrDN1Wx1L4yTCp2/SZA1BqyF2w1rmTnb6Ym/7xjb7" &
            "WWfgmOSZ5p+8wHkaOEa+AQguHJQL4sKkML47+TatIflTS3i0E/E3bWsQ1OEEBdZzzcxLNM4EazR+G+/Ec62tbsJ29xsmzmQPXIvr" &
            "S/NNYgfnPBtra/HxyJizvm2xf1vcqkFeG9funNew45uWQY7vB+uGd97ms+//WiAcfpFt7HIZvs+1GN4XeO66yff9y/ctx7Pmprf0" &
            "URY1z+AcE6w3xTl3OE9fonPQuwEf5dWGRXeNT0jCvDXU1GLeBm+jSXvOzXffoN889yc0XnRbXT5vcfS1uFtg/m5+yaNt1yYcw88e" &
            "VxlL05u0n2snvM998bVrgNj5VlmrnHt8Pp9iboVjdy8oHTc1zhX7nkNrEtZsNq+B+bHTH/Ce2O595uW8PfTa+Jxv+gjJ98NdPeSG" &
            "7+OEqR76Lhf9McUGD4CiPJ+4rRn2H+UVkmG+0ezuG4yZuPSTw41uf9OyjvtnYFzTaHAO8hxrX+tPOY9yQ28xzX8LGs/5XZsvVse0" &
            "OHPsm0Cdxj82r2oZw1jz6XdcMHEd47gGcsy3jrVbc/zCNeSxHuWFp/eX4Ry00edc7Zxwn3BNTWMXH1DneuMnMZ6fwNzpYZI8bOtR" &
            "u8Xk/OlPmTAozl0jJ09ZFhG4KGuEQ66fytQjXEfQYp3DfsJ+82zf1Ztzc+ibOOZP48wrD/oI58w5+7tmLvs8Ejtfq7PZGpre2uRr" &
            "NdPmvnk5b5hi6WM/x2kdj5O9RJhDfcfQ56NtRGqc+MdmLDxnv93kgrM9YC3b7aOduZ3/ql+M7XTCv+IfVVsvnIXvkGn3nNEeMJf/" &
            "DQo5btP8Os7Y2ThG8q75HwktzgAO+Kw1xO6j8xgueuJNeYNr2Ry7tgP94XvBnpOLOduNI77Ac7iQp83DgYdL+tFseXxsccbb6E37" &
            "JWu1a8FkI9wnpnjb6Nv1A8dTc4pZJXfj8toJpvNV9kmOLddRbrCTtvPE1ji7fLT5PmJ/cODVdgNz5OjcPBK0kcdrvO3bqU43gy+8" &
            "A49/sgXWbzkZ086tuTbXqLl9FQZwkfmZWJCinTj9K/4YozfrhDbgVQa4m4yliXXuFjMtGmMcN+kln2/mOZo/IVzzPQ/JRb5jDNdz" &
            "a9wEP1gD741vkyNwPDXtI+gzz33iFu2Fsba1cFvS9TwFrWZqrBuvDdp4TbZaHUN4nXfn1Gva1reNNTa/sfMtxPu6bLV6XtaNY5/W" &
            "kfdSIvrxMZ/hWlkXOebbbt+urgNzccl3IBxMG1jOmYBxEb6WvwxqPQ7Ag6Eu7eRzkxscpLUNboiAY47PcxGfMdlaPT5vscZRNnHm" &
            "m2twFmP/c0AdxjZbm7eg8V3r28D1uZ4dt6HZY0ucb2bc7+G23G0Pt/3n+prtVrQ6WvOecTPsb5yA+rbbn5Zr3nxqsN+0zSU8L0S4" &
            "5rA+527nxG6drRcO0eaPsL7hGpfu07l/Ryccrsslv8h9Un3EbqK4iEzciiCngZxoe5PE7w1lxDYdCWrs9JzPi7KLO7MFnKtb0LSy" &
            "4NZxn5jmwNpE4zfY7zqmWNvdJ5rv1vqW4s+4RLQdk773CI8Tzmo5Ni+a2tyaE5g7IfHOR+2mlTpz3mJjm2oMzHVr3J2/cW/Bjtd0" &
            "Wh3mEGe++HOPbfysxXQvabapNmo4ztwj+5LGFuS2+yNb5DX7OslBhOf8BG32NVjD2tYzt9l3ttbPMYs1LXzgumI7g/PQbi03o9nW" &
            "iX2nR+z8jmf/LIftjFtl/qc26cVnrnk7TLxpbC0/ERsfOBy39SY4Z/rTnNBP2/TCj3PlWsNr57u+sbOlTqPZAvqY+wyek1tj2/oe" &
            "5V26tc5yuA43cyfEd0lB3hzElMTnBzaEN0W4Uw7CHMZb06+UHMM4T7559jeNFus++QT7vvGYY+1bbe4TrO85jfE+z5E3BXOJNu6m" &
            "S7AO1+YajVt5z8Wx+Z5it4/OanjbOGMXY/s0Dmp4zc6QWF6f1iPX94yGa3klbq5127lBzZzvmmOtMcFcxrT5jc18xzZbA/2e76a1" &
            "Q2q7XMrHRmuY8KlQJr/oTwE3HU/UrfAgm89ITY51zfYz3jDnu4Brst31uQaf31pf04p917fNftcZ/zH8zt8c22LnkTD3VjxnL7aa" &
            "znzuG2f+NWjv4sgPz3z3Dcfemr/xbAs8987pY2v2+SHEc/ft23Em7jqZixx9fqY5YRd36EWNc9LuNqH58/C4Xq8/+5foFnNzcAP5" &
            "frK537TJ5dPRcLx1buWl75sZ4T7HPs3Dgs+czPUa8j0HbVzHjXPNeMN29xuYfwfXw424y7PzGbdyWfMtzbB/ekX3XYPaz81xa13k" &
            "MJfH1rR4PblvvvuxWd+8xmmgz/lb3MSJnW0COe16d39C87sf8H7j+W5vEKizq8c8+695gITQ/iwByQ8PD0//53lg0aUbJ7X8cHCe" &
            "5Of/+ZsJ8MSkMVf8PHcOY+czuEC0tf7O3mo0XHs4bW4zn5zX+Hkkwmtjon9C4lwf/Us1uF7G7H5N9ja4RefMv7AnDz3wOGfez23s" &
            "O5z5CWqyEa7RfqKt/Srzd5SHZOD5oJ1gXHQC1zyBGh6X+0EbB/sej3NMus8Fc+00b/EvzFnuy+3flezyTTaP3/YDnzSN/6FUAxeZ" &
            "hfrvpBhJyM3ni452cptmq9Ua9q9y851w1U2ec8Q+eZOeFzRodbaaGzxu21rfcE3ur7Jp7GsXYs6DpuubR8PO3zQn7HRWGaNbOATX" &
            "9Bb+26BpUN+5Gp+Y/JM9YC7aDNt4j+D1kmuG/N16tjH7uqOdNucJyJvgWOtN/obm45jtI3a+pXn2rzF3sTufsRvnG/8OhKRmj+/Q" &
            "q7H824/cGPwAYFzOjWiSx/yMca3Tw6g1a7iWlt8aU2xsbH5ILOVu/pzT7pzOzfWY0GImMF9rXuNprohmW4+x1tnV9lywHuuzHvto" &
            "I9fjcxx95DbbBPvdjy1Hz9cxXBfm+Nh4u2s5fduzpux7Xg3ndX1Lca6LvjYW63PdvMZey7Tn5KRWG2+D49lca1tf1hbYb16zxU4w" &
            "d86f/haW4WThNX7j0bdKch7JbbYcqW3eKrV54ex3Po+RN8kDm8ebaCFX3o3Rn3i2afETZxt9hMcYOI66Dba735C8ztHmtMFxtrXm" &
            "B1eLm3DdfOS2btRY4Bm2U+9Me2d3XNPjnExawZl/bfK2fdv45liroe0b57f9TL9dGw2J5Z6mr+W27xjmhzbGebzWt84q47FmwPtC" &
            "zhlrHYNaE675W1hMtErhLsywrcXRt0rOC/4R0oTmi635VpngVttVH1kZjjOXC+RxEWe1Bq7Nvl08azCmONvdN+g/G9M0Z+7fAutM" &
            "88ScjHHOyR7bUV4s5Dz7ldwG5mg81+e+ua4ndtZFf+M+x054X03c1NNupkazBdahLefmE643aPUwR66hxgumGm6J494xrDvpEMzn" &
            "63+ag7W5V7iG8NxWPsJawx/2YvH2xU/E7wLisz/njb+GGHMYP+GM03zNllo8D2sY6y1zRsTnPJ6DnLvR1zgNLW7C5D+zTznOalvD" &
            "GBq/2QjGtpwtz5l/4i752rnjJk5r4TQu9XzuGD9sbsG0p91sn7i0mUM0fhrfle54O19sxsQ/yoPA1yvtbmvQjD1jol7sjZ/zM7Qa" &
            "Gmz3mIL5UfiICDmpmzmOd0FtMsixDtG4jW/tXSM/iCa1pzxXvPJoHGLKvYubfK55ietcjU/sfGtYzzb/5N2St8H89L0ujedmu/kN" &
            "h77vmuKNM3/QtJyj5XVMYO7EW9KwruPa+gbmWsP1TM1c95vPHD9ICPLbOxrrTiCfWi3Oc7a7LicbYT/zm3cLpnWl/Uzr6f9E9yBy" &
            "bngSsmh8Yvov9VLHA2c+cnmTaIN0nNG0d2g1tNhmW/oIrmk1H+30t/xHeXvaeG8La7jf0DgeY+MQHjNt7nP8b7MnCPNu6Z/pn/kn" &
            "THHNtoNrbPFTLmPSsr3Z7I/NN3i38HjM+cSZmv3usxaDtuw5X3eM3z1E1rBXV/mOdYJ5O34bK23r5J7a4HzsP/0pEzrSnIQXL2M8" &
            "uAlTngZzk9sx7abStKlnTBO6yvhiMzgnS3WynrM2xVKXcH+yvS3avLjexvnzRNbL+3DC5LP9TGuyf1fgnPpIcL6nuW9xDbfwXIuP" &
            "zwHHmH6OvqGbu274rjQx4dGWc8I50jePoG9375vsAf3mZV39oMueN39COLdwn4vjOH72Edbl8R+FtD9BwoIZaNjvlolwDNvSzTw1" &
            "eQJdk3nJ57xX/Upqyhv/xOPvrdsFHO4S3zniXyVPbJMvsJ459sd2DA9+1uhaEjNdnAvxztu0XGt85CyNgTWw/h0y3sazzfldh8dF" &
            "bs5XufDJm+yuZWHsu3XZoY1jFT3XNuVovLPG649rQb+v10Nzznq5/9oYfD+IP3GHXpDe3d29oUG4VvsNc7lXPWcT/8AYWj7PI+0E" &
            "NW1jn/Nrv+E6L1wYFxC0RWy+iLOZ675tE6Jl3ZzfouPYCeS0sVOHmzKY8rQaj/JOL/aGyb5OfMTEi501NW6r1+Ac8Nj0guZrtnWD" &
            "VsMtdX9bJMdub0/25+KW8bc1sG93foasw1nMLTzWyJut4euxYfI1PeOWWoPn1nGLJkG+a7K2wb24Sjzn274zHHk4xrDb+FwwFjNx" &
            "3LedR3LP4Jy3Djyc9kqAHB7XMAZrJYb1k8M8rNf5E09b+uabY5v9bW6bjWh5jameVis5zh1+exjfiikm9pYz6/PnCe531/Bd4Jb6" &
            "G2eyNfuu7rbWbS/73M32CbtaAs71pGe763E7g/O1OPcJ1xJ4Lu3zfDgH/eFzTxqsvfEdU/8hIQP9v1OtzSTlX6SnOaGTE60Ow3rO" &
            "H1vr+0h/85l3vfHz9qVx+m3r1IiW5xZO60+YtB2/q5NonKnGBnPcv2XvuNYpv332Pwe72FZzsxnU3OkHbczOczzjBc3U34FcPpgZ" &
            "Z83JnvtOG8PS/ckcYqq72dYwH61eIvnbi1O3gP1b7mdnNQTmRZs5bOMcTvptPZ7G7eA2CHLou+APHx5Y+PbQWZvJaLYdPAlH2bTO" &
            "HfuE1OYxNv8qY2tgzNu+2mXOqbZgqs2bZKdB3MozHMf89rue6XzCLZxV1o92Hp+DXYznu+V+Dhzr/g6eU9ZinVv6u9g030gbz5zG" &
            "I9/+oO33wLE+Grs8DdZOy7vpHZ6bh0fCe422Nje2ER6Pkfjr472+jpKBPG+Jr3hlPr3CMWJ3i8/gBAWOPcpDhLE7uIZmp5+L4wVp" &
            "/AMPtPDNa7knUNc2nxOT/TmY6g0me8C1nLiT/QyszW2C1+67gvPm/NYcjj/DxJ3moNmIKc64hbfzx87jUR4u5hu3XOeBcxk7+9pc" &
            "9zxv96wG5+KYm8+cprFD085xNx6+uwr34eHhz/4dSAtqdvqdkO9IplfczXYLWAvPb31o7eBxENHzKwpvIIP1NOxiGyadHXY5zuo7" &
            "hndxO1jL8bdeVG+DM82df+f7LnA21w2eux1u1W28ZjN29R/D9eb6vfYtLn3eP9waou2cU93m7XArd8dz/obd+HZg3C3xnkuet3ta" &
            "/L7PBt/4DsSk6+OTJiLm5sifnLGQFuOWGH7OdugfjrXvVMLjuWt3a0icN22btOjQbi75wXEcTx/trWG8hGslr+XmOTkH/p8Xwzlj" &
            "o915CecLn/PT5t71meM+ubm5pO1gHdq4n1ptQZtPc1sjx8e0jMF818NxWn+VC5/a8ROOdx2tPs81x7HKXNsfm2EObczvXOQx/lp+" &
            "ihot51llv+7inPeW2qhl2yrzZu4UR1CD+Y1mW5t4rjnrC47j8d+BNKToLCI3UgppokY4PLJQngfTQCd4ohN/iw75OxtrpM9z4cWg" &
            "3Zq3wFptvtx/G3j+njuHPD9rhn3m2+/YCdabfH8eaDkb2nreilv0ibfJ431njWZrYK1T3bE7X/RznOaWvNZazBm8h7xvplp2OON7" &
            "HBNYAx9mse1qsy41iNz7yQuu/h8JCdr9KoS/suKX5owjaOOC+pt91+B+Q8u3Ngvb8nni7Q+u5bsL+3NMm2BO41rPr5rPcjTcUntg" &
            "DmEfdbnhngvPf/R2mq6FsB4vkl0cYY3WjMl+y9q1OOs1znNBjamWHdoYpjlx3zhOPjJNrqPcLM3j+VRjjjw3xz6ObcrfbA3k8dpz" &
            "ze4ndldTa2cgj/kmnXC+8RHWhPAOfeTggRqT3/3Axbpw99dGy2gca8XmOhg71dbA2ppW6xM73zQvU4zHtMqmZWPcFE/OMbyKmUC9" &
            "1v68QH3ndGNMi7fNPvO+azifER/Xc9ofE5LDa+tz15Lz4+ThcAuoPY23ccy1/ZaY2NLyYoa2XczUJzI/t8yTr0+Dde7qI3zd+17Q" &
            "NK75/0DsZHF59ceCWFg7P8M0AR6stei3b0mXA2+ghieLHKPV5pp2cQ07H9HG7775PGfz5qL91nUMWAttZ82xU06+yzXferfiLKb5" &
            "Y7tljhi/4xkt72Rn375vC14LHMctY1833AjbdebxsNG/006NPLZam67P028a7k+2VdZu17dG4xq+bxGe57TL8L0a/eSQG1wu+Jfo" &
            "drrP5Gz0ecPx3NpBi2E/2tMNz3Acec3W4EnPkTHTooV3y+Z1bYZ5E7fVMc0rbamTNwXn8HGHzEmrp8HaZznOeJN9aT52vDYHtDu2" &
            "2dZmfyxpuZFzC1rsc8C41DvV3cDcb1vDGmI9h95frc6ms07st87fjjfZbwXraFr0+QHeYjx3vr5XWbtdI1jDNx8xABfMBdGWV4j5" &
            "jN6JfaQGYb+Lb/3ma/knbW5G15N+i29gXucn5zmwVsC6yfMaGdYxrGtMtUz8CdZIv2lNr4YarBu0cTnXc9bJfvcb2twF8Z1xboE1" &
            "bo2bwLljI5iDfuc2b8K0TtfyveutSG7XdIZdnda6ZQ3PfNZwjDnkLa2T+e5b22vcxp6Yy+Xysy/ReeMPEsyHAx8SfOW68DNbxi48" &
            "+WIPngrQq1/W4AfS26JNgrVvyWG/+7QlZ9M/ytgZa35gLjmZc8b43PNMLfpY+8JFy3W1RjDZA9uZ03mpxVp2rcW5HnLTNzJmx9LP" &
            "82t54dRAv+tj43qQ6zUkpv0UH/ueA+vFz2uemPhcgwZzwms2ImNv/84sMbHRT32vGe0TL+B8cKy8L0YjfSK1UN95qBuf54L1Nt8q" &
            "c0jN+MxxnOv0Hgj36VdYMdIZRICbiQnCOfT2qiV2vsvmt8apgwMnl3Y2a/B84gWuIf2JfwuY0xcyOUFytlpsa2Oxzf4J4Z3xWYfr" &
            "CZrd+q3Gljv5mibRYoO38U3258J1c328VrRNc7LzrZJvst2Kdk23e8EuR6vT2HHiy0PEXM9ZbPQFbe+6duvvMM0B+5OPttTpFwlt" &
            "bOGvUrvBeTCaLXBt7fwbnwuwUBI9Sdk8fEC0YjhJUxFBuNOEBbdOGB9OBPWplXOOc5qPoNlafTudyR5Mes8Fx8U1odZzdK21w9kY" &
            "l9Zlx2ugfms7kHPGneA49wPXSTtt05w6LrY13KR8nDjGNB/WIqax0d/s69F3HX704lp2OoG5Z3zC+ZrPyHwwl+cniJ1cH/0QIdhv" &
            "/gZyzuryeeA81/YzXg4uOMor5yteidzf37/xioTgADnBHviueHONyTfpnOkFu4UmPDdGizff/YbdeBp2PsO89J0z/Wu5gUxoY3a+" &
            "huTY5eIcWN853Let1XkLGHdLrPmOyXVCruE487hOhrk7XK/XdX9//7QGyZuPiYI2pinPZCc8/mP4F/rx+/7Txn0LnLfpeIwez9n9" &
            "IOBHbYHnuemvZ8yhx+GjkfyEayDnjb+FRUIQnwfEwq7lu5IWQ1vDVOTEX4PPkzTlbDaCY2xjOPAR3FmOaJxh0lk31EtM3LZBngvH" &
            "T7kIz92Enc+YuLQnH/PT3mLeBrt45/b5BNfqmm9B+LdcSzu0NT9rE/9WhMuj7wu5/nIP2u1v6iR2qulMg+DD9NZxciysiw+VW3QC" &
            "xqZv/3PGZLR6rvl3ICEYHFhLbkHaeZ7m/2Zy4YZGfU9sbAHztrqCVhvhHByv7cnD8WTBPCZyiDZGj51o9ew0b4VzswYfndfjsp92" &
            "c5dy+3P0+M37Nmgarult4TnheL+LnNP8NdjuNdr1V7m+eN72eObVOoHrIRzjPjH5Mga2dcO+Mb/p02/75OfNO7nJcU7XeOiBNtUf" &
            "MIfrcm1rWI9mC5oGkfov7CwNxGRe8Cw2MYk34m8PkIWBUHvS8wR5IlszPF7yHEMu0WLcz3mDdTlux09aiW/1TbbnxizkZX6OmTUm" &
            "x1Rrg32emzXkJjyXTWMHzovPd811GIeup+fWtcq6B6xzlXlxHP3m3grHu4Yr7hPGrrY2J8y1Hjmt7thc29TIyXlD7KzNcakp9yu/" &
            "ICI//Rx5rbdcbU6WfvG6Sk1+ELGeSTOwluszLgvEW5MQTJDCWbQ3kwtiThZJ+7QZWbcnauFibYM3l4i96Wec+SVItD2u2KjJhbc2" &
            "0bTW5gIKGJcNHfs0PznaR3/0qO862rgC2jiGi17ZTnXE186JlnPXDM/HOtFxTNA41vMYwuENyNqMabaAY3PNrIvcdpxs2VuuwXPg" &
            "MeTcujlPzMRZRYet3X/oD6wZm+2Ov5b7GXNSw+PIeer0vieH9Scf9wXjHMsWG/1cO89TA8fBe0Dw9AAxGLgrapVfDJjrpLFPfk4K" &
            "bZywXYy5q+TJuTmE+7fAdRG0u55dvfadYcofNK3Ymm+VOd2Nk3gOx+N0jqk2Y8ppu/tnMN/1rXLdBOnH5jjaHWvOma2h8Whrc8ta" &
            "nlvXKpq7PjUmvYZWG/eRj47ZIX6+GAu4zj46V45uDW2O2hgawmVtzjflXeU+vgqf/evuOxD3jfYETT9PWLYURe6l/MMgttjaAgZ+" &
            "KnKypwlnjh34joFa06JSz9ruf1u4/qbfbMY0Rw2e213s23ADrs8tYwieww1anrfV8ThiN5xvsuf8jMujbZn79tEKj0R0juFVJ/1u" &
            "QZuLVfaD+wHrN5zP57w3rXKNNs1VdHykpv22Wcs21+A5aLrBxIuPa3yUdzz0Ea6Tfn8K9KSdDo2rFDlx3FyACyFsd8yx2Qz0TZyc" &
            "e7EaEt9qzrhyTns7D6xlXfICz3uzG2eaZ5i0OeZwJu4qOSdu9Lw2jOfcWbfhjONcRMtjvfSb3TbnsX+n5XMfDfrPuEZidvxjeIgY" &
            "t/oP3MDbPE3jOKsjft8LAu+z3X7YgTVOdTZbq9s21+Qc5hpTvON2tbmfGGsHT/8OhAETwtkJ3moLrOWB0OZz9z14np/VcAaPN+e2" &
            "r1IjQS7rew485sn3NnhOvMcygeuy460bdHhs2PnO4Bq/jZb3BbW8X4jdHNnHOr9tzRy752FC4zlmxzE3IN8c92Nzjglel9h4NKZa" &
            "YnNrn5xYg8ejfF+U43S+A+eijdecteE5LznH7h0ISW6rvKU5w1VfhrsYgjXsaiPM2/GTO/kZm+OhVzzUm+q27tuANXuxzuBx+PwM" &
            "HudkM5rvlriz+Wrj59E5drkMx9JOP23mrGEfmx9M9uDW+aDOTnPnIzgXLSZ523q0fnT8biDn8RnXzavjqbYJ5h/6crq1NYw18Tny" &
            "3P6J03QZ0+poMUua5NnnH2RcN1+WL8RMcOx19x2Iiybs84TF7wkyj3CfYNwO4R3avKw35574gHnaW+Y2NvqabsvZ4t8WrpH2W8B5" &
            "o635PaceK+G4Fn8G8hLLo3Ub2prscIumfdfNxWkt7wP7eLQ9YAw1yEtNzkF/248T12h1J5dffAX+2PmWXBwba/axwTV6fqbW+BOS" &
            "w+PZ5T6Dv7Oa4JptJ1znVF/zT3jjjykGWeAlYRfLREzYkrvftIzGCc8++q3FGB7pY99jmXDoQcXYVl84DbYzfrrY3Kdtyt10iB1n" &
            "Z2Nci6e91eXzNn/Uba9giRbvvLE1e3LdkoeYxh/sfA3WbwjHXOdpfdbTatvNn+eYOjwaibHfdRzDg2ZaszP4HRHBnK6jgb5Wi/WO" &
            "8g7I/DNYr8W09eB5iyW/2eNjP+eXdJw4Qlf9+4XrDe8kXIj1mrYnNH7XZl/ecmWAHEM75vwo71Jcu/OydsOa5rEu1rTLGTvzTkfX" &
            "SbDvfKwzuR0fuK5dI5wjttYI153jUR5CrL01xjpP4t1vcYw99IBxjF+ETTrh80iwtuZfm3kNHEeOx+5Y1r+b15wzLoi9xcfv1q4l" &
            "x7v2qU9NvztyLrZoWDdwLeExb8DzBo/VtUy1rlKHY6ZGWKvB47p44IYnJHABbTANO1+DB9v6q9RHH/uOJ9pYPUbbGlotxJR/KXY6" &
            "b3Cd7E+xUw1rE3MrWnxsu7zErbwGz8GkFd7kb3AM+7b5RQpfjBmOzblvpDkatB964UG7bWdo10XgnO18lWuKzQ/aNM8VYxq/1bdU" &
            "C3mxu+5b52jK5/pz7vGsMheBHxJs0bCWbdRs3FU4rJf+Kf7p5RODXYSPvihyjC2wHo/Os4Zc9Fk/tp39rDmmofE9Bo/FSKzn1rqB" &
            "52qHFj8hWs+JmfAcjWmemsZuXTjvXoMGxz8XuzVaN+y154Jx0zm59vm88YnrcN2uos9mOzHlb2i605qaS/3EOKf7xuS3/oSpnqCN" &
            "Jbb2QAms27TPcHaNRNP64bYY4jj0X9o6GYu/lL9l5US7hI3TJoeDMsctcF73yTWaHvEcrVtwbJ7mhPO0+bsV1LIusfNN2MW45rav" &
            "Jpz5n4tJj7bsfc6xbax90lyIC8Kb+AZ5zse85jnG2Nkbdq+Ene+5OIvn3Mfv/Gf24NDHV8HZ9WQdwzmn/BwL98Z0PsWc1RvkHQ/f" &
            "STTEnvlx7a7HOhdPaMBACl8ul3Ut312wYE8KNVkw4f4EDvBssK6BXMcaZ4sVf8szwbW6mfNcONbzxNZ4zwX1G47NjwluxaT9HM02" &
            "5tifizaenY55Uy3PBXV4bPq003ctN4SGdo9wn2jrYtsUP9mJWzirzEeLa3Ny1bsDz1/TCcwz3/NAkB+e99pkC5xr4q1SK20ENaz3" &
            "xv8HkoRsRvOl77dkOTffxbZGHrnWCFqunHN8Bn1N07W7/23Q9FeZn28Lv/L6LjTPNDivHN9ZHDFxvR7fFswTPR6/TY5pDJN9neSc" &
            "4mzP3qG92Qyv1aEfp0ywn/28Cg6m/Ic+Hg+c37luQcZhcD6Sx/exNmfNRljXdrbJZx3D80Ikzu8g6d/BNUyo/yMhW0AefR6A+7fA" &
            "g7O9+YIpH+0TZ8HXctyS/20xzXFbtMl+K67lF2Zvo/Nt0Ma5q2HnI854t+Sa4D2020e3wvPQzm/FLeO6hUNwf+zqa3PR5ih7b4fE" &
            "nfGMaX1u0eH4dnzPgecleaf8no8lHbe82CPvObBW4tvaEPQx9iz/5e7ubi0I8L+n9cdRFJsGHo77jKdeJiu268m/hjTaxHhRJ07G" &
            "x4liba7dfSJ+jsNwDEF9HtnMjS1jaEhNbU7Pcj4Hjml69Lc1Iezb1eMczjeN32vVajTIzRgaL7CPNWXdeLFSs2m7T+ziqJ1+rj1y" &
            "XA/nhHNDm3VpN98gp+kQbUw8Z+4WH7vHt4p2bH43EjDeeV3X0r4hlzza8t9FtPWk1qQX0EYtj4k67eHh+oKnn/Gu4YIKnPyKn6ZN" &
            "kxy4qEk38INnDd+xMMY219oaYye45hx3MRN2Ma79DBPHOr5xel53aFzauO6ux/1b4fkOqOe1IDL+W/N7voh2IQWup/HIOaup+a1p" &
            "/3PAMbbxtvyxeaxGszV4PIbrsW2a5yB2j4OYYidM+ZzjGD5+C1ib55U5Yp/uqcmTcx4bmobheNdn30LNT/8SnUcGs9/suakHnAxO" &
            "Ku3xBdFxLdOksg7yc856pnqpZW7giZ0WjHb7JrQ5OYP5nieCY/O4Ao//DC3P0hwbLYf7S/PR8jjG8zDlmXz053yVdeEx54xzreFR" &
            "l7mnOiYwp9FsO0x1+PyK6+Ra3t167hsme+B5of0MZ+vQ1pB989fm12bkuuamt4sPotNqjz2fBDlnsKv3OXAM87W8xhvvQAwK3SLs" &
            "J3AG5cGuonHorbDrSux18yAhms8x9JsbpF4vlCfe/YZW09LYzrDj3OLz0f4G+jh2jplzu9NqaHrWfg6mGpptwm6tg8m+iu+587PL" &
            "u545lh12OrfWe+b3OLjeXuezfE1r0jPIa2j+iRtMfms1zWmszbaGeZvy34Ipdqqr4Rt/C+sYvnzx4nowrZirPuMNPAH0Xcu7BPKm" &
            "ugzbW7/ZnwOP2TpnNXrsq2gYx/CgaTbr72ppcG2tb9sOu7xNJzbmyfku9+RzXOMYqbnln+ay2W/JFTRus62TOTWo4fFM4FjOuEEb" &
            "f9DmnnManGkcwwtTw5r2ufHjocS28duXeq/DO7Zdna6Ltui1uMm2a2e4lZ+xvvEOZArk5PDjIYI6rW9N2s6Kts+8KabBdZ3BY8qR" &
            "tdg36U/2t8GZli+uo3wh59obnjOPXqdb4bl8Dm6NMc85XbfH57ESjWf9nLuOM136nxM7wfXwvNV3hrepIXDO5+RnrHUM2z2njjV/" &
            "wlX/RICtjavlIlwXj7fCeXb51sn4p7jUec0DZA1CJActUeANH1uDB2itwD6f+4vOSaeBtXFSboHr+i6w04vvlvo4F03zORqG7daa" &
            "cr4tbtHiHja8N1p97t8Cx7S9z7zNPuHMfytYDzWn82+DW3R2ed0/w7SWhPO1ex01Jq1pXQ3zjJZzDXuHMD/YxRiOT3/SMN9Izd/4" &
            "DuR4vCm/ePGiingS7u7unv7EyRVf/gTNxuK5qOnTtvALLNuN2Hn0+aX8NU7HLNQ2TTBrcT1+qFHr4eFh3d/f13G6Jua+bl7tuEbn" &
            "M1iv62cc63Nu10of65k49O9qNdeIv+l6nOaZ02Ddt4Hjr2X+WFOObmfwvltl/jLPHFc+tgmfteR6sS/gx9M88qOgHLmnYouftbR6" &
            "4vf3n/S5PttdO+F7gueQH6tzHKwn1zbvdzvNBmo5L2O8ho4PnDc25iBaTvOS4+CvsNawoJ4AJidcyFIs4UEyV46McX/KlaPz2tb8" &
            "AWtznsZz28F5gykf4ZgzeOwGbRPHc3HLGG/FrTqec4O2Vu8taOvynPjnos31rUhsanPNhMc02VrcLbiV18D95PzrW2g7rmkH37aG" &
            "zKV1rrjpNo3Y7XMN7jOGseHlaB91Wnz6+bcnS7ldK/tv/H8gdh56xR5woiY46SoTQnBSHXcG1mvQ18bTYpYWw5yzsRuO91zG5lyO" &
            "i828W+HYnQ7X2GOdYt4WrmuH1NLqariFY7QY52sc4xZOcMvYiWm+XOfZ3F6HmxnxnHHswByuk2h27lkebZvGa033g6Z/hnAzJr47" &
            "4DjbmF2r/Q2ttsTZx9rsI55zj+ccfeNXWMRR3sI+PDw8DTp92qa3gsxDO2300e/JoT91nA246TbbQj6/lW5vJQNrGKx3gmvL0THu" &
            "rxs2nmN2/Ml3lI8mviu0cQZep1ZfauOaOWZqAdedY22cHaz7LwpTTu8lz6d9jGnx9p/hKL8cnOLJc43xtXoC1936zc41c73eL/G5" &
            "Htpyzr/sYb/zNDDfOhl7MNldW/NRP3Y/EBc+anzzbxmg4FYgBZiAMX6SEeEsLSYfOoxnvoB1MQ+5jLHNvikHz+2PncecT2Mn2hzQ" &
            "Z+0zW+A6HdN8Dfad9Z8L1u6az+Dc7i9s7jZHDbzAw0/8c+v7zwK3jPHbwPP4XeQ7yqcB7Vr8LpD6+eKCaDnPxuj7jceTvcPWHiKE" &
            "5zm2Ve5fzJ84xrcaCNtT31X3/tj94jmcb/wxRQYxCYVbiyiP0WLShjbA2Fsd9i9NPvMc+pLWOi3v0sKRnzxT3A6sK5vZcxW0sSxw" &
            "bL8FnpedRvweq9fgbdDy3qI5zYlftHDNbtW15i1xgWPfFt9Wh2s24VZOjrfwJtC/4zJfw2Rv8LpROz7vj7N9cuuLEc/tUT6RCXY5" &
            "J+6uVuaY6rRGdKzn+EPv0Mj/xgOEJCda5VcXbOG9zSDOwKcfj/5IqdUWbsvp/hRD5Obf8pvbEM40Rzt7m9vn5PdaTWMMrH3GvxUc" &
            "w3O0PHb3vw2sdUtdjjGaxlnMd4G3ydH21hrG8G1BTe7rlj84809IjPfvbh8z14EHgR8mk8a1vCuxv4E514bn+XtbWIda7q9Sz2Xh" &
            "oeC3WNfHG2T7C70B7YGL2k3goXcI+Rkc/xaMES7r9buL+KnDGsJzXUHqMjyp7Ges0Y4taGO2z3PMMSWWfNoIxrnmpfE5FzktdmkN" &
            "mCttshOeu+Zjo8aBfUXb1N4W7QbAcXhc0xhdi+fE/nBugbUm31XrsrQGk0arjWjx1J+OrIv23XwHqSfrE5DbambO9HkfSZ8/xfVD" &
            "g3BtqYl/Tdf+IPrR5piaPbAm6/aRzWO3boNrML9+KNiItMd3Lb+c4mCm4jkBnoyGM85U7xkScxZ7lI9xznLSd0ue5PB8sm9N2wLW" &
            "2vxrw2m1ns1/Q9PJuffBUS60Bo+F8fb9/xvOrgv3V5mPsz5t33Z9nwPndO407g/6ifQZR86te2kC9Y3sszavt8K6nBOet4cW52mq" &
            "odVPfpuvBtpdz4GH/xv/I2HILprwqwOC3CRga3bzG67D5uKAXHfOm4/+HRjX8hvxOcbnk819Yqd11r8FbazPheOoN2l7b9wK74N/" &
            "EZjGQHCsxm4engvPrZt5xHdVwxlu1Z/uBwb3ic8dHzTbrWj78pb5XYpxfTnynVPQ1tX33Gmt3waeR8J5nOvh4eGb34EYbUD2p7kI" &
            "TzzBSZ1izDPn2Exuzp+LVvMtOlyIHc78Z/BcGJ6LHTiPwXPGvEPT2WnuxhRwbf9Fo43nOfA87May861SQ9ObOEazfVe45Zrg9d14" &
            "rb6MpfEbdrzo7zhE+FNdq2hxbPYF0xwwHz9OndbzVkw1tfy+x7LWbz4CC24t+Pr4u2d+h8GPrwhO9tSCFusBTbil9sTvdIxbdBuO" &
            "svHPdDhXhOumRs53us+Bc+9wlrPV+Vxw/m/RMP85scHbcndxz/FxDVx7zml/zpoRbxtn8DrOOW07tLHlnGO1fdL2NTLpNLDmfG+x" &
            "btByzoD8pblJPzzy/X3PdwHmbvdq12q8UY0HwqItxCTpp4jXr1/X7z2u+PyO8Ynj4lDT56mDT0bmJ891G6yDNuYjdlpruFCajZvD" &
            "4DimOohpjG2uG5q/2YKWbzeegGO3xi4uYE1vE/9tcMv4lsb2Nhf8bt6X/Gf749APJXhu23+W4JxOa+z15tjZduvTtKw74Xpyg+W9" &
            "qNUZZE9M+6JpN1iXdtbJ+gxqcA5p3+VffoDE0QYRoZaU555kx/toOH9sRot33c9By7FUj8diWKONZW1qm+wL2uTsajGmWoL4dpwz" &
            "sJYzPc7l2Z4wPAdrk+e7xK31GW1enrt2PD+bW/sdb1vrx9bst6Ktr8ecG69vwGy++XEMbObY1uq5BdaxdnSmB0O4Hlerh1r2R4sv" &
            "kg3PBzU4x+aG7340qEVcjvLdxUKhfiXMROtR9IKfraVRr+nbzuLM5wDiDxzfOAE51HS+JV0ew7VWy0s7EQ3nnfrJR1jDsav8JJhx" &
            "BvWt2fg77ObC8+jaeM7md6ctx3Ph/Eabk9i9Hjt4HgjPR86b7aofk7Q5jeZUY2yN43y220cO9aztWu1r+hyLwbwN1m8+1+F6Wu3G" &
            "oZuyx0BeYHseOofuQ5yTtFzLzscWXeoxZ3Acx3rx4sUbf0CRPuZ4ePxTVemznqfHZZK0ZPS5IE9iQ3wZVMO1XBBLk5x+G4x1XY/7" &
            "sVHrbdDmzVoeT44eq7Hj7Gw+0s/xmrfL17DjN1vg+WlourfETdjVGljf/VvgGPeN+F3XWdwarsdb4FxnaPrNZrxtHmtz7ewzmp/z" &
            "1Pzrhlod51qaNvvRZ56Wk3t0V7P3svtBm1POJ3PlRdqUc5XYyyqTE7AgT5b97Hvg5LeBOr+LND9gnp0G+RPCc64ppo3Pc+TYKUdA" &
            "/8SZYG0fJ9zCm3xTnVxvw3PSbIz7NnMSJPaWsX5XaOt/hjZG1k6ftd1fg17wXHvQ8qzBft28em/8oPHXyXgmXPVv0tI/Wx/6Gi82" &
            "6qXf+IbH4XX2WKlJO3kTv9Xk/Au8W8aSvBd/xNGKXxDL2xn/7fhoeDAsqhVDW/MHrnGCB7/TDDzmNn7jzG+Qz7p4vGVjE9RkzXy3" &
            "Zp659HMe2AzObRuLuZOP2PmnOp6LW2shnsO9BZ6zHTxm95vWLbq3YLf+hnO6b7Buc2kzx3zWN9XpXL7frcdY2hzT0Dg5TrU0WCM2" &
            "c3Z9wvPR6nR9mcfdJ0nWCJ7+R0I7VrmhBP7crv2Pg20AZzdI18K+29rUl9h2nHDmX+VG5vyugWh1eExu5tyCrIlreW7fsP+KdXd9" &
            "5k5wHMf8XcLz2Y6NS9u3RdPdwXPovm077Ra7459hN3cTXIPXxJqxXfVdbIO1JzD+TIs1NZjXtMNpR46L1xDHH7RcaW1ebp2P4MD3" &
            "2Gc/BPDxjQeIB0EceEqtsgAZTBD+0pe5rSV+yt1st+AsjjW2ST+Lb6AeF95Hj/3b4NbaXVOLI8I/453B4/Q6t/NW/7eBcwbT2Mxv" &
            "sbfAOoTtbb6n+rzPyLMuwXpyvuNPeJt1mmr0eFmX9d3fYcpBMEebS9vpN6/pmMuYa3k47sYeP+FcbobtOc8bgl1swFqf/pSJB0BE" &
            "jNxw/LbHsZ4QtxYTtIEuabZ+4LHdgt3E7cAapgVIHV6kxm2+xjOS/wzmPGeOVtmIxE5r5wu4rn8eOPTrlz8vPKd+1+H+2d4ynDvx" &
            "Pm+4Rd94TgzHsWsNsU+81ufNcUK7gbaYyR4fj7ZPNq6FuczndWt1ek9P50t7qvmt71zH8fg/EsaY4vhETEG+4PwXc9kMFnbG8yCW" &
            "NpxhvTZQgz7GsW9efG55BTEhehkD7S0Xc+ac897GurRmXpOmGy1qsu3Aulhbjobt0xjoY20Nk/27xFS3c7s/2QjPt/lt/I3LfvP5" &
            "2FoD94zngXCOnZ35rOs9mf11BucNrsMLKfNdR2y+EQccg+evaS2NybZcQ7FZj3DfusnPRvsOV32CZFgzePovbe3IYLIQ9q/ho6mA" &
            "fdvNafHGmd/wBPIYcHyG4zmWVq917Fs3LmTzWzu2Sfeq75tcr22stcGcxuccpRbyXH9qdu1Ey0Oc+SfcMu6ppgln89PgHImb4if+" &
            "DmfjDM44Ox36buFwzT0mw37vc9rpsz5zk+84+ya0cWaMbKvUZnt7UE65j/JO5Lq5Rzd43gzXF7h/zUdYRooMyZ/TRciCnAxPIkFf" &
            "a0sD4wSbe5aj2RxnrscVm33Wj99Ha+1gTdfoWt0nYs/6+aFyBvPY38Xb1+bE8D4iWg1uf17I+rl9W0THr3SveLHwNuNznazXR6LZ" &
            "djjjn/kDj431NnB+2vgYSzs/ouI7Z+fyfNu/Ss2xXU8+kaAWNXZ6BMfrcbtm+sxfJecxfNTHvmPWKn/KhMiEZDAclBM1m/m03dI4" &
            "8OhYu9mXNo/huElrScdj47Gh5V4nMQTrco3NZjRO4051PgdNd4Ln8sy+ypqd2Y2JM9mfA9Y97VXDMfY12O74af5arolntPltvJ2u" &
            "42+Fx9O0G9p47dtpp15z6Gu4ll9W5djqOYa9Ms35Mdzkgylm4gfUTd9+22IfHyAcCN99GEzaimWf59GbdBvMdZ9g/RPPfve/LaxH" &
            "3ZZj59/V5bjGnTbHBMffCsa1XN4bO+z8bYz/ojDlbvWe8VrM28DXXYP3gK9F2mxvOMvXMNXZck3z3MAxTe0Mnhej1dJiwpveidyC" &
            "jN1rc8tYEnPGM6aHyFTHOnsHErTBrFJgG+DOZr9tzGuNID7bUiuPbQwTsviO34F+c2/N79odN8VP9lXWqeEWzq1gLVxTY5qPaa2J" &
            "+Fv8rXhu7FTv2ozPMK9xmm0NdutNyJyaP9mC6+bV83Pg2pnXe9vcYOdvtgnmspZpnnZgzC1javotdgfmY45bYicklrVNmsl90wNk" &
            "hzZBhCfLn/kGTwWVf2ey027nQQa/m4SGib+GSSYY69zWnXIYjef8ztXQYmwzPMfpt7hd7jXELMXdwglYz58n2hq2eogz/9popZ/x" &
            "vc0Ymy7xHM1pfax/qybHlRjOL48591ydja+h1U6bayKPfec+8L3K28A5Glynccu8uGaPNfagzTs5x/H4M14mJyaf7Xyr5i+rJrQJ" &
            "8cNlGoARPyfk1tilCfHYcmx2L8AtuVaZZ9pY/w5n/tQ21TeNqcE6PG848+9yGazRdRDP0XwOqHtLjls4S/N/a8wEz0nTbJwdbpn3" &
            "ZiNaHcEuNjHTHE3n6ZPPI8fh8ZFL7HKtMjfur2Ec4bV6GrdhsgftIzTXx5w7uKb1qPX0M97A33c4iDZzPZENLITNg+KX94FtPLIZ" &
            "toWXCeaktgn2WA59vOaa2G961uE8WnuHxkm+thZTXcxHv2MamG+Xm31r2tZqsc5Us2twn9jZ23ngvPatUotzuT7HN+xi3IgD8+tr" &
            "1pjip36rI+dEeFOdHlND+OZRZ6dvnr/sZjwRf9ONnxyC42Y+6nB9YqOvzU369HstcqRWA3OzfmtzLBlPfYswidDnYiLYCm+2VQrM" &
            "BAfkWqth8rl2229F6rdO4Hobl3Y/OI7hVxnfFq7F/X8RcC7XQ3vAfUdM9qZzyzi9DwnXaC33GU89zvk09sA8v3Bq3IYph/vfFp6z" &
            "wPPpccVG37fBtbwQzdH5yGk+29majb5Vxk6YS8SeeGu3uGnvEi3OtfMBR47Pya3/DuQWtIKaFpPuJqHZrLeLbxvHdvvO4FclbEGz" &
            "NaQOL/ROK0eP4dvAdTq/bfbdgjYGnz8HvEA4F83254E2FoNjXjesmfnPQWKnHG1/XYdXyMGunsne0HIfeGE01bw2++YWTHHs7zhT" &
            "a3zHrPKjmzXsT+uyb9A+cd4GHuO1vDua1skxb3wHsh4H7e8iYl8lOf2erAnmWSscHuPn0Rz6vwtwojzuVjPB+Uhr87oe8/CdV2zf" &
            "NTzvnlue3zLGnY9o+jnfadDX5s1wzWf63xWc45Zab+EE0/6+5Zw27kXazXf/VrS9taAX23PGTrQ41+o9cAZyW6znjWPIect1lE8T" &
            "COfy3NCfGuhv2OVbw1rHziNz5Xts58/43/gOJAGtSItPxSyIpy0tRAP9LNIT6ZgdWq5m26G9ski/1WR4PC2/x3im+V3AdXxXOalz" &
            "i+bZeOPzkf6m8dw6duD6TXCOqS76dtjlCqzjPjGNYeIbznMLWj3OvwNjcy9o/rNjkHpSg8d0La/El+Zuup59DKjjepY0GeuY1G39" &
            "+Jr2gm/yr1KDuc7JsV6mf9nYbIb9TkykMHIcb4TPV+fW2MH67t8CThbBOnJsc+ZaW/2Jsf27Aut0zQ32uf8c3JLvDL6gfTTO/Dsk" &
            "D/Otsv4Nt+bLtRC0uOb30X72Y8uebK2Ns+HMP4HXA/Ol7yPrCq569Z3zCZ6jXe3THFzLx327vKw9vF3eCU1naSytfRt4XNZuc8B6" &
            "LnRkQo0I5UZ+6J0KffF7EiY4N4t23zjLsYu9BdZu44q+bR5HszPOuQzrn8GcqR5zdn3CGjvuGaZY10zeLmbX36FxvS5tD0xoemfI" &
            "OFszz2i8gDWz9pxPeW5Fmw/miy5ztHreFrva7WMtRJuDidtqbuOd/F6DpkewLp+voZ4ztPmPtj+2n3BZZXL8efzlcllX/Q90CwNn" &
            "0oeHh3V/f18HdpS/udI0GJt/oLOb5MnuVxG3wBM2aZu3Cpdjzbw0nvtE5oRzz3FxbrgG03xSl/Pf5tnH6PBobeaMbhC+x5vac2y6" &
            "C/HxGfZxXliXeeQ3cJ4ap/kydtsXri/Gpb7YiaaTc46n7fedLxpce8b4nLCd59ThkXPifUruFE8w1rjiOmCexPG/oghoZ02s/RZM" &
            "65SxZ/yeb3Lu7u7e+C/D4+N6HeWh7NwNrMfwuDNvmUNzH/Iz3paYk8cC7VtlMze9wFq0mXfGob35d3WsskGn+N3NlTzaj83N0DW1" &
            "2ie0eeGRaLa3waSTcXuc8dl2Bm/SVXK7v8paGNO8G017Gps5u/6tsK7BOlzTc3K2MRkTp9mMM//C3mEzvG6N8xywruRstgbPR44T" &
            "f7ITHj/zu2/cWjftVz0UMp6ct5xtzLR941dYJLAZth833jAcM/nS94Bi93H3iiTgeHKz2vEb8jQ+Nj848ALQ3viBa3E/4DhuGXPD" &
            "ro5b4LFMeQL6c36rBsc4jdlabzO+sxjmPbTfp7oanGcXY27QYibu2vhs5zhuHU/DFDfZg9TjutI/q2ny7caT67nlcB3BZDeuuIf5" &
            "fsGcxs7XXmgRiW2tYeczPI+X6cbLfibBr8KvmxtxYmxnLBepDTLnHlx0PZg2jh123J2m+8FU78Rfxef+ZCOav9l2ML+N2+Ba8mhb" &
            "8xvhsRm2uU/sfMEtnIYr9v3bwnNHu68Fc9j3XjvDNL/NFuRFU+4VgW+Ik/ZSnZN/un7WEGNQoyEaE8fzbpt9xlTjdbgXcq48d1Me" &
            "awS0T5yle9RuTLa5vnAu15O3NQxOUAMLYVyOTm4b4z0oF904BPnp23YLPMEHPs5q2I17Z2vY+VaZP/eNlvesHxuPRrN7XSbNM9zC" &
            "Y8238I23jUmcx/ptMent8rVr4m3nI2h5jOfOA/muz+fum7NDm4db8i5c4xyPOQvjjW96IR7sxsAar/jOwVz3g2lshHNMjZjmgHP0" &
            "9BFWis6XSH6oTO80sljeQIxNP0cXEzt1rEfsfAs5vAjOO02Qx2i0B4jHZw3a7VulHoNxjp/6PhKtXrcdGudsDMQUbzSeQU47P4vf" &
            "wRo5eqzptzHcirO9fy3XSOM+d7yeI+szB+fDjTG3Yler533HbXO341vvuvmUZWqTjvM6JjibZ/NsJ656AzDxDNdAW2BN1vv0p0xI" &
            "8sOjtYBfMFsnyLmLyI3Y9hbjSfYgDceznh1c91XzYLhu2yfbFHeGpnsLOA/RaPWw33gE7dM4pliDa9vQdFhj+u38u0Sr0Tb3/7zA" &
            "a+I5Oc/WdUHbN9SlvTTB9Zjb1upMc+d7DqjjOj3WgGNO44tq1t7qNGcNuWIzN3C80WICr2nuv1zj6V6eFt4TP0S++2hCtuX8er2+" &
            "YY+NBSRZkHxZgMvjT9doC2/pQtn56Lc9Pk6GYRt5znd3d/f08CQ/cK27nPbzyOZ3VAbH6XeOhnPlPI0Pd/r8MPW8Bwf2QatraX4Z" &
            "5zWLPWAd6dPn9ucFazPfVMMV14PrJ9/jX5pT8g3HLdXaHgrEpLtDrl3utzP9qYVjvs+P8nHTtHdou5Z9nP3OfRW+rzuuA+2tHyQn" &
            "z72HWZfrZfwq1wPHwr5b/DxyLlhX/Afud0HyP/1L9KBN4KGnlovn5uEg42/FNc6u3+A6gl2t1r4V1FmlRuNtc4U/xbU6jGYzbuHc" &
            "Auu4fwveJoYX2BnO1sJ+95+LszhrT+M405nwtnGB6+Fce44Mx04gL+fMY1trt6LVSRzlC+6FOOfy/nCs63N80OLPuMTEbbilHs8t" &
            "j63GI+9ADJLz7sAi4V31ACEnPh6tETQ7+Tm3Po+EJ8Og3Xl4NFwHj9GcYpdyTXbmiO3AW86GrI+1p/F/GzjHrX2OxWi1u/9tsNNq" &
            "893qCXyBBdZp8Y6ZwDoW9jNtDTvfLfDYbq13PZMb7GJYg9/tNzSt3VqsIabBvFvW4hbcEr/jZF9kjrxPGEufz3f7lnsx3ODpS3QH" &
            "LU3a9fEhwH9N7UL5sODDpGmvsigehBt57XyVmm1zn9rTcYJrc12T/QzmcK7jt75jAo/9u0SroY259SeYZ91V5uMWOL7ZdzbD+R3j" &
            "eWnn5tPvC/UWPJdveEyraDbO2th34JjdAq61azEca0THtbo/4VberXCt0xx8l7BuuxaS359GcU+G8/TvQII86U1+eHhYr1+/fuN7" &
            "EidPzPRugzFni5F410C7ayfIS17XTEw6huug3baAOXM+1e9+bFOuxudYm/9WnM1Xa/Q1THrELp54zjg97zlvOG58xRtQZ6q9cRov" &
            "mHyc4+8aU122ZX68lu5Pelwv+m07a41LndSZcx+5J9rR50t7jo1ILbYb9Dsnm3U83h0c65zUcZ/wWMP7xr9EP8oX6NfyroKgxlE+" &
            "SqGmdc2lz/3U4dhJJ7G5Yewmr8F+nrexkEM/+65hmtOAdXINHNNeLfyLgufItcV+BnOsG7zN+Myf5tGw3+tn/9rUF36LIbiOze7j" &
            "Gc54zc8cre5pjM32XESjzTVr8L0gvCA1+mEXW7Abf+D4pmtYY4dDL74Jjzs2Hg2uj+dxd1xl/ndjfPr/QK4nT7q0cFrhbaA7WNuw" &
            "jbXyvG0kxtiemIfHP2HgmLfFpON5XQOXC+ZF9xjbmP68scthn+s7w457LXtzYZ52sUvz2epyP7ZmJ7gO4bJOr6FjGuzbXW9tTowW" &
            "1+C8HluraRrrLciacG2annWvuHaNaazOwXOPbQfX7EZE03aCcW2ufQxstz9wXeb7yBf5aXnQurboXjwBR/noKvYDf0XzofyF2BTB" &
            "j8Go3Z6ubZAusg2QvMA++9urzqseiq1GxvBorcDz2VqbpwnhWyO+yZ/mmhPXal+aB8fkyBrIYy30NYST89TknHy1uFBfajybR+tF" &
            "02tAbprnlWi2wDUTbY54zHlriXfeVuPEd54gvMxJw5TXYL3pr8JnTuYO59bxxx44brquk5M2cwLWSaSeA/uX/aV6HJv8HD+vP483" &
            "57Hv5iv5qOH6L48/lOLcUy9Hnq/UC5034IEuFUER+slji92vHDxY5rVeg3NM8OBz7mbY70bcWothzakZt+RJXIsPdr5b0Or4LjSb" &
            "7g6eJ66H262w5q3jmmLeJv9z0PjcQ/ZPtlvQNN1f2B/xtZs5eTtQq9mbz2Ddrd6le4/hOMZzfXN+Lf+uwwiv+ZptbezBdTPXE8z1" &
            "+O3/xh9TNMGTkYEatnsyDjwp48sA26tdxj0HzrnKZ6VTfoL98BgTf46s01oTdnXEfiuc/9Z4c9wnXBu5vDh2GoR51/Jqq6HFtTV8" &
            "G3Cftzn1+YQ2R8RkJ5hvAvOQd2tMs+f8ObDmFO918rnjaeO9w77A/Qmsly1oeyBxDS3+Wt7hBM7L2Jaj2daj3XMae5rvLY2TPn3m" &
            "NTz9KZMdzPGFleNzJnxtinQMdXn0efqcmDQ/AKaHyjTZ1PV5+kTzORcbsfPdglvibTfX6xg0bXOtbdBvrTOY17TeBlOc6/Nxwi21" &
            "TJxDH33QHkyxa6htN46d1tK1FjT+juPaDdpcI+EcwXX42JXIvLax7+KC+H0vOYu7BYc+hrav4SxvG5eP5LHfYJ3r9fpn34GYuLRx" &
            "rri5ZrA7PsGCqGdf6xOJZY7kpC2D8+LyPH2en21A4xaufcnTajtD+B4vcYuOwRqmeNt3XIO854zXmGJcy8QjEtNip/i3qX2nt8Mx" &
            "vNNfZT6Zg0f7GBO062byk5NzY9Jr3GZbj/ZcI+2a9PcQ4Ztnffavj9eROUHsvF+dtcC1OUez3XJNtzjX0OoJz3NkXvrmGfS98d7q" &
            "TDi2DJZPzCzGlPBB/9Xtenw7G78n/VaY6zFQO1zWOdW8Bt+0yKvME2Ot08C4LOIOHlfOXQd9PqZlLXZouu4TZ7zmbznOYJ12nr61" &
            "c26fY4lpD1jDvh1c07W8UPCesOYttRvP4RItbrK1eWGtrps3MD5EGqxLO49G7lfTNdTO02cjx3rmkdv6vI82f9DmZMoXtPvrLn6n" &
            "tcB/eoA4qBVIZLD5Bp8PEHNX+R6CDw8eV7kxXnExcSI44ZlU5gmmc/ad/3j8wp81s4VnnQnknv1qgjV5HSY4dm1udBOc59b45+Yl" &
            "17Eev9utaBr0XcsrLcbwuLAn2H8unGdC/LscbTzGlG/SbXOxwG9x5rO/2+cT4vO13HJMsM99joftDLtamOPWMTe7+2dg3Yx1PdMY" &
            "XWOaH1KMj+2af0jImzkDSJ7OD/3KwFrUdJ9ouZt/0ms3WurZF8Q36Vp/p0M4lvNirZ1mW7igXWQNtPM8uqx90iCYr+WdxjbZPU7C" &
            "Pue96sWFx8J8rpX8ScNo+tYlrGfeLtb1E9Tc1RtMtbZ5bTHkc0yu0X3CWvaf4VZ91zY1xhGeT/I59lzTng9f60GuVV//7EcrNu8f" &
            "glrrsQ6OL/HkszXQ7jExT9rTPyQMmjht9DXuBE+ydQgWmmNiWuOC5djOqTn1rWlf8Jyb99RuAcfufmsT6Gs8borvAtRpa8963W81" &
            "tJoN66xB623QtI02tgaO+/9b3N/9WpdlaX7Q3Pu8H5kZ+R0ZWZlZWV2ZuMrVH2oDAoyQWgIkIyRkwQ1cGISgheDKEuKGC4RF/QmI" &
            "G5B8AXJbQjKNLQuBbBkhLnBj6Jastqov2qKq2lWu6qrK7sqIyMiIeD/OXlzkeU7+3l88Y+79RmbLjzS15hzjGc8Yc6651tp7n33O" &
            "2WHnp6/1d7HENd5uLpM9uBa7axPsO/CzDPsaGi/xDY1PuO42B49vjSN4/RAeh+d9f5SHUcbOax7h6/gzH2F57Oan5y2YJkVc03Md" &
            "rCV+HpstC8PGB4X1aSOncc0z6Pc6eBwbT66129hcc4im6Y1j/zVMmu67VjavRdP8Twqu1c245tth8lPvKB/DvQ0OrbfP/4Tk8l5x" &
            "bcbEzzz4omxC8zUbkXlN87PNc/C+3cHzMp/jae5H+fivvaBdQ21eZ4LrEN2p1uAYfla01vrsLxKaEDDJ7kSTR79PUiawSs6JS03H" &
            "2Gb+gq71gyys4Tm5Na5tQdM3zDmuPLhdjx+s7tu2hpwt1wRyd3He3G+bp2E3r18Wfhm6u9g2B/reBrfwb5mP19Nc+6xJO6896pBj" &
            "fV6PLaaB+r6evcdpazzmdn72d80cIw8Gc5l7d+0bU83xtTXY1ee6Mo7WG78HYgEHrRtuADmBsV170Bg8eZywxwS1Jt1V5rfeUpcn" &
            "0nxz2RqHuIUTtDzp+9j8wc5HW/MFOdeBc8Zm/7U1vwbniK1hZ286DeGYy3m0uUxg7l0Nzedc7JPfYgPab+VdszFvkNriz3haq8ke" &
            "OLfHhH2ep/3TuXRNnKc1gvjsb/aTrqFWww7Ucz4iefxOxvlYI/U8Xg+xZ/4+RyvaCaZER7mptrdHPGZS04KFyxpcj2uwzTCPx1bD" &
            "KjHWpc9vRxuaVvpNn3DNgc/P2nBXmSs55ntstNg2t8C5A5/Ta5g0PW/X0ca7Fkz7tIFxHNtOTHl3+EV4zUbcWofXZLo+yd1xyD3p" &
            "Bem1msxtvmPzkcwO12pouZnzGsh5G77zpr/br8fmge7cTT/a5yUBiwUn/KEvLn5uloYXI1wWY8Tndnn45cXG8426abeNGl6LNcew" &
            "j3XsYj2HHFuMx7HlgiIn9kmftqYb0GfeFBOYa52W2+PJxrnZRzSe99yujl1ruMbZ+YhbOIR1D7yy3Gk1n8eE1zKgTtY8XO7B7FVe" &
            "oz5HnodzreGexBgfm07qaGjc3dwXauJ8yDFvV9d0TJty8Ei7bTxHrNsajG06RHyf+Wu8azhhC/YL/hJvEEFquSDnCWxPnFvDlC84" &
            "yoOrHZut+aifMf1+B3Jg3uQ7rmnG7jk2NDs1ljZ9/ORNjbCtnTePbW8wb+p7/zjfKvOzn2ParD0h58N8z6HBtZl7lHfywVFeMAUt" &
            "ZlcP7ZM/aNrBjpcXfpyzr8W2jrGv8gKgcdvDk3O3nTBn4t2C1JY57eqmzzY/6JpOcnh9rUn71K7V1vzBZ36IvsrPLVwUHyDxHWVz" &
            "xL5QBN/FuEi/u4mmG2M4OS4KW+JYi2skJz7rmJ++ueQRO06zJ4/tQVsr69DPGPoCb0hzdrq2029O1o71T0drsEbPn2icIPw214ZD" &
            "+8PnJHBtrs8xnlNDW4MgMYy3Tou1jm3sn8q10uKJY3jXwWt40giXGuG2OthsZz6j8ZqW4fX1XOy3j37mso01mzOdb/IcE03P0Vz7" &
            "kmv68zFvfI2X8EQSzJOSlodJO1mJo55zBS6QfeeM3yclfMK68XuxaNvVGWSurbYpz1Svm+2G7YyZ0PjpuzXeeqi//XVRI3GcL/W9" &
            "V5zbeWOb9tet8NxazmZjjDnGFBtb2wNL14pxi61pco7tupnOj7Xpb/2T3mE3DnXob/k8Dho3sN36rU38rFX2W+MGvu6NKed0vo/j" &
            "WPf392/8+3DC4yDnYKrZca2u7Ae+0M84x8ReLpef/0tbJzMi5KRTY8xCwqbZ4MWNrk/WFO9aiMS0+RC22b+GLwoQsTUO+7u1WOLu" &
            "NImmOXFX8TkPYV3j2Jyba/BcWyNuyeN4awT07ThG093VRd8Us6uDNudtfqPZ1jCP2BtY45STmrsb7So62cNcl2ldJ7vh2uzzNW1O" &
            "4Hyue0LT2+UJdppB04jN6xifm+E1IR7/H8gOt5zEpmHO0lu+drEQx3AT8oRb7D8p7PKlVtdmvsde14y95kQ0mr41DMY2284+jRsm" &
            "X9tPDVP8UTY0Nd0mtHlOYyKat6zBKud3XdGnpmu0z37C/rYWjvf4Fkw1NT37TuWj6x3Cm84xtdp8DXJS06EXqkZyxud8rS5qc2zE" &
            "xnn4nsk46h7l3e21fOvBl/nSNjXijf+J7hYkQWxeII7zFseTsCbR8k7cINyLvqG1w6TtcbCzez5cCx6NqYalmCmecA2Tdqu3cd0n" &
            "x41otmCyT3BdU05yPi/edr2DKe+0RlOf2MXZ3kDurZh0aXffR/fNmXytT/4vGzvNk27Ka6iroV3vt8QFU57oeR3dT/ykY/+BF+U8" &
            "plmH/XDoe/xFQgb71V1seUIxoYth38UQLoRtegu505vA2nfak30NN5ddHT4xzUe0+bkW5nPuo8wr58rcBudtvqlN8PklrNH8PF6D" &
            "tdxWOSdtvxLNRj3W1uZA+9QMa05wHezblrm2nKeHF3y2B663tfAcR7S1XA+8y8PPUP2tzgbOY2qE7cmXPjmNP9Xd4Fi/mm/HHfiC" &
            "fUGf9uZ3HRN8HXAcGzWa/3w+/+yH6CTyRutiPanYgyl5+u2ENH0ugI/BlItg7NTIa/npn7TeBj4JrOHADyOD9L12ztvq8diwj7U0" &
            "MMfEMZpm0/D4GryG19rbgGvdNG7RNmfXWhzH9k9wrNH8XMcG1+q6rUdYt+39Y3PdW9v5HTPF2d6OU1zbCxOsk6M1Wp9j5/GaEUdZ" &
            "P+qc9H+bDP6w3DwfA3IfP8IyaZUFYaFJPE3ID5+FHMfDtwz4asC5GMdYL+YF/6gqPtbgh96Uy/6let2C5OSiXuMG13xH+Uwz3HxU" &
            "GJAfTkAN5mTfthwZSw3HusZgmlPAPUW4jvRbcw6CvGtouTymzXlbMy/cXYxzeN0PXWNp4bo1LerR53wTPCei+WJredbwwtVr4fnT" &
            "F71dzYR1vA78tpF9E3b+aPJnG54fYycdx8TGe2nAXNQL5/zwv5xSF/nkLc0tx8vl8vNfJAw5BWbCLXAqMrG8eU+b4RpujXHd5rN+" &
            "culv/cB6Bv3pN52FE83jVPfa6ATN39bjbfRpS9zufDbdtVm3lnOn5XPXcGxuntND3Tl3+oZjrsU2P227eprtFlxbt+j6fBLeC4TH" &
            "hOvdcYmjvGBibVOd5LX9FbQ18X6JjbVPOX9RsB4/yGJ3/jYH2r12hjWdz7im+/gzkKlI231BEhe8G2Djjcewzboe2978tNkX0M7F" &
            "IXxSs8mmV/+E19A3318UrCnH6SQn/7U1aevQ4nbxrIO8yU64TtqNrCNrmtpSbW7W97iBeWljzlv8OzDH5crPB4xdDtaRRn2fK8/B" &
            "dms5hnENjU979oT35xRHOCZgnPdDA+toc5zirOk42mwPbPc4+jy2nNO1ZVtbi4zDTdxnfgbCQCZNo6/5W2tIMRPH9l0++h3jo5v9" &
            "12LbRmmazW6/tehraDWtYcPkyJzObxt9jWtQv+XysTVrGS3esM26t2CnP2GXx1r2x+b5sT/V5LFBHR7Tb838ljdosfTtMPl932n6" &
            "Uz9j82OfEJ/zHeVjUXKJyX4Nt8axJt77/GLU95IJzHctt9eg9evvgbiYvHq+v79fr1+//ozYrhmxxd9e4a6yoVIDY6wV2MejkVde" &
            "k38Nr8LTd6OdPMcEXuvGta41iIljW9PkeAeel4A6XNNdu4aJYx3q+WE63ZTcfhEkftK6Zpv6gffIBOb3/Ka52nft2iKmGHPIpS3w" &
            "OVuF4xrt89g22l2LG+3mtDHBuUxzynG3bkZ4fohcg3N6rZ07fs6hXe+n/EzHAktJc+N+/fr140OkCj38QP1tFoSLYU1PNHV40af+" &
            "BOZrbYdrfsKajG05Jz9t9plDWLdtGsfTzoem4ZjWzI/m7pzzyFpon0D+27a3gWOY17YJLYZodl8Pa+At1djW23MIYk/M7gbV9A3m" &
            "c+1L947mJ1hbA+e4a1PMZOd1sOMSmVebU7heN5/fdg9kXWz0O6c5vs4aJwjHuVLXOV/jbeANOyfNhUTk7u7ujWQ5OrHH5Ae3Llzs" &
            "5BjNRkSrbUqfiAkth22uc5oHbea3PuE1uwZrTbq7Wslp3Gazn3D9LdcOvlmycV+5fR7s5kGYM41tD1wreR67Jvftb9jxXGvjXFvX" &
            "azGHrkfqtFhjx2nzajba21ynmLfFLRpcS66N65vQzsMta+o481LX49/C4pM2BP6w+Hw+Pz5Mwk8/YrFxQ0QnfebKw2fho6RL+Uu/" &
            "BheOnBYTLvOGw0VqN5/U5AVvGpn7/f39o91x1OO6eH2oHQ3Oi33nSb/ldu05v9RIn/Gx89hsjG3zOOmVWasr6+hc5HD+t8IxLWea" &
            "czF2tz60WWuVGhzT/PRdhr+64BqNNlfGRJNzc542h2vzdf0Zh8cYIvn9Kp51cT6Oty+tzStgzYzNmDzrEy0vtdnyFVrG7cD6qUN9" &
            "HhnHGNfmORjxm3fKA8Qn3xshwbZFhBM44UESTLEGC2VrJyrwIjZMuW0zjyfCNTWYE96On6P5ztM0Gs++ZkvzOWp92+zzZjU+r7/V" &
            "T0y+Xdw0B8NrxDGb0WzB2/i8Jrfmc31THHHt2vllYsrjOnNNe9zWJcd2z5pgnTXsDa+jtaPTYnjPmuKD6OzqMqjtOOZyXut5nQ3H" &
            "0346nX7+Q/TWruHAwyc3o8S5mKadfjYHfdFsf+KAG4p5rB8b4XFDq3EH+1kHdexrscat+SeO7bva3H8b+Hz73LwNfE4ntLk0n+3m" &
            "GNZtHOMWztui5fY4Ns+rHRtanPu3IOeMOd3ob5h81PC+atq0Bby/2BfQn+Z7D+M553Ad72at6HhO1grMM2iz32Mjfh5bf6GON36I" &
            "zoJj59vI08PbyuPKBN0CxrjPB1D0+BRnDYFzX0O4Lc61rmE9Gq9xMyfbW9txWw72uUbUmOI4ZqzHb4sW12xvgzavqYV/rZmXtZ/W" &
            "juC5v6YbMI9jHDthx2m2wPvUtTV7878tPDfaG8f5eU68boHntq7kdW7jWqxbfORNMcGpfLRLX2Jz3LUJTXMCtY7hPtR4zP+Z/wdC" &
            "IeKEz+xaYfHnB+rtQlv64Txz8uh+9PizAvIcZ1uQuGjs2hpOokG7uceVm7K5jbfzLfh9A2x82pp/lfUkmt05nT82Nq8z4bXPPrRG" &
            "9IPJbl/WyfuvxbtO+wPbqduO/yTgut13fW2+rc5bajaHus5hTuOy+V5BeA95PyU+fddiTeYMnCOwVrNHi7xT+aJAuAZrbrWRR7Bm" &
            "108dxsXG6yOwBvHGLxJG4DJ8dLSwAIlhESd8LS9gwa3oxHGcGOaZ0BaHY0/cfNtiT6z5nINzmRMbx61G9jm+hvB3+kZ8mRvPZ3y3" &
            "ajiGfmv+Ipg0phzX8rf6bc/R5781oo13rcUE1m/XlXUcM2kHvp4/D3z9rBvyGuQ3vWA314V93dbF8Y6znfcy+m9pviYZS701fHLA" &
            "6zMtMNewj/l9H560dudgIe4z/w+ED5HY0nhyyHNRTsJGuEhzzLe/Le6OZ2QuU07G7ebREA6PzrPTsY98+ybseJwXkXFbrwnTHOln" &
            "m2DfNf6tuFVjd5Fem+N6izxEi2k52vlwvey7mUPubnwr2n7aae18DbwW2w13gtegrWNAbnjTvcNryljnXOXjeGs6JhxzHWe0euhr" &
            "49jywOTcJ0Sm/h4Ii8jEefJyzFdWww1/4WcnXAAWv8or85NeDSeWk4wOazOowfHabET6yWtzYN6W32A84XnsmmEfOVPM0oXO2OlF" &
            "gOcf29rkZG3kRMf+NPsIj412U2Hd9qef/Wq4ttjo55q5PtraGlq/NYI2Ho+yfya0GmLn2Fo+BuSR09bFR8Iaq1x/BOtl3BTTamqN" &
            "GrHd39+P68NfN3A+aja7/R4TsXsdpphmI46yZ5rWm/XdP7TLw/H1ulxe/+xbWL55cMHawoeT5vjE8tgWmH1PwJugcY9NfRMcv2vW" &
            "NmybeA3h8chGn+F1Cab8k+24cgPlkXbbruEX5Xt8K3a1Zn/xY6EdopU1i+7uXLC/q+UaGG/dawjHGs0+8Sd7bPQHbV2sZVhv4q2N" &
            "BrGrodka/6SvOZPrexrh8QSvZfq+p5ET3qRhm30NO87pxLV5c/6PDxAKhEx7ePHz80H/4h9jCJ4MT87F+2TS77gW32B+WubmtWjj" &
            "5OFxym175uT8bsxjeF0C8puOj4RtrIXja5hqC27VabgWu8vNc7jTaGt0lIs5mHJey3GUn801MJ9ztzrMJ9f1u2+ex7RPiH+amzWc" &
            "i3Zz3CZMvulmz/w7fdoeb576QtEU23CU65z52/2IPPMbpzXGT/D5Cz/zZnv8IfokGh8ny4+n4vPfyGL8QvLARRLtpEyxk47zGVxM" &
            "zzH9jHcnOs1o+re0tmGIZrcGeZMtR86trbt1mt0xO/4uZvLtNOnfnXNypwuzwbzk2OXZjYPJbniebBOucSY7MXGs7XGD/eZab2cz" &
            "rtl4rqzp5mueY+s6b8Ztf+ziYmt1eO9Zx4329KlljaDtZc6BR3Mf/6HUUlGxu5hbwElkHLAAF+MFmTQM6zS0eObg0f0WuwbNYLeB" &
            "uEmZw42bl3B97Dddx/DoWMJ2x5jXfLRZq43JJ1oNE7zR/c3AtdGZ7ARrd81uzT7B/im+NfPaOLjlepk43tdusZNjbhs7bgfer3L0" &
            "zTbwuNkct/Ob6/WgPXXZN2nRn1hy6SO8HgvfqG15G3zN2N5862fXVv+9jlWKnj6qajGGJ7MQ3+y+CV7LaXvGXEjqGlwDr0c2wzRe" &
            "mjcX3HU7jpwJjG167cHumCnWvviv6RzD2np9qU0e7R6bz2bNgBzXEPBiyEPFDxbDF0/m2FrLOdlY624dHU8fc5rrcbOb3zhGfGys" &
            "h3VTg2tobY+Ndm3y+kq8c8fuI+Nsawiv6dt3KQ+M2Lw+3I++XyzcZ9x2YC0E15DweWG8ayHie3wH4uaCI5zFoMgqCTJ2gTla2/3G" &
            "8QmkJjm7Rs5RTiLrdd9+Y+K45gmN0zYD9aY2YedjLvNobz7a7G9otd4a1/pruIFMjfA4cIxjJzv9rZ+xbROs41ift/R5HXPPk8Px" &
            "rY1oOobjJ94q69Tg62ttcjQ928zfxRLkeX3ZX3iIJM5oc1rFfugrxju0PA3U6fX3+9rlcvn5H1NsSFAKOZ1Oj/9UKjFcoFv+na0n" &
            "xQVhXDAtFnnWbJOdwDpdO3NkHbxRAtbpuMB5Wqz5Oe5yk2sb7Y2zVFeOrY5r8Pxuyb20Rt4vgbXSwufeM5qdseS02huaf7LFbr/z" &
            "pe3W3vvafsN7ivaG8Bnn+lqt5HMPtHO5rmi6Oc57zNeF4zhuehOOh09dJj3m9vwDrqHrnPpTHPcq9V0bfYTPLeexNC/GN1v65xRl" &
            "0YDiARcofC/AQjIuLtFiM/ZiU9egn3r0Wd82czO2vcWvzY3v88AnzrnbWrseHqdGnrltPo7nPgi8MVsu2mh33zb7nGNqjb+D48P3" &
            "seEWzg7O57a0PyZei+E1GPut/QbejILEtGuQ/tZn7K4d2HvUvTy8kMhH7eSzOWds7R5FkE/9hul+GuzqMH/SCK7V3eD8q+j8nPNm" &
            "bTzn51U2V4htQuFOJ8ix089YHNcaeQ3kuhaPaefR/V1jTurwZ0Osl3FBW4ulc8Cc1GVrORjX7K1vMKebOekbbY4tp2tpWoZrYi3m" &
            "sW/9nc2gffK3MY+T7RafeeY2mN/OySovBlt/wqGbCe05su99FUx7Py17ng8Ha4fHh4fztFqc02BdOTJf89HWQA3nZhzta7hPN0xa" &
            "RJtXw89q+HnfeONrvJ5MkMKdNP3d2HY2nsQJ08ZqNmrdsti3apHv8S3xzWa7fUTz3xpLhHdLXLsAA+oE3CPX1v0WTHld+zR2f9Jw" &
            "fONx7D75bUy7NXfY8Zo2fc3e0Objvm/6bOav4bpr9ViH9gnt/hPE5tweO9bjoM3Px6DlOPTCps23jW1PPzl215jjaHOuNZwr4udx" &
            "/V5wyu+BrPKOYCq0jWlzovbqwbmi48m2xjw5+kGUmnbfsrEWx4HnusrmoS3HzNGvcnx03vRb3sBrPWGnEVzjtBoX4nLObH8bXItx" &
            "bsK+3dpmT7TmGJ8T55ng+CnOdq5nw1SbfesGDfdv9cfucbuWV6nDsUSLJ3yudjD3mnYQTpsPx8fmd9LIbTrmuN841+btfNfA9bHu" &
            "FL+r942/xkvHNbH4WyHhZVJcTPY9EWqb304I+w3OQ7vBHBmTd2te18kHiVtgW9sMnkNgLdpbn3DeCa6b/GldzW+xjjHs8zi5r+2h" &
            "W+oyJvsqN40GxzrfLa3pWMuInXvolnPEsX3X7PZP+rTvONY1x7Zj+CJOrr326wc+Wj9oPmvRH7R6yPO9r4H7O9jdS2yzPxz2G6fB" &
            "nPQf/yOhJ2PxQ78HYsHT5qf68cfGcVtALu70M5TgltpjM8xLmzYI/fFZK30/OJqdsbEzxuPYqJm5O8/bNNc0+VvdrZHvObh+69wK" &
            "7p/p3Gec4+XGjxRuqWO3JwPXkD5bWyeukXm72lw/x7ymGr/F7HI1MC5rfYsuOethbfnXLsxLP3l2mhy7kWffTjM+723yd/vjFm78" &
            "nj9rsN31Gq7fzftryv0z38+Oj78Hcjxsspy4fPQTYW+IiDO5L1D2o998OXLDtOI9wR2Y07Wybz3X5sZ1yNjzjkbWdQ3vKgjWafvC" &
            "eSCmOXjOjesLj/OhnrUJamYdvB7mZk1aLoP6jZd57vZNjq6TmpN+QztHgTV5pJ+8W/TIzdjn2HMKP/7AmvRdA2MZ79ZqWWWu5kzn" &
            "0vaWh1qnzR/JZGziuV+tvcr11Frjn/QRb8Bar+EYXiAbsdHn2tLfXXec05vzOa+1Tus41kM7rbVOb/5L25+TP3sxLkzG7wqcLDb7" &
            "YzO/FZ2+J9tOFEGNW3BNK0fy6GeuNqbddR3YvPYzttnSbrlYreGcTbfFEi3GWuROOoZ1CO6Vad7hBTu9tZlfYL/Hgee7A2tqjbxg" &
            "2pvse02sy5hb4fh20550m9/z2GHSfRtEg3W4duexz5w23sF82ls/99j0iVaTta2741p/lZi1fvawOPTwOA79T/SAF2rGtn82yVxM" &
            "G1tj4nGc1p6i7ofTuOQZmYNrI/eWeU6wZmq1jVzWb94UP2m1Me30rWE9fKMimuYJr8bsS8wU70bYZz+RGlqzhvWaduP9IrBWOx76" &
            "AW6z2cf935pjpmO0pvjJ5pi2d5wn8N4jN342YtJv1xL7bew1tOauERm3c9jgPer7WWyuaQeuVVuzpkH7z44/Hz/+LSwXabE2afIn" &
            "HvtH2ezOE7RJOqfrZF1s7bdJd6DedILSz7rt9B1nHm32Bfa3nMfmAp1y7PI1n/PbZp5hTou9BYk9yjl627YDObuYa5rNtkpc02h2" &
            "9w3HtOvFXMe1cWy7ozFpOI/R+Du0e0XTOLRn7LvWJtBv3s5+DD+javfK1M1zea0u3kvbfdWwbpr3z3Ec+/9I6D5hIU5sAhfJi0U/" &
            "x4Yn5eaFbQ+ba425DHNuRYux7SgP2Fvg+qe5hsuYnY7RfLa53/aDOexPyLo4/y8TnMu1eblZp6FxY+ex2XftFrTrYJV1dc7Aechb" &
            "+oXECU3j1tbiJ5BvjcnetHdxt/C81mu4pwXN12zXap58wU7TaHauwxsfYU03di6CfQFF2YIpLmgxrfiAfF8Yk5Zb7L64grYWCzHW" &
            "aWg10N7qIeifOIG55jl+Ot8B+TutXb5mDyaNa/xbYL1bWovlOP0Gx9wCc09X3s3Gzn1qmxs5bgb5OU46Uxz92Vu3akzN/mmujZvx" &
            "Djue9Q+9WG61XLuXmD/BOr5er123E27xT7W59s+8A3Ggx5yEm8UNTtjcqflEEPQzZ45e4ImXvk+Yv0ro2hqcc5UT5prTmj3x8ZsT" &
            "uM/YgBvP/TbHtXllaX7b6BNca/O5DvOItq8M6rmZ4zj7WzOajaCf9Z/0zZ30c/T5v6VNODYvhHjcNWo1kJv9mz01aU37PGtDPfcJ" &
            "xh7l50cNvC4C53GbfK7dSEz7PRXWRw3ek9gaPEevR4PtrsX2s0+Kk6Q42mPzDXaayBp8x+amaV5gnWi0E+eTYj812ridIHJbrYS5" &
            "bNO82ed44hOTzsRfZT0Jxr8t2hwauL8Cz5Fj2n1uiF3OCT7PF311fYdW39tgWgcfm7793ufxt/nRbx37pnFsQbOb3+pr3Izp8zH9" &
            "08O9zPcj6wfXclvX9xP3ybFGdAL7GG8NgnOzb4J5rikt9umaWlrTIx9hpehcMCG0oIzP5/O6u7tba611d3f3aGsxKZD2+ILpa2u2" &
            "cWEZbzvnRO4URzC+xXE+4eQEZA1dz5THNtrd6EsNrMXxbQ2olZrDjT0atl3zTTnsb3shfsOajI/ftZj/tnB802Lf6xy/133pBkBY" &
            "P7a2f83nHmg18jy7mTv5Cdo95zTXbTSfc6dxfl63wHMkUo9rIt/nytwd2jW4NMemdWgvtz/K6iP1cqTdvIU1y5928gv/6BDeU/bH" &
            "/vjXeGn0Yrqdym+KNlv0qDkhvikXeWmMaycwMLfF+0heq50aGdNuP8FavVat32wtT9b+Fk3Huy3U1nzGxEl/sk16Ew7cDLxPPb6l" &
            "RZNjo+2t2C7llzu5X6Y8aZxHODxOcW6ug2ia9N3SDNrdN8f+Qw8Fcto8rNHsXkNyc9Mk6KeOcwTmmOfc9jec8Krf8c3W9vy1PFnn" &
            "zD/8tqfX5n5EGzU+8zOQwEXyol033lwybhzb6OOkdxPieNqQjon/2vHYXNyB89hPzgRrk2/btWbYb723QdOhr4H5cnT/bdutegH7" &
            "Dfa/jYbrMVwTm28G4TPul4G30Wo81xbwWsvR/daaP7C9xexuoOxzbA367bOtnac13GivHYl2A2810O6x9Q+9u2c/mPoeU7PVH9vj" &
            "t7CSkO3QCXv9+vV6/fr1GxNg88lNIvrZWlxyt4fH1OhvMG+Vt3rt6L/9Ra02drsVkxZ9zW/elJO+y+ZjSjfyds1aHLu+qV2DOS22" &
            "cZq94dZ6Gs8XYuOlZT+1fcXjGn5g3nKtKzWk30BOQ3R5TN+1Wct21+P5NTg2cS32bfZsYoJT+ZgvcD7GrJLLtsBj2pt+y2U784Xn" &
            "vWDdyWbYz5is1/gOZGliWcTcVIMU7MVkn4VQk42a/igsfMKau2bYtzseQ732X6txGrdGH7k8EvSlFq6neW0+TdeYOK7ZtVrfee0n" &
            "Jp/zNE7gPK2Rt4ul/RY43s3gjSzxbwvH7PJdg2OnceObSzS/ebT7PjRdb27c6+S1I+H7mZtBramROzX6rTvhmq81+81dw4uSIJzH" &
            "P6boB0HDoW9lTXAhEybONCH6OHabYJ/jGM/jLY2aDYfWzrHuM27SJJrmuvLKymh+x3l8DeRNOrQRLdat3SCyzru93OD8tPHo/q5O" &
            "21f5uUlDi2269C/d+OybYG23tsbm2P+2sAaP7rcxW6u38WNbwyv3nX+nZZsbue6vsm9bfOD7duMYTWuKoXZbg8cHCI0kN4FWQGJj" &
            "a/5riDZfYbRJtmPjpe72amVtFq3Zk8O5mt83CNdrUHPiEI3j+na1rg2ffsM2xthH2MeYKX6qxfW6fV7sYl3vrc3x6fN6MxKba8GI" &
            "zXl83PUznuLd37Udrvnbfeco1w/9RurgOxQejWZbmwfvUc6Xc9rnPrVW+QRgAjnJ19ZmWkdq875N0Jaj59uQuM/8Q6njOB4/QuK3" &
            "F8742m7+ttTlclnn8/kxnv91sP39qUzCDyQjcWzhtsWyfaeZk8CT2LQJ1sGxEV1/zBdMcYHnzMZ6zV26ABznNeMaRJf+Y7ho6D/w" &
            "N8ZWefjv6mVM4mJzbOzxMbbZGWed2Jp9ag3mUNtjr0PA/Tbly9oQ4THWeR0z5SKP6+lzv7RvGBcd20/65k9r1qNObNwfh769FbiG" &
            "xBptzaJ30c8GXRc1WFP8zut+5tpqbHpem4C5rGFO7NGPjzzCa+scGXMO55bMEzKShMkSN33lNgsSe4OLczxrOvAqzXY3252TreWx" &
            "n/b0Xbc3wKRnrTZewwYxZ0LjWePa2LiFR47BuDSvvbkcr7IPzaPfPvczdk2tOb6NDeefYH2fd/ZbI7guBPlZc+/fxExj6zoX9z95" &
            "4fBeE3gcrtsOOx710zfXe2rCzt/mFjCfj+63WsPx+eK9poG1hN/Oj8e223fm/ylvgg0pJtxM0N+cCqzLI3ORx5rcpslYt/m90Lu6" &
            "psY6dn3aWg1phvMYjHFt9i2sa1s/2yd92+lPvOe5ytv18Fuf+s3u/tJNwfz47bOeNRvI5VrF13gtz7Vmvan+FuO6gvB4nhifMf3U" &
            "cr5w13DTtz5zcNxqaRzaDMfsuKnV9RrxX+Oe9A6RfMZxLlzrz1MrY6kZn7mxeR8xv7kZG65l5V/aejJOuspFQpDfkhAsuME1BD4B" &
            "5Bq7xc+xzdFgLa1x3Vhf2ySse4K1vTEYb27zOda29Hf1slGHtqZpjXbe2tjzDsf1OGdAP21Gs12DtdvYmOZ9rRmx+UZAX4u1Zo7c" &
            "pwfWnXzvK2K6blwL463NNtXAOlzPVNtSfa419wbqNzR92qxrMMfbtgbe08ydYgL6nWvK2WKMt/oWlnEMN5op2bphokub0Lb0d+1t" &
            "MM150js2c2bcdIE0vvU9bvwpt2N3fdsOXcTThUU4v+fYbIxr7Zq/tSmG82n8Kc5wjH20W8fa5juOfd40HU+/m2Ft23c5yG19wzw3" &
            "wtdeu+maa3/jNrtzE43f/OnzSLT7qLXdyCHMWWW9JjC23X/MZYxBm/3n05U/QZLmhfHG9s2SBYdj7QmM3fFWqTM2z2dtnuCOZZ+x" &
            "AWM4x8Zr69JyMobjBvMn3wTXSLBm10lOs9s3ca7B8U0j9lYrG9efXB6bhse75rramEfbmj+NdUz+cAjGOO4anGOXP3ZfaztYbzqS" &
            "9zZwfe38Nt3JNrUJ7d7T4tK3nX423oOdo9kYe9GnGe0+yDiicdaDxrlN1BfZGwGbb1b4RJGX/g6tSKMtUNsc1uLicvE4x11sYK7t" &
            "Hpvfmv0Nze4ctNvn89zQ6jKo6/1hTvo7PWPiWXMH5vOc3rbtNIxmm9C4tvmiTz/N+56cW8CbTtsfnm/LFR7RtAzW73lYL+OdruPT" &
            "phxGbI43mm0H8pmDtqYZ+3SNNbT1ob51fN7tvwVv/EOp4+Ep53ckLKBNOLwLvsJ6wtc6w6dmmyzzsbULKZgeaH6YZWxuxu1EcTzV" &
            "3fJQl2BO29K4bmzUtsYqX1QgWuzEXeUBe9H/LMjRa0E0fdfBljyMZTPs57ov7MXYPKdmp8/jgHNuvHDpa5yM+dV32i7D16+tZT+/" &
            "Rh9Orrtw2FwjG3Wdx/HtyzjWd0zQzkF4zMtfDXAjz/0cnd+5HLuzuVHD83fO2ALvTe/bQ+tL7oHrcJpLfLG5TuqxkeOWmMev8Tph" &
            "Nl3a5Fs6aW+Dpr1DuDnyQUcta7I+ztcL5hPg1rQIc1epedK8ZnO7lIcqY63hcWuNM+mlTRuW56BxW7u2/s7RcI3nvWKu984tsEbG" &
            "Pl4Dedfm0eC1Ssu6kveLwjncch4nvrUmXuOH12D7LdpTjgmOSyzHbR+TY755vh/xPhLOqfzaxHRMf0LzN5vvs2/8HojBi2264Ajf" &
            "JKd4FxH7DowJuJDOzTbZj+FEM+7zwPGs23nbDXhXg3k5sm8bx8xpe2us02tFn7UMxvjisGbz3QrrJZYa3is7WKu1iRe7j621/UCd" &
            "tbkGpta0rMlx85vnxnOVPn+RdleDddJ33uiwDnNon8bM4T0WHvk7WG/XGMO+OTxOtbmf8cRvNdJvm3UmZC9+5mcgxvS7He2m2Oy2" &
            "0b7rZ+yJsjGGdr7ds6ZhLbY2F4L+9HlzmrCrnfU0Dm2t31rzEc3uuU+NHGLK23jX2jXeTov2NexFahCTnTiu3EjCmcYtttmYr7Xc" &
            "YMMx3/1cH4ljjLkZT3XZltb4buR4Do1zrRmx0ec+G+8Z1msP8AmOneDad7WRF/ge17i2WYMco9mIMx8QUxLfTMyN34vPPv1H2RTc" &
            "0M5vv7kET3KbF3nmBNYMJvuENr9otGPTd53sM9br6RY4n/1EdN2sZw3GZXwLWBuP9FM3cF2tHtupZT65nrtbeDvulD9968RvHWqF" &
            "w+MtiG47uobwGZd+4wbWnPQnW/JwzuYx3jDX60Ye+x7n6AfHNObHSUv6jonftqab2rIe5MTOettcV/FlvLNPiP+Nn4Ec2rRNNH2i" &
            "+cxpG4G5qNMa/U1rlcVPjPnmBE2z2dNStzm2+UJI38epRavlydG+iU/uLXnJ3yF61uS5NW7JZw51uXfI5bjtLdZJHzUZO4H5rDXx" &
            "DNdrsG63KWYNOdvY+d1ib2uy47a9wBjC41VejBLO60aez6G1YmMucyZbA2/41iXst833JsJasVmPcE0Tb+HB1R56C1qXy+WzDxC3" &
            "hgRzzCNBLV7M3lzm2baDY2KjfafpPtt0MVh/0vWFsGsTZ5pba0TGnMO1eOawDkFeGtfpGrdxbG9x5k/+CY3n8WSbwDpom3LZFnvA" &
            "Fz85ut0Kx7H5lWyDYwzaDl3jre3Q/I5vHGN302v9BnJ8/RlNl2P7p0atHcLhDd65JzROsxHeJ8x195u/+Zu/fXd3N36jKX9tNwHv" &
            "vvvu+u53v7uePXv2xqQ//PDD9cd//Mfr8vAW64SvAx8PG4u6sbMwFkqe4RpTH29ejvOYuVve9Ftdrs0nwHNbWnjHs9/0A68X+y3O" &
            "8UT40WF9gdeIXOYzHEdbdMgx6DeXN6hgV8eks0pcOJnfDtZinluRNdyt5RLP2OXLXPxQp15bH+diLNdomrP5OyTPpBV4Ln5hxps8" &
            "NaKfuVrHfPfbAzFg7XzxZC5jAvsdYw5tnkvsPrrF7rVzI49fB2/+x3cg0wJcs/sH1i2R+7RNMK/xm86hzdF4tttv325ujX8Nzs++" &
            "NWxvPvPsb83xxGQnmp79xHTzm+JXydHOA+2O47jBOs1nTHajzTdw3qmGa7iF3zjO5xp2jfxJs/knWJc265hH7j9pOGeOrc6GibuL" &
            "O8re5kOfNj/wnScw12i+nd7j/wMxKcVfa54kdXzB07/zBZ4MfTvb0sc2zNP4tLk/1eu5u12DNWjPkVrUdC7r2N+41iPon47ktUZ4" &
            "PG1282if1rn5d3Ac+c1GvK19wu5V96TluibeKtwJnC9rudameJ8jw/zWbuVNMczFo3VtM3fyp3mujj3pr5Oba37gex7h6+YWkOeH" &
            "x1RLs014/BrvFNCSpJ9YHnd6R1l4+jLmsXEnfvOnJW8DF9VzWkM9zX6NT7/rss06LZbxTSd2Nscxh8epo8F5di18Y+LtxrFZw1y3" &
            "nd+aHDeem2MMc1tMju36mfqGc5jLOlxT/G+LtrfWJhfbtDdbsz5BGzk83hJnON51TI3g+Wxz8AOj5XTMUlx0Ccc5T2Ceda5hfAfi" &
            "3//YbWwfG5ewhse00++xbeQ1n5s5QVvsFjOh+Z3HtbhNG9WxHk9x17jExLG9+ZqG48IzfxeXRjh+16a5tnNtWGtqjplidyDHcc3e" &
            "Ytp5cUzQbMHOFzRtHtOmmrKP2vlpevYR1m55aJtifEO+JSb25ids99i8yXaUa8n8hlt41Fzlfs6az15UNiLvLtKCQycntms6xsSd" &
            "fLG3+uPLkf3Gdyz1qZF428jLseVhXOA57hrX2Rfc1Pe5ci6Pm/2WufgYMD71pxm0UbvZXENr12Jpn5rXvPXNtT2+9Jl34k+t8TmX" &
            "HFmnNVocx7Q1PrHbv9Na7dbJ+Zz3Fv2pGcxBTeoZtFF3Oqbve15ymbez8++meb4TkvdUvryyi7MvY9sff/OlPSC8kCf9sgy5Fvbk" &
            "XPxu402wb9KmvSF8n4TYLvq6MWPIcWx4jG2coG0qchw76ayynvb5XO20bD+GOTdwTuQxPv3WrN1qJ881MZ5jarCeay3wXJg/Y4K5" &
            "pxqdq/Ficx4eGz/9xknswvma8pPb6pw+57dW/LQ7hrkaTvhGZ47uuz6Po0O9wLXs6nHcdCTPn+oELR/7qfPQPB3D+Zz0R3FdE3m+" &
            "Nt4Gj3+NlwnXQyImZtHhphAWkEmyz8m6hcdxsOMmBxc34GI0e9Njn/rpew6rfD/cOu6Ts8rJS595rHOgNoI8bzLD+rvmeadRZ5W1" &
            "dWu5G+z3ODafE4M2rmlbl5Yr3Gt/AdY2N+5T5ks84diWg/VP52YX53ivhcG6vV+X1pmaGbsWcm1behcacA7s02f+NC/XtHQdt7ab" &
            "c7tmDXIC1sCWWng9sT7GNrhW2uizhnM0nYD1vvHHFOloNm48v5UKt504+x1nLvM3u084OS3Gfi+Mx8xxDeRkk7g+1+pjA2OsF//E" &
            "99hc+nk+0pYeCI5jPO23NscQtNlnxM9aHc/GdWQ8+45hHLluPkdcyxbDnOw7nzmtWYP91OEY6qfv68A3nQZruoYpV/rUtq+1aW0C" &
            "89kmXsbNn3zeA9OaTJrWDyadVdbjGo7yO2bTMcjc0nee3NMC+x9/BsKNlr4X7dDncBZrMKdt5lUmZoR7S43UpY1jb1w3ImMep9Z4" &
            "rLNpt1j7Oe8WM43tM6LZ6jvKOq3yrY+m7dyfp90Kx7mxXh6da5fTXNtuabfE7LSJxm32CS2m5bpma/5rvMbNPjvKHqctnAbHMBd1" &
            "mj457pt/C6jja4sP5VYH0WzX7pU7sAbbWTPtOzz+QMMT2QXSt1uMnUbQOFN8xpM/oN31UKNxvFnM53Fqk79p299yOM5jwprh+6Yf" &
            "btOybvN5bJ1d23GJnS+Ini9Sx00a9OdInmtw7Y51DJF41/t5Wstje+MEtpvT+oyhjTznIZrOKjezcNo5pT/Ht83r9SeHPDdj5wus" &
            "0fKkf41/CxzbdNqLQo+nWOPqn3PPA4IPCsdMSVviU/lZC7kc+yQ7b3Aqn0Eyd6sjmPJlzBsvfS1fuK1Ox0zNNVB/GnudJl8b87wy" &
            "1prWJS9cazueOte41meeqTGW4Hg6NzymP43d38E1tuZ5Jc7ngMf0pzzkT5y2ps3mRr1V3pHGxx/i0k4da0/+dcPHKWs4v4H1p+b5" &
            "G8fwHxjDZWzmT5/16WOO1icmDTfWON3DCepNyLze/PvDAkVODzf+u7u7uiF2i0lwQzHWk6YvcYY35xomb022iXOtJtui5YvJHGq0" &
            "Rp+1mybztRy78U63xRjkuDHG+ZyDvlbjpN2aa9vBsS3GPudwf+IS9F+bj7V4dKPfut4nXMupLq/3xKGNNSy9uLNvp0Mfj4wjGNNg" &
            "f+M6tzm2s+62TozhOoTfYK3oNJjD8c7WHuyB86/h3hu88REWjw15iPjraOm3h0MDc7XF8nhabMP5uACs1/ocO6/t5lyztYvD48kW" &
            "val+xzg3OamD9TS+W+OkFmubF59t15pjUsNl+ONun7d5bsE0Ntc2+w3GXTsHk33yWz9wXBrPnTW8vtSiZoP5TYtj7x36HdtyXrOz" &
            "8d5k2OZY+4PsycZpNtrju/W+Gbgu6xm+fzQ4P/enedR6/AjrNLw9ZHE82YlhPB8sbVGsZVsa8wccexITXEOOtgXO32ryOGA9jZ82" &
            "XSxGs3NtA+s30G5+zmkuhGv1JYb+Sds5pljnIi/tVjim6ZnbYA3bLjf8jgVj27jxg2Z3nhZPmzltbHtrPhfW4JigzdertRzvvIwh" &
            "x/HmxBa4jsBau2tsauSmT0y5idyzqLOkZR/9iY/GrTjKr2M43uNTPsJi0lZAbi5pFkoyT6JxGt+2o9zkj4cNFSQHb0gN1qWdx4UT" &
            "fI23yrsi8lLrdOE1DnU8Jscbw5r0NU6zt99wdRx96be9whzO6THt0XSeppuxG9ebMeaRQ3/60cmaUIt92to4aHMLXBPzm2M+/W3u" &
            "7jee/Ywhl/wJ1tnlaBwidmuw7/rauq3ygmsC6+FxWrMced8gb2efPslp9874AusTzuXjZAtS21Q7+4sfYRkhtZNCseniaMWtjd2I" &
            "NifAfOmH23RtT9+6hn3WmHQ+b7OOa7hmjy9+gn85oMWYT5v5bj7vtJtL5IGz40569BkthrGs99a6mcs2+loN/0k01uB6WKN5zb5r" &
            "5Lcb2Xrwe0297uFZ2zdMcq3Z9PNC19wJvGGyxsvwzcj0Ezfd+AnXuq48NK6BdfjoufqBcO08BNYhjuOYHyBBKy4nyCfHC8Hk9LnI" &
            "gFppnmirp2kFLY7ajjd/6YZnf9OLnXAM52Udxlp/Wgu3NWxOat3f3z/a40sOn1+i6U39a63lmPoeW6vZmj9zbOtJHuEx4XjnyNgc" &
            "ajLGOo3fQH7G7F/TahzzqDfB8ZNWuOyT1+4ZXs9W76V8vMgx4Vg3c4l2fQXWcA32tzi37I+dXvyNk/6l/CI4ORPoT//qAyTgIjEx" &
            "3+aH54K40K0I26b2i8J61m19clxDfD4JE98Xg3OQb//lclmvX79+XHvmZAyPyecbvjWIyW+eLxrX4Ua02t8Gt8Y5v2ui3XETHHdL" &
            "83ztn7iN32CNXZv0afe+aHyOG+8yfMQ74Ra/62uNfMcmrp0P+wLv82Cyr7I+u3FrbX62RSewn75wPcdbGuPdP6Z3IEf52llDfJcr" &
            "30QI2tOaxezaxKXOzj/B3Clv6xP0+2gb7ZMtzRc213mKtc3jXQtvykPYTn67SFtz/K5/i15rhO1Nb+LT576bec23s9NnONctjTdG" &
            "6tK3a87rsfWs2TDpOMZ9c1rfrdXUmmEb8xjWSt6ddjhsOw3rND85tl3KuzP6HW+Qt6Z3IO2dAltuZCmcm/OkH8I4voG6znULh83+" &
            "HY7yA7CG2M3hHLlmrslt2jBeV9s55jGNcM7Ga3NP3y8gHGu7mzkNzj+1cKZ539JuyTM15m3njaDNR8N+8hzjWlhPxtTheA2vmFv8" &
            "FENtHm1ja9r2XauhYeK67/M15ZrymZM2nXvGOA99hnkN5jRN26xnnjHZJxy7dyA5ps8FmU7IGjYqJ8R3IbRPxTeObWyuaw01BYmx" &
            "zjVY2/FTu+gzyta81uk7X9Owts8ROau89V3K4fW8tXmePh/J4ZtUWsZrc6P7PI15Jt1bbdbLmGh2zzlHrkuDc08tXMZZezefa7B2" &
            "a9Gf8rTm8zHl3KFpUo8+j1sO6rSfGzRYs3F9HU+IP+ev7Z3T8C3ICd5jrK/F28bxG3+N1zeO9fAtHi4Cg92fOLbT70WJn9ycNPs4" &
            "zkk16HNbuJAcb176J/wp+4X628Zq+RjXvi6XfrSmDcOY2Mmhn3PzOW4aLTZ9jx03xbqR22KbBjmp5ZYcq5znKcdkC7z2rT76zVt6" &
            "IAa5CVzwbrPp0NZg/lH2kHXI4/UYHmvjfL1Gbl7zaE+5GOdcsbMe52Gzpm1Z513sLhfjyGd/0qSP8LpbJ/CYsN01BNwTgXMYPGdp" &
            "j/8PJGCyJPHJbsnXpliCPm8e+4lDJ80bLJwcXYdt1/zTxkm/1c5xNjvh3BlzLv7ZA7Udmzzm8pg6kiN88hbOMb/2G7vzUcfjaLY/" &
            "l9BqJLzPjORjXtfg5hgfXTfrip0xboT17HfsTte+qXa25EtMyzv5m5ZzczzVEV2fQ9fgmJ0uY6kR3YbGmbiu1XBO2i/l55LX9vEq" &
            "e2HpGmR8jtfmEVzKi+1cx7FzXWMjwien8epHWKsQY+MmWbro47cvsS7Gi9w2C8H8aW3cbM3fNJ3Pdo/Nu0WPcG0tnjFcL8a1GOu6" &
            "XYM1WJ8vGsIb35jiDOeeWuN53HzJT40WR5/rJt/xTcs88+1zvuR0bmq5DjfGt+ZcboR1rW+Nyc74NgdyEx9Mvqbb1oBjomna13Qb" &
            "f8I1rq+hHZ+5pzW7lG/HudnvebH/+BFWa62I2Dix9pBIgsB6RmzWuMbNcWreQLS1HC3fBG+ctall4hi2eXwLnNvt1vOQI+PSyCHa" &
            "XtjBte0a+U3DNh59zhuv7RVziKbVGn2uY2qJ4dG2W1rb8y1+0iY8XuVFIo+rrPukO7XGS9/nyM25Dft8bLyp8drImDrUnvQZwxfm" &
            "1+A8br5evRcYu8pDi3DMOcYmuMoGCDj2JnJMeJPPeRvPoM1aLT422huHeoR50fIr8cZxM8/5PI6NR6PlZExrjrWvHZPDtqmRc43f" &
            "zg2Pl/KxINHy2u7W/E3PmpPfvgbvgaD1ycv8Wy3mhp+j955jOCao5/Gh/4keP3ktxprryk0r8IuTwHqTf+nGzMZYxnHPtTxG8zOu" &
            "aUwtmB4kTYc+nu/7h18aJsdj5nE+awf1H0rRRt/SSQx4IoLEeNPaZh2CHPImv5vzuFHDMLc1XpiJsYYv3ou+zdFqdH7Gmsdc0fW3" &
            "RajXbM7nFr+P6bf4yT75OEfPMzEe58JnfGtca6/l5cpbeura/3lgHdqa3XW4HoL8W5tj05/OQ4ulRo5NO7rW2fFbi8bEbfZms7+B" &
            "sTlOMddszslz6n1I+GH3NnBOx08PjGZr8776DoTgBHZ867UxNwHhh5CPTWuHa/yjLCo5jnU/bdoA8ZF3y8VJfjtGm3mobx3yrWON" &
            "Zm+wzhTfuLTvWuPRxiNhn3WbXgN5t8712oXeYgzXyPzusyYerWM0H227Fm47p/Hl6Dj7ybuGpmE0e2r1usVutHp2tXocOJdtXj/e" &
            "X28FtX3/bLkD79G2b5ttQefuhz/84W/nL0OmBaeHV3h5m3ocx/rGN76xvvvd764nT56sE/4a7o9+9KP1p3/6p49jT8SaHAc5kSma" &
            "i+lvJsTe4DlQpy004QUnqNOQuU010u658KvB8RPWTnxqpZ3+INqubymGNVDbPp/nHA9ckIknYmt52oVMMM66sVmD+VrucDIf32A8" &
            "7/AX1ouaXAfCa04/Y1p94Uzn0zEtfvo6vs+Vteyzze92zeMN8oSvu2ftpriMGW+u23QOqZlj6iDodzx1zG3Y1RE/j+ln7HPteMP3" &
            "7qab+XrtG1x3q+f+/v6zX+MNmNB2F+fvU5vLYuPn8YI/7Lerp8W77/HEp5Ztqcljt6YT+Ma6NjVM+cxprekZTTN2zoUbynlcT9Nu" &
            "6/I2zTW5kROePw60n8f4PD8iN+r2IGAMa/KcPW4a1LbtWlvlgcRYjnd9Hr3WbM4fPn/+xzWO5q4xfzA9SKnvFljTvBa/47uxjnbe" &
            "b0XjOtfn0Q0Y4/onzg5+4WLU/weyyqTS8vQKxxstoI/xjZdmW5CcE7fFMJdrsI412DyHaQNRL+DCe13Zd474mr5z+Zhc5rYW0OZa" &
            "YnPsLn7SeRvOrS0aPic+pk4ep9rTGsd8tvvyf1XcqOmja3ZrdtZHTOMWY800r+3ka/5wkqPlbDY3alCLsL9pcG1ZLzWYj63BnNYa" &
            "b135FlRrxmTb5bzma5pB8x0Pz4LH/0hIx2X45kEeMHw7zBOy9PaIaDfQ2NNcKMfRZD32t5rSZ41EbK0m+qIVuBbz0qLHI+Pc2vlo" &
            "OZIntoB1c97keEz7Upx96XMujkltOf6yWvJR132CNvuI8Fyv5+x+47c+9Vsd9FHbjXbG5uh8zd/6Dc4RfsYtX2vUomaLuxW8Rqid" &
            "mlwDz8cteW6tyTm4pq15zuEGvE9MuY/NPeLa/FyP24RWT2p441tY3BC2M8k0Zgz9uSHHF8Ru/zShTKS1wBuonTTC9nZyPKZ262fs" &
            "eM+VcJ7YbHe+lrfZrdfWkhy3+FzHUXK5BvsXbiDXzg9BjanteJNWxva7NvPZyGtzs07TeNvmWu1vtuY3rJ2x52Q/wTyrXJdTu5SP" &
            "zVc5P4RrsJ795rRYo9nbPc3zjN/NcetBj1+Ltt/xzZf+DofWudVmO8fBKf/S1sSG4+FnHa9evfrMzzxaPHxmYecAAFCdSURBVDda" &
            "8wfNFrhYHoOm7/EOjnebeLTbn5NDH49+iHjsHLZnbfORSWtee19IPE43Bua0jvM7Hxv13Dcc69zTvmr53FocbdRPax9LcZ1a7VP+" &
            "qTmn/a05h2F9c6xjG8fUuFZjm0uLZT+wjbqsyfmNdgMmvzXyaZ/m23AtLhwfW1u611HXvHCneyRt1oiNzefAoO3sgo7yzZOMLw//" &
            "1Ohe/y6SoozbFRUbx47zQnh8CzwP2umjjT6PYyNanNF87aQ7j+MyzgZN8808a2sdN2owV875rc0a0SGavY2n1nLc6rONdj98s8fb" &
            "P/DyOk1wLsYw/1Tb1KjvXFPfsYE12G9jP0idi2vV1ss+N+YLh/ljby9aJjS/9ZjL9ZofXtMNGq/ZjORsvoW41EzwXrKDtV2Txy2G" &
            "eHwHQrgYboYUHg5t9vPtGMdNe6kO56M/8fGzPt9IuTBcFGJarHZTzzysw3rZ5/wSH741sjb2ZV7Wcs602PKgjza/ju0ctNHnHEuv" &
            "FHd91mPNaUw7fQTHWTPuqSM/4Cv7zXmiFR7Xeq31Rt9xjo0t/F1MWzPGk0vQzvzeky02uS4PX6P1XnNu2t03L7bYV7mPBAd+nkqb" &
            "4bzGrXUszJ3nc+lewrqsGa02H5976lOT/MSEk8Zz2TT9Vd3j4VccMg6ik7zJ6bmF55yJCaid8ZH/B+IJXkNORApwIiab+i2fJ8u+" &
            "ddqEGqgVTuK50Vrb+W5B49nGcdP2uNlcW+bl+jlOfwfrtkau+7bd2t6Wn5ilm6Qv5Fua+e2iM8f25mPzeZk0CHN2x1ta1qe9MCEm" &
            "OxE9wte3dTi2r+3d5DB3Yc5Tn2PG2Gb4vDe91uizHtE4sb8Nwre+4Xy8L75NTs717KdUI9GfJxUvUt+k2W5BuNw09HkzOW6Xx4vq" &
            "2qextdMPz5oTqNPaNU7Tcj9jziHj3ccOsVGD2my7NSLamLGuw40azm/u29obJn7Qrgmvm/27NuVJvx3Jo21qXmvGMH87H7fmMY/H" &
            "5mt9a1Enrc2FHMLx7LvZF/h+QaSeSePz6LlPG0GdnSbBelw379mBx0GrKVqP74f8ACAO/TzC3GnyrUhy3dqG9gZyzqZF2Dc18h1H" &
            "OK7xf5mtodljS5xvFNYz3+NpzXkht9gcd+dt52N9rdk/cc2jbdqTqYf71nvd6zrNofXNMc8gn2vmnLS1WOvb1xpzOc4abRxbizPf" &
            "cZP/Wtvpe83sY95rfY+bz7aGts8amt5peFdMeJ4Lv7HullpYh+OttfLn3AlfMIHHnkB8Tc+LE367GOJvfLedVtAuvMQFnDNhLv2u" &
            "hXXweEtbuHnZ7jHz3wLHtz6b18trZq5t1Jg41m+c5DPo55g63nPk+4IN17rx2ba0Dq32S7khUZ/cBmt6Lae1Y6zn75zM3TSp6zHt" &
            "zUZ+4xzDOrSxj/Q3n3nHpoa3aca1e0b6HtPuPeh9yxjjUh7szEd/wBx+eHyeh8hxHOvuBz/4wW/f3d09iiVRjploAr7xjW+s73zn" &
            "O+vp06dvFPz++++vP/uzP3vks1AXNRXmmNjCD5oeQW4w8Wln3euGzcBY5zzw97uyrvS1fnJbi/XY57UjMqYv+tZyv/lpS86GqUbb" &
            "43OfR8Z5vVvsKnN0XDgNTdMx1nYtgeNta635bOO4aXIPmcdxQLs5Bx6IzW/7NKadtmZ3PPk7XvO1cbsWjEMPnoB7wOvCsbmugTyD" &
            "uWmbanJMfC1n7NfGx/DfUd3e+J/oE+IPl5PLzckXKWOajTq0+xUM/QHt9lPbT2nC8a2F53poz5G6XPgdrvlXmY/zuqY2nmJ38V7/" &
            "FpM4gjbuCecnL/bw2Fp+2jOm1vSDYWtNvtYmTD7mydF9z2Wqyxz7W674msYuvtVpDsdNz3HOcwwvPszf2XhM/9q9KGP3WRfbKu8G" &
            "HeM1o37TvsVm2HeNM+kQjcP129WV/umEP2VyaHNQJP2I5xU13ymQ58LoTzyL9dso1xINNp40g9o7DXKNZlt69bK0wWKPz7Vdq8W2" &
            "zJE3RN8czW3NuonlWruFkxzhc10cyxhre90S7zycz27s2IDnPpqMJXY6tLW9wLr4RQWOyfE6UZ8c1kSebdZYqtNariFtird+NHik" &
            "9j2+Pu8ardPszdcac/sc5/d3Mi/v14ydNxyuSzjMwbho+aMgclMjGzWbzXNjPfaFz3p59DwDankPWC9zZFxsp9Pps//SNmCRGZ/w" &
            "IJk+mgo8EbdVcgQt1n5ybDOv+XYnqLXAeQ3HeUM2PdsMaza+fa1xnoxzPEH/BOfZ2ZuOeW7BdK6i4aNb7LxR8NwYiXNO+xvHXIK1" &
            "XGvkM/7WxhuEdQLH7NoO9js2ftvYpnNsDvUmzWC6zzQkzhr0O6/hfK6pxZBzbf7UcQtyb77lHs18bW3XZk53P/jBDx7/H0geDO2d" &
            "RcD/B5KTfX9/v95///31ox/96HHDOmFDOI3LCewm02IJ+6dFdY61ibXNx6a14IvfPGrbxxjX0DBxnLutZebJXD6mzzoTF47ncAsc" &
            "09bEnFXq8/w9znmgFm+2t2Cqx+MGxrrvOuxvPvbb0Zqxs8XGI3nNfoufY/MmfrjNNnGX9qTzE+E1zWP42Zb1yLfP+7blCfzAWEN9" &
            "gfPm3JrrPR84H3XYmCdH9+9+/dd//bfv7u7WSR8lBRyfTqf17rvvrm9/+9vrfD6/8bbVP0S/BafhKclFCK+BcY3Dk9B4LabBNTqu" &
            "2dt8eLLi91wn23qwZ06208a3067VSJz501wM5t3x1sbPddnBN8Gck0mXoHbr35K/cR1njV1tnnfTbMedzfb1UEPzpz/FN7T5TPxJ" &
            "0/lo97H1zWffR6LZghZ36ObsG+8Ea5BPn/XIyzljnDkHaiIn8YFj2fiw4Dhx1CCi//gAyTex4jh00z2dTuvu7m5985vfXN/61rfW" &
            "Cf+N8P7+fn3wwQfrz//8z9flcnnjgePmAmyfCm0LEzStiddqiY+cZnMMOemTl5OSftar6QTkB84RRN/nysemlTon3wTPaWnNGnY+" &
            "n2+D+X5RcK2o2XJwbZs/GvZx7HlnbK3YYj/K59w87vo725LWNd2Mp/ksPNSbno8N9kWHbeFmS16L4dh+x2XsFyaxs/lmH7zteSKH" &
            "muY2vvO3I+fS9mjL64cQj4xJn3O++/Vf//XHj7AWbgZu8X3jG99Y77333md+sPLhhx+uf/SP/tFj4hYf+4RWvH2sxcfWDyafebGd" &
            "yit9Y9KkL8g6tbqNa3knWNPzbHU2pFaixZkT7Oxs7cKdEG6rg5hyE66jxcTWXo0Fjm9HxjV74zgXuZy/89nebNFPm86B+Qbt9Duu" &
            "xWYOjjPf+tZyLvY99jzj54s6Ht2oNe1B82jnONjtrcB1MId5hPeJY11fjtfWyb43/h9ICFlE+5KgFUIf7eTFb17TJMf8tHYim42w" &
            "xkVv4Rqn8cMz3+Pma8fWn9Ygfo49j19Giya115U1ZmzTsFaOhnVaXe63+NYYO31rKI03lPDvH/5Sb+wtb6uP/VvbGl4QBOFQf5fH" &
            "nB3XMZNtbW5Y5IcbW+NehgfaupKDsM+5kmPiWIs4DS+MjZ0vCKfVYfsO9PNn2A1NL9xra896Lw/Xwpu/4YYbEcEAf1UvMbmgVlk8" &
            "cr1A2Vj5+p03WmppJyy6tu8wcaOVnKnDjfyAG4oaE5reUm3Uaxve/FvhObBxzrs1iEbrO848bsIcnYe8Vd5iey84r/VaCz8t+481" &
            "ZN0Dc1ue9VA763BtbuROLZjsyUvckmNp/cIhonvZ/PHFNqY+7UFbkxabxvpac12OIcinj/vzwF474QtGE5f2xBLm2Bc/+97r8ZNH" &
            "jmMd1+YcmG8b82ZdzzGwMQkXnxwKRswLnYX0iV+lsNipG7SJsRbXl36DuW67Dcpm7Gr0mLmcz1yPWwx55nvcGjUn/caPLmPN3TXy" &
            "W97oL1yU9rV2bc5G7FN++xzX7Dw2LpvXj3yvS3xZ/6bv8S3Nc0+jfmphnc4Vf/OZY03X4PWYNGm3xgTnYJzn6HsV46/laveFVfaI" &
            "HwKsh5j0bGesdVwz+02H/bTU9/gPpRqBvrXWuru7W0+ePHnjKWzOGp6awc63SsG2GVz0wPNh/8A/EPLGbWjxbOTt2sRjDd40jqc/" &
            "sY3v+djunGl+Z5lYakyt1UYbczt2F8c6XC/93lMtlnkC7x8fmYdxGTtP47DP2ltNHN/arGfdz9u81m4736F14Nyu6Tr+0DsBo/Ft" &
            "I+w/9E/FXFeuC8dzDrEH3FeuxWPOqfHMb7Cf9RFex10+rkNijcc/pnjgLZeFCHK80OYa9Lc4Hm3bYYprNdF+a/2TfZV5WJccI3Y/" &
            "0KzBse2xBdFovHDdmn6DtdyYk3qu5ZoWY66hcaxnzbQGx5nvo/u2TdeUa3GOqXmNJ51bWnJdykekrRG0e+/sWmo2l7rOsYP1r8Uc" &
            "5aOl2C/lo8ydVuOZ38aOWeUhwsY85ExwvPNMuTLenR/i8N/CInEqoPFyZMumXMNTz5rBpH0Njsu4bRbCdbdck4bjpka8jZZPpPkG" &
            "4w69crK24yc7fe4HrNN952zxa5O/7RtzOd7ldJxxS+waarWPdtucp/F2Y8dnHB6Pgce2ud/yNu6t59o2+nhN8FwHjWcd6hFNZ+Le" &
            "gt1epP6Os8tvn2Psm0C+zw05U4z5tqXVn4E0mDNxj+GJuR4W3/HkpE+7OW+DW2J3ftZwDZ6PY9qFkTFj/E7EuuEZ5rQbOseMYX7z" &
            "pjrIy3FXO7nONWkHHhOxMy9rv5bTaHW4ljbHa3wezSW/wXo5TrlvbYb95jqG9sZ1nDn0+YUCkfsG19ywnbkS6/0QOPfkby/+nCex" &
            "TScw37E7jvedY6/1p/EOjDHOx3BB7FoTmzgetzzWaf1raCc3mGpeQxxrmzZtq3OaU3J4k1rjbVqLaba0i96et/NALtv0YGh+52j5" &
            "mo5rYOO82hytzWYuNXx0Izh2Lvftvww/X3obOH/zOb8ba518tjX7xHc9trd3EORfg/M5rr1Azdq3j4lvyW+f9Ruo6zzXbNb1PaOt" &
            "+6QV2N58hm3MEZxOpzd/D4QbnsK3JFzyudjWt77zXEO7+RstF8dGqyH8rAvh2qlvbmDOUX5BjLHWSS30OScvJt683qZxP3jjci0c" &
            "R/+u31rzt/hdbY51reSv8kUQcgnGWM81XqtzB2q3nOTRxvyMdfs8XPOm/DuYS91ra9PsUz1TM4fj9M3ZtQZr7eB8zcd3PzvtqS7b" &
            "yDM3/oaJ/8bvgRwPJ3K64eQpHiEXYn702uZbV16RcDw9KByzQ2oJJs0FXT79fYOInfypZQ2ydry5M2fTIlJDeKyjNcYc+CaLefQl" &
            "xuesNcPxnPMa6uQ5CSe5E0+92Juv1byz028cZX6M57hdL1Ne6tp+a2Ms8xDx8RhwXzce8zTeKnvRcTymRq91+H6HkKN51KWPHHKX" &
            "rnPejH1tM1/siWm1x2e9tamFaD6O23x5/U7r3+DzHV6rOX0eyQ0v47vvf//7b/w13rT2t7HWWuurX/3qeu+999aTJ0/eOOE//vGP" &
            "149//OOajIUkORfDPv9ZlR04mYyd1zUQ7bc2qelY1mT9tPhbXRPiY6zjnTcwN2BOH8nZja/BNR0PmztwDbs+bUQult3aB9byupy0" &
            "52LbwXxiys++4yeOefHfyrtF1/ZjeKFEmD9pTn3biPiS94L/HeP4xHoPWMuxjWOe7wPmEd4v1nKM/bQZjp/6GTfbNHZ+1xJbxrlW" &
            "0tp1/ZkHCB8kEWGSr33ta+vb3/724wPkeHgF8f7776/333//jRj2m82TS143YrJNYI6leMfRnnoM1s6FNxwbzQV+m0vs/F2bpRyJ" &
            "S+PaTZj8rIU230jaHJfmtcomDMewnmNsc44Ga3DOTcdjr4H9QXh+FZgjm+OcwxzCWhP3wMXtOgz7yeH4KC+izHU/R+7L2NOoyYdG" &
            "Q4unvd3QvB93oN7niWPfNtc+cYPjhnUIvLYBc9nGlnU33+PYFj5VsP3MTc0jC2TiS3mLectGe0yoP8IYTRdOTYI5Wo1u1qB9bT6u" &
            "IVp80544bV257oz7RdDqXOUG3vJwPu6nZa24Zj6mtfktrAHzLOSyPnnce94z5NHeYN1dbo9ta7rpG9Qw19qZp3UYl3F4PrY4+t1O" &
            "egHZ5p4W0E/43NtPtPiMnTP9S3lgEhlnLrwG46dum8e0h3c17Bq5O61dDZ5H4Bpoc99wzrfB+dArbRZIeybJzyoTmwKmlo2YRaLe" &
            "hN2EqJ0xj0SzreFkX7tYWizH9nN8C7KWq3yjxrDPY/Om/ts21+WbzNLeaRs9PI95ZCNsm/xuO7/nwP26s1GbvClfxj6yMceUjz5e" &
            "i621OqztPI0/6TVubA3kZJ/4ReWuGdHw/WqVtY+/6RA7v30Zu86pTbE8NrT52cZ4azq2YZd/SSPzufvVX/3Vx38o5YuetgR8/etf" &
            "X++99956+vTpoy0fYfFnIK1gaoXjZq7Hydk0WlyzeXEN26mf3PYT8bs21hw4dt1Q3w7JsXQec7Qmx54b58GxPy++PLwQaPl281uo" &
            "ibmdj8eWgzqB5xm0OdLG4y0anN8uhphyUdO+gDd9+shx33w3PvSsmX7OsfO147W+0ezhO45j1u2Y1EufORx7n1q3odkn/QZyyXEt" &
            "a8i1dK2Y08Y7Hu07W/qXy+XnDxD/7GNCHiB3d3ePxbx+/fozPwOxFid60h9cpH+Ka3ibmGZbxc6Fa5rxO67VQFvWyrxwwwnaCaTP" &
            "Oo23Sp1rw13F5zyeV8Dz2pD5W9++6E76zX9tHbyu5nhM2zXtoOka5qRve3Ka21rj7fiOdYxjOV431OaxeY4JeF4d3+L8APW7KMYk" &
            "Lrz0yfOLotTBWOLAXnXOlt9gDLnT+tq2ruzNpu9+43PsI/tZx7vvfe97v/3kyZPHb10ZTvS1r31tffOb31x3d3ePJ22ttT788MP1" &
            "/vvvfyYxL/pD7xryEAl8EmlzHT7Zre34XhD6wuWR8ZkH4dzxMw9tJ3zeHITb6jlpDZyD8cZl+GbLrWCtrJdwLbSxTw3HtP0XcB24" &
            "Fm0dXGfmu3tVSl5AXXOOh3PIcdBsC3U1rYaJt5tHa+T5VTt9OTLO3KbtccA6D1375lrP4L1mKScfHn6wGC1maX82bvMx1652xxGH" &
            "1iU2XwushTzD82F9XHvGps81pP3AV9U957vvf//7j//Slok5IS7AV7/61fWtb31rPXny5FHkcrmsDz744PEB4htj06H+GhaDk6V/" &
            "qrFpEMzXbNGLLSfRca691UNcs3GO7QL3/BfibSemHC2GNvOu5UrdOzh20vR4lfVzrdQyN/ZVzjX91pxswenGBwF9bqs8DAxyW3zs" &
            "7ciYdvPNzYh8o+W0vnkL62M/93fz02ZOxu0GZy5jgsYLuIesYW6Lj53HW+0N7fpo/RzbeWxjtmbn2H6Og8cHCF9hpniPj+NYX/3q" &
            "V9e777677u7uHn+A9/r16/XBBx+sH//4x+vQxxCMjwbHtLdYTyyYNOxz7pbfOdPPz4YSS+6uH7Bejs2N/aJXRfZnPezbrQ/XkI3g" &
            "eOLE5zmbbw4RH9fU8bQbzMEjEZtz0Mej89J+DO8yMgfbqXNL86s56qRvn3m0eX8418I52z24ppzXWsC1eZt48q/FkWeux2mui7Bm" &
            "02Lfe/zYPBw/L5zH2vRf4zqOYO3Nx761zqvc0Ca0RLnp+cbHYltRbF74VWoKxxdd0yOP9QXmui3ld570b9Fkftc+Neo5puXLmJi4" &
            "rJlovkmDOZ3XXNqWfhjbeITzHDqvObb1beN2ZI6FffaLNNaf5jpdn7k8Njhm/ZJqP7AerJG2jFsd4ez81HHz3I/N/CdMPtduTL7Y" &
            "uR6Tlmvm3Ajf3wLzWnzGPj+G49awBtPY9oa7X/u1X/ttf77NfhCx/Cb6s2fPHu3HcawPP/xwffjhh288EScwj+Hcu8lYJ7Etxrq0" &
            "00/e+Xx+nJ/ze2xt+wPXwVpdr/lTLPk7hG+dwHkmzmTn0X3a2px9DDh2nLlryGlMnKa3Q26UU1yr3WPbG6zT5m6b14qwhtfD/th4" &
            "JJrP+VsjJp911rDXyJ1s1CKOzbu39Ik2vsXWcAuHuMZPXtd/evi9Htqu1Uhuju4//iY6T0q7SSTwq1/96vr2t7+9nj59+mi/v79f" &
            "H3zwwfrggw/e4KfwtTnpHDdO8l7jXat78lGraXCBvXjp72IC5mFsjlOM4zwHgxzX7lqv5VwbjvvXdDlXvnLy3GljTGpfys+x7QTj" &
            "18BNvp3PtdHXwDjaPL/mM6f1HcdxcMJHN1l7cwjnnmxEtMlxn3nN8ZjwuMFx1+YY0M8aiIy9Lyb+5HP82sS2a8lcj2Nj7l2bMMU3" &
            "2+M/lHKzWPrcjE102kTWnRZo0kgMT4L12o2kaTR7Qy421tMuwPhaoz9rR9t0Iw3Mv6WxZutHz5pExo5zi/a1WNcw/fLbLbYpF4+3" &
            "gFzrNx/trMv1edxazg/P067dwjnKusfGubDuxncjv9VhXcZNazTlTYxr5rGh5di1BnPehk+0++PnQWLbQ+fzoNVCW+6fvscSbU5n" &
            "khmcE7JwgmLzTdga4bTGDeJCPY7NX2kzjnJTij3Hy8Nv7b5+/fozmzTwPNn3wnFOSw8br50b/Z5z4zF3Wz9zYguYL76MXZtzE/Sl" &
            "DsasoaasU2LNid9f5DDP+W1v3PDb/uA8AsfSHqRW6rMm5+HR+YKWd1qL+Bi3Hj5yzbXS9BhHzlTTwvWcvvO2NQxcQ/j5Suih+8Qu" &
            "riF61Ik9Y388Tz3am581BqyJ5zy+/KUO8hzL/cK8WUfe75jb6+R5uZY2f2uYH05srD+xp4c9cX9///N3IBY2OPk1bO5rcGG+4Hhs" &
            "fU7WGrHTfyuox7Fr4MbIOljHMdTe5XCbdNfm7bk1fdN0m2Iz18R7jZ2HGhN2PmKq0fWZd61RI/0gmq3t1pBo410zp8Ex5Hm81nrj" &
            "nJHnGwN9aZ6jOebf0oidr6HxrGF/OA2TPbCf4/R5tN/xExrP52Xp99+of+2+63M98W1LrjXUuAb7Z77G66efRfMzkGfPnr1xkb3/" &
            "/vvrgw8++Eyh0Uk79ET/ZcGLsR5qdj2shbZ2JFy3NXii20IvxNjvuKkOj41DDzbmcc7gmn3S8Dzb3Fxvi6HPaDbi1nOwy9n49GU9" &
            "7Q+uxdPOcxOO57BuWLfYeFzDCwufE9bVdAP73E+N5E1aa6jX52LX59E5G4cgJ2BurpF5R3kHwiPtk8+w3+Mgmq3+Cd47q9TmPUFO" &
            "Gl9QkBPE//inTPz/PxpOp9Mbf849D5D8Hkh+iG4Nn6xVTgQvJPYDT7jBMYEXImA+w7bUxLYetL3Y5hDm5Wiux7ZZgzb2fSQmn+vx" &
            "OHM49BEE9Vy/c0xw3Brmah5ruhW38O3nPNM3Z5W4oNm5brZ7b9HX+g07f9bR6xkwd9Oxjxyvj49EeOTH3o4tpsWTR3jekz5xy7kI" &
            "blnPjGlPay8G4svHp0Tjtea6GOd3oWnhNe4b38JKI5I07etf//r61re+9finTA58C+snP/nJGxfYwiayPdrhBM4fTJq0NXghCM53" &
            "0mpxSxcH9U/6h1jObb3UwEbfwjlI3xqB14Swj/XG7s/P6W/zdS7GmWtb0Gqa4M9z15U86RPX+I3XYP+kxXWL32tp0OfYW/rRb7Dd" &
            "5zDY5actdh7JbXGNvxv7mP6hj1Edfw3t3PBom9uEtqbO08Zr+OIOOXywtDpob82wPvvNFqSOu+9///u/fdLfZcqRQbF97Wtfe/xN" &
            "9As+K+c7kAM3l6a1QyuaNfgG0sZEi8186Xcu5+XRmHjUmjBprhLH8aEbeE7oKvkbTg8XT7spryuxBOO4hq2Z33KmLjbzCNbp9Z9s" &
            "iWFdAfMTHNM/cSe/fRMap51X9tur1lXm1OIPPXjC99gxzed+a0vnxpo7TDrX0PZF4Lysx3uncW33eLJznD6vZWKKu4Ydb1e767J9" &
            "8R9KNScRH5+QbPyBa56ejN35eFzl5L4NpvoJ6nserjHrw7Z0AmmnJtfUc27rybWgn/GtTx3mCcd918ofFBMtl8H8rZ6m2xBO++G9" &
            "NWljrLV2Nmo4h230+YfqhvlNryF2c83n2DHeh6mh1dxy8JjaDdfD82vNdv5b38cdWO/SQ+iW+4Z51outodk9TzbzjF29/Fn00gvE" &
            "XR7C96xV5tviHdfqTNxjle1E+JsA5DGxN+mp/CP66HrifAUc29tOyotCOG7hpB/lr16Sb03WzvnteAu1cn1YU9OzRngnvXvK2pPn" &
            "ulwHdad6CNe0ilZq8RzCoS+wZutnfk2X2tSizTkd53VifNOlr2mH09YnjQ/J6PFIcN9yHB7PW+zUSnOsbbF7TrS5NvqanxzmY34f" &
            "mTu1eQ1WibOP9qyR7Rl7vuxzHY1mjy2x0T7r31c4NuOTPgIPMgf/uMFrY3vjMjd59NGfupgrdZw5CZNoD6aJEx4bjrfPmq2mVeyJ" &
            "2em/LVxLgznTOgask7HWCazd7InNhm0+55t8bhPPGo3HizN+33TYz9g6hu3mt9ZuBr5wXMsO8TOHx2634BadWzhZe89x1xqa33Gt" &
            "XdNYw362zq4ZtFGPuKYRML7xHdvGR9nv9htck3YfaTFrY196UHA/NNi+q+HuV3/1V387T0c+IReePsRXvvKV9c1vfvONb2HlZyAf" &
            "ffTROuMbAinWBUx2YtJwTFuYa5yJR47RbEFijvKknkDOLfWQM228CU03SM3pe4O3I2FtcnkOCT5UzG85VjkvrnlXI0GeNdqDw7Vf" &
            "Q+ZmHYK5fYzfLXb7OU7fN6pg4k8c2wjXcA1N13O3lmtwM7xHAnNbLdfGLafHBmN2/R3CdS08tr51m9+8jP0CrsWEd/e9733vjd8D" &
            "WTqxFFhrrS9/+cvr3XffXU+fPn3jAfKTn/xkffTRR4//VyTx/oholW/T+MRzUo1DbtswhvWaTvPTxuOE08N68SMm+5udvjX4V8k/" &
            "8W6F9ZY2n/s8rhviJ/ABEkxxXHvnazUZXteWJzY/QJyvgZp+MDqP4X2V/cPYpmEbuXyle+AFDTmubZcraPz0Hed5NQ7Bmsxtcdfq" &
            "nexrmD/ttk0++g3vo4a3WSPP18dd30ge8zPevfhI4/zGv4WVjZhgitBGZLO2dzPkkNs468rCcuy6U7ttrHs6wa6HMbs2cWhv/ayx" &
            "L/pb24T4r2ny/JLftBzX+r/slvxtL6SfObhWc1oj1xqBY3bN+a7B/PxchP7GzZh5yXFjTNO8pRn2ud9gzvFQm3/GZW7Gga9RN8P7" &
            "Z2rhEDvfKi8ydlzDsWszb9fq5tjmazC/wT7W/Zl3IFxsk9fDO5D8R8Kc8Pv7+/Xhhx+ujz76aJ3Kx160+SZtf8bu++gcge3OFZhH" &
            "OGaqecLks72d3F1dgXUmtLrbeTiVV76OWzfWZg51qN9sh36g71iPGd90fXRrfsdPsbvGmKxtMK0rm8+HNVtcxjy6n3HTaRz6zAmo" &
            "F07m6BxNi7bWJ5qNmPLm2PocH2WPkedzt6un6TV4f9C+hvWN/VYw1kf6WXP8fKiTm/7j50sW2SVqrxRoT59jc8kP18W2G0l0eEzf" &
            "eaY+69qNCY8D2tN3fOYwcb1O8d9iW9hsXqcgMT5vAWvwWhiMb3XH5/PW4trcmTt+c6hJW4vjnA3W4vzu7xDeVEvL4zg2z5ljz+3W" &
            "eboeajUu+y3PrpFj7ZbT2OVdw4Oi7TfHx24edVzz0n2I1zLnyvoSs8q9YN2Yw2A+2owp1kfzvA5eI9rjO51OP3sHwo+cTLhcLo8/" &
            "GD89/CkT/hB9rfX4p0x++tOfvlGIF4OFewKGawm8aMnnup03iK/FhTv5yLkVk8aExnV9Dc3e5m07L6wcWw1LHIMxWd+GyZ54x7Lu" &
            "+FyH67V/yhlEc8dzLnKZx82c9HNjsZ19617rG7btuKv4rZ8+a283yICxjDnw5zhaTscTu7yxu0+/x1PeKQfh2MY9NtfTCf/kKfu8" &
            "1WTdjK3L64Tw3CYb41KH6+F4rbXuvvOd7/z2+Xx+429hsR3H8fiD8dPDA+Qb3/jGOp/Pj79/kAfIJ5988igcflrGPN4KalyD83Gy" &
            "1jn0QPMC5Yfhjmn9W5GYaLqGHK1tDmFu0LiEN4PB9djxglvXhj7XGF9y7uZ9K3z+PB/buBfiNxzfjs3vXA3mu28O/Y5pPrb2AqId" &
            "qTP5iQMfhTBuDecjuDz8iXPq81rJsT0caLN/lT3UarPPYA2209b8hOdkTHl4PfAbr7fANcZmeB6sxb67X/mVX3n8GUhakBOZQs/n" &
            "8/rKV76yvvrVrz4+QC74Y4off/zxYxwnyjELsM0byyd8DZugwbyAtWQcuA7X5/6ENsdrcF1vC9dsO9Hm0HiTrdkDb7KGnT/x19aw" &
            "2dZG+1Qeygb3nM/5tXE7TjUGUz3U4atg1mfu0jsD10ct8gjX3zRsD7huaVlz5mP9riHcdg6Ca7XvbNm7jHetAfttbubY5txB87vv" &
            "2vIw5LXHF/vhe28QzsE+G/2eAzkZ3333u9/97SdPnjy+AwlOD0+4y8NHWLF9+ctfXl/5ylfW6XR6fAfy6tWr9ZOf/GR98sknb0wy" &
            "MRlnshwTbWy9Bk90QnS8CNb3eA2LZ/+toH7ye20ad21ObgNjnbPFp4Zw7HPfHOOaP4ie5xqwrox3aFzHtLV2vUe5gZPTxq2/ypo5" &
            "NxGOcy/Nx/mcs8G8k26qLV9uYNf0qdGauYZtx/DAae8umMM6xlSX49veIafZOc7aruF8Tz6ekyUtN87BOsRunoynjY026pwT2Fp8" &
            "DL5/+K9+r1+/rl+/YxGBCwlvB/s9ITdjsu1iDPOvNb8CbD7rcpy+a2CfWsw35W46E6jF82o9922zFuO5DobtjPFcrbtrxi22jKmz" &
            "W5fWfM4XrjXDMS3W/FsR7VtB7uXKwyPXu9dm4csZ4dnW6oqNc0+j9oQWx/i86E3j/c1H95nb90mj3UtjbzyPW4t/Auv7PPAa3IK7" &
            "73znO5/5CCsfWZ3wqjiiX/jCF9Y777zzxudv+UXCFy9ePNrb4kfLC8a+W5BYcmNvcLxh/9RvNtdi8J3cBNZvLY+XNnTGOzvHOz9t" &
            "PrY6jLZuLccquj6ad21sezDNt/FPm1ffx3AjP8o5M5xrmut64DJ/bDwS1iDH+5T9SXM3H9fVwPqp5boap/kNxy5oU8d5qTVpL+g3" &
            "OKd5zbbKOaLNx1XOyRo0VpnXLZhidvPLeOe/++53v/v4D6VOuKke5R+XXC6XNx4g62GSeYB8+umnj/YpeVu8+OnjTZg1xN+Q+lkb" &
            "damTzxCtbexy8Zg8aTs0/y0PnWvwWjffrZjqYY5mu5aH56PBr+4/D1pcq5Hn7igPi7S2fzwHj6f6qUsOj3mFfgtSn3NZ237CtS/V" &
            "uauHuresU6vD58H2wPM5hr2U9TB/h/hz/TKWnDZmI1Jf0wqan3OydtOgra1HMNVoH3M2O2Pvvvvd777xD6VC8sTSnj9/vr785S+v" &
            "J0+ePPIul8v65JNP1qeffvqZRNRpYL6FE5i+a6CPGoxtnCAa1jZnqZbEkTvV3PIa5mR+nKPrYn73jWZrSE7C81xaE9u5jm194uMx" &
            "MIctyP4kz/0WF7SPTHhsDy2PmZ/nmnCNHluTvt144ZxEN2PXwBzTRz6Jp44x1Tr50qc9tbnOprlk9/zsMyZ952pj22xPP+vJ/bTK" &
            "D/Z5rqhB7QmOaWNy2W/rErgOarN5Ls7h4+M7ECZP/4Kv1GXRnj59ur70pS+tJ0+ePPKP41gff/zx+uSTT94oghs04xyZZ0IKTVxA" &
            "XfMC8g3WFFh/gnMGqSk+NqPZCK9TQJv7k2Z81opvlQv99PCu0q88ue5ea+ef1oNx5viGx3qSlxrMyxxs0wVB2y2cpXrJs+9Q3bFR" &
            "bzdvIlpEW0PGO5fthh+gjbOGeJ6XcGJvY9p4bLpeu2tw/R4HPt+B1zkcr0985i29c29c+rxf0s/ROQPW6Ws0/KzfCb9rwphpTmnW" &
            "jd+8u+9973u/zWSrbFAmubu7e3yAxH5/f79++tOfvvF7ICyYfcO5pjqOclEasbPehviYxzmnHEarx2Pbdn4ffxk4Da8yG7yGt9RB" &
            "7bYeDazJ58vnp3GIttmDKSZIrPWvxRmcC23UtaZ9jjW8tpy3c/joPjHlts3r7L1y4BqNnf7WN+zz0Tc+t4DnlTgeaozde9Vzys02" &
            "th18fmi333lYP/P4OgkmPv2tz3GzU7fZeDyO4+e/SBhwkhd8JzsTOZ/P64tf/OL2AeIiYvPiJtep/BMVH6f42PkxnBFe+vaxb38w" &
            "2ZN/F0uYl36zvQ2meJ+H2HiOHNvO31STebfYksP2aUy+QZ3WyPPaT9z41mbeBGuwbRoTk4927pNdHtta3zY3wr5rnIU9PnEZY1x7" &
            "QDiXxxOO8io/SK2ue6dNW/reX0SzX8uxSq2xNXsQH+vK2Pl4pKa5buvhXN195zvfefyf6EufNx+4eHmDf/78+Xr69Okj5/7+/vFn" &
            "ICzCC+p+8sbuRaY9uo3rPLsaYgsyR/OSk1oN4TF+F+P6fbwVjc95pAbWYhtrZkw41AuH/mme0Qp/4vk82XYLrvFZM+fC+qe5cA5v" &
            "A8c0bXMW8jV+YF9q91xsj8/wu4oG67n2A+9OvEcI12J/7B47zn3aXFvAdXUOg3Nt3FbDumHurbaWw2Ni5yPMy7i9m2pzpW1nv/v2" &
            "t7/9mb/Gm3ZgQ/PCe/bs2Rs/A7m/v1+ffvrpevHixRuLFB2Odz4WSrsRW/N50s5DMOeOdwsS3+Yx1dvsjDU/oL1pBF4Lg7Hp81yb" &
            "8zZgTuefxq3WtjY7bcLza3NgzszdcfERp3Keg1vsydV8bbyQc2oB52ObwToap+mvsh/sD1yDm7nmvy14XiatpusY1rI7Tw3meByb" &
            "5xt7jlNexxDXfJ4n7Xz3R7vH3Id33/72t/9Xd3d3p3bh5COsBCXB06dP15MnT94QevHixXr58uUbk6Zmxsa0SG1MsFbCMXnnZDvh" &
            "ub8NuMBrE+95Nt6k07itXufYzbl9bDnlvwXRaFo73dR5lG+3NHBeOx7XxzkDx5Pr4wI/Nr96n/IQbS2mc+V8Sz8ADefY3GyMxs35" &
            "sqZ12vxO+E11osVT1/7UQC6P4dyCpsMxc1nTtbV9ENh2bU6BbSftUWu082M0G8E9s8oauXbaG2K/+5Vf+ZV/6fwzrHVlwdZD4N3d" &
            "3cqfP1kPxb169Wq9fv3a9M/otYKmxTNcU7sJEh5bO+PE8mQFTcM6S/knDduDnWaOrNV+glzXwBpjazBvlbUzWF/jHuVGlRx+5XML" &
            "mo7R5uwcHq+y7qm9odWQmKa9lDN92toatrXyGjgmsD1atsdm+zG8K53G1Gj9dr5b33WsK7lsb33D5629iGk5ee+J7VotzrXDgXfD" &
            "5vqBzfVqtbba3G8vSsx1HcfDO5B/6XQ6ndfDDZmv2DMBTiLj/PLhevgIK38iIO9aPHEWkUmSR/s1hJc4T9iwj2POkfknzcl+S+0t" &
            "rtmMaY60JX82QjYZz+m1+fm8EOY2mMPztIbPX3dgPZ7nKjr2HcNFmH44buT4Yo2P2g05D+Q4L+H8AeNZi3nGVL/zpEbbG59z5bkJ" &
            "j0fD+un7lTFr2YF6jm9oetwXp4d3Ulmv3bnl3J1/DWvmdSLXiK7zh+sj+85HsCbyj3IewufDPmPi7tvf/vb/8nQ63flbTOkzWXDG" &
            "n39feIBw8bkAPhIsro0ZYw4nZ84qv01tHuc48YLMyTXEZ+xszOmj+1wP1+o6WRvhuGu15ejc03y9Fi3XLpZ91x47+xNvFf3AdeRm" &
            "wYeaNTl2PO3HsD6uJTlO+vLIUfay8xheh2jS7uvxVngezddszbcGO7UZby7HmUfj8+ibnvnNziPPS9qp3Cus4Ub+rWtJ3i6fdcMl" &
            "J2h1HPhLI+06cIuPOI5j3b333nv/i/P5/LQtkEGBE765xSKmCbR+eC0nJ0oba5wmd5SHlxdkWtRWf3w8XoN1dnM0nCvHk24E9tu+" &
            "NnNzfYRj0g+o1fzBND9u1sA6h27Ka9g7zRd4jtHkw8M64RmT1g7hZB5tLke54RFtzuRa03qMpY1rS3uzOZ725nMjz0f3G9/jxrP2" &
            "KmvXYtJ8Lnl+bIt9yhuEnz0QTJqBX8yvIcZH8xpYc+7fvnd7Xmltz9y99957//Pz+fy8FRMwkMjHI8fDpuW7EPJbLG38mMVwPR4H" &
            "XnDavSjkpW/OrfUnL3VaHYH5DfZzTDvranp8lUu0+QSet7keB7afymYzWm07W/MZrX7n9zjwuu/Q4g3Of7K7Gb4uzHecbdORaBo8" &
            "pp9r25hqaaB24zrnDvH7hcBR3q0TLbfHsUXLdh4npAbHr6IdLfJvzWM4L3WOh/PIe3dsLU/8Rvh377333v/sfD6/w4u0taUTFi5/" &
            "DsIiWkEer/J7JwRryiQDLorrDKax7QsLNS1Ys63hpuPxhMzLmOq84Bc7Cdq4FqyN68WjefSl73VxzbY3rWldDc+PtdnX9JzHtXku" &
            "AdfBPsa02AbnMyb70jry/Ln5evCYcbTxRZ59ge2+nt13fnNsa/UZzT7tZ/u9Vwjn5Dlnn/e5a/Uyn2s0qME48j0/12s0PeLQR1W5" &
            "78bGmnxkP2uR8d177733L57P569ns/pVaysoAuEvvA2Kn+AJIJzLiG+KT83pN5CTMZF5EI6JzX0fDfuj2/TpT79hsq+hbiLnjRuE" &
            "b5e9cdqaN5gXrdRi/63wuhGex8JNzvk8DmhnDupOsdfgeg3rtvm4pmPzkKDN2oSvUfLb/BkT2J+6dnnfBp5PwDV1DdwrgdduF2fs" &
            "1nmqieOT7lmtlmY/ykdFXv/sC8ZyHDDPwvVxwn2bX4Biztbn+FFnrfXh6XQ6GrkhnMvDR1avXr36zB/eazqcrMeNH1+QSfME8dji" &
            "F/S96PS1GqznceCTRrSYZgtcA22+8I02h9jpn0Bejm72tyNj203FmtY391qMY/kQoa+dd8J8c6x5S2s50yaYl5YXaJP+FGtf4zUO" &
            "bQ0t3rjmJ9p17FjntP+kmzb51xpjfMM2x3bfi8zJ0XkMcx2zysNjXfkUh4gv8Zmr99Uut3Xu3n333X/hfD7/2t3d3Wdm34RYeArJ" &
            "BFLQUTb3etDjArigwLHUYz2cSHJn7JoaWINtzd7GbW34ubX9nmvs9Hud2pyN5GwaF3y1mrnanGLj2rY6Tg9rzA1ofuC4BnKW1pfz" &
            "ic05HW9Qp4FzbA++pbV3PdZnbYzhHiUv8PpMNXOPBeZFi5rN1nK0ceacZp5tXFPDXHKsyVp5s7u2Vp6rc+7iU7fzOEdsAXOwP82v" &
            "jac1I5yzrVF0uJ/P5/N68uTJOp/P6/7hv8vGl2Pb/yf9Yvnxs79Icrl79913/5t3d3e/eTqdzvlqLjc5i/OCnx9+EDOdGC4i7Z5w" &
            "QP417lI904JG0zbD+U4PC+V6zE+/8bge5rNv7VvBWjMmPM/MKX3HL8SY5/U1P3ZzGxzjsWuib5Uaj3JRt3giMY6j5lSH4fpvwS43" &
            "x423hjyNZ5DDuRq2Zcxz4NpazFFu9As6fCCsYS2d51a0/dLqvhW3xHiuGdseNPtpcw219bzGpy+/BH4cxxufHoU3vdg2Hq6NT+7e" &
            "e++9v7bW+mcul8vTBOfVzVTU0sJwQpmgYz3p2MIjvy16sxGJb7W4RsN5zJ1y0+44z2fSYI1tzXJiw2PcwttXxjLmFjg2x+RLPz7a" &
            "mafpGOY6D4+G89FuWNNoWh4vnANiGvuYPnUdS8Tn8z7FNn/LnX6z29f8b4OploDXgTkZ53qgrWm1WNsbJn+7eToHwevRNs+jcdxv" &
            "2M0pOdJvXNrP5/Pj3zF8+fLlevny5eOcp/jAvuNnf77qP777yle+8oPz+fzPPn369CtPnjx5fJtCMidJG1t8HpNLnPAKv+VZmNSB" &
            "Px3AyXlS6Uez1ZO2yjuoVmdgO7npk0MbeUSbt2smTpsHikE/14k5pzxrqJXzoBY10w+ss8oPctOnbou7hl0M16Kth230tXV3jY6n" &
            "vcUGrItwHDnt2OINa6bt1m2VPMx3GvY359n8U82sp/mXtCedgPU1v23t4xuP15W9tkpM6nAc5zuBc7XNsJ3vMJ49e7aePXu21lrr" &
            "xYsX69WrV29ci1nXAx97WQ+fTt2/evXq37373ve+9+7r16//2nEcv3I+n+vfs9qBJzPghHcLx+MqG4w2xwfm0Rb7Wb+hGxvrmvQn" &
            "P+32BVPs0saZajd2PPvs9zhotVmr+VqO+D2vU9kj9Md3C6fVu8RjDbGxxdb8gePb0bXY3+Yz5SBavfY1Hrktjn3eKHP0uW3njeOp" &
            "foKca3zXEZtriJ3H9F3flLNpLmk4dmdvuc1xv8Vd6zcbx9Rj/8mTJ+vZs2fr7u5uvXr1an366afr1atXn9Gb4jNea63nz5+vd955" &
            "58O7u7t/++6b3/zmO+fz+Z+9u7v7zadPn64T/rpmArwYgRMG136GYj4RPcZMm5ATXcPXgtvY9bDfanMNsfH4eeC5cD72NV6rn5xp" &
            "LjneUjtzMcZr3zjxT/U4v+fpfObfipbf/TaPXe1E47jWFt9sa3iHFtjGvK6XXPfTXCf96du3yvwC552w43HtbV+qyXV4/xGcd8aG" &
            "711EG1vT/jXkaXVMfPPYZ4w10548ebK+8IUvrCdPnqzXr1+vTz75ZL148eIzH1+5Ebm33t/fJ+ffffHixb96961vfetyd3f3m5fL" &
            "5b9wHMddyEFbNNpYMCeTPvnWYpE5GcRJ36aadGJzXRzHT17yOS7HXe3WnjD5XG8w1cS1YW1Nx/w1nAfHEfa1XKfycUzL41rag77N" &
            "bw11O2fgPEF7O964ydu+GBI4Hxv9U/wE8lPHbo3sWw8fV3ieQTSDrEnT4ZxOuPaiy7kyl/23gFoZe42vwWtu+1JtR/lhcYu1bYJr" &
            "ZD1tjdqY829r4JgJ1llrradPn64vfelL63w+r08//XR9/PHH69WrV4r8bB7a86ONh/bh69ev/40//MM//D/f/eAHP7i/v7//7nEc" &
            "/+W11pdO+tpnBJYW2RO8xjOfhTIXx84fON43pGO4MIJJj3F8iBKulaDGxPHPcsz1PFqt7ciP6QJr0xa7/YHnYt5RLkLDa2qcyk2J" &
            "vsSRc02PMVwPaziOfdbV9laafUGzBc7f6sq55IPPNdMebkP8mRO1OE9y08/R86FWwL5tJ/wvIfOm80V/OLkvhd9AvpE12r3yJmyL" &
            "NuuJ3TAncD7nXOW8BORb13PK+OnTp+uLX/zievbs2Xrx4sX66U9/ul6+fPmGzqEHGLVyTE339/fr2bNnf/z8+fN/4+7u7u/c/cN/" &
            "+A/vv/3tb3/5crn8l47j+M7pdDrtbp4TvLiTrYE8cz3+ZaDlaTBnqnENG5o8nqT4rBeNjFsex+Vc5ejYXc6mH9jH+XEjB8x3Kjcm" &
            "cgjrBI0bJL9bfOQYzbZUI9cmc3EexxmunzzqmEe/z1FsHAdec/t3mGpoiJ31t3Uhx/2luNzEaDemHETz2xadNo/GJ7xOO+4t8DW7" &
            "yvq6nz2RGK5X+lnPL3zhC+vLX/7yev78+bpcLuvjjz9en3zyybq/v/9M7CoPJebID+Pv7u7unz179rfv7+//9T/8wz/8j+7WWuvr" &
            "X//65e7u7p86juOvrLWeJsEEL2SDJxrbKgvPQn8ROP7QK2jXdBo260Js8DY1elMa1GI99qc/6Z30EV8D5zHlM6i7dNGlmW9dc3gu" &
            "6PPcHPc2SCxrbPUG5AScg+djLd74gmltW663wTG8MuSR3LcBzwnn2M6/4fmSc+iVre2ZU9O91eb8QeMG01zeBo73OHVN9eVa8PWw" &
            "Sn32c83C9To+e/ZsvfPOO+udd95Zx3GsDz/8cH300UePPzhPXdbyQyScPBOeP3/+o9Pp9DdevHjxf/vggw8+vVtrre9+97svnj59" &
            "+uXL5fLPXC6X9/zWnJNtE15lkuSbQy45bbHDbb7AeoRroJ3HW3ycU5sXMWkEXuM2T8f6BrJQU6shaLXQ5tj28HA/Y69JO8dL58G+" &
            "oNX3eeA6G+j3RcN5tDp2cwxSw279Jhybj6+4B8LLuK1/q38N55+5GOf1WWVPUWfCzp/6029IfW2fpMZpvp4jc0wxE6b6grYPGnbz" &
            "tQbt1vQvA+bh8aUvfWmttdbHH3+8Pvzwwzd+cH7CH1L0GgbHw/4K58mTJy+fPn36/z2O42/8/u///u+utdbdWmv96Ec/un/33Xd/" &
            "+urVqx+8fv36r5xOpydvKAltwqvYbzkx3IDTohE7TjSo6WNr9lGPfXPo90k2l7Dt0LuExlmF1/SbvfW5CU+6YaZxTuE7HxH+judz" &
            "NtlWWV9rOq7NnfB8cmRcYrkG5BrkL6yTY2zfIbzcFGIjdnNseRq/2QjOqdkZb47Bupx3ytPg+TEm2tYx1/4W4xpXyUVkv7S4NcRy" &
            "HoR1HEtf3h2E8/z58/XlL395vfPOO+t8Pq9PPvlk/eQnP1mffvrpGw+Io+wvr08eHsfDz/uePn36D9Zaf+Nyufzf33///RcrD5D1" &
            "s4+xPrq7u3t6Op3+s5fL5Vuxrxsm1JIH04K+zSZsJzhodi4y9XmS2SZwXgF17bcu18Y6C/qu1bHBNBeOzW91tjH5RKt7lfPneqkZ" &
            "eL62p0/7DtHKsdUf+OLJkXEt3vOiLfzGCY/8nY2NF679XnfWzFehRKuRerYT9tHfNJfype94xjqeGi1+Ggccu2/uLWANq6xREDv3" &
            "UHL63PictNpoYz/7I39Jdz38nsfz58/X1772tfXFL35xnc/n9fHHH68PPvhg/fSnP33jW1epJdeE84ZD3+l0enE+n/+tly9f/h/+" &
            "4A/+4I/Ce3yA/Pmf//nle9/73o+P4/iV+/v7v7zWev4Q+JkFIegzvNmbrcU3PheeOHRDavUxV5o/Qgocb7RcrMG6OfHhsDk+tms1" &
            "BI5PLG+WRmJSI/NxHrbzGOzWL/NJP3VRIzmmXNZnbQH7zNn8hDWoPd28Gdf813I1rm2Hzl/qSk2eX9Y2nMC1OEfz7WwcN7tt3su8" &
            "gdqfund7L6BGfFwTxrnv8+oYr23Q9KdxwPMycTwP2n2t0Bd/1u3p06frnXfeWV//+tfX8+fP1/39/froo48eHx743Y3HOM+ftdr3" &
            "8O7j718ul3/5fD7/ux988MHjRnt8gKy11p/92Z/99L333rscx/GX7u/vv7/WOkUkou2bA174XMjcEAH75Bi2t8VkHud0vEFe+r7x" &
            "Tho5gQS53ADUn+bQ4DVjTuvE3+z02xcb14JabtfAfDsbfdONhZyFGq/pTfbmp3bTZV2tkcML0xruO3bXVtmXyde46Td/a03fiF7A" &
            "czE15vcNi9cE9YKjvJBJ/AnfPCRyDp1rFY3YGmeVa9kcXi/2sZ89wfm6FtcVJCbzYrs8/EfBJ0+erC9+8YvrK1/5yvriF7+4TqfT" &
            "evHixePPPD755JM37hlem9QXe3Ld39+v88Mf1T2OYz19+vQfP3v27N9ca/1r/+Af/IP3UeabD5C11vrqV7/64y996UtfOp1Of/U4" &
            "jq9E+ITvcrcF5PiWV+Fn/BvbtoBE8zs3wfrIc93RZX1EG5PHOZETOPdJN8d1g2arq8298Xawhsesq2mzXttTv+OpY02fE2tNvh2Y" &
            "a5Ubpf2uc9d2/PUwn/QbHONGXuBztMr6B9biDd1t2r8c52gub1BpvlG1frj2Twgn9xbHThqsq9WR+eT6DE6434XvXDlOazvFGZM9" &
            "4E1+4eOqL3zhC4+/47HWWi9fvlwffPDB+uijj9aLFy8eP95aZR1afRlnfR8eJC+ePXv2/1hr/e9/93d/9++ttd4o9jMPkB//+Mef" &
            "fOc73/mj4zj+U5fL5Z++v79/4wfqSTBdIN7kLjAbYPqmz9LN5JYcBDdEg+08+UHGtPPYalpXfEHzOxdtHLv2VpO1Wj6ukW+qgS+K" &
            "VbQu5V/smt9afH5BEru1Wmt82sxhvuYnb9cI++Ln/m04hhvv1FbZ19QOLz7vaeu5tfVnfNsj8bd5mOPrmc0PmwbrthjmajVYg9it" &
            "6dQmDmE9+6hDm+OOh3k9efLkjV8MfPLkZ7fmly9fro8++mj95Cc/WZ988skb/+MjGnkITXkWznPefdzd3b1+9uzZ3727u/vfvHjx" &
            "4v/14YcfvngjoD1A1lrrT//0Tz/45je/+fo4jr+41vrO/f39eZWnf+AFsM82I37fjOhbg99gLTw6lovo+qax7YHt1vV4B3MZM629" &
            "bYTnbSTeN5FdHeSshxypzbyg6ZHri9g1NHuDY9yMozyYiRZvLY8X1sRoWsZkc/7WcnP1Tb9prmJ3XcxLMNct4P5gW8jF9eJ+aFzn" &
            "5c1xgn3JcdLf/3MjPPa+NSa763Uuj+/u7tazZ88eP6o6jmO9fPly/fSnP10fffTR+ulPf7o+/fTTdTzsZ96rrbWGewn3zOl0ujx/" &
            "/vz/dz6f/5U///M//9f/5E/+5CdvkB9QHyDrZx9l/dnDPx/5y5fL5RsPT6Q3OCmsFdOKXmUj0c7j28B6Le9kazETt/kc53mZH5vn" &
            "6QuGrxhO5ecozjOBvFZfbgK7llhq7tD8jm+cNfCmG5Xr27U2z8QR3sv2U88g1+c3MamDeXb5Yrul8eEx6TRYh7zoTdwG71WCcTsN" &
            "+tpaL+hzXVnvLXl83b0NjuHe18DayG81cr55EKQdx7E+/fTTN9qLFy/WcRzr7u7ujb+H1ubEfcmabLu7u/uDZ8+e/SuffPLJ//FP" &
            "/uRP/vSRIGzv1j/84Q9//fnz5//i5XL5H71+/fob9PGG7595cMKtkZOfhZwf/v4PdQgvBBGfta/ZWqztgWOtx/jJTthOLm8yq/xi" &
            "H4+tLh/bulrPcfQ1P+u1ttcusJ3zYj2thtPp9PgCxjrObxzlYmLMFO88Hq/Net3d3b0xp/h8dJ+Y7A0nvQN8m9i14XvdiObzWnKf" &
            "hM8as9fbPaTFRI8PiTxA4j8e9pA1AvbNYZ7AN33C607ERu3GC/gQPB7WhD8n5oMyDwrW5rlYM8iYemmnn92T//zp06f/6ul0+t9+" &
            "8MEH/+GPfvSj/gRfVx4ga63zX/gLf+EvvfPOO//T+/v7/9arV6++zRP4kOyNk7UeJuKbA9uORz7BBSAy6XUlvtkI6oeXOdKWfstF" &
            "Tk58y0k+T25izOPY60Cb5+gxwZjA85hid7oLca7VdZ70jRpu4lZX9grXzGBceOE6/zU4h2O4P4LUv3vot5p2YI7cKGPnXD6v/io1" &
            "GrxBB8nDGNbjI/kn/CZ04P2/So5TuQZcg2Gf9YLpQcEaDde3So3mtD2zys0+4/CzXlk7+9bDGh7lHbfnEJtzPjw8/q/Hcfyvf//3" &
            "f/8/WGtt/0HU9atordNv/dZv/dXT6fQ/fvXq1b+w1np3abHzyjD/0ZA3z9bWwwSanYtLexaA/gUd4lQ+A6SPfXNcR4u1hhFdNvp4" &
            "dF73DddDHeezTsvp9WOs+eQwxjURjl14dd5ysB5zzE2f82n506jTaibHdcdHfWqvsna7Wo4rr76jyZramDl5k2BdhvV3x9PDtdRu" &
            "QAvXpfmeu+E1aPzMaTcXg3NL37Ec0+9+4xOsn5yce+tax+eVa0ltgpq51+7yNC3z0z+fzx+cz+f/y+l0+t/96Z/+6f/n448/3j48" &
            "1rrtAbLWWk9+4zd+4y/d3d39Ty6Xy3/7crl8J7+ccjyc/OPhb7Bc9Kosi5mHCiccfzacN1HG3AjmNFjLCxd/muvhkb61+SIBkTme" &
            "y1eV/XBlTYTHbQ5BdLiOrX5rOr81mwb9tyDnLTjho6isRXDoJr87hk/9qSbntz3nK/nNmXKcHtaPL24YzyN1EsO4u7u7moO1G54v" &
            "uZlL83HsGPajwY8Ow/HNLv3seddGkO8HE3HSA8SantPCWre5M5fPie9bnJfB+hPLGtMahwgntbA+9llLjk2vjVvus/7z7P39/fHk" &
            "yZMf3d3d/VuvX7/+l//gD/7gb6+1PvONq4b5LH8W5x/+8Ie/cXd39985n8///ZcvX/7Ta61T/svVqbzy4ALl6E3ABaffOrzYboHj" &
            "eRLsm2xLF1J8p4e5UpM+xucmmQ3q5pocnxzk8cZLuzXiS/20hefY5mtwjQ3Ov1B/y2nu2tSYubA1PdvIuyWGNwf7qGMbx+Yd5QHi" &
            "FxrWCd7mOmBu3kS4Xqw9II8a4af+cDx/3wdiD/d4eNBwDQyu2VQj7a7V8Pyt33IsnduJk3nEzxqon8b1JKa18FpPNU+21JY/uvj0" &
            "6dP16tWrdT6f1/Pnz18fx/F7T58+/ZuvX7/+m7/7u7/7O2utz/63qQF9tWecfu3Xfu07X//61/+7n3zyyf/wcrn81vl8fvrq1avH" &
            "j69WeZWdfoM5ab6gfOFYzwtH/47LnDteEA5fMWZTMD42zyO28J3DPm4AxgbhcM3pCzy3jH1MHvvSGrgOPBqJz/fXG3Y188FJHy88" &
            "rjNrztH7KDGMM3IOjLyICLiG1rMuNb22O26D+bExd2piM2izn3qpJxyvQduLq6zLgR8GE14zxqzyUD/K9Rc+OYwJ2hwI2idOHiCG" &
            "czqe9e58nO8x7IXUEB1rXx4+7jqdTo9/F+t8Pn/85MmTv/f8+fO/eXd39zd/53d+5z9aa+3/l4fw2Z13A/7yX/7L33zx4sV/5enT" &
            "p/+94zj+q69evfr6cRynTCBveXNS28kl6DuXt8BcNG+u+InGMbzQzknOKprkOm5hU/sBkjy8GTIPHyDxZV2nG+gtsOYabshcF86V" &
            "4yBzZJyPAWOZl2g5CPrYbxfUUt0nvQuNf+nVqWs4XfkYpYH1hM84r1HLaZ7zM5ZHw3ZqGbS5Rl4fh17UMI5z2c05fa594JqtM80h" &
            "uWNnvS0/x5N918+YjbVybo4LvH6x0R6d7KumaQ4RXv5EyatXr15/5Stf+dHTp0//nRcvXvyfvvGNb/x7f+fv/J1/5Lhb0HfdbXj6" &
            "/e9//69+4Qtf+B984Qtf+G98+OGHP3j69OmT9oqCN0VvDoKcxHDyibUG800cw/5b4uy7Np+FLxikRuZxLG188DDWcw3fa26E6/jW" &
            "z9i1cpzN2mrY1XIqD9XYV3lVb7jmrE+LiT81pu81XTo/ba5Nf7LZzpp3YC2s1Wg21kx/dJqdfuZqPK5L6mv8YJozY6jRaqTd54F9" &
            "8oi2nu0Gu8qLEOexfvy8qYfn2jyna/BaRI81Mkf6HlNjPeg+f/7848vl8ve+8IUv/Nsff/zxv/bHf/zHf//FixcvH0lvic+u+tvh" &
            "9Bf+wl/44a/+6q/+1/7kT/7kn3/+/Pl/7tWrV+/d398/4YSz8fwqP4vNsVsQnk8kfUS0rdG0ifh4EmgLeBOMVo65CTKP66cea2IM" &
            "a5zmGDT/Ds5BLc6DYH05v7Yt1eK6TlceIJOW15k85yC87rbH53NBcOyczp8x185zCZgr88u43ZiY2/W7Zs6nwTWxDnPioxbjW9/8" &
            "jM03z/m9RvavUp/tzmk/Y8hpdSbG58f8loM4yr1p4cV207OtvSOx/fRwvZ3P5xdrrX94d3f3/3727Nm/+eLFi7/1e7/3e3+01upP" &
            "1BvRZ/eW+Ot//a8/+Vt/62/98Hw+/9dfvHjxz621/ovHcXwrD5KHCaz7+/vtL1jx2zneOG8LxuSEpM9mfmyMoT1c/gyE/pyw+NJP" &
            "I49IHOsyZ5WNR/tUj+dmMGfGzW5bHiCxt9rauQvHdTlfmxNbfNYJnNt5c6HxfLEG67Xa2MxNjayT2m1u9F302Trrb7Guj2Bu23d9" &
            "xjR9Hm3zfBjLsR+25u7yLq2dfbQ5doo5Nh+tcS7hUJf9tt5E88fG/Cf93scur+OePHny8el0+qO7u7t//+7u7v95Pp//nWfPnv3+" &
            "3/27f/etftYx4bMz+AVwOp2++Gu/9mu/9ezZs3/ucrn8tfv7+//0Wus7a60v8Nta04ZqPzuJn8fmiz1HLirRtK1ljn3pZ+M7r/Vz" &
            "ZC7nP5UHSMB6ouO5eUwN5ww838D8duR8WFN7gUD9A6/OvR7RCo7ys6+8GDmVX0SLRnwcR5drx/mnljxM0jeP4Lx5gXse4a5SYxAb" &
            "+e0mFp+RNTE838YJyDNcm9FysJ/zkf4q6xkbtZyL2m39pvhrc+daO25pj5DDFns4U75J0yAv9fkF6ZD/uLu7+2it9Q/u7u7+1uVy" &
            "+feePXv27z958uT3fud3fqf+TavPi175L4jf+I3f+PLLly9/7dmzZ//5u7u7/8zLly//0vl8/qdOp9O3juP46v39/d3lcjnxhF/0" &
            "C4g5ckMEPkH0JyZ2+xrHOvRPoN/10OfamYNITrYgNwdukja3nbb72nBvvAoMpptScrmeVnuDzzPBObQ5sh3lJtJu+s7jugly/fMr" &
            "IvONhrVaTtbCGsin5vQAWappt57BrrZV6iEmvucchM+HRuafvrnMQW3rN57t8fHIfqsjf/qcceYTrq3l4/kmwsnDgDbi0AM4Y94r" &
            "8TB+fTqdPlpr/cla6+8/efLk7718+fLvfPLJJ3/7z/7sz/5srfW5f86xw2dn90vEX/yLf/H5s2fPvv7BBx9873w+/6WnT5/+5uVy" &
            "+SvHcfz65XL51bXW147jeHYcx5PjOM6+ELyo7WSchgdM7Gk8mdafdJu95WLfMeavIR9v4NFxW9i4abG1B0DQ8gXR8DuHtXmABK6B" &
            "de7irs0rHK4d9fiQcIzzcuxc6efoeK4pa2E/Y9uunY8WE/Ch6DoJ1+y5T/CatPWgf8JRvvVDfnuAOJ9jws3RLxCo12B702K+Nm/X" &
            "xxs9OTk2jWvgWgTH8MDwGq61LqfT6fX5fP7kdDr99P7+/o+P4/j9J0+e/N7Lly//g/v7+//gOI4//aM/+qP33+Z3Oj4P5t3xS8Y3" &
            "vvGNZ9/4xje++Pr16/fO5/P3zufzb621fvjw/9ffu7u7+5XXr19/83w+f/l8Pr9zHMfzy+Xy5DiO08/O4fl08k7DBeCNHOQE8UTE" &
            "nnGRXavYvbEI6vOEk0+/te2j3+PoNn1zCXNjO/38B21v8Kg1zZs1OHfG4TQf9a9p0RdM2u1IOJc1Yg84/7YWtrW5GZOm66Kt1blD" &
            "uNPDcGcLWAdtazO3tXmA8J1VbJ5zm6999E81TPalNd3xJv3Ycu+h3tIe4DmjHzjYTqfT5XK53K+1Xh7H8ela65O11sfHcfz4+fPn" &
            "P3r9+vV/fD6f/+Hd3d2HL168+A9fvnz5e1/60pc++IM/+IMPX758edNvkf8ycPtO/OXi/N3vfvcL5/P5i69evXp6d3f3pbu7u/ee" &
            "P3/+3nEc33r4qOvdy+XyteM4vnk+n7/2+vXrb5xOp6+dTqcvrbW+cBzHs9Pp9PR0Op0fTvD5OI7TcRx5zpzWz07W6Wfn6nGqj53j" &
            "Z6/c3/goLXbbVtkohF+tN86E5GJb0mg3AH/EcRp+iWt6uAapffeqmfD68ILwxcO5JO5h3T+zXtTKA81r0dbVNudmP2CujD2nVjv9" &
            "O3Atc56oGXhezEOO83ncwHWY9DxPxhrxx9fWw++e+KIkcX4n4FzOw/6xebG4blyXVdZ0F+c5EseD8XjA+vl/7DtOp1PGbJe11v3D" &
            "R06vHv5kyCdrrY8ul8tPj+P46HQ6/fR8Pn+81vrg/v7+H621/vHd3d37p9Pp/cvl8sHLly//8atXr358HMenz58/P/7oj/7ok2t/" &
            "9PCfFP7/FvBcTof0pcIAAAAASUVORK5CYII="
        Dim bytes() As Byte = Convert.FromBase64String(base64)
        Using ms As New MemoryStream(bytes)
            Return Image.FromStream(ms)
        End Using
    End Function

    Private Function ScaleBitmapTwoStage(source As Bitmap,
                                         targetSize As Size) As Bitmap

        Dim current = New Bitmap(source)

        Try
            Do While targetSize.Width <= current.Width \ 2 AndAlso
                     targetSize.Height <= current.Height \ 2

                Dim nextSize As New Size(
                    Math.Max(targetSize.Width, current.Width \ 2),
                    Math.Max(targetSize.Height, current.Height \ 2))

                Dim nextBmp As Bitmap = ScaleBitmapHighQuality(current, nextSize)
                current.Dispose()
                current = nextBmp
            Loop

            If current.Width <> targetSize.Width OrElse current.Height <> targetSize.Height Then
                Dim finalBmp As Bitmap = ScaleBitmapHighQuality(current, targetSize)
                current.Dispose()
                current = finalBmp
            End If

            Return current

        Catch
            current.Dispose()
            Throw
        End Try

    End Function

    Private Function ScaleBitmapHighQuality(source As Bitmap,
                                            targetSize As Size) As Bitmap

        Dim result As New Bitmap(targetSize.Width, targetSize.Height, Imaging.PixelFormat.Format32bppArgb)

        Using g As Graphics = Graphics.FromImage(result)
            g.Clear(Color.Transparent)
            SetGraphicsMode(g, sourceOver:=False)

            g.DrawImage(source, New Rectangle(0, 0, targetSize.Width, targetSize.Height))

        End Using

        Return result

    End Function

    Private Sub DisposeBitmap(ByRef bmp As Bitmap)

        If bmp IsNot Nothing Then
            bmp.Dispose()
            bmp = Nothing
        End If

    End Sub

End Module

