<Reflection.Obfuscation(Feature:="renaming", ApplyToMembers:=False, Exclude:=True)> ' needed to avoid resource-file-names obfuscation errors, see issue #5
Public Class PasteResultForm

    Public Sub New(notMatchingStrings As List(Of String))
        InitializeComponent()
        Initialize(notMatchingStrings)
    End Sub

    Private Sub Initialize(notMatchingStrings As List(Of String))
        Me.BackColor = Color.White
        Me.Icon = My.Resources.MismatchingConfig

        Me.ulvPasteResults.BeginUpdate()

        Dim notMachtingStringCounter As Integer = 0

        For Each notMatchingString As String In notMatchingStrings
            Me.ulvPasteResults.Items.Add(notMatchingString, notMatchingString)

            If (notMachtingStringCounter = 50) Then
                Me.ulvPasteResults.Items.Add(Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS, Zuken.E3.HarnessAnalyzer.Shared.ELLIPSIS)

                Exit For
            End If

            notMachtingStringCounter += 1
        Next

        Me.ulvPasteResults.ItemSettings.DefaultImage = My.Resources.PastedString.ToBitmap
        Me.ulvPasteResults.View = Infragistics.Win.UltraWinListView.UltraListViewStyle.List
        Me.ulvPasteResults.EndUpdate()
    End Sub


    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
    End Sub

    Private Sub btnOK_Click(sender As Object, e As EventArgs) Handles btnOK.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
    End Sub

End Class