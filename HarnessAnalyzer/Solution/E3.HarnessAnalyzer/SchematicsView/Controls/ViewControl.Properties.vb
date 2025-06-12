Imports System.ComponentModel
Imports Zuken.E3.HarnessAnalyzer.Schematics.Converter
Imports Zuken.E3.HarnessAnalyzer.Schematics.Converter.Kbl

Namespace Schematics.Controls

    Partial Public Class ViewControl

        Private _autoZoomSelection As Boolean
        Private _syncButtonVisible As Boolean

        <DefaultValue(False)> Property AutoZoomSelection As Boolean
            Get
                Return _autoZoomSelection
            End Get
            Set(value As Boolean)
                _autoZoomSelection = value
                SyncButtonVisible = Not value
            End Set
        End Property

        <DefaultValue(True)> Property ProgressBarOnGenerate As Boolean = True

        Private ReadOnly Property CurrentConverterId As Guid
            Get
                Return _currentConverterId
            End Get
        End Property

        Private ReadOnly Property ModelIsAvail As Boolean
            Get
                Return _kblDocumentsData IsNot Nothing AndAlso _modelIsAttached
            End Get
        End Property

        ReadOnly Property Selected As SelectedEntities
            Get
                Return _selected
            End Get
        End Property

        ReadOnly Property KblBlocks As KblDocumentDataCollection
            Get
                Return _kblDocumentsData
            End Get
        End Property

        ReadOnly Property ActiveEntities As ActiveEntities
            Get
                Return _activeEntities
            End Get
        End Property

        ReadOnly Property Entities As DocumentsCollection
            Get
                Return _entities
            End Get
        End Property

        Private Property SyncButtonVisible As Boolean
            Get
                Return _syncButtonVisible
            End Get
            Set(value As Boolean)
                _syncButtonVisible = value
                If ModelIsAvail Then
                    btnSyncSelection.Visible = value
                End If
            End Set
        End Property

    End Class

End Namespace