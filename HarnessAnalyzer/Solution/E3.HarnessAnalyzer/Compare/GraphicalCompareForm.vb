Imports System.IO
Imports System.Reflection
Imports Infragistics.Documents.Excel
Imports Infragistics.Win.UltraWinDataSource
Imports Infragistics.Win.UltraWinGrid
Imports Infragistics.Win.UltraWinToolbars
Imports Infragistics.Win.UltraWinToolTip
Imports VectorDraw.Geometry
Imports VectorDraw.Professional
Imports VectorDraw.Professional.vdFigures
Imports VectorDraw.Professional.vdObjects
Imports VectorDraw.Professional.vdObjects.vdDocument
Imports VectorDraw.Professional.vdPrimaries
Imports Zuken.E3.HarnessAnalyzer.Settings

<Reflection.Obfuscation(Feature:="renaming", ApplyToMembers:=False, Exclude:=True)> ' needed to avoid resource-file-names obfuscation errors, see issue #5
Public Class GraphicalCompareForm

    Private _areaRectCompare As VdSVGGroup
    Private _areaRectRef As VdSVGGroup
    Private _checkedCompareResult As CheckedCompareResult
    Private _clickedInView As Boolean
    Private _clickedInGrid As Boolean
    Private _clickPoint As gPoint
    Private _compareDocument As DocumentForm
    Private _compareDrawing As DrawingCanvas
    Private _compareLayout As vdPrimaries.vdLayout
    Private _compareRunning As Boolean
    Private _compareSVGFile As String
    Private _contextMenuButton As PopupMenuTool
    Private _contextMenuGrid As PopupMenuTool
    Private _ctrlKeyPressed As Boolean
    Private _exportCancelled As Boolean
    Private _exportFileName As String
    Private _exportRunning As Boolean
    Private _harnessPartNumberMismatch As Boolean
    Private _header As Infragistics.Win.UltraWinGrid.ColumnHeader
    Private _hiddenGroups As List(Of VdSVGGroup)
    Private _isDirty As Boolean
    Private _isPanning As Boolean
    Private _lastWindowState As FormWindowState
    Private _moveDistanceTolerance As Integer
    Private _previousResultInformation As Dictionary(Of Integer, Tuple(Of Boolean, Boolean, String))
    Private _refDocument As DocumentForm
    Private _refDrawing As DrawingCanvas
    Private _refLayout As vdPrimaries.vdLayout
    Private _refSVGFile As String
    Private _thresholdNofInstancesWithIdenticalOffsetValue As Single
    Private _undoActionRunning As Boolean
    Private _undoActionTriggered As Boolean
    Private _undoEnabled As Boolean = True
    Private _vertexPositionPairs As Dictionary(Of String, List(Of gPoint))
    Private _viewCenterCompare As gPoint
    Private _viewCenterRef As gPoint
    Private _viewSizeCompare As Double
    Private _viewSizeRef As Double
    Private _factory As New SvgFactory

    Private Const LAYER_CHANGE_INDICATORS As String = "ChangeIndicators"

    Public Sub New(refDocument As DocumentForm, compareDocuments As IEnumerable(Of IDocumentForm), moveDistanceTolerance As Integer, thresholdNofInstancesWithIdenticalOffsetValue As Single)
        InitializeComponent()

        _lastWindowState = FormWindowState.Normal
        _moveDistanceTolerance = moveDistanceTolerance

        _refDocument = refDocument
        _thresholdNofInstancesWithIdenticalOffsetValue = thresholdNofInstancesWithIdenticalOffsetValue

        Initialize(compareDocuments)
    End Sub

    Private Sub CalculateDrawingOffset()
        _vertexPositionPairs = New Dictionary(Of String, List(Of gPoint))

        If (_refDrawing Is Nothing) OrElse (_compareDrawing Is Nothing) Then
            Exit Sub
        End If

        For Each group As VdSVGGroup In _refDrawing.vdCanvas.BaseControl.ActiveDocument.Model.Entities
            If (group.SymbolType = SvgSymbolType.Vertex.ToString) AndAlso (group.SVGType = SvgType.Undefined.ToString) AndAlso (Not group.BoundingBox.IsEmpty) Then
                If (Not _vertexPositionPairs.ContainsKey(group.KblId)) Then
                    _vertexPositionPairs.Add(group.KblId, New List(Of gPoint))
                    _vertexPositionPairs(group.KblId).Add(New gPoint(Math.Round(group.BoundingBox.MidPoint.x, 2), Math.Round(group.BoundingBox.MidPoint.y, 2)))
                End If
            End If
        Next

        For Each group As VdSVGGroup In _compareDrawing.vdCanvas.BaseControl.ActiveDocument.Model.Entities
            If (group.SymbolType = SvgSymbolType.Vertex.ToString) AndAlso (group.SVGType = SvgType.Undefined.ToString) AndAlso (Not group.BoundingBox.IsEmpty) Then
                If (_vertexPositionPairs.ContainsKey(group.KblId)) Then
                    _vertexPositionPairs(group.KblId).Add(New gPoint(Math.Round(group.BoundingBox.MidPoint.x, 2), Math.Round(group.BoundingBox.MidPoint.y, 2)))
                Else
                    _vertexPositionPairs.Add(group.KblId, New List(Of gPoint))
                    _vertexPositionPairs(group.KblId).Add(Nothing)
                    _vertexPositionPairs(group.KblId).Add(New gPoint(Math.Round(group.BoundingBox.MidPoint.x, 2), Math.Round(group.BoundingBox.MidPoint.y, 2)))
                End If
            End If
        Next

        Dim offsets As New Dictionary(Of String, Integer)

        For Each vertexPositionPair As List(Of gPoint) In _vertexPositionPairs.Values
            If (vertexPositionPair.Count = 2) AndAlso (vertexPositionPair(0) IsNot Nothing) Then
                Dim offset As New gPoint(Math.Round(vertexPositionPair(0).x - vertexPositionPair(1).x, 2), Math.Round(vertexPositionPair(0).y - vertexPositionPair(1).y, 2))

                If (Not offsets.ContainsKey(String.Format("{0}|{1}", offset.x, offset.y))) Then
                    offsets.Add(String.Format("{0}|{1}", offset.x, offset.y), 1)
                Else
                    offsets(String.Format("{0}|{1}", offset.x, offset.y)) += 1
                End If
            End If
        Next

        If (offsets.Count <> 0) Then
            Dim mostFrequentOffset As KeyValuePair(Of String, Integer) = offsets.OrderBy(Function(offset) offset.Value).Last
            If (mostFrequentOffset.Value / _vertexPositionPairs.Where(Function(pair) pair.Value.Count = 2).Count >= _thresholdNofInstancesWithIdenticalOffsetValue) Then
                Me.uckConsiderOffset.Checked = True
                Me.uneOffsetVectorX.Value = mostFrequentOffset.Key.Split("|"c, StringSplitOptions.RemoveEmptyEntries)(0)
                Me.uneOffsetVectorY.Value = mostFrequentOffset.Key.Split("|"c, StringSplitOptions.RemoveEmptyEntries)(1)
            Else
                Me.uneOffsetVectorX.Value = 0
                Me.uneOffsetVectorY.Value = 0
            End If
        End If

        If (offsets.Count = 0) OrElse (CDbl(Me.uneOffsetVectorX.Value) = 0 AndAlso CDbl(Me.uneOffsetVectorY.Value) = 0) Then
            Me.btnOffsetPreview.Visible = False
            Me.lblOffsetVector.Visible = False
            Me.uckConsiderOffset.Visible = False
            Me.uneOffsetVectorX.Visible = False
            Me.uneOffsetVectorY.Visible = False
        Else
            Me.btnOffsetPreview.Visible = True
            Me.lblOffsetVector.Visible = True
            Me.uckConsiderOffset.Enabled = True
            Me.uckConsiderOffset.Visible = True
            Me.uneOffsetVectorX.Visible = True
            Me.uneOffsetVectorY.Visible = True
        End If
    End Sub

    Private Sub CompareDocuments(e As System.ComponentModel.DoWorkEventArgs)
        Dim compareGroupsWithIds As New Dictionary(Of String, List(Of VdSVGGroup))
        Dim compareGroupsWithoutIds As New Dictionary(Of String, List(Of VdSVGGroup))
        Dim entityIndex As Integer = 1
        Dim progressIndex As Integer = 0
        Dim referenceGroupsWithIds As New Dictionary(Of String, List(Of VdSVGGroup))
        Dim referenceGroupsWithoutIds As New Dictionary(Of String, List(Of VdSVGGroup))

        If (_harnessPartNumberMismatch) Then
            _checkedCompareResult = Nothing
        Else
            If (_refDocument.GetCompareResultInfo([Lib].IO.Files.Hcv.KnownContainerFileFlags.GCRI).CheckedCompareResults.ContainsCheckedCompareResult(Me.uceCompareDrw.Text, Me.uceReferenceDrw.Text)) Then
                _checkedCompareResult = _refDocument.GetCompareResultInfo([Lib].IO.Files.Hcv.KnownContainerFileFlags.GCRI).CheckedCompareResults.FindCheckedCompareResult(Me.uceCompareDrw.Text, Me.uceReferenceDrw.Text)
            Else
                _checkedCompareResult = _refDocument.GetCompareResultInfo([Lib].IO.Files.Hcv.KnownContainerFileFlags.GCRI).CheckedCompareResults.AddCheckedCompareResult(Me.uceCompareDrw.Text, Me.uceReferenceDrw.Text)
            End If
        End If

        GetGroupsWithIds(_refLayout.Entities, referenceGroupsWithIds, referenceGroupsWithoutIds, _refDocument.KBL)
        GetGroupsWithIds(_compareLayout.Entities, compareGroupsWithIds, compareGroupsWithoutIds, _compareDocument.KBL)

        With Me.udsResults
            If .Band.Columns.Count = 0 Then
                With .Band
                    .Columns.Add(NameOf(GraphicalCompareFormStrings.Checked_ColumnCaption), GetType(Boolean))
                    .Columns.Add(NameOf(GraphicalCompareFormStrings.ToBeChanged_ColumnCaption), GetType(Boolean))
                    .Columns.Add(NameOf(GraphicalCompareFormStrings.DiffType_ColumnCaption))
                    .Columns.Add(NameOf(GraphicalCompareFormStrings.EntityIndex_ColumnCaption), GetType(Integer))
                    .Columns.Add(NameOf(GraphicalCompareFormStrings.Representation_ColumnCaption))
                    .Columns.Add(NameOf(GraphicalCompareFormStrings.ObjName_ColumnCaption))
                    .Columns.Add(NameOf(GraphicalCompareFormStrings.ObjType_ColumnCaption))
                    .Columns.Add(NameOf(GraphicalCompareFormStrings.Classification_ColumnCaption))
                    .Columns.Add(NameOf(GraphicalCompareFormStrings.Details_ColumnCaption))
                    .Columns.Add(NameOf(GraphicalCompareFormStrings.Comment_ColumnCaption))
                    .Columns.Add(NameOf(GraphicalCompareFormStrings.CompSign_ColumnCaption))
                    .Columns.Add(NameOf(GraphicalCompareFormStrings.RefSign_ColumnCaption))
                End With
            End If

            .Rows.Clear()

            For Each referenceGroupsWithId As KeyValuePair(Of String, List(Of VdSVGGroup)) In referenceGroupsWithIds
                Me.bwCompare.ReportProgress(CInt((progressIndex * 100) / referenceGroupsWithIds.Count))

                If (Me.bwCompare.CancellationPending) Then
                    e.Cancel = True

                    Exit For
                End If

                For Each group As VdSVGGroup In referenceGroupsWithId.Value
                    If (compareGroupsWithIds.ContainsKey(referenceGroupsWithId.Key)) Then
                        FillGridWithMatchingCompareGroups(compareGroupsWithIds, entityIndex, group, referenceGroupsWithId.Key)
                    Else
                        AddDeletedResultRowWithInverse(referenceGroupsWithId.Key, group, entityIndex)
                    End If

                    entityIndex += 1
                Next

                progressIndex += 1
            Next

            For Each referenceGroupsWithoutId As KeyValuePair(Of String, List(Of VdSVGGroup)) In referenceGroupsWithoutIds
                For Each group As VdSVGGroup In referenceGroupsWithoutId.Value
                    If compareGroupsWithoutIds.ContainsKey(referenceGroupsWithoutId.Key) Then
                        FillGridWithMatchingCompareGroups(compareGroupsWithoutIds, entityIndex, group, referenceGroupsWithoutId.Key)
                    Else
                        AddDeletedResultRowWithInverse(referenceGroupsWithoutId.Key, group, entityIndex, bBoxSignature:=False)
                    End If

                    entityIndex += 1
                Next
            Next

            entityIndex = 1
            progressIndex = 0

            For Each compareGroupsWithId As KeyValuePair(Of String, List(Of VdSVGGroup)) In compareGroupsWithIds
                Me.bwCompare.ReportProgress(CInt((progressIndex * 100) / compareGroupsWithIds.Count))

                If (Me.bwCompare.CancellationPending) Then Exit For

                For Each group As VdSVGGroup In compareGroupsWithId.Value
                    If (Not referenceGroupsWithIds.ContainsKey(compareGroupsWithId.Key)) Then
                        AddAddedResultRowWithInverse(compareGroupsWithId.Key, group, entityIndex)
                    End If

                    entityIndex += 1
                Next

                progressIndex += 1
            Next

            For Each compareGroupsWithoutId As KeyValuePair(Of String, List(Of VdSVGGroup)) In compareGroupsWithoutIds
                For Each group As VdSVGGroup In compareGroupsWithoutId.Value
                    If (Not referenceGroupsWithoutIds.ContainsKey(compareGroupsWithoutId.Key)) Then
                        AddAddedResultRowWithInverse(compareGroupsWithoutId.Key, group, entityIndex, bBoxSignature:=False)
                    End If
                    entityIndex += 1
                Next
            Next
        End With
    End Sub

    Private Function GetEntityType(group As VdSVGGroup) As String
        Select Case group.SVGType.Parse(Of SvgType)
            Case SvgType.dimension
                Return GraphicalCompareFormStrings.Dimension_EntityType
            Case SvgType.cell
                Return GraphicalCompareFormStrings.TableCell_EntityType
            Case SvgType.ref
                Return GraphicalCompareFormStrings.Reference_EntityType
            Case SvgType.row
                Return GraphicalCompareFormStrings.TableRow_EntityType
            Case SvgType.table
                Return GraphicalCompareFormStrings.Table_EntityType
            Case Else
                Return GraphicalCompareFormStrings.EntityType_Undefined
        End Select
    End Function

    Private Sub AddAddedResultRowWithInverse(key As String, entity As VdSVGGroup, entityIndex As Integer, Optional bBoxSignature As Boolean = True)
        AddResultRow(key, entity, CompareChangeType.New, entityIndex, Nothing, , bBoxSignature)
    End Sub

    Private Sub AddDeletedResultRowWithInverse(key As String, entity As VdSVGGroup, entityIndex As Integer, Optional bBoxSignature As Boolean = True)
        AddResultRow(key, Nothing, CompareChangeType.Deleted, entityIndex, entity, , bBoxSignature)
    End Sub

    Private Sub AddModifiedResultRow(key As String, entity As VdSVGGroup, entityIndex As Integer, changeClass As String, refEntity As VdSVGGroup, hashStringDiff As Integer, Optional bBoxSignature As Boolean = True)
        AddResultRow(key, entity, CompareChangeType.Modified, entityIndex, refEntity, hashStringDiff, bBoxSignature, changeClass)
    End Sub

    Private Sub AddResultRow(key As String, entity As VdSVGGroup, changeType As CompareChangeType, entityIndex As Integer, refEntity As VdSVGGroup, Optional hashStringDiff As Integer = 0, Optional bBoxSignature As Boolean = True, Optional changeClass As String = "")
        Dim signatures As New CompareSignatures(key, entity, refEntity, bBoxSignature, chkConsiderText.Checked, _moveDistanceTolerance)
        With signatures
            .Update(changeType)

            Dim row As UltraDataRow = Me.udsResults.Rows.Add

            row.SetCellValue(NameOf(GraphicalCompareFormStrings.DiffType_ColumnCaption), signatures.GetLocalizedDiffType)
            row.SetCellValue(NameOf(GraphicalCompareFormStrings.EntityIndex_ColumnCaption), entityIndex)
            row.SetCellValue(NameOf(GraphicalCompareFormStrings.Representation_ColumnCaption), GetEntityType(If(entity, refEntity)))

            If (refEntity IsNot Nothing) Then
                GetObjectNameAndType(refEntity, row, _refDocument.KBL)
            Else
                GetObjectNameAndType(entity, row, _compareDocument.KBL)
            End If

            row.SetCellValue(NameOf(GraphicalCompareFormStrings.Classification_ColumnCaption), changeClass)

            If (changeClass.Contains(GraphicalCompareFormStrings.Move_Class)) Then
                row.SetCellValue(NameOf(GraphicalCompareFormStrings.Details_ColumnCaption), String.Format(GraphicalCompareFormStrings.DeltaXY_CellValue, CInt(refEntity.BoundingBox.Min.x - entity.BoundingBox.Min.x), CInt(refEntity.BoundingBox.Min.y - entity.BoundingBox.Min.y)))
            ElseIf (hashStringDiff <> 0) Then
                row.SetCellValue(NameOf(GraphicalCompareFormStrings.Details_ColumnCaption), String.Format(GraphicalCompareFormStrings.ApprDiffMetric_CellValue, hashStringDiff))
            ElseIf (changeClass.Contains(GraphicalCompareFormStrings.Appearance_Class)) Then
                row.SetCellValue(NameOf(GraphicalCompareFormStrings.Details_ColumnCaption), GraphicalCompareFormStrings.Transformation_CellValue)
            End If

            row.SetCellValue(NameOf(GraphicalCompareFormStrings.Checked_ColumnCaption), False)
            row.SetCellValue(NameOf(GraphicalCompareFormStrings.ToBeChanged_ColumnCaption), False)

            If (_checkedCompareResult IsNot Nothing) Then
                Dim checkedCompareResultEntry As CheckedCompareResultEntry = _checkedCompareResult.CheckedCompareResultEntries.FindCheckedCompareResultEntry(signatures.EntitySignature, signatures.ReferenceSignature)
                If (checkedCompareResultEntry IsNot Nothing) Then
                    row.SetCellValue(NameOf(GraphicalCompareFormStrings.Checked_ColumnCaption), True)
                    row.SetCellValue(NameOf(GraphicalCompareFormStrings.ToBeChanged_ColumnCaption), checkedCompareResultEntry.ToBeChanged)
                    row.SetCellValue(NameOf(GraphicalCompareFormStrings.Comment_ColumnCaption), checkedCompareResultEntry.Comment)
                End If
            End If

            row.SetCellValue(NameOf(GraphicalCompareFormStrings.CompSign_ColumnCaption), .EntitySignature)
            row.SetCellValue(NameOf(GraphicalCompareFormStrings.RefSign_ColumnCaption), .ReferenceSignature)

            If (entity IsNot Nothing) Then
                If (CBool(row.GetCellValue(NameOf(GraphicalCompareFormStrings.Checked_ColumnCaption)))) Then
                    entity.GraphicalCompareCheckState = GraphicalCompareCheckState.Checked
                Else
                    entity.GraphicalCompareCheckState = GraphicalCompareCheckState.Unchecked
                End If

                If (Not entity.BoundingBox.IsEmpty) Then
                    _compareLayout.Entities.AddItem(DrawChangeIndicator(_compareLayout.Document, entity))
                End If
            End If

            If (refEntity IsNot Nothing) Then
                If (CBool(row.GetCellValue(NameOf(GraphicalCompareFormStrings.Checked_ColumnCaption)))) Then
                    refEntity.GraphicalCompareCheckState = GraphicalCompareCheckState.Checked
                Else
                    refEntity.GraphicalCompareCheckState = GraphicalCompareCheckState.Unchecked
                End If

                If (Not refEntity.BoundingBox.IsEmpty) Then
                    _refLayout.Entities.AddItem(DrawChangeIndicator(_refLayout.Document, refEntity))
                End If
            End If

            row.Tag = New Tuple(Of VdSVGGroup, VdSVGGroup)(refEntity, entity)
        End With
    End Sub

    Private Sub CreatePictureOfSelectedObject(document As vdObjects.vdDocument, boundingBox As Box, isRef As Boolean, renderImage As Boolean)
        Dim compHighlightRects As List(Of VdRectEx) = _compareDrawing.HighlightRects
        Dim refHighlightRects As List(Of VdRectEx) = _refDrawing.HighlightRects

        _compareDrawing.HighlightRects = Nothing
        _refDrawing.HighlightRects = Nothing

        document.ActiveLayOut.Label = "Printing"

        Dim layout As vdPrimaries.vdLayout

        If (isRef) Then
            layout = _refLayout
        Else
            layout = _compareLayout
        End If

        With layout
            If (Not E3.Lib.DotNet.Expansions.Devices.My.Computer.Keyboard.CtrlKeyDown) Then
                If (Not renderImage) Then
                    If (boundingBox IsNot Nothing) Then
                        Dim viewSize As Double = Math.Max(boundingBox.Height + (boundingBox.Height / 3), boundingBox.Width + (boundingBox.Width / 3))
                        viewSize = Math.Max(viewSize, 100)

                        If (isRef) Then
                            _viewCenterRef = boundingBox.MidPoint
                            _viewSizeRef = viewSize
                        Else
                            _viewCenterCompare = boundingBox.MidPoint
                            _viewSizeCompare = viewSize
                        End If
                    End If
                Else
                    If (isRef) Then
                        If (_viewCenterRef IsNot Nothing) Then .ViewCenter = _viewCenterRef

                        .ViewSize = _viewSizeRef
                    Else
                        If (_viewCenterCompare IsNot Nothing) Then .ViewCenter = _viewCenterCompare

                        .ViewSize = _viewSizeCompare
                    End If
                End If
            End If
        End With

        If (renderImage) Then
            Dim bmp As Bitmap = Nothing

            If (isRef) Then
                If (Me.picRefResult.Width <> 0) AndAlso (Me.picRefResult.Height <> 0) Then bmp = New Bitmap(Me.picRefResult.Width, Me.picRefResult.Height)
            Else
                If (Me.picCompResult.Width <> 0) AndAlso (Me.picCompResult.Height <> 0) Then bmp = New Bitmap(Me.picCompResult.Width, Me.picCompResult.Height)
            End If

            If (bmp IsNot Nothing) Then
                Dim graphics As Graphics = Graphics.FromImage(bmp)
                layout.RenderToGraphics(graphics, Nothing, bmp.Width, bmp.Height)
                graphics.Dispose()

                document.ActiveLayOut.Label = String.Empty

                If (isRef) Then
                    Me.picRefResult.Image = bmp
                    Me.picRefResult.Update()
                Else
                    Me.picCompResult.Image = bmp
                    Me.picCompResult.Update()
                End If
            End If
        End If

        _compareDrawing.HighlightRects = compHighlightRects
        _refDrawing.HighlightRects = refHighlightRects
    End Sub

    Private Sub DeselectAll()
        With Me.ugResults
            .BeginUpdate()
            .EventManager.AllEventsEnabled = False
            .ActiveRow = Nothing
            .Selected.Rows.Clear()
            .EventManager.AllEventsEnabled = True
            .EndUpdate()
        End With

        ShowGraphics(True)
    End Sub

    Private Function DrawChangeIndicator(document As vdObjects.vdDocument, group As VdSVGGroup) As VdSVGGroup
        If (document.Layers.FindName(LAYER_CHANGE_INDICATORS) Is Nothing) Then
            Dim layer As New vdLayer
            With layer
                .SetUnRegisterDocument(document)
                .setDocumentDefaults()

                .Name = LAYER_CHANGE_INDICATORS
            End With

            document.Layers.Add(layer)
        End If

        Dim changeIndicatorGroup As New VdSVGGroup
        With changeIndicatorGroup
            .SetUnRegisterDocument(document)
            .setDocumentDefaults()

            .Label = group.Id.ToString
            .Layer = document.Layers.FindName(LAYER_CHANGE_INDICATORS)
            .SVGType = SvgType.Undefined.ToString
            .SymbolType = SvgType.Undefined.ToString
        End With

        Dim basePoint As gPoint = group.BoundingBox.Max

        If (group.SymbolType = SvgSymbolType.Segment.ToString) Then
            basePoint = group.BoundingBox.MidPoint
        End If

        Dim vdLine As New VdLineEx(document)
        Dim style As VdStyle = New VdStyle

        With style
            .LineType = document.LineTypes.DPIDot

            If (group.GraphicalCompareCheckState = GraphicalCompareCheckState.Checked) Then
                .PenColor = SvgColor.Green
            ElseIf (group.GraphicalCompareCheckState = GraphicalCompareCheckState.Unchecked) Then
                .PenColor = SvgColor.Orange
            End If

        End With


        With vdLine
            .LineType = style.LineType
            .PenColor.SystemColor = style.PenColor.SystemColor
            .PenColor.AlphaBlending = style.PenColor.AlphaBlending
            .StartPoint.x = 0.25
            .StartPoint.y = 0.25
            .EndPoint.x = 2.25
            .EndPoint.y = 2.25
        End With

        changeIndicatorGroup.AddFigure(vdLine)

        Dim vdPolyline As New VdPolylineEx(document)
        style = New VdStyle
        With style
            If (group.GraphicalCompareCheckState = GraphicalCompareCheckState.Checked) Then
                .FillColor = SvgColor.Green
            ElseIf (group.GraphicalCompareCheckState = GraphicalCompareCheckState.Unchecked) Then
                .FillColor = SvgColor.Orange
            End If
            .FillMode = VectorDraw.Professional.Constants.VdConstFill.VdFillModeSolid
        End With

        With vdPolyline

            Dim fillColor As vdColor = Nothing
            If (group.GraphicalCompareCheckState = GraphicalCompareCheckState.Checked) Then
                fillColor = SvgColor.Green
            ElseIf (group.GraphicalCompareCheckState = GraphicalCompareCheckState.Unchecked) Then
                fillColor = SvgColor.Orange
            End If
            .HatchProperties = _factory.GetHatchproperties(document, fillColor)
            .PenWidth = 0.25

            .VertexList.Add(New gPoint(2.5, 2.5))
            .VertexList.Add(.VertexList.Last + New gPoint(2.5, 0))
            .VertexList.Add(.VertexList.Last + New gPoint(-1.25, 2.5))
            .VertexList.Add(.VertexList.Last + New gPoint(-1.25, -2.5))

        End With

        changeIndicatorGroup.AddFigure(vdPolyline)

        Dim vdText As New VdTextEx(document)
        With vdText
            .InsertionPoint.x = 3.75
            .InsertionPoint.y = 3.5
            .HorJustify = Constants.VdConstHorJust.VdTextHorCenter
            .Bold = True
            .IsRedlining = True
            .PenColor = SvgColor.Black
            .TextString = "!"
        End With

        changeIndicatorGroup.AddFigure(vdText)

        Dim matrix As New Matrix
        matrix.TranslateMatrix(basePoint)

        changeIndicatorGroup.Transformby(matrix)

        matrix.Dispose()

        Return changeIndicatorGroup
    End Function

    Private Sub FilterGroupsInSelectedArea(boundingBox As Box, groups As vdCollections.vdEntities, layout As vdLayout, resetSelection As Boolean)
        For Each group As VdSVGGroup In layout.Entities
            If (group.SymbolType <> SvgSymbolType.DocumentFrame.ToString) AndAlso (group.SymbolType <> SvgSymbolType.Redlining.ToString) AndAlso (group.visibility = vdPrimaries.vdFigure.VisibilityEnum.Visible) AndAlso (group.Lighting <> Lighting.Lowlight) Then
                If (resetSelection AndAlso boundingBox.PointInBox(group.BoundingBox.MidPoint)) OrElse (Not resetSelection AndAlso (boundingBox.PointInBox(group.BoundingBox.Min) OrElse boundingBox.PointInBox(group.BoundingBox.Max) OrElse boundingBox.PointInBox(group.BoundingBox.UpperLeft) OrElse boundingBox.PointInBox(group.BoundingBox.LowerRight) OrElse boundingBox.PointInBox(group.BoundingBox.MidPoint))) Then
                    groups.Add(group)
                Else
                    group.visibility = vdPrimaries.vdFigure.VisibilityEnum.Invisible
                    group.Invalidate()

                    _hiddenGroups.Add(group)
                End If
            End If
        Next
    End Sub

    Private Sub FilterResultGridRowsDynamically()
        If (Me.uckFilterDynamically.Checked) Then
            Dim boundingBox As Box = _refLayout.ActiveRender.ClipBounds.ToBox

            Me.ugResults.BeginUpdate()

            For Each row As UltraGridRow In Me.ugResults.Rows
                If (Not row.IsFilteredOut) Then
                    Dim group As VdSVGGroup = If(DirectCast(DirectCast(row.ListObject, UltraDataRow).Tag, Tuple(Of VdSVGGroup, VdSVGGroup)).Item1, DirectCast(DirectCast(row.ListObject, UltraDataRow).Tag, Tuple(Of VdSVGGroup, VdSVGGroup)).Item2)
                    If (group IsNot Nothing) Then
                        row.Hidden = group.visibility = vdFigure.VisibilityEnum.Invisible OrElse Not (boundingBox.PointInBox(group.BoundingBox.Min) OrElse boundingBox.PointInBox(group.BoundingBox.Max) OrElse boundingBox.PointInBox(group.BoundingBox.UpperLeft) OrElse boundingBox.PointInBox(group.BoundingBox.LowerRight) OrElse boundingBox.PointInBox(group.BoundingBox.MidPoint))
                    End If
                End If
            Next

            Me.ugResults.EndUpdate()
        End If
    End Sub

    Private Sub FindConnectorTableModifications_Recursively(refTableCompareEntry As SvgTableCompareEntry, compTableCompareEntry As SvgTableCompareEntry)
        If (refTableCompareEntry.Childs.Count >= compTableCompareEntry.Childs.Count) Then
            For Each refChildTableCompareEntry As SvgTableCompareEntry In refTableCompareEntry.Childs
                If (compTableCompareEntry.Childs.Any(Function(entry) entry.GetMatchString = refChildTableCompareEntry.GetMatchString)) Then
                    Dim compChildTableCompareEntry As SvgTableCompareEntry = compTableCompareEntry.Childs.Where(Function(entry) entry.GetMatchString = refChildTableCompareEntry.GetMatchString AndAlso Not entry.HasMatchEntry).FirstOrDefault
                    If (compChildTableCompareEntry IsNot Nothing) Then
                        refChildTableCompareEntry.HasMatchEntry = True
                        compChildTableCompareEntry.HasMatchEntry = True

                        FindConnectorTableModifications_Recursively(refChildTableCompareEntry, compChildTableCompareEntry)
                    End If
                End If
            Next
        Else
            For Each compChildTableCompareEntry As SvgTableCompareEntry In compTableCompareEntry.Childs
                If (refTableCompareEntry.Childs.Any(Function(entry) entry.GetMatchString = compChildTableCompareEntry.GetMatchString)) Then
                    Dim refChildTableCompareEntry As SvgTableCompareEntry = refTableCompareEntry.Childs.Where(Function(entry) entry.GetMatchString = compChildTableCompareEntry.GetMatchString AndAlso Not entry.HasMatchEntry).FirstOrDefault
                    If (refChildTableCompareEntry IsNot Nothing) Then
                        compChildTableCompareEntry.HasMatchEntry = True
                        refChildTableCompareEntry.HasMatchEntry = True

                        FindConnectorTableModifications_Recursively(refChildTableCompareEntry, compChildTableCompareEntry)
                    End If
                End If
            Next
        End If
    End Sub

    Private Sub GetConnectorTableTexts_Recursively(depth As Integer, group As VdSVGGroup, ByRef parentTableCompareEntry As SvgTableCompareEntry)
        For Each childGroup As VdSVGGroup In group.ChildGroups
            Dim subTableCompareEntry As SvgTableCompareEntry = Nothing
            Dim tableCompareEntry As SvgTableCompareEntry = Nothing

            If (childGroup.Figures.Count <> 0) AndAlso (childGroup.Figures.Any(Function(element) TypeOf element Is VdTextEx)) Then
                tableCompareEntry = New SvgTableCompareEntry(childGroup, depth, parentTableCompareEntry)

                For Each figure As Object In childGroup.Figures
                    If (TypeOf figure Is VdTextEx) AndAlso (Not DirectCast(figure, VdTextEx).IsMarker) Then tableCompareEntry.Texts.Add(DirectCast(figure, VdTextEx))
                Next

                If (tableCompareEntry.Texts.Count = 1 OrElse childGroup.SVGType = SvgType.cell.ToString OrElse childGroup.SVGType = SvgType.row.ToString) Then
                    parentTableCompareEntry.Childs.Add(tableCompareEntry)
                Else
                    Dim prevX As Double = Double.MinValue
                    Dim textCounter As Integer = 1

                    Dim texts As New Dictionary(Of Integer, List(Of VdTextEx))
                    texts.Add(textCounter, New List(Of VdTextEx))

                    For Each txt As VdTextEx In tableCompareEntry.Texts
                        If (Math.Round(txt.InsertionPoint.x, 3) <= Math.Round(prevX, 3)) Then
                            textCounter += 1

                            texts.Add(textCounter, New List(Of VdTextEx))
                        End If

                        prevX = Math.Round(txt.InsertionPoint.x, 3)
                        texts(textCounter).Add(txt)
                    Next

                    For Each textBlock As List(Of VdTextEx) In texts.Values
                        subTableCompareEntry = New SvgTableCompareEntry(childGroup, depth, parentTableCompareEntry)
                        subTableCompareEntry.Texts.AddRange(textBlock)

                        parentTableCompareEntry.Childs.Add(subTableCompareEntry)

                        GetConnectorTableTexts_Recursively(depth + 1, childGroup, subTableCompareEntry)
                    Next
                End If
            End If

            If (subTableCompareEntry Is Nothing) Then
                GetConnectorTableTexts_Recursively(If(tableCompareEntry IsNot Nothing, depth + 1, depth), childGroup, If(tableCompareEntry, parentTableCompareEntry))
            End If
        Next
    End Sub

    Private Sub GetGroupsWithIds(entities As vdCollections.vdEntities, groupsWithIds As Dictionary(Of String, List(Of VdSVGGroup)), groupsWithoutIds As Dictionary(Of String, List(Of VdSVGGroup)), kblMapper As KblMapper)
        Dim TakeUserId As Boolean = True
        For Each group As VdSVGGroup In entities
            If (group.SymbolType <> SvgSymbolType.DocumentFrame.ToString) AndAlso (group.SymbolType <> SvgSymbolType.Redlining.ToString) AndAlso (group.visibility = vdPrimaries.vdFigure.VisibilityEnum.Visible) AndAlso (group.Lighting <> Lighting.Lowlight) Then
                If (group.KblId = String.Empty OrElse Not kblMapper.KBLOccurrenceMapper.ContainsKey(group.KblId)) Then

                    Dim key As String = String.Format("{0}|{1}", CInt(group.BoundingBox.Min.x / _moveDistanceTolerance), CInt(group.BoundingBox.Min.y / _moveDistanceTolerance))
                    If (Not groupsWithoutIds.ContainsKey(key)) Then
                        groupsWithoutIds.Add(key, New List(Of VdSVGGroup))
                    End If
                    groupsWithoutIds(key).Add(group)
                Else

                    Dim secondaryKblIds As String = String.Empty
                    For Each secondaryKblId As String In group.SecondaryKblIds
                        If TakeUserId Then
                            secondaryKblIds = String.Format("{0} {1}", secondaryKblIds, GetUserIdFromKblId(secondaryKblId, kblMapper))
                        Else
                            secondaryKblIds = String.Format("{0} {1}", secondaryKblIds, secondaryKblId)
                        End If
                    Next

                    secondaryKblIds = Microsoft.VisualBasic.Trim(secondaryKblIds)
                    Dim kblId As String
                    If TakeUserId Then
                        kblId = GetUserIdFromKblId(group.KblId, kblMapper)
                    Else
                        kblId = group.KblId
                    End If

                    If Not String.IsNullOrEmpty(kblId) Then
                        Dim key As String = String.Format("{0}|{1}|{2}|{3}", kblId, secondaryKblIds, group.SVGType, group.SymbolType)
                        If (Not groupsWithIds.ContainsKey(key)) Then
                            groupsWithIds.Add(key, New List(Of VdSVGGroup))
                        End If
                        groupsWithIds(key).Add(group)
                    End If

                End If
            End If
        Next
    End Sub

    Private Function GetUserIdFromKblId(kblId As String, kblMapper As KblMapper) As String
        If (kblMapper.KBLOccurrenceMapper.ContainsKey(kblId)) Then

            Dim kbl_occ As IKblOccurrence = kblMapper.GetOccurrenceObjectUntyped(kblId)
            If kbl_occ IsNot Nothing Then

                Select Case kbl_occ.ObjectType
                    Case KblObjectType.Accessory_occurrence
                        Return DirectCast(kblMapper.KBLOccurrenceMapper(kblId), Accessory_occurrence).Id
                    Case KblObjectType.Component_occurrence, KblObjectType.Fuse_occurrence
                        Return DirectCast(kblMapper.KBLOccurrenceMapper(kblId), Component_occurrence).Id
                    Case KblObjectType.Connector_occurrence
                        Return DirectCast(kblMapper.KBLOccurrenceMapper(kblId), Connector_occurrence).Id
                    Case KblObjectType.Fixing_occurrence
                        Return DirectCast(kblMapper.KBLOccurrenceMapper(kblId), Fixing_occurrence).Id
                    Case KblObjectType.Node
                        Return DirectCast(kblMapper.KBLOccurrenceMapper(kblId), Node).Id
                    Case KblObjectType.Segment
                        Return DirectCast(kblMapper.KBLOccurrenceMapper(kblId), Segment).Id
                    Case KblObjectType.Wire_protection_occurrence
                        Return DirectCast(kblMapper.KBLOccurrenceMapper(kblId), Wire_protection_occurrence).Id
                    Case KblObjectType.Dimension_specification
                        Return DirectCast(kblMapper.KBLOccurrenceMapper(kblId), Dimension_specification).Id
                End Select
            End If
        End If
        Return String.Empty
    End Function

    Private Function GetHighlightRect(document As vdObjects.vdDocument, height As Double, id As Integer, insertionPoint As gPoint, penColor As Color, penWidth As Double, width As Double) As VdRectEx
        Const MARKER_LAYER As String = "HighLightRects"

        Dim markerLayer As vdLayer = document.Layers.FindName(MARKER_LAYER)
        If (markerLayer Is Nothing) Then
            markerLayer = New vdLayer
            With markerLayer
                .SetUnRegisterDocument(document)
                .setDocumentDefaults()
                .Lock = True
                .Name = MARKER_LAYER
            End With
            document.Layers.Add(markerLayer)
        End If

        Dim rect As New VdRectEx(document)
        With rect
            .setDocumentDefaults()
            .HatchProperties = _factory.GetHatchproperties(document, New vdColor(penColor, 50))
            .Height = height
            .InsertionPoint = insertionPoint
            .Label = id.ToString
            .PenColor.SystemColor = penColor
            .PenWidth = penWidth
            .Width = width
            .Layer = markerLayer
        End With

        Return rect
    End Function

    Private Sub FillGridWithMatchingCompareGroups(compareGroupsWithOrWithoutIds As Dictionary(Of String, List(Of VdSVGGroup)), entityIndex As Integer, referenceGroup As VdSVGGroup, referenceGroupKey As String)
        Dim compareGroups As New List(Of VdSVGGroup)

        For Each compareGroup As VdSVGGroup In compareGroupsWithOrWithoutIds(referenceGroupKey)
            If (compareGroup.IsEqual(referenceGroup)) Then
                Exit Sub
            Else
                compareGroups.Add(compareGroup)
            End If
        Next

        If (compareGroups.Count <> 0) Then
            Dim compareGroup As VdSVGGroup = Nothing
            Dim distance As Integer = Integer.MaxValue
            Dim minHashStringDiff As Integer = Integer.MaxValue
            Dim referenceCoordinates As IEnumerable(Of Tuple(Of String, gPoint)) = referenceGroup.GetCoordinates

            For Each compGroup As VdSVGGroup In compareGroups
                If (Math.Abs(compGroup.GetHashString(False, Me.chkConsiderText.Checked).Length - referenceGroup.GetHashString(False, Me.chkConsiderText.Checked).Length) < minHashStringDiff) Then
                    compareGroup = compGroup
                    distance = CInt(compareGroup.BoundingBox.Min.Distance2D(referenceGroup.BoundingBox.Min))
                    minHashStringDiff = Math.Abs(compGroup.GetHashString(False, Me.chkConsiderText.Checked).Length - referenceGroup.GetHashString(False, Me.chkConsiderText.Checked).Length)

                    If (minHashStringDiff = 0) AndAlso (referenceGroup.GetHashString(False, Me.chkConsiderText.Checked) = compareGroup.GetHashString(False, Me.chkConsiderText.Checked)) Then
                        Dim compareCoordinates As IEnumerable(Of Tuple(Of String, gPoint)) = compareGroup.GetCoordinates

                        If (referenceCoordinates.Count = compareCoordinates.Count) Then
                            Dim coordinatesAreEqual As Boolean = True

                            For coordinateCounter As Integer = 0 To referenceCoordinates.Count - 1
                                Dim referenceCoordinate As Tuple(Of String, gPoint) = referenceCoordinates(coordinateCounter)
                                Dim compareCoordinate As Tuple(Of String, gPoint) = compareCoordinates(coordinateCounter)

                                If (referenceCoordinate.Item1 <> compareCoordinate.Item1) OrElse (CInt(referenceCoordinate.Item2.Distance2D(compareCoordinate.Item2)) >= _moveDistanceTolerance) Then
                                    coordinatesAreEqual = False

                                    If (distance < _moveDistanceTolerance) Then
                                        distance = CInt(referenceCoordinate.Item2.Distance2D(compareCoordinate.Item2))
                                    End If

                                    Exit For
                                End If
                            Next

                            If (coordinatesAreEqual) OrElse ((Me.uckConsiderOffset.Checked) AndAlso (Math.Round(referenceGroup.BoundingBox.Min.x - compareGroup.BoundingBox.Min.x, 2) = CDbl(Me.uneOffsetVectorX.Value)) AndAlso (Math.Round(referenceGroup.BoundingBox.Min.y - compareGroup.BoundingBox.Min.y, 2) = CDbl(Me.uneOffsetVectorY.Value))) Then
                                compareGroup = Nothing

                                Exit For
                            End If
                        End If
                    End If
                End If
            Next

            If (compareGroup IsNot Nothing) Then
                If (distance < _moveDistanceTolerance) AndAlso (compareGroup.GetHashString(True, Me.chkConsiderText.Checked) <> referenceGroup.GetHashString(True, Me.chkConsiderText.Checked)) Then
                    AddModifiedResultRow(referenceGroupKey, compareGroup, entityIndex, GraphicalCompareFormStrings.Appearance_Class, referenceGroup, minHashStringDiff)
                ElseIf (distance >= _moveDistanceTolerance) AndAlso (compareGroup.GetHashString(True, Me.chkConsiderText.Checked) = referenceGroup.GetHashString(True, Me.chkConsiderText.Checked)) Then
                    AddModifiedResultRow(referenceGroupKey, compareGroup, entityIndex, GraphicalCompareFormStrings.Move_Class, referenceGroup, minHashStringDiff)
                ElseIf (distance >= _moveDistanceTolerance) AndAlso (compareGroup.GetHashString(True, Me.chkConsiderText.Checked) <> referenceGroup.GetHashString(True, Me.chkConsiderText.Checked)) Then
                    AddModifiedResultRow(referenceGroupKey, compareGroup, entityIndex, GraphicalCompareFormStrings.AppearanceMove_Class, referenceGroup, minHashStringDiff)
                End If
            End If
        End If
    End Sub

    Private Sub GetObjectNameAndType(group As VdSVGGroup, row As UltraDataRow, kblMapper As KblMapper)
        Dim kblId As String = group.KblId
        Dim kbl_occ As IKblOccurrence = kblMapper.GetOccurrenceObjectUntyped(kblId)
        If kbl_occ IsNot Nothing Then
            Select Case kbl_occ.ObjectType
                Case KblObjectType.Accessory_occurrence
                    row.SetCellValue(NameOf(GraphicalCompareFormStrings.ObjName_ColumnCaption), kbl_occ.Id)
                    row.SetCellValue(NameOf(GraphicalCompareFormStrings.ObjType_ColumnCaption), KblObjectType.Accessory_occurrence.ToLocalizedString)
                Case KblObjectType.Component_occurrence, KblObjectType.Fuse_occurrence
                    row.SetCellValue(NameOf(GraphicalCompareFormStrings.ObjName_ColumnCaption), kbl_occ.Id)
                    row.SetCellValue(NameOf(GraphicalCompareFormStrings.ObjType_ColumnCaption), KblObjectType.Component_occurrence.ToLocalizedString)
                Case KblObjectType.Connector_occurrence
                    row.SetCellValue(NameOf(GraphicalCompareFormStrings.ObjName_ColumnCaption), kbl_occ.Id)
                    row.SetCellValue(NameOf(GraphicalCompareFormStrings.ObjType_ColumnCaption), KblObjectType.Connector_occurrence.ToLocalizedString)
                Case KblObjectType.Fixing_occurrence
                    row.SetCellValue(NameOf(GraphicalCompareFormStrings.ObjName_ColumnCaption), kbl_occ.Id)
                    row.SetCellValue(NameOf(GraphicalCompareFormStrings.ObjType_ColumnCaption), KblObjectType.Fixing_occurrence.ToLocalizedString)
                Case KblObjectType.Node
                    row.SetCellValue(NameOf(GraphicalCompareFormStrings.ObjName_ColumnCaption), kbl_occ.Id)
                    row.SetCellValue(NameOf(GraphicalCompareFormStrings.ObjType_ColumnCaption), KblObjectType.Node.ToLocalizedString)
                Case KblObjectType.Segment
                    row.SetCellValue(NameOf(GraphicalCompareFormStrings.ObjName_ColumnCaption), kbl_occ.Id)
                    row.SetCellValue(NameOf(GraphicalCompareFormStrings.ObjType_ColumnCaption), KblObjectType.Segment.ToLocalizedString)
                Case KblObjectType.Wire_protection_occurrence
                    row.SetCellValue(NameOf(GraphicalCompareFormStrings.ObjName_ColumnCaption), kbl_occ.Id)
                    row.SetCellValue(NameOf(GraphicalCompareFormStrings.ObjType_ColumnCaption), KblObjectType.Wire_protection.ToLocalizedString)
            End Select
        Else
            row.SetCellValue(NameOf(GraphicalCompareFormStrings.ObjName_ColumnCaption), KblObjectType.Undefined.ToLocalizedString)

            If (group.SVGType = SvgType.table.ToString) AndAlso (group.SymbolType = SvgSymbolType.ModuleTable.ToString) Then
                row.SetCellValue(NameOf(GraphicalCompareFormStrings.ObjType_ColumnCaption), KblObjectType.Module_Table.ToLocalizedString)
            Else
                row.SetCellValue(NameOf(GraphicalCompareFormStrings.ObjType_ColumnCaption), KblObjectType.Not_available.ToLocalizedString)
            End If
        End If
    End Sub

    Private Function GetOpenDrawingsFromDocument(document As DocumentForm) As Dictionary(Of String, DrawingCanvas)
        Dim drawings As New Dictionary(Of String, DrawingCanvas)

        For Each drawingTab As Infragistics.Win.UltraWinTabControl.UltraTab In document.utcDocument.Tabs
            If (drawingTab.Visible AndAlso TypeOf (drawingTab.TabPage.Controls(0)) Is DrawingCanvas) Then
                drawings.Add(drawingTab.Key, DirectCast(drawingTab.TabPage.Controls(0), DrawingCanvas))
            End If
        Next

        Return drawings
    End Function

    Private Sub Initialize(compareDocuments As IEnumerable(Of IDocumentForm))
        Me.BackColor = Color.White
        Me.Icon = My.Resources.CompareGraphic
        Me.Text = GraphicalCompareFormStrings.Caption

        Me.btnCompare.Enabled = False
        Me.btnExport.Enabled = False
        Me.btnSave.Enabled = False
        Me.btnSync.Appearance.Image = My.Resources.Sync
        Me.btnResetAreaFilter.Visible = False

        Me.lblLegend1.Visible = False
        Me.lblLegend2.Visible = False
        Me.lblLegend3.Visible = False
        Me.lblRowCountInfo.Visible = False

        Me.txtComment.ButtonsRight("btnEdit").Appearance.Image = My.Resources.Visibility.ToBitmap
        Me.txtReferenceDocument.Text = _refDocument.Text

        Me.uckDisplayIndicators.Checked = True
        Me.uckDisplayIndicators.Visible = False
        Me.uckFilterDynamically.Visible = False
        Me.uddbSetAreaFilter.Visible = False
        Me.ugResults.SyncWithCurrencyManager = False

        Dim refDrawings As Dictionary(Of String, DrawingCanvas) = GetOpenDrawingsFromDocument(_refDocument)

        For Each refDrawing As KeyValuePair(Of String, DrawingCanvas) In refDrawings
            Me.uceReferenceDrw.Items.Add(refDrawing.Key, IO.Path.GetFileNameWithoutExtension(refDrawing.Key)).Tag = refDrawing.Value
        Next

        Me.uceReferenceDrw.SortStyle = Infragistics.Win.ValueListSortStyle.Ascending
        If (Me.uceReferenceDrw.Items.Count > 0) Then
            Me.uceReferenceDrw.SelectedItem = Me.uceReferenceDrw.Items(0)
        End If

        For Each compareDocument As IDocumentForm In compareDocuments
            Me.uceCompareDocument.Items.Add(compareDocument, compareDocument.TextResolved)
        Next

        Me.uceCompareDocument.SortStyle = Infragistics.Win.ValueListSortStyle.Ascending

        If (Me.uceCompareDocument.Items.Count <> 0) Then
            Me.uceCompareDocument.SelectedItem = Me.uceCompareDocument.Items(0)
        End If

        Me.upbCompare.Visible = False
        Me.upnResult.Visible = False

        _contextMenuButton = New PopupMenuTool("ContextMenuButton")
        With _contextMenuButton
            .DropDownArrowStyle = DropDownArrowStyle.None

            Dim fromCurrentViewButton As New ButtonTool("FromCurrentView")
            fromCurrentViewButton.SharedProps.Caption = GraphicalCompareFormStrings.FromCurrentView_ButtonCaption

            Dim fromRefDrawingButton As New ButtonTool("FromRefDrawing")
            fromRefDrawingButton.SharedProps.Caption = GraphicalCompareFormStrings.FromRefDrawingTab_ButtonCaption

            Me.utmCompare.Tools.AddRange(New ToolBase() {_contextMenuButton, fromCurrentViewButton, fromRefDrawingButton})

            .Tools.AddTool(fromCurrentViewButton.Key)
            .Tools.AddTool(fromRefDrawingButton.Key)
        End With

        Me.uddbSetAreaFilter.PopupItem = _contextMenuButton

        _contextMenuGrid = New PopupMenuTool("ContextMenuGrid")

        With _contextMenuGrid
            .DropDownArrowStyle = DropDownArrowStyle.None

            Dim checkButton As New ButtonTool("Check")
            checkButton.SharedProps.Caption = GraphicalCompareFormStrings.CheckSelResult_ButtonCaption
            checkButton.SharedProps.AppearancesSmall.Appearance.Image = My.Resources.Check.ToBitmap

            Dim checkAllButton As New ButtonTool("CheckAll")
            checkAllButton.SharedProps.Caption = GraphicalCompareFormStrings.CheckAllResults_ButtonCaption
            checkAllButton.SharedProps.AppearancesSmall.Appearance.Image = My.Resources.CheckAll.ToBitmap

            Dim uncheckButton As New ButtonTool("Uncheck")
            uncheckButton.SharedProps.Caption = GraphicalCompareFormStrings.UncheckSelResult_ButtonCaption
            uncheckButton.SharedProps.AppearancesSmall.Appearance.Image = My.Resources.Uncheck.ToBitmap

            Dim uncheckAllButton As New ButtonTool("UncheckAll")
            uncheckAllButton.SharedProps.Caption = GraphicalCompareFormStrings.UncheckAllResults_ButtonCaption
            uncheckAllButton.SharedProps.AppearancesSmall.Appearance.Image = My.Resources.UncheckAll.ToBitmap

            Dim undoLastAction As New ButtonTool("Undo")
            undoLastAction.SharedProps.Caption = GraphicalCompareFormStrings.Undo_ButtonCaption
            undoLastAction.SharedProps.AppearancesSmall.Appearance.Image = My.Resources.Undo.ToBitmap

            Dim redoPreviousAction As New ButtonTool("Redo")
            redoPreviousAction.SharedProps.Caption = GraphicalCompareFormStrings.Redo_ButtonCaption
            redoPreviousAction.SharedProps.AppearancesSmall.Appearance.Image = My.Resources.Redo.ToBitmap

            Me.utmCompare.Tools.AddRange(New ToolBase() {_contextMenuGrid, checkButton, checkAllButton, uncheckButton, uncheckAllButton, undoLastAction, redoPreviousAction})

            .Tools.AddTool(checkButton.Key)
            .Tools.AddTool(checkAllButton.Key)
            .Tools.AddTool(uncheckButton.Key)
            .Tools.AddTool(uncheckAllButton.Key)

            .Tools.AddTool(undoLastAction.Key).InstanceProps.IsFirstInGroup = True
            .Tools.AddTool(redoPreviousAction.Key)
        End With

        Dim toolTipInfo As UltraToolTipInfo = Me.uttmCompare.GetUltraToolTip(Me.btnExport)
        toolTipInfo.ToolTipText = GraphicalCompareFormStrings.ExportExcel_Tooltip

        toolTipInfo = Me.uttmCompare.GetUltraToolTip(Me.btnSave)
        toolTipInfo.ToolTipText = GraphicalCompareFormStrings.SaveCompareResult_Tooltip

        toolTipInfo = Me.uttmCompare.GetUltraToolTip(Me.btnSync)
        toolTipInfo.ToolTipText = GraphicalCompareFormStrings.SyncViewCenter_Tooltip

        toolTipInfo = Me.uttmCompare.GetUltraToolTip(Me.txtReferenceDocument)
        toolTipInfo.ToolTipText = _refDocument.Text
    End Sub

    Private Sub MarkConnectorTableModifications(refGroup As VdSVGGroup, compGroup As VdSVGGroup)
        Dim refTableCompareEntry As New SvgTableCompareEntry(refGroup, 0, Nothing)
        Dim compTableCompareEntry As New SvgTableCompareEntry(compGroup, 0, Nothing)

        GetConnectorTableTexts_Recursively(1, refGroup, refTableCompareEntry)
        GetConnectorTableTexts_Recursively(1, compGroup, compTableCompareEntry)

        FindConnectorTableModifications_Recursively(refTableCompareEntry, compTableCompareEntry)

        MarkConnectorTableModifications_Recursively(refTableCompareEntry)
        MarkConnectorTableModifications_Recursively(compTableCompareEntry)

    End Sub

    Private Sub MarkConnectorTableModifications_Recursively(tableCompareEntry As SvgTableCompareEntry)
        Const EPSILON As Single = 0.01
        If (Not tableCompareEntry.HasMatchEntry) AndAlso (tableCompareEntry.Parent IsNot Nothing) Then
            Dim marker As New VdTextEx(tableCompareEntry.Group.Document)
            With marker
                .setDocumentDefaults()
                .InsertionPoint.x = CSng(tableCompareEntry.GetMinXCoordinate)
                .InsertionPoint.y = tableCompareEntry.Texts.Select(Function(txt) txt.InsertionPoint.y).Average
                .HorJustify = Constants.VdConstHorJust.VdTextHorRight
                .Bold = True
                .IsMarker = True
                .PenColor = New vdObjects.vdColor(Color.Red)
                .Height = CSng(tableCompareEntry.Texts.Select(Function(txt) txt.Height).Average * 1.25)
                .TextString = "=>  "
            End With

            Dim found As Boolean = False
            For Each localMarker As VdTextEx In tableCompareEntry.Group.Figures.Where(Function(element) TypeOf element Is VdTextEx AndAlso DirectCast(element, VdTextEx).IsMarker).ToList
                If Math.Abs(localMarker.InsertionPoint.x - marker.InsertionPoint.x) < EPSILON AndAlso Math.Abs(localMarker.InsertionPoint.y - marker.InsertionPoint.y) < EPSILON Then
                    found = True
                    Exit For
                End If
            Next
            If Not found Then
                tableCompareEntry.Group.Figures.Add(marker)
                tableCompareEntry.Group.Update()
            End If
        End If
        For Each childTableCompareEntry As SvgTableCompareEntry In tableCompareEntry.Childs
            MarkConnectorTableModifications_Recursively(childTableCompareEntry)
        Next
    End Sub

    Private Sub MarkSelectedObjectInDrawings(compGroup As VdSVGGroup, refGroup As VdSVGGroup)
        Dim compHighlightRects As New List(Of VdRectEx)

        If (Not compGroup.BoundingBox.IsEmpty) Then
            _compareDrawing.vdCanvas.BaseControl.ActiveDocument.ZoomWindow(New gPoint(compGroup.BoundingBox.UpperLeft.x - 200, compGroup.BoundingBox.UpperLeft.y + 200), New gPoint(compGroup.BoundingBox.LowerRight.x + 200, compGroup.BoundingBox.LowerRight.y - 200))
        Else
            _compareDrawing.vdCanvas.BaseControl.ActiveDocument.ZoomWindow(New gPoint(refGroup.BoundingBox.UpperLeft.x - 200, refGroup.BoundingBox.UpperLeft.y + 200), New gPoint(refGroup.BoundingBox.LowerRight.x + 200, refGroup.BoundingBox.LowerRight.y - 200))
        End If

        If (Not refGroup.BoundingBox.IsEmpty) Then
            _refDrawing.vdCanvas.BaseControl.ActiveDocument.ZoomWindow(New gPoint(refGroup.BoundingBox.UpperLeft.x - 200, refGroup.BoundingBox.UpperLeft.y + 200), New gPoint(refGroup.BoundingBox.LowerRight.x + 200, refGroup.BoundingBox.LowerRight.y - 200))
        Else
            _refDrawing.vdCanvas.BaseControl.ActiveDocument.ZoomWindow(New gPoint(compGroup.BoundingBox.UpperLeft.x - 200, compGroup.BoundingBox.UpperLeft.y + 200), New gPoint(compGroup.BoundingBox.LowerRight.x + 200, compGroup.BoundingBox.LowerRight.y - 200))
        End If

        compHighlightRects.Add(GetHighlightRect(_compareDrawing.vdCanvas.BaseControl.ActiveDocument, compGroup.BoundingBox.Height + 40, compGroup.Id, New gPoint(compGroup.BoundingBox.Min.x - 20, compGroup.BoundingBox.Min.y - 20), Color.Orange, 2, compGroup.BoundingBox.Width + 40))

        _compareDrawing.HighlightRects = compHighlightRects

        _compareDrawing.vdCanvas.BaseControl.ActiveDocument.Invalidate()
        _refDrawing.vdCanvas.BaseControl.ActiveDocument.Invalidate()
    End Sub

    Private Sub RestorePreviousResultInformation()
        _undoActionRunning = True

        Dim previousResultInformation As Dictionary(Of Integer, Tuple(Of Boolean, Boolean, String)) = StoreCurrentResultInformation()

        With Me.ugResults
            .BeginUpdate()

            For Each row As UltraGridRow In .Rows
                If (_previousResultInformation.ContainsKey(row.ListIndex)) Then
                    With _previousResultInformation(row.ListIndex)
                        row.Cells(NameOf(GraphicalCompareFormStrings.Checked_ColumnCaption)).Value = .Item1
                        row.Cells(NameOf(GraphicalCompareFormStrings.ToBeChanged_ColumnCaption)).Value = .Item2
                        row.Cells(NameOf(GraphicalCompareFormStrings.Comment_ColumnCaption)).Value = .Item3
                    End With
                End If
            Next

            .EndUpdate()
        End With

        ShowGraphics(True)

        _previousResultInformation = previousResultInformation
        _undoActionRunning = False
    End Sub

    Private Function SaveCheckedCompareResultInformation() As Boolean
        Dim dialogResult As DialogResult = MessageBox.Show(Me, GraphicalCompareFormStrings.SaveCompareResult_Msg, [Shared].MSG_BOX_TITLE, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question)
        If (dialogResult = System.Windows.Forms.DialogResult.Yes) Then
            Dim identicalCompareResultEntriesExists As Boolean = False

            If _checkedCompareResult Is Nothing Then
                If _harnessPartNumberMismatch Then
                    MessageBoxEx.ShowError("Saving compare results aborted: Harness part number is not equal!")
                    Return False
                End If
