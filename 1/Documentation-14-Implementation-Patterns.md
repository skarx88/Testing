# Implementation Patterns

This section explores the design patterns used in the E3.Lib.Licensing library. Understanding these patterns provides insight into the architectural decisions that shape the library and can help developers work with and extend the codebase effectively.

## Abstract Base Class Pattern

The E3.Lib.Licensing library uses an abstract base class pattern to provide core functionality while allowing type-safe specialization.

### LicenseManagerBase Abstract Foundation

The `LicenseManagerBase` provides the fundamental licensing infrastructure:

```vb
' Abstract base class defining core licensing operations
Public MustInherit Class LicenseManagerBase
    Implements IDisposable
    
    ' Template method pattern - defines algorithm structure
    Public Function InitAndAuthenticateFeatures(Optional features As String() = Nothing) As Result
        Dim resInit As Result = Init(createRegKeys)
        If resInit.IsSuccess Then
            Return AuthenticateFeatures(features)
        End If
        Return resInit
    End Function
    
    ' Abstract properties that derived classes must implement
    Protected Friend MustOverride ReadOnly Property MainFeature As String
    Protected MustOverride Function TryGetFeatureStringAsValue(featureName As String, ByRef value As Object) As Boolean
    Protected MustOverride Function GetFeaturesAsString(result As FeaturesResult) As String()
    
    ' Virtual methods that can be overridden
    Protected Overridable Sub InitAvailableFeatures(Optional createRegKeys As Boolean = False)
        ' Default implementation
    End Sub
End Class
```

### Type-Safe Generic Specialization

The generic `LicenseManager(Of TFeatures)` provides compile-time type safety:

```vb
' Type-safe specialization of the abstract base
Public MustInherit Class LicenseManager(Of TFeatures As Structure)
    Inherits LicenseManagerBase
    
    ' Implements abstract methods with type-safe logic
    Protected Overrides Function TryGetFeatureStringAsValue(featureName As String, ByRef value As Object) As Boolean
        Return [Enum](Of TFeatures).TryGetValueOfName(featureName, value, True)
    End Function
    
    ' Type-safe feature authentication
    Public Overloads Function AuthenticateFeatures(ParamArray features As TFeatures()) As AuthFeaturesResult
        Dim featureNames As New List(Of String)
        For Each feature As TFeatures In features
            featureNames.AddRange([Enum](Of TFeatures).GetCurrentValuesAndNames(feature).Keys)
        Next
        Return MyBase.AuthenticateFeatures(featureNames.ToArray)
    End Function
End Class
```

## Factory Patterns

### Abstract Factory for Platform Wrappers

The library uses an abstract factory pattern to create platform-specific FlexNet wrappers:

```vb
' Abstract factory for creating platform-specific wrappers
Public Class GeneralLicenseWrapper
    Private _wrapper As INativeLicenseWrapper
    
    Public Sub New()
        _wrapper = CreatePlatformWrapper()
    End Sub
    
    Private Function CreatePlatformWrapper() As INativeLicenseWrapper
        ' Factory method based on runtime environment
        Select Case Environment.Version.Major
            Case 4
                Return New NetFramework472Wrapper()
            Case 6
                Return New Net6Wrapper()
            Case 8
                Return New Net8Wrapper()
            Case Else
                Throw New NotSupportedException($"Framework version {Environment.Version} not supported")
        End Select
    End Function
    
    ' Facade methods delegate to concrete wrapper
    Public Function InitLicensing(productName As String) As Integer
        Return _wrapper.InitLicensing(productName)
    End Function
End Class
```

### Result Factory Pattern

Different result types are created using factory methods:

