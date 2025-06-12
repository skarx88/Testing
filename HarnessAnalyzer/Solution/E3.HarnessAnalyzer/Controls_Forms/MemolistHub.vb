Imports System.Data
Imports Infragistics.Win
Imports Infragistics.Win.UltraWinToolbars
Imports Infragistics.Win.UltraWinTree

<Reflection.Obfuscation(Feature:="renaming", ApplyToMembers:=False, Exclude:=True)> ' needed to avoid resource-file-names obfuscation errors, see issue #5
Public Class MemolistHub

    Friend Event HubDoubleClicked(ByVal sender As MemolistHub, ByVal e As String)
    Friend Event MemolistChanged(ByVal sender As MemolistHub, ByVal e As String)

    Private _commentCell As UltraTreeNodeCell
    Private _contextMenuTree As PopupMenuTool
    Private _kblMapper As KblMapper
    Private _memolist As DataSet

    Public Sub New()
        InitializeComponent()
        InitializeContextMenu()
    End Sub

    Friend Sub AddObjects(selectedObjects As Dictionary(Of String, Object))
        Me.utMemolist.BeginUpdate()

        For Each selectedObject As KeyValuePair(Of String, Object) In selectedObjects
            Try
                If (TypeOf selectedObject.Value Is Accessory_occurrence) Then
                    Dim row As DataRow = _memolist.Tables(KblObjectType.Accessory_occurrence.ToString).NewRow

                    With DirectCast(selectedObject.Value, Accessory_occurrence)
                        row(NameOf(MemolistHubStrings.Id_ColCaption)) = .SystemId
                        row(NameOf(MemolistHubStrings.Name_ColCaption)) = .Id
                        row(NameOf(MemolistHubStrings.Type_ColCaption)) = KblObjectType.Accessory_occurrence.ToString
                        row(NameOf(MemolistHubStrings.IsSelected_ColCaption)) = False
                    End With

                    _memolist.Tables(KblObjectType.Accessory_occurrence.ToString).Rows.Add(row)
                ElseIf (TypeOf selectedObject.Value Is Approval) Then
                    Dim row As DataRow = _memolist.Tables(KblObjectType.Approval.ToString).NewRow

                    With DirectCast(selectedObject.Value, Approval)
                        row(NameOf(MemolistHubStrings.Id_ColCaption)) = .SystemId
                        row(NameOf(MemolistHubStrings.Name_ColCaption)) = .Name
                        row(NameOf(MemolistHubStrings.Type_ColCaption)) = KblObjectType.Approval.ToString
                        row(NameOf(MemolistHubStrings.IsSelected_ColCaption)) = False
                    End With

                    _memolist.Tables(KblObjectType.Approval.ToString).Rows.Add(row)
                ElseIf (TypeOf selectedObject.Value Is Assembly_part_occurrence) Then
                    Dim row As DataRow = _memolist.Tables(KblObjectType.Assembly_part_occurrence.ToString).NewRow

                    With DirectCast(selectedObject.Value, Assembly_part_occurrence)
                        row(NameOf(MemolistHubStrings.Id_ColCaption)) = .SystemId
                        row(NameOf(MemolistHubStrings.Name_ColCaption)) = .Id
                        row(NameOf(MemolistHubStrings.Type_ColCaption)) = KblObjectType.Assembly_part_occurrence.ToString
                        row(NameOf(MemolistHubStrings.IsSelected_ColCaption)) = False
                    End With

                    _memolist.Tables(KblObjectType.Assembly_part_occurrence.ToString).Rows.Add(row)
                ElseIf (TypeOf selectedObject.Value Is Special_wire_occurrence) Then
                    Dim row As DataRow = _memolist.Tables(KblObjectType.Special_wire_occurrence.ToString).NewRow

                    With DirectCast(selectedObject.Value, Special_wire_occurrence)
                        row(NameOf(MemolistHubStrings.Id_ColCaption)) = .SystemId
                        row(NameOf(MemolistHubStrings.Name_ColCaption)) = .Special_wire_id
                        row(NameOf(MemolistHubStrings.Type_ColCaption)) = KblObjectType.Special_wire_occurrence.ToString
                        row(NameOf(MemolistHubStrings.IsSelected_ColCaption)) = False
                    End With

                    _memolist.Tables(KblObjectType.Special_wire_occurrence.ToString).Rows.Add(row)
                ElseIf (TypeOf selectedObject.Value Is Cavity_occurrence) OrElse (TypeOf selectedObject.Value Is Contact_point) Then
                    Dim cavityOccurrence As Cavity_occurrence = TryCast(selectedObject.Value, Cavity_occurrence)
                    If (cavityOccurrence Is Nothing) Then cavityOccurrence = DirectCast(_kblMapper.KBLOccurrenceMapper(DirectCast(selectedObject.Value, Contact_point).Contacted_cavity.SplitSpace.FirstOrDefault), Cavity_occurrence)

                    Dim row As DataRow = _memolist.Tables(KblObjectType.Cavity_occurrence.ToString).NewRow

                    With cavityOccurrence
                        row(NameOf(MemolistHubStrings.Id_ColCaption)) = .SystemId
                        row(NameOf(MemolistHubStrings.Name_ColCaption)) = String.Format("{0},{1}", DirectCast(_kblMapper.KBLOccurrenceMapper(_kblMapper.KBLCavityConnectorMapper(.SystemId)), Connector_occurrence).Id, DirectCast(_kblMapper.KBLPartMapper(.Part), Cavity).Cavity_number)
                        row(NameOf(MemolistHubStrings.Type_ColCaption)) = KblObjectType.Cavity_occurrence.ToString
                        row(NameOf(MemolistHubStrings.IsSelected_ColCaption)) = False
                    End With

                    _memolist.Tables(KblObjectType.Cavity_occurrence.ToString).Rows.Add(row)
                ElseIf (TypeOf selectedObject.Value Is Component_box_occurrence) Then
                    Dim row As DataRow = _memolist.Tables(KblObjectType.Component_box_occurrence.ToString).NewRow

                    With DirectCast(selectedObject.Value, Component_box_occurrence)
                        row(NameOf(MemolistHubStrings.Id_ColCaption)) = .SystemId
                        row(NameOf(MemolistHubStrings.Name_ColCaption)) = .Id
                        row(NameOf(MemolistHubStrings.Type_ColCaption)) = KblObjectType.Component_box_occurrence.ToString
                        row(NameOf(MemolistHubStrings.IsSelected_ColCaption)) = False
                    End With

                    _memolist.Tables(KblObjectType.Component_box_occurrence.ToString).Rows.Add(row)
                ElseIf (TypeOf selectedObject.Value Is Component_occurrence) Then
                    Dim row As DataRow = _memolist.Tables(KblObjectType.Component_occurrence.ToString).NewRow

                    With DirectCast(selectedObject.Value, Component_occurrence)
                        row(NameOf(MemolistHubStrings.Id_ColCaption)) = .SystemId
                        row(NameOf(MemolistHubStrings.Name_ColCaption)) = .Id
                        row(NameOf(MemolistHubStrings.Type_ColCaption)) = KblObjectType.Component_occurrence.ToString
                        row(NameOf(MemolistHubStrings.IsSelected_ColCaption)) = False
                    End With

                    _memolist.Tables(KblObjectType.Component_occurrence.ToString).Rows.Add(row)
                ElseIf (TypeOf selectedObject.Value Is Connector_occurrence) Then
                    Dim row As DataRow = _memolist.Tables(KblObjectType.Connector_occurrence.ToString).NewRow

                    With DirectCast(selectedObject.Value, Connector_occurrence)
                        row(NameOf(MemolistHubStrings.Id_ColCaption)) = .SystemId
                        row(NameOf(MemolistHubStrings.Name_ColCaption)) = .Id
                        row(NameOf(MemolistHubStrings.Type_ColCaption)) = KblObjectType.Connector_occurrence.ToString
                        row(NameOf(MemolistHubStrings.IsSelected_ColCaption)) = False
                    End With

                    _memolist.Tables(KblObjectType.Connector_occurrence.ToString).Rows.Add(row)
                ElseIf (TypeOf selectedObject.Value Is Core_occurrence) Then
                    Dim row As DataRow = _memolist.Tables(KblObjectType.Core_occurrence.ToString).NewRow

                    With DirectCast(selectedObject.Value, Core_occurrence)
                        row(NameOf(MemolistHubStrings.Id_ColCaption)) = .SystemId
                        row(NameOf(MemolistHubStrings.Name_ColCaption)) = .Wire_number
                        row(NameOf(MemolistHubStrings.Type_ColCaption)) = KblObjectType.Core_occurrence.ToString
                        row(NameOf(MemolistHubStrings.IsSelected_ColCaption)) = False
                    End With

                    _memolist.Tables(KblObjectType.Core_occurrence.ToString).Rows.Add(row)
                ElseIf (TypeOf selectedObject.Value Is Co_pack_occurrence) Then
                    Dim row As DataRow = _memolist.Tables(KblObjectType.Co_pack_occurrence.ToString).NewRow

                    With DirectCast(selectedObject.Value, Co_pack_occurrence)
                        row(NameOf(MemolistHubStrings.Id_ColCaption)) = .SystemId
                        row(NameOf(MemolistHubStrings.Name_ColCaption)) = .Id
                        row(NameOf(MemolistHubStrings.Type_ColCaption)) = KblObjectType.Co_pack_occurrence.ToString
                        row(NameOf(MemolistHubStrings.IsSelected_ColCaption)) = False
                    End With

                    _memolist.Tables(KblObjectType.Co_pack_occurrence.ToString).Rows.Add(row)
                ElseIf (TypeOf selectedObject.Value Is Default_dimension_specification) Then
                    Dim row As DataRow = _memolist.Tables(KblObjectType.Default_dimension_specification.ToString).NewRow

                    With DirectCast(selectedObject.Value, Default_dimension_specification)
                        row(NameOf(MemolistHubStrings.Id_ColCaption)) = .SystemId
                        row(NameOf(MemolistHubStrings.Name_ColCaption)) = .Tolerance_type
                        row(NameOf(MemolistHubStrings.Type_ColCaption)) = KblObjectType.Default_dimension_specification.ToString
                        row(NameOf(MemolistHubStrings.IsSelected_ColCaption)) = False
                    End With

                    _memolist.Tables(KblObjectType.Default_dimension_specification.ToString).Rows.Add(row)
                ElseIf (TypeOf selectedObject.Value Is Dimension_specification) Then
                    Dim row As DataRow = _memolist.Tables(KblObjectType.Dimension_specification.ToString).NewRow

                    With DirectCast(selectedObject.Value, Dimension_specification)
                        row(NameOf(MemolistHubStrings.Id_ColCaption)) = .SystemId
                        row(NameOf(MemolistHubStrings.Name_ColCaption)) = .Id
                        row(NameOf(MemolistHubStrings.Type_ColCaption)) = KblObjectType.Dimension_specification.ToString
                        row(NameOf(MemolistHubStrings.IsSelected_ColCaption)) = False
                    End With

                    _memolist.Tables(KblObjectType.Dimension_specification.ToString).Rows.Add(row)
                ElseIf (TypeOf selectedObject.Value Is Fixing_occurrence) Then
                    Dim row As DataRow = _memolist.Tables(KblObjectType.Fixing_occurrence.ToString).NewRow

                    With DirectCast(selectedObject.Value, Fixing_occurrence)
                        row(NameOf(MemolistHubStrings.Id_ColCaption)) = .SystemId
                        row(NameOf(MemolistHubStrings.Name_ColCaption)) = .Id
                        row(NameOf(MemolistHubStrings.Type_ColCaption)) = KblObjectType.Fixing_occurrence.ToString
                        row(NameOf(MemolistHubStrings.IsSelected_ColCaption)) = False
                    End With

                    _memolist.Tables(KblObjectType.Fixing_occurrence.ToString).Rows.Add(row)
                ElseIf (TypeOf selectedObject.Value Is [Module]) Then
                    Dim row As DataRow = _memolist.Tables(KblObjectType.Module.ToString).NewRow

                    With DirectCast(selectedObject.Value, [Module])
                        row(NameOf(MemolistHubStrings.Id_ColCaption)) = .SystemId
                        row(NameOf(MemolistHubStrings.Name_ColCaption)) = String.Format("{0} [{1}]", .Abbreviation, .Part_number)
                        row(NameOf(MemolistHubStrings.Type_ColCaption)) = KblObjectType.Module.ToString
                        row(NameOf(MemolistHubStrings.IsSelected_ColCaption)) = False
                    End With

                    _memolist.Tables(KblObjectType.Module.ToString).Rows.Add(row)
                ElseIf (TypeOf selectedObject.Value Is String) Then
                    Dim row As DataRow = _memolist.Tables(KblObjectType.Net.ToString).NewRow

                    row(NameOf(MemolistHubStrings.Id_ColCaption)) = selectedObject.Value
                    row(NameOf(MemolistHubStrings.Name_ColCaption)) = selectedObject.Value
                    row(NameOf(MemolistHubStrings.Type_ColCaption)) = KblObjectType.Net.ToString
                    row(NameOf(MemolistHubStrings.IsSelected_ColCaption)) = False


                    _memolist.Tables(KblObjectType.Net.ToString).Rows.Add(row)
                ElseIf (TypeOf selectedObject.Value Is Protection_area) OrElse (TypeOf selectedObject.Value Is Wire_protection_occurrence) Then
                    Dim row As DataRow = _memolist.Tables(KblObjectType.Protection_on_segment.ToString).NewRow

                    If (TypeOf selectedObject.Value Is Protection_area) Then
                        row(NameOf(MemolistHubStrings.Id_ColCaption)) = DirectCast(selectedObject.Value, Protection_area).Associated_protection
                        row(NameOf(MemolistHubStrings.Name_ColCaption)) = DirectCast(_kblMapper.KBLOccurrenceMapper(DirectCast(selectedObject.Value, Protection_area).Associated_protection), Wire_protection_occurrence).Id
                    Else
                        row(NameOf(MemolistHubStrings.Id_ColCaption)) = DirectCast(selectedObject.Value, Wire_protection_occurrence).SystemId
                        row(NameOf(MemolistHubStrings.Name_ColCaption)) = DirectCast(selectedObject.Value, Wire_protection_occurrence).Id
                    End If

                    row(NameOf(MemolistHubStrings.Type_ColCaption)) = KblObjectType.Protection_on_segment.ToString
                    row(NameOf(MemolistHubStrings.IsSelected_ColCaption)) = False

                    _memolist.Tables(KblObjectType.Protection_on_segment.ToString).Rows.Add(row)
                ElseIf (TypeOf selectedObject.Value Is Segment) Then
                    Dim row As DataRow = _memolist.Tables(KblObjectType.Segment.ToString).NewRow

                    With DirectCast(selectedObject.Value, Segment)
                        row(NameOf(MemolistHubStrings.Id_ColCaption)) = .SystemId
                        row(NameOf(MemolistHubStrings.Name_ColCaption)) = .Id
                        row(NameOf(MemolistHubStrings.Type_ColCaption)) = KblObjectType.Segment.ToString
                        row(NameOf(MemolistHubStrings.IsSelected_ColCaption)) = False
                    End With

                    _memolist.Tables(KblObjectType.Segment.ToString).Rows.Add(row)
                ElseIf (TypeOf selectedObject.Value Is Node) Then
                    Dim row As DataRow = _memolist.Tables(KblObjectType.Node.ToString).NewRow

                    With DirectCast(selectedObject.Value, Node)
                        row(NameOf(MemolistHubStrings.Id_ColCaption)) = .SystemId
                        row(NameOf(MemolistHubStrings.Name_ColCaption)) = .Id
                        row(NameOf(MemolistHubStrings.Type_ColCaption)) = KblObjectType.Node.ToString
                        row(NameOf(MemolistHubStrings.IsSelected_ColCaption)) = False
                    End With

                    _memolist.Tables(KblObjectType.Node.ToString).Rows.Add(row)
                ElseIf (TypeOf selectedObject.Value Is Wire_occurrence) Then
                    Dim row As DataRow = _memolist.Tables(KblObjectType.Wire_occurrence.ToString).NewRow

                    With DirectCast(selectedObject.Value, Wire_occurrence)
                        row(NameOf(MemolistHubStrings.Id_ColCaption)) = .SystemId
                        row(NameOf(MemolistHubStrings.Name_ColCaption)) = .Wire_number
                        row(NameOf(MemolistHubStrings.Type_ColCaption)) = KblObjectType.Wire_occurrence.ToString
                        row(NameOf(MemolistHubStrings.IsSelected_ColCaption)) = False
                    End With

                    _memolist.Tables(KblObjectType.Wire_occurrence.ToString).Rows.Add(row)
                ElseIf (TypeOf selectedObject.Value Is Change_description) Then
                    Dim row As DataRow = _memolist.Tables(KblObjectType.Change_description.ToString).NewRow

                    With DirectCast(selectedObject.Value, Change_description)
                        row(NameOf(MemolistHubStrings.Id_ColCaption)) = .SystemId
                        row(NameOf(MemolistHubStrings.Name_ColCaption)) = .Description
                        row(NameOf(MemolistHubStrings.Type_ColCaption)) = KblObjectType.Change_description.ToString
                        row(NameOf(MemolistHubStrings.IsSelected_ColCaption)) = False
                    End With

                    _memolist.Tables(KblObjectType.Change_description.ToString).Rows.Add(row)
                End If
            Catch ex As Exception
                Continue For
            End Try
        Next

        Me.utMemolist.EndUpdate()
    End Sub

    Friend Sub InitializeData(memoItemList As MemoItemList, triggeredByImport As Boolean)
        If triggeredByImport Then
            For Each table As DataTable In _memolist.Tables
                table.Rows.Clear()
            Next
        End If

        For Each memoItem As MemoItem In memoItemList
            Try
                Dim row As DataRow = _memolist.Tables(memoItem.Type).NewRow
                row(NameOf(MemolistHubStrings.Id_ColCaption)) = memoItem.Id
                row(NameOf(MemolistHubStrings.Name_ColCaption)) = memoItem.Name
                row(NameOf(MemolistHubStrings.Type_ColCaption)) = memoItem.Type
                row(NameOf(MemolistHubStrings.IsSelected_ColCaption)) = memoItem.IsSelected
                row(NameOf(MemolistHubStrings.Comment_ColCaption)) = memoItem.Comment

                _memolist.Tables(memoItem.Type).Rows.Add(row)
            Catch ex As Exception
                Continue For
            End Try
        Next

        If (triggeredByImport) Then
            RaiseEvent MemolistChanged(Me, "Values changed")
        End If
    End Sub

    Friend Function InitializeTree(kblMapper As KblMapper) As Boolean
        _kblMapper = kblMapper
        _memolist = New DataSet("Memolist")

        Try
            _memolist.Tables.Add(KblObjectType.Accessory_occurrence.ToString)
            _memolist.Tables.Add(KblObjectType.Approval.ToString)
            _memolist.Tables.Add(KblObjectType.Assembly_part_occurrence.ToString)
            _memolist.Tables.Add(KblObjectType.Special_wire_occurrence.ToString)
            _memolist.Tables.Add(KblObjectType.Cavity_occurrence.ToString)
            _memolist.Tables.Add(KblObjectType.Change_description.ToString)
            _memolist.Tables.Add(KblObjectType.Component_box_occurrence.ToString)
            _memolist.Tables.Add(KblObjectType.Component_occurrence.ToString)
            _memolist.Tables.Add(KblObjectType.Connector_occurrence.ToString)
            _memolist.Tables.Add(KblObjectType.Core_occurrence.ToString)
            _memolist.Tables.Add(KblObjectType.Co_pack_occurrence.ToString)
            _memolist.Tables.Add(KblObjectType.Default_dimension_specification.ToString)
            _memolist.Tables.Add(KblObjectType.Dimension_specification.ToString)
            _memolist.Tables.Add(KblObjectType.Fixing_occurrence.ToString)
            _memolist.Tables.Add(KblObjectType.Module.ToString)
            _memolist.Tables.Add(KblObjectType.Net.ToString)
            _memolist.Tables.Add(KblObjectType.Protection_on_segment.ToString)
            _memolist.Tables.Add(KblObjectType.Segment.ToString)
            _memolist.Tables.Add(KblObjectType.Node.ToString)
            _memolist.Tables.Add(KblObjectType.Wire_occurrence.ToString)

            For Each table As DataTable In _memolist.Tables
                AddColumnsToTable(table)
            Next

            Me.utMemolist.BeginUpdate()
            Me.utMemolist.Override.SelectionType = SelectType.Extended
            Me.utMemolist.SelectionBehavior = SelectionBehavior.ExtendedAcrossCollections
            Me.utMemolist.SetDataBinding(_memolist, Nothing)
            Me.utMemolist.NodeLevelOverrides(0).Sort = SortType.Ascending

            Me.utMemolist.EndUpdate()


        Catch ex As Exception
            MessageBox.Show(String.Format(MemolistHubStrings.ErrorLoadTree_Msg, vbCrLf, ex.Message), [Shared].MSG_BOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error)

            Me.utMemolist.EndUpdate()

            Return False
        End Try

        Return True
    End Function

    Friend Sub UpdateData(memoItemList As MemoItemList)
        memoItemList.Clear()

        For Each rootNode As UltraTreeNode In Me.utMemolist.Nodes
            For Each node As UltraTreeNode In rootNode.Nodes
                memoItemList.AddMemoItem(New MemoItem With
                                         {
                                         .Id = node.Cells(NameOf(MemolistHubStrings.Id_ColCaption)).Value.ToString,
                                         .Name = node.Cells(NameOf(MemolistHubStrings.Name_ColCaption)).Value.ToString,
                                         .Type = node.Cells(NameOf(MemolistHubStrings.Type_ColCaption)).Value.ToString,
                                         .IsSelected = CBool(node.Cells(NameOf(MemolistHubStrings.IsSelected_ColCaption)).Value),
                                         .Comment = node.Cells(NameOf(MemolistHubStrings.Comment_ColCaption)).Value.ToString
                                         })
            Next
        Next
    End Sub

    Private Sub AddColumnsToTable(table As DataTable)
        With table
            .Columns.Add(NameOf(MemolistHubStrings.Id_ColCaption), GetType(String))
            .Columns.Add(NameOf(MemolistHubStrings.Name_ColCaption), GetType(String))
            .Columns.Add(NameOf(MemolistHubStrings.Type_ColCaption), GetType(String))
            .Columns.Add(NameOf(MemolistHubStrings.IsSelected_ColCaption), GetType(Boolean))
            .Columns.Add(NameOf(MemolistHubStrings.Comment_ColCaption), GetType(String))

            .PrimaryKey = New DataColumn() { .Columns(NameOf(MemolistHubStrings.Id_ColCaption))}
        End With
    End Sub

    Private Function GetCellUI_Recursivley(element As UIElement) As UltraTreeNodeCellUIElement
        If (element Is Nothing OrElse element.Parent Is Nothing) Then
            Return Nothing
        End If

        If (TypeOf element.Parent Is UltraTreeNodeCellUIElement) Then
            Return DirectCast(element.Parent, UltraTreeNodeCellUIElement)
        Else
            Return GetCellUI_Recursivley(element.Parent)
        End If
    End Function

    Private Sub InitializeContextMenu()
        Me.BackColor = Color.White

        _contextMenuTree = New PopupMenuTool("TreeContextMenu")

        With _contextMenuTree
            .DropDownArrowStyle = DropDownArrowStyle.None

            Dim removeSelectedButton As New ButtonTool("RemoveSelected")
            removeSelectedButton.SharedProps.Caption = MemolistHubStrings.RemSelItem_CtxtMnu_Caption
            removeSelectedButton.SharedProps.AppearancesSmall.Appearance.Image = My.Resources.RemoveSelected.ToBitmap

            Dim removeAllButton As New ButtonTool("RemoveAll")
            removeAllButton.SharedProps.Caption = MemolistHubStrings.RemAll_CtxtMnu_Caption
            removeAllButton.SharedProps.AppearancesSmall.Appearance.Image = My.Resources.RemoveAll.ToBitmap

            Dim checkAllButton As New ButtonTool("CheckAll")
            checkAllButton.SharedProps.Caption = MemolistHubStrings.CheckAll_CtxtMnu_Caption
            checkAllButton.SharedProps.AppearancesSmall.Appearance.Image = My.Resources.CheckAll.ToBitmap

            Dim uncheckAllButton As New ButtonTool("UncheckAll")
            uncheckAllButton.SharedProps.Caption = MemolistHubStrings.UncheckAll_CtxtMnu_Caption
            uncheckAllButton.SharedProps.AppearancesSmall.Appearance.Image = My.Resources.UncheckAll.ToBitmap

            Me.utmMemolist.Tools.AddRange(New ToolBase() {_contextMenuTree, removeSelectedButton, removeAllButton, checkAllButton, uncheckAllButton})

            .Tools.AddTool(removeSelectedButton.Key)
            .Tools.AddTool(removeAllButton.Key)
            .Tools.AddTool(checkAllButton.Key)
            .Tools.AddTool(uncheckAllButton.Key)

            .Tools(checkAllButton.Key).InstanceProps.IsFirstInGroup = True
        End With
    End Sub

    Private Sub RemoveSelectedNodes()
        Me.utMemolist.BeginUpdate()

        Dim rowsForDeletion As New List(Of DataRow)

        For Each rootNode As UltraTreeNode In Me.utMemolist.Nodes
            For Each node As UltraTreeNode In rootNode.Nodes
                If (node.IsActive) OrElse (node.Selected) Then
                    rowsForDeletion.Add(_memolist.Tables(rootNode.BandName).Rows.Find(node.Cells(NameOf(MemolistHubStrings.Id_ColCaption)).Value))
                End If
            Next
        Next

        For Each row As DataRow In rowsForDeletion
            row.Delete()
        Next

        Me.utMemolist.EndUpdate()

        RaiseEvent MemolistChanged(Me, "Values changed")
    End Sub


    Private Sub utMemolist_AfterCellEnterEditMode(sender As Object, e As AfterCellEnterEditModeEventArgs) Handles utMemolist.AfterCellEnterEditMode
        If e.Column.Key = NameOf(MemolistHubStrings.Comment_ColCaption) Then
            _commentCell = e.Cell
        End If
    End Sub

    Private Sub utMemolist_AfterCellExitEditMode(sender As Object, e As AfterCellExitEditModeEventArgs) Handles utMemolist.AfterCellExitEditMode
        If e.Column.Key = NameOf(MemolistHubStrings.Comment_ColCaption) Then
            _commentCell = Nothing
        End If
    End Sub

    Private Sub utMemolist_AfterNodeLayoutItemResize(sender As Object, e As AfterNodeLayoutItemResizeEventArgs) Handles utMemolist.AfterNodeLayoutItemResize
        Me.utMemolist.BeginUpdate()
        Me.utMemolist.EventManager.AllEventsEnabled = False

        For Each rootNode As UltraTreeNode In Me.utMemolist.Nodes
            For Each node As UltraTreeNode In rootNode.Nodes
                node.Cells(e.Column.Key).Column.LayoutInfo.PreferredCellSize = e.Column.LayoutInfo.PreferredCellSize
                node.Cells(e.Column.Key).Column.LayoutInfo.PreferredLabelSize = e.Column.LayoutInfo.PreferredLabelSize
            Next
        Next

        Me.utMemolist.EventManager.AllEventsEnabled = True
        Me.utMemolist.EndUpdate()
    End Sub

    Private Sub utMemolist_Click(sender As Object, e As EventArgs) Handles utMemolist.Click
        Dim uiElement As UIElement = Me.utMemolist.UIElement.ElementFromPoint(Me.utMemolist.PointToClient(Cursor.Position))
        Dim cellUIElement As UltraTreeNodeCellUIElement = GetCellUI_Recursivley(uiElement)
        If (cellUIElement IsNot Nothing) AndAlso (Not cellUIElement.Cell.IsInEditMode AndAlso cellUIElement.Cell.Column.AllowCellEdit = AllowCellEdit.Full) Then
            cellUIElement.Cell.BeginEdit()

            If (TypeOf cellUIElement.Cell.EditorResolved Is CheckEditor) Then
                DirectCast(cellUIElement.Cell.EditorResolved, CheckEditor).Value = Not CBool(DirectCast(cellUIElement.Cell.EditorResolved, CheckEditor).Value)
            End If

            RaiseEvent MemolistChanged(Me, "Values changed")
        End If
    End Sub

    Private Sub utMemolist_ColumnSetGenerated(sender As Object, e As ColumnSetGeneratedEventArgs) Handles utMemolist.ColumnSetGenerated
        e.ColumnSet.BorderStyleCell = UIElementBorderStyle.Dotted
        e.ColumnSet.CellAppearance.BorderColor = Color.LightGray

        For Each column As UltraTreeNodeColumn In e.ColumnSet.Columns
            If column.Key = NameOf(MemolistHubStrings.Id_ColCaption) OrElse column.Key = NameOf(MemolistHubStrings.Type_ColCaption) Then
                column.Visible = False
            End If

            If column.Key = NameOf(MemolistHubStrings.Name_ColCaption) Then
                column.SortType = SortType.Ascending
                column.TipStyleCell = TipStyleCell.Show
                column.Text = MemolistHubStrings.Name_ColCaption
            End If

            If column.Key = NameOf(MemolistHubStrings.IsSelected_ColCaption) Then
                column.AllowCellEdit = AllowCellEdit.Full
                column.Text = " "
            End If

            If column.Key = NameOf(MemolistHubStrings.Comment_ColCaption) Then
                column.AllowCellEdit = AllowCellEdit.Full
                column.TipStyleCell = TipStyleCell.Show
                column.Text = MemolistHubStrings.Comment_ColCaption
            End If
        Next
    End Sub

    Private Sub utMemolist_InitializeDataNode(sender As Object, e As InitializeDataNodeEventArgs) Handles utMemolist.InitializeDataNode
        If (e.Node.IsBandNode) AndAlso (e.Node.LeftImages.Count = 0) Then
            Dim kbl_obj_type As KblObjectType = [Enum].Parse(Of KblObjectType)(e.Node.BandName)
            e.Node.Expanded = True
            e.Node.LeftImages.Add(My.Resources.Memolist.ToBitmap)
            e.Node.Text = kbl_obj_type.ToLocalizedPluralString

        ElseIf (Not e.Node.IsBandNode) Then
            If (e.Node.Parent.Nodes.Count = 1) Then
                Dim columnLayoutUpdated As Boolean = False

                For Each rootNode As UltraTreeNode In Me.utMemolist.Nodes
                    For Each node As UltraTreeNode In rootNode.Nodes
                        If (Not node.Equals(e.Node)) Then
                            For Each column As UltraTreeNodeColumn In node.DisplayColumnSetResolved.Columns
                                columnLayoutUpdated = True

                                e.Node.DisplayColumnSetResolved.Columns(column.Key).LayoutInfo.PreferredCellSize = column.LayoutInfo.PreferredCellSize
                                e.Node.DisplayColumnSetResolved.Columns(column.Key).LayoutInfo.PreferredLabelSize = column.LayoutInfo.PreferredLabelSize
                            Next

                            Exit For
                        End If
                    Next

                    If (columnLayoutUpdated) Then
                        Exit For
                    End If
                Next
            End If

            Try
                e.Node.Expanded = True
            Catch ex As Exception
                ' Node is not visible and cannot be expanded
            End Try

            e.Node.Parent.Expanded = True
        End If
    End Sub

    Private Sub utMemolist_KeyDown(sender As Object, e As KeyEventArgs) Handles utMemolist.KeyDown
        If (e.KeyCode = Keys.Delete) Then
            If (_commentCell IsNot Nothing) Then
                _memolist.Tables(_commentCell.Node.Parent.BandName).Rows.Find(_commentCell.Node.Cells(NameOf(MemolistHubStrings.Id_ColCaption)).Value)(NameOf(MemolistHubStrings.Comment_ColCaption)) = String.Empty
            Else
                RemoveSelectedNodes()
            End If
        End If
    End Sub

    Private Sub utMemolist_MouseDoubleClick(sender As Object, e As MouseEventArgs) Handles utMemolist.MouseDoubleClick
        Dim clickedNode As UltraTreeNode = Me.utMemolist.GetNodeFromPoint(e.X, e.Y)
        If (clickedNode IsNot Nothing) AndAlso (clickedNode.DisplayColumnSetResolved.Columns.Count <> 0) Then
            RaiseEvent HubDoubleClicked(Me, clickedNode.Cells(NameOf(MemolistHubStrings.Id_ColCaption)).Value.ToString)
        End If
    End Sub

    Private Sub utMemolist_MouseDown(sender As Object, e As MouseEventArgs) Handles utMemolist.MouseDown
        Dim clickedNode As UltraTreeNode = Me.utMemolist.GetNodeFromPoint(e.X, e.Y)
        If (e.Button = System.Windows.Forms.MouseButtons.Right) AndAlso (clickedNode IsNot Nothing) AndAlso (clickedNode.DisplayColumnSetResolved.Columns.Count > 0) Then
            _contextMenuTree.ShowPopup()
        End If
    End Sub

    Private Sub utmMemolist_ToolClick(sender As Object, e As ToolClickEventArgs) Handles utmMemolist.ToolClick
        Select Case e.Tool.Key
            Case "CheckAll"
                Me.utMemolist.BeginUpdate()

                For Each rootNode As UltraTreeNode In Me.utMemolist.Nodes
                    For Each node As UltraTreeNode In rootNode.Nodes
                        node.Cells(NameOf(MemolistHubStrings.IsSelected_ColCaption)).Value = True
                    Next
                Next

                Me.utMemolist.EndUpdate()

                RaiseEvent MemolistChanged(Me, "Values changed")
            Case "RemoveAll"
                If (MessageBox.Show(MemolistHubStrings.RemFromMemolist_Msg, [Shared].MSG_BOX_TITLE, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes) Then
                    Me.utMemolist.BeginUpdate()

                    For Each table As DataTable In _memolist.Tables
                        table.Rows.Clear()
                    Next

                    Me.utMemolist.EndUpdate()

                    RaiseEvent MemolistChanged(Me, "Values changed")
                End If
            Case "RemoveSelected"
                RemoveSelectedNodes()
            Case "UncheckAll"
                Me.utMemolist.BeginUpdate()

                For Each rootNode As UltraTreeNode In Me.utMemolist.Nodes
                    For Each node As UltraTreeNode In rootNode.Nodes
                        node.Cells(NameOf(MemolistHubStrings.IsSelected_ColCaption)).Value = False
                    Next
                Next

                Me.utMemolist.EndUpdate()


                RaiseEvent MemolistChanged(Me, "Values changed")
        End Select
    End Sub

    Private Sub utMemolist_VisibleChanged(sender As Object, e As EventArgs) Handles utMemolist.VisibleChanged
        utMemolist.RefreshSort(0)
    End Sub
End Class