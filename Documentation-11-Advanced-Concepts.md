# Advanced Concepts

This section explores advanced patterns, techniques, and architectural concepts in the E3.Lib.Licensing library. These concepts are essential for developers who need to extend the library, implement custom licensing solutions, or optimize performance in complex scenarios.

### Overview

Advanced concepts in E3.Lib.Licensing cover:
- **Generic Type-Safe Licensing**: Implementing strongly-typed feature management
- **Abstract Base Class Patterns**: Understanding the extensibility architecture
- **Memory Management**: Resource lifecycle and disposal patterns
- **Performance Optimization**: Caching, lazy initialization, and efficient operations
- **Custom Feature Implementations**: Creating specialized licensing behaviors
- **Cross-Platform Considerations**: Platform-specific optimizations and fallbacks

## Generic Type-Safe Licensing

### LicenseManager<T> Pattern

The `LicenseManager(Of TFeatures)` class provides a type-safe abstraction over the base licensing functionality:

```vb
Public MustInherit Class LicenseManager(Of TFeatures As Structure)
    Inherits LicenseManagerBase
```

#### Benefits of Generic Implementation

1. **Compile-Time Safety**: Features are validated at compile time
2. **IntelliSense Support**: IDE provides autocomplete for available features
3. **Type Conversion**: Automatic conversion between enums and string representations
4. **Reduced Errors**: Eliminates string-based feature name typos

#### Feature Enum Requirements

Feature enumerations must follow specific patterns:

```vb
<Flags>
Public Enum ApplicationFeature As Integer
    None = 0
    BasicFeature = 1
    AdvancedFeature = 2
    PremiumFeature = 4
    ExpertFeature = 8
    ' Powers of 2 for bitwise operations
End Enum
```

### Generic Implementation Details

#### Feature Name Resolution

```vb
Protected Friend Overrides ReadOnly Property MainFeature As String
    Get
        Static _mainFeature As Nullable(Of KeyValuePair(Of String, Object)) = Nothing
        If Not _mainFeature.HasValue Then
            ' Find first non-ignored feature as main feature
            _mainFeature = [Enum](Of TFeatures).GetValuesAndNames.
                Where(Function(kv) Not IgnoreFeatures.Any(Function(ft) ft.ToLower = kv.Key.ToLower)).
                First
        End If
        Return _mainFeature.GetValueOrDefault.Key
    End Get
End Property
```

#### Feature String Conversion

```vb
Protected Overrides Function TryGetFeatureStringAsValue(featureName As String, ByRef value As Object) As Boolean
    ' Use reflection-based enum utilities for type-safe conversion
    Return [Enum](Of TFeatures).TryGetValueOfName(featureName, value, True)
End Function

Protected Overrides Function GetFeaturesAsString(result As FeaturesResult) As String()
    Dim stringFeatures As New List(Of String)
    Dim features As TFeatures = Nothing
    
    ' Convert numeric registry value back to enum flags
    If IsNumeric(result.Features) AndAlso [Enum].TryParse(CInt(result.Features).ToString, features) Then
        stringFeatures = [Enum](Of TFeatures).GetCurrentValuesAndNames(features).Keys.ToList
    ElseIf MainFeature IsNot Nothing Then
        ' Fallback to main feature
        stringFeatures.Add(MainFeature)
    End If
    
    ' Filter out ignored features
    Dim eFeatures As IEnumerable(Of String) = stringFeatures.Where(Function(s) IgnoreFeatures.Contains(s.ToLower))
    For Each sf As String In eFeatures.ToArray
        stringFeatures.Remove(sf)
    Next
    
    Return stringFeatures.ToArray
End Function
```

#### Bitwise Feature Operations

