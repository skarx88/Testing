Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Runtime.Serialization
Imports System.Security.Cryptography

Friend Class ImageCacheDirectory
    Implements IDisposable

    Private Const DIR_NAME As String = "HcvFilePreviewerCache"
    Private Const INDEX_FILE_NAME As String = "Index.dat"

    Private _dir As DirectoryInfo
    Private _lock As New System.Threading.SemaphoreSlim(1)
    Private _cryptoMd5 As MD5 = MD5.Create

    Friend Sub New()
        _dir = New DirectoryInfo(IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), DIR_NAME))
        If Not _dir.Exists Then
            _dir.Create()
        End If
        If Not File.Exists(IndexFilePath) Then
            Using fs As New FileStream(IndexFilePath, FileMode.Create)
            End Using
        End If
    End Sub

    Friend Function TryGetPreviewFromIndex(hcvSvgFilePath As String) As System.Drawing.Image
        _lock.Wait()
        Try
            Dim deleteLine As Integer = -1
            Try
                If IO.File.Exists(hcvSvgFilePath) Then
                    Dim hcvSvgHash As Byte() = GetHashFromFile(hcvSvgFilePath)
                    Using fs As FileStream = Utilities.FileStreamOpenRead(IndexFilePath)
                        Dim i As Integer = -1
                        Using sr As New StreamReader(fs)
                            Dim line As String = Nothing
                            Do
                                line = sr.ReadLine()
                                i += 1
                                If line IsNot Nothing Then
                                    Dim kv As String() = line.Split("|"c)
                                    If kv.Length = 2 Then
                                        Dim hash As Byte() = Convert.FromBase64String(kv.First)
                                        Dim fp As String = kv.Last
                                        If ByteArrayCompare(hash, hcvSvgHash) Then
                                            Try
                                                fp = System.IO.Path.Combine(_dir.FullName, System.IO.Path.GetFileName(fp))
                                                If System.IO.File.Exists(fp) Then
                                                    Using f = Utilities.FileStreamOpenRead(fp)
                                                        Return System.Drawing.Image.FromStream(f)
                                                    End Using
                                                Else
                                                    deleteLine = i
                                                    Return Nothing
                                                End If
                                            Catch ex As Exception
                                                Try
                                                    System.IO.File.Delete(fp)
                                                Catch ex2 As IOException
                                                    ' do nothing here
                                                End Try
                                                deleteLine = i
                                                Return Nothing
                                            End Try
                                        End If
                                    End If
                                End If
                            Loop Until line Is Nothing
                        End Using
                    End Using
                End If
                Return Nothing
            Catch ex As IOException
                Return Nothing ' 'maybe we have not the rights to access this path, then we siply process without having a cached preview image
            Finally
                If deleteLine > -1 Then
                    RemoveLineFromFile(IndexFilePath, deleteLine)
                End If
            End Try
        Finally
            _lock.Release()
        End Try
    End Function

    Private Function RemoveLineFromFile(filePath As String, lineIdx As Integer) As Boolean
        Dim content As New Text.StringBuilder
        Using fs As New FileStream(IndexFilePath, FileMode.Open)
            Dim i As Integer = -1
            Using sr As New StreamReader(fs)
                Dim line As String = Nothing
                Do
                    line = sr.ReadLine()
                    i += 1
                    If line IsNot Nothing OrElse lineIdx <> i Then
                        content.AppendLine(line)
                    End If
                Loop Until line Is Nothing
            End Using
        End Using
        If content.Length > 0 Then
            File.WriteAllText(filePath, content.ToString)
            Return True
        End If
        Return False
    End Function

    ReadOnly Property IndexFilePath As String
        Get
            Return System.IO.Path.Combine(_dir.FullName, INDEX_FILE_NAME)
        End Get
    End Property

    Friend Sub AddPreviewToIndex(hcvSvgFilePath As String, img As System.Drawing.Image)
        _lock.Wait()
        Try
            Dim imgPreviewFilePath As String = System.IO.Path.Combine(_dir.FullName, Guid.NewGuid.ToString("N") + ".png")
            Dim imgSvgHash As Byte() = GetHashFromFile(hcvSvgFilePath)
            Dim kv_line As String = String.Format("{0}|{1}", Convert.ToBase64String(imgSvgHash), System.IO.Path.GetFileName(imgPreviewFilePath))

            img.Save(imgPreviewFilePath, System.Drawing.Imaging.ImageFormat.Png)
            System.IO.File.AppendAllLines(IndexFilePath, {kv_line})
        Catch ex As Exception
            Return
        Finally
            _lock.Release()
        End Try
    End Sub

    Private Function GetHashFromFile(filePath As String) As Byte()
        Dim fi As New FileInfo(filePath)
        Using s = fi.OpenRead
            Return _cryptoMd5.ComputeHash(s)
        End Using
    End Function

    Friend Sub Dispose() Implements IDisposable.Dispose
        _cryptoMd5.Dispose()
    End Sub

    <DllImport("msvcrt.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Shared Function memcmp(ByVal b1 As Byte(), ByVal b2 As Byte(), ByVal count As Long) As Integer
    End Function

    Private Shared Function ByteArrayCompare(ByVal b1 As Byte(), ByVal b2 As Byte()) As Boolean
        Return b1.Length = b2.Length AndAlso memcmp(b1, b2, b1.Length) = 0
    End Function

End Class
