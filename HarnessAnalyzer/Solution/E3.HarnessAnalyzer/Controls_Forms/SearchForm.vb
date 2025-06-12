Imports Infragistics.Win.UltraWinTree

<Reflection.Obfuscation(Feature:="renaming", ApplyToMembers:=False, Exclude:=True)> ' needed to avoid resource-file-names obfuscation errors, see issue #5
Public Class SearchForm

    Friend Event SearchObject(sender As Object, mapperSourceId As String, objectType As String, objectId As String)
    Friend Event CloseSearchForm()

    Private _gridAppearances As Dictionary(Of KblObjectType, GridAppearance)
    Private _isTouchEnabled As Boolean
    Private _nodeStack As Dictionary(Of Integer, List(Of UltraTreeNode))
    Private _searchPatterns As Dictionary(Of String, SearchPattern)
    Private _searchString As String

    Public Sub New(gridAppearances As Dictionary(Of KblObjectType, GridAppearance), inactiveObjects As Dictionary(Of String, ITypeGroupedKblIds), isTouchEnabled As Boolean, searchPatterns As Dictionary(Of String, SearchPattern))
        InitializeComponent()

        Me.BackColor = Color.White
        Me.Icon = My.Resources.Search
        Me.Text = SearchFormStrings.Caption

        _gridAppearances = gridAppearances
        _isTouchEnabled = isTouchEnabled
        _nodeStack = New Dictionary(Of Integer, List(Of UltraTreeNode))
        _searchPatterns = searchPatterns

        InitializeTree(inactiveObjects)
    End Sub

    Private Sub InitializeTree(inactiveObjects As Dictionary(Of String, ITypeGroupedKblIds))
        Me.utSearchResults.BeginUpdate()
        Me.utSearchResults.Nodes.Clear()

        Try
            For Each kvMapper As KeyValuePair(Of String, SearchPattern) In _searchPatterns
                Dim harnessNode As New UltraTreeNode(kvMapper.Key.ToString, String.Format("{0} [{1}]", kvMapper.Value.HarnessPartnumber, kvMapper.Value.HarnessVersion))
                harnessNode.LeftImages.Add(My.Resources.Harness.ToBitmap)

                For Each ot As KeyValuePair(Of KblObjectType, Dictionary(Of String, ObjectSearchPattern)) In kvMapper.Value.Content
                    Dim objectTypeNode As New UltraTreeNode(String.Format("{0}|{1}", harnessNode.Key, ot.Key), If(_gridAppearances.Values.Any(Function(gridAppearance) gridAppearance.GridTable.Type = ot.Key), _gridAppearances.Values.Where(Function(gridAppearance) gridAppearance.GridTable.Type = ot.Key).FirstOrDefault.GridTable.Description, _gridAppearances.Values.Where(Function(gridAppearance) gridAppearance.GridTable.GridSubTable IsNot Nothing AndAlso gridAppearance.GridTable.GridSubTable.Type = ot.Key).FirstOrDefault.GridTable.GridSubTable.Description))
                    objectTypeNode.LeftImages.Add(My.Resources.GridTable.ToBitmap)

                    'If (ot.Key = KblObjectType.Wire_occurrence) Then
                    '    objectTypeNode.Text = String.Format("{0}/{1}", ot.Key, KblObjectType.Core_occurrence.ToString)
                    'End If

                    For Each searchObject As KeyValuePair(Of String, ObjectSearchPattern) In ot.Value
                        Dim objectNode As New UltraTreeNode(String.Format("{0}|{1}", harnessNode.Key, searchObject.Key), searchObject.Value.DisplayText)
                        With objectNode
                            If inactiveObjects.ContainsKey(kvMapper.Key) Then
                                For Each objectList As IEnumerable(Of String) In inactiveObjects(kvMapper.Key)
                                    For Each searchObjectId As String In searchObject.Key.SplitRemoveEmpty("|"c)
                                        If (objectList.Contains(searchObjectId)) Then
                                            .Override.NodeAppearance.ForeColor = Color.LightGray
                                            Exit For
                                        End If
                                    Next

                                    If (.Override.NodeAppearance.ForeColor = Color.LightGray) Then
                                        Exit For
                                    End If
                                Next
                            End If

                            If (.Text = String.Empty) Then
                                .Text = KblObjectType.Undefined.ToLocalizedString
                            End If

                            .Tag = searchObject.Value.PropertyValuePattern
                            .Visible = False
                        End With

                        objectTypeNode.Nodes.Add(objectNode)
                    Next

                    harnessNode.Nodes.Add(objectTypeNode)
                Next

                Me.utSearchResults.Nodes.Add(harnessNode)
            Next

            Me.utSearchResults.ExpandAll(ExpandAllType.Always)
            Me.utSearchResults.Override.SortComparer = New NumericStringSortComparer
            Me.utSearchResults.Override.Sort = SortType.Ascending
        Catch ex As Exception
            ex.ShowMessageBox(String.Format(SearchFormStrings.LoadOfSearchFailed_Msg, vbCrLf, ex.Message))
            Me.utSearchResults.Nodes.Clear()
        Finally
            Me.utSearchResults.EndUpdate()
        End Try
    End Sub


    Private Sub SearchForm_FormClosed(ByVal sender As Object, ByVal e As FormClosedEventArgs) Handles Me.FormClosed
        RaiseEvent CloseSearchForm()
        Me.Dispose(True)
    End Sub

    Private Sub SearchForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        If (_isTouchEnabled) Then
            Dim path As String = Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles) & "\Microsoft Shared\ink\TabTip.exe"
            If (Not IO.File.Exists(path)) Then
                path = Environment.GetFolderPath(Environment.SpecialFolder.System) & "\osk.exe"
            End If

            If (IO.File.Exists(path)) Then
                System.Diagnostics.ProcessEx.KillAll(IO.Path.GetFileNameWithoutExtension(path))
            End If
        End If
    End Sub

    Private Sub SearchForm_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        If (e.KeyCode = Keys.Escape) Then
            Me.Close()
        End If
    End Sub

    Private Sub SearchForm_Shown(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Shown
        Me.txtSearchString.Focus()
    End Sub

    Private Sub btnClose_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnClose.Click
        Me.Close()
    End Sub

    Private Sub txtSearchString_GotFocus(sender As Object, e As EventArgs) Handles txtSearchString.GotFocus
        If (_isTouchEnabled) Then
            Dim path As String = Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles) & "\Microsoft Shared\ink\TabTip.exe"
            If (Not IO.File.Exists(path)) Then
                path = Environment.GetFolderPath(Environment.SpecialFolder.System) & "\osk.exe"
            End If

            ProcessEx.TryStart(path)
        End If
    End Sub

    Private Sub txtSearchString_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles txtSearchString.TextChanged
        Me.Cursor = Cursors.WaitCursor

        Me.utSearchResults.BeginUpdate()

        For Each harnessNode As UltraTreeNode In Me.utSearchResults.Nodes
            For Each objTypeNode As UltraTreeNode In harnessNode.Nodes
                For Each objNode As UltraTreeNode In objTypeNode.Nodes
                    objNode.Visible = False
                Next
            Next
        Next

        _searchString = Me.txtSearchString.Text

        If (_searchString = String.Empty) Then
            _nodeStack.Clear()
        Else
            If (Me.uckIgnoreBlanks.Checked) Then
                _searchString = Replace(_searchString, " ", String.Empty)
            End If

            If (_nodeStack.ContainsKey(_searchString.Length)) Then
                Dim stackCount As Integer = _nodeStack.Count
                For stackCounter As Integer = stackCount To _searchString.Length Step -1
                    _nodeStack.Remove(stackCounter)
                Next
            End If

            Dim nodeList As New List(Of UltraTreeNode)

            For Each kvMapperSearchPattern As KeyValuePair(Of String, SearchPattern) In _searchPatterns
                For Each ot As KeyValuePair(Of KblObjectType, Dictionary(Of String, ObjectSearchPattern)) In kvMapperSearchPattern.Value.Content
                    For Each searchObject As KeyValuePair(Of String, ObjectSearchPattern) In ot.Value
                        For Each searchProperty As String In searchObject.Value.PropertyValuePattern.SplitRemoveEmpty("|"c)
                            Dim index As Integer = -1

                            If (Me.uckIgnoreBlanks.Checked) Then
                                searchProperty = Replace(searchProperty, " ", String.Empty)
                            End If

                            If (Me.uckCaseSensitive.Checked) Then
                                index = searchProperty.IndexOf(_searchString, StringComparison.InvariantCulture)
                            Else
                                index = searchProperty.IndexOf(_searchString, StringComparison.InvariantCultureIgnoreCase)
                            End If

                            If ((index = 0) AndAlso (Me.uckBeginsWith.Checked)) OrElse ((index >= 0) AndAlso (Not Me.uckBeginsWith.Checked)) Then
                                nodeList.Add(Me.utSearchResults.GetNodeByKey(String.Format("{0}|{1}", kvMapperSearchPattern.Key, searchObject.Key)))

                                Exit For
                            End If
                        Next
                    Next
                Next
            Next

            If (_nodeStack.ContainsKey(_searchString.Length)) Then
                _nodeStack(_searchString.Length) = nodeList
            Else
                _nodeStack.Add(_searchString.Length, nodeList)
            End If
        End If

        If (_nodeStack.ContainsKey(_searchString.Length)) Then
            For Each node As UltraTreeNode In _nodeStack(_searchString.Length)
                If (node IsNot Nothing) Then
                    node.Visible = True
                End If
            Next
        End If

        Me.utSearchResults.EndUpdate()

        Me.Cursor = Cursors.Default
    End Sub

    Private Sub uckBeginsWith_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles uckBeginsWith.CheckedChanged
        txtSearchString_TextChanged(sender, e)

        Me.txtSearchString.Focus()
    End Sub

    Private Sub uckCaseSensitive_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles uckCaseSensitive.CheckedChanged
        txtSearchString_TextChanged(sender, e)

        Me.txtSearchString.Focus()
    End Sub

    Private Sub uckIgnoreBlanks_CheckedChanged(sender As Object, e As EventArgs) Handles uckIgnoreBlanks.CheckedChanged
        txtSearchString_TextChanged(sender, e)

        Me.txtSearchString.Focus()
    End Sub

    Private Sub utSearchResults_MouseDoubleClick(ByVal sender As Object, ByVal e As MouseEventArgs) Handles utSearchResults.MouseDoubleClick
        Using Me.BeginTryWaitCursorBlock
            Dim node As UltraTreeNode = Me.utSearchResults.GetNodeFromPoint(e.X, e.Y)
            If (node IsNot Nothing) AndAlso (node.LeftImages.Count = 0) Then
                RaiseEvent SearchObject(Me, node.Parent.Parent.Key, node.Parent.Key.Split("|"c, StringSplitOptions.RemoveEmptyEntries)(1), node.Key.Split("|"c, StringSplitOptions.RemoveEmptyEntries)(1))
            End If
        End Using
    End Sub

End Class