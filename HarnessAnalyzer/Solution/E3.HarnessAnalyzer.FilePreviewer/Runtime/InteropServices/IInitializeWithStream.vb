Imports Zuken.E3.HarnessAnalyzer.FilePreviewer.Shell

Namespace Global.System.Runtime.InteropServices

    ''' <summary>
    ''' Provides means by which to initialize with a stream.
    ''' </summary>
    <Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix")>
    <ComImport>
    <Guid("b824b49d-22ac-4161-ac8a-9916e8fa3f7f")>
    <InterfaceType(ComInterfaceType.InterfaceIsIUnknown)>
    Friend Interface IInitializeWithStream
        ''' <summary>
        ''' Initializes with a stream.
        ''' </summary>
        ''' <param name="stream"></param>
        ''' <param name="fileMode"></param>
        Sub Initialize(stream As ComTypes.IStream, fileMode As AccessModes)
    End Interface

End Namespace
