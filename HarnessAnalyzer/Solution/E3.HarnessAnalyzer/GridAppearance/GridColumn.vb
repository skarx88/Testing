Imports Zuken.E3.HarnessAnalyzer

<Serializable()>
Public Class GridColumn

    Public Sub New()
        Name = "Undefined"
        Ordinal = -1
        KBLPropertyName = "Unknown"
    End Sub

    Public Sub New(appearance As GridAppearance, ByVal columnName As String, ByVal columnPosition As Integer, searchable As Boolean, visible As Boolean, ByVal propertyName As String, comparable As Boolean, hideComparable As Boolean, hideSearchable As Boolean, d3d As D3DGridColumnSetting)
        Me.Name = columnName
        Me.KBLPropertyName = propertyName
        Me.Ordinal = columnPosition 'HINT If negativ this is a special use case to make this not editable (EndNode in 3D protection)
        Me.Searchable = searchable
        Me.Visible = visible
        Me.Comparable = comparable
        Me.D3D = d3d
        Me.HideComparable = hideComparable
        Me.HideSearchable = hideSearchable
        Me.Appearance = appearance
    End Sub

    Public Sub New(appearance As GridAppearance, ByVal columnName As String, ByVal columnPosition As Integer, searchable As Boolean, visible As Boolean, ByVal propertyName As String, comparable As Boolean, d3d As D3DGridColumnSetting)
        Me.Name = columnName
        Me.KBLPropertyName = propertyName
        Me.Ordinal = columnPosition
        Me.Searchable = searchable
        Me.Visible = visible
        Me.Comparable = comparable
        Me.D3D = d3d
        Me.Appearance = appearance
    End Sub

    Public Sub New(appearance As GridAppearance, ByVal columnName As String, ByVal columnPosition As Integer, searchable As Boolean, visible As Boolean, ByVal propertyName As String, comparable As Boolean)
        Me.New(appearance, columnName, columnPosition, searchable, visible, propertyName, comparable, New D3DGridColumnSetting(False))
    End Sub

    Public Sub New(appearance As GridAppearance, ByVal columnName As String, ByVal columnPosition As Integer, searchable As Boolean, visible As Boolean, ByVal propertyName As String)
        Me.New(appearance, columnName, columnPosition, searchable, visible, propertyName, True)
    End Sub

    Public Overrides Function ToString() As String
        Return String.Format("[{0}], Name = {1}, Visible = {2}, Comparable = {3}, D3D = ""{4}""", Me.KBLPropertyName, Me.Name, Me.Visible, Me.Comparable, Me.D3D.ToString)
    End Function

    Public Property Name As String              'TODO: is serialized as localized string: needs re-evaluting. Maybe this property can completely removed by resolving on runtime
    Public Property KBLPropertyName As String
    Public Property Ordinal As Integer 'HINT If negativ this is a special use case to make this not editable (EndNode in 3D protection)
    Public Property Searchable As Boolean
    Public Property D3D As New D3DGridColumnSetting
    Public Property Visible As Boolean
    Public Property Comparable As Boolean = True
    Public Property HideComparable As Boolean = False
    Public Property HideSearchable As Boolean = False
    Public Property Appearance As GridAppearance

End Class

Public Class GridColumnList
    Implements IEnumerable(Of GridColumn)

    Private _dic As New Dictionary(Of String, HashSet(Of GridColumn))
    Private _gridTable As GridTable

    Public Sub New(gridTable As GridTable)
        MyBase.New()
        _gridTable = gridTable
    End Sub

    Public Sub Add(gridColum As GridColumn)
        _dic.GetOrAddNew(gridColum.KBLPropertyName).Add(gridColum)
    End Sub

    Public Function AddNew(appearance As GridAppearance, ByVal columnName As String, ByVal columnPosition As Integer, searchable As Boolean, visible As Boolean, ByVal propertyName As String, comparable As Boolean, hideComparable As Boolean, hideSearchable As Boolean, d3d As D3DGridColumnSetting) As GridColumn
        Dim newCol As New GridColumn(appearance, columnName, columnPosition, searchable, visible, propertyName, comparable, hideComparable, hideSearchable, d3d)
        Me.Add(newCol)
        Return newCol
    End Function

    Public Function AddNew(appearance As GridAppearance, ByVal columnName As String, ByVal columnPosition As Integer, searchable As Boolean, visible As Boolean, ByVal propertyName As String, comparable As Boolean, d3d As D3DGridColumnSetting) As GridColumn
        Dim newCol As New GridColumn(appearance, columnName, columnPosition, searchable, visible, propertyName, comparable, d3d)
        Me.Add(newCol)
        Return newCol
    End Function

    Public Function AddNew(appearance As GridAppearance, ByVal columnName As String, ByVal columnPosition As Integer, searchable As Boolean, visible As Boolean, ByVal propertyName As String, comparable As Boolean) As GridColumn
        Return Me.AddNew(appearance, columnName, columnPosition, searchable, visible, propertyName, comparable, New D3DGridColumnSetting(False))
    End Function

    Public Function AddNew(appearance As GridAppearance, ByVal columnName As String, ByVal columnPosition As Integer, searchable As Boolean, visible As Boolean, ByVal propertyName As String) As GridColumn
        Return Me.AddNew(appearance, columnName, columnPosition, searchable, visible, propertyName, True)
    End Function

    Public Function Contains(gridColumn As GridColumn) As Boolean
        For Each gridCol As GridColumn In Me
            If (gridCol.Name.ToLower = gridColumn.Name.ToLower) Then
                Return True
            End If
        Next

        Return False
    End Function

    Public Function Contains(propertyName As String) As Boolean
        Return _dic.ContainsKey(propertyName)
    End Function

    Default ReadOnly Property Item(propertyName As String) As List(Of GridColumn)
        Get
            Return _dic(propertyName).ToList
        End Get
    End Property

    Public Function FindGridColumn(key As String) As List(Of GridColumn)
        Dim cols As New List(Of GridColumn)
        For Each gridCol As GridColumn In Me
            If (gridCol.KBLPropertyName.ToLower = key.ToLower) Then
                cols.Add(gridCol)
            End If
        Next

        Return cols
    End Function

    Public Function GetEnumerator() As IEnumerator(Of GridColumn) Implements IEnumerable(Of GridColumn).GetEnumerator
        Return _dic.SelectMany(Function(kv) kv.Value).GetEnumerator
    End Function


    Private Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
        Return GetEnumerator()
    End Function

    Public ReadOnly Property GridTable As GridTable
        Get
            Return _gridTable
        End Get
    End Property

End Class
