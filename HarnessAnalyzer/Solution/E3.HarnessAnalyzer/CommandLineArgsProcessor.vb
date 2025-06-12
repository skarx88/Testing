Imports System.IO

Public Class CommandLineArgsProcessor

    <Flags()>
    Private Enum CommandLineSwitches
        NoSwitch = 0
        BuildServerTest = 1
    End Enum

    Public Enum FileState
        NoFile = 0
        HasAnotherFile = 1
        HasDSI = 2
        HasHCV = 3
        HasKBL = 4
        HasNonExistingFile = 5
        HasXHCV = 6
        HasPLMXML = 7
    End Enum

    Private _file As String = String.Empty
    Private _fileArgStatus As FileState = FileState.NoFile
    Private _isInTestMode As Boolean = False
    Private _isProcessed As Boolean = False

    Friend Const COMMANDLINESWITCH_CHAR As Char = "/"c

    Public Sub New()
    End Sub

    Public Sub Process(commandLineArgs As IReadOnlyCollection(Of String))
        Dim collectedCmdSwitches As CommandLineSwitches = CommandLineSwitches.NoSwitch

        _file = String.Empty
        _fileArgStatus = FileState.NoFile

        If commandLineArgs Is Nothing Then
            commandLineArgs = My.Application.CommandLineArgs
        End If

        For Each arg As String In commandLineArgs
            If arg.StartsWith(COMMANDLINESWITCH_CHAR) Then
                Dim currentSwitch As CommandLineSwitches = TryGetSwitch(arg)

                If (currentSwitch <> CommandLineSwitches.NoSwitch) Then
                    collectedCmdSwitches = CType(collectedCmdSwitches + currentSwitch, CommandLineSwitches)
                End If
            ElseIf Not String.IsNullOrEmpty(arg) Then
                Try
                    Dim fileInfo As New IO.FileInfo(arg)
                    If fileInfo.Exists Then
                        If fileInfo.HasExtension(KnownFile.Type.DSI) Then
                            _fileArgStatus = FileState.HasDSI
                        ElseIf fileInfo.HasExtension(KnownFile.Type.HCV) Then
                            _fileArgStatus = FileState.HasHCV
                        ElseIf fileInfo.HasExtension(KnownFile.Type.xHCV) Then
                            _fileArgStatus = FileState.HasXHCV
                        ElseIf fileInfo.HasExtension(KnownFile.Type.KBL) Then
                            _fileArgStatus = FileState.HasKBL
                        ElseIf fileInfo.HasExtension(KnownFile.Type.PLMXML) Then
                            _fileArgStatus = FileState.HasPLMXML
                        Else
                            _fileArgStatus = FileState.HasAnotherFile
                        End If
                    Else
                        _fileArgStatus = FileState.HasNonExistingFile
                    End If

                    _file = fileInfo.FullName
                Catch ex As Exception
                    _fileArgStatus = FileState.NoFile
                End Try
            End If
        Next

        ProcessCommandLineSwitches(collectedCmdSwitches)

        _isProcessed = True
    End Sub


    Private Sub ProcessCommandLineSwitches(switches As CommandLineSwitches)
        _isInTestMode = switches.HasFlag(CommandLineSwitches.BuildServerTest)
    End Sub

    Private Function TryGetSwitch(switchString As String) As CommandLineSwitches
        If (switchString.StartsWith(COMMANDLINESWITCH_CHAR)) Then switchString = switchString.Remove(0, 1)

        Dim uCurrentArgStr As String = switchString.ToUpper
        Dim cmdSwitchesType As Type = GetType(CommandLineSwitches)

        ' Select the switch but don't care about the case
        Dim i As Integer = -1
        For Each Switch As String In [Enum].GetNames(cmdSwitchesType)
            i += 1

            Dim uSwitch As String = Switch.ToString.ToUpper
            If (uSwitch = uCurrentArgStr) Then Return CType([Enum].GetValues(cmdSwitchesType).GetValue(i), CommandLineSwitches)
        Next

        Return CommandLineSwitches.NoSwitch
    End Function

    ''' <summary>
    ''' Returns nothing if the CommandLineFile is empty!
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function TryGetAsFileInfo() As IO.FileInfo
        If (Not String.IsNullOrEmpty(Me.File)) Then Return New IO.FileInfo(Me.File)

        Return Nothing
    End Function


    ReadOnly Property File As String
        Get
            Return _file
        End Get
    End Property

    ReadOnly Property FileArgStatus As FileState
        Get
            Return _fileArgStatus
        End Get
    End Property

    ReadOnly Property HasArguments As Boolean
        Get
            Return My.Application.CommandLineArgs.Count > 0
        End Get
    End Property

    ReadOnly Property IsInTestMode As Boolean
        Get
            Return _isInTestMode
        End Get
    End Property

    ReadOnly Property IsProcessed As Boolean
        Get
            Return _isProcessed
        End Get
    End Property

End Class
