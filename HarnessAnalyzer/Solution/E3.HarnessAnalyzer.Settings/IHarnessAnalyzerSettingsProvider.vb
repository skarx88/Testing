Imports Zuken.E3.HarnessAnalyzer.Settings.QualityStamping.Specification

Public Interface IHarnessAnalyzerSettingsProvider

    ReadOnly Property DiameterSettings As DiameterSettings
    ReadOnly Property GeneralSettings As GeneralSettingsBase
    ReadOnly Property QMStampSpecifications As QMStampSpecifications
    ReadOnly Property HasView3DFeature As Boolean
    ReadOnly Property HasSchematicsFeature As Boolean
    ReadOnly Property HasTopoCompareFeature As Boolean
End Interface
