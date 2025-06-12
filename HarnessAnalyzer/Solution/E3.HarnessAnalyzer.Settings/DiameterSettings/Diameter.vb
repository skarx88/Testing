Imports System.ComponentModel

<Serializable()> _
Public Class Diameter

    Public Property PartNumber As String
    Public Property WireType As String
    Public Property Value As Single

    Public Sub New()
        PartNumber = String.Empty
        WireType = String.Empty
        Value = 0
    End Sub

    Public Sub New(partNo As String, wirType As String, val As Single)
        PartNumber = partNo
        WireType = wirType
        Value = val
    End Sub

End Class


Public Class DiameterList
    Inherits BindingList(Of Diameter)

    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub AddDiameter(diameter As Diameter)
        If (Not ContainsDiameter(diameter)) Then
            Me.Add(diameter)
        End If
    End Sub

    Public Sub CreateDefaultDiameters()
        With Me
            .Add(New Diameter(String.Empty, "FLY 0.5", 2.3))
            .Add(New Diameter(String.Empty, "FLY 0.75", 2.5))
            .Add(New Diameter(String.Empty, "FLY 1", 2.7))
            .Add(New Diameter(String.Empty, "FLY 1.5", 3.0))
            .Add(New Diameter(String.Empty, "FLY 2", 3.3))
            .Add(New Diameter(String.Empty, "FLY 2.5", 3.6))
            .Add(New Diameter(String.Empty, "FLY 3", 3.9))
            .Add(New Diameter(String.Empty, "FLY 4", 4.4))
            .Add(New Diameter(String.Empty, "FLY 6", 5.0))
            .Add(New Diameter(String.Empty, "FLY 10", 6.5))
            .Add(New Diameter(String.Empty, "FLY 16", 8.3))
            .Add(New Diameter(String.Empty, "FLY 25", 10.2))
            .Add(New Diameter(String.Empty, "FLY 35", 10.7))
            .Add(New Diameter(String.Empty, "FLY 50", 13.0))
            .Add(New Diameter(String.Empty, "FLY 70", 15.0))
            .Add(New Diameter(String.Empty, "FLY 95", 16.2))
            .Add(New Diameter(String.Empty, "FLY 120", 19.7))

            .Add(New Diameter(String.Empty, "FLRY-A 0.22", 1.2))
            .Add(New Diameter(String.Empty, "FLRY-A 0.35", 1.3))
            .Add(New Diameter(String.Empty, "FLRY-A 0.5", 1.6))
            .Add(New Diameter(String.Empty, "FLRY-A 0.75", 1.9))
            .Add(New Diameter(String.Empty, "FLRY-A 1", 2.1))
            .Add(New Diameter(String.Empty, "FLRY-A 1.5", 2.4))
            .Add(New Diameter(String.Empty, "FLRY-A 2", 2.8))
            .Add(New Diameter(String.Empty, "FLRY-A 2.5", 3.0))

            .Add(New Diameter(String.Empty, "FLRY-B 0.35", 1.4))
            .Add(New Diameter(String.Empty, "FLRY-B 0.5", 1.6))
            .Add(New Diameter(String.Empty, "FLRY-B 0.75", 1.9))
            .Add(New Diameter(String.Empty, "FLRY-B 1", 2.1))
            .Add(New Diameter(String.Empty, "FLRY-B 1.5", 2.4))
            .Add(New Diameter(String.Empty, "FLRY-B 2", 2.8))
            .Add(New Diameter(String.Empty, "FLRY-B 2.5", 3.0))
            .Add(New Diameter(String.Empty, "FLRY-B 3", 3.2))
            .Add(New Diameter(String.Empty, "FLRY-B 4", 3.7))
            .Add(New Diameter(String.Empty, "FLRY-B 6", 4.3))
            .Add(New Diameter(String.Empty, "FLRY-B 10", 6.0))
            .Add(New Diameter(String.Empty, "FLRY-B 16", 7.9))
            .Add(New Diameter(String.Empty, "FLRY-B 25", 9.4))
        End With
    End Sub

    Public Function ContainsDiameter(diameter As Diameter) As Boolean
        For Each dia As Diameter In Me
            If (dia.PartNumber = diameter.PartNumber) AndAlso (dia.WireType = diameter.WireType) Then Return True
        Next

        Return False
    End Function

    Public Sub DeleteDiameter(diameter As Diameter)
        Me.Remove(diameter)
    End Sub

    Public Function FindDiameterFromPartNumber(partNumber As String) As Diameter
        If (partNumber <> String.Empty) Then
            For Each diameter As Diameter In Me
                If (diameter.PartNumber = partNumber) Then Return diameter
            Next
        End If

        Return Nothing
    End Function

    Public Function FindDiameterFromWireType(wireType As String) As Diameter
        If (wireType <> String.Empty) Then
            For Each diameter As Diameter In Me
                If (diameter.WireType = wireType) Then Return diameter
            Next
        End If

        Return Nothing
    End Function

End Class