Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

Imports System.Xml.Serialization

<XmlRoot("GfxPack")>
Public Class GfxPack
    <XmlAttribute> Public Property Name As String
    <XmlElement("Entry")> Public Property Entries As List(Of GfxEntry) = New List(Of GfxEntry)
End Class

<Serializable>
Public Class GfxEntry
    Public Property Key As String           ' basisKey (ohne q##), wie gehabt
    Public Property Kind As String          ' "svg" / "png"
    Public Property SizeHint As String      ' bisher: "##q" oder "", darf bleiben (optional)
    Public Property DataGzBase64 As String  ' gz + base64 payload

    ' NEU: für ImageList-Gruppierung und Reihenfolge
    Public Property Folder As String        ' Relativer Ordner (z.B. "ImageList_Buttons" oder "Pfeile")
    Public Property BaseName As String      ' Dateiname ohne Ext/q## (für konsistente Sortierung)
End Class