```vb
' Result factory methods for different operation types
Public Class ResultFactory
    Public Shared Function CreateSuccess() As Result
        Return New Result(ResultState.Success)
    End Function
    
    Public Shared Function CreateFailure(message As String) As Result
        Return New Result(ResultState.Failed, message)
    End Function
    
    Public Shared Function CreateLicenseError(errorCode As LicensingErrorCodes) As LicenseErrorCodeResult
        Return New LicenseErrorCodeResult(ResultState.Failed, errorCode)
    End Function
    
    Public Shared Function CreateFeatureResult(success As Boolean, features As Object) As FeaturesResult
        If success Then
            Return New FeaturesResult(ResultState.Success) With {.Features = features}
        Else
            Return New FeaturesResult(ResultState.Failed, "Feature authentication failed")
        End If
    End Function
End Class
```

### TempFile Factory Pattern

The `TempFile` class uses factory methods for different creation scenarios:

```vb
' Factory methods for different temporary file scenarios
Public Class TempFile
    Implements IDisposable
    
    ' Factory method for new empty temp file
    Public Shared Function CreateNew(Optional withExtension As String = Nothing) As TempFile
        Return New TempFile(System.IO.PathEx.GetTempFileName(withExtension), False, True)
    End Function
    
    ' Factory method for temp file with stream content
    Public Shared Function CreateNewWithStreamContent(s As System.IO.Stream, Optional withExtension As String = Nothing) As TempFile
        Dim tempFile As New TempFile(System.IO.PathEx.GetTempFileName(extension))
        tempFile.WriteStream(s)
        Return tempFile
    End Function
    
    ' Factory method for named temp file
    Public Shared Function Create(fileName As String, Optional withExtension As String = Nothing) As TempFile
        If Not String.IsNullOrEmpty(withExtension) Then
            Return New TempFile(System.IO.Path.Combine(System.IO.Path.GetTempPath, 
                System.IO.Path.ChangeExtension(System.IO.Path.GetFileName(fileName), withExtension)))
        Else
            Return New TempFile(System.IO.Path.Combine(System.IO.Path.GetTempPath, 
                System.IO.Path.GetFileName(fileName)))
        End If
    End Function
End Class
```

## Strategy Pattern Implementation

### Error Handling Strategies

Different error handling strategies are used based on operation context:

```vb
' Strategy pattern for different error handling approaches
Public Interface IErrorHandlingStrategy
    Function HandleError(ex As Exception, context As String) As Result
End Interface

Public Class LicensingErrorStrategy
    Implements IErrorHandlingStrategy
    
    Public Function HandleError(ex As Exception, context As String) As Result Implements IErrorHandlingStrategy.HandleError
        ' Map FlexNet errors to licensing error codes
        Select Case ex.HResult
            Case -2147467259 ' FlexNet server unavailable
                Return New LicenseErrorCodeResult(ResultState.Failed, LicensingErrorCodes.ServerUnavailable)
            Case -2147467260 ' Feature expired
                Return New LicenseErrorCodeResult(ResultState.Failed, LicensingErrorCodes.FeatureExpired)
            Case Else
                Return New LicenseErrorCodeResult(ResultState.Failed, LicensingErrorCodes.GeneralError)
        End Select
    End Function
End Class

Public Class GeneralErrorStrategy
    Implements IErrorHandlingStrategy
    
    Public Function HandleError(ex As Exception, context As String) As Result Implements IErrorHandlingStrategy.HandleError
        ' Generic error handling for non-licensing operations
        Return New Result(ex)
    End Function
End Class
```

### Result Processing Strategies

Different strategies for processing and formatting results:

```vb
' Strategy pattern for result processing
Public Interface IResultProcessor
    Function ProcessResult(result As Object) As String()
End Interface

Public Class EnumResultProcessor
    Implements IResultProcessor
    
    Public Function ProcessResult(result As Object) As String() Implements IResultProcessor.ProcessResult
        ' Process enum-based feature results
        If TypeOf result Is [Enum] Then
            Return [Enum].GetNames(result.GetType()).Where(Function(name) 
                CInt([Enum].Parse(result.GetType(), name)) And CInt(result) = CInt([Enum].Parse(result.GetType(), name))
            ).ToArray()
        End If
        Return New String() {}
    End Function
End Class

Public Class StringArrayResultProcessor
    Implements IResultProcessor
    
    Public Function ProcessResult(result As Object) As String() Implements IResultProcessor.ProcessResult
        ' Process string array results
        If TypeOf result Is String() Then
            Return CType(result, String())
        ElseIf TypeOf result Is String Then
            Return New String() {CType(result, String)}
        End If
        Return New String() {}
    End Function
End Class
```

