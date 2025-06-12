Imports System.Text.RegularExpressions
Imports Infragistics.Win.UltraWinToolbars
Imports Infragistics.Win.UltraWinTree
Imports Zuken.E3.HarnessAnalyzer.Settings
Imports Zuken.E3.HarnessAnalyzer.Shared.Common

<Reflection.Obfuscation(Feature:="renaming", ApplyToMembers:=False, Exclude:=True)> ' needed to avoid resource-file-names obfuscation errors, see issue #5
Public Class ModulesHub

    Friend Event ActiveModulesChanged(sender As ModulesHub, e As ModulesHubEventArgs)
    Friend Event ModuleSelectionChanged(sender As ModulesHub, e As ModulesHubEventArgs)

    Private _allExpanded As Boolean
    Private _contextMenuTree As PopupMenuTool
    Private _harnessModuleConfigurations As HarnessModuleConfigurationCollection
    Private _harnessNode As UltraTreeNode
    Private _isDirty As Boolean
    Private _kblMapper As KblMapper
    Private _moduleIds As List(Of String)
    Private _modulesHubEventArgs As ModulesHubEventArgs
    Private _systemChange As Boolean
    Private _strgDown As Boolean = False
    Private _currentSelection As New List(Of String)
    Private _settings As IHarnessAnalyzerSettingsProvider

    Friend WithEvents _blinkingTimer As Timer

    Public Sub New(isTouchEnabled As Boolean, settings As IHarnessAnalyzerSettingsProvider)
        InitializeComponent()

        Me.BackColor = Color.White
        Me.upnButton.Visible = False

        _settings = settings
        _blinkingTimer = New Timer()
        _blinkingTimer.Interval = 500
        _modulesHubEventArgs = New ModulesHubEventArgs

        Me.utchpModulesHub.Enabled = isTouchEnabled

        InitializeContextMenu()
    End Sub

    Friend Shared Function GetNodeText(abbreviation As String, modulePartNumber As String) As String
        abbreviation = (Regex.Replace(abbreviation, "\t|\r|\v|\n", String.Empty)).Trim

        If (abbreviation <> String.Empty) Then
            Return String.Format("{0} [{1}]", abbreviation, modulePartNumber.Trim)
        End If

        Return modulePartNumber.Trim
    End Function

    Friend Function InitializeConfigurations(harnessModuleConfigurations As HarnessModuleConfigurationCollection, Optional moduleConfigName As String = "") As Boolean
        Try
            Me.uceModuleConfigs.BeginUpdate()
            Me.uceModuleConfigs.Items.Clear()

            If (moduleConfigName = String.Empty) Then
                moduleConfigName = KblObjectType.Custom.ToLocalizedString
            End If

            _harnessModuleConfigurations = harnessModuleConfigurations

            Dim activeItem As Infragistics.Win.ValueListItem = Nothing

            For Each harnessModuleConfig As HarnessModuleConfiguration In _harnessModuleConfigurations
                If (Regex.Replace(harnessModuleConfig.HarnessConfiguration.Part_number, "\s", String.Empty) <> String.Empty) Then
                    Me.uceModuleConfigs.Items.Add(harnessModuleConfig.HarnessConfiguration, If(harnessModuleConfig.HarnessConfiguration.Abbreviation IsNot Nothing AndAlso harnessModuleConfig.HarnessConfiguration.Abbreviation <> String.Empty, String.Format("{0} [{1}]", harnessModuleConfig.HarnessConfiguration.Abbreviation, harnessModuleConfig.HarnessConfiguration.Part_number), harnessModuleConfig.HarnessConfiguration.Part_number))

                    If (harnessModuleConfig.HarnessConfiguration.Part_number = moduleConfigName) Then
                        activeItem = Me.uceModuleConfigs.Items(Me.uceModuleConfigs.Items.Count - 1)
                        harnessModuleConfig.IsActive = True
                    Else
                        harnessModuleConfig.IsActive = False
                    End If
                ElseIf (Regex.Replace(harnessModuleConfig.HarnessConfiguration.Abbreviation, "\s", String.Empty) <> String.Empty) Then
                    Me.uceModuleConfigs.Items.Add(harnessModuleConfig.HarnessConfiguration, harnessModuleConfig.HarnessConfiguration.Abbreviation)

                    If (harnessModuleConfig.HarnessConfiguration.Abbreviation = moduleConfigName) Then
                        activeItem = Me.uceModuleConfigs.Items(Me.uceModuleConfigs.Items.Count - 1)
                        harnessModuleConfig.IsActive = True
                    Else
                        harnessModuleConfig.IsActive = False
                    End If
                ElseIf (Regex.Replace(harnessModuleConfig.HarnessConfiguration.Description, "\s", String.Empty) <> String.Empty) Then
                    Me.uceModuleConfigs.Items.Add(harnessModuleConfig.HarnessConfiguration, harnessModuleConfig.HarnessConfiguration.Description)

                    If (harnessModuleConfig.HarnessConfiguration.Description = moduleConfigName) Then
                        activeItem = Me.uceModuleConfigs.Items(Me.uceModuleConfigs.Items.Count - 1)
                        harnessModuleConfig.IsActive = True
                    Else
                        harnessModuleConfig.IsActive = False
                    End If
                End If
            Next

            Me.uceModuleConfigs.SortStyle = Infragistics.Win.ValueListSortStyle.Ascending
            Me.uceModuleConfigs.SelectedItem = activeItem
            Me.uceModuleConfigs.EndUpdate()

        Catch ex As Exception
            MessageBox.Show(String.Format(ModulesHubStrings.LoadConfigFailed_Msg, vbCrLf, ex.Message), [Shared].MSG_BOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error)

            Me.uceModuleConfigs.EndUpdate()

            Return False
        End Try

        Return True
    End Function

    Friend Function InitializeTree(kblMapper As KblMapper, modulePartNumbersForActivation As List(Of String)) As Boolean
        Try
            _kblMapper = kblMapper

            Me.utModules.BeginUpdate()
            Me.utModules.EventManager.AllEventsEnabled = False
            Me.utModules.HideSelection = False

            Dim modulesForActivation As New List(Of [Module])

            For Each modulePartNumberForActivation As String In modulePartNumbersForActivation
                For Each [module] As [Module] In _kblMapper.GetModules.Where(Function([mod]) [mod].Part_number.Trim.Replace(" ", String.Empty) = modulePartNumberForActivation)
                    modulesForActivation.Add([module])
                Next
            Next

            _harnessNode = New UltraTreeNode(_kblMapper.GetHarness.SystemId, String.Format(ModulesHubStrings.Harness_NodeTxt, _kblMapper.HarnessPartNumber))

            If (modulesForActivation.Count <> 0) AndAlso (modulesForActivation.Count <> _kblMapper.GetModules.Count) Then
                _harnessNode.CheckedState = CheckState.Indeterminate
            Else
                _harnessNode.CheckedState = CheckState.Checked
            End If

            _harnessNode.LeftImages.Add(My.Resources.Harness.ToBitmap)
            _harnessNode.Override.NodeStyle = NodeStyle.CheckBoxTriState

            Me.utModules.Nodes.Add(_harnessNode)

            Me.lblHarnessInfo.Text = String.Format(ModulesHubStrings.HarnessInfo, _kblMapper.HarnessPartNumber)

            For Each moduleFamily As Module_family In _kblMapper.GetModuleFamilies
                Dim moduleFamilyNode As New UltraTreeNode(moduleFamily.SystemId, moduleFamily.Id)

                moduleFamilyNode.CheckedState = CheckState.Checked
                moduleFamilyNode.LeftImages.Add(My.Resources.ModuleFamily.ToBitmap)
                moduleFamilyNode.Override.NodeStyle = NodeStyle.CheckBoxTriState

                _harnessNode.Nodes.Add(moduleFamilyNode)
            Next

            _moduleIds = New List(Of String)

            For Each [module] As [Module] In _kblMapper.GetModules
                Dim moduleNode As New UltraTreeNode([module].SystemId, GetNodeText([module].Abbreviation, [module].Part_number))
                moduleNode.CheckedState = CheckState.Checked
                moduleNode.LeftImages.Add(My.Resources.HarnessModule.ToBitmap)
                moduleNode.Override.NodeStyle = NodeStyle.CheckBox
                moduleNode.Tag = [module]

                If ([module].Of_family <> String.Empty) AndAlso (Me.utModules.GetNodeByKey([module].Of_family) IsNot Nothing) Then
                    Me.utModules.GetNodeByKey([module].Of_family).Nodes.Add(moduleNode)

                    If (modulePartNumbersForActivation.Count <> 0) AndAlso (Not modulePartNumbersForActivation.Contains([module].Part_number.Trim.Replace(" ", String.Empty))) Then
                        moduleNode.CheckedState = CheckState.Unchecked
                        moduleNode.Parent.CheckedState = CheckState.Indeterminate

                        Dim allChildNodesUnchecked As Boolean = True

                        For Each childNode As UltraTreeNode In moduleNode.Parent.Nodes
                            If (childNode.CheckedState = CheckState.Checked) Then
                                allChildNodesUnchecked = False

                                Exit For
                            End If
                        Next

                        If (allChildNodesUnchecked) Then moduleNode.Parent.CheckedState = CheckState.Unchecked
                    End If
                Else
                    _harnessNode.Nodes.Add(moduleNode)
                End If

                _moduleIds.Add([module].SystemId)
            Next

            _allExpanded = True

            Me.utModules.ExpandAll()

            Me.utModules.Override.SelectionType = SelectType.Extended
            Me.utModules.Override.SortComparer = New NumericStringSortComparer(My.Settings.ConsiderCheckStateForModuleTreeSorting)
            Me.utModules.Override.Sort = SortType.Ascending
            Me.utModules.SelectionBehavior = SelectionBehavior.ExtendedAcrossCollections

            Me.utModules.EventManager.AllEventsEnabled = True
            Me.utModules.EndUpdate()
        Catch ex As Exception
            MessageBox.Show(String.Format(ModulesHubStrings.LoadModTreeFailed_Msg, vbCrLf, ex.Message), [Shared].MSG_BOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error)

            Me.utModules.EventManager.AllEventsEnabled = True
            Me.utModules.EndUpdate()

            Return False
        End Try

        Return True
    End Function

    Friend Sub UpdateTreeNodes_Recursively(parentNode As UltraTreeNode)
        If (parentNode Is Nothing) Then
            For Each node As UltraTreeNode In Me.utModules.Nodes
                If (TypeOf node.Tag Is [Module]) Then
                    _systemChange = True

                    If (_kblMapper.InactiveModules.ContainsKey(node.Key)) Then
                        node.CheckedState = CheckState.Unchecked
                    Else
                        node.CheckedState = CheckState.Checked
                    End If

                    _systemChange = False
                ElseIf (node.Nodes.Count <> 0) Then
                    UpdateTreeNodes_Recursively(node)
                End If
            Next
        Else
            For Each node As UltraTreeNode In parentNode.Nodes
                If (TypeOf node.Tag Is [Module]) Then
                    _systemChange = True

                    If (_kblMapper.InactiveModules.ContainsKey(node.Key)) Then
                        node.CheckedState = CheckState.Unchecked
                    Else
                        node.CheckedState = CheckState.Checked
                    End If

                    _systemChange = False
                ElseIf (node.Nodes.Count <> 0) Then
                    UpdateTreeNodes_Recursively(node)
                End If
            Next
        End If

        Me.utModules.RefreshSort()
    End Sub

    Private Function AllNodes() As List(Of UltraTreeNode)
        Dim nodes As New List(Of UltraTreeNode)
        GetAllNodesRecursively(Me.utModules.Nodes(0), nodes)
        Return nodes
    End Function

    Private Sub GetAllNodesRecursively(currentNode As UltraTreeNode, ByRef nodes As List(Of UltraTreeNode))
        For Each node As UltraTreeNode In currentNode.Nodes
            nodes.Add(node)
            If node.HasNodes Then GetAllNodesRecursively(node, nodes)
        Next
    End Sub

    Private Sub InitializeContextMenu()
        _contextMenuTree = New PopupMenuTool("TreeContextMenu")

        With _contextMenuTree
            .DropDownArrowStyle = DropDownArrowStyle.None

            Dim considerCheckStateForSortingButton As New StateButtonTool(ContextKey.ConsiderCheckStateForSorting.ToString)
            considerCheckStateForSortingButton.Checked = My.Settings.ConsiderCheckStateForModuleTreeSorting
            considerCheckStateForSortingButton.SharedProps.Caption = If(My.Settings.ConsiderCheckStateForModuleTreeSorting, ModulesHubStrings.ConsiderAlphanumericOrderForSorting, ModulesHubStrings.ConsiderCheckStateForSorting)
            considerCheckStateForSortingButton.SharedProps.AppearancesSmall.Appearance.Image = My.Resources.ConsiderCheckStateForSorting.ToBitmap

            Dim checkSelectOnlyButton As New ButtonTool(ContextKey.CheckSelectOnly.ToString)
            With checkSelectOnlyButton.SharedProps
                .Caption = ModulesHubStrings.CheckSelectOnly
                .AppearancesSmall.Appearance.Image = My.Resources.checkbox
            End With

            Dim uncheckAllButton As New ButtonTool(ContextKey.UncheckAll.ToString)
            With uncheckAllButton.SharedProps
                .Caption = ModulesHubStrings.UnCheckAll
                .AppearancesSmall.Appearance.Image = My.Resources.checkbox_unchecked
            End With

            Dim compareModulesButton As New ButtonTool(ContextKey.CompareModules.ToString)
            With compareModulesButton.SharedProps
                .Caption = ModulesHubStrings.CompareModules
                .AppearancesSmall.Appearance.Image = My.Resources.CompareComposite
            End With


            Me.utmModulesHub.Tools.AddRange(New ToolBase() {_contextMenuTree, considerCheckStateForSortingButton, checkSelectOnlyButton, uncheckAllButton, compareModulesButton})

            .Tools.AddTool(considerCheckStateForSortingButton.Key)
            .Tools.AddTool(checkSelectOnlyButton.Key)
            .Tools.AddTool(uncheckAllButton.Key)
            .Tools.AddTool(compareModulesButton.Key)

        End With
    End Sub

    Private Sub SelectAll(sender As Object, e As EventArgs)
    End Sub

    Private Sub DeSelectAll(sender As Object, e As EventArgs)
        For Each nd As UltraTreeNode In _utModules.Nodes
            nd.CheckedState = CheckState.Unchecked
        Next
        _utModules.Invalidate()
    End Sub

    Private Sub UpdateParentNodeCheckState(parentNode As UltraTreeNode)
        Dim hasCheckedNodes As Boolean = False
        Dim hasUncheckedNodes As Boolean = False

        For Each childNode As UltraTreeNode In parentNode.Nodes
            If (childNode.CheckedState = CheckState.Checked) Then hasCheckedNodes = True

            If (childNode.CheckedState = CheckState.Indeterminate) Then
                parentNode.CheckedState = CheckState.Indeterminate

                Exit Sub
            End If

            If (childNode.CheckedState = CheckState.Unchecked) Then
                hasUncheckedNodes = True
            End If
        Next

        If (hasCheckedNodes AndAlso Not hasUncheckedNodes) Then
            parentNode.CheckedState = CheckState.Checked
        End If
        If (Not hasCheckedNodes AndAlso hasUncheckedNodes) Then
            parentNode.CheckedState = CheckState.Unchecked
        End If
        If (hasCheckedNodes AndAlso hasUncheckedNodes) Then
            parentNode.CheckedState = CheckState.Indeterminate
        End If
    End Sub

    Private Sub _blinkingTimer_Tick(sender As Object, e As EventArgs) Handles _blinkingTimer.Tick
        If (Me.upnButton.Visible) Then
            If (Me.btnApply.Appearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True) Then
                Me.btnApply.Appearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.False
                Me.btnApply.Appearance.ForeColor = Color.Black
            Else
                Me.btnApply.Appearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True
                Me.btnApply.Appearance.ForeColor = Color.Red
            End If
        End If
    End Sub

    Private Sub btnApply_Click(sender As Object, e As EventArgs) Handles btnApply.Click
        Me.Cursor = Cursors.WaitCursor

        If (Me.uceModuleConfigs.SelectedItem IsNot Nothing) Then
            _kblMapper.InactiveModules.Clear()

            For Each node As UltraTreeNode In _harnessNode.Nodes
                If (node.Tag IsNot Nothing) AndAlso (node.CheckedState = CheckState.Unchecked) Then
                    _kblMapper.InactiveModules.Add(node.Key, DirectCast(node.Tag, [Module]))
                Else
                    For Each subNode As UltraTreeNode In node.Nodes
                        If (subNode.Tag IsNot Nothing) AndAlso (subNode.CheckedState = CheckState.Unchecked) Then
                            _kblMapper.InactiveModules.Add(subNode.Key, DirectCast(subNode.Tag, [Module]))
                        End If
                    Next
                End If
            Next

            For Each harnessModuleConfig As HarnessModuleConfiguration In _harnessModuleConfigurations
                If (harnessModuleConfig.HarnessConfiguration.Equals(Me.uceModuleConfigs.SelectedItem.DataValue)) Then
                    harnessModuleConfig.IsActive = True
                Else
                    harnessModuleConfig.IsActive = False
                End If
            Next

            If (Me.uceModuleConfigs.Text = KblObjectType.Custom.ToLocalizedString) Then
                Dim harnessConfig As Harness_configuration = TryCast(Me.uceModuleConfigs.SelectedItem.DataValue, Harness_configuration)
                harnessConfig.Modules = String.Empty

                For Each moduleId As String In _moduleIds
                    If (Not _kblMapper.InactiveModules.ContainsKey(moduleId)) Then
                        If (harnessConfig.Modules = String.Empty) Then
                            harnessConfig.Modules = moduleId
                        Else
                            harnessConfig.Modules = String.Format("{0} {1}", harnessConfig.Modules, moduleId)
                        End If
                    End If
                Next
            End If

            _modulesHubEventArgs.HarnessConfig = DirectCast(Me.uceModuleConfigs.SelectedItem.DataValue, Harness_configuration)
            _modulesHubEventArgs.IsDirty = _isDirty

            RaiseEvent ActiveModulesChanged(Me, _modulesHubEventArgs)
        End If

        _blinkingTimer.Stop()

        Me.btnApply.Appearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.False
        Me.btnApply.Appearance.ForeColor = Color.Black

        Me.upnButton.Visible = False

        _isDirty = False

        Me.Cursor = Cursors.Default
    End Sub

    Private Sub uceModuleConfigs_SelectionChanged(sender As Object, e As EventArgs) Handles uceModuleConfigs.SelectionChanged
        If (Me.uceModuleConfigs.SelectedItem IsNot Nothing) Then
            Dim harnessConfig As Harness_configuration = TryCast(Me.uceModuleConfigs.SelectedItem.DataValue, Harness_configuration)
            If (harnessConfig IsNot Nothing) AndAlso (Not _systemChange) Then
                Dim moduleIds As List(Of String) = harnessConfig.Modules.SplitSpace.ToList
                Dim oldInactiveModules As List(Of KeyValuePair(Of String, [Module])) = _kblMapper.InactiveModules.ToList 'HINT: we must create a copy here because the inactive modules dic is changed in the next steps and maybe this collection is recycled afterwards (decided by BL) -> only pointer to the dictionary just would result in a changed dictionary after the following steps

                _kblMapper.InactiveModules.Clear()

                For Each moduleId As String In _moduleIds
                    If (Not moduleIds.Contains(moduleId)) Then
                        _kblMapper.InactiveModules.Add(moduleId, Nothing)
                    End If
                Next

                UpdateTreeNodes_Recursively(Nothing)

                If (Me.Enabled) Then
                    _kblMapper.InactiveModules = oldInactiveModules.ToDictionary(Function(kv) kv.Key, Function(kv) kv.Value)
                Else
                    _kblMapper.InactiveModules.Clear()
                End If
            End If
        End If

    End Sub

    Private Sub uceModuleConfigs_GotFocus(sender As Object, e As EventArgs) Handles uceModuleConfigs.GotFocus
        _blinkingTimer.Stop()

        Me.btnApply.Appearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.False
        Me.btnApply.Appearance.ForeColor = Color.Black
    End Sub

    Private Sub uceModuleConfigs_LostFocus(sender As Object, e As EventArgs) Handles uceModuleConfigs.LostFocus
        _blinkingTimer.Start()
    End Sub

    Private Sub utModules_AfterCheck(sender As Object, e As NodeEventArgs) Handles utModules.AfterCheck
        Me.upnButton.Visible = True

        Me.utModules.BeginUpdate()
        Me.utModules.EventManager.AllEventsEnabled = False

        If (e.TreeNode.HasNodes) Then
            For Each childNode As UltraTreeNode In e.TreeNode.Nodes
                childNode.CheckedState = e.TreeNode.CheckedState

                If (childNode.HasNodes) Then
                    For Each subChildNode As UltraTreeNode In childNode.Nodes
                        subChildNode.CheckedState = e.TreeNode.CheckedState
                    Next
                End If
            Next
        End If

        If (e.TreeNode.Parent IsNot Nothing) Then
            UpdateParentNodeCheckState(e.TreeNode.Parent)

            If (e.TreeNode.Parent.Parent IsNot Nothing) Then UpdateParentNodeCheckState(e.TreeNode.Parent.Parent)
        End If

        Me.utModules.EventManager.AllEventsEnabled = True

        If (Not _systemChange) Then Me.utModules.RefreshSort()

        Me.utModules.EndUpdate()

        If (Not _systemChange) Then
            _isDirty = True
            _systemChange = True

            Me.uceModuleConfigs.Text = KblObjectType.Custom.ToLocalizedString

            _systemChange = False
        End If
    End Sub

    Private Sub utModules_AfterSelect(sender As Object, e As SelectEventArgs) Handles utModules.AfterSelect
        If Not _strgDown Then

            Dim moduleIds As List(Of String) = GetSelection()

            Me.Update()
            If Me.uceModuleConfigs.SelectedItem IsNot Nothing Then RaiseEvent ModuleSelectionChanged(Me, New ModulesHubEventArgs(DirectCast(Me.uceModuleConfigs.SelectedItem.DataValue, Harness_configuration), _isDirty, moduleIds))
            _currentSelection = moduleIds
        End If

    End Sub

    Private Sub utModules_BeforeCheck(sender As Object, e As BeforeCheckEventArgs) Handles utModules.BeforeCheck
        If (e.NewValue = CheckState.Indeterminate) Then
            If (e.TreeNode.CheckedState = CheckState.Checked) Then e.NewValue = CheckState.Unchecked
            If (e.TreeNode.CheckedState = CheckState.Unchecked) Then e.NewValue = CheckState.Checked
        End If
    End Sub

    Private Sub utModules_GotFocus(sender As Object, e As EventArgs) Handles utModules.GotFocus
        _blinkingTimer.Stop()

        Me.btnApply.Appearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.False
        Me.btnApply.Appearance.ForeColor = Color.Black
    End Sub

    Private Sub utModules_LostFocus(sender As Object, e As EventArgs) Handles utModules.LostFocus

        Dim myselection As List(Of String) = GetSelection()

        If _strgDown AndAlso Not List1EqualList2(_currentSelection, myselection) Then

            Me.Update()

            RaiseEvent ModuleSelectionChanged(Me, New ModulesHubEventArgs(DirectCast(Me.uceModuleConfigs.SelectedItem.DataValue, Harness_configuration), _isDirty, myselection))
        End If

        _currentSelection.Clear()
        _strgDown = False
        _blinkingTimer.Start()
    End Sub

    Private Sub utModules_MouseDown(sender As Object, e As MouseEventArgs) Handles utModules.MouseDown
        Dim clickedNode As UltraTreeNode = Me.utModules.GetNodeFromPoint(e.X, e.Y)

        If (e.Button = System.Windows.Forms.MouseButtons.Right) Then

            Dim item As ToolBase = _contextMenuTree.Tools.Item(ContextKey.ConsiderCheckStateForSorting.ToString)
            item.SharedProps.Visible = False

            Dim itemCheckSelectOnly As ToolBase = _contextMenuTree.Tools.Item(ContextKey.CheckSelectOnly.ToString)
            itemCheckSelectOnly.SharedProps.Visible = False

            Dim itemUnCheckAll As ToolBase = _contextMenuTree.Tools.Item(ContextKey.UncheckAll.ToString)
            itemUnCheckAll.SharedProps.Visible = False

            If (clickedNode IsNot Nothing) Then

                For Each nd As UltraTreeNode In AllNodes()
                    If nd.CheckedState = CheckState.Checked Then
                        itemUnCheckAll.SharedProps.Visible = True
                        Exit For
                    End If
                Next

                If (clickedNode.Key = _kblMapper.GetHarness.SystemId) Then
                    item.SharedProps.Visible = True
                    itemCheckSelectOnly.SharedProps.Visible = False
                    itemUnCheckAll.SharedProps.Visible = False
                Else
                    item.SharedProps.Visible = False
                    If clickedNode.Tag IsNot Nothing Then
                        If (clickedNode.Selected) Then
                            itemCheckSelectOnly.SharedProps.Visible = True
                            itemCheckSelectOnly.Tag = clickedNode
                        Else
                            itemCheckSelectOnly.SharedProps.Visible = False
                        End If
                    End If
                End If

                If item.SharedProps.Visible OrElse itemCheckSelectOnly.SharedProps.Visible OrElse itemUnCheckAll.VisibleResolved Then _contextMenuTree.ShowPopup()

            End If

        End If

    End Sub

    Private Sub utModules_MouseDoubleClick(sender As Object, e As MouseEventArgs) Handles utModules.MouseDoubleClick
        Dim clickedNode As UltraTreeNode = Me.utModules.GetNodeFromPoint(e.X, e.Y)
        If (clickedNode Is Nothing) AndAlso (TypeOf Me.utModules.UIElement.LastElementEntered Is NodeClientAreaUIElement) Then
            If (_allExpanded) Then
                Me.utModules.CollapseAll()
            Else
                Me.utModules.ExpandAll()
            End If

            _allExpanded = Not _allExpanded
        End If
    End Sub

    Private Sub utmModulesHub_ToolClick(sender As Object, e As ToolClickEventArgs) Handles utmModulesHub.ToolClick
        Select Case e.Tool.Key
            Case ContextKey.ConsiderCheckStateForSorting.ToString
                My.Settings.ConsiderCheckStateForModuleTreeSorting = Not My.Settings.ConsiderCheckStateForModuleTreeSorting

                e.Tool.SharedProps.Caption = If(My.Settings.ConsiderCheckStateForModuleTreeSorting, ModulesHubStrings.ConsiderAlphanumericOrderForSorting, ModulesHubStrings.ConsiderCheckStateForSorting)

                With Me.utModules
                    .BeginUpdate()

                    .Override.SortComparer = New NumericStringSortComparer(My.Settings.ConsiderCheckStateForModuleTreeSorting)
                    .RefreshSort()

                    .EndUpdate()
                End With

            Case ContextKey.CheckSelectOnly.ToString
                Me.utModules.BeginUpdate()
                For Each nd As UltraTreeNode In AllNodes()

                    If nd.Selected Then
                        If nd.CheckedState <> CheckState.Checked Then nd.CheckedState = CheckState.Checked
                    Else
                        If Not nd.Selected Then nd.CheckedState = CheckState.Unchecked
                    End If
                Next
                Me.utModules.EndUpdate()

            Case ContextKey.UncheckAll.ToString
                Me.utModules.BeginUpdate()

                For Each nd As UltraTreeNode In AllNodes()
                    If nd.CheckedState <> CheckState.Unchecked Then nd.CheckedState = CheckState.Unchecked
                Next
                _utModules.ActiveNode = Nothing
                Me.utModules.SelectedNodes.Clear()
                Me.utModules.EndUpdate()

            Case ContextKey.CompareModules.ToString
                Me.utModules.BeginUpdate()
                Dim selNodes As IEnumerable(Of UltraTreeNode) = utModules.SelectedNodes.OfType(Of UltraTreeNode).Where(Function(n, x) TypeOf n.Tag Is [Module])
                If (selNodes.Count = 2) Then
                    CompareCompositeModules(CType(selNodes(0).Tag, [Module]), CType(selNodes(1).Tag, [Module]))
                End If
                Me.utModules.EndUpdate()

        End Select

    End Sub

    Private Function List1EqualList2(list1 As List(Of String), list2 As List(Of String)) As Boolean
        Dim res As Boolean = True
        If list1.Count = list2.Count Then
            For Each item As String In list1
                If Not list2.Contains(item) Then
                    res = False
                    Exit For
                End If
            Next
        Else
            res = False
        End If
        Return res
    End Function

    Private Sub utModules_KeyUp(sender As Object, e As KeyEventArgs) Handles utModules.KeyUp
        If e.KeyCode = Keys.ControlKey Then
            Dim myselection As List(Of String) = GetSelection()

            If Not List1EqualList2(_currentSelection, myselection) Then

                Me.Update()

                RaiseEvent ModuleSelectionChanged(Me, New ModulesHubEventArgs(DirectCast(Me.uceModuleConfigs.SelectedItem.DataValue, Harness_configuration), _isDirty, myselection))
                _currentSelection = myselection
            End If
        End If
        _strgDown = False
    End Sub
    Private Function GetSelection() As List(Of String)
        Dim moduleIds As New List(Of String)

        For Each node As UltraTreeNode In Me.utModules.SelectedNodes
            moduleIds.Add(node.Key)
        Next
        Return moduleIds
    End Function

    Private Sub utModules_BeforeSelect(sender As Object, e As BeforeSelectEventArgs) Handles utModules.BeforeSelect
        If E3.Lib.DotNet.Expansions.Devices.My.Computer.Keyboard.CtrlKeyDown Then
            If Not _strgDown Then _strgDown = True
        End If
    End Sub

    Friend WriteOnly Property SystemChange() As Boolean
        Set(value As Boolean)
            _systemChange = value
        End Set
    End Property


    Friend Sub SelectionChangedFromDocumentForm(objIds As IEnumerable(Of String))

        With utModules
            .BeginUpdate()
            .ActiveNode = Nothing
            .EventManager.AllEventsEnabled = False
            .SelectedNodes.Clear()
            .EventManager.AllEventsEnabled = True

            For Each moduleId As String In objIds
                If (.GetNodeByKey(moduleId) IsNot Nothing) Then
                    .ActiveNode = .GetNodeByKey(moduleId)
                    .GetNodeByKey(moduleId).BringIntoView()
                    .EventManager.AllEventsEnabled = False
                    .GetNodeByKey(moduleId).Selected = True
                    .EventManager.AllEventsEnabled = True
                End If
            Next
            .EndUpdate()

        End With
    End Sub

    Private Sub ModulesHub_KeyDown(sender As Object, e As KeyEventArgs) Handles utModules.KeyDown
        If (e.KeyCode = Keys.Escape) Then
            utModules.ActiveNode = Nothing
            utModules.SelectedNodes.Clear()
        End If
    End Sub

    Private Sub CompareCompositeModules(refMod As [Module], compMod As [Module])
        Dim refHarnessConfig As New Harness_configuration
        With refHarnessConfig
            .SystemId = Guid.NewGuid.ToString
            .Description = HarnessModuleConfigurationType.UserDefined.ToString
            .Part_number = refMod.Abbreviation
            .Version = "1"
            .Modules = refMod.SystemId
        End With

        Dim compareHarnessConfig As New Harness_configuration
        With compareHarnessConfig
            .SystemId = Guid.NewGuid.ToString
            .Description = HarnessModuleConfigurationType.UserDefined.ToString
            .Part_number = compMod.Abbreviation
            .Version = "1"
            .Modules = compMod.SystemId
        End With

        Using f As New CompareForm(_kblMapper, refHarnessConfig, _kblMapper, compareHarnessConfig, My.Application.MainForm, _settings?.GeneralSettings)
            If MainForm IsNot Nothing Then
                AddHandler f.CompareHubSelectionChanged, AddressOf My.Application.MainForm.MainStateMachine.HighlightCompareHubSelection
            End If
            f.ShowDialog(Me)
            If MainForm IsNot Nothing Then
                RemoveHandler f.CompareHubSelectionChanged, AddressOf My.Application.MainForm.MainStateMachine.HighlightCompareHubSelection
            End If
        End Using

    End Sub

    Private Sub utmModulesHub_BeforeToolDropdown(sender As Object, e As BeforeToolDropdownEventArgs) Handles utmModulesHub.BeforeToolDropdown
        If e.Tool.Key = _contextMenuTree.Key Then
            _contextMenuTree.Tools(ContextKey.CompareModules.ToString).SharedProps.Enabled = utModules.SelectedNodes.OfType(Of UltraTreeNode).Count(Function(n) TypeOf n.Tag Is [Module]) = 2
        End If
    End Sub

    Public Enum ContextKey
        ConsiderCheckStateForSorting
        CheckSelectOnly
        UncheckAll
        CompareModules
    End Enum

End Class