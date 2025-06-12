Imports Infragistics.Win
Imports Infragistics.Win.UltraWinGrid
Imports Zuken.E3.Lib.Schema.Kbl.Properties

Partial Public Class InformationHub

    Protected Friend Class ValidityCheckCreationFilter
        Implements IUIElementCreationFilter

        Private _validityCheckContainer As ValidityCheckContainer = Nothing

        Public Sub New(parentForm As Form)
            _validityCheckContainer = If(TypeOf parentForm Is DocumentForm, DirectCast(parentForm, DocumentForm)._validityCheckContainer, Nothing)
        End Sub

        Public Sub AfterCreateChildElements(parent As UIElement) Implements IUIElementCreationFilter.AfterCreateChildElements
            If (_validityCheckContainer Is Nothing) Then
                Exit Sub
            End If

            If (TypeOf parent Is CellUIElement) Then
                Dim cell As UltraGridCell = Nothing
                Dim id As String = String.Empty
                Dim partNumbers As New List(Of String)

                Select Case parent.Control.Name
                    Case NameOf(ugAccessories), NameOf(ugAssemblyParts), NameOf(ugComponentBoxes), NameOf(ugComponents), NameOf(ugDimSpecs), NameOf(ugFixings)
                        cell = If(DirectCast(parent, CellUIElement).Cell.Column.Key = CommonPropertyName.Id, DirectCast(parent, CellUIElement).Cell, Nothing)
                        If (cell IsNot Nothing) Then
                            id = cell.Value.ToString
                            If (parent.Control.Name <> NameOf(ugDimSpecs)) Then
                                partNumbers.Add(cell.Row.Cells(PartPropertyName.Part_number).Value.ToString)
                            End If
                        End If
                    Case NameOf(ugCables)
                        cell = If(DirectCast(parent, CellUIElement).Cell.Column.Key = CablePropertyName.Special_wire_id OrElse DirectCast(parent, CellUIElement).Cell.Column.Key = WirePropertyName.Wire_number, DirectCast(parent, CellUIElement).Cell, Nothing)
                        If (cell IsNot Nothing) Then
                            id = cell.Value.ToString
                            If (cell.Row.HasChild) Then
                                partNumbers.Add(cell.Row.Cells(PartPropertyName.Part_number).Value.ToString)
                            End If
                        End If
                    Case NameOf(ugConnectors)
                        cell = If(DirectCast(parent, CellUIElement).Cell.Column.Key = ConnectorPropertyName.Id OrElse DirectCast(parent, CellUIElement).Cell.Column.Key = ConnectorPropertyName.Cavity_number, DirectCast(parent, CellUIElement).Cell, Nothing)
                        If (cell IsNot Nothing) Then
                            id = If(cell.Row.HasChild, cell.Value.ToString, String.Format("{0},{1}", cell.Row.ParentRow.Cells(ConnectorPropertyName.Id).Value.ToString, cell.Value.ToString))

                            If (cell.Row.HasChild) Then
                                partNumbers.Add(cell.Row.Cells(PartPropertyName.Part_number).Value.ToString)
                            Else
                                partNumbers.Add(cell.Row.Cells(ConnectorPropertyName.Terminal_part_number).Value.ToString)
                                partNumbers.Add(cell.Row.Cells(ConnectorPropertyName.Seal_part_number).Value.ToString)
                                partNumbers.Add(cell.Row.Cells(ConnectorPropertyName.Plug_part_number).Value.ToString)
                            End If
                        End If
                    Case NameOf(ugHarness)
                        cell = If(DirectCast(parent, CellUIElement).Cell.Column.Key = NameOf(InformationHubStrings.PropVal_ColumnCaption) AndAlso DirectCast(parent, CellUIElement).Cell.Row.Cells(NameOf(InformationHubStrings.PropName_ColumnCaption)).Value.ToString = InformationHubStrings.PartNumber_Caption, DirectCast(parent, CellUIElement).Cell, Nothing)

                        If (cell IsNot Nothing) Then
                            partNumbers.Add(cell.Value.ToString)
                        End If
                    Case NameOf(ugModules)
                        cell = If(DirectCast(parent, CellUIElement).Cell.Column.Key = PartPropertyName.Abbreviation, DirectCast(parent, CellUIElement).Cell, Nothing)

                        If (cell IsNot Nothing) Then
                            partNumbers.Add(cell.Row.Cells(PartPropertyName.Part_number).Value.ToString)
                        End If
                    Case NameOf(ugSegments), NameOf(ugVertices)
                        cell = If(DirectCast(parent, CellUIElement).Cell.Column.Key = CommonPropertyName.Id, DirectCast(parent, CellUIElement).Cell, Nothing)
                        If (cell IsNot Nothing) Then
                            id = cell.Value.ToString

                            If (cell.Row.HasParent) Then
                                partNumbers.Add(cell.Row.Cells(PartPropertyName.Part_number).Value.ToString)
                            End If
                        End If
                    Case NameOf(ugWires)
                        cell = If(DirectCast(parent, CellUIElement).Cell.Column.Key = WirePropertyName.Wire_number, DirectCast(parent, CellUIElement).Cell, Nothing)
                        If (cell IsNot Nothing) Then
                            id = cell.Value.ToString
                            partNumbers.Add(cell.Row.Cells(PartPropertyName.Part_number).Value.ToString)
                        End If
                End Select

                If (cell IsNot Nothing) AndAlso (_validityCheckContainer.ValidityCheckEntries.Any(Function(entry) (id <> String.Empty AndAlso entry.ID = id) OrElse (partNumbers.Contains(entry.ID)))) Then
                    Dim buttonElement As New ButtonUIElementWithImageAndTag(parent, My.Resources.ValidityCheck_Small.ToBitmap, _validityCheckContainer.ValidityCheckEntries.Where(Function(entry) (id <> String.Empty AndAlso entry.ID = id) OrElse (partNumbers.Contains(entry.ID))).ToList)

                    AddHandler buttonElement.ElementClick, AddressOf OnButtonClick

                    Dim rect As Rectangle = parent.RectInsideBorders
                    With rect
                        .X = .Right - 18
                        .Width = 16
                        .Height = 16
                    End With

                    buttonElement.Rect = rect

                    For Each child As UIElement In parent.ChildElements
                        Dim childRect As Rectangle = child.Rect
                        If (childRect.Left < rect.Right) Then
                            childRect.Width -= 18
                            child.Rect = childRect
                        End If
                    Next

                    parent.ChildElements.Add(buttonElement)

                    cell.ToolTipText = InformationHubStrings.ObjectContainsOneOrMoreValidityCheckResults_TooltipText
                ElseIf (cell IsNot Nothing) AndAlso (cell.Row.HasChild) AndAlso ((parent.Control.Name = NameOf(ugCables) AndAlso cell.Column.Key = CablePropertyName.Special_wire_id.ToString) _
                        OrElse (parent.Control.Name = NameOf(ugConnectors) AndAlso cell.Column.Key = ConnectorPropertyName.Id) _
                        OrElse (parent.Control.Name = NameOf(ugSegments) AndAlso cell.Column.Key = SegmentPropertyName.Id) _
                        OrElse (parent.Control.Name = NameOf(ugVertices) AndAlso cell.Column.Key = VertexPropertyName.Id)) _
                    Then

                    Dim hasValidityCheckResults As Boolean = False
                    For Each childRow As UltraGridRow In cell.Row.ChildBands(0).Rows
                        Select Case parent.Control.Name
                            Case NameOf(ugCables)
                                hasValidityCheckResults = _validityCheckContainer.ValidityCheckEntries.Any(Function(entry) entry.ID = childRow.Cells(WirePropertyName.Wire_number.ToString).Value.ToString)
                            Case NameOf(ugConnectors)
                                partNumbers.Clear()
                                partNumbers.Add(childRow.Cells(ConnectorPropertyName.Terminal_part_number).Value.ToString)
                                partNumbers.Add(childRow.Cells(ConnectorPropertyName.Seal_part_number).Value.ToString)
                                partNumbers.Add(childRow.Cells(ConnectorPropertyName.Plug_part_number).Value.ToString)
                                hasValidityCheckResults = _validityCheckContainer.ValidityCheckEntries.Any(Function(entry) entry.ID = String.Format("{0},{1}", cell.Value.ToString, childRow.Cells(ConnectorPropertyName.Cavity_number.ToString).Value.ToString) OrElse (partNumbers.Contains(entry.ID)))
                            Case NameOf(ugSegments), NameOf(ugVertices)
                                partNumbers.Clear()
                                partNumbers.Add(childRow.Cells(PartPropertyName.Part_number).Value.ToString)
                                hasValidityCheckResults = _validityCheckContainer.ValidityCheckEntries.Any(Function(entry) entry.ID = childRow.Cells(WireProtectionPropertyName.Id.ToString).Value.ToString OrElse (partNumbers.Contains(entry.ID)))
                        End Select

                        If (hasValidityCheckResults) Then
                            Exit For
                        End If
                    Next

                    If (hasValidityCheckResults) Then
                        Dim imageElement As New ImageUIElement(parent, My.Resources.ValidityCheckIndicator.ToBitmap)
                        Dim rect As Rectangle = parent.RectInsideBorders
                        With rect
                            .X = .Right - 18
                            .Width = 16
                            .Height = 16
                        End With

                        imageElement.Rect = rect

                        For Each child As UIElement In parent.ChildElements
                            Dim childRect As Rectangle = child.Rect
                            If (childRect.Left < rect.Right) Then
                                childRect.Width -= 18
                                child.Rect = childRect
                            End If
                        Next

                        parent.ChildElements.Add(imageElement)

                        cell.ToolTipText = InformationHubStrings.ChildObjectContainsOneOrMoreValidityCheckResults_TooltipText
                    End If
                End If
            End If
        End Sub

        Public Function BeforeCreateChildElements(parent As UIElement) As Boolean Implements IUIElementCreationFilter.BeforeCreateChildElements
            Return False
        End Function

        Private Sub OnButtonClick(sender As Object, e As UIElementEventArgs)
            Using validityCheckForm As New ValidityCheckForm(DirectCast(DirectCast(e.Element, ButtonUIElementWithImageAndTag).Tag, List(Of ValidityCheckEntry)))
                validityCheckForm.ShowDialog(e.Element.Control)
            End Using
        End Sub

    End Class

End Class
