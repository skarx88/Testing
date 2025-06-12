Imports System.IO
Imports devDept.Eyeshot.Entities
Imports devDept.Geometry
Imports Zuken.E3.HarnessAnalyzer.Project.Documents
Imports Zuken.E3.Lib.Eyeshot.Model.Converter

Namespace D3D.Document

    <Reflection.Obfuscation(Feature:="renaming", ApplyToMembers:=False, Exclude:=True)> ' needed to avoid resource-file-names obfuscation errors, see issue #5
    Public Class DocumentDesign
        Inherits DesignEEBound
        Implements IToolTipControlExFilter
        Implements IBaseFileProvider

        Private _isHiddenSelection As Boolean
        Private _isResetHiddenEntity As Boolean

        Friend MouseState As New MouseEventArgs(MouseButtons.None, 0, 0, 0, 0)
        Friend ShowEntityTooltipAutomatically As Boolean = True

        Public Sub New()
            MyBase.New
        End Sub

        Public Property HiddenEntities As New List(Of HiddenEntity)

        Public Property IsHiddenSelection As Boolean
            Get
                Return _isHiddenSelection
            End Get
            Set(value As Boolean)
                _isHiddenSelection = value
            End Set
        End Property

        Public Property IsResetHiddenEntity As Boolean
            Get
                Return _isResetHiddenEntity
            End Get
            Set(value As Boolean)
                _isResetHiddenEntity = value
            End Set
        End Property

        Private Event WndProcMessage(sender As Object, e As CancelableMessageEventArgs) Implements IToolTipControlExFilter.WndProcMessage
        Private _wheelZoomEnabled As Boolean = False
        Private _Document As HcvDocument

        Protected Overrides Sub DrawViewport(myParams As DrawSceneParams)
            myParams.Entities = SortEntitiesForTransparency(myParams.Viewport, myParams.Entities)
            MyBase.DrawViewport(myParams)
        End Sub

        Private Sub BeforeHide(e As CancelToolTipExInfoEventArgs) Implements IToolTipControlExFilter.BeforeHide
        End Sub

        Private Sub AfterHide() Implements IToolTipControlExFilter.AfterHide
        End Sub

        <DebuggerNonUserCode>
        Public Property WheelZoomEnabled As Boolean
            Get
                Return _wheelZoomEnabled
            End Get
            Set(value As Boolean)
                _wheelZoomEnabled = value
            End Set
        End Property

        Protected Overrides Sub WndProc(ByRef m As Message)
            If Not CancelableMessageEventArgs.FilterMessage(m, Sub(e) RaiseEvent WndProcMessage(Me, e)) Then
                MyBase.WndProc(m)
            End If
        End Sub

        Private Sub BeforeShown(e As CancelToolTipExInfoEventArgs) Implements IToolTipControlExFilter.BeforeShown
        End Sub

        Private Sub AfterShow() Implements IToolTipControlExFilter.AfterShow
        End Sub

        Protected Overrides Sub OnMouseWheel(e As MouseEventArgs)
            If _wheelZoomEnabled Then
                MyBase.OnMouseWheel(e) 'HINT: this blocking was implmenented to avoid crash of eyeshot during load of 3D entities
            End If
        End Sub

        Friend Sub ResetHiddenEntities()
            For Each hi As HiddenEntity In HiddenEntities
                hi.Reset()
            Next

            HiddenEntities.Clear()
            Me.UpdateVisibleSelection(False)
            Me.Invalidate()
        End Sub

        Private Function GetNearestEntity(m As DocumentDesign, pos As System.Drawing.Point) As Entity
            Dim rect As New Rectangle(pos, New System.Drawing.Size(2, 2))
            Dim indices As Integer() = m.GetAllCrossingEntities(rect, False)
            Dim hits As New Dictionary(Of Integer, Double)

            For Each Index As Integer In indices
                Dim en As Entity = m.Entities.Item(Index)
                Dim d As Double = GetShortestDistance(m, CType(en, Mesh), pos)
                hits.Add(Index, d)
            Next

            Dim mySortedList As List(Of KeyValuePair(Of Integer, Double)) = hits.OrderBy(Function(x) x.Value).ToList()
            Dim myEntities As New List(Of Entity)

            For Each item As KeyValuePair(Of Integer, Double) In mySortedList
                myEntities.Add(m.Entities.Item(item.Key))
            Next

            Return myEntities.First
        End Function

        Friend Function GetSortedListFromCrossingEntities(m As DocumentDesign, rect As Rectangle, Optional SelectableOnly As Boolean = True) As List(Of Entity)
            Dim indices As Integer() = m.GetAllCrossingEntities(rect, SelectableOnly)
            Dim hits As New Dictionary(Of Integer, Double)
            Dim pos As System.Drawing.Point = New System.Drawing.Point(rect.X + CInt(System.Math.Round(0.5 * rect.Width, 0)), rect.Y + CInt(System.Math.Round(0.5 * rect.Height)))

            For Each Index As Integer In indices
                Dim en As Entity = m.Entities.Item(Index)
                Dim d As Double = GetShortestDistance(m, CType(en, Mesh), pos)
                hits.Add(Index, d)
            Next

            Dim mySortedList As List(Of KeyValuePair(Of Integer, Double)) = hits.OrderBy(Function(x) x.Value).ToList()
            Dim myEntities As New List(Of Entity)

            For Each item As KeyValuePair(Of Integer, Double) In mySortedList
                myEntities.Add(m.Entities.Item(item.Key))
            Next

            Return myEntities
        End Function

        Friend Function GetSortedListFromCrossingHiddenEntities(m As DocumentDesign, pos As System.Drawing.Point) As List(Of HiddenEntity)
            Dim result As New List(Of HiddenEntity)
            Dim dx As Integer = CInt(Me.PickBoxSize / 2)
            Dim dy As Integer = CInt(Me.PickBoxSize / 2)

            Dim rect As Rectangle = New Rectangle(New System.Drawing.Point(pos.X - dx, pos.Y - dy), New Size(PickBoxSize, PickBoxSize))
            Dim indices As Integer() = m.GetAllCrossingEntities(rect, False)

            Dim hits As New Dictionary(Of Integer, Double)
            Dim myHiddenentities As List(Of Entity) = HiddenEntities.Select(Function(h) h.Entity).ToList()
            For Each Index As Integer In indices
                Dim en As Entity = m.Entities.Item(Index)
                If myHiddenentities.Contains(en) Then
                    Dim d As Double = GetShortestDistance(m, CType(en, Mesh), pos)
                    hits.Add(Index, d)
                End If
            Next

            Dim mySortedList As List(Of KeyValuePair(Of Integer, Double)) = hits.OrderBy(Function(x) x.Value).ToList()
            Dim myEntities As New List(Of Entity)

            For Each item As KeyValuePair(Of Integer, Double) In mySortedList
                Dim myEntity As Entity = m.Entities.Item(item.Key)
                Dim myHidden As HiddenEntity = HiddenEntities.Where(Function(h) h.Entity Is myEntity).FirstOrDefault()
                If myHidden IsNot Nothing Then
                    result.Add(myHidden)
                End If
            Next

            Return result
        End Function

        Private Function GetShortestDistance(m As DocumentDesign, mesh As Mesh, MousePosition As System.Drawing.Point) As Double
            Dim distance As Double = Double.MaxValue
            Dim p0 As Point3D = m.ActiveViewport.Camera.Location
            Dim p As Point3D = m.ActiveViewport.ScreenToWorld(MousePosition)
            If p IsNot Nothing Then
                Dim line As Line = New Line(p0, p)
                line.Scale(p0, m.ActiveViewport.Camera.Far)
                Dim seg As devDept.Geometry.Segment3D = New devDept.Geometry.Segment3D(line.StartPoint, line.EndPoint)
                Dim hits As IList(Of Entities.HitTriangle) = mesh.FindClosestTriangle(Nothing, seg)
                For Each hit As Entities.HitTriangle In hits
                    Dim d As Double = Point3D.Distance(p0, hit.IntersectionPoint)
                    If d < distance Then
                        distance = d
                    End If
                Next
            End If
            Return distance
        End Function

        Private Sub DocumentModel_BeforeProcessSelection(sender As Object, selectionBox As Rectangle, ByRef cancelSelection As Boolean) Handles Me.BeforeProcessSelection
            If MouseState.Button = MouseButtons.Left Then

                If IsHiddenSelection Then
                    Dim sorted As List(Of Entity) = GetSortedListFromCrossingEntities(Me, selectionBox, True)
                    If sorted.Count > 0 Then
                        HiddenEntities.Add(New HiddenEntity(sorted(0)))
                        cancelSelection = True
                        Me.UpdateVisibleSelection(False)
                        Me.Invalidate()
                    End If
                    IsHiddenSelection = False
                End If
            End If
        End Sub

        Private Sub DocumentModel_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
            If e.Control And e.KeyCode = Keys.S Then
                IsHiddenSelection = True
                e.Handled = True
            End If
            If e.Control And e.KeyCode = Keys.E Then
                ResetHiddenEntities()
                e.Handled = True
            End If
            If e.Control And e.KeyCode = Keys.D Then
                e.Handled = True
                If HiddenEntities.Count > 0 Then
                    IsResetHiddenEntity = True
                Else
                    IsResetHiddenEntity = False
                End If

            End If
        End Sub

        Private Sub DocumentModel_MouseDown(sender As Object, e As MouseEventArgs) Handles Me.MouseDown
            MouseState = e
            If IsResetHiddenEntity Then
                Dim sorted As List(Of HiddenEntity) = GetSortedListFromCrossingHiddenEntities(Me, e.Location)
                If sorted.Count > 0 Then
                    sorted.Last.Reset()
                    Me.UpdateVisibleSelection(False)
                    Me.Invalidate()
                End If
                Me.Entities.GetWorkspace
            End If
        End Sub

        Private Sub DocumentModel_KeyUp(sender As Object, e As KeyEventArgs) Handles Me.KeyUp
            IsHiddenSelection = False
            IsResetHiddenEntity = False
        End Sub

        Public Property Document As HcvDocument
            Get
                Return _Document
            End Get
            Set
                If Value IsNot Nothing AndAlso Not Value.Equals(_Document) Then
                    _Document = Value
                    Me.EEModel = _Document.Model
                ElseIf Value Is Nothing Then
                    _Document = Nothing
                    Me.EEModel = Nothing
                End If
            End Set
        End Property

        Private ReadOnly Property File As IBaseFile Implements IBaseFileProvider.File
            Get
                Return Me.Document
            End Get
        End Property

        Protected Overrides Sub Dispose(disposing As Boolean)
            MyBase.Dispose(disposing)
            _Document = Nothing
        End Sub

    End Class

End Namespace
