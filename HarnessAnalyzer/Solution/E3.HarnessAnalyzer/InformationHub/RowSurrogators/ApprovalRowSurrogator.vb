Imports Infragistics.Win.UltraWinDataSource
Imports Zuken.E3.Lib.Schema.Kbl.Properties

Friend Class ApprovalRowSurrogator
    Inherits ComparisonRowSurrogator(Of Approval, ApprovalPartDummy)

    Public Sub New(kbl_reference As IKblContainer, kbl_compare As IKblContainer, corresponding_DS As UltraDataSource, mainForm As MainForm)
        MyBase.New(kbl_reference, kbl_compare, corresponding_DS, mainForm)
    End Sub

    Protected Overrides Sub OnAfterOccurrenceRowInitialized(row As UltraDataRow)
        MyBase.OnAfterOccurrenceRowInitialized(row)
        If (row.Band.Columns.Exists(InformationHubStrings.DiffType_ColumnCaption)) AndAlso (row.GetCellValue(InformationHubStrings.DiffType_ColumnCaption).ToString = InformationHubStrings.Added_Text) Then
            IsReference = If(Me.HasCompare, False, True)
        End If
    End Sub

    Protected Overrides Function GetCompareCellValueCore(row As UltraDataRow, column As UltraDataColumn, reference As Boolean) As Object
        Select Case column.Key
            Case ApprovalPropertyName.Is_applied_to
                Dim kblMapper As IKblContainer = If(IsReference, Me.ReferenceObj.Kbl, Me.CompareObj.Kbl)
                If (kblMapper.HarnessSystemId = Me.RowOccurrence.Is_applied_to) Then
                    Return String.Format("{0} [{1}]", kblMapper.HarnessPartNumber, [Lib].Schema.Kbl.Resources.ObjectTypeStrings.Harness)
                ElseIf (kblMapper.GetHarnessConfiguration(Me.RowOccurrence.Is_applied_to) IsNot Nothing) Then
                    Return String.Format("{0} [{1}]", kblMapper.GetHarnessConfiguration(Me.RowOccurrence.Is_applied_to).Part_number, [Lib].Schema.Kbl.Resources.ObjectTypeStrings.ModuleConfiguration)
                ElseIf (kblMapper.GetHarness.GetModule(Me.RowOccurrence.Is_applied_to) IsNot Nothing) Then
                    Return String.Format("{0} [{1}]", kblMapper.GetModule(Me.RowOccurrence.Is_applied_to).Part_number, [Lib].Schema.Kbl.Resources.ObjectTypeStrings.HarnessModule)
                End If
            Case Else
                Return MyBase.GetCompareCellValueCore(row, column, reference)
        End Select
        Return Nothing
    End Function

    Public Class ApprovalPartDummy
        Inherits Part

        Public Sub New()
        End Sub
    End Class

End Class

