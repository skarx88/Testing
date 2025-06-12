Imports System.ComponentModel
Imports Zuken.E3.HarnessAnalyzer.Settings

Partial Public Class CalculatedWeightRow

#Region "Browsable"

    ReadOnly Property Cores As BindingList(Of CalculatedWeightRow)
        Get
            Return _coresList
        End Get
    End Property

    Property Id As String
        Get
            Return _id
        End Get
        Private Set(value As String)
            _id = value
            OnPropertyChanged("Id")
        End Set
    End Property

    Property SourceString As String
        Get
            Return _sourceString
        End Get
        Private Set(value As String)
            _sourceString = value
            OnPropertyChanged("SourceString")
        End Set
    End Property

    Property CsaSqMm As Nullable(Of Double)
        Get
            If Not _isCable Then
                Return _csaSqMm
            Else
                Return Nothing
            End If
        End Get
        Set(value As Nullable(Of Double))
            _csaSqMm = value
            If Not _isChangingCsaSqMm Then
                _isChangingCsaSqMm = True
                If AutoUpdateEnabled Then
                    Me.Update()
                End If
                OnPropertyChanged("CsaSqMm")
                _isChangingCsaSqMm = False
            End If
        End Set
    End Property

    Property Conductor As Nullable(Of Double)
        Get
            Return _conductor
        End Get
        Private Set(value As Nullable(Of Double))
            _conductor = value
            OnPropertyChanged("Conductor")
        End Set
    End Property

    Property Total As Nullable(Of Double)
        Get
            Return _total
        End Get
        Private Set(value As Nullable(Of Double))
            _total = value
            OnPropertyChanged("Total")
        End Set
    End Property

    Public Property ResultingLengthValue As Nullable(Of Double)
        Get
            If Me.ResultingLength IsNot Nothing Then
                Return Me.ResultingLength.Value
            Else
                Return Nothing
            End If
        End Get
        Private Set(value As Nullable(Of Double))
            Me.ResultingLength = CreateNewNumericValueWithCurrentLengthUnit(value)
            OnPropertyChanged("ResultingLengthValue")
        End Set
    End Property

    Property LengthValue As Nullable(Of Double)
        Get
            If Me.Length IsNot Nothing Then
                Return Me.Length.Value
            Else
                Return Nothing
            End If
        End Get
        Set(value As Nullable(Of Double))
            Me.Length = CreateNewNumericValueWithCurrentLengthUnit(value)
            OnPropertyChanged("LengthValue")
        End Set
    End Property

    <Browsable(False)>
    ReadOnly Property OriginalLengthValue As Nullable(Of Double)
        Get
            Return _originalLengthValue
        End Get
    End Property

#End Region

#Region "Non-Browsable"

    <Browsable(False)>
    Property ResultingLength As NumericValue
        Get
            Return _resultingLength
        End Get
        Private Set(value As NumericValue)
            _resultingLength = value
            OnPropertyChanged("ResultingLength")
            If AutoUpdateEnabled Then
                Me.Update()
            End If
        End Set
    End Property

    <Browsable(False)>
    Property Source As WeightCalculator.Source
        Get
            Return _source
        End Get
        Private Set(value As WeightCalculator.Source)
            _source = value
            OnPropertyChanged("Source")
        End Set
    End Property

    <Browsable(False)>
    ReadOnly Property HasError As Boolean
        Get
            Return _error IsNot Nothing
        End Get
    End Property

    <DisplayName("Kbl")>
    Property KblMass As Nullable(Of Double)
        Get
            Return _kblMass
        End Get
        Private Set(value As Nullable(Of Double))
            _kblMass = value
            OnPropertyChanged("KblMass")
        End Set
    End Property

    <Browsable(False)>
    Property ListObject As Object
        Get
            Return _listObject
        End Get
        Private Set(value As Object)
            _listObject = value
            OnPropertyChanged("ListObject")
        End Set
    End Property

    <Browsable(False)>
    Property Length As NumericValue
        Get
            Return _length
        End Get
        Set(value As NumericValue)
            _length = value
            If Not _isChangingLength Then
                _isChangingLength = True
                OnPropertyChanged("Length")
                UpdateResultingLength()
                _isChangingLength = False
            End If
        End Set
    End Property

    <Browsable(False)>
    ReadOnly Property IsCable As Boolean
        Get
            Return _isCable
        End Get
    End Property

    <Browsable(False)>
    Property AutoUpdateEnabled As Boolean = True

    <Browsable(False)>
    Property LengthDeltaValue As Nullable(Of Double)
        Get
            Return _lengthDeltaValue
        End Get
        Set(value As Nullable(Of Double))
            _lengthDeltaValue = value
            UpdateResultingLength()
            OnPropertyChanged("LengthDeltaValue")
        End Set
    End Property

#End Region

End Class
