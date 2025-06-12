Imports System.Runtime.CompilerServices
Imports Zuken.E3.HarnessAnalyzer.FilePreviewer

Namespace Global.System.Runtime.InteropServices.Shell

    <ComImport, System.Runtime.InteropServices.GuidAttribute(Shell.ShellIIDGuid.IEnumIDList), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)>
    Friend Interface IEnumIDList

        <PreserveSig>
        <MethodImpl(MethodImplOptions.InternalCall, MethodCodeType:=MethodCodeType.Runtime)>
        Function [Next](celt As UInteger, <Out> ByRef rgelt As IntPtr, <Out> ByRef pceltFetched As UInteger) As HResult

        <PreserveSig>
        <MethodImpl(MethodImplOptions.InternalCall, MethodCodeType:=MethodCodeType.Runtime)>
        Function Skip(<[In]> celt As UInteger) As HResult

        <PreserveSig>
        <MethodImpl(MethodImplOptions.InternalCall, MethodCodeType:=MethodCodeType.Runtime)>
        Function Reset() As HResult

        <PreserveSig>
        <MethodImpl(MethodImplOptions.InternalCall, MethodCodeType:=MethodCodeType.Runtime)>
        Function Clone(<Out> <MarshalAs(UnmanagedType.Interface)> ByRef ppenum As IEnumIDList) As HResult

    End Interface

End Namespace
