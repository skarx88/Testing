Imports System.Data.Common
Imports System.Xml.Serialization
Imports Infragistics.Win.UltraWinDataSource
Imports Zuken.E3.Lib.Schema.Kbl.Properties

Friend Class DefaultDimensionSpecificationRowSurrogator
    Inherits ComparisonRowSurrogator(Of Default_dimension_specification, DefaultDimSpecPartDummy)

    Public Sub New(kbl_reference As IKblContainer, kbl_compare As IKblContainer, corresponding_DS As UltraDataSource, mainForm As MainForm)
        MyBase.New(kbl_reference, kbl_compare, corresponding_DS, mainForm)
    End Sub

    Protected Overrides Function GetCompareCellValueCore(row As UltraDataRow, column As UltraDataColumn, reference As Boolean) As Object
        Select Case column.Key
            Case DefaultDimensionSpecificationPropertyName.Dimension_value_range
                If (Me.RowOccurrence.Dimension_value_range IsNot Nothing) Then
                    Return GetValueRangeCellValue(Me.RowOccurrence.Dimension_value_range)
                End If
                Return String.Empty
            Case DefaultDimensionSpecificationPropertyName.External_references
                If (Me.RowOccurrence.External_references IsNot Nothing) AndAlso (Me.RowOccurrence.External_references.SplitSpace.Length > 0) Then
                    Return Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS
                End If
                Return String.Empty
            Case DefaultDimensionSpecificationPropertyName.Tolerance_indication
                Return DimensionSpecificationRowSurrogator.GetToleranceIndicationCellValue(Me, Me.RowOccurrence.Tolerance_indication)
            Case Else
                Return MyBase.GetCompareCellValueCore(row, column, reference)
        End Select
        Return Nothing
    End Function

    Public Class DefaultDimSpecPartDummy
        Inherits Part

        Public Sub New()
        End Sub
    End Class

End Class

