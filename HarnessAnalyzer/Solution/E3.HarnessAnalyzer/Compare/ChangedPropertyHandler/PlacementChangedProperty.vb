Imports System.Reflection
Imports System.Text
Imports Zuken.E3.HarnessAnalyzer.Settings

Public Class PlacementChangedProperty
    Inherits ChangedProperty

    Public Sub New(owner As ComparisonMapper, currentKBLMapper As KblMapper, compareKBLMapper As KblMapper, settings As GeneralSettingsBase)
        MyBase.New(owner, currentKBLMapper, compareKBLMapper, settings)
    End Sub

    Public Overrides Sub CompareObjectProperties(currentObject As Object, compareObject As Object, excludeProperties As List(Of String))
        Dim compareTransformation As Transformation = DirectCast(compareObject, Transformation)
        Dim currentTransformation As Transformation = DirectCast(currentObject, Transformation)

        If currentTransformation IsNot Nothing Then
            Dim currentSignature As String = GetSignature(currentTransformation, _currentKBLMapper)
            If compareTransformation IsNot Nothing Then
                If IsTransformationSet(currentTransformation) AndAlso IsTransformationSet(compareTransformation) Then
                    Dim compareSignature As String = GetSignature(compareTransformation, _compareKBLMapper)
                    If (currentSignature <> compareSignature) Then
                        MyBase.ChangedProperties.Add(InformationHub.PLACEMENT, Nothing)
                    End If
                End If
            Else
                MyBase.ChangedProperties.Add(InformationHub.PLACEMENT, Nothing)
            End If
        ElseIf compareTransformation IsNot Nothing Then
            MyBase.ChangedProperties.Add(InformationHub.PLACEMENT, Nothing)
        End If
    End Sub

    Public Overrides Function CompareObjectProperty(objProperty As PropertyInfo, currentObject As Object, compareObject As Object, excludeProperties As List(Of String)) As Boolean
        Return False
    End Function

    Private Function IsTransformationSet(transform As Transformation) As Boolean
        'HINT the vector must be reasonable, if all components are zero we do not compare the whole placement (typically 2D case)
        Dim txfSet As Boolean = False
        If transform IsNot Nothing Then
            If (Math.Abs(Math.Round(transform.U(0), 1)) > 0) Then
                txfSet = True
            End If
            If (Math.Abs(Math.Round(transform.U(1), 1)) > 0) Then
                txfSet = True
            End If

            If (transform.U.Length > 2) Then
                If (Math.Abs(Math.Round(transform.U(2), 1)) > 0) Then
                    txfSet = True
                End If
            End If
        End If
        Return txfSet
    End Function

    Private Function GetSignature(transform As Transformation, mapper As KblMapper) As String
        Dim currentTransformSignature As New StringBuilder
        Dim currentCartesianPoint As Cartesian_point = mapper.GetOccurrenceObject(Of Cartesian_point)(transform.Cartesian_point)

        currentTransformSignature.AppendFormat("{0};", Math.Round(currentCartesianPoint.Coordinates(0), 1).ToString)
        currentTransformSignature.AppendFormat("{0};", Math.Round(currentCartesianPoint.Coordinates(1), 1).ToString)

        If (currentCartesianPoint.Coordinates.Length > 2) Then
            currentTransformSignature.AppendFormat("{0};", Math.Round(currentCartesianPoint.Coordinates(2), 1).ToString)
        End If

        currentTransformSignature.AppendFormat("{0};", Math.Round(transform.U(0), 1).ToString)
        currentTransformSignature.AppendFormat("{0};", Math.Round(transform.U(1), 1).ToString)

        If (transform.U.Length > 2) Then
            currentTransformSignature.AppendFormat("{0};", Math.Round(transform.U(2), 1).ToString)
        End If

        currentTransformSignature.AppendFormat("{0};", Math.Round(transform.V(0), 1).ToString)
        currentTransformSignature.AppendFormat("{0};", Math.Round(transform.V(1), 1).ToString)

        If (transform.V.Length > 2) Then
            currentTransformSignature.AppendFormat("{0};", Math.Round(transform.V(2), 1).ToString)
        End If
        Return currentTransformSignature.ToString
    End Function

End Class
