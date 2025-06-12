Imports System.ComponentModel

Public Class FinishingTask

    Private _taskAction As Action
    Private _asyncFinished As Func(Of IResult, Task)
    Private _cancel As System.Threading.CancellationToken

    Public Sub New(taskAction As Action, asyncFinishedAction As Func(Of IResult, Task), Optional cancelToken As System.Threading.CancellationToken = Nothing)
        _taskAction = taskAction
        _asyncFinished = asyncFinishedAction
        _cancel = cancelToken
    End Sub

    Public Async Function StartNew() As Task(Of IResult)
        Dim result As IResult = System.ComponentModel.Result.Success

        Try
            Dim t1 As Task = Task.Factory.StartNew(_taskAction, _cancel)
            Await t1
            If t1.IsFaulted Then
                result = ComponentModel.Result.Faulted(t1.Exception.Message)
            ElseIf t1.IsCanceled Then
                result = ComponentModel.Result.Cancelled
            End If
        Catch ex As Exception
            If TypeOf ex Is OperationCanceledException OrElse TypeOf ex Is TaskCanceledException Then
                result = System.ComponentModel.Result.Cancelled(ex.Message)
            Else
                result = New Result(ex)
            End If
        End Try

        If _asyncFinished IsNot Nothing Then
            Try
                Dim t2 As Task = _asyncFinished.Invoke(result)
                Await t2
                If t2.IsFaulted Then
                    result = ComponentModel.Result.Faulted(t2.Exception.Message)
                ElseIf t2.IsCanceled Then
                    result = ComponentModel.Result.Cancelled
                End If
            Catch ex As Exception
                If TypeOf ex Is OperationCanceledException OrElse TypeOf ex Is TaskCanceledException Then
                    result = System.ComponentModel.Result.Cancelled(ex.Message)
                Else
                    result = New Result(ex)
                End If
            End Try
        End If

        Return result
    End Function

End Class
