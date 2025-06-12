Imports System.Collections.Specialized
Imports System.ComponentModel
Imports System.IO
Imports Zuken.E3.HarnessAnalyzer.Checks.Cavities.Views.Document


Namespace Checks.Cavities.Views.Model

    <Reflection.Obfuscation(Feature:="properties renaming")> ' for DataSource binding
    Public Class ModelView
        Implements IDisposable

        Public Event ConnectorsVisibilityChanged(sender As Object, e As EventArgs)
        Public Event CavitiesVisibilityChanged(sender As Object, e As EventArgs)
        Public Event Changed(sender As Object, e As EventArgs)
        Public Event SettingsUpdated(sender As Object, e As EventArgs)
        Public Event SelectionChanged(sender As Object, e As EventArgs)

        Private WithEvents _selectedViewObjectsCollection As New SelectedViewObjectsCollection
        Private WithEvents _settings As New Settings.CavityCheckSettings(Me)
        Private WithEvents _connectors As New BindingList(Of ConnectorView)
        Private _kbl As KblMapper
        Private _disposedValue As Boolean

        Friend Sub New(kbl As KblMapper)
            _kbl = kbl
        End Sub

        Property Document As DocumentView

        ReadOnly Property Connectors As BindingList(Of ConnectorView)
            Get
                Return _connectors
            End Get
        End Property

        Friend Sub Update()
            Dim conns As New BindingList(Of ConnectorView)

            Dim connToCavs As New Concurrent.ConcurrentDictionary(Of String, List(Of Cavity_occurrence))
            For Each kv As KeyValuePair(Of String, String) In _kbl.KBLCavityConnectorMapper
                Dim cav As Cavity_occurrence = _kbl.GetOccurrenceObject(Of Cavity_occurrence)(kv.Key)
                Dim lst As List(Of Cavity_occurrence) = connToCavs.GetOrAdd(kv.Value, Function() New List(Of Cavity_occurrence))
                lst.Add(cav)
            Next

            Dim cavViews As New Concurrent.ConcurrentDictionary(Of String, List(Of CavityWireView))
            For Each kv As KeyValuePair(Of String, Dictionary(Of String, List(Of Cavity_occurrence))) In _kbl.KBLWireCavityMapper
                For Each wireCv As KeyValuePair(Of String, List(Of Cavity_occurrence)) In kv.Value
                    For Each cav As Cavity_occurrence In wireCv.Value
                        Dim lst As List(Of CavityWireView) = cavViews.GetOrAdd(cav.SystemId, Function() New List(Of CavityWireView))
                        Dim wireCoreOcc As IKblWireCoreOccurrence = _kbl.GetWireOrCoreOccurrence(wireCv.Key)
                        Dim newCavWireView As CavityWireView

                        If TypeOf wireCoreOcc Is Wire_occurrence Then
                            newCavWireView = New CavityWireView(CType(wireCoreOcc, Wire_occurrence), cav, _kbl, My.Application.MainForm.GeneralSettings)
                        Else
                            newCavWireView = New CavityWireView(CType(wireCoreOcc, Core_occurrence), cav, _kbl, My.Application.MainForm.GeneralSettings)
                        End If

                        lst.Add(newCavWireView)
                        newCavWireView.Model = Me
                    Next
                Next
            Next

            For Each conn As Connector_occurrence In _kbl.GetConnectorOccurrences
                Dim connHousing As Connector_Housing = CType(_kbl.KBLPartMapper(conn.Part), Connector_Housing)
                Dim cavs As List(Of Cavity_occurrence) = connToCavs.GetOrAdd(conn.SystemId, Function() New List(Of Cavity_occurrence))
                Dim cViewList As New List(Of CavityWireView)

                For Each cv As Cavity_occurrence In cavs
                    If cavViews.ContainsKey(cv.SystemId) Then
                        cViewList.AddRange(cavViews(cv.SystemId))
                    Else
                        'HINT: Add dummy wire to make checking for empty cavities possible!
                        cViewList.Add(New VirtualCavityWireView(cv, _kbl, My.Application.MainForm.GeneralSettings))
                    End If
                Next

                Dim connView As New ConnectorView(conn, cViewList, My.Application.MainForm.GeneralSettings)
                connView.IsKSL = connHousing.IsKSL
                connView.Model = Me
                conns.Add(connView)
            Next

            If Me.Connectors IsNot Nothing Then
                For Each c As ConnectorView In Me.Connectors
                    c.Model = Nothing
                Next
            End If

            Me._connectors = conns
        End Sub

        Friend Sub Invalidate()
            UpdateModelVisibility()
        End Sub

        Public Function AnyChanged() As Boolean
            Return Connectors.SelectMany(Function(c) c.CavWires).Any(Function(cavWire) cavWire.CheckState <> CheckState.Indeterminate)
        End Function

        ReadOnly Property IsSelecting As Boolean

        Private Sub UpdateModelVisibility()
            Dim changedConnectors As Boolean
            Dim changedCavities As Boolean

            For Each conn As ConnectorView In Me.Connectors

                Dim hasConnActiveWires As Boolean = False
                For Each cv As CavityWireView In conn.CavWires
                    If _kbl.KBLObjectModuleMapper.ContainsKey(cv.KblWireId) Then
                        Dim modIds As List(Of String) = _kbl.KBLObjectModuleMapper(cv.KblWireId).ToList
                        If modIds.Count = 0 OrElse modIds.Except(_kbl.InactiveModules.Keys).Any() Then
                            hasConnActiveWires = True
                            If cv.Readonly Then
                                cv.Readonly = False
                                changedCavities = True
                            End If
                        Else
                            If Not cv.Readonly Then
                                cv.Readonly = True
                                changedCavities = True
                            End If
                        End If
                    End If
                Next


                If _kbl.KBLObjectModuleMapper.ContainsKey(conn.KblId) Then
                    Dim modIds As List(Of String) = _kbl.KBLObjectModuleMapper(conn.KblId).ToList
                    If modIds.Count = 0 OrElse hasConnActiveWires OrElse modIds.Except(_kbl.InactiveModules.Keys).Any() Then
                        If Not conn.Visible Then
                            conn.Visible = True
                            changedConnectors = True
                        End If
                    Else
                        If conn.Visible Then
                            conn.Visible = False
                            changedConnectors = True
                        End If
                    End If
                End If
            Next

            If changedConnectors Then
                OnConnectorsVisibilityChanged(Me, New EventArgs)
            End If

            If changedCavities Then
                OnCavitiesVisibilityChanged(Me, New EventArgs)
            End If
        End Sub

        ReadOnly Property Settings As Settings.CavityCheckSettings
            Get
                Return _settings
            End Get
        End Property

        Public Function TryLoadAndApplySettingsFrom(settings As [Lib].IO.Files.Hcv.CavityCheckSettingsContainerFile) As Boolean
            Try
                Using s As Stream = settings.GetDataAsStream
                    _settings = Cavities.Settings.CavityCheckSettings.Load(s)
                End Using

                _settings.Model = Me
                If _settings IsNot Nothing Then
                    'HINT: deactivate Events to block raise Model.Changed - event which will set document to dirty what we don't want to have on loading settings
                    Using Me.ProtectProperty(NameOf(EventsEnabled), False)
                        _settings.ApplyToModel()
                    End Using
                End If
                Return True
            Catch ex As IOException
                Return False
            End Try
            Return False
        End Function

        Private Sub _connectors_ListChanged(sender As Object, e As ListChangedEventArgs) Handles _connectors.ListChanged
            If e.ListChangedType = ListChangedType.ItemChanged Then
                OnChanged(Me, New EventArgs)
            End If
        End Sub

        Protected Overridable Sub OnChanged(sender As Object, e As EventArgs)
            If EventsEnabled Then
                RaiseEvent Changed(sender, e)
            End If
        End Sub

        ''' <summary>
        ''' Resolves the current selected inactive modules (from kblMapper)
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property CurrentInactiveModules As Dictionary(Of String, [Module])
            Get
                Return _kbl?.InactiveModules
            End Get
        End Property

        Public Function GetModules() As IEnumerable(Of [Module])
            Return _kbl.GetModules
        End Function

        Protected Overridable Sub OnConnectorsVisibilityChanged(sender As Object, e As EventArgs)
            If EventsEnabled Then
                RaiseEvent ConnectorsVisibilityChanged(sender, e)
            End If
        End Sub

        Protected Overridable Sub OnCavitiesVisibilityChanged(sender As Object, e As EventArgs)
            If EventsEnabled Then
                RaiseEvent CavitiesVisibilityChanged(sender, e)
            End If
        End Sub

        Property EventsEnabled As Boolean = True

        ReadOnly Property Selected As SelectedViewObjectsCollection
            Get
                Return _selectedViewObjectsCollection
            End Get
        End Property

        Overridable Sub OnSelectionChanged(sender As Object, e As EventArgs)
            If EventsEnabled Then
                RaiseEvent SelectionChanged(sender, e)
            End If
        End Sub

        Private Sub _selectedViewObjectsCollection_CollectionChanged(sender As Object, e As NotifyCollectionChangedEventArgs) Handles _selectedViewObjectsCollection.CollectionChanged
            If Not IsSelecting Then
                Try
                    _IsSelecting = True
                    If e.Action <> NotifyCollectionChangedAction.Move Then
                        OnSelectionChanged(Me, New EventArgs)
                    End If
                Finally
                    _IsSelecting = False
                End Try
            End If
        End Sub

        Public Function TryGetFirstConnectorAndCavityViewsFromKbl(kblIds As IEnumerable(Of String)) As IEnumerable(Of BaseView)
            'TODO: this is a cumbersome solution and should use indexed ways!
            Dim objectHash As New HashSet(Of String)(kblIds.Distinct)
            Dim views As New HashSet(Of Checks.Cavities.Views.BaseView)
            If objectHash.Count > 0 Then


                For Each conn As Checks.Cavities.Views.Model.ConnectorView In Connectors
                    Dim connectorAdded As Boolean = False
                    Dim cavitiesAdded As Boolean = False
                    If objectHash.Contains(conn.KblId) Then
                        connectorAdded = views.Add(conn)
                    End If

                    Dim potentialCavities As New List(Of Checks.Cavities.Views.Model.CavityWireView)
                    For Each cav As CavityWireView In conn.CavWires
                        If objectHash.Contains(cav.KblCavityId) Then
                            If Not connectorAdded Then
                                connectorAdded = views.Add(conn)
                            End If

                            'HINT find the right view row even if we have multiple wires in the cavity
                            If Not String.IsNullOrEmpty(cav.KblWireId) AndAlso objectHash.Contains(cav.KblWireId) Then
                                If views.Add(cav) Then
                                    cavitiesAdded = True
                                End If
                            ElseIf (Not String.IsNullOrEmpty(cav.KblContactPointId) AndAlso objectHash.Contains(cav.KblContactPointId)) Then
                                If views.Add(cav) Then
                                    cavitiesAdded = True
                                End If
                            Else
                                potentialCavities.Add(cav)
                            End If

                        End If
                    Next

                    If (potentialCavities.Count > 0 AndAlso Not cavitiesAdded) Then
                        For Each cav As Checks.Cavities.Views.Model.CavityWireView In potentialCavities.Distinct
                            If views.Add(cav) Then cavitiesAdded = True
                        Next
                    End If

                    If cavitiesAdded AndAlso connectorAdded Then
                        Exit For
                    End If
                Next
            End If

            Return views
        End Function

        Protected Overridable Sub OnSettingsUpdated(sender As Object, e As EventArgs)
            RaiseEvent SettingsUpdated(sender, e)
        End Sub

        Private Sub _settings_Updated(sender As Object, e As EventArgs) Handles _settings.Updated
            OnSettingsUpdated(Me, e)
        End Sub

        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not _disposedValue Then
                If disposing Then
                End If

                _kbl = Nothing
                _connectors = Nothing
                _settings = Nothing
                _selectedViewObjectsCollection = Nothing
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