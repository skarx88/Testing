Imports Zuken.E3.HarnessAnalyzer.Settings.Schematics.Identifier.Component
Imports Zuken.E3.Lib.CoreTech.Model

Namespace Schematics.Identifier.Component

    <Serializable>
    Public Class IdentifierGroup
        Inherits IdentifierGroupBase

        Public Sub New(componentTypeJT As ComponentTypeJT, Optional componentSuffix As String = "")
            MyBase.New(componentTypeJT, componentSuffix)
        End Sub

        Friend Sub New(componentSuffux As String)
            MyBase.New(componentSuffux)
        End Sub

        Protected Overrides Function GetSpliceComponetType() As Integer
            Throw New NotImplementedException
        End Function

        Protected Overrides Function GetEyeletComponetType() As Integer
            Throw New NotImplementedException
        End Function

        Protected Overrides Function GetInlinerComponetType() As Integer
            Throw New NotImplementedException()
        End Function

        Protected Overrides Function GetEcuComponetType() As Integer
            Throw New NotImplementedException()
        End Function

        Protected Overrides Function GetSvgComponetType() As Integer
            Throw New NotImplementedException()
        End Function

        Protected Overrides Function GetUndefinedComponetType() As Integer
            Throw New NotImplementedException()
        End Function
    End Class

End Namespace