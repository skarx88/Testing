Imports System.ComponentModel

Public MustInherit Class IdentifierBaseCollection(Of T As IdentifierBase)
    Inherits BindingList(Of T)

    Public Sub New()
        MyBase.New()
    End Sub

    Protected Sub AddIdentifier(identifier As T)
        If (Not ContainsIdentifier(identifier)) Then
            Me.Add(identifier)
        End If
    End Sub

    Public MustOverride Sub CreateDefaultIdentifiers()

    Public Function ContainsIdentifier(identifier As T) As Boolean
        For Each localIdentifier As T In Me
            If (localIdentifier.KBLPropertyName = identifier.KBLPropertyName) AndAlso (localIdentifier.IdentificationCriteria = identifier.IdentificationCriteria) Then
                Return True
            End If
        Next
        Return False
    End Function

    Protected Function FindIdentifier(kblPropertyName As String) As T
        For Each localIdentifier As T In Me
            If (localIdentifier.KBLPropertyName = kblPropertyName) Then
                Return localIdentifier
            End If
        Next
        Return Nothing
    End Function



End Class
