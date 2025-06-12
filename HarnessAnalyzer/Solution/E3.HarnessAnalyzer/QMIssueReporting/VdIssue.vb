Imports VectorDraw.Geometry
Imports VectorDraw.Professional.vdFigures
Imports VectorDraw.Professional.vdObjects

Namespace IssueReporting

    Public Class VdIssue
        Inherits VdSVGGroup

        Private Const BORDER_WIDTH As Single = 2
        Private Const LINE_SPACE As Single = 1
        Private Const MAX_TEXTWIDTH As Single = 50
        Private Const ARROW_LENGTH As Double = 5
        Private Const ARROW_ANGLE As Double = 0.2

        Private _fillColor As Color = Color.Transparent
        Private _issue As Issue
        Private _vdRef As VdSVGGroup
        Private _calcRelativeXY As New gPoint(0, 0)
        Private _textHeight As Single = 6

        Private _quadrant As Integer = 1

        Public Sub New(issue As IssueReporting.Issue)
            MyBase.New()
            _issue = issue
            Me.SymbolType = SvgSymbolType.QMIssue.ToString
        End Sub

        ReadOnly Property Issue As Issue
            Get
                Return _issue
            End Get
        End Property

        Property OffsetXY As New gPoint(10, 10)

        Property vdReference As VdSVGGroup
            Get
                Return _vdRef
            End Get
            Set(value As VdSVGGroup)
                _vdRef = value
                If value IsNot Nothing Then
                    Me.KblId = value.KblId
                Else
                    Me.Origin = New gPoint(0, 0)
                    Me.KblId = String.Empty
                End If
            End Set
        End Property

        Property TextHeight As Single
            Get
                Return _textHeight
            End Get
            Set(value As Single)
                _textHeight = value
            End Set
        End Property

        Friend Sub CalculateCirclePosition(orderIndex As Integer, max As Integer)
            If _vdRef IsNot Nothing Then
                Dim issueMapper As IDictionary(Of String, IList(Of VdSVGGroup)) = GetDocumentForm.ActiveDrawingCanvas.IssueMapper
                Dim rootRef As VdSVGGroup = _vdRef.GetRootGroup
                Me.Origin = rootRef.BoundingBox.MidPoint

                If _vdRef.SymbolType = SvgSymbolType.Segment.ToString Then
                    _calcRelativeXY = New gPoint(OffsetXY.x, OffsetXY.y)
                Else
                    _calcRelativeXY = New gPoint(rootRef.BoundingBox.Width / 2 + OffsetXY.x, rootRef.BoundingBox.Height / 2 + OffsetXY.y)
                End If

                If issueMapper.ContainsKey(_vdRef.KblId) Then
                    If orderIndex > 0 Then
                        Dim degress As Double = 90
                        If max > 4 Then degress = 360 / max
                        Dim rotateDegrees As Double = degress * orderIndex
                        If rotateDegrees < 90 Then ' 1. Q
                            _quadrant = 1
                        ElseIf rotateDegrees < 180 Then ' 4. Q
                            _quadrant = 4
                        ElseIf rotateDegrees < 270 Then ' 3. Q
                            _quadrant = 3
                        ElseIf rotateDegrees < 360 Then ' 2.Q
                            _quadrant = 2
                        End If

                        _calcRelativeXY = RotatePoint(_calcRelativeXY, New gPoint(0, 0), rotateDegrees)
                    End If
                End If

            End If
        End Sub


        Private Function RotatePoint(pointToRotate As gPoint, centerPoint As gPoint, angleInDegrees As Double) As gPoint
            Dim angleInRadians As Double = angleInDegrees * (Math.PI / 180)
            Dim cosTheta As Double = Math.Cos(angleInRadians)
            Dim sinTheta As Double = Math.Sin(angleInRadians)
            Return New gPoint() With {
                 .x = CInt(cosTheta * (pointToRotate.x - centerPoint.x) - sinTheta * (pointToRotate.y - centerPoint.y) + centerPoint.x),
                 .y = CInt(sinTheta * (pointToRotate.x - centerPoint.x) + cosTheta * (pointToRotate.y - centerPoint.y) + centerPoint.y)
            }
        End Function

        Private Function GetDocumentForm() As DocumentForm
            Return DirectCast(Me.Document.ActionControl.FindForm, DocumentForm)
        End Function

        Property FillColor As System.Drawing.Color
            Get
                Return _fillColor
            End Get
            Set(value As System.Drawing.Color)
                If value <> _fillColor Then
                    _fillColor = value
                    PenColor.SystemColor = GetContrastColor(value)
                End If
            End Set
        End Property

        Public Overrides Sub FillShapeEntities(ByRef entities As VectorDraw.Professional.vdCollections.vdEntities)
            MyBase.FillShapeEntities(entities)

            Dim rect As New vdRect(Me.Document)
            With rect
                .setDocumentDefaults()
                .PenColor.SystemColor = Color.Black  ' Me.FillColor
                .PenWidth = Me.PenWidth
                .Width = 5
                .Height = 5
                .InsertionPoint = _calcRelativeXY

                If FillColor <> Color.Transparent Then
                    .HatchProperties = New vdHatchProperties(VectorDraw.Professional.Constants.VdConstFill.VdFillModeSolid)
                    With .HatchProperties
                        .SetUnRegisterDocument(Me.Document)
                        .FillColor.SystemColor = Me.FillColor
                    End With
                End If
            End With

            Dim lines As New vdTextCollection
            If Issue IsNot Nothing Then
                lines = GetTextLinesEntities(rect.InsertionPoint,
                                                String.Format(QualityIssuesStrings.Id_Text, Issue.Id),
                                                String.Format(QualityIssuesStrings.IssueTag_Text, Issue.IssueTag),
                                                String.Format(QualityIssuesStrings.NofOccurences, Issue.NofOccurrences))

                rect.Width = lines.Box.Width + BORDER_WIDTH * 2
                rect.Height = lines.Box.Height + BORDER_WIDTH * 2
            End If

            rect.InsertionPoint = GetQuadrantCorrectedRectPoint(rect.BoundingBox, _calcRelativeXY) 'HINT: at this point the width and height is available -> calculate here and add entity after calculation to take effect | for calculation the insetionpoint and width/height is needed to change the point by current quadrant
            entities.Add(rect)

            Dim i As Integer = -1
            For Each txtEnt As vdText In lines.Reverse
                i += 1
                txtEnt.InsertionPoint = GetTextInsertionPoint(rect.InsertionPoint, i)  ' HINT: re-correct the insertion point because maybe it was recalculated.
                entities.Add(txtEnt)
            Next

            If Math.Abs(rect.InsertionPoint.x) > 0 OrElse Math.Abs(rect.InsertionPoint.y) > 0 Then
                Dim vdLine As New vdLine(Me.Document)
                With vdLine
                    .setDocumentDefaults()
                    .LineType = Me.Document.LineTypes.Solid
                    .PenColor.SystemColor = Color.Black
                    .PenWidth = 0.1
                    .StartPoint = New gPoint(0, 0)
                    .EndPoint = _calcRelativeXY
                End With
                entities.Add(vdLine)

                Dim angle As Double = vdLine.StartPoint.GetAngle(_calcRelativeXY)

                Dim fillHatch As vdHatchProperties = New vdHatchProperties()
                With fillHatch
                    .SetUnRegisterDocument(Document)
                    .FillMode = VectorDraw.Professional.Constants.VdConstFill.VdFillModeSolid
                    .FillColor.SystemColor = Color.Black
                    .FillColor.AlphaBlending = 255
                End With

                Dim arrow As New vdPolyline(Document)
                With arrow
                    .setDocumentDefaults()
                    .PenWidth = 0.1
                    .PenColor.SystemColor = Color.Black
                    .PenColor.AlphaBlending = 255
                    .HatchProperties = fillHatch
                End With

                Dim vts As New Vertexes
                vts.Add(New Vertex(vdLine.StartPoint))
                vts.Add(New Vertex(vdLine.StartPoint.Polar(angle + ARROW_ANGLE, ARROW_LENGTH)))
                vts.Add(New Vertex(vdLine.StartPoint.Polar(angle - ARROW_ANGLE, ARROW_LENGTH)))
                arrow.VertexList = vts
                arrow.VertexList.makeClosed()
                entities.Add(arrow)
            End If

        End Sub

        Private Function GetContrastColor(color As Color) As Color
            Dim d As Integer = 0

            'HINT: Counting the perceptive luminance (eye favors green color...) 
            Dim a As Double = 1 - (0.299 * color.R + 0.587 * color.G + 0.114 * color.B) / 255

            If a < 0.5 Then
                d = 0
            Else
                'HINT: bright colors - black font
                d = 255
            End If
            ' HINT: dark colors - white font
            Return Color.FromArgb(d, d, d)
        End Function

        Private Function GetQuadrantCorrectedRectPoint(rect As Box, pt As gPoint) As gPoint
            Select Case _quadrant
                Case 1
                    Return pt
                Case 2
                    Return New gPoint(pt.x, pt.y - rect.Height)
                Case 3
                    Return New gPoint(pt.x - rect.Width, pt.y - rect.Height)
                Case 4
                    Return New gPoint(pt.x - rect.Width, pt.y)
                Case Else
                    Throw New NotImplementedException
            End Select
        End Function

        Private Function GetTextLinesEntities(atRelativePoint As gPoint, ParamArray lines() As String) As vdTextCollection
            Dim list As New vdTextCollection(Me.Document)
            Dim i As Integer = -1
            For Each textLine As String In lines
                i += 1
                Dim newText As vdText = list.GetNewText(textLine, GetTextInsertionPoint(atRelativePoint, i), _textHeight, Me.PenColor.SystemColor)
                list.Add(GetLimitedText(newText))
            Next
            Return list
        End Function

        Private Function GetLimitedText(vdText As vdText) As vdText
            If vdText.BoundingBox.Width > MAX_TEXTWIDTH Then
                Do
                    vdText = CType(vdText.Clone(Me.Document), VectorDraw.Professional.vdFigures.vdText)
                    If vdText.TextString.Length > 0 Then
                        vdText.TextString = vdText.TextString.Remove(vdText.TextString.Length - 1)
                    Else
                        Throw New InvalidOperationException(String.Format("Maximum text length must be higher than the width of a single letter {0}!", vdText.BoundingBox.Width))
                    End If
                Loop Until vdText.BoundingBox.Width <= MAX_TEXTWIDTH
                If vdText.TextString.Length > 3 Then
                    vdText.TextString = String.Format("{0}...", vdText.TextString.Remove(vdText.TextString.Length - 3))
                Else
                    vdText.TextString = "WWW"
                    vdText.Update()
                    Throw New InvalidOperationException(String.Format("Maximum text length must be higher than the width of three letters (>~{0}) !", vdText.BoundingBox.Width))
                End If

            End If
            Return vdText
        End Function

        Private Function GetTextInsertionPoint(relXY As gPoint, lineIndex As Integer) As gPoint
            Return New gPoint(relXY.x + BORDER_WIDTH, relXY.y + BORDER_WIDTH + (_textHeight + LINE_SPACE) * lineIndex)
        End Function

        Private Class vdTextCollection
            Inherits System.Collections.ObjectModel.Collection(Of vdText)

            Private _box As New Box
            Private _document As vdDocument

            Public Sub New(document As vdDocument)
                _document = document
            End Sub

            Public Sub New()

            End Sub

            Protected Overrides Sub InsertItem(index As Integer, item As vdText)
                MyBase.InsertItem(index, item)
                _box.AddBox(item.BoundingBox)
            End Sub

            Protected Overrides Sub RemoveItem(index As Integer)
                Throw New NotSupportedException("Item remove not supported!")
            End Sub

            Protected Overrides Sub SetItem(index As Integer, item As vdText)
                Throw New NotSupportedException("Set item not supported!")
            End Sub

            Protected Overrides Sub ClearItems()
                MyBase.ClearItems()
                _box = New Box
            End Sub

            Public Function AddNew(Optional text As String = "", Optional pt As gPoint = Nothing, Optional txtHeight As Double = 1, Optional penColor As System.Drawing.Color = Nothing) As vdText
                Dim newVdText As vdText = GetNewText(text, pt, txtHeight, penColor)
                Me.Add(newVdText)
                Return newVdText
            End Function

            Public Function GetNewText(Optional text As String = "", Optional pt As gPoint = Nothing, Optional txtHeight As Double = 1, Optional penColor As System.Drawing.Color = Nothing) As vdText
                Dim newVdTxt As New vdText(_document)
                If _document IsNot Nothing Then
                    newVdTxt.setDocumentDefaults()
                End If

                newVdTxt.TextString = text
                newVdTxt.Height = txtHeight
                newVdTxt.PenColor.SystemColor = penColor
                newVdTxt.InsertionPoint = pt
                Return newVdTxt
            End Function

            ReadOnly Property Box As Box
                Get
                    Return _box
                End Get
            End Property

        End Class

    End Class

End Namespace