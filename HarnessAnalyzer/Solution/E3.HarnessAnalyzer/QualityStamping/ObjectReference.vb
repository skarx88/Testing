Imports System.ComponentModel
Imports System.Runtime.Serialization
Imports VectorDraw.Geometry
Imports System.Xml.Serialization

Namespace QualityStamping

    <DataContract(Namespace:="Zuken.E3.HarnessAnalyzer.QualityStamping")>
    Public Class ObjectReference

        Public Sub New()
            Me.New(String.Empty, String.Empty, KblObjectType.Undefined)
        End Sub

        Public Sub New(svg As VdSVGGroup, objName As String, Optional rotation As Nullable(Of Double) = Nothing)
            Dim symbolType As SvgSymbolType = [Enum].Parse(Of SvgSymbolType)(svg.SymbolType)
            Me.KblId = svg.KblId
            Me.ObjectName = objName
            Me.ObjectType = symbolType.AsKblObjectType
            Me.SvgType = svg.SVGType
            Me.Rotation = rotation
        End Sub


        Public Sub New(kblId As String, objName As String, objType As KblObjectType, Optional rotation As Nullable(Of Double) = Nothing)
            Me.KblId = kblId
            Me.ObjectName = objName
            Me.ObjectType = objType
            Me.Rotation = rotation
        End Sub

        Public ReadOnly Property SvgType As String

        <DataMember> Public Property KblId As String
        <DataMember> Public Property ObjectName As String

        <DataMember(Name:="ObjectType")>
        <ComponentModel.EditorBrowsable(EditorBrowsableState.Never)>
        Public Property ObjectTypeSerializationProxy As String
            Get
                Return Me.ObjectType.ToString
            End Get
            Set(value As String)
                Dim result As KblObjectType
                If [Enum].TryParse(Of KblObjectType)(value, result) Then
                    Me.ObjectType = result
                ElseIf value?.ToLower = "vertex" Then ' hardcoded backwards compatibility because we changed from string to KblObjectType -> in KblObjectType-Enum the value vertex does no longer exist
                    Me.ObjectType = KblObjectType.Node
                Else
                    Throw New InvalidEnumArgumentException($"Can not parse ""{value}"" to enum ""{NameOf(KblObjectType)}""")
                End If
            End Set
        End Property

        <IgnoreDataMember>
        Public Property ObjectType As KblObjectType

        <DataMember> Public Property RelativeX As Double
        <DataMember> Public Property RelativeY As Double
        ''' <summary>
        ''' Rotation angle in radiants
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember>
        Public Property Rotation As Nullable(Of Double) = Nothing

        Public ReadOnly Property IsEmptyPoint As Boolean
            Get
                Return Me.RelativeX = 0 AndAlso Me.RelativeY = 0
            End Get
        End Property

        Public ReadOnly Property RelativePt As gPoint
            Get
                Return New gPoint(Me.RelativeX, Me.RelativeY)
            End Get
        End Property

        Private _vdSVGRefObjects As New List(Of VdSVGGroup)

        Public Property vdSVGRefObjects As List(Of VdSVGGroup)
            Get
                If _vdSVGRefObjects Is Nothing Then
                    _vdSVGRefObjects = New List(Of VdSVGGroup)
                End If
                Return _vdSVGRefObjects
            End Get
            Set(value As List(Of VdSVGGroup))
                _vdSVGRefObjects = value
            End Set
        End Property

        Public Property List As ObjectReferenceList

    End Class


    <CollectionDataContract(Namespace:="Zuken.E3.HarnessAnalyzer.QualityStamping")>
    Public Class ObjectReferenceList
        Inherits BindingList(Of ObjectReference)

        Friend Sub New()
            MyBase.New()
        End Sub

        Public Sub New(owner As QMStamp)
            MyBase.New()
            Me.Owner = owner
        End Sub

        Public Shadows Function AddNew(kblId As String) As ObjectReference
            If Not Me.Contains(kblId) Then
                Dim newObjRef As New ObjectReference(kblId, String.Empty, KblObjectType.Undefined)
                Me.Add(newObjRef)

                Return newObjRef
            End If

            Return Nothing
        End Function

        Public Shadows Function AddNew(svgObj As VdSVGGroup) As ObjectReference
            If (Not Me.Contains(svgObj.KblId)) Then
                Dim occ As Object = My.Application.MainForm.ActiveDocument.KBL.GetOccurrenceObjectUntyped(svgObj.KblId)
                Dim newObjRef As New ObjectReference(svgObj, [Lib].Schema.Kbl.Utils.GetUserId(occ))
                newObjRef.vdSVGRefObjects.Add(svgObj)
                Me.Add(newObjRef)

                Return newObjRef
            End If
            Return Nothing
        End Function

        Protected Overrides Sub InsertItem(index As Integer, item As ObjectReference)
            If (Not Me.Contains(item)) Then
                MyBase.InsertItem(index, item)
                item.List = Me
            End If
        End Sub

        Public Overloads Function Contains(objRef As ObjectReference) As Boolean
            Return Contains(objRef.KblId)
        End Function

        Public Overloads Function Contains(KblId As String) As Boolean
            Return Where(Function(ref) ref.KblId = KblId).Any()
        End Function

        Public Sub Delete(objRef As ObjectReference)
            Me.Remove(objRef)
        End Sub

        Protected Overrides Sub RemoveItem(index As Integer)
            Dim item As ObjectReference = Me.Items(index)
            MyBase.RemoveItem(index)
            item.List = Nothing
        End Sub

        Public Function FindObjRefFromKblId(kblId As String) As ObjectReference
            For Each objRef As ObjectReference In Me
                If (objRef.KblId = kblId) Then
                    Return objRef
                End If
            Next

            Return Nothing
        End Function

        Public Function FindObjRefFromObjectNameAndType(objName As String, objType As KblObjectType) As ObjectReference
            For Each objRef As ObjectReference In Me
                If (objRef.ObjectName = objName) AndAlso (objRef.ObjectType = objType) Then
                    Return objRef
                End If
            Next
            Return Nothing
        End Function

        Public Property Owner As QMStamp

    End Class

End Namespace