Imports System.ComponentModel
Imports Infragistics.Win.UltraWinGrid
Imports Zuken.E3.HarnessAnalyzer.Settings

Namespace Checks.Cavities.Views.Model

    Public Class CavityWireView
        Inherits BaseView

        Private _checkState As CheckState = CheckState.Indeterminate
        Private _wireOrCore As Object
        Private _cavity As Cavity_occurrence
        Private _colorImage As System.Drawing.Image
        Private _kblContactPointID As String = String.Empty
        Private _generalSettings As GeneralSettings

        Private Shared _lock As New Object

        Friend Sub New(other As CavityWireView, generalSettings As GeneralSettings)
            MyBase.New(other)

            _checkState = other.CheckState
            _wireOrCore = other._wireOrCore
            _cavity = other._cavity
            _generalSettings = generalSettings

            Me.CavityName = other.CavityName
            Me.KblWireId = other.KblWireId
            Me.KblCavityId = other.KblCavityId
            Me.WireCSA = other.WireCSA
            Me.WireType = other.WireType
            Me.WirePartNumber = other.WirePartNumber
            Me.Color = other.WirePartNumber
        End Sub

        Friend Sub New(core As Core_occurrence, cavity As Cavity_occurrence, kbl As KBLMapper, generalSettings As GeneralSettings)
            _wireOrCore = core
            _cavity = cavity
            _generalSettings = generalSettings

            Me.CavityName = CType(kbl.KblPartMapper(cavity.Part), Cavity).Cavity_number
            Me.KblCavityId = cavity.SystemId
            InitCoreProperties(core, kbl)
            _kblContactPointID = GetContactPoint(cavity, kbl)
        End Sub

        Friend Sub New(wire As Wire_occurrence, cavity As Cavity_occurrence, kbl As KBLMapper, generalSettings As GeneralSettings)
            _wireOrCore = wire
            _cavity = cavity
            _generalSettings = generalSettings

            Me.CavityName = CType(kbl.KblPartMapper(cavity.Part), Cavity).Cavity_number
            Me.KblCavityId = _cavity.SystemId
            InitWireProperties(wire, kbl)
            _kblContactPointID = GetContactPoint(cavity, kbl)
        End Sub

        Private Function GetContactPoint(cavity As Cavity_occurrence, kbl As KBLMapper) As String
            If kbl.KBLCavityContactPointMapper.ContainsKey(cavity.SystemId) Then
                For Each contactPt As Contact_point In kbl.KBLCavityContactPointMapper(cavity.SystemId)
                    If kbl.KBLContactPointWireMapper.ContainsKey(contactPt.SystemId) Then
                        For Each wireId As String In kbl.KBLContactPointWireMapper(contactPt.SystemId)
                            If _KblWireId = wireId Then
                                Return contactPt.SystemId
                            End If
                        Next
                    End If
                Next
            End If

            Return String.Empty
        End Function

        Private Sub InitCoreProperties(core As Core_occurrence, kbl As KBLMapper)
            _KblWireId = core.SystemId

            Dim corePart As Core = core.GetCore(kbl)
            If corePart IsNot Nothing Then
                With corePart
                    _WireCSA = (corePart.Cross_section_area?.Value_component).GetValueOrDefault
                    _WireType = corePart.Wire_type
                    _WirePartNumber = core.GetCable(kbl)?.GetGeneralWire(kbl)?.Part_number
                    _Color = corePart.GetColours
                End With
            End If
        End Sub

        Protected Overridable Sub InitWireProperties(wire As Wire_occurrence, kbl As KBLMapper)
            _KblWireId = wire?.SystemId

            Dim genWire As General_wire = wire.GetGeneralWire(kbl)
            If genWire IsNot Nothing Then
                With genWire
                    _WireCSA = (genWire.Cross_section_area?.Value_component).GetValueOrDefault
                    _WireType = genWire.Wire_type
                    _WirePartNumber = genWire.Part_number
                    _Color = genWire.GetColours
                End With
            End If
        End Sub

        <AllowDisable>
        Overridable ReadOnly Property WireName As String
            Get
                If TypeOf _wireOrCore Is Wire_occurrence Then
                    Return CType(_wireOrCore, Wire_occurrence).Wire_number
                ElseIf TypeOf _wireOrCore Is Core_occurrence Then
                    Return CType(_wireOrCore, Core_occurrence).Wire_number
                End If
                Return String.Empty
            End Get
        End Property

        Public Overrides Property Visible As Boolean
            Get
                If Me.Parent IsNot Nothing AndAlso Me.Parent.Visible Then
                    Return MyBase.Visible
                End If
                Return False
            End Get
            Set(value As Boolean)
                MyBase.Visible = value
            End Set
        End Property

        Property CheckState As CheckState
            Get
                Return _checkState
            End Get
            Set(value As CheckState)
                If _checkState <> value Then
                    _checkState = value
                    OnAfterCheckStateChanged()
                End If
            End Set
        End Property

        Private Sub OnAfterCheckStateChanged()
            If EventsEnabled Then
                SyncLock _lock
                    OnPropertyChanged(Me, New PropertyChangedEventArgs(NameOf(CheckState)))
                    OnPropertyChanged(Me, New PropertyChangedEventArgs(NameOf(StateImage)))
                End SyncLock
            End If
        End Sub

        <DebuggerNonUserCode>
        Private Property EventsEnabled As Boolean = True

        <DebuggerNonUserCode>
        ReadOnly Property CavityName As String

        Public Sub ToggleCheckState()
            If Not IsPropertyReadonly(NameOf(CheckState)) Then
                Dim chkState As CheckState = Me.CheckState
                chkState = CType(CInt(chkState) - 1, CheckState)
                If chkState > 2 Then
                    chkState = CheckState.Unchecked
                ElseIf chkState < 0 Then
                    chkState = CheckState.Indeterminate
                End If
                Me.CheckState = chkState
            End If
        End Sub

        <AllowDisable>
        <DebuggerNonUserCode>
        ReadOnly Property WireCSA As Double

        <AllowDisable>
        <DebuggerNonUserCode>
        ReadOnly Property WireType As String

        <AllowDisable>
        <DebuggerNonUserCode>
        ReadOnly Property WirePartNumber As String

        <AllowDisable>
        <DebuggerNonUserCode>
        ReadOnly Property Color As String

        ReadOnly Property StateImage As Bitmap
            Get
                Select Case CheckState
                    Case CheckState.Checked
                        Return My.Resources.checkbox
                    Case CheckState.Indeterminate
                        Return My.Resources.checkbox_intermediate
                    Case CheckState.Unchecked
                        Return My.Resources.checkbox_unchecked
                    Case Else
                        Throw New NotImplementedException($"Checkstate: {CheckState.ToString}")
                End Select
            End Get
        End Property

        <Browsable(False)>
        ReadOnly Property KblWireId As String = String.Empty

        <Browsable(False)>
        ReadOnly Property KblCavityId As String

        <Browsable(False)>
        ReadOnly Property KblContactPointId As String
            Get
                Return _kblContactPointID
            End Get
        End Property

        ReadOnly Property ColorImage As System.Drawing.Image
            Get
                If _colorImage Is Nothing Then
                    UpdateColorImage()
                End If
                Return _colorImage
            End Get
        End Property

        Private Sub UpdateColorImage()
            _colorImage = New System.Drawing.Bitmap(32, 32)
            Dim ci As ColorInfo = ColorInfo.ParseFromWireColor(Me.Color, _generalSettings)
            Dim BorderWidth As Integer = 2
            Using g As Graphics = Graphics.FromImage(_colorImage)
                If ci.HasColor1 AndAlso ci.HasColor2 Then
                    Dim path1 As New Drawing2D.GraphicsPath
                    path1.AddLines(New Point() {New Point(0, 0), New Point(_colorImage.Width - BorderWidth, _colorImage.Height - BorderWidth), New Point(0, _colorImage.Height - BorderWidth), New Point(0, 0)})
                    Dim region1 As New Region(path1)
                    g.FillRegion(New SolidBrush(ci.Color1), region1)
                    g.DrawPath(Pens.Black, path1)

                    Dim path2 As New Drawing2D.GraphicsPath
                    path2.AddLines(New Point() {New Point(0, 0), New Point(_colorImage.Width - BorderWidth, 0), New Point(_colorImage.Width - BorderWidth, _colorImage.Height - BorderWidth), New Point(0, 0)})
                    Dim region2 As New Region(path2)
                    g.FillRegion(New SolidBrush(ci.Color2), region2)
                    g.DrawPath(Pens.Black, path2)
                ElseIf ci.HasColor1 Then
                    Dim rect As New Rectangle(0, 0, _colorImage.Width - BorderWidth, _colorImage.Height - BorderWidth)
                    g.FillRectangle(New SolidBrush(ci.Color1), rect)
                    g.DrawRectangle(Pens.Black, rect)
                End If
            End Using
        End Sub

        Protected Overrides Sub InitializeRowCore(row As UltraGridRow, disabledColor As Color, normalColor As Color)
            For Each cell As UltraGridCell In row.Cells
                If IsPropertyReadonly(cell.Column.Key) Then
                    cell.Appearance.ForeColor = disabledColor
                Else
                    cell.Appearance.ForeColor = normalColor
                End If
            Next
        End Sub

        <Browsable(False)>
        Public Overridable ReadOnly Property IsVirtual As Boolean
            <DebuggerNonUserCode>
            Get
                Return False
            End Get
        End Property

    End Class

End Namespace