# Technical Reference

This section provides a comprehensive technical reference for the E3.Lib.Licensing library, including complete API documentation, configuration options, best practices, and troubleshooting guides.

## API Reference

### Core Classes

#### LicenseManagerBase

The abstract base class that provides core licensing functionality for all E3 applications.

```vb
Public MustInherit Class LicenseManagerBase
    Implements IDisposable
    
    ' Constructor
    Public Sub New()
    
    ' Properties
    Public ReadOnly Property IsInitialized As Boolean
    Public ReadOnly Property AuthenticatedFeatures As ReadOnlyCollection(Of String)
    Protected Friend MustOverride ReadOnly Property MainFeature As String
    Public MustOverride ReadOnly Property ProductName As String
    Protected Overridable ReadOnly Property Vendor As String
    Protected Overridable ReadOnly Property IgnoreFeatures As String()
    
    ' Core Methods
    Public Function Init(Optional createRegKeys As Boolean = False) As Result
    Public Function InitAndAuthenticateFeatures(Optional features As String() = Nothing, Optional createRegKeys As Boolean = False) As Result
    Public Function InitAndAuthenticateAvailableFeatures(Optional createRegKeys As Boolean = False) As Result
    Public Function AuthenticateFeatures(ParamArray features As String()) As AuthFeaturesResult
    Public Function AuthenticateAvailableFeatures() As AuthFeaturesResult
    
    ' Status and Information Methods
    Public Function GetStatus(ParamArray features As String()) As StatusResult
    Public Function GetApplicationFeatureInfos() As ApplicationFeatureInfosResult
    Public Function IsAuthenticated(feature As String) As Boolean
    
    ' Borrow Management Methods
    Public Function BorrowFeature(featureName As String, hours As Integer) As BorrowResult
    Public Function ReturnBorrowedFeatures(ParamArray features As String()) As BorrowResult
    Public Function GetBorrowedFeatures() As BorrowResult
    
    ' Utility Methods
    Public Function RunLmStat(ParamArray parameters As String()) As LmStatResult
    Public Function RunLmBorrow(ParamArray parameters As String()) As ProcessResult
    
    ' Abstract Methods (must be implemented by derived classes)
    Protected MustOverride Function TryGetFeatureStringAsValue(featureName As String, ByRef value As Object) As Boolean
    Protected MustOverride Function GetFeaturesAsString(result As FeaturesResult) As String()
    Protected Friend MustOverride Function GetAllFeatureNames() As String()
    Protected Friend MustOverride Function GetAllFeatureValues() As Object()
    
    ' Virtual Methods (can be overridden)
    Protected Overridable Sub InitAvailableFeatures(Optional createRegKeys As Boolean = False, Optional availableFeaturesKeySetting As AvailableFeatureKeySettings = Nothing)
    
    ' Resource Management
    Public Sub Dispose() Implements IDisposable.Dispose
    Protected Overridable Sub Dispose(disposing As Boolean)
End Class
```

#### LicenseManager(Of TFeatures)

Generic type-safe license manager for strongly-typed feature management.

```vb
Public MustInherit Class LicenseManager(Of TFeatures As Structure)
    Inherits LicenseManagerBase
    
    ' Type-safe feature authentication
    Public Overloads Function AuthenticateFeatures(ParamArray features As TFeatures()) As AuthFeaturesResult
    Public Function AuthenticateAllFeatures() As AuthFeaturesResult
    
    ' Override abstract methods with type-safe implementations
    Protected Overrides Function TryGetFeatureStringAsValue(featureName As String, ByRef value As Object) As Boolean
    Protected Overrides Function GetFeaturesAsString(result As FeaturesResult) As String()
    Protected Friend Overrides Function GetAllFeatureNames() As String()
    Protected Friend Overrides Function GetAllFeatureValues() As Object()
    
    ' Type-safe helper methods
    Protected Function GetAllFeaturesAsFlag() As Integer
    Protected Friend Overrides ReadOnly Property MainFeature As String
End Class
```

### Result Classes Hierarchy

#### Result (Base Class)

The foundational result class providing basic success/failure information.

