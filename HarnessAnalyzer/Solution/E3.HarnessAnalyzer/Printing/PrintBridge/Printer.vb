Imports System.ComponentModel
Imports System.Drawing.Printing
Imports System.IO
Imports System.Printing
Imports System.Reflection
Imports System.Runtime.Serialization
Imports System.Xml
Imports Zuken.E3.HarnessAnalyzer.Printing.Printer

Namespace Printing

    <DataContract>
    <KnownType(GetType(PagesCollectionEx))>
    <KnownType(GetType(PrinterSettingsEx))>
    <KnownType(GetType(List(Of Point)))>
    Public Class Printer
        Implements INotifyPropertyChanged

        Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

        Private _hasChanges As Boolean
        Private _isDefault As Boolean
        Private _papers As PagesCollectionEx
        Private _printResolution As List(Of Point)
        Private _internalPrinter As PrintQueue

        Private Shared _lastPrintQueues As PrintQueueCollection
        <DataMember> Private WithEvents _settings As PrinterSettingsEx
        <DataMember> Private _name As String
        <DataMember> Private _fullName As String

        Friend Sub New(printer As Compatibility.Printing.Printer_2023, parent As PrintersCollection)
            If _lastPrintQueues Is Nothing Then
                _lastPrintQueues = Printing.PrintersCollection.GetAllLocalPrinterQueues
            End If

            Dim printQueue As PrintQueue = _lastPrintQueues.Where(Function(pq) pq.FullName = printer.FullName).FirstOrDefault
            Update(printQueue, parent)

            _hasChanges = printer.HasChanges
            _isDefault = printer.IsDefault
            _printResolution = printer.GetPrintResolution
            _papers = New PagesCollectionEx(printer.Papers)
            _settings = New PrinterSettingsEx(printer.Settings, Me)
            _Printers = parent
        End Sub

        Public Sub New(printQueue As PrintQueue, parent As PrintersCollection)
            Update(printQueue, parent)
            _settings = New PrinterSettingsEx(Me)
        End Sub

        Friend Sub Update(printer As PrintQueue, parent As PrintersCollection)
            _internalPrinter = printer
            With printer
                _name = .Name
                _fullName = .FullName
            End With
            _Printers = parent
        End Sub

        ReadOnly Property Name As String
            Get
                Return _name
            End Get
        End Property

        ReadOnly Property FullName As String
            Get
                Return _fullName
            End Get
        End Property

        Property IsDefault As Boolean
            Get
                Return _isDefault
            End Get
            Set(value As Boolean)
                If _isDefault <> value Then
                    _isDefault = value
                    OnPropertyChangedAuto()
                End If
            End Set
        End Property

        ReadOnly Property Papers As PagesCollectionEx
            Get
                SyncLock Me
                    If _papers Is Nothing Then
                        _papers = New PagesCollectionEx()
                        PrinterSettingsEx.ExecuteAppStyleSafe(
                                              Sub()
                                                  Try
                                                      Dim printerSetting As New System.Drawing.Printing.PrinterSettings
                                                      printerSetting.PrinterName = Me.FullName
                                                      For Each ps As PaperSize In printerSetting.PaperSizes
                                                          If Not String.IsNullOrEmpty(ps.PaperName) Then
                                                              _papers.AddNew(ps)
                                                          End If
                                                      Next
                                                  Catch ex As Exception
                                                      Throw New Exception("Exception when getting PaperSizes (PrinterSettings): " & ex.Message)
                                                  End Try
                                              End Sub)
                    End If
                    Return _papers
                End SyncLock
            End Get
        End Property

        ReadOnly Property Printers As PrintersCollection

        Property Settings As PrinterSettingsEx
            Get
                Return _settings
            End Get
            Set(value As PrinterSettingsEx)
                If Not _settings Is value Then
                    _settings = value
                    CType(_settings, IPrinter).Printer = Me
                    OnPropertyChangedAuto
                End If
            End Set
        End Property

        Friend Sub UpdateFromSettings(setting As System.Drawing.Printing.PrinterSettings)
            Try
                If setting.PrinterName <> Me.FullName Then
                    Throw New ArgumentException(String.Format("Invalid setting for this printer ({0}): The given setting is for printer ""{1}""", Me.FullName, setting.PrinterName))
                End If

                Me.Settings = New PrinterSettingsEx(Me, setting)

                If Not Me.Papers.Contains(_settings.Page.Name) Then
                    If _settings.Page.PageType = PageMediaSizeName.Unknown Then
                        Me.Papers.Add(_settings.Page)
                    Else
                        Throw New PageDoesNotExistException(String.Format("Given page ""{0}"" does not exist at this printer ({1})!", _settings.Page.Name, Me.Name), _settings.Page, Me)
                    End If
                End If
            Finally
                OnPropertyChanged(New PropertyChangedEventArgs(NameOf(Settings)))
            End Try
        End Sub

        Public Class PageDoesNotExistException
            Inherits Exception

            Public Sub New(message As String, page As PageEx, printer As Printer)
                MyBase.New(message)
                Me.Page = page
                Me.Printer = printer
            End Sub

            ReadOnly Property Page As PageEx
            ReadOnly Property Printer As Printer
        End Class

        Protected Overridable Sub OnPropertyChanged(e As PropertyChangedEventArgs)
            _hasChanges = True
            RaiseEvent PropertyChanged(Me, e)
        End Sub

        Private Sub _settings_PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Handles _settings.PropertyChanged
            OnPropertyChanged(New PropertyChangedEventArgs(NameOf(Settings)))
        End Sub

        ReadOnly Property HasChanges As Boolean
            Get
                Return _hasChanges OrElse Settings.HasChanges
            End Get
        End Property

        Public Function CommitChanges() As Drawing.Printing.PrinterSettings
            _hasChanges = False
            Return Settings.CommitChanges
        End Function

        Public Overrides Function ToString() As String
            Return String.Format("{0} ({1})", Me.Name, Me.FullName)
        End Function

        Public Shared Function ReadFromString(data As String) As Printer
            Dim uncompressed As Byte() = Nothing
            If Compression.UtilsEx.TryGUnzipString64(data, uncompressed) Then
                Using ucs As New MemoryStream(uncompressed)
                    Dim ser As New DataContractSerializer(GetType(Printer))
                    Return CType(ser.ReadObject(ucs), Printer)
                End Using
            End If

            Using ms As New IO.MemoryStream(Convert.FromBase64String(data))
                Try
                    If BinaryFile.IsBinFormatted(ms) Then
                        Dim printer2023 As Compatibility.Printing.Printer_2023 = Compatibility.Printing.Printer_2023.Deserialize(data)
                        Return New Printer(printer2023, Nothing)
                    ElseIf XmlFile.IsXml(ms) Then
                        ms.Position = 0
                        Dim ser As New DataContractSerializer(GetType(Printer))
                        Return CType(ser.ReadObject(ms), Printer)
                    Else
                        Throw New NotSupportedException("File format not supported for deserializing printer-data!")
                    End If
                Catch ex As TargetInvocationException
                    Return Nothing
                Catch ex As System.Runtime.Serialization.SerializationException
                    Return Nothing
                End Try
            End Using
        End Function

        Public Function SaveAsCompressedString() As String
            Using ms As New IO.MemoryStream
                Save(ms)
                ms.Position = 0
                Return System.IO.Compression.UtilsEx.GZipToString64(ms)
            End Using
        End Function

        Public Sub Save(s As System.IO.Stream)
            Dim settings As New XmlWriterSettings
            settings.Indent = True

            Using writer As XmlWriter = XmlWriter.Create(s, settings)
                Dim ds As New DataContractSerializer(Me.GetType)
                ds.WriteObject(writer, Me)
            End Using
        End Sub

    End Class

End Namespace