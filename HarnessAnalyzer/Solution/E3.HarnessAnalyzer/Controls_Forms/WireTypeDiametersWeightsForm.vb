Imports Infragistics.Win
Imports Infragistics.Win.UltraWinGrid
Imports Zuken.E3.HarnessAnalyzer.Settings

<Reflection.Obfuscation(Feature:="renaming", ApplyToMembers:=False, Exclude:=True)> ' needed to avoid resource-file-names obfuscation errors, see issue #5
Public Class WireTypeDiametersWeightsForm

    Public Sub New(diameterSettings As DiameterSettings)
        InitializeComponent()

        Me.BackColor = Color.White
        Me.Icon = My.Resources.WireTypeDiameters
        Me.Text = WireTypeDiametersWeightsFormStrings.CaptionDiameters

        Me.ugWireTypeDiametersWeights.SyncWithCurrencyManager = False
        Me.ugWireTypeDiametersWeights.SetDataBinding(diameterSettings.Diameters, String.Empty, True, True)
    End Sub

    Public Sub New(weightSettings As WeightSettings)
        InitializeComponent()

        Me.BackColor = Color.White
        Me.Icon = My.Resources.WireTypeWeights
        Me.Text = WireTypeDiametersWeightsFormStrings.CaptionWeights

        Me.ugWireTypeDiametersWeights.SyncWithCurrencyManager = False
        Me.ugWireTypeDiametersWeights.SetDataBinding(weightSettings.Weights, String.Empty, True, True)
    End Sub


    Private Sub ugWireTypeDiametersWeights_InitializeLayout(sender As Object, e As InitializeLayoutEventArgs) Handles ugWireTypeDiametersWeights.InitializeLayout
        With e.Layout
            .AutoFitStyle = AutoFitStyle.ExtendLastColumn
            .GroupByBox.Hidden = True

            With .Override
                .AllowColMoving = AllowColMoving.NotAllowed
                .AllowDelete = DefaultableBoolean.False
                .AllowRowFiltering = DefaultableBoolean.True
                .ColumnAutoSizeMode = ColumnAutoSizeMode.VisibleRows
                .ColumnSizingArea = ColumnSizingArea.EntireColumn
                .RowSelectors = DefaultableBoolean.True
                .SelectTypeRow = SelectType.None
            End With

            For Each band As UltraGridBand In .Bands
                For Each column As UltraGridColumn In band.Columns
                    With column
                        .CellActivation = Activation.NoEdit
                        .CellAppearance.TextHAlign = HAlign.Center
                        .CellAppearance.TextVAlign = VAlign.Middle
                        .Header.Appearance.TextHAlign = HAlign.Center
                        .Header.Appearance.TextVAlign = VAlign.Middle

                        Select Case column.Key
                            Case "Conductor"
                                .Header.Caption = WireTypeDiametersWeightsFormStrings.ConductorWeight
                            Case "PartNumber"
                                .Header.Caption = WireTypeDiametersWeightsFormStrings.PartNumber
                            Case "Total"
                                .Header.Caption = WireTypeDiametersWeightsFormStrings.TotalWeight
                            Case "Value"
                                .Header.Caption = WireTypeDiametersWeightsFormStrings.Diameter
                            Case "WireType"
                                .Header.Caption = WireTypeDiametersWeightsFormStrings.WireType
                        End Select

                        .MinWidth = If(.Index = 0, 250, If(.Index = 1, 200, 125))
                        .PerformAutoResize()
                    End With
                Next
            Next
        End With
    End Sub

End Class