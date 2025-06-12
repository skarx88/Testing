Imports System.ComponentModel
Imports System.Data
Imports devDept.Eyeshot.Entities
Imports Infragistics.Win.UltraWinGrid
Imports Infragistics.Win.UltraWinListView
Imports Zuken.E3.Lib.Eyeshot.Model
Imports Zuken.E3.Lib.IO.KBL
Imports Zuken.E3.Lib.Model
Imports Zuken.E3.Lib.Schema.Kbl.Properties

Namespace D3D.Document.Controls.ToolTips

    Public Class EntityToolTipControl

        Public Shadows Event MouseEnter(sender As Object, e As EventArgs)
        Public Shadows Event MouseLeave(sender As Object, e As EventArgs)
        Public Event SelectionChanged(sender As Object, e As ToolTipSelectionChangedEventArgs)

        Private _entity As IEntity

        Public Sub New()
            InitializeComponent()

            ugConnector.DataSource = GetConnectorTable()
            ugSupplements.DataSource = GetSupplementTable()
            PropertiesListView.ItemSettings.SelectionType = SelectionType.Single
            PropertiesListView.View = UltraListViewStyle.Details
        End Sub

        Private Function GetConnectorTable() As DataTable
            Dim connectorTable As New DataTable
            Dim connectorGridAppearance As D3DConnectorTableAppearance = GridAppearance.All.OfType(Of D3DConnectorTableAppearance).Single
            Dim sortedGridColumns As New SortedDictionary(Of Integer, GridColumn)

            For Each gridColumn As GridColumn In connectorGridAppearance.GridTable.GridColumns
                sortedGridColumns.Add(gridColumn.Ordinal, gridColumn)
            Next
            For Each gridColumn As GridColumn In sortedGridColumns.Values
                Dim col As New DataColumn(gridColumn.KBLPropertyName, GetType(String))
                connectorTable.Columns.Add(col)
            Next
            Return connectorTable
        End Function

        Private Function GetSupplementTable() As DataTable
            Dim supplementTable As New DataTable
            Dim supplementGridAppearance As D3DSupplementTableAppearance = GridAppearance.All.OfType(Of D3DSupplementTableAppearance).Single
            Dim sortedGridColumns As New SortedDictionary(Of Integer, GridColumn)

            For Each gridColumn As GridColumn In supplementGridAppearance.GridTable.GridColumns
                sortedGridColumns.Add(gridColumn.Ordinal, gridColumn)
            Next

            For Each gridColumn As GridColumn In sortedGridColumns.Values
                Dim col As New DataColumn(gridColumn.KBLPropertyName, GetType(String))
                supplementTable.Columns.Add(col)
            Next
            Return supplementTable
        End Function

        Friend Sub SetEntity(entity As IEntity, model As IEEModel)
            _entity = entity
            UpdateData(model)
            CreateVisibleProperties(model)
            UpdateProperties()
            PropertiesListView.SelectedItems.Clear()
            PropertiesListView.Select()
        End Sub

        Private Sub CreateVisibleProperties(model As IEEModel)
            Me.Properties.Clear()

            If TypeOf _entity Is IBaseModelEntityEx Then
                Dim table As GridTable = Nothing

                Select Case CType(_entity, IBaseModelEntityEx).EntityType
                    Case ModelEntityType.Bundle
                        table = GridAppearance.All.OfType(Of SegmentGridAppearance).SingleOrDefault?.GridTable
                    Case ModelEntityType.Node
                        table = GridAppearance.All.OfType(Of VertexGridAppearance).SingleOrDefault?.GridTable
                    Case ModelEntityType.Fixing
                        table = GridAppearance.All.OfType(Of FixingGridAppearance).SingleOrDefault?.GridTable
                    Case ModelEntityType.Protection
                        Dim segAppearance As SegmentGridAppearance = GridAppearance.All.OfType(Of SegmentGridAppearance).SingleOrDefault
                        If segAppearance IsNot Nothing Then
                            table = segAppearance.GridTable.GridSubTable
                        End If
                    Case ModelEntityType.Splice, ModelEntityType.Eylet, ModelEntityType.Connector
                        table = GridAppearance.All.OfType(Of ConnectorGridAppearance).SingleOrDefault?.GridTable
                    Case ModelEntityType.Supplement
                        table = GridAppearance.All.OfType(Of AccessoryGridAppearance).SingleOrDefault?.GridTable
                    Case ModelEntityType.Component
                        Dim component As E3.Lib.Model.Component = model.Containers.OfType(Of E3.Lib.Model.Component).Where(Function(cmp) cmp.Id = CType(_entity, IEEObjectIdProvider).GetEEObjectIds.FirstOrDefault).SingleOrDefault
                        If component IsNot Nothing Then
                            Select Case component.Class
                                Case ComponentClass.Fusebox
                                    table = GridAppearance.All.OfType(Of ComponentBoxGridAppearance).SingleOrDefault?.GridTable
                                Case Else
                                    table = GridAppearance.All.OfType(Of ComponentGridAppearance).SingleOrDefault?.GridTable
                            End Select
                        Else
                            'HINT: normally not shown to user, but as a safety fallback to get information that we have an error
                            MessageBoxEx.ShowError($"Component not found in model ({NameOf(CreateVisibleProperties)})")
                        End If
                    Case Else
                        'HINT: normally not shown to user, but as a safety fallback to get information that we have an error
                        MessageBoxEx.ShowError($"For Entity of Type ""{CType(_entity, IBaseModelEntityEx).EntityType.ToString}"" the property creation is not implemented! ({NameOf(CreateVisibleProperties)})")
                End Select

                Dim moduleInfo As String = GetModuleString(model)
                Dim instruction As String = GetIntructionString(model)
                Dim mountingDirection As String = GetMountingDirectionString(model)
                Dim fixingDirection As String = GetFixingDirectionString(model)

                If table IsNot Nothing Then

                    Dim excludedProps As New List(Of String)
                    excludedProps.Add([Lib].Schema.Kbl.OccurrencePropertyName.Id.ToString)
                    excludedProps.Add(PartPropertyName.Part_number.ToString)
                    excludedProps.Add(PartPropertyName.Copyright_note.ToString)
                    excludedProps.Add(PartPropertyName.External_references.ToString)
                    excludedProps.Add(PartPropertyName.Change.ToString)
                    excludedProps.Add(PartPropertyName.Degree_of_maturity.ToString)
                    excludedProps.Add(PartPropertyName.Part_Localized_description.ToString)
                    excludedProps.Add(PartPropertyName.Part_number_type.ToString)
                    excludedProps.Add(PartPropertyName.Predecessor_part_number.ToString)
                    excludedProps.Add(PartPropertyName.Material_information.ToString)
                    'HINT: here we might remove other properties which are not to be considerd in the view
                    'These are handled right now by some caught exceptions

                    Me.Properties.AddRange(GetPropertyViews(table, model, excludedProps.ToArray()).OrderBy(Function(pv) pv.Name))


                    If Not String.IsNullOrEmpty(fixingDirection) Then
                        Me.Properties.Add(New PropertyView(Document3DStrings.FixingDirectionText, fixingDirection))
                    End If

                    If Not String.IsNullOrEmpty(mountingDirection) Then
                        Me.Properties.Add(New PropertyView(Document3DStrings.MountingDirectionText, mountingDirection))
                    End If

                    If Not String.IsNullOrEmpty(instruction) Then
                        Me.Properties.Add(New PropertyView(Document3DStrings.InstructionsText, instruction))
                    End If

                    If Not String.IsNullOrEmpty(moduleInfo) Then
                        'TODO: this localization is somewhat makeshift....
                        Me.Properties.Add(New PropertyView(Me.ugConnector.DisplayLayout.Bands(0).Columns(HarnessAnalyzer.Shared.MODULE_KEY).Header.Caption, moduleInfo))
                    End If
                End If
            End If
        End Sub

        Private Function GetMountingDirectionString(model As IEEModel) As String
            'HINT. special gimmick Daimler clock values in intructions
            Dim obj As ObjectBaseNaming = CType(model(CType(_entity, IBaseModelEntityEx).GetEEObjectIds.FirstOrDefault), ObjectBaseNaming)
            Dim bag As KblPropertyBagBase = obj.CustomAttributes.OfType(Of KblPropertyBagAttribute).SingleOrDefault?.PropertyBag
            If bag IsNot Nothing Then
                Select Case obj.HostContainerId
                    Case ContainerId.AdditionalParts
                        Return CType(bag, KblFixingPropertyBag).MountingDirection
                End Select
            End If
            Return String.Empty
        End Function

        Private Function GetFixingDirectionString(model As E3.Lib.Model.IEEModel) As String
            'HINT. special gimmick Daimler clock values in intructions
            Dim obj As ObjectBaseNaming = CType(model(CType(_entity, IBaseModelEntityEx).GetEEObjectIds.FirstOrDefault), ObjectBaseNaming)
            Dim bag As KblPropertyBagBase = obj.CustomAttributes.OfType(Of KblPropertyBagAttribute).SingleOrDefault?.PropertyBag
            If bag IsNot Nothing Then
                Select Case obj.HostContainerId
                    Case ContainerId.AdditionalParts
                        Return CType(bag, KblFixingPropertyBag).FixingDirection
                End Select
            End If
            Return String.Empty
        End Function

        Private Function GetIntructionString(model As E3.Lib.Model.IEEModel) As String
            'HINT. special gimmick Daimler instructions from processing informations or installation instructions
            Dim obj As ObjectBaseNaming = CType(model(CType(_entity, IBaseModelEntityEx).GetEEObjectIds.FirstOrDefault), ObjectBaseNaming)
            Dim bag As KblPropertyBagBase = obj.CustomAttributes.OfType(Of KblPropertyBagAttribute).SingleOrDefault?.PropertyBag
            If bag IsNot Nothing Then
                Select Case obj.HostContainerId
                    Case ContainerId.Connectors
                        Return RemoveLF(CType(bag, KblConnectorPropertyBag).Instruction)
                    Case ContainerId.Components
                        Return RemoveLF(CType(bag, KblComponentPropertyBag).Instruction)
                    Case ContainerId.Protections
                        Return RemoveLF(CType(bag, KblProtectionPropertyBag).Instruction)
                    Case ContainerId.AdditionalParts
                        Return RemoveLF(CType(bag, KblFixingPropertyBag).Instruction)
                    Case ContainerId.Supplements
                        Return RemoveLF(CType(bag, KblAccessoryPropertyBag).Instruction)
                    Case ContainerId.Segments
                        Return RemoveLF(CType(bag, KblSegmentPropertyBag).Instruction)
                    Case ContainerId.Vertices
                        Return RemoveLF(CType(bag, KblNodePropertyBag).Instruction)
                End Select
            End If
            Return String.Empty
        End Function

        Private Function RemoveLF(s As String) As String
            Return s.Replace(vbLf, "; ").Trim()
        End Function

        Private Function GetModuleString(model As E3.Lib.Model.IEEModel) As String
            Dim obj As ObjectBaseNaming = CType(model(CType(_entity, IBaseModelEntityEx).GetEEObjectIds.FirstOrDefault), ObjectBaseNaming)
            Select Case obj.HostContainerId
                Case ContainerId.Connectors
                    Return JoinModules(CType(obj, E3.Lib.Model.Connector).GetModules.Entries.Select(Function(m) m.ShortName))
                Case ContainerId.Components
                    Return JoinModules(CType(obj, E3.Lib.Model.Component).GetModules.Entries.Select(Function(m) m.ShortName))
                Case ContainerId.Protections
                    Return JoinModules(CType(obj, E3.Lib.Model.Protection).GetModules.Entries.Select(Function(m) m.ShortName))
                Case ContainerId.AdditionalParts
                    Return JoinModules(CType(obj, E3.Lib.Model.AdditionalPart).GetModules.Entries.Select(Function(m) m.ShortName))
                Case ContainerId.Supplements
                    Return JoinModules(CType(obj, E3.Lib.Model.Supplement).GetModules.Entries.Select(Function(m) m.ShortName))
            End Select
            Return String.Empty
        End Function

        Private Function JoinModules(modules As IEnumerable(Of String)) As String
            Return String.Join(";", modules).Trim()
        End Function

        Private Function GetPropertyViews(table As GridTable, model As E3.Lib.Model.IEEModel, ParamArray excludePropertyNames As String()) As List(Of PropertyView)
            Dim obj As ObjectBaseNaming = CType(model(CType(_entity, IBaseModelEntityEx).GetEEObjectIds.FirstOrDefault), ObjectBaseNaming)
            Dim list As New List(Of PropertyView)
            If obj IsNot Nothing Then
                For Each col As GridColumn In table.GridColumns.OrderBy(Function(c) c.Ordinal).ToArray
                    If Not excludePropertyNames.Contains(col.KBLPropertyName) Then
                        Dim failed As KblPropertyResolveFailedType
                        Dim valueStr As String = obj.GetValueOrExceptionFromKblPropertyName(col.KBLPropertyName, failed)
                        Dim kblid As String = String.Empty
                        If col.KBLPropertyName = SegmentPropertyName.Start_node.ToString AndAlso (TypeOf (obj) Is Supplement Or TypeOf (obj) Is AdditionalPart Or TypeOf (obj) Is Protection) Then
                            kblid = obj.GetValueFromKblPropertyName(CustomPropertyNames.StartNodeId.ToString)
                        End If
                        list.Add(New PropertyView(col.Name, valueStr, col.D3D.PropertyVisible, failed <> KblPropertyResolveFailedType.None, kblid))
                    End If
                Next
            End If
            Return list
        End Function

        Private Sub UpdateProperties()
            PropertiesListView.Items.Clear()

            For Each p As PropertyView In Me.Properties
                If p.Visible Then
                    Dim item As New UltraListViewItem(p.Name, New Object() {p.Value})
                    PropertiesListView.Items.Add(item)
                    If p.Failed Then
                        item.Appearance.ForeColor = Drawing.Color.Red
                    End If
                    item.Tag = p.KblId
                    If EnableCrossHighlight AndAlso Not String.IsNullOrEmpty(p.KblId) AndAlso item.SubItems.Count > 0 Then
                        item.SubItems(0).Appearance.Image = My.Resources.Show
                    End If
                End If
            Next

            PropertiesListView.MainColumn.PerformAutoResize(Infragistics.Win.UltraWinListView.ColumnAutoSizeMode.AllItemsAndHeader)
        End Sub

        Private Function GetCaptionText(entity As BaseModelEntity, obj As IObjectBase) As String
            If obj IsNot Nothing Then
                Return String.Format("{0} ({1})", entity.DisplayName, Utils.GetContainerIdText(obj.HostContainerId, Utils.TextCase.ProperCase))
            Else
                Return Document3DStrings.ModelObjectNotFoundErrorText
            End If
        End Function

        Private Sub UpdateData(model As IEEModel)
            Me.Text = Nothing
            PartNumberTextBox.Text = Nothing

            If TypeOf _entity Is BaseModelEntity AndAlso model IsNot Nothing Then
                With CType(_entity, BaseModelEntity)
                    Dim obj As IObjectBase = model(CType(_entity, IBaseModelEntityEx).GetEEObjectIds.FirstOrDefault)
                    Me.Text = GetCaptionText(CType(_entity, BaseModelEntity), obj)

                    If obj IsNot Nothing Then
                        If TypeOf obj Is ObjectBaseNaming Then
                            PartNumberTextBox.Text = KblToModelResolver.ResolvePartValueString(PartPropertyName.Part_number, CType(obj, ObjectBaseNaming))
                        End If

                        Select Case .EntityType
                            Case ModelEntityType.Splice, ModelEntityType.Eylet, ModelEntityType.Connector
                                If TypeOf _entity Is IBaseModelEntityEx Then
                                    Me.GroupBox1.Dock = DockStyle.Top
                                    Me.UltraSplitter1.Visible = True
                                    Me.UltraSplitter2.Visible = False
                                    Me.ugConnector.Visible = True
                                    Me.ugSupplements.Visible = False

                                    Dim conn As Connector = TryCast(obj, Connector)
                                    If conn IsNot Nothing Then
                                        UpdateConnectorGrid(conn)
                                        If UpdateSupplementGrid(conn) Then
                                            Me.UltraSplitter2.Visible = True
                                            Me.ugSupplements.Visible = True
                                        End If
                                    End If
                                End If
                            Case Else
                                Me.GroupBox1.Dock = DockStyle.Fill
                                Me.UltraSplitter1.Visible = False
                                Me.UltraSplitter2.Visible = False
                                Me.ugConnector.Visible = False
                                Me.ugSupplements.Visible = False
                        End Select
                    End If
                End With
            End If
        End Sub

        Private Sub UpdateConnectorGrid(conn As Connector)
            ugConnector.BeginUpdate()
            Dim table As DataTable = CType(ugConnector.DataSource, DataTable)
            table.Rows.Clear()
            For Each cav As E3.Lib.Model.Cavity In conn.GetCavities.Entries
                Dim terminals As String = String.Join(";", cav.GetTerminals.Entries.Select(Function(m) m.PartNumber))
                Dim seals As String = String.Join(";", cav.GetSeals.Entries.Select(Function(m) m.PartNumber))
                Dim plugs As String = String.Join(";", cav.GetPlugs.Entries.Select(Function(m) m.PartNumber))
                Dim platingMaterials As String = String.Join(";", cav.GetTerminals.Entries.Select(Function(ter) ter.PlatingMaterial))

                If cav.GetWires.Entries.Count > 0 Then
                    For Each wire As E3.Lib.Model.Wire In cav.GetWires.Entries
                        Dim row As DataRow = table.NewRow
                        row.Item(ConnectorPropertyName.Cavity_number) = cav.ShortName
                        row.Item(ConnectorPropertyName.Terminal_part_number) = terminals
                        row.Item(ConnectorPropertyName.Seal_part_number) = seals
                        row.Item(ConnectorPropertyName.Plug_part_number) = plugs
                        row.Item(ConnectorPropertyName.Plating) = platingMaterials
                        row.Item(HarnessAnalyzer.[Shared].CORE_WIRE_NUMBER_KEY) = wire.GetValueOrExceptionFromKblPropertyName(WirePropertyName.Wire_number)
                        row.Item(WirePropertyName.Wire_type) = wire.GetValueOrExceptionFromKblPropertyName(WirePropertyName.Wire_type)
                        row.Item(WirePropertyName.Cross_section_area) = wire.GetValueOrExceptionFromKblPropertyName(WirePropertyName.Cross_section_area)
                        row.Item(WirePropertyName.Core_Colour) = wire.GetValueOrExceptionFromKblPropertyName(WirePropertyName.Cover_colour)
                        row.Item(HarnessAnalyzer.Shared.MODULE_KEY) = String.Join(";", wire.GetCable?.GetModules.Entries.Select(Function(m) m.ShortName))

                        table.Rows.Add(row)
                    Next
                Else
                    Dim row As DataRow = table.NewRow
                    row.Item(ConnectorPropertyName.Cavity_number) = cav.ShortName
                    row.Item(ConnectorPropertyName.Terminal_part_number) = terminals
                    row.Item(ConnectorPropertyName.Seal_part_number) = seals
                    row.Item(ConnectorPropertyName.Plug_part_number) = plugs
                    row.Item(ConnectorPropertyName.Plating) = platingMaterials
                    table.Rows.Add(row)
                End If
            Next
            ugConnector.EndUpdate()
        End Sub

        Private Function UpdateSupplementGrid(conn As Connector) As Boolean
            Dim table As DataTable = CType(ugSupplements.DataSource, DataTable)
            table.Rows.Clear()

            For Each supp As Supplement In conn.GetSupplements.Entries
                Dim row As DataRow = table.NewRow
                row.Item(AccessoryPropertyName.Id.ToString) = supp.ShortName
                row.Item(PartPropertyName.Part_description.ToString) = KblToModelResolver.ResolvePartValueString(PartPropertyName.Part_description, supp)
                row.Item(PartPropertyName.Part_number.ToString) = supp.PartNumber
                row.Item(HarnessAnalyzer.Shared.MODULE_KEY) = JoinModules(supp.GetModules.Entries.Select(Function(m) m.ShortName))
                table.Rows.Add(row)
            Next

            For Each comp As E3.Lib.Model.Component In conn.GetComponents.Entries
                Dim row As DataRow = table.NewRow
                row.Item(ComponentPropertyName.Id.ToString) = comp.ShortName
                'not accessible right now but same for pin number and place ?
                'row.Item(ComponentPropertyName.Colour.ToString) = KblToModelResolver.GetModelPropertyValueString(ComponentPropertyName.Colour.ToString, comp)
                row.Item(PartPropertyName.Part_description.ToString) = KblToModelResolver.ResolvePartValueString(PartPropertyName.Part_description, comp)
                row.Item(PartPropertyName.Part_number.ToString) = comp.PartNumber
                row.Item(HarnessAnalyzer.Shared.MODULE_KEY) = JoinModules(comp.GetModules.Entries.Select(Function(m) m.ShortName))
                table.Rows.Add(row)
            Next

            Return CBool(table.Rows.Count > 0)
        End Function

        Private Sub ContextMenuStrip1_Opening(sender As Object, e As CancelEventArgs) Handles ContextMenuStripConnector.Opening
            CopyConnectorDataMenuItem.Enabled = ugConnector.Selected.Rows.Count > 0
        End Sub

        Private Sub CopyToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CopyConnectorDataMenuItem.Click
            ugConnector.PerformAction(UltraGridAction.Copy)
        End Sub

        Private Sub ContextMenuStripProperties_Opening(sender As Object, e As CancelEventArgs) Handles ContextMenuStripProperties.Opening
            CopyPropertiesMenuItem.Enabled = PropertiesListView.SelectedItems.Count > 0
        End Sub

        Private Sub CopyPropertiesMenuItem_Click(sender As Object, e As EventArgs) Handles CopyPropertiesMenuItem.Click
            Dim lines As New System.Text.StringBuilder
            For Each item As UltraListViewItem In PropertiesListView.SelectedItems
                lines.AppendLine(String.Join(vbTab, {item.Text}.Concat(item.SubItems.OfType(Of UltraListViewSubItem).Select(Function(subItem) subItem.Text))))
            Next
            If lines.Length > 0 Then
                Clipboard.SetText(lines.ToString)
            End If
        End Sub

        Private Sub TextBoxToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles TextBoxToolStripMenuItem.Click
            Dim textBox As TextBox = DirectCast(DirectCast(DirectCast(sender, ToolStripMenuItem).Owner, ContextMenuStrip).SourceControl, TextBox)
            textBox.Copy()
        End Sub

        Private Sub ContextMenuStripTextBox_Opening(sender As Object, e As CancelEventArgs) Handles ContextMenuStripTextBox.Opening
            Dim textBox As TextBox = CType(ContextMenuStripTextBox.SourceControl, TextBox)
            TextBoxToolStripMenuItem.Enabled = textBox.SelectionLength > 0
        End Sub

        Private Sub ConnectorHousingUltraGrid_InitializeLayout(sender As Object, e As InitializeLayoutEventArgs) Handles ugConnector.InitializeLayout
            ugConnector.BeginUpdate()
            ugConnector.EventManager.AllEventsEnabled = False

            With e.Layout.Bands(0).Columns(ConnectorPropertyName.Cavity_number.ToString)
                .SortComparer = New NumericStringSortComparer
                .SortIndicator = SortIndicator.Ascending
            End With

            Dim connectorGridAppearance As D3DConnectorTableAppearance = GridAppearance.All.OfType(Of D3DConnectorTableAppearance).Single
            For Each gridCol As GridColumn In connectorGridAppearance.GridTable.GridColumns
                If (e.Layout.Bands(0).Columns.Exists(gridCol.KBLPropertyName)) Then
                    With e.Layout.Bands(0).Columns(gridCol.KBLPropertyName)
                        .Header.Caption = gridCol.Name
                        .Hidden = Not gridCol.Visible
                    End With
                End If
            Next

            ugConnector.EventManager.AllEventsEnabled = True
            ugConnector.EndUpdate()

        End Sub

        Private Sub ugSupplements_InitializeLayout(sender As Object, e As InitializeLayoutEventArgs) Handles ugSupplements.InitializeLayout
            ugSupplements.BeginUpdate()
            ugSupplements.EventManager.AllEventsEnabled = False

            Dim supplementGridAppearance As D3DSupplementTableAppearance = GridAppearance.All.OfType(Of D3DSupplementTableAppearance).Single
            For Each gridCol As GridColumn In supplementGridAppearance.GridTable.GridColumns
                If (e.Layout.Bands(0).Columns.Exists(gridCol.KBLPropertyName)) Then
                    With e.Layout.Bands(0).Columns(gridCol.KBLPropertyName)
                        .Header.Caption = gridCol.Name
                        .Hidden = Not gridCol.Visible
                    End With
                End If
            Next

            ugSupplements.EventManager.AllEventsEnabled = True
            ugSupplements.EndUpdate()
        End Sub

        Private Sub PropertiesListView_ItemSelectionChanged(sender As Object, e As ItemSelectionChangedEventArgs) Handles PropertiesListView.ItemSelectionChanged
            If e.SelectedItems IsNot Nothing AndAlso e.SelectedItems.Count = 1 Then
                OnSelectionChanged(New ToolTipSelectionChangedEventArgs(Me, e.SelectedItems(0).Tag.ToString))
            Else
                OnSelectionChanged(New ToolTipSelectionChangedEventArgs(Me))
            End If
        End Sub

        Protected Overridable Sub OnSelectionChanged(e As ToolTipSelectionChangedEventArgs)
            RaiseEvent SelectionChanged(Me, e)
        End Sub

        Public ReadOnly Property EnableCrossHighlight As Boolean
            Get
                Return TypeOf Me.Parent IsNot D3DComparerCntrl
            End Get
        End Property

        ReadOnly Property IsAllowedToClose As Boolean
            Get
                Return Not (ContextMenuStripConnector?.Visible).GetValueOrDefault AndAlso
                    Not (ContextMenuStripProperties?.Visible).GetValueOrDefault AndAlso
                    Not (ContextMenuStripTextBox?.Visible).GetValueOrDefault
            End Get
        End Property

        Public ReadOnly Property Entity As IEntity
            Get
                Return _entity
            End Get
        End Property

        ReadOnly Property Properties As New List(Of PropertyView)

    End Class

End Namespace