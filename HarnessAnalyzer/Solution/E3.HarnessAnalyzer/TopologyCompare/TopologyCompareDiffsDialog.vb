Imports System.ComponentModel
Imports devDept.Eyeshot.Entities
Imports Infragistics.Documents.Excel
Imports Infragistics.Win.UltraWinGrid
Imports Infragistics.Win.UltraWinGrid.ExcelExport
Imports Zuken.E3.Lib.Comparer.Topology.Documents
Imports Zuken.E3.Lib.Eyeshot.Model
Imports Zuken.E3.Lib.IO.KBL

<Reflection.Obfuscation(Feature:="renaming", ApplyToMembers:=False, Exclude:=True)> ' needed to avoid resource-file-names obfuscation errors, see issue #5
Public Class TopologyCompareDiffsDialog

    Private _document As CompareDocument
    Private _exporting As Boolean = False
    Private _exportCancelled As Boolean = False
    Private _exportError As Exception
    Private _lock As New System.Threading.SemaphoreSlim(1)

    Protected Overrides Sub OnLoad(e As EventArgs)
        If _document IsNot Nothing Then
            Me.ugCompareDifferences.DisplayLayout.Bands(0).Groups(0).Header.Caption = IO.Path.GetFileNameWithoutExtension(_document.LeftFile)
            Me.ugCompareDifferences.DisplayLayout.Bands(0).Groups(1).Header.Caption = IO.Path.GetFileNameWithoutExtension(_document.RightFile)

            For Each obj As DocumentObject In _document.Objects
                If obj.LeftEntity?.EntityType = ModelEntityType.Bundle OrElse obj.RightEntity?.EntityType = ModelEntityType.Bundle Then
                    AddNewRows(obj)
                End If
            Next
        End If
        MyBase.OnLoad(e)
    End Sub

    Protected Overrides Sub OnShown(e As EventArgs)
        chkIgnoreColumnSizes.Checked = My.Settings.TopoCompareExportIgnoreColumnSizes
        PerformAutoResize()
        ugCompareDifferences.DisplayLayout.Bands(0).Columns("D1_Segments").PerformAutoResize(PerformAutoSizeType.AllRowsInBand, AutoResizeColumnWidthOptions.All)
        ugCompareDifferences.DisplayLayout.Bands(0).Columns("D2_Segments").PerformAutoResize(PerformAutoSizeType.AllRowsInBand, AutoResizeColumnWidthOptions.All)
        MyBase.OnShown(e)
    End Sub

    Private Sub PerformAutoResize()
        ugCompareDifferences.DisplayLayout.PerformAutoResizeColumns(False, PerformAutoSizeType.AllRowsInBand)
    End Sub

    Private Sub AddNewRows(docObj As DocumentObject, Optional parentSegment As SegmentPair = Nothing)
        Dim leftEntities As New List(Of IEntity)
        Dim rightEntities As New List(Of IEntity)

        If (TypeOf docObj.LeftEntity Is IGroupEntityEx) Then
            leftEntities.AddRange(CType(docObj.LeftEntity, IGroupEntityEx).Flatten)
        Else
            leftEntities.Add(docObj.LeftEntity)
        End If

        If (TypeOf docObj.RightEntity Is IGroupEntityEx) Then
            rightEntities.AddRange(CType(docObj.RightEntity, IGroupEntityEx).Flatten)
        Else
            rightEntities.Add(docObj.RightEntity)
        End If

        Dim maxCount As Integer = Math.Max(leftEntities.Count, rightEntities.Count)
        For i As Integer = 0 To maxCount - 1
            Dim leftEEEntity As IBaseModelEntityEx = TryCast(leftEntities.ElementAtOrDefault(i), IBaseModelEntityEx)
            Dim rightEEEntity As IBaseModelEntityEx = TryCast(rightEntities.ElementAtOrDefault(i), IBaseModelEntityEx)
            Dim leftIds As Guid() = If(leftEEEntity?.GetEEObjectIds, Array.Empty(Of Guid)())
            Dim rightIds As Guid() = If(rightEEEntity?.GetEEObjectIds, Array.Empty(Of Guid)())

            Dim maxIdCount As Integer = Math.Max(leftIds.Length, rightIds.Length)
            For i2 As Integer = 0 To maxIdCount
                Dim leftId As Guid = leftIds.ElementAtOrDefault(i2)
                Dim rightId As Guid = rightIds.ElementAtOrDefault(i2)
                AddNewRow(leftId, rightId, parentSegment)
            Next
        Next
    End Sub

    Private Sub AddNewRow(leftEEId As Guid, rightEEId As Guid, Optional parentSegment As SegmentPair = Nothing)
        Dim leftObjBase As E3.Lib.Model.ObjectBase = If(leftEEId <> Guid.Empty, _document.ModelLeft(leftEEId), Nothing)
        Dim rightObjBase As E3.Lib.Model.ObjectBase = If(rightEEId <> Guid.Empty, _document.ModelRight(rightEEId), Nothing)

        If TypeOf leftObjBase Is E3.Lib.Model.Segment OrElse TypeOf rightObjBase Is E3.Lib.Model.Segment Then
            AddNewSegmentRow(TryCast(leftObjBase, E3.Lib.Model.Segment), TryCast(rightObjBase, E3.Lib.Model.Segment))
        ElseIf TypeOf leftObjBase Is E3.Lib.Model.AdditionalPart OrElse TypeOf rightObjBase Is E3.Lib.Model.AdditionalPart Then
            Dim addPartLeft As E3.Lib.Model.AdditionalPart = TryCast(leftObjBase, E3.Lib.Model.AdditionalPart)
            Dim addPartRight As E3.Lib.Model.AdditionalPart = TryCast(rightObjBase, E3.Lib.Model.AdditionalPart)
            AddRow(parentSegment?.LeftObj, parentSegment?.RightObj, New AdditionalPartPair(addPartLeft, addPartRight))
        End If
    End Sub

    Private Sub AddNewSegmentRow(left As E3.Lib.Model.Segment, right As E3.Lib.Model.Segment)
        Dim leftEnts As List(Of E3.Lib.Model.AdditionalPartAssignment) = If(left?.GetAdditionalPartAssignments.Entries, New List(Of E3.Lib.Model.AdditionalPartAssignment))
        Dim rightEnts As List(Of E3.Lib.Model.AdditionalPartAssignment) = If(right?.GetAdditionalPartAssignments.Entries, New List(Of E3.Lib.Model.AdditionalPartAssignment))
        Dim addAssParts As E3.Lib.Model.AdditionalPartAssignment() = leftEnts.Concat(rightEnts).ToArray
        Dim fixingIds As New HashSet(Of Guid)
        For Each fixing As E3.Lib.Model.AdditionalPartAssignment In addAssParts
            Dim addPart As E3.Lib.Model.AdditionalPart = fixing.GetAdditionalPart
            If addPart.AdditionalPartType = E3.Lib.Model.AdditionalPartType.Fixing Then
                fixingIds.Add(addPart.Id)
            End If
        Next

        If fixingIds.Count > 0 Then
            For Each fixObj As DocumentObject In _document.Objects.FindObjectsByEEModelId(fixingIds.ToArray)
                AddNewRows(fixObj, New SegmentPair(left, right))
            Next
        Else
            AddRow(left, right)
        End If
    End Sub

    Private Sub AddRow(segLeft As E3.Lib.Model.Segment, segRight As E3.Lib.Model.Segment, Optional fixingsPair As AdditionalPartPair = Nothing)
        ' Segment,Length,FixingID,FixPartNumber,FixingPos
        Dim fixBagLeft As KblFixingPropertyBag = TryCast(fixingsPair?.LeftObj?.CustomAttributes.OfType(Of KblPropertyBagAttribute).FirstOrDefault?.PropertyBag, KblFixingPropertyBag)
        Dim fixBagRight As KblFixingPropertyBag = TryCast(fixingsPair?.RightObj?.CustomAttributes.OfType(Of KblPropertyBagAttribute).FirstOrDefault?.PropertyBag, KblFixingPropertyBag)

        Me.UltraDataSource1.Rows.Add({
                              segLeft?.ShortName, GetLengthValue(segLeft, DocumentSide.Left), fixingsPair?.LeftObj?.ShortName, fixingsPair?.LeftObj?.PartNumber, fixBagLeft?.Location,
                              segRight?.ShortName, GetLengthValue(segRight, DocumentSide.Right), fixingsPair?.RightObj?.ShortName, fixingsPair?.RightObj?.PartNumber, fixBagRight?.Location
                             })
    End Sub

    Private Function GetLengthValue(seg As E3.Lib.Model.Segment, side As DocumentSide) As Nullable(Of Integer)
        If seg IsNot Nothing Then
            Dim lc As E3.Lib.Model.LengthClass
            Select Case side
                Case DocumentSide.Left
                    lc = _document.LengthClassLeft
                Case DocumentSide.Right
                    lc = _document.LengthClassRight
                Case Else
                    Throw New NotImplementedException("DocumentSide: " & side.ToString)
            End Select

            Select Case lc
                Case E3.Lib.Model.LengthClass.DMU
                    Return seg.DMULength
                Case E3.Lib.Model.LengthClass.Nominal
                    Return seg.NomLength
                Case E3.Lib.Model.LengthClass.User
                    Return seg.UserLength
            End Select
        End If
        Return Nothing
    End Function

    Public Overloads Function ShowDialog(owner As IWin32Window, document As CompareDocument) As DialogResult
        _document = document
        Return MyBase.ShowDialog(owner)
    End Function

    Private Async Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnExport.Click
        Await _lock.WaitAsync
        Try
            btnExport.Enabled = False
            btnClose.Enabled = False
            If SaveExcelFileDialog.ShowDialog(Me) = DialogResult.OK Then
                _exportError = Nothing
                _exportCancelled = False
                _exporting = True
                Using Me.EnableWaitCursor
                    Dim workbook As New Workbook(GetWorkBookFormat(SaveExcelFileDialog.FileName))
                    UltraGridExcelExporter1.ExportAsync(Me.ugCompareDifferences, workbook)
                    Await TaskEx.WaitUntil(Function() Not _exporting)
                    MergeSameCellValues(workbook.Worksheets(0), "D1_Segments", "D2_Segments") ' HINT: cell merging is not implemented in exporter
                    If chkIgnoreColumnSizes.Checked Then
                        For Each col As WorksheetColumn In workbook.Worksheets(0).Columns
                            col.AutoFitWidth()
                        Next
                    End If
                    workbook.Save(SaveExcelFileDialog.FileName)
                End Using

                If _exportError IsNot Nothing Then
                    Throw _exportError
                End If

                If Not _exportCancelled Then
                    If MessageBoxEx.Show(Me, My.Resources.TopologyCompareTexts.ExportFinishedSuccessfullyOpenQuestion, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                        ProcessEx.Start(SaveExcelFileDialog.FileName)
                    End If
                Else
                    MessageBoxEx.Show(Me, My.Resources.TopologyCompareTexts.ExportCancelledMessage, MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
            End If
        Catch ex As Exception
            MessageBoxEx.ShowError(Me, ex.Message)
        Finally
            btnClose.Enabled = True
            btnExport.Enabled = True
            _lock.Release()
        End Try
    End Sub

    Private Sub MergeSameCellValues(ws As Worksheet, ParamArray colKeys As String())
        Dim indicies As New List(Of Integer)
        For Each col As UltraGridColumn In ugCompareDifferences.DisplayLayout.Bands(0).Columns
            If colKeys.Contains(col.Key) Then
                indicies.Add(col.Index)
            End If
        Next
        MergeSameCellValues(ws, indicies.ToArray)
    End Sub

    Private Sub MergeSameCellValues(ws As Worksheet, ParamArray colIndicies As Integer())
        For Each cI As Integer In colIndicies
            Dim rIdx As Integer = -1
            Dim lastCellValue As String = Nothing
            Dim mergeStartRowIdx As Integer = -1
            Dim mergeCount As Integer = 0

            For Each row As WorksheetRow In ws.Rows
                rIdx += 1
                Dim cell As WorksheetCell = row.Cells(cI)
                Dim cellTxt As String = CStr(If(IsDBNull(cell.Value), String.Empty, CStr(cell.Value)))
                If rIdx = 0 OrElse lastCellValue <> cellTxt OrElse String.IsNullOrWhiteSpace(cellTxt) Then
                    If mergeCount > 1 AndAlso Not String.IsNullOrWhiteSpace(cellTxt) Then
                        ws.MergedCellsRegions.Add(mergeStartRowIdx, cI, rIdx - 1, cI)
                    End If
                    mergeStartRowIdx = rIdx
                    mergeCount = 1
                Else
                    mergeCount += 1
                End If
                lastCellValue = If(IsDBNull(cell.Value), String.Empty, CStr(cell.Value))
            Next
        Next
    End Sub

    Private Function GetWorkBookFormat(fileName As String) As WorkbookFormat
        Select Case IO.KnownFile.GetFileType(fileName)
            Case IO.KnownFile.Type.XLS
                Return WorkbookFormat.Excel97To2003
            Case IO.KnownFile.Type.XLSX
                Return WorkbookFormat.Excel2007
            Case Else
                Throw New NotSupportedException($"File extension ""{IO.Path.GetExtension(fileName)}"" is not supported!")
        End Select
    End Function

    Private Sub UltraGridExcelExporter1_ExportEnded(sender As Object, e As ExportEndedEventArgs) Handles UltraGridExcelExporter1.ExportEnded
        _exportCancelled = e.Canceled
        _exporting = False
    End Sub

    Private Sub UltraGridExcelExporter1_AsynchronousExportError(sender As Object, e As AsynchronousExportErrorEventArgs) Handles UltraGridExcelExporter1.AsynchronousExportError
        e.DisplayErrorMessage = False
        _exportError = e.Exception
        _exporting = False
    End Sub

    Private Sub ugCompareDifferences_InitializeLayout(sender As Object, e As InitializeLayoutEventArgs) Handles ugCompareDifferences.InitializeLayout

    End Sub

    Private Sub TopologyCompareDiffsDialog_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        e.Cancel = _exporting
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Me.DialogResult = DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub ugCompareDifferences_InitializeRow(sender As Object, e As InitializeRowEventArgs) Handles ugCompareDifferences.InitializeRow
        Dim coll As New CellPairCollection
        coll.Add(GetLengthValues(e.Row))
        coll.Add(GetSegmentValues(e.Row))
        coll.Add(GetFixIDValues(e.Row))
        coll.Add(GetFixPNValues(e.Row))
        coll.Add(GetFixPosValues(e.Row))
        coll.UpdateNotEqualColors(Drawing.Color.Orange, Drawing.Color.Black)
    End Sub

    Private Function GetLengthValues(row As UltraGridRow) As CellPair(Of Integer, Integer)
        Return GetCellValues(Of Integer, Integer)(row, "D1_Length", "D2_Length")
    End Function

    Private Function GetSegmentValues(row As UltraGridRow) As CellPair(Of String, String)
        Return GetCellValues(Of String, String)(row, "D1_Segments", "D2_Segments")
    End Function

    Private Function GetFixIDValues(row As UltraGridRow) As CellPair(Of String, String)
        Return GetCellValues(Of String, String)(row, "D1_FixingID", "D2_FixingID")
    End Function

    Private Function GetFixPNValues(row As UltraGridRow) As CellPair(Of String, String)
        Return GetCellValues(Of String, String)(row, "D1_Fixing_PartNumber", "D2_Fixing_PartNumber")
    End Function

    Private Function GetFixPosValues(row As UltraGridRow) As CellPair(Of String, String)
        Return GetCellValues(Of String, String)(row, "D1_FixingPos", "D2_FixingPos")
    End Function

    Private Function GetCellValues(Of TLeft, TRight)(row As UltraGridRow, keyLeft As String, keyRight As String) As CellPair(Of TLeft, TRight)
        Dim leftCell As UltraGridCell = row.Cells(keyLeft)
        Dim rightCell As UltraGridCell = row.Cells(keyRight)
        Return New CellPair(Of TLeft, TRight)(leftCell, rightCell)
    End Function

    Private MustInherit Class CellPairBase

        Public Sub New(leftCell As UltraGridCell, rightCell As UltraGridCell)
            Me.LeftCell = leftCell
            Me.RightCell = rightCell
        End Sub

        Public ReadOnly Property LeftCell As UltraGridCell
        Public ReadOnly Property RightCell As UltraGridCell

        Protected Friend MustOverride Function IsEqual() As Boolean

        Public Sub SetBackground(color As Drawing.Color)
            LeftCell.Appearance.BackColor = color
            RightCell.Appearance.BackColor = color
        End Sub

        Public Sub SetForeground(color As Drawing.Color)
            LeftCell.Appearance.ForeColor = color
            RightCell.Appearance.ForeColor = color
        End Sub

    End Class

    Private Class CellPair(Of TLeft, TRight)
        Inherits CellPairBase
        Public Sub New(leftCell As UltraGridCell, rightCell As UltraGridCell)
            MyBase.New(leftCell, rightCell)
            Left = If(IsDBNull(leftCell.Value), Nothing, CType(leftCell.Value, TLeft))
            Right = If(IsDBNull(rightCell.Value), Nothing, CType(rightCell.Value, TRight))
        End Sub

        Public ReadOnly Property Left As TLeft
        Public ReadOnly Property Right As TRight

        Protected Friend Overrides Function IsEqual() As Boolean
            Return (Right Is Nothing AndAlso Left Is Nothing) OrElse ((Right IsNot Nothing AndAlso Right.Equals(Left)) OrElse (Left IsNot Nothing AndAlso Left.Equals(Right)))
        End Function
    End Class

    Private Class CellPairCollection
        Inherits System.Collections.ObjectModel.Collection(Of CellPairBase)

        Public Sub UpdateNotEqualColors(backcolor As Drawing.Color, Optional foreColor As Nullable(Of Color) = Nothing)
            For Each pair As CellPairBase In Me
                If Not pair.IsEqual Then
                    pair.SetBackground(backcolor)
                    If Not foreColor.HasValue Then
                        pair.SetForeground(backcolor.GetContrast)
                    Else
                        pair.SetForeground(foreColor.Value)
                    End If
                End If
            Next
        End Sub

    End Class

    Private Class SegmentPair
        Inherits ModelObjectPair
        Public Sub New(leftSegment As E3.Lib.Model.Segment, rightSegment As E3.Lib.Model.Segment)
            MyBase.New(leftSegment, rightSegment)
        End Sub

        Public Shadows ReadOnly Property LeftObj As E3.Lib.Model.Segment
            Get
                Return CType(MyBase.LeftObj, E3.Lib.Model.Segment)
            End Get
        End Property

        Public Shadows ReadOnly Property RightObj As E3.Lib.Model.Segment
            Get
                Return CType(MyBase.RightObj, E3.Lib.Model.Segment)
            End Get
        End Property
    End Class

    Private Class AdditionalPartPair
        Inherits ModelObjectPair
        Public Sub New(left As E3.Lib.Model.AdditionalPart, right As E3.Lib.Model.AdditionalPart)
            MyBase.New(left, right)
        End Sub

        Public Shadows ReadOnly Property LeftObj As E3.Lib.Model.AdditionalPart
            Get
                Return CType(MyBase.LeftObj, E3.Lib.Model.AdditionalPart)
            End Get
        End Property

        Public Shadows ReadOnly Property RightObj As E3.Lib.Model.AdditionalPart
            Get
                Return CType(MyBase.RightObj, E3.Lib.Model.AdditionalPart)
            End Get
        End Property
    End Class

    Private Class ModelObjectPair
        Public Sub New(leftObj As E3.Lib.Model.ObjectBase, rightObj As E3.Lib.Model.ObjectBase)
            Me.LeftObj = leftObj
            Me.RightObj = rightObj
        End Sub

        Public ReadOnly Property LeftObj As E3.Lib.Model.ObjectBase
        Public ReadOnly Property RightObj As E3.Lib.Model.ObjectBase
    End Class

    Private Sub TopologyCompareDiffsDialog_Closed(sender As Object, e As EventArgs) Handles Me.Closed
        My.Settings.TopoCompareExportIgnoreColumnSizes = chkIgnoreColumnSizes.Checked
    End Sub

    Private Sub AutoSizeColumnsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AutoSizeColumnsToolStripMenuItem.Click
        PerformAutoResize()
    End Sub

    Private Sub CopyToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CopyToolStripMenuItem.Click
        ugCompareDifferences.PerformAction(UltraGridAction.Copy)
    End Sub

    Private Sub ContextMenuStrip1_Opening(sender As Object, e As CancelEventArgs) Handles ContextMenuStrip1.Opening
        CopyToolStripMenuItem.Enabled = ugCompareDifferences.Selected.Rows.Count > 0
    End Sub

End Class
