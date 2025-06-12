Imports System.ComponentModel
Imports devDept.Eyeshot.Entities

Namespace D3D.Consolidated.Controls

    Partial Public Class Consolidated3DControl

        Friend WithEvents ContextMenuStrip1 As New ContextMenuStrip

        Dim currentEntityFromContext As Entity
        Dim currentHiddenEntity As HiddenEntity

        Dim itemShowAll As New ToolStripMenuItem()
        Dim itemShow As New ToolStripMenuItem()
        Dim itemHide As New ToolStripMenuItem()

        Private Sub FitSelectionToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FitSelectionToolStripMenuItem.Click
            ZoomFitActiveToSelection()
            Me.Design3D.ActiveViewport.Invalidate()
        End Sub

        Private Sub FitAllToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FitAllToolStripMenuItem.Click
            ZoomFitAll()
            Me.Design3D.Invalidate()
        End Sub

        Private Sub ListOfDocumentsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ListOfDocumentsToolStripMenuItem.Click
            If _documentsListForm Is Nothing Then
                _documentsListForm = New DocumentsListForm() ' TODO: Why create always a new form (and not only refreshing it?)
                _documentsListForm.Show(Me)
            Else
                If _documentsListForm.IsDisposed Then
                    _documentsListForm = New DocumentsListForm()
                    _documentsListForm.Show(Me)
                Else
                    _documentsListForm.Show(Me)
                End If
            End If
        End Sub

        Private Sub documentsListForm_ItemVisibilityChanged(sender As Object, e As EventArgs) Handles _documentsListForm.ItemVisibilityChanged
            UpdateViews()
        End Sub

        Public Sub InitContextMenu()

            Me.ContextMenuStrip = ContextMenuStrip1

            itemShowAll.Text = Document3DStrings.MenuItem_ShowAll ' "Alle einblenden (a)"
            itemShowAll.Image = My.Resources.ShowAll

            itemShow.Text = Document3DStrings.MenuItem_Show ' "Einblenden (shift)"
            itemShow.Image = My.Resources.Show

            itemHide.Text = Document3DStrings.MenuItem_Hide ' "Ausblenden (h)"
            itemHide.Image = My.Resources.hide

            ContextMenuStrip1.Items.Add(itemShowAll)
            ContextMenuStrip1.Items.Add(itemShow)
            ContextMenuStrip1.Items.Add(itemHide)

            AddHandler itemShowAll.Click, AddressOf ResetAllHiddenEntities
            AddHandler itemShow.Click, AddressOf ResetHiddenEntity
            AddHandler itemHide.Click, AddressOf AddEntityToHiddens
        End Sub

        Private Sub ContextMenuStrip1_Opening(sender As Object, e As CancelEventArgs) Handles ContextMenuStrip1.Opening
            Dim pos As Drawing.Point = Design3D.MouseState.Location
            currentEntityFromContext = Design3D.GetEntityUnderMouseCursor(pos, True)
            Dim myEntities As List(Of Entity) = Design3D.GetSortedListFromCrossingEntities(Design3D, New Rectangle(pos.X - 1, pos.Y - 1, 3, 3), False)

            itemShowAll.Enabled = CBool(Design3D.HiddenEntities.Count > 0)
            itemHide.Visible = False
            itemShow.Visible = False

            If currentEntityFromContext IsNot Nothing AndAlso currentEntityFromContext.Selected Then
                itemHide.Visible = True

            ElseIf myEntities.Count > 0 Then
                currentHiddenEntity = GetEntitiesFromHidden(myEntities).FirstOrDefault()
                itemShow.Visible = CBool(currentHiddenEntity IsNot Nothing)
            ElseIf currentHiddenEntity Is Nothing AndAlso currentEntityFromContext IsNot Nothing Then
                itemHide.Visible = True
            End If

            e.Cancel = ContextMenuStrip1.Items.Count = 0
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
            Design3D.ResetHiddenEntities()
            Design3D.IsHiddenSelection = False
        End Sub

        Private Sub ResetHiddenEntity(sender As Object, e As EventArgs)
            If currentHiddenEntity IsNot Nothing Then
                currentHiddenEntity.Reset()
                Design3D.HiddenEntities.Remove(currentHiddenEntity)
                Design3D.UpdateVisibleSelection(False)
            End If
            itemShow.Visible = False
            Design3D.Invalidate()
        End Sub

        Private Sub AddEntityToHiddens(sender As Object, e As EventArgs)
            If currentEntityFromContext IsNot Nothing Then
                currentEntityFromContext.Selected = False
                Design3D.HiddenEntities.Add(New HiddenEntity(currentEntityFromContext))
                Design3D.UpdateVisibleSelection(True)
            End If
            itemHide.Visible = False
            Design3D.Invalidate()
        End Sub

    End Class

End Namespace

