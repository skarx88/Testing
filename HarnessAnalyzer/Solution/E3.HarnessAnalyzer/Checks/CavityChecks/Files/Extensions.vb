Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports System.Runtime.Serialization

Namespace Checks.Cavities.Files

    Public Module Extensions

        <Extension>
        Public Sub CopyPropertiesTo(Of T)(source As T, target As Object, Optional filter As Func(Of String, Boolean) = Nothing)
            For Each p As PropertyInfo In source.GetType.GetProperties
                Dim targetProp As PropertyInfo = target.GetType.GetProperty(p.Name)
                If targetProp IsNot Nothing AndAlso targetProp.CanWrite AndAlso targetProp.PropertyType = p.PropertyType Then
                    If filter Is Nothing OrElse Not filter.Invoke(p.Name) Then
                        targetProp.SetValue(target, p.GetValue(source))
                    End If
                End If
            Next
        End Sub

        <Extension>
        Public Sub AddProperties(info As SerializationInfo, obj As Object, Optional getValue As Func(Of String, Object) = Nothing)
            Dim added As New HashSet(Of String)
            For Each p As PropertyInfo In obj.GetType.GetProperties
                If added.Add(p.Name) Then
                    Dim entryValue As Object = Nothing
                    If getValue IsNot Nothing Then
                        entryValue = getValue.Invoke(p.Name)
                    End If
                    If entryValue Is Nothing Then
                        entryValue = p.GetValue(obj)
                    End If

                    info.AddValue(p.Name, entryValue)
                End If
            Next
        End Sub

        <Extension>
        Public Sub SetProperties(info As SerializationInfo, obj As Object)
            For Each entry As SerializationEntry In info
                obj.GetType.GetProperty(entry.Name).SetValue(obj, entry.Value)
            Next
        End Sub

    End Module

End Namespace