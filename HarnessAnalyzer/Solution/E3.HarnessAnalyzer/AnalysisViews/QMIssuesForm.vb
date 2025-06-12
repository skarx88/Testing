Imports System.IO

<Reflection.Obfuscation(Feature:="renaming", ApplyToMembers:=False, Exclude:=True)> ' needed to avoid resource-file-names obfuscation errors, see issue #5
Public Class QMIssuesForm

    Private _issueReportFile As IssueReporting.ReportFile
    Private _openQMFile As New System.Windows.Forms.OpenFileDialog()
    Friend _kblMapper As KblMapper

    Public Sub New(doc As DocumentForm)
        InitializeComponent()
        Me.Text = AnalysisFormStrings.ShowQMIssues_Caption
        _kblMapper = doc.KBL
        Me.ugbQM.Visible = True
        Me.txtQMFile.Text = My.Settings.LastSelectedStampFile

        InitializeColorRanges()
    End Sub

    Public Property ActiveObjects As New List(Of String)

    Private Sub InitializeColorRanges()
        Dim list As New List(Of KeyValuePair(Of UInteger, Tuple(Of Object, Integer)))

        For Each rangeAndValue As String() In My.Settings.ColorRangeValues.Split(" "c).Select(Function(rv) rv.Split(";"c))
            If (rangeAndValue.Length > 1) Then
                Dim colorValue As Integer = Integer.Parse(rangeAndValue(1), Globalization.CultureInfo.InvariantCulture)

                If (Not String.IsNullOrEmpty(rangeAndValue(0)) AndAlso IsNumeric(rangeAndValue(0))) Then
                    Dim nVal As UInteger = CUInt(rangeAndValue(0))
                    list.Add(New KeyValuePair(Of UInteger, Tuple(Of Object, Integer))(nVal, New Tuple(Of Object, Integer)(nVal, colorValue)))
                Else
                    list.Add(New KeyValuePair(Of UInteger, Tuple(Of Object, Integer))(UInteger.MaxValue, New Tuple(Of Object, Integer)(DBNull.Value, colorValue)))
                End If
            End If
        Next

        list = list.OrderBy(Function(kv) kv.Key).ToList

        For i As Integer = list.Count - 1 To 0 Step -1
            Select Case i
                Case 4
                    colorRange5.ResetMinValue()
                    colorRange5.Value = list(i).Value.Item1
                    ucp5.Value = System.Drawing.Color.FromArgb(list(i).Value.Item2)
                Case 3
                    colorRange4.ResetMinValue()
                    colorRange4.Value = list(i).Value.Item1
                    ucp4.Value = System.Drawing.Color.FromArgb(list(i).Value.Item2)
                Case 2
                    colorRange3.ResetMinValue()
                    colorRange3.Value = list(i).Value.Item1
                    ucp3.Value = System.Drawing.Color.FromArgb(list(i).Value.Item2)
                Case 1
                    colorRange2.ResetMinValue()
                    colorRange2.Value = list(i).Value.Item1
                    ucp2.Value = System.Drawing.Color.FromArgb(list(i).Value.Item2)
                Case 0
                    colorRange1.ResetMinValue()
                    colorRange1.Value = list(i).Value.Item1
                    ucp1.Value = System.Drawing.Color.FromArgb(list(i).Value.Item2)
            End Select
        Next
    End Sub

    Friend Function GetColorRanges() As IssueReporting.ColorRangeCollection
        Dim coll As New IssueReporting.ColorRangeCollection

        Dim colRange1 As UInteger = If(colorRange1.Value Is Nothing OrElse IsDBNull(colorRange1.Value), UInteger.MaxValue, CUInt(colorRange1.Value))
        Dim colRange2 As UInteger = If(colorRange2.Value Is Nothing OrElse IsDBNull(colorRange2.Value), UInteger.MaxValue, CUInt(colorRange2.Value))
        Dim colRange3 As UInteger = If(colorRange3.Value Is Nothing OrElse IsDBNull(colorRange3.Value), UInteger.MaxValue, CUInt(colorRange3.Value))
        Dim colRange4 As UInteger = If(colorRange4.Value Is Nothing OrElse IsDBNull(colorRange4.Value), UInteger.MaxValue, CUInt(colorRange4.Value))
        Dim colRange5 As UInteger = If(colorRange5.Value Is Nothing OrElse IsDBNull(colorRange5.Value), UInteger.MaxValue, CUInt(colorRange5.Value))

        If colorRange1.Enabled Then coll.AddNew(colRange1, CType(Me.ucp1.Value, Color))
        If colorRange2.Enabled Then coll.AddNew(colRange2, CType(Me.ucp2.Value, Color))
        If colorRange3.Enabled Then coll.AddNew(colRange3, CType(Me.ucp3.Value, Color))
        If colorRange4.Enabled Then coll.AddNew(colRange4, CType(Me.ucp4.Value, Color))
        If colorRange5.Enabled Then coll.AddNew(colRange5, CType(Me.ucp5.Value, Color))

        Return coll
    End Function

    Private Sub colorRange1_ValueChanged(sender As Object, e As EventArgs) Handles colorRange1.ValueChanged
        colorRange2.Enabled = Not IsDBNull(colorRange1.Value)
        colorRange3.Enabled = colorRange2.Enabled AndAlso Not IsDBNull(colorRange1.Value) AndAlso Not IsDBNull(colorRange2.Value)
        colorRange4.Enabled = colorRange3.Enabled AndAlso Not IsDBNull(colorRange1.Value) AndAlso Not IsDBNull(colorRange3.Value)
        colorRange5.Enabled = colorRange4.Enabled AndAlso Not IsDBNull(colorRange1.Value) AndAlso Not IsDBNull(colorRange4.Value)

        If Not IsDBNull(colorRange1.Value) Then
            colorRange2.MinValue = CInt(colorRange1.Value) + 1
            colorRange3.MinValue = CInt(colorRange1.Value) + 2
            colorRange4.MinValue = CInt(colorRange1.Value) + 3
            colorRange5.MinValue = CInt(colorRange1.Value) + 4
        End If
    End Sub

    Private Sub colorRange2_ValueChanged(sender As Object, e As EventArgs) Handles colorRange2.ValueChanged
        colorRange3.Enabled = Not IsDBNull(colorRange2.Value)
        colorRange4.Enabled = colorRange3.Enabled AndAlso Not IsDBNull(colorRange2.Value) AndAlso Not IsDBNull(colorRange3.Value)
        colorRange5.Enabled = colorRange4.Enabled AndAlso Not IsDBNull(colorRange2.Value) AndAlso Not IsDBNull(colorRange4.Value)

        If IsDBNull(colorRange2.Value) Then
            colorRange1.MaxValue = Integer.MaxValue
        Else
            colorRange1.MaxValue = CInt(colorRange2.Value) - 1

            colorRange3.MinValue = CInt(colorRange2.Value) + 1
            colorRange4.MinValue = CInt(colorRange2.Value) + 2
            colorRange5.MinValue = CInt(colorRange2.Value) + 3
        End If
    End Sub

    Private Sub colorRange3_ValueChanged(sender As Object, e As EventArgs) Handles colorRange3.ValueChanged
        colorRange4.Enabled = Not IsDBNull(colorRange3.Value)
        colorRange5.Enabled = colorRange4.Enabled AndAlso Not IsDBNull(colorRange3.Value) AndAlso Not IsDBNull(colorRange4.Value)

        If IsDBNull(colorRange3.Value) Then
            colorRange2.MaxValue = Integer.MaxValue
        Else
            colorRange2.MaxValue = CInt(colorRange3.Value) - 1

            colorRange4.MinValue = CInt(colorRange3.Value) + 1
            colorRange5.MinValue = CInt(colorRange3.Value) + 2
        End If
    End Sub

    Private Sub colorRange4_ValueChanged(sender As Object, e As EventArgs) Handles colorRange4.ValueChanged
        colorRange5.Enabled = Not IsDBNull(colorRange4.Value)

        If IsDBNull(colorRange4.Value) Then
            colorRange3.MaxValue = Integer.MaxValue
        Else
            colorRange3.MaxValue = CInt(colorRange4.Value) - 1

            colorRange5.MinValue = CInt(colorRange4.Value) + 1
        End If
    End Sub

    Private Sub colorRange5_ValueChanged(sender As Object, e As EventArgs) Handles colorRange5.ValueChanged
        If IsDBNull(colorRange5.Value) Then
            colorRange4.MaxValue = Integer.MaxValue
        Else
            colorRange4.MaxValue = CInt(colorRange5.Value) - 1
        End If
    End Sub

    Private Sub CreateTestStampsOnObjects(fileName As String)
        Dim countStr As String = InputBox("How many issues per object?", [Shared].MSG_BOX_TITLE, "4")

        If (Not String.IsNullOrEmpty(countStr)) Then
            Dim count As Integer

            If (Not Integer.TryParse(countStr, count)) Then
                If MessageBoxEx.ShowError("Invalid numeric value!", MessageBoxButtons.RetryCancel) = System.Windows.Forms.DialogResult.Retry Then
                    CreateTestStampsOnObjects(fileName)
                End If

                Return
            End If

            If (count <= 0) Then
                If MessageBoxEx.ShowError("Please enter a value >0 !", MessageBoxButtons.RetryCancel) = System.Windows.Forms.DialogResult.Retry Then
                    CreateTestStampsOnObjects(fileName)
                End If

                Return
            End If

            With My.Application.MainForm.ActiveDocument.ActiveDrawingCanvas.vdCanvas.BaseControl
                Dim i As Integer = 0
                Dim ii As UInteger = 0
                Dim reportFile As New IssueReporting.ReportFile
                Dim kblMapper As KblMapper = My.Application.MainForm.ActiveDocument.KBL
                reportFile.HarnessPartNumber = kblMapper.HarnessPartNumber

                Dim occurrences As New List(Of IKblOccurrence)
                occurrences.AddRange(kblMapper.GetObjects(Of Cavity_occurrence))
                occurrences.AddRange(kblMapper.GetObjects(Of Connector_occurrence))
                occurrences.AddRange(kblMapper.GetObjects(Of Wire_occurrence))
                occurrences.AddRange(kblMapper.GetObjects(Of Segment))

                For Each occ As IKblOccurrence In occurrences
                    If (My.Application.MainForm.ActiveDocument.ActiveDrawingCanvas.GroupMapper.ContainsKey(occ.SystemId)) Then
                        ii += 1UI

                        For c As Integer = 1 To count
                            i += 1
                            reportFile.Issues.AddNew(i.ToString, [Lib].Schema.Kbl.Utils.GetUserId(occ), "description" & i.ToString, "Test" & c, ii * 2UI)
                        Next
                    End If
                Next

                reportFile.SaveAs(fileName)
            End With
        End If
    End Sub

    Private Function GetSelectedIssuesAsKblIds() As List(Of String)
        Dim kblIds As New List(Of String)

        For Each iss As IssueReporting.Issue In _issueReportFile.Issues
            If (_kblMapper.KBLIdMapper.ContainsKey(iss.ObjectReference)) Then
                kblIds.AddRange(_kblMapper.KBLIdMapper(iss.ObjectReference))
            End If
        Next

        Return kblIds.Distinct.ToList
    End Function

    Private Function LoadIssueReportFile(fileName As String) As Boolean
        Dim fi As New IO.FileInfo(fileName)
        If (fi.Exists) Then
            Try
                _issueReportFile = IssueReporting.ReportFile.LoadFrom(fi.FullName)

                My.Settings.LastSelectedStampFile = fi.FullName

                ActiveObjects.Clear()
                ActiveObjects.AddRange(GetSelectedIssuesAsKblIds)

                Return True
            Catch ex As IOException
                If (MessageBoxEx.ShowError(String.Format(ErrorStrings.QM_ErrorAccessingIssueReportFile, fi.FullName), MessageBoxButtons.RetryCancel) = System.Windows.Forms.DialogResult.Retry) Then
                    Return LoadIssueReportFile(fileName)
                End If
            Catch ex As Exception
