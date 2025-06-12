Namespace D3D

    Public Class ChangedObject

        Public Property ObjType As KblObjectType
        Public Property SrcId As String
        Public Property KblIdRef As String
        Public Property KblIdComp As String
        Public Property ModelType As ModelType
        Public Property SourceType As KblObjectType

        Public Property ChangeType As CompareChangeType
        Public Property ID As String

        Public Property Net As String = ""
        Public Property WireId As String = ""
        Public Property Special_wire_id As String = ""

        Public Sub New(objType As KblObjectType, srcId As String, kblIdRef As String, kblIdComp As String, changeType As CompareChangeType, modelType As ModelType, Optional sourceType As KblObjectType = Nothing, Optional id As String = "", Optional special_wire_id As String = "")
            Me.ObjType = objType
            Me.SrcId = srcId
            Me.KblIdRef = kblIdRef
            Me.KblIdComp = kblIdComp
            Me.ModelType = modelType
            Me.ChangeType = changeType
            Me.SourceType = sourceType
            Me.ID = id
            Me.Special_wire_id = special_wire_id
        End Sub

        Public Sub New(objType As KblObjectType, srcId As String, kblIdRef As String, modelType As ModelType)
            Me.ObjType = objType
            Me.SrcId = srcId
            Me.KblIdRef = kblIdRef
            Me.ModelType = modelType
        End Sub
    End Class

End Namespace