# E3.Lib.Licensing Quick Reference

This quick reference provides essential information for developers working with the E3.Lib.Licensing library. It covers the most commonly used classes, methods, patterns, and best practices.

## Core Classes

### LicenseManagerBase (Abstract)
```vb
Option Strict On
Option Explicit On
Option Infer Off

Public MustInherit Class LicenseManagerBase
    Implements IDisposable
    
    ' Core Methods
    Public Function Init(Optional createRegKeys As Boolean = False) As Result
    Public Function AuthenticateFeatures(features As String()) As FeaturesResult
    Public Sub TerminateLicense()
    
    ' Properties
    Public ReadOnly Property Status As StatusProvider
    Public ReadOnly Property Borrow As BorrowManager
    Protected Friend MustOverride ReadOnly Property MainFeature As String
    Protected Friend MustOverride ReadOnly Property ProductName As String
End Class
```

### LicenseManager<TFeatures> (Generic)
```vb
Option Strict On
Option Explicit On
Option Infer Off

Public MustInherit Class LicenseManager(Of TFeatures As Structure)
    Inherits LicenseManagerBase
    
    ' Type-Safe Methods
    Public Overloads Function AuthenticateFeatures(ParamArray features As TFeatures()) As AuthFeaturesResult
    Public Function GetAvailableFeatures() As TFeatures()
    Public Function GetAvailableFeaturesAsFlag() As Integer
End Class
```

### Key Result Classes
```vb
Option Strict On
Option Explicit On
Option Infer Off

' Base result class
Public Class Result
    Public ReadOnly Property State As ResultState
    Public ReadOnly Property Message As String
    Public ReadOnly Property IsSuccess As Boolean
End Class

' Feature authentication results
Public Class FeaturesResult : Inherits LicenseErrorCodeResult
Public Class AuthFeaturesResult : Inherits FeaturesResult
Public Class StatusResult : Inherits LicenseErrorCodeResult
Public Class BorrowResult : Inherits LicenseErrorCodeResult
```

## Basic Usage Patterns

### 1. Define Feature Enumeration
```vb
Option Strict On
Option Explicit On
Option Infer Off

<Flags>
Public Enum MyAppFeatures As Integer
    None = 0
    Base = 1
    Advanced = 2
    Export = 4
    Import = 8
    Reporting = 16
    All = Base Or Advanced Or Export Or Import Or Reporting
End Enum
```

### 2. Create License Manager
```vb
Option Strict On
Option Explicit On
Option Infer Off

Public Class MyAppLicenseManager
    Inherits LicenseManager(Of MyAppFeatures)
    
    Protected Friend Overrides ReadOnly Property MainFeature As String
        Get
            Return MyAppFeatures.Base.ToString()
        End Get
    End Property
    
    Protected Friend Overrides ReadOnly Property ProductName As String
        Get
            Return "MyApplication"
        End Get
    End Property
End Class
```

### 3. Initialize and Authenticate
```vb
Option Strict On
Option Explicit On
Option Infer Off

' Basic initialization
Dim licenseManager As New MyAppLicenseManager()
Dim initResult As Result = licenseManager.Init(createRegKeys:=True)

If initResult.IsSuccess Then
    ' Authenticate specific features
    Dim authResult As AuthFeaturesResult = licenseManager.AuthenticateFeatures(MyAppFeatures.Base, MyAppFeatures.Advanced)
    If authResult.IsSuccess Then
        ' Application logic here
    End If
End If

' Always clean up
licenseManager.TerminateLicense()
licenseManager.Dispose()
```

## Feature Management

### Authentication Methods
```vb
Option Strict On
Option Explicit On
Option Infer Off

' Single feature
Dim result As AuthFeaturesResult = licenseManager.AuthenticateFeatures(MyAppFeatures.Advanced)

' Multiple features
Dim result2 As AuthFeaturesResult = licenseManager.AuthenticateFeatures(MyAppFeatures.Base, MyAppFeatures.Export)

' All features
Dim result3 As AuthFeaturesResult = licenseManager.AuthenticateAllFeatures()

' Using flags
Dim features As Integer = MyAppFeatures.Base Or MyAppFeatures.Advanced
Dim result4 As AuthFeaturesResult = licenseManager.AuthenticateFeatures(features)
```

