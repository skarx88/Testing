Imports System.Collections.Specialized
Imports Zuken.E3.App.Controls

Namespace Schematics.Converter.Kbl

    Public Class EdbConversionWireEntity
        Inherits BaseChildrensEntity(Of Connectivity.Model.Wire, EdbConversionCavityEntity)

        Private _specifier As String
        Private _csa As Double
        Private _function As String
        Private WithEvents _modules As New ObserableCollection(Of String)

        Public Sub New(blockId As String, originalSystemId As String, edbWire As Connectivity.Model.Wire)
            MyBase.New(blockId, originalSystemId, edbWire)
        End Sub

        Property Specifier As String
            Get
                Return _specifier
            End Get
            Set(value As String)
                _specifier = value
                Me.EdbItem.SetSpecifier(value)
            End Set
        End Property

        Property Csa As Double
            Get
                Return _csa
            End Get
            Set(value As Double)
                _csa = value
                Me.EdbItem.SetCsa(value)
            End Set
        End Property

        Property [Function] As String
            Get
                Return _function
            End Get
            Set(value As String)
                _function = value
                Me.EdbItem.SetFunction(value)
            End Set
        End Property

        ReadOnly Property Modules As ObserableCollection(Of String)
            Get
                Return _modules
            End Get
        End Property

        Private Sub _modules_CollectionChanged(sender As Object, e As NotifyCollectionChangedEventArgs) Handles _modules.CollectionChanged
            Me.EdbItem.SetModules(_modules.ToList)
        End Sub

        Public ReadOnly Property Cavities As IdCollection(Of EdbConversionCavityEntity)
            Get
                Return MyBase.Children
            End Get
        End Property

        Protected Overrides Sub OnAfterChildrenCollectionItemsAdded(newItems As IEnumerable(Of EdbConversionCavityEntity))
            For Each item As EdbConversionCavityEntity In newItems
                Me.EdbItem.AddCavity(item.EdbItem)
            Next
        End Sub
    End Class

End Namespace