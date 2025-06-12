Imports System.IO
Imports Infragistics.Win
Imports Infragistics.Win.UltraWinToolbars
Imports Infragistics.Win.UltraWinToolTip
Imports Infragistics.Win.UltraWinTree

<Reflection.Obfuscation(Feature:="renaming", ApplyToMembers:=False, Exclude:=True)> ' needed to avoid resource-file-names obfuscation errors, see issue #5
Public Class GridAppearanceForm

    Private _allExpanded As Boolean
    Private _clickedNode As UltraTreeNode
    Private _contextMenu As PopupMenuTool
    Private _gridAppearances As Dictionary(Of KblObjectType, GridAppearance)
    Private _dragStartNode As UltraTreeNode
    Private _internal As Boolean = False

    Private Shared _resetted As Boolean = False
    Private _controlStateDict As New Dictionary(Of Control, Boolean) ' store the initial enabled control state

    Private Enum TreeContextMenuToolKey
        ChangeColumnCaption
        ToggleFindRelevance
        ToggleVisibility
        ToggleComparable
        ToggleViewIn3D
        ExpandSelection
        CollapseSelection
        SelectAll
    End Enum

    Public Sub New()
        InitializeComponent()
        Initialize()
        DisableAllControlsEnabled()
    End Sub

    Private Sub DisableAllControlsEnabled()
        If _controlStateDict.Count = 0 Then
            For Each ctrl As Control In Me.Controls
                _controlStateDict.Add(ctrl, ctrl.Enabled)
                ctrl.Enabled = False
            Next
        End If
    End Sub

    Private Sub ResetAllControlsEnabled()
        For Each ctrl As Control In Me.Controls
            If _controlStateDict.ContainsKey(ctrl) Then
                ctrl.Enabled = _controlStateDict(ctrl)
            End If
        Next
        _controlStateDict.Clear()
    End Sub

    Private Sub Initialize()
        Me.BackColor = Color.White
        Me.Text = GridAppearanceFormStrings.Caption
        Me.Icon = My.Resources.DataTableSettings

        _gridAppearances = New Dictionary(Of KblObjectType, GridAppearance)

        For Each ga As GridAppearance In GridAppearance.All
            Dim newAppClone As GridAppearance = CType(ga.Clone, GridAppearance)
            _gridAppearances.Add(newAppClone.GridTable.Type, newAppClone)
        Next

        InitializeContextMenu()
        InitializeTree()
    End Sub

    Private Sub InitializeContextMenu()
        Me.utmGridAppearance.Tools.Clear()

        _contextMenu = New PopupMenuTool("TreeContextMenu")

        With _contextMenu
            .DropDownArrowStyle = DropDownArrowStyle.None

            Dim changeColumnHeaderButton As New ButtonTool(TreeContextMenuToolKey.ChangeColumnCaption.ToString)
            With changeColumnHeaderButton
                .SharedProps.Caption = GridAppearanceFormStrings.ChangeCaption_CtxtMnu_Caption
                .SharedProps.AppearancesSmall.Appearance.Image = My.Resources.ColumnCaption.ToBitmap
            End With

            Dim toggleFindRelevanceButton As New ButtonTool(TreeContextMenuToolKey.ToggleFindRelevance.ToString)
            With toggleFindRelevanceButton
                .SharedProps.Caption = GridAppearanceFormStrings.ToggleFinIncl_CtxtMnu_Caption
                .SharedProps.AppearancesSmall.Appearance.Image = My.Resources.Search.ToBitmap
            End With

            Dim toggleVisibilityButton As New ButtonTool(TreeContextMenuToolKey.ToggleVisibility.ToString)
            With toggleVisibilityButton
                .SharedProps.Caption = GridAppearanceFormStrings.ToggleVisibility_CtxtMnu_Caption
                .SharedProps.AppearancesSmall.Appearance.Image = My.Resources.Visibility.ToBitmap
            End With

            Dim toggleComparableButton As New ButtonTool(TreeContextMenuToolKey.ToggleComparable.ToString)
            With toggleComparableButton
                .SharedProps.Caption = GridAppearanceFormStrings.ToggleComparable_CtxtMnu_Caption
                .SharedProps.AppearancesSmall.Appearance.Image = My.Resources.CompareData.ToBitmap
            End With

            Dim toggleShowIn3DButton As New ButtonTool(TreeContextMenuToolKey.ToggleViewIn3D.ToString)
            With toggleShowIn3DButton
                .SharedProps.Caption = GridAppearanceFormStrings.ToggleShowIn3D_CtxtMnu_Caption
                .SharedProps.AppearancesSmall.Appearance.Image = CType(My.Resources.ShowProp3DActive.Clone, Bitmap)
            End With

            Dim expandSelectionButton As New ButtonTool(TreeContextMenuToolKey.ExpandSelection.ToString)
            With expandSelectionButton
                .SharedProps.Caption = GridAppearanceFormStrings.ExpandSelection_CtxtMnu_Caption
            End With

            Dim collapseSelectionButton As New ButtonTool(TreeContextMenuToolKey.CollapseSelection.ToString)
            With collapseSelectionButton
                .SharedProps.Caption = GridAppearanceFormStrings.CollapseSelection_CtxtMnu_Caption
            End With

            Dim selectAllButton As New ButtonTool(TreeContextMenuToolKey.SelectAll.ToString)
            With selectAllButton
                .SharedProps.Caption = GridAppearanceFormStrings.SelectAll_CtxMnu_Caption
            End With

            Me.utmGridAppearance.Tools.AddRange(New ToolBase() {_contextMenu, changeColumnHeaderButton, toggleFindRelevanceButton, toggleVisibilityButton, toggleComparableButton, toggleShowIn3DButton, expandSelectionButton, collapseSelectionButton, selectAllButton})

            .Tools.AddTool(changeColumnHeaderButton.Key)
            .Tools.AddTool(toggleFindRelevanceButton.Key)
            .Tools.AddTool(toggleVisibilityButton.Key)
            .Tools.AddTool(toggleComparableButton.Key)
            .Tools.AddTool(toggleShowIn3DButton.Key)
            .Tools.AddTool(expandSelectionButton.Key)
            .Tools.AddTool(collapseSelectionButton.Key)
            .Tools.AddTool(selectAllButton.Key)
        End With
    End Sub

    Private Sub InitializeTree()
        Me.utGridAppearance.BeginUpdate()
        Me.utGridAppearance.Nodes.Clear()

        Try
            For Each gridAppearance As KeyValuePair(Of KblObjectType, GridAppearance) In _gridAppearances
                Dim sortedGridColumns As New SortedDictionary(Of Integer, GridColumn)
                Dim tableNode As New UltraTreeNode(gridAppearance.Key.ToString, gridAppearance.Value.GridTable.Description)
                tableNode.LeftImages.Add(My.Resources.GridTable.ToBitmap)
                tableNode.Tag = gridAppearance.Value.GridTable

                Me.utGridAppearance.Nodes.Add(tableNode)

                If (gridAppearance.Value.GridTable.GridSubTable IsNot Nothing) Then
                    Dim subTableNode As New UltraTreeNode(gridAppearance.Value.GridTable.GridSubTable.Type.ToString, gridAppearance.Value.GridTable.GridSubTable.Description)
                    subTableNode.LeftImages.Add(My.Resources.GridTable.ToBitmap)
                    subTableNode.Tag = gridAppearance.Value.GridTable.GridSubTable

                    tableNode.Nodes.Add(subTableNode)

                    For Each subGridColumn As GridColumn In gridAppearance.Value.GridTable.GridSubTable.GridColumns
                        If subGridColumn.Ordinal >= 0 Then
                            sortedGridColumns.Add(subGridColumn.Ordinal, subGridColumn)
                        End If
                    Next

                    For Each subGridColumn As GridColumn In sortedGridColumns.Values
                        Dim subGridColumnNode As New UltraTreeNode(String.Format("{0}|{1}", subTableNode.Key, subGridColumn.KBLPropertyName), subGridColumn.Name)
                        If (subGridColumn.Visible) Then
                            subGridColumnNode.CheckedState = CheckState.Checked
                        Else
                            subGridColumnNode.CheckedState = CheckState.Unchecked
                        End If

                        subGridColumnNode.Override.NodeStyle = NodeStyle.CheckBox

                        AddGridColumNodeImages(subGridColumn, subGridColumnNode)

                        subGridColumnNode.Tag = subGridColumn

                        subTableNode.Nodes.Add(subGridColumnNode)
                    Next
                End If

                sortedGridColumns.Clear()

                For Each gridColumn As GridColumn In gridAppearance.Value.GridTable.GridColumns
                    If gridColumn.Ordinal >= 0 Then
                        sortedGridColumns.Add(gridColumn.Ordinal, gridColumn)
                    End If
                Next

                For Each gridColumn As GridColumn In sortedGridColumns.Values
                    Dim gridColumnNode As New UltraTreeNode(String.Format("{0}|{1}", tableNode.Key, gridColumn.KBLPropertyName), gridColumn.Name)

                    If (gridColumn.Visible) Then
                        gridColumnNode.CheckedState = CheckState.Checked
                    Else
                        gridColumnNode.CheckedState = CheckState.Unchecked
                    End If

                    gridColumnNode.Override.NodeStyle = NodeStyle.CheckBox

                    If gridAppearance.Key <> KblObjectType.Connector_occurrence_D3D Then
                        AddGridColumNodeImages(gridColumn, gridColumnNode)
                    End If


                    gridColumnNode.Tag = gridColumn

                    tableNode.Nodes.Add(gridColumnNode)
                Next
            Next

            _allExpanded = True

            Me.utGridAppearance.AllowDrop = True
            Me.utGridAppearance.Nodes.Override.Sort = SortType.Ascending
            Me.utGridAppearance.ExpandAll()
        Catch ex As Exception
            MessageBox.Show(String.Format(GridAppearanceFormStrings.LoadTreeFailed_Msg, vbCrLf, ex.Message), [Shared].MSG_BOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error)

            Me.utGridAppearance.Nodes.Clear()
        Finally
            Me.utGridAppearance.EndUpdate()
        End Try
    End Sub

    Private Sub AddGridColumNodeImages(colum As GridColumn, node As UltraTreeNode)
        If Not colum.HideSearchable Then
            If (colum.Searchable) Then
                Dim searchableBmp As Bitmap = My.Resources.Searchable.ToBitmap
                searchableBmp.Tag = ImageCategory.CategorySearchable
                node.RightImages.Add(searchableBmp)
            Else
                Dim notSearchableBmp As Bitmap = My.Resources.NotSearchable.ToBitmap
                notSearchableBmp.Tag = ImageCategory.CategorySearchable
                node.RightImages.Add(notSearchableBmp)
            End If
        End If

        If Not colum.HideComparable Then
            If (colum.Comparable) Then
                Dim cmpData As Bitmap = My.Resources.Comparable.ToBitmap
                cmpData.Tag = ImageCategory.CategoryComparable
                node.RightImages.Add(cmpData)
            Else
                Dim cmpData As Bitmap = My.Resources.NotComparable
                cmpData.Tag = ImageCategory.CategoryComparable
                node.RightImages.Add(cmpData)
            End If
        End If


        If colum.D3D.ButtonVisible Then
            If colum.D3D.PropertyVisible Then
                Dim bmpShowIn3D As Bitmap = CType(My.Resources.ShowProp3DActive.Clone, Bitmap)
                bmpShowIn3D.Tag = ImageCategory.CategoryViewIn3D
                node.RightImages.Add(bmpShowIn3D)
            Else
                Dim bmpHideIn3D As Bitmap = CType(My.Resources.ShowProp3DInActive.Clone, Bitmap)
                bmpHideIn3D.Tag = ImageCategory.CategoryViewIn3D
                node.RightImages.Add(bmpHideIn3D)
            End If
        End If

    End Sub

    Private Function UpdateGridColumnNodeImages(node As UltraTreeNode) As Boolean
        Dim column As GridColumn = TryCast(node.Tag, GridColumn)
        If column IsNot Nothing Then
            node.RightImages.Clear()
            AddGridColumNodeImages(column, node)
            Return True
        End If
        Return False
    End Function

    Private Sub GridAppearanceForm_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        If (e.KeyCode = Keys.Escape) Then Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
    End Sub

    Private Sub btnOK_Click(sender As Object, e As EventArgs) Handles btnOK.Click
        For Each gridAppearance As KeyValuePair(Of KblObjectType, GridAppearance) In _gridAppearances
            gridAppearance.Value.Save()
        Next

        Me.DialogResult = System.Windows.Forms.DialogResult.OK
    End Sub

    Private Sub btnReset_Click(sender As Object, e As EventArgs) Handles btnReset.Click
        If (MessageBox.Show(GridAppearanceFormStrings.ResetDefault_Msg, [Shared].MSG_BOX_TITLE, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) = MsgBoxResult.Yes) Then
            Try
                Dim dir As New DirectoryInfo(GridAppearance.DirectoryPath)
                If dir.Exists Then
                    dir.Delete(True)
                End If
                _resetted = True
                Me.DialogResult = DialogResult.OK
            Catch ex As Exception
                MessageBoxEx.Show(Me, String.Format(GridAppearanceFormStrings.ErrorDeletingDirectory, ex.Message), [Shared].MSG_BOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try

        End If
    End Sub

    Private Sub utGridAppearance_AfterCheck(sender As Object, e As NodeEventArgs) Handles utGridAppearance.AfterCheck
        If TypeOf e.TreeNode.Tag Is GridColumn Then
            DirectCast(e.TreeNode.Tag, GridColumn).Visible = CBool(e.TreeNode.CheckedState)
            SetSelectionCheckState(e.TreeNode.CheckedState)
        End If
    End Sub

    Private Sub utGridAppearance_DragDrop(sender As Object, e As DragEventArgs) Handles utGridAppearance.DragDrop
        Dim targetNode As UltraTreeNode = Me.utGridAppearance.GetNodeFromPoint(Me.utGridAppearance.PointToClient(New Point(e.X, e.Y)))
        Dim sourceNodes As UltraTreeNode() = DirectCast(e.Data.GetData(GetType(UltraTreeNode())), UltraTreeNode())

        For Each sourceNode As UltraTreeNode In sourceNodes
            sourceNode.Reposition(targetNode, NodePosition.Previous)
        Next

        CType(targetNode.Tag, GridColumn).Ordinal = targetNode.Index

        For Each parentNode As UltraTreeNode In sourceNodes.GroupBy(Function(node) node.Parent).Select(Function(grp) grp.Key)
            UpdateOrdinals(parentNode.Nodes.Cast(Of UltraTreeNode))
        Next

    End Sub

    Private Sub UpdateOrdinals(nodes As IEnumerable(Of UltraTreeNode))
        For Each node As UltraTreeNode In nodes
            If TypeOf node.Tag Is GridColumn Then
                CType(node.Tag, GridColumn).Ordinal = node.Index
            End If
        Next
    End Sub

    Private Sub utGridAppearance_DragOver(sender As Object, e As DragEventArgs) Handles utGridAppearance.DragOver
        Dim targetNode As UltraTreeNode = Me.utGridAppearance.GetNodeFromPoint(Me.utGridAppearance.PointToClient(New Point(e.X, e.Y)))

        Dim sourceNodes As UltraTreeNode() = DirectCast(e.Data.GetData(GetType(UltraTreeNode())), UltraTreeNode())
        If (targetNode IsNot Nothing) AndAlso (TypeOf targetNode.Tag Is GridColumn AndAlso _dragStartNode.Key <> targetNode.Key AndAlso sourceNodes.All(Function(sourceNode) targetNode.Parent IsNot Nothing AndAlso targetNode.Parent.Key = sourceNode.Parent.Key)) Then
            e.Effect = DragDropEffects.Move
        Else
            e.Effect = DragDropEffects.None
        End If
    End Sub

    Private Sub utGridAppearance_MouseClick(sender As Object, e As MouseEventArgs) Handles utGridAppearance.MouseClick
        _clickedNode = Me.utGridAppearance.GetNodeFromPoint(e.X, e.Y)

        If (_clickedNode IsNot Nothing) Then
            utGridAppearance.BeginUpdate()
            If Not E3.Lib.DotNet.Expansions.Devices.My.Computer.Keyboard.CtrlKeyDown AndAlso Not _clickedNode.Selected Then
                utGridAppearance.SelectedNodes.Clear()
            End If
            _clickedNode.Selected = True
            utGridAppearance.EndUpdate()

            If (e.Button = System.Windows.Forms.MouseButtons.Right) Then
                Dim gridCol As GridColumn = TryCast(_clickedNode.Tag, GridColumn)
                If gridCol IsNot Nothing Then
                    _contextMenu.Tools.Item(TreeContextMenuToolKey.ToggleComparable.ToString).SharedProps.Enabled = Not gridCol.HideComparable
                    _contextMenu.Tools.Item(TreeContextMenuToolKey.ToggleFindRelevance.ToString).SharedProps.Enabled = Not gridCol.HideSearchable
                End If
                _contextMenu.ShowPopup()
            End If
        End If
    End Sub

    Private Sub utGridAppearance_MouseLeaveElement(sender As Object, e As UIElementEventArgs) Handles utGridAppearance.MouseLeaveElement
        If TypeOf e.Element Is ImageUIElement Then
            UltraToolTipManager.HideToolTip()
        End If
    End Sub

    Private Sub utGridAppearance_MouseEnterElement(sender As Object, e As UIElementEventArgs) Handles utGridAppearance.MouseEnterElement
        If TypeOf e.Element Is ImageUIElement Then
            Dim node As UltraTreeNode = CType(e.Element.GetContext(GetType(UltraTreeNode)), UltraTreeNode)
            If node IsNot Nothing AndAlso (TypeOf node.Tag Is GridColumn) AndAlso (node.RightImages.Count <> 0) Then
                Dim cat As ImageCategory = CType(CType(e.Element, ImageUIElement).Image.Tag, ImageCategory)
                Select Case cat
                    Case ImageCategory.CategoryComparable
                        ShowToolTip(cat, _contextMenu.Tools(TreeContextMenuToolKey.ToggleComparable.ToString).CaptionResolved)
                    Case ImageCategory.CategorySearchable
                        ShowToolTip(cat, _contextMenu.Tools(TreeContextMenuToolKey.ToggleFindRelevance.ToString).CaptionResolved)
                    Case ImageCategory.CategoryViewIn3D
                        ShowToolTip(cat, _contextMenu.Tools(TreeContextMenuToolKey.ToggleViewIn3D.ToString).CaptionResolved)
                End Select
            End If
        End If
    End Sub

    Private Sub ShowToolTip(imageCategory As ImageCategory, text As String)
        If Not UltraToolTipManager.IsToolTipVisible Then
            Dim tti As New UltraToolTipInfo(text, Infragistics.Win.ToolTipImage.None, String.Empty, Infragistics.Win.DefaultableBoolean.False)
            tti.Tag = imageCategory
            UltraToolTipManager.SetUltraToolTip(Me.utGridAppearance, tti)
            UltraToolTipManager.ShowToolTip(Me.utGridAppearance)
        End If
    End Sub

    Private Sub utGridAppearance_MouseDoubleClick(sender As Object, e As MouseEventArgs) Handles utGridAppearance.MouseDoubleClick
        _clickedNode = Me.utGridAppearance.GetNodeFromPoint(e.X, e.Y)
        If (_clickedNode Is Nothing) Then
            If (_allExpanded) Then
                Me.utGridAppearance.CollapseAll()
            Else
                Me.utGridAppearance.ExpandAll()
            End If

            _allExpanded = Not _allExpanded
        ElseIf (TypeOf _clickedNode.Tag Is GridColumn) AndAlso (_clickedNode.RightImages.Count <> 0) Then
            Dim cat As ImageCategory = GetImageCategoryFromPoint(_clickedNode, e.Location)
            Dim result As Boolean
            If TrySetImageCategory(_clickedNode, CategoryState.Toggle, cat, result) Then
                SetImageCategoryToSelectedNode(CType(result, CategoryState), cat)
            End If
        End If
    End Sub

    Private Sub SetImageCategoryToSelectedNode(enabled As CategoryState, category As ImageCategory)
        For Each node As UltraTreeNode In utGridAppearance.SelectedNodes
            TrySetImageCategory(node, enabled, category)
        Next
    End Sub

    Private Function TrySetImageCategory(node As UltraTreeNode, enabled As CategoryState, category As ImageCategory, Optional ByRef result As Boolean = Nothing) As Boolean
        If TypeOf node.Tag Is GridColumn Then
            With CType(node.Tag, GridColumn)
                Select Case category
                    Case ImageCategory.CategoryComparable
                        result = If(enabled <> CategoryState.Toggle, CBool(enabled), Not .Comparable)
                        .Comparable = result
                        UpdateGridColumnNodeImages(node)
                        Return True
                    Case ImageCategory.CategorySearchable
                        result = If(enabled <> CategoryState.Toggle, CBool(enabled), Not .Searchable)
                        .Searchable = result
                        UpdateGridColumnNodeImages(node)
                        Return True
                    Case ImageCategory.CategoryViewIn3D
                        result = If(enabled <> CategoryState.Toggle, CBool(enabled), Not .D3D.PropertyVisible)
                        .D3D.PropertyVisible = result
                        UpdateGridColumnNodeImages(node)
                        Return True
                End Select
            End With
        End If
        Return False
    End Function

    Private Enum CategoryState
        Disabled = 0
        Enabled = 1
        Toggle = 2
    End Enum

    Private Function GetImageCategoryFromPoint(node As UltraTreeNode, clickedLocation As Point) As ImageCategory
        Dim imgElement As Infragistics.Win.ImageUIElement = TryCast(node.UIElement.ElementFromPoint(clickedLocation), Infragistics.Win.ImageUIElement)
        If imgElement IsNot Nothing AndAlso TypeOf imgElement.Image.Tag Is ImageCategory Then
            Return CType(imgElement.Image.Tag, ImageCategory)
        End If
        Return ImageCategory.Unknown
    End Function

    Private Sub utGridAppearance_SelectionDragStart(sender As Object, e As EventArgs) Handles utGridAppearance.SelectionDragStart
        If Me.utGridAppearance.SelectedNodes.Cast(Of UltraTreeNode).Any(Function(node) TypeOf node.Tag Is GridColumn) Then
            _dragStartNode = Me.utGridAppearance.GetNodeFromPoint(Me.utGridAppearance.PointToClient(Form.MousePosition))
            If _dragStartNode IsNot Nothing AndAlso TypeOf _dragStartNode.Tag Is GridColumn Then
                DoDragDrop(Me.utGridAppearance.SelectedNodes.Cast(Of UltraTreeNode).Where(Function(node) TypeOf node.Tag Is GridColumn).ToArray, DragDropEffects.Move)
            End If
        End If
    End Sub

    Private Sub utmGridAppearance_ToolClick(sender As Object, e As ToolClickEventArgs) Handles utmGridAppearance.ToolClick
        Select Case e.Tool.Key.Parse(Of TreeContextMenuToolKey)
            Case TreeContextMenuToolKey.ChangeColumnCaption
                Using inputForm As New InputForm(String.Format(GridAppearanceFormStrings.ChangeCaption_InputBoxPrompt, DirectCast(_clickedNode.Tag, GridColumn).KBLPropertyName), String.Format(GridAppearanceFormStrings.DataTableColumn_InputBoxTitle, _clickedNode.Text), _clickedNode.Text)
                    If (inputForm.ShowDialog(Me) = System.Windows.Forms.DialogResult.OK) Then
                        Dim columnName As String = inputForm.Response.Trim
                        If (columnName <> String.Empty) Then
                            If (columnName.Length <= 50) Then
                                _clickedNode.Text = columnName

                                DirectCast(_clickedNode.Tag, GridColumn).Name = columnName
                            Else
                                MessageBox.Show(GridAppearanceFormStrings.ErrorMaxCaptionLength_Msg, [Shared].MSG_BOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            End If
                        End If
                    End If
                End Using
            Case TreeContextMenuToolKey.ToggleViewIn3D
                If (_clickedNode.RightImages.Count <> 0) Then
                    Dim result As Boolean
                    utGridAppearance.BeginUpdate()
                    TrySetImageCategory(_clickedNode, CategoryState.Toggle, ImageCategory.CategoryViewIn3D, result)
                    SetImageCategoryToSelectedNode(CType(result, CategoryState), ImageCategory.CategoryViewIn3D)
                    utGridAppearance.EndUpdate()
                End If
            Case TreeContextMenuToolKey.ToggleFindRelevance
                If (_clickedNode.RightImages.Count <> 0) Then
                    Dim result As Boolean
                    utGridAppearance.BeginUpdate()
                    TrySetImageCategory(_clickedNode, CategoryState.Toggle, ImageCategory.CategorySearchable, result)
                    SetImageCategoryToSelectedNode(CType(result, CategoryState), ImageCategory.CategorySearchable)
                    utGridAppearance.EndUpdate()
                End If
            Case TreeContextMenuToolKey.ToggleComparable
                If (_clickedNode.RightImages.Count <> 0) Then
                    Dim result As Boolean
                    utGridAppearance.BeginUpdate()
                    TrySetImageCategory(_clickedNode, CategoryState.Toggle, ImageCategory.CategoryComparable, result)
                    SetImageCategoryToSelectedNode(CType(result, CategoryState), ImageCategory.CategoryComparable)
                    utGridAppearance.EndUpdate()
                End If
            Case TreeContextMenuToolKey.ToggleVisibility
                utGridAppearance.BeginUpdate()
                ToggleCheckState(_clickedNode)
                SetSelectionCheckState(_clickedNode.CheckedState)
                utGridAppearance.EndUpdate()
            Case TreeContextMenuToolKey.ExpandSelection
                For Each node As UltraTreeNode In utGridAppearance.SelectedNodes
                    node.ExpandAll()
                Next
            Case TreeContextMenuToolKey.CollapseSelection
                For Each node As UltraTreeNode In utGridAppearance.SelectedNodes
                    node.CollapseAll()
                Next
            Case TreeContextMenuToolKey.SelectAll
                utGridAppearance.SelectedNodes.AddRange(utGridAppearance.Nodes.Cast(Of UltraTreeNode).ToArray, True)
        End Select
    End Sub

    Private Sub utmGridAppearance_BeforeToolDropdown(sender As Object, e As BeforeToolDropdownEventArgs) Handles utmGridAppearance.BeforeToolDropdown
        If e.Tool.Key = _contextMenu.Key Then
            With CType(e.Tool, PopupMenuTool)

                If (TypeOf _clickedNode.Tag Is GridColumn) Then
                    .Tools(TreeContextMenuToolKey.ChangeColumnCaption).SharedProps.Visible = True
                    .Tools(TreeContextMenuToolKey.ExpandSelection.ToString).SharedProps.Visible = False
                    .Tools(TreeContextMenuToolKey.CollapseSelection.ToString).SharedProps.Visible = False
                    If (_clickedNode.Parent IsNot Nothing AndAlso _clickedNode.Parent.Key <> KblObjectType.Connector_occurrence_D3D.ToString) Then
                        .Tools(TreeContextMenuToolKey.ToggleComparable).SharedProps.Visible = True
                        .Tools(TreeContextMenuToolKey.ToggleFindRelevance).SharedProps.Visible = True
                        .Tools(TreeContextMenuToolKey.ToggleVisibility).SharedProps.Visible = True
                        .Tools(TreeContextMenuToolKey.ToggleViewIn3D).SharedProps.Visible = CType(_clickedNode.Tag, GridColumn).D3D.ButtonVisible
                    Else
                        .Tools(TreeContextMenuToolKey.ToggleComparable).SharedProps.Visible = False
                        .Tools(TreeContextMenuToolKey.ToggleFindRelevance).SharedProps.Visible = False
                        .Tools(TreeContextMenuToolKey.ToggleVisibility).SharedProps.Visible = False
                        .Tools(TreeContextMenuToolKey.ToggleViewIn3D).SharedProps.Visible = False
                    End If

                Else
                    .Tools(TreeContextMenuToolKey.ChangeColumnCaption).SharedProps.Visible = False
                    .Tools(TreeContextMenuToolKey.ToggleComparable).SharedProps.Visible = False
                    .Tools(TreeContextMenuToolKey.ToggleFindRelevance).SharedProps.Visible = False
                    .Tools(TreeContextMenuToolKey.ToggleVisibility).SharedProps.Visible = False
                    .Tools(TreeContextMenuToolKey.ToggleViewIn3D).SharedProps.Visible = False
                    .Tools(TreeContextMenuToolKey.ExpandSelection.ToString).SharedProps.Visible = True
                    .Tools(TreeContextMenuToolKey.CollapseSelection.ToString).SharedProps.Visible = True
                End If

                .Tools(TreeContextMenuToolKey.ExpandSelection.ToString).SharedProps.Enabled = utGridAppearance.SelectedNodes.Count > 0 AndAlso utGridAppearance.SelectedNodes.Cast(Of UltraTreeNode).Any(Function(node) node.HasExpansionIndicator AndAlso Not node.Expanded)
                .Tools(TreeContextMenuToolKey.CollapseSelection.ToString).SharedProps.Enabled = utGridAppearance.SelectedNodes.Count > 0 AndAlso utGridAppearance.SelectedNodes.Cast(Of UltraTreeNode).Any(Function(node) node.HasExpansionIndicator AndAlso node.Expanded)
                .Tools(TreeContextMenuToolKey.SelectAll.ToString).SharedProps.Visible = _clickedNode Is Nothing OrElse _clickedNode.Parent Is Nothing
                .Tools(TreeContextMenuToolKey.SelectAll.ToString).SharedProps.Enabled = utGridAppearance.Nodes.Cast(Of UltraTreeNode).Any(Function(node) Not node.Selected)
            End With
        End If
    End Sub

    Private Enum ImageCategory
        Unknown = 0
        CategoryComparable = 1
        CategorySearchable = 2
        CategoryViewIn3D = 3
    End Enum

    Private Sub txtFilter_ValueChanged(sender As Object, e As EventArgs) Handles txtFilter.ValueChanged
        With Me.utGridAppearance
            .BeginUpdate()
            UpdateTreeFilter(.Nodes, txtFilter.Text)
            .EndUpdate()
        End With
    End Sub

    Private Sub UpdateTreeFilter(treeNodes As TreeNodesCollection, filterText As String)
        For Each node As UltraTreeNode In treeNodes.Cast(Of UltraTreeNode).ToArray
            SetVisibleParentalRecursive(node, node.Text.ToLower.Contains(filterText.ToLower))
            UpdateTreeFilter(node.Nodes, filterText)
        Next
    End Sub

    Private Sub SetVisibleParentalRecursive(node As UltraTreeNode, visible As Boolean)
        node.Visible = visible
        If node.Visible AndAlso node.Parent IsNot Nothing Then
            SetVisibleParentalRecursive(node.Parent, True)
        End If
    End Sub

    Private Sub utGridAppearance_KeyDown(sender As Object, e As KeyEventArgs) Handles utGridAppearance.KeyDown
        If e.KeyCode = Keys.Space Then
            e.Handled = True
            For Each node As UltraTreeNode In utGridAppearance.SelectedNodes
                ToggleCheckState(node)
            Next
        End If
    End Sub

    Private Sub ToggleCheckState(node As UltraTreeNode)
        If Not _internal Then
            _internal = True
            Try
                If node.CheckedState = CheckState.Checked Then
                    node.CheckedState = CheckState.Unchecked
                Else
                    node.CheckedState = CheckState.Checked
                End If
            Finally
                _internal = False
            End Try
        End If
    End Sub

    Private Sub SetSelectionCheckState(checkState As CheckState)
        If Not _internal Then
            _internal = True
            Try
                For Each node As UltraTreeNode In utGridAppearance.SelectedNodes
                    node.CheckedState = checkState
                Next
            Finally
                _internal = False
            End Try
        End If
    End Sub

    Private Sub GridAppearanceForm_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        If _resetted Then
            MessageBoxEx.Show(Me, DialogStrings.MainStatMachine_CloseAppFirstBecauseReset, [Shared].MSG_BOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Information)
            Me.DialogResult = DialogResult.Cancel
            Me.Close()
            Return
        End If
        ResetAllControlsEnabled() ' HINT: for showing the MessageBox above with disabled controls in the background - this reset's back the normal initial state when no MsgBox was shown
    End Sub
End Class