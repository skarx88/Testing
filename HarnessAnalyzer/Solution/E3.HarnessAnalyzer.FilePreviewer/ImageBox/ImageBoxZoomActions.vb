Imports System

Namespace Global.System.Windows.Forms
    ' Cyotek ImageBox
    ' Copyright (c) 2010-2015 Cyotek Ltd.
    ' http://cyotek.com/blog/tag/imagebox
    ' Licensed under the MIT License.
    ''' <summary>
    ''' Describes the zoom action occuring
    ''' </summary>
    <Flags>
    Friend Enum ImageBoxZoomActions
        ''' <summary>
        ''' No action.
        ''' </summary>
        None = 0

        ''' <summary>
        ''' The control is increasing the zoom.
        ''' </summary>
        ZoomIn = 1

        ''' <summary>
        ''' The control is decreasing the zoom.
        ''' </summary>
        ZoomOut = 2

        ''' <summary>
        ''' The control zoom was reset.
        ''' </summary>
        ActualSize = 4
    End Enum
End Namespace
