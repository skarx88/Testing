Namespace D3D.Consolidated.Controls

    <Reflection.Obfuscation(Feature:="renaming", ApplyToMembers:=False, Exclude:=True)> ' needed to avoid resource-file-names obfuscation errors, see issue #5
    Public Class DocumentsListForm

        Public Event ItemVisibilityChanged(sender As Object, e As EventArgs)

        Public Sub New()
            InitializeComponent()
        End Sub

        Private Sub CheckedListBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CheckedListBox1.SelectedIndexChanged
            Dim lbx As CheckedListBox = CType(sender, CheckedListBox)
            Dim selected As Integer = lbx.SelectedIndex

            If selected <> -1 Then
                If lbx.GetItemCheckState(selected) = CheckState.Checked Then
                    DirectCast(lbx.SelectedItem, Consolidated.Designs.Group3D).Visible = True
                ElseIf lbx.GetItemCheckState(selected) = CheckState.Unchecked Then
                    DirectCast(lbx.SelectedItem, Consolidated.Designs.Group3D).Visible = False
                End If
                RaiseEvent ItemVisibilityChanged(Me, New EventArgs)
            End If
        End Sub

        Private Sub FrmHarnessList_Load(sender As Object, e As EventArgs) Handles Me.Load
            Me.CheckedListBox1.DisplayMember = NameOf(Designs.Group3D.Id)
            For Each harness As Designs.Group3D In DirectCast(Me.Parent, Consolidated3DControl).Entities.OfType(Of Designs.Group3D)()
                CheckedListBox1.Items.Add(harness, harness.Visible)
            Next
        End Sub

    End Class
End Namespace