'
' SPDX-License-Identifier: GPL-3.0-or-later
'###########################################################################
'#                                                                         #
'#   Copyright © 2025–2026 Götz Kircher <mahjonggk@t-online.de>            #
'#                                                                         #
'#                     MahjongGK  -  Mahjong Solitär                       #
'#                                                                         #
'#   This program is free software: you can redistribute it and/or modify  #
'#   it under the terms of the GNU General Public License as published by  #
'#   the Free Software Fundament, either version 3 of the License, or     #
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
'
Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

#Disable Warning IDE0079
#Disable Warning IDE1006

Imports System.Drawing.Text
Imports System.Reflection

' 
''' <summary>
''' MessageBox-ähnliche Form mit CRLF-Formattreue, RichTextBox, Kopieren-Kontextmenü,
''' zentriertem Header und Größenbegrenzung (Scrollbars > 2/3 Screen).
''' API: Show(...) Overloads analog MessageBox.
''' </summary>
Public NotInheritable Class MessageBoxFormatiert
    Inherits Form

    '--- Felder
    Private ReadOnly _headerLabel As New Label()
    Private ReadOnly _iconBox As New PictureBox()
    Private ReadOnly _rtb As New RichTextBox()
    Private ReadOnly _buttonsPanel As New FlowLayoutPanel()
    Private _result As DialogResult = DialogResult.None

    '-- Fonts (kommen idealerweise von INI.*; Fallback auf Arial 8.25)
    Private _fontHeader As Font
    Private _fontMessage As Font
    Private Shared _useMonospacedFont As Boolean

    Private _flowMode As Boolean  ' True = Fließtext (MessageBox-ähnlich, WordWrap)
    ' … bestehende Felder …
    Private _widthHintPx As Integer = 0 ' Breiten-Hinweis aus erster Zeile via "|"
    Private _rtbWidthCapPx As Integer = 0         ' tatsächlich anzuwendende Obergrenze für _rtb.Width


    '--- Konstanten/Layout
    Private Shadows Const PADDING As Integer = 12
    Private Const GAP As Integer = 10
    Private Const ICON_SIZE As Integer = 32
    Private Const BUTTON_GAP As Integer = 8
    Private Const MIN_CLIENT_W As Integer = 360
    Private Const MIN_CLIENT_H As Integer = 160

    Public Const MEASURE_FUDGE_W As Integer = 16
    Public Const MEASURE_FUDGE_H As Integer = 6
    '

    Private Enum SnapMode
        None
        FullHeight
        FullWidth
        FullBoth
    End Enum

    Private _snapMode As SnapMode = SnapMode.None
    Private _prevBounds As Rectangle
    ' --- Snap-Zustände (unabhängig)
    Private _snapWidth As Boolean = False
    Private _snapHeight As Boolean = False
    Private _baseBounds As Rectangle = Rectangle.Empty ' Original, auf den wir zurückstellen


    ''' <summary>
    ''' PRIVATER Konstruktor – benutze die Shared Show(...) Overloads.
    ''' </summary>
    Private Sub New()
        Me.FormBorderStyle = FormBorderStyle.SizableToolWindow
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.ShowInTaskbar = False
        Me.TopMost = Not Me.TopMost
        Me.BackColor = INI.InfoMessageBox_FormBackColor

        ' Titelzeile = Assembly-Name
        Me.Text = Assembly.GetExecutingAssembly().GetName().Name

        ' Icon
        _iconBox.SizeMode = PictureBoxSizeMode.CenterImage
        _iconBox.Width = ICON_SIZE
        _iconBox.Height = ICON_SIZE
        _iconBox.Cursor = Cursors.Hand

        ' Undokumentiert: Icon-Klick toggelt TopMost + visuelles Feedback
        AddHandler _iconBox.Click, AddressOf IconBox_Click
        AddHandler _iconBox.Paint, AddressOf IconBox_Paint

        ' Header
        _headerLabel.TextAlign = ContentAlignment.MiddleCenter
        _headerLabel.AutoSize = False
        _headerLabel.Cursor = Cursors.Hand

        AddHandler _headerLabel.MouseClick, AddressOf HeaderLabel_MouseClick

        ' RichTextBox
        _rtb.ReadOnly = True
        _rtb.BorderStyle = BorderStyle.None
        _rtb.WordWrap = False
        _rtb.ScrollBars = RichTextBoxScrollBars.None
        _rtb.DetectUrls = False
        _rtb.ShortcutsEnabled = True
        _rtb.BackColor = INI.InfoMessageBox_TextBackColor


        ' Kontextmenü "Kopieren" (mit kurzer Sichtbarkeit der Auswahl)
        Dim cms As New ContextMenuStrip()
        Dim miCopy As New ToolStripMenuItem("Kopieren")

        AddHandler miCopy.Click,
            Sub(sender As Object, e As EventArgs)
                Dim hadSelection As Boolean = (_rtb.SelectionLength > 0)
                Dim oldStart As Integer = _rtb.SelectionStart
                Dim oldLen As Integer = _rtb.SelectionLength
                Dim oldHide As Boolean = _rtb.HideSelection

                If hadSelection Then
                    ' Normales Kopieren bei vorhandener Auswahl
                    _rtb.Copy()
                Else
                    ' Keine Auswahl: kurzzeitig Auswahl sichtbar machen
                    _rtb.HideSelection = False
                    _rtb.Focus()               ' sicherstellen, dass die Markierung sichtbar ist
                    _rtb.SelectAll()
                    _rtb.Copy()

                    ' Nach 500 ms Auswahl zurücknehmen und Zustand wiederherstellen
                    Dim t As New Timer() With {.Interval = 500}
                    AddHandler t.Tick,
                Sub()
                    t.Stop()
                    t.Dispose()
                    ' Auswahl zurück auf ursprüngliche Caret-Position
                    _rtb.SelectionStart = oldStart
                    _rtb.SelectionLength = 0
                    _rtb.HideSelection = oldHide
                End Sub
                    t.Start()
                End If
            End Sub

        cms.Items.Add(miCopy)
        _rtb.ContextMenuStrip = cms

        ' (optional, reines Komfort-Feedback für den Cursor)
        AddHandler _headerLabel.MouseMove,
    Sub(s, e)
        Dim lbl As Label = DirectCast(s, Label)
        Dim w As Integer = lbl.ClientSize.Width : If w <= 0 Then Return
        Dim x As Integer = If(lbl.RightToLeft = RightToLeft.Yes, w - e.X, e.X)
        Dim third As Integer = w \ 3
        lbl.Cursor = If(x < third, Cursors.SizeNS, If(x < 2 * third, Cursors.SizeAll, Cursors.SizeWE))
    End Sub
        AddHandler _headerLabel.MouseLeave, Sub(s, e) _headerLabel.Cursor = Cursors.Default
        AddHandler Me.Resize, Sub(sender, e) LayoutControls()

        ' Buttons Panel (rechts-unten)
        _buttonsPanel.FlowDirection = FlowDirection.RightToLeft
        _buttonsPanel.WrapContents = False
        _buttonsPanel.AutoSize = True
        _buttonsPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink
        _buttonsPanel.Padding = System.Windows.Forms.Padding.Empty
        _buttonsPanel.Margin = New Padding(0)

        ' Icon bleibt links oben
        _iconBox.Anchor = AnchorStyles.Top Or AnchorStyles.Left

        ' Header soll oben über die Breite laufen
        _headerLabel.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right

        ' RichTextBox soll in beide Richtungen mitwachsen
        _rtb.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right Or AnchorStyles.Bottom

        ' Button-Panel bleibt rechts unten
        _buttonsPanel.Anchor = AnchorStyles.Right Or AnchorStyles.Bottom

        ' Grund-Layout (manuell; simpel und robust)
        Controls.Add(_iconBox)
        Controls.Add(_headerLabel)
        Controls.Add(_rtb)
        Controls.Add(_buttonsPanel)

        AddHandler Me.ResizeBegin, Sub(sender, e) _snapMode = SnapMode.None

    End Sub

    '
    ''' <summary>
    ''' Zentrale Show-Core (übernimmt alle Parameter).
    ''' </summary>
    Private Shared Function ShowCore(owner As IWin32Window,
                                     text As String,
                                     caption As String,
                                     buttons As MessageBoxButtons,
                                     icon As MessageBoxIcon,
                                     defaultButton As MessageBoxDefaultButton,
                                     options As MessageBoxOptions,
                                     Optional useMonospacedFont As Boolean = False) As DialogResult

        _useMonospacedFont = useMonospacedFont

        Dim dlg As New MessageBoxFormatiert()
        Try
            ' Header-Text (Fenstertitel bleibt Assembly-Name)
            dlg._headerLabel.Text = If(caption, String.Empty)

            ' Fonts (INI oder Fallback)
            If _useMonospacedFont Then
                dlg._fontMessage = GetSafeFont(If(INI.InfoMessageBox_FontMessageMonoSpaced, Nothing), New Font("Arial", 8.25F, FontStyle.Regular))
                dlg._fontHeader = GetSafeFont(If(INI.InfoMessageBox_FontHeaderMonoSpaced, Nothing), New Font("Arial", 8.25F, FontStyle.Bold))
            Else
                dlg._fontMessage = GetSafeFont(If(INI.InfoMessageBox_FontMessage, Nothing), New Font("Arial", 8.25F, FontStyle.Regular))
                dlg._fontHeader = GetSafeFont(If(INI.InfoMessageBox_FontHeader, Nothing), New Font("Arial", 8.25F, FontStyle.Bold))
            End If

            dlg._headerLabel.Font = dlg._fontHeader
            dlg._rtb.Font = dlg._fontMessage

            ' --- Hintertür: kein CR/LF -> Fließtext (WordWrap), "~" erzwingt Umbruch ---
            Dim rawText As String = If(text, String.Empty)

            ' 3. Regel (neu): In der ERSTEN Zeile ein "|" -> Breitenhinweis messen und dieses eine "|" entfernen
            rawText = dlg.ProcessWidthHintPipe(rawText, dlg._widthHintPx)

            ' 2. Regel (neu): Wenn mindestens ein "~" vorkommt -> GANZER Text wird Fließtext:
            '    - zuerst ALLE CR/LF zu Leerzeichen (Designer-Zeilenumbrüche sind "falsch positioniert")
            '    - dann "~" -> CRLF
            '    - WordWrap = True
            If rawText.IndexOf("~"c) >= 0 Then
                Dim normalized As String = rawText.Replace(vbCrLf, "").Replace(vbLf, "")
                normalized = normalized.Replace("~", vbCrLf)
                dlg._flowMode = True
                dlg._rtb.WordWrap = True
                dlg._rtb.Text = normalized
            Else
                ' Ursprungslogik beibehalten: ohne CR/LF -> Fließtext, sonst CR/LF-formattreu
                Dim hasCrLf As Boolean = rawText.Contains(vbCrLf) OrElse rawText.Contains(vbLf)
                dlg._flowMode = Not hasCrLf
                dlg._rtb.WordWrap = dlg._flowMode
                dlg._rtb.Text = rawText
            End If

            ' Harte Obergrenze für die RTB-Breite vorbereiten (innen):
            If dlg._widthHintPx > 0 AndAlso dlg._flowMode Then
                dlg._rtbWidthCapPx = dlg._widthHintPx + SystemInformation.VerticalScrollBarWidth + MessageBoxFormatiert.MEASURE_FUDGE_W
            Else
                dlg._rtbWidthCapPx = 0
            End If

            ' Icon
            dlg._iconBox.Image = IconToImage(icon)
            dlg._iconBox.Visible = dlg._iconBox.Image IsNot Nothing


            ' Options: nur RightAlign und RtlReading sind relevant
            Dim isRtl As Boolean = (options And MessageBoxOptions.RtlReading) = MessageBoxOptions.RtlReading
            Dim rightAlign As Boolean = (options And MessageBoxOptions.RightAlign) = MessageBoxOptions.RightAlign

            dlg.RightToLeft = If(isRtl, RightToLeft.Yes, RightToLeft.No)
            dlg._rtb.RightToLeft = dlg.RightToLeft
            dlg._headerLabel.RightToLeft = dlg.RightToLeft

            ' Rechtsbündig nur setzen, wenn gewünscht
            If rightAlign Then
                dlg._rtb.SelectAll()
                dlg._rtb.SelectionAlignment = HorizontalAlignment.Right
                dlg._rtb.Select(0, 0)
            Else
                dlg._rtb.SelectAll()
                dlg._rtb.SelectionAlignment = HorizontalAlignment.Left
                dlg._rtb.Select(0, 0)
            End If

            ' Buttons
            dlg.BuildButtons(buttons)

            ' Default-Button
            dlg.ApplyDefaultButton(defaultButton)

            ' Größen/Positionen
            dlg.ApplySizing(owner)

            ' Zentrierung relativ zum Owner (falls vorhanden)
            If owner IsNot Nothing Then
                dlg.StartPosition = FormStartPosition.Manual
                Dim ow As Control = TryCast(owner, Control)
                If ow IsNot Nothing AndAlso ow.Visible Then
                    dlg.Location = New Point(ow.Left + (ow.Width - dlg.Width) \ 2,
                                             ow.Top + (ow.Height - dlg.Height) \ 2)
                Else
                    dlg.StartPosition = FormStartPosition.CenterScreen
                End If
            End If

            ' Modal anzeigen
            If owner IsNot Nothing Then
                dlg.ShowDialog(owner)
            Else
                dlg.ShowDialog()
            End If
            Return dlg._result

        Finally
            dlg.Dispose()
        End Try
    End Function


    ' 
    ''' <summary>
    ''' Buttons entsprechend MessageBoxButtons erzeugen.
    ''' </summary>
    Private Sub BuildButtons(buttons As MessageBoxButtons)
        _buttonsPanel.Controls.Clear()

        ' Lokale Hilfsfunktion zum Erzeugen eines Buttons
        Dim addBtn As Func(Of String, DialogResult, Boolean, Button) =
            Function(text As String, dr As DialogResult, isCancel As Boolean) As Button
                Dim b As New Button() With {
                    .Text = text,
                    .AutoSize = True,
                    .Margin = New Padding(BUTTON_GAP, 0, 0, 0),
                    .Tag = dr
                }

                ' Klick-Handler
                AddHandler b.Click,
                    Sub(sender As Object, e As EventArgs)
                        _result = dr
                        Me.DialogResult = dr
                        Me.Close()
                    End Sub

                ' Falls Cancel-Button, merken
                If isCancel Then
                    Me.CancelButton = b
                End If

                _buttonsPanel.Controls.Add(b)
                Return b
            End Function

        ' Buttons nach Enum erzeugen
        Select Case buttons
            Case MessageBoxButtons.OK
                addBtn("OK", DialogResult.OK, True)

            Case MessageBoxButtons.OKCancel
                Dim btnCancel As Button = addBtn("Abbrechen", DialogResult.Cancel, True)
                Dim btnOk As Button = addBtn("OK", DialogResult.OK, False)
                Me.AcceptButton = btnOk

            Case MessageBoxButtons.YesNo
                Dim btnNo As Button = addBtn("Nein", DialogResult.No, True)
                Dim btnYes As Button = addBtn("Ja", DialogResult.Yes, False)
                Me.AcceptButton = btnYes

            Case MessageBoxButtons.YesNoCancel
                Dim btnCancel As Button = addBtn("Abbrechen", DialogResult.Cancel, True)
                Dim btnNo As Button = addBtn("Nein", DialogResult.No, False)
                Dim btnYes As Button = addBtn("Ja", DialogResult.Yes, False)
                Me.AcceptButton = btnYes

            Case MessageBoxButtons.RetryCancel
                Dim btnCancel As Button = addBtn("Abbrechen", DialogResult.Cancel, True)
                Dim btnRetry As Button = addBtn("Wiederholen", DialogResult.Retry, False)
                Me.AcceptButton = btnRetry

            Case MessageBoxButtons.AbortRetryIgnore
                Dim btnIgnore As Button = addBtn("Ignorieren", DialogResult.Ignore, False)
                Dim btnRetry As Button = addBtn("Wiederholen", DialogResult.Retry, False)
                Dim btnAbort As Button = addBtn("Abbrechen", DialogResult.Abort, True)
                Me.AcceptButton = btnRetry

            Case Else
                ' Fallback
                addBtn("OK", DialogResult.OK, True)
        End Select
    End Sub

    '
    ''' <summary>
    ''' Default-Button gemäß MessageBoxDefaultButton fokussieren.
    ''' </summary>
    Private Sub ApplyDefaultButton(defBtn As MessageBoxDefaultButton)
        ' Bestimme die "erste/zweite/dritte" Schaltfläche (rechts-nach-links Panel!)
        Dim count As Integer = _buttonsPanel.Controls.Count
        If count = 0 Then Return

        Dim idxFirst As Integer = 0
        Dim idxSecond As Integer = Math.Min(1, count - 1)
        Dim idxThird As Integer = Math.Min(2, count - 1)

        ' Rechts-nach-Links: Index 0 ist die rechte Schaltfläche (visuell erste)
        Dim target As Button
        Select Case defBtn
            Case MessageBoxDefaultButton.Button1
                target = TryCast(_buttonsPanel.Controls(idxFirst), Button)
            Case MessageBoxDefaultButton.Button2
                target = TryCast(_buttonsPanel.Controls(idxSecond), Button)
            Case MessageBoxDefaultButton.Button3
                target = TryCast(_buttonsPanel.Controls(idxThird), Button)
            Case Else
                target = TryCast(_buttonsPanel.Controls(idxFirst), Button)
        End Select

        If target IsNot Nothing Then
            Me.AcceptButton = target
            target.Select()
            target.Focus()
        End If
    End Sub

    Private Sub ApplySizing(owner As IWin32Window)
        ' Arbeitsbereich bestimmen
        Dim wa As Rectangle
        If owner IsNot Nothing Then
            Dim ow As Control = TryCast(owner, Control)
            wa = If(ow IsNot Nothing, Screen.FromControl(ow).WorkingArea, Screen.PrimaryScreen.WorkingArea)
        Else
            wa = Screen.PrimaryScreen.WorkingArea
        End If

        Dim maxW As Integer = CInt(Math.Floor(wa.Width * 2.0 / 3.0))
        Dim maxH As Integer = CInt(Math.Floor(wa.Height * 2.0 / 3.0))

        ' Icon und Header
        Dim iconW As Integer = If(_iconBox.Image IsNot Nothing, ICON_SIZE, 0)
        Dim headerSize As Size = TextRenderer.MeasureText(_headerLabel.Text, _fontHeader)

        ' Text messen
        Dim contentSize As Size
        If _flowMode Then
            ' Maximalbreite: "|"-Hinweis als Obergrenze (innen), nicht als Mindestbreite
            Dim widthBoundPx As Integer = maxW
            If _widthHintPx > 0 Then
                widthBoundPx = Math.Min(maxW, _widthHintPx + MEASURE_FUDGE_W)
            End If

            contentSize = TextRenderer.MeasureText(
                _rtb.Text,
                _fontMessage,
                New Size(widthBoundPx, Integer.MaxValue),
                TextFormatFlags.WordBreak Or TextFormatFlags.NoPadding Or TextFormatFlags.TextBoxControl
            )
        Else
            contentSize = TextRenderer.MeasureText(
                _rtb.Text,
                _fontMessage,
                New Size(Integer.MaxValue, Integer.MaxValue),
                TextFormatFlags.NoPadding Or TextFormatFlags.TextBoxControl
            )
        End If


        ' Scrollbar-Breiten pauschal aufschlagen
        contentSize = New Size(
            contentSize.Width + SystemInformation.VerticalScrollBarWidth,
            contentSize.Height + SystemInformation.HorizontalScrollBarHeight
        )


        ' Scrollbar-Breiten pauschal aufschlagen
        contentSize = New Size(contentSize.Width + SystemInformation.VerticalScrollBarWidth,
                               contentSize.Height + SystemInformation.HorizontalScrollBarHeight)

        ' Clientgröße berechnen
        Dim desiredContentW As Integer = iconW + If(iconW > 0, GAP, 0) + Math.Max(headerSize.Width, contentSize.Width)
        Dim desiredContentH As Integer = Math.Max(ICON_SIZE, headerSize.Height + GAP + contentSize.Height)

        Dim buttonsH As Integer = 35
        Dim desiredClientW As Integer = PADDING * 2 + Math.Max(desiredContentW, _buttonsPanel.PreferredSize.Width)
        Dim desiredClientH As Integer = PADDING * 3 + desiredContentH + buttonsH

        desiredClientW = Math.Max(desiredClientW, MIN_CLIENT_W)
        desiredClientH = Math.Max(desiredClientH, MIN_CLIENT_H)

        ' ... nach deiner Größenberechnung:
        Dim finalW As Integer = Math.Min(desiredClientW, maxW)
        Dim finalH As Integer = Math.Min(desiredClientH, maxH)

        ' Nur initial setzen (ShowCore ruft ApplySizing genau einmal vor dem ShowDialog)
        Me.ClientSize = New Size(finalW, finalH)

        ' Scrollbar-Policy bleibt:
        _rtb.ScrollBars = RichTextBoxScrollBars.Both


        LayoutControls()
    End Sub

    Private Sub LayoutControls()
        Dim x As Integer = PADDING
        Dim y As Integer = PADDING

        Dim iconW As Integer = If(_iconBox.Image IsNot Nothing, ICON_SIZE, 0)
        _iconBox.Location = New Point(5, 5)

        ' Header über die Breite (links neben dem Icon beginnen)
        Dim headerLeft As Integer = iconW + If(iconW > 0, GAP, 0)
        _headerLabel.Location = New Point(headerLeft, y)
        _headerLabel.Width = Me.ClientSize.Width - headerLeft - PADDING
        _headerLabel.Height = TextRenderer.MeasureText(_headerLabel.Text, _fontHeader).Height

        ' RTB darunter, bis knapp über die Buttons

        ' RTB-Standardbreite:
        Dim rtbW As Integer = Me.ClientSize.Width - headerLeft - PADDING

        ' Harte Kappe, falls aktiv:
        If _rtbWidthCapPx > 0 Then
            rtbW = Math.Min(rtbW, _rtbWidthCapPx)
            'nur einmal anwenden, damit die Rtb über die _rtbWidthCapPx-Breite hinaus mitzieht,
            'wenn die Form vergrößert wird.
            _rtbWidthCapPx = 0
        End If

        _rtb.Location = New Point(headerLeft, _headerLabel.Bottom + GAP)
        _rtb.Width = rtbW
        _rtb.Height = Me.ClientSize.Height - _rtb.Top - PADDING - _buttonsPanel.PreferredSize.Height - GAP

        '_rtb.Location = New Point(headerLeft, _headerLabel.Bottom + GAP)
        '_rtb.Width = Me.ClientSize.Width - headerLeft - PADDING
        '_rtb.Height = Me.ClientSize.Height - _rtb.Top - PADDING - _buttonsPanel.PreferredSize.Height - GAP

        ' Buttons rechts unten
        _buttonsPanel.Location = New Point(Me.ClientSize.Width - PADDING - _buttonsPanel.PreferredSize.Width,
                                       Me.ClientSize.Height - PADDING - _buttonsPanel.PreferredSize.Height)
    End Sub

    Private Sub HeaderLabel_MouseClick(sender As Object, e As MouseEventArgs)
        Dim lbl As Label = DirectCast(sender, Label)
        Dim w As Integer = lbl.ClientSize.Width
        If w <= 0 Then Return

        ' X für RTL spiegeln, damit links/rechts visuell stimmt
        Dim x As Integer = If(lbl.RightToLeft = RightToLeft.Yes, w - e.X, e.X)
        Dim third As Integer = w \ 3

        ' --- Regeln:
        ' Links: Höhe toggeln
        ' Rechts: Breite toggeln
        ' Mitte: wenn (Breite oder Höhe) nicht maximiert -> beide maximieren; wenn beide maximiert -> zurück

        If x < third Then
            ' Links: Höhe
            If Not _snapHeight Then EnsureBaseBounds()
            _snapHeight = Not _snapHeight

        ElseIf x < 2 * third Then
            ' Mitte
            If _snapWidth AndAlso _snapHeight Then
                _snapWidth = False
                _snapHeight = False
            Else
                EnsureBaseBounds()
                _snapWidth = True
                _snapHeight = True
            End If

        Else
            ' Rechts: Breite
            If Not _snapWidth Then EnsureBaseBounds()
            _snapWidth = Not _snapWidth
        End If

        ' Anwenden bzw. vollständiges Restore
        If Not _snapWidth AndAlso Not _snapHeight AndAlso Not _baseBounds.IsEmpty Then
            ' kompletter Rückweg
            Me.Bounds = _baseBounds
            _baseBounds = Rectangle.Empty
            LayoutControls()
        Else
            ApplySnapStates()
        End If
    End Sub



    Private Sub SnapTo(mode As SnapMode)
        Dim wa As Rectangle = Screen.FromControl(Me).WorkingArea

        ' Vorherige Bounds merken, aber nur wenn wir nicht schon in einem Snap sind
        If _snapMode = SnapMode.None Then
            _prevBounds = Me.Bounds
        End If

        Select Case mode
            Case SnapMode.FullHeight
                ' Breite/Left behalten, nur nach oben ziehen und Höhe auf WorkingArea
                Dim newLeft As Integer = Me.Left
                Dim newWidth As Integer = Me.Width
                Dim newTop As Integer = wa.Top + PADDING
                Dim newHeight As Integer = wa.Height - 2 * PADDING
                Me.Bounds = New Rectangle(newLeft, newTop, newWidth, newHeight)

            Case SnapMode.FullWidth
                ' Höhe/Top behalten, nur in die volle Breite gehen
                Dim newTop As Integer = Me.Top
                Dim newHeight As Integer = Me.Height
                Dim newLeft As Integer = wa.Left + PADDING
                Dim newWidth As Integer = wa.Width - 2 * PADDING
                Me.Bounds = New Rectangle(newLeft, newTop, newWidth, newHeight)

            Case SnapMode.FullBoth
                ' quasi maximiert innerhalb der WorkingArea mit etwas Rand
                Me.Bounds = New Rectangle(
                wa.Left + PADDING,
                wa.Top + PADDING,
                wa.Width - 2 * PADDING,
                wa.Height - 2 * PADDING)
        End Select

        _snapMode = mode
        LayoutControls() ' damit sich alles sauber anpasst
    End Sub

    Private Sub RestorePrevBounds()
        If _snapMode = SnapMode.None Then Return
        If _prevBounds.Width > 0 AndAlso _prevBounds.Height > 0 Then
            Me.Bounds = _prevBounds
        End If
        _snapMode = SnapMode.None
        LayoutControls()
    End Sub

    Private Sub EnsureBaseBounds()
        ' Ausgangszustand nur einmal festhalten (beim ersten Einschalten irgendeines Snaps)
        If _baseBounds.IsEmpty Then _baseBounds = Me.Bounds
    End Sub

    Private Sub ApplySnapStates()
        Dim wa As Rectangle = Screen.FromControl(Me).WorkingArea

        Dim left As Integer = If(_snapWidth, wa.Left + PADDING, If(_baseBounds.IsEmpty, Me.Left, _baseBounds.Left))
        Dim top As Integer = If(_snapHeight, wa.Top + PADDING, If(_baseBounds.IsEmpty, Me.Top, _baseBounds.Top))
        Dim width As Integer = If(_snapWidth, wa.Width - 2 * PADDING, If(_baseBounds.IsEmpty, Me.Width, _baseBounds.Width))
        Dim height As Integer = If(_snapHeight, wa.Height - 2 * PADDING, If(_baseBounds.IsEmpty, Me.Height, _baseBounds.Height))

        ' Mindestgrößen beachten
        width = Math.Max(width, Me.MinimumSize.Width)
        height = Math.Max(height, Me.MinimumSize.Height)

        Me.Bounds = New Rectangle(left, top, width, height)
        LayoutControls()
    End Sub



    '============ Shared Show(...) Overloads ============

    '
    ''' <summary>' 1: text </summary>
    Public Overloads Shared Function Show(text As String) As DialogResult
        Return ShowCore(Nothing, text, String.Empty, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, CType(0, MessageBoxOptions))
    End Function

    '
    ''' <summary>' 2: text, caption </summary>
    Public Overloads Shared Function Show(text As String, caption As String) As DialogResult
        Return ShowCore(Nothing, text, caption, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, CType(0, MessageBoxOptions))
    End Function

    '
    ''' <summary>' 3: text, caption, buttons </summary>
    Public Overloads Shared Function Show(text As String, caption As String, buttons As MessageBoxButtons) As DialogResult
        Return ShowCore(Nothing, text, caption, buttons, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, CType(0, MessageBoxOptions))
    End Function

    '
    ''' <summary>' 4: text, caption, buttons, icon </summary>
    Public Overloads Shared Function Show(text As String, caption As String, buttons As MessageBoxButtons, icon As MessageBoxIcon) As DialogResult
        Return ShowCore(Nothing, text, caption, buttons, icon, MessageBoxDefaultButton.Button1, CType(0, MessageBoxOptions))
    End Function

    '
    ''' <summary>' 5: text, caption, buttons, icon, defaultButton </summary>
    Public Overloads Shared Function Show(text As String, caption As String, buttons As MessageBoxButtons, icon As MessageBoxIcon, defaultButton As MessageBoxDefaultButton) As DialogResult
        Return ShowCore(Nothing, text, caption, buttons, icon, defaultButton, CType(0, MessageBoxOptions))
    End Function

    '
    ''' <summary>' 6: text, caption, buttons, icon, defaultButton, options </summary>
    Public Overloads Shared Function Show(text As String, caption As String, buttons As MessageBoxButtons, icon As MessageBoxIcon, defaultButton As MessageBoxDefaultButton, options As MessageBoxOptions) As DialogResult
        Return ShowCore(Nothing, text, caption, buttons, icon, defaultButton, options)
    End Function

    '
    ''' <summary>' 7: owner, text </summary>
    Public Overloads Shared Function Show(owner As IWin32Window, text As String) As DialogResult
        Return ShowCore(owner, text, String.Empty, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, CType(0, MessageBoxOptions))
    End Function
    '
    ''' <summary>' 8: owner, text, caption, buttons </summary>
    Public Overloads Shared Function Show(owner As IWin32Window, text As String, caption As String, buttons As MessageBoxButtons) As DialogResult
        Return ShowCore(owner, text, caption, buttons, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, CType(0, MessageBoxOptions))
    End Function

    '
    ''' <summary>' 9: owner, text, caption, buttons, icon </summary>
    Public Overloads Shared Function Show(owner As IWin32Window, text As String, caption As String, buttons As MessageBoxButtons, icon As MessageBoxIcon) As DialogResult
        Return ShowCore(owner, text, caption, buttons, icon, MessageBoxDefaultButton.Button1, CType(0, MessageBoxOptions))
    End Function

    '
    ''' <summary>' 10: owner, text, caption, buttons, icon, defaultButton </summary>
    Public Overloads Shared Function Show(owner As IWin32Window, text As String, caption As String, buttons As MessageBoxButtons, icon As MessageBoxIcon, defaultButton As MessageBoxDefaultButton) As DialogResult
        Return ShowCore(owner, text, caption, buttons, icon, defaultButton, CType(0, MessageBoxOptions))
    End Function

    '
    ''' <summary>' 11: owner, text, caption, buttons, icon, defaultButton, options </summary>
    Public Overloads Shared Function Show(owner As IWin32Window, text As String, caption As String, buttons As MessageBoxButtons, icon As MessageBoxIcon, defaultButton As MessageBoxDefaultButton, options As MessageBoxOptions) As DialogResult
        Return ShowCore(owner, text, caption, buttons, icon, defaultButton, options)
    End Function

    Public Overloads Shared Function ShowInfo(text As String) As DialogResult
        Return ShowCore(Nothing, text, String.Empty, MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, CType(0, MessageBoxOptions))
    End Function
    Public Overloads Shared Function ShowInfo(text As String, caption As String) As DialogResult
        Return ShowCore(Nothing, text, caption, MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, CType(0, MessageBoxOptions))
    End Function

    Public Overloads Shared Function ShowInfo(owner As IWin32Window, text As String) As DialogResult
        Return ShowCore(owner, text, String.Empty, MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, CType(0, MessageBoxOptions))
    End Function
    Public Overloads Shared Function ShowInfo(owner As IWin32Window, text As String, caption As String) As DialogResult
        Return ShowCore(owner, text, caption, MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, CType(0, MessageBoxOptions))
    End Function

    Public Overloads Shared Function ShowInfoMonoSpaced(text As String) As DialogResult
        Return ShowCore(Nothing, text, String.Empty, MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, CType(0, MessageBoxOptions), useMonospacedFont:=True)
    End Function
    Public Overloads Shared Function ShowInfoMonoSpaced(text As String, caption As String) As DialogResult
        Return ShowCore(Nothing, text, caption, MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, CType(0, MessageBoxOptions), useMonospacedFont:=True)
    End Function

    Public Overloads Shared Function ShowInfoMonoSpaced(owner As IWin32Window, text As String) As DialogResult
        Return ShowCore(owner, text, String.Empty, MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, CType(0, MessageBoxOptions), useMonospacedFont:=True)
    End Function
    Public Overloads Shared Function ShowInfoMonoSpaced(owner As IWin32Window, text As String, caption As String) As DialogResult
        Return ShowCore(owner, text, caption, MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, CType(0, MessageBoxOptions), useMonospacedFont:=True)
    End Function


    '============ Helfer ============
    ''' <summary>
    ''' Sucht in der ERSTEN Zeile das erste "|" und liefert:
    ''' - processedText: Text mit entferntem "|" (nur dieses eine in Zeile 1)
    ''' - widthHintPx:   gerenderte Breite des Textes VOR dem "|" in Pixel (Font = _fontMessage)
    ''' </summary>
    Private Function ProcessWidthHintPipe(text As String, ByRef widthHintPx As Integer) As String
        widthHintPx = 0
        If String.IsNullOrEmpty(text) Then Return String.Empty

        ' erste Zeile isolieren
        Dim nlPos As Integer = text.IndexOf(vbCrLf, StringComparison.Ordinal)
        If nlPos < 0 Then nlPos = text.IndexOf(ChrW(10)) ' Fallback reines LF
        Dim firstLineEnd As Integer = If(nlPos >= 0, nlPos, text.Length)

        Dim pipePos As Integer = text.IndexOf("|"c)
        If pipePos >= 0 AndAlso pipePos < firstLineEnd Then
            ' Breite bis zur Pipe messen (Text ohne Pipe-Zeichen)
            Dim beforePipe As String = If(pipePos > 0, text.Substring(0, pipePos), String.Empty)
            widthHintPx = TextRenderer.MeasureText(
                beforePipe,
                _fontMessage,
                New Size(Integer.MaxValue, Integer.MaxValue),
                TextFormatFlags.NoPadding Or TextFormatFlags.TextBoxControl
            ).Width
            If String.IsNullOrWhiteSpace(beforePipe) Then
                text = text.Substring(pipePos + 1)
            Else
                ' genau DIESES Pipe in Zeile 1 entfernen
                text = text.Remove(pipePos, 1)
            End If
        End If

        Return text
    End Function

    '
    ''' <summary>
    ''' Sichere Font-Erstellung (Fallback auf Arial, wenn Familie/Style nicht verfügbar).
    ''' </summary>
    Private Shared Function GetSafeFont(preferred As Font, fallback As Font) As Font
        Try
            If preferred Is Nothing Then Return fallback
            Using ifc As New InstalledFontCollection()
                If Not ifc.Families.Any(Function(f) f.Name.Equals(preferred.FontFamily.Name, StringComparison.OrdinalIgnoreCase)) Then
                    Return fallback
                End If
            End Using
            ' Style verfügbar?
            If Not preferred.FontFamily.IsStyleAvailable(preferred.Style) Then
                Return New Font(preferred.FontFamily, preferred.Size, FontStyle.Regular, preferred.Unit)
            End If
            Return preferred
        Catch
            Return fallback
        End Try
    End Function

    '
    ''' <summary>
    ''' MessageBoxIcon -> SystemIcons.Image
    ''' </summary>
    Private Shared Function IconToImage(icon As MessageBoxIcon) As Image
        Dim ico As Icon
        Select Case icon
            Case MessageBoxIcon.Error, MessageBoxIcon.Hand, MessageBoxIcon.Stop
                ico = SystemIcons.Error
            Case MessageBoxIcon.Exclamation, MessageBoxIcon.Warning
                ico = SystemIcons.Warning
            Case MessageBoxIcon.Question
                ico = SystemIcons.Question
            Case MessageBoxIcon.Information, MessageBoxIcon.Asterisk
                ico = SystemIcons.Information
            Case Else
                Return Nothing
        End Select
        Return ico.ToBitmap()
    End Function

    Private Sub IconBox_Click(sender As Object, e As EventArgs)
        Me.TopMost = Not Me.TopMost
        _iconBox.Invalidate()   ' Rahmen neu zeichnen
    End Sub

    Private Sub Me_TopMostChanged(sender As Object, e As EventArgs)
        _iconBox.Invalidate()
    End Sub

    Private Sub IconBox_Paint(sender As Object, e As PaintEventArgs)
        If Me.TopMost Then
            Dim rect As New Rectangle(0, 0, _iconBox.Width - 1, _iconBox.Height - 1)
            Using p As New Pen(Color.DimGray, 1)
                e.Graphics.DrawRectangle(p, rect)
            End Using
        End If
    End Sub
End Class

