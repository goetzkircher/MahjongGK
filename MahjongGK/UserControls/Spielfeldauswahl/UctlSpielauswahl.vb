Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

#Disable Warning IDE0079
#Disable Warning IDE1006

Imports System.Drawing.Drawing2D
Imports System.IO
Imports MahjongGK.Spielfeld

Public Class UctlSpielauswahl

    Public Event SpielAusgewaehlt(fullPath As String, sfInfo As SFInfo)

    Private Const ITEM_W As Integer = 420
    Private Const ITEM_H As Integer = 235
    Private Shadows Const MARGIN As Integer = 12
    Private Const PIC_W As Integer = 380
    Private Const PIC_H As Integer = 165
    Private Const TEXT_W As Integer = 380
    Private Const ITEM_GAP_X As Integer = 12
    Private Const ITEM_GAP_Y As Integer = 12

    Private ReadOnly _items As New List(Of SpielauswahlItem)
    Private _selectedIndex As Integer = -1

    Private Class SpielauswahlItem
        Public Property FullName As String
        Public Property SFInfo As SFInfo
        Public Property Picture As Bitmap
    End Class

    Public Sub New()

        InitializeComponent()

        AddHandler Me.Disposed, AddressOf OnUctlSpielauswahlDisposed

        Me.DoubleBuffered = True
        Me.TabStop = True

        SetStyle(ControlStyles.AllPaintingInWmPaint Or
                 ControlStyles.UserPaint Or
                 ControlStyles.OptimizedDoubleBuffer Or
                 ControlStyles.ResizeRedraw Or
                 ControlStyles.Selectable, True)

    End Sub

    Public Sub Initialisierung(arrFI() As FileInfo)

        ClearItems()

        If arrFI Is Nothing Then
            UpdateScrollbars()
            Invalidate()
            Return
        End If

        For Each item As FileInfo In arrFI

            Dim fullName As String = item.FullName

            If String.IsNullOrWhiteSpace(fullName) OrElse Not File.Exists(fullName) Then
                Continue For
            End If

            Dim sfInf As SFInfo = SFInfo.Load(fullName)

            If sfInf IsNot Nothing AndAlso sfInf.IsValide Then

                Dim bmp As Bitmap = Nothing

                If sfInf.SpielfeldPicture IsNot Nothing Then
                    bmp = New Bitmap(sfInf.SpielfeldPicture)
                End If

                _items.Add(New SpielauswahlItem With {
                    .FullName = fullName,
                    .SFInfo = sfInf,
                    .Picture = bmp
                })

            End If
        Next

        If _items.Count > 0 Then
            _selectedIndex = 0
        Else
            _selectedIndex = -1
        End If

        UpdateScrollbars()
        Invalidate()

    End Sub

    Private Sub ClearItems()

        For Each it As SpielauswahlItem In _items
            If it.Picture Is Nothing Then
                Continue For
            End If
            it.Picture.Dispose()
        Next

        _items.Clear()
        _selectedIndex = -1

    End Sub

    Protected Overrides Sub OnResize(e As EventArgs)

        MyBase.OnResize(e)
        UpdateScrollbars()
        Invalidate()

    End Sub

    Private Sub UpdateScrollbars()

        Dim colCount As Integer = GetColumnCount()
        Dim rowCount As Integer

        If _items.Count = 0 Then
            rowCount = 0
        Else
            rowCount = ((_items.Count - 1) \ colCount) + 1
        End If

        Dim contentW As Integer = MARGIN * 2 + colCount * ITEM_W + Math.Max(0, colCount - 1) * ITEM_GAP_X
        Dim contentH As Integer = MARGIN * 2 + rowCount * ITEM_H + Math.Max(0, rowCount - 1) * ITEM_GAP_Y

        Dim viewW As Integer = Math.Max(1, ClientSize.Width - vsb.Width)
        Dim viewH As Integer = Math.Max(1, ClientSize.Height - hsb.Height)

        hsb.Visible = contentW > viewW
        vsb.Visible = contentH > viewH

        If hsb.Visible Then
            hsb.Left = 0
            hsb.Top = ClientSize.Height - hsb.Height
            hsb.Width = ClientSize.Width - If(vsb.Visible, vsb.Width, 0)
            hsb.LargeChange = Math.Max(1, viewW)
            hsb.SmallChange = 30
            hsb.Maximum = Math.Max(0, contentW - viewW + hsb.LargeChange - 1)
        Else
            hsb.Value = 0
        End If

        If vsb.Visible Then
            vsb.Top = 0
            vsb.Left = ClientSize.Width - vsb.Width
            vsb.Height = ClientSize.Height - If(hsb.Visible, hsb.Height, 0)
            vsb.LargeChange = Math.Max(1, viewH)
            vsb.SmallChange = 30
            vsb.Maximum = Math.Max(0, contentH - viewH + vsb.LargeChange - 1)
        Else
            vsb.Value = 0
        End If

    End Sub

    Protected Overrides Sub OnPaint(e As PaintEventArgs)

        MyBase.OnPaint(e)

        e.Graphics.Clear(SystemColors.Window)
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias
        e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic
        e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality

        For idx As Integer = 0 To _items.Count - 1

            Dim itemRect As Rectangle = GetItemRect(idx)

            If itemRect.Right < 0 OrElse
           itemRect.Bottom < 0 OrElse
           itemRect.Left > ClientSize.Width OrElse
           itemRect.Top > ClientSize.Height Then

                Continue For

            End If

            DrawItem(e.Graphics, idx, itemRect)

        Next

    End Sub

    Private Sub DrawItem(g As Graphics, idx As Integer, itemRect As Rectangle)

        Dim it As SpielauswahlItem = _items(idx)
        Dim selected As Boolean = idx = _selectedIndex

        If selected Then
            Using br As New SolidBrush(Color.FromArgb(35, 60, 120, 220))
                g.FillRectangle(br, itemRect)
            End Using

            Using pen As New Pen(Color.FromArgb(80, 120, 220), 2.0F)
                g.DrawRectangle(pen, itemRect)
            End Using
        End If

        Dim x As Integer = itemRect.Left + 12
        Dim y As Integer = itemRect.Top + 8

        Using fName As New Font(Me.Font.FontFamily, 10.5F, FontStyle.Bold)
            Using br As New SolidBrush(SystemColors.ControlText)
                Dim nameText As String = If(it.SFInfo.Name, "")
                g.DrawString(nameText, fName, br, New RectangleF(x, y, TEXT_W, 22.0F))
            End Using
        End Using

        y += 26

        Dim picRect As New Rectangle(x, y, PIC_W, PIC_H)

        Using br As New SolidBrush(Color.FromArgb(245, 245, 245))
            g.FillRectangle(br, picRect)
        End Using

        If it.Picture IsNot Nothing Then
            Dim dst As Rectangle = GetProportionalRect(it.Picture.Size, picRect)
            g.DrawImage(it.Picture, dst)

            If selected Then
                Using brBlue As New SolidBrush(Color.FromArgb(70, 70, 120, 255))
                    g.FillRectangle(brBlue, dst)
                End Using
            End If
        End If

        Using pen As New Pen(Color.Silver)
            g.DrawRectangle(pen, picRect)
        End Using

        y += PIC_H + 6

        Using fText As New Font(Me.Font.FontFamily, 9.0F, FontStyle.Regular)
            Using br As New SolidBrush(SystemColors.ControlText)

                Dim beschrRect As New RectangleF(x, y, TEXT_W, 45.0F)

                Using sf As New StringFormat()
                    sf.Trimming = StringTrimming.EllipsisWord
                    sf.FormatFlags = StringFormatFlags.LineLimit
                    g.DrawString(If(it.SFInfo.Beschreibung, ""), fText, br, beschrRect, sf)
                End Using

                y += 48

                Using sfInfo As New StringFormat()
                    sfInfo.Trimming = StringTrimming.EllipsisCharacter
                    sfInfo.FormatFlags = StringFormatFlags.NoWrap
                    g.DrawString(If(it.SFInfo.SpielInfo, ""), fText, br, New RectangleF(x, y, TEXT_W, 18.0F), sfInfo)
                End Using

            End Using
        End Using

    End Sub

    Private Shared Function GetProportionalRect(srcSize As Size, dstRect As Rectangle) As Rectangle

        If srcSize.Width <= 0 OrElse srcSize.Height <= 0 Then
            Return Rectangle.Empty
        End If

        Dim fx As Double = CDbl(dstRect.Width) / CDbl(srcSize.Width)
        Dim fy As Double = CDbl(dstRect.Height) / CDbl(srcSize.Height)
        Dim f As Double = Math.Min(fx, fy)

        Dim w As Integer = CInt(Math.Round(srcSize.Width * f))
        Dim h As Integer = CInt(Math.Round(srcSize.Height * f))

        Dim x As Integer = dstRect.Left + (dstRect.Width - w) \ 2
        Dim y As Integer = dstRect.Top + (dstRect.Height - h) \ 2

        Return New Rectangle(x, y, w, h)

    End Function

    Private Function GetViewWidth() As Integer
        Return Math.Max(1, ClientSize.Width - If(vsb.Visible, vsb.Width, 0))
    End Function

    Private Function GetViewHeight() As Integer
        Return Math.Max(1, ClientSize.Height - If(hsb.Visible, hsb.Height, 0))
    End Function

    Private Function GetColumnCount() As Integer

        Dim availableW As Integer = Math.Max(1, ClientSize.Width - vsb.Width - MARGIN * 2)
        Dim fullItemW As Integer = ITEM_W + ITEM_GAP_X

        Return Math.Max(1, availableW \ fullItemW)

    End Function

    Private Function GetItemRect(idx As Integer) As Rectangle

        Dim colCount As Integer = GetColumnCount()

        Dim col As Integer = idx Mod colCount
        Dim row As Integer = idx \ colCount

        Dim x As Integer = MARGIN + col * (ITEM_W + ITEM_GAP_X) - hsb.Value
        Dim y As Integer = MARGIN + row * (ITEM_H + ITEM_GAP_Y) - vsb.Value

        Return New Rectangle(x, y, ITEM_W, ITEM_H)

    End Function
    Protected Overrides Sub OnMouseDown(e As MouseEventArgs)

        MyBase.OnMouseDown(e)
        Me.Focus()

        Dim idx As Integer = HitTest(e.Location)

        If idx >= 0 Then
            _selectedIndex = idx
            Invalidate()
        End If

    End Sub

    Protected Overrides Sub OnMouseDoubleClick(e As MouseEventArgs)

        MyBase.OnMouseDoubleClick(e)

        Dim idx As Integer = HitTest(e.Location)

        If idx >= 0 Then
            _selectedIndex = idx
            SelectCurrentGame()
        End If

    End Sub

    Private Function HitTest(p As Point) As Integer

        For idx As Integer = 0 To _items.Count - 1

            Dim r As Rectangle = GetItemRect(idx)

            If r.Contains(p) Then
                Return idx
            End If

        Next

        Return -1

    End Function

    Protected Overrides Sub OnMouseWheel(e As MouseEventArgs)

        MyBase.OnMouseWheel(e)

        If Not vsb.Visible Then
            Return
        End If

        Dim newValue As Integer = vsb.Value - Math.Sign(e.Delta) * vsb.SmallChange * 3
        SetVScrollValue(newValue)

    End Sub

    Protected Overrides Function IsInputKey(keyData As Keys) As Boolean

        Select Case keyData And Keys.KeyCode
            Case Keys.Up, Keys.Down, Keys.PageUp, Keys.PageDown, Keys.Home, Keys.End, Keys.Enter
                Return True
        End Select

        Return MyBase.IsInputKey(keyData)

    End Function

    Protected Overrides Sub OnKeyDown(e As KeyEventArgs)

        MyBase.OnKeyDown(e)

        If _items.Count = 0 Then
            Return
        End If

        Select Case e.KeyCode

            Case Keys.Up
                SetSelectedIndex(Math.Max(0, _selectedIndex - 1))
                e.Handled = True

            Case Keys.Down
                SetSelectedIndex(Math.Min(_items.Count - 1, _selectedIndex + 1))
                e.Handled = True

            Case Keys.PageUp
                SetSelectedIndex(Math.Max(0, _selectedIndex - 5))
                e.Handled = True

            Case Keys.PageDown
                SetSelectedIndex(Math.Min(_items.Count - 1, _selectedIndex + 5))
                e.Handled = True

            Case Keys.Home
                SetSelectedIndex(0)
                e.Handled = True

            Case Keys.End
                SetSelectedIndex(_items.Count - 1)
                e.Handled = True

            Case Keys.Enter
                SelectCurrentGame()
                e.Handled = True

        End Select

    End Sub

    Private Sub SetSelectedIndex(idx As Integer)

        If idx < 0 OrElse idx >= _items.Count Then
            Return
        End If

        _selectedIndex = idx
        EnsureSelectedVisible()
        Invalidate()

    End Sub

    Private Sub EnsureSelectedVisible()

        If _selectedIndex < 0 Then
            Return
        End If

        Dim r As Rectangle = GetItemRect(_selectedIndex)

        Dim viewW As Integer = ClientSize.Width - If(vsb.Visible, vsb.Width, 0)
        Dim viewH As Integer = ClientSize.Height - If(hsb.Visible, hsb.Height, 0)

        If r.Left < 0 Then
            SetHScrollValue(hsb.Value + r.Left)
        ElseIf r.Right > viewW Then
            SetHScrollValue(hsb.Value + r.Right - viewW)
        End If

        If r.Top < 0 Then
            SetVScrollValue(vsb.Value + r.Top)
        ElseIf r.Bottom > viewH Then
            SetVScrollValue(vsb.Value + r.Bottom - viewH)
        End If

    End Sub

    Private Sub SelectCurrentGame()

        If _selectedIndex < 0 OrElse _selectedIndex >= _items.Count Then
            Return
        End If

        Dim it As SpielauswahlItem = _items(_selectedIndex)
        RaiseEvent SpielAusgewaehlt(it.FullName, it.SFInfo)

    End Sub

    Private Sub vsb_Scroll(sender As Object, e As ScrollEventArgs) Handles vsb.Scroll
        Invalidate()
    End Sub

    Private Sub hsb_Scroll(sender As Object, e As ScrollEventArgs) Handles hsb.Scroll
        Invalidate()
    End Sub

    Private Sub SetVScrollValue(value As Integer)

        If Not vsb.Visible Then
            Return
        End If

        Dim maxVal As Integer = Math.Max(vsb.Minimum, vsb.Maximum - vsb.LargeChange + 1)
        vsb.Value = Math.Max(vsb.Minimum, Math.Min(maxVal, value))
        Invalidate()

    End Sub

    Private Sub SetHScrollValue(value As Integer)

        If Not hsb.Visible Then
            Return
        End If

        Dim maxVal As Integer = Math.Max(hsb.Minimum, hsb.Maximum - hsb.LargeChange + 1)
        hsb.Value = Math.Max(hsb.Minimum, Math.Min(maxVal, value))
        Invalidate()

    End Sub
    Private Sub OnUctlSpielauswahlDisposed(sender As Object, e As EventArgs)

        ClearItems()

    End Sub

End Class
