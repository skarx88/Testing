Imports System.Runtime.InteropServices

Public Class SigningChecker

    Private Declare Auto Function WinVerifyTrust Lib "wintrust.dll" Alias "WinVerifyTrust" (ByVal hwnd As IntPtr, <MarshalAs(UnmanagedType.LPStruct)> ByVal pgActionID As Guid, ByRef pWVTData As WINTRUST_DATA) As Integer

    Public Shared Function IsSigned(ByVal filePath As String) As Boolean
        If filePath Is Nothing Then Throw New ArgumentNullException(NameOf(filePath))
        Dim file As New WINTRUST_FILE_INFO()
        file.cbStruct = Marshal.SizeOf(GetType(WINTRUST_FILE_INFO))
        file.pcwszFilePath = filePath

        Dim data As New WINTRUST_DATA()
        data.cbStruct = Marshal.SizeOf(GetType(WINTRUST_DATA))
        data.dwUIChoice = WTD_UI_NONE
        data.dwUnionChoice = WTD_CHOICE_FILE
        data.fdwRevocationChecks = WTD_REVOKE_NONE
        data.pFile = Marshal.AllocHGlobal(file.cbStruct)
        Marshal.StructureToPtr(file, data.pFile, False)
        Dim hr As Integer

        Try
            hr = WinVerifyTrust(INVALID_HANDLE_VALUE, WINTRUST_ACTION_GENERIC_VERIFY_V2, data)
        Finally
            Marshal.FreeHGlobal(data.pFile)
        End Try

        Return hr = 0
    End Function

    <StructLayoutAttribute(LayoutKind.Sequential, CharSet:=CharSet.Unicode)>
    Private Structure WINTRUST_FILE_INFO
        Public cbStruct As Integer
        Public pcwszFilePath As String
        Public hFile As IntPtr
        Public pgKnownSubject As IntPtr
    End Structure

    <StructLayoutAttribute(LayoutKind.Sequential)>
    Private Structure WINTRUST_DATA
        Public cbStruct As Integer
        Public pPolicyCallbackData As IntPtr
        Public pSIPClientData As IntPtr
        Public dwUIChoice As Integer
        Public fdwRevocationChecks As Integer
        Public dwUnionChoice As Integer
        Public pFile As IntPtr
        Public dwStateAction As Integer
        Public hWVTStateData As IntPtr
        Public pwszURLReference As IntPtr
        Public dwProvFlags As Integer
        Public dwUIContext As Integer
        Public pSignatureSettings As IntPtr
    End Structure

    Private Const WTD_UI_NONE As Integer = 2
    Private Const WTD_REVOKE_NONE As Integer = 0
    Private Const WTD_CHOICE_FILE As Integer = 1
    Private Shared ReadOnly INVALID_HANDLE_VALUE As IntPtr = New IntPtr(-1)
    Private Shared ReadOnly WINTRUST_ACTION_GENERIC_VERIFY_V2 As Guid = New Guid("{00AAC56B-CD44-11d0-8CC2-00C04FC295EE}")
End Class
