#If NETFRAMEWORK Or WINDOWS7_0_OR_GREATER Then
Imports System.Drawing
Imports System.Printing
Imports System.Reflection
Imports System.Runtime.Serialization
Imports System.Runtime.Serialization.Formatters.Binary

Namespace Printing

    <Serializable>
    Public Class Printer_2023
        Implements System.Runtime.Serialization.ISerializable

        Private _hasChanges As Boolean
        Private _isDefault As Boolean
        Private _internalPrinter As PrintQueue
        Private _papers As PagesCollectionEx_2023
        Private _printerCapabilites As PrintCapabilities
        Private _printResolution As List(Of Point)

        Private WithEvents _settings As PrinterSettingsEx_2023

        Public Sub New(printQueue As PrintQueue, parent As PrintersCollection_2023)
            Update(printQueue, parent)
            _settings = New PrinterSettingsEx_2023(Me)
        End Sub

        Friend Sub Update(printer As PrintQueue, parent As PrintersCollection_2023)
            _internalPrinter = printer
            With printer
                _Name = .Name
                _FullName = .FullName
            End With
            _Printers = parent
        End Sub

        Public Sub New(info As SerializationInfo, context As StreamingContext)
            With info
                Me.Name = .GetString("Name")
                Me.FullName = .GetString("FullName")
                Dim setData As String = .GetString("Settings")
                _settings = PrinterSettingsEx_2023.Deserialize(setData, Me)
                If _settings Is Nothing Then
                    _settings = New PrinterSettingsEx_2023(Me)
                End If
            End With
        End Sub

        Public Sub GetObjectData(info As SerializationInfo, context As StreamingContext) Implements ISerializable.GetObjectData
            With info
                .AddValue("Name", Me.Name)
                .AddValue("FullName", Me.FullName)
                .AddValue("Settings", Me.Settings.Serialize)
            End With
        End Sub

        ReadOnly Property Name As String
        ReadOnly Property FullName As String

        Property IsDefault As Boolean
            Get
                Return _isDefault
            End Get
            Set(value As Boolean)
                If _isDefault <> value Then
                    _isDefault = value
                End If
            End Set
        End Property

        ReadOnly Property Papers As PagesCollectionEx_2023
            Get
                Return _papers
            End Get
        End Property

        ReadOnly Property Printers As PrintersCollection_2023

        Property Settings As PrinterSettingsEx_2023
            Get
                Return _settings
            End Get
            Set(value As PrinterSettingsEx_2023)
                If Not _settings Is value Then
                    If value Is Nothing AndAlso _settings IsNot Nothing Then
                        CType(_settings, IPrinter_2023).Printer = Nothing
                    End If

                    _settings = value

                    If _settings IsNot Nothing Then
                        CType(_settings, IPrinter_2023).Printer = Me
                    End If
                End If
            End Set
        End Property

        ReadOnly Property HasChanges As Boolean
            Get
                Return _hasChanges
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return String.Format("{0} ({1})", Me.Name, Me.FullName)
        End Function

        Public Shared Function Deserialize(data As String) As Printer_2023
            Try
                Dim formatter As New BinaryFormatter
                Dim bytes As Byte() = Convert.FromBase64String(data)
                Using ms As New IO.MemoryStream(bytes)
                    Return CType(formatter.Deserialize(ms), Printer_2023)
                End Using
            Catch ex As TargetInvocationException
                Return Nothing
            Catch ex As System.Runtime.Serialization.SerializationException
                Return Nothing
            End Try
        End Function

        Public Function Serialize() As String
            Dim formatter As New BinaryFormatter
            Using stream As New System.IO.MemoryStream
                formatter.Serialize(stream, Me)
                Return Convert.ToBase64String(stream.ToArray)
            End Using
        End Function

        Public Function GetPrintResolution() As List(Of Point)
            Return _printResolution
        End Function
    End Class

End Namespace
#End If