Imports Infragistics.Win
Imports Zuken.E3.HarnessAnalyzer.Project.Documents
Imports Zuken.E3.Lib.Comparer.Topology.Documents
Imports Zuken.E3.Lib.IO.Files.Hcv

<Reflection.Obfuscation(Feature:="renaming", ApplyToMembers:=False, Exclude:=True)> ' needed to avoid resource-file-names obfuscation errors, see issue #5
Public Class TopologyCompareWizardFileSelector

    Public Event SelectedDocumentChanged(sender As Object, e As EventArgs)

    Private _isSelecting As Boolean = False
    Private _currentCompLengthClass As E3.Lib.Model.LengthClass
    Private _currentRefLengthClass As E3.Lib.Model.LengthClass
    Private _hcvLeft As HcvFile = Nothing

    Public Sub New()
        InitializeComponent()
    End Sub

    Protected Overrides Sub OnLoad(e As EventArgs)
        MyBase.OnLoad(e)

        For Each doc As HcvDocument In My.Application.MainForm.Project.Documents.Cast(Of HcvDocument) 'HINT: currently no open-check (we only need the file-name) because the full document impementation is not ready and the open is skipped when no JT-Data is available because the current usage for document is mostly for 3D document part (future replacement of the old structure planned...)
            If doc.FullName.ToLower <> FileLeft?.ToLower Then
                cmbRightDocument.Items.Add(doc, doc.Caption)
            End If
        Next

        If cmbRightDocument.Items.Count > 0 Then
            SelectFileRightNotEqualLeft()
            SyncFileRightToDocument()
        Else
            cmbRightDocument.Items.Add(Nothing, My.Resources.TopologyCompareTexts.NoOtherDocumentAvailablePlaceHolderText)
            cmbRightDocument.SelectedIndex = 0
            cmbRightDocument.Enabled = False
        End If

        RefreshLengthClassItems(cmbLengthClassComp, _currentCompLengthClass)
        RefreshLengthClassItems(cmbLengthClassRef, _currentRefLengthClass)
    End Sub

    Private Function GetDocument(filePath As String) As HcvDocument
        For Each doc As HcvDocument In My.Application.MainForm.Project.Documents.Cast(Of HcvDocument) 'HINT: currently no open-check (we only need the file-name) because the full document impementation is not ready and the open is skipped when no JT-Data is available because the current usage for document is mostly for 3D document part (future replacement of the old structure planned...)
            If doc.FullName.ToLower = filePath?.ToLower Then
                Return doc
            End If
        Next
        Return Nothing
    End Function

    Private Sub cmbDocument_ValueChanged(sender As Object, e As EventArgs) Handles cmbRightDocument.ValueChanged
        Dim docRight As HcvDocument = CType(cmbRightDocument.Value, HcvDocument)
        HcvRight = docRight?.File

        OnSelectedDocumentChanged(New EventArgs)
    End Sub

    Private Sub RefreshLengthClassItems(cmb As UltraWinEditors.UltraComboEditor, currentLengthClass As E3.Lib.Model.LengthClass)
        Dim currentLcOrDefault As E3.Lib.Model.LengthClass = If(currentLengthClass = E3.Lib.Model.LengthClass.User, E3.Lib.Model.LengthClass.DMU, currentLengthClass)

        cmb.Items.Clear()
        Dim currentItem As ValueListItem = AddAllLengthClassItems(cmb.Items, E3.Lib.Model.LengthClass.User, currentLcOrDefault)
        If currentItem IsNot Nothing Then
            cmb.SelectedItem = currentItem
        End If
    End Sub

    Private Function AddAllLengthClassItems(items As ValueListItemsCollection, Optional exceptLengthClass As Nullable(Of E3.Lib.Model.LengthClass) = Nothing, Optional returnLengthClassItem As Nullable(Of E3.Lib.Model.LengthClass) = Nothing) As ValueListItem
        Dim resultItem As ValueListItem = Nothing
        For Each lc As E3.Lib.Model.LengthClass In EnumEx(Of E3.Lib.Model.LengthClass).GetValues
            If Not exceptLengthClass.HasValue OrElse lc <> exceptLengthClass.Value Then
                Dim item As ValueListItem = items.Add(lc, GetLengthClassResourceText(lc))
                If returnLengthClassItem.HasValue AndAlso lc = returnLengthClassItem.Value Then
                    resultItem = item
                End If
            End If
        Next
        Return resultItem
    End Function

    Private Function GetLengthClassResourceText(lengthClass As E3.Lib.Model.LengthClass) As String
        Select Case lengthClass
            Case E3.Lib.Model.LengthClass.DMU
                Return GeneralSettingsFormStrings.VirtualLength
            Case E3.Lib.Model.LengthClass.Nominal
                Return GeneralSettingsFormStrings.PhysicalLength
            Case E3.Lib.Model.LengthClass.User
                Return E3.Lib.Model.LengthClass.User.ToString
            Case Else
                Throw New NotImplementedException($"length class ""{lengthClass}"" not implemented!")
        End Select
    End Function

    Public Overrides Property FileLeft As String
        Get
            Return MyBase.FileLeft
        End Get
        Set(value As String)
            MyBase.FileLeft = value

            If Not String.IsNullOrEmpty(value) Then
                txtLeftFilePath.Text = IO.Path.GetFileNameWithoutExtension(value)
            Else
                txtLeftFilePath.Text = String.Empty
            End If

            cmbLengthClassRef.Clear()
            Dim docLeft As HcvDocument = GetDocument(value)
            If docLeft IsNot Nothing Then
                RefreshLengthClassItems(cmbLengthClassRef, _currentRefLengthClass)
            End If
        End Set
    End Property

    Public Overrides Property FileRight As String
        Get
            Return MyBase.FileRight
        End Get
        Set(value As String)
            MyBase.FileRight = value

            If Not _isSelecting Then
                Try
                    _isSelecting = True
                    SyncFileRightToDocument()
                Finally
                    _isSelecting = False
                End Try
            End If
        End Set
    End Property

    Public Property HcvRight As HcvFile
        Get
            If cmbRightDocument.SelectedItem IsNot Nothing Then
                Return CType(cmbRightDocument.SelectedItem.DataValue, HcvDocument)?.File
            End If
            Return Nothing
        End Get
        Set(value As HcvFile)
            If value IsNot Nothing Then
                For Each item As ValueListItem In cmbRightDocument.Items
                    Dim doc As HcvDocument = CType(item.DataValue, HcvDocument)
                    If value Is doc.File Then
                        cmbRightDocument.SelectedItem = item
                        Return
                    End If
                Next
                Throw New Exception("Can't set hcv file because it does not exist within available documents!")
            Else
                cmbRightDocument.Value = Nothing
            End If
        End Set
    End Property

    Public Property HcvLeft As HcvFile
        Get
            Return _hcvLeft
        End Get
        Set(value As HcvFile)
            _hcvLeft = value
            Me.FileLeft = _hcvLeft?.FullName
        End Set
    End Property

    Private Sub SelectFileRightNotEqualLeft()
        cmbRightDocument.SelectedIndex = 0
        For Each item As ValueListItem In cmbRightDocument.Items
            Dim doc As HcvDocument = CType(item.DataValue, HcvDocument)
            If FileLeft.ToLower <> doc.FullName.ToLower Then
                cmbRightDocument.SelectedItem = item
                Exit For
            End If
        Next
    End Sub

    Private Sub SyncFileRightToDocument()
        For Each item As ValueListItem In cmbRightDocument.Items
            Dim doc As HcvDocument = CType(item.DataValue, HcvDocument)
            If FileRight.ToLower = doc?.FullName.ToLower Then
                cmbRightDocument.SelectedItem = item
                Exit For
            End If
        Next
    End Sub

    Protected Overridable Sub OnSelectedDocumentChanged(e As EventArgs)
        RaiseEvent SelectedDocumentChanged(Me, e)
    End Sub

    Private Sub cmbLengthClassComp_ValueChanged(sender As Object, e As EventArgs) Handles cmbLengthClassComp.ValueChanged
        If cmbLengthClassComp.Value IsNot Nothing Then
            _currentCompLengthClass = CType(cmbLengthClassComp.Value, E3.Lib.Model.LengthClass)
        Else
            _currentCompLengthClass = E3.Lib.Model.LengthClass.DMU
        End If
    End Sub

    Private Sub cmbLengthClassRef_ValueChanged(sender As Object, e As EventArgs) Handles cmbLengthClassRef.ValueChanged
        If cmbLengthClassRef.Value IsNot Nothing Then
            _currentRefLengthClass = CType(cmbLengthClassRef.Value, E3.Lib.Model.LengthClass)
        Else
            _currentRefLengthClass = E3.Lib.Model.LengthClass.DMU
        End If
    End Sub

    Property SelectedRefLengthClass As E3.Lib.Model.LengthClass
        Get
            Return _currentRefLengthClass
        End Get
        Set(value As E3.Lib.Model.LengthClass)
            If Not cmbLengthClassRef?.IsDisposed Then
                cmbLengthClassRef.Value = value
            End If
            _currentRefLengthClass = value
        End Set
    End Property

    Property SelectedCompareLengthClass As E3.Lib.Model.LengthClass
        Get
            Return _currentCompLengthClass
        End Get
        Set(value As E3.Lib.Model.LengthClass)
            If Not cmbLengthClassComp?.IsDisposed Then
                cmbLengthClassComp.Value = value
            End If
            _currentCompLengthClass = value
        End Set
    End Property

    Public Overrides ReadOnly Property IsValid As Boolean
        Get
            Return (HcvLeft?.KblDocument?.HasData).GetValueOrDefault AndAlso (HcvRight?.KblDocument?.HasData).GetValueOrDefault
        End Get
    End Property

    Protected Overrides Function GetNewDocument() As EEModelsDocument
        Return New HarnessAnalyzerEEModelsDocument(HcvLeft, HcvRight)
    End Function

End Class
