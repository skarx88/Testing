Imports Zuken.E3.Lib.Eyeshot.Model

Namespace D3D

    Public Class ChangedObjectEx
        Inherits ChangedObject

        Public Property Entity As IBaseModelEntityEx
        Public Property ModifiedEntity As IBaseModelEntityEx
        Public Property MapperSourceId As String

        Public Sub New(obj As ChangedObject, en As IBaseModelEntityEx, mapperSourceId As String)
            MyBase.New(obj.ObjType, obj.SrcId, obj.KblIdRef, obj.KblIdComp, obj.ChangeType, obj.ModelType, obj.SourceType, obj.ID, obj.Special_wire_id)
            Me.Net = obj.Net
            Me.Entity = en
            Me.MapperSourceId = mapperSourceId
            Me.WireId = obj.WireId
        End Sub
    End Class

End Namespace