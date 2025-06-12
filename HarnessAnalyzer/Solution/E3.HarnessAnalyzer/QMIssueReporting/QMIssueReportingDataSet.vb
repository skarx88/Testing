Namespace IssueReporting
    Partial Class QMIssueReportingDataSet
        Private WithEvents _bindingSource As BindingSource

        Partial Class IssuesDataTable

            Public Function AddIssueRow(issue As Issue) As IssuesRow
                With issue
                    Dim rowIssuesRow As IssuesRow = CType(Me.NewRow, IssuesRow)
                    Dim date1 As Object = Nothing
                    If .DateOfConfirmation.HasValue Then date1 = issue.DateOfConfirmation.Value
                    Dim columnValuesArray() As Object = New Object() { .Id, .ObjectReference, .Description, .IssueTag, .NofOccurrences, .ConfirmedBy, date1, .ConfirmationComment, issue}
                    rowIssuesRow.ItemArray = columnValuesArray
                    Me.Rows.Add(rowIssuesRow)
                    Return rowIssuesRow
                End With
            End Function

            Public Sub SubmitChanges()
                For Each row As IssuesRow In Me.GetChanges.Rows
                    Dim issue As Issue = CType(row.ListObject, IssueReporting.Issue)
                    With issue
                        If Not row.IsConfirmationCommentNull Then .ConfirmationComment = row.ConfirmationComment Else .ConfirmationComment = Nothing
                        If Not row.IsConfirmedByNull Then .ConfirmedBy = row.ConfirmedBy Else .ConfirmedBy = Nothing
                        If Not row.IsDateOfConfirmationNull Then .DateOfConfirmation = row.DateOfConfirmation Else .DateOfConfirmation = Nothing
                        If Not row.IsDescriptionNull Then .Description = row.Description Else .Description = Nothing
                        If Not row.IsIssueTagNull Then .IssueTag = row.IssueTag Else .IssueTag = Nothing
                    End With
                Next
                Me.AcceptChanges()
            End Sub

        End Class


    End Class

End Namespace