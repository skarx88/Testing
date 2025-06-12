Imports System.Data.Common
Imports Infragistics.Win.UltraWinDataSource
Imports Zuken.E3.Lib.Schema.Kbl.Properties

Friend Class AssemblyPartRowSurrogator
    Inherits ComparisonRowSurrogator(Of Assembly_part_occurrence, Assembly_part)

    Public Sub New(kbl_reference As IKblContainer, kbl_compare As IKblContainer, corresponding_DS As UltraDataSource, mainForm As MainForm)
        MyBase.New(kbl_reference, kbl_compare, corresponding_DS, mainForm)
    End Sub

    Public Overrides Function GetPartCellValue(kblPropertyName As String, reference As Boolean) As Object
        Select Case kblPropertyName
            Case Else
                Return MyBase.GetPartCellValue(kblPropertyName, reference)
        End Select
        Return Nothing
    End Function

    Protected Overrides Function GetCompareCellValueCore(row As UltraDataRow, column As UltraDataColumn, reference As Boolean) As Object
        Select Case column.Key
            Case Else
                Return MyBase.GetCompareCellValueCore(row, column, reference)
        End Select
        Return Nothing
    End Function

End Class

