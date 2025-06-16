# Integration Examples

This section provides comprehensive, real-world integration examples for the E3.Lib.Licensing library. These examples demonstrate common usage patterns, best practices, and implementation strategies for different application scenarios.

### Overview

The integration examples cover:
- **Basic Application Integration**: Simple licensing setup for desktop applications
- **WinForms Integration**: UI-specific integration patterns and error handling
- **Service and Console Applications**: Non-interactive licensing scenarios
- **Enterprise Integration**: Advanced patterns for large-scale deployments
- **Testing and Development**: Patterns for development and testing environments
- **Error Handling and Recovery**: Robust error handling and user experience patterns

## Basic Application Integration

### Simple Desktop Application

This example demonstrates the fundamental integration pattern for a typical E3Series desktop application:

```vb
Option Strict On
Option Explicit On
Option Infer Off

' Feature enumeration definition
<Flags>
Public Enum E3SeriesFeatures As Integer
    None = 0
    BaseFeature = 1
    SchematicEditor = 2
    PanelEditor = 4
    CableDesigner = 8
    Reports = 16
    ThreeDIntegration = 32
    DataExchange = 64
End Enum

' Concrete license manager implementation
Public Class E3SeriesLicenseManager
    Inherits LicenseManager(Of E3SeriesFeatures)
    
    Protected Overrides ReadOnly Property Vendor As String
        Get
            Return "zuken"
        End Get
    End Property
    
    Public Overrides ReadOnly Property ProductName As String
        Get
            Return "E3.Series"
        End Get
    End Property
    
    ' Optional: Custom feature initialization
    Protected Overrides Sub InitAvailableFeatures(
        Optional createRegKeys As Boolean = False,
        Optional availableFeaturesKeySetting As AvailableFeatureKeySettings = Nothing
    )
        ' Use default behavior with all features available
        If availableFeaturesKeySetting Is Nothing Then
            availableFeaturesKeySetting = New AvailableFeatureKeySettings(
                GetAllFeaturesAsFlag(),  ' Value when creating new registry entry
                GetAllFeaturesAsFlag()   ' Default value when registry entry missing
            )
        End If
        
        MyBase.InitAvailableFeatures(createRegKeys, availableFeaturesKeySetting)
    End Sub
End Class

' Main application class
Public Class E3SeriesApplication
    Private _licenseManager As E3SeriesLicenseManager
    Private _logger As ILogger
    
    Public Sub New()
        _logger = LogManager.GetCurrentClassLogger()
        _licenseManager = New E3SeriesLicenseManager()
    End Sub
    
    Public Function Initialize() As Boolean
        Try
            ' Initialize licensing system
            Dim initResult As Result = _licenseManager.Init(createRegKeys:=True)
            If Not initResult.IsSuccess Then
                _logger.Error(String.Format(CultureInfo.InvariantCulture, "License initialization failed: {0}", initResult.Message))
                ShowLicenseError("Initialization Error", initResult.Message)
                Return False
            End If
            
            ' Authenticate essential features
            Dim authResult As AuthFeaturesResult = _licenseManager.AuthenticateFeatures(
                E3SeriesFeatures.BaseFeature,
                E3SeriesFeatures.SchematicEditor
            )
            
            If Not authResult.IsSuccess Then
                _logger.Error(String.Format(CultureInfo.InvariantCulture, "Feature authentication failed: {0}", authResult.Message))
                ShowLicenseError("License Authentication", authResult.Message)
                Return False
            End If
            
            ' Log successful authentication
            _logger.Info(String.Format(CultureInfo.InvariantCulture, "Licensed features: {0}", String.Join(", ", authResult.LicensedFeatures)))
            
            ' Configure application based on available features
            ConfigureFeatures(authResult.LicensedFeatures)
            
            Return True
            
        Catch ex As Exception
            _logger.Error(ex, "Unexpected error during license initialization")
            ShowLicenseError("Unexpected Error", ex.Message)
            Return False
        End Try
    End Function
    
    Private Sub ConfigureFeatures(licensedFeatures As ReadOnlyCollection(Of String))
        ' Enable/disable application features based on licensing
        Dim availableFeatures As E3SeriesFeatures() = _licenseManager.GetAvailableFeatures()
        
        ' Configure UI and functionality
        If availableFeatures.Contains(E3SeriesFeatures.PanelEditor) Then
            EnablePanelEditorFeatures()
        End If
        
        If availableFeatures.Contains(E3SeriesFeatures.Reports) Then
            EnableReportingFeatures()
        End If
        
        If availableFeatures.Contains(E3SeriesFeatures.ThreeDIntegration) Then
            Enable3DIntegrationFeatures()
        End If
    End Sub
    
    Private Sub ShowLicenseError(title As String, message As String)
        MessageBox.Show(
            String.Format(CultureInfo.InvariantCulture, "Licensing Error: {0}{1}{1}Please contact your system administrator or check your license configuration.", 
                message, Environment.NewLine),
            title,
            MessageBoxButtons.OK,
            MessageBoxIcon.Error
        )
    End Sub
    
    Public Sub Shutdown()
        Try
            If _licenseManager IsNot Nothing Then
                _licenseManager.TerminateLicense()
                _licenseManager.Dispose()
            End If
            _logger.Info("Application licensing terminated successfully")
        Catch ex As Exception
            _logger.Warn(ex, "Error during license termination")
        End Try
    End Sub
End Class
    End Sub
End Class
```

