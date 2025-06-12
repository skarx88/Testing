Imports System.Drawing
Imports System.IO
Imports System.IO.Compression
Imports System.Reflection
Imports System.Runtime.InteropServices
Imports System.Windows.Forms

<PreviewHandlerAttribute("HarnessAnalyzerPreviewHandlerNet472", ".hcv;.kbl;.svg;.exif;.xhcv", "{F3F529EA-E501-4467-B162-55628D1E8FEE}", DisableLowILProcessIsolation:=False)>
<ComDefaultInterface(GetType(IPreviewFromFile))>
Public Class HarnessAnalyzerFilePreviewHandler
    Inherits WinFormsPreviewHandlerEx
    Implements IPreviewFromFile

    Private _imageCache As New ImageCacheDirectory
    Private _lock As New System.Threading.SemaphoreSlim(1)
    Private _cancel As New System.Threading.CancellationTokenSource
    Private _lastFileToLoad As String
    Private _hasJTData As Boolean = False
    Private _hasTopSvg As Boolean = False
    Private _hasKblZData As Boolean = False
    Friend Const TOPOLOGY_FILE_NAME As String = "topologie.svg"

    Public Sub New()
        MyBase.New
        Me.Control = New PreviewControl
        Me.Control.Dock = DockStyle.Fill
    End Sub

    Friend Async Sub Load(info As FileInfo) Implements IPreviewFromFile.Load
        _cancel.Cancel()
        _lastFileToLoad = info.FullName
        Await _lock.WaitAsync
        _cancel = New Threading.CancellationTokenSource
        Try
            If info.FullName = _lastFileToLoad Then ' skip the current loading of the user has already selected other files before while we where waiting for the lock to release
                With CType(Me.Control, PreviewControl)
                    Try
                        .Reset(PreviewControl.ResetStyle.ResetToSwapViewer)
                        Select Case info.Extension
                            Case ".hcv"
                                Await LoadAndSetHcvPreviewAsync(info.FullName)
                            Case ".kbl"
                            Case ".svg"
                                Await LoadAndSetSvgPreviewAsync(info.FullName)
                            Case ".exif"
                                Dim img = System.Drawing.Image.FromFile(info.FullName)
                                .SetImage(img, False)
                            Case ".xhcv"
                                Using s = Utilities.FileStreamOpenRead(info.FullName)
                                    .SetxHcv(s, info.Name)
                                End Using
                        End Select
                        .chkJT.Checked = _hasJTData
                        .chkTopo2D.Checked = _hasTopSvg
                    Catch ex As Exception
                        .SetErrorText(ex.Message)
                    End Try
                End With
            End If
        Finally
            _lock.Release()
        End Try
    End Sub

    Private Sub LoadKblPreview(s As System.IO.Stream)
        ' TODO. draft converter ...
        'Using kbl As KblFile = KblFile.Create(s)
        '    CType(Me.Control, PreviewControl).Vdraw.ActiveDocument.Model.Entities.RemoveAll()

        '    If kbl.KblDocument.CreateVdPreview(CType(Me.Control, PreviewControl).Vdraw).IsSuccess Then
        '        CType(Me.Control, PreviewControl).Vdraw.ActiveDocument.ZoomExtents()

        '        Dim img As System.Drawing.Image = CType(Me.Control, PreviewControl).Vdraw.GetImage
        '        UpdatePreviewImage(img)
        '    End If
        'End Using
    End Sub

    Friend Shared Function SvgIsTopologyFile(svgfileName As String) As Boolean
        svgfileName = Path.GetFileName(svgfileName)
        If svgfileName.ToLower = TOPOLOGY_FILE_NAME.ToLower Then
            Return True
        End If
        Return svgfileName.ToLower.StartsWith("topo") OrElse Replace(svgfileName.ToLower, " ", String.Empty).StartsWith("sheet1")
    End Function

    Private Async Function LoadAndSetSvgPreviewAsync(svgFilePath As String) As Task(Of Boolean)
        'Dim img As System.Drawing.Image = Nothing

        'If img Is Nothing Then
        Await CType(Me.Control, PreviewControl).SetSvgAsync(svgFilePath, _cancel.Token)
            Return False
        'Else
        '    CType(Me.Control, PreviewControl).SetImage(img)
        '    Return True
        'End If
    End Function

    Private Async Function LoadAndSetHcvPreviewAsync(hcvFilePath As String) As Task(Of Boolean)
        Using fs As New FileStream(hcvFilePath, FileMode.Open, FileAccess.Read, FileShare.Read)
            Dim filePath As String = Await GetTopologyStreamAsync(fs)
            If filePath IsNot Nothing Then
                With CType(Me.Control, PreviewControl)
                    .chkJT.Checked = _hasJTData
                    .chkTopo2D.Checked = _hasTopSvg
                    .chkTopo3D.Checked = _hasKblZData
                    Await .SetSvgAsync(filePath, _cancel.Token)
                End With

                Return True
            End If
        End Using

        With CType(Me.Control, PreviewControl)
            .SetImage(My.Resources.noimage_400X265)
        End With

        Return False
    End Function

    Private Async Function GetTopologyStreamAsync(hcv As System.IO.Stream) As Task(Of String)
        _hasJTData = False
        _hasTopSvg = False
        _hasKblZData = False

        Dim svgLoaded As Boolean = False
        Dim svgStream As System.IO.Stream = Nothing
        Using zip As New ZipArchive(hcv)
            For Each entry In zip.Entries
                If IO.Path.GetExtension(entry.Name).ToLower = ".svg" Then
                    If Not svgLoaded AndAlso SvgIsTopologyFile(entry.FullName) Then
                        Using fs As New FileStream(Utilities.GetTempFilePath, FileMode.Create)
                            Await entry.Open.CopyToAsync(fs, 4096, _cancel.Token)
                            If Not _cancel.IsCancellationRequested Then
                                svgLoaded = True
                                _hasTopSvg = True
                                fs.Position = 0
                                Return fs.Name
                            End If
                        End Using
                    End If
                ElseIf IO.Path.GetExtension(entry.Name).ToLower = ".jt" Then
                    _hasJTData = True
                ElseIf IO.Path.GetExtension(entry.Name).ToLower = ".kbl" Then
                    If Utilities.HasKBLZData(entry.Open) Then
                        _hasKblZData = True
                    End If
                End If
            Next
        End Using
        Return Nothing
    End Function

    Protected Overrides Sub Dispose(disposing As Boolean)
        MyBase.Dispose(disposing)
        If disposing Then
            _imageCache?.Dispose()
        End If
        _imageCache = Nothing
    End Sub

End Class
