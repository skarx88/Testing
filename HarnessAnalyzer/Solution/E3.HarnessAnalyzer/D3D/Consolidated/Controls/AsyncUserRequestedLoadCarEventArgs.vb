Imports System.Threading

Namespace D3D.Consolidated.Controls

    Public Class AsyncUserRequestedLoadCarEventArgs
        Inherits AsyncCallBackEventArgs

        Public Sub New(filePath As String)
            MyBase.New()
            Me.FilePath = filePath
        End Sub

        ReadOnly Property FilePath As String
        Property Cancelled As Boolean
    End Class

End Namespace