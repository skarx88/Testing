Public Interface ICollectionGrouping(Of TKey, TElement)
    Inherits IGrouping(Of TKey, TElement)
    Inherits ICollection(Of TElement)

    Shadows Function Add(element As TElement) As Boolean
    Shadows Function Remove(element As TElement) As Boolean

End Interface
