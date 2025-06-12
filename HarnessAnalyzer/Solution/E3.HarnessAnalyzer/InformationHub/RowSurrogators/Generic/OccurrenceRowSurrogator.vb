Imports Infragistics.Win.UltraWinDataSource
Imports Infragistics.Win.UltraWinGrid

Public Class OccurrenceRowSurrogator(Of TOccurrence As {IKblOccurrence, New}, TOccurrencePart As {IKblPartObject, New})
    Inherits KblOccurrenceSurrogator(Of TOccurrence, TOccurrencePart)

    Private _row As UltraDataRow

    Public Sub New(kbl As IKblContainer)
        MyBase.New(kbl)
    End Sub

    Public ReadOnly Property IsInitialized As Boolean = False

    Public Function InitializeRow(row As UltraDataRow) As Boolean
        If _row Is Nothing OrElse row.DataSource IsNot _row.DataSource OrElse _row.Index <> row.Index Then
            _IsInitialized = False

            If TypeOf row.Tag Is String Then
                Me.SystemId = CType(row.Tag, String)
                If Me.Kbl.GetOccurrenceObject(Of TOccurrence)(Me.SystemId) IsNot Nothing Then
                    _row = row
                    _IsInitialized = True
                    Return True
                End If
            ElseIf TypeOf row.Tag Is TOccurrence Then
                Me.SystemId = CType(row.Tag, TOccurrence).SystemId
                _row = row
                _IsInitialized = True
                Return True
            End If
        End If
        Return False
    End Function

    ReadOnly Property RowIndex As Integer
        Get
            If _row IsNot Nothing Then
                Return _row.Index
            End If
            Return -1
        End Get
    End Property

End Class
