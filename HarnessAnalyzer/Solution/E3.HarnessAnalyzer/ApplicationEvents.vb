Imports System.Collections.ObjectModel
Imports System.Exceptions
Imports Zuken.E3.HarnessAnalyzer.Settings

Namespace My

    ' The following events are available for MyApplication:
    ' 
    ' Startup: Raised when the application starts, before the startup form is created.
    ' Shutdown: Raised after all application forms are closed.  This event is not raised if the application terminates abnormally.
    ' UnhandledException: Raised if the application encounters an unhandled exception.
    ' StartupNextInstance: Raised when launching a single-instance application and the application is already active. 
    ' NetworkAvailabilityChanged: Raised when the network connection is connected or disconnected.
    Partial Friend Class MyApplication
        Inherits Microsoft.VisualBasic.ApplicationServices.WindowsFormsApplicationBase

        Private _initManager As New InitManager
        Private _exMan As UnHandledExceptionObserver

        Protected Overrides Function OnInitialize(commandLineArgs As ReadOnlyCollection(Of String)) As Boolean
            If MyBase.OnInitialize(commandLineArgs) Then
                Me.EnableVisualStyles = True
                Me.SaveMySettingsOnExit = True
                Me.ShutdownStyle = ShutdownMode.AfterMainFormCloses
                Return True
            End If
            Return False
        End Function

        Protected Overrides Function OnStartup(eventArgs As ApplicationServices.StartupEventArgs) As Boolean
            devDept.Eyeshot.Settings.Defaults.InitLicense([Shared].EYESHOT_KEY)
            If Not InitApplication() Then
                eventArgs.Cancel = True
                Environment.Exit(_initManager.CurrentError.ExitCode)
            End If
            Return MyBase.OnStartup(eventArgs)
        End Function

        Protected Overrides Sub OnShutdown()
            MyBase.OnShutdown()
            _exMan.Detach()

            If _initManager IsNot Nothing Then
                _initManager.Dispose()
            End If

            _initManager = Nothing
        End Sub

        Protected Overrides Sub OnCreateSplashScreen()
            MyBase.SplashScreen = New SplashScreen
        End Sub

        Protected Overrides Async Sub OnStartupNextInstance(eventArgs As ApplicationServices.StartupNextInstanceEventArgs)
            If _initManager.InitializeAndProcessCommandLine(eventArgs.CommandLine) AndAlso My.Application.MainForm IsNot Nothing Then
                Await My.Application.MainForm.OpenDocumentFromCommandLine()
            End If
        End Sub

        Protected Overrides Sub OnCreateMainForm()
            MyBase.MainForm = New MainForm
        End Sub

        Private Function InitApplication() As Boolean
            _exMan = UnHandledExceptionObserver.Attach ' Handles the unHandledException, but with more detailed information about it
            With _initManager
                If _initManager.InitAll() Then
                    Return True
                End If

#If DEBUG Or CONFIG = "Debug" Then
                Throw New Exception("Error initialize: " & _initManager.CurrentError.Message)
#Else
                .ShowCurrentErrorOrWarning()
#End If

                ' Always false in test mode (Init-failed), and also failed when not in test-mode and the error is not a warning
                If .IsInTestMode Then
                    Return False
                End If

                Return .CurrentError.IsWarning
            End With
        End Function

        ReadOnly Property InitManager As InitManager
            Get
                Return _initManager
            End Get
        End Property

        Public Shadows ReadOnly Property MainForm As MainForm
            Get
                Return CType(MyBase.MainForm, MainForm)
            End Get
        End Property


    End Class

End Namespace