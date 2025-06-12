Namespace D3D.Document.Controls.ToolTips

    Public Class PropertyView

        Public Sub New(name As String, value As String, Optional visible As Boolean = True, Optional failed As Boolean = False, Optional Id As String = "")
            Me.Name = name
            Me.Value = If(String.IsNullOrEmpty(value), Document3DStrings.PropertyEmptyValueText, value)
            Me.KblId = Id
            Me.Visible = visible
            Me.Failed = failed
        End Sub

        ReadOnly Property Name As String
        ReadOnly Property Value As String
        ReadOnly Property KblId As String
        Property Visible As Boolean = True
        Property Failed As Boolean = False

        Public Overrides Function ToString() As String
            Return $"{Name}: {Value}; ({NameOf(Visible)}:{Visible})"
        End Function

    End Class

End Namespace