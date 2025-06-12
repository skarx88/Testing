Imports System.Reflection
Imports Zuken.E3.HarnessAnalyzer.Settings
Imports Zuken.E3.Lib.Schema.Kbl.Properties

Public Class WireChangedProperty
    Inherits ChangedProperty

    Public Sub New(owner As ComparisonMapper, currentKBLMapper As KblMapper, compareKBLMapper As KblMapper, settings As GeneralSettingsBase)
        MyBase.New(owner, currentKBLMapper, compareKBLMapper, settings)
    End Sub

    Public Overrides Function CompareObjectProperty(objProperty As PropertyInfo, currentObject As Object, compareObject As Object, excludeProperties As List(Of String)) As Boolean
        Dim currentProperty As Object = objProperty.GetValue(currentObject, Nothing)
        Dim compareProperty As Object = objProperty.GetValue(compareObject, Nothing)
        If (currentProperty IsNot Nothing) Then
            Select Case objProperty.Name
                Case NameOf(Wire_occurrence.Part)
                    Dim currentGeneralWire As General_wire = _currentKBLMapper.GetPart(Of General_wire)(currentProperty.ToString)
                    Dim compareGeneralWire As General_wire = _compareKBLMapper.GetPart(Of General_wire)(compareProperty.ToString)
                    Dim partChangedProperty As GeneralWireChangedProperty = New GeneralWireChangedProperty(Me._owner, _currentKBLMapper, _compareKBLMapper, False, Me.Settings)

                    partChangedProperty.CompareObjectProperties(currentGeneralWire, compareGeneralWire, excludeProperties)
                    If (partChangedProperty.ChangedProperties.Count <> 0) Then MyBase.ChangedProperties.Add(objProperty.Name, partChangedProperty)
                Case NameOf(Wire_occurrence.Installation_information)
                    Dim listConvertToDictionary As New ListConvertToDictionary(DirectCast(currentProperty, IEnumerable(Of Installation_instruction)), DirectCast(compareProperty, IEnumerable(Of Installation_instruction)))
                    Dim installationInfoComparisonMapper As New InstallationInfoComparisonMapper(Me._owner, _currentKBLMapper, _compareKBLMapper, Nothing, Nothing, listConvertToDictionary, Me.Settings)
                    installationInfoComparisonMapper.CompareObjects()

                    If (installationInfoComparisonMapper.HasChanges) Then
                        MyBase.ChangedProperties.Add(NameOf(Wire_occurrence.Installation_information), installationInfoComparisonMapper)
                    End If
                Case NameOf(Wire_occurrence.Length_information)
                    Dim listConvertToDictionary As ListConvertToDictionary
                    listConvertToDictionary = New ListConvertToDictionary(DirectCast(currentProperty, IEnumerable(Of Wire_length)), DirectCast(compareProperty, IEnumerable(Of Wire_length)))

                    Dim wireLengthComparisonMapper As WireLengthComparisonMapper
                    wireLengthComparisonMapper = New WireLengthComparisonMapper(Me._owner, listConvertToDictionary, Me.Settings)
                    wireLengthComparisonMapper.CompareObjects()

                    If (wireLengthComparisonMapper.HasChanges) Then
                        MyBase.ChangedProperties.Add(NameOf(Wire_occurrence.Length_information), wireLengthComparisonMapper)
                        FillChangesForPrimaryLength(currentProperty, compareProperty, wireLengthComparisonMapper)
                    End If
                Case NameOf(Wire_occurrence.Wire_number)
                    If (compareProperty IsNot Nothing AndAlso currentProperty.ToString <> compareProperty.ToString) OrElse (compareProperty Is Nothing AndAlso currentProperty.ToString <> String.Empty) Then
                        MyBase.ChangedProperties.Add(objProperty.Name, compareProperty)
                    End If
            End Select
        ElseIf (compareProperty IsNot Nothing) Then
            Select Case objProperty.Name
                Case NameOf(Wire_occurrence.Installation_information)
                    Dim listConvertToDictionary As New ListConvertToDictionary(New List(Of Installation_instruction), DirectCast(compareProperty, IEnumerable(Of Installation_instruction)))
                    Dim installationInfoComparisonMapper As New InstallationInfoComparisonMapper(Me._owner, _currentKBLMapper, _compareKBLMapper, Nothing, Nothing, listConvertToDictionary, Me.Settings)
                    installationInfoComparisonMapper.CompareObjects()

                    If (installationInfoComparisonMapper.HasChanges) Then
                        MyBase.ChangedProperties.Add(NameOf(Wire_occurrence.Installation_information), installationInfoComparisonMapper)
                    End If
                Case NameOf(Wire_occurrence.Length_information)
                    Dim listConvertToDictionary As ListConvertToDictionary
                    listConvertToDictionary = New ListConvertToDictionary(New List(Of Wire_length), DirectCast(compareProperty, IEnumerable(Of Wire_length)))

                    Dim wireLengthComparisonMapper As New WireLengthComparisonMapper(Me._owner, listConvertToDictionary, Me.Settings)
                    wireLengthComparisonMapper.CompareObjects()

                    If (wireLengthComparisonMapper.HasChanges) Then
                        MyBase.ChangedProperties.Add(NameOf(Wire_occurrence.Length_information), wireLengthComparisonMapper)
                    End If
                Case Else
                    MyBase.ChangedProperties.Add(objProperty.Name, compareProperty)
            End Select
        End If

        Return False
    End Function

    Private Sub FillChangesForPrimaryLength(currentProperty As Object, compareProperty As Object, lengthComparisonMapper As WireLengthComparisonMapper)
        'HINT Special hack for Daimler to get changes in wire length display 
        For Each chg As ChangedItem In lengthComparisonMapper.Changes
            If chg.Key = lengthComparisonMapper.DefaultWireLengthType.ToLower Then

                Dim refLengthes As New List(Of Wire_length)
                For Each wlength As Wire_length In DirectCast(currentProperty, Wire_length())
                    If wlength.Length_type.ToLower = lengthComparisonMapper.DefaultWireLengthType.ToLower Then
                        refLengthes.Add(wlength)
                    End If
                Next
                Dim compLengthes As New List(Of Wire_length)
                For Each wlength As Wire_length In DirectCast(compareProperty, Wire_length())
                    If wlength.Length_type.ToLower = lengthComparisonMapper.DefaultWireLengthType.ToLower Then
                        compLengthes.Add(wlength)
                    End If
                Next

                Dim listConvertToDictionary As New ListConvertToDictionary(refLengthes, compLengthes)
                Dim wireLengthComparisonMapper As New WireLengthComparisonMapper(Me._owner, listConvertToDictionary, Me.Settings)
                wireLengthComparisonMapper.CompareObjects()

                MyBase.ChangedProperties.Add(InformationHub.PRIMARY_LENGTH_COLUMN_KEY, wireLengthComparisonMapper)

            End If
        Next
    End Sub

    Public Overrides Sub OnAfterCompareObjectProperties(currentObject As Object, compareObject As Object)
        Dim currentWire As Wire_occurrence = DirectCast(currentObject, Wire_occurrence)
        Dim compareWire As Wire_occurrence = DirectCast(compareObject, Wire_occurrence)

        Dim currentConnection As Connection = If(_currentKBLMapper.KBLWireNetMapper.ContainsKey(currentWire.SystemId), DirectCast(_currentKBLMapper.KBLWireNetMapper(currentWire.SystemId), Connection), Nothing)
        Dim compareConnection As Connection = If(_compareKBLMapper.KBLWireNetMapper.ContainsKey(compareWire.SystemId), DirectCast(_compareKBLMapper.KBLWireNetMapper(compareWire.SystemId), Connection), Nothing)

        Dim currentStartContactPointId As String = currentConnection?.GetStartContactPointId
        Dim currentEndContactPointId As String = currentConnection?.GetEndContactPointId
        Dim compareStartContactPointId As String = compareConnection?.GetStartContactPointId
        Dim compareEndContactPointId As String = compareConnection?.GetEndContactPointId

        If (currentConnection?.Signal_name Is Nothing AndAlso compareConnection?.Signal_name IsNot Nothing) OrElse (currentConnection?.Signal_name IsNot Nothing AndAlso compareConnection?.Signal_name Is Nothing) OrElse (currentConnection?.Signal_name <> compareConnection?.Signal_name) Then
            MyBase.ChangedProperties.Add(ConnectionPropertyName.Signal_name.ToString, compareConnection.Signal_name)
        End If

        Dim currentStartConnector As Connector_occurrence = _currentKBLMapper.GetConnectorOfContactPoint(currentStartContactPointId)
        Dim compareStartConnector As Connector_occurrence = _compareKBLMapper.GetConnectorOfContactPoint(compareStartContactPointId)

