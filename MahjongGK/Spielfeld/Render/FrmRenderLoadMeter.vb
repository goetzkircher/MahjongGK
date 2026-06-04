Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

#Disable Warning IDE0079
#Disable Warning IDE1006
#Disable Warning IDE0017

Public NotInheritable Class FrmRenderLoadMeter
    Inherits Form

    Private Enum RenderKind
        Pur = 0
        WithCreateNewLayout = 1
    End Enum

    Private Const BUCKET_COUNT As Integer = 23
    Private Const LAST_BUCKET_INDEX As Integer = BUCKET_COUNT - 1

    Private ReadOnly _bucketsPur(LAST_BUCKET_INDEX) As Long
    Private ReadOnly _bucketsWithLayout(LAST_BUCKET_INDEX) As Long

    Private _paintCycleCountForAverage As Long
    Private _sumPaintCycleTicks As Double
    Private _avgPaintCycleTicks As Double

    Private ReadOnly lblStatus As Label
    Private ReadOnly lblSummary As Label
    Private ReadOnly lvBuckets As ListView
    Private ReadOnly btnStop As Button
    Private ReadOnly uiTimer As Timer

    Private _isRunning As Boolean = True

    Private _hasLastPaint As Boolean
    Private _lastPaintTick As Long

    Private _isRendering As Boolean
    Private _renderBeginTick As Long
    Private _renderTicksInCycle As Long

    Private ReadOnly _paintCycles As Long
    Private _renderStarts As Long
    Private _renderPurCount As Long
    Private _renderWithLayoutCount As Long
    Private _abortCount As Long
    Private _blitCycles As Long

    Private _sumPercentPur As Double
    Private _sumPercentWithLayout As Double

    Private _maxPercentPur As Double
    Private _maxPercentWithLayout As Double

    Private _countPurOver75 As Long
    Private _countPurOver90 As Long
    Private _countPurOver100 As Long

    Private _countWithLayoutOver75 As Long
    Private _countWithLayoutOver90 As Long
    Private _countWithLayoutOver100 As Long

    Public Sub New()

        Text = "Renderlast-Messung"
        TopMost = True
        StartPosition = FormStartPosition.Manual
        Size = New Size(720, 720)
        MinimumSize = New Size(620, 420)
        DoubleBuffered = True
        Font = New Font("Cascadia Mono", 9.0F, FontStyle.Regular)

        lblStatus = New Label()
        lblStatus.Dock = DockStyle.Top
        lblStatus.Height = 28
        lblStatus.TextAlign = ContentAlignment.MiddleLeft
        lblStatus.Text = "Messung läuft..."

        lblSummary = New Label()
        lblSummary.Dock = DockStyle.Top
        lblSummary.Height = 120
        lblSummary.TextAlign = ContentAlignment.MiddleLeft

        btnStop = New Button()
        btnStop.Text = "Stop"
        btnStop.Dock = DockStyle.Bottom
        btnStop.Height = 36
        AddHandler btnStop.Click, AddressOf BtnStop_Click

        lvBuckets = New ListView()
        lvBuckets.Dock = DockStyle.Fill
        lvBuckets.View = View.Details
        lvBuckets.FullRowSelect = True
        lvBuckets.GridLines = True
        lvBuckets.Columns.Add("Bereich", 90)
        lvBuckets.Columns.Add("Pur Anzahl", 90)
        lvBuckets.Columns.Add("Pur Anteil", 90)
        lvBuckets.Columns.Add("Layout Anzahl", 100)
        lvBuckets.Columns.Add("Layout Anteil", 100)
        lvBuckets.Columns.Add("Balken", 220)

        Controls.Add(lvBuckets)
        Controls.Add(lblSummary)
        Controls.Add(lblStatus)
        Controls.Add(btnStop)

        InitBucketRows()

        uiTimer = New Timer()
        uiTimer.Interval = 1000
        AddHandler uiTimer.Tick, AddressOf UiTimer_Tick
        uiTimer.Start()

    End Sub

    Public Sub NotificationWindowsPaint()

        If Not _isRunning Then
            Return
        End If

        Dim nowTick As Long = Stopwatch.GetTimestamp()

        If _hasLastPaint Then

            Dim paintCycleTicks As Long = nowTick - _lastPaintTick

            If paintCycleTicks > 0 Then
                _paintCycleCountForAverage += 1
                _sumPaintCycleTicks += CDbl(paintCycleTicks)
                _avgPaintCycleTicks = _sumPaintCycleTicks / CDbl(_paintCycleCountForAverage)
            End If

        End If

        _lastPaintTick = nowTick
        _hasLastPaint = True
        _renderTicksInCycle = 0

    End Sub

    Public Sub NotificationBlitLastRendering()

        If Not _isRunning Then
            Return
        End If

        _blitCycles += 1

    End Sub

    Public Sub BeginnRendering()

        If Not _isRunning Then
            Return
        End If

        If _isRendering Then
            Return
        End If

        _renderStarts += 1
        _renderBeginTick = Stopwatch.GetTimestamp()
        _isRendering = True

    End Sub

    Public Sub EndRenderingPur()

        EndRenderingCore(RenderKind.Pur)

    End Sub

    Public Sub EndRenderingWithCreateNewLayout()

        EndRenderingCore(RenderKind.WithCreateNewLayout)

    End Sub

    Public Sub AbortRendering()

        If Not _isRunning Then
            Return
        End If

        If Not _isRendering Then
            Return
        End If

        _isRendering = False
        _abortCount += 1

        If _renderStarts > 0 Then
            _renderStarts -= 1
        End If

    End Sub

    Private Sub EndRenderingCore(kind As RenderKind)

        If Not _isRunning Then
            Return
        End If

        If Not _isRendering Then
            Return
        End If

        Dim nowTick As Long = Stopwatch.GetTimestamp()
        Dim renderTicks As Long = nowTick - _renderBeginTick

        _isRendering = False

        If renderTicks <= 0 Then
            Return
        End If

        If _avgPaintCycleTicks <= 0.0R Then
            Return
        End If

        AddRenderValue(kind, renderTicks)

    End Sub

    Private Sub AddRenderValue(kind As RenderKind, renderTicks As Long)

        Dim percent As Double = CDbl(renderTicks) * 100.0R / _avgPaintCycleTicks

        Dim bucket As Integer = CInt(Math.Floor(percent / 5.0R))

        If bucket < 0 Then
            bucket = 0
        ElseIf bucket > LAST_BUCKET_INDEX Then
            bucket = LAST_BUCKET_INDEX
        End If

        Select Case kind

            Case RenderKind.Pur

                _renderPurCount += 1
                _sumPercentPur += percent
                _bucketsPur(bucket) += 1

                If percent > _maxPercentPur Then
                    _maxPercentPur = percent
                End If

                If percent >= 75.0R Then _countPurOver75 += 1
                If percent >= 90.0R Then _countPurOver90 += 1
                If percent >= 100.0R Then _countPurOver100 += 1

            Case RenderKind.WithCreateNewLayout

                _renderWithLayoutCount += 1
                _sumPercentWithLayout += percent
                _bucketsWithLayout(bucket) += 1

                If percent > _maxPercentWithLayout Then
                    _maxPercentWithLayout = percent
                End If

                If percent >= 75.0R Then _countWithLayoutOver75 += 1
                If percent >= 90.0R Then _countWithLayoutOver90 += 1
                If percent >= 100.0R Then _countWithLayoutOver100 += 1

        End Select

    End Sub

    Public Sub StopMeasurement()

        If Not _isRunning Then
            Return
        End If

        If _isRendering Then
            AbortRendering()
        End If

        _isRunning = False
        uiTimer.Stop()
        btnStop.Enabled = False
        lblStatus.Text = "Messung beendet."
        UpdateDisplay()

    End Sub

    Private Sub InitBucketRows()

        lvBuckets.Items.Clear()

        For idx As Integer = 0 To LAST_BUCKET_INDEX

            Dim textRange As String

            If idx < LAST_BUCKET_INDEX Then
                textRange = (idx * 5).ToString() & "–<" & ((idx + 1) * 5).ToString() & " %"
            Else
                textRange = ">= 110 %"
            End If

            Dim item As New ListViewItem(textRange)
            item.SubItems.Add("0")
            item.SubItems.Add("0,00 %")
            item.SubItems.Add("0")
            item.SubItems.Add("0,00 %")
            item.SubItems.Add(String.Empty)

            lvBuckets.Items.Add(item)

        Next

    End Sub

    Private Sub UpdateDisplay()

        Me.SuspendLayout()
        lvBuckets.BeginUpdate()

        Dim avgPur As Double = 0.0R
        Dim avgWithLayout As Double = 0.0R
        Dim avgPaintMs As Double = 0.0R

        If _avgPaintCycleTicks > 0.0R Then
            avgPaintMs = _avgPaintCycleTicks * 1000.0R / CDbl(Stopwatch.Frequency)
        End If

        If _renderPurCount > 0 Then
            avgPur = _sumPercentPur / CDbl(_renderPurCount)
        End If

        If _renderWithLayoutCount > 0 Then
            avgWithLayout = _sumPercentWithLayout / CDbl(_renderWithLayoutCount)
        End If

        lblSummary.Text =
            "Renderstarts: " & _renderStarts.ToString() &
            "    Pur: " & _renderPurCount.ToString() &
            "    Mit Layout: " & _renderWithLayoutCount.ToString() &
            "    Blit: " & _blitCycles.ToString() & Environment.NewLine &
            "Pur Ø: " & avgPur.ToString("0.00") & " %    Max: " & _maxPercentPur.ToString("0.00") & " %" & Environment.NewLine &
            "Pur >=75 %: " & _countPurOver75.ToString() &
            "    >=90 %: " & _countPurOver90.ToString() &
            "    >=100 %: " & _countPurOver100.ToString() & Environment.NewLine &
            "Layout Ø: " & avgWithLayout.ToString("0.00") & " %    Max: " & _maxPercentWithLayout.ToString("0.00") & " %" & Environment.NewLine &
            "Layout >=75 %: " & _countWithLayoutOver75.ToString() &
            "    >=90 %: " & _countWithLayoutOver90.ToString() &
            "    >=100 %: " & _countWithLayoutOver100.ToString() & Environment.NewLine &
            "Paint Ø: " & avgPaintMs.ToString("0.000") & " ms" & Environment.NewLine

        ''"    Abbruch: " & _abortCount.ToString() & 'herausgenommen, da derzeit identsich Blit.
        For idx As Integer = 0 To LAST_BUCKET_INDEX

            Dim countPur As Long = _bucketsPur(idx)
            Dim countWithLayout As Long = _bucketsWithLayout(idx)

            Dim sharePur As Double = 0.0R
            Dim shareWithLayout As Double = 0.0R

            If _renderPurCount > 0 Then
                sharePur = CDbl(countPur) * 100.0R / CDbl(_renderPurCount)
            End If

            If _renderWithLayoutCount > 0 Then
                shareWithLayout = CDbl(countWithLayout) * 100.0R / CDbl(_renderWithLayoutCount)
            End If

            lvBuckets.Items(idx).SubItems(1).Text = countPur.ToString()
            lvBuckets.Items(idx).SubItems(2).Text = sharePur.ToString("0.00") & " %"
            lvBuckets.Items(idx).SubItems(3).Text = countWithLayout.ToString()
            lvBuckets.Items(idx).SubItems(4).Text = shareWithLayout.ToString("0.00") & " %"
            lvBuckets.Items(idx).SubItems(5).Text = MakeBar(sharePur) & " | " & MakeBar(shareWithLayout)

        Next

        lvBuckets.EndUpdate()
        Me.ResumeLayout()

    End Sub

    Private Shared Function MakeBar(percent As Double) As String

        Dim len As Integer = CInt(Math.Round(percent / 2.0R))

        If len < 0 Then
            len = 0
        ElseIf len > 50 Then
            len = 50
        End If

        Return New String("#"c, len)

    End Function

    Private Sub BtnStop_Click(sender As Object, e As EventArgs)
        StopMeasurement()
    End Sub

    Private Sub UiTimer_Tick(sender As Object, e As EventArgs)
        UpdateDisplay()
    End Sub

    Protected Overrides Sub OnFormClosing(e As FormClosingEventArgs)

        StopMeasurement()
        MyBase.OnFormClosing(e)

    End Sub

End Class