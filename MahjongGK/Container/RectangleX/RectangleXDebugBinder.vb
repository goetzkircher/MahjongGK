Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On
Imports System.Reflection


'###############################################################################
'# RectangleXDebugBinder
'#
'# Warum ist dieses Vorgehen notwendig?
'#
'# In .NET/VB kennt eine Instanz ihr Feld- oder Variablennamen nicht. 
'# Beispiel:
'#     Public rxHeader As New RectangleX()
'# 
'# Das Objekt in rxHeader weiß nicht, dass es unter diesem Namen im 
'# Container deklariert ist. Für die Laufzeit ist es nur eine Referenz.
'#
'# Wenn wir im Debug-Overlay (DrawDebug) den Instanznamen einblenden 
'# wollen, müssen wir diesen Namen aktiv an die Instanz weitergeben.
'# 
'# Genau das macht RectangleXDebugBinder:
'#  - Er läuft per Reflection durch alle Felder und Properties in 
'#    SpielfeldDatenClass (oder jedem beliebigen Container).
'#  - Findet er ein RectangleX (oder ein Array/List(Of RectangleX)), 
'#    liest er den Wert und setzt mit __SetDebugName(...) den 
'#    entsprechenden Deklarationsnamen hinein.
'#  - Danach kann DrawDebug den Namen direkt aus der Instanz abfragen 
'#    und ins Bild malen.
'#
'# Wichtige Hinweise:
'#  - Der Binder muss erst aufgerufen werden, wenn die RectangleX-
'#    Instanzen erzeugt sind (sonst sind die Felder Nothing).
'#  - Das Ganze dient nur der Diagnose/Visualisierung. Für die Logik 
'#    selbst ist der Instanzname ohne Bedeutung.
'#  - Reflection kostet ein wenig Performance, daher sollte BindNames 
'#    nur einmal im Setup/Init aufgerufen werden, nicht im Renderloop.
'#
'###############################################################################

Public Module RectangleXDebugBinder
    ''' <summary>
    ''' Weist allen RectangleX-Feldern/Eigenschaften im Container ihren Debug-Namen zu.
    ''' Nur für Debug/Diagnose aufrufen (z.B. im Ctor/Init von SpielfeldDatenClass).
    ''' </summary>
    Public Sub BindNames(container As Object)
        If container Is Nothing Then Exit Sub
        Dim t As Type = container.GetType()

        ' 1) Felder
        'in f werden die einzelnen Rechtanglex erkannt, in rx nicht mehr.
        For Each f As FieldInfo In t.GetFields(BindingFlags.Instance Or BindingFlags.Public Or BindingFlags.NonPublic)
            If GetType(RectangleX).IsAssignableFrom(f.FieldType) Then
                Dim rx As Object = Nothing
                Try
                    rx = f.GetValue(container)
                Catch
                End Try

                Dim r As RectangleX = TryCast(rx, RectangleX)
                If r IsNot Nothing Then
                    r.__SetDebugName(f.Name)
                End If
            End If
        Next

        ' 2) Eigenschaften (nur lesbare, parameterlose)
        For Each p As PropertyInfo In t.GetProperties(BindingFlags.Instance Or BindingFlags.Public Or BindingFlags.NonPublic)
            If Not p.CanRead Then Continue For
            If p.GetIndexParameters().Length <> 0 Then Continue For
            If Not GetType(RectangleX).IsAssignableFrom(p.PropertyType) Then Continue For

            Dim rx As Object = Nothing
            Try
                rx = p.GetValue(container, Nothing)
            Catch
            End Try
            Dim r As RectangleX = TryCast(rx, RectangleX)
            If r IsNot Nothing Then
                r.__SetDebugName(p.Name)
            End If
        Next
    End Sub
End Module

