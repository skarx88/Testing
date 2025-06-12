Imports System.Runtime.InteropServices
Imports System.Text

Namespace Global.System.Windows

    Friend Class Native

        <StructLayout(LayoutKind.Sequential)>
        Friend Structure RECT
            Friend Left As Integer
            Friend Top As Integer
            Friend Right As Integer
            Friend Bottom As Integer
        End Structure

        <DllImport("user32.dll")>
        Friend Shared Function GetFocus() As IntPtr
        End Function

        <DllImport("shell32.dll", CharSet:=CharSet.Unicode, ExactSpelling:=True, PreserveSig:=False)>
        Shared Function SHGetKnownFolderPath(<MarshalAs(UnmanagedType.LPStruct)> ByVal rfid As Guid, ByVal dwFlags As UInteger, ByVal hToken As IntPtr) As String
        End Function

        Public Shared ReadOnly Property FOLDERID_LocalAppDataLow As New Guid("{A520A1A4-1780-4FF6-BD18-167343C5AF16}")

    End Class

End Namespace