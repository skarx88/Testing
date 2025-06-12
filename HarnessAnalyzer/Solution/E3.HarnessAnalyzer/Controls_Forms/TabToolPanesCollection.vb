Imports Infragistics.Win.UltraWinDock
Imports Zuken.E3.HarnessAnalyzer.Shared.Common

Public Class TabToolPanesCollection
    Inherits ToolPanesCollection

    Public Sub New(udmMain As UltraDockManager)
        MyBase.New(udmMain)
    End Sub

    Public Overloads Function AddNew(tab As HomeTabToolKey, caption As String, control As System.Windows.Forms.Control, dock As DockedSide, size As System.Drawing.Size, Optional hide As Boolean = False) As DockableControlPane
        Return AddNewCore(tab.ToString, caption, control, dock, hide, size)
    End Function

    Public Overloads Function AddNew(tab As HomeTabToolKey, caption As String, control As System.Windows.Forms.Control, dock As DockedSide, Optional hide As Boolean = False) As DockableControlPane
        Return AddNewCore(tab.ToString, caption, control, dock, hide)
    End Function

    Public Overloads Function AddNew(Of T As {System.Windows.Forms.Control, New})(tab As HomeTabToolKey, caption As String, dock As DockedSide, Optional hide As Boolean = False) As DockableControlPane
        Return AddNewCore(tab.ToString, caption, Activator.CreateInstance(Of T), dock, hide)
    End Function

    Public Overloads Function AddNew(Of T As {System.Windows.Forms.Control, New})(tab As HomeTabToolKey, caption As String, dock As DockedSide, size As System.Drawing.Size, Optional hide As Boolean = False) As DockableControlPane
        Return AddNewCore(tab.ToString, caption, Activator.CreateInstance(Of T), dock, hide, size)
    End Function

    Public ReadOnly Property Consolidated3DPane As DockableControlPane
        Get
            Return GetHomePane(HomeTabToolKey.Display3DView)
        End Get
    End Property

    ReadOnly Property SchematicsPane As DockableControlPane
        Get
            Return GetHomePane(HomeTabToolKey.DisplaySchematicsView)
        End Get
    End Property

    ReadOnly Property TopologyHubPane As DockableControlPane
        Get
            Return GetCommonPane(PaneKeys.TopologyHub)
        End Get
    End Property

    Public ReadOnly Property DrawingsHubPane As DockableControlPane
        Get
            Return GetCommonPane(PaneKeys.DrawingsHub)
        End Get
    End Property

    Private Function GetHomePane(key As HomeTabToolKey) As DockableControlPane
        If Me.DockManager.ControlPanes.Exists(key.ToString) Then
            Return Me.DockManager.ControlPanes(key.ToString)
        End If
        Return Nothing
    End Function

End Class
