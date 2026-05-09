Imports System.Drawing
Imports System.IO
Imports MahjongGK.Contracts.GlobalEnum

Friend Module ResLichtkarteDiagonal

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
    Public Function Image_LichtkarteDiagonal(request As TileRequest) As Bitmap

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
    Public Sub DisposeLichtkartenDiagonal()

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
            _imageLichtkarteOriginal = LoadImageLichtkarteDiagonal()
        End If

        Return _imageLichtkarteOriginal

    End Function

    private Function LoadImageLichtkarteDiagonal() As Image

        Dim base64 As String = _
            "iVBORw0KGgoAAAANSUhEUgAAAZAAAAH0CAYAAAAT2nuAAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMA" &
            "AA7DAcdvqGQAAAAZdEVYdFNvZnR3YXJlAFBhaW50Lk5FVCA1LjEuMTITAUd0AAAAuGVYSWZJSSoACAAAAAUAGgEFAAEAAABKAAAA" &
            "GwEFAAEAAABSAAAAKAEDAAEAAAACAAAAMQECABEAAABaAAAAaYcEAAEAAABsAAAAAAAAAGAAAAABAAAAYAAAAAEAAABQYWludC5O" &
            "RVQgNS4xLjEyAAADAACQBwAEAAAAMDIzMAGgAwABAAAAAQAAAAWgBAABAAAAlgAAAAAAAAACAAEAAgAEAAAAUjk4AAIABwAEAAAA" &
            "MDEwMAAAAADZp5qVybcLXwAA/rxJREFUeF7s/V2Mbet614m9Y8yqtfbX+dg+nHM4OGAbY4JpCLgRbUxkJLpRJ0AURbGioCixFKEY" &
            "yYrIBVIUriyEQAKMhZBAkbiNFHHTSH3BRQKJWoroCBTSThAyjYkbY3UCBtsxts/ee9WcIxc1/7X/67f+z/u+Y85Za+8T8ZPGGuN9" &
            "vp9nfMyqWbNqLe2zZWmt3f/Ij/zIe1/60pfu13W9e/Hixf2LFy8O77zzzv0777xz9+LFi5cffPDBB++///6HH3744ddevnz5xXfe" &
            "eeeD+/v7D1pr723b9s7pdLprrd1v2/aytXbYtm3dtu1+Xdf7ZVnWdV3XZVkO27YtrbXldDq11trDeTsty7Js27a11rZlWY5W27Zt" &
            "27Ysy2F55LgsSzudTtvZd9u2bW2ttWVZ2tlmXZbl9Mknn3y8LMtx27bDsizLOfbpbHNorbXTuZDW2qG1tp5Op1cPDw8fb9t2aq0d" &
            "DofD/Vn3cDqdXq3ruqzrejjX+MmyLG1d18PhcDhs2/ZwOp1+bVmWh3Vd13NtHx+Px2/Kd1mWl9u2HVtrn5xOp+O5psO6rvfbth1a" &
            "a6fz1h7bXl6c6z8dj8fDtm2Hs387z+G4bdvh7u7u/cPh8OI8xo9Op9Mn5xjbuXfNTTN9dTwe1c8727a1bds+2rbtobV2dzgcltba" &
            "8Sx7dZ5RW9f14XyeFm3bth23bbtbluXl6XT65OHh4VdOp9NhXdcvLsvyfmtta629Op+X9XQ6PSzL8s1lWQ6Hw+HL67q+s67r8XQ6" &
            "fbRt20nXzrZtp9Pp9NHxeHy4u7u7W9f1PdV6Op2+eTqdPjocDg+ttcOyLIfzqVxba/fnnl9s23Z/7ve0LMvpdDot27bdbdu2HY/H" &
            "Y2ttOxwOLw6Hw7u6JjSfbdva+ZrdztfJsq7r1lp7eHh4eDj73q3r+mJd17uz3bY8noTDeX86nU6v2uO1dnc6nR5aa5+cfb6wruv9" &
            "6XQ6nrf1XNcny7Ksd3d3L9d1fXG+ro/nWMfz/XJ4PB3rtizLum3bejqdtm3bHo7H43I4HO7Xdb1b13U9X8vbtm3teDzqflrP89rO" &
            "193hfH7b+X49HI9HXYvreZ7aTo+X33I478+XRGvrut4ty3K/bdtyznN3ttseHh62x5G3pmfA+bo7Lsuie/yg6+Rss50eeTj33s56" &
            "xdUJW86FtPP1pufBdr4+X51Op4d1XZe7u7v7ZVm287X60fF4/LfLspzO8/nko48++v9+85vf/P989NFH/+KXfumXfu7h4eHj4/H4" &
            "0cPDw8effPLJq48//vjjV69efbwsyyc/9VM/9ct/+2//7V9rrX2sWt42CwXPzOH7v//73//hH/7hb/viF7/4pffee+/XPTw8/Dc+" &
            "/PDD33x/f/+1w+Hw/rqu79/d3b3zzjvvvHzx4sX9uq4vD4fDe3d3dy+Px+M7h8Ph5eFwuF+W5e788F708D5fOMv55lvOD83lfGE2" &
            "9WsnejudTu10Okkv+fmafe0h+KTXBbssi2LpgnmK8/DwoJulnfPqwlrWdW26gCVT4OPxeNq2bTvXrIfkdn4I6QLezjfy5rLj8fgk" &
            "O8c7nrfTuq7L+QG3nR+UJ9W0rutBL4SSnWs72DXis2vL4xNRs9IL7NlkO8ruzKZZnbdt27bj+fzoRnx6cTjPZzs/LF4L1B4Tbue5" &
            "tPNNqhcHPTCX84P8TrPStaIXyXOx9+e5qKbtbLOcr43tXEc7HA6H88NSL/zbOXZbPn2KLeda2jmGz3Q79/4U++y7nl/E29n/dI71" &
            "dF3ohUG9nx6f1sv5WlrP+dt5HueUT18APX1BcI67nd2e/B4eHtr5/DyN4axfff7yt9KUTw/bdvY9Xw6f3h+qzzebU2ufvoAonseX" &
            "3Wa3o86V6luW83PgPMcn/+3x4azZt3au59Ov3z7Nc47xVKO90DzVbDV5HxHLs63r2g6HQzvfq2f14+nUi9vpdDoej8eH8xdhHz88" &
            "PHx8Op2+eTweP354ePjkk08++eh4PH7zeDz+4kcfffRf/fIv//JPv3r16qd/+Zd/+b/+yZ/8yX/5t/7W3/oVP2fPTd35bbj7oR/6" &
            "off/wB/4Ax9+93d/93e8fPnyN33hC1/47mVZ/pv39/e/4cWLF19f1/XDly9ffrAsy8vW2nJ3d7ds27YcDod2d3fX2uNQ2/L41fbT" &
            "ydKJ8ROqi6adL4R1Xdv5xhyeaEEbXogOZV4HbR3Wm/zUl9ZJJ30VT7B31ubrpPN81Hl91Duq1bcU021bqEe4n47xovyk93UVr6EH" &
            "r7OdH24ub3YeJE9xvA7aCI+jHlJsyTZ7+LFe1el+tNPaY8nPZ+j28tHe5Q1zEd57CzP1WrT3OpO/41/4ue3y+N2uWb7ZF3vguoWe" &
            "RvpevUt4fm3n2T88PLTj8fhaP81qlh1ezE7btn386tWrf/srv/Ir//Ljjz/+mVevXv3DX/7lX/5Hv/RLv/Rf/t2/+3d/7u/8nb/z" &
            "b8/vlDwbr3d8O+7+1J/6U9/+Az/wA7/zK1/5yve9//77v/WDDz74bff397/xnXfe+dLpdHpHF4sPdVmWdjgc2na+eNZ1fToJy7K0" &
            "4/HY5Cek9xun4YR5DMGLIcGLij5+MTA2bXvQlzKXb8VDgjl9pr53ZM+bLeHnqVk+z0/cXrOkH6nOb4WfA+EPe7dRn2lmvZkmXbNa" &
            "WSNt0nEL9bXiBUTQzlGt9BFeZ7LzGSnWZl+k8T5NseRT4XG9d99GeF7G0ExUn9fCuryHtG7wYTzKUv3Sa356nkm25ReGJ1+PI7t2" &
            "znX+jqq11tpHH33UXr161ZZl+eav/dqv/cuPPvroH33zm9/8L372Z3/2//mLv/iL/+jHf/zHf7a19itPAW/I+Izt4/2/8lf+ym/+" &
            "nu/5nt/7la985fd/8MEH3/fOO+9858uXLz9Y1/XF4XBoDw8P7XA4PF2QOvF3d3ftcDi00+n0pG+tvTYoDV74SUuDl4572aV4FR5X" &
            "+EXnx8k2yURVF+vxXMrnOf2h4j0v5wuYMbTWC7PDtcs8p/flcq+/6k/Hvm7hiwGP619UNMTmQ7UVfaT6vV6P71AuH5fTpuFcjEj+" &
            "rZihU83E8b5ZE2fim8+GdVSydMxNcblV8AXC1y3ko8xxn5STfv6QT/1yPpL7CzDxFw9H/XkP6tfjv3r1+OPBTz75pD08PLR1Xdvx" &
            "eGyffPLJq9Pp9Ku/8iu/8q8+/vjjf7xt2//tX//rf/1///mf//n/8s/+2T/7L1prH72W8ArenNxlvPgLf+Ev/Jbf8Tt+x3/44Ycf" &
            "/qEvfelLv/u999779cuyvFzXtd3f37fl/N2FXix0IvRioYvBv9LRcPnq7QPniaG+dW4oQXvBi6End9moBul0oZBKLjwWXzQ8tvaM" &
            "x3nwwnRo63if7rfZw0HHrvNj13NNNvtuwHO43uP7tZHyUkZ7rTVjtxfV9ed9pH4472TjD0iew2a+nqsVsZqdZ8aRzGfC+VQxE+7n" &
            "54ub+vI5sTaHNUnme+qS3HNKP+qvl4f3YIOd95XybJ/+jOa12mjDmK9evXrt7a/Hz1Z8+sJ0fjE5bdv2b3/t137t5z766KN//PM/" &
            "//N/55/8k3/yn/31v/7X/1+ttacPqFzKm5XuY/nxH//x3/Cbf/Nv/oPf+Z3f+Ufefffd3//OO+98Y13XF621dn9/3+7v7x8Nz0O5" &
            "v79/GpReFO7u7l57sUhwgBW9E91w8QjVo73LE7LhhaMYXitz+cXUQg7GSXaKmXyJZAu+xfcY7DnlIuy16rcXP+VJcRjfj7W5jR6U" &
            "+mJEOvd1e8GeFFtI5mv/gsdjeo2+dzw/r/tUh9bpiymvIdWd7Dy/ZNr7teJ7Hgv3dRlflGmTak3o+SDkp4cl0bxakc/nQFI9yqW5" &
            "Vf6e19fVvafNkQ3rT/Yff/zxUz2vXr160p9OJ72t1R4eHtq2be2TTz75+Hg8/otf+IVf+L/+m3/zb/72P/2n//T/9Nf+2l/7+Xb+" &
            "5OUlvHklzLG01r7wYz/2Y//+93//9/+Pv/GNb/zHH3744W+4u7t7pz1+YqUty/L0tpSfEH030YqL8nT+gZ4PmQ9qh0OmvIWLNFHF" &
            "oR/jUqZjvwBm0YXgax6zF6+btVbIrmfvNyztmJ99skbhtW44r27Dr1qFHpwt9KD+3aeKIyRnTB673n2W83fQtEt9pXm0YlaKw7q9" &
            "V9oyhsfdMGvOgnmobyE+ZfRRzPTCqD3PoeN2vm92HiQf1a/1UryVJPwLD5LqEPJroTbuU8/sRfoFHwbwXpXn4eHhyUZ7fUei72qO" &
            "x+OT7HQ6tU8++eTjb37zm//vX/3VX/3P/9k/+2d/68/8mT/zf2yt/dJToh28eebG3P3Yj/3Yb/1tv+23/ZHv+q7v+u999atf/W+t" &
            "6/qhPjG1rmt78eJFW88fWfMXg4bBpWOdDK19qG7Hk8w1T5rnSngOnmjX9Uh1EsaiHfuVzPHZJDlRD1V9yY/2wm1po2OfXwv10o7n" &
            "x/XaaKNj9eb2fg1RL1LtaQ4tvBWmG5U1eby0bmEW3ofLhffvMt87epGlTwt+npOxuHYYV7iP18Damd9hjdw7lKV8PSr/Fmob1UF7" &
            "ySu/NBPauI7rxX6Qvp2vSR1LrheRtP/444/b8Xj8tY8++ugf/8zP/Mz/7j/9T//T/+Tv/b2/91+31p5+WWaGN7vu885f+kt/6T/4" &
            "nb/zd/5Pv/GNb/x3vvSlL337/f394XT+uca6rk/fdTT7TsThOsEXnYablcduI9LJEIzdigsk2c1C3149bUftrdAzX8UeX84k2eli" &
            "XuxbdT1gef49nteh8y1b+Sue50/XBnsa1Ts6Jh7Hb1TpvC/ZSi4Za0r+gjL6aj6uk37B2zqSjep0aEd9BWv2PE5Pl3pjPS4jKWaS" &
            "qQbnhI/RtiKPy2jvyM6v54TXQhvOw/Vaa9N59+OTvWiczm9r6YuMX/3VX20fffTR8ZNPPvkX/+pf/av/w8/+7M/+73/iJ37iP9/z" &
            "i4l19+D7vu/7vvQn/sSf+I++53u+53/+ta997Q++99577/t3Gi9evGjtPFA9OLQRynwtH39QSLYVF53LXc+TQWTLk+K6vaSYJOWg" &
            "36hf7b3vXk7GdzllDnWpnmZxNvsKXV9IpNyylV6by1t4K43faTbETf1s9kEM3swpp6hkXpN/xS9SvyPo3/OVzvd+XjgznjO3T7lG" &
            "a5L0nInTk1OXZHtg78LjzsaftWs4N1WekQ17r2y09xcNrv07EP2s5HT+TuTVq1fto48++uYv/MIv/Gc/8zM/87/9y3/5L/+fW2u/" &
            "/JSkw6dv4Hb4ju/4jg/+5J/8k//97/3e7/3Rr33taz/4hS984b1333336cVD331orWNtDQ8Jrd3mcDg8+WrvfhU8KaKSOzxB1+J1" &
            "Mm6vB+p4QTEW1/QXtCN8eOjYz5H2vfPAGmlLvWy0r/L5caqBMdPcvCf50s73Dn17dcqeOm6EMq4dr8dhDZJ5Tq9NJL9qLSp5sxw+" &
            "X9dxnWQ8TnbeE/H+fU+9x6WPZJxhjxS3WmvPuLRjfb1jj8W41J3sZz3W5/3d3d23v/vuu9/5u37X7zp94Qtf+Jmf/umf/uZrgQLD" &
            "F5A//If/8Bd++Id/+I/+9t/+2/+XX//61/+Dd9555x1/yN/d3b329tWK7xxaaNS39ILj/jwBKYYPSPoRbnNJjKRPMavY6pM66bmN" &
            "9Nxo14M27j8i5eI5ZHzB8yoquaPrItkql+v0BQn9NnxH4VDGuIzlUFbZtaIGfafkNjxOMX3t9Vb+ru9t7svNY7mNrxOU+9rn4fsU" &
            "t6qnt9G/2Q+f/St47Xnc2xRnO/88QseMWdWUjh3Kl/CcTH6ttae3sBb7xWy9kJxOp/t33nnnN7z77ru/9e7u7qMvfvGL/9VP//RP" &
            "/ypjOG/e1a+z/viP//h/9Ht+z+/5X3/ta1/7A++///4L/bD85cuXrz38W2tPf3pkCx9vfAp4fvER/qDxfcNNpCE5SSZ5K4aYHmQ9" &
            "Lo0hv8o21chc1HlMxuUFJJnwByh1PsdKnujp2lnvdSZ770lr9SedbjbZsVb5+lw2u0mX8JFe13vOCsVo53p0bbKGEcrFfo7nv4XE" &
            "Orwf+Wjte6GHgfdEP/rIVsdJx9k5yumktcu8R8ndhvUwHqFd73yoJ/pwTyp5M51fcws+Ok9771HHvvd5uB9l6dxs+GG6Nv2uiD69" &
            "ta5r+/jjj9vHH3/cPvnkk3Y8HttHH310/MVf/MV/+M//+T//qz/xEz/xn7TWfu21hEY95dbaX/yLf/H7fu/v/b3/m2984xt/9OXL" &
            "l++/fPmytfMLhX7OoRcQHXMQOpYu6R0+6BIcVM+2FbmSjMzETiSfDQ9TP+k6buErzxEpV7P4Fa6jrY4pd7x+rZ0Ub4YUJ82Jdgn3" &
            "a/DxuDPIV/aKewo/eBXMl/bUV7Fa4cs4vfunJ2N/rk+6rZjdghcbx9eaXdLTz0kz5TFlzEuZcHvGviSvjlNO+qVrVHMklCmH163j" &
            "Dd9B+XdZ2l69etWOj7902L75zW+2V69etVevXn30C7/wC/+Xn/qpn/oLf+Nv/I2/24o/0Fi+hfXH//gf/3U/+IM/+KPf8R3f8T/8" &
            "4IMPvnR3d9fu7++fvoPQ3n92wYtfF5NeZPyFZgQHoQHwWGu35UYd1ykeZWljTe6nHCkO86d4shvhdpy/MxvvUqrcVd7KvrpR/Qbj" &
            "TZZuPtenY60Zt9paUU+PGRvheRJ+3TisUzF6sRzmTbFIJW+Dec/S82P8qt/evIjb0F4wb5uw3SZfPJxt4jlBe9/zWGvV4jW5Ts/Q" &
            "5fzW1ieffHK3bdvXXr58+c5v/I2/8f/xkz/5k79gIZ+oXkDe/9N/+k//j77ru77rf/Hhhx9+O3/OwR928zsPbW7fO9nux4ey73ns" &
            "W4pZUQ1Y2yy05dqpaqSPz28Gt2esS0n5GZv5aD+iZ9/TVcz4JBv2lXAbHadYpLKprgXHbWZqbJ18bwOvd6bunq4He1ScdI5GOajn" &
            "ukfPttLN1OX9uV2vb23UkSqe5HrLa/30ZyIvtm37ysPDw79prf2Tn/u5n3vjb2ilF5DDX/2rf/W/+7t/9+/+X33hC1/49+7v75fD" &
            "4fD03cfd3d1rF0r6bsK/O0l6seDhdy56OJAtXKwc8B6Uv1lsr6vC7bwOyiQXvZjPBetKNZKejlS2Se6zSlBezdGp5GTWLnGr8zbq" &
            "v6KXf28s4veUy/bCGFy3C+PO4vex78VS3NNJJpKP1kknfSX3fZpPD7ffwhfUbuP5Uy3N6tzsO5TT6x/3/WDbtq9+8Ytf/Jf/4B/8" &
            "g3/KXzR84wXkz/25P/e9v+/3/b4f/bZv+7Y/8O67777Q37PS73n4dxvV21Z68aBOLHhw8e0fDoL2LlPT1F+K56yGLpjTa2qdk+Z2" &
            "jPFcPHee2fizdsTPS4rR+0Lls4LnP51z9kIfx2fQ8BBinEup4vTqctzf70v5z8YhvXucsbl3UgzacS1ZkvdgrlQXbfYyWxdzei0e" &
            "Qy8esjmdTsu6rl+5v7//0m/5Lb/lZ/7hP/yH/+IpEF9AfuRHfuTLf/AP/sH/2de//vX/yTvvvPNlvXjoO4p2/uUw/1mGLl69lcW3" &
            "tJoVL5lfVBt+sON6HjfE8kHQzpEtt8SGC5U+I3/RO7H03RO3B+PcKqbvK3p6r6FnN8MS3jq7Nubb4pI66eNz5L1A20tQjOraHTGq" &
            "YaRP8Hwnko3b+nwYY6ZnPheEz76Xo4pNuxael0nH48RSvLPiMu216Q8v6m2sh4eH9vDwcHc4HL7x5S9/+dX3fu/3/hd//+///X+r" &
            "HP5l2/IDP/AD//5Xv/rVP/ruu+9+1f+2lRrRz0GeHGxgKkbfeXhzelGRT7NXOr3vxnhCjUnvL1C+OUnmeEzCF7+9yLdXg5+wVEfP" &
            "l1QzuAXsxfPM5KSe60pWsYSH5jWk2ffw3n1fseCTiZt92CLBevbM+lZU1+SIqsYUr7KdgbG4JlUuypON48+cZufWn4cixZqVtVAb" &
            "nxd8p0Y+rId6kZ6f2vxPUbXzr2u899577cWLFx/c39//tz/44IPf3vy/IX6ybO2D3/SbftPv//KXv/y9+hPs/CG5F6Hj7fzKtuL3" &
            "Oyq77fyq5r9gIzvpucmfcenLGF4z4xHaXYryCtbmNRHmvcRnD735UuZ2acacm+qt6nZm+hQjG9fRlnl6cdSP980ZtBCTpLkuuM6p" &
            "E0nneWhL+x6e3/0op95tWmfe9BEpJvtyWSLFoE6kGLQRtK3y8HmYzgN9WBNztY6vdOnDRG0w9ypeVcNiX/Qczn9lxH6W/d3vvvvu" &
            "f/zH/tgf+4rsn15A/ubf/Jvf9eGHH/7guq5f3s7/R4e+CxEqxl/plJA/8+DxYn/kzb/ruJQZf9nM2F5COgFilFMz6cUge2xHeH1b" &
            "8e25I/2C7y6fE+ZJa8p6sOfq+uB6D7NzTPiMR/D64f5to7y92fV0CdnP9FT138tZfSfYBn4t5BF75YL5uK641o73hFjshWQ9/6eA" &
            "67p+cV3XP/qVr3zl+2SnF5D37u7ufvDdd9/9Xeu6LsuytPv7+6cXBQUSfvPpIuaAlvBV6Vb896lOT/fcfJa5P0tm+5Yd7XmOr+Fa" &
            "/xG6JpVnNl+v5+eCORO892b7momdGMW9BD4nSE+XUG9+rqvznnSJnq515lnJRXVPtWIuvvbj5N8G8Ym/YOibAh3bdyXf823f9m3/" &
            "gx/6oR/6RtMLyJ//83/+N371q1/9D9d1/dq6ru3ly5dts7em/CL1plyvtdvIb7P3fjmQhDfL2CMYn+u3hc/M2VNPuoASs3Zij21F" &
            "1d+lXNNDdcy1ckhG2730Yl/LzGxnbHr0/Gd72fCW5qXwvPRiJV0lu+S87LEV9Jl9htEvkWx8Vr5nDveVLs3Ea+QLiR+/ePHi/uXL" &
            "l3/k27/92//Qr//1v/7F2lo7fP3rX/8th8Ph39u2bTkbvfbdh7+IcDCe0NnswuIPy2fwXIz9nNw6163jXQMvmktgPylekvXYa1+x" &
            "J84e2x63mOkt8XpYG8/dDO7PPrlOVDlTjazX7XrHe2E+1jCC9qO6ZmInHWNX+Ftxvbooc/i85fFi//vmq1evvv6FL3zhu//QH/pD" &
            "L9Yf/dEfff/999//3e++++7X33vvvbaua/vkk0/aixcvnn5p0LcqKeVis19M2cJ/cfl5pBrgpVziy5OfqOSJPbYVvQvwbTJbA+3S" &
            "edhzbmftnhN/ELA/p9I9dw+qi/cQbXzP4yRLPtT5WjWIqoYkG822nW38h9r06dVLmDf5JFkzH3+HZzZekvvzWTPTNxH6puLly5cv" &
            "TqfTt//8z//8++t3fud3vv/FL37xu1++fPnB8Xhsd3d37d133336a40cvC6MdHGoIOn1wqFXSMZK0Ibrt8mtc6cTdil+kTwXfgGm" &
            "PEkmejpn1m7E3jh+DX+r4Oec/bos6ZxR37RPMq6vQbVXMXty6rT251N6Rvk6xZFOKIbbb/g0FGNRThvKHa4TjMMt2WntOpGe63rx" &
            "0PH5byIeXr58+T1f+cpXvrF++OGH77/33nu/btu2gz/0+XlgspzfuiJehF4Vm/2AZmYwM6Q4SfZZkS7a0Y070lekC+NaerF4gT4X" &
            "nMc1189szcnGHx6XovyzdZDkMyvrwRmT7fyQnKHKXckT18yI6Fnkz6RW5EgyyV1PneupI0nmeA3cRtBeW+/cKa72vWtBLyTr+X/1" &
            "fPny5Ve+/vWvf2P9whe+8OXD4fClZVkW/zbFh+4NqLD0cw+398Z5Anvs8eGwbslMTOpnal6KHzjStzqu8Dn4+ZMv14mZWfZ0ZBRL" &
            "eN4Z+7bTp2fHOMnO17RLG5GMdpX9CD+HKQavnWRfXQ8u87i9mrmWzG25dxsnyWbp+UnnvXHzt4HoS3/BdYXH7M1eyJ7z6PlUVH2K" &
            "Bb+X5G+Fse/l/MuGrbWvvHz58jvWu7u79+7u7t5d13Vzo/RemAKyUTbJxO4/w8xQZmzeNqP+OCenku8hzeQWcZ+L564tzYPM2LSi" &
            "1hnf5LcXxuCapHuRPlyLSj6i51fpfH7+7OHzxeF6hjSPhM8q2VRyZ8amFbl6frTv2YpkQxlnzRcP2stuWZb3D4fDd67n7zgef/kj" &
            "BHTOjm+cZKJC9jAT91sBzaza96DNpfMYnfzPkl5twm24Sb8HfzjN4jn25mOdI3/2R3nP5hpSfB3PQj/WODr2c8JnD+vo1cfYo1rS" &
            "vZVkIuUkXl/KX5HsFnu7lj0wT4I+PRTHf2advhOT7ZmX27Z917qdJXyA03nDn2+QLNm5rDohonfSRlzjO4v6YZ8O9T3b52B2Dr26" &
            "erpbwBm53Bn1QXtRyUUvrs+vZ5eo7Ef1VKTrjbHchpt6SVsF44vK59IaXb4X+jCmy0ll29Cjv8/fij5dnkhyylItnivVS13lm3x4" &
            "7NcDY27hhSR9V7Jt2+F0On11XR5pi/3OR/rheEPDvLhYEEmyPfT8WcvboupVSNezIT3bNP/UezjZ3bjfqvDB2OsxzWkE51bF78W+" &
            "5BoQKf8lcUSqxWUpPnujfpYUmzLqRSVvg14qfYVfT6lv1usw54y99smGz1NurnNcl0h9aZ9eKCrZ6XQ6nE6nD/VK8VpUN04s4QfB" &
            "qWFfy8YbYDMVVR2fN0Z1ahaag8+K85xhdn572VvHiCpeJb+G54jZQtyZ2dNnD/Tlepbq2tI66Spox9i+pqxHz4Zyrnu47Yxfqn8v" &
            "ya+Kx/potxU/b9ZxBWPQb+Q72s52y/F4/PJ6/j9Bnj6v64mIAlQ3T0hCk9K3FboU41uBXt09XSv0SXYrnjN2wq+RhF9HaZvFbf2Y" &
            "X8i4PDHKOdInqlwJxufa2TOrkb7HjO/IZqZGR7Y9n6RzP9fzHFA/Q7KfiTPqhTW7na+TvgfzcgbSVZv05+9C3lsPh8Pdtm0Hvm3F" &
            "m4wFSic7Bd3Cb4D6Jn1VOKE/8caeCw6xl5M2buc90J/9VTGSbAbGd6pc7fy7PJVuFubl2tmK9/GFjinrzY51J1kPxm8Wg/OpcqZ+" &
            "tO/5yc59/Heq6JP8WyFPstaphz2w/h6M2dvSB3AUn3sn1eSb5LKlbwVzeZ2+F6rD83p8xiNV3R7L83LzOIR9pnMpqrime7m21u6X" &
            "ZbljYBmlfUWy8+Qj/xH05/o5qHoZkeZ5LczNdQ/W7utUK+0rZMftW5FL6tbs/Eb0G3JEsq3qoJ3Dc5t0fm5oU5Hicl8x0l8K+9nw" &
            "MK1IvYyYtWsD254u0etlRu4zoiwx0mnDD9Xv1nVdXy7L8vg/SIWL1J1dthfG+KxJffVg/zN+6eFQ0Ys5o6v0wu3YSytqnYlZ0dM9" &
            "B8zHdaKayV44t0vwGKmWvS9Mvb6SrIXrTMcu58xSrGRf6Xt4DN/rmDWkF5Hk67qkZ9xK1pMntvDdSqKn7+laMZdRfa7j9cV42A7r" &
            "4XB4qe9AKmfKelyrnyHVtYe9vnvtCee6h2ty733oNKv1mryEsbi+BI/B45n4MzZvC818z3naws8iRz2l2WhdyX2d/Hsk2yRL9OyS" &
            "riejjr34nra3grG5djlhncT7YVweJxuSnhtbfstsXQ+Hw8u7u7s3/uhVlYCByai4a+jFrnS9WmdJMZLs88Sl9WmGo/N8Kekcjej5" &
            "uK5n5/TserpbwuvVZz2ae+UnHXtIMskTlHM9Q/K55FzNkOKO4msms3aJSk68pl68RNUPzzvxPDx2m4YvYHxznexRx7Ku63rXWlv3" &
            "XMAJD85CuH2W8ESM6A13DyOfcHLeWF9Dr3blvmU+cYuYqeZW3AyXcE3/7sttL35+eL6uiUt6cSjXmvXMwFiiks+whe+8iMevjkf0" &
            "ZtRC3LSN6iQpZiLJmZvynk3D//NenWvqlmVZ19baYVmWp49geXAP4vsUXFTyzwMc2ucRP9l72GM/c45G53mGPTX1GMUZ6Xtc43sL" &
            "Uv69M0/26TpKD45rUF5/Xoy4ZX4xk9d5jhpGpPNR0bPp6QjPfTpuYX5cVyzLsqzbth1aawv/Boq2xT42qP8jdzu/H5YeMlwLj5no" &
            "6YTnYx79Bj1zj/I22NB2j19PR71D3RY+yjqCNszNzWEuxroU5dJ1lT6e2cK5p1+ymYV9M08FfSrbal6U9WJRvoVPFemY9yfjeT3M" &
            "w1rdX3LW7bFS/mQjWe9XAxiDsrQphsv4Q2n26MjX8Tguc121rvK0s62eqaxPeo/rqIdeL4w5snfchnWczn/GpPc/yC6f/tWSZd22" &
            "7V5vYfnWO/mERaTEle+/499xDelaa53rcIa9fsk+yfZyaYxL/GZ8ZmwSfi72POhmYV1ci0pe4bXK13vZGy9RxajkM2i2PuM981aP" &
            "3qvguVuXZXmp30T3xAv+c/XqxHOwTOgk/+emV881PFdc4fGfO9e3KpxLdQ1yXckuYSbOrE1Vu2TUJWZs2g67PaSYSdYGXwU7o/Us" &
            "e/yqZ5THqOqVLMkd5kixE/RrhX2ym0X1+3dl6Xxt27asrbV3/WcgbsgXEX+bKBU4M7jn5m3kfxs52gXztBP7xpbo6SpGMd8Ge/LL" &
            "bsa+istZphkkmeQVlY8zY3MN7Id5tPZ9tZFtx99yqmLMkuqoco1I9nwu+uak/IlKRznXbfLFIfnNQt+l+Lt9x+Oxrdu2vWitxT+/" &
            "y0H5wJREa+o/C9g417fgOWJ+Fnyr9rG37nRdMoZuiEuhL9fC5ZVNs3q4jfS9mOm+ZEwiGfc9WEv1c8lqXR2ndY89NQuvO8HrqMJz" &
            "MybXkvlex7SrqGpivBm24udhLtvsZyVra+3udDqtnsBvOgapim3QzxZ8K95GvreRw+FF1Jv92+Bt97+XdM2SW/dQxeO5q+yckQ/X" &
            "s/g8UgzJeroeqjvZJll1nryO5DdL5VvJSepnprZk72vqXUbdHmhfxaJdj/Qa4HvVvp5Op7vT6bR4M8kxnXCRkrWdBX/eeVu9VDO+" &
            "Jbfq5VZxngNeixVvo4eZHDM2z8Eorz8XKKO8R/VsoX4PySfJWkfupJ58zeNkT2jD9Syc1wjmnMHteJ507N9RbucXkPvj8bh4gG3b" &
            "3vjB+WKvOh54Cx85ZBLfKtJFJTzvHmbtWaNvPsCePeU9km+C9dN2Jg7PYaLyZV8udx2hvdOrmR/19euOjPphfB5rS3FSjZQxvttw" &
            "neIwL2uoZP6xUMYQysGPQHsNQv7uo011uo/7yt5lQrWlOMzhsbkR+jrVmjUndJ2lLeVqqJe9pLnRXj7ek8fxTTH9nLsu2SdcR39H" &
            "dTGeamznetbW2ot2/g+lWJzD9duCDVe6inSD7WEmx2fBbF29+X0rwwu71xt1vM6pvwTWkmLO2BBev167y9K6eig2q2WmhgT9RmtR" &
            "yWdhr5fEu7Z30fN3HWcv2EsPj6E41bNaVPIRXpfPivGWZXl8C2vDL67thT5s8FJY8OcF9sX15w0/+dVMK/kM7ss4XO8hXbTXwFjp" &
            "vNGGMtbE2Sb9DLTzezHdl1z32IpfAGTOxIyNoO1o7fR0rZgzSQ++Ctcr3uxMmb+qa6aGkU0P+XNLpLqSbXXNua2/w7SeTqdNSncW" &
            "LC4ldahXISzoWpinYtZuL9f0dImP6J2Dnq7tnMUe2zaRew97Y8k2+XGd6NlUOpdXeSmTnGvKHL9WqmuO/mnteXxd5U9yrhPM4/KR" &
            "TSLVSF/GcDn9ksz3nC3p5XIZ7YTrKpuE1578ejoxm5szcD++NqwPDw8P/h4ffwNdhlVCBWTSa6jyzQzpuXiunN7Tc/XGuFy7/Bbc" &
            "uhfOh1tFT9eC/tp1BeucqX2E/NN3FtK7na99fwn0ZQ7asJZ07DL6Mh8fYtqnWaQ15aP4yY5r2iYZ5bShPOn4sxKRvrCo8HjM5/Rq" &
            "Ug3r8Xg86e+ejApgoOrFwwvb01gLOf4dc1QXA2VckyrOtVwSc08ts3YtxE3X5554yV94HMbkOkEbr13HzKG1HjK3uAeTrAX5zJqy" &
            "WSpfl3MWCdm7T2Ur3Ie2SSb5CH/R0w+nU55evf6W0ug8ewwiX8/vuVyvv5e1Ho/HF/oYLwPNMGs7a/fv2E+6GBKzdteQciTZiEt8" &
            "ZqhuIK5vQYrJ/LxJHZclG+orXQv3X/WiQr8Z6MN1ko3WhPpUv2zSrJ6LS/LwvHFN3KayTfNwUtwK5duKT6BJdzwe23o4HN5Lr2Be" &
            "EANV3yom2OQI5U22zDlDiiMUq5czzWMv9N0zk8ovyZiHVLrk15t1rybpZuuq8tBHNn4dVjZux7gJxnGq+ijrxZBOG2MJl9PG/SkT" &
            "9KnwGVV439wc1iBkm/SKUcVUbdU5VDz3l23Vk9tySzZa+34Wxq3Y8OxJfaV6Gua+hedz9Yxe7JN5oqo11UX5uq7ri3VdFz/JPOGS" &
            "cZslDeDzSKpvZtBvgyr3nvPQwkUhqvg9kk+S9dhr78z68vqj3zZ5w4+oYuy5hlRLb+sx0pNUG2NwvZdt4u1xcW2uWzGad5rbHiqf" &
            "3pxm793KrqLydziPRX8f8XA4PBnTSHhA6mbZ60d7rm9J6vmWXBN/xpcnW4z8xKyd8IsoyZ+TS3MmuyRzRvo9zJzHW5DyuIz7nl5b" &
            "eqDMwniX4LW4rII6ris8z0yuSt5jr89ee8Jzxx4VX3ue52Tbzj93ORwObW2tbcuyPGncOH076EGYjOxtvrKv5CPY9DXcIs5MPdLP" &
            "2DqjLwKELqjq3CVfj0ndsuNbYjJr1wbX2p44hLVLtocUo3VqnjkHt2ZPzMq2qndEFa9Bl+woq2ZNZNezdZtUh8srW9LTiV4srnsk" &
            "/70whvfbcK3q/Ouvsj+9gLgzPxWQmk1FpwfJLPS7JAaZiVHdEOx5JtaIPTH22N6CXp9JJtL8evbOzPmm3PNJR5tKJqjz3qsZJFTL" &
            "yD7NyGFPo3gj6M+1U/XMmqWnXfIlPX1PN0NVV4Xbj3xG+gR9qjkS1cP6qrVks1S2jMefObmeX/Asj29hbev2yJOihQFzELck5e6t" &
            "Z7jEp4W+JXP4alxBP1HJe6iuVF8i2Y/2IxhPMtpwXW30oYyxBB+4PZiT9pVcuhGja8AZxUv9V1sPf2GTbbWvSHr33cIXmmnzOLO5" &
            "HebUNsusX9KPzm2qbQa33cIX6XrXR8e+9v0MzMWtknOTnfP0ArIsy0nD8ocjX2247uGJPw+MahnV29PNUJ2EGZJPko1gj0vn7yQR" &
            "1r+F98Ur3+dgdP3tpdd7u7C35JNkZMZmFu+rN7N0/isZ9716aX8rRnlbkTvNYBRHJLskq6At5+o99V5U6EuSTZKltfBalJ81nu//" &
            "bd227WSCpyFv9hd5pe/9b4ROzyY11oKPCncdayTeINc+AG4z7PVZwgP6Eti3eldMnVw/yWTDp2Bkt5zPKW1THK3Zk+TpnLDmEV5X" &
            "5cPaWAvrSrBu/qdH9GUtVQ7KmcfPnfvwmHGJf4XPXOmrf9bveM3yr2r2fNJL5zLfWF+KQ6hL10MvDuuRnfeWfFkf47iNw9p6VDP1" &
            "TXrG9bXb+zOA54+9sHbZLeH57nF8bWxra+3o72GlkzVDCL4L+nM9omd/ST9vi17dI3oXRcVe28q+p9tDOjcpdpLN4r5VjM5N8hoz" &
            "+pFNG9SUZnINKRf3bsd6uHYZnxf0r3IzV7V/DvbEZs2UJ6jjmjNLSD+yE3xxTPsZUj717l+0nF+stnXbtiePqjE5sZBLCpwl1eJ1" &
            "cJth1u7zyKhPzoq47yWz6zGKs1fH+no2e+AMKt01jM6Dwx5mfFOdjJHieC7u9+L+VQzm842wnmTjjPTOXttkz/pTX27r+4TrdL58" &
            "Y0yS8jlVrUnn2+n839RS1tvWZVnu/LcIlShdhLdEeUayS/FYM8cV1RxmfHvoBPXwE+ky389S9ZGYiT1TB2t3ks77HW0V1HFdyW7F" &
            "TOxeDzpP1Pd8Wkfvch4nGyfZJzteW/JL/klf2TmVXHiMWZi7oqqdUDbrN4v78zjJmKuqQbLj8fjG22DS+YuGy9ZlWV4uj7wRWAG4" &
            "9iKovxW6KHlxXorXWR1X3KoGMZPzufCvdBK8cHqM9BX0m8nVg9dkj73n/pZ4jV4zN7f3PY/T2mU9P6d3PbSBL6n6oz7R0/Wo4u+J" &
            "xzpd3tO3oPN1kkvWmzlhHB6zNuaijHod+4sHCfJtXdf1XW+EQZ+LmeHN2CRm6p6xSVzqd0sureHSeSZ4EfZqmrVrF/TWs6eO6+eg" &
            "mvGe3G5bHfva95SJqq499GpxerpeHZWuFy/1XjFTv+JU+h7069WmddVzO+sYj2vf8zhR6VM9tJVOdWzbtqyHw+GFPonDAoU5vCF3" &
            "Rl/JOPR9G7CHz6KGHqzPqc5BG1yEjsdQnG3wd4qqnAnazfi6fmQr2MMMe2zbZO2J5MMetebcezkruZC+FyPBGoTHI8zVs03seU5c" &
            "CmvhOsGHp3xuVSvnJfbGTzEkT+tq3pQv4ROJzeIcDofXPrW5bdu2LsuyeqCqiIZXIF8nFNOHJj/mmIUNj2De5M91qld29E92I3q2" &
            "Pl/aeX5utEkwnudgn07KQ7nrZ2ci++Q3iuG10KbyTfW3ibyVPOlbOAfJJzGy44yF594GXwx4DI+12Fe6snEZc7Bn1kMf2mjtuUjK" &
            "MYrZw3uq/NhPykk5fXqwV8YgWziftPV11aOv/bz75udCLyLupz9fIv1Zvq2ttbW19lqVLLKFG1CkgfD4ltwyJnuqhj4DbX3tJ+05" &
            "2VtzCzOoZLcmzScx049fczNU+Wb9e+gcVOeC94vvR7gdc3C/h1P4m3eJFNvzJn2aNW2T3174YPRjbiTJq/poV8kSKQ9JtVPHtdsm" &
            "H5el8yGqa3PBC73ttzf//9ozLEQs4ZfPZvEiLiHVQ2Zs9sATwxOyh2t6F7O5Z2utdHtrreKQVA/Xs+zxm7Hd0zPnm/pKzNi8DUZ1" &
            "SD+yE5WdyzkjriuSTZK1znnxXEnm/olk61TxXEcZ8fq0r+LSxj8lRbsqBqFdsvcXav0eyImGWodXnDdkFTMFj9jTfCsaHlH5jPpL" &
            "VLVyfQuqPEmemLG5ZAaC8au6kuyWMG+vJ9n1amK8Hj3bPeeK0LcXw22Zj8fJxvW+T7qKkb6FWVW1JBunOr+9eNRRRh8hXXpoV8fM" &
            "Q5g32RCPl459neIxH+0Y12Snddu2hQ4OT0hl56SEPO7Rqyex116MfNj7JYxyXIt6n8mTzsktYDyec+rfFr28VY2+7/kLfVHlX1wR" &
            "nqOZuLegysPeK56rXs54FJ92I5/0RS7PzSjGiJEvZ0z7tKYPdb6xnxZ6nIF1EObdXv9uZ1tPp9PKz/5WhchmlLQhBofRg3ovPLFX" &
            "XrHXfkRVczVbx3tO261QrFvGdHpxr+kl+SUZmZk9mYn7HDCvr312yW7vbHu2zHsprCnF8tov6SO9YCRZD8+ZauC6deLSlvG4lmwG" &
            "t0s9LoMvaLhPeSX3t8ewPb6A0LGHN10VmEjDupa9sfbU+5z4LNI2orLRRXNJn1XMEfTjmqQeub4FszHdbuRD27SNmLF5G7CO0boH" &
            "Z7DZD14Zx9f0mUW2e3wa7NN9wtqoo6wi9UVfl6fYXhvj0XeGXq89lEMvGjqW/I3/D8Rfwchs0kvZE79nW+lcXh3fglG8NFvRexHQ" &
            "SWv442mJ5N8GF10lr3hu+8RMjMpGvVf6RDXHipkcilnZSdbT8XiEx+TmNi7jcaKSt4GOsCbW5lTnhL1of0msPfTuV9+7nDLJ07HL" &
            "Uj/JNtGr0Y/Zj+f0F4/W2rYuy3JMzh6YDzW/AQQb49pxXc+uoemRbdLxZq1s0nATVQxnJpbbcOYjf8/N45n6evRijOpqwcavnd65" &
            "ZC6HNWljTJHytSKObDx/qiXJElXuHqxpVjdL5Ud5lYvHyYZQ15uJbGfiOsv5nvEvpiRrFk9y1iB90jlJX9Xqa7dPsVlntYlUB1H/" &
            "9BWS+Zwkdx/lSDZiWZa26v9DTw6XcosYe6ka3VNL78QkqtiV3OnZ8CS6LNHT3RLmmZmXzgV9L4Ex9sSt7CjX2veVja+5JRh79DBo" &
            "yD9jP/OQEbTh+lqqOQjvLc27qifFTbNf8JvVbuOb3p7xWE6qkfpqc5sK1efnreq9deqoZIT50vWSatc8fabrsizbeXtt0AxS4cGf" &
            "g5kaCGu5JEYixaGMa0cX1YxNBXvbC2NzXUG7qs4kax37ETMza528e6luqBGzdfagP2NRX8mEPyiaxfP+2CfXl9CrqRV9Ud6rw3WV" &
            "3aXnsBUxWXMP9e/bHlJ+wVml2H4Nc0tIrn1Vt794nG23dV3XB//VdQXwV2TRK4LJbs0oPmur6hxxqd+tSf1S5mvqbkW6kPw6oY7Q" &
            "hmvJrqHy9zoTqqXSJ9hLIsmTrBVyr8mvx16tI12CD45ET5dgrpl5tUKnnnwT6V7vzSrF1zODz45kK3o6J9XsjObKmtogN+dUbQ7j" &
            "t3Mc/JzjqRa9gGhr5+9Ajo9+j8buyCCEBX0eSHXu5ZKT5/sW6qA/55ygXjEZ+1pSv62oK8lEpVOvlX4PjOEzoq6iZ6tZcB7uw2Oy" &
            "hE8fOWketPe110I74rGZw2F/Tk93Cb06BHP6fEf+bkv5rO8IjzMTt03E1nVCO5dxLiL5zdDz877SNxDbmz933NbT6XRyRxn1qAq4" &
            "NcqzJ5/79IY1w2gOlzBb0y3qvzVVLalOrq8l5UhcMrdk67KkJ9ULD1EsrzPFVxza7H0xScdiVOteUo5r4Iwu3RjTY6fj6sFekfIt" &
            "O/7kU/K/lBl/75V71uHXND/5ua5rW/W/EcpJxr2Lq6f7rPCmZ4YoZnuhHXOM1pXMYQ4y0o+o8ie5y0b6ikvOSZU39T4bs4fX55vL" &
            "kn0i1XgJisN6qEskH+/H2Sbv9+eCORd7EWatDb352yzsOc3PZ8BjlwnWJmgnGeXeS0XyuwXsq+rRcR3tfZ6au3pbD4fDHQcuJ//Z" &
            "CD8qpz19PfhogLN4nCp2ZePQl/oWhl+RfJ0UJ9WnjbmoV0zqmYc5RVpzS7rqRk1+9HVG8xKpV9f5J0A8pstcrjgeL/kJr5+xOIuG" &
            "8+S5Uv2zpNiCXwUK5mRfgvU32FLHXKzHqeQO4znyTzWKLVyT1AvOj7asN+lTDqenE6kOfxAL9lTpqpx7dFw31Mnc0nvtTX/OPZ04" &
            "/iDFdUxMRvoZqhgup83eNenpe7qEnwQ/Gbxo20Tskb7Cz90I2qY6LyHFSTKH9fKc+5ozpn4G3jDCY22dt3ZHOTnbW5Pmo/ewtWaN" &
            "t6yFOU6dvwyb8HN3C/xFlrN4bvxa4lx4DmTjJH/qeB263RK+GL0Uv7fScTu/eDw8rQpmC6oavAYOMcm4nsVPas+/pxvBwXM2o9g9" &
            "fU8nkk2SNZOP5rEX7539j2Ad6XztjdkKH5d5nioXdaTSV7LeVvn1SPkZz9eptyoG2fAuxYiUU/LZGAnvzetJtXFNRnpSXePqKX1R" &
            "TlyW7JgjxWid80cZdY7nWfAznbN8W7dteyVHL84/78uBPDczzYlbyQkH/TbYk2uPraj68V6T/hJufd1cUxd9r+3zkr56+ahL9XE9" &
            "wm/8GXp2ruvVUcXwftgb1yNGtt6318N1j1EOodq9N8qTzvcizcT9qaOMa8okdz3fCqz8Woip43VZliMHOxr2SH8NVQNtoOtBPw7D" &
            "6en2MDohMyTfS2PO+NyiZuFxLo1L+23iz044yZ8yMdKRmfzC/VMswRp6c5Ms6So8XrXJjn4VW/hZkeO+jMP1CK+v2hrOzUxdaT2C" &
            "9qm2pBO969j92V+KX8E4aU+q2Nu2tYeHh3Y8Hp/eElyW5bS21p5+C33U0Cj5LalypDpoy/UM1eAq9tqO7Knn+hKUdyYWbbi+FMZR" &
            "Pek6a8F+hj09Vhvh/ZBsWhGzdx9VSFfVI2Z1Ho+6RKWfmcE1MOYls3Nm7Spm51XVyK/qK9tW9FqdQ26On59kw+MUlzqSbFtrp3VZ" &
            "ljveLGbwhpMncHsO4rmpGq3o2fd0iRl7zqqSJWZsnovZGj8rqut0xMhOfY/s9nJpvJ7fSJe2GWg7uqdlyzxcV9D+VnisFJcyrht6" &
            "T3qv3WWX9KJrOs2DMm6q019ELqXK2fAFlWSn0+m43t3d3d/d3bXD4WChHmExfvOm4I432YMDSdCGdszNtWCMFIuwt23wFabHTDYO" &
            "7YWfLN8qKj39GauXm1R2bus2ki/ho5DLsrzxA0XX+cZ50qeqVVv64SlrTp8W8q0VeVoxL/r2oI336LpK5rCOxEwc6fnVtG+u44zT" &
            "THrIlnUlejassYXrlDYey30crh2Pz1ytmDf9XCZSXalut/N4vZobbLfzOfSaUtx1Xdvd3Z3+lMmm70BWOehCYGPXwuGJvXKHg6Sc" &
            "x89NypNkI/w8pC0hOc/byO8aGLfKsYW3qypbJ/WU/CRjPT3cZxaPz1yUU+82tEvwHJJUSy/epczG4/lN9GKl2fSYySc4n1HsNlFD" &
            "T9cm9OKaPFU/vdm4Pf16yDbci9v6ePx6IXuCtzCIvf4OfblOpGHO+M1yydBn8dpZP7fk5+uKPbZOskuxkh3RFykz9OyYm0iWdII1" &
            "j2660dq3nq5nl44po45rMqP32KmOVPMorkNfwjwum4W+p/B7KHtisufqCxjm9bVkvndk2/tOvEfKvUffkycW/BJve/Q/rafTaU3J" &
            "jsfj07G/8vToFaNYvt2KFEs1z9RN9tRJG64rejZeOzfCOFw33ABJn2TOzCzcpmdPfdrcNtGzo85nhos/ykWKw012DvtgHMqTztfJ" &
            "bgTtGSPpqwcZoR/X1PU2UsmbnaN0rkTy3yZ7c52OU73c81jrZOc63/iOQwWvZc6CsSRLsSu5s+HdA7/mz3lO67Zt99u2vVaJDMgS" &
            "3oMWXvCtGMWqarmGKl6SU8b1SJZ0M9BP69kLsQf9uHYuzeV+aavo6VpH33vgz8AbVVRysSfXHluR8jNObz0z98qG9j1dj9TDJezJ" &
            "6XhfOtZzzm3S3unJKp3nliwdJzi3Kh7Xe6G/bdvpdNrWw+Fw8JtLNxsLfBvsaXCP7dtgtp5kxxN9CX7uZujl6+naoF6/yBKV3En+" &
            "XAvJK72jGbXw3QjXflzVQz+SzgfXKbZT6b2uqsYesnW/2Tg9PXUz8WafOencOaM8I5K/z4Z7zovH3DO+r3t2TtV7K663hlqTPKE4" &
            "rFFvDZ59T+u6Pv1B3ieWHX+KuKIqrGKvvfDhVNsslW0lvzWsO2090g0247eHXqye7m3w3L3OngfC+6tN1ur6dLzH33F5elA4lHNd" &
            "QTuuWzGX56D3YCWS8R66JVW8dI4TS/EpKR2P6MVOJPuz7Lhu2/Zxs/+uUKQBqlC/6DZ85UubHvKvCuTmKAd/sHMNXnvVU5WrkrfQ" &
            "J/thbPZNf9bDWj3PDF57r8cWaqWugrUn9uq8T3/LFV8lvbElKvmtYO0+R+pEsqn6SLLKR/EkYw1uX9XmnOwjoBVJ7/VKl2rWxvf3" &
            "UwzqRZIR5VYep4rbOrr0YwDWS5mvq7iCsdS//xV1j9Xs3vAeqxx+zhRLtsvjR/FP68cff/xLr1692vQr6r2gSfZZ83mp6W3V4Scx" &
            "sYX3cF038m87euEDQTLKZ+PN1OZ4Hua8JT63tM1S1bgnRqLypzzl39NDZZfOA3ON8myTL1YJj6vrj+eI12VVC/1Yt6+pI+wn2Xos" &
            "wVpJVZvTq/Gac9SsvmVZtvXVq1e//PDwcNKrpQdnAmekHzFTqJNyJbu99IbGE8XtluyN27NLOpexD+rcZkQ6L60jF3YRUnURKc5M" &
            "/RU+hxkqu9Qj13uRP8+Rx63ko9xVH5W8B3Ol680Z6QhlXO/F/XXsNVPmtpSl8+4w3izJh3l78VQTa6MPe+J6eXz3Z1kfHl89XlPs" &
            "ueBG+r2wkfYMOVpxEVA/yx5bwjr24CeVJ5iM9O2KPqrzM8pH9tqLKn+7oCe31zHny1nSbpa99okUg+s2eBFhPyP5CMZvIZavR/Gp" &
            "Z5xkIxnlSZbwHlhrtTm+pl7H246P8Db4VZvbMp7WPD/0T3u+LSzW1trT74HIUMcc4giPU21CsSu9bNisoO0eKt+qjoqeLfsaxZ7R" &
            "VZvb+X6E+1c+zFVtyWcvvfOdGNUuZuqZsUmkGnhMm0tzOcmfuao8lKWZ02YP7JckOWsfUfXWY9Ze85iZS6qbNmKmZtnQTmv+mIH2" &
            "9E363osW5X7scznLt3Vd14MJXgtGRvoZvLBL2eubLgTRi+XDTNu1zMSYtUl2lVzoge02l/a312+vvaAf97cgxa+Y7WE2Xpuw2cLH" &
            "kUd1uN0It+nFla7aHF1rvTrSfUq7FLtaj/I57uP7WVSXP5zTD+Mrqrk5rvN7l1uy17raEtSFnNu6ruu9Psrr2+FweO0EfF6omr0E" &
            "xbplzEu4Zf6qp+c6j7zIrmHmevNct8r7XNiNRlXJHtu9D7sUuzdz2Sd9ikXcZiYPjy/hEv89PuyBa8HzzhzUO9TRjmvJ6Jdwu5Gt" &
            "oJ18T6fTtr548eLdu7u75f7+vumv8moofN+LiWXnsl5huogq/QyKwRPH+ri5ne9b5yKo5K1TxzX4XBV3T3z25HEumfmMzy1m4DWm" &
            "nH4eUy76VDXxmqg2j+H/M+csW/GXHFgn87qetolt8hd/Uz2Kr7wehzFZY6pN8vQ+uetd7jI9a9p59rSVPf1oQypbz+lx+bzTNeBw" &
            "3lynmnh+FFfyyt9r0zFzO0mXfCU7nU7teDy+9qer3F+b29rMTuu6rod1XTc2xGC34Jbxbhmrh18cPhPubwFzvW0uzbnXj/ZcO5Wu" &
            "kouR/hp6sXu6vTCWr3vXX5K9DUZ5k35W1s7yGZ2Oe7aEMq793txDqiXJHD4DKrtWxOrZO+ynl0+2mMNp3bbtQS8e2vSKs5dLh/xZ" &
            "4yeBW6I36Fmu8b0EnZu0XcOePnoz3YPHuEW8Pfi5n81NW64pT7bJXvR0iZ59T1dxiU/b6beFD/Wk2YxiSp/sqGPsFr6Cd70/ZH1P" &
            "++TrXGIzsnf8vuf9L3/GSfJt27Z127ZPtm17+g5EMHDFrJ1AAa/pZpgdUsL9Zuu+Jl9iFK+nuxTlrLZLmfW9No8zinNprtH1MBOT" &
            "NqkWriVLcuLnrOeTHnLJh8dcz7DXblRHj8ouxUx4DRVJx1q19ocwH8ij66kVtmlOM70R2iuG51LdXn8vJ9+eXJZlW7dte9jOEjrM" &
            "MjMsJxU3wyU+7Qq/FobWcMEkUj7GcBiLdtTP4jl1nDbp99Kri/HJjJzHvbVk19Drxxn11kItI3tRxdY66Rx/KPGhcCleEze32Qtj" &
            "uIwb9b7mMR/IW/jZDOOnuJW9w5+PCNpV11aSpzx76Pnu1bFvznY9nU5P/6HUhh9m7cEDp6F81qThjF4IKqoTXMlvwd46b1lHFWuv" &
            "vF04I7dP/klGRvoZZvI0yyX7Xv20dehPfZK5D3VOTydmbNqEHeuvaksy4T7PfS/QfjRP1/fsGvqgbe8ZKlt/PjMP16L6QoIvBjz2" &
            "nPRbHt+xOqz67kO40zVwAM9JNbhbwCH7cGfZY9vjuWbK+noXk2+JSt6CbrQW1/ZdxZ2h59vTJfbaO34etPd4OuZX2g1/KJW6t02q" &
            "WfiDLpEesJR5f9rrgSdYQ/IhjCF6z0mPyfOiF4Lq4eykmjyeb4wnUv9uRx/G9djL+dc87u7u7tfW2lFKfZRLTfhJ0jELZGJRyWcZ" &
            "DdEbm4HxWogpqtiaAweabJ2U+xJGuVyunLTnuaWs0hGvZbYuj5Nsq5qdqp4eVW+O52Q/nJ8f09ZxHa8V+rB3btLJ1nuhHRnpRcpZ" &
            "QTuuXabjFu6Fnr3rOGvq3EcPZod1JKjjuuH6q+pxaKP63J4xluKvjFc5iM+qhbnyWpQPryuiuGeb5e7u7n7dtu3UWttaOJk8ZjI2" &
            "xPU13DJWGgh7uhQ/CT1SDWLG39kbK8neFr3cnF1lS5vK7hbsja16en49nZixcXrXwGeJ15VqTH2O5jci+XrupG8deQs6PlirmiXj" &
            "nsda+0Y8p8dLtk5PP1vXDMuyrOuyLCcV6sXyW66zwxuD/FaAfXwW3CJ/b/aUM1/yTTL6PSd+vc3kTTa8PhMenzFmc89w61i99S1h" &
            "3dXxrMyvq945IUk2Ip1XzznqZQR9Ul+aH2uhb4J+PZ+eLqHz0DsHjNlbw35rrR3X1trxvCiTpUStuDhkm+xnSH5J9lxcmus5/Xoz" &
            "TedMdtpTX5Hi3wqPvYXfKPceUx2UVXYpbjrmOsVLNfme9pfC2C5nHsp6em6JSt4GOsdrcfxcLPaOBe3ISC9SPJ5/wZjuy/lUtjxO" &
            "a8GYFayDx4lZHeNUx0lGvWJhe1iXZXl6s9AT6oRr7Y6JpOe6R8r1rUZVc5LP9jiySXNPJBv3pe45SDmq3FVdSVbRsxvpkr6SUZ5k" &
            "pOpPuoTL6cv1DMk+yZykT3Wwv+THr+L30IsrejoyisdeenbXwNmleOlFkvW5PP3MxX2YI+n9HSn7GdPDuizL3bIsC4OIKkhF0nNN" &
            "Kn2KVVHZ8iKt7Fqnjj2MYvTyE9qN6ncqmz0xnpM9Neyxm7GdsRHJtidLuhnoz154XG2zpDzMQX1i6/yGuH6QTX//rjn5jnCb9B14" &
            "6+RM+ar+XaeHJm2Z29fUJZKN556dCdmKH5a7nv0S7zVtx+Px1bo80pbOpxwoZzJPRHvaEMrpdyueI2bFTN97YRzOeJQzzTXJ3hZe" &
            "v2rgDS4ko0+qnfNgTPowzuiY/k5Pdylen475YPBthmSXehU+f4c+Xt/oi810nvfAuCme2/A6cFgnY7fwsVknxU0yoprS9ZlyJXvZ" &
            "JvxcVHgOj886aL898rCmYtxZepf5XsjP4/Vi96DfDF6ncjAO14735VvySSeRjHpN/knWMOtkw9qpc5n3Nkuy5by5JVynGfosped8" &
            "UzzOwWO7fVUP5YrntbhtknmtrMehzuNxE/Sp2IoXlJ7NCOZ2P+bRVp1PfuXOc+2b4LlwerW4TcM1Rl/q3c7zqofF/iKt9+OwNpen" &
            "4xZyOb0Hv0j9pxrdjhttFvuzVtJrPv63EvUCcu8Ddrj2ZrVWQupuDWvZy6i+dPG0cPLT4PegWVX+zF/Jbgn7mt16zNj0qHz9Wqv+" &
            "jERFr3bKejOnbSVLyG7WvkfqhT36Om2JSu7Qxs9Lir+Ft7oSvA+9D+1nNqE1H6iO1+V9CMZ0eU/vjPQk1dEGuaoZUEfYx2bnSnUs" &
            "4XdTjsfjq7W1dt9aW+S45yuVEaMYSZ+GNiLFaWHYlZ3g0ATjzMAYThXrkjyzKHbaevT0PR251FbHyT/JLiXNozp/Dn0SI32CPlzP" &
            "wNq4HkFbrh2fn2/JJpHuF9nKL32nNbNVzzSuRSVPMC7XPWir/ikX3pPWla6iesbxBZZxZG81bq210+N/RXh2SCeIAfbCQnpUtpT3" &
            "GtW6kvegzRLeC+RcPi/coib2eE3Ma3xFisH6erXu1Sc7v6lnqeLNxqAd13sZ+Uvv9bJurknquXpeJFvKevLRRj9Cma+pa3gOOFt4" &
            "ZlI/ii2qWQn6ep/aq549MC/jEsiXtbX24APwLXHpCwlhfK6vwWOxl9Qf15XM6enahP8I+lZz9zzem7bKT7jtLWAsrmegT/Jnn2lL" &
            "9jMwzqxfK2ptHXmDjnZci1FdSc9rYSu+Oq+O07qFv7+lPP7iK52OuVHHtWSMP7M5HpskWY+qTtbbi8u+XJ5isZ826IlUMVNcZ7MX" &
            "J9kfDofDejweN10ACpKKn0lSMdPYLHtijWw5yB7snesRaaZkpL+G6oaq5C3UzI1QxvVzUNUiRvp2wdx7syKVPEG7tB7F89p6tpLT" &
            "pnecYiWZwxjUsScd8yt7+l5DrybBc8y56mFa+V/CTCzO6RL8Oe971wnm0/r8zFjX0+n06tr/D2SG3knj2un5OZXOm65s0gM0PViT" &
            "rMIHnnqoarmUPfGq+inn+m3D2bVwDrh2OHfGkrxZHJ635OP0bCr5iBST60tIPfncXK5j2pPe/GfxGIrjtfby7+EWsTwGZ5S2BHVc" &
            "t+LeYz7B2fWgHfOKGZvTaWt6C2vbio+synmxT1y5LvlUsHGxN0Y6djweB9bDTwRjXEvV+zXwIkoo72xPlHNdMdtbz061zszK+/Fj" &
            "+c1cJ4nZfjnPa0l1J3r6pfhzIS7z2Wrvfpfgc+idO89f1eewbn9/n7EY0+1873LKRrjPXl/C65W6xT71VM2Xe/k6jO/3DPVVHq1b" &
            "29qytLZtp/b4N3hPD2t7/ATW0y8TjgpwqLvmQpzJ7QOjrqrfmbFpIb7We7Ye0o/sRBXb+1FP7I1+tKW9y2mXbAVr6fl4D+ytmglt" &
            "vB/C+M3qSzFuDWMzZ9pk16PSMxbt1LfjOSsdoS1r38IXKkT2tGEc2VTxeI0lG49JuW+U6zjJKdu7taL3LfyMQfbE++XPn0bov+xg" &
            "TS301J5qVb2n115AjseHj9dt29Z1XV/rKDUwKlD6kd2lbMUvK7l+Njft3HdPz7Mwjtee8jqVXKj2Ko7PirrngufC+/Vri7NmL7dk" &
            "Ni5tuBaKl65Hn3flL0b6NrDhDBOVfMSs36wd4X1QbW7P+97x60p+rI3rJHNf6vaS6mFPLj+FP5nCXmR3i9r8mPFU27oubV2Xtiyv" &
            "/d7Vdjptn6zLsty5lKQkTCS2nW9p9WDexOiCSlSxWpFT+715RC+fmLEh6Twk2SUzakWsHrL3eaWHKqGfy2dh3hlSfJdVNSW52Ju7" &
            "iuNUtlxfyt6adcy6LqlH10jy9VknfYX3s8eP+Xo9bufnXK82xnCZjlvxAlL5cp/sZmBc1uV61ce5Ptps27osy8pvg/x49gIjLOS5" &
            "GOXgsKijrAcfignF3BO7sqvkhLl6fjO2SfYcKE9Vz0wdMzaEPqOZUF/JRrjPjO/ItpI7HqOyH13Te6hyVLC+kf/MPZjw+MxBWbJN" &
            "PhVJx/iEudK2l54P8/k+szQf+7ZtS2vtTv8j4VNA/9bIT5ROXDqBnpjN9ovKjPyVI+kcHwxtezl6uh57bBvqqo4Te+obxXoumPfS" &
            "Gi71a2FOqSbG55rQx+NKzhj0SVT6JE+yHqx3Bu8lHbvNpVTzmiE9hyqq+l3n67TRnn4i2VZ47FEv1FfP4kRVh3w9Bnv+1HdrOlzO" &
            "P+Bf1/V+ba2dlmXZ+GLhx0zUK7oqdpaeP09mJRuR7N8cWJZVzNjMsCfOHttE8k+yvTBGtU7X0xLe0uBaVPLe9bmXrXiQVMhmj0+C" &
            "flx/nrimNp9Xmzh3e3Ol85BiuA3te1R2lZy4HR/kCb9n0v0jKv8G3x4+uzdnsrR1Pbxcl2V5+nMmLIo3NvepiCR7Dt5s6HV6uopL" &
            "fK5l1McsjHFJXNpzXdGzcx1rmvUjjOMkHdezMq4Tns/tq/sjkfIsxUdy3yazeS+tk/Zaz86tFbZp9n6etE7HrYjZ0GeyYRwnPUsd" &
            "1Zx0ZNaOeP2pf8l9S/L2+IP1ZVnaO2tr7eCBZMAC/dMB2nSRqyEdk1RApaeca68v5UowfuXbqzPJkyzpqq1ZLdT1tkTVU4XiMC5z" &
            "Vfkcz8s4iZ6uhV5YT9qSnWQelzLBa1tw7bIN176jXNJ7bm5JXsVI55h+Lnd0D6dP8KT6CPsgo/pkI5lg7gqvjf5pPpU8xUhy5qMP" &
            "46beHNkznuw9XqqBc6ry0JdQznVCNvoY8Lquy7quh1W/hd7Ofx7bNzXrF166+K6hipXk1QD9JEo/gjZaM/a3Ot7XSMae001XIVvu" &
            "r6WXM1Hd1LqWK1gvbbkWPhfOkzFbyCMor/KNSB+IEZoNc11CFWPD84IzcaR3O617pN4o0wx8FoT+bss47EXwHtkDY9M/1eL0zqX3" &
            "wxyMI3o6v4/ao+22ba2tp9Pp4XQ6bSrG/+Y719fgxfUaqeTCGnhtLdmML/G6kvxbidQj506SzBnpSaqhh8evjrWmTFTyVtSjWL2Y" &
            "hLY6djltPHeqwxnpGVsyh/eD9rQjPouRLWHOXhyvaQ/0S71x7yRZK+SM7TZc76UXO7HXvoL+jMMeRzzGOp3W0+n0MOOw50boMZOL" &
            "VI1X8kuo/H3w3BI93V5uGYv43EY9zTDydX11/bCmpKNsxtZtkv0leDzKdMxcvbzJdmQ/QnNeiq9UU43UVUiX7JRvCb/YJx/ZJSq5" &
            "59R+dnN7j1Edu20LNTH2JTCmz62F/pxK7lTP7MpnFM852x7XbdseTqfT01/k7W3ufA2M12PWrkd1AzWLz2H3fCpGte7p+23gvad+" &
            "ef6ra2HU00jfBjY9nZipjWun8vc95b52W+olr/TMSb1gjYxZ+RHauT/j0NZlKb9fS3xopfi05zXYwnwoc1gT5Ty+BuaQbETVg8fj" &
            "7NJex9zo1wbPwIbcjCPgv51Oj/+l7XFZlidrb0CBeid3hlRM6xTaCl01ENo5PR15jh4vxePtjZ168HmO4qXZJ2TXs/ecbpNqTIzi" &
            "EtbDdY9eTSkGZVXeJKvsdTzLjK3Hpow1JZId1yNbkmSJnh3zp3yz/tr7NZAe4s7sM2gG1ux1cU0b2vGbAfoRxknH/hrwGPfRYl0e" &
            "/8DJQgc68gfsz0mqo10gv4bew+QSnqNGMnMx93SkZ5t0jxfW6xeu5BU9HalsJad+tCYjfevYjPqlLNn73PzYqXoV8vPNdWSkT9CO" &
            "+VJu2ZHZ/B6Pdkk3G3eWXl+X4HXyeCZPz45y6kXyJa7nM3Hbtk0vHq9F2c4f1/IHkgfiWtC+siOz9mygFcNJsZjD9SmGXsWZk76S" &
            "Ef/qhOsUo7KnPPmK6pMiDuMR1uT5mJdrknQpVs+Ox4Lzma1LOv+kED/1k2Qt1O7zkd5juq30Hsth7LTRlzFa5/y6TxVzRjbSMb70" &
            "Oj4ej2/MRjacp2SM7ce+9eKO/CnTWrAOv9fS3m10rI/AKp7nElVOQR+tlcOvW8/Ba/J0OrWHh4d2PB5fs5cP7ZVSNqfTsR2Px01/" &
            "yuTp/wPRNvoug80pKQcygw/hWlIMynxNHblGTx3XnxXX1sHzdW08Z+91sMe2R+pl1GOS8b64Fp8181W5ejX3YDw+NH1PuWCMHvTl" &
            "ehbOhrXOxHUb9dDrpacTyYY19WbMuit5xchez/otvGC8SZK1p+9AngJdg5LPxtlj26zh6sRUsbwuH6rbV8cO5Vw7PV2CtSV68krX" &
            "4KevUFIuxuBasq3z2XWS9DN+FXv8lGfGx+1orzWvO9pVslbM+xLoX9XkdvQhrmc8v9907ez9QjHVRKhzH+pE0tHPN9pU6xno42vO" &
            "cJTfZay/0s+SYhDl2sJ3cbR7na2t67rer+u6Vg/mWdj4zPacpDxJ5ox011D5M6eOvVbaUF/hej+effhfC+OPclJX2c/2zvXoGveY" &
            "9BfyH+UXXuuMfaLnT1ll14KO6x6yo/1opgnmZcyUawu/ie06l/HY7UiSJZLdduW7LlwrFmN63zN4/1zviaH94/ap3LZ1XZbloIvA" &
            "L4TZRJfAXLN48RU9fZWTfbs/1zOM7JN+Ns8eu4Tk1I/WQrNK14xINXKmPZL/LJXfsvM77Nm+CK+ltJ+F+bT2jfakkjFuJU+yhPc9" &
            "mgE3J9n1uNQ+wRjcC14b7lfV4PcL58ON0GcWj8W+mM/v68S2xbe41nVZlk9/imJUgSpGBST2DoaNb/hbRGn4hCeSJzMd7yH5zcou" &
            "ZbZ32fh+1tfh3HRcxZqZayWfgX0ltvCBCCG/dE0kmduTqgavkfESnFmKK3nSJ1nC60pykeqd6UMsEy/gM3rOZYTXyFor/95cCW1H" &
            "9qxl5LdnxgnOy3P58YJP2up8bdvWTidtn35ntCzLsq6HZT2dTsv5Pwd5KvbSoulfbaSncziAzxuX1lWd5LSRWZno6WYZnc9b1fo2" &
            "6OWveutRzeSW9OZb6WahP2Opt+fucQ+9evfgvY9iev/Uc4bVvZJyNVxD3PN4FuZgfdoz9mMvr/9cRHb6Y4p3bFiOl+BDSXErZmz9" &
            "1VHo0wN183NbKx6MjFnZSe4whzbZqXYnxXUYSyzFd2GS8X3VFi4YXzOPb6qb8ktQ3W3yGnDcfsNX9t5XqltUc+ZcRK9nxnIbP6/u" &
            "xxiO7HnNi9RX2kSvD87F4XXKOCk/beXvOtaQoE3KR7bw85LKliQ7zj7FYx5uPTuH190Wzk3qnfF4rLiKmeIlJF+WT2tTvGV5/Kzu" &
            "4TFufYGyuM8rn0V9z5Vzb9y99onnOMejeCO9M2NLG1/rmDYJv+bd3m/EdM/cihSbMq/lOblF/D0xeJ7SeUikc83jhMfeU6eorgXW" &
            "MKqjR/JlTq4vIeX5tPbXdMu2bYv+QylXdJvt6T5LUk3s61IYJ+Vq4YK5hN58K3mF27OHHlvn5wUJ3kDJN9WeZAnOJK1JkjWTK0Zl" &
            "59A29ddCTvertuTXo8otuc6Fb8yX4PljHsYZxWvhPI1IM6n2zmyeXt0uY++t8OWc5cc+3Cfl7kF7rdP5dnkF47nMe8x2r/lvrbW2" &
            "Lsty18KfMskB3pTdGjbRa2iG0UBnSLmvqWmvX8rFuXCdmJmF+4/s/cLVuuGCJKyTeyf101tTV0Gf5FfVmaBdFZPM2s0yOl8z8Bzu" &
            "JfXvaz7wqE8y7nt4/iou97TZC/sRHtfz8C2jNJMW+uVaVPI9bMVbvE6a6dpau08GaoZNPRdvK88lzNTGofug08UxA0+Wy0drl/lx" &
            "rw7Gabi4ucmHG6lkkrPWKk4L+WYZ2fbici3Yg9tRztiU9XL0bure+azwGrTvHVe5G2bjfnoYsXaPOYrditmn+ynVnXS+9p/rEMa/" &
            "FJ+DNpH6SHAGe+bXw3PvjbUsS1vXZVtba6eGk6/Avp9t9t9RU82PJ272ZI4uJJd77nQu/fzPQluPwXipRsqod2g7Q2XPWYxI17/q" &
            "SXWxd5J8ErTjWnhtM/1UsKcqn0g6+ngcvpgk6D/qh/FS7oTXVMHrhLV4bxv+FJT0lW2vtoRsfc/4vp8l9eSon21rTz9IP/ssy/kv" &
            "8R49uV+IDC75tbBIcYvYt+aWNaUTTir5DCPf2V5GcRJ7L2Dace30dAna67pm/76mTyvugcebqX4AJNkleI4qFxndu4lR3EpfyR3Z" &
            "sH6uXS5S/UnmpJhE52/mxayFnFw39KPZj66tEbM+nnvGx+sc4TbL0t74g4rLshzW0+l03LZt0yuxF+MFab1N/ICVjbgv/anbA309" &
            "BteX4v6MOVr3ZMRtRvPxi5QXLG0ZJ8lcrljMm2wTXo9sfT8TY08+Z7MHA/359oFkbkO/9BZMKx4ShD6sp4I2nOeGF0PWyTwei3ZO" &
            "1Yv+qCrthefhfEnK63L1yJn7DLxOrmdQjfyVAMZhfn6U2vvm+XA/3+uYtt6zZG7jm2SsxfNWPlz7PaG120n3WPPaluXQWnv8bmTb" &
            "lnY6tcO6bdsrT65gaX8tPOGXxHWfGf8ZmxEpBmVcXwov5Ipr8438e3XoAvP1NVzr/zZI90I1I/YzWidow3Xr5L+EFH+WzR42nNOe" &
            "uLymfEtQXs1jFIckuyRrnZwVvTqSvGfvjHrs6YQ/n/ms1lr/fdQ53rJu23bcQmSJfD8qsoLF3IrniFmxt+cen56MffXLnrX0zknK" &
            "U9nupZdXXKsXvP6qLVHJ91LlSTdaw3wqX6encxSHX6n3SHZJ1ibOq/fEvft6jJljx2OluPTTzC+91pNdytmzc9tZki3jpNhJdyv4" &
            "zEjxt21bTqfTsm7hh+Zm9MZw9sKY7Yqm3edS/739zNrNwnjVfEh68djTh5i1n7VLzM6Y+uSTZDOMfFyfzgGp6vB7Zyn+3hNlo/lQ" &
            "zrVIdbutjit/J9XksqTvwTgkyVonZ9ouIc3MSXGZN9kQ2VT5PAaPuSU7tyX0YS3Jx0mvB/72lt76Op1ObV3X9bA8YiEyVcE9aM/1" &
            "tczU3Yq8SeaM9Hvw2fEEO1y3HT0mqryObHxrnbxuMyNP0I7rSlZR2bqc/TlVr60Tm2zh54PMT1gTbdKatmlPP9LT7/Vnzy4bxXHc" &
            "VjVwa4jtPacYPdinx5jdSJJVVDHaRBz3TTVV/vRxGWFc384vKNv5vwJ58wK4BSy2KlSM9BV76k91cDi0SX0k/bLjrSmPwXocxvL4" &
            "KZ8fK8eetzp6VH1TnujpGbOKV8mSfETy4axbsOO8RTWL6twk0hxm1xXJhjLF8phcJ5JN8k92TqVLvrz2aZ9gDMmoJ0km5MMfPNMm" &
            "wdx+rJiMm+zcv7JL0IbXaJVXbI81bg8PD8t6/m30J0fCk8R1Ra+xxEh/Czi4PST7mb7awJdzmoXnYbH/htjP5Z6Ye3iO+L1YSZdk" &
            "rSMX1HOWgj2O7PyYOchI71S21T2bmLVrwXZmrS2tZ7nU7zlgL0k3orpeWrhmPJ9T2fWYsUl4vakmf504nU7L8Xhc9bewFj2Aqo+r" &
            "KYDvK5SIW4+ejZrxrQf1HIDnoq1knoezSVvyE95X0vdg31UO2fq+debKWLPxZkjxUg1EtbIujzWSzWz040coSao92W/h54m0kY59" &
            "Sk5Svb7m+aWNy2TrPoxH3yRnPslYC3v0Tb6+9v+PwmPquOG9+Es2fjdOvXTsJflKXm3VdxKM0aw/5kxz9fger4rdEFfHbus5qhit" &
            "tXY8Hp9s7+/v79f0CawEG6RbavQWMA/pDa2SX8ql8fwEf6vRq/lS3YjetdSL69dCui5SzGRXIduT/b5D79wmmeO1uG2KOYo1orpv" &
            "E1Xe5Mt5Olv4mZBIsRw/V4yh85DmREb1Ox43bUSy9CDXnn5ct+I6oB1rSfq0dnmqi8fE8/kL6Lquy+FwuFuXZdkYxNd+M/NEvk2q" &
            "5ip6OmfW7m3h83+u2p4r7iWMaknXHa9X37t+NEP69HRVvCSbgT21kFN7xk8yJ8VuRXyS8lbHgg/AWT3tpNvCd3LU+yZouwfG5FZB" &
            "ndYup42obJi72px0zr0WHqetx+l0asfj8emLp/N3infrtm0LgyfSjezsKYa2lZ/XRTlle/EYM/FG+m8VPg99sIbq/Dvp2kv2Hotx" &
            "U4wWrjOPy2N/W0IybmKUL+ExenYzXOvfMBP2R/Y8wNOckg9nO1NHD/fz3hKVvOE8+TptlT2Pkx/lvPYSzNvCW39uR3nCdXotWNd1" &
            "Wdvj33V/eieruug/D4yaFCObSr9Xfkv29DayS/oZv7dNqqeSJbmQjvtLcN/q2GWj2ip6PnwQM4fLUhyXV/oejM9YVd6KyjbJmUOb" &
            "HoB8gHoM1llBv4rqPLg/41BPG+oor7Zkq+MK6fRdA/24uQ/j+s+AT6dTe3h4aMfj8eHxp1UT+Im7hj0xlolPE7H5yq6CPvTnei97" &
            "/avv9EZ1ikr+WcPzyH2CPUs2Wqf5tWArKnvhfpWtaq1yJJK9fzXOGdFWpNxp7XvCvO5fPUhd5vs2+K6iIsV0Kr3XQ7mTzh1rru6/" &
            "BHNVm+tJJSesM/nxHOo4yejb8LfShM/CP2C1bdt2PB5fPb2A8ISzGN9moM8eX1L5ubyyIbSbvViuhXmdpBvdtN8KXFpv6tVl1Tx8" &
            "nR4GtHc9dVwL+Sh/un42+0F70rfQT0XVa4Xbq46ZPIS2qY9UF2feOnZO6pM+zfwq/0tJebmnXsduxzXtnWTrx65n326fZt5CnKoO" &
            "Zybncn77almWtvaCVkk3+8uZlFOWSDbu6w1rOGlAPTyGbyNon3ySnPVdaiMkd33lz1o5syoHYSz383oo5+b0ZJQLxhvZt7OP981r" &
            "RnrFSDEpo14yP0765J9svc5kxz42fHSTG/0po61y+LF/VJ2wDr1IVlQxJFffqsfjpxrSnNhPklVwDskv5fP1HljzTEzaafPZMI5D" &
            "OeMkX62ra62d53V/f3+/ttYO7uh7hydTpMTJn1Q2KV7r5G+dWLP0YrdQU6KSC79penjPHnPGjyTZNczGq86Z95YY6a/lktiqiVsr" &
            "+htR+UqXYviDNekFdczh/uyl4RqdeUAt4XdZeCw7xqyYsekx689euO4xa1vNkLPkWjLXcZ1iJ3u3aeHcJjzGFn7fpJ3jrOt6t67r" &
            "+s6q39yxH7i4IdeUtR1DnYXxuH7bcIAjeTWna2HMlL866ZegGJfEkQ99ua5koqfrQb+ZXpIuyciMjWANyVfn2c/3zDWV9MxHZuey" &
            "hV+mS/kc1VzZJfkS3t1werVWcoez19rnwG1EZVP5J3mSVVQ1J5uEnxedA48zen6cfdb1cDi8w7+HJUeeXK5vhRfpRbOB1Mys7Dm4" &
            "Jk/Vl3qeiZ1sZn1vRao96ZN8T697STFH1y97oY6yntxJ+tR7suvBGaZ4KWayT7LKPtk4mvNo3u2CnvnAq3QJ2qf1TH8k2aYYqbaU" &
            "T8fci719OvL1bYYNLyrntzC39fzf2j4ZcWOQzwIOt6otyXvM2PvFOspBm54dj3v2zozNrWFP7I+1JxvKSaXjegb6eFy/YSTnV1vV" &
            "3n1SrSMqvyTrQfsUl/G4FuxHdpX9iOd88SDMUcXjbEilT7Iee2fn9px/Kz75yp5H0D/BmNULi9d7PB7b8Xg8rafT6UEGyZEFsEmR" &
            "/KqNNiMY+xbM5BXJdm8fVf+3RPGeY14k9SM5SXaipyOVrWRJ7zNJLx7u63u3S/NMuXrQlvmTTsdJntaUuY49eH7WwpiaXTVDhzEY" &
            "v7f1YP1cC8ap1twnUo6efQt6rh3ve2YGDm05R+p57ngeZ5CP8pxOp7aeTqdXDw8P22affNAnMfY0N9LPwibTSRSX5tzrp1mkOXAt" &
            "3N5tKvtZGFMz4pyS7FJ4sfErIx0nWQXnkhjpR7CeUc6ezvFYI5+R3mE8HSd5WnMjSZfWPXi9bcWndaRzkk2SV/4iXduMuZfKl3kE" &
            "7bl2XOdz82O31Zr3HTfNnW8t9ajq9Lx+rHWz3wOxH5kv68PDw8PpdHrtBSQNzQt3lvPD1ffXMhtj1u4a1G/K1TtZtOfsOMcZZn2q" &
            "c3gNo3ipt9l6W7i+6FvpaEc8Lt+q8jl5bLdxKh3XPZK/5H7s9ThcO1XsFs7fhodNz7fhhcNn5ps/0PhQS/fKKGfDeSHVuRP0YSzq" &
            "K5lgHuZzXLf3uIJ5fdOM06zdztc6boMXKz/nfg2s63rQS8kiRwZKJ6nhhYNrTziLN9U6DUnH+EmW8Bx7STk4bPYh2M8svZjtrOdF" &
            "U9mzRrehPOmFXxMpT8P1kuK5HfXMm3SMx2P69DbBcytZ1a/kya/Kn6BdOh7hOVIuX/OYtoTXLmtbiu/QqzXlhHrajmJIznOn/RZe" &
            "5KoeqXNm6nBdWlewBtpqnXSUpzhJlvrUDO27jie7dV0P6/mf1sJAPCCDp6JbiNFjxtZ11Y0skmwPe/y3iQuLsuegF9d1lV2q9Rbw" &
            "oaIcVT6/2Rvs3D7NPMXbi+Km+KLSzeSf8a3ut+RL/WJvPSd4HgTn3qPy1x5/6uIN24pZuz0ov79IcJOd7zkLzpmwdtpzvumYdWmr" &
            "8qWa/TzQ3veVLtmxHq/rfL7Xp/8TnUFFavhavOCKGb03M7IfscffbX0mqa8kmyX5MZ8fc6uo9MnX19z7hZtgDq4r9tjN2o5QL71+" &
            "2qQd51eRdKP7bZRbVNdJxUzMdH1IXjETt4XrV7K2I0YLcVI8wp6Ub5TX4/Zi+DljXVVNoqoh9TS6NlJe908yr9vtztu6ttbuXCG8" &
            "kBTE5XtgDOfW8WbwYe4lnQgb7sVxK3oxL51dFY9yriuqeVDP44rKhnKub83MzUmSTPR0KQ9llT/r5Lx9k63smUOkc0hZD9bkMEYV" &
            "2/0Zj8fs0+MxbmKPLaEP8++BfTqVvHXOOdfV5jEow7as67q+6BXqyEYB9+CFpFwuYxOJZJ9Iufbi8VMul90iXwt5RmvJktxJ+spP" &
            "8pn+uTW7AXiDy2eG0TxZX49RrDaoi71cyqjmNC8/li/nXSEbPx9L8XOLRM+OtYg9s+rFdxiPa9KLl3xTDV6b67fiLaYWfFy+B/n7" &
            "LDnXUQ1Jxo0/D0oku23b2tpau9f/ia6LSg694ryha5mJ5UUL2VdNXwvzkUqvOSbdtaS4vKioT8zYzKA4qa5bof54fVySjzEaenBZ" &
            "6inVIZK9Q79km+6FdFzVd0s4DzF7rbFfrhNvoy+R6vH8rEPHW+f5Jxs+aN3XY3ocboTXAW0YO8lZEzfiuvQisy7LcsfC3CANyo+9" &
            "sArGS81r3YtDRrbMMUNvmGTWZjae6NlWOsq5dvyc0S7JBOUeR/uqXz/no/OSro8ezLUH9sBj0tNdgse75B5oHftKvodeDOqq815B" &
            "24p0jhKj6ys9w1yW4m/24ByR6tRM0mxEVa9wv2vujc0+Mad1qqnqw4/PcZb1cDis67ou/gkKL7IanhfDvfSpQJd5Htoyjmzd3qF/" &
            "kqWaPF56hRXUq6YKr5c9Uk5cznoYi3U5bjMjd9wm2fpaNaXa3F5rxqpI8/E4zOv6VB/l0jEPZfLTnKt7QjAPa/Lz75tsfS9YI0k5" &
            "fT+L18KaqOOc3F5vd7h8BGekOL53G1/Tl7CndM8wFuPymH4O8yU4T8G8msUWrj2fue91zPPk9gs+nttsLr7XseporbXD4dCWZVn1" &
            "X9q2hoYIm6rwplm826R4jF3paLeHqh7pEtfkazfwvzXX9lnZ+fnkub0Fo3gj/R54vXlfSV7BeXCjrvLVwzjZEfdzeN65HsGaK3jf" &
            "92yJfPigZAzXU6f13v6E167jqhbmbsi7t4YUL+E1au3yBf+3S9UH83kc7ase1nVd19PpdNweaW1iYHtQgfR3ebKhvaCdqBp0kl8P" &
            "1rXHf4/tLL0T6XCm7IOk/no+lCU7rp2ejrAmkmQt1O/7aqOvr3mcbKpY3NOG0MYflHyoJpgz6ZzedZXsR/UT9s7jVjy0BGeRNtkw" &
            "bgX7dT/qCHtIfVTrxEy9wnNW/XNNueJUdg7rp37btrY+PDwcj8fja8H92I1TEqeKwY30dLP4C1/aEknHoVVUdox3CYxR5dqL4vbi" &
            "MXcPt93jdy08b75mHVxXjGKm+L3YtK9sFZv5nZ6uBX3P9lJY52wO2ib/KlaSs4bZeG7j+2SbmPH34+oeq/L1YiUf2WtLLxB8kfHj" &
            "vXhsr3U9nU6n85/mjcHpUOEJtE8+SSb0AvCtitc+OzdC++pCvCU8dxWX6DmTS+hdU1xX7LGrbFlHZeuydP6Sj/CYtEs6t6nqqah6" &
            "qNaUE9ql2pLM1+nY8TgzG5HM925brZM/Yzm0oz3XPd9kQ/QFdOv8fGeWdM0m2el0ams7/x0sUjnsKSSR4nrzz8Go5ko/e/I+r/iF" &
            "mnqo5Ime3aUX6h5m4t66jjS/FDfJiF/f1RdKSZbo1SK8ZsqSrmIrPhxDG9aU7N2Peq5F9WxIMsaijHUwp/u5njC+9oyTjpmbVPIW" &
            "6iYpV2VLqjk767q+9kP3bduafpHw6a+ZVEFUSKWnXAVRTkZ6Z+9QHPqMTobo6ffU/rbp1T2Cc74mVrN4tybFnJUl3G7Wh7jf7D1V" &
            "HdOfcbhOXNqHs/c6GNlXeq4vwWMznmSu69m4zPcjOWEs95uNQaqfYVRbuyCH489zv+7WZVnu9criP7WXk5LyJ/rt3IRs1nV92jup" &
            "aI+TGp2Btl6zYEzmqnRiFIsnMcGZ0dZzJFgv/Ss8r+dONtSxbz9Om9vRN22yY+8pHun5+flwG86+it1CHylWb92Keka2zlLcF8m/" &
            "B/1TXSm/w3z0843vUNCXa6eqgTl6MUSKJZniVDJfJ1tH54M1MZ7LXZ9sCG2TD+vt1ZP8WyFXb3z2e/zyLay9pAKSrIWhJKjjmoz0" &
            "zh7ba6lO3HPMoGLkN9LvhRcwmcnHGLxw91LdAIQ6rSlPsK+Uj+fcr4PqWhH0raDdZh8DTtDe2cIDSfKq5qSjzEmyCrdlXVWeS+OP" &
            "SOc19Ux6dVOeYI4Eczj08XjUV9ewvlh4/duFAgboUQ0j4bY9n0reIw1jL8k3DZd69sKvRNlz8nG2wQOgohezx4wPbTgHrq+BsZg7" &
            "wXm3wfmS3uHaoW5PTb4Rl3n9p/AJGvqnmL1cxO1YZ3qQJJhH/qrfe2C+xEztik+Zb5xdoqf3vnnsfszrm6MYrqvsXX8JjO/HjMm1" &
            "6mS923mmw+9AOKzRBZTgYFLhTqXzi1gnrtoSPZ1IMejDtQ/X8Tisr9pIr44ee2xnmInHa+PS66V18rmcF3dC9ikeZSm2QxnP+8w5" &
            "JPLp+bq8suWa90qq1WEd1Cdo6/v0YuFfSPmx21DHXA5lXKca/Nj3z4ny+iZUn7/AcmPdqeY9/dAm1eU6weu/tbat57exulQPAl6U" &
            "ZKYoQh3XI5J9VYfoDXAG+aUZtfFJiFxaS2JPrGRLGddOda3sgfG5dvZcg71z7Da+H9GzZ+497LHv2V5yLjgvzi3FlI0/sF3XW0vG" &
            "PK7zY9omn1vCuqrjPVxSP31m56Dzlc5b68gTur/P2/Laf2nbwx0r2NzbJOXk+hKqGDx5ojenJHMW+66qdXLv4doYl/p7r6O+ic/2" &
            "0vz0ZcwUlzKuR7i9eq5yCdyQr+nkN2Mj+BXrLJxL8md/qR7BeA3XgeurXFU9lU/D3PdQ2VPu9fR0vueMer6U+TZLsvdzxXqo7yG7" &
            "pw9ebdt2bPb3sK6FhSeqQimfifWcXJuf/fRINxf1s/F4AV/Ktf63pJrJHugveJNWdjO4bxVnb90OrxPH+2BPFT29z6Nn1yb67j28" &
            "nJlcomc7yjMDe/J5uIw6ylpRz0hPql5boWMNzkw+h7Wet219eHj4WL+F7gZ+vG1b0587kZ3ktO/BhlQI9bSbwWPt9U81pG/FSZWH" &
            "MVhbmtds3z0b9lEdVzU48vG6mJvyZCM70quDsegvmZ8jfgS9gr6+yV8xdMw6GIOxTqdTOx6Pb+SUP/G8zN/CPBKSV9eAqPSey/vR" &
            "fd/bGM9zMF81s4qkZ0zauJ42blv5sL4KxkrPjCqnZJwD7T2O+/l1Ivnp/DMTh/E4G7fxY8XzuNt2bI/fb5zasmxtXR//lMnD6XTa" &
            "GITBWMg1ME9q6v8f4MOgurGcSk7S+eidG57Dnm2brKNnw3NcQT3XZKQXs7UxHufZ06e1Q3+tXVbVueda6dVA9ti2kNt7UKwUc6Y2" &
            "zqaFfI7nY1zeZ9oS9B/10kLfowe/73u6PVR+ac18G17g6CMZZ+ZxwhtV23o6nbb0apsSVBcF/ahP0KZ3whP0r2QzXOrXwgXfi8UT" &
            "RN/ehd+bq+toRx/pfXNS7gr6XsMoVqpVVLrUS7JrsGW8NKsUh/ncj/bVea7gdTPrm2pmXak+wbmQkb+o9Em+4GeBrbBLsh4pJte+" &
            "d7m20QsH/V3HOMkmwXja+7Hz6XcMOTfthc71m9eWnvsQn38TfZ29IKvEbaDrwSZnoF1vKM9Jb2Y9XQ/1MbpxE7QbrRMjG806bT0q" &
            "W19rn2aXZIwlkm2z+KwlxWid+EkmqtykF8PZ88JRxUwzTv2zXz3I91I/iD6lpyNVDR6DD3bCHt2GM3Bo63LqtU/nKcVoHbnDWKKq" &
            "rYK29NfaX9yWZWnrurSihLYeDof78/8u1Vox4Iah9Bq6lEt96cf1paSLYA+pDsajDdcz8Hxx7zC/wxj0TzLitTAeoZ42af5L5+cR" &
            "yd/3W/j2vAf7YB6uieTqg5tsuJEqH+2T7x56/qydG/Eaidu7v8t7fWmd8vIrb/om3Cb9/MD3TsrPeSQ/9tarlfHEFn5G4/7p3Ggt" &
            "n/QdkMs3/EyQP2PUrNdlWQ7NPsZbNeNIXzU3gs21ybxtEL+next4fj8p1UY7x9eclah8KJfM5akWyZO909M16Ht2raiZtW3hB4Sp" &
            "Bq6ruVVywTgVtNMN6rCPpEsyl9NGDw/pqO/R6z3l9odOz6YXd4T79vru6cQWrpVL8H5HpN59TpwZ5+c+XHPjC2QVq6EHtxnJE48v" &
            "Iq+fp9PptK1qnsH8wjkbdxNcgzfUo6fv6Wa5RYzWicMZ087nTXiTMk7ycTy2bHvHl1L5Mjd1XI+2Eemmbiav9KI3iyQTPEesmWvK" &
            "kjzZ0HYEbdk/9QnaqAbGki4dO+mavqQ3x31THNbKteyrutyGe7etHvQpjpNiMebMi2MvRwXPR3uj99dfK/QdyPFR9mljevEg6ZVd" &
            "SbXpW52EbLw5NsomvIG0VVQ6+s/GSjbskzPwTb7uQ3/R6z/Z7IWxdMxcKW8L5zzVQh174vEWvnJ03Qj24F/wpPpaURPrYVzfnNQn" &
            "Z8Daqliag8sZt6FGxiDuk2SSs17Riy+d9ydSr9ycdK5oP9Oz62jn54XnSPZVfb5Of8F8tj6R7Pz8a/OZckbuzz6015bqlB/nILn3" &
            "qb+63lprDw8PbX36z9DPxqkhMtKPUJFerDfmjV9D8ueA3gapjllYr18Ms7jtHr/nZrMLmhc+rwHv+5L+/QbxG8WPmdPhtep+beA7" &
            "wyV+l/g4afYVe214jnrzcdukd5I+yVqogUg/skvs9eM1R3rxOBvW3Zs7oW4Ue3v6wi7GXdbtESp2Myq8gjemF95CQzO43axPIuW8" &
            "VeyE98mLLNUieY+RPsHcI1KOql7huipf8ve43M/Qs1XsyoYvGiTV04tHaNvL1Sb0TlXDqCensq1iU57W3jP3pJI3+CY7z5VyVqjf" &
            "PT49OD+uhc/abao+2RtrpI6+jvf86XdCr39XfLZZ1tbap78yazBo6zR7LR6XOTTIdCJnucR+r0/DSfJ1RdJVs/DYHt/1lBHWxc3h" &
            "eUgkv1kqP5f3akj9pphu528L8C2Cds5HWYJ1VTWkGElWMWPLWhKjOFvxBYvrqJfOY6f7lDauSzLKW+ixsvM+vP5kS3o2vA8vhX04" &
            "vf4pc1v3SbLk5/Dc6lj3QoOf7JfHt7PWdVmWrdmvGPIEECYkbCJtCTbBjTBOsnGq/KP1LPTjegTtvVbqKKvsuK5ke0lzlJzrbfBw" &
            "GsHrQLEYU6S5MQ/XDq+3Kg9R3ip2T0eSnct07LNYirefkx/3Izwu94nROXIUuxevIvlV/aY8qZdkdyk6J4L1uHwk83XPdkZOaOPn" &
            "jb7b628HH55+iZA/DOLFKdJF0Ssukewloy41NQsb134LP7DdA2t0OPDEjI1gjfSdjTNrN5rzbJwEL0ztRz903PCzEl6jiSpWsxi+" &
            "VfoWYnntLmMPSUbcxm2TLF2zrL2F+hTD34KgzqGNjmfOk+AHSVrnmZJINn4+WJvjeh37pjlS7j60YQ978Hg8f8yvnG7PWpMvbShn" &
            "TuF+nou2j/vXr4tleXwL66rhXAobrXS+JklG3NeH5fp0fAkz/jM2wvsn7IP0dA3+tF2KT+ElFCfZJ5mzFQ8lj8kaF/tUiNaur2BM" &
            "boIPPfd1Um2+p32ismFt23lOe6hi84HUOrZipp8Uy2dYPWOqF4UZRvbUa47V5vNOMbyHdJ3MwLmzxmbPKdbkftqr9kSK7bE8j88g" &
            "2fpax60ty7pt2xvZNRgGaCGo7C/F422D91sTXgsvUsbiiWefiVSL0/MlsmPP9K/khDGSXyV3vfcouzQ738tWMt/ow2PBelIcrr0X" &
            "2jmy4U1YQftW9Ex6sVM/DdeM+3ObtZXO8Xn5Ps1ypj8/Zm63JbOxOSPB+fF8ploqudPT87z1bCVjTm087z3oKzgD1eQkP0JdVRvj" &
            "fBp7advWzn8Ta22trct6Op2ObDShgNqP7GdgQ5Kl4kfQ7hb1taLGFvKNmLXlnK9hNsZML5V+NOeevqdLc2CdWvfiOCO7qsdEqs8Z" &
            "ydnLHtIMEqN+E1Us5Umb9ESyS+rgQ7OFHFv4AqHKSd9ZvD/G6PVewTp7sIdKlmB8X3uMKh5r3M4vHI+iT33Wbdtesak3nR+P9a1S" &
            "lXQE446grdfJjSTZc3JJPr9JqhulilvJxUif4EzTbLkm0jNGFc9xX+21+bfpjDFat3DNsib3YR1kK160UizXpeMePbuUhzlGfbQw" &
            "B5fTJtnTjz7UJ9sE58sYjOOyypc+JN2LhLG4n4Fz6fmqFtbVq7UXT9BXx17bFn4u4vJt25Z1WZYHCWTAgL5PMCltK1lPRz1tKvbY" &
            "7mFvHY770N9PIuPz2GdyLbyAhPd5i3yMNWLDw9lvIIdxuZZsRIqhY99Tn2CNJMV0RvFH9PIzbi/XqE6nikM51wnZbMXPxsSoz8pP" &
            "eH89e7frybmnnY7TlmxHVP0zpssFffkcYH3cyCohAzvJkST/KqlkrkuyPVS5LsXjjeKyD/dLx06VgzG1p11ixkb4FwrJr5IluaBO" &
            "9r7NUL14tBCTJJmQzm0Yr4rr6Obzm7C6IX29hI93jnI1zCPNJJHsUu6qZ9Ur2LN6cd8qPmUkzU0bc/ZmwNisNdlIxnpn7SoYi8cj" &
            "/8QeH7dN+ZKeNq4T53ku67qud/pECxt0Y99XCXprh8W4nBeFr0cXzq3gUIkPurJNfiTFqWLuiXcJo7nujT3qy0m6VIseAIxZrR23" &
            "8T2PueYxbR3WnGzdZhQvwRxO0vXic25Jp+Pq+qBMsTwmY1XQzj8dxE8IVaScTpq/NuZJMSpZT049j6t81CW/FL+yr3BddT51LGS3" &
            "rut6z98BSd8+ps9174UxK5iD6yTrDXMPPd/n0lV1a035JXB+FSO7Xi1Jx/5Sn8L1/OGo9LT1dQ/aO0vnUy2+HuH1akv30ogZ2xQ/" &
            "+VVy4TrG6fk5ml/KxfizeA1pc5vRjOlLO8l6s0wyyROS+76K4SS9y7xWbYQxuJaMcbUfPQOMZb27u7s/HA6LXwTNAi7hlwxdPwOL" &
            "HTFr7zaqj3WKKh7l9FUtPpe0uW/SJbkuWF0EzF31MovPw2vkefQaCXvyY27SCcatzg/X7kddC3ErWbNaZ+KkfmhHmw1fuSoP94Rz" &
            "SOekqs+hn6N1ZcMaEm7DutQ71+mhJhjDZZ5Lx1WN8vFc3q/XXMH4Xr/Xydqo4+YwZg/27LNIOdJcRjAOa65iykZ79bMeHl89npQM" &
            "WiWoEiX22HquBOsbwV6SjvJLGPXIHFVNKU6SzdDrzy9K2brOSf6XMBvHbyDvgXX5das9N9pTpz1ji0rusF6XE9YkWE9l51TxU+8j" &
            "kr3iV3mIyzb7WY/Xw1i9OPQllbxC56gNYic5ZVwT9ilGfiTF8T6SnnjPvk/MxlMM/TXeja+6iVQ0bbX2Jn09U2BFyjXaCHXX1EPf" &
            "lK915A7rcvklVHPQcS9usnf8PHr8ZNtCvJF9YsNXuy5PKA+3hHQ8n9JVx5VPC31Wa8ormFt7bgnGp0/l19BHgjGqYyc90Ko4PbnH" &
            "UNx0XY6o7ChnzOrcO+lZuQ3e2pQ8fecyk5NU83KSnraK4/L1dDodVWT17VtyVEA/pr5d2LDD/FUtzl47yq6B/mlNmeO6nl0P+VVz" &
            "4Lonr2IQxk/HTmWT7FN+rpMd1yMqO8agHa9x5nd5T1fJezoim97m8AE32hytk77n18KLiI5HD9beWlTyFr6wFZUPe6hqcxi/mrG/" &
            "/VbZeD7ZMP6tYI/cAst6PB5PfKWrHCp5e6amqlwOa+Ix9a7jOslmGeXlOiH9jO2I5J/q6jFrN4vnZi2pLtclH7dLW7LRcfKbobIb" &
            "Xf+VXyXvwV64VXYje7LHJsmYs4L6ypYP2URPnnRJlqA/104lT/iMtOdG20thXManDdfUtfN3ICtfCWduBiZ2qmQz7PUb1SpSTTN5" &
            "NI+0ST/CczDfJf6jrQ3i0nYPzMUYXAuX8zj5SJZ0ovIlvXqlJ2l+yU64LvleC+dR9dLC+a32bk88xkg/y0zMHq6feTFpYU5cV+eq" &
            "F1Own55PypNkoldzK57TyU5UcuIxfF/FXtv5D5ssy9IOh0Nb8NdOnSoQ128bHySHWjGqeaSfgbMaxazmeym6yJbww0y36eExKhQz" &
            "9arcJNUiUrzWeWj0Yo3wueyJ4fbJj/Oq1pQ7jM+926WNPpWcJNkMVU9cN+RI11jqRXKR7Ku1yxMzNbq8mh/XidleK2Zs9pB6SbKw" &
            "fvxrvC70Fw42WVENvxpIko3YOj+wnOGSnDP50tB57GvOhHYizc9l6edVl1D5VXJBPetMxz2Z4/34njIek2vmIkb+/t37yPYSGJPr" &
            "GSqfJOeMk43T0ydd75y4jsf8YXJFz6bK3bvP5VPV1fAn8mnj6NchRrhvyk+7Kt9e+dL5ZCaPT6dTW5alrQ8PD6+kZHN8paQ86Vkc" &
            "X4RGTQvq1dyIGZtmdbF+UclVF/NQXs3GjxmjgrY81tpr4Eb7Szb3Z2+u8zX1vNGop8zllb/rZcMX1xS34VpuiNnL5VSyPWunmq10" &
            "vBeSfRvkaKg79eBITzve327r8rTpxZf2fi64kXSfOZ6LOXjOPQ71ipGuK25u4/EY13WUMaZkrkt29HE/P2bvJMmZc1mWp09hLZ50" &
            "Xdd2OBxikFmY7NJYt/JLJ5AnbQRjcv22SHVXtfjsRzbVuUpyHo98eONxTX/FqEj+jJNkrnPSA2Pk39P5sdY9nxZqcvycb50XcB6n" &
            "mFUNmz0kRcpD2GMFdcrn8pl8QnmZO10byYdfICS79KIhWKvHdLsRlX2SieTDdUI23g978z2fldyvKYDfTGKmOIf+1zCbe2TnNc3U" &
            "l2aToI4nYg9p9pcym7/KN9M39ZWMyK664XyGaXNdZcd4SUe7BH3pk3TMQZ8ee2xJL1fSScaHJG0p41rwAVORfBMpX6+OmZiJVK/H" &
            "4nyI5NULTuXnJD3jVDrPQ5t0nNaOx+Ns9IxaluXxh+ibRdrwVUhV/AyeiA9vbWkIiZ6udfSVvMeoFuF2PXvq2LM2nqgePdtU/2jN" &
            "c1TJKlJOnnPKWjHD0Z4y5iUjfQtxvHfpmJM+oqqXMZxKXjFjN7KhvuorrR1eJzzX7FvH8qMdvyupYK3Jh9eb59oD83Cr5K5PcVxe" &
            "yXxNO9pIRlttnEeF+zCOWJalrcuyPCzhQe6Ge+EF9ZzsqdXtKp9KLkbz8WHfgl7NPuNRXc6MTdsxW9Y46yfcnv7cO0lWkWpi3Yl0" &
            "bxDq3Y4y5qSMOh7Txvcup4ww9mhz295xik254DNiC28r+d6hjGsn5eFW4Xr6uJ+O+WKYNrf3Y+4d16VYlLkfN8YTnFM76/ldlWz1" &
            "AnKSYZWEJLnLRvpbUcWs5M363EvlV8mvJZ3Mil7+Xn2Uaz3yqfSUuR391B9fBB33FdVcel+0uI51uI2TcmvNbZYqpnB9is91YqRv" &
            "EzbMz7w8rrY9MGYP6atcrMP1fHuJNpUsQTuRriXGow/XbSKOZBWVHetgDN0vfs+k7wr1AvKatHcjKkDSu2wpvmpLxY6ofJKMjPJS" &
            "nmx67LGfsfV50Z7rGWZ9PB9nxo0+I52veay1712urXdNCvrfAq+BdTujfnq+Ce91FG9P3IZ4vRjSJx1xW257UN/Vue7FrPK6jC8e" &
            "vuZX2SmWI3mlb8WMKaM/106KlfDaU76evHcOZO8vMOvjetlc6A5M5nhSvWj08PhVzKoBt+UxN9pUtpQneroWhp9mSNsK1ue2jOvQ" &
            "h74NJz2dB/dhXsdtezdditV29tF4sZ5/Ryn1IXyd9B47yXTsX3HpF2sZy/FZpG/33Z99avN6vT+vN9VKf/ZNO260cb+GWhhTevok" &
            "qvqoYw2jutyO12MVx/fVJpSDfcqm+urc963IdbL/yiHNJflJ1rOprkXGIS7n+Uq1reu6LjwRgg5V0gYd7UZrxxtN9PKISn4JveGR" &
            "Ud4Uw2OnXH7CdbFdgsfZw8y8K3guZ2vgHNLcnJFepF5GtcxQxfB+KxuH9u6XjhkzPTy5p8xtOXNei5KTlKOH59ULLx/CjOl1Oi5P" &
            "+hZ8GTP5UZ6uxc3uSdpv5xf0FNuZsemRemA86vzFhdeMx2KcdO7XZVnu+SJCWGCF7BSnKmYUK/mk9dsgzWQ0D9fxwqsumNn569i3" &
            "S6j8ZuSVTcLr3gNnwTUZ6UU1S6330vOpzk+qtbJtmCFrZQ9Osh3Ruw4d2Y1svT7WyYeXv4i4rfskfSLlo07H2vO5RV/vtYqR/ESS" &
            "z/oS1uJ7yn2dbFNuP/ZzveDHE4+vHANmLhSHxYzkTmrw88SorpFezPRJ3Wjdw0+6+3kdKV4l7+HXio6T7FpuFeeSHm+J566+Ep+F" &
            "N3tFL6779ex6VH68Bjl7rluoh3rJKxhf8byGZEdZ0nk8l3mdPCa0l6wHY/fWvq+o9LqO+Bbutm3teDy2tbX28JpHwIPMkJrxAllI" &
            "2qgjlPVsE8w3u83gdjrm3mGOZNNsbj2bPfRqSuzNywfY6KFWkeaiOIp5SdyKPT2KqsZRXW7PvH5+UvxLoD/XSZby08ZJNlXtvKaT" &
            "TWJk24snmV9DkicfyfjiXp1fj8OfQfSo8lfQjr465t5xn15+3mfqaz2dTqs7cCDuWA2sheK4dtlMDPqzybdFb6iE9XvN1LtNtY2Y" &
            "tUtUNSUuybOEP8y2t7fe+lLSNcz1Hnr9KC7jJ/tqTv5+NedHW4/rNm5LWYrjcE3ZFr4Cd1JMwbkk0gyZnySZ4PlnDfL163crXggU" &
            "y2NKT9see2xFsvc41LOu3lb9TEpyn8X6qHtzMEn2nHiuihkbwgum7YyzxzYhfz9BiUpHmcfbg9tfGmMvzFP12CPVfSm8DhLX5iDp" &
            "AVjhufmCMbMphh9Ttgf5sRbX+34PW/h4dhVvzwxnSbGY1/H+ZccXD7dNa8qvxetwWTpHTupl6/xQvfJp5x+ib/7/f7iBCqOzBxAc" &
            "ovAha0v+MzBOs9qocxsnDeQavKetc7OJao6pp+SfcFvGTzod038km6Hnxx6TrnfO0jWaGOUhW3h7QizLEnWqgfFSH7RhLEe1yCfF" &
            "SzNmDpcl2wXvazNHJWud66gVdbRg18LPe0SKsYTfLWMN7DMhPftPpP6Z13tIc01+svW92/BBLh3tEpxTBfWep+fPmayHw+EFb0wG" &
            "07GcORyRhkJS0Z8FM7XO4jNKuPwW+Sqq/LOwtkvieQzG65FmyOvQr8eKyjZdt9QnNrxIVHY9Us1Jdgm3iKOeGIvnwmfK+b5NeO/6" &
            "Pp3ndq73hI/B76k/xbwGv64YWz2k+mZlDvWL/aeByqP58AUxIf/D4dDWw+Gw+g/HeWFonRriCUuN0ycxY9MKu6rJFi560vOt8Jgp" &
            "PuWuG+WTrc915HMLUh7VkvoQ9NmL50hyMpLzPIhenVXMHuncsAbOjftKRt0Mnofy3rqFc8+6k1wPmeo7CEexmUN7P6bMSbI2OLft" &
            "7DdTZ6KqhfhstJ6B8ateGDfNKsVJvfv58HPvPSR7x33X4/H4yl+Z04nmcQrawkB6cq5dlnQi6VhPyid5wgfHjXZuT1xW2bTCbsb2" &
            "ufALwi+wEalurp1kL7bBxZ7weDMzTTaUU5/iVKR4vTxV7KrfSi5GNSed19bz0bG+Su29xZJg7pSXdVW5nCQTnnMmluO+W3j3hXX7" &
            "VsFna/J3aJPqr3yJ65fwNpffa6zT70v327atrQ8PD796PB6f/qK7AvQKos6D92DRkvl+BsZ4bi7JxxPR0KPPord9lvhNwxun6oUy" &
            "Xycde+TaL2zpGTvh9rzx6Ns7Ty5L/sm27eg/6fcgv1Sb67l2W/8CMsVIDxyx2QOGetoKPgRbMZ+00TatRcqRNsFrzEn2kl/KVlyb" &
            "zONr2lDne/prUz7tF7ylxXvH9zo+Ho/t4eGhrQ8PD58cj8ctNcGHIPXapHObGejL41twbbzkz4eqbjA+iBzOh7OrtlvQq6uFHtlb" &
            "8med3EZ2KZbgtbiXdM36WrjdTI+kkjFfZZdIdVQoBmNxTdhXVeMsjOPxKJt58XA/PWDpl2JwY8xqTao4Sc+6SCVvZ5331+uTMsZJ" &
            "MuKyFJf3OvW+16b/kfCNFxAnBXWY4DmYvbFuXUPq1/EHre8TKU46KcnuUnr1OCln5cuaE6kfriXj8chuBtau65vwpunB+kYkm2t6" &
            "atbXjO828UWN9zITs+3on4x6T7IW/LgmSTZDb1YJ5uG6F0998LnLjbD3kU0719GL2UKsyq5Bt97d3d0dDofFP4kl0itiCzddb1CJ" &
            "UbyE+8ieg0l61knZLMyzha+OBOO7bgvvp7aiZt8zZoL1qUbOhh8zpI9suZZ/b5NfgjZbeMCdwh+MTDlaqNvxfunnNpV/K/LSjjFS" &
            "rL3yXo+yZU/U+347/9mJZMvcRDaei/MYbfTTJl2a8WJvq/iHfJyUowdz0FfXXjVb4bUnvctSb9734XB47a0jz8u1fFnngnvaj5P9" &
            "DB5Tx4fD4Y0Y67IsqxyolJx72iT8BHFLVPJrqGJWclLZeR+VDZm1S/R8ezrBuc/6JCr5iJlrxkn1sg/Xc6OOJNlzsDfPZje8913h" &
            "961mzP5lR5KMMObs/T9LqnV2nUgzY83VsZPkjM16tnAd+uZ2M9BPNfVq68l69WjNY86u4ZrQ/wfypHAHOt6SqpkE6/IGE5W8BR3X" &
            "kknOAbp98m2TNjMwDufV02/4DsntPYaT4mrNvFWMHrPXU8rDXFWtoqo3xRI9nTNjR5uUm/OgvfbcaNtwnXpc5rgVy+CDNiN4T1V9" &
            "sW+XV1Q6zmXPbNL5qNbpbSmPweMK9xW9mlMewhq4TsfVtdX0HUg1TDbPhE4VY4THTTmqmFUdIuk5oLROfnvo+fd0FdVcdFzpqKdf" &
            "BfX0pd7l3ER1DnswlsfkPvn4esSsXQvzmIF2XEvWqz/phd/c1eZvA6XzwZhVPt7n9EvQhvmZQ7KKno71Ec6lsvUc1SxYx4ZPLfnb" &
            "27QlrKOXR7aUu8x1KXbyHdlp7bJ1XdeFH98i1SvqtaQ4rCPZiJ5OpJrpR13aBOvrwTyzMPcWvqqhXSLZcesx0nMOMzH3zK/Zz+F4" &
            "I6benKSvtgT1tJvRbeFnUAnXpZg93wZ9mu2emc/kks1M3KofytOMJKtsaO/wGUJfUuWoSDby5b2abFvIxVlS34JNQ397c5Iqvvek" &
            "874sj/8n+qL3sEbNzEC/RGVTyVPTXM/ifhz4npistedfyRO0TXH31ptupkvoPTAujZno5WmWa2/OGfuZ+bhex6rXa2OctKZM9PoX" &
            "7r/Zz05cP2ImzzWoRt9ImhntRusevP6dVFPPXjq3STMcxU3rGar5OK4b5fB+0nHDOXS/5fwCcmitPVkno17wVCD9uLmN+8zSG96t" &
            "4eBaqD3pb4HHTScx6dL5uwTFSTG4Fqm2t4HXOoIzciq507PRrHo20lU2MzEIz3+1OTOzaufYvNY91kwc5uaaMh0nu9aRN8zCSdex" &
            "Q3uue749UhyeY9ZGn0p2KeyF+Z3NvvuQjfewnpWbCxnAG5WMesmSrkfK6TB+wwBcz1zUsbYRm3275jH0lgqR/ng8PsXnnsfsv5oF" &
            "7XoxxKhn9ZLOPXPRVzOhLPnRTih/sqGv4qYcWjOG09NVqD7VyLcmKlSj23ITlDFPD55znwPjOtTT1tc8z2n+9NHMeI/4LD1fj1RX" &
            "0nts19GO9TG+9t53smk2C5+J2zb7SG2qUT6sh3FTTpcJ5a5qSXFc57mF18uPVJ9Op7Zu2/aQirmEW8URjKcmKX8OZnPIbtZeeC/u" &
            "uzfOXqqcLuOx26XjS6B/dVP4xZwY6QXtUq4R9OFaqBd/CDhptk4lT/Bc8XjDg4u59+Sq6MXo6RrqcNuRX7txD2ImpmpNW4LyZMve" &
            "qzqk82tKx7zOnCpO2hxew7LRNbWeTqfT9sgbzqJX2F6qHJ8VqR6X9eYiOe3TsZPkzJNs9uLnTfHTRr2vdZziXAJjtVAn4Yc8eFHP" &
            "QvvUS8rfTJ58XO62Xid77G0O147reOwvGGlLPilXkomkS7JWyG9VR4IxK5Ku8k31ak1ZG1xvW/hOk/oUs8LvDbLZd1QpbqqpnWP5" &
            "LxDS7ng8Pr6F5U6iV1BFdcOMqBoTrnebyr6FE881bairSHUkKr3nZiyuXeYke9rsoYrncXs5XF7ZOClWytmuuKYqGIM97oG1EuZy" &
            "0lxH8RpqTPEZw2fKLUHfdOzrno3o2bAm2tKeXKtvqIEyX/vebbg59JNNevFwfRXPqe6LKi713OhLPJf2a2tta0ggWNgeUrzUTEXP" &
            "ttckhzGrq5BPyklZddwunCVj3ILUC+fCvMk2xSHec882yZmz0mnNLdlVsgVvn/VqbUHPY89P22ZzoR+P6cf1DP6Q8XqqrYUeZqAd" &
            "Y7pN0jnUJbska8G3J+dasmTbw235gHV9Fdf12qc6km+bkKdYiQ1vjblc6FrSdyZra+3YWnvth+gpyDWkwjm0RBpAz96ZtXNmfFId" &
            "XLv8UujbW1f524UvXI7i9nLsIdXtMj7M/WGbNuHrrfMWDrlkPpWP4lcP60SSV/GJ95t6dpuGF5MKt0+1JWjHdSUbwblVvVHu9r6v" &
            "dPSlroLz5F7MxPF9Q130HdWXfFmT2yVZtTW8iOS/UmaMLjhnj+0MjKUG2HQLw5+lF5PQJp0AkeRJRnoxE25b+fUucs44wRy+VXY9" &
            "tvDJqco35abeN74dSz39Z9D8fFacG3NSpjXrYBzORfiaPxT3dXoREek6aEUuykQlFyP9LIzD9YiRPefuLOEDHT43+vr1QR3jCNrS" &
            "r519k7yFuFpTLlxe9cLzrmPaOOvh8f+0XTQ0GqQgjhfBn6f4UDUML973YgvvDwo2kpqlj+y49Xwko5z5ifcr25STvsmmWb7Nfmjl" &
            "uqX4j2A8Jh8s1LMW4fpky/5cJvnJ/gooST5EsXVdKZ7LPL/H5HlgPPbDY26CMR3aV7azuZwUp1msDTORjPW4H3U8p4w5iqs1bZJ8" &
            "5rpM/iM8ZvKVnnkbPk4rW8ZLfkLyas70o47Xy7Isb9QkOXO04r8s8JqX8Iz3tWKezs8azy07rY/H4+MP0ZdluZdjuog8UQ8fBAvS" &
            "uorhw9hLr0b24FTy5yTVyjqSja+rrbLT2unNmxcmfbl2Uj7GS8cOY9CO61szqrE3u72k+Il0HlzOWVX2I9g747qOJBlhPPpwTVgf" &
            "ZT3UT9pc7/biknOuh7Wv+eCvmLFJ9afnbdWTk/J5fL6I6MXjeDw+vYW1sMGZxCMu9UsN9WCtvt4ba4aqL+Z2kjzJdA78BMpu1Avt" &
            "XUY7Z/bCTni9DRev9l6D9+LXGm3SJvy4hZuT+gTrIczH2kiS9Zixr3K1oKvsnJ5N6i8dV+sko46yPTBvkqXYldxJMcWl94VTxajk" &
            "rdB5n6xTpHug8ql6FsznMXSsXyTcWig6Jb2W2XheC+tKMG5vOC5LPXItKjmhXcrRw2unn2Rpc18ek2TvVPJmNfBi5d5tKSfeBzfq" &
            "te7hthWMxb3D/JL5vpLthXlclnRt8h5pRWyXp/p5TLsUk+s9sAbG8pzUVczYVvmuYXReXJ/ypj69Tupcn6AP1y4j6Zm8nk6nBw8y" &
            "s80gO+6pF/4VZKKnE4wpvG63qexnmZlJT+fQzmPP5GmYdWWb+nf7pK9ItpUsHdM+rSW7ZqvitqJu13kMynXscspuQYqXZL17JPVR" &
            "yb0PbtT37ATteexrykUVu6cbxUy4j8esYiS7zX7+x7qqY5e5D19gZjaHz1Xapo0ohnRusx6Px1en02nj+1zcLoEJub8lHruK73La" &
            "XFvbbN4RI1vl6W3JltDHN/+BXQ+/qFpRu2Ssg2vJEoxRbfTp6ao1dZJVsW5Jiq91lZ8PCOI+PG6DFx2H+X3NmgTtXZ6o5Ana+hz4" &
            "wNR+tFVIl56PyS6tac/jpHcZ86ZNz/ARjN1Ds6SP9k+/ia7haE+S7G0ze7FfCnvkegYOmQOfgbaMRz11yYZrZys+mSQdZYJ1Vbhd" &
            "T66HgP+fzm6bttEXPtpIJRfUJXtf65g219CL5fPR3Diz5yLVxVnTxh9ElPVgHMH+e3hN2vya0XGaYerrlhvz8PlL+56vx6juiRau" &
            "l9S3ZJ5Hcn3y83A4tHVZlne2bXt6ISGemIUnWGwLr2KJpPNGvdkK92c8r4t5HNqktWAtvk55WANjE+9dNpwl/Tkv2dLHdVrzoqNN" &
            "tbXiInQ9faoXLNk29EqYa0/tkld6+lf3hcN6ErTxfHxwOOyN9Wm94fcGGK/yp16ka021pAew1+Ewb4rrctcR+pBK57XpmM+8Db3M" &
            "bjx3Ok73opDcbYVkW+f3QFo4L+yHNl6j6N1j7ue9aDu/kKzveJNVQjUrWUpY4baMK3qDIsk2xbw1sznYo89sNkaDfeqZJJuUL8la" &
            "4S8qH9WYevO+XUY7wTj0c3q6BOtMvbK2ypaxKqj3NeW0qexcTh3t91D5pWcDH5gzyF7PEZcleroRqtM3l+uYzOh87fF6MD99eX1x" &
            "TXRO/LwkkpwyX1Mnqj6XxxeR9X5d18UNk7HDgVS4noUmvzQ4r8mH5rYpFuHQKyr9TI4Zmzaw47zcNvWuNXvr3eRuT7+EbPxc8BxS" &
            "XuVOJPvUP0l+JPXJdQvXahWbMvY82hiD8YTmLXjMdQ/Zp75H556kXIzBHJRRJzinS/A8Ho+b9A5nSr3oyanbBm8psSafUTUnQjuP" &
            "x7iudxsdJ3jufFuXZTksy7J5AhY0gsVSl+StsHdG+jZpQ3hyev1W9bss6S+FJ9r3jp9EypP9CF4gPE5oNl4nZSOSXYon0nz2kPqp" &
            "etDa0Xxp0ztOm/SCeZIN8VhJlnol9J8h1Ta6VprZpLpFT7eXXqzUQ2IUw7dKx3XSuZ/D6z3l8vtV0C7l4jod81nAc7ye/y+QN848" &
            "DZm4R9WoUzWUSLFcRh1Jjadh8ThR5U2yHskuyT6veL8+kxmSLePw+BqqOKw76QlllX9lp5xcJ13FjM0ePD9hfa14YFUwJteikhOf" &
            "UbVV+HOgV3svxohRDWRUu2R7dDNrz1flJmlm6+l0eqX/UMpfZUYPXaIi/K0TL7RXZNJ7DJfxrZkqJhn5pPxpIyNZ0veo7BmzVw/P" &
            "Ic9jQrE8bi+H7wlj+N6pdL7mD2id1JtsfZN8Cd89SDeD2zHvJfTy9nQk9SR5C7VKzrdVpKu2WWivde8HvFqL6tzOoJi8B9Lm9qK6" &
            "Tkgvxox/s562wVtdsqlkHsuhD224dqTjufB86+l0euBDWdBJG0+Ak+LMMNNcGu4MbsdjxuF6Dz2/pGNeHnO+qbbk14qTnvqVroI1" &
            "XItypZxeW6pT+PXHm5gwjta9FyZCO+W7Zjapppla2uBcCupYq3S9Onythz9nTl/KXMf4CcYlI71ip+ujhf/Xm6TaKGMN1I/WhLPh" &
            "F+HJxje3q6BtWifbxGIvsOv5P5V6AwWQsY4JE/OYxaTCqL/UZg/Jf2+OkX6WXhyfOWuq/HjzVOetF6+K7aS4zfLPkmxTftYrWbMY" &
            "3ley5TzTcUWqM8kqWBPXlFE3S4rh+2qrYpB0ftP8te7NPOXw+Mwjn9kvJj0OY12D4jFuVUc76/iFi/uzH9lU/bnMa2C8Hl6H4zG0" &
            "9011r8uyrN6ED4XGHtiDu49e3b141zupQS/SYQ1VrLS5Dfe0kVzxGYs+lDEWa051a0//Km5at+Ji1HHKy41Q3utlFEe4L2sSKYZT" &
            "5WnFtdEGNdC+10vVs2xnY1XyBG2q2j1eOk45Wa/WbkNf6kiym9EJ6difdNL7sez43QXrrnIzDkl+KT/xeab87sdz6rErnZPmRWTD" &
            "WtzX91t4sZbO/0Opp6wM4iQZ4UBnmhrB4kkld2TDves9R/W2nujVc0uYQ/NMWyL1Wx1rXckod6r8FbPxvLfZHCmu+3o/smVsyf19" &
            "+1HNRLbV3ntjTW6bth6uZ6yGa4hx3cb9t+JB4nHTsdv63u0So5yO63W+KvuRrPIjsqlsRzWLGRunZ1vl9Fpd73b0EX4d8Gc0Tf8n" &
            "ut+gHsgvcAUhPT1vyktgzAq/KQhjcHBpqxjpCW3Tmnm38GGBhJ83x+O4jLW7HXW0SVS6nrzK41R9tc78k2yWKmaze8L1vvaeKrnW" &
            "zsy1Sv+kq+JT5vbJl/EJa6UPfbkWlT3ZwgOr5+M2PbseM37seZQzyZMfr4fKz497ddCfvtTR31Fd6cV5XZbl6Xs+FpRu5C18mmIv" &
            "LDgVzzXxWl1W+VCX1g71knGd7CpoN+qB8f1Fkjc0YQySZK1jPwOvk9k4tPN1ugbJqNdW+Pt8fZM958z4Va6eXeXTQh++dpuZ+L1Y" &
            "J/sf51xfbZwF9YotPFfPnnqX+YtH74spxtXGr5SJ26Z1z5aylIszI+5HKEt1ui7t3Tb5zti5TQv30LZtbV3X9c4vDA4jOQnqyKig" &
            "xKwdoY/ndB3tEsmGMWjDdSUT0vne46Ycya7HjI3DHtMN4Pmr+OzD95fSy9fwVtMMvViEtuwvbbTTuopxCYxPqvipPsrch9eAoC91" &
            "jJNysAZtfPFgrF48+tCP9PTU9XLQVszKRy/SVR6tuXdc53oeu5450/NgbY8//9iSAwO2HR+D417Hvp5lj5/b0o81US84JIfxKv8k" &
            "J6ynmW/SkRmbdtbz5LPH2Vit0/ceFCPFcZ3ruXb5JaQcFTM2rZgjc1SxeE4qKv+Um7qGr449p8/Da6aMOrepoL3L92w9aOs+I3/a" &
            "E8byfY/KhjF65549cKON79MxfZPc49DOa13b44vHIqP0CljRa9oZxWkdm56MjXF9K2ZijWZRxaCca2dGt2cOfIA4kvdsPs+kvmf7" &
            "0Aw5S2cJXyT42u+jihR/VKOf5xnStaBzOvOFIGVpLpWtH9OGsnTM70Jo52vWleJVazLSO1vxVr90s7FYd+qnikVfl3t9M/A7q4a4" &
            "CvW4f/wvbTd3kLFuEG16T5Lv2/mNomO+naALtnoYsXFBuZ8o1kY8l/eVSDr5S8fZcBOqh/k9BmcomINybknnPg7tqPfZJlxe2RDP" &
            "MToX7KcHe+CsUyz3STl8zf4qH56jSpegvfB6Ba8j9ct+kszrpizdN7SRbws9uU7xtCU9N8YTiuW2fP6MYhOPQ71fP7J1nWQp/mwP" &
            "7Iek+MmWsq34GZWQXF8suE2aZeIxRmvLsrXWtqd9a4+/iX48nU6bAqYBO2xOaxXGk+E+e0g+qo0X/hK+GmxFjIZaL8F75xyuxWvi" &
            "hUE8p5+XWzHTV7JhDyPcXxd7r/fU9wjaeXzPSeinfv0+4Z7HTpqXqOSzzOQnlZ3PQjXT1uXpRSPNk3H5YGWOhGxoy3y9eJW8hTgj" &
            "1Ic/n9gXST373n0qfz4HneSTYB293llnO7+FdVQxDOaOLuPFMlMsY7ica8pEr76eXvhwdFwNrIqR8Jx6GLGO2VitqEkPOsIc1drl" &
            "ybcNZuJ2qacUK8WhnDU1PNwJexO0pb5N/G5PCw84yZbil6oYTw8P3SO9Puhb4XUkqnp68ZN9YmS32UzSfEe+8uNWzU34eWjh2vW6" &
            "e/lFlavnTzgD9/M4Lmff9K9yVza8fn1zXO55aSckpr5+EzQUxzUZnfRLGQ2iB+ufgXaMwTpoLySv9BUbvi3Vw2tmk7+v95DO36V9" &
            "VKQ4SdYuvKY8Vm8WPV1FZZ9kpJcvyVqQcxbUz8hYB9du5/tZGLsV59Hz+gOsyjfS93Afj+Ny1icuzef3rct5PPcA/3SOCekr/wr3" &
            "o6/Lkn6xP2WyVIW1HUXN2gnac00ZL0LXpQa9eZJkrZCnC8FhTfwqQvteDEE/HvfwHJXPSO8ku6q+FNfX3DtJluD5b0WNZEafNul8" &
            "z3Nd5WedFcmfOSvcjnG4bkVNya5i1k4k+6oG34tk24IdZdRzTkLxXa5rbE/uhlgVqY5e3YJy1sZYtBfK6xuRnM+wyn5dluXp90AI" &
            "nTRY/kDGSUmckV4wd7N83Ltd1azLpdNx6sNhLEF5ip/sZmEfVZyUM+kqRvq9MGdVW0/mG+mdd4f6pGMu2rltw4tYum6k9xoZM/nN" &
            "Ql/G5ronS7WRDfdHmgVrclIen1+aZ5I5lCk285BeLRW0r3A77nlMZnKMbJizZ5tY8C6H2LatnU5bO/+mx2u65fxf2t49SSYS88Rq" &
            "P/Jrg4H2dM7evBzIHiq/FNMvxKTfQ/KtZL257V2TKnbll+pJtknWTF75tYkbfg+eR8cpN9ezJL9R/cnHcT1jeS+UXcMWfo+IjPSz" &
            "jGJU/VTnTnVRTkZ6Utl7HaPzUcWYeZakOSXbVIPQbBRL+zd7yDG28/8HctDiZD/VX/TXFotPpzRL5CfJC369iPpkcl01kt4rZG3M" &
            "6XLacE0fr9VtfOiqS38agjW6PWslskmfz/fZMYf0Oley8Rr8W1JH9fna6/YcgudHxyL5EM+j+jRHxfce9syvmiNzysdhb8pNGWFs" &
            "zp++jClc7tBf+Fx0vlzXzNf/fAl9R7gP+0m1Me6G65a1ykYyv5eoY87e5jl5TjxH5dfQsyO7CvfRrP2vlfvefViP+3tOn7F8dCxY" &
            "e9VvK65Ff/7zejqdTm1d1/Xpz7nTgMe8KKTn3osgPd1eenW1kIu1pXXSpTgjUiyPl3L35DpuRb/0TTYjPG+CdVSwh72wf+1d7tes" &
            "X7t7YZ1ci9neHdbEtWQz8tk+Xe8z0zHn2Iq6Zkl1bcVD8NIa3JYxGIvyLXzBNdr8Yaw9cXllk5i1VS2Es264NnnMGPRtnVztrJOP" &
            "x9SM1vYYdGFhqYCUvKGBCsZyqrjEa6piXQLjej86Zo2zPY+oeqG8qsNhHLfl+SXsOW0zzNj16mjnGLzhJZ+JP2I2huclrKXqiXI/" &
            "D9TNUNXEWD47bjNUtp5fW3pu+PnTA5ybx2uhhxFVXy7jdeT5qafOYzpb6Fn96PzSn+seyc5zpntZPsk34f3Kx2uXjahyrqfT6eF4" &
            "PL6RuGpYQaRP2yWkAiuYh34zNSQbxpnFY3EOnA3z7s0pf17AjCtm5sqbzDf5exzuK3p1zSDfLTwI0jbC7Xp+SU5fkmScT7IhysMH" &
            "GdeydT/ivlt4G7CK4zko13Hak8qHDy6S8ju8Hit6MYTH8Lp0zLl7H4zPXnXMrSLFZ00u95i9e9JjtuKZwNrerHM5/zb6o5/s19Pp" &
            "9Mr/nEkPL9hhMSNSjEvwwVQ1eM1V/aKSC958bs816/F1TzeiF0c1zFwgSaZjzso3UsmdGZsK+lW1cN1Dtr7njeHHHjvpKKMt/VzG" &
            "jWz2sEgPM24JXguEtTGOr2lT1SWW8HNEUa2Zf0S63iW/BK9jpha37b0wiqTnHKjjluxEshV8fnBGbv+m7tPz/Ri7Pf4tLHegE2FR" &
            "spdv5V81tAfG7sXr6USy8TqTXjZ74Izcv8rBWXJNGLOFeUlXbdLP2CfbNnEd7GEUp6erSLUrTurRSb7U6dhtufWgb4LxKjunmtVW" &
            "fPGV4rIu1kofHaf4LfThMh27Lf0crilLPsTrrOpwfdJVcsmSPLEV58VJ9VbxXc571K9/t9H+UfzmdznL8umfc38t4AglmrV/DrzZ" &
            "amizbPiWUDLXV1DHdTUjnmyteR64lizB3A51XPdk1caZJb9LqXoUfjGPbEVVj+SzcRzvc9QzdZwn/d+8YR/3tEu+jmaU8nB+VYwR" &
            "jMH47EF23Pf6GNm5zHXVcVpTVunTtZJyU9eDNpwddU4vd2JPbL14+Kdydby2x+9CXht+C4W43v/Yneuk50XCWNJX7zF6HObxGIvd" &
            "FOlbR5enze2qPA59GnrVYL2ulM+hjj7Up9g+Q/armPxIq+Qe22UzW8+voVbaCp+fdMmHSC/blCvZCD/fzX4OJLnr/KOXvp3CR0yT" &
            "XUM+P18O6/W173VcxWmde5CxHfkkW+Xyj6uLXi2pduXxuSqu5sQ4knmeXi+i1wvr1bp3Tbo9Y9O2Wa+8rlRH5d9aa/rZNK9Ht/NN" &
            "9bMvP65gbdxOp9d7enohWZbl4MXR0RMQFuYFJL3wQtJwRKqjoZbKV4z0s1Q9cd1umLOHcoxm2OzmE6nmJOuRzs2ojllSHOaSLB37" &
            "mvIR9NvCQyvZ69htfM9jxhGUJz9uhNdG0jmsl7KUYw9beAj6ccqbqHyc1N+IFGs2TvJ1enMc3Zec2aX05qYc/VzLaz9A973+lEn3" &
            "b2G10IwX4joNioWOcL/kz7WT6uK+B3sTrCXtU62CMel/LYx/S6q+KrlTzZOMYvk11WOkb0UuroWf197Wg3YjX8oq3/RC5sd+c6fN" &
            "baqaXO/faRDWzHPOHmSTrg/WQFhrr+5L2MJ3Hg57SbVWdSXbXq4ZejHTuRZeW7KRr7/L9Kh+87vCbdvaqlcOBpWMTiIVkujFILKT" &
            "j2/PhQ/bYU6vrcenQ38zZoP/KFaCPqN8hP4uq3S981DJR7gf94mqBvetbBLJhv7a8y2TRCXfA/PqmLkrGUnXxIJPRVVv5/g1lWKL" &
            "SsceSKotkXwJc/V80vMtzbCK4baVjUMb+vfiVDNijIrkTx9/fsg++bX25vXX7L+0fUNBXF7ZiDQYl1HP44rkl/aJNBTGa2bnA3Xb" &
            "itQLSTqvgX1Q576MI/ymT1tF0jFf2pKevr6mXY/KhjEqO0F9r+4RvMncn1uPylbHlM/CWNr0nQTfJ6cNc6pP/05EtvxuyGO5rR+7" &
            "LanuN+4TqomyCtbF/rSXropVyfm8YT9pI5R5LTx/Cerdjsdae92fHj9+EsvjaVuXT/9/wtfwB6gKTl+pSO97l3txCV6IHHzCY6b4" &
            "rKcXk76tHOKn8MWFpJguSzULn1nqM8kEP9yQbBJJ77Kkb3iIar8VN7LkrMd9ae9y9iVol3S+Zp6kUx7l5Fy5Zx6hmKwx2adaWJP3" &
            "X82Cfr7mQ6fKmR5SDd+JycefCzzvTsrVzn34p3to6/C8CNY0i/txreNK5mj99Mkk+8BK71phnIqtOHdeF/eMrbXPrXqei9flnx77" &
            "OXvtOxA34Alq4QJyKOM6URWfbo4WBsP1LdEMeAEkG3GLeuTPPnmcZBWX6JfwqSbCGqqt8hmxx/bW+PnXDcPzPUtvHnvwWojiplxb" &
            "eAC5nz/4tdYngKp7nnk8l9CsXEd7t+3NNsUnzHMp7tuLk3r1a4Y6cWl99PE43JMqZyV3eFpeuy8e16//1UWh4Fv4qtLx4rkR1zGf" &
            "65OMG21833AB0176BR8P5Kuy18kLgzG5noExK1Lv2jMG61CP1VbpZ+qq8Dpd5jp+AeEyl/t5pL/D+mXj55S5k46b9NU8OHtubpdi" &
            "MI822bN/9S2dfPXQ93jen8871ec5RFWD8Dyq2XVeI/P11tWcPJbyJhtf++Yzpt6hrOfvetf5zGjvVLLRluySf8O14nrac81rRiz6" &
            "LkQL7dOJE16M4/49vIC0TszYXAuH5/K9JB+emMRodpdQ5Urw3M/UI59quxWpjyRzRvpmNjO2bWA3ew8In9E1M2NNupa9t7Q5M3np" &
            "UzFrR1iX1z/iGl9Hc0h+Pjt/qCbclrURrzXZJ98kcxgrxU34deDH8uP9vSxrW4/H48Pp8b+ces2RyUYX+IK3PBiHQ2H8EXvt26RP" &
            "ZcN69+AnzPvfGyfh54Hnw09uj14t7jsTa5YqX+voeA6SHWedth4zNoKz2IrvSjQ31jGby/17zOjdhmsiXc8m4ff6jC/tmDftZ+I2" &
            "XA+X4rP3eIxb2bg+1UEZ15QlPfHrkHXxu6YRisXzymt9WR5/C/1h27bFHYgHZFDqR4yaUFyPP/IRs3bC7VPtjFedhOTrJJ9rqXKO" &
            "ciU9Y3Fdoeuht7Ui5wiee98zHmVa0064nnHT5nhfM3Oq4nAtGDP5Mr/rU+2M4fKkvxbG2gZvK0nGfbVV9HxHJBvKPH4L50owd6rD" &
            "Y4zi6Nj3YuTrfpTRvnqh8euM+rW1dtSCSdwwPRgSvLgdxmaORFUDYRyunZm8QnZur+PeTZH6S7JLYQ6Py/hp3aujkl+LclZzc5nj" &
            "fVHn8t7mtiM/wmuf9XNfxaG8skkkOeMR6r3+qt5RPOH3ebof29leP6NIcT132tzGqfK1Io9gj7Rl7gqfn7OFn2ORSp5gne7nx5xH" &
            "ula1Z22+5s/MGDPx9McUU0CXk6og6XztNtykd1seM37rNHQJGnTvgwQkyQR7TDGoSyetwu1SHNqwJ4e1sQaunRkd97Owr94m++ri" &
            "ly75uY0fJxvhObWvfPw+UQ2sMfk5lY51EH+AtAn7qp/K3mMzT/XD1xZsmUfHfKAxjkOdx1SMCuYdIZuqR+X0GmZjt2Imt8Jr496P" &
            "Zcs6eE2ty7LcefAe1YNo5OfwxSXxHIMTo7gc0Aja+vAreGJ6thVbuEh7N1q6aR3KuK5gH8mP60sZxUl6ybZiPjNzqW4ql0vnNozt" &
            "e7dLOn/hIaO4Il2brLkV8VzHOAnaqS/P5/r0HJCM8naON6qj8tM+1eFwluqJffme18alqL4qhstpU/XjsEbGYG6vxzf2u27b9vSh" &
            "chaSAnKgDRe7+1CuApxZ3WijT2LrfDRSx/TnxcMt3ZBa01Yb5+fQNm2VPc9N5VfZnfAVPGdVUeVhrYI2nlvw2mn211tdzpwV3kvq" &
            "3e1SXNVY6QjtqXM9YT2cP++TVtgxttc+Ux91PCeu07Hi8nw67lvZM6/Oi+tSHf5LiUnPdQvPPV4rvrUir/zYj9u6j+9pl3LK1nNq" &
            "rblxTlW97ud1OlW/9FsfHh5eeQOOJyRVka5zP7f3oVA/y8g+9dND9l471yTJWrggZ2FO0otLXYohGS9K4nayZfwevR6E50i2SZaY" &
            "tZthpkf25sfJ3+3dVje99LTxfTomSUcZHwqz0LbXp2yTjcOYwuWVTQtzap1ni5NiJplIM7yWVLvnoVzwvmVtJMWp9qLXn+fUa8Z6" &
            "PB4fHh4eXnsVTLhuw1e7yYeyXmFOskuDm4W2PoTedi1VnFEvVR3uV90oyc91PE52Yum8neAkfaqj6ivRqy/NT3ZpJknGuIzJPfO4" &
            "3teVL2UVjF/F0cbrwHXSO1pT3qNXr6h6q2qbwe2q/lMsl2s+qoMxXOaM4jvsUbKl+IsBTsrjsqRP0NY3/+5iFIe90J7r1lpbj8fj" &
            "cdu2JSkTsuNF6P5esMt5we9lVOPsoBqGzm8hfZOt++wh2TNOysfcoppfFS/lSvZEfp6PueXHG5U2KX476/SVuMu4r2Ik2QjGJ8vg" &
            "rZJqE/ShTPs0K8IcKR9tKfN9G7yIpBiSV1Q+iVTPCLet/Cqb6poUXo9vrueWSLpeXsFrzbfqmTQLfSr/3vXwKbVuvbu7uzv/QcUn" &
            "mCg1sYWbgPo9pPiuS9CnYqRvHZtUD+npCHtk/CQT3i91rif0cZskS+tK1oP2o/pHJP9LYyUY12Xc3CbZuTztCX2SPY97+bjuyRnD" &
            "5b5unQcN/QnrYP5kS1Ju2qZ4wnut6qCejHTEv7jiw7rKVckTrFk+SVbhc6XtsixtXZf2+J9KvTn/dVmWF/oPpdzZm9Tej0mvCMd1" &
            "VYMpJ6Gca+K5aJtqqvS+TraCFw1hzkqm45TH+0n2lEvnNi5L0CbF1Tp9u8y9k2QJj88f+PVmLNK17LrK33v1TTq3qWSEuXq2Lp+x" &
            "cdmMPNm0jpwo3she+rRPm6N5cW6Evr2YItlQxq0i2VDW8xfMV22yJcwvRvNrIbdYzi8ifKZt5z/n/tIFqShR6fwky4YFj2KLNBz3" &
            "nY1DZn34QJn1awNb1e1v2bAvkfJz78zOpqdLKN4ovvryjXriMsZN50Abf1ZH2x4pZsLz+dr1vudxBe8T9eJz8zijvljnjP3ePKO+" &
            "enrO2/fp2G38vHocyRmbW5I7zE2Z65K8hWuPtp5b9wTPNWM4Sd7rRzL5JTvJZ6nmrf16Op02JWLBPHFtkNwb6Nm1yTgbHhi88CXn" &
            "5riMdrSdoefvg+bQG/qSTZKnvedjDVvnPVP6VtCWPjxOm/ec8qW4zWbFeG6755jxe3quW3HTtLPtnjn7mrGoTzLuHdpV65RX8Fx5" &
            "rWmTjeeq5pTsiMtSfMbosYVz01AfdY7L3E7+9EmxmIs9VzI/9ro9h8vp56SaKHM76ihnbs5j3bbt1ZP3BEwmOBgnFX9LqnhVrT6U" &
            "yjfBGJRdA+OpRt4UlKUYhHKuCfv0dXWeeWHTL+VMslb4JlvJ/Su6ZNfMttI7/iBI6xZqp14kudfBOLN67XmcvsjqwfPWg7Gb+ac4" &
            "vWOtfba+lw03nmtulb5iRq897TxXK841YQzJUvxKPsrTq4fxPMdmL3C0a8W5WpdlObXwn0rNwiSXwmH14qbmCIfE414M1iJZhdv3" &
            "7C5FcU/n//NBF650s1T1pT6TXQtyrnnRUi8oTzWM0Ey010xoU8XbKxeVnr0L1VDVUvVeHVf4PKp8qUbaCPnzodzsYeIPFcIa/GGb" &
            "eqPMN8/Petgz9dpSXM/Hh6Pr0rYXr8H3FazDSTr2Q5nwFwnt073TQkzGWx/lbyZJMpKCMwmP05q5tKa8R5XfYVy3rXyTrEfPnie9" &
            "smvBllR5KGPPPWgzmk2zOnsPk+TnjPSO1+L1jV5QKU+9MWbaj/AZcBazMRzWlI4TSZ/qcRutKZeuwuPymPF6x4Qy2qe1Xweun8Uf" &
            "rntjeC0uY52XUvlW8oTfp7weKnozWLdtO3FYbPoWpKEm9g561o6wHjKqMekZk/oKt+MNSNLDaU8uUfVAKj1r9gtSxx7f83FOTk8n" &
            "GWO0Yh7cXN9D+mSXZCTZJFkPr5l9sBeXzfzXu5yV72dgLawnMWvXTMd7QdeUwz65JrxWXZ5Q3clnBGv1WVM3S+U7O9vUQ5K1ECfl" &
            "Xk+n08OjLl8QI3r2PjDtacu81TaiZ5cGlGSjfElHn3RMG+kSSe4X/WK/4Zp6cFKsVsir2li/95HipJqSXSvyJF2K6Wgmsku1LuHh" &
            "QxvJfD9LL1ZDD7QjVQxfp7ccKr+qDq2ZQ6Q6vc+00S4hefIjuuZHyMbvk0sY1XMpo15HM+Ga19MefEZpTp6fe9cv5/8T/ektLC9+" &
            "VNSMTY+9/rO2vbhpWG3g0yb0wm2q4xHK5Tnp73LqXE9m7KucFalWyhKX6vwcjm6C9L69652qXpdVesqTrFl+6UaxheK5zVI8+Lfw" &
            "M4HenCpSbSkf8by0V/5UR7InSe+1MUaqhWvaEs2N+iqOoG2PNI9ZerErndfe60FI/6bd9vj/gXiQdKG96fhIuiHpK/YMdBaP43Wn" &
            "GlyW9LekOjmVjFR2lPX6YFyuJUtykXISt+EPL8lMPKeyT9eoy5JPklXIjvs2MfOUI9WV7KofMlcwrm8eq8cWPlqamInVzjWxFved" &
            "yeU27suY3HQNVDnYA/19S1R6ylgD+0mbdISxroU5R3LxKF6e9trWx7dNX/+PlJbwnytVgXXB6FhU9l6kn3CefNajPFWj9Kfe19R5" &
            "rhaGyb6qTXgdexn1QWbyKAbrndkqP4+rY7540If+jvfNeadjrv0P16WZMPfIhpt89pybZjG5TlvStzAbbfx5B+vx9RK+ip5hT+3C" &
            "62EfTupPULbhdzK4uV86JvTvbS3c067ndU/bRJWDep+D22rPWIxDvfs5W+ijoZdH8dIe/6zJ2lpb27o8Hi2vG9aJZmETe5DvzIkQ" &
            "aUhci3SiXF7pGbdipBe04TqR6pmBtmlNmUjnQPa+VXqtKz23CtbB89RjdC3R39epLsXrxUxU1zZz9HQ6pj1fRD3HhocR4zuMT31C" &
            "NfK7UG6zVPWxB25u53sn1ZHsWpBzXcmc1P+oXtf5bH3PmG6/F9Yjqhxiba09/Y+ErROoTQ7Kob30lIut81ls6avBJZkjfYrbOicu" &
            "HfdmVNVQyYX0yS7JZkm+lHE9ojdryTmjal4Jxa9yJPbY9vC8rIE63yc2fHqnmkGKkeKn/A1xmU8+aUv09Mnf19XbZvS5hFlfXnd7" &
            "SXnS+VOelEsxKl3KUeFzbvY257UzTbW1ov+KZVke/0dCFaL/zav6H716wWlTFVhBf+q4Uc415R7Lj2nr+AxcNrpwZmAdghfpLHty" &
            "X8NsHtn5vKrZjc5Dm5hFz1ekmTO3y9OePi7rMap/D6NcDdeuvivgWxTaqzbqPM+p+NQXY4nReaZMx74nlVwwX6LqT6QYW+e7N+I9" &
            "phxp1lqPYO3JJ8l6eE8ef4T6WFtrJwmW8LOPS6gavCRu1ZTnYC4eJ/0WvhtxO+pmLpyKGR1rJJU+yUb4RePbCOaq1pSL2VzeaxXL" &
            "4Ww4U8ZLMqcnr7akJ+o76QTjuYxxKaOO+/Qi4tBHxy7nNksVY4+sJ5fO95QluZOuS/onP8HaerMWrh/Z3gK//1K/gnUllmV5+lMm" &
            "T4IlfNV9CZV/Ktpt/QRQ1oN+bu83LmP1HmjL+QU1vaiyvj307LfwwjYL43JdoRmkWaSZtXCtUJ+gbTr2OIzJ2ojHoC9zJRnXYubB" &
            "m7YebjPqK8FataVaWQvrlJ/wejxOgnF8E7z/Rn5b8XZYip3WlDOOs028zZhq8y3ZcGsW2+8b92U+X8uv2oj7z6K6HK8xsbKIypBD" &
            "SCTfyo/rBAdJpK9skpyyUR188ejlm6Hn29OJS/Pzgqs2xe7lGc1M+PmpYl3KTA0pt/fnNg7tUxzX+77C60218xqbQT5eG1883I6w" &
            "t9m8hDGYmzrpeUyZy6lzeVqLyj7JNKd0LpTbX9gkYxzZJXr3GNe3xq+XRCXv8cbHeNmEH/NClK0ePpKN0KdFmCuh2PzI4iw6mVUu" &
            "yaSnHfXJVnXxwkvbSCc9Y9KWX6FVdoxNpKvsmGN0npOctciGsXr6JOe14DYpn9v5XsfJjjmFnx/OpYqV/NyH9r3N8Vhi2/Fdhd8f" &
            "rmMeydwmnQs/ZmzH+08y+i34CpnxmMPn7M8Pj6d18vV9C/mE+6a6aevHKafrKr3DPFyrT+lSbLfdw7qu6+LB0wnXng8tMqujHfPd" &
            "Asb0PXUjaOcn5FYw3p76BC9AJ72IcjYt1MF1C7FFin0JPD861gNRbx2kHEk2gz9g2LPWvqdNBWeS+kq2IxlnwFgVIxuvsbKVrnpW" &
            "sD7fknwGt5v1qajOHXtKsl7NSe9x6OfPXeVp4QML7sdrjzG5dqSr+t+D6lib/Sl3L5yFpOFQtwcfbAWHdSmjXEnuPtSzrt5s9nCL" &
            "XhnDa+v1RNhjG/hVsSlnHSNoO/PQZB6X+dpJ/TrSuU3PvlnO9NDkmvWwVm60lYw1eT7KUiwhuc+besVlLLdPudwv2adjX6eYs8z6" &
            "jOJTzprcl7bE7XWedDzydSrbXh8tXDPCfXR/8D55+ltYbHYpftDjTY4YFd3TO1WDFYxbNT+C/Sf5tbAen7PWe2HMJJudR8+mV9uo" &
            "B9dxnlU+4T7cCGVct+LF4Rawtt5W4TrZ8tz5mjrGuDWKrT1zE7fnNoNs0wsXSbl8vQfGcDnX3JKdYE3Jb+Tbw20q+yRPdfO8btv2" &
            "+OfcR690e5pJF1DyYWO04Y3AmLMw7l5UW9qSzQxpRonZeMT9qjysmT6zNToplstGW4I1JDvGYf7k4yQ/ofxJR5nWKR7r24qvwumT" &
            "jp3R/cGaEj25b624dmf8KedxshOVLska7GmTcjs+T9qmmLLzjTbVcU9GmIM+XBOvi7a9uK6nrJ3/P5DXNByISDKXkyQTjJuOb4Hi" &
            "ee2X5mCMS+O0zk0/il3JReXreVxf2TusMcGYJMlaMVPW5OskTz6uT2vaztLLV8G8W3hrYhRr1rY6VzP+emHwGFW8Fq4p9ljlcdyn" &
            "ItXOfIR2aUu4zt9KYj73T8fcU+9o5tSl8yFoW1HZeU/JJsmIfNf2+NexWhtcMG0icDU4Z2Tj8lE9l1INkGuX+7HbVfsRe3qbjen0" &
            "4lf9zzDrM7LjTAnrSzZiT64K5ms2wxl/UdmmPpjPSfIkc3p66rweXiv+4KLOZZwPZ8icKZZsko5wXozvsmSb7Anr59rtEj0f2uyl" &
            "8ksyZ1R3FbfCe1vXdX36Uybb+asjXUD842xaL+dXTN83+4Hb6P1JL4D4R4oZxy/c5Cu8fn781/08diUnHtv1fgPQz3XtrPfeJOvB" &
            "WtNGWx47yb7aWvHQSLHdzs8Tz4H7Vp+AYmzJuLUwY50jl7MHJ8X0/EnmOtenTTqvi8etqK0hL+8L6SnzmH4vu075fVYeQ5xOp3Y8" &
            "/v+Ke7sf27b0vOuds2p/nH3sc047p+1jHDWxHQKxFUJ8kTaRIAgsEPiGOySkkHviK/8ZuUQRQRFRpIgIiQvHsuLIchS50+AosXHi" &
            "BDBtkwCx2jHC3T4f+7uq1hpc7PXUefZvP++Yc9U+bR5paM7xfjzvxxhzzrVWrao61OFwuI2vucfzwdpoo7nn1tWQcpLuiF/qU37M" &
            "Rxjh+vN8BPq6rrAnPDf6eU0+lLPnKnuviX7k6GSei3jZB9l1YC3kW4/H44UavxdKhOTS7YX7ae6NICRzXWe7hb0+nd1MTt3W/P9v" &
            "nJNPWoNOznP6ELLpRtqnW5xvu0cTmJeDe3oGvyj3QvE8tufAfATJGaurY0tX6C15E7o8FUM3Pr8BJvsUT3ryCu5Df+4Nco3wLq3D" &
            "XrsZUp6pH5QlsBaXc+wBfdbj8XgYzaLNkBZh1jxPMvl1vsyJT9zO7g8anhNfOdwF8nP/rkdEt+kkS3Kh0++Rswephi0OwjlTrBn8" &
            "QqwQm5zUESO84pPc7dN+Tnx1xpp2YC7EgneBhPsnjpl+du12PpwLKU6SORh3Zpdy9XP36+TUVROTccp4ZjV1OTAf+gmucxuep0F/" &
            "2nfn6/F4fJFudmwA0dkmPyY1QypM8u4VKIvikM593gbk7QYfJMlP5wlb+g7J3uMRni9zTX4pL7ejfI+eNg7mkWwSEje5qNe5y7Sn" &
            "Jde6phdd6ebhYA7cI92D1+fphiJfghwO+jKX5Eu56mVOfiSc3/k6ro5nhuTHdUl5yoZHwX3IX8Hewfss6yTIxZ5xEEmneVrvrvey" &
            "F4/7rGOMoxRMuIPsUkNSEjr6cH2SOyj3OeN9p6AYrKeTOVhjsnGcY/s2mMWhjnpHp0u+6Xwrju+rmV2SVeBPYwbX+7nvu7vsQ/Km" &
            "fGa5uT0fZsyFPJq7/9bDw68B6dmDdCQX54Svc6FOzWegLf2T7Wh+xpp4Orh/13/33+JOPq7rBm2SPdHJBen5ImWtqnuvSXZiND+Q" &
            "YdIdthIWEhcXZy/2xNsDz4nDbTj+IJD6Rczymemq4eV6+A3A83HfxCPQtsOWHXOQbAbPdwbWLPjNZ4Y9Nh381SMfHH6ebmRb8J7N" &
            "ctyyY1zaJj/aSEZ0dclWusQnUO62Se695k3U4bkxT/EyhoM1uMzlKVcHa9gD1ea8i30ZKeX/xn8kFLaCepOErqhu7vYuS3Zvg7f1" &
            "75Bq/SLxRfHv5dhr18H3AveFg3Pii1h3cuzp5ZZe4N7njaIaLs8l+dROLj48qO8gW7dnLbRL3DNZtwcSlvBCQ3UleAwfrmPOe2qh" &
            "75i8I+PxO40UZ6uvjuQvcK957TM/x7q8wmtCJ/YF8uZpkSXXV/G6xa+wuNwIzu+D/p2MQ+Bm46ZLIAdBHecC6+vshJS36zTSTUS+" &
            "ztHFY+85HOSnjHEpp8xjpJsgc5FumXw1Vzy++Xl0eD5u57HEzfw0PxwOt7LChe3nyTdh4MZVkz3AucudxznIKd70dVvBfSjXYK+o" &
            "p0+d+HijSr6uV55J5yAne1phfZxvi1e26fqbxaZeNoT86K+cj/arFmldPT8/Mhb53ZZ2kulc4/YL2ArO74tL1yEl5kVR7ufk5XwP" &
            "9vqkfCo0hPV0fnuR/JOsJnLBe+YbyDHj8Nq2zlMfutEh5VfI0fXMgbYOcns/tvKiLuXYgTELvdmDFG+vbyGuoPi6IfCmlfiTbIYZ" &
            "T6dLoK3PtzjO0fN8hBtkx8c14t7yIbCGjtvBOLXTz5H2A7E3J16PPvwa01ir6rCEX7pjYXSkXddIJkxuxhHoVxPbTt41Lc1Tro6Z" &
            "LiH1g/3ymHv4t2yoTzl4XManP+WdLefcGwnMgdyUC+TtYtE/7dMOWzYek7lu+Qr02Tvk2/EcT7/wlx4kyYegzGtMNozvtuRy2ZbO" &
            "b/R+nMXozl3G0UE67RvfX+RgPiO86p/Fcrg9e+A5MW7yITp5BR3z9j4s9rxYq+roF4Q3qpMTqbjj5KMsgtypGBayF2zwFpKNNzPp" &
            "O+y1JX83zgHtO44kS9iTY8LswpNM4A2v46zwIElwf513PilWkhEDN4vOhzVz1OR3nBydnKCd6ha3rs8Uy+cpZ59TR7sE+iV0PJxL" &
            "5vJkI7DHjOO+6lnaM/SZjeTX6dxmBurdt8MWb9Kn58Htw2Nda724uFi6i5FkQicv6FJCSe7F0552adA+6TyGHwX1YMHPYgTa78Hs" &
            "5skc78LfgVweg0Pg+rMXFdYnxRkbr4IIv7F9UfA899SqmynXq0PqgermQzDdsNx3j7yz6cDr2Xvsfntf5NFvFrvQC83JUSFPgvYV" &
            "1sh5GcP1nR3P/ejocmW8pOPR4fGFFKdg658WkT9xzvIUkl9Nal9OH2FdclHSkVCwYZ+NuayzZSGzOEkmbPkKe+x4gY3w9tGP7tMh" &
            "6T2Xu8Jz6Ybb7ok3s0t11KSXnp8fZ+AG5Tyhy3lMvuEkuJ792oLi+g3S5a6nTHZ7H7Du10G1+KtCyVKNmns+e5Dq0GAsH6nWbn0k" &
            "8/w9pqOTC55nB7fx9aJe5wM/ByCSTPBYHK4XFINfGnHduehipLnXKRn96/QvbW8fIFtFCZ3cweQc9Oc8oVuAvdiyJS/tOSdSrUmW" &
            "eLy2PdhrRzCftO4cyb4D60j+3BczTtpW4PQ8qWN9LiOvQI6CrccaG38jiXl5HufAfZK/1+Q9S7aOLb2Q6hDSGgl763YOHgX5Uy6k" &
            "HvFYId+UV8qXHH7cQsrDdUnua8mc3YZzyoQuDjHjIMYYtS7Lsq6nXxLpnsIp8HL6Gp4/If0bXPLxhMS1hHcrPp8NQbzUa/AV4J4Y" &
            "nrc4+E6E9i5P58kv5XYXkDdxdfKyGrsN08krbDSvlUg89HeZ7xfvldaD/dNwPx09H3IUvhrqdj4q3DQ8lvs6Un+8Rr5bIDeRbL2m" &
            "9JdxHZ6n4lfzVWq3cx7asFeMoSNzcXnyZRzJ/K8BJzvG57syt5MtdQRjVFjbNAq/syM/+aYcXEdu7xf9Bc4F9WKE+7xieg6eh/fb" &
            "MU5f4739XRBPNiVH0JZ+PPJ8j2wrj1m8vRx70Pl3tTEHopM7lPcXxZv4JKe+sxVmF9w5SPwpdrIjzrWvHXbddTHzk25m4xdtwrly" &
            "gTnuyXcLMw7KWZfr0w2KSDGEma6gn8UgyOt+1CWwzxp6aCQ7zqlzn0JO2pPEzL+bC+QXmJv8dbx9y0CjVHwH2bsNAyVdh6RPMkeK" &
            "k+adjNjafOrTDFs1i2Mv15j8+Zit9Uqx/NjZC7wxbMF9yX0OUr5J3+m+CKQcGHfruBepv0mWsMeO+Xgd1Enfgb7cI+fkw/icJ3T6" &
            "PXEJcoljhHub5JxTlsCedXLqKzwUqRfoz2MH8hNJt77ifT0YAyVdsqFvZ+tIOk90thkZK82Zc4o3A304d3kHj085c3RdAu157kcH" &
            "YyQbItnTL8WnnnOXpXXdA/J4DozZQbZ77bfQ8dwlt3PhN7xuJLD/brfHv+DDG9zsptRxJnnKK+m2QFtyVdPLbj4De8GcZ72pSV+7" &
            "eyLne6AYM1/lmT4OfO2/EToZCbumyZb2QrLvdFsNuwu6vB2djnE7O8pTT6up3W3oQx7JxUM7l1HfyVyX4NyUJV4/T35+nni2QHuf" &
            "U+76bu7YK9uLlE83F2a16DzV0PEldP4dRyd3JE5ilrvmyT/JCHIm7I2RuJJM8gr3TL8v8rpPPELyqZAf9UInr3D/SXlQrlz4LT9h" &
            "HWOsM0eBep5Tv0fezc9B5zvL/S5I/uTlnJDObdhnYQ+P/DoOgnazGIV6xuSH/925g3LnmY0OtOsGfTjX0EeAPicH/SVzu25OH9py" &
            "7LF1Oc8Tkt59E5hLZ1cnvf/QmL46ctCGdpQ50pyD+ZA72aWRfBN0jcmG151AeeJlbObY2TpS7ATaORdzlX6tqkUGHZiUF+SyroAK" &
            "HDOwaef4zrCVY230QdjiqJ02WhTGZF87XWFhZz3zb8g5RngwdEP2OqY4CbRJvMlOMtp2ducg2ac4HC7nz52IFKNwgyGnn6chffIR" &
            "6ON2BDkSTzXXBfeu7H0/zW54AuWcJzBvIeXjGJNX7wLz5fVFsE4+PDukGiRj7tR7LB/u4+vT3WscMz3XUWOtqoXJFgqhTOfLstx+" &
            "rc71DOb+Do+R4nV2kjm6wjs5OZ2bG8Zj8VyDzU02jtmCUk6OxFdNrW7Ht6DMs4PHZ2ydd/UwZw7qeBEQ1M36JNlMvwXmm0a317tB" &
            "G6/Z9RXq82vNkfo+G/JJX2MWpJcNP74Yzef4KVanc714fC+Rf2z0O+kcKVcd6T/wi4P0df9O72AdHtvhsV3G3NyPebrvLC/q3Jc6" &
            "z0FYB7LvEnRIvtjGYzCH8yRuNmoLySbJhFlue9AtBs/T4iXMbDq5wN4JnV+SS0Yd12kLnU3H2+VeO7lSf+XHmOfA8+vkPNdcN3RH" &
            "6mPioK3gMWZ63lBcx/M9YG8po34J13+Kt1dW2JvMpYKfz70XtJvB+9XJulzYjwTn7WwSmENX01bPqrl2XFfYL27XxV/XtdYxxu07" &
            "EDekMee1kRTtPQZ1RKenb1dYwizXJCvI95yfC/mmvLqaOnmHu9izxzMO5u2Y+c3AmF2Mu/ILHsePSS54LsmHw+22bN2us5ess/F5" &
            "p09IPZaMflv7P8mYn6C9nzhTPYlboC7V7bIup4SkT/H2gDWTp5MJ7se+zfwExk/9IDr57c9AEokHcKRATMrtukG4rLMRpOtsOnkh" &
            "15QzbRO4cH7e+STQlz3QsRsOxiUX7Qly05581FHmmPnuQVorx0yXkHKgTPOt2hzuw6MGP2ZJNmlsgXbJl5z8SOIumPGk2DrneqXr" &
            "kD4JndyRat7L6/pZznuQ9nGKn2I7GJfzGTrbLpb3yrGc/pjiMSlTAQxMPwWiXOh0Lu/OOzCnLwJ74ru8s9mDFIvne5FySjVw7jLZ" &
            "d3MiyVMenDtvsukG4XzfaXgOKSfmMsuJ/jryYynaDPzCHm9GMzDX9PGbo8ufPGXX4Z7cOl4ixamNaz7p2Eu36XLxnvvR9ZQRKRdi" &
            "xtHVn7ClrzNsvLbOR7Wd9S2sxX6o5vK94KaaJbvF7fky9y1fj8v4lLmP2/CcduniIb/zuA3Pae9IOsZIQ3aOrblAecfNOPSjzn0o" &
            "G6cbHl890u4u2OunHGTPNeZ6O3jTEgdrTDLWtidf2pCjkzm6nLr5Ftgf+viccYSOg77MjzZvi8S/B6N5B5SG++wB/VxG+QyeV5IL" &
            "y7K8+hkIF4UXhsv5AJkNgrwz+ySnf8qxQ+JzMBcO2dDe5+nckfIlj2Qd2D8/T/nuGY63kfPCmIGvtCvk67Lu4SEkWQdyV7O3NGfP" &
            "98YiJ8F6yU15yntYb9gj6TpOouP3c5dRvhepn8wxgeuTkHzJv4VxepeScpN+WN/PgffMuX3OsQf0oT/PHWmf0l969x2nv8b7GhuJ" &
            "JONDQ0FJzM0huPzcr5J6IRU2UhdTc8+N9rIRks45mAvnlHPQZmsumefNege+Trng/yjMchC29ELiSuc+JyTjza5CbQLrk56x3KeT" &
            "+z4gF8FedvtDIG/hn/8IHZ/Q6bscHcynwrfFlDdtu55J7+eaM773oLvWPZbD46YHIuE5eE6UuY55uLyL1ekoYw4uo1w6gjG6GrrY" &
            "jDWaryPP/IXZ+XL6GQi/yXsLJ9Z8nJ68JCOH6wUmXCGp7yQU1+OnXLo8WKPDfWZ2AnOhD+db8LUaYcN4730NRvg6KGNz7rJO53LG" &
            "E1Kf9+RAneC+tKessN7UsScV6mCMDtL5emyhszk3JmUpb76z43lNrtMuT4E85E55VlOncnWIwwd1Pncwd593fgMv1roY5JbO95yf" &
            "+4PWuYkk60BeyTg60Ib2y+kBcvun3KtpyOFwuP1fA7TjW+SyBsySo49jpqug53wG1kZZms+gPvhIcJ3OeZPa8t+C/Lng3AiyVfz0" &
            "Si/lwxw4p4w80o3mHSxtKKO8Qo2E+2qeBl8pa2zdsJKuQ+cnuFznnawbe+3c3m39eqa+cG17vxyJn0jcM3hOypPD71Hk72LwmvDz" &
            "xNXB7ejTnXPfdnvZeWfwuOQlmIeObks56xivrpv1gomngHVGkwpJsxmdH5Mk6CtZOk9zR6fbK+/mniOPlKVBe8deu8Jm1MePwgiv" &
            "OqmnjnZpn9CGHC7jIMgtJNuEjlegPtlKxlzoR1/WlmwEyclJJFmC8/Gcg37d4IsL99kCfQjpEr/r3UZ//cJf+PDY8c2wx0d7YUx+" &
            "Cz/h3JzITZ8trqRjDt3oQLth/1Dq9ofjfuMRdE75XeALQCTZDG4/86VOczaMdh06u46XoF3KIdlwuN0W0mYUj9Y/2TE/otsPzJU8" &
            "o7nY3SbtRfnOID159w4hxZacSP6Omc7hNlv26RqtwLHFU02vXJfWSjqHy6nrQE4H69Lwh4f7SddxJpngvoLvQc9F90q+OKvQS5e5" &
            "Dc9pOxsOymfzseNhy2OHcfoh+hsPjrQxKd8KkuTufy7IxzlB/VaTOvukOwedr8s9VoeZjefpI8F1XO+3WR/Hnr7NdAlbuZGPc4E9" &
            "4khI9Sgf+pIv6QTybtnx3OHy1KvkxxyTTTW1zuxpk+ySbC86ToFx9+RC+8K9z+U892uIsbqYhTp4rPDwdHS8jMkjH7qzc4Fyn6/r" &
            "ur6RBZ0Jl3mTvZGE65OdJ0VQzvyo3wKbRH/ydzqCNTm6PCnr7BzJhnPJki3R1bvgG0jnwvvha75svON13KUGlyVfzgnqR3hlSptO" &
            "Jsx0Dtp1Mb0u+uyB92aP/2yNOnTclKU8unsFfbfg3HzFLb3g+5M6nXMfVLjuPQbrchsHORwpV9cludfCHgrJz/nIzXl1/9K2Qyok" &
            "JTZDsmeSnW4Ptmp4WzB/zh2pp8fTD2WTH/P2ficu2ifQh5vJL5YZ30xXoVbac761sVM+nAuMmXyJPTYVbhh7/eijY+efZKlH7j/C" &
            "TXEJP0cUh2zcp/soyMEc9oB5dtyUb+UidPl4ruJJfMxPkK/zqEfsdfIT9tgkcK0Jcrqc8D4QaU23Yi/hI7u1ql6zJPGwHxY5KQN4" &
            "YbJ3rtTQDslW8VMObp+G28/iUz+zrbDZ3J885C70iXYce3wIciQ7zhO3QFvyMkayT71SvC24Hz/HFZzXkfJLcunE7Xbp3NfD++a2" &
            "4mIM1wvsBXnp63LqicTlHH50+852Buaydz5OPyRnTMH7w75XyN/l5NR513OvXQ8Q2tWJh/vQwbgJntMesO4K9XQxvT5yuCz5U7eO" &
            "MQ4KSkKea06ScyGfVAgL6mQ1kddb5CZ0vpR3OXh8t6G/o+NK8sSTZIUNnGy4HjO4v9tyHakXUk8E56A+5c663MZtqaMs8dKuO08j" &
            "6SnrXmknG924vC/0I2Z652GfHcy9wu/HuNxfpZ+Tq0Bej00Z/QTG7XxS3ZR1vK7ng0Pnvo99CORKkM3MlrwV8n5bcC0c6xjjoIkH" &
            "80YQtCNpmneLmRrAOX0EylORSbZnnrDHpkPqmfMlfYdON/Pzjd7ZaC30NnWEV2eSpxoE90n+Fda9s6umXyNcvG7bnZPH4fruRkkk" &
            "G3JI1vnQZqabnXO+5Uske81HeEeW7ASu7wzics5Odg66fJhXZ5f0LkuDINcWP/OkrZBiFfLSPCHpO5kGX/QMfwciMGGS8ahzFUt7" &
            "n7u8QqwZEk+FnBJmuoS99lv1EbJP+q06Un8TPEaKxXmHbm1m3O7T+ROzi0RgbxhXufAjBoE5dyPZElv5Jj+fd+eUMZ8EctHXzxOP" &
            "6qBcMh9JrnkH7ldyETPO0byYcWzx18a+TLrExzy34s72TOcnex5nSL323LpYtbO/DnGtx+NxZRAG3sKsQY4Ux8ENzVw4hO68A204" &
            "JxiPSHlRRv+ko81dQd4E6rlR05qSj/PCRidnh8Rz157Qnn3e4qVtZ7cHM3+Xpz5t9czhXF4fa2VNs/wcy+SFy948mWPiS31wXfJh" &
            "jQ7n6WLOwD6xd+eA9e/BVj8E8jHfMXlx1UH8rJs2a9n/RJ99JJCGSIQl3HQE59V5Ssx1bwPmuRe059zh/B6PtY7wcUYaSbcXY+OH" &
            "Z91mdL8ZyOf5FtZ+i4/1+jHJnNfteBFxJDD23rnzuX4PZtcFMbNlDuyH6wjWQa507rI9OTm6WB18jbtY5+wvYcaXsMeW9ZwTY6vX" &
            "xBbvrMf+c6k98DVLcs9FNb/2L21lLAMWSwLqKJshxSRks1XY2+Bt/PfkRzuXOzoOR2fjMsassFYzuH+3ebc4tkD/lF+qKYH7tBqO" &
            "NE/QPk7xvaczLtXDa0Ky5Cs76ZIvkXiIZJNq69DpvQ7vi8/py7nLRriHOJLvDDOuGTwfHlNNe5B8yOtg7ppTJtDfZcw7xeN8Bs9j" &
            "nP6USR0Or36OzoYPe5XjQfhLYHzn4t8Vpm/Z5hvhB3POS3gzxMmChNR0tyEHkfIWkk4cHrfj9fo5COrTcFs/utxtdWSuzJf2Qup3" &
            "0iW95HxH5ro0p875ki33JPXkI7f3Iq2Xw+XsJ4f4KEu8bqtzyqrpaQJjKV+X6bzzETTX36VyTto5uO5b9g63Z//cRkf+0Fdyl3X2" &
            "nmeKQQ6PIXDddJ7y1jsFcvn5Ev5qr2S8b9Kmwu80uZ45UieZ1z7GqPV4PN4w8UQgsECXexPcVnLGcV+3ZR6ySWDe7kPMOHh0W59v" &
            "cexB4u78qZ/VJySb1JvZhV9hH1TIR+e+xtyotOW8s3OZw3nIQWzphRkfc0xzyfyc1w/nkjk4L/RlhqV5d1MNb01q03l3zc7AGt8G" &
            "Xdy0zxxJzjq6vo7w8WBXz3Hy8RB5O3R2XXzlRr+0v4gtnfR+7HyWZan1cDgcxyvcKpRcGq7vkPTkoT5BiTOuj1Qg544RfqBETsaj" &
            "HR+GbpP8Uo6MQ5nL92IrpmPGzbz2wvsxmj6x1iR3PrfVuvk8cbg/ZQnkkizB9+SeQT+db63P26DLPWHrGuvgNt2N/Nz6nKOLv0we" &
            "koLXwhc1LmOtHpO5pPWSjdfvduRNMTyO0MmZw9vuoy6OwFjEOP0xxaVO/xedRbJgOnf6FIyNJSRLOuGceAR9ySG4nDV2PhX4JUtI" &
            "NozT+RL0oV+n8/VI/aNf509fxheSnNxuo/PZkUPwnKjbkgusj+dCylnnIzzkXDfLYUu/hZlfqqNDl7fmfiQo57yaHJKdME43c35k" &
            "Q7B3M05ijx974TIiyYhufyV4/XfFnpwqXAeFdV/Xdb30Xxzjhk/DiZiIy6hL6BJ0HulSPMm7Obm2sLUoe7kp2+J1bPF2euqYqx+F" &
            "btOSp/MTqHdZ0jmSnvvBz5N9BR7OhT18Xrfnwrpn8PrTucP1W6DNnpzoswczn618O/kWkh9l3Z4VaL/Ve8H1tE1+e+Mnmea893Wc" &
            "ZTkN25PJXpxJV+BhnXshv3VZlsuk7IIXitecgx8J7IXHdd+uKYyhc5cnHtokMFYnOxeeA49bYJ/TSLbk9964X2dPuD3X2nOoM2pL" &
            "68u5w3vHmILntQfeD/aG6OQOt+E5+Tl3JNkMW3u823c+93PmPkOqY8uHSBx7dBVqciQd913Hm+Tu63nJdtYD9pq5zXLiXP7kIJhb" &
            "GgkuV5x1XV99jVcGMuqSSEFE5vLuXQyxp2AHm0R+xujkFRbvLui4O9wlzqw+1/vnsLTVOftNbrdze/rJd+86O8gzg+fRYaY7B567" &
            "HzuZxjnxZ7ZbfUtIfL5Wztnxew2JjyBnx1uTmLWh2wLX4IuE+MjbyYktvUO26XrrwJrPiedg/xJPF8vzXA+Hw5HGdCRSUN1M9jSh" &
            "TkkM3Pi2NjK5ddyba2fHJvnwXMiR6qQ/Y9JeMg19JW+EHwA6pPejy/muoOORzyzX4+n/TZOz4Eu91rjjZS/c1vlc73Zek/pHP5fP" &
            "hvfaeZNMvPro19evy1PnCcyFtunc8/CR/GXnviP0SXAOrpHrC9egOL3elIvn4bbsn9snbq4JIb6uPx7vaH+wktzk59whX7fxmORz" &
            "GePN4nDtusGfFbmP7jXk1CCP8BrHsiy3f0zREycBbRLc7q7wBia565NdTeSFGjvsqTfJmR/lM2zp94A9ugsn/ToOyjX3I222wL0m" &
            "ONe5nA5xpxhJVhN5beRCXZc/5w72kBw81ka+1fgwB7/upUu8XC+3XXDToU0CdV4/b8ozsEbx0J9zr4W1VbCvnfeKAp8fGeMcsM6E" &
            "mY5QPp6X9y49jNZlWQ7LsgwReMDRPIll18kEbw4bxcIUK+lp6zIe98DjlDXJMcvXa2ajaetQjYzH886fYI6EuPbyOTpf1kl9J3c+" &
            "6gjvq4Ox7wJeINTdlVeY5cj5Oeg4HdJ1tQnOkfiSv4N6ziXjcKS4DvVx612Gg3VtcUhe4V61J9+upy4jl87Jn2TJr4PX62MPOm75" &
            "6yg751/5VHF4At4M/9YWk2XiHXdhkyQu2bgt9W7nm0RHLnLHl3iX8FB0/7QxySGQx+Ud1Luuf8KW/lywH+xR0tNOc74AoT6BNp1f" &
            "51+Nznu5p6/V7B/PhTzMLeUx0yWkuI49NSVfzyPpXbeFZJP6wnPNXabrzvW0o542GnxRyuEPkcI158NBGTklOweMUeGFbpldV6/n" &
            "4KCMtbEmgryar4lYR3+4LHYz7W6cHESSa065ZO5D/5lPB3KeA/r6nHwznUC58+7F3g2wFx6bebB22qSLivM9g5jVRd/kL5mOvqd9" &
            "dJjpupgJbus+fr7Vw5mc5y6jfMblD3za8zzNJXO519X1kzwpZudbISblrue5btb0Jyfj076TCR6zdly3zN25GYc1JR/u+bT/6UO5" &
            "7491jHGUIG0cEesoO0eXyF540ZQlMEfNk8+MW0fmnPgSN+F+9Hck3VZOe/G2a1FNHakXrKGaG2Cycw7ace5IddE2+VWQJy6H6zo7" &
            "5i271IeEPXKe7xm0TzdInqeR9DO4Hd95OthP6ivYEIl3Bs9J3KwxnRf2ivum4boEt5kh7SG+Y0pxEyRXHT74RiHF9fGar/4aLx8e" &
            "TIzwZqaxB86dzrvYjj02AutzzHLufIjUM86/KHi+vhZvA+Y9g+vPXfPUJ+oTUozOtjbquWvPuvySrMM5tgJr0eBN2vM7tzaBPC5P" &
            "NrRzPZFySnYdkn+dySGIi7mSy/vp8SWnvbA0H8XxvEOKN2zNia18BN/7PHeZ4vlwjuX0Law3HNxIcyV9cXHx2lxPRBbnhQuM5UeB" &
            "8pS8y93W5dTrfNYkgTqdO7q5uBNfQoote/cbeBWZhttuDYe4k17nrIG5uQ977KBcfuJgHpK5jcdLa+nwi03rwjieD3NjPOpmYP3k" &
            "60YHv9aELt/j6avXOie3r9EsvvtrzpHkCdTp3L9OShtBdXI92WPNk871Hps9cDudMy+3636OTE7KXO7+tNG6u63sBPpIp3Pxe45p" &
            "L6VeCOs6qupYVcdallHrWrUuy7LKeYaZnoEkm43Oz9HpXe5c5N+L5JNkAuWc70XnR3lXr0MLfxckv9l6C3tsBK7NXt9UL/NNNjN4" &
            "LqO5wXbYyrvLk9yMx5y+E/giebf6QLxt7L3+slMf/abpOfNmOkMXm3w87uWvsB8k20LyczAH7jGfD7xYUA3LstS6LlX15ouOterV" &
            "/wRJSbiMDXEit2XCXyTYrJSzwMbsQWfPuYP1JzCXFENwOW22fIU9a7CHK625Y+Y7Q/JL3LTrZB2STjJxnfOqfkvPkUC52+tdA1/x" &
            "04/xqXdQnvJj74nZHqg75NBB+437juhsUm3CFneSJZzDnWwJz9lzF0/H4bG6mgXmpKG9r/3m5873yj/fC9bD4XCgQ8KbhK/r2ADK" &
            "qN/CzO4cnprkIl2ycV9Hp2NPyjh9UejP4bru6HaOrXyEzr8aHblmOQhb+hnIz3kncyQd1yHdqP3cbRPos2cI6ukS3jnSnnq36fSO" &
            "2Y1Gc92QZoNwGXkr1Ma6tvgF+hHU03aLf0svME6C55DkPNd82Ee1e0GeLXj+PmYvoj7vy5uxltOfMnlGZyItsoIndHJh5isoFot1" &
            "vy1ZN9yWR4696Gwl582KNzO3pzxxdzZumy6KFOuuYK7MqZqbSOebbFLf2Dv6pnkaDt9vOnax3J/nScZYnPs6pX3fgfmk4TiH2yG7" &
            "N28qWVbWP50zFvOkXqDc59R1MoH1z8DcBl6xl90XZU9f3zsdpEtH+nEueI4O3rMdKV5hPenr/NKvY4wXY4whgTvRgWDCqYi7gEUl" &
            "3jTfI5PcsUx+cORIui2fMpuBGyJtts4TnHPL1rHX1jfU20Ixu1xdThvOiaTnPEF+PniTkJ3fQFI8gnr34bHQ42SbMNMleD0+9iLt" &
            "hy5vzffC6+32m1+rlPP+RdBnto4dj2xZf7JPMoLxOZeMcBn3KMFcKSO8l6k2l6/LK7xh7IuZnLdkXxQYP8k4TzLOHVw0zjvssemg" &
            "GIxLmxlmvo69upkdcY5tB+9B6scXDe5TxfKjhl8Ps/z27PvkJ3k67sUsr4Q9uSZ0fpIn/d6ciOSTuLqecY0F2p07T/A4vl+6HGon" &
            "b03q+yLAvB0e7/V6Xn3TjL8zsl5eXt5flmXxhIddQPrarvSHw6Gur69fe4UmnX8lThjhlZvrNFzmR0+Wtm5HyMfnlHfxt4a/QiVS" &
            "XMZzudu6zZ6cHB6D8DjqY6HXHKrxcDjcDtacYlXgE1irILvF/kKo9hJtkp/k7HPXY4dz6vyNiyTkUuil6zhky/xcz6Pbdvl7jp2N" &
            "x+Dcc+A7q27QRjxdHn7NJ32FdSz8GQ/vhfN1ufDceQVxOZ/vO891sXWWPMVhDxlr4Fthnp8P5pWQcky9LVx3nptzkOd1nyr9DMRz" &
            "PBwOtY4xXvuHUiJnUYVEVGT6M99ux2SEWXNmunMw40m6JDsX3oeuJ1u4S8+EJdzUCuvqm0Wy5DMD7cVBuWOmqx36GWaxvfZkk+Sz" &
            "NZjVKhmvAdfxfAvn2Drox7nQyYUt/R7MerYHaS32gmvGdeBw+LXS7YkKPaJtutboQ6R8iBF+j4mxJSe8Nj9WyG2Mz2XDHnL6e8uv" &
            "Wadm+jmboeHJpyLkwzljSU50nOdgb6y7oOvXXfnnC/pmHULqkdu7X2fbQTrugYQu5h6fTu+Y5U7djI+2e5E4fY+m/er1sdZ0ZC+W" &
            "5gUCsceGudWG39voRvhZHc/dPkH99P5W06ctJHvdFJln7VjLmV3CmLyz7OD1M67rE9g39tD9yOt4fZ1e/Q7IFZWeHBNNScjO5T4I" &
            "56b89QQ/X6CUg85THHInDPuh0x57gnm5LOWdcK6tH3muHmzxFOJSltDJHV0tnneS+5w23X6jPsm8H+SVPvlX8HFOfcyh354WnI/c" &
            "jL0HHp/+1HGci3N99sbjGmxdby7vbKpZ92SfZIXffhe2Hh5pTZM9wVxnto49/RVSfg5xdPqEz300uJ9f/SLhVZ3egTDhlHgnUzPV" &
            "0LfBHv+9jei4turcC8/DL5aErQX2nCib5UoblxHJprMlOjvKE2/SO9yO+jc3bo89/SUHLz7aal8nkKuaPVGNbQJ95Jf2117ODrM9" &
            "2eGcmMnWb7ppPRK4RsQejoTU65ST7LhPzsUenxSfc6Hri3O4b7JLcF+dL/YQ0XF9ZfP6xSJ0wRiUwcQze5i4vLPZAvMjOl5viufs" &
            "8j1Im4r6NIi75kL9sL7zInX7JDsH7sPztOYpLnV+zrwJypcdP9QU6DtDZ7uVX23szW4fuCxxsw7Wm3w6JNskOwczf9Y7y7eTO2Y2" &
            "2g/CVn86e9d3kF1aU+fh2na5dPIy3cAP44lZLZR3Oj8myHdd7X3czCFBBfhber4tLPDKR0dPtosv3axpjo6HmMXcC8+nO3cZNxIx" &
            "wrc5Ojs/JrjO+RzLxtdUHefoyOfHpKOM6PrJ87HxECVm6+H727+dI/iLLUE2GnviO6/X0fk4aMP5XUCO1HuBth1Yp/w6/07O9ZJd" &
            "4k5Ium4d98Jtmd852GO/x6bsOpj12XXUJz+dyn7oH0qlje5zXxw1+3j6BpZAPfkk442sWyjPQ/HJ57Yu93xpw9hbw0Gd56Qb1iyW" &
            "8ySkrxA6Uu0V6vfY6UYqW50zXrJ3frfXUbEYm/Gcg0gy59HcddL7cCR752JOHN2akIODvWBs8pDfH1qyc/+Un+QpjsPtZUd+yhOS" &
            "DfNTHK6L59nFlp33xfcC9eLgfDR7iDkyjnKmnUN1UNYh5eu1024Wu5p16+YcW3jdZqkxljoeq8ZYqmqtZVlffYRlVjUmNx0O2etI" &
            "GfXngr4eYwsDG8nljrQwXiMHLwTZp3OXSc4LgPaej855/IOA50wkHS+Mc/ePn9PO+aj3eAnesxTLMesv7clLbr8BMd8KschfwWYL" &
            "tJ9xMn9fO+8r8/6iwH5xJDuBdVLGPvvedCRuYpbXjFuY6SroU6wOtNF8lhN9zsXn3K/Gm583Gdg8vynQLkFyP3a21Sw8dakxSVYh" &
            "L86F5O+5qnbeFLdADg4iyZWb94M2d8FeDuac8ldu7CPt00h29E82W/YJzM8hv1QLOX1NmCP3SPdgc5BfmD2AvhPw/eX9kCwdz0Gq" &
            "QT3ysTcG10rrwRdpCz5el12FetgDH8luL7paPH+P0dkL1HNe4HZsxfF+Jv+quv2B+jpeMdyyJIfUSMnTuc/TMfnxmMDcZrZCyjuh" &
            "2zgJrGXLXuDG3ovZQu7B2PjOeZd7ku+pd6YTyKPBFymUdUO2fuR5hT1EsNf0F5JdysfnDspGeJfV2d0Fnd9MnmqZYabv/D0O47kN" &
            "ZYTWY8/6Eh0v83K7WTzm3vF/UfD+dD10PXX0K/Qp1Vgn+bosy+c/yMCF0Tk6GFhIvinpLWxdpAnepJldh+STGprsPDZlCd0m7DDj" &
            "Irbq39ILrGWPT4U9wLng/eSQ3PW0TaCOfaZ+D9ye/p7Pntw4T4P6NCcXQd+ZnHOhi72FZNf5U665ZNTNoLXu9lviJNJemdkLW9yU" &
            "J95Z7kSKN+ubhveoi5dkhfzW4/F4FGEy0JFFEl0j9oB+W/BGpLiyOQfJntyp0Z1fQvIXZjrPo7N5G7BOl43wQ3bpdeQ7BIJ7qyY9" &
            "cnh8t09xlGOXh9fQcbnvuTbSOdgzx8A11/Exj6Tr8iGYO4fDe9XZOKjz+Yxjz94gV3c+ws8luAbycV+39znlHWjD/GdIfSHfFrwm" &
            "1ncOl/es6x9t1jHGtTefYFIsVnJHl0DidyQe1/noblrpnDaE2xGSeU0eX3P6Jplz+Jxyh9fIj8BSjD3Y47PHpsK6VKhzL1edaTtD" &
            "6lft2IP6HL4muXi9W3x71rjAOYPb7LFP8Fjq0wx3jbMF9YO90TnjpnPmxXkFftp4vCT3n534sZqcHF5D0t8Vs94UcuS+Y9+36ifc" &
            "fz0ej1djjAObXIFQoJwN9eI8SdfRhjziEqgrxHI+oeOtkKcfycUYtHW7CjUnW7cXlKds0w3Q4WvG3Ajm6dxpfQSXuy3lPHcZh5Dk" &
            "XCvpPC7ju437JB7+jKXQd7d3MJZkfCGTbMhV1lefp/xdLp0ecmkwBx/H01fvGddjui33n+S0lQ1lPnzdnEs5pK8ss24N5sDaOR/Y" &
            "W9w7nodiOVIuCbTx2OJwm8L1JHjfnYecAv0J2bsdc01I8Ty34+mPKT5fluVYYRN3RwZnEM6Ju9qei65hhNeWzml3DpZwo3OkXiS7" &
            "ampItp2sm6f1fFvMOEbYQ0Kq0bFXT/4RHsg695p1zpF8aE/Qj4M2bksM3Dh5gyQfdZ3PzN71M+6ko88Muk597bp1Zuw9w/3SvnC7" &
            "BPatrC7yEUnP+rx+1zH3LaT+ed5L+NcEDvoQzqWxjjFejjHm72FPYEE+XJ+aJnmX/BeBlE+K6TltIfnvwV18CPbY8+a8kzmoJ3en" &
            "S0i2Lk/1k482nG/Jkq5wQdZGLayfg7bCXfeF4PzO6zKN9OBImOU98A5NPDO+c/G2PHv8U8/lt8e/65Gj2zspzlYPU74dFHd2k+9y" &
            "T9eEXweUC6yF3PRlP9aT0Rv/lXAJ35UmXM9E0nELiYPwWMle+s5/L7oF7JDikYO5b42Z77nouCmbcdO203V6Bzdlks8gu+4Cocxv" &
            "wo6t2mc6wXWzHghu09mnPtAm+XbzEV5MdXbdoF2HpE+xOGfN5JhhZksdY/GcfarQnw7S0V9wfWdTjf8sbk1yT0g24u/icM3q1c+H" &
            "1gfLsqwk1NxJ2UD60MZtmRSTYRyCsmSfZMS5coEx9qCzpVz5Uu66Du67ZVsNH+fCObwFe32u3EF8He+W/hyIg/v1HPj10OXUyatZ" &
            "Y59TVyFfzmcg12y+lzdxsK6uvjQnZrrayNPXJ9mlev1mm3wSvE7my/kW2L+9/uf6JJskE7pesIfLq3dLrx4gr1mGjUCQzOWz4Uiy" &
            "BNnMbJNubLy9rODntfCm0eWR5ElGsH7OE2if0PG43C+ecyCOLsYedByUzfjdhjzErNa03o7Ol3G7+Ck3993Sz6DcUn4V1nsPZ4eZ" &
            "n3QzG4fbKSdep915hbq31lBwHudIPrRN54TrmLMw8y+sfeLzI2W09XPn7WpLH50lH+dcl2W5p/+Jfgzf5phh4NsVLDgF70A78p2D" &
            "lIfOHZxvYcaVQBuvyXWzPu3pQ+frmHGw7zp2fUxYNv6fOc/JR3mau8yPM/je9N67L21oTz/6d6BN55d4k0zw3jpS7g7+MNhj0bfj" &
            "Sf7Elr7QC3LO/Ar1M789YH1eJ+MnG8cs145LOpdxuI/gXOwXY7l90jkYmzm4nTBOz4t1WZaLevWXsW4D+asBfRTBZJh0CpjstsA4" &
            "deJ2OW0TmA9tOe/AGrZeKfkgXJ7yo8zRLSohfubh+SQe5pxq6WpawsPDY2jO3PgxV4pPbOVCkIe1e660cx3nbp/yZi4uH3ixRiS9" &
            "/DwfB+XMQf3vYhJdHYT0PDqSTKBuFpP1eA/SumzB15R8nX5mJ3h8nUufaqC/dNXUMvDFCsb2HjIXDd7HxOG5kMsh+TrGODIIk+cr" &
            "F9e5H206sFlE4kmNZtF7eTmf5ZvqGmEBOvvkm+wF1pB8E29ZHyRXjp4rN4eDsXg+izl7cPg6EeRjnm7DG1/Kh3Lq6sTNd0kVatZR" &
            "/WM9bsfz2qjX+dOcPinn2UOb+1McBOOmXNKcOS7hBYLbKTb5vKfJhjdK9qADOYUupnQdyKM55QK5Uk9mSHaMmfq1BdbrfuSbvdiQ" &
            "zVpVh8KfdCecUGDzk6w7Z7GcE1v6LSgv5scGCrTx4wwpBrE0v7DnsqT3C0n2RPLjXDKCMt9MlElObvZ51gfCfbqPwVJM+bpdB+mY" &
            "F3005wunEW4+5KpwHSRIn+y8BxpE8tuDxMf6OxnhNt4b5s54jrSekvMhOKtZ8SvY+Zy6c+Drqry4Pxycn4OtnNOLBs/B10LnPnh9" &
            "0d9r8hdQtHvj/4E4uuSSzo8snok62GTOHTPdXdDlxjicC51/gi8I4TKe08dl1Cc7YsuGnATjzmzr1CPq2TPNvZ/M0+0E9p9xyEG9" &
            "5Amsr7PrcmCu5yJxCuniJ5iXy5LOwXhCku/pTYqZfPaslyOtzawuYTTvxmZI9lxrt2G9hOfOGoSZvz+UO/8ErodipN776B7m6xjj" &
            "9mcgrvDmeIO6xjABggl50WnufpTVRi7EwGeGNVngFDsh+VazmGnOmgnK3ZY6l1HXxaJdwpYNud2e+8j1HW+n37PW9OG8wpoxd9dT" &
            "J9leuJ/DY3TnXWwOB2WslbKunx2vyzl3OcG1SzEF5k+/lIvDbbv6EpIdY3BOeEw94LsHvbi2OLfsyO32aQjeJ0dnP8NaVfdecb25" &
            "YFtgwNlwe3Ik0I+yc5DySLouDnvh884ngTFcTqQYfs6cacecHZ1PAmO4vCwO8/E5h5ByTLbcmwR9PIbg/rTlcGjuR8ocKcdkt4UU" &
            "j+hydqR9lPqZeDjvZNXk6/klv05eyDHl6X60cUje6c+F4qbcJJ/pBc+ffdLPHlKtCeyPjt3oQJ3nr6PbLMur/weyerEs3IOmRChj" &
            "EkLS05bzvTjHz/PmInl+zJW9WcJHM2V+3kOXk9fjVbPRuRaMy7lALvflOZHidPC8/K2uy2dce+1Yj7DlV+Em4z6M3XHN8uS+oJ7o" &
            "ahHuyuE+SU90P5/Yil0bNil/l+3JzXsq+1l/93Jv6cXjMVLMLZ4Ecgji93tSepB0e8xz6WJUU4fkzuH91jsp+fmarFV1WVVLRyq5" &
            "H9NINs5DUMb5dxLMj3CdN9KR6qMsxaBNp6sQk3NHytlBbiLlRR7VlLgo5znHlk2C5Nq8ROcnyKfLYQspN86/U/CLlnIi5ZRk3d5O" &
            "SLULLvc15Njy2ZNHIV/25JyaHOfYez3cU6zTbYSuLzpPOmKmYzyBcWccNeml+63LstxfThaJlAHXda2BpyPt/Zw/e3AbX3xuBNmR" &
            "39HxC9xYjLHgc8rE43Pa0d6599gwJ6+Hrz7clnUIKd4MsuWrHIdisR6XO5zPwdxdz9gpJ+rF6dwJyb/CxUE79t99hYF14Wfe9PO6" &
            "ef2QnzWRi2D+zJtcfmQsl6X+COPUJ4d/80r1prwZm4PfHkp+rI92jE0+2jOmx0iD9x7q3YaxPS73Gm05l0z+nkOy5zzB9eLSOjq3" &
            "x1tO70BufwbCm6nDk2AyXoAHSBwJnZzwhujovrMbl0C9Y28egtcspP44GD/1ijaSjfDRmOtm8Lw4iMRFH9kkf6HLK8n2wvtFnlku" &
            "DrdLPFtwHz9SzqGb7WhuEkK3Lkn2BwXm62Cd7IVsHHzYEuzjOUj2KVay69Ctleepo4Y/XNkT7yeHwDmx4CvTyT5xe37dixjZup/i" &
            "6Vinb2HV2HiFy6RmYEDy8Xgukj+5uFic09bBfBPchzEc5KCtz2mzFzN78jI+QZuko9zhNluv3muSxwzsqeBy55T8LrGIjmOrzg7d" &
            "Xuu4JHNdshP26rp4gl9zHITzLJN3II49XILbet+TbcE+xZn5CqzJj5QndPzqj8BcUr5CkrM3SUdOxhzho0T6+Pl6c3NzrScRlSko" &
            "F0zH5fRUuri4uD3Sd4aZDYsUyE2O0XycIh0Xi0iyLaTe8NzhtrT3IezJiT7Ow7fLKa/k6+d8604fl1HuXJTxPEH7jH0gL3m6vCv0" &
            "lPoE8ThfkiVw33ZIfIy1hS2bLf0eLGe8I90L1u6+7AeHwFw47/JJ69Pxu13am8wz+TEPzhNYsw9/p+vxy3qgHGZ5bGGcfhP9asDT" &
            "AxNKML31YVJchHNA7g6d3pupcx93wR4/xnAf9oO2LicSXwVOwu27HuzZRDOezqfCxVJNDOazNdyPOXT8HBX6R73Lu3O3dx25hU7O" &
            "/MiZ8nL9lqwm8r1I+VFX4V5AvYO1dnaE++yB2yWfJBNSbUKqVdiqpfNL8N50w2157vkTKccZr+Tr6QfpkXQGJt4N2Tq6WJ28A3kZ" &
            "t87klO1sQzio93qZhyCfpKuJvMJGEJjHXcH896Cz8x6mXs56dC4Sv8NjeFyPv4QvBCQ5e8RBnc8dnm/SO9xW+czshS2bLT2R7Fk/" &
            "a95aGwfrSvEqXJ/cZ7N4XM9zsMXtGG/xMxCP4/10nhncnzkn/yST3N8oiEv2y6t3XOtFSljnHXkhAM+3fN8We/L0xu0FfbgABPvG" &
            "HPYg5d6hs0s50jZtKvqxr4LbzXwctPuikOLtibXHJnGfA66l7wsO1+vcbzrMl/M6I9/kuxcpBut0JHnXB+pn50Lax5y7nLG25MQI" &
            "PxcQunpqR8+VbxqFnvkxgbX73NFxpPwd6gGH/phi1aRgJjTGqMPhUOO04ZdlqePxWBcXF7dJMJD8uocMC0j+4nA7+s74kr7jSoM2" &
            "Ptc5bSTbqmXg5kF9hYvCQdtCzR0Ul7l43gLX0nk1n8VK8DjL5OubjiR3HnEoH8lkl/JMMslnfWQ/3Mb7yprclr4657XCV4OExy7r" &
            "6bDrlXqP6zl6nh6XdoV/+SB75u5xnCch8XsOs1xmNlsgjwZ9aUNfnyeZQF/+/IRxO8iO/JxLRj3jpmvQbf1bX+vxeHymP+nuBfFr" &
            "YUzG575ovnDcMOcibYYOvuAO5kgkvXjSjdVBn3TUeZcfMdOTm5y+Pm5HbOXjPWFMR5Ixh3PhnIm/IOf+7PZomgspjtuxX12O3iva" &
            "+pw3ZIfnOMu1y6OTu85tHWk+wg2UdkKyYz4C++tHyr0nztlxJ1lhrzich5xpzySbDvKnDTlm8LzTSHB5smHNFfYe/Zjvsiy1jjGe" &
            "H4/HsWdTd6Sj+bxP5xUa6bFkLx6NZOfo5pSnmC4XFN+PhMfohnJPDyDvU1ow5lSTmB06G/rzVWKCy5PdjIfxElK9FeKmc8FfEWne" &
            "9XbWZwdjMi5roz7Z+N5O9nvyShCX5zE75750eK70T6PDlt1MV806C8mv49u73tXkzD1Dnd7Zyl+g3yy+/JxX0Lk4bl/52353/i5W" &
            "kg3cj2rin7CcPsK6loALQDLNJfPC2dgE+dKvA+04aJfiJvstHeW0oZzzZJNuss7pYDzJCOfruHmkb3czq1BDQnrQp7lkkqe1Evb2" &
            "ivtTsjTn3t0Lz4UPgHQj3srXj/QlnIv9cHk6dxvB15o3Dgf9UwyCvZVt6hFtfLiuwroxn3R0kH/LxmUO5uGyCvau93V3/xkYx496" &
            "aC2Tj3xnsg4zXerhGKPW4/F4U1XDydnMCgklwmQnW/chEo8jxTwXs+ZUyE326WaW4PqZndDZci7syUFwm5SXjnvWM/m7jvNknzgT" &
            "KN8z72Qpl2Rf4QKVHdd+z5B/B14bQsexxVUbNhX0iTvltBf0ZTwH63Rs1ZPuKZQneEyPzXWXrcPnKSZj019wO/elv8t9dO+qEyjn" &
            "XJhxJHjvTudjXZblpqqOTHgGJ+ErjI6ja2wnJxJfKOg1G8GbXpMNRTBmnZFvgufK4yyPZD+7uREuH+Ezbc5p3/EKydZlbrfFVSF+" &
            "QsdL3zQ6jI2+zgZ5XE494XYjXFNCuq4k9yOxpfdXtEQXs8K1vrfmVJvkhMv8fJaXwHxS3K43XhP3hOSJz+GctGM86mvHn3txdPnM" &
            "6uNwDvII4BnrGOO6qo5SMpATzRqSQK5zca5/KrxrYDX21fRhC+faK26KP4PnzMU+h8s3juYV1ntPXUv4XYlzctnCjMt7wJH0khGU" &
            "vW0NjLvFxdxmPmndqJ8h6V1GPecC91A1eSf/ZOfoOBmvOxfusgZpzoeIdPQRmP+52KrL0fHLLx0pK/RKc4E+p/PjenFxcXjVn2Md" &
            "Doc3GiXjYU9iHpdluf0TJg4PWmEx/XNYJu7xk02Fxnq8FIsygfJkMwMbyz6Q03nlw1oE2XY+Xq+OjOPcfn60Lyu4vusheYUR3tUI" &
            "lHt/Uqzj6WvhW6CN6lhsr/pwftbi+97hPOJW7rR1XFxc3A7/Ab/HZP6E90Zg3rLphvv5UXX4K1zXER5Lcz/Szrnlw9yWsE6eAwdj" &
            "UC5dB7dPe6PClzHKeu7+kmn4nhK4b+jndgQ5U10pR6KTcQh+D/fc2WfJ1nU9rsfj8VDh/6Iv4S2NDxboieg8ybp5Am1YyMB39FMu" &
            "e8FctzhoT7DxzI/+ndyPbptAnsSZQJu9fg7asn5ixk954mG+OtJXoM5r7EbCwM0kofN1pFic78HMtuOiPOVS4Sbl8JwJ55r16RzM" &
            "cqGM9XAkUM54M/9OLiSd+yT9FuSfHkAO5ub3S8Jr1B5Pw+yP+hnILZuTpHnh2xwqggm5nrK9WPCNLckcHafnTY5k4/MOiSfJEpJd" &
            "ijdbYIfrw8K+MVxOf4H2ks2wpffcuHa1w18QD/NzUMe6qa+q21+w43C/DqkeR9IzTjdon+DyFMv7lTi6mA7uK4fmnX+X0wxdPpw7" &
            "XJdyYp1pOLTXPFd/t5JGNTlSR7+ZvOPT2Hp4JHS8Dq5RtwfGGDfrGOOgn4F0xHC6TV5v/fkRwKwB5+AcfxYtkMNz1DHl6vKkL3ub" &
            "Rx39hr0S6/Is0zkfuRPGjs20xZP0nYw96WR+3IMlfPxXxk8w7mhezLgt/TT3I88TeIPpkGzE3V2YyWcLidmjt7YAACX9SURBVNN1" &
            "rI2j8Dl/B/qM8BFOugHPQE7JdNNON273c5+9SFzU064byZ/1d9c/uZyTcDmv98S/Zx2cc4SPB4fdW5SbrcvVenFxcbWu66FOAfcU" &
            "oYeGnx8Ot38R5RazxPfC83G+lCebxeakkUA/P6evH13uG19gfr5Y1NWOC3qEizeBec9sZ3A/ciRezf2Y7BKSrdfKm4p83PcucF99" &
            "Ju4/s9kLxudc4NrP9q9DPUg3WGIr785vC+x34kk2Lidc7/Xxeup4eT1xuC/BProdOcpylH4LiaOafJKsELNCvS53dDraSSY5+63z" &
            "03pcrZeXlzcXFxdHf+XHJqbGpuFQAn5MyRJdwsIWB/2Z15auQq5eH/NKtVeTJ22Tn0BdF4t9Jn/ykc6R8k2yCr6cCx57locPv0kI" &
            "e26SSSYs4Qe73jf/wal0tO16kbBVs8P1WzES7yzGsFeUrNFttuD23bnD1ynl53PqJPObpIO1d7oZuhoUt8tb6PaG/J2DYP7Jpk55" &
            "0daPhOesPCgb+CSE5916FmzM9rAurxCdGdyL6EaCJ/o22PL3/HSc5VWTBakd8bbgfe1y2upfJ3P5LEby73CXdSK/5s6R6nXbgQvP" &
            "kfySrtBvl0tH0D7VLxt+tMZ8mJuDupRLwY4+jtQT5lOhPsn8mHSO1Fvaca5c/CEgmY8ZyCkoD/pzLnTy1BuBDwCP5z1YsC9UMx8i" &
            "8vVcJE85JFlN1t311HEU+P082ahOH/bOfKynP6T42h9TFJnAgLLVBtHPQDyokAqQjXPRvvBVuLI45HGkWC73eToXn+zJLx03CPkT" &
            "pN/ySfn4PPVXOsnYp3RROFzHPHlBdDnQz7kE+YnP89R8BsaqUIvy6+Tk4LuTBNUteAzJWbdGZ6d6ZbOua/SxCzbm7+fCrBZB3F1u" &
            "ykO2HM7h73A6SC975s+cmYs4PG+BNUi2N7cKffRau97LzkG90OW7lSfr4a9M8D5JzProebi97uev130o/ch8WaouLpbDenNz8+J4" &
            "PN4wgIh4zmSSHxvFIxMWnMPRyavJh0h1dEj6Ll/B4zJW4iPcVxsh8cxqJNhnrsMedBusQp+YG/MUR6dPmOkZL8HzTrm4zvWcO/zC" &
            "dZCT82rq8b6km8BsDT1PxvM+u2xP387B2/Iwpz18yc5l5NvDPdN12OLu5AL3J2157XXxaEdbIsnEseDdHc8/H8eqqpv1cDg8PxwO" &
            "13x13yVLebKpsKG3QP+tuZBySLasQcdk62BDHfRPvMxvxrXnvONIsr2gr28mnxMpn721ciQ9zwn6M2+386PL9eqN79Bo5xB/kjP2" &
            "DG7fxRa21qLCg4UPJPaq01HuR6HrdQJ90zzl20H2icfXMY3EITll1Bf6lvRuk7iFzrc21niGGWeF3Iku5ivbVMd4sVbVi9Pvgrxm" &
            "4AFSQQMfbbBpKcEOsk3HGRft7iIT9tgIWzl1g0iyBOfgBSK9H7lWRJdPQuJKvolzS+Z1zOxmmNm5vLOp8PGe0OXmSP1JcA6/nrrr" &
            "hvYCc2F+ztPphHRdJ6ScHOTtwFz8nPlu8dFfx3Tu6ORbcJ+0ZuTsdJx3MseC34djLmkNZ3y1c9/OYr36scfyZL28vFzWdR1M0p2C" &
            "8+3wiy8VQ85ZYVsXio4cXxQSr3+kxFjM1214vgX6E5R3cx25DgL9ZpjZUpfW3tHVtkc24y1c0H5M5+TmvENnR7nP1RPaJHhuOt/j" &
            "V7DvRjV9nMVgTgmUcx9Qn8C8Op9OXied8zAPgv2R/8zPe9H1JdXu9jN0Np28y1NIfl192qfJR3DfZVkOVeP/XS8uLu6t67pI4QQ6" &
            "Z1PG6cGhj734qtgHfXQkf4rvvj7EIyzhh2oC7Tob1eEjxfOc09Ft9kAxiJRnhTjun3gcrEd1dpwcbpPgefi51ruD2xLe37R+nCs2" &
            "10wyrq3g/KkfOk++yY56yQnJPK7bJd7Ew9ocyrusX5LNOCvUIj8NB9fiHKh+5pHi18meOskF6iQjX5L50eH95V7ZAuM4PI+Ui9Dt" &
            "Uelo5+hiOGfSO2S3rsvVuq7/Yl3X9X415Gle2Ky8INM8JcR5hQ2YbOokTxdKNY3rZEKqN+XPeN74NHc7B3kEyRMfZfRJYNzCGjrI" &
            "qbr9yJ77Oe0JytO3f4SuZs75ooHxR1gzwrk9RiEn56ac8z0Qnzi9FnGS1+tySJfyo43H5ZomP4dzcLwNZn1PcNsy+8TR5cl5Z6se" &
            "Se/Hav4Ao8D1dKScJWcO9K3gJ7CXAutx/uWNb1t9Hu9zrs8f7q9irFfruv7uWlUXp6C3H2NdXFy8RioSnVPOgCwiNcChpLVQArm8" &
            "APfz4b5+dDl1bCbzd1sHY7q8m4/mq8EVfkGpEHdpFphDYB6My97R3uXidjvNde7xyUW51pp+svHBGx3t9py7L/UVeuOypBO6mtUb" &
            "39PkWfAQFBe/6ZXyZSwOwmXJJsUVPL77sq9JprmPQm+Zy0xPHrf13ngvkp4c5yL5MlfHwIubzk5gjqke9mgWX2Av5NOtfYHfpM+X" &
            "ZfnWejgcRlWty7IsTjJrdjd/M8h+pAWZgXGS/9589tgkuB85PDbtaOtYcJNmnzvMOAWtEXm4npL5kfysbUzefSSoD+6vY8chXRpu" &
            "42DejuTnfHu4Ug4O95GdvwJMfuyNIFvXJ/6Odwtet/szXorttnvhtvTr8k/9o+3SXEOKQXuBXN25gz5l/erQ+fiRSD41sXfQTz5H" &
            "+10P2bFnsl+Wz+s6veh5vq7Lx+v19fXV4XBYh70y1t+1IhmL4NzRyYlkRxnnb4MvksvBhvPcZV0O6mc3qtkwHkPD5YU1TBwCc+ts" &
            "tfmk50Xd+c0wq5G96EaC5ykb2m5xOGhLH66Ho4tBGS9q14uXnwRUWAfyFnLwI+2Tr6C4qVb2egauDXNI2KrPIT37NAN5t2I43HbB" &
            "zxTScHh+Xa7J/5zaBPkvp//lNJqPQD9f01cf1emXGC8u1k+r1k/W6+vrFzc3N+Pm5ub2puAEOmexNSlSSD4OcjN5zh30mdnWRi5b" &
            "CzDTf97g1/VpM9CmkL/mxCz+DOSaxeig2Mkn8bGec8AayePcd+FPPomT/MkvIXFR18H3iH+sRc6x42MxgrUlWcrPuZJN2pdpnuyk" &
            "69DFSXm4jLouNkFfzmdItoq5J7b3aG++HZjvrB7FWtf1tY+JycFalmUZY9S31/X4yXo4HF5U1c3R/sLuCD+MJEh6LsidYmwh+Xij" &
            "fLh98usWzuVJP0OyTzKB+Rbsl40b+Ww+q13odJ1cm67C/2/ufASvVT32Xm/5C+xPB+djLzwX2s3mAuM6V7ooWS8HQX/x+jUqzLhk" &
            "6/lpJPsKtXXo/B1beoI1K0/Kkl1NcpJMx9QXIvVqZl9hLXzcFVsxE2irud5RFK5l2bzu9xrHcVnqW+u6fLK+fPny2fF4fFH2g82B" &
            "b2p40k7KxaSt5AlagAo2jCOZH2dgvolPdoTX1C02ZZwLKefufAuMwZoYi3XT3kFdspPNrCfUdfEZz6F94TYpnsNjMwfCc9CxG3ug" &
            "WPTtxgyeu2zdh1yJt+Pw83OuU9rQrybr08kFxu7sUzyuM/Py88RLWao/xenAHB2Jw9co+VGW7Lxm6qT3c811vLy8rIuLi/I3D1t+" &
            "p7yvq5ZvH4+XT9aPP/7406urqydjjOEPDf8djzQ8AHWpGAcXfgu06Qr0ueflIFeFzZQWfA+26kqyCovEkeRJ5jrn5nolO4IbPNmn" &
            "HnnvPE9B5923PrpaUm+7nJhDsquQ18xWYM1pjxEp3xSHtSeb2ugFr1HGdHs/Z10Dn0KszZ/DL+wVB+2IVKPzL+Fbh7JJ56kO1eB8" &
            "e0CeGVKOCeTkGrjcwbkw8E60s3PIZ1mW+ABhjp9jqXW9Xc+b4/Hw+Orq5dX6ySeffHp9ff2xfnAuIn2VV2CCmsvGi/GNN/NNkNxt" &
            "ltPndJ2PQxvFQS7p92wq+fpwXcIIr9Rpy7lkGr4p2E/ykos3EdY8uxmIi/Fkk+IRHS/9nNOP1HX1O38Cc2Yc+vs81VBhP1AueI7k" &
            "EDouYWl+UO5zl6tXWnfmxzw4F5gL7Zgv7Sv0MkF1sFedvcNtk79zpHVxnfTJ1+E6+bAPHMwjcdOnA21m+bqd9pBfR5eXl3X//v0a" &
            "Y9TV1VUdDoda8KmQ5/JqvlbVUlXL9bpePHn27NnN+pWvfKUOh8Ozm5ubsa7r7QPk5ubmjcI4BG8qh2OmE9iMZCcb2jq8ueJIXJR1" &
            "OXLOhVOcNAjKyCPQn37EVlzvCfN30JdzgXHIqXPaCZL5GlGnc78pSsd6fSSO5C8o19SbtCaMRbsE1cm8EpeDvLJjXpLRXvKym4m/" &
            "A/T+dzls5TjDHp+Uc0Ky45qxp8NehNDOQR8fKS79k01hb+noeQjON9NtQflqjX1+PB7r4uKi3nnnnbq4uKibm5u6vr5+zdeh+ef+" &
            "o25uDleHw/Hlo0fvjvWb3/zm8ydPnnx8fX398urqqhZ7a+MNFFk3PCDlbMYeuM+Y3EBcxlwctHVZknOekDaBMMvlbeC8vhGZh+az" &
            "PFhfZ+dINsxDsf1dFOEy6qljfpKdM8jtsg6prx1ot8XPvFy+B119e6Bcvb6l+fblDHtsHLOcOe/Q2fl+dzBmGsnvbZCuhT9o6FOG" &
            "YfdOyd5555165513qqrq+vq6xukB0/VgnD6Rurm5qZubm1rX9fHFxcXH19fXN+tf/st/+ZMnT57848Ph8Pv6ucezZ8/e+B/ns0az" &
            "SUqatr5gM7D5XGzJ0pF6yjquc3gSyMubZ7fBBfawkyWQ228KCcyB9lt+XqvH7NbM7Ygkd5462eijmS6HvWOGmZ49SXzMu8DJ863R" &
            "QTraDHuF7Wu5JVsmN49qck06zR0up871PncZ+7mFPfaMwRz2wOOwt1vgOlC+B9531rMsy+3PsC8uLm7fDBwOh3r06FG999579eDB" &
            "gzocDrcPEOf1cw29S7m8vBzH4/FfPnv27J///M///Iu1qp5/8sknv3Fzc/MvltPnZA8ePKgjvtbr5HqiOZbJ99cJ8nXwhroPz7v5" &
            "uTiXJ+WXsIfLsbUhxcUNKFCe4nPOeOToQBvy+pzn3m+HYovb18NHp0uce8BaHCP8XKvDzMZz3Iu99lxzP/K8yzHlx95KRiSZ4Bzk" &
            "45FgXcmu4+Cadf4zdL2inPMOjM8cz0HqY+FPBenF1/379+uDDz6oR48e1eFwqJcvX9bNze1/8og8mo/Tu5Dj8XhzOBz+z4uLi9+u" &
            "quNaVfXDP/zDv/Pxxx//8vPnzz+7urq6fQfir6KdfEx++u9zvmo8t0mpED+fzR1vu4E6OI9iaDNs1brXzrHX/m1qlB85eK658nFZ" &
            "6osw403zChxCshXcJ/l7L3nuYL5718315JBMIJ/3MV1jwlaNW2DMFMNz0XEmS5B+ZtfJHeTZ4hS29Al711ngXtKYxb6rrppeOPQl" &
            "qMPhUIfDoR48eFAffvhhvfvuu3Vzc1NPnjyp58+f3/r6fdoh/bIs9fz583r58uWTe/fu/W/vvvvut6uq1qqqv/AX/sLvPnv27O++" &
            "ePHiNy8uLg767Gyc3vZ4EAXSufQKloJTLt054GZPR56fi7RZZjUU9L6J0rdnpKNsLzwG0eVXVldno/z5giHV7XOuCX38QkqYcTPO" &
            "jKfM3m3cxzloQ1tBnKzL4T6djeB8QspF4HoksMYKa5Hi7gH93D/JiE6XeIi0Vp1tNb2njHqBa9DNqXObDsvGl2tmvltI9YhbcY/H" &
            "Y73zzjv1Pd/zPfXee+/Vsiz19OnT+vTTT+vq6urW5ohv7jHX4+njsHVdv/n06dN/+tf+2l97XHqAVNX18+fPf/3ly5dff/z48Sf6" &
            "WpceDgrAhwcDCN5wgcXubZwvmmLpfC/HDOLf2iQVakjoeDz3hJnO4XXPfLoaEpTbOT4dZjk5ZJdi+hoL7OcMs/VM8sSr/ez7upr6" &
            "ZuucuB1u675+ZMwZJ20JXrdvA+bb8dKG53uR+kweymg3i8l9wnlCZ7MVj/mdg3G633qOziHZ8fSx1Ze+9KV6//33a1mWevz4cT1+" &
            "/LhevHjxxl4Qr3Ms9iC6urp6fn19/Q8Ph8Nv1OlX01/9HntV/b2/9/eu/uyf/bMPHj169McvLy//ldW+5yeSrokVCvCRbGi3Bdml" &
            "hjsH89A8xUiyPfAYns8slqPrifun3iRexvc+6Jw8krFXbl+hl27n/ilX5eUcrhM8loZzJ9vZnHlWs+8kTzaMTX9Bec7sUo5J3oE1" &
            "VZPr1tzzJBTDh+RbIL/HUX90FPbwCm7ruTlSzuxRV3+Xl/MxJnUa3gthFpO8AmtJ8Jjj9P9kdMuW7sGDB/XBBx/Ul770pVrXtZ48" &
            "eVLf+ta36unTp7cPoLJ89DVf8p6+eVXLsvzO1dXVf/f06dN/+M/+2T87lD9Aqur4J//kn/zsgw8++PK9e/f++OXl5btSeHOU3Bbc" &
            "hk30prvsHHT+5JktFhvIc2LgIZo2jKOTez/vgi1fzyvV7Uhc2jw+T2DPZ/2gzl6fvBHP5TzO6kl7wnOc+e4F80x9Yiz6vE0ezs2+" &
            "dXFlN4s703Xg+iTZgodsisP+VFi7rk7yMRfqBfdn31yvc3LRj3WnmoQUj5jpF/udjmEPD/V5Xdd6991364MPPqjv/u7vvn14/P7v" &
            "/349ffr09ptXugZTb29ubl6LU1WHMcbXnz179jf+9t/+2/9S9q/9PYm/+Bf/4u9+9tlnf/dwOPzG4XC41g/SdVTC6a2PBpFkXIh0" &
            "LqQF4eIlP4HxZ7YzeB4cCZR7XOo4J6hnDcpDcs5pfw641g7Ou3i0c5lzu53rHbQjkp75FHjTucvYV/InJNuUR4cuhueXYjiWt3yh" &
            "IognxeJcGBtfAKhmrRIG9nL380XBeffwz7gq9NzBPe88tCX2xkzQDf7y8tJv8vXgwYP6Q3/oD9WHH35Y77//flVVffLJJ/Wtb32r" &
            "Hj9+XIfDodbTH1H0fa2jeiEb6Y7H4+8cDoe/873f+73/x20SeAdSVVV/9I/+0WcfffTR9y/L8seXZfkuybUZdL6F1IDUZKJr2BYY" &
            "zzcF43FOX88hySpwfFFgHCLVtCVbmhtJV9vAzWKEV18OzolO73GV55at65l38vU+0J5zgXLOCebN+V54nHROTq1R1xPp7pqPg2t1" &
            "Ljof9tbXy+E9oE562ZAzoYtDSM8+ex5pfbZ47wLFXOxPO2lcXl7We++9V1/+8pfr/fffr/v379f19XV9+umn9e1vf7uePXtWw/51" &
            "Mt99OLdqHaff/xhjPL24uPilR48e/Y2/+lf/6u94Tm88QH7lV37l6Y/+6I8+/vDDD//wuq4/vCzLpZqh4A4u3GheeciPTeZic+5I" &
            "OudI8fYicd8FKRci6bZk7JtDduyxH0fzdj7FFbo1JM6VV6jNB3OibQfpOn+XdzadLOkJz00X6CzfPWvSxaWctXd6ouut85CrA7m2" &
            "Yna89NvLW6Gn6TzJyuLM+Gc61kNb6mtH/CTzHlxcXNSjR4/qgw8+qPfff78ePXpUVVVPnz6tjz/+uD799NPX/lSJoHV1/uX0OyN6" &
            "l3L6ecjLi4uL//Hly5f/zePHj3/1G9/4xmu/Yf7GA6Sq6utf//rv/8RP/ERdXl7+m1X15ULzFZgfbzCpGdhcIflK5jr5d/E8X8oc" &
            "9OXcZUlX1pOyfFIst9sC7dKm8vOZ/TlI3I4uD5dpUOZzgXlK162brzuR+uv9oU7ockuyWXzHLGfPhbo9OMd2hi2evX3xo+pZ8KKg" &
            "8/HzrZ5RTp1zetzE62Asl3VgTak/jq7erTgCa1mWpR4+fFjvvvtuvfvuu/Xw4cOqqnr+/Hl99tln9dlnn9WTJ09uv6o78E1Zz3kJ" &
            "H1Gupx+eL8vyz4/H41/65V/+5V/4lV/5lRe3BCfEB0hV3fzIj/zIt99///2PquqH1nV9NMZYFOhg//LWF48L4An60ZEWbwZydX4p" &
            "Vpery3WkfxdHIAf9Z1A+s5qok0+yPReKTe7a2OQpdsrLeR3iZm0JXQ4Vep7mzGkLzD/Fd3ninvXubZHivQ0Sn8tYB/vpPWbdiTv1" &
            "zX0W24+68YmXcR2M7SD/DJ6fYnY+nt8M7Fnt8BFU13r6gfmDBw/qnXfeqQcPHlSd/qbV8+fP69mzZ/X48eN6+fLl7a9h6GfYziP4" &
            "uo3Tb5ufzsfNzc3vHo/Hn3n69Olf/9Vf/dVPbp0M3QOk/v7f//tPfuzHfuzje/fuffjgwYM/sq7rw7KG6ofqSmzYd5PL/iy8wObx" &
            "nBvDn5bi5JEc0nEoN2+U88jff87jeubmIGcFe49DuH+qgbZ8FZGOnrfzeE6M53O3I5LO+VlLsuO5o/Otk48+GhI623PRrdnAjYM2" &
            "7Jn3m0fHVt57fBMHrxsfAv08d80ZI3E4D29QtBWY02J7z308F8ooF3xvpDwEfhTP2A7lxh5JtwfO4bEYk31x+bqudXl5eTv0EdPN" &
            "zU1dXV3dPjyeP3/+2l9S93uy9zQNreHhcKibm5vfW5blf7i+vv4rP/uzP/svbpMB2gdIVdXXvva1b/3pP/2nP17X9XsvLy//1WVZ" &
            "7qlpCug3XR0l646eMJN3fZ2Kdn6dezyXcVEEtxGc22Uaid/BB04673i4SbbA2l2uIzeLbxj6UMZNpnPKt3Imr+c3JheP50Sd4L2s" &
            "UIfHcr3intvzwkWno9fhduyd4Hl6TszX/Tr7zm8v5M/8E8TP2jwuc9rbZ8/Bz2d+1KX6mYfQnfucPo700NEx5TEDa2VMr0HvOGRz" &
            "PP2M4urqqq6ururFixf14sWLurq6un1wiINrM8tVusOr/wvy6eFw+Jnr6+u/8jM/8zPfKPw/W8f0AVJVh6997Wvf/LEf+7H/5+HD" &
            "h1++vLz86Hg8PtRHWHqACIu9mvBEeS6/NKRnA6h3Ppd5bPcryzfJPUfKurwE+mqufugoXcrf5dxQytX9BM9FsVzH3L0Gr9Hz0JxH" &
            "1zsUkz46p58ujGSTcpBPknPe2QjU+54lmEs6934mX+rIMZqbp9sluN57cw68dq4h7bgnWZ/8dLNLPkRaU8+ny8XRXZMV+ul66iRj" &
            "PjrvdG4juK33mDns0Zflrr8OcnNz89o5eyCONHc5bU4Pq2/f3Nz83MuXL/+rv/k3/+Y/rao3bzqGrQdIVdXh61//+u/8+I//+Lcu" &
            "Li6+fO/eva8cj8d76R9O6Sanc+p0rFPifmPk8CI1V+Pcjr+jkmJy7vY6Kie3pYz2AueSJb80PB9yeJ3SO58fky9jUzbTUb4F9/G5" &
            "+/uNxe3pw7nHcHQc1Kc5Y6RBfeoz5xzUL/jvcAmMPZPPeBzJnr60SbaK7aPCg4b2DslSH/bMGTvB9b5u9OUxyejjoNzP/VqiLWXk" &
            "dvkR90fdDxiX1y65ugdzvfpG12fX19c///z58//6137t1/7xZ599lg0N85cIhh/4gR+4/+f//J//Mx999NF/+V3f9V3/wbIsX1rX" &
            "ddEvpPCzaW0mH4V/dJIKlU6cfrNR83yTSkeZ4DrncEjvzU1HNj7FpF8nK6vVh/dKMs+ZMT0v1zGmc6aj1+5yngvMo8xOuXpdA2uq" &
            "eJ5HisnYab7lO+z77+5TTc8c5E82xLALtbNPN85ZX9xW1wDlXMMOnY9zus0I72yTXeG6l60fFYN7xDFOP8x1v+7cZYwpdDFYE/Xp" &
            "KLhvTWKkc4HxNe/6R74RXpyRi/a+xvX5z43G8Xj89uFw+PnHjx//pZ/7uZ/7R1vvPIQ37wJz3P+pn/qpf/srX/nKf/7w4cP/+P79" &
            "+3/4/v37S3rb6kfXS8bGsAl+wbudQ/p0MRIeJ9mmhhduhg6vwRfO63IwrvrBnLo83I790zHJEzzeON1ck33HxRwF8uroD33V7bW7" &
            "vUN62qV94f6JUzFp43rKKtgxlst19P1AuIwvZByyS/vPc2CMbm0E2icZYwnsX8qBNqmv7ut23VwyHelbYX0d8vF8dL6Vnx8TP+0o" &
            "nyHF7+pIcUbz4pLXm2Sae31VdRxj/F/X19c/9/Lly//+Z3/2Z/9RVb3+3wQneDPTbdz76Z/+6R96//33/9N33nnnP3v06NGP3Lt3" &
            "70GdfqklvRPRuw7NKzRYzfDC9RBRwfShjo33pqcF2IJy0nlZ/r7wXlMXhzWwD5SzFsXjTYcxfS5w0/Doa0ZfxqtJjoW37Ixb+AW7" &
            "rrdeB/kEt/UXEL4urCXlSxshra/g/Byyn/kQLiM3ZdRpzngJXI9uLjBXxkj5cA39xYnH85x17nOH8/MopJ4LzE/7hfcXB/emwPqo" &
            "53yWU8pZMuXl8JiuS3LnVc1ln3qc5i+Px+P/cnNz89efP3/+c0+ePPmdX/qlX/r8P0ztQK5uH9756Z/+6X//ww8//C8ePXr074wx" &
            "vvfhw4cX4/T2c13XOhwOde/evRpj1L179+pw+g1HFaCi9N1jf4CUvQtRY9jU1DDXC95cLpggP/rT1/07W+q0gMzXbWjPPHXj9Xw4" &
            "7/gclI/JOxDPW/MtdDd8h+rzGhz+0NIPC6uJ33FwLl+un/ct9V1yP7rfsAcYublmgmpnjg71wPtJ+z25S8/8WAvnfCGY4Nx+9LUl" &
            "aCt4/KRPdbgN10hgTr63HMxZfdCQjfi55uKWjOvB/jof50nH3PiA81xubm7qwYMHt79EqD9rspz+dtbhcBiHw+HT4/H4Pz158uS/" &
            "/Vt/62/9QlW9vA1wBt7ccWfgx3/8x+//5E/+5L/xe7/3e//h93//9/8nDx48+LcuLy+/tNjfarm8vKzj6e/S++J5w7VZ9QDxRZCt" &
            "27GZFS5KNt99fGEF2bidywXPiUh5+dFvxh28bp+zFxzUaS4/5+DmT+j0rN/r8dgC10Uyro/D5Uf7gkHKR3B94mVflZPb0iadu73O" &
            "fV3dLtXIHATKNfwB4qCMuQu0E7gGhbji4pG2HZyrbB9I5yB3NX0u8BCpB+7vtZGDMuYvG9lR5z6yTUfZUcY5dYLH5P3EefmiS8fr" &
            "6+sXh8Phn19eXv7d4/H4M//kn/yTX/vN3/zNJ7ckZyJneSa++tWvfvcf+2N/7E989NFH/9H777//1cvLyx+9uLj4vmVZ7t27d6/q" &
            "9Fcix+mPc/mDQA8NX5gZ1AjaaZ4aSl7F8+bKhq9QpHPIlvIELTIXewbPzedeu+cr3sXeMXhfVYP7q07vgccUP/Og3vvgeSim27rO" &
            "3/XoKCi3FENgLi6n7bCHA2M6f+rFzI/+zuFIMtkLtPEHJ2PqXHPl6HmKj3EExvMaiMTv9qlXict9dXRf2iYer4s+rnc7XiPVfDQr" &
            "MKZiebyUm8Pj+ce2tE81ELTxuMpNdR5P/znQ/odHHY/HWtf1s8Ph8NsXFxe//vTp01988eLF137hF37hX57z846E7ezPwJ/7c3/u" &
            "ey4vL3/o+77v+/69y8vLf/fhw4c/eO/evY/GGO9dXFzcv7y8fM3eF9cXlgslcBFdnhaUMl9UH8lWYDzP2WWFzSuZFtV99tbk5wv+" &
            "gqbnoYthPf12ql8c8j1toqrmFXPqB9eiA/shHq/X46WaPWc/r2Zt6M96ElJOPpeN5+e1MIdC7Sm/Lh9yuZ3HSnGZt+csufhSDNYg" &
            "GeOIL/G7LWslj6PzZ588H+a0Bc+HPC5zJJkwJp9+zHzqlINu5C6XznvaoavZa9GvM9QpxumTn+dV9ftV9a2rq6t/+PLly7+zruv/" &
            "+o1vfOO3f+u3fusZ+e6CnNlb4id+4ic++MpXvvKH33vvvY/u3bv3gw8fPvxTl5eXf+ry8vJfu7y8fL+qLlX4wKth/+xVFycb7Buu" &
            "2ywCZQNf6XTQL0H56ubqvMzL62MegueR/AVuQufkDXcG8pbVqhhek+B5Jv9ZDoypuer1vrBHHrfCw6XMx3vHvorTezjLWxysmw9L" &
            "7oOarCOPngvhMubJHCr0yeG1qG7yp3PZz7grxCa/y9ORfRZotwXn09Fz0TGtuezoK53eMbuNdOkoeO+J9FASfL0099i+7/Q7Icuy" &
            "3Kzr+vzm5ubb67r+70+fPv2fq+o3Hj58+Mknn3zyW7/4i7/421V11g/Jt/BmVV887v3UT/3Ul+7fv/+vL8vyZ955550/cXFx8UcO" &
            "h8OXl2X54OLi4ruWZblfVZeFdyC6UTt8gX1h/NW3FlrwxXV/yrigiWeEV/5uRy4hXfSCby6vqXBRaMOU8TnvsIej16ejc/kFoZ6z" &
            "p12+AmPr6LHYByGtQ1prPjj12a7m6cgeeN9c7vtFNsnO4XnqKFuP7w9j+SR+5q5cxCMZOXyIT0fJFUP5OE8C+bx+yV2XbJzDfXye" &
            "cnYeh3OQT/CafV4h5rA+co3kq55JPk695HoLbpdyFB8hHvdhH5TvSXe0cTgej9djjKfH4/HbVfV7x+Pxm8fj8f++ubn5xvF4/PXH" &
            "jx9/85NPPnny67/+66Mmf47kbZBX7TuD5atf/ep3/9AP/dCXPvjggy/d3Nx8z7quP3D//v0fvLi4+OGq+oF1XT9cluVLy7J817qu" &
            "98YY6xhjfdW7pS4uLhbfdHajWMYYy/HVb2gutui39Z0WaZHvSTa4cWxBx8l+2OYYhRuPjeWU53Ly8/DtBuOGUQ6C1SL58ZT3GK82" &
            "1+35GGNcXFyoT0u9+gWhW7vj8Tiqaqyv/tvYOsa41VcVf59nqarleDyuY4x1WZa1qhblkmqR7NSbY1Ud1nU9rOt6VLtlp/4Y1+26" &
            "abMvpzVf13U5+S3+G7ieg/zLbuCni3ac9kcNvPvUOh77X8zTQtwKT/Wp37Usy8CajVPs5RR/ORwOi+KIAzcv5T60j9Z1XZZlWW0d" &
            "2r9GULaPFvs52HK6RvStSMn9Rujw+nXD8764TmNZlsPp351qIy5aZ/XVOMZpD92ur47M5bRut+dW9+cL9Oo/quppqDXRXw0fltNR" &
            "fdV9ol7VsS7LcqH+L8uyrOs6Tr0Xx23dmp/6OpZlua3zdH3d2no9qu/VNnztRaz25mv9WNd1jDEOh8Ph+fF4fDLG+KyqPq2qJ+u6" &
            "PhljPD/9varfG2P89ul/lX/8+PHjx5999tmTR48ePfsH/+Af3OlbVefi/wOs+oodv7EofQAAAABJRU5ErkJggg=="

        Dim bytes() As Byte = Convert.FromBase64String(base64)
        Using ms As New MemoryStream(bytes)
            Return Image.FromStream(ms)
        End Using
    End Function

    Private Function ScaleBitmapTwoStage(source As Bitmap,
                                         targetSize As Size) As Bitmap

        Dim current As Bitmap = New Bitmap(source)

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
