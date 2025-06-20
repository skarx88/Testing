Option Strict On
Option Explicit On
Option Infer Off

Imports System
Imports System.Collections.Generic
Imports System.Diagnostics
Imports System.IO
Imports VectorDraw.Professional.vdObjects
Imports VectorDraw.Professional.vdPrimaries
Imports VectorDraw.Professional.vdFigures
Imports VectorDraw.Professional.Control
Imports VectorDraw.Geometry
Imports Zuken.E3.Lib.Converter.Svg
Imports System.Reflection

Namespace E3.Lib.Converter.Svg.ComparisonTests.UltraModern

    ''' <summary>
    ''' Wrapper for the original SVGConverter implementation
    ''' </summary>
    Public Class OriginalSVGConverterWrapper
        Implements ComparisonTests.ISVGConverterWrapper

        Private ReadOnly _vdraw As VectorDrawBaseControl
        Private _enableOptimizations As Boolean = False

        Public Sub New(vdraw As VectorDrawBaseControl)
            _vdraw = vdraw
        End Sub

        Public ReadOnly Property Name As String Implements ComparisonTests.ISVGConverterWrapper.Name
            Get
                Return "Original SVGConverter"
            End Get
        End Property

        Public ReadOnly Property Description As String Implements ComparisonTests.ISVGConverterWrapper.Description
            Get
                Return If(_enableOptimizations, "Original SVG Converter with optimizations enabled", "Original SVG Converter (baseline)")
            End Get
        End Property

        Public Sub ConfigureOptimizations(enableOptimizations As Boolean) Implements ComparisonTests.ISVGConverterWrapper.ConfigureOptimizations
            _enableOptimizations = enableOptimizations
        End Sub

        Public Function ConvertSVG(svgFilePath As String) As ComparisonTests.SVGConversionResult Implements ComparisonTests.ISVGConverterWrapper.ConvertSVG
            Dim result As New ComparisonTests.SVGConversionResult()
            Dim totalStopwatch As New Stopwatch()
            Dim initStopwatch As New Stopwatch()
            Dim processStopwatch As New Stopwatch()
            Dim initialMemory As Long = GC.GetTotalMemory(True)

            totalStopwatch.Start()

            Try
                initStopwatch.Start()

                Console.WriteLine("  Converting with optimizations " & If(_enableOptimizations, "enabled", "disabled") & "...")
                Console.WriteLine("    Initializing VectorDraw control...")

                ' Initialize VectorDraw control
                Console.WriteLine("    Calling CreateControl() on VectorDraw...")
                _vdraw.CreateControl()

                Console.WriteLine("    Creating new document...")
                _vdraw.ActiveDocument.New()
                Console.WriteLine("    Document created successfully")

                Console.WriteLine("    Creating empty KBL container...")
                Dim emptyKblContainer As IKblContainer = Zuken.E3.Lib.Schema.Kbl.KblMapper.CreateEmpty(Zuken.E3.Lib.Schema.Kbl.KblSchemaVersion.V24)
                Console.WriteLine("    KBL container created successfully")

                initStopwatch.Stop()
                processStopwatch.Start()

                Console.WriteLine($"    Opening SVG file: {svgFilePath}")
                Using fileStream As New FileStream(svgFilePath, FileMode.Open, FileAccess.Read)
                    Console.WriteLine("    File stream opened, creating SVG converter...")
                    Console.WriteLine($"    Parameters: Container={If(emptyKblContainer IsNot Nothing, "OK", "NULL")}, Stream={If(fileStream IsNot Nothing, "OK", "NULL")}, Path={svgFilePath}, VDraw={If(_vdraw IsNot Nothing, "OK", "NULL")}")

                    Using converter As New SVGConverter(emptyKblContainer, fileStream, svgFilePath, _vdraw)
                        Console.WriteLine("    SVG converter created successfully")
                        ' Configure optimizations
                        Console.WriteLine($"    Setting optimization flags to: {_enableOptimizations}")
                        SetOptimizationFlags(converter, _enableOptimizations)

                        Console.WriteLine("    Converting SVG...")
                        Dim conversionResult As ComponentModel.Result = converter.ConvertSVGDrawing()
                        Console.WriteLine("    SVG converted successfully")

                        processStopwatch.Stop()
                        totalStopwatch.Stop()

                        ' Check success
                        result.Success = conversionResult.IsSuccess
                        If Not result.Success Then
                            result.ErrorMessage = "Conversion failed"
                        End If

                        ' Collect performance metrics
                        result.ConversionTimeMs = totalStopwatch.Elapsed.TotalMilliseconds
                        result.InitializationTimeMs = initStopwatch.Elapsed.TotalMilliseconds
                        result.ProcessingTimeMs = processStopwatch.Elapsed.TotalMilliseconds
                        result.MemoryUsageMB = (GC.GetTotalMemory(True) - initialMemory) / 1024.0 / 1024.0
                        ' Collect geometry data
                        For Each svgGroup As VdSVGGroup In converter.Converted
                            result.Groups.Add(svgGroup)
                            result.EntityCount += svgGroup.Figures.Count

                            For Each figure As vdFigure In svgGroup.Figures
                                result.Entities.Add(CreateDetailedEntityInfo(figure))
                            Next
                        Next

                        result.GroupCount = converter.Converted.Count
                        If converter.CalculateBoundingBox Then
                            result.BoundingBox = converter.BoundingBox
                        End If

                    End Using
                End Using

            Catch ex As Exception
                result.Success = False
                result.ErrorMessage = ex.Message
                Console.WriteLine($"    X Error: {ex.Message}")
            End Try

            Return result
        End Function

        Private Sub SetOptimizationFlags(converter As SVGConverter, enableOptimizations As Boolean)
            Console.WriteLine($"    Converter type: {converter.GetType().FullName}")

            ' Try to set static optimization properties using reflection
            Dim converterType As Type = converter.GetType()

            ' Common optimization flags to try
            Dim staticOptimizationFlags() As String = {
                "EnableOptimizedParsing",
                "EnableLazyLineProcessing",
                "EnableLineObjectPooling",
                "EnableSpatialSorting"
            }

            For Each flagName As String In staticOptimizationFlags
                Try
                    Dim prop As Reflection.PropertyInfo = converterType.GetProperty(flagName, Reflection.BindingFlags.Public Or Reflection.BindingFlags.Static)
                    If prop IsNot Nothing AndAlso prop.CanWrite Then
                        prop.SetValue(Nothing, enableOptimizations)
                        Console.WriteLine($"    OK Set static {flagName} = {enableOptimizations}")
                    Else
                        Console.WriteLine($"    X Static property {flagName} not found or not writable")
                    End If
                Catch ex As Exception
                    Console.WriteLine($"    X Failed to set {flagName}: {ex.Message}")
                End Try
            Next

            ' Try instance properties
            Dim instanceOptimizationFlags() As String = {
                "EnableBulkStyleApplication",
                "EnableOptimizedGeometry",
                "EnableCaching"
            }

            For Each flagName As String In instanceOptimizationFlags
                Try
                    Dim prop As Reflection.PropertyInfo = converterType.GetProperty(flagName, Reflection.BindingFlags.Public Or Reflection.BindingFlags.Instance)
                    If prop IsNot Nothing AndAlso prop.CanWrite Then
                        prop.SetValue(converter, enableOptimizations)
                        Console.WriteLine($"    OK Set instance {flagName} = {enableOptimizations}")
                    Else
                        Console.WriteLine($"    X Instance property {flagName} not found or not writable")
                    End If
                Catch ex As Exception
                    Console.WriteLine($"    X Failed to set {flagName}: {ex.Message}")
                End Try
            Next
        End Sub
    End Class

    ''' <summary>
    ''' Wrapper for the ultra-modern high-performance SVGConverter implementation
    ''' </summary>
    Public Class UltraModernSVGConverterWrapper
        Implements ComparisonTests.ISVGConverterWrapper

        Private ReadOnly _vdraw As VectorDrawBaseControl
        Private _ultraConverter As UltraModernSVGConverter

        Public Sub New(vdraw As VectorDrawBaseControl)
            _vdraw = vdraw
        End Sub

        Public ReadOnly Property Name As String Implements ComparisonTests.ISVGConverterWrapper.Name
            Get
                Return "Ultra-Modern SVGConverter"
            End Get
        End Property

        Public ReadOnly Property Description As String Implements ComparisonTests.ISVGConverterWrapper.Description
            Get
                Return "Ultra-modern high-performance converter with .NET 8 optimizations, SIMD, parallel processing, and advanced memory management"
            End Get
        End Property

        Public Sub ConfigureOptimizations(enableOptimizations As Boolean) Implements ComparisonTests.ISVGConverterWrapper.ConfigureOptimizations
            ' Ultra-modern converter always uses optimizations, but we can configure specific features
        End Sub
        
        Public Function ConvertSVG(svgFilePath As String) As ComparisonTests.SVGConversionResult Implements ComparisonTests.ISVGConverterWrapper.ConvertSVG
            Dim result As New ComparisonTests.SVGConversionResult()
            Dim totalStopwatch As New Stopwatch()
            Dim initStopwatch As New Stopwatch()
            Dim processStopwatch As New Stopwatch()
            Dim initialMemory As Long = GC.GetTotalMemory(True)

            totalStopwatch.Start()

            Try
                initStopwatch.Start()

                Console.WriteLine("  Testing Ultra-Modern converter...")

                ' Initialize ultra-modern converter with baseline compatibility mode
                _ultraConverter = New UltraModernSVGConverter(_vdraw)
                
                ' === ENABLE BASELINE COMPATIBILITY MODE ===
                _ultraConverter.BaselineCompatibilityMode = True
                _ultraConverter.EnableEntityConsolidation = True
                _ultraConverter.EnableGroupAggregation = True
                _ultraConverter.SuppressGeometricPrimitives = True

                initStopwatch.Stop()
                processStopwatch.Start()

                ' Convert using ultra-modern implementation
                Dim groups As List(Of VdSVGGroup) = CType(_ultraConverter.ConvertFromFile(svgFilePath), List(Of VdSVGGroup))

                processStopwatch.Stop()
                totalStopwatch.Stop()

                ' Check success
                result.Success = groups IsNot Nothing AndAlso groups.Count > 0

                ' Collect performance metrics
                result.ConversionTimeMs = totalStopwatch.Elapsed.TotalMilliseconds
                result.InitializationTimeMs = initStopwatch.Elapsed.TotalMilliseconds
                result.ProcessingTimeMs = processStopwatch.Elapsed.TotalMilliseconds
                result.MemoryUsageMB = (GC.GetTotalMemory(True) - initialMemory) / 1024.0 / 1024.0

                ' Collect geometry data if successful
                If result.Success Then
                    result.Groups.AddRange(groups)
                    For Each svgGroup As VdSVGGroup In groups
                        result.EntityCount += svgGroup.Figures.Count

                        For Each figure As vdFigure In svgGroup.Figures
                            result.Entities.Add(CreateDetailedEntityInfo(figure))
                        Next
                    Next

                    result.GroupCount = groups.Count
                    result.PerformanceDetails = "Ultra-modern converter with baseline compatibility mode enabled"
                End If

            Catch ex As Exception
                result.Success = False
                result.ErrorMessage = ex.Message
                Console.WriteLine($"    X Ultra-modern converter error: {ex.Message}")
            End Try
            Return result
        End Function

    End Class

    ''' <summary>
    ''' Helper module for creating detailed entity information with comprehensive property extraction
    ''' </summary>
    Public Module EntityInfoHelper

        ''' <summary>
        ''' Creates a detailed EntityInfo object with comprehensive property extraction from a VectorDraw figure
        ''' </summary>
        ''' <param name="figure">The VectorDraw figure to extract information from</param>
        ''' <returns>EntityInfo with detailed property information</returns>
        Public Function CreateDetailedEntityInfo(figure As vdFigure) As ComparisonTests.EntityInfo
            Dim entityInfo As New ComparisonTests.EntityInfo() With {
                .Type = figure.GetType().Name,
                .BoundingBox = figure.BoundingBox
            }

            ' Extract common properties for all entity types
            ExtractCommonProperties(figure, entityInfo)

            ' Extract type-specific properties
            Select Case True
                Case TypeOf figure Is VdLineEx
                    ExtractLineProperties(DirectCast(figure, VdLineEx), entityInfo)
                Case TypeOf figure Is VdTextEx
                    ExtractTextProperties(DirectCast(figure, VdTextEx), entityInfo)
                Case TypeOf figure Is VdPolylineEx
                    ExtractPolylineProperties(DirectCast(figure, VdPolylineEx), entityInfo)
                Case TypeOf figure Is VdCircleEx
                    ExtractCircleProperties(DirectCast(figure, VdCircleEx), entityInfo)
                Case TypeOf figure Is VdEllipseEx
                    ExtractEllipseProperties(DirectCast(figure, VdEllipseEx), entityInfo)
                Case TypeOf figure Is VdRectEx
                    ExtractRectProperties(DirectCast(figure, VdRectEx), entityInfo)
                Case TypeOf figure Is vdLine
                    ExtractLineProperties(DirectCast(figure, vdLine), entityInfo)
                Case TypeOf figure Is vdText
                    ExtractTextProperties(DirectCast(figure, vdText), entityInfo)
                Case TypeOf figure Is vdPolyline
                    ExtractPolylineProperties(DirectCast(figure, vdPolyline), entityInfo)
                Case TypeOf figure Is vdCircle
                    ExtractCircleProperties(DirectCast(figure, vdCircle), entityInfo)
                Case TypeOf figure Is vdEllipse
                    ExtractEllipseProperties(DirectCast(figure, vdEllipse), entityInfo)
                Case Else
                    ' For unknown entity types, try to extract whatever we can using reflection
                    ExtractGenericProperties(figure, entityInfo)
            End Select

            Return entityInfo
        End Function

        ''' <summary>
        ''' Extracts common properties available to all VectorDraw entities
        ''' </summary>
        Private Sub ExtractCommonProperties(figure As vdFigure, entityInfo As ComparisonTests.EntityInfo)
            Try
                ' Basic entity properties
                entityInfo.Layer = If(figure.Layer?.Name, "")
                entityInfo.Color = If(figure.PenColor.ColorIndex.ToString(), "")
                entityInfo.LineWeight = If(figure.LineTypeScale = Double.NaN, 0.0, figure.LineTypeScale)
                entityInfo.Style = If(figure.LineType?.Name, "")

                ' Store additional properties in the Properties dictionary
                entityInfo.Properties("Handle") = If(figure.Handle?.ToString(), "")
                entityInfo.Properties("Visible") = figure.IsVisible.ToString()
                entityInfo.Properties("Deleted") = figure.Deleted.ToString()

                ' Color properties
                If figure.PenColor IsNot Nothing Then
                    entityInfo.Properties("PenColor.Red") = figure.PenColor.Red.ToString()
                    entityInfo.Properties("PenColor.Green") = figure.PenColor.Green.ToString()
                    entityInfo.Properties("PenColor.Blue") = figure.PenColor.Blue.ToString()
                    entityInfo.Properties("PenColor.ColorIndex") = figure.PenColor.ColorIndex.ToString()
                End If

                ' Matrix/transformation properties
                If figure.ECSMatrix IsNot Nothing Then
                    entityInfo.Properties("Matrix") = figure.ECSMatrix.ToString()
                End If

            Catch ex As Exception
                ' Silently ignore property extraction errors - some properties may not be accessible
                entityInfo.Properties("CommonPropertiesError") = ex.Message
            End Try
        End Sub

        ''' <summary>
        ''' Extracts properties specific to line entities
        ''' </summary>
        Private Sub ExtractLineProperties(line As vdLine, entityInfo As ComparisonTests.EntityInfo)
            Try
                entityInfo.Properties("StartPoint.X") = line.StartPoint.x.ToString("F6")
                entityInfo.Properties("StartPoint.Y") = line.StartPoint.y.ToString("F6")
                entityInfo.Properties("StartPoint.Z") = line.StartPoint.z.ToString("F6")
                entityInfo.Properties("EndPoint.X") = line.EndPoint.x.ToString("F6")
                entityInfo.Properties("EndPoint.Y") = line.EndPoint.y.ToString("F6")
                entityInfo.Properties("EndPoint.Z") = line.EndPoint.z.ToString("F6")
                entityInfo.Properties("Length") = line.Length.ToString("F6")
            Catch ex As Exception
                entityInfo.Properties("LinePropertiesError") = ex.Message
            End Try
        End Sub

        ''' <summary>
        ''' Extracts properties specific to text entities
        ''' </summary>
        Private Sub ExtractTextProperties(text As vdText, entityInfo As ComparisonTests.EntityInfo)
            Try
                entityInfo.Properties("TextString") = If(text.TextString, "")
                entityInfo.Properties("InsertionPoint.X") = text.InsertionPoint.x.ToString("F6")
                entityInfo.Properties("InsertionPoint.Y") = text.InsertionPoint.y.ToString("F6")
                entityInfo.Properties("InsertionPoint.Z") = text.InsertionPoint.z.ToString("F6")
                entityInfo.Properties("Height") = text.Height.ToString("F6")
                entityInfo.Properties("Rotation") = text.Rotation.ToString("F6")
                entityInfo.Properties("WidthFactor") = text.WidthFactor.ToString("F6")

                If text.Style IsNot Nothing Then
                    entityInfo.Properties("TextStyle.Name") = If(text.Style.Name, "")
                    entityInfo.Properties("TextStyle.FontName") = If(text.Style.FontFile, "")
                End If
            Catch ex As Exception
                entityInfo.Properties("TextPropertiesError") = ex.Message
            End Try
        End Sub

        ''' <summary>
        ''' Extracts properties specific to polyline entities
        ''' </summary>
        Private Sub ExtractPolylineProperties(polyline As vdPolyline, entityInfo As ComparisonTests.EntityInfo)
            Try
                entityInfo.Properties("VertexList.Count") = polyline.VertexList.Count.ToString()
                entityInfo.Properties("Closed") = polyline.Flag.ToString()
                entityInfo.Properties("Length") = polyline.Length.ToString("F6")

                ' Store first few vertices for comparison
                For i As Integer = 0 To Math.Min(2, polyline.VertexList.Count - 1)
                    Dim vertex As Vertex = polyline.VertexList(i)
                    entityInfo.Properties($"Vertex{i}.X") = vertex.x.ToString("F6")
                    entityInfo.Properties($"Vertex{i}.Y") = vertex.y.ToString("F6")
                    entityInfo.Properties($"Vertex{i}.Z") = vertex.z.ToString("F6")
                Next
            Catch ex As Exception
                entityInfo.Properties("PolylinePropertiesError") = ex.Message
            End Try
        End Sub

        ''' <summary>
        ''' Extracts properties specific to circle entities
        ''' </summary>
        Private Sub ExtractCircleProperties(circle As vdCircle, entityInfo As ComparisonTests.EntityInfo)
            Try
                entityInfo.Properties("Center.X") = circle.Center.x.ToString("F6")
                entityInfo.Properties("Center.Y") = circle.Center.y.ToString("F6")
                entityInfo.Properties("Center.Z") = circle.Center.z.ToString("F6")
                entityInfo.Properties("Radius") = circle.Radius.ToString("F6")
                entityInfo.Properties("Circumference") = circle.Length.ToString("F6")
            Catch ex As Exception
                entityInfo.Properties("CirclePropertiesError") = ex.Message
            End Try
        End Sub        ''' <summary>
        ''' Extracts properties specific to ellipse entities
        ''' </summary>
        Private Sub ExtractEllipseProperties(ellipse As vdEllipse, entityInfo As ComparisonTests.EntityInfo)
            Try
                entityInfo.Properties("Center.X") = ellipse.Center.x.ToString("F6")
                entityInfo.Properties("Center.Y") = ellipse.Center.y.ToString("F6")
                entityInfo.Properties("Center.Z") = ellipse.Center.z.ToString("F6")
                entityInfo.Properties("MajorLength") = ellipse.MajorLength.ToString("F6")
                entityInfo.Properties("MinorLength") = ellipse.MinorLength.ToString("F6")
                ' Calculate ratio as MinorLength / MajorLength
                If ellipse.MajorLength <> 0 Then
                    entityInfo.Properties("Ratio") = (ellipse.MinorLength / ellipse.MajorLength).ToString("F6")
                Else
                    entityInfo.Properties("Ratio") = "0"
                End If
            Catch ex As Exception
                entityInfo.Properties("EllipsePropertiesError") = ex.Message
            End Try
        End Sub

        ''' <summary>
        ''' Extracts properties specific to rectangle entities
        ''' </summary>
        Private Sub ExtractRectProperties(rect As Object, entityInfo As ComparisonTests.EntityInfo)
            Try
                ' Use reflection to extract rectangle properties since VdRectEx may have custom properties
                Dim rectType As Type = rect.GetType()

                ' Try to get common rectangle properties
                Dim centerProp As PropertyInfo = rectType.GetProperty("Center")
                If centerProp IsNot Nothing Then
                    Dim center As Object = centerProp.GetValue(rect)
                    If center IsNot Nothing Then
                        entityInfo.Properties("Center") = center.ToString()
                    End If
                End If

                Dim widthProp As PropertyInfo = rectType.GetProperty("Width")
                If widthProp IsNot Nothing Then
                    entityInfo.Properties("Width") = widthProp.GetValue(rect)?.ToString()
                End If

                Dim heightProp As PropertyInfo = rectType.GetProperty("Height")
                If heightProp IsNot Nothing Then
                    entityInfo.Properties("Height") = heightProp.GetValue(rect)?.ToString()
                End If

            Catch ex As Exception
                entityInfo.Properties("RectPropertiesError") = ex.Message
            End Try
        End Sub

        ''' <summary>
        ''' Extracts properties from unknown entity types using reflection
        ''' </summary>
        Private Sub ExtractGenericProperties(figure As vdFigure, entityInfo As ComparisonTests.EntityInfo)
            Try
                ' Use reflection to try to extract some common geometric properties
                Dim figureType As Type = figure.GetType()

                ' Try common geometric properties
                Dim commonProps As String() = {"Length", "Area", "Center", "Radius", "Width", "Height", "StartPoint", "EndPoint"}

                For Each propName As String In commonProps
                    Try
                        Dim prop As PropertyInfo = figureType.GetProperty(propName)
                        If prop IsNot Nothing AndAlso prop.CanRead Then
                            Dim value As Object = prop.GetValue(figure)
                            If value IsNot Nothing Then
                                entityInfo.Properties(propName) = value.ToString()
                            End If
                        End If
                    Catch
                        ' Ignore individual property errors
                    End Try
                Next

            Catch ex As Exception
                entityInfo.Properties("GenericPropertiesError") = ex.Message
            End Try
        End Sub

    End Module

End Namespace