### Application Entry Point

```vb
Module Program
    Private _application As E3SeriesApplication
    
    <STAThread>
    Sub Main()
        ' Configure application settings
        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(False)
        
        Try
            ' Initialize application
            _application = New E3SeriesApplication()
            
            If Not _application.Initialize() Then
                ' License initialization failed - exit gracefully
                Environment.Exit(1)
                Return
            End If
            
            ' Setup shutdown handler
            AddHandler Application.ApplicationExit, AddressOf Application_ApplicationExit
            
            ' Start main application form
            Application.Run(New MainForm())
            
        Catch ex As Exception
            MessageBox.Show($"Fatal application error: {ex.Message}", "Application Error",
                           MessageBoxButtons.OK, MessageBoxIcon.Error)
            Environment.Exit(1)
        End Try
    End Sub
    
    Private Sub Application_ApplicationExit(sender As Object, e As EventArgs)
        _application?.Shutdown()
    End Sub
End Module
```

## WinForms Integration

### Main Form with License Management

```vb
Public Class MainForm
    Inherits Form
    
    Private _licenseManager As E3SeriesLicenseManager
    Private _licenseStatusTimer As Timer
    
    Public Sub New()
        InitializeComponent()
        InitializeLicensing()
        SetupLicenseMonitoring()
    End Sub
    
    Private Sub InitializeLicensing()
        _licenseManager = New E3SeriesLicenseManager()
        
        ' Initialize and authenticate
        Dim initResult = _licenseManager.Init(True)
        If Not initResult.IsSuccess Then
            HandleLicenseInitializationFailure(initResult)
            Return
        End If
        
        ' Authenticate all available features
        Dim authResult = _licenseManager.AuthenticateAllFeatures()
        UpdateUIBasedOnLicensing(authResult)
    End Sub
    
    Private Sub SetupLicenseMonitoring()
        ' Monitor license status every 30 seconds
        _licenseStatusTimer = New Timer() With {
            .Interval = 30000,
            .Enabled = True
        }
        AddHandler _licenseStatusTimer.Tick, AddressOf CheckLicenseStatus
    End Sub
    
    Private Sub CheckLicenseStatus(sender As Object, e As EventArgs)
        Try
            ' Check status of critical features
            For Each feature In {E3SeriesFeatures.BaseFeature, E3SeriesFeatures.SchematicEditor}
                Dim statusResult = _licenseManager.Status.GetStatus(feature.ToString())
                
                If Not statusResult.IsSuccess Then
                    ShowLicenseWarning($"License status check failed for {feature}: {statusResult.Message}")
                ElseIf statusResult.HasExpirationDate AndAlso statusResult.IsExpiring(7) Then
                    ShowExpirationWarning(feature, statusResult.ExpirationDate)
                End If
            Next
            
        Catch ex As Exception
            ' Log but don't disrupt user workflow
            Logger.Warn(ex, "Error during periodic license check")
        End Try
    End Sub
    
    Private Sub UpdateUIBasedOnLicensing(authResult As AuthFeaturesResult)
        If authResult.IsSuccess Then
            ' Enable features based on licensing
            Dim availableFeatures = _licenseManager.GetAvailableFeatures()
            
            ' Update menu items
            PanelEditorMenuItem.Enabled = availableFeatures.Contains(E3SeriesFeatures.PanelEditor)
            ReportsMenuItem.Enabled = availableFeatures.Contains(E3SeriesFeatures.Reports)
            ThreeDMenuItem.Enabled = availableFeatures.Contains(E3SeriesFeatures.ThreeDIntegration)
            
            ' Update toolbar buttons
            PanelEditorButton.Enabled = availableFeatures.Contains(E3SeriesFeatures.PanelEditor)
            
            ' Show license status in status bar
            LicenseStatusLabel.Text = $"Licensed: {String.Join(", ", authResult.LicensedFeatures)}"
            LicenseStatusLabel.ForeColor = Color.Green
        Else
            ' Handle authentication failure
            HandleAuthenticationFailure(authResult)
        End If
    End Sub
    
    Private Sub HandleAuthenticationFailure(result As AuthFeaturesResult)
        ' Show error details
        LicenseStatusLabel.Text = "License Error"
        LicenseStatusLabel.ForeColor = Color.Red
        
        ' Disable premium features
        DisableAdvancedFeatures()
        
        ' Show recovery options
        ShowLicenseRecoveryDialog(result)
    End Sub
    
    Private Sub ShowLicenseRecoveryDialog(result As AuthFeaturesResult)
        Using dialog As New LicenseRecoveryDialog(result, _licenseManager)
            If dialog.ShowDialog(Me) = DialogResult.OK Then
                ' User chose to retry or took corrective action
                RetryLicenseAuthentication()
            End If
        End Using
    End Sub
    
    Private Sub RetryLicenseAuthentication()
        Dim authResult = _licenseManager.AuthenticateAllFeatures()
        UpdateUIBasedOnLicensing(authResult)
    End Sub
    
    Protected Overrides Sub OnFormClosing(e As FormClosingEventArgs)
        Try
            _licenseStatusTimer?.Stop()
            _licenseStatusTimer?.Dispose()
            _licenseManager?.TerminateLicense()
            _licenseManager?.Dispose()
        Catch ex As Exception
            ' Log but don't prevent shutdown
            Logger.Warn(ex, "Error during form closing")
        End Try
        
        MyBase.OnFormClosing(e)
    End Sub
End Class
```

