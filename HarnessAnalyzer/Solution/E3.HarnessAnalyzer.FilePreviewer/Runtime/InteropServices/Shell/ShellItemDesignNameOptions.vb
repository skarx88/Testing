Namespace Global.System.Runtime.InteropServices.Shell
    Friend Enum ShellItemDesignNameOptions
        Normal = &H0           ' SIGDN_NORMAL
        ParentRelativeParsing = &H80018001   ' SIGDN_INFOLDER | SIGDN_FORPARSING
        DesktopAbsoluteParsing = &H80028000  ' SIGDN_FORPARSING
        ParentRelativeEditing = &H80031001   ' SIGDN_INFOLDER | SIGDN_FOREDITING
        DesktopAbsoluteEditing = &H8004C000  ' SIGDN_FORPARSING | SIGDN_FORADDRESSBAR
        FileSystemPath = &H80058000             ' SIGDN_FORPARSING
        Url = &H80068000                     ' SIGDN_FORPARSING
        ParentRelativeForAddressBar = &H8007C001     ' SIGDN_INFOLDER | SIGDN_FORPARSING | SIGDN_FORADDRESSBAR
        ParentRelative = &H80080001           ' SIGDN_INFOLDER
    End Enum
End Namespace