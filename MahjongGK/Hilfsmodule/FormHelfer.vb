Imports System.Reflection

Namespace MjMix
    Public Module FormHelfer
        '
        ''' <summary>
        ''' Setzt in der angegebenen Form alle TabStops auf False (rekursiv).
        ''' </summary>
        ''' <param name="frm">Die Form, deren Controls bearbeitet werden.</param>
        Public Sub DisableAllTabStops(frm As Form)
            SetTabStopRecursive(frm.Controls)
        End Sub
        '
        ''' <summary>
        ''' Setzt in dem angegebenen UserControl alle TabStops auf False (rekursiv).
        ''' </summary>
        Public Sub DisableAllTabStops(uc As UserControl)
            SetTabStopRecursive(uc.Controls)
        End Sub
        '
        ''' <summary>
        ''' Hilfsroutine: durchläuft alle Controls und setzt TabStop auf False.
        ''' </summary>
        ''' <param name="controls">Controls-Sammlung</param>
        Private Sub SetTabStopRecursive(controls As Control.ControlCollection)
            For Each ctrl As Control In controls
                ' Viele Controls besitzen die Eigenschaft TabStop
                Dim prop As PropertyInfo = ctrl.GetType().GetProperty("TabStop")
                If prop IsNot Nothing AndAlso prop.PropertyType Is GetType(Boolean) Then
                    prop.SetValue(ctrl, False, Nothing)
                End If

                ' Rekursiv in Untercontrols (z. B. Panels, TabPages, GroupBoxes …)
                If ctrl.HasChildren Then
                    SetTabStopRecursive(ctrl.Controls)
                End If
            Next
        End Sub
    End Module
End Namespace