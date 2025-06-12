Imports System.Runtime.InteropServices
Imports System
Imports System.Collections
Imports System.IO
Imports System.Reflection
Imports System.Security
Imports System.Security.Permissions
Imports System.Text
Imports System.Threading
Imports Microsoft.Win32
Imports System.Runtime.CompilerServices
Imports System.Globalization
Imports System.Runtime.Versioning
Imports System.Diagnostics.Contracts



''' <summary>
''' This is a extended version of System.Runtime.InteropServices.RegistrationServices which has the extended functionality to specify the registry-key target where the registration data should be written.
''' The <see cref="RegistrationServicesex"/> can only write to the HKEY_LOCAL_MACHINE-Key in the registry which always needs admin rigths. This version can also register the com-class to the current user without needing admin rights!
''' </summary>
Friend Class RegistrationServicesEx
        Implements IRegistrationServices

        Private Const strManagedCategoryGuid As String = "{62C8FE65-4EBB-45e7-B440-6E39B2CDBF29}"
        Private Const strDocStringPrefix As String = ""
        Private Const strManagedTypeThreadingModel As String = "Both"
        Private Const strComponentCategorySubKey As String = "Component Categories"
        Private Const strManagedCategoryDescription As String = ".NET Category"
        Private Const strImplementedCategoriesSubKey As String = "Implemented Categories"
        Private Const strMsCorEEFileName As String = "mscoree.dll"
        Private Const strRecordRootName As String = "Record"
        Private Const strClsIdRootName As String = "CLSID"
        Private Const strTlbRootName As String = "TypeLib"
        Private Shared s_ManagedCategoryGuid As New Guid(strManagedCategoryGuid)

        Friend Overridable Function RegisterAssembly(ByVal assembly As Assembly, ByVal flags As AssemblyRegistrationFlags, target As RegistryTarget) As Boolean
            If assembly Is Nothing Then
                Throw New ArgumentNullException("assembly")
            End If

            If assembly.ReflectionOnly Then
                Throw New InvalidOperationException("InvalidOperation_AsmLoadedForReflectionOnly")
            End If

            Contract.EndContractBlock()
            If assembly.GetType.ToString <> "System.Reflection.RuntimeAssembly" Then
                Throw New ArgumentException("Argument_MustBeRuntimeAssembly")
            End If
            Dim strAsmName As String = assembly.FullName
            If strAsmName Is Nothing Then Throw New InvalidOperationException("InvalidOperation_NoAsmName")
            Dim strAsmCodeBase As String = Nothing

            If (flags And AssemblyRegistrationFlags.SetCodeBase) <> 0 Then
            strAsmCodeBase = CStr(assembly.GetType.GetMethods(BindingFlags.NonPublic Or BindingFlags.Public Or BindingFlags.Instance).Where(Function(m) m.Name = "GetCodeBase" AndAlso m.GetParameters.Count = 1).FirstOrDefault?.Invoke(assembly, {False}))
            If String.IsNullOrEmpty(strAsmCodeBase) AndAlso Not String.IsNullOrEmpty(assembly.Location) Then
                strAsmCodeBase = New Uri(assembly.Location).ToString
            End If
            If strAsmCodeBase Is Nothing Then
                Throw New InvalidOperationException("InvalidOperation_NoAsmCodeBase")
            End If
        End If

        Dim aTypes As Type() = GetRegistrableTypesInAssembly(assembly)
        Dim NumTypes As Integer = aTypes.Length

        Dim strAsmVersion As String = assembly.GetType.GetMethod("GetVersion", BindingFlags.NonPublic Or BindingFlags.Public Or BindingFlags.Instance).Invoke(assembly, {}).ToString
        Dim strRuntimeVersion As String = assembly.ImageRuntimeVersion

        For cTypes As Integer = 0 To NumTypes - 1

            If IsRegisteredAsValueType(aTypes(cTypes)) Then
                RegisterValueType(aTypes(cTypes), strAsmName, strAsmVersion, strAsmCodeBase, strRuntimeVersion, target)
            ElseIf TypeRepresentsComType(aTypes(cTypes)) Then
                RegisterComImportedType(aTypes(cTypes), strAsmName, strAsmVersion, strAsmCodeBase, strRuntimeVersion, target)
            Else
                RegisterManagedType(target, aTypes(cTypes), strAsmName, strAsmVersion, strAsmCodeBase, strRuntimeVersion)
            End If

            CallUserDefinedRegistrationMethod(aTypes(cTypes), True, target)
        Next

        Dim aPIAAttrs As Object() = assembly.GetCustomAttributes(GetType(PrimaryInteropAssemblyAttribute), False)
        Dim NumPIAAttrs As Integer = aPIAAttrs.Length

        For cPIAAttrs As Integer = 0 To NumPIAAttrs - 1
            RegisterPrimaryInteropAssembly(assembly, strAsmCodeBase, CType(aPIAAttrs(cPIAAttrs), PrimaryInteropAssemblyAttribute), target)
        Next

        If aTypes.Length > 0 OrElse NumPIAAttrs > 0 Then
            Return True
        Else
            Return False
        End If
    End Function

    <System.Security.SecurityCritical>
    <ResourceExposure(ResourceScope.None)>
    <ResourceConsumption(ResourceScope.Machine, ResourceScope.Machine)>
    Friend Overridable Function RegisterAssembly(ByVal assembly As Assembly, ByVal flags As AssemblyRegistrationFlags) As Boolean Implements IRegistrationServices.RegisterAssembly
        Return RegisterAssembly(assembly, flags, RegistryTarget.Machine)
    End Function

    Friend Overridable Function UnregisterAssembly(ByVal assembly As Assembly, target As RegistryTarget) As Boolean
        If assembly Is Nothing Then
            Throw New ArgumentNullException("assembly")
        End If
        If assembly.ReflectionOnly Then
            Throw New InvalidOperationException("InvalidOperation_AsmLoadedForReflectionOnly")
        End If
        Contract.EndContractBlock()
        Dim classesRoot As RegistryKey = GetClassesRoot(target)

        If assembly.GetType.ToString <> "System.Reflection.RuntimeAssembly" Then
            Throw New ArgumentException("Argument_MustBeRuntimeAssembly")
        End If

        Dim bAllVersionsGone As Boolean = True
        Dim aTypes As Type() = GetRegistrableTypesInAssembly(assembly)
        Dim NumTypes As Integer = aTypes.Length
        Dim strAsmVersion As String = assembly.GetType.GetMethod("GetVersion", BindingFlags.Public Or BindingFlags.NonPublic Or BindingFlags.Instance).Invoke(assembly, {}).ToString()

        For cTypes As Integer = 0 To NumTypes - 1
            CallUserDefinedRegistrationMethod(aTypes(cTypes), False, target)

            If IsRegisteredAsValueType(aTypes(cTypes)) Then
                If Not UnregisterValueType(aTypes(cTypes), strAsmVersion, target) Then
                    bAllVersionsGone = False
                End If
            ElseIf TypeRepresentsComType(aTypes(cTypes)) Then
                If Not UnregisterComImportedType(aTypes(cTypes), strAsmVersion, target) Then
                    bAllVersionsGone = False
                End If
            Else
                If Not UnregisterManagedType(aTypes(cTypes), strAsmVersion, target) Then
                    bAllVersionsGone = False
                End If
            End If
        Next

        Dim aPIAAttrs As Object() = assembly.GetCustomAttributes(GetType(PrimaryInteropAssemblyAttribute), False)
        Dim NumPIAAttrs As Integer = aPIAAttrs.Length

        If bAllVersionsGone Then

            For cPIAAttrs As Integer = 0 To NumPIAAttrs - 1
                UnregisterPrimaryInteropAssembly(assembly, CType(aPIAAttrs(cPIAAttrs), PrimaryInteropAssemblyAttribute), target)
            Next
        End If

        If aTypes.Length > 0 OrElse NumPIAAttrs > 0 Then
            Return True
        Else
            Return False
        End If
    End Function

    <System.Security.SecurityCritical>
    Friend Overridable Function UnregisterAssembly(ByVal assembly As Assembly) As Boolean Implements IRegistrationServices.UnregisterAssembly
        Return UnregisterAssembly(assembly, RegistryTarget.Machine)
    End Function

    <System.Security.SecurityCritical>
    Friend Overridable Function GetRegistrableTypesInAssembly(ByVal assembly As Assembly) As Type() Implements IRegistrationServices.GetRegistrableTypesInAssembly
        If assembly Is Nothing Then
            Throw New ArgumentNullException("assembly")
        End If
        Contract.EndContractBlock()
        If assembly.GetType.ToString <> "System.Reflection.RuntimeAssembly" Then
            Throw New ArgumentException("Argument_MustBeRuntimeAssembly", "assembly")
        End If
        Dim aTypes As Type() = assembly.GetExportedTypes()
        Dim NumTypes As Integer = aTypes.Length
        Dim TypeList As ArrayList = New ArrayList()

        For cTypes As Integer = 0 To NumTypes - 1
            Dim CurrentType As Type = aTypes(cTypes)
            If TypeRequiresRegistration(CurrentType) Then
                TypeList.Add(CurrentType)
            End If
        Next

        Dim RetArray As Type() = New Type(TypeList.Count - 1) {}
        TypeList.CopyTo(RetArray)
        Return RetArray
    End Function

    <System.Security.SecurityCritical>
    Friend Overridable Function GetProgIdForType(ByVal type As Type) As String Implements IRegistrationServices.GetProgIdForType
        Return Marshal.GenerateProgIdForType(type)
    End Function

    <System.Security.SecurityCritical>
    Friend Overridable Sub RegisterTypeForComClients(ByVal type As Type, ByRef g As Guid) Implements IRegistrationServices.RegisterTypeForComClients
        Throw New NotImplementedException("CoreCLR_REMOVED -- managed activation removed")
    End Sub

    Friend Overridable Function GetManagedCategoryGuid() As Guid Implements IRegistrationServices.GetManagedCategoryGuid
        Return s_ManagedCategoryGuid
    End Function

    <System.Security.SecurityCritical>
    Friend Overridable Function TypeRequiresRegistration(ByVal type As Type) As Boolean Implements IRegistrationServices.TypeRequiresRegistration
        Return TypeRequiresRegistrationHelper(type)
    End Function

    <System.Security.SecuritySafeCritical>
    Friend Overridable Function TypeRepresentsComType(ByVal type As Type) As Boolean Implements IRegistrationServices.TypeRepresentsComType
        If Not type.IsCOMObject Then Return False
        If type.IsImport Then Return True
        Dim baseComImportType As Type = GetBaseComImportType(type)
        Contract.Assert(baseComImportType IsNot Nothing, "baseComImportType != null")
        If Marshal.GenerateGuidForType(type) = Marshal.GenerateGuidForType(baseComImportType) Then Return True
        Return False
    End Function

    <System.Security.SecurityCritical>
    <ComVisible(False)>
    Friend Overridable Function RegisterTypeForComClients(ByVal type As Type, ByVal classContext As RegistrationClassContext, ByVal flags As RegistrationConnectionType) As Integer
        Throw New NotImplementedException("CoreCLR_REMOVED -- managed activation removed")
    End Function

    <System.Security.SecurityCritical>
    <ComVisible(False)>
    Friend Overridable Sub UnregisterTypeForComClients(ByVal cookie As Integer)
        CoRevokeClassObject(cookie)
    End Sub

    <System.Security.SecurityCritical>
    Friend Shared Function TypeRequiresRegistrationHelper(ByVal type As Type) As Boolean
        If Not type.IsClass AndAlso Not type.IsValueType Then Return False
        If type.IsAbstract Then Return False
        If Not type.IsValueType AndAlso type.GetConstructor(BindingFlags.Instance Or BindingFlags.Public, Nothing, New Type(-1) {}, Nothing) Is Nothing Then
            Return False
        End If
        Return Marshal.IsTypeVisibleFromCom(type)
    End Function

    <System.Security.SecurityCritical>
    <ResourceExposure(ResourceScope.Machine)>
    <ResourceConsumption(ResourceScope.Machine)>
    Private Sub RegisterValueType(ByVal type As Type, ByVal strAsmName As String, ByVal strAsmVersion As String, ByVal strAsmCodeBase As String, ByVal strRuntimeVersion As String, target As RegistryTarget)
        Dim strRecordId As String = "{" & Marshal.GenerateGuidForType(type).ToString().ToUpper(CultureInfo.InvariantCulture) & "}"
        Dim classesRoot As RegistryKey = GetClassesRoot(target)
        Using RecordRootKey As RegistryKey = classesRoot.CreateSubKey(strRecordRootName)
            Using RecordKey As RegistryKey = RecordRootKey.CreateSubKey(strRecordId)
                Using RecordVersionKey As RegistryKey = RecordKey.CreateSubKey(strAsmVersion)
                    RecordVersionKey.SetValue("Class", type.FullName)
                    RecordVersionKey.SetValue("Assembly", strAsmName)
                    RecordVersionKey.SetValue("RuntimeVersion", strRuntimeVersion)
                    If strAsmCodeBase IsNot Nothing Then
                        RecordVersionKey.SetValue("CodeBase", strAsmCodeBase)
                    End If
                End Using
            End Using
        End Using
    End Sub

    Private Shared Function GetClassesRoot(target As RegistryTarget) As RegistryKey
        Return GetTargetKey(target).OpenSubKey("Software", True).OpenSubKey("Classes", True)
    End Function

    Friend Shared Function GetTargetKey(target As RegistryTarget) As RegistryKey
        Dim targetKey As RegistryKey = Nothing
        Select Case target
            Case RegistryTarget.Machine
                Return Registry.LocalMachine
            Case RegistryTarget.User
                Return Registry.CurrentUser
            Case Else
                Throw New NotImplementedException($"Target {target} not implemented!")
        End Select
    End Function

    <System.Security.SecurityCritical>
    <ResourceExposure(ResourceScope.Machine)>
    <ResourceConsumption(ResourceScope.Machine)>
    Private Sub RegisterManagedType(target As RegistryTarget, ByVal type As Type, ByVal strAsmName As String, ByVal strAsmVersion As String, ByVal strAsmCodeBase As String, ByVal strRuntimeVersion As String)
        Dim strDocString As String = strDocStringPrefix & type.FullName
        Dim strClsId As String = "{" & Marshal.GenerateGuidForType(type).ToString().ToUpper(CultureInfo.InvariantCulture) & "}"
        Dim strProgId As String = GetProgIdForType(type)
        Dim classesRoot As RegistryKey = GetClassesRoot(target)
        If strProgId <> String.Empty Then
            Using TypeNameKey As RegistryKey = classesRoot.CreateSubKey(strProgId)
                TypeNameKey.SetValue("", strDocString)

                Using ProgIdClsIdKey As RegistryKey = TypeNameKey.CreateSubKey("CLSID", True)
                    ProgIdClsIdKey.SetValue("", strClsId)
                End Using
            End Using
        End If

        Using ClsIdRootKey As RegistryKey = classesRoot.CreateSubKey(strClsIdRootName, True)
            Using ClsIdKey As RegistryKey = ClsIdRootKey.CreateSubKey(strClsId, True)
                ClsIdKey.SetValue("", strDocString)
                Using InProcServerKey As RegistryKey = ClsIdKey.CreateSubKey("InprocServer32", True)
                    InProcServerKey.SetValue("", strMsCorEEFileName)
                    InProcServerKey.SetValue("ThreadingModel", strManagedTypeThreadingModel)
                    InProcServerKey.SetValue("Class", type.FullName)
                    InProcServerKey.SetValue("Assembly", strAsmName)
                    InProcServerKey.SetValue("RuntimeVersion", strRuntimeVersion)
                    If strAsmCodeBase IsNot Nothing Then
                        InProcServerKey.SetValue("CodeBase", strAsmCodeBase)
                    End If

                    Using VersionSubKey As RegistryKey = InProcServerKey.CreateSubKey(strAsmVersion, True)
                        VersionSubKey.SetValue("Class", type.FullName)
                        VersionSubKey.SetValue("Assembly", strAsmName)
                        VersionSubKey.SetValue("RuntimeVersion", strRuntimeVersion)
                        If strAsmCodeBase IsNot Nothing Then
                            VersionSubKey.SetValue("CodeBase", strAsmCodeBase)
                        End If
                    End Using

                    If strProgId <> String.Empty Then
                        Using ProgIdKey As RegistryKey = ClsIdKey.CreateSubKey("ProgId", True)
                            ProgIdKey.SetValue("", strProgId)
                        End Using
                    End If
                End Using

                Using CategoryKey As RegistryKey = ClsIdKey.CreateSubKey(strImplementedCategoriesSubKey, True)
                    Using ManagedCategoryKey As RegistryKey = CategoryKey.CreateSubKey(strManagedCategoryGuid, True)
                    End Using
                End Using
            End Using
        End Using

        EnsureManagedCategoryExists(target)
    End Sub

    <System.Security.SecurityCritical>
    <ResourceExposure(ResourceScope.Machine)>
    <ResourceConsumption(ResourceScope.Machine)>
    Private Sub RegisterComImportedType(ByVal type As Type, ByVal strAsmName As String, ByVal strAsmVersion As String, ByVal strAsmCodeBase As String, ByVal strRuntimeVersion As String, target As RegistryTarget)
        Dim strClsId As String = "{" & Marshal.GenerateGuidForType(type).ToString().ToUpper(CultureInfo.InvariantCulture) & "}"
        Dim classesRoot As RegistryKey = GetClassesRoot(target)
        Using ClsIdRootKey As RegistryKey = classesRoot.CreateSubKey(strClsIdRootName)

            Using ClsIdKey As RegistryKey = ClsIdRootKey.CreateSubKey(strClsId)

                Using InProcServerKey As RegistryKey = ClsIdKey.CreateSubKey("InprocServer32")
                    InProcServerKey.SetValue("Class", type.FullName)
                    InProcServerKey.SetValue("Assembly", strAsmName)
                    InProcServerKey.SetValue("RuntimeVersion", strRuntimeVersion)
                    If strAsmCodeBase IsNot Nothing Then
                        InProcServerKey.SetValue("CodeBase", strAsmCodeBase)
                    End If

                    Using VersionSubKey As RegistryKey = InProcServerKey.CreateSubKey(strAsmVersion)
                        VersionSubKey.SetValue("Class", type.FullName)
                        VersionSubKey.SetValue("Assembly", strAsmName)
                        VersionSubKey.SetValue("RuntimeVersion", strRuntimeVersion)
                        If strAsmCodeBase IsNot Nothing Then
                            VersionSubKey.SetValue("CodeBase", strAsmCodeBase)
                        End If
                    End Using
                End Using
            End Using
        End Using
    End Sub

    <System.Security.SecurityCritical>
    <ResourceExposure(ResourceScope.None)>
    <ResourceConsumption(ResourceScope.Machine, ResourceScope.Machine)>
    Private Function UnregisterValueType(ByVal type As Type, ByVal strAsmVersion As String, target As RegistryTarget) As Boolean
        Dim bAllVersionsGone As Boolean = True
        Dim strRecordId As String = "{" & Marshal.GenerateGuidForType(type).ToString().ToUpper(CultureInfo.InvariantCulture) & "}"
        Dim classesRoot As RegistryKey = GetClassesRoot(target)
        Using RecordRootKey As RegistryKey = classesRoot.OpenSubKey(strRecordRootName, True)

            If RecordRootKey IsNot Nothing Then

                Using RecordKey As RegistryKey = RecordRootKey.OpenSubKey(strRecordId, True)

                    If RecordKey IsNot Nothing Then

                        Using VersionSubKey As RegistryKey = RecordKey.OpenSubKey(strAsmVersion, True)

                            If VersionSubKey IsNot Nothing Then
                                VersionSubKey.DeleteValue("Assembly", False)
                                VersionSubKey.DeleteValue("Class", False)
                                VersionSubKey.DeleteValue("CodeBase", False)
                                VersionSubKey.DeleteValue("RuntimeVersion", False)
                                If (VersionSubKey.SubKeyCount = 0) AndAlso (VersionSubKey.ValueCount = 0) Then RecordKey.DeleteSubKey(strAsmVersion)
                            End If
                        End Using

                        If RecordKey.SubKeyCount <> 0 Then bAllVersionsGone = False
                        If (RecordKey.SubKeyCount = 0) AndAlso (RecordKey.ValueCount = 0) Then RecordRootKey.DeleteSubKey(strRecordId)
                    End If
                End Using

                If (RecordRootKey.SubKeyCount = 0) AndAlso (RecordRootKey.ValueCount = 0) Then classesRoot.DeleteSubKey(strRecordRootName)
            End If
        End Using

        Return bAllVersionsGone
    End Function

    <System.Security.SecurityCritical>
    <ResourceExposure(ResourceScope.None)>
    <ResourceConsumption(ResourceScope.Machine, ResourceScope.Machine)>
    Private Function UnregisterManagedType(ByVal type As Type, ByVal strAsmVersion As String, target As RegistryTarget) As Boolean
        Dim bAllVersionsGone As Boolean = True
        Dim classesRoot As RegistryKey = GetClassesRoot(target)
        Dim strClsId As String = "{" & Marshal.GenerateGuidForType(type).ToString().ToUpper(CultureInfo.InvariantCulture) & "}"
        Dim strProgId As String = GetProgIdForType(type)

        Using ClsIdRootKey As RegistryKey = classesRoot.OpenSubKey(strClsIdRootName, True)

            If ClsIdRootKey IsNot Nothing Then

                Using ClsIdKey As RegistryKey = ClsIdRootKey.OpenSubKey(strClsId, True)

                    If ClsIdKey IsNot Nothing Then

                        Using InProcServerKey As RegistryKey = ClsIdKey.OpenSubKey("InprocServer32", True)

                            If InProcServerKey IsNot Nothing Then

                                Using VersionSubKey As RegistryKey = InProcServerKey.OpenSubKey(strAsmVersion, True)

                                    If VersionSubKey IsNot Nothing Then
                                        VersionSubKey.DeleteValue("Assembly", False)
                                        VersionSubKey.DeleteValue("Class", False)
                                        VersionSubKey.DeleteValue("RuntimeVersion", False)
                                        VersionSubKey.DeleteValue("CodeBase", False)
                                        If (VersionSubKey.SubKeyCount = 0) AndAlso (VersionSubKey.ValueCount = 0) Then InProcServerKey.DeleteSubKey(strAsmVersion)
                                    End If
                                End Using

                                If InProcServerKey.SubKeyCount <> 0 Then bAllVersionsGone = False

                                If bAllVersionsGone Then
                                    InProcServerKey.DeleteValue("", False)
                                    InProcServerKey.DeleteValue("ThreadingModel", False)
                                End If

                                InProcServerKey.DeleteValue("Assembly", False)
                                InProcServerKey.DeleteValue("Class", False)
                                InProcServerKey.DeleteValue("RuntimeVersion", False)
                                InProcServerKey.DeleteValue("CodeBase", False)
                                If (InProcServerKey.SubKeyCount = 0) AndAlso (InProcServerKey.ValueCount = 0) Then ClsIdKey.DeleteSubKey("InprocServer32")
                            End If
                        End Using

                        If bAllVersionsGone Then
                            ClsIdKey.DeleteValue("", False)

                            If strProgId <> String.Empty Then

                                Using ProgIdKey As RegistryKey = ClsIdKey.OpenSubKey("ProgId", True)

                                    If ProgIdKey IsNot Nothing Then
                                        ProgIdKey.DeleteValue("", False)
                                        If (ProgIdKey.SubKeyCount = 0) AndAlso (ProgIdKey.ValueCount = 0) Then ClsIdKey.DeleteSubKey("ProgId")
                                    End If
                                End Using
                            End If

                            Using CategoryKey As RegistryKey = ClsIdKey.OpenSubKey(strImplementedCategoriesSubKey, True)

                                If CategoryKey IsNot Nothing Then

                                    Using ManagedCategoryKey As RegistryKey = CategoryKey.OpenSubKey(strManagedCategoryGuid, True)

                                        If ManagedCategoryKey IsNot Nothing Then
                                            If (ManagedCategoryKey.SubKeyCount = 0) AndAlso (ManagedCategoryKey.ValueCount = 0) Then CategoryKey.DeleteSubKey(strManagedCategoryGuid)
                                        End If
                                    End Using

                                    If (CategoryKey.SubKeyCount = 0) AndAlso (CategoryKey.ValueCount = 0) Then ClsIdKey.DeleteSubKey(strImplementedCategoriesSubKey)
                                End If
                            End Using
                        End If

                        If (ClsIdKey.SubKeyCount = 0) AndAlso (ClsIdKey.ValueCount = 0) Then ClsIdRootKey.DeleteSubKey(strClsId)
                    End If
                End Using

                If (ClsIdRootKey.SubKeyCount = 0) AndAlso (ClsIdRootKey.ValueCount = 0) Then classesRoot.DeleteSubKey(strClsIdRootName)
            End If

            If bAllVersionsGone Then

                If strProgId <> String.Empty Then

                    Using TypeNameKey As RegistryKey = classesRoot.OpenSubKey(strProgId, True)

                        If TypeNameKey IsNot Nothing Then
                            TypeNameKey.DeleteValue("", False)

                            Using ProgIdClsIdKey As RegistryKey = TypeNameKey.OpenSubKey("CLSID", True)

                                If ProgIdClsIdKey IsNot Nothing Then
                                    ProgIdClsIdKey.DeleteValue("", False)
                                    If (ProgIdClsIdKey.SubKeyCount = 0) AndAlso (ProgIdClsIdKey.ValueCount = 0) Then TypeNameKey.DeleteSubKey("CLSID")
                                End If
                            End Using

                            If (TypeNameKey.SubKeyCount = 0) AndAlso (TypeNameKey.ValueCount = 0) Then classesRoot.DeleteSubKey(strProgId)
                        End If
                    End Using
                End If
            End If
        End Using

        Return bAllVersionsGone
    End Function

    <System.Security.SecurityCritical>
    <ResourceExposure(ResourceScope.None)>
    <ResourceConsumption(ResourceScope.Machine, ResourceScope.Machine)>
    Private Function UnregisterComImportedType(ByVal type As Type, ByVal strAsmVersion As String, target As RegistryTarget) As Boolean
        Dim bAllVersionsGone As Boolean = True
        Dim strClsId As String = "{" & Marshal.GenerateGuidForType(type).ToString().ToUpper(CultureInfo.InvariantCulture) & "}"
        Dim classesRoot As RegistryKey = GetClassesRoot(target)

        Using ClsIdRootKey As RegistryKey = classesRoot.OpenSubKey(strClsIdRootName, True)

            If ClsIdRootKey IsNot Nothing Then

                Using ClsIdKey As RegistryKey = ClsIdRootKey.OpenSubKey(strClsId, True)

                    If ClsIdKey IsNot Nothing Then

                        Using InProcServerKey As RegistryKey = ClsIdKey.OpenSubKey("InprocServer32", True)

                            If InProcServerKey IsNot Nothing Then
                                InProcServerKey.DeleteValue("Assembly", False)
                                InProcServerKey.DeleteValue("Class", False)
                                InProcServerKey.DeleteValue("RuntimeVersion", False)
                                InProcServerKey.DeleteValue("CodeBase", False)

                                Using VersionSubKey As RegistryKey = InProcServerKey.OpenSubKey(strAsmVersion, True)

                                    If VersionSubKey IsNot Nothing Then
                                        VersionSubKey.DeleteValue("Assembly", False)
                                        VersionSubKey.DeleteValue("Class", False)
                                        VersionSubKey.DeleteValue("RuntimeVersion", False)
                                        VersionSubKey.DeleteValue("CodeBase", False)
                                        If (VersionSubKey.SubKeyCount = 0) AndAlso (VersionSubKey.ValueCount = 0) Then InProcServerKey.DeleteSubKey(strAsmVersion)
                                    End If
                                End Using

                                If InProcServerKey.SubKeyCount <> 0 Then bAllVersionsGone = False
                                If (InProcServerKey.SubKeyCount = 0) AndAlso (InProcServerKey.ValueCount = 0) Then ClsIdKey.DeleteSubKey("InprocServer32")
                            End If
                        End Using

                        If (ClsIdKey.SubKeyCount = 0) AndAlso (ClsIdKey.ValueCount = 0) Then ClsIdRootKey.DeleteSubKey(strClsId)
                    End If
                End Using

                If (ClsIdRootKey.SubKeyCount = 0) AndAlso (ClsIdRootKey.ValueCount = 0) Then classesRoot.DeleteSubKey(strClsIdRootName)
            End If
        End Using

        Return bAllVersionsGone
    End Function

    <System.Security.SecurityCritical>
    <ResourceExposure(ResourceScope.Machine)>
    <ResourceConsumption(ResourceScope.Machine)>
    Private Sub RegisterPrimaryInteropAssembly(ByVal assembly As Assembly, ByVal strAsmCodeBase As String, ByVal attr As PrimaryInteropAssemblyAttribute, target As RegistryTarget)
        Dim FriendKeyObject As String = CStr(assembly.GetType.GetMethod("GetFriendKey", BindingFlags.NonPublic Or BindingFlags.Public Or BindingFlags.Instance).Invoke(assembly, {}))

        If FriendKeyObject.Length = 0 Then
            Throw New InvalidOperationException("InvalidOperation_PIAMustBeStrongNamed")
        End If

        Dim strTlbId As String = "{" & Marshal.GetTypeLibGuidForAssembly(assembly).ToString().ToUpper(CultureInfo.InvariantCulture) & "}"
        Dim strVersion As String = attr.MajorVersion.ToString("x", CultureInfo.InvariantCulture) & "." + attr.MinorVersion.ToString("x", CultureInfo.InvariantCulture)
        Dim classesRoot As RegistryKey = GetClassesRoot(target)
        Using TypeLibRootKey As RegistryKey = classesRoot.CreateSubKey(strTlbRootName)
            Using TypeLibKey As RegistryKey = TypeLibRootKey.CreateSubKey(strTlbId)
                Using VersionSubKey As RegistryKey = TypeLibKey.CreateSubKey(strVersion)
                    VersionSubKey.SetValue("PrimaryInteropAssemblyName", assembly.FullName)
                    If strAsmCodeBase IsNot Nothing Then
                        VersionSubKey.SetValue("PrimaryInteropAssemblyCodeBase", strAsmCodeBase)
                    End If
                End Using
            End Using
        End Using
    End Sub

    <System.Security.SecurityCritical>
    <ResourceExposure(ResourceScope.None)>
    <ResourceConsumption(ResourceScope.Machine, ResourceScope.Machine)>
    Private Sub UnregisterPrimaryInteropAssembly(ByVal assembly As Assembly, ByVal attr As PrimaryInteropAssemblyAttribute, target As RegistryTarget)
        Dim strTlbId As String = "{" & Marshal.GetTypeLibGuidForAssembly(assembly).ToString().ToUpper(CultureInfo.InvariantCulture) & "}"
        Dim strVersion As String = attr.MajorVersion.ToString("x", CultureInfo.InvariantCulture) & "." + attr.MinorVersion.ToString("x", CultureInfo.InvariantCulture)
        Dim classesRoot As RegistryKey = GetClassesRoot(target)
        Using TypeLibRootKey As RegistryKey = classesRoot.OpenSubKey(strTlbRootName, True)
            If TypeLibRootKey IsNot Nothing Then
                Using TypeLibKey As RegistryKey = TypeLibRootKey.OpenSubKey(strTlbId, True)
                    If TypeLibKey IsNot Nothing Then
                        Using VersionSubKey As RegistryKey = TypeLibKey.OpenSubKey(strVersion, True)

                            If VersionSubKey IsNot Nothing Then
                                VersionSubKey.DeleteValue("PrimaryInteropAssemblyName", False)
                                VersionSubKey.DeleteValue("PrimaryInteropAssemblyCodeBase", False)
                                If (VersionSubKey.SubKeyCount = 0) AndAlso (VersionSubKey.ValueCount = 0) Then
                                    TypeLibKey.DeleteSubKey(strVersion)
                                End If
                            End If
                        End Using
                        If (TypeLibKey.SubKeyCount = 0) AndAlso (TypeLibKey.ValueCount = 0) Then
                            TypeLibRootKey.DeleteSubKey(strTlbId)
                        End If
                    End If
                End Using

                If (TypeLibRootKey.SubKeyCount = 0) AndAlso (TypeLibRootKey.ValueCount = 0) Then
                    classesRoot.DeleteSubKey(strTlbRootName)
                End If
            End If
        End Using
    End Sub

    <ResourceExposure(ResourceScope.None)>
    <ResourceConsumption(ResourceScope.Machine, ResourceScope.Machine)>
    Private Sub EnsureManagedCategoryExists(target As RegistryTarget)
        If Not ManagedCategoryExists(target) Then

            Using ComponentCategoryKey As RegistryKey = GetClassesRoot(target).CreateSubKey(strComponentCategorySubKey)

                Using ManagedCategoryKey As RegistryKey = ComponentCategoryKey.CreateSubKey(strManagedCategoryGuid)
                    ManagedCategoryKey.SetValue("0", strManagedCategoryDescription)
                End Using
            End Using
        End If
    End Sub

    <ResourceExposure(ResourceScope.None)>
    <ResourceConsumption(ResourceScope.Machine, ResourceScope.Machine)>
    Private Shared Function ManagedCategoryExists(target As RegistryTarget) As Boolean
        Using componentCategoryKey As RegistryKey = GetClassesRoot(target).OpenSubKey(strComponentCategorySubKey, False)
            If componentCategoryKey Is Nothing Then Return False

            Using managedCategoryKey As RegistryKey = componentCategoryKey.OpenSubKey(strManagedCategoryGuid, False)
                If managedCategoryKey Is Nothing Then Return False
                Dim value As Object = managedCategoryKey.GetValue("0")
                If value Is Nothing OrElse value.[GetType]() <> GetType(String) Then Return False
                Dim stringValue As String = CStr(value)
                If stringValue <> strManagedCategoryDescription Then Return False
            End Using
        End Using

        Return True
    End Function

    <System.Security.SecurityCritical>
    Private Sub CallUserDefinedRegistrationMethod(ByVal type As Type, ByVal bRegister As Boolean, target As RegistryTarget)
        Dim regFuncAttrType As Type
        If bRegister Then
            regFuncAttrType = GetType(ComRegisterFunctionAttribute)
        Else
            regFuncAttrType = GetType(ComUnregisterFunctionAttribute)
        End If

        If Not CallUserDefinedRegistrationMethodCore(type, bRegister, regFuncAttrType, target, True) Then
            CallUserDefinedRegistrationMethodCore(type, bRegister, regFuncAttrType, target, False)
        End If
    End Sub

    Friend Shared Function GetMethodsRecursive(type As Type, flags As BindingFlags) As List(Of MethodInfo)
        Dim lst As New List(Of MethodInfo)
        lst.AddRange(type.GetMethods(flags))
        If type.BaseType IsNot Nothing Then
            lst.AddRange(GetMethodsRecursive(type.BaseType, flags))
        End If
        Return lst
    End Function

    Friend Shared Function GetMethodWithAttribute(type As Type, attrbType As Type) As MethodInfo
        Dim aMethods As MethodInfo() = GetMethodsRecursive(type, BindingFlags.Instance Or BindingFlags.Public Or BindingFlags.NonPublic Or BindingFlags.[Static]).ToArray
        For Each m As MethodInfo In aMethods
            Dim a As Object = m.GetCustomAttribute(attrbType, True)
            If a IsNot Nothing Then
                Return m
            End If
        Next
        Return Nothing
    End Function

    <System.Security.SecurityCritical>
    Private Function CallUserDefinedRegistrationMethodCore(ByVal type As Type, ByVal bRegister As Boolean, regFuncAttrType As Type, target As RegistryTarget, searchForExtendedParameters As Boolean) As Boolean
        Dim bFunctionCalled As Boolean = False
        Dim currType As Type = type
        Dim com_m As MethodInfo = GetMethodWithAttribute(type, regFuncAttrType)

        If com_m IsNot Nothing Then
            While Not bFunctionCalled AndAlso currType IsNot Nothing
                Dim aMethods As MethodInfo() = GetMethodsRecursive(currType, BindingFlags.Instance Or BindingFlags.Public Or BindingFlags.NonPublic Or BindingFlags.[Static]).ToArray
                Dim NumMethods As Integer = aMethods.Length

                For cMethods As Integer = 0 To NumMethods - 1
                    Dim CurrentMethod As MethodInfo = aMethods(cMethods)
                    If CurrentMethod.Name = com_m.Name Then
                        If Not CurrentMethod.IsStatic Then

                            If bRegister Then
                                Throw New InvalidOperationException("InvalidOperation_NonStaticComRegFunction: " + CurrentMethod.Name + "|" + currType.Name)
                            Else
                                Throw New InvalidOperationException("InvalidOperation_NonStaticComUnRegFunction: " + CurrentMethod.Name + "|" + currType.Name)
                            End If
                        End If
                        Dim aParams As ParameterInfo() = CurrentMethod.GetParameters()
                        If Not searchForExtendedParameters Then
                            If (CurrentMethod.ReturnType IsNot GetType(Void) OrElse aParams Is Nothing OrElse aParams.Length > 1 OrElse (aParams(0).ParameterType IsNot GetType(String) AndAlso aParams(0).ParameterType IsNot GetType(Type))) Then
                                If (bRegister) Then
                                    Throw New InvalidOperationException("InvalidOperation_InvalidComRegFunctionSig: " + CurrentMethod.Name + "|" + currType.Name)
                                Else
                                    Throw New InvalidOperationException("InvalidOperation_InvalidComUnRegFunctionSig: " + CurrentMethod.Name + "|" + currType.Name)
                                End If
                            End If
                        ElseIf aParams.Length > 1 Then 'check the extended method only if it has more than 1 parameters
                            If (CurrentMethod.ReturnType IsNot GetType(Void) OrElse aParams Is Nothing OrElse aParams.Length > 2 OrElse (aParams(0).ParameterType IsNot GetType(String) AndAlso aParams(0).ParameterType IsNot GetType(Type))) OrElse aParams(1).ParameterType IsNot GetType(RegistryKey) Then
                                If (bRegister) Then
                                    Throw New InvalidOperationException("InvalidOperation_InvalidComRegFunctionExSig: " + CurrentMethod.Name + "|" + currType.Name)
                                Else
                                    Throw New InvalidOperationException("InvalidOperation_InvalidComUnRegFunctionExSig: " + CurrentMethod.Name + "|" + currType.Name)
                                End If
                            End If
                        Else
                            Continue For
                        End If

                        If bFunctionCalled Then

                            If bRegister Then
                                Throw New InvalidOperationException("InvalidOperation_MultipleComRegFunctions: " + currType.Name)
                            Else
                                Throw New InvalidOperationException("InvalidOperation_MultipleComUnRegFunctions: " + currType.Name)
                            End If
                        End If

                        Dim objs As New List(Of Object)

                        If aParams(0).ParameterType = GetType(String) Then
                            Dim tstr As String = GetClassesRoot(target).ToString.TrimEnd("\"c)
                            objs.Add($"{tstr}\CLSID\{" & Marshal.GenerateGuidForType(type).ToString().ToUpper(CultureInfo.InvariantCulture) & "}")
                        Else
                            objs.Add(type)
                        End If

                        If searchForExtendedParameters Then
                            objs.Add(GetTargetKey(target))
                        End If

                        CurrentMethod.Invoke(Nothing, objs.ToArray)
                        bFunctionCalled = True
                    End If
                Next

                currType = currType.BaseType
            End While
        End If
        Return bFunctionCalled
    End Function

    Private Function GetBaseComImportType(ByVal type As Type) As Type
            While type IsNot Nothing AndAlso Not type.IsImport
                type = type.BaseType
            End While

            Return type
        End Function

        Private Function IsRegisteredAsValueType(ByVal type As Type) As Boolean
            If Not type.IsValueType Then
                Return False
            End If
            Return True
        End Function

        <DllImport("Ole32.dll", CharSet:=CharSet.Auto, PreserveSig:=False)>
        <ResourceExposure(ResourceScope.None)>
        Private Shared Sub CoRevokeClassObject(ByVal cookie As Integer)
        End Sub

End Class