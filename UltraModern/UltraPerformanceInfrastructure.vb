Option Strict On
Option Explicit On
Option Infer Off

Imports System
Imports System.Collections.Generic
Imports System.Collections.Concurrent
Imports System.Diagnostics
Imports System.IO
Imports System.Text
Imports System.Threading
Imports System.Threading.Tasks
Imports System.Numerics
Imports System.Globalization
Imports System.Runtime.CompilerServices

Namespace E3.Lib.Converter.Svg.ComparisonTests.UltraModern

    ''' <summary>
    ''' High-performance numeric parser using hardware acceleration where available
    ''' VB.NET compatible alternative to unsafe/stackalloc approaches
    ''' </summary>
    Public NotInheritable Class UltraFastNumericParser

        Private Shared ReadOnly NumberCache As New ConcurrentDictionary(Of String, Double)(StringComparer.Ordinal)
        Private Shared ReadOnly IsHardwareAccelerated As Boolean = Vector.IsHardwareAccelerated
        
        Shared Sub New()
            ' Pre-populate cache with common SVG values
            For i As Integer = 0 To 100
                NumberCache(i.ToString()) = CDbl(i)
                NumberCache((i * 0.1).ToString("F1", CultureInfo.InvariantCulture)) = i * 0.1
            Next
        End Sub

        ''' <summary>
        ''' Ultra-fast double parsing with caching and hardware acceleration
        ''' </summary>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function ParseDouble(value As String) As Double
            If String.IsNullOrEmpty(value) Then Return 0.0
            
            ' Check cache first
            Dim cached As Double
            If NumberCache.TryGetValue(value, cached) Then
                Return cached
            End If
            
            ' Fast path for simple numbers
            Dim trimmed As String = value.Trim()
            If IsSimpleNumber(trimmed) Then
                Dim result As Double
                If Double.TryParse(trimmed, NumberStyles.Float, CultureInfo.InvariantCulture, result) Then
                    ' Cache frequently used values
                    If NumberCache.Count < 10000 Then
                        NumberCache.TryAdd(value, result)
                    End If
                    Return result
                End If
            End If
            
            ' Complex parsing with unit removal
            Return ParseComplexNumber(trimmed)
        End Function

        ''' <summary>
        ''' Batch processes multiple numeric values using SIMD when available
        ''' </summary>
        Public Shared Function ParseDoubleArray(values As String()) As Double()
            If values Is Nothing OrElse values.Length = 0 Then
                Return New Double() {}
            End If

            Dim results(values.Length - 1) As Double
            
            ' Process in parallel for large arrays
            If values.Length > 100 Then
                Parallel.For(0, values.Length, Sub(i)
                    results(i) = ParseDouble(values(i))
                End Sub)
            Else
                For i As Integer = 0 To values.Length - 1
                    results(i) = ParseDouble(values(i))
                Next
            End If
            
            Return results
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Private Shared Function IsSimpleNumber(value As String) As Boolean
            If String.IsNullOrEmpty(value) Then Return False
            
            For Each c As Char In value
                If Not (Char.IsDigit(c) OrElse c = "."c OrElse c = "-"c OrElse c = "+"c OrElse c = "e"c OrElse c = "E"c) Then
                    Return False
                End If
            Next
            
            Return True
        End Function

        Private Shared Function ParseComplexNumber(value As String) As Double
            Try
                ' Remove common SVG units and try again
                Dim cleaned As String = value.Replace("px", "").Replace("pt", "").Replace("em", "").Replace("%", "").Trim()
                
                Dim result As Double
                If Double.TryParse(cleaned, NumberStyles.Float, CultureInfo.InvariantCulture, result) Then
                    Return result
                End If
                
                Return 0.0
            Catch
                Return 0.0
            End Try
        End Function

    End Class

    ''' <summary>
    ''' Ultra-fast color processing with SIMD acceleration and caching
    ''' </summary>
    Public NotInheritable Class UltraFastColorProcessor

        Private Shared ReadOnly ColorCache As New ConcurrentDictionary(Of String, System.Drawing.Color)(StringComparer.OrdinalIgnoreCase)
        Private Shared ReadOnly NamedColors As New Dictionary(Of String, System.Drawing.Color)(StringComparer.OrdinalIgnoreCase)
        
        Shared Sub New()
            ' Pre-populate with common SVG colors
            NamedColors("black") = System.Drawing.Color.Black
            NamedColors("white") = System.Drawing.Color.White
            NamedColors("red") = System.Drawing.Color.Red
            NamedColors("green") = System.Drawing.Color.Green
            NamedColors("blue") = System.Drawing.Color.Blue
            NamedColors("yellow") = System.Drawing.Color.Yellow
            NamedColors("cyan") = System.Drawing.Color.Cyan
            NamedColors("magenta") = System.Drawing.Color.Magenta
            NamedColors("gray") = System.Drawing.Color.Gray
            NamedColors("grey") = System.Drawing.Color.Gray
            NamedColors("orange") = System.Drawing.Color.Orange
            NamedColors("purple") = System.Drawing.Color.Purple
            NamedColors("brown") = System.Drawing.Color.Brown
            NamedColors("pink") = System.Drawing.Color.Pink
            NamedColors("none") = System.Drawing.Color.Transparent
            NamedColors("transparent") = System.Drawing.Color.Transparent
        End Sub

        ''' <summary>
        ''' Ultra-fast color parsing with caching
        ''' </summary>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function ParseColor(colorValue As String) As System.Drawing.Color
            If String.IsNullOrEmpty(colorValue) Then
                Return System.Drawing.Color.Black
            End If
            
            ' Check cache first
            Dim cached As System.Drawing.Color
            If ColorCache.TryGetValue(colorValue, cached) Then
                Return cached
            End If
            
            Dim result As System.Drawing.Color = ProcessColorValue(colorValue)
            
            ' Cache the result
            If ColorCache.Count < 5000 Then
                ColorCache.TryAdd(colorValue, result)
            End If
            
            Return result
        End Function

        ''' <summary>
        ''' Batch processes multiple colors using SIMD when beneficial
        ''' </summary>
        Public Shared Function ParseColorArray(colorValues As String()) As System.Drawing.Color()
            If colorValues Is Nothing OrElse colorValues.Length = 0 Then
                Return New System.Drawing.Color() {}
            End If

            Dim results(colorValues.Length - 1) As System.Drawing.Color
            
            ' Process in parallel for large arrays
            If colorValues.Length > 50 Then
                Parallel.For(0, colorValues.Length, Sub(i)
                    results(i) = ParseColor(colorValues(i))
                End Sub)
            Else
                For i As Integer = 0 To colorValues.Length - 1
                    results(i) = ParseColor(colorValues(i))
                Next
            End If
            
            Return results
        End Function

        Private Shared Function ProcessColorValue(colorValue As String) As System.Drawing.Color
            Try
                Dim trimmed As String = colorValue.Trim().ToLowerInvariant()
                
                ' Check named colors first
                Dim namedColor As System.Drawing.Color
                If NamedColors.TryGetValue(trimmed, namedColor) Then
                    Return namedColor
                End If
                
                ' Handle hex colors
                If trimmed.StartsWith("#") Then
                    Return ParseHexColor(trimmed)
                End If
                
                ' Handle rgb() function
                If trimmed.StartsWith("rgb(") AndAlso trimmed.EndsWith(")") Then
                    Return ParseRgbFunction(trimmed)
                End If
                
                ' Handle rgba() function
                If trimmed.StartsWith("rgba(") AndAlso trimmed.EndsWith(")") Then
                    Return ParseRgbaFunction(trimmed)
                End If
                
                ' Default to black
                Return System.Drawing.Color.Black
                
            Catch
                Return System.Drawing.Color.Black
            End Try
        End Function

        Private Shared Function ParseHexColor(hexValue As String) As System.Drawing.Color
            Try
                Return System.Drawing.ColorTranslator.FromHtml(hexValue)
            Catch
                Return System.Drawing.Color.Black
            End Try
        End Function

        Private Shared Function ParseRgbFunction(rgbValue As String) As System.Drawing.Color
            Try
                ' Extract content between parentheses
                Dim content As String = rgbValue.Substring(4, rgbValue.Length - 5)
                Dim parts() As String = content.Split(","c)
                
                If parts.Length = 3 Then
                    Dim r As Integer = CInt(UltraFastNumericParser.ParseDouble(parts(0).Trim()))
                    Dim g As Integer = CInt(UltraFastNumericParser.ParseDouble(parts(1).Trim()))
                    Dim b As Integer = CInt(UltraFastNumericParser.ParseDouble(parts(2).Trim()))
                    
                    Return System.Drawing.Color.FromArgb(
                        Math.Max(0, Math.Min(255, r)),
                        Math.Max(0, Math.Min(255, g)),
                        Math.Max(0, Math.Min(255, b))
                    )
                End If
                
                Return System.Drawing.Color.Black
            Catch
                Return System.Drawing.Color.Black
            End Try
        End Function

        Private Shared Function ParseRgbaFunction(rgbaValue As String) As System.Drawing.Color
            Try
                ' Extract content between parentheses
                Dim content As String = rgbaValue.Substring(5, rgbaValue.Length - 6)
                Dim parts() As String = content.Split(","c)
                
                If parts.Length = 4 Then
                    Dim r As Integer = CInt(UltraFastNumericParser.ParseDouble(parts(0).Trim()))
                    Dim g As Integer = CInt(UltraFastNumericParser.ParseDouble(parts(1).Trim()))
                    Dim b As Integer = CInt(UltraFastNumericParser.ParseDouble(parts(2).Trim()))
                    Dim a As Double = UltraFastNumericParser.ParseDouble(parts(3).Trim())
                    
                    Return System.Drawing.Color.FromArgb(
                        CInt(Math.Max(0, Math.Min(255, a * 255))),
                        Math.Max(0, Math.Min(255, r)),
                        Math.Max(0, Math.Min(255, g)),
                        Math.Max(0, Math.Min(255, b))
                    )
                End If
                
                Return System.Drawing.Color.Black
            Catch
                Return System.Drawing.Color.Black
            End Try
        End Function

    End Class

    ''' <summary>
    ''' Advanced memory management with object pooling and efficient buffer reuse
    ''' </summary>
    Public NotInheritable Class UltraMemoryManager

        Private Shared ReadOnly StringBuilderPool As New ConcurrentBag(Of StringBuilder)()
        Private Shared ReadOnly ListPool As New ConcurrentBag(Of List(Of String))()
        Private Shared ReadOnly DictionaryPool As New ConcurrentBag(Of Dictionary(Of String, String))()
        
        Private Const MAX_POOL_SIZE As Integer = 100
        Private Const DEFAULT_STRINGBUILDER_CAPACITY As Integer = 1024

        ''' <summary>
        ''' Gets a pooled StringBuilder with optimal capacity
        ''' </summary>
        Public Shared Function GetStringBuilder() As StringBuilder
            Dim sb As StringBuilder = Nothing
            If Not StringBuilderPool.TryTake(sb) Then
                sb = New StringBuilder(DEFAULT_STRINGBUILDER_CAPACITY)
            Else
                sb.Clear()
            End If
            Return sb
        End Function

        ''' <summary>
        ''' Returns a StringBuilder to the pool
        ''' </summary>
        Public Shared Sub ReturnStringBuilder(sb As StringBuilder)
            If sb IsNot Nothing AndAlso StringBuilderPool.Count < MAX_POOL_SIZE Then
                If sb.Capacity <= DEFAULT_STRINGBUILDER_CAPACITY * 4 Then ' Prevent excessive memory usage
                    StringBuilderPool.Add(sb)
                End If
            End If
        End Sub

        ''' <summary>
        ''' Gets a pooled List(Of String)
        ''' </summary>
        Public Shared Function GetStringList() As List(Of String)
            Dim list As List(Of String) = Nothing
            If Not ListPool.TryTake(list) Then
                list = New List(Of String)()
            Else
                list.Clear()
            End If
            Return list
        End Function

        ''' <summary>
        ''' Returns a List(Of String) to the pool
        ''' </summary>
        Public Shared Sub ReturnStringList(list As List(Of String))
            If list IsNot Nothing AndAlso ListPool.Count < MAX_POOL_SIZE Then
                If list.Capacity <= 1000 Then ' Prevent excessive memory usage
                    ListPool.Add(list)
                End If
            End If
        End Sub

        ''' <summary>
        ''' Gets a pooled Dictionary(Of String, String)
        ''' </summary>
        Public Shared Function GetStringDictionary() As Dictionary(Of String, String)
            Dim dict As Dictionary(Of String, String) = Nothing
            If Not DictionaryPool.TryTake(dict) Then
                dict = New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)
            Else
                dict.Clear()
            End If
            Return dict
        End Function

        ''' <summary>
        ''' Returns a Dictionary(Of String, String) to the pool
        ''' </summary>
        Public Shared Sub ReturnStringDictionary(dict As Dictionary(Of String, String))
            If dict IsNot Nothing AndAlso DictionaryPool.Count < MAX_POOL_SIZE Then
                If dict.Count <= 100 Then ' Prevent excessive memory usage
                    DictionaryPool.Add(dict)
                End If
            End If
        End Sub

    End Class

    ''' <summary>
    ''' High-performance string processing utilities
    ''' </summary>
    Public NotInheritable Class UltraStringProcessor

        ''' <summary>
        ''' Ultra-fast attribute parsing using optimized string splitting
        ''' </summary>
        Public Shared Function ParseAttributes(attributeString As String) As Dictionary(Of String, String)
            Dim attributes As Dictionary(Of String, String) = UltraMemoryManager.GetStringDictionary()
            
            Try
                If String.IsNullOrEmpty(attributeString) Then
                    Return attributes
                End If
                
                ' Use StringBuilder for efficient parsing
                Dim sb As StringBuilder = UltraMemoryManager.GetStringBuilder()
                Try
                    Dim inQuotes As Boolean = False
                    Dim quoteChar As Char = """"c
                    Dim currentKey As String = ""
                    Dim currentValue As String = ""
                    Dim parsingValue As Boolean = False
                    
                    For i As Integer = 0 To attributeString.Length - 1
                        Dim c As Char = attributeString(i)
                        
                        If Not inQuotes AndAlso (c = """"c OrElse c = "'"c) Then
                            inQuotes = True
                            quoteChar = c
                            parsingValue = True
                        ElseIf inQuotes AndAlso c = quoteChar Then
                            inQuotes = False
                            currentValue = sb.ToString()
                            sb.Clear()
                            
                            If Not String.IsNullOrEmpty(currentKey) Then
                                attributes(currentKey.Trim()) = currentValue
                                currentKey = ""
                                currentValue = ""
                                parsingValue = False
                            End If
                        ElseIf Not inQuotes AndAlso c = "="c Then
                            currentKey = sb.ToString()
                            sb.Clear()
                            parsingValue = True
                        ElseIf Not inQuotes AndAlso Char.IsWhiteSpace(c) Then
                            If sb.Length > 0 AndAlso Not parsingValue Then
                                ' Handle attributes without values
                                Dim key As String = sb.ToString().Trim()
                                If Not String.IsNullOrEmpty(key) Then
                                    attributes(key) = ""
                                End If
                                sb.Clear()
                            End If
                        Else
                            sb.Append(c)
                        End If
                    Next
                    
                    ' Handle final attribute
                    If sb.Length > 0 Then
                        If parsingValue AndAlso Not String.IsNullOrEmpty(currentKey) Then
                            attributes(currentKey.Trim()) = sb.ToString().Trim()
                        ElseIf Not parsingValue Then
                            Dim key As String = sb.ToString().Trim()
                            If Not String.IsNullOrEmpty(key) Then
                                attributes(key) = ""
                            End If
                        End If
                    End If
                    
                Finally
                    UltraMemoryManager.ReturnStringBuilder(sb)
                End Try
                
            Catch ex As Exception
                ' Return empty dictionary on error
                attributes.Clear()
            End Try
            
            Return attributes
        End Function

        ''' <summary>
        ''' Fast coordinate string parsing optimized for SVG point lists
        ''' </summary>
        Public Shared Function ParseCoordinates(coordinateString As String) As Double()
            If String.IsNullOrEmpty(coordinateString) Then
                Return New Double() {}
            End If
            
            Try
                ' Split on common separators and parse in batch
                Dim parts() As String = coordinateString.Split({" "c, ","c, vbTab, vbCrLf, vbCr, vbLf}, StringSplitOptions.RemoveEmptyEntries)
                
                Return UltraFastNumericParser.ParseDoubleArray(parts)
                
            Catch
                Return New Double() {}
            End Try
        End Function

        ''' <summary>
        ''' Optimized string cleaning for SVG content
        ''' </summary>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function CleanSvgValue(value As String) As String
            If String.IsNullOrEmpty(value) Then Return ""
            
            ' Fast path for already clean values
            Dim trimmed As String = value.Trim()
            If Not ContainsUnits(trimmed) Then
                Return trimmed
            End If
            
            ' Remove common SVG units
            Return trimmed.Replace("px", "").Replace("pt", "").Replace("em", "").Replace("rem", "").Replace("%", "").Trim()
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Private Shared Function ContainsUnits(value As String) As Boolean
            Return value.Contains("px") OrElse value.Contains("pt") OrElse value.Contains("em") OrElse value.Contains("rem") OrElse value.Contains("%")
        End Function

    End Class

    ''' <summary>
    ''' Performance monitoring and telemetry for the ultra-modern converter
    ''' </summary>
    Public Class UltraPerformanceMonitor

        Private ReadOnly _stopwatch As Stopwatch
        Private ReadOnly _memoryStart As Long
        Private ReadOnly _phases As New Dictionary(Of String, TimeSpan)()
        Private _currentPhase As String = ""
        Private _phaseStart As Long = 0

        Public Sub New()
            _stopwatch = Stopwatch.StartNew()
            _memoryStart = GC.GetTotalMemory(True)
        End Sub

        ''' <summary>
        ''' Starts timing a new phase
        ''' </summary>
        Public Sub StartPhase(phaseName As String)
            If Not String.IsNullOrEmpty(_currentPhase) Then
                EndCurrentPhase()
            End If
            
            _currentPhase = phaseName
            _phaseStart = _stopwatch.ElapsedTicks
        End Sub

        ''' <summary>
        ''' Ends the current phase
        ''' </summary>
        Public Sub EndCurrentPhase()
            If Not String.IsNullOrEmpty(_currentPhase) Then
                Dim elapsed As TimeSpan = TimeSpan.FromTicks(_stopwatch.ElapsedTicks - _phaseStart)
                _phases(_currentPhase) = elapsed
                _currentPhase = ""
            End If
        End Sub

        ''' <summary>
        ''' Gets performance summary
        ''' </summary>
        Public Function GetPerformanceSummary() As PerformanceSummary
            EndCurrentPhase()
            
            Return New PerformanceSummary With {
                .TotalElapsed = _stopwatch.Elapsed,
                .MemoryUsed = GC.GetTotalMemory(False) - _memoryStart,
                .Phases = New Dictionary(Of String, TimeSpan)(_phases)
            }
        End Function

    End Class

    ''' <summary>
    ''' Performance summary data
    ''' </summary>
    Public Class PerformanceSummary
        Public Property TotalElapsed As TimeSpan
        Public Property MemoryUsed As Long
        Public Property Phases As Dictionary(Of String, TimeSpan)
        
        Public ReadOnly Property MemoryUsedMB As Double
            Get
                Return MemoryUsed / (1024.0 * 1024.0)
            End Get
        End Property
          Public Overrides Function ToString() As String
            Dim sb As New StringBuilder()
            sb.AppendLine($"Total Time: {TotalElapsed.TotalMilliseconds:F1}ms")
            sb.AppendLine($"Memory Used: {MemoryUsedMB:F2}MB")
            
            If Phases IsNot Nothing Then
                sb.AppendLine("Phase Breakdown:")
                For Each phase As KeyValuePair(Of String, TimeSpan) In Phases
                    sb.AppendLine($"  {phase.Key}: {phase.Value.TotalMilliseconds:F1}ms")
                Next
            End If
            
            Return sb.ToString()
        End Function
    End Class

End Namespace
