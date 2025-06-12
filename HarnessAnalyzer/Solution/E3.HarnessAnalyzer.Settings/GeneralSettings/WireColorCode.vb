Imports System.ComponentModel
Imports System.Xml.Serialization

Public Class WireColorCode

    Public Sub New()
        ColorCode = "BK"
        ColorRGB = Drawing.Color.Black.ToArgb
    End Sub

    Public Sub New(code As String, colRGB As Integer)
        ColorCode = code
        ColorRGB = colRGB
    End Sub


    Public Property ColorCode As String
    Public Property ColorRGB As Integer

    <XmlIgnore>
    Public Property Color As System.Drawing.Color
        Get
            Return System.Drawing.Color.FromArgb(ColorRGB)
        End Get
        Set(value As Drawing.Color)
            ColorRGB = value.ToArgb
        End Set
    End Property

End Class

Public Class WireColorCodeList
    Inherits BindingList(Of WireColorCode)

    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub AddWireColorCode(wireColorCodeMapping As WireColorCode)
        If (Not ContainsWireColorCode(wireColorCodeMapping)) Then
            Me.Add(wireColorCodeMapping)
        End If
    End Sub

    Public Function ContainsWireColorCode(wireColorCode As WireColorCode) As Boolean
        For Each wireColorCodeMapping As WireColorCode In Me
            If (wireColorCodeMapping.ColorCode = wireColorCode.ColorCode) AndAlso (wireColorCodeMapping.ColorRGB = wireColorCode.ColorRGB) Then Return True
        Next

        Return False
    End Function

    Public Sub CreateDefaultColorCodeMapping()
        With Me
            .Add(New WireColorCode("AL", Drawing.Color.LightGray.ToArgb))
            .Add(New WireColorCode("BG", Drawing.Color.Beige.ToArgb))
            .Add(New WireColorCode("BK", Drawing.Color.Black.ToArgb))
            .Add(New WireColorCode("BN", Drawing.Color.Brown.ToArgb))
            .Add(New WireColorCode("BU", Drawing.Color.Blue.ToArgb))
            .Add(New WireColorCode("DW", Drawing.Color.DarkGray.ToArgb))
            .Add(New WireColorCode("GN", Drawing.Color.Green.ToArgb))
            .Add(New WireColorCode("GY", Drawing.Color.Gray.ToArgb))
            .Add(New WireColorCode("NT", Drawing.Color.FromArgb(60, 60, 60).ToArgb))
            .Add(New WireColorCode("OG", Drawing.Color.Orange.ToArgb))
            .Add(New WireColorCode("PK", Drawing.Color.Pink.ToArgb))
            .Add(New WireColorCode("RD", Drawing.Color.Red.ToArgb))
            .Add(New WireColorCode("SC", Drawing.Color.FromArgb(60, 60, 60).ToArgb))
            .Add(New WireColorCode("TR", Drawing.Color.FromArgb(60, 60, 60).ToArgb))
            .Add(New WireColorCode("VT", Drawing.Color.Violet.ToArgb))
            .Add(New WireColorCode("WH", Drawing.Color.FromArgb(240, 240, 240).ToArgb))
            .Add(New WireColorCode("YE", Drawing.Color.Yellow.ToArgb))
        End With
    End Sub

    Public Sub DeleteWireColorCode(wireColorCode As WireColorCode)
        Me.Remove(wireColorCode)
    End Sub

    Public Function FindWireColorCode(colorCode As String) As WireColorCode
        For Each wireColorCodeMapping As WireColorCode In Me
            If (wireColorCodeMapping.ColorCode = colorCode) Then Return wireColorCodeMapping
        Next

        Return Nothing
    End Function

End Class