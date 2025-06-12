Imports Zuken.E3.Lib.Converter.Unit

Public MustInherit Class SegmentedBase
    Inherits System.Collections.ObjectModel.Collection(Of Numerical_value)

    Private _lengthValue As Double = 0
    Private _lengthUnit As String = String.Empty
    Private _kblMapper As KBLMapper

    Protected Sub New(mapper As KBLMapper)
        _kblMapper = mapper
    End Sub

    ReadOnly Property Length As Numerical_value
        Get
            If Me.Count = 0 Then
                Return GetObjectLength()
            End If
            Return New Numerical_value() With {.Unit_component = _lengthUnit, .Value_component = _lengthValue}
        End Get
    End Property

    Protected MustOverride Function GetObjectLength() As Numerical_value

    ReadOnly Property Mapper As KBLMapper
        Get
            Return _kblMapper
        End Get
    End Property

    Protected Overrides Sub ClearItems()
        MyBase.ClearItems()
        _lengthUnit = Nothing
        _lengthValue = 0
    End Sub

    Protected Overrides Sub SetItem(index As Integer, item As Numerical_value)
        Throw New NotSupportedException(String.Format("setting an item in {0}!", Me.GetType.Name))
    End Sub

    Protected Overrides Sub RemoveItem(index As Integer)
        Dim item As Numerical_value = Me(index)
        MyBase.RemoveItem(index)

        If Me.Count = 0 Then
            _lengthUnit = Nothing
            _lengthValue = 0
        Else
            RemoveLength(item)
        End If
    End Sub

    Protected Overrides Sub InsertItem(index As Integer, item As Numerical_value)
        MyBase.InsertItem(index, item)
        AddLength(item)
    End Sub

    Private Function GetCurrCalcUnit() As CalcUnit
        If Not String.IsNullOrEmpty(_lengthUnit) Then
            Dim currUnit As Unit = _kblMapper.KBLUnitMapper(_lengthUnit)
            Return CalcUnit.CreateFrom(KblUnitConverter.GetUnitInfo(currUnit))
        End If
        Return Nothing
    End Function

    Private Function ConvertToCurrentUnitValue(item As Numerical_value) As Nullable(Of Double)
        If item IsNot Nothing Then
            Dim currUnit As CalcUnit = GetCurrCalcUnit()
            Dim toUnitValue As Nullable(Of Double) = UnitConverter.Convert(item.Value_component, item.GetCalcUnit(_kblMapper), currUnit, True)
            Return toUnitValue
        End If
        Return 0
    End Function

    Private Sub AddLength(item As Numerical_value)
        If String.IsNullOrEmpty(_lengthUnit) Then
            _lengthUnit = item.Unit_component
            _lengthValue = item.Value_component
        Else
            If _lengthUnit <> item.Unit_component Then
                ' convert the added length to the current unit length (before adding it)
                Dim toUnitValue As Nullable(Of Double) = ConvertToCurrentUnitValue(item)
                If toUnitValue.HasValue Then
                    _lengthValue += toUnitValue.Value
                End If
            Else
                _lengthValue += item.Value_component
            End If
        End If
    End Sub

    Private Sub RemoveLength(item As Numerical_value)
        If _lengthUnit <> item.Unit_component Then
            ' convert the removed length to the current unit length (before recuding the length value)
            Dim currUnit As CalcUnit = GetCurrCalcUnit()
            Dim toUnitValue As Nullable(Of Double) = ConvertToCurrentUnitValue(item)
            If toUnitValue.HasValue Then
                _lengthValue -= toUnitValue.Value
            End If
        Else
            _lengthValue -= item.Value_component
        End If
    End Sub

    MustOverride ReadOnly Property Id As String

    Public Overrides Function ToString() As String
        Return String.Format("{0};{1}", Me.Id, MyBase.ToString)
    End Function

End Class