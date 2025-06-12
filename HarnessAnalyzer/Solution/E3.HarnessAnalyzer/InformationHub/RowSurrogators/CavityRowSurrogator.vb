Imports System.Data.Common
Imports Infragistics.Win.UltraWinDataSource
Imports Zuken.E3.Lib.Schema.Kbl.Properties

Friend Class CavityRowSurrogator
    Inherits ComparisonRowSurrogator(Of Cavity_occurrence, Cavity)

    Dim _cavityHousing As Tuple(Of Object, Tuple(Of String, String)) = Nothing
    Dim _cavityOccurrence As Cavity_occurrence = Nothing
    Dim _contactPoint As Contact_point = Nothing
    Dim _sealPartNumber As String = String.Empty
    Dim _terminalDescription As String = String.Empty
    Dim _terminalPartNumber As String = String.Empty
    Dim _terminalPlating As String = String.Empty
    Dim _fromReference As Boolean = Nothing

    Public Sub New(kbl_reference As IKblContainer, kbl_compare As IKblContainer, corresponding_DS As UltraDataSource, mainForm As MainForm)
        MyBase.New(kbl_reference, kbl_compare, corresponding_DS, mainForm)
    End Sub

    Public Overrides Function CanGetCellValue(row As UltraDataRow, column As UltraDataColumn) As Boolean
        If row.ParentRow IsNot Nothing Then
            If TypeOf row.Tag Is Tuple(Of Object, Tuple(Of String, String)) Then
                Return True
            End If
        End If
        Return False
    End Function

    Protected Overrides Sub OnAfterOccurrenceRowInitialized(row As UltraDataRow)
        _cavityHousing = TryCast(row.Tag, Tuple(Of Object, Tuple(Of String, String)))
        _cavityOccurrence = TryCast(_cavityHousing.Item1, Cavity_occurrence)
        _contactPoint = TryCast(_cavityHousing.Item1, Contact_point)
        _sealPartNumber = If(_cavityHousing.Item2 IsNot Nothing, _cavityHousing.Item2.Item2, String.Empty)
        _terminalPartNumber = If(_cavityHousing.Item2 IsNot Nothing, _cavityHousing.Item2.Item1, String.Empty)

        If HasChangeTypeWithInverse(row, CompareChangeType.New) Then
            If _cavityOccurrence Is Nothing AndAlso Me.HasCompare Then
                _cavityOccurrence = Me.CompareObj.Kbl.GetCavityOccurrenceOfContactPointId(_contactPoint.SystemId)
                Me.IsReference = False
            End If

            If _cavityOccurrence Is Nothing Then
                _cavityOccurrence = Me.ReferenceObj.Kbl.GetCavityOccurrenceOfContactPointId(_contactPoint.SystemId)
                Me.IsReference = True
            End If
        Else
            If _cavityOccurrence Is Nothing AndAlso Me.HasCompare Then
                _cavityOccurrence = Me.CompareObj.Kbl.GetCavityOccurrenceOfContactPointId(_contactPoint.SystemId)
                Me.IsReference = False
            End If

            If (_cavityOccurrence Is Nothing) Then
                _cavityOccurrence = Me.ReferenceObj.Kbl.GetCavityOccurrenceOfContactPointId(_contactPoint.SystemId)
                Me.IsReference = True
            End If
        End If

        If Not String.IsNullOrEmpty(_terminalPartNumber) Then
            Dim terminal As General_terminal = Me.ReferenceObj.Kbl.GetGeneralTerminals.Where(Function(genTer) genTer.Part_number = _terminalPartNumber).FirstOrDefault
            If (terminal Is Nothing) Then
                terminal = Me.CompareObj.Kbl.GetGeneralTerminals.Where(Function(genTer) genTer.Part_number = _terminalPartNumber).FirstOrDefault
            End If

            If (terminal IsNot Nothing) Then
                _terminalDescription = terminal.Description
                _terminalPlating = terminal.Plating_material
            End If
        End If
    End Sub

    Protected Overrides Function GetCompareCellValueCore(row As UltraDataRow, column As UltraDataColumn, reference As Boolean) As Object
        Select Case column.Key
            Case CommonPropertyName.SystemId
                If _contactPoint IsNot Nothing Then
                    Return _contactPoint.SystemId
                Else
                    Return Me.RowOccurrence.SystemId
                End If
            Case ConnectorPropertyName.Cavity_number
                If reference AndAlso Me.HasReference Then
                    Return If(TryCast(_cavityOccurrence.GetPart(ReferenceObj.Kbl), Cavity)?.Cavity_number, String.Empty)
                ElseIf (Not reference) AndAlso Me.HasCompare Then
                    Return If(TryCast(_cavityOccurrence.GetPart(CompareObj.Kbl), Cavity)?.Cavity_number, String.Empty)
                End If
            Case ConnectorPropertyName.Description
                Return _terminalDescription
            Case ConnectorPropertyName.Terminal_part_number
                Return _terminalPartNumber
            Case ConnectorPropertyName.Terminal_part_information
                If Not String.IsNullOrEmpty(_terminalPartNumber) Then
                    Return Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS
                End If
            Case ConnectorPropertyName.Seal_part_number
                Return _sealPartNumber
            Case ConnectorPropertyName.Seal_part_information
                If Not String.IsNullOrEmpty(_sealPartNumber) Then
                    Return Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS
                End If
            Case ConnectorPropertyName.Plug_part_number
                If Not String.IsNullOrEmpty(_cavityOccurrence?.Associated_plug) Then
                    Dim cavity_plug As Cavity_plug = _cavityOccurrence.GetCavityPlug(Me.ReferenceObj.Kbl)
                    If Not reference AndAlso Not Me.HasReference AndAlso Me.HasCompare Then
                        cavity_plug = _cavityOccurrence.GetCavityPlug(Me.CompareObj.Kbl)
                    End If
                    Return cavity_plug?.Part_number
                End If
            Case ConnectorPropertyName.Plug_part_information
                If (_cavityOccurrence.Associated_plug IsNot Nothing) Then
                    Return Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS
                End If
            Case ConnectorPropertyName.Plating
                Return _terminalPlating
            Case HarnessAnalyzer.[Shared].CORE_WIRE_NUMBER_KEY
                If reference Then
                    If _contactPoint IsNot Nothing Then
                        Return String.Join(" ", Me.ReferenceObj.Kbl.GetWireOrCoresOfContactPoint(_contactPoint.SystemId).Select(Function(wc) wc.Wire_number))
                    End If
                End If
            Case WirePropertyName.Wire_type
                If reference Then
                    Return GetWireTypeCellValueOfContactingWireCores(_contactPoint)
                End If
            Case WirePropertyName.Cross_section_area
                If _fromReference Then
                    Return GetCrossSectionAreaCellValueOfContactingWireCores(_contactPoint)
                End If
            Case WirePropertyName.Core_Colour
                If reference Then
                    Return GetColoursCellValueOfContactingWireCores(_contactPoint)
                End If
            Case Else
                Return MyBase.GetCompareCellValueCore(row, column, reference)
        End Select
        Return Nothing
    End Function

    Protected Overrides Function GetAssignedModulesCellValueCore(kblSystemId As String) As String
        Dim sealOcc As Cavity_seal_occurrence = Nothing
        Dim sTerminalOcc As Special_terminal_occurrence = Nothing
        Dim termOcc As Terminal_occurrence = Nothing
        Dim kblMapper As IKblContainer = If(IsReference, Me.ReferenceObj.Kbl, Me.CompareObj.Kbl)

        For Each info As OccurrencePartInfo In _contactPoint.GetAssociatedParts(kblMapper, KblObjectType.Cavity_seal_occurrence, KblObjectType.Special_terminal_occurrence, KblObjectType.Terminal_occurrence)
            If sealOcc Is Nothing OrElse sTerminalOcc Is Nothing OrElse termOcc Is Nothing Then
                Select Case info.Occurrence.ObjectType
                    Case KblObjectType.Cavity_seal_occurrence
                        If info.Part.Part_Number = _sealPartNumber Then
                            sealOcc = CType(info.Occurrence, Cavity_seal_occurrence)
                        End If
                    Case KblObjectType.Special_terminal_occurrence
                        If info.Part.Part_Number = _terminalPartNumber Then
                            sTerminalOcc = CType(info.Occurrence, Special_terminal_occurrence)
                        End If
                    Case KblObjectType.Terminal_occurrence
                        If info.Part.Part_Number = _terminalPartNumber Then
                            termOcc = CType(info.Occurrence, Terminal_occurrence)
                        End If
                End Select
            Else
                Exit For
            End If
        Next

        'HINT: unclear if the order added to stringbuilder is mandatory -> this is the original-implementation order from trunk, keep that in mind when changing: 1.TerminalOccurrence, 2. Special-Term-Occurrence, 3. Seal-Occrurrence, 4. Cavity-Occcurrence
        Dim result_str As New Text.StringBuilder
        result_str.AppendLine(GetOccurrencencesAssignedModulesCellValueCore(termOcc, sTerminalOcc, sealOcc))

        If (_cavityOccurrence.Associated_plug IsNot Nothing) Then
            'HH: ?? -> if occurrence can be resolved by Associated_plug (which should then be the systemId), why the resolve and not provide it directly ? (original BL from trunk)
            Dim associated_plug As Cavity_plug_occurrence = Me.ReferenceObj.Kbl.GetOccurrenceObject(Of Cavity_plug_occurrence)(_cavityOccurrence.Associated_plug)
            AppendAssignedModulesCellText(result_str, [Lib].Schema.Kbl.Resources.ObjectTypeStrings.Plug, associated_plug.SystemId)
        End If

        Return result_str.ToString
    End Function

    Private Function GetOccurrencencesAssignedModulesCellValueCore(ParamArray occurrences As IKblOccurrence()) As String
        Dim txt As New Text.StringBuilder
        For Each occ As IKblOccurrence In occurrences
            Dim object_type_descr As String = String.Empty
            Select Case occ.ObjectType
                Case KblObjectType.Cavity_seal_occurrence
                    object_type_descr = [Lib].Schema.Kbl.Resources.ObjectTypeStrings.Seals2
                Case KblObjectType.Terminal_occurrence, KblObjectType.Special_terminal_occurrence
                    object_type_descr = [Lib].Schema.Kbl.Resources.ObjectTypeStrings.Terminals2
                Case Else
                    Throw New NotImplementedException($"Type descriptions resolve for kblType ""{occ.ObjectType.ToString}"" not implemented!")
            End Select

            AppendAssignedModulesCellText(txt, object_type_descr, occ.SystemId)
        Next
        Return txt.ToString
    End Function

    Private Sub AppendAssignedModulesCellText(append As Text.StringBuilder, typeDescriptionString As String, kbl_type_object_SystemId As String)
        append.AppendLine($"{typeDescriptionString}:{vbCrLf}{MyBase.GetAssignedModulesCellValueCore(kbl_type_object_SystemId)}")
    End Sub

    Private Function GetWireTypeCellValueOfContactingWireCores(ByVal contactPoint As Contact_point) As String
        If contactPoint IsNot Nothing Then
            Dim wireTypes As New HashSet(Of String)
            For Each wire As IKblWireCoreOccurrence In Me.ReferenceObj.Kbl.GetWireOrCoresOfContactPoint(contactPoint.SystemId)
                Dim wc_part As IKblWireCorePart = CType(wire.GetPart(Me.ReferenceObj.Kbl), IKblWireCorePart)
                If Not String.IsNullOrEmpty(wc_part?.Wire_type) Then
                    wireTypes.Add(wc_part.Wire_type.Trim) ' = hashSet = no double entries possible
                End If
            Next
            Return String.Join(" ", wireTypes)
        End If
        Return String.Empty
    End Function

    Private Function GetColoursCellValueOfContactingWireCores(ByVal contactPoint As Contact_point) As String
        If (contactPoint IsNot Nothing) Then
            Dim colourList As New HashSet(Of String)
            For Each wireCore As IKblWireCoreOccurrence In Me.ReferenceObj.Kbl.GetWireOrCoresOfContactPoint(contactPoint.SystemId)
                Dim wireCorePart As IKblWireCorePart = CType(wireCore.GetPart(Me.ReferenceObj.Kbl), IKblWireCorePart)
                If (wireCorePart IsNot Nothing) Then
                    Dim colours As String = wireCorePart.GetColours
                    If Not String.IsNullOrEmpty(colours) Then
                        colourList.Add(colours) ' = hashSet = no double entries possible
                    End If
                End If
            Next
            Return String.Join(" ", colourList)
        End If
        Return String.Empty
    End Function

    Private Function GetCrossSectionAreaCellValueOfContactingWireCores(ByVal contactPoint As Contact_point) As String
        If (contactPoint IsNot Nothing) Then
            Dim numericalCellValueList As New HashSet(Of String)
            For Each wireCore As IKblWireCoreOccurrence In Me.ReferenceObj.Kbl.GetWireOrCoresOfContactPoint(contactPoint.SystemId)
                Dim wireCorePart As IKblWireCorePart = TryCast(wireCore.GetPart(Me.ReferenceObj.Kbl), IKblWireCorePart)
                If (wireCorePart IsNot Nothing) AndAlso (wireCorePart.Cross_section_area IsNot Nothing) Then
                    Dim cell_value_str As String = Me.GetNumericalCellValue(wireCorePart.Cross_section_area)
                    If Not String.IsNullOrEmpty(cell_value_str) Then
                        numericalCellValueList.Add(cell_value_str) ' = hashSet = no double entries possible
                    End If
                End If
            Next
            Return String.Join(" ", numericalCellValueList)
        End If
        Return String.Empty
    End Function

End Class

