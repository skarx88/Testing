Imports System.ComponentModel
Imports Zuken.E3.HarnessAnalyzer.Checks.Cavities.Settings
Imports Zuken.E3.HarnessAnalyzer.Checks.Cavities.Views.Document

Namespace Checks.Cavities

    Public Class CavityCheckForm

        Private WithEvents _documents As Views.Document.DocumentViewCollection

        Private Sub New(mainform As MainForm)
            InitializeComponent()
            Me.Icon = My.Resources.Checks_CavityAssignment
            Me.MainForm = mainform
        End Sub

        Friend Sub New(documents As Views.Document.DocumentViewCollection, mainForm As MainForm)
            Me.New(mainForm)
            _documents = documents
        End Sub

        Private Sub _documents_ActiveChanged(sender As Object, e As EventArgs) Handles _documents.ActiveChanged
            Me.CavityNavigator.Document = _documents.Active
        End Sub

        Property Document As DocumentView
            Get
                Return Me.CavityNavigator.Document
            End Get
            Set(value As DocumentView)
                Me.CavityNavigator.Document = value
            End Set
        End Property

        ReadOnly Property MainForm As MainForm

        Protected Overrides Sub OnClosing(e As CancelEventArgs)
            e.Cancel = True
            Me.Hide()
        End Sub

        Private Function CheckDifferentModulesActiveDocument() As DifferentModulesResult
            If _documents.Active IsNot Nothing Then
                If _documents.Active.Model.Settings IsNot Nothing AndAlso Not _documents.Active.Model.Settings.ActiveModules.AreEqualToCurrentInactive(_documents.Active.Model) Then
                    Dim dlg As New DifferentModulesDialog()
                    Select Case dlg.ShowDialog(Me, _documents.Active.Model)
                        Case DialogResult.Yes
                            Return DifferentModulesResult.Apply
                        Case DialogResult.Cancel
                            Return DifferentModulesResult.Cancel
                    End Select
                End If
            End If
            Return DifferentModulesResult.Ignore
        End Function

        Public Shadows Sub Show(owner As IWin32Window)
            Me.DialogResult = DialogResult.OK
            Dim result As DifferentModulesResult = CheckDifferentModulesActiveDocument()
            Select Case result
                Case DifferentModulesResult.Cancel
                    Me.DialogResult = DialogResult.Cancel
                    Me.Hide()
                Case Else
                    If _documents?.Active IsNot Nothing Then
                        With _documents.Active
                            Select Case result
                                Case DifferentModulesResult.Apply
                                    .RaiseNeedsInactiveModuleSettingsApplied()
                                Case DifferentModulesResult.Ignore
                                    UpdateActiveModelSettings()
                            End Select
                        End With
                        MyBase.Show(owner)
                    Else
                        MessageBoxEx.ShowError(Me, "Un-normal behavior detected: no active documentview is set to apply module settings here! Process was aborted!")
                    End If
            End Select
        End Sub

        Private Function GetActiveHarnessConfigurationId() As String
            Return My.Application.MainForm?.ActiveDocument?._harnessModuleConfigurations?.Where(Function(hc) hc.IsActive).FirstOrDefault?.HarnessConfigurationObject.SystemId
        End Function

        Private Sub UpdateActiveModelSettings()
            UpdateModelSettings(_documents.Active)
        End Sub

        Private Sub UpdateModelSettings(doc As DocumentView)
            If doc IsNot Nothing Then
                With doc
                    .Model.Settings.Update()
                    .Model.Settings.ActiveHarnessConfigurationId = GetActiveHarnessConfigurationId()
                End With
            End If
        End Sub

        Private Enum DifferentModulesResult
            Ignore = 0
            Apply = 1
            Cancel = 2
        End Enum

        Private Sub CavityCheckForm_VisibleChanged(sender As Object, e As EventArgs) Handles Me.VisibleChanged
            If Me.Visible Then
                Me.CavityNavigator.Focus()
            ElseIf Me.DialogResult <> DialogResult.Cancel Then
                UpdateActiveModelSettings()
            End If
        End Sub

        Private Sub btnReset_Click(sender As Object, e As EventArgs) Handles btnReset.Click
            If MessageBox.Show(Me, My.Resources.CavityChecksStrings.DoYouReallyWantToResetAllCheckedStates, [Shared].MSG_BOX_TITLE, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                CavityNavigator.ResetAllCheckStates()
            End If
        End Sub

        Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
            Me.Close()
        End Sub

        Private Sub _documents_Closed(sender As Object, e As DocumentEventArgs) Handles _documents.Closed
            If _documents.Count = 0 Then
                Me.Hide()
            End If
        End Sub

        Private Sub _documents_Closing(sender As Object, e As DocumentEventArgs) Handles _documents.Closing
            UpdateModelSettings(e.Document)
        End Sub

    End Class

End Namespace