```vb
Public Class Result
    ' Constructors
    Public Sub New(result As ResultState)
    Public Sub New(result As ResultState, message As String)
    Public Sub New(exception As Exception)
    Protected Sub New()
    Protected Friend Sub New(other As IResult)
    
    ' Properties
    Public Property IsSuccess As Boolean
    Public Property Message As String
    Public Property ResultState As ResultState
    Public Property Exception As Exception
    
    ' Static factory methods
    Public Shared Function Success() As Result
    Public Shared Function Failed(message As String) As Result
    Public Shared Function FromException(ex As Exception) As Result
End Class
```

#### LicenseErrorCodeResult

Extends Result with licensing-specific error codes and diagnostics.

```vb
Public Class LicenseErrorCodeResult
    Inherits Result
    Implements ILicenseErrorCodeResult
    
    ' Constructors
    Public Sub New(result As ResultState, Optional errorCode As LicensingErrorCodes = LicensingErrorCodes.Undefined)
    Public Sub New(exception As Exception, Optional errorCode As LicensingErrorCodes = LicensingErrorCodes.Undefined)
    Public Sub New(result As ResultState, message As String, Optional errorCode As LicensingErrorCodes = LicensingErrorCodes.Undefined)
    Protected Sub New()
    Protected Friend Sub New(other As IResult)
    
    ' Properties
    Public Property ErrorCode As LicensingErrorCodes
    Public Property DiagnosticsErrorCode As Integer
End Class
```

#### FeaturesResult (AuthFeaturesResult)

Result class for feature authentication operations.

```vb
Public Class FeaturesResult
    Inherits LicenseErrorCodeResult
    
    ' Constructors
    Public Sub New(result As ResultState, ParamArray authFeatures As String())
    Public Sub New(result As ResultState, authFeatures As String(), Optional errorCode As LicensingErrorCodes = LicensingErrorCodes.Undefined)
    Public Sub New(result As ResultState, message As String, ParamArray authFeatures As String())
    Public Sub New(result As ResultState, message As String, authFeatures As String(), Optional errorCode As LicensingErrorCodes = LicensingErrorCodes.Undefined)
    Public Sub New(exception As Exception, Optional errorCode As LicensingErrorCodes = LicensingErrorCodes.Undefined)
    
    ' Properties
    Public Property Features As Object
    Public Property AuthenticatedFeatures As String()
    Public Property FailedFeatures As String()
    
    ' Methods
    Public Function HasAuthenticatedFeatures() As Boolean
    Public Function GetAuthenticatedFeatureCount() As Integer
End Class

' Type alias for backward compatibility
Public Class AuthFeaturesResult
    Inherits FeaturesResult
End Class
```

#### StatusResult

Result class for license status information.

```vb
Public Class StatusResult
    Inherits LicenseErrorCodeResult
    
    ' Constructors
    Public Sub New(result As ResultState, Optional errorCode As LicensingErrorCodes = LicensingErrorCodes.Undefined)
    Public Sub New(result As ResultState, message As String, Optional errorCode As LicensingErrorCodes = LicensingErrorCodes.Undefined)
    Public Sub New(exception As Exception, Optional errorCode As LicensingErrorCodes = LicensingErrorCodes.Undefined)
    
    ' Properties
    Public Property HasExpirationDate As Boolean
    Public Property ExpirationDate As DateTime
    Public Property HasVendor As Boolean
    Public Property Vendor As String
    Public Property ServerVersion As String
    Public Property LicenseType As String
    Public Property UsersCount As Integer
    Public Property MaxUsers As Integer
End Class
```

#### BorrowResult

Result class for borrow/return operations.

```vb
Public Class BorrowResult
    Inherits LicenseErrorCodeResult
    
    ' Constructors
    Public Sub New(result As ResultState, ParamArray authFeatures As String())
    Public Sub New(result As ResultState, authFeatures As String(), Optional errorCode As LicensingErrorCodes = LicensingErrorCodes.Undefined)
    Public Sub New(result As ResultState, message As String, ParamArray authFeatures As String())
    Public Sub New(result As ResultState, message As String, authFeatures As String(), Optional errorCode As LicensingErrorCodes = LicensingErrorCodes.Undefined)
    Public Sub New(exception As Exception, Optional errorCode As LicensingErrorCodes = LicensingErrorCodes.Undefined)
    
    ' Properties
    Public Property BorrowEndDate As DateTime
    Public Property FeatureName As String
    Public Property AuthenticatedFeatures As String()
    Public Property BorrowedFeatures As String()
    Public Property RemainingHours As Integer
End Class
```

