Imports System.Threading

Namespace D3D.Consolidated.Controls

    Public Class D3DCancellationTokenSource
        Inherits CancellationTokenSource

        Private _cancelHasDone As Boolean = False

        Public Sub New(canBeCancelled As Boolean)
            canBeCancelled = True
        End Sub

        Public Sub New()
            MyBase.New()
        End Sub

        Public Async Function CancelAndAsyncWait(Optional timeout As Nullable(Of TimeSpan) = Nothing) As Task(Of Boolean)
            Return Await Task.Factory.StartNew(Function() CancelAndWait(timeout), TaskCreationOptions.AttachedToParent)
        End Function

        Public Function CancelAndWait(Optional timeOut As Nullable(Of TimeSpan) = Nothing) As Boolean
            _cancelHasDone = False
            Me.Cancel()
            If timeOut IsNot Nothing Then
                Return Threading.SpinWait.SpinUntil(Function() _cancelHasDone, CInt(timeOut.Value.TotalMilliseconds))
            Else
                Threading.SpinWait.SpinUntil(Function() _cancelHasDone)
            End If
            Return True
        End Function

        Public Sub TellCancelHasDone()
            _cancelHasDone = True
        End Sub

        Friend Property CanBeCancelled As Boolean = False

    End Class

End Namespace