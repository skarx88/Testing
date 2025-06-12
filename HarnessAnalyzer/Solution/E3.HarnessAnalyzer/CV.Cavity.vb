Imports VectorDraw.Geometry
Imports VectorDraw.Professional.vdCollections
Imports VectorDraw.Professional.vdFigures
Imports VectorDraw.Professional.vdObjects
Imports VectorDraw.Professional.vdPrimaries

Namespace ContactingViewer
    Public Class Cavity
        Inherits BaseCavity

        Private _isTerminalPresent As Boolean
        Private _isInsertPresent As Boolean

        Sub New()

        End Sub

        Public Property IsTerminalPresent As Boolean
            Get
                Return _isTerminalPresent
            End Get
            Set(value As Boolean)
                _isTerminalPresent = value
            End Set
        End Property

        Public Property IsTerminalGreen As Boolean
            Get
                Return _isInsertPresent
            End Get
            Set(value As Boolean)
                _isInsertPresent = value
            End Set
        End Property

        Protected Overrides Sub OnDocumentSelected(document As vdDocument)
            MyBase.OnDocumentSelected(document)
        End Sub

        Protected Overrides Sub OnDocumentDefaults()
            MyBase.OnDocumentDefaults()
        End Sub

        Public Overrides Sub FillShapeEntities(ByRef entities As vdEntities)
            MyBase.FillShapeEntities(entities)
            entities.Add(AddConnection())
            entities.Add(AddContactPoint())
            entities.Add(AddTerminal())
            'cavity as symbol and its name are available in BaseCavity
        End Sub

        Private Function AddConnection() As vdLine
            Dim connection As New vdLine()
            With connection
                .SetUnRegisterDocument(Document)
                .setDocumentDefaults()
                .StartPoint = New gPoint(0, GetConnectionLength)
                .EndPoint = New gPoint(0, -(GetTerminalHt + GetConnectionLength))
                If HasConnection Then
                    .LineType = Document.LineTypes.Solid
                    .PenColor.SystemColor = Color.Blue
                Else
                    .LineType = Document.LineTypes.DPIDash
                    .LineWeight = VectorDraw.Professional.Constants.VdConstLineWeight.LW_20
                    .PenColor.SystemColor = Color.Gray
                End If
            End With
            Return connection
        End Function

        Private Function AddContactPoint() As vdCircle
            Dim hatchprops As New vdHatchProperties(VectorDraw.Professional.Constants.VdConstFill.VdFillModeSolid)
            hatchprops.FillBkColor.SystemColor = Color.Gray
            hatchprops.FillColor.SystemColor = Color.Gray

            Dim circle As New vdCircle
            With circle
                .SetUnRegisterDocument(Document)
                .setDocumentDefaults()
                .Radius = 0.5
                .PenColor.SystemColor = Color.Black
                .LineWeight = VectorDraw.Professional.Constants.VdConstLineWeight.LW_DOCUMENTDEFAULT
                .LineType = Document.LineTypes.Solid
                .Center = New gPoint(0, 0)
                .ToolTip = "Contacting point"
                circle.HatchProperties = hatchprops
            End With
            Return circle
        End Function

        Private Function AddTerminal() As vdFigure
            Dim hatchProp As New vdHatchProperties(VectorDraw.Professional.Constants.VdConstFill.VdFillModeSolid)
            If IsTerminalGreen Then
                hatchProp.FillColor.SystemColor = Color.LightGreen
            Else
                hatchProp.FillColor.SystemColor = Color.Orange
            End If

            If _isTerminalPresent Then
                Dim rectangle As New vdRect
                With rectangle
                    .SetUnRegisterDocument(Document)
                    .setDocumentDefaults()
                    .Height = GetTerminalHt
                    .Width = 1
                    .ToolTip = "Terminal"
                    .PenColor.SystemColor = Color.Black
                    .PenWidth = 0
                    .LineType = Document.LineTypes.Solid
                    .InsertionPoint = New gPoint(-GetContactPtRadius, -(GetTerminalHt + GetContactPtRadius))
                    .SmoothAngle = 0.5
                    rectangle.HatchProperties = hatchProp
                End With

                Return rectangle
            Else
                'When Terminal is not present
                Dim optTerminal As New vdLine()
                With optTerminal
                    .SetUnRegisterDocument(Document)
                    .setDocumentDefaults()
                    .StartPoint = New gPoint(0, -(GetContactPtRadius))
                    .EndPoint = New gPoint(0, -(GetTerminalHt))
                    If HasConnection Then
                        .LineType = Document.LineTypes.Solid
                        .PenColor.SystemColor = Color.Blue
                    Else
                        .LineType = Document.LineTypes.DPIDash
                        .PenColor.SystemColor = Color.Gray
                    End If
                    .ToolTip = "Optional Terminal"
                End With
                Return optTerminal
            End If
        End Function

    End Class
End Namespace
