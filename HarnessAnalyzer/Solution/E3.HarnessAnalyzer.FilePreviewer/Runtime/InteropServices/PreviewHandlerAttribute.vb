Namespace Global.System.Runtime.InteropServices

    ''' <summary>    
    ''' This class attribute is applied to a Preview Handler to specify registration parameters.
    ''' </summary>
    <AttributeUsage(AttributeTargets.Class, AllowMultiple:=False, Inherited:=False)>
    Public NotInheritable Class PreviewHandlerAttribute
        Inherits Attribute

        Private _Name As String, _Extensions As String, _AppId As String

        ''' <summary>
        ''' Creates a new instance of the attribute.
        ''' </summary>
        ''' <param name="name">Name of the Handler</param>
        ''' <param name="extensions">Semi-colon-separated list of file extensions supported by the handler.</param>
        ''' <param name="appId">A unique guid used for process isolation.</param>
        Public Sub New(name As String, extensions As String, appId As String)
            If Equals(name, Nothing) Then Throw New ArgumentNullException("name")
            If Equals(extensions, Nothing) Then Throw New ArgumentNullException("extensions")
            If Equals(appId, Nothing) Then Throw New ArgumentNullException("appId")

            Me.Name = name
            Me.Extensions = extensions
            Me.AppId = appId
            DisableLowILProcessIsolation = False
        End Sub

        Public Property Name As String
            Get
                Return _Name
            End Get
            Private Set(value As String)
                _Name = value
            End Set
        End Property

        Public Property Extensions As String
            Get
                Return _Extensions
            End Get
            Private Set(value As String)
                _Extensions = value
            End Set
        End Property

        Public Property AppId As String
            Get
                Return _AppId
            End Get
            Private Set(value As String)
                _AppId = value
            End Set
        End Property

        ''' <summary>
        ''' Disables low integrity-level process isolation.        
        ''' <remarks>This should be avoided as it could be a security risk.</remarks>
        ''' </summary>
        Public Property DisableLowILProcessIsolation As Boolean
    End Class

End Namespace
