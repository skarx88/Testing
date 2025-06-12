Imports System.ComponentModel
Imports System.IO
Imports System.Runtime.Serialization
Imports Zuken.E3.HarnessAnalyzer.Settings.Resources

Namespace QualityStamping.Specification

    <DataContract(Namespace:="Zuken.E3.HarnessAnalyzer.QualityStamping.Specification")>
    Public Class QMStampSpecifications

        <Category("Version")>
        <DataMember(Order:=0)> Property Version As String
        <DataMember(Order:=1)> Property Connector As New QMStampSpecificationCollection
        <DataMember(Order:=2)> Property Fixing As New QMStampSpecificationCollection
        <DataMember(Order:=3)> Property Protection As New QMStampSpecificationCollection
        <DataMember(Order:=4)> Property Segment As New QMStampSpecificationCollection
        <DataMember(Order:=5)> Property Unspecified As New QMStampSpecificationCollection

        <IgnoreDataMember> Private _fullName As String = String.Empty

        Private Shared _loadError As Exception = Nothing

        Public Sub New()
        End Sub

        Public Shared Function LoadFromFile(fullName As String) As QMStampSpecifications
            Try
                Using fileStream As New FileStream(fullName, FileMode.Open, FileAccess.Read)
                    Dim qmStampSpecifications As QMStampSpecifications = HarnessAnalyzer.Shared.Utilities.ReadXml(Of QMStampSpecifications)(fileStream)
                    qmStampSpecifications._fullName = fullName

                    Return qmStampSpecifications
                End Using
            Catch ex As Exception
#If DEBUG Or CONFIG = "Debug" Then
                Throw
#End If
                _loadError = ex

                Return Nothing
            End Try
        End Function

        Public Sub ResetToDefaults()
            Version = E3.Lib.DotNet.Expansions.Devices.My.Application.Info.Version.ToString
            Connector.Add(New QMStampSpecification(QMStampSpecificationStrings.BranchToConnector))
            Connector.Add(New QMStampSpecification(QMStampSpecificationStrings.InstallationInstruction))

            Fixing.Add(New QMStampSpecification(QMStampSpecificationStrings.BranchToClip))
            Fixing.Add(New QMStampSpecification(QMStampSpecificationStrings.BranchToDuct))
            Fixing.Add(New QMStampSpecification(QMStampSpecificationStrings.ClipToClip))
            Fixing.Add(New QMStampSpecification(QMStampSpecificationStrings.Direction))
            Fixing.Add(New QMStampSpecification(QMStampSpecificationStrings.DuctToDuct))
            Fixing.Add(New QMStampSpecification(QMStampSpecificationStrings.InstallationInstruction))

            Protection.Add(New QMStampSpecification(QMStampSpecificationStrings.InstallationInstruction))

            Segment.Add(New QMStampSpecification(QMStampSpecificationStrings.Branch))
            Segment.Add(New QMStampSpecification(QMStampSpecificationStrings.BranchToClip))
            Segment.Add(New QMStampSpecification(QMStampSpecificationStrings.BranchToConnector))
            Segment.Add(New QMStampSpecification(QMStampSpecificationStrings.BranchToDuct))
            Segment.Add(New QMStampSpecification(QMStampSpecificationStrings.ClipToClip))
            Segment.Add(New QMStampSpecification(QMStampSpecificationStrings.Direction))
            Segment.Add(New QMStampSpecification(QMStampSpecificationStrings.DuctToDuct))
            Segment.Add(New QMStampSpecification(QMStampSpecificationStrings.InstallationInstruction))

            Unspecified.Add(New QMStampSpecification(QMStampSpecificationStrings.ElectricalCheck))
        End Sub

        Public Sub SaveTo(fullPath As String)
            Static qm_stamp_data_contract As DataContractAttribute = CType(Me.GetType.GetCustomAttributes(GetType(DataContractAttribute), False).SingleOrDefault, DataContractAttribute)
            Using fs As New FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None)
                HarnessAnalyzer.Shared.Utilities.WriteXml(Me, fs, String.Empty, qm_stamp_data_contract.Namespace)
                fs.Flush()
            End Using
        End Sub

        <IgnoreDataMember>
        Public Shared ReadOnly Property LoadError As Exception
            Get
                Return _loadError
            End Get
        End Property

    End Class

End Namespace