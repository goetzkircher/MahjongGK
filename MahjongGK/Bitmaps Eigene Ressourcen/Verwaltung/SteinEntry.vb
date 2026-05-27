Imports System.Xml.Serialization

Public Class SteinEntry
    <XmlAttribute> Public Property Status As String        ' SteinStatus
    <XmlAttribute> Public Property Index As String         ' SteinSymbol
    <XmlElement> Public Property PngGzBase64 As String   ' GZip(Base64(PNG bytes))
End Class
