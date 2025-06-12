Imports Zuken.E3.HarnessAnalyzer.Settings

Namespace Checks.Cavities.Views.Model

    Public Class VirtualCavityWireView
        Inherits CavityWireView

        Public Sub New(cavity As Cavity_occurrence, kbl As KBLMapper, generalSettings As GeneralSettings)
            MyBase.New(wire:=Nothing, cavity:=cavity, kbl:=kbl, generalSettings:=generalSettings)
        End Sub

        Protected Overrides Sub InitWireProperties(wire As Wire_occurrence, kbl As KBLMapper)
            ' do nothing here...
        End Sub

        <DebuggerNonUserCode>
        Public Overrides ReadOnly Property WireName As String
            <DebuggerNonUserCode>
            Get
                Return Zuken.E3.HarnessAnalyzer.Shared.NOT_AVAILABLE '$"{CavityName}: N/A" ' HINT: the value here is needed for databinding in ToolStripItemComboBox in CavityNavigator which tries to read out from this property. When nothing the .text-property can't be accessed in ComboBox and application will crash at this moment
            End Get
        End Property

        <DebuggerNonUserCode>
        Public Overrides ReadOnly Property IsVirtual As Boolean
            <DebuggerNonUserCode>
            Get
                Return True
            End Get
        End Property

    End Class

End Namespace