Public Enum RegistryTarget
    ''' <summary>
    ''' registers or unregisters a .NET COM object in HKEY_LOCAL_MACHINE, for all users, needs proper rights
    ''' </summary>
    Machine
    ''' <summary>
    ''' registers or unregisters a .NET COM object in HKEY_CURRENT_USER to avoid UAC prompts
    ''' </summary>
    User
End Enum

