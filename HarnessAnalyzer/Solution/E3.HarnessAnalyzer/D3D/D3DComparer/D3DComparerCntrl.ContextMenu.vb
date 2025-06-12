Imports System.ComponentModel
Imports devDept.Eyeshot.Entities
Imports Zuken.E3.HarnessAnalyzer.Settings
Imports Zuken.E3.Lib.Eyeshot.Model

Namespace D3D

    Partial Public Class D3DComparerCntrl
        Private _currentEntityFromContext As Entity
        Private _currentHiddenEntity As HiddenEntity
        Private _generalSettings As GeneralSettingsBase

        Private _itemShowAll As New ToolStripMenuItem()
        Private _itemShow As New ToolStripMenuItem()
        Private _itemHide As New ToolStripMenuItem()
        Private _itemShowTooltip As New ToolStripMenuItem()
        Private _hiddenAlpha As Integer = 10
        Private _isContext As Boolean

        Public Sub InitContextMenu(generalSettings As GeneralSettingsBase)
            Me.ContextMenuStrip = ContextMenuStrip1
            _generalSettings = generalSettings

            _itemShowAll.Text = Document3DStrings.MenuItem_ShowAll
            _itemShowAll.Image = My.Resources.ShowAll

            _itemShow.Text = Document3DStrings.MenuItem_Show
            _itemShow.Image = My.Resources.Show

            _itemHide.Text = Document3DStrings.MenuItem_Hide
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
            _isContext = True
            _itemShowTooltip.Visible = False
            _itemHide.Visible = False
            _itemShow.Visible = False
            _itemShowAll.Enabled = CBool(Design3D.HiddenEntities.Count > 0)

            Dim position As Drawing.Point = Design3D.MouseState.Location
            _currentEntityFromContext = Design3D.GetEntityUnderMouseCursor(position, True)

            Dim myEntities As List(Of Entity) = Design3D.GetSortedListFromCrossingEntities(Design3D, New Rectangle(position.X - 1, position.Y - 1, 3, 3), False)

            If _currentEntityFromContext IsNot Nothing Then
                _itemShowTooltip.Visible = CBool(_generalSettings IsNot Nothing AndAlso Not _generalSettings.AutomaticTooltips)
                If _currentEntityFromContext.Selected Then
                    If Not (_routedRefElements.Contains(CType(_currentEntityFromContext, IBaseModelEntityEx)) OrElse (_routedCompElements.Contains(CType(_currentEntityFromContext, IBaseModelEntityEx)))) Then
                        _itemHide.Visible = True
                    End If
                End If
            ElseIf myEntities.Count > 0 Then
                _currentHiddenEntity = GetEntitiesFromHidden(myEntities).FirstOrDefault()
                _itemShow.Visible = CBool(_currentHiddenEntity IsNot Nothing)
            ElseIf _currentHiddenEntity Is Nothing AndAlso _currentEntityFromContext IsNot Nothing Then
                If Not (_routedRefElements.Contains(CType(_currentEntityFromContext, IBaseModelEntityEx)) OrElse (_routedCompElements.Contains(CType(_currentEntityFromContext, IBaseModelEntityEx)))) Then
                    _itemHide.Visible = True
                End If
            End If
            e.Cancel = CBool(ContextMenuStrip1.Items.Count = 0)


        End Sub

        Private Function GetEntitiesFromHidden(entities As List(Of Entity)) As List(Of HiddenEntity)
            Dim result As New List(Of HiddenEntity)
            Dim myHiddens As List(Of Entity) = Design3D.HiddenEntities.Select(Function(h) h.Entity).ToList()
            For Each en As Entity In entities
                If myHiddens.Contains(en) Then
                    Dim myHidden As HiddenEntity = Design3D.HiddenEntities.Where(Function(h) h.Entity Is en).FirstOrDefault()
                    If myHidden IsNot Nothing Then
                        result.Add(myHidden)
                    End If
                End If
            Next
            Return result
        End Function

        Private Sub ResetAllHiddenEntities(sender As Object, e As EventArgs)
            For Each item As HiddenEntity In Design3D.HiddenEntities
                Dim en As IBaseModelEntityEx = CType(item.Entity, IBaseModelEntityEx)
                item.Reset()

                If EntityIsReferenceEntity(item.Entity) Then

                    SetTransparency(en, GetRefTrans)
                    If Trackbar.Value > 0 Then
                        en.Selectable = False
                    End If
                    _refChanges.Add(en)
                Else
                    SetTransparency(en, GetCompTrans)
                    If Trackbar.Value <= 0 Then
                        en.Selectable = False
                    End If
                    _compChanges.Add(en)
                End If
            Next

            Design3D.HiddenEntities.Clear()
            Design3D.IsHiddenSelection = False
            Design3D.Invalidate()
        End Sub

        Private Sub ResetHiddenEntity(sender As Object, e As EventArgs)
            If _currentHiddenEntity IsNot Nothing Then
                _currentHiddenEntity.Reset()

                Dim myChange As ChangedObjectEx = GetChangedObject(_currentHiddenEntity.Entity)

                If myChange IsNot Nothing AndAlso myChange.ModifiedEntity IsNot Nothing Then

                    Dim myModifiedEntity As IBaseModelEntityEx = myChange.ModifiedEntity

                    Dim myHidden As HiddenEntity = Design3D.HiddenEntities.Where(Function(en) en.Entity Is CType(myModifiedEntity, Entity)).FirstOrDefault
                    If myHidden IsNot Nothing Then
                        myHidden.Reset()
                    End If

                    If myChange.MapperSourceId = _refDoc.Kbl.id Then
                        If Trackbar.Value <= 0 Then
                            _currentHiddenEntity.Entity.Selectable = True
                            myHidden.Entity.Selectable = False
                        Else
                            _currentHiddenEntity.Entity.Selectable = False
                            myHidden.Entity.Selectable = True
                        End If
                    End If

                    Design3D.HiddenEntities.Remove(myHidden)
                Else
                    If myChange IsNot Nothing AndAlso myChange.MapperSourceId = _refDoc.Kbl.Id Then
                        If Trackbar.Value <= 0 Then
                            _currentHiddenEntity.Entity.Selectable = True
                        Else
                            _currentHiddenEntity.Entity.Selectable = False
                        End If
                    End If
                End If

                Design3D.HiddenEntities.Remove(_currentHiddenEntity)
                Design3D.UpdateVisibleSelection(False)
            End If
            _itemShow.Visible = False
            Design3D.Invalidate()
        End Sub

        Private Sub AddEntityToHiddens(sender As Object, e As EventArgs)
            Dim CallSelectionChanged As Boolean = False

            If _currentEntityFromContext IsNot Nothing Then
                CallSelectionChanged = _currentEntityFromContext.Selected
                If _currentEntityFromContext.Selected Then
                    _currentEntityFromContext.Selected = False
                End If

                If _refSelection.Contains(CType(_currentEntityFromContext, IBaseModelEntityEx)) Then
                    _refSelection.Remove(CType(_currentEntityFromContext, IBaseModelEntityEx))
                End If

                If _compSelection.Contains(CType(_currentEntityFromContext, IBaseModelEntityEx)) Then
                    _compSelection.Remove(CType(_currentEntityFromContext, IBaseModelEntityEx))
                End If

                Design3D.HiddenEntities.Add(New HiddenEntity(_currentEntityFromContext, _hiddenAlpha))
            End If

            _itemHide.Visible = False
            If CallSelectionChanged Then
                RaiseEvent SelectionChangedInD3DComparer(Me, New ComparerSelectionChangedEventArgs())
            End If
            Design3D.Invalidate()
        End Sub

        Private Function GetChangedObject(en As Entity) As ChangedObjectEx
            Dim myChangedEntity As ChangedObjectEx = Nothing
            Dim myBaseEntity As BaseModelEntity = CType(en, BaseModelEntity)
            myChangedEntity = _kblId_EntityMapper.Values.Where(Function(val) val.Entity.Id = myBaseEntity.Id).FirstOrDefault

            Return myChangedEntity
        End Function

        Private Sub ShowTooltipOnEntity(sender As Object, e As EventArgs)
            ShowEntityToolTip(_currentEntityFromContext)
        End Sub

        Private Sub ContextMenuStrip1_Opened(sender As Object, e As EventArgs) Handles ContextMenuStrip1.Opened
            _isContext = False
        End Sub
    End Class

End Namespace