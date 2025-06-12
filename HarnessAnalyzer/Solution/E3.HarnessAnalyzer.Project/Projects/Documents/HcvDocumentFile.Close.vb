Imports System.ComponentModel

Namespace Documents

    Partial Public Class HcvDocument

        Public Shadows Async Function CloseAsync(Optional force As Boolean = False) As Task(Of IResult) 'Expose CloseAsync method to public
            Return Await MyBase.CloseAsync(force)
        End Function

        Public Shadows Function Close(Optional force As Boolean = False) As IResult 'Expose close method to public
            Return MyBase.Close(force)
        End Function

        Protected Overrides Function CloseCore(state As E3.Lib.IO.Files.WorkState, userData As Object) As Task(Of IResult)
            ExecuteCloseCoreWork()
            Return Task.FromResult(CType(Result.Success, IResult))
        End Function

        Private Sub ExecuteCloseCoreWork()
            If Model IsNot Nothing Then
                Model.Dispose()
            End If

            If Entities IsNot Nothing Then
                DisposeEachEntity()
                Entities.Clear()
            End If

            If _converted IsNot Nothing Then
                _converted.Dispose()
            End If

            If _hcvFile IsNot Nothing Then
                _hcvFile.Dispose()
            End If

            _hcvFile = Nothing
            _converted = Nothing

            _LoadedContent = LoadContentType.None
            _spliceLocator = Nothing
            _Model = Nothing
            _VariantUsedToActivate = Nothing
        End Sub

    End Class

End Namespace