Imports System.Text.RegularExpressions
Imports System.Xml.Serialization

<Reflection.Obfuscation(Feature:="renaming", ApplyToMembers:=False, Exclude:=True)> ' needed to avoid resource-file-names obfuscation errors, see issue #5
Public Class ContactingView
    Private _connector As Connector_occurrence
    Private _kblMapper As KblMapper

    Private _dicOfContactedCavityPart As Dictionary(Of String, String)
    Private _dicOfContentCavityIdsTerminalPresent As Dictionary(Of String, Boolean)
    Private _dicOfContactedPartAndIsContactedPartCreated As Dictionary(Of String, Boolean)
    Private _dicOfAssociatedPartAndListOfContactPoints As Dictionary(Of String, List(Of Contact_point))
    Private _listOfContactPoints As List(Of Contact_point)
    Private _dicOfContactedCavityAndContactPoint As Dictionary(Of String, String)
    Private _dicOfContactPointAndIsBridgeInsert As Dictionary(Of String, Boolean)
    Private _dicOfContactedCavAndIsItUsed As Dictionary(Of String, Boolean)
    Private WithEvents _contactingViewerControl As E3.HarnessAnalyzer.ContactingViewerControl

    Sub New(connector As Connector_occurrence, kblMapper As KblMapper)
        InitializeComponent()
        _dicOfContactedCavityPart = New Dictionary(Of String, String) 'To get the cavity name
        _dicOfContentCavityIdsTerminalPresent = New Dictionary(Of String, Boolean) 'To check if current Contact Pt have terminal
        _dicOfContactedPartAndIsContactedPartCreated = New Dictionary(Of String, Boolean) 'to check if contact pt is already created 
        _dicOfAssociatedPartAndListOfContactPoints = New Dictionary(Of String, List(Of Contact_point))
        _listOfContactPoints = New List(Of Contact_point) 'To preserve original contact points
        _dicOfContactedCavityAndContactPoint = New Dictionary(Of String, String) 'hash leitung: for special logic ,when cavity of same name(have same Id) are repeated which must be shown as only one 
        _dicOfContactPointAndIsBridgeInsert = New Dictionary(Of String, Boolean) ' for special logic , when contact point have two or more contacted cavity
        _dicOfContactedCavAndIsItUsed = New Dictionary(Of String, Boolean) ' for special logic ,when contacted cavity have more than one contact pts
        _contactingViewerControl = New E3.HarnessAnalyzer.ContactingViewerControl()

        _connector = connector
        _kblMapper = kblMapper
    End Sub

    Private Sub ContactingView_Load(sender As Object, e As EventArgs) Handles Me.Load
        _contactingViewerControl.Dock = DockStyle.Fill
        Me.UltraPanel1.ClientArea.Controls.Add(_contactingViewerControl)
        Me.BackColor = Color.White
        Me.StartPosition = FormStartPosition.CenterScreen

        Dim conn As New ContactingViewer.Connector()
        If Not String.IsNullOrEmpty(_connector.Id) Then
            conn.Name = _connector.Id
            Me.Text = Me.Text.Replace("*", _connector.Id)
        End If

        If _connector.UsageSpecified Then
            conn.Usage = _connector.Usage.ToStringOrXmlName
        End If

        If _connector.Contact_points.Length > 0 Then
            conn.HasContactPoints = True
            FillInsertWithCavitiesandTerminalinConnector(conn)
        Else
            conn.HasContactPoints = False
        End If

        _contactingViewerControl.ShowConnector(conn)
        _contactingViewerControl.ShowFullScreen()
    End Sub

    Private Sub FillInsertWithCavitiesandTerminalinConnector(conn As ContactingViewer.Connector)
        Dim ContactPtsToRemove As New List(Of Contact_point) 'if used by bridge insert

        'Cavity name 
        For Each slot As Slot_occurrence In _connector.Slots
            For Each cav As Cavity_occurrence In slot.Cavities
                If Not _dicOfContactedCavityPart.ContainsKey(cav.SystemId) Then
                    _dicOfContactedCavityPart.Add(cav.SystemId, cav.Part)
                End If
            Next
        Next

        _listOfContactPoints.AddRange(_connector.Contact_points) 'To keep original contact point as it is,even after remove operation

        'Get each associatedPart with ContactPt(s) and Checks if Terminal/Seal/nothing
        For Each contactPt As Contact_point In _listOfContactPoints
            If Not _dicOfContactedCavityAndContactPoint.ContainsKey(contactPt.Contacted_cavity) Then
                _dicOfContactedCavityAndContactPoint.Add(contactPt.Contacted_cavity, contactPt.SystemId)
            End If

            _dicOfContactedCavAndIsItUsed.TryAdd(contactPt.Contacted_cavity, False)

            If contactPt.Contacted_cavity IsNot Nothing Then ' for bridge insert
                If contactPt.Contacted_cavity.SplitSpace.Length > 1 Then
                    _dicOfContactPointAndIsBridgeInsert.TryAdd(contactPt.SystemId, True)
                Else
                    _dicOfContactPointAndIsBridgeInsert.TryAdd(contactPt.SystemId, False)
                End If
            End If
            If contactPt.Associated_parts IsNot Nothing Then
                For Each associatedPart_id As String In contactPt.Associated_parts.SplitSpace

                    Dim terminal_occ As IKblOccurrence = _kblMapper.GetAnyOccurrenceObjectOfKblObjectTypes(associatedPart_id, KblObjectType.Special_terminal_occurrence, KblObjectType.Terminal_occurrence)
                    If terminal_occ IsNot Nothing Then

                        If Not _dicOfAssociatedPartAndListOfContactPoints.ContainsKey(associatedPart_id) Then
                            Dim listOfContactPt As New List(Of Contact_point) From {
                                contactPt
                            }
                            _dicOfAssociatedPartAndListOfContactPoints.Add(associatedPart_id, listOfContactPt)
                        Else
                            _dicOfAssociatedPartAndListOfContactPoints(associatedPart_id).Add(contactPt)
                        End If

                        _dicOfContentCavityIdsTerminalPresent.TryAdd(contactPt.Contacted_cavity, True)
                    End If
                Next
            Else
                'When contact point have no associated part
                _dicOfContentCavityIdsTerminalPresent.TryAdd(contactPt.Contacted_cavity, False)
            End If
        Next
        'If contact point have more than one contacted cavity
        For Each contactPt As Contact_point In _listOfContactPoints
            If contactPt.Contacted_cavity IsNot Nothing Then
                If contactPt.Contacted_cavity.SplitSpace.Length > 1 Then

                    Dim ins As ContactingViewer.BridgeInsert = conn.AddNewBridgeInsert()
                    ins.IsInsertPresent = True

                    For Each cav As String In contactPt.Contacted_cavity.SplitSpace
                        Dim c As ContactingViewer.BaseCavity = ins.AddNewBaseCavity()
                        c.HasConnection = CheckIfHaveAnyConnection(contactPt.SystemId)
                        SetBridgeCavityTerminal(c, cav)
                    Next

                    If _dicOfContactedPartAndIsContactedPartCreated.TryAdd(contactPt.Id, True) Then
                        ContactPtsToRemove.Add(contactPt)
                    End If
                End If
            End If
        Next

        For Each pt As Contact_point In ContactPtsToRemove 'used for Bridge insert
            _listOfContactPoints.Remove(pt)
        Next

        CreateContactPointsForExistingAssociatedPrt(conn) 'for normal insert(s) and single terminal

        For Each contactPt As Contact_point In _listOfContactPoints
            If contactPt.Associated_parts Is Nothing Then
                If _dicOfContactedCavityAndContactPoint.ContainsKey(contactPt.Contacted_cavity) Then
                    If _dicOfContactedCavityAndContactPoint(contactPt.Contacted_cavity) = contactPt.SystemId Then
                        'when associated part is nothing
                        conn.IsInsertPresent = False
                        Dim ins As ContactingViewer.Insert = conn.AddNewInsert()
                        ins.IsInsertPresent = False

                        Dim c As ContactingViewer.Cavity = ins.AddNewCavity()
                        c.HasConnection = CheckIfHaveAnyConnection(contactPt.SystemId)
                        SetCavityTerminal(c, contactPt, False)

                    End If
                End If
            End If
        Next

        SetConnectorType(conn)
    End Sub

    Private Function CheckIfHaveAnyConnection(contactPtId As String) As Boolean
        Dim HasConnection As Boolean = False
        If _kblMapper.KBLContactPointWireMapper.ContainsKey(contactPtId) Then
            If _kblMapper.KBLContactPointWireMapper(contactPtId).Any() Then
                HasConnection = True
            End If
        End If
        Return HasConnection
    End Function

    Private Sub SetCavityTerminal(cav As ContactingViewer.Cavity, contactPt As Contact_point, IsInsertPresent As Boolean)
        Dim IsAssociatedPartsExist As Boolean = False

        If contactPt.Associated_parts IsNot Nothing Then
            If contactPt.Associated_parts.SplitSpace.Length > 1 Then
                IsAssociatedPartsExist = True
            End If
        End If

        If _dicOfContactedCavityPart.ContainsKey(contactPt.Contacted_cavity) Then
            cav.Name = DirectCast(_kblMapper.KBLPartMapper(_dicOfContactedCavityPart(contactPt.Contacted_cavity)), Cavity).Cavity_number
        End If


        If _dicOfContentCavityIdsTerminalPresent(contactPt.Contacted_cavity) = True AndAlso IsAssociatedPartsExist AndAlso IsInsertPresent Then
            cav.IsTerminalGreen = True ' Green Terminal with insert
            cav.IsTerminalPresent = True
        Else
            cav.IsTerminalGreen = False 'Cavity Orange if have two or more associated parts

            If contactPt.Associated_parts IsNot Nothing Then
                For Each associatedPart_id As String In contactPt.Associated_parts.SplitSpace
                    Dim ass_part_occ As IKblOccurrence = _kblMapper.GetOccurrenceObjectUntyped(associatedPart_id)

                    If ass_part_occ IsNot Nothing Then
                        If TypeOf ass_part_occ Is Special_terminal_occurrence Or TypeOf ass_part_occ Is Terminal_occurrence Then
                            If IsInsertPresent Then
                                cav.IsTerminalPresent = False ' No terminal
                            Else
                                cav.IsTerminalPresent = True ' Orange terminal
                            End If

                            Exit For
                        Else
                            cav.IsTerminalPresent = CheckIfTerminalToShow() ' No terminal
                        End If
                    Else
                        cav.IsTerminalPresent = CheckIfTerminalToShow() ' No terminal
                    End If

                Next
            Else
                cav.IsTerminalPresent = CheckIfTerminalToShow()
            End If
        End If
    End Sub

    Private Function CheckIfTerminalToShow() As Boolean
        Dim IsTerminalPresent As Boolean = False
        If _connector.UsageSpecified Then
            If _connector.Usage = Connector_usage.ringterminal Then
                IsTerminalPresent = True
            End If
        End If
        Return IsTerminalPresent
    End Function

    Private Sub SetBridgeCavityTerminal(cav As ContactingViewer.BaseCavity, contactCav As String)
        If _dicOfContactedCavityPart.ContainsKey(contactCav) Then
            cav.Name = DirectCast(_kblMapper.KBLPartMapper(_dicOfContactedCavityPart(contactCav)), Cavity).Cavity_number
        End If
    End Sub

    Private Sub CreateContactPointsForExistingAssociatedPrt(conn As ContactingViewer.Connector)
        'If associated have more than or equal to 2 contact pts then build insert and add terminal accordingly ,which depends on its existence
        For Each kvpOfAssoPartAndDicOfCtPts As KeyValuePair(Of String, List(Of Contact_point)) In _dicOfAssociatedPartAndListOfContactPoints
            If kvpOfAssoPartAndDicOfCtPts.Value.Count >= 2 Then

                If Not IsCavityOccuranceUsed(kvpOfAssoPartAndDicOfCtPts.Value) Then
                    'Getting all contact points which have same associatedPart i.e Insert

                    conn.IsInsertPresent = True 'Cavity Green
                    Dim ins As ContactingViewer.Insert = conn.AddNewInsert()
                    ins.IsInsertPresent = True

                    For Each contactPoint As Contact_point In kvpOfAssoPartAndDicOfCtPts.Value

                        If _dicOfContactedCavAndIsItUsed.ContainsKey(contactPoint.Contacted_cavity) Then
                            _dicOfContactedCavAndIsItUsed(contactPoint.Contacted_cavity) = True
                        End If

                        Dim c As ContactingViewer.Cavity = ins.AddNewCavity()
                        c.HasConnection = CheckIfHaveAnyConnection(contactPoint.SystemId)
                        SetCavityTerminal(c, contactPoint, True)

                        _dicOfContactedPartAndIsContactedPartCreated.TryAdd(contactPoint.Id, True)
                        _listOfContactPoints.Remove(contactPoint)
                    Next
                End If

            Else

                For Each contactPoint As Contact_point In kvpOfAssoPartAndDicOfCtPts.Value
                    If _dicOfContactedCavityAndContactPoint.ContainsKey(contactPoint.Contacted_cavity) Then
                        If _dicOfContactedCavityAndContactPoint(contactPoint.Contacted_cavity) = contactPoint.SystemId Then
                            If _dicOfContactedPartAndIsContactedPartCreated.ContainsKey(contactPoint.Id) Then 'avoids to create already created contact point
                                If Not _dicOfContactedPartAndIsContactedPartCreated(contactPoint.Id) Then '
                                    conn.IsInsertPresent = False
                                    Dim ins As ContactingViewer.Insert = conn.AddNewInsert()
                                    ins.IsInsertPresent = False

                                    Dim c As ContactingViewer.Cavity = ins.AddNewCavity()
                                    c.HasConnection = CheckIfHaveAnyConnection(contactPoint.SystemId)
                                    SetCavityTerminal(c, contactPoint, False)

                                    _listOfContactPoints.Remove(contactPoint)
                                End If
                            Else
                                conn.IsInsertPresent = False
                                Dim ins As ContactingViewer.Insert = conn.AddNewInsert()
                                ins.IsInsertPresent = False

                                Dim c As ContactingViewer.Cavity = ins.AddNewCavity()
                                c.HasConnection = CheckIfHaveAnyConnection(contactPoint.SystemId)
                                SetCavityTerminal(c, contactPoint, False)

                                _listOfContactPoints.Remove(contactPoint)
                            End If

                            _dicOfContactedPartAndIsContactedPartCreated.TryAdd(contactPoint.Id, True)
                        End If
                    End If
                Next
            End If
        Next
    End Sub
    Private Function IsCavityOccuranceUsed(ContactPts As List(Of Contact_point)) As Boolean
        Dim IsCavOccuranceUsed As Boolean = False
        For Each contactPt As Contact_point In ContactPts
            If _dicOfContactedCavAndIsItUsed.ContainsKey(contactPt.Contacted_cavity) Then
                If _dicOfContactedCavAndIsItUsed(contactPt.Contacted_cavity) = True Then
                    IsCavOccuranceUsed = True
                End If
            End If
        Next
        Return IsCavOccuranceUsed
    End Function

    Private Sub SetConnectorType(conn As ContactingViewer.Connector)
        Dim listOfCavityNames As New List(Of String)
        Dim listOfAllInsertCheck As New List(Of Boolean)
        Dim IsHsdNumberFits As Boolean = False

        For Each ins As ContactingViewer.Insert In conn.ListOfInsert
            If ins.Cavities.Count = 5 Then
                listOfAllInsertCheck.Add(True)
            End If

            For Each cav As ContactingViewer.Cavity In ins.Cavities
                listOfCavityNames.Add(cav.Name)
            Next
        Next

        If listOfAllInsertCheck.Count = conn.ListOfInsert.Count Then ' means insert(s) have 5 cavities
            IsHsdNumberFits = True
        End If

        Dim ConnectorType As String
        If IsRequiredTypeMatched(listOfCavityNames, "^x") AndAlso IsRequiredTypeMatched(listOfCavityNames, "^s") Then 'Fakra
            ConnectorType = ContactingViewConnectorType.FAKRA.ToString
        ElseIf IsRequiredTypeMatched(listOfCavityNames, "^s[0-9]+") AndAlso IsRequiredTypeMatched(listOfCavityNames, "^[0-9]+") Then
            ConnectorType = ContactingViewConnectorType.Ethernet.ToString
        ElseIf (IsHsdNumberFits And IsRequiredTypeMatched(listOfCavityNames, "^s")) Then
            ConnectorType = ContactingViewConnectorType.HSD.ToString
        Else
            ConnectorType = ContactingViewConnectorType.Standard.ToString ' e.g only numbers
        End If


        If _connector.UsageSpecified AndAlso _connector.Usage = Connector_usage.ringterminal Then
            conn.MyConnectorType = My.Resources.ContactingViewStrings.RingTerminal
        ElseIf _connector.UsageSpecified AndAlso _connector.Usage = Connector_usage.splice Then
            conn.MyConnectorType = My.Resources.ContactingViewStrings.Splice
        Else
            conn.MyConnectorType = ConnectorType
        End If
    End Sub

    Private Function IsRequiredTypeMatched(listOfCavityNames As List(Of String), pattern As String) As Boolean
        For Each value As String In listOfCavityNames
            If Regex.IsMatch(value, pattern, RegexOptions.IgnoreCase) Then
                Return True
                Exit For
            End If
        Next
        Return False
    End Function

    Private Sub RedrawBtn_Click(sender As Object, e As EventArgs) Handles RedrawBtn.Click
        _contactingViewerControl.ShowFullScreen()
    End Sub

    Private Sub CloseBtn_Click(sender As Object, e As EventArgs) Handles CloseBtn.Click
        Me.Close()
    End Sub
End Class

