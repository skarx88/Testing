Imports System.ComponentModel
Imports Zuken.E3.Lib.Comparer.Topology
Imports Zuken.E3.Lib.Comparer.Topology.Documents
Imports Zuken.E3.Lib.Comparer.Topology.Wizard
Imports Zuken.E3.Lib.IO.Files.Hcv

<Reflection.Obfuscation(Feature:="renaming", ApplyToMembers:=False, Exclude:=True)> ' needed to avoid resource-file-names obfuscation errors, see issue #5
Public Class TopologyCompareWizardForm

    Private _document As EEModelsDocument
    Property _semStartCompare As New System.Threading.SemaphoreSlim(1)
    Private _lastLoadingStatusText As String

    Public Sub New()
        Application.EnableVisualStyles() ' HINT: this seems to be necessary for this form here, but i have no clue why, otherwise the form is not rendered in the correct "default" style as the other forms
        InitializeComponent()
    End Sub

    Property HcvLeft As HcvFile
        Get
            Return CompareWizard.FileLeft
        End Get
        Set
            CompareWizard.FileLeft = Value
        End Set
    End Property

    Property HcvRight As HcvFile
        Get
            Return CompareWizard.FileRight
        End Get
        Set
            CompareWizard.FileRight = Value
        End Set
    End Property

    Private Sub CompareWizardControl1_WizardStepChanged(sender As Object, e As WizardStepChangeEventArgs) Handles CompareWizard.WizardStepChanged
        Select Case e.CurrentStep
            Case WizardStepType.DirectionVertices
                toolStripStatusLabel1.Text = TopologyCompareStatusTexts.SelectDirectionVertices
            Case WizardStepType.Files
                If String.IsNullOrEmpty(_lastLoadingStatusText) Then
                    toolStripStatusLabel1.Text = TopologyCompareStatusTexts.SelectFilesToOpen
                Else
                    toolStripStatusLabel1.Text = _lastLoadingStatusText
                End If
            Case WizardStepType.StartCompare
                toolStripStatusLabel1.Text = TopologyCompareStatusTexts.StartCompareWithSelection
            Case WizardStepType.StartVertices
                toolStripStatusLabel1.Text = TopologyCompareStatusTexts.SelectStartVertices
        End Select
    End Sub

    Private Sub CompareWizardControl1_ContentChanged(sender As Object, e As EventArgs) Handles CompareWizard.ContentChanged
        btnOk.Enabled = CompareWizard.IsValid
    End Sub

    Property UseSwapDetection As Boolean
        Get
            Return CBool(CompareWizard?.UseSwapDetection)
        End Get
        Set(value As Boolean)
            If CompareWizard IsNot Nothing Then
                CompareWizard.UseSwapDetection = value
            End If
        End Set
    End Property

    Property LengthTolerance As Integer
        Get
            Return (CompareWizard?.LengthTolerance).GetValueOrDefault
        End Get
        Set(value As Integer)
            If CompareWizard IsNot Nothing Then
                CompareWizard.LengthTolerance = value
            End If
        End Set
    End Property

    Property SelectedRefLengthClass As E3.Lib.Model.LengthClass
        Get
            Return CompareWizard.SelectedRefLengthClass
        End Get
        Set(value As E3.Lib.Model.LengthClass)
            CompareWizard.SelectedRefLengthClass = value
        End Set
    End Property

    Property SelectedCompareLengthClass As E3.Lib.Model.LengthClass
        Get
            Return CompareWizard.SelectedCompareLengthClass
        End Get
        Set(value As E3.Lib.Model.LengthClass)
            CompareWizard.SelectedCompareLengthClass = value
        End Set
    End Property

    Public Function GetDocument() As CompareDocument
        Dim newComparer As New TopologyComparer(_document.ModelLeft, _document.ModelRight)
        With newComparer
            .UseSwapDetection = UseSwapDetection
            .LengthToleranceOnCompare = LengthTolerance
            .UseRefLengthClass = SelectedRefLengthClass
            .UseCompLengthClass = SelectedCompareLengthClass
            RemoveSingularVerticesFromModel(.CompareModel)
            RemoveSingularVerticesFromModel(.ReferenceModel)
            RemoveBridgeSegmentsFromModel(.CompareModel)
            RemoveBridgeSegmentsFromModel(.ReferenceModel)
        End With

#Region "More comparer properties"
        ' HINT: all other properties that can be set
        'UseShortesPath
        'ReportAdditionalPartOrientation
        'ReportProtectionChanges
        'MapNodesByName
