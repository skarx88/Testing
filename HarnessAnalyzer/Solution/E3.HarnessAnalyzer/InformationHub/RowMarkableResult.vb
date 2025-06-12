Imports Infragistics.Win.UltraWinGrid

Friend Class RowMarkableResult

    Public Sub New(row As UltraGridRow, isMarkRow As Boolean)
        Me.New(row, Array.Empty(Of UltraGridRow))
        Me.CanBeMarkRow = isMarkRow
    End Sub

    Public Sub New(row As UltraGridRow)
        Me.New(row, Array.Empty(Of UltraGridRow))
    End Sub

    Public Sub New(row As UltraGridRow, children As IEnumerable(Of UltraGridRow), Optional isMarkRow As Nullable(Of Boolean) = Nothing)
        Me.Row = row
        If children IsNot Nothing Then
            Me.Children.AddRange(children)
        End If

        If Not isMarkRow.HasValue Then
            Me.CanBeMarkRow = InformationHubUtils.CanSetMarkedRowAppearance(row)
        Else
            Me.CanBeMarkRow = isMarkRow.Value
        End If
    End Sub

    Property CanBeMarkRow As Boolean
    ReadOnly Property Row As UltraGridRow
    ReadOnly Property Children As New List(Of UltraGridRow)

    Public Sub EnsureVisible(Optional expandChildIfChild As Boolean = True)
        If HasChildren Then
            InformationHubUtils.EnsureRowVisible(Children.First, expandChildIfChild)
        Else
            InformationHubUtils.EnsureRowVisible(Row, False)
        End If
    End Sub

    ReadOnly Property HasChildren As Boolean
        Get
            Return (Children?.Count).GetValueOrDefault > 0
        End Get
    End Property

    Public Sub UpdateColors()
        If CanBeMarkRow Then
            InformationHubUtils.SetMarkedRowAppearance(Row)
            If HasChildren Then
                For Each child_row As UltraGridRow In Children
                    InformationHubUtils.SetMarkedRowAppearance(child_row)
                Next
            End If
        Else
            InformationHubUtils.ResetMarkedRowAppearance(Row)
            If HasChildren Then
                For Each child_row As UltraGridRow In Children
                    InformationHubUtils.ResetMarkedRowAppearance(child_row)
                Next
            End If
        End If
    End Sub

End Class
