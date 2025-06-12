Imports System.ComponentModel
Imports System.Exceptions.Issues
Imports System.Exceptions.Issues.Entries
Imports Microsoft.Win32
Imports Zuken.E3.HarnessAnalyzer.Project.Documents
Imports Zuken.E3.HarnessAnalyzer.Settings
Imports Zuken.E3.HarnessAnalyzer.Shared
Imports Zuken.E3.Lib.IO.Files.Hcv
Imports Zuken.E3.Lib.Licensing
Imports Zuken.E3.Lib.Licensing.LicenseManagerBase

Public Class InitManager
    Implements IDisposable

    Private _commandLineArgsProcessor As CommandLineArgsProcessor = Nothing
    Private _currentError As New InitError
    Private _errorManager As New ErrorManager(Me)
    Private _hasErrorOrWarning As Boolean = False
    Private _licMan As New [Shared].LicenseManager
    Private _lastExpirationDate As Date
    Private _hasExpirationDate As Boolean = False
    Private _license_initialized As Boolean = False
    Private _licensedFeatures As ApplicationFeature = 0
    Private _disposedValue As Boolean

    Public Sub New()
    End Sub

    Friend Function InitAll() As Boolean
        InitExceptionReporting() ' init exception reporting first that the report manager-interface is pereared well (but without license-variables which will re added after licensemanager was licensed)
        Try
            If InitializeAndProcessCommandLine() Then
                Dim result As ErrorCodeResult = SwitchDaimler3DLicense()
                If result.IsSuccess AndAlso InitializeLicensing() Then
                    Dim settings_result As Settings.GeneralSettings.GeneralSettingsResult = Settings.GeneralSettings.Current ' this first call loads the settings if available
                    If settings_result.IsFaulted Then
                        ErrorManager.ShowMsgBoxErrorOrWriteConsole(settings_result.Message)
                        Return False
                    End If

                    InitCurrentCultureFromSettings(settings_result.GeneralSettings)
                    Return True
                End If
            End If
            Return False
        Finally
            InitExceptionReportingEnvLicVars()
        End Try
    End Function

    Private Sub InitCurrentCultureFromSettings(settings As GeneralSettings)
        Dim specCulture As Globalization.CultureInfo = Globalization.CultureInfo.CreateSpecificCulture(settings.UILanguage)
        Globalization.CultureInfo.DefaultThreadCurrentCulture = specCulture
        Globalization.CultureInfo.DefaultThreadCurrentUICulture = specCulture
        Globalization.CultureInfo.CurrentCulture = specCulture
        Globalization.CultureInfo.CurrentUICulture = specCulture
    End Sub

    Private Sub InitExceptionReporting()
        'HINT: this is the setting for creation of the "snapshot"-package in the exception handling dialog
        Exceptions.Issues.ExceptionReportSetting.Current.Entries.OfType(Of Exceptions.Issues.Entries.RegistryEntry).Single.SoftwareRegSubKey = [Shared].LicenseManager.COMPANY_NAME
        InitExceptionReportingEnvLicVars()
    End Sub

    Private Sub InitExceptionReportingEnvLicVars()
        If _licMan IsNot Nothing Then
            Dim envEntry As EnvironmentEntry = ExceptionReportSetting.Current.Entries.OfType(Of EnvironmentEntry).Single
            envEntry.VariableNames.AddRange(_licMan.EnvironmentVariables)
        End If
    End Sub

    Private Function SwitchDaimler3DLicense() As ErrorCodeResult
        If My.Settings.Check3DContent Then
            Dim result As ErrorCodeResult = _licMan.Init(False)
            If result.IsSuccess Then
                Dim features As List(Of String) = _licMan.AvailableFeatures.ToList
                If _commandLineArgsProcessor.FileArgStatus = CommandLineArgsProcessor.FileState.HasHCV Or _commandLineArgsProcessor.FileArgStatus = CommandLineArgsProcessor.FileState.HasXHCV Then
                    Dim hcvFile As BaseHcvFile = BaseHcvFile.Create(_commandLineArgsProcessor.File, Project.Common.ZIP_CACHING_METHOD)
                    If HarnessAnalyzerProject.AnalyseContent(hcvFile, HcvDocument.ContentAnalyseType.HasKBLZData) OrElse HarnessAnalyzerProject.AnalyseContent(hcvFile, HcvDocument.ContentAnalyseType.HasJTData) Then
                        features.Add(ApplicationFeature.E3HarnessAnalyzer3D.ToString)
                    Else
                        features.Remove(ApplicationFeature.E3HarnessAnalyzer3D.ToString)
                    End If
                Else
                    features.Remove(ApplicationFeature.E3HarnessAnalyzer3D.ToString)
                End If

                _licMan.SetAvailableFeatures(features.ToArray)
            End If
            Return result
        End If
        Return New ErrorCodeResult(ResultState.Success)
    End Function

    Friend Function InitializeAndProcessCommandLine(Optional commandLineArgs As IReadOnlyCollection(Of String) = Nothing) As Boolean
        Try
            _commandLineArgsProcessor = New CommandLineArgsProcessor
            _commandLineArgsProcessor.Process(commandLineArgs)
        Catch ex As Exception
