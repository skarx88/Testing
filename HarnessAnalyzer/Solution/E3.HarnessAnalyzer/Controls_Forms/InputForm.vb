<Reflection.Obfuscation(Feature:="renaming", ApplyToMembers:=False, Exclude:=True)> ' needed to avoid resource-file-names obfuscation errors, see issue #5
Public Class InputForm

    Private _responseFromList As Boolean

    Public Sub New(propmt As String, Optional title As String = [Shared].MSG_BOX_TITLE, Optional defaultResponse As String = "", Optional valueList As Infragistics.Win.ValueList = Nothing)
        InitializeComponent()

        Me.Text = title
        Me.lblPrompt.Text = propmt

        If (valueList IsNot Nothing) Then
            _responseFromList = True

            Me.txtResponse.Visible = False
            Me.uceResponse.ValueList = valueList
            Me.uceResponse.SelectedItem = Me.uceResponse.ValueList.ValueListItems(0)
        Else
            Me.txtResponse.Text = defaultResponse
        End If
    End Sub

    Private Sub InputForm_Activated(sender As Object, e As EventArgs) Handles Me.Activated
        Me.txtResponse.Focus()
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
    End Sub

    Private Sub btnOK_Click(sender As Object, e As EventArgs) Handles btnOK.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
    End Sub

    Friend ReadOnly Property Response() As String
        Get
            If (_responseFromList) Then
                Return Me.uceResponse.SelectedItem.DisplayText
            Else
                Return Me.txtResponse.Text
            End If
        End Get
    End Property

End Class