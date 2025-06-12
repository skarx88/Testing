Public Class AggregatedCheckedCompareResultInfoResult
    Inherits System.ComponentModel.AggregatedResult(Of CheckedCompareResultInfoResult)

    Public Sub New(collection As IEnumerable(Of CheckedCompareResultInfoResult))
        MyBase.New(collection)
    End Sub

    Protected Sub New()
    End Sub

    Public Function HasTechnical() As Boolean
        Return GetTechnical() IsNot Nothing
    End Function

    Public Function HasGraphical() As Boolean
        Return GetGraphical() IsNot Nothing
    End Function

    Public Function HasFaultedTechnical() As Boolean
        Return Me.GetFaultedTechnical IsNot Nothing
    End Function

    Public Function HasFaultedGraphical() As Boolean
        Return Me.GetFaultedGraphical IsNot Nothing
    End Function

    Public Function GetFaultedGraphical() As CheckedCompareResultInfoResult
        Return Me.Where(Function(res) res.Type = [Lib].IO.Files.Hcv.KnownContainerFileFlags.GCRI AndAlso res.IsFaulted).FirstOrDefault
    End Function

    Public Function GetGraphical() As CheckedCompareResultInfoResult
        Return Me.Where(Function(res) res.Type = [Lib].IO.Files.Hcv.KnownContainerFileFlags.GCRI).FirstOrDefault
    End Function

    Public Function GetTechnical() As CheckedCompareResultInfoResult
        Return Me.Where(Function(res) res.Type = [Lib].IO.Files.Hcv.KnownContainerFileFlags.TCRI).FirstOrDefault
    End Function

    Public Function GetFaultedTechnical() As CheckedCompareResultInfoResult
        Return Me.Where(Function(res) res.Type = [Lib].IO.Files.Hcv.KnownContainerFileFlags.TCRI AndAlso res.IsFaulted).FirstOrDefault
    End Function

    Public Shared Shadows ReadOnly Property Faulted(message As String) As AggregatedCheckedCompareResultInfoResult
        Get
            Return CreateFaulted(Of AggregatedCheckedCompareResultInfoResult)(message)
        End Get
    End Property

    Public Shared Shadows ReadOnly Property Success As AggregatedCheckedCompareResultInfoResult
        Get
            Static mySuccessResult As AggregatedCheckedCompareResultInfoResult = CreateSuccess(Of AggregatedCheckedCompareResultInfoResult)()
            Return mySuccessResult
        End Get
    End Property

End Class
