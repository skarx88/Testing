Imports System.ComponentModel
Imports System.IO
Imports System.Runtime.Serialization

<DataContract(Namespace:="Zuken.E3.HarnessAnalyzer.Settings")>
<KnownType(GetType(WeightSettings.WeightsCollection))>
<KnownType(GetType(WeightSettings.MaterialSpecList))>
Public Class WeightSettings

    Private Shared _loadError As Exception = Nothing

    Public Sub New()
        ResetToDefaults()
    End Sub

    <DataMember(Order:=0)> Public Property Name As String
    <Category("Version")>
    <DataMember(Order:=1)> Property Version As String
    <DataMember(Order:=2)> Property Weights As New WeightsCollection
    <DataMember(Order:=3)> Property MaterialSpecField As String
    <DataMember(Order:=4)> Property MaterialSpecs As New MaterialSpecList(Me)
    <DataMember(Order:=5)> Property GenericInsulationWeightParameters As New GenericWeightParameters
    <DataMember(Order:=6)> Property CopperFallBackEnabled As Boolean = True

    Public Shared Function LoadFromFile(fullPath As String) As WeightSettings
        Try
            Return HarnessAnalyzer.Shared.Utilities.ReadXml(Of WeightSettings)(fullPath)
        Catch ex As Exception
#If DEBUG Or CONFIG = "Debug" Then
            Throw
#End If
            _loadError = ex

            Return Nothing
        End Try
    End Function

    Private Sub ResetToDefaults()
        Me.Weights.Clear()
        Me.Version = E3.Lib.DotNet.Expansions.Devices.My.Application.Info.Version.ToString
        Me.Name = "Weight Settings"

        Using sr As New StringReader(My.Resources.DefaultEntriesWeightSettings)
            Using parser As New FileIO.TextFieldParser(sr)

                parser.TextFieldType = FileIO.FieldType.Delimited
                parser.SetDelimiters(";")

                Do While Not parser.EndOfData
                    Dim row As String() = parser.ReadFields
                    Me.Weights.AddNew(row(0), Double.Parse(row(3), Globalization.CultureInfo.InvariantCulture), Double.Parse(row(2), Globalization.CultureInfo.InvariantCulture))
                Loop

                Me.MaterialSpecs.Clear()
                Me.MaterialSpecField = "Abbreviation"
                Me.MaterialSpecs.Add(New MaterialSpec With {.Description = "default", .SpecificWeight = 8.92, .SpecRegEx = "copper"})

                Me.GenericInsulationWeightParameters.GIW_Square = -0.002
                Me.GenericInsulationWeightParameters.GIW_Slope = 1.51
                Me.GenericInsulationWeightParameters.GIW_Offset = 1.2
            End Using
        End Using
    End Sub

    Public Sub SaveTo(fullPath As String)
        Using fs As New FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None)
            HarnessAnalyzer.Shared.Utilities.WriteXml(Me, fs)
            fs.Flush()
        End Using
    End Sub

    <OnDeserialized>
    Private Sub OnDeserialized(ctx As StreamingContext)
        CType(MaterialSpecs, ISettings).Settings = Me
    End Sub

    <IgnoreDataMember>
    Public Shared ReadOnly Property LoadError As Exception
        Get
            Return _loadError
        End Get
    End Property

    Friend Interface ISettings
        Property Settings As WeightSettings
    End Interface

End Class
