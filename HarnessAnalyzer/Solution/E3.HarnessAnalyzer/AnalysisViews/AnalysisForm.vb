<Reflection.Obfuscation(Feature:="renaming", ApplyToMembers:=False, Exclude:=True)> ' needed to avoid resource-file-names obfuscation errors, see issue #5
Public Class AnalysisForm

    Private _kblMapper As KblMapper
    Private _activeObjects As ICollection(Of String)

    Public Event ShowResults(sender As Object, e As EventArgs)

    Public Sub New()
        InitializeComponent()
    End Sub

    Public Sub New(doc As DocumentForm)
        InitializeComponent()

        If doc IsNot Nothing Then
            _kblMapper = doc.KBL
        End If

        _activeObjects = New List(Of String)
        Me.TopMost = True
        Me.StartPosition = FormStartPosition.CenterScreen
    End Sub

    Public Property ActiveObjects As ICollection(Of String)
        Get
            Return _activeObjects
        End Get
        Set(value As ICollection(Of String))
            _activeObjects = value
        End Set
    End Property

    Friend ReadOnly Property Kbl As KblMapper
        Get
            Return _kblMapper
        End Get
    End Property

    Public Sub btnViewClicked()
        Me.DialogResult = DialogResult.OK
        RaiseEvent ShowResults(Me, New EventArgs)
    End Sub

    Private Sub AnalysisForm_Closed(sender As Object, e As EventArgs) Handles Me.Closed
        Me.TopMost = False
        Me.DialogResult = DialogResult.None
    End Sub
End Class