Imports Infragistics.Win.UltraWinDataSource

Friend MustInherit Class KblTupleRowSurrogator(Of TOccurrence As {IKblOccurrence, New}, TOccurrencePart As {IKblPartObject, New})
    Inherits DisposableObject

    Private WithEvents _dataSource As UltraDataSource
    Private _isChildRowSurrogator As Boolean

    Public Sub New(kbl1 As IKblContainer, kbl2 As IKblContainer, corresponding_DS As UltraDataSource, isChildRowSurrogator As Boolean)
        If KblOccurrenceRow1 Is Nothing AndAlso kbl1 IsNot Nothing Then
            _KblOccurrenceRow1 = CreateObjectSurrogator(kbl1)
        End If

        If KblOccurrenceRow2 Is Nothing AndAlso kbl2 IsNot Nothing Then
            _KblOccurrenceRow2 = CreateObjectSurrogator(kbl2)
        End If

        _isChildRowSurrogator = isChildRowSurrogator
        _dataSource = corresponding_DS
    End Sub

    Protected ReadOnly Property DataSource As UltraDataSource
        Get
            Return _dataSource
        End Get
    End Property

    Protected Overridable Function CreateObjectSurrogator(kbl As IKblContainer) As OccurrenceRowSurrogator(Of TOccurrence, TOccurrencePart)
        Return CType(Activator.CreateInstance(GetType(OccurrenceRowSurrogator(Of TOccurrence, TOccurrencePart)), {kbl}), OccurrenceRowSurrogator(Of TOccurrence, TOccurrencePart))
    End Function

    Public Function GetCellValue(row As UltraDataRow, column As UltraDataColumn) As Object
        If Me.KblOccurrenceRow1?.InitializeRow(row) Or Me.KblOccurrenceRow2?.InitializeRow(row) Then
            OnAfterOccurrenceRowInitialized(row)
        End If

        If Me.KblOccurrenceRow1?.IsInitialized OrElse Me.KblOccurrenceRow2?.IsInitialized Then
            Return GetCellValueCore(row, column)
        End If
        Return Nothing
    End Function

    Public Overridable Function CanGetCellValue(row As UltraDataRow, column As UltraDataColumn) As Boolean
        If (_isChildRowSurrogator AndAlso row.ParentRow IsNot Nothing) OrElse (Not _isChildRowSurrogator AndAlso row.ParentRow Is Nothing) Then
            If TypeOf row.Tag Is TOccurrence OrElse TypeOf row.Tag Is String Then
                Return True
            End If
        End If
        Return False
    End Function

    Public Property OnUnhandledCellDataRequested As Func(Of UltraDataRow, UltraDataColumn, Object)

    ReadOnly Property KblOccurrenceRow1 As OccurrenceRowSurrogator(Of TOccurrence, TOccurrencePart)
    ReadOnly Property KblOccurrenceRow2 As OccurrenceRowSurrogator(Of TOccurrence, TOccurrencePart)

    Protected MustOverride Sub OnAfterOccurrenceRowInitialized(row As UltraDataRow)
    Protected MustOverride Function GetCellValueCore(row As UltraDataRow, column As UltraDataColumn) As Object

    Private Sub _dataSource_CellDataRequested(sender As Object, e As CellDataRequestedEventArgs) Handles _dataSource.CellDataRequested
        e.CacheData = False
        If CanGetCellValue(e.Row, e.Column) Then
            Dim data As Object = Me.GetCellValue(e.Row, e.Column)
            If KblOccurrenceRow1?.IsInitialized OrElse KblOccurrenceRow2?.IsInitialized Then
                e.Data = data
            End If
        End If
    End Sub

End Class
