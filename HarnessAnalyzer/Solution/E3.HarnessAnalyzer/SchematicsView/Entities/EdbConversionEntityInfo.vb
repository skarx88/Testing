Imports Zuken.E3.App.Controls

Namespace Schematics.Converter

    Public Class EdbConversionEntityInfo

        Private _objType As Connectivity.Model.ObjType
        Private _documentId As String
        Private _originalIds As String()
        Private _id As String
        Private _shortName As String

        Public Sub New(documentId As String, id As String, originalIds() As String, shortName As String, objectType As Connectivity.Model.ObjType)
            _id = id
            _documentId = documentId
            _originalIds = originalIds
            _objType = objectType
            _shortName = shortName
        End Sub

        Public Sub New(documentId As String, id As String, originalId As String, shortName As String, objectType As Connectivity.Model.ObjType)
            Me.New(documentId, id, New String() {originalId}, shortName, objectType)
        End Sub

        Public Overridable Property ShortName As String
            Get
                Return _shortName
            End Get
            Set(value As String)
                _shortName = value
            End Set
        End Property

        Public Overridable ReadOnly Property DocumentId As String
            Get
                Return _documentId
            End Get
        End Property

        Public Overridable ReadOnly Property OriginalIds As String()
            Get
                Return _originalIds
            End Get
        End Property

        Public Overridable ReadOnly Property Id As String
            Get
                Return _id
            End Get
        End Property

        Public Overridable ReadOnly Property ObjectType As Connectivity.Model.ObjType
            Get
                Return _objType
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return String.Format("{0} ({1})", Id, Me.ObjectType.ToString)
        End Function

    End Class

End Namespace