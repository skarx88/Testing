Imports System
Imports System.Collections
Imports System.Collections.Generic

Namespace Global.System.Windows.Forms
    ' Cyotek ImageBox
    ' Copyright (c) 2010-2015 Cyotek Ltd.
    ' http://cyotek.com/blog/tag/imagebox
    ' Licensed under the MIT License.
    ''' <summary>
    ''' Represents available levels of zoom in an <see cref="ImageBox"/> control
    ''' </summary>
    Friend Class ZoomLevelCollection
        Implements IList(Of Integer)
#Region "Friend Constructors"

        ''' <summary>
        ''' Initializes a new instance of the <see cref="ZoomLevelCollection"/> class.
        ''' </summary>
        Friend Sub New()
            List = New SortedList(Of Integer, Integer)()
        End Sub

        ''' <summary>
        ''' Initializes a new instance of the <see cref="ZoomLevelCollection"/> class.
        ''' </summary>
        ''' <param name="collection">The default values to populate the collection with.</param>
        ''' <exception cref="System.ArgumentNullException">Thrown if the <c>collection</c> parameter is null</exception>
        Friend Sub New(collection As IEnumerable(Of Integer))
            Me.New()
            If collection Is Nothing Then
                Throw New ArgumentNullException(NameOf(collection))
            End If

            AddRange(collection)
        End Sub

#End Region

#Region "Friend Class Properties"

        ''' <summary>
        ''' Returns the default zoom levels
        ''' </summary>
        Friend Shared ReadOnly Property [Default] As ZoomLevelCollection
            Get
                Return New ZoomLevelCollection({7, 10, 15, 20, 25, 30, 50, 70, 100, 150, 200, 300, 400, 500, 600, 700, 800, 1200, 1600})
            End Get
        End Property

#End Region

#Region "Friend Properties"

        ''' <summary>
        ''' Gets the number of elements contained in the <see cref="ZoomLevelCollection"/>.
        ''' </summary>
        ''' <returns>
        ''' The number of elements contained in the <see cref="ZoomLevelCollection"/>.
        ''' </returns>
        Friend ReadOnly Property Count As Integer Implements ICollection(Of Integer).Count
            Get
                Return List.Count
            End Get
        End Property

        ''' <summary>
        ''' Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        ''' </summary>
        ''' <value><c>true</c> if this instance is read only; otherwise, <c>false</c>.</value>
        ''' <returns>true if the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only; otherwise, false.
        ''' </returns>
        Friend ReadOnly Property IsReadOnly As Boolean Implements ICollection(Of Integer).IsReadOnly
            Get
                Return False
            End Get
        End Property

        ''' <summary>
        ''' Gets or sets the zoom level at the specified index.
        ''' </summary>
        ''' <param name="index">The index.</param>
        Default Public Property Item(index As Integer) As Integer Implements IList(Of Integer).Item
            Get
                Return List.Values(index)
            End Get
            Set(value As Integer)
                List.RemoveAt(index)
                Add(value)
            End Set
        End Property

#End Region

