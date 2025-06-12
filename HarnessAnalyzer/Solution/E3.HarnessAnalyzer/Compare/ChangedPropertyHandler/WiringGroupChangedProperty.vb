Imports System.Reflection
Imports Zuken.E3.HarnessAnalyzer.Settings

Public Class WiringGroupChangedProperty
    Inherits ChangedProperty

    Public Sub New(owner As ComparisonMapper, currentKBLMapper As KblMapper, compareKBLMapper As KblMapper, settings As GeneralSettingsBase)
        MyBase.New(owner, currentKBLMapper, compareKBLMapper, settings)
    End Sub

    Public Overrides Function CompareObjectProperty(objProperty As PropertyInfo, currentObject As Object, compareObject As Object, excludeProperties As List(Of String)) As Boolean
        Dim currentProperty As Object = objProperty.GetValue(currentObject, Nothing)
        Dim compareProperty As Object = objProperty.GetValue(compareObject, Nothing)

        If objProperty.Name <> NameOf(Wiring_group.GetAssignedCoreWireIds) AndAlso objProperty.Name <> NameOf(Wiring_group.GetConsistencyState) Then
            If (currentProperty IsNot Nothing) Then

                Select Case objProperty.Name
                    Case NameOf(Wiring_group.Assigned_wire)
                        Dim hasChanges As Boolean = False
                        Dim listConvertToDictionary As New ListConvertToDictionary(currentProperty.ToString, _currentKBLMapper, compareProperty.ToString, _compareKBLMapper)

                        If (listConvertToDictionary.CurrentObjects.Count <> listConvertToDictionary.CompareObjects.Count) Then

                        Else
                            For Each wireCoreName As String In listConvertToDictionary.CurrentObjects.Keys
                                If (Not listConvertToDictionary.CompareObjects.ContainsKey(wireCoreName)) Then
                                    Exit For
                                End If
                            Next
                        End If

                        If (hasChanges) Then
                            Dim wireCoreNumbers As String = String.Empty

                            For Each wireCoreNumber As String In listConvertToDictionary.CompareObjects.Keys
                                If (wireCoreNumbers = String.Empty) Then
                                    wireCoreNumbers = wireCoreNumber
                                Else
                                    wireCoreNumbers = String.Format("{0}, {1}", wireCoreNumbers, wireCoreNumber)
                                End If
                            Next

                            MyBase.ChangedProperties.Add(NameOf(Wiring_group.Assigned_wire), wireCoreNumbers)
                        End If
                    Case NameOf(Wiring_group.Processing_information)
                        Dim listConvertToDictionary As New ListConvertToDictionary(DirectCast(currentProperty, IEnumerable(Of Processing_instruction)), DirectCast(compareProperty, IEnumerable(Of Processing_instruction)))
                        Dim processingInfoComparisonMapper As New ProcessingInfoComparisonMapper(Me._owner, _currentKBLMapper, _compareKBLMapper, Nothing, Nothing, listConvertToDictionary, Me.Settings)
                        processingInfoComparisonMapper.CompareObjects()

                        If (processingInfoComparisonMapper.HasChanges) Then
                            MyBase.ChangedProperties.Add(NameOf(Wiring_group.Processing_information), processingInfoComparisonMapper)
                        End If
                    Case NameOf(Wiring_group.Installation_information)
                        Dim listConvertToDictionary As New ListConvertToDictionary(DirectCast(currentProperty, IEnumerable(Of Installation_instruction)), DirectCast(compareProperty, IEnumerable(Of Installation_instruction)))
                        Dim installationInfoComparisonMapper As New InstallationInfoComparisonMapper(Me._owner, _currentKBLMapper, _compareKBLMapper, Nothing, Nothing, listConvertToDictionary, Me.Settings)
                        installationInfoComparisonMapper.CompareObjects()

                        If (installationInfoComparisonMapper.HasChanges) Then
                            MyBase.ChangedProperties.Add(objProperty.Name.ToString, installationInfoComparisonMapper)
                        End If
                    Case Else
                        If (TypeOf currentProperty Is String) OrElse (IsNumeric(currentProperty)) Then
                            If (compareProperty IsNot Nothing AndAlso currentProperty.ToString <> compareProperty.ToString) OrElse (compareProperty Is Nothing AndAlso currentProperty.ToString <> String.Empty) Then
                                MyBase.ChangedProperties.Add(objProperty.Name, compareProperty)
                            End If
                        End If
                End Select
            ElseIf (compareProperty IsNot Nothing) Then
                Select Case objProperty.Name
                    Case NameOf(Wiring_group.Processing_information)
                        Dim listConvertToDictionary As New ListConvertToDictionary(New List(Of Processing_instruction), DirectCast(compareProperty, IEnumerable(Of Processing_instruction)))
                        Dim processingInfoComparisonMapper As New ProcessingInfoComparisonMapper(Me._owner, _currentKBLMapper, _compareKBLMapper, Nothing, Nothing, listConvertToDictionary, Me.Settings)
                        processingInfoComparisonMapper.CompareObjects()

                        If (processingInfoComparisonMapper.HasChanges) Then
                            MyBase.ChangedProperties.Add(NameOf(Wiring_group.Processing_information), processingInfoComparisonMapper)
                        End If
                    Case NameOf(Wiring_group.Installation_information)
                        Dim listConvertToDictionary As New ListConvertToDictionary(New List(Of Installation_instruction), DirectCast(compareProperty, IEnumerable(Of Installation_instruction)))
                        Dim installationInfoComparisonMapper As New InstallationInfoComparisonMapper(Me._owner, _currentKBLMapper, _compareKBLMapper, Nothing, Nothing, listConvertToDictionary, Me.Settings)
                        installationInfoComparisonMapper.CompareObjects()

                        If (installationInfoComparisonMapper.HasChanges) Then
                            MyBase.ChangedProperties.Add(objProperty.Name.ToString, installationInfoComparisonMapper)
                        End If
                    Case Else
                        MyBase.ChangedProperties.Add(objProperty.Name, compareProperty)
                End Select
            End If
        End If
        Return False
    End Function
End Class
