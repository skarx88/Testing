Public Class FormHelpers

    Public Const DEFAULT_CANCEL_TIMEOUT_MS As Integer = 10000
    Public Const DEFAULT_SHOW_TIMEOUT_MS As Integer = 1000

    Public Shared Async Function ShowDialogAfterTimeoutAsync(Of TForm As Form)(owner As IWin32Window, timeout As TimeSpan, condition As Func(Of Boolean), cancelTimeOut As TimeSpan) As Task(Of DialogResult)
        Return Await ShowDialogAfterTimeoutAsync(Activator.CreateInstance(Of TForm), owner, timeout, condition, cancelTimeOut)
    End Function

    Public Shared Async Function ShowDialogAfterTimeoutAsync(formToShow As Form, owner As IWin32Window, timeout As TimeSpan, condition As Func(Of Boolean), cancelTimeOut As TimeSpan) As Task(Of DialogResult)
        Application.UseWaitCursor = True
        Try
            If Not Await Task.Factory.StartNew(Function() Threading.SpinWait.SpinUntil(Function() condition.Invoke, timeout)) Then
                Return ShowDialogInternal(formToShow, owner, timeout, condition, cancelTimeOut)
            End If
        Finally
            Application.UseWaitCursor = False
        End Try
        Return DialogResult.OK
    End Function

    ''' <summary>
    ''' Show's dialog after 1s timeout (condition=false) as default
    ''' </summary>
    ''' <param name="owner"></param>
    ''' <param name="condition"></param>
    ''' <param name="cancelTimeOut">Default value is 10s</param>
    ''' <returns></returns>
    Public Shared Async Function ShowDialogAfterTimeoutAsync(formToShow As Form, owner As IWin32Window, condition As Func(Of Boolean), Optional cancelTimeOut As TimeSpan = Nothing) As Task(Of DialogResult)
        If cancelTimeOut = TimeSpan.Zero Then
            cancelTimeOut = TimeSpan.FromMilliseconds(DEFAULT_CANCEL_TIMEOUT_MS)
        End If
        Return Await ShowDialogAfterTimeoutAsync(formToShow, owner, TimeSpan.FromMilliseconds(DEFAULT_SHOW_TIMEOUT_MS), condition, cancelTimeOut)
    End Function

    Public Shared Function ShowDialogAfterTimeout(Of TForm As Form)(owner As IWin32Window, timeout As TimeSpan, condition As Func(Of Boolean), cancelTimeOut As TimeSpan) As DialogResult
        Return ShowDialogAfterTimeout(Activator.CreateInstance(Of TForm), owner, timeout, condition, cancelTimeOut)
    End Function

    ''' <summary>
    ''' Show's dialog after 1s timeout (condition=false) as default
    ''' </summary>
    ''' <param name="owner"></param>
    ''' <param name="condition"></param>
    ''' <param name="cancelTimeOut">Default value is 10s</param>
    ''' <returns></returns>
    Public Shared Async Function ShowDialogAfterTimeoutAsync(Of TForm As Form)(owner As IWin32Window, condition As Func(Of Boolean), Optional cancelTimeOut As TimeSpan = Nothing) As Task(Of DialogResult)
        Return Await ShowDialogAfterTimeoutAsync(Activator.CreateInstance(Of TForm), owner, condition, cancelTimeOut)
    End Function

    ''' <summary>
    ''' Show's dialog after 1s timeout (condition=false) as default
    ''' </summary>
    ''' <param name="owner"></param>
    ''' <param name="condition"></param>
    ''' <param name="cancelTimeOut">Default value is 10s</param>
    ''' <returns></returns>
    Public Shared Function ShowDialogAfterTimeout(formToShow As Form, owner As IWin32Window, condition As Func(Of Boolean), Optional cancelTimeOut As TimeSpan = Nothing) As DialogResult
        If cancelTimeOut = TimeSpan.Zero Then
            cancelTimeOut = TimeSpan.FromMilliseconds(DEFAULT_CANCEL_TIMEOUT_MS)
        End If
        Return ShowDialogAfterTimeout(formToShow, owner, TimeSpan.FromMilliseconds(DEFAULT_SHOW_TIMEOUT_MS), condition, cancelTimeOut)
    End Function

    Public Shared Function ShowDialogAfterTimeout(Of TForm As Form)(owner As IWin32Window, condition As Func(Of Boolean), Optional cancelTimeOut As TimeSpan = Nothing) As DialogResult
        Return ShowDialogAfterTimeout(Activator.CreateInstance(Of TForm), owner, condition, cancelTimeOut)
    End Function

    Public Shared Function ShowDialogAfterTimeout(formToShow As Form, owner As IWin32Window, timeout As TimeSpan, condition As Func(Of Boolean), cancelTimeOut As TimeSpan) As DialogResult
        Application.UseWaitCursor = True
        Try
            If Not Threading.SpinWait.SpinUntil(Function() condition.Invoke, timeout) Then
                Return ShowDialogInternal(formToShow, owner, timeout, condition, cancelTimeOut)
            End If
        Finally
            Application.UseWaitCursor = False
        End Try
        Return DialogResult.OK
    End Function

    Private Shared Function ShowDialogInternal(formToShow As Form, owner As IWin32Window, timeout As TimeSpan, condition As Func(Of Boolean), cancelTimeout As TimeSpan) As DialogResult
        Dim intervalCount As Integer = 0
        Dim timer As New Timer
        timer.Interval = 200

        Dim tick As EventHandler =
              Sub()
                  intervalCount += 1
                  If Not condition.Invoke Then
                      If TimeSpan.FromMilliseconds((intervalCount * timer.Interval) + timeout.TotalMilliseconds) >= cancelTimeout Then
                          formToShow.Close()
                          formToShow.DialogResult = DialogResult.Cancel
                      End If
                  Else
                      formToShow.Close()
                      formToShow.DialogResult = DialogResult.OK
                  End If
              End Sub
        AddHandler Timer.Tick, tick

        timer.Start()
        Try
            Return formToShow.ShowDialog(owner)
        Finally
            timer.Stop()
            RemoveHandler timer.Tick, tick
        End Try
    End Function

End Class
