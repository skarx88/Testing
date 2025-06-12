Imports System.Drawing
Imports System.IO
Imports System.Xml

Friend Class OffScreenWebbrowser

    Friend Event ProgressChanged(sender As Object, e As WebBrowserProgressChangedEventArgs)

    Private _isCompleted As Boolean = False
    Private _isCancelled As Boolean
    Private WithEvents _instance As WebBrowser

    Friend Sub New()
    End Sub

    Friend Async Function GetBitmapFromStreamAsync(s As System.IO.Stream, documentSize As System.Drawing.Size, targetSize As System.Drawing.Size, onImageFinished As Action(Of Bitmap)) As Task(Of Boolean)
        Dim t As New Threading.Thread(
            Sub()
                _isCancelled = False
                _isCompleted = False

                _instance = New WebBrowser
                _instance.Width = documentSize.Width
                _instance.Height = documentSize.Height
                _instance.ScriptErrorsSuppressed = True
                Try
                    _instance.ScriptErrorsSuppressed = True
                    _instance.DocumentStream = s
                    System.Threading.SpinWait.SpinUntil(Function()
                                                            If Not _isCompleted And Not _isCancelled Then
                                                                Application.DoEvents()
                                                            End If
                                                            Return _isCompleted Or _isCancelled
                                                        End Function)
                    If Not _isCancelled Then
                        Dim img = New Bitmap(Math.Min(targetSize.Width, 30000), Math.Min(targetSize.Height, 30000))
                        _instance.DrawToBitmapFast(img, New Rectangle(0, 0, targetSize.Width, targetSize.Height))
                        If onImageFinished IsNot Nothing Then
                            onImageFinished.Invoke(img)
                        End If
                    End If
                Finally
                    _instance.Dispose()
                End Try
            End Sub)

        t.SetApartmentState(Threading.ApartmentState.STA)
        t.Start()

        If Not Await Task.Run(Function() System.Threading.SpinWait.SpinUntil(Function() t.ThreadState = Threading.ThreadState.Stopped, TimeSpan.FromSeconds(3))) Then
            Return False
        End If

        Return True
    End Function

    Friend Sub StopLoad()
        _isCancelled = True
        If _instance IsNot Nothing Then
            _instance.Stop()
        End If
    End Sub

    Private Sub _instance_DocumentCompleted(sender As Object, e As WebBrowserDocumentCompletedEventArgs) Handles _instance.DocumentCompleted
        _isCompleted = True
    End Sub

    Private Sub _instance_ProgressChanged(sender As Object, e As WebBrowserProgressChangedEventArgs) Handles _instance.ProgressChanged
        RaiseEvent ProgressChanged(Me, e)
    End Sub

    Friend Shared Function ReadSvgSizeFromFile(filePath As String, Optional ByRef viewBox As Rectangle = Nothing) As SizeF
        Using fs As New FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read)
            Return ReadSvgSize(fs, viewBox)
        End Using
    End Function

    Friend Shared Function ReadSvgSize(svg As System.IO.Stream, Optional ByRef viewBox As Rectangle = Nothing) As SizeF
        Using sReader As New StreamReader(svg, System.Text.Encoding.Default, True, 1024, leaveOpen:=True)
            Using reader As Xml.XmlReader = Xml.XmlReader.Create(sReader, New XmlReaderSettings() With {.IgnoreComments = True, .DtdProcessing = DtdProcessing.Parse})
                reader.MoveToContent()
                If reader.Name = "svg" AndAlso reader.HasAttributes Then
                    Dim width As Single = CSng(reader.GetAttribute("width"))
                    Dim height As Single = CSng(reader.GetAttribute("height"))
                    Dim box As String = reader.GetAttribute("viewBox")
                    If Not String.IsNullOrEmpty(box) Then
                        box = box.Trim
                        Dim bps As String() = box.Split(" "c)
                        If bps.Length = 1 Then
                            bps = bps.First.Split(","c)
                        End If
                        viewBox = New Rectangle(CInt(bps(0)), CInt(bps(1)), CInt(bps(2)), CInt(bps(3)))
                    End If
                    Return New SizeF(width, height)
                Else
                    Throw New System.IO.InvalidDataException("Provided data is not an svg file!")
                End If
            End Using
        End Using
    End Function

End Class
