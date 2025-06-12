Imports System.ComponentModel

Namespace Checks.Cavities.Views.Model
    Public Class CavityWiresBindingList
        Inherits BindingList(Of CavityWireView)

        Public Sub New(owner As BaseView, list As List(Of CavityWireView))
            MyBase.New(list)
            Me.Owner = owner
            For Each item As CavityWireView In list
                item.Parent = Me.Owner
            Next
        End Sub

        Public Sub New(owner As BaseView)
            MyBase.New()
            Me.Owner = owner
        End Sub

        Protected Overrides Sub InsertItem(index As Integer, item As CavityWireView)
            MyBase.InsertItem(index, item)
            item.Parent = Me.Owner
        End Sub

        Protected Overrides Sub RemoveItem(index As Integer)
            Dim item As CavityWireView = Me(index)
            MyBase.RemoveItem(index)
            item.Parent = Nothing
        End Sub

        Protected Overrides Sub ClearItems()
            For Each item As CavityWireView In Me
                item.Parent = Nothing
            Next
            MyBase.ClearItems()
        End Sub

        <DebuggerStepThrough>
        Public Function IsAnyUnchecked() As Boolean
            For Each w As CavityWireView In Me
                If w.CheckState = CheckState.Unchecked Then
                    Return True
                End If
            Next
            Return False
        End Function

        <DebuggerStepThrough>
        Public Function AllChecked() As Boolean
            For Each w As CavityWireView In Me
                If w.CheckState <> CheckState.Checked Then
                    Return False
                End If
            Next
            Return True
        End Function

        <DebuggerStepThrough>
        Public Function AllIntermediate() As Boolean
            For Each w As CavityWireView In Me
                If w.CheckState <> CheckState.Indeterminate Then
                    Return False
                End If
            Next
            Return True
        End Function

        ReadOnly Property Owner As BaseView

    End Class

End Namespace
