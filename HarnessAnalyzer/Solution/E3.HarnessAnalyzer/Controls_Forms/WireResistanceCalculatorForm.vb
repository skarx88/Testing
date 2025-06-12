Imports System.Reflection
Imports Zuken.E3.HarnessAnalyzer.Settings.WeightSettings

<Reflection.Obfuscation(Feature:="renaming", ApplyToMembers:=False, Exclude:=True)> ' needed to avoid resource-file-names obfuscation errors, see issue #5
Public Class WireResistanceCalculatorForm

    Private _hasError As Boolean
    Private _internalValueChange As Boolean = False
    Private _calculators As WireResistanceCalcCollection
    Private _kblMappers As IEnumerable(Of KblMapper)
    Private _addResMultiply As UInt32

    Private Sub New(mappers As IEnumerable(Of KblMapper), coreOrWireIds() As String, additionalResistanceMultiply As UInt32)
        InitializeComponent()
        _kblMappers = mappers

        Dim objects As New List(Of Object)
        For Each coreOrWireId As String In coreOrWireIds
            Dim harnessId As String = HarnessConnectivity.GetHarnessFromUniqueId(coreOrWireId)
            Dim resolvedCoreOrWireId As String = HarnessConnectivity.GetKblIdFromUniqueId(coreOrWireId)

            Dim harnessMapper As KblMapper = mappers.Where(Function(m) m.HarnessPartNumber = harnessId).FirstOrDefault
            If (harnessMapper IsNot Nothing) Then
                Dim coreOccurrence As Core_occurrence = TryCast(harnessMapper.KBLOccurrenceMapper(resolvedCoreOrWireId), Core_occurrence)
                Dim wireOccurrence As Wire_occurrence = TryCast(harnessMapper.KBLOccurrenceMapper(resolvedCoreOrWireId), Wire_occurrence)

                If coreOccurrence IsNot Nothing Then
                    objects.Add(WireResistanceCalculator.OverrideObject.OverrideWithCable(coreOccurrence, harnessMapper))
                Else
                    objects.Add(wireOccurrence)
                End If
            End If
        Next

        Me.Init(objects.ToArray)
        Me.AdditionalResMultiply = additionalResistanceMultiply
    End Sub

    Private Sub Init(wiresOrCores As IEnumerable(Of Object))
        If wiresOrCores Is Nothing OrElse Not wiresOrCores.Any() Then
            Throw New ArgumentException("No wire or core objects defined in parameter!", NameOf(wiresOrCores))
        End If

        _calculators = New WireResistanceCalcCollection(_kblMappers, wiresOrCores)
        Me.UltraToolTipManager1.Enabled = False
        InitializeControls(wiresOrCores.Select(
                           Function(wc)
                               If TypeOf wc Is WireResistanceCalculator.OverrideObject Then
                                   Return CType(wc, WireResistanceCalculator.OverrideObject).Object
                               Else
                                   Return wc
                               End If
                           End Function))
    End Sub

    Private Sub UpdateCalculatorsFromAll()
        If Not _internalValueChange Then
            UpdateCalculatorsFrom(UpdateFromCtrlType.UneCsa)
            UpdateCalculatorsFrom(UpdateFromCtrlType.UneLength)
            UpdateCalculatorsCore()
        End If
    End Sub

    Private Sub UpdateCalculatorsFrom(updateType As UpdateFromCtrlType)
        If Not _internalValueChange Then
            Select Case updateType
                Case UpdateFromCtrlType.UneCsa
                    For Each calc As WireResistanceCalculator In _calculators
                        calc.Csa = GetUneWireCsa()
                    Next
                Case UpdateFromCtrlType.UneLength
                    _calculators.SetEachLengthRelativeToSum(GetUneWireLength)
            End Select
        End If
    End Sub

    Private Sub UpdateCalculatorsCore()
        If Not _internalValueChange Then
            If (Me.uceConductorMaterial.SelectedItem IsNot Nothing) Then
                If TypeOf Me.uceConductorMaterial.SelectedItem.DataValue Is MaterialSpec Then
                    _calculators.UpdateResistance(CType(Me.uceConductorMaterial.SelectedItem.DataValue, MaterialSpec), CDbl(Me.uneTemperature.Value))
                ElseIf TypeOf Me.uceConductorMaterial.SelectedItem.DataValue Is WireResistanceCalculator.Temperature Then
                    With CType(Me.uceConductorMaterial.SelectedItem.DataValue, WireResistanceCalculator.Temperature)
                        Dim newTemp As New WireResistanceCalculator.Temperature(CInt(Me.uneTemperature.Value), .Coefficient)
                        _calculators.UpdateResistance(CDbl(uneResistivity.Value), newTemp)
                    End With
                Else
                    Throw New NotSupportedException(String.Format("Item data-type ""{0}"" not supported!", uceConductorMaterial.SelectedItem.DataValue.GetType.Name))
                End If
            ElseIf uneResistivity.Value IsNot Nothing Then
                _calculators.UpdateResistance(CDbl(uneResistivity.Value))
            End If
        End If
    End Sub

    Private Enum UpdateFromCtrlType
        UneCsa
        UneLength
    End Enum

    Private Enum ResisistanceUpdateType
        ''' <summary> Updates resistance from current csa- /length- and core-input-parameters</summary>
        All = 0
        ''' <summary>Update resistance only from materialSpec, temperature (incl. coefficient) and resistivity</summary>
        Core = 1
    End Enum

    Private Sub UpdateResistance(Optional updateType As ResisistanceUpdateType = ResisistanceUpdateType.All)
        If Not _internalValueChange Then
            Select Case updateType
                Case ResisistanceUpdateType.All
                    UpdateCalculatorsFromAll()
                Case ResisistanceUpdateType.Core
                    UpdateCalculatorsCore()
            End Select

            Try
                UpdateUneWireResistanceFromCalculator()
            Catch ex As Exception
                MessageBoxEx.ShowError(ex.GetInnerOrDefaultMessage)
            End Try
        End If
    End Sub

    Private Sub UpdateUneWireResistanceFromCalculator()
        If Not _internalValueChange AndAlso _calculators IsNot Nothing Then
            _internalValueChange = True

            Me.uneWireResistance.Appearance.ForeColor = Color.Black

            Dim resistanceValue As Double = _calculators.ResistanceSum + If(Not IsDBNull(txtResistanceMultiplier.Value), AdditionalResMultiply * CDbl(txtResistanceMultiplier.Value), 0)
            If Double.IsNaN(resistanceValue) OrElse Double.IsInfinity(resistanceValue) Then
                uneWireResistance.Value = Nothing
            Else
                uneWireResistance.Value = resistanceValue
            End If

            If uneWireResistance.Value Is Nothing OrElse Double.IsNaN(CDbl(uneWireResistance.Value)) OrElse Double.IsInfinity(CDbl(uneWireResistance.Value)) OrElse CDbl(uneWireResistance.Value) = 0 Then
                Me.uneWireResistance.Appearance.ForeColor = Color.Red
                Me.UltraToolTipManager1.Enabled = True
            End If

            _internalValueChange = False
        End If
    End Sub

    Public Property AdditionalResMultiply As UInt32
        Get
            Return _addResMultiply
        End Get
        Set(value As UInt32)
            Static multiplyOrgText As String = lblAddMultiply.Text
            lblAddMultiply.Visible = value > 0
            txtResistanceMultiplier.Visible = value > 0
            lblAdditionalResistance.Visible = value > 0
            _addResMultiply = value
            lblAddMultiply.Text = String.Format(multiplyOrgText, value)
        End Set
    End Property

    Private Function GetUneWireLength() As Double
        If IsDBNull(uneWireLength.Value) Then
            uneWireLength.Value = uneWireLength.MinValue
        End If
        Return CDbl(uneWireLength.Value)
    End Function

    Private Function GetUneWireCsa() As Double
        If IsDBNull(uneWireCSA.Value) Then
            uneWireCSA.Value = uneWireCSA.MinValue
        End If
        Return CDbl(uneWireCSA.Value)
    End Function

    Private Sub InitializeControls(wiresOrCores As IEnumerable(Of Object))
        Me.lblCaption.Text = String.Format(WireResistanceCalculatorFormStrings.Caption, String.Join(";", _calculators.Names))
        Try
            _internalValueChange = True
            Try
                Me.uneWireLength.Value = _calculators.Length.Sum
                Me.uneWireLength.ReadOnly = Not _calculators.Csa.AllEqual
                Me.uneWireCSA.Value = _calculators.Csa.Sum / Math.Max(_calculators.Count, 1)
                Me.uneWireCSA.Visible = _calculators.Csa.AllEqual
            Catch ex As Exception
                _hasError = True
                MessageBoxEx.ShowError(Me, WireResistanceCalculatorFormStrings.ErrorInitCalculator_Msg)
            End Try

            If (_calculators.Length.Sum = 0) Then
                Me.uneWireLength.Appearance.ForeColor = Color.Red
            End If

            If (_calculators.Csa.Sum = 0) Then
                Me.uneWireCSA.Appearance.ForeColor = Color.Red
            End If

            Me.uneTemperature.Value = WireResistanceCalculator.Temperature.Default.Value

            _hasError = Not InitializeConductorMaterials(wiresOrCores.ToArray)
        Finally
            _internalValueChange = False
        End Try
    End Sub

    Private Function InitializeConductorMaterials(ParamArray listObjects() As Object) As Boolean
        Dim dCount As Integer = 0
        Dim errors As New System.Text.StringBuilder

        Me.uceConductorMaterial.Items.Add(AluminiumTemperature.Default, WireResistanceCalculatorFormStrings.Alu_Text)
        Me.uceConductorMaterial.Items.Add(CopperTemperature.Default, WireResistanceCalculatorFormStrings.Copper_Text)
        Me.uceConductorMaterial.Items.Add(New WireResistanceCalculator.Temperature(CDbl(Me.uneTemperature.Value), 0), KblObjectType.Custom.ToLocalizedString)
        Me.uceConductorMaterial.SelectedItem = Me.uceConductorMaterial.Items(1)

        Dim addedMatSpecs As New List(Of MaterialSpec)
        If listObjects IsNot Nothing Then 'should be wire occurence or special wire occurence and nothing else ?
            For Each listObject As Object In listObjects
                If listObject IsNot Nothing Then
                    Dim specs As New List(Of MaterialSpec)
                    Try
                        specs = My.Application.MainForm.WeightSettings.MaterialSpecs.FindMaterialSpecsFor2(GetGeneralWire(listObject))
                    Catch ex As Exception
                        errors.AppendLine(String.Format(ErrorStrings.WeightCalc_ErrorRetrievingMaterialSpec, [Lib].Schema.Kbl.Utils.GetUserId(listObject), ex.Message))
                    End Try

                    If specs.Count > 0 Then
                        For Each spec As MaterialSpec In specs
                            If Not addedMatSpecs.Contains(spec) Then
                                addedMatSpecs.Add(spec)
                                Dim specDescr As String = spec.Description
                                If specDescr.Trim = WireResistanceCalculatorFormStrings.Alu_Text.Trim OrElse specDescr.Trim = WireResistanceCalculatorFormStrings.Copper_Text.Trim Then
                                    dCount += 1
                                    specDescr = String.Format("{0} ({1})", specDescr, dCount)
                                End If
                                Dim itm As Infragistics.Win.ValueListItem = Me.uceConductorMaterial.Items.Insert(0, spec, specDescr)
                                Me.uceConductorMaterial.SelectedItem = itm
                            End If
                        Next
                    End If
                End If
            Next
        End If

        If errors.Length > 0 Then
            MessageBoxEx.ShowError(Me, errors.ToString)
        End If

        Return errors.Length = 0
    End Function

    Private Function GetGeneralWire(listObject As Object) As General_wire
        Dim mapper As KblMapper = Nothing
        If TypeOf (listObject) Is Wire_occurrence Then
            mapper = _kblMappers.Where(Function(mp) mp.KBLWireList.Contains(DirectCast(listObject, Wire_occurrence))).FirstOrDefault
            Return DirectCast(mapper.KBLPartMapper(DirectCast(listObject, Wire_occurrence).Part), General_wire)
        ElseIf (TypeOf (listObject) Is Special_wire_occurrence) Then
            mapper = _kblMappers.Where(Function(mp) mp.KBLCableList.Contains(DirectCast(listObject, Special_wire_occurrence))).FirstOrDefault
            Return DirectCast(mapper.KBLPartMapper(DirectCast(listObject, Special_wire_occurrence).Part), General_wire)
        End If
        Return Nothing
    End Function


    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
    End Sub

    Private Sub uceConductorMaterial_SelectionChanged(sender As Object, e As EventArgs) Handles uceConductorMaterial.SelectionChanged
        Me.uneResistivity.ReadOnly = Me.uceConductorMaterial.SelectedItem.DisplayText <> KblObjectType.Custom.ToLocalizedString

        If TypeOf Me.uceConductorMaterial.SelectedItem.DataValue Is MaterialSpec Then
            Me.uneResistivity.Value = CType(Me.uceConductorMaterial.SelectedItem.DataValue, MaterialSpec).Resistivity
        ElseIf TypeOf Me.uceConductorMaterial.SelectedItem.DataValue Is IResistivityProvider Then
            Me.uneResistivity.Value = CType(Me.uceConductorMaterial.SelectedItem.DataValue, IResistivityProvider).Resistivity
        End If
    End Sub

    Private Sub uneResistivity_ValueChanged(sender As Object, e As EventArgs) Handles uneResistivity.ValueChanged
        UpdateResistance(ResisistanceUpdateType.Core)
    End Sub

    Private Sub uneTemperature_ValueChanged(sender As Object, e As EventArgs) Handles uneTemperature.ValueChanged
        UpdateResistance(ResisistanceUpdateType.Core)
    End Sub

    Private Sub uneWireLength_ValueChanged(sender As Object, e As EventArgs) Handles uneWireLength.ValueChanged
        UpdateCalculatorsFrom(UpdateFromCtrlType.UneLength)
        UpdateResistance(ResisistanceUpdateType.Core)
    End Sub

    Private Sub uneWireCSA_ValueChanged(sender As Object, e As EventArgs) Handles uneWireCSA.ValueChanged
        UpdateCalculatorsFrom(UpdateFromCtrlType.UneCsa)
        UpdateResistance(ResisistanceUpdateType.Core)
    End Sub

    Friend ReadOnly Property HasError() As Boolean
        Get
            Return _hasError
        End Get
    End Property

    Private Sub uneWireLengthWireCSA_KeyDown(sender As Object, e As KeyEventArgs) Handles uneWireLength.KeyDown, uneWireCSA.KeyDown, uneTemperature.KeyDown
        If e.Control AndAlso e.KeyCode = Keys.A Then CType(sender, Infragistics.Win.UltraWinEditors.UltraNumericEditor).SelectAll()
    End Sub

    Private Sub txtResistanceMultiplier_ValueChanged(sender As Object, e As EventArgs) Handles txtResistanceMultiplier.ValueChanged
        UpdateUneWireResistanceFromCalculator()
    End Sub

    Public Shadows Function ShowDialog(owner As IWin32Window) As DialogResult
        If (Not Me.HasError) Then
            Return MyBase.ShowDialog(owner)
        End If

        Return DialogResult.Abort
    End Function

    Public Shadows Function ShowDialog() As DialogResult
        If (Not Me.HasError) Then
            Return MyBase.ShowDialog()
        End If

        Return DialogResult.Abort
    End Function

    Public Shared Shadows Function ShowDialog(mappers As IEnumerable(Of KblMapper), coreOrWireIds() As String) As DialogResult
        Return ShowDialog(Nothing, mappers, coreOrWireIds, 0)
    End Function

    Public Shared Shadows Function ShowDialog(owner As IWin32Window, mappers As IEnumerable(Of KblMapper), coreOrWireIds() As String) As DialogResult
        Return ShowDialog(owner, mappers, coreOrWireIds, 0)
    End Function

    Public Shared Shadows Function ShowDialog(mappers As IEnumerable(Of KblMapper), coreOrWireIds() As String, additionalResistanceMultiply As UInt32) As DialogResult
        Return ShowDialog(Nothing, mappers, coreOrWireIds, additionalResistanceMultiply)
    End Function

    Public Shared Shadows Function ShowDialog(owner As IWin32Window, mappers As IEnumerable(Of KblMapper), coreOrWireIds() As String, additionalResistanceMultiply As UInt32) As DialogResult
        Dim dlg As WireResistanceCalculatorForm = Nothing
        Try
            dlg = New WireResistanceCalculatorForm(mappers, coreOrWireIds, additionalResistanceMultiply)
        Catch ex As Exception
            ex.ShowComponentCrashMessage(If(dlg?.Text, NameOf(WireResistanceCalculatorForm)), False)
            If dlg IsNot Nothing Then
                dlg.Dispose()
            End If
            Return DialogResult.Abort
        End Try

        If dlg IsNot Nothing Then
            Using dlg
                Return dlg.ShowDialog(owner)
            End Using
        End If

        Return DialogResult.Abort
    End Function

    Private Sub WireResistanceCalculatorForm_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        Me.UpdateResistance(ResisistanceUpdateType.Core) ' HINT: Initially ignore the current csa and length from numeric editors to normally override each obeject-csa/length (only for each object if they are changed the first time - This is a gui problem here).
        '                                  The gui doesn't support multiple length and csa-input's for each given object. 
        '                                  But we should change them seperately for each wire or core in the gui to solve this for the calculation. 
        '                                  To have a correct initial calculation for each object we do not override them the first time. 
        '                                  If changed later by the user each object-length and object-csa will be overriden (at each calculator) with the current values and based on this the resistance will be calculated. (but this shoudn't normally happen when having more objects, because we deactivate the input for the user then! -> only possible when one single core or wire was provided)
    End Sub

    Public Class AluminiumTemperature
        Inherits WireResistanceCalculator.Temperature
        Implements IResistivityProvider

        Public Sub New(value As Double)
            MyBase.New(value, My.Application.MainForm.GeneralSettings.TemperatureCoefficientAluminium)
        End Sub

        Protected Overrides Function IClonable_Clone() As Object
            Return New AluminiumTemperature(Me.Value)
        End Function

        Public Shared Shadows ReadOnly Property [Default] As AluminiumTemperature
            Get
                Static myDef As New AluminiumTemperature(WireResistanceCalculator.Temperature.Default.Value)
                Return myDef
            End Get
        End Property

        Public ReadOnly Property Resistivity As Double Implements IResistivityProvider.Resistivity
            Get
                Return My.Application.MainForm.GeneralSettings.ResistivityAluminium
            End Get
        End Property
    End Class

    Public Class CopperTemperature
        Inherits WireResistanceCalculator.Temperature
        Implements IResistivityProvider

        Public Sub New(value As Double)
            MyBase.New(value, My.Application.MainForm.GeneralSettings.TemperatureCoefficientCopper)
        End Sub

        Protected Overrides Function IClonable_Clone() As Object
            Return New CopperTemperature(Me.Value)
        End Function

        Public Shared Shadows ReadOnly Property [Default] As CopperTemperature
            Get
                Static myDef As New CopperTemperature(WireResistanceCalculator.Temperature.Default.Value)
                Return myDef
            End Get
        End Property

        Public ReadOnly Property Resistivity As Double Implements IResistivityProvider.Resistivity
            Get
                Return My.Application.MainForm.GeneralSettings.ResistivityCopper
            End Get
        End Property
    End Class

    Public Interface IResistivityProvider

        ReadOnly Property Resistivity As Double

    End Interface

    Private Sub txtResistanceMultiplier_KeyDown(sender As Object, e As KeyEventArgs) Handles txtResistanceMultiplier.KeyDown
        If e.Control AndAlso e.KeyCode = Keys.A Then CType(sender, Infragistics.Win.UltraWinEditors.UltraNumericEditor).SelectAll()
    End Sub

End Class