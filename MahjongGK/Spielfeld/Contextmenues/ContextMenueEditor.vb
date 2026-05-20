Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On
Imports System.Text
Imports MahjongGK.Spielfeld

Public NotInheritable Class ContextMenueEditor
    Inherits ContextMenuStrip

    Private ReadOnly _mnuDoubleClickSetStein As ToolStripMenuItem
    Private ReadOnly _mnuDoubleClickRemoveStein As ToolStripMenuItem
    Private ReadOnly _mnuNoDoubleClick As ToolStripMenuItem

    Private ReadOnly _mnuWindsAreInOneClickGroup As ToolStripMenuItem
    Private ReadOnly _mnuEditorSteinflugAnimation As ToolStripMenuItem

    Public Sub New()

        _mnuDoubleClickSetStein = New ToolStripMenuItem("Doppelklick setzt Stein")
        _mnuDoubleClickRemoveStein = New ToolStripMenuItem("Doppelklick entfernt Stein")
        _mnuNoDoubleClick = New ToolStripMenuItem("Kein Doppelklick")

        AddHandler _mnuDoubleClickSetStein.Click, AddressOf OnDoubleClickSetStein
        AddHandler _mnuDoubleClickRemoveStein.Click, AddressOf OnDoubleClickRemoveStein
        AddHandler _mnuNoDoubleClick.Click, AddressOf OnNoDoubleClick

        Items.Add(_mnuDoubleClickSetStein)
        Items.Add(_mnuDoubleClickRemoveStein)
        Items.Add(_mnuNoDoubleClick)

        Dim mnuInfoDoubleClick As New ToolStripMenuItem("Info")
        AddHandler mnuInfoDoubleClick.Click, AddressOf OnInfoDoubleClick
        Items.Add(mnuInfoDoubleClick)

        Items.Add(New ToolStripSeparator())

        _mnuWindsAreInOneClickGroup = New ToolStripMenuItem("Spielregelvereinfachung")
        AddHandler _mnuWindsAreInOneClickGroup.Click, AddressOf OnWindsAreInOneClickGroup
        Items.Add(_mnuWindsAreInOneClickGroup)

        Dim mnuInfoWinds As New ToolStripMenuItem("Info")
        AddHandler mnuInfoWinds.Click, AddressOf OnInfoWinds
        Items.Add(mnuInfoWinds)

        Items.Add(New ToolStripSeparator())

        _mnuEditorSteinflugAnimation = New ToolStripMenuItem("Steinfluganimation")
        AddHandler _mnuEditorSteinflugAnimation.Click, AddressOf OnEditorSteinflugAnimation
        Items.Add(_mnuEditorSteinflugAnimation)

        Dim mnuInfoSteinflug As New ToolStripMenuItem("Info")
        AddHandler mnuInfoSteinflug.Click, AddressOf OnInfoSteinflug
        Items.Add(mnuInfoSteinflug)

        Items.Add(New ToolStripSeparator())

        Dim mnuShuffleAll As New ToolStripMenuItem("Alle Steine mischen")
        AddHandler mnuShuffleAll.Click, AddressOf OShuffleAll
        Items.Add(mnuShuffleAll)

        Dim mnuShuffleOnly As New ToolStripMenuItem("Mischen ohne reservierte Steine")
        AddHandler mnuShuffleOnly.Click, AddressOf ObShuffleOnly
        Items.Add(mnuShuffleOnly)

        Dim mnuInfoShuffle As New ToolStripMenuItem("Info + Info Doppelklick im Vorrat")
        AddHandler mnuInfoShuffle.Click, AddressOf OnInfoShuffle
        Items.Add(mnuInfoShuffle)

        RefreshCheckMarks()

    End Sub

    Private Sub RefreshCheckMarks()

        _mnuDoubleClickSetStein.Checked = INI.Editor_DoubleClickSetStein
        _mnuDoubleClickRemoveStein.Checked = INI.Editor_DoubleClickRemoveStein
        _mnuNoDoubleClick.Checked =
            Not INI.Editor_DoubleClickSetStein AndAlso
            Not INI.Editor_DoubleClickRemoveStein

        _mnuWindsAreInOneClickGroup.Checked = INI.Spielbetrieb_WindsAreInOneClickGroup
        _mnuEditorSteinflugAnimation.Checked = INI.Editor_SteinflugAnimation

    End Sub

    Private Sub OnDoubleClickSetStein(sender As Object, e As EventArgs)

        INI.Editor_DoubleClickSetStein = True
        INI.Editor_DoubleClickRemoveStein = False

        RefreshCheckMarks()

    End Sub

    Private Sub OnDoubleClickRemoveStein(sender As Object, e As EventArgs)

        INI.Editor_DoubleClickSetStein = False
        INI.Editor_DoubleClickRemoveStein = True

        RefreshCheckMarks()

    End Sub

    Private Sub OnNoDoubleClick(sender As Object, e As EventArgs)

        INI.Editor_DoubleClickSetStein = False
        INI.Editor_DoubleClickRemoveStein = False

        RefreshCheckMarks()

    End Sub

    Private Sub OnWindsAreInOneClickGroup(sender As Object, e As EventArgs)

        INI.Spielbetrieb_WindsAreInOneClickGroup = Not INI.Spielbetrieb_WindsAreInOneClickGroup

        RefreshCheckMarks()

    End Sub

    Private Sub OnEditorSteinflugAnimation(sender As Object, e As EventArgs)

        INI.Editor_SteinflugAnimation = Not INI.Editor_SteinflugAnimation

        RefreshCheckMarks()

    End Sub

    Private Sub OShuffleAll(sender As Object, e As EventArgs)
        SFMain.SFDat.SFStock.ShuffleStock(includeNoSortArea:=True)
    End Sub
    Private Sub ObShuffleOnly(sender As Object, e As EventArgs)
        SFMain.SFDat.SFStock.ShuffleStock(includeNoSortArea:=False)
    End Sub

    Private Sub OnInfoDoubleClick(sender As Object, e As EventArgs)

        Dim sb As New StringBuilder
        sb.Append("Wenn ""Doppelklick setzt Stein"" eingeschaltet ist, erscheint, sofern noch ein Stein im Vorrat ist, ")
        sb.Append("eine Geistergrafik an den Stellen, wo eine Steinablage möglich ist. ")
        sb.Append("Ein Doppelklick setzt dort den Stein, der sich im Vorrat ganz links befindet.(Gemeint ist hier der erste sichtbare Stein, das ist nicht zwangsläufig der erste Stein im Vorrat.))")
        sb.AppendLine()
        sb.Append("Unabhängig davon kann jeder Stein aus Vorrat oder Spielfeld per Drag & Drop in alle Richtungen gezogen werden, ")
        sb.Append("also sowohl innerhalb von Spielfeld und Vorrat, als auch zwischen Spielfeld und Vorrat.")
        sb.AppendLine()
        sb.AppendLine()
        sb.Append("Wenn ""Doppelklick löscht Stein"" eingeschaltet ist, wird beim Doppelklick ein Stein, ")
        sb.AppendLine("dessen Oberseite frei liegt, der also vollständig sichtbar ist, gelöscht und zurück in den Vorrat gebracht. Auch hier an die erste sichtbare Position.")
        sb.Append("Auch hier ist das Drag & Drop weiterhin möglich.")
        sb.AppendLine()
        sb.AppendLine()
        sb.Append("Hinweis: Ein Stein belegt vier Rechtecke. Das linke obere Rechteck ist das ""Führungsrechteck"". ")
        sb.Append("Ein Klick/Doppelklick in diesen Bereich eines Steines verhindert das gelegentliche Springen und andere Besonderheiten im Verhalten des Steins, das durch den ersten Klick ausgelößt wird.")

        MessageBox.Show(
            sb.ToString,
            "Info",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information)

    End Sub

    Private Sub OnInfoWinds(sender As Object, e As EventArgs)

        MessageBox.Show(
            "Wenn eingeschaltet, können die vier Winde in beliebiger Kombination paarweise entnommen werden. Im Editor wird die Anzeige der entnehmbaren Steinpaare entsprechend geschaltet.",
            "Info",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information)

    End Sub

    Private Sub OnInfoSteinflug(sender As Object, e As EventArgs)

        MessageBox.Show(
            "Wenn eine Aktion abgebrochen wird (Loslassen des Steines an einer Position ohne Geistergrafik) oder Steine per Doppelklick entfernt werden, beamen oder fliegen diese zurück, je nach Schalterstellung.",
            "Info",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information)

    End Sub
    Private Sub OnInfoShuffle(sender As Object, e As EventArgs)

        Dim sb As New StringBuilder

        sb.AppendLine("Mischen der Steine:")
        sb.Append("Zunächst: Es gibt im Steinvorrat zwei Bereiche: Am Anfang, die ersten 10 Steine (Menge in den Einstellungen änderbar; Im Steinvorrat abgetrennt durch einen schmalen Abstand) ")
        sb.Append("das ist der ""reservierte Bereich"". Das heißt: Dieser Bereich kann vom Mischen der Steine ausgenommen werden.")
        sb.AppendLine("Damit ist es möglich bestimmte Steine zu sammeln.")
        sb.AppendLine("Und damit dürfte auch klar sein, was mit: ""Alle Steine mischen"" und mit ""Mischen ohne reservierte Steine"" gemeint ist.")
        sb.AppendLine()
        sb.AppendLine("Doppelklick im Steinvorrat:")
        sb.Append("Ein Doppelklick auf einen Stein im Steinvorrat transportiert diesen Stein immer ganz an den Anfang an die erste Position der reservierten Steine.")

        MessageBox.Show(
            sb.ToString,
            "Info",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information)
    End Sub

    Protected Overrides Sub OnOpening(e As System.ComponentModel.CancelEventArgs)

        INI.Volatil_ContextMenueEditorIsOpen = True
        MyBase.OnOpening(e)
        RefreshCheckMarks()

    End Sub

    Protected Overrides Sub OnClosed(e As ToolStripDropDownClosedEventArgs)

        MyBase.OnClosed(e)
        INI.Volatil_ContextMenueEditorIsOpen = False

    End Sub
End Class
