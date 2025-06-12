Imports System.Runtime.InteropServices

''' <summary>
''' Provides means by which to initialize with a file.
''' </summary>
<ComImport>
<InterfaceType(ComInterfaceType.InterfaceIsIUnknown)>
<Guid("b7d14566-0509-4cce-a71f-0a554233bd9b")>
Public Interface IInitializeWithFile
    ''' <summary>
    ''' Initializes with a file.
    ''' </summary>
    ''' <param name="filePath"></param>
    ''' <param name="fileMode"></param>
    Sub Initialize(
    <MarshalAs(UnmanagedType.LPWStr)> filePath As String, fileMode As Shell.AccessModes)
End Interface

