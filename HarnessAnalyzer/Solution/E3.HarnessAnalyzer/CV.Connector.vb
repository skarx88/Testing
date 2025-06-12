Imports VectorDraw.Geometry
Imports VectorDraw.Professional.vdCollections
Imports VectorDraw.Professional.vdFigures
Imports VectorDraw.Professional.vdObjects
Imports VectorDraw.Professional.vdPrimaries

Namespace ContactingViewer
    Public Class Connector
        Inherits vdShape

        Private _listOfInsert As New List(Of Insert)
        Private _name As String = String.Empty
        Private _usage As String
        Private _isInsertPresent As Boolean
        Private _hasContactPoints As Boolean
        Private _myConnectorType As String

        Sub New()

        End Sub
        Public Property Name As String
            Get
                Return _name
            End Get
            Set(value As String)
                _name = value
            End Set
        End Property
        Public Property MyConnectorType As String
            Get
                Return _myConnectorType
            End Get
            Set(value As String)
                _myConnectorType = value
            End Set
        End Property
        Public Property Usage As String
            Get
                Return _usage
            End Get
            Set(value As String)
                _usage = value
            End Set
        End Property

        Public ReadOnly Property ListOfInsert As IEnumerable(Of Insert)
            Get
                Return _listOfInsert
            End Get
        End Property

        Public ReadOnly Property GetWidth As Double
            Get
                Return GetWidthOfConnector()
            End Get
        End Property

        Private Function GetWidthOfConnector() As Double
            Dim width As Double = 0

            For Each ins As Insert In _listOfInsert
                width += ins.GetInsertWidth
            Next
            width += (_listOfInsert.Count * Cavity.GetFixedDistance) + Cavity.GetFixedDistance
            Return width
        End Function

        Public ReadOnly Property GetHeight As Double
            Get
                Return (Cavity.GetContactPtRadius + Cavity.GetTerminalHt + Cavity.GetConnectionLength + (Cavity.GetDiagonalOfCavity / 2) + Cavity.GetFixedDistance)
            End Get
        End Property

        Public Property IsInsertPresent As Boolean
            Get
                Return _isInsertPresent
            End Get
            Set(value As Boolean)
                _isInsertPresent = value
            End Set
        End Property
        Public Property HasContactPoints As Boolean
            Get
                Return _hasContactPoints
            End Get
            Set(value As Boolean)
                _hasContactPoints = value
            End Set
        End Property

        Public Sub Sort()
            _listOfInsert.Sort(New InsertSorter())
            Dim insCurrentCount As Integer = 0

            For Each ins As Insert In _listOfInsert
                Dim xCord As Double = 0
                Dim cavCurrentCount As Integer = 0

                If insCurrentCount > 0 Then

                    xCord += GetAllInsertWidthTillThisPoint(insCurrentCount) ' gets width of insert(s) ,which  sometime can be differerent
                    xCord += (insCurrentCount * Cavity.GetFixedDistance) + Cavity.GetFixedDistance
                Else
                    xCord = Cavity.GetFixedDistance
                End If

                ins.Origin = New gPoint(xCord, -ins.GetInsertHeight)
                ins.Update()

                ins.Cavities.Sort(New CavitySorter())
                For Each cav As BaseCavity In ins.Cavities
                    Dim Xcord2 As Double = 0

                    If ins.Cavities.Count >= 2 Then
                        If cavCurrentCount = 0 Then
                            Xcord2 = Cavity.GetInitialDistance
                        Else
                            Xcord2 = (cavCurrentCount * Cavity.GetFixedDistance) + Cavity.GetInitialDistance
                        End If

                    Else
                        Xcord2 = 0
                    End If
                    cav.Origin = New gPoint(Xcord2, ins.GetInsertHeight)
                    cav.Update()
                    cavCurrentCount += 1
                Next
                insCurrentCount += 1
            Next
        End Sub

        Private Function GetAllInsertWidthTillThisPoint(insCurrentCount As Integer) As Double
            Dim i As Integer = 0
            Dim allInsertWidthTillthisPoint As Double
            For Each ins As Insert In _listOfInsert
                If i < insCurrentCount Then
                    allInsertWidthTillthisPoint += ins.GetInsertWidth
                End If
                i += 1
            Next
            Return allInsertWidthTillthisPoint
        End Function

        Protected Overrides Sub OnDocumentSelected(document As vdDocument)
            MyBase.OnDocumentSelected(document)
            For Each ins As Insert In _listOfInsert
                ins.SetUnRegisterDocument(document)
            Next
        End Sub

        Protected Overrides Sub OnDocumentDefaults()
            MyBase.OnDocumentDefaults()
            For Each ins As Insert In _listOfInsert
                ins.setDocumentDefaults()
            Next
        End Sub

        Public Overrides Sub FillShapeEntities(ByRef entities As vdEntities)
            MyBase.FillShapeEntities(entities)
            If HasContactPoints Then
                entities.Add(AddConnectorOutline())
            Else
                entities.Add(NoContactPointMsg)
            End If

            For Each ins As Insert In _listOfInsert
                entities.Add(ins)
            Next
            entities.Add(ShowHeader())
        End Sub

        Public Function AddNewInsert() As Insert
            Dim ins As New ContactingViewer.Insert()
            ins.SetUnRegisterDocument(Document)
            ins.setDocumentDefaults()
            ins.Update()

            _listOfInsert.Add(ins)

            Return ins
        End Function

        Public Function AddNewBridgeInsert() As BridgeInsert
            Dim ins As New ContactingViewer.BridgeInsert()
            ins.SetUnRegisterDocument(Document)
            ins.setDocumentDefaults()
            ins.Update()

            _listOfInsert.Add(ins)

            Return ins
        End Function

        Private Function AddConnectorOutline() As vdRect
            Dim hatchProp As New vdHatchProperties(VectorDraw.Professional.Constants.VdConstFill.VdFillModeSolid)
            Dim IsBackgroundLightSkyBlue As Boolean = True
            If Usage IsNot Nothing Then
                If Usage.ToLower = My.Resources.ContactingViewStrings.RingTerminalForComparison Or Usage.ToLower = My.Resources.ContactingViewStrings.SpliceForComparison Then
                    IsBackgroundLightSkyBlue = False
                End If
            End If

            If IsBackgroundLightSkyBlue Then
                hatchProp.FillColor.SystemColor = Color.LightSkyBlue
            Else
                hatchProp.FillColor.SystemColor = Color.White
            End If

            Dim rectangle As New vdRect
            With rectangle
                .SetUnRegisterDocument(Document)
                .setDocumentDefaults()
                .Height = GetHeight
                .Width = GetWidth
                .ToolTip = "Connector"
                .PenColor.SystemColor = Color.Black
                .LineType = Document.LineTypes.Solid
                .InsertionPoint = New gPoint(0, -GetHeight)
                rectangle.HatchProperties = hatchProp
            End With

            Return rectangle
        End Function

        Public Function ShowHeader() As vdText
            Dim headerText As VectorDraw.Professional.vdFigures.vdText = New VectorDraw.Professional.vdFigures.vdText()
            headerText.SetUnRegisterDocument(Document)
            headerText.setDocumentDefaults()
            headerText.PenColor.SystemColor = Color.Black

            headerText.TextString = String.Format("{0}: {1}", Me.Name, Me.MyConnectorType)
            headerText.Bold = True
            headerText.CreateExtra()
            headerText.Height = 1
            Document.TextStyles.Standard.FontFile = "Verdana"

            'We set the insertion point depending the width of the Text from the vdFigure's BoundingBox
            headerText.InsertionPoint = New VectorDraw.Geometry.gPoint(0, 6)
            headerText.TextLine = VectorDraw.Render.grTextStyleExtra.TextLineFlags.None
            Return headerText
        End Function

        Public Function NoContactPointMsg() As vdText
            Dim headerText As VectorDraw.Professional.vdFigures.vdText = New VectorDraw.Professional.vdFigures.vdText()
            headerText.SetUnRegisterDocument(Document)
            headerText.setDocumentDefaults()
            headerText.PenColor.SystemColor = Color.Black

            headerText.TextString = My.Resources.ContactingViewStrings.NoContactPoint
            headerText.Bold = True
            headerText.CreateExtra()
            headerText.Height = 1
            Document.TextStyles.Standard.FontFile = "Verdana"

            'We set the insertion point depending the width of the Text from the vdFigure's BoundingBox
            headerText.InsertionPoint = New VectorDraw.Geometry.gPoint(0, -GetHeight)
            headerText.TextLine = VectorDraw.Render.grTextStyleExtra.TextLineFlags.None
            Return headerText
        End Function

    End Class

End Namespace

