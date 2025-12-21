Imports System.Xml.Serialization

<XmlRoot("SteinPack")>
Public Class SteinPack
    <XmlAttribute> Public Property SteinSatz As String
    <XmlAttribute> Public Property RefWidth As Integer
    <XmlAttribute> Public Property RefHeight As Integer
    <XmlElement("SteinEntry")> Public Property Entries As List(Of SteinEntry) = New List(Of SteinEntry)
End Class
