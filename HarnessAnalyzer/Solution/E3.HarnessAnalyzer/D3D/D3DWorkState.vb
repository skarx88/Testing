Imports System.ComponentModel
Imports System.Reflection

Namespace D3D

    <Obfuscation(Feature:="renaming", Exclude:=True, ApplyToMembers:=True)>
    Public Class D3DWorkState

        Private _cancel As System.Threading.CancellationToken

        Public Sub New(action As Action(Of Integer))
            Me.New(New Progress(Of Integer)(action))
        End Sub

        Public Sub New(action As Action(Of Integer), result As IResult)
            Me.New(New Progress(Of Integer)(action), result)
        End Sub

        Public Sub New(progress As IProgress(Of Integer))
            Me.New(progress, New Result(ResultState.Undefined))
        End Sub

        Public Sub New(progress As IProgress(Of Integer), result As IResult)
            Me.Progress = progress
            Me.Result = result
        End Sub

        ReadOnly Property Handler As Action(Of Integer)
            Get
                Dim p As Progress(Of Integer) = TryCast(Me.Progress, Progress(Of Integer))
                If p IsNot Nothing Then
                    Dim f As FieldInfo = p.GetType.GetField("_handler", BindingFlags.NonPublic Or BindingFlags.Instance)
                    If f IsNot Nothing Then
                        Return TryCast(f.GetValue(p), Action(Of Integer))
                    End If
                End If
                Return Nothing
            End Get
        End Property

        Property Progress As IProgress(Of Integer)
        Property Result As IResult

        ReadOnly Property IsCancellationRequested As Boolean
            Get
                Return _cancel.IsCancellationRequested
            End Get
        End Property

        Public Function IsCancellingOrCancelled() As Boolean
            Return Me.Result.IsCancelled OrElse IsCancellationRequested
        End Function

        Public Sub AddResult(newResult As IResult)
            If Not TypeOf Me.Result Is AggregatedResult Then
                Me.Result = New AggregatedResult({Me.Result, newResult})
            Else
                CType(Me.Result, AggregatedResult).Add(newResult)
            End If
        End Sub

        Public Sub AddIfFaultedOrCancelled(newResult As IResult)
            If newResult.IsFaultedOrCancelled Then
                Me.AddResult(newResult)
            End If
        End Sub

        'Public Shared Narrowing Operator CType(ByVal initialData As System.Threading.CancellationToken) As D3DWorkState
        '    Return New ThisClass()
        'End Operator

        Public Shared Widening Operator CType(ByVal initialData As D3DWorkState) As System.Threading.CancellationToken
            If initialData IsNot Nothing Then
                Return initialData._cancel
            End If
            Return Nothing
        End Operator
    End Class

End Namespace