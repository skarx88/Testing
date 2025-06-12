Imports Infragistics.Win.UltraWinDataSource
Imports Zuken.E3.Lib.Schema.Kbl.Properties

Friend Class ConnectionRowSurrogator
    Inherits ComparisonRowSurrogator(Of [Lib].Schema.Kbl.Connection, ConnectionPartDummy)

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
            Case ConnectionPropertyName.Wire
                Dim wire_occ = Me.ReferenceObj.Kbl.GetWireOrCoreOccurrence(Me.RowOccurrence.Wire)
                If (reference) AndAlso wire_occ.ObjectType = KblObjectType.Core Then
                    Return String.Format("{0} [{1}]", Me.ReferenceObj.Kbl.KBLCoreCableNameMapper(.Wire), DirectCast(Me.ReferenceObj.Kbl.KBLOccurrenceMapper(.Wire), Core_occurrence).Wire_number)
                ElseIf (reference) AndAlso (Me.ReferenceObj.Kbl.KBLOccurrenceMapper.ContainsKey(.Wire) AndAlso TypeOf Me.ReferenceObj.Kbl.KBLOccurrenceMapper(.Wire) Is Special_wire_occurrence) Then
                    Return Me.ReferenceObj.Kbl.KBLCoreCableNameMapper(.Wire)
                ElseIf (reference) AndAlso (Me.ReferenceObj.Kbl.KBLOccurrenceMapper.ContainsKey(.Wire) AndAlso TypeOf Me.ReferenceObj.Kbl.KBLOccurrenceMapper(.Wire) Is Wire_occurrence) Then
                    Return DirectCast(Me.ReferenceObj.Kbl.KBLOccurrenceMapper(.Wire), Wire_occurrence).Wire_number
                ElseIf (Not reference) AndAlso (Me.CompareObj.Kbl IsNot Nothing) AndAlso (Me.CompareObj.Kbl.KBLOccurrenceMapper.ContainsKey(.Wire) AndAlso TypeOf Me.CompareObj.Kbl.KBLOccurrenceMapper(.Wire) Is Core_occurrence) Then
                    Return String.Format("{0} [{1}]", Me.CompareObj.Kbl.KBLCoreCableNameMapper(.Wire), DirectCast(Me.CompareObj.Kbl.KBLOccurrenceMapper(.Wire), Core_occurrence).Wire_number)
                ElseIf (Not reference) AndAlso (Me.CompareObj.Kbl IsNot Nothing) AndAlso (Me.CompareObj.Kbl.KBLOccurrenceMapper.ContainsKey(.Wire) AndAlso TypeOf Me.CompareObj.Kbl.KBLOccurrenceMapper(.Wire) Is Special_wire_occurrence) Then
                    Return Me.CompareObj.Kbl.KBLCoreCableNameMapper(.Wire)
                ElseIf (Not reference) AndAlso (Me.CompareObj.Kbl IsNot Nothing) AndAlso (Me.CompareObj.Kbl.KBLOccurrenceMapper.ContainsKey(.Wire) AndAlso TypeOf Me.CompareObj.Kbl.KBLOccurrenceMapper(.Wire) Is Wire_occurrence) Then
                    Return DirectCast(Me.CompareObj.Kbl.KBLOccurrenceMapper(.Wire), Wire_occurrence).Wire_number
                Else
                    Return .Wire
                End If
            Case ConnectionPropertyName.Processing_Information
                If (Me.RowOccurrence.Processing_information.Length > 0) Then
                    Return Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS
                End If
            Case ConnectionPropertyName.Nominal_voltage.ToString
                If Not String.IsNullOrEmpty(Me.RowOccurrence.Nominal_voltage) Then
                    Return Me.RowOccurrence.Nominal_voltage
                End If
            Case Else
                Return MyBase.GetCompareCellValueCore(row, column, reference)
        End Select
        Return Nothing
    End Function

    Public Class ConnectionPartDummy
        Inherits Part

        Public Sub New()
        End Sub
    End Class

End Class

