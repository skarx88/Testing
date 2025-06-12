Imports Zuken.E3.HarnessAnalyzer.Settings.Schematics.Identifier.Component

Namespace Schematics.Identifier.Component

    Public Class IdentifierGroupCollection
        Inherits IdentifierGroupCollectionBase

        Protected Overrides Function CreateNewInstance(componentSuffix As String) As IdentifierGroupBase
            Return New IdentifierGroup(componentSuffix)
        End Function

    End Class

End Namespace
