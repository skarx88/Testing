#If NETFRAMEWORK Or WINDOWS7_0_OR_GREATER Then
Imports System.ComponentModel
Imports System.IO
Imports System.Reflection
Imports System.Runtime.Serialization

Namespace Printing

    Partial Public Class Printer_2023

        <Serializable>
        Public Class PrinterSettingsEx_2023
            Implements INotifyPropertyChanged
            Implements ISerializable
            Implements IPrinter_2023

            Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

            Public Const DEFAULT_COPIES As Integer = 1

            Private _copies As Nullable(Of Integer) = Nothing
            Private _landscape As Nullable(Of Boolean) = Nothing
            Private WithEvents _page As PageEx_2023

            Public Sub New(owningPrinter As Printer_2023)
                Me.Printer = owningPrinter
            End Sub

            Public Sub New(info As SerializationInfo, context As StreamingContext)
                With info
                    _landscape = .GetBoolean("Landscape")
                    _copies = CShort(.GetValue("Copies", GetType(Short)))
                    _page = CType(.GetValue("Page", GetType(PageEx_2023)), PageEx_2023)
                End With
            End Sub

            Public Sub GetObjectData(info As SerializationInfo, context As StreamingContext) Implements ISerializable.GetObjectData
                With info
                    .AddValue("Landscape", Me.Landscape)
                    .AddValue("Copies", Me.Copies)
                    .AddValue("Page", Me.Page)
                End With
            End Sub

            Property Page As PageEx_2023
                Get
                    Return _page
                End Get
                Set(value As PageEx_2023)
                    If Not _page Is value Then
                        _page = value
                    End If
                End Set
            End Property

            Property Landscape As Boolean
                Get
                    Return _landscape.GetValueOrDefault
                End Get
                Set(value As Boolean)
                    If _landscape <> value Then
                        _landscape = value
                    End If
                End Set
            End Property

            Property Copies As Integer
                Get
                    If _copies IsNot Nothing Then
                        Return _copies.Value
                    Else
                        Return DEFAULT_COPIES
                    End If
                End Get
                Set(value As Integer)
                    If _copies <> value Then
                        _copies = value
                    End If
                End Set
            End Property

            ReadOnly Property Printer As Printer_2023

            Private Property IPrinter_Printer As Printer_2023 Implements IPrinter_2023.Printer
                Get
                    Return Me.Printer
                End Get
                Set(value As Printer_2023)
                    If _Printer IsNot value Then
                        Me._Printer = value
                    End If
                End Set
            End Property

            Public Shared Function Deserialize(data As String, Optional printer As Printer_2023 = Nothing) As PrinterSettingsEx_2023
                Try
                    Dim formatter As New System.Runtime.Serialization.Formatters.Binary.BinaryFormatter
                    Using ms As New MemoryStream(Convert.FromBase64String(data))
                        Dim settings As PrinterSettingsEx_2023 = CType(formatter.Deserialize(ms), PrinterSettingsEx_2023)
                        settings._Printer = printer
                        Return settings
                    End Using
                Catch ex As TargetInvocationException
                    Return Nothing
                Catch ex As System.Runtime.Serialization.SerializationException
                    Return Nothing
                End Try
            End Function

            Public Function Serialize() As String
                Dim formatter As New System.Runtime.Serialization.Formatters.Binary.BinaryFormatter
                Using ms As New MemoryStream()
                    formatter.Serialize(ms, Me)
                    Return Convert.ToBase64String(ms.ToArray)
                End Using
            End Function
        End Class

        Private Interface IPrinter_2023
            Property Printer As Printer_2023
        End Interface

    End Class

End Namespace
#End If