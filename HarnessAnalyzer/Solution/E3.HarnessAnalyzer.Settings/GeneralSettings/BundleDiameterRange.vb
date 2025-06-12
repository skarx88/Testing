Imports System.ComponentModel
Imports System.Xml.Serialization

<Serializable()> _
Public Class BundleDiameterRange

    Public Property MinDiameter As Integer
    Public Property MaxDiameter As Integer
    Public Property ColorRGB As Integer

    Public Sub New()
        MinDiameter = 0
        MaxDiameter = 1
        ColorRGB = Drawing.Color.Black.ToArgb
    End Sub

    Public Sub New(minDia As Integer, maxDia As Integer, colRGB As Integer)
        MinDiameter = minDia
        MaxDiameter = maxDia
        ColorRGB = colRGB
    End Sub


    <XmlIgnore>
    Public Property Color() As Drawing.Color
        Get
            Return Drawing.Color.FromArgb(ColorRGB)
        End Get
        Set(value As Drawing.Color)
            ColorRGB = value.ToArgb
        End Set
    End Property

End Class

Public Class BundleDiameterRangeList
    Inherits BindingList(Of BundleDiameterRange)

    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub AddBundleDiameterRange(bundleDiameterRange As BundleDiameterRange)
        If (Not ContainsBundleDiameterRange(bundleDiameterRange)) Then
            Me.Add(bundleDiameterRange)
        End If
    End Sub

    Public Sub CreateDefaultBundleDiameterRanges()
        With Me
            .Add(New BundleDiameterRange(0, 5, Drawing.Color.Blue.ToArgb))
            .Add(New BundleDiameterRange(5, 10, Drawing.Color.Green.ToArgb))
            .Add(New BundleDiameterRange(10, 15, Drawing.Color.Yellow.ToArgb))
            .Add(New BundleDiameterRange(15, 20, Drawing.Color.Orange.ToArgb))
            .Add(New BundleDiameterRange(20, 1000, Drawing.Color.Red.ToArgb))
        End With
    End Sub

    Public Function ContainsBundleDiameterRange(bundleDiameterRange As BundleDiameterRange) As Boolean
        For Each diameterRange As BundleDiameterRange In Me
            If (diameterRange.MinDiameter = bundleDiameterRange.MinDiameter) AndAlso (diameterRange.MaxDiameter = bundleDiameterRange.MaxDiameter) Then Return True
        Next

        Return False
    End Function

    Public Sub DeleteBundleDiameterRange(bundleDiameterRange As BundleDiameterRange)
        Me.Remove(bundleDiameterRange)
    End Sub

    Public Function FindBundleDiameterRange(minDiameter As Integer, maxDiameter As Integer) As BundleDiameterRange
        For Each diameterRange As BundleDiameterRange In Me
            If (diameterRange.MinDiameter = minDiameter) AndAlso (diameterRange.MaxDiameter = maxDiameter) Then Return diameterRange
        Next

        Return Nothing
    End Function

End Class
