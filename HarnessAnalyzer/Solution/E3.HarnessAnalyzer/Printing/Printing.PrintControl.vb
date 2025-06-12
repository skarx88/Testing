Imports System.Collections.Specialized
Imports System.ComponentModel
Imports System.Drawing.Printing
Imports System.Printing
Imports System.Reflection
Imports Infragistics.Win
Imports Infragistics.Win.UltraWinEditors

Namespace Printing

#Region "Hint"
    ' HH: Interaction between gui and objects should be totally reworked because we made so much changes related to async and then after that to new printing farmwork (to avoid lags and AppStyle problems!).
    ' The new printing framework is much faster/better then the old one from System.Drawing.Printing so that this old interaction structure which was build for System.Drawing.Printing is complete obsolete
    ' I tried to implemente a better "PrintBridge" that wraps all interaction stuff between System.Printing and System.Drawing.Printing into objects where to user musn't care about conversions, etc.
    ' This implementation is limited to the business logic without gui interaction. The implementation in the control/gui
    ' is another thing. Here it tried to implement the new structure for System.Printing / PrintBridge to the current structure and business logic which doesn't look very good now!
    ' For example: there is some now async implementations that are no longer needed due to the fast behavior of System.Printing.
    ' A other thing is that we rely at the end - when it comes to real printing - to System.Drawings.Printing. Which is not good. The complete new Printing-API should be used!
    ' At my state of implementation this whole replacement was not possible because it would take much longer to replace the whole gui and vdraw stuff (with better code) !
    ' Take the code with a salty grain, this is not the best implementation now, but the best implementation that was possible under this circumstances.
    ' ------- 
    ' REMARKS: The whole printing is a lot faster in x86 (only for our own erlangen-printer - it seems to depend also on the printer driver that is used !)
