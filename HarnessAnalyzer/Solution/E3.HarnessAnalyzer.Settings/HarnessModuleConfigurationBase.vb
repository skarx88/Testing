Imports Zuken.E3.HarnessAnalyzer.Shared.Common
Imports Zuken.E3.Lib.Schema.KBL

Namespace Global.Zuken.E3.HarnessAnalyzer

    Public MustInherit Class HarnessModuleConfigurationBase

        Private _IsActive As Boolean
        Private _configurationType As HarnessModuleConfigurationType

        Public Sub New()
            IsActive = False
        End Sub

        Public Property ConfigurationType As HarnessModuleConfigurationType
            Get
                Return _configurationType
            End Get
            Set
                _configurationType = Value
            End Set
        End Property

        Public MustOverride Property HarnessConfigurationObject As IKblBaseObject

        Protected MustOverride Function GetHarnessConfigurationPartNumber() As String

        Public Property IsActive As Boolean
            Get
                Return _IsActive
            End Get
            Set
                If _IsActive <> Value Then
                    _IsActive = Value
                End If
            End Set
        End Property

        Public Overrides Function ToString() As String
            Return $"[{Me.GetType.Name}-{ConfigurationType.ToString}]: ""{GetHarnessConfigurationPartNumber()}"" {If(IsActive, "(Active)", String.Empty)}"
        End Function

    End Class

End Namespace