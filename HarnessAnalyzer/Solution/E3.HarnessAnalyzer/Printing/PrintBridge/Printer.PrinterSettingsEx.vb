Imports System.ComponentModel
Imports System.IO
Imports System.Reflection
Imports System.Runtime.InteropServices
Imports System.Runtime.Serialization

Namespace Printing

    Partial Public Class Printer

        <DataContract>
        <KnownType(GetType(PageEx))>
        <KnownType(GetType(Nullable(Of Integer)))>
        <KnownType(GetType(Nullable(Of Boolean)))>
        Public Class PrinterSettingsEx
            Implements INotifyPropertyChanged
            Implements IPrinter

            Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

            Public Const DEFAULT_COPIES As Integer = 1

            Private _hasChanges As Boolean
            Private _lastCommittedSettings As System.Drawing.Printing.PrinterSettings

            <DataMember> Private _copies As Nullable(Of Integer) = Nothing
            <DataMember> Private _landscape As Nullable(Of Boolean) = Nothing
            <DataMember> Private WithEvents _page As PageEx

            Friend Sub New(settings2023 As Compatibility.Printing.Printer_2023.PrinterSettingsEx_2023, Optional owningPrinter As Printer = Nothing)
                Me.New(owningPrinter)
                _copies = settings2023.Copies
                _landscape = settings2023.Landscape
                If settings2023.Page IsNot Nothing Then
                    _page = New PageEx(settings2023.Page)
                End If
            End Sub

            Public Sub New(owningPrinter As Printer)
                Me.Printer = owningPrinter
            End Sub

            Private Sub InitProperties()
                If Me.Printer._internalPrinter.DefaultPrintTicket IsNot Nothing Then
                    With Me.Printer._internalPrinter.DefaultPrintTicket
                        If .PageMediaSize.PageMediaSizeName.HasValue Then
                            If .PageMediaSize.PageMediaSizeName.Value <> System.Printing.PageMediaSizeName.Unknown Then
                                If Me.Printer.Papers.Contains(.PageMediaSize.PageMediaSizeName.Value) Then
                                    _page = Me.Printer.Papers(.PageMediaSize.PageMediaSizeName.Value)
                                Else
                                    ' HINT: there is a case where the current page size isn't contained in the pages!
                                    '       We don't know the reason for that, but we simply fallback to A4/Default size in that case, seemingly it could have something to do with network-printers (not available or partial available or something else)
                                    _page = Nothing
                                End If
                            Else
                                _page = Me.Printer.Papers.AddNew(.PageMediaSize)
                            End If
                        End If

                        _copies = .CopyCount.GetValueOrDefault

                        Select Case .PageOrientation.Value
                            Case System.Printing.PageOrientation.Landscape
                                _landscape = True
                            Case System.Printing.PageOrientation.Portrait
                                _landscape = False
                        End Select
                    End With

                    If _page Is Nothing Then
                        _page = Me.Printer.Papers.GetByTypeOrDefault(System.Printing.PageMediaSizeName.ISOA4)
                    End If
                End If
            End Sub

            Friend Shared Sub ExecuteAppStyleSafe(action As Action)
                Dim oldState As VisualStyles.VisualStyleState = Application.VisualStyleState
                action.Invoke
                If oldState <> Application.VisualStyleState Then
                    Application.VisualStyleState = oldState
                End If
            End Sub

            Public Sub New(owningPrinter As Printer, settings As System.Drawing.Printing.PrinterSettings)
                Me.New(owningPrinter)
                _page = New PageEx(settings.DefaultPageSettings.PaperSize)
                _landscape = settings.DefaultPageSettings.Landscape
                _copies = settings.Copies
            End Sub

            Public Property Page As PageEx
                Get
                    If _page Is Nothing Then
                        ExecuteAppStyleSafe(Sub() InitProperties())
                    End If

                    Return _page
                End Get
                Set(value As PageEx)
                    If Not _page Is value Then
                        _page = value
                        OnPropertyChanged(New PropertyChangedEventArgs(NameOf(Page)))
                    End If
                End Set
            End Property

            Public Property Landscape As Boolean
                Get
                    If _landscape Is Nothing Then
                        ExecuteAppStyleSafe(Sub() InitProperties())
                    End If
                    Return _landscape.GetValueOrDefault
                End Get
                Set(value As Boolean)
                    If _landscape <> value Then
                        _landscape = value
                        OnPropertyChanged(New PropertyChangedEventArgs(NameOf(Landscape)))
                    End If
                End Set
            End Property

            Public Property Copies As Integer
                Get
                    If _copies Is Nothing Then
                        ExecuteAppStyleSafe(Sub() InitProperties())
                    End If
                    If _copies IsNot Nothing Then
                        Return _copies.Value
                    Else
                        Return DEFAULT_COPIES
                    End If
                End Get
                Set(value As Integer)
                    If _copies <> value Then
                        _copies = value
                        OnPropertyChanged(New PropertyChangedEventArgs(NameOf(Copies)))
                    End If
                End Set
            End Property

            Public ReadOnly Property Printer As Printer

            Protected Overridable Sub OnPropertyChanged(e As PropertyChangedEventArgs)
                _hasChanges = True
                RaiseEvent PropertyChanged(Me, e)
            End Sub

            Private Sub _page_PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Handles _page.PropertyChanged
                OnPropertyChanged(New PropertyChangedEventArgs(NameOf(Page)))
            End Sub

            Public Function CommitChanges() As System.Drawing.Printing.PrinterSettings
                If _hasChanges OrElse _lastCommittedSettings Is Nothing Then
                    _hasChanges = False
                    Return ToPrinterSettings()
                End If
                Return _lastCommittedSettings
            End Function

            Private Function ToPrinterSettings() As System.Drawing.Printing.PrinterSettings
                _lastCommittedSettings = New System.Drawing.Printing.PrinterSettings()
                With _lastCommittedSettings
                    .PrinterName = Me.Printer.FullName
                    .DefaultPageSettings.PaperSize = Me.Page.ToPaperSize
                    .DefaultPageSettings.Landscape = Me.Landscape
                    .DefaultPageSettings.Margins = Me.Page.Margins.ToMargins
                    .Copies = CShort(Me.Copies)
                End With
                Return _lastCommittedSettings
            End Function

            Public ReadOnly Property HasChanges As Boolean
                Get
                    Return _hasChanges
                End Get
            End Property

            Private Property IPrinter_Printer As Printer Implements IPrinter.Printer
                Get
                    Return Me.Printer
                End Get
                Set(value As Printer)
                    If _Printer IsNot value Then
                        Me._Printer = value
                        OnPropertyChanged(New PropertyChangedEventArgs(NameOf(Printer)))
                    End If
                End Set
            End Property

            Public Function ShowEditPrinterSettingsDialog(owner As IWin32Window, Optional cancel As System.Threading.CancellationToken = Nothing) As DialogResult
                Dim newPrinterSettings As System.Drawing.Printing.PrinterSettings = Me.CommitChanges()

                With newPrinterSettings
                    Dim myReturnValue As DialogResult = DialogResult.Cancel
                    Dim hDevMode As IntPtr = .GetHdevmode(newPrinterSettings.DefaultPageSettings)
                    Dim pDevMode As IntPtr = System.Windows.Native.GlobalLock(hDevMode)
                    Dim sizeNeeded As Integer = System.Windows.Native.DocumentProperties(owner.Handle, IntPtr.Zero, .PrinterName, IntPtr.Zero, pDevMode, 0)
                    Dim devModeData As IntPtr = Marshal.AllocHGlobal(sizeNeeded)
                    Dim userChoice As Long = System.Windows.Native.DocumentProperties(owner.Handle, IntPtr.Zero, .PrinterName, devModeData, pDevMode, (System.Windows.Native.DM_IN_BUFFER Or (System.Windows.Native.DM_PROMPT Or System.Windows.Native.DM_OUT_BUFFER)))

                    Try
                        If userChoice = DialogResult.OK Then
                            myReturnValue = DialogResult.OK
                            .SetHdevmode(devModeData)
                            .DefaultPageSettings.SetHdevmode(devModeData)
                            Try
                                Me.Printer.Printers.AddNewOrUpdateFrom(newPrinterSettings)
                            Catch ex As Printer.PageDoesNotExistException
                                ex.Printer.Settings.Page = PageEx.NewCustom(ex.Printer.Settings.Page.Width, ex.Printer.Settings.Page.Height, ex.Printer.Settings.Page.Margins)
                                MessageBox.Show(String.Format(ErrorStrings.Printing_MissingPageToCustomUpdate, ex.Page.Name, ex.Printer.Name), [Shared].MSG_BOX_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            End Try
                        End If
                    Finally
                        System.Windows.Native.GlobalUnlock(hDevMode)
                        System.Windows.Native.GlobalFree(hDevMode)
                        Marshal.FreeHGlobal(devModeData)
                    End Try
                    Return myReturnValue
                End With
            End Function

            Public Shared Function Deserialize(data As String, printer As Printer) As PrinterSettingsEx
                Try
                    Using ms As New MemoryStream(Convert.FromBase64String(data))
                        If BinaryFile.IsBinFormatted(ms) Then
                            Dim pSettings2023 As Compatibility.Printing.Printer_2023.PrinterSettingsEx_2023 = Compatibility.Printing.Printer_2023.PrinterSettingsEx_2023.Deserialize(data)
                            Return New PrinterSettingsEx(pSettings2023) With {._Printer = printer}
                        ElseIf XmlFile.IsXml(ms) Then
                            ms.Position = 0
                            Dim settings As PrinterSettingsEx = DataContractXmlFile.Load(Of PrinterSettingsEx)(ms)
                            settings._Printer = printer
                            Return settings
                        Else
                            Throw New NotSupportedException("File format for deserializing printersettingsEx is not supported!")
                        End If
                    End Using
                Catch ex As TargetInvocationException
                    Return Nothing
                Catch ex As System.Runtime.Serialization.SerializationException
                    Return Nothing
                End Try
            End Function

            'Public Function Serialize() As String
            '    Using ms As New MemoryStream()
            '        XmlFile.WriteObject(Me, ms)
            '        Return Convert.ToBase64String(ms.ToArray)
            '    End Using
            'End Function

        End Class

        Private Interface IPrinter
            Property Printer As Printer
        End Interface

    End Class

End Namespace