Imports System.IO
Imports System.Text.RegularExpressions

Imports VectorDraw.Geometry
Imports VectorDraw.Professional.Constants
Imports VectorDraw.Professional.vdCollections
Imports VectorDraw.Professional.vdFigures
Imports VectorDraw.Professional.vdObjects
Imports VectorDraw.Professional.vdPrimaries
Imports Zuken.E3.HarnessAnalyzer.Settings

<Reflection.Obfuscation(Feature:="renaming", ApplyToMembers:=False, Exclude:=True)> ' needed to avoid resource-file-names obfuscation errors, see issue #5
Public Class EntireRoutingPathForm

    Friend Event EntireRoutingPathViewMouseClick(sender As Object, e As InformationHubEventArgs)

    Private _currentXPos As Double = 0
    Private _entireRoutingPath As Dictionary(Of String, RoutingPath)
    Private _harnessXPos As Double = 0
    Private _inlinerPairCheckClassifications As InlinerPairCheckClassificationList
    Private _transformedConnectorGroups As List(Of VdSVGGroup)

    Private Const CONNECTOR_GROUP_YOFFSET As Double = 10
    Private Const HARNESS_RECT_OFFSET As Double = 10
    Private WithEvents _wStateDetector As New WindowStateChangedDetector(Me)

    Public Sub New(entireRoutingPath As Dictionary(Of String, RoutingPath), inlinerPairCheckClassifications As InlinerPairCheckClassificationList)
        InitializeComponent()

        _entireRoutingPath = entireRoutingPath
        _inlinerPairCheckClassifications = inlinerPairCheckClassifications
        _transformedConnectorGroups = New List(Of VdSVGGroup)

        Me.BackColor = Color.White

        Me.vDraw.AllowDrop = False
        Me.vDraw.EnsureDocument()
        Me.vDraw.InputDevice_TouchGesture.Actions = VectorDraw.Professional.Actions.TouchGestureActions.Pan Or VectorDraw.Professional.Actions.TouchGestureActions.Zoom

        With Me.vDraw.ActiveDocument
            .UndoHistory.PushEnable(False)

            .Blocks.AddFromFile("DummyCon.vdcl", False)
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
            .UrlActionKey = Keys.None
        End With
    End Sub


    Friend Sub AddConnectorGroups(drawingCanvas As DrawingCanvas, kblMapper As KblMapper, routingPath As RoutingPath)
        For Each connection As RoutingConnection In routingPath.Connections.Values
            Dim connectors As New Dictionary(Of String, Connector_occurrence)
            connectors.Add(connection.ConnectorA.SystemId, connection.ConnectorA)
            connectors.Add(connection.ConnectorB.SystemId, connection.ConnectorB)

            For Each connector As KeyValuePair(Of String, Connector_occurrence) In connectors
                Dim connectorTableAdded As Boolean = False

                If (drawingCanvas IsNot Nothing) AndAlso (drawingCanvas.GroupMapper.ContainsKey(connector.Key)) Then
                    For Each group As VdSVGGroup In drawingCanvas.GroupMapper(connector.Key).Values.ToList
                        If (group.visibility = vdFigure.VisibilityEnum.Invisible) OrElse (group.SVGType = SvgType.dimension.ToString) OrElse (group.SVGType = SvgType.ref.ToString) Then Continue For
                        If (group.SVGType = SvgType.table.ToString) Then
                            If (connectorTableAdded) OrElse (Not ContainsConnectorTableGroupHighlightingWireRow_Recursively(group, If(connection.Core IsNot Nothing, connection.Core.SystemId, connection.Wire.SystemId))) Then
                                Continue For
                            Else
                                connectorTableAdded = True
                            End If
                        End If

                        Dim clonedGroup As VdSVGGroup = DirectCast(group.Clone(Me.vDraw.ActiveDocument), VdSVGGroup)
                        clonedGroup.XProperties.Add("HarnessPartNumber").PropValue = routingPath.Harness.Part_number
                        clonedGroup.XProperties.Add("ShortName").PropValue = connector.Value.Id

                        connection.ConnectorSymbols.Add(clonedGroup)
                    Next
                Else
                    Dim modules As String = String.Empty
                    Dim wires As New List(Of String)

                    If (kblMapper.KBLObjectModuleMapper.ContainsKey(connector.Key)) Then
                        For Each moduleId As String In kblMapper.KBLObjectModuleMapper(connector.Key)
                            With DirectCast(kblMapper.KBLOccurrenceMapper(moduleId), [Module])
                                If (modules = String.Empty) Then
                                    If (.Abbreviation <> String.Empty) Then
                                        modules = DirectCast(kblMapper.KBLOccurrenceMapper(moduleId), [Module]).Abbreviation
                                    Else
                                        modules = DirectCast(kblMapper.KBLOccurrenceMapper(moduleId), [Module]).Part_number
                                    End If
                                Else
                                    If (.Abbreviation <> String.Empty) Then
                                        modules = String.Format("{0}, {1}", modules, DirectCast(kblMapper.KBLOccurrenceMapper(moduleId), [Module]).Abbreviation)
                                    Else
                                        modules = String.Format("{0}, {1}", modules, DirectCast(kblMapper.KBLOccurrenceMapper(moduleId), [Module]).Part_number)
                                    End If
                                End If
                            End With
                        Next
                    Else
                        modules = KblObjectType.Not_available.ToLocalizedString
                    End If

                    For Each contactPoint As Contact_point In connector.Value.Contact_points
                        If (kblMapper.KBLContactPointWireMapper.ContainsKey(contactPoint.SystemId)) Then
                            For Each wireId As String In kblMapper.KBLContactPointWireMapper(contactPoint.SystemId)
                                If (Not wires.Contains(wireId)) Then wires.Add(wireId)
                            Next
                        End If
                    Next

                    Dim insert As New vdInsert
                    With insert
                        .SetUnRegisterDocument(Me.vDraw.ActiveDocument)
                        .setDocumentDefaults()

                        .Block = Me.vDraw.ActiveDocument.Blocks.FindName("DummyCon")

                        If (.Block Is Nothing) Then .Block = Me.vDraw.ActiveDocument.Blocks.FindName("VDDIM_DEFAULT")

                        .CreateDefaultAttributes()

                        If (.Attributes.FindTagName("Abbreviation") IsNot Nothing) AndAlso (kblMapper.KBLPartMapper.ContainsKey(connector.Value.Part)) Then .Attributes.FindTagName("Abbreviation").ValueString = DirectCast(kblMapper.KBLPartMapper(connector.Value.Part), Part).Abbreviation
                        If (.Attributes.FindTagName("CavityCount") IsNot Nothing) Then .Attributes.FindTagName("CavityCount").ValueString = If(connector.Value.Slots IsNot Nothing AndAlso connector.Value.Slots.Length <> 0, connector.Value.Slots(0).Cavities.Length.ToString, "0")
                        If (.Attributes.FindTagName("Description") IsNot Nothing) Then .Attributes.FindTagName("Description").ValueString = connector.Value.Description
                        If (.Attributes.FindTagName("PartNumber") IsNot Nothing) AndAlso (kblMapper.KBLPartMapper.ContainsKey(connector.Value.Part)) Then .Attributes.FindTagName("PartNumber").ValueString = DirectCast(kblMapper.KBLPartMapper(connector.Value.Part), Part).Part_number
                        If (.Attributes.FindTagName("Modules") IsNot Nothing) Then .Attributes.FindTagName("Modules").ValueString = modules
                        If (.Attributes.FindTagName("WireCount") IsNot Nothing) Then .Attributes.FindTagName("WireCount").ValueString = wires.Count.ToString

                        .XProperties.Add("HarnessPartNumber").PropValue = routingPath.Harness.Part_number
                        .XProperties.Add("ShortName").PropValue = connector.Value.Id
                    End With

                    connection.ConnectorSymbols.Add(insert)
                End If
            Next
        Next
    End Sub


    Private Sub AddInformationBubbleImage(insertionPoint As gPoint)
        Dim image As New vdImage
        With image
            .SetUnRegisterDocument(Me.vDraw.ActiveDocument)
            .setDocumentDefaults()

            .Height = 10

            Dim imageDefinition As New vdImageDef
            With imageDefinition
                .SetUnRegisterDocument(Me.vDraw.ActiveDocument)
                .setDocumentDefaults()

                .Image.Image = My.Resources.About_Large
            End With

            .ImageDefinition = imageDefinition
            .InsertionPoint = insertionPoint
            .PenColor.SystemColor = Color.Transparent
            .Width = 10
        End With

        Me.vDraw.ActiveDocument.ActiveLayOut.Entities.AddItem(image)
    End Sub

    Private Sub AddWarningRectangleImage(insertionPoint As gPoint, imageName As String)
        Dim image As New vdImage
        With image
            .SetUnRegisterDocument(Me.vDraw.ActiveDocument)
            .setDocumentDefaults()

            .Height = 10

            Dim imageDefinition As New vdImageDef
            With imageDefinition
                .SetUnRegisterDocument(Me.vDraw.ActiveDocument)
                .setDocumentDefaults()

                .Image.Image = DirectCast(My.Resources.ResourceManager.GetObject(imageName), Drawing.Bitmap)
            End With

            .ImageDefinition = imageDefinition
            .InsertionPoint = insertionPoint
            .PenColor.SystemColor = Color.Transparent
            .Width = 10
        End With

        Me.vDraw.ActiveDocument.ActiveLayOut.Entities.AddItem(image)
    End Sub

    Private Function ContainsConnectorTableGroupHighlightingWireRow_Recursively(group As VdSVGGroup, wireId As String) As Boolean
        If (group.KblId = wireId AndAlso group.SVGType = SvgType.row.ToString) OrElse (group.SecondaryKblIds.Contains(wireId) AndAlso group.SVGType = SvgType.row.ToString) Then Return True

        Dim retVal As Boolean = False

        For Each childGroup As VdSVGGroup In group.ChildGroups
            If (ContainsConnectorTableGroupHighlightingWireRow_Recursively(childGroup, wireId)) Then
                retVal = True

                Exit For
            End If
        Next

        Return retVal
    End Function

    Private Sub GenerateDrawing(initializing As Boolean)
        Me.vDraw.ActiveDocument.ActiveLayOut.Entities.RemoveAll()
        Me.vDraw.ActiveDocument.Purge()

        Dim startRoutingPath As RoutingPath = Nothing

        GetStartRoutingPath_Recursively(_entireRoutingPath.Values.Where(Function(routingPath) routingPath.IsOriginHarness)(0).Connections, startRoutingPath)

        If (startRoutingPath IsNot Nothing) Then
            _currentXPos = 0
            _harnessXPos = 0

            GenerateRoutingPathDrawing_Recursively(startRoutingPath)
        End If

        If (initializing) Then
            Me.vDraw.ActiveDocument.ZoomExtents()
        End If

        Me.vDraw.ActiveDocument.Redraw(True)
    End Sub

    Private Sub GeneratePartialHarnessDrawing(ByRef connectorHeight As Double, connectorWidth As Double, connectorSymbol As vdFigure, ByRef harnessWidth As Double, routingPath As RoutingPath, shortNameText As vdText)
        If (shortNameText.BoundingBox.Width > connectorWidth) Then
            _currentXPos += shortNameText.BoundingBox.Width
            harnessWidth += shortNameText.BoundingBox.Width
        Else
            _currentXPos += connectorWidth
            harnessWidth += connectorWidth
        End If

        Dim cableRect As vdRect = Nothing
        Dim cableText As vdText = Nothing
        Dim wireArrow As vdInsert = Nothing
        Dim wireLine As vdLine = Nothing
        Dim wirePolyline As vdPolyline = Nothing
        Dim wireRect As vdRect = Nothing
        Dim wireText As vdText = Nothing
        Dim wireWidth As Double = 0
        Dim wireYOffset As Double = 0

        Dim sortedConnections As New SortedDictionary(Of String, List(Of RoutingConnection))

        For Each connection As RoutingConnection In routingPath.Connections.Values
            If (connection.Cable Is Nothing) Then
                If (Not sortedConnections.ContainsKey(connection.Wire.Wire_number)) Then sortedConnections.Add(connection.Wire.Wire_number, New List(Of RoutingConnection))

                sortedConnections(connection.Wire.Wire_number).Add(connection)
            Else
                If (Not sortedConnections.ContainsKey(connection.Cable.Special_wire_id)) Then sortedConnections.Add(connection.Cable.Special_wire_id, New List(Of RoutingConnection))

                sortedConnections(connection.Cable.Special_wire_id).Add(connection)
            End If
        Next

        For Each sortedConnection As KeyValuePair(Of String, List(Of RoutingConnection)) In sortedConnections
            Using boundsSelection As New vdSelection
                For Each connection As RoutingConnection In sortedConnection.Value
                    Dim wireId As String = String.Empty
                    Dim wireNumber As String = String.Empty

                    If (connection.Core IsNot Nothing) Then
                        wireId = connection.Core.SystemId
                        wireNumber = connection.Core.Wire_number
                    Else
                        wireId = connection.Wire.SystemId
                        wireNumber = connection.Wire.Wire_number
                    End If

                    wireText = New vdText
                    With wireText
                        .SetUnRegisterDocument(Me.vDraw.ActiveDocument)
                        .setDocumentDefaults()

                        .Height = 3
                        .Label = wireId

                        If (Not connection.IsActiveConnection) Then .PenColor.SystemColor = Color.WhiteSmoke

                        .InsertionPoint = New gPoint(_currentXPos + 7, connectorSymbol.BoundingBox.MidPoint.y + 3.5 + wireYOffset)
                        .TextString = String.Format(EntireRoutingPathFormStrings.Wire_Text, wireNumber, vbCrLf, routingPath.SignalName)
                        .XProperties.Add("Harness").PropValue = routingPath.Harness.Part_number
                    End With

                    Me.vDraw.ActiveDocument.ActiveLayOut.Entities.AddItem(wireText)

                    wireLine = New vdLine
                    With wireLine
                        .SetUnRegisterDocument(Me.vDraw.ActiveDocument)
                        .setDocumentDefaults()

                        .EndPoint = New gPoint(wireText.BoundingBox.Right - 5, connectorSymbol.BoundingBox.MidPoint.y + wireYOffset)
                        .Label = wireId

                        If (Not connection.IsActiveConnection) Then .PenColor.SystemColor = Color.WhiteSmoke

                        .PenWidth = 1.5
                        .StartPoint = New gPoint(wireText.BoundingBox.Left, connectorSymbol.BoundingBox.MidPoint.y + wireYOffset)
                        .XProperties.Add("Harness").PropValue = routingPath.Harness.Part_number
                    End With

                    Me.vDraw.ActiveDocument.ActiveLayOut.Entities.AddItem(wireLine)

                    wireArrow = New vdInsert
                    With wireArrow
                        .SetUnRegisterDocument(Me.vDraw.ActiveDocument)
                        .setDocumentDefaults()

                        .Block = Me.vDraw.ActiveDocument.Blocks.FindName("VDDIM_DEFAULT")
                        .InsertionPoint = New gPoint(wireText.BoundingBox.Right, connectorSymbol.BoundingBox.MidPoint.y + wireYOffset)
                        .Label = wireId

                        If (Not connection.IsActiveConnection) Then .PenColor.SystemColor = Color.WhiteSmoke

                        .XProperties.Add("Harness").PropValue = routingPath.Harness.Part_number
                        .Xscale = CONNECTOR_GROUP_YOFFSET
                        .Yscale = CONNECTOR_GROUP_YOFFSET
                    End With

                    Me.vDraw.ActiveDocument.ActiveLayOut.Entities.AddItem(wireArrow)

                    Dim boundingBox As Box = wireText.BoundingBox
                    boundingBox.AddBox(wireLine.BoundingBox)
                    boundingBox.AddBox(wireArrow.BoundingBox)
                    boundingBox.AddBox(New Box(New gPoint(boundingBox.Min.x - 1, boundingBox.Min.y - 1), New gPoint(boundingBox.Max.x + 1, boundingBox.Max.y + 1)))

                    wirePolyline = New vdPolyline
                    With wirePolyline
                        .SetUnRegisterDocument(Me.vDraw.ActiveDocument)
                        .setDocumentDefaults()

                        .Flag = VectorDraw.Professional.Constants.VdConstPlineFlag.PlFlagCLOSE

                        Dim hatchProperties As New vdHatchProperties(VdConstFill.VdFillModeSolid)
                        hatchProperties.SetUnRegisterDocument(Me.vDraw.ActiveDocument)
                        hatchProperties.FillColor.SystemColor = Color.LightGray

                        .HatchProperties = hatchProperties
                        .Label = wireId
                        .SPlineFlag = VectorDraw.Professional.Constants.VdConstSplineFlag.SFlagQUADRATIC

                        .VertexList.Add(boundingBox.Min)
                        .VertexList.Add(New gPoint(boundingBox.Min.x, boundingBox.Min.y + 2))
                        .VertexList.Add(New gPoint(boundingBox.Min.x, boundingBox.UpperLeft.y - 2))
                        .VertexList.Add(boundingBox.UpperLeft)
                        .VertexList.Add(New gPoint(boundingBox.UpperLeft.x + 2, boundingBox.UpperLeft.y))
                        .VertexList.Add(New gPoint(boundingBox.Max.x - 2, boundingBox.Max.y))
                        .VertexList.Add(boundingBox.Max)
                        .VertexList.Add(New gPoint(boundingBox.Max.x, boundingBox.Max.y - 2))
                        .VertexList.Add(New gPoint(boundingBox.Max.x, boundingBox.LowerRight.y + 2))
                        .VertexList.Add(boundingBox.LowerRight)
                        .VertexList.Add(New gPoint(boundingBox.LowerRight.x - 2, boundingBox.LowerRight.y))
                        .VertexList.Add(New gPoint(boundingBox.Min.x + 2, boundingBox.Min.y))

                        .XProperties.Add("Harness").PropValue = routingPath.Harness.Part_number
                    End With

                    Me.vDraw.ActiveDocument.ActiveLayOut.Entities.AddItem(wirePolyline)
                    Me.vDraw.ActiveDocument.ActiveLayOut.Entities.ChangeOrder(wirePolyline, True)

                    boundsSelection.AddItem(wirePolyline, True, vdSelection.AddItemCheck.Nochecking)

                    wireWidth = Math.Max(wireWidth, boundingBox.Width)
                    wireYOffset += 20
                Next

                If (boundsSelection.Count <> 0) AndAlso (sortedConnection.Value.Count > 1) Then
                    cableText = New vdText
                    With cableText
                        .SetUnRegisterDocument(Me.vDraw.ActiveDocument)
                        .setDocumentDefaults()

                        .Height = 2
                        .HorJustify = VdConstHorJust.VdTextHorCenter
                        .InsertionPoint = New gPoint(boundsSelection.GetBoundingBox.MidPoint.x, boundsSelection.GetBoundingBox.Max.y + 2.5)
                        .TextString = String.Format(EntireRoutingPathFormStrings.Cable_Text, sortedConnection.Key)
                        .VerJustify = VdConstVerJust.VdTextVerCen
                    End With

                    Me.vDraw.ActiveDocument.ActiveLayOut.Entities.AddItem(cableText)

                    boundsSelection.AddItem(cableText, True, vdSelection.AddItemCheck.Nochecking)

                    cableRect = New vdRect
                    With cableRect
                        .SetUnRegisterDocument(Me.vDraw.ActiveDocument)
                        .setDocumentDefaults()

                        .Height = boundsSelection.GetBoundingBox.Height + 2.5
                        .InsertionPoint = New gPoint(boundsSelection.GetBoundingBox.Min.x - 1.25, boundsSelection.GetBoundingBox.Min.y - 1.25)
                        .Width = boundsSelection.GetBoundingBox.Width + 2.5
                    End With

                    Me.vDraw.ActiveDocument.ActiveLayOut.Entities.AddItem(cableRect)

                    wireYOffset += 5
                End If
            End Using
        Next

        If (cableRect IsNot Nothing) Then
            connectorHeight = Math.Max(connectorHeight, cableRect.BoundingBox.Top + CONNECTOR_GROUP_YOFFSET)
        Else
            connectorHeight = Math.Max(connectorHeight, wirePolyline.BoundingBox.Top + CONNECTOR_GROUP_YOFFSET)
        End If

        _currentXPos += wireWidth + 10
        harnessWidth += wireWidth + 10
    End Sub

    Private Sub GenerateRoutingPathDrawing_Recursively(routingPath As RoutingPath)
        Dim connectorHeight As Double = 0
        Dim harnessWidth As Double = 0
        Dim successorRoutingPath As RoutingPath = Nothing

        For Each routingConnection As RoutingConnection In routingPath.Connections.Values
            If (routingConnection.IsActiveConnection) Then
                For Each connectorSymbol As vdFigure In routingConnection.ConnectorSymbols
                    Me.vDraw.ActiveDocument.ActiveLayOut.Entities.AddItem(connectorSymbol)

                    Dim connectorGroup As VdSVGGroup = TryCast(connectorSymbol, VdSVGGroup)

                    If (connectorGroup IsNot Nothing) AndAlso (Not _transformedConnectorGroups.Contains(connectorGroup)) Then
                        ' HINT: This is a preliminary hack: The mirroring of all groups is done on the outer group. In case of connectors this is the vertex group and here
                        ' we get the real face (inner group) and hence this is not mirrored. The rotation must be reset on the real face or the outer group must be counter rotated.
                        If (connectorGroup.SVGType <> SvgType.table.ToString) Then
                            Dim matrix As New Matrix
                            matrix.A11 = -matrix.A11

                            If (connectorGroup.ECSMatrix.IsEqualMatrix(New Matrix, 0.1)) Then
                                connectorGroup.Transformby(matrix)
                            Else
                                Me.vDraw.ActiveDocument.CommandAction.CmdMirror(connectorGroup, connectorGroup.BoundingBox.MidPoint, New gPoint(connectorGroup.BoundingBox.Min.x, connectorGroup.BoundingBox.MidPoint.y), "no")
                            End If
                        End If

                        Dim rotation As Double = 0

                        For Each figure As vdFigure In connectorGroup.ChildGroups
                            If (TryCast(figure, VdSVGGroup) IsNot Nothing) Then
                                rotation = DirectCast(figure, VdSVGGroup).Rotation

                                Exit For
                            End If
                        Next

                        connectorGroup.Rotation = rotation

                        _transformedConnectorGroups.Add(connectorGroup)
                    End If

                    If (Not connectorSymbol.Deleted) AndAlso (connectorGroup Is Nothing OrElse connectorGroup.SVGType = SvgType.table.ToString) Then
                        If (harnessWidth = 0) Then _currentXPos += HARNESS_RECT_OFFSET
                        If (harnessWidth = 0) Then harnessWidth += HARNESS_RECT_OFFSET

                        Me.vDraw.ActiveDocument.CommandAction.CmdMove(connectorSymbol, connectorSymbol.BoundingBox.Min, New gPoint(_currentXPos, CONNECTOR_GROUP_YOFFSET))

                        Dim connectorShortName As String = connectorSymbol.XProperties.FindName("ShortName").PropValue.ToString
                        Dim connectorWidth As Double = connectorSymbol.BoundingBox.Width

                        If (connectorGroup IsNot Nothing) Then
                            For Each conSymbol As vdFigure In routingConnection.ConnectorSymbols
                                Dim conGroup As VdSVGGroup = TryCast(conSymbol, VdSVGGroup)

                                If (conGroup IsNot Nothing) AndAlso (Not conGroup.Deleted) AndAlso (conGroup.SVGType <> SvgType.table.ToString) AndAlso (conGroup.XProperties.FindName("ShortName").PropValue.ToString = connectorGroup.XProperties.FindName("ShortName").PropValue.ToString) Then
                                    Me.vDraw.ActiveDocument.CommandAction.CmdMove(conGroup, conGroup.BoundingBox.Min, New gPoint(connectorGroup.BoundingBox.Left, connectorGroup.BoundingBox.Top + CONNECTOR_GROUP_YOFFSET))

                                    If (conGroup.BoundingBox.Width > connectorWidth) Then connectorWidth = conGroup.BoundingBox.Width

                                    connectorHeight = Math.Max(connectorHeight, conGroup.BoundingBox.Top + CONNECTOR_GROUP_YOFFSET)

                                    Exit For
                                End If
                            Next

                            UpdateLighting_Recursively(connectorGroup.ChildGroups, Lighting.Normal)
                        Else
                            connectorHeight = Math.Max(connectorHeight, connectorSymbol.BoundingBox.Top + CONNECTOR_GROUP_YOFFSET)
                        End If

                        Dim shortNameText As New vdText
                        With shortNameText
                            .SetUnRegisterDocument(Me.vDraw.ActiveDocument)
                            .setDocumentDefaults()

                            .Bold = True
                            .Height = 4
                            .InsertionPoint = New gPoint(connectorSymbol.BoundingBox.Left, 4)
                            .TextString = connectorShortName
                        End With

                        Me.vDraw.ActiveDocument.ActiveLayOut.Entities.AddItem(shortNameText)

                        If (connectorGroup IsNot Nothing) Then
                            If (routingConnection.Core IsNot Nothing) Then
                                UpdateLighting_Recursively(connectorGroup.ChildGroups, Lighting.Highlight, routingConnection.Core.SystemId)
                            Else
                                UpdateLighting_Recursively(connectorGroup.ChildGroups, Lighting.Highlight, routingConnection.Wire.SystemId)
                            End If
                        End If

                        If (harnessWidth = HARNESS_RECT_OFFSET) Then
                            GeneratePartialHarnessDrawing(connectorHeight, connectorWidth, connectorSymbol, harnessWidth, routingPath, shortNameText)
                        Else
                            If (shortNameText.BoundingBox.Width > connectorWidth) Then
                                _currentXPos += shortNameText.BoundingBox.Width
                                harnessWidth += shortNameText.BoundingBox.Width
                            Else
                                _currentXPos += connectorWidth
                                harnessWidth += connectorWidth
                            End If
                        End If
                    End If
                Next

                Dim warningTextYOffset As Integer = -15

                If (routingConnection.SuccessorConnection IsNot Nothing) Then
                    If (routingConnection.ConnectorB IsNot Nothing) AndAlso (routingConnection.SuccessorConnection.ConnectorA IsNot Nothing) Then
                        If (routingConnection.ConnectorB.Slots.Length <> 0) AndAlso (routingConnection.SuccessorConnection.ConnectorA.Slots.Length <> 0) AndAlso (routingConnection.ConnectorB.Slots(0).Cavities.Length <> routingConnection.SuccessorConnection.ConnectorA.Slots(0).Cavities.Length) Then
                            AddWarningRectangleImage(New gPoint(_harnessXPos + harnessWidth + HARNESS_RECT_OFFSET - 15, warningTextYOffset - 2), "Error")

                            Dim cavCountMismatchWarningText As New vdText
                            With cavCountMismatchWarningText
                                .SetUnRegisterDocument(Me.vDraw.ActiveDocument)
                                .setDocumentDefaults()

                                .Height = 6
                                .InsertionPoint = New gPoint(_harnessXPos + harnessWidth + HARNESS_RECT_OFFSET, warningTextYOffset)
                                .TextString = EntireRoutingPathFormStrings.CavCountMismatch_Text
                            End With

                            Me.vDraw.ActiveDocument.ActiveLayOut.Entities.AddItem(cavCountMismatchWarningText)

                            warningTextYOffset -= 10
                        End If
                    End If

                    successorRoutingPath = _entireRoutingPath(routingConnection.SuccessorConnection.DocumentName)

                    If (routingPath.SignalName <> successorRoutingPath.SignalName) AndAlso (_inlinerPairCheckClassifications.FindInlinerPairCheckClassification("Signal") IsNot Nothing) AndAlso (_inlinerPairCheckClassifications.FindInlinerPairCheckClassification("Signal").Classification = "Error" OrElse _inlinerPairCheckClassifications.FindInlinerPairCheckClassification("Signal").Classification = "Warning") Then
                        AddWarningRectangleImage(New gPoint(_harnessXPos + harnessWidth + HARNESS_RECT_OFFSET - 15, warningTextYOffset - 2), _inlinerPairCheckClassifications.FindInlinerPairCheckClassification("Signal").Classification)

                        Dim signalWarningText As New vdText
                        With signalWarningText
                            .SetUnRegisterDocument(Me.vDraw.ActiveDocument)
                            .setDocumentDefaults()

                            .Height = 6
                            .InsertionPoint = New gPoint(_harnessXPos + harnessWidth + HARNESS_RECT_OFFSET, warningTextYOffset)
                            .TextString = EntireRoutingPathFormStrings.SignalWarning_Text
                        End With

                        Me.vDraw.ActiveDocument.ActiveLayOut.Entities.AddItem(signalWarningText)

                        warningTextYOffset -= 10
                    End If

                    Dim csaValueCurrent As Double = routingConnection.CrossSectionArea.Value_component
                    Dim csaValueSuccessor As Double = routingConnection.SuccessorConnection.CrossSectionArea.Value_component

                    If (Math.Round(csaValueCurrent, 2) <> Math.Round(csaValueSuccessor, 2)) AndAlso (_inlinerPairCheckClassifications.FindInlinerPairCheckClassification("CSA") IsNot Nothing) AndAlso (_inlinerPairCheckClassifications.FindInlinerPairCheckClassification("CSA").Classification = "Error" OrElse _inlinerPairCheckClassifications.FindInlinerPairCheckClassification("CSA").Classification = "Warning") Then
                        AddWarningRectangleImage(New gPoint(_harnessXPos + harnessWidth + HARNESS_RECT_OFFSET - 15, warningTextYOffset - 2), _inlinerPairCheckClassifications.FindInlinerPairCheckClassification("CSA").Classification)

                        Dim csaWarningText As New vdText
                        With csaWarningText
                            .SetUnRegisterDocument(Me.vDraw.ActiveDocument)
                            .setDocumentDefaults()

                            .Height = 6
                            .InsertionPoint = New gPoint(_harnessXPos + harnessWidth + HARNESS_RECT_OFFSET, warningTextYOffset)
                            .TextString = EntireRoutingPathFormStrings.CSAWarning_Text
                        End With

                        Me.vDraw.ActiveDocument.ActiveLayOut.Entities.AddItem(csaWarningText)

                        warningTextYOffset -= 10
                    End If
                End If

                Dim terminalPairs As New List(Of Tuple(Of General_terminal, General_terminal))
                terminalPairs.Add(New Tuple(Of General_terminal, General_terminal)(routingConnection.GeneralTerminalA, routingConnection.GeneralTerminalB))

                If (routingConnection.SuccessorConnection IsNot Nothing) Then terminalPairs.Add(New Tuple(Of General_terminal, General_terminal)(routingConnection.GeneralTerminalB, routingConnection.SuccessorConnection.GeneralTerminalA))

                For Each terminalPair As Tuple(Of General_terminal, General_terminal) In terminalPairs
                    If (terminalPair.Item1 IsNot Nothing) AndAlso (terminalPair.Item2 IsNot Nothing) Then
                        If (terminalPair.Item1.Plating_material IsNot Nothing AndAlso terminalPair.Item2.Plating_material Is Nothing) OrElse (terminalPair.Item1.Plating_material Is Nothing AndAlso terminalPair.Item2.Plating_material IsNot Nothing) OrElse (terminalPair.Item1.Plating_material IsNot Nothing AndAlso terminalPair.Item2.Plating_material IsNot Nothing AndAlso terminalPair.Item1.Plating_material.ToLower <> terminalPair.Item2.Plating_material.ToLower) Then
                            If (terminalPairs.IndexOf(terminalPair) = 0) Then
                                AddInformationBubbleImage(New gPoint(_harnessXPos + (harnessWidth / 3), -17))
                            ElseIf (_inlinerPairCheckClassifications.FindInlinerPairCheckClassification("TerminalPlating") IsNot Nothing) AndAlso (_inlinerPairCheckClassifications.FindInlinerPairCheckClassification("TerminalPlating").Classification = "Error" OrElse _inlinerPairCheckClassifications.FindInlinerPairCheckClassification("TerminalPlating").Classification = "Warning") Then
                                AddWarningRectangleImage(New gPoint(_harnessXPos + harnessWidth + HARNESS_RECT_OFFSET - 15, warningTextYOffset - 2), _inlinerPairCheckClassifications.FindInlinerPairCheckClassification("TerminalPlating").Classification)
                            End If

                            Dim platMatWarningText As New vdText
                            With platMatWarningText
                                .SetUnRegisterDocument(Me.vDraw.ActiveDocument)
                                .setDocumentDefaults()

                                .Height = 6

                                If (terminalPairs.IndexOf(terminalPair) = 0) Then
                                    .InsertionPoint = New gPoint(_harnessXPos + (harnessWidth / 3) + 15, -15)
                                    .TextString = EntireRoutingPathFormStrings.PlatMatInfo_Text
                                ElseIf (_inlinerPairCheckClassifications.FindInlinerPairCheckClassification("TerminalPlating") IsNot Nothing) AndAlso (_inlinerPairCheckClassifications.FindInlinerPairCheckClassification("TerminalPlating").Classification = "Error" OrElse _inlinerPairCheckClassifications.FindInlinerPairCheckClassification("TerminalPlating").Classification = "Warning") Then
                                    .InsertionPoint = New gPoint(_harnessXPos + harnessWidth + HARNESS_RECT_OFFSET, warningTextYOffset)
                                    .TextString = EntireRoutingPathFormStrings.PlatMatWarning_Text
                                End If
                            End With

                            Me.vDraw.ActiveDocument.ActiveLayOut.Entities.AddItem(platMatWarningText)
                        End If
                    ElseIf (terminalPairs.IndexOf(terminalPair) <> 0) AndAlso (_inlinerPairCheckClassifications.FindInlinerPairCheckClassification("TerminalPlating") IsNot Nothing) AndAlso (_inlinerPairCheckClassifications.FindInlinerPairCheckClassification("TerminalPlating").Classification = "Error" OrElse _inlinerPairCheckClassifications.FindInlinerPairCheckClassification("TerminalPlating").Classification = "Warning") Then
                        AddWarningRectangleImage(New gPoint(_harnessXPos + harnessWidth + HARNESS_RECT_OFFSET - 15, warningTextYOffset - 2), _inlinerPairCheckClassifications.FindInlinerPairCheckClassification("TerminalPlating").Classification)

                        Dim terminalWarningText As New vdText
                        With terminalWarningText
                            .SetUnRegisterDocument(Me.vDraw.ActiveDocument)
                            .setDocumentDefaults()

                            .Height = 6
                            .InsertionPoint = New gPoint(_harnessXPos + harnessWidth + HARNESS_RECT_OFFSET, warningTextYOffset)
                            .TextString = EntireRoutingPathFormStrings.TermMismatchWarning_Text
                        End With

                        Me.vDraw.ActiveDocument.ActiveLayOut.Entities.AddItem(terminalWarningText)
                    End If
                Next
            End If
        Next

        Dim harnessRect As New vdRect
        With harnessRect
            .SetUnRegisterDocument(Me.vDraw.ActiveDocument)
            .setDocumentDefaults()

            .Height = connectorHeight + CONNECTOR_GROUP_YOFFSET
            .InsertionPoint = New gPoint(_harnessXPos, 0)

            If (_entireRoutingPath.Values.Where(Function(path) path.IsOriginHarness)(0).Harness.Equals(routingPath.Harness)) Then .PenColor.SystemColor = Color.Magenta

            .PenWidth = 1
            .Width = harnessWidth + HARNESS_RECT_OFFSET
        End With

        Me.vDraw.ActiveDocument.ActiveLayOut.Entities.AddItem(harnessRect)

        Dim harnessText As New vdText
        With harnessText
            .SetUnRegisterDocument(Me.vDraw.ActiveDocument)
            .setDocumentDefaults()

            .Bold = True
            .Height = 6
            .HorJustify = VectorDraw.Professional.Constants.VdConstHorJust.VdTextHorCenter
            .InsertionPoint = New gPoint(harnessRect.BoundingBox.MidPoint.x, harnessRect.BoundingBox.Max.y - (CONNECTOR_GROUP_YOFFSET / 2))

            If (_entireRoutingPath.Values.Where(Function(path) path.IsOriginHarness)(0).Harness.Equals(routingPath.Harness)) Then .PenColor.SystemColor = Color.Magenta

            .TextString = String.Format(EntireRoutingPathFormStrings.Harness_Text, routingPath.Harness.Part_number)
            .VerJustify = VectorDraw.Professional.Constants.VdConstVerJust.VdTextVerCen
        End With

        Me.vDraw.ActiveDocument.ActiveLayOut.Entities.AddItem(harnessText)

        harnessText = New vdText
        With harnessText
            .SetUnRegisterDocument(Me.vDraw.ActiveDocument)
            .setDocumentDefaults()

            .Bold = True
            .Height = 4
            .HorJustify = VectorDraw.Professional.Constants.VdConstHorJust.VdTextHorCenter
            .InsertionPoint = New gPoint(harnessRect.BoundingBox.MidPoint.x, harnessRect.BoundingBox.Max.y - CONNECTOR_GROUP_YOFFSET - 4)

            If (_entireRoutingPath.Values.Where(Function(path) path.IsOriginHarness)(0).Harness.Equals(routingPath.Harness)) Then .PenColor.SystemColor = Color.Magenta

            .TextString = routingPath.Harness.Description
            .VerJustify = VectorDraw.Professional.Constants.VdConstVerJust.VdTextVerCen
        End With

        Me.vDraw.ActiveDocument.ActiveLayOut.Entities.AddItem(harnessText)

        _currentXPos += 2 * HARNESS_RECT_OFFSET
        _harnessXPos = _currentXPos

        If (successorRoutingPath IsNot Nothing) Then
            Dim harnessArrow As New vdInsert
            With harnessArrow
                .SetUnRegisterDocument(Me.vDraw.ActiveDocument)
                .setDocumentDefaults()

                .Block = Me.vDraw.ActiveDocument.Blocks.FindName("VDDIM_DEFAULT")
                .InsertionPoint = New gPoint(harnessRect.BoundingBox.Max.x + HARNESS_RECT_OFFSET, HARNESS_RECT_OFFSET)
                .PenColor.SystemColor = Color.Red
                .Xscale = CONNECTOR_GROUP_YOFFSET
                .Yscale = CONNECTOR_GROUP_YOFFSET
            End With

            Me.vDraw.ActiveDocument.ActiveLayOut.Entities.AddItem(harnessArrow)

            GenerateRoutingPathDrawing_Recursively(successorRoutingPath)
        End If
    End Sub

    Private Function GetInformationEventArgs(ParamArray entities() As vdFigure) As InformationHubEventArgs
        Dim informationHubEventArgs As New InformationHubEventArgs(String.Empty)
        informationHubEventArgs.ObjectIds = New HashSet(Of String)

        If (entities IsNot Nothing AndAlso entities.Length > 0) Then
            Dim idPairs As New List(Of KeyValuePair(Of String, String))
            For Each entity As vdFigure In entities
                If entity IsNot Nothing AndAlso entity.Label <> String.Empty Then
                    idPairs.Add(New KeyValuePair(Of String, String)(entity.XProperties.FindName("Harness").PropValue.ToString, entity.Label))
                End If
            Next

            For Each grp_pair As KeyValuePair(Of String, List(Of String)) In idPairs.GroupBy(Function(kv) kv.Key).ToDictionary(Function(grp) grp.Key, Function(grp) grp.Select(Function(kv) kv.Value).Distinct.ToList)
                For Each value As String In grp_pair.Value
                    informationHubEventArgs.ObjectIds.Add(grp_pair.Key)
                    informationHubEventArgs.ObjectIds.Add(value)
                Next
            Next

            informationHubEventArgs.ObjectType = KblObjectType.Wire_occurrence
        End If

        Return informationHubEventArgs
    End Function

    Private Sub GetStartRoutingPath_Recursively(connections As Dictionary(Of String, RoutingConnection), ByRef startRoutingPath As RoutingPath)
        If (startRoutingPath Is Nothing) Then
            For Each connection As RoutingConnection In connections.Values
                If (connection.IsActiveConnection) Then
                    If (connection.PredecessorConnection IsNot Nothing) Then
                        GetStartRoutingPath_Recursively(_entireRoutingPath(connection.PredecessorConnection.DocumentName).Connections, startRoutingPath)
                    Else
                        startRoutingPath = _entireRoutingPath(connection.DocumentName)
                    End If
                End If
            Next
        End If
    End Sub

    Private Sub UpdateLighting_Recursively(entities As vdEntities, lighting As Lighting, Optional kblId As String = Nothing)
        For Each figure As vdFigure In entities
            If (TypeOf figure Is VdSVGGroup) Then
                If (kblId Is Nothing OrElse DirectCast(figure, VdSVGGroup).KblId = kblId OrElse DirectCast(figure, VdSVGGroup).SecondaryKblIds.Contains(kblId)) AndAlso (DirectCast(figure, VdSVGGroup).Lighting <> Lighting.Lowlight) Then
                    DirectCast(figure, VdSVGGroup).Lighting = lighting
                End If

                UpdateLighting_Recursively(DirectCast(figure, VdSVGGroup).ChildGroups, lighting, kblId)
            End If
        Next
    End Sub

    Private Sub UpdateActiveConnections_Recursively(routingConnection As RoutingConnection, isPredecessor As Boolean)
        For Each connection As RoutingConnection In _entireRoutingPath(routingConnection.DocumentName).Connections.Values
            If (connection.Equals(routingConnection)) Then
                connection.IsActiveConnection = True

                If (isPredecessor) Then
                    If (connection.PredecessorConnection IsNot Nothing) Then UpdateActiveConnections_Recursively(connection.PredecessorConnection, isPredecessor)
                Else
                    If (connection.SuccessorConnection IsNot Nothing) Then UpdateActiveConnections_Recursively(connection.SuccessorConnection, isPredecessor)
                End If
            Else
                connection.IsActiveConnection = False
            End If
        Next
    End Sub


    Private Sub EntireRoutingPathForm_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        GenerateDrawing(True)
    End Sub

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Me.Close()
    End Sub

    Private Sub btnExport_Click(sender As Object, e As EventArgs) Handles btnExport.Click
        Dim routingPath As RoutingPath = _entireRoutingPath.Values.Where(Function(path) path.IsOriginHarness)(0)
        Dim wireNumber As String = String.Empty

        If (routingPath.Connections.First.Value.Core IsNot Nothing) Then
            wireNumber = routingPath.Connections.First.Value.Core.Wire_number
        Else
            wireNumber = routingPath.Connections.First.Value.Wire.Wire_number
        End If

        Using sfdExport As New SaveFileDialog
            With sfdExport
                .DefaultExt = KnownFile.DXF.Trim("."c)
                .FileName = String.Format("Entire_routing_path_for_wire_{0}", Regex.Replace(wireNumber, "\W", "_"))
                .Filter = "Autodesk DXF file(*.dxf)|*.dxf|JPEG (*.jpg)|*.jpg|PNG (*.png)|*.png|Bitmap (*.bmp)|*.bmp"
                .Title = EntireRoutingPathFormStrings.ExportRoutingPathToFile_Title

                If (.ShowDialog(Me) = DialogResult.OK) Then
                    Try
                        If (VdOpenSave.SaveAs(Me.vDraw.ActiveDocument, sfdExport.FileName)) Then
                            MessageBox.Show(Me, EntireRoutingPathFormStrings.ExportSuccess_Msg, [Shared].MSG_BOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Information)
                        Else
                            MessageBox.Show(Me, EntireRoutingPathFormStrings.ProblemExport_Msg, [Shared].MSG_BOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        End If
                    Catch ex As Exception
                        MessageBox.Show(Me, String.Format(EntireRoutingPathFormStrings.ErrorExport_Msg, vbCrLf, ex.Message), [Shared].MSG_BOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    End Try
                End If
            End With
        End Using
    End Sub

    Private Sub btnPrint_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnPrint.Click
        Dim printing As Printing.VdPrinting = Nothing

        Try
            printing = New Printing.VdPrinting(Me.vDraw.ActiveDocument)
            printing.DocumentName = Me.Text
        Catch ex As Exception
            ex.ShowMessageBox(Me, String.Format(EntireRoutingPathFormStrings.PrintFailed_Msg, ex.Message))
            Return
        End Try

        If printing IsNot Nothing Then
            Using printForm As New Printing.PrintForm(printing, Nothing, False)
                printForm.ShowDialog(Me)
            End Using
        End If
    End Sub

    Private Sub btnRedraw_Click(sender As Object, e As EventArgs) Handles btnRedraw.Click
        If Me.vDraw.ActiveDocument IsNot Nothing Then
            Me.vDraw.ActiveDocument.ZoomExtents()
            Me.vDraw.ActiveDocument.Redraw(True)
        End If
    End Sub

    Private Sub btnSelectAll_Click(sender As Object, e As EventArgs) Handles btnSelectAll.Click
        Dim args As InformationHubEventArgs = GetInformationEventArgs(Me.vDraw.ActiveDocument.ActiveLayOut.Entities.Cast(Of vdFigure).ToArray)
        If (args.ObjectIds.Count > 0) Then
            RaiseEvent EntireRoutingPathViewMouseClick(Me, args)
        End If
    End Sub

    Private Sub vDraw_MouseClick(sender As Object, e As MouseEventArgs) Handles vDraw.MouseClick
        If (e.Button = System.Windows.Forms.MouseButtons.Left) Then
            Dim clickPoint As gPoint = Me.vDraw.ActiveDocument.CCS_CursorPos
            Dim hitEntity As vdFigure = Me.vDraw.ActiveDocument.ActiveLayOut.GetEntityFromPoint(Me.vDraw.ActiveDocument.ActiveRender.View2PixelI(clickPoint), 4, False, vdDocument.LockLayerMethodEnum.DisableAll)
            If (hitEntity IsNot Nothing) AndAlso (hitEntity.Label <> String.Empty) Then
                Dim routingPath As RoutingPath = _entireRoutingPath.Values.Where(Function(path) path.Connections.ContainsKey(hitEntity.Label))(0)

                For Each routingConnection As KeyValuePair(Of String, RoutingConnection) In routingPath.Connections
                    If (routingConnection.Key = hitEntity.Label) Then
                        routingConnection.Value.IsActiveConnection = True

                        If (routingConnection.Value.PredecessorConnection IsNot Nothing) Then UpdateActiveConnections_Recursively(routingConnection.Value.PredecessorConnection, True)
                        If (routingConnection.Value.SuccessorConnection IsNot Nothing) Then UpdateActiveConnections_Recursively(routingConnection.Value.SuccessorConnection, False)
                    Else
                        routingConnection.Value.IsActiveConnection = False
                    End If
                Next

                GenerateDrawing(False)

                Dim informationHubEventArgs As InformationHubEventArgs = GetInformationEventArgs(hitEntity)
                If (informationHubEventArgs.ObjectIds.Count > 0) Then
                    RaiseEvent EntireRoutingPathViewMouseClick(Me, informationHubEventArgs)
                End If
            End If
        End If
    End Sub

    Private Sub vDraw_MouseMove(sender As Object, e As MouseEventArgs) Handles vDraw.MouseMove
        Dim hoveredEntity As vdFigure = Me.vDraw.ActiveDocument.ActiveLayOut.GetEntityFromPoint(Me.vDraw.ActiveDocument.ActiveRender.View2PixelI(Me.vDraw.ActiveDocument.CCS_CursorPos), 4, False, vdDocument.LockLayerMethodEnum.DisableAll)
        If (hoveredEntity IsNot Nothing) AndAlso (hoveredEntity.Label <> String.Empty) Then
            Me.vDraw.SetCustomMousePointer(Cursors.Hand)
            Me.vDraw.ActiveDocument.Invalidate()
        ElseIf (Me.vDraw.GetCustomMousePointer IsNot Nothing) Then
            Me.vDraw.SetCustomMousePointer(Nothing)
            Me.vDraw.ActiveDocument.Invalidate()
        End If
    End Sub

    Private Sub _wStateDetector_WindowStateChanged(sender As Object, e As EventArgs) Handles _wStateDetector.WindowStateChanged
        Me.vDraw.ActiveDocument.CommandAction.Zoom("E", 0, 0)
    End Sub

End Class