### Feature Checking
```vb
Option Strict On
Option Explicit On
Option Infer Off

' Check authentication status
If authResult.AuthenticatedFeatures.Contains(MyAppFeatures.Advanced) Then
    ' Feature is available
End If

' Get available features
Dim availableFeatures() As MyAppFeatures = licenseManager.GetAvailableFeatures()
For Each feature In availableFeatures
    Console.WriteLine(String.Format("Available: {0}", feature))
Next
```

## Result System

### Result States
```vb
Option Strict On
Option Explicit On
Option Infer Off

Public Enum ResultState
    Success = 0
    Failed = 1
    Warning = 2
    Information = 3
    Cancelled = 4
End Enum
```

### Common Result Patterns
```vb
Option Strict On
Option Explicit On
Option Infer Off

' Check result status
If result.IsSuccess Then
    ' Success logic
ElseIf result.State = ResultState.Warning Then
    ' Warning handling
Else
    ' Error handling
    Console.WriteLine(String.Format("Error: {0}", result.Message))
    If TypeOf result Is LicenseErrorCodeResult Then
        Dim licResult As LicenseErrorCodeResult = DirectCast(result, LicenseErrorCodeResult)
        Console.WriteLine(String.Format("Error Code: {0}", licResult.LicensingErrorCode))
    End If
End If
```

## Status and Monitoring

### License Status Checking
```vb
Option Strict On
Option Explicit On
Option Infer Off

' Get status for specific feature
Dim statusResult As StatusResult = licenseManager.Status.GetStatus("AdvancedFeature")
If statusResult.IsSuccess Then
    Console.WriteLine(String.Format("Users: {0}/{1}", statusResult.UsersCount, statusResult.MaxUsers))
    Console.WriteLine(String.Format("Expires: {0}", statusResult.ExpirationDate))
End If

' Check expiration
Dim isExpiring As Boolean = licenseManager.Status.IsExpiring("AdvancedFeature", 30) ' 30 days
If isExpiring Then
    ' Show warning to user
End If
```

### Server Connectivity
```vb
Option Strict On
Option Explicit On
Option Infer Off

' Get available features from server
Dim serverResult As FeaturesResult = licenseManager.Status.GetAvailableFeaturesFromServer()
If serverResult.IsSuccess Then
    For Each featureInfo In serverResult.Features
        Console.WriteLine(String.Format("Feature: {0}, Version: {1}", featureInfo.Name, featureInfo.Version))
    Next
End If
```

## Borrow Management

### Borrowing Licenses
```vb
Option Strict On
Option Explicit On
Option Infer Off

' Borrow for specific duration
Dim borrowResult As BorrowResult = licenseManager.Borrow.BorrowLicense("AdvancedFeature", 7) ' 7 days
If borrowResult.IsSuccess Then
    Console.WriteLine(String.Format("License borrowed until: {0}", borrowResult.BorrowEndDate))
End If

' Borrow until specific date
Dim endDate As DateTime = DateTime.Now.AddDays(10)
Dim borrowResult2 As BorrowResult = licenseManager.Borrow.BorrowLicense("ExportFeature", endDate)
```

### Managing Borrowed Licenses
```vb
Option Strict On
Option Explicit On
Option Infer Off

' Check if feature is borrowed
If licenseManager.Borrow.IsBorrowed("AdvancedFeature") Then
    ' Get borrow information
    Dim borrowInfo As BorrowInfo = licenseManager.Borrow.GetBorrowInfo("AdvancedFeature")
    Console.WriteLine(String.Format("Borrowed until: {0}", borrowInfo.EndDate))
    Console.WriteLine(String.Format("Days remaining: {0}", borrowInfo.DaysRemaining))
End If

' Return borrowed license
Dim returnResult As BorrowResult = licenseManager.Borrow.ReturnBorrowedLicense("AdvancedFeature")

' Return all borrowed licenses
Dim returnAllResult As BorrowResult = licenseManager.Borrow.ReturnAllBorrowedLicenses()
```

## Error Handling

### Common Error Codes
```vb
Option Strict On
Option Explicit On
Option Infer Off

Public Enum LicensingErrorCodes
    SUCCESS = 0
    FEATURE_NOT_FOUND = 1
    NO_SERVER_RESPONSE = 2
    AUTHENTICATION_FAILED = 3
    BORROW_FEATURES_FAILED = 4
    INSUFFICIENT_FEATURES = 5
    INVALID_FEATURE_SPECIFICATION = 6
    SERVER_NOT_RESPONDING = 501
    ' ... more error codes
End Enum
```

