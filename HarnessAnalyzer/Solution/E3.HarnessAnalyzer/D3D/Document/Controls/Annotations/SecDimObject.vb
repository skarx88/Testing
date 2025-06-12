Imports System.ComponentModel
Imports devDept.Eyeshot.Entities
Imports devDept.Eyeshot.Workspace
Imports devDept.Geometry
Imports devDept.Serialization
Imports Zuken.E3.HarnessAnalyzer.D3D.Document
Imports Zuken.E3.Lib.Eyeshot.Model


Public Class SecDimObject
    Inherits IndicatorAnnotation
    Private _d3d As devDept.Eyeshot.Workspace
    Private _entity As BaseModelEntity
    Private Shadows _isHidden As Boolean
    Public Markers As New List(Of Entity)
    Public SubCurve As CurveEx

    Public Overloads Property IsHidden As Boolean
        Get
            If _d3d IsNot Nothing Then _isHidden = CType(_d3d, DocumentDesign).HiddenEntities.Select(Function(e) e.Entity).ToList.Contains(_entity)
            Return _isHidden
        End Get
        Set(value As Boolean)
            _isHidden = value
            If _isHidden AndAlso _d3d IsNot Nothing Then
                If _d3d.TempEntities.Contains(CType(SubCurve, Entity)) Then
                    _d3d.TempEntities.Remove(CType(SubCurve, Entity))
                    For Each lbl As devDept.Eyeshot.Labels.Label In Me.Labels(AnnotationBase.ViewportType.All)
                        lbl.Selected = False
                    Next
                End If
            End If
        End Set
    End Property

    Public Sub New()

    End Sub
    Public Sub New(d3d As DocumentDesign, en As BaseModelEntity, anchorpoint As Point3D, offset As devDept.Geometry.Vector2D, text As String, subcurve As CurveEx)
        MyBase.New(en, anchorpoint, offset, text)

        en.Annotations.Add(Me)
        _entity = en
        Me.FillColor = Color.FromArgb(200, Color.GreenYellow)
        Me.BorderColor = Color.LightGray
        Me.SubCurve = subcurve
        Me.SubCurve.Color = Color.Yellow
        Me.SubCurve.LineWeight = 4
        Me._d3d = d3d
        IsHidden = d3d.HiddenEntities.Select(Function(e) e.Entity).ToList.Contains(en)
        AddMarkers()

        For Each lbl As devDept.Eyeshot.Labels.Label In Me.Labels(ViewportType.All)
            lbl.LabelData = subcurve
        Next
    End Sub

    Private Sub AddMarkers()

        Dim marker1 As New devDept.Eyeshot.Entities.Point(SubCurve.StartPoint, 10)
        marker1.Color = Color.Yellow
        marker1.Regen(0.1)
        Dim marker2 As New devDept.Eyeshot.Entities.Point(SubCurve.EndPoint, 10)
        marker2.Color = Color.Yellow
        marker2.Regen(0.1)
        Me.Markers.Add(marker1)
        Me.Markers.Add(marker2)

        If Me.Visible AndAlso Not IsHidden Then
            If Not _d3d.TempEntities.Contains(marker1) Then _d3d.TempEntities.Add(marker1)
            If Not _d3d.TempEntities.Contains(marker2) Then _d3d.TempEntities.Add(marker2)
        End If
    End Sub
    Public Sub Clear()
        Dim model As devDept.Eyeshot.Design = CType(_d3d, devDept.Eyeshot.Design)
        If _entity IsNot Nothing AndAlso _entity.Annotations IsNot Nothing AndAlso _entity.Annotations.Contains(Me) Then _entity.Annotations.Remove(Me)
        For Each lbl As devDept.Eyeshot.Labels.Label In Me.Labels(AnnotationBase.ViewportType.All)
            model.ActiveViewport.Labels.Remove(lbl)
            lbl = Nothing
        Next
        HideMarkers()
        Markers.Clear()
        _d3d.TempEntities.Remove(CType(SubCurve, Entity))
        SubCurve = Nothing
    End Sub
    Private Sub HideMarkers()
        For Each marker As Entity In Me.Markers
            marker.Regen(0.1)
            _d3d.TempEntities.Remove(marker)
        Next
    End Sub
    Public Sub Hide()
        For Each lbl As devDept.Eyeshot.Labels.Label In Me.Labels(AnnotationBase.ViewportType.All)
            lbl.Visible = False
            lbl.Selected = False
        Next
        HideMarkers()
    End Sub

    Private Sub ShowMarker()
        For Each marker As Entity In Me.Markers
            _d3d.TempEntities.Add(marker)
            marker.Regen(0.1)
        Next
    End Sub
    Private Sub SecDimObject_PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Handles Me.PropertyChanged
        If TypeOf sender Is SecDimObject AndAlso e.PropertyName = NameOf(Visible) Then
            If CType(sender, SecDimObject).Entity.Visible Then
                If Not IsHidden Then ShowMarker()
                IsHidden = False
            Else
                HideMarkers()
                IsHidden = True
            End If
            If _d3d IsNot Nothing Then CType(_d3d, devDept.Eyeshot.Design).Invalidate()
        End If
    End Sub

End Class
