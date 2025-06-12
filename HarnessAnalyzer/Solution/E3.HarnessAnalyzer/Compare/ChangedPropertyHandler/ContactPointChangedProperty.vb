Imports System.Reflection
Imports Zuken.E3.HarnessAnalyzer.Settings
Imports Zuken.E3.Lib.Schema.Kbl.Properties

Public Class ContactPointChangedProperty
    Inherits ChangedProperty

    Private _currentActiveObjects As ICollection(Of String)
    Private _compareActiveObjects As ICollection(Of String)

    Public Sub New(owner As ComparisonMapper, currentKBLMapper As KblMapper, compareKBLMapper As KblMapper, currentActiveObjects As ICollection(Of String), compareActiveObjects As ICollection(Of String), settings As GeneralSettingsBase)
        MyBase.New(owner, currentKBLMapper, compareKBLMapper, settings)

        _currentActiveObjects = currentActiveObjects
        _compareActiveObjects = compareActiveObjects
    End Sub

    Public Overrides Function CompareObjectProperty(objProperty As PropertyInfo, currentObject As Object, compareObject As Object, excludeProperties As List(Of String)) As Boolean
        Dim currentProperty As Object = objProperty.GetValue(currentObject, Nothing)
        Dim compareProperty As Object = objProperty.GetValue(compareObject, Nothing)
        If (currentProperty IsNot Nothing) Then
            Select Case objProperty.Name
                Case NameOf(Contact_point.Associated_parts)
                    Dim currentTerminalSealPartPairs As List(Of Tuple(Of General_terminal, Cavity_seal)) = _currentKBLMapper.GetTerminalSealPartPairsOfContactPoint(DirectCast(currentObject, Contact_point), _currentActiveObjects).ToList
                    Dim compareTerminalSealPartPairs As List(Of Tuple(Of General_terminal, Cavity_seal)) = _compareKBLMapper.GetTerminalSealPartPairsOfContactPoint(DirectCast(compareObject, Contact_point), _compareActiveObjects).ToList

                    For Each currentSealTerminalPart As Tuple(Of General_terminal, Cavity_seal) In currentTerminalSealPartPairs
                        Dim compareContactPointPart As Tuple(Of General_terminal, Cavity_seal) = compareTerminalSealPartPairs.Where(Function(part) part.Item1 IsNot Nothing AndAlso currentSealTerminalPart.Item1 IsNot Nothing AndAlso part.Item1.Part_number = currentSealTerminalPart.Item1.Part_number).FirstOrDefault
                        Dim currentSealPartNumber As String = If(currentSealTerminalPart.Item2 IsNot Nothing, currentSealTerminalPart.Item2.Part_number, String.Empty)
                        Dim currentTerminalPartNumber As String = If(currentSealTerminalPart.Item1 IsNot Nothing, currentSealTerminalPart.Item1.Part_number, String.Empty)
                        Dim deletedSealPartNumber As String = String.Empty
                        Dim deletedTerminalPartNumber As String = String.Empty
                        Dim sealComparisonMapper As SingleObjectComparisonMapper = Nothing
                        Dim terminalComparisonMapper As SingleObjectComparisonMapper = Nothing


                        If (compareContactPointPart IsNot Nothing) Then
                            terminalComparisonMapper = New SingleObjectComparisonMapper(Me._owner, currentSealTerminalPart.Item1, compareContactPointPart.Item1, _currentKBLMapper, _compareKBLMapper, Me.Settings)
                            If Not excludeProperties.Contains(ConnectorPropertyName.Terminal_part_information.ToString) Then
                                terminalComparisonMapper.CompareObjects()
                            End If

                        Else
                            deletedTerminalPartNumber = currentTerminalPartNumber
                        End If

                        If (currentSealTerminalPart.Item2 IsNot Nothing) Then
                            compareContactPointPart = compareTerminalSealPartPairs.Where(Function(part) part.Item2 IsNot Nothing AndAlso part.Item2.Part_number = currentSealPartNumber).FirstOrDefault
                            If (compareContactPointPart IsNot Nothing) Then
                                sealComparisonMapper = New SingleObjectComparisonMapper(Me._owner, currentSealTerminalPart.Item2, compareContactPointPart.Item2, _currentKBLMapper, _compareKBLMapper, Me.Settings)
                                If Not excludeProperties.Contains(ConnectorPropertyName.Seal_part_information.ToString) Then
                                    sealComparisonMapper.CompareObjects()
                                End If

                            Else
                                deletedSealPartNumber = currentSealPartNumber
                            End If
                        End If


                        If (currentSealTerminalPart.Item2 IsNot Nothing) Then
                            If (terminalComparisonMapper IsNot Nothing) AndAlso (terminalComparisonMapper.HasChanges) Then
                                If (sealComparisonMapper IsNot Nothing) AndAlso (sealComparisonMapper.HasChanges) Then
                                    If (Not ChangedProperties.ContainsKey(String.Format("{0}#{1}", currentTerminalPartNumber, currentSealPartNumber))) Then
                                        MyBase.ChangedProperties.Add(String.Format("{0}#{1}", currentTerminalPartNumber, currentSealPartNumber), New Tuple(Of Object, Object)(terminalComparisonMapper, sealComparisonMapper))
                                    End If
                                ElseIf (deletedSealPartNumber <> String.Empty) Then
                                    If (Not ChangedProperties.ContainsKey(String.Format("{0}#{1}", currentTerminalPartNumber, currentSealPartNumber))) Then
                                        MyBase.ChangedProperties.Add(String.Format("{0}#{1}", currentTerminalPartNumber, currentSealPartNumber), New Tuple(Of Object, Object)(terminalComparisonMapper, ChangedItem.GetText(CompareChangeType.Deleted)))
                                    End If
                                Else
                                    If (Not ChangedProperties.ContainsKey(String.Format("{0}#{1}", currentTerminalPartNumber, currentSealPartNumber))) Then
                                        MyBase.ChangedProperties.Add(String.Format("{0}#{1}", currentTerminalPartNumber, currentSealPartNumber), New Tuple(Of Object, Object)(terminalComparisonMapper, "Original"))
                                    End If
                                End If
                            Else
                                If (sealComparisonMapper IsNot Nothing) AndAlso (sealComparisonMapper.HasChanges) Then
                                    If (Not ChangedProperties.ContainsKey(String.Format("{0}#{1}", currentTerminalPartNumber, currentSealPartNumber))) Then
                                        MyBase.ChangedProperties.Add(String.Format("{0}#{1}", currentTerminalPartNumber, currentSealPartNumber), New Tuple(Of Object, Object)(If(deletedTerminalPartNumber <> String.Empty, ChangedItem.GetText(CompareChangeType.Deleted), "Original"), sealComparisonMapper))
                                    End If
                                ElseIf (deletedSealPartNumber <> String.Empty) Then
                                    If (Not ChangedProperties.ContainsKey(String.Format("{0}#{1}", currentTerminalPartNumber, currentSealPartNumber))) Then
                                        MyBase.ChangedProperties.Add(String.Format("{0}#{1}", currentTerminalPartNumber, currentSealPartNumber), New Tuple(Of Object, Object)(If(deletedTerminalPartNumber <> String.Empty, ChangedItem.GetText(CompareChangeType.Deleted), "Original"), ChangedItem.GetText(CompareChangeType.Deleted)))
                                    End If
                                ElseIf (deletedTerminalPartNumber <> String.Empty) Then
                                    If (Not ChangedProperties.ContainsKey(String.Format("{0}#{1}", currentTerminalPartNumber, currentSealPartNumber))) Then
                                        MyBase.ChangedProperties.Add(String.Format("{0}#{1}", currentTerminalPartNumber, currentSealPartNumber), New Tuple(Of Object, Object)(ChangedItem.GetText(CompareChangeType.Deleted), "Original"))
                                    End If
                                End If
                            End If
                        Else
                            If (terminalComparisonMapper IsNot Nothing) AndAlso (terminalComparisonMapper.HasChanges) Then
                                If (Not ChangedProperties.ContainsKey(String.Format("{0}#{1}", currentTerminalPartNumber, String.Empty))) Then
                                    MyBase.ChangedProperties.Add(String.Format("{0}#{1}", currentTerminalPartNumber, String.Empty), New Tuple(Of Object, Object)(terminalComparisonMapper, String.Empty))
                                End If
                            ElseIf (deletedTerminalPartNumber <> String.Empty) Then
                                If (Not ChangedProperties.ContainsKey(String.Format("{0}#{1}", currentTerminalPartNumber, String.Empty))) Then
                                    MyBase.ChangedProperties.Add(String.Format("{0}#{1}", currentTerminalPartNumber, String.Empty), New Tuple(Of Object, Object)(ChangedItem.GetText(CompareChangeType.Deleted), String.Empty))
                                End If
                            End If
                        End If
                    Next

                    For Each compareTerminalSealPart As Tuple(Of General_terminal, Cavity_seal) In compareTerminalSealPartPairs
                        Dim addedSealPartNumber As String = String.Empty
                        Dim addedTerminalPartNumber As String = String.Empty
                        Dim compareSealPartNumber As String = If(compareTerminalSealPart.Item2 IsNot Nothing, compareTerminalSealPart.Item2.Part_number, String.Empty)
                        Dim compareTerminalPartNumber As String = If(compareTerminalSealPart.Item1 IsNot Nothing, compareTerminalSealPart.Item1.Part_number, String.Empty)


                        If (Not currentTerminalSealPartPairs.Any(Function(part) part.Item1 IsNot Nothing AndAlso part.Item1.Part_number = compareTerminalPartNumber)) Then
                            addedTerminalPartNumber = compareTerminalPartNumber
                        End If

                        If (Not currentTerminalSealPartPairs.Any(Function(part) part.Item2 IsNot Nothing AndAlso part.Item2.Part_number = compareSealPartNumber)) Then
                            addedSealPartNumber = compareSealPartNumber
                        End If


                        If (addedTerminalPartNumber <> String.Empty) AndAlso (addedSealPartNumber <> String.Empty) Then
                            If (Not ChangedProperties.ContainsKey(String.Format("{0}#{1}", compareTerminalPartNumber, compareSealPartNumber))) Then
                                MyBase.ChangedProperties.Add(String.Format("{0}#{1}", compareTerminalPartNumber, compareSealPartNumber), New Tuple(Of Object, Object)("Added", "Added"))
                            End If
                        ElseIf (addedTerminalPartNumber <> String.Empty) Then
                            If (Not ChangedProperties.ContainsKey(String.Format("{0}#{1}", compareTerminalPartNumber, compareSealPartNumber))) Then
                                MyBase.ChangedProperties.Add(String.Format("{0}#{1}", compareTerminalPartNumber, compareSealPartNumber), New Tuple(Of Object, Object)("Added", If(compareTerminalSealPart.Item2 IsNot Nothing, "Original", String.Empty)))
                            End If
                        ElseIf (addedSealPartNumber <> String.Empty) Then
                            If (Not ChangedProperties.ContainsKey(String.Format("{0}#{1}", compareTerminalPartNumber, compareSealPartNumber))) Then
                                MyBase.ChangedProperties.Add(String.Format("{0}#{1}", compareTerminalPartNumber, compareSealPartNumber), New Tuple(Of Object, Object)("Original", "Added"))
                            End If
                        End If
                    Next
            End Select
        ElseIf (compareProperty IsNot Nothing) Then
            If (compareProperty.ToString = NameOf(Contact_point.Associated_parts)) Then
                Dim compareTerminalSealPartPairs As List(Of Tuple(Of General_terminal, Cavity_seal)) = _compareKBLMapper.GetTerminalSealPartPairsOfContactPoint(DirectCast(compareObject, Contact_point), _compareActiveObjects).ToList

                For Each compareTerminalSealPart As Tuple(Of General_terminal, Cavity_seal) In compareTerminalSealPartPairs
                    Dim addedTerminalPartNumber As String = String.Empty
                    Dim addedSealPartNumber As String = String.Empty

                    If (compareTerminalSealPart.Item1 IsNot Nothing) Then
                        addedTerminalPartNumber = compareTerminalSealPart.Item1.Part_number
                    End If

                    If (compareTerminalSealPart.Item2 IsNot Nothing) Then
                        addedSealPartNumber = compareTerminalSealPart.Item2.Part_number
                    End If

                    If (addedTerminalPartNumber <> String.Empty) AndAlso (addedSealPartNumber <> String.Empty) Then
                        MyBase.ChangedProperties.Add(String.Format("{0}#{1}", compareTerminalSealPart.Item1.Part_number, compareTerminalSealPart.Item2.Part_number), New Tuple(Of Object, Object)("Added", "Added"))
                    ElseIf (addedTerminalPartNumber <> String.Empty) Then
                        MyBase.ChangedProperties.Add(String.Format("{0}#{1}", compareTerminalSealPart.Item1.Part_number, String.Empty), New Tuple(Of Object, Object)("Added", String.Empty))
                    End If
                Next
            End If
        End If
        Return False
    End Function
End Class