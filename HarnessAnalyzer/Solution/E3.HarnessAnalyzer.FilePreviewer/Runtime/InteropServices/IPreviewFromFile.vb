Imports System.IO

Namespace Global.System.Runtime.InteropServices

    ''' <summary>
    ''' This interface exposes the <see cref="Load"/> function for initializing the 
    ''' Preview Handler with a <type paramrefname = "FileInfo"/>.
    ''' This interface can be used in conjunction with the other intialization interfaces,
    ''' but only 1 will be accessed according to the priorities preset by the Windows Shell:
    ''' <type paramrefname="IPreviewFromStream"/>
    ''' <type paramrefname="IPreviewFromShellObject"/>
    ''' <type paramrefname="IPreviewFromFile"/>
    ''' </summary>
    Friend Interface IPreviewFromFile
        ''' <summary>
        ''' Provides the <type paramrefname="info"></type> to the item from which a preview should be created.        
        ''' </summary>
        ''' <param name="info">File information to the previewed file.</param>
        Sub Load(info As FileInfo)
    End Interface

End Namespace
