Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

Public Class MenuStripEx
    Inherits MenuStrip

    ' --- Konstruktoren ---
    Public Sub New()
        MyBase.New()
    End Sub

    ' --- Click-Through-Logik ---
    Private Const WM_MOUSEACTIVATE As Integer = &H21
    Private Shared ReadOnly MA_ACTIVATE As IntPtr = New IntPtr(1) ' Aktivieren + Klick durchlassen

    Protected Overrides Sub WndProc(ByRef m As Message)
        If m.Msg = WM_MOUSEACTIVATE Then
            ' Fenster aktivieren, Klick nicht verschlucken
            m.Result = MA_ACTIVATE
            Return
        End If
        MyBase.WndProc(m)
    End Sub
End Class

