<Serializable>
Public Class D3DGridColumnSetting

    Public Sub New()
        Me.New(False)
    End Sub

    Public Sub New(isVisible As Boolean)
        Me.ButtonVisible = isVisible
        Me.PropertyVisible = isVisible
    End Sub

    Property ButtonVisible As Boolean
    Property PropertyVisible As Boolean

    Public Overrides Function ToString() As String
        Return $" {NameOf(ButtonVisible)}:{ButtonVisible};  {NameOf(PropertyVisible)}:{PropertyVisible}"
    End Function

End Class
