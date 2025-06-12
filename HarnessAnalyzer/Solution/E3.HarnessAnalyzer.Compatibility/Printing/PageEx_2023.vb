#If NETFRAMEWORK Or WINDOWS7_0_OR_GREATER Then
Imports System.Drawing.Printing
Imports System.Printing

Namespace Printing

    <Serializable>
    Public Class PageEx_2023

        Private WithEvents _margins As New MarginsEx_2023

        Public Sub New()
        End Sub

        ReadOnly Property Name As String
        ReadOnly Property PageType As PageMediaSizeName
        ReadOnly Property Height As MilliInch96_2023
        ReadOnly Property Width As MilliInch96_2023
        ReadOnly Property IsCustom As Boolean
        ReadOnly Property PageKind As PaperKind

        Property Margins As MarginsEx_2023
            Get
                Return _margins
            End Get
            Set(value As MarginsEx_2023)
                If Not value Is _margins Then
                    _margins = value
                End If
            End Set
        End Property

        Public Overrides Function ToString() As String
            Return String.Format("{0} ({1}x{2})", Me.Name, Me.Width, Me.Height)
        End Function

    End Class

End Namespace
#end if