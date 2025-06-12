Namespace Checks.Cavities.Views

    ''' <summary>
    ''' Allow disabling property-column when readonly-property set and object is databound and initialized by [BaseView].InitializeRow
    ''' </summary>
    <AttributeUsage(AttributeTargets.Property, AllowMultiple:=False)>
    Public Class AllowDisableAttribute
        Inherits Attribute

        Public Sub New()
            Me.New(True)
        End Sub

        Public Sub New(allowed As Boolean)
            Me.Allowed = allowed
        End Sub

        ReadOnly Property Allowed As Boolean

    End Class

End Namespace
