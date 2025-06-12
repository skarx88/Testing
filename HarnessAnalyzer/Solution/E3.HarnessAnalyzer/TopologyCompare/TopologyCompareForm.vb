Imports System.ComponentModel
Imports System.Threading
Imports devDept.Eyeshot
Imports devDept.Eyeshot.Entities
Imports Infragistics.Win.UltraWinEditors
Imports Zuken.E3.App.Windows.Controls.Comparer.Topology
Imports Zuken.E3.Lib.Comparer.Topology.Designs
Imports Zuken.E3.Lib.Comparer.Topology.Documents
Imports Zuken.E3.Lib.Eyeshot.Model
Imports Zuken.E3.Lib.IO.KBL

<Reflection.Obfuscation(Feature:="renaming", ApplyToMembers:=False, Exclude:=True)> ' needed to avoid resource-file-names obfuscation errors, see issue #5
Public Class TopologyCompareForm
    Inherits Form

    Public Event ProgressChanged As ProgressChangedEventHandler
    Public Event WorkFinished As EventHandler(Of WorkFinishedEventArgs)
    Public Event SelectionChanged(sender As Object, e As ModelSelectionChangedEventArgs)

    Private Const SEGMENTS_ANNOTATION_KEY As String = "segments"
    Private Const VERTICES_ANNOTATION_KEY As String = "vertices"
    Private Const FIXINGS_ANNOTATION_KEY As String = "fixings"

    Private _lastOverrideValue As Double
    Private _segmentItem As Infragistics.Win.ValueListItem
    Private _verticesItem As Infragistics.Win.ValueListItem
    Private _fixingsItem As Infragistics.Win.ValueListItem
    Private _storedbundleRadius As System.Collections.Generic.Dictionary(Of IEntity, Integer) = New System.Collections.Generic.Dictionary(Of IEntity, Integer)()
    Private _semUpdate As SemaphoreSlim = New SemaphoreSlim(1)
    Private _lockUpdatAnnotations As New SemaphoreSlim(1)
    Private _selecting As Boolean = False

    Private WithEvents _documentLeft As DocumentForm
    Private WithEvents _documentRight As DocumentForm
    Private WithEvents topologyComparerCtrl1 As TopologyComparerControl

    Public Sub New()
        InitializeComponent()
        InitTopoCompareControl()
        InitCheckAnnotations()
        txtOverrideRadius.Value = BundleCompareEntity.DEFAULT_RADIUS
        With topologyComparerCtrl1
            .Designs(DocumentSide.Left).SetAllInitialViews(viewType.Top)
            .Designs(DocumentSide.Right).SetAllInitialViews(viewType.Top)
        End With
    End Sub

    Private Sub InitCheckAnnotations()
        _segmentItem = New Infragistics.Win.ValueListItem(SEGMENTS_ANNOTATION_KEY, My.Resources.AnnotationCheckBoxStrings.Segments)
        _verticesItem = New Infragistics.Win.ValueListItem(VERTICES_ANNOTATION_KEY, My.Resources.AnnotationCheckBoxStrings.Vertices)
        _fixingsItem = New Infragistics.Win.ValueListItem(FIXINGS_ANNOTATION_KEY, My.Resources.AnnotationCheckBoxStrings.Fixings)

        cmbAnnotations.Items.Add(_segmentItem)
        cmbAnnotations.Items.Add(_verticesItem)
        cmbAnnotations.Items.Add(_fixingsItem)
    End Sub

    Private Sub TopologyComparerCtrl1_ProgressChanged(ByVal sender As Object, ByVal e As ProgressChangedEventArgs)
        RaiseEvent ProgressChanged(Me, e)
    End Sub

    Private Sub TopologyComparerCtrl1_WorkFinished(ByVal sender As Object, ByVal e As WorkFinishedEventArgs)
        RaiseEvent WorkFinished(Me, New WorkFinishedEventArgs(e.Success))
    End Sub

    Public ReadOnly Property Document As CompareDocument
        Get
            Return TryCast(Me.topologyComparerCtrl1.Document, CompareDocument)
        End Get
    End Property

    Public Async Function SetDocumentAsync(value As CompareDocument) As Task(Of Boolean)
        Return Await Me.topologyComparerCtrl1.SetDocumentAsync(value)
    End Function

    Private Sub InitTopoCompareControl()
        Me.topologyComparerCtrl1 = New TopologyComparerControl
        Me.topologyComparerCtrl1.AllowedSelectionCount = 2147483647
        Me.topologyComparerCtrl1.Dock = DockStyle.Fill
        Me.topologyComparerCtrl1.AnnotationsState = Zuken.E3.App.Windows.Controls.Comparer.Topology.AnnotationsState.Disabled
        Me.topologyComparerCtrl1.AutoSyncCameraLeft = True
        Me.topologyComparerCtrl1.AutoSyncCameraRight = True
        'Me.topologyComparerCtrl1.LeftDebugInfoForm = Nothing
        Me.topologyComparerCtrl1.Name = "topologyComparerCtrl1"
        'Me.topologyComparerCtrl1.RightDebugInfoForm = Nothing
        Me.Panel1.Controls.Add(Me.topologyComparerCtrl1)
    End Sub

    Public Sub RestoreAnnotationState()
        _segmentItem.CheckState = CheckState.Unchecked
        _verticesItem.CheckState = CheckState.Unchecked
        _fixingsItem.CheckState = CheckState.Unchecked
        Dim savedState As AnnotationsState = CType(My.Settings.AnnotationsState, AnnotationsState)

        If savedState = AnnotationsState.AllEnabled Then
            RemoveHandler cmbAnnotations.ValueChanged, AddressOf cmbAnnotations_ValueChanged
            _segmentItem.CheckState = CheckState.Checked
            _fixingsItem.CheckState = CheckState.Checked
            AddHandler cmbAnnotations.ValueChanged, AddressOf cmbAnnotations_ValueChanged
            _verticesItem.CheckState = CheckState.Checked
        Else

            If savedState.HasFlag(AnnotationsState.SegmentsEnabled) Then
                _segmentItem.CheckState = CheckState.Checked
            End If

            If savedState.HasFlag(AnnotationsState.VerticesEnabled) Then
                _verticesItem.CheckState = CheckState.Checked
            End If

            If savedState.HasFlag(AnnotationsState.FixingsEnabled) Then
                _fixingsItem.CheckState = CheckState.Checked
            End If
        End If
    End Sub

    Protected Overrides Sub OnClosing(ByVal e As CancelEventArgs)
        MyBase.OnClosing(e)

        If Not e.Cancel AndAlso Me.topologyComparerCtrl1.IsBusy Then
            e.Cancel = True
            topologyComparerCtrl1.Cancel()

            Using cancellationPendingForm As CancellationPendingForm = New CancellationPendingForm()
                Dim task As Task = Task.Factory.StartNew(Sub() SpinWait.SpinUntil(Function() Not topologyComparerCtrl1.IsBusy))
                task.ContinueWith(Sub(t)
                                      cancellationPendingForm.Invoke(CType((Sub() cancellationPendingForm.Close()), Action))
                                  End Sub)
                If Not task.IsCompleted Then cancellationPendingForm.ShowDialog(Me)
                Close()
            End Using
        End If
    End Sub

    Private Sub btnClose_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnClose.Click
        Me.Close()
    End Sub

    Private Async Sub cmbAnnotations_ValueChanged(ByVal sender As Object, ByVal e As EventArgs) Handles cmbAnnotations.ValueChanged
        Await _lockUpdatAnnotations.WaitAsync()
        Try
            Dim newState As AnnotationsState = AnnotationsState.Disabled

            If _segmentItem.CheckState = CheckState.Checked Then
                newState = newState Or AnnotationsState.SegmentsEnabled
            End If

            If _verticesItem.CheckState = CheckState.Checked Then
                newState = newState Or AnnotationsState.VerticesEnabled
            End If

            If _fixingsItem.CheckState = CheckState.Checked Then
                newState = newState Or AnnotationsState.FixingsEnabled
            End If

            My.Settings.AnnotationsState = CInt(newState)
            Await topologyComparerCtrl1.SetAnnotationStateAsync(newState, True)
        Finally
            _lockUpdatAnnotations.Release()
        End Try
    End Sub

    Private Async Sub UpdateEntitiesRadius()
        Await Task.Factory.StartNew(Sub() SpinWait.SpinUntil(Function() Not topologyComparerCtrl1.IsBusy))
        Await _semUpdate.WaitAsync()

        Try
            _lastOverrideValue = CDbl(txtOverrideRadius.Value)

            If UseOverrideRadius Then
                Await topologyComparerCtrl1.SetEntitiesRadius(CDbl(txtOverrideRadius.Value))
            Else
                Await topologyComparerCtrl1.SetEntitiesRadius(Nothing)
            End If

        Finally
            _semUpdate.Release()
        End Try
    End Sub

    Private ReadOnly Property UseOverrideRadius As Boolean
        Get
            Return (CType(txtOverrideRadius.ButtonsLeft(0), StateEditorButton)).Checked
        End Get
    End Property

    Private ReadOnly Property OverrideValueHasChanged As Boolean
        Get
            Return (_lastOverrideValue <> CDbl(txtOverrideRadius.Value))
        End Get
    End Property

    Private Sub txtOverrideRadius_EditorButtonClick_1(ByVal sender As Object, ByVal e As EditorButtonEventArgs) Handles txtOverrideRadius.EditorButtonClick
        UpdateEntitiesRadius()
    End Sub

    Private Sub resetToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles resetToolStripMenuItem.Click
        txtOverrideRadius.Value = BundleCompareEntity.DEFAULT_RADIUS

        If UseOverrideRadius AndAlso OverrideValueHasChanged Then
            UpdateEntitiesRadius()
        End If
    End Sub

    Private Sub txtOverrideRadius_Validated(ByVal sender As Object, ByVal e As EventArgs) Handles txtOverrideRadius.Validated
        If OverrideValueHasChanged Then
            UpdateEntitiesRadius()
        End If
    End Sub

    Private Sub topologyComparerCtrl1_GetTextLeft(sender As Object, e As TextEventArgs) Handles topologyComparerCtrl1.GetTextLeft
        If IO.PathEx.IsValidFileName(e.Text) Then
            e.Text = IO.Path.GetFileNameWithoutExtension(e.Text)
        End If
    End Sub

    Private Sub topologyComparerCtrl1_GetTextRight(sender As Object, e As TextEventArgs) Handles topologyComparerCtrl1.GetTextRight
        If IO.PathEx.IsValidFileName(e.Text) Then
            e.Text = IO.Path.GetFileNameWithoutExtension(e.Text)
        End If
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Using dlg As New TopologyCompareDiffsDialog
            dlg.ShowDialog(Me, Document)
        End Using
    End Sub

    Private Sub topologyComparerCtrl1_BeforeToolTipShown(sender As Object, e As BeforeToolTipShownEventArgs) Handles topologyComparerCtrl1.BeforeToolTipShown
        'HINT: overwrite default length shown in tooltip when entity is bundle by selected length classes
        Select Case e.Txt.EntityType
            Case ModelEntityType.Bundle
                If TypeOf e.Txt.EntityData Is SegmentEEData Then
                    Select Case e.Side
                        Case DocumentSide.Left
                            e.Txt.SetSegmentLength(Document.LengthClassLeft)
                        Case DocumentSide.Right
                            e.Txt.SetSegmentLength(Document.LengthClassRight)
                    End Select
                End If
        End Select
    End Sub

    Private Sub topologyComparerCtrl1_SelectionChanged(sender As Object, e As ModelSelectionChangedEventArgs) Handles topologyComparerCtrl1.SelectionChanged
        If Not _selecting Then
            _selecting = True
            Try
                OnSelectionChanged(e)
            Finally
                _selecting = False
            End Try
        End If
    End Sub

    Protected Overridable Sub OnSelectionChanged(e As ModelSelectionChangedEventArgs)
        RaiseEvent SelectionChanged(Me, e)
    End Sub

    Private Sub TopologyCompareForm_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        _documentLeft = My.Application.MainForm.GetAllDocuments.Where(Function(doc) doc.Value.File.FullName = Me.topologyComparerCtrl1.Document.LeftFile).Single.Value
        _documentRight = My.Application.MainForm.GetAllDocuments.Where(Function(doc) doc.Value.File.FullName = Me.topologyComparerCtrl1.Document.RightFile).Single.Value
    End Sub

    Private Sub _documentRight_HubSelectionChanged(sender As Object, e As InformationHubEventArgs) Handles _documentRight.HubSelectionChanged, _documentLeft.HubSelectionChanged
        If Not _selecting Then
            _selecting = True
            Try
                Dim side As DocumentSide = If(sender Is _documentLeft, DocumentSide.Left, DocumentSide.Right)
                SelectKblIds(e.ObjectIds.ToArray, side)
            Finally
                _selecting = False
            End Try
        End If
    End Sub

    Private Sub _documentLeft_CanvasSelectionChanged(sender As Object, e As CanvasSelectionChangedEventArgs) Handles _documentLeft.CanvasSelectionChanged, _documentRight.CanvasSelectionChanged
        If Not _selecting Then
            _selecting = True
            Try
                Dim side As DocumentSide = If(sender Is _documentLeft, DocumentSide.Left, DocumentSide.Right)
                Dim kblSelectedIds As String() = DirectCast(sender, DocumentForm).GetSelectedKblIds()
                SelectKblIds(kblSelectedIds, side)
            Finally
                _selecting = False
            End Try
        End If
    End Sub

    Private Sub SelectKblIds(kblIds As String(), sourceSide As DocumentSide)
        Dim selectedObjs As New List(Of E3.Lib.Model.ObjectBaseNaming)
        Dim dic As Dictionary(Of String, HashSet(Of E3.Lib.Model.ObjectBaseNaming)) = Nothing

        Select Case sourceSide
            Case DocumentSide.Left
                dic = Me.topologyComparerCtrl1.Document.ModelLeft.OfType(Of E3.Lib.Model.ObjectBaseNaming).SelectMany(Function(o) o.GetKblIds.Select(Function(id) New KeyValuePair(Of String, E3.Lib.Model.ObjectBaseNaming)(id, o))).GroupBy(Function(kv) kv.Key).ToDictionary(Function(grp) grp.Key, Function(grp) grp.Select(Function(kv) kv.Value).ToHashSet)
            Case DocumentSide.Right
                dic = Me.topologyComparerCtrl1.Document.ModelRight.OfType(Of E3.Lib.Model.ObjectBaseNaming).SelectMany(Function(o) o.GetKblIds.Select(Function(id) New KeyValuePair(Of String, E3.Lib.Model.ObjectBaseNaming)(id, o))).GroupBy(Function(kv) kv.Key).ToDictionary(Function(grp) grp.Key, Function(grp) grp.Select(Function(kv) kv.Value).ToHashSet)
            Case Else
                Throw New NotImplementedException($"Document side ""{sourceSide.ToString}"" not implemented!")
        End Select

        For Each id As String In kblIds
            If dic.ContainsKey(id) Then
                selectedObjs.AddRange(dic(id))
            End If
        Next

        Select Case sourceSide
            Case DocumentSide.Left
                Me.topologyComparerCtrl1.SelectedEntitiesLeft.Clear()
            Case DocumentSide.Right
                Me.topologyComparerCtrl1.SelectedEntitiesRight.Clear()
        End Select

        If selectedObjs.Count > 0 Then
            Me.topologyComparerCtrl1.TrySelectObjectEntitiesByModelObjectIds(selectedObjs.Select(Function(o) o.Id).ToArray)
        End If

        Me.topologyComparerCtrl1.ZoomFitAllSelection()
        Me.topologyComparerCtrl1.InvalidateAllEntities()
    End Sub


End Class

