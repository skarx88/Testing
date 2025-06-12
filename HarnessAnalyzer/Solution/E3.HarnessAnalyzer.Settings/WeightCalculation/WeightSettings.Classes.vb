Imports System.Reflection
Imports System.Runtime.Serialization
Imports System.Text.RegularExpressions
Imports Zuken.E3.Lib.Schema.Kbl

Partial Public Class WeightSettings

    <DataContract(Name:="Weight", Namespace:="Zuken.E3.HarnessAnalyzer")>
    Public Class Weight
        <DataMember(Order:=0)> Property PartNumber As String = String.Empty
        <DataMember(Order:=1)> Property WireType As String = String.Empty
        ''' <summary>
        ''' Weight of the complete part in g/m
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember(Order:=2)> Property Total As Double
        ''' <summary>
        ''' Weight of the Conductor in g/m
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember(Order:=3)> Property Conductor As Double

        Public Overrides Function ToString() As String
            Return String.Format("Typ:{0};PartNr:{1};Total:{2};Conductor:{3}", Me.WireType, PartNumber, Total, Conductor)
        End Function

    End Class

    Public Class WeightsCollection
        Inherits System.Collections.ObjectModel.Collection(Of Weight)

        Private _byPartNumber As New Dictionary(Of String, List(Of Integer))
        Private _byWireType As New Dictionary(Of String, List(Of Integer))

        Public Function AddNew(wireType As String, conductor As Double, total As Double) As Weight
            Dim nW As New Weight
            With nW
                .WireType = wireType
                .Conductor = conductor
                .Total = total
                .PartNumber = String.Empty
            End With
            Me.Add(nW)
            Return nW
        End Function

        Public Function GetByPartNumber(partNumber As String) As List(Of Weight)
            Dim list As New List(Of Weight)
            If Not String.IsNullOrEmpty(partNumber) AndAlso _byPartNumber.ContainsKey(partNumber) Then
                For Each weightItemIdx As Integer In _byPartNumber(partNumber)
                    list.Add(Me(weightItemIdx))
                Next
            End If
            Return list
        End Function

        Public Function GetByWireType(wireType As String) As List(Of Weight)
            Dim list As New List(Of Weight)
            If Not String.IsNullOrEmpty(wireType) AndAlso _byWireType.ContainsKey(wireType) Then
                For Each weightItemIdx As Integer In _byWireType(wireType)
                    list.Add(Me(weightItemIdx))
                Next
            End If
            Return list
        End Function

        Protected Overrides Sub InsertItem(index As Integer, item As Weight)
            MyBase.InsertItem(index, item)
            _byPartNumber.AddOrUpdate(item.PartNumber, index)
            _byWireType.AddOrUpdate(item.WireType, index)
        End Sub

        Protected Overrides Sub RemoveItem(index As Integer)
            Dim removeItem As Weight = Me(index)
            MyBase.RemoveItem(index)
            _byPartNumber.TryRemove(removeItem.PartNumber, index)
            _byWireType.TryRemove(removeItem.WireType, index)
        End Sub

        Protected Overrides Sub ClearItems()
            MyBase.ClearItems()
            _byPartNumber.Clear()
            _byWireType.Clear()
        End Sub

        Protected Overrides Sub SetItem(index As Integer, item As Weight)
            Throw New NotSupportedException("Setting item is not supported!")
        End Sub

    End Class

    <DataContract(Name:="MaterialSpec", Namespace:="Zuken.E3.HarnessAnalyzer")>
    Public Class MaterialSpec
        <DataMember> Property Description As String
        <DataMember(Name:="SpecRegex")> Property SpecRegEx As String
        ''' <summary>
        ''' Weight of the Material in g/cm³
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember> Property SpecificWeight As Double
        ''' <summary>
        ''' Resistivity in [ 10-8 Ωm]
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember> Property Resistivity As Double
        ''' <summary>
        ''' TemperatureCoefficient in [10-3/K]
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember> Property TemperatureCoefficient As Double
    End Class

    <DataContract(Name:="GenericWeightParameters", Namespace:="Zuken.E3.HarnessAnalyzer")>
    Public Class GenericWeightParameters
        <DataMember> Property GIW_Offset As Double
        <DataMember> Property GIW_Slope As Double
        <DataMember> Property GIW_Square As Double
    End Class

    Public Class MaterialSpecList
        Inherits List(Of MaterialSpec)
        Implements ISettings

        Private _settings As WeightSettings

        Friend Sub New()
        End Sub

        Public Sub New(owner As WeightSettings)
            _settings = owner
        End Sub


        ReadOnly Property Settings As WeightSettings
            Get
                Return _settings
            End Get
        End Property

        Friend Function FindMaterialSpecsFor(generalWire As General_wire) As List(Of MaterialSpec)
            Dim lst As New List(Of MaterialSpec)

            If generalWire IsNot Nothing Then
                If Not String.IsNullOrEmpty(Settings.MaterialSpecField) Then
                    Dim prop As PropertyInfo = generalWire.GetType.GetProperty(Settings.MaterialSpecField)
                    If prop Is Nothing Then
                        Throw New MaterialFieldNotFoundException(String.Format("Material specification field ""{0}"" not found in part ""{1}""", Settings.MaterialSpecField, generalWire.GetType.Name), Settings.MaterialSpecField, generalWire, Nothing)
                    End If

                    Dim mSpecValue As Object = prop.GetValue(generalWire)
                    If mSpecValue IsNot Nothing Then
                        For Each spec As WeightSettings.MaterialSpec In Me
                            Dim isMatch As Boolean
                            Try
                                isMatch = Regex.IsMatch(mSpecValue.ToString, spec.SpecRegEx)
                            Catch ex As ArgumentException
                                Throw New RegexException(ex.Message, spec.SpecRegEx, spec, ex)
                            End Try

                            If isMatch Then
                                lst.Add(spec)
                            End If
                        Next
                    End If
                End If
            Else
                Throw New ArgumentException("Please define an occurrence or part to search the material specification for!", "object")
            End If

            Return lst
        End Function

        'Public Function FindMaterialSpecsFor([object] As Object) As List(Of MaterialSpec)
        '    Dim lst As New List(Of MaterialSpec)

        '    Dim partId As String = Part.TryGetPartId([object])
        '    If Not String.IsNullOrWhiteSpace(partId) Then
        '        Dim IsInAnyDocument As Boolean = False
        '        For Each doc As DocumentForm In My.Application.MainForm.GetAllDocuments.Values
        '            If doc._kblMapper.PartMapper.ContainsKey(partId) Then
        '                IsInAnyDocument = True
        '                [object] = doc._kblMapper.PartMapper(partId)
        '                Exit For
        '            End If
        '        Next

        '        If Not IsInAnyDocument Then
        '            Throw New NullReferenceException(String.Format("Could not find part for ""{0}"" at any open document", [object].GetType.Name))
        '        End If
        '    End If

        '    If [object] IsNot Nothing Then
        '        With My.Application.MainForm.WeightSettings
        '            If Not String.IsNullOrEmpty(.MaterialSpecField) Then
        '                Dim prop As PropertyInfo = [object].GetType.GetProperty(.MaterialSpecField)
        '                If prop Is Nothing Then
        '                    Throw New MaterialFieldNotFoundException(String.Format("Material specification field ""{0}"" not found in part ""{1}""", .MaterialSpecField, [object].GetType.Name), .MaterialSpecField, [object], Nothing)
        '                End If

        '                Dim mSpecValue As Object = prop.GetValue([object])
        '                If mSpecValue IsNot Nothing Then
        '                    For Each spec As WeightSettings.MaterialSpec In .MaterialSpecs
        '                        Dim isMatch As Boolean
        '                        Try
        '                            isMatch = Regex.IsMatch(mSpecValue.ToString, spec.SpecRegEx)
        '                        Catch ex As ArgumentException
        '                            Throw New RegexException(ex.Message, spec.SpecRegEx, spec, ex)
        '                        End Try

        '                        If isMatch Then
        '                            lst.Add(spec)
        '                        End If
        '                    Next
        '                End If
        '            End If
        '        End With
        '    Else
        '        Throw New ArgumentException("Please define an occurrence or part to search the material specification for!", "object")
        '    End If

        '    Return lst
        'End Function

        Private Property I_Settings As WeightSettings Implements ISettings.Settings
            Get
                Return _settings
            End Get
            Set(value As WeightSettings)
                _settings = value
            End Set
        End Property
    End Class

End Class
