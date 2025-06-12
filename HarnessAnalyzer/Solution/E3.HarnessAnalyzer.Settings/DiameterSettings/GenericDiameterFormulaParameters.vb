
<Serializable()> _
Public Class GenericDiameterFormulaParameters

    Public Property BDL_Coeff1 As Single
    Public Property BDL_Corr As Single
    Public Property BDL_Exp As Single
    Public Property MCD_Coeff1 As Single
    Public Property MCD_Corr As Single
    Public Property MCD_Exp As Single
    Public Property WD_Coeff1 As Single
    Public Property WD_Coeff2 As Single
    Public Property WD_Coeff3 As Single
    Public Property WD_Exp As Single

    Public Sub New()
        BDL_Coeff1 = 1.425
        BDL_Corr = 1.0
        BDL_Exp = 0.5
        MCD_Coeff1 = 1.49
        MCD_Corr = 1.1
        MCD_Exp = 0.52
        WD_Coeff1 = 0.4
        WD_Coeff2 = 0.055
        WD_Coeff3 = 1.6259
        WD_Exp = 0.4943
    End Sub

End Class