### License Recovery Dialog

```vb
Public Class LicenseRecoveryDialog
    Inherits Form
    
    Private _authResult As AuthFeaturesResult
    Private _licenseManager As E3SeriesLicenseManager
    
    Public Sub New(authResult As AuthFeaturesResult, licenseManager As E3SeriesLicenseManager)
        InitializeComponent()
        _authResult = authResult
        _licenseManager = licenseManager
        PopulateErrorInformation()
    End Sub
    
    Private Sub PopulateErrorInformation()
        ' Show error details
        ErrorMessageTextBox.Text = _authResult.Message
        
        ' Show specific error code if available
        If TypeOf _authResult Is ILicenseErrorCodeResult Then
            Dim licResult = CType(_authResult, ILicenseErrorCodeResult)
            ErrorCodeLabel.Text = $"Error Code: {licResult.ErrorCode}"
        End If
        
        ' Show recovery suggestions
        PopulateRecoverySuggestions()
    End Sub
    
    Private Sub PopulateRecoverySuggestions()
        RecoverySuggestionsListBox.Items.Clear()
        
        If TypeOf _authResult Is ILicenseErrorCodeResult Then
            Dim licResult = CType(_authResult, ILicenseErrorCodeResult)
            
            Select Case licResult.ErrorCode
                Case LicensingErrorCodes.SERVER_NOT_ACCESSIBLE
                    RecoverySuggestionsListBox.Items.AddRange({
                        "1. Check network connectivity",
                        "2. Verify license server is running",
                        "3. Check firewall settings",
                        "4. Try borrowing licenses for offline use"
                    })
                    
                Case LicensingErrorCodes.AUTH_FAILED
                    RecoverySuggestionsListBox.Items.AddRange({
                        "1. Verify license file contains required features",
                        "2. Check if all licenses are in use",
                        "3. Contact license administrator",
                        "4. Try again in a few minutes"
                    })
                    
                Case LicensingErrorCodes.DATE_EXPIRED
                    RecoverySuggestionsListBox.Items.AddRange({
                        "1. Contact license administrator for renewal",
                        "2. Update license file",
                        "3. Check system date/time settings"
                    })
                    BorrowButton.Enabled = False  ' Can't borrow expired licenses
                    
                Case Else
                    RecoverySuggestionsListBox.Items.AddRange({
                        "1. Restart the application",
                        "2. Check license configuration",
                        "3. Contact technical support"
                    })
            End Select
        End If
    End Sub
    
    Private Sub RetryButton_Click(sender As Object, e As EventArgs) Handles RetryButton.Click
        ' Attempt to re-authenticate
        Dim authResult = _licenseManager.AuthenticateAllFeatures()
        
        If authResult.IsSuccess Then
            MessageBox.Show("License authentication successful!", "Success",
                           MessageBoxButtons.OK, MessageBoxIcon.Information)
            Me.DialogResult = DialogResult.OK
            Me.Close()
        Else
            ' Update error information
            _authResult = authResult
            PopulateErrorInformation()
            MessageBox.Show("Authentication still failing. Please try the suggested recovery steps.",
                           "Authentication Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If
    End Sub
    
    Private Sub BorrowButton_Click(sender As Object, e As EventArgs) Handles BorrowButton.Click
        ' Show borrow licenses dialog
        Using borrowDialog As New BorrowLicensesDialog(_licenseManager)
            borrowDialog.ShowDialog(Me)
        End Using
    End Sub
    
    Private Sub DiagnosticsButton_Click(sender As Object, e As EventArgs) Handles DiagnosticsButton.Click
        ' Show diagnostic information
        Using diagDialog As New DiagnosticsDialog(_licenseManager)
            diagDialog.ShowDialog(Me)
        End Using
    End Sub
End Class
```