#If DEBUG OrElse CONFIG = "Debug" Then
                Throw New NullReferenceException("_checkedCompareResult is null!")
#Else
                Return false
#End If
            End If

            _checkedCompareResult.CheckedCompareResultEntries?.Clear()

            For Each row As UltraGridRow In Me.ugResults.Rows
                If (CBool((row.Cells(NameOf(GraphicalCompareFormStrings.Checked_ColumnCaption)).Value))) Then
                    Dim compareResultEntry As CheckedCompareResultEntry = _checkedCompareResult.CheckedCompareResultEntries.AddCheckedCompareResultEntry(row.Cells(NameOf(GraphicalCompareFormStrings.CompSign_ColumnCaption)).Value.ToString, row.Cells(NameOf(GraphicalCompareFormStrings.RefSign_ColumnCaption)).Value.ToString)
                    If (compareResultEntry IsNot Nothing) Then
                        With compareResultEntry
                            .Comment = row.Cells(NameOf(GraphicalCompareFormStrings.Comment_ColumnCaption)).Value.ToString
                            .ToBeChanged = CBool(row.Cells(NameOf(GraphicalCompareFormStrings.ToBeChanged_ColumnCaption)).Value)
                        End With
                    Else
                        _refDocument._logHub.WriteLogMessage(New LogEventArgs With {.LogLevel = LogEventArgs.LoggingLevel.Warning, .LogMessage = String.Format(GraphicalCompareFormStrings.CannotSaveResult_Msg, row.Cells(NameOf(GraphicalCompareFormStrings.ObjName_ColumnCaption)).Value.ToString, row.Cells(NameOf(GraphicalCompareFormStrings.ObjType_ColumnCaption)).Value.ToString)})

                        identicalCompareResultEntriesExists = True
                    End If
                End If
            Next

            Dim saveError As String = SaveCheckedCompareResultInformationIntoReferenceDocument()
            If (saveError <> String.Empty) Then
                MessageBox.Show(String.Format(GraphicalCompareFormStrings.ErrorSaveResult_Msg, vbCrLf, saveError), [Shared].MSG_BOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error)

                Return False
            ElseIf (identicalCompareResultEntriesExists) Then
                MessageBox.Show(String.Format(GraphicalCompareFormStrings.SaveWarning1_Msg, vbCrLf, GraphicalCompareFormStrings.SaveWarning2_Msg), [Shared].MSG_BOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If

            _isDirty = False
            _previousResultInformation = New Dictionary(Of Integer, Tuple(Of Boolean, Boolean, String))
            _undoActionTriggered = False
        ElseIf (dialogResult = System.Windows.Forms.DialogResult.Cancel) Then
            Return False
        End If

        Return True
    End Function

    Private Function SaveCheckedCompareResultInformationIntoReferenceDocument() As String
        Using Me.EnableWaitCursor
            Dim old_gcri As [Lib].IO.Files.Hcv.GraphicalCheckedCompareResultInfoContainerFile = _refDocument.File.GCRI
            Dim old_tcri As [Lib].IO.Files.Hcv.TechnicalCheckedCompareResultInfoContainerFile = _refDocument.File.TCRI

            Try
                _refDocument.File.GCRI = CType(_refDocument.GetCompareResultInfoFileContainer([Lib].IO.Files.Hcv.KnownContainerFileFlags.GCRI), [Lib].IO.Files.Hcv.GraphicalCheckedCompareResultInfoContainerFile)
                _refDocument.File.TCRI = CType(_refDocument.GetCompareResultInfoFileContainer([Lib].IO.Files.Hcv.KnownContainerFileFlags.TCRI), [Lib].IO.Files.Hcv.TechnicalCheckedCompareResultInfoContainerFile)

                If Not _refDocument.File.IsXhcvChild Then
                    _refDocument.File.Save(useTempIntermediateFile:=True) ' hint use temp intermediate to avoid corrupting zip archives while saving them -> when temp files is used the current file will only be overwritten when saving was successfull
                End If
            Catch ex As Exception
                _refDocument.File.GCRI = old_gcri
                _refDocument.File.TCRI = old_tcri
                Return ex.Message
            End Try
        End Using
        Return String.Empty
    End Function

    Private Function SelectFromView(layout As vdLayout, isRef As Boolean, picBox As PictureBox, viewCenter As gPoint, x As Integer, y As Integer) As Boolean
        If (viewCenter Is Nothing) Then Return False

        Dim picCenter As New Drawing.Point(CInt(picBox.Width / 2), CInt(picBox.Height / 2))
        Dim offsetFromCenter As New gPoint((x - picCenter.X), (y - picCenter.Y))

        Dim vdrawCenterPixel As gPoint
        If (isRef) Then
            offsetFromCenter *= GetAppliedReferenceRatio()
            vdrawCenterPixel = layout.ActiveRender.View2PixelD(_viewCenterRef)
        Else
            offsetFromCenter *= GetAppliedCompareRatio()
            vdrawCenterPixel = layout.ActiveRender.View2PixelD(_viewCenterCompare)
        End If
        vdrawCenterPixel += offsetFromCenter

        Dim targetClickPoint As System.Drawing.Point
        targetClickPoint.X = CInt(vdrawCenterPixel.x)
        targetClickPoint.Y = CInt(vdrawCenterPixel.y)

        Dim group As VdSVGGroup = TryCast(layout.GetEntityFromPoint(targetClickPoint, 6, False, LockLayerMethodEnum.DisableAll), VdSVGGroup)
        If (group IsNot Nothing) Then
            If (group.Layer.Name = LAYER_CHANGE_INDICATORS) Then
                group = DirectCast(layout.Entities.FindFromId(CInt(group.Label)), VdSVGGroup)
            End If

            Dim success As Boolean = False

            _clickedInView = True

            Me.ugResults.BeginUpdate()

            For Each row As UltraGridRow In Me.ugResults.Rows
                If (Not row.Hidden) Then
                    Dim groupFromRow As VdSVGGroup = If(isRef, DirectCast(DirectCast(row.ListObject, UltraDataRow).Tag, Tuple(Of VdSVGGroup, VdSVGGroup)).Item1, DirectCast(DirectCast(row.ListObject, UltraDataRow).Tag, Tuple(Of VdSVGGroup, VdSVGGroup)).Item2)
                    If (groupFromRow IsNot Nothing) AndAlso (groupFromRow Is group) Then
                        Me.ugResults.Selected.Rows.Clear()
                        Me.ugResults.Selected.Rows.Add(row)

                        success = True

                        Exit For
                    End If
                End If
            Next

            If (Me.ugResults.Selected.Rows.Count <> 0) Then Me.ugResults.ActiveRowScrollRegion.ScrollRowIntoView(Me.ugResults.Selected.Rows(0))

            Me.ugResults.EndUpdate()

            _clickedInView = False

            Return success
        Else
            DeselectAll()
        End If

        Return False
    End Function

    Private Sub SetAreaFilter(boundingBox As Box, resetSelection As Boolean)
        Dim compareGroups As New vdCollections.vdEntities
        Dim groupsInGrid As New vdCollections.vdEntities
        Dim refGroups As New vdCollections.vdEntities

        _hiddenGroups = New List(Of VdSVGGroup)

        FilterGroupsInSelectedArea(boundingBox, refGroups, _refLayout, resetSelection)
        FilterGroupsInSelectedArea(boundingBox, compareGroups, _compareLayout, resetSelection)

        Me.ugResults.BeginUpdate()

        For Each row As UltraGridRow In Me.ugResults.Rows
            Dim refGroup As VdSVGGroup = DirectCast(DirectCast(row.ListObject, UltraDataRow).Tag, Tuple(Of VdSVGGroup, VdSVGGroup)).Item1
            Dim compareGroup As VdSVGGroup = DirectCast(DirectCast(row.ListObject, UltraDataRow).Tag, Tuple(Of VdSVGGroup, VdSVGGroup)).Item2

            If (refGroup IsNot Nothing) Then
                row.Hidden = Not refGroups.FindItem(refGroup)

                If (Not row.Hidden) Then groupsInGrid.AddItem(refGroup)
            ElseIf (compareGroup IsNot Nothing) Then
                row.Hidden = Not compareGroups.FindItem(compareGroup)

                If (Not row.Hidden) Then
                    groupsInGrid.AddItem(compareGroup)
                End If
            Else
                row.Hidden = True
            End If
        Next

        Me.ugResults.EndUpdate()

        boundingBox = groupsInGrid.GetBoundingBox(True, True)

        _areaRectCompare = New VdSVGGroup
        With _areaRectCompare
            .SetUnRegisterDocument(_compareLayout.Document)
            .setDocumentDefaults()

            .KblId = String.Empty
            Dim myRect As New vdRect
            With myRect
                .InsertionPoint.x = CSng(boundingBox.Min.x - 2.5)
                .InsertionPoint.y = CSng(boundingBox.Min.y - 2.5)
                .Height = CSng(boundingBox.Height) + 5
                .LineType = _compareLayout.Document.LineTypes.DPIDashDot
                .PenColor = New vdObjects.vdColor(Color.Magenta)
                .PenWidth = 0.1
                .Width = CSng(boundingBox.Width) + 5
            End With
            .Figures.Add(myRect)
            .SVGType = SvgType.Undefined.ToString
            .SymbolType = SvgType.Undefined.ToString
        End With

        _compareLayout.Entities.AddItem(_areaRectCompare)
        _compareLayout.Invalidate()

        _areaRectRef = DirectCast(_areaRectCompare.Clone(_refLayout.Document), VdSVGGroup)

        _refLayout.Entities.AddItem(_areaRectRef)
        _refLayout.Invalidate()

        If (resetSelection) Then
            For Each row As UltraGridRow In Me.ugResults.Rows
                If (Not row.Hidden) Then
                    Me.ugResults.EventManager.AllEventsEnabled = False
                    Me.ugResults.Selected.Rows.Clear()
                    Me.ugResults.EventManager.AllEventsEnabled = True

                    Me.ugResults.Selected.Rows.Add(row)

                    Exit For
                End If
            Next
        Else
            ShowGraphics(True)
        End If

        Me.ugbCompareResults.Text = GraphicalCompareFormStrings.FilterResults_GroupBoxCaption
        Me.btnResetAreaFilter.Enabled = True
        Me.uddbSetAreaFilter.Enabled = False
    End Sub

    Private Sub ClearGraphics()
        Me.picRefResult.Image = Nothing
        Me.picRefResult.Update()
        Me.picCompResult.Image = Nothing
        Me.picCompResult.Update()
    End Sub

    Private Sub ShowGraphics(renderImage As Boolean)
        Me.lblNoGraphicsToDisplay.Visible = False

        If (Me.ugResults.Selected.Rows.Count = 0) Then
            If (Me.ugResults.Rows.Count > 0) Then
                CreatePictureOfSelectedObject(_refDrawing.vdCanvas.BaseControl.ActiveDocument, Nothing, True, renderImage)
                CreatePictureOfSelectedObject(_compareDrawing.vdCanvas.BaseControl.ActiveDocument, Nothing, False, renderImage)
            Else
                ClearGraphics()
            End If
        Else
            For Each selectedRow As UltraGridRow In Me.ugResults.Selected.Rows
                Me.txtComment.Text = selectedRow.Cells(NameOf(GraphicalCompareFormStrings.Comment_ColumnCaption)).Value.ToString

                If (DirectCast(selectedRow.ListObject, UltraDataRow).Tag IsNot Nothing) Then
                    Dim compBoundingBox As Box = Nothing
                    Dim compGroup As VdSVGGroup = DirectCast(DirectCast(selectedRow.ListObject, UltraDataRow).Tag, Tuple(Of VdSVGGroup, VdSVGGroup)).Item2
                    Dim compIndicatorGroup As VdSVGGroup = Nothing

                    Dim refBoundingBox As Box = Nothing
                    Dim refGroup As VdSVGGroup = DirectCast(DirectCast(selectedRow.ListObject, UltraDataRow).Tag, Tuple(Of VdSVGGroup, VdSVGGroup)).Item1
                    Dim refIndicatorGroup As VdSVGGroup = Nothing

                    If (refGroup IsNot Nothing) Then
                        refBoundingBox = refGroup.BoundingBox

                        If ((refGroup.SymbolType = SvgSymbolType.Connector.ToString OrElse refGroup.SymbolType = SvgSymbolType.ModuleTable.ToString) AndAlso refGroup.SVGType = SvgType.table.ToString) AndAlso (compGroup IsNot Nothing AndAlso (compGroup.SymbolType = SvgSymbolType.Connector.ToString OrElse compGroup.SymbolType = SvgSymbolType.ModuleTable.ToString) AndAlso compGroup.SVGType = SvgType.table.ToString) Then
                            MarkConnectorTableModifications(refGroup, compGroup)
                        End If

                        refGroup.Lighting = Lighting.Highlight
                        refGroup.Invalidate()

                        For Each group As VdSVGGroup In _refLayout.Entities
                            If (group.Layer.Name = LAYER_CHANGE_INDICATORS) AndAlso (CInt(group.Label) = refGroup.Id) Then
                                refIndicatorGroup = group
                                refIndicatorGroup.Lighting = Lighting.Highlight
                                refIndicatorGroup.Invalidate()

                                Exit For
                            End If
                        Next
                    Else
                        refBoundingBox = compGroup.BoundingBox
                    End If

                    If (compGroup IsNot Nothing) Then
                        compBoundingBox = compGroup.BoundingBox

                        If (compGroup.SymbolType = SvgSymbolType.Connector.ToString AndAlso compGroup.SVGType = SvgType.table.ToString) AndAlso (refGroup IsNot Nothing AndAlso refGroup.SymbolType = SvgSymbolType.Connector.ToString AndAlso refGroup.SVGType = SvgType.table.ToString) Then
                            MarkConnectorTableModifications(compGroup, refGroup)
                        End If

                        compGroup.Lighting = Lighting.Highlight
                        compGroup.Invalidate()

                        For Each group As VdSVGGroup In _compareLayout.Entities
                            If (group.Layer.Name = LAYER_CHANGE_INDICATORS) AndAlso (CInt(group.Label) = compGroup.Id) Then
                                compIndicatorGroup = group
                                compIndicatorGroup.Lighting = Lighting.Highlight
                                compIndicatorGroup.Invalidate()

                                Exit For
                            End If
                        Next
                    Else
                        compBoundingBox = refGroup.BoundingBox
                    End If

                    If (refBoundingBox.IsEmpty) Then refBoundingBox = compBoundingBox
                    If (compBoundingBox.IsEmpty) Then compBoundingBox = refBoundingBox

                    If (refBoundingBox.IsEmpty) AndAlso (compBoundingBox.IsEmpty) Then
                        Me.lblNoGraphicsToDisplay.Visible = True

                        Exit Sub
                    End If

                    CreatePictureOfSelectedObject(_refDrawing.vdCanvas.BaseControl.ActiveDocument, refBoundingBox, True, renderImage)
                    CreatePictureOfSelectedObject(_compareDrawing.vdCanvas.BaseControl.ActiveDocument, compBoundingBox, False, renderImage)

                    If (refGroup IsNot Nothing) Then
                        refGroup.Lighting = Lighting.Normal
                        refGroup.Invalidate()
                    End If

                    If (refIndicatorGroup IsNot Nothing) Then
                        refIndicatorGroup.Lighting = Lighting.Normal
                        refIndicatorGroup.Invalidate()
                    End If

                    If (compGroup IsNot Nothing) Then
                        compGroup.Lighting = Lighting.Normal
                        compGroup.Invalidate()
                    End If

                    If (compIndicatorGroup IsNot Nothing) Then
                        compIndicatorGroup.Lighting = Lighting.Normal
                        compIndicatorGroup.Invalidate()
                    End If

                    If (Not renderImage) Then
                        If (compGroup Is Nothing) Then compGroup = refGroup
                        If (refGroup Is Nothing) Then refGroup = compGroup

                        MarkSelectedObjectInDrawings(compGroup, refGroup)
                    End If

                    Exit For
                End If
            Next
        End If

        If (Not _clickedInGrid) Then FilterResultGridRowsDynamically()
        If (Not renderImage) Then ShowGraphics(True)  'HINT MR this solution is a pure mess! just removed call to wheel event handler, the rest would need to entirely restructure this hell
    End Sub

    Private Function StoreCurrentResultInformation() As Dictionary(Of Integer, Tuple(Of Boolean, Boolean, String))
        Dim alreadyUpdating As Boolean = Me.ugResults.IsUpdating
        Dim prevResultInformation As New Dictionary(Of Integer, Tuple(Of Boolean, Boolean, String))

        With Me.ugResults
            If (Not alreadyUpdating) Then
                .BeginUpdate()
            End If

            For Each row As UltraGridRow In .Rows
                prevResultInformation.Add(row.ListIndex, New Tuple(Of Boolean, Boolean, String)(CBool(row.Cells(NameOf(GraphicalCompareFormStrings.Checked_ColumnCaption)).Value), CBool(row.Cells(NameOf(GraphicalCompareFormStrings.ToBeChanged_ColumnCaption)).Value), row.Cells(NameOf(GraphicalCompareFormStrings.Comment_ColumnCaption)).Value.ToString))
            Next

            If (Not alreadyUpdating) Then
                .EndUpdate()
            End If
        End With

        Return prevResultInformation
    End Function

    Private Sub UpdateCheckStateInGraphic(checked As Boolean, group As VdSVGGroup, highlightRects As List(Of VdRectEx), isFilteredOut As Boolean, layout As vdLayout)
        If (isFilteredOut) Then
            group.GraphicalCompareCheckState = GraphicalCompareCheckState.None
        Else
            If (checked) Then
                group.GraphicalCompareCheckState = GraphicalCompareCheckState.Checked
            Else
                group.GraphicalCompareCheckState = GraphicalCompareCheckState.Unchecked
            End If
        End If

        For Each highlightRect As VdRectEx In highlightRects.Where(Function(rect) CInt(rect.Label) = group.Id)
            If (isFilteredOut) Then
                highlightRect.HatchProperties.FillMode = Constants.VdConstFill.VdFillModeNone
                highlightRect.PenColor.AlphaBlending = 0
            Else
                highlightRect.HatchProperties.FillMode = Constants.VdConstFill.VdFillModeSolid
                highlightRect.PenColor.AlphaBlending = 255

                If (checked) Then
                    highlightRect.HatchProperties.FillColor.SystemColor = Color.Green
                    highlightRect.PenColor.SystemColor = Color.Green
                Else
                    highlightRect.HatchProperties.FillColor.SystemColor = Color.Orange
                    highlightRect.PenColor.SystemColor = Color.Orange
                End If
            End If

            highlightRect.Update()
            highlightRect.Invalidate()
        Next

        For Each grp As VdSVGGroup In layout.Entities
            If (grp.Label <> String.Empty) AndAlso (CInt(grp.Label) = group.Id) Then
                layout.Entities.RemoveItem(grp)

                If (Not isFilteredOut) Then
                    Dim changeIndicatorGroup As VdSVGGroup = DrawChangeIndicator(layout.Document, group)

                    layout.Entities.AddItem(changeIndicatorGroup)
                End If

                Exit For
            End If
        Next
    End Sub

    Private Sub GraphicalCompareForm_FormClosed(sender As Object, e As FormClosedEventArgs) Handles Me.FormClosed
        ClearHighlightRects()
    End Sub

    Private Sub ClearHighlightRects()
        If _compareDrawing IsNot Nothing Then
            If _compareDrawing.HighlightRects IsNot Nothing Then
                _compareDrawing.HighlightRects = Nothing
                If _compareDrawing.vdCanvas.BaseControl.ActiveDocument IsNot Nothing Then
                    _compareDrawing.vdCanvas.BaseControl.ActiveDocument.Invalidate()
                End If
            End If
        End If
        If _refDrawing IsNot Nothing Then
            If (_refDrawing.vdCanvas.BaseControl.ActiveDocument IsNot Nothing) AndAlso (_refDrawing.vdCanvas.BaseControl.ActiveDocument.CommandAction.OpenLoops <> 0) Then
                _refDrawing.vdCanvas.BaseControl.ActiveDocument.CommandAction.Cancel()
            End If
            If _refDrawing.HighlightRects IsNot Nothing Then
                _refDrawing.HighlightRects = Nothing
                _refDrawing.vdCanvas.BaseControl.ActiveDocument?.Invalidate()
            End If
        End If
    End Sub

    Private Sub GraphicalCompareForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        If Me.bwCompare.IsBusy Then
            Me.bwCompare.CancelAsync()

            e.Cancel = True
        ElseIf _exportRunning Then
            _exportCancelled = True
            e.Cancel = True
            MessageBox.Show(GraphicalCompareFormStrings.ExportCancelled_Msg, [Shared].MSG_BOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Information)
        ElseIf (Not _harnessPartNumberMismatch) AndAlso (_isDirty) Then
            e.Cancel = Not SaveCheckedCompareResultInformation()
        End If
    End Sub

    Private Sub GraphicalCompareForm_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        If (e.Control) Then _ctrlKeyPressed = True
        If (e.KeyCode = Keys.Escape) Then Me.Close()
        If (e.KeyCode = Keys.Space) AndAlso (Me.ugResults.Selected.Rows.Count <> 0) AndAlso (Not Me.txtComment.Focused) Then
            Me.ugResults.BeginUpdate()

            For Each selectedRow As UltraGridRow In Me.ugResults.Selected.Rows
                If (CBool(selectedRow.Cells(NameOf(GraphicalCompareFormStrings.Checked_ColumnCaption)).Value)) Then
                    If (MessageBox.Show(GraphicalCompareFormStrings.Uncheck_Msg, [Shared].MSG_BOX_TITLE, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) = System.Windows.Forms.DialogResult.Yes) Then
                        selectedRow.Cells(NameOf(GraphicalCompareFormStrings.Checked_ColumnCaption)).Value = False
                        selectedRow.Cells(NameOf(GraphicalCompareFormStrings.ToBeChanged_ColumnCaption)).Value = False
                        selectedRow.Cells(NameOf(GraphicalCompareFormStrings.Comment_ColumnCaption)).Value = String.Empty

                        Me.txtComment.Text = String.Empty
                    End If
                Else
                    selectedRow.Cells(NameOf(GraphicalCompareFormStrings.Checked_ColumnCaption)).Value = True
                End If
            Next

            If ugResults.Selected.Rows.Count = 1 Then
                btnNext_Click(Nothing, New EventArgs)
            End If

            Me.ugResults.EndUpdate()

            _isDirty = True

            ShowGraphics(True)
        End If
    End Sub

    Private Sub GraphicalCompareForm_KeyUp(sender As Object, e As KeyEventArgs) Handles Me.KeyUp
        _ctrlKeyPressed = False
    End Sub

    Private Sub GraphicalCompareForm_MouseMove(sender As Object, e As MouseEventArgs) Handles Me.MouseMove
        If (Me.ugResults.Visible) Then
            Me.lblRowCountInfo.Text = String.Format(GraphicalCompareFormStrings.RowCount_Label, Me.ugResults.Rows.VisibleRowCount, Me.ugResults.Rows.Count)
        Else
            Me.lblRowCountInfo.Text = String.Empty
        End If
    End Sub

    Private Sub GraphicalCompareForm_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        If (WindowState <> _lastWindowState) Then
            _lastWindowState = WindowState

            Me.picRefResult.Size = New Drawing.Size(CInt(Me.upnEntry.Size.Width / 2) - 3, CInt(Me.upnEntry.Size.Height / 2))
            Me.picCompResult.Location = New POINT(Me.picRefResult.Size.Width + 3, Me.picCompResult.Location.Y)
            Me.picCompResult.Size = New Drawing.Size(CInt(Me.upnEntry.Size.Width / 2) - 3, CInt(Me.upnEntry.Size.Height / 2))

            If (Me.upnResult.Visible AndAlso Me.ugResults.Rows.Count <> 0) Then
                ShowGraphics(True)
            End If
        End If
    End Sub

    Private Sub GraphicalCompareForm_ResizeEnd(sender As Object, e As EventArgs) Handles Me.ResizeEnd
        Me.picRefResult.Size = New Drawing.Size(CInt(Me.upnEntry.Size.Width / 2) - 3, CInt(Me.upnEntry.Size.Height / 2))
        Me.picCompResult.Location = New POINT(Me.picRefResult.Size.Width + 3, Me.picCompResult.Location.Y)
        Me.picCompResult.Size = New Drawing.Size(CInt(Me.upnEntry.Size.Width / 2) - 3, CInt(Me.upnEntry.Size.Height / 2))

        If (Me.upnResult.Visible AndAlso Me.ugResults.Rows.Count <> 0) Then
            ShowGraphics(True)
        End If
    End Sub

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Me.Close()
    End Sub

    Private Sub btnCompare_Click(sender As Object, e As EventArgs) Handles btnCompare.Click


        If (_compareRunning) Then
            _compareRunning = False

            Me.btnClose.Enabled = True
            Me.btnCompare.Text = GraphicalCompareFormStrings.Compare_Text
            Me.bwCompare.CancelAsync()
        ElseIf (_exportRunning) Then
            _exportCancelled = True
        ElseIf (_refDrawing IsNot Nothing) Then
            If (Not _harnessPartNumberMismatch) AndAlso (_isDirty) Then
                If (Not SaveCheckedCompareResultInformation()) Then
                    Exit Sub
                End If
            End If

            If (_compareDrawing Is Nothing) Then
                MessageBox.Show(GraphicalCompareFormStrings.NoCompareDrw_Msg, [Shared].MSG_BOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning)

                Exit Sub
            End If

            If (_refDocument.KBL.HarnessPartNumber <> _compareDocument.KBL.HarnessPartNumber) Then
                _harnessPartNumberMismatch = True
            Else
                _harnessPartNumberMismatch = False
            End If

            If (_harnessPartNumberMismatch) AndAlso (MessageBox.Show(GraphicalCompareFormStrings.DifferentHarnessPartNumbers_Msg, [Shared].MSG_BOX_TITLE, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) = MsgBoxResult.No) Then
                Exit Sub
            End If

            Me.Cursor = Cursors.WaitCursor

            _compareRunning = True

            Me.btnClose.Enabled = False
            Me.btnCompare.Text = GraphicalCompareFormStrings.Cancel_Text
            Me.btnExport.Enabled = False
            Me.btnResetAreaFilter.Visible = False
            Me.btnSave.Enabled = False
            Me.chkConsiderText.Enabled = False

            Me.lblLegend1.Visible = False
            Me.lblLegend2.Visible = False
            Me.lblLegend3.Visible = False
            Me.lblRowCountInfo.Visible = False

            Me.uceCompareDocument.ReadOnly = True
            Me.uceCompareDrw.ReadOnly = True
            Me.uceReferenceDrw.ReadOnly = True
            Me.uckConsiderOffset.Enabled = False
            Me.uckDisplayIndicators.Visible = False
            Me.uckFilterDynamically.Visible = False
            Me.uddbSetAreaFilter.Visible = False
            Me.ugResults.DataSource = Nothing
            Me.upnResult.Visible = False
            Me.upbCompare.Visible = True
            Me.upbCompare.Value = 0

            ClearGraphics()


            Me.bwCompare.RunWorkerAsync()
        End If
    End Sub

    Private Sub btnExport_Click(sender As Object, e As EventArgs) Handles btnExport.Click
        Using sfdExcel As New SaveFileDialog
            With sfdExcel
                .DefaultExt = KnownFile.XLSX.Trim("."c)

                If (_refDocument.KBL.GetChanges.Any) Then
                    .FileName = String.Format("{0}{1}{2}_{3}_{4}_Compare_Result.xlsx", Now.Year, Format(Now.Month, "00"), Format(Now.Day, "00"), Replace(_refDocument.KBL.HarnessPartNumber, " ", String.Empty), _refDocument.KBL.GetChanges.Max(Function(change) change.Id))
                Else
                    .FileName = String.Format("{0}{1}{2}_{3}_Compare_Result.xlsx", Now.Year, Format(Now.Month, "00"), Format(Now.Day, "00"), Replace(_refDocument.KBL.HarnessPartNumber, " ", String.Empty))
                End If

                .Filter = "Excel files (*.xlsx)|*.xlsx|Excel files (97-2003) (*.xls)|*.xls"
                .Title = GraphicalCompareFormStrings.ExportExcelFile_Title

                If (.ShowDialog(Me) = DialogResult.OK) Then
                    Me.Cursor = Cursors.WaitCursor

                    Try
                        Me.ugResults.BeginUpdate()
                        Me.ugResults.DisplayLayout.Bands(0).Columns(NameOf(GraphicalCompareFormStrings.Comment_ColumnCaption)).Hidden = False

                        _exportFileName = .FileName
                        _exportRunning = True

                        Dim workbook As New Workbook

                        If (Path.GetExtension(_exportFileName).Equals(".xlsx", StringComparison.CurrentCultureIgnoreCase)) Then
                            workbook.SetCurrentFormat(WorkbookFormat.Excel2007)
                        Else
                            workbook.SetCurrentFormat(WorkbookFormat.Excel97To2003)
                        End If

                        workbook.Worksheets.Add(GraphicalCompareFormStrings.Worksheet_Name)

                        Me.ugeeResults.ExportAsync(Me.ugResults, workbook, 1, 0)
                    Catch ex As Exception

                        MessageBox.Show(String.Format(GraphicalCompareFormStrings.ErrorExportExcel_Msg, vbCrLf, ex.Message), [Shared].MSG_BOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error)

                        Me.ugResults.DisplayLayout.Bands(0).Columns(NameOf(GraphicalCompareFormStrings.Comment_ColumnCaption)).Hidden = True
                        Me.ugResults.EndUpdate()

                        _exportRunning = False
                    End Try

                    Me.Cursor = Cursors.Default
                End If
            End With
        End Using
    End Sub

    Private Sub btnNext_Click(sender As Object, e As EventArgs) Handles btnNext.Click
        If (Me.ugResults.Selected.Rows.Count = 0) Then
            If (Me.ugResults.Rows.Count <> 0) Then Me.ugResults.Rows(0).Selected = True
        Else
            If (_ctrlKeyPressed) AndAlso (Me.ugResults.Selected.Rows.Count <> 0) Then
                For Each row As UltraGridRow In Me.ugResults.Selected.Rows
                    row.Cells(NameOf(GraphicalCompareFormStrings.Checked_ColumnCaption)).Value = True
                Next
            End If

            Dim index As Integer = Me.ugResults.Selected.Rows(0).Index + 1
            If (index > Me.ugResults.Rows.Count - 1) Then
                Exit Sub
            End If

            Do While (Me.ugResults.Rows(index).IsFilteredOut OrElse Me.ugResults.Rows(index).Hidden)
                If (index >= Me.ugResults.Rows.Count - 1) Then
                    Exit Sub
                End If

                index += 1
            Loop

            Me.ugResults.ActiveRow = Me.ugResults.Rows(index)
            Me.ugResults.Rows(index).Selected = True
        End If

        Me.ugResults.Focus()

        If (Me.ugResults.Selected.Rows.Count <> 0) Then
            Me.ugResults.ActiveRowScrollRegion.ScrollRowIntoView(Me.ugResults.Selected.Rows(0))
        End If
    End Sub

    Private Sub btnOffsetPreview_Click(sender As Object, e As EventArgs) Handles btnOffsetPreview.Click
        Using offsetVectorPreviewForm As New OffsetVectorPreviewForm(_vertexPositionPairs)
            offsetVectorPreviewForm.ShowDialog(Me)
        End Using
    End Sub

    Private Sub btnPrevious_Click(sender As Object, e As EventArgs) Handles btnPrevious.Click
        If (Me.ugResults.Selected.Rows.Count = 0) Then
            If (Me.ugResults.Rows.Count <> 0) Then Me.ugResults.Rows(0).Selected = True
        Else
            If (_ctrlKeyPressed) AndAlso (Me.ugResults.Selected.Rows.Count <> 0) Then
                For Each row As UltraGridRow In Me.ugResults.Selected.Rows
                    row.Cells(NameOf(GraphicalCompareFormStrings.Checked_ColumnCaption)).Value = True
                Next
            End If

            Dim index As Integer = Me.ugResults.Selected.Rows(0).Index - 1
            If (index < 0) Then Exit Sub

            Do While (Me.ugResults.Rows(index).IsFilteredOut OrElse Me.ugResults.Rows(index).Hidden)
                If (index <= 0) Then Exit Sub

                index -= 1
            Loop

            Me.ugResults.ActiveRow = Me.ugResults.Rows(index)
            Me.ugResults.Rows(index).Selected = True
        End If

        Me.ugResults.Focus()

        If (Me.ugResults.Selected.Rows.Count <> 0) Then
            Me.ugResults.ActiveRowScrollRegion.ScrollRowIntoView(Me.ugResults.Selected.Rows(0))
        End If
    End Sub

    Private Sub btnResetAreaFilter_Click(sender As Object, e As EventArgs) Handles btnResetAreaFilter.Click
        For Each group As VdSVGGroup In _hiddenGroups
            group.visibility = vdPrimaries.vdFigure.VisibilityEnum.Visible
            group.Invalidate()
        Next

        _compareLayout.Entities.RemoveItem(_areaRectCompare)
        _compareLayout.Invalidate()

        _refLayout.Entities.RemoveItem(_areaRectRef)
        _refLayout.Invalidate()

        _hiddenGroups.Clear()

        Me.ugbCompareResults.Text = GraphicalCompareFormStrings.CompareResults_GroupBoxCaption

        Me.ugResults.BeginUpdate()

        For Each row As UltraGridRow In Me.ugResults.Rows
            row.Hidden = False
        Next

        If (Me.ugResults.Selected.Rows.Count <> 0) Then
            Me.ugResults.ActiveRowScrollRegion.ScrollRowIntoView(Me.ugResults.Selected.Rows(0))
        End If

        Me.ugResults.EndUpdate()

        Me.btnResetAreaFilter.Enabled = False
        Me.uddbSetAreaFilter.Enabled = True

        ShowGraphics(True)
    End Sub

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        SaveCheckedCompareResultInformation()

    End Sub

    Private Sub btnSync_Click(sender As Object, e As EventArgs) Handles btnSync.Click
        If (_compareLayout IsNot Nothing) AndAlso (_refLayout IsNot Nothing) AndAlso (Me.ugResults.Rows.Count <> 0) Then
            _viewCenterCompare = _viewCenterRef
            _viewSizeCompare = _viewSizeRef

            ShowGraphics(True)
        End If
    End Sub

    Private Sub bwCompare_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles bwCompare.DoWork

        Me.bwCompare.ReportProgress(0, "clone")

        _refLayout = DirectCast(_refDrawing.vdCanvas.BaseControl.ActiveDocument.ActiveLayOut.Clone(_refDrawing.vdCanvas.BaseControl.ActiveDocument), vdPrimaries.vdLayout)
        _viewCenterRef = CType(_refDrawing.vdCanvas.BaseControl.ActiveDocument.ActiveLayOut.ViewCenter.Clone, gPoint)
        _viewSizeRef = _refDrawing.vdCanvas.BaseControl.ActiveDocument.ActiveLayOut.ViewSize

        _compareLayout = DirectCast(_compareDrawing.vdCanvas.BaseControl.ActiveDocument.ActiveLayOut.Clone(_compareDrawing.vdCanvas.BaseControl.ActiveDocument), vdPrimaries.vdLayout)
        _viewCenterCompare = CType(_compareDrawing.vdCanvas.BaseControl.ActiveDocument.ActiveLayOut.ViewCenter.Clone, gPoint)
        _viewSizeCompare = _compareDrawing.vdCanvas.BaseControl.ActiveDocument.ActiveLayOut.ViewSize

        Me.bwCompare.ReportProgress(100, "clone")

        CompareDocuments(e)
    End Sub

    Private Sub bwCompare_ProgressChanged(sender As Object, e As System.ComponentModel.ProgressChangedEventArgs) Handles bwCompare.ProgressChanged
        If e.UserState IsNot Nothing AndAlso TypeOf e.UserState Is String AndAlso CType(e.UserState, String) = "clone" Then
            If e.ProgressPercentage = 0 Then
                ProgressBar1.Enabled = True
                ProgressBar1.Visible = True
            Else
                ProgressBar1.Visible = False
                ProgressBar1.Enabled = False
            End If
        Else
            If (e.ProgressPercentage > 0 AndAlso e.ProgressPercentage < 100) Then
                Me.upbCompare.Value = e.ProgressPercentage
            End If
        End If
    End Sub

    Private Sub bwCompare_RunWorkerCompleted(sender As Object, e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles bwCompare.RunWorkerCompleted
        If (e.Error IsNot Nothing) Then
            MessageBoxEx.ShowError(String.Format(GraphicalCompareFormStrings.ErrorCompare_Msg, e.Error.Message))
        End If

        If Not e.Cancelled AndAlso e.Error Is Nothing Then
            With Me.ugResults
                .BeginUpdate()
                .DataSource = Me.udsResults

                If (Me.ugResults.Rows.Count <> 0) Then
                    .Selected.Rows.Add(Me.ugResults.Rows(0))
                End If

                .EndUpdate()
            End With

            Me.upnResult.Visible = True

            Dim highlightRects As New List(Of VdRectEx)

            For Each row As UltraGridRow In Me.ugResults.Rows
                Dim refGroup As VdSVGGroup = If(DirectCast(DirectCast(row.ListObject, UltraDataRow).Tag, Tuple(Of VdSVGGroup, VdSVGGroup)).Item1, DirectCast(DirectCast(row.ListObject, UltraDataRow).Tag, Tuple(Of VdSVGGroup, VdSVGGroup)).Item2)
                If (refGroup IsNot Nothing) Then
                    highlightRects.Add(GetHighlightRect(_refDrawing.vdCanvas.BaseControl.ActiveDocument, refGroup.BoundingBox.Height, refGroup.Id, refGroup.BoundingBox.Min, If(CBool(row.Cells(NameOf(GraphicalCompareFormStrings.Checked_ColumnCaption)).Value), Color.Green, Color.Orange), 2, refGroup.BoundingBox.Width))
                End If
            Next

            _refDrawing.HighlightRects = highlightRects
            _refDrawing.vdCanvas.BaseControl.ActiveDocument.Invalidate()
        End If

        _compareRunning = False
        _previousResultInformation = New Dictionary(Of Integer, Tuple(Of Boolean, Boolean, String))
        _undoActionTriggered = False

        Me.btnClose.Enabled = True
        Me.btnCompare.Enabled = False
        Me.btnCompare.Text = GraphicalCompareFormStrings.Compare_Text
        Me.btnExport.Enabled = True
        Me.btnResetAreaFilter.Visible = Me.ugResults.Rows.Count <> 0
        Me.btnResetAreaFilter.Enabled = False
        Me.btnSave.Enabled = True
        Me.chkConsiderText.Enabled = True

        Me.lblLegend1.Visible = True
        Me.lblLegend2.Visible = True
        Me.lblLegend3.Visible = True
        Me.lblRowCountInfo.Visible = True

        Me.uceCompareDocument.ReadOnly = False
        Me.uceCompareDrw.ReadOnly = False
        Me.uceReferenceDrw.ReadOnly = False
        Me.uckDisplayIndicators.Visible = True
        Me.uckFilterDynamically.Visible = True
        Me.uddbSetAreaFilter.Visible = Me.ugResults.Rows.Count <> 0
        Me.upbCompare.Visible = False

        Me.Cursor = Cursors.Default
    End Sub

    Private Sub picCompResult_MouseDown(sender As Object, e As MouseEventArgs) Handles picCompResult.MouseDown
        Me.picCompResult.Focus()

        If (e.Button = System.Windows.Forms.MouseButtons.Left) Then
            SelectFromView(_compareLayout, False, picCompResult, _viewCenterCompare, e.X, e.Y)

        ElseIf (e.Button = System.Windows.Forms.MouseButtons.Middle) AndAlso (Not _isPanning) Then
            Me.Cursor = Cursors.SizeAll

            Dim pt As System.Drawing.Point = e.Location
            pt.X = CInt(pt.X * GetAppliedCompareRatio())
            pt.Y = CInt(pt.Y * GetAppliedCompareRatio())
            _clickPoint = _compareLayout.ActiveRender.Pixel2View(pt)
            _isPanning = True

        ElseIf (e.Button = System.Windows.Forms.MouseButtons.Right) Then
            If (SelectFromView(_compareLayout, False, picCompResult, _viewCenterCompare, e.X, e.Y)) Then
                _contextMenuGrid.Tools("Check").InstanceProps.Visible = Infragistics.Win.DefaultableBoolean.True
                _contextMenuGrid.Tools("CheckAll").InstanceProps.Visible = Infragistics.Win.DefaultableBoolean.True
                _contextMenuGrid.Tools("Uncheck").InstanceProps.Visible = Infragistics.Win.DefaultableBoolean.True
                _contextMenuGrid.Tools("UncheckAll").InstanceProps.Visible = Infragistics.Win.DefaultableBoolean.True

                _contextMenuGrid.Tools("Undo").InstanceProps.Visible = Infragistics.Win.DefaultableBoolean.False
                _contextMenuGrid.Tools("Redo").InstanceProps.Visible = Infragistics.Win.DefaultableBoolean.False

                _contextMenuGrid.ShowPopup()
            End If
        End If
    End Sub

    Private Function GetAppliedCompareRatio() As Double
        Dim vr As Double = _compareLayout.DisplayHeight / picCompResult.Height
        Dim hr As Double = _compareLayout.DisplayWidth / picCompResult.Width
        Return Math.Max(hr, vr)
    End Function

    Private Function GetAppliedReferenceRatio() As Double
        Dim vr As Double = _refLayout.DisplayHeight / picRefResult.Height
        Dim hr As Double = _refLayout.DisplayWidth / picRefResult.Width
        Return Math.Max(hr, vr)
    End Function

    Private Sub picCompResult_MouseMove(sender As Object, e As MouseEventArgs) Handles picCompResult.MouseMove
        Me.lblRowCountInfo.Text = String.Format(GraphicalCompareFormStrings.RowCount_Label, Me.ugResults.Rows.VisibleRowCount, Me.ugResults.Rows.Count)

        If (_isPanning) AndAlso (Me.ugResults.Rows.Count <> 0) Then
            Dim pt As System.Drawing.Point = e.Location
            pt.X = CInt(pt.X * GetAppliedCompareRatio())
            pt.Y = CInt(pt.Y * GetAppliedCompareRatio())
            Dim panVector As gPoint = New gPoint(_clickPoint - _compareLayout.ActiveRender.Pixel2View(pt))

            _viewCenterCompare += panVector
            _viewCenterRef += panVector

            ShowGraphics(True)
        End If
    End Sub

    Private Sub picCompResult_MouseUp(sender As Object, e As MouseEventArgs) Handles picCompResult.MouseUp
        Me.Cursor = Cursors.Default
        _isPanning = False
    End Sub

    Private Sub picCompResult_MouseWheel(sender As Object, e As MouseEventArgs) Handles picCompResult.MouseWheel
        Zoom(e)
    End Sub

    Private Sub picRefResult_MouseDown(sender As Object, e As MouseEventArgs) Handles picRefResult.MouseDown
        Me.picRefResult.Focus()

        If (e.Button = System.Windows.Forms.MouseButtons.Left) Then
            SelectFromView(_refLayout, True, picRefResult, _viewCenterRef, e.X, e.Y)

        ElseIf (e.Button = System.Windows.Forms.MouseButtons.Middle) AndAlso (Not _isPanning) Then
            Me.Cursor = Cursors.SizeAll

            Dim pt As System.Drawing.Point = e.Location
            pt.X = CInt(pt.X * GetAppliedReferenceRatio())
            pt.Y = CInt(pt.Y * GetAppliedReferenceRatio())
            _clickPoint = _refLayout.ActiveRender.Pixel2View(pt)
            _isPanning = True

        ElseIf (e.Button = System.Windows.Forms.MouseButtons.Right) Then
            If (SelectFromView(_refLayout, True, picRefResult, _viewCenterRef, e.X, e.Y)) Then
                _contextMenuGrid.Tools("Check").InstanceProps.Visible = Infragistics.Win.DefaultableBoolean.True
                _contextMenuGrid.Tools("CheckAll").InstanceProps.Visible = Infragistics.Win.DefaultableBoolean.True
                _contextMenuGrid.Tools("Uncheck").InstanceProps.Visible = Infragistics.Win.DefaultableBoolean.True
                _contextMenuGrid.Tools("UncheckAll").InstanceProps.Visible = Infragistics.Win.DefaultableBoolean.True

                _contextMenuGrid.Tools("Undo").InstanceProps.Visible = Infragistics.Win.DefaultableBoolean.False
                _contextMenuGrid.Tools("Redo").InstanceProps.Visible = Infragistics.Win.DefaultableBoolean.False

                _contextMenuGrid.ShowPopup()
            End If

        End If
    End Sub

    Private Sub picRefResult_MouseMove(sender As Object, e As MouseEventArgs) Handles picRefResult.MouseMove
        Me.lblRowCountInfo.Text = String.Format(GraphicalCompareFormStrings.RowCount_Label, Me.ugResults.Rows.VisibleRowCount, Me.ugResults.Rows.Count)

        If (_isPanning) AndAlso (Me.ugResults.Rows.Count <> 0) Then
            Dim pt As System.Drawing.Point = e.Location
            pt.X = CInt(pt.X * GetAppliedReferenceRatio())
            pt.Y = CInt(pt.Y * GetAppliedReferenceRatio())
            Dim panVector As gPoint = New gPoint(_clickPoint - _refLayout.ActiveRender.Pixel2View(pt))

            _viewCenterCompare += panVector
            _viewCenterRef += panVector

            ShowGraphics(True)
        End If
    End Sub

    Private Sub picRefResult_MouseUp(sender As Object, e As MouseEventArgs) Handles picRefResult.MouseUp
        Me.Cursor = Cursors.Default
        _isPanning = False
    End Sub

    Private Sub picRefResult_MouseWheel(sender As Object, e As MouseEventArgs) Handles picRefResult.MouseWheel
        Zoom(e)
    End Sub

    Private Sub Zoom(e As MouseEventArgs)
        Const MIN_SIZE As Integer = 20
        Const MAX_SIZE As Integer = 1000
        Const ZOOM_INC As Double = 0.2

        If (Me.ugResults.Rows.Count <> 0) Then
            If (e.Delta < 0) Then
                _viewSizeCompare *= (1 + ZOOM_INC)
                _viewSizeRef *= (1 + ZOOM_INC)

            ElseIf (e.Delta > 0) Then
                _viewSizeCompare *= (1 - ZOOM_INC)
                _viewSizeRef *= (1 - ZOOM_INC)

            End If

            If (_viewSizeCompare < MIN_SIZE) Then _viewSizeCompare = MIN_SIZE
            If (_viewSizeRef < MIN_SIZE) Then _viewSizeRef = MIN_SIZE

            If (_viewSizeCompare > MAX_SIZE) Then _viewSizeCompare = MAX_SIZE
            If (_viewSizeRef > MAX_SIZE) Then _viewSizeRef = MAX_SIZE

            ShowGraphics(True)

        End If
    End Sub

    Private Sub txtComment_EditorButtonClick(sender As Object, e As Infragistics.Win.UltraWinEditors.EditorButtonEventArgs) Handles txtComment.EditorButtonClick
        Using editCommentForm As New EditCommentForm(Me.txtComment.Text)
            editCommentForm.ShowDialog(Me)

            Me.txtComment.Text = editCommentForm.txtComment.Text
        End Using
    End Sub

    Private Sub txtComment_KeyDown(sender As Object, e As KeyEventArgs) Handles txtComment.KeyDown
        If (e.KeyCode = Keys.Enter) Then
            btnNext_Click(sender, e)

            Me.txtComment.Focus()
        End If
    End Sub

    Private Sub txtComment_TextChanged(sender As Object, e As EventArgs) Handles txtComment.TextChanged
        Dim toolTipInfo As UltraToolTipInfo = Me.uttmCompare.GetUltraToolTip(Me.txtComment)
        toolTipInfo.ToolTipText = Me.txtComment.Text
    End Sub

    Private Sub txtReferenceDocument_BeforeEnterEditMode(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles txtReferenceDocument.BeforeEnterEditMode
        e.Cancel = True
    End Sub

    'Private Sub uceCompareDrw_SelectionChanged(sender As Object, e As EventArgs) Handles uceCompareDrw.SelectionChanged
    '    If (Me.uceCompareDrw.SelectedItem IsNot Nothing) Then
    '        ClearHighlightRects()
    '        _compareSVGFile = Me.uceCompareDrw.SelectedItem.DataValue.ToString
    '        _compareDrawing = DirectCast(Me.uceCompareDrw.SelectedItem.Tag, DrawingCanvas)
    '        _compareLayout = DirectCast(_compareDrawing.vdCanvas.BaseControl.ActiveDocument.ActiveLayOut.Clone(_compareDrawing.vdCanvas.BaseControl.ActiveDocument), vdPrimaries.vdLayout)

    '        _viewCenterCompare = CType(_compareLayout.ViewCenter.Clone, gPoint)
    '        _viewSizeCompare = _compareLayout.ViewSize

    '        If (_refDrawing IsNot Nothing) Then
    '            _refLayout = DirectCast(_refDrawing.vdCanvas.BaseControl.ActiveDocument.ActiveLayOut.Clone(_refDrawing.vdCanvas.BaseControl.ActiveDocument), vdPrimaries.vdLayout)
    '            _viewCenterRef = CType(_refLayout.ViewCenter.Clone, gPoint)
    '            _viewSizeRef = _refLayout.ViewSize
    '        End If


    '        CalculateDrawingOffset()

    '        If (_refDrawing IsNot Nothing) Then
    '            Me.btnCompare.Enabled = True
    '        End If
    '    End If
    'End Sub

    Private Sub uceCompareDocument_SelectionChanged(sender As Object, e As EventArgs) Handles uceCompareDocument.SelectionChanged
        If (Me.uceCompareDocument.SelectedItem IsNot Nothing) AndAlso (Me.uceCompareDocument.SelectedItem.DataValue IsNot Nothing) Then
            _compareDocument = DirectCast(Me.uceCompareDocument.SelectedItem.DataValue, DocumentForm)
            _compareDrawing = Nothing

            Me.uceCompareDrw.Items.Clear()

            Dim compareDrawings As Dictionary(Of String, DrawingCanvas) = GetOpenDrawingsFromDocument(_compareDocument)
            If (compareDrawings.Count <> 0) Then
                For Each compareDrawing As KeyValuePair(Of String, DrawingCanvas) In compareDrawings
                    Me.uceCompareDrw.Items.Add(compareDrawing.Key, IO.Path.GetFileNameWithoutExtension(compareDrawing.Key)).Tag = compareDrawing.Value
                Next

                Me.uceCompareDrw.SortStyle = Infragistics.Win.ValueListSortStyle.Ascending
                Me.uceCompareDrw.SelectedItem = Me.uceCompareDrw.Items(0)
            End If

            Dim toolTipInfo As UltraToolTipInfo = Me.uttmCompare.GetUltraToolTip(Me.uceCompareDocument)
            toolTipInfo.ToolTipText = Me.uceCompareDocument.SelectedItem.DisplayText
        End If
    End Sub

    'Private Sub uceReferenceDrw_SelectionChanged(sender As Object, e As EventArgs) Handles uceReferenceDrw.SelectionChanged
    '    If (Me.uceReferenceDrw.SelectedItem IsNot Nothing) Then
    '        ClearHighlightRects()
    '        _refSVGFile = Me.uceReferenceDrw.SelectedItem.DataValue.ToString
    '        _refDrawing = DirectCast(Me.uceReferenceDrw.SelectedItem.Tag, DrawingCanvas)
    '        _refLayout = DirectCast(_refDrawing.vdCanvas.BaseControl.ActiveDocument.ActiveLayOut.Clone(_refDrawing.vdCanvas.BaseControl.ActiveDocument), vdPrimaries.vdLayout)

    '        _viewCenterRef = CType(_refLayout.ViewCenter.Clone, gPoint)
    '        _viewSizeRef = _refLayout.ViewSize

    '        If (_compareDrawing IsNot Nothing) Then
    '            _compareLayout = DirectCast(_compareDrawing.vdCanvas.BaseControl.ActiveDocument.ActiveLayOut.Clone(_compareDrawing.vdCanvas.BaseControl.ActiveDocument), vdPrimaries.vdLayout)
    '            _viewCenterCompare = CType(_compareLayout.ViewCenter.Clone, gPoint)
    '            _viewSizeCompare = _compareLayout.ViewSize
    '        End If

    '        CalculateDrawingOffset()

    '        If (_compareDrawing IsNot Nothing) Then
    '            Me.btnCompare.Enabled = True
    '        End If
    '    End If
    'End Sub
    Private Sub uceCompareDrw_SelectionChanged(sender As Object, e As EventArgs) Handles uceCompareDrw.SelectionChanged
        If (Me.uceCompareDrw.SelectedItem IsNot Nothing) Then
            ClearHighlightRects()
            _compareSVGFile = Me.uceCompareDrw.SelectedItem.DataValue.ToString
            _compareDrawing = DirectCast(Me.uceCompareDrw.SelectedItem.Tag, DrawingCanvas)

            CalculateDrawingOffset()

            If (_refDrawing IsNot Nothing) Then
                Me.btnCompare.Enabled = True
            End If
        End If
    End Sub
    Private Sub uceReferenceDrw_SelectionChanged(sender As Object, e As EventArgs) Handles uceReferenceDrw.SelectionChanged
        If (Me.uceReferenceDrw.SelectedItem IsNot Nothing) Then
            ClearHighlightRects()
            _refSVGFile = Me.uceReferenceDrw.SelectedItem.DataValue.ToString
            _refDrawing = DirectCast(Me.uceReferenceDrw.SelectedItem.Tag, DrawingCanvas)

            CalculateDrawingOffset()

            If (_compareDrawing IsNot Nothing) Then
                Me.btnCompare.Enabled = True
            End If
        End If
    End Sub

    Private Sub uckDisplayIndicators_CheckedValueChanged(sender As Object, e As EventArgs) Handles uckDisplayIndicators.CheckedValueChanged
        If (_compareLayout IsNot Nothing) AndAlso (_compareLayout.Document.Layers.FindName(LAYER_CHANGE_INDICATORS) IsNot Nothing) AndAlso (_refLayout IsNot Nothing) AndAlso (_refLayout.Document.Layers.FindName(LAYER_CHANGE_INDICATORS) IsNot Nothing) Then
            _compareLayout.Document.Layers.FindName(LAYER_CHANGE_INDICATORS).Frozen = Not Me.uckDisplayIndicators.Checked
            _refLayout.Document.Layers.FindName(LAYER_CHANGE_INDICATORS).Frozen = Not Me.uckDisplayIndicators.Checked

            ShowGraphics(True)
        End If
    End Sub

    Private Sub uckFilterDynamically_CheckedValueChanged(sender As Object, e As EventArgs) Handles uckFilterDynamically.CheckedValueChanged
        If (Me.uckFilterDynamically.Checked) Then
            FilterResultGridRowsDynamically()
        Else
            Me.ugResults.BeginUpdate()

            For Each row As UltraGridRow In Me.ugResults.Rows
                If (Not row.IsFilteredOut) Then
                    Dim group As VdSVGGroup = If(DirectCast(DirectCast(row.ListObject, UltraDataRow).Tag, Tuple(Of VdSVGGroup, VdSVGGroup)).Item1, DirectCast(DirectCast(row.ListObject, UltraDataRow).Tag, Tuple(Of VdSVGGroup, VdSVGGroup)).Item2)
                    If (group IsNot Nothing) Then
                        row.Hidden = group.visibility = vdFigure.VisibilityEnum.Invisible
                    End If
                End If
            Next

            Me.ugResults.EndUpdate()
        End If
    End Sub

    Private Sub ugeeResults_ExportEnded(sender As Object, e As ExcelExport.ExportEndedEventArgs) Handles ugeeResults.ExportEnded
        _exportCancelled = False
        _exportRunning = False

        e.Workbook.Worksheets(0).Rows(0).Cells(0).CellFormat.Font.Bold = ExcelDefaultableBoolean.True

        Me.ugResults.DisplayLayout.Bands(0).Columns(NameOf(GraphicalCompareFormStrings.Comment_ColumnCaption)).Hidden = True
        Me.ugResults.EndUpdate()

        Try
            e.Workbook.Save(_exportFileName)
        Catch ex As Exception
            ex.ShowMessageBox(String.Format(GraphicalCompareFormStrings.ErrorExportExcel_Msg, vbCrLf, ex.Message))
            Return
        End Try

        If (Not e.Canceled) AndAlso (MessageBoxEx.ShowQuestion(GraphicalCompareFormStrings.ExportFinished_Msg) = System.Windows.Forms.DialogResult.Yes) Then
            ProcessEx.Start(_exportFileName)
        End If
    End Sub

    Private Sub ugeeResults_HeaderRowExporting(sender As Object, e As ExcelExport.HeaderRowExportingEventArgs) Handles ugeeResults.HeaderRowExporting
        If (e.CurrentRowIndex > 1) AndAlso (e.CurrentOutlineLevel = 0) Then
            e.Cancel = True
        End If
    End Sub

    Private Sub ugeeResults_InitializeRow(sender As Object, e As ExcelExport.ExcelExportInitializeRowEventArgs) Handles ugeeResults.InitializeRow
        If (_exportCancelled) Then
            e.TerminateExport = True
        End If
    End Sub

    Private Sub ugeeResults_RowExporting(sender As Object, e As ExcelExport.RowExportingEventArgs) Handles ugeeResults.RowExporting
        If (e.GridRow.Cells(NameOf(GraphicalCompareFormStrings.ObjName_ColumnCaption)).Value.ToString = KblObjectType.Undefined.ToLocalizedString) OrElse (e.GridRow.Cells(NameOf(GraphicalCompareFormStrings.ObjType_ColumnCaption)).Value.ToString = KblObjectType.Not_available.ToLocalizedString) Then
            e.Cancel = True
        End If
    End Sub

    Private Sub ugResults_AfterCellUpdate(sender As Object, e As CellEventArgs) Handles ugResults.AfterCellUpdate
        If (e.Cell.Column.Key = NameOf(GraphicalCompareFormStrings.Checked_ColumnCaption)) Then
            Dim compGroup As VdSVGGroup = DirectCast(DirectCast(e.Cell.Row.ListObject, UltraDataRow).Tag, Tuple(Of VdSVGGroup, VdSVGGroup)).Item2
            If (compGroup IsNot Nothing) Then
                UpdateCheckStateInGraphic(CBool(e.Cell.Value), compGroup, _refDrawing.HighlightRects, e.Cell.Row.IsFilteredOut, _compareLayout)
            End If

            Dim refGroup As VdSVGGroup = DirectCast(DirectCast(e.Cell.Row.ListObject, UltraDataRow).Tag, Tuple(Of VdSVGGroup, VdSVGGroup)).Item1
            If (refGroup IsNot Nothing) Then
                UpdateCheckStateInGraphic(CBool(e.Cell.Value), refGroup, _refDrawing.HighlightRects, e.Cell.Row.IsFilteredOut, _refLayout)
            End If

            If (Not Me.ugResults.IsUpdating) Then
                DeselectAll()
            End If
        End If

        If e.Cell.Column.Key = NameOf(GraphicalCompareFormStrings.Checked_ColumnCaption) OrElse e.Cell.Column.Key = NameOf(GraphicalCompareFormStrings.ToBeChanged_ColumnCaption) Then
            _isDirty = True
        End If
    End Sub

    Private Sub ugResults_AfterRowFilterChanged(sender As Object, e As AfterRowFilterChangedEventArgs) Handles ugResults.AfterRowFilterChanged
        Me.Cursor = Cursors.WaitCursor

        For Each row As UltraGridRow In Me.ugResults.Rows
            Dim compGroup As VdSVGGroup = DirectCast(DirectCast(row.ListObject, UltraDataRow).Tag, Tuple(Of VdSVGGroup, VdSVGGroup)).Item2
            If (compGroup IsNot Nothing) Then
                UpdateCheckStateInGraphic(CBool(row.Cells(NameOf(GraphicalCompareFormStrings.Checked_ColumnCaption)).Value), compGroup, _refDrawing.HighlightRects, row.IsFilteredOut, _compareLayout)
            End If

            Dim refGroup As VdSVGGroup = DirectCast(DirectCast(row.ListObject, UltraDataRow).Tag, Tuple(Of VdSVGGroup, VdSVGGroup)).Item1
            If (refGroup IsNot Nothing) Then
                UpdateCheckStateInGraphic(CBool(row.Cells(NameOf(GraphicalCompareFormStrings.Checked_ColumnCaption)).Value), refGroup, _refDrawing.HighlightRects, row.IsFilteredOut, _refLayout)
            End If
        Next

        If (Not Me.ugResults.IsUpdating) Then

            DeselectAll()
        End If

        Me.Cursor = Cursors.Default
    End Sub

    Private Sub ugResults_AfterSelectChange(sender As Object, e As AfterSelectChangeEventArgs) Handles ugResults.AfterSelectChange
        If (e.Type = GetType(UltraGridCell)) Then
            Dim row As UltraGridRow = Me.ugResults.Selected.Cells(0).Row

            Me.ugResults.EventManager.AllEventsEnabled = False
            Me.ugResults.Selected.Cells.Clear()
            Me.ugResults.EventManager.AllEventsEnabled = True

            Me.ugResults.Selected.Rows.Add(row)
        Else
            ShowGraphics(_clickedInView)
        End If

        _clickedInGrid = False
    End Sub

    Private Sub ugResults_BeforeCellUpdate(sender As Object, e As BeforeCellUpdateEventArgs) Handles ugResults.BeforeCellUpdate
        If (e.Cell.Column.Key = NameOf(GraphicalCompareFormStrings.Checked_ColumnCaption)) AndAlso (CBool(e.Cell.Value) <> CBool(e.NewValue)) AndAlso (Not _undoActionRunning) AndAlso (_undoEnabled) Then
            _previousResultInformation = StoreCurrentResultInformation()
            _undoActionTriggered = False
        End If

        If (e.Cell.Column.Key = NameOf(GraphicalCompareFormStrings.Comment_ColumnCaption)) AndAlso (e.Cell.Value.ToString <> e.NewValue.ToString) Then
            _isDirty = True
        End If
    End Sub

    Private Sub ugResults_BeforeSelectChange(sender As Object, e As BeforeSelectChangeEventArgs) Handles ugResults.BeforeSelectChange
        _clickedInGrid = True

        For Each row As UltraGridRow In Me.ugResults.Selected.Rows
            If (Me.txtComment.Text <> String.Empty) Then
                row.Cells(NameOf(GraphicalCompareFormStrings.Checked_ColumnCaption)).Value = True
            End If

            row.Cells(NameOf(GraphicalCompareFormStrings.Comment_ColumnCaption)).Value = Me.txtComment.Text
        Next
    End Sub

    Private Sub ugResults_CellChange(sender As Object, e As CellEventArgs) Handles ugResults.CellChange
        Dim activeColumnKey As String = Me.ugResults.ActiveCell.Column.Key

        If activeColumnKey = NameOf(GraphicalCompareFormStrings.Checked_ColumnCaption) Then
            If (CBool(Me.ugResults.ActiveCell.Value)) Then
                If (MessageBox.Show(GraphicalCompareFormStrings.Uncheck_Msg, [Shared].MSG_BOX_TITLE, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) = System.Windows.Forms.DialogResult.No) Then
                    Me.ugResults.ActiveCell.Value = True
                Else
                    Me.ugResults.ActiveRow.Cells(NameOf(GraphicalCompareFormStrings.ToBeChanged_ColumnCaption)).Value = False
                    Me.ugResults.ActiveRow.Cells(NameOf(GraphicalCompareFormStrings.Comment_ColumnCaption)).Value = String.Empty

                    Me.txtComment.Text = String.Empty
                End If
            End If
        End If

        If activeColumnKey = NameOf(GraphicalCompareFormStrings.ToBeChanged_ColumnCaption) Then
            If (CBool(Me.ugResults.ActiveCell.Value)) Then
                If (MessageBox.Show(GraphicalCompareFormStrings.UncheckToBeChangedFlag_Msg, [Shared].MSG_BOX_TITLE, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) = System.Windows.Forms.DialogResult.No) Then
                    Me.ugResults.ActiveCell.Value = True
                Else
                    Me.ugResults.ActiveRow.Cells(NameOf(GraphicalCompareFormStrings.Checked_ColumnCaption)).Value = False
                End If
            Else
                Me.ugResults.ActiveRow.Cells(NameOf(GraphicalCompareFormStrings.Checked_ColumnCaption)).Value = True
            End If
        End If

        If (Me.ugResults.ActiveRow IsNot Nothing) AndAlso (Not Me.ugResults.ActiveRow.Selected) Then
            Me.ugResults.EventManager.AllEventsEnabled = False
            Me.ugResults.Selected.Rows.Clear()
            Me.ugResults.EventManager.AllEventsEnabled = True

            Me.ugResults.Selected.Rows.Add(Me.ugResults.ActiveRow)
        End If

        If activeColumnKey = NameOf(GraphicalCompareFormStrings.Checked_ColumnCaption) Then
            DeselectAll()
        End If

        If (_checkedCompareResult IsNot Nothing) Then
            _isDirty = True
        End If
    End Sub

    Private Sub ugResults_InitializeLayout(sender As Object, e As InitializeLayoutEventArgs) Handles ugResults.InitializeLayout
        Me.ugResults.BeginUpdate()

        With e.Layout
            .AutoFitStyle = AutoFitStyle.ResizeAllColumns
            .CaptionVisible = Infragistics.Win.DefaultableBoolean.False
            .GroupByBox.Hidden = True
            .LoadStyle = LoadStyle.LoadOnDemand

            With .Override
                .AllowColMoving = AllowColMoving.NotAllowed
                .AllowDelete = Infragistics.Win.DefaultableBoolean.False
                .AllowRowFiltering = Infragistics.Win.DefaultableBoolean.True
                .AllowUpdate = Infragistics.Win.DefaultableBoolean.True
                .RowSelectors = Infragistics.Win.DefaultableBoolean.False
                .SelectTypeCell = SelectType.SingleAutoDrag
                .SelectTypeRow = SelectType.SingleAutoDrag
            End With

            For Each band As UltraGridBand In .Bands
                With band
                    For Each column As UltraGridColumn In .Columns
                        If Not column.Hidden Then
                            column.Header.Caption = GraphicalCompareFormStrings.ResourceManager.GetString(column.Key)
                            With column
                                If .Key = NameOf(GraphicalCompareFormStrings.Checked_ColumnCaption) OrElse .Key = NameOf(GraphicalCompareFormStrings.ToBeChanged_ColumnCaption) Then
                                    .FilterOperandDropDownItems = FilterOperandDropDownItems.All Or FilterOperandDropDownItems.CellValues
                                    .Header.Appearance.TextHAlign = Infragistics.Win.HAlign.Center
                                    .Header.Appearance.TextVAlign = Infragistics.Win.VAlign.Middle
                                    .Style = ColumnStyle.CheckBox
                                    .Width = 105
                                ElseIf .Key = NameOf(GraphicalCompareFormStrings.EntityIndex_ColumnCaption) OrElse .Key = NameOf(GraphicalCompareFormStrings.Comment_ColumnCaption) OrElse .Key = NameOf(GraphicalCompareFormStrings.CompSign_ColumnCaption) OrElse .Key = NameOf(GraphicalCompareFormStrings.RefSign_ColumnCaption) Then
                                    .Hidden = True
                                Else
                                    .CellActivation = Activation.NoEdit
                                    .CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center
                                    .CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle
                                    .Header.Appearance.TextHAlign = Infragistics.Win.HAlign.Center
                                    .Header.Appearance.TextVAlign = Infragistics.Win.VAlign.Middle
                                End If
                            End With
                        End If
                    Next
                End With
            Next
        End With

        Me.ugResults.EndUpdate()
    End Sub

    Private Sub ugResults_InitializeRow(sender As Object, e As InitializeRowEventArgs) Handles ugResults.InitializeRow
        If e.Row.Cells.Exists(NameOf(GraphicalCompareFormStrings.DiffType_ColumnCaption)) AndAlso e.Row.Cells(NameOf(GraphicalCompareFormStrings.DiffType_ColumnCaption)).Value IsNot Nothing Then
            Select Case e.Row.Cells(NameOf(GraphicalCompareFormStrings.DiffType_ColumnCaption)).Value.ToString
                Case GraphicalCompareFormStrings.Added
                    For Each cell As UltraGridCell In e.Row.Cells
                        If (cell.Column.Index > 2) Then
                            cell.Appearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True
                        End If
                    Next
                Case GraphicalCompareFormStrings.Deleted
                    For Each cell As UltraGridCell In e.Row.Cells
                        If (cell.Column.Index > 2) Then
                            cell.Appearance.FontData.Strikeout = Infragistics.Win.DefaultableBoolean.True
                        End If
                    Next
                Case GraphicalCompareFormStrings.Modified
                    For Each cell As UltraGridCell In e.Row.Cells
                        If (cell.Column.Index > 2) Then
                            cell.Appearance.ForeColor = [Shared].CHANGED_MODIFIED_FORECOLOR.ToColor
                        End If
                    Next
            End Select
        End If
    End Sub

    Private Sub ugResults_MouseClick(sender As Object, e As MouseEventArgs) Handles ugResults.MouseClick
        If (e.Button = System.Windows.Forms.MouseButtons.Right) Then
            Dim element As Infragistics.Win.UIElement = Me.ugResults.DisplayLayout.UIElement.LastElementEntered
            _header = TryCast(element.GetContext(GetType(Infragistics.Win.UltraWinGrid.ColumnHeader)), ColumnHeader)

            If (_header IsNot Nothing) AndAlso (_header.Column.Key = NameOf(GraphicalCompareFormStrings.Checked_ColumnCaption) OrElse _header.Column.Key = NameOf(GraphicalCompareFormStrings.ToBeChanged_ColumnCaption)) Then
                _contextMenuGrid.Tools("Check").InstanceProps.Visible = Infragistics.Win.DefaultableBoolean.False
                _contextMenuGrid.Tools("CheckAll").InstanceProps.Visible = Infragistics.Win.DefaultableBoolean.True
                _contextMenuGrid.Tools("Uncheck").InstanceProps.Visible = Infragistics.Win.DefaultableBoolean.False
                _contextMenuGrid.Tools("UncheckAll").InstanceProps.Visible = Infragistics.Win.DefaultableBoolean.True

                _contextMenuGrid.Tools("Undo").InstanceProps.Visible = If(_header.Column.Key = NameOf(GraphicalCompareFormStrings.Checked_ColumnCaption), Infragistics.Win.DefaultableBoolean.True, Infragistics.Win.DefaultableBoolean.False)
                _contextMenuGrid.Tools("Undo").SharedProps.Enabled = Not _undoActionTriggered AndAlso _previousResultInformation.Count <> 0
                _contextMenuGrid.Tools("Redo").InstanceProps.Visible = If(_header.Column.Key = NameOf(GraphicalCompareFormStrings.Checked_ColumnCaption), Infragistics.Win.DefaultableBoolean.True, Infragistics.Win.DefaultableBoolean.False)
                _contextMenuGrid.Tools("Redo").SharedProps.Enabled = _undoActionTriggered AndAlso _previousResultInformation.Count <> 0

                _contextMenuGrid.ShowPopup()
            End If
        End If
    End Sub

    Private Sub ugResults_MouseLeave(sender As Object, e As EventArgs) Handles ugResults.MouseLeave
        _header = Nothing
    End Sub

    Private Sub ugResults_MouseMove(sender As Object, e As MouseEventArgs) Handles ugResults.MouseMove
        Me.lblRowCountInfo.Text = String.Format(GraphicalCompareFormStrings.RowCount_Label, Me.ugResults.Rows.VisibleRowCount, Me.ugResults.Rows.Count)
    End Sub

    Private Sub utmCompare_ToolClick(sender As Object, e As ToolClickEventArgs) Handles utmCompare.ToolClick
        _clickedInGrid = True

        Me.Cursor = Cursors.WaitCursor

        Select Case e.Tool.Key
            Case "Check"
                Me.ugResults.BeginUpdate()

                For Each row As UltraGridRow In Me.ugResults.Selected.Rows
                    If (Not row.Hidden) AndAlso (Not row.IsFilteredOut) Then
                        row.Cells(NameOf(GraphicalCompareFormStrings.Checked_ColumnCaption)).Value = True
                    End If
                Next

                Me.ugResults.EndUpdate()

                DeselectAll()
                ShowGraphics(True)

            Case "CheckAll"
                _previousResultInformation = StoreCurrentResultInformation()
                _undoActionTriggered = False
                _undoEnabled = False

                Me.ugResults.BeginUpdate()

                If (_header IsNot Nothing) Then
                    For Each row As UltraGridRow In Me.ugResults.Rows
                        If (Not row.Hidden) AndAlso (Not row.IsFilteredOut) Then
                            row.Cells(_header.Column.Key).Value = True

                            If (_header.Column.Key = NameOf(GraphicalCompareFormStrings.ToBeChanged_ColumnCaption)) Then
                                row.Cells(NameOf(GraphicalCompareFormStrings.Checked_ColumnCaption)).Value = True
                            End If
                        End If
                    Next
                Else
                    For Each row As UltraGridRow In Me.ugResults.Rows
                        If (Not row.Hidden) AndAlso (Not row.IsFilteredOut) Then
                            row.Cells(NameOf(GraphicalCompareFormStrings.Checked_ColumnCaption)).Value = True
                        End If
                    Next
                End If

                Me.ugResults.EndUpdate()

                DeselectAll()
                ShowGraphics(True)
                _undoEnabled = True

            Case "FromCurrentView"
                SetAreaFilter(_refLayout.ActiveRender.ClipBounds.ToBox, False)

            Case "FromRefDrawing"
                Me.Hide()

                With _refDrawing.vdCanvas.BaseControl
                    Dim boundingBox As Box = Nothing

                    If (.ActiveDocument.ActionUtility.getUserRectViewCS(Nothing, boundingBox) = VectorDraw.Actions.StatusCode.Success) Then
                        SetAreaFilter(boundingBox, True)
                    End If
                    If (Not Me.IsDisposed) Then Me.Show()
                End With

                _refDrawing.vdCanvas.BaseControl.ActiveDocument?.Invalidate()

            Case "Redo"
                _undoActionTriggered = False

                RestorePreviousResultInformation()
            Case "Uncheck"
                Me.ugResults.BeginUpdate()

                If (MessageBox.Show(GraphicalCompareFormStrings.UncheckFromSelected_Msg, [Shared].MSG_BOX_TITLE, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) = System.Windows.Forms.DialogResult.Yes) Then
                    For Each row As UltraGridRow In Me.ugResults.Selected.Rows
                        If (Not row.Hidden) AndAlso (Not row.IsFilteredOut) Then
                            row.Cells(NameOf(GraphicalCompareFormStrings.Checked_ColumnCaption)).Value = False
                            row.Cells(NameOf(GraphicalCompareFormStrings.Comment_ColumnCaption)).Value = String.Empty
                            row.Cells(NameOf(GraphicalCompareFormStrings.ToBeChanged_ColumnCaption)).Value = False
                        End If
                    Next
                End If

                Me.ugResults.EndUpdate()

                DeselectAll()
                ShowGraphics(True)

            Case "UncheckAll"
                Me.ugResults.BeginUpdate()

                If (_header IsNot Nothing) AndAlso (_header.Column.Key <> GraphicalCompareFormStrings.Checked_ColumnCaption OrElse MessageBox.Show(GraphicalCompareFormStrings.UncheckAllVisible_Msg, [Shared].MSG_BOX_TITLE, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) = System.Windows.Forms.DialogResult.Yes) Then
                    _previousResultInformation = StoreCurrentResultInformation()
                    _undoActionTriggered = False
                    _undoEnabled = False

                    For Each row As UltraGridRow In Me.ugResults.Rows
                        If (Not row.Hidden) AndAlso (Not row.IsFilteredOut) Then
                            row.Cells(_header.Column.Key).Value = False

                            If (_header.Column.Key = NameOf(GraphicalCompareFormStrings.Checked_ColumnCaption)) Then
                                row.Cells(NameOf(GraphicalCompareFormStrings.Comment_ColumnCaption)).Value = String.Empty
                                row.Cells(NameOf(GraphicalCompareFormStrings.ToBeChanged_ColumnCaption)).Value = False
                            End If

                        End If
                    Next

                    _undoEnabled = True
                ElseIf (_header Is Nothing) AndAlso (MessageBox.Show(GraphicalCompareFormStrings.UncheckAllVisible_Msg, [Shared].MSG_BOX_TITLE, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) = System.Windows.Forms.DialogResult.Yes) Then
                    _previousResultInformation = StoreCurrentResultInformation()
                    _undoActionTriggered = False
                    _undoEnabled = False

                    For Each row As UltraGridRow In Me.ugResults.Rows
                        If (Not row.Hidden) AndAlso (Not row.IsFilteredOut) Then
                            row.Cells(NameOf(GraphicalCompareFormStrings.Checked_ColumnCaption)).Value = False
                            row.Cells(NameOf(GraphicalCompareFormStrings.Comment_ColumnCaption)).Value = String.Empty
                            row.Cells(NameOf(GraphicalCompareFormStrings.ToBeChanged_ColumnCaption)).Value = False
                        End If
                    Next

                    _undoEnabled = True
                End If

                Me.ugResults.EndUpdate()

                DeselectAll()
                ShowGraphics(True)

            Case "Undo"
                _undoActionTriggered = True
                RestorePreviousResultInformation()

        End Select

        Me.Cursor = Cursors.Default

        _clickedInGrid = False
    End Sub

    Private Sub GraphicalCompareForm_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        Me.chkConsiderText.Checked = My.Settings.GrphCompareConsiderText
    End Sub

    Private Sub GraphicalCompareForm_Closed(sender As Object, e As EventArgs) Handles Me.Closed
        My.Settings.GrphCompareConsiderText = Me.chkConsiderText.Checked
    End Sub

    Private Sub chkConsiderText_CheckedChanged(sender As Object, e As EventArgs) Handles chkConsiderText.CheckedChanged
        If (_compareDrawing IsNot Nothing) Then
            Me.btnCompare.Enabled = True
        End If
    End Sub

End Class