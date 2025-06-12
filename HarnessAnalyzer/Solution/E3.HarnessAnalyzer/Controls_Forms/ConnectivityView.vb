Imports System.IO
Imports System.Text.RegularExpressions
Imports Infragistics.Win
Imports Infragistics.Win.UltraWinToolTip
Imports VectorDraw.Geometry
Imports VectorDraw.Geometry.Globals
Imports VectorDraw.Professional.Constants
Imports VectorDraw.Professional.vdFigures
Imports VectorDraw.Professional.vdObjects
Imports Zuken.E3.HarnessAnalyzer.Settings

<Reflection.Obfuscation(Feature:="renaming", ApplyToMembers:=False, Exclude:=True)> ' needed to avoid resource-file-names obfuscation errors, see issue #5
Public Class ConnectivityView

    Friend Event ConnectivityViewMouseClick(sender As Object, e As InformationHubEventArgs)

    Private _sourceConnector As Connector_occurrence
    Private _kblMapper As KblMapper
    Private _caption As String
    Private _activeDoc As vdDocument
    Private _currentPositionX As Double
    Private _currentPositionY As Double
    Private _sourceConnectorHeight As Double
    Private _connectorWidth As Double
    Private _destinationConnector As Dictionary(Of String, Dictionary(Of String, Cavity_occurrence))
    Private _wireInformationMapper As Dictionary(Of String, WireInformation)
    Private _connectionWirePolyLines As List(Of vdPolyline)
    Private _shortCircuitPolyLines As List(Of vdPolyline)
    Private _sourceCable As Dictionary(Of String, List(Of gPoint))
    Private _destinationCable As Dictionary(Of String, List(Of gPoint))
    Private _sourcePointWireMapper As Dictionary(Of String, gPoint)
    Private _wireConnectivityList As List(Of WireConnectivity)
    Private _destinationConnectorRectBox As Dictionary(Of String, Box)
    Private _destinationComponentRectBox As Dictionary(Of String, Box)
    Private _wireColorCodeList As WireColorCodeList
    Private _inactiveObjects As ITypeGroupedKblIds

    Private Const HALF_OR_DOUBLE_VALUE As Double = 2
    Private Const COMPONENT_TEXT_OFFSET As Double = 0.35
    Private Const COMPONENT_TEXT_OFFSET1 As Double = 0.65
    Private Const COMPONENT_TEXT_OFFSET2 As Double = 0.7
    Private Const CONNECTOR_NAME_OFFSET As Double = 0.5
    Private Const CONNECTOR_DESCRIPTION_OFFSET As Double = 0.9
    Private Const DESCRIPTION_TEXT_WEIGHT As Double = 0.3
    Private Const NAME_TEXT_WEIGHT As Double = 0.4
    Private Const WIRE_NO_WEIGHT As Double = 0.2
    Private Const WIRE_OFFSET_X As Double = 0.6
    Private Const WIRE_OFFSET_Y As Double = 0.1
    Private Const INCREASE_LENGTH As Double = 0.4
    Private Const SOURCE_CONNECTOR_WIDTH As Double = 2.3
    Private Const DESTINATION_CONNECTOR_WIDTH As Double = 2.0
    Private Const NOT_CONNECTOR_WIRE_OFFSET_X As Double = 1 / 6
    Private Const NOT_CONNECTOR_WIRE_OFFSET_Y1 As Double = 2.2
    Private Const NOT_CONNECTOR_WIRE_OFFSET_Y2 As Double = 1 / 4
    Private Const NOT_CONNECTOR_WIRE_TEXT_OFFSET_X As Double = 0.1
    Private Const NOT_CONNECTOR_WIRE_TEXT_OFFSET_Y As Double = 1.75
    Private Const CAVITY_RADIUS As Double = 0.08
    Private Const CAVITY_SPACE As Double = 1
    Private Const ORIGIN_POSITION_X As Double = 0
    Private Const ORIGIN_POSITION_Y As Double = 0
    Private Const SOURCE_DESTINATION_DISTANCE As Double = 7
    Private Const DESTINATION_CONNECTOR_SPACE As Double = 3
    Private Const SHORT_CIRCUIT_SPACE As Double = 0.3
    Private Const CONNECTOR_SIZE As Double = 1.5
    Private Const ADDITIONAL_WIDTH As Double = 1
    Private Const ADDITIONAL_CAVITY_NO As Double = 2
    Private Const DESTINATION_OFFSET_Y As Double = 1.7
    Private Const TEXT_ROTATION_ANGLE As Double = 90
    Private Const X_SHIFTING As Double = 0.5
    Private Const Y_SHIFTING As Double = 0.15
    Private Const ELLIPSE_HEIGHT As Double = 0.7
    Private Const ELLIPSE_WIDTH As Double = 0.15
    Private Const ELLIPSE_OFFSET As Double = 2.5

    Private Structure WireConnectivity
        Public SourcePoint As gPoint
        Public WireID As String
        Public DestinationPoint As gPoint
        Public DestinationConnector As Connector_occurrence
    End Structure

    Private Structure WireInformation
        Public WireNumber As String
        Public ToolTip As String
        Public CSA As Double
        Public Color1 As Color
        Public Color2 As Color
        Public CableId As String
    End Structure


    Public Sub New(connector As Connector_occurrence, inactiveObjects As ITypeGroupedKblIds, kblMapper As KblMapper, wireColorCodeList As WireColorCodeList)
        InitializeComponent()

        _kblMapper = kblMapper
        _sourceConnector = connector
        _caption = Me.Text
        _wireColorCodeList = wireColorCodeList
        _inactiveObjects = inactiveObjects

        Me.BackColor = Color.White
        Me.Text = _caption.Replace("*", _sourceConnector.Id)
    End Sub

    Private Sub DoDraw()
        InitializeMappers()
        InitializeVectorDrawBaseControl()

        DrawConnectivityView()

        Me.vDraw.Visible = True

        _activeDoc.ZoomExtents()
        _activeDoc.Redraw(True)
    End Sub

    Private Sub InitializeMappers()
        _connectionWirePolyLines = New List(Of vdPolyline)
        _destinationCable = New Dictionary(Of String, List(Of gPoint))
        _destinationComponentRectBox = New Dictionary(Of String, Box)
        _destinationConnector = New Dictionary(Of String, Dictionary(Of String, Cavity_occurrence))
        _destinationConnectorRectBox = New Dictionary(Of String, Box)
        _shortCircuitPolyLines = New List(Of vdPolyline)
        _sourceCable = New Dictionary(Of String, List(Of gPoint))
        _sourcePointWireMapper = New Dictionary(Of String, gPoint)
        _wireConnectivityList = New List(Of WireConnectivity)
    End Sub

    Private Sub InitializeVectorDrawBaseControl()
        If (Me.vDraw.ActiveDocument Is Nothing) Then
            Me.vDraw.AllowDrop = False
            Me.vDraw.EnsureDocument()
            Me.vDraw.InputDevice_TouchGesture.Actions = VectorDraw.Professional.Actions.TouchGestureActions.Pan Or VectorDraw.Professional.Actions.TouchGestureActions.Zoom
        End If

        _activeDoc = Me.vDraw.ActiveDocument
        _activeDoc.[New]()

        With _activeDoc
            .UndoHistory.PushEnable(False)

            .DisableZoomOnResize = True
            .EnableAutoGripOn = False
            .FreezeGridEvents = True

            .GlobalRenderProperties.AxisSize = 10
            .GlobalRenderProperties.CrossSize = 8

            .GridMeasure.GridColor.SystemColor = Color.Green
            .GridMode = False
            .GripSnap = True
            .GridSpaceX = 1.0
            .GridSpaceY = 1.0

            .OrbitActionKey = VectorDraw.Professional.vdObjects.vdDocument.KeyWithMouseStroke.None
            .OsnapDialogKey = Keys.None
            .Palette.Background = Color.White
            .PenCapsSquare = True
            .ShowUCSAxis = False
            .SnapMode = False
            .UrlActionKey = Keys.None
        End With
    End Sub

    Private Sub DrawConnectivityView()
        _currentPositionX = ORIGIN_POSITION_X
        _currentPositionY = ORIGIN_POSITION_Y

        DrawSourceConnector()
        DrawDestinationConnectors()
        DrawWires()
        DrawCables()
    End Sub

    Private Sub DrawWires()
        For Each wireConnectivity As WireConnectivity In _wireConnectivityList
            If _inactiveObjects IsNot Nothing AndAlso (_inactiveObjects.ContainsValue(KblObjectType.Core_occurrence, wireConnectivity.WireID) OrElse _inactiveObjects.ContainsValue(KblObjectType.Wire_occurrence, wireConnectivity.WireID)) Then
                ' HINT: Wire is inactive --> Ignore it
                Continue For
            Else
                Dim endPoint As gPoint = Nothing
                Dim startPoint As gPoint = Nothing

                Dim dashedPolyLine As vdPolyline = Nothing
                Dim polyLine As vdPolyline = Nothing

                Dim wireInfo As New WireInformation
                Dim wireText As vdText = Nothing

                With wireConnectivity
                    endPoint = .DestinationPoint
                    startPoint = .SourcePoint
                    wireInfo = _wireInformationMapper(.WireID)
                End With

                If (startPoint.x = endPoint.x) Then
                    Dim distanceX As Double = DESTINATION_CONNECTOR_SPACE * CAVITY_SPACE + _shortCircuitPolyLines.Count * SHORT_CIRCUIT_SPACE
                    Dim vertexList As Vertexes = New Vertexes

                    With vertexList
                        .Add(startPoint)
                        .Add(New gPoint(startPoint.x + distanceX, startPoint.y))
                        .Add(New gPoint(endPoint.x + distanceX, endPoint.y))
                        .Add(endPoint)
                    End With

                    polyLine = DrawPolyline(vertexList, wireInfo, wireInfo.Color1, GetWirePenWidth(wireInfo.CSA), False)
                    _shortCircuitPolyLines.Add(polyLine)

                    If (wireInfo.Color2.Name <> "0") Then
                        dashedPolyLine = DrawPolyline(vertexList, wireInfo, wireInfo.Color2, GetWirePenWidth(wireInfo.CSA), True)
                    End If
                Else
                    Dim vertexList As Vertexes = New Vertexes
                    With vertexList
                        .Add(startPoint)
                        .Add(New gPoint(endPoint.x, startPoint.y))
                        .Add(endPoint)
                    End With

                    polyLine = DrawPolyline(vertexList, wireInfo, wireInfo.Color1, GetWirePenWidth(wireInfo.CSA), False)

                    If (wireInfo.Color2.Name <> "0") Then
                        dashedPolyLine = DrawPolyline(vertexList, wireInfo, wireInfo.Color2, GetWirePenWidth(wireInfo.CSA), True)
                    End If
                End If

                If (dashedPolyLine IsNot Nothing) Then
                    dashedPolyLine.Label = wireConnectivity.WireID
                End If

                polyLine.Label = wireConnectivity.WireID
                wireText = DrawText(New gPoint(startPoint.x + X_SHIFTING, startPoint.y + Y_SHIFTING), wireInfo.WireNumber, Color.Black, VdConstHorJust.VdTextHorLeft, DESCRIPTION_TEXT_WEIGHT)

                With _activeDoc
                    .ActiveLayOut.Entities.AddItem(polyLine)
                    _connectionWirePolyLines.Add(polyLine)

                    If wireInfo.Color2.Name <> "0" Then
                        .ActiveLayOut.Entities.AddItem(dashedPolyLine)
                        _connectionWirePolyLines.Add(dashedPolyLine)
                    End If

                    .ActiveLayOut.Entities.AddItem(wireText)
                End With
            End If
        Next
    End Sub

    Private Sub DrawCables()
        RemoveUndrawCable()

        For Each cable As KeyValuePair(Of String, List(Of gPoint)) In _destinationCable
            If Not (_inactiveObjects IsNot Nothing AndAlso _inactiveObjects.ContainsValue(KblObjectType.Special_wire_occurrence, cable.Key)) Then
                DrawCableEllipseAndText(cable.Key, cable.Value, False)
            End If
        Next

        For Each cable As KeyValuePair(Of String, List(Of gPoint)) In _sourceCable
            If Not (_inactiveObjects IsNot Nothing AndAlso _inactiveObjects.ContainsValue(KblObjectType.Special_wire_occurrence, cable.Key)) Then
                DrawCableEllipseAndText(cable.Key, cable.Value, True)
            End If
        Next
    End Sub

    Private Sub DrawSourceConnector()
        DrawConnector(_sourceConnector, True)

        _currentPositionX = _currentPositionX + SOURCE_DESTINATION_DISTANCE * CAVITY_SPACE
        _currentPositionY = _currentPositionY + _sourceConnectorHeight + SOURCE_DESTINATION_DISTANCE * CAVITY_SPACE

        InitializeConnectorList()
    End Sub

    Private Sub InitializeConnectorList()
        Dim sourceItem As New Infragistics.Win.ValueListItem(_sourceConnector, _sourceConnector.Id)

        Me.uceConnectors.Items.Clear()
        Me.uceConnectors.Items.Add(sourceItem)

        For Each destinationConnector As KeyValuePair(Of String, Dictionary(Of String, Cavity_occurrence)) In _destinationConnector
            Dim connector As Connector_occurrence = _kblMapper.GetConnectorOccurrence(destinationConnector.Key)
            If (connector IsNot _sourceConnector) Then
                Me.uceConnectors.Items.Add(connector, connector.Id)
            End If
        Next

        Me.uceConnectors.SortStyle = Infragistics.Win.ValueListSortStyle.Ascending
        Me.uceConnectors.SelectedItem = sourceItem
    End Sub

    Private Sub DrawConnector(connector As Connector_occurrence, isSourceConnector As Boolean, Optional _destinationCavity As Dictionary(Of String, Cavity_occurrence) = Nothing)
        Dim cavities As New List(Of Cavity_occurrence)

        If (connector.Slots IsNot Nothing) AndAlso (connector.Slots.Length <> 0) Then
            For Each cavity As Cavity_occurrence In connector.Slots(0).Cavities
                cavities.Add(cavity)
            Next
        End If

        cavities.Sort(New CavitySortComparer(_kblMapper.KBLPartMapper))

        Dim cavityCount As Integer = GetCavityConnectCount(cavities, isSourceConnector)

        DrawConnectorSymbol(connector, cavityCount, isSourceConnector)
        DrawCavity(cavities, cavityCount, isSourceConnector, _destinationCavity)
    End Sub

    Private Function GetCavityConnectCount(cavities As List(Of Cavity_occurrence), isSourceConnector As Boolean) As Integer
        Dim count As Integer = 0

        For Each cavity As Cavity_occurrence In cavities
            If (_kblMapper.KBLWireCavityMapper.ContainsKey(cavity.SystemId)) Then
                count += _kblMapper.KBLWireCavityMapper(cavity.SystemId).Count

                If (isSourceConnector) Then
                    For Each cavityWire As KeyValuePair(Of String, List(Of Cavity_occurrence)) In _kblMapper.KBLWireCavityMapper(cavity.SystemId)
                        If (_kblMapper.KBLCavityConnectorMapper.ContainsKey(cavityWire.Value.FirstOrDefault.SystemId)) Then
                            With cavityWire
                                Dim connector As Connector_occurrence = _kblMapper.GetConnectorOfCavity(.Value.FirstOrDefault.SystemId)
                                Dim wireCavityMapper As New Dictionary(Of String, Cavity_occurrence)

                                If (_destinationConnector.ContainsKey(connector.SystemId)) Then
                                    wireCavityMapper = _destinationConnector(connector.SystemId)

                                    If Not wireCavityMapper.ContainsKey(.Key) Then
                                        wireCavityMapper.Add(.Key, .Value.FirstOrDefault)
                                    End If
                                Else
                                    wireCavityMapper.Add(.Key, .Value.FirstOrDefault)

                                    _destinationConnector.Add(connector.SystemId, wireCavityMapper)
                                End If
                            End With
                        End If
                    Next
                End If
            Else
                count += 1
            End If
        Next

        Return count
    End Function

    Private Sub DrawConnectorSymbol(connector As Connector_occurrence, cavityCount As Integer, isSourceConnector As Boolean)
        Dim parentComponent As Component_occurrence = GetParentComponent(connector)
        Dim componentRect As vdRect = Nothing
        Dim componentName As vdText = Nothing
        Dim componentDiscription As vdText = Nothing
        Dim connectorRect As New vdRect()
        Dim connectorName As vdText = New vdText()
        Dim connectorDiscription As vdText = Nothing
        Dim insertionPoint As gPoint

        If parentComponent IsNot Nothing Then
            If isSourceConnector Then
                Dim width As Double = 0

                If parentComponent.Description IsNot Nothing Then
                    width = CAVITY_SPACE * CONNECTOR_SIZE
                Else
                    width = CAVITY_SPACE
                End If

                insertionPoint = New gPoint(_currentPositionX, _currentPositionY)
                componentRect = DrawRectangle(insertionPoint, width, (cavityCount + ADDITIONAL_CAVITY_NO) * CAVITY_SPACE, Color.Black, Color.LightGray)
            Else
                Dim height As Double = 0

                If parentComponent.Description IsNot Nothing Then
                    height = CAVITY_SPACE * CONNECTOR_SIZE
                Else
                    height = CAVITY_SPACE
                End If

                insertionPoint = New gPoint(_currentPositionX - CAVITY_SPACE / HALF_OR_DOUBLE_VALUE, _currentPositionY)
                componentRect = DrawRectangle(insertionPoint, (cavityCount + ADDITIONAL_CAVITY_NO) * CAVITY_SPACE, height, Color.Black, Color.LightGray)
            End If

            componentName = New vdText

            If isSourceConnector Then
                If parentComponent.Description IsNot Nothing Then
                    insertionPoint = New gPoint(_currentPositionX + COMPONENT_TEXT_OFFSET2 * componentRect.Width, _currentPositionY + (cavityCount + ADDITIONAL_CAVITY_NO) * CAVITY_SPACE / 2)
                Else
                    insertionPoint = New gPoint(_currentPositionX + COMPONENT_TEXT_OFFSET1 * componentRect.Width, _currentPositionY + (cavityCount + ADDITIONAL_CAVITY_NO) * CAVITY_SPACE / 2)
                End If

                componentName = DrawText(insertionPoint, parentComponent.Id, Color.Red, , NAME_TEXT_WEIGHT, DegreesToRadians(TEXT_ROTATION_ANGLE))
            Else
                If parentComponent.Description IsNot Nothing Then
                    insertionPoint = New gPoint(_currentPositionX + (cavityCount + ADDITIONAL_WIDTH) * CAVITY_SPACE / HALF_OR_DOUBLE_VALUE, _currentPositionY + COMPONENT_TEXT_OFFSET2 * componentRect.Height)
                Else
                    insertionPoint = New gPoint(_currentPositionX + (cavityCount + ADDITIONAL_WIDTH) * CAVITY_SPACE / HALF_OR_DOUBLE_VALUE, _currentPositionY + COMPONENT_TEXT_OFFSET * componentRect.Height)
                End If

                componentName = DrawText(insertionPoint, parentComponent.Id, Color.Red, , NAME_TEXT_WEIGHT)
            End If

            If parentComponent.Description IsNot Nothing Then
                componentDiscription = New vdText()

                If isSourceConnector Then
                    insertionPoint = New gPoint(_currentPositionX + COMPONENT_TEXT_OFFSET * componentRect.Height, _currentPositionY + (cavityCount + ADDITIONAL_WIDTH) * CAVITY_SPACE / HALF_OR_DOUBLE_VALUE)
                    componentDiscription = DrawText(insertionPoint, Replace("[*]", "*", parentComponent.Description), Color.Black, , DESCRIPTION_TEXT_WEIGHT, DegreesToRadians(TEXT_ROTATION_ANGLE))
                Else
                    insertionPoint = New gPoint(_currentPositionX + (cavityCount + ADDITIONAL_WIDTH) * CAVITY_SPACE / HALF_OR_DOUBLE_VALUE, _currentPositionY + COMPONENT_TEXT_OFFSET * componentRect.Height)
                    componentDiscription = DrawText(insertionPoint, Replace("[*]", "*", parentComponent.Description), Color.Black, , DESCRIPTION_TEXT_WEIGHT)
                End If
            End If

            If isSourceConnector Then
                _currentPositionX = _currentPositionX + componentRect.Width
                _currentPositionY = _currentPositionY + CAVITY_SPACE / HALF_OR_DOUBLE_VALUE
            End If
        End If

        If isSourceConnector Then
            insertionPoint = New gPoint(_currentPositionX, _currentPositionY)
            connectorRect = DrawRectangle(insertionPoint, SOURCE_CONNECTOR_WIDTH * CAVITY_SPACE, (cavityCount + ADDITIONAL_WIDTH) * CAVITY_SPACE, Color.Black, Color.Yellow)
            connectorRect.Label = connector.SystemId
            _sourceConnectorHeight = connectorRect.Height
        Else
            insertionPoint = New gPoint(_currentPositionX, _currentPositionY - DESTINATION_CONNECTOR_WIDTH * CAVITY_SPACE)
            connectorRect = DrawRectangle(insertionPoint, (cavityCount + ADDITIONAL_WIDTH) * CAVITY_SPACE, DESTINATION_CONNECTOR_WIDTH * CAVITY_SPACE, Color.Black, Color.Yellow)
            _connectorWidth = connectorRect.Width
        End If

        If isSourceConnector Then
            insertionPoint = New gPoint(_currentPositionX + CONNECTOR_NAME_OFFSET, _currentPositionY + (cavityCount + ADDITIONAL_WIDTH) * CAVITY_SPACE / HALF_OR_DOUBLE_VALUE)
            connectorName = DrawText(insertionPoint, connector.Id, Color.Black, , NAME_TEXT_WEIGHT, DegreesToRadians(TEXT_ROTATION_ANGLE))
        Else
            insertionPoint = New gPoint(_currentPositionX + (cavityCount + ADDITIONAL_WIDTH) * CAVITY_SPACE / HALF_OR_DOUBLE_VALUE, _currentPositionY - CONNECTOR_NAME_OFFSET)
            connectorName = DrawText(insertionPoint, connector.Id, Color.Black, , NAME_TEXT_WEIGHT)
        End If

        If connector.Description IsNot Nothing Then
            If isSourceConnector Then
                insertionPoint = New gPoint(_currentPositionX + CONNECTOR_DESCRIPTION_OFFSET, _currentPositionY + (cavityCount + ADDITIONAL_WIDTH) * CAVITY_SPACE / HALF_OR_DOUBLE_VALUE)
                connectorDiscription = DrawText(insertionPoint, Replace("[*]", "*", connector.Description), Color.Black, , DESCRIPTION_TEXT_WEIGHT, DegreesToRadians(TEXT_ROTATION_ANGLE))
            Else
                insertionPoint = New gPoint(_currentPositionX + (cavityCount + ADDITIONAL_WIDTH) * CAVITY_SPACE / HALF_OR_DOUBLE_VALUE, _currentPositionY - CONNECTOR_DESCRIPTION_OFFSET)
                connectorDiscription = DrawText(insertionPoint, Replace("[*]", "*", connector.Description), Color.Black, , DESCRIPTION_TEXT_WEIGHT)
            End If
        End If

        If isSourceConnector Then
            ReviseRectHeight(componentRect, componentName, componentDiscription, connectorRect, connectorName, connectorDiscription)
        Else
            ReviseRectWidth(componentRect, componentName, componentDiscription, connectorRect, connectorName, connectorDiscription)

            _destinationConnectorRectBox.Add(connector.SystemId, connectorRect.BoundingBox)
            If componentRect IsNot Nothing Then _destinationComponentRectBox.Add(connector.SystemId, componentRect.BoundingBox)
        End If

        With _activeDoc
            .ActiveLayOut.Entities.AddItem(componentRect)
            .ActiveLayOut.Entities.AddItem(componentName)
            .ActiveLayOut.Entities.AddItem(componentDiscription)
            .ActiveLayOut.Entities.AddItem(connectorRect)
            .ActiveLayOut.Entities.AddItem(connectorName)
            .ActiveLayOut.Entities.AddItem(connectorDiscription)
        End With
    End Sub

    Private Sub DrawCavity(cavities As List(Of Cavity_occurrence), cavityCount As Integer, isSourceConnector As Boolean, Optional _destinationCavity As Dictionary(Of String, Cavity_occurrence) = Nothing)
        Dim positionX As Double
        Dim positionY As Double
        Dim cavitySpace As Double
        Dim emptyCavity As New List(Of Cavity_occurrence)
        Dim count As Integer = 0

        If isSourceConnector Then
            positionX = _currentPositionX + ADDITIONAL_CAVITY_NO * CAVITY_SPACE
            cavitySpace = _sourceConnectorHeight / (cavityCount + 1)
            _wireInformationMapper = New Dictionary(Of String, WireInformation)
        Else
            positionY = _currentPositionY - DESTINATION_OFFSET_Y * CAVITY_SPACE
            cavitySpace = _connectorWidth / (cavityCount + 1)
        End If

        If (Not isSourceConnector) Then
            For Each cavity As Cavity_occurrence In cavities
                If _destinationCavity.ContainsValue(cavity) Then
                    For Each destinationCavity As KeyValuePair(Of String, Cavity_occurrence) In _destinationCavity
                        If destinationCavity.Value Is cavity Then
                            count += 1
                            positionX = _currentPositionX + count * cavitySpace

                            DrawCavitySymbol(cavity, positionX, positionY, isSourceConnector)

                            Dim core_occ As Core_occurrence = _kblMapper.GetOccurrenceObject(Of Core_occurrence)(destinationCavity.Key)
                            If core_occ IsNot Nothing Then
                                CreateCableList(destinationCavity.Key, New gPoint(positionX, positionY), cavitySpace, False)
                            End If
                            AddPointToPointWirePointList(New gPoint(positionX, positionY), destinationCavity.Key, _kblMapper.GetConnectorOfCavity(cavity.SystemId))
                        End If
                    Next
                End If
            Next
        End If

        For Each cavity As Cavity_occurrence In cavities
            If _kblMapper.KBLWireCavityMapper.ContainsKey(cavity.SystemId) Then
                For Each cavityWire As KeyValuePair(Of String, List(Of Cavity_occurrence)) In _kblMapper.KBLWireCavityMapper(cavity.SystemId)
                    If isSourceConnector = False Then
                        If _destinationCavity.ContainsKey(cavityWire.Key) = False Then
                            count = count + 1
                            positionX = _currentPositionX + count * cavitySpace

                            DrawCavitySymbol(cavity, positionX, positionY, isSourceConnector, 1)
                            DrawNotConnectedWireInfo(CType(_kblMapper.GetOccurrenceObjectUntyped(cavityWire.Key), IKblWireCoreOccurrence), positionX, positionY)
                        End If
                    Else
                        count = count + 1
                        positionY = _currentPositionY + _sourceConnectorHeight - count * cavitySpace

                        DrawCavitySymbol(cavity, positionX, positionY, isSourceConnector)
                        AddWireInfoToWireInfoMapping(cavityWire.Key)

                        Dim core_occ As Core_occurrence = _kblMapper.GetOccurrenceObject(Of Core_occurrence)(cavityWire.Key)
                        If core_occ IsNot Nothing Then
                            CreateCableList(cavityWire.Key, New gPoint(positionX, positionY), cavitySpace, True)
                        End If

                        AddPointToPointWirePointList(New gPoint(positionX, positionY), cavityWire.Key)
                    End If
                Next
            Else
                emptyCavity.Add(cavity)
            End If
        Next

        For Each cavity As Cavity_occurrence In emptyCavity
            count = count + 1

            If isSourceConnector Then
                positionY = _currentPositionY + _sourceConnectorHeight - count * cavitySpace
            Else
                positionX = _currentPositionX + count * cavitySpace
            End If

            DrawCavitySymbol(cavity, positionX, positionY, isSourceConnector, 2)
        Next
    End Sub

    Private Sub DrawDestinationConnectors()
        For Each destinationConnector As KeyValuePair(Of String, Dictionary(Of String, Cavity_occurrence)) In _destinationConnector
            Dim connector As Connector_occurrence = _kblMapper.GetOccurrenceObject(Of Connector_occurrence)(destinationConnector.Key)
            If (connector IsNot _sourceConnector) AndAlso Not (_inactiveObjects IsNot Nothing AndAlso _inactiveObjects.ContainsValue(KblObjectType.Connector_occurrence, connector.SystemId)) Then
                DrawConnector(connector, False, destinationConnector.Value)
                _currentPositionX = _currentPositionX + _connectorWidth + DESTINATION_CONNECTOR_SPACE * CAVITY_SPACE
            End If
        Next
    End Sub

    Private Function GetParentComponent(connector As Connector_occurrence) As Component_occurrence
        For Each component As Component_occurrence In _kblMapper.GetComponentOccurrences
            If (component.Mounting IsNot Nothing) AndAlso (component.Mounting.Contains(connector.SystemId)) Then
                Return component
                Exit For
            End If
        Next

        Return Nothing
    End Function

    Private Sub ReviseRectWidth(componentRect As vdRect, componentName As vdText, componentDiscription As vdText, connectorRect As vdRect, connectorName As vdText, connectorDiscription As vdText)
        Dim textWidth As Double
        Dim nameTextWidth As Double = connectorName.BoundingBox.Width
        Dim descriptionTextWidth As Double = 0

        If connectorDiscription IsNot Nothing Then
            descriptionTextWidth = connectorDiscription.BoundingBox.Width
        End If

        If descriptionTextWidth > nameTextWidth Then
            textWidth = descriptionTextWidth
        Else
            textWidth = nameTextWidth
        End If

        If connectorRect.Width < textWidth Then
            Dim increaseLength As Double = textWidth + INCREASE_LENGTH - connectorRect.Width

            If componentRect IsNot Nothing Then
                componentRect.Width = componentRect.Width + increaseLength
            End If

            connectorRect.Width = textWidth + INCREASE_LENGTH
            _connectorWidth = connectorRect.Width

            If componentName IsNot Nothing Then
                componentName.InsertionPoint = New gPoint(connectorName.InsertionPoint.x + increaseLength / HALF_OR_DOUBLE_VALUE, componentName.InsertionPoint.y)
            End If

            If componentDiscription IsNot Nothing Then
                connectorDiscription.InsertionPoint = New gPoint(connectorDiscription.InsertionPoint.x + increaseLength / HALF_OR_DOUBLE_VALUE, componentDiscription.InsertionPoint.y)
            End If

            connectorName.InsertionPoint = New gPoint(connectorName.InsertionPoint.x + increaseLength / HALF_OR_DOUBLE_VALUE, connectorName.InsertionPoint.y)

            If connectorDiscription IsNot Nothing Then
                connectorDiscription.InsertionPoint = New gPoint(connectorDiscription.InsertionPoint.x + increaseLength / HALF_OR_DOUBLE_VALUE, connectorDiscription.InsertionPoint.y)
            End If
        End If
    End Sub

    Private Sub ReviseRectHeight(componentRect As vdRect, componentName As vdText, componentDiscription As vdText, connectorRect As vdRect, connectorName As vdText, connectorDiscription As vdText)
        Dim nameTextHeight As Double = connectorName.BoundingBox.Height
        Dim descriptionTextHeight As Double = 0

        If connectorDiscription IsNot Nothing Then descriptionTextHeight = connectorDiscription.BoundingBox.Height

        Dim textHeight As Double
        If descriptionTextHeight > nameTextHeight Then
            textHeight = descriptionTextHeight
        Else
            textHeight = nameTextHeight
        End If

        If connectorRect.Height < textHeight Then
            Dim increaseLength As Double = textHeight + INCREASE_LENGTH - connectorRect.Height
            If componentRect IsNot Nothing Then
                componentRect.Height = componentRect.Height + increaseLength
            End If

            connectorRect.Height = textHeight + INCREASE_LENGTH
            _sourceConnectorHeight = connectorRect.Height

            If componentName IsNot Nothing Then
                Dim x As Double = componentName.InsertionPoint.x
                componentName.InsertionPoint = New gPoint(x, componentName.InsertionPoint.y + increaseLength / HALF_OR_DOUBLE_VALUE)
            End If

            If componentDiscription IsNot Nothing Then
                connectorDiscription.InsertionPoint = New gPoint(connectorDiscription.InsertionPoint.x, componentDiscription.InsertionPoint.y + increaseLength / HALF_OR_DOUBLE_VALUE)
            End If

            connectorName.InsertionPoint = New gPoint(connectorName.InsertionPoint.x, connectorName.InsertionPoint.y + increaseLength / HALF_OR_DOUBLE_VALUE)
            If connectorDiscription IsNot Nothing Then
                connectorDiscription.InsertionPoint = New gPoint(connectorDiscription.InsertionPoint.x, connectorDiscription.InsertionPoint.y + increaseLength / HALF_OR_DOUBLE_VALUE)
            End If
        End If
    End Sub

    Private Sub DrawCavitySymbol(cavity As Cavity_occurrence, positionX As Double, positionY As Double, isSourceConnector As Boolean, Optional cavityType As Integer = 0)
        Dim cavityPoint As New vdCircle
        Dim cavityName As New vdText
        Dim cavityNumber As String = DirectCast(_kblMapper.KBLPartMapper(cavity.Part), Cavity).Cavity_number

        With cavityPoint
            .SetUnRegisterDocument(_activeDoc)
            .setDocumentDefaults()

            .Center = New gPoint(positionX, positionY)
            .Radius = CAVITY_RADIUS

            Select Case cavityType
                Case 2
                    .PenColor.SystemColor = Color.Gray
                Case 1
                    .HatchProperties = New vdHatchProperties()
                    .HatchProperties.FillMode = VdConstFill.VdFillModeSolid
                    .HatchProperties.FillColor.SystemColor = Color.Gray
                    .PenColor.SystemColor = Color.Gray
                Case 0
                    .HatchProperties = New vdHatchProperties()
                    .HatchProperties.FillMode = VdConstFill.VdFillModeSolid
                    .HatchProperties.FillColor.SystemColor = Color.Black
            End Select
        End With

        If isSourceConnector Then
            cavityName = DrawText(New gPoint(positionX - WIRE_OFFSET_X, positionY - WIRE_OFFSET_Y), cavityNumber, Color.Black, VdConstHorJust.VdTextHorLeft, WIRE_NO_WEIGHT)
            If cavityType = 2 Then
                cavityName = DrawText(New gPoint(positionX - WIRE_OFFSET_X, positionY - WIRE_OFFSET_Y), cavityNumber, Color.Gray, VdConstHorJust.VdTextHorLeft, WIRE_NO_WEIGHT)
            End If
        Else
            cavityName = DrawText(New gPoint(positionX, positionY + WIRE_OFFSET_Y * HALF_OR_DOUBLE_VALUE), cavityNumber, Color.Black, VdConstHorJust.VdTextHorCenter, WIRE_NO_WEIGHT)
            If cavityType <> 0 Then
                cavityName = DrawText(New gPoint(positionX, positionY + WIRE_OFFSET_Y * HALF_OR_DOUBLE_VALUE), cavityNumber, Color.Gray, VdConstHorJust.VdTextHorCenter, WIRE_NO_WEIGHT)
            End If
        End If

        With _activeDoc
            .ActiveLayOut.Entities.AddItem(cavityPoint)
            .ActiveLayOut.Entities.AddItem(cavityName)
        End With
    End Sub

    Private Function GetWireCoreInfo(wireCore As IKblWireCoreOccurrence) As WireInformation
        Dim wireInfo As New WireInformation
        Dim csa As String = String.Empty
        Dim color1 As String = String.Empty
        Dim color2 As String = String.Empty
        Dim id As String = String.Empty
        Dim wirecore_Part As IKblWireCorePart = Nothing

        id = wireCore.SystemId
        wireInfo.WireNumber = wireCore.Wire_number

        Select Case wireCore.ObjectType
            Case KblObjectType.Wire_occurrence
                wirecore_Part = _kblMapper.GetPart(Of General_wire)(wireCore.Part)
            Case KblObjectType.Core_occurrence
                wirecore_Part = _kblMapper.GetPart(Of Core)(wireCore.Part)
                wireInfo.CableId = _kblMapper.GetCableOfWireOrCore(wireCore.SystemId).SystemId
            Case Else
                Throw New NotImplementedException($"object type ""{wireCore.ObjectType.ToString}"" not implemented for method ""{NameOf(GetWireCoreInfo)}""")
        End Select

        With wirecore_Part
            If (.Cross_section_area IsNot Nothing) Then
                csa = String.Format("{0} {1}", Math.Round(.Cross_section_area.Value_component, 2), _kblMapper.GetUnit(.Cross_section_area.Unit_component).Unit_name)
                wireInfo.CSA = Math.Round(.Cross_section_area.Value_component, 2)
            End If

            Dim colors As List(Of String) = .GetColours.Split("/".ToCharArray, StringSplitOptions.RemoveEmptyEntries).ToList

            color1 = If(colors.Count >= 1, colors(0), String.Empty)

            Dim wireColor1 As WireColorCode = _wireColorCodeList.FindWireColorCode(color1)
            If (wireColor1 IsNot Nothing) Then
                wireInfo.Color1 = Color.FromArgb(wireColor1.ColorRGB)
            Else
                wireInfo.Color1 = Color.Black
            End If

            If (colors.Count > 1) Then
                color2 = colors(1)

                Dim wireColor2 As WireColorCode = _wireColorCodeList.FindWireColorCode(color2)
                If (wireColor2 IsNot Nothing) Then
                    wireInfo.Color2 = Color.FromArgb(wireColor2.ColorRGB)
                Else
                    wireInfo.Color2 = Color.Black
                End If
            End If
        End With

        Dim assignedModules As String = SchematicsViewStrings.AssignedModules_Tooltip

        For Each [module] As [Module] In _kblMapper.GetModulesOfObject(id)
            Dim moduleInfo As String = String.Empty
            If Not String.IsNullOrEmpty([module]?.Abbreviation) Then
                moduleInfo = String.Format("{0} [{1}]", [module].Abbreviation, [module].Part_number)
            Else
                moduleInfo = [module].Part_number
            End If

            If (assignedModules = SchematicsViewStrings.AssignedModules_Tooltip) Then
                assignedModules &= moduleInfo
            Else
                assignedModules = String.Format("{0}, {1}", assignedModules, moduleInfo)
            End If
        Next

        wireInfo.ToolTip = String.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}", SchematicsViewStrings.WireName_Tooltip, wireInfo.WireNumber, vbCrLf, SchematicsViewStrings.CSA_Tooltip, csa, vbCrLf, SchematicsViewStrings.Color1_Tooltip, color1, vbCrLf, SchematicsViewStrings.Color2_Tooltip, color2, vbCrLf, assignedModules)

        Return wireInfo
    End Function

    Private Function GetWirePenWidth(csa As Double) As Double
        If (csa < 0.35) Then
            Return 0.05
        ElseIf (csa < 0.5) Then
            Return 0.075
        ElseIf (csa < 0.75) Then
            Return 0.1
        ElseIf (csa < 1) Then
            Return 0.125
        ElseIf (csa < 1.5) Then
            Return 0.15
        ElseIf (csa < 2) Then
            Return 0.175
        ElseIf (csa < 5) Then
            Return 0.2
        ElseIf (csa < 10) Then
            Return 0.3
        ElseIf (csa < 15) Then
            Return 0.5
        ElseIf (csa < 20) Then
            Return 0.75
        Else
            Return 1
        End If
    End Function

    Private Sub AddWireInfoToWireInfoMapping(wireID As String)
        Dim connectWire As IKblWireCoreOccurrence = TryCast(_kblMapper.GetOccurrenceObjectUntyped(wireID), IKblWireCoreOccurrence)
        Dim wireInfo As New WireInformation
        If connectWire IsNot Nothing Then
            wireInfo = GetWireCoreInfo(connectWire)
        End If

        If (Not _wireInformationMapper.ContainsKey(wireID)) Then
            _wireInformationMapper.Add(wireID, wireInfo)
        End If
    End Sub

    Private Sub AddPointToPointWirePointList(point As gPoint, wire As String, Optional destinationConnector As Connector_occurrence = Nothing)
        Dim isDestinationPoint As Boolean = False

        If _sourcePointWireMapper.Count <> 0 Then
            If _sourcePointWireMapper.ContainsKey(wire) Then
                Dim wireConnectivity As New WireConnectivity
                With wireConnectivity
                    .SourcePoint = _sourcePointWireMapper(wire)
                    .DestinationPoint = point
                    .WireID = wire
                    .DestinationConnector = destinationConnector
                End With

                _wireConnectivityList.Add(wireConnectivity)
                isDestinationPoint = True
            End If
        End If

        If (Not isDestinationPoint) Then
            _sourcePointWireMapper.Add(wire, point)
        End If
    End Sub

    Private Sub DrawNotConnectedWireInfo(wireCore As IKblWireCoreOccurrence, positionX As Double, positionY As Double)
        Dim wireId As String = wireCore.SystemId
        Dim wireNo As String = wireCore.Wire_number
        Dim line As vdLine = New vdLine
        Dim polyLine As vdPolyline = New vdPolyline
        Dim wireNoText As vdText = New vdText
        Dim wireInfo As WireInformation = GetWireCoreInfo(wireCore)

        With line
            .SetUnRegisterDocument(_activeDoc)
            .setDocumentDefaults()

            .EndPoint = New gPoint(positionX, positionY - NOT_CONNECTOR_WIRE_OFFSET_Y1 * CAVITY_SPACE)
            .LineType = _activeDoc.LineTypes.DPIDash
            .PenColor.SystemColor = Color.Gray
            .PenWidth = 0.05
            .StartPoint = New gPoint(positionX, positionY)
            .XProperties.Add("WireInfo").PropValue = wireInfo
        End With

        Dim vertexList As Vertexes = New Vertexes
        With vertexList
            .Add(New gPoint(positionX - NOT_CONNECTOR_WIRE_OFFSET_X * CAVITY_SPACE, positionY - NOT_CONNECTOR_WIRE_OFFSET_Y1 * CAVITY_SPACE + NOT_CONNECTOR_WIRE_OFFSET_Y2 * CAVITY_SPACE))
            .Add(New gPoint(positionX, positionY - NOT_CONNECTOR_WIRE_OFFSET_Y1 * CAVITY_SPACE))
            .Add(New gPoint(positionX + NOT_CONNECTOR_WIRE_OFFSET_X * CAVITY_SPACE, positionY - NOT_CONNECTOR_WIRE_OFFSET_Y1 * CAVITY_SPACE + NOT_CONNECTOR_WIRE_OFFSET_Y2 * CAVITY_SPACE))
        End With

        polyLine = DrawPolyline(vertexList, wireInfo, Color.Gray, 0.05, True)
        polyLine.Label = wireId

        wireNoText = DrawText(New gPoint(positionX - NOT_CONNECTOR_WIRE_TEXT_OFFSET_X * CAVITY_SPACE, positionY - NOT_CONNECTOR_WIRE_TEXT_OFFSET_Y * CAVITY_SPACE), wireNo, Color.Gray, VdConstHorJust.VdTextHorLeft, 0.25, DegreesToRadians(TEXT_ROTATION_ANGLE))

        With _activeDoc
            .ActiveLayOut.Entities.AddItem(line)
            .ActiveLayOut.Entities.AddItem(polyLine)
            .ActiveLayOut.Entities.AddItem(wireNoText)
        End With
    End Sub

    Private Sub CreateCableList(wireNumber As String, cavityPoint As gPoint, cavitySpace As Double, isSouceCable As Boolean)
        Dim cableId As String = _wireInformationMapper(wireNumber).CableId
        Dim cablePointList As List(Of gPoint) = New List(Of gPoint)

        If isSouceCable Then
            If Not _sourceCable.TryAdd(cableId, cablePointList) Then
                cablePointList = _sourceCable(cableId)
                If cavitySpace - 0.1 < Math.Abs(cablePointList((cablePointList.Count - 1)).y - cavityPoint.y) And Math.Abs(cablePointList((cablePointList.Count - 1)).y - cavityPoint.y) < cavitySpace + 0.1 Then
                    cablePointList.Add(cavityPoint)
                End If
            Else
                cablePointList.Add(cavityPoint)
            End If
        Else
            If Not _destinationCable.TryAdd(cableId, cablePointList) Then
                cablePointList = _destinationCable(cableId)
                If cavitySpace - 0.1 < Math.Abs(cablePointList((cablePointList.Count - 1)).x - cavityPoint.x) And Math.Abs(cablePointList((cablePointList.Count - 1)).x - cavityPoint.x) < cavitySpace + 0.1 Then
                    cablePointList.Add(cavityPoint)
                End If
            Else
                cablePointList.Add(cavityPoint)
            End If
        End If
    End Sub

    Private Sub RemoveUndrawCable()
        Dim removeList As New List(Of String)

        For Each cable As KeyValuePair(Of String, List(Of gPoint)) In _destinationCable
            If cable.Value.Count < 2 Then
                removeList.Add(cable.Key)
            End If
        Next

        For Each removeCable As String In removeList
            _destinationCable.Remove(removeCable)
        Next

        removeList = New List(Of String)

        For Each cable As KeyValuePair(Of String, List(Of gPoint)) In _sourceCable
            If cable.Value.Count < 2 Then removeList.Add(cable.Key)
            If _destinationCable.ContainsKey(cable.Key) Then removeList.Add(cable.Key)
        Next

        For Each removeCable As String In removeList
            _sourceCable.Remove(removeCable)
        Next
    End Sub

    Private Sub DrawCableEllipseAndText(cableName As String, cavityPoint As List(Of gPoint), isSource As Boolean)
        Dim ellipse As vdEllipse = New vdEllipse
        Dim length As Double

        If isSource Then
            length = cavityPoint(0).y - cavityPoint(cavityPoint.Count - 1).y
        Else
            length = cavityPoint(cavityPoint.Count - 1).x - cavityPoint(0).x
        End If

        With ellipse
            .SetUnRegisterDocument(_activeDoc)
            .setDocumentDefaults()

            If isSource Then
                .MinorLength = length * ELLIPSE_HEIGHT
                .MajorLength = ELLIPSE_WIDTH
                .Center = New gPoint(cavityPoint(0).x + ELLIPSE_OFFSET * CAVITY_SPACE, cavityPoint(0).y - length / HALF_OR_DOUBLE_VALUE)
            Else
                .MinorLength = ELLIPSE_WIDTH
                .MajorLength = length * ELLIPSE_HEIGHT
                .Center = New gPoint(cavityPoint(0).x + length / HALF_OR_DOUBLE_VALUE, cavityPoint(0).y - ELLIPSE_OFFSET * CAVITY_SPACE)
            End If
        End With

        _activeDoc.ActiveLayOut.Entities.AddItem(ellipse)

        Dim cableText As vdText = Nothing

        Dim cable As Special_wire_occurrence = _kblMapper.GetOccurrenceObject(Of Special_wire_occurrence)(cableName)
        If isSource Then
            cableText = DrawText(New gPoint(ellipse.Center.x + (ELLIPSE_HEIGHT / 2), ellipse.Center.y), cable.Special_wire_id, Color.Black, VdConstHorJust.VdTextHorCenter, 0.15, DegreesToRadians(TEXT_ROTATION_ANGLE))
        Else
            cableText = DrawText(New gPoint(ellipse.Center.x, ellipse.Center.y - (ELLIPSE_HEIGHT / 2)), cable.Special_wire_id, Color.Black, VdConstHorJust.VdTextHorCenter, 0.15)
        End If

        _activeDoc.ActiveLayOut.Entities.AddItem(cableText)
    End Sub

    Private Function DrawPolyline(vertexList As Vertexes, wireInfo As WireInformation, penColor As Color, penWidth As Double, isDash As Boolean) As vdPolyline
        Dim polyline As vdPolyline = New vdPolyline
        With polyline
            .SetUnRegisterDocument(_activeDoc)
            .setDocumentDefaults()

            If (isDash) Then
                .LineType = _activeDoc.LineTypes.DPIDash
            End If

            .PenColor.SystemColor = penColor
            .PenWidth = penWidth
            .VertexList = vertexList
            .XProperties.Add("WireInfo").PropValue = wireInfo
        End With

        Return polyline
    End Function

    Private Function DrawRectangle(insertionPoint As gPoint, width As Double, height As Double, penColor As Color, hatchFillColor As Color) As vdRect
        Dim rect As vdRect = New vdRect
        With rect
            .SetUnRegisterDocument(_activeDoc)
            .setDocumentDefaults()

            .HatchProperties = New vdHatchProperties()
            .HatchProperties.FillMode = VdConstFill.VdFillModeSolid
            .HatchProperties.FillColor.SystemColor = hatchFillColor

            .Height = height
            .InsertionPoint = insertionPoint
            .Width = width

            .PenColor.SystemColor = penColor
            .PenWidth = 0.1
        End With

        Return rect
    End Function

    Private Function DrawText(insertionPoint As gPoint, textString As String, color As Color, Optional horJustify As VdConstHorJust = VdConstHorJust.VdTextHorCenter, Optional height As Double = 0.25, Optional rotaion As Double = 0) As vdText
        Dim text As vdText = New vdText
        With text
            .SetUnRegisterDocument(_activeDoc)
            .setDocumentDefaults()

            .InsertionPoint = insertionPoint
            .HorJustify = horJustify
            .TextString = textString
            .PenColor.SystemColor = color
            .Height = height
            .Rotation = rotaion
        End With

        Return text
    End Function

    Private Sub OpenUndo()
        _activeDoc.UndoHistory.PushEnable(True)
        _activeDoc.UndoHistory.StoreUndoGroup(True)
    End Sub

    Private Sub CallUndo()
        _activeDoc.UndoHistory.Undo()

        For i As Integer = 0 To _activeDoc.UndoHistory.UndoStackCollection.Count - 1
            _activeDoc.UndoHistory.UndoStackCollection.Pop()
        Next

        _activeDoc.Redraw(True)
    End Sub

    Private Sub CloseUndo()
        _activeDoc.UndoHistory.StoreUndoGroup(False)
        _activeDoc.UndoHistory.PushEnable(False)
    End Sub


    Private Sub ConnectivityView_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        DoDraw()
    End Sub

    Private Sub btnClose_Click(sender As System.Object, e As System.EventArgs) Handles btnClose.Click
        Me.Close()
    End Sub

    Private Sub btnExport_Click(sender As System.Object, e As System.EventArgs) Handles btnExport.Click
        With sfdExport
            .DefaultExt = KnownFile.DXF.Trim("."c)
            .FileName = Regex.Replace(Me.Text, "\W", "_")
            .Filter = "Autodesk DXF file (*.dxf)|*.dxf|JPEG (*.jpg)|*.jpg|PNG (*.png)|*.png|Bitmap (*.bmp)|*.bmp"
            .Title = SchematicsViewStrings.ExportViewToFile_Title

            If (.ShowDialog(Me) = DialogResult.OK) Then
                Try
                    If (VdOpenSave.SaveAs(_activeDoc, sfdExport.FileName)) Then
                        MessageBox.Show(SchematicsViewStrings.ExportSuccess_Msg, [Shared].MSG_BOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Information)
                    Else
                        MessageBoxEx.ShowError(SchematicsViewStrings.ExportProblem_Msg)
                    End If
                Catch ex As Exception
                    ex.ShowMessageBox(String.Format(SchematicsViewStrings.ExportError_Msg, vbCrLf, ex.Message))
                End Try

                _activeDoc.UndoHistory.Clear()
            End If
        End With
    End Sub

    Private Sub btnPrint_Click(sender As System.Object, e As System.EventArgs) Handles btnPrint.Click
        Dim printing As New HarnessAnalyzer.Printing.VdPrinting(_activeDoc)
        printing.DocumentName = Me.Text

        Using printForm As New Printing.PrintForm(printing, Nothing, False)
            printForm.ShowDialog(Me)
        End Using
    End Sub

    Private Sub btnRedraw_Click(sender As System.Object, e As System.EventArgs) Handles btnRedraw.Click
        DoDraw()
    End Sub

    Private Sub uceConnectors_SelectionChanged(sender As Object, e As EventArgs) Handles uceConnectors.SelectionChanged
        If (Me.uceConnectors.Text <> String.Empty) AndAlso (Me.uceConnectors.Text <> _sourceConnector.Id) Then
            _sourceConnector = DirectCast(uceConnectors.SelectedItem.DataValue, Connector_occurrence)

            Me.Text = _caption.Replace("*", _sourceConnector.Id)

            DoDraw()
        End If
    End Sub

    Private Sub uckSimpleWireView_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles uckSimpleWireView.CheckedChanged
        If (Me.uckSimpleWireView.Checked) Then
            OpenUndo()

            For Each polyLine As vdPolyline In _connectionWirePolyLines
                With polyLine
                    .PenColor.SystemColor = Color.Black
                    .PenWidth = 0.1

                    .Update()
                End With
            Next

            CloseUndo()

            _activeDoc.Redraw(True)
        Else
            CallUndo()
        End If
    End Sub

    Private Sub vDraw_Click(sender As Object, e As System.EventArgs) Handles vDraw.Click
        Dim clickPoint As gPoint = _activeDoc.CCS_CursorPos

        For Each connectorRect As KeyValuePair(Of String, Box) In _destinationConnectorRectBox
            If (clickPoint.x > connectorRect.Value.Min.x AndAlso clickPoint.x < connectorRect.Value.Max.x) AndAlso (clickPoint.y > connectorRect.Value.Min.y AndAlso clickPoint.y < connectorRect.Value.Max.y) Then
                RaiseEvent ConnectivityViewMouseClick(sender, New InformationHubEventArgs(_kblMapper.Id, KblObjectType.Connector_occurrence, connectorRect.Key))

                Exit Sub
            End If
        Next

        For Each componentRect As KeyValuePair(Of String, Box) In _destinationComponentRectBox
            If (clickPoint.x > componentRect.Value.Min.x AndAlso clickPoint.x < componentRect.Value.Max.x) AndAlso (clickPoint.y > componentRect.Value.Min.y AndAlso clickPoint.y < componentRect.Value.Max.y) Then
                RaiseEvent ConnectivityViewMouseClick(sender, New InformationHubEventArgs(_kblMapper.Id, KblObjectType.Component_occurrence, componentRect.Key))

                Exit Sub
            End If
        Next

        Dim hitEntity As VectorDraw.Professional.vdPrimaries.vdFigure = _activeDoc.ActiveLayOut.GetEntityFromPoint(_activeDoc.ActiveRender.View2PixelI(clickPoint), 6, False, VectorDraw.Professional.vdObjects.vdDocument.LockLayerMethodEnum.DisableAll)
        If (hitEntity IsNot Nothing) AndAlso (hitEntity.Label <> String.Empty) Then
            If (TypeOf hitEntity Is vdPolyline) Then
                RaiseEvent ConnectivityViewMouseClick(sender, New InformationHubEventArgs(_kblMapper.Id, KblObjectType.Wire_occurrence, hitEntity.Label))
            Else
                RaiseEvent ConnectivityViewMouseClick(sender, New InformationHubEventArgs(_kblMapper.Id, KblObjectType.Connector_occurrence, hitEntity.Label))
            End If
        End If
    End Sub

    Private Sub vDraw_DoubleClick(sender As Object, e As System.EventArgs) Handles vDraw.DoubleClick
        Dim clickPoint As gPoint = _activeDoc.CCS_CursorPos

        For Each connectorRect As KeyValuePair(Of String, Box) In _destinationConnectorRectBox
            If (clickPoint.x > connectorRect.Value.Min.x AndAlso clickPoint.x < connectorRect.Value.Max.x) AndAlso (clickPoint.y > connectorRect.Value.Min.y AndAlso clickPoint.y < connectorRect.Value.Max.y) Then
                Me.uceConnectors.Text = _kblMapper.GetConnectorOccurrence(connectorRect.Key).Id

                Exit Sub
            End If
        Next

        For Each componentRect As KeyValuePair(Of String, Box) In _destinationComponentRectBox
            If (clickPoint.x > componentRect.Value.Min.x AndAlso clickPoint.x < componentRect.Value.Max.x) AndAlso (clickPoint.y > componentRect.Value.Min.y AndAlso clickPoint.y < componentRect.Value.Max.y) Then
                Me.uceConnectors.Text = _kblMapper.GetConnectorOccurrence(componentRect.Key).Id

                Exit Sub
            End If
        Next
    End Sub

    Private Sub vDraw_MouseMove(sender As Object, e As MouseEventArgs) Handles vDraw.MouseMove
        Static mpos As System.Drawing.Point
        If Not (mpos.X - e.Location.X = 0 AndAlso mpos.Y - e.Location.Y = 0) Then
            Dim currentCursorPoint As gPoint = _activeDoc.CCS_CursorPos
            Dim hitEntity As VectorDraw.Professional.vdPrimaries.vdFigure = _activeDoc.ActiveLayOut.GetEntityFromPoint(_activeDoc.ActiveRender.View2PixelI(currentCursorPoint), 6, False, VectorDraw.Professional.vdObjects.vdDocument.LockLayerMethodEnum.DisableAll)

            Me.uttmConnectivityView.HideToolTip()

            If (hitEntity IsNot Nothing) AndAlso (hitEntity.XProperties.FindName("WireInfo") IsNot Nothing) Then
                Dim ttInfo As UltraToolTipInfo = Me.uttmConnectivityView.GetUltraToolTip(Me.vDraw)
                ttInfo.ToolTipText = DirectCast(hitEntity.XProperties.FindName("WireInfo").PropValue, WireInformation).ToolTip
                ttInfo.ToolTipImage = ToolTipImage.Info
                ttInfo.ToolTipTitle = SchematicsViewStrings.WireCaption_Tooltip
                ttInfo.Enabled = DefaultableBoolean.False

                Me.uttmConnectivityView.ShowToolTip(Me.vDraw)
            End If
        End If
        mpos = e.Location
    End Sub

End Class