## Service and Console Applications

### Windows Service Integration

```vb
Public Class LicensingService
    Inherits ServiceBase
    
    Private _licenseManager As E3SeriesLicenseManager
    Private _monitoringTimer As Timer
    Private _logger As ILogger
    
    Public Sub New()
        MyBase.New()
        InitializeComponent()
        _logger = LogManager.GetCurrentClassLogger()
    End Sub
    
    Protected Overrides Sub OnStart(args As String())
        Try
            _logger.Info("Starting licensing service")
            
            ' Initialize license manager
            _licenseManager = New E3SeriesLicenseManager()
            
            Dim initResult = _licenseManager.Init(createRegKeys:=True)
            If Not initResult.IsSuccess Then
                _logger.Error($"License initialization failed: {initResult.Message}")
                ExitCode = 1
                Stop()
                Return
            End If
            
            ' Authenticate required features for service operation
            Dim authResult = _licenseManager.AuthenticateFeatures(
                E3SeriesFeatures.BaseFeature,
                E3SeriesFeatures.DataExchange
            )
            
            If Not authResult.IsSuccess Then
                _logger.Error($"Feature authentication failed: {authResult.Message}")
                ExitCode = 2
                Stop()
                Return
            End If
            
            ' Setup monitoring
            SetupLicenseMonitoring()
            
            _logger.Info($"Service started successfully with features: {String.Join(", ", authResult.LicensedFeatures)}")
            
        Catch ex As Exception
            _logger.Fatal(ex, "Fatal error during service startup")
            ExitCode = 3
            Stop()
        End Try
    End Sub
    
    Protected Overrides Sub OnStop()
        Try
            _logger.Info("Stopping licensing service")
            
            _monitoringTimer?.Stop()
            _monitoringTimer?.Dispose()
            
            _licenseManager?.TerminateLicense()
            _licenseManager?.Dispose()
            
            _logger.Info("Service stopped successfully")
            
        Catch ex As Exception
            _logger.Error(ex, "Error during service shutdown")
        End Try
    End Sub
    
    Private Sub SetupLicenseMonitoring()
        ' Monitor license status every 5 minutes for services
        _monitoringTimer = New Timer(300000)  ' 5 minutes
        AddHandler _monitoringTimer.Elapsed, AddressOf MonitorLicenseStatus
        _monitoringTimer.Start()
    End Sub
    
    Private Sub MonitorLicenseStatus(sender As Object, e As ElapsedEventArgs)
        Try
            Dim statusResult = _licenseManager.Status.GetStatus(E3SeriesFeatures.BaseFeature.ToString())
            
            If Not statusResult.IsSuccess Then
                _logger.Warn($"License status check failed: {statusResult.Message}")
                
                ' Attempt to recover by re-authenticating
                Dim authResult = _licenseManager.AuthenticateFeatures(E3SeriesFeatures.BaseFeature)
                If authResult.IsSuccess Then
                    _logger.Info("License successfully recovered")
                Else
                    _logger.Error($"License recovery failed: {authResult.Message}")
                    ' Consider stopping service or entering degraded mode
                End If
            ElseIf statusResult.HasExpirationDate AndAlso statusResult.IsExpiring(1) Then
                _logger.Warn($"License expiring soon: {statusResult.ExpirationDate}")
                ' Notify administrators
            End If
            
        Catch ex As Exception
            _logger.Error(ex, "Error during license monitoring")
        End Try
    End Sub
End Class
```

### Console Application Integration

