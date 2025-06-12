Imports Infragistics.Win.UltraWinDataSource
Imports Zuken.E3.Lib.Schema.Kbl.Properties

Friend Class AccessoriesRowSurrogator
    Inherits ComparisonRowSurrogator(Of Accessory_occurrence, Accessory)

    Private _startNode As Node = Nothing
    Private _fixingAssignment As Fixing_assignment

    Public Const PLACEMENT As String = "Placement" 'corresponds to the placement property on fixings , accessories and connectors

    Public Sub New(kbl_reference As IKblContainer, kbl_compare As IKblContainer, corresponding_DS As UltraDataSource, mainForm As MainForm)
        MyBase.New(kbl_reference, kbl_compare, corresponding_DS, mainForm)
    End Sub

    Protected Overrides Sub OnAfterOccurrenceRowInitialized(row As UltraDataRow)
        MyBase.OnAfterOccurrenceRowInitialized(row)

        If Me.RowPartObject IsNot Nothing Then
            If Not IsReference() AndAlso Me.CompareObj IsNot Nothing Then
                _startNode = Me.RowOccurrence.GetStartNode(Me.CompareObj.Kbl, _fixingAssignment)
            Else
                _startNode = Me.RowOccurrence.GetStartNode(Me.ReferenceObj.Kbl, _fixingAssignment)
            End If
        End If
    End Sub

    Protected Overrides Function GetCompareCellValueCore(row As UltraDataRow, column As UltraDataColumn, reference As Boolean) As Object
        Select Case column.Key
            Case AccessoryPropertyName.Reference_element
                MyBase.GetReferenceElementCellValue()
                If Me.RowOccurrence.Reference_element IsNot Nothing Then
                    If Me.RowOccurrence.Reference_element.SplitSpace.Length = 1 Then
                        Return Harness.GetReferenceElement(Me.RowOccurrence.Reference_element, If(reference, Me.ReferenceObj.Kbl, Me.CompareObj.Kbl))
                    ElseIf Me.RowOccurrence.Reference_element.SplitSpace.Length > 1 Then
                        Return Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS
                    End If
                End If
            Case SegmentPropertyName.Start_node
                Return _startNode?.Id
            Case FixingPropertyName.SegmentLocation
                If _fixingAssignment IsNot Nothing Then
                    Return String.Format("{0} %", CInt(_fixingAssignment.Location * 100))
                End If
            Case FixingPropertyName.SegmentAbsolute_location
                If _fixingAssignment?.Absolute_location IsNot Nothing Then
                    Return String.Format("{0} {1}", Math.Round(_fixingAssignment.Absolute_location.Value_component, 2), _fixingAssignment.Absolute_location.GetUnitName(Me.ReferenceObj.Kbl))
                End If
            Case InformationHubStrings.AssignedModules_ColumnCaption
                Return Me.GetAssignedModulesCellValue
            Case AccessoryPropertyName.Placement
                If row.Band.Tag IsNot Nothing Then
                    If TryCast(row.Band.Tag, Dictionary(Of String, Object)) IsNot Nothing Then
                        Dim fcp As AccessoryChangedProperty = TryCast(DirectCast(row.Band.Tag, Dictionary(Of String, Object)).Values.First, AccessoryChangedProperty)
                        If fcp IsNot Nothing Then
                            If fcp.ChangedProperties.ContainsKey(PLACEMENT) Then
                                Return InformationHubStrings.PlacementModified
                            End If
                        End If
                    End If
                End If
            Case Else
                Return MyBase.GetCompareCellValueCore(row, column, reference)
        End Select
        Return Nothing
    End Function

End Class

