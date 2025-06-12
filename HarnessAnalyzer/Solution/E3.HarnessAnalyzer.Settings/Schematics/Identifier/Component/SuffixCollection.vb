Imports System.Collections.ObjectModel

Namespace Schematics.Identifier.Component

    <Serializable>
    Public Class SuffixCollection
        Inherits System.Collections.ObjectModel.ObservableCollection(Of String)
        Implements IDisposable

        Private _group As IdentifierGroupBase

        Public Sub New(group As IdentifierGroupBase)
            Me.New(group, New String() {})
        End Sub

        Public Sub New(group As IdentifierGroupBase, suffixes As IEnumerable(Of String))
            MyBase.New(suffixes.ToList)
            _group = group
        End Sub

        Protected Overrides Sub InsertItem(index As Integer, item As String)
            If _group.IsInlinerType Then
                MyBase.InsertItem(index, item)
            Else
                Throw New NotSupportedException(String.Format("Suffixes are only supported for Inliner ComponentTypes !"))
            End If
        End Sub

        Public Overrides Function ToString() As String
            Return String.Join(";", Me)
        End Function

        Public Function GetMatches(shortName As String) As ReadOnlyCollection(Of SuffixMatch)
            Dim list As New List(Of SuffixMatch)
            For Each suffix As String In Me
                Dim idx As Integer = shortName.IndexOf(suffix)
                If idx <> -1 Then
                    list.Add(New SuffixMatch(idx, suffix))
                End If
            Next
            Return New ReadOnlyCollection(Of SuffixMatch)(list)
        End Function

        Public Sub AddRange(items As IEnumerable(Of String))
            For Each Item As String In items
                Me.Add(Item)
            Next
        End Sub

#Region "IDisposable Support"
        Private disposedValue As Boolean ' So ermitteln Sie überflüssige Aufrufe

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                End If
                _group = Nothing
            End If
            Me.disposedValue = True
        End Sub

        ' Dieser Code wird von Visual Basic hinzugefügt, um das Dispose-Muster richtig zu implementieren.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Ändern Sie diesen Code nicht. Fügen Sie oben in Dispose(disposing As Boolean) Bereinigungscode ein.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class

End Namespace