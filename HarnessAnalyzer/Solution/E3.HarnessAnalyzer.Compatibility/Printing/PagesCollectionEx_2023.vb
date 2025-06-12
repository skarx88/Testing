#If NETFRAMEWORK Or WINDOWS7_0_OR_GREATER Then
Imports System.Printing

Namespace Printing

    Public Class PagesCollectionEx_2023
        Implements IEnumerable(Of PageEx_2023)

        Private _mediaSizePages As New Dictionary(Of PageMediaSizeName, PageEx_2023)
        Private _unknownPages As New Dictionary(Of String, PageEx_2023)

        Public Function Add(item As PageEx_2023) As Boolean
            If item.PageType = PageMediaSizeName.Unknown Then
                If Not _unknownPages.ContainsKey(item.Name) Then
                    _unknownPages.Add(item.Name, item)
                    Return True
                End If
            Else
                Dim key As PageMediaSizeName = GetKeyForItem(item)
                If Not _mediaSizePages.ContainsKey(key) Then
                    _mediaSizePages.Add(key, item)
                    Return True
                End If
            End If
            Return False
        End Function

        Protected Function GetKeyForItem(item As PageEx_2023) As PageMediaSizeName
            Return item.PageType
        End Function

        Public Function GetEnumerator() As IEnumerator(Of PageEx_2023) Implements IEnumerable(Of PageEx_2023).GetEnumerator
            Return _mediaSizePages.Values.Concat(_unknownPages.Values).GetEnumerator
        End Function

        Private Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
            Return GetEnumerator()
        End Function
    End Class

End Namespace
#End If