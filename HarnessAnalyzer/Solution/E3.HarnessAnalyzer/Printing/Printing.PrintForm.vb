Imports System.Drawing.Printing
Imports System.Reflection

Namespace Printing

    <Reflection.Obfuscation(Feature:="renaming", ApplyToMembers:=False, Exclude:=True)> ' needed to avoid resource-file-names obfuscation errors, see issue #5
    Public Class PrintForm

        <Obfuscation(Feature:="renaming")>
        Private _initializingDrawing As Boolean
        <Obfuscation(Feature:="renaming")>
        Private _initializingSchematics As Boolean
        <Obfuscation(Feature:="renaming")>
        Private _initializingThreeD As Boolean

        Private _printing As VdPrinting
        Private _printAreaChangeable As Boolean
        Private _isClosing As Boolean = False
        Private _schematicsControl As Schematics.Controls.ViewControl
        Private _threeDPrntControl As EyeshotPrinting
        Private Shared _lastPrinters As PrintersCollection

        '3D printing
        Public Sub New(threeDControl As EyeshotPrinting, Optional printAreaChangeable As Boolean = True)
            InitializeComponent()
            _threeDPrntControl = threeDControl

            Me.Icon = My.Resources.Print

            Me.ThreeDPrintControl.Enabled = True
            Me.ThreeDTabControl.Tab.Enabled = True
            Me.ThreeDTabControl.Tab.Visible = True
            _printAreaChangeable = printAreaChangeable

            Me.DrawingEnabled = False
            Me.DrawingVisible = False

            Me.SchematicsEnabled = False
            Me.SchematicsVisible = False
        End Sub

        'Drawings and Schematics 
        Public Sub New(printing As VdPrinting, schematicsControl As Schematics.Controls.ViewControl, Optional printAreaChangeable As Boolean = True)
            InitializeComponent()
            _schematicsControl = schematicsControl

            Me.Icon = My.Resources.Print
            _printing = printing

            Me.DrawingPrintControl.DocumentName = printing.DocumentName
            _printAreaChangeable = printAreaChangeable

            Me.SchematicsVisible = schematicsControl IsNot Nothing AndAlso My.Application.MainForm.HasSchematicsFeature
            Me.SchematicsEnabled = My.Application.MainForm.SchematicsView IsNot Nothing AndAlso My.Application.MainForm.SchematicsView.ActiveEntities.Count > 0

            Me.ThreeDTabEnabled = False
            Me.ThreeDTabVisible = False
        End Sub

        Protected Overrides Sub OnShown(e As EventArgs)
            If InitPrinters() Then
                MyBase.OnShown(e)
                Init()
            Else
                Me.DialogResult = DialogResult.Cancel
                Me.Close()
            End If
        End Sub

        Private Function InitPrinters() As Boolean
            If PrintersCollection.InstalledPrinterNames.Length > 0 Then
                If _lastPrinters Is Nothing OrElse PrintersCollection.NeedsUpdate Then
                    Try
                        _lastPrinters = PrintersCollection.GetAllLocalPrinters()
                    Catch ex As Exception
                        ex.ShowMessageBox(Me, String.Format(ErrorStrings.Printing_ErrorRetrievingAllLocalPrinters, GetExceptionMessage(ex)))
                        Return False
                    End Try
                End If
            Else
                'HINT: using me is not possible at this point because the dialog window handle has not created!
                MessageBoxEx.ShowError(ErrorStrings.Printing_NoPrintersAvailable)
                Return False
            End If
            Return True
        End Function

        Private Function GetExceptionMessage(ex As Exception) As String
            If TypeOf ex Is AggregateException Then
                Return CType(ex, AggregateException).InnerException.Message
            Else
                Return ex.Message
            End If
        End Function

        Private Sub Init()
            Try
                Me.UseWaitCursor = True

                If _printing IsNot Nothing Then
                    InitDrawingPrintControl(_printing, _printAreaChangeable)
                End If
                If _schematicsControl IsNot Nothing Then
                    InitSchematicsPrintControl()
                End If
                If _threeDPrntControl IsNot Nothing Then
                    InitThreeDPrintControl()
                End If
            Catch ex As OperationCanceledException
                Return
            Finally
                Me.UseWaitCursor = False
            End Try
        End Sub

        Private Sub InitDrawingPrintControl(printing As VdPrinting, printAreaChangeable As Boolean)
            _initializingDrawing = True

            Try
                With Me.DrawingPrintControl

                    .Printers = _lastPrinters
                    _printing.PrintExtends = _printing.PrintSelection Is Nothing
                    If _printing IsNot Nothing Then
                        .OnlyMargins = _printing.OnlyMarginsPreview
                        .SetPickEnabled(_printing.PrintExtends)
                        .PrintAreaCheckedIndex = If(Not _printing.PrintExtends, 1, 0)
                        .DrawingUnits = _printing.ScaleDenumerator
                        .PrinterUnits = _printing.ScaleNumerator
                        .CenterToPaper = _printing.CenterToPaper
                    End If
                    .ugbViews.Visible = False
                    .Initialize(printAreaChangeable)
                End With
            Finally
                _initializingDrawing = False
            End Try
        End Sub
        Private Sub InitThreeDPrintControl()
            _initializingThreeD = True
            Try

                With Me.ThreeDPrintControl
                    If _threeDPrntControl IsNot Nothing Then
                        .ugbPrintArea.Visible = True
                        .ugbPrintArea.Enabled = True
                        .OnlyMargins = _threeDPrntControl.OnlyMarginsPreview
                        .SetPickEnabled(_threeDPrntControl.IsPrintWindowSelected)
                        .PrintAreaCheckedIndex = If(_threeDPrntControl.IsPrintWindowSelected, 1, 0)
                        .CenterToPaper = True
                    End If

                    .Printers = _lastPrinters
                    .tblSettings.SetColumn(.ugbOrientation, 1)
                    .tblSettings.ColumnStyles(1).Width = 0
                    .tblSettings.ColumnStyles(3).Width = 0
                    .tblSettings.SetColumnSpan(.ugbOrientation, 1)

                    .uosUnits.Visible = False
                    .lblUnits.Visible = False
                    .uckOnlyMargins.Visible = False
                    .ugbMargins.Visible = True
                    .ugbScale.Visible = False
                    .ugbPrintArea.Visible = False
                    .uckOnlyMargins.Visible = False
                    .ugbViews.Visible = True

                    .Initialize(True)
                End With
            Finally
                _initializingThreeD = False
            End Try
        End Sub
        Private Sub InitSchematicsPrintControl()
            _initializingSchematics = True
            Try
                With Me.SchematicsPrintControl

                    .Printers = _lastPrinters
                    .tblSettings.SetColumn(.ugbOrientation, 0)
                    .tblSettings.ColumnStyles(1).Width = 0
                    .tblSettings.ColumnStyles(3).Width = 0
                    .tblSettings.SetColumnSpan(.ugbOrientation, 2)
                    .uosUnits.Visible = False
                    .lblUnits.Visible = False
                    .uckOnlyMargins.Visible = False
                    .ugbMargins.Visible = False
                    .ugbScale.Visible = False
                    .ugbPrintArea.Visible = False
                    .uckOnlyMargins.Visible = False
                    .ugbViews.Visible = False
                    .Initialize(False)
                End With
            Finally
                _initializingSchematics = False
            End Try
        End Sub

        Private Sub DrawingPrintControl_AfterDrawingUnitsEdited(sender As Object, e As EventArgs) Handles DrawingPrintControl.AfterDrawingUnitsChanged
            _printing.ScaleDenumerator = CDbl(Me.DrawingPrintControl.DrawingUnits)
        End Sub

        Private Sub DrawingPrintControl_AfterPrintAreaChanged(sender As Object, e As EventArgs) Handles DrawingPrintControl.AfterPrintAreaChanged
            If _printing IsNot Nothing Then
                _printing.PrintExtends = Me.DrawingPrintControl.PrintAreaCheckedIndex = 0
            End If
        End Sub

        Private Sub DrawingPrintControl_AfterPrinterUnitsChanged(sender As Object, e As EventArgs) Handles DrawingPrintControl.AfterPrinterUnitsChanged
            _printing.ScaleNumerator = CDbl(Me.DrawingPrintControl.PrinterUnits)
        End Sub

        Private Sub DrawingPrintControl_AfterUpdatePaperSizes(sender As Object, e As EventArgs) Handles DrawingPrintControl.AfterUpdatePaperSizes
            Me.DrawingPrintControl.uckScaleToFit.Visible = _printing IsNot Nothing
            If _printing IsNot Nothing Then
                Me.DrawingPrintControl.ScaleToFit = _printing.ScaleToFit
            End If
        End Sub

        Private Sub DrawingPrintControl_BeforeUpdatePreview(sender As Object, e As PrintControl.BeforeUpdatePrintPreviewEventArgs) Handles DrawingPrintControl.BeforeUpdatePrintPreview
            If (e.Outdated) AndAlso (Not _printing.OnlyMarginsPreview) Then
                DrawingPrintControl.OnlyMargins = False
                e.Cancel = True
            End If
        End Sub

        Private Sub DrawingPrintControl_Close(sender As Object, e As EventArgs) Handles DrawingPrintControl.Close, SchematicsPrintControl.Close
            Me.DialogResult = System.Windows.Forms.DialogResult.OK
            Me.Close()
        End Sub

        Private Sub DrawingPrintControl_CustomPaperSizeChanged(sender As Object, e As EventArgs) Handles DrawingPrintControl.CustomPaperSizeChanged
        End Sub

        Private Sub DrawingPrintControl_OnlyMarginsChanged(sender As Object, e As EventArgs) Handles DrawingPrintControl.OnlyMarginsChanged
            _printing.OnlyMarginsPreview = Me.DrawingPrintControl.OnlyMargins
        End Sub

        Private Sub DrawingPrintControl_Picking(sender As Object, e As EventArgs) Handles DrawingPrintControl.Picking
            Me.DialogResult = System.Windows.Forms.DialogResult.Ignore
            Me.Close()
        End Sub

        Private Sub PrintForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
            If Not _isClosing Then
                If Not Me.DrawingPrintControl.IsPrinting AndAlso Not Me.SchematicsPrintControl.IsPrinting AndAlso Not Me.ThreeDPrintControl.IsPrinting Then
                    _isClosing = True

                    If GetAllMyControlsRecursively.OfType(Of PrintControl).Any(Function(iclos) Not iclos.CanBeClosed) Then
                        Me.UltraToolTipManager1.ShowToolTip(Me) ' HINT: the reason why using a tooltip here to show up this message is that it is possible to produce a total Application lock when using a messagebox while the driver dialog is prepared. If the internal show and this messagebox are hitting together at the same time window it seems that this locks everything and the user can't proceed (only be killing the app over the TaskManager).
                        e.Cancel = True
                        _isClosing = False
                        Return
                    End If
                End If
            End If
        End Sub

        Private Function GetAllMyControlsRecursively() As List(Of Control)
            Static myList As List(Of Control)
            If myList Is Nothing Then
                myList = GetAllControlsRecursively(Me)
            End If
            Return myList
        End Function

        Private Function GetAllControlsRecursively(ctrl As Control) As List(Of Control)
            Dim myList As New List(Of Control)
            For Each c As Control In ctrl.Controls
                myList.Add(c)
                myList.AddRange(GetAllControlsRecursively(c))
            Next

            Return myList
        End Function
        Property DrawingEnabled As Boolean
            Get
                Return Me.DrawingTabControl.Tab.Enabled
            End Get
            Set(value As Boolean)
                Me.DrawingTabControl.Tab.Enabled = value
            End Set
        End Property

        Property DrawingVisible As Boolean
            Get
                Return Me.DrawingTabControl.Tab.Visible
            End Get
            Set(value As Boolean)
                Me.DrawingTabControl.Tab.Visible = value
            End Set
        End Property
        Property SchematicsEnabled As Boolean
            Get
                Return Me.SchematicsTabControl.Tab.Enabled
            End Get
            Set(value As Boolean)
                Me.SchematicsTabControl.Tab.Enabled = value
            End Set
        End Property

        Property SchematicsVisible As Boolean
            Get
                Return Me.SchematicsTabControl.Tab.Visible
            End Get
            Set(value As Boolean)
                Me.SchematicsTabControl.Tab.Visible = value
            End Set
        End Property
        Property ThreeDTabEnabled As Boolean
            Get
                Return Me.ThreeDTabControl.Tab.Enabled
            End Get
            Set(value As Boolean)
                Me.ThreeDTabControl.Tab.Enabled = value
            End Set
        End Property

        Property ThreeDTabVisible As Boolean
            Get
                Return Me.ThreeDTabControl.Tab.Visible
            End Get
            Set(value As Boolean)
                Me.ThreeDTabControl.Tab.Visible = value
            End Set
        End Property

        Private Sub btnPrint_Click(sender As Object, e As EventArgs)
            If My.Application.MainForm.SchematicsView IsNot Nothing Then
                My.Application.MainForm.SchematicsView.Print()
            End If
            Me.DialogResult = System.Windows.Forms.DialogResult.OK
            Me.Close()
        End Sub

        Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles Me.FormClosed
            Me.Close()
        End Sub

        Private Sub PrintForm_Closed(sender As Object, e As EventArgs) Handles Me.FormClosed
            If (_threeDPrntControl IsNot Nothing) Then
                If Not _threeDPrntControl.IsPrintWindowSelected Then
                    _threeDPrntControl.DisposeModelAfterFormClosed()
                End If
            End If
        End Sub

        Private Sub ThreeDPrintControl_Close(sender As Object, e As EventArgs) Handles ThreeDPrintControl.Close
            Me.DialogResult = System.Windows.Forms.DialogResult.OK
            Me.Close()
        End Sub

        Private Sub ThreeDPrintControl_AfterPrintAreaChanged(sender As Object, e As EventArgs) Handles ThreeDPrintControl.AfterPrintAreaChanged
            If _threeDPrntControl IsNot Nothing Then
                _threeDPrntControl.PrintAreaCheckedIndex = Me.ThreeDPrintControl.PrintAreaCheckedIndex
                If Me.ThreeDPrintControl.PrintAreaCheckedIndex = 0 Then
                    _threeDPrntControl.IsPrintWindowSelected = False
                    _threeDPrntControl.InternalModelZoomFitAndRefresh()

                Else
                    _threeDPrntControl.IsPrintWindowSelected = True
                End If
            End If
        End Sub

        Private Sub ThreeDPrintControl_AfterUpdatePaperSizes(sender As Object, e As EventArgs) Handles ThreeDPrintControl.AfterUpdatePaperSizes
            Me.ThreeDPrintControl.uckScaleToFit.Visible = _printing IsNot Nothing
            If _threeDPrntControl IsNot Nothing Then
                Me.ThreeDPrintControl.ScaleToFit = True
            End If
        End Sub
        Private Sub ThreeDPrintControl_BeforeUpdatePreview(sender As Object, e As PrintControl.BeforeUpdatePrintPreviewEventArgs) Handles ThreeDPrintControl.BeforeUpdatePrintPreview
            If (e.Outdated) AndAlso (Not ThreeDPrintControl.OnlyMargins) Then
                ThreeDPrintControl.OnlyMargins = False
                e.Cancel = True
            End If
        End Sub
        Private Sub ThreeDPrintControl_OnlyMarginsChanged(sender As Object, e As EventArgs) Handles ThreeDPrintControl.OnlyMarginsChanged
            _threeDPrntControl.OnlyMarginsPreview = Me.ThreeDPrintControl.OnlyMargins
        End Sub

        Private Sub ThreeDPrintControl_Picking(sender As Object, e As EventArgs) Handles ThreeDPrintControl.Picking
            Me.DialogResult = System.Windows.Forms.DialogResult.Ignore
            Me.Close()

            _threeDPrntControl.IsPrintWindowSelected = True
        End Sub
        Private Sub ThreeDPrintControl_AfterViewchanged(sender As Object, e As EventArgs) Handles ThreeDPrintControl.AfterViewChanged
            _threeDPrntControl.ViewSelectedIndex = ThreeDPrintControl.ViewSelectedIndex
            Me.ThreeDPrintControl.PrintAreaCheckedIndex = 0
            _threeDPrntControl.IsPrintWindowSelected = False
            _threeDPrntControl.InternalModelZoomFitAndRefresh()
        End Sub

        Private Sub _ThreeDPage_PrintPage(sender As Object, e As System.Drawing.Printing.PrintPageEventArgs) Handles ThreeDPrintControl.Printing
            Dim oldOnlyMarginsPreview As Boolean = _threeDPrntControl.OnlyMarginsPreview
            Try
                If ThreeDPrintControl.IsPrinting Then
                    _threeDPrntControl.OnlyMarginsPreview = False
                End If
                _threeDPrntControl.PrintPage(e)

            Catch ex As Exception
