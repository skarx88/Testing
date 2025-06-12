Public Class ErrorManager

    Private _initManager As InitManager

    Public Enum ErrMngMsgBoxType
        Exclamation = 48
        Critical = 16
    End Enum

    Public Sub New(initManager As InitManager)
        _initManager = initManager
    End Sub

    ''' <summary>
    ''' Use this command, which correctly flushes all console output from the stream (errors) that were written before...
    ''' </summary>
    ''' <param name="errCode"></param>
    ''' <remarks></remarks>
    Public Sub ExitAppWithErrorCode(errCode As Integer)
        If _initManager IsNot Nothing AndAlso _initManager.LicenseManager IsNot Nothing Then
            _initManager.TerminateLicense()
        End If
        Console.Error.Flush()
        Environment.Exit(errCode)
    End Sub

    Public Sub ShowMsgBoxExceptionOrWriteConsole(addTextToMessage As String, ex As Exception, Optional errorCode As ErrorCodes = ErrorCodes.ERROR_UNKNOWN)
        Dim msg As String = String.Empty

        ' If in Testmode then write all exceptionmessages that we can get (Console)
        If (_initManager.IsInTestMode) Then
            msg = addTextToMessage & GetAllInnerExceptionMessages(ex)
        Else
            ' If not test-mode then show only the message of the highest exception
            msg = addTextToMessage + ex.Message
        End If

        ShowMsgBoxErrorOrWriteConsole(msg, errorCode, ex.Source & vbCrLf & If(ex.TargetSite IsNot Nothing, ex.TargetSite.Name, "") & vbCrLf & ex.StackTrace)
    End Sub

    Public Sub ShowMsgBoxErrorOrWriteConsole(errorMsg As String, Optional errorCode As ErrorCodes = ErrorCodes.ERROR_UNKNOWN, Optional stackTrace As String = Nothing)
        If stackTrace Is Nothing Then
            WriteConsoleErrorOrMsgBoxInternal(errorMsg, _initManager.IsInTestMode, MessageBoxIcon.Error, errorCode)
        Else
            WriteConsoleErrorOrMsgBoxInternal(errorMsg, _initManager.IsInTestMode, MessageBoxIcon.Error, errorCode, stackTrace)
        End If
    End Sub

    Public Sub ShowMsgBoxExclamtionOrWriteConsole(errorMsg As String, Optional errorCode As ErrorCodes = ErrorCodes.ERROR_UNKNOWN)
        WriteConsoleErrorOrMsgBoxInternal(errorMsg, _initManager.IsInTestMode, MessageBoxIcon.Error, errorCode)
    End Sub

    Public Sub ShowMsgBoxWarningOrWriteConsole(warningMsg As String, Optional errorCode As ErrorCodes = ErrorCodes.ERROR_UNKNOWN)
        WriteConsoleErrorOrMsgBoxInternal(warningMsg, False, MessageBoxIcon.Warning, errorCode)
    End Sub

    Public Sub ShowMsgBoxInformationOrWriteConsole(infoMsg As String)
        WriteConsoleErrorOrMsgBoxInternal(infoMsg, False, MessageBoxIcon.Information, ErrorCodes.NONE)
    End Sub

    Public Sub WriteConsoleError(msg As String)
        Console.WriteLine(msg)
        Console.Error.WriteLine(msg)
    End Sub

    Private Sub GetAllExceptionMessagesInternal(ex As Exception, ByRef result As System.Text.StringBuilder)
        If (result Is Nothing) Then result = New System.Text.StringBuilder
        result.AppendLine(ex.Message)

        If (ex.InnerException IsNot Nothing) Then
            result.Append("InnerException: ")

            GetAllExceptionMessagesInternal(ex.InnerException, result)
        End If
    End Sub

    Private Function GetAllInnerExceptionMessages(ex As Exception) As String
        Dim resultMessage As New System.Text.StringBuilder

        GetAllExceptionMessagesInternal(ex, resultMessage)

        Return resultMessage.ToString
    End Function

    Public Sub ShowMessageBoxOnParent(parent As Form, message As String, iconType As MessageBoxIcon, Optional debugInformation As String = Nothing)
        Dim messageBoxShow As Action =
            Sub()
                If Not String.IsNullOrEmpty(debugInformation) Then
                    Using exFrm As New ExceptionForm
                        exFrm.ShowDialog(parent, message, debugInformation)
                    End Using
                Else
                    MessageBoxEx.Show(parent, message, My.Application.Info.Title, MessageBoxButtons.OK, iconType)
                End If
            End Sub

        If (parent IsNot Nothing) Then
            parent.InvokeOrDefault(Sub() messageBoxShow.Invoke)
        Else
            messageBoxShow.Invoke()
        End If
    End Sub

    Private Sub WriteConsoleErrorOrMsgBoxInternal(errorMsg As String, exitapplication As Boolean, msgBoxIconType As MessageBoxIcon, errorCode As ErrorCodes, stackTrace As String)
        If _initManager.IsInTestMode Then
            WriteConsoleError(errorMsg)
        Else
            Dim debugInfo As String = String.Join(vbCrLf,
                        String.Format("Product: {0}", My.Application.Info.ProductName),
                        String.Format("Operating system: {0}", E3.Lib.DotNet.Expansions.Devices.My.Computer.Info.OSFullName),
                        String.Format("Version: {0}", My.Application.Info.Version),
                        String.Format("Language: {0}", Globalization.CultureInfo.CurrentCulture.EnglishName),
                        String.Format("UI Language: {0}", Globalization.CultureInfo.CurrentUICulture.EnglishName), vbCrLf,
                        stackTrace)

            ShowMessageBoxOnParent(My.Application.SplashScreen, errorMsg, msgBoxIconType, debugInfo)
        End If

        If exitapplication Then
            ExitAppWithErrorCode(errorCode)
        End If
    End Sub

    Private Sub WriteConsoleErrorOrMsgBoxInternal(errorMsg As String, exitapplication As Boolean, msgBoxIconType As MessageBoxIcon, errorCode As ErrorCodes)
        If _initManager.IsInTestMode Then
            If msgBoxIconType.HasFlag(MessageBoxIcon.Information) OrElse msgBoxIconType.HasFlag(MessageBoxIcon.Warning) Then
                Console.WriteLine(errorMsg)
            Else
                WriteConsoleError(errorMsg)
            End If
        Else
            Dim owner As Form = Nothing
            If My.Application.MainForm IsNot Nothing OrElse My.Application.SplashScreen IsNot Nothing Then
                If (My.Application.SplashScreen?.Visible).GetValueOrDefault Then
                    owner = My.Application.SplashScreen
                Else
                    owner = My.Application.MainForm
                End If
            End If
            ShowMessageBoxOnParent(owner, errorMsg, msgBoxIconType)
        End If

        If exitapplication Then
            ExitAppWithErrorCode(errorCode)
        End If
    End Sub

End Class

