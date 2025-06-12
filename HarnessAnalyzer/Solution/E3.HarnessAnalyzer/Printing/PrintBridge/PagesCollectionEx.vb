Imports System.Drawing.Printing
Imports System.Printing
Imports System.Runtime.Serialization

Namespace Printing

    <CollectionDataContract>
    Public Class PagesCollectionEx
        Implements IEnumerable(Of PageEx)

        Private _mediaSizePages As New Dictionary(Of PageMediaSizeName, PageEx)
        Private _unknownPages As New Dictionary(Of String, PageEx)

        Friend Sub New(pages2023 As Compatibility.Printing.PagesCollectionEx_2023)
            For Each page As Compatibility.Printing.PageEx_2023 In pages2023
                Me.Add(New PageEx(page))
            Next
        End Sub

        Public Sub New()

        End Sub

        Public Sub AddOrReplace(item As PageEx)
            If Me.Contains(GetKeyForItem(item)) Then
                Me.Remove(GetKeyForItem(item))
            End If
            Me.Add(item)
        End Sub

        Public Function AddNew(item As PaperSize) As PageEx
            Dim newPage As New PageEx(item)
            Me.Add(newPage)
            Return newPage
        End Function

        Public Function AddNew(item As PageMediaSize) As PageEx
            Dim newPage As New PageEx(item)
            Me.Add(newPage)
            Return newPage
        End Function

        Public Function Add(item As PageEx) As Boolean
            If item.PageType = PageMediaSizeName.Unknown Then
                Return _unknownPages.TryAdd(item.Name, item)
            Else
                Dim key As PageMediaSizeName = GetKeyForItem(item)
                Return _mediaSizePages.TryAdd(key, item)
            End If
            Return False
        End Function

        Default Public ReadOnly Property Item(pageMediaSizeName As PageMediaSizeName) As PageEx
            Get
                Return _mediaSizePages(pageMediaSizeName)
            End Get
        End Property

        Default Public ReadOnly Property Item(pageName As String) As PageEx
            Get
                If _unknownPages.ContainsKey(pageName) Then
                    Return _unknownPages(pageName)
                Else
                    Dim page As PageEx = GetMsPageByName(pageName)
                    If page Is Nothing Then
                        Throw New ArgumentException(String.Format("Item with name ""{0}"" not found!", pageName), NameOf(pageName))
                    End If
                    Return page
                End If
            End Get
        End Property

        Private Function GetMsPageByName(pageName As String) As PageEx
            For Each msPage As PageEx In _mediaSizePages.Values
                If msPage.Name = pageName Then
                    Return msPage
                End If
            Next
            Return Nothing
        End Function

        Public Function Remove(key As PageMediaSizeName) As Boolean
            Return _mediaSizePages.Remove(key)
        End Function

        Public Function Remove(pageName As String) As Boolean
            If _unknownPages.ContainsKey(pageName) Then
                Return _unknownPages.Remove(pageName)
            Else
                Dim page As PageEx = GetMsPageByName(pageName)
                If page IsNot Nothing Then
                    Return _mediaSizePages.Remove(page.PageType)
                End If
            End If
            Return False
        End Function

        Public Function Contains(pageMediaSize As PageMediaSizeName) As Boolean
            Return _mediaSizePages.ContainsKey(pageMediaSize)
        End Function

        Public Function Contains(pageName As String) As Boolean
            Return _unknownPages.ContainsKey(pageName) OrElse GetMsPageByName(pageName) IsNot Nothing
        End Function

        Protected Function GetKeyForItem(item As PageEx) As PageMediaSizeName
            Return item.PageType
        End Function

        Public Function GetByTypeOrDefault(pageType As PageMediaSizeName, Optional [default] As PageMediaSizeName = PageMediaSizeName.ISOA4) As PageEx
            If Me.Contains(pageType) Then
                Return Me(pageType)
            ElseIf Me.Contains([default]) Then
                Return Me([default])
            Else
                Return Me.FirstOrDefault
            End If
        End Function

        Public Function GetEnumerator() As IEnumerator(Of PageEx) Implements IEnumerable(Of PageEx).GetEnumerator
            Return _mediaSizePages.Values.Concat(_unknownPages.Values).GetEnumerator
        End Function

        Private Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
            Return GetEnumerator()
        End Function
    End Class

End Namespace