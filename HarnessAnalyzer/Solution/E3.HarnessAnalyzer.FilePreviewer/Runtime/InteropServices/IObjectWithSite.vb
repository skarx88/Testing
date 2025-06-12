Namespace Global.System.Runtime.InteropServices

    <ComImport>
    <InterfaceType(ComInterfaceType.InterfaceIsIUnknown)>
    <Guid("fc4801a3-2ba9-11cf-a229-00aa003d7352")>
    Friend Interface IObjectWithSite
        Sub SetSite(<[In], MarshalAs(UnmanagedType.IUnknown)> pUnkSite As Object)
        Sub GetSite(ByRef riid As Guid, <Out> <MarshalAs(UnmanagedType.IUnknown)> ByRef ppvSite As Object)
    End Interface

End Namespace