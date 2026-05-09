Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On
Imports System.Drawing
Imports MahjongGK.Contracts

#Disable Warning IDE0079
#Disable Warning IDE1006
#Disable Warning IDE0031
#Disable Warning IDE0017

Public Module GlobalHelper
    '
    ''' <summary>
    ''' Berechnet die Steinabmessungen aus dem Breiten/Höhenverhältniss.
    ''' Größer 0: Hochformat, Kleiner 0: Querformat.
    ''' </summary>
    ''' <param name="ratio"></param>
    ''' <param name="aktW"></param>
    ''' <param name="aktH"></param>
    ''' <returns></returns>
    Public Function SteinSizeFromWidthOrHeight(ratio As Double, Optional aktW As Integer = -1, Optional aktH As Integer = -1) As Size

        Dim baseW As Double, baseH As Double

        If ratio < 0 Then
            baseH = 10000 'große Zahl nehmen, damit BaseW immer > 100 wird
            baseW = baseH / ratio
        Else
            baseW = 10000
            baseH = baseW * ratio
        End If

        Return SteinSizeFromWidthOrHeight(baseW, baseH, aktW, aktH)

    End Function
    Public Function SteinSizeFromWidthOrHeight(baseW As Double, baseH As Double, Optional aktW As Integer = -1, Optional aktH As Integer = -1) As Size

        If aktW = -1 AndAlso aktH = -1 Then
            Throw New Exception("Programmierfehler: SteinSizeFromWidthOrHeight: Übergabe entweder aktW oder aktH. Nichts angegeben")
        ElseIf aktW <> -1 AndAlso aktH <> -1 Then
            Throw New Exception("Programmierfehler: SteinSizeFromWidthOrHeight: Übergabe entweder aktW oder aktH. Beide angegeben")
        End If

        If baseW < 100 OrElse baseH < 100 Then
            Throw New Exception("Programmierfehler: SteinSizeFromWidthOrHeight: Mindestmaß für BaseW und BaseH = 100.")
        End If

        Dim ratio As Double = baseH / baseW

        ' --- 4) Zweite Seite proportional berechnen -------------------------------------
        Dim targetW As Double
        Dim targetH As Double

        If aktW > 0 Then
            targetW = aktW
            targetH = aktW * ratio
        Else
            targetW = aktH / ratio
            targetH = aktH
        End If

        ' --- 5) Auf nächste gerade Integerzahl ABRUNDEN ---------------------------------
        Dim iW As Integer = FloorToEvenInt(targetW)
        Dim iH As Integer = FloorToEvenInt(targetH)

        ' --- 6) Mindestmaß-Prüfung -------------------------------------------------------
        ' Wenn eine Seite unter das Mindestmaß fällt, beide auf Mindestmaß setzen.
        If iW < MJ_GRAFIK_STEIN_MIN_WIDTH_OR_HEIGHT OrElse iH < MJ_GRAFIK_STEIN_MIN_WIDTH_OR_HEIGHT Then
            iW = MJ_GRAFIK_STEIN_MIN_WIDTH_OR_HEIGHT
            iH = MJ_GRAFIK_STEIN_MIN_WIDTH_OR_HEIGHT
        End If

        Return New Size(iW, iH)

    End Function

    ''' <summary>
    ''' Rundet einen Double-Wert ab und liefert die nächstkleinere gerade Integerzahl.
    ''' Beispiel: 101,9 -> 100; 100,0 -> 100; 2,0 -> 2; 1,2 -> 0.
    ''' </summary>
    Private Function FloorToEvenInt(value As Double) As Integer
        Dim n As Integer = CInt(Math.Floor(value))
        If (n And 1) <> 0 Then
            n -= 1 ' auf die nächstkleinere gerade Zahl
        End If
        Return n
    End Function
End Module
