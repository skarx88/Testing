Imports System.ComponentModel
Imports Infragistics.Win
Imports Infragistics.Win.UltraWinTabbedMdi
Imports Infragistics.Win.UltraWinTabs

Friend Class TabDocumentCheckBoxCreationFilter
    Implements Infragistics.Win.IUIElementCreationFilter
    Implements IMessageFilter
    Implements IDisposable

    Public Event CheckBoxPropertyChanged(sender As Object, e As CheckBoxProperyChangedEventArgs)
    Public Event InitializeCheckBox(sender As Object, e As CheckBoxEventArgs)

    Private WithEvents _mdiManager As Infragistics.Win.UltraWinTabbedMdi.UltraTabbedMdiManager
    Private WithEvents _checkBoxes As New CheckBoxInfoCollection

    Private _isOverChkIndicator As Boolean
    Private _visible As Boolean = True
    Private _closingUiElements As New Dictionary(Of String, CheckBoxUIElement)
    Private _disposedValue As Boolean
    Private _lock As New System.Threading.SemaphoreSlim(1)

    Public Sub New(ByVal manager As Infragistics.Win.UltraWinTabbedMdi.UltraTabbedMdiManager)
        _mdiManager = manager

        Application.AddMessageFilter(Me)
    End Sub

    Private Sub _mdiManager_TabClosing(sender As Object, e As CancelableMdiTabEventArgs) Handles _mdiManager.TabClosing
        If (e.Tab.UIElement IsNot Nothing) Then
            Dim form As DocumentForm = CType(e.Tab.Form, DocumentForm)
            _closingUiElements.TryAdd(form.Id, CheckBoxInfo.GetUIElement(e.Tab))
        End If
    End Sub

    Private Sub _mdiManager_TabClosed(sender As Object, e As MdiTabEventArgs) Handles _mdiManager.TabClosed
        If _closingUiElements IsNot Nothing Then
            For Each kv As KeyValuePair(Of String, CheckBoxUIElement) In _closingUiElements.ToArray
                If _checkBoxes.Contains(kv.Key) Then
                    Dim chkBoxInfo As CheckBoxInfo = _checkBoxes(kv.Key)
                    chkBoxInfo.Remove(kv.Value)
                ElseIf kv.Value?.Parent IsNot Nothing Then
                    CheckBoxInfo.RemoveFromParent(kv.Value)
                End If
                _checkBoxes.Remove(kv.Key)
                _closingUiElements.Remove(kv.Key)
            Next
        End If
    End Sub

    Private Sub SwapChildElementsVertically(ByVal pe As Infragistics.Win.UIElement, ByVal startIndex As Integer)
        Dim e1 As Infragistics.Win.UIElement = pe.ChildElements(startIndex)
        Dim e2 As Infragistics.Win.UIElement = pe.ChildElements(startIndex + 1)

        If TypeOf e2 Is Infragistics.Win.ImageAndTextUIElement.ImageAndTextDependentTextUIElement Then
            Dim r1 As Rectangle, r2 As Rectangle

            Select Case _mdiManager.TabGroupSettings.TabOrientation
                Case Infragistics.Win.UltraWinTabs.TabOrientation.[Default], Infragistics.Win.UltraWinTabs.TabOrientation.TopLeft, Infragistics.Win.UltraWinTabs.TabOrientation.TopRight, Infragistics.Win.UltraWinTabs.TabOrientation.BottomLeft, Infragistics.Win.UltraWinTabs.TabOrientation.BottomRight
                    r1 = New Rectangle(e1.Rect.X, e2.Rect.Y, e2.Rect.Width, e2.Rect.Height)
                    r2 = New Rectangle(e1.Rect.X + e2.Rect.Width, e1.Rect.Y, e1.Rect.Width, e1.Rect.Height)
                    e1.Rect = r2
                    e2.Rect = r1
                    pe.ChildElements.Reverse(startIndex, 2)
                    Exit Select
                Case Infragistics.Win.UltraWinTabs.TabOrientation.LeftTop, Infragistics.Win.UltraWinTabs.TabOrientation.LeftBottom
                    r2 = New Rectangle(e1.Rect.X, e2.Rect.Y, e1.Rect.Width, e1.Rect.Height)
                    r1 = New Rectangle(e2.Rect.X, e2.Rect.Y + e1.Rect.Height, e2.Rect.Width, e2.Rect.Height)
                    e1.Rect = r2
                    e2.Rect = r1
                    pe.ChildElements.Reverse(startIndex, 2)
                    Exit Select
                Case Infragistics.Win.UltraWinTabs.TabOrientation.RightTop, Infragistics.Win.UltraWinTabs.TabOrientation.RightBottom
                    r1 = New Rectangle(e2.Rect.X, e1.Rect.Y, e2.Rect.Width, e2.Rect.Height)
                    r2 = New Rectangle(e1.Rect.X, e1.Rect.Y + e2.Rect.Height, e1.Rect.Width, e1.Rect.Height)
                    e1.Rect = r2
                    e2.Rect = r1
                    pe.ChildElements.Reverse(startIndex, 2)
                    Exit Select
            End Select
        End If
    End Sub

    Public Sub AfterCreateChildElements(ByVal parent As Infragistics.Win.UIElement) Implements Infragistics.Win.IUIElementCreationFilter.AfterCreateChildElements
        If TypeOf parent Is Infragistics.Win.ImageAndTextUIElement.ImageAndTextDependentTextUIElement AndAlso TypeOf parent.Parent Is TabItemUIElement Then
            Dim tab As MdiTab = CType(parent.Parent.GetContext(GetType(MdiTab)), MdiTab)
            If tab.Form IsNot Nothing AndAlso TypeOf tab.Form Is DocumentForm Then
                If Visible Then
                    Dim imageElement As Infragistics.Win.ImageAndTextUIElement.ImageAndTextDependentTextUIElement = CType(parent, Infragistics.Win.ImageAndTextUIElement.ImageAndTextDependentTextUIElement)
                    Dim parentElement As Infragistics.Win.UIElement = imageElement.Parent.ChildElements.OfType(Of Infragistics.Win.ImageAndTextUIElement.ImageAndTextDependentTextUIElement).Single
                    Dim created As Boolean = False
                    Dim checkInfo As CheckBoxInfo = GetOrCreateInfo(tab, created)
                    Dim element As CheckBoxUIElement = checkInfo.UpdateUIElement(tab, False)
                    checkInfo.Visible = Me.Visible
                    'swap the locations of the button element and text element.
                    If element IsNot Nothing AndAlso parentElement.ChildElements.Count >= 2 Then
                        SwapChildElementsVertically(parentElement, 0)
                    End If
                End If
            End If
        End If
    End Sub

    Protected Overridable Sub OnInitializeCheckBox(e As CheckBoxEventArgs)
        If EventsEnabled Then
            RaiseEvent InitializeCheckBox(Me, e)
        End If
    End Sub

    Private Sub OnCheckBoxPropertyChanged(e As CheckBoxProperyChangedEventArgs)
        If EventsEnabled Then
            RaiseEvent CheckBoxPropertyChanged(Me, e)
        End If
    End Sub

    Public Function BeforeCreateChildElements1(ByVal parent As Infragistics.Win.UIElement) As Boolean Implements Infragistics.Win.IUIElementCreationFilter.BeforeCreateChildElements
        Return False
    End Function

    <DebuggerNonUserCode>
    Public Function PreFilterMessage(ByRef m As Message) As Boolean Implements IMessageFilter.PreFilterMessage
        Static WM_LBUTTONDOWN As Integer = &H201
        Static WM_LBUTTONUP As Integer = &H202
        Static mouseWasDownOnChk As Boolean = False
        If m.Msg = WM_LBUTTONDOWN Then
            If _isOverChkIndicator Then
                mouseWasDownOnChk = True
                Return True
            End If
        ElseIf m.Msg = WM_LBUTTONUP Then
            If mouseWasDownOnChk AndAlso _isOverChkIndicator Then
                Dim position As Point = Form.MousePosition
                Dim tab As MdiTab = _mdiManager.TabFromPoint(position)
                If tab IsNot Nothing Then
                    Dim chkInfo As CheckBoxInfo = GetOrCreateInfo(tab, Nothing)
                    chkInfo.ToggleState()
                    chkInfo.UpdateUIElement(tab)
                    chkInfo.Visible = Me.Visible
                End If
            End If
            mouseWasDownOnChk = False
        End If
        Return False
    End Function

    Property EventsEnabled As Boolean = True

    Public Async Sub SetCheckBoxOfTab(tab As MdiTab, checked As Boolean)
        Await _lock.WaitAsync
        Try
            Dim chk As CheckBoxInfo = Await GetOrCreateCheckBox(tab, True)
            If chk IsNot Nothing Then
                If checked Then
                    chk.State = CheckState.Checked
                Else
                    chk.State = CheckState.Unchecked
                End If
                Invalidate()
            End If
        Finally
            _lock.Release()
        End Try
    End Sub

    Private Async Function GetOrCreateCheckBox(tab As MdiTab, waitIfnotPossible As Boolean) As Task(Of CheckBoxInfo)
        If tab.UIElement Is Nothing AndAlso waitIfnotPossible Then
            Await Task.Factory.StartNew(Sub() System.Threading.SpinWait.SpinUntil(Function() tab.UIElement IsNot Nothing OrElse tab.Disposed, 1000))
        End If

        If Not tab.Disposed Then
            Dim created As Boolean
            Return GetOrCreateInfo(tab, created)
        End If
        Return Nothing
    End Function

    Private Function GetOrCreateInfo(tab As MdiTab, ByRef created As Boolean) As CheckBoxInfo
        Dim form As DocumentForm = CType(tab.Form, DocumentForm)
        If _checkBoxes.Contains(form.Id.ToString) Then
            Return _checkBoxes(form.Id.ToString)
        Else
            Dim chk As New CheckBoxInfo(form.Id.ToString)
            _checkBoxes.Add(chk)
            OnInitializeCheckBox(New CheckBoxEventArgs(chk))
            created = True
            Return chk
        End If
    End Function

    Public Async Function SetEnabledCheckBoxOfTab(tab As MdiTab, enabled As Boolean, Optional waitIfnotPossible As Boolean = False) As Task
        Await _lock.WaitAsync()
        Try
            Dim chkInfo As CheckBoxInfo = Await GetOrCreateCheckBox(tab, waitIfnotPossible)
            If chkInfo IsNot Nothing Then
                chkInfo.Enabled = enabled
                chkInfo.Visible = Me.Visible
                chkInfo.UpdateUIElement(tab)
            End If
        Finally
            _lock.Release()
        End Try
    End Function

    Private Sub _mdiManager_MouseEnterElement(sender As Object, e As UIElementEventArgs) Handles _mdiManager.MouseEnterElement
        If TypeOf e.Element Is CheckIndicatorUIElement Then
            _isOverChkIndicator = True
        End If
    End Sub

    Private Sub _mdiManager_MouseLeaveElement(sender As Object, e As UIElementEventArgs) Handles _mdiManager.MouseLeaveElement
        If TypeOf e.Element Is CheckIndicatorUIElement Then
            _isOverChkIndicator = False
        End If
    End Sub

    Property Visible As Boolean
        Get
            Return _visible
        End Get
        Set(value As Boolean)
            If _visible <> value Then
                _visible = value
                If _checkBoxes.Count > 0 Then
                    For Each chk As CheckBoxInfo In _checkBoxes
                        chk.Visible = value
                    Next
                    Invalidate()
                End If
            End If
        End Set
    End Property

    Private Sub Invalidate()
        For Each grp As MdiTabGroup In _mdiManager.TabGroups
            For Each t As MdiTab In grp.Tabs
                If t.UIElement IsNot Nothing Then
                    t.UIElement.DirtyChildElements(True)
                    t.UIElement.Invalidate()
                End If
            Next
        Next
    End Sub

    Private Shared Sub DirtyRecursively(element As UIElement)
        element.DirtyChildElements(True)
        element.VerifyChildElements()
        For Each child As UIElement In element.ChildElements
            DirtyRecursively(child)
        Next
    End Sub

    Private Sub _checkBoxes_CheckedStateChanged(sender As Object, e As CheckBoxProperyChangedEventArgs) Handles _checkBoxes.PropertyChanged
        OnCheckBoxPropertyChanged(e)
    End Sub

    Private Class CheckBoxInfoCollection
        Inherits System.Collections.ObjectModel.HookableKeyedCollection(Of String, CheckBoxInfo)

        Public Event PropertyChanged(sender As Object, e As CheckBoxProperyChangedEventArgs)

        Protected Overrides Sub AfterInsertItem(item As CheckBoxInfo)
            AddHandler item.PropertyChanged, AddressOf _item_PropretyChanged
            MyBase.AfterInsertItem(item)
        End Sub

        Private Sub _item_PropretyChanged(sender As Object, e As System.ComponentModel.PropertyChangedEventArgs)
            OnPropertyChanged(New CheckBoxProperyChangedEventArgs(DirectCast(sender, CheckBoxInfo), e.PropertyName))
        End Sub

        Protected Overridable Sub OnPropertyChanged(e As CheckBoxProperyChangedEventArgs)
            RaiseEvent PropertyChanged(Me, e)
        End Sub

        Protected Overrides Sub AfterRemoveItem(item As CheckBoxInfo)
            MyBase.AfterRemoveItem(item)
            RemoveHandler item.PropertyChanged, AddressOf _item_PropretyChanged
        End Sub

        Protected Overrides Function GetKeyForItem(item As CheckBoxInfo) As String
            Return item.DocumentId
        End Function

    End Class

    Private Class ToolTip
        Implements IToolTipItem

        Public Sub New(text As String)
            Me.Text = text
        End Sub

        Property Text As String

        Public Function GetToolTipInfo(mousePosition As Point, element As UIElement, previousToolTipElement As UIElement, toolTipInfoDefault As ToolTipInfo) As ToolTipInfo Implements IToolTipItem.GetToolTipInfo
            Dim tt As New ToolTipInfo(mousePosition)
            Dim chk As CheckIndicatorUIElement = CType(element.GetDescendant(GetType(CheckIndicatorUIElement)), CheckIndicatorUIElement)
            If chk.PointInElement(element.Control.PointToClient(mousePosition)) Then
                tt.ToolTipText = Text
                Return tt
            Else
                Dim tab As MdiTab = CType(element.GetContext(GetType(MdiTab)), MdiTab)
                tt.ToolTipText = tab.TextResolved
                Return tt
            End If
        End Function
    End Class

    Public Class CheckBoxInfo
        Implements INotifyPropertyChangedEx

        Private _Enabled As Boolean
        Private _Visible As Boolean
        Private _ReadOnly As Boolean
        Private _State As CheckState

        Public Event PropertyChanged As System.ComponentModel.PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

        Public Sub New(documentId As String)
            Me.Enabled = True
            Me.State = CheckState.Checked
            Me.Visible = True
            Me.DocumentId = documentId
        End Sub

        ReadOnly Property DocumentId As String

        Public Function UpdateUIElement(tab As MdiTab, Optional setDirty As Boolean = True) As CheckBoxUIElement
            Dim element As CheckBoxUIElement = GetUIElement(tab)
            Dim parent As UIElement = Nothing
            If element IsNot Nothing Then
                parent = element.Parent
            End If

            If Me.Visible AndAlso element Is Nothing Then
                element = AddNewCheckBoxElement(tab, setDirty)
            ElseIf Not Visible AndAlso element IsNot Nothing Then
                If parent IsNot Nothing Then
                    parent.ChildElements.Remove(element)
                    If setDirty Then
                        parent.DirtyChildElements(True)
                        parent.Invalidate()
                    End If
                End If
                element.Dispose()
                element = Nothing
            End If

            If element IsNot Nothing Then
                With element
                    .CheckState = State
                    .ReadOnly = [ReadOnly]
                    .Enabled = Enabled
                    If setDirty Then
                        .Invalidate()
                        If parent IsNot Nothing Then
                            parent.DirtyChildElements(True)
                            parent.Invalidate()
                        End If
                    End If
                End With
            End If
            Return element
        End Function

        Public Shared Function AddNewCheckBoxElement(tab As MdiTab, Optional setDirty As Boolean = False) As CheckBoxUIElement
            If tab.UIElement IsNot Nothing Then
                Dim keyElement As ImageAndTextUIElement.ImageAndTextDependentTextUIElement = GetKeyUIElement(CType(tab.UIElement, TabItemUIElement))
                Dim boxSize As Integer = CInt(keyElement.DrawingRectInsideBorders.Height / 1.5)
                Dim checkBoxUIElement As New CheckBoxUIElement(keyElement)
                checkBoxUIElement.CheckAlign = ContentAlignment.MiddleRight
                checkBoxUIElement.Rect = New Rectangle(New Point(keyElement.Rect.Location.X + keyElement.Rect.Width - boxSize + 2, keyElement.Rect.Location.Y), New Size(boxSize, boxSize))
                checkBoxUIElement.CheckState = CheckState.Checked
                checkBoxUIElement.ReadOnly = True
                checkBoxUIElement.ToolTipItem = New ToolTip(MainFormStrings.D3DVisibilityToolTipTabText)
                keyElement.Parent.ChildElements.Add(checkBoxUIElement)
                If setDirty Then
                    keyElement.Parent.DirtyChildElements(True)
                    keyElement.Parent.Invalidate()
                End If
                Return checkBoxUIElement
            End If
            Return Nothing
        End Function

        Private Shared Function GetKeyUIElement(element As CheckBoxUIElement) As UIElement
            Dim tabItemUI As TabItemUIElement = CType(element.GetAncestor(GetType(TabItemUIElement)), TabItemUIElement)
            If tabItemUI IsNot Nothing Then
                Return GetKeyUIElement(tabItemUI)
            End If
            Return Nothing
        End Function

        Private Shared Function GetKeyUIElement(tabItemElement As TabItemUIElement) As ImageAndTextUIElement.ImageAndTextDependentTextUIElement
            Return tabItemElement.ChildElements.OfType(Of Infragistics.Win.ImageAndTextUIElement.ImageAndTextDependentTextUIElement).Single
        End Function

        Public Sub Remove(tab As MdiTab)
            Dim element As CheckBoxUIElement = GetUIElement(tab)
            Remove(element)
        End Sub

        Public Sub Remove(element As CheckBoxUIElement)
            If element IsNot Nothing Then
                RemoveFromParent(element)
            End If
        End Sub

        Public Shared Sub RemoveFromParent(element As CheckBoxUIElement)
            Dim parent As UIElement = element.Parent
            parent.ChildElements.Remove(element)
            parent.DirtyChildElements(True)
        End Sub

        Public Sub ToggleState()
            If State = CheckState.Checked Then
                State = CheckState.Unchecked
            Else
                State = CheckState.Checked
            End If
        End Sub

        Public Shared Function GetUIElement(tab As MdiTab) As CheckBoxUIElement
            If tab.UIElement IsNot Nothing Then
                Return CType(tab.UIElement.GetDescendant(GetType(CheckBoxUIElement)), CheckBoxUIElement)
            End If
            Return Nothing
        End Function

        Protected Overridable Sub OnPropertyChanged(sender As Object, e As System.ComponentModel.PropertyChangedEventArgs) Implements INotifyPropertyChangedEx.OnPropertyChanged
            RaiseEvent PropertyChanged(sender, e)
        End Sub

        Property [ReadOnly] As Boolean
            Get
                Return _ReadOnly
            End Get
            Set
                If _ReadOnly <> Value Then
                    _ReadOnly = Value
                    OnPropertyChangedEx
                End If
            End Set
        End Property

        Property Visible As Boolean
            Get
                Return _Visible
            End Get
            Set
                If _Visible <> Value Then
                    _Visible = Value
                    OnPropertyChangedEx
                End If
            End Set
        End Property

        Property Enabled As Boolean
            Get
                Return _Enabled
            End Get
            Set
                If _Enabled <> Value Then
                    _Enabled = Value
                    Me.OnPropertyChangedEx
                End If
            End Set
        End Property

        Property State As CheckState
            Get
                Return _State
            End Get
            Set
                If Value <> _State Then
                    _State = Value
                    OnPropertyChangedEx
                End If
            End Set
        End Property
    End Class

    Public Class CheckBoxEventArgs
        Inherits EventArgs

        Private _checkBox As CheckBoxInfo

        Public Sub New(checkBox As CheckBoxInfo)
            MyBase.New()
            _checkBox = checkBox
        End Sub

        ReadOnly Property CheckBox As CheckBoxInfo
            Get
                Return _checkBox
            End Get
        End Property

    End Class

    Public Class CheckBoxProperyChangedEventArgs
        Inherits CheckBoxEventArgs

        Public Sub New(checkBox As CheckBoxInfo, propertyName As String)
            MyBase.New(checkBox)
            Me.PropertyName = propertyName
        End Sub

        ReadOnly Property PropertyName As String

    End Class

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not _disposedValue Then
            If disposing Then
                If _checkBoxes IsNot Nothing Then
                    _checkBoxes.Clear()
                End If
                If _closingUiElements IsNot Nothing Then
                    _closingUiElements.Clear()
                End If
                Application.RemoveMessageFilter(Me)
            End If

            _closingUiElements = Nothing
            _checkBoxes = Nothing
            _mdiManager = Nothing
            _disposedValue = True
        End If
    End Sub


    Public Sub Dispose() Implements IDisposable.Dispose
        ' Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in der Methode "Dispose(disposing As Boolean)" ein.
        Dispose(disposing:=True)
        GC.SuppressFinalize(Me)
    End Sub
End Class