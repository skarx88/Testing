Imports VectorDraw.Geometry
Imports VectorDraw.Professional.vdCollections
Imports VectorDraw.Professional.vdFigures
Imports VectorDraw.Professional.vdObjects
Imports VectorDraw.Professional.vdPrimaries
Namespace ContactingViewer
    'hint : Used for Bridge insert when contact point have more than one contacted cavity
    'this cavities are connected to only one contact point with insert, so called BridgeInsert
    'As an individual object ,here cavities are drawn without contact point 
    Public Class BaseCavity
        Inherits vdShape

        Private _name As String = String.Empty
        Private _hasConnection As Boolean

        Sub New()

        End Sub
        Public Shared ReadOnly Property GetInitialDistance As Double
            Get
                Return 2
            End Get
        End Property
        Public Shared ReadOnly Property GetContactPtRadius As Double
            Get
                Return 0.5
            End Get
        End Property
        Public Shared ReadOnly Property GetTerminalHt As Double
            Get
                Return 4
            End Get
        End Property
        Public Shared ReadOnly Property GetConnectionLength As Double
            Get
                Return 2
            End Get
        End Property
        Public Shared ReadOnly Property GetFixedDistance As Double
            Get
                Return 4
            End Get
        End Property
        Public Shared ReadOnly Property GetDiagonalOfCavity As Double
            Get
                Return Math.Round(Math.Sqrt(2) * 1, 2) 'side of square * squareroot(2)
            End Get
        End Property
        Public Property Name As String
            Get
                Return _name
            End Get
            Set(value As String)
                _name = value
            End Set
        End Property
        Public Property HasConnection As Boolean
            Get
                Return _hasConnection
            End Get
            Set(value As Boolean)
                _hasConnection = value
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
            If HasConnection Then
                entities.Add(AddConnection())
            End If
            entities.Add(AddCavityPoly())
            entities.Add(AddCavityName())
        End Sub
        Private Function AddConnection() As vdLine
            Dim connection As New vdLine()
            With connection
                .SetUnRegisterDocument(Document)
                .setDocumentDefaults()
                .StartPoint = New gPoint(0, 0)
                .EndPoint = New gPoint(0, -(GetContactPtRadius + GetTerminalHt + GetConnectionLength))
                .LineType = Document.LineTypes.Solid
                .PenColor.SystemColor = Color.Blue
            End With
            Return connection
        End Function
        Private Function AddCavityPoly() As vdPolyline
            Dim hatchProp As New vdHatchProperties(VectorDraw.Professional.Constants.VdConstFill.VdFillModeSolid)
            hatchProp.FillColor.SystemColor = Color.Blue

            Dim g As Double = (GetDiagonalOfCavity / 2) ' 1 * root of 2
            Dim p As Double = GetTerminalHt + GetConnectionLength
            Dim cavity As New vdPolyline

            With cavity
                .SetUnRegisterDocument(Document)
                .setDocumentDefaults()
                .VertexList.Add(New VectorDraw.Geometry.gPoint(0, -(p)))
                .VertexList.Add(New VectorDraw.Geometry.gPoint(-GetContactPtRadius, -(p + g)))
                .VertexList.Add(New VectorDraw.Geometry.gPoint(0, -(p + (g * 2))))
                .VertexList.Add(New VectorDraw.Geometry.gPoint(GetContactPtRadius, -(p + g)))
                .ToolTip = "Cavity"
                .PenColor.SystemColor = Color.Blue
                .PenWidth = 0
                .LineType = Document.LineTypes.Solid
                .visibility = VisibilityEnum.Visible
                .Flag = VectorDraw.Professional.Constants.VdConstPlineFlag.PlFlagCLOSE
                cavity.HatchProperties = hatchProp
            End With

            Return cavity
        End Function
        Private Function AddCavityName() As vdText
            Dim textAtHeight As Double = GetTerminalHt + GetConnectionLength + 3
            Dim cavityText As vdText = New vdText()
            With cavityText
                .SetUnRegisterDocument(Document)
                .setDocumentDefaults()
                .PenColor.SystemColor = Color.Black
                .Thickness = 0.2
                .TextString = String.Format("{0}", Name)
                .InsertionPoint = New gPoint(0, -textAtHeight)
                .TextLine = VectorDraw.Render.grTextStyleExtra.TextLineFlags.None
                .AlignToView = True
                .HorJustify = VectorDraw.Professional.Constants.VdConstHorJust.VdTextHorCenter
            End With

            Document.TextStyles.Standard.FontFile = "courier"
            Return cavityText
        End Function
    End Class
End Namespace
