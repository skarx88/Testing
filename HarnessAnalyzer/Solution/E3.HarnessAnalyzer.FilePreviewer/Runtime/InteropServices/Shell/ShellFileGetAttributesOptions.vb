Namespace Global.System.Runtime.InteropServices.Shell
    <Flags>
    Friend Enum ShellFileGetAttributesOptions
        ''' <summary>
        ''' The specified items can be copied.
        ''' </summary>
        CanCopy = &H1

        ''' <summary>
        ''' The specified items can be moved.
        ''' </summary>
        CanMove = &H2

        ''' <summary>
        ''' Shortcuts can be created for the specified items. This flag has the same value as DROPEFFECT. 
        ''' The normal use of this flag is to add a Create Shortcut item to the shortcut menu that is displayed 
        ''' during drag-and-drop operations. However, SFGAO_CANLINK also adds a Create Shortcut item to the Microsoft 
        ''' Windows Explorer's File menu and to normal shortcut menus. 
        ''' If this item is selected, your application's IContextMenu::InvokeCommand is invoked with the lpVerb 
        ''' member of the CMINVOKECOMMANDINFO structure set to "link." Your application is responsible for creating the link.
        ''' </summary>
        CanLink = &H4

        ''' <summary>
        ''' The specified items can be bound to an IStorage interface through IShellFolder::BindToObject.
        ''' </summary>
        Storage = &H8

        ''' <summary>
        ''' The specified items can be renamed.
        ''' </summary>
        CanRename = &H10

        ''' <summary>
        ''' The specified items can be deleted.
        ''' </summary>
        CanDelete = &H20

        ''' <summary>
        ''' The specified items have property sheets.
        ''' </summary>
        HasPropertySheet = &H40

        ''' <summary>
        ''' The specified items are drop targets.
        ''' </summary>
        DropTarget = &H100

        ''' <summary>
        ''' This flag is a mask for the capability flags.
        ''' </summary>
        CapabilityMask = &H177

        ''' <summary>
        ''' Windows 7 and later. The specified items are system items.
        ''' </summary>
        System = &H1000

        ''' <summary>
        ''' The specified items are encrypted.
        ''' </summary>
        Encrypted = &H2000

        ''' <summary>
        ''' Indicates that accessing the object = through IStream or other storage interfaces, 
        ''' is a slow operation. 
        ''' Applications should avoid accessing items flagged with SFGAO_ISSLOW.
        ''' </summary>
        IsSlow = &H4000

        ''' <summary>
        ''' The specified items are ghosted icons.
        ''' </summary>
        Ghosted = &H8000

        ''' <summary>
        ''' The specified items are shortcuts.
        ''' </summary>
        Link = &H10000

        ''' <summary>
        ''' The specified folder objects are shared.
        ''' </summary>    
        Share = &H20000

        ''' <summary>
        ''' The specified items are read-only. In the case of folders, this means 
        ''' that new items cannot be created in those folders.
        ''' </summary>
        [ReadOnly] = &H40000

        ''' <summary>
        ''' The item is hidden and should not be displayed unless the 
        ''' Show hidden files and folders option is enabled in Folder Settings.
        ''' </summary>
        Hidden = &H80000

        ''' <summary>
        ''' This flag is a mask for the display attributes.
        ''' </summary>
        DisplayAttributeMask = &HFC000

        ''' <summary>
        ''' The specified folders contain one or more file system folders.
        ''' </summary>
        FileSystemAncestor = &H10000000

        ''' <summary>
        ''' The specified items are folders.
        ''' </summary>
        Folder = &H20000000

        ''' <summary>
        ''' The specified folders or file objects are part of the file system 
        ''' that is, they are files, directories, or root directories).
        ''' </summary>
        FileSystem = &H40000000

        ''' <summary>
        ''' The specified folders have subfolders = and are, therefore, 
        ''' expandable in the left pane of Windows Explorer).
        ''' </summary>
        HasSubFolder = &H80000000

        ''' <summary>
        ''' This flag is a mask for the contents attributes.
        ''' </summary>
        ContentsMask = &H80000000

        ''' <summary>
        ''' When specified as input, SFGAO_VALIDATE instructs the folder to validate that the items 
        ''' pointed to by the contents of apidl exist. If one or more of those items do not exist, 
        ''' IShellFolder::GetAttributesOf returns a failure code. 
        ''' When used with the file system folder, SFGAO_VALIDATE instructs the folder to discard cached 
        ''' properties retrieved by clients of IShellFolder2::GetDetailsEx that may 
        ''' have accumulated for the specified items.
        ''' </summary>
        Validate = &H1000000

        ''' <summary>
        ''' The specified items are on removable media or are themselves removable devices.
        ''' </summary>
        Removable = &H2000000

        ''' <summary>
        ''' The specified items are compressed.
        ''' </summary>
        Compressed = &H4000000

        ''' <summary>
        ''' The specified items can be browsed in place.
        ''' </summary>
        Browsable = &H8000000

        ''' <summary>
        ''' The items are nonenumerated items.
        ''' </summary>
        Nonenumerated = &H100000

        ''' <summary>
        ''' The objects contain new content.
        ''' </summary>
        NewContent = &H200000

        ''' <summary>
        ''' It is possible to create monikers for the specified file objects or folders.
        ''' </summary>
        CanMoniker = &H400000

        ''' <summary>
        ''' Not supported.
        ''' </summary>
        HasStorage = &H400000

        ''' <summary>
        ''' Indicates that the item has a stream associated with it that can be accessed 
        ''' by a call to IShellFolder::BindToObject with IID_IStream in the riid parameter.
        ''' </summary>
        Stream = &H400000

        ''' <summary>
        ''' Children of this item are accessible through IStream or IStorage. 
        ''' Those children are flagged with SFGAO_STORAGE or SFGAO_STREAM.
        ''' </summary>
        StorageAncestor = &H800000

        ''' <summary>
        ''' This flag is a mask for the storage capability attributes.
        ''' </summary>
        StorageCapabilityMask = &H70C50008

        ''' <summary>
        ''' Mask used by PKEY_SFGAOFlags to remove certain values that are considered 
        ''' to cause slow calculations or lack context. 
        ''' Equal to SFGAO_VALIDATE | SFGAO_ISSLOW | SFGAO_HASSUBFOLDER.
        ''' </summary>
        PkeyMask = &H81044000
    End Enum
End Namespace