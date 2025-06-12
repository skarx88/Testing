Imports System.Threading
Imports devDept.Eyeshot.Entities
Imports Infragistics.Win.UltraWinListView

Namespace D3D.Consolidated.Controls

    Partial Public Class Consolidated3DControl


        Private Sub _transparencyCtrlCar_ValueChanged(sender As Object, e As OpacityControl.ValueChangedEventArgs) Handles _transparencyCtrlCar.ValueChanged
            _lastFocusForTransparency = True
            _carStateMachine.TransparencyPercent = _transparencyCtrlCar.TransparencyPercent
            Design3D.Invalidate()
            My.Settings.LastTransparency = CUShort(_carStateMachine.TransparencyPercent / 100 * Byte.MaxValue)
        End Sub

        Private Sub SetUpperPanelCollapsed(collapsed As Boolean)
            Me.UpperTableLayoutPanel.Visible = Not collapsed
            If UpperTableLayoutPanel.Visible Then
                Me.UpdateTextBoxes()
            End If
        End Sub

        Public Function GetAllModelEntitiesExceptCar() As IEnumerable(Of IEntity)
            Return _carStateMachine.GetAllModelEntitiesExceptCar()
        End Function

        Private Async Sub CarModelsViewControl_UserRequestedLoadCarModelAsync(sender As Object, e As AsyncUserRequestedLoadCarEventArgs) Handles CarModelsViewControl.UserRequestedLoadCarModelAsync
            e.WorkStarted()
            Try
                If _objManipulatorManager.HasChanges Then
                    _objManipulatorManager.Apply()
                End If

                Dim loadResult As System.ComponentModel.IResult = Await _carStateMachine.LoadFromFile(e.FilePath)
                e.Cancelled = loadResult.IsFaultedOrCancelled
            Finally
                e.WorkFinished()
            End Try
        End Sub

        Private Async Sub CarModelsViewControl_UserRequestedUnLoadCarModelAsync(sender As Object, e As AsyncCallBackEventArgs) Handles CarModelsViewControl.UserRequestedUnLoadCarModelAsync
            e.WorkStarted()
            Try
                Await _carStateMachine.RemoveCarModel()
            Finally
                e.WorkFinished()
            End Try
        End Sub

        Private Async Sub _view3D_ToolBarButtonClick(sender As Object, e As ToolBarButtonEventArgs) Handles Me.ToolBarButtonClick
            Using Await _lock.BeginWaitAsync
                Select Case e.Button.Name
                    Case ToolBarButtons.LoadCarModel.ToString
                        Dim onAfterItemAdded As Action(Of UltraListViewItem) =
                            Sub(item As UltraListViewItem)
                                If Not String.IsNullOrEmpty(_carStateMachine.FilePath) Then
                                    Dim fileName As String = IO.Path.GetFileName(item.Key)
                                    If IO.Path.GetFileName(_carStateMachine.FilePath).ToLower = fileName Then
                                        Me.CarModelsViewControl.TrySetItemChecked(item.Key, True, False).WaitWithPumping ' HINT: wait with pumping a temp solution whichs pumps the Message-queue manually after wait is executed when within waited task there is also an UI-Wait this leads to an thread-lock otherwise. The while BL-Tree of this call needs to inspected to find the possible blocks if it was blocking here without WaitWithPumping 
                                    End If
                                End If
                            End Sub

                        If Not CarModelsViewControl.IsDirectoryLoaded Then
                            CarModelsViewControl.LoadDirectoryAsync(onAfterItemAdded)
                        End If

                        viewModelFilesPopUp.Show()
                        viewModelFilesPopUp.PopupControl.Focus()
                    Case ToolBarButtons.ChangeCarTransparency.ToString, ToolBarButtons.ChangeJTTransparency.ToString
                        ToggleTransparencyPopUp(e.Button.Name = ToolBarButtons.ChangeCarTransparency.ToString)
                    Case ToolBarButtons.SynchronizeSelection.ToString
                        SynchronizeSelection()
                        'Case ToolBarButtons.Home.ToString ' this will be removed because handling is currently intersecting with other BL for transformation
                        '    If _adjustModeStateMachine.AdjustMode AndAlso _objManipulatorManager.HasChanges Then
                        '        Select Case MessageBox.Show(Me, DialogStringsD3D.ResetTransformationQuestion, [Shared].MSG_BOX_TITLE, MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                        '            Case DialogResult.Yes
                        '               'TODO: reset tranformation (objectmanipulator cancel, move to original position and re-activate omp)
                        '               
                        '        End Select
                        '    End If
                End Select
            End Using
        End Sub

        Private Sub SetCarTransparencySettingToControl()
            _transparencyCtrlCar.TransparencyPercent = CUShort(My.Settings.LastTransparency / Byte.MaxValue * 100)
        End Sub

        Private Sub ToggleTransparencyPopUp(isCar As Boolean)
            If (isCar) Then
                If Not _transparencyCarPopUp.IsDisplayed Then
                    SetCarTransparencySettingToControl()
                    _transparencyCarPopUp.Show(Me)
                    _transparencyCtrlCar.Focus()
                Else
                    _transparencyCarPopUp.Close()
                End If
            Else
                If Not _transparencyJTPopup.IsDisplayed Then
                    SetJTTransparencySettingToControl()
                    _transparencyJTPopup.Show(Me)
                    _transparencyCtrlJT.Focus()
                Else
                    _transparencyJTPopup.Close()
                End If
            End If
        End Sub

        Private Sub _carStateMachine_Changing(sender As Object, e As CarChangeEventArgs) Handles _carStateMachine.Changing
            Select Case e.ChangeType
                Case CarGroupChangeType.Remove
                    If _objManipulatorManager IsNot Nothing Then
                        _objManipulatorManager.Cancel()
                    End If
            End Select

            RaiseEvent CarChanging(Me, e)
        End Sub

        Private Async Sub _carStateMachine_Changed(sender As Object, e As CarChangeEventArgs) Handles _carStateMachine.Changed
            Select Case e.ChangeType
                Case CarGroupChangeType.Add
                    If AdjustMode Then
                        SetUpperPanelCollapsed(False)
                    End If
                    Await Me.CarModelsViewControl.TrySetItemChecked(_carStateMachine.FilePath, True, False)

                Case CarGroupChangeType.Remove
                    SetUpperPanelCollapsed(True)

                    Dim carFileName As String = String.Empty
                    If Not String.IsNullOrEmpty(_carStateMachine.FilePath) Then
                        carFileName = IO.Path.GetFileName(_carStateMachine.FilePath)
                    End If

                    Dim item As UltraListViewItem = Me.CarModelsViewControl.FindItemsWithFileName(carFileName).FirstOrDefault
                    If item IsNot Nothing Then
                        Await Me.CarModelsViewControl.TrySetItemChecked(item.Key, False, False)
                    End If

                    If ToolBarButtonExists(ToolBarButtons.ChangeCarTransparency) Then
                        GetToolBarButton(ToolBarButtons.ChangeCarTransparency).Enabled = False
                    End If

                    Design3D.Invalidate(True)

                    'Me.CarModelSelectCtrl.CheckedItem = Nothing
                Case Else
                    Throw New NotImplementedException(String.Format("Change type ""{0}"" for car group not implemented!", e.ChangeType.ToString))
            End Select

            RaiseEvent CarChanged(Me, e)
        End Sub

    End Class

End Namespace
