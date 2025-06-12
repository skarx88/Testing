Imports System.IO
Imports VectorDraw.Professional.vdObjects

<Reflection.Obfuscation(Feature:="renaming", ApplyToMembers:=False, Exclude:=True)> ' needed to avoid resource-file-names obfuscation errors, see issue #5
Public Class PdfSaveForm

    Private _entitiesBounds As New VectorDraw.Geometry.Box
    Private _vdDocument As vdDocument
    Private _exportType As PdfExportType

    Public Const PDF_MAX_X As Single = 5080
    Public Const PDF_MAX_Y As Single = 5080

    Public Sub New(ByVal vdDocument As vdDocument, ByVal initFileName As String)
        InitializeComponent()
        InitializeControl(initFileName)

        _vdDocument = vdDocument
    End Sub

    Private Sub InitializeControl(ByVal initFileName As String)
        Me.BackColor = Color.White
        Me.txtDrawingSavePath.Text = If(String.IsNullOrEmpty(My.Settings.LastPdfDrawingExportFilePath), Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), String.Format("{0}.pdf", initFileName)), My.Settings.LastPdfDrawingExportFilePath)
        Me.txtSchematicsSavePath.Text = If(String.IsNullOrEmpty(My.Settings.LastPdfSchematicsExportFilePath), Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), String.Format("{0}.pdf", initFileName)), My.Settings.LastPdfSchematicsExportFilePath)
        Me.uckEmbeddTTF.Checked = True
    End Sub

    Private Function CheckFilePath() As Boolean
        If (String.IsNullOrEmpty(Me.FilePath)) OrElse (Not Directory.Exists(IO.Path.GetDirectoryName(Me.FilePath))) Then Return False

        Return True
    End Function

    Private Function GetFileName(ByVal filePath As String) As String
        Using sfdPDF As New SaveFileDialog
            With sfdPDF
                .DefaultExt = KnownFile.PDF.Trim("."c)
                .Filter = "PDF files (*.pdf)|*.pdf"
                .InitialDirectory = filePath
                .SupportMultiDottedExtensions = True
                .Title = PdfSaveFormStrings.SavePDFFile_Title

                If KnownFile.IsType(KnownFile.Type.PDF, filePath) Then
                    .FileName = Path.GetFileNameWithoutExtension(filePath)
                Else
                    .FileName = Path.GetFileName(filePath)
                End If

                If .ShowDialog() = System.Windows.Forms.DialogResult.OK Then
                    Return .FileName
                Else
                    Return filePath
                End If
            End With
        End Using
    End Function

    ReadOnly Property PDFExportBounds As System.Drawing.SizeF
        Get
            Dim sizeX As Double = _entitiesBounds.Width
            Dim sizeY As Double = _entitiesBounds.Height

            If chkScaleToMaximum.Checked Then
                Dim reduceXPerc As Double = 1
                Dim reduceYPerc As Double = 1

                If sizeX > 0 Then reduceXPerc = PDF_MAX_X * 100 / sizeX
                If sizeY > 0 Then reduceYPerc = PDF_MAX_Y * 100 / sizeY

                If sizeX > PDF_MAX_X AndAlso sizeY > PDF_MAX_Y Then
                    sizeX = PDF_MAX_X
                    sizeY = PDF_MAX_Y
                ElseIf sizeX > PDF_MAX_X Then
                    sizeX = sizeX * reduceXPerc / 100
                    sizeY = sizeY * reduceXPerc / 100
                ElseIf sizeY > PDF_MAX_Y Then
                    sizeY = sizeY * reduceYPerc / 100
                    sizeX = sizeX * reduceYPerc / 100
                End If

                'HINT: 2nd check for absolute safety reason. Ensure really that the size calculated does not exceed the limits 
                If sizeX > PDF_MAX_X Then sizeX = PDF_MAX_X
                If sizeY > PDF_MAX_Y Then sizeY = PDF_MAX_Y
            End If

            Return New SizeF(CSng(sizeX), CSng(sizeY))
        End Get
    End Property

    Property DrawingExportVisible As Boolean
        Get
            Return Me.DrawingTabControl.Tab.Visible
        End Get
        Set(value As Boolean)
            Me.DrawingTabControl.Tab.Visible = value
        End Set
    End Property

    Property SchematicsExportVisible As Boolean
        Get
            Return Me.SchematicsTabControl.Tab.Visible
        End Get
        Set(value As Boolean)
            Me.SchematicsTabControl.Tab.Visible = value
        End Set
    End Property

    Property SchematicsExportEnabled As Boolean
        Get
            Return Me.SchematicsTabControl.Tab.Enabled
        End Get
        Set(value As Boolean)
            Me.SchematicsTabControl.Tab.Enabled = value
        End Set
    End Property

    ReadOnly Property Type As PdfExportType
        Get
            Return _exportType
        End Get
    End Property

    Public Enum PdfExportType
        Drawing = 0
        Schematics = 1
        ThreeD = 2
    End Enum

    Private Sub PdfSaveForm_FormClosing(ByVal sender As Object, ByVal e As FormClosingEventArgs) Handles Me.FormClosing
        If (Me.DialogResult = System.Windows.Forms.DialogResult.OK) AndAlso (Not CheckFilePath()) Then
            e.Cancel = True

            MessageBox.Show(PdfSaveFormStrings.InputValidPath_Msg, [Shared].MSG_BOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If

        If Me.DialogResult = System.Windows.Forms.DialogResult.OK AndAlso Not e.Cancel Then
            Select Case Me.Type
                Case PdfExportType.Drawing
                    My.Settings.LastPdfDrawingExportFilePath = Me.FilePath
                Case PdfExportType.Schematics
                    My.Settings.LastPdfSchematicsExportFilePath = Me.FilePath
                Case PdfExportType.ThreeD
                    My.Settings.LastPdfThreeDExportFilePath = Me.FilePath
            End Select
        End If

    End Sub

    Private Sub btnCancel_Click(ByVal sender As Object, ByVal e As EventArgs)
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
    End Sub

    Public ReadOnly Property UseTTFText() As Boolean
        Get
            Return Me.uckEmbeddTTF.Checked
        End Get
    End Property

    Public ReadOnly Property FilePath() As String
        Get
            If Me.UltraTabControl1.ActiveTab.Key = "Drawing" Then
                Return Me.txtDrawingSavePath.Text
            ElseIf Me.UltraTabControl1.ActiveTab.Key = "Schematics" Then
                Return Me.txtSchematicsSavePath.Text
            ElseIf Me.UltraTabControl1.ActiveTab.Key = "ThreeD" Then
                Return Me.txtSchematicsSavePath.Text
            Else
                Throw New NotImplementedException(String.Format("FilePath for Tab {0} not implemented!", Me.UltraTabControl1.ActiveTab.Key))
            End If
        End Get
    End Property

    Private Sub btnDrawingOK_Click(sender As Object, e As EventArgs) Handles btnDrawingOK.Click

        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        _exportType = PdfExportType.Drawing
        If PDFExportBounds.Width > PDF_MAX_X OrElse PDFExportBounds.Height > PDF_MAX_Y Then
            Select Case MessageBox.Show(Me, PdfSaveFormStrings.PDFPageSizeTooBigWarning, [Shared].MSG_BOX_TITLE, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning)
                Case DialogResult.Yes
                    chkScaleToMaximum.Checked = True
                Case DialogResult.Cancel
                    Me.DialogResult = System.Windows.Forms.DialogResult.None
            End Select
        End If
    End Sub

    Private Sub btnSchematicsOK_Click(sender As Object, e As EventArgs) Handles btnSchematicsOK.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        _exportType = PdfExportType.Schematics
    End Sub

    Private Sub btnDrawingCancel_Click(sender As Object, e As EventArgs) Handles btnDrawingCancel.Click, btnSchematicsCancel.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
    End Sub

    Private Sub txtDrawingSavePath_EditorButtonClick(sender As Object, e As Infragistics.Win.UltraWinEditors.EditorButtonEventArgs) Handles txtDrawingSavePath.EditorButtonClick
        Me.txtDrawingSavePath.Text = GetFileName(Me.txtDrawingSavePath.Text)
    End Sub

    Private Sub txtSchematicsSavePath_EditorButtonClick(sender As Object, e As Infragistics.Win.UltraWinEditors.EditorButtonEventArgs) Handles txtSchematicsSavePath.EditorButtonClick
        Me.txtSchematicsSavePath.Text = GetFileName(Me.txtSchematicsSavePath.Text)
    End Sub

    Private Sub PdfSaveForm_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        _entitiesBounds = New VectorDraw.Geometry.Box(_vdDocument.ActiveLayOut.Entities.GetBoundingBox(True, False))
        chkScaleToMaximum.Enabled = _entitiesBounds.Width > PDF_MAX_X OrElse _entitiesBounds.Height > PDF_MAX_Y
    End Sub

End Class