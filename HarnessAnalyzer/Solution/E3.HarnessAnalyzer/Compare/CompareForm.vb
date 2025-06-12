Imports System.ComponentModel
Imports System.IO
Imports System.IO.Compression
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports System.Text
Imports System.Text.RegularExpressions
Imports devDept.Eyeshot.Entities
Imports Infragistics.Win.UltraWinDataSource
Imports Infragistics.Win.UltraWinGrid
Imports Infragistics.Win.UltraWinTabbedMdi
Imports Infragistics.Win.UltraWinToolTip
Imports Zuken.E3.HarnessAnalyzer.Compare.Table
Imports Zuken.E3.HarnessAnalyzer.D3D
Imports Zuken.E3.HarnessAnalyzer.Project.Documents
Imports Zuken.E3.HarnessAnalyzer.Settings
Imports Zuken.E3.Lib.Eyeshot.Model
Imports Zuken.E3.Lib.IO.Files.Hcv

<Obfuscation(Feature:="renaming", ApplyToMembers:=False)>
Public Class CompareForm

    Friend Event CompareHubSelectionChanged(sender As InformationHub, e As InformationHubEventArgs)
    Friend Event LogMessage(sender As CompareForm, e As LogEventArgs)

    <Obfuscation(Feature:="renaming")>
    Friend Event CompareFinished(sender As Object, e As EventArgs)
    Friend Event TrackBarValueChanged(sender As Object, e As EventArgs)

    Private _activeHarnessConfig As Harness_configuration
    Private _activeMapper As KblMapper
    Private _activeDocument As IDocumentForm
    Private _compareDocument As IDocumentForm
    Private _generalSettings As GeneralSettingsBase
    Private _compareHarnessConfig As Harness_configuration
    Private _compareMapper As KblMapper
    Private _compareRunning As Boolean
    Private _comparisonMapperList As New Dictionary(Of KblObjectType, ComparisonMapper)
    Private _currentActiveObjects As ICollection(Of String) = New List(Of String)
    Private _showWarningMessage As Boolean
    Private _differencesDialog As New DiffDefaultPropsWarningDialog
    Private _isDirty As Boolean = False
    Private _dataErrorException As Exception
    Private _harnessPartNumberMismatch As Boolean
    Private _currentCheckedResult As CheckedCompareResult
    Private _compare_Mappers_Use As KblObjectType() = New KblObjectType() {
                                                     KblObjectType.Approval,
                                                     KblObjectType.Accessory_occurrence,
                                                     KblObjectType.Assembly_part_occurrence,
                                                     KblObjectType.Special_wire_occurrence,
                                                     KblObjectType.Change_description,
                                                     KblObjectType.Component_box_occurrence,
                                                     KblObjectType.Component_occurrence,
                                                     KblObjectType.Connector_occurrence,
                                                     KblObjectType.Co_pack_occurrence,
                                                     KblObjectType.Default_dimension_specification,
                                                     KblObjectType.Dimension_specification,
                                                     KblObjectType.Fixing_occurrence,
                                                     KblObjectType.Module,
                                                     KblObjectType.Net,
                                                     KblObjectType.Segment,
                                                     KblObjectType.Node,
                                                     KblObjectType.Wire_occurrence}

    Private _internalSetComment As Boolean = False
    Private _rowsChecked As New Dictionary(Of String, UltraDataRow)
    Private _rowsUnChecked As New Dictionary(Of String, UltraDataRow)
    Private _mainForm As MainForm
    Private _compositeCompare As Boolean = False
    Private _compareResult As New Dictionary(Of KblObjectType, List(Of ChangedObject))
    Private _refModel As E3.Lib.Model.IEEModel
    Private _compModel As E3.Lib.Model.IEEModel
    Private _netList As New List(Of String)
    Private _reloadedDocuments As New List(Of Infragistics.Win.ValueListItem)
    Private _3DSelection As Boolean = False

    Private WithEvents _compareHub As InformationHub
    Private WithEvents _d3dCntrl As D3DComparerCntrl

    Public Sub New(activeDocument As IDocumentForm, ParamArray compareDocuments As IDocumentForm())
        InitializeComponent()

        SplitContainer1.Panel1Collapsed = True
        SplitContainer1.Panel2Collapsed = False

        _compositeCompare = False
        _activeMapper = CType(activeDocument.Kbl, KblMapper)
        _activeDocument = activeDocument
        _generalSettings = activeDocument.GeneralSettings
        _mainForm = TryCast(activeDocument, DocumentForm)?.MainForm
        _refModel = TryCast(activeDocument?.HcvDocument, HcvDocument)?.Model

        Initialize(activeDocument, compareDocuments)
    End Sub

    Public Sub New(activeMapper As KblMapper, activeHarnessConfig As Harness_configuration, compareMapper As KblMapper, compHarnessConfig As Harness_configuration, mainForm As MainForm, generalSettings As GeneralSettingsBase)
        'HINT: to be used for module compare on composites only (two modules from the same document)
        InitializeComponent()
        _compositeCompare = True
        _mainForm = mainForm
        _activeDocument = _mainForm?.ActiveDocument
        _generalSettings = generalSettings
        _activeMapper = activeMapper
        _compareMapper = compareMapper

        Me.btnSelectDocument.Enabled = False
        Me.uceCompareDocument.Enabled = False
        Me.txtReferenceDocument.Enabled = False
        Me.lblCompareDocument.Enabled = False
        Me.lblReferenceDocument.Enabled = False

        Me.BackColor = Color.White
        Me.Icon = My.Resources.CompareData
        Me.Text = CompareFormStrings.CaptionCompositeCompare

        Me.btnCompare.Enabled = False
        Me.btnExport.Enabled = False
        Me.lblLegend1.Visible = False
        Me.lblLegend2.Visible = False
        Me.lblLegend3.Visible = False
        Me.upbCompare.Visible = False

        Me.uceReferenceConfig.Items.Add(activeHarnessConfig, activeHarnessConfig.Part_number)
        Me.uceReferenceConfig.Items.Add(compHarnessConfig, compHarnessConfig.Part_number)

        Me.uceCompareConfig.Items.Add(compHarnessConfig, compHarnessConfig.Part_number)
        Me.uceCompareConfig.Items.Add(activeHarnessConfig, activeHarnessConfig.Part_number)

        Me.uceReferenceConfig.SelectedItem = Me.uceReferenceConfig.Items(0)
        Me.uceCompareConfig.SelectedItem = Me.uceCompareConfig.Items(0)

        SplitContainer1.Panel1Collapsed = True
        SplitContainer1.Panel2Collapsed = False
    End Sub

    Public Sub New(activeMapper As KblMapper, compareMapper As KblMapper, settings As IHarnessAnalyzerSettingsProvider)
        Me.New(activeMapper, activeMapper.GetHarnessConfigurations.FirstOrDefault, compareMapper, compareMapper.GetHarnessConfigurations.FirstOrDefault, TryCast(settings, MainForm), settings.GeneralSettings)
    End Sub

    Public Function CompareDocuments() As Dictionary(Of KblObjectType, ComparisonMapper)
        _currentActiveObjects = New List(Of String)
        _showWarningMessage = True

        Dim refTxt As String = Me.uceReferenceConfig.SelectedItem.DisplayText
        If (_activeHarnessConfig Is Nothing) Then
            If (_activeMapper.GetModules IsNot Nothing) Then
                _currentActiveObjects.AddRange(_activeMapper.GetModules.ToIds)
            End If

            _currentActiveObjects = GetActiveObjectsAndOrUpdateActiveModulesCollection(_currentActiveObjects, _activeMapper, refTxt, CompareFormStrings.Reference_Msg)
        Else
            _currentActiveObjects = GetActiveObjectsAndOrUpdateActiveModulesCollection(_activeHarnessConfig.Modules.SplitSpace.ToList, _activeMapper, refTxt, CompareFormStrings.Reference_Msg)
        End If

        If _currentActiveObjects Is Nothing Then
            Return Nothing
        End If

        _currentCheckedResult = Me.CheckedResultList.AddOrGet(Me.uceCompareDocument.Text, Me.txtReferenceDocument.Text)

        If (Me.bwCompare.IsBusy) AndAlso (Me.bwCompare.CancellationPending) Then
            Return Nothing
        End If

        Dim compareActiveObjects As ICollection(Of String) = New List(Of String)
        Dim compTxt As String = Me.uceCompareConfig.SelectedItem.DisplayText
        If (_compareHarnessConfig Is Nothing) Then
            If (_compareMapper.GetModules IsNot Nothing) Then
                compareActiveObjects.AddRange(_compareMapper.GetModules.ToIds)
            End If
            compareActiveObjects = GetActiveObjectsAndOrUpdateActiveModulesCollection(compareActiveObjects, _compareMapper, compTxt, CompareFormStrings.Compare_Msg)
        Else
            compareActiveObjects = GetActiveObjectsAndOrUpdateActiveModulesCollection(_compareHarnessConfig.Modules.SplitSpace.ToList, _compareMapper, compTxt, CompareFormStrings.Compare_Msg)
        End If

        If (compareActiveObjects Is Nothing) Then
            Return Nothing
        End If

        If (Me.bwCompare.IsBusy) AndAlso (Me.bwCompare.CancellationPending) Then
            Return Nothing
        End If

        _comparisonMapperList = CompareDocumentsCore(compareActiveObjects, _compare_Mappers_Use)

        Return _comparisonMapperList
    End Function

    Public ReadOnly Property IsD3D As Boolean
        Get
            If (CompareDocument IsNot Nothing AndAlso CompareDocument.HcvDocument IsNot Nothing AndAlso ActiveDocument IsNot Nothing AndAlso ActiveDocument.HcvDocument IsNot Nothing) Then
                Return True
            Else
                Return False
            End If
        End Get
    End Property

    Public Property CompareDocument As IDocumentForm
        Get
            Return _compareDocument
        End Get
        Set(value As IDocumentForm)
            _compareDocument = value
            If _compareDocument IsNot Nothing AndAlso _compareDocument.Model IsNot Nothing Then
                _compModel = _compareDocument.Model
            End If
        End Set
    End Property

    Private Function CompareDocumentsCore(compareActiveObjects As ICollection(Of String), ParamArray compareMappersUse() As KblObjectType) As Dictionary(Of KblObjectType, ComparisonMapper)
        Dim mapperDic As New Dictionary(Of KblObjectType, ComparisonMapper)
        For Each ta As TypeWithA(Of KblObjectTypeAttribute) In ComparisonMapper.All
            If (Me.bwCompare.IsBusy) AndAlso (Me.bwCompare.CancellationPending) Then
                Return Nothing
            End If

            If compareMappersUse.Contains(ta.Attribute.ObjectType) Then
                mapperDic.Add(ta.Attribute.ObjectType, CompareObjects(ta, compareActiveObjects))
                If (Me.bwCompare.IsBusy) Then
                    Me.bwCompare.ReportProgress(CInt((mapperDic.Count * 100) / compareMappersUse.Length))
                End If
            End If
        Next
        Return mapperDic
    End Function

    Private Function CompareObjects(comparisonMapperType As TypeWithA(Of KblObjectTypeAttribute), compareActiveObjects As ICollection(Of String)) As ComparisonMapper
        If Not GetType(ComparisonMapper).IsAssignableFrom(comparisonMapperType.Type) Then
            Throw New ArgumentException(String.Format("Invalid argument type: must be derived from type {0}", GetType(ComparisonMapper).Name), NameOf(comparisonMapperType))
        End If

        Dim mapper As ComparisonMapper
        Try
            mapper = CType(Activator.CreateInstance(comparisonMapperType.Type, _activeMapper, _compareMapper, _currentActiveObjects, compareActiveObjects, _generalSettings), ComparisonMapper)
            Dim appearance As GridAppearance = GridAppearance.ResolveByObjectType(comparisonMapperType.Attribute.ObjectType)
            If (appearance IsNot Nothing) Then
                mapper.ExcludedProperties = appearance.ExcludedProperties
            End If
            mapper.CompareObjects()
            Return mapper
        Finally
        End Try
    End Function

    Private Function CompareObjects(Of T As ComparisonMapper)(compareActiveObjects As ICollection(Of String)) As T
        Return CType(CompareObjects(ComparisonMapper.All.Where(Function(ta) ta.Type Is GetType(T)).Single, compareActiveObjects), T)
    End Function

    Private Sub AddOccurrenceObjectToList(ByRef activeObjectIds As ICollection(Of String), considerActiveObjects As Boolean, kblId As String, kblMapper As KblMapper, ByRef multipleObjects As Dictionary(Of String, String), occurrenceObject As IKblBaseObject)
        Dim objectTypeText As String = Replace(occurrenceObject.GetType.ToString, String.Format("{0}.", occurrenceObject.GetType.Namespace), String.Empty)
        Dim harness As String = kblMapper.HarnessPartNumber
        Dim occType As KblObjectType = occurrenceObject.ObjectType
        Select Case occurrenceObject.ObjectType
            Case KblObjectType.Accessory_occurrence
                With DirectCast(occurrenceObject, Accessory_occurrence)
                    If (Not activeObjectIds.Contains(.Id)) Then
                        Dim count As Integer = kblMapper.GetAccessoryOccurrences.Where(Function(accessoryOcc) accessoryOcc.Id = .Id).Count
                        If (count = 1) Then
                            activeObjectIds.Add(.Id)
                        ElseIf (count > 1) AndAlso (Not multipleObjects.ContainsKey(String.Format("{0}|{1}", .Id, objectTypeText))) Then
                            multipleObjects.Add(String.Format("{0}|{1}", .Id, objectTypeText), String.Format(CompareFormStrings.MultipleAccessoryInstances_Msg, count, .Id))
                        End If
                    End If
                End With
            Case KblObjectType.Approval
                With DirectCast(occurrenceObject, Approval)
                    If (.Name IsNot Nothing) AndAlso (Not activeObjectIds.Contains(.Name)) Then
                        Dim count As Integer = kblMapper.GetApprovals.Where(Function(appr) appr.Name IsNot Nothing AndAlso appr.Name = .Name).Count
                        If (count = 1) Then
                            activeObjectIds.Add(.Name)
                        ElseIf (count > 1) AndAlso (Not multipleObjects.ContainsKey(String.Format("{0}|{1}", .Name, objectTypeText))) Then
                            multipleObjects.Add(String.Format("{0}|{1}", .Name, objectTypeText), String.Format(CompareFormStrings.MultipleApprovalInstances_Msg, count, .Name))
                        End If
                    End If
                End With
            Case KblObjectType.Assembly_part_occurrence
                With DirectCast(occurrenceObject, Assembly_part_occurrence)
                    If (Not activeObjectIds.Contains(.Id)) Then
                        Dim count As Integer = kblMapper.GetAssemblyPartOccurrences.Where(Function(assemblyPartOcc) assemblyPartOcc.Id = .Id).Count
                        If (count = 1) Then
                            activeObjectIds.Add(.Id)
                        ElseIf (count > 1) AndAlso (Not multipleObjects.ContainsKey(String.Format("{0}|{1}", .Id, objectTypeText))) Then
                            multipleObjects.Add(String.Format("{0}|{1}", .Id, objectTypeText), String.Format(CompareFormStrings.MultipleAssemblyPartInstances_Msg, count, .Id))
                        End If
                    End If
                End With
            Case KblObjectType.Cavity_occurrence
                If (Not considerActiveObjects) Then
                    ' HINT: "Id" property of Cavity_occurrence element is NOT mandatory, therefore no check for same Id values possible!!!
                    For Each connectorOccurrence As Connector_occurrence In kblMapper.GetConnectorOccurrences.Where(Function(conOcc) conOcc.Slots IsNot Nothing AndAlso conOcc.Slots.Length <> 0 AndAlso conOcc.Slots(0).Cavities.Where(Function(cavOcc) cavOcc.SystemId = kblId).Any())
                        For Each cavityOccurrence As Cavity_occurrence In connectorOccurrence.Slots(0).Cavities.Where(Function(cavOcc) cavOcc.SystemId = kblId)
                            activeObjectIds.TryAdd(String.Format("Cav_{0}|{1}", connectorOccurrence.Id, DirectCast(kblMapper.KBLPartMapper(cavityOccurrence.Part), Cavity).Cavity_number))
                        Next
                    Next
                End If
            Case KblObjectType.Cavity_plug_occurrence
                ' HINT: "Id" property of Cavity_plug element is NOT mandatory, therefore no check for same Id values possible!!!
                For Each connectorOccurrence As Connector_occurrence In kblMapper.GetConnectorOccurrences.Where(Function(conOcc) conOcc.Slots IsNot Nothing AndAlso conOcc.Slots.Length <> 0 AndAlso conOcc.Slots(0).Cavities.Where(Function(cavOcc) cavOcc.Associated_plug IsNot Nothing AndAlso cavOcc.Associated_plug = kblId).Any())
                    For Each cavityOccurrence As Cavity_occurrence In connectorOccurrence.Slots(0).Cavities.Where(Function(cavOcc) cavOcc.Associated_plug IsNot Nothing AndAlso cavOcc.Associated_plug = kblId)
                        activeObjectIds.TryAdd(String.Format("Cav_{0}|{1}", connectorOccurrence.Id, DirectCast(kblMapper.KBLPartMapper(cavityOccurrence.Part), Cavity).Cavity_number))
                    Next
                Next
            Case KblObjectType.Cavity_seal_occurrence, KblObjectType.Special_terminal_occurrence, KblObjectType.Terminal_occurrence
                ' HINT: "Id" property of Cavity_seal_occurrence and Terminal_occurrence elements is NOT mandatory, therefore no check for same Id values possible!!!
                If (TypeOf occurrenceObject Is Cavity_seal_occurrence) Then
                    With DirectCast(occurrenceObject, Cavity_seal_occurrence)
                        If (Not activeObjectIds.Contains(.Id)) Then
                            Dim count As Integer = kblMapper.GetCavitySealOccurrences.Where(Function(sealOcc) sealOcc.SystemId = .SystemId).Count
                            If (count = 1) Then
                                activeObjectIds.Add(.SystemId)
                            ElseIf (count > 1) AndAlso (Not multipleObjects.ContainsKey(String.Format("{0}|{1}", .SystemId, objectTypeText))) Then
                                multipleObjects.Add(String.Format("{0}|{1}", .SystemId, objectTypeText), String.Format(CompareFormStrings.MultipleSealInstances_Msg, count, .SystemId))
                            End If
                        End If
                    End With
                ElseIf (TypeOf occurrenceObject Is Special_terminal_occurrence) Then
                    With DirectCast(occurrenceObject, Special_terminal_occurrence)
                        If (Not activeObjectIds.Contains(.SystemId)) Then
                            Dim count As Integer = kblMapper.GetSpecialTerminalOccurrences.Where(Function(termOcc) termOcc.SystemId = .SystemId).Count
                            If (count = 1) Then
                                activeObjectIds.Add(.SystemId)
                            ElseIf (count > 1) AndAlso (Not multipleObjects.ContainsKey(String.Format("{0}|{1}", .SystemId, objectTypeText))) Then
                                multipleObjects.Add(String.Format("{0}|{1}", .SystemId, objectTypeText), String.Format(CompareFormStrings.MultipleTerminalInstances_Msg, count, .SystemId))
                            End If
                        End If
                    End With
                Else
                    With DirectCast(occurrenceObject, Terminal_occurrence)
                        If (Not activeObjectIds.Contains(.SystemId)) Then
                            Dim count As Integer = kblMapper.GetTerminalOccurrences.Where(Function(termOcc) termOcc.SystemId = .SystemId).Count
                            If (count = 1) Then
                                activeObjectIds.Add(.SystemId)
                            ElseIf (count > 1) AndAlso (Not multipleObjects.ContainsKey(String.Format("{0}|{1}", .SystemId, objectTypeText))) Then
                                multipleObjects.Add(String.Format("{0}|{1}", .SystemId, objectTypeText), String.Format(CompareFormStrings.MultipleTerminalInstances_Msg, count, .SystemId))
                            End If
                        End If
                    End With
                End If

                For Each connectorOccurrence As Connector_occurrence In kblMapper.GetConnectorOccurrences.Where(Function(conOcc) conOcc.Contact_points IsNot Nothing AndAlso conOcc.Contact_points.Where(Function(conPoint) conPoint.Associated_parts IsNot Nothing AndAlso conPoint.Associated_parts.SplitSpace.ToList.Contains(kblId)).Any())
                    For Each contactPoint As Contact_point In connectorOccurrence.Contact_points.Where(Function(conPoint) conPoint.Associated_parts IsNot Nothing AndAlso conPoint.Associated_parts.SplitSpace.Contains(kblId))
                        Dim contactPointId As String = contactPoint.GetUniqueIdForCompare(connectorOccurrence.Id, kblMapper)
                        Dim conn_cav_str As String = String.Format("Cav_{0}|{1}", connectorOccurrence.Id, kblMapper.GetCavityOfContactPointId(contactPoint.SystemId)?.Cavity_number)
                        activeObjectIds.TryAdd(conn_cav_str)
                        activeObjectIds.TryAdd(contactPointId)
                    Next
                Next
            Case KblObjectType.Change_description
                With DirectCast(occurrenceObject, Change_description)
                    If (.Id IsNot Nothing) AndAlso (Not activeObjectIds.Contains(.Id)) Then
                        Dim count As Integer = kblMapper.GetChangeDescriptions.Where(Function(changeDesc) changeDesc.Id IsNot Nothing AndAlso changeDesc.Id = .Id).Count
                        If (count = 1) Then
                            activeObjectIds.Add(.Id)
                        ElseIf (count > 1) Then
                            multipleObjects.TryAdd(String.Format("{0}|{1}", .Id, objectTypeText), String.Format(CompareFormStrings.MultipleChangeDescriptionInstances_Msg, count, .Id))
                        End If
                    End If
                End With
            Case KblObjectType.Connection
                With DirectCast(occurrenceObject, Connection)
                    If (.Id IsNot Nothing) AndAlso (Not activeObjectIds.Contains(.Id)) Then
                        Dim count As Integer = kblMapper.GetConnections.Where(Function(connection) connection.Id IsNot Nothing AndAlso connection.Id = .Id).Count
                        If (count = 1) Then
                            activeObjectIds.Add(.Id)
                        ElseIf (count > 1) Then
                            Dim wireCore_occ As IKblWireCoreOccurrence = kblMapper.GetWireOrCoreOccurrence(.Wire)
                            multipleObjects.TryAdd(String.Format("{0}|{1}", .Id, objectTypeText), String.Format(CompareFormStrings.MultipleConnectionInstances_Msg, count, .Id, If(.Signal_name IsNot Nothing AndAlso .Signal_name <> String.Empty, .Signal_name, "-"), wireCore_occ.Wire_number))
                        End If
                    End If
                End With
            Case KblObjectType.Core_occurrence
                With DirectCast(occurrenceObject, Core_occurrence)
                    If (Not activeObjectIds.Contains(.Wire_number)) Then
                        Dim count As Integer = kblMapper.GetHarness.General_wire_occurrence.Where(Function(genWireOcc) TypeOf genWireOcc Is Special_wire_occurrence AndAlso DirectCast(genWireOcc, Special_wire_occurrence).Core_occurrence.Where(Function(coreOcc) coreOcc.Wire_number = .Wire_number).Count = 1).Count
                        If (count = 1) Then
                            activeObjectIds.Add(.Wire_number)
                        ElseIf (count > 1) Then
                            multipleObjects.TryAdd(String.Format("{0}|{1}", .Wire_number, objectTypeText), String.Format(CompareFormStrings.MultipleCoreInstances_Msg, count, .Wire_number))
                        End If
                    End If
                End With
            Case KblObjectType.Component_box_occurrence
                With DirectCast(occurrenceObject, Component_box_occurrence)
                    If (Not activeObjectIds.Contains(.Id)) Then
                        Dim count As Integer = kblMapper.GetComponentBoxOccurrences.Where(Function(compBoxOcc) compBoxOcc.Id = .Id).Count
                        If (count = 1) Then
                            activeObjectIds.Add(.Id)
                        ElseIf (count > 1) Then
                            multipleObjects.TryAdd(String.Format("{0}|{1}", .Id, objectTypeText), String.Format(CompareFormStrings.MultipleComponentBoxInstances_Msg, count, .Id))
                        End If
                    End If
                End With
            Case KblObjectType.Component_occurrence, KblObjectType.Fuse_occurrence
                With DirectCast(occurrenceObject, Component_occurrence)
                    If (Not activeObjectIds.Contains(.Id)) Then
                        Dim count As Integer = kblMapper.GetComponentOccurrences.Where(Function(compOcc) compOcc.Id = .Id).Count
                        If (count = 1) Then
                            activeObjectIds.Add(.Id)
                        ElseIf (count > 1) Then
                            multipleObjects.TryAdd(String.Format("{0}|{1}", .Id, objectTypeText), String.Format(CompareFormStrings.MultipleComponentInstances_Msg, count, .Id))
                        End If
                    End If
                End With
            Case KblObjectType.Connector_occurrence
                With DirectCast(occurrenceObject, Connector_occurrence)
                    If (Not activeObjectIds.Contains(.Id)) Then
                        Dim count As Integer = kblMapper.GetConnectorOccurrences.Where(Function(connOcc) connOcc.Id = .Id AndAlso kblMapper.KBLPartMapper.ContainsKey(connOcc.Part) AndAlso (Not kblMapper.GetPart(Of Connector_housing)(connOcc.Part).IsKSL)).Count
                        If (count = 1) Then
                            activeObjectIds.Add(.Id)
                        ElseIf (count > 1) Then
                            multipleObjects.TryAdd(String.Format("{0}|{1}", .Id, objectTypeText), String.Format(CompareFormStrings.MultipleConnectorInstances_Msg, count, .Id))
                        End If

                        If (.Slots.Length <> 0) Then
                            For Each cavityOcc As Cavity_occurrence In .Slots(0).Cavities
                                If (String.IsNullOrEmpty(cavityOcc.Associated_plug)) Then
                                    activeObjectIds.TryAdd(String.Format("Cav_{0}|{1}", .Id, DirectCast(kblMapper.KBLPartMapper(cavityOcc.Part), Cavity).Cavity_number))
                                End If
                            Next
                        End If
                    End If
                End With
            Case KblObjectType.Co_pack_occurrence
                With DirectCast(occurrenceObject, Co_pack_occurrence)
                    If (Not activeObjectIds.Contains(.Id)) Then
                        Dim count As Integer = kblMapper.GetCoPackOccurrences.Where(Function(coPackOcc) coPackOcc.Id = .Id).Count
                        If (count = 1) Then
                            activeObjectIds.Add(.Id)
                        ElseIf (count > 1) Then
                            multipleObjects.TryAdd(String.Format("{0}|{1}", .Id, objectTypeText), String.Format(CompareFormStrings.MultipleCoPackInstances_Msg, count, .Id))
                        End If
                    End If
                End With
            Case KblObjectType.Default_dimension_specification
                With DirectCast(occurrenceObject, Default_dimension_specification)
                    Dim dimValRange As String = If(.Dimension_value_range IsNot Nothing, String.Format("Min.: {0} / Max.: {1}", Math.Round(.Dimension_value_range.Minimum, 2), Math.Round(.Dimension_value_range.Maximum, 2)), String.Empty)

                    If (dimValRange <> String.Empty) AndAlso (Not activeObjectIds.Contains(dimValRange)) Then
                        Dim count As Integer = kblMapper.GetDefaultDimensionSpecifications.Where(Function(dds) dds.Dimension_value_range IsNot Nothing AndAlso String.Format("Min.: {0} / Max.: {1}", Math.Round(dds.Dimension_value_range.Minimum, 2), Math.Round(dds.Dimension_value_range.Maximum, 2)) = dimValRange).Count
                        If (count = 1) Then
                            activeObjectIds.Add(dimValRange)
                        ElseIf (count > 1) Then
                            multipleObjects.TryAdd(String.Format("{0}|{1}", dimValRange, objectTypeText), String.Format(CompareFormStrings.MultipleDefDimSpecInstances_Msg, count, dimValRange))
                        End If
                    End If
                End With
            Case KblObjectType.Dimension_specification
                With DirectCast(occurrenceObject, Dimension_specification)
                    If (.Id IsNot Nothing) AndAlso (Not activeObjectIds.Contains(.Id)) Then
                        Dim count As Integer = kblMapper.GetDimensionSpecifications.Where(Function(ds) ds.Id IsNot Nothing AndAlso ds.Id = .Id).Count
                        If (count = 1) Then
                            activeObjectIds.Add(.Id)
                        ElseIf (count > 1) Then
                            multipleObjects.TryAdd(String.Format("{0}|{1}", .Id, objectTypeText), String.Format(CompareFormStrings.MultipleDimSpecInstances_Msg, count, .Id))
                        End If
                    End If
                End With
            Case KblObjectType.Fixing_occurrence
                With DirectCast(occurrenceObject, Fixing_occurrence)
                    If (Not activeObjectIds.Contains(.Id)) Then
                        Dim count As Integer = kblMapper.GetFixingOccurrences.Where(Function(fixingOcc) fixingOcc.Id = .Id).Count
                        If (count = 1) Then
                            activeObjectIds.Add(.Id)
                        ElseIf (count > 1) Then
                            multipleObjects.TryAdd(String.Format("{0}|{1}", .Id, objectTypeText), String.Format(CompareFormStrings.MultipleFixingInstances_Msg, count, .Id))
                        End If
                    End If
                End With
            Case KblObjectType.Node
                With DirectCast(occurrenceObject, Node)
                    If (Not activeObjectIds.Contains(.Id)) Then
                        Dim count As Integer = kblMapper.GetNodes.Where(Function(node) node.Id = .Id).Count
                        If (count = 1) Then
                            activeObjectIds.Add(.Id)
                        ElseIf (count > 1) Then
                            multipleObjects.TryAdd(String.Format("{0}|{1}", .Id, objectTypeText), String.Format(CompareFormStrings.MultipleVertexInstances_Msg, count, .Id))
                        End If
                    End If
                End With
            Case KblObjectType.Segment
                With DirectCast(occurrenceObject, Segment)
                    If (Not activeObjectIds.Contains(.Id)) Then
                        Dim count As Integer = kblMapper.GetSegments.Where(Function(segment) segment.Id = .Id).Count
                        If (count = 1) Then
                            activeObjectIds.Add(.Id)
                        ElseIf (count > 1) Then
                            multipleObjects.TryAdd(String.Format("{0}|{1}", .Id, objectTypeText), String.Format(CompareFormStrings.MultipleSegmentInstances_Msg, count, .Id))
                        End If
                    End If
                End With
            Case KblObjectType.Special_wire_occurrence
                With DirectCast(occurrenceObject, Special_wire_occurrence)
                    If (Not activeObjectIds.Contains(.Special_wire_id)) Then
                        Dim count As Integer = kblMapper.GetHarness.General_wire_occurrence.Where(Function(genWireOcc) TypeOf genWireOcc Is Special_wire_occurrence AndAlso DirectCast(genWireOcc, Special_wire_occurrence).Special_wire_id = .Special_wire_id).Count
                        If (count = 1) Then
                            activeObjectIds.Add(.Special_wire_id)
                        ElseIf (count > 1) Then
                            multipleObjects.TryAdd(String.Format("{0}|{1}", .Special_wire_id, objectTypeText), String.Format(CompareFormStrings.MultipleCableInstances_Msg, count, .Special_wire_id))
                        End If
                    End If
                End With
            Case KblObjectType.Wire_occurrence
                With DirectCast(occurrenceObject, Wire_occurrence)
                    If (Not activeObjectIds.Contains(.Wire_number)) Then
                        Dim count As Integer = kblMapper.GetHarness.General_wire_occurrence.Where(Function(genWireOcc) TypeOf genWireOcc Is Wire_occurrence AndAlso DirectCast(genWireOcc, Wire_occurrence).Wire_number = .Wire_number).Count
                        If (count = 1) Then
                            activeObjectIds.Add(.Wire_number)
                        ElseIf (count > 1) Then
                            multipleObjects.TryAdd(String.Format("{0}|{1}", .Wire_number, objectTypeText), String.Format(CompareFormStrings.MultipleWireInstances_Msg, count, .Wire_number))
                        End If
                    End If
                End With
            Case KblObjectType.Wire_protection_occurrence
                With DirectCast(occurrenceObject, Wire_protection_occurrence)
                    If (Not activeObjectIds.Contains(.SystemId)) Then
                        Dim count As Integer = kblMapper.GetHarness.Wire_protection_occurrence.Where(Function(wireProtOcc) wireProtOcc.Id = .Id).Count
                        If (count = 1) Then
                            activeObjectIds.Add(.SystemId)
                        ElseIf (count > 1) Then
                            multipleObjects.TryAdd(String.Format("{0}|{1}", .Id, objectTypeText), String.Format(CompareFormStrings.MultipleWireProtectionInstances_Msg, count, .Id))
                        End If
                    End If
                End With
            Case KblObjectType.Cartesian_point,
                 KblObjectType.Slot_occurrence,
                 KblObjectType.Contact_point,
                 KblObjectType.External_reference,
                 KblObjectType.Fixing_assignment,
                 KblObjectType.Protection_area,
                 KblObjectType.Change,
                 KblObjectType.Module_configuration,
                 KblObjectType.Module,
                 KblObjectType.Module_family,
                 KblObjectType.Wiring_group
                'HINT: nothing to do here, just catch them to avoid the not implemented exception
            Case Else
                Throw New NotImplementedException($"Object type ""{occurrenceObject.ObjectType}"" not implemented for method ""{NameOf(AddOccurrenceObjectToList)}"" !")
        End Select
    End Sub

    Private Sub CompareHarnessModuleConfigurations()
        Me.lblConfigStatus.Text = String.Empty
        Me.picConfigStatus.Image = Nothing

        If (_activeHarnessConfig IsNot Nothing) AndAlso (_compareHarnessConfig IsNot Nothing) Then
            Dim activeModuleIds As List(Of String) = _activeHarnessConfig.Modules.SplitSpace.ToList
            Dim activeModulePartNumbers As New List(Of String)

            For Each moduleId As String In activeModuleIds
                For Each actModule As [Lib].Schema.Kbl.[Module] In _activeMapper.GetModules.Where(Function([module]) [module].SystemId = moduleId)
                    activeModulePartNumbers.TryAdd(actModule.Part_number)
                Next
            Next

            activeModulePartNumbers.Sort()

            Dim compareModuleIds As List(Of String) = _compareHarnessConfig.Modules.SplitSpace.ToList
            Dim compareModulePartNumbers As New List(Of String)

            For Each moduleId As String In compareModuleIds
                For Each compModule As [Lib].Schema.Kbl.[Module] In _compareMapper.GetModules.Where(Function([module]) [module].SystemId = moduleId)
                    compareModulePartNumbers.TryAdd(compModule.Part_number)
                Next
            Next

            compareModulePartNumbers.Sort()

            Dim activeModules As New StringBuilder
            Dim compareModules As New StringBuilder

            For Each activeModule As String In activeModulePartNumbers
                activeModules.Append(String.Format("{0}|", activeModule))
            Next

            For Each compareModule As String In compareModulePartNumbers
                compareModules.Append(String.Format("{0}|", compareModule))
            Next

            If (_compositeCompare) Then
                Me.lblConfigStatus.Visible = False
            Else
                Me.lblConfigStatus.Visible = True
                If (activeModules.ToString = compareModules.ToString) Then
                    Me.lblConfigStatus.Appearance.ForeColor = Color.Green
                    Me.lblConfigStatus.Text = CompareFormStrings.MatchingConfig_Label
                    Me.picConfigStatus.Image = My.Resources.MatchingConfig.ToBitmap
                Else
                    Me.lblConfigStatus.Appearance.ForeColor = Color.Orange
                    Me.lblConfigStatus.Text = CompareFormStrings.MismatchingConfig_Label
                    Me.picConfigStatus.Image = My.Resources.MismatchingConfig.ToBitmap
                End If
            End If
        End If
    End Sub

    Private Function GetActiveObjectsAndOrUpdateActiveModulesCollection(activeModuleIds As ICollection(Of String), kblMapper As KblMapper, moduleConfiguration As String, referenceOrCompare As String) As ICollection(Of String)
        Dim activeObjectIds As ICollection(Of String) = New List(Of String)
        Dim multipleObjects As New Dictionary(Of String, String)

        If (kblMapper.GetModules Is Nothing) OrElse (Not kblMapper.GetModules.Any()) OrElse (moduleConfiguration = CompareFormStrings.EntireHarness_ModConfig) Then
            For Each occurrenceObject As KeyValuePair(Of String, IKblOccurrence) In kblMapper.KBLOccurrenceMapper
                AddOccurrenceObjectToList(activeObjectIds, False, occurrenceObject.Key, kblMapper, multipleObjects, CType(occurrenceObject.Value, IKblBaseObject))
            Next
        Else
            For Each [module] As [Lib].Schema.Kbl.[Module] In kblMapper.GetModules
                Dim moduleObjects As IGroupingKblObjects = Nothing
                If (activeModuleIds.Contains([module].SystemId)) AndAlso (kblMapper.KBLModuleObjectMapper.TryGetValue([module].SystemId, moduleObjects)) Then
                    For Each kblObject As IKblBaseObject In moduleObjects.Cast(Of IKblBaseObject)
                        AddOccurrenceObjectToList(activeObjectIds, True, kblObject.SystemId, kblMapper, multipleObjects, kblObject)
                    Next
                End If
            Next
        End If

        If (kblMapper.GetApprovals IsNot Nothing) Then
            For Each approval As Approval In kblMapper.GetApprovals
                If (approval.Name IsNot Nothing) Then
                    AddOccurrenceObjectToList(activeObjectIds, kblMapper.GetModules IsNot Nothing OrElse kblMapper.GetModules.Any() OrElse moduleConfiguration <> CompareFormStrings.EntireHarness_ModConfig, approval.SystemId, kblMapper, multipleObjects, approval)
                End If
            Next
        End If

        If (kblMapper.GetChangeDescriptions IsNot Nothing) Then
            For Each changeDescription As Change_description In kblMapper.GetChangeDescriptions
                If (changeDescription.Id IsNot Nothing) Then
                    AddOccurrenceObjectToList(activeObjectIds, kblMapper.GetModules IsNot Nothing OrElse kblMapper.GetModules.Any() OrElse moduleConfiguration <> CompareFormStrings.EntireHarness_ModConfig, changeDescription.SystemId, kblMapper, multipleObjects, changeDescription)
                End If
            Next
        End If

        If (kblMapper.GetDefaultDimensionSpecifications IsNot Nothing) Then
            For Each defDimSpec As Default_dimension_specification In kblMapper.GetDefaultDimensionSpecifications
                If (defDimSpec.Dimension_value_range IsNot Nothing) Then
                    AddOccurrenceObjectToList(activeObjectIds, kblMapper.GetModules IsNot Nothing OrElse kblMapper.GetModules.Any() OrElse moduleConfiguration <> CompareFormStrings.EntireHarness_ModConfig, defDimSpec.SystemId, kblMapper, multipleObjects, defDimSpec)
                End If
            Next
        End If

        If (kblMapper.GetDimensionSpecifications IsNot Nothing) Then
            For Each dimSpec As Dimension_specification In kblMapper.GetDimensionSpecifications
                If (dimSpec.Id IsNot Nothing) Then
                    AddOccurrenceObjectToList(activeObjectIds, kblMapper.GetModules IsNot Nothing OrElse kblMapper.GetModules.Any() OrElse moduleConfiguration <> CompareFormStrings.EntireHarness_ModConfig, dimSpec.SystemId, kblMapper, multipleObjects, dimSpec)
                End If
            Next
        End If

        If (multipleObjects.Count <> 0) AndAlso (_showWarningMessage) Then
            _showWarningMessage = False

            For Each multipleObject As String In multipleObjects.Values
                RaiseEvent LogMessage(Me, New LogEventArgs With {.LogLevel = LogEventArgs.LoggingLevel.Warning, .LogMessage = String.Format(CompareFormStrings.MultipleObjectsWithIdenticalIdDetected_Msg, referenceOrCompare, multipleObject)})
            Next

            'TODO: showing this message here is a bad/lazy implementation and in addition it's shown within a Worker-Thread accessing the main-thread -> added basic workaround with invoke but this BL should be moved outside -> and just raising an event or calling a method which does the checks
            If (Me.bwCompare.IsBusy) Then
                If Me.InvokeOrDefault(Function() MessageBoxEx.ShowWarning(Me, String.Format(CompareFormStrings.MultipleObjectsWithSameIdDetected_Msg, referenceOrCompare, vbCrLf), MessageBoxButtons.YesNo)) = System.Windows.Forms.DialogResult.Yes Then
                    Return activeObjectIds
                End If
            End If
            Return Nothing
        Else
            Return activeObjectIds
        End If
    End Function

    Private Sub GetCompareDocument(fileName As String, deleteFile As Boolean, containerFileName As String)
        Using xmlStream As New FileStream(fileName, FileMode.Open, FileAccess.Read)
            Using xmlReader As Xml.XmlReader = Xml.XmlReader.Create(xmlStream)
                Dim kbl As KBL_container = KBL_container.Read(xmlReader)
                If kbl IsNot Nothing Then
                    _compareMapper = New KblMapper(kbl)
                End If
                If _compareMapper IsNot Nothing Then
                    With _compareMapper
                        .ReBuild()
                    End With
                End If
            End Using
        End Using

        If (deleteFile) Then
            File.Delete(fileName)
        End If

        Dim valueListItem As New Infragistics.Win.ValueListItem(_compareMapper, Path.GetFileNameWithoutExtension(containerFileName))
        Me.uceCompareDocument.Items.Add(valueListItem)
        Me.uceCompareDocument.SelectedItem = valueListItem
        CompareDocument = Nothing
    End Sub

    Private Sub Initialize(activeDocument As IDocumentForm, ParamArray compareDocuments As IDocumentForm())
        Me.BackColor = Color.White
        Me.Icon = My.Resources.CompareData
        Me.Text = CompareFormStrings.Caption

        Me.btnCompare.Enabled = False
        Me.btnExport.Enabled = False
        Me.lblLegend1.Visible = False
        Me.lblLegend2.Visible = False
        Me.lblLegend3.Visible = False
        Me.txtReferenceDocument.Text = activeDocument.TextResolved

        Me.txtComment.ButtonsRight("btnEdit").Appearance.Image = My.Resources.Visibility.ToBitmap

        Me.uceReferenceConfig.Items.Add(Nothing, CompareFormStrings.EntireHarness_ModConfig)

        For Each harnessModuleConfig As HarnessModuleConfiguration In activeDocument.HarnessModulConfigurations
            If (Regex.Replace(harnessModuleConfig.HarnessConfiguration.Part_number, "\s", String.Empty) <> String.Empty) Then
                Me.uceReferenceConfig.Items.Add(harnessModuleConfig.HarnessConfiguration, harnessModuleConfig.HarnessConfiguration.Part_number)
            ElseIf (Regex.Replace(harnessModuleConfig.HarnessConfiguration.Abbreviation, "\s", String.Empty) <> String.Empty) Then
                Me.uceReferenceConfig.Items.Add(harnessModuleConfig.HarnessConfiguration, harnessModuleConfig.HarnessConfiguration.Abbreviation)
            ElseIf (Regex.Replace(harnessModuleConfig.HarnessConfiguration.Description, "\s", String.Empty) <> String.Empty) Then
                Me.uceReferenceConfig.Items.Add(harnessModuleConfig.HarnessConfiguration, harnessModuleConfig.HarnessConfiguration.Description)
            End If
        Next

        Me.uceReferenceConfig.SelectedItem = Me.uceReferenceConfig.Items(0)

        For Each compareDocument As IDocumentForm In compareDocuments
            Me.uceCompareDocument.Items.Add(compareDocument, compareDocument.TextResolved)
        Next

        Me.uceCompareDocument.SortStyle = Infragistics.Win.ValueListSortStyle.Ascending
        Me.upbCompare.Visible = False

        If compareDocuments.Length = 1 Then
            Me.uceCompareDocument.SelectedItem = Me.uceCompareDocument.Items(0)
        End If

        Dim toolTipInfo As UltraToolTipInfo = Me.uttmCompare.GetUltraToolTip(Me.txtReferenceDocument)
        toolTipInfo.ToolTipText = activeDocument.TextResolved
    End Sub

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Me.Close()
    End Sub

    Private Sub btnCompare_Click(sender As Object, e As EventArgs) Handles btnCompare.Click
        Compare()
    End Sub

    Private Sub Compare()
        If _compareRunning Then
            _compareRunning = False
            Me.btnClose.Enabled = True
            Me.btnCompare.Text = CompareFormStrings.Compare_Text
            Me.bwCompare.CancelAsync()
        ElseIf (_activeMapper IsNot Nothing) Then
            If (Not _harnessPartNumberMismatch) AndAlso (_isDirty) Then
                If (Not SaveCheckedCompareResultInformation()) Then
                    Return
                End If
            End If

            Dim dialog_result As DialogResult
            If (_activeMapper.HarnessPartNumber <> String.Empty AndAlso _compareMapper.HarnessPartNumber <> String.Empty AndAlso _activeMapper.HarnessPartNumber = _compareMapper.HarnessPartNumber) Then
                dialog_result = System.Windows.Forms.DialogResult.Yes
                _harnessPartNumberMismatch = False
            ElseIf (_activeMapper.HarnessPartNumber = String.Empty AndAlso _compareMapper.HarnessPartNumber = String.Empty AndAlso _activeMapper.GetHarness.Abbreviation = _compareMapper.GetHarness.Abbreviation) Then
                dialog_result = System.Windows.Forms.DialogResult.Yes
                _harnessPartNumberMismatch = False
            Else
                _harnessPartNumberMismatch = True
                dialog_result = MessageBoxEx.ShowQuestion(Me, CompareFormStrings.DifferentHarnessPartNumbers_Msg)
            End If

            If (dialog_result = System.Windows.Forms.DialogResult.Yes) Then
                Me.Cursor = Cursors.WaitCursor

                _compareRunning = True

                Me.btnClose.Enabled = False
                Me.btnCompare.Text = CompareFormStrings.Cancel_Text

                _compareHub = New CompareInformationHub(_generalSettings, Me)
                _compareHub.Dock = DockStyle.Fill
                _compareHub.Visible = False

                If (Me.ugbCompareResults.Controls.Count <> 0) Then
                    Me.ugbCompareResults.Controls.RemoveAt(0)
                End If

                Me.lblLegend1.Visible = False
                Me.lblLegend2.Visible = False
                Me.lblLegend3.Visible = False

                Me.ugbCompareResults.Controls.Add(_compareHub)
                Me.upbCompare.Visible = True
                Me.upbCompare.Value = 0

                Me.bwCompare.RunWorkerAsync()
            End If
        End If
    End Sub

    Private Sub btnExport_Click(sender As Object, e As EventArgs) Handles btnExport.Click
        Using sfdExcel As New SaveFileDialog
            With sfdExcel
                .DefaultExt = KnownFile.XLSX.Trim("."c)

                If (_activeMapper.GetChanges.Any()) Then
                    .FileName = String.Format("{0}{1}{2}_{3}_{4}_Compare_Result.xlsx", Now.Year, Format(Now.Month, "00"), Format(Now.Day, "00"), Replace(_activeMapper.HarnessPartNumber, " ", String.Empty), _activeMapper.GetChanges.Max(Function(change) change.Id))
                Else
                    .FileName = String.Format("{0}{1}{2}_{3}_Compare_Result.xlsx", Now.Year, Format(Now.Month, "00"), Format(Now.Day, "00"), Replace(_activeMapper.HarnessPartNumber, " ", String.Empty))
                End If

                .Filter = "Excel files (*.xlsx)|*.xlsx|Excel files (97-2003) (*.xls)|*.xls"
                .Title = CompareFormStrings.ExportExcelFile_Title

                If (.ShowDialog(Me) = DialogResult.OK) Then
                    Using Me.EnableWaitCursor
                        Me.lblLegend1.Visible = False
                        Me.lblLegend2.Visible = False
                        Me.lblLegend3.Visible = False

                        Me.upbCompare.Visible = True
                        Me.upbCompare.Value = 0

                        _compareHub.ExportAllToExcel(.FileName, Me.upbCompare, TryCast(_differencesDialog.ColumnViewBindingSource.List, IEnumerable(Of ColumnView)))

                        Me.upbCompare.Visible = False

                        Me.lblLegend1.Visible = True
                        Me.lblLegend2.Visible = True
                        Me.lblLegend3.Visible = True
                    End Using
                End If
            End With
        End Using
    End Sub

    Private Sub btnSelectDocument_Click(sender As Object, e As EventArgs) Handles btnSelectDocument.Click
        Using ofdDocument As New OpenFileDialog
            ofdDocument.DefaultExt = KnownFile.HCV.Trim("."c)
            ofdDocument.FileName = String.Empty
            ofdDocument.Filter = "HCV files (*.hcv)|*.hcv|KBL files (*.kbl)|*.kbl"
            ofdDocument.Title = CompareFormStrings.SelectHcvOrKblFile_Title

            If (ofdDocument.ShowDialog(Me) = DialogResult.OK) Then
                Using Me.EnableWaitCursor
                    Try
                        If KnownFile.IsKbl(ofdDocument.FileName) Then
                            GetCompareDocument(ofdDocument.FileName, False, ofdDocument.FileName)
                        Else
                            Dim temporaryFolder As String = IO.Directory.CreateDirectory(IO.Path.Combine(IO.Path.GetTempPath, Guid.NewGuid.ToString)).FullName
                            Using zipArchive As ZipArchive = ZipFile.OpenRead(ofdDocument.FileName)
                                For Each zipArchiveEntry As ZipArchiveEntry In zipArchive.Entries
                                    If KnownFile.IsKbl(zipArchiveEntry.FullName) Then
                                        zipArchiveEntry.ExtractToFile(Path.Combine(temporaryFolder, zipArchiveEntry.FullName))
                                        GetCompareDocument(Path.Combine(temporaryFolder, zipArchiveEntry.FullName), True, ofdDocument.FileName)
                                        Exit For
                                    End If
                                Next
                            End Using

                            IO.Directory.Delete(temporaryFolder, True)
                        End If
                        _reloadedDocuments.Add(Me.uceCompareDocument.SelectedItem)
                        If (CType(_activeDocument.HcvDocument, HcvDocument)?.Entities.Count > 0) IsNot Nothing Then
                            MessageBoxEx.ShowWarning(CompareFormStrings.No3DAvailable_Msg)
                        End If
                    Catch ex As Exception
                        MessageBoxEx.ShowError(Me, String.Format(CompareFormStrings.ErrorExtractFile_Msg, vbCrLf, ex.Message))
                    End Try
                End Using
            End If
        End Using
    End Sub

    Private Sub bwCompare_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles bwCompare.DoWork
        If (CompareDocuments() IsNot Nothing) Then
            'HINT: when _compareMapper is set then the Diff-Column will be visible

            Dim progressWrapped As Action(Of Integer) =
                Sub(p As Integer)
                    Try
                        If bwCompare.IsBusy Then
                            bwCompare.ReportProgress(p)
                        End If
                    Catch ex As Exception
                        Debug.WriteLine(ex.Message)
                    End Try
                End Sub     ' HINT: this is a temp solution to pass throu the progress to bwCompare until refactoring 
            _compareHub.InitializeDataSources(_activeMapper, New QualityStamping.QMStamps, New RedliningInformation, progressWrapped, _comparisonMapperList, _compareMapper)
        Else
            e.Cancel = True
        End If
    End Sub

    Private Sub bwCompare_ProgressChanged(sender As Object, e As System.ComponentModel.ProgressChangedEventArgs) Handles bwCompare.ProgressChanged
        Me.upbCompare.Value = e.ProgressPercentage
    End Sub

    Private Sub bwCompare_RunWorkerCompleted(sender As Object, e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles bwCompare.RunWorkerCompleted
        _rowsUnChecked.Clear()
        _rowsChecked.Clear()

        If (e.Error IsNot Nothing) Then
            MessageBoxEx.ShowError(Me, String.Format(CompareFormStrings.ErrorCompare_Msg, e.Error.Message))
        End If

        If (Not e.Cancelled) AndAlso (e.Error Is Nothing) Then
            If (_compareHub.InitializeGrids()) Then
                If (_compositeCompare) Then
                    _compareHub.utcInformationHub.Tabs("tabHarness").Visible = False
                End If
                _compareHub.Visible = True
            End If

            If IsD3D Then
                Show3D()
            End If
        End If

        _compareRunning = False
        UpdateBtnNextAndPrevious()
        txtComment.Enabled = _compareHub.ActiveGrid?.ActiveRow IsNot Nothing
        Me.btnClose.Enabled = True
        Me.btnCompare.Enabled = False
        Me.btnCompare.Text = CompareFormStrings.Compare_Text
        Me.btnExport.Enabled = True

        Me.lblLegend1.Visible = True
        Me.lblLegend2.Visible = True
        Me.lblLegend3.Visible = True
        Me.upbCompare.Visible = False

        Me.Cursor = Cursors.Default
        OnCompareFinished(New EventArgs)
    End Sub

    Protected Overridable Sub OnCompareFinished(e As EventArgs)
        If IsD3D Then
            SplitContainer1.Panel1Collapsed = False
            SplitContainer1.SplitterDistance = CInt(0.5 * SplitContainer1.Height)
        Else
            SplitContainer1.Panel1Collapsed = True
            SplitContainer1.Panel2Collapsed = False
        End If
        RaiseEvent CompareFinished(Me, e)
    End Sub

    Private Sub _compareHub_HubSelectionChanged(sender As InformationHub, e As InformationHubEventArgs) Handles _compareHub.HubSelectionChanged
        If Not _3DSelection Then
            Dim ev As InformationHubEventArgs = CType(e.Clone(), InformationHubEventArgs)
            If _d3dCntrl IsNot Nothing Then
                If _d3dCntrl.GetChangeType() = CompareChangeType.Modified AndAlso _d3dCntrl.GetCurrentKBLMapper Is CompareDocument.Kbl AndAlso e.ObjectIds.Count > 0 Then
                    ev.ObjectIds.Clear()
                    Dim id As String

                    If e.ObjectType = KblObjectType.Net Then
                        Dim myWireIds As List(Of String) = _d3dCntrl.GetWireIds(e.ObjectIds.ToList, e.KblMapperSourceId)
                        If myWireIds.Count > 0 Then
                            For Each id In _d3dCntrl.GetModifiedCompareId(myWireIds.First, KblObjectType.Net.ToString)
                                ev.ObjectIds.Add(id)
                            Next
                        End If
                    Else
                        Dim ids As List(Of String) = _d3dCntrl.GetModifiedCompareId(e.ObjectIds.First, e.ObjectType.ToString)
                        If ids.Count > 0 Then
                            ev.ObjectIds.Add(ids.First)
                        End If
                    End If
                    ev.KblMapperSourceId = _d3dCntrl.CurrentMapperId
                End If
            End If
            RaiseEvent CompareHubSelectionChanged(sender, ev)
        End If
    End Sub

    Private Sub txtReferenceDocument_BeforeEnterEditMode(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles txtReferenceDocument.BeforeEnterEditMode
        e.Cancel = True
    End Sub

    Private Sub uceCompareConfig_SelectionChanged(sender As Object, e As EventArgs) Handles uceCompareConfig.SelectionChanged
        If (Me.uceCompareConfig.SelectedItem IsNot Nothing) Then
            _compareHarnessConfig = TryCast(Me.uceCompareConfig.SelectedItem.DataValue, Harness_configuration)

            CompareHarnessModuleConfigurations()

            If (_activeMapper IsNot Nothing) Then
                Me.btnCompare.Enabled = True
                Clear3DComparer()
            End If
        End If
    End Sub

    Private Sub uceCompareDocument_SelectionChanged(sender As Object, e As EventArgs) Handles uceCompareDocument.SelectionChanged
        If (Me.uceCompareDocument.SelectedItem IsNot Nothing) AndAlso (Me.uceCompareDocument.SelectedItem.DataValue IsNot Nothing) Then

            If _reloadedDocuments.Contains(Me.uceCompareDocument.SelectedItem) Then
                MessageBoxEx.ShowWarning(CompareFormStrings.No3DAvailable_Msg)
            End If

            Me.uceCompareConfig.Items.Clear()
            Me.uceCompareConfig.Items.Add(Nothing, CompareFormStrings.EntireHarness_ModConfig)

            If (TypeOf Me.uceCompareDocument.SelectedItem.DataValue Is IDocumentForm) Then
                CompareDocument = DirectCast(Me.uceCompareDocument.SelectedItem.DataValue, IDocumentForm)
                _compareMapper = CType(DirectCast(Me.uceCompareDocument.SelectedItem.DataValue, IDocumentForm).Kbl, KblMapper)

                For Each harnessModuleConfig As HarnessModuleConfiguration In DirectCast(Me.uceCompareDocument.SelectedItem.DataValue, IDocumentForm).HarnessModulConfigurations
                    If (Regex.Replace(harnessModuleConfig.HarnessConfiguration.Part_number, "\s", String.Empty) <> String.Empty) Then
                        Me.uceCompareConfig.Items.Add(harnessModuleConfig.HarnessConfiguration, harnessModuleConfig.HarnessConfiguration.Part_number)
                    ElseIf (Regex.Replace(harnessModuleConfig.HarnessConfiguration.Abbreviation, "\s", String.Empty) <> String.Empty) Then
                        Me.uceCompareConfig.Items.Add(harnessModuleConfig.HarnessConfiguration, harnessModuleConfig.HarnessConfiguration.Abbreviation)
                    ElseIf (Regex.Replace(harnessModuleConfig.HarnessConfiguration.Description, "\s", String.Empty) <> String.Empty) Then
                        Me.uceCompareConfig.Items.Add(harnessModuleConfig.HarnessConfiguration, harnessModuleConfig.HarnessConfiguration.Description)
                    End If
                Next
            Else
                _compareMapper = CType(DirectCast(Me.uceCompareDocument.SelectedItem.DataValue, IDocumentForm).Kbl, KblMapper)
                CompareDocument = Nothing

                If (_compareMapper.GetHarnessConfigurations IsNot Nothing) Then
                    For Each harnessConfig As Harness_configuration In _compareMapper.GetHarnessConfigurations
                        If (Regex.Replace(harnessConfig.Part_number, "\s", String.Empty) <> String.Empty) Then
                            Me.uceCompareConfig.Items.Add(harnessConfig, harnessConfig.Part_number)
                        ElseIf (Regex.Replace(harnessConfig.Abbreviation, "\s", String.Empty) <> String.Empty) Then
                            Me.uceCompareConfig.Items.Add(harnessConfig, harnessConfig.Abbreviation)
                        ElseIf (Regex.Replace(harnessConfig.Description, "\s", String.Empty) <> String.Empty) Then
                            Me.uceCompareConfig.Items.Add(harnessConfig, harnessConfig.Description)
                        End If
                    Next
                End If
            End If

            Me.lblConfigStatus.Text = String.Empty
            Me.btnCompare.Enabled = True
            Me.picConfigStatus.Image = Nothing
            Me.uceCompareConfig.SelectedItem = Me.uceCompareConfig.Items(0)

            Dim toolTipInfo As UltraToolTipInfo = Me.uttmCompare.GetUltraToolTip(Me.uceCompareDocument)
            toolTipInfo.ToolTipText = Me.uceCompareDocument.SelectedItem.DisplayText
            Clear3DComparer()
        End If
    End Sub

    Private Sub Clear3DComparer()
        If _d3dCntrl IsNot Nothing Then
            If (Me.ugbCompareResults.Controls.Count <> 0) Then
                Me.ugbCompareResults.Controls.RemoveAt(0)
            End If

            If SplitContainer1.Panel1.Controls.Contains(_d3dCntrl) Then
                SplitContainer1.Panel1.Controls.Remove(_d3dCntrl)
                SplitContainer1.Panel1.Refresh()
            End If

            _d3dCntrl.Dispose()
        End If
    End Sub

    Private Sub uceReferenceConfig_SelectionChanged(sender As Object, e As EventArgs) Handles uceReferenceConfig.SelectionChanged
        If (Me.uceReferenceConfig.SelectedItem IsNot Nothing) Then
            _activeHarnessConfig = TryCast(Me.uceReferenceConfig.SelectedItem.DataValue, Harness_configuration)

            CompareHarnessModuleConfigurations()

            If (_compareMapper IsNot Nothing) Then
                Me.btnCompare.Enabled = True
                Clear3DComparer()
            End If
        End If
    End Sub

    Public ReadOnly Property CurrentActiveObjects As ICollection(Of String)
        Get
            Return _currentActiveObjects
        End Get
    End Property

    Private Sub btnDiffsInformationImage_Click(sender As Object, e As EventArgs) Handles btnDiffsInformationImage.Click
        _differencesDialog.ShowDialog(Me)
    End Sub

    Private Function GetComparableDifferencesData() As BindingList(Of ColumnView)
        Dim bl As New BindingList(Of ColumnView)

        For Each ga As GridAppearance In GridAppearance.All
            If ga.DefaultGridTable IsNot Nothing Then
                Dim diffResult As GridTableDiffer.DifferencesCollection = ga.GridTable.DiffWith(ga.DefaultGridTable, "Comparable")
                bl.AddRange(CreateDifferencesData(diffResult))
            End If
        Next

        Return bl
    End Function

    Private Function CreateDifferencesData(diffResult As GridTableDiffer.DifferencesCollection, Optional fullTableName As String = "") As List(Of ColumnView)
        Dim list As New List(Of ColumnView)
        If diffResult.AnyDifferentOrTableMiss Then
            If String.IsNullOrEmpty(fullTableName) Then
                fullTableName = diffResult.TableType.ToString
            End If

            If diffResult.SubDifferences IsNot Nothing Then
                list.AddRange(CreateDifferencesData(diffResult.SubDifferences, fullTableName + "\" + diffResult.SubDifferences.TableType.ToString))
            End If

            If diffResult.ColumnDifferences.Length > 0 Then
                Dim column As New ColumnView With {.ColumnName = fullTableName}
                list.Add(column)
                For Each diffCol As GridTableDiffer.ColumnDifference In diffResult.ColumnDifferences
                    Dim diffProp As GridTableDiffer.PropertyDifference = diffCol.SingleOrDefault 'HINT: should always be only one property (Comparable, because we are only compared the "Comparable"-Property between the columns)
                    If diffProp IsNot Nothing Then
                        Dim propView As New PropertyView With {.Name = CStr(diffCol.GetColumn1Or2PropValue("Name")), .Current = CBool(diffProp.Value1), .[Default] = CBool(diffProp.Value2)}
                        column.Properties.Add(propView)
                    End If
                Next
            End If
        End If
        Return list
    End Function

    Private Sub CompareForm_Shown(sender As Object, e As EventArgs) Handles Me.Shown ' HINT: loading the data within the shown-even to have the messagebox (when error happens) associated to this form
        If _dataErrorException IsNot Nothing Then
            MessageBoxEx.Show(Me, String.Format(ErrorStrings.CompareForm_UnExpectedErrorGettingDifferences, _dataErrorException.Message), [Shared].MSG_BOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
        UpdateBtnNextAndPrevious()
    End Sub

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        If Me.CurrentResultEntries IsNot Nothing AndAlso ShowSaveCompareResultDialog() = DialogResult.Yes Then
            SaveCheckedCompareResultInformation()
        End If
    End Sub

    Private Function ShowSaveCompareResultDialog(Optional buttons As MessageBoxButtons = MessageBoxButtons.YesNo) As DialogResult
        Return MessageBoxEx.ShowQuestion(Me, GraphicalCompareFormStrings.SaveCompareResult_Msg, buttons, Nothing)
    End Function

    Private Function GetCheckedValue(row As UltraGridRow) As Nullable(Of Boolean)
        If row.Cells.Exists(NameOf(GraphicalCompareFormStrings.Checked_ColumnCaption)) Then
            Return GetCheckedValue(CType(row.ListObject, UltraDataRow))
        End If
        Return Nothing
    End Function

    Private Function GetCheckedValue(row As UltraDataRow) As Nullable(Of Boolean)
        Dim checkedValue As Object = row.GetCellValue(NameOf(GraphicalCompareFormStrings.Checked_ColumnCaption))
        If Not IsDBNull(checkedValue) AndAlso checkedValue IsNot Nothing Then
            Return CBool(checkedValue)
        End If
    End Function

    Private Function SaveCheckedCompareResultInformation() As Boolean   ' HINT: transport current grid information to CurrentResultEntries-List and call the save-method for CurrentResultEntries
        If Me.CurrentResultEntries Is Nothing Then
            Return False
        End If

        Dim identicalCompareResultEntriesExists As Boolean = False
        'Me.CurrentResultEntries.Clear() 
        ' HINT/WARN: do not clear this collection before the check through all rows is done! 
        ' The GetCellDataValue -event of the grid want's to retrieve (under unknown circumstances?) data and tries to access this collection by calling the grid.rows-getter. -> As an Result we will get all row's false!!

        For Each kv As KeyValuePair(Of String, UltraDataRow) In _rowsChecked
            Dim systemId As String = GetSystemId(kv.Value)
            If systemId <> String.Empty Then
                Dim comment As String = If(IsDBNull(kv.Value.GetCellValue(InformationHub.CompareCommentColumnKey)), String.Empty, kv.Value.GetCellValue(InformationHub.CompareCommentColumnKey).ToString)
                Dim toBeChanged As Boolean = If(IsDBNull(kv.Value.GetCellValue(NameOf(GraphicalCompareFormStrings.ToBeChanged_ColumnCaption))), False, CBool(kv.Value.GetCellValue(NameOf(GraphicalCompareFormStrings.ToBeChanged_ColumnCaption))))
                Me.CurrentResultEntries.SetOrAddNew(systemId, comment, toBeChanged)
            Else
                TryCast(_activeDocument, DocumentForm)?._logHub.WriteLogMessage(New LogEventArgs With {.LogLevel = LogEventArgs.LoggingLevel.Warning, .LogMessage = String.Format(GraphicalCompareFormStrings.CannotSaveResult_Msg, kv.Value.GetCellValue(NameOf(GraphicalCompareFormStrings.ObjName_ColumnCaption)).ToString, kv.Value.GetCellValue(NameOf(GraphicalCompareFormStrings.ObjType_ColumnCaption)).ToString)})
                identicalCompareResultEntriesExists = True
            End If
        Next

        For Each kv As KeyValuePair(Of String, UltraDataRow) In _rowsUnChecked
            Me.CurrentResultEntries.Remove(kv.Key)
        Next

        Dim saveError As String = SaveCheckedCompareResultInformationCore()
        If saveError <> String.Empty Then
            MessageBoxEx.ShowError(String.Format(GraphicalCompareFormStrings.ErrorSaveResult_Msg, vbCrLf, saveError))
            Return False
        ElseIf (identicalCompareResultEntriesExists) Then
            MessageBoxEx.ShowError(String.Format(GraphicalCompareFormStrings.SaveWarning1_Msg, vbCrLf, GraphicalCompareFormStrings.SaveWarning2_Msg))
        End If

        _rowsUnChecked.Clear()
        _rowsChecked.Clear()

        IsDirty = False

        Return True
    End Function

    Private Function SaveCheckedCompareResultInformationCore() As String
        Using Me.EnableWaitCursor
            Dim active_old_gcri As [Lib].IO.Files.Hcv.GraphicalCheckedCompareResultInfoContainerFile = CType(_activeDocument.HcvFile, HcvFile).GCRI
            Dim active_old_tcri As [Lib].IO.Files.Hcv.TechnicalCheckedCompareResultInfoContainerFile = CType(_activeDocument.HcvFile, HcvFile).TCRI

            Try
                CType(_activeDocument.HcvFile, HcvFile).TCRI = CType(_activeDocument.GetTechnicallCompareResultInfoFileContainer, TechnicalCheckedCompareResultInfoContainerFile)
                CType(_activeDocument.HcvFile, HcvFile).GCRI = CType(_activeDocument.GetGraphicalCompareResultInfoFileContainer, GraphicalCheckedCompareResultInfoContainerFile)

                If Not CType(_activeDocument.HcvFile, HcvFile).IsXhcvChild Then
                    CType(_activeDocument.HcvFile, HcvFile).Save(useTempIntermediateFile:=True) ' hint use temp intermediate to avoid corrupting zip archives while saving them -> when temp files is used the current file will only be overwritten when saving was successfull
                End If
            Catch ex As Exception
                CType(_activeDocument.HcvFile, HcvFile).GCRI = active_old_gcri ' HINT: restore backup when something failed and original hcv does not exist!
                CType(_activeDocument.HcvFile, HcvFile).TCRI = active_old_tcri ' HINT: restore backup when something failed and original hcv does not exist!
#If DEBUG Or CONFIG = "Debug" Then
                Throw
#Else
                    Return ex.Message
#End If
            End Try
        End Using
        Return String.Empty
    End Function

    Private Sub CompareForm_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim data As New BindingList(Of ColumnView)
        Try
            data = GetComparableDifferencesData()
            _differencesDialog.ColumnViewBindingSource.DataSource = Nothing
            _differencesDialog.ColumnViewBindingSource.DataSource = data
        Catch ex As Exception
            _dataErrorException = ex
        End Try

        btnDiffsInformationImage.Visible = data.Count > 0
        If data.Any(Function(d) d.Properties.Any(Function(p) p.Default = True AndAlso p.Current = False)) Then
            btnDiffsInformationImage.Appearance.Image = My.Resources.Warning
        Else
            btnDiffsInformationImage.Appearance.Image = My.Resources.information
        End If
    End Sub

    Private Sub _compareHub_UnhandledCellDataRequested(sender As Object, e As CellDataRequestedEventArgs) Handles _compareHub.UnhandledCellDataRequested
        Select Case e.Column.Key
            Case NameOf(InformationHubStrings.ToBeChanged_ColumnCaption), NameOf(InformationHubStrings.Checked_ColumnCaption), InformationHub.CompareCommentColumnKey
                Dim entry As CheckedCompareResultEntry = GetCompareEntry(e.Row)
                Select Case e.Column.Key
                    Case NameOf(InformationHubStrings.ToBeChanged_ColumnCaption), NameOf(InformationHubStrings.Checked_ColumnCaption)
                        If entry IsNot Nothing Then
                            Select Case e.Column.Key
                                Case NameOf(InformationHubStrings.ToBeChanged_ColumnCaption)
                                    e.Data = entry.ToBeChanged
                                    Return
                                Case NameOf(InformationHubStrings.Checked_ColumnCaption)
                                    e.Data = True
                                    Return
                            End Select
                        Else
                            e.Data = False
                            Return
                        End If
                    Case InformationHub.CompareCommentColumnKey
                        e.Data = entry?.Comment
                        Return
                End Select
        End Select
    End Sub

    Private Function GetOrAddCompareEntry(row As UltraGridRow, comment As String, toBeChanged As Boolean) As CheckedCompareResultEntry
        Return GetOrAddCompareEntry(row, comment, toBeChanged)
    End Function

    Private Function GetCompareEntry(row As UltraGridRow) As CheckedCompareResultEntry
        Return GetCompareEntry(CType(row.ListObject, UltraDataRow))
    End Function

    Private Function GetCompareEntry(row As UltraDataRow) As CheckedCompareResultEntry
        Dim systemId As String = GetSystemId(row)
        If Not String.IsNullOrEmpty(systemId) AndAlso Me.CurrentResultEntries IsNot Nothing Then
            Return Me.CurrentResultEntries.GetEntry(systemId)
        End If
        Return Nothing
    End Function

    Private Sub SetCommentTextInternal(text As String)
        Try
            _internalSetComment = True
            Me.txtComment.Text = text
        Finally
            _internalSetComment = False
        End Try
    End Sub

    Private Sub _compareHub_ClickCell(sender As Object, e As ClickCellEventArgs) Handles _compareHub.ClickCell
        ClickCellCore(e.Cell)
    End Sub

    Private Sub ClickCellCore(cell As UltraGridCell)
        Dim grid As UltraGrid = CType(cell.Column.Band.Layout.Grid, UltraGrid)

        Select Case cell.Column.Key
            Case NameOf(InformationHubStrings.ToBeChanged_ColumnCaption), NameOf(InformationHubStrings.Checked_ColumnCaption)
                Dim row As UltraDataRow = CType(cell.Row.ListObject, UltraDataRow)
                Select Case cell.Column.Key
                    Case NameOf(GraphicalCompareFormStrings.Checked_ColumnCaption)
                        If CBool(cell.Value) Then
                            If (MessageBox.Show(GraphicalCompareFormStrings.Uncheck_Msg, [Shared].MSG_BOX_TITLE, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) = System.Windows.Forms.DialogResult.Yes) Then
                                row.SetCellValue(NameOf(GraphicalCompareFormStrings.Checked_ColumnCaption), False)
                                row.SetCellValue(NameOf(GraphicalCompareFormStrings.ToBeChanged_ColumnCaption), False)
                                row.SetCellValue(InformationHub.CompareCommentColumnKey, String.Empty)
                                SetCommentTextInternal(String.Empty)
                                AddRowUnChecked(row)
                            End If
                        Else
                            row.SetCellValue(NameOf(GraphicalCompareFormStrings.Checked_ColumnCaption), True)
                            AddRowChecked(row)
                        End If
                    Case NameOf(GraphicalCompareFormStrings.ToBeChanged_ColumnCaption)
                        If (CBool(cell.Value)) Then
                            If MessageBoxEx.ShowWarning(GraphicalCompareFormStrings.UncheckToBeChangedFlag_Msg, MessageBoxButtons.YesNo) = System.Windows.Forms.DialogResult.Yes Then
                                row.SetCellValue(NameOf(GraphicalCompareFormStrings.ToBeChanged_ColumnCaption), False)
                                AddRowChecked(row)
                            End If
                        Else
                            row.SetCellValue(NameOf(GraphicalCompareFormStrings.ToBeChanged_ColumnCaption), True)
                            row.SetCellValue(NameOf(GraphicalCompareFormStrings.Checked_ColumnCaption), True)
                            AddRowChecked(row)
                        End If
                End Select

                'HINT: missing deselect ? / currently we can't use selection because InformationHub does not allow this
                'If (Me.ugResults.ActiveRow IsNot Nothing) AndAlso (Not Me.ugResults.ActiveRow.Selected) Then
                '    Me.ugResults.EventManager.AllEventsEnabled = False
                '    Me.ugResults.Selected.Rows.Clear()
                '    Me.ugResults.EventManager.AllEventsEnabled = True

                '    Me.ugResults.Selected.Rows.Add(Me.ugResults.ActiveRow)
                'End If

                If Me.CurrentResultEntries IsNot Nothing Then ' HINT: we have some compare that was called
                    IsDirty = True
                End If
        End Select
        grid.Focus()
    End Sub

    Private Function GetKblOccurrcence(row As UltraDataRow) As IKblBaseObject
        Dim occurrenceObject As IKblBaseObject = TryCast(If(TypeOf row.Tag Is ITuple, CType(row.Tag, ITuple).Item(0), row.Tag), IKblBaseObject)
        Return occurrenceObject
    End Function

    Private Function GetSystemId(row As UltraDataRow) As String
        If TypeOf row.Tag Is String OrElse TypeOf row.Tag Is NetRowInfoContainer Then ' HINT: check for NetRowInfoContainer is normally not needed but to emulate the old busniess logic 100% when the net row information was a string we also check for NetRowInfoContainer -> to be re-evaluated if needed anymore
            Return row.Tag?.ToString
        ElseIf row.Tag IsNot Nothing Then
            Return Reflection.UtilsEx.TryGetPropertyString(row.Tag, E3.HarnessAnalyzer.Shared.SYSTEM_ID)
        End If
        Return String.Empty
    End Function

    Private Function TryGetId(obj As Object) As String
        If obj IsNot Nothing Then
            Try
                Return Reflection.UtilsEx.TryGetPropertyString(obj, "Id")
            Catch ex As Exception
                Return String.Empty
            End Try
        End If
        Return String.Empty
    End Function

    Private Sub AddRowChecked(row As UltraDataRow)
        Dim systemId As String = GetSystemId(row)
        If Not String.IsNullOrEmpty(systemId) Then
            _rowsChecked.TryAdd(systemId, row)
            _rowsUnChecked.Remove(systemId)
        End If
    End Sub

    Private Sub AddRowUnChecked(row As UltraDataRow)
        Dim systemId As String = GetSystemId(row)
        If Not String.IsNullOrEmpty(systemId) Then
            _rowsUnChecked.TryAdd(systemId, row)
            _rowsChecked.Remove(systemId)
        End If
    End Sub

    Private Sub DeselectAll(grid As UltraGrid)
        With grid
            .BeginUpdate()
            .EventManager.AllEventsEnabled = False
            .ActiveRow = Nothing
            .Selected.Rows.Clear()
            .EventManager.AllEventsEnabled = True
            .EndUpdate()
        End With
    End Sub

    Private Sub CompareForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        If Me.bwCompare.IsBusy Then
            Me.bwCompare.CancelAsync()

            e.Cancel = True
        ElseIf (Not _harnessPartNumberMismatch) AndAlso (IsDirty) Then
            Dim result As DialogResult = ShowSaveCompareResultDialog(MessageBoxButtons.YesNoCancel)
            e.Cancel = result = DialogResult.Cancel
            If Not e.Cancel AndAlso result <> DialogResult.No Then
                e.Cancel = Not SaveCheckedCompareResultInformation()
            End If
        End If
    End Sub

    Public ReadOnly Property ActiveDocument As IDocumentForm
        Get
            Return _activeDocument
        End Get
    End Property

    Private ReadOnly Property CheckedResultList As CheckedCompareResultList
        Get
            If _activeDocument IsNot Nothing Then
                Return _activeDocument.GetTechnicalCheckedCompareCompareResultInfo
            End If
            Return New CheckedCompareResultList
        End Get
    End Property

    Private ReadOnly Property CurrentResultEntries As CheckedCompareResultEntryList
        Get
            If _currentCheckedResult IsNot Nothing Then
                Return _currentCheckedResult.CheckedCompareResultEntries
            End If
            Return Nothing
        End Get
    End Property

    Private Property IsDirty As Boolean
        Get
            Return _isDirty
        End Get
        Set(value As Boolean)
            If _isDirty <> value Then
                _isDirty = value
                Me.btnSave.Enabled = _isDirty
            End If
        End Set
    End Property

    Private Sub txtComment_TextChanged(sender As Object, e As EventArgs) Handles txtComment.TextChanged
        If Not _internalSetComment Then
            _compareHub?.ActiveGrid?.ActiveRow?.Cells(NameOf(GraphicalCompareFormStrings.Checked_ColumnCaption)).SetValue(True, False)
            _compareHub?.ActiveGrid?.ActiveRow?.Cells(InformationHub.CompareCommentColumnKey).SetValue(Me.txtComment.Text, False)

            If _compareHub?.ActiveGrid?.ActiveRow IsNot Nothing Then
                AddRowChecked(CType((_compareHub?.ActiveGrid?.ActiveRow.ListObject), UltraDataRow))
            End If
            IsDirty = True
        End If
    End Sub

    Private Sub txtComment_KeyDown(sender As Object, e As KeyEventArgs) Handles txtComment.KeyDown
        If (e.KeyCode = Keys.Enter) Then
            btnNext_Click(sender, e)

            Me.txtComment.Focus()
        End If
    End Sub

    Private Sub txtComment_EditorButtonClick(sender As Object, e As Infragistics.Win.UltraWinEditors.EditorButtonEventArgs) Handles txtComment.EditorButtonClick
        Using editCommentForm As New EditCommentForm(Me.txtComment.Text)
            editCommentForm.ShowDialog(Me)

            Me.txtComment.Text = editCommentForm.txtComment.Text
        End Using
    End Sub

    Private Function PerformPreviousRow() As Boolean
        If _compareHub.ActiveGrid IsNot Nothing Then
            If _compareHub.ActiveGrid.PerformAction(UltraGridAction.PrevRow) Then
                UpdateBtnNextAndPrevious()
                Return True
            End If
        End If
        Return False
    End Function

    Private Function PerformNextRow() As Boolean
        If _compareHub.ActiveGrid IsNot Nothing Then
            If _compareHub.ActiveGrid.PerformAction(UltraGridAction.NextRow) Then
                UpdateBtnNextAndPrevious()
                Return True
            End If
        End If
        Return False
    End Function

    Private Sub btnNext_Click(sender As Object, e As EventArgs) Handles btnNext.Click
        PerformNextRow()
    End Sub

    Private Sub btnPrevious_Click(sender As Object, e As EventArgs) Handles btnPrevious.Click
        PerformPreviousRow()
    End Sub

    Private Sub UpdateBtnNextAndPrevious()
        btnPrevious.Enabled = _compareHub?.ActiveGrid?.ActiveRow IsNot Nothing AndAlso _compareHub.ActiveGrid.ActiveRow.VisibleIndex <> 0
        btnNext.Enabled = _compareHub?.ActiveGrid?.ActiveRow IsNot Nothing AndAlso _compareHub.ActiveGrid.Rows.VisibleRowCount <> _compareHub.ActiveGrid.ActiveRow.VisibleIndex + 1
    End Sub

    Private Sub _compareHub_AfterRowActivate(sender As Object, e As RowEventArgs) Handles _compareHub.AfterRowActivate
        If e.Row?.Cells.Exists(InformationHub.CompareCommentColumnKey) Then
            txtComment.Enabled = e.Row IsNot Nothing
            UpdateBtnNextAndPrevious()

            Dim cellValue As Object = e.Row?.Cells(InformationHub.CompareCommentColumnKey).Value
            If cellValue IsNot Nothing AndAlso Not IsDBNull(cellValue) Then
                SetCommentTextInternal(CStr(cellValue))
            Else
                Me.SetCommentTextInternal(String.Empty)
            End If
        Else
            txtComment.Enabled = False
            btnPrevious.Enabled = False
            btnNext.Enabled = False
        End If
    End Sub

    Private Sub _compareHub_GridKeyDown(sender As Object, e As InformationHub.GridKeyDownEventArgs) Handles _compareHub.GridKeyDown
        If e.KeyEventArgs.KeyCode = Keys.Space AndAlso Not Me.txtComment.Focused AndAlso e.Grid.Selected.Rows.Count <> 0 Then
            With e.Grid
                .BeginUpdate()

                For Each selectedRow As UltraGridRow In .Selected.Rows
                    ClickCellCore(selectedRow.Cells(NameOf(GraphicalCompareFormStrings.Checked_ColumnCaption)))
                Next

                If .Selected.Rows.Count = 1 Then
                    PerformNextRow()
                End If

                .EndUpdate()
            End With
            _isDirty = True
            e.KeyEventArgs.Handled = True

        ElseIf e.KeyEventArgs.KeyCode = Keys.Escape AndAlso e.Grid.Selected.Rows.Count <> 0 Then
            _compareHub.SelectRowsInGrids(New List(Of String), True, True, True, True)
        End If
    End Sub

    Private Sub AddKblObjectToList(kblObject As IKblBaseObject, parentId As String, id As String, changeType As CompareChangeType)
        Dim kblSysId As String = kblObject.SystemId
        Dim kblidref As String = String.Empty
        Dim kblidcomp As String = String.Empty
        Dim modelType As ModelType = ModelType.reference

        Select Case kblObject.ObjectType
            Case KblObjectType.Fixing_occurrence
                With DirectCast(kblObject, Fixing_occurrence)
                    Dim change As ChangedItem = _comparisonMapperList(KblObjectType.Fixing_occurrence).Changes.ByChange(changeType).Where(Function(item) item.Key = .SystemId).FirstOrDefault()
                    kblSysId = .SystemId

                    If changeType = CompareChangeType.New Then
                        If change IsNot Nothing Then
                            Dim changedObject As New ChangedObject(kblObject.ObjectType, kblSysId, change.KblIdRef, change.KblIdComp, changeType, ModelType.compare)
                            changedObject.ID = id
                            AddResult(changedObject)
                        End If
                    Else
                        If change IsNot Nothing Then
                            If changeType = CompareChangeType.Deleted Then
                                Dim changedObject As New ChangedObject(kblObject.ObjectType, kblSysId, change.KblIdRef, change.KblIdComp, changeType, ModelType.reference)
                                changedObject.ID = id
                                AddResult(changedObject)
                            ElseIf changeType = CompareChangeType.Modified Then
                                Dim changedObject As New ChangedObject(kblObject.ObjectType, kblSysId, change.KblIdRef, change.KblIdComp, changeType, ModelType.reference)
                                changedObject.ID = id
                                AddResult(changedObject)
                                changedObject = New ChangedObject(kblObject.ObjectType, kblSysId, change.KblIdRef, change.KblIdComp, changeType, ModelType.compare)
                                changedObject.ID = id
                                AddResult(changedObject)
                            End If
                        End If
                    End If
                End With
            Case KblObjectType.Node
                With DirectCast(kblObject, Node)
                    Dim nd As Node = DirectCast(kblObject, Node)
                    kblSysId = .SystemId
                    Dim change As ChangedItem = Nothing
                    change = _comparisonMapperList(KblObjectType.Node).Changes.ByChange(changeType).Where(Function(item) item.Key = kblSysId).FirstOrDefault()
                    If changeType = CompareChangeType.New Then
                        If change IsNot Nothing Then
                            Dim changedObject As New ChangedObject(kblObject.ObjectType, kblSysId, change.KblIdRef, change.KblIdComp, changeType, ModelType.compare)
                            AddResult(changedObject)
                        End If
                    Else
                        If change IsNot Nothing Then
                            If changeType = CompareChangeType.Deleted Then
                                Dim changedObject As New ChangedObject(kblObject.ObjectType, kblSysId, change.KblIdRef, change.KblIdComp, changeType, ModelType.reference)
                                changedObject.ID = id
                                AddResult(changedObject)
                            ElseIf changeType = CompareChangeType.Modified Then
                                Dim changedObject As New ChangedObject(kblObject.ObjectType, kblSysId, change.KblIdRef, change.KblIdComp, changeType, ModelType.reference)
                                changedObject.ID = id
                                AddResult(changedObject)

                                changedObject = New ChangedObject(kblObject.ObjectType, kblSysId, change.KblIdRef, change.KblIdComp, changeType, ModelType.compare)
                                changedObject.ID = id
                                AddResult(changedObject)
                            End If
                        End If
                    End If
                End With
            Case KblObjectType.Segment
                With DirectCast(kblObject, Segment)
                    Dim segment As Segment = DirectCast(kblObject, Segment)
                    kblSysId = .SystemId

                    Dim change As ChangedItem = Nothing
                    change = _comparisonMapperList(KblObjectType.Segment).Changes.ByChange(changeType).Where(Function(item) item.Key = kblSysId).FirstOrDefault()

                    If TypeOf (change.Item) Is SegmentChangedProperty OrElse TypeOf (change.Item) Is Segment AndAlso changeType = CompareChangeType.Deleted Then
                        CheckForProtections(change, segment, changeType, kblSysId)
                    End If

                    If changeType = CompareChangeType.New Then
                        If change IsNot Nothing Then
                            AddResult(New ChangedObject(kblObject.ObjectType, kblSysId, change.KblIdRef, change.KblIdComp, changeType, ModelType.compare))
                        End If
                    Else
                        If change IsNot Nothing Then
                            If changeType = CompareChangeType.Deleted Then
                                Dim changedObject As New ChangedObject(kblObject.ObjectType, kblSysId, change.KblIdRef, change.KblIdComp, changeType, ModelType.reference)
                                changedObject.ID = id
                                AddResult(changedObject)
                            ElseIf changeType = CompareChangeType.Modified Then

                                Dim changedObject As New ChangedObject(kblObject.ObjectType, kblSysId, change.KblIdRef, change.KblIdComp, changeType, ModelType.reference)
                                changedObject.ID = id
                                AddResult(changedObject)
                                changedObject = New ChangedObject(kblObject.ObjectType, kblSysId, change.KblIdRef, change.KblIdComp, changeType, ModelType.compare)
                                changedObject.ID = id
                                AddResult(changedObject)
                            End If
                        End If

                    End If
                End With
            Case KblObjectType.Special_wire_occurrence
                Dim mySpecialWireOccurrence As Special_wire_occurrence = DirectCast(kblObject, Special_wire_occurrence)
                Dim change As ChangedItem = Nothing
                Dim connectors As New List(Of Connector_occurrence)
                Dim segmentsRef As New List(Of Segment)
                Dim segmentsComp As New List(Of Segment)
                Dim connector As New Connector_occurrence

                kblSysId = mySpecialWireOccurrence.SystemId
                change = _comparisonMapperList(KblObjectType.Special_wire_occurrence).Changes.ByChange(changeType).Where(Function(item) item.Key = kblSysId).FirstOrDefault()

                Dim myModelType As ModelType = ModelType.reference
                If change.Change = CompareChangeType.New Then myModelType = ModelType.compare

                Dim myChangedObj As New ChangedObject(KblObjectType.Special_wire_occurrence, kblSysId, change.KblIdRef, change.KblIdComp, change.Change, myModelType, KblObjectType.Special_wire_occurrence, mySpecialWireOccurrence.Special_wire_id)
                AddResult(myChangedObj)

                If Not String.IsNullOrEmpty(change.KblIdRef) Then

                    If _generalSettings.Mark3DConnectorsOnWireModification Then
                        Dim myConnectors As New List(Of ChangedObject)
                        For Each core As Core_occurrence In mySpecialWireOccurrence.Core_occurrence
                            connectors = GetConnectorsFromCore(_activeMapper, core)

                            For Each cn As Connector_occurrence In connectors
                                Dim obj As New ChangedObject(connector.ObjectType, kblSysId, cn.SystemId, "", CompareChangeType.Routed, ModelType.reference, mySpecialWireOccurrence.ObjectType, myChangedObj.ID)
                                AddResult(obj)
                                myConnectors.Add(obj)
                            Next
                        Next

                    End If
                    segmentsRef = GetSegmentsFromCable(_activeMapper, mySpecialWireOccurrence)
                    For Each sg As Segment In segmentsRef
                        Dim obj As New ChangedObject(sg.ObjectType, kblSysId, sg.SystemId, "", CompareChangeType.Routed, ModelType.reference, mySpecialWireOccurrence.ObjectType, sg.Id)
                        AddResult(obj)
                    Next
                End If

                If Not String.IsNullOrEmpty(change.KblIdComp) Then
                    mySpecialWireOccurrence = _compareMapper.KBLCableList.Where(Function(w) w.SystemId = change.KblIdComp).FirstOrDefault
                    If mySpecialWireOccurrence IsNot Nothing Then

                        If _generalSettings.Mark3DConnectorsOnWireModification Then
                            Dim myConnectors As New List(Of ChangedObject)

                            For Each core As Core_occurrence In mySpecialWireOccurrence.Core_occurrence
                                connectors = GetConnectorsFromCore(_compareMapper, core)
                                For Each cn As Connector_occurrence In connectors
                                    Dim obj As New ChangedObject(connector.ObjectType, kblSysId, "", cn.SystemId, CompareChangeType.Routed, ModelType.compare, mySpecialWireOccurrence.ObjectType, myChangedObj.ID)
                                    AddResult(obj)
                                    myConnectors.Add(obj)
                                Next
                            Next
                        End If

                        segmentsComp = GetSegmentsFromCable(_compareMapper, mySpecialWireOccurrence)

                        For Each sg As Segment In segmentsComp
                            Dim obj As New ChangedObject(sg.ObjectType, kblSysId, "", sg.SystemId, CompareChangeType.Routed, ModelType.compare, mySpecialWireOccurrence.ObjectType, sg.Id)
                            AddResult(obj)
                        Next
                    End If
                End If

            Case KblObjectType.Wire_occurrence
                Dim myWire As Wire_occurrence = DirectCast(kblObject, Wire_occurrence)
                kblSysId = myWire.SystemId
                Dim change As ChangedItem = Nothing
                change = _comparisonMapperList(KblObjectType.Wire_occurrence).Changes.ByChange(changeType).Where(Function(item) item.Key = kblSysId).FirstOrDefault()

                Dim myModelType As ModelType = ModelType.reference
                If change.Change = CompareChangeType.New Then myModelType = ModelType.compare
                Dim myChangedObj As New ChangedObject(KblObjectType.Wire_occurrence, kblSysId, change.KblIdRef, change.KblIdComp, change.Change, myModelType, KblObjectType.Wire_occurrence, myWire.Wire_number)
                AddResult(myChangedObj)

                Dim connectors As New List(Of Connector_occurrence)
                Dim segmentsRef As New List(Of Segment)
                Dim segmentsComp As New List(Of Segment)

                Dim connector As New Connector_occurrence
                Dim targetType As Type = connector.GetType


                If Not String.IsNullOrEmpty(myChangedObj.KblIdRef) Then
                    If _generalSettings.Mark3DConnectorsOnWireModification Then
                        connectors = GetConnectorsFromWire(_activeMapper, myWire)
                        For Each cn As Connector_occurrence In connectors
                            Dim obj As New ChangedObject(KblObjectType.Connector_occurrence, kblSysId, cn.SystemId, "", CompareChangeType.Routed, ModelType.reference, myWire.ObjectType, myWire.Wire_number)
                            AddResult(obj)
                        Next
                    End If

                    segmentsRef = GetSegmentsFromWire(_activeMapper, myWire)

                    For Each sg As Segment In segmentsRef
                        Dim obj As New ChangedObject(sg.ObjectType, kblSysId, sg.SystemId, "", CompareChangeType.Routed, ModelType.reference, myWire.ObjectType, myWire.Wire_number)
                        AddResult(obj)
                    Next
                End If


                If Not String.IsNullOrEmpty(change.KblIdComp) Then
                    myWire = _compareMapper.KBLWireList.Where(Function(w) w.SystemId = change.KblIdComp).FirstOrDefault
                    If myWire IsNot Nothing Then

                        If _generalSettings.Mark3DConnectorsOnWireModification Then
                            connectors = GetConnectorsFromWire(_compareMapper, myWire)
                            For Each cn As Connector_occurrence In connectors
                                Dim obj As New ChangedObject(KblObjectType.Connector_occurrence, kblSysId, "", cn.SystemId, CompareChangeType.Routed, ModelType.compare, myWire.ObjectType, myWire.Wire_number)
                                AddResult(obj)
                            Next
                        End If

                        segmentsComp = GetSegmentsFromWire(_compareMapper, myWire)
                        For Each sg As Segment In segmentsComp
                            Dim obj As New ChangedObject(sg.ObjectType, kblSysId, "", sg.SystemId, CompareChangeType.Routed, ModelType.compare, myWire.ObjectType, myWire.Wire_number)
                            AddResult(obj)
                        Next
                    End If
                End If

            Case KblObjectType.Accessory_occurrence
                With DirectCast(kblObject, Accessory_occurrence)
                    kblSysId = .SystemId

                    Dim change As ChangedItem = Nothing
                    change = _comparisonMapperList(KblObjectType.Accessory_occurrence).Changes.ByChange(changeType).Where(Function(item) item.Key = kblSysId).FirstOrDefault()
                    If changeType = CompareChangeType.New Then
                        If change IsNot Nothing Then
                            AddResult(New ChangedObject(kblObject.ObjectType, kblSysId, change.KblIdRef, change.KblIdComp, changeType, ModelType.compare))
                        End If
                    Else
                        If change IsNot Nothing Then
                            If changeType = CompareChangeType.Deleted Then
                                Dim changedObject As New ChangedObject(kblObject.ObjectType, kblSysId, change.KblIdRef, change.KblIdComp, changeType, ModelType.reference)
                                AddResult(changedObject)
                            ElseIf changeType = CompareChangeType.Modified Then
                                Dim changedObject As New ChangedObject(kblObject.ObjectType, kblSysId, change.KblIdRef, change.KblIdComp, changeType, ModelType.reference)
                                AddResult(changedObject)
                                changedObject = New ChangedObject(kblObject.ObjectType, kblSysId, change.KblIdRef, change.KblIdComp, changeType, ModelType.compare)
                                AddResult(changedObject)
                            End If
                        End If
                    End If
                End With
            Case KblObjectType.Component_box_occurrence
                With DirectCast(kblObject, Component_box_occurrence)
                    kblSysId = .SystemId

                    Dim change As ChangedItem = Nothing
                    change = _comparisonMapperList(KblObjectType.Component_box_occurrence).Changes.ByChange(changeType).Where(Function(item) item.Key = kblSysId).FirstOrDefault()
                    If changeType = CompareChangeType.New Then
                        If change IsNot Nothing Then
                            AddResult(New ChangedObject(kblObject.ObjectType, kblSysId, change.KblIdRef, change.KblIdComp, changeType, ModelType.compare))
                        End If

                    Else
                        If change IsNot Nothing Then
                            If changeType = CompareChangeType.Deleted Then
                                Dim changedObject As New ChangedObject(kblObject.ObjectType, kblSysId, change.KblIdRef, change.KblIdComp, changeType, ModelType.compare)
                                AddResult(changedObject)
                            ElseIf changeType = CompareChangeType.Modified Then
                                Dim changedObject As New ChangedObject(kblObject.ObjectType, kblSysId, change.KblIdRef, change.KblIdComp, changeType, ModelType.reference)
                                AddResult(changedObject)
                                changedObject = New ChangedObject(kblObject.ObjectType, kblSysId, change.KblIdRef, change.KblIdComp, changeType, ModelType.compare)
                                AddResult(changedObject)
                            End If
                        End If

                    End If

                End With
            Case KblObjectType.Component_occurrence, KblObjectType.Fuse_occurrence
                With DirectCast(kblObject, Component_occurrence)
                    kblSysId = .SystemId

                    Dim change As ChangedItem = Nothing
                    change = _comparisonMapperList(KblObjectType.Component_occurrence).Changes.ByChange(changeType).Where(Function(item) item.Key = kblSysId).FirstOrDefault()

                    If changeType = CompareChangeType.New Then
                        If change IsNot Nothing Then
                            AddResult(New ChangedObject(kblObject.ObjectType, kblSysId, change.KblIdRef, change.KblIdComp, changeType, ModelType.compare))
                        End If
                    Else
                        If change IsNot Nothing Then
                            If changeType = CompareChangeType.Deleted Then
                                Dim changedObject As New ChangedObject(kblObject.ObjectType, kblSysId, change.KblIdRef, change.KblIdComp, changeType, ModelType.compare)
                                AddResult(changedObject)
                            ElseIf changeType = CompareChangeType.Modified Then
                                Dim changedObject As New ChangedObject(kblObject.ObjectType, kblSysId, change.KblIdRef, change.KblIdComp, changeType, ModelType.reference)
                                AddResult(changedObject)
                                changedObject = New ChangedObject(kblObject.ObjectType, kblSysId, change.KblIdRef, change.KblIdComp, changeType, ModelType.compare)
                                AddResult(changedObject)
                            End If
                        End If
                    End If
                End With
            Case KblObjectType.Connector_occurrence
                Dim connector As Connector_occurrence = CType(kblObject, Connector_occurrence)
                Dim connectors As List(Of ChangedObject) = GetConnectorsFromConnectorOccurrence(connector, kblSysId, changeType)
                If connectors.Count > 0 Then
                    For Each cn As ChangedObject In connectors
                        cn.ID = connector.Id
                        AddResult(cn)
                    Next
                End If

            Case KblObjectType.Wire_protection_occurrence
                Dim myProtection As Terminal_occurrence = DirectCast(kblObject, Terminal_occurrence)
                Dim myMapper As KblMapper
                If changeType = CompareChangeType.New Then
                    myMapper = _compareMapper
                Else
                    myMapper = _activeMapper
                End If

            Case KblObjectType.Connection
                Dim myConnection As Connection = DirectCast(kblObject, Connection)
                If _netList.Contains(myConnection.Signal_name) Then
                    Exit Select
                Else
                    _netList.Add(myConnection.Signal_name)
                End If

                kblSysId = myConnection.SystemId
                Dim change As ChangedItem = Nothing

                Dim refChange As ChangedItem = Nothing
                Dim compChange As ChangedItem = Nothing

                change = _comparisonMapperList(KblObjectType.Net).Changes.ByChange(changeType).Where(Function(item) item.Key = myConnection.Signal_name).FirstOrDefault()
                Dim myChangedObj As ChangedObject = Nothing

                If changeType = CompareChangeType.New Then
                    myChangedObj = New ChangedObject(KblObjectType.Connection, kblSysId, "", myConnection.Signal_name, change.Change, ModelType.compare, KblObjectType.Connection, myConnection.Id)

                ElseIf changeType = CompareChangeType.Deleted Then
                    myChangedObj = New ChangedObject(KblObjectType.Connection, kblSysId, myConnection.Signal_name, "", change.Change, ModelType.reference, KblObjectType.Connection, myConnection.Id)

                ElseIf changeType = CompareChangeType.Modified Then
                    myChangedObj = New ChangedObject(KblObjectType.Connection, kblSysId, myConnection.Signal_name, myConnection.Signal_name, change.Change, ModelType.reference, KblObjectType.Connection, myConnection.Id)
                End If

                myChangedObj.Net = myConnection.Signal_name

                Dim myRefConnections As HashSet(Of Connection) = _activeMapper.KBLNetMapper(myConnection.Signal_name)
                Dim myCompConnections As HashSet(Of Connection) = _compareMapper.KBLNetMapper(myConnection.Signal_name)

                For Each cn As Connection In myRefConnections
                    myChangedObj = New ChangedObject(KblObjectType.Connection, cn.SystemId, cn.Signal_name, "", change.Change, ModelType.reference, KblObjectType.Connection, cn.Id)
                    myChangedObj.WireId = cn.Wire
                    myChangedObj.Net = cn.Signal_name
                    AddResult(myChangedObj)
                Next

                For Each cn As Connection In myCompConnections
                    myChangedObj = New ChangedObject(KblObjectType.Connection, cn.SystemId, "", cn.Signal_name, change.Change, ModelType.compare, KblObjectType.Connection, cn.Id)
                    myChangedObj.WireId = cn.Wire
                    myChangedObj.Net = cn.Signal_name
                    AddResult(myChangedObj)
                Next

                Dim mapper As KblMapper = _activeMapper

                For Each cn As Connection In myRefConnections
                    Dim refids As List(Of String) = GetSegmentsIdsFromWire(_activeMapper, cn.Wire)
                    For Each segId As String In refids
                        Dim mySegment As Segment = _activeMapper.GetSegments.Where(Function(s) s.SystemId = id).FirstOrDefault
                        Dim obj As New ChangedObject(KblObjectType.Segment, cn.SystemId, segId, "", change.Change, ModelType.reference, KblObjectType.Connection, cn.Id)
                        obj.Net = cn.Signal_name
                        obj.WireId = cn.Wire
                        AddResult(obj)
                    Next
                Next

                mapper = _compareMapper
                For Each cn As Connection In myCompConnections
                    Dim compids As List(Of String) = GetSegmentsIdsFromWire(mapper, cn.Wire)
                    For Each comp_id As String In compids
                        Dim mySegment As Segment = mapper.GetSegments.Where(Function(s) s.SystemId = comp_id).FirstOrDefault
                        Dim obj As New ChangedObject(KblObjectType.Segment, cn.SystemId, "", comp_id, change.Change, ModelType.compare, KblObjectType.Connection, cn.Id)
                        obj.Net = cn.Signal_name
                        obj.WireId = cn.Wire
                        AddResult(obj)
                    Next
                Next
        End Select
    End Sub

    Private Function GetChangeType(change_str As String) As CompareChangeType
        If change_str = "Added" Then
            Return CompareChangeType.New
        ElseIf change_str = "Modified" Then
            Return CompareChangeType.Modified
        ElseIf change_str = "Deleted" Then
            Return CompareChangeType.Deleted
        End If
        Return CompareChangeType.New
    End Function

    Private Sub AddObjectToList(row As UltraDataRow)
        Dim compareObjKey As String = ""
        Dim myParent As UltraDataRow = row.ParentRow
        Dim parentId As String = ""

        If myParent IsNot Nothing Then
            compareObjKey = DirectCast(myParent.Band.Tag, Dictionary(Of String, Object)).Keys.ElementAt(myParent.Index)
        Else
            compareObjKey = DirectCast(row.Band.Tag, Dictionary(Of String, Object)).Keys.ElementAt(row.Index)
        End If

        Dim change_Type As CompareChangeType
        If TypeOf row.Tag Is String Then
            parentId = compareObjKey.Split("|"c).Last ' HINT: only for compatibility -> to be removed when all tags have been replaced
        ElseIf TypeOf row.Tag Is NetRowInfoContainer Then
            parentId = CType(row.Tag, NetRowInfoContainer).NetName
        End If

        change_Type = GetChangeType(compareObjKey.Split("|").First)

        Dim id As String = TryGetId(row.Tag)
        Dim occurrenceObject As IKblBaseObject = GetKblOccurrcence(row)

        If occurrenceObject IsNot Nothing Then
            AddKblObjectToList(occurrenceObject, parentId, id, change_Type)
        Else
            If Not String.IsNullOrEmpty(parentId) Then
                Dim kblSysId As String = GetSystemId(row) ' kleinId

                If change_Type = CompareChangeType.Deleted Then
                    Dim mapper As KblMapper = _activeMapper
                    If mapper.KBLNetMapper.ContainsKey(parentId) Then
                        Dim myRefConnections As HashSet(Of Connection) = mapper.KBLNetMapper.Item(parentId)
                        If myRefConnections IsNot Nothing Then

                            For Each cn As Connection In myRefConnections
                                Dim myChangedObj As New ChangedObject(KblObjectType.Connection, kblSysId, parentId, "", change_Type, ModelType.reference, KblObjectType.Connection, cn.Id)
                                AddResult(myChangedObj)
                                If change_Type = CompareChangeType.Modified OrElse change_Type = CompareChangeType.Deleted Then
                                    Dim refids As List(Of String) = GetSegmentsIdsFromWire(mapper, cn.Wire)
                                    For Each segId As String In refids
                                        Dim mySegment As Segment = mapper.GetSegments.Where(Function(s) s.SystemId = id).FirstOrDefault
                                        Dim obj As New ChangedObject(KblObjectType.Segment, cn.SystemId, segId, "", change_Type, ModelType.reference, KblObjectType.Connection, cn.Id)
                                        obj.Net = parentId
                                        obj.WireId = cn.Wire
                                        AddResult(obj)
                                    Next
                                End If
                            Next
                        End If
                    End If
                End If

                If change_Type = CompareChangeType.New Then
                    Dim mapper As KblMapper = _compareMapper
                    If mapper.KBLNetMapper.ContainsKey(parentId) Then
                        Dim mycompConnections As HashSet(Of Connection) = mapper.KBLNetMapper.Item(parentId)
                        If mycompConnections IsNot Nothing Then
                            For Each cn As Connection In mycompConnections
                                Dim myChangedObj As New ChangedObject(KblObjectType.Connection, kblSysId, "", parentId, change_Type, ModelType.compare, KblObjectType.Connection, cn.Id)
                                AddResult(myChangedObj)
                                Dim compids As List(Of String) = GetSegmentsIdsFromWire(mapper, cn.Wire)
                                For Each segId As String In compids
                                    Dim mySegment As Segment = mapper.GetSegments.Where(Function(s) s.SystemId = id).FirstOrDefault
                                    Dim obj As New ChangedObject(KblObjectType.Segment, cn.SystemId, "", segId, change_Type, ModelType.compare, KblObjectType.Connection, cn.Id)
                                    obj.Net = parentId
                                    obj.WireId = cn.Wire
                                    AddResult(obj)
                                Next
                            Next
                        End If
                    End If
                End If
            End If
        End If
    End Sub

    Private Sub CheckForProtections(change As ChangedItem, segment As Segment, changeType As CompareChangeType, srcId As String)
        Dim protectionArea As New Wire_protection_occurrence

        If change IsNot Nothing AndAlso TypeOf (change.Item) Is SegmentChangedProperty Then
            If CType(change.Item, SegmentChangedProperty).ChangedProperties.ContainsKey(Properties.SegmentPropertyName.Protection_Area) Then
                Dim myItem As ProtectionAreaComparisonMapper = CType(CType(change.Item, SegmentChangedProperty).ChangedProperties.Item(Properties.SegmentPropertyName.Protection_Area), ProtectionAreaComparisonMapper)
                Dim protections As Protection_area() = segment.Protection_area

                For Each entry As ChangedItem In myItem.Changes
                    If entry.Change = CompareChangeType.New Then
                        AddResult(New ChangedObject(protectionArea.ObjectType, srcId, entry.KblIdRef, entry.KblIdComp, entry.Change, ModelType.compare))
                    Else
                        If entry.Change = CompareChangeType.Deleted Then
                            Dim changedObject As New ChangedObject(protectionArea.ObjectType, srcId, entry.KblIdRef, entry.KblIdComp, changeType, ModelType.reference)
                            AddResult(changedObject)

                        ElseIf changeType = CompareChangeType.Modified Then

                            Dim changedObject As New ChangedObject(protectionArea.ObjectType, srcId, entry.KblIdRef, entry.KblIdComp, changeType, ModelType.reference)
                            AddResult(changedObject)
                            changedObject = New ChangedObject(protectionArea.ObjectType, srcId, entry.KblIdRef, entry.KblIdComp, changeType, ModelType.compare)
                            AddResult(changedObject)
                        End If
                    End If
                Next
            End If
        ElseIf change IsNot Nothing AndAlso TypeOf (change.Item) Is Segment Then
            For Each item As Protection_area In segment.Protection_area
                Dim changedObject As New ChangedObject(protectionArea.ObjectType, segment.SystemId, item.Associated_protection, "", changeType, ModelType.reference)
                AddResult(changedObject)
            Next
        End If
    End Sub

    Private Function GetConnectorsFromCore(mapper As KblMapper, core As Core_occurrence) As List(Of Connector_occurrence)
        Dim cns As New List(Of Connector_occurrence)
        Dim cnids As New List(Of String)

        For Each item As KeyValuePair(Of String, Dictionary(Of String, List(Of Cavity_occurrence))) In mapper.KBLWireCavityMapper
            If item.Value.ContainsKey(core.SystemId) Then
                Dim cavities As List(Of Cavity_occurrence) = item.Value.Item(core.SystemId)
                For Each cavity As Cavity_occurrence In cavities
                    Dim connectorId As String = mapper.KBLCavityConnectorMapper.Item(cavity.SystemId)
                    If Not cnids.Contains(connectorId) Then
                        cnids.Add(connectorId)
                    End If
                Next
            End If
        Next

        For Each id As String In cnids
            Dim conn_occ As Connector_occurrence = mapper.GetOccurrenceObject(Of Connector_occurrence)(id)
            If conn_occ IsNot Nothing Then
                cns.Add(conn_occ)
            End If
        Next

        Return cns
    End Function

    Private Function GetConnectorsFromCable(mapper As KblMapper, wr As Special_wire_occurrence) As List(Of Connector_occurrence)
        Dim cns As New List(Of Connector_occurrence)
        Dim cnids As New List(Of String)
        For Each c As Core_occurrence In wr.Core_occurrence
            Dim test1 As List(Of List(Of String)) = mapper.KBLSegmentWireMapper.Select(Function(v) v.Value).Where(Function(t) t.Contains(c.SystemId)).ToList
            Dim x As Integer = test1.Count

            For Each item As KeyValuePair(Of String, Dictionary(Of String, List(Of Cavity_occurrence))) In mapper.KBLWireCavityMapper
                If item.Value.ContainsKey(c.SystemId) Then
                    Dim test3 As List(Of Cavity_occurrence) = item.Value.Item(c.SystemId)
                    For Each entry As Cavity_occurrence In test3
                        Dim cnid As String = mapper.KBLCavityConnectorMapper.Item(entry.SystemId)
                        If Not cnids.Contains(cnid) Then
                            cnids.Add(cnid)
                        End If
                    Next
                End If

            Next

            For Each id As String In cnids
                Dim conn_occ As Connector_occurrence = mapper.GetOccurrenceObject(Of Connector_occurrence)(id)
                If conn_occ IsNot Nothing Then
                    cns.Add(conn_occ)
                End If
            Next

        Next
        Return cns
    End Function

    Private Function GetConnectorsFromCable_(mapper As KblMapper, wr As Special_wire_occurrence) As List(Of Connector_occurrence)
        Dim cns As New List(Of Connector_occurrence)

        Dim dic As List(Of KeyValuePair(Of String, Dictionary(Of String, List(Of Cavity_occurrence)))) = mapper.KBLWireCavityMapper.Where(Function(obj) obj.Value.ContainsKey(wr.SystemId)).ToList
        For Each item As KeyValuePair(Of String, Dictionary(Of String, List(Of Cavity_occurrence))) In dic
            Dim cv As KeyValuePair(Of String, List(Of Cavity_occurrence)) = item.Value.Where(Function(obj) obj.Key = wr.SystemId).FirstOrDefault
            If cv.Value IsNot Nothing Then
                Dim myCavities As List(Of Cavity_occurrence) = cv.Value
                For Each cavity As Cavity_occurrence In myCavities
                    Dim cn As KeyValuePair(Of String, String) = mapper.KBLCavityConnectorMapper.Where(Function(obj) obj.Key = cavity.SystemId).FirstOrDefault
                    If cn.Value IsNot Nothing Then
                        Dim mycn As Connector_occurrence = mapper.GetConnectorOccurrences.Where(Function(obj) obj.SystemId = cn.Value).FirstOrDefault
                        If mycn IsNot Nothing Then
                            cns.Add(mycn)
                        End If
                    End If
                Next
            End If
        Next
        Return cns
    End Function

    Private Function GetConnectorsFromWire(mapper As KblMapper, wr As Wire_occurrence) As List(Of Connector_occurrence)
        Dim cns As New List(Of Connector_occurrence)

        Dim dic As List(Of KeyValuePair(Of String, Dictionary(Of String, List(Of Cavity_occurrence)))) = mapper.KBLWireCavityMapper.Where(Function(obj) obj.Value.ContainsKey(wr.SystemId)).ToList
        For Each item As KeyValuePair(Of String, Dictionary(Of String, List(Of Cavity_occurrence))) In dic
            Dim cv As KeyValuePair(Of String, List(Of Cavity_occurrence)) = item.Value.Where(Function(obj) obj.Key = wr.SystemId).FirstOrDefault
            If cv.Value IsNot Nothing Then
                Dim myCavities As List(Of Cavity_occurrence) = cv.Value
                For Each cavity As Cavity_occurrence In myCavities
                    Dim cn As KeyValuePair(Of String, String) = mapper.KBLCavityConnectorMapper.Where(Function(obj) obj.Key = cavity.SystemId).FirstOrDefault
                    If cn.Value IsNot Nothing Then
                        Dim mycn As Connector_occurrence = mapper.GetHarness.Connector_occurrence.Where(Function(obj) obj.SystemId = cn.Value).FirstOrDefault
                        If mycn IsNot Nothing Then
                            cns.Add(mycn)
                        End If
                    End If
                Next
            End If
        Next
        Return cns
    End Function

    Private Function GetConnectorsFromConnectorOccurrence(connector As Connector_occurrence, srcId As String, ChangeType As CompareChangeType) As List(Of ChangedObject)
        Dim kblid As String
        Dim res As New List(Of ChangedObject)
        With DirectCast(connector, Connector_occurrence)
            kblid = .SystemId

            Dim change As ChangedItem = Nothing
            change = _comparisonMapperList(KblObjectType.Connector_occurrence).Changes.ByChange(ChangeType).Where(Function(item) item.Key = kblid).FirstOrDefault()

            If ChangeType = CompareChangeType.New Then
                If change IsNot Nothing Then
                    res.Add(New ChangedObject(connector.ObjectType, srcId, change.KblIdRef, change.KblIdComp, ChangeType, ModelType.compare))
                End If
            Else
                If change IsNot Nothing Then
                    If ChangeType = CompareChangeType.Deleted Then
                        Dim changedObject As New ChangedObject(connector.ObjectType, srcId, change.KblIdRef, change.KblIdComp, ChangeType, ModelType.reference)
                        res.Add(changedObject)

                    ElseIf ChangeType = CompareChangeType.Modified Then
                        Dim changedObject As New ChangedObject(connector.ObjectType, srcId, change.KblIdRef, change.KblIdComp, ChangeType, ModelType.reference)
                        res.Add(changedObject)

                        changedObject = New ChangedObject(connector.ObjectType, srcId, change.KblIdRef, change.KblIdComp, ChangeType, ModelType.compare)
                        res.Add(changedObject)
                    End If
                End If
            End If
        End With
        Return res
    End Function

    Private Function GetSegmentsIdsFromWire(mapper As KblMapper, wireId As String) As List(Of String)
        Return mapper.KBLWireSegmentMapper.Where(Function(x) x.Key = wireId).Select(Function(s) s.Value).ToList
    End Function

    Private Function GetSegmentsFromWire(mapper As KblMapper, wire As Wire_occurrence) As List(Of Segment)
        Dim segments As New List(Of Segment)
        Dim myWire As Wire_occurrence = mapper.KBLWireList.Where(Function(w) w.SystemId = wire.SystemId).FirstOrDefault
        If myWire IsNot Nothing Then
            For Each seg As Segment In mapper.KBLWireSegmentMapper.GetSegmentsOrEmpty(myWire.SystemId)
                Dim sg As Segment = mapper.GetSegments.Where(Function(s) s.SystemId = seg.SystemId).FirstOrDefault
                If sg IsNot Nothing Then
                    For Each obj As IKblWireCoreOccurrence In sg.GetWiresAndCores(mapper)
                        If obj.ObjectType = KblObjectType.Wire_occurrence Then
                            If CType(obj, Wire_occurrence).SystemId = wire.SystemId Then
                                segments.Add(sg)
                                Exit For
                            End If
                        End If
                    Next
                End If
            Next
        End If

        Return segments
    End Function

    Private Function GetSegmentsFromCable(mapper As KblMapper, wire As Special_wire_occurrence) As List(Of Segment)
        Dim resultSegs As New List(Of Segment)
        Dim myCable As Special_wire_occurrence = mapper.KBLCableList.Where(Function(w) w.SystemId = wire.SystemId).FirstOrDefault
        If myCable IsNot Nothing Then
            Dim segments As IEnumerable(Of Segment) = mapper.KBLWireSegmentMapper.GetSegmentsOrEmpty(myCable.SystemId)
            For Each seg As Segment In segments
                Dim sg As Segment = mapper.GetSegments.Where(Function(s) s.SystemId = seg.SystemId).FirstOrDefault
                If sg IsNot Nothing Then
                    resultSegs.Add(sg)
                End If
            Next
        End If
        Return resultSegs
    End Function

    Private Sub Show3D()
        _netList.Clear()

        If IsD3D Then
            _compareResult = New Dictionary(Of KblObjectType, List(Of ChangedObject))

            For Each row As UltraDataRow In _compareHub.Rows
                AddObjectToList(row)
                Dim b As UltraDataBand = row.Band
                If b.ChildBands.Count > 0 Then
                    If row.GetChildRows(0).Count > 0 Then
                        For Each crow As UltraDataRow In row.GetChildRows(0)
                            AddObjectToList(crow)
                        Next
                    End If
                End If
            Next
            If _d3dCntrl IsNot Nothing Then
                If SplitContainer1.Panel1.Controls.Contains(_d3dCntrl) Then
                    SplitContainer1.Panel1.Controls.Remove(_d3dCntrl)
                    SplitContainer1.Panel1.Refresh()
                End If
                _d3dCntrl.Dispose()
            End If
            _d3dCntrl = New D3DComparerCntrl(_generalSettings, _compareHub, _activeDocument, _compareDocument, _compareResult)
            _d3dCntrl.Dock = DockStyle.Fill
            SplitContainer1.Panel1.Controls.Add(_d3dCntrl)
        End If
    End Sub

    Private Sub D3DCompareFormSelectionChanged(sender As Object, e As ComparerSelectionChangedEventArgs) Handles _d3dCntrl.SelectionChangedInD3DComparer
        Dim kleinIds As List(Of String) = e.Source

        Dim kblMapper As KblMapper
        Dim myChangeType As String = ""

        If e.KblMapperSourceId = _compareDocument.Kbl.Id Then
            kblMapper = _compareMapper
        Else
            kblMapper = _activeMapper
        End If

        If e.ChangedObject IsNot Nothing AndAlso e.ChangedObject.ChangeType = CompareChangeType.Modified AndAlso Not String.IsNullOrEmpty(e.ChangedObject.KblIdRef) Then
            kleinIds = New List(Of String) From {e.ChangedObject.KblIdRef}
            kblMapper = _activeMapper
        End If

        If e.ChangedObject IsNot Nothing Then
            myChangeType = e.ChangedObject.ChangeType.ToString
            If myChangeType = "New" Then
                myChangeType = "Added"
            End If
        End If
        Dim selRows As List(Of String) = _compareHub.SelectRowsInCompareHubGrids(kleinIds, kblMapper, myChangeType, True, True, False, True)

        Dim ev As New InformationHubEventArgs(e.KblMapperSourceId)
        ev.ObjectIds = New HashSet(Of String)(e.Source)

        RaiseEvent CompareHubSelectionChanged(_compareHub, ev)

        Me.Select()
    End Sub

    Private Sub D3d_SelectedEntitiesChanged(en As IBaseModelEntityEx, HarnessId As String)
        DeselectAllIn3D()
        For Each drawingMdiTab As MdiTab In My.Application.MainForm.utmmMain.TabGroups.All.Cast(Of MdiTabGroup).SelectMany(Function(grp) grp.Tabs.Cast(Of MdiTab)())
            Dim docForm As IDocumentForm = TryCast(drawingMdiTab.Form, IDocumentForm)
            If docForm IsNot Nothing Then
                If docForm.Kbl.Id = HarnessId Then
                    Dim docEntsDic As Dictionary(Of String, IBaseModelEntityEx) = TryCast(docForm.HcvDocument, HcvDocument)?.Entities.ToDictionary(Function(ent) ent.Id, Function(ent) ent)
                    Dim docEntity As IBaseModelEntityEx = Nothing
                    If docEntsDic.TryGetValue(en.Id, docEntity) Then
                        docEntity.Selected = True
                    End If
                    TryCast(docForm.HcvDocument, HcvDocument)?.Entities.UpdateVisibleSelection()
                End If
            End If
        Next
    End Sub

    Private Sub DeselectAllIn3D()
        For Each drawingMdiTab As MdiTab In My.Application.MainForm.utmmMain.TabGroups.All.Cast(Of MdiTabGroup).SelectMany(Function(grp) grp.Tabs.Cast(Of MdiTab)())
            Dim docForm As IDocumentForm = TryCast(drawingMdiTab.Form, IDocumentForm)
            If docForm IsNot Nothing Then
                For Each entity As Entity In (TryCast(docForm.HcvDocument, HcvDocument)?.Entities).OrEmpty
                    If entity.Selectable Then
                        entity.Selected = False
                    End If
                Next
                TryCast(docForm.HcvDocument, HcvDocument)?.Entities?.UpdateVisibleSelection()
            End If
        Next
    End Sub

    Private Sub AddResult(obj As ChangedObject)
        If _generalSettings.InverseCompare Then
            If obj.ChangeType = CompareChangeType.New Then
                obj.ModelType = ModelType.reference
            ElseIf obj.ChangeType = CompareChangeType.Deleted Then
                obj.ModelType = ModelType.compare
            End If
        End If

        If _compareResult.ContainsKey(obj.ObjType) Then
            _compareResult(obj.ObjType).Add(obj)
        Else
            _compareResult.Add(obj.ObjType, New List(Of ChangedObject) From {obj})
        End If
    End Sub

    Private Sub CompareForm_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        If _compareHub IsNot Nothing AndAlso _d3dCntrl IsNot Nothing Then
            If e.KeyCode = Keys.Escape Then
                D3d_SelectedEntitiesChanged(Nothing, Guid.Empty.ToString)
                _3DSelection = True
                Dim kblMapper As KblMapper = _d3dCntrl.GetCurrentKBLMapper
                _compareHub?.SelectRowsInCompareHubGrids(New List(Of String), kblMapper, "", True, True, False, True)
                _3DSelection = False
            End If
        End If
    End Sub

    Private Sub CompareForm_FormClosed(sender As Object, e As FormClosedEventArgs) Handles Me.FormClosed
        If _d3dCntrl IsNot Nothing Then
            _d3dCntrl.Dispose()
        End If
        If _compareHub IsNot Nothing Then
            _compareHub.Dispose()
        End If

        _compareResult?.Clear()
        _netList?.Clear()
        _comparisonMapperList?.Clear()
        _currentActiveObjects?.Clear()
        _reloadedDocuments?.Clear()

        _d3dCntrl = Nothing
        _compareHub = Nothing

        _refModel = Nothing
        _compModel = Nothing
        _compareDocument = Nothing
        _compareHarnessConfig = Nothing
        _compareMapper = Nothing
        _currentCheckedResult = Nothing
        _activeDocument = Nothing
        _activeMapper = Nothing
        _activeHarnessConfig = Nothing
    End Sub

End Class