```vb
Protected Function GetAllFeaturesAsFlag() As Integer
    Dim valuesAndNames As IReadOnlyDictionary(Of String, Object) = [Enum](Of TFeatures).GetValuesAndNames
    Dim features As Integer = 0
    
    For Each ftName As String In GetAllFeatureNames()
        If valuesAndNames.ContainsKey(ftName) Then
            Dim value As Object = valuesAndNames(ftName)
            features = features Or CInt(value)  ' Bitwise OR for flag combination
        End If
    Next
    
    Return features
End Function

Public Function GetAvailableFeaturesAsFlag() As Integer
    Dim valuesAndNames As IReadOnlyDictionary(Of String, Object) = [Enum](Of TFeatures).GetValuesAndNames
    Dim features As Integer = 0
    
    For Each ft As TFeatures In GetAvailableFeatures()
        If valuesAndNames.ContainsKey(ft.ToString) Then
            Dim value As Object = valuesAndNames(ft.ToString)
            features = features Or CInt(value)
        End If
    Next
    
    Return features
End Function
```

## Abstract Base Class Architecture

### Extensibility Patterns

The licensing system uses the Template Method pattern extensively:

```vb
' Base class defines the algorithm
Public Function InitAndAuthenticateFeatures(Optional features As String() = Nothing) As Result
    ' Template method - calls abstract methods implemented by subclasses
    Dim initResult As Result = Init()
    If initResult.IsSuccess Then
        Return AuthenticateFeatures(features)
    End If
    Return initResult
End Function

' Abstract methods that must be implemented
Protected Friend MustOverride Function GetAllFeatureNames() As String()
Protected Friend MustOverride Function GetAllFeatureValues() As Object()
Protected MustOverride Function GetFeaturesAsString(result As FeaturesResult) As String()
Protected MustOverride Function TryGetFeatureStringAsValue(featureName As String, ByRef value As Object) As Boolean
Protected Friend MustOverride ReadOnly Property Vendor As String
```

### Custom License Manager Implementation

```vb
Public Class CustomLicenseManager
    Inherits LicenseManager(Of CustomFeature)
    
    ' Required implementations
    Protected Overrides ReadOnly Property Vendor As String
        Get
            Return "MyCompany"
        End Get
    End Property
    
    Public Overrides ReadOnly Property ProductName As String
        Get
            Return "MyApplication"
        End Get
    End Property
    
    ' Optional overrides for customization
    Protected Overrides Sub InitAvailableFeatures(
        Optional createRegKeys As Boolean = False,
        Optional availableFeaturesKeySetting As AvailableFeatureKeySettings = Nothing
    )
        ' Custom registry configuration
        If availableFeaturesKeySetting Is Nothing Then
            availableFeaturesKeySetting = New AvailableFeatureKeySettings(
                Nothing,                ' Don't create new keys automatically
                GetAllFeaturesAsFlag   ' Default to all features if registry missing
            )
        End If
        
        MyBase.InitAvailableFeatures(createRegKeys, availableFeaturesKeySetting)
    End Sub
    
    ' Custom ignored features
    Protected Overrides ReadOnly Property IgnoreFeatures As String()
        Get
            Return {"DebugFeature", "TestFeature"}
        End Get
    End Property
End Class
```

## Memory Management and Resource Lifecycle

### IDisposable Implementation

The licensing system implements proper resource disposal:

```vb
Partial Public Class LicenseManagerBase
    Implements IDisposable
    
    Private disposedValue As Boolean
    
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' Dispose managed resources
                TerminateLicense()
                _licenseWrapper.Dispose()
                _lmBorrowFile?.Dispose()
                _lmStatFile?.Dispose()
            End If
            
            ' Clear references
            _lmStatFile = Nothing
            _lmBorrowFile = Nothing
            _licenseWrapper = Nothing
        End If
        disposedValue = True
    End Sub
    
    Public Sub Dispose() Implements IDisposable.Dispose
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
End Class
```

### Resource Management Patterns

#### RAII (Resource Acquisition Is Initialization)

