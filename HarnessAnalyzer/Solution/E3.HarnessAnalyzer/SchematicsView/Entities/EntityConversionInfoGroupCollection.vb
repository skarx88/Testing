Imports System.Collections.ObjectModel

Namespace Schematics.Converter

    Public Class EntityConversionInfoGroupCollection
        Inherits KeyedCollection(Of String, IGrouping(Of String, EdbConversionEntityInfo))

        Public Sub New()
            MyBase.New()
        End Sub

        Public Sub New(dic As Dictionary(Of String, IEnumerable(Of EdbConversionEntityInfo)))
            Me.New()

            For Each kv As KeyValuePair(Of String, IEnumerable(Of EdbConversionEntityInfo)) In dic
                Me.Add(New EntityInfoGroup(kv.Key, kv.Value))
            Next
        End Sub

        Protected Overrides Function GetKeyForItem(item As IGrouping(Of String, EdbConversionEntityInfo)) As String
            Return item.Key
        End Function

        Public Function TryGetGroup(blockId As String, ByRef group As IGrouping(Of String, EdbConversionEntityInfo)) As Boolean
            If Me.Dictionary IsNot Nothing Then
                Return Me.Dictionary.TryGetValue(blockId, group)
            End If
            Return False
        End Function

        Private Class EntityInfoGroup
            Implements IGrouping(Of String, EdbConversionEntityInfo)

            Private _list As New List(Of EdbConversionEntityInfo)
            Private _key As String

            Public Sub New(key As String, coll As IEnumerable(Of EdbConversionEntityInfo))
                _key = key
                _list.AddRange(coll)
            End Sub

            Public Function GetEnumerator() As IEnumerator(Of EdbConversionEntityInfo) Implements IEnumerable(Of EdbConversionEntityInfo).GetEnumerator
                Return _list.GetEnumerator
            End Function

            Private Function GetEnumerator1() As IEnumerator Implements IEnumerable.GetEnumerator
                Return GetEnumerator()
            End Function

            Public ReadOnly Property Key As String Implements IGrouping(Of String, EdbConversionEntityInfo).Key
                Get
                    Return _key
                End Get
            End Property
        End Class

    End Class

End Namespace
