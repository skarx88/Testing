Public Class InformationHubEventArgs
    Inherits EventArgs
    Implements ICloneable

    Public Sub New(mapperSourceId As String)
        Me.KblMapperSourceId = mapperSourceId
    End Sub

    Public Sub New(mapperId As String, objType As KblObjectType, ParamArray objIds As String())
        Me.KblMapperSourceId = mapperId
        ObjectIds = objIds.ToHashSet
        ObjectType = objType
    End Sub

    Public Sub New(mapperId As String, objIds As IEnumerable(Of String), objType As KblObjectType)
        Me.KblMapperSourceId = mapperId
        ObjectIds = objIds.ToHashSet
        ObjectType = objType
    End Sub

    Public Sub New(mapperId As String, objIds As IEnumerable(Of String), objType As KblObjectType, stmpIds As List(Of String))
        Me.KblMapperSourceId = mapperId
        ObjectIds = objIds.ToHashSet
        ObjectType = objType
        StampIds = stmpIds
    End Sub

    Public Sub New(documentId As String, objIds As IEnumerable(Of String), objType As KblObjectType, remPrevSel As Boolean, stmpIds As List(Of String))
        Me.KblMapperSourceId = documentId
        ObjectIds = objIds.ToHashSet
        ObjectType = objType
        RemovePreviousSelection = remPrevSel
        StampIds = stmpIds
    End Sub

    Public Function Clone() As Object Implements ICloneable.Clone
        Dim localArgs As New InformationHubEventArgs(Me.KblMapperSourceId)
        localArgs.ObjectIds.AddRange(Me.ObjectIds)
        localArgs.StampIds.AddRange(Me.StampIds)
        localArgs.ObjectType = Me.ObjectType
        Return localArgs
    End Function

    Public Function AreEqual(args As InformationHubEventArgs) As Boolean
        If Me.KblMapperSourceId <> args.KblMapperSourceId Then
            Return False
        End If

        If Me.ObjectType <> args.ObjectType Then
            Return False
        End If

        If Me.ObjectIds.Count <> args.ObjectIds.Count Then
            Return False
        Else
            If (Me.ObjectIds.Intersect(args.ObjectIds).Count <> Me.ObjectIds.Count) Then
                Return False
            End If
        End If
        If Me.StampIds.Count <> args.StampIds.Count Then
            Return False
        Else
            If (Me.StampIds.Intersect(args.StampIds).Count <> Me.StampIds.Count) Then
                Return False
            End If
        End If
        Return True
    End Function

    Public Property ObjectIds As New HashSet(Of String)
    Public Property ObjectType As KblObjectType
    Public Property RemovePreviousSelection As Boolean
    Public Property SelectIn3DView As Boolean = True
    Public Property HighlightInConnectivityView As Boolean = True
    Public Property StampIds As New List(Of String)
    Public Property KblMapperSourceId As String

End Class