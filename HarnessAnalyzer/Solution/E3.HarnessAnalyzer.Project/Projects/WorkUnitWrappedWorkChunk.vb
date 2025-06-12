Imports devDept.Eyeshot
Imports Zuken.E3.Lib.IO.Files

Public Class WorkUnitWrappedWorkChunk
    Inherits WorkUnitWrapped
    Implements IWorkChunkAsync
    Implements IProvidesWorkViewAdapter

    Public Sub New(workChunk As Zuken.E3.Lib.IO.Files.IWorkChunk)
        MyBase.New(workChunk)
    End Sub

    Public Shadows ReadOnly Property Work As Zuken.E3.Lib.IO.Files.IWorkChunk
        Get
            Return CType(MyBase.Work, Zuken.E3.Lib.IO.Files.IWorkChunk)
        End Get
    End Property

    Public Shadows ReadOnly Property IsCancelled As Boolean Implements Zuken.E3.Lib.IO.Files.IWorkChunkAsync.IsCancellationRequested
        Get
            Return (BackgroundWorker?.CancellationPending).GetValueOrDefault OrElse (Work?.IsCancellationRequested).GetValueOrDefault OrElse MyBase.IsCancelled
        End Get
    End Property

    Public Property Maximum As Long Implements Zuken.E3.Lib.IO.Files.IWorkChunk.Maximum
        Get
            Return Work.Maximum
        End Get
        Set(value As Long)
            Work.Maximum = value
        End Set
    End Property

    Public ReadOnly Property View As IWorkFileViewAdapter Implements IProvidesWorkViewAdapter.View
        Get
            Return TryCast(MyBase.Work, Zuken.E3.Lib.IO.Files.IProvidesWorkViewAdapter)?.View
        End Get
    End Property

End Class
