Imports Infragistics.Win.UltraWinDataSource
Imports Zuken.E3.Lib.Schema.Kbl.Properties

Friend Class WireCoreRowSurrogator(Of TOccurrence As {IKblWireCoreOccurrence, New}, TPart As {IKblWireCorePart, New})
    Inherits ComparisonRowSurrogator(Of TOccurrence, TPart)

    Private _connection As Connection
    Private _wiringGroups As List(Of Wiring_group)

    Public Sub New(kbl_reference As IKblContainer, kbl_compare As IKblContainer, corresponding_DS As UltraDataSource, mainForm As MainForm)
        MyBase.New(kbl_reference, kbl_compare, corresponding_DS, mainForm)
    End Sub

    Protected Overrides Sub OnAfterOccurrenceRowInitialized(row As UltraDataRow)
        MyBase.OnAfterOccurrenceRowInitialized(row)

        If Not IsReference() AndAlso Me.CompareObj IsNot Nothing Then
            _connection = Me.CompareObj.GetConnection
            _wiringGroups = Me.CompareObj.GetWiringGroups
        Else
            _connection = Me.ReferenceObj.GetConnection
            _wiringGroups = Me.ReferenceObj.GetWiringGroups
        End If
    End Sub

    Public Shadows ReadOnly Property Compare As WireCoreRowSurrogator
        Get
            Return CType(MyBase.CompareObj, WireCoreRowSurrogator)
        End Get
    End Property

    Public Shadows ReadOnly Property Reference As WireCoreRowSurrogator
        Get
            Return CType(MyBase.ReferenceObj, WireCoreRowSurrogator)
        End Get
    End Property

    Public Overrides Function GetPartCellValue(kblPropertyName As String, reference As Boolean) As Object
        Select Case kblPropertyName
            Case WirePropertyName.Bend_radius
                Dim bend_radius As Numerical_value = CType(Me.GetRowPropertyValue(kblPropertyName, CompareSurrogatorObjectSource.RowPart), Numerical_value)
                If bend_radius IsNot Nothing Then
                    Return GetNumericalCellValue(bend_radius,)
                End If
            Case WirePropertyName.Cross_section_area.ToString
                If Me.RowPartObject?.Cross_section_area IsNot Nothing Then
                    Return GetNumericalCellValue(Me.RowPartObject.Cross_section_area,)
                End If
            Case WirePropertyName.Outside_diameter.ToString
                Dim outside_diameter As Numerical_value = CType(Me.GetRowPropertyValue(kblPropertyName, CompareSurrogatorObjectSource.RowPart), Numerical_value)
                If outside_diameter IsNot Nothing Then
                    Return GetNumericalCellValue(outside_diameter)
                End If
            Case WirePropertyName.Core_Colour
                Return Me.RowPartObject?.GetColours
            Case Else
                Return MyBase.GetPartCellValue(kblPropertyName, reference)
        End Select
        Return Nothing
    End Function

    Protected Overrides Function GetCompareCellValueCore(row As UltraDataRow, column As UltraDataColumn, reference As Boolean) As Object
        Select Case column.Key
            Case InformationHub.WIRE_CLASS_COLUMN_KEY
                Return E3.Lib.Schema.Kbl.Utils.GetLocalizedObjectName(KblObjectType.Wire_occurrence, System.Globalization.CultureInfo.CurrentUICulture)
            Case WirePropertyName.Wire_number
                Return MyBase.GetRowPropertyValue(column.Key, CompareSurrogatorObjectSource.RowOccurrence)
            Case WirePropertyName.Wiring_group
                Dim wg As Wiring_group = Me.RowOccurrence.GetWiringGroup(_wiringGroups)
                If wg IsNot Nothing Then
                    If HasCompare Then
                        Return wg.Id
                    Else
                        Return Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS
                    End If
                End If
            Case WirePropertyName.Cable_designator
                Return MyBase.GetRowPropertyValue(column.Key, CompareSurrogatorObjectSource.RowOccurrence)
            Case WirePropertyName.Wire_type
                Return MyBase.GetRowPropertyValue(column.Key, CompareSurrogatorObjectSource.RowOccurrence)
            Case WirePropertyName.Installation_Information
                GetEllipsisPropertyValue(column.Key, CompareSurrogatorObjectSource.RowOccurrence)
            Case WirePropertyName.Length_information
                If Me.RowOccurrence?.Length_information.Length > 0 Then
                    If HasCompare Then
                        For Each wireLength As Wire_length In Me.RowOccurrence.Length_information.OrEmpty
                            If (wireLength.Length_type.ToLower = GeneralSettings.DefaultWireLengthType.ToLower) Then
                                Dim wire_length_unit_name As String = Me.ReferenceObj?.GetUnit(wireLength.Length_value)?.Unit_name
                                Return String.Format("{0} {1}", Math.Round(wireLength.Length_value.Value_component, 2), wire_length_unit_name.OrEmpty)
                            End If
                        Next
                    Else
                        If Me.RowOccurrence.Length_information.Count = 1 Then
                            Return GetNumericalCellValue(Me.RowOccurrence.Length_information.First.Length_value)
                        Else
                            Return Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS
                        End If
                    End If
                End If
            Case InformationHubStrings.AddLengthInfo_ColumnCaption
                Return Me.GetEllipsisPropertyValue(column.Key, CompareSurrogatorObjectSource.RowOccurrence, 2)
            Case ConnectionPropertyName.Connector_A
                Return GetCellValueOfContactPoint(_connection?.GetStartContactPointId, reference)
            Case ConnectionPropertyName.Cavity_A
                Return GetCellValueOfCavityContactPoint(_connection?.GetStartContactPointId, reference)
            Case ConnectionPropertyName.Connector_B
                Return GetCellValueOfContactPoint(_connection?.GetEndContactPointId, reference)
            Case ConnectionPropertyName.Cavity_B
                Return GetCellValueOfCavityContactPoint(_connection?.GetEndContactPointId, reference)
            Case ConnectionPropertyName.Localized_description
                Return GetLocalizedDescriptionCellValue(_connection)
            Case WirePropertyName.Routing
                Dim segments As List(Of Segment) = GetSegments(reference)
                If segments.Count = 1 Then
                    Return segments(0).Id
                ElseIf segments.Count > 1 Then
                    Return Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS
                End If
            Case WirePropertyName.AdditionalExtremities.ToString
                Me.GetEllipsisPropertyValue(column.Key, CompareSurrogatorObjectSource.RowOccurrence, 3)
            Case InformationHubStrings.AssignedModules_ColumnCaption
                Return Me.GetAssignedModulesCellValue()
            Case Else
                Return MyBase.GetCompareCellValueCore(row, column, reference)
        End Select
        Return Nothing
    End Function

    Private Function GetSegments(reference As Boolean) As List(Of Segment)
        If reference Then
            Return Me.Reference.GetSegments
        ElseIf Not reference AndAlso HasCompare Then
            Return Me.Compare.GetSegments
        End If
        Return New List(Of Segment)
    End Function

    Private Function GetCellValueOfCavityContactPoint(contactPointId As String, reference As Boolean) As Object
        Dim ref_cavity As Cavity_occurrence = Me.ReferenceObj.Kbl.GetCavityOccurrenceOfContactPointId(contactPointId)
        If reference AndAlso ref_cavity IsNot Nothing Then
            Return CType(ref_cavity.GetPart(Me.ReferenceObj.Kbl), Cavity).Cavity_number
        Else
            Dim comp_cavity As Cavity_occurrence = Me.CompareObj.Kbl.GetCavityOccurrenceOfContactPointId(contactPointId)
            If Not reference AndAlso comp_cavity IsNot Nothing Then
                Return DirectCast(comp_cavity.GetPart(Me.CompareObj.Kbl), Cavity).Cavity_number
            End If
        End If
        Return Nothing
    End Function

    Private Function GetCellValueOfContactPoint(contactPointId As String, reference As Boolean) As Object
        If Not String.IsNullOrEmpty(contactPointId) Then
            Dim ref_connector As Connector_occurrence = Me.ReferenceObj.Kbl.GetConnectorOfContactPoint(contactPointId)
            If reference AndAlso ref_connector IsNot Nothing Then
                If Not Me.HasCompare AndAlso Not ref_connector.Description.IsNullOrEmpty Then
                    Return String.Format("{0} - {1}", ref_connector.Id, ref_connector.Description)
                Else
                    Return ref_connector.Id
                End If
            Else
                Dim comp_box_id As String = Nothing
                If reference AndAlso CType(Me.ReferenceObj.Kbl, IKblMappersProvider).KBLContactPointComponentBoxMapper.TryGetValue(contactPointId, comp_box_id) Then
                    Dim ref_comp_box As Component_box_occurrence = ReferenceObj.Kbl.GetOccurrenceObject(Of Component_box_occurrence)(comp_box_id)
                    If Not HasCompare AndAlso Not ref_comp_box.Description.IsNullOrEmpty Then
                        Return String.Format("{0} - {1}", ref_comp_box.Id, ref_comp_box.Description)
                    Else
                        Return ref_comp_box.Id
                    End If
                ElseIf Me.HasCompare Then
                    Dim comp_conn_occ As Connector_occurrence = Me.CompareObj.Kbl.GetConnectorOfContactPoint(contactPointId)
                    If comp_conn_occ IsNot Nothing Then
                        Return comp_conn_occ.Id
                    ElseIf CType(Me.CompareObj.Kbl, IKblMappersProvider).KBLContactPointComponentBoxMapper.TryGetValue(contactPointId, comp_box_id) Then
                        Dim ref_comp_box As Component_box_occurrence = CompareObj.Kbl.GetOccurrenceObject(Of Component_box_occurrence)(comp_box_id)
                        If ref_comp_box IsNot Nothing Then
                            Return ref_comp_box.Id
                        End If
                    End If
                End If
            End If
        End If
        Return Nothing
    End Function

    Protected Overrides Function CreateObjectSurrogator(kbl As IKblContainer) As OccurrenceRowSurrogator(Of TOccurrence, TPart)
        Return New WireCoreRowSurrogator(kbl)
    End Function

    Public Class WireCoreRowSurrogator
        Inherits OccurrenceRowSurrogator(Of TOccurrence, TPart)

        Public Sub New(kbl As IKblContainer)
            MyBase.New(kbl)
        End Sub

        Public Function GetSegments() As List(Of Segment)
            Dim segments As New Dictionary(Of String, Segment)
            For Each seg As Segment In Me.Kbl.GetSegmentsOfWireOrCore(Me.SystemId)
                segments.TryAdd(seg.SystemId, seg)
            Next
            Return segments.Values.ToList
        End Function

    End Class

End Class

