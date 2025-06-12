Imports Infragistics.Win.UltraWinDock
Imports Zuken.E3.HarnessAnalyzer.Shared.Common

Public Class ToolPanesCollection
    Implements IDisposable

    Private _udmMain As UltraDockManager
    Private disposedValue As Boolean

    Public Sub New(udmMain As UltraDockManager)
        _udmMain = udmMain
    End Sub

    Public Function AddNew(key As PaneKeys, caption As String, control As System.Windows.Forms.Control, dock As DockedSide, Optional hide As Boolean = False) As DockableControlPane
        Return AddNewCore(key.ToString, caption, control, dock, hide)
    End Function

    Public Function AddNew(Of T As {System.Windows.Forms.Control, New})(key As PaneKeys, caption As String, dock As DockedSide, Optional hide As Boolean = False) As DockableControlPane
        Return AddNewCore(key.ToString, caption, Activator.CreateInstance(Of T), dock, hide)
    End Function

    Public Function AddNew(Of T As {System.Windows.Forms.Control, New})(key As PaneKeys, caption As String, dock As DockedSide, size As System.Drawing.Size, Optional hide As Boolean = False) As DockableControlPane
        Return AddNewCore(key.ToString, caption, Activator.CreateInstance(Of T), dock, hide, size)
    End Function

    Protected Function AddNewCore(key As String, caption As String, control As System.Windows.Forms.Control, dock As DockedSide, Optional hide As Boolean = False, Optional size As Nullable(Of Size) = Nothing) As DockableControlPane
        Dim pane As New DockableControlPane(key, caption)
        control.Dock = DockStyle.Fill
        pane.Control = control
        If size.HasValue Then
            pane.Size = size.Value
        Else
            pane.Size = control.Size
        End If

        _udmMain.ControlPanes.Add(pane)

        If hide Then
            pane.Dock(DockedSide.Bottom) ' HINT: to create a dockarea pane
            pane.Close()
        End If
        Return pane
    End Function

    Protected Function GetCommonPane(key As PaneKeys) As DockableControlPane
        If _udmMain.ControlPanes.Exists(key.ToString) Then
            Return _udmMain.ControlPanes(key.ToString)
        End If
        Return Nothing
    End Function

    ReadOnly Property DockManager As UltraDockManager
        Get
            Return _udmMain
        End Get
    End Property

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: Verwalteten Zustand (verwaltete Objekte) bereinigen
            End If

            _udmMain = Nothing
            disposedValue = True
        End If
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        ' Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in der Methode "Dispose(disposing As Boolean)" ein.
        Dispose(disposing:=True)
        GC.SuppressFinalize(Me)
    End Sub
End Class
