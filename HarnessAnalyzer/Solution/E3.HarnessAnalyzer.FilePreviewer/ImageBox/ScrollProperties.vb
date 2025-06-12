Imports System.ComponentModel

' Original ScrollControl code by Scott Crawford (http://sukiware.com/)

Namespace Global.System.Windows.Forms
    Partial Class ScrollControl
#Region "Nested Types"

        ''' <summary>
        ''' Provides basic properties for the horizontal scroll bar in a <see cref="ScrollControl"/>.
        ''' </summary>
        Friend Class HScrollProperties
            Inherits ScrollProperties
#Region "Friend Constructors"

            ''' <summary>
            ''' Initializes a new instance of the <see cref="ScrollProperties"/> class.
            ''' </summary>
            ''' <param name="container">The <see cref="ScrollControl"/> whose scrolling properties this object describes.</param>
            Friend Sub New(container As ScrollControl)
                MyBase.New(container)
            End Sub

#End Region
        End Class

        ''' <summary>
        ''' Encapsulates properties related to scrolling.
        ''' </summary>
        Friend MustInherit Class ScrollProperties
#Region "Instance Fields"

            Private ReadOnly _container As ScrollControl

#End Region

#Region "Protected Constructors"

            ''' <summary>
            ''' Initializes a new instance of the <see cref="ScrollProperties"/> class.
            ''' </summary>
            ''' <param name="container">The <see cref="ScrollControl"/> whose scrolling properties this object describes.</param>
            Protected Sub New(container As ScrollControl)
                'System.Windows.Forms.ScrollProperties
                _container = container
            End Sub

#End Region

#Region "Friend Properties"

            ''' <summary>
            ''' Gets or sets whether the scroll bar can be used on the container.
            ''' </summary>
            ''' <value><c>true</c> if the scroll bar can be used; otherwise, <c>false</c>.</value>
            <DefaultValue(True)>
            Public Property Enabled As Boolean

            ''' <summary>
            ''' Gets or sets the distance to move a scroll bar in response to a large scroll command.
            ''' </summary>
            ''' <value>An <see cref="Integer"/> describing how far, in pixels, to move the scroll bar in response to a large change.</value>
            <DefaultValue(10)>
            Public Property LargeChange As Integer

            ''' <summary>
            ''' Gets or sets the upper limit of the scrollable range.
            ''' </summary>
            ''' <value>An <see cref="Integer"/> representing the maximum range of the scroll bar.</value>
            <DefaultValue(100)>
            Public Property Maximum As Integer

            ''' <summary>
            ''' Gets or sets the lower limit of the scrollable range.
            ''' </summary>
            ''' <value>An <see cref="Integer"/> representing the lower range of the scroll bar.</value>
            <DefaultValue(0)>
            Public Property Minimum As Integer

            ''' <summary>
            ''' Gets the control to which this scroll information applies.
            ''' </summary>
            ''' <value>A <see cref="ScrollControl"/>.</value>
            Friend ReadOnly Property ParentControl As ScrollControl
                Get
                    Return _container
                End Get
            End Property

            ''' <summary>
            ''' Gets or sets the distance to move a scroll bar in response to a small scroll command.
            ''' </summary>
            ''' <value>An <see cref="Integer"/> representing how far, in pixels, to move the scroll bar.</value>
            <DefaultValue(1)>
            Public Property SmallChange As Integer

            ''' <summary>
            ''' Gets or sets a numeric value that represents the current position of the scroll bar box.
            ''' </summary>
            ''' <value>An <see cref="Integer"/> representing the position of the scroll bar box, in pixels. </value>
            <Bindable(True)>
            <DefaultValue(0)>
            Public Property Value As Integer

            ''' <summary>
            ''' Gets or sets whether the scroll bar can be seen by the user.
            ''' </summary>
            ''' <value><c>true</c> if it can be seen; otherwise, <c>false</c>.</value>
            <DefaultValue(False)>
            Public Property Visible As Boolean

#End Region
        End Class

        ''' <summary>
        ''' Provides basic properties for the vertical scroll bar in a <see cref="ScrollControl"/>.
        ''' </summary>
        Friend Class VScrollProperties
            Inherits ScrollProperties
#Region "Friend Constructors"

            ''' <summary>
            ''' Initializes a new instance of the <see cref="ScrollProperties"/> class.
            ''' </summary>
            ''' <param name="container">The <see cref="ScrollControl"/> whose scrolling properties this object describes.</param>
            Friend Sub New(container As ScrollControl)
                MyBase.New(container)
            End Sub

#End Region
        End Class

#End Region
    End Class
End Namespace
