Imports System.Drawing
Imports System.Runtime.InteropServices

<ComImport()>
<Guid("0000010d-0000-0000-C000-000000000046")>
<InterfaceType(ComInterfaceType.InterfaceIsIUnknown)>
Friend Interface IViewObject
    <PreserveSig>
    Function Draw(dwDrawAspect As UInteger, lindex As Integer, pvAspect As IntPtr, ptd As IntPtr, hdcTargetDev As IntPtr, hdcDraw As IntPtr, lprcBounds As Rectangle, lprcWBounds As IntPtr, pfnContinue As IntPtr, dwContinue As Integer) As Integer
    Function GetColorSet(dwDrawAspect As UInteger, lindex As Integer, pvAspect As IntPtr, ptd As IntPtr, hicTargetDev As IntPtr, <Out> ppColorSet As IntPtr) As Integer
    Function Freeze(dwDrawAspect As UInteger, lindex As Integer, pvAspect As IntPtr, <Out> pdwFreeze As UInteger) As Integer
    Function Unfreeze(dwFreeze As UInteger) As Integer
    Function SetAdvise(aspects As UInteger, advf As UInteger, pAdvSink As IntPtr) As Integer
    Function GetAdvise(<Out> pAspects As UInteger, <Out> pAdvf As UInteger, <Out> ppAdvSink As IntPtr) As Integer
End Interface