Imports Zuken.E3.HarnessAnalyzer.Settings

Namespace Checks.Cavities.Views.Model

    Public Class ColorInfo

        Public Const COLOR_SEPERATOR As Char = "/"c

        Public Sub New(color1 As Color, color2 As Color)
            Me.Color1 = color1
            Me.Color2 = color2
        End Sub

        ReadOnly Property Color1 As Color

        ReadOnly Property HasColor1 As Boolean
            Get
                Return Color1 <> Color.Empty
            End Get
        End Property

        ReadOnly Property Color2 As Color

        ReadOnly Property HasColor2 As Boolean
            Get
                Return Color2 <> Color.Empty
            End Get
        End Property

        Public Shared Function ParseFromWireColor(color As String, generalSettings As GeneralSettings) As ColorInfo
            Dim color1 As System.Drawing.Color
            Dim color2 As System.Drawing.Color
            If Not String.IsNullOrWhiteSpace(color) Then
                Dim colors As List(Of String) = color.Split(New Char() {COLOR_SEPERATOR}, StringSplitOptions.RemoveEmptyEntries).ToList

                If colors.Count > 0 Then
                    For Each colorCode As WireColorCode In generalSettings.WireColorCodes
                        If colorCode.ColorCode = colors(0) Then
                            color1 = colorCode.Color
                        End If
                        If colors.Count > 1 AndAlso colorCode.ColorCode = colors(1) Then
                            color2 = colorCode.Color
                        End If

                        If (colors.Count < 2 AndAlso color1 <> System.Drawing.Color.Empty) OrElse (colors.Count > 1 AndAlso color1 <> System.Drawing.Color.Empty AndAlso color2 <> System.Drawing.Color.Empty) Then
                            Exit For
                        End If
                    Next
                End If
            End If
            Return New ColorInfo(color1, color2)
        End Function


    End Class

End Namespace