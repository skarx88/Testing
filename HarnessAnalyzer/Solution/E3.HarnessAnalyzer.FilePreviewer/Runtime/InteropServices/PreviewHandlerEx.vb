Imports System
Imports System.Diagnostics
Imports System.IO
Imports System.Linq
Imports System.Reflection
Imports System.Runtime.InteropServices
Imports System.Runtime.InteropServices.ComTypes
Imports System.Runtime.InteropServices.Shell
Imports System.Windows
Imports Microsoft.Win32
Imports Zuken.E3.HarnessAnalyzer.FilePreviewer.Shell

Public MustInherit Class PreviewHandlerEx
    Implements ICustomQueryInterface, IPreviewHandler, IPreviewHandlerVisuals, IOleWindow, IObjectWithSite, IInitializeWithItem, IInitializeWithFile, IDisposable
    Implements IInitializeWithStream

    Private _isPreviewShowing As Boolean
    Private _parentHwnd As IntPtr
    Private _frame As IPreviewHandlerFrame
    Private disposedValue As Boolean

    Friend Sub New()

    End Sub

    Friend Function RegisterMe(target As RegistryTarget) As Boolean
        Dim reg As New RegistrationServicesEx
        Return reg.RegisterAssembly(Me.GetType.Assembly, AssemblyRegistrationFlags.SetCodeBase, target)
    End Function

    Friend Function UnRegisterMe(target As RegistryTarget) As Boolean
        Dim reg As New RegistrationServicesEx
        Return reg.UnregisterAssembly(Me.GetType.Assembly, target)
    End Function

    Friend ReadOnly Property IsPreviewShowing As Boolean
        Get
            Return _isPreviewShowing
        End Get
    End Property

    Friend Shared Function IsRegistered(name As String, target As RegistryTarget) As Boolean
        Dim root = RegistrationServicesEx.GetTargetKey(target)
        Dim previewHandlersKey As RegistryKey = root.OpenSubKey("Software").OpenSubKey("Microsoft").OpenSubKey("Windows").OpenSubKey("CurrentVersion").OpenSubKey("PreviewHandlers")
        For Each v_name As String In previewHandlersKey.GetValueNames
            Dim value As Object = previewHandlersKey.GetValue(v_name)
            If value IsNot Nothing AndAlso CStr(value) = name Then
                Return True
            End If
        Next
        Return False
    End Function

    Protected Overridable Sub Initialize()
    End Sub

    Protected Overridable Sub Uninitialize()
    End Sub

    Protected MustOverride ReadOnly Property Handle As IntPtr
    Protected MustOverride Sub UpdateBounds(ByVal bounds As NativeRect)
    Protected MustOverride Sub HandleInitializeException(ByVal caughtException As Exception)
    Protected MustOverride Sub SetFocus()
    Protected MustOverride Sub SetBackground(ByVal argb As Integer)
    Protected MustOverride Sub SetForeground(ByVal argb As Integer)
    Protected MustOverride Sub SetFont(ByVal font As LogFont)
    Protected MustOverride Sub SetParentHandle(ByVal handle As IntPtr)

    Private Sub SetWindow(ByVal hwnd As IntPtr, ByRef rect As NativeRect) Implements IPreviewHandler.SetWindow
        _parentHwnd = hwnd
        UpdateBounds(rect)
        SetParentHandle(_parentHwnd)
    End Sub

    Private Sub SetRect(ByRef rect As NativeRect) Implements IPreviewHandler.SetRect
        UpdateBounds(rect)
    End Sub

    Friend Sub DoPreview() Implements IPreviewHandler.DoPreview
        _isPreviewShowing = True

        Try
            Initialize()
        Catch exc As Exception
            HandleInitializeException(exc)
        End Try
    End Sub

    Private Sub Unload() Implements IPreviewHandler.Unload
        Uninitialize()
        _isPreviewShowing = False
    End Sub

    Private Sub IPreviewHandler_SetFocus() Implements IPreviewHandler.SetFocus
        Me.SetFocus()
    End Sub

    Private Sub QueryFocus(<Out> ByRef phwnd As IntPtr) Implements IPreviewHandler.QueryFocus
        phwnd = Native.GetFocus()
    End Sub

    Private Function TranslateAccelerator(ByRef pmsg As Message) As HResult Implements IPreviewHandler.TranslateAccelerator
        Return If(_frame IsNot Nothing, _frame.TranslateAccelerator(pmsg), HResult.[False])
    End Function

    Private Sub SetBackgroundColor(ByVal color As NativeColorRef) Implements IPreviewHandlerVisuals.SetBackgroundColor
        SetBackground(CInt(color.Dword))
    End Sub

    Private Sub IPreviewHandlerVisuals_SetTextColor(ByVal color As NativeColorRef) Implements IPreviewHandlerVisuals.SetTextColor
        SetForeground(CInt(color.Dword))
    End Sub

    Private Sub IPreviewHandlerVisuals_SetFont(ByRef plf As LogFont) Implements IPreviewHandlerVisuals.SetFont
        SetFont(plf)
    End Sub

    Private Sub GetWindow(<Out> ByRef phwnd As IntPtr) Implements IOleWindow.GetWindow
        phwnd = Handle
    End Sub

    Private Sub ContextSensitiveHelp(ByVal fEnterMode As Boolean) Implements IOleWindow.ContextSensitiveHelp
        Throw New NotImplementedException()
    End Sub

    Private Sub SetSite(ByVal pUnkSite As Object) Implements IObjectWithSite.SetSite
        _frame = TryCast(pUnkSite, IPreviewHandlerFrame)
    End Sub

    Private Sub GetSite(ByRef riid As Guid, <Out> ByRef ppvSite As Object) Implements IObjectWithSite.GetSite
        ppvSite = CObj(_frame)
    End Sub

