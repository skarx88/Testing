Imports Zuken.E3.App.Controls

Namespace Schematics.Converter

    Public Class EdbConversionEntity(Of T As Connectivity.Model.BaseItem)
        Inherits EdbConversionEntity

        Protected Sub New(documentId As String, originalSystemIds() As String, edbSysId As String, shortName As String, edbItem As T, objectType As Connectivity.Model.ObjType)
            MyBase.New(documentId, originalSystemIds, edbSysId, shortName, edbItem, objectType)
        End Sub

        Protected Sub New(documentId As String, originalSystemId As String, edbSysId As String, shortName As String, edbItem As T, objectType As Connectivity.Model.ObjType)
            MyBase.New(documentId, originalSystemId, edbSysId, shortName, edbItem, objectType)
        End Sub

        Public Sub New(documentId As String, originalSystemId As String, edb As T)
            MyBase.New(documentId, originalSystemId, edb)
        End Sub

        Public Shadows ReadOnly Property EdbItem As T
            Get
                Return CType(MyBase.EdbItem, T)
            End Get
        End Property

    End Class

End Namespace
