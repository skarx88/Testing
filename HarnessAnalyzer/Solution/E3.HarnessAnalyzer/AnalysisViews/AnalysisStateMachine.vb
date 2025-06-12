Imports Infragistics.Win.UltraWinToolbars
Imports Zuken.E3.HarnessAnalyzer.Shared.Common

Public Class AnalysisStateMachine
    Private ReadOnly _logEventArgs As LogEventArgs

    Private WithEvents _documentForm As DocumentForm
    Private WithEvents _utmDocument As UltraToolbarsManager
    Private WithEvents _base As AnalysisForm
    Private WithEvents _editIssuesForm As IssueReporting.EditIssuesForm

    Private _activeButton As StateButtonTool

    Friend _stateButtons As New List(Of String)
    Friend IsAnalysisActive As Boolean
    Friend Event LogMessage(sender As DocumentForm, e As LogEventArgs)

    Public QMIButton As StateButtonTool

    Public Property ActiveButton As StateButtonTool
        Get
            Return _activeButton
        End Get
        Set(value As StateButtonTool)
            If _activeButton IsNot Nothing AndAlso value IsNot Nothing Then

                If _activeButton.Key <> value.Key Then 'Toggle action
                    If _base IsNot Nothing Then
                        _base.Close()
                        _base = Nothing
                    End If
                    If (_editIssuesForm IsNot Nothing) AndAlso (_editIssuesForm.Visible) Then _editIssuesForm.Close()
                    ResetStateButton()
                    _activeButton = value
                    ShowAnalysisForm(value.Key)
                Else
                    If _base IsNot Nothing Then ' Close action
                        _base.Close()
                        _base = Nothing
                    End If
                    If (_editIssuesForm IsNot Nothing) AndAlso (_editIssuesForm.Visible) Then _editIssuesForm.Close()
                    ResetStateButton()
                    ClearResults()
                End If
            ElseIf value IsNot Nothing Then ' Start action
                _activeButton = value
                ShowAnalysisForm(value.Key)
            End If
        End Set
    End Property

    Public Sub New(documentForm As DocumentForm)
        _documentForm = documentForm
        _logEventArgs = New LogEventArgs
        _utmDocument = documentForm.utmDocument

        InitializeDocumentRibbon()
    End Sub

    Private Sub InitializeDocumentRibbon()
        _utmDocument.BeginUpdate()

        Try
            _utmDocument.Office2007UICompatibility = False
            _utmDocument.Style = ToolbarStyle.Office2010

            CreateTabTools()
            InitializeRibbon()

        Catch ex As Exception
            ex.ShowMessageBox(String.Format(ErrorStrings.DocStatMachine_ErrorLoadMenu_Msg, vbCrLf, ex.Message))
        Finally
            _utmDocument.EndUpdate()
        End Try
    End Sub
    Private Sub CreateTabTools()
        With _utmDocument
            Dim analysisShowDryWet As New StateButtonTool(HomeTabToolKey.AnalysisShowDryWet.ToString)
            With analysisShowDryWet.SharedProps
                .AppearancesLarge.Appearance.Image = My.Resources.AnalysisShowDryWet.ToBitmap
                .AppearancesSmall.Appearance.Image = My.Resources.AnalysisShowDryWet.ToBitmap
                .Caption = MenuButtonStrings.ShowDryWetEnv
            End With

            Dim analysisShowEyelets As New StateButtonTool(HomeTabToolKey.AnalysisShowEyelets.ToString)
            With analysisShowEyelets.SharedProps
                .AppearancesLarge.Appearance.Image = My.Resources.AnalysisShowEyelets.ToBitmap
                .AppearancesSmall.Appearance.Image = My.Resources.AnalysisShowEyelets.ToBitmap
                .Caption = MenuButtonStrings.ShowEyelets
            End With

            Dim analysisShowPlatingMat As New StateButtonTool(HomeTabToolKey.AnalysisShowPlatingMat.ToString)
            With analysisShowPlatingMat.SharedProps
                .AppearancesLarge.Appearance.Image = My.Resources.AnalysisShowPlatingMat.ToBitmap
                .AppearancesSmall.Appearance.Image = My.Resources.AnalysisShowPlatingMat.ToBitmap
                .Caption = MenuButtonStrings.ShowPlatingMaterial
            End With

            Dim analysisShowProtections As New StateButtonTool(HomeTabToolKey.AnalysisShowProtections.ToString)
            With analysisShowProtections.SharedProps
                .AppearancesLarge.Appearance.Image = My.Resources.AnalysisShowProtections.ToBitmap
                .AppearancesSmall.Appearance.Image = My.Resources.AnalysisShowProtections.ToBitmap
                .Caption = MenuButtonStrings.ShowProtections
            End With

            Dim analysisShowPartnumbers As New StateButtonTool(HomeTabToolKey.AnalysisShowPartnumbers.ToString)
            With analysisShowPartnumbers.SharedProps
                .AppearancesLarge.Appearance.Image = My.Resources.AnalysisShowPartnumbers.ToBitmap
                .AppearancesSmall.Appearance.Image = My.Resources.AnalysisShowPartnumbers.ToBitmap
                .Caption = MenuButtonStrings.ShowPartnumbers
            End With

            Dim analysisShowQMIssues As New StateButtonTool(HomeTabToolKey.AnalysisShowQMIssues.ToString)
            With analysisShowQMIssues.SharedProps
                .AppearancesLarge.Appearance.Image = My.Resources.AnalysisShowQMIssues.ToBitmap
                .AppearancesSmall.Appearance.Image = My.Resources.AnalysisShowQMIssues.ToBitmap
                .Caption = MenuButtonStrings.ShowQMIssues
            End With

            Dim analysisShowSplices As New StateButtonTool(HomeTabToolKey.AnalysisShowSplices.ToString)
            With analysisShowSplices.SharedProps
                .AppearancesLarge.Appearance.Image = My.Resources.AnalysisShowSplices.ToBitmap
                .AppearancesSmall.Appearance.Image = My.Resources.AnalysisShowSplices.ToBitmap
                .Caption = MenuButtonStrings.ShowSplices
            End With

            _stateButtons.Add(analysisShowDryWet.Key)
            _stateButtons.Add(analysisShowEyelets.Key)
            _stateButtons.Add(analysisShowPlatingMat.Key)
            _stateButtons.Add(analysisShowProtections.Key)
            _stateButtons.Add(analysisShowPartnumbers.Key)
            _stateButtons.Add(analysisShowQMIssues.Key)
            _stateButtons.Add(analysisShowSplices.Key)

            .Tools.Add(analysisShowDryWet)
            .Tools.Add(analysisShowEyelets)
            .Tools.Add(analysisShowPlatingMat)
            .Tools.Add(analysisShowProtections)
            .Tools.Add(analysisShowPartnumbers)
            .Tools.Add(analysisShowQMIssues)
            .Tools.Add(analysisShowSplices)
            QMIButton = analysisShowQMIssues
        End With
    End Sub
    Private Sub InitializeRibbon()
        With _utmDocument.Ribbon
            Dim homeTab As RibbonTab = .Tabs.Item(NameOf(MenuButtonStrings.Home))
            With homeTab
                .Caption = MenuButtonStrings.Home

                Dim analysisViewGroup As RibbonGroup = .Groups.Add(NameOf(MenuButtonStrings.AnalysisViews))
                With analysisViewGroup
                    .Caption = MenuButtonStrings.AnalysisViews
                    .MergeOrder = 7
                    .Tools.AddTool(HomeTabToolKey.AnalysisShowDryWet.ToString)
                    .Tools(HomeTabToolKey.AnalysisShowDryWet.ToString).InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large
                    .Tools.AddTool(HomeTabToolKey.AnalysisShowEyelets.ToString)
                    .Tools(HomeTabToolKey.AnalysisShowEyelets.ToString).InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large
                    .Tools.AddTool(HomeTabToolKey.AnalysisShowPlatingMat.ToString)
                    .Tools(HomeTabToolKey.AnalysisShowPlatingMat.ToString).InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large
                    .Tools.AddTool(HomeTabToolKey.AnalysisShowQMIssues.ToString)
                    .Tools(HomeTabToolKey.AnalysisShowQMIssues.ToString).InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large
                    .Tools.AddTool(HomeTabToolKey.AnalysisShowProtections.ToString)
                    .Tools(HomeTabToolKey.AnalysisShowProtections.ToString).InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large
                    .Tools.AddTool(HomeTabToolKey.AnalysisShowPartnumbers.ToString)
                    .Tools(HomeTabToolKey.AnalysisShowPartnumbers.ToString).InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large
                    .Tools.AddTool(HomeTabToolKey.AnalysisShowSplices.ToString)
                    .Tools(HomeTabToolKey.AnalysisShowSplices.ToString).InstanceProps.PreferredSizeOnRibbon = RibbonToolSize.Large
                End With
            End With
        End With
    End Sub

    Public Sub EnableButton(key As HomeTabToolKey)
        Dim myStateButton As StateButtonTool
        With _utmDocument.Ribbon
            Dim homeTab As RibbonTab = .Tabs(NameOf(MenuButtonStrings.Home))
            With homeTab
                .Caption = MenuButtonStrings.Home

                Dim analysisViewGroup As RibbonGroup = .Groups(NameOf(MenuButtonStrings.AnalysisViews))
                With analysisViewGroup
                    .Caption = MenuButtonStrings.AnalysisViews
                    myStateButton = CType(.Tools.Item(key.ToString), StateButtonTool)
                End With
            End With
        End With

        If myStateButton IsNot Nothing Then
            myStateButton.SharedProps.Enabled = True
        End If
    End Sub

    Public Sub DisableButton(key As HomeTabToolKey)
        Dim myStateButton As StateButtonTool
        With _utmDocument.Ribbon
            Dim homeTab As RibbonTab = .Tabs.Item(NameOf(MenuButtonStrings.Home))
            With homeTab
                .Caption = MenuButtonStrings.Home
                Dim analysisViewGroup As RibbonGroup = .Groups.Item(NameOf(MenuButtonStrings.AnalysisViews))
                With analysisViewGroup
                    .Caption = MenuButtonStrings.AnalysisViews
                    myStateButton = CType(.Tools.Item(key.ToString), StateButtonTool)
                End With
            End With
        End With
        If myStateButton IsNot Nothing Then
            myStateButton.SharedProps.Enabled = False
        End If
    End Sub


    Private Sub _utmDocument_ToolClick(sender As Object, e As ToolClickEventArgs) Handles _utmDocument.ToolClick
        If TypeOf (e.Tool) Is StateButtonTool AndAlso _stateButtons.Contains(CType(e.Tool, StateButtonTool).Key) Then
            ActiveButton = CType(e.Tool, StateButtonTool)
        End If
    End Sub

    Private Sub ShowAnalysisForm(key As String)

        Select Case key

            Case HomeTabToolKey.AnalysisShowEyelets.ToString
                _base = New EyeletsForm(_documentForm)
                IsAnalysisActive = True
                ShowResults(CType(_base, EyeletsForm).ActiveObjects)
                _base.Close()
                _base = Nothing

            Case HomeTabToolKey.AnalysisShowSplices.ToString
                _base = New SplicesForm(_documentForm)
                IsAnalysisActive = True
                ShowResults(CType(_base, SplicesForm).ActiveObjects)
                _base.Close()
                _base = Nothing

            Case HomeTabToolKey.AnalysisShowDryWet.ToString
                _base = New DryWetForm(_documentForm)

            Case HomeTabToolKey.AnalysisShowPartnumbers.ToString
                _base = New PartNumbersForm(_documentForm)

            Case HomeTabToolKey.AnalysisShowPlatingMat.ToString
                _base = New PlatingMaterialForm(_documentForm)

            Case HomeTabToolKey.AnalysisShowProtections.ToString
                _base = New ProtectionsForm(_documentForm)

            Case HomeTabToolKey.AnalysisShowQMIssues.ToString
                _base = Nothing
                Using myForm As New QMIssuesForm(_documentForm)
                    If myForm.ShowDialog(_documentForm) = DialogResult.OK Then
                        ShowResults(myForm.ActiveObjects)
                        _editIssuesForm = New IssueReporting.EditIssuesForm(myForm.IssueReportFile, myForm.GetColorRanges)
                        _editIssuesForm.Show(_documentForm)
                    Else
                        ResetStateButton()
                    End If
                End Using

        End Select

        If _base IsNot Nothing Then
            _base.Show(_documentForm)
        End If
    End Sub

    Private Sub ResetStateButton()
        If _activeButton IsNot Nothing Then
            _utmDocument.EventManager.AllEventsEnabled = False
            _activeButton.Checked = False
            _utmDocument.EventManager.AllEventsEnabled = True
            _activeButton = Nothing
        End If
    End Sub

    Private Sub ShowResults(ids As ICollection(Of String))
        _documentForm.Cursor = Cursors.WaitCursor
        _documentForm.FilterAnalysisViewObjects(ids)
        _documentForm.Cursor = Cursors.Default
        IsAnalysisActive = True
    End Sub

    Friend Sub ClearResults()
        If IsAnalysisActive Then
            _documentForm.FilterAnalysisViewObjects(Nothing)
            _documentForm.ApplyModuleConfiguration(False, False) 'TODO  HINT check if this is neccassary
        End If

        _activeButton = Nothing
        IsAnalysisActive = False
    End Sub

    Private Sub _editIssuesForm_Closed(sender As Object, e As EventArgs) Handles _editIssuesForm.Closed
        _editIssuesForm = Nothing
        ResetStateButton()
        ClearResults()
    End Sub

    Private Sub _base_showResults(sender As Object, e As EventArgs) Handles _base.ShowResults
        Dim ids As ICollection(Of String) = CType(sender, AnalysisForm).ActiveObjects
        ShowResults(ids)
    End Sub

    Private Sub _base_Closed(sender As Object, e As EventArgs) Handles _base.Closed
        ResetStateButton()
        ClearResults()
    End Sub
    Public Sub CloseOpenForms()
        If _base IsNot Nothing Then
            _base.Close()
        End If
        If _editIssuesForm IsNot Nothing Then
            _editIssuesForm.Close()
        End If
    End Sub
End Class