### Error Handling Patterns
```vb
Option Strict On
Option Explicit On
Option Infer Off

Select Case result.LicensingErrorCode
    Case LicensingErrorCodes.SUCCESS
        ' Continue operation
    Case LicensingErrorCodes.FEATURE_NOT_FOUND
        ' Feature not available
        ShowUserMessage("Required feature not available")
    Case LicensingErrorCodes.NO_SERVER_RESPONSE
        ' Server connectivity issue
        ShowUserMessage("Cannot connect to license server")
    Case LicensingErrorCodes.INSUFFICIENT_FEATURES
        ' Not enough licenses
        ShowUserMessage("All licenses are in use")    Case Else
        ' General error
        ShowUserMessage(String.Format("Licensing error: {0}", result.Message))
End Select
```

## Best Practices

### License Initialization
- **Early Verification**: Initialize and verify licenses at application startup before accessing protected features
- **Graceful Failure**: Provide clear error messages when licensing fails
- **Feature Granularity**: Request only the specific features your application needs
- **Registry Keys**: Use `createRegKeys:=True` on first run or when features change

### Resource Management
- **Proper Cleanup**: Always call `TerminateLicense()` when your application exits
- **Dispose Pattern**: Implement proper disposal using `Using` statements or try/finally blocks
- **AppDomain Isolation**: The license manager isolates native licensing code for clean unloading
- **Error Handling**: Handle licensing errors gracefully to prevent application crashes

### License Borrowing
- **User Guidance**: Provide clear instructions for borrowing and returning licenses
- **Time Limits**: Set reasonable borrowing time limits (typically 1-30 days)
- **Status Tracking**: Maintain and display the status of borrowed licenses
- **Return Policy**: Implement automatic return when application closes normally

### License Monitoring
- **Status Checks**: Periodically check license status during long-running operations
- **UI Integration**: Update UI elements based on available licenses
- **Expiration Warnings**: Warn users about upcoming license expirations
- **Diagnostics**: Provide diagnostic information when licensing fails

### Performance Optimization
- **Cache Results**: Cache authentication results to avoid repeated server calls
- **Lazy Loading**: Initialize license manager only when needed
- **Background Checks**: Perform status checks in background threads
- **Registry Caching**: Use registry to cache feature availability

## Common Code Examples

### Complete Application Integration
```vb
Option Strict On
Option Explicit On
Option Infer Off

Public Class MyApplication
    Private _licenseManager As MyAppLicenseManager
    Private _requiredFeatures As MyAppFeatures() = {MyAppFeatures.Base, MyAppFeatures.Advanced}
    
    Public Function Initialize() As Boolean        Try
            _licenseManager = New MyAppLicenseManager()
            
            ' Initialize license system
            Dim initResult As Result = _licenseManager.Init(createRegKeys:=True)
            If Not initResult.IsSuccess Then
                ShowError(String.Format("License initialization failed: {0}", initResult.Message))
                Return False
            End If
            
            ' Authenticate required features
            Dim authResult As AuthFeaturesResult = _licenseManager.AuthenticateFeatures(_requiredFeatures)
            If Not authResult.IsSuccess Then
                ShowError(String.Format("Feature authentication failed: {0}", authResult.Message))
                Return False
            End If
            
            ' Check for missing features
            For Each feature In _requiredFeatures
                If Not authResult.AuthenticatedFeatures.Contains(feature) Then
                    ShowWarning(String.Format("Feature {0} not available", feature))
                End If
            Next
            
            Return True
            
        Catch ex As Exception
            ShowError(String.Format("Unexpected error: {0}", ex.Message))
            Return False
        End Try
    End Function
    
    Public Sub Shutdown()
        Try
            _licenseManager?.TerminateLicense()
            _licenseManager?.Dispose()        Catch ex As Exception
            ' Log but don't throw during shutdown
            Console.WriteLine(String.Format("Error during license cleanup: {0}", ex.Message))
        End Try
    End Sub
End Class
```

### Feature-Specific Functionality
```vb
Option Strict On
Option Explicit On
Option Infer Off

Public Function CanExportData() As Boolean
    Return (_licenseManager?.IsAuthenticated(MyAppFeatures.Export) = True)
End Function

Public Function ExportData() As Boolean
    If Not CanExportData() Then
        ShowError("Export feature not available")
        Return False
    End If
    
    ' Perform export operation
    Return True
End Function
```

