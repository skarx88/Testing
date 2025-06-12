Namespace Global.System.Runtime.InteropServices
    ''' <summary>
    ''' This interface exposes the <see cref="Load"/> function for initializing the 
    ''' Preview Handler with a <type paramrefname="Stream"/>.
    ''' This interface can be used in conjunction with the other intialization interfaces,
    ''' but only 1 will be accessed according to the priorities preset by the Windows Shell:
    ''' <type paramrefname="IPreviewFromStream"/>
    ''' <type paramrefname="IPreviewFromShellObject"/>
    ''' <type paramrefname="IPreviewFromFile"/>
    ''' </summary>
    Friend Interface IPreviewFromStream
        ''' <summary>
        ''' Provides the <typepararef name="stream"/> to the item from which a preview should be created.        
        ''' </summary>
        ''' <param name="stream">Stream to the previewed file, this stream is only available in the scope of this method.</param>
        Sub Load(stream As IO.Stream)
    End Interface

End Namespace
