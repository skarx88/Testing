Namespace D3D.Consolidated.Controls

    <Reflection.Obfuscation(Feature:="renaming", ApplyToMembers:=False, Exclude:=True)> ' needed to avoid resource-file-names obfuscation errors, see issue #5
    Public Class OpacityControl

        Public Event ValueChanged(sender As Object, e As ValueChangedEventArgs)
        Public Shadows Event KeyUp(sender As Object, e As KeyEventArgs)

        Public Sub New()
            InitializeComponent()
        End Sub

        Public Property TransparencyPercent As UShort
            Get
                Return CUShort(TrackBar1.Value)
            End Get
            Set(value As UShort)
                TrackBar1.Value = value
            End Set
        End Property

        'ReadOnly Property TransparencyValue As Integer
        '    Get
        '        Return CInt(Transparency / 100 * 255)
        '    End Get
        'End Property

        Private Sub TrackBar1_KeyUp(sender As Object, e As KeyEventArgs) Handles TrackBar1.KeyUp
            RaiseEvent KeyUp(Me, e)
        End Sub

        Private Sub TrackBar1_ValueChanged(sender As Object, e As EventArgs) Handles TrackBar1.ValueChanged
            lbl.Text = (100 - TransparencyPercent).ToString + "%"
            RaiseEvent ValueChanged(Me, New ValueChangedEventArgs(TrackBar1.Value))
            lbl.Top = CInt(TrackBar1.Location.Y + TrackBar1.Height - TrackBar1.Height * TrackBar1.Value / 100)
        End Sub

        Private Sub TrackBar1_MouseDown(sender As Object, e As MouseEventArgs) Handles TrackBar1.MouseDown
            lbl.Text = (100 - TransparencyPercent).ToString + "%"
            lbl.Visible = True
        End Sub

        Private Sub TrackBar1_Move(sender As Object, e As EventArgs) Handles TrackBar1.Move
            lbl.Text = (100 - TransparencyPercent).ToString + "%"
        End Sub

        Private Sub frmOpacity_Load(sender As Object, e As EventArgs) Handles MyBase.Load
            lbl.Text = (100 - TransparencyPercent).ToString + "%"
            lbl.Top = CInt(TrackBar1.Location.Y + TrackBar1.Height - TrackBar1.Height * TrackBar1.Value / 100)
        End Sub

        Public Class ValueChangedEventArgs
            Inherits EventArgs

            Private _value As Integer

            Public Sub New(value As Integer)
                _value = value
            End Sub

            ReadOnly Property Value As Integer
                Get
                    Return _value
                End Get
            End Property

        End Class

    End Class
End Namespace