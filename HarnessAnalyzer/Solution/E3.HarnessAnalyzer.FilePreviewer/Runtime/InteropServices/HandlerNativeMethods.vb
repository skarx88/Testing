Namespace Global.System.Runtime.InteropServices

    Friend Class HandlerNativeMethods

        <DllImport("user32.dll")>
        Friend Shared Function SetParent(ByVal hWndChild As IntPtr, ByVal hWndNewParent As IntPtr) As IntPtr
        End Function

        <ComImport>
        <InterfaceType(ComInterfaceType.InterfaceIsIUnknown)>
        <Guid("fec87aaf-35f9-447a-adb7-20234491401a")>
        Interface IPreviewHandlerFrame
            Sub GetWindowContext(ByVal pinfo As IntPtr)
            <PreserveSig>
            Function TranslateAccelerator(ByRef pmsg As MSG) As UInteger
        End Interface

        <StructLayout(LayoutKind.Sequential)>
        Friend Structure MSG
            Friend hwnd As IntPtr
            Friend message As Integer
            Friend wParam As IntPtr
            Friend lParam As IntPtr
            Friend time As Integer
            Friend pt_x As Integer
            Friend pt_y As Integer
        End Structure

        Friend Shared IMarshalGuid As System.Guid = New System.Guid("00000003-0000-0000-C000-000000000046")
        Friend Shared IInitializeWithStreamGuid As System.Guid = New System.Guid("b824b49d-22ac-4161-ac8a-9916e8fa3f7f")
        Friend Shared IInitializeWithItemGuid As System.Guid = New System.Guid("7f73be3f-fb79-493c-a6c7-7ee14e245841")
        Friend Shared IInitializeWithFileGuid As Guid = New Guid("b7d14566-0509-4cce-a71f-0a554233bd9b")
        Friend Shared IPreviewHandlerGuid As Guid = New Guid("8895b1c6-b41f-4c1c-a562-0d564250836f")

    End Class

End Namespace