#### ApplicationFeatureInfosResult

Result class for feature information queries.

```vb
Public Class ApplicationFeatureInfosResult
    Inherits LicenseErrorCodeResult
    
    ' Constructors
    Public Sub New(result As ResultState, ParamArray applicationFeatureInfos As ApplicationFeatureInfo())
    Public Sub New(result As ResultState, applicationFeatureInfos As ApplicationFeatureInfo(), Optional errorCode As LicensingErrorCodes = LicensingErrorCodes.Undefined)
    Public Sub New(exception As Exception, Optional errorCode As LicensingErrorCodes = LicensingErrorCodes.Undefined)
    
    ' Properties
    Public Property ApplicationFeatureInfos As ApplicationFeatureInfo()
    
    ' Methods
    Public Function FindFeature(featureName As String) As ApplicationFeatureInfo
    Public Function GetFeatureNames() As String()
End Class
```

#### ProcessResult

Result class for external process execution (lmstat, lmborrow).

```vb
Public Class ProcessResult
    Inherits LicenseErrorCodeResult
    
    ' Constructors
    Public Sub New(result As ResultState, Optional errorCode As LicensingErrorCodes = LicensingErrorCodes.Undefined)
    Public Sub New(result As ResultState, message As String, Optional errorCode As LicensingErrorCodes = LicensingErrorCodes.Undefined)
    Public Sub New(exception As Exception, Optional errorCode As LicensingErrorCodes = LicensingErrorCodes.Undefined)
    
    ' Properties
    Public Property ExitCode As Integer
    Public Property StandardOutput As String
    Public Property StandardError As String
    Public Property ExecutionTime As TimeSpan
    Public Property ProcessId As Integer
End Class

' Specialized process result for lmstat
Public Class LmStatResult
    Inherits ProcessResult
    
    ' Additional methods for parsing lmstat output
    Public Function ParseFeatureInfo() As ApplicationFeatureInfo()
    Public Function ParseServerStatus() As String
    Public Function ParseVendorDaemonStatus() As String
End Class
```

### Supporting Classes

#### ApplicationFeatureInfo

Information about a licensed feature.

```vb
Public Class ApplicationFeatureInfo
    ' Constructor
    Public Sub New(name As String, version As String, licenses As String, vendor As String, expires As String)
    
    ' Properties
    Public Property Name As String
    Public Property Version As String
    Public Property Licenses As String
    Public Property Vendor As String
    Public Property Expires As String
    
    ' Methods
    Public Function IsExpired() As Boolean
    Public Function GetExpirationDate() As DateTime?
    Public Function GetLicenseCount() As Integer
    Public Overrides Function ToString() As String
End Class
```

#### AvailableFeatureKeySettings

Configuration for available features registry settings.

```vb
Public Class AvailableFeatureKeySettings
    ' Constructor
    Public Sub New(availableFeaturesValue As Object, defaultAvailableFeaturesValue As Object)
    
    ' Properties
    Public Property AvailableFeaturesValue As Object
    Public Property DefaultAvailableFeaturesValue As Object
End Class
```

### Enumerations

#### LicensingErrorCodes

Comprehensive error codes for licensing operations.

```vb
Public Enum LicensingErrorCodes As Integer
    ' Success and undefined
    Undefined = 0
    
    ' General errors (1-99)
    NO_FEATURES_BORROWED = 1
    BORROW_FEATURES_FAILED = 2
    AUTH_FAILED = 3
    SERVER_REMOVE_FEATURE_FAILED = 4
    SERVER_NOT_ACCESSIBLE = 5
    DATE_EXPIRED = 6
    VENDOR_DEAMON_DOWN = 7
    NOT_INITIALIZED = 8
    
    ' Extended error codes for comprehensive error handling
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
```

