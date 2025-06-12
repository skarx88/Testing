'TODO: there are currently lot more of compare stuff within the base that should be moved in the future to this class 

'HINT/TODO: There is lots of more compare-stuff in the base information hub, this is only the minimal overriding for the DetailInformationForm to get it working between normal and compare-hub.
'           For the future the whole compare stuff should be moved out from information-hub to this class because the handling of two similar information-hubs that have different targets in one class is a mess for maintainance!

Imports Zuken.E3.HarnessAnalyzer.Settings

Public Class CompareInformationHub
    Inherits InformationHub

    Public Sub New(generalSettings As GeneralSettingsBase, parentForm As Form)
        MyBase.New(Nothing, generalSettings, parentForm, Nothing)
    End Sub

    Protected Overrides Function GetDetailInformationForm(caption As String, displayData As Object, Optional inactiveObjects As IDictionary(Of String, IEnumerable(Of String)) = Nothing, Optional kblMapper As KblMapper = Nothing, Optional objectId As String = Nothing, Optional wireLengthType As String = Nothing, Optional objectType As String = Nothing) As DetailInformationForm
        Return New DetailInformationCompareForm(caption, displayData, inactiveObjects, Me.Kbl, objectId, wireLengthType, objectType)
    End Function

    Protected Overrides Sub OnGridKeyDown(sender As Object, e As GridKeyDownEventArgs)
        MyBase.OnGridKeyDown(sender, e)
        With e.KeyEventArgs
            If Not .Handled AndAlso .KeyCode = Keys.Enter Then
                .Handled = True
            End If
        End With
    End Sub

End Class