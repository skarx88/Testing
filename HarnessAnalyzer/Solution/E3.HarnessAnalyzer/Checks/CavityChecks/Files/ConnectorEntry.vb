Imports System.Runtime.Serialization
Imports System.Text
Imports System.Xml.Serialization
Imports Zuken.E3.HarnessAnalyzer.Checks.Cavities.Settings

Namespace Checks.Cavities.Files

    <DataContract(Name:="Connector", [Namespace]:=CavityChecksFile.Namespace)>
    <XmlType(TypeName:="Connector")> ' for schema generation
    <KnownType(GetType(CavityEntryCollection))>
    Public Class ConnectorEntry

        Private Sub New()
        End Sub

        Friend Sub New(conn As Views.Model.ConnectorView)
            If Not conn.Model.Settings.Connectors.Contains(conn.KblId) Then
                Throw New ArgumentException(String.Format("Connector {0} ({1}) not found und current Model-setting", conn.Name, conn.KblId))
            End If

            Dim cSetting As Cavities.Settings.ConnectorSetting = conn.Model.Settings.Connectors(conn.KblId)
            Dim cavSettings As Dictionary(Of String, List(Of Cavities.Settings.CavityWireSetting)) = cSetting.Cavities.GroupBy(Function(cv) cv.CavityId).ToDictionary(Function(grp) grp.Key, Function(grp) grp.ToList)
            Dim cavWires As Dictionary(Of String, List(Of Views.Model.CavityWireView)) = conn.CavWires.GroupBy(Function(cw) cw.KblCavityId).ToDictionary(Function(grp) grp.Key, Function(grp) grp.ToList)

            For Each cavitySet As KeyValuePair(Of String, List(Of Settings.CavityWireSetting)) In cavSettings
                If cavWires.ContainsKey(cavitySet.Key) Then
                    Dim wires As New HashSet(Of String)(cavitySet.Value.Select(Function(cs) cs.WireId))
                    Dim wiresOfCav As List(Of Views.Model.CavityWireView) = cavWires(cavitySet.Key)
                    Dim cavEntry As New CavityEntry
                    cavEntry.CopyPropertiesFrom(wiresOfCav.First)
                    cavEntry.Checked = wiresOfCav.GetCheckState

                    For Each wView As Views.Model.CavityWireView In wiresOfCav
                        If Not wView.IsVirtual AndAlso wires.Contains(wView.KblWireId) Then
                            Dim wireEntry As New WireEntry
                            With wireEntry
                                .CopyPropertiesFrom(wView)
                            End With
                            cavEntry.Wires.Add(wireEntry)
                        End If
                    Next

                    Cavities.Add(cavEntry)
                End If
            Next

            CopyProptertiesFrom(conn)
        End Sub

        Private Sub CopyProptertiesFrom(conn As Views.Model.ConnectorView)
            Me.KblId = conn.KblId
            Me.Name = New StringBuilder(conn.Name).ToString
            Me.Checked = CheckedString.GetCheckedState(conn.CheckState)
        End Sub

        <IgnoreDataMember>
        <XmlIgnore> ' for xsd schema generation
        Property KblId As String = String.Empty

        <DataMember(Order:=0)>
        <XmlElement(IsNullable:=True, Order:=0)>
        Property Name As String

        <DataMember(Order:=1)>
        <XmlElement(IsNullable:=False, Order:=1)>
        Property Checked As CheckedState

        <DataMember(Order:=2)>
        <XmlElement(IsNullable:=False, Order:=2)>
        ReadOnly Property Cavities As New CavityEntryCollection

        Public Overrides Function ToString() As String
            Return String.Format("{0}, Cavities:{1}", KblId, Me.Cavities.Count)
        End Function

    End Class

End Namespace