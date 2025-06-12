
Imports Zuken.E3.HarnessAnalyzer.Shared.Common

Namespace Checks.Cavities.Views

    Public Class CanvasView
        Implements IDisposable

        Friend Event SelectionChanged(sender As Object, e As EventArgs)

        Private WithEvents _document As DocumentForm
        Private _selected As List(Of String)
        Private _drawings As New List(Of DrawingCanvas)
        Private _groupMapper As New Dictionary(Of String, IDictionary(Of VdSVGGroup, VdSVGGroup))
        Private _disposedValue As Boolean

        Friend Sub New(document As DocumentForm)
            _document = document
            _drawings.AddRange(document.GetAllDrawingCanvas)
            For Each canvas As DrawingCanvas In _drawings
                If canvas IsNot Nothing Then
                    InitCanvas(canvas)
                End If
            Next
        End Sub

        Private Sub InitCanvas(canvas As DrawingCanvas)
            For Each kv As KeyValuePair(Of String, IDictionary(Of VdSVGGroup, VdSVGGroup)) In canvas.GroupMapper
                If Not _groupMapper.ContainsKey(kv.Key) Then
                    _groupMapper.Add(kv.Key, kv.Value)
                Else
                    Dim dic As IDictionary(Of VdSVGGroup, VdSVGGroup) = _groupMapper(kv.Key)
                    For Each kv2 As KeyValuePair(Of VdSVGGroup, VdSVGGroup) In kv.Value
                        If Not dic.ContainsKey(kv2.Key) Then
                            dic.Add(kv2.Key, kv2.Value)
                        End If
                    Next
                End If
            Next
        End Sub

        Private Sub _document_CanvasSelectionChanged(sender As Object, e As EventArgs) Handles _document.CanvasSelectionChanged
            _selected = _document.GetSelectedKblIds.ToList
            OnSelectionChanged(Me, New EventArgs)
        End Sub

        ReadOnly Property GroupMapper As Dictionary(Of String, IDictionary(Of VdSVGGroup, VdSVGGroup))
            Get
                Return _groupMapper
            End Get
        End Property

        ReadOnly Property Selected As IEnumerable(Of String)
            Get
                Return _selected
            End Get
        End Property

        Public Function GetSelectedKblIds() As String()
            Return _document.GetSelectedKblIds
        End Function

        Protected Overridable Sub OnSelectionChanged(sender As Object, e As EventArgs)
            RaiseEvent SelectionChanged(sender, e)
        End Sub

        Public Function GetConnectorClone(connector As Connector_occurrence, target As VectorDraw.Professional.Control.VectorDrawBaseControl) As IEnumerable(Of VdSVGGroup)
            Dim figures As New List(Of VdSVGGroup)
            '   Dim connectorTableAdded As Boolean
            If GroupMapper.ContainsKey(connector.SystemId) Then
                For Each connGrp As VdSVGGroup In GroupMapper(connector.SystemId).Values.ToList
                    If (connGrp.SVGType = SvgType.dimension.ToString) OrElse (connGrp.SVGType = SvgType.ref.ToString) Then
                        Continue For
                    End If

                    'If (svgType = HarnessAnalyzer.SvgType.table.ToString) Then
                    '    If (connectorTableAdded) Then
                    '        Continue For
                    '    Else
                    '        connectorTableAdded = True
                    '    End If
                    'End If

                    figures.Add(GetConnectorClone(connGrp, connector.Id, target))
                Next
            End If
            Return figures
        End Function

        Private Function GetConnectorClone(srcGrp As VdSVGGroup, key As String, target As VectorDraw.Professional.Control.VectorDrawBaseControl) As VdSVGGroup
            Dim clone As VdSVGGroup = DirectCast(srcGrp.Clone(target.ActiveDocument), VdSVGGroup)
            clone.XProperties.Add(PROPERTY_CONNECTORSHORTNAME).PropValue = key
            clone.Lighting = Lighting.Normal 'HINT do not clone highlight
            Return clone
        End Function

        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not _disposedValue Then
                If disposing Then

                    If _drawings IsNot Nothing Then
                        _drawings.Clear()
                    End If

                    If _groupMapper IsNot Nothing Then
                        _groupMapper.Clear()
                    End If

                    If _selected IsNot Nothing Then
                        _selected = Nothing
                    End If
                End If

                _document = Nothing
                _drawings = Nothing
                _selected = Nothing
                _groupMapper = Nothing
                _disposedValue = True
            End If
        End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            ' Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in der Methode "Dispose(disposing As Boolean)" ein.
            Dispose(disposing:=True)
            GC.SuppressFinalize(Me)
        End Sub


    End Class

End Namespace