Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

' ──────────────────────────────────────────────────────────────────────────────
'  MODULE: StrictOnCheatSheet
'  Zweck:  Spickzettel für VB.NET mit Option Strict On + Option Infer Off
'          Enthält nur Kommentare und Beispiel-Snippets.
'          Dient als Nachschlagewerk direkt in der IDE.
' ──────────────────────────────────────────────────────────────────────────────

Public Module StrictOnCheatSheet

    ' ===========================================================================
    ' 1) Collections & Dictionaries
    ' ===========================================================================

    ' For Each mit Typangabe:
    ' For Each kv As KeyValuePair(Of String, Integer) In dict
    '     Dim k As String = kv.Key
    '     Dim v As Integer = kv.Value
    ' Next

    ' ConcurrentDictionary mit Lambda-Factory:
    ' Dim img As Image = cd.GetOrAdd("key",
    '     Function(k As String) As Image
    '         Return New Bitmap(16, 16)
    '     End Function)

    ' ===========================================================================
    ' 2) Lambdas, Delegates, Events
    ' ===========================================================================

    ' Typisierte Lambda-Funktion:
    ' Dim f As Func(Of Integer, String) =
    '     Function(n As Integer) As String
    '         Return n.ToString()
    '     End Function

    ' Eventhandler mit Sub-Lambda:
    ' AddHandler btn.Click, Sub(sender As Object, e As EventArgs)
    '                           ' …
    '                       End Sub

    ' ===========================================================================
    ' 3) Casting & Konvertierungen
    ' ===========================================================================

    ' Wert-zu-Wert:
    ' Dim i As Integer = CInt(3.9)

    ' Referenztyp:
    ' Dim s As String = DirectCast(o, String)
    ' Dim ctl As Control = TryCast(o, Control)

    ' ===========================================================================
    ' 4) LINQ strikt typisieren
    ' ===========================================================================

    ' Dim q As IEnumerable(Of String) =
    '     From x As String In arr
    '     Where x.Length >= 2
    '     Select x

    ' ===========================================================================
    ' 5) Reflection strikt
    ' ===========================================================================

    ' Dim t As Type = GetType(Button)
    ' Dim pi As PropertyInfo = t.GetProperty("Text")
    ' If pi IsNot Nothing Then
    '     Dim txt As Object = pi.GetValue(btn, Nothing)
    '     Dim s As String = If(txt IsNot Nothing, txt.ToString(), "")
    ' End If

    ' ===========================================================================
    ' 6) Ressourcen & Images
    ' ===========================================================================

    ' Dim obj As Object = My.Resources.ResourceManager.GetObject("CompassN")
    ' Dim baseImg As Image = DirectCast(obj, Image)
    ' Dim clone As New Bitmap(baseImg)

    ' Using g As Graphics = Graphics.FromImage(clone)
    '     ' …
    ' End Using

    ' ===========================================================================
    ' 7) Zahlen & Math
    ' ===========================================================================

    ' Clamp:
    ' Public Function ClampInt(v As Integer, min As Integer, max As Integer) As Integer
    '     If v < min Then Return min
    '     If v > max Then Return max
    '     Return v
    ' End Function

    ' ===========================================================================
    ' 8) Enums
    ' ===========================================================================

    ' Public Enum ControlStatus
    '     Normal
    '     MouseOver
    '     Selected
    '     Disabled
    '     NotVisible
    ' End Enum

    ' If [Enum].TryParse(Of ControlStatus)("Selected", True, st) Then
    '     ' …
    ' End If

    ' ===========================================================================
    ' 9) Strings & Kultur
    ' ===========================================================================

    ' Dim ci As CultureInfo = CultureInfo.InvariantCulture
    ' Dim s As String = 1.23D.ToString(ci)

    ' ===========================================================================
    ' 10) XML-Serialisierung
    ' ===========================================================================

    ' <Serializable>
    ' Public Class SteinInfo
    '     <XmlAttribute> Public Property Id As Integer
    '     <XmlElement>   Public Property Name As String
    '     <XmlIgnore>    Public Property Tmp As Integer
    ' End Class

    ' ===========================================================================
    ' 11) WinForms-Layout
    ' ===========================================================================

    ' MenuStrip1.Dock = DockStyle.Top
    ' StatusStrip1.Dock = DockStyle.Bottom
    ' PanelContent.Dock = DockStyle.Fill
    ' PanelContent.BringToFront()

    ' ===========================================================================
    ' 12) Hilfs-Templates
    ' ===========================================================================

    ' Safe Dispose:
    ' Public Sub SafeDispose(Of T As Class)(ByRef o As T)
    '     Dim d As IDisposable = TryCast(o, IDisposable)
    '     If d IsNot Nothing Then d.Dispose()
    '     o = Nothing
    ' End Sub

End Module

