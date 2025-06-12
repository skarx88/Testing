<Reflection.Obfuscation(Feature:="renaming", ApplyToMembers:=False, Exclude:=True)> ' needed to avoid resource-file-names obfuscation errors, see issue #5
Public Class ProgressLoadingXhcvForm

    Private WithEvents _mainStateMachine As MainStateMachine
    Private _originalTextGrpOpeningFilesFinished As String
    Private _originalTextLblOpeningFile As String
    Private _documentsOpened As Integer
    Private _totalFiles As Integer
    Private _xhcvFileName As String

    Public Sub New()
        InitializeComponent()
    End Sub

    Public Sub New(mainstateMachine As MainStateMachine)
        InitializeComponent()
        _mainStateMachine = mainstateMachine
    End Sub

    Protected Overrides Sub OnLoad(e As EventArgs)
        MyBase.OnLoad(e)

        If Not Me.DesignMode Then
            _originalTextGrpOpeningFilesFinished = Me.grpOpeningFilesFinished.Text
            _originalTextLblOpeningFile = Me.lblOpeningFile.Text
            Me.lblOpeningFile.Text = String.Format(_originalTextLblOpeningFile, IO.Path.GetFileName(_xhcvFileName))
            Me.grpOpeningFilesFinished.Text = String.Format(_originalTextGrpOpeningFilesFinished, 0, _totalFiles)
            Me.ProgressBar1.Maximum = _totalFiles
            Me.ProgressBar1.Style = ProgressBarStyle.Blocks
            Me.ProgressBar1.Step = 1
        End If
    End Sub

    Public Shadows Function ShowDialog(owner As IWin32Window, xhcvFileName As String, totalFilesCount As Integer) As DialogResult
        _totalFiles = totalFilesCount
        _documentsOpened = 0
        _xhcvFileName = xhcvFileName
        Return MyBase.ShowDialog(owner)
    End Function

    Private Sub _mainStateMachine_DocumentOpenFinished(sender As Object, e As DocumentOpenFinshedEventArgs) Handles _mainStateMachine.DocumentOpenFinished
        PerformOpenedStep()
    End Sub

    Public Sub PerformOpenedStep()
        _documentsOpened += 1
        Me.grpOpeningFilesFinished.Text = String.Format(_originalTextGrpOpeningFilesFinished, _documentsOpened, _totalFiles)
        Me.ProgressBar1.PerformStep()
        Me.Update()
    End Sub

End Class