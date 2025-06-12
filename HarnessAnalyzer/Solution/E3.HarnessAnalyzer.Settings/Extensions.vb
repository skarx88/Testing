Imports System.Runtime.CompilerServices

<HideModuleName>
Friend Module Extensions

    <Extension>
    Friend Function AddOrUpdate(Of TKey, TValue, TList As IList)(dic As Dictionary(Of TKey, TList), key As TKey, value As TValue, Optional newFactory As Func(Of TList) = Nothing, Optional distinct As Boolean = False) As AddOrUpdateResult
        Dim res As AddOrUpdateResult = AddOrUpdateResult.OnlyUpdate
        If Not dic.ContainsKey(key) Then
            Dim newList As TList
            If newFactory IsNot Nothing Then
                newList = newFactory.Invoke()
            Else
                newList = Activator.CreateInstance(Of TList)()
            End If

            dic.Add(key, newList)
            res = AddOrUpdateResult.Added
        End If

        Dim list As TList = dic(key)
        If Not distinct OrElse Not list.Contains(value) Then
            list.Add(value)
        End If

        Return res
    End Function

    <Extension>
    Friend Function TryRemove(Of Tkey, TValue, TList As IList)(ByRef dic As Dictionary(Of Tkey, TList), key As Tkey, value As TValue) As Boolean
        If Not dic.ContainsKey(key) Then
            dic(key).Remove(value)
            If dic(key).Count = 0 Then
                dic.Remove(key)
            End If
            Return True
        End If
        Return False
    End Function

    Friend Enum AddOrUpdateResult
        Added
        OnlyUpdate
    End Enum

End Module
