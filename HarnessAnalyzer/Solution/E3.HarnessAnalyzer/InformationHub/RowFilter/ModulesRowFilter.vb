Imports Infragistics.Win.UltraWinGrid

Friend Class ModulesRowFilter
    Inherits BaseRootRowFilter

    Friend Sub New()
        Me.New(Nothing, Nothing)
    End Sub

    Public Sub New(info As RowFilterInfo, grid As UltraGrid)
        MyBase.New(info, grid, True)
    End Sub

    Public Overrides Property KblObjectTypes As KblObjectType()
        Get
            Return {KblObjectType.Module}
        End Get
        Protected Set(value As KblObjectType())
            Throw New NotSupportedException("Setting object types to ModulesRowFilter not supported!")
        End Set
    End Property

    Protected Overrides Function FilterKblObjectCore(kblObjectType As KblObjectType, systemId As String) As KblObjectFilterType
        If Info?.KBL?.InactiveModules IsNot Nothing AndAlso Not String.IsNullOrEmpty(systemId) Then
            If Info.KBL.InactiveModules.ContainsKey(systemId) Then
                Return KblObjectFilterType.Filtered
            End If
        End If
        Return KblObjectFilterType.Unfiltered
    End Function

End Class
