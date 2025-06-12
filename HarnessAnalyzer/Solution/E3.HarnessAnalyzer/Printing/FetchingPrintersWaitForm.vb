Namespace Printing

    <Reflection.Obfuscation(Feature:="renaming", ApplyToMembers:=False, Exclude:=True)> ' needed to avoid resource-file-names obfuscation errors, see issue #5
    Public Class FetchingPrintersWaitForm
        Inherits PleaseWaitForm

        Public Sub New()
            MyBase.New
            InitializeComponent()
        End Sub

        Private Sub InitializeComponent()
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FetchingPrintersWaitForm))
            Me.SuspendLayout()
            '
            'Label1
            '
            resources.ApplyResources(Me.Label1, "Label1")
            '
            'FetchingPrintersWaitForm
            '
            resources.ApplyResources(Me, "$this")
            Me.Name = "FetchingPrintersWaitForm"
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
    End Class
End Namespace
