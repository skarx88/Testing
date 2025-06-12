Imports Infragistics.Win

Partial Public Class InformationHub

    Protected Friend Class ButtonUIElementWithImageAndTag
        Inherits ButtonUIElement

        Private _image As Bitmap = Nothing

        Property Tag As Object

        Public Sub New(parent As UIElement, image As Bitmap, tag As Object)
            MyBase.New(parent)

            _image = image
            _Tag = tag
        End Sub

        Protected Overrides Function DrawTheme(ByRef drawParams As UIElementDrawParams) As Boolean
            Return False
        End Function

        Protected Overrides Sub InitAppearance(ByRef appearance As AppearanceData, ByRef requestedProps As AppearancePropFlags)
            MyBase.InitAppearance(appearance, requestedProps)

            appearance.ImageBackground = _image
        End Sub

    End Class

End Class
