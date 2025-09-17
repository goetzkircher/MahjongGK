Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

#Disable Warning IDE0079
#Disable Warning IDE1006


Public Class MenuStripEx
    Inherits MenuStrip

    ' --- Konstruktoren ---
    Public Sub New()
        MyBase.New()
    End Sub

    ' --- Click-Through-Logik ---
    Private Const WM_MOUSEACTIVATE As Integer = &H21
    Private Shared ReadOnly MA_ACTIVATE As New IntPtr(1) ' Aktivieren + Klick durchlassen

    Protected Overrides Sub WndProc(ByRef m As Message)
        If m.Msg = WM_MOUSEACTIVATE Then
            ' Fenster aktivieren, Klick nicht verschlucken
            m.Result = MA_ACTIVATE
            Return
        End If
        MyBase.WndProc(m)
    End Sub
End Class

