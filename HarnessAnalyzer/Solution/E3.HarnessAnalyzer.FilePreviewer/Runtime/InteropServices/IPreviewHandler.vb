Imports System.Runtime.InteropServices

<ComImport>
<InterfaceType(ComInterfaceType.InterfaceIsIUnknown)>
<Guid("8895b1c6-b41f-4c1c-a562-0d564250836f")>
Public Interface IPreviewHandler
    Sub SetWindow(hwnd As IntPtr, ByRef rect As NativeRect)
    Sub SetRect(ByRef rect As NativeRect)
    Sub DoPreview()
    Sub Unload()
    Sub SetFocus()
    Sub QueryFocus(<Out> ByRef phwnd As IntPtr)
    <PreserveSig> Function TranslateAccelerator(ByRef pmsg As Message) As HResult
End Interface

