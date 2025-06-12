Imports System.Runtime.CompilerServices
Imports Zuken.E3.App.Controls

Namespace Schematics.Converter.Kbl

    <HideModuleName>
    Friend Module ConverterExtensions

        <Extension>
        Public Sub AddCavities(wire As Connectivity.Model.Wire, cavities As IEnumerable(Of Connectivity.Model.Cavity))
            For Each cav As Connectivity.Model.Cavity In cavities
                wire.AddCavity(cav)
            Next
        End Sub

    End Module

End Namespace