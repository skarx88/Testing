Imports System.ComponentModel
Imports System.IO
Imports devDept.Eyeshot
Imports Infragistics.Win
Imports Infragistics.Win.UltraWinListView
Imports Infragistics.Win.UltraWinToolTip

Namespace D3D.Consolidated.Controls

    <Reflection.Obfuscation(Feature:="renaming", ApplyToMembers:=False, Exclude:=True)> ' needed to avoid resource-file-names obfuscation errors, see issue #5
    Public Class CarModelsViewControl

        Public Event UserRequestedLoadCarModelAsync(sender As Object, e As AsyncUserRequestedLoadCarEventArgs)
        Public Event UserRequestedUnLoadCarModelAsync(sender As Object, e As Threading.AsyncCallBackEventArgs)

        Private Const PREVIEW_FILE_FILTER As String = "*.exif"
        Private Const IMG_DESCRIPTION As Integer = &H10E

        Private _directory As String = String.Empty
        Private _asyncDirectoryLoadingStack As New System.Collections.Concurrent.ConcurrentStack(Of String)
        Private _isDirectoryLoaded As Boolean
        Private _design As devDept.Eyeshot.Design
        Private _currentPopToolTip As UltraToolTipInfo
        Private _itemsLastChecked As New List(Of UltraListViewItem)
        Private _itemsEnabled As Boolean = True
        Private _lock As New System.Threading.LockStateMachine
        Private WithEvents _popUpTimer As New System.Timers.Timer
        Private WithEvents _carStateMachine As CarStateMachine

        Public Sub New()
            InitializeComponent()

            _popUpTimer.Interval = 400
            _popUpTimer.AutoReset = False
            _popUpTimer.SynchronizingObject = Me
        End Sub

        Property CarStateMachine As CarStateMachine
            Get
                Return _carStateMachine
            End Get
            Set
                _carStateMachine = Value
            End Set
        End Property

        Property Directory As String
            Get
                Return _directory
            End Get
            Set(value As String)
                Dim s1 As String = If(Not String.IsNullOrEmpty(value), value.ToLower, String.Empty)
                Dim s2 As String = If(Not String.IsNullOrEmpty(_directory), _directory.ToLower, String.Empty)
                If s2 <> s1 Then
                    _directory = value
                    _isDirectoryLoaded = False
                    OnAfterDirectoryChanged()
                End If
            End Set
        End Property

        Private Sub OnAfterDirectoryChanged()
        End Sub

        Public ReadOnly Property CanDirectoryLoaded As Boolean
            Get
                Return _design.IsHandleCreated
            End Get
        End Property

        Public ReadOnly Property IsDirectoryLoaded As Boolean
            Get
                Return _isDirectoryLoaded
            End Get
        End Property

        Private Function CreateTempViewPortLayout(template As devDept.Eyeshot.Design) As devDept.Eyeshot.Design
            ArgumentNullException.ThrowIfNull(template)
            Dim tempVp As devDept.Eyeshot.Design = New Design

            With tempVp
                .InitializeViewports()
            End With
            template.CopySettingsTo(tempVp)
            Return tempVp
        End Function

        Public Async Sub LoadDirectoryAsync(Optional onNewItemAdded As Action(Of UltraListViewItem) = Nothing)
            Dim loadingDir As String = Me.Directory
            If loadingDir Is Nothing Then
                loadingDir = String.Empty
            End If

            If Not String.IsNullOrEmpty(loadingDir) Then
                _asyncDirectoryLoadingStack.Push(loadingDir)

                Using Await _lock.BeginWaitAsync
                    Me.UltraListView1.Visible = False
                    changeCarModelPathPanel.Visible = True
                    btnChangeCarModelPath.Visible = True

                    Dim allDirectores As String() = Array.Empty(Of String)
                    _asyncDirectoryLoadingStack.TryPopAll(allDirectores)

                    Dim lastPushedDirectoryPath As String = allDirectores.FirstOrDefault

                    If Not String.IsNullOrWhiteSpace(lastPushedDirectoryPath) Then
                        Me.UltraListView1.ViewSettingsDetails.ImageSize = New Size(50, 50)

                        Await Task.Factory.StartNew(
                            Sub()
                                Threading.SpinWait.SpinUntil(Function() CanDirectoryLoaded)

                                Dim isEmpty As Boolean = False
                                Dim supportedExtensions As String() = D3D.Consolidated.Designs.ObjectFileLoader.SupportedFileFilters.Select(Function(ff) Consolidated.Designs.ObjectFileLoader.GetExtensionFromFilter(ff)).ToArray

                                Me.UltraListView1.InvokeOrDefault( ' HINT: switch back to UI-thread, now it's possible that dispose is finished, re-check is auto-implemented in InvokeOrDefault
                                Sub()
                                    Me.UltraListView1.BeginUpdate()
                                    Me.UltraListView1.Items.Cast(Of UltraListViewItem).Where(Function(item) item.Appearance.Image IsNot Nothing).ForEach(Sub(item) DirectCast(item.Appearance.Image, Bitmap).Dispose())
                                    Me.UltraListView1.Items.Clear()
                                    Me.UltraListView1.EndUpdate()
                                End Sub)

                                Dim dir As New DirectoryInfo(lastPushedDirectoryPath)
                                If dir.Exists Then
                                    For Each file As FileInfo In dir.GetFiles("*.*", SearchOption.TopDirectoryOnly)
                                        Dim extfile As String = file.Extension.TrimStart("."c)
                                        If supportedExtensions.Contains(extfile) Then
                                            Dim fileName As String = IO.Path.GetFileNameWithoutExtension(file.FullName)
                                            Me.UltraListView1.InvokeOrDefault( ' HINT: switch back to UI-thread, now it's possible that dispose is finished, re-check is auto-implemented in InvokeOrDefault
                                            Sub()
                                                Dim newItem As UltraListViewItem = Me.UltraListView1.Items.Add(file.FullName.ToLower, fileName)
                                                newItem.Tag = file.FullName
                                                If onNewItemAdded IsNot Nothing Then
                                                    onNewItemAdded.Invoke(newItem)
                                                End If
                                            End Sub)
                                        End If

                                        If Me.IsDisposedOrDisposing Then ' HINT: abort on dispose
                                            Exit For
                                        End If
                                    Next

                                    If Not IsDisposedOrDisposing() Then
                                        For Each item As UltraListViewItem In Me.UltraListView1.Items.All.ToArray
                                            Dim file As String = item.Tag.ToString
                                            Dim previewPng As String = String.Concat(file, PREVIEW_FILE_FILTER.TrimStart("*"c))
                                            Dim previewBitmap As Bitmap = Nothing

                                            If IO.File.Exists(previewPng) Then
                                                previewBitmap = New Bitmap(previewPng)
                                            Else
                                                _design.InvokeOrDefault(Sub() _design.Clear()) ' HINT: switch back to UI-thread, now it's possible that dispose is finished, re-check is auto-implemented in InvokeOrDefault
                                                Dim myReadObj As Translators.ReadFileAsync
                                                Try
                                                    myReadObj = D3D.Consolidated.Designs.ObjectFileLoader.GetReader(file)
                                                    myReadObj.DoWork()
                                                    myReadObj.Entities.ForEach(Sub(entity) entity.ColorMethod = Entities.colorMethodType.byEntity)

                                                    _design.InvokeOrDefault( ' HINT: switch back to UI-thread, now it's possible that dispose is finished, re-check is auto-implemented in InvokeOrDefault
                                                    Sub()
                                                        myReadObj.AddToScene(_design)
                                                        _design.ZoomFit()
                                                        previewBitmap = _design.RenderToBitmap(1, False)
                                                    End Sub)
                                                Catch ex As Exception
                                                    previewBitmap = CType(My.Resources.Error_Large.Clone, Bitmap)
                                                    previewBitmap.SetPropertyItemValue(IMG_DESCRIPTION, ex.Message)
                                                End Try

                                                If previewBitmap IsNot Nothing Then
                                                    previewBitmap.Save(previewPng, System.Drawing.Imaging.ImageFormat.Exif)
                                                End If
                                            End If

                                            Me.UltraListView1.InvokeOrDefault( ' HINT: switch back to UI-thread, now it's possible that dispose is finished, re-check is auto-implemented in InvokeOrDefault
                                            Sub() item.Appearance.Image = previewBitmap)

                                            If IsDisposedOrDisposing() Then
                                                Exit For
                                            End If
                                        Next
                                    End If
                                End If
                            End Sub)
                        If Not IsDisposedOrDisposing() Then
                            _isDirectoryLoaded = True
                        End If
                    End If

                    If Not IsDisposedOrDisposing() Then
                        Me.UltraListView1.Visible = UltraListView1.Items.Count > 0
                        changeCarModelPathPanel.Visible = Not Me.UltraListView1.Visible
                        btnChangeCarModelPath.Visible = Not Me.UltraListView1.Visible
                    End If
                End Using
            End If
        End Sub

        Property CurrentCarFilePath As String
            Get
                If Me.UltraListView1.CheckedItems.Count > 0 Then
                    Dim chkItem As UltraListViewItem = Me.UltraListView1.CheckedItems.Single
                    Return chkItem.Tag.ToString
                End If
                Return Nothing
            End Get
            Set(value As String)
                For Each chkItem As UltraListViewItem In Me.UltraListView1.CheckedItems.ToArray
                    chkItem.ResetCheckState()
                Next

                If Not String.IsNullOrEmpty(value) Then
                    Dim eqItem As UltraListViewItem = Me.UltraListView1.Items.Cast(Of UltraListViewItem).Where(Function(item) item.Tag.ToString.ToUpper = value.ToUpper).FirstOrDefault
                    If eqItem IsNot Nothing Then
                        eqItem.CheckState = CheckState.Checked
                    End If
                End If
            End Set
        End Property

        Public Function FindItemsWithFileName(fileName As String) As UltraListViewItem()
            Dim list As New List(Of UltraListViewItem)
            If Not String.IsNullOrEmpty(fileName) Then
                fileName = IO.Path.GetFileName(fileName).ToLower
                For Each item As UltraListViewItem In Me.UltraListView1.Items
                    Dim itemFileName As String = IO.Path.GetFileName(item.Key)
                    If itemFileName = fileName Then
                        list.Add(item)
                    End If
                Next
            End If
            Return list.ToArray
        End Function

        Public Function GetItemWithKey(key As String) As UltraListViewItem
            If UltraListView1.Items.Exists(key.ToLower) Then
                Return Me.UltraListView1.Items(key.ToLower)
            End If
            Return Nothing
        End Function

        Private Function IsDisposedOrDisposing() As Boolean
            Return Me.Disposing OrElse Me.IsDisposed
        End Function

        Private Sub CarModelSelectCtrl_ParentChanged(sender As Object, e As EventArgs) Handles Me.ParentChanged
            Dim consolidated3DControl As Consolidated3DControl = FindParentOf(Of Consolidated3DControl)(Me)
            If _design Is Nothing AndAlso consolidated3DControl IsNot Nothing Then
                If consolidated3DControl.Design3D Is Nothing Then
                    Throw New NullReferenceException("Property:" + consolidated3DControl.GetType.FullName + "." + NameOf(consolidated3DControl.Design3D) + "is null!")
                End If

                _design = CreateTempViewPortLayout(consolidated3DControl.Design3D)
                Me.Controls.Add(_design)
            End If
        End Sub

        Private Function FindParentOf(Of T As Control)(ctrl As Control) As T
            If ctrl.Parent IsNot Nothing Then
                If TypeOf ctrl.Parent Is T Then
                    Return CType(ctrl.Parent, T)
                Else
                    Return FindParentOf(Of T)(ctrl.Parent)
                End If
            End If
            Return Nothing
        End Function

        Public Async Function TrySetItemChecked(key As String, checked As Boolean, Optional [raiseEvent] As Boolean = True) As Task(Of UltraListViewItem)
            Using Await _lock.BeginWaitAsync()
                Using Me.UltraListView1.EventManager.ProtectProperty(NameOf(UltraListView1.EventManager.AllEventsEnabled), [raiseEvent])
                    Dim item As UltraListViewItem = Me.GetItemWithKey(key)
                    If item IsNot Nothing Then
                        item.CheckState = If(checked, CheckState.Checked, CheckState.Unchecked)
                        UltraListView1.SelectedItems.Clear()
                        item.Activate()
                        UltraListView1.SelectedItems.Add(item)
                        item.BringIntoView()
                    End If
                    Return item
                End Using
            End Using
        End Function

        Private Async Sub UltraListView1_ItemCheckStateChanged(sender As Object, e As ItemCheckStateChangedEventArgs) Handles UltraListView1.ItemCheckStateChanged
            Await ProcessRequestLoadCarModelFromItem(e.Item)
        End Sub

        Private Async Function ProcessRequestLoadCarModelFromItem(item As UltraListViewItem) As Task
            If Visible Then
                Using Await _lock.BeginWaitAsync()
                    If item.CheckState = CheckState.Checked Then
                        Using Me.ProtectProperty(NameOf(ItemsEnabled), False)
                            Using Me.UltraListView1.EventManager.ProtectProperty(NameOf(UltraListView1.EventManager.AllEventsEnabled), False)
                                For Each ulvItem As UltraListViewItem In _itemsLastChecked
                                    If ulvItem.CheckState <> CheckState.Unchecked Then
                                        ulvItem.CheckState = CheckState.Unchecked
                                        Await ProcessUnloadCarModelFromItemCore(ulvItem) ' HINT: use core method to avoid another lockLoad (dead-lock)
                                    End If
                                Next
                                _itemsLastChecked.Clear()
                            End Using

                            Dim args As New AsyncUserRequestedLoadCarEventArgs(item.Key)
                            OnUserRequestedLoadCarModel(args)
                            Await args.WaitFinishedAsync

                            If args.Cancelled Then
                                Using Me.UltraListView1.EventManager.ProtectProperty(NameOf(UltraListView1.EventManager.AllEventsEnabled), False)
                                    item.CheckState = CheckState.Unchecked
                                End Using
                            End If

                        End Using ' HINT: restore itemsEnabled old state
                    Else
                        Await ProcessUnloadCarModelFromItemCore(item) ' HINT: use core method to avoid another lockLoad (dead-lock)
                    End If
                End Using
            End If
        End Function

        Private Async Function ProcessUnloadCarModelFromItem(item As UltraListViewItem) As Task
            Using Await _lock.BeginWaitAsync()
                Await ProcessUnloadCarModelFromItemCore(item)
            End Using
        End Function

        Private Async Function ProcessUnloadCarModelFromItemCore(item As UltraListViewItem) As Task
            Dim args As New Threading.AsyncCallBackEventArgs()
            OnUserRequestedUnLoadCarModel(args)
            Await args.WaitFinishedAsync
        End Function

        Private Sub OnUserRequestedLoadCarModel(e As AsyncUserRequestedLoadCarEventArgs)
            RaiseEvent UserRequestedLoadCarModelAsync(Me, e)
        End Sub

        Private Sub OnUserRequestedUnLoadCarModel(e As Threading.AsyncCallBackEventArgs)
            RaiseEvent UserRequestedUnLoadCarModelAsync(Me, e)
        End Sub

        Private Async Sub UltraListView1_ItemCheckStateChanging(sender As Object, e As ItemCheckStateChangingEventArgs) Handles UltraListView1.ItemCheckStateChanging
            If e.NewValue = CheckState.Checked Then
                Using Await _lock.BeginWaitAsync()
                    _itemsLastChecked = Me.UltraListView1.CheckedItems.ToList()
                End Using
            End If
        End Sub

        Private Async Sub UltraListView1_ItemDoubleClick(sender As Object, e As ItemDoubleClickEventArgs) Handles UltraListView1.ItemDoubleClick
            If e.Item.Enabled Then
                Using Await _lock.BeginWaitAsync()
                    e.Item.CheckState = CheckState.Checked
                End Using
            End If
        End Sub

        Private Async Sub UltraListView1_KeyUp(sender As Object, e As KeyEventArgs) Handles UltraListView1.KeyUp
            If e.KeyCode = Keys.Enter OrElse e.KeyCode = Keys.Return Then
                Using Await _lock.BeginWaitAsync()
                    Dim item As UltraListViewItem = Me.UltraListView1.SelectedItems.SingleOrDefault
                    If item IsNot Nothing AndAlso item.Enabled Then
                        item.CheckState = CheckState.Checked
                    End If
                End Using
            End If
        End Sub

        Private Sub UltraListView1_MouseEnterElement(sender As Object, e As UIElementEventArgs) Handles UltraListView1.MouseEnterElement
            If ItemsEnabled AndAlso TypeOf e.Element Is UltraListViewImageUIElement Then
                Dim imageElement As UltraListViewImageUIElement = DirectCast(e.Element, UltraListViewImageUIElement)
                Dim descr As String = String.Empty
                If imageElement.Image.PropertyIdList.Contains(IMG_DESCRIPTION) Then
                    Dim propItem As Imaging.PropertyItem = imageElement.Image.GetPropertyItem(IMG_DESCRIPTION)
                    descr = System.Text.Encoding.UTF8.GetString(propItem.Value)
                End If
                _currentPopToolTip = New UltraToolTipInfo(descr, ToolTipImage.Custom, String.Empty, DefaultableBoolean.True)
                _currentPopToolTip.Appearance.Image = imageElement.Image
                _popUpTimer.Start()
            End If
        End Sub

        Private Sub UltraListView1_MouseLeaveElement(sender As Object, e As UIElementEventArgs) Handles UltraListView1.MouseLeaveElement
            If TypeOf e.Element Is UltraListViewImageUIElement Then
                _popUpTimer.Stop()
                UltraToolTipManager1.HideToolTip()
            End If
        End Sub

        Private Sub _popUpTimer_Tick(sender As Object, e As Timers.ElapsedEventArgs) Handles _popUpTimer.Elapsed
            If Not Me.Disposing AndAlso Not Me.IsDisposed AndAlso Me.Visible AndAlso Me.Enabled Then
                UltraToolTipManager1.SetUltraToolTip(UltraListView1, _currentPopToolTip)
                UltraToolTipManager1.ShowToolTip(UltraListView1)
            End If
        End Sub

        Property ItemsEnabled As Boolean
            Get
                Return _itemsEnabled
            End Get
            Set(value As Boolean)
                If _itemsEnabled <> value Then
                    _itemsEnabled = value
                    For Each item As UltraListViewItem In Me.UltraListView1.Items
                        item.Enabled = value
                    Next
                End If
            End Set
        End Property

        Private Sub btnChangeCarModelPath_Click(sender As Object, e As EventArgs) Handles btnChangeCarModelPath.Click
            If My.Application?.MainForm?.MainStateMachine IsNot Nothing Then
                My.Application.MainForm.MainStateMachine.ShowGeneralSettings(GeneralSettingsForm.TAB_3D)
            End If
        End Sub

        Private Sub _carStateMachine_Changed(sender As Object, e As CarChangeEventArgs) Handles _carStateMachine.Changed
            Me.InvokeOrDefault(
                    Sub()
                        Using Me.UltraListView1.EventManager.ProtectProperty(NameOf(UltraListView1.EventManager.AllEventsEnabled), False)
                            Select Case e.ChangeType
                                Case CarGroupChangeType.Add
                                    UpdateCheckStateFromCarStateMachine(CheckState.Checked)
                                Case CarGroupChangeType.Remove
                                    UpdateCheckStateFromCarStateMachine(CheckState.Unchecked)
                            End Select
                        End Using
                    End Sub)
        End Sub

        Private Sub UpdateCheckStateFromCarStateMachine(checkState As CheckState)
            For Each item As UltraListViewItem In Me.UltraListView1.CheckedItems.ToArray
                item.CheckState = CheckState.Unchecked
            Next

            If Not String.IsNullOrEmpty(_carStateMachine.FilePath) Then
                For Each item As UltraListViewItem In Me.FindItemsWithFileName(_carStateMachine.FilePath)
                    item.CheckState = checkState
                Next
            End If
        End Sub

    End Class

End Namespace