Imports VectorDraw.Professional.vdCollections
Imports VectorDraw.Professional.vdFigures
Imports VectorDraw.Professional.vdObjects
Imports VectorDraw.Professional.vdPrimaries

Namespace ContactingViewer
    Public Class Insert
        Inherits vdShape

        Private _cavities As New List(Of BaseCavity)
        Private _isInsertPresent As Boolean
        Sub New()

        End Sub
        Public ReadOnly Property Cavities As List(Of BaseCavity) ' for sorting , it must be list
            Get
                Return _cavities
            End Get
        End Property

        Public ReadOnly Property GetInsertHeight As Double
            Get
                Return (Cavity.GetContactPtRadius + (Cavity.GetTerminalHt / 2))
            End Get
        End Property

        Public ReadOnly Property GetInsertWidth As Double
            Get
                If IsInsertPresent Then
                    Return ((_cavities.Count - 1) * Cavity.GetFixedDistance) + Cavity.GetFixedDistance
                Else
                    Return 0
                End If
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

        Public Function AddNewCavity() As Cavity
            Dim cav As New ContactingViewer.Cavity()
            cav.SetUnRegisterDocument(Document)
            cav.setDocumentDefaults()
            cav.Update()

            _cavities.Add(cav)
            Return cav
        End Function

        Public Function AddNewBaseCavity() As BaseCavity
            Dim cav As New ContactingViewer.BaseCavity()
            cav.SetUnRegisterDocument(Document)
            cav.setDocumentDefaults()
            cav.Update()

            _cavities.Add(cav)
            Return cav
        End Function

        Protected Overrides Sub OnDocumentSelected(document As vdDocument)
            MyBase.OnDocumentSelected(document)
            For Each cav As BaseCavity In _cavities
                cav.SetUnRegisterDocument(document)
            Next
        End Sub

        Protected Overrides Sub OnDocumentDefaults()
            MyBase.OnDocumentDefaults()
            For Each cav As BaseCavity In _cavities
                cav.setDocumentDefaults()
            Next
        End Sub

        Public Overrides Sub FillShapeEntities(ByRef entities As vdEntities)
            MyBase.FillShapeEntities(entities)
            entities.Add(AddInsertOutline())
            For Each cav As BaseCavity In _cavities
                entities.Add(cav)
            Next
        End Sub

        Private Function AddInsertOutline() As vdRect
            Dim hatchProp As New vdHatchProperties(VectorDraw.Professional.Constants.VdConstFill.VdFillModeSolid)
            If IsInsertPresent Then
                Dim rectangle As New vdRect
                With rectangle
                    Call .SetUnRegisterDocument(MyBase.Document)
                    .setDocumentDefaults()
                    .Height = GetInsertHeight
                    .Width = GetInsertWidth
                    hatchProp.FillColor.SystemColor = Color.Orange
                    .PenColor.SystemColor = Color.Black
                    .ToolTip = "Insert"
                    .PenWidth = 0
                    .LineType = MyBase.Document.LineTypes.Solid
                    rectangle.HatchProperties = hatchProp

                End With
                Return rectangle
            Else
                Return Nothing
            End If
        End Function

    End Class
End Namespace

