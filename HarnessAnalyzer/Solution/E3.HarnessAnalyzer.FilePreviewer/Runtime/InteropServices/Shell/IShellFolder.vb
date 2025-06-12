Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports Zuken.E3.HarnessAnalyzer.FilePreviewer

Namespace Global.System.Runtime.InteropServices.Shell

    <ComImport, System.Runtime.InteropServices.GuidAttribute(Shell.ShellIIDGuid.IShellFolder), InterfaceType(ComInterfaceType.InterfaceIsIUnknown), ComConversionLoss>
    Friend Interface IShellFolder
        <MethodImpl(MethodImplOptions.InternalCall, MethodCodeType:=MethodCodeType.Runtime)>
        Sub ParseDisplayName(hwnd As IntPtr,
            <[In], MarshalAs(UnmanagedType.Interface)> pbc As ComTypes.IBindCtx,
            <[In], MarshalAs(UnmanagedType.LPWStr)> pszDisplayName As String,
            <[In], Out> ByRef pchEaten As UInteger,
            <Out> ppidl As IntPtr,
            <[In], Out> ByRef pdwAttributes As UInteger)

        <PreserveSig>
        <MethodImpl(MethodImplOptions.InternalCall, MethodCodeType:=MethodCodeType.Runtime)>
        Function EnumObjects(<[In]> hwnd As IntPtr, <[In]> grfFlags As ShellFolderEnumerationOptions, <Out> <MarshalAs(UnmanagedType.Interface)> ByRef ppenumIDList As IEnumIDList) As HResult

        <PreserveSig>
        <MethodImpl(MethodImplOptions.InternalCall, MethodCodeType:=MethodCodeType.Runtime)>
        Function BindToObject(
            <[In]> pidl As IntPtr, pbc As IntPtr,
            <[In]> ByRef riid As Guid,
            <Out, MarshalAs(UnmanagedType.Interface)> ByRef ppv As IShellFolder) As HResult

        <MethodImpl(MethodImplOptions.InternalCall, MethodCodeType:=MethodCodeType.Runtime)>
        Sub BindToStorage(
            <[In]> ByRef pidl As IntPtr,
            <[In], MarshalAs(UnmanagedType.Interface)> pbc As ComTypes.IBindCtx,
            <[In]> ByRef riid As Guid, <Out> ByRef ppv As IntPtr)

        <MethodImpl(MethodImplOptions.InternalCall, MethodCodeType:=MethodCodeType.Runtime)>
        Sub CompareIDs(
            <[In]> lParam As IntPtr,
            <[In]> ByRef pidl1 As IntPtr,
            <[In]> ByRef pidl2 As IntPtr)

        <MethodImpl(MethodImplOptions.InternalCall, MethodCodeType:=MethodCodeType.Runtime)>
        Sub CreateViewObject(
            <[In]> hwndOwner As IntPtr,
            <[In]> ByRef riid As Guid, <Out> ByRef ppv As IntPtr)

        <MethodImpl(MethodImplOptions.InternalCall, MethodCodeType:=MethodCodeType.Runtime)>
        Sub GetAttributesOf(
            <[In]> cidl As UInteger,
            <[In]> apidl As IntPtr,
            <[In], Out> ByRef rgfInOut As UInteger)


        <MethodImpl(MethodImplOptions.InternalCall, MethodCodeType:=MethodCodeType.Runtime)>
        Sub GetUIObjectOf(
            <[In]> hwndOwner As IntPtr,
            <[In]> cidl As UInteger,
            <[In]> apidl As IntPtr,
            <[In]> ByRef riid As Guid,
            <[In], Out> ByRef rgfReserved As UInteger, <Out> ByRef ppv As IntPtr)

        <MethodImpl(MethodImplOptions.InternalCall, MethodCodeType:=MethodCodeType.Runtime)>
        Sub GetDisplayNameOf(
            <[In]> ByRef pidl As IntPtr,
            <[In]> uFlags As UInteger, <Out> ByRef pName As IntPtr)

        <MethodImpl(MethodImplOptions.InternalCall, MethodCodeType:=MethodCodeType.Runtime)>
        Sub SetNameOf(
            <[In]> hwnd As IntPtr,
            <[In]> ByRef pidl As IntPtr,
            <[In], MarshalAs(UnmanagedType.LPWStr)> pszName As String,
            <[In]> uFlags As UInteger,
            <Out> ppidlOut As IntPtr)
    End Interface
End Namespace
