#If NETFRAMEWORK
Namespace Global.System.Runtime.InteropServices.Shell

    ''' <summary>
    ''' Options for commiting (flushing) an IStream storage stream
    ''' </summary>
    <Flags>
    Friend Enum StorageStreamCommitOptions
        ''' <summary>
        ''' Uses default options
        ''' </summary>
        None = 0

        ''' <summary>
        ''' Overwrite option
        ''' </summary>
        Overwrite = 1

        ''' <summary>
        ''' Only if current
        ''' </summary>
        OnlyIfCurrent = 2

        ''' <summary>
        ''' Commits to disk cache dangerously
        ''' </summary>
        DangerouslyCommitMerelyToDiskCache = 4

        ''' <summary>
        ''' Consolidate
        ''' </summary>
        Consolidate = 8
    End Enum
End Namespace
#End If