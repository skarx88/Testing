Imports devDept.Eyeshot

Namespace D3D.Consolidated.Controls

    Partial Public Class Consolidated3DControl

        Private Sub InitViewport()
            InitClippPlanes()
            AddToolBarButtonEvents()
            InitAdjustModeBoxesToolTips()

            _layout4.HideAllGrids()
            _layout4.Main.Invalidate()
        End Sub

        Private Sub AddToolBarButtonEvents()
            RemoveToolBarButtonEvents()

            For Each button As ToolBarButton In Me.Design3D.ActiveViewport.ToolBar.Buttons
                _toolBarButtons.Add(button.Name, button)
                button.ToolTipText = GetToolBarButtonTTText(button.Name)
                AddHandler button.Click, AddressOf _toolBarButton_Click
            Next
        End Sub

        Private Sub RemoveToolBarButtonEvents()
            If _toolBarButtons IsNot Nothing Then
                For Each button As devDept.Eyeshot.ToolBarButton In _toolBarButtons.Values
                    RemoveHandler button.Click, AddressOf _toolBarButton_Click
                Next
                _toolBarButtons.Clear()
            End If
        End Sub

        Private Sub InitAdjustModeBoxesToolTips()
            Me.UltraToolTipManager2.SetUltraToolTip(uneRotX, New Infragistics.Win.UltraWinToolTip.UltraToolTipInfo(Consolidated3DControlStrings.TT_uneRotX, Infragistics.Win.ToolTipImage.Default, String.Empty, Infragistics.Win.DefaultableBoolean.True))
            Me.UltraToolTipManager2.SetUltraToolTip(uneRotY, New Infragistics.Win.UltraWinToolTip.UltraToolTipInfo(Consolidated3DControlStrings.TT_uneRotY, Infragistics.Win.ToolTipImage.Default, String.Empty, Infragistics.Win.DefaultableBoolean.True))
            Me.UltraToolTipManager2.SetUltraToolTip(uneRotZ, New Infragistics.Win.UltraWinToolTip.UltraToolTipInfo(Consolidated3DControlStrings.TT_uneRotZ, Infragistics.Win.ToolTipImage.Default, String.Empty, Infragistics.Win.DefaultableBoolean.True))
            Me.UltraToolTipManager2.SetUltraToolTip(uneScalePercent, New Infragistics.Win.UltraWinToolTip.UltraToolTipInfo(Consolidated3DControlStrings.TT_uneScalePercent, Infragistics.Win.ToolTipImage.Default, String.Empty, Infragistics.Win.DefaultableBoolean.True))
            Me.UltraToolTipManager2.SetUltraToolTip(une_PosX, New Infragistics.Win.UltraWinToolTip.UltraToolTipInfo(Consolidated3DControlStrings.TT_une_PosX, Infragistics.Win.ToolTipImage.Default, String.Empty, Infragistics.Win.DefaultableBoolean.True))
            Me.UltraToolTipManager2.SetUltraToolTip(une_PosY, New Infragistics.Win.UltraWinToolTip.UltraToolTipInfo(Consolidated3DControlStrings.TT_une_PosY, Infragistics.Win.ToolTipImage.Default, String.Empty, Infragistics.Win.DefaultableBoolean.True))
            Me.UltraToolTipManager2.SetUltraToolTip(une_PosZ, New Infragistics.Win.UltraWinToolTip.UltraToolTipInfo(Consolidated3DControlStrings.TT_une_PosZ, Infragistics.Win.ToolTipImage.Default, String.Empty, Infragistics.Win.DefaultableBoolean.True))
            Me.UltraToolTipManager2.SetUltraToolTip(btnAutoAdjust, New Infragistics.Win.UltraWinToolTip.UltraToolTipInfo(Consolidated3DControlStrings.TT_btnAutoAdjust, Infragistics.Win.ToolTipImage.Default, String.Empty, Infragistics.Win.DefaultableBoolean.True))
        End Sub

        Private Function GetToolBarButtonTTText(buttonName As String) As String
            Static resName As String = Nothing
            Static man As System.Resources.ResourceManager

            If resName Is Nothing Then
                resName = Me.GetType.Assembly.GetManifestResourceNames.Where(Function(rName) Not String.IsNullOrEmpty(rName) AndAlso rName.EndsWith(String.Format(".{0}.resources", NameOf(Consolidated3DControlStrings)))).Single
                man = New System.Resources.ResourceManager(resName.Replace(".resources", String.Empty), Me.GetType.Assembly)
            End If

            Return man.GetString(String.Format("TT_ToolBarButton_{0}", buttonName))
        End Function

        Private Sub InitClippPlanes()
            _clippingPlanes.Init()
            _activeClipPlanes = New List(Of ClippingPlane)
        End Sub

        Public Enum ToolBarButtons
            Home
            MagnifyingGlass
            ZoomWindow
            Zoom
            Pan
            Rotate
            ZoomFit
            ToggleAdjustMode
            LoadCarModel
            ChangeCarTransparency
            ChangeJTTransparency
            SynchronizeSelection
        End Enum

    End Class

End Namespace