Imports System.Reflection
Imports Zuken.E3.HarnessAnalyzer.Settings

Public Class AliasIdChangedProperty
    Inherits ChangedProperty

    Public Sub New(owner As ComparisonMapper, settings As GeneralSettingsBase)
        MyBase.New(owner, settings)
    End Sub

    Public Overrides Sub CompareObjectProperties(currentObject As Object, compareObject As Object, excludeProperties As List(Of String))
        Dim compareAliasId As Alias_identification = DirectCast(compareObject, Alias_identification)
        Dim currentAliasId As Alias_identification = DirectCast(currentObject, Alias_identification)

        If (currentObject Is Nothing OrElse compareObject Is Nothing) Then Exit Sub

        With ChangedProperties
            If (currentAliasId.Alias_id <> compareAliasId.Alias_id) Then
                .Add(NameOf(Alias_identification.Scope), compareAliasId.Alias_id)
            End If

            If (currentAliasId.Scope IsNot Nothing) Then
                If (compareAliasId.Scope IsNot Nothing) Then
                    If currentAliasId.Scope <> compareAliasId.Scope Then
                        .Add(NameOf(Alias_identification.Scope), compareAliasId.Scope)
                    End If
                Else
                    .Add(NameOf(Alias_identification.Scope), String.Empty)
                End If
            Else
                If compareAliasId.Scope IsNot Nothing Then
                    .Add(NameOf(Alias_identification.Scope), compareAliasId.Scope)
                End If
            End If

            If (currentAliasId.Description IsNot Nothing) Then
                If compareAliasId.Description IsNot Nothing Then
                    If currentAliasId.Description <> compareAliasId.Description Then
                        .Add(NameOf(Alias_identification.Description), compareAliasId.Description)
                    End If
                Else
                    .Add(NameOf(Alias_identification.Description), String.Empty)
                End If
            Else
                If compareAliasId.Description IsNot Nothing Then
                    .Add(NameOf(Alias_identification.Description), compareAliasId.Description)
                End If
            End If
        End With
    End Sub

    Public Overrides Function CompareObjectProperty(objProperty As PropertyInfo, currentObject As Object, compareObject As Object, excludeProperties As List(Of String)) As Boolean
        Return False
    End Function
End Class
