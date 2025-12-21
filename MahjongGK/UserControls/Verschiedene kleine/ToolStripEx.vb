' ToolStripEx.vb
Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

Public Class ToolStripEx
    Inherits ToolStrip

    ' Standard-Konstruktor (Designer & Code brauchen den)
    Public Sub New()
        MyBase.New()
    End Sub

    ' Falls du auch Konstruktoren mit Items brauchst, kannst du die ebenfalls durchreichen:
    Public Sub New(ParamArray items As ToolStripItem())
        MyBase.New(items)
    End Sub

    ' --- Kern der Klasse ---
    ' Click-through erlauben: erster Klick aktiviert + löst gleich das Event aus.
    Private Const WM_MOUSEACTIVATE As Integer = &H21
    Private Shared ReadOnly MA_ACTIVATE As IntPtr = New IntPtr(1) ' = MA_ACTIVATE

    <DebuggerStepThrough>
    Protected Overrides Sub WndProc(ByRef m As Message)
        If m.Msg = WM_MOUSEACTIVATE Then
            ' Aktivieren, aber Klick nicht verschlucken
            m.Result = MA_ACTIVATE
            Return
        End If
        MyBase.WndProc(m)
    End Sub
End Class

