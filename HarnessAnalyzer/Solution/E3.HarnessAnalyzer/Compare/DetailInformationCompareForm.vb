Imports Infragistics.Win.UltraWinDataSource
Imports Infragistics.Win.UltraWinGrid

'TODO: there are currently lot's of compare stuff within the DetailInformationForm-base that should be moved to here in the future (currently only the InitializeRow is moved because of the needed distinction between compare and not compare from Informatio
Public Class DetailInformationCompareForm
    Inherits DetailInformationForm

    Public Sub New(caption As String, displayData As Object, Optional inactiveObjects As IDictionary(Of String, IEnumerable(Of String)) = Nothing, Optional kblMapper As KblMapper = Nothing, Optional objectId As String = Nothing, Optional wireLengthType As String = Nothing, Optional objectType As String = Nothing)
        MyBase.New(caption, displayData, inactiveObjects, kblMapper, objectId, wireLengthType, objectType)
    End Sub

    Protected Overrides Sub InitializeDetailInformationRowCore(row As UltraGridRow)
        'HINT: override the base initializeRow-logic (thus ignores the highlight of the wire length type from general settings here because this conflicts with our added (bold!) /deleted-logic of the rows in compare !)
        Dim dataRow As UltraDataRow = DirectCast(row.ListObject, UltraDataRow)
        If dataRow.Tag IsNot Nothing Then

            Dim chgType As Nullable(Of CompareChangeType) = GetChangeType(dataRow?.Tag)
            If chgType = CompareChangeType.[New] Then
                row.Appearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True
            ElseIf chgType = CompareChangeType.Deleted Then
                row.Appearance.FontData.Strikeout = Infragistics.Win.DefaultableBoolean.True
            ElseIf (TypeOf dataRow.Tag Is String) AndAlso (row.HasCell(DetailInformationFormStrings.ObjId_PropName)) Then
                With row.Cells(DetailInformationFormStrings.ObjId_PropName)
                    .Appearance.ForeColor = [Shared].CHANGED_MODIFIED_FORECOLOR.ToColor

                    If (dataRow.Tag?.ToString = String.Empty) Then
                        .ToolTipText = String.Format("{0}{1}{2}{3}{4}", InformationHubStrings.RefDoc_TooltipPart, .Value.ToString, vbLf, vbLf, InformationHubStrings.CompDoc1_TooltipPart)
                    Else
                        .ToolTipText = String.Format("{0}{1}{2}{3}{4}{5}", InformationHubStrings.RefDoc_TooltipPart, .Value.ToString, vbLf, vbLf, InformationHubStrings.CompDoc1_TooltipPart, dataRow.Tag?.ToString)
                    End If

                    If (.Value.ToString = String.Empty) AndAlso (.ToolTipText <> String.Empty) Then
                        DirectCast(.Row.ListObject, UltraDataRow).SetCellValue(.Column.Key, "-")
                    End If
                End With
            ElseIf (TypeOf dataRow.Tag Is Dictionary(Of String, Object)) Then
                SetGridCellTooltip(row, dataRow)
            End If
        End If

        For Each cell As UltraGridCell In row.Cells
            If (cell.Value IsNot Nothing AndAlso cell.Value.ToString = Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS) Then
                cell.Style = ColumnStyle.Button
            End If
        Next
    End Sub

End Class
