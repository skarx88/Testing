Imports Zuken.E3.App.Controls

Namespace Schematics.Converter.Kbl

    Public Class CavityInfo
        Inherits EdbConversionEntityInfo

        Private _cavityNumber As Integer

        Public Sub New(blockId As String, edbCavityId As String, originalCavityId As String, shortName As String, cavityNumber As Integer)
            MyBase.New(blockId, edbCavityId, originalCavityId, shortName, Connectivity.Model.ObjType.Cavity)
            _cavityNumber = cavityNumber
        End Sub

        ReadOnly Property CavityNumber As Integer
            Get
                Return _cavityNumber
            End Get
        End Property

    End Class

End Namespace