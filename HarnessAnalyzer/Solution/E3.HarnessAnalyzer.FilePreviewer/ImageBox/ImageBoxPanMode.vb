Imports System
Imports System.Windows.Forms

Namespace Global.System.Windows.Forms
    ''' <summary>
    ''' Specifies constants that define which mouse buttons can be used to pan an <see cref="ImageBox"/> control.
    ''' </summary>
    <Flags>
    Friend Enum ImageBoxPanMode
        ''' <summary>
        ''' No mouse buttons can be used to pan the control.
        ''' </summary>
        None = 0

        ''' <summary>
        ''' The left mouse button can be used to pan the control.
        ''' </summary>
        Left = MouseButtons.Left

        ''' <summary>
        ''' The middle mouse button can be used to pan the control.
        ''' </summary>
        Middle = MouseButtons.Middle

        ''' <summary>
        ''' Both the left and left mouse buttons can be used to pan the control.
        ''' </summary>
        Both = Left Or Middle
    End Enum
End Namespace
