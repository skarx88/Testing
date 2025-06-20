Option Strict On
Option Explicit On
Option Infer Off

Imports System
Imports System.Collections.Generic
Imports System.Diagnostics
Imports System.IO
Imports VectorDraw.Professional.vdObjects
Imports VectorDraw.Professional.vdPrimaries
Imports VectorDraw.Professional.Control
Imports VectorDraw.Geometry
Imports Zuken.E3.Lib.Converter.Svg

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
                        Dim conversionResult = converter.ConvertSVGDrawing()
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
                                result.Entities.Add(New ComparisonTests.EntityInfo() With {
                                    .Type = figure.GetType().Name,
                                    .BoundingBox = figure.BoundingBox
                                })
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
                Console.WriteLine($"    ✗ Error: {ex.Message}")
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
                        Console.WriteLine($"    ✓ Set static {flagName} = {enableOptimizations}")
                    Else
                        Console.WriteLine($"    ✗ Static property {flagName} not found or not writable")
                    End If
                Catch ex As Exception
                    Console.WriteLine($"    ✗ Failed to set {flagName}: {ex.Message}")
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
                        Console.WriteLine($"    ✓ Set instance {flagName} = {enableOptimizations}")
                    Else
                        Console.WriteLine($"    ✗ Instance property {flagName} not found or not writable")
                    End If
                Catch ex As Exception
                    Console.WriteLine($"    ✗ Failed to set {flagName}: {ex.Message}")
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
                
                ' Initialize ultra-modern converter
                _ultraConverter = New UltraModernSVGConverter(_vdraw)
                
                initStopwatch.Stop()
                processStopwatch.Start()
                
                ' Convert using ultra-modern implementation
                Dim groups As List(Of VdSVGGroup) = _ultraConverter.ConvertFromFile(svgFilePath)
                
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
                            result.Entities.Add(New ComparisonTests.EntityInfo() With {
                                .Type = figure.GetType().Name,
                                .BoundingBox = figure.BoundingBox
                            })
                        Next
                    Next
                    
                    result.GroupCount = groups.Count
                    result.PerformanceDetails = "Ultra-modern converter with SIMD, parallel processing, and advanced memory pooling"
                End If
                
            Catch ex As Exception
                result.Success = False
                result.ErrorMessage = ex.Message
                Console.WriteLine($"    ✗ Ultra-modern converter error: {ex.Message}")
            End Try
            
            Return result
        End Function
    End Class

End Namespace
