<Reflection.Obfuscation(Feature:="renaming", ApplyToMembers:=False, Exclude:=True)> ' needed to avoid resource-file-names obfuscation errors, see issue #5
Public Class EyeletsForm
    Inherits AnalysisForm
    Public Sub New()
        MyBase.New
        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()

        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.

    End Sub

    Public Sub New(doc As DocumentForm)
        MyBase.New(doc)
        Me.Visible = False
        InitializeComponent()
        For Each connectorOccurrence As Connector_occurrence In Kbl.GetConnectorOccurrences.Where(Function(con) con.Usage = Connector_usage.ringterminal)
            ActiveObjects.Add(connectorOccurrence.SystemId)
        Next

    End Sub

End Class