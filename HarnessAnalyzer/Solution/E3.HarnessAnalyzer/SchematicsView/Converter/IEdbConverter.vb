Imports System.ComponentModel
Imports Zuken.E3.HarnessAnalyzer.Schematics.Controls

Namespace Schematics.Converter

    Public Interface IEdbConverter

        Event ResolveEntityId(sender As Object, e As IdEventArgs)
        Event AfterEdbEntityCreated(sender As Object, e As EntityEventArgs)
        Event IKblConverter_BeforeConversionStart(sender As Object, e As CancelEventArgs)
        Event IKblConverter_AfterConversionFinished(sender As Object, e As FinishedEventArgs)

        ReadOnly Property Id As Guid

    End Interface

End Namespace