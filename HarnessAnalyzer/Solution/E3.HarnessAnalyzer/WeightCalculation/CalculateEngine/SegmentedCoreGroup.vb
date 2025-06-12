
Public Class SegmentedCoreGroup
    Inherits SegmentedCore

    Private _cores As New List(Of SegmentedCore)

    Public Sub New(cores As IEnumerable(Of SegmentedCore))
        MyBase.New(cores.First.Object, cores.First.Mapper)
        For Each segCore As SegmentedCore In cores.Distinct
            Me.Add(segCore)
        Next
    End Sub

    Public Sub New(kblMapper As KBLMapper)
        MyBase.New(Nothing, kblMapper)
    End Sub

    Public Shadows Sub Add(core As SegmentedCore)
        Dim unitComp As String = _cores.Select(Function(c) c.Length.Unit_component).Distinct.SingleOrDefault
        If unitComp IsNot Nothing AndAlso core.Length.Unit_component <> unitComp Then
            Throw New ArgumentException($"Can't add cores with different unit_components ({unitComp}<>{core.Length.Unit_component})")
        End If

        _cores.Add(core)
    End Sub

    Public Shadows Function Remove(core As SegmentedCore) As Boolean
        Return _cores.Remove(core)
    End Function

    Public Shadows Sub Clear()
        _cores.Clear()
    End Sub

    Protected Overrides Function GetObjectLength() As Numerical_value
        Dim numValue As Numerical_value = Nothing
        For Each l As Numerical_value In _cores.Select(Function(c) c.Length)
            If l IsNot Nothing AndAlso Not Double.IsNaN(l.Value_component) Then
                If numValue Is Nothing Then
                    numValue = New Numerical_value
                    numValue.Value_component = 0
                    numValue.Unit_component = l.Unit_component
                End If
                numValue.Value_component += l.Value_component
            End If
        Next
        Return numValue
    End Function

End Class