#### ResultState

States for result objects.

```vb
Public Enum ResultState As Integer
    Success = 0
    Failed = 1
    Warning = 2
    Information = 3
End Enum
```

## Configuration and Constants

### Environment Variables

The library recognizes and uses the following environment variables:

```vb
' Primary license file environment variables (in order of precedence)
Public Const ZUKEN_LICENSE_FILE As String = "ZUKEN_LICENSE_FILE"
Public Const ECADLM_LICENSE_FILE As String = "ECADLM_LICENSE_FILE"
Public Const LM_LICENSE_FILE As String = "LM_LICENSE_FILE"

' Company and vendor information
Public Const COMPANY_NAME As String = "Zuken"

' External tool executables
Public Const LMBORROWEXE As String = "lmborrow.exe"
Public Const LMSTATEXE As String = "lmstat.exe"

' Registry keys
Public Const REG_AVAIL_FEATURES_KEY As String = "AvailableFeatures"
```

### Registry Configuration

The library uses Windows Registry for persistent configuration:

#### Registry Paths
- **Base Path**: `HKEY_CURRENT_USER\Software\{Company}\{ProductName}`
- **Available Features**: `{BasePath}\AvailableFeatures`
- **Feature Settings**: `{BasePath}\Features\{FeatureName}`

#### Registry Values
```vb
' Available features configuration
REG_AVAIL_FEATURES_KEY = "AvailableFeatures"  ' DWORD value containing flags

' Individual feature settings
"LastAuthenticated" = DateTime    ' When feature was last authenticated
"AuthenticationCount" = Integer   ' Number of successful authentications
"LastError" = String             ' Last error message for this feature
```

## Implementation Guidelines

### Creating a Custom License Manager

To create a custom license manager for your application:

1. **Define Feature Enumeration**:
```vb
<Flags>
Public Enum MyApplicationFeature As Integer
    None = 0
    BasicFeature = 1
    AdvancedFeature = 2
    PremiumFeature = 4
    ExpertFeature = 8
End Enum
```

2. **Implement License Manager**:
```vb
Public Class MyApplicationLicenseManager
    Inherits LicenseManager(Of MyApplicationFeature)
    
    ' Required property implementations
    Public Overrides ReadOnly Property ProductName As String
        Get
            Return "MyApplication"
        End Get
    End Property
    
    Protected Overrides ReadOnly Property Vendor As String
        Get
            Return "MyCompany"
        End Get
    End Property
    
    ' Optional: Customize available features initialization
    Protected Overrides Sub InitAvailableFeatures(Optional createRegKeys As Boolean = False, Optional availableFeaturesKeySetting As AvailableFeatureKeySettings = Nothing)
        If availableFeaturesKeySetting Is Nothing Then
            ' Use all features by default
            availableFeaturesKeySetting = New AvailableFeatureKeySettings(GetAllFeaturesAsFlag, GetAllFeaturesAsFlag)
        End If
        MyBase.InitAvailableFeatures(createRegKeys, availableFeaturesKeySetting)
    End Sub
    
    ' Optional: Specify features to ignore
    Protected Overrides ReadOnly Property IgnoreFeatures As String()
        Get
            Return New String() {"None"}
        End Get
    End Property
End Class
```

3. **Usage Example**:
```vb
' Initialize and use the license manager
Using licenseManager As New MyApplicationLicenseManager()
    ' Initialize licensing system
    Dim initResult As Result = licenseManager.Init(createRegKeys:=True)
    If Not initResult.IsSuccess Then
        ' Handle initialization failure
        Console.WriteLine($"Licensing initialization failed: {initResult.Message}")
        Return
    End If
    
    ' Authenticate specific features
    Dim authResult As AuthFeaturesResult = licenseManager.AuthenticateFeatures(
        MyApplicationFeature.BasicFeature, 
        MyApplicationFeature.AdvancedFeature)
    
    If authResult.IsSuccess Then
        Console.WriteLine($"Authenticated features: {String.Join(", ", authResult.AuthenticatedFeatures)}")
        
        ' Check if specific feature is available
        If licenseManager.IsAuthenticated("BasicFeature") Then
            ' Use basic functionality
        End If
        
        If licenseManager.IsAuthenticated("AdvancedFeature") Then
            ' Use advanced functionality
        End If
    Else
        Console.WriteLine($"Authentication failed: {authResult.Message} (Error: {authResult.ErrorCode})")
    End If
End Using
```

