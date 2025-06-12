Public Class CorePackage
    Inherits Collections.ObjectModel.Collection(Of SegmentedCore)

    Private _owningCable As Special_wire_occurrence

    Public Sub New(owningCable As Special_wire_occurrence)
        _owningCable = owningCable
    End Sub

    Public ReadOnly Property OwningCable As Special_wire_occurrence
        Get
            Return _owningCable
        End Get
    End Property

    Public ReadOnly Property IsComplete As Boolean
        Get
            If Flattern.Count > CableCoresCount Then
#If DEBUG Then
                Throw New Exception($"Warning! {NameOf(CorePackage)} ({Id}) has more different cores ({Flattern.Count}) added than it's cable  has ({CableCoresCount})! (unexpected, please check! Hint: this warning only comes in debug mode)")
#End If
            End If
            Return CableCoresCount = Flattern.Count
        End Get
    End Property

    Public ReadOnly Property Id As String
        Get
            Return _owningCable?.Special_wire_id
        End Get
    End Property

    Private ReadOnly Property CableCoresCount As Integer
        Get
            Return _owningCable.Core_occurrence.Length
        End Get
    End Property

    ''' <summary>
    ''' Retrieves the biggest core length (we assume that the cable is so long as the longest core)
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetMaxCoreLength() As Numerical_value
        Return Me.Flattern.Where(Function(sc) sc.Length IsNot Nothing).Select(Function(sc) sc.Length).OrderByDescending(Function(ln) ln.Value_component).FirstOrDefault
    End Function

    Public Function Flattern() As IEnumerable(Of SegmentedCore)
        Dim list As New List(Of SegmentedCoreGroup)
        For Each grp As IGrouping(Of String, SegmentedCore) In Me.GroupBy(Function(segCore) segCore.Id)
            Dim segCoreGrp As New SegmentedCoreGroup(grp)
            list.Add(segCoreGrp)
        Next
        Return list
    End Function

    Private Function GetCoresAsWeightRows() As List(Of CalculatedWeightRow)
        Dim list As New List(Of CalculatedWeightRow)
        For Each core As SegmentedCore In Me.Flattern
            list.Add(New CalculatedWeightRow(core))
        Next
        Return list
    End Function

    ''' <summary>
    ''' Creates weight rows with the maximum core length used as length
    ''' </summary>
    ''' <param name="kblMapper"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Friend Function AsWeightRows(kblMapper As KBLMapper) As List(Of CalculatedWeightRow)
        Dim list As New List(Of CalculatedWeightRow)
        If IsComplete Then
            list.Add(New CalculatedWeightRow(Me, kblMapper, GetMaxCoreLength.Extract(kblMapper)))
        Else
            list.AddRange(GetCoresAsWeightRows())
        End If
        Return list
    End Function

End Class