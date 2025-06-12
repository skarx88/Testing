Imports System.IO
Imports System.Reflection

Public MustInherit Class GridAppearance
    Implements ICloneable

    Protected _gridTable As GridTable

    Public Const DIRECTORY_NAME As String = "GridAppearance"
    Public Const FILE_EXT As String = "xml"

    Protected Sub New(gridType As KblObjectType)
        Me.TableType = gridType
        Me.FilePath = GetFilePath(gridType)
        Me.DefaultFilePath = GetDefaultFilePath(gridType)
        Me.Update()
    End Sub

    Public MustOverride Sub CreateDefaultTable()

    Public Shared Function LoadOrDefaultNew(type As Type) As GridAppearance
        If type.IsAbstract Then
            Throw New ArgumentException(String.Format("Can't load data with abstract {0}-type: ""{1}""", GetType(GridAppearance).Name, type.Name))
        End If

        Dim appearance As GridAppearance = CType(Activator.CreateInstance(type), GridAppearance)
        appearance.LoadOrCreateDefaultTable()
        Return appearance
    End Function

    Public Sub LoadOrCreateDefaultTable()
        If Not TryLoad() Then
            CreateDefaultTable()
        End If
    End Sub

    Public Shared Function LoadOrDefaultNew(Of T As GridAppearance)() As T
        Return CType(LoadOrDefaultNew(GetType(T)), T)
    End Function

    Public Function TryLoad() As Boolean
        Try
            Return TryLoadGridTable()
        Finally
            TryLoadDefaultGridTable()
        End Try
    End Function

    Private Function TryLoadGridTable() As Boolean
        Me.Update()
        Me._gridTable = Nothing
        If FileExists Then
            Me._gridTable = GridTable.LoadFromFile(FilePath)
            If _gridTable IsNot Nothing Then
                For Each colum As GridColumn In _gridTable.GridColumns
                    If Not colum.Comparable Then
                        ExcludedProperties.TopLevelExcludedProperties.Add(colum.KBLPropertyName)
                    End If
                Next
                If (_gridTable.GridSubTable IsNot Nothing) Then
                    For Each colum As GridColumn In _gridTable.GridSubTable.GridColumns
                        If Not colum.Comparable Then
                            ExcludedProperties.SubLevelExcludedProperties.Add(colum.KBLPropertyName)
                        End If
                    Next
                End If
            End If
            Return True
        End If
        Return False
    End Function

    Private Sub TryLoadDefaultGridTable()
        If DefaultFileExists Then
            Try
                Me._DefaultGridTable = GridTable.LoadFromFile(DefaultFilePath)
            Catch ex As Exception
                'do nothing  here / ignore error and proceed -> so we have no default then when loading failed
            End Try
        End If
    End Sub

    ''' <summary>
    ''' Copies the default file only when it exists and the current file not
    ''' </summary>
    ''' <returns></returns>
    Public Function TryCopyFromDefaultDir() As Boolean
        Me.Update()
        If Not FileExists Then
            If DefaultFileExists Then
                Try
                    CreateDirectoryIfNeeded()
                    File.Copy(DefaultFilePath, FilePath, True)
                    Return True
                Catch ex As Exception
                    Return False
                End Try
            End If
        End If

        Return False
    End Function

    Public Sub Save()
        CreateDirectoryIfNeeded()
        _gridTable.Save(Me.FilePath)
    End Sub

    Private Shared Sub CreateDirectoryIfNeeded()
        If Not Directory.Exists(DirectoryPath) Then
            Directory.CreateDirectory(DirectoryPath)
        End If
    End Sub

    ReadOnly Property TableType As KblObjectType
    ReadOnly Property FilePath As String
    ReadOnly Property DefaultFilePath As String
    ReadOnly Property FileExists As Boolean
    ReadOnly Property DefaultFileExists As Boolean

    Shared ReadOnly Property DirectoryPath As String
        Get
            Dim appDataDir As String = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
            Return IO.Path.Combine(appDataDir, Path.Combine(HarnessAnalyzer.[Shared].COMPANY_FOLDER, HarnessAnalyzer.[Shared].PRODUCT_FOLDER, DIRECTORY_NAME))
        End Get
    End Property

    Shared ReadOnly Property DefaultDirectoryPath As String
        Get
            Return Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), String.Format("{0}Defaults", DIRECTORY_NAME))
        End Get
    End Property

    Public Sub Update()
        Me._FileExists = Not String.IsNullOrEmpty(Me.FilePath) AndAlso File.Exists(Me.FilePath)
        Me._DefaultFileExists = Not String.IsNullOrEmpty(Me.DefaultFilePath) AndAlso File.Exists(Me.DefaultFilePath)
    End Sub

    Public Function TryDeleteFile() As Boolean
        Return System.IO.FileEx.TryDelete(GetFilePath(Me.TableType))
    End Function

    Public Shared Function GetFilePath(gridType As KblObjectType) As String
        Return Path.Combine(DirectoryPath, GetFileName(gridType))
    End Function

    Public Shared Function GetDefaultFilePath(gridType As KblObjectType) As String
        Return Path.Combine(DefaultDirectoryPath, GetFileName(gridType))
    End Function

    Private Shared Function GetGridTypeFileName(gridType As KblObjectType) As String
        Dim name As String = gridType.ToString
        Return name.Replace("_occurrence", String.Empty) ' HINT: remove "_occurrence"- from filename to let the file-name look a bit "cleaner". But this removal is not mandatory
    End Function

    Private Shared Function GetFileName(gridType As KblObjectType) As String
        Return Path.ChangeExtension(GetGridTypeFileName(gridType), FILE_EXT)
    End Function

    ReadOnly Property GridTable() As GridTable
        Get
            Return _gridTable
        End Get
    End Property

    ReadOnly Property HasGridAppearance As Boolean
        Get
            Return _gridTable IsNot Nothing
        End Get
    End Property

    ReadOnly Property DefaultGridTable As GridTable

    ReadOnly Property ExcludedProperties() As New ExcludedProperties

    Private Shared Function GetAllCore() As GridAppearance()
        Dim appearancesList As New List(Of GridAppearance)
        For Each gatType As Type In AllTypes
            appearancesList.Add(CType(Activator.CreateInstance(gatType), GridAppearance))
        Next
        Return appearancesList.ToArray
    End Function

    Shared ReadOnly Property All(Optional buffered As Boolean = True) As GridAppearance()
        Get
            Static myAppearances As List(Of GridAppearance)
            If Not buffered OrElse myAppearances Is Nothing Then
                myAppearances = New List(Of GridAppearance)
                For Each gapp As GridAppearance In GetAllCore()
                    myAppearances.Add(gapp)
                Next
            End If
            Return myAppearances.ToArray
        End Get
    End Property

    Shared ReadOnly Property AllTypes(Optional buffered As Boolean = True) As Type()
        Get
            Static types As Type()
            If Not buffered OrElse types Is Nothing Then
                types = GetType(GridAppearance).Assembly.GetTypes(Of GridAppearance).Where(Function(tp) Not tp.IsAbstract And tp.IsClass).ToArray
            End If
            Return types
        End Get
    End Property

    Public Shared Function ResolveByObjectType(objectType As KblObjectType, Optional buffered As Boolean = True) As GridAppearance
        Static gaByType As Dictionary(Of KblObjectType, GridAppearance)
        If Not buffered OrElse gaByType Is Nothing Then
            gaByType = New Dictionary(Of KblObjectType, GridAppearance)
            For Each ga As GridAppearance In All(buffered)
                Dim kblObjType As KblObjectTypeAttribute = ga.GetType.GetCustomAttributes(GetType(KblObjectTypeAttribute), False).OfType(Of KblObjectTypeAttribute).SingleOrDefault
                If kblObjType IsNot Nothing Then
                    gaByType.Add(kblObjType.ObjectType, ga)
                End If
            Next
        End If

        If gaByType.ContainsKey(objectType) Then
            Return gaByType(objectType)
        End If

        Return Nothing
    End Function

    Public Overridable Function Clone() As Object Implements ICloneable.Clone
        Dim newAppearance As GridAppearance = CType(Activator.CreateInstance(Me.GetType), GridAppearance)
        With newAppearance
            If _gridTable IsNot Nothing Then
                newAppearance._gridTable = Clone(_gridTable)
                newAppearance._ExcludedProperties = ExcludedProperties
                If DefaultGridTable IsNot Nothing Then
                    newAppearance._DefaultGridTable = Clone(DefaultGridTable)
                End If
            End If
        End With
        Return newAppearance
    End Function

    Private Shared Function Clone(table As GridTable) As GridTable
        Using ms As New MemoryStream
            table.SaveToStream(ms)
            ms.Position = 0
            Return GridTable.LoadFromStream(ms)
        End Using
    End Function

End Class
