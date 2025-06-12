
''' <summary>
''' HRESULT Wrapper    
''' </summary>    
Public Enum HResult
    ''' <summary>     
    ''' S_OK          
    ''' </summary>    
    Ok = &H0

    ''' <summary>
    ''' S_FALSE
    ''' </summary>        
    [False] = &H1

    ''' <summary>
    ''' E_INVALIDARG
    ''' </summary>
    InvalidArguments = &H80070057

    ''' <summary>
    ''' E_OUTOFMEMORY
    ''' </summary>
    OutOfMemory = &H8007000E

    ''' <summary>
    ''' E_NOINTERFACE
    ''' </summary>
    NoInterface = &H80004002

    ''' <summary>
    ''' E_FAIL
    ''' </summary>
    Fail = &H80004005

    ''' <summary>
    ''' E_ELEMENTNOTFOUND
    ''' </summary>
    ElementNotFound = &H80070490

    ''' <summary>
    ''' TYPE_E_ELEMENTNOTFOUND
    ''' </summary>
    TypeElementNotFound = &H8002802B

    ''' <summary>
    ''' NO_OBJECT
    ''' </summary>
    NoObject = &H800401E5

    ''' <summary>
    ''' Win32 Error code: ERROR_CANCELLED
    ''' </summary>
    Win32ErrorCanceled = 1223

    ''' <summary>
    ''' ERROR_CANCELLED
    ''' </summary>
    Canceled = &H800704C7

    ''' <summary>
    ''' The requested resource is in use
    ''' </summary>
    ResourceInUse = &H800700AA

    ''' <summary>
    ''' The requested resources is read-only.
    ''' </summary>
    AccessDenied = &H80030005
End Enum