### Best Practices

#### 1. Resource Management
Always use `Using` statements or proper disposal:

```vb
' Correct resource management
Using licenseManager As New MyLicenseManager()
    ' Use license manager
End Using

' Or manual disposal
Dim licenseManager As New MyLicenseManager()
Try
    ' Use license manager
Finally
    licenseManager?.Dispose()
End Try
```

#### 2. Error Handling
Check results and handle errors appropriately:

```vb
Dim result As AuthFeaturesResult = licenseManager.AuthenticateFeatures(features)

If result.IsSuccess Then
    ' Success case
    ProcessAuthenticatedFeatures(result.AuthenticatedFeatures)
Else
    ' Handle specific error codes
    Select Case result.ErrorCode
        Case LicensingErrorCodes.SERVER_NOT_ACCESSIBLE
            ShowOfflineMode()
        Case LicensingErrorCodes.DATE_EXPIRED
            ShowLicenseExpiredDialog()
        Case LicensingErrorCodes.AUTH_FAILED
            ShowLicenseInvalidDialog()
        Case Else
            ShowGenericErrorDialog(result.Message)
    End Select
End If
```

#### 3. Feature Checking
Use type-safe feature checking:

```vb
' Type-safe feature checking
If licenseManager.IsAuthenticated(MyApplicationFeature.AdvancedFeature.ToString()) Then
    EnableAdvancedFeatures()
End If

' Or using string constants
Private Const ADVANCED_FEATURE As String = "AdvancedFeature"
If licenseManager.IsAuthenticated(ADVANCED_FEATURE) Then
    EnableAdvancedFeatures()
End If
```

#### 4. Initialization Patterns
Initialize once at application startup:

```vb
Public Class ApplicationStartup
    Private Shared _licenseManager As MyLicenseManager
    
    Public Shared Function InitializeLicensing() As Boolean
        Try
            _licenseManager = New MyLicenseManager()
            Dim result As Result = _licenseManager.InitAndAuthenticateAvailableFeatures(createRegKeys:=True)
            Return result.IsSuccess
        Catch ex As Exception
            ' Log error
            Return False
        End Try
    End Function
    
    Public Shared ReadOnly Property LicenseManager As MyLicenseManager
        Get
            Return _licenseManager
        End Get
    End Property
    
    Public Shared Sub Cleanup()
        _licenseManager?.Dispose()
    End Sub
End Class
```

## Common Operations

### Feature Authentication

```vb
' Authenticate specific features
Dim result = licenseManager.AuthenticateFeatures("Feature1", "Feature2")

' Authenticate using enum (type-safe)
Dim result = licenseManager.AuthenticateFeatures(MyFeature.Feature1, MyFeature.Feature2)

' Authenticate all available features
Dim result = licenseManager.AuthenticateAvailableFeatures()

' Authenticate all possible features
Dim result = licenseManager.AuthenticateAllFeatures()
```

### Status Checking

```vb
' Get status for specific features
Dim statusResult = licenseManager.GetStatus("Feature1", "Feature2")
If statusResult.IsSuccess Then
    Console.WriteLine($"Vendor: {statusResult.Vendor}")
    Console.WriteLine($"Expires: {statusResult.ExpirationDate}")
    Console.WriteLine($"Users: {statusResult.UsersCount}/{statusResult.MaxUsers}")
End If

' Check individual feature authentication
If licenseManager.IsAuthenticated("Feature1") Then
    ' Feature is available
End If
```

### Borrow Operations

```vb
' Borrow a feature for offline use
Dim borrowResult = licenseManager.BorrowFeature("Feature1", hours:=24)
If borrowResult.IsSuccess Then
    Console.WriteLine($"Feature borrowed until: {borrowResult.BorrowEndDate}")
End If

' Get currently borrowed features
Dim borrowedResult = licenseManager.GetBorrowedFeatures()
If borrowedResult.IsSuccess Then
    Console.WriteLine($"Borrowed features: {String.Join(", ", borrowedResult.BorrowedFeatures)}")
End If

' Return borrowed features
Dim returnResult = licenseManager.ReturnBorrowedFeatures("Feature1")
```

