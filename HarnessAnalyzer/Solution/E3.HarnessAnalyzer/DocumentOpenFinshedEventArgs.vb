Imports System.Reflection

<ObfuscationAttribute(Feature:="renaming", ApplyToMembers:=True)>
Public Class DocumentOpenFinshedEventArgs
    Inherits EventArgs

    Private _document As DocumentForm

    Public Sub New(document As DocumentForm)
        MyBase.New()
        _document = document
    End Sub

    ReadOnly Property Document As DocumentForm
        Get
            Return _document
        End Get
    End Property

End Class
