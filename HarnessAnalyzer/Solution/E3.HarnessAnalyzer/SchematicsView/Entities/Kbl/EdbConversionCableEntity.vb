Imports Zuken.E3.App.Controls

Namespace Schematics.Converter.Kbl

    Public Class EdbConversionCableEntity
        Inherits BaseChildrensEntity(Of Connectivity.Model.Cable, EdbConversionWireEntity)

        Public Sub New(blockId As String, originalSystemId As String, edbCable As Connectivity.Model.Cable)
            MyBase.New(blockId, originalSystemId, edbCable)
        End Sub

        ReadOnly Property Wires As IdCollection(Of EdbConversionWireEntity)
            Get
                Return MyBase.Children
            End Get
        End Property

        Protected Overrides Sub OnAfterChildrenCollectionItemsAdded(newItems As IEnumerable(Of EdbConversionWireEntity))
            For Each item As EdbConversionWireEntity In newItems
                Me.EdbItem.AddWire(item.EdbItem)
            Next
        End Sub

    End Class

End Namespace
