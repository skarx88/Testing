Imports System.Collections.ObjectModel
Imports Infragistics.Win.UltraWinDataSource
Imports Infragistics.Win.UltraWinGrid

Friend Class KblRowSurrogatorsCollection
    Inherits KeyedCollection(Of UltraDataSource, IKBLCompareSurrogator)
    Implements IDisposable

    Private _disposedValue As Boolean
    Private _kbl_reference As IKblContainer
    Private _kbl_compare As IKblContainer
    Private _mainForm As MainForm

    Public Sub New(kbl_reference As IKblContainer, kbl_compare As IKblContainer, mainForm As MainForm)
        MyBase.New
        _kbl_reference = kbl_reference
        _kbl_compare = kbl_compare
        _mainForm = mainForm
    End Sub

    Public Function AddNew(Of TSurrogator As IKBLCompareSurrogator)(dataSource As UltraDataSource) As TSurrogator
        Dim surrogator As TSurrogator = CType(Activator.CreateInstance(GetType(TSurrogator), {_kbl_reference, _kbl_compare, dataSource, _mainForm}), TSurrogator)
        Return surrogator
    End Function

    Public Function GetCellValue(datasource As UltraDataSource, row As UltraDataRow, column As UltraDataColumn) As Object
        If Me.Contains(datasource) Then
            Dim surrogator As IKBLCompareSurrogator = Me.Item(datasource)
            Return surrogator.GetCellValue(row, column)
        End If
        Throw New KeyNotFoundException($"Can't resolve cell value: the corresponding surrogator for the DataSource ""{datasource.Band.Key} ({datasource.GetType.Name})"" is not part of this collection!")
    End Function

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not _disposedValue Then
            If disposing Then
                For Each surrogator As IKBLCompareSurrogator In Me.OfType(Of IKBLCompareSurrogator).ToArray
                    surrogator.Dispose()
                Next
                Me.Clear()
            End If

            _mainForm = Nothing
            _kbl_reference = Nothing
            _kbl_compare = Nothing
            _disposedValue = True
        End If
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        ' Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in der Methode "Dispose(disposing As Boolean)" ein.
        Dispose(disposing:=True)
        GC.SuppressFinalize(Me)
    End Sub

    Protected Overrides Function GetKeyForItem(item As IKBLCompareSurrogator) As UltraDataSource
        Return item.DataSource
    End Function

End Class
