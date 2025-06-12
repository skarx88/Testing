Imports VectorDraw.Geometry
Imports VectorDraw.Professional.vdCollections
Imports VectorDraw.Professional.vdFigures
Imports VectorDraw.Professional.vdObjects

Namespace ContactingViewer

    'special case when contact point have more than one contacted cavity
    Public Class BridgeInsert
        Inherits ContactingViewer.Insert

        Sub New()
        End Sub

        Protected Overrides Sub OnDocumentSelected(document As vdDocument)
            MyBase.OnDocumentSelected(document)
        End Sub

        Protected Overrides Sub OnDocumentDefaults()
            MyBase.OnDocumentDefaults()
        End Sub

        Public Overrides Sub FillShapeEntities(ByRef entities As vdEntities)
            MyBase.FillShapeEntities(entities)
            entities.Add(AddConnection())
            entities.Add(AddHorizontalConnection())
            entities.Add(AddContactPoint())
        End Sub

        Private Function AddConnection() As vdLine
            Dim connection As New vdLine()
            With connection
                .SetUnRegisterDocument(Document)
                .setDocumentDefaults()
                .StartPoint = New gPoint(GetInsertWidth / 2, GetInsertHeight + Cavity.GetConnectionLength)
                .EndPoint = New gPoint(GetInsertWidth / 2, GetInsertHeight + Cavity.GetContactPtRadius)
                .LineType = Document.LineTypes.Solid
                .ToolTip = "Connection"
                .PenColor.SystemColor = Color.Blue
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
                .Center = New gPoint(GetInsertWidth / 2, GetInsertHeight)
                .ToolTip = "Contacting point"
                circle.HatchProperties = hatchprops
            End With
            Return circle
        End Function

        Private Function AddHorizontalConnection() As vdLine
            Dim connection As New vdLine()
            With connection
                .SetUnRegisterDocument(Document)
                .setDocumentDefaults()
                .StartPoint = New gPoint(Cavity.GetInitialDistance, GetInsertHeight)
                .EndPoint = New gPoint((Cavities.Count - 1) * Cavity.GetFixedDistance + Cavity.GetInitialDistance, GetInsertHeight)
                .LineType = Document.LineTypes.Solid
                .ToolTip = "Connection"
                .LineWeight = VectorDraw.Professional.Constants.VdConstLineWeight.LW_40
                .PenColor.SystemColor = Color.Blue
            End With
            Return connection
        End Function
    End Class

End Namespace