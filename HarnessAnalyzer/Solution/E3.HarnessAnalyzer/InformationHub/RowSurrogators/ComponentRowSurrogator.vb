Imports System.Data.Common
Imports Infragistics.Win.UltraWinDataSource
Imports Zuken.E3.Lib.Schema.Kbl.Properties

Friend Class ComponentRowSurrogator
    Inherits ComparisonRowSurrogator(Of Component_occurrence, Component)

    Private Const COMPONENT_CLASS_COLUMN_KEY As String = "ComponentClass"

    Public Sub New(kbl_reference As IKblContainer, kbl_compare As IKblContainer, corresponding_DS As UltraDataSource, mainForm As MainForm)
        MyBase.New(kbl_reference, kbl_compare, corresponding_DS, mainForm)
    End Sub

    Protected Overrides Function GetCompareCellValueCore(row As UltraDataRow, column As UltraDataColumn, reference As Boolean) As Object
        Select Case column.Key
            Case COMPONENT_CLASS_COLUMN_KEY
                Select Case Me.RowOccurrence.ObjectType
                    Case KblObjectType.Fuse_occurrence
                        Return [Lib].Schema.Kbl.Resources.ObjectTypeStrings.Fuse
                    Case Else
                        Return [Lib].Schema.Kbl.Resources.ObjectTypeStrings.Component
                End Select
            Case ComponentPropertyName.Mounting
                If Not Me.RowOccurrence.Mounting.IsNullOrEmpty Then
                    If Me.RowOccurrence.GetMountingIds.Length = 1 Then
                        Dim mounting As OccurrencePartInfo = Me.RowOccurrence.GetMounting(Me.ReferenceObj.Kbl).SingleOrDefault
                        If mounting IsNot Nothing Then
                            Select Case mounting?.Occurrence.ObjectType
                                Case KblObjectType.Cavity_occurrence
                                    Dim conn_occ As Connector_occurrence = Me.ReferenceObj.Kbl.GetConnectorOfCavity(mounting.Occurrence.SystemId)
                                    Return String.Format("{0},{1}", conn_occ.Id, CType(mounting.Part, Cavity).Cavity_number)
                                Case KblObjectType.Connector_occurrence
                                    Return mounting.Occurrence.Id
                                Case KblObjectType.Slot_occurrence
                                    Dim connectorOccurrence As Connector_occurrence = Me.ReferenceObj.Kbl.GetConnectorOfSlot(Me.RowOccurrence.Mounting)
                                    Dim cavityOcc As Cavity_occurrence = CType(mounting.Occurrence, Slot_occurrence).Cavities.FirstOrDefault
                                    If cavityOcc IsNot Nothing Then
                                        Return String.Format("{0},{1}", connectorOccurrence.Id, Me.ReferenceObj.Kbl.GetPart(Of Cavity)(cavityOcc.Part).Cavity_number)
                                    End If
                            End Select
                        End If
                        Return Me.RowOccurrence.GetMountingIds.First
                    Else
                        Return Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS
                    End If
                End If
            Case ComponentPropertyName.ComponentPinMaps
                Return Me.GetEllipsisPropertyValue(column.Key, CompareSurrogatorObjectSource.RowOccurrence)
            Case Else
                Return MyBase.GetCompareCellValueCore(row, column, reference)
        End Select
        Return Nothing
    End Function

End Class

