Imports System.ComponentModel

<Serializable()>
Public Class Tutorial
    Public Property Title As String = String.Empty
    Public Property Url As String = String.Empty
    Public Property UILanguage As String = String.Empty

    Sub New()
    End Sub

    Sub New(uiLanguage As String, url As String, title As String)
        Me.UILanguage = uiLanguage
        Me.Url = url
        Me.Title = title
    End Sub
End Class

Public Class TutorialList
    Inherits BindingList(Of Tutorial)

    Public Sub New()
        MyBase.New()
    End Sub


    Public Sub CreateDefaultTutorialList()
        With Me
            .Add(New Tutorial("de-DE", "https://vimeo.com/837937902/3753825fe1?share=copy", "Module"))
            .Add(New Tutorial("en-US", "https://vimeo.com/837943558/a577b0f262?share=copy", "Modules"))

            .Add(New Tutorial("de-DE", "https://vimeo.com/837937826/9fadb17f50?share=copy", "Memoliste und Markierungen"))
            .Add(New Tutorial("en-US", "https://vimeo.com/837943530/0795ba9ae4?share=copy", "Mark and Memo"))

            .Add(New Tutorial("de-DE", "https://vimeo.com/837937637/2d7ccaa671?share=copy", "Analysefunktionen"))
            .Add(New Tutorial("en-US", "https://vimeo.com/837943458/9c84efb17c?share=copy", "Analysis"))

            .Add(New Tutorial("de-DE", "https://vimeo.com/837937707/48a7ba9735?share=copy", "Hilfreiche Funktionen"))
            .Add(New Tutorial("en-US", "https://vimeo.com/837943496/8825e8946f?share=copy", "Auxillary Functions"))

            .Add(New Tutorial("de-DE", "https://vimeo.com/837937130/80586b6e3a?share=copy", "Bündelberechnung"))
            .Add(New Tutorial("en-US", "https://vimeo.com/837943088/7f641cb47e?share=copy", "Bundle Calculation"))

            .Add(New Tutorial("de-DE", "https://vimeo.com/837937996/5cb2960272?share=copy", "Redlining"))
            .Add(New Tutorial("en-US", "https://vimeo.com/837943633/f43b3706b4?share=copy", "Redlining"))

            .Add(New Tutorial("de-DE", "https://vimeo.com/837937533/fed35a78b9?share=copy", "Exports"))
            .Add(New Tutorial("en-US", "https://vimeo.com/837943421/980ed6c700?share=copy", "Exporting"))

            .Add(New Tutorial("de-DE", "https://vimeo.com/837938067/1dc679f025?share=copy", "Schaltplan"))
            .Add(New Tutorial("en-US", "https://vimeo.com/837943659/47f8bfc944?share=copy", "Schematics"))

            .Add(New Tutorial("de-DE", "https://vimeo.com/837937304/28e2ae648e?share=copy", "Vergleich"))
            .Add(New Tutorial("en-US", "https://vimeo.com/837943311/6e08a7b391?share=copy", "Compare"))

            .Add(New Tutorial("de-DE", "https://vimeo.com/837937210/347a9a6755?share=copy", "Kammerbelegungsprüfung"))
            .Add(New Tutorial("en-US", "https://vimeo.com/837943275/38bc03bf2e?share=copy", "Cavity Checker"))

            .Add(New Tutorial("de-DE", "https://vimeo.com/837937029/57294c13a5?share=copy", "3D Darstellung"))
            .Add(New Tutorial("en-US", "https://vimeo.com/837943048/dbf1ab97e9?share=copy", "3D Visualization"))

            .Add(New Tutorial("de-DE", "https://vimeo.com/837936950/4fb9af530f?share=copy", "Bemaßungen in 3D"))
            .Add(New Tutorial("en-US", "https://vimeo.com/837943006/80b44e1222?share=copy", "Dimensions in 3D"))

            .Add(New Tutorial("de-DE", "https://vimeo.com/837938153/5e2739a59e?share=copy", "Spleißanalyse"))
            .Add(New Tutorial("en-US", "https://vimeo.com/837943695/c32aa20357?share=copy", "Splice Analysis"))

            .Add(New Tutorial("de-DE", "https://vimeo.com/837938255/7e4665d07c?share=copy", "Spannungsabfall"))
            .Add(New Tutorial("en-US", "https://vimeo.com/837943730/647491f802?share=copy", "Voltagedrop"))

            .Add(New Tutorial("de-DE", "https://vimeo.com/837938394/570e3d3949?share=copy", "xHCV in 2D"))
            .Add(New Tutorial("en-US", "https://vimeo.com/837943595/e7a8200a0b?share=copy", "xHCV in 2D"))

            .Add(New Tutorial("de-DE", "https://vimeo.com/837936871/1bd8f01026?share=copy", "xHCV in 3D"))
            .Add(New Tutorial("en-US", "https://vimeo.com/837943777/8f94366c95?share=copy", "xHCV in 3D"))

            .Add(New Tutorial("de-DE", "https://vimeo.com/837937367/bef2eb7179?share=copy", "QM-Reports"))
            .Add(New Tutorial("en-US", "https://vimeo.com/837943341/a7974cce7c?share=copy", "QM Reports"))

            .Add(New Tutorial("de-DE", "https://vimeo.com/837937437/a12b841c66?share=copy", "Validierungs-Reports"))
            .Add(New Tutorial("en-US", "https://vimeo.com/837943377/1c095d1392?share=copy", "Validation Reports"))

        End With
    End Sub

End Class

