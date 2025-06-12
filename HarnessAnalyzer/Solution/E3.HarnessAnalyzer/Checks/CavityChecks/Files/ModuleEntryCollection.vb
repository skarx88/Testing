Imports System.Runtime.Serialization
Imports System.Xml.Serialization

Namespace Checks.Cavities.Files

    <KnownType(GetType(ModuleEntry))>
    <CollectionDataContract(Name:="Modules", ItemName:="Module", [Namespace]:=CavityChecksFile.Namespace)>
    <XmlType(TypeName:="Modules")> ' for schema generation
    Public Class ModuleEntryCollection
        Inherits ObjectModel.KeyedCollection(Of String, ModuleEntry)

        Public Sub New()
        End Sub

        Friend Sub New(ccFile As CavityChecksFile)
            Me.File = ccFile
        End Sub

        Protected Overrides Function GetKeyForItem(item As ModuleEntry) As String
            Return item.SystemId
        End Function

        Public Sub UpdateAll()
            Me.Clear()

            For Each m As [Module] In File.Document.Model.GetModules
                If File.Document.Model.Settings.ActiveModules.Contains(m.SystemId) Then
                    Me.AddNew(m)
                End If
            Next
        End Sub

        Public Function AddNew([module] As [Module]) As ModuleEntry
            Dim mEntry As New ModuleEntry([module])
            Me.Add(mEntry)
            Return mEntry
        End Function

        Friend Property File As CavityChecksFile

    End Class

End Namespace