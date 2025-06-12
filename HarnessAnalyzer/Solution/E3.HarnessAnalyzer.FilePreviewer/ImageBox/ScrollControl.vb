Imports System
Imports System.ComponentModel
Imports System.Diagnostics
Imports System.Drawing
Imports System.Windows.Forms
' Original ScrollControl code by Scott Crawford (http://sukiware.com/)

Namespace Global.System.Windows.Forms
    ''' <summary>
    ''' Defines a base class for controls that support scrolling behavior.
    ''' </summary>
    <ToolboxItem(False)>
    Partial Friend Class ScrollControl
        Inherits Control
        Private _HorizontalScroll As System.Windows.Forms.ScrollControl.HScrollProperties, _VerticalScroll As System.Windows.Forms.ScrollControl.VScrollProperties

        Private _alwaysShowHScroll As Boolean
        Private _alwaysShowVScroll As Boolean
        Private _borderStyle As BorderStyle
        Private _pageSize As System.Drawing.Size
        Private _scrollSize As System.Drawing.Size
        Private _stepSize As System.Drawing.Size

        ''' <summary>
        '''   Initializes a new instance of the <see cref="ScrollControl"/> class.
        ''' </summary>
        Friend Sub New()
            ' ReSharper disable DoNotCallOverridableMethodsInConstructor
            BorderStyle = BorderStyle.Fixed3D
            ScrollSize = System.Drawing.Size.Empty
            PageSize = System.Drawing.Size.Empty
            StepSize = New System.Drawing.Size(10, 10)
            HorizontalScroll = New HScrollProperties(Me)
            VerticalScroll = New VScrollProperties(Me)
            ' ReSharper restore DoNotCallOverridableMethodsInConstructor
        End Sub

        ''' <summary>
        '''   Occurs when the BorderStyle property value changes
        ''' </summary>
        <Category("Property Changed")>
        Friend Event BorderStyleChanged As EventHandler

        ''' <summary>
        ''' Occurs when the mouse wheel moves while the control has focus.
        ''' </summary>
        <Browsable(True)>
        <EditorBrowsable(EditorBrowsableState.Always)>
        <Category("Mouse")>
        Friend Shadows Event MouseWheel As MouseEventHandler

        ''' <summary>
        '''   Occurs when the PageSize property value changes
        ''' </summary>
        <Category("Property Changed")>
        Friend Event PageSizeChanged As EventHandler

        ''' <summary>
        '''   Occurs when the user or code scrolls through the client area.
        ''' </summary>
        <Category("Action")>
        Friend Event Scroll As ScrollEventHandler

        ''' <summary>
        '''   Occurs when the ScrollSize property value changes
        ''' </summary>
        <Category("Property Changed")>
        Friend Event ScrollSizeChanged As EventHandler

        ''' <summary>
        '''   Occurs when the StepSize property value changes
        ''' </summary>
        <Category("Property Changed")>
        Friend Event StepSizeChanged As EventHandler

        ''' <summary>
        '''   Gets the required creation parameters when the control handle is created.
        ''' </summary>
        ''' <value>The create params.</value>
        ''' <returns>
        '''   A <see cref="T:System.Windows.Forms.CreateParams"/> that contains the required creation parameters when the handle to the control is created.
        ''' </returns>
        Protected Overrides ReadOnly Property CreateParams As CreateParams
            Get
                Dim lCreateParams As CreateParams

                lCreateParams = MyBase.CreateParams

                Select Case _borderStyle
                    Case BorderStyle.FixedSingle
                        lCreateParams.Style = lCreateParams.Style Or ImageBoxNativeMethods.WS_BORDER

                    Case BorderStyle.Fixed3D
                        lCreateParams.ExStyle = lCreateParams.ExStyle Or ImageBoxNativeMethods.WS_EX_CLIENTEDGE
                End Select

                Return lCreateParams
            End Get
        End Property

        ''' <summary>
        '''   Raises the <see cref="System.Windows.Forms.Control.EnabledChanged"/> event.
        ''' </summary>
        ''' <param name="e">
        '''   An <see cref="T:System.EventArgs"/> that contains the event data.
        ''' </param>
        Protected Overrides Sub OnEnabledChanged(e As EventArgs)
            MyBase.OnEnabledChanged(e)

            UpdateScrollbars()
        End Sub

        ''' <summary>
        '''   Raises the <see cref="System.Windows.Forms.Control.MouseDown"/> event.
        ''' </summary>
        ''' <param name="e">
        '''   A <see cref="T:System.Windows.Forms.MouseEventArgs"/> that contains the event data.
        ''' </param>
        Protected Overrides Sub OnMouseDown(e As MouseEventArgs)
            MyBase.OnMouseDown(e)

            If Not Focused Then
                Focus()
            End If
        End Sub

        ''' <summary>
        '''   Raises the <see cref="System.Windows.Forms.Control.MouseWheel"/> event.
        ''' </summary>
        ''' <param name="e">
        '''   A <see cref="T:System.Windows.Forms.MouseEventArgs"/> that contains the event data.
        ''' </param>
        Protected Overrides Sub OnMouseWheel(e As MouseEventArgs)
            Dim handler As MouseEventHandler

            If WheelScrollsControl Then
                Dim x As Integer
                Dim y As Integer
                Dim delta As Integer

                x = HorizontalScroll.Value
                y = VerticalScroll.Value

                ' TODO: Find if we are hovering over a horizontal scrollbar and scroll that instead of the default vertical.
                If VerticalScroll.Visible AndAlso VerticalScroll.Enabled Then
                    If ModifierKeys = Keys.Control Then
                        delta = VerticalScroll.LargeChange
                    Else
                        delta = SystemInformation.MouseWheelScrollLines * VerticalScroll.SmallChange
                    End If

                    y += If(e.Delta > 0, -delta, delta)
                ElseIf HorizontalScroll.Visible AndAlso HorizontalScroll.Enabled Then
                    If ModifierKeys = Keys.Control Then
                        delta = HorizontalScroll.LargeChange
                    Else
                        delta = SystemInformation.MouseWheelScrollLines * HorizontalScroll.SmallChange
                    End If

                    x += If(e.Delta > 0, -delta, delta)
                End If

                ScrollTo(x, y)
            End If

            handler = MouseWheelEvent
            If handler IsNot Nothing Then
                handler(Me, e)
            End If

            MyBase.OnMouseWheel(e)
        End Sub

        ''' <summary>
        '''   Processes Windows messages.
        ''' </summary>
        ''' <param name="msg">
        '''   The Windows <see cref="T:System.Windows.Forms.Message"/> to process.
        ''' </param>
        <DebuggerStepThrough>
        Protected Overrides Sub WndProc(ByRef msg As Message)
            Select Case msg.Msg
                Case ImageBoxNativeMethods.WM_HSCROLL, ImageBoxNativeMethods.WM_VSCROLL
                    WmScroll(msg)
                Case Else
                    MyBase.WndProc(msg)
            End Select
        End Sub


        ''' <summary>
        '''   Gets or sets a value indicating whether the horizontal scrollbar should always be displayed, even when not required.
        ''' </summary>
        ''' <value>
        '''   <c>true</c> if the horizontal scrollbar should always be displayed; otherwise, <c>false</c>.
        ''' </value>
        <Category("Layout")>
        <DefaultValue(False)>
        Public Property AlwaysShowHScroll As Boolean
            Get
                Return _alwaysShowHScroll
            End Get
            Set(value As Boolean)
                If _alwaysShowHScroll <> value Then
                    _alwaysShowHScroll = value

                    If value Then
                        Dim scrollInfo As ImageBoxNativeMethods.SCROLLINFO

                        scrollInfo = New ImageBoxNativeMethods.SCROLLINFO()
                        scrollInfo.fMask = ImageBoxNativeMethods.SIF.SIF_RANGE Or ImageBoxNativeMethods.SIF.SIF_DISABLENOSCROLL
                        scrollInfo.nMin = 0
                        scrollInfo.nMax = 0
                        scrollInfo.nPage = 1
                        SetScrollInfo(ScrollOrientation.HorizontalScroll, scrollInfo, False)

                        Invalidate()
                    Else
                        UpdateHorizontalScrollbar()
                    End If
                End If
            End Set
        End Property

        ''' <summary>
        '''   Gets or sets a value indicating whether the vertical scrollbar should always be displayed, even when not required.
        ''' </summary>
        ''' <value>
        '''   <c>true</c> if the vertical scrollbar should always be displayed; otherwise, <c>false</c>.
        ''' </value>
        <Category("Layout")>
        <DefaultValue(False)>
        Public Property AlwaysShowVScroll As Boolean
            Get
                Return _alwaysShowVScroll
            End Get
            Set(value As Boolean)
                Dim shown = VScroll

                _alwaysShowVScroll = value
                If _alwaysShowVScroll <> shown Then
                    If _alwaysShowVScroll Then
                        Dim scrollInfo As ImageBoxNativeMethods.SCROLLINFO

                        scrollInfo = New ImageBoxNativeMethods.SCROLLINFO()

                        scrollInfo.fMask = ImageBoxNativeMethods.SIF.SIF_RANGE Or ImageBoxNativeMethods.SIF.SIF_DISABLENOSCROLL
                        scrollInfo.nMin = 0
                        scrollInfo.nMax = 0
                        scrollInfo.nPage = 1
                        SetScrollInfo(ScrollOrientation.VerticalScroll, scrollInfo, False)

                        Invalidate()
                    Else
                        UpdateVerticalScrollbar()
                    End If
                End If
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the border style.
        ''' </summary>
        ''' <value>The border style.</value>
        <Category("Appearance")>
        <DefaultValue(GetType(BorderStyle), "Fixed3D")>
        Friend Overridable Property BorderStyle As BorderStyle
            Get
                Return _borderStyle
            End Get
            Set(value As BorderStyle)
                If BorderStyle <> value Then
                    _borderStyle = value

                    OnBorderStyleChanged(EventArgs.Empty)
                End If
            End Set
        End Property

        ''' <summary>
        '''   Gets the horizontal scrollbar properties.
        ''' </summary>
        ''' <value>The horizontal scrollbar properties.</value>
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        <Browsable(False)>
        Public Property HorizontalScroll As HScrollProperties
            Get
                Return _HorizontalScroll
            End Get
            Protected Set(value As HScrollProperties)
                _HorizontalScroll = value
            End Set
        End Property

        ''' <summary>
        '''   Gets or sets the System.Drawing.Size of the scroll pages.
        ''' </summary>
        ''' <value>The System.Drawing.Size of the page.</value>
        ''' <exception cref="System.ArgumentOutOfRangeException"></exception>
        <Browsable(False)>
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Overridable Property PageSize As System.Drawing.Size
            Get
                Return _pageSize
            End Get
            Set(value As System.Drawing.Size)
                If value.Width < 0 Then
                    Throw New ArgumentOutOfRangeException(NameOf(value), "Width must be a positive integer.")
                End If

                If value.Height < 0 Then
                    Throw New ArgumentOutOfRangeException(NameOf(value), "Height must be a positive integer.")
                End If

                If PageSize <> value Then
                    _pageSize = value

                    OnPageSizeChanged(EventArgs.Empty)
                End If
            End Set
        End Property

        ''' <summary>
        '''   Gets or sets the System.Drawing.Size of the scroll area.
        ''' </summary>
        ''' <value>The System.Drawing.Size of the scroll.</value>
        ''' <exception cref="System.ArgumentOutOfRangeException"></exception>
        <Browsable(False)>
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Friend Overridable Property ScrollSize As System.Drawing.Size
            Get
                Return _scrollSize
            End Get
            Set(value As System.Drawing.Size)
                If value.Width < 0 Then
                    Throw New ArgumentOutOfRangeException(NameOf(value), "Width must be a positive integer.")
                End If

                If value.Height < 0 Then
                    Throw New ArgumentOutOfRangeException(NameOf(value), "Height must be a positive integer.")
                End If

                If ScrollSize <> value Then
                    _scrollSize = value

                    OnScrollSizeChanged(EventArgs.Empty)
                End If
            End Set
        End Property

        ''' <summary>
        '''   Gets or sets the System.Drawing.Size of scrollbar stepping.
        ''' </summary>
        ''' <value>The System.Drawing.Size of the step.</value>
        ''' <exception cref="System.ArgumentOutOfRangeException"></exception>
        <Category("Layout")>
        <DefaultValue(GetType(System.Drawing.Size), "10, 10")>
        Friend Overridable Property StepSize As System.Drawing.Size
            Get
                Return _stepSize
            End Get
            Set(value As System.Drawing.Size)
                If value.Width < 0 Then
                    Throw New ArgumentOutOfRangeException(NameOf(value), "Width must be a positive integer.")
                End If

                If value.Height < 0 Then
                    Throw New ArgumentOutOfRangeException(NameOf(value), "Height must be a positive integer.")
                End If

                If StepSize <> value Then
                    _stepSize = value

                    OnStepSizeChanged(EventArgs.Empty)
                End If
            End Set
        End Property

        ''' <summary>
        '''   Gets the vertical scrollbar properties.
        ''' </summary>
        ''' <value>The vertical scrollbar properties.</value>
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        <Browsable(False)>
        Public Property VerticalScroll As VScrollProperties
            Get
                Return _VerticalScroll
            End Get
            Protected Set(value As VScrollProperties)
                _VerticalScroll = value
            End Set
        End Property

        ''' <summary>
        '''   Gets or sets a value indicating whether the horizontal scrollbar is displayed
        ''' </summary>
        ''' <value>
        '''   <c>true</c> if the horizontal scrollbar is displayed; otherwise, <c>false</c>.
        ''' </value>
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        <Browsable(False)>
        Protected Property HScroll As Boolean
            Get
                Return (ImageBoxNativeMethods.GetWindowLong(Handle, ImageBoxNativeMethods.GWL_STYLE) And ImageBoxNativeMethods.WS_HSCROLL) = ImageBoxNativeMethods.WS_HSCROLL
            End Get
            Set(value As Boolean)
                Dim longValue = ImageBoxNativeMethods.GetWindowLong(Handle, ImageBoxNativeMethods.GWL_STYLE)

                If value Then
                    longValue = CUInt(longValue Or ImageBoxNativeMethods.WS_HSCROLL)
                Else
                    longValue = CUInt(longValue Xor ImageBoxNativeMethods.WS_HSCROLL)
                End If
                ImageBoxNativeMethods.SetWindowLong(Handle, ImageBoxNativeMethods.GWL_STYLE, longValue)
            End Set
        End Property

        ''' <summary>
        '''   Gets or sets a value indicating whether the vertical scrollbar is displayed
        ''' </summary>
        ''' <value>
        '''   <c>true</c> if the vertical scrollbar is displayed; otherwise, <c>false</c>.
        ''' </value>
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        <Browsable(False)>
        Protected Property VScroll As Boolean
            Get
                Return (ImageBoxNativeMethods.GetWindowLong(Handle, ImageBoxNativeMethods.GWL_STYLE) And ImageBoxNativeMethods.WS_VSCROLL) = ImageBoxNativeMethods.WS_VSCROLL
            End Get
            Set(value As Boolean)
                Dim longValue = ImageBoxNativeMethods.GetWindowLong(Handle, ImageBoxNativeMethods.GWL_STYLE)

                If value Then
                    longValue = CUInt(longValue Or ImageBoxNativeMethods.WS_VSCROLL)
                Else
                    longValue = CUInt(longValue Xor ImageBoxNativeMethods.WS_VSCROLL)
                End If
                ImageBoxNativeMethods.SetWindowLong(Handle, ImageBoxNativeMethods.GWL_STYLE, longValue)
            End Set
        End Property

        ''' <summary>
        '''   Gets or sets a value indicating whether the control is scrolled when the mouse wheel is spun
        ''' </summary>
        ''' <value>
        '''   <c>true</c> if the mouse wheel scrolls the control; otherwise, <c>false</c>.
        ''' </value>
        <Browsable(False)>
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Protected Property WheelScrollsControl As Boolean

        ''' <summary>
        '''   Scrolls both scrollbars to the given values
        ''' </summary>
        ''' <param name="x">The x.</param>
        ''' <param name="y">The y.</param>
        Friend Sub ScrollTo(x As Integer, y As Integer)
            ScrollTo(ScrollOrientation.HorizontalScroll, x)
            ScrollTo(ScrollOrientation.VerticalScroll, y)
        End Sub

        ''' <summary>
        '''   Gets the type of scrollbar event.
        ''' </summary>
        ''' <param name="wParam">The wparam value from a window proc.</param>
        ''' <returns></returns>
        ''' <exception cref="System.ArgumentException"></exception>
        Protected Function GetEventType(wParam As IntPtr) As ScrollEventType
            Select Case wParam.ToInt32() And &HFFFF
                Case ImageBoxNativeMethods.SB_BOTTOM
                    Return ScrollEventType.Last
                Case ImageBoxNativeMethods.SB_ENDSCROLL
                    Return ScrollEventType.EndScroll
                Case ImageBoxNativeMethods.SB_LINEDOWN
                    Return ScrollEventType.SmallIncrement
                Case ImageBoxNativeMethods.SB_LINEUP
                    Return ScrollEventType.SmallDecrement
                Case ImageBoxNativeMethods.SB_PAGEDOWN
                    Return ScrollEventType.LargeIncrement
                Case ImageBoxNativeMethods.SB_PAGEUP
                    Return ScrollEventType.LargeDecrement
                Case ImageBoxNativeMethods.SB_THUMBPOSITION
                    Return ScrollEventType.ThumbPosition
                Case ImageBoxNativeMethods.SB_THUMBTRACK
                    Return ScrollEventType.ThumbTrack
                Case ImageBoxNativeMethods.SB_TOP
                    Return ScrollEventType.First
                Case Else
                    Throw New ArgumentException(String.Format("{0} isn't a valid scroll event type.", wParam), "wparam")
            End Select
        End Function

        ''' <summary>
        '''   Raises the <see cref="BorderStyleChanged"/> event.
        ''' </summary>
        ''' <param name="e">
        '''   The <see cref="EventArgs"/> instance containing the event data.
        ''' </param>
        Protected Overridable Sub OnBorderStyleChanged(e As EventArgs)
            Dim handler As EventHandler

            UpdateStyles()

            handler = BorderStyleChangedEvent

            If handler IsNot Nothing Then
                handler(Me, e)
            End If
        End Sub

        ''' <summary>
        '''   Raises the <see cref="PageSizeChanged"/> event.
        ''' </summary>
        ''' <param name="e">
        '''   The <see cref="EventArgs"/> instance containing the event data.
        ''' </param>
        Protected Overridable Sub OnPageSizeChanged(e As EventArgs)
            Dim handler As EventHandler

            UpdateScrollbars()

            handler = PageSizeChangedEvent

            If handler IsNot Nothing Then
                handler(Me, e)
            End If
        End Sub

        ''' <summary>
        '''   Raises the <see cref="Scroll"/> event.
        ''' </summary>
        ''' <param name="e">
        '''   The <see cref="ScrollEventArgs"/> instance containing the event data.
        ''' </param>
        Protected Overridable Sub OnScroll(e As ScrollEventArgs)
            Dim handler As ScrollEventHandler

            UpdateHorizontalScroll()
            UpdateVerticalScroll()

            handler = ScrollEvent

            If handler IsNot Nothing Then
                handler(Me, e)
            End If
        End Sub

        ''' <summary>
        '''   Raises the <see cref="ScrollSizeChanged"/> event.
        ''' </summary>
        ''' <param name="e">
        '''   The <see cref="EventArgs"/> instance containing the event data.
        ''' </param>
        Protected Overridable Sub OnScrollSizeChanged(e As EventArgs)
            Dim handler As EventHandler

            UpdateScrollbars()

            handler = ScrollSizeChangedEvent

            If handler IsNot Nothing Then
                handler(Me, e)
            End If
        End Sub

        ''' <summary>
        '''   Raises the <see cref="StepSizeChanged"/> event.
        ''' </summary>
        ''' <param name="e">
        '''   The <see cref="EventArgs"/> instance containing the event data.
        ''' </param>
        Protected Overridable Sub OnStepSizeChanged(e As EventArgs)
            Dim handler As EventHandler

            handler = StepSizeChangedEvent

            If handler IsNot Nothing Then
                handler(Me, e)
            End If
        End Sub

        ''' <summary>
        '''   Set the given scrollbar's tracking position to the specified value
        ''' </summary>
        ''' <param name="scrollbar">The scrollbar.</param>
        ''' <param name="value">The value.</param>
        Protected Overridable Sub ScrollTo(scrollbar As ScrollOrientation, value As Integer)
            Dim oldInfo As ImageBoxNativeMethods.SCROLLINFO

            oldInfo = GetScrollInfo(scrollbar)

            If value > oldInfo.nMax - oldInfo.nMin + 1 - oldInfo.nPage Then
                value = oldInfo.nMax - oldInfo.nMin + 1 - oldInfo.nPage
            End If
            If value < oldInfo.nMin Then
                value = oldInfo.nMin
            End If

            If oldInfo.nPos <> value Then
                Dim scrollInfo As ImageBoxNativeMethods.SCROLLINFO

                scrollInfo = New ImageBoxNativeMethods.SCROLLINFO()
                scrollInfo.fMask = ImageBoxNativeMethods.SIF.SIF_POS
                scrollInfo.nPos = value
                SetScrollInfo(scrollbar, scrollInfo, True)

                OnScroll(New ScrollEventArgs(ScrollEventType.ThumbPosition, oldInfo.nPos, value, scrollbar))
            End If
        End Sub

        ''' <summary>
        '''   Updates the properties of the horizontal scrollbar.
        ''' </summary>
        Protected Overridable Sub UpdateHorizontalScroll()
            Dim scrollInfo As ImageBoxNativeMethods.SCROLLINFO

            scrollInfo = GetScrollInfo(ScrollOrientation.HorizontalScroll)

            HorizontalScroll.Enabled = Enabled
            HorizontalScroll.LargeChange = scrollInfo.nPage
            HorizontalScroll.Maximum = scrollInfo.nMax
            HorizontalScroll.Minimum = scrollInfo.nMin
            HorizontalScroll.SmallChange = StepSize.Width
            HorizontalScroll.Value = scrollInfo.nPos
            HorizontalScroll.Visible = True
        End Sub

        ''' <summary>
        ''' Updates the horizontal scrollbar.
        ''' </summary>
        Protected Overridable Sub UpdateHorizontalScrollbar()
            Dim scrollInfo As ImageBoxNativeMethods.SCROLLINFO
            Dim scrollWidth As Integer
            Dim pageWidth As Integer

            scrollWidth = ScrollSize.Width - 1
            pageWidth = PageSize.Width

            If scrollWidth < 1 Then
                scrollWidth = 0
                pageWidth = 1
            End If

            scrollInfo = New ImageBoxNativeMethods.SCROLLINFO()
            scrollInfo.fMask = ImageBoxNativeMethods.SIF.SIF_PAGE Or ImageBoxNativeMethods.SIF.SIF_RANGE
            If AlwaysShowHScroll OrElse Not Enabled Then
                scrollInfo.fMask = scrollInfo.fMask Or ImageBoxNativeMethods.SIF.SIF_DISABLENOSCROLL
            End If
            scrollInfo.nMin = 0
            scrollInfo.nMax = scrollWidth
            scrollInfo.nPage = pageWidth

            SetScrollInfo(ScrollOrientation.HorizontalScroll, scrollInfo, True)
        End Sub

        ''' <summary>
        '''   Updates the scrollbars.
        ''' </summary>
        Protected Sub UpdateScrollbars()
            UpdateHorizontalScrollbar()
            UpdateVerticalScrollbar()
        End Sub

        ''' <summary>
        '''   Updates the properties of the vertical scrollbar.
        ''' </summary>
        Protected Overridable Sub UpdateVerticalScroll()
            Dim scrollInfo As ImageBoxNativeMethods.SCROLLINFO

            scrollInfo = GetScrollInfo(ScrollOrientation.VerticalScroll)

            VerticalScroll.Enabled = Enabled
            VerticalScroll.LargeChange = scrollInfo.nPage
            VerticalScroll.Maximum = scrollInfo.nMax
            VerticalScroll.Minimum = scrollInfo.nMin
            VerticalScroll.SmallChange = StepSize.Height
            VerticalScroll.Value = scrollInfo.nPos
            VerticalScroll.Visible = True
        End Sub

        ''' <summary>
        ''' Updates the vertical scrollbar.
        ''' </summary>
        Protected Overridable Sub UpdateVerticalScrollbar()
            Dim scrollInfo As ImageBoxNativeMethods.SCROLLINFO
            Dim scrollHeight As Integer
            Dim pageHeight As Integer

            scrollHeight = ScrollSize.Height - 1
            pageHeight = PageSize.Height

            If scrollHeight < 1 Then
                scrollHeight = 0
                pageHeight = 1
            End If

            scrollInfo = New ImageBoxNativeMethods.SCROLLINFO()
            scrollInfo.fMask = ImageBoxNativeMethods.SIF.SIF_PAGE Or ImageBoxNativeMethods.SIF.SIF_RANGE
            If AlwaysShowVScroll Then
                scrollInfo.fMask = scrollInfo.fMask Or ImageBoxNativeMethods.SIF.SIF_DISABLENOSCROLL
            End If
            scrollInfo.nMin = 0
            scrollInfo.nMax = scrollHeight
            scrollInfo.nPage = pageHeight

            SetScrollInfo(ScrollOrientation.VerticalScroll, scrollInfo, True)
        End Sub

        ''' <summary>
        '''   Processes the WM_HSCROLL and WM_VSCROLL Windows messages.
        ''' </summary>
        ''' <param name="msg">
        '''   The Windows <see cref="T:System.Windows.Forms.Message"/> to process.
        ''' </param>
        Protected Overridable Sub WmScroll(ByRef msg As Message)
            Dim scrollbar As ScrollOrientation
            Dim oldValue As Integer
            Dim newValue As Integer
            Dim eventType As ScrollEventType

            eventType = GetEventType(msg.WParam)
            scrollbar = If(msg.Msg = ImageBoxNativeMethods.WM_HSCROLL, ScrollOrientation.HorizontalScroll, ScrollOrientation.VerticalScroll)

            If eventType <> ScrollEventType.EndScroll Then
                Dim [step] As Integer
                Dim scrollInfo As ImageBoxNativeMethods.SCROLLINFO

                [step] = If(scrollbar = ScrollOrientation.HorizontalScroll, StepSize.Width, StepSize.Height)

                scrollInfo = GetScrollInfo(scrollbar)
                scrollInfo.fMask = ImageBoxNativeMethods.SIF.SIF_POS
                oldValue = scrollInfo.nPos

                Select Case eventType
                    Case ScrollEventType.ThumbPosition, ScrollEventType.ThumbTrack
                        scrollInfo.nPos = scrollInfo.nTrackPos

                    Case ScrollEventType.SmallDecrement
                        scrollInfo.nPos = oldValue - [step]

                    Case ScrollEventType.SmallIncrement
                        scrollInfo.nPos = oldValue + [step]

                    Case ScrollEventType.LargeDecrement
                        scrollInfo.nPos = oldValue - scrollInfo.nPage

                    Case ScrollEventType.LargeIncrement
                        scrollInfo.nPos = oldValue + scrollInfo.nPage

                    Case ScrollEventType.First
                        scrollInfo.nPos = scrollInfo.nMin

                    Case ScrollEventType.Last
                        scrollInfo.nPos = scrollInfo.nMax
                    Case Else
                        Debug.Assert(False, String.Format("Unknown scroll event type {0}", eventType))
                End Select

                If scrollInfo.nPos > scrollInfo.nMax - scrollInfo.nMin + 1 - scrollInfo.nPage Then
                    scrollInfo.nPos = scrollInfo.nMax - scrollInfo.nMin + 1 - scrollInfo.nPage
                End If

                If scrollInfo.nPos < scrollInfo.nMin Then
                    scrollInfo.nPos = scrollInfo.nMin
                End If

                newValue = scrollInfo.nPos
                SetScrollInfo(scrollbar, scrollInfo, True)
            Else
                oldValue = 0
                newValue = 0
            End If

            OnScroll(New ScrollEventArgs(eventType, oldValue, newValue, scrollbar))
        End Sub

        ''' <summary>
        '''   Gets scrollbar properties
        ''' </summary>
        ''' <param name="scrollbar">The bar.</param>
        ''' <returns></returns>
        Private Function GetScrollInfo(scrollbar As ScrollOrientation) As ImageBoxNativeMethods.SCROLLINFO
            Dim info As ImageBoxNativeMethods.SCROLLINFO

            info = New ImageBoxNativeMethods.SCROLLINFO()
            info.fMask = ImageBoxNativeMethods.SIF.SIF_ALL

            ImageBoxNativeMethods.GetScrollInfo(Handle, scrollbar, info)

            Return info
        End Function

        ''' <summary>
        '''   Sets scrollbar properties.
        ''' </summary>
        ''' <param name="scrollbar">The scrollbar.</param>
        ''' <param name="scrollInfo">The scrollbar properties.</param>
        ''' <param name="refresh">
        '''   if set to <c>true</c> the scrollbar will be repainted.
        ''' </param>
        Private Function SetScrollInfo(scrollbar As ScrollOrientation, scrollInfo As ImageBoxNativeMethods.SCROLLINFO, refresh As Boolean) As Integer ' ReSharper restore UnusedMethodReturnValue.Local
            Return ImageBoxNativeMethods.SetScrollInfo(Handle, scrollbar, scrollInfo, refresh)
        End Function

    End Class
End Namespace
