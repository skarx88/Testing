Imports Zuken.E3.Lib.Comparer.Topology.Documents
Imports Zuken.E3.Lib.IO.Files.Hcv

Public Class HarnessAnalyzerEEModelsDocument
    Inherits EEModelsDocument

    Private _leftFile As HcvFile
    Private _rightFile As HcvFile

    Public Sub New(leftHcv As HcvFile, rightHcv As HcvFile)
        MyBase.New(leftHcv.FullName, rightHcv.FullName)
        _leftFile = leftHcv
        _rightFile = rightHcv
    End Sub

    Protected Overrides Sub InitLoadFilesManager(ByRef filesManager As ILoadManager)
        filesManager = New HcvLoadFilesManager
    End Sub

    Public Shadows Property LeftFile As HcvFile
        Get
            Return _leftFile
        End Get
        Set(value As HcvFile)
            _leftFile = value
            MyBase.LeftFile = _leftFile?.FullName
        End Set
    End Property

    Public Shadows Property RightFile As HcvFile
        Get
            Return _rightFile
        End Get
        Set(value As HcvFile)
            _rightFile = value
            MyBase.RightFile = _rightFile?.FullName
        End Set
    End Property

    Protected Overrides Function LoadModelAsync(side As DocumentSide, Optional forceReloadFile As Boolean = False, Optional userData As Object = Nothing) As Task(Of LoadModelResult)
        If userData Is Nothing Then
            Return MyBase.LoadModelAsync(side, forceReloadFile, If(side = DocumentSide.Left, Me.LeftFile, Me.RightFile))
        Else
            Return MyBase.LoadModelAsync(side, forceReloadFile, userData)
        End If
    End Function

    Protected Overrides Sub Dispose(disposing As Boolean)
        MyBase.Dispose(disposing)
        _leftFile = Nothing
        _rightFile = Nothing
    End Sub

    Private Class HcvLoadFilesManager
        Inherits LoadFilesManager(Of ModelContainer)

        Protected Overrides Function LoadFile(startedInDocument As DocumentBase, filePath As String, side As DocumentSide, Optional forceReloadFile As Boolean = False, Optional userData As Object = Nothing) As LoadModelResult
            If TypeOf userData Is HcvFile Then
                Return LoadFile(startedInDocument, filePath, side, False, forceReloadFile, userData)
            Else
                Return MyBase.LoadFile(startedInDocument, filePath, side, forceReloadFile, userData)
            End If
        End Function

        Protected Overrides Function FileExists(filePath As String, userData As Object) As Boolean
            If TypeOf userData Is HcvFile Then
                Return (CType(userData, HcvFile).KblDocument?.HasData).GetValueOrDefault
            Else
                Return MyBase.FileExists(filePath, userData)
            End If
        End Function

        Protected Overrides Function LoadFileCore(filePath As String, userData As Object) As ModelContainer
            'TOdO: PLMXML/XML
            If TypeOf userData Is HcvFile Then
                Dim container As ModelContainer = CreateNewContainer()
                CType(userData, HcvFile).KblDocument.CopyToModel(container.Model)
                Return container
            Else
                Return MyBase.LoadFileCore(filePath, userData)
            End If
        End Function

    End Class

End Class