#If DEBUG Or CONFIG = "Debug" Then
            Throw New Exception(ex.Message, ex)
#Else
            _currentError = New InitError(ErrorCodes.ERROR_UNKNOWN, ex.Message)
#End If
            Return False
        End Try

        Return True
    End Function

    Friend Function InitializeLicensing() As Boolean
        If _commandLineArgsProcessor Is Nothing Then
            Dim errMsg As String = ErrorStrings.InitMng_InitCmdLine_Msg
#If DEBUG Or CONFIG = "Debug" Then
            Throw New Exception(errMsg)
#Else
            _currentError = New InitError(ErrorCodes.ERROR_UNKNOWN, errMsg, False)
            Return False
#End If
        End If

        _license_initialized = False

        Dim res As ErrorCodeResult = _licMan.Init()
        If res.IsFaulted Then
            ' There was an error on initializing something before in the license manager...
            SetError(res.ErrorCode, res.Message)
            Return False
        Else
            _license_initialized = True
        End If

        ' Process and extract data from command line arguments
        Dim splashScreen As SplashScreen = DirectCast(My.Application.SplashScreen, SplashScreen)
        splashScreen.TxtBorrowedLicense = My.Resources.LoggingStrings.InitMng_InitLic

        If AuthenticateLicense() Then
            Return SetBorrowedExpireDate(splashScreen)
        Else
            Return False
        End If

        Return True
    End Function

    Friend Function SetBorrowedExpireDate(splashScreen As SplashScreen) As Boolean
#If DEBUG Or CONFIG = "Debug" Then
        splashScreen.TxtBorrowedLicense = "DEBUG-MODE: <IGNORING BORROW>"
#Else
        With _licMan
            ' Check if day's are left or if the currentDateDay is the same as the day when it expires (the subtract counts also the time, but not the lmborrow)
            Dim res As DateExpiredResult = HasBorrowedLicenseExpired()
            If res.IsFaulted Then
                ' Couldn't read the Date and there was an error while reading
                If Not _license_initialized Then
                    splashScreen.TxtBorrowedLicense = String.Empty
                    SetError(-1, String.Format(ErrorStrings.InitMng_ErrorReadLic_Msg, res.Message), True)
                    Return False
                Else
                    ' Couldn't read the date. This means there is no license borrowed: Ignore this, only show the borrow-date
                    splashScreen.TxtBorrowedLicense = String.Empty
                End If
            Else
                If res.Expired Then
                    splashScreen.TxtBorrowedLicense = String.Format(String.Format(DialogStrings.SplashScrren_BorrowLicExpired_Label))
                ElseIf Not res.Expired AndAlso res.HasDate Then
                    splashScreen.TxtBorrowedLicense = String.Format(String.Format(DialogStrings.SplashScreen_BorrowLicValidUntil_Label, _lastExpirationDate.ToShortDateString))
                Else
                    ' Couldn't read the date. This means there is no license borrowed: Ignore this, only show the borrow-date
                    splashScreen.TxtBorrowedLicense = String.Empty
                End If
            End If
        End With
