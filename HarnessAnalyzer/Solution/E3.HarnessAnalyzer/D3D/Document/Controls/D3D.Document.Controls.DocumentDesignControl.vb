Imports devDept.Eyeshot
Imports devDept.Eyeshot.Entities
Imports devDept.Eyeshot.Labels
Imports devDept.Eyeshot.Workspace
Imports Zuken.E3.HarnessAnalyzer.D3D.Shared
Imports Zuken.E3.Lib.Eyeshot.Model

Namespace D3D.Document.Controls

    <Reflection.Obfuscation(Feature:="renaming", ApplyToMembers:=False, Exclude:=True)> ' needed to avoid resource-file-names obfuscation errors, see issue #5
    Public Class DocumentDesignControl

        Private _drawBorderColor As Color?
        Private _actionModeChanging As actionType

        Public Event MouseEnterEntity(sender As Object, e As MouseEntityEventArgs)
        Public Event MouseLeaveEntity(sender As Object, e As MouseEntityEventArgs)

        Public Event SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        Public Event EmptySpaceClicked(sender As Object, e As EventArgs)
        Public Event ActionModelChanged(sender As Object, e As ActionModeChangedEventArgs)
        Public Event NeedsEEObjectData(sender As Object, e As EEObjectDataEventArgs)
        Public Event ModelLostFocus(sender As Object, e As EventArgs)
        Public Event ClearSelection(sender As Object, e As EventArgs)
        Public Event SelectRedlining(sender As Object, e As SelectRedliningEventArgs)

        Public Sub New()
            InitializeComponent()
            Design.InitDefaults(initObjManipulatorManager:=False)
            AddHandler Design.MouseClickLabel, AddressOf OnLabelClicked
        End Sub

        Property DrawBorderColor As Nullable(Of System.Drawing.Color)
            Get
                Return _drawBorderColor
            End Get
            Set
                _drawBorderColor = Value
                If Value.HasValue Then
                    Me.Padding = New Padding(5)
                    Me.BackColor = Value.Value
                Else
                    Me.BackColor = Nothing
                    Me.Padding = New Padding(0)
                End If
            End Set
        End Property

        Private ReadOnly Property IsDebug As Boolean
            Get
#If CONFIG = "Debug" Or DEBUG Then
                Return True
#End If
                Return False
            End Get
        End Property

        Public ReadOnly Property SelectedEntities As IEESelectedEntitiesCollection
            Get
                Return Design.SelectedEntities
            End Get
        End Property

        Private Sub Design_MouseEnterEntity(sender As Object, e As MouseEntityEventArgs) Handles Design.MouseEnterEntity
            RaiseEvent MouseEnterEntity(Me, e)
        End Sub

        Private Sub Design_MouseLeaveEntity(sender As Object, e As MouseEntityEventArgs) Handles Design.MouseLeaveEntity
            RaiseEvent MouseLeaveEntity(Me, e)
        End Sub

        Private Sub Design_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles Design.SelectionChanged
            OnSelectionChanged(e)
        End Sub

        Protected Overridable Sub OnSelectionChanged(e As SelectionChangedEventArgs)
            RaiseEvent SelectionChanged(Me, e)
        End Sub

        Public Sub AttachDocument(document As Project.Documents.HcvDocument)
            Me.Design.Document = document  ' HINT: no auto-entities conversion here (settings set to nothing), because we want to set the entities manually, but we connect the model
        End Sub

        Private Sub Design_MouseClick(sender As Object, e As MouseEventArgs) Handles Design.MouseClick
            If e.Button = MouseButtons.Left Then
                Dim lblIndex As Integer = Me.Design.GetLabelUnderMouseCursor(e.Location)
                Dim entity As Entity = Me.Design.GetEntityUnderMouseCursor(e.Location, True)

                If entity Is Nothing And lblIndex = -1 Then
                    OnEmptySpaceClicked(New EventArgs)
                End If
            End If
        End Sub

        Protected Overridable Sub OnEmptySpaceClicked(e As EventArgs)
            RaiseEvent EmptySpaceClicked(Me, e)
        End Sub

        Protected Overridable Sub OnActionModelChanged(e As ActionModeChangedEventArgs)
            RaiseEvent ActionModelChanged(Me, e)
        End Sub

        Private Sub Design_AfterActionModeChanged(sender As Object, e As ActionModeEventArgs) Handles Design.AfterActionModeChanged
            Dim args As New ActionModeChangedEventArgs(e.Action, e.Button, _actionModeChanging)
            OnActionModelChanged(args)
        End Sub

        Private Sub Design_KeyDown(sender As Object, e As KeyEventArgs) Handles Design.KeyDown
            If e.KeyCode = Keys.Escape Then
                OnClearSelection(New EventArgs)
                Design.SelectedEntities.Clear()
                Design.Invalidate()
            End If
            If e.KeyCode = Keys.S And e.Modifiers = Keys.Control Then
                Dim myHiddens As New List(Of Entity)
                For Each item As Entity In Design.SelectedEntities
                    myHiddens.Add(item)
                Next

                For Each item As Entity In myHiddens
                    _currentEntityFromContext = item
                    AddEntityToHiddens(Me, New EventArgs)
                Next
            End If
        End Sub

        Private Sub Design_NeedsEEObjectData(sender As Object, e As EEObjectDataEventArgs) Handles Design.EEObjectDataRequested
            OnNeedsEEObjectData(e)
        End Sub

        Protected Overridable Sub OnNeedsEEObjectData(e As EEObjectDataEventArgs)
            RaiseEvent NeedsEEObjectData(Me, e)
        End Sub

        Protected Friend Overridable Sub OnClearSelection(e As EventArgs)
            RaiseEvent ClearSelection(Me, e)
        End Sub

        Private Sub Design_LostFocus(sender As Object, e As EventArgs) Handles Design.LostFocus
            If Not Design.Focused Then
                RaiseEvent ModelLostFocus(Me, e)
            End If
        End Sub

        Private Sub Design_BeforeActionChanging(sender As Object, e As ActionChangingEventArgs) Handles Design.BeforeActionChanging
            _actionModeChanging = Design.ActionMode
        End Sub

        Private Sub OnLabelClicked(sender As Object, e As MouseLabelEventArgs)
            Dim label As devDept.Eyeshot.Labels.Label = Design.ActiveViewport.Labels(e.Index)
            If TypeOf label Is LeaderAndImageEx Then
                label.Selected = True

                RaiseEvent SelectRedlining(Me, New SelectRedliningEventArgs(CType(label, LeaderAndImageEx)))
            End If
        End Sub

    End Class

End Namespace