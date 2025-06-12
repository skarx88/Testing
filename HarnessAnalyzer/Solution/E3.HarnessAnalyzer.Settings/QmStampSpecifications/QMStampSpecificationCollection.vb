Imports System.Runtime.Serialization

Namespace QualityStamping.Specification

    <CollectionDataContract(Namespace:="Zuken.E3.HarnessAnalyzer.QualityStamping.Specification")>
    Public Class QMStampSpecificationCollection
        Inherits System.Collections.ObjectModel.Collection(Of QMStampSpecification)

        Public Sub New()
        End Sub

    End Class

End Namespace