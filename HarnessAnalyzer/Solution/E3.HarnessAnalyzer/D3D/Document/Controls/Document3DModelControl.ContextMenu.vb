Imports System.ComponentModel
Imports devDept.Eyeshot.Entities
Imports Zuken.E3.HarnessAnalyzer.Settings
Imports Zuken.E3.Lib.Eyeshot.Model

Namespace D3D.Document.Controls

    Partial Public Class DocumentDesignControl

        Private _currentEntityFromContext As Entity
        Private _currentHiddenEntity As HiddenEntity
        Private _generalSettings As GeneralSettingsBase

        Private _itemShowAll As New ToolStripMenuItem()
        Private _itemShow As New ToolStripMenuItem()
        Private _itemHide As New ToolStripMenuItem()
        Private _itemShowTooltip As New ToolStripMenuItem()

        Public Event HideIndicator(id As String)
        Public Event ShowIndicator(id As String)
        Public Event ShowAllIndicators()


        Public Sub InitContextMenu(generalSettings As GeneralSettingsBase)
            Me.ContextMenuStrip = ContextMenuStrip1
            _generalSettings = generalSettings

            _itemShowAll.Text = Document3DStrings.MenuItem_ShowAll
            _itemShowAll.Image = My.Resources.ShowAll

            _itemShow.Text = Document3DStrings.MenuItem_Show
            _itemShow.Visible = False
            _itemShow.Image = My.Resources.Show

            _itemHide.Text = Document3DStrings.MenuItem_Hide
            _itemHide.Visible = False
            _itemHide.Image = My.Resources.hide

            _itemShowTooltip.Text = Document3DStrings.MenuItem_ShowTooltip
            _itemShowTooltip.Image = My.Resources.About_Small

            ContextMenuStrip1.Items.Add(_itemShowAll)
            ContextMenuStrip1.Items.Add(_itemShow)
            ContextMenuStrip1.Items.Add(_itemHide)
            ContextMenuStrip1.Items.Add(_itemShowTooltip)


            AddHandler _itemShowAll.Click, AddressOf ResetAllHiddenEntities
            AddHandler _itemShow.Click, AddressOf ResetHiddenEntity
            AddHandler _itemHide.Click, AddressOf AddEntityToHiddens
            AddHandler _itemShowTooltip.Click, AddressOf ShowTooltipOnEntity
        End Sub

        Private Sub ContextMenuStrip1_Opening(sender As Object, e As CancelEventArgs) Handles ContextMenuStrip1.Opening

            Dim position As System.Drawing.Point = Design.MouseState.Location
            _currentEntityFromContext = Design.GetEntityUnderMouseCursor(position, True)

            _itemShowAll.Enabled = CBool(Design.HiddenEntities.Count > 0)
            _itemHide.Visible = False
            _itemShow.Visible = False
            _itemShowTooltip.Visible = False

            Dim myEntities As List(Of Entity) = Design.GetSortedListFromCrossingEntities(Design, New Rectangle(position.X - 1, position.Y - 1, 3, 3), False)

            If _currentEntityFromContext IsNot Nothing AndAlso _currentEntityFromContext.Selected Then
                _itemHide.Visible = True
                _itemShowTooltip.Visible = CBool(_generalSettings IsNot Nothing AndAlso Not _generalSettings.AutomaticTooltips)

            ElseIf myEntities.Count > 0 Then
                _currentHiddenEntity = GetEntitiesFromHidden(myEntities).FirstOrDefault()
                _itemShow.Visible = CBool(_currentHiddenEntity IsNot Nothing)

            ElseIf _currentHiddenEntity Is Nothing AndAlso _currentEntityFromContext IsNot Nothing Then
                _itemHide.Visible = True
            End If

            e.Cancel = ContextMenuStrip1.Items.Count = 0
        End Sub

        Private Function GetEntitiesFromHidden(entities As List(Of Entity)) As List(Of HiddenEntity)
            Dim result As New List(Of HiddenEntity)
            Dim myHiddens As List(Of Entity) = Design.HiddenEntities.Select(Function(h) h.Entity).ToList()
            For Each en As Entity In entities
                If myHiddens.Contains(en) Then
                    Dim myHidden As HiddenEntity = Design.HiddenEntities.Where(Function(h) h.Entity Is en).FirstOrDefault()
                    If myHidden IsNot Nothing Then
                        result.Add(myHidden)
                    End If
                End If
            Next
            Return result
        End Function

        Friend Sub ResetAllHiddenEntities(sender As Object, e As EventArgs)
            Design.ResetHiddenEntities()
            RaiseEvent ShowAllIndicators()
            Design.IsHiddenSelection = False
        End Sub

        Friend Sub ResetHiddenEntity(sender As Object, e As EventArgs)
            If _currentHiddenEntity IsNot Nothing Then
                _currentHiddenEntity.Reset()
                Design.HiddenEntities.Remove(_currentHiddenEntity)
                Design.UpdateVisibleSelection(False)
            End If
            _itemShow.Visible = False
            Dim id As String = CType(_currentHiddenEntity.Entity, BaseModelEntity).Id
            RaiseEvent ShowIndicator(id)
            Design.Invalidate()
        End Sub

        Private Sub AddEntityToHiddens(sender As Object, e As EventArgs)
            If _currentEntityFromContext IsNot Nothing Then
                _currentEntityFromContext.Selected = False
                Design.HiddenEntities.Add(New HiddenEntity(_currentEntityFromContext))
                Design.UpdateVisibleSelection(True)
            End If
            _itemHide.Visible = False
            Dim id As String = CType(_currentEntityFromContext, BaseModelEntity).Id
            RaiseEvent HideIndicator(id)
            Design.Invalidate()
        End Sub

        Private Sub ShowTooltipOnEntity(sender As Object, e As EventArgs)
            CType(Me.Parent, Document3DControl).ShowEntityToolTip(_currentEntityFromContext)
        End Sub

    End Class

End Namespace
