Imports System.Data.Common
Imports System.Xml.Serialization
Imports Infragistics.Win.UltraWinDataSource
Imports Zuken.E3.Lib.Schema.Kbl.Properties

Friend Class CoPackRowSurrogator
    Inherits ComparisonRowSurrogator(Of Co_pack_occurrence, Co_pack_part)

    Public Sub New(kbl_reference As IKblContainer, kbl_compare As IKblContainer, corresponding_DS As UltraDataSource, mainForm As MainForm)
        MyBase.New(kbl_reference, kbl_compare, corresponding_DS, mainForm)
    End Sub

    Protected Overrides Function GetCompareCellValueCore(row As UltraDataRow, column As UltraDataColumn, reference As Boolean) As Object
        Select Case column.Key
            Case CoPackPropertyName.Installation_Information
                If (Me.RowOccurrence.Installation_information.Any) Then
                    Return Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS
                End If
                Return String.Empty
            Case Else
                Return MyBase.GetCompareCellValueCore(row, column, reference)
        End Select
        Return Nothing
    End Function

End Class

