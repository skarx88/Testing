Imports System.Drawing
Imports System.Runtime.Serialization

<Serializable>
<KnownType(GetType(Point))>
<KnownType(GetType(Size))>
Public Class ViewportSizeSetting
    Public Sub New(idx As Integer, location As Point, sizePercent As Size)
        Me.Index = idx
        Me.Location = location
        Me.SizePercent = sizePercent
    End Sub

    Property Location As Point
    Property SizePercent As Size
    Property Index As Integer = 0
End Class