Imports System.ComponentModel
Imports Zuken.E3.Lib.Licensing

Public Class LicenseManager
    Inherits LicenseManager(Of ApplicationFeature)

    Public Const VENDOR_ZUKEN As String = "zuken"

    Public Sub New()
        MyBase.New()
    End Sub

    Protected Overrides ReadOnly Property Vendor As String
        Get
            Return VENDOR_ZUKEN
        End Get
    End Property

    Public Function InitAndAuthenticate(Optional features() As String = Nothing, Optional createRegKeys As Boolean = False) As Result
        If features Is Nothing Then
            features = New String() {}
        End If
        Dim result As ErrorCodeResult = Init(createRegKeys)
        Dim errorText As String = String.Format(My.Resources.LicenseErrorStrings.LicenseAccess_ErrorAuth_Msg, result.Message)
        If result.IsSuccess Then
            'HINT: if no features are provided here -> the first feature in [ApplicationFeatures.vb] that is not ignored will be authenticated
            result = AuthenticateFeatures(features)
        End If

        Return result
    End Function

    Public Overrides ReadOnly Property ProductName As String
        Get
            Return "E3.HarnessAnalyzer"
        End Get
    End Property

    Public ReadOnly Property EnvironmentVariables As String()
        Get
            Return MyBase.GetLicensingEnvironmentVariableNames
        End Get
    End Property

    Public Shadows ReadOnly Property LmBorrowExecutablePath As String
        Get
            Return MyBase.LmBorrowExecutablePath
        End Get
    End Property


End Class