#If NETFRAMEWORK Then
    Private Sub Initialize(ByVal stream As System.Runtime.InteropServices.ComTypes.IStream, ByVal fileMode As AccessModes) Implements IInitializeWithStream.Initialize
        Dim preview As IPreviewFromStream = TryCast(Me, IPreviewFromStream)

        If preview Is Nothing Then
            Throw New InvalidOperationException(String.Format(System.Globalization.CultureInfo.InvariantCulture, "PreviewHandlerUnsupportedInterfaceCalled", "IPreviewFromStream"))
        End If

        Dim ss As StorageStream = CType(Activator.CreateInstance(GetType(StorageStream), System.Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Instance, Nothing, New Object() {stream, fileMode <> AccessModes.ReadWrite}), StorageStream)
        Using ss
            preview.Load(ss)
        End Using
    End Sub
#End If

    Private Sub Initialize(ByVal shellItem As IShellItem, ByVal accessMode As AccessModes) Implements IInitializeWithItem.Initialize
        Dim preview As IPreviewFromShellObject = TryCast(Me, IPreviewFromShellObject)

        If preview Is Nothing Then
            Throw New InvalidOperationException(String.Format(System.Globalization.CultureInfo.InvariantCulture, "PreviewHandlerUnsupportedInterfaceCalled", "IPreviewFromShellObject"))
        End If

        preview.Load(shellItem)
    End Sub

    Private Sub Initialize(ByVal filePath As String, ByVal fileMode As AccessModes) Implements IInitializeWithFile.Initialize
        Dim preview As IPreviewFromFile = TryCast(Me, IPreviewFromFile)

        If preview Is Nothing Then
            Throw New InvalidOperationException(String.Format(System.Globalization.CultureInfo.InvariantCulture, "PreviewHandlerUnsupportedInterfaceCalled", "IPreviewFromFile"))
        End If

        preview.Load(New FileInfo(filePath))
    End Sub

    <ComRegisterFunction>
    Friend Shared Sub Register(ByVal registerType As Type)
        RegisterCore(registerType, Registry.LocalMachine)
    End Sub

    Friend Shared Sub Register(ByVal registerType As Type, target As RegistryKey)
        RegisterCore(registerType, target)
    End Sub

    Private Shared Sub RegisterCore(ByVal registerType As Type, target As RegistryKey)
        If registerType IsNot Nothing AndAlso registerType.IsSubclassOf(GetType(PreviewHandlerEx)) Then
            Dim attrs As Object() = CType(registerType.GetCustomAttributes(GetType(PreviewHandlerAttribute), True), Object())

            If attrs IsNot Nothing AndAlso attrs.Length = 1 Then
                Dim attr As PreviewHandlerAttribute = TryCast(attrs(0), PreviewHandlerAttribute)
                ThrowIfNotValid(registerType)
                RegisterPreviewHandler(registerType.GUID, attr, target)
            Else
                Throw New NotSupportedException("PreviewHandlerInvalidAttributes: " + registerType.Name)
            End If
        End If
    End Sub

    <ComUnregisterFunction>
    Friend Shared Sub Unregister(ByVal registerType As Type)
        UnregisterCore(registerType, Registry.LocalMachine)
    End Sub

    Friend Shared Sub Unregister(ByVal registerType As Type, target As RegistryKey)
        UnregisterCore(registerType, target)
    End Sub

    Private Shared Sub UnregisterCore(ByVal registerType As Type, target As RegistryKey)
        If registerType IsNot Nothing AndAlso registerType.IsSubclassOf(GetType(PreviewHandlerEx)) Then
            Dim attrs As Object() = CType(registerType.GetCustomAttributes(GetType(PreviewHandlerAttribute), True), Object())

            If attrs IsNot Nothing AndAlso attrs.Length = 1 Then
                Dim attr As PreviewHandlerAttribute = TryCast(attrs(0), PreviewHandlerAttribute)
                UnregisterPreviewHandler(registerType.GUID, attr, target)
            End If
        End If
    End Sub

    Private Shared Function GetClassesRoot(target As RegistryKey) As RegistryKey
        Return target.OpenSubKey("Software", True).OpenSubKey("Classes", True)
    End Function

    Private Shared Sub RegisterPreviewHandler(ByVal previewerGuid As Guid, ByVal attribute As PreviewHandlerAttribute)
        RegisterPreviewHandler(previewerGuid, attribute, Registry.LocalMachine)
    End Sub

    Private Shared Sub RegisterPreviewHandler(ByVal previewerGuid As Guid, ByVal attribute As PreviewHandlerAttribute, target As RegistryKey)
        Dim guid As String = previewerGuid.ToString("B")

        Using appIdsKey As RegistryKey = GetClassesRoot(target).OpenSubKey("AppID", True)
            Using appIdKey As RegistryKey = appIdsKey.CreateSubKey(attribute.AppId)
                appIdKey.SetValue("DllSurrogate", "%SystemRoot%\system32\prevhost.exe", RegistryValueKind.ExpandString)
            End Using
        End Using

        Using handlersKey As RegistryKey = target.OpenSubKey("SOFTWARE\Microsoft\Windows\CurrentVersion\PreviewHandlers", True)
            handlersKey.SetValue(guid, attribute.Name, RegistryValueKind.String)
        End Using

        Using clsidKey As RegistryKey = GetClassesRoot(target).OpenSubKey("CLSID")
            Using idKey As RegistryKey = clsidKey.OpenSubKey(guid, True)
                idKey.SetValue("DisplayName", attribute.Name, RegistryValueKind.String)
                idKey.SetValue("AppID", attribute.AppId, RegistryValueKind.String)
                idKey.SetValue("DisableLowILProcessIsolation", If(attribute.DisableLowILProcessIsolation, 1, 0), RegistryValueKind.DWord)

                Using inproc As RegistryKey = idKey.OpenSubKey("InprocServer32", True)
                    inproc.SetValue("ThreadingModel", "Apartment", RegistryValueKind.String)
                End Using
            End Using
        End Using

        For Each extension As String In attribute.Extensions.Split(New Char() {";"c}, StringSplitOptions.RemoveEmptyEntries)
            Trace.WriteLine("Registering extension '" & extension & "' with previewer '" & guid & "'")

            Using extensionKey As RegistryKey = GetClassesRoot(target).CreateSubKey(extension)
                Using shellexKey As RegistryKey = extensionKey.CreateSubKey("shellex")
                    Using previewKey As RegistryKey = shellexKey.CreateSubKey(HandlerNativeMethods.IPreviewHandlerGuid.ToString("B"))
                        previewKey.SetValue(Nothing, guid, RegistryValueKind.String)
                    End Using
                End Using
            End Using
        Next
    End Sub

    Private Shared Sub UnregisterPreviewHandler(ByVal previewerGuid As Guid, ByVal attribute As PreviewHandlerAttribute, target As RegistryKey)
        Dim guid As String = previewerGuid.ToString("B")

        For Each extension As String In attribute.Extensions.Split(New Char() {";"c}, StringSplitOptions.RemoveEmptyEntries)
            Trace.WriteLine("Unregistering extension '" & extension & "' with previewer '" & guid & "'")

            Using shellexKey As RegistryKey = GetClassesRoot(target).OpenSubKey(extension & "\shellex", True)
                If shellexKey IsNot Nothing Then
                    shellexKey.DeleteSubKey(HandlerNativeMethods.IPreviewHandlerGuid.ToString(), False)
                End If
            End Using
        Next

        Using appIdsKey As RegistryKey = GetClassesRoot(target).OpenSubKey("AppID", True)
            appIdsKey.DeleteSubKey(attribute.AppId, False)
        End Using

        Using classesKey As RegistryKey = target.OpenSubKey("SOFTWARE\Microsoft\Windows\CurrentVersion\PreviewHandlers", True)
            classesKey.DeleteValue(guid, False)
        End Using
    End Sub

    Private Shared Sub UnregisterPreviewHandler(ByVal previewerGuid As Guid, ByVal attribute As PreviewHandlerAttribute)
        UnregisterPreviewHandler(previewerGuid, attribute, Registry.LocalMachine)
    End Sub

    Private Shared Sub ThrowIfNotValid(ByVal type As Type)
        Dim interfaces = type.GetInterfaces()

        If Not interfaces.Any(Function(x) x = GetType(IPreviewFromStream) OrElse x = GetType(IPreviewFromShellObject) OrElse x = GetType(IPreviewFromFile)) Then
            Throw New NotImplementedException(String.Format(System.Globalization.CultureInfo.InvariantCulture, "PreviewHandlerInterfaceNotImplemented", type.Name))
        End If
    End Sub

    Private Function GetInterface(ByRef iid As Guid, <Out> ByRef ppv As IntPtr) As CustomQueryInterfaceResult Implements ICustomQueryInterface.GetInterface
        ppv = IntPtr.Zero

        If iid = HandlerNativeMethods.IMarshalGuid Then
            Return CustomQueryInterfaceResult.Failed
        End If

        If (iid = HandlerNativeMethods.IInitializeWithStreamGuid AndAlso Not (TypeOf Me Is IPreviewFromStream)) OrElse (iid = HandlerNativeMethods.IInitializeWithItemGuid AndAlso Not (TypeOf Me Is IPreviewFromShellObject)) OrElse (iid = HandlerNativeMethods.IInitializeWithFileGuid AndAlso Not (TypeOf Me Is IPreviewFromFile)) Then
            Return CustomQueryInterfaceResult.Failed
        End If

        Return CustomQueryInterfaceResult.NotHandled
    End Function

    Public Shared Function RegisterAllPreviewHandlers(Of T)(target As RegistryTarget) As Boolean
        Return RegisterAllPreviewHandlers(GetType(T).Assembly, target)
    End Function

    Shared Function RegisterAllPreviewHandlers(assembly As Reflection.Assembly, target As RegistryTarget) As Boolean
        For Each t As Type In assembly.GetTypes
            If GetType(PreviewHandlerEx).IsSubclassOf(t) Then
                Using inst As PreviewHandlerEx = CType(Activator.CreateInstance(t), PreviewHandlerEx)
                    If Not inst.RegisterMe(target) Then
                        Return False
                    End If
                End Using
            End If
        Next
        Return True
    End Function

    Shared Function UnRegisterAllPreviewHandlers(Of T)(target As RegistryTarget) As Boolean
        Return UnRegisterAllPreviewHandlers(GetType(T).Assembly, target)
    End Function

    Shared Function UnRegisterAllPreviewHandlers(assembly As Reflection.Assembly, target As RegistryTarget) As Boolean
        For Each t As Type In assembly.GetTypes
            If GetType(PreviewHandlerEx).IsSubclassOf(t) Then
                Using inst As PreviewHandlerEx = CType(Activator.CreateInstance(t), PreviewHandlerEx)
                    If Not inst.UnRegisterMe(target) Then
                        Return False
                    End If
                End Using
            End If
        Next
        Return True
    End Function

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                Me.Uninitialize()
            End If

            disposedValue = True
        End If
    End Sub

    Friend Sub Dispose() Implements IDisposable.Dispose
        ' Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in der Methode "Dispose(disposing As Boolean)" ein.
        Dispose(disposing:=True)
        GC.SuppressFinalize(Me)
    End Sub
End Class