#If CONFIG = "Debug" Or DEBUG Then
                ex.ShowMessageBox(String.Format(ErrorStrings.QM_InvalidIssueReportFile, fi.FullName) & vbCrLf & ex.Message)
#Else
                MessageBox.Show(String.Format(ErrorStrings.QM_InvalidIssueReportFile, fi.FullName), [Shared].MSG_BOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error)
#End If
            End Try
        Else
            MessageBox.Show(String.Format(ErrorStrings.QM_IssueReportFileDoesNotExist, fi.FullName), [Shared].MSG_BOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If

        Return False
    End Function

    Private Sub txtQMFile_EditorButtonClick(sender As Object, e As Infragistics.Win.UltraWinEditors.EditorButtonEventArgs) Handles txtQMFile.EditorButtonClick
        If (_openQMFile.ShowDialog(Me) = System.Windows.Forms.DialogResult.OK) Then
            Me.txtQMFile.Text = _openQMFile.FileName
        End If
    End Sub

    Private Sub txtQMFile_TextChanged(sender As Object, e As EventArgs) Handles txtQMFile.TextChanged
        Me.btnView.Enabled = Not String.IsNullOrWhiteSpace(txtQMFile.Text)
    End Sub

    Private Sub btnView_Click(sender As Object, e As EventArgs) Handles btnView.Click
        Me.DialogResult = DialogResult.OK

        If (Not String.IsNullOrEmpty(txtQMFile.Text)) Then
            If (Not LoadIssueReportFile(txtQMFile.Text)) Then
                Me.DialogResult = System.Windows.Forms.DialogResult.None

                Return
            End If

            Dim currPartNumber As String = My.Application.MainForm.ActiveDocument.KBL.HarnessPartNumber

            If (Not String.IsNullOrEmpty(_issueReportFile.HarnessPartNumber)) AndAlso (_issueReportFile.HarnessPartNumber <> currPartNumber) Then
                If (MessageBox.Show(String.Format(ErrorStrings.QM_IssuesReportFileNotMatching, _issueReportFile.HarnessPartNumber, currPartNumber), [Shared].MSG_BOX_TITLE, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) = System.Windows.Forms.DialogResult.No) Then
                    Me.DialogResult = System.Windows.Forms.DialogResult.None

                    Return
                End If
            End If

            If (_issueReportFile.Issues.Count > 0) Then
                Dim colRanges As IssueReporting.ColorRangeCollection = GetColorRanges()
                Dim maxNoOfOcc As UInteger = 0
                Dim IssuesInDoc As List(Of IssueReporting.Issue) = _issueReportFile.Issues.Where(Function(iss) IssueReporting.EditIssuesForm.IsInDocument(iss, True)).ToList
                Dim maxIssue As IssueReporting.Issue = IssuesInDoc.OrderByDescending(Function(iss) iss.NofOccurrences).FirstOrDefault

                If (maxIssue IsNot Nothing) Then maxNoOfOcc = maxIssue.NofOccurrences
                If (maxNoOfOcc > colRanges.Max) Then
                    MessageBox.Show(String.Format(ErrorStrings.QM_MaximumRangeColorTooLow, maxNoOfOcc), [Shared].MSG_BOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Me.DialogResult = System.Windows.Forms.DialogResult.None
                Else
                    My.Settings.ColorRangeValues = String.Concat(colorRange1.Value, ";", CType(ucp1.Value, Color).ToArgb, " ",
                                                                 colorRange2.Value, ";", CType(ucp2.Value, Color).ToArgb, " ",
                                                                 colorRange3.Value, ";", CType(ucp3.Value, Color).ToArgb, " ",
                                                                 colorRange4.Value, ";", CType(ucp4.Value, Color).ToArgb, " ",
                                                                 colorRange5.Value, ";", CType(ucp5.Value, Color).ToArgb)
                End If
            Else
                MessageBox.Show(ErrorStrings.QM_IssueReportFileEmpty, [Shared].MSG_BOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Stop)
                Me.DialogResult = System.Windows.Forms.DialogResult.None
            End If
        Else
            MessageBox.Show(ErrorStrings.QM_SelectIssueReportFile, [Shared].MSG_BOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Me.DialogResult = System.Windows.Forms.DialogResult.None
        End If

        Return
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Me.DialogResult = DialogResult.Cancel
    End Sub

    ReadOnly Property IssueReportFile As IssueReporting.ReportFile
        Get
            Return _issueReportFile
        End Get
    End Property

    Private Sub QMIssuesForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Icon = My.Resources.AnalysisShowQMIssues
    End Sub

End Class