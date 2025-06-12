Imports devDept.Eyeshot
Imports devDept.Eyeshot.Workspace
Imports Zuken.E3.HarnessAnalyzer.Settings
Imports Zuken.E3.Lib.Eyeshot.Model


Namespace D3D.Consolidated.Controls

    Partial Public Class Consolidated3DControl

        Protected Overridable Sub OnSelectedEntitiesChanged(e As SelectedEntitiesChangedEventArgs)
            If EventsEnabled Then
                _IsSelecting = True
                Try
                    RaiseEvent SelectedEntitiesChanged(Me, e)
                Finally
                    _IsSelecting = False
                End Try
            End If
        End Sub

        Private Sub _selectedEntities_CollectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles Design3D.SelectionChanged
            Design3D.StopAllBlinking()
            GetToolBarButton(ToolBarButtons.SynchronizeSelection).Enabled = SelectedEntities.Count > 0

            Dim newItems As New List(Of IBaseModelEntityEx)
            If e.AddedItems IsNot Nothing Then
                newItems = e.AddedItems.Select(Function(ent) ent.Item).Cast(Of IBaseModelEntityEx).ToList
                If newItems.Count > 0 Then
                    OnSelectedEntitiesChanged(New SelectedEntitiesChangedEventArgs(Specialized.NotifyCollectionChangedAction.Add, newItems.Select(Function(ent) ent.Id).Distinct.ToArray))
                End If
            End If

            Dim oldItems As New List(Of IBaseModelEntityEx)
            If e.RemovedItems IsNot Nothing Then
                oldItems = e.RemovedItems.Select(Function(ent) ent.Item).Cast(Of IBaseModelEntityEx).ToList
                If oldItems.Count > 0 Then
                    OnSelectedEntitiesChanged(New SelectedEntitiesChangedEventArgs(Specialized.NotifyCollectionChangedAction.Remove, oldItems.Select(Function(ent) ent.Id).Distinct.ToArray))
                End If
            End If
        End Sub

        Private Sub SynchronizeSelection()
            ZoomFitActiveToSelection()
            Me.Design3D.ActiveViewport.Invalidate()
        End Sub

        Friend Async Function RemoveCar() As Task
            Await _carStateMachine.RemoveCarModel()
        End Function

        Friend Sub ToggleAdjustMode(apply As ObjectManipulatorChangeType)
            _adjustModeStateMachine.Toggle(apply)
        End Sub

        Friend Sub LoadCarTransformationSettings(fileName As String)
            _carStateMachine.LoadCarTransformationSettings(fileName)
        End Sub

        Friend Async Function LoadAndAddCarModel(generalSettings As GeneralSettings, currentTrans As CarTransformationSetting) As Task(Of CarStateMachine.CarModelLoadResult)
            Return Await _carStateMachine.LoadAndAddCarModel(generalSettings, currentTrans)
        End Function

        Friend Async Function RevertCarModel() As Task
            Await _carStateMachine.RevertCarModel()
        End Function

        Friend Sub SetSelectedCar(carFullPath As String)
            CarModelsViewControl.CurrentCarFilePath = carFullPath
        End Sub

        Friend Sub ApplyCarModelChanges()
            _carStateMachine.ApplyCarModelChanges()
        End Sub

    End Class

End Namespace