```vb
Module ConsoleApplication
    Private _licenseManager As E3SeriesLicenseManager
    Private _cancellationTokenSource As New CancellationTokenSource()
    
    Sub Main(args As String())
        ' Setup console cancellation handling
        AddHandler Console.CancelKeyPress, AddressOf Console_CancelKeyPress
        
        Try
            Console.WriteLine("E3Series Console Processor")
            Console.WriteLine("Initializing licensing...")
            
            If Not InitializeLicensing() Then
                Environment.Exit(1)
                Return
            End If
            
            Console.WriteLine("Processing data...")
            ProcessData()
            
            Console.WriteLine("Processing completed successfully.")
            
        Catch ex As Exception
            Console.WriteLine($"Error: {ex.Message}")
            Environment.Exit(1)
        Finally
            CleanupLicensing()
        End Try
    End Sub
    
    Private Function InitializeLicensing() As Boolean
        _licenseManager = New E3SeriesLicenseManager()
        
        ' Initialize with automatic registry creation
        Dim initResult = _licenseManager.Init(createRegKeys:=True)
        If Not initResult.IsSuccess Then
            Console.WriteLine($"License initialization failed: {initResult.Message}")
            Return False
        End If
        
        ' Authenticate features needed for data processing
        Dim authResult = _licenseManager.AuthenticateFeatures(
            E3SeriesFeatures.BaseFeature,
            E3SeriesFeatures.DataExchange
        )
        
        If Not authResult.IsSuccess Then
            Console.WriteLine($"Feature authentication failed: {authResult.Message}")
            
            ' Try to provide helpful information
            If authResult.FailedFeatures.Any() Then
                Console.WriteLine("Failed features:")
                For Each failedFeature In authResult.FailedFeatures
                    Console.WriteLine($"  - {failedFeature.Item1}: {failedFeature.Item2.Message}")
                Next
            End If
            
            Return False
        End If
        
        Console.WriteLine($"Licensed features: {String.Join(", ", authResult.LicensedFeatures)}")
        Return True
    End Function
    
    Private Sub ProcessData()
        ' Simulate data processing work
        For i As Integer = 1 To 100
            If _cancellationTokenSource.Token.IsCancellationRequested Then
                Console.WriteLine("Operation cancelled by user.")
                Exit For
            End If
            
            ' Check license status periodically during long operations
            If i Mod 25 = 0 Then
                CheckLicenseStatus()
            End If
            
            ' Simulate work
            Thread.Sleep(100)
            
            ' Update progress
            If i Mod 10 = 0 Then
                Console.Write(".")
            End If
        Next
        
        Console.WriteLine()
    End Sub
    
    Private Sub CheckLicenseStatus()
        Try
            Dim statusResult = _licenseManager.Status.GetStatus(E3SeriesFeatures.BaseFeature.ToString())
            If Not statusResult.IsSuccess Then
                Console.WriteLine($"Warning: License status check failed: {statusResult.Message}")
            End If
        Catch ex As Exception
            Console.WriteLine($"Warning: Error checking license status: {ex.Message}")
        End Try
    End Sub
    
    Private Sub Console_CancelKeyPress(sender As Object, e As ConsoleCancelEventArgs)
        Console.WriteLine("Cancellation requested...")
        _cancellationTokenSource.Cancel()
        e.Cancel = True  ' Prevent immediate termination
    End Sub
    
    Private Sub CleanupLicensing()
        Try
            _licenseManager?.TerminateLicense()
            _licenseManager?.Dispose()
            Console.WriteLine("Licensing cleaned up successfully.")
        Catch ex As Exception
            Console.WriteLine($"Warning: Error during licensing cleanup: {ex.Message}")
        End Try
    End Sub
End Module
```

## Advanced Enterprise Patterns

### Dependency Injection Integration

