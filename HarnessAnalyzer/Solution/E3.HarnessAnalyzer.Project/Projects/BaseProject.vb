Imports System.ComponentModel
Imports Zuken.E3.HarnessAnalyzer.Project.Documents
Imports Zuken.E3.Lib.IO.Files
Imports Zuken.E3.Lib.IO.Files.Hcv
Imports Zuken.E3.Lib.IO.Files.Project

'HINT: solution-property is currently not exposed because not needed for HA, DoWork is also not exposed we don't want multiple workChunks executed. A WorkChunkCollection is only blocking within itself. To avoid non blocking between document and project-files we only use the WorkerCollection of the documents and don't expose theese here
Public Class BaseProject(Of TDocument As HcvDocument)
    Inherits ProjectFile(Of TDocument)

    Public Sub New()
        MyBase.New
    End Sub

    ReadOnly Property Documents As BaseHcvDocumentCollection(Of TDocument)
        Get
            Return CType(MyBase.Files, BaseHcvDocumentCollection(Of TDocument))
        End Get
    End Property

    Protected Overrides Function NewFilesCollectionInstance() As IO.IFilesCollection
        Return New BaseHcvDocumentCollection(Of TDocument)
    End Function

    Protected Overrides Function CloseCore(state As E3.Lib.IO.Files.WorkState, userData As Object) As Task(Of IResult)
        For Each doc As HcvDocument In Me.Documents.OfType(Of HcvDocument).ToArray
            If Not doc.HasState(OcsState.Close) Then
                'TODO
            End If
        Next

        Return Task.FromResult(CType(Result.Success, IResult))
    End Function

    Public Shared Function AnalyseContent(file As BaseHcvFile, type As Documents.HcvDocument.ContentAnalyseType) As Boolean
        If TypeOf file Is XhcvFile Then
            For Each hcvfile As HcvFile In CType(file, XhcvFile).OfType(Of HcvFile)
                If AnalyseContent(hcvfile, type) Then
                    Return True
                End If
            Next
        ElseIf TypeOf file Is HcvFile Then
            Return HcvDocument.AnalyseContent(CType(file, HcvFile), type)
        End If
        Return False
    End Function

    Public Overloads Function DoWorkChunksAsync(ParamArray workChunks() As IWorkChunk) As Task(Of IResult)
        Return MyBase.DoWorkChunksAsync(Nothing, Nothing, workChunks)
    End Function

    Protected Overrides Function OpenCore(state As E3.Lib.IO.Files.WorkState, userData As Object) As Task(Of IResult)
        'TODO: this is written for future preperations when changing completely to project and not the current intermdiate state
        'For Each doc As HcvDocumentFile In Me.Documents.OfType(Of Documents.Document).ToArray
        '    If Not doc.IsBusy AndAlso Not doc.HasState(OcsWorkState.Open) Then

        '    End If
        'Next

        Return Task.FromResult(CType(Result.Success, IResult))
    End Function

    Public Sub Invalidate()
        For Each doc As HcvDocument In Me.Documents
            doc.Entities.Invalidate()
        Next
    End Sub

End Class

