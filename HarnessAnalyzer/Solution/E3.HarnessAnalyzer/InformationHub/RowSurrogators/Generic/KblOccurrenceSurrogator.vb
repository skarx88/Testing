Imports Infragistics.Win.UltraWinDataSource

Public Class KblOccurrenceSurrogator(Of TOccurrence As {IKblOccurrence, New}, TOccurrencePart As {IKblPartObject, New})
    Inherits DisposableObject

    Private _occurrence As TOccurrence
    Private _part As TOccurrencePart
    Private _systemId As String
    Private _kbl As IKblContainer

    Protected Sub New(kbl As IKblContainer)
        _kbl = kbl
    End Sub

    Public Sub New(kbl As IKblContainer, systemId As String)
        Me.New(kbl)
        Me.SystemId = systemId
    End Sub

    Public ReadOnly Property Kbl As IKblContainer
        Get
            Return _kbl
        End Get
    End Property

    Public Function GetOccurrence() As TOccurrence
        If _occurrence Is Nothing Then
            _occurrence = _kbl.GetOccurrenceObject(Of TOccurrence)(_systemId)
        End If
        Return _occurrence
    End Function

    Public Function GetPart() As TOccurrencePart
        If _part Is Nothing Then
            _part = _kbl.GetPart(Of TOccurrencePart)(GetOccurrence().Part)
        End If
        Return _part
    End Function

    Public Function GetUnit(value As Numerical_value) As Unit
        If Not String.IsNullOrEmpty(value.Unit_component) Then
            Return _kbl.GetUnit(value.Unit_component)
        End If
        Return Nothing
    End Function

    Public Function GetConnection() As Connection
        If Me.GetOccurrence IsNot Nothing Then
            Return Me.Kbl.GetConnectionOfWire(SystemId)
        End If
        Return Nothing
    End Function

    Public Function GetWiringGroups() As List(Of Wiring_group)
        Return Me.Kbl.GetWiringGroups.ToList
    End Function

    Property SystemId As String
        Get
            Return _systemId
        End Get
        Protected Set
            _systemId = Value
        End Set
    End Property

End Class
