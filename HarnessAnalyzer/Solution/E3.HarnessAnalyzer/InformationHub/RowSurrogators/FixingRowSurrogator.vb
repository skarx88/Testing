Imports System.Data.Common
Imports System.Xml.Serialization
Imports Infragistics.Win.UltraWinDataSource
Imports Zuken.E3.Lib.Schema.Kbl.Properties

Friend Class FixingRowSurrogator
    Inherits ComparisonRowSurrogator(Of Fixing_occurrence, Fixing)

    Private _startNode As Node
    Private _segment As Segment = Nothing
    Private _segment_fixing_assignment As Fixing_assignment

    Public Sub New(kbl_reference As IKblContainer, kbl_compare As IKblContainer, corresponding_DS As UltraDataSource, mainForm As MainForm)
        MyBase.New(kbl_reference, kbl_compare, corresponding_DS, mainForm)
    End Sub

    Protected Overrides Sub OnAfterOccurrenceRowInitialized(row As UltraDataRow)
        MyBase.OnAfterOccurrenceRowInitialized(row)

        If Not IsReference AndAlso Me.HasCompare Then
            _segment = Me.CompareObj.Kbl.GetSegments.Where(Function(s) s.Fixing_assignment.Any(Function(fa) fa.Fixing = Me.CompareObj.SystemId)).FirstOrDefault
            If _segment IsNot Nothing Then
                _startNode = Me.CompareObj.Kbl.GetOccurrenceObject(Of Node)(_segment.Start_node)
                _segment_fixing_assignment = _segment.Fixing_assignment.Where(Function(fa) fa.Fixing = Me.CompareObj.SystemId).FirstOrDefault
            End If
        Else
            _segment = Me.ReferenceObj.Kbl.GetSegments.Where(Function(s) s.Fixing_assignment.Any(Function(fa) fa.Fixing = Me.ReferenceObj.SystemId)).FirstOrDefault
            If _segment IsNot Nothing Then
                _startNode = Me.ReferenceObj.Kbl.GetOccurrenceObject(Of Node)(_segment.Start_node)
                _segment_fixing_assignment = _segment.Fixing_assignment.Where(Function(fa) fa.Fixing = Me.ReferenceObj.SystemId).FirstOrDefault
            End If
        End If
    End Sub

    Public Overrides Function GetPartCellValue(kblPropertyName As String, reference As Boolean) As Object
        Select Case kblPropertyName
            Case FixingPropertyName.Fixing_type
                If (Me.RowPartObject.Fixing_type IsNot Nothing) Then
                    Return Me.RowPartObject.Fixing_type
                End If
                Return String.Empty
        End Select
        Return MyBase.GetPartCellValue(kblPropertyName, reference)
    End Function

    Public Overrides Function CanGetCellValue(row As UltraDataRow, column As UltraDataColumn) As Boolean
        If TypeOf row.Band.Tag Is Dictionary(Of String, Object) Then
            Return True
        End If
        Return MyBase.CanGetCellValue(row, column)
    End Function

    Protected Overrides Function GetCompareCellValueCore(row As UltraDataRow, column As UltraDataColumn, reference As Boolean) As Object
        Select Case column.Key
            Case FixingPropertyName.AssignmentProcessingInfos
                If (_segment_fixing_assignment?.Processing_information?.Length).GetValueOrDefault > 0 Then
                    Return Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS
                End If
                Return String.Empty
            Case SegmentPropertyName.Start_node
                If _startNode IsNot Nothing Then
                    Return _startNode.Id
                End If
                Return String.Empty
            Case FixingPropertyName.SegmentLocation
                If _segment_fixing_assignment IsNot Nothing Then
                    Return String.Format("{0} %", CInt(_segment_fixing_assignment.Location * 100))
                End If
                Return String.Empty
            Case FixingPropertyName.SegmentAbsolute_location
                If _segment_fixing_assignment.Absolute_location IsNot Nothing Then
                    Return GetNumericalCellValue(_segment_fixing_assignment.Absolute_location)
                End If
                Return String.Empty
            Case FixingPropertyName.Placement
                Dim fixingChanges As Dictionary(Of String, Object) = TryCast(row.Band.Tag, Dictionary(Of String, Object))
                If fixingChanges IsNot Nothing Then
                    Dim fcp As FixingChangedProperty = TryCast(fixingChanges.Values.First, FixingChangedProperty)
                    If fcp IsNot Nothing Then
                        If fcp.ChangedProperties.ContainsKey(InformationHub.PLACEMENT) Then
                            Return InformationHubStrings.PlacementModified
                        End If
                    End If
                End If
                Return String.Empty
            Case Else
                Return MyBase.GetCompareCellValueCore(row, column, reference)
        End Select
        Return Nothing
    End Function

End Class

