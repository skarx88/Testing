Imports Zuken.E3.HarnessAnalyzer.Project.Documents
Imports Zuken.E3.Lib.IO.Files.Project

Public Class BaseHcvDocumentCollection(Of TDocument As HcvDocument)
    Inherits OCSFilesCollection(Of TDocument)

    Public Sub New()
        MyBase.New()
    End Sub

    ReadOnly Property Project As BaseProject(Of TDocument)
        Get
            Return CType(MyBase.Owner, BaseProject(Of TDocument))
        End Get
    End Property

    Protected Overrides Function GetNewFileInstanceCore(data As Object) As TDocument
        Dim doc As TDocument = MyBase.GetNewFileInstanceCore(data)
        doc.SynchronizationContext = Me.Project?.SynchronizationContext
        Return doc
    End Function

End Class

