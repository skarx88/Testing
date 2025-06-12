Imports System.ComponentModel

Namespace Schematics.Identifier.Component

    <Serializable>
    Public Class IdentifierCollection
        Inherits BindingList(Of Identifier)

        Protected Overrides Sub InsertItem(index As Integer, item As Identifier)
            MyBase.InsertItem(index, item)
        End Sub

        Public Overloads Function AddNew(typeDef As ConditionType, value As String) As Identifier
            Dim newIdentifier As New Identifier(typeDef, value)
            Me.Add(newIdentifier)
            Return newIdentifier
        End Function

    End Class

End Namespace