```vb
Public Class SafeLicenseManager
    Inherits LicenseManager(Of ApplicationFeature)
    
    Private _isDisposed As Boolean = False
    
    Protected Overrides Sub Dispose(disposing As Boolean)
        If Not _isDisposed AndAlso disposing Then
            ' Ensure all resources are properly cleaned up
            Try
                If _initialized Then
                    TerminateLicense()
                End If
            Catch ex As Exception
                ' Log disposal errors but don't throw
                Logger.LogWarning($"Error during license disposal: {ex.Message}")
            End Try
        End If
        
        MyBase.Dispose(disposing)
        _isDisposed = True
    End Sub
    
    Private Sub ThrowIfDisposed()
        If _isDisposed Then
            Throw New ObjectDisposedException(GetType(SafeLicenseManager).Name)
        End If
    End Sub
    
    Public Overrides Function Init(Optional createRegKeys As Boolean = False) As Result
        ThrowIfDisposed()
        Return MyBase.Init(createRegKeys)
    End Function
End Class
```

#### Temporary Resource Management

```vb
Public Class TempFile
    Implements IDisposable
    
    Private ReadOnly _filePath As String
    Private _disposed As Boolean = False
    
    Public Sub New(content As String, Optional extension As String = ".tmp")
        _filePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + extension)
        File.WriteAllText(_filePath, content)
    End Sub
    
    Public ReadOnly Property FilePath As String
        Get
            If _disposed Then
                Throw New ObjectDisposedException(GetType(TempFile).Name)
            End If
            Return _filePath
        End Get
    End Property
    
    Public Sub Dispose() Implements IDisposable.Dispose
        If Not _disposed Then
            Try
                If File.Exists(_filePath) Then
                    File.Delete(_filePath)
                End If
            Catch
                ' Ignore cleanup errors
            End Try
            _disposed = True
        End If
    End Sub
End Class
```

## Performance Optimization Strategies

### Lazy Initialization Patterns

```vb
Public Class OptimizedLicenseManager
    Inherits LicenseManager(Of ApplicationFeature)
    
    Private _lazyFeatureCache As Lazy(Of Dictionary(Of String, Object))
    Private _lazyRegistryAccess As Lazy(Of Boolean)
    
    Public Sub New()
        MyBase.New()
        InitializeLazyProperties()
    End Sub
    
    Private Sub InitializeLazyProperties()
        _lazyFeatureCache = New Lazy(Of Dictionary(Of String, Object))(
            Function() BuildFeatureCache(),
            Threading.LazyThreadSafetyMode.ExecutionAndPublication
        )
        
        _lazyRegistryAccess = New Lazy(Of Boolean)(
            Function() TestRegistryAccess(),
            Threading.LazyThreadSafetyMode.ExecutionAndPublication
        )
    End Sub
    
    Private Function BuildFeatureCache() As Dictionary(Of String, Object)
        Dim cache As New Dictionary(Of String, Object)
        For Each kv In [Enum](Of ApplicationFeature).GetValuesAndNames()
            cache(kv.Key.ToLower()) = kv.Value
        Next
        Return cache
    End Function
    
    Protected Overrides Function TryGetFeatureStringAsValue(featureName As String, ByRef value As Object) As Boolean
        Return _lazyFeatureCache.Value.TryGetValue(featureName.ToLower(), value)
    End Function
End Class
```

### Caching Strategies

```vb
Public Class CachedLicenseManager
    Inherits LicenseManager(Of ApplicationFeature)
    
    Private ReadOnly _authCache As New ConcurrentDictionary(Of String, (Result As AuthFeaturesResult, Timestamp As DateTime))
    Private ReadOnly _cacheTimeout As TimeSpan = TimeSpan.FromMinutes(5)
    
    Public Overrides Function AuthenticateFeatures(features As String()) As AuthFeaturesResult
        Dim cacheKey As String = String.Join(",", features.OrderBy(Function(f) f))
        
        ' Check cache first
        If _authCache.TryGetValue(cacheKey, out Dim cachedResult) Then
            If DateTime.UtcNow - cachedResult.Timestamp < _cacheTimeout Then
                Logger.LogDebug($"Using cached authentication result for: {cacheKey}")
                Return cachedResult.Result
            Else
                ' Remove expired entry
                _authCache.TryRemove(cacheKey, out Dim _)
            End If
        End If
        
        ' Perform authentication
        Dim result As AuthFeaturesResult = MyBase.AuthenticateFeatures(features)
        
        ' Cache successful results
        If result.IsSuccess Then
            _authCache.TryAdd(cacheKey, (result, DateTime.UtcNow))
        End If
        
        Return result
    End Function
    
    Protected Overrides Sub Dispose(disposing As Boolean)
        If disposing Then
            _authCache.Clear()
        End If
        MyBase.Dispose(disposing)
    End Sub
End Class
```

