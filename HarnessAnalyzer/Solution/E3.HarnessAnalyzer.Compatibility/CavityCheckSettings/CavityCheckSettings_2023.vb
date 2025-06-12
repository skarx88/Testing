Imports System.IO
Imports System.Reflection
Imports System.Runtime.Serialization
Imports System.Xml

<Serializable()>
<KnownType(GetType(ConnectorSettingCollection_2023))>
<KnownType(GetType(ModuleSettingCollection_2023))>
Public Class CavityCheckSettings_2023
    Implements ISerializable

    Public Sub New()
    End Sub

    Friend Sub New(info As SerializationInfo, context As StreamingContext)
        For Each entry As SerializationEntry In info
            Select Case entry.Name
                Case "Connectors"
                    Connectors = CType(entry.Value, ConnectorSettingCollection_2023)
                Case "Modules"
                    ActiveModules = CType(entry.Value, ModuleSettingCollection_2023)
                Case "ActiveHarnessConfigurationId"
                    _ActiveHarnessConfigurationId = CStr(entry.Value)
            End Select
        Next
    End Sub

    Property Connectors As New ConnectorSettingCollection_2023
    Property ActiveModules As New ModuleSettingCollection_2023
    Property ActiveHarnessConfigurationId As String

    Public Sub GetObjectData(info As SerializationInfo, context As StreamingContext) Implements ISerializable.GetObjectData
        info.AddValue("Connectors", Me.Connectors)
        info.AddValue("Modules", Me.ActiveModules)
        info.AddValue("ActiveHarnessConfigurationId", ActiveHarnessConfigurationId)
    End Sub

    Public Shared Function LoadFromFile(filePath As String) As CavityCheckSettings_2023
        Using fs As New FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read)
            Return Load(fs)
        End Using
    End Function

    Public Shared Function Load(s As System.IO.Stream) As CavityCheckSettings_2023
        Dim ser1 As New DataContractSerializer(GetType(CavityCheckSettings_2023), "CavityCheckSettings", "http://schemas.datacontract.org/2004/07/Zuken.E3.HarnessAnalyzer.Checks.Cavities.Settings")
        Dim dsSettings As New DataContractSerializerSettings()
        dsSettings.RootName = CType(ser1.GetType.GetField("_rootName", BindingFlags.NonPublic Or BindingFlags.Instance).GetValue(ser1), XmlDictionaryString)
        dsSettings.RootNamespace = CType(ser1.GetType.GetField("_rootNamespace", BindingFlags.NonPublic Or BindingFlags.Instance).GetValue(ser1), XmlDictionaryString)
        dsSettings.DataContractResolver = New TypeResolver

        Dim ser As New DataContractSerializer(GetType(CavityCheckSettings_2023), dsSettings)
        'Dim s As New DataContractSerializer(GetType(String), "", "")
        Return CType(ser.ReadObject(s), CavityCheckSettings_2023)
    End Function

    Public Shared Function Is2023Xml(s As System.IO.Stream) As Boolean
        Dim oldPosition As Long = s.Position
        Try
            If s.CanSeek Then
                s.Position = 0
            End If

            Dim hasStarted As Boolean = False
            Using r As New StreamReader(s, System.Text.Encoding.Default, True, 1024, True)
                Dim line As String = Nothing
                Do While True
                    line = r.ReadLine()
                    If String.IsNullOrEmpty(line) Then
                        Exit Do
                    ElseIf Not hasStarted Then
                        If line.StartsWith("<") Then
                            hasStarted = True
                        End If
                        If line.Trim.StartsWith(My.Resources.CavityCheckSettingsRoot2023) Then
                            Return True
                        End If
                    ElseIf hasStarted Then ' the first element should be the root be we didn't resolve it before so exit returning false
                        Exit Do
                    End If
                Loop
            End Using
            Return False
        Finally
            If s.CanSeek Then
                s.Position = oldPosition
            End If
        End Try
    End Function

    Private Class TypeResolver
        Inherits DataContractResolver

        Public Overrides Function TryResolveType(type As Type, declaredType As Type, knownTypeResolver As DataContractResolver, ByRef typeName As Xml.XmlDictionaryString, ByRef typeNamespace As Xml.XmlDictionaryString) As Boolean
            Return False
        End Function

        Public Overrides Function ResolveName(typeName As String, typeNamespace As String, declaredType As Type, knownTypeResolver As DataContractResolver) As Type
            If typeName = "ArrayOfConnectorSetting" Then
                Return GetType(ConnectorSettingCollection_2023)
            End If
            If typeName = "ArrayOfModuleSetting" Then
                Return GetType(ModuleSettingCollection_2023)
            End If
            If typeName = "string" Then
                Return GetType(String)
            End If

            Throw New NotImplementedException("type resolve not implemented: " + typeName)
        End Function
    End Class

End Class




