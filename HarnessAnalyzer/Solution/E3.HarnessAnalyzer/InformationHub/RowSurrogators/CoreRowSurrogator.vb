Imports System.Data.Common
Imports System.Web.Services.Description
Imports Infragistics.Win.UltraWinDataSource
Imports Infragistics.Win.UltraWinGrid
Imports vdIFC
Imports Zuken.E3.Lib.Schema.Kbl.Properties

Friend Class CoreRowSurrogator
    Inherits WireCoreRowSurrogator(Of Core_occurrence, Core)

    Public Sub New(kbl_reference As IKblContainer, kbl_compare As IKblContainer, corresponding_DS As UltraDataSource, mainForm As MainForm)
        MyBase.New(kbl_reference, kbl_compare, corresponding_DS, mainForm)
    End Sub

End Class