### Asynchronous Operations

```vb
Public Class AsyncLicenseManager
    Inherits LicenseManager(Of ApplicationFeature)
    
    Public Async Function InitAndAuthenticateFeaturesAsync(
        features As String(),
        Optional cancellationToken As CancellationToken = Nothing
    ) As Task(Of AuthFeaturesResult)
        
        Return Await Task.Run(Function()
            ' Ensure cancellation is checked
            cancellationToken.ThrowIfCancellationRequested()
            
            ' Perform initialization
            Dim initResult As Result = Init()
            If Not initResult.IsSuccess Then
                Return New AuthFeaturesResult(
                    New [Error](initResult.Message, ErrorCodes.ERROR_GEN_FAILURE),
                    LicensingErrorCodes.NOT_INITIALIZED
                )
            End If
            
            ' Check cancellation before authentication
            cancellationToken.ThrowIfCancellationRequested()
            
            ' Perform authentication
            Return AuthenticateFeatures(features)
        End Function, cancellationToken)
    End Function
    
    Public Async Function GetLicenseStatusAsync(
        Optional timeout As TimeSpan = Nothing
    ) As Task(Of StatusResult)
        
        If timeout = TimeSpan.Zero Then
            timeout = TimeSpan.FromSeconds(30)
        End If
        
        Using cts As New CancellationTokenSource(timeout)
            Try
                Return Await Task.Run(Function() GetStatus(), cts.Token)
            Catch ex As OperationCanceledException
                Return New StatusResult(
                    ResultState.Faulted,
                    "License status check timed out",
                    ErrorCodes.ERROR_TIMEOUT
                )
            End Try
        End Using
    End Function
End Class
```

## Custom Feature Implementation Patterns

### Feature Groups and Dependencies

```vb
Public Class GroupedFeatureLicenseManager
    Inherits LicenseManager(Of ApplicationFeature)
    
    Private ReadOnly _featureGroups As New Dictionary(Of String, ApplicationFeature()) From {
        {"Basic", {ApplicationFeature.Core, ApplicationFeature.BasicReports}},
        {"Professional", {ApplicationFeature.Core, ApplicationFeature.BasicReports, ApplicationFeature.AdvancedReports, ApplicationFeature.DataExport}},
        {"Enterprise", {ApplicationFeature.Core, ApplicationFeature.BasicReports, ApplicationFeature.AdvancedReports, ApplicationFeature.DataExport, ApplicationFeature.APIAccess, ApplicationFeature.CustomScripts}}
    }
    
    Private ReadOnly _featureDependencies As New Dictionary(Of ApplicationFeature, ApplicationFeature()) From {
        {ApplicationFeature.AdvancedReports, {ApplicationFeature.Core, ApplicationFeature.BasicReports}},
        {ApplicationFeature.CustomScripts, {ApplicationFeature.Core, ApplicationFeature.APIAccess}}
    }
    
    Public Function AuthenticateFeatureGroup(groupName As String) As AuthFeaturesResult
        If Not _featureGroups.ContainsKey(groupName) Then
            Return New AuthFeaturesResult(
                New [Error]($"Unknown feature group: {groupName}", ErrorCodes.ERROR_INVALID_PARAMETER),
                LicensingErrorCodes.AUTH_FAILED
            )
        End If
        
        Dim features As ApplicationFeature() = _featureGroups(groupName)
        Return AuthenticateFeatures(features)
    End Function
    
    Public Function ValidateFeatureDependencies(features As ApplicationFeature()) As Result
        For Each feature In features
            If _featureDependencies.ContainsKey(feature) Then
                Dim dependencies As ApplicationFeature() = _featureDependencies(feature)
                For Each dependency In dependencies
                    If Not features.Contains(dependency) Then
                        Return New Result(
                            ResultState.Faulted,
                            $"Feature {feature} requires {dependency} but it was not requested"
                        )
                    End If
                Next
            End If
        Next
        
        Return New Result(ResultState.Success)
    End Function
End Class
```

