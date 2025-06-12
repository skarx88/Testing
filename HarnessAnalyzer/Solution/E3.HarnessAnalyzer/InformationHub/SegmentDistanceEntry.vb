Imports Zuken.E3.HarnessAnalyzer.Shared

''' <summary>
''' From / To is referenced to the startnode as defined from kbl. To/From must be in [0.0,1.0]
''' </summary>
Public Class SegmentDistanceEntry
    Implements IKblDistanceEntry
    Public Property Segment As Segment
    Public Property FixingIds As New List(Of String)
    Public Property Start As Double Implements IKblDistanceEntry.Start
    Public Property [End] As Double Implements IKblDistanceEntry.[End]

    Sub New(seg As Segment, start As Double, [end] As Double)
        If start > [end] Then
            Me.Start = [end]
            Me.End = start
        Else
            Me.Start = start
            Me.End = [end]
        End If
        Me.Segment = seg
    End Sub

    ReadOnly Property ActiveFraction As Double
        Get
            Return Math.Abs(Me.End - Me.Start)
        End Get
    End Property

    ReadOnly Property IsComplete As Boolean
        Get
            Return (Math.Abs(ActiveFraction - 1.0) <= 0.01)
        End Get
    End Property

    Private ReadOnly Property KblId As String Implements IKblDistanceEntry.KblId
        Get
            Return Segment.Id
        End Get
    End Property

    Private ReadOnly Property KblSystemId As String Implements IKblDistanceEntry.KblSystemId
        Get
            Return Segment.SystemId
        End Get
    End Property

End Class
