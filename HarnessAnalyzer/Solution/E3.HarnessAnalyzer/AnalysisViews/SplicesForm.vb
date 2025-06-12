<Reflection.Obfuscation(Feature:="renaming", ApplyToMembers:=False, Exclude:=True)> ' needed to avoid resource-file-names obfuscation errors, see issue #5
Public Class SplicesForm
    Inherits AnalysisForm

    Public Sub New()
        MyBase.New
        InitializeComponent()
    End Sub

    Public Sub New(doc As DocumentForm)
        MyBase.New(doc)
        Me.Visible = False

        InitializeComponent()
        For Each connectorOccurrence As Connector_occurrence In Kbl.GetConnectorOccurrences.Where(Function(con) con.Usage = Connector_usage.splice)
            ActiveObjects.Add(connectorOccurrence.SystemId)
        Next
    End Sub

End Class