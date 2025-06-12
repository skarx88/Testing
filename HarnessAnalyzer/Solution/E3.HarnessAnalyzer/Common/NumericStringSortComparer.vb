Imports Infragistics.Win.UltraWinGrid
Imports Infragistics.Win.UltraWinTree

Public Class NumericStringSortComparer
    Implements IComparer

    Private _considerTreeNodeCheckedState As Boolean = False

    Public Sub New()
    End Sub

    Friend Sub New(considerTreeNodeCheckedState As Boolean)
        _considerTreeNodeCheckedState = considerTreeNodeCheckedState
    End Sub

    Public Overridable Function Compare(x As Object, y As Object) As Integer Implements IComparer.Compare
        Dim value1 As String = String.Empty
        Dim value2 As String = String.Empty

        If (TypeOf x Is UltraGridCell) Then
            If (DirectCast(x, UltraGridCell).Value.ToString <> String.Empty) Then
                value1 = DirectCast(x, UltraGridCell).Value.ToString.SplitSpace(0)
            End If
            If (DirectCast(y, UltraGridCell).Value.ToString <> String.Empty) Then
                value2 = DirectCast(y, UltraGridCell).Value.ToString.SplitSpace(0)
            End If
        ElseIf (TypeOf x Is UltraTreeNode) Then
            value1 = DirectCast(x, UltraTreeNode).Text
            value2 = DirectCast(y, UltraTreeNode).Text

            If (_considerTreeNodeCheckedState) Then
                value1 = String.Format("{0}|{1}", If(DirectCast(x, UltraTreeNode).CheckedState = CheckState.Checked, "0", If(DirectCast(x, UltraTreeNode).CheckedState = CheckState.Indeterminate, "1", "2")), value1)
                value2 = String.Format("{0}|{1}", If(DirectCast(y, UltraTreeNode).CheckedState = CheckState.Checked, "0", If(DirectCast(y, UltraTreeNode).CheckedState = CheckState.Indeterminate, "1", "2")), value2)
            End If
        ElseIf (TypeOf x Is String) Then
            value1 = DirectCast(x, String)
            value2 = DirectCast(y, String)
        Else
            Return 0
        End If

        Return Compare(value1, value2)
    End Function

    Protected Overridable Function Compare(x As String, y As String) As Integer
        Dim isNumX As Boolean = IsNumeric(x)
        Dim isNumY As Boolean = IsNumeric(y)

        If (isNumX AndAlso isNumY) Then
            Return CSng(x).CompareTo(CSng(y))
        End If
        If (isNumX AndAlso Not isNumY) Then
            Return -1
        End If
        If (Not isNumX AndAlso isNumY) Then
            Return 1
        End If

        Return [String].Compare(x.ToString, y.ToString, True)
    End Function

End Class