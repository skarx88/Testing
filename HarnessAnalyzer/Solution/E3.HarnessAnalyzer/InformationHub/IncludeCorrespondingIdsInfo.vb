Imports Infragistics.Win.UltraWinGrid

Public Class IncludeCorrespondingIdsInfo

    Property KblMapper As KblMapper
    Property Value As Boolean
    Property CorrespondingGrid As UltraGridBase

    Public Shared Function GetTrue(kblmapper As KblMapper, correspondingGrid As UltraGridBase) As IncludeCorrespondingIdsInfo
        Return New IncludeCorrespondingIdsInfo With {.KblMapper = kblmapper, .Value = True, .CorrespondingGrid = correspondingGrid}
    End Function

End Class