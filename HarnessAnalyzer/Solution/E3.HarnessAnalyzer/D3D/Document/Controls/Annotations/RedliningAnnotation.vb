Imports devDept.Geometry
Imports Zuken.E3.Lib.Eyeshot.Model

Public Class RedliningAnnotation
    Inherits ImageAnnotation

    Public Sub New()
    End Sub

    Public Sub New(anchorPoint As Point3D, offset As Vector2D, image As Bitmap, redlining As Redlining, Optional fillColor As Drawing.Color = Nothing)
        MyBase.New(anchorPoint, offset, image, redlining.ObjectId)
        Me.Redlining = redlining
        Me.IsImmutable = True
        Me.Classification = redlining.Classification.ToString
    End Sub

    Public Property Classification As String = String.Empty
    Public Property Redlining As Redlining

End Class

