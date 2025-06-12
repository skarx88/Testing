Imports System.Data.Common
Imports System.Xml.Serialization
Imports Infragistics.Win.UltraWinDataSource
Imports Zuken.E3.Lib.Schema.Kbl.Properties

Friend Class HarnessRowSurrogator
    Inherits ComparisonRowSurrogator(Of HarnessOccurrenceWrapper, HarnessPartDummy)

    Public Sub New(kbl_reference As IKblContainer, kbl_compare As IKblContainer, corresponding_DS As UltraDataSource, mainForm As MainForm)
        MyBase.New(kbl_reference, kbl_compare, corresponding_DS, mainForm)
    End Sub

    Protected Overrides Sub OnAfterOccurrenceRowInitialized(row As UltraDataRow)
        MyBase.OnAfterOccurrenceRowInitialized(row)
    End Sub

    Public Class HarnessPartDummy
        Inherits Part
    End Class

    Public Class HarnessOccurrenceWrapper
        Implements IKblOccurrence

        Public Property Id As String Implements IKblOccurrence.Id
            Get
                Throw New NotImplementedException()
            End Get
            Set(value As String)
                Throw New NotImplementedException()
            End Set
        End Property

        Public Property Part As String Implements IKblOccurrence.Part
            Get
                Throw New NotImplementedException()
            End Get
            Set(value As String)
                Throw New NotImplementedException()
            End Set
        End Property

        Public Property SystemId As String Implements IKblBaseObject.SystemId
            Get
                Throw New NotImplementedException()
            End Get
            Set(value As String)
                Throw New NotImplementedException()
            End Set
        End Property

        Public ReadOnly Property ObjectType As KblObjectType Implements IKblBaseObject.ObjectType
            Get
                Throw New NotImplementedException()
            End Get
        End Property
    End Class

End Class

