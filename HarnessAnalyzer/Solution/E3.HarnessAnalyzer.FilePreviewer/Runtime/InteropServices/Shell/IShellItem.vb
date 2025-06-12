Imports System.Runtime.CompilerServices
Imports Zuken.E3.HarnessAnalyzer.FilePreviewer

Namespace Global.System.Runtime.InteropServices.Shell

    <ComImport, System.Runtime.InteropServices.GuidAttribute(Shell.ShellIIDGuid.IShellItem), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)>
    Friend Interface IShellItem
        ' Not supported: IBindCtx.
        <PreserveSig>
        <MethodImpl(MethodImplOptions.InternalCall, MethodCodeType:=MethodCodeType.Runtime)>
        Function BindToHandler(<[In]> pbc As IntPtr, <[In]> ByRef bhid As Guid, <[In]> ByRef riid As Guid, <Out, MarshalAs(UnmanagedType.Interface)> ByRef ppv As IShellFolder) As HResult

        <MethodImpl(MethodImplOptions.InternalCall, MethodCodeType:=MethodCodeType.Runtime)>
        Sub GetParent(<Out> <MarshalAs(UnmanagedType.Interface)> ByRef ppsi As IShellItem)

        <PreserveSig>
        <MethodImpl(MethodImplOptions.InternalCall, MethodCodeType:=MethodCodeType.Runtime)>
        Function GetDisplayName(<[In]> sigdnName As ShellItemDesignNameOptions, <Out> ByRef ppszName As IntPtr) As HResult

        <MethodImpl(MethodImplOptions.InternalCall, MethodCodeType:=MethodCodeType.Runtime)> Sub GetAttributes(<[In]> sfgaoMask As ShellFileGetAttributesOptions, <Out> ByRef psfgaoAttribs As ShellFileGetAttributesOptions)

        <PreserveSig>
        <MethodImpl(MethodImplOptions.InternalCall, MethodCodeType:=MethodCodeType.Runtime)>
        Function Compare(<[In], MarshalAs(UnmanagedType.Interface)> psi As IShellItem, <[In]> hint As SICHINTF, <Out> ByRef piOrder As Integer) As HResult
    End Interface
End Namespace
