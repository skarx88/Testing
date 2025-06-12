Imports System.Data.Common
Imports Infragistics.Win.UltraWinDataSource
Imports Zuken.E3.Lib.Schema.Kbl.Properties

Friend Class ChangeDescriptionRowSurrogator
    Inherits ComparisonRowSurrogator(Of Change_description, ChangeDescriptionPartDummy)

    Public Sub New(kbl_reference As IKblContainer, kbl_compare As IKblContainer, corresponding_DS As UltraDataSource, mainForm As MainForm, isChildRowSurrogator)
        MyBase.New(kbl_reference, kbl_compare, corresponding_DS, mainForm, isChildRowSurrogator)
    End Sub

    Protected Overrides Function GetCompareCellValueCore(row As UltraDataRow, column As UltraDataColumn, reference As Boolean) As Object
        Select Case column.Key
            Case ChangeDescriptionPropertyName.Changed_elements
                If Not String.IsNullOrEmpty(Me.RowOccurrence?.Changed_elements) Then
                    Dim changedElementIds As String() = Me.RowOccurrence.Changed_elements.SplitSpace.ToArray
                    If changedElementIds.Length = 1 Then
                        Return Change_description.GetChangedElement(changedElementIds.FirstOrDefault, If(IsReference, Me.ReferenceObj.Kbl, Me.CompareObj.Kbl))
                        'needs data object with link
                    Else
                        Return Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS
                    End If
                End If
            Case ChangeDescriptionPropertyName.External_references,
                 ChangeDescriptionPropertyName.Related_changes
                Return GetEllipsisPropertyValue(column.Key, CompareSurrogatorObjectSource.RowOccurrence)
            Case Else
                Return MyBase.GetCompareCellValueCore(row, column, reference)
        End Select

        Return Nothing
    End Function

    Friend Class ChangeDescriptionPartDummy
        Inherits Part

        Public Sub New()
            MyBase.New
        End Sub
    End Class

End Class

