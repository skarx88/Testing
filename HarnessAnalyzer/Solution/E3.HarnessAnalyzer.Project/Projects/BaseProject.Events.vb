Imports Zuken.E3.HarnessAnalyzer.Project.Documents
Imports Zuken.E3.Lib.IO.Files
Imports Zuken.E3.Lib.IO.Files.Project

Partial Public Class BaseProject(Of TDocument As HcvDocument)

    Public Event ProjectStateChanging(sender As Object, e As OcsCancelEventArgs)
    Public Event ProjectStateChanged(sender As Object, e As OcsResultEventArgs)

    Public Event DocumentStateChanging(sender As Object, e As WorkStateCancelFileEventArgs)
    Public Event DocumentStateChanged(sender As Object, e As WorkStateResultFileEventArgs)

    Protected Overrides Sub OnStateChanging(e As OcsCancelEventArgs)
        MyBase.OnStateChanging(e)
        RaiseEvent ProjectStateChanging(Me, e)
    End Sub

    Protected Overrides Sub OnStateChanged(e As OcsResultEventArgs)
        MyBase.OnStateChanged(e)
        RaiseEvent ProjectStateChanged(Me, e)
    End Sub

    Protected Overrides Sub OnFileStateChanging(e As WorkStateCancelFileEventArgs)
        MyBase.OnFileStateChanging(e)
        RaiseEvent DocumentStateChanging(Me, e)
    End Sub

    Protected Overrides Sub OnFileStateChanged(e As WorkStateResultFileEventArgs)
        MyBase.OnFileStateChanged(e)
        RaiseEvent DocumentStateChanged(Me, e)
    End Sub

End Class