#End If
        Return True
    End Function

    Friend Sub ShowCurrentErrorOrWarning()
        With _currentError
            If .IsWarning Then
                _errorManager.ShowMsgBoxExclamtionOrWriteConsole(.Message, CType(.ExitCode, ErrorCodes))
            Else
                _errorManager.ShowMsgBoxErrorOrWriteConsole(.Message, CType(.ExitCode, ErrorCodes))
            End If
        End With
    End Sub

    Private Function AuthenticateLicense() As Boolean
        With _licMan
            Dim res As ErrorCodeResult = AuthenticateE3HarnessAnalyzer()
            If res.IsFaulted Then
                Dim resEx As DateExpiredResult = HasBorrowedLicenseExpired()
                If resEx.IsSuccess Then
                    If resEx.HasDate AndAlso resEx.Expired Then ' check if the error is a possible expiration of borrowed license date
                        SetError(res.ErrorCode, String.Format(ErrorStrings.InitMng_BorrowedLicExpired_Msg, vbCrLf, res.Message))
                    ElseIf Not resEx.HasDate Then
                        SetError(-1, "Coudn't read expiration date!")
                    Else
                        SetError(res.ErrorCode, res.Message)
                    End If
                ElseIf res.ErrorCode = LicensingErrorCodes.VENDOR_DEAMON_DOWN Then
                    SetError(-97, res.Message)
                Else
                    SetError(-1, resEx.Message)
                End If
                Return False
            End If
        End With

        Return True
    End Function

    Friend Function AuthenticateE3HarnessAnalyzer() As ErrorCodeResult
        If _license_initialized Then
#If DEBUG Or CONFIG = "Debug" Then
            _licensedFeatures = Nothing
            For Each feature As ApplicationFeature In [Enum](Of ApplicationFeature).GetValues
                _licensedFeatures = _licensedFeatures Or feature
            Next
            Return New ErrorCodeResult(ResultState.Success, String.Empty, Diagnostics.ErrorCodes.NONE)
#Else
            Dim res As AuthFeaturesResult = _licMan.AuthenticateAvailableFeatures
            If _licMan.AuthenticatedFeatures.Contains(ApplicationFeature.E3HarnessAnalyzer.ToString) Then
                For Each ftName As String In _licMan.AuthenticatedFeatures
                    Dim ft As ApplicationFeature = Nothing
                    If [Enum].TryParse(Of ApplicationFeature)(ftName, ft) Then
                        _licensedFeatures = _licensedFeatures Or ft
                    End If
                Next
            End If

            For Each tpl As Tuple(Of String, [Error], LicensingErrorCodes) In res.FailedFeatures
                Console.WriteLine($"Failed to license feature ""{tpl.Item1}"": " + vbCrLf + tpl.Item2.Message)
                Console.Error.WriteLine($"Failed to license feature ""{tpl.Item1}"": " + vbCrLf + tpl.Item2.Message)
            Next
            Return res
#End If
        End If

        Return New ErrorCodeResult(ResultState.Faulted, "Licensing not initialized!", ErrorCodes.ERROR_ACCESS_DENIED, LicensingErrorCodes.NOT_INITIALIZED)
    End Function

    Private Function HasBorrowedLicenseExpired() As DateExpiredResult
        With _licMan
            Dim res As DateResult = _licMan.Borrow.GetExpirationDate
            If res.IsSuccess Then
                _hasExpirationDate = res.Date <> Date.MinValue
                If _hasExpirationDate Then
                    _lastExpirationDate = res.Date
#If DEBUG Or CONFIG = "Debug" Then
                    Return New DateExpiredResult(ResultState.Success) ' in debug borrow-date is never beeing expiring
