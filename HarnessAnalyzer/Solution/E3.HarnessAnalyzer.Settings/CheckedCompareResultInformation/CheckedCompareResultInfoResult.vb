Public Class CheckedCompareResultInfoResult
    Inherits System.ComponentModel.Result(Of CheckedCompareResultInformation)

    Public Sub New(data As CheckedCompareResultInformation)
        MyBase.New(data)
    End Sub

    Public Sub New(result As System.ComponentModel.ResultState, Optional data As CheckedCompareResultInformation = Nothing)
        MyBase.New(result, data)
    End Sub

    Public Sub New(exception As Exception, Optional data As CheckedCompareResultInformation = Nothing)
        MyBase.New(exception, data)
    End Sub

    Public Sub New(result As System.ComponentModel.ResultState, message As String, Optional data As CheckedCompareResultInformation = Nothing)
        MyBase.New(result, message, data)
    End Sub

    Protected Sub New()
    End Sub

    Protected Friend Sub New(other As System.ComponentModel.IResult)
        MyBase.New(other)
    End Sub

    Friend ReadOnly Property Type As [Lib].IO.Files.Hcv.KnownContainerFileFlags
        Get
            Return (Me.Data?.Type).GetValueOrDefault
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return $"{Me.Type.ToString}: " + Me.Data.Name
    End Function

    Public ReadOnly Property CCInfo As CheckedCompareResultInformation
        Get
            Return MyBase.Data
        End Get
    End Property

End Class
