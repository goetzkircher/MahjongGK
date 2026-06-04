Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

Imports System.Text
Imports System.Xml.Serialization
Imports MahjongGK.Contracts.GlobalEnum

#Disable Warning IDE0079
#Disable Warning IDE1006

Public Enum UnReDoJob
    None
    EtoE
    EtoS
    StoE
    StoS
    SortStock
    FillStock
End Enum
'
''' <summary>
''' Container, die die UndoRedo-Werte eines einzelnen Schrittes hält.
''' </summary>
Public Class EditorUndoItem

    Sub New()

    End Sub
    '
    ''' <summary>
    ''' Jeder der möglichen Undo-Schritte hat einen eigenen Konstruktor.
    ''' Dieser hier für das Verschieben eines Steines innerhalb des Editors.
    ''' </summary>
    Sub New(SteinSymbol As SteinSymbol, EditTplSrc As Triple, EditTplDst As Triple)
        Job = UnReDoJob.EtoE
        Me.SteinSymbol = SteinSymbol
        Me.EditTplSrc = EditTplSrc
        Me.EditTplDst = EditTplDst
    End Sub
    '
    ''' <summary>
    ''' Entnehmen eines Steines aus dem Editor.
    ''' </summary>
    Sub New(SteinSymbol As SteinSymbol, EditTplSrc As Triple, StockIdxDst As Integer)
        Job = UnReDoJob.EtoS
        Me.SteinSymbol = SteinSymbol
        Me.EditTplSrc = EditTplSrc
        Me.StockIdxDst = StockIdxDst
    End Sub
    '
    ''' <summary>
    ''' Hinzufügen eines Steines zum Feld.
    ''' </summary>
    Sub New(SteinSymbol As SteinSymbol, StockIdxSrc As Integer, EditTplDst As Triple)
        Job = UnReDoJob.StoE
        Me.SteinSymbol = SteinSymbol
        Me.StockIdxSrc = StockIdxSrc
        Me.EditTplDst = EditTplDst
    End Sub
    '
    ''' <summary>
    ''' Verschieben eines Steines innerhalb des Vorrat.
    ''' </summary>
    Sub New(SteinSymbol As SteinSymbol, StockIdxSrc As Integer, StockIdxDst As Integer)
        Job = UnReDoJob.StoS
        Me.SteinSymbol = SteinSymbol
        Me.StockIdxSrc = StockIdxSrc
        Me.StockIdxDst = StockIdxDst
    End Sub
    '
    ''' <summary>
    ''' Snapshot des Vorraten vor dem Sortieren und vor dem Auffüllen.
    ''' </summary>
    Sub New(job As UnReDoJob, StockSnapshot() As SteinSymbol)
        If job <> UnReDoJob.FillStock AndAlso job <> UnReDoJob.SortStock Then
            Throw New Exception("Nur UnReDoJob.FillStock und UnReDoJob.SortStock erlaubt.")
        End If
        Me.Job = job
        Me.StockSnapshot = StockSnapshot
    End Sub

    <XmlElement(ElementName:="Symb")>
    Public Property SteinSymbol As SteinSymbol

    <XmlElement(ElementName:="Job")>
    Public Property Job As UnReDoJob

    <XmlIgnore>
    Public Property EditTplSrc As Triple
        Get
            Return TripleX.FromXmlString(EditTplSrcXml)
        End Get
        Set(value As Triple)
            EditTplSrcXml = value.ToXmlString
        End Set
    End Property

    <XmlElement(ElementName:="TplSrc")>
    Public Property EditTplSrcXml As String

    <XmlIgnore>
    Public Property EditTplDst As Triple
        Get
            Return TripleX.FromXmlString(EditTplDstXml)
        End Get
        Set(value As Triple)
            EditTplDstXml = value.ToXmlString
        End Set
    End Property

    <XmlElement(ElementName:="TplDst")>
    Public Property EditTplDstXml As String

    <XmlElement(ElementName:="IdxSrc")>
    Public Property StockIdxSrc As Integer = -1

    <XmlElement(ElementName:="IdxDst")>
    Public Property StockIdxDst As Integer = -1

    <XmlIgnore>
    Public Property StockSnapshot() As SteinSymbol()
        Get
            Dim value As String = StockSnapshotXml
            If String.IsNullOrEmpty(value) Then
                Dim arr() As SteinSymbol = {}
                Return arr
            Else
                Dim arrS() As String = value.Split(";"c)
                Dim arrI(arrS.GetUpperBound(0)) As SteinSymbol
                For idx As Integer = 0 To arrS.GetUpperBound(0)
                    arrI(idx) = CType(Integer.Parse(arrS(idx)), SteinSymbol)
                Next
                Return arrI
            End If
        End Get
        Set(value As SteinSymbol())
            If value Is Nothing Then
                StockSnapshotXml = String.Empty
            ElseIf value.Length <= 0 Then
                StockSnapshotXml = String.Empty
            Else
                Dim sb As New StringBuilder(value.Length * 4) 'das reicht für um die 1000 Steine
                For idx As Integer = 0 To value.GetUpperBound(0)
                    sb.Append(CInt(value(idx))) '<--  Das das hier eine Integerzahl. sb.Append(value(idx)) wäre der Namen. 
                    sb.Append(";"c)
                Next
                sb.Length -= 1 'Das letzte Semikolon wieder entfernen.
                StockSnapshotXml = sb.ToString
            End If
        End Set
    End Property
    '
    ''' <summary>
    ''' Speichert den StockStatus als String und verhindert so die hunderte 
    ''' von Einträgen in der Xml, die dadurch beim Debuggen unleserlich wird.
    ''' Der normale Zugrff erfolgt über den Array StockStatus() As Integer()
    ''' </summary>
    ''' <returns></returns>
    <XmlElement(ElementName:="Stock")>
    Public Property StockSnapshotXml As String

End Class
