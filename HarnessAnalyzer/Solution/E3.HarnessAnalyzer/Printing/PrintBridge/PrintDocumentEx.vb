Imports System.ComponentModel
Imports System.Drawing.Printing

Namespace Printing

    Public Class PrintDocumentEx
        Implements INotifyPropertyChanged

        Private WithEvents _printDoc As New System.Drawing.Printing.PrintDocument

        Public Event BeginPrint(sender As Object, e As PrintEventArgs)
        Public Event EndPrint(sender As Object, e As PrintEventArgs)
        Public Event PrintPage(sender As Object, e As PrintPageEventArgs)
        Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

        Private _hasChanges As Boolean
        Private WithEvents _printer As Printer

        Public Sub New()
        End Sub

        Private Sub _printDoc_BeginPrint(sender As Object, e As PrintEventArgs) Handles _printDoc.BeginPrint
            CommitChanges()
            RaiseEvent BeginPrint(Me, e)
        End Sub

        Private Sub _printDoc_EndPrint(sender As Object, e As PrintEventArgs) Handles _printDoc.EndPrint
            RaiseEvent EndPrint(Me, e)
        End Sub

        Private Sub _printDoc_PrintPage(sender As Object, e As PrintPageEventArgs) Handles _printDoc.PrintPage
            RaiseEvent PrintPage(Me, e)
        End Sub

        Private Sub _printerSettings_PropertyChanged(sender As Object, e As PropertyChangedEventArgs)
            OnPropertyChanged(New PropertyChangedEventArgs(NameOf(Printer)))
        End Sub

        Public Property OriginAtMargins As Boolean
            Get
                Return _printDoc.OriginAtMargins
            End Get
            Set(value As Boolean)
                If _printDoc.OriginAtMargins <> value Then
                    _printDoc.OriginAtMargins = value
                    OnPropertyChanged(New PropertyChangedEventArgs(NameOf(OriginAtMargins)))
                End If
            End Set
        End Property

        Public Property DocumentName As String
            Get
                Return _printDoc.DocumentName
            End Get
            Set(value As String)
                If _printDoc.DocumentName <> value Then
                    _printDoc.DocumentName = value
                    OnPropertyChanged(New PropertyChangedEventArgs(NameOf(DocumentName)))
                End If
            End Set
        End Property

        Public Property Printer As Printer
            Get
                Return _printer
            End Get
            Set(value As Printer)
                If Not _printer Is value Then
                    _printer = value
                    OnPropertyChanged(New PropertyChangedEventArgs(NameOf(Printer)))
                End If
            End Set
        End Property

        Public Sub Print()
            If Printer Is Nothing Then Throw New ArgumentException("Can't print: property ""Printer"" is not set!")
            CommitChanges()
            Try
                _printDoc.Print()
            Catch ex As Exception
                RaiseEvent EndPrint(Me, New PrintEventArgs()) ' HINT: EndPrint is not raised when exception happened
                Throw
            End Try
        End Sub

        Public Sub CommitChanges()
            _printDoc.PrinterSettings = Printer.CommitChanges()
            _hasChanges = False
        End Sub

        Public ReadOnly Property HasChanges As Boolean
            Get
                Return _hasChanges OrElse Printer.Settings.HasChanges
            End Get
        End Property

        Protected Overridable Sub OnPropertyChanged(e As PropertyChangedEventArgs)
            _hasChanges = True
            RaiseEvent PropertyChanged(Me, e)
        End Sub

        Public Overloads Shared Widening Operator CType(documentEx As PrintDocumentEx) As PrintDocument
            Return documentEx._printDoc
        End Operator

        Private Sub _printer_PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Handles _printer.PropertyChanged
            OnPropertyChanged(New PropertyChangedEventArgs(NameOf(Printer)))
        End Sub
    End Class

End Namespace