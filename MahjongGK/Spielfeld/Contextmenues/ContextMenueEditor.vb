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
    Private ReadOnly _mnuShowGhost As ToolStripMenuItem
    Private ReadOnly _mnuWindsAreInOneClickGroup As ToolStripMenuItem

    Public Sub New()

        _mnuDoubleClickSetStein = New ToolStripMenuItem("Doppelklick setzt Stein")
        _mnuDoubleClickRemoveStein = New ToolStripMenuItem("Doppelklick entfernt Stein")
        _mnuNoDoubleClick = New ToolStripMenuItem("Kein Doppelklick")
        _mnuShowGhost = New ToolStripMenuItem("Ablegeposition als Geist")

        AddHandler _mnuDoubleClickSetStein.Click, AddressOf OnDoubleClickSetStein
        AddHandler _mnuDoubleClickRemoveStein.Click, AddressOf OnDoubleClickRemoveStein
        AddHandler _mnuNoDoubleClick.Click, AddressOf OnNoDoubleClick
        AddHandler _mnuShowGhost.Click, AddressOf OnShowGhost

        Items.Add(_mnuDoubleClickSetStein)
        Items.Add(_mnuDoubleClickRemoveStein)
        Items.Add(_mnuNoDoubleClick)
        Items.Add(_mnuShowGhost)

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

        Dim mnuToolBoxOpen As New ToolStripMenuItem("Toolbox öffnen")
        AddHandler mnuToolBoxOpen.Click, AddressOf OnToolboxOpen
        Items.Add(mnuToolBoxOpen)

        Dim mnuInfoToolbox As New ToolStripMenuItem("Info")
        AddHandler mnuInfoToolbox.Click, AddressOf OnInfoToolboxOpen
        Items.Add(mnuInfoToolbox)

        Items.Add(New ToolStripSeparator())

        Dim mnuShuffleAll As New ToolStripMenuItem("Alle Steine mischen")
        AddHandler mnuShuffleAll.Click, AddressOf OnShuffleAll
        Items.Add(mnuShuffleAll)

        Dim mnuShuffleOnly As New ToolStripMenuItem("Ohne reservierte Steine mischen")
        AddHandler mnuShuffleOnly.Click, AddressOf OnShuffleOnly
        Items.Add(mnuShuffleOnly)

        Dim mnuInfoShuffle As New ToolStripMenuItem("Info + Info Doppelklick im Vorrat")
        AddHandler mnuInfoShuffle.Click, AddressOf OnInfoShuffle
        Items.Add(mnuInfoShuffle)

        Items.Add(New ToolStripSeparator())

        Dim mnuSortPur As New ToolStripMenuItem("Steine aufteigend sortieren")
        AddHandler mnuSortPur.Click, AddressOf OnSortPur
        Items.Add(mnuSortPur)

        Dim mnuSortByPair As New ToolStripMenuItem("Steine paarweise zusammenstellen")
        AddHandler mnuSortByPair.Click, AddressOf OnSortByPair
        Items.Add(mnuSortByPair)

        Dim mnuSeperateStockWidow As New ToolStripMenuItem("Witwen an den Anfang ziehen")
        AddHandler mnuSeperateStockWidow.Click, AddressOf OnSeperateStockWidow
        Items.Add(mnuSeperateStockWidow)

        Dim mnuInfoSort As New ToolStripMenuItem("Info")
        AddHandler mnuInfoSort.Click, AddressOf OnInfoSort
        Items.Add(mnuInfoSort)

        RefreshCheckMarks()

    End Sub

    Private Sub RefreshCheckMarks()

        _mnuDoubleClickSetStein.Checked = INI.Editor_DoubleClickSetStein
        _mnuDoubleClickRemoveStein.Checked = INI.Editor_DoubleClickRemoveStein
        _mnuNoDoubleClick.Checked =
            Not INI.Editor_DoubleClickSetStein AndAlso
            Not INI.Editor_DoubleClickRemoveStein

        _mnuWindsAreInOneClickGroup.Checked = INI.Spielbetrieb_WindsAreInOneClickGroup
        _mnuShowGhost.Checked = INI.Editor_ShowDoubleclickGhost
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
    Private Sub OnShowGhost(sender As Object, e As EventArgs)

        INI.Editor_ShowDoubleclickGhost = Not INI.Editor_ShowDoubleclickGhost

        RefreshCheckMarks()

    End Sub

    Private Sub OnWindsAreInOneClickGroup(sender As Object, e As EventArgs)

        INI.Spielbetrieb_WindsAreInOneClickGroup = Not INI.Spielbetrieb_WindsAreInOneClickGroup

        RefreshCheckMarks()

    End Sub

    Private Sub OnToolboxOpen(sender As Object, e As EventArgs)
        frmMain.DoToolBox()
    End Sub

    Private Sub OnShuffleAll(sender As Object, e As EventArgs)
        SFMain.SFDat.SFStock.ShuffleStock(includeNoSortArea:=True)
    End Sub
    Private Sub OnShuffleOnly(sender As Object, e As EventArgs)
        SFMain.SFDat.SFStock.ShuffleStock(includeNoSortArea:=False)
    End Sub
    Private Sub OnSortPur(sender As Object, e As EventArgs)
        SFMain.SFDat.SFStock.SortStockPur()
    End Sub
    Private Sub OnSortByPair(sender As Object, e As EventArgs)
        SFMain.SFDat.SFStock.SortStockByPairs()
    End Sub
    Private Sub OnSeperateStockWidow(sender As Object, e As EventArgs)
        SFMain.SFDat.SFStock.SeperateStockWidow()
    End Sub

    Private Sub OnInfoDoubleClick(sender As Object, e As EventArgs)

        Dim sb As New StringBuilder

        sb.Append("Wenn ""Doppelklick setzt Stein"" eingeschaltet ist, erscheint, sofern noch ein Stein im Vorrat ist, ")
        sb.Append("eine Geistergrafik/ein Stein an den Stellen, wo eine Steinablage möglich ist. ")
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
        sb.AppendLine()
        sb.AppendLine()
        sb.AppendLine("Wenn ""Ablegeposition als Geist"" ausgeschaltet ist, erscheint dort ein (fast) normaler Stein.")

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

    Private Sub OnInfoToolboxOpen(sender As Object, e As EventArgs)

        MessageBox.Show(
            "Die Toolbox ist die Werkzeugkiste, mit der Größe des Spielfeldes, Hintergrundbild und Name, festgelegt oder geändert werden.",
            "Info",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information)

    End Sub
    Private Sub OnInfoShuffle(sender As Object, e As EventArgs)

        Dim sb As New StringBuilder

        sb.AppendLine("Mischen der Steine:")
        sb.Append("Es gibt im Steinvorrat zwei Bereiche: Am Anfang, die ersten 10 Steine (Menge in den Einstellungen änderbar; im Steinvorrat abgetrennt durch einen schmalen Abstand) ")
        sb.Append("das ist der ""reservierte Bereich"". Das heißt: Dieser Bereich kann vom Mischen der Steine ausgenommen werden.")
        sb.AppendLine("Damit ist es möglich bestimmte Steine zu sammeln.")
        sb.AppendLine("Und damit dürfte auch klar sein, was mit: ""Alle Steine mischen"" und mit ""Mischen ohne reservierte Steine"" gemeint ist.")
        sb.AppendLine()
        sb.AppendLine("Doppelklick im Steinvorrat:")
        sb.Append("Ein Doppelklick auf einen Stein im Steinvorrat transportiert diesen Stein immer ganz an den Anfang an die erste Position im reservierten Bereich.")

        MessageBox.Show(
            sb.ToString,
            "Info",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information)
    End Sub

    Private Sub OnInfoSort(sender As Object, e As EventArgs)

        Dim sb As New StringBuilder
        sb.AppendLine("Es gibt verschiedene Sortiermöglichkeiten.")
        sb.AppendLine()
        sb.AppendLine("Aufsteigend:")
        sb.AppendLine("Einfache systematische Sortierung aller Steine.")
        sb.AppendLine()
        sb.AppendLine("Paarweise:")
        sb.AppendLine("Die Reihenfolge bleibt erhalten mit zwei Besonderheiten:")
        sb.AppendLine("1.) Jedem Stein wird sein Partnerstein zugeordnet.")
        sb.AppendLine("2.) die ""Witwen"" kommen ganz nach vorne")
        sb.AppendLine("Witwen sind die Steine, deren Partnerstein bereits auf dem Spielfeld steht.")
        sb.AppendLine()
        sb.AppendLine("Witwen an den Anfang ziehen:")
        sb.AppendLine("Jeder Stein braucht einen Partnerstein.")
        sb.AppendLine("Die Funktion zieht die im Spielfeld noch fehlenden Steine ganz nach vorne. Sie sind mit einem ""!"" markiert.")

        sb.AppendLine("Generell:")
        sb.Append("Witwensteine werden im Vorrat immer mit einem Ausrufezeichen ""!"" links oben markiert. ")
        sb.Append("Aber: Wenn zwei gleiche Steine im Blickfeld sind, und einer wird entnommen, wird nicht ")
        sb.Append("zwangsweise der Andere makiert. Es wird der erste Parterstein von Links markiert, und das ")
        sb.Append("kann ein anderer Stein sein, der sich außerhalb des Sichtfeldes befindet.")

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
        INI.Volatil_SetConsumeDoRenderAfterContextMenueClosed()
    End Sub
End Class
