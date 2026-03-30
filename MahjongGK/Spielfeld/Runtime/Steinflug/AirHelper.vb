
Namespace Spielfeld
    Module AirHelper

        Public Function AirGetKlickArea(ptMousePos As Point, sfLay As SFLayout) As AirKlickArea

            With sfLay
                ' Ganz oben liegende Flächen zuerst
                If .rxUndo IsNot Nothing AndAlso .rxUndo.Contains(ptMousePos) Then
                    Return AirKlickArea.Undo
                End If

                If .rxRedo IsNot Nothing AndAlso .rxRedo.Contains(ptMousePos) Then
                    Return AirKlickArea.Redo
                End If

                If .rxHistoryBoxSmall IsNot Nothing AndAlso .rxHistoryBoxSmall.Contains(ptMousePos) Then
                    Return AirKlickArea.HistoryBoxSmall
                End If

                ' Alles Weitere liegt innerhalb von rxContent.
                ' Wenn pt nicht einmal in rxContent liegt, ist sofort Schluss.
                If .rxContent Is Nothing OrElse Not .rxContent.Contains(ptMousePos) Then
                    Return AirKlickArea.None
                End If

                ' Unterbereiche innerhalb von rxContent
                If .rxStockScrollbar IsNot Nothing AndAlso .rxStockScrollbar.Contains(ptMousePos) Then
                    Return AirKlickArea.StockScrollbar
                End If

                If .rxStock IsNot Nothing AndAlso .rxStock.Contains(ptMousePos) Then
                    Return AirKlickArea.Stock
                End If

                If .rxHistoryBoxLeftScrollbar IsNot Nothing AndAlso .rxHistoryBoxLeftScrollbar.Contains(ptMousePos) Then
                    Return AirKlickArea.HistoryBoxLeftScrollbar
                End If

                If .rxHistoryBoxRightScrollbar IsNot Nothing AndAlso .rxHistoryBoxRightScrollbar.Contains(ptMousePos) Then
                    Return AirKlickArea.HistoryBoxRightScrollbar
                End If

                If .rxHistoryBoxLeft IsNot Nothing AndAlso .rxHistoryBoxLeft.Contains(ptMousePos) Then
                    Return AirKlickArea.HistoryBoxLeft
                End If

                If .rxHistoryBoxRight IsNot Nothing AndAlso .rxHistoryBoxRight.Contains(ptMousePos) Then
                    Return AirKlickArea.HistoryBoxRight
                End If

                If .rxStageUsed IsNot Nothing AndAlso .rxStageUsed.Contains(ptMousePos) Then
                    Dim hasStock As Boolean = (.rxStock IsNot Nothing)
                    If .rxStock IsNot Nothing Then
                        Return AirKlickArea.Editor
                    Else
                        Return AirKlickArea.Spielfeld
                    End If
                End If

                If .rxHeader IsNot Nothing AndAlso .rxHeader.Contains(ptMousePos) Then
                    Return AirKlickArea.Header
                End If

                If .rxContent IsNot Nothing AndAlso .rxContent.Contains(ptMousePos) Then
                    Return AirKlickArea.Content
                End If
                '
                'wird aktuell nicht erreicht.
                Return AirKlickArea.Outside

            End With
        End Function

        Public Function AirGetKlickAreaGroup(aka As AirKlickArea) As AirKlickAreaGroup
            Select Case aka
                Case AirKlickArea.None
                    Return AirKlickAreaGroup.None

                Case AirKlickArea.Content
                    Return AirKlickAreaGroup.Outside

                Case AirKlickArea.Stock
                    Return AirKlickAreaGroup.Editor

                Case AirKlickArea.StockScrollbar
                    Return AirKlickAreaGroup.Scrollbar

                Case AirKlickArea.HistoryBoxLeft
                    Return AirKlickAreaGroup.Historybox

                Case AirKlickArea.HistoryBoxRight
                    Return AirKlickAreaGroup.Historybox

                Case AirKlickArea.HistoryBoxSmall
                    Return AirKlickAreaGroup.Historybox

                Case AirKlickArea.HistoryBoxLeftScrollbar
                    Return AirKlickAreaGroup.Scrollbar

                Case AirKlickArea.HistoryBoxRightScrollbar
                    Return AirKlickAreaGroup.Scrollbar

                Case AirKlickArea.Spielfeld
                    Return AirKlickAreaGroup.Spielfeld

                Case AirKlickArea.Editor
                    Return AirKlickAreaGroup.Editor

                Case AirKlickArea.Undo
                    Return AirKlickAreaGroup.Button

                Case AirKlickArea.Redo
                    Return AirKlickAreaGroup.Button

                Case AirKlickArea.Header
                    Return AirKlickAreaGroup.Outside

                Case AirKlickArea.Outside
                    Return AirKlickAreaGroup.Outside

                Case Else
                    Return AirKlickAreaGroup.None

            End Select
        End Function

        Public Function AirIsDragDropKandidat(aka As AirKlickArea) As Boolean
            Select Case aka

                Case AirKlickArea.Stock
                    Return True

                Case AirKlickArea.Editor
                    Return True

                Case Else
                    Return False

            End Select
        End Function

    End Module
End Namespace