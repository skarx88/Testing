Imports System.ComponentModel
Imports System.Drawing
Imports System.IO
Imports System.Runtime.CompilerServices

<HideModuleName>
Public Module Extensions

#Region "Collection-Extensions"

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
    Friend Function AddOrUpdateGet(Of TKey, TValue, TList As IList)(dic As Dictionary(Of TKey, TList), key As TKey, value As TValue, Optional newFactory As Func(Of TList) = Nothing, Optional distinct As Boolean = False) As TList
        If Not dic.ContainsKey(key) Then
            Dim newList As TList
            If newFactory IsNot Nothing Then
                newList = newFactory.Invoke()
            Else
                newList = Activator.CreateInstance(Of TList)()
            End If

            dic.Add(key, newList)
        End If

        Dim list As TList = dic(key)
        If Not distinct OrElse Not list.Contains(value) Then
            list.Add(value)
        End If

        Return list
    End Function

    <Extension>
    Friend Function TryAdd(Of Tkey, TValue, TList As IList)(ByVal dic As Dictionary(Of Tkey, TList), key As Tkey, value As TValue) As Boolean
        If Not dic.ContainsKey(key) Then
            dic.AddOrUpdate(key, value)
            Return True
        End If
        Return False
    End Function

    <Extension>
    Friend Function TryAdd(Of Tkey, TValue)(ByVal dic As Dictionary(Of Tkey, TValue), key As Tkey, value As TValue) As Boolean
        If Not dic.ContainsKey(key) Then
            dic.Add(key, value)
            Return True
        End If
        Return False
    End Function

    <Extension>
    Friend Function AddNewOrGet(Of Tkey, TValue)(dic As Dictionary(Of Tkey, TValue), key As Tkey) As TValue
        If Not dic.ContainsKey(key) Then
            Dim value As TValue = Activator.CreateInstance(Of TValue)()
            dic.Add(key, value)
            Return value
        Else
            Return dic(key)
        End If
    End Function

    <Extension>
    Friend Function AddOrGet(Of Tkey, TValue)(ByRef dic As Dictionary(Of Tkey, TValue), key As Tkey, value As TValue) As TValue
        If Not dic.ContainsKey(key) Then
            dic.Add(key, value)
            Return value
        Else
            Return dic(key)
        End If
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

    <Extension>
    Friend Sub AddRange(Of T)(bList As BindingList(Of T), items As IEnumerable(Of T))
        For Each item As T In items
            bList.Add(item)
        Next
    End Sub

    <Extension>
    Public Sub ForEach(Of T)(numerable As IEnumerable(Of T), action As Action(Of T))
        For Each item As T In numerable
            action.Invoke(item)
        Next
    End Sub

#End Region

#Region "EnumExtensions"
    <System.Runtime.CompilerServices.Extension>
    Public Function GetFlags(value As [Enum]) As IEnumerable(Of [Enum])
        Return GetFlags(value, [Enum].GetValues(value.[GetType]()).Cast(Of [Enum])().ToArray())
    End Function

    ''' <summary>
    ''' gets all the individual flags for a type. So values containing multiple bits are left out.
    ''' </summary>
    ''' <param name="value"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <System.Runtime.CompilerServices.Extension>
    Public Function GetIndividualFlags(value As [Enum]) As IEnumerable(Of [Enum])
        Return GetFlags(value, GetFlagValues(value.[GetType]()).ToArray())
    End Function

    Private Function GetFlags(value As [Enum], values As [Enum]()) As IEnumerable(Of [Enum])
        Dim bits As ULong = Convert.ToUInt64(value)
        Dim results As New List(Of [Enum])()
        For i As Integer = values.Length - 1 To 0 Step -1
            Dim mask As ULong = Convert.ToUInt64(values(i))
            If i = 0 AndAlso mask = 0L Then
                Exit For
            End If
            If (bits And mask) = mask Then
                results.Add(values(i))
                bits -= mask
            End If
        Next
        If bits <> 0L Then
            Return Enumerable.Empty(Of [Enum])()
        End If
        If Convert.ToUInt64(value) <> 0L Then
            results.Reverse()
            Return results
        End If
        If bits = Convert.ToUInt64(value) AndAlso values.Length > 0 AndAlso Convert.ToUInt64(values(0)) = 0L Then
            Return values.Take(1)
        End If
        Return Enumerable.Empty(Of [Enum])()
    End Function

    Private Function GetFlagValues(enumType As Type) As IEnumerable(Of [Enum])
        Dim flag As ULong = &H1
        Dim list As New List(Of [Enum])
        For Each value As [Enum] In [Enum].GetValues(enumType).Cast(Of [Enum])()
            Dim bits As ULong = Convert.ToUInt64(value)
            If bits = 0L Then
                'yield return value;
                Continue For
            End If
            ' skip the zero value
            While flag < bits
                flag <<= 1
            End While
            If flag = bits Then
                list.Add(value)
            End If
        Next
        Return list
    End Function

#End Region

#Region "ImageExtensions"

#If NETFRAMEWORK Or WINDOWS Then
    <Extension>
    Public Sub DrawOverlay(ByRef img As System.Drawing.Image, pt As Point, overlay As System.Drawing.Image)
        DrawOverlay(img, New PointF(CSng(pt.X), CSng(pt.Y)), overlay)
    End Sub

    <Extension>
    Public Sub DrawOverlay(ByRef img As System.Drawing.Image, pt As PointF, overlay As System.Drawing.Image)
        Dim imgClone As System.Drawing.Image = CType(img.Clone, Image)
        Using g As Graphics = Graphics.FromImage(imgClone)
            g.DrawImage(overlay, pt)
        End Using
        img = imgClone
    End Sub

    <Extension>
    Public Function OnDebugCheckForVectorDrawEvaluationError(ex As Exception) As Boolean
#If DEBUG Or CONFIG = "Debug" Then
            If ex.IsVectorDrawEvaluationError Then
            If SkipVectorDrawLicenseErrors OrElse System.Windows.Forms.MessageBox.Show($"DEBUG-QUESTION:{vbCrLf}Try to proceed without vectorDraw control initialization ?{vbCrLf}(When selecting yes: proceed carefully and expect more licensing error messages!)", E3.Lib.DotNet.Expansions.Devices.My.Application.Info.Title, System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Warning) = System.Windows.Forms.DialogResult.Yes Then
                ' do nothing, try to proceed without vectorDraw if evaluation period is not longer present in debug mode
                SkipVectorDrawLicenseErrors = True
                Return True
            End If
        End If
#End If
        Return SkipVectorDrawLicenseErrors
    End Function

    Friend Property SkipVectorDrawLicenseErrors As Boolean = False

    <Extension>
    Friend Function IsVectorDrawEvaluationError(ex As Exception) As Boolean
        If ex.Source.StartsWith("VectorDraw") AndAlso ex.Message.Contains("License") AndAlso SkipVectorDrawLicenseErrors Then
            Return True
        End If
        Return ex.GetInnerOrDefaultMessage.Contains(VECTORDRAW_EVALUATION_EXPIRED_EXCEPTION_MESSAGE)
    End Function

#End If
#End Region

End Module
