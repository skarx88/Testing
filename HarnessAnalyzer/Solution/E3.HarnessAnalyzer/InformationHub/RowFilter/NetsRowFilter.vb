Imports Infragistics.Win.UltraWinGrid

Friend Class NetsRowFilter
    Inherits BaseRowFilter

    Friend Sub New()
        Me.New(Nothing, Nothing)
    End Sub

    Public Sub New(info As RowFilterInfo, grid As UltraGrid)
        MyBase.New(info, grid)
    End Sub

    Public Overrides Property KblObjectTypes As KblObjectType()
        Get
            Return {KblObjectType.Connection, KblObjectType.Net}
        End Get
        Protected Set(value As KblObjectType())
            Throw New NotSupportedException("Setting object types to NetsRowFilter not supported!")
        End Set
    End Property

    Public Overrides Function DoRowFilter(row As UltraGridRow, Optional overrideKblObjectType As KblObjectType? = Nothing) As KblObjectFilterType
        If row.HasParent Then ' a child row = connection -> filter connections
            Dim id As String = row.Cells(InformationHub.SYSTEM_ID_COLUMN_KEY)?.Value?.ToString
            Dim result As KblObjectFilterType
            If overrideKblObjectType.HasValue Then
                result = Me.FilterKblObject(id, overrideKblObjectType.Value)
            Else
                result = Me.FilterKblObject(id)
            End If
            Return result
        ElseIf row.Tag IsNot Nothing Then
            If TypeOf row.Tag Is NetRowInfoContainer Then
                Return FilterKblObject(CType(row.Tag, NetRowInfoContainer).NetName, If(overrideKblObjectType.HasValue, overrideKblObjectType.Value, KblObjectType.Net))
            End If
        End If
        Return KblObjectFilterType.Unfiltered
    End Function

    Protected Overrides Function FilterKblObjectCore(kblObjectType As KblObjectType, systemId As String) As KblObjectFilterType
        Select Case kblObjectType
            Case KblObjectType.Connection
                If IsConnectionFiltered(systemId) Then
                    Return KblObjectFilterType.Filtered
                End If
            Case KblObjectType.Net
                If Info.KBL.GetConnections.Where(Function(con) Not String.IsNullOrEmpty(con.Signal_name) AndAlso con.Signal_name = systemId).All(Function(c) IsConnectionFiltered(c.SystemId)) Then
                    Return KblObjectFilterType.Filtered
                End If
        End Select
        Return KblObjectFilterType.Unfiltered
    End Function

    Private Function IsConnectionFiltered(id As String) As Boolean
        Return Me.Info.InactiveObjects.ContainsValue(KblObjectType.Connection, id)
    End Function

End Class