### Dynamic Feature Discovery

```vb
Public Class DynamicFeatureLicenseManager
    Inherits LicenseManagerBase
    
    Private _dynamicFeatures As Dictionary(Of String, Object)
    
    Protected Overrides Sub InitAvailableFeatures(Optional createRegKeys As Boolean = False, Optional availableFeaturesKeySetting As AvailableFeatureKeySettings = Nothing)
        ' Load features from configuration
        LoadDynamicFeatures()
        MyBase.InitAvailableFeatures(createRegKeys, availableFeaturesKeySetting)
    End Sub
    
    Private Sub LoadDynamicFeatures()
        _dynamicFeatures = New Dictionary(Of String, Object)
        
        ' Load from configuration file
        Try
            Dim configPath As String = Path.Combine(Application.StartupPath, "features.json")
            If File.Exists(configPath) Then
                Dim json As String = File.ReadAllText(configPath)
                Dim config = JsonConvert.DeserializeObject(Of Dictionary(Of String, Integer))(json)
                
                For Each kv In config
                    _dynamicFeatures(kv.Key) = kv.Value
                Next
            End If
        Catch ex As Exception
            Logger.LogWarning($"Failed to load dynamic features: {ex.Message}")
        End Try
        
        ' Ensure at least a main feature exists
        If Not _dynamicFeatures.Any() Then
            _dynamicFeatures("MainFeature") = 1
        End If
    End Sub
    
    Protected Friend Overrides Function GetAllFeatureNames() As String()
        Return _dynamicFeatures.Keys.ToArray()
    End Function
    
    Protected Friend Overrides Function GetAllFeatureValues() As Object()
        Return _dynamicFeatures.Values.ToArray()
    End Function
    
    Protected Overrides Function TryGetFeatureStringAsValue(featureName As String, ByRef value As Object) As Boolean
        Return _dynamicFeatures.TryGetValue(featureName, value)
    End Function
End Class
```

## Cross-Platform Considerations

### Platform-Specific Implementations

```vb
Public Class CrossPlatformLicenseManager
    Inherits LicenseManager(Of ApplicationFeature)
    
    Protected Overrides Sub InitAvailableFeatures(Optional createRegKeys As Boolean = False, Optional availableFeaturesKeySetting As AvailableFeatureKeySettings = Nothing)
#If WINDOWS Then
        ' Full registry support on Windows
        MyBase.InitAvailableFeatures(createRegKeys, availableFeaturesKeySetting)
#ElseIf NETCOREAPP Then
        ' Alternative storage on .NET Core
        InitFeaturesCrossplatform(createRegKeys, availableFeaturesKeySetting)
#Else
        ' Fallback for other platforms
        InitFeaturesLegacy(createRegKeys, availableFeaturesKeySetting)
#End If
    End Sub
    
    Private Sub InitFeaturesCrossplatform(createKeys As Boolean, settings As AvailableFeatureKeySettings)
        ' Use app settings or configuration files instead of registry
        Dim configPath As String = GetConfigurationPath()
        
        If createKeys AndAlso Not File.Exists(configPath) Then
            CreateDefaultConfiguration(configPath, settings)
        End If
        
        LoadFeaturesFromConfiguration(configPath, settings)
    End Sub
    
    Private Function GetConfigurationPath() As String
        Dim appDataPath As String = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
        Dim companyPath As String = Path.Combine(appDataPath, COMPANY_NAME)
        Dim productPath As String = Path.Combine(companyPath, ProductName)
        
        Directory.CreateDirectory(productPath)
        Return Path.Combine(productPath, "licensing.json")
    End Function
    
    Private Sub CreateDefaultConfiguration(configPath As String, settings As AvailableFeatureKeySettings)
        Dim config As New Dictionary(Of String, Object) From {
            {"AvailableFeatures", If(settings?.ValueCreateNew, GetAllFeaturesAsFlag())},
            {"Created", DateTime.UtcNow.ToString("O")},
            {"Product", ProductName}
        }
        
        Dim json As String = JsonConvert.SerializeObject(config, Formatting.Indented)
        File.WriteAllText(configPath, json)
    End Sub
End Class
```

