Imports System.ComponentModel
Imports devDept.Eyeshot

Namespace D3D.Consolidated.Controls

    Partial Public Class Consolidated3DControl

        Private _internalRotValuesChanging As Boolean = False

        Private Sub UpdateTextBoxes(Optional updateRot As Boolean = True, Optional updateScale As Boolean = True, Optional updatePosition As Boolean = True)
            _internalRotValuesChanging = True
            SyncLock Me
                With Me.Design3D.ObjectManipulator
                    If updateRot Then
                        UpdateRotationTextBoxes()
                    End If

                    If updateScale Then
                        UpdateScaleNumericEditorValue()
                    End If

                    If updatePosition Then
                        UpdatePositionTextBoxes()
                    End If
                End With
            End SyncLock
            _internalRotValuesChanging = False
        End Sub

        Private Sub UpdatePositionTextBoxes()
            Me.une_PosX.Value = _objManipulatorManager.Differences.Position.X
            Me.une_PosY.Value = _objManipulatorManager.Differences.Position.Y
            Me.une_PosZ.Value = _objManipulatorManager.Differences.Position.Z
        End Sub

        Private Sub UpdateRotationTextBoxes()
            uneRotX.Value = _objManipulatorManager.Differences.Rotation.X
            uneRotY.Value = _objManipulatorManager.Differences.Rotation.Y
            uneRotZ.Value = _objManipulatorManager.Differences.Rotation.Z
        End Sub

        Private Sub UpdateScaleNumericEditorValue()
            Dim scalePercent As Single = _objManipulatorManager.Differences.Scale.Percent
            If scalePercent > CSng(uneScalePercent.MaxValue) Then
                scalePercent = CSng(uneScalePercent.MaxValue)
            End If
            If scalePercent < CSng(uneScalePercent.MinValue) Then
                scalePercent = CSng(uneScalePercent.MinValue)
            End If
            uneScalePercent.Value = scalePercent
        End Sub

        Private Sub _objectManipulator_MouseOver(sender As Object, e As ObjectManipulator.ObjectManipulatorEventArgs)
            SetClipPlanes()
        End Sub

        Private Sub UpdateObjectManipulatorPositionText()
            Me.StatusTextLabel2.Text = String.Format("Current: X {0}, Y {1}, Z {2}", _objManipulatorManager.CenterDifference.X, _objManipulatorManager.CenterDifference.Y, _objManipulatorManager.CenterDifference.Z)
        End Sub

        Private Sub _objManipulatorManager_ObjectManipulatorEnabledChanging(sender As Object, e As CancelEventArgs) Handles _objManipulatorManager.Changing
            e.Cancel = Not _carStateMachine.HasCar
        End Sub

        Private Sub uneBoxes_KeyUp(sender As Object, e As KeyEventArgs) Handles uneRotX.KeyUp, uneRotY.KeyUp, uneRotZ.KeyUp, uneScalePercent.KeyUp, une_PosX.KeyUp, une_PosY.KeyUp, une_PosZ.KeyUp
            If e.KeyCode = Keys.Return Or e.KeyCode = Keys.Enter Then
                _objManipulatorManager.Apply()
            ElseIf e.KeyCode = Keys.Escape Then
                _objManipulatorManager.Cancel(AdjustMode)
            End If
        End Sub

        Private Sub uneRot_ValueChanged(sender As Object, e As EventArgs) Handles uneRotZ.ValueChanged, uneRotY.ValueChanged, uneRotX.ValueChanged
            If Not _internalRotValuesChanging AndAlso _isControlLoaded Then
                _carStateMachine.UpdateRotation(CSng(uneRotX.Value), CSng(uneRotY.Value), CSng(uneRotZ.Value), _adjustCarSetting)
            End If
        End Sub

        Private Sub unePos_ValueChanged(sender As Object, e As EventArgs) Handles une_PosX.ValueChanged, une_PosY.ValueChanged, une_PosZ.ValueChanged
            If Not _internalRotValuesChanging AndAlso _isControlLoaded Then
                _carStateMachine.UpdatePosition(Une_GetXPos, Une_GetYPos, Une_GetZPos, _adjustCarSetting)
            End If
        End Sub

        Private Sub uneScalePercent_ValueChanged(sender As Object, e As EventArgs) Handles uneScalePercent.ValueChanged
            If Not _internalRotValuesChanging AndAlso _isControlLoaded Then
                If CDbl(uneScalePercent.Value) <= CDbl(uneScalePercent.MinValue) Then
                    _internalRotValuesChanging = True
                    uneScalePercent.Value = uneScalePercent.MinValue
                    _internalRotValuesChanging = False
                End If

                If CDbl(uneScalePercent.Value) >= CDbl(uneScalePercent.MaxValue) Then
                    _internalRotValuesChanging = True
                    uneScalePercent.Value = uneScalePercent.MaxValue
                    _internalRotValuesChanging = False
                End If

                If uneScalePercent.Value IsNot Nothing AndAlso CDbl(uneScalePercent.Value) > 0 Then
                    _carStateMachine.UpdateScale(CSng(uneScalePercent.Value), _adjustCarSetting)
                End If
            End If
        End Sub

        Private Function Une_GetXPos() As Single
            Return CSng(Me.une_PosX.Value)
        End Function

        Private Function Une_GetYPos() As Single
            Return CSng(Me.une_PosY.Value)
        End Function

        Private Function Une_GetZPos() As Single
            Return CSng(Me.une_PosZ.Value)
        End Function

        Private Sub _objManipulatorManager_Changed(sender As Object, e As ObjectManipulatorChangeEventArgs) Handles _objManipulatorManager.Changed
            If e.ChangeType.HasFlag(ObjectManipulatorChangeType.Transformation) Then
                UpdateTextsFromObjManipulator()
            End If
        End Sub

        Private Sub UpdateTextsFromObjManipulator()
            UpdateObjectManipulatorPositionText()
            UpdateTextBoxes()
        End Sub

        <Flags>
        Private Enum UpdatePositionType
            None = 0
            Normal = 1
            Added = 2 Or UpdatePositionType.Normal
            RotInv = 4 Or UpdatePositionType.Normal
        End Enum

    End Class

End Namespace
