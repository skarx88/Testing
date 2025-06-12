Imports System.Runtime.Serialization

Namespace QualityStamping.Specification

    <DataContract(NameSpace:="Zuken.E3.HarnessAnalyzer.QualityStamping.Specification")>
    Public Class QMStampSpecification

        <DataMember> Property Specification As String

        Public Sub New()
        End Sub

        Public Sub New(spec As String)
            Specification = spec
        End Sub

    End Class

End Namespace