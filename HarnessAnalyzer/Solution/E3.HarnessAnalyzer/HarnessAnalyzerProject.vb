Imports System.IO
Imports Zuken.E3.HarnessAnalyzer.Project.Documents
Imports Zuken.E3.Lib.IO.Files

Public Class HarnessAnalyzerProject
    Inherits HarnessAnalyzer.Project.BaseProject(Of HcvDocument)

    Public Sub New()
    End Sub

    Protected Overrides Sub OnFileAddingNew(e As AddingNewFileEventArgs)
        MyBase.OnFileAddingNew(e)
        If TypeOf e.NewFile Is HcvDocument AndAlso TypeOf e.UsedDataAsSource Is Hcv.HcvFile Then
            Dim hcvDoc As HcvDocument = CType(e.NewFile, HcvDocument)
            Dim hcvFile As Hcv.HcvFile = CType(e.UsedDataAsSource, Hcv.HcvFile)
            hcvDoc.File = hcvFile
        End If
    End Sub

End Class
