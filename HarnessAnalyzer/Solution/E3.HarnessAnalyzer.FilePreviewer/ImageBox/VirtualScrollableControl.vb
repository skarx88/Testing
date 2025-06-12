Imports System
Imports System.ComponentModel
Imports System.Drawing
Imports System.Windows.Forms

' Original VirtualScrollableControl code by Scott Crawford (http://sukiware.com/)

Namespace Global.System.Windows.Forms
    ''' <summary>
    ''' Defines a base class for controls that support auto-scrolling behavior.
    ''' </summary>
    <ToolboxItem(False)>
    Friend Class VirtualScrollableControl
        Inherits ScrollControl
#Region "Instance Fields"

        Private _autoScroll As Boolean

        Private _autoScrollMargin As System.Drawing.Size

        Private _autoScrollMinSize As System.Drawing.Size

        Private _autoScrollPosition As System.Drawing.Point

#End Region

#Region "Friend Constructors"

        ''' <summary>
        '''   Initializes a new instance of the <see cref="VirtualScrollableControl"/> class.
        ''' </summary>
        Friend Sub New()
            AutoScrollMargin = System.Drawing.Size.Empty
            AutoScrollMinSize = System.Drawing.Size.Empty
            AutoScrollPosition = System.Drawing.Point.Empty
            AutoScroll = True

            SetStyle(ControlStyles.ContainerControl, True)
        End Sub

#End Region

#Region "Events"

        ''' <summary>
        '''   Occurs when the AutoScroll property value changes
        ''' </summary>
        <Category("Property Changed")>
        Friend Event AutoScrollChanged As EventHandler

        ''' <summary>
        '''   Occurs when the AutoScrollMargin property value changes
        ''' </summary>
        <Category("Property Changed")>
        Friend Event AutoScrollMarginChanged As EventHandler

        ''' <summary>
        '''   Occurs when the AutoScrollMinSize property value changes
        ''' </summary>
        <Category("Property Changed")>
        Friend Event AutoScrollMinSizeChanged As EventHandler

        ''' <summary>
        '''   Occurs when the AutoScrollPosition property value changes
        ''' </summary>
        <Category("Property Changed")>
        Friend Event AutoScrollPositionChanged As EventHandler

#End Region

#Region "Overridden Methods"

        ''' <summary>
        '''   Raises the <see cref="System.Windows.Forms.Control.Resize"/> event.
        ''' </summary>
        ''' <param name="e">
        '''   An <see cref="T:System.EventArgs"/> that contains the event data.
        ''' </param>
        Protected Overrides Sub OnResize(e As EventArgs)
            MyBase.OnResize(e)

            AdjustScrollbars()

            If AutoScroll AndAlso Not AutoScrollPosition.IsEmpty Then
                Dim xOffset As Integer
                Dim yOffset As Integer
                Dim scrollArea As Rectangle

                scrollArea = Me.ScrollArea
                xOffset = scrollArea.Right - DisplayRectangle.Right
                yOffset = scrollArea.Bottom - DisplayRectangle.Bottom

                If AutoScrollPosition.Y < 0 AndAlso yOffset < 0 Then
                    yOffset = Math.Max(yOffset, AutoScrollPosition.Y)
                Else
                    yOffset = 0
                End If

                If AutoScrollPosition.X < 0 AndAlso xOffset < 0 Then
                    xOffset = Math.Max(xOffset, AutoScrollPosition.X)
                Else
                    xOffset = 0
                End If

                ScrollByOffset(New System.Drawing.Size(xOffset, yOffset))
            End If
        End Sub

        ''' <summary>
        '''   Raises the <see cref="ScrollControl.Scroll"/> event.
        ''' </summary>
        ''' <param name="e">
        '''   The <see cref="ScrollEventArgs"/> instance containing the event data.
        ''' </param>
        Protected Overrides Sub OnScroll(e As ScrollEventArgs)
            If e.Type <> ScrollEventType.EndScroll Then
                Select Case e.ScrollOrientation
                    Case ScrollOrientation.HorizontalScroll
                        ScrollByOffset(New System.Drawing.Size(e.NewValue + AutoScrollPosition.X, 0))
                    Case ScrollOrientation.VerticalScroll
                        ScrollByOffset(New System.Drawing.Size(0, e.NewValue + AutoScrollPosition.Y))
                End Select
            End If

            MyBase.OnScroll(e)
        End Sub

        ''' <summary>
        '''   Raises the <see cref="System.Windows.Forms.Control.VisibleChanged"/> event.
        ''' </summary>
        ''' <param name="e">
        '''   An <see cref="T:System.EventArgs"/> that contains the event data.
        ''' </param>
        Protected Overrides Sub OnVisibleChanged(e As EventArgs)
            If Visible Then
                PerformLayout()
            End If

            MyBase.OnVisibleChanged(e)
        End Sub

#End Region