#If DEBUG Or CONFIG = "Debug" Then
                Throw
#End If
                'HINT: For safty reasons... (do nothing here) When there is an exception we don't break the whole printing dialog. Only the preview is not working
            Finally
                _threeDPrntControl.OnlyMarginsPreview = oldOnlyMarginsPreview
            End Try
        End Sub


        Private Sub _schematicsPage_PrintPage(sender As Object, e As System.Drawing.Printing.PrintPageEventArgs) Handles SchematicsPrintControl.Printing
            Try
                _schematicsControl.Print(e.Graphics)
            Catch ex As Exception
#If DEBUG Or CONFIG = "Debug" Then
                Throw
#End If
                'HINT: For safety reasons... (do nothing here) When there is an exception we don't break the whole printing dialog. Only the preview is not working
            End Try
        End Sub

        Private Sub _drawingPage_PrintPage(sender As Object, e As PrintPageEventArgs) Handles DrawingPrintControl.Printing
            Dim oldOnlyMarginsPreview As Boolean = _printing.OnlyMarginsPreview
            Try
                If DrawingPrintControl.IsPrinting Then
                    _printing.OnlyMarginsPreview = False
                End If
                _printing.PrintPage(e)
            Catch ex As Exception
#If DEBUG Or CONFIG = "Debug" Then
                Throw
