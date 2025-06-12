Imports System.Reflection

Namespace Schematics.Converter.Kbl

    Partial Public Class KblEdbConverter

        <AttributeUsage(AttributeTargets.Method)>
        Private Class UntypedAddObjectMethodAttribute
            Inherits Attribute
        End Class

        <AttributeUsage(AttributeTargets.Method)>
        Private Class GetEdbSystemIdInternalMethodAttribute
            Inherits Attribute
        End Class

        Private Class AttributeMethodGetter(Of TClass As Class, TAttributeType As Attribute)
            Implements IDisposable

            Private _methods As MethodInfo()
            Private _methodsByParameterTypes As New Dictionary(Of String, Tuple(Of Type(), MethodInfo()))
            Private _disposedValue As Boolean ' So ermitteln Sie überflüssige Aufrufe

            Friend Sub New()
                _methods = GetType(TClass).GetMethods(BindingFlags.Instance Or BindingFlags.NonPublic Or BindingFlags.IgnoreCase).Where(Function(m) m.GetCustomAttributes(Of TAttributeType)().Any()).ToArray
                _methodsByParameterTypes = _methods.GroupBy(Function(mi) GetParametersTypeKey(mi)).ToDictionary(Function(grp) GetKey(grp.Key), Function(grp) New Tuple(Of Type(), MethodInfo())(grp.Key, grp.ToArray))
            End Sub

            Private Function GetParametersTypeKey(mi As MethodInfo) As Type()
                Dim params As ParameterInfo() = mi.GetParameters()
                If params IsNot Nothing Then
                    Return params.Select(Function(pi) pi.ParameterType).ToArray
                End If
                Return Array.Empty(Of Type)()
            End Function

            <DebuggerNonUserCode>
            Private Function GetKey(types As IEnumerable(Of Type)) As String
                Return String.Join(";", types.Select(Function(tp) tp.GUID.ToString))
            End Function

            <DebuggerNonUserCode>
            Public Function GetMethodByTypes(ParamArray parameterTypes() As Type) As MethodInfo()
                Dim foundMethodsWithParamTypes As Tuple(Of Type(), MethodInfo()) = Nothing
                If _methodsByParameterTypes.TryGetValue(GetKey(parameterTypes), foundMethodsWithParamTypes) Then
                    Return foundMethodsWithParamTypes.Item2
                Else
                    For Each methodKv As Tuple(Of Type(), MethodInfo()) In _methodsByParameterTypes.Values
                        If parameterTypes.All(Function(paramType) methodKv.Item1.Any(Function(methodParamType) methodParamType.IsAssignableFrom(paramType))) Then
                            Return methodKv.Item2
                        End If
                    Next
                End If

                ThrowMethodsNotFound(parameterTypes)
                Return Nothing
            End Function

            Private Sub ThrowMethodsNotFound(parameterTypes() As Type)
                Throw New ArgumentException(String.Format("Attributed with ""{0}""-methods for object types ""{1}"" not found!", GetType(TAttributeType).Name, String.Join(",", parameterTypes.Select(Function(type) type.Name))))
            End Sub

            <DebuggerNonUserCode>
            Public Function GetMethods(ParamArray parameters() As Object) As MethodInfo()
                Return GetMethodByTypes(parameters.Select(Function(pObj) pObj.GetType).ToArray)
            End Function

            <DebuggerNonUserCode>
            Public Function GetMethod(ParamArray parameters() As Object) As MethodInfo
                Return GetMethods(parameters).Single
            End Function

            <DebuggerNonUserCode>
            Public Function GetAndInvokeMethod(Of TResult)([object] As Object, ParamArray parameters() As Object) As TResult
                Dim methods As MethodInfo() = GetMethods(parameters)
                For Each method As MethodInfo In methods
                    If GetType(TResult).IsAssignableFrom(method.ReturnType) Then
                        Return CType(method.Invoke([object], parameters), TResult)
                    End If
                Next

                ThrowMethodsNotFound(parameters.Select(Function(obj) obj.GetType).ToArray)
                Return Nothing
            End Function

            Protected Overridable Sub Dispose(disposing As Boolean)
                If Not Me._disposedValue Then
                    If disposing Then
                    End If

                    _methodsByParameterTypes = Nothing
                    _methods = Nothing
                End If
                Me._disposedValue = True
            End Sub

            Public Sub Dispose() Implements IDisposable.Dispose
                ' Ändern Sie diesen Code nicht. Fügen Sie oben in Dispose(disposing As Boolean) Bereinigungscode ein.
                Dispose(True)
                GC.SuppressFinalize(Me)
            End Sub

        End Class

    End Class
End Namespace