### Feature Information

```vb
' Get detailed feature information
Dim infoResult = licenseManager.GetApplicationFeatureInfos()
If infoResult.IsSuccess Then
    For Each feature In infoResult.ApplicationFeatureInfos
        Console.WriteLine($"Feature: {feature.Name}")
        Console.WriteLine($"Version: {feature.Version}")
        Console.WriteLine($"Licenses: {feature.Licenses}")
        Console.WriteLine($"Vendor: {feature.Vendor}")
        Console.WriteLine($"Expires: {feature.Expires}")
        Console.WriteLine()
    Next
End If
```

### License Server Utilities

```vb
' Run lmstat to get server information
Dim lmstatResult = licenseManager.RunLmStat("-a")
If lmstatResult.IsSuccess Then
    Console.WriteLine("Server Output:")
    Console.WriteLine(lmstatResult.StandardOutput)
End If

' Run lmborrow with custom parameters
Dim borrowProcessResult = licenseManager.RunLmBorrow("-status")
If borrowProcessResult.IsSuccess Then
    Console.WriteLine("Borrow Status:")
    Console.WriteLine(borrowProcessResult.StandardOutput)
End If
```

## Troubleshooting Guide

### Common Issues and Solutions

#### 1. "NOT_INITIALIZED" Error

**Problem**: License manager reports NOT_INITIALIZED error code.

**Causes**:
- `Init()` method was not called
- Initialization failed silently
- License wrapper initialization failed

**Solutions**:
```vb
' Always check initialization result
Dim initResult = licenseManager.Init(createRegKeys:=True)
If Not initResult.IsSuccess Then
    Console.WriteLine($"Initialization failed: {initResult.Message}")
    If TypeOf initResult Is LicenseErrorCodeResult Then
        Dim errorResult = CType(initResult, LicenseErrorCodeResult)
        Console.WriteLine($"Error code: {errorResult.ErrorCode}")
    End If
End If
```

#### 2. "SERVER_NOT_ACCESSIBLE" Error

**Problem**: Cannot connect to license server.

**Causes**:
- License server is down
- Network connectivity issues
- Incorrect license file path
- Firewall blocking connection

**Solutions**:
```vb
' Check environment variables
Dim licenseFile = Environment.GetEnvironmentVariable("ZUKEN_LICENSE_FILE")
If String.IsNullOrEmpty(licenseFile) Then
    licenseFile = Environment.GetEnvironmentVariable("LM_LICENSE_FILE")
End If
Console.WriteLine($"License file: {licenseFile}")

' Test server connectivity
Dim lmstatResult = licenseManager.RunLmStat("-c")
Console.WriteLine(lmstatResult.StandardOutput)
```

#### 3. "AUTH_FAILED" Error

**Problem**: Feature authentication fails.

**Causes**:
- Feature not available in license
- Feature name mismatch
- License expired
- No available licenses

**Solutions**:
```vb
' Get available feature information
Dim infoResult = licenseManager.GetApplicationFeatureInfos()
If infoResult.IsSuccess Then
    Dim availableFeatures = infoResult.ApplicationFeatureInfos.Select(Function(f) f.Name).ToArray()
    Console.WriteLine($"Available features: {String.Join(", ", availableFeatures)}")
End If

' Check specific feature status
Dim statusResult = licenseManager.GetStatus("FeatureName")
If statusResult.IsSuccess Then
    Console.WriteLine($"Feature expires: {statusResult.ExpirationDate}")
    Console.WriteLine($"Available licenses: {statusResult.MaxUsers - statusResult.UsersCount}")
End If
```

#### 4. Registry Access Issues

**Problem**: Registry operations fail.

**Causes**:
- Insufficient permissions
- Registry key corruption
- Antivirus interference

**Solutions**:
```vb
' Try initialization without registry creation
Dim result = licenseManager.Init(createRegKeys:=False)

' Or run application with elevated permissions
' Check Windows Event Log for specific registry errors
```

