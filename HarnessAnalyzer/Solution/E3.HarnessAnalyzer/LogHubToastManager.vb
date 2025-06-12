Imports Infragistics.Win.UltraWinListView
Imports Zuken.E3.HarnessAnalyzer.Shared
Imports Zuken.E3.Lib.Toasts

Public Class LogHubToastManager
    Inherits ToastManager

    Public Sub New(form As MainForm)
        MyBase.New(form)
    End Sub

    Property LogHub As LogHub

    Protected Overrides ReadOnly Property MaxItems As Integer
        Get
            Return 3
        End Get
    End Property

    Public Function AddLogMessageShowToast(args As LogEventArgs) As UltraListViewItem
        With Me.LogHub.ulvLog
            Dim listItem As UltraListViewItem = LogHub.GetLogItem(args)
            .Items.Add(listItem)

            Dim tMsg As ToastMessage = Nothing
            Select Case args.LogLevel
                Case LogEventArgs.LoggingLevel.Warning
                    If CType(HostForm, MainForm).GeneralSettings.ToastMessageVerbosity.HasFlag(ToastMessageVerbosity.Warnings) Then
                        tMsg = New E3.Lib.Toasts.WarningToast(args.LogMessage)
                        tMsg.Data = listItem
                    End If
                Case LogEventArgs.LoggingLevel.Error
                    If CType(HostForm, MainForm).GeneralSettings.ToastMessageVerbosity.HasFlag(ToastMessageVerbosity.Errors) Then
                        tMsg = New ErrorToast(args.LogMessage)
                        tMsg.Data = listItem
                    End If
            End Select

            If tMsg IsNot Nothing Then
                Show(tMsg)
            End If

            .EndUpdate()
            Return listItem
        End With
    End Function

    Protected Overrides Sub Dispose(disposing As Boolean)
        If disposing Then
            LogHub = Nothing
        End If
        MyBase.Dispose(disposing)
    End Sub

End Class
