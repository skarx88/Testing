Namespace Global.System.Runtime.InteropServices

    <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Auto)>
    Public Class LogFont
        Friend height As Integer
        Friend width As Integer
        Friend escapement As Integer
        Friend orientation As Integer
        Friend weight As Integer
        Friend italic As Byte
        Friend underline As Byte
        Friend strikeOut As Byte
        Friend charSet As Byte
        Friend outPrecision As Byte
        Friend clipPrecision As Byte
        Friend quality As Byte
        Friend pitchAndFamily As Byte
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=32)> Friend lfFaceName As String = String.Empty
    End Class

End Namespace