#Region "Friend Properties"

        ''' <summary>
        '''   Gets or sets a value indicating whether the container enables the user to scroll to any controls placed outside of its visible boundaries.
        ''' </summary>
        ''' <value>
        '''   <c>true</c> if the container enables auto-scrolling; otherwise, <c>false</c>.
        ''' </value>
        <Category("Layout")>
        <DefaultValue(True)>
        Friend Overridable Property AutoScroll As Boolean
            Get
                Return _autoScroll
            End Get
            Set(value As Boolean)
                If AutoScroll <> value Then
                    _autoScroll = value

                    OnAutoScrollChanged(EventArgs.Empty)
                End If
            End Set
        End Property

        ''' <summary>
        '''   Gets or sets the System.Drawing.Size of the auto-scroll margin.
        ''' </summary>
        ''' <value>
        '''   A <see cref="T:System.Drawing.System.Drawing.Size"/> that represents the height and width of the auto-scroll margin in pixels.
        ''' </value>
        ''' <exception cref="System.ArgumentOutOfRangeException"></exception>
        <Category("Layout")>
        <DefaultValue(GetType(System.Drawing.Size), "0, 0")>
        Friend Overridable Property AutoScrollMargin As System.Drawing.Size
            Get
                Return _autoScrollMargin
            End Get
            Set(value As System.Drawing.Size)
                If value.Width < 0 Then
                    Throw New ArgumentOutOfRangeException(NameOf(value), "Width must be a positive integer.")
                ElseIf value.Height < 0 Then
                    Throw New ArgumentOutOfRangeException(NameOf(value), "Height must be a positive integer.")
                End If

                If AutoScrollMargin <> value Then
                    _autoScrollMargin = value

                    OnAutoScrollMarginChanged(EventArgs.Empty)
                End If
            End Set
        End Property

        ''' <summary>
        '''   Gets or sets the minimum System.Drawing.Size of the auto-scroll.
        ''' </summary>
        ''' <value>
        '''   A <see cref="T:System.Drawing.System.Drawing.Size"/> that determines the minimum System.Drawing.Size of the virtual area through which the user can scroll.
        ''' </value>
        <Category("Layout")>
        <DefaultValue(GetType(System.Drawing.Size), "0, 0")>
        Friend Overridable Property AutoScrollMinSize As System.Drawing.Size
            Get
                Return _autoScrollMinSize
            End Get
            Set(value As System.Drawing.Size)
                If AutoScrollMinSize <> value Then
                    _autoScrollMinSize = value

                    OnAutoScrollMinSizeChanged(EventArgs.Empty)
                End If
            End Set
        End Property

        ''' <summary>
        '''   Gets or sets the location of the auto-scroll position.
        ''' </summary>
        ''' <value>
        '''   A <see cref="T:System.Drawing.System.Drawing.Point"/> that represents the auto-scroll position in pixels.
        ''' </value>
        <Browsable(False)>
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Friend Overridable Property AutoScrollPosition As System.Drawing.Point
            Get
                Return _autoScrollPosition
            End Get
            Set(value As System.Drawing.Point)
                Dim translated As System.Drawing.Point

                translated = AdjustPositionToSize(New System.Drawing.Point(-value.X, -value.Y))

                If AutoScrollPosition <> translated Then
                    ScrollByOffset(New System.Drawing.Size(_autoScrollPosition.X - translated.X, _autoScrollPosition.Y - translated.Y))
                    _autoScrollPosition = translated

                    OnAutoScrollPositionChanged(EventArgs.Empty)
                End If
            End Set
        End Property

#End Region

#Region "Protected Properties"

        ''' <summary>
        '''   Total area of all visible controls which are scrolled with this container
        ''' </summary>
        Protected ReadOnly Property ScrollArea As Rectangle
            Get
                Dim area As Rectangle

                area = Rectangle.Empty

                For Each child As Control In Controls
                    If child.Visible Then
                        area = Rectangle.Union(child.Bounds, area)
                    End If
                Next

                Return Rectangle.Union(area, New Rectangle(_autoScrollPosition, _autoScrollMinSize))
            End Get
        End Property

        ''' <summary>
        '''   Gets the view port rectangle.
        ''' </summary>
        ''' <value>The view port rectangle.</value>
        Protected ReadOnly Property ViewPortRectangle As Rectangle
            Get
                Return New Rectangle(-_autoScrollPosition.X, -_autoScrollPosition.Y, DisplayRectangle.Width, DisplayRectangle.Height)
            End Get
        End Property

#End Region

#Region "Friend Members"

        ''' <summary>
        '''   Scrolls the specified child control into view on an auto-scroll enabled control.
        ''' </summary>
        ''' <param name="activeControl">The child control to scroll into view.</param>
        Friend Sub ScrollControlIntoView(activeControl As Control)
            If activeControl.Visible AndAlso AutoScroll AndAlso (HorizontalScroll.Visible OrElse VerticalScroll.Visible) Then
                Dim position As System.Drawing.Point

                position = AdjustPositionToSize(New System.Drawing.Point(AutoScrollPosition.X + activeControl.Left, AutoScrollPosition.Y + activeControl.Top))

                If position.X <> AutoScrollPosition.X OrElse position.Y <> AutoScrollPosition.Y Then
                    ScrollByOffset(New System.Drawing.Size(AutoScrollPosition.X - position.X, AutoScrollPosition.Y - position.Y))
                End If
            End If
        End Sub

