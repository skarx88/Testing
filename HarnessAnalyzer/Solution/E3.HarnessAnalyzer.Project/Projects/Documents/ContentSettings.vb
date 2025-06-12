Namespace Documents

    Public Class ContentSettings

        Property LengthClass As Zuken.E3.Lib.Model.LengthClass = Zuken.E3.Lib.Model.LengthClass.DMU
        Property UseKblAbsoluteLocations As Boolean
        ReadOnly Property JT As New JTSettings

        Public Class JTSettings

            Public Sub New()
            End Sub

            ''' <summary>
            ''' Defines the model objects/entities that can be overwritten by JT-Data if available. 
            ''' </summary>
            ''' <returns></returns>
            Property OverwriteGeomertry As New List(Of Zuken.E3.Lib.Model.ContainerId)(HcvDocument.USE_CONTAINER_IDS)
            Property UseColors As Boolean = False

        End Class

    End Class

End Namespace