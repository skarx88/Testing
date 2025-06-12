Namespace Global.System.Runtime.InteropServices

    <ComImport>
    <Guid("00000114-0000-0000-C000-000000000046")>
    <InterfaceType(ComInterfaceType.InterfaceIsIUnknown)>
    Friend Interface IOleWindow
        Sub GetWindow(<Out> ByRef phwnd As IntPtr)
        Sub ContextSensitiveHelp(
        <MarshalAs(UnmanagedType.Bool)> fEnterMode As Boolean)
    End Interface

End Namespace