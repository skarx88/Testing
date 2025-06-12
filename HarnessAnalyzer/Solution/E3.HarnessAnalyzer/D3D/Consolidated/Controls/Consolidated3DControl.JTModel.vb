Imports Zuken.E3.HarnessAnalyzer.D3D.Consolidated.Designs

Namespace D3D.Consolidated.Controls

    Partial Public Class Consolidated3DControl

        Private Sub ClearAndRemoveJTModelGroup()
            If _jtModelGroup IsNot Nothing Then
                Me.RemoveJTModel()
                _jtModelGroup.Clear()
            End If
        End Sub

        Public Sub RemoveJTModel()
            If (_jtModelGroup IsNot Nothing) AndAlso (Not _internalRemovingJTModel) Then
                _internalRemovingJTModel = True
                Entities.Remove(_jtModelGroup)
                OnAfterJTModelGroupChanged(JTModelGroupChangedType.Removed)
                _internalRemovingJTModel = False

                If ToolBarButtonExists(ToolBarButtons.ChangeCarTransparency) Then
                    GetToolBarButton(ToolBarButtons.ChangeCarTransparency).Enabled = False
                End If
            End If
        End Sub

        Private Sub OnAfterJTModelGroupChanged(type As JTModelGroupChangedType)
            If type = JTModelGroupChangedType.Removed Then
                _jtModelGroup = New Group3D()
            End If
        End Sub

        Private Sub SetJTTransparencySettingToControl()
            _transparencyCtrlJT.TransparencyPercent = CUShort(My.Settings.LastTransparencyJT / Byte.MaxValue * 100)
        End Sub

        Private Sub UpdateJTTransparency(Optional invalidate As Boolean = True)
            _jtModelGroup.TransparencyPercent = _transparencyCtrlJT.TransparencyPercent
            If invalidate Then
                Me.Design3D.Invalidate()
            End If
        End Sub

        Private Sub _transparencyCtrlJT_ValueChanged(sender As Object, e As OpacityControl.ValueChangedEventArgs) Handles _transparencyCtrlJT.ValueChanged
            _lastFocusForTransparency = True
            UpdateJTTransparency()
            My.Settings.LastTransparencyJT = CUShort(e.Value * Byte.MaxValue / 100)
        End Sub

        Private Enum JTModelGroupChangedType
            Added
            Removed
        End Enum

    End Class

End Namespace