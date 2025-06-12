Namespace Documents

    Public Enum OpenChildFilesFlag
        None = 0
        ''' <summary>
        ''' Open child files also after file was opened successfully
        ''' </summary>
        OpenFiles = 1
        ''' <summary>
        ''' Open all child files recursively after file was opened successfully
        ''' </summary>
        OpenFilesRecursive = 2
    End Enum

End Namespace