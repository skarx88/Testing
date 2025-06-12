Imports System.ComponentModel
Imports System.IO
Imports Zuken.E3.Lib.IO.Files.Hcv

Friend Class Document3DContainerFileDummy
    Implements IContainerFile

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Public Sub New()
        Me.FullName = DocumentForm.TAB_DOC3D_KEY + ".document"
    End Sub

    Public Sub New(fullName As String)
        _FullName = fullName
    End Sub

    Public ReadOnly Property Type As KnownContainerFileFlags Implements IContainerFile.Type
        Get
            Return KnownContainerFileFlags.Unspecified
        End Get
    End Property

    Public ReadOnly Property IsDisposed As Boolean Implements IContainerFile.IsDisposed

    Public ReadOnly Property FullName As String Implements IBaseFile.FullName

    Public ReadOnly Property Caption As String Implements IBaseFile.Caption
        Get
            Return DocumentForm.TAB_DOC3D_KEY
        End Get
    End Property

    Public Property Owner As IBaseFileCollection Implements IOwner(Of IBaseFileCollection).Owner

    Public Sub Initialize(data As Object) Implements IBaseFile.Initialize
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
    End Sub

    Public Function Clone() As Object Implements ICloneable.Clone
        Dim dummy_clone As New Document3DContainerFileDummy(Me.FullName)
        Return dummy_clone
    End Function

End Class
