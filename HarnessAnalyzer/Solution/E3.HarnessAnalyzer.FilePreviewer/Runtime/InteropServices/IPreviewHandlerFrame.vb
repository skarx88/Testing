Imports Zuken.E3.HarnessAnalyzer.FilePreviewer

Namespace Global.System.Runtime.InteropServices

    <ComImport>
    <InterfaceType(ComInterfaceType.InterfaceIsIUnknown)>
    <Guid("fec87aaf-35f9-447a-adb7-20234491401a")>
    Friend Interface IPreviewHandlerFrame
        Sub GetWindowContext(pinfo As IntPtr)
        <PreserveSig>
        Function TranslateAccelerator(ByRef pmsg As Message) As HResult
    End Interface

End Namespace