#Region "Hint!!"
        '      Look at the old Net framework trunk code what was done there, this is the awfulst code i've EVER (EVER!!!) seen in my life! This is not programming this is A NEW WAY OF ENCRYPTING CODE!
        '      Masses of checks in a Fu... ONE-LINER and no variables are used to temp store the objects, instead the pattern OBJ.PROPERTY(1).PROPERTY(A).PROPERTY(B).ITEM(X).PROPERT.ITEM is repeated like MADNESS! What the heck?
        '      I had to Analyze this code (which was like hacking into fort-knox!) -> encapsulate the repeating business logic (especially the Try.GetCavityOfContactPointId -> which is fucking huge, complicated and very often used (I REPEAT: IN A F. ONE LINER!!!)),
        '      I really can't undestand how someone can EVEN HANDLE THIS!!! What are you, a godlike "Neo of the matrix" who can encrypt the matrix-code directly from the screen? - on the fly? in seconds? Man you should work on the CIA or somthing!)
        '      I hope it's a better readable now, yes there are now more lines here, but what is the point of this ONE-LINE-CODE. THIS THE F. HELL!!!
        '      When translating this code to a new platform, there can typos here, which have to be fixed. Yes, that's totally normal.
        '      But it's not normal that you have to read like chinese or something only from left to right (without seeing the rest of the code!!), not for me, the f. monitor of a normal person is not 100:9. 
        '      Maybe yours, but not everybody has the money to own such a huge wide-screen, congrats you have cramped the whole application in a single line, thanks for nothing...