#End Region

    <Reflection.Obfuscation(Feature:="renaming", ApplyToMembers:=False, Exclude:=True)> ' needed to avoid resource-file-names obfuscation errors, see issue #5
    Public Class PrintControl
        Implements IMessageFilter

        Public Event AfterViewChanged(sender As Object, e As EventArgs)
        Public Event AfterPrintAreaChanged(sender As Object, e As EventArgs)
        Public Event AfterDrawingUnitsChanged(sender As Object, e As EventArgs)
        Public Event AfterPrinterUnitsChanged(sender As Object, e As EventArgs)
        Public Event BeforeUpdatePrintPreview(sender As Object, e As BeforeUpdatePrintPreviewEventArgs)
        Public Event ScaleToFitChanged(sender As Object, e As EventArgs)
        Public Event OnlyMarginsChanged(sender As Object, e As EventArgs)
        Public Event CenterToPaperChanged(sender As Object, e As EventArgs)
        Public Event CustomPaperSizeChanged(sender As Object, e As EventArgs)
        Public Event AfterUpdatePaperSizes(sender As Object, e As EventArgs)
        Public Event Picking(sender As Object, e As EventArgs)
        Public Event Close(sender As Object, e As EventArgs)
        <Obfuscation(Feature:="renaming")>
        Public Event Printing(sender As Object, e As PrintPageEventArgs)
        Public Event EndPrint(sender As Object, e As PrintEventArgs)
        Public Event AbortingPreviewGenerate(sender As Object, e As EventArgs)

        Private Shared _isFetchingPrinterSystemSettings As Boolean ' HINT: global block: we do not show multiple system printer dialogs at the same time (blocks showing printer dialogs over all printControls in the application)
        Private Shared _lastStoredPrinterSettings As New StringCollection

        Private WithEvents _printDocument As New PrintDocumentEx
        Private WithEvents _printers As PrintersCollection

        Private _cancel As New Threading.CancellationTokenSource
        Private _internalValueSet As Boolean = True
        <Obfuscation(Feature:="renaming")>
        Private _initialized As Boolean = False
        Private _isGeneratingPepared As Boolean
        Private _isPrinting As Boolean = False
        Private _canBeClosed As Boolean = True
        Private _updatePreviewEnabled As Boolean = True
        Private _isGeneratingPreview As Boolean
        Private _previewGenerated As Boolean = False

        Private Const MAXSIZE_FOXIT_INCH As Single = 128
        Private Const MAXSIZE_MICROSOFT_INCH As Single = 20

        Public Sub New()
            InitializeComponent()
            _internalValueSet = False
            Me.ctrlPrintPreview.Document = _printDocument
            Application.AddMessageFilter(Me)
        End Sub

        Public Sub Initialize(Optional printAreaChangeable As Boolean = True)
            Me.ugbPrintArea.Visible = printAreaChangeable
            InitializeInternal()
            If Me.Visible Then
                Me.UpdatePrintPreview(False)
            End If
        End Sub

        Private Sub GeneratePreviewCore(regenerate As Boolean)
            Try
                ctrlPrintPreview.GeneratePreview(regenerate)
                _previewGenerated = True
            Catch ex As InvalidOperationException
                'do nothing, thread aborted
            End Try
        End Sub

        Public ReadOnly Property IsGeneratingPreview As Boolean
            Get
                Return _isGeneratingPreview
            End Get
        End Property
        Public ReadOnly Property ViewCurrentValue As String
            Get
                Return uceViews.Value.ToString
            End Get
        End Property
        Public ReadOnly Property ViewSelectedIndex As Integer
            Get
                Return uceViews.SelectedIndex
            End Get
        End Property
        Private Function PrepareGeneratePreview() As Boolean  'HINT: Is maybe Async
            If Not _isGeneratingPepared AndAlso Me.ctrlPrintPreview.Document IsNot Nothing Then
                _isGeneratingPepared = True
                Me.uckOnlyMargins.Visible = False
                Me.ctrlPrintPreview.Visible = False
                Me.lblLoadingPreview.Visible = True
                Me.btnPrint.Enabled = False
                Me.btnUpdatePreview.Enabled = False
                Me.Update()
                Return True
            End If
            Return False
        End Function

        Private Sub FinalizeGeneratePreview()
            If Not Me.IsDisposed Then
                If _isGeneratingPepared Then
                    If Me.ctrlPrintPreview.Document IsNot Nothing Then
                        Me.uckOnlyMargins.Visible = True
                        Me.lblLoadingPreview.Visible = False
                        Me.ctrlPrintPreview.Visible = True
                        Me.btnPrint.Enabled = True
                        Me.btnUpdatePreview.Enabled = True
                    End If
                    _isGeneratingPepared = False
                End If
                SettingsState.SetEnabled(Me, True)
                Me.btnPick.Enabled = Me.uosPrintArea.CheckedIndex = 1
                If CurrentPrinter IsNot Nothing AndAlso CurrentPrinter.Settings IsNot Nothing AndAlso CurrentPrinter.Settings.Page IsNot Nothing Then
                    Me.ucePaperSizes.Enabled = Not CurrentPrinter.Settings.Page.IsCustom
                End If
            End If
        End Sub

        Private Function UpdateInstalledPrintersBox() As Boolean
            With Me.uceInstalledPrinters
                If Not .Items.Cast(Of ValueListItem).Select(Function(item) CType(item.DataValue, Printer)).SequenceEqual(_printers) Then
                    .BeginUpdate()
                    .SortStyle = ValueListSortStyle.Ascending
                    Try
                        .Items.Clear()
                        For Each pItem As Printer In _printers
                            With .Items.Add(pItem, pItem.Name)
                                If pItem.IsDefault Then
                                    .Appearance.Image = My.Resources.PrinterDefault
                                Else
                                    .Appearance.Image = My.Resources.Printer
                                End If
                            End With
                        Next
                    Finally
                        .EndUpdate()
                    End Try
                    Return True
                End If
            End With
            Return False
        End Function

        Private Function TryLoadLastSetting() As Printer
            If _lastStoredPrinterSettings IsNot Nothing Then
                For Each sett As String In _lastStoredPrinterSettings
                    Dim sp As String() = sett.Split(";"c)
                    If sp(0) = Me.Name Then
                        Return Printer.ReadFromString(sp(1))
                    End If
                Next
            End If
            Return Nothing
        End Function

        Property Printers As PrintersCollection
            Get
                Return _printers
            End Get
            Set(value As PrintersCollection)
                If Not _printers Is value Then
                    _printers = value
                End If
            End Set
        End Property
        Private Function GetExceptionMessage(ex As Exception) As String
            If TypeOf ex Is AggregateException Then
                Return CType(ex, AggregateException).InnerException.Message
            Else
                Return ex.Message
            End If
        End Function
        Private Function UpdatePrinters() As IEnumerable(Of ValueListItem)
            If _printers Is Nothing Then
                Throw New ArgumentException(String.Format("Property ""Printers"" was not set on control {0}: {1}", Me.GetType.Name, Me.Name))
            End If
            UpdateInstalledPrintersBox()
            Return Me.uceInstalledPrinters.Items.Cast(Of ValueListItem)
        End Function

        Private Sub InitializeInternal()
            _cancel = New Threading.CancellationTokenSource
            With Me.uckCustomPaperSize
                Me.ucePaperSizes.Enabled = Not .Checked
                Me.uneHeight.Enabled = .Checked
                Me.uneWidth.Enabled = .Checked
            End With

            _initialized = False
            Try
                Dim lastPrinterItem As ValueListItem = UpdatePrinters().LastOrDefault

                If Not _cancel.IsCancellationRequested Then
                    Dim startPrinterToWork As Printer = If(lastPrinterItem IsNot Nothing, CType(lastPrinterItem.DataValue, Printer), Nothing)
                    If _printers.Default IsNot Nothing Then
                        startPrinterToWork = _printers.Default
                    End If
                    If startPrinterToWork IsNot Nothing Then
                        TryLoadStoredLastSettingTo(startPrinterToWork)
                        UpdatePaperWidthControlsFrom(startPrinterToWork.Settings.Page)
                        UpdateIsCustomControlsFrom(startPrinterToWork)
                    End If
                    Me.CurrentPrinter = startPrinterToWork
                End If
            Finally
                Me.uckOnlyMargins.Enabled = True
                Me.tblSettings.Enabled = True
                Me.uceInstalledPrinters.Enabled = True
                Me.btnPrinterSettings.Enabled = True
                _initialized = True
            End Try
        End Sub

        Private Sub UpdateIsCustomControlsFrom(printer As Printer)
            If printer IsNot Nothing Then
                With printer
                    If .Settings IsNot Nothing AndAlso .Settings.Page IsNot Nothing Then
                        Me.uckCustomPaperSize.Checked = .Settings.Page.IsCustom
                        Me.ucePaperSizes.Enabled = Not .Settings.Page.IsCustom
                    End If
                End With
            End If
        End Sub

        Private Sub TryLoadStoredLastSettingTo(ByRef toPrinter As Printer)
            Dim loadedPrinter As Printer = TryLoadLastSetting()
            If loadedPrinter IsNot Nothing AndAlso toPrinter IsNot Nothing Then
                If _printers.Contains(loadedPrinter.FullName) Then
                    toPrinter = _printers(loadedPrinter.FullName)
                End If
                toPrinter.Settings = loadedPrinter.Settings
            End If
        End Sub

        Private Sub StoreCurrentToMyLastPaperSizeName()
            If Me.CurrentPrinter IsNot Nothing Then
                Dim newColl As New Concurrent.ConcurrentBag(Of String)
                If _lastStoredPrinterSettings IsNot Nothing Then
                    For Each settinfo As String In _lastStoredPrinterSettings.Cast(Of String).ToArray
                        If settinfo.Split(";"c).First <> Me.Name Then
                            newColl.Add(settinfo)
                        End If
                    Next
                End If

                newColl.Add(String.Join(";", Me.Name, CurrentPrinter.SaveAsCompressedString))

                _lastStoredPrinterSettings = New StringCollection
                _lastStoredPrinterSettings.AddRange(newColl.ToArray)
            End If
        End Sub

        Property CurrentPrinter As Printer
            Get
                If Me.uceInstalledPrinters.SelectedItem IsNot Nothing Then
                    Return CType(Me.uceInstalledPrinters.SelectedItem.DataValue, Printer)
                End If
                Return Nothing
            End Get
            Set(value As Printer)
                If value IsNot Nothing Then
                    With Me.uceInstalledPrinters
                        Dim printerItem As ValueListItem = .Items.Cast(Of ValueListItem).Where(Function(item) item.DataValue Is value).FirstOrDefault
                        If printerItem IsNot Nothing Then
                            .SelectedItem = printerItem
                        End If
                    End With
                Else
                    Me.uceInstalledPrinters.SelectedItem = Nothing
                End If
            End Set
        End Property

        Private Sub UpdatePrinterOfDocument()
            _printDocument.Printer = CurrentPrinter
        End Sub

        Private Function UpdatePaperSizeUserControls() As Boolean
            Dim oldInternal As Boolean = _internalValueSet
            Dim oldEnabled As Boolean = Me.ucePaperSizes.Enabled

            _internalValueSet = True
            Try
                With Me.ucePaperSizes
                    .Enabled = False
                    .SelectedItem = Nothing ' HINT: will show "Fetching PaperSizes" in the control to give the user some informations whats happening so long
                    .Items.Clear()
                    .Refresh()
                    .BeginUpdate()
                    .SortStyle = ValueListSortStyle.Ascending

                    If CurrentPrinter IsNot Nothing Then
                        .Items.AddRange(CurrentPrinter.Papers.Select(Function(pItem) New ValueListItem(pItem, pItem.Name)).ToArray)
                        With CurrentPrinter
                            If .Settings IsNot Nothing Then
                                Me.UpdateIsCustomControlsFrom(CurrentPrinter)
                                If Me.ucePaperSizes.SelectedItem Is Nothing AndAlso .Settings.Page IsNot Nothing Then
                                    Me.ucePaperSizes.SelectedItem = GetPageComboBoxItemOrA4(.Settings.Page.Name)
                                End If
                            End If
                        End With
                        If .SelectedItem Is Nothing Then .SelectedIndex = 0
                    End If
                End With
            Catch ex As OperationCanceledException
                Return False
            Finally
                ucePaperSizes.Enabled = oldEnabled
                ucePaperSizes.EndUpdate(True)
                _internalValueSet = oldInternal
            End Try

            Return True
        End Function

        Private Function GetPageComboBoxItemOrA4(pagetype As PageMediaSizeName) As ValueListItem
            With Me.ucePaperSizes
                Dim a4Item As ValueListItem = Nothing
                For Each item As ValueListItem In Me.ucePaperSizes.Items
                    Dim pageEx As PageEx = CType(item.DataValue, PageEx)
                    If pageEx.PageType = pagetype Then
                        Return item
                    ElseIf pageEx.PageType = PageMediaSizeName.ISOA4 Then
                        a4Item = item
                    End If
                Next
                Return a4Item
            End With
        End Function

        Private Function GetPageComboBoxItemOrA4(pageName As String) As ValueListItem
            With Me.ucePaperSizes
                Dim a4Item As ValueListItem = Nothing
                For Each item As ValueListItem In Me.ucePaperSizes.Items
                    Dim pageEx As PageEx = CType(item.DataValue, PageEx)
                    If pageEx.Name = pageName Then
                        Return item
                    ElseIf pageEx.PageType = PageMediaSizeName.ISOA4 Then
                        a4Item = item
                    End If
                Next
                Return a4Item
            End With
        End Function

        Private Sub UpdatePrintMarginsUserControls()
            If CurrentPrinter IsNot Nothing Then
                If CurrentPrinter.Settings.Page Is Nothing Then
                    UpdatePageSettings(False)
                End If
                UpdatePrintMarginsUserControlsFrom(CurrentPrinter.Settings.Page)
            End If
        End Sub

        Private Sub UpdatePrintMarginsUserControlsFrom(page As PageEx)
            Dim oldInternal As Boolean = _internalValueSet
            _internalValueSet = True
            If page IsNot Nothing Then
                With page.Margins
                    If Not _cancel.IsCancellationRequested Then
                        Me.uneBottomMargin.Value = If(Me.uosUnits.CheckedIndex = 0, .Bottom.Inch, .Bottom.Millimeter)
                    End If
                    If Not _cancel.IsCancellationRequested Then
                        Me.uneLeftMargin.Value = If(Me.uosUnits.CheckedIndex = 0, .Left.Inch, .Left.Millimeter)
                    End If
                    If Not _cancel.IsCancellationRequested Then
                        Me.uneRightMargin.Value = If(Me.uosUnits.CheckedIndex = 0, .Right.Inch, .Right.Millimeter)
                    End If
                    If Not _cancel.IsCancellationRequested Then
                        Me.uneTopMargin.Value = If(Me.uosUnits.CheckedIndex = 0, .Top.Inch, .Top.Millimeter)
                    End If
                End With
            End If
            _internalValueSet = oldInternal
        End Sub

        Property ScaleToFit As Boolean
            Get
                Return uckScaleToFit.Checked
            End Get
            Set(value As Boolean)
                uckScaleToFit.Checked = value
            End Set
        End Property

        Property DrawingUnits As Object
            Get
                Return uneDrawingUnits.Value
            End Get
            Set(value As Object)
                Dim oldInternal As Boolean = _internalValueSet
                _internalValueSet = True
                uneDrawingUnits.Value = value
                _internalValueSet = oldInternal
            End Set
        End Property

        Property PrinterUnits As Object
            Get
                Return unePrinterUnits.Value
            End Get
            Set(value As Object)
                Dim oldInternal As Boolean = _internalValueSet
                _internalValueSet = True
                unePrinterUnits.Value = value
                _internalValueSet = oldInternal
            End Set
        End Property

        Property CustomPaperSize As Boolean
            Get
                Return Me.uckCustomPaperSize.Checked
            End Get
            Set(value As Boolean)
                Me.uckCustomPaperSize.Checked = value
            End Set
        End Property

        Property DocumentName As String
            Get
                Return _printDocument.DocumentName
            End Get
            Set(value As String)
                _printDocument.DocumentName = value
            End Set
        End Property

        Property PrintAreaCheckedIndex As Integer
            Get
                Return Me.uosPrintArea.CheckedIndex
            End Get
            Set(value As Integer)
                Me.uosPrintArea.CheckedIndex = value
            End Set
        End Property

        Property OnlyMargins As Boolean
            Get
                Return Me.uckOnlyMargins.Checked
            End Get
            Set(value As Boolean)
                Dim oldInternal As Boolean = _internalValueSet
                _internalValueSet = True
                Me.uckOnlyMargins.Checked = value
                _internalValueSet = oldInternal
            End Set
        End Property

        Private Sub UpdatePrintPreview(outdated As Boolean, Optional onFinished As Action = Nothing)
            If Me.ctrlPrintPreview.Document IsNot Nothing AndAlso _updatePreviewEnabled AndAlso Me.CurrentPrinter IsNot Nothing Then
                If CheckPageSizeValid() Then
                    Me.PrepareGeneratePreview()
                    Try
                        _isGeneratingPreview = True
                        Me.btnUpdatePreview.Enabled = False
                        Me.Cursor = Cursors.AppStarting

                        Dim args As New BeforeUpdatePrintPreviewEventArgs(outdated)
                        RaiseEvent BeforeUpdatePrintPreview(Me, args)
                        If Not args.Cancel Then
                            Try
                                GeneratePreviewCore(True)
                            Catch ex As Exception
                                MessageBoxEx.ShowError(Me, ex.Message)
                            End Try
                        End If

                        Me.Cursor = Cursors.Default
                        Me.btnUpdatePreview.Enabled = True

                        Me.FinalizeGeneratePreview()
                        If onFinished IsNot Nothing Then onFinished.Invoke()
                    Finally
                        _isGeneratingPreview = False
                    End Try
                End If
            End If
        End Sub

        Private Sub UpdatePrintSettingsUserControls(updatePreview As Boolean)
            Dim oldUpdatePreview As Boolean = _updatePreviewEnabled
            Dim oldInternal As Boolean = _internalValueSet
            _internalValueSet = True
            _updatePreviewEnabled = False

            Dim oldCursor As Cursor = Cursor.Current
            Cursor.Current = Cursors.WaitCursor
            Try
                Me.uosOrientation.CheckedIndex = If(CurrentPrinter IsNot Nothing AndAlso CurrentPrinter.Settings.Landscape, 1, 0)
                UpdatePrintMarginsUserControls()  'HINT: must come AFTER  UpdatePaperSizeUserControls (because this method needs the current-Paper which is nothing before update)
                Dim updated As Boolean = UpdatePaperSizeUserControls()
                If _printDocument IsNot Nothing AndAlso updated Then
                    RaiseEvent AfterUpdatePaperSizes(Me, New EventArgs)
                    Me.nudNumberOfCopies.Value = If(CurrentPrinter IsNot Nothing, CurrentPrinter.Settings.Copies, Printer.PrinterSettingsEx.DEFAULT_COPIES)
                    _updatePreviewEnabled = oldUpdatePreview
                    If (updatePreview) Then
                        '    UpdatePrintPreview(False)
                    Else
                        Me.FinalizeGeneratePreview()
                    End If
                End If
            Finally
                Cursor.Current = oldCursor
                _updatePreviewEnabled = oldUpdatePreview
                _internalValueSet = oldInternal
            End Try
        End Sub

        Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
            RaiseEvent Close(Me, e)
        End Sub

        Private Sub btnPick_Click(sender As Object, e As EventArgs) Handles btnPick.Click
            btnPick.Enabled = False

            '  TryCancel()
            '   _singleGeneratePreview.SkipAllNext = True
            Try
                'If Await FormHelpers.ShowDialogAfterTimeoutAsync(Of CancellingForm)(Me, Function() Not _singleGeneratePreview.IsRunning) = DialogResult.Cancel Then
                '    Me.Abort()
                'End If
                RaiseEvent Picking(Me, e)
            Finally
                btnPick.Enabled = True
            End Try
        End Sub

        Private Sub btnPrint_Click(sender As Object, e As EventArgs) Handles btnPrint.Click
            If _printDocument IsNot Nothing Then
                Try
                    If CheckPageSizeValid() Then
                        _printDocument.CommitChanges()
                        _isPrinting = True
                        Me.Enabled = False
                        _printDocument.Print()
                    End If
                Catch ex As Exception
                    MessageBox.Show(String.Format(ErrorStrings.Printing_ErrorWhilePrinting, ex.Message), [Shared].MSG_BOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    _isPrinting = False
                    Me.Enabled = True
                End Try
            End If
        End Sub

        Private Function CheckPageSizeValid() As Boolean
            If _printDocument.Printer.Name = "Microsoft Print to PDF" Then
                Dim sizeMax As New MilliInch100(MAXSIZE_MICROSOFT_INCH * 100)

                If _printDocument.Printer.Settings.Page.Height.Millimeter > sizeMax.Millimeter Then
                    If Me.uosUnits.CheckedIndex = 1 Then
                        UltraToolTipManager1.SetUltraToolTip(uneHeight, New UltraWinToolTip.UltraToolTipInfo(String.Format(ErrorStrings.Printing_MaxHeightExceedsMM, Math.Round(sizeMax.Millimeter, 1, MidpointRounding.ToEven)), ToolTipImage.Error, String.Empty, DefaultableBoolean.False))
                    Else
                        UltraToolTipManager1.SetUltraToolTip(uneHeight, New UltraWinToolTip.UltraToolTipInfo(String.Format(ErrorStrings.Printing_MaxHeightExceedsInch, Math.Round(sizeMax.Inch, 1, MidpointRounding.ToEven)), ToolTipImage.Error, String.Empty, DefaultableBoolean.False))
                    End If

                    Me.UltraToolTipManager1.ShowToolTip(uneHeight, uneHeight.PointToScreen(New Point(uneHeight.Location.X + Me.Parent.Location.X, uneHeight.Location.Y + Me.Parent.Location.Y)))
                    Return False
                ElseIf _printDocument.Printer.Settings.Page.Width.Millimeter > sizeMax.Millimeter Then
                    If Me.uosUnits.CheckedIndex = 1 Then
                        UltraToolTipManager1.SetUltraToolTip(uneWidth, New UltraWinToolTip.UltraToolTipInfo(String.Format(ErrorStrings.Printing_MaxWidthExceedsMM, Math.Round(sizeMax.Millimeter, 1, MidpointRounding.ToEven)), ToolTipImage.Error, String.Empty, DefaultableBoolean.False))
                    Else
                        UltraToolTipManager1.SetUltraToolTip(uneWidth, New UltraWinToolTip.UltraToolTipInfo(String.Format(ErrorStrings.Printing_MaxWidthExceedsInch, Math.Round(sizeMax.Inch, 1, MidpointRounding.ToEven)), ToolTipImage.Error, String.Empty, DefaultableBoolean.False))
                    End If

                    Me.UltraToolTipManager1.ShowToolTip(uneWidth, uneWidth.PointToScreen(New Point(uneWidth.Size.Width, uneWidth.Location.Y + Me.Parent.Location.Y)))
                    Return False
                End If
            ElseIf _printDocument.Printer.Name = "Foxit Reader PDF Printer" Then
                Dim sizeMax As New MilliInch100(MAXSIZE_FOXIT_INCH * 100)

                If _printDocument.Printer.Settings.Page.Height.Millimeter > sizeMax.Millimeter Then
                    If Me.uosUnits.CheckedIndex = 1 Then
                        UltraToolTipManager1.SetUltraToolTip(uneHeight, New UltraWinToolTip.UltraToolTipInfo(String.Format(ErrorStrings.Printing_MaxHeightExceedsMM, Math.Round(sizeMax.Millimeter, 1, MidpointRounding.ToEven)), ToolTipImage.Error, String.Empty, DefaultableBoolean.False))
                    Else
                        UltraToolTipManager1.SetUltraToolTip(uneHeight, New UltraWinToolTip.UltraToolTipInfo(String.Format(ErrorStrings.Printing_MaxHeightExceedsInch, Math.Round(sizeMax.Inch, 1, MidpointRounding.ToEven)), ToolTipImage.Error, String.Empty, DefaultableBoolean.False))
                    End If
                    Me.UltraToolTipManager1.ShowToolTip(uneHeight, uneHeight.PointToScreen(New Point(uneHeight.Location.X + Me.Parent.Location.X, uneHeight.Location.Y + Me.Parent.Location.Y)))
                    Return False
                ElseIf _printDocument.Printer.Settings.Page.Width.Millimeter > sizeMax.Millimeter Then
                    If Me.uosUnits.CheckedIndex = 1 Then
                        UltraToolTipManager1.SetUltraToolTip(uneWidth, New UltraWinToolTip.UltraToolTipInfo(String.Format(ErrorStrings.Printing_MaxWidthExceedsMM, Math.Round(sizeMax.Millimeter, 1, MidpointRounding.ToEven)), ToolTipImage.Error, String.Empty, DefaultableBoolean.False))
                    Else
                        UltraToolTipManager1.SetUltraToolTip(uneWidth, New UltraWinToolTip.UltraToolTipInfo(String.Format(ErrorStrings.Printing_MaxWidthExceedsInch, Math.Round(sizeMax.Inch, 1, MidpointRounding.ToEven)), ToolTipImage.Error, String.Empty, DefaultableBoolean.False))
                    End If
                    Me.UltraToolTipManager1.ShowToolTip(uneWidth, uneWidth.PointToScreen(New Point(uneWidth.Size.Width, uneWidth.Location.Y + Me.Parent.Location.Y)))
                    Return False
                End If
            End If
            Return True
        End Function
        Private Sub btnRefreshPrinterCollection_Click(sender As Object, e As EventArgs) Handles btnRefreshPrinterCollection.Click
            If _printers.Count <> PrintersCollection.InstalledPrinterNames.Length Then
                'add or remove printer
                _printers = Nothing

                If PrintersCollection.InstalledPrinterNames.Length > 0 Then
                    If _printers Is Nothing OrElse PrintersCollection.NeedsUpdate Then
                        Try
                            _printers = PrintersCollection.GetAllLocalPrinters()
                        Catch ex As Exception
                            MessageBox.Show(Me, String.Format(ErrorStrings.Printing_ErrorRetrievingAllLocalPrinters, GetExceptionMessage(ex)), [Shared].MSG_BOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error)

                        End Try
                    End If
                Else
                    MessageBox.Show(Me, ErrorStrings.Printing_NoPrintersAvailable, [Shared].MSG_BOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error)

                End If

                Me.uceInstalledPrinters.Items.Clear()

                Dim lastPrinterItem As ValueListItem = UpdatePrinters().LastOrDefault

                If Not _cancel.IsCancellationRequested Then
                    Dim startPrinterToWork As Printer = If(lastPrinterItem IsNot Nothing, CType(lastPrinterItem.DataValue, Printer), Nothing)
                    If _printers.Default IsNot Nothing Then
                        startPrinterToWork = _printers.Default
                    End If
                    If startPrinterToWork IsNot Nothing Then
                        TryLoadStoredLastSettingTo(startPrinterToWork)
                        UpdatePaperWidthControlsFrom(startPrinterToWork.Settings.Page)
                        UpdateIsCustomControlsFrom(startPrinterToWork)
                    End If
                    Me.CurrentPrinter = startPrinterToWork
                Else
                    Me.uceInstalledPrinters.SelectedItem = Me.uceInstalledPrinters.Items.Cast(Of ValueListItem).LastOrDefault
                End If
            End If
        End Sub

        Private Sub btnPrinterSettings_Click(sender As Object, e As EventArgs) Handles btnPrinterSettings.Click
            Try
                _canBeClosed = False
                Dim myForm As Form = Me.FindForm()
                Dim olduseWait As Boolean = Me.UseWaitCursor
                If myForm IsNot Nothing Then myForm.UseWaitCursor = True
                Try
                    If CurrentPrinter IsNot Nothing Then
                        If CurrentPrinter.Settings.ShowEditPrinterSettingsDialog(Me) = DialogResult.OK Then
                            UpdatePrintSettingsUserControls(True)
                        End If
                    Else
                        MessageBox.Show(Me, ErrorStrings.Printing_NoPrinterWasSelected, [Shared].MSG_BOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    End If
                Finally
                    If myForm IsNot Nothing Then myForm.UseWaitCursor = olduseWait
                    _isFetchingPrinterSystemSettings = False
                End Try
            Finally
                _canBeClosed = True
                Me.Enabled = True
            End Try
        End Sub

        Private Sub btnUpdatePreview_Click(sender As Object, e As EventArgs) Handles btnUpdatePreview.Click
            UpdatePrintPreview(False)
        End Sub

        Private Function UpdatePageSettings(peparePreviewOnChange As Boolean) As Boolean
            Dim changed As Boolean = False
            Static isUpdatingPageSettings As Boolean = False

            Dim setChangedAction As Action =
                Sub()
                    If Not changed Then
                        changed = True
                        '   If peparePreviewOnChange Then PrepareGeneratePreview()
                    End If
                End Sub
            Try
                If _printDocument IsNot Nothing AndAlso Not isUpdatingPageSettings Then
                    isUpdatingPageSettings = True

                    If CurrentPrinter IsNot Nothing Then
                        With CurrentPrinter.Settings
                            If (Me.uckCustomPaperSize.Checked) AndAlso CurrentPageWidth <> -1 AndAlso CurrentPageHeight <> -1 Then
                                .Page = PageEx.NewCustom(CurrentPageWidth, CurrentPageHeight)
                                setChangedAction.Invoke()
                            ElseIf ucePaperSizes.SelectedItem IsNot Nothing Then
                                .Page = DirectCast(Me.ucePaperSizes.SelectedItem.DataValue, PageEx)
                                setChangedAction.Invoke()
                            End If

                            With .Page
                                If CurrentTopMargin <> -1 AndAlso .Margins.Top <> CurrentTopMargin Then
                                    .Margins.Top = CurrentTopMargin
                                    setChangedAction.Invoke()
                                End If

                                If CurrentBottomMargin <> -1 AndAlso .Margins.Bottom <> CurrentBottomMargin Then
                                    .Margins.Bottom = CurrentBottomMargin
                                    setChangedAction.Invoke()
                                End If

                                If CurrentLeftMargin <> -1 AndAlso .Margins.Left <> CurrentLeftMargin Then
                                    .Margins.Left = CurrentLeftMargin
                                    setChangedAction.Invoke()
                                End If

                                If CurrentRightMargin <> -1 AndAlso .Margins.Right <> CurrentRightMargin Then
                                    .Margins.Right = CurrentRightMargin
                                    setChangedAction.Invoke()
                                End If
                            End With

                            If Me.uosOrientation.CheckedItem IsNot Nothing Then
                                If .Landscape <> CBool(Me.uosOrientation.CheckedItem.DataValue) Then
                                    .Landscape = CBool(Me.uosOrientation.CheckedItem.DataValue)
                                    setChangedAction.Invoke()
                                End If
                            End If

                            If .Copies <> CShort(Me.nudNumberOfCopies.Value) Then
                                .Copies = CShort(Me.nudNumberOfCopies.Value)
                                setChangedAction.Invoke()
                            End If
                        End With
                    End If
                End If
            Finally
                isUpdatingPageSettings = False
            End Try

            Return changed
        End Function

        Private Sub NudNumberOfCopies_ValueChanged(sender As Object, e As EventArgs) Handles nudNumberOfCopies.ValueChanged
            UpdatePageSettings(False)
        End Sub

        Private Sub uceInstalledPrinters_SelectionChanged(sender As Object, e As EventArgs) Handles uceInstalledPrinters.SelectionChanged
            UpdatePrinterOfDocument()
            If (Me.uceInstalledPrinters.SelectedIndex <> -1) Then
                UpdatePrintSettingsUserControls(_initialized)
            End If
        End Sub

        Private Sub ucePaperSizes_SelectionChanged(sender As Object, e As EventArgs) Handles ucePaperSizes.SelectionChanged
            If (Me.ucePaperSizes.SelectedIndex <> -1) AndAlso Not uckCustomPaperSize.Checked Then
                Dim oldInternal As Boolean = _internalValueSet
                _internalValueSet = True
                UpdatePaperWidthControlsFrom(CType(Me.ucePaperSizes.SelectedItem.DataValue, PageEx))
                UpdatePageSettings(True)
                _internalValueSet = oldInternal
                'If _initialized Then UpdatePrintPreview(pageSettingsChanged)
            End If
        End Sub

        Private Sub UpdatePaperWidthControlsFrom(page As PageEx)
            If page IsNot Nothing Then
                With page
                    Dim height As Double = If(Me.uosUnits.CheckedIndex = 0, .Height.Inch, .Height.Millimeter)
                    Dim width As Double = If(Me.uosUnits.CheckedIndex = 0, .Width.Inch, .Width.Millimeter)
                    If height >= CDbl(Me.uneHeight.MinValue) Then Me.uneHeight.Value = height
                    If width >= CDbl(Me.uneWidth.MinValue) Then Me.uneWidth.Value = width
                End With
            End If
        End Sub

        Private Sub uckCustomPaperSize_CheckedValueChanged(sender As Object, e As EventArgs) Handles uckCustomPaperSize.CheckedValueChanged
            Dim oldInternal As Boolean = _internalValueSet
            _internalValueSet = True
            Me.ucePaperSizes.Enabled = Not Me.uckCustomPaperSize.Checked
            Me.uneHeight.Enabled = Me.uckCustomPaperSize.Checked
            Me.uneWidth.Enabled = Me.uckCustomPaperSize.Checked

            If (Me.uckCustomPaperSize.Checked) AndAlso CurrentPrinter IsNot Nothing Then
                UpdatePaperWidthControlsFrom(CurrentPrinter.Settings.Page)
            End If

            _internalValueSet = oldInternal

            If UpdatePageSettings(True) Then
                RaiseEvent CustomPaperSizeChanged(Me, New EventArgs)
                '   UpdatePrintPreview(True)
            End If
        End Sub

        Property CurrentPageHeight As MilliInch100
            Get
                Return GetMilliInch100From(uneHeight)
            End Get
            Set(value As MilliInch100)
                If value IsNot Nothing Then
                    Me.uneHeight.Value = If(Me.uosUnits.CheckedIndex = 0, value.Inch, value.Millimeter)
                Else
                    Me.uneHeight.Value = Nothing
                End If
            End Set
        End Property

        Property CurrentPageWidth As MilliInch100
            Get
                Return GetMilliInch100From(uneWidth)
            End Get
            Set(value As MilliInch100)
                If value IsNot Nothing Then
                    Me.uneWidth.Value = If(Me.uosUnits.CheckedIndex = 0, value.Inch, value.Millimeter)
                Else
                    Me.uneWidth.Value = Nothing
                End If
            End Set
        End Property

        Private Function GetMilliInch100From(numericEditor As UltraNumericEditor) As MilliInch100
            Dim value As MilliInch100 = Nothing
            If numericEditor.Value IsNot Nothing Then
                value = If(Me.uosUnits.CheckedIndex = 0, MilliInch100.FromInch(CSng(numericEditor.Value)), MilliInch100.FromMillimeter(CSng(numericEditor.Value)))
            End If
            Return value
        End Function

        Private Sub SetMilliInch100To(numericEditor As UltraNumericEditor, milliInch As MilliInch100)
            Dim value As MilliInch100 = Nothing
            numericEditor.Value = If(Me.uosUnits.CheckedIndex = 0, milliInch.Inch, milliInch.Millimeter)
        End Sub

        ReadOnly Property CurrentBottomMargin As MilliInch100 ' gets the current (input) bottom margin in 1/100 inch or centimeter
            Get
                Return GetMilliInch100From(uneBottomMargin)
            End Get
        End Property

        ReadOnly Property CurrentLeftMargin As MilliInch100 ' gets the current (input) lef margin in 1/100 inch or centimeter
            Get
                Return GetMilliInch100From(uneLeftMargin)
            End Get
        End Property

        ReadOnly Property CurrentRightMargin As MilliInch100 ' gets the current (input) right margin in 1/100 inch or centimeter
            Get
                Return GetMilliInch100From(uneRightMargin)
            End Get
        End Property

        ReadOnly Property CurrentTopMargin As MilliInch100 ' gets the current (input) top margin in 1/100 inch or centimeter
            Get
                Return GetMilliInch100From(uneTopMargin)
            End Get
        End Property

        Private Sub uckOnlyMargins_CheckedValueChanged(sender As Object, e As EventArgs) Handles uckOnlyMargins.CheckedValueChanged
            If Not _internalValueSet Then
                RaiseEvent OnlyMarginsChanged(Me, New EventArgs)
            End If
        End Sub

        Private Sub uckScaleToFit_CheckedValueChanged(sender As Object, e As EventArgs) Handles uckScaleToFit.CheckedValueChanged
            Me.uneDrawingUnits.Enabled = Not Me.uckScaleToFit.Checked
            Me.unePrinterUnits.Enabled = Not Me.uckScaleToFit.Checked

            If Not _internalValueSet Then
                RaiseEvent ScaleToFitChanged(Me, New EventArgs)
            End If
        End Sub

#Region "uneDrawingUnits"
        Private _drawingUnitsBeforeEnteredValue As Object = Nothing
        Private Sub uneDrawingUnits_BeforeEnterEditMode(sender As Object, e As CancelEventArgs) Handles uneDrawingUnits.BeforeEnterEditMode
            _drawingUnitsBeforeEnteredValue = uneDrawingUnits.Value
        End Sub
        Private Sub uneDrawingUnits_AfterExitEditMode(sender As Object, e As EventArgs) Handles uneDrawingUnits.AfterExitEditMode
            If Not _internalValueSet Then
                If (_drawingUnitsBeforeEnteredValue Is Nothing AndAlso uneDrawingUnits.Value IsNot Nothing) OrElse (_drawingUnitsBeforeEnteredValue IsNot Nothing AndAlso Not _drawingUnitsBeforeEnteredValue.Equals(uneDrawingUnits.Value)) Then
                    RaiseEvent AfterDrawingUnitsChanged(Me, New EventArgs)
                End If
            End If
        End Sub
#End Region

#Region "uneHeight"
        Private _heightBeforeEnteredValue As Object = Nothing
        Private Sub uneHeight_BeforeEnterEditMode(sender As Object, e As CancelEventArgs) Handles uneHeight.BeforeEnterEditMode
            _heightBeforeEnteredValue = uneHeight.Value
        End Sub

        Private Sub uneHeight_AfterExitEditMode(sender As Object, e As EventArgs) Handles uneHeight.AfterExitEditMode
            If Not _internalValueSet AndAlso (CurrentPrinter.Settings.Page.Height <> CurrentPageHeight.ToInch96) Then
                If (_heightBeforeEnteredValue Is Nothing AndAlso uneHeight.Value IsNot Nothing) OrElse (_heightBeforeEnteredValue IsNot Nothing AndAlso Not _heightBeforeEnteredValue.Equals(uneHeight.Value)) Then
                    UpdatePageSettings(True)
                End If
            End If
        End Sub
#End Region

#Region "uneWidth"
        Private _widthBeforeEnteredValue As Object = Nothing
        Private Sub uneWidth_BeforeEnterEditMode(sender As Object, e As CancelEventArgs) Handles uneWidth.BeforeEnterEditMode
            _widthBeforeEnteredValue = uneWidth.Value
        End Sub

        Private Sub uneWidth_AfterExitEditMode(sender As Object, e As EventArgs) Handles uneWidth.AfterExitEditMode
            If Not _internalValueSet AndAlso (CurrentPrinter.Settings.Page.Width <> CurrentPageWidth.ToInch96) Then
                If (_widthBeforeEnteredValue Is Nothing AndAlso uneWidth.Value IsNot Nothing) OrElse (_widthBeforeEnteredValue IsNot Nothing AndAlso Not _widthBeforeEnteredValue.Equals(uneWidth.Value)) Then
                    UpdatePageSettings(True)
                End If
            End If
        End Sub
#End Region

#Region "Margins"

#Region "uneLeftMargin"
        Private _leftMarginBeforeEnteredValue As Object = Nothing
        Private Sub uneLeftMargin_BeforeEnterEditMode(sender As Object, e As CancelEventArgs) Handles uneLeftMargin.BeforeEnterEditMode
            _leftMarginBeforeEnteredValue = uneLeftMargin.Value
        End Sub

        Private Sub uneLeftMargin_AfterExitEditMode(sender As Object, e As EventArgs) Handles uneLeftMargin.AfterExitEditMode
            If Not _internalValueSet Then
                If (_leftMarginBeforeEnteredValue Is Nothing AndAlso uneLeftMargin.Value IsNot Nothing) OrElse (_leftMarginBeforeEnteredValue IsNot Nothing AndAlso Not _leftMarginBeforeEnteredValue.Equals(uneLeftMargin.Value)) Then
                    UpdatePageSettings(True)
                End If
            End If
        End Sub
#End Region

#Region "uneRightMargin"
        Private _uneRightMarginBeforeEnteredValue As Object = Nothing
        Private Sub uneRightMargin_BeforeEnterEditMode(sender As Object, e As CancelEventArgs) Handles uneRightMargin.BeforeEnterEditMode
            _uneRightMarginBeforeEnteredValue = uneRightMargin.Value
        End Sub
        Private Sub uneRightMargin_ValueChanged(sender As Object, e As EventArgs) Handles uneRightMargin.AfterExitEditMode
            If Not _internalValueSet Then
                If (_uneRightMarginBeforeEnteredValue Is Nothing AndAlso uneRightMargin.Value IsNot Nothing) OrElse (_uneRightMarginBeforeEnteredValue IsNot Nothing AndAlso Not _uneRightMarginBeforeEnteredValue.Equals(uneRightMargin.Value)) Then
                    UpdatePageSettings(True)
                End If
            End If
        End Sub
#End Region

#Region "uneTopMargin"
        Private _uneTopMarginBeforeEnteredValue As Object = Nothing
        Private Sub uneTopMargin_BeforeEnterEditMode(sender As Object, e As CancelEventArgs) Handles uneTopMargin.BeforeEnterEditMode
            _uneTopMarginBeforeEnteredValue = uneTopMargin.Value
        End Sub

        Private Sub uneTopMargin_ValueChanged(sender As Object, e As EventArgs) Handles uneTopMargin.AfterExitEditMode
            If Not _internalValueSet Then
                If (_uneTopMarginBeforeEnteredValue Is Nothing AndAlso uneTopMargin.Value IsNot Nothing) OrElse (_uneTopMarginBeforeEnteredValue IsNot Nothing AndAlso Not _uneTopMarginBeforeEnteredValue.Equals(uneTopMargin.Value)) Then
                    UpdatePageSettings(True)
                End If
            End If
        End Sub
#End Region

#Region "uneBottomMargin"
        Private _uneBottomMarginBeforeEnteredValue As Object = Nothing
        Private Sub uneBottomMargin_BeforeEnterEditMode(sender As Object, e As CancelEventArgs) Handles uneBottomMargin.BeforeEnterEditMode
            _uneBottomMarginBeforeEnteredValue = uneBottomMargin.Value
        End Sub

        Private Sub uneBottomMargin_ValueChanged(sender As Object, e As EventArgs) Handles uneBottomMargin.AfterExitEditMode
            If Not _internalValueSet Then
                If (_uneBottomMarginBeforeEnteredValue Is Nothing AndAlso uneBottomMargin.Value IsNot Nothing) OrElse (_uneBottomMarginBeforeEnteredValue IsNot Nothing AndAlso Not _uneBottomMarginBeforeEnteredValue.Equals(uneBottomMargin.Value)) Then
                    UpdatePageSettings(True)
                End If
            End If
        End Sub
#End Region

#End Region

#Region "unePrinterUnits"
        Private _printerUnitsBeforeEnteredValue As Object = Nothing
        Private Sub unePrinterUnits_BeforeEnterEditMode(sender As Object, e As CancelEventArgs) Handles unePrinterUnits.BeforeEnterEditMode
            _printerUnitsBeforeEnteredValue = unePrinterUnits.Value
        End Sub

        Private Sub unePrinterUnits_AfterExitEditMode(sender As Object, e As EventArgs) Handles unePrinterUnits.AfterExitEditMode
            If Not _internalValueSet Then
                If (_printerUnitsBeforeEnteredValue Is Nothing AndAlso unePrinterUnits.Value IsNot Nothing) OrElse (_printerUnitsBeforeEnteredValue IsNot Nothing AndAlso Not _printerUnitsBeforeEnteredValue.Equals(unePrinterUnits.Value)) Then
                    RaiseEvent AfterPrinterUnitsChanged(Me, New EventArgs)
                End If
            End If
        End Sub
#End Region

        Private Sub uosOrientation_ValueChanged(sender As Object, e As EventArgs) Handles uosOrientation.ValueChanged
            If Not _internalValueSet Then
                UpdatePageSettings(True)
            End If
        End Sub

        Private Sub uosPrintArea_ValueChanged(sender As Object, e As EventArgs) Handles uosPrintArea.ValueChanged
            If (_printDocument IsNot Nothing) Then
                btnPick.Enabled = Me.uosPrintArea.CheckedIndex = 1
                If Not _internalValueSet Then
                    RaiseEvent AfterPrintAreaChanged(Me, New EventArgs)
                End If
            End If
        End Sub
        Private Sub uceViews_ValueChanged(sender As Object, e As EventArgs) Handles uceViews.ValueChanged

            If Not _internalValueSet Then
                RaiseEvent AfterViewChanged(Me, e)
            End If
        End Sub
        Private Sub uosUnits_ValueChanged(sender As Object, e As EventArgs) Handles uosUnits.ValueChanged
            If (CurrentPrinter IsNot Nothing) Then
                With CurrentPrinter.Settings.Page
                    Me.CurrentPageHeight = .Height.ToInch100
                    Me.CurrentPageWidth = .Width.ToInch100
                    If Not _internalValueSet Then
                        UpdatePrintSettingsUserControls(False)
                    End If
                End With
            End If
        End Sub

        Private Sub ctrlPrintPreviewl_MouseWheel(sender As Object, e As MouseEventArgs) Handles ctrlPrintPreview.MouseWheel
            ctrlPrintPreview.MouseAction = Infragistics.Win.Printing.PreviewMouseAction.DynamicZoom
        End Sub

        Private Sub PrintControl_HandleDestroyed(sender As Object, e As EventArgs) Handles Me.HandleDestroyed
            StoreCurrentToMyLastPaperSizeName()
        End Sub

        Private Sub _printDocument_PrintPage(sender As Object, e As PrintPageEventArgs) Handles _printDocument.PrintPage
            RaiseEvent Printing(Me, e)
        End Sub

        Private Sub _printDocument_EndPrint(sender As Object, e As PrintEventArgs) Handles _printDocument.EndPrint ' HINT: Is Async
            If Not Me.IsDisposed Then
                If _isPrinting Then
                    Me.UseWaitCursor = False
                    Me.Enabled = True
                    _isPrinting = False
                End If
            End If
        End Sub

        <Obfuscation(Feature:="renaming")>
        ReadOnly Property IsPrinting As Boolean
            Get
                Return _isPrinting
            End Get
        End Property

        <Obfuscation(Feature:="renaming")>
        ReadOnly Property IsPrintingOrPreviewGenerate As Boolean
            Get
                Return IsPrinting OrElse IsGeneratingPreview
            End Get
        End Property

        ReadOnly Property Document As PrintDocumentEx
            Get
                Return _printDocument
            End Get
        End Property

        ReadOnly Property CanBeClosed As Boolean
            Get
                Return _canBeClosed
            End Get
        End Property

        Property CenterToPaper As Boolean
            Get
                Return Me.uckCenterToPaper.Checked
            End Get
            Set(value As Boolean)
                Me.uckCenterToPaper.Checked = value
            End Set
        End Property

        <DebuggerStepThrough>
        Public Function PreFilterMessage(ByRef m As Message) As Boolean Implements IMessageFilter.PreFilterMessage
            If Me.Visible Then
                If Me.ContainsFocus AndAlso ctrlPrintPreview.Focused Then
                    Select Case m.Msg
                        Case System.Windows.WindowsMessage.LBUTTONDOWN, System.Windows.WindowsMessage.MBUTTONDOWN, System.Windows.WindowsMessage.RBUTTONDOWN
                            ctrlPrintPreview.MouseAction = Infragistics.Win.Printing.PreviewMouseAction.Hand
                    End Select
                End If
            End If
            Return False
        End Function

        Public Sub SetPickEnabled(enabled As Boolean)
            Me.btnPick.Enabled = enabled
        End Sub

        Private Sub ctrlPrintPreview_DoubleClick(sender As Object, e As EventArgs) Handles ctrlPrintPreview.DoubleClick
            ResetPrintPreview()
        End Sub

        Private Sub UltraToolbarsManager1_ToolClick(sender As Object, e As UltraWinToolbars.ToolClickEventArgs) Handles UltraToolbarsManager1.ToolClick
            If e.Tool.Key = "Reset" Then
                ResetPrintPreview()
            ElseIf e.Tool.Key = "Update" Then
                UpdatePrintPreview(False)
            End If
        End Sub

        Private Sub ResetPrintPreview()
            ctrlPrintPreview.Settings.ZoomMode = Infragistics.Win.Printing.ZoomMode.WholePage
        End Sub

        Private Sub _printers_CollectionChanged(sender As Object, e As NotifyCollectionChangedEventArgs) Handles _printers.CollectionChanged
            UpdateInstalledPrintersBox()
        End Sub

        Private Sub UltraCheckEditor1_CheckedChanged(sender As Object, e As EventArgs) Handles uckCenterToPaper.CheckedChanged
            RaiseEvent CenterToPaperChanged(Me, e)
            If _initialized Then
                '  UpdatePrintPreview(True)
            End If
        End Sub

        Private Sub PrintControl_VisibleChanged(sender As Object, e As EventArgs) Handles Me.VisibleChanged
            If Me.Visible AndAlso _initialized AndAlso Not _previewGenerated Then
                Me.UpdatePrintPreview(False)
            End If
        End Sub

        Public Class BeforeUpdatePrintPreviewEventArgs
            Inherits System.ComponentModel.CancelEventArgs

            Public Sub New(outdated As Boolean)
                MyBase.New()
                Me.Outdated = outdated
            End Sub

            ReadOnly Property Outdated As Boolean

        End Class


    End Class

End Namespace