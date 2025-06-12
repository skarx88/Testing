Imports Infragistics.Win.Misc
Imports Infragistics.Win
Imports System.Reflection
Imports Zuken.E3.HarnessAnalyzer.Shared
Imports Zuken.E3.Lib.Licensing

<Reflection.Obfuscation(Feature:="renaming", ApplyToMembers:=False, Exclude:=True)> ' needed to avoid resource-file-names obfuscation errors, see issue #5
Public Class BorrowUtility

    Private Const COMMANDLINESWITCHCHAR As String = "/"

    Private _commandLineSwitch As CommandLineSwitch
    Private _logger As New Zuken.E3.Lib.Licensing.Logger
    Private _manager As New E3.HarnessAnalyzer.[Shared].LicenseManager
    Private _logView As New LogViewFrm
    Private _serverLicDlg As New ServerLicensesDialog
    Private _lock As New System.Threading.SemaphoreSlim(1)

    Public Sub New()
        InitializeComponent()
        _logView.Logger = _logger
        _logView.Owner = Me
        _serverLicDlg.Logger = _logger
        _serverLicDlg.Manager = _manager
        _serverLicDlg.Owner = Me
        _manager.Init()
    End Sub

    Private Function GetCommandLineSwitch() As CommandLineSwitch
        Dim result As CommandLineSwitch = CommandLineSwitch.NoSwitch
        If My.Application.CommandLineArgs.Count > 0 Then
            For Each arg As String In My.Application.CommandLineArgs
                If arg.StartsWith(COMMANDLINESWITCHCHAR) Then
                    arg = arg.Remove(0, COMMANDLINESWITCHCHAR.Length).Trim
                    Dim switch As CommandLineSwitch = CommandLineSwitch.NoSwitch
                    If [Enum].TryParse(Of CommandLineSwitch)(arg, True, switch) Then
                        result = result Or switch
                    End If
                End If
            Next
        End If
        Return result
    End Function

    Private Function IsAnalyzerRunning() As Boolean
        Dim p As Process() = Process.GetProcessesByName([Shared].Common.E3HA_PROCESS_NAME)
        Return CBool(p.Length > 0)
    End Function


    Private Sub BorrowUtility_Load(sender As Object, e As EventArgs) Handles Me.Load
        _commandLineSwitch = GetCommandLineSwitch()

        Dim validationSettings As ValidationSettings = Me.uvalBorrowDate.GetValidationSettings(Me.ucalBorrow)
        Dim condition As OperatorCondition = CType(validationSettings.Condition, OperatorCondition)
        condition = New OperatorCondition(ConditionOperator.GreaterThanOrEqualTo, Now.Date)
        validationSettings.NotificationSettings.Caption = My.Resources.Strings.InvalidDate
        validationSettings.Condition = condition
        validationSettings.IsRequired = True
        validationSettings.RetainFocusOnError = False

        Me.uvalBorrowDate.ValidationTrigger = ValidationTrigger.OnPropertyValueChanged
        Me.ucalBorrow.Value = Now.Date

#If DEBUG Or CONFIG = "Debug" Then
        Me.MenuStrip1.Visible = True
#Else
        Me.MenuStrip1.Visible = _commandLineSwitch.HasFlag(CommandLineSwitch.Debug)
