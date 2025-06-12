Imports System.Reflection

<Obfuscation(Feature:="renaming", Exclude:=True, ApplyToMembers:=True)>
Public Class BackgroundWorkerPack

    Public Delegate Sub DoOnFinishedDelegate(sender As Object, loadDrawings As Boolean)

    Private _file As [Lib].IO.Files.Hcv.KblContainerFile = Nothing
    Private _loadDrawings As Boolean = True

    Public Sub New(kblFile As [Lib].IO.Files.Hcv.KblContainerFile, loadDrawings As Boolean)
        _file = kblFile
        _loadDrawings = loadDrawings
    End Sub

    ReadOnly Property File As [Lib].IO.Files.Hcv.KblContainerFile
        Get
            Return _file
        End Get
    End Property

    ReadOnly Property HcvFilePath As String

    ReadOnly Property LoadDrawings As Boolean
        Get
            Return _loadDrawings
        End Get
    End Property

End Class