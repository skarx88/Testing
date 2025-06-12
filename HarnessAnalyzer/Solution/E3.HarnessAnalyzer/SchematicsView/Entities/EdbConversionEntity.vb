Imports Zuken.E3.App.Controls

Namespace Schematics.Converter

    Public Class EdbConversionEntity
        Inherits EdbConversionEntityInfo

        Protected Sub New(documentId As String, originalSystemIds() As String, edbSysId As String, shortName As String, edbItem As Connectivity.Model.BaseItem, objectType As Connectivity.Model.ObjType)
            MyBase.New(documentId, edbSysId, originalSystemIds, shortName, objectType)
            Me.EdbItem = edbItem
        End Sub

        Protected Sub New(documentId As String, originalSystemId As String, edbSysId As String, shortName As String, edbItem As Connectivity.Model.BaseItem, objectType As Connectivity.Model.ObjType)
            Me.New(documentId, New String() {originalSystemId}, edbSysId, shortName, edbItem, objectType)
        End Sub

        Public Sub New(documentId As String, originalSystemId As String, edbItem As Connectivity.Model.BaseItem)
            Me.New(documentId, originalSystemId, If(edbItem IsNot Nothing, edbItem.SysId, String.Empty), If(edbItem IsNot Nothing, edbItem.Name, String.Empty), edbItem, If(edbItem IsNot Nothing, edbItem.Type, Connectivity.Model.ObjType.None))
        End Sub

        ReadOnly Property EdbItem As Connectivity.Model.BaseItem

        Public Overrides Property ShortName As String
            Get
                If EdbItem IsNot Nothing Then
                    Return EdbItem.Name
                End If
                Return String.Empty
            End Get
            Set(value As String)
                Throw New NotSupportedException(String.Format("Changing the shortName in an ""{0}"" is not supported because the ""{1}"" does not support it!", Me.GetType.Name, Me.EdbItem?.GetType.Name))
            End Set
        End Property

        Public Overridable Property IsActive As Boolean
            Get
                If Me.EdbItem IsNot Nothing Then
                    Return Me.EdbItem.IsActive
                End If
                Return False
            End Get
            Set(value As Boolean)
                If Me.EdbItem IsNot Nothing Then
                    Me.EdbItem.IsActive = value
                End If
            End Set
        End Property

        Public Overridable Property IsVirtual As Boolean

    End Class

End Namespace