```vb
' Service interface
Public Interface ILicenseService
    Function IsFeatureAvailable(feature As E3SeriesFeatures) As Boolean
    Function GetAvailableFeatures() As E3SeriesFeatures()
    Function AuthenticateFeature(feature As E3SeriesFeatures) As Boolean
    Event FeatureAvailabilityChanged(availableFeatures As E3SeriesFeatures())
End Interface

' Service implementation
Public Class LicenseService
    Implements ILicenseService
    Implements IDisposable
    
    Private ReadOnly _licenseManager As E3SeriesLicenseManager
    Private ReadOnly _logger As ILogger
    Private _isInitialized As Boolean = False
    
    Public Event FeatureAvailabilityChanged(availableFeatures As E3SeriesFeatures()) Implements ILicenseService.FeatureAvailabilityChanged
    
    Public Sub New(logger As ILogger)
        _logger = logger ?? throw New ArgumentNullException(NameOf(logger))
        _licenseManager = New E3SeriesLicenseManager()
    End Sub
    
    Public Function Initialize() As Task(Of Boolean)
        Return Task.Run(Function()
            Try
                Dim initResult = _licenseManager.Init(createRegKeys:=True)
                If Not initResult.IsSuccess Then
                    _logger.Error($"License initialization failed: {initResult.Message}")
                    Return False
                End If
                
                Dim authResult = _licenseManager.AuthenticateAllFeatures()
                If authResult.IsSuccess Then
                    _isInitialized = True
                    _logger.Info($"License service initialized with features: {String.Join(", ", authResult.LicensedFeatures)}")
                    RaiseEvent FeatureAvailabilityChanged(GetAvailableFeatures())
                    Return True
                Else
                    _logger.Warn($"Some features failed authentication: {authResult.Message}")
                    _isInitialized = True  ' Partial success
                    RaiseEvent FeatureAvailabilityChanged(GetAvailableFeatures())
                    Return True
                End If
                
            Catch ex As Exception
                _logger.Error(ex, "Error during license service initialization")
                Return False
            End Try
        End Function)
    End Function
    
    Public Function IsFeatureAvailable(feature As E3SeriesFeatures) As Boolean Implements ILicenseService.IsFeatureAvailable
        If Not _isInitialized Then
            Return False
        End If
        
        Try
            Return _licenseManager.GetAvailableFeatures().Contains(feature)
        Catch ex As Exception
            _logger.Error(ex, $"Error checking feature availability: {feature}")
            Return False
        End Try
    End Function
    
    Public Function GetAvailableFeatures() As E3SeriesFeatures() Implements ILicenseService.GetAvailableFeatures
        If Not _isInitialized Then
            Return {}
        End If
        
        Try
            Return _licenseManager.GetAvailableFeatures()
        Catch ex As Exception
            _logger.Error(ex, "Error getting available features")
            Return {}
        End Try
    End Function
    
    Public Function AuthenticateFeature(feature As E3SeriesFeatures) As Boolean Implements ILicenseService.AuthenticateFeature
        If Not _isInitialized Then
            Return False
        End If
        
        Try
            Dim authResult = _licenseManager.AuthenticateFeatures(feature)
            If authResult.IsSuccess Then
                RaiseEvent FeatureAvailabilityChanged(GetAvailableFeatures())
                Return True
            Else
                _logger.Warn($"Feature authentication failed for {feature}: {authResult.Message}")
                Return False
            End If
        Catch ex As Exception
            _logger.Error(ex, $"Error authenticating feature: {feature}")
            Return False
        End Try
    End Function
    
    Public Sub Dispose() Implements IDisposable.Dispose
        Try
            _licenseManager?.TerminateLicense()
            _licenseManager?.Dispose()
            _logger.Info("License service disposed")
        Catch ex As Exception
            _logger.Error(ex, "Error during license service disposal")
        End Try
    End Sub
End Class

' DI Container Registration (using Microsoft.Extensions.DependencyInjection)
Public Module ServiceCollectionExtensions
    <Extension>
    Public Function AddLicensing(services As IServiceCollection) As IServiceCollection
        Return services.
            AddSingleton(Of ILicenseService, LicenseService)().
            AddHostedService(Of LicenseInitializationService)()
    End Function
End Module

' Hosted service for initialization
Public Class LicenseInitializationService
    Inherits BackgroundService
    
    Private ReadOnly _licenseService As ILicenseService
    Private ReadOnly _logger As ILogger(Of LicenseInitializationService)
    
    Public Sub New(licenseService As ILicenseService, logger As ILogger(Of LicenseInitializationService))
        _licenseService = licenseService
        _logger = logger
    End Sub
    
    Protected Overrides Async Function ExecuteAsync(stoppingToken As CancellationToken) As Task
        Try
            Dim success = Await CType(_licenseService, LicenseService).Initialize()
            If Not success Then
                _logger.Error("Failed to initialize licensing service")
                ' Could request application shutdown here
            End If
        Catch ex As Exception
            _logger.Error(ex, "Error during license service initialization")
        End Try
    End Function
End Class
```

### Configuration-Based Integration

