Imports devDept.Eyeshot
Imports devDept.Eyeshot.Entities
Imports Zuken.E3.Lib.Eyeshot.Model
Imports Zuken.E3.Lib.IO.KBL
Imports Zuken.E3.Lib.Model
Imports Zuken.E3.Lib.Schema.Kbl

Namespace Documents

    Public Class DocumentEntitiesCollection
        Inherits ObjectModel.Collection(Of IBaseModelEntityEx)
        Implements IDisposable

        Public Event Updating(sender As Object, e As EventArgs)
        Public Event Updated(sender As Object, e As EntitiesUpdatedEventArgs)
        Public Event EntitiesSelected(sender As Object, e As EntitiesEventArgs)
        Public Event EntitiesVisibilityChanged(sender As Object, e As EntitiesEventArgs)

        Private _entitiesByEEObjectId As New Concurrent.ConcurrentDictionary(Of Guid, List(Of IBaseModelEntityEx))
        Private _isUpdating As Boolean
        Private _doc As HcvDocument

        Public Sub New(doc As HcvDocument)
            _doc = doc
        End Sub

        Protected Overrides Sub ClearItems()
            MyBase.ClearItems()
            If _entitiesByEEObjectId IsNot Nothing Then
                _entitiesByEEObjectId.Clear()
            End If
        End Sub

        Protected Overrides Sub InsertItem(index As Integer, item As IBaseModelEntityEx)
            If item Is Nothing Then
                Throw New ArgumentNullException(NameOf(item))
            End If
            MyBase.InsertItem(index, item)
        End Sub

        Public Sub UpdateColors()
            For Each ent As IBaseModelEntityEx In Me.ToArray
                ent.UpdateColors()
            Next
        End Sub

        Public Sub BeginUpdate(Optional clear As Boolean = False)
            OnUpdating(New EventArgs)
            If clear Then
                Me.Clear()
            End If
            _isUpdating = True
        End Sub

        Public Sub EndUpdate(Optional info As EntitiesUpdateInfo = Nothing)
            RefresEntitesIdsMapping(_doc.Model)
            _isUpdating = False
            OnUpdated(New EntitiesUpdatedEventArgs(info))
        End Sub

        Private Sub RefresEntitesIdsMapping(model As EESystemModel)
            _entitiesByEEObjectId.Clear()

            For Each entity As IBaseModelEntityEx In Me.ToArray
                If TypeOf entity Is IEEObjectIdProvider Then
                    With CType(entity, IEEObjectIdProvider)
                        For Each eeObjectId As Guid In .GetEEObjectIds
                            _entitiesByEEObjectId.GetOrAdd(eeObjectId, Function() New List(Of IBaseModelEntityEx)).Add(entity)
                        Next
                    End With
                End If
            Next
        End Sub

        Public Sub Invalidate()
            DoForAllModels(Of devDept.Eyeshot.Workspace)(Sub(env) env.Invalidate())
        End Sub

        Public Sub UpdateVisibleSelection()
            DoForAllModels(Of devDept.Eyeshot.Workspace)(
                Sub(env)
                    If TypeOf env Is DesignEx Then
                        CType(env, DesignEx).UpdateVisibleSelection()
                    Else
                        env.UpdateVisibleSelection()
                    End If
                End Sub)
        End Sub

        Public Function GetByKblIds(ParamArray kblIds() As String) As IBaseModelEntityEx()
            Dim result As New List(Of IBaseModelEntityEx)
            For Each g_kblId_equal As IEnumerable(Of Tuple(Of String, IBaseModelEntityEx)) In Me.SelectMany(Function(entity) entity.GetKblIds.Select(Function(id) New Tuple(Of String, IBaseModelEntityEx)(id, entity))).GroupBy(Function(tpl) tpl.Item1).Where(Function(g) kblIds.Contains(g.Key))
                result.AddRange(g_kblId_equal.Select(Function(tpl) tpl.Item2))
            Next
            Return result.Flatten.OfType(Of IBaseModelEntityEx).Distinct.ToArray
        End Function

        Public Function GetByEEObjectId(ParamArray eeObjectIds() As Guid) As IBaseModelEntityEx()
            Dim result As New List(Of IBaseModelEntityEx)
            For Each id As Guid In eeObjectIds
                Dim entities As List(Of IBaseModelEntityEx) = Nothing
                If _entitiesByEEObjectId.TryGetValue(id, entities) Then
                    result.AddRange(entities)
                End If
            Next
            Return result.Cast(Of IEntity).Flatten.OfType(Of IBaseModelEntityEx).Distinct.ToArray
        End Function

        Public Function TrySetVisibility(visible As Boolean, ParamArray entities() As IBaseModelEntityEx) As Boolean
            Dim changed As Boolean = False
            Dim list As New List(Of IBaseModelEntityEx)
            For Each ent As IBaseModelEntityEx In entities
                If ent.GetVisible <> visible Then
                    ent.SetVisible(visible)
                    list.Add(ent)
                    changed = True
                End If
            Next

            If changed Then
                OnEntitiesVisibilityChanged(New EntitiesEventArgs(list))
            End If

            Return changed
        End Function

        Public Function SelectByKblId(selected As Boolean, ParamArray ids() As String) As IBaseModelEntityEx()
            Dim list As New List(Of IBaseModelEntityEx)
            Dim changed As Boolean = False

            'Reset all selected
            For Each ent As IBaseModelEntityEx In Me.ToArray
                If ent.Selectable AndAlso ent.Selected Then
                    ent.Selected = False
                    changed = True
                End If
            Next

            'set selection
            For Each kblid As String In ids
                For Each ent As IBaseModelEntityEx In GetByKblIds(kblid)
                    If ent.Selectable Then
                        If ent.Selected <> selected Then
                            ent.Selected = selected
                            list.Add(ent)
                            changed = True
                        End If
                    End If
                Next
            Next

            If changed Then
                OnEntitiesSelected(New EntitiesEventArgs(list))
            End If

            Return list.ToArray
        End Function

        Public Function BlinkEntities(ParamArray eeObjectIds() As Guid) As IBaseModelEntityEx()
            StopAllBlinking()
            Dim entities As IBaseModelEntityEx() = Me.GetByEEObjectId(eeObjectIds)
            BlinkEntities(entities)
            Return entities
        End Function

        Public Shared Function BlinkEntities(ParamArray entities() As IBaseModelEntityEx) As IBaseModelEntityEx()
            StopAllBlinking(entities)
            For Each entity As IBlinkingEntity In entities.OfType(Of IBlinkingEntity)
                entity.SetBlinkState(True, New BlinkEntitySettings(BlinkStyle.BlinkTempSilhouette2, System.Drawing.Color.Red, 1.1))
            Next

            Return entities
        End Function

        Private Shared Sub StopAllBlinking(entities As IEnumerable(Of IBaseModelEntityEx))
            DoForAllModelsCore(Of DesignEx)(entities, Sub(env) env.StopAllBlinking())
        End Sub

        Public Sub StopAllBlinking()
            StopAllBlinking(Me)
        End Sub

        Private Sub DoForAllModels(Of TModel As devDept.Eyeshot.Workspace)(action As Action(Of TModel))
            DoForAllModelsCore(Me, action)
        End Sub

        Private Shared Sub DoForAllModelsCore(Of TModel As devDept.Eyeshot.Workspace)(entities As IEnumerable(Of IBaseModelEntityEx), action As Action(Of TModel))
            For Each entGrp As IGrouping(Of IWorkspace, IBaseModelEntityEx) In entities.GroupBy(Function(ent) ent.Workspace)
                If entGrp.Key IsNot Nothing AndAlso TypeOf entGrp.Key Is TModel Then
                    action.Invoke(CType(entGrp.Key, TModel))
                End If
            Next
        End Sub

        Friend Sub ClearAnnotions()
            For Each entity As BaseModelEntity In Me.Cast(Of IEntity).Flatten.OfType(Of BaseModelEntity).ToArray
                entity.ShowAnnotations = False
                If entity.Annotations IsNot Nothing Then
                    entity.Annotations.Clear()
                End If
            Next
            DoForAllModels(Of devDept.Eyeshot.Design)(Sub(env) env.ClearAllViewsLabels) ' hint: only for safety reasons, normally not necessary because all should be already cleared now
        End Sub

        Public Async Function RegenAsync() As Task
            Dim tasks As New List(Of Task)

            DoForAllModels(Of devDept.Eyeshot.Workspace)(
                Sub(env)
                    If TypeOf env Is DesignEx Then
                        tasks.Add(CType(env, DesignEx).Entities.RegenAsync())
                    Else
                        env.Entities.Regen(New RegenOptions() With {.Async = True})
                    End If
                End Sub)

            Await Task.WhenAll(tasks.ToArray)
        End Function

        Public Function GetAsClones(ParamArray kblIds() As String) As IBaseModelEntityEx()
            Dim list As New List(Of IBaseModelEntityEx)
            For Each entity As IBaseModelEntityEx In GetByKblIds(kblIds)
                Dim clone As IBaseModelEntityEx = CType(entity.Clone, IBaseModelEntityEx)
                list.Add(clone)

                If TypeOf clone Is GenericConnectorEntity Then
                    CType(clone, GenericConnectorEntity).Node = Nothing 'HINT: set internal id for re-link to nothing before entity is added/updated to avoid linking exception because node will not exist in model
                End If
            Next
            Return list.ToArray
        End Function

        Protected Overridable Sub OnUpdating(e As EventArgs)
            RaiseEvent Updating(Me, e)
        End Sub

        Protected Overridable Sub OnUpdated(e As EntitiesUpdatedEventArgs)
            RaiseEvent Updated(Me, e)
        End Sub

        Protected Overridable Sub OnEntitiesSelected(e As EntitiesEventArgs)
            RaiseEvent EntitiesSelected(Me, e)
        End Sub

        Protected Overridable Sub OnEntitiesVisibilityChanged(e As EntitiesEventArgs)
            RaiseEvent EntitiesVisibilityChanged(Me, e)
        End Sub

    End Class

End Namespace