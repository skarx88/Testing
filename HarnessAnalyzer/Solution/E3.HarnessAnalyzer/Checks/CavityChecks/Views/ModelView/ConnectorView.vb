Imports System.ComponentModel
Imports devDept.Eyeshot.Entities
Imports VectorDraw.Geometry
Imports VectorDraw.Professional.vdObjects
Imports VectorDraw.Professional.vdPrimaries
Imports Zuken.E3.HarnessAnalyzer.Settings
Imports Zuken.E3.Lib.Eyeshot.Model

Namespace Checks.Cavities.Views.Model
    Public Class ConnectorView
        Inherits BaseView

        Private _conn As Connector_occurrence
        Private _vdFigure As VdSVGGroup
        Private _cavWires As CavityWiresBindingList
        Private _generalSettings As GeneralSettings

        Friend Sub New(other As ConnectorView, generalSettings As GeneralSettings)
            MyBase.New(other)

            _conn = other._conn
            _generalSettings = generalSettings
            Me.IsKSL = other.IsKSL

            For Each cavWView As CavityWireView In other.CavWires
                Dim wv As New CavityWireView(cavWView, generalSettings)
                Me.CavWires.Add(wv)
                AddEventHandler(wv)
            Next
        End Sub

        Friend Sub New(connector As Connector_occurrence, cavities As IEnumerable(Of CavityWireView), generalSettings As GeneralSettings)
            _generalSettings = generalSettings
            _conn = connector
            _cavWires = New CavityWiresBindingList(Me, cavities.ToList)

            For Each w As CavityWireView In Me.CavWires
                AddEventHandler(w)
            Next
        End Sub

        <DebuggerHidden>
        ReadOnly Property CavWires As CavityWiresBindingList
            <DebuggerHidden>
            Get
                Return _cavWires
            End Get
        End Property

        Public Overrides Property Visible As Boolean
            <DebuggerStepThrough>
            Get
                If Not IsKSL Then
                    If Not IsSplice OrElse AreSplicesVisible() Then
                        Return MyBase.Visible
                    End If
                End If
                Return False
            End Get
            <DebuggerStepThrough>
            Set(value As Boolean)
                MyBase.Visible = value
            End Set
        End Property

        Private Function AreSplicesVisible() As Boolean
            Return (My.Application?.MainForm?.GeneralSettings?.ViewSplicesInCavityChecks).GetValueOrDefault
        End Function

        Private Sub AddEventHandler(wc As CavityWireView)
            AddHandler wc.PropertyChanged, AddressOf _wire_PropertyChanged
        End Sub

        Private Sub RemoveEventHandler(wc As CavityWireView)
            RemoveHandler wc.PropertyChanged, AddressOf _wire_PropertyChanged
        End Sub

        <Browsable(False)>
        Property IsKSL As Boolean

        <Browsable(False)>
        ReadOnly Property IsSplice As Boolean
            Get
                Return _conn.Usage = Connector_usage.splice
            End Get
        End Property

        Public Sub CheckCavitites(checkState As CheckState, Optional includeReadonly As Boolean = False)
            CheckCavitites(CavWires, checkState, includeReadonly)
        End Sub

        Public Sub CheckCavitites(cavities As IEnumerable(Of CavityWireView), checkState As CheckState, Optional includeReadonly As Boolean = False)
            Dim i As Integer = 0
            Dim diffCount As Integer = cavities.Where(Function(cv) cv.CheckState <> checkState).Count
            For Each cavWire As CavityWireView In cavities
                If (includeReadonly OrElse Not cavWire.Readonly) AndAlso cavWire.CheckState <> checkState Then
                    Try
                        i += 1
                        If i < diffCount Then
                            RemoveEventHandler(cavWire)
                        End If
                        cavWire.CheckState = checkState
                    Finally
                        If i < diffCount Then
                            AddEventHandler(cavWire)
                        End If
                    End Try
                End If
            Next
        End Sub

        Public Sub ToggleCheckStates(cavities As IEnumerable(Of CavityWireView))
            If cavities Is Nothing Then
                cavities = Me.CavWires.ToArray
            End If
            Dim i As Integer = 0
            Dim count As Integer = cavities.Count
            For Each cavWire As CavityWireView In cavities
                i += 1
                If Not cavWire.IsPropertyReadonly(NameOf(cavWire.CheckState)) Then
                    Try
                        If i < count Then
                            RemoveEventHandler(cavWire)
                        End If
                        cavWire.ToggleCheckState()
                    Finally
                        If i < count Then
                            AddEventHandler(cavWire)
                        End If
                    End Try
                End If
            Next
        End Sub

        Private Sub _wire_PropertyChanged(sender As Object, e As PropertyChangedEventArgs)
            If e.PropertyName = NameOf(CavityWireView.CheckState) Then
                OnPropertyChanged(Me, New PropertyChangedEventArgs(NameOf(StateImage)))
            End If
        End Sub

        <DebuggerHidden>
        ReadOnly Property CheckState As State
            <DebuggerHidden>
            Get
                If Me.CavWires.IsAnyUnchecked Then
                    Return State.AnyUnchecked
                ElseIf Me.CavWires.AllChecked Then
                    Return State.Checked
                ElseIf Me.CavWires.AllIntermediate Then
                    Return State.Indeterminate
                Else
                    Return State.Partial
                End If
            End Get
        End Property


        Public Function GetOrAddGraphicEntities(nav As CavityNavigator, Optional justify As Boolean = True) As Tuple(Of vdFigure, IEntity())
            If _vdFigure Is Nothing OrElse nav.vdConnectorView IsNot _vdFigure.Document.ActionControl Then
                _vdFigure = (Me.Model?.Document?.Canvas.GetConnectorClone(_conn, nav.vdConnectorView)).OfType(Of VdSVGGroup).Where(Function(grp) grp.SVGType = SvgType.Undefined.ToString).FirstOrDefault
                If _vdFigure IsNot Nothing Then
                    nav.vdConnectorView.ActiveDocument.Model.Entities.AddItem(_vdFigure)
                    If justify Then
                        JustifyEntities(nav.vdConnectorView.ActiveDocument, New VdSVGGroup() {_vdFigure})
                    End If
                End If
            End If

            Dim entities As New List(Of IEntity)
            If Me.Model.Document IsNot Nothing Then
                Dim ents As IBaseModelEntityEx() = Me.Model.Document.Get3DEntityClones(_conn.SystemId)
                For Each ent As IBaseModelEntityEx In ents
                    ent.Selected = False
                Next
                entities.AddRange(ents)
            End If

            Return New Tuple(Of vdFigure, IEntity())(_vdFigure, entities.ToArray)
        End Function

        ReadOnly Property StateImage As Bitmap
            <DebuggerHidden>
            Get
                Select Case CheckState
                    Case State.Checked
                        Return My.Resources.circle_green
                    Case State.Indeterminate
                        Return My.Resources.Question
                    Case State.Partial
                        Return My.Resources.circle_yellow
                    Case State.AnyUnchecked
                        Return My.Resources.circle_red
                End Select
                Return Nothing
            End Get
        End Property

        ReadOnly Property Name As String
            <DebuggerHidden>
            Get
                Return _conn.Id
            End Get
        End Property

        <Browsable(False)>
        Friend ReadOnly Property KblId As String
            <DebuggerHidden>
            Get
                Return _conn.SystemId
            End Get
        End Property

        Private Sub JustifyEntities(doc As vdDocument, entities As IEnumerable(Of VdSVGGroup))
            For Each group As VdSVGGroup In entities
                If group IsNot Nothing Then
                    ' HINT: This is a preliminary hack: The mirroring of all groups is done on the outer group. In case of connectors this is the vertex group and here
                    ' we get the real face (inner group) and hence this is not mirrored. The rotation must be reset on the real face or the outer group must be counter rotated.
                    If (Not String.IsNullOrEmpty(group?.SVGType) AndAlso group.SVGType <> SvgType.table.ToString) Then
                        Dim matrix As New Matrix
                        matrix.A11 = -matrix.A11

                        If (group.ECSMatrix.IsEqualMatrix(New Matrix, 0.1)) Then
                            group.Transformby(matrix)
                        Else
                            doc.CommandAction.CmdMirror(group, group.BoundingBox.MidPoint, New gPoint(group.BoundingBox.Min.x, group.BoundingBox.MidPoint.y), "no")
                        End If
                    End If

                    group.Rotation = GetGroupRadius(group)
                End If
            Next
        End Sub

        Private Function GetGroupRadius(group As VdSVGGroup) As Double
            For Each figure As vdFigure In group.ChildGroups
                If (TryCast(figure, VdSVGGroup) IsNot Nothing) Then
                    Return DirectCast(figure, VdSVGGroup).Rotation
                End If
            Next
            Return 0
        End Function

        Public Enum State
            Indeterminate = 0
            AnyUnchecked = 1
            Checked = 2
            [Partial] = 3
        End Enum

    End Class

End Namespace