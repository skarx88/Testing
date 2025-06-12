Imports System.Reflection

<Obfuscation(Feature:="renaming", Exclude:=True, ApplyToMembers:=True)>
Public Class MessageEventArgs
    Inherits EventArgs

    Public ProgressPercentage As Integer = -1
    Public StatusMessage As String = Nothing
    Public XPosition As Double = Double.NaN
    Public YPosition As Double = Double.NaN

    Public Property ProgressId As Guid
    Public Property MessageType As MsgType

    Public Sub New(MessageType As MsgType)
        ProgressId = Guid.NewGuid
        ProgressPercentage = 0
        Me.MessageType = MessageType
    End Sub

    Public Sub New(MessageType As MsgType, progressValue As Integer)
        Me.New(MessageType)
        Me.Set(progressValue)
    End Sub

    Public Sub New(MessageType As MsgType, progressValue As Integer, statusMsg As String)
        Me.New(MessageType)
        Me.Set(progressValue, statusMsg)
    End Sub

    Public Sub SetUseLastPosition()
        XPosition = Double.NaN
        YPosition = Double.NaN
    End Sub

    Public Sub [Set](progressValue As Integer)
        Me.Set(progressValue, Nothing, Double.NaN, Double.NaN)
    End Sub

    Public Sub [Set](progressValue As Integer, statusMsg As String)
        Me.Set(progressValue, statusMsg, Double.NaN, Double.NaN)
    End Sub

    Public Sub [Set](progressValue As Integer, statusMsg As String, xPos As Double, yPos As Double)
        Me.ProgressPercentage = progressValue
        Me.StatusMessage = statusMsg
        Me.XPosition = xPos
        Me.YPosition = yPos
    End Sub

    Public Sub [Set](type As MsgType)
        If type.HasFlag(MsgType.HideStatusMsg) Then
            SetClearStatusMsg()
        End If
        If type.HasFlag(MsgType.HidePosition) Then
            SetHidePosition()
        End If
        If type.HasFlag(MsgType.HideProgress) Then
            SetHideProgress()
        End If
        If type.HasFlag(MsgType.UseLastProgress) Then
            Me.ProgressPercentage = -1
        End If
        If type.HasFlag(MsgType.UseLastStatusMsg) Then
            Me.StatusMessage = Nothing
        End If
        If type.HasFlag(MsgType.UseLastPosition) Then
            Me.XPosition = Double.NaN
            Me.YPosition = Double.NaN
        End If

    End Sub

    Public Sub SetClearStatusMsg()
        Me.StatusMessage = String.Empty
        Me.ProgressPercentage = -1
    End Sub

    Public Sub SetHidePosition()
        Me.XPosition = Double.NegativeInfinity
        Me.YPosition = Double.NegativeInfinity
    End Sub

    Public Sub SetHideProgress()
        Me.ProgressPercentage = 0
    End Sub

    <Flags>
    Public Enum MsgType
        None = 0
        HideStatusMsg = 1
        HidePosition = 2
        HideProgress = 4
        HideAll = MsgType.HideStatusMsg Or MsgType.HidePosition Or MsgType.HideProgress
        UseLastProgress = 16
        UseLastStatusMsg = 32
        UseLastPosition = 64
        UseLastAll = MsgType.UseLastProgress Or MsgType.UseLastStatusMsg Or MsgType.UseLastPosition
        ShowCoordinates = 256
        ShowProgressAndMessage = 512
        ShowMessage = 1024
    End Enum

End Class
