Imports VectorDraw.Geometry
Imports VectorDraw.Professional.vdCollections
Imports VectorDraw.Professional.vdFigures
Imports VectorDraw.Professional.vdObjects
Imports VectorDraw.Professional.vdPrimaries
Imports VectorDraw.Professional.vdPrimaries.vdFigure

Public Enum ContactingViewConnectorType
    Ethernet
    FAKRA
    HSD
    Standard
End Enum

<Reflection.Obfuscation(Feature:="renaming", ApplyToMembers:=False, Exclude:=True)> ' needed to avoid resource-file-names obfuscation errors, see issue #5
Public Class ContactingViewerControl
    Private _xCordToZoom As Double

    Public Sub New()
        InitializeComponent()
    End Sub

    Public Overloads ReadOnly Property Document As VectorDraw.Professional.vdObjects.vdDocument
        Get
            Return Me.VDBaseCntrl.ActiveDocument
        End Get
    End Property
    Private Sub CVControl_Load(sender As Object, e As EventArgs) Handles Me.Load
        With Me.VDBaseCntrl.ActiveDocument
            .GlobalRenderProperties.AxisSize = 10
            .GlobalRenderProperties.CrossSize = 8

            .GridMeasure.GridColor.SystemColor = Color.Green
            .GridMode = False
            .GripSnap = True
            .GridSpaceX = 1.0
            .GridSpaceY = 1.0
            .SnapMode = False
            .Palette.Background = Color.White
            .ShowUCSAxis = False
        End With

        Document.ActiveLayOut.Entities.Add(ShowLegend())
    End Sub
    Public Sub ShowConnector(conn As ContactingViewer.Connector)
        conn.Sort()
        conn.SetUnRegisterDocument(Document)
        conn.setDocumentDefaults()
        _xCordToZoom = conn.GetWidth
        Document.ActiveLayOut.Entities.Add(conn)
    End Sub

    Public Sub ShowFullScreen()
        Me.VDBaseCntrl.ActiveDocument.ZoomExtents()
        Me.VDBaseCntrl.ActiveDocument.Redraw(True)
    End Sub

    Private Function ShowLegend() As vdInsert

        Dim entities As New VectorDraw.Professional.vdCollections.vdEntities()
        AddRectangle(15, 25, New gPoint(0, 0), Color.FloralWhite, entities) 'Legend OuterBorder

        AddRectangle(2, 6, New gPoint(1, 12), Color.LightSkyBlue, entities) 'Connector Occurence
        AddText(My.Resources.ContactingViewStrings.ConnectorOccurrence, New vdText, New gPoint(8, 13), entities)

        Dim connection As New vdLine()
        With connection
            .SetUnRegisterDocument(Document)
            .setDocumentDefaults()
            .StartPoint = New gPoint(2, 10.5)
            .EndPoint = New gPoint(5, 10.5)
            .Thickness = 0.8
            .LineType = Document.LineTypes.Solid
            .ToolTip = My.Resources.ContactingViewStrings.Connection
            .PenColor.SystemColor = Color.Blue
        End With
        entities.AddItem(connection)
        AddText(My.Resources.ContactingViewStrings.Connection, New vdText, New gPoint(8, 10), entities)

        Dim hatchprops As New vdHatchProperties(VectorDraw.Professional.Constants.VdConstFill.VdFillModeSolid)
        hatchprops.FillColor.SystemColor = Color.Gray
        Dim circle As New vdCircle
        With circle
            .SetUnRegisterDocument(Document)
            .setDocumentDefaults()
            .Radius = 0.5
            .PenColor.SystemColor = Color.Black
            .LineWeight = VectorDraw.Professional.Constants.VdConstLineWeight.LW_BYBLOCK
            .LineType = Document.LineTypes.Solid
            .Center = New gPoint(3, 8)
            .ToolTip = My.Resources.ContactingViewStrings.ContactPoint
            circle.HatchProperties = hatchprops
        End With
        entities.AddItem(circle)
        AddText(My.Resources.ContactingViewStrings.ContactPoint, New vdText, New gPoint(8, 7), entities)

        AddRectangle(1, 3, New gPoint(0.5, 4), Color.LawnGreen, entities)
        AddRectangle(1, 3, New gPoint(4.5, 4), Color.Orange, entities)
        AddText(My.Resources.ContactingViewStrings.Terminal, New vdText, New gPoint(8, 4), entities)

        Dim hatchProp1 As New vdHatchProperties(VectorDraw.Professional.Constants.VdConstFill.VdFillModeSolid)
        hatchProp1.FillColor.SystemColor = Color.Blue
        Dim g As Double = Math.Round(Math.Sqrt(2) / 2, 2) 'diagonal
        Dim p As Double = 1

        Dim cavity As New vdPolyline
        With cavity
            .SetUnRegisterDocument(Document)
            .setDocumentDefaults()
            .VertexList.Add(New VectorDraw.Geometry.gPoint(3, (p)))
            .VertexList.Add(New VectorDraw.Geometry.gPoint(2.5, (p + g)))
            .VertexList.Add(New VectorDraw.Geometry.gPoint(3, (p + (g * 2))))
            .VertexList.Add(New VectorDraw.Geometry.gPoint(3.5, (p + g)))
            .ToolTip = My.Resources.ContactingViewStrings.Cavity
            .PenColor.SystemColor = Color.Black
            .PenWidth = 0
            .LineType = Document.LineTypes.Solid
            .visibility = VisibilityEnum.Visible
            .Flag = VectorDraw.Professional.Constants.VdConstPlineFlag.PlFlagCLOSE
            cavity.HatchProperties = hatchProp1
        End With

        entities.AddItem(cavity)
        AddText(My.Resources.ContactingViewStrings.Cavity, New vdText, New gPoint(8, 1), entities)

        Dim blk As New VectorDraw.Professional.vdPrimaries.vdBlock
        blk = Document.Blocks.Add("Legend")
        For Each vdfig As vdFigure In entities
            blk.Entities.AddItem(vdfig)
        Next
        blk.Update()

        Dim ins1 As VectorDraw.Professional.vdFigures.vdInsert = New VectorDraw.Professional.vdFigures.vdInsert()
        ins1.SetUnRegisterDocument(Document)
        ins1.setDocumentDefaults()
        ins1.Block = Document.Blocks.FindName("Legend")
        ins1.InsertionPoint = New VectorDraw.Geometry.gPoint(_xCordToZoom / 2, -40)
        ins1.PenColor.ColorIndex = 55
        ins1.ShowGrips = False

        Return ins1
    End Function

    Private Sub AddText(text As String, txt As vdText, insertPt As gPoint, entities As vdEntities)
        txt = New VectorDraw.Professional.vdFigures.vdText()
        With txt
            .SetUnRegisterDocument(Document)
            .setDocumentDefaults()
            .PenColor.SystemColor = Color.Black
            .TextString = String.Format("{0}", text)
            .Height = 1
            .InsertionPoint = New VectorDraw.Geometry.gPoint(insertPt.x, insertPt.y)
            .TextLine = VectorDraw.Render.grTextStyleExtra.TextLineFlags.None
            Document.TextStyles.Standard.FontFile = "Verdana"
        End With

        entities.AddItem(txt)
    End Sub
    Private Sub AddRectangle(height As Double, width As Double, insertPt As gPoint, color As Color, entities As vdEntities)
        Dim hatchProp As New vdHatchProperties(VectorDraw.Professional.Constants.VdConstFill.VdFillModeSolid)
        hatchProp.FillColor.SystemColor = color

        Dim rectangle As New vdRect
        With rectangle
            .SetUnRegisterDocument(Document)
            .setDocumentDefaults()
            .Height = height
            .Width = width
            .PenColor.SystemColor = color
            .PenWidth = 0
            .LineType = Document.LineTypes.Solid
            .InsertionPoint = New gPoint(insertPt.x, insertPt.y)
            rectangle.HatchProperties = hatchProp
        End With
        entities.AddItem(rectangle)
    End Sub

    Private Sub VDBaseCntrl_GetGripPoints(sender As Object, gripPoints As gPoints, ByRef cancel As Boolean) Handles VDBaseCntrl.GetGripPoints
        cancel = True
    End Sub

End Class
