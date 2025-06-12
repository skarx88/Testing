Imports System.Text.RegularExpressions
Imports System.IO
Imports System.Reflection
Imports System.Threading
Imports Zuken.E3.HarnessAnalyzer.Shared

<Obfuscation(Feature:="renaming", ApplyToMembers:=False)>
Public Class FlmBorrowHandler

    Private _availableFeatures As ApplicationFeature = 0
    Private _vendor As String
    Private _lastError As String = ""
    Private _lastWarning As String = ""

    Private Const LMBORROW As String = "lmborrow.exe"
    Private Const LMSTAT As String = "lmstat.exe"
    Private Const BORROW_HELPER As String = "BorrowHelper.exe"
    Private Const FEATURE_COL_NAME As String = "Feature"
    Private Const VENDOR_COL_NAME As String = "Vendor"

    Sub New(Vendor As String)
        _vendor = Vendor
    End Sub

    ReadOnly Property GetLastError As String
        Get
            Return _lastError
        End Get
    End Property

    ReadOnly Property GetLastWarning As String
        Get
            Return _lastWarning
        End Get
    End Property

    Private Function GetPlatformPath(fileName As String) As String
        If Environment.Is64BitProcess Then
            Return Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "x64", fileName)
        Else
            Return Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "x86", fileName)
        End If
    End Function

    Private Function ExecuteProcess(Executable As String, Arguments As String) As LmResult
        Dim filePath As String = GetPlatformPath(Executable)
        Dim lmProcess As New Process
        With lmProcess.StartInfo
            .UseShellExecute = False
            .FileName = filePath
            .CreateNoWindow = True
            .RedirectStandardOutput = True
            .Arguments = Arguments
        End With
        Try
            If (SigningChecker.IsSigned(filePath)) Then
                If (lmProcess.Start()) Then
                    'HACK: the WaitForExit(x) on the process does not work properly, no matter in which sequence or way the call sequence is.
                    'it blocks if the amout of data is bigger see Inet and stakc overflow thread on this topic
                    'this is one proposal but not clear if it works in all circumstances.
                    Dim token As CancellationToken
                    Dim stdOut As String = ""
                    Dim task As Task = System.Threading.Tasks.Task.Factory.StartNew(
                        Sub()
                            stdOut = lmProcess.StandardOutput.ReadToEnd()
                            lmProcess.WaitForExit()
                        End Sub, token)

                    If (task.Wait(60000, token)) Then
                        Return New LmResult(lmProcess.ExitCode, stdOut)
                    Else
                        Return New LmResult(-1, String.Format("Execution of {0} failed, timeout occurred!", Executable))
                    End If
                Else
                    Return New LmResult(-2, String.Format("Start of {0} failed!", filePath))
                End If
            Else
                Throw New Exception("Assembly is not signed!")
            End If

        Catch ex As Exception
            Throw New Exception(String.Format("Execution of {0} failed!", filePath), ex)
        Finally
            lmProcess.Dispose()
        End Try
    End Function

    Public Function GetExpirationDate(ByRef expirationDate As Date) As Boolean
        _lastError = ""
        Dim processResult As LmResult = ExecuteProcess(LMBORROW, "-status")
        If (processResult.ErrorCode = 0) Then
            Dim a As String() = processResult.Result.Split(New Char() {CChar(vbCrLf)}, StringSplitOptions.RemoveEmptyEntries)
            For Each s As String In a
                If s.Trim(CChar(vbLf)).StartsWith(_vendor) Then 'find vendor start line
                    Dim s1 As String() = s.Trim(CChar(vbLf)).Split(New Char() {" "c}, StringSplitOptions.RemoveEmptyEntries)
                    If (s1.Length >= 3) Then 'beware time after date
                        If (s1(0) = _vendor) Then 'exact check
                            If (s1(1) = ApplicationFeature.E3HarnessAnalyzer.ToString) Then
                                Date.TryParse(s1(2), expirationDate)
                                Return True
                            End If
                        End If
                    End If
                End If
            Next
        Else
            _lastError = processResult.Result
        End If
        Return False
    End Function

    <Obfuscation(Feature:="renaming")>
    Public Function SetAvailableFeaturesFromServerLicense() As Boolean
        Dim processResult As LmResult = ExecuteProcess(LMSTAT, "-i")
        If (processResult.ErrorCode = 0) Then
            Dim state As Integer = 0
            Dim indexFeature As Integer
            Dim indexVendor As Integer
            Dim reader As New StringReader(processResult.Result)
            Dim line As String

            Do
                line = reader.ReadLine
                If Not String.IsNullOrWhiteSpace(line) Then
                    Dim tokens As List(Of String) = line.Split(" ".ToCharArray, StringSplitOptions.RemoveEmptyEntries).ToList

                    Select Case state
                        Case 0
                            If (tokens.Count >= 5) Then
                                If tokens.Contains(FEATURE_COL_NAME) AndAlso tokens.Contains(VENDOR_COL_NAME) Then
                                    indexFeature = tokens.IndexOf(FEATURE_COL_NAME)
                                    indexVendor = tokens.IndexOf(VENDOR_COL_NAME)
                                    state = 1
                                End If
                            End If

                        Case 1
                            'discard underscores
                            state = 2

                        Case 2
                            If (tokens.Count >= 5) Then
                                If tokens.Contains(FEATURE_COL_NAME) AndAlso tokens.Contains(VENDOR_COL_NAME) Then
                                    indexFeature = tokens.IndexOf(FEATURE_COL_NAME)
                                    indexVendor = tokens.IndexOf(VENDOR_COL_NAME)
                                    state = 1
                                ElseIf indexFeature <> -1 AndAlso indexVendor <> -1 Then
                                    Dim featureName As String = tokens(indexFeature).Trim
                                    If Not String.IsNullOrWhiteSpace(featureName) Then
                                        Dim appFeature As ApplicationFeature = 0
                                        Dim vendor As String = tokens(indexVendor).Trim
                                        If (vendor = _vendor) AndAlso ([Enum].TryParse(featureName, appFeature)) Then
                                            _availableFeatures = _availableFeatures Or appFeature
                                        End If
                                    End If
                                End If
                            Else
                                state = 0
                            End If
                    End Select
                End If
            Loop Until line Is Nothing

            If (_availableFeatures = 0) Then
                _lastError = "Could not retrieve features from server!"
                Return False
            Else
                Return True
            End If
        Else
            _lastError = processResult.Result
            Return False
        End If
    End Function

    Public Function BorrowLicense(expirationDate As Date) As Boolean
        _lastError = ""
        _lastWarning = ""
        Dim res As Boolean = False
        Dim processResult As LmResult = ExecuteProcess(LMBORROW, String.Format("{0} {1}", _vendor, expirationDate.ToString("dd-MMM-yyyy", System.Globalization.CultureInfo.InvariantCulture)))
        If (processResult.ErrorCode = 0) Then

            Dim lmBorrowProcess As New Process
            With lmBorrowProcess.StartInfo
                .UseShellExecute = False
                .FileName = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), BORROW_HELPER)
                .CreateNoWindow = True
                .RedirectStandardOutput = True
                .RedirectStandardError = True
                .Arguments = CInt(_availableFeatures).ToString
            End With

            Try
                If (lmBorrowProcess.Start()) Then
                    lmBorrowProcess.WaitForExit()
                    Dim Output As String = lmBorrowProcess.StandardOutput.ReadToEnd()
                    If lmBorrowProcess.ExitCode <> 0 AndAlso String.IsNullOrEmpty(Output) Then
                        Output = lmBorrowProcess.StandardError.ReadToEnd
                    End If
                    processResult = New LmResult(lmBorrowProcess.ExitCode, Output)
                    If (processResult.ErrorCode = 0) OrElse (processResult.ErrorCode > 500) Then
                        ExecuteProcess(LMBORROW, "-clear")
                        res = True

                        If (processResult.ErrorCode <> 0) Then
                            _lastWarning = RemoveLicFilePathInfo(processResult.Result)
                        End If
                    Else
                        _lastError = RemoveLicFilePathInfo(processResult.Result)
                    End If
                Else
                    Throw New Exception(String.Format("Execution of {0} failed!", BORROW_HELPER))
                End If
            Catch ex As Exception
                Throw New Exception(String.Format("Execution of {0} failed!", BORROW_HELPER), ex)
            End Try
        Else
            _lastError = processResult.Result
        End If
        Return res
    End Function

    Private Function GetLmServerList(ByVal ServerList As List(Of String)) As Boolean
        _lastError = ""
        ServerList.Clear()
        Dim processResult As LmResult = ExecuteProcess(LMSTAT, "")
        If (processResult.ErrorCode = 0) Then
            Dim r As New Regex("License server status:\s*(?<port>\d+)@(?<server>\S+)")
            For Each m As Match In r.Matches(processResult.Result)
                ServerList.Add(m.Result("${port}") & "@" & m.Result("${server}"))
            Next
            Return True
        Else
            _lastError = processResult.Result
            Return False
        End If
    End Function

    Public Function ReturnLicense() As Boolean
        Dim ServerList As New List(Of String)
        If (GetLmServerList(ServerList)) Then
            If (ServerList.Count = 0) Then
                _lastError = "No license servers accessible!"
                Return False
            End If
            Dim processResult As New LmResult
            For Each server As String In ServerList
                ReturnFeatureLicenses(server)
                _lastError = ""
                processResult = ExecuteProcess(LMBORROW, String.Format("-return -c ""{0}"" {1}", server, ApplicationFeature.E3HarnessAnalyzer.ToString))
                If (processResult.ErrorCode = 0) Then
                    Return True
                End If
            Next
            _lastError = processResult.Result
            Return False
        Else
            Return False
        End If
    End Function

    Private Sub ReturnFeatureLicenses(server As String)
        For Each appFeature As ApplicationFeature In [Enum].GetValues(GetType(ApplicationFeature))
            If (appFeature <> ApplicationFeature.E3HarnessAnalyzer) Then
                ExecuteProcess(LMBORROW, String.Format("-return -c ""{0}"" {1}", server, [Enum].GetName(GetType(ApplicationFeature), appFeature)))
            End If
        Next
    End Sub

    Public Function PurgeUnusedLicenseEntries() As Boolean
        _lastError = ""
        Dim processResult As LmResult = ExecuteProcess(LMBORROW, "-purge")
        If (processResult.ErrorCode = 0) Then
            Return True
        Else
            _lastError = processResult.Result
            Return False
        End If
    End Function

    Private Function RemoveLicFilePathInfo(errString As String) As String
        Dim r As New Regex("(License path.*)", RegexOptions.Multiline)
        Return r.Replace(errString, "")
    End Function

    Private Class LmResult
        Property Result As String = ""
        Property ErrorCode As Integer = 0

        Sub New()
        End Sub
        Sub New(errorCode As Integer, result As String)
            Me.ErrorCode = errorCode
            Me.Result = result
        End Sub
    End Class

End Class
