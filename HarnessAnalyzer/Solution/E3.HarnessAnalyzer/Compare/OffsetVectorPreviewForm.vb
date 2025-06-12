Imports Infragistics.Win.UltraWinToolTip

Imports VectorDraw.Geometry
Imports VectorDraw.Professional.vdFigures
Imports VectorDraw.Professional.vdObjects

<Reflection.Obfuscation(Feature:="renaming", ApplyToMembers:=False, Exclude:=True)> ' needed to avoid resource-file-names obfuscation errors, see issue #5
Public Class OffsetVectorPreviewForm

    Private _vertexPositionPairs As Dictionary(Of String, List(Of gPoint))

    Public Sub New(vertexPositionPairs As Dictionary(Of String, List(Of gPoint)))
        InitializeComponent()

        _vertexPositionPairs = vertexPositionPairs

        Me.vDraw.AllowDrop = False
        Me.vDraw.EnsureDocument()
        Me.vDraw.InputDevice_TouchGesture.Actions = VectorDraw.Professional.Actions.TouchGestureActions.Pan Or VectorDraw.Professional.Actions.TouchGestureActions.Zoom
        With Me.vDraw.ActiveDocument
            .UndoHistory.PushEnable(False)

            .DisableZoomOnResize = True
            .EnableAutoGripOn = False
            .EnableUrls = False

            With .GlobalRenderProperties
                .AxisSize = 10
                .CrossSize = 8
                .GridColor = Color.Black
                .SelectingCrossColor = Color.Transparent
                .SelectingWindowColor = Color.Transparent
            End With

            .GridMode = False
            .OrbitActionKey = vdDocument.KeyWithMouseStroke.None
            .OsnapDialogKey = Keys.None
            .Palette.Background = Color.White
            .ShowUCSAxis = False
            .ToolTipDispProps.UseReverseOrder = False
            .UrlActionKey = Keys.None
        End With

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

            Me.uneOffsetVectorX.Value = mostFrequentOffset.Key.SplitRemoveEmpty("|"c)(0)
            Me.uneOffsetVectorY.Value = mostFrequentOffset.Key.SplitRemoveEmpty("|"c)(1)
            Me.lblInfo.Text = String.Format(OffsetVectorPreviewFormStrings.Information_Label, _vertexPositionPairs.Where(Function(pair) pair.Value.Count = 2 AndAlso pair.Value(0) IsNot Nothing).Count, mostFrequentOffset.Value)
        End If

        InitializeView()
    End Sub

    Private Sub InitializeView()
        Dim refBox As New Box
        Dim compBox As New Box

        For Each vertexPositionPair As KeyValuePair(Of String, List(Of gPoint)) In _vertexPositionPairs
            If (vertexPositionPair.Value.Count = 2) AndAlso (vertexPositionPair.Value(0) IsNot Nothing) Then
                Dim offset As New gPoint(Math.Round(vertexPositionPair.Value(0).x - vertexPositionPair.Value(1).x, 2), Math.Round(vertexPositionPair.Value(0).y - vertexPositionPair.Value(1).y, 2))

                Dim line As New vdLine
                With line
                    .SetUnRegisterDocument(Me.vDraw.ActiveDocument)
                    .setDocumentDefaults()

                    .EndPoint = vertexPositionPair.Value(1)
                    .LineType = If(offset.x = CDbl(Me.uneOffsetVectorX.Value) AndAlso offset.y = CDbl(Me.uneOffsetVectorY.Value), Me.vDraw.ActiveDocument.LineTypes.Solid, Me.vDraw.ActiveDocument.LineTypes.DPIDash)
                    .PenColor.SystemColor = If(offset.x = CDbl(Me.uneOffsetVectorX.Value) AndAlso offset.y = CDbl(Me.uneOffsetVectorY.Value), Color.Blue, Color.Black)
                    .StartPoint = vertexPositionPair.Value(0)
                    .XProperties.Add("Tooltip").PropValue = If(offset.x = CDbl(Me.uneOffsetVectorX.Value) AndAlso offset.y = CDbl(Me.uneOffsetVectorY.Value), String.Format(OffsetVectorPreviewFormStrings.OffsetVectorArrow_Tooltip, vertexPositionPair.Key, vbCrLf), String.Format("KBL-Id: '{0}'", vertexPositionPair.Key))
                End With

                Me.vDraw.ActiveDocument.Model.Entities.AddItem(line)

                Dim lineAngle As Double = line.StartPoint.GetAngle(line.EndPoint)
                Dim lineDistance As Double = line.StartPoint.Distance2D(line.EndPoint)

                If (lineDistance > 1) Then
                    Dim pointAtLine As gPoint = line.getPointAtDist(lineDistance * 0.85)
                    Dim firstArrowPoint As gPoint = line.getPointAtDist(lineDistance * 0.86)
                    Dim secondArrowPoint As gPoint = line.getPointAtDist(lineDistance * 0.86)

                    Dim matrix As New Matrix
                    matrix.TranslateMatrix(-1D * pointAtLine)
                    matrix.RotateZMatrix(Globals.DegreesToRadians(90D))
                    matrix.TranslateMatrix(pointAtLine)

                    firstArrowPoint = matrix.Transform(firstArrowPoint)

                    matrix = New Matrix
                    matrix.TranslateMatrix(-1D * pointAtLine)
                    matrix.RotateZMatrix(-Globals.DegreesToRadians(90D))
                    matrix.TranslateMatrix(pointAtLine)

                    secondArrowPoint = matrix.Transform(secondArrowPoint)

                    Dim polyline As New vdPolyline
                    With polyline
                        .SetUnRegisterDocument(Me.vDraw.ActiveDocument)
                        .setDocumentDefaults()

                        .Flag = VectorDraw.Professional.Constants.VdConstPlineFlag.PlFlagCLOSE

                        Dim hatchProperties As New vdHatchProperties(VectorDraw.Professional.Constants.VdConstFill.VdFillModeSolid)
                        hatchProperties.SetUnRegisterDocument(Me.vDraw.ActiveDocument)
                        hatchProperties.FillColor.SystemColor = If(offset.x = CDbl(Me.uneOffsetVectorX.Value) AndAlso offset.y = CDbl(Me.uneOffsetVectorY.Value), Color.Blue, Color.Black)

                        .HatchProperties = hatchProperties
                        .PenColor.SystemColor = If(offset.x = CDbl(Me.uneOffsetVectorX.Value) AndAlso offset.y = CDbl(Me.uneOffsetVectorY.Value), Color.Blue, Color.Black)

                        With .VertexList
                            .Add(line.StartPoint)
                            .Add(line.EndPoint)
                            .Add(firstArrowPoint)
                            .Add(secondArrowPoint)
                            .Add(line.EndPoint)
                        End With

                        .XProperties.Add("Tooltip").PropValue = If(offset.x = CDbl(Me.uneOffsetVectorX.Value) AndAlso offset.y = CDbl(Me.uneOffsetVectorY.Value), String.Format(OffsetVectorPreviewFormStrings.OffsetVectorArrow_Tooltip, vertexPositionPair.Key, vbCrLf), String.Format("KBL-Id: '{0}'", vertexPositionPair.Key))
                    End With

                    Me.vDraw.ActiveDocument.Model.Entities.AddItem(polyline)
                End If
            End If

            For vertexPositionCounter As Integer = 0 To vertexPositionPair.Value.Count - 1
                If (vertexPositionPair.Value(vertexPositionCounter) IsNot Nothing) Then
                    Dim circle As New vdCircle
                    With circle
                        .SetUnRegisterDocument(Me.vDraw.ActiveDocument)
                        .setDocumentDefaults()

                        .Center = vertexPositionPair.Value(vertexPositionCounter)
                        .HatchProperties = New vdHatchProperties(VectorDraw.Professional.Constants.VdConstFill.VdFillModeSolid)
                        .HatchProperties.FillColor.SystemColor = If(vertexPositionCounter = 0, Color.Red, Color.Green)
                        .PenColor.SystemColor = If(vertexPositionCounter = 0, Color.Red, Color.Green)
                        .Radius = 0.5
                        .XProperties.Add("Tooltip").PropValue = String.Format("KBL-Id: '{0}'", vertexPositionPair.Key)
                    End With

                    Me.vDraw.ActiveDocument.Model.Entities.AddItem(circle)

                    If (vertexPositionCounter = 0) Then
                        refBox.AddPoint(vertexPositionPair.Value(vertexPositionCounter))
                    Else
                        compBox.AddPoint(vertexPositionPair.Value(vertexPositionCounter))
                    End If
                End If
            Next
        Next

        Dim rect As New vdRect
        With rect
            .SetUnRegisterDocument(Me.vDraw.ActiveDocument)
            .setDocumentDefaults()

            .Height = refBox.Height
            .InsertionPoint = refBox.Min
            .PenColor.SystemColor = Color.Red
            .Width = refBox.Width
            .XProperties.Add("Tooltip").PropValue = OffsetVectorPreviewFormStrings.RefDrwBoundsBox_Tooltip
        End With

        Me.vDraw.ActiveDocument.Model.Entities.AddItem(rect)

        rect = New vdRect
        With rect
            .SetUnRegisterDocument(Me.vDraw.ActiveDocument)
            .setDocumentDefaults()

            .Height = compBox.Height
            .InsertionPoint = compBox.Min
            .PenColor.SystemColor = Color.Green
            .Width = compBox.Width
            .XProperties.Add("Tooltip").PropValue = OffsetVectorPreviewFormStrings.CompDrwBoundsBox_Tooltip
        End With

        Me.vDraw.ActiveDocument.Model.Entities.AddItem(rect)
        Me.vDraw.ActiveDocument.ZoomExtents()
        Me.vDraw.ActiveDocument.Invalidate()
    End Sub

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Me.Close()
    End Sub

    Private Sub vDraw_MouseMove(sender As Object, e As MouseEventArgs) Handles vDraw.MouseMove
        Static mpos As System.Drawing.Point
        If Not (mpos.X - e.Location.X = 0 AndAlso mpos.Y - e.Location.Y = 0) Then


            Dim currentCursorPoint As gPoint = Me.vDraw.ActiveDocument.CCS_CursorPos
            Dim hitEntity As VectorDraw.Professional.vdPrimaries.vdFigure = Me.vDraw.ActiveDocument.ActiveLayOut.GetEntityFromPoint(Me.vDraw.ActiveDocument.ActiveRender.View2PixelI(currentCursorPoint), 6, False, VectorDraw.Professional.vdObjects.vdDocument.LockLayerMethodEnum.DisableAll)

            Me.uttmOffsetVectorPreview.HideToolTip()

            If (hitEntity IsNot Nothing) AndAlso (hitEntity.XProperties.FindName("Tooltip") IsNot Nothing) AndAlso (hitEntity.XProperties.FindName("Tooltip").PropValue IsNot Nothing) Then
                Dim ttInfo As UltraToolTipInfo = Me.uttmOffsetVectorPreview.GetUltraToolTip(Me.vDraw)
                ttInfo.ToolTipText = hitEntity.XProperties.FindName("Tooltip").PropValue.ToString
                ttInfo.ToolTipImage = Infragistics.Win.ToolTipImage.Info
                ttInfo.ToolTipTitle = BundleCrossSectionFormStrings.BundlePic_Tooltip
                ttInfo.Enabled = Infragistics.Win.DefaultableBoolean.False

                Me.uttmOffsetVectorPreview.ShowToolTip(Me.vDraw)
            End If
        End If
        mpos = e.Location
    End Sub

End Class