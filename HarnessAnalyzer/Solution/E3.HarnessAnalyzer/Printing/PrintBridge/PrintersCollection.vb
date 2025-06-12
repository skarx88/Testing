Imports System.Collections.Specialized
Imports System.Printing
Imports System.Runtime.Serialization

Namespace Printing

    <CollectionDataContract>
    <KnownType(GetType(Printer))>
    Public Class PrintersCollection
        Inherits ObjectModel.KeyedCollection(Of String, Printer)
        Implements INotifyCollectionChanged

        Public Event CollectionChanged As NotifyCollectionChangedEventHandler Implements INotifyCollectionChanged.CollectionChanged

        Private _eventsEnabled As Boolean

        Private Shared _lastPrinterCollection As PrintQueueCollection

        Friend Sub New(printers2023 As Compatibility.Printing.PrintersCollection_2023)
            Me.Init(printers2023.Select(Function(p) New Printer(p, Me)))
        End Sub

        Private Sub New(printers As IEnumerable(Of Printer))
            Me.Init(printers)
        End Sub

        Private Sub Init(printers As IEnumerable(Of Printer))
            For Each pt As Printer In printers
                Me.Add(pt)
            Next
            Sync()
            _eventsEnabled = True
        End Sub

        Public Sub New(printers As IEnumerable(Of PrintQueue))
            MyBase.New()
            For Each pt As PrintQueue In printers
                Me.Add(New Printer(pt, Me))
            Next

            RefreshDefault()
            _eventsEnabled = True
        End Sub

        ReadOnly Property [Default] As Printer

        Public Sub Sync()
            For Each pt As PrintQueue In GetAllLocalPrinterQueues()
                If Not Me.Contains(pt.FullName) Then
                    Me.Remove(pt.FullName)
                Else
                    Me(pt.FullName).Update(pt, Me)
                End If
            Next
            RefreshDefault()
        End Sub

        Public Sub RefreshDefault()
            _Default = Nothing
            If Me.Count > 0 Then
                If Me.Default IsNot Nothing Then
                    Me.Default.IsDefault = False
                End If

                Dim defPrinter As PrintQueue = Nothing
                Try
                    defPrinter = LocalPrintServer.GetDefaultPrintQueue()
                    If Me.Contains(defPrinter.FullName) Then
                        Me(defPrinter.FullName).IsDefault = True
                        _Default = Me(defPrinter.FullName)
                    End If
                Catch ex As Exception
                    ' no default printer or anything was found (this exception happens when no printers are installed on the system)
                End Try
            End If
        End Sub

        Protected Overrides Function GetKeyForItem(item As Printer) As String
            Return item.FullName
        End Function

        Public Shared Function GetAllLocalPrinters(Optional useCache As Boolean = True) As PrintersCollection
            Dim coll As PrintersCollection = Nothing
            If useCache AndAlso Not NeedsUpdate Then
                coll = New PrintersCollection(_lastPrinterCollection)
            Else
                coll = New PrintersCollection(GetAllLocalPrinterQueues())
            End If
            Return coll
        End Function

        Public Shared ReadOnly Property NeedsUpdate As Boolean
            Get
                Return _lastPrinterCollection Is Nothing OrElse Not (InstalledPrinterNames.SequenceEqual(_lastPrinterCollection.Select(Function(item) item.FullName)))
            End Get
        End Property

        Friend Shared Function GetAllLocalPrinterQueues() As PrintQueueCollection
            Dim printers As New PrintQueueCollection()
            Try
                printers = (New LocalPrintServer).GetPrintQueues({EnumeratedPrintQueueTypes.Local, EnumeratedPrintQueueTypes.Connections})
            Catch ex As Exception
                Throw New Exception("Exception when getting printers from local print server (GetPrintQueues): " & ex.Message)
            End Try
            _lastPrinterCollection = printers
            Return printers
        End Function

        Public Function AddNewOrUpdateFrom(settings As Drawing.Printing.PrinterSettings) As Printer
            Dim printerName As String = String.Empty
            Dim oldAppStyle As VisualStyles.VisualStyleState = Application.VisualStyleState
            printerName = settings.PrinterName
            If oldAppStyle <> Application.VisualStyleState Then Application.VisualStyleState = oldAppStyle

            If Not Me.Contains(settings.PrinterName) Then
                Dim ptQeue As PrintQueue = GetAllLocalPrinterQueues.Where(Function(pt) pt.FullName = settings.PrinterName).FirstOrDefault
                If ptQeue Is Nothing Then
                    Throw New ArgumentException(String.Format("Printer ""{0}"" does not exist as a local printer!", settings.PrinterName))
                End If
                Me.Add(New Printer(ptQeue, Me))
            End If

            Dim printer As Printer = Me(settings.PrinterName)
            Try
                printer.UpdateFromSettings(settings)
            Finally
                Me.RefreshDefault()
            End Try
            Return printer
        End Function

        Protected Overrides Sub ClearItems()
            MyBase.ClearItems()
            OnCollectionChanged(New NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset))
        End Sub

        Protected Overrides Sub InsertItem(index As Integer, item As Printer)
            MyBase.InsertItem(index, item)
            OnCollectionChanged(New NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item))
        End Sub

        Protected Overrides Sub RemoveItem(index As Integer)
            Dim item As Printer = Me(index)
            MyBase.RemoveItem(index)
            OnCollectionChanged(New NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item))
        End Sub

        Protected Overrides Sub SetItem(index As Integer, item As Printer)
            Dim oldItem As Printer = Me(index)
            MyBase.SetItem(index, item)
            OnCollectionChanged(New NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, item, oldItem))
        End Sub

        Protected Overridable Sub OnCollectionChanged(e As NotifyCollectionChangedEventArgs)
            If _eventsEnabled Then
                RaiseEvent CollectionChanged(Me, e)
            End If
        End Sub

        Public Class AddUpdateResult
            Public Sub New(addedNew As Boolean, printer As Printer)
                Me.AddedNew = addedNew
                Me.Printer = printer
            End Sub

            ReadOnly Property AddedNew As Boolean
            ReadOnly Property Printer As Printer
        End Class

        Public Shared Function Deserialize(data As String) As PrintersCollection
            Dim bytes As Byte() = Convert.FromBase64String(data)
            Using ms As New IO.MemoryStream(bytes)
                If XmlFile.IsXml(ms) Then
                    ms.Position = 0
                    Dim pList As New List(Of Printer)
                    For Each kv As KeyValuePair(Of String, String) In DataContractXmlFile.ReadObject(Of Dictionary(Of String, String))(ms, {GetType(Dictionary(Of String, String))})
                        Dim printer As Printer = Printer.ReadFromString(kv.Value)
                        pList.Add(printer)
                    Next
                    Return New PrintersCollection(pList)
                Else
                    Dim oldCollection As Compatibility.Printing.PrintersCollection_2023 = Compatibility.Printing.PrintersCollection_2023.Deserialize(data)
                    Return New PrintersCollection(oldCollection)
                End If
            End Using
        End Function

        'Public Function Serialize() As String
        '    Using stream As New System.IO.MemoryStream
        '        Dim dic As New Dictionary(Of String, String)
        '        For Each item As Printer In Me
        '            dic.Add(item.FullName, item.Writebject)
        '        Next

        '        XmlFile.WriteObject(dic, stream, , {GetType(Dictionary(Of String, String))})
        '        Return Convert.ToBase64String(stream.ToArray)
        '    End Using
        'End Function

        Shared ReadOnly Property InstalledPrinterNames As String()
            Get
                Return Drawing.Printing.PrinterSettings.InstalledPrinters.Cast(Of String).ToArray
            End Get
        End Property

    End Class

End Namespace