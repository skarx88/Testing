Imports System.Runtime.InteropServices.Shell

Namespace Global.System.Runtime.InteropServices

    ''' <summary>
    ''' This interface exposes the <see cref="Load"/> function for initializing the 
    ''' Preview Handler with a <type paramrefname="ShellObject"/>
    ''' This interface can be used in conjunction with the other intialization interfaces,
    ''' but only 1 will be accessed according to the priorities preset by the Windows Shell:
    ''' <type paramrefname="IPreviewFromStream"/>
    ''' <type paramrefname="IPreviewFromShellObject"/>
    ''' <type paramrefname="IPreviewFromFile"/>
    ''' </summary>
    Friend Interface IPreviewFromShellObject
        ''' <summary>
        ''' Provides the <type paramrefname="ShellObject"/> from which a preview should be created.        
        ''' </summary>
        ''' <param name="shellObject">ShellItem for the previewed file</param>
        Sub Load(shellObject As IShellItem)
    End Interface

End Namespace
