Imports Zuken.E3.HarnessAnalyzer.D3D.Document.Controls.ToolTips

Public Class ToolTipControlEventArgs
    Inherits EventArgs

    Public Sub New(control As EntityToolTipControl)
        Me.ToolTipControl = control
    End Sub

    ReadOnly Property ToolTipControl As EntityToolTipControl

End Class