## Template Method Pattern

### Licensing Operation Template

The base class defines the algorithm structure while allowing customization:

```vb
' Template method pattern in licensing operations
Public MustInherit Class LicenseManagerBase
    ' Template method defining the algorithm
    Public Function AuthenticateFeatures(features As String()) As FeaturesResult
        ' Step 1: Validate input (fixed)
        If features Is Nothing OrElse features.Length = 0 Then
            features = New String() {MainFeature}
        End If
        
        ' Step 2: Process authentication (customizable)
        Dim authResult = PerformAuthentication(features)
        
        ' Step 3: Update state (fixed)
        UpdateAuthenticatedFeatures(authResult)
        
        ' Step 4: Create result (customizable)
        Return CreateAuthenticationResult(authResult)
    End Function
    
    ' Abstract methods for customization
    Protected MustOverride Function PerformAuthentication(features As String()) As Object
    Protected MustOverride Function CreateAuthenticationResult(authResult As Object) As FeaturesResult
    
    ' Concrete methods (fixed steps)
    Private Sub UpdateAuthenticatedFeatures(authResult As Object)
        ' Update internal state
    End Sub
End Class
```

### Initialization Template

The initialization process follows a template pattern:

```vb
' Template method for initialization
Public MustInherit Class LicenseManagerBase
    Public Function Init(Optional createRegKeys As Boolean = False) As Result
        Try
            ' Step 1: Setup registry (customizable)
            SetupRegistryKeys(createRegKeys)
            
            ' Step 2: Initialize wrapper (fixed)
            InitializeLicenseWrapper()
            
            ' Step 3: Setup features (customizable)
            SetupAvailableFeatures(createRegKeys)
            
            ' Step 4: Validate setup (fixed)
            ValidateInitialization()
            
            Return New Result(ResultState.Success)
        Catch ex As Exception
            Return New Result(ex)
        End Try
    End Function
    
    ' Customizable steps
    Protected Overridable Sub SetupRegistryKeys(createRegKeys As Boolean)
        ' Default implementation
    End Sub
    
    Protected Overridable Sub SetupAvailableFeatures(createRegKeys As Boolean)
        ' Default implementation
    End Sub
    
    ' Fixed steps
    Private Sub InitializeLicenseWrapper()
        ' Always the same logic
    End Sub
    
    Private Sub ValidateInitialization()
        ' Always the same validation
    End Sub
End Class
```

## Facade Pattern Implementation

### LicenseManagerBase as Facade

The `LicenseManagerBase` acts as a facade over complex FlexNet operations:

```vb
' Facade pattern simplifying complex operations
Public MustInherit Class LicenseManagerBase
    Private _licenseWrapper As GeneralLicenseWrapper
    Private _registryManager As RegistryManager
    Private _featureValidator As FeatureValidator
    
    ' Simplified facade methods
    Public Function InitAndAuthenticateFeatures(features As String()) As Result
        ' Coordinates multiple subsystems
        Dim initResult = InitializeSubsystems()
        If Not initResult.IsSuccess Then Return initResult
        
        Dim registryResult = SetupRegistry()
        If Not registryResult.IsSuccess Then Return registryResult
        
        Dim authResult = AuthenticateWithFlexNet(features)
        If Not authResult.IsSuccess Then Return authResult
        
        Return ValidateAndFinalize(features)
    End Function
    
    ' Private methods coordinate subsystems
    Private Function InitializeSubsystems() As Result
        Try
            _licenseWrapper.Initialize()
            _registryManager.Initialize()
            _featureValidator.Initialize()
            Return New Result(ResultState.Success)
        Catch ex As Exception
            Return New Result(ex)
        End Try
    End Function
    
    Private Function SetupRegistry() As Result
        Return _registryManager.SetupLicensingKeys()
    End Function
    
    Private Function AuthenticateWithFlexNet(features As String()) As Result
        Return _licenseWrapper.AuthenticateFeatures(features)
    End Function
End Class
```

