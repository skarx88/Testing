# Registry Integration

The E3.Lib.Licensing library uses the Windows Registry as a persistence mechanism for storing and retrieving licensing configuration data. This integration provides a way to cache feature availability information and persist licensing state between application sessions.

### Overview

Registry integration in E3.Lib.Licensing serves several key purposes:
- **Feature Persistence**: Store available features discovered during license checks
- **Configuration Management**: Maintain product-specific licensing settings
- **Performance Optimization**: Cache feature availability to reduce repeated FlexNet queries
- **Cross-session State**: Preserve licensing information between application restarts

## Registry Key Structure

The registry key hierarchy follows a standardized pattern:

```
HKEY_CURRENT_USER\Software\Zuken\{ProductName}\
├── AvailableFeatures (REG_DWORD)
└── [Other product-specific values]
```

### Key Components

#### Base Path
- **Root**: `HKEY_CURRENT_USER` - User-specific licensing configuration
- **Software**: Standard software configuration location
- **Company**: `"Zuken"` - Company identifier constant
- **Product**: Dynamic product name from `ProductName` property

#### Registry Constants

```vb
Public Const REG_AVAIL_FEATURES_KEY As String = "AvailableFeatures"
Public Const COMPANY_NAME As String = "Zuken"
```

## Registry Key Enumeration

The `FeaturesRegistryKey` enumeration tracks which registry keys were successfully accessed:

```vb
Public Enum FeaturesRegistryKey
    None = 0
    CurrentUser = 1      ' HKEY_CURRENT_USER accessible
    Software = 2         ' Software key accessible
    Company = 4          ' Zuken key accessible
    Product = 8          ' Product-specific key accessible
    AvailableFeatures = 16  ' AvailableFeatures value exists
End Enum
```

### Flag Usage

The enumeration uses bitwise flags to represent multiple key states:

```vb
' Example: All keys accessible
Dim allKeys As FeaturesRegistryKey = 
    FeaturesRegistryKey.CurrentUser Or 
    FeaturesRegistryKey.Software Or 
    FeaturesRegistryKey.Company Or 
    FeaturesRegistryKey.Product Or 
    FeaturesRegistryKey.AvailableFeatures

' Check if product key is accessible
If registryKeys.HasFlag(FeaturesRegistryKey.Product) Then
    ' Product key exists and is writable
End If
```

## Feature Storage Mechanism

### Available Features Value

Features are stored as a single `REG_DWORD` value where each bit represents a specific feature:

```vb
' Example: Store multiple features as bitwise flags
Dim featureFlags As Integer = 0
For Each featureName As String In availableFeatures
    Dim featureValue As Object = Nothing
    If TryGetFeatureStringAsValue(featureName, featureValue) Then
        featureFlags = featureFlags Or CInt(featureValue)
    End If
Next
productKey.SetValue(REG_AVAIL_FEATURES_KEY, featureFlags)
```

### Feature Value Conversion

The conversion between feature names and registry values depends on the specific implementation:

```vb
' Abstract method implemented by derived classes
Protected MustOverride Function TryGetFeatureStringAsValue(
    featureString As String, 
    ByRef value As Object
) As Boolean
```

## Registry Access Patterns

### Key Creation and Access

The `GetProductKey` method implements a robust key access pattern:

```vb
Private Function GetProductKey(
    createNeededKeys As Boolean, 
    ByRef keysFound As FeaturesRegistryKey
) As RegistryKey
    
    keysFound = FeaturesRegistryKey.None
    
    ' Open Software key with write access
    Dim softKey As RegistryKey = Registry.CurrentUser.OpenSubKey("Software", True)
    If softKey Is Nothing AndAlso createNeededKeys Then
        softKey = Registry.CurrentUser.CreateSubKey("Software")
    End If
    
    If softKey IsNot Nothing Then
        keysFound = keysFound Or 
                   FeaturesRegistryKey.CurrentUser Or 
                   FeaturesRegistryKey.Software
        
        ' Open/Create Company key
        Dim compKey As RegistryKey = softKey.OpenSubKey(COMPANY_NAME, True)
        If compKey Is Nothing AndAlso createNeededKeys Then
            compKey = softKey.CreateSubKey(COMPANY_NAME)
        End If
        
        If compKey IsNot Nothing Then
            keysFound = keysFound Or FeaturesRegistryKey.Company
            
            ' Open/Create Product key
            Dim productRegKey As RegistryKey = compKey.OpenSubKey(ProductName, True)
            If productRegKey Is Nothing AndAlso createNeededKeys Then
                productRegKey = compKey.CreateSubKey(ProductName)
            End If
            
            Return productRegKey
        End If
    End If
    
    Return Nothing
End Function
```

