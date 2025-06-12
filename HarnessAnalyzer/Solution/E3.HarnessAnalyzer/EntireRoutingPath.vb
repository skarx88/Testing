Public Class RoutingConnection

    Friend Property Cable As Special_wire_occurrence
    Friend Property CavityNumberA As String
    Friend Property CavityNumberB As String
    Friend Property ConnectorA As Connector_occurrence
    Friend Property ConnectorB As Connector_occurrence
    Friend Property ConnectorSymbols As List(Of VectorDraw.Professional.vdPrimaries.vdFigure)
    Friend Property Core As Core_occurrence
    Friend Property CrossSectionArea As Numerical_value
    Friend Property DocumentName As String
    Friend Property GeneralTerminalA As General_terminal
    Friend Property GeneralTerminalB As General_terminal
    Friend Property IsActiveConnection As Boolean
    Friend Property PredecessorConnection As RoutingConnection
    Friend Property SuccessorConnection As RoutingConnection
    Friend Property Wire As Wire_occurrence

    Public Sub New(docName As String)
        ConnectorSymbols = New List(Of VectorDraw.Professional.vdPrimaries.vdFigure)
        DocumentName = docName
    End Sub

End Class


Public Class RoutingPath

    Friend Property Connections As Dictionary(Of String, RoutingConnection)
    Friend Property Harness As Harness
    Friend Property IsOriginHarness As Boolean
    Friend Property SignalName As String

    Public Sub New(isOriginHrns As Boolean, hrns As Harness)
        Connections = New Dictionary(Of String, RoutingConnection)
        IsOriginHarness = isOriginHrns
        Harness = hrns
    End Sub

End Class