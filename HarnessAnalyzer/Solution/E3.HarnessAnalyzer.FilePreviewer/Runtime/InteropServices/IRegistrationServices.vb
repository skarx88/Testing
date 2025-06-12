
Imports System.Reflection
Imports System.Runtime.InteropServices
Imports System.Security

<Guid("CCBD682C-73A5-4568-B8B0-C7007E11ABA2")>
<ComVisible(True)>
Friend Interface IRegistrationServices
    <SecurityCritical>
    Function RegisterAssembly(ByVal assembly As Assembly, ByVal flags As AssemblyRegistrationFlags) As Boolean
    <SecurityCritical>
    Function UnregisterAssembly(ByVal assembly As Assembly) As Boolean
    <SecurityCritical>
    Function GetRegistrableTypesInAssembly(ByVal assembly As Assembly) As Type()
    <SecurityCritical>
    Function GetProgIdForType(ByVal type As Type) As String
    <SecurityCritical>
    Sub RegisterTypeForComClients(ByVal type As Type, ByRef g As Guid)
    Function GetManagedCategoryGuid() As Guid
    <SecurityCritical>
    Function TypeRequiresRegistration(ByVal type As Type) As Boolean
    Function TypeRepresentsComType(ByVal type As Type) As Boolean
End Interface
