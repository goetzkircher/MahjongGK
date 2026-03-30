Module BitmaskConst

    Public Const DIV_DIG1 As Integer = 1                 ' 10^0
    Public Const DIV_DIG2 As Integer = 10                ' 10^1
    Public Const DIV_DIG3 As Integer = 100               ' 10^2
    Public Const DIV_DIG4 As Integer = 1000              ' 10^3
    Public Const DIV_DIG5 As Integer = 10000             ' 10^4
    Public Const DIV_DIG6 As Integer = 100000            ' 10^5
    Public Const DIV_DIG7 As Integer = 1000000            ' 10^6
    Public Const DIV_DIG8 As Integer = 10000000           ' 10^7
    Public Const DIV_DIG9 As Integer = 100000000          ' 10^8
    Public Const DIV_DIG10 As Long = 1000000000L        ' 10^9
    Public Const DIV_DIG11 As Long = 10000000000L       ' 10^10
    Public Const DIV_DIG12 As Long = 100000000000L      ' 10^11
    Public Const DIV_DIG13 As Long = 1000000000000L     ' 10^12
    Public Const DIV_DIG14 As Long = 10000000000000L    ' 10^13
    Public Const DIV_DIG17 As Long = 100000000000000000L    ' 10^17
    Public Const DIV_DIG18 As Long = 1000000000000000000L   ' 10^18

    ' Konstanten für einzelne Bits (Bitmasken)
    Public Const BIT0 As Integer = 1       ' Einerstelle Bit 0
    Public Const BIT1 As Integer = 2       ' Einerstelle Bit 1
    Public Const BIT2 As Integer = 4       ' Einerstelle Bit 2
    Public Const BIT3 As Integer = 8       ' Einerstelle Bit 3

    Public Const BIT4 As Integer = 16      ' Zehnerstelle Bit 0
    Public Const BIT5 As Integer = 32      ' Zehnerstelle Bit 1
    Public Const BIT6 As Integer = 64      ' Zehnerstelle Bit 2
    Public Const BIT7 As Integer = 128     ' Zehnerstelle Bit 3

    'Flags:
    Public Const FLAG_XOffset As Integer = BIT0
    Public Const FLAG_YOffset As Integer = BIT1
    Public Const FLAG_ToggleFlag As Integer = BIT2

    Public Const FLAG_IsRemoved As Integer = BIT3 ' = 8
    Public Const FLAG_Frei4 As Integer = BIT4
    Public Const FLAG_Frei5 As Integer = BIT5
    Public Const FLAG_Frei6 As Integer = BIT6
    Public Const FLAG_Frei7 As Integer = BIT7

    Public Const FB_FLAG_MASK As Integer = &HF
    Public Const FB_INDEX_SHIFT As Integer = 4
    Public Const FB_INDEX_FACTOR As Integer = 1 << FB_INDEX_SHIFT

End Module
