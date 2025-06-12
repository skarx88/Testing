Imports System.Reflection

<Obfuscation(Feature:="renaming", Exclude:=True, ApplyToMembers:=True)>
Public Class LogEventArgs
    Inherits EventArgs

    Public Enum LoggingLevel
        [Error] = 0
        Information = 1
        Warning = 2
    End Enum

    Public LogLevel As LoggingLevel
    Public LogMessage As String

    Public Sub New()
    End Sub

    Public Sub New(message As [Lib].IO.MessageInfo)
        Select Case message.Type
            Case [Lib].IO.MessageType.Error
                Me.LogLevel = LoggingLevel.Error
            Case [Lib].IO.MessageType.Information, [Lib].IO.MessageType.Undefined
                Me.LogLevel = LoggingLevel.Information
            Case [Lib].IO.MessageType.Warning
                Me.LogLevel = LoggingLevel.Warning
        End Select
        Me.LogMessage = message.Text
    End Sub

    Public Sub New(ByVal level As LoggingLevel, ByVal message As String)
        LogLevel = level
        LogMessage = message
    End Sub

    Public Overrides Function ToString() As String
        Return String.Format("{0}: {1}", LogLevel.ToString, Me.LogMessage)
    End Function

End Class
