Imports System.Data.Common
Imports System.Xml.Serialization
Imports Infragistics.Win.UltraWinDataSource
Imports Zuken.E3.Lib.Schema.Kbl.Properties

Friend Class ConnectorRowSurrogator
    Inherits ComparisonRowSurrogator(Of Connector_occurrence, Connector_housing)

    Public Sub New(kbl_reference As IKblContainer, kbl_compare As IKblContainer, corresponding_DS As UltraDataSource, mainForm As MainForm)
        MyBase.New(kbl_reference, kbl_compare, corresponding_DS, mainForm)
    End Sub

    Public Overrides Function CanGetCellValue(row As UltraDataRow, column As UltraDataColumn) As Boolean
        If row.ParentRow Is Nothing Then
            Return MyBase.CanGetCellValue(row, column)
        End If
        Return False
    End Function

    Protected Overrides Function GetCompareCellValueCore(row As UltraDataRow, column As UltraDataColumn, reference As Boolean) As Object
        Select Case column.Key
            Case ConnectorPropertyName.Usage
                If Me.RowOccurrence.UsageSpecified Then
                    Return Me.RowOccurrence.Usage.ToStringOrXmlName
                End If
                Return String.Empty
            Case InformationHub.CAVITY_COUNT_COLUMN_KEY
                If (Me.RowOccurrence.Slots IsNot Nothing) AndAlso (Me.RowOccurrence.Slots.Length > 0) Then
                    Return Me.RowOccurrence.Slots.First.Cavities.Length
                Else
                    Return 0
                End If
            Case ConnectorPropertyName.Placement
                If row.Band.Tag IsNot Nothing Then
                    If TryCast(row.Band.Tag, Dictionary(Of String, Object)) IsNot Nothing Then
                        Dim fcp As ConnectorChangedProperty = TryCast(DirectCast(row.Band.Tag, Dictionary(Of String, Object)).Values.First, ConnectorChangedProperty)
                        If fcp IsNot Nothing Then
                            If fcp.ChangedProperties.ContainsKey(InformationHub.PLACEMENT) Then
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

