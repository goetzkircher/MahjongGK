Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

#Disable Warning IDE0079
#Disable Warning IDE1006

Public Module EnumHelper

    ''So sieht die Verwendung In einer XML-Property aus

    '<XmlIgnore>
    'Public Property SteinDesign As SteinDesign = SteinDesign.Default

    '<XmlElement("SteinDesign")>
    'Public Property SteinDesignXml As String
    '    Get
    '        Return EnumHelper.ToXmlString(Of SteinDesign)(Me.SteinDesign)
    '    End Get
    '    Set(ByVal value As String)
    '        Me.SteinDesign = EnumHelper.ParseOrDefault(Of SteinDesign)(
    '        value,
    '        SteinDesign.Default)
    '    End Set
    'End Property

    ''Für andere Enums genau genauso:

    '<XmlIgnore>
    'Public Property FaceLightMapNormal As LightMap = LightMap.Zentral

    '<XmlElement("FaceLightMapNormal")>
    'Public Property FaceLichtkarteXml As String
    '    Get
    '        Return EnumHelper.ToXmlString(Of LightMap)(Me.FaceLightMapNormal)
    '    End Get
    '    Set(ByVal value As String)
    '        Me.FaceLightMapNormal = EnumHelper.ParseOrDefault(Of LightMap)(
    '        value,
    '        LightMap.Zentral)
    '    End Set
    'End Property

    'Wenn du es noch knapper willst, kannst du im Get auch einfach schreiben:
    'Return Me.SteinDesign.ToString()
    'Dann brauchst du ToXmlString eigentlich nicht zwingend. Der eigentliche Gewinn steckt In ParseOrDefault.
    'Die Kurzform für deine Set-Blocks ist dann immer
    'Me.DeineEnumProperty = EnumHelper.ParseOrDefault(Of DeineEnum)(
    '    value,
    '    DeineEnum.DeinDefault)

    ''' <summary>
    ''' Wandelt einen String in eine Enumeration um.
    ''' Im Fehlerfall wird im Debugger eine Exception geworfen,
    ''' sonst der angegebene Defaultwert geliefert.
    ''' </summary>
    Public Function ParseOrDefault(Of TEnum As Structure)(
        ByVal value As String,
        ByVal defaultValue As TEnum) As TEnum

        If Not GetType(TEnum).IsEnum Then
            Throw New InvalidOperationException(
                $"{GetType(TEnum).FullName} ist kein Enum-Typ.")
        End If

        If value Is Nothing Then
            If Debugger.IsAttached Then
                Throw New InvalidOperationException(
                    $"Ungültiger {GetType(TEnum).Name}-Wert: <Nothing>.")
            End If

            Return defaultValue
        End If

        Dim parsed As TEnum = Nothing

        If [Enum].TryParse(Of TEnum)(value, True, parsed) Then
            Return parsed
        End If

        If Debugger.IsAttached Then
            Throw New InvalidOperationException(
                $"Ungültiger {GetType(TEnum).Name}-Wert: '{value}'.")
        End If

        Return defaultValue
    End Function

    ''' <summary>
    ''' Liefert den Enumwert als Xml-String.
    ''' </summary>
    Public Function ToXmlString(Of TEnum As Structure)(
        ByVal value As TEnum) As String

        If Not GetType(TEnum).IsEnum Then
            Throw New InvalidOperationException(
                $"{GetType(TEnum).FullName} ist kein Enum-Typ.")
        End If

        Return value.ToString()
    End Function

End Module
