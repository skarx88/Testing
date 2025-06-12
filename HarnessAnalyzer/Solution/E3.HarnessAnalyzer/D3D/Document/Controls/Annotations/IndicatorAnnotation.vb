
Imports devDept.Geometry
Imports Zuken.E3.Lib.Eyeshot.Model

Public Class IndicatorAnnotation
    Inherits TextAndImageAnnotation

    Public Sub New()
    End Sub

    Public Sub New(entity As BaseModelEntity, anchorPoint As Point3D, offset As Vector2D, text As String, Optional fillcolor As Drawing.Color = Nothing, Optional isref As Boolean = False)
        MyBase.New(text, String.Empty)
        Me.Offset = offset
        Me.AnchorPoint = anchorPoint
        Me.IsRef = isref
        If fillcolor <> Nothing Then
            Me.FillColor = fillcolor
        Else
            Me.FillColor = Color.Transparent
        End If
        Me.IsImmutable = True
        Me.Entity = entity
    End Sub

    Public Sub New(anchorPoint As Point3D, offset As Vector2D, text As String, image As Image, Optional fillcolor As Drawing.Color = Nothing)
        MyBase.New(text, image, String.Empty)
        Me.Offset = offset
        Me.AnchorPoint = anchorPoint
        If fillcolor <> Nothing Then
            Me.FillColor = fillcolor
        Else
            Me.FillColor = Color.Transparent
        End If
        Me.IsImmutable = True

    End Sub

    Public Entity As BaseModelEntity
    Public Property IsHidden As Boolean
    Public Property IsRef As Boolean = False
    Public Property SegmentId As Guid
    Public Property OrigId As Guid
    Public Property Childs As New List(Of IndicatorAnnotation)

End Class