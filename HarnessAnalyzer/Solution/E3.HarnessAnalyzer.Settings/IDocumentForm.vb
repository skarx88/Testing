Imports System.IO
Imports Zuken.E3.Lib.IO.Files
Imports Zuken.E3.Lib.Model
Imports Zuken.E3.Lib.Schema.KBL

Public Interface IDocumentForm

    ReadOnly Property TextResolved As String

    ReadOnly Property HarnessModulConfigurations As HarnessModuleConfigurationCollection

    ReadOnly Property Model As IEEModel

    ReadOnly Property Kbl As IKblBaseContainer

    ReadOnly Property GeneralSettings As GeneralSettingsBase

    ReadOnly Property HcvFile As IO.IBaseDataFile
    ReadOnly Property HcvDocument As IWorkFile

    Function GetGraphicalCompareResultInfoFileContainer() As IBaseDataFile
    Function GetTechnicallCompareResultInfoFileContainer() As IBaseDataFile
    Function GetTechnicalCheckedCompareCompareResultInfo() As CheckedCompareResultList

End Interface
