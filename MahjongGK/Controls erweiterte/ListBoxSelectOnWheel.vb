'───────────────────────────────────────────────────────────────────────────────
' ListBox, die bei Mausrad die SELEKTION verschiebt (statt nur zu scrollen)
' Option Strict-kompatibel
'───────────────────────────────────────────────────────────────────────────────
Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

Public NotInheritable Class ListBoxSelectOnWheel
    Inherits ListBox

    ' Anzahl Einträge pro „Rast“ des Mausrads
    Public Property StepsPerWheel As Integer = 1

    ' Anzahl Einträge pro „Rast“ bei gedrückter Shift-Taste
    Public Property AccelShiftStep As Integer = 5

    ' Wenn True, läuft die Auswahl am Ende/Anfang um
    Public Property WrapSelection As Boolean = False

    Protected Overrides Sub OnMouseWheel(ByVal e As MouseEventArgs)
        ' Basis nicht aufrufen, damit nicht zusätzlich gescrollt wird
        If Me.Items Is Nothing OrElse Me.Items.Count = 0 Then Exit Sub

        ' Ein „Klick“ des Rads = Delta 120
        Dim notches As Integer = 0
        If e IsNot Nothing Then
            notches = e.Delta \ 120
        End If
        If notches = 0 Then Exit Sub

        ' Richtung: >0 = Rad nach vorne/oben -> Auswahl nach oben (Index--)
        Dim stepBase As Integer = If((Control.ModifierKeys And Keys.Shift) = Keys.Shift, AccelShiftStep, StepsPerWheel)
        If stepBase <= 0 Then stepBase = 1

        Dim deltaSteps As Integer = Math.Abs(notches) * stepBase
        Dim direction As Integer = If(notches > 0, -1, 1)

        Dim current As Integer = Me.SelectedIndex
        If current < 0 Then current = 0

        Dim target As Integer = current + (direction * deltaSteps)

        If WrapSelection Then
            ' modulo-basiertes Wrapping
            Dim count As Integer = Me.Items.Count
            target = ((target Mod count) + count) Mod count
        Else
            ' clamp
            target = Math.Max(0, Math.Min(Me.Items.Count - 1, target))
        End If

        If target <> Me.SelectedIndex Then
            Me.SelectedIndex = target

            ' Sichtbar halten (sanft nachführen)
            If Me.ItemHeight > 0 Then
                Dim top As Integer = Math.Max(0, target - 2)
                If top <> Me.TopIndex Then
                    Me.TopIndex = top
                End If
            End If
        End If

        ' Basis NICHT aufrufen, um das Standard-Scrollen zu unterbinden
        ' MyBase.OnMouseWheel(e)
    End Sub
End Class