#End If

        If _commandLineSwitch = CommandLineSwitch.Debug Then
            Me.MenuStrip1.Visible = True
        End If

        Dim expirationDate As New Date
        _manager.Borrow.Purge(_logger)

        Dim res As DateResult = _manager.Borrow.GetExpirationDate(_logger)
        If res.IsSuccess Then
            Me.ucalExpire.Value = res.Date
            Me.upReturn.Visible = True
            Me.upBorrow.Visible = False
            Me.upReturn.Dock = DockStyle.Fill
        Else
            Me.ucalExpire.Value = DBNull.Value
            Me.ucalBorrow.Value = Now + TimeSpan.FromDays(1)
            Me.upBorrow.Visible = True
            Me.upReturn.Visible = False
            Me.upBorrow.Dock = DockStyle.Fill
        End If
    End Sub

    Private Sub BorrowUtility_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        If _commandLineSwitch.HasFlag(CommandLineSwitch.BuildServerTest) Then
            Me.Close()
        End If
    End Sub

    Private Async Sub ubtnBorrow_Click(sender As Object, e As EventArgs) Handles ubtnBorrow.Click
        Await _lock.WaitAsync
        Try
            Dim val As Validation = Me.uvalBorrowDate.Validate(True, False)
            If val.IsValid Then
                Me.UseWaitCursor = True

                If IsAnalyzerRunning() Then
                    MessageBox.Show(Me, My.Resources.Strings.CloseHarnessAnalyzerBeforeBorrowing, Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Information)
                Else
                    Try
                        Me.WaitBorrow_Panel.Visible = True
                        Dim resFt As ApplicationFeatureInfosResult = Await Task.Run(Function() _manager.Status.GetAvailableFeaturesFromServer(_logger))
                        If resFt.IsSuccess AndAlso resFt.FeatureInfos.Length > 0 Then
                            Dim appFeatureNames As New List(Of String)
                            For Each fInfo As ApplicationFeatureInfo In resFt.FeatureInfos
                                appFeatureNames.Add(fInfo.Name)
                            Next

                            Dim resBorrow As BorrowResult = Await Task.Run(Function() _manager.Borrow.BorrowFeaturesOrAvailable(appFeatureNames.ToArray, CDate(Me.ucalBorrow.Value), _logger))
                            If resBorrow.IsSuccess Then
                                Me.ubtnBorrow.Enabled = False
                                Me.ucalBorrow.Enabled = False
                                Me.ulStatus.Appearance.ForeColor = Color.Black

                                If (resBorrow.Message = String.Empty) Then
                                    Me.ulStatus.Text = My.Resources.Strings.BorrowedSuccessfully_StatusText
                                Else
                                    Me.ulStatus.Text = My.Resources.Strings.BorrowedSuccessfullyWithWarnings_StatusText
                                    MessageBox.Show(Me, resBorrow.Message, Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                End If
                            Else
                                Me.ulStatus.Appearance.ForeColor = Color.Red
                                Me.ulStatus.Text = My.Resources.Strings.BorrowingFailed_StatusText
                                MessageBox.Show(Me, resBorrow.Message, Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
                            End If
                        Else
                            Me.ulStatus.Appearance.ForeColor = Color.Red
                            Me.ulStatus.Text = My.Resources.Strings.BorrowingFailed_StatusText
                            MessageBox.Show(Me, resFt.Message, Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        End If
                    Finally
                        Me.WaitBorrow_Panel.Visible = False
                    End Try
                End If
            End If
        Finally
            Me.UseWaitCursor = False
            _lock.Release()
        End Try
    End Sub

    Private Sub ubtnClose_Click(sender As Object, e As EventArgs) Handles ubtnClose.Click
        Me.Close()
    End Sub

    Private Async Sub ubtnReturn_Click(sender As Object, e As EventArgs) Handles ubtnReturn.Click
        Await _lock.WaitAsync()
        Try
            Me.UseWaitCursor = True
            If IsAnalyzerRunning() Then
                MessageBox.Show(Me, My.Resources.Strings.CloseHarnessAnalyzerBeforeBorrowing, Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Information)
            Else
                Try
                    Me.WaitReturn_Panel.Visible = True
                    Dim res As LicenseErrorCodeResult = Await Task.Run(Function() _manager.Borrow.ReturnAllFeaturesFromAllServers(_logger))
                    If res.IsSuccess Then
                        Me.ubtnReturn.Enabled = False
                        Me.ulStatus.Appearance.ForeColor = Color.Black
                        Me.ulStatus.Text = My.Resources.Strings.ReturnedSuccessfully_StatusText
                        Me.ucalExpire.Value = DBNull.Value
                    Else
                        Me.ulStatus.Appearance.ForeColor = Color.Red
                        Me.ulStatus.Text = My.Resources.Strings.ReturningFailed_StatusText
                        MessageBox.Show(Me, res.Message, Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    End If
                Finally
                    Me.WaitReturn_Panel.Visible = False
                End Try
            End If
        Finally
            Me.UseWaitCursor = False
            _lock.Release()
        End Try
    End Sub

    Private Sub uvalBorrowDate_ValidationError(sender As Object, e As Infragistics.Win.Misc.ValidationErrorEventArgs) Handles uvalBorrowDate.ValidationError
        If e.Control Is Me.ucalBorrow Then
            e.NotificationSettings.Text = My.Resources.Strings.DateMustNotInPast_ValidationText
        End If
    End Sub

    Private Sub ShowServerLicensesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ShowServerLicensesToolStripMenuItem.Click
        If Not _serverLicDlg.Visible Then
            _serverLicDlg.Show(Me)
        End If
    End Sub

    Private Sub ViewLogToolStripMenuItem_Click_1(sender As Object, e As EventArgs) Handles ViewLogToolStripMenuItem.Click
        If Not _logView.Visible Then
            _logView.Show(My.Forms.BorrowUtility)
        End If
    End Sub

    ReadOnly Property LogView As LogViewFrm
        Get
            Return _logView
        End Get
    End Property

    Private Sub EditLicensePathToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles EditLicensePathToolStripMenuItem.Click
        ShowLicenseFileDialog()
    End Sub

    Private Function ShowLicenseFileDialog(Optional showOpenFile As Boolean = False) As Boolean
        Using dlg As New LicenseFileDlg
            dlg.Manager = _manager
            dlg.ShowOpenFileOnOpen = showOpenFile
            If dlg.ShowDialog(Me) = DialogResult.OK Then
                If dlg.IsValid Then
                    dlg.ApplyTextToLicenseFilePath(_logger)
                    Return True
                Else
                    MessageBox.Show(Me, My.Resources.Strings.LicensePathNotValid_Message, My.Application.Info.Title, MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
            End If
        End Using
        Return False
    End Function

    Private Sub CloseToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CloseToolStripMenuItem.Click
        Me.Close()
    End Sub

    Private Sub ResetFlexLmToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ResetFlexLmToolStripMenuItem.Click
        Try
            _manager.Borrow.ClearEverything(_logger)
            ShowLicenseFileDialog(True)
            _manager.ResetLicensing()
        Catch ex As Exception
            MessageBox.Show(Me, String.Format(My.Resources.Strings.ResetFailed_Message, ex.Message), My.Application.Info.Title, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End Try
        LicenseFileDlg.Manager = _manager
        Dim path As String = LicenseFileDlg.GetLicenseFilePath
        If String.IsNullOrEmpty(path) Then
            MessageBox.Show(Me, My.Resources.Strings.LicensePathMustBeSetOrWillNotWorkAsExpected_Message, My.Application.Info.Title, MessageBoxButtons.OK, MessageBoxIcon.Warning)
        Else
            MessageBox.Show(Me, My.Resources.Strings.ResetFinished_Message, My.Application.Info.Title, MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub

    Private Sub ShowLicenseEnvVariablesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ShowLicenseEnvVariablesToolStripMenuItem.Click
        Using dlg As New EnvVarsDialog
            dlg.Manager = _manager
            dlg.Populate()
            dlg.ShowDialog(Me)
        End Using
    End Sub

    Private Sub ToolStripMenuItem1_Click_1(sender As Object, e As EventArgs) Handles ToolStripMenuItem1.Click
        Using dlg As New AvailableFeaturesDlg
            dlg.Manager = _manager
            If dlg.ShowDialog(Me) = DialogResult.OK Then
                _manager.SetAvailableFeatures(dlg.CheckedFeatures.Cast(Of Object).Select(Function(obj) CStr(obj)).ToArray)
            End If
        End Using
    End Sub

End Class
