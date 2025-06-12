Imports System.Runtime.InteropServices.Shell
Imports Zuken.E3.HarnessAnalyzer.FilePreviewer.Shell

Namespace Global.System.Runtime.InteropServices

    ''' <summary>
    ''' Provides means by which to initialize with a ShellObject
    ''' </summary>
    <ComImport>
    <InterfaceType(ComInterfaceType.InterfaceIsIUnknown)>
    <Guid("7f73be3f-fb79-493c-a6c7-7ee14e245841")>
    Friend Interface IInitializeWithItem
        ''' <summary>
        ''' Initializes with ShellItem
        ''' </summary>
        ''' <param name="shellItem"></param>
        ''' <param name="accessMode"></param>
        Sub Initialize(shellItem As IShellItem, accessMode As AccessModes)
    End Interface
End Namespace