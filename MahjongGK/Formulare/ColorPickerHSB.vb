'
' SPDX-License-Identifier: GPL-3.0-or-later
'###########################################################################
'#                                                                         #
'#   Copyright © 2025–2026 Götz Kircher <MahjongGK@t-online.de>            #
'#                                                                         #
'#                     MahjongGK  -  Mahjong Solitär                       #
'#                                                                         #
'#   This program is free software: you can redistribute it and/or modify  #
'#   it under the terms of the GNU General Public License as published by  #
'#   the Free Software Foundation, either version 3 of the License, or     #
'#   at your option any later version.                                     #
'#                                                                         #
'#   This program is distributed in the hope that it will be useful,       #
'#   but WITHOUT ANY WARRANTY; without even the implied warranty of        #
'#   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the          #
'#   GNU General Public License for more details.                          #
'#   https://www.gnu.org/licenses/gpl-3.0.html                             #
'#                                                                         #
'###########################################################################
'
Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On
'
#Disable Warning IDE0079
#Disable Warning IDE1006

Imports System.ComponentModel
Imports System.Drawing.Drawing2D


'
''' <summary>
''' HSB-Farbwahl mit breitem Hue-Ring, Reglern für Sättigung/Helligkeit,
''' Info-Button, Grauskala-Hintergrundwahl und 6 Merkfarben (Drag and Drop).
''' ShowDialog gibt zurück DialogResult.Cancel oder DialogResult.None,
''' wenn Abbrechen gewählt oder nichts gewählt wurde, ansonsten DialogResult.OK
''' Für Stand-Alone Anwendung am Ende der Klasse ButtonInfo aktivieren.
''' </summary>
Public NotInheritable Class ColorPickerHSB
    Inherits Form

    ' ── Konstanten ─────────────────────────────────────────────────────────────
    Private Const RING_WIDTH As Integer = 80
    Private Const MEM_COLS As Integer = 2
    Private Const MEM_ROWS As Integer = 4
    Private Const MEM_COUNT As Integer = MEM_COLS * MEM_ROWS

    Private _initialColor As Color = Color.Empty

    ' ── Öffentliche API ────────────────────────────────────────────────────────
    '
    ''' <summary>Vom Benutzer übernommene Farbe (nur bei DialogResult.OK gesetzt).</summary>
    <Browsable(False)>
    Public Property SelectedColor As Color
        Get
            Return _selectedColor
        End Get
        Set(value As Color)
            _initialColor = value
        End Set
    End Property
    '
    ''' <summary>
    ''' Hintergrundfarbe des Pickers (Form + Farbkreis).
    ''' Wird auch über den Grauskala-Strip gesetzt.
    ''' </summary>
    <Browsable(True)>
    Public Property PickerBackColor As Color
        Get
            Return _pickerBackColor
        End Get
        Set(value As Color)
            If value.IsEmpty Then
                value = Color.FromArgb(182, 182, 182)
            End If
            _pickerBackColor = value
            Me.BackColor = value
            pbWheel.BackColor = value
            RebuildWheelBitmap()
            pbWheel.Invalidate()
            If pbGrayStrip IsNot Nothing Then pbGrayStrip.Invalidate()
        End Set
    End Property
    '
    ''' <summary>
    ''' CSV-String der 6 Merkfarben (RGB-Triplets, Semikolon-getrennt).
    ''' Format: "R,G,B;R,G,B;... (8x)". Leerer String ⇒ alle Merklabels löschen.
    ''' </summary>
    <Browsable(True)>
    Public Property SavedColorsString As String
        Get
            Return BuildSavedColorsString()
        End Get
        Set(value As String)
            ApplySavedColorsString(value)
        End Set
    End Property

    ' ── Private Felder ─────────────────────────────────────────────────────────
    Private _selectedColor As Color = Color.Empty
    Private _frozen As Boolean
    Private _frozenHue As Double ' 0..360 (gewählte Farbe)
    Private _hoverHue As Double  ' 0..360 (aktuelle Hover-Farbe)
    Private _saturation As Double = 1.0 ' 0..1
    Private _brightness As Double = 1.0 ' 0..1
    Private _wheelBmp As Bitmap ' Farbring-Cache für aktuelle S/B
    Private _showInitialMarker As Boolean = True
    Private _pickerBackColor As Color = SystemColors.Control

    ' ── Steuerelemente ─────────────────────────────────────────────────────────
    Private ReadOnly pbWheel As New PictureBox()

    Private ReadOnly lblStartCaption As New Label()
    Private ReadOnly swatchStart As New Label()

    Private ReadOnly lblCurrCaption As New Label()
    Private ReadOnly swatchCurr As New Label()

    Private ReadOnly lblFrozenCaption As New Label()
    Private ReadOnly swatchFrozen As New Label()

    Private ReadOnly lblSat As New Label()
    Private ReadOnly tbSat As New TrackBar()
    Private ReadOnly lblSatVal As New Label()

    Private ReadOnly lblBri As New Label()
    Private ReadOnly tbBri As New TrackBar()
    Private ReadOnly lblBriVal As New Label()

    Private ReadOnly btnAbbrechen As New Button()
    Private ReadOnly btnUebernehmen As New Button()
    Private ReadOnly btnReset As New Button()

    ' Info-Button & Grauskala
    Private ReadOnly btnInfo As New ButtonInfo()
    Private ReadOnly pbGrayStrip As New PictureBox()

    ' Merkfarben (2×3)
    Private ReadOnly memLabels As New List(Of Label)(MEM_COUNT)

    ' Hue-Center-Steuerung im Farbkreis
    Private ReadOnly lblHueCenter As New Label()
    Private ReadOnly btnHueMinus As New Button()
    Private ReadOnly btnHuePlus As New Button()


    ' ── Konstruktor ────────────────────────────────────────────────────────────
    '
    ''' <summary>Initialisiert die Form inkl. Merkfarben-Raster.</summary>
    Public Sub New()
        MyBase.New()
        Me.Text = "Farbwahl (HSB-Farbraum)"
        Me.StartPosition = FormStartPosition.CenterParent
        Me.FormBorderStyle = FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.ShowInTaskbar = False
        Me.KeyPreview = True

        Me.ClientSize = New Size(600, 560)

        BuildUi()
        BuildMemoryGrid()
        WireEvents()
    End Sub

    ' ── UI-Aufbau ───────────────────────────────────────────────────────────────
    '
    ''' <summary>Erzeugt und positioniert die Basis-Controls.</summary>
    Private Sub BuildUi()
        ' Farbkreis links oben
        pbWheel.Name = "pbWheel"
        pbWheel.Size = New Size(360, 360)
        pbWheel.Location = New Point(12, 12)
        pbWheel.Cursor = Cursors.Cross
        pbWheel.BackColor = _pickerBackColor
        pbWheel.BorderStyle = BorderStyle.FixedSingle
        pbWheel.SizeMode = PictureBoxSizeMode.Normal
        pbWheel.AllowDrop = False
        Me.Controls.Add(pbWheel)

        ' ── Hue-Center (im Farbkreis) ─────────────────────────────────────────────
        ' Parent in die PictureBox, damit es „mitwandert“
        lblHueCenter.Parent = pbWheel
        btnHueMinus.Parent = pbWheel
        btnHuePlus.Parent = pbWheel

        ' Label: aktuelle Hue-Anzeige (0–360°)
        lblHueCenter.AutoSize = False
        lblHueCenter.Size = New Size(70, 24)
        lblHueCenter.TextAlign = ContentAlignment.MiddleCenter
        lblHueCenter.BorderStyle = BorderStyle.FixedSingle
        lblHueCenter.BackColor = Color.FromArgb(220, 240, 240, 240) ' leicht transparent wirkend
        lblHueCenter.ForeColor = Color.Black
        lblHueCenter.Text = "0°"

        ' Buttons: Feinsteuerung
        btnHueMinus.Text = "−"
        btnHueMinus.Size = New Size(26, 26)
        btnHueMinus.TabStop = False

        btnHuePlus.Text = "+"
        btnHuePlus.Size = New Size(26, 26)
        btnHuePlus.TabStop = False

        AddHandler btnHueMinus.Click, AddressOf OnHueMinusClick
        AddHandler btnHuePlus.Click, AddressOf OnHuePlusClick

        ' Rechts oben: drei Farbfelder (Ausgang, Aktuell, Gewählt)
        Dim rightX As Integer = 390
        Dim swWidth As Integer = 180
        Dim swHeight As Integer = 28

        lblStartCaption.AutoSize = True
        lblStartCaption.Text = "Ausgangsfarbe"
        lblStartCaption.Location = New Point(rightX, 12)
        Me.Controls.Add(lblStartCaption)

        swatchStart.BorderStyle = BorderStyle.FixedSingle
        swatchStart.Size = New Size(swWidth, swHeight)
        swatchStart.Location = New Point(rightX, 32)
        swatchStart.TextAlign = ContentAlignment.MiddleCenter
        Me.Controls.Add(swatchStart)

        lblCurrCaption.AutoSize = True
        lblCurrCaption.Text = "Aktuelle Farbe"
        lblCurrCaption.Location = New Point(rightX, 72)
        Me.Controls.Add(lblCurrCaption)

        swatchCurr.BorderStyle = BorderStyle.FixedSingle
        swatchCurr.Size = New Size(swWidth, swHeight)
        swatchCurr.Location = New Point(rightX, 92)
        swatchCurr.TextAlign = ContentAlignment.MiddleCenter
        swatchCurr.AllowDrop = False
        AddHandler swatchCurr.MouseDown, AddressOf Swatch_MouseDown_StartDrag
        Me.Controls.Add(swatchCurr)

        lblFrozenCaption.AutoSize = True
        lblFrozenCaption.Text = "Gewählte Farbe"
        lblFrozenCaption.Location = New Point(rightX, 132)
        Me.Controls.Add(lblFrozenCaption)

        swatchFrozen.BorderStyle = BorderStyle.FixedSingle
        swatchFrozen.Size = New Size(swWidth, swHeight)
        swatchFrozen.Location = New Point(rightX, 152)
        swatchFrozen.TextAlign = ContentAlignment.MiddleCenter
        swatchFrozen.AllowDrop = False
        AddHandler swatchFrozen.MouseDown, AddressOf Swatch_MouseDown_StartDrag
        Me.Controls.Add(swatchFrozen)

        ' Info-Button
        btnInfo.Location = New Point(rightX + swWidth - 26, 190)
        btnInfo.Size = New Size(26, 26)
        btnInfo.InfoHeader = "Hilfe – Farbwahl (HSB Farbraum)"
        btnInfo.InfoText =
            "• Kreis = Farbton (Hue); Sättigung/Helligkeit per Regler unten." & Environment.NewLine &
            "• Maus über dem Ring zeigt 'Aktuelle Farbe'." & Environment.NewLine &
            "• Klick auf den Ring friert die Farbe ein ⇒ 'Gewählte Farbe'." & Environment.NewLine &
            "• Ziehe 'Aktuelle' oder 'Gewählte' Farbe per Drag & Drop auf ein Merkfeld." & Environment.NewLine &
            "• Klick auf ein Merkfeld stellt diese Farbe wieder her." & Environment.NewLine &
            "• Doppelklick auf ein Merkfeld löscht dieses." & Environment.NewLine &
            "• 'Übernehmen' übernimmt die gewählte Farbe." & Environment.NewLine &
            "• 'Reset' stellt die Ausgangsfarbe wieder her." & Environment.NewLine &
            "• Grauskala-Strip: Klick setzt die Hintergrundfarbe (Form + Farbkreis)." & Environment.NewLine &
            "• Hinweis: Schwarz, Grauwerte, Weiß mit Sättigung = 0% über die Helligkeit."

        Me.Controls.Add(btnInfo)

        ' Grauskala-Strip (Breite wie Swatches, halbe Höhe)
        pbGrayStrip.Name = "pbGrayStrip"
        pbGrayStrip.Size = New Size(swWidth, swHeight \ 2)
        pbGrayStrip.Location = New Point(rightX, 224)
        pbGrayStrip.BorderStyle = BorderStyle.FixedSingle
        pbGrayStrip.BackColor = _pickerBackColor
        pbGrayStrip.Cursor = Cursors.Hand
        AddHandler pbGrayStrip.Paint, AddressOf OnGrayStripPaint
        AddHandler pbGrayStrip.MouseDown, AddressOf OnGrayStripMouseDown
        Me.Controls.Add(pbGrayStrip)

        ' Slider Sättigung
        lblSat.AutoSize = True
        lblSat.Text = "Sättigung"
        lblSat.Location = New Point(12, 384)
        Me.Controls.Add(lblSat)

        tbSat.Name = "tbSat"
        tbSat.Location = New Point(12, 404)
        tbSat.Size = New Size(500, 45)
        tbSat.Minimum = 0
        tbSat.Maximum = 100
        tbSat.TickFrequency = 10
        tbSat.Value = 100
        tbSat.SmallChange = 1
        tbSat.LargeChange = 5
        Me.Controls.Add(tbSat)

        lblSatVal.AutoSize = True
        lblSatVal.Text = "100 %"
        lblSatVal.Location = New Point(520, 412)
        Me.Controls.Add(lblSatVal)

        ' Slider Helligkeit
        lblBri.AutoSize = True
        lblBri.Text = "Helligkeit"
        lblBri.Location = New Point(12, 452)
        Me.Controls.Add(lblBri)

        tbBri.Name = "tbBri"
        tbBri.Location = New Point(12, 472)
        tbBri.Size = New Size(500, 45)
        tbBri.Minimum = 0
        tbBri.Maximum = 100
        tbBri.TickFrequency = 10
        tbBri.Value = 100
        tbBri.SmallChange = 1
        tbBri.LargeChange = 5
        Me.Controls.Add(tbBri)

        lblBriVal.AutoSize = True
        lblBriVal.Text = "100 %"
        lblBriVal.Location = New Point(520, 480)
        Me.Controls.Add(lblBriVal)

        ' Buttons unten
        btnAbbrechen.Text = "Abbrechen"
        btnAbbrechen.Size = New Size(100, 30)
        btnAbbrechen.Location = New Point(Me.ClientSize.Width - 112, Me.ClientSize.Height - 44)
        btnAbbrechen.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        Me.Controls.Add(btnAbbrechen)

        btnUebernehmen.Text = "Übernehmen"
        btnUebernehmen.Size = New Size(100, 30)
        btnUebernehmen.Location = New Point(Me.ClientSize.Width - 224, Me.ClientSize.Height - 44)
        btnUebernehmen.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        btnUebernehmen.Enabled = False
        Me.Controls.Add(btnUebernehmen)

        btnReset.Text = "Reset"
        btnReset.Size = New Size(100, 30)
        btnReset.Location = New Point(12, Me.ClientSize.Height - 44)
        btnReset.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left
        Me.Controls.Add(btnReset)

        ' Anfangsposition (wird im Resize exakt mittig gesetzt)
        PositionHueCenterControls()


    End Sub
    '
    ''' <summary>Erzeugt das 2×3-Merkfeld-Raster unter der Grauskala.</summary>
    Private Sub BuildMemoryGrid()
        Dim startX As Integer = 390
        Dim startY As Integer = 224 + pbGrayStrip.Height + 8 ' unterhalb der Grauskala
        Dim cellW As Integer = 84
        Dim cellH As Integer = 24
        Dim gap As Integer = 8

        ' Zwei Spalten nebeneinander, drei Zeilen
        For r As Integer = 0 To MEM_ROWS - 1
            For c As Integer = 0 To MEM_COLS - 1
                Dim idx As Integer = r * MEM_COLS + c
                Dim lbl As New Label With {
                    .BorderStyle = BorderStyle.FixedSingle,
                    .Size = New Size(cellW, cellH),
                    .Location = New Point(startX + c * (cellW + gap), startY + r * (cellH + gap)),
                    .TextAlign = ContentAlignment.MiddleCenter,
                    .BackColor = SystemColors.Control,
                    .ForeColor = SystemColors.ControlText,
                    .Tag = idx,
                    .AllowDrop = True,
                    .Cursor = Cursors.Hand,
                    .Text = "—"
                }
                AddHandler lbl.MouseDown, AddressOf MemLabel_ClickOrDragStart
                AddHandler lbl.DragEnter, AddressOf MemLabel_DragEnter
                AddHandler lbl.DragDrop, AddressOf MemLabel_DragDrop
                AddHandler lbl.DoubleClick, AddressOf MemLabel_DoubleClick
                Me.Controls.Add(lbl)
                memLabels.Add(lbl)
            Next
        Next
    End Sub
    '
    ''' <summary>Verdrahtet Ereignisse.</summary>
    Private Sub WireEvents()
        AddHandler Me.Load, AddressOf OnFormLoad
        AddHandler Me.Shown, AddressOf OnFormShown

        AddHandler tbSat.ValueChanged, AddressOf OnSatChanged
        AddHandler tbBri.ValueChanged, AddressOf OnBriChanged

        AddHandler pbWheel.Paint, AddressOf OnWheelPaint
        AddHandler pbWheel.MouseMove, AddressOf OnWheelMouseMove
        AddHandler pbWheel.MouseLeave, AddressOf OnWheelMouseLeave
        AddHandler pbWheel.MouseDown, AddressOf OnWheelMouseDown
        AddHandler pbWheel.Resize, AddressOf OnWheelResize

        AddHandler btnAbbrechen.Click, Sub() Me.DialogResult = DialogResult.Cancel
        AddHandler btnUebernehmen.Click, AddressOf OnAccept
        AddHandler btnReset.Click, AddressOf OnReset
    End Sub

    ' ── Lifecycle ───────────────────────────────────────────────────────────────
    '
    ''' <summary>Initiale Werte setzen, Swatches füllen, Ring rendern.</summary>
    Private Sub OnFormLoad(sender As Object, e As EventArgs)
        Me.BackColor = _pickerBackColor
        pbWheel.BackColor = _pickerBackColor

        If IsEmptyOrTransparent(_initialColor) Then
            ' Vorgabe für „keine Startfarbe“
            _hoverHue = 0.0
            _frozenHue = 0.0
            _saturation = 1.0
            _brightness = 1.0
            _frozen = False
            _showInitialMarker = False
            btnUebernehmen.Enabled = False

            tbSat.Value = 100 : lblSatVal.Text = "100 %"
            tbBri.Value = 100 : lblBriVal.Text = "100 %"

            SetSwatchEmpty(swatchStart)
            SetSwatchEmpty(swatchCurr)
            UpdateFrozenSwatch(clearWhenNone:=True)

            RebuildWheelBitmap()
            pbGrayStrip.Invalidate()
            UpdateHueLabel()
            Return
        End If

        ' Normalfall mit definierter Startfarbe
        Dim h As Double, s As Double, b As Double
        RgbToHsv(_initialColor, h, s, b)
        _hoverHue = h
        _frozenHue = h
        _saturation = s
        _brightness = b

        tbSat.Value = CInt(Math.Round(_saturation * 100.0))
        tbBri.Value = CInt(Math.Round(_brightness * 100.0))
        lblSatVal.Text = tbSat.Value.ToString() & " %"
        lblBriVal.Text = tbBri.Value.ToString() & " %"

        swatchStart.BackColor = _initialColor
        swatchStart.Text = ColorToRgbString(_initialColor)
        swatchStart.ForeColor = IdealTextColor(_initialColor)

        UpdateCurrentSwatch(ColorFromHsb(_hoverHue, _saturation, _brightness))
        UpdateFrozenSwatch(clearWhenNone:=True)

        RebuildWheelBitmap()
        pbGrayStrip.Invalidate()
        UpdateHueLabel()
    End Sub

    '
    ''' <summary>Fokus auf den Farbkreis setzen.</summary>
    Private Sub OnFormShown(sender As Object, e As EventArgs)
        pbWheel.Focus()
    End Sub

    ' ── Slider-Events ──────────────────────────────────────────────────────────
    '
    ''' <summary>Sättigung geändert → Ring neu, Ausgangsmarker ausblenden, Hover updaten.</summary>
    Private Sub OnSatChanged(sender As Object, e As EventArgs)
        _saturation = tbSat.Value / 100.0
        lblSatVal.Text = tbSat.Value.ToString() & " %"
        _showInitialMarker = False
        RebuildWheelBitmap()
        UpdateHoverFromMouse()
        UpdateHueLabel()
    End Sub
    '
    ''' <summary>Helligkeit geändert → Ring neu, Ausgangsmarker ausblenden, Hover updaten.</summary>
    Private Sub OnBriChanged(sender As Object, e As EventArgs)
        _brightness = tbBri.Value / 100.0
        lblBriVal.Text = tbBri.Value.ToString() & " %"
        _showInitialMarker = False
        RebuildWheelBitmap()
        UpdateHoverFromMouse()
        UpdateHueLabel()
    End Sub

    ' ── Wheel-Events ───────────────────────────────────────────────────────────
    '
    ''' <summary>Zeichnet Ring (Bitmap) + Markierungen (gewählt/ggf. Ausgang).</summary>
    Private Sub OnWheelPaint(sender As Object, e As PaintEventArgs)
        If _wheelBmp IsNot Nothing Then
            e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor
            e.Graphics.PixelOffsetMode = PixelOffsetMode.Half
            e.Graphics.DrawImageUnscaled(_wheelBmp, 0, 0)
        End If

        If _frozen Then
            DrawAngleMarker(e.Graphics, _frozenHue, Pens.Black, Brushes.White)
        End If

        If _showInitialMarker AndAlso Not _frozen Then
            Dim h0 As Double, s0 As Double, b0 As Double
            RgbToHsv(_initialColor, h0, s0, b0)
            DrawAngleMarker(e.Graphics, h0, Pens.Gray, Brushes.LightGray)
        End If
    End Sub
    '
    ''' <summary>Hover → „Aktuelle Farbe“ aus Winkel ableiten.</summary>
    Private Sub OnWheelMouseMove(sender As Object, e As MouseEventArgs)
        Dim ok As Boolean
        Dim hue As Double = HueAtPoint(e.Location, ok)
        If ok Then
            _hoverHue = hue
            UpdateCurrentSwatch(ColorFromHsb(_hoverHue, _saturation, _brightness))
            UpdateHueLabel()
            pbWheel.Invalidate()
        End If
    End Sub
    '
    ''' <summary>Klick friert die Farbe ein → „Gewählte Farbe“ + Übernehmen aktiv.</summary>
    Private Sub OnWheelMouseDown(sender As Object, e As MouseEventArgs)
        Dim ok As Boolean
        Dim hue As Double = HueAtPoint(e.Location, ok)
        If ok Then
            _frozen = True
            _frozenHue = hue
            btnUebernehmen.Enabled = True
            _showInitialMarker = False
            UpdateFrozenSwatch(clearWhenNone:=False)
            UpdateHueLabel()
            pbWheel.Invalidate()
        End If
    End Sub
    '
    ''' <summary>Größenänderung → Bitmap neu rendern.</summary>
    Private Sub OnWheelResize(sender As Object, e As EventArgs)
        RebuildWheelBitmap()
        PositionHueCenterControls()
        pbWheel.Invalidate()
    End Sub
    '
    ''' <summary>
    ''' Maus hat den Farbkreis verlassen. Bewusst keine Aktion – die Anzeige bleibt stehen.
    ''' (Falls gewünscht, könnte man hier z. B. auf die zuletzt „gewählte“/eingefrorene Farbe zurückspringen.)
    ''' </summary>
    Private Sub OnWheelMouseLeave(sender As Object, e As EventArgs)
        ' keine Aktion nötig
    End Sub

    ' ── Buttons ────────────────────────────────────────────────────────────────
    '
    ''' <summary>„Übernehmen“ übernimmt die gewählte (eingefrorene) Farbe.</summary>
    Private Sub OnAccept(sender As Object, e As EventArgs)
        If Not _frozen Then
            Me.DialogResult = DialogResult.None
            Return
        End If
        _selectedColor = ColorFromHsb(_frozenHue, _saturation, _brightness)
        Me.DialogResult = DialogResult.OK
    End Sub
    '
    ''' <summary>„Reset“ setzt Ausgangsfarbe/Slider/Marker und leert Auswahl.</summary>
    Private Sub OnReset(sender As Object, e As EventArgs)
        If IsEmptyOrTransparent(_initialColor) Then
            _hoverHue = 0.0
            _frozenHue = 0.0
            _frozen = False
            btnUebernehmen.Enabled = False
            _showInitialMarker = False

            _saturation = 1.0
            _brightness = 1.0
            tbSat.Value = 100 : lblSatVal.Text = "100 %"
            tbBri.Value = 100 : lblBriVal.Text = "100 %"

            SetSwatchEmpty(swatchStart)
            SetSwatchEmpty(swatchCurr)
            UpdateFrozenSwatch(clearWhenNone:=True)

            RebuildWheelBitmap()
            pbWheel.Invalidate()
            UpdateHueLabel()
            Return
        End If

        ' Normalfall mit definierter Startfarbe
        Dim h As Double, s As Double, b As Double
        RgbToHsv(_initialColor, h, s, b)

        _hoverHue = h
        _frozenHue = h
        _frozen = False
        btnUebernehmen.Enabled = False
        _showInitialMarker = True

        _saturation = s
        _brightness = b
        tbSat.Value = CInt(Math.Round(_saturation * 100.0))
        tbBri.Value = CInt(Math.Round(_brightness * 100.0))
        lblSatVal.Text = tbSat.Value.ToString() & " %"
        lblBriVal.Text = tbBri.Value.ToString() & " %"

        UpdateCurrentSwatch(ColorFromHsb(_hoverHue, _saturation, _brightness))
        UpdateFrozenSwatch(clearWhenNone:=True)

        RebuildWheelBitmap()
        pbWheel.Invalidate()
        UpdateHueLabel()
    End Sub


    ' ── Grauskala-Strip ────────────────────────────────────────────────────────
    '
    ''' <summary>Zeichnet 8 Graustufen-Rechtecke (Schwarz → Weiß).</summary>
    Private Sub OnGrayStripPaint(sender As Object, e As PaintEventArgs)
        Dim rect As Rectangle = pbGrayStrip.ClientRectangle
        If rect.Width <= 0 OrElse rect.Height <= 0 Then Return

        Dim segW As Integer = Math.Max(1, rect.Width \ 8)
        For i As Integer = 0 To 7
            Dim gval As Integer = CInt(Math.Round(i * (255.0 / 7.0)))
            Using br As New SolidBrush(Color.FromArgb(gval, gval, gval))
                Dim r As New Rectangle(rect.X + i * segW, rect.Y, If(i = 7, rect.Right - (rect.X + i * segW), segW), rect.Height)
                e.Graphics.FillRectangle(br, r)
                e.Graphics.DrawRectangle(Pens.Gray, r)
            End Using
        Next
    End Sub
    '
    ''' <summary>Klick auf die Grauskala: setzt PickerBackColor (Form + Wheel).</summary>
    Private Sub OnGrayStripMouseDown(sender As Object, e As MouseEventArgs)
        Dim rect As Rectangle = pbGrayStrip.ClientRectangle
        If rect.Width <= 0 Then Return
        Dim idx As Integer = Math.Min(7, Math.Max(0, (e.X * 8) \ Math.Max(1, rect.Width)))
        Dim gval As Integer = CInt(Math.Round(idx * (255.0 / 7.0)))
        Me.PickerBackColor = Color.FromArgb(gval, gval, gval)
    End Sub

    ' ── Merkfarben: Drag & Drop / Klick ────────────────────────────────────────
    '
    ''' <summary>Startet Drag and Drop von einer Swatch (Aktuell/Gewählt).</summary>
    Private Sub Swatch_MouseDown_StartDrag(sender As Object, e As MouseEventArgs)
        If e.Button <> MouseButtons.Left Then Return
        Dim src As Label = DirectCast(sender, Label)
        Dim col As Color = src.BackColor
        ' Nichts ziehen, wenn leer (—)
        If src.Text = "—" Then Return

        Dim data As New DataObject()
        data.SetData(GetType(Color), col)
        data.SetText(ColorToRgbString(col))
        src.DoDragDrop(data, DragDropEffects.Copy)
    End Sub
    '
    ''' <summary>Klick oder Drag-Start auf Merklabel: Linksklick = anwenden, DragStart ignoriert.</summary>
    Private Sub MemLabel_ClickOrDragStart(sender As Object, e As MouseEventArgs)
        Dim lbl As Label = DirectCast(sender, Label)
        If e.Button = MouseButtons.Left AndAlso e.Clicks = 1 Then
            If lbl.Text <> "—" Then
                ' Farbe anwenden wie ein Klick im Ring (wird "gewählte" Farbe)
                Dim c As Color = lbl.BackColor
                ApplyColorAsSelected(c)
            End If
        End If
        ' DragStart von Merklabels ist nicht vorgesehen (nur Drop-Ziel)
    End Sub
    '
    ''' <summary>DragEnter: prüfen, ob Color/Text ankommt.</summary>
    Private Sub MemLabel_DragEnter(sender As Object, e As DragEventArgs)
        If e.Data Is Nothing Then Return
        If e.Data.GetDataPresent(GetType(Color)) OrElse e.Data.GetDataPresent(DataFormats.Text) Then
            e.Effect = DragDropEffects.Copy
        Else
            e.Effect = DragDropEffects.None
        End If
    End Sub
    '
    ''' <summary>DragDrop: Farbe ins Merklabel übernehmen (BackColor + RGB-Text).</summary>
    Private Sub MemLabel_DragDrop(sender As Object, e As DragEventArgs)
        Dim lbl As Label = DirectCast(sender, Label)
        Dim c As Color

        If e.Data.GetDataPresent(GetType(Color)) Then
            c = DirectCast(e.Data.GetData(GetType(Color)), Color)
        ElseIf e.Data.GetDataPresent(DataFormats.Text) Then
            Dim s As String = TryCast(e.Data.GetData(DataFormats.Text), String)
            If Not TryParseRgbTriplet(s, c) Then Return
        Else
            Return
        End If

        lbl.BackColor = c
        lbl.Text = ColorToRgbString(c)
        lbl.ForeColor = IdealTextColor(c)
    End Sub
    '
    ''' <summary>
    ''' Doppelklick auf ein Merklabel setzt es zurück auf „leer“.
    ''' </summary>
    Private Sub MemLabel_DoubleClick(sender As Object, e As EventArgs)
        Dim lbl As Label = DirectCast(sender, Label)
        lbl.BackColor = SystemColors.Control
        lbl.Text = "—"
        lbl.ForeColor = SystemColors.ControlText
    End Sub


    ' ── Rendering / Geometrie ──────────────────────────────────────────────────
    '
    ''' <summary>Erzeugt/aktualisiert die Bitmap des Hue-Rings für aktuelle S/B (breiter Ring).</summary>
    Private Sub RebuildWheelBitmap()
        If pbWheel.Width < 10 OrElse pbWheel.Height < 10 Then
            _wheelBmp = Nothing
            Return
        End If

        Dim w As Integer = pbWheel.Width
        Dim h As Integer = pbWheel.Height
        Dim size As Integer = Math.Min(w, h)
        Dim cx As Integer = size \ 2
        Dim cy As Integer = size \ 2
        Dim radius As Integer = (size \ 2) - 2
        Dim inner As Integer = Math.Max(0, radius - RING_WIDTH)

        Dim bmp As New Bitmap(size, size, Imaging.PixelFormat.Format24bppRgb)

        For y As Integer = 0 To size - 1
            Dim dy As Double = y - cy
            For x As Integer = 0 To size - 1
                Dim dx As Double = x - cx
                Dim r As Double = Math.Sqrt(dx * dx + dy * dy)
                If r >= inner AndAlso r <= radius Then
                    Dim angle As Double = Math.Atan2(dy, dx)
                    Dim hue As Double = (angle * 180.0 / Math.PI) + 90
                    If hue < 0 Then hue += 360.0
                    Dim c As Color = ColorFromHsb(hue, _saturation, _brightness)
                    bmp.SetPixel(x, y, c)
                Else
                    bmp.SetPixel(x, y, _pickerBackColor)
                End If
            Next
        Next

        Dim old As Bitmap = _wheelBmp
        _wheelBmp = bmp
        old?.Dispose()
    End Sub
    '
    ''' <summary>Hue (0..360) am Punkt im Ring; ok=False, wenn außerhalb.</summary>
    Private Function HueAtPoint(p As Point, ByRef ok As Boolean) As Double
        ok = False
        If _wheelBmp Is Nothing Then Return 0.0

        Dim size As Integer = Math.Min(pbWheel.Width, pbWheel.Height)
        Dim cx As Integer = size \ 2
        Dim cy As Integer = size \ 2
        Dim radius As Integer = (size \ 2) - 2
        Dim inner As Integer = Math.Max(0, radius - RING_WIDTH)

        Dim dx As Double = p.X - cx
        Dim dy As Double = p.Y - cy
        Dim r As Double = Math.Sqrt(dx * dx + dy * dy)
        If r < inner OrElse r > radius Then Return 0.0

        Dim angle As Double = Math.Atan2(dy, dx)
        Dim hue As Double = (angle * 180.0 / Math.PI) + 90
        If hue < 0 Then hue += 360.0
        ok = True
        Return hue
    End Function
    '
    ''' <summary>Markierungskreis am Ring (für Ausgang/gewählt).</summary>
    Private Sub DrawAngleMarker(g As Graphics, hue As Double, pen As Pen, fill As Brush)
        Dim size As Integer = Math.Min(pbWheel.Width, pbWheel.Height)
        Dim cx As Single = size / 2.0F
        Dim cy As Single = size / 2.0F
        Dim radius As Single = (size / 2.0F) - (RING_WIDTH / 2.0F) - 2.0F

        Dim rad As Double = (hue * Math.PI / 180.0) - (Math.PI / 2)
        Dim px As Single = cx + CSng(Math.Cos(rad) * radius)
        Dim py As Single = cy + CSng(Math.Sin(rad) * radius)

        Dim d As Single = 8.0F
        Dim rect As New RectangleF(px - d / 2.0F, py - d / 2.0F, d, d)
        g.SmoothingMode = SmoothingMode.AntiAlias
        g.FillEllipse(fill, rect)
        g.DrawEllipse(pen, rect)
    End Sub

    ' ── Swatch-Updates ─────────────────────────────────────────────────────────
    '
    ''' <summary>Aktuelle (Hover-)Farbe aktualisieren.</summary>
    Private Sub UpdateCurrentSwatch(c As Color)
        swatchCurr.BackColor = c
        swatchCurr.Text = ColorToRgbStringX(c)
        swatchCurr.ForeColor = IdealTextColor(c)
    End Sub
    '
    ''' <summary>Gewählte (eingefrorene) Farbe aktualisieren oder leeren.</summary>
    Private Sub UpdateFrozenSwatch(clearWhenNone As Boolean)
        If _frozen Then
            Dim c As Color = ColorFromHsb(_frozenHue, _saturation, _brightness)
            swatchFrozen.BackColor = c
            swatchFrozen.Text = ColorToRgbStringX(c)
            swatchFrozen.ForeColor = IdealTextColor(c)
        ElseIf clearWhenNone Then
            swatchFrozen.BackColor = SystemColors.Control
            swatchFrozen.Text = "—"
            swatchFrozen.ForeColor = SystemColors.ControlText
        End If
    End Sub
    '
    ''' <summary>Nach S/B-Änderung Hover-Farbe neu bestimmen, ggf. gewählte refreschen.</summary>
    Private Sub UpdateHoverFromMouse()
        Dim p As Point = pbWheel.PointToClient(Control.MousePosition)
        Dim ok As Boolean
        Dim hue As Double = HueAtPoint(p, ok)
        If ok Then _hoverHue = hue
        UpdateCurrentSwatch(ColorFromHsb(_hoverHue, _saturation, _brightness))
        If _frozen Then UpdateFrozenSwatch(clearWhenNone:=False)
        pbWheel.Invalidate()
    End Sub

    ' ── Farbe anwenden / Helper ────────────────────────────────────────────────
    '
    ''' <summary>
    ''' Wählt für eine gegebene Hintergrundfarbe eine gut lesbare Textfarbe (Schwarz/Weiß).
    ''' Nutzung der YIQ-Helligkeitsformel als einfacher, robuster Heuristik.
    ''' </summary>
    Private Function IdealTextColor(bg As Color) As Color
        Dim yiq As Integer = ((bg.R * 299) + (bg.G * 587) + (bg.B * 114)) \ 1000
        Return If(yiq >= 128, Color.Black, Color.White)
    End Function
    '
    ''' <summary>Setzt eine gegebene RGB-Farbe als „gewählt“ inkl. Slider/Hue/Marker.</summary>
    Private Sub ApplyColorAsSelected(c As Color)
        Dim h As Double, s As Double, v As Double
        RgbToHsv(c, h, s, v)

        _hoverHue = h
        _frozenHue = h
        _frozen = True
        btnUebernehmen.Enabled = True
        _showInitialMarker = False

        _saturation = s
        _brightness = v
        tbSat.Value = CInt(Math.Round(_saturation * 100.0))
        tbBri.Value = CInt(Math.Round(_brightness * 100.0))
        lblSatVal.Text = tbSat.Value.ToString() & " %"
        lblBriVal.Text = tbBri.Value.ToString() & " %"

        UpdateCurrentSwatch(ColorFromHsb(_hoverHue, _saturation, _brightness))
        UpdateFrozenSwatch(clearWhenNone:=False)

        RebuildWheelBitmap()
        pbWheel.Invalidate()
    End Sub

    ' ── Farb-Helfer ────────────────────────────────────────────────────────────
    '
    ''' <summary>HSV → RGB (ohne Alpha), h=0..360, s/v=0..1.</summary>
    Private Function ColorFromHsb(h As Double, s As Double, v As Double) As Color
        Dim r As Integer, g As Integer, b As Integer
        HsvToRgb(h, s, v, r, g, b)
        Return Color.FromArgb(r, g, b)
    End Function
    '
    ''' <summary>HSV → RGB.</summary>
    Private Sub HsvToRgb(h As Double, s As Double, v As Double, ByRef r As Integer, ByRef g As Integer, ByRef b As Integer)
        If s <= 0.0 Then
            Dim vi As Integer = CInt(Math.Round(v * 255.0))
            r = vi : g = vi : b = vi
            Return
        End If
        Dim hh As Double = (h Mod 360.0) / 60.0
        Dim i As Integer = CInt(Math.Floor(hh))
        Dim ff As Double = hh - i
        Dim p As Double = v * (1.0 - s)
        Dim q As Double = v * (1.0 - (s * ff))
        Dim t As Double = v * (1.0 - (s * (1.0 - ff)))

        Dim rd As Double, gd As Double, bd As Double
        Select Case i
            Case 0 : rd = v : gd = t : bd = p
            Case 1 : rd = q : gd = v : bd = p
            Case 2 : rd = p : gd = v : bd = t
            Case 3 : rd = p : gd = q : bd = v
            Case 4 : rd = t : gd = p : bd = v
            Case Else : rd = v : gd = p : bd = q
        End Select

        r = CInt(Math.Round(rd * 255.0))
        g = CInt(Math.Round(gd * 255.0))
        b = CInt(Math.Round(bd * 255.0))
    End Sub
    '
    ''' <summary>RGB → HSV (h=0..360, s/v=0..1).</summary>
    Private Sub RgbToHsv(c As Color, ByRef h As Double, ByRef s As Double, ByRef v As Double)
        Dim r As Double = c.R / 255.0
        Dim g As Double = c.G / 255.0
        Dim b As Double = c.B / 255.0

        Dim maxv As Double = Math.Max(r, Math.Max(g, b))
        Dim minv As Double = Math.Min(r, Math.Min(g, b))
        v = maxv

        Dim d As Double = maxv - minv
        s = If(maxv = 0.0, 0.0, d / maxv)

        If d = 0.0 Then
            h = 0.0
        Else
            If maxv = r Then
                h = 60.0 * (((g - b) / d) Mod 6.0)
            ElseIf maxv = g Then
                h = 60.0 * (((b - r) / d) + 2.0)
            Else
                h = 60.0 * (((r - g) / d) + 4.0)
            End If
            If h < 0.0 Then h += 360.0
        End If
    End Sub
    '
    ''' <summary>RGB → "R,G,B".</summary>
    Private Function ColorToRgbStringX(c As Color) As String
        Return $"{c.R},{c.G},{c.B} - #{c.R:X2}{c.G:X2}{c.B:X2}"
    End Function
    Private Function ColorToRgbString(c As Color) As String
        Return $"{c.R},{c.G},{c.B}"
    End Function
    '
    ''' <summary>Parst "R,G,B" robust (0..255).</summary>
    Private Function TryParseRgbTriplet(s As String, ByRef c As Color) As Boolean
        If String.IsNullOrWhiteSpace(s) Then Return False
        Dim parts As String() = s.Trim().Split(","c)
        If parts.Length <> 3 Then Return False
        Dim r As Integer, g As Integer, b As Integer
        If Integer.TryParse(parts(0).Trim(), r) AndAlso
           Integer.TryParse(parts(1).Trim(), g) AndAlso
           Integer.TryParse(parts(2).Trim(), b) Then
            If r < 0 OrElse r > 255 OrElse g < 0 OrElse g > 255 OrElse b < 0 OrElse b > 255 Then Return False
            c = Color.FromArgb(r, g, b)
            Return True
        End If
        Return False
    End Function

    ' ── SavedColorsString Handling ─────────────────────────────────────────────
    '
    ''' <summary>Erzeugt den CSV-String aus den 6 Merklabels.</summary>
    Private Function BuildSavedColorsString() As String
        Dim items As New List(Of String)(MEM_COUNT)
        For Each lbl As Label In memLabels
            If lbl.Text = "—" Then
                items.Add("") ' leeres Feld
            Else
                items.Add(lbl.Text) ' R,G,B
            End If
        Next
        ' Normalisieren: genau 6 Teile; leere bleiben leer
        Return String.Join(";", items)
    End Function
    '
    ''' <summary>Setzt die 6 Merklabels aus einem CSV-String. Leer ⇒ alles löschen.</summary>
    Private Sub ApplySavedColorsString(value As String)
        If String.IsNullOrWhiteSpace(value) Then
            ' Alles löschen
            For Each lbl As Label In memLabels
                lbl.BackColor = SystemColors.Control
                lbl.Text = "—"
                lbl.ForeColor = SystemColors.ControlText
            Next
            Return
        End If

        Dim parts As String() = value.Split(";"c)
        Dim n As Integer = Math.Min(MEM_COUNT, parts.Length)
        For i As Integer = 0 To MEM_COUNT - 1
            Dim lbl As Label = memLabels(i)
            If i < n AndAlso Not String.IsNullOrWhiteSpace(parts(i)) Then
                Dim c As Color
                If TryParseRgbTriplet(parts(i), c) Then
                    lbl.BackColor = c
                    lbl.Text = ColorToRgbString(c)
                    lbl.ForeColor = IdealTextColor(c)
                Else
                    ' ungültig → löschen
                    lbl.BackColor = SystemColors.Control
                    lbl.Text = "—"
                    lbl.ForeColor = SystemColors.ControlText
                End If
            Else
                lbl.BackColor = SystemColors.Control
                lbl.Text = "—"
                lbl.ForeColor = SystemColors.ControlText
            End If
        Next
    End Sub

    '
    ''' <summary>
    ''' Positioniert Minus-Button, Hue-Label und Plus-Button zentriert in der PictureBox.
    ''' </summary>
    Private Sub PositionHueCenterControls()
        If pbWheel.Width <= 0 OrElse pbWheel.Height <= 0 Then Return

        ' Gesamtlayout: [ − ] [   HueLabel   ] [ ＋ ]
        Dim gap As Integer = 6
        Dim minusSize As Size = btnHueMinus.Size
        Dim plusSize As Size = btnHuePlus.Size
        Dim labelSize As Size = lblHueCenter.Size

        Dim totalW As Integer = minusSize.Width + gap + labelSize.Width + gap + plusSize.Width
        Dim y As Integer = (pbWheel.Height - Math.Max(Math.Max(minusSize.Height, plusSize.Height), labelSize.Height)) \ 2
        Dim x As Integer = (pbWheel.Width - totalW) \ 2

        btnHueMinus.Location = New Point(x, y)
        lblHueCenter.Location = New Point(x + minusSize.Width + gap, y + (minusSize.Height - labelSize.Height) \ 2)
        btnHuePlus.Location = New Point(lblHueCenter.Right + gap, y)
    End Sub

    '
    ''' <summary>
    ''' Aktualisiert die Hue-Anzeige (°) – zeigt die relevante Hue an:
    ''' eingefroren ⇒ _frozenHue, sonst _hoverHue.
    ''' </summary>
    Private Sub UpdateHueLabel()
        Dim h As Integer = CInt(Math.Round(If(_frozen, _frozenHue, _hoverHue))) Mod 360
        If h < 0 Then h += 360
        lblHueCenter.Text = h.ToString() & "°"
    End Sub

    '
    ''' <summary>
    ''' Normalisiert eine Hue in den Bereich 0..360.
    ''' </summary>
    Private Function NormalizeHue(h As Double) As Double
        h = h Mod 360.0
        If h < 0 Then h += 360.0
        Return h
    End Function

    '
    ''' <summary>
    ''' Erhöht/vermindert den Hue um delta (in Grad). Bei eingefrorener Auswahl
    ''' wird _frozenHue angepasst, sonst _hoverHue. Swatches + Ring werden aktualisiert.
    ''' </summary>
    Private Sub NudgeHue(delta As Integer)
        If _frozen Then
            _frozenHue = NormalizeHue(_frozenHue + delta)
        Else
            _hoverHue = NormalizeHue(_hoverHue + delta)
        End If

        ' Aktuelle Farbe aus der „aktiven“ Hue ableiten
        Dim activeHue As Double = If(_frozen, _frozenHue, _hoverHue)
        Dim c As Color = ColorFromHsb(activeHue, _saturation, _brightness)

        UpdateCurrentSwatch(c)
        If _frozen Then UpdateFrozenSwatch(clearWhenNone:=False)

        UpdateHueLabel()
        pbWheel.Invalidate()
    End Sub

    '
    ''' <summary>Minus-Button: −1°.</summary>
    Private Sub OnHueMinusClick(sender As Object, e As EventArgs)
        NudgeHue(-1)
    End Sub

    '
    ''' <summary>Plus-Button: +1°.</summary>
    Private Sub OnHuePlusClick(sender As Object, e As EventArgs)
        NudgeHue(+1)
    End Sub

    '
    ''' <summary>True, wenn keine definierte Startfarbe vorliegt.</summary>
    Private Function IsEmptyOrTransparent(c As Color) As Boolean
        Return c.IsEmpty OrElse c.A = 0
    End Function

    '
    ''' <summary>Setzt ein Swatch-Label in den „leer“-Zustand.</summary>
    Private Sub SetSwatchEmpty(lbl As Label)
        lbl.BackColor = SystemColors.Control
        lbl.Text = "—"
        lbl.ForeColor = SystemColors.ControlText
    End Sub

    ' ── Aufräumen ──────────────────────────────────────────────────────────────
    '
    ''' <summary>Bitmap freigeben.</summary>
    Protected Overrides Sub Dispose(disposing As Boolean)
        If disposing Then
            _wheelBmp?.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub


    'Für Stand-Alone Anwendung aktivieren
    '
    '' ── Inneres Hilfs-Control: ButtonInfo ──────────────────────────────────────
    ''
    '''' <summary>
    '''' Kleiner Info-Button (26×26) mit zwei Properties:
    '''' - InfoHeader (Titel der MessageBox)
    '''' - InfoText (Inhalt der MessageBox)
    '''' Klick zeigt eine MessageBox an.
    '''' </summary>
    'Friend NotInheritable Class ButtonInfo
    '    Inherits Button

    '    Private _infoHeader As String = "Info"
    '    Private _infoText As String = "(Keine Hilfe hinterlegt.)"

    '    '''
    '    '
    '    ''' <summary>Überschrift der Hilfe.</summary>
    '    <Browsable(True)>
    '    Public Property InfoHeader As String
    '        Get
    '            Return _infoHeader
    '        End Get
    '        Set(value As String)
    '            _infoHeader = If(value, String.Empty)
    '        End Set
    '    End Property
    '    '
    '    ''' <summary>Hilfetext.</summary>
    '    <Browsable(True)>
    '    Public Property InfoText As String
    '        Get
    '            Return _infoText
    '        End Get
    '        Set(value As String)
    '            _infoText = If(value, String.Empty)
    '        End Set
    '    End Property
    '    '
    '    ''' <summary>Konstruktor: fixiert Größe 26×26, schlichtes „i“.</summary>
    '    Public Sub New()
    '        MyBase.New()
    '        Me.Text = "i"
    '        Me.Font = New Font(SystemFonts.DefaultFont.FontFamily, 10.0F, FontStyle.Bold, GraphicsUnit.Point)
    '        Me.Size = New Size(26, 26)
    '        Me.MinimumSize = New Size(26, 26)
    '        Me.MaximumSize = New Size(26, 26)
    '        Me.FlatStyle = FlatStyle.System
    '        AddHandler Me.Click, AddressOf OnInfoClick
    '    End Sub
    '    '
    '    ''' <summary>Erzwingt immer 26×26.</summary>
    '    Protected Overrides Sub SetBoundsCore(x As Integer, y As Integer, width As Integer, height As Integer, specified As BoundsSpecified)
    '        MyBase.SetBoundsCore(x, y, 26, 26, specified)
    '    End Sub
    '    '
    '    ''' <summary>Zeigt die Hilfe an.</summary>
    '    Private Sub OnInfoClick(sender As Object, e As EventArgs)
    '        MessageBox.Show(Me, _infoText, _infoHeader, MessageBoxButtons.OK, MessageBoxIcon.Information)
    '    End Sub
    'End Class

End Class