#End Region

        Return New CompareDocument(_document.LeftFile, _document.RightFile, newComparer, True, _document.FilterContainers)
    End Function

    Private Sub RemoveParallelSegmentsFromModel(ByVal model As E3.Lib.Model.EESystemModel)
        Dim hs As HashSet(Of String) = New HashSet(Of String)()

        For Each s As E3.Lib.Model.Segment In model.Segments
            Dim ids As List(Of String) = New List(Of String)()

            For Each vt As E3.Lib.Model.Vertex In s.GetVertices().Entries()
                ids.Add(vt.Id.ToString())
            Next

            ids.Sort()
            Dim combinedId As String = String.Empty

            For Each id As String In ids
                combinedId = String.Format("{0};{1}", combinedId, id)
            Next

            If Not hs.Add(combinedId) Then
                s.Deleted = True
            End If
        Next
    End Sub

    Private Sub RemoveBridgeSegmentsFromModel(ByVal model As E3.Lib.Model.EESystemModel)
        Dim hs As HashSet(Of String) = New HashSet(Of String)()

        For Each s As E3.Lib.Model.Segment In model.Segments
            Dim del As Boolean = True

            For Each vt As E3.Lib.Model.Vertex In s.GetVertices().Entries()

                If vt.GetSegments().Count > 1 Then
                    del = False
                    Exit For
                End If
            Next

            If del Then

                For Each vt As E3.Lib.Model.Vertex In s.GetVertices().Entries()
                    vt.Deleted = True
                Next

                s.Deleted = True
            End If
        Next
    End Sub

    Private Sub RemoveSingularVerticesFromModel(ByVal model As E3.Lib.Model.EESystemModel)
        For Each vt As E3.Lib.Model.Vertex In model.Vertices
            If vt.GetSegments().Count = 0 Then
                vt.Deleted = True
            End If
        Next
    End Sub

    Private Async Sub btnOk_Click(sender As Object, e As EventArgs) Handles btnOk.Click
        Await _semStartCompare.WaitAsync
        Try
            If CompareWizard.IsValid Then
                _document = CType(Await CompareWizard.GetDocumentAsync, EEModelsDocument)
                If _document IsNot Nothing Then
                    If _document.IsOpen Then
                        DialogResult = DialogResult.OK
                        Close()
                        Return
                    Else
                        DialogResult = DialogResult.Cancel
                        Close()
                        Return
                    End If
                End If
            End If

            MessageBox.Show(Me, ErrorStrings.TopologyCompareSelectValidDocuments, My.Application.Info.Title, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Catch ex As NullReferenceException
            'TODO nothing here because the compare was cancelled/closed while loading document (Simulate this: press OK in wizard + Immediately close wizard)
        Finally
            _semStartCompare.Release()
        End Try
    End Sub

    Private Sub btnCancel_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCancel.Click
        DialogResult = DialogResult.Cancel
        Close()
    End Sub

    Protected Overrides Sub OnClosing(e As CancelEventArgs)
        If CompareWizard.IsBusy Then

            Using cancellationPendingForm As New CancellationPendingForm()
                CompareWizard.Cancel()
                Dim tsk1 As Task = Task.Factory.StartNew(Sub() Threading.SpinWait.SpinUntil(Function() Not CompareWizard.IsBusy)).ContinueWith(Sub(tsk) cancellationPendingForm.Invoke(Sub() cancellationPendingForm.Close()))
                cancellationPendingForm.ShowDialog(Me)
            End Using
        End If

        MyBase.OnClosing(e)
    End Sub

    Private Sub compareWizard_ProgressChanged(ByVal sender As Object, ByVal e As ProgressChangedEventArgs)
        Dim info As HcvDocumentUpdateState = CType(e.UserState, HcvDocumentUpdateState)
        If Not toolStripProgressBar.IsDisposed Then ' TODO: check in compareWizard the right cancellation
            toolStripProgressBar.Visible = True
            toolStripProgressBar.Value = e.ProgressPercentage

            Select Case info
                Case HcvDocumentUpdateState.Load
                    toolStripStatusLabel1.Text = TopologyCompareStatusTexts.PreparingDocuments
                    _lastLoadingStatusText = toolStripStatusLabel1.Text
                Case HcvDocumentUpdateState.Updating
                    toolStripStatusLabel1.Text = TopologyCompareStatusTexts.PreparingDocuments
                    _lastLoadingStatusText = toolStripStatusLabel1.Text
            End Select
        End If
    End Sub

    Private Sub compareWizard_DocumentOpened(ByVal sender As Object, ByVal e As EventArgs) Handles CompareWizard.DocumentOpened
        toolStripStatusLabel1.Text = "Files loaded"
        _lastLoadingStatusText = toolStripStatusLabel1.Text
        toolStripProgressBar.Visible = False
        btnOk.Enabled = True
    End Sub

    Private Sub compareWizard_DocumentOpening(ByVal sender As Object, ByVal e As EventArgs) Handles CompareWizard.DocumentOpening
        btnOk.Enabled = False
    End Sub

    Private Sub compareWizard_SelectedDocumentChanged(sender As Object, e As EventArgs) Handles CompareWizard.SelectedDocumentChanged
        btnOk.Enabled = CompareWizard.IsValid
    End Sub

End Class