Imports System.Reflection
Imports System.Runtime.CompilerServices

Namespace Compare.Table

    Public Class GridTableDiffer

        Public Sub New(gridTable1 As GridTable, gridTable2 As GridTable)
            Me.GridTable1 = gridTable1
            Me.GridTable2 = gridTable2
        End Sub

        Public Property GridTable1 As GridTable
        Public Property GridTable2 As GridTable

        Public Function DiffProperties(keyPropertyName As String, properties() As String, recursive As Boolean) As DifferencesCollection
            If Me.GridTable1 IsNot Nothing OrElse Me.GridTable2 IsNot Nothing Then
                Dim startTable As GridTable = If(Me.GridTable1 IsNot Nothing AndAlso GridTable1.GridColumns.Count >= GridTable2.GridColumns.Count, Me.GridTable1, Me.GridTable2)
                Dim startIs1 As Boolean = startTable Is GridTable1

                Dim propsHashSet As New HashSet(Of String)(properties)
                Dim result As New DifferencesCollection(Me.GridTable1, Me.GridTable2)

                If recursive AndAlso ((Me.GridTable1 IsNot Nothing AndAlso GridTable1.GridSubTable IsNot Nothing) AndAlso (Me.GridTable2 IsNot Nothing AndAlso GridTable2.GridSubTable IsNot Nothing)) Then
                    result.SubDifferences = Me.GridTable1.GridSubTable.DiffWith(Me.GridTable2.GridSubTable, properties)
                End If

                For Each col As GridColumn In startTable.GridColumns
                    Dim colKeyPropValue As String = CStr(col.GetType.GetProperty(keyPropertyName).GetValue(col))
                    Dim contains1 As Boolean = Me.GridTable1 IsNot Nothing AndAlso Me.GridTable1.GridColumns.Contains(colKeyPropValue)
                    Dim contains2 As Boolean = Me.GridTable2 IsNot Nothing AndAlso Me.GridTable2.GridColumns.Contains(colKeyPropValue)
                    If contains1 AndAlso contains2 Then
                        Dim props As PropertyInfo() = col.GetType.GetProperties().Where(Function(pi) propsHashSet.Contains(pi.Name)).ToArray
                        For Each p As PropertyInfo In props
                            Dim pv1 As Object = If(startIs1, p.GetValue(col), p.GetValue(Me.GridTable2.GridColumns(colKeyPropValue)))
                            Dim pv2 As Object = If(startIs1, p.GetValue(Me.GridTable2.GridColumns(colKeyPropValue)), p.GetValue(col))
                            result.AddNewPropertyDiff(colKeyPropValue, p.Name, pv1, pv2)
                        Next
                    Else
                        Dim newDiffResult As New ColumnDifference(colKeyPropValue) With {.Table1 = If(contains1, Me.GridTable1, Nothing), .Table2 = If(contains2, Me.GridTable2, Nothing)}
                        result.Add(newDiffResult)
                    End If
                Next

                Return result
            End If
            Return Nothing
        End Function

        Public Function DiffProperties(keyPropertyName As String, ParamArray properties() As String) As DifferencesCollection
            Return DiffProperties(keyPropertyName, properties, True)
        End Function

        Public Class DifferencesCollection
            Inherits System.Collections.ObjectModel.KeyedCollection(Of String, ColumnDifference)

            Private _table1 As GridTable
            Private _table2 As GridTable

            Private _different As New Dictionary(Of String, ColumnDifference)

            Public Sub New(table1 As GridTable, table2 As GridTable)
                _table1 = table1
                _table2 = table2
            End Sub

            Public Function AddNewPropertyDiff(columnKey As String, propertyName As String, value1 As Object, value2 As Object) As ColumnDifference
                If Not Me.Contains(columnKey) Then
                    Me.Add(New ColumnDifference(columnKey, _table1, _table2))
                End If

                Dim diffCol As ColumnDifference = Me(columnKey)
                diffCol.AddNewPropertyResult(propertyName, value1, value2)
                UpdateColDifferences(diffCol)
                Return diffCol
            End Function

            Protected Overrides Function GetKeyForItem(item As ColumnDifference) As String
                Return item.Key
            End Function

            Public ReadOnly Property AnyDifferentOrTableMiss As Boolean
                Get
                    Return _different.Count > 0 OrElse (SubDifferences IsNot Nothing AndAlso SubDifferences.AnyDifferentOrTableMiss)
                End Get
            End Property

            Protected Overrides Sub InsertItem(index As Integer, item As ColumnDifference)
                MyBase.InsertItem(index, item)
                UpdateColDifferences(item)
            End Sub

            Private Sub UpdateColDifferences(item As ColumnDifference)
                If item.IsAnyPropDiffOrTableMiss AndAlso Not _different.ContainsKey(item.Key) Then
                    _different.Add(item.Key, item)
                End If
            End Sub

            Protected Overrides Sub ClearItems()
                MyBase.ClearItems()
                _different.Clear()
            End Sub

            Protected Overrides Sub SetItem(index As Integer, item As ColumnDifference)
                Throw New NotSupportedException("SetItem not supported!")
            End Sub

            Protected Overrides Sub RemoveItem(index As Integer)
                Throw New NotSupportedException("Removing items not supported!")
            End Sub

            Public ReadOnly Property ColumnDifferences As ColumnDifference()
                Get
                    Return _different.Values.ToArray
                End Get
            End Property

            Public Property SubDifferences As DifferencesCollection

            Public Overrides Function ToString() As String
                Return String.Format("Columns = {0}, Differences = {1}, SubDiff = {2}", Me.Count, Me.ColumnDifferences.Length, If(SubDifferences IsNot Nothing, Me.SubDifferences.Count.ToString, "<Empty>"))
            End Function

            Public ReadOnly Property TableType As KblObjectType
                Get
                    If _table1 IsNot Nothing OrElse _table2 IsNot Nothing Then
                        If _table1.Type = KblObjectType.Undefined AndAlso _table2.Type <> KblObjectType.Undefined AndAlso (_table1?.Type = _table2?.Type) Then
                            Return _table1.Type
                        ElseIf _table1 IsNot Nothing Then
                            Return _table1.Type
                        Else
                            Return _table2.Type
                        End If
                    Else
                        Return KblObjectType.Undefined
                    End If
                End Get
            End Property

            Public ReadOnly Property GridTable1 As GridTable
                Get
                    Return _table1
                End Get
            End Property

            Public ReadOnly Property GridTable2 As GridTable
                Get
                    Return _table2
                End Get
            End Property

        End Class

        Public Class ColumnDifference
            Implements IEnumerable(Of PropertyDifference)

            Private _dic As New Dictionary(Of String, PropertyDifference)
            Private _diffPropNames As New List(Of String)

            Public Sub New(key As String, table1 As GridTable, table2 As GridTable)
                Me.New(key)
                Me.Table1 = table1
                Me.Table2 = table2
            End Sub

            Public Sub New(key As String)
                Me.Key = key
            End Sub

            Public Property Table1 As GridTable
            Public Property Table2 As GridTable

            Public Property Key As String

            Public ReadOnly Property IsAnyPropDifferent As Boolean
                Get
                    Return _diffPropNames.Count > 0
                End Get
            End Property

            Public ReadOnly Property IsAnyPropDiffOrTableMiss As Boolean
                Get
                    Return IsAnyPropDifferent OrElse Table1 Is Nothing OrElse Table2 Is Nothing
                End Get
            End Property

            Public Sub AddNewPropertyResult(propertyName As String, value1 As Object, value2 As Object)
                Dim newRes As New PropertyDifference(propertyName, value1, value2)
                _dic.Add(propertyName, newRes)
                If Not newRes.IsEqual Then
                    _diffPropNames.Add(propertyName)
                End If
            End Sub

            Public Function GetEnumerator() As IEnumerator(Of PropertyDifference) Implements IEnumerable(Of PropertyDifference).GetEnumerator
                Return _dic.Values.GetEnumerator
            End Function

            Private Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
                Return _dic.Values.GetEnumerator
            End Function

            Public ReadOnly Property DifferentProps As IEnumerable(Of PropertyDifference)
                Get
                    Return _diffPropNames.Select(Function(pName) _dic(pName))
                End Get
            End Property

            Public ReadOnly Property Count As Integer
                Get
                    Return _dic.Count
                End Get
            End Property

            Public Function GetColumn1Or2PropValue(propertyName As String) As Object
                If Me.Table1 IsNot Nothing Then
                    Return Me.Table1.GridColumns(Me.Key).GetType.GetProperty(propertyName).GetValue(Me.Table1.GridColumns(Me.Key))
                ElseIf Me.Table2 IsNot Nothing Then
                    Return Me.Table2.GridColumns(Me.Key).GetType.GetProperty(propertyName).GetValue(Me.Table2.GridColumns(Me.Key))
                Else
                    Throw New ArgumentException("No tables available to get column property-data!")
                End If
            End Function

            Public Overrides Function ToString() As String
                Return String.Format("[{0}], Properties = {1}, Different = {2}", Me.Key, Me.Count, Me._diffPropNames.Count)
            End Function

        End Class

        Public Class PropertyDifference

            Public Sub New(propertyName As String, value1 As Object, value2 As Object)
                Me.PropertyName = propertyName
                Me.Value1 = value1
                Me.Value2 = value2
                Me.IsEqual = GetIsEqual()
            End Sub

            Public ReadOnly Property PropertyName As String
            Public ReadOnly Property Value1 As Object
            Public ReadOnly Property Value2 As Object
            Public ReadOnly Property IsEqual As Boolean

            Private Function GetIsEqual() As Boolean
                Return (Value1 Is Nothing AndAlso Value2 Is Nothing) OrElse (Value1 IsNot Nothing AndAlso Value1.Equals(Value2)) OrElse (Value2 IsNot Nothing AndAlso Value2.Equals(Value1))
            End Function

            Public Overrides Function ToString() As String
                Return String.Format("[{0}], Different: {1}", Me.PropertyName, Not Me.IsEqual)
            End Function

        End Class

    End Class

    <HideModuleName>
    Friend Module ColumnDifferExtensions

        <Extension>
        Public Function DiffWith(table1 As GridTable, table2 As GridTable, ParamArray diffProperties As String()) As GridTableDiffer.DifferencesCollection
            Return (New GridTableDiffer(table1, table2)).DiffProperties("KBLPropertyName", diffProperties)
        End Function

    End Module

End Namespace