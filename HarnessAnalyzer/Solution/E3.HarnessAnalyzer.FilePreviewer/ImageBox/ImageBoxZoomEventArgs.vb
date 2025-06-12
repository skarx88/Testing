Imports System

Namespace Global.System.Windows.Forms
    ' Cyotek ImageBox
    ' Copyright (c) 2010-2015 Cyotek Ltd.
    ' http://cyotek.com/blog/tag/imagebox
    ' Licensed under the MIT License.
    ''' <summary>
    ''' Contains event data for the <see cref="ImageBox.ZoomChanged"/> event.
    ''' </summary>
    Friend Class ImageBoxZoomEventArgs
        Inherits EventArgs

        Private _Actions As System.Windows.Forms.ImageBoxZoomActions, _NewZoom As Integer, _OldZoom As Integer, _Source As System.Windows.Forms.ImageBoxActionSources

        ''' <summary>
        ''' Initializes a new instance of the <see cref="ImageBoxZoomEventArgs"/> class.
        ''' </summary>
        ''' <param name="actions">The zoom operation being performed.</param>
        ''' <param name="source">The source of the operation.</param>
        ''' <param name="oldZoom">The old zoom level.</param>
        ''' <param name="newZoom">The new zoom level.</param>
        Friend Sub New(actions As ImageBoxZoomActions, source As ImageBoxActionSources, oldZoom As Integer, newZoom As Integer)
            Me.New()
            Me.Actions = actions
            Me.Source = source
            Me.OldZoom = oldZoom
            Me.NewZoom = newZoom
        End Sub

        ''' <summary>
        ''' Initializes a new instance of the <see cref="ImageBoxZoomEventArgs"/> class.
        ''' </summary>
        Protected Sub New()
        End Sub

        ''' <summary>
        ''' Gets or sets the actions that occured.
        ''' </summary>
        Public Property Actions As ImageBoxZoomActions
            Get
                Return _Actions
            End Get
            Protected Set(value As ImageBoxZoomActions)
                _Actions = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the new zoom level.
        ''' </summary>
        Public Property NewZoom As Integer
            Get
                Return _NewZoom
            End Get
            Protected Set(value As Integer)
                _NewZoom = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the old zoom level.
        ''' </summary>
        Public Property OldZoom As Integer
            Get
                Return _OldZoom
            End Get
            Protected Set(value As Integer)
                _OldZoom = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the source of the operation..
        ''' </summary>
        Public Property Source As ImageBoxActionSources
            Get
                Return _Source
            End Get
            Protected Set(value As ImageBoxActionSources)
                _Source = value
            End Set
        End Property

    End Class
End Namespace
