Imports System.ComponentModel
Imports Infragistics.Win.UltraWinGrid

Namespace Checks.Cavities

    <Reflection.Obfuscation(Feature:="renaming", ApplyToMembers:=False, Exclude:=True)> ' needed to avoid resource-file-names obfuscation errors, see issue #5
    Public Class NaviUltraGrid
        Inherits UltraGrid

        Private _isSelectionDragWires As Boolean
        Private _selectionEventEnabled As Boolean = True

        Protected Overrides Sub OnKeyDown(e As KeyEventArgs)
            If e.Shift Then
                _isSelectionDragWires = True
            End If

            MyBase.OnKeyDown(e)
        End Sub

        Protected Overrides Sub OnKeyUp(e As KeyEventArgs)
            If e.Shift Then
                _isSelectionDragWires = False
            End If

            MyBase.OnKeyUp(e)
        End Sub

        Protected Overrides Sub OnAfterSelectChange(e As AfterSelectChangeEventArgs)
            If _selectionEventEnabled Then
                MyBase.OnAfterSelectChange(e)
            End If
        End Sub


        Protected Overrides Sub OnMouseUp(e As MouseEventArgs)
            _isSelectionDragWires = False
            MyBase.OnMouseUp(e)
        End Sub

        Protected Overrides Sub OnSelectionDrag(e As CancelEventArgs)
            _isSelectionDragWires = True
            MyBase.OnSelectionDrag(e)
        End Sub

    End Class

End Namespace