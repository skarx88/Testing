Imports System.IO
Imports System.IO.Compression
Imports System.Reflection
Imports System.Runtime.Serialization
Imports System.Text

Public Class Utilities

#If NETFRAMEWORK Or WINDOWS Then
    Private Const FILE_PREVIEWER_REG_NAME As String = "HarnessAnalyzerPreviewHandlerNet472"
#End If
    Private Const FILE_PREVIEWER_ASSEMBLY_NAME As String = "E3.HarnessAnalyzer.FilePreviewer.dll"
    Private Const FILE_PREVIEWER_TARGET_DIR_NAME As String = "FilePreviewer"

    Public Shared Function GetApplicationSettingsPath() As String
        Dim appDataDir As String = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
        Return IO.Path.Combine(appDataDir, Path.Combine(HarnessAnalyzer.[Shared].COMPANY_FOLDER, HarnessAnalyzer.[Shared].PRODUCT_FOLDER))
    End Function

    Private Shared Function GetFilePreviewAssemblyPath() As String
        Dim appDir As New DirectoryInfo(GetApplicationSettingsPath)
        If Not appDir.Exists Then
            appDir.Create()
        End If

        Dim userDir As New DirectoryInfo(IO.Path.Combine(appDir.FullName, FILE_PREVIEWER_TARGET_DIR_NAME))
        Dim filePreviewAssembly As String = IO.Path.Combine(userDir.FullName, FILE_PREVIEWER_ASSEMBLY_NAME)

        If Not userDir.Exists Then
            userDir.Create()
        End If

        If Not IO.File.Exists(filePreviewAssembly) Then
            Dim filePrev_res_name As String = IO.Path.GetFileNameWithoutExtension(FILE_PREVIEWER_ASSEMBLY_NAME)

            Using s As Stream = Assembly.GetExecutingAssembly().GetManifestResourceStreamWithPartialName(filePrev_res_name)
                If s IsNot Nothing Then
                    Using a As New ZipArchive(s, ZipArchiveMode.Read, True)
#If NETFRAMEWORK Then
                        a.ExtractToDirectory(userDir.FullName)
#Else
                        a.ExtractToDirectory(userDir.FullName, True)
#End If
                    End Using
                Else
                    Return Nothing
                End If
            End Using
        End If

        Return filePreviewAssembly
    End Function

#If NETFRAMEWORK Or WINDOWS Then
    Public Shared Function IsFilePreviewerRegistered() As Boolean
        Return Runtime.InteropServices.PreviewHandlerEx.IsRegistered(FILE_PREVIEWER_REG_NAME, Runtime.InteropServices.RegistryTarget.User)
    End Function

    Public Shared Function RegisterFilePreviewer() As Boolean
        Dim path As String = GetFilePreviewAssemblyPath()
        If Not String.IsNullOrEmpty(path) Then
            Dim a As Reflection.Assembly = System.Reflection.Assembly.LoadFile(path)
            If a IsNot Nothing Then
                Return System.Runtime.InteropServices.PreviewHandlerEx.RegisterAllPreviewHandlers(a, Runtime.InteropServices.RegistryTarget.User)
            End If
        End If
        Return False
    End Function

    Public Shared Function UnRegisterFilePreviewer() As Boolean
        Dim path As String = GetFilePreviewAssemblyPath()
        If Not String.IsNullOrEmpty(path) Then
            Dim a As Reflection.Assembly = System.Reflection.Assembly.LoadFile(path)
            Return System.Runtime.InteropServices.PreviewHandlerEx.UnRegisterAllPreviewHandlers(a, Runtime.InteropServices.RegistryTarget.User)
        End If
        Return False
    End Function
