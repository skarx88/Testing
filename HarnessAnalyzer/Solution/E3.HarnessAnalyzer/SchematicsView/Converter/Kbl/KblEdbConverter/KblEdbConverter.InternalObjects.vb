Imports System.Collections.ObjectModel

Namespace Schematics.Converter.Kbl

    Partial Public Class KblEdbConverter

        Public Class InitializeError

            Private _object As Object
            Private _documentId As String
            Private _type As Type

            Public Sub New(type As Type, documentId As String, [object] As Object)
                _documentId = documentId
                _object = [object]
                _type = type
            End Sub

            Public Enum Type
                Unknown = 0
                AddErrorConnectorAtCavityMapper = 1
                AddErrorCavityAtWireMapper = 2
                AddErrorWireAtCavityMapper = 3
                AddErrorPartMapper = 4
                AddErrorUnitsMapper = 5
                AddErrorWireConenctions = 6
                AddErrorObjectAtModuleMapper = 7
                AddErrorWireAtWireGroupMapper = 8
            End Enum

            ReadOnly Property ErrorType As Type
                Get
                    Return _type
                End Get
            End Property

            ReadOnly Property DocumentId As String
                Get
                    Return _documentId
                End Get
            End Property

            ReadOnly Property [Object] As Object
                Get
                    Return _object
                End Get
            End Property

        End Class

        Public Class InitializeErrorEventArgs
            Inherits EventArgs

            Private _object As Object
            Private _documentId As String = Nothing
            Private _type As ErrorType = ErrorType.Unknown
            Private _message As String

            Public Sub New(type As ErrorType, documentId As String, obj As Object, message As String)
                _object = obj
                _documentId = documentId
                _type = type
                _message = message
            End Sub

            ReadOnly Property Message As String
                Get
                    Return _message
                End Get
            End Property

            ReadOnly Property [Object] As Object
                Get
                    Return _object
                End Get
            End Property

            ReadOnly Property DocumentId As String
                Get
                    Return _documentId
                End Get
            End Property

            ReadOnly Property Type As ErrorType
                Get
                    Return _type
                End Get
            End Property

            Public Enum ErrorType
                Unknown = 0
                EntityAlreadyExists = 1
            End Enum

        End Class

        Private Class ComponentResolveResult

            Private _connector As ConnectorInfo

            Public Sub New(connector As ConnectorInfo)
                _connector = connector
            End Sub

            ReadOnly Property Connector As ConnectorInfo
                Get
                    Return _connector
                End Get
            End Property

        End Class

        Private Class ConnectorInfoMapperCollection
            Inherits System.Collections.ObjectModel.KeyedCollection(Of String, ConnectorInfo)
            Implements IDisposable

            Private _disposedValue As Boolean
            Private _byComponentShortName As New Dictionary(Of String, List(Of String))

            Protected Overrides Function GetKeyForItem(item As ConnectorInfo) As String
                Return item.Id
            End Function

            Protected Overrides Sub ClearItems()
                _byComponentShortName.Clear()
                MyBase.ClearItems()
            End Sub

            Protected Overrides Sub InsertItem(index As Integer, item As ConnectorInfo)
                AddMapping(item)
                MyBase.InsertItem(index, item)
            End Sub

            Protected Overrides Sub RemoveItem(index As Integer)
                Dim item As ConnectorInfo = Me.Items(index)
                RemoveMapping(item)
                MyBase.RemoveItem(index)
            End Sub

            Protected Overrides Sub SetItem(index As Integer, item As ConnectorInfo)
                Dim oldItem As ConnectorInfo = Me.Items(index)
                RemoveMapping(oldItem)
                MyBase.SetItem(index, item)
                AddMapping(item)
            End Sub

            Private Sub RemoveMapping(item As ConnectorInfo)
                _byComponentShortName.TryRemove(GetComponentShortName(item), GetKeyForItem(item))
            End Sub

            Private Sub AddMapping(item As ConnectorInfo)
                _byComponentShortName.AddOrUpdate(GetComponentShortName(item), GetKeyForItem(item))
            End Sub

            Private Function GetComponentShortName(cmp As ConnectorInfo) As String
                Return cmp.Component.ShortName.ToUpper
            End Function

            Public Function TryAdd(item As ConnectorInfo) As Boolean
                If Not Me.Contains(GetKeyForItem(item)) Then
                    Me.Add(item)
                    Return True
                End If
                Return False
            End Function

            Public Function GetConnectors(componentShortName As String) As List(Of ConnectorInfo)
                Dim list As New List(Of ConnectorInfo)
                Dim connectorIds As List(Of String) = Nothing
                If _byComponentShortName.TryGetValue(componentShortName.ToUpper, connectorIds) Then
                    For Each connId As String In connectorIds
                        list.Add(Me(connId))
                    Next
                End If
                Return list
            End Function

            Public Function GetComponents(componetShortName As String) As List(Of ComponentInfo)
                Dim conns As List(Of ConnectorInfo) = GetConnectors(componetShortName)
                Return conns.Select(Function(conn) conn.Component).Distinct.ToList
            End Function

            Public Shadows Function TryGetValue(key As String, ByRef item As ConnectorInfo) As Boolean
                If Me.Dictionary IsNot Nothing Then
                    Return Me.Dictionary.TryGetValue(key, item)
                End If
                Return False
            End Function

            Protected Overridable Sub Dispose(disposing As Boolean)
                If Not _disposedValue Then
                    If disposing Then
                        _byComponentShortName = Nothing
                    End If
                End If
                _disposedValue = True
            End Sub

            ' Dieser Code wird von Visual Basic hinzugefügt, um das Dispose-Muster richtig zu implementieren.
            Public Sub Dispose() Implements IDisposable.Dispose
                ' Ändern Sie diesen Code nicht. Fügen Sie oben in Dispose(disposing As Boolean) Bereinigungscode ein.
                Dispose(True)
                GC.SuppressFinalize(Me)
            End Sub

        End Class

        Friend Class GroupedCastableObjectsCollection(Of T)
            Inherits GroupedObjectsCollection(Of T)

            Friend Sub New(documents As IEnumerable(Of KblDocumentData), mapperSelector As Func(Of KBLMapper, IEnumerable(Of KeyValuePair(Of String, T))), Optional cancelToken As Nullable(Of System.Threading.CancellationToken) = Nothing)
                MyBase.New(documents, mapperSelector, Nothing, cancelToken)
            End Sub

            Public Overloads Function TryGetObject(Of T2 As T)(objectId As String, documentId As String, ByRef resultObject As T2) As Boolean
                Dim resultTemp As T = Nothing
                Dim success As Boolean = MyBase.TryGetDocumentObject(objectId, documentId, resultTemp)
                resultObject = CType(resultTemp, T2)
                Return success
            End Function

            Public Overloads Function TryGetObject(Of T2 As T)(objectId As String, documentId As String) As T2
                Dim result As T2 = Nothing
                TryGetObject(objectId, documentId, result)
                Return result
            End Function

        End Class

        Friend Class GroupedObjectsCollection(Of T)
            Inherits KeyedCollection(Of String, ObjectGroup(Of T))

            Friend Sub New(kblDocuments As IEnumerable(Of KblDocumentData), mapperSelector As Func(Of KBLMapper, IEnumerable(Of KeyValuePair(Of String, T))), Optional errorAdd As Action(Of String, T) = Nothing, Optional cancelToken As Nullable(Of System.Threading.CancellationToken) = Nothing)
                If Not (cancelToken?.IsCancellationRequested).GetValueOrDefault Then
                    For Each documentData As KblDocumentData In kblDocuments
                        For Each occKv As KeyValuePair(Of String, T) In mapperSelector.Invoke(documentData.Kbl)
                            If cancelToken?.IsCancellationRequested Then
                                Return
                            End If

                            Dim occGroup As ObjectGroup(Of T) = New ObjectGroup(Of T)(occKv.Key)
                            If Me.Contains(occKv.Key) Then
                                occGroup = Me(occKv.Key)
                            Else
                                Me.Add(occGroup)
                            End If

                            If Not occGroup.TryAdd(documentData.DocumentId, CType(occKv.Value, T)) Then
                                If errorAdd IsNot Nothing Then
                                    errorAdd.Invoke(documentData.DocumentId, occKv.Value)
                                Else
                                    Throw New ArgumentException(String.Format("key ""{0}"" already exists", documentData.DocumentId))
                                End If
                            End If
                        Next
                    Next
                End If
            End Sub

            Protected Overrides Function GetKeyForItem(item As ObjectGroup(Of T)) As String
                Return item.ObjectId
            End Function

            Public Function OfObjectType(Of T2 As T)() As IEnumerable(Of IObjectGroup)
                Dim results As New List(Of IObjectGroup)
                For Each grp As IObjectGroup In Me
                    If grp.Count > 0 Then
                        ' all objects are the same type in a group (group = objectId to documents mapper) -> no additional filtering is needed
                        If grp.All(Function(kv) TypeOf kv.Value Is T2) Then
                            results.Add(grp)
                        ElseIf grp.Any(Function(kv) TypeOf kv.Value Is T2) Then ' objects with the same ID can be the different type because id's in kbl over all documents are not globally unique -> additionally filtering by type in this group is needed to pass back only objects of the given argument type
                            Dim typeFilteredGroup As IObjectGroup = CType(grp, ObjectGroup(Of T)).CloneWithOnlyType(Of T2)
                            results.Add(typeFilteredGroup)
                        End If
                    End If
                Next
                Return results
            End Function

            Public Function TryGetDocumentObject(objectId As String, documentId As String, ByRef resultObject As T) As Boolean
                If Me.Contains(objectId) Then
                    Dim objGroup As ObjectGroup(Of T) = Me(objectId)
                    If objGroup.ContainsDocumentId(documentId) Then
                        resultObject = objGroup(documentId)
                        Return True
                    End If
                End If
                Return False
            End Function

            Public Function TryGetDocumentObject(objectId As String, documentId As String) As T
                Dim result As T = Nothing
                TryGetDocumentObject(objectId, documentId, result)
                Return result
            End Function

        End Class

        Friend Class ObjectGroup(Of T)
            Implements IEnumerable(Of KeyValuePair(Of String, T))
            Implements IObjectGroup

            Private _objectId As String
            Private _objectsByDocuments As New Dictionary(Of String, T)

            Public Sub New(objectId As String)
                _objectId = objectId
            End Sub

            ReadOnly Property ObjectId As String Implements IObjectGroup.ObjectId
                Get
                    Return _objectId
                End Get
            End Property

            ReadOnly Property documentIds As IEnumerable(Of String)
                Get
                    Return _objectsByDocuments.Keys
                End Get
            End Property

            Public Function TryAdd(documentId As String, [object] As T) As Boolean
                Return _objectsByDocuments.TryAdd(documentId, [object])
            End Function

            Public Sub Add(documentId As String, [object] As T)
                _objectsByDocuments.Add(documentId, [object])
            End Sub

            Public Function Remove(documentId As String) As Boolean
                Return _objectsByDocuments.Remove(documentId)
            End Function

            Public Function ContainsDocumentId(documentId As String) As Boolean Implements IObjectGroup.ContainsDocumentId
                Return _objectsByDocuments.ContainsKey(documentId)
            End Function

            Default ReadOnly Property Item(documentId As String) As T
                Get
                    Return _objectsByDocuments(documentId)
                End Get
            End Property

            Private ReadOnly Property IObjectGroup_Item(documentId As String) As Object Implements IObjectGroup.Item
                Get
                    Return Me.Item(documentId)
                End Get
            End Property

            ReadOnly Property Count As Integer Implements IObjectGroup.Count
                Get
                    Return _objectsByDocuments.Values.Count
                End Get
            End Property

            Public Function GetEnumerator() As IEnumerator(Of KeyValuePair(Of String, T)) Implements IEnumerable(Of KeyValuePair(Of String, T)).GetEnumerator
                Return _objectsByDocuments.GetEnumerator
            End Function

            Private Function GetEnumerator1() As IEnumerator Implements IEnumerable.GetEnumerator
                Return _objectsByDocuments.GetEnumerator
            End Function

            Private Function GetEnumerator2() As IEnumerator(Of DictionaryEntry) Implements IEnumerable(Of DictionaryEntry).GetEnumerator
                Return _objectsByDocuments.Select(Function(kv) New DictionaryEntry(kv.Key, kv.Value)).GetEnumerator
            End Function

            Public Overrides Function ToString() As String
                Return String.Format("{0}; Count: {1}", _objectId, _objectsByDocuments.Count)
            End Function

            Public Function CloneWithOnlyType(Of TValue As T)() As ObjectGroup(Of TValue)
                Dim cloneGroup As New ObjectGroup(Of TValue)(Me.ObjectId)
                For Each entry As KeyValuePair(Of String, T) In _objectsByDocuments
                    If TypeOf entry.Value Is TValue Then
                        cloneGroup.Add(entry.Key, CType(entry.Value, TValue))
                    End If
                Next
                Return cloneGroup
            End Function

        End Class

        Public Interface IObjectGroup
            Inherits IEnumerable(Of DictionaryEntry)

            Function ContainsDocumentId(documentId As String) As Boolean

            ReadOnly Property ObjectId As String
            Default ReadOnly Property Item(documentId As String) As Object
            ReadOnly Property Count As Integer

        End Interface

    End Class

End Namespace