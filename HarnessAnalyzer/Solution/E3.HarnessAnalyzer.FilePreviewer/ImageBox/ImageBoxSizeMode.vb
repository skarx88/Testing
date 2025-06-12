Namespace Global.System.Windows.Forms
    ' Cyotek ImageBox
    ' Copyright (c) 2010-2015 Cyotek Ltd.
    ' http://cyotek.com/blog/tag/imagebox
    ' Licensed under the MIT License.
    ''' <summary>
    ''' Determines the sizing mode of an image hosted in an <see cref="ImageBox"/> control.
    ''' </summary>
    Friend Enum ImageBoxSizeMode
        ''' <summary>
        ''' The image is disiplayed according to current zoom and scroll properties.
        ''' </summary>
        Normal

        ''' <summary>
        ''' The image is stretched to fill the client area of the control.
        ''' </summary>
        Stretch

        ''' <summary>
        ''' The image is stretched to fill as much of the client area of the control as possible, whilst retaining the same aspect ratio for the width and height.
        ''' </summary>
        Fit
    End Enum
End Namespace