#End If

    Public Shared Function ConvertBooleanToString(value As String, defaultValue As String) As String
        If (String.IsNullOrEmpty(value)) Then
            Return defaultValue
        Else
            If (value.Trim.ToLower = Boolean.TrueString.ToLower) Then
                value = "yes"
            ElseIf (value.Trim.ToLower = Boolean.FalseString.ToLower) Then
                value = "no"
            Else
                Return defaultValue
            End If

            Return Convert.ToString(value, Globalization.CultureInfo.InvariantCulture)
        End If
    End Function

    Public Shared Function ConvertBooleanToYesNoString(value As Boolean) As String
        If (Not value) Then
            Return "no"
        End If
        Return "yes"
    End Function

    Public Shared Function ConvertDateToDsiDateString(dateValue As DateTime) As String
        Return String.Format("{0:dd}/{0:MM}/{0:yyyy}", dateValue)
    End Function

    Public Shared Function ConvertToBoolean(value As String, defaultValue As Boolean) As Boolean
        If (String.IsNullOrEmpty(value)) Then
            Return defaultValue
        Else
            Select Case value.Trim.ToLower
                Case "yes", "y", "true", "t", "1"
                    value = Boolean.TrueString
                Case "no", "n", "false", "f", "0"
                    value = Boolean.FalseString
                Case Else
                    value = defaultValue.ToString
            End Select

            Return Convert.ToBoolean(value, Globalization.CultureInfo.InvariantCulture)
        End If
    End Function

    Public Shared Function ConvertToSingle(value As String, defaultValue As Single) As Single
        If (String.IsNullOrEmpty(value)) Then
            Return defaultValue
        End If

        Dim lastDot As Integer = value.LastIndexOf("."c)
        Dim lastComma As Integer = value.LastIndexOf(","c)

        If (lastDot < lastComma AndAlso lastComma > -1) Then
            value = value.Replace(".", String.Empty)
            value = value.Replace(",", ".")
        End If

        If (lastComma < lastDot AndAlso lastDot > -1) Then
            value = value.Replace(",", String.Empty)
        End If

        Dim tmpValue As Single

        If (Single.TryParse(value, Globalization.NumberStyles.Float, Globalization.CultureInfo.InvariantCulture, tmpValue)) Then
            Return tmpValue
        End If
        Return defaultValue
    End Function

    Public Shared Function GetIdrefs(idrefs As String) As List(Of String)
        Dim l As New List(Of String)
        If (Not String.IsNullOrEmpty(idrefs)) Then
            l.AddRange(idrefs.Split(" ".ToCharArray, StringSplitOptions.RemoveEmptyEntries))
        End If
        Return l
    End Function

    Public Shared Function ReadXml(Of T)(stream As System.IO.Stream, Optional rootName As String = Nothing, Optional rootNameSpace As String = Nothing) As T
        Dim ser As DataContractSerializer
        If rootName IsNot Nothing OrElse rootNameSpace IsNot Nothing Then
            ser = New DataContractSerializer(GetType(T), rootName, rootNameSpace)
        Else
            ser = New DataContractSerializer(GetType(T))
        End If
        Return DirectCast(ser.ReadObject(stream), T)
    End Function

    Public Shared Function ReadXml(Of T)(fullPath As String) As T
        Using fs As New FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read)
            Try
                Return ReadXml(Of T)(fs)
            Finally
                fs.Close()
            End Try
        End Using
    End Function

    Public Shared Sub WriteXml(graph As Object, stream As System.IO.Stream, Optional rootName As String = "", Optional rootNameSpace As String = "")
        If rootName = String.Empty Then
            rootName = graph.GetType.Name
        End If
        If rootNameSpace = String.Empty Then
            rootNameSpace = graph.GetType.Namespace
        End If

        Dim dcs As New DataContractSerializer(graph.GetType, rootName, rootNameSpace)
        Dim xmlWriterSettings As System.Xml.XmlWriterSettings = New System.Xml.XmlWriterSettings()
        xmlWriterSettings.Encoding = Encoding.UTF8
        xmlWriterSettings.NewLineChars = vbCrLf.ToCharArray
        xmlWriterSettings.Indent = True
        xmlWriterSettings.IndentChars = "    "

        Using xmlWriter As System.Xml.XmlWriter = System.Xml.XmlTextWriter.Create(stream, xmlWriterSettings)
            dcs.WriteObject(xmlWriter, graph)
            xmlWriter.Close()
        End Using
    End Sub

    Public Shared Function GetOrCreateApplicationDirectoryName(folderName As String) As String
        Dim appDataDir As String = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
        Dim e3HarnessAnalyzerDir As String = IO.Path.Combine(appDataDir, Path.Combine(COMPANY_FOLDER, PRODUCT_FOLDER))
        Dim fontAndShapeFilesDir As String = IO.Path.Combine(e3HarnessAnalyzerDir, folderName)

        If (Not IO.Directory.Exists(e3HarnessAnalyzerDir)) Then
            IO.Directory.CreateDirectory(e3HarnessAnalyzerDir)
        End If

        If (Not IO.Directory.Exists(fontAndShapeFilesDir)) Then
            IO.Directory.CreateDirectory(fontAndShapeFilesDir)
        End If

        Return fontAndShapeFilesDir
    End Function

End Class
