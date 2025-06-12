Namespace Printing

    Partial Public Class PrintControl

        Private Class SettingsState

            Public Sub New(control As PrintControl)
                With control
                    _ViewsGroupEnabled = .ugbViews.Enabled
                    _PrintAreaGroupEnabled = .ugbPrintArea.Enabled
                    _MarginsGroupEnabled = .ugbMargins.Enabled
                    _OrientationGroupEnabled = .ugbOrientation.Enabled
                    _PaperGroupEnabled = .ugbOrientation.Enabled
                    _ScaleGroupEnabled = .ugbScale.Enabled
                    _NOfCopiesEnabled = .nudNumberOfCopies.Enabled
                    _UnitsEnabled = .uosUnits.Enabled
                    _PartialEnabled = .uckOnlyMargins.Enabled
                    _BtnPickEnabled = .btnPick.Enabled
                    _BtnPrintEnabled = .btnPrint.Enabled
                    _BtnPrinterSettingsEnabled = .btnPrinterSettings.Enabled
                    _BtnUpdatePreviewEnabled = .btnUpdatePreview.Enabled
                End With
            End Sub

            ReadOnly Property ViewsGroupEnabled As Boolean
            ReadOnly Property MarginsGroupEnabled As Boolean
            ReadOnly Property OrientationGroupEnabled As Boolean
            ReadOnly Property PaperGroupEnabled As Boolean
            ReadOnly Property ScaleGroupEnabled As Boolean
            ReadOnly Property PrintAreaGroupEnabled As Boolean
            ReadOnly Property NOfCopiesEnabled As Boolean
            ReadOnly Property UnitsEnabled As Boolean
            ReadOnly Property PartialEnabled As Boolean
            ReadOnly Property BtnPickEnabled As Boolean
            ReadOnly Property BtnPrintEnabled As Boolean
            ReadOnly Property BtnPrinterSettingsEnabled As Boolean
            ReadOnly Property BtnUpdatePreviewEnabled As Boolean

            Public Sub [Set](control As PrintControl)
                SettingsState.SetControl(control, Me)
            End Sub

            Public Shared Sub SetEnabled(control As PrintControl, enabled As Boolean)
                With control
                    .ugbViews.Enabled = enabled
                    .ugbPrintArea.Enabled = enabled
                    .ugbMargins.Enabled = enabled
                    .ugbOrientation.Enabled = enabled
                    .ugbOrientation.Enabled = enabled
                    .ugbScale.Enabled = enabled
                    .ugbPrintArea.Enabled = enabled
                    .nudNumberOfCopies.Enabled = enabled
                    .uosUnits.Enabled = enabled
                    .uckOnlyMargins.Enabled = enabled
                    .btnPick.Enabled = enabled
                    .btnPrint.Enabled = enabled
                    .btnPrinterSettings.Enabled = enabled
                    .btnUpdatePreview.Enabled = enabled

                End With
            End Sub

            Public Shared Sub SetControl(control As PrintControl, state As SettingsState)
                With control
                    .ugbViews.Enabled = state.ViewsGroupEnabled
                    .ugbPrintArea.Enabled = state.PrintAreaGroupEnabled
                    .ugbMargins.Enabled = state.MarginsGroupEnabled
                    .ugbOrientation.Enabled = state.OrientationGroupEnabled
                    .ugbPaper.Enabled = state.PaperGroupEnabled
                    .ugbScale.Enabled = state.ScaleGroupEnabled
                    .ugbScale.Enabled = state.PrintAreaGroupEnabled
                    .nudNumberOfCopies.Enabled = state.NOfCopiesEnabled
                    .uosUnits.Enabled = state.UnitsEnabled
                    .uckOnlyMargins.Enabled = state.PartialEnabled
                    .btnPick.Enabled = state.BtnPickEnabled
                    .btnPrint.Enabled = state.BtnPrintEnabled
                    .btnPrinterSettings.Enabled = state.BtnPrinterSettingsEnabled
                    .btnUpdatePreview.Enabled = state.BtnUpdatePreviewEnabled
                End With
            End Sub
        End Class

    End Class

End Namespace