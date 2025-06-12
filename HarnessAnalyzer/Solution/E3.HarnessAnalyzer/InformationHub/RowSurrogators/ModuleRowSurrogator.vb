Imports Infragistics.Win.UltraWinDataSource
Imports Zuken.E3.Lib.Schema.Kbl.Properties

Friend Class ModulRowSurrogator
    Inherits ComparisonRowSurrogator(Of [Lib].Schema.Kbl.Module, ModulePartDummy)

    Public Sub New(kbl_reference As IKblContainer, kbl_compare As IKblContainer, corresponding_DS As UltraDataSource, mainForm As MainForm, isChildRowSurrogator As Boolean)
        MyBase.New(kbl_reference, kbl_compare, corresponding_DS, mainForm, isChildRowSurrogator)
    End Sub

    Protected Overrides Sub OnAfterOccurrenceRowInitialized(row As UltraDataRow)
        MyBase.OnAfterOccurrenceRowInitialized(row)
        If (row.Band.Columns.Exists(InformationHubStrings.DiffType_ColumnCaption)) AndAlso (row.GetCellValue(InformationHubStrings.DiffType_ColumnCaption).ToString = InformationHubStrings.Added_Text) Then
            IsReference = If(Me.HasCompare, False, True)
        End If
    End Sub

    Public Overrides Function CanGetCellValue(row As UltraDataRow, column As UltraDataColumn) As Boolean
        If row.ParentRow Is Nothing Then
            Return MyBase.CanGetCellValue(row, column)
        End If
        Return False
    End Function

    Protected Overrides Function GetCompareCellValueCore(row As UltraDataRow, column As UltraDataColumn, reference As Boolean) As Object
        Select Case column.Key
            Case "ZGS"
                Return (GetLastChange()?.Id).OrEmpty
            Case "KEM"
                Return (GetLastChange()?.Change_request).OrEmpty
            Case ModulePropertyName.Of_family
                If (Me.RowOccurrence.Of_family IsNot Nothing) Then
                    Dim mod_family As Module_family = Me.ReferenceObj.Kbl.GetOccurrenceObject(Of Module_family)(Me.RowOccurrence.Of_family)
                    If reference AndAlso mod_family IsNot Nothing Then
                        Return mod_family.Id
                    Else
                        mod_family = Me.CompareObj.Kbl.GetOccurrenceObject(Of Module_family)(Me.RowOccurrence.Of_family)
                        If Not reference AndAlso mod_family IsNot Nothing Then
                            Return mod_family.Id
                        End If
                    End If
                End If
                Return String.Empty
            Case ModulePropertyName.Logistic_control_information
                Return Me.RowOccurrence.Module_configuration.Logistic_control_information
            Case ModulePropertyName.Configuration_type
                If Me.RowOccurrence.Module_configuration.Configuration_typeSpecified Then
                    Return Me.RowOccurrence.Module_configuration.Configuration_type
                End If
                Return String.Empty
            Case ModulePropertyName.Controlled_components
                If Not String.IsNullOrEmpty(Me.RowOccurrence.Module_configuration?.Controlled_components) Then
                    Return Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS
                End If
                Return String.Empty
            Case Else
                Return MyBase.GetCompareCellValueCore(row, column, reference)
        End Select
        Return Nothing
    End Function

    Private Function GetLastChange() As Change
        If (Me.RowOccurrence.Change.Length > 0) Then
            Dim changes As List(Of Change) = Me.RowOccurrence.Change.Where(Function(change) change.Change_date IsNot Nothing AndAlso DateTime.TryParse(change.Change_date, New DateTime)).ToList
            If (changes.Count > 0) Then
                Return changes.Where(Function(change) CDate(change.Change_date).Ticks = changes.Max(Function(cng) CDate(cng.Change_date).Ticks)).LastOrDefault
            Else
                Return Me.RowOccurrence.Change.LastOrDefault
            End If
        End If
        Return Nothing
    End Function

    Public Class ModulePartDummy
        Inherits Part

        Public Sub New()
        End Sub
    End Class

End Class

