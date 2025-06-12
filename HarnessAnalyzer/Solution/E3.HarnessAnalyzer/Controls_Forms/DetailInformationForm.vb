Imports System.Globalization
Imports System.IO
Imports System.Reflection
Imports System.Xml.Serialization
Imports Infragistics.Win.UltraWinDataSource
Imports Infragistics.Win.UltraWinGrid
Imports Zuken.E3.Lib.Schema.Kbl.Properties
Imports Zuken.E3.HarnessAnalyzer.Shared

<Reflection.Obfuscation(Feature:="renaming", ApplyToMembers:=False, Exclude:=True)> ' needed to avoid resource-file-names obfuscation errors, see issue #5
Public Class DetailInformationForm

    Friend Event SelectionChanged(sender As DetailInformationForm, e As DetailInformationEventArgs)

    Private _displayData As Object
    Private _exportFileName As String
    Private _inactiveObjects As IDictionary(Of String, IEnumerable(Of String))
    Private _kblMapper As KblMapper
    Private _keyMapper As IDictionary(Of String, String)
    Private _objectId As String
    Private _objectType As String
    Private _wireLengthType As String

    Public Sub New(caption As String, displayData As Object, Optional inactiveObjects As IDictionary(Of String, IEnumerable(Of String)) = Nothing, Optional kblMapper As KblMapper = Nothing, Optional objectId As String = Nothing, Optional wireLengthType As String = Nothing, Optional objectType As String = Nothing)
        InitializeComponent()

        _displayData = displayData
        _inactiveObjects = inactiveObjects
        _kblMapper = kblMapper
        _keyMapper = New Dictionary(Of String, String)
        _objectId = objectId
        _objectType = If(objectType Is Nothing, String.Empty, objectType)
        _wireLengthType = wireLengthType

        Initialize(caption)
    End Sub

    Friend Shared Function CreateInstance(type As Type, caption As String, displayData As Object, Optional inactiveObjects As Dictionary(Of String, IEnumerable(Of String)) = Nothing, Optional kblMapper As KblMapper = Nothing, Optional objectId As String = Nothing, Optional wireLengthType As String = Nothing, Optional objectType As String = Nothing) As DetailInformationForm
        Return CType(Activator.CreateInstance(type, caption, displayData, inactiveObjects, kblMapper, objectId, wireLengthType, objectType), DetailInformationForm)
    End Function

    Private Sub AddAliasIdRow(aliasId As Alias_identification, tag As Object)
        Dim row As UltraDataRow = Me.udsDetailInformation.Rows.Add
        row.Tag = tag

        row.SetCellValue(DetailInformationFormStrings.AliasId_ColCaption, aliasId.Alias_id)

        If (aliasId.Scope IsNot Nothing) Then
            row.SetCellValue(DetailInformationFormStrings.Scope_ColCaption, aliasId.Scope)
        End If
        If (aliasId.Description IsNot Nothing) Then
            row.SetCellValue(DetailInformationFormStrings.Description_ColCaption, aliasId.Description)
        End If
    End Sub

    Private Sub AddCavityRows(part As Part, tag As Object)
        Dim row As UltraDataRow

        With part
            row = Me.udsDetailInformation.Rows.Add
            row.SetCellValue(DetailInformationFormStrings.PropName_ColCaption, DetailInformationFormStrings.PartNumber_PropName)
            row.SetCellValue(DetailInformationFormStrings.PropVal_ColCaption, .Part_number)

            _keyMapper.Add(PartPropertyName.Part_number.ToString, DetailInformationFormStrings.PartNumber_PropName)

            row = Me.udsDetailInformation.Rows.Add
            row.SetCellValue(DetailInformationFormStrings.PropName_ColCaption, DetailInformationFormStrings.CompanyName_PropName)
            row.SetCellValue(DetailInformationFormStrings.PropVal_ColCaption, .Company_name)

            _keyMapper.Add(PartPropertyName.Company_name.ToString, DetailInformationFormStrings.CompanyName_PropName)

            row = Me.udsDetailInformation.Rows.Add
            row.SetCellValue(DetailInformationFormStrings.PropName_ColCaption, DetailInformationFormStrings.AliasId_ColCaption)
            If (.Alias_id.Length <> 0) Then
                If (.Alias_id.Length = 1) AndAlso (.Alias_id(0).Description Is Nothing) AndAlso (.Alias_id(0).Scope Is Nothing) Then
                    row.SetCellValue(DetailInformationFormStrings.PropVal_ColCaption, .Alias_id(0).Alias_id)
                Else
                    row.SetCellValue(DetailInformationFormStrings.PropVal_ColCaption, Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS)
                    row.Tag = .Alias_id
                End If
            End If

            _keyMapper.Add(PartPropertyName.Part_alias_ids.ToString, DetailInformationFormStrings.AliasId_ColCaption)

            row = Me.udsDetailInformation.Rows.Add
            row.SetCellValue(DetailInformationFormStrings.PropName_ColCaption, DetailInformationFormStrings.Version_PropName)
            row.SetCellValue(DetailInformationFormStrings.PropVal_ColCaption, .Version)

            _keyMapper.Add(PartPropertyName.Version.ToString, DetailInformationFormStrings.Version_PropName)

            row = Me.udsDetailInformation.Rows.Add
            row.SetCellValue(DetailInformationFormStrings.PropName_ColCaption, DetailInformationFormStrings.Abbreviation_PropName)
            row.SetCellValue(DetailInformationFormStrings.PropVal_ColCaption, .Abbreviation)

            _keyMapper.Add(PartPropertyName.Abbreviation.ToString, DetailInformationFormStrings.Abbreviation_PropName)

            row = Me.udsDetailInformation.Rows.Add
            row.SetCellValue(DetailInformationFormStrings.PropName_ColCaption, DetailInformationFormStrings.Description_ColCaption)
            row.SetCellValue(DetailInformationFormStrings.PropVal_ColCaption, .Description)

            _keyMapper.Add(PartPropertyName.Part_description.ToString, DetailInformationFormStrings.Description_ColCaption)

            row = Me.udsDetailInformation.Rows.Add
            row.SetCellValue(DetailInformationFormStrings.PropName_ColCaption, DetailInformationFormStrings.PredPartNumber_PropName)
            If (.Predecessor_part_number IsNot Nothing) Then row.SetCellValue(DetailInformationFormStrings.PropVal_ColCaption, .Predecessor_part_number)

            _keyMapper.Add(PartPropertyName.Predecessor_part_number.ToString, DetailInformationFormStrings.PredPartNumber_PropName)

            row = Me.udsDetailInformation.Rows.Add
            row.SetCellValue(DetailInformationFormStrings.PropName_ColCaption, DetailInformationFormStrings.DegreeOfMaturity_PropName)
            If (.Degree_of_maturity IsNot Nothing) Then row.SetCellValue(DetailInformationFormStrings.PropVal_ColCaption, .Degree_of_maturity)

            _keyMapper.Add(PartPropertyName.Degree_of_maturity.ToString, DetailInformationFormStrings.DegreeOfMaturity_PropName)

            row = Me.udsDetailInformation.Rows.Add
            row.SetCellValue(DetailInformationFormStrings.PropName_ColCaption, DetailInformationFormStrings.CopyrightNote_PropName)
            If (.Copyright_note IsNot Nothing) Then row.SetCellValue(DetailInformationFormStrings.PropVal_ColCaption, .Copyright_note)

            _keyMapper.Add(PartPropertyName.Copyright_note.ToString, DetailInformationFormStrings.CopyrightNote_PropName)

            row = Me.udsDetailInformation.Rows.Add
            row.SetCellValue(DetailInformationFormStrings.PropName_ColCaption, DetailInformationFormStrings.MassInfo_PropName)
            If (.Mass_information IsNot Nothing) Then row.SetCellValue(DetailInformationFormStrings.PropVal_ColCaption, String.Format("{0} {1}", Math.Round(.Mass_information.Value_component, 3), _kblMapper.KBLUnitMapper(.Mass_information.Unit_component).Unit_name))

            _keyMapper.Add(PartPropertyName.Mass_information.ToString, DetailInformationFormStrings.MassInfo_PropName)

            row = Me.udsDetailInformation.Rows.Add
            row.SetCellValue(DetailInformationFormStrings.PropName_ColCaption, DetailInformationFormStrings.PartNumberType_PropName)
            If (.Part_number_type IsNot Nothing) Then row.SetCellValue(DetailInformationFormStrings.PropVal_ColCaption, .Part_number_type)

            _keyMapper.Add(PartPropertyName.Part_number_type.ToString, DetailInformationFormStrings.PartNumberType_PropName)


            row = Me.udsDetailInformation.Rows.Add
            row.SetCellValue(DetailInformationFormStrings.PropName_ColCaption, DetailInformationFormStrings.LocalizedDesc_Value)
            If (.Localized_description.Length <> 0) Then
                If (.Localized_description.Length = 1) Then
                    row.SetCellValue(DetailInformationFormStrings.PropVal_ColCaption, .Localized_description(0).Value)
                Else
                    row.SetCellValue(DetailInformationFormStrings.PropVal_ColCaption, Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS)
                    row.Tag = .Localized_description
                End If
            End If
            _keyMapper.Add(PartPropertyName.Part_Localized_description.ToString, DetailInformationFormStrings.LocalizedDesc_Value)



            row = Me.udsDetailInformation.Rows.Add
            row.SetCellValue(DetailInformationFormStrings.PropName_ColCaption, DetailInformationFormStrings.ExtRef_PropName)
            If (.External_references IsNot Nothing) AndAlso (.External_references.SplitSpace.Length <> 0) Then
                row.SetCellValue(DetailInformationFormStrings.PropVal_ColCaption, Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS)
                row.Tag = .External_references
            End If

            _keyMapper.Add(PartPropertyName.External_references.ToString, DetailInformationFormStrings.ExtRef_PropName)

            row = Me.udsDetailInformation.Rows.Add
            row.SetCellValue(DetailInformationFormStrings.PropName_ColCaption, DetailInformationFormStrings.Change_PropName)
            If (.Change.Length <> 0) Then
                row.SetCellValue(DetailInformationFormStrings.PropVal_ColCaption, Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS)
                row.Tag = .Change
            End If

            _keyMapper.Add(PartPropertyName.Change.ToString, DetailInformationFormStrings.Change_PropName)

            row = Me.udsDetailInformation.Rows.Add
            row.SetCellValue(DetailInformationFormStrings.PropName_ColCaption, DetailInformationFormStrings.MatInfo_PropName)
            If (.Material_information IsNot Nothing) Then
                If (.Material_information.Material_reference_system Is Nothing) Then
                    row.SetCellValue(DetailInformationFormStrings.PropVal_ColCaption, .Material_information.Material_key)
                Else
                    row.SetCellValue(DetailInformationFormStrings.PropVal_ColCaption, Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS)
                    row.Tag = .Material_information
                End If
            End If

            _keyMapper.Add(PartPropertyName.Material_information.ToString, DetailInformationFormStrings.MatInfo_PropName)

            row = Me.udsDetailInformation.Rows.Add
            row.SetCellValue(DetailInformationFormStrings.PropName_ColCaption, DetailInformationFormStrings.ProcInfo_PropName)
            If (.Processing_information.Length <> 0) Then
                row.SetCellValue(DetailInformationFormStrings.PropVal_ColCaption, Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS)
                row.Tag = .Processing_information
            End If

            _keyMapper.Add(PartPropertyName.Part_processing_information.ToString, DetailInformationFormStrings.ProcInfo_PropName)

            If (TypeOf part Is Cavity_plug) Then
                row = Me.udsDetailInformation.Rows.Add
                row.SetCellValue(DetailInformationFormStrings.PropName_ColCaption, DetailInformationFormStrings.Color_PropName)
                If (DirectCast(part, Cavity_plug).Colour IsNot Nothing) Then row.SetCellValue(DetailInformationFormStrings.PropVal_ColCaption, DirectCast(part, Cavity_plug).Colour)

                _keyMapper.Add(PlugSealTerminalPropertyName.Colour.ToString, DetailInformationFormStrings.Color_PropName)

                row = Me.udsDetailInformation.Rows.Add
                row.SetCellValue(DetailInformationFormStrings.PropName_ColCaption, DetailInformationFormStrings.PlugType_PropName)
                If (DirectCast(part, Cavity_plug).Plug_type IsNot Nothing) Then row.SetCellValue(DetailInformationFormStrings.PropVal_ColCaption, DirectCast(part, Cavity_plug).Plug_type)

                _keyMapper.Add(PlugSealTerminalPropertyName.Plug_type.ToString, DetailInformationFormStrings.PlugType_PropName)
            ElseIf (TypeOf part Is Cavity_seal) Then
                row = Me.udsDetailInformation.Rows.Add
                row.SetCellValue(DetailInformationFormStrings.PropName_ColCaption, DetailInformationFormStrings.Color_PropName)
                If (DirectCast(part, Cavity_seal).Colour IsNot Nothing) Then row.SetCellValue(DetailInformationFormStrings.PropVal_ColCaption, DirectCast(part, Cavity_seal).Colour)

                _keyMapper.Add(PlugSealTerminalPropertyName.Colour.ToString, DetailInformationFormStrings.Color_PropName)

                row = Me.udsDetailInformation.Rows.Add
                row.SetCellValue(DetailInformationFormStrings.PropName_ColCaption, DetailInformationFormStrings.SealType_PropName)
                If (DirectCast(part, Cavity_seal).Seal_type IsNot Nothing) Then row.SetCellValue(DetailInformationFormStrings.PropVal_ColCaption, DirectCast(part, Cavity_seal).Seal_type)

                _keyMapper.Add(PlugSealTerminalPropertyName.Seal_type.ToString, DetailInformationFormStrings.SealType_PropName)

                row = Me.udsDetailInformation.Rows.Add
                row.SetCellValue(DetailInformationFormStrings.PropName_ColCaption, DetailInformationFormStrings.WireSize_PropName)
                If (DirectCast(part, Cavity_seal).Wire_size IsNot Nothing) Then
                    Dim minimum As Double = DirectCast(part, Cavity_seal).Wire_size.Minimum
                    Dim maximum As Double = DirectCast(part, Cavity_seal).Wire_size.Maximum

                    row.SetCellValue(DetailInformationFormStrings.PropVal_ColCaption, String.Format(DetailInformationFormStrings.MinMax_PropVal, minimum, _kblMapper.KBLUnitMapper(DirectCast(part, Cavity_seal).Wire_size.Unit_component).Unit_name, maximum))
                End If

                _keyMapper.Add(PlugSealTerminalPropertyName.Wire_size.ToString, DetailInformationFormStrings.WireSize_PropName)
            ElseIf (TypeOf part Is General_terminal) Then
                row = Me.udsDetailInformation.Rows.Add
                row.SetCellValue(DetailInformationFormStrings.PropName_ColCaption, DetailInformationFormStrings.TerminalType_PropName)
                If (DirectCast(part, General_terminal).Terminal_type IsNot Nothing) Then row.SetCellValue(DetailInformationFormStrings.PropVal_ColCaption, DirectCast(part, General_terminal).Terminal_type)

                _keyMapper.Add(PlugSealTerminalPropertyName.Terminal_type.ToString, DetailInformationFormStrings.TerminalType_PropName)

                row = Me.udsDetailInformation.Rows.Add
                row.SetCellValue(DetailInformationFormStrings.PropName_ColCaption, DetailInformationFormStrings.PlatMat_PropName)
                If (DirectCast(part, General_terminal).Plating_material IsNot Nothing) Then row.SetCellValue(DetailInformationFormStrings.PropVal_ColCaption, DirectCast(part, General_terminal).Plating_material)

                _keyMapper.Add(PlugSealTerminalPropertyName.Plating_material.ToString, DetailInformationFormStrings.PlatMat_PropName)

                row = Me.udsDetailInformation.Rows.Add
                row.SetCellValue(DetailInformationFormStrings.PropName_ColCaption, DetailInformationFormStrings.CSA_PropName)
                If (DirectCast(part, General_terminal).Cross_section_area IsNot Nothing) Then
                    Dim minimum As Double = DirectCast(part, General_terminal).Cross_section_area.Minimum
                    Dim maximum As Double = DirectCast(part, General_terminal).Cross_section_area.Maximum

                    row.SetCellValue(DetailInformationFormStrings.PropVal_ColCaption, String.Format(DetailInformationFormStrings.MinMax_PropVal, Math.Round(minimum, 2), _kblMapper.KBLUnitMapper(DirectCast(part, General_terminal).Cross_section_area.Unit_component).Unit_name, Math.Round(maximum, 2)))
                End If

                _keyMapper.Add(PlugSealTerminalPropertyName.Cross_section_area.ToString, DetailInformationFormStrings.CSA_PropName)

                row = Me.udsDetailInformation.Rows.Add
                row.SetCellValue(DetailInformationFormStrings.PropName_ColCaption, DetailInformationFormStrings.OutsideDia_PropName)
                If (DirectCast(part, General_terminal).Outside_diameter IsNot Nothing) Then
                    Dim minimum As Double = DirectCast(part, General_terminal).Outside_diameter.Minimum
                    Dim maximum As Double = DirectCast(part, General_terminal).Outside_diameter.Maximum

                    row.SetCellValue(DetailInformationFormStrings.PropVal_ColCaption, String.Format(DetailInformationFormStrings.MinMax_PropVal, Math.Round(minimum, 2), _kblMapper.KBLUnitMapper(DirectCast(part, General_terminal).Outside_diameter.Unit_component).Unit_name, Math.Round(maximum, 2)))
                End If

                _keyMapper.Add(PlugSealTerminalPropertyName.Outside_diameter.ToString, DetailInformationFormStrings.OutsideDia_PropName)
            End If
        End With

        If (_objectId IsNot Nothing) AndAlso (_objectId <> String.Empty) AndAlso (_kblMapper.GetHarness.GetCavityPlugOccurrence(_objectId) IsNot Nothing) Then
            Dim cavityPlugOccurrence As Cavity_plug_occurrence = _kblMapper.GetHarness.GetCavityPlugOccurrence(_objectId)
            With cavityPlugOccurrence
                row = Me.udsDetailInformation.Rows.Add
                row.SetCellValue(DetailInformationFormStrings.PropName_ColCaption, DetailInformationFormStrings.OccId_ColCaption)
                If (.Id IsNot Nothing) AndAlso (.Id <> String.Empty) Then
                    row.SetCellValue(DetailInformationFormStrings.PropVal_ColCaption, .Id)
                End If

                row = Me.udsDetailInformation.Rows.Add
                row.SetCellValue(DetailInformationFormStrings.PropName_ColCaption, DetailInformationFormStrings.OccInstallationInfos_ColCaption)
                If (.Installation_information.Length <> 0) Then
                    row.SetCellValue(DetailInformationFormStrings.PropVal_ColCaption, Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS)
                    row.Tag = .Installation_information
                End If
            End With
        ElseIf (_objectId IsNot Nothing) AndAlso (_objectId <> String.Empty) AndAlso (_kblMapper.GetHarness.GetCavitySealOccurrence(_objectId) IsNot Nothing) Then
            Dim cavitySealOccurrence As Cavity_seal_occurrence = _kblMapper.GetHarness.GetCavitySealOccurrence(_objectId)
            With cavitySealOccurrence
                row = Me.udsDetailInformation.Rows.Add
                row.SetCellValue(DetailInformationFormStrings.PropName_ColCaption, DetailInformationFormStrings.OccId_ColCaption)
                If (.Id IsNot Nothing) AndAlso (.Id <> String.Empty) Then
                    row.SetCellValue(DetailInformationFormStrings.PropVal_ColCaption, .Id)
                End If

                row = Me.udsDetailInformation.Rows.Add
                row.SetCellValue(DetailInformationFormStrings.PropName_ColCaption, DetailInformationFormStrings.OccInstallationInfos_ColCaption)
                If (.Installation_information.Length <> 0) Then
                    row.SetCellValue(DetailInformationFormStrings.PropVal_ColCaption, Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS)
                    row.Tag = .Installation_information
                End If
            End With
        ElseIf (_objectId IsNot Nothing) AndAlso (_objectId <> String.Empty) AndAlso (_kblMapper.GetHarness.GetSpecialTerminalOccurrence(_objectId) IsNot Nothing) Then
            Dim specialTerminalOccurrence As Special_terminal_occurrence = _kblMapper.GetHarness.GetSpecialTerminalOccurrence(_objectId)
            With specialTerminalOccurrence
                row = Me.udsDetailInformation.Rows.Add
                row.SetCellValue(DetailInformationFormStrings.PropName_ColCaption, DetailInformationFormStrings.OccId_ColCaption)
                row.SetCellValue(DetailInformationFormStrings.PropVal_ColCaption, .Id)

                row = Me.udsDetailInformation.Rows.Add
                row.SetCellValue(DetailInformationFormStrings.PropName_ColCaption, DetailInformationFormStrings.OccAliasIds_ColCaption)
                If (.Alias_id.Length <> 0) Then
                    If (.Alias_id.Length = 1) AndAlso (.Alias_id(0).Description Is Nothing) AndAlso (.Alias_id(0).Scope Is Nothing) Then
                        row.SetCellValue(DetailInformationFormStrings.PropVal_ColCaption, .Alias_id(0).Alias_id)
                    Else
                        row.SetCellValue(DetailInformationFormStrings.PropVal_ColCaption, Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS)
                        row.Tag = .Alias_id
                    End If
                End If

                row = Me.udsDetailInformation.Rows.Add
                row.SetCellValue(DetailInformationFormStrings.PropName_ColCaption, DetailInformationFormStrings.OccDescription_ColCaption)
                row.SetCellValue(DetailInformationFormStrings.PropVal_ColCaption, .Description)

                row = Me.udsDetailInformation.Rows.Add
                row.SetCellValue(DetailInformationFormStrings.PropName_ColCaption, DetailInformationFormStrings.OccInstallationInfos_ColCaption)
                If (.Installation_information.Length <> 0) Then
                    row.SetCellValue(DetailInformationFormStrings.PropVal_ColCaption, Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS)
                    row.Tag = .Installation_information
                End If
            End With
        ElseIf (_objectId IsNot Nothing) AndAlso (_objectId <> String.Empty) AndAlso (_kblMapper.GetHarness.GetTerminalOccurrence(_objectId) IsNot Nothing) Then
            Dim terminalOccurrence As Terminal_occurrence = _kblMapper.GetHarness.GetTerminalOccurrence(_objectId)
            With terminalOccurrence
                row = Me.udsDetailInformation.Rows.Add
                row.SetCellValue(DetailInformationFormStrings.PropName_ColCaption, DetailInformationFormStrings.OccId_ColCaption)
                If (.Id IsNot Nothing) AndAlso (.Id <> String.Empty) Then
                    row.SetCellValue(DetailInformationFormStrings.PropVal_ColCaption, .Id)
                End If

                row = Me.udsDetailInformation.Rows.Add
                row.SetCellValue(DetailInformationFormStrings.PropName_ColCaption, DetailInformationFormStrings.OccInstallationInfos_ColCaption)
                If (.Installation_information.Length <> 0) Then
                    row.SetCellValue(DetailInformationFormStrings.PropVal_ColCaption, Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS)
                    row.Tag = .Installation_information
                End If
            End With
        End If

        Me.udsDetailInformation.Band.Tag = tag
    End Sub

    Private Sub AddChangeRow(change As Change, tag As Object)
        Dim row As UltraDataRow = Me.udsDetailInformation.Rows.Add
        row.Tag = tag

        With change
            If (.Id IsNot Nothing) Then row.SetCellValue(DetailInformationFormStrings.Id_PropName, .Id)
            If (.Description IsNot Nothing) Then row.SetCellValue(DetailInformationFormStrings.Description_ColCaption, .Description)
            If (.Change_request IsNot Nothing) Then row.SetCellValue(DetailInformationFormStrings.ChangeReq_PropName, .Change_request)
            If (.Change_date IsNot Nothing) Then row.SetCellValue(DetailInformationFormStrings.ChangeDat_PropName, .Change_date)

            row.SetCellValue(DetailInformationFormStrings.RespDes_PropName, .Responsible_designer)
            row.SetCellValue(DetailInformationFormStrings.DesDep_PropName, .Designer_department)

            If (.Approver_name IsNot Nothing) Then row.SetCellValue(DetailInformationFormStrings.ApprName_PropName, .Approver_name)
            If (.Approver_department IsNot Nothing) Then row.SetCellValue(DetailInformationFormStrings.ApprDep_PropName, .Approver_department)
        End With
    End Sub

    Private Sub AddComponentPinMapRow(componentPinMap As Component_pin_map, tag As Object)
        Dim row As UltraDataRow = Me.udsDetailInformation.Rows.Add
        row.Tag = tag

        row.SetCellValue(DetailInformationFormStrings.ComponentPinNumber_PropName, componentPinMap.Component_pin_number)
        row.SetCellValue(DetailInformationFormStrings.CavityNumber_PropName, componentPinMap.Cavity_number)

        If (componentPinMap.Connected_contact_points IsNot Nothing) Then row.SetCellValue(DetailInformationFormStrings.ConnectedContactPoints_PropName, componentPinMap.GetConnectedContactPointInformation(_kblMapper))

    End Sub

    Private Sub AddCSARow(crossSectionArea As Cross_section_area, tag As Object)
        Dim row As UltraDataRow = Me.udsDetailInformation.Rows.Add
        row.Tag = tag

        With crossSectionArea
            row.SetCellValue(DetailInformationFormStrings.ValDet_PropName, crossSectionArea.Value_determination)

            If (crossSectionArea.Area IsNot Nothing) Then
                Dim unit As Unit = _kblMapper.GetUnit(crossSectionArea.Area.Unit_component)
                If unit IsNot Nothing Then
                    row.SetCellValue(DetailInformationFormStrings.Area_PropName, String.Format("{0} {1}", Math.Round(.Area.Value_component, 2), unit.Unit_name))
                End If
            End If
        End With
    End Sub

    Private Sub AddExternalReferenceRow(externalReference As External_reference, tag As Object)
        Dim row As UltraDataRow = Me.udsDetailInformation.Rows.Add
        row.Tag = tag

        With externalReference
            row.SetCellValue(DetailInformationFormStrings.DocType_PropName, .Document_type)
            row.SetCellValue(DetailInformationFormStrings.DocNumber_PropName, .Document_number)
            row.SetCellValue(DetailInformationFormStrings.ChangeLvl_PropName, .Change_level)

            If (.File_name IsNot Nothing) Then
                row.SetCellValue(DetailInformationFormStrings.FileName_PropName, .File_name)
            End If

            If (.Location IsNot Nothing) Then
                row.SetCellValue(DetailInformationFormStrings.Directory_PropName, .Location)
            End If

            row.SetCellValue(DetailInformationFormStrings.DataFormat_PropName, .Data_format)

            If (.Creating_system IsNot Nothing) Then
                row.SetCellValue(DetailInformationFormStrings.CreatingSys_PropName, .Creating_system)
            End If
        End With
    End Sub

    Private Sub AddFixingAssignmentRow(fixingAssignment As Fixing_assignment, tag As Object)
        Dim row As UltraDataRow = Me.udsDetailInformation.Rows.Add
        row.Tag = tag

        With fixingAssignment
            Dim id As String = If(fixingAssignment.Id, fixingAssignment.Fixing) 'HINT: Serializer is no longer filling in the id (sometimes?) but there is also no id in the kbl, in old trunk this id filled with the same value as the .Fixing-Property
            If (id IsNot Nothing) Then
                row.SetCellValue(DetailInformationFormStrings.Id_PropName, id)
            End If

            If (.Alias_id.Length <> 0) Then
                If (.Alias_id.Length = 1) AndAlso (.Alias_id(0).Description Is Nothing) AndAlso (.Alias_id(0).Scope Is Nothing) Then
                    row.SetCellValue(DetailInformationFormStrings.AliasId_ColCaption, .Alias_id(0).Alias_id)
                Else
                    If row.Tag Is Nothing Then
                        row.SetCellValue(DetailInformationFormStrings.AliasId_ColCaption, Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS)
                        row.Tag = .Alias_id 'HINT: the current structure cannot handle compare information and sub dialog information
                    Else
                        row.SetCellValue(DetailInformationFormStrings.AliasId_ColCaption, Zuken.E3.HarnessAnalyzer.Shared.NOT_AVAILABLE)
                    End If
                End If
            End If

            If (.Processing_information.Length <> 0) Then
                If row.Tag Is Nothing Then
                    row.SetCellValue(DetailInformationFormStrings.ProcInfo_PropName, Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS)
                    row.Tag = .Processing_information 'HINT: the current structure cannot handle compare information and sub dialog information
                Else
                    row.SetCellValue(DetailInformationFormStrings.ProcInfo_PropName, Zuken.E3.HarnessAnalyzer.Shared.NOT_AVAILABLE)
                End If
            End If

            row.SetCellValue(DetailInformationFormStrings.Location_PropName, String.Format("{0} %", Math.Round(.Location * 100, NOF_DIGITS_LOCATIONS)))

            If .Absolute_location IsNot Nothing Then
                Dim unit As Unit = _kblMapper.GetUnit(fixingAssignment.Absolute_location.Unit_component)
                If unit IsNot Nothing Then
                    row.SetCellValue(DetailInformationFormStrings.AbsoluteLocation_PropName, String.Format("{0} {1}", Math.Round(fixingAssignment.Absolute_location.Value_component, 2), unit.Unit_name))
                End If
            End If

            Dim refElementStr As String = Harness.GetReferenceElement(.Fixing, _kblMapper)
            If Not String.IsNullOrEmpty(refElementStr) Then
                row.SetCellValue(DetailInformationFormStrings.AccFixName_PropName, refElementStr)
                If Not TypeOf (Me) Is DetailInformationCompareForm Then
                    row.SetCellValue(DetailInformationFormStrings.ObjectId, .Fixing)
                End If
            Else
                row.SetCellValue(DetailInformationFormStrings.AccFixName_PropName, .Fixing)
            End If

        End With
    End Sub

    Private Sub AddInstallationInformationRow(installationInformation As Installation_instruction, tag As Object)
        Dim row As UltraDataRow = Me.udsDetailInformation.Rows.Add
        row.Tag = tag

        With installationInformation
            row.SetCellValue(DetailInformationFormStrings.InstructionType_PropName, .Instruction_type)
            row.SetCellValue(DetailInformationFormStrings.InstructionValue_PropName, .Instruction_value)

            If (.ClassificationSpecified) Then
                row.SetCellValue(DetailInformationFormStrings.Classification_PropName, .Classification)
            End If

            If (.ExternalReferences.OrEmpty.SplitSpace.Length > 0) Then
                If row.Tag Is Nothing Then
                    row.SetCellValue(DetailInformationFormStrings.ExtRef_PropName, Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS)
                    row.Tag = .ExternalReferences 'HINT: the current structure cannot handle compare information and sub dialog information
                Else
                    row.SetCellValue(DetailInformationFormStrings.ExtRef_PropName, Zuken.E3.HarnessAnalyzer.Shared.NOT_AVAILABLE)
                End If
            End If
        End With
    End Sub

    Private Sub AddLocalizedDescriptionRow(LocDescription As Localized_string, tag As Object)
        Dim row As UltraDataRow = Me.udsDetailInformation.Rows.Add
        row.Tag = tag
        With LocDescription
            row.SetCellValue(DetailInformationFormStrings.LocalizedDesc_LangCode, .Language_code)
            row.SetCellValue(DetailInformationFormStrings.LocalizedDesc_Value, .Value)
        End With
    End Sub

    Private Sub AddProcessingInformationRow(processingInformation As Processing_instruction, tag As Object)
        Dim row As UltraDataRow = Me.udsDetailInformation.Rows.Add
        row.Tag = tag

        With processingInformation
            row.SetCellValue(DetailInformationFormStrings.InstructionType_PropName, .Instruction_type)
            row.SetCellValue(DetailInformationFormStrings.InstructionValue_PropName, .Instruction_value)

            If (.ClassificationSpecified) Then
                row.SetCellValue(DetailInformationFormStrings.Classification_PropName, .Classification.ToStringOrXmlName)
            End If
            If (.External_reference.OrEmpty.SplitSpace.Length > 0) Then
                If row.Tag Is Nothing Then
                    row.SetCellValue(DetailInformationFormStrings.ExtRef_PropName, Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS)
                    row.Tag = .External_reference 'HINT: the current structure cannot handle compare information and sub dialog information
                Else
                    row.SetCellValue(DetailInformationFormStrings.ExtRef_PropName, Zuken.E3.HarnessAnalyzer.Shared.NOT_AVAILABLE)
                End If
            End If
        End With
    End Sub

    Private Sub AddWireLengthRow(wireLength As Wire_length, tag As Object)
        Dim row As UltraDataRow = Me.udsDetailInformation.Rows.Add
        row.Tag = tag

        With wireLength
            row.SetCellValue(DetailInformationFormStrings.LengthType_PropName, .Length_type)

            If (_kblMapper.KBLUnitMapper.ContainsKey(.Length_value.Unit_component)) Then
                row.SetCellValue(DetailInformationFormStrings.LengthValue_PropName, String.Format("{0} {1}", Math.Round(.Length_value.Value_component, 2), _kblMapper.KBLUnitMapper(.Length_value.Unit_component).Unit_name))
            Else
                row.SetCellValue(DetailInformationFormStrings.LengthValue_PropName, Math.Round(.Length_value.Value_component, 2))
            End If
        End With
    End Sub

    Private Sub AddWiringGroupRows(wiringGroup As Wiring_group, tag As Object)
        Dim row As UltraDataRow

        With wiringGroup
            row = Me.udsDetailInformation.Rows.Add
            row.SetCellValue(DetailInformationFormStrings.PropName_ColCaption, DetailInformationFormStrings.WirGroupId_PropName)
            row.SetCellValue(DetailInformationFormStrings.PropVal_ColCaption, .Id)

            _keyMapper.Add(WiringGroupPropertyName.Id.ToString, DetailInformationFormStrings.WirGroupId_PropName)

            row = Me.udsDetailInformation.Rows.Add
            row.SetCellValue(DetailInformationFormStrings.PropName_ColCaption, DetailInformationFormStrings.Type_PropName)
            If (.Type IsNot Nothing) Then
                row.SetCellValue(DetailInformationFormStrings.PropVal_ColCaption, .Type)
            End If

            _keyMapper.Add(WiringGroupPropertyName.Type.ToString, DetailInformationFormStrings.Type_PropName)

            row = Me.udsDetailInformation.Rows.Add
            row.SetCellValue(DetailInformationFormStrings.PropName_ColCaption, DetailInformationFormStrings.ProcInfo_PropName)

            If (.Processing_information.Length <> 0) Then
                row.SetCellValue(DetailInformationFormStrings.PropVal_ColCaption, Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS)
                row.Tag = .Processing_information
            End If

            _keyMapper.Add(WiringGroupPropertyName.Processing_Information.ToString, DetailInformationFormStrings.ProcInfo_PropName)

            row = Me.udsDetailInformation.Rows.Add
            row.SetCellValue(DetailInformationFormStrings.PropName_ColCaption, DetailInformationFormStrings.AssignedWirCor_PropName)

            _keyMapper.Add(WiringGroupPropertyName.Assigned_wire.ToString, DetailInformationFormStrings.AssignedWirCor_PropName)

            Dim wireCoreNumbers As String = String.Empty

            For Each wireCoreId As String In .Assigned_wire.OrEmpty.SplitSpace
                Dim wireCoreNumber As String = _kblMapper.GetWireOrCoreOccurrence(wireCoreId)?.Wire_number
                If String.IsNullOrEmpty(wireCoreNumbers) Then
                    wireCoreNumbers = wireCoreNumber
                Else
                    wireCoreNumbers = String.Format("{0}, {1}", wireCoreNumbers, wireCoreNumber)
                End If
            Next

            row.SetCellValue(DetailInformationFormStrings.PropVal_ColCaption, wireCoreNumbers)

            row = Me.udsDetailInformation.Rows.Add
            row.SetCellValue(DetailInformationFormStrings.PropName_ColCaption, DetailInformationFormStrings.ConsistencyState_PropName)

            Select Case .GetConsistencyState(_kblMapper)
                Case [Lib].Schema.Kbl.WiringGroupConsistencyState.Valid
                    row.SetCellValue(DetailInformationFormStrings.PropVal_ColCaption, DetailInformationFormStrings.IsValid_ConsistState)
                Case [Lib].Schema.Kbl.WiringGroupConsistencyState.HasSingleCoreOrWireAssigned
                    row.SetCellValue(DetailInformationFormStrings.PropVal_ColCaption, DetailInformationFormStrings.OnlyOneCorWirAss_ConsistState)
                Case [Lib].Schema.Kbl.WiringGroupConsistencyState.HasCoresAndWiresAssigned
                    row.SetCellValue(DetailInformationFormStrings.PropVal_ColCaption, DetailInformationFormStrings.AtLeastOneCorWirAss_ConsistState)
                Case [Lib].Schema.Kbl.WiringGroupConsistencyState.HasCoresOfDifferentCablesAssigned
                    row.SetCellValue(DetailInformationFormStrings.PropVal_ColCaption, DetailInformationFormStrings.CoresOfDiffCabAss_ConsistState)
            End Select

            row = Me.udsDetailInformation.Rows.Add
            row.SetCellValue(DetailInformationFormStrings.PropName_ColCaption, DetailInformationFormStrings.AssignedModules_PropName)
            row.SetCellValue(DetailInformationFormStrings.PropVal_ColCaption, GetAssignedModules(.SystemId))
        End With

        Me.udsDetailInformation.Band.Tag = tag
    End Sub

    Private Sub FillInternal(Of T)(addAction As Action(Of T, Object), Optional TkeyPropertyName As String = "Id", Optional getRelatedFromOccurrence As Func(Of Object, IEnumerable(Of T)) = Nothing)
        FillInternal(Of T)(addAction, Function(item)
                                          Return Reflection.UtilsEx.TryGetPropertyString(item, TkeyPropertyName)
                                      End Function, getRelatedFromOccurrence)
    End Sub

    Private Sub FillInternal(Of T)(addAction As Action(Of T, Object), TkeySelector As Func(Of T, String), Optional getRelatedFromOccurrence As Func(Of Object, IEnumerable(Of T)) = Nothing)
        ArgumentNullException.ThrowIfNull(TkeySelector)

        If getRelatedFromOccurrence Is Nothing Then
            getRelatedFromOccurrence = Function(occurrence)
                                           Dim value As Object = Nothing
                                           Dim list As IEnumerable(Of T) = New List(Of T)
                                           If Reflection.UtilsEx.TryGetPropertyValue(occurrence, DirectCast(_displayData, KeyValuePair(Of String, Object)).Key, value) Then
                                               list = CType(value, IEnumerable(Of T))
                                           End If
                                           Return list
                                       End Function
        End If

        If (TypeOf _displayData Is IEnumerable(Of T)) Then
            For Each fillItem As T In DirectCast(_displayData, IEnumerable(Of T))
                addAction.Invoke(fillItem, Nothing)
            Next
        Else
            Dim compareMapper As ComparisonMapper = DirectCast(DirectCast(_displayData, KeyValuePair(Of String, Object)).Value, ComparisonMapper)
            Dim itemList As IEnumerable(Of T) = Nothing
            Dim occurrence As IKblBaseObject = Nothing
            If _objectId = KblObjectType.Harness.ToLocalizedPluralString Then
                occurrence = _kblMapper.GetHarness
            Else
                occurrence = _kblMapper.GetOccurrenceObjectUntyped(_objectId)
            End If

            If compareMapper.Changes.HasModified Then
                Dim itemsfromOcc As IEnumerable(Of T) = If(occurrence IsNot Nothing, getRelatedFromOccurrence.Invoke(occurrence), Nothing)
                If itemsfromOcc IsNot Nothing Then
                    itemList = New List(Of T)(itemsfromOcc)
                End If
            End If

            Dim itemsBySelector As Dictionary(Of String, T()) = Nothing
            If compareMapper.Changes.HasModified AndAlso itemList IsNot Nothing Then
                itemsBySelector = itemList.Where(Function(item) Not String.IsNullOrEmpty(TkeySelector.Invoke(item))).GroupBy(Function(chg) TkeySelector.Invoke(chg)).ToDictionary(Function(grp) grp.Key.ToLower, Function(grp) grp.ToArray)
            End If

            For Each cItem As ChangedItem In GetOrderedWithInverse(compareMapper.Changes)
                Select Case cItem.Change
                    Case CompareChangeType.New, CompareChangeType.Deleted
                        addAction.Invoke(CType(cItem.Item, T), cItem.Text)
                    Case CompareChangeType.Modified
                        If (itemsBySelector IsNot Nothing) Then
                            If itemsBySelector.ContainsKey(cItem.Key.ToLower) Then
                                itemsBySelector(cItem.Key.ToLower).ForEach(Sub(change) addAction.Invoke(change, CType(cItem.Item, ChangedProperty).ChangedProperties))
                            End If
                        End If
                End Select
            Next

        End If
    End Sub

    Private Sub FillAliasIds()
        If (TypeOf _displayData Is IEnumerable(Of Alias_identification)) Then
            For Each aliasId As Alias_identification In DirectCast(_displayData, IEnumerable(Of Alias_identification))
                AddAliasIdRow(aliasId, Nothing)
            Next
        Else
            Dim aliasIdMapper As AliasIdComparisonMapper = DirectCast(DirectCast(_displayData, KeyValuePair(Of String, Object)).Value, AliasIdComparisonMapper)
            Dim aliasIds As IEnumerable(Of Alias_identification) = New List(Of Alias_identification)
            If aliasIdMapper.Changes.HasModified Then
                aliasIds = GetObjectAliasIds()
            End If

            For Each change As ChangedItem In GetOrderedWithInverse(aliasIdMapper.Changes)
                Select Case change.Change
                    Case CompareChangeType.Deleted, CompareChangeType.New
                        AddAliasIdRow(CType(change.Item, Alias_identification), change.Text)
                    Case CompareChangeType.Modified
                        For Each aliasId As Alias_identification In aliasIds
                            If (aliasId.Alias_id = change.Key) Then
                                AddAliasIdRow(aliasId, CType(change.Item, ChangedProperty).ChangedProperties)
                            End If
                        Next
                End Select
            Next
        End If
    End Sub

    Private Sub FillCavityOrWiringGroup()
        If (TypeOf _displayData Is Part) Then
            AddCavityRows(DirectCast(_displayData, Part), Nothing)
        ElseIf (TypeOf _displayData Is Wiring_group) Then
            AddWiringGroupRows(DirectCast(_displayData, Wiring_group), Nothing)
        Else
            Dim singleObjectMapper As SingleObjectComparisonMapper = DirectCast(DirectCast(_displayData, KeyValuePair(Of String, Object)).Value, SingleObjectComparisonMapper)
            Dim kbl_occ As IKblOccurrence = _kblMapper.GetOccurrenceObjectUntyped(_objectId)
            If kbl_occ IsNot Nothing Then
                Select Case kbl_occ.ObjectType
                    Case KblObjectType.Cavity_occurrence
                        Dim sealOccurrence As Cavity_seal_occurrence = Nothing
                        Dim specialTerminalOccurrence As Special_terminal_occurrence = Nothing
                        Dim terminalOccurrence As Terminal_occurrence = Nothing

                        For Each contactPoint As Contact_point In _kblMapper.GetContactPointsOfCavity(kbl_occ.SystemId)
                            If (contactPoint.Contacted_cavity.SplitSpace.Contains(kbl_occ.SystemId)) AndAlso (contactPoint.Associated_parts IsNot Nothing) Then
                                For Each associatePart As String In contactPoint.Associated_parts.SplitSpace
                                    Dim ass_part_occ As IKblOccurrence = _kblMapper.GetOccurrenceObjectUntyped(associatePart)
                                    If (TypeOf ass_part_occ Is Cavity_seal_occurrence) Then
                                        sealOccurrence = DirectCast(ass_part_occ, Cavity_seal_occurrence)
                                    ElseIf (TypeOf ass_part_occ Is Special_terminal_occurrence) Then
                                        specialTerminalOccurrence = DirectCast(ass_part_occ, Special_terminal_occurrence)
                                    ElseIf (TypeOf ass_part_occ Is Terminal_occurrence) Then
                                        terminalOccurrence = DirectCast(ass_part_occ, Terminal_occurrence)
                                    End If
                                Next

                                Exit For
                            End If
                        Next

                        If (sealOccurrence IsNot Nothing OrElse specialTerminalOccurrence IsNot Nothing OrElse terminalOccurrence IsNot Nothing) Then
                            _objectId = If(sealOccurrence IsNot Nothing, sealOccurrence.SystemId, If(specialTerminalOccurrence IsNot Nothing, specialTerminalOccurrence.SystemId, terminalOccurrence.SystemId))
                        End If

                        Dim part As Part = Nothing

                        If (Me.Text = DetailInformationFormStrings.PlugPart_Caption) Then
                            part = _kblMapper.GetPart(Of Cavity_plug)(_kblMapper.GetCavityPlugOccurrence(CType(kbl_occ, Cavity_occurrence).Associated_plug).Part)
                        ElseIf (Me.Text = DetailInformationFormStrings.SealPart_Caption) Then
                            part = _kblMapper.GetCavitySeal(sealOccurrence.Part)
                        Else
                            If (specialTerminalOccurrence IsNot Nothing) Then
                                part = _kblMapper.GetGeneralTerminal(specialTerminalOccurrence.Part)
                            Else
                                part = _kblMapper.GetGeneralTerminal(terminalOccurrence.Part)
                            End If
                        End If

                        For Each deletedPart As Part In singleObjectMapper.Changes.DeletedItems
                            AddCavityRows(deletedPart, ChangedItem.GetText(CompareChangeType.Deleted, Me.InverseCompare))
                        Next

                        If (singleObjectMapper.Changes.HasModified) Then
                            AddCavityRows(part, singleObjectMapper.Changes.ModifiedItems(0).ChangedProperties)
                        End If

                        For Each addedPart As Part In singleObjectMapper.Changes.NewItems
                            AddCavityRows(addedPart, ChangedItem.GetText(CompareChangeType.New, Me.InverseCompare))
                        Next
                    Case KblObjectType.Contact_point
                        Dim cavitySeal As Cavity_seal = _kblMapper.GetCavitySeals.Where(Function(seal) seal.Part_number = DirectCast(_displayData, KeyValuePair(Of String, Object)).Key).FirstOrDefault
                        Dim terminal As General_terminal = _kblMapper.GetGeneralTerminals.Where(Function(seal) seal.Part_number = DirectCast(_displayData, KeyValuePair(Of String, Object)).Key).FirstOrDefault

                        For Each deletedPart As Part In singleObjectMapper.Changes.DeletedItems
                            AddCavityRows(deletedPart, ChangedItem.GetText(CompareChangeType.Deleted, Me.InverseCompare))
                        Next

                        If (singleObjectMapper.Changes.HasModified) Then
                            If (cavitySeal IsNot Nothing) Then
                                Me.Text = InformationHubStrings.SealPart_Caption

                                AddCavityRows(cavitySeal, singleObjectMapper.Changes.ModifiedItems(0).ChangedProperties)
                            Else
                                Me.Text = InformationHubStrings.TerminalPart_Caption

                                AddCavityRows(terminal, singleObjectMapper.Changes.ModifiedItems(0).ChangedProperties)
                            End If
                        End If

                        For Each addedPart As Part In singleObjectMapper.Changes.NewItems
                            AddCavityRows(addedPart, ChangedItem.GetText(CompareChangeType.New, Me.InverseCompare))
                        Next
                    Case Else
                        Dim wiringGroup As Wiring_group = DirectCast(kbl_occ, Wire_occurrence).GetWiringGroup(_kblMapper.GetWiringGroups)

                        For Each deletedWiringGroup As Wiring_group In singleObjectMapper.Changes.DeletedItems
                            AddWiringGroupRows(deletedWiringGroup, ChangedItem.GetText(CompareChangeType.Deleted, Me.InverseCompare))
                        Next

                        If (singleObjectMapper.Changes.HasModified) Then
                            AddWiringGroupRows(wiringGroup, singleObjectMapper.Changes.ModifiedItems(0).ChangedProperties)
                        End If

                        For Each addedWiringGroup As Wiring_group In singleObjectMapper.Changes.NewItems
                            AddWiringGroupRows(addedWiringGroup, ChangedItem.GetText(CompareChangeType.New, Me.InverseCompare))
                        Next
                End Select
            End If
        End If
    End Sub

    Private Sub FillChangedElements()
        If (TypeOf _displayData Is IEnumerable(Of Tuple(Of String, String))) Then
            For Each gridData As Tuple(Of String, String) In DirectCast(_displayData, IEnumerable(Of Tuple(Of String, String)))
                Dim row As UltraDataRow = Me.udsDetailInformation.Rows.Add
                row.SetCellValue(DetailInformationFormStrings.ChangedElements_PropName, gridData.Item2)
                If Not TypeOf (Me) Is DetailInformationCompareForm Then
                    row.SetCellValue(DetailInformationFormStrings.ObjectId, gridData.Item1)
                End If
            Next
        Else
            Dim compareChangedElements As IEnumerable(Of String) = DirectCast(_displayData, KeyValuePair(Of String, Object)).Value.ToString.Split(vbCrLf.ToCharArray, StringSplitOptions.RemoveEmptyEntries).ToList
            Dim currentChangedElements As New List(Of String)
            Dim changedElementIds As List(Of String) = _kblMapper.GetOccurrenceObject(Of Change_description)(_objectId)?.Changed_elements.OrEmpty.SplitSpace.ToList

            For Each changedElementId As String In changedElementIds
                currentChangedElements.Add(Change_description.GetChangedElement(changedElementId, _kblMapper))
            Next

            For Each gridData As String In compareChangedElements
                If (Not currentChangedElements.Contains(gridData)) Then
                    Dim row As UltraDataRow = Me.udsDetailInformation.Rows.Add
                    row.SetCellValue(DetailInformationFormStrings.ChangedElements_PropName, gridData)
                    row.Tag = ChangedItem.GetText(CompareChangeType.New, Me.InverseCompare)
                End If
            Next

            For Each gridData As String In currentChangedElements
                If (Not compareChangedElements.Contains(gridData)) Then
                    Dim row As UltraDataRow = Me.udsDetailInformation.Rows.Add
                    row.SetCellValue(DetailInformationFormStrings.ChangedElements_PropName, gridData)
                    row.Tag = ChangedItem.GetText(CompareChangeType.Deleted, Me.InverseCompare)
                End If
            Next
        End If
    End Sub

    Private Sub FillChanges()
        FillInternal(Of Change)(Sub(change, tag) AddChangeRow(change, tag),,
                                                              Function(occ)
                                                                  If (occ IsNot Nothing) AndAlso (TypeOf occ Is Change_description) Then
                                                                      Return DirectCast(occ, Change_description).Related_changes.SplitSpace.Select(Function(id) _kblMapper.KBLChangeMapper(id))
                                                                  Else
                                                                      Dim partId As String = Part.TryGetPartId(occ)
                                                                      If Not String.IsNullOrEmpty(partId) AndAlso _kblMapper.KBLPartMapper.ContainsKey(partId) Then
                                                                          With If(occ Is Nothing, _kblMapper.GetHarness, DirectCast(_kblMapper.KBLPartMapper(partId), Part))
                                                                              Return .Change
                                                                          End With
                                                                      End If
                                                                  End If
                                                                  Return Nothing
                                                              End Function)
    End Sub

    Private Sub FillComponentPinMaps()
        If (TypeOf _displayData Is IEnumerable(Of Component_pin_map)) Then
            For Each componentPinMap As Component_pin_map In DirectCast(_displayData, IEnumerable(Of Component_pin_map))
                AddComponentPinMapRow(componentPinMap, Nothing)
            Next
        Else
            Dim componentPinMapMapper As ComponentPinMapComparisonMapper = DirectCast(DirectCast(_displayData, KeyValuePair(Of String, Object)).Value, ComponentPinMapComparisonMapper)
            Dim componentPinMaps As IEnumerable(Of Component_pin_map) = New List(Of Component_pin_map)
            If (componentPinMapMapper.Changes.HasModified) Then
                componentPinMaps = GetObjectComponentPinMaps()
            End If

            For Each change As ChangedItem In GetOrderedWithInverse(componentPinMapMapper.Changes)
                Select Case change.Change
                    Case CompareChangeType.Deleted, CompareChangeType.New
                        AddComponentPinMapRow(DirectCast(change.Item, Component_pin_map), change.Text)
                    Case CompareChangeType.Modified
                        For Each componentPinMap As Component_pin_map In componentPinMaps
                            If (String.Format("{0}|{1}", componentPinMap.Component_pin_number, componentPinMap.Cavity_number) = change.Key) Then AddComponentPinMapRow(componentPinMap, DirectCast(change.Item, ChangedProperty).ChangedProperties)
                        Next
                End Select
            Next
        End If
    End Sub

    Private Sub FillControlledComponents()
        Dim controlledComponents As IEnumerable(Of String) = Nothing
        Dim compareControlledComponents As IEnumerable(Of String) = Nothing

        If (TypeOf _displayData Is IEnumerable(Of String)) Then
            controlledComponents = DirectCast(_displayData, IEnumerable(Of String))
        Else
            compareControlledComponents = DirectCast(DirectCast(_displayData, KeyValuePair(Of String, Object)).Value, IEnumerable(Of String))
            controlledComponents = _kblMapper.GetModule(_objectId).Module_configuration.Controlled_components.SplitSpace.ToList()
        End If

        Dim currentControlledComponents As IEnumerable(Of String) = Harness.GetInformationOfComponentsControlledByModule(controlledComponents, _kblMapper)

        If (compareControlledComponents IsNot Nothing) Then
            For Each gridData As String In compareControlledComponents
                If (Not currentControlledComponents.Contains(gridData)) Then
                    With Me.udsDetailInformation.Rows.Add
                        .SetCellValue(DetailInformationFormStrings.ObjId_PropName, gridData)
                        .Tag = ChangedItem.GetText(CompareChangeType.New, Me.InverseCompare)
                    End With
                End If
            Next
        End If

        For Each gridData As String In currentControlledComponents
            If (compareControlledComponents IsNot Nothing) Then
                If (Not compareControlledComponents.Contains(gridData)) Then
                    With Me.udsDetailInformation.Rows.Add
                        .SetCellValue(DetailInformationFormStrings.ObjId_PropName, gridData)
                        .Tag = ChangedItem.GetText(CompareChangeType.Deleted, Me.InverseCompare)
                    End With
                End If
            Else
                With Me.udsDetailInformation.Rows.Add
                    .SetCellValue(DetailInformationFormStrings.ObjId_PropName, gridData)
                End With
            End If
        Next
    End Sub

    Private Sub FillCrossSectionAreas()
        FillInternal(Of Cross_section_area)(Sub(csa, tag) AddCSARow(csa, tag), CrossSectionAreaPropertyName.Value_determination.ToString)
    End Sub

    Private Sub FillExternalReferences()
        FillInternal(Of External_reference)(Sub(extRef, tag) AddExternalReferenceRow(extRef, tag), Zuken.E3.HarnessAnalyzer.Shared.SYSTEM_ID,
                               Function(occurrence)
                                   Dim externalRefList As New List(Of External_reference)
                                   Dim part As Part = Nothing
                                   If (occurrence Is Nothing) AndAlso (_objectId = [Lib].Schema.Kbl.Utils.GetLocalizedName(KblObjectType.Harness)) Then
                                       part = _kblMapper.GetHarness
                                   ElseIf (TypeOf occurrence Is [Lib].Schema.Kbl.Module) Then
                                       part = DirectCast(occurrence, Part)
                                   Else
                                       part = DirectCast(_kblMapper.KBLPartMapper(occurrence.GetType.GetProperties.Where(Function(x) x.Name = NameOf(IKblOccurrence.Part))(0).GetValue(occurrence).ToString), Part)
                                   End If

                                   If (part IsNot Nothing) Then
                                       For Each partProp As PropertyInfo In part.GetType.GetProperties.Where(Function(x) x.Name = DirectCast(_displayData, KeyValuePair(Of String, Object)).Key)
                                           For Each externalReference As String In partProp.GetValue(part).ToString.SplitSpace
                                               Dim extRef_occ As External_reference = _kblMapper.GetOccurrenceObject(Of External_reference)(externalReference)
                                               If extRef_occ IsNot Nothing Then
                                                   externalRefList.Add(extRef_occ)
                                               End If
                                           Next
                                       Next
                                   End If

                                   Return externalRefList
                               End Function)
    End Sub

    Private Sub FillFixingAssignments()
        FillInternal(Of Fixing_assignment)(Sub(fixAss, tag) AddFixingAssignmentRow(fixAss, tag), FixingAssignmentPropertyName.Id.ToString) 'FixingAssignmentPropertyName.Fixing.ToString
    End Sub

    Private Sub FillInstallationInformation()
        FillInternal(Of Installation_instruction)(Sub(instInstr, tag) AddInstallationInformationRow(instInstr, tag), ProcessingInstructionPropertyName.Instruction_type.ToString)
    End Sub

    Private Sub FillLocalizedDescriptions()
        FillInternal(Of Localized_string)(Sub(descr, tag) AddLocalizedDescriptionRow(descr, tag),
                                          Function(item)
                                              Return CType(Reflection.UtilsEx.TryGetPropertyString(item, "Language_code"), Language_code).ToString
                                          End Function)
    End Sub

    Private Sub FillMountingObjects()
        If (TypeOf _displayData Is IEnumerable(Of Tuple(Of String, String))) Then
            For Each gridData As Tuple(Of String, String) In DirectCast(_displayData, IEnumerable(Of Tuple(Of String, String)))
                Dim row As UltraDataRow = Me.udsDetailInformation.Rows.Add
                row.SetCellValue(DetailInformationFormStrings.MountObjs_PropName, gridData.Item2)
                If Not TypeOf (Me) Is DetailInformationCompareForm Then
                    row.SetCellValue(DetailInformationFormStrings.ObjectId, gridData.Item1)
                End If
            Next
        Else
            Dim compareMountingObjects As IEnumerable(Of String) = DirectCast(_displayData, KeyValuePair(Of String, Object)).Value.ToString.SplitSpace.ToList
            Dim currentMountingObjects As New List(Of String)

            Dim compOcc As Component_occurrence = _kblMapper.GetOccurrenceObject(Of Component_occurrence)(_objectId)
            If Not String.IsNullOrEmpty(compOcc?.Mounting) Then
                For Each mountingObject As String In compOcc.Mounting.SplitSpace
                    Dim cav_occ As Cavity_occurrence = _kblMapper.GetOccurrenceObject(Of Cavity_occurrence)(mountingObject)
                    If cav_occ IsNot Nothing Then
                        Dim connectorOccurrence As Connector_occurrence = _kblMapper.GetConnectorOfCavity(mountingObject)
                        Dim cavityPart As Cavity = _kblMapper.GetPart(Of Cavity)(cav_occ.Part)

                        currentMountingObjects.Add(String.Format("{0},{1}", connectorOccurrence.Id, cavityPart.Cavity_number))
                    Else
                        Dim conn_occ As Connector_occurrence = _kblMapper.GetOccurrenceObject(Of Connector_occurrence)(mountingObject)
                        If conn_occ IsNot Nothing Then
                            currentMountingObjects.Add(conn_occ.Id)
                        Else
                            Dim slot_occ As Slot_occurrence = _kblMapper.GetOccurrenceObject(Of Slot_occurrence)(mountingObject)
                            If slot_occ IsNot Nothing Then
                                Dim connectorOccurrence As Connector_occurrence = _kblMapper.GetOccurrenceObject(Of Connector_occurrence)(_kblMapper.KBLSlotConnectorMapper(slot_occ.SystemId))
                                For Each cavityOccurrence As Cavity_occurrence In slot_occ.Cavities
                                    currentMountingObjects.Add(String.Format("{0},{1}", connectorOccurrence.Id, _kblMapper.GetPart(Of Cavity)(cavityOccurrence.Part).Cavity_number))
                                    Exit For
                                Next
                            Else
                                currentMountingObjects.Add(mountingObject)
                            End If
                        End If
                    End If
                Next
            End If

            For Each gridData As String In compareMountingObjects
                If (Not currentMountingObjects.Contains(gridData)) Then
                    Dim row As UltraDataRow = Me.udsDetailInformation.Rows.Add
                    row.SetCellValue(DetailInformationFormStrings.MountObjs_PropName, gridData)
                    row.Tag = ChangedItem.GetText(CompareChangeType.New, Me.InverseCompare)
                End If
            Next

            For Each gridData As String In currentMountingObjects
                If (Not compareMountingObjects.Contains(gridData)) Then
                    Dim row As UltraDataRow = Me.udsDetailInformation.Rows.Add
                    row.SetCellValue(DetailInformationFormStrings.MountObjs_PropName, gridData)
                    row.Tag = ChangedItem.GetText(CompareChangeType.Deleted, Me.InverseCompare)
                End If
            Next
        End If
    End Sub

    Private Sub FillProcessingInstructions()
        FillInternal(Of Processing_instruction)(Sub(procssInstr, tag) AddProcessingInformationRow(procssInstr, tag), ProcessingInstructionPropertyName.Instruction_type.ToString,
                                               Function(occurrence)
                                                   Dim processingInfoList As IEnumerable(Of Processing_instruction) = Nothing
                                                   If (occurrence IsNot Nothing) Then
                                                       For Each occurrenceProp As PropertyInfo In occurrence.GetType.GetProperties.Where(Function(x) x.Name = DirectCast(_displayData, KeyValuePair(Of String, Object)).Key)
                                                           processingInfoList = DirectCast(occurrenceProp.GetValue(occurrence), IEnumerable(Of Processing_instruction))
                                                       Next
                                                   End If

                                                   If (occurrence IsNot Nothing) AndAlso (processingInfoList Is Nothing) Then
                                                       If (occurrence.GetType.GetProperties.Where(Function(x) x.Name = NameOf(IKblOccurrence.Part)).Any()) Then
                                                           Dim part As Part = DirectCast(_kblMapper.KBLPartMapper(occurrence.GetType.GetProperties.Where(Function(x) x.Name = NameOf(IKblOccurrence.Part))(0).GetValue(occurrence).ToString), Part)

                                                           processingInfoList = part.Processing_information.ToList
                                                       Else
                                                           processingInfoList = DirectCast(occurrence.GetType.GetProperties.Where(Function(x) x.Name = CommonPropertyName.Processing_Information.ToString)(0).GetValue(occurrence), IEnumerable(Of Processing_instruction))
                                                       End If
                                                   End If
                                                   Return processingInfoList
                                               End Function)
    End Sub

    Private Sub FillReferencedElements()
        'HINT for now only for the referenced elements on accessories!
        If (TypeOf _displayData Is IEnumerable(Of String)) Then
            For Each gridData As String In DirectCast(_displayData, IEnumerable(Of String))
                Dim row As UltraDataRow = Me.udsDetailInformation.Rows.Add
                row.SetCellValue(DetailInformationFormStrings.RefElements_PropName, gridData)
            Next
        Else

            Dim compareReferencedElements As IEnumerable(Of String) = DirectCast(_displayData, KeyValuePair(Of String, Object)).Value.ToString.Split(vbCrLf.ToCharArray, StringSplitOptions.RemoveEmptyEntries).ToList
            Dim currentReferencedElements As New List(Of String)
            If (_kblMapper.KBLOccurrenceMapper.ContainsKey(_objectId)) Then
                If (TypeOf _kblMapper.KBLOccurrenceMapper(_objectId) Is Accessory_occurrence) Then
                    For Each id As String In DirectCast(_kblMapper.KBLOccurrenceMapper(_objectId), Accessory_occurrence).Reference_element.SplitSpace.ToList()
                        currentReferencedElements.Add(Harness.GetReferenceElement(id, _kblMapper))
                    Next
                ElseIf (TypeOf _kblMapper.KBLOccurrenceMapper(_objectId) Is Connector_occurrence) Then
                    For Each id As String In DirectCast(_kblMapper.KBLOccurrenceMapper(_objectId), Connector_occurrence).Reference_element.SplitSpace.ToList()
                        currentReferencedElements.Add(Harness.GetReferenceElement(id, _kblMapper))
                    Next
                End If
            End If

            For Each gridData As String In compareReferencedElements
                If (Not currentReferencedElements.Contains(gridData)) Then
                    Dim row As UltraDataRow = Me.udsDetailInformation.Rows.Add
                    row.SetCellValue(DetailInformationFormStrings.RefElements_PropName, gridData)
                    row.Tag = ChangedItem.GetText(CompareChangeType.New, Me.InverseCompare)
                End If
            Next

            For Each gridData As String In currentReferencedElements
                If (Not compareReferencedElements.Contains(gridData)) Then
                    Dim row As UltraDataRow = Me.udsDetailInformation.Rows.Add
                    row.SetCellValue(DetailInformationFormStrings.RefElements_PropName, gridData)
                    row.Tag = ChangedItem.GetText(CompareChangeType.Deleted, Me.InverseCompare)
                End If
            Next

        End If
    End Sub

    Private Sub FillReferencedCavities()
        If (TypeOf _displayData Is IEnumerable(Of String)) Then
            For Each gridData As String In DirectCast(_displayData, IEnumerable(Of String))
                Dim row As UltraDataRow = Me.udsDetailInformation.Rows.Add
                row.SetCellValue(DetailInformationFormStrings.RefCavities_PropName, gridData)
            Next
        Else

            Dim compareReferencedCavities As IEnumerable(Of String) = DirectCast(_displayData, KeyValuePair(Of String, Object)).Value.ToString.Split(vbCrLf.ToCharArray, StringSplitOptions.RemoveEmptyEntries).ToList
            Dim currentReferencedCavities As New List(Of String)
            For Each id As String In _kblMapper.GetNode(_objectId)?.Referenced_cavities.OrEmpty.SplitSpace.ToList()
                currentReferencedCavities.Add(Harness.GetReferenceElement(id, _kblMapper))
            Next

            For Each gridData As String In compareReferencedCavities
                If (Not currentReferencedCavities.Contains(gridData)) Then
                    Dim row As UltraDataRow = Me.udsDetailInformation.Rows.Add
                    row.SetCellValue(DetailInformationFormStrings.RefCavities_PropName, gridData)
                    row.Tag = ChangedItem.GetText(CompareChangeType.New, Me.InverseCompare)
                End If
            Next

            For Each gridData As String In currentReferencedCavities
                If (Not compareReferencedCavities.Contains(gridData)) Then
                    Dim row As UltraDataRow = Me.udsDetailInformation.Rows.Add
                    row.SetCellValue(DetailInformationFormStrings.RefCavities_PropName, gridData)
                    row.Tag = ChangedItem.GetText(CompareChangeType.Deleted, Me.InverseCompare)
                End If
            Next
        End If
    End Sub

    Private Sub FillReferencedComponents()
        If TypeOf _displayData Is IEnumerable(Of String) Then
            For Each gridData As String In DirectCast(_displayData, IEnumerable(Of String))
                Dim row As UltraDataRow = Me.udsDetailInformation.Rows.Add
                row.SetCellValue(DetailInformationFormStrings.RefComps_PropName, gridData)
            Next
        Else
            Dim compareReferencedComponents As IEnumerable(Of String) = DirectCast(_displayData, KeyValuePair(Of String, Object)).Value.ToString.Split(vbCrLf.ToCharArray, StringSplitOptions.RemoveEmptyEntries).ToList
            Dim currentReferencedComponents As New List(Of String)

            'If (_kblMapper.KBLOccurrenceMapper.ContainsKey(_objectId)) Then
            '    Dim referencedComponents As String() = Nothing

            '    If (TypeOf _kblMapper.KBLOccurrenceMapper(_objectId) Is Node) Then referencedComponents = DirectCast(_kblMapper.KBLOccurrenceMapper(_objectId), Node).Referenced_components.SplitS

            '    If (referencedComponents IsNot Nothing) Then
            '        For Each referencedComponent As String In referencedComponents
            '            currentReferencedComponents.Add(Harness.GetReferenceElement(referencedComponent, _kblMapper))
            '        Next
            '    End If
            'End If

            Dim node_occ As Node = _kblMapper.GetOccurrenceObject(Of Node)(_objectId)
            If node_occ IsNot Nothing Then
                If node_occ.Referenced_cavities IsNot Nothing Then
                    For Each referencedComponent As String In node_occ.Referenced_cavities
                        currentReferencedComponents.Add(Harness.GetReferenceElement(referencedComponent, _kblMapper))
                    Next
                End If
            End If

            For Each gridData As String In compareReferencedComponents
                If (Not currentReferencedComponents.Contains(gridData)) Then
                    Dim row As UltraDataRow = Me.udsDetailInformation.Rows.Add
                    row.SetCellValue(DetailInformationFormStrings.RefComps_PropName, gridData)
                    row.Tag = ChangedItem.GetText(CompareChangeType.New, Me.InverseCompare)
                End If
            Next

            For Each gridData As String In currentReferencedComponents
                If (Not compareReferencedComponents.Contains(gridData)) Then
                    Dim row As UltraDataRow = Me.udsDetailInformation.Rows.Add
                    row.SetCellValue(DetailInformationFormStrings.RefComps_PropName, gridData)
                    row.Tag = ChangedItem.GetText(CompareChangeType.Deleted, Me.InverseCompare)
                End If
            Next
        End If
    End Sub

    Private Sub FillRouting()
        If (TypeOf _displayData Is Dictionary(Of String, Segment)) Then
            For Each kvp As KeyValuePair(Of String, Segment) In DirectCast(_displayData, Dictionary(Of String, Segment))
                Dim row As UltraDataRow = Me.udsDetailInformation.Rows.Add
                row.SetCellValue(KblObjectType.Segment.ToLocalizedPluralString, kvp.Value.Id)
                If Not TypeOf (Me) Is DetailInformationCompareForm Then
                    row.SetCellValue(DetailInformationFormStrings.ObjectId, kvp.Key)
                End If
            Next
        Else
            Dim compareRouting As CommonComparisonMapper = DirectCast(DirectCast(_displayData, KeyValuePair(Of String, Object)).Value, CommonComparisonMapper)
            Dim currentRouting As New Dictionary(Of String, String)

            Dim occ As IKblOccurrence = _kblMapper.GetOccurrenceObjectUntyped(_objectId)
            If occ IsNot Nothing Then
                If TypeOf occ Is Special_wire_occurrence Then
                    For Each coreOccurrence As Core_occurrence In DirectCast(occ, Special_wire_occurrence).Core_occurrence
                        For Each segment As Segment In _kblMapper.KBLWireSegmentMapper.GetSegmentsOrEmpty(coreOccurrence.SystemId)
                            currentRouting.TryAdd(segment.SystemId, segment.Id)
                        Next
                    Next
                Else
                    For Each segmentId As String In _kblMapper.KBLWireSegmentMapper.GetSegmentIdsOrEmpty(_objectId, _objectId)
                        Dim segOcc As Segment = _kblMapper.GetOccurrenceObject(Of Segment)(segmentId)
                        'currentRouting.TryAdd(segmentId, DirectCast(_kblMapper.KBLOccurrenceMapper(segmentId), Segment).Id)
                        'currentRouting.TryAdd(DirectCast(_kblMapper.KBLOccurrenceMapper(segmentId), Segment).Id, DirectCast(_kblMapper.KBLOccurrenceMapper(segmentId), Segment).Id)
                        currentRouting.TryAdd(segOcc.Id, segOcc.Id)
                    Next
                End If
            End If

            For Each cItem As ChangedItem In GetOrderedWithInverse(compareRouting.Changes)
                Dim value As Object = Nothing

                Select Case cItem.Change
                    Case CompareChangeType.New
                        If (Not currentRouting.ContainsKey(cItem.Key)) = Not InverseCompare Then
                            value = cItem.Item
                        End If
                    Case CompareChangeType.Deleted, CompareChangeType.Modified
                        If currentRouting.ContainsKey(cItem.Key) = Not InverseCompare Then
                            Select Case cItem.Change
                                Case CompareChangeType.Modified
                                    value = currentRouting(cItem.Key)
                                Case CompareChangeType.Deleted
                                    value = cItem.Item
                            End Select
                        End If
                End Select

                If value IsNot Nothing Then
                    With Me.udsDetailInformation.Rows.Add()
                        .Tag = cItem.Text
                        .SetCellValue(KblObjectType.Segment.ToLocalizedPluralString, value)
                    End With
                End If
            Next
        End If
    End Sub

    Private Function GetObjectAliasIds() As IEnumerable(Of Alias_identification)
        Dim occurrence As Object = Nothing
        Dim aliasIds As IEnumerable(Of Alias_identification) = New List(Of Alias_identification)
        occurrence = _kblMapper.GetOccurrenceObjectUntyped(_objectId)
        If occurrence Is Nothing AndAlso _objectId = [Lib].Schema.Kbl.Utils.GetLocalizedName(KblObjectType.Harness) Then
            occurrence = _kblMapper.GetHarness
        End If

        If (occurrence IsNot Nothing) AndAlso (DirectCast(_displayData, KeyValuePair(Of String, Object)).Key <> PartPropertyName.Part_alias_ids.ToString) Then
            Dim aliasIdsValue As Object = Nothing
            If Reflection.UtilsEx.TryGetPropertyValue(occurrence, DirectCast(_displayData, KeyValuePair(Of String, Object)).Key, aliasIdsValue) Then
                aliasIds = CType(aliasIdsValue, IEnumerable(Of Alias_identification))
            End If
        Else
            Dim partId As String = Part.TryGetPartId(occurrence)
            If Not String.IsNullOrEmpty(partId) Then
                With CType(_kblMapper.KBLPartMapper(partId), Part)
                    aliasIds = .Alias_id.ToList
                End With
            Else
                aliasIds = DirectCast(Reflection.UtilsEx.GetPropertyValue(occurrence, NameOf(Alias_identification.Alias_id)), IEnumerable(Of Alias_identification))
            End If
        End If

        Return aliasIds
    End Function

    Private Function GetObjectComponentPinMaps() As IEnumerable(Of Component_pin_map)
        Dim occurrence As IKblOccurrence = _kblMapper.GetOccurrenceObjectUntyped(_objectId)
        Dim componentPinMaps As IEnumerable(Of Component_pin_map) = New List(Of Component_pin_map)

        If (occurrence IsNot Nothing) Then
            Dim componentPinMapsValue As Object = Nothing

            If Reflection.UtilsEx.TryGetPropertyValue(occurrence, DirectCast(_displayData, KeyValuePair(Of String, Object)).Key, componentPinMapsValue) Then
                componentPinMaps = DirectCast(componentPinMapsValue, IEnumerable(Of Component_pin_map))
            End If
        End If

        Return componentPinMaps
    End Function

    Private Function GetOrderedWithInverse(changes As CompareChangesCollection) As IEnumerable(Of ChangedItem)
        Dim list As New List(Of ChangedItem)

        For Each cItem As ChangedItem In changes.ByChange(If(Me.InverseCompare, CompareChangeType.[New], CompareChangeType.Deleted))
            list.Add(cItem)
        Next

        For Each cItem As ChangedItem In changes.ByChange(CompareChangeType.Modified)
            list.Add(cItem)
        Next

        For Each cItem As ChangedItem In changes.ByChange(If(Me.InverseCompare, CompareChangeType.Deleted, CompareChangeType.[New]))
            list.Add(cItem)
        Next

        Return list
    End Function

    Private Sub FillWireLengthes()
        FillInternal(Of Wire_length)(Sub(wireLen, tag) AddWireLengthRow(wireLen, tag), WireLengthPropertyName.Length_type.ToString)
    End Sub

    Private Function GetAssignedModules(kblId As String) As String
        Dim assignedModules As String = String.Empty
        Dim modules As New List(Of String)

        For Each [module] As E3.Lib.Schema.Kbl.Module In _kblMapper.GetModulesOfObject(kblId)
            If (Not modules.Contains([module].SystemId)) Then
                assignedModules &= String.Format("{0} [{1}]{2}", [module].Abbreviation, [module].Part_number, vbCrLf)
                modules.Add([module].SystemId)
            End If
        Next

        Return assignedModules
    End Function

    Private Sub Initialize(caption As String)
        Me.BackColor = Color.White
        Me.Text = caption
        Me.ugDetailInformation.SyncWithCurrencyManager = False

        Dim kblObjectType As [Lib].Schema.Kbl.KblObjectType = [Lib].Schema.Kbl.KblObjectType.Undefined

        If TypeOf _displayData Is IEnumerable Then

            If TypeOf _displayData Is IDictionary AndAlso CType(_displayData, IDictionary).Count > 0 Then
                If CType(_displayData, IDictionary).Keys.Cast(Of Object).First.GetType.IsKblObject Then
                    kblObjectType = CType(_displayData, IDictionary).Keys.Cast(Of Object).First.GetType.GetKblObjectType
                ElseIf CType(_displayData, IDictionary).Values.Cast(Of Object).First.GetType.IsKblObject Then
                    kblObjectType = CType(_displayData, IDictionary).Values.Cast(Of Object).First.GetType.GetKblObjectType
                End If
            Else
                Dim listType As Type = ListBindingHelper.GetListItemType(_displayData)
                If listType.IsKblObject Then
                    kblObjectType = listType.GetKblObjectType
                End If
            End If
        ElseIf _displayData.GetType.IsKblObject Then
            kblObjectType = _displayData.GetType.GetKblObjectType
        End If

        'TODO: this equalizing of kblObjectType = XY is maybe not needed, but for safty it was ported from the original logic that was an TypeOf compare
        If (TypeOf _displayData Is KeyValuePair(Of String, Object) AndAlso TypeOf DirectCast(_displayData, KeyValuePair(Of String, Object)).Value Is AliasIdComparisonMapper) Then
            kblObjectType = KblObjectType.Alias_indentification
        ElseIf (TypeOf _displayData Is KeyValuePair(Of String, Object) AndAlso TypeOf DirectCast(_displayData, KeyValuePair(Of String, Object)).Value Is LocalizedDescriptionComparisonMapper) Then
            kblObjectType = KblObjectType.Localized_string
        ElseIf (TypeOf _displayData Is Cavity_seal) OrElse (TypeOf _displayData Is General_terminal) OrElse (TypeOf _displayData Is Wiring_group) OrElse (TypeOf _displayData Is KeyValuePair(Of String, Object) AndAlso (TypeOf DirectCast(_displayData, KeyValuePair(Of String, Object)).Value Is SingleObjectComparisonMapper)) Then
            kblObjectType = KblObjectType.Cavity_plug
        ElseIf (TypeOf _displayData Is KeyValuePair(Of String, Object) AndAlso TypeOf DirectCast(_displayData, KeyValuePair(Of String, Object)).Value Is CommonComparisonMapper AndAlso (caption = KblObjectType.Change.ToLocalizedString OrElse caption = ChangeDescriptionPropertyName.Related_changes.ToString)) Then
            kblObjectType = KblObjectType.Change
        ElseIf (TypeOf _displayData Is KeyValuePair(Of String, Object) AndAlso TypeOf DirectCast(_displayData, KeyValuePair(Of String, Object)).Value Is ComponentPinMapComparisonMapper) Then
            kblObjectType = KblObjectType.Component_pin_map
        ElseIf (TypeOf _displayData Is KeyValuePair(Of String, Object) AndAlso TypeOf DirectCast(_displayData, KeyValuePair(Of String, Object)).Value Is CrossSectionAreaComparisonMapper) Then
            kblObjectType = KblObjectType.Cross_section_area
        ElseIf (TypeOf _displayData Is KeyValuePair(Of String, Object) AndAlso TypeOf DirectCast(_displayData, KeyValuePair(Of String, Object)).Value Is CommonComparisonMapper AndAlso caption = DetailInformationFormStrings.ExtRef_PropName) Then
            kblObjectType = KblObjectType.External_reference
        ElseIf (TypeOf _displayData Is KeyValuePair(Of String, Object) AndAlso TypeOf DirectCast(_displayData, KeyValuePair(Of String, Object)).Value Is FixingAssignmentComparisonMapper) Then
            kblObjectType = KblObjectType.Fixing_assignment
        ElseIf (TypeOf _displayData Is KeyValuePair(Of String, Object) AndAlso TypeOf DirectCast(_displayData, KeyValuePair(Of String, Object)).Value Is InstallationInfoComparisonMapper) Then
            kblObjectType = KblObjectType.Installation_instruction
        ElseIf (TypeOf _displayData Is KeyValuePair(Of String, Object) AndAlso TypeOf DirectCast(_displayData, KeyValuePair(Of String, Object)).Value Is ProcessingInfoComparisonMapper) Then
            kblObjectType = KblObjectType.ProcessingInstruction
        ElseIf (TypeOf _displayData Is KeyValuePair(Of String, Object) AndAlso TypeOf DirectCast(_displayData, KeyValuePair(Of String, Object)).Value Is WireLengthComparisonMapper) Then
            kblObjectType = KblObjectType.Wire_length
        ElseIf (TypeOf _displayData Is KeyValuePair(Of String, Object) AndAlso TypeOf DirectCast(_displayData, KeyValuePair(Of String, Object)).Value Is CommonComparisonMapper AndAlso caption = DetailInformationFormStrings.RoutingInfo_Caption) Then
            kblObjectType = KblObjectType.Segment
        End If

        With Me.udsDetailInformation
            With .Band
                .Columns.Add(DetailInformationFormStrings.ObjectId)
                Select Case kblObjectType
                    Case KblObjectType.Alias_indentification
                        .Columns.Add(DetailInformationFormStrings.AliasId_ColCaption)
                        .Columns.Add(DetailInformationFormStrings.Scope_ColCaption)
                        .Columns.Add(DetailInformationFormStrings.Description_ColCaption)

                        _keyMapper.Add(AliasIdPropertyName.Alias_id.ToString, DetailInformationFormStrings.AliasId_ColCaption)
                        _keyMapper.Add(AliasIdPropertyName.Scope.ToString, DetailInformationFormStrings.Scope_ColCaption)
                        _keyMapper.Add(AliasIdPropertyName.Description.ToString, DetailInformationFormStrings.Description_ColCaption)
                    Case KblObjectType.Localized_string
                        .Columns.Add(DetailInformationFormStrings.LocalizedDesc_LangCode)
                        .Columns.Add(DetailInformationFormStrings.LocalizedDesc_Value)

                        _keyMapper.Add(LocalizedDescriptionPropertyName.Language_code.ToString, DetailInformationFormStrings.LocalizedDesc_LangCode)
                        _keyMapper.Add(LocalizedDescriptionPropertyName.Value.ToString, DetailInformationFormStrings.LocalizedDesc_Value)
                    Case KblObjectType.Cavity_plug
                        .Columns.Add(DetailInformationFormStrings.PropName_ColCaption)
                        .Columns.Add(DetailInformationFormStrings.PropVal_ColCaption)
                    Case KblObjectType.Change
                        .Columns.Add(DetailInformationFormStrings.Id_PropName)
                        .Columns.Add(DetailInformationFormStrings.Description_ColCaption)
                        .Columns.Add(DetailInformationFormStrings.ChangeReq_PropName)
                        .Columns.Add(DetailInformationFormStrings.ChangeDat_PropName)
                        .Columns.Add(DetailInformationFormStrings.RespDes_PropName)
                        .Columns.Add(DetailInformationFormStrings.DesDep_PropName)
                        .Columns.Add(DetailInformationFormStrings.ApprName_PropName)
                        .Columns.Add(DetailInformationFormStrings.ApprDep_PropName)

                        _keyMapper.Add(ChangePropertyName.Id.ToString, DetailInformationFormStrings.Id_PropName)
                        _keyMapper.Add(ChangePropertyName.Description.ToString, DetailInformationFormStrings.Description_ColCaption)
                        _keyMapper.Add(ChangePropertyName.Change_request.ToString, DetailInformationFormStrings.ChangeReq_PropName)
                        _keyMapper.Add(ChangePropertyName.Change_date.ToString, DetailInformationFormStrings.ChangeDat_PropName)
                        _keyMapper.Add(ChangePropertyName.Responsible_designer.ToString, DetailInformationFormStrings.RespDes_PropName)
                        _keyMapper.Add(ChangePropertyName.Designer_department.ToString, DetailInformationFormStrings.DesDep_PropName)
                        _keyMapper.Add(ChangePropertyName.Approver_name.ToString, DetailInformationFormStrings.ApprName_PropName)
                        _keyMapper.Add(ChangePropertyName.Approver_department.ToString, DetailInformationFormStrings.ApprDep_PropName)
                    Case KblObjectType.Component_pin_map
                        .Columns.Add(DetailInformationFormStrings.ComponentPinNumber_PropName)
                        .Columns.Add(DetailInformationFormStrings.CavityNumber_PropName)
                        .Columns.Add(DetailInformationFormStrings.ConnectedContactPoints_PropName)

                        _keyMapper.Add(ComponentPinMapPropertyName.Component_pin_number.ToString, DetailInformationFormStrings.ComponentPinNumber_PropName)
                        _keyMapper.Add(ComponentPinMapPropertyName.Cavity_number.ToString, DetailInformationFormStrings.CavityNumber_PropName)
                        _keyMapper.Add(ComponentPinMapPropertyName.Connected_contact_points.ToString, DetailInformationFormStrings.ConnectedContactPoints_PropName)
                    Case KblObjectType.Cross_section_area
                        .Columns.Add(DetailInformationFormStrings.ValDet_PropName)
                        .Columns.Add(DetailInformationFormStrings.Area_PropName)

                        _keyMapper.Add(CrossSectionAreaPropertyName.Value_determination.ToString, DetailInformationFormStrings.ValDet_PropName)
                        _keyMapper.Add(CrossSectionAreaPropertyName.Area.ToString, DetailInformationFormStrings.Area_PropName)
                    Case KblObjectType.External_reference
                        .Columns.Add(DetailInformationFormStrings.DocType_PropName)
                        .Columns.Add(DetailInformationFormStrings.DocNumber_PropName)
                        .Columns.Add(DetailInformationFormStrings.ChangeLvl_PropName)
                        .Columns.Add(DetailInformationFormStrings.FileName_PropName)
                        .Columns.Add(DetailInformationFormStrings.Directory_PropName)
                        .Columns.Add(DetailInformationFormStrings.DataFormat_PropName)
                        .Columns.Add(DetailInformationFormStrings.CreatingSys_PropName)

                        _keyMapper.Add(ExternalReferencePropertyName.Document_type.ToString, DetailInformationFormStrings.DocType_PropName)
                        _keyMapper.Add(ExternalReferencePropertyName.Document_number.ToString, DetailInformationFormStrings.DocNumber_PropName)
                        _keyMapper.Add(ExternalReferencePropertyName.Change_level.ToString, DetailInformationFormStrings.ChangeLvl_PropName)
                        _keyMapper.Add(ExternalReferencePropertyName.File_name.ToString, DetailInformationFormStrings.FileName_PropName)
                        _keyMapper.Add(ExternalReferencePropertyName.Location.ToString, DetailInformationFormStrings.Directory_PropName)
                        _keyMapper.Add(ExternalReferencePropertyName.Data_format.ToString, DetailInformationFormStrings.DataFormat_PropName)
                        _keyMapper.Add(ExternalReferencePropertyName.Creating_system.ToString, DetailInformationFormStrings.CreatingSys_PropName)
                    Case KblObjectType.Fixing_assignment
                        .Columns.Add(DetailInformationFormStrings.AccFixName_PropName)
                        .Columns.Add(DetailInformationFormStrings.Location_PropName)
                        .Columns.Add(DetailInformationFormStrings.AbsoluteLocation_PropName)
                        .Columns.Add(DetailInformationFormStrings.Id_PropName)
                        .Columns.Add(DetailInformationFormStrings.AliasId_ColCaption)
                        .Columns.Add(DetailInformationFormStrings.ProcInfo_PropName)

                        _keyMapper.Add(FixingAssignmentPropertyName.Fixing.ToString, DetailInformationFormStrings.AccFixName_PropName)
                        _keyMapper.Add(FixingAssignmentPropertyName.Location.ToString, DetailInformationFormStrings.Location_PropName)
                        _keyMapper.Add(FixingAssignmentPropertyName.Absolute_location.ToString, DetailInformationFormStrings.AbsoluteLocation_PropName)
                        _keyMapper.Add(FixingAssignmentPropertyName.Id.ToString, DetailInformationFormStrings.Id_PropName)
                        _keyMapper.Add(FixingAssignmentPropertyName.Alias_Id.ToString, DetailInformationFormStrings.AliasId_ColCaption)
                        _keyMapper.Add(FixingAssignmentPropertyName.Processing_Information.ToString, DetailInformationFormStrings.ProcInfo_PropName)
                    Case KblObjectType.Installation_instruction
                        .Columns.Add(DetailInformationFormStrings.InstructionType_PropName)
                        .Columns.Add(DetailInformationFormStrings.InstructionValue_PropName)
                        .Columns.Add(DetailInformationFormStrings.Classification_PropName)
                        .Columns.Add(DetailInformationFormStrings.ExtRef_PropName)

                        _keyMapper.Add(ProcessingInstructionPropertyName.Instruction_type.ToString, DetailInformationFormStrings.InstructionType_PropName)
                        _keyMapper.Add(ProcessingInstructionPropertyName.Instruction_value.ToString, DetailInformationFormStrings.InstructionValue_PropName)
                        _keyMapper.Add(ProcessingInstructionPropertyName.Classification.ToString, DetailInformationFormStrings.Classification_PropName)
                        _keyMapper.Add(ProcessingInstructionPropertyName.External_references.ToString, DetailInformationFormStrings.ExtRef_PropName)
                    Case KblObjectType.Material
                        .Columns.Add(DetailInformationFormStrings.MatKey_PropName)
                        .Columns.Add(DetailInformationFormStrings.MatRefSys_PropName)

                        _keyMapper.Add(MaterialPropertyName.Material_key.ToString, DetailInformationFormStrings.MatKey_PropName)
                        _keyMapper.Add(MaterialPropertyName.Material_reference_system.ToString, DetailInformationFormStrings.MatRefSys_PropName)

                        'TODO...
                    Case KblObjectType.Module
                        .Columns.Add(DetailInformationFormStrings.ModId_PropName)
                        .Columns.Add(DetailInformationFormStrings.PartNumber_PropName)
                        .Columns.Add(DetailInformationFormStrings.Abbreviation_PropName)
                        .Columns.Add(DetailInformationFormStrings.Description_ColCaption)
                        .Columns.Add(KblObjectType.Module_family.ToLocalizedString)
                        .Columns.Add(DetailInformationFormStrings.Type_PropName)

                        _keyMapper.Add(Zuken.E3.HarnessAnalyzer.Shared.SYSTEM_ID, DetailInformationFormStrings.ModId_PropName)
                        _keyMapper.Add(PartPropertyName.Part_number.ToString, DetailInformationFormStrings.PartNumber_PropName)
                        _keyMapper.Add(PartPropertyName.Abbreviation.ToString, DetailInformationFormStrings.Abbreviation_PropName)
                        _keyMapper.Add(PartPropertyName.Part_description.ToString, DetailInformationFormStrings.Description_ColCaption)
                        _keyMapper.Add(ModulePropertyName.Of_family, KblObjectType.Module_family.ToLocalizedString)
                    Case KblObjectType.ProcessingInstruction
                        .Columns.Add(DetailInformationFormStrings.InstructionType_PropName)
                        .Columns.Add(DetailInformationFormStrings.InstructionValue_PropName)
                        .Columns.Add(DetailInformationFormStrings.Classification_PropName)
                        .Columns.Add(DetailInformationFormStrings.ExtRef_PropName)

                        _keyMapper.Add(ProcessingInstructionPropertyName.Instruction_type.ToString, DetailInformationFormStrings.InstructionType_PropName)
                        _keyMapper.Add(ProcessingInstructionPropertyName.Instruction_value.ToString, DetailInformationFormStrings.InstructionValue_PropName)
                        _keyMapper.Add(ProcessingInstructionPropertyName.Classification.ToString, DetailInformationFormStrings.Classification_PropName)
                        _keyMapper.Add(ProcessingInstructionPropertyName.External_references.ToString, DetailInformationFormStrings.ExtRef_PropName)
                    Case KblObjectType.Wire_length
                        .Columns.Add(DetailInformationFormStrings.LengthType_PropName)
                        .Columns.Add(DetailInformationFormStrings.LengthValue_PropName)
                        _keyMapper.Add(WireLengthPropertyName.Length_type.ToString, DetailInformationFormStrings.LengthType_PropName)
                        _keyMapper.Add(WireLengthPropertyName.Length_value.ToString, DetailInformationFormStrings.LengthValue_PropName)
                        'TODO ...
                    Case KblObjectType.Segment
                        .Columns.Add(KblObjectType.Segment.ToLocalizedPluralString)
                        'TODO ...
                    Case Else
                        If (TypeOf _displayData Is IEnumerable(Of Tuple(Of String, String)) OrElse TypeOf _displayData Is IEnumerable(Of String)) OrElse (TypeOf _displayData Is KeyValuePair(Of String, Object) AndAlso (TypeOf DirectCast(_displayData, KeyValuePair(Of String, Object)).Value Is IEnumerable(Of String) OrElse TypeOf DirectCast(_displayData, KeyValuePair(Of String, Object)).Value Is String)) Then
                            If (caption = DetailInformationFormStrings.CtrlCompsByMod_Caption) Then
                                .Columns.Add(DetailInformationFormStrings.ObjId_PropName)
                            Else
                                .Columns.Add(caption)
                            End If
                        End If
                End Select

            End With

            Select Case kblObjectType
                Case KblObjectType.Alias_indentification
                    FillAliasIds()
                Case KblObjectType.Cavity_plug
                    FillCavityOrWiringGroup()
                Case KblObjectType.Change
                    FillChanges()
                Case KblObjectType.Component_pin_map
                    FillComponentPinMaps()
                Case KblObjectType.Cross_section_area
                    FillCrossSectionAreas()
                Case KblObjectType.External_reference
                    FillExternalReferences()
                Case KblObjectType.Fixing_assignment
                    FillFixingAssignments()
                Case KblObjectType.Installation_instruction
                    FillInstallationInformation()
                Case KblObjectType.Localized_string
                    FillLocalizedDescriptions()
                Case KblObjectType.Material
                    Dim row As UltraDataRow = .Rows.Add
                    row.SetCellValue(DetailInformationFormStrings.MatKey_PropName, DirectCast(_displayData, Material).Material_key)

                    If (DirectCast(_displayData, Material).Material_reference_system IsNot Nothing) Then
                        row.SetCellValue(DetailInformationFormStrings.MatRefSys_PropName, DirectCast(_displayData, Material).Material_reference_system)
                    End If
                Case KblObjectType.Module
                    For Each [module] As [Lib].Schema.Kbl.[Module] In DirectCast(_displayData, IDictionary).Keys.Cast(Of [Lib].Schema.Kbl.Module)
                        Dim row As UltraDataRow = .Rows.Add
                        row.SetCellValue(DetailInformationFormStrings.ModId_PropName, [module].SystemId)
                        row.SetCellValue(DetailInformationFormStrings.PartNumber_PropName, [module].Part_number)
                        row.SetCellValue(DetailInformationFormStrings.Abbreviation_PropName, [module].Abbreviation)
                        row.SetCellValue(DetailInformationFormStrings.Description_ColCaption, [module].Description)

                        If ([module].Of_family IsNot Nothing) Then
                            Dim module_family_occ As Module_family = _kblMapper.GetOccurrenceObject(Of Module_family)([module].Of_family)
                            If module_family_occ IsNot Nothing Then
                                row.SetCellValue(KblObjectType.Module_family.ToLocalizedString, module_family_occ.Id)
                            End If
                        End If

                        Dim objTypes As List(Of String) = CType(CType(_displayData, IDictionary).Item([module]), IEnumerable(Of String)).ToList
                        objTypes.Sort()
                        row.SetCellValue(DetailInformationFormStrings.Type_PropName, String.Join(", ", objTypes))
                    Next

                    Me.btnApply.Visible = True
                Case KblObjectType.ProcessingInstruction
                    FillProcessingInstructions()
                Case KblObjectType.Wire_length
                    FillWireLengthes()
                Case KblObjectType.Segment
                    FillRouting()
                Case Else
                    If (TypeOf _displayData Is IEnumerable(Of Tuple(Of String, String)) OrElse TypeOf _displayData Is IEnumerable(Of String)) OrElse (TypeOf _displayData Is KeyValuePair(Of String, Object) AndAlso (TypeOf DirectCast(_displayData, KeyValuePair(Of String, Object)).Value Is IEnumerable(Of String) OrElse TypeOf DirectCast(_displayData, KeyValuePair(Of String, Object)).Value Is String)) Then
                        If (caption = DetailInformationFormStrings.ChangedElements_PropName) Then
                            FillChangedElements()
                        ElseIf (caption = DetailInformationFormStrings.CtrlCompsByMod_Caption) Then
                            FillControlledComponents()
                        ElseIf (caption = DetailInformationFormStrings.MountObjs_PropName) Then
                            FillMountingObjects()
                        ElseIf (caption = DetailInformationFormStrings.RefComps_PropName) Then
                            FillReferencedComponents()
                        ElseIf (caption = DetailInformationFormStrings.RefCavities_PropName) Then
                            FillReferencedCavities()
                        ElseIf (caption = DetailInformationFormStrings.RefElements_PropName) Then
                            FillReferencedElements()
                        Else
                            If (TypeOf _displayData Is IEnumerable(Of String)) Then
                                For Each gridData As String In DirectCast(_displayData, IEnumerable(Of String))
                                    Dim row As UltraDataRow = .Rows.Add
                                    row.SetCellValue(caption, gridData)
                                Next
                            Else
                                For Each gridData As String In DirectCast(DirectCast(_displayData, KeyValuePair(Of String, Object)).Value, String).SplitSpace
                                    Dim row As UltraDataRow = .Rows.Add
                                    row.SetCellValue(caption, gridData)
                                Next
                            End If
                        End If
                    End If
            End Select
        End With

        Me.ugDetailInformation.DataSource = Me.udsDetailInformation
    End Sub

    Private Sub InitializeGridLayout(layout As UltraGridLayout)
        With layout
            .AutoFitStyle = AutoFitStyle.ResizeAllColumns
            .CaptionVisible = Infragistics.Win.DefaultableBoolean.False
            .GroupByBox.Hidden = True

            With .Override
                .AllowColMoving = AllowColMoving.NotAllowed
                .AllowDelete = Infragistics.Win.DefaultableBoolean.False
                .AllowUpdate = Infragistics.Win.DefaultableBoolean.False
                .ButtonStyle = Infragistics.Win.UIElementButtonStyle.Button3D
                .CellClickAction = CellClickAction.RowSelect
                .RowSelectors = Infragistics.Win.DefaultableBoolean.False
                .SelectTypeRow = SelectType.Single

                'HINT Allow for multiline text values use in instructions
                .RowSizing = RowSizing.AutoFree
                .CellMultiLine = Infragistics.Win.DefaultableBoolean.True
            End With

            For Each band As UltraGridBand In .Bands
                With band
                    For Each column As UltraGridColumn In .Columns
                        If (Not column.Hidden) Then
                            With column
                                If (.Key = DetailInformationFormStrings.ObjectId) Then
                                    .Hidden = True
                                End If

                                If (.Key = DetailInformationFormStrings.ModId_PropName) Then
                                    .Hidden = True
                                End If

                                If (.Key = DetailInformationFormStrings.Type_PropName) AndAlso (TypeOf _displayData Is Dictionary(Of [Lib].Schema.Kbl.Module, List(Of String))) Then
                                    Dim types As IEnumerable(Of String) = DirectCast(_displayData, Dictionary(Of [Lib].Schema.Kbl.Module, List(Of String))).Values.SelectMany(Function(x) x).Distinct().ToList
                                    If (types.Count > 1) OrElse (_objectType = KblObjectType.Segment.ToLocalizedPluralString) OrElse (_objectType = KblObjectType.Node.ToLocalizedPluralString) Then
                                        band.SortedColumns.Add(DetailInformationFormStrings.Type_PropName, False, True)
                                        layout.ViewStyleBand = ViewStyleBand.OutlookGroupBy
                                    Else
                                        .Hidden = True
                                    End If
                                End If

                                .CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center
                                .CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle
                                .Header.Appearance.TextHAlign = Infragistics.Win.HAlign.Center
                                .Header.Appearance.TextVAlign = Infragistics.Win.VAlign.Middle

                                If (.Index = 0 AndAlso .Key <> DetailInformationFormStrings.PropName_ColCaption) OrElse (.Key = DetailInformationFormStrings.PartNumber_PropName) Then
                                    .SortIndicator = SortIndicator.Ascending
                                End If
                            End With
                        End If
                    Next
                End With
            Next
        End With

        layout.Rows.ExpandAll(False)
    End Sub

    Protected Sub SetGridCellTooltip(row As UltraGridRow, ByVal dataRow As UltraDataRow)
        For Each compareObject As KeyValuePair(Of String, Object) In DirectCast(dataRow.Tag, Dictionary(Of String, Object))
            Dim compareKey As String = compareObject.Key

            If (compareKey = "StringValue") AndAlso (Me.Text = DetailInformationFormStrings.RoutingInfo_Caption) Then
                compareKey = KblObjectType.Segment.ToLocalizedPluralString
            End If
            If (_keyMapper.ContainsKey(compareKey)) Then
                compareKey = _keyMapper(compareKey)
            End If

            If (row.Cells.Exists(compareKey)) Then
                Dim cell As UltraGridCell = row.Cells(compareKey)
                With cell
                    .Appearance.ForeColor = CHANGED_MODIFIED_FORECOLOR.ToColor

                    If (.Value.ToString <> Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS) Then
                        Dim compareValue As Object = compareObject.Value
                        If (compareValue.ToString = String.Empty) Then
                            .ToolTipText = String.Format("{0}{1}{2}{3}{4}", InformationHubStrings.RefDoc_TooltipPart, .Value.ToString, vbLf, vbLf, InformationHubStrings.CompDoc2_TooltipPart)
                        ElseIf (IsNumeric(compareValue) OrElse TypeOf compareValue Is String) Then
                            If (.Value.ToString.EndsWith("%"c)) AndAlso (IsNumeric(Replace(.Value.ToString, "%", String.Empty).Trim)) Then
                                .ToolTipText = String.Format("{0}{1}{2}{3}{4}{5} %", InformationHubStrings.RefDoc_TooltipPart, .Value.ToString, vbLf, vbLf, InformationHubStrings.CompDoc1_TooltipPart, CInt(CDbl(compareValue) * 100).ToString)
                            Else
                                'HINT this Hack tries to retrieve the length unit. The whole comparison of length value should include the units, but a change of the compare value is too dangerous
                                If (compareObject.Key = WireLengthPropertyName.Length_value.ToString AndAlso .Value IsNot Nothing) Then
                                    Dim strArr As String() = .Value.ToString.SplitSpace
                                    If strArr.Length = 2 Then
                                        compareValue = String.Format("{0} {1}", compareValue.ToString, strArr(1))
                                    End If
                                End If
                                .ToolTipText = String.Format("{0}{1}{2}{3}{4}{5}", InformationHubStrings.RefDoc_TooltipPart, .Value.ToString, vbLf, vbLf, InformationHubStrings.CompDoc1_TooltipPart, compareValue.ToString)
                            End If
                        ElseIf (TypeOf compareValue Is AliasIdComparisonMapper) Then
                            .ToolTipText = String.Format("{0}{1}{2}{3}{4}", InformationHubStrings.RefDoc_TooltipPart, .Value.ToString, vbLf, vbLf, InformationHubStrings.CompDoc1_TooltipPart)

                            For Each aliasId As Alias_identification In DirectCast(compareValue, AliasIdComparisonMapper).Changes.NewItems
                                .ToolTipText &= String.Format(InformationHubStrings.Added_TooltipPart, vbLf, String.Format(InformationHubStrings.AliasId_TooltipPart, aliasId.Alias_id, If(aliasId.Scope Is Nothing OrElse aliasId.Scope = String.Empty, "-", aliasId.Scope), If(aliasId.Description Is Nothing OrElse aliasId.Description = String.Empty, "-", aliasId.Description)))
                            Next

                            For Each aliasId As Alias_identification In DirectCast(compareValue, AliasIdComparisonMapper).Changes.DeletedItems
                                .ToolTipText &= String.Format(InformationHubStrings.Deleted_TooltipPart, vbLf, String.Format(InformationHubStrings.AliasId_TooltipPart, aliasId.Alias_id, If(aliasId.Scope Is Nothing OrElse aliasId.Scope = String.Empty, "-", aliasId.Scope), If(aliasId.Description Is Nothing OrElse aliasId.Description = String.Empty, "-", aliasId.Description)))
                            Next
                        ElseIf (TypeOf compareValue Is CommonComparisonMapper) Then
                            .ToolTipText = String.Format("{0}{1}{2}{3}{4}", InformationHubStrings.RefDoc_TooltipPart, .Value.ToString, vbLf, vbLf, InformationHubStrings.CompDoc1_TooltipPart)

                            For Each addedObject As Object In DirectCast(compareValue, CommonComparisonMapper).Changes.NewItems
                                If (TypeOf addedObject Is Change) Then
                                    With DirectCast(addedObject, Change)
                                        cell.ToolTipText &= String.Format(InformationHubStrings.Added_TooltipPart, vbLf, String.Format(InformationHubStrings.Change_TooltipPart, If(.Id IsNot Nothing, .Id, "-"), If(.Description IsNot Nothing, .Description, "-"), If(.Change_request IsNot Nothing, .Change_request, "-"), If(.Change_date IsNot Nothing, .Change_date, "-"), .Responsible_designer, .Designer_department, If(.Approver_name IsNot Nothing, .Approver_name, "-"), If(.Approver_department IsNot Nothing, .Approver_department, "-")))
                                    End With
                                ElseIf (TypeOf addedObject Is External_reference) Then
                                    With DirectCast(addedObject, External_reference)
                                        cell.ToolTipText &= String.Format(InformationHubStrings.Added_TooltipPart, vbLf, String.Format(InformationHubStrings.ExtReference_TooltipPart, If(.Document_type <> String.Empty, .Document_type, "-"), If(.Document_number <> String.Empty, .Document_number, "-"), If(.Change_level <> String.Empty, .Change_level, "-"), If(.File_name IsNot Nothing, .File_name, "-"), If(.Location IsNot Nothing, .Location, "-"), .Data_format, If(.Creating_system IsNot Nothing, .Creating_system, "-")))
                                    End With
                                ElseIf (TypeOf addedObject Is String) Then
                                    .ToolTipText &= String.Format(InformationHubStrings.Added_TooltipPart, vbLf, addedObject.ToString)
                                End If
                            Next

                            For Each deletedObject As Object In DirectCast(compareValue, CommonComparisonMapper).Changes.DeletedItems
                                If (TypeOf deletedObject Is Change) Then
                                    With DirectCast(deletedObject, Change)
                                        cell.ToolTipText &= String.Format(InformationHubStrings.Deleted_TooltipPart, vbLf, String.Format(InformationHubStrings.Change_TooltipPart, If(.Id IsNot Nothing, .Id, "-"), If(.Description IsNot Nothing, .Description, "-"), If(.Change_request IsNot Nothing, .Change_request, "-"), If(.Change_date IsNot Nothing, .Change_date, "-"), .Responsible_designer, .Designer_department, If(.Approver_name IsNot Nothing, .Approver_name, "-"), If(.Approver_department IsNot Nothing, .Approver_department, "-")))
                                    End With
                                ElseIf (TypeOf deletedObject Is External_reference) Then
                                    With DirectCast(deletedObject, External_reference)
                                        cell.ToolTipText &= String.Format(InformationHubStrings.Deleted_TooltipPart, vbLf, String.Format(InformationHubStrings.ExtReference_TooltipPart, If(.Document_type <> String.Empty, .Document_type, "-"), If(.Document_number <> String.Empty, .Document_number, "-"), If(.Change_level <> String.Empty, .Change_level, "-"), If(.File_name IsNot Nothing, .File_name, "-"), If(.Location IsNot Nothing, .Location, "-"), .Data_format, If(.Creating_system IsNot Nothing, .Creating_system, "-")))
                                    End With
                                ElseIf (TypeOf deletedObject Is String) Then
                                    .ToolTipText &= String.Format(InformationHubStrings.Deleted_TooltipPart, vbLf, deletedObject.ToString)
                                End If
                            Next
                        ElseIf (TypeOf compareValue Is CrossSectionAreaComparisonMapper) Then
                            .ToolTipText = String.Format("{0}{1}{2}{3}{4}", InformationHubStrings.RefDoc_TooltipPart, .Value.ToString, vbLf, vbLf, InformationHubStrings.CompDoc1_TooltipPart)

                            For Each csaChangedProperties As ChangedProperty In DirectCast(compareValue, CrossSectionAreaComparisonMapper).Changes.ModifiedItems
                                If (csaChangedProperties.ChangedProperties.ContainsKey("Area")) Then .ToolTipText &= String.Format("{0}{1} {2}", vbLf, DirectCast(csaChangedProperties.ChangedProperties("Area"), Numerical_value).Value_component.ToString("#0.##", CultureInfo.InvariantCulture), _kblMapper.KBLUnitMapper(DirectCast(csaChangedProperties.ChangedProperties("Area"), Numerical_value).Unit_component).Unit_name)
                            Next

                            For Each crossSectionArea As Cross_section_area In DirectCast(compareValue, CrossSectionAreaComparisonMapper).Changes.NewItems
                                .ToolTipText &= String.Format(InformationHubStrings.Added2_TooltipPart, vbLf, crossSectionArea.Area.Value_component.ToString("#0.##", CultureInfo.InvariantCulture), _kblMapper.KBLUnitMapper(crossSectionArea.Area.Unit_component).Unit_name)
                            Next

                            For Each crossSectionArea As Cross_section_area In DirectCast(compareValue, CrossSectionAreaComparisonMapper).Changes.DeletedItems
                                .ToolTipText &= String.Format(InformationHubStrings.Deleted2_TooltipPart, vbLf, crossSectionArea.Area.Value_component.ToString("#0.##", CultureInfo.InvariantCulture), _kblMapper.KBLUnitMapper(crossSectionArea.Area.Unit_component).Unit_name)
                            Next
                        ElseIf (TypeOf compareValue Is FixingAssignmentComparisonMapper) Then
                            .ToolTipText = String.Format("{0}{1}{2}{3}{4}", InformationHubStrings.RefDoc_TooltipPart, .Value.ToString, vbLf, vbLf, InformationHubStrings.CompDoc1_TooltipPart)

                            For Each fixingAssignment As Fixing_assignment In DirectCast(compareValue, FixingAssignmentComparisonMapper).Changes.NewItems
                                If (_kblMapper.KBLOccurrenceMapper.ContainsKey(fixingAssignment.Fixing)) Then
                                    If (TypeOf _kblMapper.KBLOccurrenceMapper(fixingAssignment.Fixing) Is Accessory_occurrence) Then
                                        With DirectCast(_kblMapper.KBLOccurrenceMapper(fixingAssignment.Fixing), Accessory_occurrence)
                                            cell.ToolTipText &= String.Format(InformationHubStrings.FixingAssignmentAdd_TooltipPart, vbLf, .Id, If(.Description IsNot Nothing, .Description, "-"), If(_kblMapper.KBLPartMapper.ContainsKey(If(.Part Is Nothing, String.Empty, .Part)), DirectCast(_kblMapper.KBLPartMapper(.Part), Accessory).Part_number, "-"), String.Format("{0} %", Math.Round(fixingAssignment.Location * 100, NOF_DIGITS_LOCATIONS)))
                                        End With
                                    Else
                                        With DirectCast(_kblMapper.KBLOccurrenceMapper(fixingAssignment.Fixing), Fixing_occurrence)
                                            cell.ToolTipText &= String.Format(InformationHubStrings.FixingAssignmentAdd_TooltipPart, vbLf, .Id, If(.Description IsNot Nothing, .Description, "-"), If(_kblMapper.KBLPartMapper.ContainsKey(If(.Part Is Nothing, String.Empty, .Part)), DirectCast(_kblMapper.KBLPartMapper(.Part), Fixing).Part_number, "-"), String.Format("{0} %", Math.Round(fixingAssignment.Location * 100, NOF_DIGITS_LOCATIONS)))
                                        End With
                                    End If
                                End If
                            Next

                            For Each fixingAssignment As Fixing_assignment In DirectCast(compareValue, FixingAssignmentComparisonMapper).Changes.DeletedItems
                                If (_kblMapper.KBLOccurrenceMapper.ContainsKey(fixingAssignment.Fixing)) Then
                                    If (TypeOf _kblMapper.KBLOccurrenceMapper(fixingAssignment.Fixing) Is Accessory_occurrence) Then
                                        With DirectCast(_kblMapper.KBLOccurrenceMapper(fixingAssignment.Fixing), Accessory_occurrence)
                                            cell.ToolTipText &= String.Format(InformationHubStrings.FixingAssignmentDel_TooltipPart, vbLf, .Id, If(.Description IsNot Nothing, .Description, "-"), If(_kblMapper.KBLPartMapper.ContainsKey(If(.Part Is Nothing, String.Empty, .Part)), DirectCast(_kblMapper.KBLPartMapper(.Part), Accessory).Part_number, "-"), String.Format("{0} %", Math.Round(fixingAssignment.Location * 100, NOF_DIGITS_LOCATIONS)))
                                        End With
                                    Else
                                        With DirectCast(_kblMapper.KBLOccurrenceMapper(fixingAssignment.Fixing), Fixing_occurrence)
                                            cell.ToolTipText &= String.Format(InformationHubStrings.FixingAssignmentDel_TooltipPart, vbLf, .Id, If(.Description IsNot Nothing, .Description, "-"), If(_kblMapper.KBLPartMapper.ContainsKey(If(.Part Is Nothing, String.Empty, .Part)), DirectCast(_kblMapper.KBLPartMapper(.Part), Fixing).Part_number, "-"), String.Format("{0} %", Math.Round(fixingAssignment.Location * 100, NOF_DIGITS_LOCATIONS)))
                                        End With
                                    End If
                                End If
                            Next
                        ElseIf (TypeOf compareValue Is InstallationInfoComparisonMapper) Then
                            .ToolTipText = String.Format("{0}{1}{2}{3}{4}", InformationHubStrings.RefDoc_TooltipPart, .Value.ToString, vbLf, vbLf, InformationHubStrings.CompDoc1_TooltipPart)

                            For Each installationInstruction As Installation_instruction In DirectCast(compareValue, InstallationInfoComparisonMapper).Changes.NewItems
                                .ToolTipText &= String.Format(InformationHubStrings.Added3_TooltipPart, vbLf, installationInstruction.Instruction_type, installationInstruction.Instruction_value)
                            Next

                            For Each installationInstruction As Installation_instruction In DirectCast(compareValue, InstallationInfoComparisonMapper).Changes.DeletedItems
                                .ToolTipText &= String.Format(InformationHubStrings.Deleted3_TooltipPart, vbLf, installationInstruction.Instruction_type, installationInstruction.Instruction_value)
                            Next
                        ElseIf (TypeOf compareValue Is Numerical_value) Then
                            .ToolTipText = String.Format("{0}{1}{2}{3}{4}{5} {6}", InformationHubStrings.RefDoc_TooltipPart, .Value.ToString, vbLf, vbLf, InformationHubStrings.CompDoc1_TooltipPart, DirectCast(compareValue, Numerical_value).Value_component.ToString("#0.##", CultureInfo.InvariantCulture), _kblMapper.KBLUnitMapper(DirectCast(compareValue, Numerical_value).Unit_component).Unit_name)
                        ElseIf (TypeOf compareValue Is Material) Then
                            .ToolTipText = String.Format("{0}{1}{2}{3}{4}{5}", InformationHubStrings.RefDoc_TooltipPart, .Value.ToString, vbLf, vbLf, InformationHubStrings.CompDoc1_TooltipPart, DirectCast(compareValue, Material).Material_key)
                        ElseIf (TypeOf compareValue Is ProcessingInfoComparisonMapper) Then
                            .ToolTipText = String.Format("{0}{1}{2}{3}{4}", InformationHubStrings.RefDoc_TooltipPart, .Value.ToString, vbLf, vbLf, InformationHubStrings.CompDoc1_TooltipPart)

                            For Each processingInstruction As Processing_instruction In DirectCast(compareValue, ProcessingInfoComparisonMapper).Changes.NewItems
                                .ToolTipText &= String.Format(InformationHubStrings.Added3_TooltipPart, vbLf, processingInstruction.Instruction_type, processingInstruction.Instruction_value)
                            Next

                            For Each processingInstruction As Processing_instruction In DirectCast(compareValue, ProcessingInfoComparisonMapper).Changes.DeletedItems
                                .ToolTipText &= String.Format(InformationHubStrings.Deleted3_TooltipPart, vbLf, processingInstruction.Instruction_type, processingInstruction.Instruction_value)
                            Next
                        ElseIf (TypeOf compareValue Is SingleObjectComparisonMapper) Then
                            .ToolTipText = String.Format("{0}{1}{2}{3}{4}", InformationHubStrings.RefDoc_TooltipPart, .Value.ToString, vbLf, vbLf, InformationHubStrings.CompDoc1_TooltipPart)

                            For Each singleObject As Object In DirectCast(compareValue, SingleObjectComparisonMapper).Changes.NewItems
                                If (TypeOf singleObject Is Part) Then
                                    .ToolTipText &= String.Format(InformationHubStrings.Added_TooltipPart, vbLf, DirectCast(singleObject, Part).Part_number)
                                ElseIf (TypeOf singleObject Is Wiring_group) Then
                                    .ToolTipText &= String.Format(InformationHubStrings.Added_TooltipPart, vbLf, DirectCast(singleObject, Wiring_group).Id)
                                End If
                            Next

                            For Each singleObject As Object In DirectCast(compareValue, SingleObjectComparisonMapper).Changes.DeletedItems
                                If (TypeOf singleObject Is Part) Then
                                    .ToolTipText &= String.Format(InformationHubStrings.Deleted_TooltipPart, vbLf, DirectCast(singleObject, Part).Part_number)
                                ElseIf (TypeOf singleObject Is Wiring_group) Then
                                    .ToolTipText &= String.Format(InformationHubStrings.Deleted_TooltipPart, vbLf, DirectCast(singleObject, Wiring_group).Id)
                                End If
                            Next
                        ElseIf (TypeOf compareValue Is WireLengthComparisonMapper) Then
                            For Each wireChangedProperties As ChangedProperty In DirectCast(compareValue, WireLengthComparisonMapper).Changes.ModifiedItems
                                If (wireChangedProperties.ChangedProperties.ContainsKey(WireLengthPropertyName.Length_value.ToString)) Then .ToolTipText = String.Format("{0}{1}{2}{3}{4}{5} {6}", InformationHubStrings.RefDoc_TooltipPart, .Value.ToString, vbLf, vbLf, InformationHubStrings.CompDoc1_TooltipPart, wireChangedProperties.ChangedProperties(WireLengthPropertyName.Length_value.ToString).ToString, "mm")
                            Next

                            For Each wireLength As Wire_length In DirectCast(compareValue, WireLengthComparisonMapper).Changes.NewItems
                                .ToolTipText = String.Format(InformationHubStrings.Added2_TooltipPart, vbLf, Math.Round(wireLength.Length_value.Value_component, 2).ToString, "mm")
                            Next

                            For Each wireLength As Wire_length In DirectCast(compareValue, WireLengthComparisonMapper).Changes.DeletedItems
                                .ToolTipText = String.Format(InformationHubStrings.Deleted2_TooltipPart, vbLf, Math.Round(wireLength.Length_value.Value_component, 2).ToString, "mm")
                            Next

                            If (.ToolTipText.StartsWith(vbLf)) Then
                                .ToolTipText = .ToolTipText.Substring(1, .ToolTipText.Length - 1)
                            End If
                        End If

                        If (.Value.ToString = String.Empty) AndAlso (.ToolTipText <> String.Empty) Then
                            DirectCast(.Row.ListObject, UltraDataRow).SetCellValue(.Column.Key, "-")
                        End If
                    Else
                        .Appearance.FontData.SizeInPoints = 14
                    End If
                End With
            End If
        Next
    End Sub

    Private Sub SetGridRowTooltip(ByVal dataBand As UltraDataBand, ByVal row As UltraGridRow)
        For Each compareObject As KeyValuePair(Of String, Object) In DirectCast(dataBand.Tag, Dictionary(Of String, Object))
            Dim compareKey As String = compareObject.Key

            If (_keyMapper.ContainsKey(compareKey)) Then
                compareKey = _keyMapper(compareKey)
            End If

            If (row.Cells(DetailInformationFormStrings.PropName_ColCaption).Value.ToString = compareKey) Then
                With row.Cells(DetailInformationFormStrings.PropVal_ColCaption)
                    .Appearance.ForeColor = CHANGED_MODIFIED_FORECOLOR.ToColor

                    If (.Value.ToString <> Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS) Then
                        Dim compareValue As Object = compareObject.Value

                        If (compareValue Is Nothing OrElse compareValue.ToString = String.Empty) Then
                            .ToolTipText = String.Format("{0}{1}{2}{3}{4}", InformationHubStrings.RefDoc_TooltipPart, .Value.ToString, vbLf, vbLf, InformationHubStrings.CompDoc1_TooltipPart)
                        ElseIf (IsNumeric(compareValue) OrElse TypeOf compareValue Is String) Then
                            If (.Value.ToString.EndsWith("%"c)) AndAlso (IsNumeric((Replace(.Value.ToString, "%", String.Empty)).Trim)) Then
                                .ToolTipText = String.Format("{0}{1}{2}{3}{4}{5} %", InformationHubStrings.RefDoc_TooltipPart, .Value.ToString, vbLf, vbLf, InformationHubStrings.CompDoc1_TooltipPart, CInt(CDbl(compareValue) * 100).ToString)
                            Else
                                .ToolTipText = String.Format("{0}{1}{2}{3}{4}{5}", InformationHubStrings.RefDoc_TooltipPart, .Value.ToString, vbLf, vbLf, InformationHubStrings.CompDoc1_TooltipPart, compareValue.ToString)
                            End If
                        ElseIf (TypeOf compareValue Is AliasIdComparisonMapper) Then
                            .ToolTipText = String.Format("{0}{1}{2}{3}{4}", InformationHubStrings.RefDoc_TooltipPart, .Value.ToString, vbLf, vbLf, InformationHubStrings.CompDoc1_TooltipPart)

                            For Each aliasId As Alias_identification In DirectCast(compareValue, AliasIdComparisonMapper).Changes.NewItems
                                .ToolTipText &= String.Format(DetailInformationFormStrings.Added_Tooltip, vbLf, aliasId.Alias_id)
                            Next

                            For Each aliasId As Alias_identification In DirectCast(compareValue, AliasIdComparisonMapper).Changes.DeletedItems
                                .ToolTipText &= String.Format(DetailInformationFormStrings.Deleted_Tooltip, vbLf, aliasId.Alias_id)
                            Next
                        ElseIf (TypeOf compareValue Is CrossSectionAreaComparisonMapper) Then
                            .ToolTipText = String.Format("{0}{1}{2}{3}{4}", InformationHubStrings.RefDoc_TooltipPart, .Value.ToString, vbLf, vbLf, InformationHubStrings.CompDoc1_TooltipPart)

                            For Each csaChangedProperties As ChangedProperty In DirectCast(compareValue, CrossSectionAreaComparisonMapper).Changes.ModifiedItems
                                If (csaChangedProperties.ChangedProperties.ContainsKey("Area")) Then .ToolTipText &= String.Format("{0}{1} {2}", vbLf, DirectCast(csaChangedProperties.ChangedProperties("Area"), Numerical_value).Value_component.ToString("#0.##", CultureInfo.InvariantCulture), _kblMapper.KBLUnitMapper(DirectCast(csaChangedProperties.ChangedProperties("Area"), Numerical_value).Unit_component).Unit_name)
                            Next

                            For Each crossSectionArea As Cross_section_area In DirectCast(compareValue, CrossSectionAreaComparisonMapper).Changes.NewItems
                                .ToolTipText &= String.Format(DetailInformationFormStrings.Added2_Tooltip, vbLf, crossSectionArea.Area.Value_component.ToString("#0.##", CultureInfo.InvariantCulture), _kblMapper.KBLUnitMapper(crossSectionArea.Area.Unit_component).Unit_name)
                            Next

                            For Each crossSectionArea As Cross_section_area In DirectCast(compareValue, CrossSectionAreaComparisonMapper).Changes.DeletedItems
                                .ToolTipText &= String.Format(DetailInformationFormStrings.Deleted2_Tooltip, vbLf, crossSectionArea.Area.Value_component.ToString("#0.##", CultureInfo.InvariantCulture), _kblMapper.KBLUnitMapper(crossSectionArea.Area.Unit_component).Unit_name)
                            Next
                        ElseIf (TypeOf compareValue Is FixingAssignmentComparisonMapper) Then
                            .ToolTipText = String.Format("{0}{1}{2}{3}{4}", InformationHubStrings.RefDoc_TooltipPart, .Value.ToString, vbLf, vbLf, InformationHubStrings.CompDoc1_TooltipPart)

                            For Each fixingAssignment As Fixing_assignment In DirectCast(compareValue, FixingAssignmentComparisonMapper).Changes.NewItems
                                .ToolTipText &= String.Format(DetailInformationFormStrings.Added_Tooltip, vbLf, fixingAssignment.Fixing)
                            Next

                            For Each fixingAssignment As Fixing_assignment In DirectCast(compareValue, FixingAssignmentComparisonMapper).Changes.DeletedItems
                                .ToolTipText &= String.Format(DetailInformationFormStrings.Deleted_Tooltip, vbLf, fixingAssignment.Fixing)
                            Next
                        ElseIf (TypeOf compareValue Is Numerical_value) Then
                            .ToolTipText = String.Format("{0}{1}{2}{3}{4}{5} {6}", InformationHubStrings.RefDoc_TooltipPart, .Value.ToString, vbLf, vbLf, InformationHubStrings.CompDoc1_TooltipPart, DirectCast(compareValue, Numerical_value).Value_component.ToString("#0.##", CultureInfo.InvariantCulture), _kblMapper.KBLUnitMapper(DirectCast(compareValue, Numerical_value).Unit_component).Unit_name)
                        ElseIf (TypeOf compareValue Is Material) Then
                            .ToolTipText = String.Format("{0}{1}{2}{3}{4}{5}", InformationHubStrings.RefDoc_TooltipPart, .Value.ToString, vbLf, vbLf, InformationHubStrings.CompDoc1_TooltipPart, DirectCast(compareValue, Material).Material_key)
                        ElseIf (TypeOf compareValue Is ProcessingInfoComparisonMapper) Then
                            .ToolTipText = String.Format("{0}{1}{2}{3}{4}", InformationHubStrings.RefDoc_TooltipPart, .Value.ToString, vbLf, vbLf, InformationHubStrings.CompDoc1_TooltipPart)

                            For Each processingInstruction As Processing_instruction In DirectCast(compareValue, ProcessingInfoComparisonMapper).Changes.NewItems
                                .ToolTipText &= String.Format(DetailInformationFormStrings.Added3_Tooltip, vbLf, processingInstruction.Instruction_type, processingInstruction.Instruction_value)
                            Next

                            For Each processingInstruction As Processing_instruction In DirectCast(compareValue, ProcessingInfoComparisonMapper).Changes.DeletedItems
                                .ToolTipText &= String.Format(DetailInformationFormStrings.Deleted3_Tooltip, vbLf, processingInstruction.Instruction_type, processingInstruction.Instruction_value)
                            Next
                        ElseIf (TypeOf compareValue Is SingleObjectComparisonMapper) Then
                            .ToolTipText = String.Format("{0}{1}{2}{3}{4}", InformationHubStrings.RefDoc_TooltipPart, .Value.ToString, vbLf, vbLf, InformationHubStrings.CompDoc1_TooltipPart)

                            For Each singleObject As Object In DirectCast(compareValue, SingleObjectComparisonMapper).Changes.NewItems
                                If (TypeOf singleObject Is Part) Then
                                    .ToolTipText &= String.Format(DetailInformationFormStrings.Added_Tooltip, vbLf, DirectCast(singleObject, Part).Part_number)
                                ElseIf (TypeOf singleObject Is Wiring_group) Then
                                    .ToolTipText &= String.Format(DetailInformationFormStrings.Added_Tooltip, vbLf, DirectCast(singleObject, Wiring_group).Id)
                                End If
                            Next

                            For Each singleObject As Object In DirectCast(compareValue, SingleObjectComparisonMapper).Changes.DeletedItems
                                If (TypeOf singleObject Is Part) Then
                                    .ToolTipText &= String.Format(DetailInformationFormStrings.Deleted_Tooltip, vbLf, DirectCast(singleObject, Part).Part_number)
                                ElseIf (TypeOf singleObject Is Wiring_group) Then
                                    .ToolTipText &= String.Format(DetailInformationFormStrings.Deleted_Tooltip, vbLf, DirectCast(singleObject, Wiring_group).Id)
                                End If
                            Next
                        ElseIf (TypeOf compareValue Is WireLengthComparisonMapper) Then
                            For Each wireChangedProperties As ChangedProperty In DirectCast(compareValue, WireLengthComparisonMapper).Changes.ModifiedItems
                                If (wireChangedProperties.ChangedProperties.ContainsKey(WireLengthPropertyName.Length_value.ToString)) Then .ToolTipText = String.Format("{0}{1}{2}{3}{4}{5} {6}", InformationHubStrings.RefDoc_TooltipPart, .Value.ToString, vbLf, vbLf, InformationHubStrings.CompDoc1_TooltipPart, wireChangedProperties.ChangedProperties(WireLengthPropertyName.Length_value.ToString).ToString, "mm")
                            Next

                            For Each wireLength As Wire_length In DirectCast(compareValue, WireLengthComparisonMapper).Changes.NewItems
                                .ToolTipText = String.Format(DetailInformationFormStrings.Added2_Tooltip, vbLf, Math.Round(wireLength.Length_value.Value_component, 2).ToString, "mm")
                            Next

                            For Each wireLength As Wire_length In DirectCast(compareValue, WireLengthComparisonMapper).Changes.DeletedItems
                                .ToolTipText = String.Format(DetailInformationFormStrings.Deleted2_Tooltip, vbLf, Math.Round(wireLength.Length_value.Value_component, 2).ToString, "mm")
                            Next

                            If (.ToolTipText.StartsWith(vbLf)) Then .ToolTipText = .ToolTipText.Substring(1, .ToolTipText.Length - 1)
                        End If

                        If (.Value.ToString = String.Empty) AndAlso (.ToolTipText <> String.Empty) Then DirectCast(.Row.ListObject, UltraDataRow).SetCellValue(.Column.Key, "-")
                    Else
                        .Appearance.FontData.SizeInPoints = 14
                        .Tag = compareObject
                    End If
                End With

                Exit For
            End If
        Next
    End Sub

    Private Sub DetailInformationForm_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        If (e.KeyCode = Keys.Escape) Then
            Me.Close()
        End If
    End Sub

    Private Sub btnApply_Click(sender As Object, e As EventArgs) Handles btnApply.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Abort
    End Sub

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Me.Close()
    End Sub

    Private Sub btnExport_Click(sender As Object, e As EventArgs) Handles btnExport.Click
        Using sfdExcel As New SaveFileDialog
            With sfdExcel
                .DefaultExt = KnownFile.XLSX.Trim("."c)

                If (_kblMapper.GetChanges.Any()) Then
                    .FileName = String.Format("{0}{1}{2}_{3}_{4}_{5}{6}", Now.Year, Format(Now.Month, "00"), Format(Now.Day, "00"), Replace(_kblMapper.HarnessPartNumber, " ", String.Empty), _kblMapper.GetChanges.Max(Function(change) change.Id), Replace(Me.Text, " ", "_"), KnownFile.XLSX)
                Else
                    .FileName = String.Format("{0}{1}{2}_{3}_{4}{5}", Now.Year, Format(Now.Month, "00"), Format(Now.Day, "00"), Replace(_kblMapper.HarnessPartNumber, " ", String.Empty), Replace(Me.Text, " ", "_"), KnownFile.XLSX)
                End If

                .Filter = "Excel files (*.xlsx)|*.xlsx|Excel files (97-2003) (*.xls)|*.xls"
                .Title = DetailInformationFormStrings.ExportToExcelFile_Title

                If (.ShowDialog(Me) = DialogResult.OK) Then
                    Try
                        _exportFileName = .FileName

                        If System.IO.KnownFile.IsXlsx(_exportFileName) Then
                            Me.ugeeDetailInformation.ExportAsync(Me.ugDetailInformation, .FileName, Infragistics.Documents.Excel.WorkbookFormat.Excel2007)
                        Else
                            Me.ugeeDetailInformation.ExportAsync(Me.ugDetailInformation, .FileName, Infragistics.Documents.Excel.WorkbookFormat.Excel97To2003)
                        End If
                    Catch ex As Exception
                        MessageBox.Show(String.Format(DetailInformationFormStrings.ExportExcelError_Msg, vbCrLf, ex.Message), [Shared].MSG_BOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    End Try
                End If
            End With
        End Using
    End Sub

    Private Sub ugDetailInformation_ClickCellButton(sender As Object, e As CellEventArgs) Handles ugDetailInformation.ClickCellButton
        If (DirectCast(e.Cell.Row.ListObject, UltraDataRow).Tag IsNot Nothing) Then
            If (TypeOf DirectCast(e.Cell.Row.ListObject, UltraDataRow).Tag Is IEnumerable(Of Alias_identification)) Then
                Using detailInformationForm As DetailInformationForm = DetailInformationForm.CreateInstance(Me.GetType, DetailInformationFormStrings.AliasId_ColCaption, If(e.Cell.Tag IsNot Nothing, e.Cell.Tag, DirectCast(DirectCast(e.Cell.Row.ListObject, UltraDataRow).Tag, IEnumerable(Of Alias_identification))), Nothing, _kblMapper, _objectId)
                    detailInformationForm.ShowDialog(Me)
                End Using
            ElseIf (TypeOf DirectCast(e.Cell.Row.ListObject, UltraDataRow).Tag Is IEnumerable(Of Change)) Then
                Using detailInformationForm As DetailInformationForm = DetailInformationForm.CreateInstance(Me.GetType, PartPropertyName.Change.ToString, If(e.Cell.Tag IsNot Nothing, e.Cell.Tag, DirectCast(DirectCast(e.Cell.Row.ListObject, UltraDataRow).Tag, IEnumerable(Of Change))), Nothing, _kblMapper, _objectId)
                    detailInformationForm.ShowDialog(Me)
                End Using
            ElseIf (TypeOf DirectCast(e.Cell.Row.ListObject, UltraDataRow).Tag Is String) Then
                Dim externalReferences As New List(Of External_reference)

                For Each externalReference As String In DirectCast(e.Cell.Row.ListObject, UltraDataRow).Tag.ToString.SplitSpace
                    externalReferences.Add(_kblMapper.GetOccurrenceObject(Of External_reference)(externalReference))
                Next

                Using detailInformationForm As DetailInformationForm = DetailInformationForm.CreateInstance(Me.GetType, DetailInformationFormStrings.ExtRef_PropName, If(e.Cell.Tag IsNot Nothing, e.Cell.Tag, externalReferences), Nothing, _kblMapper, _objectId)
                    detailInformationForm.ShowDialog(Me)
                End Using
            ElseIf (TypeOf DirectCast(e.Cell.Row.ListObject, UltraDataRow).Tag Is IEnumerable(Of Installation_instruction)) Then
                Using detailInformationForm As DetailInformationForm = DetailInformationForm.CreateInstance(Me.GetType, InformationHubStrings.InstallInfo_Caption, If(e.Cell.Tag IsNot Nothing, e.Cell.Tag, DirectCast(DirectCast(e.Cell.Row.ListObject, UltraDataRow).Tag, IEnumerable(Of Installation_instruction))), Nothing, _kblMapper, _objectId)
                    detailInformationForm.ShowDialog(Me)
                End Using
            ElseIf (TypeOf DirectCast(e.Cell.Row.ListObject, UltraDataRow).Tag Is Material) Then
                Using detailInformationForm As DetailInformationForm = DetailInformationForm.CreateInstance(Me.GetType, DetailInformationFormStrings.MatInfo_PropName, If(e.Cell.Tag IsNot Nothing, e.Cell.Tag, DirectCast(DirectCast(e.Cell.Row.ListObject, UltraDataRow).Tag, Material)), Nothing, _kblMapper, _objectId)
                    detailInformationForm.ShowDialog(Me)
                End Using
            ElseIf (TypeOf DirectCast(e.Cell.Row.ListObject, UltraDataRow).Tag Is IEnumerable(Of Processing_instruction)) Then
                Using detailInformationForm As DetailInformationForm = DetailInformationForm.CreateInstance(Me.GetType, DetailInformationFormStrings.ProcInfo_PropName, If(e.Cell.Tag IsNot Nothing, e.Cell.Tag, DirectCast(DirectCast(e.Cell.Row.ListObject, UltraDataRow).Tag, IEnumerable(Of Processing_instruction))), Nothing, _kblMapper, _objectId)
                    detailInformationForm.ShowDialog(Me)
                End Using
            ElseIf (TypeOf DirectCast(e.Cell.Row.ListObject, UltraDataRow).Tag Is IEnumerable(Of Localized_string)) Then
                Using detailInformationForm As DetailInformationForm = DetailInformationForm.CreateInstance(Me.GetType, DetailInformationFormStrings.LocalizedDesc_Value, If(e.Cell.Tag IsNot Nothing, e.Cell.Tag, DirectCast(DirectCast(e.Cell.Row.ListObject, UltraDataRow).Tag, IEnumerable(Of Localized_string))), Nothing, _kblMapper, _objectId)
                    detailInformationForm.ShowDialog(Me)
                End Using
            End If
        End If
    End Sub

    Private Sub ugDetailInformation_InitializeLayout(sender As Object, e As InitializeLayoutEventArgs) Handles ugDetailInformation.InitializeLayout
        Me.ugDetailInformation.BeginUpdate()

        InitializeGridLayout(e.Layout)

        If (Me.ugDetailInformation.Rows.Count <> 0) Then
            Dim dataBand As UltraDataBand = If(Me.ugDetailInformation.Rows(0).ListObject IsNot Nothing, DirectCast(Me.ugDetailInformation.Rows(0).ListObject, UltraDataRow).Band, Nothing)
            If (dataBand IsNot Nothing) AndAlso (dataBand.Tag IsNot Nothing) Then
                Dim chgType As Nullable(Of CompareChangeType) = GetChangeType(dataBand?.Tag)
                If chgType.HasValue Then
                    Select Case chgType.Value
                        Case CompareChangeType.[New]
                            e.Layout.Bands(0).Columns(1).Header.Appearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True
                        Case CompareChangeType.Deleted
                            e.Layout.Bands(0).Columns(1).Header.Appearance.FontData.Strikeout = Infragistics.Win.DefaultableBoolean.True
                    End Select
                ElseIf (TypeOf dataBand.Tag Is Dictionary(Of String, Object)) Then
                    For Each row As UltraGridRow In Me.ugDetailInformation.Rows
                        SetGridRowTooltip(dataBand, row)
                    Next
                End If
            End If
        End If

        Me.ugDetailInformation.EndUpdate()
    End Sub

    Private Sub ugDetailInformation_InitializeRow(sender As Object, e As InitializeRowEventArgs) Handles ugDetailInformation.InitializeRow
        InitializeDetailInformationRowCore(e.Row)
    End Sub

    Protected Overridable Sub InitializeDetailInformationRowCore(row As UltraGridRow)
        Dim dataRow As UltraDataRow = DirectCast(row.ListObject, UltraDataRow)

        If (_wireLengthType IsNot Nothing) AndAlso (row.Cells.Exists(DetailInformationFormStrings.LengthType_PropName)) AndAlso (row.Cells(DetailInformationFormStrings.LengthType_PropName).Value.ToString = _wireLengthType) Then
            row.Appearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True
        End If

        For Each cell As UltraGridCell In row.Cells
            If (cell.Value IsNot Nothing AndAlso cell.Value.ToString = Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS) Then
                cell.Style = ColumnStyle.Button
            End If
        Next
    End Sub

    Private Sub ugeeDetailInformation_CellExported(sender As Object, e As ExcelExport.CellExportedEventArgs) Handles ugeeDetailInformation.CellExported
        If (e.Value.ToString = Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS) Then
            e.CurrentWorksheet.Rows(e.CurrentRowIndex).Cells(e.CurrentColumnIndex).CellFormat.Font.Height = 160
        End If
    End Sub

    Private Sub ugeeDetailInformation_ExportEnded(sender As Object, e As ExcelExport.ExportEndedEventArgs) Handles ugeeDetailInformation.ExportEnded
        If (Not e.Canceled) AndAlso (MessageBox.Show(DetailInformationFormStrings.ExportExcelSuccess_Msg, [Shared].MSG_BOX_TITLE, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) = System.Windows.Forms.DialogResult.Yes) Then
            ProcessEx.Start(_exportFileName)
        End If
    End Sub

    Private Sub ugeeDetailInformation_HeaderRowExporting(sender As Object, e As ExcelExport.HeaderRowExportingEventArgs) Handles ugeeDetailInformation.HeaderRowExporting
        If (e.CurrentRowIndex <> 0) AndAlso (e.CurrentOutlineLevel = 0) Then e.Cancel = True
    End Sub

    Protected Function GetChangeType(tag As Object) As Nullable(Of CompareChangeType)
        If tag IsNot Nothing Then
            Dim parsed As Nullable(Of CompareChangeType) = ChangedItem.TryParseFromText(tag.ToString)
            If parsed.HasValue Then
                Return parsed.Value
            End If
        End If
        Return Nothing
    End Function

    Private Sub ugDetailInformation_AfterSelectChange(sender As Object, e As AfterSelectChangeEventArgs) Handles ugDetailInformation.AfterSelectChange
        If (ugDetailInformation.Selected IsNot Nothing AndAlso ugDetailInformation.Selected.Rows.Count = 1) Then
            If (ugDetailInformation.Selected.Rows(0).Band.Columns.Exists(DetailInformationFormStrings.ObjectId)) Then
                If (ugDetailInformation.Selected.Rows(0).GetCellValue(DetailInformationFormStrings.ObjectId) IsNot Nothing) Then
                    Dim id As String = ugDetailInformation.Selected.Rows(0).GetCellValue(DetailInformationFormStrings.ObjectId).ToString
                    If (Not String.IsNullOrEmpty(id)) Then
                        RaiseEvent SelectionChanged(Me, New DetailInformationEventArgs(id))
                    End If
                End If
            End If
        End If
    End Sub

    Private ReadOnly Property InverseCompare As Boolean
        Get
            Static mainForm As MainForm = Application.OpenForms.OfType(Of MainForm).SingleOrDefault
            If mainForm?.GeneralSettings IsNot Nothing Then
                Return mainForm.GeneralSettings.InverseCompare
            End If
            Return False
        End Get
    End Property

End Class