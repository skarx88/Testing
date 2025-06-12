Imports System.ComponentModel
Imports System.Xml.Serialization

<Serializable()>
<XmlInclude(GetType(ActiveModule))> _
Public Class EntireCarHarness

    Public Property Abbreviation As String
    Public Property PartNumber As String
    Public Property Version As String

    Private _activeModules As New ActiveModuleList

    Public Sub New()
        Abbreviation = String.Empty
        PartNumber = String.Empty
        Version = String.Empty
    End Sub


    <Category("ActiveModules")> _
    Public ReadOnly Property ActiveModules() As ActiveModuleList
        Get
            Return _activeModules
        End Get
    End Property

End Class


Public Class EntireCarHarnessList
    Inherits BindingList(Of EntireCarHarness)

    Public Sub New()
        MyBase.New()
    End Sub

    Public Function FindHarnessFromAbbreviation(abbreviation As String) As EntireCarHarness
        For Each harness As EntireCarHarness In Me
            If (harness.Abbreviation.Trim.Replace(" ", String.Empty) = abbreviation.Trim.Replace(" ", String.Empty)) Then
                Return harness
            End If
        Next

        Return Nothing
    End Function

    Public Function FindHarnessFromPartNumber(partNumber As String) As EntireCarHarness
        For Each harness As EntireCarHarness In Me
            If (harness.PartNumber.Trim.Replace(" ", String.Empty) = partNumber.Trim.Replace(" ", String.Empty)) Then
                Return harness
            End If
        Next

        Return Nothing
    End Function
    Public Function FindHarnessFromPartNumberAndVersion(partNumber As String, version As String) As EntireCarHarness
        For Each harness As EntireCarHarness In Me
            If (harness.PartNumber.Trim.Replace(" ", String.Empty) = partNumber.Trim.Replace(" ", String.Empty) AndAlso harness.Version = version) Then
                Return harness
            End If
        Next

        Return Nothing
    End Function

End Class