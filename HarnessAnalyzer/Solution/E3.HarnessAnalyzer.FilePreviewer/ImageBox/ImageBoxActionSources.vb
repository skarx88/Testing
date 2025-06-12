Imports System

Namespace Global.System.Windows.Forms
    ' Cyotek ImageBox
    ' Copyright (c) 2010-2015 Cyotek Ltd.
    ' http://cyotek.com/blog/tag/imagebox
    ''' <summary>
    ''' Specifies the source of an action being performed.
    ''' </summary>
    <Flags>
    Friend Enum ImageBoxActionSources
        ''' <summary>
        ''' Unknown source.
        ''' </summary>
        Unknown = 0

        ''' <summary>
        ''' A user initialized the action.
        ''' </summary>
        User = 1
    End Enum
End Namespace
