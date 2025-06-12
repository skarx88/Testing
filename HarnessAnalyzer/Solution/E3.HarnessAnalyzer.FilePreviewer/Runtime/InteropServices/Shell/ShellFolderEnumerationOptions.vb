Namespace Global.System.Runtime.InteropServices.Shell
    <Flags>
    Friend Enum ShellFolderEnumerationOptions As UShort
        CheckingForChildren = &H10
        Folders = &H20
        NonFolders = &H40
        IncludeHidden = &H80
        InitializeOnFirstNext = &H100
        NetPrinterSearch = &H200
        Shareable = &H400
        Storage = &H800
        NavigationEnum = &H1000
        FastItems = &H2000
        FlatList = &H4000
        EnableAsync = &H8000
    End Enum

End Namespace