Imports Infragistics.Win.UltraWinListView

<Reflection.Obfuscation(Feature:="renaming", ApplyToMembers:=False, Exclude:=True)> ' needed to avoid resource-file-names obfuscation errors, see issue #5
Public Class PartNumbersForm
    Inherits AnalysisForm

    Private _cnMap As Dictionary(Of String, String)
    Private _segMap As Dictionary(Of String, String)

    Public Sub New()
        MyBase.New
        InitializeComponent()
    End Sub

    Public Sub New(doc As DocumentForm)
        MyBase.New(doc)

        InitializeComponent()
        InitializePartnumbers()
    End Sub

    Private Sub ulvPartnumbers_ItemDoubleClick(sender As Object, e As ItemDoubleClickEventArgs) Handles ulvPartnumbers.ItemDoubleClick
        btnView.PerformClick()
    End Sub

    Private Sub ulvPartnumbers_ItemSelectionChanged(sender As Object, e As ItemSelectionChangedEventArgs) Handles ulvPartnumbers.ItemSelectionChanged
        Me.btnView.Enabled = e.SelectedItems.Count > 0
    End Sub

    Private Sub tbx_partnumber_TextChanged(sender As Object, e As EventArgs) Handles tbx_partnumber.TextChanged
        If Not String.IsNullOrEmpty(tbx_partnumber.Text) Then
            Dim srch As String = tbx_partnumber.Text
            For Each item As UltraListViewItem In ulvPartnumbers.Items
                If item.Value.ToString.StartsWith(srch) Then
                    item.Visible = True
                Else
                    item.Visible = False
                End If
            Next
        Else
            For Each item As UltraListViewItem In ulvPartnumbers.Items
                item.Visible = True
            Next
        End If
    End Sub

    Private Sub InitializePartnumbers()
        Application.UseWaitCursor = True
        Me.Text = AnalysisFormStrings.ShowPartnumbers_Caption
        lbl_partnumber.Text = String.Format("{0}: ", AnalysisFormStrings.Show_PartNumber)

        If Kbl.PartnumberMapper Is Nothing Then
            PreparePartNumbers()
            Kbl.PartnumberMapper = New Dictionary(Of String, List(Of String))

            For Each item As KeyValuePair(Of String, Object) In Kbl.KBLPartMapper
                If TypeOf (item.Value) Is Part Then
                    Dim part As Part = CType(item.Value, Part)
                    Dim ids As New List(Of String)
                    ids = GetLargeIdsFromPart(part)
                    For Each id As String In ids
                        Kbl.PartnumberMapper.AddOrUpdate(part.SystemId, id)
                    Next
                End If
            Next
        End If

        ulvPartnumbers.BeginUpdate()

        For Each item As KeyValuePair(Of String, List(Of String)) In Kbl.PartnumberMapper
            Dim part As Part = CType(Kbl.KBLPartMapper(item.Key), Part)

            If item.Value.Count > 0 Then
                Dim description As String = String.Empty
                If part.Description IsNot Nothing Then description = part.Description
                Dim typeName As String = GetDisplayTypeNameFromPart(part)
                Dim newItem As New UltraListViewItem(part.Part_number, New Object() {description, typeName})
                newItem.Key = item.Key
                ulvPartnumbers.Items.Add(newItem)
            End If
        Next

        ulvPartnumbers.EndUpdate()

        Me.ugbPartnumbers.Visible = True
        Application.UseWaitCursor = False
        If ulvPartnumbers.Items.Count > 0 Then
            ulvPartnumbers.Items(0).Activate()
        End If
        lbl_partnumber.Focus()

    End Sub

    Private Function GetLargeIdsFromPart(part As Part) As List(Of String)
        Dim ids As New List(Of String)


        For Each item As Accessory_occurrence In Kbl.GetAccessoryOccurrences.Where(Function(p) p.Part = part.SystemId)
            ids.Add(item.SystemId)
            CheckReference(item.Reference_element, ids)
        Next

        For Each item As Assembly_part_occurrence In Kbl.GetAssemblyPartOccurrences.Where(Function(p) p.Part = part.SystemId)
            ids.Add(item.SystemId)
        Next

        For Each item As Cavity_plug_occurrence In Kbl.GetCavityPlugOccurrences.Where(Function(p) p.Part = part.SystemId)

            ids.Add(item.SystemId)
            Dim cnId As String = GetConnectorIdFromCavityPlug(item)
            If Not String.IsNullOrEmpty(cnId) Then ids.Add(cnId)
        Next

        For Each item As Cavity_seal_occurrence In Kbl.GetCavitySealOccurrences.Where(Function(p) p.Part = part.SystemId)
            ids.Add(item.SystemId)
            Dim cnId As String = GetConnectorIdFromCavitySeal(item)
            If Not String.IsNullOrEmpty(cnId) Then ids.Add(cnId)
        Next

        For Each item As Co_pack_occurrence In Kbl.GetCoPackOccurrences.Where(Function(p) p.Part = part.SystemId)
            ids.Add(item.SystemId)
        Next

        For Each item As Component_occurrence In Kbl.GetComponentOccurrences.Where(Function(p) p.Part = part.SystemId)
            ids.Add(item.SystemId)
            If item.Mounting IsNot Nothing Then
                For Each id As String In GetIdsFormString(item.Mounting)
                    Dim cnOcc As Connector_occurrence = Kbl.GetConnectorOccurrence(id)
                    If cnOcc IsNot Nothing Then ids.Add(cnOcc.SystemId)
                Next
            End If
        Next

        For Each item As Component_box_occurrence In Kbl.GetComponentBoxOccurrences.Where(Function(p) p.Part = part.SystemId)
            ids.Add(item.SystemId)
        Next

        For Each item As Connector_occurrence In Kbl.GetConnectorOccurrences.Where(Function(p) p.Part = part.SystemId)
            ids.Add(item.SystemId)
        Next

        For Each item As Fixing_occurrence In Kbl.GetFixingOccurrences.Where(Function(p) p.Part = part.SystemId)
            ids.Add(item.SystemId)
            Dim segId As String = GetSegmentIdFromFixingassignment(item)
            If Not String.IsNullOrEmpty(segId) Then ids.Add(segId)
        Next

        For Each item As General_wire_occurrence In Kbl.GetGeneralWireOccurrences.Where(Function(p) p.Part = part.SystemId)
            ids.Add(item.SystemId)
        Next

        For Each item As Special_terminal_occurrence In Kbl.GetSpecialTerminalOccurrences.Where(Function(p) p.Part = part.SystemId)
            ids.Add(item.SystemId)
            Dim cnId As String = GetConnectorIdFromSpecialTerminal(item)
            If Not String.IsNullOrEmpty(cnId) Then ids.Add(cnId)
        Next

        For Each item As Terminal_occurrence In Kbl.GetTerminalOccurrences.Where(Function(p) p.Part = part.SystemId)
            ids.Add(item.SystemId)
            Dim cnId As String = GetConnectorIdFromTerminal(item)
            If Not String.IsNullOrEmpty(cnId) Then ids.Add(cnId)
        Next

        For Each item As Wire_protection_occurrence In Kbl.GetWireProtectionOccurrences.Where(Function(p) p.Part = part.SystemId)
            ids.Add(item.SystemId)
            Dim segId As String = GetSegmentIdFromWireProtection(item)
            If Not String.IsNullOrEmpty(segId) Then ids.Add(segId)
        Next

        Return ids.Distinct.ToList
    End Function


    Private Function GetDisplayTypeNameFromPart(p As Part) As String
        Dim displayName As String = String.Empty
        Dim partType As Type = p.GetType
        Select Case p.GetType

            Case GetType(Accessory)
                Return AnalysisFormStrings.Accessory
            Case GetType(Assembly_part)
                Return AnalysisFormStrings.Assembly_part
            Case GetType(Cavity_plug)
                Return AnalysisFormStrings.Cavity_plug
            Case GetType(Cavity_seal)
                Return AnalysisFormStrings.Cavity_seal
            Case GetType(Co_pack_part)
                Return AnalysisFormStrings.Co_pack_part
            Case GetType(Fuse)
                Return AnalysisFormStrings.Fuse
            Case GetType(Component)
                Return AnalysisFormStrings.Component
            Case GetType(Component_box)
                Return AnalysisFormStrings.Component_box
            Case GetType(Connector_Housing)
                Return AnalysisFormStrings.Connector_housing
            Case GetType(Fixing)
                Return AnalysisFormStrings.Fixing
            Case GetType(General_terminal)
                Return AnalysisFormStrings.General_terminal
            Case GetType(General_wire)
                Return AnalysisFormStrings.General_wire
            Case GetType(Harness)
                Return String.Empty
            Case GetType(Harness_configuration)
                Return String.Empty
            Case GetType([Module])
                Return String.Empty
            Case GetType(Part_with_title_block)
                Return String.Empty
            Case GetType(Wire_protection)
                Return AnalysisFormStrings.Wire_protection
        End Select

        Return p.GetType.ToString


    End Function


    Private Sub PreparePartNumbers()
        _cnMap = New Dictionary(Of String, String)
        _segMap = New Dictionary(Of String, String)
        For Each cn As Connector_occurrence In Kbl.GetConnectorOccurrences

            For Each contactPoint As Contact_point In cn.Contact_points
                If (contactPoint.Associated_parts IsNot Nothing) Then
                    Dim ids As List(Of String) = GetIdsFormString(contactPoint.Associated_parts)
                    For Each id As String In ids
                        If Not _cnMap.ContainsKey(id) Then _cnMap.Add(id, cn.SystemId)
                    Next
                End If
            Next

            For Each slt As Slot_occurrence In cn.Slots
                For Each cv As Cavity_occurrence In slt.Cavities
                    Dim ids As List(Of String) = GetIdsFormString(cv.Associated_plug)
                    For Each id As String In ids
                        If Not _cnMap.ContainsKey(id) Then
                            _cnMap.Add(id, cn.SystemId)
                        End If
                    Next
                Next
            Next

        Next

        For Each seg As Segment In Kbl.GetSegments
            For Each pA As Protection_area In seg.Protection_area

                Dim wProtOcc As Wire_protection_occurrence = Kbl.GetWireProtectionOccurrence(pA.Associated_protection)
                If Not _segMap.ContainsKey(wProtOcc.SystemId) Then
                    _segMap.Add(wProtOcc.SystemId, seg.SystemId)
                End If

                Dim wProt As Wire_protection = Kbl.GetPart(Of Wire_protection)(wProtOcc.Part)
                If Not _segMap.ContainsKey(wProt.SystemId) Then
                    _segMap.Add(wProt.SystemId, seg.SystemId)
                End If
            Next

        Next

    End Sub

    Private Sub CheckReference(ref As String, ByRef ids As List(Of String))
        If ref IsNot Nothing Then
            ids.Add(ref)
            'Debug.Print("ref: " + ref)
        End If
    End Sub

    Private Function GetIdsFormString(m As String) As List(Of String)
        If Not String.IsNullOrEmpty(m) Then
            Return m.SplitSpace.ToList
        Else
            Return New List(Of String)
        End If
    End Function

    Private Function GetConnectorIdFromCavityPlug(item As Cavity_plug_occurrence) As String
        Dim id As String = String.Empty
        If _cnMap.ContainsKey(item.SystemId) Then
            Return _cnMap(item.SystemId)
        End If
        Return id
    End Function

    Private Function GetConnectorIdFromCavitySeal(item As Cavity_seal_occurrence) As String
        Dim id As String = String.Empty

        If _cnMap.ContainsKey(item.SystemId) Then
            Return _cnMap(item.SystemId)
        End If
        Return id
    End Function

    Private Function GetConnectorIdFromTerminal(item As Terminal_occurrence) As String
        Dim id As String = String.Empty
        If _cnMap.ContainsKey(item.SystemId) Then
            Return _cnMap(item.SystemId)
        End If
        Return id
    End Function
    Private Function GetConnectorIdFromSpecialTerminal(item As Special_terminal_occurrence) As String
        Dim id As String = String.Empty
        If _cnMap.ContainsKey(item.SystemId) Then
            Return _cnMap(item.SystemId)
        End If
        Return id
    End Function
    Private Function GetSegmentIdFromWireProtection(item As Wire_protection_occurrence) As String
        Dim id As String = String.Empty
        If _segMap.ContainsKey(item.SystemId) Then
            Return _segMap(item.SystemId)
        End If
        Return id
    End Function
    Private Function GetSegmentIdFromFixingassignment(item As Fixing_occurrence) As String
        Dim id As String = String.Empty
        If _segMap.ContainsKey(item.SystemId) Then
            Return _segMap(item.SystemId)
        End If
        Return id
    End Function

    Private Function GetSegmentIdFromGeneralwire(item As General_wire) As String
        Dim id As String = String.Empty
        If _segMap.ContainsKey(item.SystemId) Then
            Return _segMap(item.SystemId)
        End If
        Return id
    End Function
    Private Function GetSegmentIdFromGeneralwireOccurrence(item As General_wire_occurrence) As String
        Dim id As String = String.Empty
        If _segMap.ContainsKey(item.SystemId) Then
            Return _segMap(item.SystemId)
        End If
        Return id
    End Function
    Private Function GetSegmentIdFromCoreOccurrence(item As Core_occurrence) As String
        Dim id As String = String.Empty
        If _segMap.ContainsKey(item.SystemId) Then
            Return _segMap(item.SystemId)
        End If
        Return id
    End Function

    Private Sub btnView_Click(sender As Object, e As EventArgs) Handles btnView.Click
        ActiveObjects.Clear()
        ActiveObjects.AddRange(Kbl.PartnumberMapper(Me.ulvPartnumbers.SelectedItems.Single.Key))
        MyBase.btnViewClicked()
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Me.Close()
        ' MyBase.btnCancelClicked()
    End Sub

    Private Sub PartnumbersForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Icon = My.Resources.AnalysisShowPartnumbers
        BackColor = Color.White

    End Sub

    Private Sub ugbPartnumbers_Click(sender As Object, e As EventArgs) Handles ugbPartnumbers.Click

    End Sub
End Class