#End Region

#Region "Protected Members"

        ''' <summary>
        '''   Adjusts the given System.Drawing.Point according to the scroll System.Drawing.Size
        ''' </summary>
        ''' <param name="position">The position.</param>
        ''' <returns></returns>
        Protected Function AdjustPositionToSize(position As System.Drawing.Point) As System.Drawing.Point
            Dim x As Integer
            Dim y As Integer

            x = position.X
            y = position.Y

            If x < -(AutoScrollMinSize.Width - ClientRectangle.Width) Then
                x = -(AutoScrollMinSize.Width - ClientRectangle.Width)
            End If
            If y < -(AutoScrollMinSize.Height - ClientRectangle.Height) Then
                y = -(AutoScrollMinSize.Height - ClientRectangle.Height)
            End If
            If x > 0 Then
                x = 0
            End If
            If y > 0 Then
                y = 0
            End If

            Return New System.Drawing.Point(x, y)
        End Function

        ''' <summary>
        '''   Raises the <see cref="AutoScrollChanged"/> event.
        ''' </summary>
        ''' <param name="e">
        '''   The <see cref="EventArgs"/> instance containing the event data.
        ''' </param>
        Protected Overridable Sub OnAutoScrollChanged(e As EventArgs)
            Dim handler As EventHandler

            handler = AutoScrollChangedEvent

            If handler IsNot Nothing Then
                handler(Me, e)
            End If
        End Sub

        ''' <summary>
        '''   Raises the <see cref="AutoScrollMarginChanged"/> event.
        ''' </summary>
        ''' <param name="e">
        '''   The <see cref="EventArgs"/> instance containing the event data.
        ''' </param>
        Protected Overridable Sub OnAutoScrollMarginChanged(e As EventArgs)
            Dim handler As EventHandler

            handler = AutoScrollMarginChangedEvent

            If handler IsNot Nothing Then
                handler(Me, e)
            End If
        End Sub

        ''' <summary>
        '''   Raises the <see cref="AutoScrollMinSizeChanged"/> event.
        ''' </summary>
        ''' <param name="e">
        '''   The <see cref="EventArgs"/> instance containing the event data.
        ''' </param>
        Protected Overridable Sub OnAutoScrollMinSizeChanged(e As EventArgs)
            Dim handler As EventHandler

            AutoScrollPosition = AdjustPositionToSize(AutoScrollPosition)
            AdjustScrollbars()

            handler = AutoScrollMinSizeChangedEvent

            If handler IsNot Nothing Then
                handler(Me, e)
            End If
        End Sub

        ''' <summary>
        '''   Raises the <see cref="AutoScrollPositionChanged"/> event.
        ''' </summary>
        ''' <param name="e">
        '''   The <see cref="EventArgs"/> instance containing the event data.
        ''' </param>
        Protected Overridable Sub OnAutoScrollPositionChanged(e As EventArgs)
            Dim handler As EventHandler

            handler = AutoScrollPositionChangedEvent

            If handler IsNot Nothing Then
                handler(Me, e)
            End If
        End Sub

#End Region

#Region "Private Members"

        ''' <summary>
        '''   Adjusts the scrollbars.
        ''' </summary>
        Private Sub AdjustScrollbars()
            Dim clientRectangle As Rectangle

            clientRectangle = Me.ClientRectangle

            If clientRectangle.Width > 1 AndAlso clientRectangle.Height > 1 Then
                Dim scrollSize As System.Drawing.Size
                Dim pageSize As System.Drawing.Size
                Dim horizontalScrollVisible As Boolean
                Dim verticalScrollVisible As Boolean

                scrollSize = System.Drawing.Size.Empty
                pageSize = System.Drawing.Size.Empty

                horizontalScrollVisible = AutoScroll AndAlso AutoScrollMinSize.Width > clientRectangle.Width
                verticalScrollVisible = AutoScroll AndAlso AutoScrollMinSize.Height > clientRectangle.Height

                If verticalScrollVisible Then
                    scrollSize.Height = AutoScrollMinSize.Height
                    pageSize.Height = clientRectangle.Height - 1
                End If

                If horizontalScrollVisible Then
                    scrollSize.Width = AutoScrollMinSize.Width
                    pageSize.Width = clientRectangle.Width - 1
                End If

                Me.ScrollSize = scrollSize
                Me.PageSize = pageSize
            End If
        End Sub

        ''' <summary>
        '''   Scrolls child controls by the given offset
        ''' </summary>
        ''' <param name="offset">The offset.</param>
        Private Sub ScrollByOffset(offset As System.Drawing.Size)
            If Not offset.IsEmpty Then
                SuspendLayout()
                For Each child As Control In Controls
                    child.Location -= offset
                Next

                _autoScrollPosition = New System.Drawing.Point(_autoScrollPosition.X - offset.Width, _autoScrollPosition.Y - offset.Height)
                ScrollTo(-_autoScrollPosition.X, -_autoScrollPosition.Y)

                ResumeLayout()

                Invalidate()
            End If
        End Sub

#End Region
    End Class
End Namespace
