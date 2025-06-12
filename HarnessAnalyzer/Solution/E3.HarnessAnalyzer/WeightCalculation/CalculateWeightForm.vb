Imports System.ComponentModel
Imports System.Reflection
Imports Infragistics.Win.UltraWinGrid
Imports Zuken.E3.App.Windows.Controls.Infragistics.WinGrid
Imports Zuken.E3.HarnessAnalyzer

<Reflection.Obfuscation(Feature:="renaming", ApplyToMembers:=False, Exclude:=True)> ' needed to avoid resource-file-names obfuscation errors, see issue #5
Public Class CalculateWeightForm

    Public Event CalculateProgress(sender As Object, e As System.ComponentModel.ProgressChangedEventArgs)
    Public Event CalculationFinished(sender As Object, e As EventArgs)

    Private _kblMapper As KblMapper
    Private _isUpdatingInternally As Boolean
    Private _isInitializing As Boolean = False
    Private _rowsInitialized As Integer = 0
    Private _initStart As New Stopwatch
    Private _warningImage As Bitmap
    Private _isCalculated As Boolean = False
    Private _rows As New BindingList(Of CalculatedWeightRow)
    Private _syncCtx As System.Threading.SynchronizationContext
    Private _isShorterThanMinimumValidatingError As Boolean
    Private _isValid As Boolean = True
    Private _summaryValue As Double
    Private WithEvents _engine As CalculateWeightEngine

    Public Sub New(selectedRows As IEnumerable, kblMapper As KblMapper)
        InitializeComponent()
        _kblMapper = kblMapper
        _warningImage = CType(My.Resources.Warning.Clone, Bitmap) ' HINT: when using the image directly within the InitializeRow-event the grid crashes (COM-Exception) when there are a lot of rows (> 40K !). Cloning the image here first before using it, avoids this behavior
        _engine = New CalculateWeightEngine(selectedRows, kblMapper)
        _syncCtx = System.Threading.SynchronizationContext.Current
    End Sub

    Protected Overrides Sub OnShown(e As EventArgs)
        MyBase.OnShown(e)

        _initStart.Restart()
        _isInitializing = True
        ugCalculatedWeights.Enabled = False
        Me.ugCalculatedWeights.Visible = True
        ugCalculatedWeights.Enabled = True
        UltraProgressBar1.Visible = False
        _isInitializing = False
        _initStart.Stop()
    End Sub

    Protected Overrides Sub OnLoad(e As EventArgs)
        MyBase.OnLoad(e)
        CalculateData()
        Me.CalculatedWeightRowBindingSource.DataSource = _rows
    End Sub

    Public Async Function CalculateDataAsync() As Task
        Await Task.Factory.StartNew(Sub() Me.CalculateData())
    End Function

    Public Sub CalculateData()
        Try
            Dim res As CalculateWeightEngine.CalculateResult = _engine.CalculateData()
            If res.Success Then
                With _rows
                    .AllowEdit = True
                    .AllowRemove = False
                    .AllowNew = False
                    .AddRange(res.Rows)
                End With
            Else
                MessageBox.Show(String.Format(DialogStrings.CalcWeightForm_UnexpError_Msg, res.Message), [Shared].MSG_BOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                Me.Close()
            End If
        Finally
            RaiseEvent CalculationFinished(Me, New EventArgs)
        End Try
    End Sub

    Private Sub OnProgressChanged(percent As Integer)
        _syncCtx.Send(New System.Threading.SendOrPostCallback(Sub() RaiseEvent CalculateProgress(Me, New System.ComponentModel.ProgressChangedEventArgs(percent, Nothing))), Nothing)
    End Sub

    ReadOnly Property RowList As BindingList(Of CalculatedWeightRow)
        Get
            Return _rows
        End Get
    End Property


    Private Sub ugCalculatedWeights_BeforeDeleteCellValue(sender As Object, e As BeforeDeleteCellValueEventArgs) Handles ugCalculatedWeights.BeforeDeleteCellValue
        e.NewValue = New Nullable(Of Double)(0)
    End Sub

    Private Sub ugCalculatedWeights_InitializeRow(sender As Object, e As InitializeRowEventArgs) Handles ugCalculatedWeights.InitializeRow
        Dim wgRow As CalculatedWeightRow = CType(e.Row.ListObject, CalculatedWeightRow)

        If wgRow.Source Is Nothing OrElse wgRow.Source.Value = WeightCalculator.CalcSource.NotFound_Copper OrElse wgRow.HasError Then
            e.Row.RowSelectorAppearance.Image = _warningImage
            e.Row.ToolTipText = String.Empty
            If wgRow.HasError Then
                e.Row.ToolTipText = wgRow.GetErrorMessage
            Else
                e.Row.ToolTipText = ErrorStrings.WeightCalc_NoSourceMsg
            End If

            If e.Row.HasParent() Then
                e.Row.CellAppearance.FontData.Italic = Infragistics.Win.DefaultableBoolean.True
            Else
                e.Row.CellAppearance.FontData.ResetItalic()
            End If
        End If

        If e.Row.Cells.Exists(NameOf(wgRow.LengthValue)) Then
            Dim lengthValueCell As UltraGridCell = e.Row.Cells(NameOf(wgRow.LengthValue))
            With lengthValueCell
                .ToolTipText = String.Empty
                .Appearance.Image = Nothing
                .Appearance.ImageHAlign = Infragistics.Win.HAlign.Default
                Dim lengthValue As Object = lengthValueCell.Value
                If lengthValue Is Nothing Then
                    .Appearance.ImageHAlign = Infragistics.Win.HAlign.Right
                    Select Case wgRow.Type
                        Case CalculatedWeightRow.WeightRowObjectType.Cable
                            If Not CurrentKBLCableLengthClassIsValid() Then
                                .Appearance.Image = _warningImage
                                .ToolTipText = String.Format(InformationHubStrings.DefCabLengthTypeNotFound_Msg, CType(Me.FindForm, MainForm).GeneralSettings.DefaultCableLengthType)
                            End If
                        Case CalculatedWeightRow.WeightRowObjectType.Wire, CalculatedWeightRow.WeightRowObjectType.Core
                            If Not CurrentKBLWireLengthClassIsValid() Then
                                .Appearance.Image = _warningImage
                                .ToolTipText = String.Format(InformationHubStrings.DefWirLengthTypeNotFound_Msg, My.Application.MainForm.GeneralSettings.DefaultWireLengthType)
                            End If
                    End Select
                End If
            End With
        End If

        If _isInitializing Then
            _rowsInitialized += 1
            If _initStart.Elapsed.TotalSeconds > 3 Then ' HINT: When the loading process takes very long (>3s we are showing a progress-bar)
                If Not UltraProgressBar1.Visible Then UltraProgressBar1.Visible = True
                Dim newValue As Integer = CInt((_rowsInitialized * 100 / RowList.Count))
                If UltraProgressBar1.Value <> newValue Then
                    UltraProgressBar1.Value = newValue
                    UltraProgressBar1.Invalidate()
                    UltraProgressBar1.Update()
                End If
            End If
        End If
    End Sub

    Private Function CurrentKBLCableLengthClassIsValid() As Boolean
        Return Not String.IsNullOrEmpty(My.Application.MainForm.GeneralSettings.DefaultCableLengthType) AndAlso _kblMapper.KBLCableLengthTypes.Any(Function(lt) lt.ToLower = My.Application.MainForm.GeneralSettings.DefaultCableLengthType.ToLower)
    End Function

    Private Function CurrentKBLWireLengthClassIsValid() As Boolean
        Return Not String.IsNullOrEmpty(My.Application.MainForm.GeneralSettings.DefaultWireLengthType) AndAlso _kblMapper.KBLWireLengthTypes.Any(Function(lt) lt.ToLower = My.Application.MainForm.GeneralSettings.DefaultWireLengthType.ToLower)
    End Function

    Private Sub ContextMenuStrip1_Opening(sender As Object, e As CancelEventArgs) Handles ContextMenuStrip1.Opening
        Me.CopyToolStripMenuItem.Enabled = ugCalculatedWeights.Selected.Cells.Count > 0 OrElse ugCalculatedWeights.Selected.Columns.Count > 0 OrElse ugCalculatedWeights.Selected.Rows.Count > 0
    End Sub

    Private Sub CopyToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CopyToolStripMenuItem.Click
        ugCalculatedWeights.PerformAction(UltraGridAction.Copy)
    End Sub

    Private Sub ugCalculatedWeights_KeyDown(sender As Object, e As KeyEventArgs) Handles ugCalculatedWeights.KeyDown
        Select Case e.KeyCode
            Case Keys.A
                If e.Control Then
                    ugCalculatedWeights.Selected.Rows.AddRange(ugCalculatedWeights.Rows.ToArray)
                End If
            Case Keys.Escape
                If Not e.Control AndAlso Not e.Alt AndAlso Not ugCalculatedWeights.Ex.ExMassEdit.IsInMassEdit AndAlso (ugCalculatedWeights.ActiveCell Is Nothing OrElse Not ugCalculatedWeights.ActiveCell.IsInEditMode) Then
                    Me.btnClose.PerformClick()
                End If
        End Select
    End Sub

    Private Sub uneAvLength_Validated(sender As Object, e As EventArgs) Handles uneDeltaLength.Validated
        If _isValid Then
            ApplyDeltaLengthToAll()
        End If
    End Sub

    Private Function GetCurrentDeltaValue() As Double
        Return If(uneDeltaLength IsNot Nothing AndAlso Not IsDBNull(uneDeltaLength.Value), CDbl(uneDeltaLength.Value), 0)
    End Function

    Private Function GetMinimumLengthValue() As Double
        Dim cRow As CalculatedWeightRow = RowList.Where(Function(row) row.LengthValue.HasValue).OrderBy(Function(row) row.LengthValue.Value).FirstOrDefault
        Return If(cRow IsNot Nothing, cRow.LengthValue.Value, 0)
    End Function

    Private Sub ApplyDeltaLengthToAll()
        _isUpdatingInternally = True

        Me.ugCalculatedWeights.BeginUpdate()


        For Each row As CalculatedWeightRow In RowList
            row.LengthDeltaValue = GetCurrentDeltaValue()
        Next
        Me.ugCalculatedWeights.EndUpdate()

        _isUpdatingInternally = False
    End Sub

    Private Sub uneAvLength_KeyDown(sender As Object, e As KeyEventArgs) Handles uneDeltaLength.KeyDown
        Select Case e.KeyCode
            Case Keys.A
                If e.Control Then uneDeltaLength.SelectAll()
            Case Keys.Enter, Keys.Return
                Dim vali As Infragistics.Win.Misc.Validation = Me.UltraValidator1.Validate(uneDeltaLength)
                If vali Is Nothing OrElse vali.IsValid Then
                    If sender Is uneDeltaLength Then ApplyDeltaLengthToAll()
                End If
        End Select
    End Sub

    Private Sub UltraValidator1_Validating(sender As Object, e As Infragistics.Win.Misc.ValidatingEventArgs) Handles UltraValidator1.Validating
        _isValid = True
        _isShorterThanMinimumValidatingError = False
        If e.Value IsNot Nothing AndAlso Not IsDBNull(e.Value) Then
            _isShorterThanMinimumValidatingError = IsTooShortLengthValidate(GetMinimumLengthValue(), GetCurrentDeltaValue())
            e.IsValid = Not _isShorterThanMinimumValidatingError
        End If
    End Sub

    Private Function IsTooShortLengthValidate(lengthValue As Double, deltaValue As Double) As Boolean
        Return lengthValue + deltaValue < 0
    End Function

    Private Sub UltraValidator1_ValidationError(sender As Object, e As Infragistics.Win.Misc.ValidationErrorEventArgs) Handles UltraValidator1.ValidationError
        _isValid = False
        If Not _isShorterThanMinimumValidatingError Then
            e.NotificationSettings.Text = String.Format(ErrorStrings.WeightCalc_MinimumLengthRequired, CType(e.Validation.Results(0).ValidationSettings.Condition, Infragistics.Win.RangeCondition).MinimumValue)
        Else
            e.NotificationSettings.Text = String.Format(ErrorStrings.WeightCalc_DeltaBiggerThanShortestLength, Math.Abs(GetMinimumLengthValue))
        End If
    End Sub

    ReadOnly Property IsCalculated As Boolean
        Get
            Return _isCalculated
        End Get
    End Property

    Private Sub ugCalculatedWeights_GetEditability(sender As Object, e As EditabilityEventArgs) Handles ugCalculatedWeights.GetEditability
        If e.Cell.Column.Key = "CsaSqMm" Then
            Dim row As CalculatedWeightRow = CType(e.Cell.Row.ListObject, CalculatedWeightRow)
            e.CanEdit = Not row.IsCable
        End If
    End Sub

    Private Sub uneDeltaLength_Validating(sender As Object, e As CancelEventArgs) Handles uneDeltaLength.Validating
        Me.UltraValidator1.Validate(uneDeltaLength)
    End Sub

    Private Sub ugCalculatedWeights_BeforeCellUpdate(sender As Object, e As BeforeCellUpdateEventArgs) Handles ugCalculatedWeights.BeforeCellUpdate
        If ugCalculatedWeights.Ex.ExMassEdit.IsInMassEdit Then
            e.Cancel = Not BeforeCellsUpdate(e.NewValue, New UltraGridCell() {e.Cell})
        End If
    End Sub

    Private Sub ugCalculatedWeights_BeforeAnyCellUpdate(sender As Object, e As CancellableCellsEventArgs) Handles ugCalculatedWeights.BeforeAnyCellUpdate
        If Not e.IsBulkEditChange Then
            e.Cancel = Not BeforeCellsUpdate(e.NewValue, e.Cells)
        End If
    End Sub

    Private Function BeforeCellsUpdate(newValue As Object, cells As IEnumerable(Of UltraGridCell)) As Boolean
        Dim errCell As UltraGridCell = Nothing
        If Not LengthsCanBeUpdated(newValue, cells, errCell) Then
            UltraValidatorToValidateUltraGrid.ShowToolTipOnCell(errCell, String.Format(ErrorStrings.WeightCalc_LengthResultWouldBeZeroWithGivenLength, Math.Abs(GetCurrentDeltaValue)), String.Empty, Infragistics.Win.ToolTipImage.Error)
            Return False
        End If
        Return True
    End Function

    Private Function LengthsCanBeUpdated(newValue As Object, cells As IEnumerable(Of UltraGridCell), Optional ByRef errorCell As UltraGridCell = Nothing) As Boolean
        Dim myNewValue As Double = If(newValue IsNot Nothing, CDbl(newValue), 0)
        For Each cell As UltraGridCell In cells
            If cell.Column.Key = "LengthValue" Then
                Dim row As CalculatedWeightRow = CType(cell.Row.ListObject, CalculatedWeightRow)
                Dim rowLengthValue As Double = If(row.LengthValue IsNot Nothing, row.LengthValue.Value, 0)
                If IsTooShortLengthValidate(myNewValue, If(row.LengthDeltaValue IsNot Nothing, row.LengthDeltaValue.Value, 0)) Then
                    errorCell = cell
                    Return False
                End If
            End If
        Next
        Return True
    End Function

    Private Sub ugCalculatedWeights_SummaryValueChanged(sender As Object, e As SummaryValueChangedEventArgs) Handles ugCalculatedWeights.SummaryValueChanged
        If e.SummaryValue.SummarySettings.SourceColumn.Key = NameOf(CalculatedWeightRow.Total) Then
            _summaryValue = CDbl(e.SummaryValue.Value)
        End If
    End Sub

    Private Sub _engine_CalculateProgress(sender As Object, e As ProgressChangedEventArgs) Handles _engine.CalculateProgress
        OnProgressChanged(e.ProgressPercentage)
    End Sub

    <Obfuscation(Feature:="renaming")>
    ReadOnly Property SummaryValue As Double
        Get
            Return _summaryValue
        End Get
    End Property

End Class