```vb
' Configuration model
Public Class LicensingConfiguration
    Public Property ProductName As String = "E3.Series"
    Public Property Vendor As String = "zuken"
    Public Property EnableAutomaticRecovery As Boolean = True
    Public Property MonitoringIntervalSeconds As Integer = 300
    Public Property RequiredFeatures As String() = {}
    Public Property OptionalFeatures As String() = {}
    Public Property CreateRegistryKeys As Boolean = True
    Public Property EnableBorrowing As Boolean = True
    Public Property DefaultBorrowDays As Integer = 7
    Public Property LogLevel As String = "Information"
End Class

' Configurable license manager
Public Class ConfigurableLicenseManager
    Inherits E3SeriesLicenseManager
    
    Private ReadOnly _configuration As LicensingConfiguration
    Private ReadOnly _logger As ILogger
    
    Public Sub New(configuration As LicensingConfiguration, logger As ILogger)
        _configuration = configuration ?? throw New ArgumentNullException(NameOf(configuration))
        _logger = logger ?? throw New ArgumentNullException(NameOf(logger))
    End Sub
    
    Public Overrides ReadOnly Property ProductName As String
        Get
            Return _configuration.ProductName
        End Get
    End Property
    
    Protected Overrides ReadOnly Property Vendor As String
        Get
            Return _configuration.Vendor
        End Get
    End Property
    
    Public Function InitializeWithConfiguration() As Task(Of Result)
        Return Task.Run(Function()
            Try
                ' Initialize with configuration settings
                Dim initResult = MyBase.Init(_configuration.CreateRegistryKeys)
                If Not initResult.IsSuccess Then
                    _logger.Error($"License initialization failed: {initResult.Message}")
                    Return initResult
                End If
                
                ' Authenticate required features
                If _configuration.RequiredFeatures.Any() Then
                    Dim requiredFeatures = ParseFeatures(_configuration.RequiredFeatures)
                    Dim authResult = MyBase.AuthenticateFeatures(requiredFeatures)
                    
                    If Not authResult.IsSuccess Then
                        _logger.Error($"Required feature authentication failed: {authResult.Message}")
                        Return New Result(ResultState.Faulted, authResult.Message)
                    End If
                    
                    _logger.Info($"Required features authenticated: {String.Join(", ", authResult.LicensedFeatures)}")
                End If
                
                ' Try to authenticate optional features
                If _configuration.OptionalFeatures.Any() Then
                    Dim optionalFeatures = ParseFeatures(_configuration.OptionalFeatures)
                    Dim authResult = MyBase.AuthenticateFeatures(optionalFeatures)
                    
                    If authResult.IsSuccess Then
                        _logger.Info($"Optional features authenticated: {String.Join(", ", authResult.LicensedFeatures)}")
                    Else
                        _logger.Warn($"Optional feature authentication failed: {authResult.Message}")
                    End If
                End If
                
                Return New Result(ResultState.Success, "License initialization completed")
                
            Catch ex As Exception
                _logger.Error(ex, "Error during license initialization")
                Return New Result(ResultState.Faulted, ex.Message)
            End Try
        End Function)
    End Function
    
    Private Function ParseFeatures(featureNames As String()) As E3SeriesFeatures()
        Dim features As New List(Of E3SeriesFeatures)
        
        For Each featureName In featureNames
            If [Enum].TryParse(Of E3SeriesFeatures)(featureName, True, out Dim feature) Then
                features.Add(feature)
            Else
                _logger.Warn($"Unknown feature name in configuration: {featureName}")
            End If
        Next
        
        Return features.ToArray()
    End Function
End Class
```

## Testing and Development Patterns

### Mock License Manager for Testing