### Conditional Compilation Patterns

```vb
#If WINDOWS Or NETFRAMEWORK Then
    ' Windows-specific code
    Private Function GetWindowsRegistryValue() As Object
        Return Registry.CurrentUser.OpenSubKey("Software\Zuken\" & ProductName)?.GetValue("AvailableFeatures")
    End Function
#Else
    ' Cross-platform alternative
    Private Function GetCrossPlatformConfigValue() As Object
        Dim configPath As String = GetConfigurationPath()
        If File.Exists(configPath) Then
            Dim json As String = File.ReadAllText(configPath)
            Dim config = JsonConvert.DeserializeObject(Of Dictionary(Of String, Object))(json)
            Return config.GetValueOrDefault("AvailableFeatures")
        End If
        Return Nothing
    End Function
#End If

Public Function GetStoredFeatures() As Object
#If WINDOWS Or NETFRAMEWORK Then
    Return GetWindowsRegistryValue()
#Else
    Return GetCrossPlatformConfigValue()
#End If
End Function
```

## Advanced Error Handling Strategies

### Circuit Breaker Pattern

```vb
Public Class ResilientLicenseManager
    Inherits LicenseManager(Of ApplicationFeature)
    
    Private ReadOnly _circuitBreaker As New CircuitBreaker(
        failureThreshold:=5,
        timeout:=TimeSpan.FromMinutes(2)
    )
    
    Public Overrides Function AuthenticateFeatures(features As String()) As AuthFeaturesResult
        Return _circuitBreaker.Execute(Function() MyBase.AuthenticateFeatures(features))
    End Function
End Class

Public Class CircuitBreaker
    Private ReadOnly _failureThreshold As Integer
    Private ReadOnly _timeout As TimeSpan
    Private _failureCount As Integer = 0
    Private _lastFailureTime As DateTime?
    private _state As CircuitBreakerState = CircuitBreakerState.Closed
    
    Public Function Execute(Of T)(operation As Func(Of T)) As T
        Select Case _state
            Case CircuitBreakerState.Open
                If DateTime.UtcNow - _lastFailureTime.Value > _timeout Then
                    _state = CircuitBreakerState.HalfOpen
                    Return AttemptOperation(operation)
                Else
                    Throw New CircuitBreakerOpenException("Circuit breaker is open")
                End If
                
            Case CircuitBreakerState.HalfOpen, CircuitBreakerState.Closed
                Return AttemptOperation(operation)
        End Select
    End Function
    
    Private Function AttemptOperation(Of T)(operation As Func(Of T)) As T
        Try
            Dim result As T = operation()
            OnSuccess()
            Return result
        Catch ex As Exception
            OnFailure()
            Throw
        End Try
    End Function
End Class
```

## Related Components

### Dependencies for Advanced Concepts
- **System.Reflection**: Enum utilities and dynamic type operations
- **System.Threading**: Asynchronous operations and thread safety
- **System.Collections.Concurrent**: Thread-safe collections
- **Newtonsoft.Json**: Cross-platform configuration serialization

### Integration Points
- **Configuration Systems**: External feature configuration
- **Monitoring Solutions**: Performance metrics and health checks
- **Caching Frameworks**: Distributed caching for enterprise scenarios
- **Dependency Injection**: IoC container integration patterns

---

**Previous:** [10 - Error Handling and Diagnostics](./Documentation-10-Error-Handling-Diagnostics.md) | **Next:** [12 - Integration Examples](./Documentation-12-Integration-Examples.md)
