Imports Zuken.E3.App.Controls

Namespace Schematics.Converter.Kbl

    Public Class EdbConversionCavityEntity
        Inherits BaseChildrensEntity(Of Connectivity.Model.Cavity, EdbConversionCavityEntity)

        Private _parentConnectorEdbId As String

        Public Sub New(blockId As String, originalSystemIds() As String, edbSysId As String, shortName As String, edbCavity As Connectivity.Model.Cavity, parentConnectorEdbId As String)
            MyBase.New(blockId, originalSystemIds, edbSysId, shortName, edbCavity, Connectivity.Model.ObjType.Cavity)
            _parentConnectorEdbId = parentConnectorEdbId
        End Sub

        ReadOnly Property MatingCavities As IdCollection(Of EdbConversionCavityEntity)
            Get
                Return MyBase.Children
            End Get
        End Property

        Protected Overloads Overrides Sub OnAfterChildrenCollectionItemsAdded(newItems As IEnumerable(Of EdbConversionCavityEntity))
            For Each cav As EdbConversionCavityEntity In newItems
                Me.EdbItem.AddMatingCavity(cav.EdbItem)
            Next
        End Sub

        ReadOnly Property ParentConnectorEdbid As String
            Get
                Return _parentConnectorEdbId
            End Get
        End Property

    End Class

End Namespace