```vb
Public Class MockLicenseManager
    Inherits LicenseManager(Of E3SeriesFeatures)
    
    Private ReadOnly _availableFeatures As HashSet(Of E3SeriesFeatures)
    Private _initializeResult As Result = New Result(ResultState.Success)
    Private _authenticationResult As AuthFeaturesResult
    
    Public Sub New(ParamArray availableFeatures As E3SeriesFeatures())
        _availableFeatures = New HashSet(Of E3SeriesFeatures)(availableFeatures)
        
        ' Create successful authentication result
        _authenticationResult = New AuthFeaturesResult(
            ResultState.Success,
            availableFeatures.Select(Function(f) f.ToString()),
            Nothing,
            "Mock authentication successful"
        )
    End Sub
    
    ' Override properties for testing
    Protected Overrides ReadOnly Property Vendor As String
        Get
            Return "MockVendor"
        End Get
    End Property
    
    Public Overrides ReadOnly Property ProductName As String
        Get
            Return "MockProduct"
        End Get
    End Property
    
    ' Override initialization behavior
    Public Overrides Function Init(Optional createRegKeys As Boolean = False) As Result
        Return _initializeResult
    End Function
    
    ' Override authentication behavior
    Public Overrides Function AuthenticateFeatures(features As String()) As AuthFeaturesResult
        Dim availableFeatureStrings = _availableFeatures.Select(Function(f) f.ToString()).ToArray()
        Dim authenticatedFeatures = features.Where(Function(f) availableFeatureStrings.Contains(f)).ToArray()
        
        If authenticatedFeatures.Length = features.Length Then
            Return New AuthFeaturesResult(ResultState.Success, authenticatedFeatures)
        Else
            Dim failedFeatures = features.Except(authenticatedFeatures).ToArray()
            Return New AuthFeaturesResult(
                ResultState.Faulted,
                authenticatedFeatures,
                Nothing,
                $"Authentication failed for: {String.Join(", ", failedFeatures)}"
            )
        End If
    End Function
    
    ' Test configuration methods
    Public Sub SetInitializeResult(result As Result)
        _initializeResult = result
    End Sub
    
    Public Sub SetAvailableFeatures(ParamArray features As E3SeriesFeatures())
        _availableFeatures.Clear()
        For Each feature In features
            _availableFeatures.Add(feature)
        Next
    End Sub
    
    Public Sub SimulateFeatureLoss(feature As E3SeriesFeatures)
        _availableFeatures.Remove(feature)
    End Sub
    
    Public Sub SimulateFeatureRecovery(feature As E3SeriesFeatures)
        _availableFeatures.Add(feature)
    End Sub
End Class

' Unit test example
<TestClass>
Public Class LicenseManagerTests
    Private _mockLicenseManager As MockLicenseManager
    
    <TestInitialize>
    Public Sub Setup()
        _mockLicenseManager = New MockLicenseManager(
            E3SeriesFeatures.BaseFeature,
            E3SeriesFeatures.SchematicEditor
        )
    End Sub
    
    <TestMethod>
    Public Sub Init_WithValidConfiguration_ReturnsSuccess()
        ' Act
        Dim result = _mockLicenseManager.Init()
        
        ' Assert
        Assert.IsTrue(result.IsSuccess)
    End Sub
    
    <TestMethod>
    Public Sub AuthenticateFeatures_WithAvailableFeatures_ReturnsSuccess()
        ' Arrange
        _mockLicenseManager.Init()
        
        ' Act
        Dim result = _mockLicenseManager.AuthenticateFeatures(E3SeriesFeatures.BaseFeature)
        
        ' Assert
        Assert.IsTrue(result.IsSuccess)
        Assert.IsTrue(result.LicensedFeatures.Contains("BaseFeature"))
    End Sub
    
    <TestMethod>
    Public Sub AuthenticateFeatures_WithUnavailableFeatures_ReturnsFailure()
        ' Arrange
        _mockLicenseManager.Init()
        
        ' Act
        Dim result = _mockLicenseManager.AuthenticateFeatures(E3SeriesFeatures.ThreeDIntegration)
        
        ' Assert
        Assert.IsFalse(result.IsSuccess)
        Assert.AreEqual(0, result.LicensedFeatures.Count)
    End Sub
    
    <TestMethod>
    Public Sub SimulateFeatureLoss_UpdatesAvailability()
        ' Arrange
        _mockLicenseManager.Init()
        _mockLicenseManager.AuthenticateFeatures(E3SeriesFeatures.BaseFeature)
        
        ' Act
        _mockLicenseManager.SimulateFeatureLoss(E3SeriesFeatures.BaseFeature)
        Dim result = _mockLicenseManager.AuthenticateFeatures(E3SeriesFeatures.BaseFeature)
        
        ' Assert
        Assert.IsFalse(result.IsSuccess)
    End Sub
End Class
```

## Related Components

### Dependencies for Integration
- **Microsoft.Extensions.DependencyInjection**: For DI container integration
- **Microsoft.Extensions.Hosting**: For hosted service patterns
- **Microsoft.Extensions.Configuration**: For configuration-based setup
- **Microsoft.Extensions.Logging**: For structured logging
- **NUnit/MSTest**: For unit testing patterns

### Integration Points
- **Application Frameworks**: WinForms, WPF, Console, Service applications
- **Logging Systems**: NLog, Serilog, Microsoft.Extensions.Logging
- **Configuration Systems**: appsettings.json, environment variables, command line
- **Monitoring Systems**: Application Insights, custom telemetry
- **Error Handling**: Global exception handlers, user notification systems

---

**Previous:** [11 - Advanced Concepts](./Documentation-11-Advanced-Concepts.md) | **Next:** [13 - Sequence Diagrams](./Documentation-13-Sequence-Diagrams.md)