#### 5. "BORROW_FEATURES_FAILED" Error

**Problem**: Cannot borrow features for offline use.

**Causes**:
- Feature doesn't support borrowing
- Maximum borrow time exceeded
- Already borrowed by another user

**Solutions**:
```vb
' Check current borrow status
Dim borrowStatus = licenseManager.GetBorrowedFeatures()
If borrowStatus.IsSuccess Then
    Console.WriteLine($"Currently borrowed: {String.Join(", ", borrowStatus.BorrowedFeatures)}")
End If

' Try borrowing for shorter duration
Dim borrowResult = licenseManager.BorrowFeature("FeatureName", hours:=8)
```

### Diagnostic Information

#### Enabling Diagnostics

```vb
' Enable detailed logging (if supported by your implementation)
Environment.SetEnvironmentVariable("FLEXLM_DIAGNOSTICS", "3")

' Run diagnostic lmstat commands
Dim diagnosticResult = licenseManager.RunLmStat("-a", "-i")
Console.WriteLine("Diagnostic Output:")
Console.WriteLine(diagnosticResult.StandardOutput)
```

#### Registry Inspection

```vb
' Check registry values manually
Dim keyPath = $"Software\\{licenseManager.Vendor}\\{licenseManager.ProductName}"
Using key = Registry.CurrentUser.OpenSubKey(keyPath)
    If key IsNot Nothing Then
        For Each valueName In key.GetValueNames()
            Console.WriteLine($"{valueName}: {key.GetValue(valueName)}")
        Next
    Else
        Console.WriteLine("Registry key not found")
    End If
End Using
```

### Performance Optimization

#### 1. Minimize Initialization Calls
```vb
' Initialize once at application startup
Public Module LicensingModule
    Private _licenseManager As MyLicenseManager
    Private _initialized As Boolean = False
    
    Public Function GetLicenseManager() As MyLicenseManager
        If Not _initialized Then
            _licenseManager = New MyLicenseManager()
            _licenseManager.Init(createRegKeys:=True)
            _initialized = True
        End If
        Return _licenseManager
    End Function
End Module
```

#### 2. Cache Authentication Results
```vb
' Cache feature authentication status
Private Shared _featureCache As New Dictionary(Of String, Boolean)

Public Function IsFeatureAvailable(featureName As String) As Boolean
    If Not _featureCache.ContainsKey(featureName) Then
        _featureCache(featureName) = licenseManager.IsAuthenticated(featureName)
    End If
    Return _featureCache(featureName)
End Function
```

#### 3. Batch Operations
```vb
' Authenticate multiple features at once instead of individually
Dim result = licenseManager.AuthenticateFeatures("Feature1", "Feature2", "Feature3")

' Instead of:
' licenseManager.AuthenticateFeatures("Feature1")
' licenseManager.AuthenticateFeatures("Feature2")
' licenseManager.AuthenticateFeatures("Feature3")
```

## Migration and Compatibility

### Migrating from Previous Versions

When migrating from older licensing systems:

1. **Update Feature Enumerations**: Ensure your feature enums use `<Flags>` attribute for bitwise operations
2. **Update Error Handling**: Use the new `LicenseErrorCodeResult` hierarchy
3. **Registry Migration**: Clean up old registry keys if necessary
4. **Update Disposal Patterns**: Use proper `IDisposable` implementation

### Framework Compatibility

The library supports multiple .NET frameworks:
- **.NET Framework 4.7.2+**: Full functionality
- **.NET 6**: Full functionality with modern APIs
- **.NET 8**: Latest features and performance optimizations

### Thread Safety

The license manager is **not thread-safe**. For multi-threaded applications:

```vb
' Use synchronization for shared access
Private Shared _licenseLock As New Object()

Public Function CheckFeature(featureName As String) As Boolean
    SyncLock _licenseLock
        Return licenseManager.IsAuthenticated(featureName)
    End SyncLock
End Function
```

---

**Previous:** [14 - Implementation Patterns](./Documentation-14-Implementation-Patterns.md) | **Return to:** [Documentation Index](./Documentation.md)
