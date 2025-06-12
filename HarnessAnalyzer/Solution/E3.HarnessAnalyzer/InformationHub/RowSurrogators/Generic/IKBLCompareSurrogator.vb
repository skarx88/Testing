Imports Infragistics.Win.UltraWinDataSource
Imports Infragistics.Win.UltraWinGrid

Public Interface IKBLCompareSurrogator
    Inherits IDisposable

    ReadOnly Property KblReference As IKblContainer
    ReadOnly Property KBLCompare As IKblContainer
    ReadOnly Property DataSource As UltraDataSource
    ReadOnly Property IsReference As Boolean

    Function GetCellValue(row As UltraDataRow, column As UltraDataColumn) As Object

End Interface