### GeneralLicenseWrapper as Facade

The wrapper provides a simplified interface to platform-specific implementations:

```vb
' Facade over platform-specific implementations
Public Class GeneralLicenseWrapper
    Private _nativeWrapper As INativeLicenseWrapper
    
    ' Simplified interface methods
    Public Function InitLicensing(productName As String) As Integer
        ' Coordinates initialization across platforms
        Try
            ValidateProduct(productName)
            SetupEnvironment()
            Return _nativeWrapper.InitLicensing(productName)
        Catch ex As Exception
            Return -1
        End Try
    End Function
    
    Public Function AuthenticateFeature(featureName As String) As Integer
        ' Simplified authentication
        Try
            ValidateFeature(featureName)
            PrepareLicenseCheck()
            Return _nativeWrapper.AuthenticateFeature(featureName)
        Catch ex As Exception
            Return -1
        End Try
    End Function
    
    ' Private coordination methods
    Private Sub ValidateProduct(productName As String)
        If String.IsNullOrEmpty(productName) Then
            Throw New ArgumentException("Product name cannot be empty")
        End If
    End Sub
    
    Private Sub SetupEnvironment()
        ' Setup common environment variables
    End Sub
End Class
```

## Disposable Resource Pattern

### Comprehensive Resource Management

The library implements comprehensive resource management:

```vb
' Disposable pattern with comprehensive cleanup
Public MustInherit Class LicenseManagerBase
    Implements IDisposable
    
    Private _disposed As Boolean = False
    Private _licenseWrapper As GeneralLicenseWrapper
    Private _lmBorrowFile As TempFile
    Private _lmStatFile As TempFile
    
    ' Public dispose method
    Public Sub Dispose() Implements IDisposable.Dispose
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
    
    ' Protected virtual dispose method
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not _disposed Then
            If disposing Then
                ' Dispose managed resources
                _licenseWrapper?.Dispose()
                _lmBorrowFile?.Dispose()
                _lmStatFile?.Dispose()
                
                ' Remove event handlers
                RemoveHandler AppDomain.CurrentDomain.ProcessExit, AddressOf _process_exit
            End If
            
            ' Cleanup unmanaged resources if any
            CleanupUnmanagedResources()
            
            _disposed = True
        End If
    End Sub
    
    ' Finalizer
    Protected Overrides Sub Finalize()
        Dispose(False)
    End Sub
    
    ' Helper method for unmanaged cleanup
    Private Sub CleanupUnmanagedResources()
        ' Release any unmanaged resources
    End Sub
    
    ' Process exit handler
    Private Sub _process_exit(sender As Object, e As EventArgs)
        Dispose()
    End Sub
End Class
```

### TempFile Resource Management

Temporary files implement careful resource management:

```vb
' TempFile disposable pattern
Public Class TempFile
    Implements IDisposable
    
    Private _disposedValue As Boolean
    Private _leaveAlone As Boolean = False
    Private _openStream As System.IO.Stream
    Private _filePath As String
    
    Public Sub Dispose() Implements IDisposable.Dispose
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
    
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not _disposedValue Then
            If disposing Then
                ' Close any open streams
                _openStream?.Close()
                _openStream?.Dispose()
            End If
            
            ' Delete the temporary file unless flagged to leave alone
            If Not _leaveAlone AndAlso Not String.IsNullOrEmpty(_filePath) Then
                Try
                    If File.Exists(_filePath) Then
                        File.Delete(_filePath)
                    End If
                Catch
                    ' Ignore deletion errors
                End Try
            End If
            
            _disposedValue = True
        End If
    End Sub
    
    Protected Overrides Sub Finalize()
        Dispose(False)
    End Sub
End Class
```

## Registry Strategy Pattern

### Registry Operation Strategies

Different strategies for registry operations based on context:

```vb
' Strategy pattern for registry operations
Public Interface IRegistryStrategy
    Function ReadValue(keyPath As String, valueName As String) As Object
    Sub WriteValue(keyPath As String, valueName As String, value As Object)
    Function KeyExists(keyPath As String) As Boolean
End Interface

Public Class CurrentUserRegistryStrategy
    Implements IRegistryStrategy
    
    Public Function ReadValue(keyPath As String, valueName As String) As Object Implements IRegistryStrategy.ReadValue
        Using key As RegistryKey = Registry.CurrentUser.OpenSubKey(keyPath)
            Return key?.GetValue(valueName)
        End Using
    End Function
    
    Public Sub WriteValue(keyPath As String, valueName As String, value As Object) Implements IRegistryStrategy.WriteValue
        Using key As RegistryKey = Registry.CurrentUser.CreateSubKey(keyPath)
            key.SetValue(valueName, value)
        End Using
    End Sub
    
    Public Function KeyExists(keyPath As String) As Boolean Implements IRegistryStrategy.KeyExists
        Using key As RegistryKey = Registry.CurrentUser.OpenSubKey(keyPath)
            Return key IsNot Nothing
        End Using
    End Function
End Class

Public Class LocalMachineRegistryStrategy
    Implements IRegistryStrategy
    
    Public Function ReadValue(keyPath As String, valueName As String) As Object Implements IRegistryStrategy.ReadValue
        Using key As RegistryKey = Registry.LocalMachine.OpenSubKey(keyPath)
            Return key?.GetValue(valueName)
        End Using
    End Function
    
    ' Implementation for LocalMachine operations...
End Class
```

## Enum Helper Pattern

### Type-Safe Enum Operations

The library uses a sophisticated enum helper pattern for type-safe operations:

```vb
' Generic enum helper pattern
Public NotInheritable Class [Enum](Of T As Structure)
    Private Shared ReadOnly _cache As New Concurrent.ConcurrentDictionary(Of String, Object)
    
    ' Get all enum values and names
    Public Shared ReadOnly Property GetValuesAndNames As IReadOnlyDictionary(Of String, Object)
        Get
            Return GetOrCreateCache("ValuesAndNames", Function() 
                Dim result As New Dictionary(Of String, Object)
                For Each value In [Enum].GetValues(GetType(T))
                    result.Add([Enum].GetName(GetType(T), value), value)
                Next
                Return result
            End Function)
        End Get
    End Property
    
    ' Try to get enum value by name
    Public Shared Function TryGetValueOfName(name As String, ByRef value As Object, Optional ignoreCase As Boolean = False) As Boolean
        Try
            value = [Enum].Parse(GetType(T), name, ignoreCase)
            Return True
        Catch
            value = Nothing
            Return False
        End Try
    End Function
    
    ' Get current values and names for a specific enum value
    Public Shared Function GetCurrentValuesAndNames(enumValue As T) As IReadOnlyDictionary(Of String, Object)
        Dim result As New Dictionary(Of String, Object)
        Dim enumType = GetType(T)
        
        If enumType.GetCustomAttributes(GetType(FlagsAttribute), False).Length > 0 Then
            ' Handle flags enum
            For Each value In [Enum].GetValues(enumType)
                If CInt(enumValue) And CInt(value) = CInt(value) AndAlso CInt(value) <> 0 Then
                    result.Add([Enum].GetName(enumType, value), value)
                End If
            Next
        Else
            ' Handle regular enum
            result.Add([Enum].GetName(enumType, enumValue), enumValue)
        End If
        
        Return result
    End Function
    
    ' Caching helper
    Private Shared Function GetOrCreateCache(Of TResult)(key As String, factory As Func(Of TResult)) As TResult
        Return CType(_cache.GetOrAdd(key, Function(k) factory()), TResult)
    End Function
End Class
```

## Error Code Pattern

### Hierarchical Error Code Management

The library implements a hierarchical error code pattern:

```vb
' Hierarchical error code enumeration
Public Enum LicensingErrorCodes As Integer
    ' Success codes
    Undefined = 0
    Success = 1
    
    ' General errors (100-199)
    GeneralError = 100
    InvalidParameter = 101
    OperationFailed = 102
    
    ' Initialization errors (200-299)
    InitializationFailed = 200
    WrapperNotFound = 201
    RegistryAccessDenied = 202
    
    ' Authentication errors (300-399)
    AuthenticationFailed = 300
    FeatureNotFound = 301
    FeatureExpired = 302
    ServerUnavailable = 303
    
    ' Borrow errors (400-499)
    BorrowFailed = 400
    BorrowNotSupported = 401
    BorrowExpired = 402
    
    ' Status errors (500-599)
    StatusCheckFailed = 500
    ServerNotResponding = 501
End Enum

' Error code result with additional context
Public Class LicenseErrorCodeResult
    Inherits Result
    
    Public Property ErrorCode As LicensingErrorCodes
    Public Property DiagnosticsErrorCode As Integer
    
    Public Sub New(result As ResultState, Optional errorCode As LicensingErrorCodes = LicensingErrorCodes.Undefined)
        MyBase.New(result)
        Me.ErrorCode = errorCode
    End Sub
    
    ' Factory methods for specific error categories
    Public Shared Function CreateInitializationError(specificCode As LicensingErrorCodes, message As String) As LicenseErrorCodeResult
        Return New LicenseErrorCodeResult(ResultState.Failed, specificCode) With {.Message = message}
    End Function
    
    Public Shared Function CreateAuthenticationError(specificCode As LicensingErrorCodes, message As String) As LicenseErrorCodeResult
        Return New LicenseErrorCodeResult(ResultState.Failed, specificCode) With {.Message = message}
    End Function
End Class
```

## Command Pattern for Operations

### Licensing Operation Commands

Complex licensing operations are encapsulated as commands:

```vb
' Command pattern for licensing operations
Public Interface ILicensingCommand
    Function Execute() As Result
    Function CanExecute() As Boolean
    Property Context As LicensingOperationContext
End Interface

Public Class AuthenticateFeatureCommand
    Implements ILicensingCommand
    
    Private _featureName As String
    Private _licenseManager As LicenseManagerBase
    
    Public Property Context As LicensingOperationContext Implements ILicensingCommand.Context
    
    Public Sub New(licenseManager As LicenseManagerBase, featureName As String)
        _licenseManager = licenseManager
        _featureName = featureName
    End Sub
    
    Public Function CanExecute() As Boolean Implements ILicensingCommand.CanExecute
        Return Not String.IsNullOrEmpty(_featureName) AndAlso _licenseManager IsNot Nothing
    End Function
    
    Public Function Execute() As Result Implements ILicensingCommand.Execute
        If Not CanExecute() Then
            Return New LicenseErrorCodeResult(ResultState.Failed, LicensingErrorCodes.InvalidParameter)
        End If
        
        Try
            Return _licenseManager.AuthenticateFeatures(New String() {_featureName})
        Catch ex As Exception
            Return New LicenseErrorCodeResult(ex, LicensingErrorCodes.AuthenticationFailed)
        End Try
    End Function
End Class

Public Class BorrowFeatureCommand
    Implements ILicensingCommand
    
    Private _featureName As String
    Private _duration As TimeSpan
    Private _licenseManager As LicenseManagerBase
    
    Public Property Context As LicensingOperationContext Implements ILicensingCommand.Context
    
    Public Function Execute() As Result Implements ILicensingCommand.Execute
        ' Implementation for borrowing features
        Return _licenseManager.BorrowFeature(_featureName, _duration)
    End Function
    
    Public Function CanExecute() As Boolean Implements ILicensingCommand.CanExecute
        Return Not String.IsNullOrEmpty(_featureName) AndAlso 
               _duration > TimeSpan.Zero AndAlso               _licenseManager IsNot Nothing
    End Function
End Class
```

---

**Previous:** [13 - Sequence Diagrams](./Documentation-13-Sequence-Diagrams.md) | **Next:** [15 - Technical Reference](./Documentation-15-Technical-Reference.md)
