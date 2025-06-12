Imports System.ComponentModel
Imports Zuken.E3.HarnessAnalyzer.Project.Documents
Imports Zuken.E3.Lib.Eyeshot.Model
Imports Zuken.E3.Lib.IO.Files

Friend Class UpdateEntitiesVisibilityWork
    Inherits WorkChunkAsync

    Private _rowFilter As RowFiltersCollection

    Public Sub New(description As String, progressType As ProgressState, rowFilter As RowFiltersCollection)
        MyBase.New(description, progressType)
        _rowFilter = rowFilter
    End Sub

    Protected Overrides Sub DoWorkCore()
        For Each ent As IBaseModelEntityEx In CType(Me.File, HcvDocument).Entities
            ent.Visible = Not _rowFilter.IsFiltered(ent)
        Next
    End Sub

    Public Overrides Function GetResult() As Object
        Return Result.Success
    End Function

End Class