#End If
                'HINT: For safety reasons... (do nothing here) When there is an exception we don't break the whole printing dialog. Only the preview is not working
            Finally
                _printing.OnlyMarginsPreview = oldOnlyMarginsPreview
            End Try
        End Sub

        Private Sub _drawingPage_EndPrint(sender As Object, e As PrintEventArgs) Handles DrawingPrintControl.EndPrint
            If (Me.DrawingPrintControl.ScaleToFit) Then
                If (Not Double.IsInfinity(_printing.ScaleDenumerator)) Then Me.DrawingPrintControl.DrawingUnits = _printing.ScaleDenumerator
                If (Not Double.IsInfinity(_printing.ScaleNumerator)) Then Me.DrawingPrintControl.PrinterUnits = _printing.ScaleNumerator
            End If
        End Sub

        Private Sub DrawingPrintControl_ScaleToFitChanged(sender As Object, e As EventArgs) Handles DrawingPrintControl.ScaleToFitChanged
            _printing.ScaleToFit = Me.DrawingPrintControl.ScaleToFit
        End Sub

        Private Sub UltraTabControl1_ActiveTabChanged(sender As Object, e As Infragistics.Win.UltraWinTabControl.ActiveTabChangedEventArgs) Handles UltraTabControl1.ActiveTabChanged
            Dim tabPrintControl As PrintControl = e.Tab.TabPage.Controls.OfType(Of PrintControl).SingleOrDefault
            Me.CancelButton = Nothing
            If tabPrintControl IsNot Nothing Then
                Me.CancelButton = tabPrintControl.btnClose
            End If
        End Sub

        Private Sub DrawingPrintControl_AbortingPreviewGenerate(sender As Object, e As EventArgs) Handles DrawingPrintControl.AbortingPreviewGenerate
            If _printing.IsPrinting Then
                _printing.CancelPrint = True
            End If
        End Sub

        Private Sub DrawingPrintControl_CenterToPaperChanged(sender As Object, e As EventArgs) Handles DrawingPrintControl.CenterToPaperChanged
            If _printing IsNot Nothing Then
                _printing.CenterToPaper = DrawingPrintControl.CenterToPaper
            End If
        End Sub


    End Class

End Namespace