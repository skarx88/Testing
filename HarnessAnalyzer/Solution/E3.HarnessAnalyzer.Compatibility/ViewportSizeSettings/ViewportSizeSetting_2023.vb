Imports System.Drawing
Imports System.Runtime.Serialization

<Serializable>
<KnownType(GetType(Point))>
<KnownType(GetType(Size))>
Public Class ViewportSizeSetting_2023
    Public Sub New(idx As Integer, sizePercen As Size, location As Point)
        Me.Index = idx
        Me.Location = location
        Me.SizePercent = sizePercen
    End Sub

    Property Location As Point
    Property SizePercent As Size
    Property Index As Integer = 0
End Class