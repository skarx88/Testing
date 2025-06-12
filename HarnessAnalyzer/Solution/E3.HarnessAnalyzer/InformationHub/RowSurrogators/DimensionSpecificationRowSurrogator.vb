Imports System.Data.Common
Imports System.Xml.Serialization
Imports Infragistics.Win.UltraWinDataSource
Imports Zuken.E3.Lib.Schema.Kbl.Properties

Friend Class DimensionSpecificationRowSurrogator
    Inherits ComparisonRowSurrogator(Of Dimension_specification, DimSpecPartDummy)

    Public Sub New(kbl_reference As IKblContainer, kbl_compare As IKblContainer, corresponding_DS As UltraDataSource, mainForm As MainForm)
        MyBase.New(kbl_reference, kbl_compare, corresponding_DS, mainForm)
    End Sub

    Protected Overrides Function GetCompareCellValueCore(row As UltraDataRow, column As UltraDataColumn, reference As Boolean) As Object
        Select Case column.Key
            Case DimensionSpecificationPropertyName.Dimension_value
                If Me.RowOccurrence.Dimension_value IsNot Nothing Then
                    Return MyBase.GetNumericalCellValue(Me.RowOccurrence.Dimension_value, 1)
                End If
            Case DimensionSpecificationPropertyName.Segments
                If Me.RowOccurrence.Segments IsNot Nothing Then
                    Dim kbl As IKblContainer = If(IsReference, Me.ReferenceObj.Kbl, Me.CompareObj?.Kbl)
                    If kbl IsNot Nothing Then
                        Dim user_ids As New List(Of String)
                        For Each segmentId As String In Me.RowOccurrence.Segments.SplitSpace
                            user_ids.Add(kbl.GetSegment(segmentId).Id)
                        Next
                        Return String.Join(" ;", user_ids)
                    Else
                        Return String.Empty
                    End If
                End If
            Case DimensionSpecificationPropertyName.Origin
                If (Me.RowOccurrence.Origin IsNot Nothing) Then
                    If CurrentKbl IsNot Nothing Then
                        Dim fixing_ass As Fixing_assignment = CurrentKbl.GetOccurrenceObject(Of Fixing_assignment)(Me.RowOccurrence.Origin)
                        If fixing_ass IsNot Nothing Then
                            If ([Lib].Schema.Kbl.Utils.GetUserId(fixing_ass) Is Nothing) Then
                                Return [Lib].Schema.Kbl.Utils.GetUserId(CurrentKbl.GetFixingOccurrence(fixing_ass.Fixing))
                            Else
                                Return [Lib].Schema.Kbl.Utils.GetUserId(fixing_ass)
                            End If
                        Else
                            Return [Lib].Schema.Kbl.Utils.GetUserId(CurrentKbl.GetOccurrenceObjectUntyped(RowOccurrence.Origin))
                        End If
                    End If
                End If
            Case DimensionSpecificationPropertyName.Target.ToString
                If (Me.RowOccurrence.Target IsNot Nothing) Then
                    Dim fixing_ass As Fixing_assignment = CurrentKbl.GetOccurrenceObject(Of Fixing_assignment)(Me.RowOccurrence.Target)
                    If fixing_ass IsNot Nothing Then
                        If ([Lib].Schema.Kbl.Utils.GetUserId(fixing_ass) Is Nothing) Then
                            Return [Lib].Schema.Kbl.Utils.GetUserId(CurrentKbl.GetFixingOccurrence(fixing_ass.Fixing))
                        Else
                            Return [Lib].Schema.Kbl.Utils.GetUserId(fixing_ass)
                        End If
                    Else
                        Return [Lib].Schema.Kbl.Utils.GetUserId(CurrentKbl.GetOccurrenceObjectUntyped(RowOccurrence.Target))
                    End If
                End If
            Case DimensionSpecificationPropertyName.Processing_Information
                Return GetEllipsisPropertyValue(column.Key, CompareSurrogatorObjectSource.RowOccurrence)
            Case DimensionSpecificationPropertyName.Tolerance_indication
                Return GetToleranceIndicationCellValue(Me, Me.RowOccurrence.Tolerance_indication)
            Case Else
                Return MyBase.GetCompareCellValueCore(row, column, reference)
        End Select
        Return Nothing
    End Function

    Public Shared Function GetToleranceIndicationCellValue(surrogator As IKBLCompareSurrogator, tolerance_indication As Tolerance) As String
        If (tolerance_indication IsNot Nothing) Then
            If (tolerance_indication.Lower_limit IsNot Nothing) AndAlso (tolerance_indication.Upper_limit IsNot Nothing) Then
                Return String.Format("{1} / {2}", GetToleranceIndicationCellValue(surrogator, ObjectPropertyNameStrings.LowerLimit, tolerance_indication.Lower_limit), GetToleranceIndicationCellValue(surrogator, ObjectPropertyNameStrings.UpperLimit, tolerance_indication.Upper_limit))
            ElseIf (tolerance_indication.Lower_limit IsNot Nothing) Then
                Return GetToleranceIndicationCellValue(surrogator, ObjectPropertyNameStrings.LowerLimit, tolerance_indication.Lower_limit)
            Else
                Return GetToleranceIndicationCellValue(surrogator, ObjectPropertyNameStrings.UpperLimit, tolerance_indication.Upper_limit)
            End If
        End If
        Return String.Empty
    End Function

    Private Shared Function GetToleranceIndicationCellValue(surrogator As IKBLCompareSurrogator, description As String, numerical_value As Numerical_value) As String
        Return String.Format("{0}: {1}", description, GetNumericalCellValue(surrogator, numerical_value, 1))
    End Function

    ReadOnly Property CurrentKbl As IKblContainer
        Get
            If Not IsReference AndAlso Me.HasCompare Then
                Return CompareObj.Kbl
            End If
            Return ReferenceObj.Kbl
        End Get
    End Property

    Public Class DimSpecPartDummy
        Inherits Part

        Public Sub New()
        End Sub
    End Class

End Class

