Imports Zuken.E3.App.Controls
Imports Zuken.E3.HarnessAnalyzer.Schematics.Converter
Imports Zuken.E3.HarnessAnalyzer.Schematics.Converter.Kbl

Namespace Schematics.Controls

    Partial Public Class ViewControl

        Public Function ContainsItem(edbIdOrModuleFunctionId As String) As Boolean
            Return _functionSet.ContainsKey(edbIdOrModuleFunctionId) OrElse _functionSet.ContainsKey(edbIdOrModuleFunctionId) OrElse Me.Entities.Contains(edbIdOrModuleFunctionId)
        End Function

        Public Sub Highlight(ParamArray entitiyIds() As String)
            If _ctrlView IsNot Nothing Then
                If Me.ModelIsAvail Then
                    _ctrlView.Highlight(entitiyIds.ToList)
                End If
            End If
        End Sub

        Friend Function TryGetEdbItems(entityId As String) As Connectivity.Model.BaseItem()
            Return ResolveModelItemsFrom(IdConverter.ResolveEntitiesFromDocuments(Entities, entityId)).ToArray
        End Function

        Friend Function TryGetModuleName(entityId As String, ByRef name As String) As Boolean
            Return _modulesSet.TryGetValue(entityId, name)
        End Function

        Friend Function TryGetFunctionName(entityId As String, ByRef name As String) As Boolean
            Return _functionSet.TryGetValue(entityId, name)
        End Function

        Private Function ResolveModelItemsFrom(entities As IEnumerable(Of EdbConversionEntity)) As List(Of Connectivity.Model.BaseItem)
            Dim list As New List(Of Connectivity.Model.BaseItem)
            For Each ent As EdbConversionEntity In entities
                If TypeOf ent Is EdbConversionCavityGroupEntity Then
                    list.AddRange(IdConverter.ResolveEntitiesFromDocuments(Me.Entities, CType(ent, EdbConversionCavityGroupEntity).ToArray).Select(Function(be) be.EdbItem))
                Else
                    list.Add(ent.EdbItem)
                End If
            Next
            Return list.Distinct.ToList
        End Function

    End Class

End Namespace
