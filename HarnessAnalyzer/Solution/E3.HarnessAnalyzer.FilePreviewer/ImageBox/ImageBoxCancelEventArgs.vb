Imports System.ComponentModel
Imports System.Drawing

Namespace Global.System.Windows.Forms
    ' Cyotek ImageBox
    ' Copyright (c) 2010-2015 Cyotek Ltd.
    ' http://cyotek.com/blog/tag/imagebox
    ''' <summary>
    ''' Provides data for a cancelable event.
    ''' </summary>
    Friend Class ImageBoxCancelEventArgs
        Inherits CancelEventArgs
        ''' <summary>
        ''' Gets or sets the location of the action being performed.
        ''' </summary>
        Private _Location As System.Drawing.Point

        ''' <summary>
        ''' Initializes a new instance of the <see cref="ImageBoxCancelEventArgs"/> class.
        ''' </summary>
        ''' <param name="location">The location of the action being performed.</param>
        Friend Sub New(location As System.Drawing.Point)
            Me.New()
            Me.Location = location
        End Sub

        ''' <summary>
        ''' Initializes a new instance of the <see cref="ImageBoxCancelEventArgs"/> class.
        ''' </summary>
        Protected Sub New()
        End Sub

        Public Property Location As System.Drawing.Point
            Get
                Return _Location
            End Get
            Protected Set(value As System.Drawing.Point)
                _Location = value
            End Set
        End Property

    End Class
End Namespace
