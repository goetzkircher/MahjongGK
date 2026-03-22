Imports System.ComponentModel
Imports System.Reflection

Namespace Helfer
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


        ''' <summary>
        ''' Liefert True, wenn sich das aktuelle Control im Designer befindet.
        ''' Kombination aus LicenseManager.UsageMode und Site.DesignMode.
        ''' </summary>
        <DebuggerStepThrough>
        Public Function IsInDesigner(ctrl As Control) As Boolean
            If LicenseManager.UsageMode = LicenseUsageMode.Designtime Then
                Return True
            End If
            If ctrl IsNot Nothing AndAlso
               ctrl.Site IsNot Nothing AndAlso
               ctrl.Site.DesignMode Then
                Return True
            End If
            Return False
        End Function

    End Module
End Namespace