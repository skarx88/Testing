Namespace Checks.Cavities.Files

    Friend Class CheckedString

        Private _checkedString As String = String.Empty

        Public Sub New(checkedState As CheckState)
            _checkedString = GetCheckedState(checkedState).ToString
        End Sub

        Public Sub New(state As Checks.Cavities.Views.Model.ConnectorView.State)
            _checkedString = GetCheckedState(state).ToString
        End Sub

        ReadOnly Property Checked As CheckState

        Public Overrides Function ToString() As String
            Return _checkedString
        End Function

        Public Shared Function GetCheckedState(checkState As CheckState) As CheckedState
            Select Case checkState
                Case CheckState.Checked
                    Return CheckedState.Ok
                Case CheckState.Indeterminate
                    Return CheckedState.Unchecked
                Case CheckState.Unchecked
                    Return CheckedState.Error
                Case Else
                    Throw New NotImplementedException($"{checkState.GetType.Name} ""{checkState}"" is not implemented (unknown state)!")
            End Select
        End Function

        Public Shared Function GetCheckedState(state As Views.Model.ConnectorView.State) As CheckedState
            Select Case state
                Case Views.Model.ConnectorView.State.Checked, Views.Model.ConnectorView.State.Indeterminate, Views.Model.ConnectorView.State.AnyUnchecked, Views.Model.ConnectorView.State.Partial
                    Return CType(state, CheckedState)
                Case Else
                    Throw New NotImplementedException($"{state.GetType.Name} ""{state}"" is not implemented (unknown state)!")
            End Select
        End Function


        Public Shared Widening Operator CType(ByVal initialData As CheckedString) As String
            Return initialData.ToString
        End Operator

    End Class

    Public Enum CheckedState
        Unchecked = 0
        [Error] = 1
        Ok = 2
        Unfinished = 3
    End Enum

End Namespace