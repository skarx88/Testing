Imports System.Collections.ObjectModel

Namespace Global.Zuken.E3.HarnessAnalyzer

    Public Class HarnessModuleConfigurationCollection
        Inherits Collection(Of HarnessModuleConfigurationBase)

        Public Sub New()
            MyBase.New
        End Sub

        Protected Overrides Sub InsertItem(index As Integer, item As HarnessModuleConfigurationBase)
            MyBase.InsertItem(index, item)
        End Sub

        Public Function GetActiveConfiguration() As HarnessModuleConfigurationBase
            Return Me.Where(Function(cfg) cfg.IsActive).SingleOrDefault
        End Function

    End Class

End Namespace