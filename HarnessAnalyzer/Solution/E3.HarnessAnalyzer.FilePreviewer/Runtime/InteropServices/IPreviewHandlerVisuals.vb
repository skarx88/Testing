Namespace Global.System.Runtime.InteropServices

    <ComImport>
    <InterfaceType(ComInterfaceType.InterfaceIsIUnknown)>
    <Guid("8327b13c-b63f-4b24-9b8a-d010dcc3f599")>
    Friend Interface IPreviewHandlerVisuals
        Sub SetBackgroundColor(color As NativeColorRef)
        Sub SetFont(ByRef plf As LogFont)
        Sub SetTextColor(color As NativeColorRef)
    End Interface

End Namespace