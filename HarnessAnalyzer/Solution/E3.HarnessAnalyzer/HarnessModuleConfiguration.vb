Public Class HarnessModuleConfiguration
    Inherits HarnessModuleConfigurationBase

    Private _harnessConfiguration As Harness_configuration

    Public Sub New()
        IsActive = False
    End Sub

    Public Property HarnessConfiguration As Harness_configuration
        Get
            Return _harnessConfiguration
        End Get
        Set
            _harnessConfiguration = Value
        End Set
    End Property

    Public Overrides Property HarnessConfigurationObject As IKblBaseObject
        Get
            Return HarnessConfiguration
        End Get
        Set(value As IKblBaseObject)
            Me.HarnessConfiguration = CType(value, Harness_configuration)
        End Set
    End Property

    Protected Overrides Function GetHarnessConfigurationPartNumber() As String
        Return Me.HarnessConfiguration.Part_number
    End Function

End Class
