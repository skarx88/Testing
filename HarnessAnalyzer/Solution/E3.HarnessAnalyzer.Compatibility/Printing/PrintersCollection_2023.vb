#If NETFRAMEWORK Or WINDOWS7_0_OR_GREATER Then
Imports System.Printing
Imports System.Runtime.Serialization.Formatters.Binary

Namespace Printing

    Public Class PrintersCollection_2023
        Inherits ObjectModel.KeyedCollection(Of String, Printer_2023)

        Private Shared _lastPrinterCollection As PrintQueueCollection

        Private Sub New(printers As IEnumerable(Of Printer_2023))
            For Each pt As Printer_2023 In printers
                Me.Add(pt)
            Next
            Sync()
        End Sub

        Public Sub New(printers As IEnumerable(Of PrintQueue))
            MyBase.New()
            For Each pt As PrintQueue In printers
                Me.Add(New Printer_2023(pt, Me))
            Next

            RefreshDefault()
        End Sub

        ReadOnly Property [Default] As Printer_2023

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

        Protected Overrides Function GetKeyForItem(item As Printer_2023) As String
            Return item.FullName
        End Function

        Public Shared Function GetAllLocalPrinters(Optional useCache As Boolean = True) As PrintersCollection_2023
            Dim coll As PrintersCollection_2023 = Nothing
            If useCache AndAlso Not NeedsUpdate Then
                coll = New PrintersCollection_2023(_lastPrinterCollection)
            Else
                coll = New PrintersCollection_2023(GetAllLocalPrinterQueues())
            End If
            Return coll
        End Function

        Public Shared ReadOnly Property NeedsUpdate As Boolean
            Get
                Return _lastPrinterCollection Is Nothing OrElse Not (InstalledPrinterNames.SequenceEqual(_lastPrinterCollection.Select(Function(item) item.FullName)))
            End Get
        End Property

        Private Shared Function GetAllLocalPrinterQueues() As PrintQueueCollection
            Dim printers As New PrintQueueCollection()
            Try
                printers = (New LocalPrintServer).GetPrintQueues({EnumeratedPrintQueueTypes.Local, EnumeratedPrintQueueTypes.Connections})
            Catch ex As Exception
                Throw New Exception("Exception when getting printers from local print server (GetPrintQueues): " & ex.Message)
            End Try
            _lastPrinterCollection = printers
            Return printers
        End Function

        Public Shared Function Deserialize(data As String) As PrintersCollection_2023
            Dim formatter As New BinaryFormatter
            Dim bytes As Byte() = Convert.FromBase64String(data)
            Using ms As New IO.MemoryStream(bytes)
                Dim pList As New List(Of Printer_2023)
                For Each kv As KeyValuePair(Of String, String) In CType(formatter.Deserialize(ms), Dictionary(Of String, String))
                    Dim printer As Printer_2023 = Printer_2023.Deserialize(kv.Value)
                    pList.Add(printer)
                Next
                Return New PrintersCollection_2023(pList)
            End Using
        End Function

        Public Function Serialize() As String
            Dim formatter As New BinaryFormatter
            Using stream As New System.IO.MemoryStream
                Dim dic As New Dictionary(Of String, String)
                For Each item As Printer_2023 In Me
                    dic.Add(item.FullName, item.Serialize)
                Next

                formatter.Serialize(stream, dic)
                Return Convert.ToBase64String(stream.ToArray)
            End Using
        End Function

        Shared ReadOnly Property InstalledPrinterNames As String()
            Get
                Return Drawing.Printing.PrinterSettings.InstalledPrinters.Cast(Of String).ToArray
            End Get
        End Property

    End Class

End Namespace
#End If