#End Region

        If currentStartConnector?.Id <> compareStartConnector?.Id Then
            MyBase.ChangedProperties.Add(ConnectionPropertyName.Connector_A.ToString, compareStartConnector.Id)
        End If

        If Not String.IsNullOrEmpty(currentStartContactPointId) AndAlso Not String.IsNullOrEmpty(compareStartContactPointId) Then
            Dim current_start_cavity As Cavity = _currentKBLMapper.GetCavityOfContactPointId(currentStartContactPointId)
            Dim compare_start_cavity As Cavity = _compareKBLMapper.GetCavityOfContactPointId(compareStartContactPointId)
            If current_start_cavity?.Cavity_number <> compare_start_cavity?.Cavity_number Then
                MyBase.ChangedProperties.Add(ConnectionPropertyName.Cavity_A.ToString, compare_start_cavity.Cavity_number)
            End If
        End If

        Dim currentEndConnector As Connector_occurrence = _currentKBLMapper.GetConnectorOfContactPoint(currentEndContactPointId)
        Dim compareEndConnector As Connector_occurrence = _compareKBLMapper.GetConnectorOfContactPoint(compareEndContactPointId)

        If currentEndConnector?.Id <> compareEndConnector?.Id Then
            MyBase.ChangedProperties.Add(ConnectionPropertyName.Connector_B.ToString, compareEndConnector.Id)
        End If

        If Not String.IsNullOrEmpty(currentEndContactPointId) AndAlso Not String.IsNullOrEmpty(compareEndContactPointId) Then
            Dim current_end_cavity As Cavity = _currentKBLMapper.GetCavityOfContactPointId(currentEndContactPointId)
            Dim compare_end_cavity As Cavity = _compareKBLMapper.GetCavityOfContactPointId(compareEndContactPointId)
            If current_end_cavity?.Cavity_number <> compare_end_cavity?.Cavity_number Then
                MyBase.ChangedProperties.Add(ConnectionPropertyName.Cavity_B.ToString, compare_end_cavity.Cavity_number)
            End If
        End If

        Dim currentSegments As New Dictionary(Of String, String)
        Dim compareSegments As New Dictionary(Of String, String)

        For Each seg As Segment In _currentKBLMapper.GetSegmentsOfWireOrCore(currentWire.SystemId)
            currentSegments.TryAdd(seg.Id, seg.Id)
        Next

        For Each segment As Segment In _compareKBLMapper.GetSegmentsOfWireOrCore(compareWire.SystemId)
            compareSegments.TryAdd(segment.Id, segment.Id)
        Next

        Dim listConvertToDict As New ListConvertToDictionary(currentSegments, compareSegments)
        Dim commonComparisonMapper As New CommonComparisonMapper(Me._owner, _currentKBLMapper, _compareKBLMapper, Nothing, Nothing, listConvertToDict, Me.Settings)
        commonComparisonMapper.CompareObjects()

        If (commonComparisonMapper.HasChanges) Then
            MyBase.ChangedProperties.Add(WirePropertyName.Routing.ToString, commonComparisonMapper)
        End If

        Dim currentWiringGroup As Wiring_group = DirectCast(currentObject, Wire_occurrence).GetWiringGroup(_currentKBLMapper.GetWiringGroups)
        Dim compareWiringGroup As Wiring_group = DirectCast(compareObject, Wire_occurrence).GetWiringGroup(_compareKBLMapper.GetWiringGroups)
        Dim singleObjectComparisonMapper As New SingleObjectComparisonMapper(Me._owner, currentWiringGroup, compareWiringGroup, _currentKBLMapper, _compareKBLMapper, Me.Settings)
        singleObjectComparisonMapper.CompareObjects()

        If singleObjectComparisonMapper.HasChanges Then
            MyBase.ChangedProperties.Add(WirePropertyName.Wiring_group.ToString, singleObjectComparisonMapper)
        End If
    End Sub

End Class
