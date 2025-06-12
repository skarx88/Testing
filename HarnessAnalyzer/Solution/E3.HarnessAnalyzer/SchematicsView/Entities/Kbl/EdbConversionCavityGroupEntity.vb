Imports Zuken.E3.App.Controls

Namespace Schematics.Converter.Kbl

    Public Class EdbConversionCavityGroupEntity
        Inherits EdbConversionCavityEntity
        Implements IEnumerable(Of String)

        Private _groupIds As New HashSet(Of String)

        Public Sub New(blockId As String, originalSystemIds() As String, edbSysId As String, shortName As String, edbCavity As Connectivity.Model.Cavity, parentConnectorEdbId As String)
            MyBase.New(blockId, originalSystemIds, edbSysId, shortName, edbCavity, parentConnectorEdbId)
        End Sub

        Public Function AddEdbId(edbId As String) As Boolean
            Return _groupIds.Add(edbId)
        End Function

        Public Function RemoveId(edbId As String) As Boolean
            Return _groupIds.Remove(edbId)
        End Function

        Public Function Contains(edbId As String) As Boolean
            Return _groupIds.Contains(edbId)
        End Function

        Default ReadOnly Property Item(index As Integer) As String
            Get
                Return _groupIds(index)
            End Get
        End Property

        Public Function Count() As Integer
            Return _groupIds.Count
        End Function

        Public Function GetEnumerator() As IEnumerator(Of String) Implements IEnumerable(Of String).GetEnumerator
            Return _groupIds.GetEnumerator
        End Function

        Private Function GetEnumerator1() As IEnumerator Implements IEnumerable.GetEnumerator
            Return _groupIds.GetEnumerator
        End Function

    End Class

End Namespace
