
Imports System.Runtime.CompilerServices
Imports Zuken.E3.HarnessAnalyzer.Checks.Cavities.Views.Model

Namespace Checks.Cavities.Settings

    Public Module Extensions

        <Extension>
        Friend Function AreEqualToCurrentInactive(activeModuleSettings As IEnumerable(Of ModuleSetting), modelView As Views.Model.ModelView) As Boolean
            Dim activeSettingDic As Dictionary(Of String, ModuleSetting) = activeModuleSettings.ToDictionary(Function(ams) [ams].SystemId, Function(ams) [ams])
            Dim activeDocModules As New Dictionary(Of String, [Module])

            For Each m As [Module] In modelView.GetModules
                If Not modelView.CurrentInactiveModules.ContainsKey(m.SystemId) Then
                    activeDocModules.Add(m.SystemId, m)
                End If
            Next

            Return activeSettingDic.Keys.SequenceEqual(activeDocModules.Keys)
        End Function

        <Extension>
        Friend Function ResolveModules(moduleSettings As IEnumerable(Of ModuleSetting), kbl As KBLMapper) As IEnumerable(Of [Module])
            Dim resolved As New List(Of [Module])
            For Each ms As ModuleSetting In moduleSettings
                Dim m As [Module] = Nothing
                If kbl.TryGetModule(ms.SystemId, m) Then
                    resolved.Add(m)
                Else
                    resolved.Add(Nothing)
                End If
            Next
            Return resolved
        End Function

        <Extension>
        Friend Function GetCheckState(views As IEnumerable(Of Checks.Cavities.Views.Model.CavityWireView)) As Checks.Cavities.Files.CheckedState
            If views.All(Function(v) v.CheckState = CheckState.Checked) Then
                Return Files.CheckedString.GetCheckedState(ConnectorView.State.Checked)
            ElseIf views.Any(Function(v) v.CheckState = CheckState.Unchecked) Then
                Return Files.CheckedString.GetCheckedState(ConnectorView.State.AnyUnchecked)
            ElseIf views.Any(Function(v) v.CheckState = CheckState.Checked) Then
                Return Files.CheckedString.GetCheckedState(ConnectorView.State.Partial)
            Else
                Return Files.CheckedString.GetCheckedState(ConnectorView.State.Indeterminate)
            End If
        End Function

        <Extension>
        Friend Function GetKblIdsFromViews(views As IEnumerable(Of Checks.Cavities.Views.BaseView)) As List(Of String)
            Dim kblIds As New HashSet(Of String)
            For Each bw As Checks.Cavities.Views.BaseView In views
                If TypeOf bw Is Checks.Cavities.Views.Model.ConnectorView Then
                    Dim conn As Checks.Cavities.Views.Model.ConnectorView = CType(bw, Checks.Cavities.Views.Model.ConnectorView)
                    kblIds.Add(conn.KblId)
                ElseIf TypeOf bw Is Checks.Cavities.Views.Model.CavityWireView Then
                    Dim cav As Checks.Cavities.Views.Model.CavityWireView = CType(bw, Checks.Cavities.Views.Model.CavityWireView)
                    If Not String.IsNullOrEmpty(cav.KblContactPointId) Then
                        kblIds.Add(cav.KblContactPointId)
                    Else
                        kblIds.Add(cav.KblCavityId)
                    End If

                End If
            Next
            Return kblIds.ToList
        End Function

    End Module

End Namespace

Namespace Checks.Cavities

    Public Module Extensions

        <Extension>
        Friend Function GetKblIdsFromViews(views As IEnumerable(Of Checks.Cavities.Views.BaseView)) As List(Of String)
            Dim kblIds As New HashSet(Of String)
            For Each bw As Checks.Cavities.Views.BaseView In views
                If TypeOf bw Is Checks.Cavities.Views.Model.ConnectorView Then
                    Dim conn As Checks.Cavities.Views.Model.ConnectorView = CType(bw, Checks.Cavities.Views.Model.ConnectorView)
                    If Not String.IsNullOrEmpty(conn.KblId) Then kblIds.Add(conn.KblId)
                ElseIf TypeOf bw Is Checks.Cavities.Views.Model.CavityWireView Then
                    Dim cav As Checks.Cavities.Views.Model.CavityWireView = CType(bw, Checks.Cavities.Views.Model.CavityWireView)
                    'HINT Trial to get the connector selected in the connector of the cavity check even if cavity is selected alone
                    If Not String.IsNullOrEmpty(CType(cav.Parent, Checks.Cavities.Views.Model.ConnectorView).KblId) Then kblIds.Add(CType(cav.Parent, Checks.Cavities.Views.Model.ConnectorView).KblId)

                    If Not String.IsNullOrEmpty(cav.KblCavityId) Then kblIds.Add(cav.KblCavityId)
                    If Not String.IsNullOrEmpty(cav.KblWireId) Then kblIds.Add(cav.KblWireId)
                End If
            Next
            Return kblIds.ToList
        End Function

    End Module

End Namespace