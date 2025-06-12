Imports devDept.Eyeshot
Imports Zuken.E3.App.Windows.Controls.Comparer.Topology
Imports Zuken.E3.Lib.Comparer.Topology.Documents
Imports Zuken.E3.Lib.Comparer.Topology.Wizard
Imports Zuken.E3.Lib.IO.Files.Hcv

<Reflection.Obfuscation(Feature:="renaming", ApplyToMembers:=False, Exclude:=True)> ' needed to avoid resource-file-names obfuscation errors, see issue #5
Public Class TopologyCompareWizardControl

    Public Event SelectedDocumentChanged(sender As Object, e As EventArgs)

    Public Sub New()
        MyBase.New(GetType(TopologyCompareWizardFileSelector))

        InitializeComponent()

        If ModelCompareControl1 IsNot Nothing Then
            With ModelCompareControl1
                .Designs(DocumentSide.Left).SetAllInitialViews(viewType.Top)
                .Designs(DocumentSide.Right).SetAllInitialViews(viewType.Top)
            End With
        End If

        If _fileSelectorControl IsNot Nothing AndAlso Not Me.DesignMode Then
            AddHandler CType(_fileSelectorControl, TopologyCompareWizardFileSelector).SelectedDocumentChanged, AddressOf _fileSelectorControl_SelectedDocumentChanged
        End If
    End Sub

    Protected Overrides Sub InitStateMachines(lst As List(Of BaseStateMachine))
        lst.Clear()
        lst.Add(New TopologyCompareDocumentsStateMachine(Me.FileSelectorControl1, Me.ModelCompareControl1))
        lst.Add(New TopologyCompareDirectionVerticesStateMachine(Me.ModelCompareControl1))
        lst.Add(New TopologyCompareStartVerticesStateMachine(ModelCompareControl1))
        lst.Add(New TopologyCompareStartCompareStateMachine(Me.ModelCompareControl1))
        MyBase.InitStateMachines(lst)
    End Sub

    Protected Overrides Sub OnGetTextLeft(e As TextEventArgs)
        If Not e.Text.Any(Function(c) IO.Path.GetInvalidPathChars.Contains(c)) Then
            e.Text = IO.Path.GetFileNameWithoutExtension(e.Text)
        End If
    End Sub

    Protected Overrides Sub OnGetTextRight(e As TextEventArgs)
        If Not e.Text.Any(Function(c) IO.Path.GetInvalidPathChars.Contains(c)) Then
            e.Text = IO.Path.GetFileNameWithoutExtension(e.Text)
        End If
    End Sub

    Private Sub _fileSelectorControl_SelectedDocumentChanged(sender As Object, e As EventArgs)
        RaiseEvent SelectedDocumentChanged(Me, e)
    End Sub

    Property SelectedRefLengthClass As E3.Lib.Model.LengthClass
        Get
            Return CType(Me.FileSelectorControl1, TopologyCompareWizardFileSelector).SelectedRefLengthClass
        End Get
        Set(value As E3.Lib.Model.LengthClass)
            CType(Me.FileSelectorControl1, TopologyCompareWizardFileSelector).SelectedRefLengthClass = value
        End Set
    End Property

    Property SelectedCompareLengthClass As E3.Lib.Model.LengthClass
        Get
            Return CType(Me.FileSelectorControl1, TopologyCompareWizardFileSelector).SelectedCompareLengthClass
        End Get
        Set(value As E3.Lib.Model.LengthClass)
            CType(Me.FileSelectorControl1, TopologyCompareWizardFileSelector).SelectedCompareLengthClass = value
        End Set
    End Property

    Public Shadows Property FileLeft As HcvFile
        Get
            Return CType(Me.FileSelectorControl1, TopologyCompareWizardFileSelector).HcvLeft
        End Get
        Set(value As HcvFile)
            CType(Me.FileSelectorControl1, TopologyCompareWizardFileSelector).HcvLeft = value
        End Set
    End Property

    Public Shadows Property FileRight As HcvFile
        Get
            Return CType(Me.FileSelectorControl1, TopologyCompareWizardFileSelector).HcvRight
        End Get
        Set(value As HcvFile)
            CType(Me.FileSelectorControl1, TopologyCompareWizardFileSelector).HcvRight = value
        End Set
    End Property

End Class
