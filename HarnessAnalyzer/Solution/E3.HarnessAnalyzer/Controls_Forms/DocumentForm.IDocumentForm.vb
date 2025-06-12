Imports System.IO
Imports Zuken.E3.HarnessAnalyzer.Settings
Imports Zuken.E3.Lib.IO.Files
Imports Zuken.E3.Lib.Model

Partial Public Class DocumentForm
    Implements IDocumentForm

    Private ReadOnly Property IDocumentForm_HarnessModulConfigurations As HarnessModuleConfigurationCollection Implements IDocumentForm.HarnessModulConfigurations
        Get
            Return _harnessModuleConfigurations
        End Get
    End Property

    Private ReadOnly Property IDocumentForm_Model As IEEModel Implements IDocumentForm.Model
        Get
            Return Me.Document.Model
        End Get
    End Property

    Private ReadOnly Property IDocumentForm_HcvFile As IBaseDataFile Implements IDocumentForm.HcvFile
        Get
            Return Me.File
        End Get
    End Property

    Private ReadOnly Property IDocumentForm_HcvDocument As IWorkFile Implements IDocumentForm.HcvDocument
        Get
            Return Me.Document
        End Get
    End Property

    Private ReadOnly Property IDocumentForm_IDocumentForm_TextResolved As String Implements IDocumentForm.TextResolved
        Get
            If MainForm IsNot Nothing Then
                Return Me.MainForm.utmmMain.TabFromForm(Me).TextResolved
            Else
                Return Me.Text
            End If
        End Get
    End Property

    Private ReadOnly Property IDocumentForm_IDocumentForm_Kbl As IKblBaseContainer Implements IDocumentForm.Kbl
        Get
            Return Me.KBL
        End Get
    End Property

    Private ReadOnly Property IDocumentForm_IDocumentForm_GeneralSettings As GeneralSettingsBase Implements IDocumentForm.GeneralSettings
        Get
            Return Me.GeneralSettings
        End Get
    End Property

    Private Function IDocumentForm_GetTechnicalCheckedCompareCompareResultInfo() As CheckedCompareResultList Implements IDocumentForm.GetTechnicalCheckedCompareCompareResultInfo
        Return Me.GetCompareResultInfo(Hcv.KnownContainerFileFlags.TCRI).CheckedCompareResults
    End Function

    Private Function IDocumentForm_GetGraphicalCompareResultInfoFileContainer() As IBaseDataFile Implements IDocumentForm.GetGraphicalCompareResultInfoFileContainer
        Return GetCompareResultInfoFileContainer([Lib].IO.Files.Hcv.KnownContainerFileFlags.GCRI)
    End Function

    Private Function IDocumentForm_GetTechnicallCompareResultInfoFileContainer() As IBaseDataFile Implements IDocumentForm.GetTechnicallCompareResultInfoFileContainer
        Return GetCompareResultInfoFileContainer([Lib].IO.Files.Hcv.KnownContainerFileFlags.TCRI)
    End Function

End Class
