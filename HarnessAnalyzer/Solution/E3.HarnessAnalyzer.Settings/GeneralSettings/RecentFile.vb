<Serializable()> _
Public Class RecentFile

    Public Property DirectoryName As String
    Public Property FileName As String

    Public Sub New()
        DirectoryName = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
        FileName = "Unknown"
    End Sub

    Public Sub New(directory As String, file As String)
        DirectoryName = directory
        FileName = file
    End Sub

End Class

Public Class RecentFileList
    Inherits List(Of RecentFile)

    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub AddRecentFile(recentFile As RecentFile)
        If (Not ContainsRecentFile(recentFile)) Then
            Me.Add(recentFile)
        End If
    End Sub

    Public Function ContainsRecentFile(recentFile As RecentFile) As Boolean
        For Each recFile As RecentFile In Me
            If (recFile.DirectoryName.ToLower = recentFile.DirectoryName.ToLower) AndAlso (recFile.FileName.ToLower = recentFile.FileName.ToLower) Then Return True
        Next

        Return False
    End Function

    Public Sub DeleteRecentFile(file As String)
        Me.Remove(FindRecentFile(file))
    End Sub

    Public Function FindRecentFile(file As String) As RecentFile
        For Each recFile As RecentFile In Me
            If (recFile.DirectoryName.ToLower = IO.Path.GetDirectoryName(file).ToLower) AndAlso (recFile.FileName.ToLower = IO.Path.GetFileName(file).ToLower) Then Return recFile
        Next

        Return Nothing
    End Function

End Class
