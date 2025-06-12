Namespace Printing

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Class PrintForm
        Inherits System.Windows.Forms.Form

        'Das Formular überschreibt den Löschvorgang, um die Komponentenliste zu bereinigen.
        <System.Diagnostics.DebuggerNonUserCode()>
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            Try
                If disposing AndAlso components IsNot Nothing Then
                    components.Dispose()
                End If
            Finally
                MyBase.Dispose(disposing)
            End Try
        End Sub

        'Wird vom Windows Form-Designer benötigt.
        Private components As System.ComponentModel.IContainer

        'Hinweis: Die folgende Prozedur ist für den Windows Form-Designer erforderlich.
        'Das Bearbeiten ist mit dem Windows Form-Designer möglich.  
        'Das Bearbeiten mit dem Code-Editor ist nicht möglich.
        <System.Diagnostics.DebuggerStepThrough()>
        Private Sub InitializeComponent()
            Me.components = New System.ComponentModel.Container()
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(PrintForm))
            Dim UltraTab1 As Infragistics.Win.UltraWinTabControl.UltraTab = New Infragistics.Win.UltraWinTabControl.UltraTab()
            Dim UltraTab2 As Infragistics.Win.UltraWinTabControl.UltraTab = New Infragistics.Win.UltraWinTabControl.UltraTab()
            Dim UltraTab3 As Infragistics.Win.UltraWinTabControl.UltraTab = New Infragistics.Win.UltraWinTabControl.UltraTab()
            Dim UltraToolTipInfo1 As Infragistics.Win.UltraWinToolTip.UltraToolTipInfo = New Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Cancellation is currently not possible!", Infragistics.Win.ToolTipImage.[Error], "", Infragistics.Win.DefaultableBoolean.[Default])
            Me.DrawingTabControl = New Infragistics.Win.UltraWinTabControl.UltraTabPageControl()
            Me.DrawingPrintControl = New Zuken.E3.HarnessAnalyzer.Printing.PrintControl
            Me.SchematicsTabControl = New Infragistics.Win.UltraWinTabControl.UltraTabPageControl()
            Me.SchematicsPrintControl = New Zuken.E3.HarnessAnalyzer.Printing.PrintControl()
            Me.ThreeDTabControl = New Infragistics.Win.UltraWinTabControl.UltraTabPageControl()
            Me.ThreeDPrintControl = New Zuken.E3.HarnessAnalyzer.Printing.PrintControl()
            Me.UltraTabControl1 = New Infragistics.Win.UltraWinTabControl.UltraTabControl()
            Me.UltraTabSharedControlsPage1 = New Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage()
            ' Me.drawingsUserControl1 = New DrawingsControl(drawingsUserControl1.Model)
            Me.UltraToolTipManager1 = New Infragistics.Win.UltraWinToolTip.UltraToolTipManager(Me.components)
            Me.DrawingTabControl.SuspendLayout()
            Me.SchematicsTabControl.SuspendLayout()
            Me.ThreeDTabControl.SuspendLayout()

            CType(Me.UltraTabControl1, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.UltraTabControl1.SuspendLayout()
            Me.SuspendLayout()
            '
            'DrawingTabControl
            '
            Me.DrawingTabControl.Controls.Add(Me.DrawingPrintControl)
            resources.ApplyResources(Me.DrawingTabControl, "DrawingTabControl")
            Me.DrawingTabControl.Name = "DrawingTabControl"
            '
            'DrawingPrintControl
            '
            Me.DrawingPrintControl.CenterToPaper = False
            Me.DrawingPrintControl.CurrentPageHeight = Nothing
            Me.DrawingPrintControl.CurrentPageWidth = Nothing
            Me.DrawingPrintControl.CurrentPrinter = Nothing
            Me.DrawingPrintControl.CustomPaperSize = False
            resources.ApplyResources(Me.DrawingPrintControl, "DrawingPrintControl")
            Me.DrawingPrintControl.DocumentName = "document"
            Me.DrawingPrintControl.DrawingUnits = Nothing
            Me.DrawingPrintControl.Name = "DrawingPrintControl"
            Me.DrawingPrintControl.OnlyMargins = False
            Me.DrawingPrintControl.PrintAreaCheckedIndex = 0
            Me.DrawingPrintControl.Printers = Nothing
            Me.DrawingPrintControl.PrinterUnits = Nothing
            Me.DrawingPrintControl.ScaleToFit = False
            Me.DrawingPrintControl.Dock = DockStyle.Fill
            '
            'SchematicsTabControl
            '
            Me.SchematicsTabControl.Controls.Add(Me.SchematicsPrintControl)
            resources.ApplyResources(Me.SchematicsTabControl, "SchematicsTabControl")
            Me.SchematicsTabControl.Name = "SchematicsTabControl"
            '
            'SchematicsPrintControl
            '
            Me.SchematicsPrintControl.CenterToPaper = False
            Me.SchematicsPrintControl.CurrentPageHeight = Nothing
            Me.SchematicsPrintControl.CurrentPageWidth = Nothing
            Me.SchematicsPrintControl.CurrentPrinter = Nothing
            Me.SchematicsPrintControl.CustomPaperSize = False
            resources.ApplyResources(Me.SchematicsPrintControl, "SchematicsPrintControl")
            Me.SchematicsPrintControl.DocumentName = "document"
            Me.SchematicsPrintControl.DrawingUnits = Nothing
            Me.SchematicsPrintControl.Name = "SchematicsPrintControl"
            Me.SchematicsPrintControl.OnlyMargins = False
            Me.SchematicsPrintControl.PrintAreaCheckedIndex = 0
            Me.SchematicsPrintControl.Printers = Nothing
            Me.SchematicsPrintControl.PrinterUnits = Nothing
            Me.SchematicsPrintControl.ScaleToFit = False
            '
            'ThreeDTabControl
            '
            Me.ThreeDTabControl.Controls.Add(Me.ThreeDPrintControl)
            resources.ApplyResources(Me.ThreeDTabControl, "ThreeDTabControl")
            Me.ThreeDTabControl.Name = "ThreeDTabControl"
            '
            'ThreeDPrintControl
            '
            Me.ThreeDPrintControl.CenterToPaper = False
            Me.ThreeDPrintControl.CurrentPageHeight = Nothing
            Me.ThreeDPrintControl.CurrentPageWidth = Nothing
            Me.ThreeDPrintControl.CurrentPrinter = Nothing
            Me.ThreeDPrintControl.CustomPaperSize = False
            resources.ApplyResources(Me.ThreeDPrintControl, "ThreeDPrintControl")
            Me.ThreeDPrintControl.DocumentName = "document"
            Me.ThreeDPrintControl.DrawingUnits = Nothing
            Me.ThreeDPrintControl.Name = "ThreeDPrintControl"
            Me.ThreeDPrintControl.OnlyMargins = False
            Me.ThreeDPrintControl.PrintAreaCheckedIndex = 0
            Me.ThreeDPrintControl.Printers = Nothing
            Me.ThreeDPrintControl.PrinterUnits = Nothing
            Me.ThreeDPrintControl.ScaleToFit = False
            Me.ThreeDPrintControl.Dock = DockStyle.Fill

            '' 
            '' drawingsUserControl1
            '' 
            'Me.drawingsUserControl1.Dock = System.Windows.Forms.DockStyle.Fill
            'Me.drawingsUserControl1.Location = New System.Drawing.Point(3, 3)
            'Me.drawingsUserControl1.Model = Nothing
            'resources.ApplyResources(Me.drawingsUserControl1, "drawingsUserControl1")
            'Me.drawingsUserControl1.Name = "drawingsUserControl1"
            'Me.drawingsUserControl1.Size = New System.Drawing.Size(823, 582)
            'Me.drawingsUserControl1.TabIndex = 4

            '
            'UltraTabControl1
            '
            Me.UltraTabControl1.Controls.Add(Me.UltraTabSharedControlsPage1)
            Me.UltraTabControl1.Controls.Add(Me.DrawingTabControl)
            Me.UltraTabControl1.Controls.Add(Me.SchematicsTabControl)
            Me.UltraTabControl1.Controls.Add(Me.ThreeDTabControl)
            resources.ApplyResources(Me.UltraTabControl1, "UltraTabControl1")
            Me.UltraTabControl1.Name = "UltraTabControl1"
            Me.UltraTabControl1.SharedControlsPage = Me.UltraTabSharedControlsPage1
            Me.UltraTabControl1.TabOrientation = Infragistics.Win.UltraWinTabs.TabOrientation.LeftTop
            UltraTab1.Key = "Drawing"
            UltraTab1.TabPage = Me.DrawingTabControl
            resources.ApplyResources(UltraTab1, "UltraTab1")
            UltraTab1.ForceApplyResources = ""
            UltraTab2.Key = "Schematics"
            UltraTab2.TabPage = Me.SchematicsTabControl
            resources.ApplyResources(UltraTab2, "UltraTab2")
            UltraTab2.ForceApplyResources = ""
            UltraTab3.Key = "ThreeD"
            UltraTab3.TabPage = Me.ThreeDTabControl
            resources.ApplyResources(UltraTab3, "UltraTab3")
            UltraTab3.ForceApplyResources = ""
            Me.UltraTabControl1.Tabs.AddRange(New Infragistics.Win.UltraWinTabControl.UltraTab() {UltraTab1, UltraTab2, UltraTab3})
            Me.UltraTabControl1.TextOrientation = Infragistics.Win.UltraWinTabs.TextOrientation.Horizontal
            '
            'UltraTabSharedControlsPage1
            '
            resources.ApplyResources(Me.UltraTabSharedControlsPage1, "UltraTabSharedControlsPage1")
            Me.UltraTabSharedControlsPage1.Name = "UltraTabSharedControlsPage1"
            '
            'UltraToolTipManager1
            '
            Me.UltraToolTipManager1.ContainingControl = Me
            Me.UltraToolTipManager1.DisplayStyle = Infragistics.Win.ToolTipDisplayStyle.BalloonTip
            Me.UltraToolTipManager1.Enabled = False
            '
            'PrintForm
            '
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit
            resources.ApplyResources(Me, "$this")
            Me.Controls.Add(Me.UltraTabControl1)
            Me.Name = "PrintForm"
            Me.ShowInTaskbar = False
            UltraToolTipInfo1.ToolTipImage = Infragistics.Win.ToolTipImage.[Error]
            resources.ApplyResources(UltraToolTipInfo1, "UltraToolTipInfo1")
            Me.UltraToolTipManager1.SetUltraToolTip(Me, UltraToolTipInfo1)
            Me.DrawingTabControl.ResumeLayout(False)
            Me.SchematicsTabControl.ResumeLayout(False)
            Me.ThreeDTabControl.ResumeLayout(False)

            CType(Me.UltraTabControl1, System.ComponentModel.ISupportInitialize).EndInit()
            Me.UltraTabControl1.ResumeLayout(False)
            Me.ResumeLayout(False)

        End Sub
        ' Private drawingsUserControl1 As DrawingsControl
        Friend WithEvents UltraTabControl1 As Infragistics.Win.UltraWinTabControl.UltraTabControl
        Friend WithEvents UltraTabSharedControlsPage1 As Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage
        Friend WithEvents DrawingTabControl As Infragistics.Win.UltraWinTabControl.UltraTabPageControl
        Friend WithEvents SchematicsTabControl As Infragistics.Win.UltraWinTabControl.UltraTabPageControl
        Friend WithEvents ThreeDTabControl As Infragistics.Win.UltraWinTabControl.UltraTabPageControl
        Friend WithEvents DrawingPrintControl As Zuken.E3.HarnessAnalyzer.Printing.PrintControl
        Friend WithEvents SchematicsPrintControl As Zuken.E3.HarnessAnalyzer.Printing.PrintControl
        Friend WithEvents ThreeDPrintControl As Zuken.E3.HarnessAnalyzer.Printing.PrintControl
        Friend WithEvents UltraToolTipManager1 As Infragistics.Win.UltraWinToolTip.UltraToolTipManager
    End Class

End Namespace