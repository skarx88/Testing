Imports Infragistics.Win.UltraWinDataSource
Imports Infragistics.Win.UltraWinGrid

Public Class CalculateWeightEngine

    Public Event CalculateProgress(sender As Object, e As System.ComponentModel.ProgressChangedEventArgs)
    Public Event CalculationFinished(sender As Object, e As EventArgs)

    Private _kblMapper As KBLMapper
    Private _wires As New Dictionary(Of String, SegmentedWire)
    Private _coresOfCables As New Dictionary(Of String, CorePackage)
    Private _syncCtx As System.Threading.SynchronizationContext

    Public Sub New(selectedRows As IEnumerable, kblMapper As KBLMapper)
        _kblMapper = kblMapper
        _syncCtx = System.Threading.SynchronizationContext.Current
        FillWiresAndCores(selectedRows)
    End Sub

    Private Sub FillWiresAndCores(wireCoreObjs As IEnumerable)
        For Each wireCoreObj As Object In wireCoreObjs
            Dim listObject As Object = If(TypeOf wireCoreObj Is UltraGridRow, DirectCast(DirectCast(wireCoreObj, UltraGridRow).ListObject, UltraDataRow).Tag, wireCoreObj)

            If TypeOf listObject Is Segment Then
                Dim seg As Segment = DirectCast(listObject, Segment)
                For Each wireOrCore As Object In seg.GetWiresAndCores(_kblMapper)
                    AddWireOrCoreFrom(wireOrCore, seg.Physical_length)
                Next
            ElseIf TypeOf listObject Is Special_wire_occurrence Then
                For Each core As Core_occurrence In CType(listObject, Special_wire_occurrence).Core_Occurrence
                    AddWireOrCoreFrom(core)
                Next
            Else
                AddWireOrCoreFrom(listObject)
            End If
        Next
    End Sub

    Private Sub AddWireOrCoreFrom(listObject As Object, Optional segmentedLength As Numerical_value = Nothing)
        Dim wire As Wire_occurrence = TryCast(listObject, Wire_occurrence)
        Dim core As Core_occurrence = TryCast(listObject, Core_occurrence)
        Dim addedSegObject As SegmentedBase = Nothing

        If wire IsNot Nothing Then
            addedSegObject = _wires.AddOrGet(wire.SystemId, New SegmentedWire(wire, _kblMapper))
        ElseIf core IsNot Nothing Then
            Dim cable As Special_wire_occurrence = core.GetCable(_kblMapper)
            Dim newValue As New SegmentedCore(core, _kblMapper)
            Dim pkg As CorePackage = _coresOfCables.AddOrUpdateGet(cable.SystemId, newValue, Function() New CorePackage(cable))
            addedSegObject = newValue
        Else
            Throw New NotImplementedException(String.Format("Unimplemented type ""{0}""", listObject.GetType.Name))
        End If

        If segmentedLength IsNot Nothing Then
            addedSegObject.Add(segmentedLength)
        End If

    End Sub

    Public Async Function CalculateDataAsync() As Task
        Await Task.Factory.StartNew(Sub() Me.CalculateData())
    End Function

    Public Function CalculateData() As CalculateResult
        Dim i As Integer = 0
        Dim total As Integer = _wires.Values.Count + _coresOfCables.Count
        Dim lastPrct As Integer = 0

        Dim rows As New List(Of CalculatedWeightRow)
        With rows
            Try
                For Each segWire As SegmentedWire In _wires.Values
                    .Add(New CalculatedWeightRow(segWire))
                    i += 1
                    Dim current As Integer = CInt((i * 100) / total)
                    If lastPrct <> current Then
                        OnProgressChanged(current)
                        lastPrct = current
                    End If
                Next

                For Each cableAndCores As KeyValuePair(Of String, CorePackage) In _coresOfCables
                    .AddRange(cableAndCores.Value.AsWeightRows(_kblMapper))
                    i += 1
                    Dim current As Integer = CInt((i * 100) / total)
                    If lastPrct <> current Then
                        OnProgressChanged(current)
                        lastPrct = current
                    End If
                Next
                Return New CalculateResult(True, rows)
            Catch ex As Exception
                Return New CalculateResult(False, ex, ex.Message, rows)
            Finally
                RaiseEvent CalculationFinished(Me, New EventArgs)
            End Try
        End With
    End Function

    Private Sub OnProgressChanged(percent As Integer)
        _syncCtx.Send(New System.Threading.SendOrPostCallback(Sub() RaiseEvent CalculateProgress(Me, New System.ComponentModel.ProgressChangedEventArgs(percent, Nothing))), Nothing)
    End Sub

    Public Class CalculateResult

        Public Sub New(success As Boolean, rows As IEnumerable(Of CalculatedWeightRow))
            Me.New(success, Nothing, Nothing, rows)
        End Sub

        Public Sub New(success As Boolean, exception As Exception, message As String, rows As IEnumerable(Of CalculatedWeightRow))
            Me.Success = success
            Me.Exception = exception
            Me.Message = message
            Me.Rows = rows.ToArray
        End Sub

        ReadOnly Property Rows As CalculatedWeightRow()
        ReadOnly Property Success As Boolean
        ReadOnly Property Exception As Exception
        ReadOnly Property Message As String


    End Class


End Class
