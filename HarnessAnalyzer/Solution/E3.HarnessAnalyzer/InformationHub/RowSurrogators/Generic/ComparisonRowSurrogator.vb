Imports System.Reflection
Imports System.Xml
Imports Infragistics.Documents.Excel.Charts
Imports Infragistics.Win.UltraWinDataSource
Imports Zuken.E3.HarnessAnalyzer.Settings
Imports Zuken.E3.Lib.Schema.Kbl.Properties

Friend MustInherit Class ComparisonRowSurrogator(Of TOccurrence As {IKblOccurrence, New}, TOccurrencePart As {IKblPartObject, New})
    Inherits KblTupleRowSurrogator(Of TOccurrence, TOccurrencePart)
    Implements IKBLCompareSurrogator

    Private _mainForm As MainForm
    Private _isReference As Boolean
    Private _row_occurrence As TOccurrence
    Private _row_Part As TOccurrencePart

    Public Sub New(kbl_reference As IKblContainer, kbl_compare As IKblContainer, corresponding_DS As UltraDataSource, mainForm As MainForm, isChildRowSurrogator As Boolean)
        MyBase.New(kbl_reference, kbl_compare, corresponding_DS, isChildRowSurrogator)
        _mainForm = mainForm
    End Sub

    ReadOnly Property GeneralSettings As GeneralSettings
        Get
            Return _mainForm?.GeneralSettings
        End Get
    End Property

    Protected Overrides Sub OnAfterOccurrenceRowInitialized(row As UltraDataRow)
        _isReference = Not HasChangeTypeWithInverse(row, CompareChangeType.New)
        If TypeOf row.Tag Is TOccurrence Then
            _row_occurrence = CType(row.Tag, TOccurrence)
        End If

        If Not _isReference AndAlso Me.HasCompare Then
            _row_Part = Me.CompareObj.GetPart
        ElseIf Me.ReferenceObj IsNot Nothing Then
            _row_Part = Me.ReferenceObj.GetPart
        End If
    End Sub

    Private ReadOnly Property IKBLCompareSurrogator_IsReference As Boolean Implements IKBLCompareSurrogator.IsReference
        Get
            Return Me.IsReference
        End Get
    End Property

    Property IsReference As Boolean
        Get
            Return _isReference
        End Get
        Protected Set(value As Boolean)
            _isReference = value
        End Set
    End Property

    ReadOnly Property RowPartObject As TOccurrencePart
        Get
            Return _row_Part
        End Get
    End Property

    ReadOnly Property RowOccurrence As TOccurrence
        Get
            Return _row_occurrence
        End Get
    End Property

    Protected Function GetLocalizedDescriptionCellValue([object] As Object) As Object
        Dim localized_description As Localized_string() = Collections.Utils.CastEnumerableOrEmpty(Of Localized_string)(GetRowPropertyValueCore([object], CommonPropertyName.Localized_Description)).ToArray

        If (localized_description.Length > 0) Then
            If (localized_description.Length = 1) Then
                Return localized_description.First.Value
            Else
                Return Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS
            End If
        End If
        Return Nothing
    End Function

    Protected Function GetRowPropertyValueCore([object] As Object, propertyName As String) As Object
        If [object] IsNot Nothing Then
            Dim result As Object = Nothing
            If Fast.TryGetPropertyValue([object], propertyName, result) Then
                Return result
            End If
        End If
        Return Nothing
    End Function

    Protected Function GetRowPropertyValue(propertyName As String, source As CompareSurrogatorObjectSource) As Object
        Select Case source
            Case CompareSurrogatorObjectSource.RowOccurrence
                Return GetRowPropertyValueCore(Me.RowOccurrence, propertyName)
            Case CompareSurrogatorObjectSource.RowPart
                Return GetRowPropertyValueCore(Me.RowPartObject, propertyName)
            Case Else
                Throw New NotImplementedException("source not implemented: " + source.ToString)
        End Select
    End Function

    Private Function GetPropertyValue([object] As Object, propertyName As String) As Object
        If [object] IsNot Nothing Then
            Dim p = [object].GetType.GetProperty(propertyName, BindingFlags.Public Or BindingFlags.Instance Or BindingFlags.NonPublic)
            If p IsNot Nothing AndAlso p.CanRead Then
                If p.PropertyType.IsEnum Then
                    Return XmLUtils.GetStringOrXmlName(p.GetValue([object]))
                Else
                    Return p.GetValue([object])
                End If
            End If
        End If
        Return Nothing
    End Function

    Public Function GetEllipsisPropertyValue(propertyName As String, source As CompareSurrogatorObjectSource, Optional minObjectsCountForEllipsis As Integer = 1) As String
        Dim part_prop_value As Object = GetRowPropertyValue(propertyName, source)
        If TypeOf part_prop_value Is IEnumerable Then
            If CType(part_prop_value, IEnumerable).Cast(Of Object).Count > (minObjectsCountForEllipsis - 1) Then
                Return Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS
            End If
            Return String.Empty
        End If
        Return Nothing
    End Function

    Protected Overrides Function GetCellValueCore(row As UltraDataRow, column As UltraDataColumn) As Object
        Return GetCompareCellValueCore(row, column, _isReference)
    End Function

    Protected Function GetReferenceElementCellValue() As Object
        Dim reference_element As String = TryCast(Me.GetRowPropertyValue(ComponentBoxPropertyName.Reference_element, CompareSurrogatorObjectSource.RowOccurrence), String)
        If Not reference_element.IsNullOrEmpty Then
            If (reference_element.SplitSpace.Length = 1) Then
                If IsReference Then
                    Return Harness.GetReferenceElement(reference_element, Me.ReferenceObj.Kbl)
                Else
                    Return Harness.GetReferenceElement(reference_element, Me.CompareObj.Kbl)
                End If
            Else
                Return Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS
            End If
        End If
        Return Nothing
    End Function

    Public Overridable Function GetPartCellValue(kblPropertyName As String, reference As Boolean) As Object
        Select Case kblPropertyName
            Case PartPropertyName.Part_number,
                 PartPropertyName.Company_name,
                 PartPropertyName.Version,
                 PartPropertyName.Abbreviation,
                 PartPropertyName.Part_description,
                 PartPropertyName.Predecessor_part_number,
                 PartPropertyName.Degree_of_maturity,
                 PartPropertyName.Copyright_note,
                 PartPropertyName.Part_number_type
                Return GetRowPropertyValue(kblPropertyName, CompareSurrogatorObjectSource.RowPart)
            Case PartPropertyName.Part_alias_ids 'HINT: CAUTION! Special-case because the property is only a virtual (does not exist) property
                Return Me.GetAliasIdCellValue(CompareSurrogatorObjectSource.RowPart)
            Case PartPropertyName.Part_description  'HINT: CAUTION! Special-case because the property is only a virtual (does not exist) property
                Return GetRowPropertyValue(NameOf(Part.Description), CompareSurrogatorObjectSource.RowPart)
            Case PartPropertyName.Mass_information
                Dim mass_info As Numerical_value = TryCast(GetRowPropertyValue(kblPropertyName, CompareSurrogatorObjectSource.RowPart), Numerical_value)
                If mass_info IsNot Nothing Then
                    Return Me.GetNumericalCellValue(mass_info, 3)
                End If
            Case PartPropertyName.External_references
                Dim external_refs As String = TryCast(Me.GetRowPropertyValue(kblPropertyName, CompareSurrogatorObjectSource.RowPart), String)
                If external_refs IsNot Nothing AndAlso (external_refs.SplitSpace.Count > 0) Then
                    Return Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS
                End If
            Case PartPropertyName.Material_information
                Dim marterial As Material = TryCast(GetRowPropertyValue(kblPropertyName, CompareSurrogatorObjectSource.RowPart), Material)
                If (marterial IsNot Nothing) Then
                    If (marterial.Material_reference_system Is Nothing) Then
                        Return marterial.Material_key
                    Else
                        Return Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS
                    End If
                End If
            Case PartPropertyName.Change
                Return Me.GetEllipsisPropertyValue(kblPropertyName, CompareSurrogatorObjectSource.RowPart)
            Case PartPropertyName.Part_processing_information 'HINT: CAUTION! Special-case because the property is only a virtual (does not exist) property
                Return Me.GetEllipsisPropertyValue(NameOf(Part.Processing_information), CompareSurrogatorObjectSource.RowPart)
            Case PartPropertyName.Part_Localized_description  'HINT: CAUTION! Special-case because the property is only a virtual (does not exist) property
                Return Me.GetLocalizedStringCellValue(CompareSurrogatorObjectSource.RowPart)
        End Select
        Return Nothing
    End Function

    Protected Overridable Overloads Function GetCompareCellValueCore(row As UltraDataRow, column As UltraDataColumn, reference As Boolean) As Object
        Select Case column.Key
            Case CommonPropertyName.Alias_Id
                Return GetAliasIdCellValue(CompareSurrogatorObjectSource.RowOccurrence)
            '"Installation_information" in general
            Case NameOf(Assembly_part_occurrence.Installation_information) ' TODO: move to CommonPropertyName
                Return GetEllipsisPropertyValue(column.Key, CompareSurrogatorObjectSource.RowOccurrence)
            Case InformationHubStrings.AssignedModules_ColumnCaption
                Return Me.GetAssignedModulesCellValue()
            '"Reference_element" in general
            Case ComponentBoxPropertyName.Reference_element ' TODO: move to CommonPropertyNames
                Return Me.GetReferenceElementCellValue()
            '"Localized_Description" in general
            Case AccessoryPropertyName.Localized_Description
                Return Me.GetLocalizedDescriptionCellValue(Me.RowOccurrence) ' TODO: move to CommonPropertyNames
        End Select

        Dim cellValue As Object = GetPartCellValue(column.Key, reference)

        If cellValue Is Nothing Then
            cellValue = GetRowPropertyValue(column.Key, CompareSurrogatorObjectSource.RowOccurrence)
        End If

        If cellValue Is Nothing Then
            OnUnhandledCellDataRequested?.Invoke(row, column)
        End If

        Return cellValue
    End Function

    Protected Function GetAssignedModulesCellValue(Optional kblId As String = Nothing) As String
        If String.IsNullOrEmpty(kblId) Then
            kblId = Me.RowOccurrence.SystemId
        End If

        Return GetAssignedModulesCellValueCore(kblId)
    End Function

    Protected Overridable Function GetAssignedModulesCellValueCore(kblSystemId As String) As String
        Dim assignedModules As New Text.StringBuilder
        Dim modules As New HashSet(Of String)

        For Each m As [Lib].Schema.Kbl.Module In ReferenceObj.Kbl.GetModulesOfObject(kblSystemId)
            If modules.Add(m.SystemId) Then
                assignedModules.AppendLine(String.Format("{0} [{1}]", m.Abbreviation, m.Part_number))
            End If
        Next

        Return assignedModules.ToString
    End Function

    Protected Friend Function GetLocalizedStringCellValue(source As CompareSurrogatorObjectSource) As Object
        Dim localizedDescription As Localized_string() = Collections.Utils.CastEnumerableOrEmpty(Of Localized_string)(Me.GetRowPropertyValue(NameOf(Part.Localized_description), source)).ToArray
        If localizedDescription.Length = 1 Then
            Return localizedDescription.First.Value
        ElseIf localizedDescription.Length > 1 Then
            Return Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS
        End If
        Return Nothing
    End Function

    Protected Function GetAliasIdCellValue(source As CompareSurrogatorObjectSource) As Object
        Dim alias_Id As Alias_identification() = Collections.Utils.CastEnumerableOrEmpty(Of Alias_identification)(GetRowPropertyValue(CommonPropertyName.Alias_Id, source)).ToArray
        If (alias_Id.Length > 0) Then
            If (alias_Id.Count = 1) AndAlso (alias_Id.First.Description Is Nothing) AndAlso (alias_Id.First.Scope Is Nothing) Then
                Return alias_Id.First.Alias_id
            Else
                Return Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS
            End If
        End If
        Return Nothing
    End Function

    Public ReadOnly Property ReferenceObj As OccurrenceRowSurrogator(Of TOccurrence, TOccurrencePart)
        Get
            Return MyBase.KblOccurrenceRow1
        End Get
    End Property

    Public ReadOnly Property CompareObj As OccurrenceRowSurrogator(Of TOccurrence, TOccurrencePart)
        Get
            Return MyBase.KblOccurrenceRow2
        End Get
    End Property

    ReadOnly Property HasCompare As Boolean
        Get
            Return Me.CompareObj?.Kbl IsNot Nothing
        End Get
    End Property

    ReadOnly Property HasReference As Boolean
        Get
            Return Me.ReferenceObj?.Kbl IsNot Nothing
        End Get
    End Property

    Private ReadOnly Property KblReference As IKblContainer Implements IKBLCompareSurrogator.KblReference
        Get
            Return Me.ReferenceObj?.Kbl
        End Get
    End Property

    Private ReadOnly Property KBLCompare As IKblContainer Implements IKBLCompareSurrogator.KBLCompare
        Get
            Return Me.CompareObj?.Kbl
        End Get
    End Property

    Private ReadOnly Property IKBLCompareSurrogator_DataSource As UltraDataSource Implements IKBLCompareSurrogator.DataSource
        Get
            Return MyBase.DataSource
        End Get
    End Property

    Protected Function GetValueRangeCellValue(value_range As Value_range, Optional digits As Integer = 2) As String
        Dim minimum_value As String = GetNumericalCellValue(New Numerical_value() With {.Value_component = value_range.Minimum, .Unit_component = value_range.Unit_component})
        Dim maximum_value As String = GetNumericalCellValue(New Numerical_value() With {.Value_component = value_range.Maximum, .Unit_component = value_range.Unit_component})
        Return String.Format("Min.: {0} / Max.: {1}", minimum_value, maximum_value)
    End Function

    Protected Function GetNumericalCellValue(numericalValue As Numerical_value, Optional digits As Integer = 2) As String
        Return GetNumericalCellValue(Me, numericalValue, digits)
    End Function

    Public Shared Function GetNumericalCellValue(surrogator As IKBLCompareSurrogator, numericalValue As Numerical_value, Optional digits As Integer = 2) As String
        Dim unit_name As String = If(numericalValue.Unit_component IsNot Nothing, surrogator.KblReference.GetUnit(numericalValue.Unit_component)?.Unit_name, Nothing)
        Dim valueStr As String = Math.Round(numericalValue.Value_component, digits).ToString(System.Globalization.CultureInfo.InvariantCulture)
        If surrogator.IsReference AndAlso unit_name IsNot Nothing Then
            Return String.Format("{0} {1}", valueStr, unit_name)
        Else
            unit_name = If(numericalValue.Unit_component IsNot Nothing, surrogator.KBLCompare.GetUnit(numericalValue.Unit_component)?.Unit_name, Nothing)
            If Not surrogator.IsReference AndAlso unit_name IsNot Nothing Then
                Return String.Format("{0} {1}", valueStr, unit_name)
            End If
            Return valueStr
        End If
    End Function

    Protected Function HasChangeTypeWithInverse(row As UltraDataRow, type As CompareChangeType) As Boolean
        If row.Band.Columns.Exists(InformationHubStrings.DiffType_ColumnCaption) Then
            Dim diffTypeCellValue As Object = row.GetCellValue(InformationHubStrings.DiffType_ColumnCaption)
            Select Case type
                Case CompareChangeType.Modified
                    Return diffTypeCellValue.ToString = InformationHubStrings.Modified_Text
                Case CompareChangeType.Deleted
                    If _mainForm.GeneralSettings.InverseCompare Then
                        Return diffTypeCellValue.ToString = InformationHubStrings.Added_Text
                    End If
                    Return diffTypeCellValue.ToString = InformationHubStrings.Deleted_Text
                Case CompareChangeType.New
                    If _mainForm.GeneralSettings.InverseCompare Then
                        Return diffTypeCellValue.ToString = InformationHubStrings.Deleted_Text
                    End If
                    Return diffTypeCellValue.ToString = InformationHubStrings.Added_Text
                Case Else
                    Throw New NotImplementedException(String.Format("ChangeType -> {0}", GetType(InformationHubStrings).Name))
            End Select
        End If
        Return False
    End Function

    Private Function IKBLCompareSurrogator_GetCellValue(row As UltraDataRow, column As UltraDataColumn) As Object Implements IKBLCompareSurrogator.GetCellValue
        Return MyBase.GetCellValue(row, column)
    End Function

End Class
