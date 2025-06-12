Imports System.ComponentModel

<Serializable()> _
Public Class InlinerPairCheckClassification

    Public Property PropertyName As String
    Public Property Classification As String

    Public Sub New()
        PropertyName = String.Empty
        Classification = String.Empty
    End Sub

    Public Sub New(propName As String, [class] As String)
        PropertyName = propName
        Classification = [class]
    End Sub

End Class


Public Class InlinerPairCheckClassificationList
    Inherits BindingList(Of InlinerPairCheckClassification)

    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub CreateDefaultInlinerPairCheckClassifications()
        Me.Add(New InlinerPairCheckClassification("Color", "Warning"))
        Me.Add(New InlinerPairCheckClassification("CSA", "Error"))
        Me.Add(New InlinerPairCheckClassification("Signal", "Warning"))
        Me.Add(New InlinerPairCheckClassification("TerminalPlating", "Error"))
        Me.Add(New InlinerPairCheckClassification("Type", "Warning"))
    End Sub

    Public Function ContainsInlinerPairCheckClassification(inlinerPairCheckClass As InlinerPairCheckClassification) As Boolean
        For Each inlPairCheckClass As InlinerPairCheckClassification In Me
            If (inlPairCheckClass.PropertyName = inlinerPairCheckClass.PropertyName) AndAlso (inlPairCheckClass.Classification = inlinerPairCheckClass.Classification) Then Return True
        Next

        Return False
    End Function

    Public Function FindInlinerPairCheckClassification(propertyName As String) As InlinerPairCheckClassification
        For Each inlPairCheckClass As InlinerPairCheckClassification In Me
            If (inlPairCheckClass.PropertyName = propertyName) Then Return inlPairCheckClass
        Next

        Return Nothing
    End Function

End Class