#Region "Protected Properties"

        ''' <summary>
        ''' Gets or sets the backing list.
        ''' </summary>
        Protected Property List As SortedList(Of Integer, Integer)

#End Region

#Region "Friend Members"

        ''' <summary>
        ''' Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        ''' </summary>
        ''' <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        Friend Sub Add(item As Integer) Implements ICollection(Of Integer).Add
            List.Add(item, item)
        End Sub

        ''' <summary>
        ''' Adds a range of items to the <see cref="ZoomLevelCollection"/>.
        ''' </summary>
        ''' <param name="collection">The items to add to the collection.</param>
        ''' <exception cref="System.ArgumentNullException">Thrown if the <c>collection</c> parameter is null.</exception>
        Friend Sub AddRange(collection As IEnumerable(Of Integer))
            If collection Is Nothing Then
                Throw New ArgumentNullException(NameOf(collection))
            End If

            For Each value In collection
                Add(value)
            Next
        End Sub

        ''' <summary>
        ''' Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        ''' </summary>
        Friend Sub Clear() Implements ICollection(Of Integer).Clear
            List.Clear()
        End Sub

        ''' <summary>
        ''' Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"/> contains a specific value.
        ''' </summary>
        ''' <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        ''' <returns>true if <paramref name="item"/> is found in the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false.</returns>
        Friend Function Contains(item As Integer) As Boolean Implements ICollection(Of Integer).Contains
            Return List.ContainsKey(item)
        End Function

        ''' <summary>
        ''' Copies a range of elements this collection into a destination <see cref="Array"/>.
        ''' </summary>
        ''' <param name="array">The <see cref="Array"/> that receives the data.</param>
        ''' <param name="arrayIndex">A 64-bit integer that represents the index in the <see cref="Array"/> at which storing begins.</param>
        Friend Sub CopyTo(array As Integer(), arrayIndex As Integer) Implements ICollection(Of Integer).CopyTo
            For i = 0 To Count - 1
                array(arrayIndex + i) = List.Values(i)
            Next
        End Sub

        ''' <summary>
        ''' Finds the index of a zoom level matching or nearest to the specified value.
        ''' </summary>
        ''' <param name="zoomLevel">The zoom level.</param>
        Friend Function FindNearest(zoomLevel As Integer) As Integer
            Dim nearestValue = List.Values(0)
            Dim nearestDifference = Math.Abs(nearestValue - zoomLevel)
            For i = 1 To Count - 1
                Dim value = List.Values(i)
                Dim difference = Math.Abs(value - zoomLevel)
                If difference < nearestDifference Then
                    nearestValue = value
                    nearestDifference = difference
                End If
            Next
            Return nearestValue
        End Function

        ''' <summary>
        ''' Returns an enumerator that iterates through the collection.
        ''' </summary>
        ''' <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.</returns>
        Friend Function GetEnumerator() As IEnumerator(Of Integer) Implements IEnumerable(Of Integer).GetEnumerator
            Return List.Values.GetEnumerator()
        End Function

        ''' <summary>
        ''' Determines the index of a specific item in the <see cref="T:System.Collections.Generic.IList`1"/>.
        ''' </summary>
        ''' <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.IList`1"/>.</param>
        ''' <returns>The index of <paramref name="item"/> if found in the list; otherwise, -1.</returns>
        Friend Function IndexOf(item As Integer) As Integer Implements IList(Of Integer).IndexOf
            Return List.IndexOfKey(item)
        End Function

        ''' <summary>
        ''' Not implemented.
        ''' </summary>
        ''' <param name="index">The index.</param>
        ''' <param name="item">The item.</param>
        ''' <exception cref="System.NotImplementedException">Not implemented</exception>
        Friend Sub Insert(index As Integer, item As Integer) Implements IList(Of Integer).Insert
            Throw New NotImplementedException()
        End Sub

        ''' <summary>
        ''' Returns the next increased zoom level for the given current zoom.
        ''' </summary>
        ''' <param name="zoomLevel">The current zoom level.</param>
        ''' <returns>The next matching increased zoom level for the given current zoom if applicable, otherwise the nearest zoom.</returns>
        Friend Function NextZoom(zoomLevel As Integer) As Integer
            Dim index As Integer

            index = IndexOf(FindNearest(zoomLevel))
            If index < Count - 1 Then
                index += 1
            End If

            Return Me(index)
        End Function

        ''' <summary>
        ''' Returns the next decreased zoom level for the given current zoom.
        ''' </summary>
        ''' <param name="zoomLevel">The current zoom level.</param>
        ''' <returns>The next matching decreased zoom level for the given current zoom if applicable, otherwise the nearest zoom.</returns>
        Friend Function PreviousZoom(zoomLevel As Integer) As Integer
            Dim index As Integer

            index = IndexOf(FindNearest(zoomLevel))
            If index > 0 Then
                index -= 1
            End If

            Return Me(index)
        End Function

        ''' <summary>
        ''' Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        ''' </summary>
        ''' <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        ''' <returns>true if <paramref name="item"/> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false. This method also returns false if <paramref name="item"/> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"/>.</returns>
        Friend Function Remove(item As Integer) As Boolean Implements ICollection(Of Integer).Remove
            Return List.Remove(item)
        End Function

        ''' <summary>
        ''' Removes the element at the specified index of the <see cref="ZoomLevelCollection"/>.
        ''' </summary>
        ''' <param name="index">The zero-based index of the element to remove.</param>
        Friend Sub RemoveAt(index As Integer) Implements IList(Of Integer).RemoveAt
            List.RemoveAt(index)
        End Sub

        ''' <summary>
        ''' Copies the elements of the <see cref="ZoomLevelCollection"/> to a new array.
        ''' </summary>
        ''' <returns>An array containing copies of the elements of the <see cref="ZoomLevelCollection"/>.</returns>
        Friend Function ToArray() As Integer()
            Dim results As Integer()

            results = New Integer(Count - 1) {}
            CopyTo(results, 0)

            Return results
        End Function

#End Region

#Region "IList<int> Members"

        ''' <summary>
        ''' Returns an enumerator that iterates through a collection.
        ''' </summary>
        ''' <returns>An <see cref="ZoomLevelCollection"/> object that can be used to iterate through the collection.</returns>
        Private Function GetEnumerator1() As IEnumerator Implements IEnumerable.GetEnumerator
            Return GetEnumerator()
        End Function

#End Region
    End Class
End Namespace
