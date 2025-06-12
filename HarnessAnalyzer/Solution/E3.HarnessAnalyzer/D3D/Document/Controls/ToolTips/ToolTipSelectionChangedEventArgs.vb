Imports Zuken.E3.HarnessAnalyzer.D3D.Document.Controls.ToolTips

Public Class ToolTipSelectionChangedEventArgs
    Inherits ToolTipControlEventArgs

    Public Property SelectedKblId As String = String.Empty

    Public Sub New(control As EntityToolTipControl)
        MyBase.New(control)
    End Sub

    Public Sub New(control As EntityToolTipControl, selectedKblId As String)
        Me.New(control)
        Me.SelectedKblId = selectedKblId
    End Sub

End Class
