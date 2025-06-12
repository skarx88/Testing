Imports Infragistics.Win.UltraWinDataSource

Friend Class ChangeRowSurrogator
    Inherits ComparisonRowSurrogator(Of Change, ChangePartDummy)

    Public Sub New(kbl_reference As IKblContainer, kbl_compare As IKblContainer, corresponding_DS As UltraDataSource, mainForm As MainForm, isChildRowSurrogator As Boolean)
        MyBase.New(kbl_reference, kbl_compare, corresponding_DS, mainForm, isChildRowSurrogator)
    End Sub

    'HINT: currently nothing to overwrite here because all properties can be resolved by reflection (in the base)

    Friend Class ChangePartDummy
        Inherits Part

        Public Sub New()
            MyBase.New
        End Sub
    End Class

End Class
