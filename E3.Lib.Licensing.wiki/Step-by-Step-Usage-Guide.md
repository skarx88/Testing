# Step-by-Step Usage Guide

This comprehensive guide provides step-by-step instructions for implementing E3.Lib.Licensing in your applications. It covers everything from basic setup to advanced licensing scenarios, with practical examples and best practices.

## Table of Contents

- [Prerequisites and Setup](#prerequisites-and-setup)
- [Step 1: Define Your Application Features](#step-1-define-your-application-features)
- [Step 2: Choose Your Implementation Approach](#step-2-choose-your-implementation-approach)
- [Step 3: Implement Your License Manager](#step-3-implement-your-license-manager)
- [Step 4: Integrate Licensing into Your Application](#step-4-integrate-licensing-into-your-application)
- [Step 5: Handle Errors and Edge Cases](#step-5-handle-errors-and-edge-cases)
- [Step 6: Implement Advanced Features](#step-6-implement-advanced-features)
- [Step 7: Testing and Validation](#step-7-testing-and-validation)
- [Best Practices and Tips](#best-practices-and-tips)
- [Troubleshooting Common Issues](#troubleshooting-common-issues)

## Prerequisites and Setup

### System Requirements

- **.NET Framework 4.7.2** or higher (.NET 6, .NET 8 supported)
- **FlexNet Publisher SDK** installed and configured
- **E3.Lib.Licensing** library references
- **Windows operating system** (for registry integration)

### Assembly References

Add these references to your project:

```xml
<PackageReference Include="E3.Lib.Licensing" Version="[latest]" />
<PackageReference Include="E3.Lib.Licensing.MFC" Version="[latest]" />
```

### Import Statements

```vb
Option Strict On
Option Explicit On
Option Infer Off

Imports System.ComponentModel
Imports System.Windows.Forms
Imports Zuken.E3.Lib.Licensing
```

## Step 1: Define Your Application Features

Before implementing licensing, clearly define the features your application will license. Features should represent distinct functional capabilities.

### 1.1 Create Feature Enumeration

```vb
Option Strict On
Option Explicit On
Option Infer Off

' Define the features your application supports
<Flags>
Public Enum DesignerFeatures As Integer
    None = 0
    Base = 1                    ' Core functionality (always required)
    SchematicEditor = 2         ' Schematic design capabilities
    FormboardDesigner = 4       ' Formboard design features
    CableDesigner = 8           ' Cable routing functionality
    PanelDesigner = 16          ' Panel design tools
    AdvancedRouting = 32        ' Advanced routing algorithms
    DataExchange = 64           ' Import/export capabilities
    Reporting = 128             ' Report generation
    All = Base Or SchematicEditor Or FormboardDesigner Or CableDesigner Or PanelDesigner Or AdvancedRouting Or DataExchange Or Reporting
End Enum
```

### 1.2 Map Features to FlexNet Names

Create a mapping between your enum values and FlexNet feature names:

```vb
Option Strict On
Option Explicit On
Option Infer Off

Public Class FeatureMapping
    Public Shared ReadOnly Property FeatureNames As Dictionary(Of DesignerFeatures, String) = 
        New Dictionary(Of DesignerFeatures, String) From {
            {DesignerFeatures.Base, "E3.Series.Base"},
            {DesignerFeatures.SchematicEditor, "E3.Series.SchematicEditor"},
            {DesignerFeatures.FormboardDesigner, "E3.Series.FormboardDesigner"},
            {DesignerFeatures.CableDesigner, "E3.Series.CableDesigner"},
            {DesignerFeatures.PanelDesigner, "E3.Series.PanelDesigner"},
            {DesignerFeatures.AdvancedRouting, "E3.Series.AdvancedRouting"},
            {DesignerFeatures.DataExchange, "E3.Series.DataExchange"},
            {DesignerFeatures.Reporting, "E3.Series.Reporting"}
        }
End Class
```

## Step 2: Choose Your Implementation Approach

E3.Lib.Licensing offers two main approaches for implementation:

### Option A: Generic Type-Safe Approach (Recommended)

**Best for**: Applications with well-defined feature sets that benefit from compile-time type checking.

**Benefits**:
- Compile-time type safety
- IntelliSense support
- Automatic enum-to-string conversion
- Less boilerplate code

### Option B: Base Class Approach

**Best for**: Applications requiring custom licensing logic or complex feature management.

**Benefits**:
- Full control over feature management
- Custom error handling
- Complex feature relationships
- Legacy system integration

## Step 3: Implement Your License Manager

### 3.1 Generic Implementation (Option A - Recommended)

```vb
Option Strict On
Option Explicit On
Option Infer Off

' Create a concrete license manager using the generic class
Public Class DesignerLicenseManager
    Inherits LicenseManager(Of DesignerFeatures)
    
    ' Override required properties
    Protected Friend Overrides ReadOnly Property Vendor As String
        Get
            Return "ZUKEN_E3"
        End Get
    End Property
    
    Protected Friend Overrides ReadOnly Property ProductName As String
        Get
            Return "E3.Series.Designer"
        End Get
    End Property
    
    Protected Friend Overrides ReadOnly Property MainFeature As String
        Get
            Return FeatureMapping.FeatureNames(DesignerFeatures.Base)
        End Get
    End Property
    
    ' Optional: Override if you need to ignore specific features
    Protected Overrides ReadOnly Property IgnoreFeatures As String()
        Get
            Return New String() {"None", "All"}
        End Get
    End Property
End Class
```

### 3.2 Base Class Implementation (Option B)

```vb
Option Strict On
Option Explicit On
Option Infer Off

' Create a concrete license manager by inheriting from LicenseManagerBase
Public Class CustomDesignerLicenseManager
    Inherits LicenseManagerBase
    
    ' Override required properties
    Protected Friend Overrides ReadOnly Property Vendor As String
        Get
            Return "ZUKEN_E3"
        End Get
    End Property
    
    Protected Friend Overrides ReadOnly Property ProductName As String
        Get
            Return "E3.Series.Designer"
        End Get
    End Property
    
    Protected Friend Overrides ReadOnly Property MainFeature As String
        Get
            Return "E3.Series.Base"  ' Primary feature that must be licensed
        End Get
    End Property
    
    ' Implement required methods
    Protected Friend Overrides Function GetAllFeatureNames() As String()
        Return FeatureMapping.FeatureNames.Values.ToArray()
    End Function
    
    Protected Friend Overrides Function GetAllFeatureValues() As Object()
        Return FeatureMapping.FeatureNames.Keys.Cast(Of Object)().ToArray()
    End Function
    
    Protected Overrides Function GetFeaturesAsString(result As FeaturesResult) As String()
        ' Convert features from registry to string array
        Dim availableFeatures As New List(Of String)
        
        If result.Features IsNot Nothing Then
            Dim featureFlags As Integer = CInt(result.Features)
            
            ' Map flag values to feature names
            For Each kvp In FeatureMapping.FeatureNames
                If (featureFlags And CInt(kvp.Key)) > 0 Then
                    availableFeatures.Add(kvp.Value)
                End If
            Next
        Else
            ' If no features are defined, at least include the main feature
            availableFeatures.Add(MainFeature)
        End If
        
        Return availableFeatures.ToArray()
    End Function
    
    Protected Overrides Function TryGetFeatureStringAsValue(featureName As String, ByRef value As Object) As Boolean
        ' Convert feature name to its corresponding value
        For Each kvp In FeatureMapping.FeatureNames
            If kvp.Value = featureName Then
                value = kvp.Key
                Return True
            End If
        Next
        Return False
    End Function
End Class
```

## Step 4: Integrate Licensing into Your Application

### 4.1 Basic Application Integration

```vb
Option Strict On
Option Explicit On
Option Infer Off

Public Class DesignerApplication
    Inherits Form
    
    Private ReadOnly _licenseManager As DesignerLicenseManager
    Private _requiredFeatures As DesignerFeatures() = {
        DesignerFeatures.Base,
        DesignerFeatures.SchematicEditor
    }
    
    Public Sub New()
        InitializeComponent()
        
        ' Create and initialize the license manager
        _licenseManager = New DesignerLicenseManager()
        
        ' Initialize licensing and check result
        If Not InitializeLicensing() Then
            Application.Exit()
            Return
        End If
        
        ' Enable/disable features based on licensing
        UpdateFeaturesAvailability()
    End Sub
    
    Private Function InitializeLicensing() As Boolean
        Try
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
            
            ' Check for missing critical features
            If Not authResult.AuthenticatedFeatures.Contains(DesignerFeatures.Base) Then
                ShowError("Critical feature 'Base' is not available. Application cannot start.")
                Return False
            End If
            
            ' Warn about missing optional features
            For Each feature In _requiredFeatures
                If Not authResult.AuthenticatedFeatures.Contains(feature) Then
                    ShowWarning(String.Format("Feature {0} not available", feature))
                End If
            Next
            
            Return True
            
        Catch ex As Exception
            ShowError(String.Format("Unexpected licensing error: {0}", ex.Message))
            Return False
        End Try
    End Function
    
    Private Sub UpdateFeaturesAvailability()
        ' Get all available features
        Dim availableFeatures As DesignerFeatures() = _licenseManager.GetAvailableFeatures()
        
        ' Enable/disable UI elements based on available features
        schematicEditorMenuItem.Enabled = availableFeatures.Contains(DesignerFeatures.SchematicEditor)
        formboardDesignerMenuItem.Enabled = availableFeatures.Contains(DesignerFeatures.FormboardDesigner)
        cableDesignerMenuItem.Enabled = availableFeatures.Contains(DesignerFeatures.CableDesigner)
        panelDesignerMenuItem.Enabled = availableFeatures.Contains(DesignerFeatures.PanelDesigner)
        advancedRoutingMenuItem.Enabled = availableFeatures.Contains(DesignerFeatures.AdvancedRouting)
        dataExchangeMenuItem.Enabled = availableFeatures.Contains(DesignerFeatures.DataExchange)
        reportingMenuItem.Enabled = availableFeatures.Contains(DesignerFeatures.Reporting)
        
        ' Update status bar
        UpdateStatusBar(availableFeatures)
    End Sub
    
    Private Sub UpdateStatusBar(availableFeatures As DesignerFeatures())
        Dim featureCount As Integer = availableFeatures.Length
        statusLabel.Text = String.Format("{0} licensed feature(s) available", featureCount)
    End Sub
    
    Protected Overrides Sub OnFormClosing(e As FormClosingEventArgs)
        ' Ensure license is properly released when application closes
        Try
            _licenseManager?.TerminateLicense()
            _licenseManager?.Dispose()
        Catch ex As Exception
            ' Log but don't prevent shutdown
            Console.WriteLine(String.Format("Error during license cleanup: {0}", ex.Message))
        End Try
        
        MyBase.OnFormClosing(e)
    End Sub
    
    Private Sub ShowError(message As String)
        MessageBox.Show(message, "Licensing Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
    End Sub
    
    Private Sub ShowWarning(message As String)
        MessageBox.Show(message, "Licensing Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
    End Sub
End Class
```

### 4.2 Feature-Specific Functionality

```vb
Option Strict On
Option Explicit On
Option Infer Off

' Add these methods to your main application class
Public Function CanUseSchematicEditor() As Boolean
    Return _licenseManager.GetAvailableFeatures().Contains(DesignerFeatures.SchematicEditor)
End Function

Public Function CanExportData() As Boolean
    Return _licenseManager.GetAvailableFeatures().Contains(DesignerFeatures.DataExchange)
End Function

Public Function CanGenerateReports() As Boolean
    Return _licenseManager.GetAvailableFeatures().Contains(DesignerFeatures.Reporting)
End Function

' Example of feature-protected functionality
Private Sub ExportDataMenuItem_Click(sender As Object, e As EventArgs)
    If Not CanExportData() Then
        ShowError("Data export feature is not available with your current license.")
        Return
    End If
    
    ' Proceed with export functionality
    PerformDataExport()
End Sub

Private Sub PerformDataExport()
    ' Implementation of export functionality
    ' This code only runs when the feature is properly licensed
End Sub
```

## Step 5: Handle Errors and Edge Cases

### 5.1 Comprehensive Error Handling

```vb
Option Strict On
Option Explicit On
Option Infer Off

Private Sub HandleLicenseError(result As LicenseErrorCodeResult)
    Dim errorMessage As String = String.Format("License error: {0}", result.Message)
    Dim canRetry As Boolean = False
    
    ' Handle specific error codes with appropriate user guidance
    Select Case result.LicensingErrorCode
        Case LicensingErrorCodes.SUCCESS
            ' No error
            Return
            
        Case LicensingErrorCodes.NO_SERVER_RESPONSE, LicensingErrorCodes.SERVER_NOT_RESPONDING
            errorMessage = "Cannot connect to license server. Please check your network connection and try again."
            canRetry = True
            
        Case LicensingErrorCodes.FEATURE_NOT_FOUND
            errorMessage = "The requested feature is not available in your license. Please contact your administrator."
            
        Case LicensingErrorCodes.INSUFFICIENT_FEATURES
            errorMessage = "All licenses for this feature are currently in use. Please try again later."
            canRetry = True
            
        Case LicensingErrorCodes.AUTHENTICATION_FAILED
            errorMessage = "License authentication failed. Please verify your license configuration."
            
        Case LicensingErrorCodes.BORROW_FEATURES_FAILED
            errorMessage = "Unable to borrow license for offline use. Please check with your administrator."
            
        Case LicensingErrorCodes.INVALID_FEATURE_SPECIFICATION
            errorMessage = "Invalid feature specification. This may indicate a configuration problem."
            
        Case Else
            errorMessage = String.Format("{0}{1}Error code: {2}", errorMessage, Environment.NewLine, result.LicensingErrorCode)
    End Select
    
    ' Show error dialog with retry option if applicable
    If canRetry Then
        Dim dialogResult As DialogResult = MessageBox.Show(
            String.Format("{0}{1}{1}Would you like to retry?", errorMessage, Environment.NewLine),
            "Licensing Error",
            MessageBoxButtons.RetryCancel,
            MessageBoxIcon.Error)
            
        If dialogResult = DialogResult.Retry Then
            ' Attempt to reinitialize licensing
            InitializeLicensing()
        End If
    Else
        MessageBox.Show(errorMessage, "Licensing Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
    End If
End Sub
```

### 5.2 Network Connectivity Issues

```vb
Option Strict On
Option Explicit On
Option Infer Off

Public Function CheckLicenseServerConnectivity() As Boolean
    Try
        ' Test connection to license server
        Dim statusResult As StatusResult = _licenseManager.Status.GetStatus(DesignerFeatures.Base.ToString())
        Return statusResult.IsSuccess
    Catch ex As Exception
        Return False
    End Try
End Function

Public Sub HandleOfflineScenario()
    If Not CheckLicenseServerConnectivity() Then
        ' Check if we have borrowed licenses
        If _licenseManager.Borrow.IsBorrowed(DesignerFeatures.Base.ToString()) Then
            ShowInfo("Working offline with borrowed license.")
        Else
            Dim result As DialogResult = MessageBox.Show(
                "Cannot connect to license server. Would you like to borrow a license for offline use?",
                "License Server Unavailable",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question)
                
            If result = DialogResult.Yes Then
                ShowBorrowLicenseDialog()
            End If
        End If
    End If
End Sub
```

## Step 6: Implement Advanced Features

### 6.1 License Status Monitoring

```vb
Option Strict On
Option Explicit On
Option Infer Off

Public Sub MonitorLicenseStatus()
    ' Check license expiration for all authenticated features
    Dim authenticatedFeatures As DesignerFeatures() = _licenseManager.GetAvailableFeatures()
    
    For Each feature In authenticatedFeatures
        If _licenseManager.Status.IsExpiring(feature.ToString(), 30) Then ' 30 days warning
            Dim expirationDate As DateResult = _licenseManager.Status.GetExpirationDate(feature.ToString())
            If expirationDate.IsSuccess Then
                ShowWarning(String.Format("Feature {0} expires on {1}", feature, expirationDate.Value.ToShortDateString()))
            End If
        End If
    Next
    
    ' Check server connectivity
    If Not CheckLicenseServerConnectivity() Then
        ShowWarning("License server is not available. Some features may not work correctly.")
    End If
End Sub

Public Sub GetDetailedLicenseStatus()
    Dim statusResult As StatusResult = _licenseManager.Status.GetStatus(DesignerFeatures.Base.ToString())
    
    If statusResult.IsSuccess Then
        Dim statusInfo As String = String.Format(
            "License Status:{0}" &
            "Feature: {1}{0}" &
            "Users: {2}/{3}{0}" &
            "Server: Available",
            Environment.NewLine,
            DesignerFeatures.Base.ToString(),
            statusResult.UsersCount,
            statusResult.MaxUsers)
            
        If statusResult.HasExpirationDate Then
            statusInfo = String.Format("{0}{1}Expires: {2}",
                statusInfo,
                Environment.NewLine,
                statusResult.ExpirationDate.ToShortDateString())
        End If
        
        MessageBox.Show(statusInfo, "License Status", MessageBoxButtons.OK, MessageBoxIcon.Information)
    Else
        HandleLicenseError(statusResult)
    End If
End Sub
```

### 6.2 License Borrowing for Offline Use

```vb
Option Strict On
Option Explicit On
Option Infer Off

Public Sub ShowBorrowLicenseDialog()
    Dim dialog As New BorrowLicenseDialog()
    
    If dialog.ShowDialog() = DialogResult.OK Then
        BorrowLicense(dialog.SelectedFeature, dialog.BorrowDays)
    End If
End Sub

Public Sub BorrowLicense(feature As DesignerFeatures, days As Integer)
    ' Validate borrowing parameters
    If days < 1 OrElse days > 365 Then
        ShowError("Borrow period must be between 1 and 365 days.")
        Return
    End If
    
    Dim endDate As DateTime = DateTime.Now.AddDays(days)
    Dim borrowResult As BorrowResult = _licenseManager.Borrow.BorrowLicense(feature.ToString(), endDate)
    
    If borrowResult.IsSuccess Then
        MessageBox.Show(
            String.Format("License for {0} successfully borrowed until {1}", feature, endDate.ToShortDateString()),
            "License Borrowed",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information)
    Else
        HandleLicenseError(borrowResult)
    End If
End Sub

Public Sub ReturnBorrowedLicense(feature As DesignerFeatures)
    Dim returnResult As BorrowResult = _licenseManager.Borrow.ReturnBorrowedLicense(feature.ToString())
    
    If returnResult.IsSuccess Then
        MessageBox.Show(
            String.Format("Borrowed license for {0} has been returned successfully.", feature),
            "License Returned",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information)
    Else
        HandleLicenseError(returnResult)
    End Sub
End Sub

Public Sub ShowBorrowedLicenses()
    Dim borrowedFeatures As String() = _licenseManager.Borrow.GetBorrowedFeatures()
    
    If borrowedFeatures.Length = 0 Then
        MessageBox.Show("No licenses are currently borrowed.", "Borrowed Licenses", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Return
    End If
    
    Dim message As String = "Currently borrowed licenses:" & Environment.NewLine & Environment.NewLine
    
    For Each featureName In borrowedFeatures
        Dim borrowInfo As BorrowInfo = _licenseManager.Borrow.GetBorrowInfo(featureName)
        If borrowInfo IsNot Nothing Then
            message = String.Format("{0}{1}: Until {2} ({3} days remaining){4}",
                message,
                featureName,
                borrowInfo.EndDate.ToShortDateString(),
                borrowInfo.DaysRemaining,
                Environment.NewLine)
        End If
    Next
    
    MessageBox.Show(message, "Borrowed Licenses", MessageBoxButtons.OK, MessageBoxIcon.Information)
End Sub
```

### 6.3 Dynamic Feature Authentication

```vb
Option Strict On
Option Explicit On
Option Infer Off

Public Function AuthenticateFeatureOnDemand(feature As DesignerFeatures) As Boolean
    ' Check if feature is already authenticated
    If _licenseManager.GetAvailableFeatures().Contains(feature) Then
        Return True
    End If
    
    ' Attempt to authenticate the feature
    Dim authResult As AuthFeaturesResult = _licenseManager.AuthenticateFeatures(feature)
    
    If authResult.IsSuccess AndAlso authResult.AuthenticatedFeatures.Contains(feature) Then
        ' Update UI to reflect newly available feature
        UpdateFeaturesAvailability()
        Return True
    Else
        HandleLicenseError(authResult)
        Return False
    End If
End Function

Public Sub TryAdvancedFeature()
    If Not AuthenticateFeatureOnDemand(DesignerFeatures.AdvancedRouting) Then
        ShowError("Advanced routing feature is not available. Please check your license.")
        Return
    End If
    
    ' Proceed with advanced routing functionality
    PerformAdvancedRouting()
End Sub
```

## Step 7: Testing and Validation

### 7.1 Unit Testing License Manager

```vb
Option Strict On
Option Explicit On
Option Infer Off

<TestClass>
Public Class LicenseManagerTests
    Private _licenseManager As DesignerLicenseManager
    
    <TestInitialize>
    Public Sub Setup()
        _licenseManager = New DesignerLicenseManager()
    End Sub
    
    <TestMethod>
    Public Sub TestInitialization()
        Dim result As Result = _licenseManager.Init()
        Assert.IsTrue(result.IsSuccess, String.Format("Initialization failed: {0}", result.Message))
    End Sub
    
    <TestMethod>
    Public Sub TestFeatureAuthentication()
        _licenseManager.Init()
        Dim result As AuthFeaturesResult = _licenseManager.AuthenticateFeatures(DesignerFeatures.Base)
        Assert.IsTrue(result.IsSuccess, String.Format("Authentication failed: {0}", result.Message))
        Assert.IsTrue(result.AuthenticatedFeatures.Contains(DesignerFeatures.Base))
    End Sub
    
    <TestMethod>
    Public Sub TestFeatureAvailability()
        _licenseManager.Init()
        _licenseManager.AuthenticateFeatures(DesignerFeatures.Base)
        
        Dim availableFeatures As DesignerFeatures() = _licenseManager.GetAvailableFeatures()
        Assert.IsTrue(availableFeatures.Length > 0, "No features available")
        Assert.IsTrue(availableFeatures.Contains(DesignerFeatures.Base), "Base feature not available")
    End Sub
    
    <TestCleanup>
    Public Sub Cleanup()
        _licenseManager?.TerminateLicense()
        _licenseManager?.Dispose()
    End Sub
End Class
```

### 7.2 Integration Testing

```vb
Option Strict On
Option Explicit On
Option Infer Off

Public Class LicenseIntegrationTest
    Public Shared Sub RunIntegrationTests()
        Console.WriteLine("Starting license integration tests...")
        
        ' Test 1: Basic initialization
        TestBasicInitialization()
        
        ' Test 2: Feature authentication
        TestFeatureAuthentication()
        
        ' Test 3: Error handling
        TestErrorHandling()
        
        ' Test 4: Borrowing functionality
        TestBorrowFunctionality()
        
        Console.WriteLine("Integration tests completed.")
    End Sub
    
    Private Shared Sub TestBasicInitialization()
        Console.WriteLine("Testing basic initialization...")
        
        Using licenseManager As New DesignerLicenseManager()
            Dim result As Result = licenseManager.Init(createRegKeys:=True)
            
            If result.IsSuccess Then
                Console.WriteLine("✓ Initialization successful")
            Else
                Console.WriteLine(String.Format("✗ Initialization failed: {0}", result.Message))
            End If
        End Using
    End Sub
    
    Private Shared Sub TestFeatureAuthentication()
        Console.WriteLine("Testing feature authentication...")
        
        Using licenseManager As New DesignerLicenseManager()
            licenseManager.Init()
            
            Dim result As AuthFeaturesResult = licenseManager.AuthenticateFeatures(
                DesignerFeatures.Base,
                DesignerFeatures.SchematicEditor)
                
            If result.IsSuccess Then
                Console.WriteLine(String.Format("✓ Authenticated {0} features", result.AuthenticatedFeatures.Count))
            Else
                Console.WriteLine(String.Format("✗ Authentication failed: {0}", result.Message))
            End If
        End Using
    End Sub
End Class
```

## Best Practices and Tips

### Performance Optimization

1. **Initialize Once**: Create and initialize the license manager once at application startup
2. **Cache Results**: Store authentication results to avoid repeated license server calls
3. **Background Checks**: Perform status checks in background threads for long-running applications
4. **Lazy Authentication**: Only authenticate features when they're actually needed

### Security Considerations

1. **Secure Storage**: Never store license keys or sensitive information in plain text
2. **Validation**: Always validate license results before enabling features
3. **Error Messages**: Provide helpful error messages without exposing sensitive system information
4. **Logging**: Log licensing events for audit and troubleshooting purposes

### User Experience

1. **Clear Messaging**: Provide clear, actionable error messages to users
2. **Graceful Degradation**: Allow the application to function with reduced capabilities when features are unavailable
3. **Status Indicators**: Show license status in the UI (status bar, about dialog)
4. **Offline Support**: Implement license borrowing for users who work offline

### Maintenance and Support

1. **Logging**: Implement comprehensive logging for troubleshooting
2. **Diagnostics**: Provide diagnostic tools for administrators
3. **Documentation**: Document your feature mappings and licensing requirements
4. **Testing**: Implement automated tests for licensing functionality

## Troubleshooting Common Issues

### Issue: License Manager Initialization Fails

**Symptoms**: `Init()` method returns failure result

**Common Causes**:
- FlexNet wrapper DLLs not found
- Registry access permissions
- Network connectivity issues

**Solutions**:
```vb
Option Strict On
Option Explicit On
Option Infer Off

' Check FlexNet wrapper availability
If Not System.IO.File.Exists("E3.Lib.Licensing.MFC.dll") Then
    ShowError("Required licensing components not found. Please reinstall the application.")
    Return
End If

' Try initialization with detailed error handling
Dim initResult As Result = _licenseManager.Init(createRegKeys:=True)
If Not initResult.IsSuccess Then
    If TypeOf initResult Is LicenseErrorCodeResult Then
        Dim detailedResult As LicenseErrorCodeResult = DirectCast(initResult, LicenseErrorCodeResult)
        Console.WriteLine(String.Format("Error Code: {0}", detailedResult.LicensingErrorCode))
        Console.WriteLine(String.Format("Diagnostic Code: {0}", detailedResult.DiagnosticsErrorCode))
    End If
End If
```

### Issue: Features Not Available

**Symptoms**: Authentication succeeds but features are not available

**Common Causes**:
- Incorrect feature names
- License file doesn't include requested features
- Registry corruption

**Solutions**:
```vb
Option Strict On
Option Explicit On
Option Infer Off

' Verify feature names match FlexNet license file
Public Sub VerifyFeatureNames()
    Dim serverFeatures As ApplicationFeatureInfosResult = _licenseManager.Status.GetAvailableFeaturesFromServer()
    
    If serverFeatures.IsSuccess Then
        Console.WriteLine("Available features on server:")
        For Each feature In serverFeatures.Features
            Console.WriteLine(String.Format("- {0} (Version: {1})", feature.Name, feature.Version))
        Next
    End If
End Sub

' Clear and regenerate registry keys
Public Sub ResetRegistryKeys()
    _licenseManager.TerminateLicense()
    
    ' Delete existing registry keys (requires administrator privileges)
    ' Microsoft.Win32.Registry.CurrentUser.DeleteSubKeyTree("Software\\Zuken\\YourProduct", False)
    
    ' Reinitialize with fresh registry keys
    Dim result As Result = _licenseManager.Init(createRegKeys:=True)
End Sub
```

### Issue: License Server Connectivity Problems

**Symptoms**: Cannot connect to license server

**Solutions**:
```vb
Option Strict On
Option Explicit On
Option Infer Off

Public Function DiagnoseLicenseServer() As String
    Dim diagnostics As String = _licenseManager.GetDiagnosticInfo()
    
    ' Check server status using external tools
    Dim lmstatResult As ProcessResult = _licenseManager.Status.RunLmStat()
    If lmstatResult.IsSuccess Then
        diagnostics = String.Format("{0}{1}{1}LMStat Output:{1}{2}",
            diagnostics,
            Environment.NewLine,
            lmstatResult.Output)
    End If
    
    Return diagnostics
End Function
```

---

## Next Steps

Continue your journey with E3.Lib.Licensing:

📚 **Deep Dive into Architecture**: [Documentation-02-Architecture-Overview.md](./Documentation-02-Architecture-Overview.md)  
🔧 **Explore Advanced Features**: [Documentation-11-Advanced-Concepts.md](./Documentation-11-Advanced-Concepts.md)  
📖 **Complete API Reference**: [Documentation-15-Technical-Reference.md](./Documentation-15-Technical-Reference.md)  
⚡ **Quick Daily Reference**: [Quick-Reference.md](./Quick-Reference.md)
