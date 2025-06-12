<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class InformationHub
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
    ' <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing Then
                'HINT: prevent the call of SelectionChange in Grids after Disposing

                If _grids IsNot Nothing Then
                    ClearSelectedRowsInGrids()

                    For Each grid As Infragistics.Win.UltraWinGrid.UltraGrid In _grids.Values
                        grid.Dispose()
                    Next

                    If (Me.utchpInformationHub?.Enabled) Then
                        For Each grid As Infragistics.Win.UltraWinGrid.UltraGrid In _grids.Values
                            RemoveHandler grid.GestureQueryStatus, AddressOf OnGestureQueryStatus
                            grid.Dispose()
                        Next
                    End If
                End If

                _rowFilters?.Dispose()
                _currentRowFilterInfo?.Dispose()
                _kblIdRowCache?.Dispose()
                components?.Dispose()
            End If

            _currentRowFilterInfo = Nothing
            _kblMapper = Nothing
            _rowFilters = Nothing
            _grids = Nothing
            _parentForm = Nothing
            _generalSettings = Nothing
            _qmStampSpecifications = Nothing
            _diameterSettings = Nothing
            DisposableObject.Dispose(Me) ' HINT: generic dispose to ensure all fields/properties are set to nothing -> does not cover the IDisposable-interfaces, if needed you have to do that manually
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        components = New ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(InformationHub))
        Dim Appearance1 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance2 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance3 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance4 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance5 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance6 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance7 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance8 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance9 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance10 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance11 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance12 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance13 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance14 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance15 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance16 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance17 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance18 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance19 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance20 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance21 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance22 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance23 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance24 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance25 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance26 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance27 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance28 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance29 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance30 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance31 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance32 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance33 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance34 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance35 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance36 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance37 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance38 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance39 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance40 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance41 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance42 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance43 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance44 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance45 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance46 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance47 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance48 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance49 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance50 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance51 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance52 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance53 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance54 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance55 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance56 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance57 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance58 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance59 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance60 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance61 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance62 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance63 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance64 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance65 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance66 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance67 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance68 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance69 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance70 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance71 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance72 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance73 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance74 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance75 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance76 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance77 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance78 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance79 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance80 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance81 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance82 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance83 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance84 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance85 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance86 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance87 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance88 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance89 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance90 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance91 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance92 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance93 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance94 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance95 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance96 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance97 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance98 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance99 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance100 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance101 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance102 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance103 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance104 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance105 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance106 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance107 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance108 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance109 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance110 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance111 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance112 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance113 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance114 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance115 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance116 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance117 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance118 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance119 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance120 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance121 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance122 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance123 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance124 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance125 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance126 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance127 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance128 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance129 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance130 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance131 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance132 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance133 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance134 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance135 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance136 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance137 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance138 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance139 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance140 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance141 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance142 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance143 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance144 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance145 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance146 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance147 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance148 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance149 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance150 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance151 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance152 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance153 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance154 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance155 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance156 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance157 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance158 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance159 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance160 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance161 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance162 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance163 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance164 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance165 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance166 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance167 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance168 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance169 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance170 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance171 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance172 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance173 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance174 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance175 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance176 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance177 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance178 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance179 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance180 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance181 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance182 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance183 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance184 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance185 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance186 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance187 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance188 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance189 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance190 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance191 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance192 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance193 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance194 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance195 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance196 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance197 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance198 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance199 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance200 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance201 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance202 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance203 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance204 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance205 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance206 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance207 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance208 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance209 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance210 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance211 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance212 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance213 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance214 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance215 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance216 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance217 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance218 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance219 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance220 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance221 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance222 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance223 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance224 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance225 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance226 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance227 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance228 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance229 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance230 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance231 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance232 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance233 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance234 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance235 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance236 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance237 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance238 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim Appearance239 As Infragistics.Win.Appearance = New Infragistics.Win.Appearance()
        Dim UltraGridBand1 As Infragistics.Win.UltraWinGrid.UltraGridBand = New Infragistics.Win.UltraWinGrid.UltraGridBand("ColumnView", -1)
        Dim UltraGridColumn1 As Infragistics.Win.UltraWinGrid.UltraGridColumn = New Infragistics.Win.UltraWinGrid.UltraGridColumn("ColumnName")
        Dim UltraGridColumn2 As Infragistics.Win.UltraWinGrid.UltraGridColumn = New Infragistics.Win.UltraWinGrid.UltraGridColumn("Properties")
        Dim UltraGridBand2 As Infragistics.Win.UltraWinGrid.UltraGridBand = New Infragistics.Win.UltraWinGrid.UltraGridBand("Properties", 0)
        Dim UltraGridColumn3 As Infragistics.Win.UltraWinGrid.UltraGridColumn = New Infragistics.Win.UltraWinGrid.UltraGridColumn("Name")
        Dim UltraGridColumn4 As Infragistics.Win.UltraWinGrid.UltraGridColumn = New Infragistics.Win.UltraWinGrid.UltraGridColumn("Current")
        Dim UltraGridColumn5 As Infragistics.Win.UltraWinGrid.UltraGridColumn = New Infragistics.Win.UltraWinGrid.UltraGridColumn("Default")
        Dim UltraTab1 As Infragistics.Win.UltraWinTabControl.UltraTab = New Infragistics.Win.UltraWinTabControl.UltraTab()
        Dim UltraTab2 As Infragistics.Win.UltraWinTabControl.UltraTab = New Infragistics.Win.UltraWinTabControl.UltraTab()
        Dim UltraTab3 As Infragistics.Win.UltraWinTabControl.UltraTab = New Infragistics.Win.UltraWinTabControl.UltraTab()
        Dim UltraTab4 As Infragistics.Win.UltraWinTabControl.UltraTab = New Infragistics.Win.UltraWinTabControl.UltraTab()
        Dim UltraTab5 As Infragistics.Win.UltraWinTabControl.UltraTab = New Infragistics.Win.UltraWinTabControl.UltraTab()
        Dim UltraTab14 As Infragistics.Win.UltraWinTabControl.UltraTab = New Infragistics.Win.UltraWinTabControl.UltraTab()
        Dim UltraTab6 As Infragistics.Win.UltraWinTabControl.UltraTab = New Infragistics.Win.UltraWinTabControl.UltraTab()
        Dim UltraTab16 As Infragistics.Win.UltraWinTabControl.UltraTab = New Infragistics.Win.UltraWinTabControl.UltraTab()
        Dim UltraTab7 As Infragistics.Win.UltraWinTabControl.UltraTab = New Infragistics.Win.UltraWinTabControl.UltraTab()
        Dim UltraTab8 As Infragistics.Win.UltraWinTabControl.UltraTab = New Infragistics.Win.UltraWinTabControl.UltraTab()
        Dim UltraTab9 As Infragistics.Win.UltraWinTabControl.UltraTab = New Infragistics.Win.UltraWinTabControl.UltraTab()
        Dim UltraTab10 As Infragistics.Win.UltraWinTabControl.UltraTab = New Infragistics.Win.UltraWinTabControl.UltraTab()
        Dim UltraTab11 As Infragistics.Win.UltraWinTabControl.UltraTab = New Infragistics.Win.UltraWinTabControl.UltraTab()
        Dim UltraTab17 As Infragistics.Win.UltraWinTabControl.UltraTab = New Infragistics.Win.UltraWinTabControl.UltraTab()
        Dim UltraTab13 As Infragistics.Win.UltraWinTabControl.UltraTab = New Infragistics.Win.UltraWinTabControl.UltraTab()
        Dim UltraTab15 As Infragistics.Win.UltraWinTabControl.UltraTab = New Infragistics.Win.UltraWinTabControl.UltraTab()
        Dim UltraTab20 As Infragistics.Win.UltraWinTabControl.UltraTab = New Infragistics.Win.UltraWinTabControl.UltraTab()
        Dim UltraTab21 As Infragistics.Win.UltraWinTabControl.UltraTab = New Infragistics.Win.UltraWinTabControl.UltraTab()
        Dim UltraTab12 As Infragistics.Win.UltraWinTabControl.UltraTab = New Infragistics.Win.UltraWinTabControl.UltraTab()
        Dim UltraTab18 As Infragistics.Win.UltraWinTabControl.UltraTab = New Infragistics.Win.UltraWinTabControl.UltraTab()
        Dim UltraTab19 As Infragistics.Win.UltraWinTabControl.UltraTab = New Infragistics.Win.UltraWinTabControl.UltraTab()
        utpcHarness = New Infragistics.Win.UltraWinTabControl.UltraTabPageControl()
        ugHarness = New Infragistics.Win.UltraWinGrid.UltraGrid()
        utpcModules = New Infragistics.Win.UltraWinTabControl.UltraTabPageControl()
        ugModules = New Infragistics.Win.UltraWinGrid.UltraGrid()
        utpcVertices = New Infragistics.Win.UltraWinTabControl.UltraTabPageControl()
        ugVertices = New Infragistics.Win.UltraWinGrid.UltraGrid()
        utpcSegments = New Infragistics.Win.UltraWinTabControl.UltraTabPageControl()
        ugSegments = New Infragistics.Win.UltraWinGrid.UltraGrid()
        utpcAccessories = New Infragistics.Win.UltraWinTabControl.UltraTabPageControl()
        ugAccessories = New Infragistics.Win.UltraWinGrid.UltraGrid()
        utpcAssemblyParts = New Infragistics.Win.UltraWinTabControl.UltraTabPageControl()
        ugAssemblyParts = New Infragistics.Win.UltraWinGrid.UltraGrid()
        utpcFixings = New Infragistics.Win.UltraWinTabControl.UltraTabPageControl()
        ugFixings = New Infragistics.Win.UltraWinGrid.UltraGrid()
        utpcComponentBoxes = New Infragistics.Win.UltraWinTabControl.UltraTabPageControl()
        ugComponentBoxes = New Infragistics.Win.UltraWinGrid.UltraGrid()
        utpcComponents = New Infragistics.Win.UltraWinTabControl.UltraTabPageControl()
        ugComponents = New Infragistics.Win.UltraWinGrid.UltraGrid()
        utpcConnectors = New Infragistics.Win.UltraWinTabControl.UltraTabPageControl()
        ugConnectors = New Infragistics.Win.UltraWinGrid.UltraGrid()
        utpcCables = New Infragistics.Win.UltraWinTabControl.UltraTabPageControl()
        ugCables = New Infragistics.Win.UltraWinGrid.UltraGrid()
        utpcWires = New Infragistics.Win.UltraWinTabControl.UltraTabPageControl()
        ugWires = New Infragistics.Win.UltraWinGrid.UltraGrid()
        utpcNets = New Infragistics.Win.UltraWinTabControl.UltraTabPageControl()
        ugNets = New Infragistics.Win.UltraWinGrid.UltraGrid()
        utpcApprovals = New Infragistics.Win.UltraWinTabControl.UltraTabPageControl()
        ugApprovals = New Infragistics.Win.UltraWinGrid.UltraGrid()
        utpcChangeDescriptions = New Infragistics.Win.UltraWinTabControl.UltraTabPageControl()
        ugChangeDescriptions = New Infragistics.Win.UltraWinGrid.UltraGrid()
        utpcCoPacks = New Infragistics.Win.UltraWinTabControl.UltraTabPageControl()
        ugCoPacks = New Infragistics.Win.UltraWinGrid.UltraGrid()
        utpcDefDimSpecs = New Infragistics.Win.UltraWinTabControl.UltraTabPageControl()
        ugDefDimSpecs = New Infragistics.Win.UltraWinGrid.UltraGrid()
        utpcDimSpecs = New Infragistics.Win.UltraWinTabControl.UltraTabPageControl()
        ugDimSpecs = New Infragistics.Win.UltraWinGrid.UltraGrid()
        utpcRedlinings = New Infragistics.Win.UltraWinTabControl.UltraTabPageControl()
        ugRedlinings = New Infragistics.Win.UltraWinGrid.UltraGrid()
        utpcQMStamps = New Infragistics.Win.UltraWinTabControl.UltraTabPageControl()
        ugQMStamps = New Infragistics.Win.UltraWinGrid.UltraGrid()
        utpcDifferences = New Infragistics.Win.UltraWinTabControl.UltraTabPageControl()
        ugDifferences = New Infragistics.Win.UltraWinGrid.UltraGrid()
        ColumnViewBindingSource = New BindingSource(components)
        utcInformationHub = New Infragistics.Win.UltraWinTabControl.UltraTabControl()
        utscpInformationHub = New Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage()
        udsHarness = New Infragistics.Win.UltraWinDataSource.UltraDataSource(components)
        udsModules = New Infragistics.Win.UltraWinDataSource.UltraDataSource(components)
        udsVertices = New Infragistics.Win.UltraWinDataSource.UltraDataSource(components)
        udsSegments = New Infragistics.Win.UltraWinDataSource.UltraDataSource(components)
        udsAccessories = New Infragistics.Win.UltraWinDataSource.UltraDataSource(components)
        udsFixings = New Infragistics.Win.UltraWinDataSource.UltraDataSource(components)
        udsComponents = New Infragistics.Win.UltraWinDataSource.UltraDataSource(components)
        udsConnectors = New Infragistics.Win.UltraWinDataSource.UltraDataSource(components)
        udsCables = New Infragistics.Win.UltraWinDataSource.UltraDataSource(components)
        udsWires = New Infragistics.Win.UltraWinDataSource.UltraDataSource(components)
        udsNets = New Infragistics.Win.UltraWinDataSource.UltraDataSource(components)
        ugeeInformationHub = New Infragistics.Win.UltraWinGrid.ExcelExport.UltraGridExcelExporter(components)
        utmInformationHub = New Infragistics.Win.UltraWinToolbars.UltraToolbarsManager(components)
        _InformationHub_Toolbars_Dock_Area_Left = New Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea()
        _InformationHub_Toolbars_Dock_Area_Right = New Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea()
        _InformationHub_Toolbars_Dock_Area_Top = New Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea()
        _InformationHub_Toolbars_Dock_Area_Bottom = New Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea()
        udsRedlinings = New Infragistics.Win.UltraWinDataSource.UltraDataSource(components)
        timerE3Application = New Timer(components)
        udsChangeDescriptions = New Infragistics.Win.UltraWinDataSource.UltraDataSource(components)
        udsAssemblyParts = New Infragistics.Win.UltraWinDataSource.UltraDataSource(components)
        udsCoPacks = New Infragistics.Win.UltraWinDataSource.UltraDataSource(components)
        udsComponentBoxes = New Infragistics.Win.UltraWinDataSource.UltraDataSource(components)
        udsApprovals = New Infragistics.Win.UltraWinDataSource.UltraDataSource(components)
        utchpInformationHub = New Infragistics.Win.Touch.UltraTouchProvider(components)
        udsQMStamps = New Infragistics.Win.UltraWinDataSource.UltraDataSource(components)
        ugePropDifferences = New Infragistics.Win.UltraWinGrid.ExcelExport.UltraGridExcelExporter(components)
        udsDefDimSpecs = New Infragistics.Win.UltraWinDataSource.UltraDataSource(components)
        udsDimSpecs = New Infragistics.Win.UltraWinDataSource.UltraDataSource(components)
        utpcHarness.SuspendLayout()
        CType(ugHarness, ComponentModel.ISupportInitialize).BeginInit()
        utpcModules.SuspendLayout()
        CType(ugModules, ComponentModel.ISupportInitialize).BeginInit()
        utpcVertices.SuspendLayout()
        CType(ugVertices, ComponentModel.ISupportInitialize).BeginInit()
        utpcSegments.SuspendLayout()
        CType(ugSegments, ComponentModel.ISupportInitialize).BeginInit()
        utpcAccessories.SuspendLayout()
        CType(ugAccessories, ComponentModel.ISupportInitialize).BeginInit()
        utpcAssemblyParts.SuspendLayout()
        CType(ugAssemblyParts, ComponentModel.ISupportInitialize).BeginInit()
        utpcFixings.SuspendLayout()
        CType(ugFixings, ComponentModel.ISupportInitialize).BeginInit()
        utpcComponentBoxes.SuspendLayout()
        CType(ugComponentBoxes, ComponentModel.ISupportInitialize).BeginInit()
        utpcComponents.SuspendLayout()
        CType(ugComponents, ComponentModel.ISupportInitialize).BeginInit()
        utpcConnectors.SuspendLayout()
        CType(ugConnectors, ComponentModel.ISupportInitialize).BeginInit()
        utpcCables.SuspendLayout()
        CType(ugCables, ComponentModel.ISupportInitialize).BeginInit()
        utpcWires.SuspendLayout()
        CType(ugWires, ComponentModel.ISupportInitialize).BeginInit()
        utpcNets.SuspendLayout()
        CType(ugNets, ComponentModel.ISupportInitialize).BeginInit()
        utpcApprovals.SuspendLayout()
        CType(ugApprovals, ComponentModel.ISupportInitialize).BeginInit()
        utpcChangeDescriptions.SuspendLayout()
        CType(ugChangeDescriptions, ComponentModel.ISupportInitialize).BeginInit()
        utpcCoPacks.SuspendLayout()
        CType(ugCoPacks, ComponentModel.ISupportInitialize).BeginInit()
        utpcDefDimSpecs.SuspendLayout()
        CType(ugDefDimSpecs, ComponentModel.ISupportInitialize).BeginInit()
        utpcDimSpecs.SuspendLayout()
        CType(ugDimSpecs, ComponentModel.ISupportInitialize).BeginInit()
        utpcRedlinings.SuspendLayout()
        CType(ugRedlinings, ComponentModel.ISupportInitialize).BeginInit()
        utpcQMStamps.SuspendLayout()
        CType(ugQMStamps, ComponentModel.ISupportInitialize).BeginInit()
        utpcDifferences.SuspendLayout()
        CType(ugDifferences, ComponentModel.ISupportInitialize).BeginInit()
        CType(ColumnViewBindingSource, ComponentModel.ISupportInitialize).BeginInit()
        CType(utcInformationHub, ComponentModel.ISupportInitialize).BeginInit()
        utcInformationHub.SuspendLayout()
        CType(udsHarness, ComponentModel.ISupportInitialize).BeginInit()
        CType(udsModules, ComponentModel.ISupportInitialize).BeginInit()
        CType(udsVertices, ComponentModel.ISupportInitialize).BeginInit()
        CType(udsSegments, ComponentModel.ISupportInitialize).BeginInit()
        CType(udsAccessories, ComponentModel.ISupportInitialize).BeginInit()
        CType(udsFixings, ComponentModel.ISupportInitialize).BeginInit()
        CType(udsComponents, ComponentModel.ISupportInitialize).BeginInit()
        CType(udsConnectors, ComponentModel.ISupportInitialize).BeginInit()
        CType(udsCables, ComponentModel.ISupportInitialize).BeginInit()
        CType(udsWires, ComponentModel.ISupportInitialize).BeginInit()
        CType(udsNets, ComponentModel.ISupportInitialize).BeginInit()
        CType(utmInformationHub, ComponentModel.ISupportInitialize).BeginInit()
        CType(udsRedlinings, ComponentModel.ISupportInitialize).BeginInit()
        CType(udsChangeDescriptions, ComponentModel.ISupportInitialize).BeginInit()
        CType(udsAssemblyParts, ComponentModel.ISupportInitialize).BeginInit()
        CType(udsCoPacks, ComponentModel.ISupportInitialize).BeginInit()
        CType(udsComponentBoxes, ComponentModel.ISupportInitialize).BeginInit()
        CType(udsApprovals, ComponentModel.ISupportInitialize).BeginInit()
        CType(utchpInformationHub, ComponentModel.ISupportInitialize).BeginInit()
        CType(udsQMStamps, ComponentModel.ISupportInitialize).BeginInit()
        CType(udsDefDimSpecs, ComponentModel.ISupportInitialize).BeginInit()
        CType(udsDimSpecs, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' utpcHarness
        ' 
        utpcHarness.Controls.Add(ugHarness)
        resources.ApplyResources(utpcHarness, "utpcHarness")
        utpcHarness.Name = "utpcHarness"
        ' 
        ' ugHarness
        ' 
        Appearance1.BackColor = SystemColors.Window
        Appearance1.BorderColor = SystemColors.InactiveCaption
        ugHarness.DisplayLayout.Appearance = Appearance1
        ugHarness.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid
        ugHarness.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False
        Appearance2.BackColor = SystemColors.ActiveBorder
        Appearance2.BackColor2 = SystemColors.ControlDark
        Appearance2.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical
        Appearance2.BorderColor = SystemColors.Window
        ugHarness.DisplayLayout.GroupByBox.Appearance = Appearance2
        Appearance3.ForeColor = SystemColors.GrayText
        ugHarness.DisplayLayout.GroupByBox.BandLabelAppearance = Appearance3
        ugHarness.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid
        Appearance4.BackColor = SystemColors.ControlLightLight
        Appearance4.BackColor2 = SystemColors.Control
        Appearance4.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal
        Appearance4.ForeColor = SystemColors.GrayText
        ugHarness.DisplayLayout.GroupByBox.PromptAppearance = Appearance4
        ugHarness.DisplayLayout.MaxColScrollRegions = 1
        ugHarness.DisplayLayout.MaxRowScrollRegions = 1
        Appearance5.BackColor = SystemColors.Window
        Appearance5.ForeColor = SystemColors.ControlText
        ugHarness.DisplayLayout.Override.ActiveCellAppearance = Appearance5
        Appearance6.BackColor = SystemColors.Highlight
        Appearance6.ForeColor = SystemColors.HighlightText
        ugHarness.DisplayLayout.Override.ActiveRowAppearance = Appearance6
        ugHarness.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted
        ugHarness.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted
        Appearance7.BackColor = SystemColors.Window
        ugHarness.DisplayLayout.Override.CardAreaAppearance = Appearance7
        Appearance8.BorderColor = Color.Silver
        Appearance8.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter
        ugHarness.DisplayLayout.Override.CellAppearance = Appearance8
        ugHarness.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText
        ugHarness.DisplayLayout.Override.CellPadding = 0
        Appearance9.BackColor = SystemColors.Control
        Appearance9.BackColor2 = SystemColors.ControlDark
        Appearance9.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element
        Appearance9.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal
        Appearance9.BorderColor = SystemColors.Window
        ugHarness.DisplayLayout.Override.GroupByRowAppearance = Appearance9
        resources.ApplyResources(Appearance10, "Appearance10")
        Appearance10.ForceApplyResources = ""
        ugHarness.DisplayLayout.Override.HeaderAppearance = Appearance10
        ugHarness.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti
        ugHarness.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand
        Appearance11.BackColor = SystemColors.Window
        Appearance11.BorderColor = Color.Silver
        ugHarness.DisplayLayout.Override.RowAppearance = Appearance11
        ugHarness.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False
        Appearance12.BackColor = SystemColors.ControlLight
        ugHarness.DisplayLayout.Override.TemplateAddRowAppearance = Appearance12
        ugHarness.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill
        ugHarness.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate
        ugHarness.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy
        resources.ApplyResources(ugHarness, "ugHarness")
        ugHarness.Name = "ugHarness"
        ' 
        ' utpcModules
        ' 
        utpcModules.Controls.Add(ugModules)
        resources.ApplyResources(utpcModules, "utpcModules")
        utpcModules.Name = "utpcModules"
        ' 
        ' ugModules
        ' 
        Appearance13.BackColor = SystemColors.Window
        Appearance13.BorderColor = SystemColors.InactiveCaption
        ugModules.DisplayLayout.Appearance = Appearance13
        ugModules.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid
        ugModules.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False
        Appearance14.BackColor = SystemColors.ActiveBorder
        Appearance14.BackColor2 = SystemColors.ControlDark
        Appearance14.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical
        Appearance14.BorderColor = SystemColors.Window
        ugModules.DisplayLayout.GroupByBox.Appearance = Appearance14
        Appearance15.ForeColor = SystemColors.GrayText
        ugModules.DisplayLayout.GroupByBox.BandLabelAppearance = Appearance15
        ugModules.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid
        Appearance16.BackColor = SystemColors.ControlLightLight
        Appearance16.BackColor2 = SystemColors.Control
        Appearance16.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal
        Appearance16.ForeColor = SystemColors.GrayText
        ugModules.DisplayLayout.GroupByBox.PromptAppearance = Appearance16
        ugModules.DisplayLayout.MaxColScrollRegions = 1
        ugModules.DisplayLayout.MaxRowScrollRegions = 1
        Appearance17.BackColor = SystemColors.Window
        Appearance17.ForeColor = SystemColors.ControlText
        ugModules.DisplayLayout.Override.ActiveCellAppearance = Appearance17
        Appearance18.BackColor = SystemColors.Highlight
        Appearance18.ForeColor = SystemColors.HighlightText
        ugModules.DisplayLayout.Override.ActiveRowAppearance = Appearance18
        ugModules.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted
        ugModules.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted
        Appearance19.BackColor = SystemColors.Window
        ugModules.DisplayLayout.Override.CardAreaAppearance = Appearance19
        Appearance20.BorderColor = Color.Silver
        Appearance20.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter
        ugModules.DisplayLayout.Override.CellAppearance = Appearance20
        ugModules.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText
        ugModules.DisplayLayout.Override.CellPadding = 0
        Appearance21.BackColor = SystemColors.Control
        Appearance21.BackColor2 = SystemColors.ControlDark
        Appearance21.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element
        Appearance21.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal
        Appearance21.BorderColor = SystemColors.Window
        ugModules.DisplayLayout.Override.GroupByRowAppearance = Appearance21
        resources.ApplyResources(Appearance22, "Appearance22")
        Appearance22.ForceApplyResources = ""
        ugModules.DisplayLayout.Override.HeaderAppearance = Appearance22
        ugModules.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti
        ugModules.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand
        Appearance23.BackColor = SystemColors.Window
        Appearance23.BorderColor = Color.Silver
        ugModules.DisplayLayout.Override.RowAppearance = Appearance23
        ugModules.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False
        Appearance24.BackColor = SystemColors.ControlLight
        ugModules.DisplayLayout.Override.TemplateAddRowAppearance = Appearance24
        ugModules.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill
        ugModules.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate
        ugModules.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy
        resources.ApplyResources(ugModules, "ugModules")
        ugModules.Name = "ugModules"
        ' 
        ' utpcVertices
        ' 
        utpcVertices.Controls.Add(ugVertices)
        resources.ApplyResources(utpcVertices, "utpcVertices")
        utpcVertices.Name = "utpcVertices"
        ' 
        ' ugVertices
        ' 
        Appearance25.BackColor = SystemColors.Window
        Appearance25.BorderColor = SystemColors.InactiveCaption
        ugVertices.DisplayLayout.Appearance = Appearance25
        ugVertices.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid
        ugVertices.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False
        Appearance26.BackColor = SystemColors.ActiveBorder
        Appearance26.BackColor2 = SystemColors.ControlDark
        Appearance26.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical
        Appearance26.BorderColor = SystemColors.Window
        ugVertices.DisplayLayout.GroupByBox.Appearance = Appearance26
        Appearance27.ForeColor = SystemColors.GrayText
        ugVertices.DisplayLayout.GroupByBox.BandLabelAppearance = Appearance27
        ugVertices.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid
        Appearance28.BackColor = SystemColors.ControlLightLight
        Appearance28.BackColor2 = SystemColors.Control
        Appearance28.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal
        Appearance28.ForeColor = SystemColors.GrayText
        ugVertices.DisplayLayout.GroupByBox.PromptAppearance = Appearance28
        ugVertices.DisplayLayout.MaxColScrollRegions = 1
        ugVertices.DisplayLayout.MaxRowScrollRegions = 1
        Appearance29.BackColor = SystemColors.Window
        Appearance29.ForeColor = SystemColors.ControlText
        ugVertices.DisplayLayout.Override.ActiveCellAppearance = Appearance29
        Appearance30.BackColor = SystemColors.Highlight
        Appearance30.ForeColor = SystemColors.HighlightText
        ugVertices.DisplayLayout.Override.ActiveRowAppearance = Appearance30
        ugVertices.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted
        ugVertices.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted
        Appearance31.BackColor = SystemColors.Window
        ugVertices.DisplayLayout.Override.CardAreaAppearance = Appearance31
        Appearance32.BorderColor = Color.Silver
        Appearance32.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter
        ugVertices.DisplayLayout.Override.CellAppearance = Appearance32
        ugVertices.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText
        ugVertices.DisplayLayout.Override.CellPadding = 0
        Appearance33.BackColor = SystemColors.Control
        Appearance33.BackColor2 = SystemColors.ControlDark
        Appearance33.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element
        Appearance33.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal
        Appearance33.BorderColor = SystemColors.Window
        ugVertices.DisplayLayout.Override.GroupByRowAppearance = Appearance33
        resources.ApplyResources(Appearance34, "Appearance34")
        Appearance34.ForceApplyResources = ""
        ugVertices.DisplayLayout.Override.HeaderAppearance = Appearance34
        ugVertices.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti
        ugVertices.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand
        Appearance35.BackColor = SystemColors.Window
        Appearance35.BorderColor = Color.Silver
        ugVertices.DisplayLayout.Override.RowAppearance = Appearance35
        ugVertices.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False
        Appearance36.BackColor = SystemColors.ControlLight
        ugVertices.DisplayLayout.Override.TemplateAddRowAppearance = Appearance36
        ugVertices.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill
        ugVertices.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate
        ugVertices.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy
        resources.ApplyResources(ugVertices, "ugVertices")
        ugVertices.Name = "ugVertices"
        ' 
        ' utpcSegments
        ' 
        utpcSegments.Controls.Add(ugSegments)
        resources.ApplyResources(utpcSegments, "utpcSegments")
        utpcSegments.Name = "utpcSegments"
        ' 
        ' ugSegments
        ' 
        Appearance37.BackColor = SystemColors.Window
        Appearance37.BorderColor = SystemColors.InactiveCaption
        ugSegments.DisplayLayout.Appearance = Appearance37
        ugSegments.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid
        ugSegments.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False
        Appearance38.BackColor = SystemColors.ActiveBorder
        Appearance38.BackColor2 = SystemColors.ControlDark
        Appearance38.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical
        Appearance38.BorderColor = SystemColors.Window
        ugSegments.DisplayLayout.GroupByBox.Appearance = Appearance38
        Appearance39.ForeColor = SystemColors.GrayText
        ugSegments.DisplayLayout.GroupByBox.BandLabelAppearance = Appearance39
        ugSegments.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid
        Appearance40.BackColor = SystemColors.ControlLightLight
        Appearance40.BackColor2 = SystemColors.Control
        Appearance40.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal
        Appearance40.ForeColor = SystemColors.GrayText
        ugSegments.DisplayLayout.GroupByBox.PromptAppearance = Appearance40
        ugSegments.DisplayLayout.MaxColScrollRegions = 1
        ugSegments.DisplayLayout.MaxRowScrollRegions = 1
        Appearance41.BackColor = SystemColors.Window
        Appearance41.ForeColor = SystemColors.ControlText
        ugSegments.DisplayLayout.Override.ActiveCellAppearance = Appearance41
        Appearance42.BackColor = SystemColors.Highlight
        Appearance42.ForeColor = SystemColors.HighlightText
        ugSegments.DisplayLayout.Override.ActiveRowAppearance = Appearance42
        ugSegments.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted
        ugSegments.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted
        Appearance43.BackColor = SystemColors.Window
        ugSegments.DisplayLayout.Override.CardAreaAppearance = Appearance43
        Appearance44.BorderColor = Color.Silver
        Appearance44.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter
        ugSegments.DisplayLayout.Override.CellAppearance = Appearance44
        ugSegments.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText
        ugSegments.DisplayLayout.Override.CellPadding = 0
        Appearance45.BackColor = SystemColors.Control
        Appearance45.BackColor2 = SystemColors.ControlDark
        Appearance45.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element
        Appearance45.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal
        Appearance45.BorderColor = SystemColors.Window
        ugSegments.DisplayLayout.Override.GroupByRowAppearance = Appearance45
        resources.ApplyResources(Appearance46, "Appearance46")
        Appearance46.ForceApplyResources = ""
        ugSegments.DisplayLayout.Override.HeaderAppearance = Appearance46
        ugSegments.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti
        ugSegments.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand
        Appearance47.BackColor = SystemColors.Window
        Appearance47.BorderColor = Color.Silver
        ugSegments.DisplayLayout.Override.RowAppearance = Appearance47
        ugSegments.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False
        Appearance48.BackColor = SystemColors.ControlLight
        ugSegments.DisplayLayout.Override.TemplateAddRowAppearance = Appearance48
        ugSegments.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill
        ugSegments.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate
        ugSegments.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy
        resources.ApplyResources(ugSegments, "ugSegments")
        ugSegments.Name = "ugSegments"
        ' 
        ' utpcAccessories
        ' 
        utpcAccessories.Controls.Add(ugAccessories)
        resources.ApplyResources(utpcAccessories, "utpcAccessories")
        utpcAccessories.Name = "utpcAccessories"
        ' 
        ' ugAccessories
        ' 
        Appearance49.BackColor = SystemColors.Window
        Appearance49.BorderColor = SystemColors.InactiveCaption
        ugAccessories.DisplayLayout.Appearance = Appearance49
        ugAccessories.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid
        ugAccessories.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False
        Appearance50.BackColor = SystemColors.ActiveBorder
        Appearance50.BackColor2 = SystemColors.ControlDark
        Appearance50.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical
        Appearance50.BorderColor = SystemColors.Window
        ugAccessories.DisplayLayout.GroupByBox.Appearance = Appearance50
        Appearance51.ForeColor = SystemColors.GrayText
        ugAccessories.DisplayLayout.GroupByBox.BandLabelAppearance = Appearance51
        ugAccessories.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid
        Appearance52.BackColor = SystemColors.ControlLightLight
        Appearance52.BackColor2 = SystemColors.Control
        Appearance52.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal
        Appearance52.ForeColor = SystemColors.GrayText
        ugAccessories.DisplayLayout.GroupByBox.PromptAppearance = Appearance52
        ugAccessories.DisplayLayout.MaxColScrollRegions = 1
        ugAccessories.DisplayLayout.MaxRowScrollRegions = 1
        Appearance53.BackColor = SystemColors.Window
        Appearance53.ForeColor = SystemColors.ControlText
        ugAccessories.DisplayLayout.Override.ActiveCellAppearance = Appearance53
        Appearance54.BackColor = SystemColors.Highlight
        Appearance54.ForeColor = SystemColors.HighlightText
        ugAccessories.DisplayLayout.Override.ActiveRowAppearance = Appearance54
        ugAccessories.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted
        ugAccessories.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted
        Appearance55.BackColor = SystemColors.Window
        ugAccessories.DisplayLayout.Override.CardAreaAppearance = Appearance55
        Appearance56.BorderColor = Color.Silver
        Appearance56.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter
        ugAccessories.DisplayLayout.Override.CellAppearance = Appearance56
        ugAccessories.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText
        ugAccessories.DisplayLayout.Override.CellPadding = 0
        Appearance57.BackColor = SystemColors.Control
        Appearance57.BackColor2 = SystemColors.ControlDark
        Appearance57.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element
        Appearance57.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal
        Appearance57.BorderColor = SystemColors.Window
        ugAccessories.DisplayLayout.Override.GroupByRowAppearance = Appearance57
        resources.ApplyResources(Appearance58, "Appearance58")
        Appearance58.ForceApplyResources = ""
        ugAccessories.DisplayLayout.Override.HeaderAppearance = Appearance58
        ugAccessories.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti
        ugAccessories.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand
        Appearance59.BackColor = SystemColors.Window
        Appearance59.BorderColor = Color.Silver
        ugAccessories.DisplayLayout.Override.RowAppearance = Appearance59
        ugAccessories.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False
        Appearance60.BackColor = SystemColors.ControlLight
        ugAccessories.DisplayLayout.Override.TemplateAddRowAppearance = Appearance60
        ugAccessories.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill
        ugAccessories.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate
        ugAccessories.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy
        resources.ApplyResources(ugAccessories, "ugAccessories")
        ugAccessories.Name = "ugAccessories"
        ' 
        ' utpcAssemblyParts
        ' 
        utpcAssemblyParts.Controls.Add(ugAssemblyParts)
        resources.ApplyResources(utpcAssemblyParts, "utpcAssemblyParts")
        utpcAssemblyParts.Name = "utpcAssemblyParts"
        ' 
        ' ugAssemblyParts
        ' 
        Appearance61.BackColor = SystemColors.Window
        Appearance61.BorderColor = SystemColors.InactiveCaption
        ugAssemblyParts.DisplayLayout.Appearance = Appearance61
        ugAssemblyParts.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid
        ugAssemblyParts.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False
        Appearance62.BackColor = SystemColors.ActiveBorder
        Appearance62.BackColor2 = SystemColors.ControlDark
        Appearance62.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical
        Appearance62.BorderColor = SystemColors.Window
        ugAssemblyParts.DisplayLayout.GroupByBox.Appearance = Appearance62
        Appearance63.ForeColor = SystemColors.GrayText
        ugAssemblyParts.DisplayLayout.GroupByBox.BandLabelAppearance = Appearance63
        ugAssemblyParts.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid
        Appearance64.BackColor = SystemColors.ControlLightLight
        Appearance64.BackColor2 = SystemColors.Control
        Appearance64.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal
        Appearance64.ForeColor = SystemColors.GrayText
        ugAssemblyParts.DisplayLayout.GroupByBox.PromptAppearance = Appearance64
        ugAssemblyParts.DisplayLayout.MaxColScrollRegions = 1
        ugAssemblyParts.DisplayLayout.MaxRowScrollRegions = 1
        Appearance65.BackColor = SystemColors.Window
        Appearance65.ForeColor = SystemColors.ControlText
        ugAssemblyParts.DisplayLayout.Override.ActiveCellAppearance = Appearance65
        Appearance66.BackColor = SystemColors.Highlight
        Appearance66.ForeColor = SystemColors.HighlightText
        ugAssemblyParts.DisplayLayout.Override.ActiveRowAppearance = Appearance66
        ugAssemblyParts.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted
        ugAssemblyParts.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted
        Appearance67.BackColor = SystemColors.Window
        ugAssemblyParts.DisplayLayout.Override.CardAreaAppearance = Appearance67
        Appearance68.BorderColor = Color.Silver
        Appearance68.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter
        ugAssemblyParts.DisplayLayout.Override.CellAppearance = Appearance68
        ugAssemblyParts.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText
        ugAssemblyParts.DisplayLayout.Override.CellPadding = 0
        Appearance69.BackColor = SystemColors.Control
        Appearance69.BackColor2 = SystemColors.ControlDark
        Appearance69.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element
        Appearance69.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal
        Appearance69.BorderColor = SystemColors.Window
        ugAssemblyParts.DisplayLayout.Override.GroupByRowAppearance = Appearance69
        resources.ApplyResources(Appearance70, "Appearance70")
        Appearance70.ForceApplyResources = ""
        ugAssemblyParts.DisplayLayout.Override.HeaderAppearance = Appearance70
        ugAssemblyParts.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti
        ugAssemblyParts.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand
        Appearance71.BackColor = SystemColors.Window
        Appearance71.BorderColor = Color.Silver
        ugAssemblyParts.DisplayLayout.Override.RowAppearance = Appearance71
        ugAssemblyParts.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False
        Appearance72.BackColor = SystemColors.ControlLight
        ugAssemblyParts.DisplayLayout.Override.TemplateAddRowAppearance = Appearance72
        ugAssemblyParts.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill
        ugAssemblyParts.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate
        ugAssemblyParts.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy
        resources.ApplyResources(ugAssemblyParts, "ugAssemblyParts")
        ugAssemblyParts.Name = "ugAssemblyParts"
        ' 
        ' utpcFixings
        ' 
        utpcFixings.Controls.Add(ugFixings)
        resources.ApplyResources(utpcFixings, "utpcFixings")
        utpcFixings.Name = "utpcFixings"
        ' 
        ' ugFixings
        ' 
        Appearance73.BackColor = SystemColors.Window
        Appearance73.BorderColor = SystemColors.InactiveCaption
        ugFixings.DisplayLayout.Appearance = Appearance73
        ugFixings.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid
        ugFixings.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False
        Appearance74.BackColor = SystemColors.ActiveBorder
        Appearance74.BackColor2 = SystemColors.ControlDark
        Appearance74.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical
        Appearance74.BorderColor = SystemColors.Window
        ugFixings.DisplayLayout.GroupByBox.Appearance = Appearance74
        Appearance75.ForeColor = SystemColors.GrayText
        ugFixings.DisplayLayout.GroupByBox.BandLabelAppearance = Appearance75
        ugFixings.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid
        Appearance76.BackColor = SystemColors.ControlLightLight
        Appearance76.BackColor2 = SystemColors.Control
        Appearance76.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal
        Appearance76.ForeColor = SystemColors.GrayText
        ugFixings.DisplayLayout.GroupByBox.PromptAppearance = Appearance76
        ugFixings.DisplayLayout.MaxColScrollRegions = 1
        ugFixings.DisplayLayout.MaxRowScrollRegions = 1
        Appearance77.BackColor = SystemColors.Window
        Appearance77.ForeColor = SystemColors.ControlText
        ugFixings.DisplayLayout.Override.ActiveCellAppearance = Appearance77
        Appearance78.BackColor = SystemColors.Highlight
        Appearance78.ForeColor = SystemColors.HighlightText
        ugFixings.DisplayLayout.Override.ActiveRowAppearance = Appearance78
        ugFixings.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted
        ugFixings.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted
        Appearance79.BackColor = SystemColors.Window
        ugFixings.DisplayLayout.Override.CardAreaAppearance = Appearance79
        Appearance80.BorderColor = Color.Silver
        Appearance80.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter
        ugFixings.DisplayLayout.Override.CellAppearance = Appearance80
        ugFixings.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText
        ugFixings.DisplayLayout.Override.CellPadding = 0
        Appearance81.BackColor = SystemColors.Control
        Appearance81.BackColor2 = SystemColors.ControlDark
        Appearance81.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element
        Appearance81.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal
        Appearance81.BorderColor = SystemColors.Window
        ugFixings.DisplayLayout.Override.GroupByRowAppearance = Appearance81
        resources.ApplyResources(Appearance82, "Appearance82")
        Appearance82.ForceApplyResources = ""
        ugFixings.DisplayLayout.Override.HeaderAppearance = Appearance82
        ugFixings.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti
        ugFixings.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand
        Appearance83.BackColor = SystemColors.Window
        Appearance83.BorderColor = Color.Silver
        ugFixings.DisplayLayout.Override.RowAppearance = Appearance83
        ugFixings.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False
        Appearance84.BackColor = SystemColors.ControlLight
        ugFixings.DisplayLayout.Override.TemplateAddRowAppearance = Appearance84
        ugFixings.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill
        ugFixings.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate
        ugFixings.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy
        resources.ApplyResources(ugFixings, "ugFixings")
        ugFixings.Name = "ugFixings"
        ' 
        ' utpcComponentBoxes
        ' 
        utpcComponentBoxes.Controls.Add(ugComponentBoxes)
        resources.ApplyResources(utpcComponentBoxes, "utpcComponentBoxes")
        utpcComponentBoxes.Name = "utpcComponentBoxes"
        ' 
        ' ugComponentBoxes
        ' 
        Appearance85.BackColor = SystemColors.Window
        Appearance85.BorderColor = SystemColors.InactiveCaption
        ugComponentBoxes.DisplayLayout.Appearance = Appearance85
        ugComponentBoxes.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid
        ugComponentBoxes.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False
        Appearance86.BackColor = SystemColors.ActiveBorder
        Appearance86.BackColor2 = SystemColors.ControlDark
        Appearance86.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical
        Appearance86.BorderColor = SystemColors.Window
        ugComponentBoxes.DisplayLayout.GroupByBox.Appearance = Appearance86
        Appearance87.ForeColor = SystemColors.GrayText
        ugComponentBoxes.DisplayLayout.GroupByBox.BandLabelAppearance = Appearance87
        ugComponentBoxes.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid
        Appearance88.BackColor = SystemColors.ControlLightLight
        Appearance88.BackColor2 = SystemColors.Control
        Appearance88.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal
        Appearance88.ForeColor = SystemColors.GrayText
        ugComponentBoxes.DisplayLayout.GroupByBox.PromptAppearance = Appearance88
        ugComponentBoxes.DisplayLayout.MaxColScrollRegions = 1
        ugComponentBoxes.DisplayLayout.MaxRowScrollRegions = 1
        Appearance89.BackColor = SystemColors.Window
        Appearance89.ForeColor = SystemColors.ControlText
        ugComponentBoxes.DisplayLayout.Override.ActiveCellAppearance = Appearance89
        Appearance90.BackColor = SystemColors.Highlight
        Appearance90.ForeColor = SystemColors.HighlightText
        ugComponentBoxes.DisplayLayout.Override.ActiveRowAppearance = Appearance90
        ugComponentBoxes.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted
        ugComponentBoxes.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted
        Appearance91.BackColor = SystemColors.Window
        ugComponentBoxes.DisplayLayout.Override.CardAreaAppearance = Appearance91
        Appearance92.BorderColor = Color.Silver
        Appearance92.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter
        ugComponentBoxes.DisplayLayout.Override.CellAppearance = Appearance92
        ugComponentBoxes.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText
        ugComponentBoxes.DisplayLayout.Override.CellPadding = 0
        Appearance93.BackColor = SystemColors.Control
        Appearance93.BackColor2 = SystemColors.ControlDark
        Appearance93.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element
        Appearance93.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal
        Appearance93.BorderColor = SystemColors.Window
        ugComponentBoxes.DisplayLayout.Override.GroupByRowAppearance = Appearance93
        resources.ApplyResources(Appearance94, "Appearance94")
        Appearance94.ForceApplyResources = ""
        ugComponentBoxes.DisplayLayout.Override.HeaderAppearance = Appearance94
        ugComponentBoxes.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti
        ugComponentBoxes.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand
        Appearance95.BackColor = SystemColors.Window
        Appearance95.BorderColor = Color.Silver
        ugComponentBoxes.DisplayLayout.Override.RowAppearance = Appearance95
        ugComponentBoxes.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False
        Appearance96.BackColor = SystemColors.ControlLight
        ugComponentBoxes.DisplayLayout.Override.TemplateAddRowAppearance = Appearance96
        ugComponentBoxes.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill
        ugComponentBoxes.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate
        ugComponentBoxes.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy
        resources.ApplyResources(ugComponentBoxes, "ugComponentBoxes")
        ugComponentBoxes.Name = "ugComponentBoxes"
        ' 
        ' utpcComponents
        ' 
        utpcComponents.Controls.Add(ugComponents)
        resources.ApplyResources(utpcComponents, "utpcComponents")
        utpcComponents.Name = "utpcComponents"
        ' 
        ' ugComponents
        ' 
        Appearance97.BackColor = SystemColors.Window
        Appearance97.BorderColor = SystemColors.InactiveCaption
        ugComponents.DisplayLayout.Appearance = Appearance97
        ugComponents.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid
        ugComponents.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False
        Appearance98.BackColor = SystemColors.ActiveBorder
        Appearance98.BackColor2 = SystemColors.ControlDark
        Appearance98.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical
        Appearance98.BorderColor = SystemColors.Window
        ugComponents.DisplayLayout.GroupByBox.Appearance = Appearance98
        Appearance99.ForeColor = SystemColors.GrayText
        ugComponents.DisplayLayout.GroupByBox.BandLabelAppearance = Appearance99
        ugComponents.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid
        Appearance100.BackColor = SystemColors.ControlLightLight
        Appearance100.BackColor2 = SystemColors.Control
        Appearance100.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal
        Appearance100.ForeColor = SystemColors.GrayText
        ugComponents.DisplayLayout.GroupByBox.PromptAppearance = Appearance100
        ugComponents.DisplayLayout.MaxColScrollRegions = 1
        ugComponents.DisplayLayout.MaxRowScrollRegions = 1
        Appearance101.BackColor = SystemColors.Window
        Appearance101.ForeColor = SystemColors.ControlText
        ugComponents.DisplayLayout.Override.ActiveCellAppearance = Appearance101
        Appearance102.BackColor = SystemColors.Highlight
        Appearance102.ForeColor = SystemColors.HighlightText
        ugComponents.DisplayLayout.Override.ActiveRowAppearance = Appearance102
        ugComponents.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted
        ugComponents.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted
        Appearance103.BackColor = SystemColors.Window
        ugComponents.DisplayLayout.Override.CardAreaAppearance = Appearance103
        Appearance104.BorderColor = Color.Silver
        Appearance104.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter
        ugComponents.DisplayLayout.Override.CellAppearance = Appearance104
        ugComponents.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText
        ugComponents.DisplayLayout.Override.CellPadding = 0
        Appearance105.BackColor = SystemColors.Control
        Appearance105.BackColor2 = SystemColors.ControlDark
        Appearance105.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element
        Appearance105.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal
        Appearance105.BorderColor = SystemColors.Window
        ugComponents.DisplayLayout.Override.GroupByRowAppearance = Appearance105
        resources.ApplyResources(Appearance106, "Appearance106")
        Appearance106.ForceApplyResources = ""
        ugComponents.DisplayLayout.Override.HeaderAppearance = Appearance106
        ugComponents.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti
        ugComponents.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand
        Appearance107.BackColor = SystemColors.Window
        Appearance107.BorderColor = Color.Silver
        ugComponents.DisplayLayout.Override.RowAppearance = Appearance107
        ugComponents.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False
        Appearance108.BackColor = SystemColors.ControlLight
        ugComponents.DisplayLayout.Override.TemplateAddRowAppearance = Appearance108
        ugComponents.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill
        ugComponents.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate
        ugComponents.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy
        resources.ApplyResources(ugComponents, "ugComponents")
        ugComponents.Name = "ugComponents"
        ' 
        ' utpcConnectors
        ' 
        utpcConnectors.Controls.Add(ugConnectors)
        resources.ApplyResources(utpcConnectors, "utpcConnectors")
        utpcConnectors.Name = "utpcConnectors"
        ' 
        ' ugConnectors
        ' 
        Appearance109.BackColor = SystemColors.Window
        Appearance109.BorderColor = SystemColors.InactiveCaption
        ugConnectors.DisplayLayout.Appearance = Appearance109
        ugConnectors.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid
        ugConnectors.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False
        Appearance110.BackColor = SystemColors.ActiveBorder
        Appearance110.BackColor2 = SystemColors.ControlDark
        Appearance110.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical
        Appearance110.BorderColor = SystemColors.Window
        ugConnectors.DisplayLayout.GroupByBox.Appearance = Appearance110
        Appearance111.ForeColor = SystemColors.GrayText
        ugConnectors.DisplayLayout.GroupByBox.BandLabelAppearance = Appearance111
        ugConnectors.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid
        Appearance112.BackColor = SystemColors.ControlLightLight
        Appearance112.BackColor2 = SystemColors.Control
        Appearance112.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal
        Appearance112.ForeColor = SystemColors.GrayText
        ugConnectors.DisplayLayout.GroupByBox.PromptAppearance = Appearance112
        ugConnectors.DisplayLayout.MaxColScrollRegions = 1
        ugConnectors.DisplayLayout.MaxRowScrollRegions = 1
        Appearance113.BackColor = SystemColors.Window
        Appearance113.ForeColor = SystemColors.ControlText
        ugConnectors.DisplayLayout.Override.ActiveCellAppearance = Appearance113
        Appearance114.BackColor = SystemColors.Highlight
        Appearance114.ForeColor = SystemColors.HighlightText
        ugConnectors.DisplayLayout.Override.ActiveRowAppearance = Appearance114
        ugConnectors.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted
        ugConnectors.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted
        Appearance115.BackColor = SystemColors.Window
        ugConnectors.DisplayLayout.Override.CardAreaAppearance = Appearance115
        Appearance116.BorderColor = Color.Silver
        Appearance116.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter
        ugConnectors.DisplayLayout.Override.CellAppearance = Appearance116
        ugConnectors.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText
        ugConnectors.DisplayLayout.Override.CellPadding = 0
        Appearance117.BackColor = SystemColors.Control
        Appearance117.BackColor2 = SystemColors.ControlDark
        Appearance117.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element
        Appearance117.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal
        Appearance117.BorderColor = SystemColors.Window
        ugConnectors.DisplayLayout.Override.GroupByRowAppearance = Appearance117
        resources.ApplyResources(Appearance118, "Appearance118")
        Appearance118.ForceApplyResources = ""
        ugConnectors.DisplayLayout.Override.HeaderAppearance = Appearance118
        ugConnectors.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti
        ugConnectors.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand
        Appearance119.BackColor = SystemColors.Window
        Appearance119.BorderColor = Color.Silver
        ugConnectors.DisplayLayout.Override.RowAppearance = Appearance119
        ugConnectors.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False
        Appearance120.BackColor = SystemColors.ControlLight
        ugConnectors.DisplayLayout.Override.TemplateAddRowAppearance = Appearance120
        ugConnectors.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill
        ugConnectors.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate
        ugConnectors.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy
        resources.ApplyResources(ugConnectors, "ugConnectors")
        ugConnectors.Name = "ugConnectors"
        ' 
        ' utpcCables
        ' 
        utpcCables.Controls.Add(ugCables)
        resources.ApplyResources(utpcCables, "utpcCables")
        utpcCables.Name = "utpcCables"
        ' 
        ' ugCables
        ' 
        Appearance121.BackColor = SystemColors.Window
        Appearance121.BorderColor = SystemColors.InactiveCaption
        ugCables.DisplayLayout.Appearance = Appearance121
        ugCables.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid
        ugCables.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False
        Appearance122.BackColor = SystemColors.ActiveBorder
        Appearance122.BackColor2 = SystemColors.ControlDark
        Appearance122.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical
        Appearance122.BorderColor = SystemColors.Window
        ugCables.DisplayLayout.GroupByBox.Appearance = Appearance122
        Appearance123.ForeColor = SystemColors.GrayText
        ugCables.DisplayLayout.GroupByBox.BandLabelAppearance = Appearance123
        ugCables.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid
        Appearance124.BackColor = SystemColors.ControlLightLight
        Appearance124.BackColor2 = SystemColors.Control
        Appearance124.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal
        Appearance124.ForeColor = SystemColors.GrayText
        ugCables.DisplayLayout.GroupByBox.PromptAppearance = Appearance124
        ugCables.DisplayLayout.MaxColScrollRegions = 1
        ugCables.DisplayLayout.MaxRowScrollRegions = 1
        Appearance125.BackColor = SystemColors.Window
        Appearance125.ForeColor = SystemColors.ControlText
        ugCables.DisplayLayout.Override.ActiveCellAppearance = Appearance125
        Appearance126.BackColor = SystemColors.Highlight
        Appearance126.ForeColor = SystemColors.HighlightText
        ugCables.DisplayLayout.Override.ActiveRowAppearance = Appearance126
        ugCables.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted
        ugCables.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted
        Appearance127.BackColor = SystemColors.Window
        ugCables.DisplayLayout.Override.CardAreaAppearance = Appearance127
        Appearance128.BorderColor = Color.Silver
        Appearance128.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter
        ugCables.DisplayLayout.Override.CellAppearance = Appearance128
        ugCables.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText
        ugCables.DisplayLayout.Override.CellPadding = 0
        Appearance129.BackColor = SystemColors.Control
        Appearance129.BackColor2 = SystemColors.ControlDark
        Appearance129.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element
        Appearance129.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal
        Appearance129.BorderColor = SystemColors.Window
        ugCables.DisplayLayout.Override.GroupByRowAppearance = Appearance129
        resources.ApplyResources(Appearance130, "Appearance130")
        Appearance130.ForceApplyResources = ""
        ugCables.DisplayLayout.Override.HeaderAppearance = Appearance130
        ugCables.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti
        ugCables.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand
        Appearance131.BackColor = SystemColors.Window
        Appearance131.BorderColor = Color.Silver
        ugCables.DisplayLayout.Override.RowAppearance = Appearance131
        ugCables.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False
        Appearance132.BackColor = SystemColors.ControlLight
        ugCables.DisplayLayout.Override.TemplateAddRowAppearance = Appearance132
        ugCables.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill
        ugCables.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate
        ugCables.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy
        resources.ApplyResources(ugCables, "ugCables")
        ugCables.Name = "ugCables"
        ' 
        ' utpcWires
        ' 
        utpcWires.Controls.Add(ugWires)
        resources.ApplyResources(utpcWires, "utpcWires")
        utpcWires.Name = "utpcWires"
        ' 
        ' ugWires
        ' 
        Appearance133.BackColor = SystemColors.Window
        Appearance133.BorderColor = SystemColors.InactiveCaption
        ugWires.DisplayLayout.Appearance = Appearance133
        ugWires.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid
        ugWires.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False
        Appearance134.BackColor = SystemColors.ActiveBorder
        Appearance134.BackColor2 = SystemColors.ControlDark
        Appearance134.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical
        Appearance134.BorderColor = SystemColors.Window
        ugWires.DisplayLayout.GroupByBox.Appearance = Appearance134
        Appearance135.ForeColor = SystemColors.GrayText
        ugWires.DisplayLayout.GroupByBox.BandLabelAppearance = Appearance135
        ugWires.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid
        Appearance136.BackColor = SystemColors.ControlLightLight
        Appearance136.BackColor2 = SystemColors.Control
        Appearance136.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal
        Appearance136.ForeColor = SystemColors.GrayText
        ugWires.DisplayLayout.GroupByBox.PromptAppearance = Appearance136
        ugWires.DisplayLayout.MaxColScrollRegions = 1
        ugWires.DisplayLayout.MaxRowScrollRegions = 1
        Appearance137.BackColor = SystemColors.Window
        Appearance137.ForeColor = SystemColors.ControlText
        ugWires.DisplayLayout.Override.ActiveCellAppearance = Appearance137
        Appearance138.BackColor = SystemColors.Highlight
        Appearance138.ForeColor = SystemColors.HighlightText
        ugWires.DisplayLayout.Override.ActiveRowAppearance = Appearance138
        ugWires.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted
        ugWires.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted
        Appearance139.BackColor = SystemColors.Window
        ugWires.DisplayLayout.Override.CardAreaAppearance = Appearance139
        Appearance140.BorderColor = Color.Silver
        Appearance140.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter
        ugWires.DisplayLayout.Override.CellAppearance = Appearance140
        ugWires.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText
        ugWires.DisplayLayout.Override.CellPadding = 0
        Appearance141.BackColor = SystemColors.Control
        Appearance141.BackColor2 = SystemColors.ControlDark
        Appearance141.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element
        Appearance141.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal
        Appearance141.BorderColor = SystemColors.Window
        ugWires.DisplayLayout.Override.GroupByRowAppearance = Appearance141
        resources.ApplyResources(Appearance142, "Appearance142")
        Appearance142.ForceApplyResources = ""
        ugWires.DisplayLayout.Override.HeaderAppearance = Appearance142
        ugWires.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti
        ugWires.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand
        Appearance143.BackColor = SystemColors.Window
        Appearance143.BorderColor = Color.Silver
        ugWires.DisplayLayout.Override.RowAppearance = Appearance143
        ugWires.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False
        Appearance144.BackColor = SystemColors.ControlLight
        ugWires.DisplayLayout.Override.TemplateAddRowAppearance = Appearance144
        ugWires.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill
        ugWires.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate
        ugWires.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy
        resources.ApplyResources(ugWires, "ugWires")
        ugWires.Name = "ugWires"
        ' 
        ' utpcNets
        ' 
        utpcNets.Controls.Add(ugNets)
        resources.ApplyResources(utpcNets, "utpcNets")
        utpcNets.Name = "utpcNets"
        ' 
        ' ugNets
        ' 
        Appearance145.BackColor = SystemColors.Window
        Appearance145.BorderColor = SystemColors.InactiveCaption
        ugNets.DisplayLayout.Appearance = Appearance145
        ugNets.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid
        ugNets.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False
        Appearance146.BackColor = SystemColors.ActiveBorder
        Appearance146.BackColor2 = SystemColors.ControlDark
        Appearance146.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical
        Appearance146.BorderColor = SystemColors.Window
        ugNets.DisplayLayout.GroupByBox.Appearance = Appearance146
        Appearance147.ForeColor = SystemColors.GrayText
        ugNets.DisplayLayout.GroupByBox.BandLabelAppearance = Appearance147
        ugNets.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid
        Appearance148.BackColor = SystemColors.ControlLightLight
        Appearance148.BackColor2 = SystemColors.Control
        Appearance148.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal
        Appearance148.ForeColor = SystemColors.GrayText
        ugNets.DisplayLayout.GroupByBox.PromptAppearance = Appearance148
        ugNets.DisplayLayout.MaxColScrollRegions = 1
        ugNets.DisplayLayout.MaxRowScrollRegions = 1
        Appearance149.BackColor = SystemColors.Window
        Appearance149.ForeColor = SystemColors.ControlText
        ugNets.DisplayLayout.Override.ActiveCellAppearance = Appearance149
        Appearance150.BackColor = SystemColors.Highlight
        Appearance150.ForeColor = SystemColors.HighlightText
        ugNets.DisplayLayout.Override.ActiveRowAppearance = Appearance150
        ugNets.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted
        ugNets.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted
        Appearance151.BackColor = SystemColors.Window
        ugNets.DisplayLayout.Override.CardAreaAppearance = Appearance151
        Appearance152.BorderColor = Color.Silver
        Appearance152.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter
        ugNets.DisplayLayout.Override.CellAppearance = Appearance152
        ugNets.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText
        ugNets.DisplayLayout.Override.CellPadding = 0
        Appearance153.BackColor = SystemColors.Control
        Appearance153.BackColor2 = SystemColors.ControlDark
        Appearance153.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element
        Appearance153.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal
        Appearance153.BorderColor = SystemColors.Window
        ugNets.DisplayLayout.Override.GroupByRowAppearance = Appearance153
        resources.ApplyResources(Appearance154, "Appearance154")
        Appearance154.ForceApplyResources = ""
        ugNets.DisplayLayout.Override.HeaderAppearance = Appearance154
        ugNets.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti
        ugNets.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand
        Appearance155.BackColor = SystemColors.Window
        Appearance155.BorderColor = Color.Silver
        ugNets.DisplayLayout.Override.RowAppearance = Appearance155
        ugNets.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False
        Appearance156.BackColor = SystemColors.ControlLight
        ugNets.DisplayLayout.Override.TemplateAddRowAppearance = Appearance156
        ugNets.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill
        ugNets.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate
        ugNets.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy
        resources.ApplyResources(ugNets, "ugNets")
        ugNets.Name = "ugNets"
        ' 
        ' utpcApprovals
        ' 
        utpcApprovals.Controls.Add(ugApprovals)
        resources.ApplyResources(utpcApprovals, "utpcApprovals")
        utpcApprovals.Name = "utpcApprovals"
        ' 
        ' ugApprovals
        ' 
        Appearance157.BackColor = SystemColors.Window
        Appearance157.BorderColor = SystemColors.InactiveCaption
        ugApprovals.DisplayLayout.Appearance = Appearance157
        ugApprovals.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid
        ugApprovals.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False
        Appearance158.BackColor = SystemColors.ActiveBorder
        Appearance158.BackColor2 = SystemColors.ControlDark
        Appearance158.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical
        Appearance158.BorderColor = SystemColors.Window
        ugApprovals.DisplayLayout.GroupByBox.Appearance = Appearance158
        Appearance159.ForeColor = SystemColors.GrayText
        ugApprovals.DisplayLayout.GroupByBox.BandLabelAppearance = Appearance159
        ugApprovals.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid
        Appearance160.BackColor = SystemColors.ControlLightLight
        Appearance160.BackColor2 = SystemColors.Control
        Appearance160.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal
        Appearance160.ForeColor = SystemColors.GrayText
        ugApprovals.DisplayLayout.GroupByBox.PromptAppearance = Appearance160
        ugApprovals.DisplayLayout.MaxColScrollRegions = 1
        ugApprovals.DisplayLayout.MaxRowScrollRegions = 1
        Appearance161.BackColor = SystemColors.Window
        Appearance161.ForeColor = SystemColors.ControlText
        ugApprovals.DisplayLayout.Override.ActiveCellAppearance = Appearance161
        Appearance162.BackColor = SystemColors.Highlight
        Appearance162.ForeColor = SystemColors.HighlightText
        ugApprovals.DisplayLayout.Override.ActiveRowAppearance = Appearance162
        ugApprovals.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted
        ugApprovals.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted
        Appearance163.BackColor = SystemColors.Window
        ugApprovals.DisplayLayout.Override.CardAreaAppearance = Appearance163
        Appearance164.BorderColor = Color.Silver
        Appearance164.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter
        ugApprovals.DisplayLayout.Override.CellAppearance = Appearance164
        ugApprovals.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText
        ugApprovals.DisplayLayout.Override.CellPadding = 0
        Appearance165.BackColor = SystemColors.Control
        Appearance165.BackColor2 = SystemColors.ControlDark
        Appearance165.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element
        Appearance165.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal
        Appearance165.BorderColor = SystemColors.Window
        ugApprovals.DisplayLayout.Override.GroupByRowAppearance = Appearance165
        resources.ApplyResources(Appearance166, "Appearance166")
        Appearance166.ForceApplyResources = ""
        ugApprovals.DisplayLayout.Override.HeaderAppearance = Appearance166
        ugApprovals.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti
        ugApprovals.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand
        Appearance167.BackColor = SystemColors.Window
        Appearance167.BorderColor = Color.Silver
        ugApprovals.DisplayLayout.Override.RowAppearance = Appearance167
        ugApprovals.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False
        Appearance168.BackColor = SystemColors.ControlLight
        ugApprovals.DisplayLayout.Override.TemplateAddRowAppearance = Appearance168
        ugApprovals.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill
        ugApprovals.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate
        ugApprovals.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy
        resources.ApplyResources(ugApprovals, "ugApprovals")
        ugApprovals.Name = "ugApprovals"
        ' 
        ' utpcChangeDescriptions
        ' 
        utpcChangeDescriptions.Controls.Add(ugChangeDescriptions)
        resources.ApplyResources(utpcChangeDescriptions, "utpcChangeDescriptions")
        utpcChangeDescriptions.Name = "utpcChangeDescriptions"
        ' 
        ' ugChangeDescriptions
        ' 
        Appearance169.BackColor = SystemColors.Window
        Appearance169.BorderColor = SystemColors.InactiveCaption
        ugChangeDescriptions.DisplayLayout.Appearance = Appearance169
        ugChangeDescriptions.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid
        ugChangeDescriptions.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False
        Appearance170.BackColor = SystemColors.ActiveBorder
        Appearance170.BackColor2 = SystemColors.ControlDark
        Appearance170.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical
        Appearance170.BorderColor = SystemColors.Window
        ugChangeDescriptions.DisplayLayout.GroupByBox.Appearance = Appearance170
        Appearance171.ForeColor = SystemColors.GrayText
        ugChangeDescriptions.DisplayLayout.GroupByBox.BandLabelAppearance = Appearance171
        ugChangeDescriptions.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid
        Appearance172.BackColor = SystemColors.ControlLightLight
        Appearance172.BackColor2 = SystemColors.Control
        Appearance172.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal
        Appearance172.ForeColor = SystemColors.GrayText
        ugChangeDescriptions.DisplayLayout.GroupByBox.PromptAppearance = Appearance172
        ugChangeDescriptions.DisplayLayout.MaxColScrollRegions = 1
        ugChangeDescriptions.DisplayLayout.MaxRowScrollRegions = 1
        Appearance173.BackColor = SystemColors.Window
        Appearance173.ForeColor = SystemColors.ControlText
        ugChangeDescriptions.DisplayLayout.Override.ActiveCellAppearance = Appearance173
        Appearance174.BackColor = SystemColors.Highlight
        Appearance174.ForeColor = SystemColors.HighlightText
        ugChangeDescriptions.DisplayLayout.Override.ActiveRowAppearance = Appearance174
        ugChangeDescriptions.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted
        ugChangeDescriptions.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted
        Appearance175.BackColor = SystemColors.Window
        ugChangeDescriptions.DisplayLayout.Override.CardAreaAppearance = Appearance175
        Appearance176.BorderColor = Color.Silver
        Appearance176.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter
        ugChangeDescriptions.DisplayLayout.Override.CellAppearance = Appearance176
        ugChangeDescriptions.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText
        ugChangeDescriptions.DisplayLayout.Override.CellPadding = 0
        Appearance177.BackColor = SystemColors.Control
        Appearance177.BackColor2 = SystemColors.ControlDark
        Appearance177.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element
        Appearance177.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal
        Appearance177.BorderColor = SystemColors.Window
        ugChangeDescriptions.DisplayLayout.Override.GroupByRowAppearance = Appearance177
        resources.ApplyResources(Appearance178, "Appearance178")
        Appearance178.ForceApplyResources = ""
        ugChangeDescriptions.DisplayLayout.Override.HeaderAppearance = Appearance178
        ugChangeDescriptions.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti
        ugChangeDescriptions.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand
        Appearance179.BackColor = SystemColors.Window
        Appearance179.BorderColor = Color.Silver
        ugChangeDescriptions.DisplayLayout.Override.RowAppearance = Appearance179
        ugChangeDescriptions.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False
        Appearance180.BackColor = SystemColors.ControlLight
        ugChangeDescriptions.DisplayLayout.Override.TemplateAddRowAppearance = Appearance180
        ugChangeDescriptions.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill
        ugChangeDescriptions.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate
        ugChangeDescriptions.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy
        resources.ApplyResources(ugChangeDescriptions, "ugChangeDescriptions")
        ugChangeDescriptions.Name = "ugChangeDescriptions"
        ' 
        ' utpcCoPacks
        ' 
        utpcCoPacks.Controls.Add(ugCoPacks)
        resources.ApplyResources(utpcCoPacks, "utpcCoPacks")
        utpcCoPacks.Name = "utpcCoPacks"
        ' 
        ' ugCoPacks
        ' 
        Appearance181.BackColor = SystemColors.Window
        Appearance181.BorderColor = SystemColors.InactiveCaption
        ugCoPacks.DisplayLayout.Appearance = Appearance181
        ugCoPacks.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid
        ugCoPacks.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False
        Appearance182.BackColor = SystemColors.ActiveBorder
        Appearance182.BackColor2 = SystemColors.ControlDark
        Appearance182.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical
        Appearance182.BorderColor = SystemColors.Window
        ugCoPacks.DisplayLayout.GroupByBox.Appearance = Appearance182
        Appearance183.ForeColor = SystemColors.GrayText
        ugCoPacks.DisplayLayout.GroupByBox.BandLabelAppearance = Appearance183
        ugCoPacks.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid
        Appearance184.BackColor = SystemColors.ControlLightLight
        Appearance184.BackColor2 = SystemColors.Control
        Appearance184.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal
        Appearance184.ForeColor = SystemColors.GrayText
        ugCoPacks.DisplayLayout.GroupByBox.PromptAppearance = Appearance184
        ugCoPacks.DisplayLayout.MaxColScrollRegions = 1
        ugCoPacks.DisplayLayout.MaxRowScrollRegions = 1
        Appearance185.BackColor = SystemColors.Window
        Appearance185.ForeColor = SystemColors.ControlText
        ugCoPacks.DisplayLayout.Override.ActiveCellAppearance = Appearance185
        Appearance186.BackColor = SystemColors.Highlight
        Appearance186.ForeColor = SystemColors.HighlightText
        ugCoPacks.DisplayLayout.Override.ActiveRowAppearance = Appearance186
        ugCoPacks.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted
        ugCoPacks.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted
        Appearance187.BackColor = SystemColors.Window
        ugCoPacks.DisplayLayout.Override.CardAreaAppearance = Appearance187
        Appearance188.BorderColor = Color.Silver
        Appearance188.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter
        ugCoPacks.DisplayLayout.Override.CellAppearance = Appearance188
        ugCoPacks.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText
        ugCoPacks.DisplayLayout.Override.CellPadding = 0
        Appearance189.BackColor = SystemColors.Control
        Appearance189.BackColor2 = SystemColors.ControlDark
        Appearance189.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element
        Appearance189.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal
        Appearance189.BorderColor = SystemColors.Window
        ugCoPacks.DisplayLayout.Override.GroupByRowAppearance = Appearance189
        resources.ApplyResources(Appearance190, "Appearance190")
        Appearance190.ForceApplyResources = ""
        ugCoPacks.DisplayLayout.Override.HeaderAppearance = Appearance190
        ugCoPacks.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti
        ugCoPacks.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand
        Appearance191.BackColor = SystemColors.Window
        Appearance191.BorderColor = Color.Silver
        ugCoPacks.DisplayLayout.Override.RowAppearance = Appearance191
        ugCoPacks.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False
        Appearance192.BackColor = SystemColors.ControlLight
        ugCoPacks.DisplayLayout.Override.TemplateAddRowAppearance = Appearance192
        ugCoPacks.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill
        ugCoPacks.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate
        ugCoPacks.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy
        resources.ApplyResources(ugCoPacks, "ugCoPacks")
        ugCoPacks.Name = "ugCoPacks"
        ' 
        ' utpcDefDimSpecs
        ' 
        utpcDefDimSpecs.Controls.Add(ugDefDimSpecs)
        resources.ApplyResources(utpcDefDimSpecs, "utpcDefDimSpecs")
        utpcDefDimSpecs.Name = "utpcDefDimSpecs"
        ' 
        ' ugDefDimSpecs
        ' 
        Appearance193.BackColor = SystemColors.Window
        Appearance193.BorderColor = SystemColors.InactiveCaption
        ugDefDimSpecs.DisplayLayout.Appearance = Appearance193
        ugDefDimSpecs.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid
        ugDefDimSpecs.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False
        Appearance194.BackColor = SystemColors.ActiveBorder
        Appearance194.BackColor2 = SystemColors.ControlDark
        Appearance194.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical
        Appearance194.BorderColor = SystemColors.Window
        ugDefDimSpecs.DisplayLayout.GroupByBox.Appearance = Appearance194
        Appearance195.ForeColor = SystemColors.GrayText
        ugDefDimSpecs.DisplayLayout.GroupByBox.BandLabelAppearance = Appearance195
        ugDefDimSpecs.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid
        Appearance196.BackColor = SystemColors.ControlLightLight
        Appearance196.BackColor2 = SystemColors.Control
        Appearance196.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal
        Appearance196.ForeColor = SystemColors.GrayText
        ugDefDimSpecs.DisplayLayout.GroupByBox.PromptAppearance = Appearance196
        ugDefDimSpecs.DisplayLayout.MaxColScrollRegions = 1
        ugDefDimSpecs.DisplayLayout.MaxRowScrollRegions = 1
        Appearance197.BackColor = SystemColors.Window
        Appearance197.ForeColor = SystemColors.ControlText
        ugDefDimSpecs.DisplayLayout.Override.ActiveCellAppearance = Appearance197
        Appearance198.BackColor = SystemColors.Highlight
        Appearance198.ForeColor = SystemColors.HighlightText
        ugDefDimSpecs.DisplayLayout.Override.ActiveRowAppearance = Appearance198
        ugDefDimSpecs.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted
        ugDefDimSpecs.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted
        Appearance199.BackColor = SystemColors.Window
        ugDefDimSpecs.DisplayLayout.Override.CardAreaAppearance = Appearance199
        Appearance200.BorderColor = Color.Silver
        Appearance200.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter
        ugDefDimSpecs.DisplayLayout.Override.CellAppearance = Appearance200
        ugDefDimSpecs.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText
        ugDefDimSpecs.DisplayLayout.Override.CellPadding = 0
        Appearance201.BackColor = SystemColors.Control
        Appearance201.BackColor2 = SystemColors.ControlDark
        Appearance201.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element
        Appearance201.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal
        Appearance201.BorderColor = SystemColors.Window
        ugDefDimSpecs.DisplayLayout.Override.GroupByRowAppearance = Appearance201
        resources.ApplyResources(Appearance202, "Appearance202")
        Appearance202.ForceApplyResources = ""
        ugDefDimSpecs.DisplayLayout.Override.HeaderAppearance = Appearance202
        ugDefDimSpecs.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti
        ugDefDimSpecs.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand
        Appearance203.BackColor = SystemColors.Window
        Appearance203.BorderColor = Color.Silver
        ugDefDimSpecs.DisplayLayout.Override.RowAppearance = Appearance203
        ugDefDimSpecs.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False
        Appearance204.BackColor = SystemColors.ControlLight
        ugDefDimSpecs.DisplayLayout.Override.TemplateAddRowAppearance = Appearance204
        ugDefDimSpecs.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill
        ugDefDimSpecs.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate
        ugDefDimSpecs.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy
        resources.ApplyResources(ugDefDimSpecs, "ugDefDimSpecs")
        ugDefDimSpecs.Name = "ugDefDimSpecs"
        ' 
        ' utpcDimSpecs
        ' 
        utpcDimSpecs.Controls.Add(ugDimSpecs)
        resources.ApplyResources(utpcDimSpecs, "utpcDimSpecs")
        utpcDimSpecs.Name = "utpcDimSpecs"
        ' 
        ' ugDimSpecs
        ' 
        Appearance205.BackColor = SystemColors.Window
        Appearance205.BorderColor = SystemColors.InactiveCaption
        ugDimSpecs.DisplayLayout.Appearance = Appearance205
        ugDimSpecs.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid
        ugDimSpecs.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False
        Appearance206.BackColor = SystemColors.ActiveBorder
        Appearance206.BackColor2 = SystemColors.ControlDark
        Appearance206.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical
        Appearance206.BorderColor = SystemColors.Window
        ugDimSpecs.DisplayLayout.GroupByBox.Appearance = Appearance206
        Appearance207.ForeColor = SystemColors.GrayText
        ugDimSpecs.DisplayLayout.GroupByBox.BandLabelAppearance = Appearance207
        ugDimSpecs.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid
        Appearance208.BackColor = SystemColors.ControlLightLight
        Appearance208.BackColor2 = SystemColors.Control
        Appearance208.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal
        Appearance208.ForeColor = SystemColors.GrayText
        ugDimSpecs.DisplayLayout.GroupByBox.PromptAppearance = Appearance208
        ugDimSpecs.DisplayLayout.MaxColScrollRegions = 1
        ugDimSpecs.DisplayLayout.MaxRowScrollRegions = 1
        Appearance209.BackColor = SystemColors.Window
        Appearance209.ForeColor = SystemColors.ControlText
        ugDimSpecs.DisplayLayout.Override.ActiveCellAppearance = Appearance209
        Appearance210.BackColor = SystemColors.Highlight
        Appearance210.ForeColor = SystemColors.HighlightText
        ugDimSpecs.DisplayLayout.Override.ActiveRowAppearance = Appearance210
        ugDimSpecs.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted
        ugDimSpecs.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted
        Appearance211.BackColor = SystemColors.Window
        ugDimSpecs.DisplayLayout.Override.CardAreaAppearance = Appearance211
        Appearance212.BorderColor = Color.Silver
        Appearance212.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter
        ugDimSpecs.DisplayLayout.Override.CellAppearance = Appearance212
        ugDimSpecs.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText
        ugDimSpecs.DisplayLayout.Override.CellPadding = 0
        Appearance213.BackColor = SystemColors.Control
        Appearance213.BackColor2 = SystemColors.ControlDark
        Appearance213.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element
        Appearance213.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal
        Appearance213.BorderColor = SystemColors.Window
        ugDimSpecs.DisplayLayout.Override.GroupByRowAppearance = Appearance213
        resources.ApplyResources(Appearance214, "Appearance214")
        Appearance214.ForceApplyResources = ""
        ugDimSpecs.DisplayLayout.Override.HeaderAppearance = Appearance214
        ugDimSpecs.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti
        ugDimSpecs.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand
        Appearance215.BackColor = SystemColors.Window
        Appearance215.BorderColor = Color.Silver
        ugDimSpecs.DisplayLayout.Override.RowAppearance = Appearance215
        ugDimSpecs.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False
        Appearance216.BackColor = SystemColors.ControlLight
        ugDimSpecs.DisplayLayout.Override.TemplateAddRowAppearance = Appearance216
        ugDimSpecs.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill
        ugDimSpecs.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate
        ugDimSpecs.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy
        resources.ApplyResources(ugDimSpecs, "ugDimSpecs")
        ugDimSpecs.Name = "ugDimSpecs"
        ' 
        ' utpcRedlinings
        ' 
        utpcRedlinings.Controls.Add(ugRedlinings)
        resources.ApplyResources(utpcRedlinings, "utpcRedlinings")
        utpcRedlinings.Name = "utpcRedlinings"
        ' 
        ' ugRedlinings
        ' 
        Appearance217.BackColor = SystemColors.Window
        Appearance217.BorderColor = SystemColors.InactiveCaption
        ugRedlinings.DisplayLayout.Appearance = Appearance217
        ugRedlinings.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid
        ugRedlinings.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False
        Appearance218.BackColor = SystemColors.ActiveBorder
        Appearance218.BackColor2 = SystemColors.ControlDark
        Appearance218.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical
        Appearance218.BorderColor = SystemColors.Window
        ugRedlinings.DisplayLayout.GroupByBox.Appearance = Appearance218
        Appearance219.ForeColor = SystemColors.GrayText
        ugRedlinings.DisplayLayout.GroupByBox.BandLabelAppearance = Appearance219
        ugRedlinings.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid
        Appearance220.BackColor = SystemColors.ControlLightLight
        Appearance220.BackColor2 = SystemColors.Control
        Appearance220.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal
        Appearance220.ForeColor = SystemColors.GrayText
        ugRedlinings.DisplayLayout.GroupByBox.PromptAppearance = Appearance220
        ugRedlinings.DisplayLayout.MaxColScrollRegions = 1
        ugRedlinings.DisplayLayout.MaxRowScrollRegions = 1
        Appearance221.BackColor = SystemColors.Window
        Appearance221.ForeColor = SystemColors.ControlText
        ugRedlinings.DisplayLayout.Override.ActiveCellAppearance = Appearance221
        Appearance222.BackColor = SystemColors.Highlight
        Appearance222.ForeColor = SystemColors.HighlightText
        ugRedlinings.DisplayLayout.Override.ActiveRowAppearance = Appearance222
        ugRedlinings.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted
        ugRedlinings.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted
        Appearance223.BackColor = SystemColors.Window
        ugRedlinings.DisplayLayout.Override.CardAreaAppearance = Appearance223
        Appearance224.BorderColor = Color.Silver
        Appearance224.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter
        ugRedlinings.DisplayLayout.Override.CellAppearance = Appearance224
        ugRedlinings.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText
        ugRedlinings.DisplayLayout.Override.CellPadding = 0
        Appearance225.BackColor = SystemColors.Control
        Appearance225.BackColor2 = SystemColors.ControlDark
        Appearance225.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element
        Appearance225.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal
        Appearance225.BorderColor = SystemColors.Window
        ugRedlinings.DisplayLayout.Override.GroupByRowAppearance = Appearance225
        resources.ApplyResources(Appearance226, "Appearance226")
        Appearance226.ForceApplyResources = ""
        ugRedlinings.DisplayLayout.Override.HeaderAppearance = Appearance226
        ugRedlinings.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti
        ugRedlinings.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand
        Appearance227.BackColor = SystemColors.Window
        Appearance227.BorderColor = Color.Silver
        ugRedlinings.DisplayLayout.Override.RowAppearance = Appearance227
        ugRedlinings.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False
        Appearance228.BackColor = SystemColors.ControlLight
        ugRedlinings.DisplayLayout.Override.TemplateAddRowAppearance = Appearance228
        ugRedlinings.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill
        ugRedlinings.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate
        ugRedlinings.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy
        resources.ApplyResources(ugRedlinings, "ugRedlinings")
        ugRedlinings.Name = "ugRedlinings"
        ' 
        ' utpcQMStamps
        ' 
        utpcQMStamps.Controls.Add(ugQMStamps)
        resources.ApplyResources(utpcQMStamps, "utpcQMStamps")
        utpcQMStamps.Name = "utpcQMStamps"
        ' 
        ' ugQMStamps
        ' 
        Appearance229.BackColor = SystemColors.Window
        Appearance229.BorderColor = SystemColors.InactiveCaption
        ugQMStamps.DisplayLayout.Appearance = Appearance229
        ugQMStamps.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid
        ugQMStamps.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False
        Appearance230.BackColor = SystemColors.ActiveBorder
        Appearance230.BackColor2 = SystemColors.ControlDark
        Appearance230.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical
        Appearance230.BorderColor = SystemColors.Window
        ugQMStamps.DisplayLayout.GroupByBox.Appearance = Appearance230
        Appearance231.ForeColor = SystemColors.GrayText
        ugQMStamps.DisplayLayout.GroupByBox.BandLabelAppearance = Appearance231
        ugQMStamps.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid
        Appearance232.BackColor = SystemColors.ControlLightLight
        Appearance232.BackColor2 = SystemColors.Control
        Appearance232.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal
        Appearance232.ForeColor = SystemColors.GrayText
        ugQMStamps.DisplayLayout.GroupByBox.PromptAppearance = Appearance232
        ugQMStamps.DisplayLayout.MaxColScrollRegions = 1
        ugQMStamps.DisplayLayout.MaxRowScrollRegions = 1
        Appearance233.BackColor = SystemColors.Window
        Appearance233.ForeColor = SystemColors.ControlText
        ugQMStamps.DisplayLayout.Override.ActiveCellAppearance = Appearance233
        Appearance234.BackColor = SystemColors.Highlight
        Appearance234.ForeColor = SystemColors.HighlightText
        ugQMStamps.DisplayLayout.Override.ActiveRowAppearance = Appearance234
        ugQMStamps.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted
        ugQMStamps.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted
        Appearance235.BackColor = SystemColors.Window
        ugQMStamps.DisplayLayout.Override.CardAreaAppearance = Appearance235
        Appearance236.BorderColor = Color.Silver
        Appearance236.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter
        ugQMStamps.DisplayLayout.Override.CellAppearance = Appearance236
        ugQMStamps.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText
        ugQMStamps.DisplayLayout.Override.CellPadding = 0
        Appearance237.BackColor = SystemColors.Control
        Appearance237.BackColor2 = SystemColors.ControlDark
        Appearance237.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element
        Appearance237.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal
        Appearance237.BorderColor = SystemColors.Window
        ugQMStamps.DisplayLayout.Override.GroupByRowAppearance = Appearance237
        ugQMStamps.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti
        ugQMStamps.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand
        Appearance238.BackColor = SystemColors.Window
        Appearance238.BorderColor = Color.Silver
        ugQMStamps.DisplayLayout.Override.RowAppearance = Appearance238
        ugQMStamps.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False
        Appearance239.BackColor = SystemColors.ControlLight
        ugQMStamps.DisplayLayout.Override.TemplateAddRowAppearance = Appearance239
        ugQMStamps.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill
        ugQMStamps.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate
        ugQMStamps.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy
        resources.ApplyResources(ugQMStamps, "ugQMStamps")
        ugQMStamps.Name = "ugQMStamps"
        ' 
        ' utpcDifferences
        ' 
        utpcDifferences.Controls.Add(ugDifferences)
        resources.ApplyResources(utpcDifferences, "utpcDifferences")
        utpcDifferences.Name = "utpcDifferences"
        ' 
        ' ugDifferences
        ' 
        ugDifferences.DataSource = ColumnViewBindingSource
        UltraGridBand1.ColHeadersVisible = False
        resources.ApplyResources(UltraGridColumn1.Header, "UltraGridColumn1.Header")
        UltraGridColumn1.Header.VisiblePosition = 0
        UltraGridColumn1.ForceApplyResources = "Header"
        UltraGridColumn2.Header.VisiblePosition = 1
        UltraGridBand1.Columns.AddRange(New Object() {UltraGridColumn1, UltraGridColumn2})
        resources.ApplyResources(UltraGridColumn3.Header, "UltraGridColumn3.Header")
        UltraGridColumn3.Header.VisiblePosition = 0
        UltraGridColumn3.ForceApplyResources = "Header"
        UltraGridColumn4.Header.VisiblePosition = 1
        resources.ApplyResources(UltraGridColumn4.Header, "UltraGridColumn4.Header")
        UltraGridColumn4.ForceApplyResources = "Header"
        UltraGridColumn5.Header.VisiblePosition = 2
        resources.ApplyResources(UltraGridColumn5.Header, "UltraGridColumn5.Header")
        UltraGridColumn5.ForceApplyResources = "Header"
        UltraGridBand2.Columns.AddRange(New Object() {UltraGridColumn3, UltraGridColumn4, UltraGridColumn5})
        ugDifferences.DisplayLayout.BandsSerializer.Add(UltraGridBand1)
        ugDifferences.DisplayLayout.BandsSerializer.Add(UltraGridBand2)
        ugDifferences.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False
        resources.ApplyResources(ugDifferences, "ugDifferences")
        ugDifferences.Name = "ugDifferences"
        ' 
        ' ColumnViewBindingSource
        ' 
        ColumnViewBindingSource.DataSource = GetType(Compare.Table.ColumnView)
        ' 
        ' utcInformationHub
        ' 
        utcInformationHub.Controls.Add(utscpInformationHub)
        utcInformationHub.Controls.Add(utpcHarness)
        utcInformationHub.Controls.Add(utpcModules)
        utcInformationHub.Controls.Add(utpcVertices)
        utcInformationHub.Controls.Add(utpcSegments)
        utcInformationHub.Controls.Add(utpcAccessories)
        utcInformationHub.Controls.Add(utpcFixings)
        utcInformationHub.Controls.Add(utpcComponents)
        utcInformationHub.Controls.Add(utpcConnectors)
        utcInformationHub.Controls.Add(utpcCables)
        utcInformationHub.Controls.Add(utpcWires)
        utcInformationHub.Controls.Add(utpcNets)
        utcInformationHub.Controls.Add(utpcRedlinings)
        utcInformationHub.Controls.Add(utpcChangeDescriptions)
        utcInformationHub.Controls.Add(utpcAssemblyParts)
        utcInformationHub.Controls.Add(utpcCoPacks)
        utcInformationHub.Controls.Add(utpcComponentBoxes)
        utcInformationHub.Controls.Add(utpcApprovals)
        utcInformationHub.Controls.Add(utpcQMStamps)
        utcInformationHub.Controls.Add(utpcDifferences)
        utcInformationHub.Controls.Add(utpcDefDimSpecs)
        utcInformationHub.Controls.Add(utpcDimSpecs)
        resources.ApplyResources(utcInformationHub, "utcInformationHub")
        utcInformationHub.Name = "utcInformationHub"
        utcInformationHub.SharedControlsPage = utscpInformationHub
        UltraTab1.Key = "tabHarness"
        UltraTab1.TabPage = utpcHarness
        resources.ApplyResources(UltraTab1, "UltraTab1")
        UltraTab1.ForceApplyResources = ""
        UltraTab2.Key = "tabModules"
        UltraTab2.TabPage = utpcModules
        resources.ApplyResources(UltraTab2, "UltraTab2")
        UltraTab2.ForceApplyResources = ""
        UltraTab3.Key = "tabVertices"
        UltraTab3.TabPage = utpcVertices
        resources.ApplyResources(UltraTab3, "UltraTab3")
        UltraTab3.ForceApplyResources = ""
        UltraTab4.Key = "tabSegments"
        UltraTab4.TabPage = utpcSegments
        resources.ApplyResources(UltraTab4, "UltraTab4")
        UltraTab4.ForceApplyResources = ""
        UltraTab5.Key = "tabAccessories"
        UltraTab5.TabPage = utpcAccessories
        resources.ApplyResources(UltraTab5, "UltraTab5")
        UltraTab5.ForceApplyResources = ""
        UltraTab14.Key = "tabAssemblyParts"
        UltraTab14.TabPage = utpcAssemblyParts
        resources.ApplyResources(UltraTab14, "UltraTab14")
        UltraTab14.ForceApplyResources = ""
        UltraTab6.Key = "tabFixings"
        UltraTab6.TabPage = utpcFixings
        resources.ApplyResources(UltraTab6, "UltraTab6")
        UltraTab6.ForceApplyResources = ""
        UltraTab16.Key = "tabComponentBoxes"
        UltraTab16.TabPage = utpcComponentBoxes
        resources.ApplyResources(UltraTab16, "UltraTab16")
        UltraTab16.ForceApplyResources = ""
        UltraTab7.Key = "tabComponents"
        UltraTab7.TabPage = utpcComponents
        resources.ApplyResources(UltraTab7, "UltraTab7")
        UltraTab7.ForceApplyResources = ""
        UltraTab8.Key = "tabConnectors"
        UltraTab8.TabPage = utpcConnectors
        resources.ApplyResources(UltraTab8, "UltraTab8")
        UltraTab8.ForceApplyResources = ""
        UltraTab9.Key = "tabCables"
        UltraTab9.TabPage = utpcCables
        resources.ApplyResources(UltraTab9, "UltraTab9")
        UltraTab9.ForceApplyResources = ""
        UltraTab10.Key = "tabWires"
        UltraTab10.TabPage = utpcWires
        resources.ApplyResources(UltraTab10, "UltraTab10")
        UltraTab10.ForceApplyResources = ""
        UltraTab11.Key = "tabNets"
        UltraTab11.TabPage = utpcNets
        resources.ApplyResources(UltraTab11, "UltraTab11")
        UltraTab11.ForceApplyResources = ""
        UltraTab17.Key = "tabApprovals"
        UltraTab17.TabPage = utpcApprovals
        resources.ApplyResources(UltraTab17, "UltraTab17")
        UltraTab17.ForceApplyResources = ""
        UltraTab13.Key = "tabChangeDescriptions"
        UltraTab13.TabPage = utpcChangeDescriptions
        resources.ApplyResources(UltraTab13, "UltraTab13")
        UltraTab13.ForceApplyResources = ""
        UltraTab15.Key = "tabCoPacks"
        UltraTab15.TabPage = utpcCoPacks
        resources.ApplyResources(UltraTab15, "UltraTab15")
        UltraTab15.ForceApplyResources = ""
        UltraTab20.Key = "tabDefDimSpecs"
        UltraTab20.TabPage = utpcDefDimSpecs
        resources.ApplyResources(UltraTab20, "UltraTab20")
        UltraTab20.ForceApplyResources = ""
        UltraTab21.Key = "tabDimSpecs"
        UltraTab21.TabPage = utpcDimSpecs
        resources.ApplyResources(UltraTab21, "UltraTab21")
        UltraTab21.ForceApplyResources = ""
        UltraTab12.Key = "tabRedlinings"
        UltraTab12.TabPage = utpcRedlinings
        resources.ApplyResources(UltraTab12, "UltraTab12")
        UltraTab12.ForceApplyResources = ""
        UltraTab18.Key = "tabQMStamps"
        UltraTab18.TabPage = utpcQMStamps
        resources.ApplyResources(UltraTab18, "UltraTab18")
        UltraTab18.ForceApplyResources = ""
        UltraTab19.Key = "Differences"
        UltraTab19.TabPage = utpcDifferences
        resources.ApplyResources(UltraTab19, "UltraTab19")
        UltraTab19.Visible = False
        UltraTab19.ForceApplyResources = ""
        utcInformationHub.Tabs.AddRange(New Infragistics.Win.UltraWinTabControl.UltraTab() {UltraTab1, UltraTab2, UltraTab3, UltraTab4, UltraTab5, UltraTab14, UltraTab6, UltraTab16, UltraTab7, UltraTab8, UltraTab9, UltraTab10, UltraTab11, UltraTab17, UltraTab13, UltraTab15, UltraTab20, UltraTab21, UltraTab12, UltraTab18, UltraTab19})
        ' 
        ' utscpInformationHub
        ' 
        resources.ApplyResources(utscpInformationHub, "utscpInformationHub")
        utscpInformationHub.Name = "utscpInformationHub"
        ' 
        ' udsHarness
        ' 
        udsHarness.AllowAdd = False
        udsHarness.AllowDelete = False
        udsHarness.ReadOnly = True
        ' 
        ' udsModules
        ' 
        udsModules.AllowAdd = False
        udsModules.AllowDelete = False
        udsModules.ReadOnly = True
        ' 
        ' udsVertices
        ' 
        udsVertices.AllowAdd = False
        udsVertices.AllowDelete = False
        udsVertices.ReadOnly = True
        ' 
        ' udsSegments
        ' 
        udsSegments.AllowAdd = False
        udsSegments.AllowDelete = False
        udsSegments.ReadOnly = True
        ' 
        ' udsAccessories
        ' 
        udsAccessories.AllowAdd = False
        udsAccessories.AllowDelete = False
        udsAccessories.ReadOnly = True
        ' 
        ' udsFixings
        ' 
        udsFixings.AllowAdd = False
        udsFixings.AllowDelete = False
        udsFixings.ReadOnly = True
        ' 
        ' udsComponents
        ' 
        udsComponents.AllowAdd = False
        udsComponents.AllowDelete = False
        udsComponents.ReadOnly = True
        ' 
        ' udsConnectors
        ' 
        udsConnectors.AllowAdd = False
        udsConnectors.AllowDelete = False
        udsConnectors.ReadOnly = True
        ' 
        ' udsCables
        ' 
        udsCables.AllowAdd = False
        udsCables.AllowDelete = False
        udsCables.ReadOnly = True
        ' 
        ' udsWires
        ' 
        udsWires.AllowAdd = False
        udsWires.AllowDelete = False
        udsWires.ReadOnly = True
        ' 
        ' udsNets
        ' 
        udsNets.AllowAdd = False
        udsNets.AllowDelete = False
        udsNets.ReadOnly = True
        ' 
        ' ugeeInformationHub
        ' 
        ' 
        ' utmInformationHub
        ' 
        utmInformationHub.DesignerFlags = 1
        utmInformationHub.DockWithinContainer = Me
        utmInformationHub.ShowFullMenusDelay = 500
        ' 
        ' _InformationHub_Toolbars_Dock_Area_Left
        ' 
        _InformationHub_Toolbars_Dock_Area_Left.AccessibleRole = AccessibleRole.Grouping
        _InformationHub_Toolbars_Dock_Area_Left.BackColor = SystemColors.Control
        _InformationHub_Toolbars_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Left
        _InformationHub_Toolbars_Dock_Area_Left.ForeColor = SystemColors.ControlText
        resources.ApplyResources(_InformationHub_Toolbars_Dock_Area_Left, "_InformationHub_Toolbars_Dock_Area_Left")
        _InformationHub_Toolbars_Dock_Area_Left.Name = "_InformationHub_Toolbars_Dock_Area_Left"
        _InformationHub_Toolbars_Dock_Area_Left.ToolbarsManager = utmInformationHub
        ' 
        ' _InformationHub_Toolbars_Dock_Area_Right
        ' 
        _InformationHub_Toolbars_Dock_Area_Right.AccessibleRole = AccessibleRole.Grouping
        _InformationHub_Toolbars_Dock_Area_Right.BackColor = SystemColors.Control
        _InformationHub_Toolbars_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Right
        _InformationHub_Toolbars_Dock_Area_Right.ForeColor = SystemColors.ControlText
        resources.ApplyResources(_InformationHub_Toolbars_Dock_Area_Right, "_InformationHub_Toolbars_Dock_Area_Right")
        _InformationHub_Toolbars_Dock_Area_Right.Name = "_InformationHub_Toolbars_Dock_Area_Right"
        _InformationHub_Toolbars_Dock_Area_Right.ToolbarsManager = utmInformationHub
        ' 
        ' _InformationHub_Toolbars_Dock_Area_Top
        ' 
        _InformationHub_Toolbars_Dock_Area_Top.AccessibleRole = AccessibleRole.Grouping
        _InformationHub_Toolbars_Dock_Area_Top.BackColor = SystemColors.Control
        _InformationHub_Toolbars_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Top
        _InformationHub_Toolbars_Dock_Area_Top.ForeColor = SystemColors.ControlText
        resources.ApplyResources(_InformationHub_Toolbars_Dock_Area_Top, "_InformationHub_Toolbars_Dock_Area_Top")
        _InformationHub_Toolbars_Dock_Area_Top.Name = "_InformationHub_Toolbars_Dock_Area_Top"
        _InformationHub_Toolbars_Dock_Area_Top.ToolbarsManager = utmInformationHub
        ' 
        ' _InformationHub_Toolbars_Dock_Area_Bottom
        ' 
        _InformationHub_Toolbars_Dock_Area_Bottom.AccessibleRole = AccessibleRole.Grouping
        _InformationHub_Toolbars_Dock_Area_Bottom.BackColor = SystemColors.Control
        _InformationHub_Toolbars_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Bottom
        _InformationHub_Toolbars_Dock_Area_Bottom.ForeColor = SystemColors.ControlText
        resources.ApplyResources(_InformationHub_Toolbars_Dock_Area_Bottom, "_InformationHub_Toolbars_Dock_Area_Bottom")
        _InformationHub_Toolbars_Dock_Area_Bottom.Name = "_InformationHub_Toolbars_Dock_Area_Bottom"
        _InformationHub_Toolbars_Dock_Area_Bottom.ToolbarsManager = utmInformationHub
        ' 
        ' udsRedlinings
        ' 
        udsRedlinings.AllowAdd = False
        udsRedlinings.AllowDelete = False
        ' 
        ' timerE3Application
        ' 
        timerE3Application.Interval = 2000
        ' 
        ' udsChangeDescriptions
        ' 
        udsChangeDescriptions.AllowAdd = False
        udsChangeDescriptions.AllowDelete = False
        udsChangeDescriptions.ReadOnly = True
        ' 
        ' udsAssemblyParts
        ' 
        udsAssemblyParts.AllowAdd = False
        udsAssemblyParts.AllowDelete = False
        udsAssemblyParts.ReadOnly = True
        ' 
        ' udsCoPacks
        ' 
        udsCoPacks.AllowAdd = False
        udsCoPacks.AllowDelete = False
        udsCoPacks.ReadOnly = True
        ' 
        ' udsComponentBoxes
        ' 
        udsComponentBoxes.AllowAdd = False
        udsComponentBoxes.AllowDelete = False
        udsComponentBoxes.ReadOnly = True
        ' 
        ' udsApprovals
        ' 
        udsApprovals.AllowAdd = False
        udsApprovals.AllowDelete = False
        udsApprovals.ReadOnly = True
        ' 
        ' utchpInformationHub
        ' 
        utchpInformationHub.ContainingControl = Me
        ' 
        ' udsQMStamps
        ' 
        udsQMStamps.AllowAdd = False
        udsQMStamps.AllowDelete = False
        ' 
        ' udsDefDimSpecs
        ' 
        udsDefDimSpecs.AllowAdd = False
        udsDefDimSpecs.AllowDelete = False
        udsDefDimSpecs.ReadOnly = True
        ' 
        ' udsDimSpecs
        ' 
        udsDimSpecs.AllowAdd = False
        udsDimSpecs.AllowDelete = False
        udsDimSpecs.ReadOnly = True
        ' 
        ' InformationHub
        ' 
        resources.ApplyResources(Me, "$this")
        AutoScaleMode = AutoScaleMode.Font
        Controls.Add(utcInformationHub)
        Controls.Add(_InformationHub_Toolbars_Dock_Area_Left)
        Controls.Add(_InformationHub_Toolbars_Dock_Area_Right)
        Controls.Add(_InformationHub_Toolbars_Dock_Area_Bottom)
        Controls.Add(_InformationHub_Toolbars_Dock_Area_Top)
        Name = "InformationHub"
        utpcHarness.ResumeLayout(False)
        CType(ugHarness, ComponentModel.ISupportInitialize).EndInit()
        utpcModules.ResumeLayout(False)
        CType(ugModules, ComponentModel.ISupportInitialize).EndInit()
        utpcVertices.ResumeLayout(False)
        CType(ugVertices, ComponentModel.ISupportInitialize).EndInit()
        utpcSegments.ResumeLayout(False)
        CType(ugSegments, ComponentModel.ISupportInitialize).EndInit()
        utpcAccessories.ResumeLayout(False)
        CType(ugAccessories, ComponentModel.ISupportInitialize).EndInit()
        utpcAssemblyParts.ResumeLayout(False)
        CType(ugAssemblyParts, ComponentModel.ISupportInitialize).EndInit()
        utpcFixings.ResumeLayout(False)
        CType(ugFixings, ComponentModel.ISupportInitialize).EndInit()
        utpcComponentBoxes.ResumeLayout(False)
        CType(ugComponentBoxes, ComponentModel.ISupportInitialize).EndInit()
        utpcComponents.ResumeLayout(False)
        CType(ugComponents, ComponentModel.ISupportInitialize).EndInit()
        utpcConnectors.ResumeLayout(False)
        CType(ugConnectors, ComponentModel.ISupportInitialize).EndInit()
        utpcCables.ResumeLayout(False)
        CType(ugCables, ComponentModel.ISupportInitialize).EndInit()
        utpcWires.ResumeLayout(False)
        CType(ugWires, ComponentModel.ISupportInitialize).EndInit()
        utpcNets.ResumeLayout(False)
        CType(ugNets, ComponentModel.ISupportInitialize).EndInit()
        utpcApprovals.ResumeLayout(False)
        CType(ugApprovals, ComponentModel.ISupportInitialize).EndInit()
        utpcChangeDescriptions.ResumeLayout(False)
        CType(ugChangeDescriptions, ComponentModel.ISupportInitialize).EndInit()
        utpcCoPacks.ResumeLayout(False)
        CType(ugCoPacks, ComponentModel.ISupportInitialize).EndInit()
        utpcDefDimSpecs.ResumeLayout(False)
        CType(ugDefDimSpecs, ComponentModel.ISupportInitialize).EndInit()
        utpcDimSpecs.ResumeLayout(False)
        CType(ugDimSpecs, ComponentModel.ISupportInitialize).EndInit()
        utpcRedlinings.ResumeLayout(False)
        CType(ugRedlinings, ComponentModel.ISupportInitialize).EndInit()
        utpcQMStamps.ResumeLayout(False)
        CType(ugQMStamps, ComponentModel.ISupportInitialize).EndInit()
        utpcDifferences.ResumeLayout(False)
        CType(ugDifferences, ComponentModel.ISupportInitialize).EndInit()
        CType(ColumnViewBindingSource, ComponentModel.ISupportInitialize).EndInit()
        CType(utcInformationHub, ComponentModel.ISupportInitialize).EndInit()
        utcInformationHub.ResumeLayout(False)
        CType(udsHarness, ComponentModel.ISupportInitialize).EndInit()
        CType(udsModules, ComponentModel.ISupportInitialize).EndInit()
        CType(udsVertices, ComponentModel.ISupportInitialize).EndInit()
        CType(udsSegments, ComponentModel.ISupportInitialize).EndInit()
        CType(udsAccessories, ComponentModel.ISupportInitialize).EndInit()
        CType(udsFixings, ComponentModel.ISupportInitialize).EndInit()
        CType(udsComponents, ComponentModel.ISupportInitialize).EndInit()
        CType(udsConnectors, ComponentModel.ISupportInitialize).EndInit()
        CType(udsCables, ComponentModel.ISupportInitialize).EndInit()
        CType(udsWires, ComponentModel.ISupportInitialize).EndInit()
        CType(udsNets, ComponentModel.ISupportInitialize).EndInit()
        CType(utmInformationHub, ComponentModel.ISupportInitialize).EndInit()
        CType(udsRedlinings, ComponentModel.ISupportInitialize).EndInit()
        CType(udsChangeDescriptions, ComponentModel.ISupportInitialize).EndInit()
        CType(udsAssemblyParts, ComponentModel.ISupportInitialize).EndInit()
        CType(udsCoPacks, ComponentModel.ISupportInitialize).EndInit()
        CType(udsComponentBoxes, ComponentModel.ISupportInitialize).EndInit()
        CType(udsApprovals, ComponentModel.ISupportInitialize).EndInit()
        CType(utchpInformationHub, ComponentModel.ISupportInitialize).EndInit()
        CType(udsQMStamps, ComponentModel.ISupportInitialize).EndInit()
        CType(udsDefDimSpecs, ComponentModel.ISupportInitialize).EndInit()
        CType(udsDimSpecs, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)

    End Sub
    Friend WithEvents utcInformationHub As Infragistics.Win.UltraWinTabControl.UltraTabControl
    Friend WithEvents utscpInformationHub As Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage
    Friend WithEvents utpcHarness As Infragistics.Win.UltraWinTabControl.UltraTabPageControl
    Friend WithEvents utpcModules As Infragistics.Win.UltraWinTabControl.UltraTabPageControl
    Friend WithEvents utpcVertices As Infragistics.Win.UltraWinTabControl.UltraTabPageControl
    Friend WithEvents utpcSegments As Infragistics.Win.UltraWinTabControl.UltraTabPageControl
    Friend WithEvents utpcAccessories As Infragistics.Win.UltraWinTabControl.UltraTabPageControl
    Friend WithEvents utpcFixings As Infragistics.Win.UltraWinTabControl.UltraTabPageControl
    Friend WithEvents utpcComponents As Infragistics.Win.UltraWinTabControl.UltraTabPageControl
    Friend WithEvents utpcConnectors As Infragistics.Win.UltraWinTabControl.UltraTabPageControl
    Friend WithEvents utpcCables As Infragistics.Win.UltraWinTabControl.UltraTabPageControl
    Friend WithEvents utpcWires As Infragistics.Win.UltraWinTabControl.UltraTabPageControl
    Friend WithEvents utpcNets As Infragistics.Win.UltraWinTabControl.UltraTabPageControl
#Region "UltraDataSources"
    Friend WithEvents udsHarness As Infragistics.Win.UltraWinDataSource.UltraDataSource
    Friend WithEvents udsModules As Infragistics.Win.UltraWinDataSource.UltraDataSource
    Friend WithEvents udsVertices As Infragistics.Win.UltraWinDataSource.UltraDataSource
    Friend WithEvents udsSegments As Infragistics.Win.UltraWinDataSource.UltraDataSource
    Friend WithEvents udsAccessories As Infragistics.Win.UltraWinDataSource.UltraDataSource
    Friend WithEvents udsFixings As Infragistics.Win.UltraWinDataSource.UltraDataSource
    Friend WithEvents udsComponents As Infragistics.Win.UltraWinDataSource.UltraDataSource
    Friend WithEvents udsConnectors As Infragistics.Win.UltraWinDataSource.UltraDataSource
    Friend WithEvents udsAssemblyParts As Infragistics.Win.UltraWinDataSource.UltraDataSource
    Friend WithEvents udsChangeDescriptions As Infragistics.Win.UltraWinDataSource.UltraDataSource
    Friend WithEvents udsCoPacks As Infragistics.Win.UltraWinDataSource.UltraDataSource
    Friend WithEvents udsApprovals As Infragistics.Win.UltraWinDataSource.UltraDataSource
    Friend WithEvents udsComponentBoxes As Infragistics.Win.UltraWinDataSource.UltraDataSource
    Friend WithEvents udsDefDimSpecs As Infragistics.Win.UltraWinDataSource.UltraDataSource
    Friend WithEvents udsDimSpecs As Infragistics.Win.UltraWinDataSource.UltraDataSource
    Friend WithEvents udsQMStamps As Infragistics.Win.UltraWinDataSource.UltraDataSource
    Friend WithEvents udsCables As Infragistics.Win.UltraWinDataSource.UltraDataSource
    Friend WithEvents udsWires As Infragistics.Win.UltraWinDataSource.UltraDataSource
    Friend WithEvents udsNets As Infragistics.Win.UltraWinDataSource.UltraDataSource
    Friend WithEvents udsRedlinings As Infragistics.Win.UltraWinDataSource.UltraDataSource
#End Region
    Friend WithEvents ugeeInformationHub As Infragistics.Win.UltraWinGrid.ExcelExport.UltraGridExcelExporter
    Friend WithEvents utmInformationHub As Infragistics.Win.UltraWinToolbars.UltraToolbarsManager
    Friend WithEvents _InformationHub_Toolbars_Dock_Area_Left As Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea
    Friend WithEvents _InformationHub_Toolbars_Dock_Area_Right As Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea
    Friend WithEvents _InformationHub_Toolbars_Dock_Area_Bottom As Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea
    Friend WithEvents _InformationHub_Toolbars_Dock_Area_Top As Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea
    Friend WithEvents utpcRedlinings As Infragistics.Win.UltraWinTabControl.UltraTabPageControl

#Region "UltraGrids"
    Friend WithEvents ugHarness As Infragistics.Win.UltraWinGrid.UltraGrid
    Friend WithEvents ugModules As Infragistics.Win.UltraWinGrid.UltraGrid
    Friend WithEvents ugVertices As Infragistics.Win.UltraWinGrid.UltraGrid
    Friend WithEvents ugSegments As Infragistics.Win.UltraWinGrid.UltraGrid
    Friend WithEvents ugAccessories As Infragistics.Win.UltraWinGrid.UltraGrid
    Friend WithEvents ugFixings As Infragistics.Win.UltraWinGrid.UltraGrid
    Friend WithEvents ugComponents As Infragistics.Win.UltraWinGrid.UltraGrid
    Friend WithEvents ugConnectors As Infragistics.Win.UltraWinGrid.UltraGrid
    Friend WithEvents ugCables As Infragistics.Win.UltraWinGrid.UltraGrid
    Friend WithEvents ugWires As Infragistics.Win.UltraWinGrid.UltraGrid
    Friend WithEvents ugNets As Infragistics.Win.UltraWinGrid.UltraGrid
    Friend WithEvents ugRedlinings As Infragistics.Win.UltraWinGrid.UltraGrid
    Friend WithEvents ugComponentBoxes As Infragistics.Win.UltraWinGrid.UltraGrid
    Friend WithEvents ugAssemblyParts As Infragistics.Win.UltraWinGrid.UltraGrid
    Friend WithEvents ugChangeDescriptions As Infragistics.Win.UltraWinGrid.UltraGrid
    Friend WithEvents ugCoPacks As Infragistics.Win.UltraWinGrid.UltraGrid
    Friend WithEvents ugApprovals As Infragistics.Win.UltraWinGrid.UltraGrid
    Friend WithEvents ugDifferences As Infragistics.Win.UltraWinGrid.UltraGrid
    Friend WithEvents ugDimSpecs As Infragistics.Win.UltraWinGrid.UltraGrid
    Friend WithEvents ugDefDimSpecs As Infragistics.Win.UltraWinGrid.UltraGrid
    Friend WithEvents ugQMStamps As Infragistics.Win.UltraWinGrid.UltraGrid
#End Region
    Friend WithEvents timerE3Application As System.Windows.Forms.Timer
    Friend WithEvents utpcChangeDescriptions As Infragistics.Win.UltraWinTabControl.UltraTabPageControl
    Friend WithEvents utpcAssemblyParts As Infragistics.Win.UltraWinTabControl.UltraTabPageControl
    Friend WithEvents utpcCoPacks As Infragistics.Win.UltraWinTabControl.UltraTabPageControl
    Friend WithEvents utpcComponentBoxes As Infragistics.Win.UltraWinTabControl.UltraTabPageControl
    Friend WithEvents utpcApprovals As Infragistics.Win.UltraWinTabControl.UltraTabPageControl
    Friend WithEvents utchpInformationHub As Infragistics.Win.Touch.UltraTouchProvider
    Friend WithEvents utpcQMStamps As Infragistics.Win.UltraWinTabControl.UltraTabPageControl
    Friend WithEvents ColumnViewBindingSource As BindingSource
    Friend WithEvents ugePropDifferences As Infragistics.Win.UltraWinGrid.ExcelExport.UltraGridExcelExporter
    Friend WithEvents utpcDifferences As Infragistics.Win.UltraWinTabControl.UltraTabPageControl
    Friend WithEvents utpcDefDimSpecs As Infragistics.Win.UltraWinTabControl.UltraTabPageControl
    Friend WithEvents utpcDimSpecs As Infragistics.Win.UltraWinTabControl.UltraTabPageControl
End Class
