Imports Infragistics.Win.UltraWinGrid

Friend Class VerticesRowFilter
    Inherits BaseRootRowFilter

    Friend Sub New()
        Me.New(Nothing, Nothing)
    End Sub

    Public Sub New(info As RowFilterInfo, grid As UltraGrid)
        MyBase.New(info, grid)
    End Sub

    Public Overrides Property KblObjectTypes As KblObjectType()
        Get
            Return {KblObjectType.Node}
        End Get
        Protected Set(value As KblObjectType())
            Throw New NotSupportedException("Setting object types to VerticesRowFilter not supported!")
        End Set
    End Property

End Class
