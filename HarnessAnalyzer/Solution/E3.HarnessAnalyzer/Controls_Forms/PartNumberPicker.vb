Imports Infragistics.Win.UltraWinGrid

<Reflection.Obfuscation(Feature:="renaming", ApplyToMembers:=False, Exclude:=True)> ' needed to avoid resource-file-names obfuscation errors, see issue #5
Public Class PartNumberPicker

    Public Property DataSource As Object
        Get
            Return ucPartNumbers.DataSource
        End Get
        Set(value As Object)
            ucPartNumbers.DataSource = value
        End Set

    End Property

    Public ReadOnly Property SelectedRow As UltraGridRow
        Get
            Return ucPartNumbers.SelectedRow
        End Get
    End Property


    Private Sub ubtnOk_Click(sender As Object, e As EventArgs) Handles ubtnOk.Click
        If (ucPartNumbers.SelectedRow IsNot Nothing) Then
            Me.DialogResult = DialogResult.OK
        Else
            Me.DialogResult = DialogResult.Cancel
        End If

    End Sub

    Private Sub ubtnCancel_Click(sender As Object, e As EventArgs) Handles ubtnCancel.Click
        Me.DialogResult = DialogResult.Cancel
    End Sub

    Private Sub ucPartNumbers_InitializeLayout(sender As Object, e As InitializeLayoutEventArgs) Handles ucPartNumbers.InitializeLayout

        With Me.ucPartNumbers.DisplayLayout
            With .Bands(0)
                .Columns(BundleCrossSectionForm.ColumnKeys.OutsideDiaVal.ToString).Hidden = True
                .Columns(BundleCrossSectionForm.ColumnKeys.GeneralWire.ToString).Hidden = True

                With .Columns(BundleCrossSectionForm.ColumnKeys.PartNumber.ToString)
                    .SortIndicator = SortIndicator.Ascending
                    .Width = 150
                    .Header.Caption = BundleCrossSectionFormStrings.PartNumber_ColumnCaption
                End With
                With .Columns(BundleCrossSectionForm.ColumnKeys.OutsideDia.ToString)
                    .Width = 75
                    .Header.Caption = BundleCrossSectionFormStrings.OutsideDia_ColumnCaption
                End With
                With .Columns(BundleCrossSectionForm.ColumnKeys.Source.ToString)
                    .Width = 100
                    .Header.Caption = BundleCrossSectionFormStrings.Source_ColumnCaption
                End With

            End With
        End With

        Me.ucPartNumbers.ValueMember = BundleCrossSectionForm.ColumnKeys.GeneralWire.ToString
        Me.ucPartNumbers.DisplayMember = BundleCrossSectionForm.ColumnKeys.PartNumber.ToString
    End Sub
End Class