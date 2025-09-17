Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

Imports System.IO
Imports System.Xml.Serialization

'Public Enum SteinSatz
'    Klassisch
'    Chinesische
'    Modern
'End Enum

'Public Enum SteinStatus
'    Unsichtbar
'    Normal
'    Selected
'    ClickableOne
'    ClickablePartOfPair
'    Locked
'    NotUnsed
'    MissingSecond
'    WerkstückEinfügeFehler
'    WerkstückZufallsgrafik
'    Reserve1
'    Reserve2
'End Enum

'Public Enum SteinIndexEnum
'    ErrorSy
'Punkt01: Punkt02 : Punkt03 : Punkt04 : Punkt05 : Punkt06 : Punkt07 : Punkt08 : Punkt09
'Bambus1: Bambus2 : Bambus3 : Bambus4 : Bambus5 : Bambus6 : Bambus7 : Bambus8 : Bambus9
'Symbol1: Symbol2 : Symbol3 : Symbol4 : Symbol5 : Symbol6 : Symbol7 : Symbol8 : Symbol9
'DracheR: DracheG : DracheW
'WindOst: WindSüd : WindWst : WindNrd
'BlütePf: BlüteOr : BlüteCt : BlüteBa
'JahrFrl: JahrSom : JahrHer : JahrWin
'End Enum

<XmlRoot("SteinPack")>
Public Class SteinPack
    <XmlAttribute> Public Property SteinSatz As String
    <XmlAttribute> Public Property RefWidth As Integer
    <XmlAttribute> Public Property RefHeight As Integer
    <XmlElement("SteinEntry")> Public Property Entries As List(Of SteinEntry) = New List(Of SteinEntry)
End Class

Public Class SteinEntry
    <XmlAttribute> Public Property Status As String        ' SteinStatus
    <XmlAttribute> Public Property Index As String         ' SteinIndexEnum
    <XmlElement> Public Property PngGzBase64 As String   ' GZip(Base64(PNG bytes))
End Class

Public NotInheritable Class SteinManager

    Private ReadOnly _root As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                                                    "Visual Studio", "MahjongGK", "Grafiken", "Mahjongsteine")
    Private _satz As SteinSatz
    Private _pack As SteinPack

    ' Cache: $"{status}|{index}"
    Private ReadOnly _cache As New Dictionary(Of String, Bitmap)(StringComparer.OrdinalIgnoreCase)

    Private Sub New()
    End Sub

    Public Shared ReadOnly Property Instance As SteinManager = New SteinManager()

    ''' <summary>
    ''' Lädt / wechselt den aktiven Steinsatz. Leert den Puffer. Exception bei Inkonsistenzen.
    ''' </summary>
    Public Sub LoadSteinSatz(aktSteinSatz As SteinSatz)
        Dim dir As String = Path.Combine(_root, aktSteinSatz.ToString())
        If Not Directory.Exists(dir) Then
            Throw New DirectoryNotFoundException($"Steinsatz-Ordner fehlt: {dir}")
        End If

        ' Kompilat lesen (vom Hilfsprogramm erzeugt):
        Dim xmlFile As String = Path.Combine(dir, $"Steine_{aktSteinSatz}.xml")
        If Not File.Exists(xmlFile) Then
            Throw New FileNotFoundException("Stein-XML fehlt.", xmlFile)
        End If

        Dim ser As New XmlSerializer(GetType(SteinPack))
        Using fs As FileStream = File.OpenRead(xmlFile)
            _pack = DirectCast(ser.Deserialize(fs), SteinPack)
        End Using

        If _pack Is Nothing OrElse _pack.Entries Is Nothing OrElse _pack.RefWidth <= 0 OrElse _pack.RefHeight <= 0 Then
            Throw New InvalidDataException("SteinPack leer/ungültig.")
        End If

        ' Cache leeren
        _cache.Clear()
        _satz = aktSteinSatz
    End Sub

    Public Function GetSteinSize() As Size
        If _pack Is Nothing Then Throw New InvalidOperationException("SteinManager.LoadSteinSatz(...) zuerst aufrufen.")
        Return New Size(_pack.RefWidth, _pack.RefHeight)
    End Function

    Public Function GetStein(status As SteinStatus, stein As SteinIndexEnum) As Bitmap
        If _pack Is Nothing Then Throw New InvalidOperationException("SteinManager.LoadSteinSatz(...) zuerst aufrufen.")
        Dim key As String = $"{status}|{stein}"
        Dim bmp As Bitmap = Nothing
        If _cache.TryGetValue(key, bmp) Then Return bmp

        ' Suche Entry
        Dim sStatus As String = status.ToString()
        Dim sIndex As String = stein.ToString()
        Dim entry As SteinEntry = _pack.Entries.FirstOrDefault(Function(e) e.Status = sStatus AndAlso e.Index = sIndex)

        If entry Is Nothing OrElse String.IsNullOrWhiteSpace(entry.PngGzBase64) Then
            ' Geistergrafik generieren (halbtransparent grau mit rotem Kreis + Indexname)
            bmp = RenderGhost(_pack.RefWidth, _pack.RefHeight, stein.ToString())
        Else
            Dim raw As Byte() = InflateGZip(entry.PngGzBase64)
            Using ms As New MemoryStream(raw)
                bmp = DirectCast(Image.FromStream(ms), Bitmap)
            End Using
        End If

        ' Dark-Mode ggf. aufhellen (gleiche zentrale Logik)
        If INI.Global_DarkMode AndAlso INI.Global_BrightenAmount > 0 Then
            Dim b2 As Bitmap = MaybeBrighten(bmp, INI.Global_BrightenAmount)
            If Not Object.ReferenceEquals(b2, bmp) Then bmp = b2
        End If

        _cache(key) = bmp
        Return bmp
    End Function

    Private Function RenderGhost(w As Integer, h As Integer, labelText As String) As Bitmap
        Dim bmp As New Bitmap(w, h, Imaging.PixelFormat.Format32bppPArgb)
        Using g As Graphics = Graphics.FromImage(bmp)
            g.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
            g.Clear(Color.Transparent)
            Using bg As New SolidBrush(Color.FromArgb(128, 160, 160, 160))
                g.FillRectangle(bg, 0, 0, w, h)
            End Using
            Dim r As Integer = Math.Min(w, h) - 10
            Using pen As New Pen(Color.Red, Math.Max(2, r \ 20))
                g.DrawEllipse(pen, (w - r) \ 2, (h - r) \ 2, r, r)
            End Using
            Using f As New Font("Segoe UI", Math.Max(8, Math.Min(w, h) \ 6), FontStyle.Bold, GraphicsUnit.Pixel),
                  sb As New SolidBrush(Color.FromArgb(220, 40, 40, 40))
                Dim sz As SizeF = g.MeasureString(labelText, f)
                g.DrawString(labelText, f, sb, (w - sz.Width) / 2.0F, (h - sz.Height) / 2.0F)
            End Using
        End Using
        Return bmp
    End Function

End Class