#End If
                    Dim subtractedDate As TimeSpan = _lastExpirationDate.Subtract(GetNowDate)
                    ' Check if day's are left or if the currentDateDay is the same as the day when it expires (the subtract counts also the time, but not the lmborrow)
                    If Not subtractedDate.TotalDays > 0 OrElse IsEqualDate(_lastExpirationDate, GetNowDate) Then
                        Return New DateExpiredResult(ResultState.Success, True, True)
                    End If
                    Return New DateExpiredResult(res.Result, res.Message, False, True)
                End If
            End If
            Return New DateExpiredResult(res.Result, res.Message)
        End With
    End Function

    Private Shared Function GetNowDate() As Date
        'HINT: this method is a hook to make testing different dates possible
        Return Now
    End Function

    Private Function IsEqualDate(date1 As Date, date2 As Date) As Boolean
        Return (date1.Year = date2.Year) AndAlso (date1.Month = date2.Month) AndAlso (date1.Day = date2.Day)
    End Function

    Private Sub SetError(code As Integer, message As String, Optional isWarning As Boolean = False)
        _hasErrorOrWarning = True
        _currentError = New InitError(code, message, isWarning)
    End Sub

    Friend Sub TerminateLicense()
        If _license_initialized Then
            If _licMan.TerminateLicense Then
                _license_initialized = False
            End If
        End If
    End Sub

    ReadOnly Property CommandLine As CommandLineArgsProcessor
        Get
            Return _commandLineArgsProcessor
        End Get
    End Property

    ReadOnly Property CurrentError As InitError
        Get
            Return _currentError
        End Get
    End Property

    ReadOnly Property ErrorManager As ErrorManager
        Get
            Return _errorManager
        End Get
    End Property

    ReadOnly Property LicensedFeatures As [Shared].ApplicationFeature
        Get
            Return _licensedFeatures
        End Get
    End Property

    ReadOnly Property HasErrorOrWarning As Boolean
        Get
            Return _hasErrorOrWarning
        End Get
    End Property

    ReadOnly Property IsInTestMode As Boolean
        Get
            If (_commandLineArgsProcessor IsNot Nothing) Then Return _commandLineArgsProcessor.IsInTestMode

            Return False
        End Get
    End Property

    ReadOnly Property LicenseManager As [Shared].LicenseManager
        Get
            Return _licMan
        End Get
    End Property

    Public Class InitError

        Private _isWarning As Boolean = False

        Property ExitCode As Integer = 0
        Property Message As String = ""

        Public Sub New()

        End Sub

        Public Sub New(exitCode As Integer, errorMessage As String)
            Me.ExitCode = exitCode
            Me.Message = errorMessage
        End Sub

        Public Sub New(exitCode As Integer, errorMessage As String, isWarning As Boolean)
            Me.New(exitCode, errorMessage)

            _isWarning = isWarning
        End Sub

        ReadOnly Property IsWarning As Boolean
            Get
                Return _isWarning
            End Get
        End Property

    End Class

    Private Class DateExpiredResult
        Inherits LicenseErrorCodeResult

        Public Sub New(result As ResultState, Optional expired As Boolean = False, Optional hasDate As Boolean = False)
            MyBase.New(result, LicensingErrorCodes.DATE_EXPIRED)
            Me.Expired = expired
            Me.HasDate = hasDate
        End Sub

        Public Sub New(exception As Exception, Optional expired As Boolean = False, Optional hasDate As Boolean = False)
            MyBase.New(exception, LicensingErrorCodes.DATE_EXPIRED)
            Me.Expired = expired
            Me.HasDate = hasDate
        End Sub

        Public Sub New(result As ResultState, message As String, Optional expired As Boolean = False, Optional hasDate As Boolean = False)
            MyBase.New(result, message, LicensingErrorCodes.DATE_EXPIRED)
            Me.Expired = expired
            Me.HasDate = hasDate
        End Sub

        Protected Sub New()
        End Sub

        Protected Friend Sub New(other As IResult)
            MyBase.New(other)
            Me.Expired = (TryCast(other, DateExpiredResult)?.Expired).GetValueOrDefault
            Me.HasDate = (TryCast(other, DateExpiredResult)?.HasDate).GetValueOrDefault
            Me.ErrorCode = LicensingErrorCodes.DATE_EXPIRED
        End Sub

        ReadOnly Property Expired As Boolean
        ReadOnly Property HasDate As Boolean

    End Class

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not _disposedValue Then
            If disposing Then
                If _license_initialized Then
                    TerminateLicense() ' maybe that is not fully needed because license is already terminated in Dispose-method, but we call it to reset "_license_initialized" field
                End If

                If _licMan IsNot Nothing Then
                    _licMan.Dispose()
                End If
            End If

            _commandLineArgsProcessor = Nothing
            _currentError = Nothing
            _errorManager = Nothing
            _licMan = Nothing
            _lastExpirationDate = Nothing
            _license_initialized = False
            _licensedFeatures = Nothing
            _disposedValue = True
        End If
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        ' Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in der Methode "Dispose(disposing As Boolean)" ein.
        Dispose(disposing:=True)
        GC.SuppressFinalize(Me)
    End Sub

End Class