### License Status Monitoring
```vb
Option Strict On
Option Explicit On
Option Infer Off

Public Sub MonitorLicenseStatus()    ' Check expiration for all authenticated features
    For Each feature In _authenticatedFeatures
        If _licenseManager.Status.IsExpiring(feature.ToString(), 7) Then
            ShowWarning(String.Format("Feature {0} expires within 7 days", feature))
        End If
    Next
    
    ' Check server connectivity
    Dim serverFeatures As FeaturesResult = _licenseManager.Status.GetAvailableFeaturesFromServer()
    If Not serverFeatures.IsSuccess Then
        ShowWarning("Cannot connect to license server")
    End If
End Sub
```

## Troubleshooting

### Common Issues and Solutions

| Issue | Possible Causes | Solutions |
|-------|----------------|-----------|
| `FEATURE_NOT_FOUND` | Feature not in license file | Check license file, contact admin |
| `NO_SERVER_RESPONSE` | Network/server issues | Check network connectivity, server status |
| `INSUFFICIENT_FEATURES` | All licenses in use | Wait or contact admin for more licenses |
| `AUTHENTICATION_FAILED` | Invalid credentials | Check product name, feature names |
| Registry errors | Permission issues | Run as administrator, check registry access |
| Initialization fails | Missing dependencies | Check FlexNet installation, wrapper DLLs |

### Diagnostic Information
```vb
Option Strict On
Option Explicit On
Option Infer Off

' Get detailed diagnostic information
Dim diagnostics As String = _licenseManager.GetDiagnosticInfo()
Console.WriteLine(diagnostics)

' Check registry keys
Dim registryResult As RegistryResult = _licenseManager.GetAvailableRegistryFeatures()
Console.WriteLine(String.Format("Registry keys found: {0}", registryResult.RegistryKeys))

' Server status
Dim serverStatus As ServerStatusResult = _licenseManager.Status.GetServerStatus()
If serverStatus.IsSuccess Then
    Console.WriteLine(String.Format("Server: {0}", serverStatus.ServerName))
    Console.WriteLine(String.Format("Version: {0}", serverStatus.Version))
End If
```

### Debug Mode
```vb
Option Strict On
Option Explicit On
Option Infer Off

' Enable debug logging (if available)
_licenseManager.EnableDebugLogging = True

' Check initialization details
Dim initResult As Result = _licenseManager.Init(createRegKeys:=True)
If Not initResult.IsSuccess AndAlso TypeOf initResult Is LicenseErrorCodeResult Then
    Dim detailedResult As LicenseErrorCodeResult = DirectCast(initResult, LicenseErrorCodeResult)
    Console.WriteLine(String.Format("Error Code: {0}", detailedResult.LicensingErrorCode))
    Console.WriteLine(String.Format("Diagnostic Code: {0}", detailedResult.DiagnosticsErrorCode))
End If
```

## Additional Resources

### Documentation Sections
- [Introduction](./Documentation-01-Introduction.md) - Overview and core concepts
- [Architecture Overview](./Documentation-02-Architecture-Overview.md) - System design principles
- [Generic LicenseManager](./Documentation-04-Generic-LicenseManager.md) - Type-safe feature management
- [Result System](./Documentation-05-Result-System.md) - Error handling and results
- [Integration Examples](./Documentation-12-Integration-Examples.md) - Real-world usage patterns
- [Technical Reference](./Documentation-15-Technical-Reference.md) - Complete API documentation

### External Resources
- [Microsoft Licensing Management Guide](https://www.finops.org/wg/microsoft-licensing-management-guide/) - Microsoft's guide for licensing in cloud environments
- [License Management in .NET](https://www.microsoft.com/licensing/docs/view/Licensing-Guides) - Microsoft's guidance on licensing
- [Zuken E3.Lib.Licensing.MFC](https://ulm-dev.zuken.com/Team-Erlangen/E3.Lib.Licensing.MFC/src/branch/trunk/README.md) - C++ wrapper documentation

### Framework Support
- **.NET Framework 4.7.2**: Full support with C++/CLI wrapper
- **.NET 6**: Native support with modern interop
- **.NET 8**: Latest features and performance optimizations

### Development Environment
- **IDE**: Visual Studio 2019+ recommended
- **Languages**: VB.NET, C# supported
- **Dependencies**: FlexNet Publisher SDK, E3.Lib.Licensing.MFC wrapper
- **Testing**: Unit test framework integration available

---

*For detailed implementation examples and advanced topics, refer to the complete documentation sections listed above.*