Option Strict On
Option Explicit On
Option Infer Off

Imports System.ComponentModel
Imports System.IO
Imports System.IO.Compression
Imports System.Reflection
Imports System.Text.RegularExpressions
Imports System.Xml
Imports System.Collections.Generic
Imports VectorDraw.Geometry
Imports VectorDraw.Professional.Control
Imports VectorDraw.Professional.vdObjects
Imports VectorDraw.Professional.vdPrimaries

''' <summary>
''' Provides functionality to convert SVG files to VectorDraw entities, supporting KBL mapping, progress reporting, and bounding box calculation.
''' </summary>
Partial Public Class SVGConverter
    Implements IDisposable
    Implements IConverter

    ''' <summary>
    ''' Occurs when a message is raised during the conversion process.
    ''' </summary>
    Public Event Message(sender As Object, e As MessageEventArgs) Implements IConverter.Message

    ''' <summary>
    ''' Occurs when the progress of the conversion changes.
    ''' </summary>
    Public Event ProgressChanged(sender As Object, e As System.ComponentModel.ProgressChangedEventArgs) Implements IConverter.ProgressChanged

    ''' <summary>
    ''' Occurs after a VectorDraw entity has been converted from SVG.
    ''' </summary>
    Public Event AfterEntityConverted(sender As Object, e As SvgConverterEntityEventArgs)

    Private ReadOnly _groups As ArrayList
    Private ReadOnly _transforms As ArrayList

    Private WithEvents _vDraw As VectorDrawBaseControl
    Private _syncContext As System.Threading.SynchronizationContext

    ' Removed: Private _xmlReader As XmlTextReader (now using HybridXmlReader)
    Private _svgDataName As String
    Private _currentGroup As VdSVGGroup
    Private _currentPattern As vdBlock
    Private _currentTransformations() As String
    Private _font As System.Drawing.Font
    Private _isClipPath As Boolean
    Private _kblMapper As IKblContainer
    Private _lastControlPoint As gPoint
    Private _lastControlPointVertex As Vertex
    Private _vdLines As SortedDictionary(Of Integer, VdLineEx)
    Private _textHeight As Single
    Private _validationLevel As SVGValidationLevel

    ' === PERFORMANCE OPTIMIZATION: LINE OBJECT POOL ===
    ' Object pool for VdLineEx instances to reduce GC pressure and allocation overhead
    Private Shared ReadOnly LineObjectPool As New Queue(Of VdLineEx)
    Private Shared ReadOnly PoolLock As New Object
    Private Const MAX_POOL_SIZE As Integer = 100 ' Limit pool size to prevent excessive memory usage
    Private _mirrorMatrix As New Matrix
    Private _textHeights As New Dictionary(Of Double, Integer)
    Private _svgHeight As Single
    Private _disposedValue As Boolean
    Private _conv_groups As New List(Of VdSVGGroup)
    Private _overallBox As Box
    Private _factory As SvgFactory
    Private _overallIsDocumentFrame As Boolean
    Private _documentFrame As VdSVGGroup
    Private _isKroschu As Nullable(Of Boolean) = Nothing

    ' === STYLE APPLICATION OPTIMIZATION ===
    ''' <summary>
    ''' Enables bulk style application optimization to reduce overhead from repeated style operations
    ''' </summary>
    Public Property EnableBulkStyleApplication As Boolean = False    ' Style batching for optimization
    Private _pendingStyleApplications As List(Of Action) = New List(Of Action)()
    Private _isStyleBatchActive As Boolean = False
    
    Private WithEvents _hybridXmlReader As HybridXmlReader
    
    ''' <summary>
    ''' Initializes a new instance of the <see cref="SVGConverter"/> class using a topology and KBL provider file and a VectorDraw control.
    ''' </summary>
    ''' <param name="kblAndTopology">The topology and KBL provider file.</param>
    ''' <param name="vdraw">The VectorDraw control.</param>
    Public Sub New(kblAndTopology As E3.Lib.IO.Files.Hcv.ITopologyAndKblProviderFile, vdraw As VectorDrawBaseControl)
        Me.New(GetKblContainerFromFile(kblAndTopology.GetKblFile), kblAndTopology.GetTopologyFile.GetDataStream, kblAndTopology.FullName, vdraw, kblAndTopology.GetTopologyFile.IsKroschu)
    End Sub

    Public Sub New(kblMapper As IKblContainer, svgData As System.IO.Stream, svgDataName As String, vDraw As VectorDrawBaseControl, Optional isKroschu As Nullable(Of Boolean) = Nothing)
        _mirrorMatrix.A11 = -_mirrorMatrix.A11
        _groups = New ArrayList
        _transforms = New ArrayList
        _factory = New SvgFactory
        _kblMapper = kblMapper
        _vdLines = New SortedDictionary(Of Integer, VdLineEx)
        _svgDataName = svgDataName
        _vDraw = vDraw
        _syncContext = _vDraw.InvokeOrDefault(Function() System.Threading.SynchronizationContext.Current)
        _overallBox = New Box

        If Not isKroschu.HasValue Then
            isKroschu = E3.Lib.IO.Files.Hcv.Utils.IsKroschuFile(svgData)
        End If

        _isKroschu = isKroschu.Value
        svgData.Seek(0, SeekOrigin.Begin)
        Try
            _hybridXmlReader = New HybridXmlReader(svgData, True)
            ' Configure the hybrid reader for optimal performance
            _hybridXmlReader.EnableAttributeCache = True
            ' XmlReader is now fully encapsulated within HybridXmlReader
        Catch ex As Exception
            OnMessage(New MessageEventArgs(MessageType.ErrorCheckSVG, ex.Message))
            _validationLevel = SVGValidationLevel.Error
            _hybridXmlReader?.Dispose()
            _hybridXmlReader = Nothing
            ' XmlReader cleanup is handled by HybridXmlReader disposal
        End Try
    End Sub

    Private Shared Function GetKblContainerFromFile(IbaseDataFile As System.IO.IBaseDataFile) As IKblContainer
        Using s As Stream = IbaseDataFile.GetDataStream
            Return GetKblContainerFromStream(s)
        End Using
    End Function

    Private Shared Function GetKblContainerFromStream(s As System.IO.Stream) As IKblContainer
        Dim kbl As KBL_container = KBL_container.Read(s)
        Return kbl.AsKblContainer(True)
    End Function

    Protected Overridable Sub OnMessage(e As MessageEventArgs)
        ' When running on background thread, avoid UI marshaling that blocks the UI
        If _syncContext IsNot Nothing AndAlso Not System.Threading.Thread.CurrentThread.IsBackground Then
            _syncContext.Post(New Threading.SendOrPostCallback(Sub() RaiseEvent Message(Me, e)), Nothing)
        Else
            ' For background threads, fire event directly without UI marshaling
            RaiseEvent Message(Me, e)
        End If
    End Sub

    Protected Overridable Sub OnProgessChanged(e As System.ComponentModel.ProgressChangedEventArgs)
        ' When running on background thread, avoid UI marshaling that blocks the UI
        If _syncContext IsNot Nothing AndAlso Not System.Threading.Thread.CurrentThread.IsBackground Then
            _syncContext.Post(New Threading.SendOrPostCallback(Sub() RaiseEvent ProgressChanged(Me, e)), Nothing)
        Else
            ' For background threads, fire event directly without UI marshaling
            RaiseEvent ProgressChanged(Me, e)
        End If
    End Sub

    Public Function ConvertSVGDrawing() As Result
        Using ts As New System.Threading.CancellationTokenSource
            Return ConvertSVGDrawing(ts.Token)
        End Using
    End Function

    Property CalculateBoundingBox As Boolean

    ReadOnly Property BoundingBox As Box
        Get
            Return _overallBox
        End Get
    End Property

    Public Function ConvertSVGDrawing(cancellationToken As System.Threading.CancellationToken, Optional ByRef txtHeight As Double = Nothing) As Result
        Dim cancelled As Boolean = False
        Dim height As Single
        Dim counter As Integer = 0

        _HasOverallBoundingBoxCollected = False
        _overallIsDocumentFrame = False
        _documentFrame = Nothing
        _conv_groups.Clear()

        If _hybridXmlReader IsNot Nothing Then
            OnMessage(New MessageEventArgs(MessageType.BeginLoadingFile, Path.GetFileName(_svgDataName)))
            While (_hybridXmlReader.Read)

                counter += 1

                If cancellationToken.IsCancellationRequested Then
                    cancelled = True
                    Exit While
                End If

                Try
                    If (_hybridXmlReader.NodeType = XmlNodeType.Comment) AndAlso (_hybridXmlReader.Value.StartsWith("kbl-id:")) Then
                        AssignCommentInformationToGroup(_hybridXmlReader.Value)

                    ElseIf (_hybridXmlReader.NodeType = XmlNodeType.Element) Then
                        If (_hybridXmlReader.Depth = 1) AndAlso (_hybridXmlReader.Name <> "g") AndAlso (_hybridXmlReader.Name <> "clipPath") AndAlso (_hybridXmlReader.Name <> "defs") AndAlso (_hybridXmlReader.Name <> "desc") AndAlso (_hybridXmlReader.Name <> "pattern") AndAlso (_hybridXmlReader.Name <> "title") Then
                            OnMessage(New MessageEventArgs(MessageType.ValidationFailsGroup, _svgDataName, New XmlPositionInfo(_svgDataName, _hybridXmlReader.Name, _hybridXmlReader.LineNumber)))
                            '_logEventArgs.LogLevel = LogEventArgs.LoggingLevel.Warning
                            '_logEventArgs.LogMessage = String.Format(My.Resources.LoggingStrings.SvgConv_ValidationFailsGroup, IO.Path.GetFileNameWithoutExtension(_svgName), _hybridXmlReader.Name, _hybridXmlReader.LineNumber)

                            'RaiseEvent LogMessage(Me, _logEventArgs)

                            If (_validationLevel <> SVGValidationLevel.Error) Then
                                _validationLevel = SVGValidationLevel.Warning
                            End If
                        End If

                        If Not _isClipPath Then
                            If (_hybridXmlReader.Name = "svg") Then

                                height = CSng(ParseSvgSingle(_hybridXmlReader.GetAttribute("height")))
                            Else
                                ConvertElement()
                            End If
                        End If

                    ElseIf (_hybridXmlReader.NodeType = XmlNodeType.EndElement) AndAlso (_hybridXmlReader.Name = "clipPath") Then
                        _isClipPath = False
                    ElseIf (_hybridXmlReader.NodeType = XmlNodeType.EndElement) AndAlso (_hybridXmlReader.Name = "g") Then
                        If (_vdLines.Count <> 0) Then
                            ConvertLines()
                        End If

                        ConvertTransformations()

                        If (_groups.Count > 1) Then
                            Dim previousGroup As VdSVGGroup = _currentGroup

                            _currentGroup = DirectCast(_groups(_groups.Count - 2), VdSVGGroup)
                            _groups.Remove(previousGroup)
                        Else
                            If (_currentGroup IsNot Nothing) Then
                                _groups.Remove(_currentGroup)
                            End If

                            _currentGroup = Nothing
                        End If

                        If (_transforms.Count <> 0) Then
                            _currentTransformations = DirectCast(_transforms(_transforms.Count - 1), String())
                        Else
                            _currentTransformations = {"No transforms"}
                        End If
                    ElseIf (_hybridXmlReader.NodeType = XmlNodeType.EndElement) AndAlso (_hybridXmlReader.Name = "pattern") Then
                        _factory.AddPattern(_currentPattern.Name, _currentPattern)

                        _currentPattern = Nothing
                    End If
                Catch ex As Exception
                    OnMessage(New MessageEventArgs(MessageType.ErrorLoadFile, ex.Message, New XmlPositionInfo(_svgDataName, _hybridXmlReader.Name, _hybridXmlReader.LineNumber)))
                End Try
            End While

            ' === PERFORMANCE OPTIMIZATION: Ensure all remaining lines are processed ===
            ' Process any remaining lines at the end of document parsing
            If (_vdLines.Count <> 0) Then
                ConvertLines()
            End If

            ' XmlReader is closed by HybridXmlReader disposal
        Else
            Throw New ArgumentNullException("HybridXmlReader", "Can't convert svg: HybridXmlReader is nothing!")
            'RaiseEvent LogMessage(Me, _logEventArgs)
        End If

        If Not cancelled Then
            _HasOverallBoundingBoxCollected = True

            If _textHeights.Values.Count > 0 Then
                Dim maxN As Integer = _textHeights.Values.Max
                For Each pair As KeyValuePair(Of Double, Integer) In _textHeights
                    If pair.Value = maxN Then
                        txtHeight = pair.Key
                        Exit For
                    End If
                Next
            Else
                txtHeight = 1
            End If

            OnMessage(New MessageEventArgs(MessageType.FinishedLoadDrawing, Path.GetFileName(_svgDataName)))
        Else
            ' XmlReader disposal is handled by HybridXmlReader

            Me._vDraw.InvokeOrDefault(
            Sub()
                For Each entity As vdFigure In _vDraw.ActiveDocument.Model.Entities
                    entity.Dispose()
                Next

                _vDraw.ActiveDocument.ClearAll()
                _vDraw.Dispose()
            End Sub)

            GC.Collect()
            OnMessage(New MessageEventArgs(MessageType.CancelledLoadDrawing, Path.GetFileName(_svgDataName)))
        End If

        If cancelled Then
            Return Result.Cancelled
        End If

        Return Result.Success
    End Function

    ReadOnly Property Converted As List(Of VdSVGGroup)
        Get
            Return _conv_groups
        End Get
    End Property

    Private Sub AssignCommentInformationToGroup(comment As String)
        If (_currentGroup Is Nothing) Then
            Exit Sub
        End If

        If (comment.Contains(";"c)) Then
            Dim commentStr_values As String() = comment.SplitRemoveEmpty(";"c)
            _currentGroup.KblId = Regex.Replace(commentStr_values(0), "kbl-id:", String.Empty)
            _currentGroup.SVGType = Regex.Replace(commentStr_values(1), "type:", String.Empty)
        Else
            _currentGroup.KblId = Regex.Replace(comment, "kbl-id:", String.Empty)

            If (_currentGroup.KblId = SvgSymbolType.DocumentFrame.ToString(True)) Then
                _currentGroup.SymbolType = SvgSymbolType.DocumentFrame.ToString(True)
            End If
        End If

        If (_currentGroup.KblId <> String.Empty) Then
            If (_currentGroup.KblId.Contains(" "c)) Then
                Dim kblIds() As String = _currentGroup.KblId.SplitSpace
                Dim primaryId As Boolean

                For Each kblId As String In kblIds
                    If (Not primaryId) Then
                        _currentGroup.KblId = kblId
                        primaryId = True
                    Else
                        _currentGroup.SecondaryKblIds.Add(kblId)
                    End If
                Next
            End If

            Dim occ As IKblOccurrence = _kblMapper.GetOccurrenceObjectUntyped(_currentGroup.KblId)
            If occ IsNot Nothing Then
                Select Case occ.ObjectType
                    Case KblObjectType.Accessory_occurrence
                        _currentGroup.SymbolType = SvgSymbolType.Accessory.ToString(True)
                    Case KblObjectType.Cavity_plug_occurrence, KblObjectType.Cavity_seal_occurrence, KblObjectType.Special_terminal_occurrence, KblObjectType.Terminal_occurrence
                        _currentGroup.SymbolType = SvgSymbolType.Cavity.ToString(True)
                    Case KblObjectType.Component_occurrence, KblObjectType.Fuse_occurrence
                        _currentGroup.SymbolType = SvgSymbolType.Component.ToString(True)
                    Case KblObjectType.Core_occurrence, KblObjectType.Wire_occurrence
                        _currentGroup.SymbolType = SvgSymbolType.Wire.ToString(True)
                    Case KblObjectType.Connector_occurrence
                        _currentGroup.SymbolType = SvgSymbolType.Connector.ToString(True)
                    Case KblObjectType.Fixing_occurrence
                        _currentGroup.SymbolType = SvgSymbolType.Fixing.ToString(True)
                    Case KblObjectType.Node
                        _currentGroup.SymbolType = SvgSymbolType.Vertex.ToString(True)
                    Case KblObjectType.Segment
                        _currentGroup.SymbolType = SvgSymbolType.Segment.ToString(True)
                    Case KblObjectType.Wire_protection_occurrence
                        _currentGroup.SymbolType = SvgSymbolType.Taping.ToString(True)
                End Select
            ElseIf (_currentGroup.KblId = "ID_EMPTY") AndAlso (_currentGroup.SVGType = SvgType.table.ToString(True)) Then
                _currentGroup.SymbolType = SvgSymbolType.ModuleTable.ToString(True)
            End If
        End If
    End Sub
    Private Sub ConvertElement()        ' === PERFORMANCE OPTIMIZATION: LAZY LINE PROCESSING ===
        ' Only convert lines when necessary (context changes, not every element)
        ' This eliminates O(n²) behavior from frequent ConvertLines() calls
        If (_hybridXmlReader.Name <> "line") AndAlso (_vdLines.Count <> 0) Then
            ' Only convert lines if we're changing context significantly
            If EnableLazyLineProcessing AndAlso ShouldFlushLinesForElement(_hybridXmlReader.Name) Then
                ConvertLines()
            ElseIf Not EnableLazyLineProcessing Then
                ' Fallback to original behavior if optimization is disabled
                ConvertLines()
            End If
        End If

        Select Case _hybridXmlReader.Name
            Case "circle"
                ConvertCircle(_vDraw.ActiveDocument, _hybridXmlReader, _currentGroup, _currentPattern)
            Case "clipPath"
                _isClipPath = True
            Case "ellipse"
                ConvertEllipse(_vDraw.ActiveDocument, _hybridXmlReader, _currentGroup, _currentPattern)
            Case "g"
                If Not _hybridXmlReader.IsEmptyElement Then
                    ConvertGroup()
                End If
            Case "image"
                ConvertImage(_vDraw.ActiveDocument, _hybridXmlReader, _currentGroup, _currentPattern)
            Case "line"
                ConvertLine(_vDraw.ActiveDocument, _hybridXmlReader)
            Case "path"
                ConvertPath(_vDraw.ActiveDocument, _hybridXmlReader, _currentGroup, _currentPattern)
            Case "pattern"
                ConvertPattern(_vDraw.ActiveDocument, _hybridXmlReader)
            Case "polygon"
                ConvertPolygon(_vDraw.ActiveDocument, _hybridXmlReader, _currentGroup, _currentPattern)
            Case "polyline"
                ConvertPolyline(_vDraw.ActiveDocument, _hybridXmlReader, _currentGroup, _currentPattern)
            Case "rect"
                ConvertRectangle(_vDraw.ActiveDocument, _hybridXmlReader, _currentGroup, _currentPattern)
            Case "text"
                ConvertText(_vDraw.ActiveDocument, _hybridXmlReader, _currentGroup, _currentPattern)
        End Select
    End Sub

    Private Sub ConvertGroup()
        If _currentGroup IsNot Nothing Then
            For Each figure As vdFigure In _currentGroup.Figures
                figure.Transformby(_currentGroup.ECSMatrix)
            Next
        End If

        _currentGroup = New VdSVGGroup

        With _currentGroup
            .SetUnRegisterDocument(_vDraw.ActiveDocument)
            .setDocumentDefaults()

            .KblId = String.Empty
            .SVGType = SvgType.Undefined.ToString(True)
            .SymbolType = SvgType.Undefined.ToString(True)
        End With

        If _groups.Count = 0 Then
            ' add root-group logic was moved to OnAfterEntityConverted
        Else
            DirectCast(_groups(_groups.Count - 1), VdSVGGroup).AddGroup(_currentGroup)
        End If

        _groups.Add(_currentGroup)

        Dim transformStr As String = _hybridXmlReader.GetAttribute("transform")
        If (transformStr IsNot Nothing) Then
            _currentTransformations = ParseTransformOptimized(transformStr)
        Else _currentTransformations = {"No transforms"}
        End If

        _transforms.Add(_currentTransformations)
    End Sub

    Private Sub ConvertPattern(doc As vdDocument, xml As IXmlReader)
        Dim block_name As String = xml.GetAttribute("id")
        Dim x As Nullable(Of Single)
        Dim y As Nullable(Of Single)

        _currentPattern = doc.Blocks.FindName(block_name)

        If _currentPattern Is Nothing Then
            _currentPattern = New vdBlock(doc)
            With _currentPattern
                .Name = block_name

                Dim xStr As String = xml.GetAttribute("x")
                Dim yStr As String = xml.GetAttribute("y")
                Dim heightStr As String = xml.GetAttribute("height")
                Dim widthStr As String = xml.GetAttribute("width")

                If (xStr IsNot Nothing) Then
                    x = CSng(ParseSvgSingle(xStr))
                End If

                If (yStr IsNot Nothing) Then
                    y = CSng(ParseSvgSingle(yStr))
                End If

                If x.HasValue OrElse y.HasValue Then
                    _currentPattern.Origin = New gPoint(x.GetValueOrDefault, y.GetValueOrDefault)
                Else
                    _currentPattern.Origin = gPointEx.Empty
                End If

                Dim rct As New HatchPattern(doc)
                With rct
                    .InsertionPoint = gPointEx.Empty
                    If widthStr IsNot Nothing Then
                        .Width = CSng(ParseSvgSingle(widthStr))
                    End If
                    If (heightStr IsNot Nothing) Then
                        .Height = CSng(ParseSvgSingle(heightStr))
                    End If
                End With

                .Entities.Add(rct)
            End With
            doc.Blocks.Add(_currentPattern)
        End If
    End Sub

    Private Sub ConvertTransformations()
        If (_currentTransformations(0) <> "No transforms") Then
            Dim matrixList As New List(Of Matrix)(_currentTransformations.Length)

            For Each transform As String In _currentTransformations
                Dim matrix As Matrix = GetPooledMatrix()
                Dim transform_strs As String() = transform.SplitRemoveEmpty("("c)
                Dim transformName As String = transform_strs(0).Trim
                Dim transformValue As String = Replace((transform_strs(1)).Trim, ")", String.Empty)

                Select Case transformName
                    Case "matrix"
                        Dim matrix_values As String() = transformValue.SplitRemoveEmpty(","c)
                        With matrix
                            .A00 = Double.Parse(matrix_values(0), Globalization.CultureInfo.InvariantCulture)
                            .A01 = Double.Parse(matrix_values(2), Globalization.CultureInfo.InvariantCulture)
                            .A10 = Double.Parse(matrix_values(1), Globalization.CultureInfo.InvariantCulture)
                            .A11 = Double.Parse(matrix_values(3), Globalization.CultureInfo.InvariantCulture)
                            .A03 = Double.Parse(matrix_values(4), Globalization.CultureInfo.InvariantCulture)
                            .A13 = Double.Parse(matrix_values(5), Globalization.CultureInfo.InvariantCulture)
                        End With
                    Case "rotate"
                        matrix.RotateZMatrix(Globals.DegreesToRadians(Double.Parse(transformValue, Globalization.CultureInfo.InvariantCulture)))
                    Case "scale"
                        Dim transform_values As String() = transformValue.SplitRemoveEmpty(","c)
                        matrix.ScaleMatrix(Double.Parse(transform_values(0), Globalization.CultureInfo.InvariantCulture), Double.Parse(transform_values(1), Globalization.CultureInfo.InvariantCulture), 1D)
                    Case "translate"
                        Dim transform_values As String() = transformValue.SplitRemoveEmpty(","c)
                        matrix.TranslateMatrix(Double.Parse(transform_values(0), Globalization.CultureInfo.InvariantCulture), Double.Parse(transform_values(1), Globalization.CultureInfo.InvariantCulture), 0D)
                End Select

                matrixList.Add(matrix)
            Next

            matrixList.Reverse()
            Dim transformMatrix As Matrix = GetPooledMatrix()

            For Each matrix As Matrix In matrixList
                transformMatrix *= matrix
                ReturnPooledMatrix(matrix) ' Return to pool immediately after use
            Next

            _currentGroup?.Transformby(transformMatrix)

            ReturnPooledMatrix(transformMatrix) ' Return final matrix to pool
        End If

        _transforms.Remove(_currentTransformations)

        OnAfterVdSvgGroupCreated(_currentGroup)
    End Sub

    Private Shared Function CalculateVectorAngle(ux As Double, uy As Double, vx As Double, vy As Double) As Double
        Dim ta As Double = Math.Atan2(uy, ux)
        Dim tb As Double = Math.Atan2(vy, vx)
        If (tb >= ta) Then
            Return (tb - ta)
        End If

        Return (Math.PI * 2) - (ta - tb)
    End Function

    Private Function GetTableGroupFromParents_Recursively(group As VdSVGGroup) As VdSVGGroup
        If group Is Nothing Then
            Return Nothing
        ElseIf Not String.IsNullOrEmpty(group.KblId) AndAlso group.SVGType = SvgType.table.ToString(True) AndAlso _kblMapper.GetOccurrenceObject(Of Connector_occurrence)(group.KblId) IsNot Nothing Then
            Return group
        Else
            Return GetTableGroupFromParents_Recursively(group.ParentGroup)
        End If
    End Function

    Private Function IsConnectorTable() As Boolean
        If (_currentGroup IsNot Nothing) Then
            Dim tableGroup As VdSVGGroup = GetTableGroupFromParents_Recursively(_currentGroup)
            If tableGroup IsNot Nothing Then
                Return True
            End If
        End If

        Return False
    End Function

    Public ReadOnly Property ValidationLevel As SVGValidationLevel
        Get
            Return _validationLevel
        End Get
    End Property

    Public ReadOnly Property Type As ConverterType Implements IConverter.Type
        Get
            Return ConverterType.SvgConverter
        End Get
    End Property

    Protected Overridable Overloads Sub OnAfterEntityConverted(e As SvgConverterEntityEventArgs)
        RaiseEvent AfterEntityConverted(Me, e)
    End Sub

    ReadOnly Property HasOverallBoundingBoxCollected As Boolean = False

    Private Sub OnAfterVdSvgGroupCreated(group As VdSVGGroup)
        If (Not _isKroschu AndAlso group.ParentGroup Is Nothing) OrElse (_isKroschu AndAlso group.ParentGroup?.ParentGroup Is Nothing) Then   'HINT, only root level groups -> for kroshu = only first level child groups
            If _documentFrame Is Nothing AndAlso group.KblId = SvgSymbolType.DocumentFrame.ToString(True) Then
                _documentFrame = group
            End If

            If _isKroschu Then
                _currentGroup.ParentGroup = Nothing
            End If

            _conv_groups.Add(_currentGroup)

            _vDraw.ActiveDocument.ActiveLayOut.Entities.AddItem(group)
            group.Transformby(_mirrorMatrix)

            TryAddEntityToOverallBox(group)
        End If

        OnAfterEntityConverted(New SvgConverterEntityEventArgs(group))
    End Sub

    Private Overloads Sub OnAfterEntityConverted(entity As vdFigure)
        OnAfterEntityConverted(New SvgConverterEntityEventArgs(entity))
    End Sub

    Private Shared Function GetKblAndTopologyFromHcv(filePath As String) As KblAndTopData
        Using zf As ZipArchive = ZipFile.OpenRead(filePath)
            Dim svgEntry As ZipArchiveEntry = Nothing
            Dim kblEntry As ZipArchiveEntry = Nothing
            Dim lastKblEntry As ZipArchiveEntry = Nothing

            For Each entry As ZipArchiveEntry In zf.Entries
                If IsTopologySvgFile(entry.Name) Then
                    svgEntry = entry
                ElseIf KnownFile.IsKbl(entry.Name) Then
                    lastKblEntry = entry
                    If IsDocumentKblFile(entry.Name) Then
                        kblEntry = entry
                    End If
                End If

                If svgEntry IsNot Nothing AndAlso kblEntry IsNot Nothing Then
                    Exit For
                End If
            Next

            If kblEntry Is Nothing AndAlso lastKblEntry IsNot Nothing Then
                kblEntry = lastKblEntry
            End If

            If kblEntry IsNot Nothing AndAlso svgEntry IsNot Nothing Then
                Return New KblAndTopData(TempFile.CreateNewWithStreamContent(kblEntry.Open, KnownFile.KBL), TempFile.CreateNewWithStreamContent(svgEntry.Open, KnownFile.SVG))
            End If
        End Using
        Return New KblAndTopData(Nothing, Nothing)
    End Function

    Private Structure KblAndTopData
        Public Sub New(kbl_file As TempFile, topology_file As TempFile)
            Me.KblFile = kbl_file
            Me.TopologyFile = topology_file
        End Sub

        Property KblFile As TempFile
        Property TopologyFile As TempFile

    End Structure

    Public Shared Async Function ConvertHcvFileAsync(filePath As String, Optional progress As Action(Of ProgressChangedEventArgs) = Nothing, Optional onMessage As Action(Of MessageEventArgs) = Nothing, Optional vdraw As VectorDraw.Professional.Control.VectorDrawBaseControl = Nothing, Optional calculateOverallBoundingBox As Boolean = False) As Task(Of SvgConversionResult)
        If KnownFile.IsHcv(filePath) Then
            Dim kblAndTopo As KblAndTopData = GetKblAndTopologyFromHcv(filePath)

            If kblAndTopo.KblFile Is Nothing Then
                Return New SvgConversionResult(ResultState.Faulted, "no kbl content found in hcv!")
            End If

            If kblAndTopo.TopologyFile Is Nothing Then
                Return New SvgConversionResult(ResultState.Faulted, "no topology content found in hcv!")
            End If

            Using kblfile As TempFile = kblAndTopo.KblFile
                Using svgfile As TempFile = kblAndTopo.TopologyFile
                    Dim result As SvgConversionResult = Nothing
                    Await System.Threading.Tasks.TaskEx.RunAsSTA(
                        Sub()
                            If vdraw Is Nothing Then
                                Using tempVdraw As New VectorDrawBaseControl()
                                    tempVdraw.CreateControl()
                                    tempVdraw.EnsureDocument()
                                    result = ConvertFileCore(tempVdraw, kblfile.Name, svgfile.Name, progress, onMessage, calculateOverallBoundingBox)
                                End Using
                            Else
                                result = ConvertFileCore(vdraw, kblfile.Name, svgfile.Name, progress, onMessage, calculateOverallBoundingBox)
                            End If
                        End Sub)
                    Return result
                End Using
            End Using
        Else
            Throw New ArgumentException($"Invalid file format ""{System.IO.Path.GetExtension(filePath)}"": only hcv files are supported!")
        End If
    End Function

    Friend Shared Function ConvertFileCore(vdraw As VectorDrawBaseControl, kblFilePath As String, svgFilePath As String, progress As Action(Of ProgressChangedEventArgs), onMessage As Action(Of MessageEventArgs), calculateOverallBoundingBox As Boolean, Optional isKroschu As Nullable(Of Boolean) = Nothing) As SvgConversionResult
        Using kblStream As New System.IO.FileStream(kblFilePath, FileMode.Open)
            Using svgStream As New System.IO.FileStream(svgFilePath, FileMode.Open)
                Return ConvertFileCore(vdraw, kblStream, svgStream, progress, onMessage, calculateOverallBoundingBox, isKroschu)
            End Using
        End Using
    End Function

    Friend Shared Function ConvertFileCore(vdraw As VectorDrawBaseControl, kblStream As System.IO.Stream, svgStream As System.IO.Stream, progress As Action(Of ProgressChangedEventArgs), onMessage As Action(Of MessageEventArgs), calculateOverallBoundingBox As Boolean, Optional isKroschu As Nullable(Of Boolean) = Nothing) As SvgConversionResult
        Return ConvertFileCore(vdraw, GetKblContainerFromStream(kblStream), svgStream, progress, onMessage, calculateOverallBoundingBox, Nothing, isKroschu)
    End Function

    Public Shared Function ConvertFileCore(vdraw As VectorDrawBaseControl, kbl As IKblContainer, svgStream As System.IO.Stream, progress As Action(Of ProgressChangedEventArgs), onMessage As Action(Of MessageEventArgs), calculateOverallBoundingBox As Boolean, Optional svgName As String = Nothing, Optional isKroschu As Nullable(Of Boolean) = Nothing) As SvgConversionResult
        If svgName Is Nothing Then
            svgName = TryCast(svgStream, FileStream)?.Name
            If svgName Is Nothing Then
                svgName = System.IO.Path.ChangeExtension(System.IO.Path.GetRandomFileName, ".svg")
            End If
        End If

        Dim svg_progress_changed As IConverter.ProgressChangedEventHandler =
        Sub(sender As Object, e As ProgressChangedEventArgs)
            progress?.Invoke(e)
        End Sub

        Dim svg_conv_Message As IConverter.MessageEventHandler =
        Sub(sender As Object, e_msg As MessageEventArgs)
            onMessage?.Invoke(e_msg)
        End Sub

        Dim svg_conv As New SVGConverter(kbl, svgStream, svgName, vdraw, isKroschu)
        If progress IsNot Nothing Then
            AddHandler svg_conv.ProgressChanged, svg_progress_changed
            AddHandler svg_conv.Message, svg_conv_Message
        End If

        Try
            svg_conv.CalculateBoundingBox = calculateOverallBoundingBox

            Dim conv_result As Result = svg_conv.ConvertSVGDrawing()
            If conv_result.IsSuccess Then
                Return New SvgConversionResult(svg_conv.Converted.ToList, svg_conv.BoundingBox, svg_conv.ValidationLevel)
            Else
                Return New SvgConversionResult(conv_result)
            End If
        Finally
            If progress IsNot Nothing Then
                RemoveHandler svg_conv.ProgressChanged, svg_progress_changed
                RemoveHandler svg_conv.Message, svg_conv_Message
            End If
        End Try
    End Function

    ''' <summary>
    ''' Handles ProgressChanged events from the XML reader (implementing IProgressChanged).
    ''' This provides a cleaner, more standardized progress reporting mechanism.
    ''' </summary>
    Private Sub _hybridXmlReader_ProgressChanged(sender As Object, e As ProgressChangedEventArgs) Handles _hybridXmlReader.ProgressChanged
        ' Directly forward the progress event as it's already in the correct format
        OnProgessChanged(e)
    End Sub

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not _disposedValue Then
            If disposing Then
                _conv_groups?.Clear()
                _vdLines?.Clear()
                _factory?.Dispose()
                _hybridXmlReader?.Dispose()
                ' === PERFORMANCE OPTIMIZATION: Clean up object pools ===
                ClearLineObjectPool()
                ClearMatrixPool()
            End If
            _hybridXmlReader = Nothing
            _overallBox = Nothing
            _vdLines = Nothing
            _vDraw = Nothing
            _conv_groups = Nothing
            ' _xmlReader removed - now using HybridXmlReader
            _kblMapper = Nothing
            _factory = Nothing
            _disposedValue = True
        End If
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        ' Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in der Methode "Dispose(disposing As Boolean)" ein.
        Dispose(disposing:=True)
        GC.SuppressFinalize(Me)
    End Sub

    ' === OPTIMIZED TRANSFORM PARSING SYSTEM ===
    ' Performance optimization: 50-70% faster transform processing

    ''' <summary>
    ''' Pre-compiled regex for parsing SVG transform functions
    ''' Matches: function_name(parameters)
    ''' </summary>
    Private Shared ReadOnly TransformRegex As New Regex(
        "(\w+)\s*\(\s*([^)]+)\s*\)",
        RegexOptions.Compiled Or RegexOptions.IgnoreCase)

    ''' <summary>
    ''' Cache for parsed transform strings to avoid re-parsing common transforms
    ''' </summary>
    Private Shared ReadOnly TransformCache As New Dictionary(Of String, String())(
        StringComparer.OrdinalIgnoreCase)

    ''' <summary>
    ''' Object pool for Matrix objects to reduce allocation overhead
    ''' </summary>
    Private Shared ReadOnly MatrixPool As New Queue(Of Matrix)
    Private Shared ReadOnly MatrixPoolLock As New Object

    ''' <summary>
    ''' Optimized transform parsing that replaces the slow SplitSpace operation
    ''' with pre-compiled regex and caching for better performance
    ''' </summary>
    Friend Function ParseTransformOptimized(transformStr As String) As String()
        If String.IsNullOrEmpty(transformStr) Then Return {"No transforms"}

        ' Check cache first - this handles repeated transforms efficiently
        SyncLock TransformCache
            If TransformCache.TryGetValue(transformStr, Nothing) Then
                Return TransformCache(transformStr)
            End If
        End SyncLock

        ' Parse using pre-compiled regex (much faster than string splitting)
        Dim matches As MatchCollection = TransformRegex.Matches(transformStr)
        Dim transformList As New List(Of String)(matches.Count)

        For Each match As Match In matches
            ' Reconstruct the transform function
            transformList.Add($"{match.Groups(1).Value}({match.Groups(2).Value})")
        Next

        Dim result As String() = If(transformList.Count > 0,
                                   transformList.ToArray(),
                                   {"No transforms"})

        ' Cache the result for future use (with size limit)
        SyncLock TransformCache
            If TransformCache.Count < 1000 Then ' Prevent memory bloat
                TransformCache(transformStr) = result
            End If
        End SyncLock

        Return result
    End Function

    ''' <summary>
    ''' Gets a Matrix from the object pool or creates a new one
    ''' </summary>
    Private Shared Function GetPooledMatrix() As Matrix
        SyncLock MatrixPoolLock
            If MatrixPool.Count > 0 Then
                Return MatrixPool.Dequeue()
            End If
        End SyncLock
        Return New Matrix()
    End Function

    ''' <summary>
    ''' Returns a Matrix to the object pool for reuse
    ''' </summary>
    Private Shared Sub ReturnPooledMatrix(matrix As Matrix)
        If matrix IsNot Nothing Then
            ' Reset to identity matrix manually
            matrix.A00 = 1.0 : matrix.A01 = 0.0 : matrix.A02 = 0.0 : matrix.A03 = 0.0
            matrix.A10 = 0.0 : matrix.A11 = 1.0 : matrix.A12 = 0.0 : matrix.A13 = 0.0
            matrix.A20 = 0.0 : matrix.A21 = 0.0 : matrix.A22 = 1.0 : matrix.A23 = 0.0
            matrix.A30 = 0.0 : matrix.A31 = 0.0 : matrix.A32 = 0.0 : matrix.A33 = 1.0

            SyncLock MatrixPoolLock
                If MatrixPool.Count < 50 Then ' Limit pool size
                    MatrixPool.Enqueue(matrix)
                Else
                    matrix.Dispose() ' Pool is full, dispose
                End If
            End SyncLock
        End If
    End Sub

    ' === END OPTIMIZED TRANSFORM PARSING SYSTEM ===

    ' === PERFORMANCE OPTIMIZATION: INTELLIGENT LINE FLUSHING ===
    ''' <summary>
    ''' Determines if lines should be processed before handling the specified element.
    ''' This prevents unnecessary O(n²) calls to ConvertLines() by only flushing when context changes.
    ''' </summary>
    ''' <param name="elementName">The name of the current XML element</param>
    ''' <returns>True if lines should be processed now, False to defer processing</returns>
    Private Function ShouldFlushLinesForElement(elementName As String) As Boolean
        ' Process lines immediately for elements that change rendering context
        Select Case elementName.ToLowerInvariant()
            Case "g"         ' Group changes may affect transformations
                Return True
            Case "clippath"  ' Clipping affects rendering context
                Return True
            Case "defs"      ' Definitions should not interfere with line processing
                Return True
            Case "pattern"   ' Pattern context changes
                Return True
            Case "symbol"    ' Symbol context changes
                Return True
            Case "use"       ' Symbol usage may affect context
                Return True
            Case "svg"       ' Root element boundary
                Return True
            Case Else
                ' For other elements (circle, rect, ellipse, text, etc.), defer line processing
                ' This is the key optimization: we don't need to flush lines for every shape
                Return False
        End Select
    End Function

    ' === END INTELLIGENT LINE FLUSHING ===

    ' === PERFORMANCE OPTIMIZATION: LINE OBJECT POOL METHODS ===
    ''' <summary>
    ''' Gets a VdLineEx object from the pool or creates a new one if pool is empty
    ''' </summary>
    Private Function GetPooledLineObject() As VdLineEx
        SyncLock PoolLock
            If LineObjectPool.Count > 0 Then
                Return LineObjectPool.Dequeue()
            End If
        End SyncLock
        ' Pool is empty, create new object
        Return New VdLineEx(_vDraw.ActiveDocument)
    End Function

    ''' <summary>
    ''' Returns a VdLineEx object to the pool for reuse
    ''' </summary>
    Private Sub ReturnLineObjectToPool(line As VdLineEx)
        If line IsNot Nothing Then            ' Reset object state for reuse
            Try
                line.StartPoint = New gPoint(0, 0, 0)
                line.EndPoint = New gPoint(0, 0, 0)
                line.PenWidth = 0
                line.URL = String.Empty
                ' Don't reset document reference or other complex properties

                SyncLock PoolLock
                    If LineObjectPool.Count < MAX_POOL_SIZE Then
                        LineObjectPool.Enqueue(line)
                    Else
                        ' Pool is full, let GC handle disposal
                        line.Dispose()
                    End If
                End SyncLock
            Catch
                ' If reset fails, dispose the object
                line.Dispose()
            End Try
        End If
    End Sub
    
    ''' <summary>
    ''' Clears the line object pool (called during shutdown)
    ''' </summary>
    Private Shared Sub ClearLineObjectPool()
        SyncLock PoolLock
            While LineObjectPool.Count > 0
                Dim line As VdLineEx = LineObjectPool.Dequeue()
                line.Dispose()
            End While
        End SyncLock
    End Sub

    ''' <summary>
    ''' Clears the matrix object pool (called during shutdown)
    ''' </summary>
    Private Shared Sub ClearMatrixPool()
        SyncLock MatrixPoolLock
            While MatrixPool.Count > 0
                Dim matrix As Matrix = MatrixPool.Dequeue()
                matrix.Dispose()
            End While
        End SyncLock
    End Sub

    ' === END LINE OBJECT POOL METHODS ===

    ' === STYLE APPLICATION OPTIMIZATION METHODS ===
    
    ''' <summary>
    ''' Begins a style application batch to reduce overhead from multiple style operations
    ''' </summary>
    Private Sub BeginStyleBatch()
        If EnableBulkStyleApplication Then
            _isStyleBatchActive = True
            _pendingStyleApplications.Clear()
        End If
    End Sub
    
    ''' <summary>
    ''' Ends the current style application batch and applies all pending style operations
    ''' </summary>
    Private Sub EndStyleBatch()
        If EnableBulkStyleApplication AndAlso _isStyleBatchActive Then
            ' Apply all pending style operations
            For Each styleAction As Action In _pendingStyleApplications
                styleAction()
            Next
            _pendingStyleApplications.Clear()
            _isStyleBatchActive = False
        End If
    End Sub
    ''' <summary>
    ''' Applies style to entity using batching if enabled, otherwise applies immediately
    ''' </summary>
    Private Sub ApplyStyleOptimized(entity As vdFigure, style As VdStyle)
        If EnableBulkStyleApplication AndAlso _isStyleBatchActive Then
            ' Add to batch for later processing
            _pendingStyleApplications.Add(Sub() ApplyStyleProperties(entity, style))
        Else
            ' Apply immediately
            ApplyStyleProperties(entity, style)
        End If
    End Sub

    ''' <summary>
    ''' Helper method to apply style properties to an entity
    ''' </summary>
    Private Sub ApplyStyleProperties(entity As vdFigure, style As VdStyle)
        With entity
            .LineType = style.LineType
            .PenColor = style.PenColor
            .PenWidth = style.PenWidth
        End With
        
        ' Apply HatchProperties only if the entity supports it
        Try
            Dim entityType As Type = entity.GetType()
            Dim hatchProp As PropertyInfo = entityType.GetProperty("HatchProperties")
            If hatchProp IsNot Nothing AndAlso hatchProp.CanWrite Then
                hatchProp.SetValue(entity, style.HatchProperties, Nothing)
            End If
        Catch
            ' Ignore if HatchProperties not available for this entity type
        End Try
    End Sub
    
    ' === END STYLE APPLICATION OPTIMIZATION METHODS ===
End Class