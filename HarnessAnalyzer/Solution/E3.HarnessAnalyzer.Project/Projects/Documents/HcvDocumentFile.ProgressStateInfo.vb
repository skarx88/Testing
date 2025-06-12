Imports System.ComponentModel

Namespace Documents

    Partial Public Class HcvDocument

        Public Class BundleRecalcProgressStateInfo
            Inherits ProgressInfo

            Public Sub New(state As ProgressState, recalcState As BundleRecalcState)
                MyBase.New(state)
                Me.RecalcBundleState = recalcState
            End Sub

            ReadOnly Property RecalcBundleState As BundleRecalcState
        End Class

        Public Class SpliceLocatorProgressStateInfo
            Inherits UserStateProgressInfo

            Public Sub New(state As ProgressState, userState As Object)
                MyBase.New(state, userState)
            End Sub
        End Class

    End Class

End Namespace