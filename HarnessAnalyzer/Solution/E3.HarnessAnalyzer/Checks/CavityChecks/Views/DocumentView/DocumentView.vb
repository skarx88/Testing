Imports System.ComponentModel
Imports Zuken.E3.HarnessAnalyzer.Checks.Cavities.Files
Imports Zuken.E3.HarnessAnalyzer.Checks.Cavities.Views.Model
Imports Zuken.E3.HarnessAnalyzer.Project.Documents
Imports Zuken.E3.Lib.Eyeshot.Model

Namespace Checks.Cavities.Views.Document

    Public Class DocumentView
        Implements INotifyPropertyChanged
        Implements IDisposable

        Private Shared _count As Integer = 0

        Private WithEvents _model As ModelView
        Private WithEvents _canvas As CanvasView

        Private _isCanvasSelecting As Boolean = False
        Private _isModelSelecting As Boolean = False
        Private _redliningInfo As RedliningInformation
        Private _kbl As KBLMapper
        Private _document As HcvDocument
        Private _active As Boolean
        Private _disposedValue As Boolean
        Private _documentForm As DocumentForm

        Public Event ActiveModulesChanged(sender As Object, e As ActiveModulesChangedEventArgs)
        Public Event ModelSelectionChanged(sender As Object, e As EventArgs)
        Public Event InitializeRedliningDialog(sender As Object, e As EventArgs)
        Public Event RedliningChanged(sender As Object, e As EventArgs)
        Public Event CanvasSelectionChanged(sender As Object, e As EventArgs)
        Public Event Closing(sender As Object, e As EventArgs)
        Public Event Closed(sender As Object, e As EventArgs)
        Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
        Public Event ModelChanged(sender As Object, e As EventArgs)
        Public Event NeedsInactiveModuleSettingsApplied(sender As Object, e As EventArgs)

        Public Sub New()
        End Sub

        Public Sub New(name As String, kbl As KBLMapper, document As DocumentForm)
            Me.New(name, Guid.NewGuid.ToString, kbl, document)
        End Sub

        Public Sub New(name As String, id As String, kbl As KBLMapper, documentForm As DocumentForm)
            Me.New(kbl, documentForm)
            Me.Id = id
            Me.Name = name
            _documentForm = documentForm
        End Sub

        Public Sub New(kbl As KBLMapper, document As DocumentForm)
            _kbl = kbl
            _model = New ModelView(kbl)
            'Model.ReadonlyDelegate = AddressOf _modeView_ReadOnly
            Model.Document = Me
            Model.Update()
            _Has3DData = document.Document.AnalyseContent(HcvDocument.ContentAnalyseType.HasKBLZData)
            _document = document.Document
            _canvas = New CanvasView(document)
            _count += 1
            Me.Name = $"Document {_count.ToString}"
        End Sub

        ReadOnly Property DocumentForm As DocumentForm
            Get
                Return _documentForm
            End Get
        End Property

        ReadOnly Property Has3DData As Boolean

        Public Property RedLiningInformation As RedliningInformation '**
            Get
                Return _redliningInfo
            End Get
            Set(value As RedliningInformation)
                _redliningInfo = value
            End Set
        End Property

        Public Sub RaiseInitializeRedliningDialog()
            RaiseEvent InitializeRedliningDialog(Me, Nothing)
        End Sub

        Public Sub UpdateRedliningIconsForCavityNavigator()
            RaiseEvent RedliningChanged(Me, Nothing)
        End Sub

        <Browsable(False)>
        ReadOnly Property Id As String
        ReadOnly Property Model As ModelView
            Get
                Return _model
            End Get
        End Property

        ReadOnly Property Canvas As CanvasView
            Get
                Return _canvas
            End Get
        End Property

        Public Function Get3DEntityClones(kblId As String) As IBaseModelEntityEx()
            Return _document.Entities.GetAsClones(kblId)
        End Function

        Property Name As String

        Property Active As Boolean
            Get
                Return _active
            End Get
            Set(value As Boolean)
                If _active <> value Then
                    _active = value
                    OnPropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Active)))
                End If
            End Set
        End Property

        Protected Overridable Sub OnPropertyChanged(sender As Object, e As PropertyChangedEventArgs)
            RaiseEvent PropertyChanged(sender, e)
        End Sub

        Public Sub Invalidate()
            Me.Model.Invalidate()
        End Sub

        Protected Overridable Sub OnModelChanged(sender As Object, e As EventArgs)
            RaiseEvent ModelChanged(sender, e)
        End Sub

        Private Sub _model_Changed(sender As Object, e As EventArgs) Handles _model.Changed
            OnModelChanged(Me, e)
        End Sub

        Protected Overridable Sub OnNeedsInactiveModuleSettingsApplied(sender As Object, e As EventArgs)
            RaiseEvent NeedsInactiveModuleSettingsApplied(sender, e)
        End Sub

        Friend Sub RaiseNeedsInactiveModuleSettingsApplied()
            OnNeedsInactiveModuleSettingsApplied(Me, New EventArgs)
        End Sub

        Public Sub Export(filepath As String, fileFormat As ExportFileFormat)
            Select Case fileFormat
                Case ExportFileFormat.Unknown
                    Throw New NotSupportedException("Exporting to unknown file format is not supported!")
                Case ExportFileFormat.CavityChecksInformation
                    ExportCavityChecksInformationTo(filepath)
                Case Else
                    Throw New NotImplementedException(String.Format("File format ""{0}"" not implemented!", fileFormat.ToString))
            End Select
        End Sub

        Private Sub ExportCavityChecksInformationTo(filePath As String)
            Dim file As New CavityChecksFile(Me)
            With file
                .UpdateAll()
                .SaveTo(filePath)
            End With
        End Sub

        Public Enum ExportFileFormat
            Unknown
            CavityChecksInformation
        End Enum

        Private Sub _model_SelectionChanged(sender As Object, e As EventArgs) Handles _model.SelectionChanged
            OnModelSelectionChanged(Me, e)
        End Sub

        Private Sub _canvas_SelectionChanged(sender As Object, e As EventArgs) Handles _canvas.SelectionChanged
            OnCanvasSelectionChanged(sender, e)
        End Sub

        Protected Overridable Sub OnCanvasSelectionChanged(sender As Object, e As EventArgs)
            If Not _isModelSelecting Then
                _isCanvasSelecting = True
                Try
                    RaiseEvent CanvasSelectionChanged(sender, e)
                Finally
                    _isCanvasSelecting = False
                End Try
            End If
        End Sub

        Protected Friend Overridable Sub OnModelSelectionChanged(sender As Object, e As EventArgs)
            If Not _isCanvasSelecting Then
                _isModelSelecting = True
                Try
                    RaiseEvent ModelSelectionChanged(sender, e)
                Finally
                    _isModelSelecting = False
                End Try
            End If
        End Sub

        Friend Sub NotifyActiveModulesChanged(activeHarnessConfigurationId As String)
            OnActiveModulesChanged(Me, New ActiveModulesChangedEventArgs(activeHarnessConfigurationId))
        End Sub

        Protected Overridable Sub OnActiveModulesChanged(sender As Object, e As ActiveModulesChangedEventArgs)
            RaiseEvent ActiveModulesChanged(Me, e)
        End Sub

        Public Sub NotifyClosing()
            RaiseEvent Closing(Me, New EventArgs)
        End Sub

        Public Sub NotifyClosed()
            Active = False
            RaiseEvent Closed(Me, New EventArgs)
        End Sub

        Public Sub NotifyOpened(file As [Lib].IO.Files.Hcv.HcvFile, activeHarnessConfigurationId As String)
            If (file.CavityCheckSettings?.HasData).GetValueOrDefault Then
                Dim settingsLoadSuccess As Boolean = Model.TryLoadAndApplySettingsFrom(file.CavityCheckSettings)
                If Not settingsLoadSuccess Then
                    Model.Settings.Update()
                    Model.Settings.ActiveHarnessConfigurationId = activeHarnessConfigurationId
                End If
            End If
            Me.Active = True
        End Sub

        Public Function GetHarnessPartNumber() As String
            Return _kbl.HarnessPartNumber
        End Function

        Public Function GetHarnessVersion() As String
            Return _kbl.GetHarness.Version
        End Function

        Public Function GetHarnessProjectNumber() As String
            Return _kbl.GetHarness.Project_number
        End Function

        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not _disposedValue Then
                If disposing Then
                    If _canvas IsNot Nothing Then
                        _canvas.Dispose()
                    End If

                    If _model IsNot Nothing Then
                        _model.Dispose()
                    End If
                End If

                _model = Nothing
                _kbl = Nothing
                _canvas = Nothing
                _disposedValue = True
            End If
        End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            ' Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in der Methode "Dispose(disposing As Boolean)" ein.
            Dispose(disposing:=True)
            GC.SuppressFinalize(Me)
        End Sub

    End Class

End Namespace