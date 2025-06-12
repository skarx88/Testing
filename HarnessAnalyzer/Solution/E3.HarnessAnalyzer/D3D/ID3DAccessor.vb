Namespace D3D

    Public Interface ID3DAccessor

        ReadOnly Property Consolidated As D3D.Consolidated.Controls.Consolidated3DControl
        ReadOnly Property ActiveDocument As D3D.Document.Controls.Document3DControl
        ReadOnly Property ConsolidatedCarSetting() As Settings.CarTransformationSetting
        ReadOnly Property IsCancellationRequested As Boolean

    End Interface

End Namespace
