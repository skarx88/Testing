Imports System.ComponentModel
Imports System.Runtime.Serialization
Imports Infragistics.Win
Imports Infragistics.Win.UltraWinGrid
Imports Infragistics.Win.UltraWinListView
Imports Zuken.E3.HarnessAnalyzer.Settings

Public Class DeviceBOM

    Private _selectedModules As New HashSet(Of String)
    Private _compareObjects As List(Of DeviceBomCompareObjectModuleGroup)
    Private _filter As New VisibilityFilter

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Shared Function IsModuleRowVisible(selectedModules As HashSet(Of String), [module] As String) As Boolean
        Return selectedModules.Count = 0 OrElse selectedModules.Contains([module])
    End Function

    Public Shadows Function ShowDialog(mainform As MainForm) As DialogResult
        Me.LeftDocumentComboBox.Items.AddRange(mainform.GetAllDocuments.Values.Select(Function(doc) New ValueListItem(doc, doc.Text)).ToArray())
        Me.RightDocumentComboBox.Items.AddRange(mainform.GetAllDocuments.Values.Select(Function(doc) New ValueListItem(doc, doc.Text)).ToArray())

        Me.LeftDocumentComboBox.Value = mainform.ActiveDocument
        Me.RightDocumentComboBox.Value = mainform.GetAllDocuments.Values.Cast(Of DocumentForm).Except({mainform.ActiveDocument}).FirstOrDefault

        Return MyBase.ShowDialog(mainform)
    End Function

    Private Sub Close_Button_Click(sender As Object, e As EventArgs) Handles Close_Button.Click
        Me.DialogResult = DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub Compare_Button_Click(sender As Object, e As EventArgs) Handles Compare_Button.Click
        '1. get objects by module
        Dim leftModuleObjects As Dictionary(Of String, ModuleObjects) = GetModuleObjectsFrom(LeftSelected.Document).ToDictionary(Function(mo) mo.Module.Abbreviation)
        Dim rightModuleObjects As Dictionary(Of String, ModuleObjects) = GetModuleObjectsFrom(RightSelected.Document).ToDictionary(Function(mo) mo.Module.Abbreviation)

        '2. pair modules
        Dim pairsByModule As New Dictionary(Of String, ModuleObjectsPair)
        For Each kv As KeyValuePair(Of String, ModuleObjects) In leftModuleObjects
            Dim left As ModuleObjects = kv.Value
            Dim right As ModuleObjects = Nothing
            If rightModuleObjects.ContainsKey(kv.Key) Then
                ' in both documents 
                right = rightModuleObjects(kv.Key)
            Else
                ' only in left document
            End If

            Dim pair As New ModuleObjectsPair(left, right)
            pairsByModule.Add(kv.Key, pair)
        Next

        For Each kv As KeyValuePair(Of String, ModuleObjects) In rightModuleObjects
            If Not pairsByModule.ContainsKey(kv.Key) Then
                ' only in right document
                Dim pair As New ModuleObjectsPair(Nothing, kv.Value)
                pairsByModule.Add(kv.Key, pair)
            End If
        Next

        UltraListView1.Items.Clear() ' Clear the list view before adding new items

        '3. compare pairs
        Dim comparedByModule As New Dictionary(Of String, DeviceBomCompareObjectModuleGroup)
        Dim moduleBmp As Bitmap = My.Resources.HarnessModule.ToBitmap
        For Each pair As ModuleObjectsPair In pairsByModule.Values
            Dim compareObjectsCollection As New DeviceBomCompareObjectModuleGroup(pair.Module)
            compareObjectsCollection.CompareObjects.AddRange(pair.Compare(LeftSelected?.Document?.Kbl, RightSelected?.Document?.Kbl))
            comparedByModule.Add(pair.ModuleKey, compareObjectsCollection)

            Me.UltraListView1.Items.Add(New UltraListViewItem(pair.ModuleKey) With {
                .Value = pair.Module.Abbreviation,
                .Appearance = New Appearance With {
                    .Image = moduleBmp
                }
            })
        Next

        Me.UltraGrid1.DisplayLayout.Override.RowFilterMode = RowFilterMode.AllRowsInBand ' Set the row filter mode to always filter in client and use the grid filters (not the data source!)
        Me.UltraGrid1.DisplayLayout.Bands(1).ColumnFilters("Visible").FilterConditions.Add(_filter)
        _compareObjects = comparedByModule.Values.ToList()
        UpdateDataSource()
    End Sub

    Private Sub UpdateDataSource()
        Dim list As List(Of DeviceBomCompareObjectModuleGroup) = _compareObjects.Where(Function(obj) IsModuleRowVisible(_selectedModules, obj.Module)).ToList
        Me.BindingSource1.DataSource = New BindingList(Of DeviceBomCompareObjectModuleGroup)(list)
        Me.UltraGrid1.DisplayLayout.RefreshFilters()
        Me.UltraGrid1.Rows.ExpandAll(True)
    End Sub

    Private Function GetModuleObjectsFrom(document As IKblProvider) As List(Of ModuleObjects)
        Dim all_module_objects As New List(Of ModuleObjects)
        For Each m As [Module] In CType(document.Kbl, E3.Lib.Schema.Kbl.KblMapper).GetModules
            Dim moduleObjects As New ModuleObjects(m)
            moduleObjects.AddRange(CType(document.Kbl, E3.Lib.Schema.Kbl.KblMapper).GetObjectsOfModule(m.SystemId).OfType(Of IKblOccurrence))
            all_module_objects.Add(moduleObjects)
        Next
        Return all_module_objects
    End Function

    Private Sub UltraListView1_ItemSelectionChanged(sender As Object, e As ItemSelectionChangedEventArgs) Handles UltraListView1.ItemSelectionChanged
        _selectedModules.Clear()
        _selectedModules.AddRange(Me.UltraListView1.SelectedItems.Cast(Of UltraListViewItem).Select(Function(item) item.Key).Distinct)
        UpdateDataSource()
    End Sub

    Private ReadOnly Property LeftSelected As DocumentForm
        Get
            Return TryCast(Me.LeftDocumentComboBox.Value, DocumentForm)
        End Get
    End Property

    Private ReadOnly Property RightSelected As DocumentForm
        Get
            Return TryCast(Me.RightDocumentComboBox.Value, DocumentForm)
        End Get
    End Property

    Private Class ModuleObjects
        Inherits System.Collections.ObjectModel.KeyedCollection(Of String, IKblOccurrence)

        Public Sub New([module] As E3.Lib.Schema.Kbl.Module)
            MyBase.New()
            Me.Module = [module]
        End Sub

        ReadOnly Property [Module] As E3.Lib.Schema.Kbl.Module

        Public Overrides Function ToString() As String
            Return String.Format("Key={0}, Count={1}", Me.Module?.Abbreviation, Me.Count)
        End Function

        ReadOnly Property ModuleKey As String
            Get
                Return Me.Module?.Abbreviation
            End Get
        End Property

        Protected Overrides Function GetKeyForItem(item As IKblOccurrence) As String
            Return GetObjectKey(item)
        End Function

        Public Shared Function GetObjectKey(item As IKblOccurrence) As String
            If item Is Nothing Then
                Return Guid.NewGuid.ToString()
            End If
            Return item.SystemId
        End Function

    End Class

    Public Class DeviceBomCompareObject

        Private _left As IKblOccurrence
        Private _right As IKblOccurrence
        Private _left_Part As IKblPartObject
        Private _right_Part As IKblPartObject

        Public Sub New(left As IKblOccurrence, right As IKblOccurrence, leftContainer As IKblContainer, rightContainer As IKblContainer)
            _left = left
            _right = right

            Dim leftKey As String = ModuleObjects.GetObjectKey(left)
            Dim rightKey As String = ModuleObjects.GetObjectKey(right)

            If left IsNot Nothing AndAlso right IsNot Nothing Then
                If leftKey <> rightKey Then
                    Throw New ArgumentException("Left and Right objects must be the same.")
                End If
            End If

            Me.Key = If(leftKey, rightKey)

            Me.PartNumberLeft = If(left?.Part, String.Empty)
            Me.PartNumberRight = If(right?.Part, String.Empty)

            _left_Part = left?.GetPart(leftContainer)
            _right_Part = right?.GetPart(rightContainer)

            Me.PartTypeLeft = _left_Part?.Part_number_type
            Me.PartTypeRight = _right_Part?.Part_number_type

        End Sub

        ReadOnly Property Key As String
        ReadOnly Property PartNumberLeft As String
        ReadOnly Property PartTypeLeft As String
        ReadOnly Property PartNumberRight As String
        ReadOnly Property PartTypeRight As String
        ReadOnly Property [Module] As String
        Property CurrentLength As String

        ReadOnly Property StatusLeft As CompareChangeType
            Get
                If _left_Part IsNot Nothing AndAlso _right_Part IsNot Nothing Then
                    If _left_Part.Part_Number = _right_Part.Part_Number Then
                        Return CompareChangeType.Unchanged
                    Else
                        Return CompareChangeType.Modified
                    End If
                ElseIf _left_Part Is Nothing Then
                    Return CompareChangeType.Deleted 'Removed from Left Document
                ElseIf _right_Part Is Nothing Then
                    Return CompareChangeType.Unchanged ' removed from Right Document
                Else
                    Return CompareChangeType.Unknown
                End If
            End Get
        End Property

        ReadOnly Property StatusRight As CompareChangeType
            Get
                If _left_Part IsNot Nothing AndAlso _right_Part IsNot Nothing Then
                    If _left_Part.Part_Number = _right_Part.Part_Number Then
                        Return CompareChangeType.Unchanged
                    Else
                        Return CompareChangeType.Modified
                    End If
                ElseIf _left_Part Is Nothing Then
                    Return CompareChangeType.Unchanged 'Removed from Left Document
                ElseIf _right_Part Is Nothing Then
                    Return CompareChangeType.Deleted ' removed from Right Document
                Else
                    Return CompareChangeType.Unknown
                End If
            End Get
        End Property

        ReadOnly Property Visible As Boolean
            Get
                Return Me.StatusLeft <> CompareChangeType.Unchanged OrElse Me.StatusRight <> CompareChangeType.Unchanged
            End Get
        End Property

    End Class

    Private Class VisibilityFilter
        Inherits Infragistics.Win.UltraWinGrid.FilterCondition

        Public Sub New()
            MyBase.New()
        End Sub

        Protected Sub New(info As SerializationInfo, context As StreamingContext)
            MyBase.New(info, context)
        End Sub

        Public Overrides Function MeetsCriteria(row As UltraGridRow) As Boolean
            Dim obj As DeviceBomCompareObject = TryCast(row.ListObject, DeviceBomCompareObject)
            If obj IsNot Nothing Then
                If obj.Visible Then
                    If OnIsVisible IsNot Nothing Then
                        OnIsVisible.Invoke(obj)
                    Else
                        Return True
                    End If
                End If
            End If
            Return False
        End Function

        Property OnIsVisible As Func(Of DeviceBomCompareObject, Boolean)

    End Class

    Private Class DeviceBomCompareObjectModuleGroup

        Private _module As E3.Lib.Schema.Kbl.Module

        Public Sub New([module] As E3.Lib.Schema.Kbl.Module)
            Me._module = [module]
        End Sub

        ReadOnly Property [Module] As String
            Get
                Return _module?.Abbreviation
            End Get
        End Property

        ReadOnly Property CompareObjects As New BindingList(Of DeviceBomCompareObject)

    End Class

    Private Class ModuleObjectsPair

        Public Sub New(left As ModuleObjects, right As ModuleObjects)
            Me.Left = left
            Me.Right = right
            If Me.Left IsNot Nothing AndAlso Me.Right IsNot Nothing AndAlso Me.Left.ModuleKey <> Me.Right.ModuleKey Then
                Throw New ArgumentException("Left and Right ModuleObjects must have the same Module Abbreviation.")
            End If
        End Sub

        ReadOnly Property ModuleKey As String
            Get
                Return If(Left?.ModuleKey, Right?.ModuleKey)
            End Get
        End Property

        ReadOnly Property [Module] As E3.Lib.Schema.Kbl.Module
            Get
                Return If(Left?.Module, Right?.Module)
            End Get
        End Property

        ReadOnly Property Left As ModuleObjects
        ReadOnly Property Right As ModuleObjects

        Public Function Compare(leftContainer As IKblContainer, rightContainer As IKblContainer) As ICollection(Of DeviceBomCompareObject)
            Dim pairs As New Dictionary(Of String, DeviceBomCompareObject)
            For Each left_kbl_obj As IKblOccurrence In Me.Left
                Dim left_id As String = ModuleObjects.GetObjectKey(left_kbl_obj)
                If Me.Right.Contains(left_id) Then
                    ' Object exists in both documents
                    Dim right_kbl_obj As IKblOccurrence = Me.Right(left_id)
                    Dim compareObject As New DeviceBomCompareObject(left_kbl_obj, right_kbl_obj, leftContainer, rightContainer)
                    pairs.Add(left_id, compareObject)
                Else
                    ' Object only in left document
                    Dim compareObject As New DeviceBomCompareObject(left_kbl_obj, Nothing, leftContainer, rightContainer)
                    pairs.Add(left_id, compareObject)
                End If
            Next

            For Each right_kbl_obj As IKblOccurrence In Me.Right
                Dim right_id As String = ModuleObjects.GetObjectKey(right_kbl_obj)
                If Not pairs.ContainsKey(right_id) Then
                    ' Object only in right document
                    Dim compareObject As New DeviceBomCompareObject(Nothing, right_kbl_obj, leftContainer, rightContainer)
                    pairs.Add(right_id, compareObject)
                End If
            Next
            Return pairs.Values
        End Function

    End Class

    Private Sub ClearSelectionToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ResetSelectionToolStripMenuItem.Click
        UltraListView1.SelectedItems.Clear
    End Sub

    Private Sub ContextMenuStrip1_Opening(sender As Object, e As CancelEventArgs) Handles ContextMenuStrip1.Opening
        Dim pos As Point = UltraListView1.PointToClient(MousePosition)
        Dim item As UltraListViewItem = UltraListView1.ItemFromPoint(pos.X, pos.Y)
        If item IsNot Nothing Then
            UltraListView1.SelectedItems.Clear
            UltraListView1.SelectedItems.Add(item)
            UltraListView1.ActiveItem = item
        End If
    End Sub

    Private Sub CollapseAllToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CollapseAllToolStripMenuItem.Click
        Me.UltraGrid1.Rows.CollapseAll(True)
    End Sub

    Private Sub ExpandAllModulesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExpandAllModulesToolStripMenuItem.Click
        Me.UltraGrid1.Rows.ExpandAll(True)
    End Sub

End Class