### Feature Retrieval

```vb
Protected Function GetOrCreateAvailableRegistryFeatures(
    Optional createNeededKeys As Boolean = False,
    Optional availableFeaturesKeySetting As AvailableFeatureKeySettings = Nothing
) As FeaturesResult
    
    If availableFeaturesKeySetting Is Nothing Then
        availableFeaturesKeySetting = New AvailableFeatureKeySettings
    End If
    
    Dim keysFound As FeaturesRegistryKey = FeaturesRegistryKey.None
    Dim availFeatures As Object = Nothing
    Dim productRegKey As RegistryKey = GetProductKey(createNeededKeys, keysFound)
    
    If productRegKey IsNot Nothing Then
        keysFound = keysFound Or FeaturesRegistryKey.Product
        
        ' Create value if requested and doesn't exist
        If createNeededKeys AndAlso 
           Not productRegKey.GetValueNames.Contains(REG_AVAIL_FEATURES_KEY) AndAlso 
           availableFeaturesKeySetting.ValueCreateNew IsNot Nothing Then
            productRegKey.SetValue(REG_AVAIL_FEATURES_KEY, 
                                 availableFeaturesKeySetting.ValueCreateNew)
        End If
        
        ' Check if value exists
        If productRegKey.GetValueNames.Contains(REG_AVAIL_FEATURES_KEY) Then
            keysFound = keysFound Or FeaturesRegistryKey.AvailableFeatures
        End If
        
        ' Retrieve value with default fallback
        availFeatures = productRegKey.GetValue(REG_AVAIL_FEATURES_KEY, 
                                              availableFeaturesKeySetting.DefaultValue)
    End If
    
    Return New FeaturesResult(keysFound, availFeatures)
End Function
```

## Configuration Management

### AvailableFeatureKeySettings Class

This class provides configuration for registry value creation and defaults:

```vb
Protected Class AvailableFeatureKeySettings
    ''' <summary>
    ''' Value set when available features key is created newly
    ''' (key will only be created when value is set here)
    ''' </summary>
    Property ValueCreateNew As Object = Nothing
    
    ''' <summary>
    ''' Default return value when available features key is read but was not available
    ''' </summary>
    Property DefaultValue As Object = Nothing
    
    Public Sub New()
    End Sub
    
    Public Sub New(valueOnCreateNew As Object, defaultValue As Object)
        Me.ValueCreateNew = valueOnCreateNew
        Me.DefaultValue = defaultValue
    End Sub
End Class
```

### Usage Example

```vb
' Configure registry settings for feature initialization
Dim registrySettings As New AvailableFeatureKeySettings(
    GetAllFeaturesAsFlag(),  ' Value to set when creating new key
    0                        ' Default value when key doesn't exist
)

' Initialize features with registry integration
InitAvailableFeatures(createRegKeys:=True, registrySettings)
```

## Platform Considerations

### Cross-Platform Support

Registry access is conditionally compiled for Windows platforms:

```vb
#If WINDOWS Or NETFRAMEWORK Then
    ' Registry operations
    productRegKey.SetValue(REG_AVAIL_FEATURES_KEY, value)
#Else
    Debug.WriteLine("Registry operations not supported on this platform!")
#End If
```

### Supported Platforms
- **Windows**: Full registry support via `Microsoft.Win32.Registry`
- **Non-Windows**: Registry operations are skipped with debug warnings
- **.NET Framework**: Native registry support
- **.NET Core/.NET 5+**: Conditional support based on platform

## Error Handling and Diagnostics

### Registry Access Validation

Before attempting feature authentication, the system validates registry access:

```vb
If Not RegistryKeys.HasFlag(FeaturesRegistryKey.Product) AndAlso features.Length > 0 Then
    Return New AuthFeaturesResult(
        New [Error](ErrorCodes.FEATURE_REGISTRY_KEY_MISSING, 
                   "Registry key for product features could not be found or created"),
        LicensingErrorCodes.RegistryKeyMissing
    )
End If
```

### Common Registry Issues

1. **Permission Errors**: User lacks write access to `HKEY_CURRENT_USER`
2. **Key Creation Failures**: Unable to create company or product keys
3. **Value Access Errors**: Registry value corruption or type mismatches
4. **Platform Limitations**: Registry not available on non-Windows platforms

## Best Practices

### Registry Key Management

1. **Lazy Creation**: Only create keys when `createNeededKeys=True`
2. **Graceful Degradation**: Continue operation even if registry access fails
3. **Proper Disposal**: Ensure registry keys are properly closed
4. **Error Handling**: Catch and handle registry-specific exceptions

### Security Considerations

1. **User Scope**: Use `HKEY_CURRENT_USER` to avoid elevation requirements
2. **Read-Only Fallbacks**: Attempt read-only access if write access fails
3. **Validation**: Validate registry values before using them
4. **Minimal Storage**: Store only essential licensing information

### Performance Optimization

1. **Caching**: Cache registry values to minimize repeated access
2. **Batch Operations**: Group multiple registry operations together
3. **Conditional Access**: Check key existence before attempting operations
4. **Resource Management**: Dispose of registry keys promptly

## Integration Examples

### Custom Registry Configuration

```vb
Public Class CustomLicenseManager
    Inherits LicenseManagerBase
    
    Protected Overrides Sub InitAvailableFeatures(
        Optional createRegKeys As Boolean = False,
        Optional availableFeaturesKeySetting As AvailableFeatureKeySettings = Nothing
    )
        ' Configure custom registry settings
        If availableFeaturesKeySetting Is Nothing Then
            availableFeaturesKeySetting = New AvailableFeatureKeySettings(
                MyCustomFeatureFlag,  ' Custom default for new keys
                0                     ' Default when key missing
            )
        End If
        
        ' Call base implementation with custom settings
        MyBase.InitAvailableFeatures(createRegKeys, availableFeaturesKeySetting)
    End Sub
    
    Protected Overrides Function TryGetFeatureStringAsValue(
        featureString As String, 
        ByRef value As Object
    ) As Boolean
        ' Custom feature-to-value conversion logic
        Select Case featureString.ToUpper()
            Case "FEATURE_A"
                value = 1
                Return True
            Case "FEATURE_B"
                value = 2
                Return True
            Case "FEATURE_C"
                value = 4
                Return True
            Case Else
                Return False
        End Select
    End Function
End Class
```

### Registry Monitoring

```vb
Public Sub MonitorRegistryChanges()
    ' Check current registry state
    Dim currentKeys As FeaturesRegistryKey = Me.RegistryKeys
    
    ' Validate key accessibility
    If currentKeys.HasFlag(FeaturesRegistryKey.AvailableFeatures) Then
        Console.WriteLine("Available features registry value exists")
    Else
        Console.WriteLine("Available features registry value missing")
        
        ' Attempt to recreate if needed
        InitAvailableFeatures(createRegKeys:=True)
    End If
End Sub
```

## Related Components

### Dependencies
- **Microsoft.Win32.Registry**: Windows registry access
- **System.Collections.ObjectModel**: ReadOnlyCollection for feature lists
- **System.ComponentModel**: IDisposable implementation

### Related Classes
- **`FeaturesResult`**: Encapsulates registry query results
- **`LicenseManagerBase`**: Provides registry integration foundation
- **`LicenseManager(Of T)`**: Implements generic feature registry patterns

### Integration Points
- **Feature Authentication**: Registry state affects feature validation
- **Status Management**: Registry persistence for licensing state
- **Error Reporting**: Registry errors integrated into result system

---

**Previous:** [08 - FlexNet Integration](./Documentation-08-FlexNet-Integration.md) | **Next:** [10 - Error Handling and Diagnostics](./Documentation-10-Error-Handling-Diagnostics.md)
