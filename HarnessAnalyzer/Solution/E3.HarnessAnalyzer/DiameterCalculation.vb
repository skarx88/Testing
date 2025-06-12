Imports Zuken.E3.HarnessAnalyzer.Settings
Imports Zuken.E3.Lib.Packager.Circle

Public Class DiameterCalculation

    'Friend Shared Function CalculateSegmentDiameter(segmentId As String, wires As IEnumerable(Of String), kblMapper As KBLMapper, diameterSettings As DiameterSettings, ByRef cancelled As Boolean) As Double
    '    Dim dic As New Dictionary(Of String, Double)
    '    CalculateSegmentDiameters(New Tuple(Of String, IEnumerable(Of String))() {New Tuple(Of String, IEnumerable(Of String))(segmentId, wires)}, kblMapper, diameterSettings, cancelled, dic)

    '    If dic.Count > 0 Then
    '        Dim calculated As KeyValuePair(Of String, Double) = dic.Single
    '        Return calculated.Value
    '    End If

    '    Return Nothing
    'End Function

    Friend Shared Function CalculateSegmentDiameter(segment As E3.Lib.Model.Segment, diameterSettings As DiameterSettings, ByRef cancelled As Boolean) As Double
        Dim dic As New Dictionary(Of E3.Lib.Model.Segment, Double)
        CalculateSegmentDiameters({segment}, diameterSettings, cancelled, dic)

        If dic.Count > 0 Then
            Dim calculated As KeyValuePair(Of E3.Lib.Model.Segment, Double) = dic.Single
            Return calculated.Value
        End If

        Return Nothing
    End Function

    'Friend Shared Sub CalculateSegmentDiameters(segments As IEnumerable(Of Tuple(Of String, IEnumerable(Of String))), kblMapper As KBLMapper, diameterSettings As DiameterSettings, ByRef cancelled As Boolean, ByRef calculatedSegmentDiameters As Dictionary(Of String, Double))
    '    Dim circlePackager As New CirclePackager
    '    cancelled = False

    '    If calculatedSegmentDiameters Is Nothing Then calculatedSegmentDiameters = New Dictionary(Of String, Double)

    '    For Each KV As Tuple(Of String, IEnumerable(Of String)) In segments
    '        Dim segmentCircle As Circle = GetBundleWithCableAndWireCircles(CType(kblMapper.KBLOccurrenceMapper(KV.Item1), Segment), KV.Item2, diameterSettings, kblMapper)
    '        calculatedSegmentDiameters.Add(KV.Item1, BundleCrossSectionForm.GetCalculatedOutsideDiameter(segmentCircle, diameterSettings, diameterSettings.RawBundleInstallationAddOnTolerance + diameterSettings.RawBundleProvisioningAddOnTolerance, diameterSettings.IsAddOnToleranceOnArea))
    '    Next
    'End Sub

    Friend Shared Sub CalculateSegmentDiameters(segments As IEnumerable(Of E3.Lib.Model.Segment), diameterSettings As DiameterSettings, ByRef cancelled As Boolean, ByRef calculatedSegmentDiameters As Dictionary(Of E3.Lib.Model.Segment, Double))
        Dim circlePackager As New CirclePackager
        cancelled = False

        If calculatedSegmentDiameters Is Nothing Then
            calculatedSegmentDiameters = New Dictionary(Of E3.Lib.Model.Segment, Double)
        End If

        For Each seg As E3.Lib.Model.Segment In segments
            Dim segmentCircle As PackagingCircle = GetBundleWithCableAndWireCircles(seg, diameterSettings)
            calculatedSegmentDiameters.Add(seg, BundleCrossSectionForm.GetCalculatedOutsideDiameter(segmentCircle, diameterSettings, diameterSettings.RawBundleInstallationAddOnTolerance + diameterSettings.RawBundleProvisioningAddOnTolerance, diameterSettings.IsAddOnToleranceOnArea))
        Next
    End Sub

    Private Shared Function CreateNewCircleFrom(segment As E3.Lib.Model.Segment) As PackagingCircle
        Dim segmentCircle As New PackagingCircle(segment.Id.ToString)
        With segmentCircle
            .AllowEnlargement = True
            .Description = String.Format(BundleCrossSectionFormStrings.Segment_Text, segment.Id)
            .Tag = segment
        End With
        Return segmentCircle
    End Function

    Private Shared Function CreateNewCableCircleFrom(cable As E3.Lib.Model.Cable, segmentCircle As PackagingCircle, diameterSettings As DiameterSettings) As PackagingCircle
        If (cable?.IsMulticoreIndicated).GetValueOrDefault Then
            Dim cableCircle As New PackagingCircle(cable.Id.ToString)
            With cableCircle
                .Description = String.Format(BundleCrossSectionFormStrings.Cable_Text, cable.ShortName)
                .Parent = segmentCircle
                .Radius = GetRadius(cable, diameterSettings)
                .Tag = cable
            End With
            Return cableCircle
        End If
        Return Nothing
    End Function

    Friend Shared Function GetBundleWithCableAndWireCircles(segment As E3.Lib.Model.Segment, diameterSettings As DiameterSettings) As PackagingCircle
        'HINT MR the complete variant handling for the wires and cores is retrieved from the model now!
        'beware that the cable circles need to be added with their full size, even if individual wires are inactive!

        Dim segmentCircle As PackagingCircle = CreateNewCircleFrom(segment)
        Dim wiringGroupCircles As New Dictionary(Of String, PackagingCircle)

        Dim var As E3.Lib.Model.Variant = segment.HostContainer.Model.Variants.OfType(Of E3.Lib.Model.Variant).Where(Function(v) v.ShortName = HarnessAnalyzer.Shared.ISACTIVE_VARIANT_NAME).SingleOrDefault
        If var Is Nothing Then
            var = segment.HostContainer.Model.ActiveVariant
        End If

        For Each cabGrp As IGrouping(Of E3.Lib.Model.Cable, E3.Lib.Model.Wire) In segment.GetWires.Entries.GroupBy(Function(w) w.GetCable)
            If cabGrp.Key IsNot Nothing Then
                If cabGrp.Key.IsMulticoreIndicated Then
                    Dim accCoreDiameters As Double = 0

                    Dim cableActive As Boolean = False
                    For Each core As E3.Lib.Model.Wire In cabGrp
                        If (core.IsInVariant(var)) Then
                            cableActive = True
                            Exit For
                        End If
                    Next
                    If cableActive Then
                        Dim cableCircle As New PackagingCircle(cabGrp.Key.Id.ToString)
                        With cableCircle
                            .Description = String.Format(BundleCrossSectionFormStrings.Cable_Text, cabGrp.Key.ShortName)
                            .Parent = segmentCircle
                            .Radius = GetOutsideDiameter(cabGrp.Key, diameterSettings).GetValueOrDefault / 2
                            .Tag = cabGrp.Key
                        End With

                        For Each core As E3.Lib.Model.Wire In cabGrp
                            Dim twisting As E3.Lib.Model.Twisting = cabGrp.Key.GetTwisting
                            Dim twistingCircle As PackagingCircle = Nothing

                            If (twisting IsNot Nothing) Then
                                If (wiringGroupCircles.ContainsKey(twisting.Id.ToString)) Then
                                    twistingCircle = wiringGroupCircles(twisting.Id.ToString)
                                End If

                                If (twistingCircle Is Nothing) Then
                                    twistingCircle = New PackagingCircle(twisting.Id.ToString)

                                    With twistingCircle
                                        .AllowEnlargement = True
                                        .Description = String.Format(BundleCrossSectionFormStrings.WiringGroup_Text, twisting.Id)
                                        .Parent = cableCircle
                                        .Radius = 0
                                        .Tag = twisting
                                    End With

                                    cableCircle.InnerCircles.Add(twistingCircle)

                                    wiringGroupCircles.Add(twistingCircle.Id, twistingCircle)
                                End If
                            End If

                            Dim coreCircle As New PackagingCircle(core.Id.ToString)
                            With coreCircle
                                .Description = String.Format(BundleCrossSectionFormStrings.Core_Text, core.ShortName)

                                If (twistingCircle IsNot Nothing) Then
                                    .Parent = twistingCircle
                                Else
                                    .Parent = cableCircle
                                End If

                                If (cabGrp.Key.OuterDiameter <> 0) Then
                                    .Radius = cabGrp.Key.OuterDiameter / 2
                                Else
                                    .Radius = BundleCrossSectionForm.GetGenericCoreOrWireDiameter(core, diameterSettings) / 2
                                End If

                                .Tag = core
                            End With

                            If (twistingCircle IsNot Nothing) Then
                                twistingCircle.InnerCircles.Add(coreCircle)
                            Else
                                cableCircle.InnerCircles.Add(coreCircle)
                            End If

                            accCoreDiameters += coreCircle.Radius
                        Next


                        If (cableCircle.AllowEnlargement) AndAlso (accCoreDiameters = 0) Then
                            cableCircle.Radius = GetRadius(cabGrp.Key, diameterSettings)
                        End If

                        segmentCircle.InnerCircles.Add(cableCircle)
                    End If


                Else
                    'single wire
                    Dim wire As E3.Lib.Model.Wire = cabGrp.Single

                    If (wire.IsInVariant(var)) Then
                        Dim twisting As E3.Lib.Model.Twisting = cabGrp.Key.GetTwisting
                        Dim twistingCircle As PackagingCircle = Nothing

                        If (twisting IsNot Nothing) Then
                            If (wiringGroupCircles.ContainsKey(twisting.Id.ToString)) Then
                                twistingCircle = wiringGroupCircles(twisting.Id.ToString)
                            End If

                            If (twistingCircle Is Nothing) Then
                                twistingCircle = New PackagingCircle(twisting.Id.ToString)

                                With twistingCircle
                                    .AllowEnlargement = True
                                    .Description = String.Format(BundleCrossSectionFormStrings.WiringGroup_Text, twisting.ShortName)
                                    .Parent = segmentCircle
                                    .Radius = 0
                                    .Tag = twisting
                                End With

                                segmentCircle.InnerCircles.Add(twistingCircle)

                                wiringGroupCircles.Add(twistingCircle.Id, twistingCircle)
                            End If
                        End If

                        Dim wireCircle As New PackagingCircle(wire.Id.ToString)
                        With wireCircle
                            .Description = String.Format(BundleCrossSectionFormStrings.Wire_Text, wire.ShortName)
                            If (twistingCircle IsNot Nothing) Then
                                .Parent = twistingCircle
                            Else
                                .Parent = segmentCircle
                            End If
                            .Radius = GetRadius(cabGrp.Key, diameterSettings)
                            .Tag = wire
                        End With

                        If (twistingCircle IsNot Nothing) Then
                            twistingCircle.InnerCircles.Add(wireCircle)
                        Else
                            segmentCircle.InnerCircles.Add(wireCircle)
                        End If
                    End If
                End If
            End If
        Next

        Return segmentCircle
    End Function

    Private Shared Function ReplaceCoresWithCables(kblCablesCoresAndWires As IEnumerable(Of String), kbl As KBLMapper) As IEnumerable(Of String)
        Dim myCablesCoresAndWires As HashSet(Of String) = New HashSet(Of String)(kblCablesCoresAndWires)

        'HINT: replace core-ids with cable / clean- to only "cables and wires"
        For Each id As String In kblCablesCoresAndWires
            Dim coreOcc As Core_occurrence = kbl.GetOccurrenceObject(Of Core_occurrence)(id)
            If coreOcc IsNot Nothing Then
                Dim cable As Special_wire_occurrence = kbl.GetCableOfWireOrCore(coreOcc.SystemId)
                myCablesCoresAndWires.Remove(id)
                myCablesCoresAndWires.Add(cable.SystemId)
            End If
        Next

        Return myCablesCoresAndWires
    End Function

    'Friend Shared Function GetBundleWithCableAndWireCircles(segment As Segment, kblCablesCoresAndWiresOnSegment As IEnumerable(Of String), diameterSettings As DiameterSettings, kblMapper As KBLMapper) As Circle
    '    'Static userWireNumber As New Random(Now.Millisecond)

    '    Dim segmentCircle As New Circle(segment.SystemId)
    '    With segmentCircle
    '        .AllowEnlargement = True
    '        .Description = String.Format(BundleCrossSectionFormStrings.Segment_Text, segment.Id)
    '        .Tag = segment
    '    End With

    '    ' TODO: Replace the cores with it's special wires (cables) because the rest of this code was taken from BundleCrossSectionForm where the calculation process was done over the special_wires/cables. 
    '    ' But the calculation from DocumentForm of the bundle diameters must provide the cores because the SegmentWireMapper does not provide mapping from cable to Segment.
    '    ' This whole approach here is suboptimal and the recalculate method should be reworked (provide mapping from cable to segment in the kblMapper) or this method must be changed to do the calculation when there is a core_occurrence!


    '    Dim wiringGroupCircles As New Dictionary(Of String, Circle)
    '    Dim getOrCalculateOutsideDiamter As Func(Of General_wire, Double) =
    '        Function(generalWire As General_wire) As Double
    '            Return GetOutsideDiameter(generalWire, diameterSettings).Value
    '        End Function

    '    For Each wireOrCableId As String In ReplaceCoresWithCables(kblCablesCoresAndWiresOnSegment, kblMapper)
    '        Dim generalWire As General_wire = Nothing
    '        Dim wireOrCableOcc As Object = kblMapper.KBLOccurrenceMapper(wireOrCableId)
    '        If (TypeOf wireOrCableOcc Is Special_wire_occurrence) Then
    '            Dim accCoreDiameters As Double = 0
    '            Dim cable As Special_wire_occurrence = DirectCast(wireOrCableOcc, Special_wire_occurrence)

    '            generalWire = DirectCast(kblMapper.KblPartMapper(cable.Part), General_wire)

    '            Dim cableCircle As New Circle(cable.SystemId)
    '            With cableCircle
    '                .Description = String.Format(BundleCrossSectionFormStrings.Cable_Text, cable.Special_wire_id)
    '                .Parent = segmentCircle
    '                .Radius = getOrCalculateOutsideDiamter(generalWire) / 2
    '                .Tag = generalWire
    '            End With

    '            If (cable.Core_Occurrence IsNot Nothing) AndAlso (cable.Core_Occurrence.Count <> 0) Then
    '                For Each coreOcc As Core_occurrence In cable.Core_Occurrence
    '                    Dim wiringGroup As Wiring_group = coreOcc.GetWiringGroup(kblMapper.GetWiringGroups)
    '                    Dim wiringGroupCircle As Circle = Nothing

    '                    If (wiringGroup IsNot Nothing) AndAlso (wiringGroup.GetConsistencyState(kblMapper.KBLCoreCableMapper, kblMapper.KBLOccurrenceMapper) = [Lib].Schema.Kbl.WiringGroupConsistencyState.Valid) Then
    '                        If (wiringGroupCircles.ContainsKey(wiringGroup.SystemId)) Then wiringGroupCircle = wiringGroupCircles(wiringGroup.SystemId)
    '                        If (wiringGroupCircle Is Nothing) Then
    '                            wiringGroupCircle = New Circle(wiringGroup.SystemId)

    '                            With wiringGroupCircle
    '                                .AllowEnlargement = True
    '                                .Description = String.Format(BundleCrossSectionFormStrings.WiringGroup_Text, wiringGroup.Id)
    '                                .Parent = cableCircle
    '                                .Radius = 0
    '                                .Tag = wiringGroup
    '                            End With

    '                            cableCircle.InnerCircles.Add(wiringGroupCircle)

    '                            wiringGroupCircles.Add(wiringGroupCircle.Id, wiringGroupCircle)
    '                        End If
    '                    End If

    '                    Dim core As Core = DirectCast(kblMapper.KblPartMapper(coreOcc.Part), Core)
    '                    Dim coreCircle As New Circle(coreOcc.SystemId)
    '                    With coreCircle
    '                        .Description = String.Format(BundleCrossSectionFormStrings.Core_Text, coreOcc.Wire_number)

    '                        If (wiringGroupCircle IsNot Nothing) Then
    '                            .Parent = wiringGroupCircle
    '                        Else
    '                            .Parent = cableCircle
    '                        End If

    '                        If (core.Outside_diameter IsNot Nothing) AndAlso (core.Outside_diameter.Value_component <> 0) Then
    '                            .Radius = core.Outside_diameter.Value_component / 2
    '                        Else
    '                            .Radius = BundleCrossSectionForm.GetGenericCoreOrWireDiameter(core, diameterSettings) / 2
    '                        End If

    '                        .Tag = core
    '                    End With

    '                    If (wiringGroupCircle IsNot Nothing) Then
    '                        wiringGroupCircle.InnerCircles.Add(coreCircle)
    '                    Else
    '                        cableCircle.InnerCircles.Add(coreCircle)
    '                    End If

    '                    accCoreDiameters += coreCircle.Radius
    '                Next
    '            End If

    '            If (cableCircle.AllowEnlargement) AndAlso (accCoreDiameters = 0) Then
    '                cableCircle.Radius = getOrCalculateOutsideDiamter(generalWire) / 2
    '            End If

    '            segmentCircle.InnerCircles.Add(cableCircle)
    '        ElseIf (TypeOf wireOrCableOcc Is Wire_occurrence) Then
    '            Dim wire As Wire_occurrence = DirectCast(wireOrCableOcc, Wire_occurrence)
    '            Dim wiringGroup As Wiring_group = wire.GetWiringGroup(kblMapper.GetWiringGroups)
    '            Dim wiringGroupCircle As Circle = Nothing

    '            If (wiringGroup IsNot Nothing) AndAlso (wiringGroup.GetConsistencyState(kblMapper.KBLCoreCableMapper, kblMapper.KBLOccurrenceMapper) = [Lib].Schema.Kbl.WiringGroupConsistencyState.Valid) Then
    '                If (wiringGroupCircles.ContainsKey(wiringGroup.SystemId)) Then wiringGroupCircle = wiringGroupCircles(wiringGroup.SystemId)
    '                If (wiringGroupCircle Is Nothing) Then
    '                    wiringGroupCircle = New Circle(wiringGroup.SystemId)

    '                    With wiringGroupCircle
    '                        .AllowEnlargement = True
    '                        .Description = String.Format(BundleCrossSectionFormStrings.WiringGroup_Text, wiringGroup.Id)
    '                        .Parent = segmentCircle
    '                        .Radius = 0
    '                        .Tag = wiringGroup
    '                    End With

    '                    segmentCircle.InnerCircles.Add(wiringGroupCircle)

    '                    wiringGroupCircles.Add(wiringGroupCircle.Id, wiringGroupCircle)
    '                End If
    '            End If

    '            generalWire = DirectCast(kblMapper.KblPartMapper(wire.Part), General_wire)

    '            Dim wireCircle As New Circle(wire.SystemId)
    '            With wireCircle
    '                .Description = String.Format(BundleCrossSectionFormStrings.Wire_Text, wire.Wire_number)

    '                If (wiringGroupCircle IsNot Nothing) Then
    '                    .Parent = wiringGroupCircle
    '                Else
    '                    .Parent = segmentCircle
    '                End If

    '                .Radius = getOrCalculateOutsideDiamter(generalWire) / 2
    '                .Tag = generalWire
    '            End With

    '            If (wiringGroupCircle IsNot Nothing) Then
    '                wiringGroupCircle.InnerCircles.Add(wireCircle)
    '            Else
    '                segmentCircle.InnerCircles.Add(wireCircle)
    '            End If
    '        Else
    '            Throw New NotImplementedException("User wire is not implemented in this calculation ! ")
    '        End If
    '    Next

    '    Return segmentCircle
    'End Function


    Private Shared Function GetRadius(cable As E3.Lib.Model.Cable, diameterSettings As DiameterSettings) As Double
        Dim diameter As Nullable(Of Double) = GetOutsideDiameter(cable, diameterSettings)
        If diameter.HasValue Then
            Return diameter.Value / 2
        End If
        Return 0
    End Function

    Private Shared Function GetOutsideDiameter(generalWire As General_wire, diameterSettings As DiameterSettings) As Nullable(Of Double)
        Dim diameter As Diameter = diameterSettings.Diameters.FindDiameterFromPartNumber(generalWire.Part_number)

        If (diameter Is Nothing) Then diameter = diameterSettings.Diameters.FindDiameterFromWireType(generalWire.Wire_type)
        If (diameter Is Nothing) AndAlso (generalWire IsNot Nothing) AndAlso (generalWire.Outside_diameter IsNot Nothing) AndAlso (generalWire.Outside_diameter.Value_component <> 0) Then
            diameter = New Diameter(generalWire.Part_number, generalWire.Wire_type, CSng(Math.Round(generalWire.Outside_diameter.Value_component, 1)))
        End If

        If (diameter Is Nothing) AndAlso (generalWire IsNot Nothing) Then
            If (generalWire.Core IsNot Nothing) AndAlso (generalWire.Core.Length <> 0) Then
                diameter = New Diameter(generalWire.Part_number, generalWire.Wire_type, BundleCrossSectionForm.GetGenericMulticoreDiameter(generalWire, diameterSettings))
            Else
                diameter = New Diameter(generalWire.Part_number, generalWire.Wire_type, BundleCrossSectionForm.GetGenericCoreOrWireDiameter(generalWire, diameterSettings))
            End If
        End If
        Return diameter.Value
    End Function


    Private Shared Function GetOutsideDiameter(cable As E3.Lib.Model.Cable, diameterSettings As DiameterSettings) As Nullable(Of Double)
        Dim diameter As Diameter = diameterSettings.Diameters.FindDiameterFromPartNumber(cable.PartNumber)

        If diameter Is Nothing AndAlso cable IsNot Nothing Then
            diameter = diameterSettings.Diameters.FindDiameterFromWireType(cable.Specifier)
            If cable.OuterDiameter <> 0 Then
                diameter = New Diameter(cable.PartNumber, cable.Specifier, CSng(Math.Round(cable.OuterDiameter, 1)))
            End If
        End If

        If (diameter Is Nothing) Then
            If cable.IsMulticoreIndicated Then
                diameter = New Diameter(cable.PartNumber, cable.Specifier, BundleCrossSectionForm.GetGenericMulticoreDiameter(cable, diameterSettings))
            Else
                diameter = New Diameter(cable.PartNumber, cable.Specifier, BundleCrossSectionForm.GetGenericCoreOrWireDiameter((cable.GetWires.Entries.SingleOrDefault?.CSANom).GetValueOrDefault, diameterSettings))
            End If
        End If
        Return diameter.Value
    End Function

End Class