Imports System.ComponentModel
Imports System.Runtime.Serialization

Namespace QualityStamping

    <DataContract(Namespace:="Zuken.E3.HarnessAnalyzer.QualityStamping")>
    <KnownType(GetType(ObjectReferenceList))>
    Public Class QMStamp

        <DataMember> Public Property Id As String
        <DataMember> Public Property RefNo As Nullable(Of Integer)
        <DataMember> Public Property Specification As String
        <DataMember> Public Property CreatedBy As String
        <DataMember> Public Property DateOfCreation As Date
        <DataMember> Public Property CheckResult As String
        <DataMember> Public Property CheckComment As String
        <DataMember> Public Property Passed As Boolean
        <DataMember> Public Property CheckedBy As String
        <DataMember> Public Property DateOfCheck As Date
        <DataMember> Private _objectReferences As New ObjectReferenceList(Me)

        Public Sub New()
            Id = Guid.NewGuid.ToString

            Specification = String.Empty
            CreatedBy = System.Security.Principal.WindowsIdentity.GetCurrent.Name
            DateOfCreation = Now
            CheckResult = String.Empty
            CheckComment = String.Empty
            Passed = False
            CheckedBy = String.Empty
            DateOfCheck = DateTime.MinValue
        End Sub

        Public Sub New(referenceNo As Nullable(Of Integer), spec As String)
            Id = Guid.NewGuid.ToString

            RefNo = referenceNo
            Specification = spec
            CreatedBy = System.Security.Principal.WindowsIdentity.GetCurrent.Name
            DateOfCreation = Now
            CheckResult = String.Empty
            CheckComment = String.Empty
            Passed = False
            CheckedBy = String.Empty
            DateOfCheck = DateTime.MinValue
        End Sub

        <Category("ObjectReferences")> _
        Public Property ObjectReferences() As ObjectReferenceList
            Get
                If _objectReferences Is Nothing Then _objectReferences = New ObjectReferenceList(Me)

                Return _objectReferences
            End Get
            Set(value As ObjectReferenceList)
                _objectReferences = value
            End Set
        End Property

        <OnDeserialized>
        Private Sub OnDeserialized(info As StreamingContext)
            Me._objectReferences.Owner = Me
        End Sub

        Property List As QMStampList

    End Class


    <CollectionDataContract(Namespace:="Zuken.E3.HarnessAnalyzer.QualityStamping")>
    Public Class QMStampList
        Inherits BindingList(Of QMStamp)

        Public Sub New()
            MyBase.New()
        End Sub

        Protected Overrides Sub RemoveItem(index As Integer)
            Dim item As QMStamp = Me(index)
            MyBase.RemoveItem(index)
            item.List = Nothing
        End Sub

        Protected Overrides Sub InsertItem(index As Integer, item As QMStamp)
            If (Not Contains(item)) Then
                If item.RefNo Is Nothing Then item.RefNo = GetNextAvailableRefNo()

                MyBase.InsertItem(index, item)
                item.List = Me
            End If
        End Sub

        Public Shadows Function AddNew(refNo As Integer, spec As String) As QMStamp
            If (FindFirstByRefNo(refNo) Is Nothing) Then
                Dim newStamp As New QMStamp(refNo, spec)
                Me.Add(newStamp)

                Return newStamp
            End If

            Return Nothing
        End Function

        Public Shadows Function AddNew(Optional spec As String = "") As QMStamp
            Dim newStamp As New QMStamp(Nothing, spec)
            Me.Add(newStamp)

            Return newStamp
        End Function

        Public Shadows Function Contains(qmStamp As QMStamp) As Boolean
            If (Where(Function(stamp) stamp.Id = qmStamp.Id).Any()) Then
                Return True
            End If
            Return False
        End Function

        Public Function FindFirstByRefNo(refNo As Integer) As QMStamp
            Return Me.Where(Function(stamp) stamp.RefNo.HasValue AndAlso stamp.RefNo.Value = refNo).FirstOrDefault
        End Function

        Private Function GetNextAvailableRefNo() As Integer
            If (Me.Count <> 0) Then
                Return Me.Where(Function(stamp) stamp.RefNo.HasValue).Max(Function(stamp) stamp.RefNo.Value) + 1
            Else
                Return 1
            End If
        End Function

        Public Shadows Function Contains(kblId As String) As Boolean
            For Each stamp As QMStamp In Me
                For Each objRef As ObjectReference In stamp.ObjectReferences
                    If (objRef.KblId = kblId) Then Return True
                Next
            Next

            Return False
        End Function

        Public Function GetAllByKblId(kblId As String) As QMStamp()
            Dim foundStamps As New List(Of QMStamp)

            For Each stamp As QMStamp In Me
                For Each objRef As ObjectReference In stamp.ObjectReferences
                    If (objRef.KblId = kblId) Then foundStamps.Add(stamp)
                Next
            Next

            Return foundStamps.ToArray
        End Function

        Property QMStamps As QMStamps

    End Class

End Namespace