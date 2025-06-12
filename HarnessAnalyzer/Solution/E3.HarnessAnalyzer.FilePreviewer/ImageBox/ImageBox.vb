' Cyotek ImageBox
' http://cyotek.com/blog/tag/imagebox
' Copyright (c) 2010-2021 Cyotek Ltd.
' This work is licensed under the MIT License.

Imports System
Imports System.ComponentModel
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Windows.Forms

Namespace Global.System.Windows.Forms
    ''' <summary>
    '''   Component for displaying images with support for scrolling and zooming.
    ''' </summary>
    <DefaultProperty("Image")>
    <ToolboxBitmap(GetType(ImageBox), "ImageBox.bmp")>
    <ToolboxItem(True)>
    Friend Class ImageBox
        Inherits VirtualScrollableControl
#Region "Constants"

        Private Shared ReadOnly _eventAllowClickZoomChanged As Object = New Object()

        Private Shared ReadOnly _eventAllowDoubleClickChanged As Object = New Object()

        Private Shared ReadOnly _eventAllowFreePanChanged As Object = New Object()

        Private Shared ReadOnly _eventAllowUnfocusedMouseWheelChanged As Object = New Object()

        Private Shared ReadOnly _eventAllowZoomChanged As Object = New Object()

        Private Shared ReadOnly _eventAutoCenterChanged As Object = New Object()

        Private Shared ReadOnly _eventAutoPanChanged As Object = New Object()

        Private Shared ReadOnly _eventDropShadowSizeChanged As Object = New Object()

        Private Shared ReadOnly _eventGridCellSizeChanged As Object = New Object()

        Private Shared ReadOnly _eventGridColorAlternateChanged As Object = New Object()

        Private Shared ReadOnly _eventGridColorChanged As Object = New Object()

        Private Shared ReadOnly _eventGridDisplayModeChanged As Object = New Object()

        Private Shared ReadOnly _eventGridScaleChanged As Object = New Object()

        Private Shared ReadOnly _eventImageBorderColorChanged As Object = New Object()

        Private Shared ReadOnly _eventImageBorderStyleChanged As Object = New Object()

        Private Shared ReadOnly _eventImageChanged As Object = New Object()

        Private Shared ReadOnly _eventInterpolationModeChanged As Object = New Object()

        Private Shared ReadOnly _eventInvertMouseChanged As Object = New Object()

        Private Shared ReadOnly _eventLimitSelectionToImageChanged As Object = New Object()

        Private Shared ReadOnly _eventMouseWheelModeChanged As Object = New Object()

        Private Shared ReadOnly _eventPanEnd As Object = New Object()

        Private Shared ReadOnly _eventPanModeChanged As Object = New Object()

        Private Shared ReadOnly _eventPanStart As Object = New Object()

        Private Shared ReadOnly _eventPixelGridColorChanged As Object = New Object()

        Private Shared ReadOnly _eventPixelGridThresholdChanged As Object = New Object()

        Private Shared ReadOnly _eventScaleTextChanged As Object = New Object()

        Private Shared ReadOnly _eventSelected As Object = New Object()

        Private Shared ReadOnly _eventSelecting As Object = New Object()

        Private Shared ReadOnly _eventSelectionColorChanged As Object = New Object()

        Private Shared ReadOnly _eventSelectionModeChanged As Object = New Object()

        Private Shared ReadOnly _eventSelectionRegionChanged As Object = New Object()

        Private Shared ReadOnly _eventShortcutsEnabledChanged As Object = New Object()

        Private Shared ReadOnly _eventShowPixelGridChanged As Object = New Object()

        Private Shared ReadOnly _eventSizeModeChanged As Object = New Object()

        Private Shared ReadOnly _eventSizeToFitChanged As Object = New Object()

        Private Shared ReadOnly _eventTextAlignChanged As Object = New Object()

        Private Shared ReadOnly _eventTextBackColorChanged As Object = New Object()

        Private Shared ReadOnly _eventTextDisplayModeChanged As Object = New Object()

        Private Shared ReadOnly _eventTextPaddingChanged As Object = New Object()

        Private Shared ReadOnly _eventVirtualDraw As Object = New Object()

        Private Shared ReadOnly _eventVirtualModeChanged As Object = New Object()

        Private Shared ReadOnly _eventVirtualSizeChanged As Object = New Object()

        Private Shared ReadOnly _eventZoomChanged As Object = New Object()

        Private Shared ReadOnly _eventZoomed As Object = New Object()

        Private Shared ReadOnly _eventZoomLevelsChanged As Object = New Object()

        Private Const _freePanTimerInterval As Integer = 250

        Private Const _panAllDeadSize As Integer = 32

        Private Const MaxZoom As Integer = 3500

        Private Const MinZoom As Integer = 1

        Private Const SelectionDeadZone As Integer = 5

#End Region

#Region "Fields"

        Private _allowClickZoom As Boolean

        Private _allowDoubleClick As Boolean

        Private _allowFreePan As Boolean

        Private _allowUnfocusedMouseWheel As Boolean

        Private _allowZoom As Boolean

        Private _autoCenter As Boolean

        Private _currentCursor As Cursor

        Private _dropShadowSize As Integer

        Private _freePanTimer As Timer

        Private _gridCellSize As Integer

        Private _gridColor As Color

        Private _gridColorAlternate As Color

        Private _gridDisplayMode As ImageBoxGridDisplayMode

        Private _gridScale As ImageBoxGridScale

        Private _gridTile As Bitmap

        Private _image As Image

        Private _imageBorderColor As Color

        Private _imageBorderStyle As ImageBoxBorderStyle

        Private _interpolationMode As InterpolationMode

        Private _invertMouse As Boolean

        Private _limitSelectionToImage As Boolean

        Private _mouseDownStart As Double

        Private _mouseWheelMode As ImageBoxMouseWheelMode

        Private _panMode As ImageBoxPanMode

        Private _panStyle As ImageBoxPanStyle

        Private _pixelGridColor As Color

        Private _pixelGridThreshold As Integer

        Private _scaleText As Boolean

        Private _selectionColor As Color

        Private _selectionMode As ImageBoxSelectionMode

        Private _selectionRegion As RectangleF

        Private _shortcutsEnabled As Boolean

        Private _showPixelGrid As Boolean

        Private _sizeMode As ImageBoxSizeMode

        Private _startMousePosition As System.Drawing.Point

        Private _startScrollPosition As System.Drawing.Point

        Private _textAlign As ContentAlignment

        Private _textBackColor As Color

        Private _textDisplayMode As ImageBoxGridDisplayMode

        Private _textPadding As Padding

        Private _texture As Brush

        Private _updateCount As Integer

        Private _virtualMode As Boolean

        Private _virtualSize As System.Drawing.Size

        Private _zoom As Integer

        Private _zoomLevels As ZoomLevelCollection
        Private _isSelecting As Boolean

#End Region

#Region "Constructors"

        ''' <summary>
        '''   Initializes a new instance of the <see cref="ImageBox"/> class.
        ''' </summary>
        Friend Sub New()
            SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.UserPaint Or ControlStyles.OptimizedDoubleBuffer Or ControlStyles.ResizeRedraw, True)
            SetStyle(ControlStyles.StandardDoubleClick, False)

            ' ReSharper disable DoNotCallOverridableMethodsInConstructor
            BeginUpdate()
            _panMode = ImageBoxPanMode.Both
            _allowFreePan = True
            WheelScrollsControl = False
            AllowZoom = True
            MouseWheelMode = ImageBoxMouseWheelMode.Zoom
            LimitSelectionToImage = True
            DropShadowSize = 3
            ImageBorderStyle = ImageBoxBorderStyle.None
            BackColor = Color.White
            AutoSize = False
            AutoScroll = True
            GridScale = ImageBoxGridScale.Small
            GridDisplayMode = ImageBoxGridDisplayMode.Client
            GridColor = Color.Gainsboro
            GridColorAlternate = Color.White
            GridCellSize = 8
            InterpolationMode = InterpolationMode.NearestNeighbor
            AutoCenter = True
            SelectionColor = System.Drawing.SystemColors.Highlight
            ActualSize()
            ShortcutsEnabled = True
            ZoomLevels = ZoomLevelCollection.Default
            ImageBorderColor = System.Drawing.SystemColors.ControlDark
            PixelGridColor = Color.DimGray
            PixelGridThreshold = 5
            TextAlign = ContentAlignment.MiddleCenter
            TextBackColor = Color.Transparent
            TextDisplayMode = ImageBoxGridDisplayMode.Client
            EndUpdate()
            ' ReSharper restore DoNotCallOverridableMethodsInConstructor
        End Sub

#End Region

#Region "Events"

        ''' <summary>
        '''   Occurs when the AllowClickZoom property is changed.
        ''' </summary>
        <Category("Property Changed")>
        Friend Custom Event AllowClickZoomChanged As EventHandler
            AddHandler(value As EventHandler)
                Events.AddHandler(_eventAllowClickZoomChanged, value)
            End AddHandler
            RemoveHandler(value As EventHandler)
                Events.RemoveHandler(_eventAllowClickZoomChanged, value)
            End RemoveHandler
            <Category("Property Changed")>
            RaiseEvent(sender As Object, e As EventArgs)
            End RaiseEvent
        End Event

        ''' <summary>
        '''   Occurs when the AllowDoubleClick property value changes
        ''' </summary>
        <Category("Property Changed")>
        Friend Custom Event AllowDoubleClickChanged As EventHandler
            AddHandler(value As EventHandler)
                Events.AddHandler(_eventAllowDoubleClickChanged, value)
            End AddHandler
            RemoveHandler(value As EventHandler)
                Events.RemoveHandler(_eventAllowDoubleClickChanged, value)
            End RemoveHandler
            <Category("Property Changed")>
            RaiseEvent(sender As Object, e As EventArgs)
            End RaiseEvent
        End Event

        ''' <summary>
        ''' Occurs when the AllowFreePan property value changes
        ''' </summary>
        <Category("Property Changed")>
        Friend Custom Event AllowFreePanChanged As EventHandler
            AddHandler(value As EventHandler)
                Events.AddHandler(_eventAllowFreePanChanged, value)
            End AddHandler
            RemoveHandler(value As EventHandler)
                Events.RemoveHandler(_eventAllowFreePanChanged, value)
            End RemoveHandler
            <Category("Property Changed")>
            RaiseEvent(sender As Object, e As EventArgs)
            End RaiseEvent
        End Event

        ''' <summary>
        ''' Occurs when the AllowUnfocusedMouseWheel property value changes
        ''' </summary>
        <Category("Property Changed")>
        Friend Custom Event AllowUnfocusedMouseWheelChanged As EventHandler
            AddHandler(value As EventHandler)
                Events.AddHandler(_eventAllowUnfocusedMouseWheelChanged, value)
            End AddHandler
            RemoveHandler(value As EventHandler)
                Events.RemoveHandler(_eventAllowUnfocusedMouseWheelChanged, value)
            End RemoveHandler
            <Category("Property Changed")>
            RaiseEvent(sender As Object, e As EventArgs)
            End RaiseEvent
        End Event

        ''' <summary>
        '''   Occurs when the AllowZoom property is changed.
        ''' </summary>
        <Category("Property Changed")>
        Friend Custom Event AllowZoomChanged As EventHandler
            AddHandler(value As EventHandler)
                Events.AddHandler(_eventAllowZoomChanged, value)
            End AddHandler
            RemoveHandler(value As EventHandler)
                Events.RemoveHandler(_eventAllowZoomChanged, value)
            End RemoveHandler
            <Category("Property Changed")>
            RaiseEvent(sender As Object, e As EventArgs)
            End RaiseEvent
        End Event

        ''' <summary>
        '''   Occurs when the AutoCenter property is changed.
        ''' </summary>
        <Category("Property Changed")>
        Friend Custom Event AutoCenterChanged As EventHandler
            AddHandler(value As EventHandler)
                Events.AddHandler(_eventAutoCenterChanged, value)
            End AddHandler
            RemoveHandler(value As EventHandler)
                Events.RemoveHandler(_eventAutoCenterChanged, value)
            End RemoveHandler
            <Category("Property Changed")>
            RaiseEvent(sender As Object, e As EventArgs)
            End RaiseEvent
        End Event

        ''' <summary>
        '''   Occurs when the AutoPan property is changed.
        ''' </summary>
        <Category("Property Changed")>
        Friend Custom Event AutoPanChanged As EventHandler
            AddHandler(value As EventHandler)
                Events.AddHandler(_eventAutoPanChanged, value)
            End AddHandler
            RemoveHandler(value As EventHandler)
                Events.RemoveHandler(_eventAutoPanChanged, value)
            End RemoveHandler
            <Category("Property Changed")>
            RaiseEvent(sender As Object, e As EventArgs)
            End RaiseEvent
        End Event

        ''' <summary>
        '''   Occurs when the DropShadowSize property is changed.
        ''' </summary>
        <Category("Property Changed")>
        Friend Custom Event DropShadowSizeChanged As EventHandler
            AddHandler(value As EventHandler)
                Events.AddHandler(_eventDropShadowSizeChanged, value)
            End AddHandler
            RemoveHandler(value As EventHandler)
                Events.RemoveHandler(_eventDropShadowSizeChanged, value)
            End RemoveHandler
            <Category("Property Changed")>
            RaiseEvent(sender As Object, e As EventArgs)
            End RaiseEvent
        End Event

        ''' <summary>
        '''   Occurs when the GridSizeCell property is changed.
        ''' </summary>
        <Category("Property Changed")>
        Friend Custom Event GridCellSizeChanged As EventHandler
            AddHandler(value As EventHandler)
                Events.AddHandler(_eventGridCellSizeChanged, value)
            End AddHandler
            RemoveHandler(value As EventHandler)
                Events.RemoveHandler(_eventGridCellSizeChanged, value)
            End RemoveHandler
            <Category("Property Changed")>
            RaiseEvent(sender As Object, e As EventArgs)
            End RaiseEvent
        End Event

        ''' <summary>
        '''   Occurs when the GridColorAlternate property is changed.
        ''' </summary>
        <Category("Property Changed")>
        Friend Custom Event GridColorAlternateChanged As EventHandler
            AddHandler(value As EventHandler)
                Events.AddHandler(_eventGridColorAlternateChanged, value)
            End AddHandler
            RemoveHandler(value As EventHandler)
                Events.RemoveHandler(_eventGridColorAlternateChanged, value)
            End RemoveHandler
            <Category("Property Changed")>
            RaiseEvent(sender As Object, e As EventArgs)
            End RaiseEvent
        End Event

        ''' <summary>
        '''   Occurs when the GridColor property is changed.
        ''' </summary>
        <Category("Property Changed")>
        Friend Custom Event GridColorChanged As EventHandler
            AddHandler(value As EventHandler)
                Events.AddHandler(_eventGridColorChanged, value)
            End AddHandler
            RemoveHandler(value As EventHandler)
                Events.RemoveHandler(_eventGridColorChanged, value)
            End RemoveHandler
            <Category("Property Changed")>
            RaiseEvent(sender As Object, e As EventArgs)
            End RaiseEvent
        End Event

        ''' <summary>
        '''   Occurs when the GridDisplayMode property is changed.
        ''' </summary>
        <Category("Property Changed")>
        Friend Custom Event GridDisplayModeChanged As EventHandler
            AddHandler(value As EventHandler)
                Events.AddHandler(_eventGridDisplayModeChanged, value)
            End AddHandler
            RemoveHandler(value As EventHandler)
                Events.RemoveHandler(_eventGridDisplayModeChanged, value)
            End RemoveHandler
            <Category("Property Changed")>
            RaiseEvent(sender As Object, e As EventArgs)
            End RaiseEvent
        End Event

        ''' <summary>
        '''   Occurs when the GridScale property is changed.
        ''' </summary>
        <Category("Property Changed")>
        Friend Custom Event GridScaleChanged As EventHandler
            AddHandler(value As EventHandler)
                Events.AddHandler(_eventGridScaleChanged, value)
            End AddHandler
            RemoveHandler(value As EventHandler)
                Events.RemoveHandler(_eventGridScaleChanged, value)
            End RemoveHandler
            <Category("Property Changed")>
            RaiseEvent(sender As Object, e As EventArgs)
            End RaiseEvent
        End Event

        ''' <summary>
        '''   Occurs when the ImageBorderColor property value changes
        ''' </summary>
        <Category("Property Changed")>
        Friend Custom Event ImageBorderColorChanged As EventHandler
            AddHandler(value As EventHandler)
                Events.AddHandler(_eventImageBorderColorChanged, value)
            End AddHandler
            RemoveHandler(value As EventHandler)
                Events.RemoveHandler(_eventImageBorderColorChanged, value)
            End RemoveHandler
            <Category("Property Changed")>
            RaiseEvent(sender As Object, e As EventArgs)
            End RaiseEvent
        End Event

        ''' <summary>
        '''   Occurs when the ImageBorderStyle property is changed.
        ''' </summary>
        <Category("Property Changed")>
        Friend Custom Event ImageBorderStyleChanged As EventHandler
            AddHandler(value As EventHandler)
                Events.AddHandler(_eventImageBorderStyleChanged, value)
            End AddHandler
            RemoveHandler(value As EventHandler)
                Events.RemoveHandler(_eventImageBorderStyleChanged, value)
            End RemoveHandler
            <Category("Property Changed")>
            RaiseEvent(sender As Object, e As EventArgs)
            End RaiseEvent
        End Event

        ''' <summary>
        '''   Occurs when the Image property is changed.
        ''' </summary>
        <Category("Property Changed")>
        Friend Custom Event ImageChanged As EventHandler
            AddHandler(value As EventHandler)
                Events.AddHandler(_eventImageChanged, value)
            End AddHandler
            RemoveHandler(value As EventHandler)
                Events.RemoveHandler(_eventImageChanged, value)
            End RemoveHandler
            <Category("Property Changed")>
            RaiseEvent(sender As Object, e As EventArgs)
            End RaiseEvent
        End Event

        ''' <summary>
        '''   Occurs when the InterpolationMode property is changed.
        ''' </summary>
        <Category("Property Changed")>
        Friend Custom Event InterpolationModeChanged As EventHandler
            AddHandler(value As EventHandler)
                Events.AddHandler(_eventInterpolationModeChanged, value)
            End AddHandler
            RemoveHandler(value As EventHandler)
                Events.RemoveHandler(_eventInterpolationModeChanged, value)
            End RemoveHandler
            <Category("Property Changed")>
            RaiseEvent(sender As Object, e As EventArgs)
            End RaiseEvent
        End Event

        ''' <summary>
        '''   Occurs when the InvertMouse property is changed.
        ''' </summary>
        <Category("Property Changed")>
        Friend Custom Event InvertMouseChanged As EventHandler
            AddHandler(value As EventHandler)
                Events.AddHandler(_eventInvertMouseChanged, value)
            End AddHandler
            RemoveHandler(value As EventHandler)
                Events.RemoveHandler(_eventInvertMouseChanged, value)
            End RemoveHandler
            <Category("Property Changed")>
            RaiseEvent(sender As Object, e As EventArgs)
            End RaiseEvent
        End Event

        ''' <summary>
        '''   Occurs when the LimitSelectionToImage property is changed.
        ''' </summary>
        <Category("Property Changed")>
        Friend Custom Event LimitSelectionToImageChanged As EventHandler
            AddHandler(value As EventHandler)
                Events.AddHandler(_eventLimitSelectionToImageChanged, value)
            End AddHandler
            RemoveHandler(value As EventHandler)
                Events.RemoveHandler(_eventLimitSelectionToImageChanged, value)
            End RemoveHandler
            <Category("Property Changed")>
            RaiseEvent(sender As Object, e As EventArgs)
            End RaiseEvent
        End Event

        ''' <summary>
        ''' Occurs when the MouseWheelMode property value changes
        ''' </summary>
        <Category("Property Changed")>
        Friend Custom Event MouseWheelModeChanged As EventHandler
            AddHandler(value As EventHandler)
                Events.AddHandler(_eventMouseWheelModeChanged, value)
            End AddHandler
            RemoveHandler(value As EventHandler)
                Events.RemoveHandler(_eventMouseWheelModeChanged, value)
            End RemoveHandler
            <Category("Property Changed")>
            RaiseEvent(sender As Object, e As EventArgs)
            End RaiseEvent
        End Event

        ''' <summary>
        '''   Occurs when panning the control completes.
        ''' </summary>
        <Category("Action")>
        Friend Custom Event PanEnd As EventHandler
            AddHandler(value As EventHandler)
                Events.AddHandler(_eventPanEnd, value)
            End AddHandler
            RemoveHandler(value As EventHandler)
                Events.RemoveHandler(_eventPanEnd, value)
            End RemoveHandler
            <Category("Action")>
            RaiseEvent(sender As Object, e As EventArgs)
            End RaiseEvent
        End Event

        ''' <summary>
        ''' Occurs when the PanMode property value changes
        ''' </summary>
        <Category("Property Changed")>
        Friend Custom Event PanModeChanged As EventHandler
            AddHandler(value As EventHandler)
                Events.AddHandler(_eventPanModeChanged, value)
            End AddHandler
            RemoveHandler(value As EventHandler)
                Events.RemoveHandler(_eventPanModeChanged, value)
            End RemoveHandler
            <Category("Property Changed")>
            RaiseEvent(sender As Object, e As EventArgs)
            End RaiseEvent
        End Event

        ''' <summary>
        '''   Occurs when panning the control starts.
        ''' </summary>
        <Category("Action")>
        Friend Custom Event PanStart As EventHandler
            AddHandler(value As EventHandler)
                Events.AddHandler(_eventPanStart, value)
            End AddHandler
            RemoveHandler(value As EventHandler)
                Events.RemoveHandler(_eventPanStart, value)
            End RemoveHandler
            <Category("Action")>
            RaiseEvent(sender As Object, e As EventArgs)
            End RaiseEvent
        End Event

        ''' <summary>
        '''   Occurs when the PixelGridColor property value changes
        ''' </summary>
        <Category("Property Changed")>
        Friend Custom Event PixelGridColorChanged As EventHandler
            AddHandler(value As EventHandler)
                Events.AddHandler(_eventPixelGridColorChanged, value)
            End AddHandler
            RemoveHandler(value As EventHandler)
                Events.RemoveHandler(_eventPixelGridColorChanged, value)
            End RemoveHandler
            <Category("Property Changed")>
            RaiseEvent(sender As Object, e As EventArgs)
            End RaiseEvent
        End Event

        ''' <summary>
        ''' Occurs when the PixelGridThreshold property value changes
        ''' </summary>
        <Category("Property Changed")>
        Friend Custom Event PixelGridThresholdChanged As EventHandler
            AddHandler(value As EventHandler)
                Events.AddHandler(_eventPixelGridThresholdChanged, value)
            End AddHandler
            RemoveHandler(value As EventHandler)
                Events.RemoveHandler(_eventPixelGridThresholdChanged, value)
            End RemoveHandler
            <Category("Property Changed")>
            RaiseEvent(sender As Object, e As EventArgs)
            End RaiseEvent
        End Event

        ''' <summary>
        ''' Occurs when the ScaleText property value changes
        ''' </summary>
        <Category("Property Changed")>
        Friend Custom Event ScaleTextChanged As EventHandler
            AddHandler(value As EventHandler)
                Events.AddHandler(_eventScaleTextChanged, value)
            End AddHandler
            RemoveHandler(value As EventHandler)
                Events.RemoveHandler(_eventScaleTextChanged, value)
            End RemoveHandler
            <Category("Property Changed")>
            RaiseEvent(sender As Object, e As EventArgs)
            End RaiseEvent
        End Event

        ''' <summary>
        '''   Occurs when a selection region has been defined
        ''' </summary>
        <Category("Action")>
        Friend Custom Event Selected As EventHandler(Of EventArgs)
            ' TODO: The event signature is wrong and should just be EventHandler - breaking change however. Do in the 2.0 scroll changes branch.
            AddHandler(value As EventHandler(Of EventArgs))
                Events.AddHandler(_eventSelected, value)
            End AddHandler
            RemoveHandler(value As EventHandler(Of EventArgs))
                Events.RemoveHandler(_eventSelected, value)
            End RemoveHandler
            <Category("Action")>
            RaiseEvent(sender As Object, e As EventArgs)
            End RaiseEvent
        End Event

        ''' <summary>
        '''   Occurs when the user starts to define a selection region.
        ''' </summary>
        <Category("Action")>
        Friend Custom Event Selecting As EventHandler(Of ImageBoxCancelEventArgs)
            AddHandler(value As EventHandler(Of ImageBoxCancelEventArgs))
                Events.AddHandler(_eventSelecting, value)
            End AddHandler
            RemoveHandler(value As EventHandler(Of ImageBoxCancelEventArgs))
                Events.RemoveHandler(_eventSelecting, value)
            End RemoveHandler
            <Category("Action")>
            RaiseEvent(sender As Object, e As ImageBoxCancelEventArgs)
            End RaiseEvent
        End Event

        ''' <summary>
        '''   Occurs when the SelectionColor property is changed.
        ''' </summary>
        <Category("Property Changed")>
        Friend Custom Event SelectionColorChanged As EventHandler
            AddHandler(value As EventHandler)
                Events.AddHandler(_eventSelectionColorChanged, value)
            End AddHandler
            RemoveHandler(value As EventHandler)
                Events.RemoveHandler(_eventSelectionColorChanged, value)
            End RemoveHandler
            <Category("Property Changed")>
            RaiseEvent(sender As Object, e As EventArgs)
            End RaiseEvent
        End Event

        ''' <summary>
        '''   Occurs when the SelectionMode property is changed.
        ''' </summary>
        <Category("Property Changed")>
        Friend Custom Event SelectionModeChanged As EventHandler
            AddHandler(value As EventHandler)
                Events.AddHandler(_eventSelectionModeChanged, value)
            End AddHandler
            RemoveHandler(value As EventHandler)
                Events.RemoveHandler(_eventSelectionModeChanged, value)
            End RemoveHandler
            <Category("Property Changed")>
            RaiseEvent(sender As Object, e As EventArgs)
            End RaiseEvent
        End Event

        ''' <summary>
        '''   Occurs when the SelectionRegion property is changed.
        ''' </summary>
        <Category("Property Changed")>
        Friend Custom Event SelectionRegionChanged As EventHandler
            AddHandler(value As EventHandler)
                Events.AddHandler(_eventSelectionRegionChanged, value)
            End AddHandler
            RemoveHandler(value As EventHandler)
                Events.RemoveHandler(_eventSelectionRegionChanged, value)
            End RemoveHandler
            <Category("Property Changed")>
            RaiseEvent(sender As Object, e As EventArgs)
            End RaiseEvent
        End Event

        ''' <summary>
        '''   Occurs when the ShortcutsEnabled property value changes
        ''' </summary>
        <Category("Property Changed")>
        Friend Custom Event ShortcutsEnabledChanged As EventHandler
            AddHandler(value As EventHandler)
                Events.AddHandler(_eventShortcutsEnabledChanged, value)
            End AddHandler
            RemoveHandler(value As EventHandler)
                Events.RemoveHandler(_eventShortcutsEnabledChanged, value)
            End RemoveHandler
            <Category("Property Changed")>
            RaiseEvent(sender As Object, e As EventArgs)
            End RaiseEvent
        End Event

        ''' <summary>
        '''   Occurs when the ShowPixelGrid property value changes
        ''' </summary>
        <Category("Property Changed")>
        Friend Custom Event ShowPixelGridChanged As EventHandler
            AddHandler(value As EventHandler)
                Events.AddHandler(_eventShowPixelGridChanged, value)
            End AddHandler
            RemoveHandler(value As EventHandler)
                Events.RemoveHandler(_eventShowPixelGridChanged, value)
            End RemoveHandler
            <Category("Property Changed")>
            RaiseEvent(sender As Object, e As EventArgs)
            End RaiseEvent
        End Event

        ''' <summary>
        ''' Occurs when the SizeMode property value changes
        ''' </summary>
        <Category("Property Changed")>
        Friend Custom Event SizeModeChanged As EventHandler
            AddHandler(value As EventHandler)
                Events.AddHandler(_eventSizeModeChanged, value)
            End AddHandler
            RemoveHandler(value As EventHandler)
                Events.RemoveHandler(_eventSizeModeChanged, value)
            End RemoveHandler
            <Category("Property Changed")>
            RaiseEvent(sender As Object, e As EventArgs)
            End RaiseEvent
        End Event

        ''' <summary>
        '''   Occurs when the SizeToFit property is changed.
        ''' </summary>
        <Category("Property Changed")>
        Friend Custom Event SizeToFitChanged As EventHandler
            AddHandler(value As EventHandler)
                Events.AddHandler(_eventSizeToFitChanged, value)
            End AddHandler
            RemoveHandler(value As EventHandler)
                Events.RemoveHandler(_eventSizeToFitChanged, value)
            End RemoveHandler
            <Category("Property Changed")>
            RaiseEvent(sender As Object, e As EventArgs)
            End RaiseEvent
        End Event

        ''' <summary>
        ''' Occurs when the TextAlign property value changes
        ''' </summary>
        <Category("Property Changed")>
        Friend Custom Event TextAlignChanged As EventHandler
            AddHandler(value As EventHandler)
                Events.AddHandler(_eventTextAlignChanged, value)
            End AddHandler
            RemoveHandler(value As EventHandler)
                Events.RemoveHandler(_eventTextAlignChanged, value)
            End RemoveHandler
            <Category("Property Changed")>
            RaiseEvent(sender As Object, e As EventArgs)
            End RaiseEvent
        End Event

        ''' <summary>
        ''' Occurs when the TextBackColor property value changes
        ''' </summary>
        <Category("Property Changed")>
        Friend Custom Event TextBackColorChanged As EventHandler
            AddHandler(value As EventHandler)
                Events.AddHandler(_eventTextBackColorChanged, value)
            End AddHandler
            RemoveHandler(value As EventHandler)
                Events.RemoveHandler(_eventTextBackColorChanged, value)
            End RemoveHandler
            <Category("Property Changed")>
            RaiseEvent(sender As Object, e As EventArgs)
            End RaiseEvent
        End Event

        ''' <summary>
        ''' Occurs when the TextDisplayMode property value changes
        ''' </summary>
        <Category("Property Changed")>
        Friend Custom Event TextDisplayModeChanged As EventHandler
            AddHandler(value As EventHandler)
                Events.AddHandler(_eventTextDisplayModeChanged, value)
            End AddHandler
            RemoveHandler(value As EventHandler)
                Events.RemoveHandler(_eventTextDisplayModeChanged, value)
            End RemoveHandler
            <Category("Property Changed")>
            RaiseEvent(sender As Object, e As EventArgs)
            End RaiseEvent
        End Event

        ''' <summary>
        ''' Occurs when the TextPadding property value changes
        ''' </summary>
        <Category("Property Changed")>
        Friend Custom Event TextPaddingChanged As EventHandler
            AddHandler(value As EventHandler)
                Events.AddHandler(_eventTextPaddingChanged, value)
            End AddHandler
            RemoveHandler(value As EventHandler)
                Events.RemoveHandler(_eventTextPaddingChanged, value)
            End RemoveHandler
            <Category("Property Changed")>
            RaiseEvent(sender As Object, e As EventArgs)
            End RaiseEvent
        End Event

        ''' <summary>
        '''   Occurs when virtual painting should occur
        ''' </summary>
        <Category("Appearance")>
        Friend Custom Event VirtualDraw As PaintEventHandler
            AddHandler(value As PaintEventHandler)
                Events.AddHandler(_eventVirtualDraw, value)
            End AddHandler
            RemoveHandler(value As PaintEventHandler)
                Events.RemoveHandler(_eventVirtualDraw, value)
            End RemoveHandler
            <Category("Appearance")>
            RaiseEvent(sender As Object, e As PaintEventArgs)
            End RaiseEvent
        End Event

        ''' <summary>
        '''   Occurs when the VirtualMode property value changes
        ''' </summary>
        <Category("Property Changed")>
        Friend Custom Event VirtualModeChanged As EventHandler
            AddHandler(value As EventHandler)
                Events.AddHandler(_eventVirtualModeChanged, value)
            End AddHandler
            RemoveHandler(value As EventHandler)
                Events.RemoveHandler(_eventVirtualModeChanged, value)
            End RemoveHandler
            <Category("Property Changed")>
            RaiseEvent(sender As Object, e As EventArgs)
            End RaiseEvent
        End Event

        ''' <summary>
        '''   Occurs when the VirtualSize property value changes
        ''' </summary>
        <Category("Property Changed")>
        Friend Custom Event VirtualSizeChanged As EventHandler
            AddHandler(value As EventHandler)
                Events.AddHandler(_eventVirtualSizeChanged, value)
            End AddHandler
            RemoveHandler(value As EventHandler)
                Events.RemoveHandler(_eventVirtualSizeChanged, value)
            End RemoveHandler
            <Category("Property Changed")>
            RaiseEvent(sender As Object, e As EventArgs)
            End RaiseEvent
        End Event

        ''' <summary>
        '''   Occurs when the Zoom property is changed.
        ''' </summary>
        <Category("Property Changed")>
        Friend Custom Event ZoomChanged As EventHandler
            AddHandler(value As EventHandler)
                Events.AddHandler(_eventZoomChanged, value)
            End AddHandler
            RemoveHandler(value As EventHandler)
                Events.RemoveHandler(_eventZoomChanged, value)
            End RemoveHandler
            <Category("Property Changed")>
            RaiseEvent(sender As Object, e As EventArgs)
            End RaiseEvent
        End Event

        ''' <summary>
        ''' Occurs when then a zoom action is performed.
        ''' </summary>
        <Category("Action")>
        Friend Custom Event Zoomed As EventHandler(Of ImageBoxZoomEventArgs)
            AddHandler(value As EventHandler(Of ImageBoxZoomEventArgs))
                Events.AddHandler(_eventZoomed, value)
            End AddHandler
            RemoveHandler(value As EventHandler(Of ImageBoxZoomEventArgs))
                Events.RemoveHandler(_eventZoomed, value)
            End RemoveHandler
            <Category("Action")>
            RaiseEvent(sender As Object, e As ImageBoxZoomEventArgs)
            End RaiseEvent
        End Event

        ''' <summary>
        '''   Occurs when the ZoomLevels property is changed
        ''' </summary>
        <Category("Property Changed")>
        Friend Custom Event ZoomLevelsChanged As EventHandler
            AddHandler(value As EventHandler)
                Events.AddHandler(_eventZoomLevelsChanged, value)
            End AddHandler
            RemoveHandler(value As EventHandler)
                Events.RemoveHandler(_eventZoomLevelsChanged, value)
            End RemoveHandler
            <Category("Property Changed")>
            RaiseEvent(sender As Object, e As EventArgs)
            End RaiseEvent
        End Event

#End Region

#Region "Static Methods"

        ''' <summary>
        '''   Creates a bitmap image containing a 2x2 grid using the specified cell System.Drawing.Size and colors.
        ''' </summary>
        ''' <param name="cellSize">System.Drawing.Size of the cell.</param>
        ''' <param name="cellColor">Cell color.</param>
        ''' <param name="alternateCellColor">Alternate cell color.</param>
        ''' <returns></returns>
        Friend Shared Function CreateCheckerBoxTile(cellSize As Integer, cellColor As Color, alternateCellColor As Color) As Bitmap
            Dim result As Bitmap
            Dim width As Integer
            Dim height As Integer

            ' draw the tile
            width = cellSize * 2
            height = cellSize * 2
            result = New Bitmap(width, height)

            Using g = Graphics.FromImage(result)
                Using brush As Brush = New SolidBrush(cellColor)
                    g.FillRectangle(brush, New Rectangle(cellSize, 0, cellSize, cellSize))
                    g.FillRectangle(brush, New Rectangle(0, cellSize, cellSize, cellSize))
                End Using

                Using brush As Brush = New SolidBrush(alternateCellColor)
                    g.FillRectangle(brush, New Rectangle(0, 0, cellSize, cellSize))
                    g.FillRectangle(brush, New Rectangle(cellSize, cellSize, cellSize, cellSize))
                End Using
            End Using

            Return result
        End Function

        ''' <summary>
        '''   Creates a checked tile texture using default values.
        ''' </summary>
        ''' <returns></returns>
        Friend Shared Function CreateCheckerBoxTile() As Bitmap
            Return CreateCheckerBoxTile(8, Color.Gainsboro, Color.WhiteSmoke)
        End Function

        Private Shared Function GetPanAllCursor() As Cursor
            Dim type As Type

            type = GetType(ImageBox)

            Return New Cursor(type.Assembly.GetManifestResourceStream(type.Namespace & ".PanAll.cur"))
        End Function

        Private Shared Function GetPanAllSymbol() As Bitmap
            Dim type As Type

            type = GetType(ImageBox)

            Using stream = type.Assembly.GetManifestResourceStream(type.Namespace & ".PanAllSymbol.png")
                Return New Bitmap(stream)
            End Using
        End Function

        Private Shared Sub LoadPanResources()
            If _panAllCursor Is Nothing Then
                _panAllCursor = GetPanAllCursor()
            End If

            If _panAllSymbol Is Nothing Then
                _panAllSymbol = GetPanAllSymbol()
            End If
        End Sub

#End Region

#Region "Properties"

        ''' <summary>
        '''   Gets or sets a value indicating whether clicking the control with the mouse will automatically zoom in or out.
        ''' </summary>
        ''' <value>
        '''   <c>true</c> if clicking the control allows zooming; otherwise, <c>false</c>.
        ''' </value>
        <DefaultValue(False)>
        <Category("Behavior")>
        Friend Overridable Property AllowClickZoom As Boolean
            Get
                Return _allowClickZoom
            End Get
            Set(value As Boolean)
                If _allowClickZoom <> value Then
                    _allowClickZoom = value
                    OnAllowClickZoomChanged(EventArgs.Empty)
                End If
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating whether the DoubleClick event can be raised.
        ''' </summary>
        ''' <value><c>true</c> if the DoubleClick event can be raised; otherwise, <c>false</c>.</value>
        <Category("Behavior")>
        <DefaultValue(False)>
        Friend Overridable Property AllowDoubleClick As Boolean
            Get
                Return _allowDoubleClick
            End Get
            Set(value As Boolean)
                If AllowDoubleClick <> value Then
                    _allowDoubleClick = value

                    OnAllowDoubleClickChanged(EventArgs.Empty)
                End If
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating whether free panning can be used
        ''' </summary>
        ''' <value>
        ''' <c>true</c> if free panning can be used, otherwise <c>false</c>.
        ''' </value>
        <Category("Behavior")>
        <DefaultValue(True)>
        Friend Overridable Property AllowFreePan As Boolean
            Get
                Return _allowFreePan
            End Get
            Set(value As Boolean)
                If _allowFreePan <> value Then
                    _allowFreePan = value

                    OnAllowFreePanChanged(EventArgs.Empty)
                End If
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating whether the mouse wheel is processed event if the <see cref="ImageBox"/> doesn't have focus.
        ''' </summary>
        ''' <value>
        ''' <c>true</c> if the mouse wheel is processed regardless of focus, otherwise <c>false</c> to only process the mouse wheel when the control has focus.
        ''' </value>
        ''' <remarks>Setting this problem to <c>true</c> could cause conflicting behavior with other controls that also make use of the mouse wheel.</remarks>
        <Category("Behavior")>
        <DefaultValue(False)>
        Friend Overridable Property AllowUnfocusedMouseWheel As Boolean
            Get
                Return _allowUnfocusedMouseWheel
            End Get
            Set(value As Boolean)
                If AllowUnfocusedMouseWheel <> value Then
                    _allowUnfocusedMouseWheel = value

                    OnAllowUnfocusedMouseWheelChanged(EventArgs.Empty)
                End If
            End Set
        End Property

        ''' <summary>
        '''   Gets or sets a value indicating whether the user can change the zoom level.
        ''' </summary>
        ''' <value>
        '''   <c>true</c> if the zoom level can be changed; otherwise, <c>false</c>.
        ''' </value>
        <Category("Behavior")>
        <DefaultValue(True)>
        Friend Overridable Property AllowZoom As Boolean
            Get
                Return _allowZoom
            End Get
            Set(value As Boolean)
                If AllowZoom <> value Then
                    _allowZoom = value

                    OnAllowZoomChanged(EventArgs.Empty)
                End If
            End Set
        End Property

        ''' <summary>
        '''   Gets or sets a value indicating whether the image is centered where possible.
        ''' </summary>
        ''' <value>
        '''   <c>true</c> if the image should be centered where possible; otherwise, <c>false</c>.
        ''' </value>
        <DefaultValue(True)>
        <Category("Appearance")>
        Friend Overridable Property AutoCenter As Boolean
            Get
                Return _autoCenter
            End Get
            Set(value As Boolean)
                If _autoCenter <> value Then
                    _autoCenter = value
                    OnAutoCenterChanged(EventArgs.Empty)
                End If
            End Set
        End Property

        ''' <summary>
        '''   Gets or sets if the mouse can be used to pan the control.
        ''' </summary>
        ''' <value>
        '''   <c>true</c> if the control can be auto panned; otherwise, <c>false</c>.
        ''' </value>
        ''' <remarks>If this property is set, the SizeToFit property cannot be used.</remarks>
        <DefaultValue(True)>
        <Category("Behavior")>
        <Obsolete("Use the PanMode property instead", False)>
        Friend Overridable Property AutoPan As Boolean
            Get
                Return (_panMode And ImageBoxPanMode.Left) <> 0
            End Get
            Set(value As Boolean)
                If AutoPan <> value Then
                    PanMode = If(value, _panMode And ImageBoxPanMode.Left, _panMode And Not ImageBoxPanMode.Left)

                    OnAutoPanChanged(EventArgs.Empty)
                End If
            End Set
        End Property

        ''' <summary>
        '''   Gets or sets a value indicating whether the container enables the user to scroll to any content placed outside of its visible boundaries.
        ''' </summary>
        ''' <value>
        '''   <c>true</c> if the container enables auto-scrolling; otherwise, <c>false</c>.
        ''' </value>
        <DefaultValue(True)>
        Friend Overrides Property AutoScroll As Boolean
            Get
                Return MyBase.AutoScroll
            End Get
            Set(value As Boolean)
                MyBase.AutoScroll = value
            End Set
        End Property

        ''' <summary>
        '''   Gets or sets the minimum System.Drawing.Size of the auto-scroll.
        ''' </summary>
        ''' <value></value>
        ''' <returns>
        '''   A <see cref="T:System.Drawing.Size"/> that determines the minimum System.Drawing.Size of the virtual area through which the user can scroll.
        ''' </returns>
        <Browsable(False)>
        <EditorBrowsable(EditorBrowsableState.Never)>
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Overloads Property AutoScrollMinSize As System.Drawing.Size
            Get
                Return MyBase.AutoScrollMinSize
            End Get
            Set(value As System.Drawing.Size)
                MyBase.AutoScrollMinSize = value
            End Set
        End Property

        ''' <summary>
        '''   Specifies if the control should auto System.Drawing.Size to fit the image contents.
        ''' </summary>
        ''' <value></value>
        ''' <returns>
        '''   <c>true</c> if enabled; otherwise, <c>false</c>
        ''' </returns>
        <Browsable(True)>
        <EditorBrowsable(EditorBrowsableState.Always)>
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)>
        <DefaultValue(False)>
        Public Overrides Property AutoSize As Boolean
            Get
                Return MyBase.AutoSize
            End Get
            Set(value As Boolean)
                If MyBase.AutoSize <> value Then
                    MyBase.AutoSize = value
                    AdjustLayout()
                End If
            End Set
        End Property

        ''' <summary>
        '''   Gets or sets the background color for the control.
        ''' </summary>
        ''' <value></value>
        ''' <returns>
        '''   A <see cref="T:System.Drawing.Color"/> that represents the background color of the control. The default is the value of the
        '''   <see cref="P:System.Windows.Forms.Control.DefaultBackColor"/>
        '''   property.
        ''' </returns>
        <DefaultValue(GetType(Color), "White")>
        Public Overrides Property BackColor As Color
            Get
                Return MyBase.BackColor
            End Get
            Set(value As Color)
                MyBase.BackColor = value
            End Set
        End Property

        ''' <summary>
        '''   Gets or sets the background image displayed in the control.
        ''' </summary>
        ''' <value></value>
        ''' <returns>
        '''   An <see cref="T:System.Drawing.Image"/> that represents the image to display in the background of the control.
        ''' </returns>
        <Browsable(False)>
        <EditorBrowsable(EditorBrowsableState.Never)>
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Overrides Property BackgroundImage As Image
            Get
                Return MyBase.BackgroundImage
            End Get
            Set(value As Image)
                MyBase.BackgroundImage = value
            End Set
        End Property

        ''' <summary>
        '''   Gets or sets the background image layout as defined in the <see cref="T:System.Windows.Forms.ImageLayout"/> enumeration.
        ''' </summary>
        ''' <value>The background image layout.</value>
        ''' <returns>
        '''   One of the values of <see cref="T:System.Windows.Forms.ImageLayout"/> (
        '''   <see cref="F:System.Windows.Forms.ImageLayout.Center"/>
        '''   , <see cref="F:System.Windows.Forms.ImageLayout.None"/>,
        '''   <see cref="F:System.Windows.Forms.ImageLayout.Stretch"/>
        '''   , <see cref="F:System.Windows.Forms.ImageLayout.Tile"/>, or
        '''   <see cref="F:System.Windows.Forms.ImageLayout.Zoom"/>
        '''   ). <see cref="F:System.Windows.Forms.ImageLayout.Tile"/> is the default value.
        ''' </returns>
        <Browsable(False)>
        <EditorBrowsable(EditorBrowsableState.Never)>
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Overrides Property BackgroundImageLayout As ImageLayout
            Get
                Return MyBase.BackgroundImageLayout
            End Get
            Set(value As ImageLayout)
                MyBase.BackgroundImageLayout = value
            End Set
        End Property

        ''' <summary>
        ''' Gets the System.Drawing.Point at the center of the currently displayed image viewport.
        ''' </summary>
        ''' <value>The System.Drawing.Point at the center of the current image viewport.</value>
        <Browsable(False)>
        Friend ReadOnly Property CenterPoint As System.Drawing.Point
            Get
                Dim viewport As Rectangle

                viewport = GetImageViewPort()

                Return New System.Drawing.Point(CInt(viewport.Width / 2), CInt(viewport.Height / 2))
            End Get
        End Property

        ''' <summary>
        ''' Gets or sets the cursor that is displayed when the mouse pointer is over the control.
        ''' </summary>
        ''' <value>
        ''' A <see cref="T:System.Windows.Forms.Cursor"/> that represents the cursor to display when the
        ''' mouse pointer is over the control.
        ''' </value>
        ''' <seealso cref="P:System.Windows.Forms.Control.Cursor"/>
        <Browsable(False)>
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Overrides Property Cursor As Cursor
            Get
                Return MyBase.Cursor
            End Get
            Set(value As Cursor)
                MyBase.Cursor = value
            End Set
        End Property

        ''' <summary>
        '''   Gets or sets the System.Drawing.Size of the drop shadow.
        ''' </summary>
        ''' <value>The System.Drawing.Size of the drop shadow.</value>
        <Category("Appearance")>
        <DefaultValue(3)>
        Friend Overridable Property DropShadowSize As Integer
            Get
                Return _dropShadowSize
            End Get
            Set(value As Integer)
                If DropShadowSize <> value Then
                    _dropShadowSize = value

                    OnDropShadowSizeChanged(EventArgs.Empty)
                End If
            End Set
        End Property

        ''' <summary>
        '''   Gets or sets the System.Drawing.Size of the grid cells.
        ''' </summary>
        ''' <value>The System.Drawing.Size of the grid cells.</value>
        <Category("Appearance")>
        <DefaultValue(8)>
        Friend Overridable Property GridCellSize As Integer
            Get
                Return _gridCellSize
            End Get
            Set(value As Integer)
                If _gridCellSize <> value Then
                    _gridCellSize = value
                    OnGridCellSizeChanged(EventArgs.Empty)
                End If
            End Set
        End Property

        ''' <summary>
        '''   Gets or sets the color of primary cells in the grid.
        ''' </summary>
        ''' <value>The color of primary cells in the grid.</value>
        <Category("Appearance")>
        <DefaultValue(GetType(Color), "Gainsboro")>
        Friend Overridable Property GridColor As Color
            Get
                Return _gridColor
            End Get
            Set(value As Color)
                If _gridColor <> value Then
                    _gridColor = value
                    OnGridColorChanged(EventArgs.Empty)
                End If
            End Set
        End Property

        ''' <summary>
        '''   Gets or sets the color of alternate cells in the grid.
        ''' </summary>
        ''' <value>The color of alternate cells in the grid.</value>
        <Category("Appearance")>
        <DefaultValue(GetType(Color), "White")>
        Friend Overridable Property GridColorAlternate As Color
            Get
                Return _gridColorAlternate
            End Get
            Set(value As Color)
                If _gridColorAlternate <> value Then
                    _gridColorAlternate = value
                    OnGridColorAlternateChanged(EventArgs.Empty)
                End If
            End Set
        End Property

        ''' <summary>
        '''   Gets or sets the grid display mode.
        ''' </summary>
        ''' <value>The grid display mode.</value>
        <DefaultValue(ImageBoxGridDisplayMode.Client)>
        <Category("Appearance")>
        Friend Overridable Property GridDisplayMode As ImageBoxGridDisplayMode
            Get
                Return _gridDisplayMode
            End Get
            Set(value As ImageBoxGridDisplayMode)
                If _gridDisplayMode <> value Then
                    _gridDisplayMode = value
                    OnGridDisplayModeChanged(EventArgs.Empty)
                End If
            End Set
        End Property

        ''' <summary>
        '''   Gets or sets the grid scale.
        ''' </summary>
        ''' <value>The grid scale.</value>
        <DefaultValue(GetType(ImageBoxGridScale), "Small")>
        <Category("Appearance")>
        Friend Overridable Property GridScale As ImageBoxGridScale
            Get
                Return _gridScale
            End Get
            Set(value As ImageBoxGridScale)
                If _gridScale <> value Then
                    _gridScale = value
                    OnGridScaleChanged(EventArgs.Empty)
                End If
            End Set
        End Property

        ''' <summary>
        '''   Gets or sets the image.
        ''' </summary>
        ''' <value>The image.</value>
        <Category("Appearance")>
        <DefaultValueAttribute(GetType(Image), Nothing)>
        Friend Overridable Property Image As Image
            Get
                Return _image
            End Get
            Set(value As Image)
                If _image IsNot value Then
                    ' disable animations
                    If IsAnimating Then
                        Call ImageAnimator.StopAnimate(Image, New EventHandler(AddressOf OnFrameChangedHandler))
                    End If

                    _image = value
                    OnImageChanged(EventArgs.Empty)
                End If
            End Set
        End Property

        ''' <summary>
        '''   Gets or sets the color of the image border.
        ''' </summary>
        ''' <value>The color of the image border.</value>
        <Category("Appearance")>
        <DefaultValue(GetType(Color), "ControlDark")>
        Friend Overridable Property ImageBorderColor As Color
            Get
                Return _imageBorderColor
            End Get
            Set(value As Color)
                If ImageBorderColor <> value Then
                    _imageBorderColor = value

                    OnImageBorderColorChanged(EventArgs.Empty)
                End If
            End Set
        End Property

        ''' <summary>
        '''   Gets or sets the image border style.
        ''' </summary>
        ''' <value>The image border style.</value>
        <Category("Appearance")>
        <DefaultValue(GetType(ImageBoxBorderStyle), "None")>
        Friend Overridable Property ImageBorderStyle As ImageBoxBorderStyle
            Get
                Return _imageBorderStyle
            End Get
            Set(value As ImageBoxBorderStyle)
                If ImageBorderStyle <> value Then
                    _imageBorderStyle = value

                    OnImageBorderStyleChanged(EventArgs.Empty)
                End If
            End Set
        End Property

        ''' <summary>
        '''   Gets or sets the interpolation mode.
        ''' </summary>
        ''' <value>The interpolation mode.</value>
        <Category("Appearance")>
        <DefaultValue(InterpolationMode.NearestNeighbor)>
        Friend Overridable Property InterpolationMode As InterpolationMode
            Get
                Return _interpolationMode
            End Get
            Set(value As InterpolationMode)
                If value = InterpolationMode.Invalid Then
                    value = InterpolationMode.Default
                End If

                If _interpolationMode <> value Then
                    _interpolationMode = value
                    OnInterpolationModeChanged(EventArgs.Empty)
                End If
            End Set
        End Property

        ''' <summary>
        '''   Gets or sets a value indicating whether the mouse should be inverted when panning the control.
        ''' </summary>
        ''' <value>
        '''   <c>true</c> if the mouse should be inverted when panning the control; otherwise, <c>false</c>.
        ''' </value>
        <DefaultValue(False)>
        <Category("Behavior")>
        Friend Overridable Property InvertMouse As Boolean
            Get
                Return _invertMouse
            End Get
            Set(value As Boolean)
                If _invertMouse <> value Then
                    _invertMouse = value
                    OnInvertMouseChanged(EventArgs.Empty)
                End If
            End Set
        End Property

        ''' <summary>
        ''' Gets a value indicating whether the image is currently being displayed at 100% zoom
        ''' </summary>
        ''' <value><c>true</c> if the image is currently being displayed at 100% zoom; otherwise, <c>false</c>.</value>
        <Browsable(False)>
        Friend Overridable ReadOnly Property IsActualSize As Boolean
            Get
                Return Zoom = 100
            End Get
        End Property

        ''' <summary>
        '''   Gets a value indicating whether this control is panning.
        ''' </summary>
        ''' <value>
        '''   <c>true</c> if this control is panning; otherwise, <c>false</c>.
        ''' </value>
        <DefaultValue(False)>
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        <Browsable(False)>
        Public Overridable Property IsPanning As Boolean
            Get
                Return _panStyle <> ImageBoxPanStyle.None
            End Get
            Protected Set(value As Boolean)
                ProcessPanEvents(If(value, ImageBoxPanStyle.Standard, ImageBoxPanStyle.None))
            End Set
        End Property

        ''' <summary>
        '''   Gets or sets a value indicating whether this a selection region is currently being drawn.
        ''' </summary>
        ''' <value>
        '''   <c>true</c> if a selection region is currently being drawn; otherwise, <c>false</c>.
        ''' </value>
        <Browsable(False)>
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Overridable Property IsSelecting As Boolean
            Get
                Return _isSelecting
            End Get
            Protected Set(value As Boolean)
                _isSelecting = value
            End Set
        End Property

        ''' <summary>
        '''   Gets or sets a value indicating whether selection regions should be limited to the image boundaries.
        ''' </summary>
        ''' <value>
        '''   <c>true</c> if selection regions should be limited to image boundaries; otherwise, <c>false</c>.
        ''' </value>
        <Category("Behavior")>
        <DefaultValue(True)>
        Friend Overridable Property LimitSelectionToImage As Boolean
            Get
                Return _limitSelectionToImage
            End Get
            Set(value As Boolean)
                If LimitSelectionToImage <> value Then
                    _limitSelectionToImage = value

                    OnLimitSelectionToImageChanged(EventArgs.Empty)
                End If
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the how the mouse wheel is handled
        ''' </summary>
        ''' <value>
        ''' The mouse wheel mode.
        ''' </value>
        <Category("Behavior")>
        <DefaultValue(GetType(ImageBoxMouseWheelMode), "Zoom")>
        Friend Overridable Property MouseWheelMode As ImageBoxMouseWheelMode
            Get
                Return _mouseWheelMode
            End Get
            Set(value As ImageBoxMouseWheelMode)
                If _mouseWheelMode <> value Then
                    _mouseWheelMode = value

                    OnMouseWheelModeChanged(EventArgs.Empty)
                End If
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the how panning is initiated using the mouse
        ''' </summary>
        ''' <value>
        ''' The pan mode.
        ''' </value>
        <Category("Behavior")>
        <DefaultValue(GetType(ImageBoxPanMode), "Both")>
        Friend Overridable Property PanMode As ImageBoxPanMode
            Get
                Return _panMode
            End Get
            Set(value As ImageBoxPanMode)
                If _panMode <> value Then
                    _panMode = value

                    OnPanModeChanged(EventArgs.Empty)
                End If
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the color of the pixel grid.
        ''' </summary>
        ''' <value>The color of the pixel grid.</value>
        <Category("Appearance")>
        <DefaultValue(GetType(Color), "DimGray")>
        Friend Overridable Property PixelGridColor As Color
            Get
                Return _pixelGridColor
            End Get
            Set(value As Color)
                If PixelGridColor <> value Then
                    _pixelGridColor = value

                    OnPixelGridColorChanged(EventArgs.Empty)
                End If
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the minimum System.Drawing.Size of zoomed pixel's before the pixel grid will be drawn
        ''' </summary>
        ''' <value>The pixel grid threshold.</value>
        <Category("Behavior")>
        <DefaultValue(5)>
        Friend Overridable Property PixelGridThreshold As Integer
            Get
                Return _pixelGridThreshold
            End Get
            Set(value As Integer)
                If PixelGridThreshold <> value Then
                    _pixelGridThreshold = value

                    OnPixelGridThresholdChanged(EventArgs.Empty)
                End If
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating whether the font System.Drawing.Size of text is scaled according to the current zoom level.
        ''' </summary>
        ''' <value><c>true</c> if the System.Drawing.Size of text is scaled according to the current zoom level; otherwise, <c>false</c>.</value>
        <Category("Appearance")>
        <DefaultValue(False)>
        Friend Overridable Property ScaleText As Boolean
            Get
                Return _scaleText
            End Get
            Set(value As Boolean)
                If ScaleText <> value Then
                    _scaleText = value

                    OnScaleTextChanged(EventArgs.Empty)
                End If
            End Set
        End Property

        ''' <summary>
        '''   Gets or sets the color of selection regions.
        ''' </summary>
        ''' <value>
        '''   The color of selection regions.
        ''' </value>
        <Category("Appearance")>
        <DefaultValue(GetType(Color), "Highlight")>
        Friend Overridable Property SelectionColor As Color
            Get
                Return _selectionColor
            End Get
            Set(value As Color)
                If SelectionColor <> value Then
                    _selectionColor = value

                    OnSelectionColorChanged(EventArgs.Empty)
                End If
            End Set
        End Property

        ''' <summary>
        '''   Gets or sets the selection mode.
        ''' </summary>
        ''' <value>
        '''   The selection mode.
        ''' </value>
        <Category("Behavior")>
        <DefaultValue(GetType(ImageBoxSelectionMode), "None")>
        Friend Overridable Property SelectionMode As ImageBoxSelectionMode
            Get
                Return _selectionMode
            End Get
            Set(value As ImageBoxSelectionMode)
                If SelectionMode <> value Then
                    _selectionMode = value

                    OnSelectionModeChanged(EventArgs.Empty)
                End If
            End Set
        End Property

        ''' <summary>
        '''   Gets or sets the selection region.
        ''' </summary>
        ''' <value>
        '''   The selection region.
        ''' </value>
        <Browsable(False)>
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Friend Overridable Property SelectionRegion As RectangleF
            Get
                Return _selectionRegion
            End Get
            Set(value As RectangleF)
                If SelectionRegion <> value Then
                    _selectionRegion = value

                    OnSelectionRegionChanged(EventArgs.Empty)
                End If
            End Set
        End Property

        ''' <summary>
        '''   Gets or sets a value indicating whether the defined shortcuts are enabled.
        ''' </summary>
        ''' <value>
        '''   <c>true</c> to enable the shortcuts; otherwise, <c>false</c>.
        ''' </value>
        <Category("Behavior")>
        <DefaultValue(True)>
        Friend Overridable Property ShortcutsEnabled As Boolean
            Get
                Return _shortcutsEnabled
            End Get
            Set(value As Boolean)
                If ShortcutsEnabled <> value Then
                    _shortcutsEnabled = value

                    OnShortcutsEnabledChanged(EventArgs.Empty)
                End If
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating whether a pixel grid is displayed when the control is zoomed.
        ''' </summary>
        ''' <value><c>true</c> if a pixel grid is displayed when the control is zoomed; otherwise, <c>false</c>.</value>
        <Category("Appearance")>
        <DefaultValue(False)>
        Friend Overridable Property ShowPixelGrid As Boolean
            Get
                Return _showPixelGrid
            End Get
            Set(value As Boolean)
                If ShowPixelGrid <> value Then
                    _showPixelGrid = value

                    OnShowPixelGridChanged(EventArgs.Empty)
                End If
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the System.Drawing.Size mode of images hosted in the control.
        ''' </summary>
        ''' <value>The System.Drawing.Size mode.</value>
        <Category("Behavior")>
        <DefaultValue(GetType(ImageBoxSizeMode), "Normal")>
        Friend Overridable Property SizeMode As ImageBoxSizeMode
            Get
                Return _sizeMode
            End Get
            Set(value As ImageBoxSizeMode)
                If SizeMode <> value Then
                    _sizeMode = value

                    OnSizeModeChanged(EventArgs.Empty)
                End If
            End Set
        End Property

        ''' <summary>
        '''   Gets or sets a value indicating whether the control should automatically System.Drawing.Size to fit the image contents.
        ''' </summary>
        ''' <value>
        '''   <c>true</c> if the control should System.Drawing.Size to fit the image contents; otherwise, <c>false</c>.
        ''' </value>
        <DefaultValue(False)>
        <Category("Appearance")>
        <Browsable(False)>
        <EditorBrowsable(EditorBrowsableState.Never)>
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        <Obsolete("This property is deprecated and will be removed in a future version of the component. Implementors should use the SizeMode property instead.")>
        Friend Overridable Property SizeToFit As Boolean
            Get
                Return SizeMode = ImageBoxSizeMode.Fit
            End Get
            Set(value As Boolean)
                If SizeMode = ImageBoxSizeMode.Fit <> value Then
                    SizeMode = If(value, ImageBoxSizeMode.Fit, ImageBoxSizeMode.Normal)
                    OnSizeToFitChanged(EventArgs.Empty)

                    If value Then
                        AutoPan = False
                    Else
                        ActualSize()
                    End If
                End If
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the alignment of the text on the control.
        ''' </summary>
        ''' <value>One of the <see cref="ContentAlignment"/> values. The default is <c>MiddleCenter</c>.</value>
        <Category("Appearance")>
        <DefaultValue(GetType(ContentAlignment), "MiddleCenter")>
        Friend Overridable Property TextAlign As ContentAlignment
            Get
                Return _textAlign
            End Get
            Set(value As ContentAlignment)
                If TextAlign <> value Then
                    _textAlign = value

                    OnTextAlignChanged(EventArgs.Empty)
                End If
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the color of the text background.
        ''' </summary>
        ''' <value>The color of the text background.</value>
        <Category("Appearance")>
        <DefaultValue(GetType(Color), "Transparent")>
        Friend Overridable Property TextBackColor As Color
            Get
                Return _textBackColor
            End Get
            Set(value As Color)
                If TextBackColor <> value Then
                    _textBackColor = value

                    OnTextBackColorChanged(EventArgs.Empty)
                End If
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the text display mode.
        ''' </summary>
        ''' <value>The text display mode.</value>
        <Category("Appearance")>
        <DefaultValue(GetType(ImageBoxGridDisplayMode), "Client")>
        Friend Overridable Property TextDisplayMode As ImageBoxGridDisplayMode
            Get
                Return _textDisplayMode
            End Get
            Set(value As ImageBoxGridDisplayMode)
                If TextDisplayMode <> value Then
                    _textDisplayMode = value

                    OnTextDisplayModeChanged(EventArgs.Empty)
                End If
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the text padding.
        ''' </summary>
        ''' <value>
        ''' The text padding.
        ''' </value>
        <Category("Appearance")>
        <DefaultValue(GetType(Padding), "0, 0, 0, 0")>
        Friend Overridable Property TextPadding As Padding
            Get
                Return _textPadding
            End Get
            Set(value As Padding)
                If TextPadding <> value Then
                    _textPadding = value

                    OnTextPaddingChanged(EventArgs.Empty)
                End If
            End Set
        End Property

        ''' <summary>
        '''   Gets or sets a value indicating whether the control acts as a virtual image box.
        ''' </summary>
        ''' <value>
        '''   <c>true</c> if the control acts as a virtual image box; otherwise, <c>false</c>.
        ''' </value>
        ''' <remarks>
        '''   When this property is set to <c>true</c>, the Image property is ignored in favor of the VirtualSize property. In addition, the VirtualDraw event is raised to allow custom painting of the image area.
        ''' </remarks>
        <Category("Behavior")>
        <DefaultValue(False)>
        Friend Overridable Property VirtualMode As Boolean
            Get
                Return _virtualMode
            End Get
            Set(value As Boolean)
                If VirtualMode <> value Then
                    _virtualMode = value

                    OnVirtualModeChanged(EventArgs.Empty)
                End If
            End Set
        End Property

        ''' <summary>
        '''   Gets or sets the System.Drawing.Size of the virtual image.
        ''' </summary>
        ''' <value>The System.Drawing.Size of the virtual image.</value>
        <Category("Appearance")>
        <DefaultValue(GetType(System.Drawing.Size), "0, 0")>
        Friend Overridable Property VirtualSize As System.Drawing.Size
            Get
                Return _virtualSize
            End Get
            Set(value As System.Drawing.Size)
                If VirtualSize <> value Then
                    _virtualSize = value

                    OnVirtualSizeChanged(EventArgs.Empty)
                End If
            End Set
        End Property

        ''' <summary>
        '''   Gets or sets the zoom.
        ''' </summary>
        ''' <value>The zoom.</value>
        <DefaultValue(100)>
        <Category("Appearance")>
        Friend Overridable Property Zoom As Integer
            Get
                Return _zoom
            End Get
            Set(value As Integer)
                SetZoom(value, If(value > Zoom, ImageBoxZoomActions.ZoomIn, ImageBoxZoomActions.ZoomOut), ImageBoxActionSources.Unknown)
            End Set
        End Property

        ''' <summary>
        '''   Gets the zoom factor.
        ''' </summary>
        ''' <value>The zoom factor.</value>
        <Browsable(False)>
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Friend Overridable ReadOnly Property ZoomFactor As Double
            Get
                Return Zoom / 100
            End Get
        End Property

        ''' <summary>
        '''   Gets or sets the zoom levels.
        ''' </summary>
        ''' <value>The zoom levels.</value>
        <Browsable(False)>
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Friend Overridable Property ZoomLevels As ZoomLevelCollection
            Get
                Return _zoomLevels
            End Get
            Set(value As ZoomLevelCollection)
                If ZoomLevels IsNot value Then
                    _zoomLevels = value

                    OnZoomLevelsChanged(EventArgs.Empty)
                End If
            End Set
        End Property

        ''' <summary>
        '''   Gets a value indicating whether painting of the control is allowed.
        ''' </summary>
        ''' <value>
        '''   <c>true</c> if painting of the control is allowed; otherwise, <c>false</c>.
        ''' </value>
        Protected Overridable ReadOnly Property AllowPainting As Boolean
            Get
                Return _updateCount = 0
            End Get
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating whether the current image is animated.
        ''' </summary>
        ''' <value><c>true</c> if the current image is animated; otherwise, <c>false</c>.</value>
        Protected Property IsAnimating As Boolean

        ''' <summary>
        '''   Gets the height of the scaled image.
        ''' </summary>
        ''' <value>The height of the scaled image.</value>
        Protected Overridable ReadOnly Property ScaledImageHeight As Integer
            Get
                Return Convert.ToInt32(ViewSize.Height * ZoomFactor)
            End Get
        End Property

        ''' <summary>
        '''   Gets the width of the scaled image.
        ''' </summary>
        ''' <value>The width of the scaled image.</value>
        Protected Overridable ReadOnly Property ScaledImageWidth As Integer
            Get
                Return Convert.ToInt32(ViewSize.Width * ZoomFactor)
            End Get
        End Property

        ''' <summary>
        ''' Gets the System.Drawing.Size of the view.
        ''' </summary>
        ''' <value>The System.Drawing.Size of the view.</value>
        Protected Overridable ReadOnly Property ViewSize As System.Drawing.Size
            Get
                Return If(VirtualMode, VirtualSize, GetImageSize())
            End Get
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating whether a drag operation was cancelled.
        ''' </summary>
        ''' <value><c>true</c> if the drag operation was cancelled; otherwise, <c>false</c>.</value>
        Protected Property WasDragCancelled As Boolean

#End Region

#Region "Methods"

        ''' <summary>
        '''   Resets the zoom to 100%.
        ''' </summary>
        Friend Overridable Sub ActualSize()
            PerformActualSize(ImageBoxActionSources.Unknown)
        End Sub

        ''' <summary>
        '''   Disables any redrawing of the image box
        ''' </summary>
        Friend Overridable Sub BeginUpdate()
            _updateCount += 1
        End Sub

        ''' <summary>
        '''   Centers the given System.Drawing.Point in the image in the center of the control
        ''' </summary>
        ''' <param name="imageLocation">The System.Drawing.Point of the image to attempt to center.</param>
        Friend Overridable Sub CenterAt(imageLocation As System.Drawing.Point)
            ScrollTo(imageLocation, New System.Drawing.Point(CInt(ClientSize.Width / 2), CInt(ClientSize.Height / 2)))
        End Sub

        ''' <summary>
        '''   Centers the given System.Drawing.Point in the image in the center of the control
        ''' </summary>
        ''' <param name="x">The X co-ordinate of the System.Drawing.Point to center.</param>
        ''' <param name="y">The Y co-ordinate of the System.Drawing.Point to center.</param>
        Friend Sub CenterAt(x As Integer, y As Integer)
            CenterAt(New System.Drawing.Point(x, y))
        End Sub

        ''' <summary>
        '''   Centers the given System.Drawing.Point in the image in the center of the control
        ''' </summary>
        ''' <param name="x">The X co-ordinate of the System.Drawing.Point to center.</param>
        ''' <param name="y">The Y co-ordinate of the System.Drawing.Point to center.</param>
        Friend Sub CenterAt(x As Single, y As Single)
            CenterAt(New System.Drawing.Point(CInt(x), CInt(y)))
        End Sub

        ''' <summary>
        ''' Resets the viewport to show the center of the image.
        ''' </summary>
        Friend Overridable Sub CenterToImage()
            AutoScrollPosition = New System.Drawing.Point(CInt((AutoScrollMinSize.Width - ClientSize.Width) / 2), CInt((AutoScrollMinSize.Height - ClientSize.Height) / 2))
        End Sub

        ''' <summary>
        '''   Enables the redrawing of the image box
        ''' </summary>
        Friend Overridable Sub EndUpdate()
            If _updateCount > 0 Then
                _updateCount -= 1
            End If

            If AllowPainting Then
                Invalidate()
            End If
        End Sub

        ''' <summary>
        '''   Fits a given <see cref="T:System.Drawing.Rectangle"/> to match image boundaries
        ''' </summary>
        ''' <param name="rectangle">The rectangle.</param>
        ''' <returns>
        '''   A <see cref="T:System.Drawing.Rectangle"/> structure remapped to fit the image boundaries
        ''' </returns>
        Friend Function FitRectangle(rectangle As Rectangle) As Rectangle
            Dim x As Integer
            Dim y As Integer
            Dim w As Integer
            Dim h As Integer

            x = rectangle.X
            y = rectangle.Y
            w = rectangle.Width
            h = rectangle.Height

            If x < 0 Then
                x = 0
            End If

            If y < 0 Then
                y = 0
            End If

            If x + w > ViewSize.Width Then
                w = ViewSize.Width - x
            End If

            If y + h > ViewSize.Height Then
                h = ViewSize.Height - y
            End If

            Return New Rectangle(x, y, w, h)
        End Function

        ''' <summary>
        '''   Fits a given <see cref="T:System.Drawing.RectangleF"/> to match image boundaries
        ''' </summary>
        ''' <param name="rectangle">The rectangle.</param>
        ''' <returns>
        '''   A <see cref="T:System.Drawing.RectangleF"/> structure remapped to fit the image boundaries
        ''' </returns>
        Friend Function FitRectangle(rectangle As RectangleF) As RectangleF
            Dim x As Single
            Dim y As Single
            Dim w As Single
            Dim h As Single

            x = rectangle.X
            y = rectangle.Y
            w = rectangle.Width
            h = rectangle.Height

            If x < 0 Then
                w -= -x
                x = 0
            End If

            If y < 0 Then
                h -= -y
                y = 0
            End If

            If x + w > ViewSize.Width Then
                w = ViewSize.Width - x
            End If

            If y + h > ViewSize.Height Then
                h = ViewSize.Height - y
            End If

            Return New RectangleF(x, y, w, h)
        End Function

        ''' <summary>
        '''   Gets the image view port.
        ''' </summary>
        ''' <returns></returns>
        Friend Overridable Function GetImageViewPort() As Rectangle
            Dim viewPort As Rectangle

            If Not ViewSize.IsEmpty Then
                Dim innerRectangle As Rectangle
                Dim offset As System.Drawing.Point
                Dim width As Integer
                Dim height As Integer

                innerRectangle = GetInsideViewPort(True)

                If Not HScroll AndAlso Not VScroll Then ' if no scrolling is present, tinker the view port so that the image and any applicable borders all fit inside
                    innerRectangle.Inflate(-GetImageBorderOffset(), -GetImageBorderOffset())
                End If

                If SizeMode <> ImageBoxSizeMode.Stretch Then
                    If AutoCenter Then
                        Dim x As Integer
                        Dim y As Integer

                        x = CInt(If(Not HScroll, (innerRectangle.Width - (ScaledImageWidth + Padding.Horizontal)) / 2, 0))
                        y = CInt(If(Not VScroll, (innerRectangle.Height - (ScaledImageHeight + Padding.Vertical)) / 2, 0))

                        offset = New System.Drawing.Point(x, y)
                    Else
                        offset = System.Drawing.Point.Empty
                    End If

                    width = Math.Min(ScaledImageWidth - Math.Abs(AutoScrollPosition.X), innerRectangle.Width)
                    height = Math.Min(ScaledImageHeight - Math.Abs(AutoScrollPosition.Y), innerRectangle.Height)
                Else
                    offset = System.Drawing.Point.Empty
                    width = innerRectangle.Width
                    height = innerRectangle.Height
                End If

                viewPort = New Rectangle(offset.X + innerRectangle.Left, offset.Y + innerRectangle.Top, width, height)
            Else
                viewPort = Rectangle.Empty
            End If

            Return viewPort
        End Function

        ''' <summary>
        '''   Gets the inside view port, excluding any padding.
        ''' </summary>
        ''' <returns></returns>
        Friend Function GetInsideViewPort() As Rectangle
            Return GetInsideViewPort(False)
        End Function

        ''' <summary>
        '''   Gets the inside view port.
        ''' </summary>
        ''' <param name="includePadding">
        '''   if set to <c>true</c> [include padding].
        ''' </param>
        ''' <returns></returns>
        Friend Overridable Function GetInsideViewPort(includePadding As Boolean) As Rectangle
            Dim left As Integer
            Dim top As Integer
            Dim width As Integer
            Dim height As Integer

            left = 0
            top = 0
            width = ClientSize.Width
            height = ClientSize.Height

            If includePadding Then
                left += Padding.Left
                top += Padding.Top
                width -= Padding.Horizontal
                height -= Padding.Vertical
            End If

            Return New Rectangle(left, top, width, height)
        End Function

        ''' <summary>
        '''   Returns the source <see cref="T:System.Drawing.System.Drawing.Point"/> repositioned to include the current image offset and scaled by the current zoom level
        ''' </summary>
        ''' <param name="source">The source <see cref="System.Drawing.Point"/> to offset.</param>
        ''' <returns>A <see cref="System.Drawing.Point"/> which has been repositioned to match the current zoom level and image offset</returns>
        Friend Overridable Function GetOffsetPoint(source As System.Drawing.Point) As System.Drawing.Point
            Dim offset As PointF

            offset = GetOffsetPoint(New PointF(source.X, source.Y))

            Return New System.Drawing.Point(CInt(offset.X), CInt(offset.Y))
        End Function

        ''' <summary>
        '''   Returns the source co-ordinates repositioned to include the current image offset and scaled by the current zoom level
        ''' </summary>
        ''' <param name="x">The source X co-ordinate.</param>
        ''' <param name="y">The source Y co-ordinate.</param>
        ''' <returns>A <see cref="System.Drawing.Point"/> which has been repositioned to match the current zoom level and image offset</returns>
        Friend Function GetOffsetPoint(x As Integer, y As Integer) As System.Drawing.Point
            Return GetOffsetPoint(New System.Drawing.Point(x, y))
        End Function

        ''' <summary>
        '''   Returns the source co-ordinates repositioned to include the current image offset and scaled by the current zoom level
        ''' </summary>
        ''' <param name="x">The source X co-ordinate.</param>
        ''' <param name="y">The source Y co-ordinate.</param>
        ''' <returns>A <see cref="System.Drawing.Point"/> which has been repositioned to match the current zoom level and image offset</returns>
        Friend Function GetOffsetPoint(x As Single, y As Single) As PointF
            Return GetOffsetPoint(New PointF(x, y))
        End Function

        ''' <summary>
        '''   Returns the source <see cref="T:System.Drawing.PointF"/> repositioned to include the current image offset and scaled by the current zoom level
        ''' </summary>
        ''' <param name="source">The source <see cref="PointF"/> to offset.</param>
        ''' <returns>A <see cref="PointF"/> which has been repositioned to match the current zoom level and image offset</returns>
        Friend Overridable Function GetOffsetPoint(source As PointF) As PointF
            Dim viewport As Rectangle
            Dim scaled As PointF
            Dim offsetX As Integer
            Dim offsetY As Integer

            viewport = GetImageViewPort()
            scaled = GetScaledPoint(source)
            offsetX = viewport.Left + Padding.Left + AutoScrollPosition.X
            offsetY = viewport.Top + Padding.Top + AutoScrollPosition.Y

            Return New PointF(scaled.X + offsetX, scaled.Y + offsetY)
        End Function

        ''' <summary>
        '''   Returns the source <see cref="T:System.Drawing.RectangleF"/> scaled according to the current zoom level and repositioned to include the current image offset
        ''' </summary>
        ''' <param name="source">The source <see cref="RectangleF"/> to offset.</param>
        ''' <returns>A <see cref="RectangleF"/> which has been resized and repositioned to match the current zoom level and image offset</returns>
        Friend Overridable Function GetOffsetRectangle(source As RectangleF) As RectangleF
            Dim viewport As RectangleF
            Dim scaled As RectangleF
            Dim offsetX As Single
            Dim offsetY As Single

            viewport = GetImageViewPort()
            scaled = GetScaledRectangle(source)
            offsetX = viewport.Left + Padding.Left + AutoScrollPosition.X
            offsetY = viewport.Top + Padding.Top + AutoScrollPosition.Y

            Return New RectangleF(New PointF(scaled.Left + offsetX, scaled.Top + offsetY), scaled.Size)
        End Function

        ''' <summary>
        '''   Returns the source rectangle scaled according to the current zoom level and repositioned to include the current image offset
        ''' </summary>
        ''' <param name="x">The X co-ordinate of the source rectangle.</param>
        ''' <param name="y">The Y co-ordinate of the source rectangle.</param>
        ''' <param name="width">The width of the rectangle.</param>
        ''' <param name="height">The height of the rectangle.</param>
        ''' <returns>A <see cref="Rectangle"/> which has been resized and repositioned to match the current zoom level and image offset</returns>
        Friend Function GetOffsetRectangle(x As Integer, y As Integer, width As Integer, height As Integer) As Rectangle
            Return GetOffsetRectangle(New Rectangle(x, y, width, height))
        End Function

        ''' <summary>
        '''   Returns the source rectangle scaled according to the current zoom level and repositioned to include the current image offset
        ''' </summary>
        ''' <param name="x">The X co-ordinate of the source rectangle.</param>
        ''' <param name="y">The Y co-ordinate of the source rectangle.</param>
        ''' <param name="width">The width of the rectangle.</param>
        ''' <param name="height">The height of the rectangle.</param>
        ''' <returns>A <see cref="RectangleF"/> which has been resized and repositioned to match the current zoom level and image offset</returns>
        Friend Function GetOffsetRectangle(x As Single, y As Single, width As Single, height As Single) As RectangleF
            Return GetOffsetRectangle(New RectangleF(x, y, width, height))
        End Function

        ''' <summary>
        '''   Returns the source <see cref="T:System.Drawing.Rectangle"/> scaled according to the current zoom level and repositioned to include the current image offset
        ''' </summary>
        ''' <param name="source">The source <see cref="Rectangle"/> to offset.</param>
        ''' <returns>A <see cref="Rectangle"/> which has been resized and repositioned to match the current zoom level and image offset</returns>
        Friend Overridable Function GetOffsetRectangle(source As Rectangle) As Rectangle
            Dim viewport As Rectangle
            Dim scaled As Rectangle
            Dim offsetX As Integer
            Dim offsetY As Integer

            viewport = GetImageViewPort()
            scaled = GetScaledRectangle(source)
            offsetX = viewport.Left + Padding.Left + AutoScrollPosition.X
            offsetY = viewport.Top + Padding.Top + AutoScrollPosition.Y

            Return New Rectangle(New System.Drawing.Point(scaled.Left + offsetX, scaled.Top + offsetY), scaled.Size)
        End Function

        ''' <summary>
        '''   Retrieves the System.Drawing.Size of a rectangular area into which a control can be fitted.
        ''' </summary>
        ''' <param name="proposedSize">The custom-sized area for a control.</param>
        ''' <returns>
        '''   An ordered pair of type <see cref="T:System.Drawing.Size"/> representing the width and height of a rectangle.
        ''' </returns>
        Public Overrides Function GetPreferredSize(proposedSize As System.Drawing.Size) As System.Drawing.Size
            Dim size As System.Drawing.Size

            If Not ViewSize.IsEmpty Then
                Dim width As Integer
                Dim height As Integer

                ' get the System.Drawing.Size of the image
                width = ScaledImageWidth
                height = ScaledImageHeight

                ' add an offset based on padding
                width += Padding.Horizontal
                height += Padding.Vertical

                ' add an offset based on the border style
                width += GetImageBorderOffset()
                height += GetImageBorderOffset()

                size = New System.Drawing.Size(width, height)
            Else
                size = MyBase.GetPreferredSize(proposedSize)
            End If

            Return size
        End Function

        ''' <summary>
        '''   Returns the source <see cref="T:System.Drawing.System.Drawing.Point"/> scaled according to the current zoom level
        ''' </summary>
        ''' <param name="x">The X co-ordinate of the System.Drawing.Point to scale.</param>
        ''' <param name="y">The Y co-ordinate of the System.Drawing.Point to scale.</param>
        ''' <returns>A <see cref="System.Drawing.Point"/> which has been scaled to match the current zoom level</returns>
        Friend Function GetScaledPoint(x As Integer, y As Integer) As System.Drawing.Point
            Return GetScaledPoint(New System.Drawing.Point(x, y))
        End Function

        ''' <summary>
        '''   Returns the source <see cref="T:System.Drawing.System.Drawing.Point"/> scaled according to the current zoom level
        ''' </summary>
        ''' <param name="x">The X co-ordinate of the System.Drawing.Point to scale.</param>
        ''' <param name="y">The Y co-ordinate of the System.Drawing.Point to scale.</param>
        ''' <returns>A <see cref="System.Drawing.Point"/> which has been scaled to match the current zoom level</returns>
        Friend Function GetScaledPoint(x As Single, y As Single) As PointF
            Return GetScaledPoint(New PointF(x, y))
        End Function

        ''' <summary>
        '''   Returns the source <see cref="T:System.Drawing.System.Drawing.Point"/> scaled according to the current zoom level
        ''' </summary>
        ''' <param name="source">The source <see cref="System.Drawing.Point"/> to scale.</param>
        ''' <returns>A <see cref="System.Drawing.Point"/> which has been scaled to match the current zoom level</returns>
        Friend Overridable Function GetScaledPoint(source As System.Drawing.Point) As System.Drawing.Point
            Return New System.Drawing.Point(CInt(source.X * ZoomFactor), CInt(source.Y * ZoomFactor))
        End Function

        ''' <summary>
        '''   Returns the source <see cref="T:System.Drawing.PointF"/> scaled according to the current zoom level
        ''' </summary>
        ''' <param name="source">The source <see cref="PointF"/> to scale.</param>
        ''' <returns>A <see cref="PointF"/> which has been scaled to match the current zoom level</returns>
        Friend Overridable Function GetScaledPoint(source As PointF) As PointF
            Return New PointF(CSng(source.X * ZoomFactor), CSng(source.Y * ZoomFactor))
        End Function

        ''' <summary>
        '''   Returns the source rectangle scaled according to the current zoom level
        ''' </summary>
        ''' <param name="x">The X co-ordinate of the source rectangle.</param>
        ''' <param name="y">The Y co-ordinate of the source rectangle.</param>
        ''' <param name="width">The width of the rectangle.</param>
        ''' <param name="height">The height of the rectangle.</param>
        ''' <returns>A <see cref="Rectangle"/> which has been scaled to match the current zoom level</returns>
        Friend Function GetScaledRectangle(x As Integer, y As Integer, width As Integer, height As Integer) As Rectangle
            Return GetScaledRectangle(New Rectangle(x, y, width, height))
        End Function

        ''' <summary>
        '''   Returns the source rectangle scaled according to the current zoom level
        ''' </summary>
        ''' <param name="x">The X co-ordinate of the source rectangle.</param>
        ''' <param name="y">The Y co-ordinate of the source rectangle.</param>
        ''' <param name="width">The width of the rectangle.</param>
        ''' <param name="height">The height of the rectangle.</param>
        ''' <returns>A <see cref="RectangleF"/> which has been scaled to match the current zoom level</returns>
        Friend Function GetScaledRectangle(x As Single, y As Single, width As Single, height As Single) As RectangleF
            Return GetScaledRectangle(New RectangleF(x, y, width, height))
        End Function

        ''' <summary>
        '''   Returns the source rectangle scaled according to the current zoom level
        ''' </summary>
        ''' <param name="location">The location of the source rectangle.</param>
        ''' <param name="size">The System.Drawing.Size of the source rectangle.</param>
        ''' <returns>A <see cref="Rectangle"/> which has been scaled to match the current zoom level</returns>
        Friend Function GetScaledRectangle(location As System.Drawing.Point, size As System.Drawing.Size) As Rectangle
            Return GetScaledRectangle(New Rectangle(location, size))
        End Function

        ''' <summary>
        '''   Returns the source rectangle scaled according to the current zoom level
        ''' </summary>
        ''' <param name="location">The location of the source rectangle.</param>
        ''' <param name="size">The System.Drawing.Size of the source rectangle.</param>
        ''' <returns>A <see cref="Rectangle"/> which has been scaled to match the current zoom level</returns>
        Friend Function GetScaledRectangle(location As PointF, size As SizeF) As RectangleF
            Return GetScaledRectangle(New RectangleF(location, size))
        End Function

        ''' <summary>
        '''   Returns the source <see cref="T:System.Drawing.Rectangle"/> scaled according to the current zoom level
        ''' </summary>
        ''' <param name="source">The source <see cref="Rectangle"/> to scale.</param>
        ''' <returns>A <see cref="Rectangle"/> which has been scaled to match the current zoom level</returns>
        Friend Overridable Function GetScaledRectangle(source As Rectangle) As Rectangle
            Return New Rectangle(CInt(source.Left * ZoomFactor), CInt(source.Top * ZoomFactor), CInt(source.Width * ZoomFactor), CInt(source.Height * ZoomFactor))
        End Function

        ''' <summary>
        '''   Returns the source <see cref="T:System.Drawing.RectangleF"/> scaled according to the current zoom level
        ''' </summary>
        ''' <param name="source">The source <see cref="RectangleF"/> to scale.</param>
        ''' <returns>A <see cref="RectangleF"/> which has been scaled to match the current zoom level</returns>
        Friend Overridable Function GetScaledRectangle(source As RectangleF) As RectangleF
            Return New RectangleF(CSng(source.Left * ZoomFactor), CSng(source.Top * ZoomFactor), CSng(source.Width * ZoomFactor), CSng(source.Height * ZoomFactor))
        End Function

        ''' <summary>
        '''   Returns the source System.Drawing.Size scaled according to the current zoom level
        ''' </summary>
        ''' <param name="width">The width of the System.Drawing.Size to scale.</param>
        ''' <param name="height">The height of the System.Drawing.Size to scale.</param>
        ''' <returns>A <see cref="SizeF"/> which has been resized to match the current zoom level</returns>
        Friend Function GetScaledSize(width As Single, height As Single) As SizeF
            Return GetScaledSize(New SizeF(width, height))
        End Function

        ''' <summary>
        '''   Returns the source System.Drawing.Size scaled according to the current zoom level
        ''' </summary>
        ''' <param name="width">The width of the System.Drawing.Size to scale.</param>
        ''' <param name="height">The height of the System.Drawing.Size to scale.</param>
        ''' <returns>A <see cref="System.Drawing.Size"/> which has been resized to match the current zoom level</returns>
        Friend Function GetScaledSize(width As Integer, height As Integer) As System.Drawing.Size
            Return GetScaledSize(New System.Drawing.Size(width, height))
        End Function

        ''' <summary>
        '''   Returns the source <see cref="T:System.Drawing.SizeF"/> scaled according to the current zoom level
        ''' </summary>
        ''' <param name="source">The source <see cref="SizeF"/> to scale.</param>
        ''' <returns>A <see cref="SizeF"/> which has been resized to match the current zoom level</returns>
        Friend Overridable Function GetScaledSize(source As SizeF) As SizeF
            Return New SizeF(CSng(source.Width * ZoomFactor), CSng(source.Height * ZoomFactor))
        End Function

        ''' <summary>
        '''   Returns the source <see cref="T:System.Drawing.Size"/> scaled according to the current zoom level
        ''' </summary>
        ''' <param name="source">The source <see cref="System.Drawing.Size"/> to scale.</param>
        ''' <returns>A <see cref="System.Drawing.Size"/> which has been resized to match the current zoom level</returns>
        Friend Overridable Function GetScaledSize(source As System.Drawing.Size) As System.Drawing.Size
            Return New System.Drawing.Size(CInt(source.Width * ZoomFactor), CInt(source.Height * ZoomFactor))
        End Function

        ''' <summary>
        '''   Creates an image based on the current selection region
        ''' </summary>
        ''' <returns>An image containing the selection contents if a selection if present, otherwise null</returns>
        ''' <remarks>The caller is responsible for disposing of the returned image</remarks>
        Friend Function GetSelectedImage() As Image
            Dim result As Image

            result = Nothing

            If Not SelectionRegion.IsEmpty Then
                Dim rect As Rectangle

                rect = FitRectangle(New Rectangle(CInt(SelectionRegion.X), CInt(SelectionRegion.Y), CInt(SelectionRegion.Width), CInt(SelectionRegion.Height)))

                If rect.Width > 0 AndAlso rect.Height > 0 Then
                    result = New Bitmap(rect.Width, rect.Height)

                    Using g = Graphics.FromImage(result)
                        g.DrawImage(Image, New Rectangle(System.Drawing.Point.Empty, rect.Size), rect, GraphicsUnit.Pixel)
                    End Using
                End If
            End If

            Return result
        End Function

        ''' <summary>
        '''   Gets the source image region.
        ''' </summary>
        ''' <returns></returns>
        Friend Overridable Function GetSourceImageRegion() As RectangleF
            Dim region As RectangleF

            If Not ViewSize.IsEmpty Then
                If SizeMode <> ImageBoxSizeMode.Stretch Then
                    Dim sourceLeft As Single
                    Dim sourceTop As Single
                    Dim sourceWidth As Single
                    Dim sourceHeight As Single
                    Dim viewPort As Rectangle

                    viewPort = GetImageViewPort()
                    sourceLeft = CSng(-AutoScrollPosition.X / ZoomFactor)
                    sourceTop = CSng(-AutoScrollPosition.Y / ZoomFactor)
                    sourceWidth = CSng(viewPort.Width / ZoomFactor)
                    sourceHeight = CSng(viewPort.Height / ZoomFactor)

                    region = New RectangleF(sourceLeft, sourceTop, sourceWidth, sourceHeight)
                Else
                    region = New RectangleF(PointF.Empty, ViewSize)
                End If
            Else
                region = RectangleF.Empty
            End If

            Return region
        End Function

        ''' <summary>
        '''   Determines whether the specified System.Drawing.Point is located within the image view port
        ''' </summary>
        ''' <param name="Point">The System.Drawing.Point.</param>
        ''' <returns>
        '''   <c>true</c> if the specified System.Drawing.Point is located within the image view port; otherwise, <c>false</c>.
        ''' </returns>
        Friend Overridable Function IsPointInImage(point As System.Drawing.Point) As Boolean
            Return GetImageViewPort().Contains(point)
        End Function

        ''' <summary>
        '''   Determines whether the specified System.Drawing.Point is located within the image view port
        ''' </summary>
        ''' <param name="x">The X co-ordinate of the System.Drawing.Point to check.</param>
        ''' <param name="y">The Y co-ordinate of the System.Drawing.Point to check.</param>
        ''' <returns>
        '''   <c>true</c> if the specified System.Drawing.Point is located within the image view port; otherwise, <c>false</c>.
        ''' </returns>
        Friend Function IsPointInImage(x As Integer, y As Integer) As Boolean
            Return IsPointInImage(New System.Drawing.Point(x, y))
        End Function

        ''' <summary>
        '''   Determines whether the specified System.Drawing.Point is located within the image view port
        ''' </summary>
        ''' <param name="x">The X co-ordinate of the System.Drawing.Point to check.</param>
        ''' <param name="y">The Y co-ordinate of the System.Drawing.Point to check.</param>
        ''' <returns>
        '''   <c>true</c> if the specified System.Drawing.Point is located within the image view port; otherwise, <c>false</c>.
        ''' </returns>
        Friend Function IsPointInImage(x As Single, y As Single) As Boolean
            Return IsPointInImage(New System.Drawing.Point(CInt(x), CInt(y)))
        End Function

        ''' <summary>
        '''   Converts the given client System.Drawing.Size System.Drawing.Point to represent a coordinate on the source image.
        ''' </summary>
        ''' <param name="Point">The source System.Drawing.Point.</param>
        ''' <returns><c>System.Drawing.Point.Empty</c> if the System.Drawing.Point could not be matched to the source image, otherwise the new translated System.Drawing.Point</returns>
        Friend Function PointToImage(point As System.Drawing.Point) As System.Drawing.Point
            Return PointToImage(point, False)
        End Function

        ''' <summary>
        '''   Converts the given client System.Drawing.Size System.Drawing.Point to represent a coordinate on the source image.
        ''' </summary>
        ''' <param name="x">The X co-ordinate of the System.Drawing.Point to convert.</param>
        ''' <param name="y">The Y co-ordinate of the System.Drawing.Point to convert.</param>
        ''' <returns><c>System.Drawing.Point.Empty</c> if the System.Drawing.Point could not be matched to the source image, otherwise the new translated System.Drawing.Point</returns>
        Friend Function PointToImage(x As Single, y As Single) As System.Drawing.Point
            Return PointToImage(x, y, False)
        End Function

        ''' <summary>
        '''   Converts the given client System.Drawing.Size System.Drawing.Point to represent a coordinate on the source image.
        ''' </summary>
        ''' <param name="x">The X co-ordinate of the System.Drawing.Point to convert.</param>
        ''' <param name="y">The Y co-ordinate of the System.Drawing.Point to convert.</param>
        ''' <param name="fitToBounds">
        '''   if set to <c>true</c> and the System.Drawing.Point is outside the bounds of the source image, it will be mapped to the nearest edge.
        ''' </param>
        ''' <returns><c>System.Drawing.Point.Empty</c> if the System.Drawing.Point could not be matched to the source image, otherwise the new translated System.Drawing.Point</returns>
        Friend Function PointToImage(x As Single, y As Single, fitToBounds As Boolean) As System.Drawing.Point
            Return PointToImage(New System.Drawing.Point(CInt(x), CInt(y)), fitToBounds)
        End Function

        ''' <summary>
        '''   Converts the given client System.Drawing.Size System.Drawing.Point to represent a coordinate on the source image.
        ''' </summary>
        ''' <param name="x">The X co-ordinate of the System.Drawing.Point to convert.</param>
        ''' <param name="y">The Y co-ordinate of the System.Drawing.Point to convert.</param>
        ''' <returns><c>System.Drawing.Point.Empty</c> if the System.Drawing.Point could not be matched to the source image, otherwise the new translated System.Drawing.Point</returns>
        Friend Function PointToImage(x As Integer, y As Integer) As System.Drawing.Point
            Return PointToImage(x, y, False)
        End Function

        ''' <summary>
        '''   Converts the given client System.Drawing.Size System.Drawing.Point to represent a coordinate on the source image.
        ''' </summary>
        ''' <param name="x">The X co-ordinate of the System.Drawing.Point to convert.</param>
        ''' <param name="y">The Y co-ordinate of the System.Drawing.Point to convert.</param>
        ''' <param name="fitToBounds">
        '''   if set to <c>true</c> and the System.Drawing.Point is outside the bounds of the source image, it will be mapped to the nearest edge.
        ''' </param>
        ''' <returns><c>System.Drawing.Point.Empty</c> if the System.Drawing.Point could not be matched to the source image, otherwise the new translated System.Drawing.Point</returns>
        Friend Function PointToImage(x As Integer, y As Integer, fitToBounds As Boolean) As System.Drawing.Point
            Return PointToImage(New System.Drawing.Point(x, y), fitToBounds)
        End Function

        ''' <summary>
        '''   Converts the given client System.Drawing.Size System.Drawing.Point to represent a coordinate on the source image.
        ''' </summary>
        ''' <param name="Point">The source System.Drawing.Point.</param>
        ''' <param name="fitToBounds">
        '''   if set to <c>true</c> and the System.Drawing.Point is outside the bounds of the source image, it will be mapped to the nearest edge.
        ''' </param>
        ''' <returns><c>System.Drawing.Point.Empty</c> if the System.Drawing.Point could not be matched to the source image, otherwise the new translated System.Drawing.Point</returns>
        Friend Overridable Function PointToImage(point As System.Drawing.Point, fitToBounds As Boolean) As System.Drawing.Point
            Dim viewport As Rectangle
            Dim x As Integer
            Dim y As Integer

            viewport = GetImageViewPort()

            If Not fitToBounds OrElse viewport.Contains(point) Then
                If AutoScrollPosition <> System.Drawing.Point.Empty Then
                    point = New System.Drawing.Point(point.X - AutoScrollPosition.X, point.Y - AutoScrollPosition.Y)
                End If

                x = CInt((point.X - viewport.X) / ZoomFactor)
                y = CInt((point.Y - viewport.Y) / ZoomFactor)

                If fitToBounds Then
                    If x < 0 Then
                        x = 0
                    ElseIf x > ViewSize.Width Then
                        x = ViewSize.Width
                    End If

                    If y < 0 Then
                        y = 0
                    ElseIf y > ViewSize.Height Then
                        y = ViewSize.Height
                    End If
                End If
            Else
                x = 0 ' Return System.Drawing.Point.Empty if we couldn't match
                y = 0
            End If

            Return New System.Drawing.Point(x, y)
        End Function

        ''' <summary>
        '''   Scrolls the control to the given System.Drawing.Point in the image, offset at the specified display System.Drawing.Point
        ''' </summary>
        ''' <param name="x">The X co-ordinate of the System.Drawing.Point to scroll to.</param>
        ''' <param name="y">The Y co-ordinate of the System.Drawing.Point to scroll to.</param>
        ''' <param name="relativeX">The X co-ordinate relative to the <c>x</c> parameter.</param>
        ''' <param name="relativeY">The Y co-ordinate relative to the <c>y</c> parameter.</param>
        Friend Overloads Sub ScrollTo(x As Integer, y As Integer, relativeX As Integer, relativeY As Integer)
            ScrollTo(New System.Drawing.Point(x, y), New System.Drawing.Point(relativeX, relativeY))
        End Sub

        ''' <summary>
        '''   Scrolls the control to the given System.Drawing.Point in the image, offset at the specified display System.Drawing.Point
        ''' </summary>
        ''' <param name="x">The X co-ordinate of the System.Drawing.Point to scroll to.</param>
        ''' <param name="y">The Y co-ordinate of the System.Drawing.Point to scroll to.</param>
        ''' <param name="relativeX">The X co-ordinate relative to the <c>x</c> parameter.</param>
        ''' <param name="relativeY">The Y co-ordinate relative to the <c>y</c> parameter.</param>
        Friend Overloads Sub ScrollTo(x As Single, y As Single, relativeX As Single, relativeY As Single)
            ScrollTo(New System.Drawing.Point(CInt(x), CInt(y)), New System.Drawing.Point(CInt(relativeX), CInt(relativeY)))
        End Sub

        ''' <summary>
        '''   Scrolls the control to the given System.Drawing.Point in the image, offset at the specified display System.Drawing.Point
        ''' </summary>
        ''' <param name="imageLocation">The System.Drawing.Point of the image to attempt to scroll to.</param>
        ''' <param name="relativeDisplayPoint">The relative display System.Drawing.Point to offset scrolling by.</param>
        Friend Overridable Overloads Sub ScrollTo(imageLocation As System.Drawing.Point, relativeDisplayPoint As System.Drawing.Point)
            Dim x As Integer
            Dim y As Integer

            x = CInt(imageLocation.X * ZoomFactor) - relativeDisplayPoint.X
            y = CInt(imageLocation.Y * ZoomFactor) - relativeDisplayPoint.Y

            AutoScrollPosition = New System.Drawing.Point(x, y)
        End Sub

        ''' <summary>
        '''   Creates a selection region which encompasses the entire image
        ''' </summary>
        ''' <exception cref="System.InvalidOperationException">Thrown if no image is currently set</exception>
        Friend Overridable Sub SelectAll()
            SelectionRegion = New RectangleF(PointF.Empty, ViewSize)
        End Sub

        ''' <summary>
        '''   Clears any existing selection region
        ''' </summary>
        Friend Overridable Sub SelectNone()
            SelectionRegion = RectangleF.Empty
        End Sub

        ''' <summary>
        '''   Zooms into the image
        ''' </summary>
        Friend Overridable Sub ZoomIn()
            ZoomIn(True)
        End Sub

        ''' <summary>
        '''   Zooms into the image
        ''' </summary>
        ''' <param name="preservePosition"><c>true</c> if the current scrolling position should be preserved relative to the new zoom level, <c>false</c> to reset.</param>
        Friend Overridable Sub ZoomIn(preservePosition As Boolean)
            PerformZoomIn(ImageBoxActionSources.Unknown, preservePosition)
        End Sub

        ''' <summary>
        '''   Zooms out of the image
        ''' </summary>
        Friend Overridable Sub ZoomOut()
            ZoomOut(True)
        End Sub

        ''' <summary>
        '''   Zooms out of the image
        ''' </summary>
        ''' <param name="preservePosition"><c>true</c> if the current scrolling position should be preserved relative to the new zoom level, <c>false</c> to reset.</param>
        Friend Overridable Sub ZoomOut(preservePosition As Boolean)
            PerformZoomOut(ImageBoxActionSources.Unknown, preservePosition)
        End Sub

        ''' <summary>
        '''   Zooms to the maximum System.Drawing.Size for displaying the entire image within the bounds of the control.
        ''' </summary>
        Friend Overridable Sub ZoomToFit()
            If Not ViewSize.IsEmpty Then
                Dim innerRectangle As Rectangle
                Dim zoom As Double
                Dim aspectRatio As Double

                AutoScrollMinSize = System.Drawing.Size.Empty

                innerRectangle = GetInsideViewPort(True)

                If ViewSize.Width > ViewSize.Height Then
                    aspectRatio = innerRectangle.Width / ViewSize.Width
                    zoom = aspectRatio * 100.0

                    If innerRectangle.Height < ViewSize.Height * zoom / 100.0 Then
                        aspectRatio = innerRectangle.Height / ViewSize.Height
                        zoom = aspectRatio * 100.0
                    End If
                Else
                    aspectRatio = innerRectangle.Height / ViewSize.Height
                    zoom = aspectRatio * 100.0

                    If innerRectangle.Width < ViewSize.Width * zoom / 100.0 Then
                        aspectRatio = innerRectangle.Width / ViewSize.Width
                        zoom = aspectRatio * 100.0
                    End If
                End If

                Me.Zoom = CInt(Math.Round(Math.Floor(zoom)))
            End If
        End Sub

        ''' <summary>
        '''   Adjusts the view port to fit the given region
        ''' </summary>
        ''' <param name="x">The X co-ordinate of the selection region.</param>
        ''' <param name="y">The Y co-ordinate of the selection region.</param>
        ''' <param name="width">The width of the selection region.</param>
        ''' <param name="height">The height of the selection region.</param>
        Friend Sub ZoomToRegion(x As Single, y As Single, width As Single, height As Single)
            ZoomToRegion(New RectangleF(x, y, width, height))
        End Sub

        ''' <summary>
        '''   Adjusts the view port to fit the given region
        ''' </summary>
        ''' <param name="x">The X co-ordinate of the selection region.</param>
        ''' <param name="y">The Y co-ordinate of the selection region.</param>
        ''' <param name="width">The width of the selection region.</param>
        ''' <param name="height">The height of the selection region.</param>
        Friend Sub ZoomToRegion(x As Integer, y As Integer, width As Integer, height As Integer)
            ZoomToRegion(New RectangleF(x, y, width, height))
        End Sub

        ''' <summary>
        '''   Adjusts the view port to fit the given region
        ''' </summary>
        ''' <param name="rectangle">The rectangle to fit the view port to.</param>
        Friend Overridable Sub ZoomToRegion(rectangle As RectangleF)
            Dim ratioX As Double
            Dim ratioY As Double
            Dim zoomFactor As Double
            Dim cx As Integer
            Dim cy As Integer

            ratioX = ClientSize.Width / rectangle.Width
            ratioY = ClientSize.Height / rectangle.Height
            zoomFactor = Math.Min(ratioX, ratioY)
            cx = CInt(rectangle.X + rectangle.Width / 2)
            cy = CInt(rectangle.Y + rectangle.Height / 2)

            Zoom = CInt(zoomFactor * 100)
            CenterAt(New System.Drawing.Point(cx, cy))
        End Sub

        ''' <summary>
        '''   Adjusts the layout.
        ''' </summary>
        Protected Overridable Sub AdjustLayout()
            If AllowPainting Then
                If AutoSize Then
                    AdjustSize()
                ElseIf SizeMode <> ImageBoxSizeMode.Normal Then
                    ZoomToFit()
                ElseIf AutoScroll Then
                    AdjustViewPort()
                End If

                Invalidate()
            End If
        End Sub

        ''' <summary>
        '''   Adjusts the scroll.
        ''' </summary>
        ''' <param name="x">The x.</param>
        ''' <param name="y">The y.</param>
        Protected Overridable Sub AdjustScroll(x As Integer, y As Integer)
            Dim scrollPosition As System.Drawing.Point

            scrollPosition = New System.Drawing.Point(HorizontalScroll.Value + x, VerticalScroll.Value + y)

            UpdateScrollPosition(scrollPosition)
        End Sub

        ''' <summary>
        '''   Adjusts the System.Drawing.Size.
        ''' </summary>
        Protected Overridable Sub AdjustSize()
            If AutoSize AndAlso Dock = DockStyle.None Then
                Size = PreferredSize
            End If
        End Sub

        ''' <summary>
        '''   Adjusts the view port.
        ''' </summary>
        Protected Overridable Sub AdjustViewPort()
            If AutoScroll AndAlso Not ViewSize.IsEmpty Then
                AutoScrollMinSize = New System.Drawing.Size(ScaledImageWidth + Padding.Horizontal, ScaledImageHeight + Padding.Vertical)
            End If
        End Sub

        ''' <summary>
        '''   Creates the grid tile image.
        ''' </summary>
        ''' <param name="cellSize">System.Drawing.Size of the cell.</param>
        ''' <param name="firstColor">The first color.</param>
        ''' <param name="secondColor">Color of the second.</param>
        ''' <returns></returns>
        Protected Overridable Function CreateGridTileImage(cellSize As Integer, firstColor As Color, secondColor As Color) As Bitmap
            Dim scale As Single

            ' rescale the cell System.Drawing.Size
            Select Case GridScale
                Case ImageBoxGridScale.Medium
                    scale = 1.5F

                Case ImageBoxGridScale.Large
                    scale = 2

                Case ImageBoxGridScale.Tiny
                    scale = 0.5F
                Case Else
                    scale = 1
            End Select

            cellSize = CInt(cellSize * scale)

            Return CreateCheckerBoxTile(cellSize, firstColor, secondColor)
        End Function

        ''' <summary>
        '''   Clean up any resources being used.
        ''' </summary>
        ''' <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        Protected Overrides Sub Dispose(disposing As Boolean)
            If disposing Then
                If IsAnimating Then
                    Call ImageAnimator.StopAnimate(Image, New EventHandler(AddressOf OnFrameChangedHandler))
                End If

                If _texture IsNot Nothing Then
                    _texture.Dispose()
                    _texture = Nothing
                End If

                If _gridTile IsNot Nothing Then
                    _gridTile.Dispose()
                    _gridTile = Nothing
                End If

                KillTimer()
            End If

            MyBase.Dispose(disposing)
        End Sub

        ''' <summary>
        ''' Draws the background of the control.
        ''' </summary>
        ''' <param name="e">The <see cref="PaintEventArgs"/> instance containing the event data.</param>
        Protected Overridable Sub DrawBackground(e As PaintEventArgs)
            Dim innerRectangle As Rectangle

            innerRectangle = GetInsideViewPort()

            Using brush As SolidBrush = New SolidBrush(BackColor)
                e.Graphics.FillRectangle(brush, innerRectangle)
            End Using

            If _texture IsNot Nothing AndAlso GridDisplayMode <> ImageBoxGridDisplayMode.None Then
                Select Case GridDisplayMode
                    Case ImageBoxGridDisplayMode.Image
                        Dim fillRectangle As Rectangle

                        fillRectangle = GetImageViewPort()
                        e.Graphics.FillRectangle(_texture, fillRectangle)

                    Case ImageBoxGridDisplayMode.Client
                        e.Graphics.FillRectangle(_texture, innerRectangle)
                End Select
            End If
        End Sub

        ''' <summary>
        '''   Draws a drop shadow.
        ''' </summary>
        ''' <param name="g">The graphics. </param>
        ''' <param name="viewPort"> The view port. </param>
        Protected Overridable Sub DrawDropShadow(g As Graphics, viewPort As Rectangle)
            Dim rightEdge As Rectangle
            Dim bottomEdge As Rectangle

            rightEdge = New Rectangle(viewPort.Right + 1, viewPort.Top + DropShadowSize, DropShadowSize, viewPort.Height)
            bottomEdge = New Rectangle(viewPort.Left + DropShadowSize, viewPort.Bottom + 1, viewPort.Width + 1, DropShadowSize)

            Using brush As Brush = New SolidBrush(ImageBorderColor)
                g.FillRectangles(brush, {rightEdge, bottomEdge})
            End Using
        End Sub

        ''' <summary>
        '''   Draws a glow shadow.
        ''' </summary>
        ''' <param name="g">The graphics.</param>
        ''' <param name="viewPort">The view port.</param>
        Protected Overridable Sub DrawGlowShadow(g As Graphics, viewPort As Rectangle)
            ' Glow code adapted from http://www.codeproject.com/Articles/372743/gGlowBox-Create-a-glow-effect-around-a-focused-con

            g.SetClip(viewPort, CombineMode.Exclude) ' make sure the inside glow doesn't appear

            Using path As GraphicsPath = New GraphicsPath()
                Dim glowSize As Integer
                Dim feather As Integer

                path.AddRectangle(viewPort)
                glowSize = DropShadowSize * 3
                feather = 50

                For i = 1 To glowSize Step 2
                    Dim alpha As Integer

                    alpha = CInt(feather - feather / glowSize * i)

                    Using pen As Pen = New Pen(Color.FromArgb(alpha, ImageBorderColor), i) With {
            .LineJoin = LineJoin.Round
          }
                        g.DrawPath(pen, path)
                    End Using
                Next
            End Using
        End Sub

        ''' <summary>
        '''   Draws the image.
        ''' </summary>
        ''' <param name="g">The g.</param>
        Protected Overridable Sub DrawImage(g As Graphics)
            Dim currentInterpolationMode As InterpolationMode
            Dim currentPixelOffsetMode As PixelOffsetMode

            currentInterpolationMode = g.InterpolationMode
            currentPixelOffsetMode = g.PixelOffsetMode

            g.InterpolationMode = GetInterpolationMode()

            ' disable pixel offsets. Thanks to Rotem for the info.
            ' http://stackoverflow.com/questions/14070311/why-is-graphics-drawimage-cropping-part-of-my-image/14070372#14070372
            g.PixelOffsetMode = PixelOffsetMode.HighQuality

            Try
                ' Animation. Thanks to teamalpha5441 for the contribution
                If IsAnimating AndAlso Not DesignMode Then
                    ImageAnimator.UpdateFrames(Image)
                End If

                g.DrawImage(Image, GetImageViewPort(), GetSourceImageRegion(), GraphicsUnit.Pixel)
            Catch ex As Exception
                TextRenderer.DrawText(g, ex.Message, Font, ClientRectangle, ForeColor, BackColor, TextFormatFlags.VerticalCenter Or TextFormatFlags.HorizontalCenter Or TextFormatFlags.WordBreak Or TextFormatFlags.NoPadding Or TextFormatFlags.NoPrefix)
            End Try

            g.PixelOffsetMode = currentPixelOffsetMode
            g.InterpolationMode = currentInterpolationMode
        End Sub

        ''' <summary>
        '''   Draws a border around the image.
        ''' </summary>
        ''' <param name="graphics"> The graphics. </param>
        Protected Overridable Sub DrawImageBorder(graphics As Graphics)
            If ImageBorderStyle <> ImageBoxBorderStyle.None Then
                Dim viewPort As Rectangle

                graphics.SetClip(GetInsideViewPort()) ' make sure the image border doesn't overwrite the control border

                viewPort = GetImageViewPort()
                viewPort = New Rectangle(viewPort.Left - 1, viewPort.Top - 1, viewPort.Width + 1, viewPort.Height + 1)

                Using borderPen As Pen = New Pen(ImageBorderColor)
                    graphics.DrawRectangle(borderPen, viewPort)
                End Using

                Select Case ImageBorderStyle
                    Case ImageBoxBorderStyle.FixedSingleDropShadow
                        DrawDropShadow(graphics, viewPort)
                    Case ImageBoxBorderStyle.FixedSingleGlowShadow
                        DrawGlowShadow(graphics, viewPort)
                End Select

                graphics.ResetClip()
            End If
        End Sub

        ''' <summary>
        ''' Draws the specified text within the specified bounds using the specified device context.
        ''' </summary>
        ''' <param name="graphics">The device context in which to draw the text.</param>
        ''' <param name="text">The text to draw.</param>
        ''' <param name="bounds">The <see cref="Rectangle"/> that represents the bounds of the text.</param>
        Protected Sub DrawLabel(graphics As Graphics, text As String, bounds As Rectangle)
            DrawLabel(graphics, text, Font, ForeColor, TextBackColor, TextAlign, bounds)
        End Sub

        ''' <summary>
        ''' Draws the specified text within the specified bounds using the specified device context and font.
        ''' </summary>
        ''' <param name="graphics">The device context in which to draw the text.</param>
        ''' <param name="text">The text to draw.</param>
        ''' <param name="font">The <see cref="Font"/> to apply to the drawn text.</param>
        ''' <param name="bounds">The <see cref="Rectangle"/> that represents the bounds of the text.</param>
        Protected Sub DrawLabel(graphics As Graphics, text As String, font As Font, bounds As Rectangle)
            DrawLabel(graphics, text, font, ForeColor, TextBackColor, TextAlign, bounds)
        End Sub

        ''' <summary>
        ''' Draws the specified text within the specified bounds using the specified device context, font, and color.
        ''' </summary>
        ''' <param name="graphics">The device context in which to draw the text.</param>
        ''' <param name="text">The text to draw.</param>
        ''' <param name="font">The <see cref="Font"/> to apply to the drawn text.</param>
        ''' <param name="foreColor">The <see cref="Color"/> to apply to the text.</param>
        ''' <param name="bounds">The <see cref="Rectangle"/> that represents the bounds of the text.</param>
        Protected Sub DrawLabel(graphics As Graphics, text As String, font As Font, foreColor As Color, bounds As Rectangle)
            DrawLabel(graphics, text, font, foreColor, TextBackColor, TextAlign, bounds)
        End Sub

        ''' <summary>
        ''' Draws the specified text within the specified bounds using the specified device context, font, color, and back color.
        ''' </summary>
        ''' <param name="graphics">The device context in which to draw the text.</param>
        ''' <param name="text">The text to draw.</param>
        ''' <param name="font">The <see cref="Font"/> to apply to the drawn text.</param>
        ''' <param name="foreColor">The <see cref="Color"/> to apply to the text.</param>
        ''' <param name="backColor">The <see cref="Color"/> to apply to the area represented by <c>bounds</c>.</param>
        ''' <param name="bounds">The <see cref="Rectangle"/> that represents the bounds of the text.</param>
        Protected Sub DrawLabel(graphics As Graphics, text As String, font As Font, foreColor As Color, backColor As Color, bounds As Rectangle)
            DrawLabel(graphics, text, font, foreColor, backColor, TextAlign, bounds)
        End Sub

        ''' <summary>
        ''' Draws the specified text within the specified bounds using the specified device context, font, color, back color, and formatting instructions.
        ''' </summary>
        ''' <param name="graphics">The device context in which to draw the text.</param>
        ''' <param name="text">The text to draw.</param>
        ''' <param name="font">The <see cref="Font"/> to apply to the drawn text.</param>
        ''' <param name="foreColor">The <see cref="Color"/> to apply to the text.</param>
        ''' <param name="backColor">The <see cref="Color"/> to apply to the area represented by <c>bounds</c>.</param>
        ''' <param name="textAlign">The <see cref="ContentAlignment"/> to apply to the text.</param>
        ''' <param name="bounds">The <see cref="Rectangle"/> that represents the bounds of the text.</param>
        Protected Sub DrawLabel(graphics As Graphics, text As String, font As Font, foreColor As Color, backColor As Color, textAlign As ContentAlignment, bounds As Rectangle)
            DrawLabel(graphics, text, font, foreColor, backColor, textAlign, bounds, ScaleText)
        End Sub

        ''' <summary>
        ''' Draws the specified text within the specified bounds using the specified device context, font, color, back color, and formatting instructions.
        ''' </summary>
        ''' <param name="graphics">The device context in which to draw the text.</param>
        ''' <param name="text">The text to draw.</param>
        ''' <param name="font">The <see cref="Font"/> to apply to the drawn text.</param>
        ''' <param name="foreColor">The <see cref="Color"/> to apply to the text.</param>
        ''' <param name="backColor">The <see cref="Color"/> to apply to the area represented by <c>bounds</c>.</param>
        ''' <param name="textAlign">The <see cref="ContentAlignment"/> to apply to the text.</param>
        ''' <param name="bounds">The <see cref="Rectangle"/> that represents the bounds of the text.</param>
        ''' <param name="scaleText">If set to <c>true</c> the font System.Drawing.Size is scaled according to the current zoom level.</param>
        Protected Overridable Sub DrawLabel(graphics As Graphics, text As String, font As Font, foreColor As Color, backColor As Color, textAlign As ContentAlignment, bounds As Rectangle, scaleText As Boolean)
            DrawLabel(graphics, text, font, foreColor, backColor, textAlign, bounds, scaleText, Padding.Empty)
        End Sub

        ''' <summary>
        ''' Draws the specified text within the specified bounds using the specified device context, font, color, back color, and formatting instructions.
        ''' </summary>
        ''' <param name="graphics">The device context in which to draw the text.</param>
        ''' <param name="text">The text to draw.</param>
        ''' <param name="font">The <see cref="Font"/> to apply to the drawn text.</param>
        ''' <param name="foreColor">The <see cref="Color"/> to apply to the text.</param>
        ''' <param name="backColor">The <see cref="Color"/> to apply to the area represented by <c>bounds</c>.</param>
        ''' <param name="textAlign">The <see cref="ContentAlignment"/> to apply to the text.</param>
        ''' <param name="bounds">The <see cref="Rectangle"/> that represents the bounds of the text.</param>
        ''' <param name="scaleText">If set to <c>true</c> the font System.Drawing.Size is scaled according to the current zoom level.</param>
        ''' <param name="padding">Padding to apply around the text</param>
        Protected Overridable Sub DrawLabel(graphics As Graphics, text As String, font As System.Drawing.Font, foreColor As Color, backColor As Color, textAlign As ContentAlignment, bounds As Rectangle, scaleText As Boolean, padding As Padding)
            Dim flags As TextFormatFlags

            If scaleText Then
                font = New System.Drawing.Font(font.FontFamily, CInt(font.Size * ZoomFactor), font.Style)
            End If

            flags = TextFormatFlags.NoPrefix Or TextFormatFlags.WordEllipsis Or TextFormatFlags.WordBreak Or TextFormatFlags.NoPadding

            Select Case textAlign
                Case ContentAlignment.TopLeft, ContentAlignment.MiddleLeft, ContentAlignment.BottomLeft
                    flags = flags Or TextFormatFlags.Left
                Case ContentAlignment.TopRight, ContentAlignment.MiddleRight, ContentAlignment.BottomRight
                    flags = flags Or TextFormatFlags.Right
                Case Else
                    flags = flags Or TextFormatFlags.HorizontalCenter
            End Select

            Select Case textAlign
                Case ContentAlignment.TopCenter, ContentAlignment.TopLeft, ContentAlignment.TopRight
                    flags = flags Or TextFormatFlags.Top
                Case ContentAlignment.BottomCenter, ContentAlignment.BottomLeft, ContentAlignment.BottomRight
                    flags = flags Or TextFormatFlags.Bottom
                Case Else
                    flags = flags Or TextFormatFlags.VerticalCenter
            End Select

            If padding.Horizontal <> 0 OrElse padding.Vertical <> 0 Then
                Dim size As System.Drawing.Size
                Dim x As Integer
                Dim y As Integer
                Dim width As Integer
                Dim height As Integer

                size = TextRenderer.MeasureText(graphics, text, font, bounds.Size, flags)
                width = size.Width
                height = size.Height

                Select Case textAlign
                    Case ContentAlignment.TopLeft
                        x = bounds.Left + padding.Left
                        y = bounds.Top + padding.Top
                    Case ContentAlignment.TopCenter
                        x = CInt(bounds.Left + padding.Left + ((bounds.Width - width) / 2 - padding.Right))
                        y = bounds.Top + padding.Top
                    Case ContentAlignment.TopRight
                        x = bounds.Right - (padding.Right + width)
                        y = bounds.Top + padding.Top
                    Case ContentAlignment.MiddleLeft
                        x = bounds.Left + padding.Left
                        y = CInt(bounds.Top + padding.Top + (bounds.Height - height) / 2)
                    Case ContentAlignment.MiddleCenter
                        x = CInt(bounds.Left + padding.Left + ((bounds.Width - width) / 2 - padding.Right))
                        y = CInt(bounds.Top + padding.Top + (bounds.Height - height) / 2)
                    Case ContentAlignment.MiddleRight
                        x = bounds.Right - (padding.Right + width)
                        y = CInt(bounds.Top + padding.Top + (bounds.Height - height) / 2)
                    Case ContentAlignment.BottomLeft
                        x = bounds.Left + padding.Left
                        y = bounds.Bottom - (padding.Bottom + height)
                    Case ContentAlignment.BottomCenter
                        x = CInt(bounds.Left + padding.Left + ((bounds.Width - width) / 2 - padding.Right))
                        y = bounds.Bottom - (padding.Bottom + height)
                    Case ContentAlignment.BottomRight
                        x = bounds.Right - (padding.Right + width)
                        y = bounds.Bottom - (padding.Bottom + height)
                    Case Else
                        Throw New ArgumentOutOfRangeException(NameOf(textAlign))
                End Select

                If backColor <> Color.Empty AndAlso backColor.A > 0 Then
                    Using brush As Brush = New SolidBrush(backColor)
                        graphics.FillRectangle(brush, x - padding.Left, y - padding.Top, width + padding.Horizontal, height + padding.Vertical)
                    End Using
                End If

                bounds = New Rectangle(x, y, width, height)

                'bounds = new Rectangle(bounds.Left + padding.Left, bounds.Top + padding.Top, bounds.Width - padding.Horizontal, bounds.Height - padding.Vertical);
            End If

            TextRenderer.DrawText(graphics, text, font, bounds, foreColor, backColor, flags)

            If scaleText Then
                font.Dispose()
            End If
        End Sub

        ''' <summary>
        '''   Draws a pixel grid.
        ''' </summary>
        ''' <param name="g">The graphics to draw the grid to.</param>
        Protected Overridable Sub DrawPixelGrid(g As Graphics)
            Dim pixelSize As Single

            pixelSize = CSng(ZoomFactor)

            If pixelSize > PixelGridThreshold Then
                Dim viewport As Rectangle
                Dim offsetX As Single
                Dim offsetY As Single

                viewport = GetImageViewPort()
                offsetX = Math.Abs(AutoScrollPosition.X) Mod pixelSize
                offsetY = Math.Abs(AutoScrollPosition.Y) Mod pixelSize

                Using pen As Pen = New Pen(PixelGridColor) With {
          .DashStyle = DashStyle.Dot
        }
                    Dim x = viewport.Left + pixelSize - offsetX

                    While x < viewport.Right
                        g.DrawLine(pen, x, viewport.Top, x, viewport.Bottom)
                        x += pixelSize
                    End While

                    Dim y = viewport.Top + pixelSize - offsetY

                    While y < viewport.Bottom
                        g.DrawLine(pen, viewport.Left, y, viewport.Right, y)
                        y += pixelSize
                    End While

                    g.DrawRectangle(pen, viewport)
                End Using
            End If
        End Sub

        ''' <summary>
        '''   Draws the selection region.
        ''' </summary>
        ''' <param name="e">
        '''   The <see cref="System.Windows.Forms.PaintEventArgs"/> instance containing the event data.
        ''' </param>
        Protected Overridable Sub DrawSelection(e As PaintEventArgs)
            Dim rect As RectangleF

            e.Graphics.SetClip(GetInsideViewPort(True))

            rect = GetOffsetRectangle(SelectionRegion)

            Using brush As Brush = New SolidBrush(Color.FromArgb(128, SelectionColor))
                e.Graphics.FillRectangle(brush, rect)
            End Using

            Using pen As Pen = New Pen(SelectionColor)
                e.Graphics.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height)
            End Using

            e.Graphics.ResetClip()
        End Sub

        ''' <summary>
        ''' Draws the text.
        ''' </summary>
        ''' <param name="e">The <see cref="PaintEventArgs"/> instance containing the event data.</param>
        Protected Overridable Sub DrawText(e As PaintEventArgs)
            Dim bounds As Rectangle

            bounds = If(TextDisplayMode = ImageBoxGridDisplayMode.Client, GetInsideViewPort(), GetImageViewPort())

            DrawLabel(e.Graphics, Text, Font, ForeColor, TextBackColor, TextAlign, bounds, ScaleText, TextPadding)
        End Sub

        ''' <summary>
        ''' Completes an ongoing selection or drag operation.
        ''' </summary>
        Protected Overridable Sub EndDrag()
            IsSelecting = False
            OnSelected(EventArgs.Empty)
        End Sub

        ''' <summary>
        ''' Gets a cursor suitable for the current state of the control
        ''' </summary>
        ''' <param name="location">The mouse cursor position in client co-ordinates.</param>
        ''' <returns>
        ''' A <see cref="Cursor"/> object suitable for the current state of the control
        ''' </returns>
        Protected Overridable Function GetCursor(location As System.Drawing.Point) As Cursor
            Dim cursor As Cursor

            Select Case _panStyle
                Case ImageBoxPanStyle.None
                    cursor = Cursors.Default
                Case ImageBoxPanStyle.Standard
                    cursor = Cursors.SizeAll
                Case ImageBoxPanStyle.Free
                    Select Case GetPanDirection(location)
                        Case ImageBoxPanDirection.None
                            cursor = _panAllCursor
                        Case ImageBoxPanDirection.Up
                            cursor = Cursors.PanNorth
                        Case ImageBoxPanDirection.Down
                            cursor = Cursors.PanSouth
                        Case ImageBoxPanDirection.Left
                            cursor = Cursors.PanWest
                        Case ImageBoxPanDirection.Right
                            cursor = Cursors.PanEast
                        Case Else
                            cursor = _panAllCursor
                    End Select

                Case Else
                    cursor = Cursors.Default
            End Select

            Return cursor
        End Function

        ''' <summary>
        '''   Gets an offset based on the current image border style.
        ''' </summary>
        ''' <returns></returns>
        Protected Overridable Function GetImageBorderOffset() As Integer
            Dim offset As Integer

            Select Case ImageBorderStyle
                Case ImageBoxBorderStyle.FixedSingle
                    offset = 1

                Case ImageBoxBorderStyle.FixedSingleDropShadow
                    offset = DropShadowSize + 1
                Case Else
                    offset = 0
            End Select

            Return offset
        End Function

        ''' <summary>
        ''' Gets the interpolation mode used to render the image.
        ''' </summary>
        ''' <returns>
        ''' The interpolation mode.
        ''' </returns>
        ''' <remarks>Returns the value of the <see cref="InterpolationMode"/> property, unless this is set to <code>InterpolationMode.Default</code>, in which case it will use <code>InterpolationMode.HighQualityBicubic</code> for zoomed images otherwise <code>InterpolationMode.NearestNeighbor</code>.</remarks>
        Protected Overridable Function GetInterpolationMode() As InterpolationMode
            Dim mode As InterpolationMode

            mode = InterpolationMode

            If mode = InterpolationMode.Default Then
                ' ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
                If Zoom < 100 Then
                    ' TODO: Check to see if we should cherry pick other modes depending on how much the image is actually zoomed
                    mode = InterpolationMode.HighQualityBicubic
                Else
                    mode = InterpolationMode.NearestNeighbor
                End If
            End If

            Return mode
        End Function

        ''' <summary>
        '''   Determines whether the specified key is a regular input key or a special key that requires preprocessing.
        ''' </summary>
        ''' <param name="keyData">
        '''   One of the <see cref="T:System.Windows.Forms.Keys"/> values.
        ''' </param>
        ''' <returns>
        '''   true if the specified key is a regular input key; otherwise, false.
        ''' </returns>
        Protected Overrides Function IsInputKey(keyData As Keys) As Boolean
            Dim result As Boolean

            If (keyData And Keys.Right) = Keys.Right Or (keyData And Keys.Left) = Keys.Left Or (keyData And Keys.Up) = Keys.Up Or (keyData And Keys.Down) = Keys.Down Then
                result = True
            Else
                result = MyBase.IsInputKey(keyData)
            End If

            Return result
        End Function

        ''' <summary>
        '''   Raises the <see cref="AllowClickZoomChanged"/> event.
        ''' </summary>
        ''' <param name="e">
        '''   The <see cref="System.EventArgs"/> instance containing the event data.
        ''' </param>
        Protected Overridable Sub OnAllowClickZoomChanged(e As EventArgs)
            Dim handler As EventHandler

            handler = CType(Events(_eventAllowClickZoomChanged), EventHandler)

            handler?.Invoke(Me, e)
        End Sub

        ''' <summary>
        '''   Raises the <see cref="AllowDoubleClickChanged"/> event.
        ''' </summary>
        ''' <param name="e">
        '''   The <see cref="EventArgs"/> instance containing the event data.
        ''' </param>
        Protected Overridable Sub OnAllowDoubleClickChanged(e As EventArgs)
            Dim handler As EventHandler

            SetStyle(ControlStyles.StandardDoubleClick, AllowDoubleClick)

            handler = CType(Events(_eventAllowDoubleClickChanged), EventHandler)

            handler?.Invoke(Me, e)
        End Sub

        ''' <summary>
        ''' Raises the <see cref="AllowFreePanChanged"/> event.
        ''' </summary>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Protected Overridable Sub OnAllowFreePanChanged(e As EventArgs)
            Dim handler As EventHandler

            handler = CType(Events(_eventAllowFreePanChanged), EventHandler)

            handler?.Invoke(Me, e)
        End Sub

        ''' <summary>
        ''' Raises the <see cref="AllowUnfocusedMouseWheelChanged"/> event.
        ''' </summary>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Protected Overridable Sub OnAllowUnfocusedMouseWheelChanged(e As EventArgs)
            Dim handler As EventHandler

            If AllowUnfocusedMouseWheel Then
                ' TODO: Not doing any reference counting so there's
                ' currently no way of disabling the message filter
                ' after the first time it has been enabled
                ImageBoxMouseWheelMessageFilter.Active = True
            End If

            handler = CType(Events(_eventAllowUnfocusedMouseWheelChanged), EventHandler)

            handler?.Invoke(Me, e)
        End Sub

        ''' <summary>
        '''   Raises the <see cref="AllowZoomChanged"/> event.
        ''' </summary>
        ''' <param name="e">
        '''   The <see cref="System.EventArgs"/> instance containing the event data.
        ''' </param>
        Protected Overridable Sub OnAllowZoomChanged(e As EventArgs)
            Dim handler As EventHandler

            handler = CType(Events(_eventAllowZoomChanged), EventHandler)

            handler?.Invoke(Me, e)
        End Sub

        ''' <summary>
        '''   Raises the <see cref="AutoCenterChanged"/> event.
        ''' </summary>
        ''' <param name="e">
        '''   The <see cref="System.EventArgs"/> instance containing the event data.
        ''' </param>
        Protected Overridable Sub OnAutoCenterChanged(e As EventArgs)
            Dim handler As EventHandler

            Invalidate()

            handler = CType(Events(_eventAutoCenterChanged), EventHandler)

            handler?.Invoke(Me, e)
        End Sub

        ''' <summary>
        '''   Raises the <see cref="AutoPanChanged"/> event.
        ''' </summary>
        ''' <param name="e">
        '''   The <see cref="System.EventArgs"/> instance containing the event data.
        ''' </param>
        Protected Overridable Sub OnAutoPanChanged(e As EventArgs)
            Dim handler As EventHandler

            handler = CType(Events(_eventAutoPanChanged), EventHandler)

            handler?.Invoke(Me, e)
        End Sub

        ''' <summary>
        '''   Raises the <see cref="System.Windows.Forms.Control.BackColorChanged"/> event.
        ''' </summary>
        ''' <param name="e">
        '''   An <see cref="T:System.EventArgs"/> that contains the event data.
        ''' </param>
        Protected Overrides Sub OnBackColorChanged(e As EventArgs)
            MyBase.OnBackColorChanged(e)

            Invalidate()
        End Sub

        ''' <summary>
        '''   Raises the <see cref="ScrollControl.BorderStyleChanged"/> event.
        ''' </summary>
        ''' <param name="e">
        '''   The <see cref="EventArgs"/> instance containing the event data.
        ''' </param>
        Protected Overrides Sub OnBorderStyleChanged(e As EventArgs)
            MyBase.OnBorderStyleChanged(e)

            AdjustLayout()
        End Sub

        ''' <summary>
        '''   Raises the <see cref="System.Windows.Forms.Control.DockChanged"/> event.
        ''' </summary>
        ''' <param name="e">
        '''   An <see cref="T:System.EventArgs"/> that contains the event data.
        ''' </param>
        Protected Overrides Sub OnDockChanged(e As EventArgs)
            MyBase.OnDockChanged(e)

            If Dock <> DockStyle.None Then
                AutoSize = False
            End If
        End Sub

        ''' <summary>
        '''   Raises the <see cref="DropShadowSizeChanged"/> event.
        ''' </summary>
        ''' <param name="e">
        '''   The <see cref="EventArgs"/> instance containing the event data.
        ''' </param>
        Protected Overridable Sub OnDropShadowSizeChanged(e As EventArgs)
            Dim handler As EventHandler

            Invalidate()

            handler = CType(Events(_eventDropShadowSizeChanged), EventHandler)

            handler?.Invoke(Me, e)
        End Sub

        ''' <summary>
        ''' Raises the <see cref="E:System.Windows.Forms.Control.FontChanged"/> event.
        ''' </summary>
        ''' <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        Protected Overrides Sub OnFontChanged(e As EventArgs)
            MyBase.OnFontChanged(e)

            Invalidate()
        End Sub

        ''' <summary>
        ''' Raises the <see cref="E:System.Windows.Forms.Control.ForeColorChanged"/> event.
        ''' </summary>
        ''' <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        Protected Overrides Sub OnForeColorChanged(e As EventArgs)
            MyBase.OnForeColorChanged(e)

            Invalidate()
        End Sub

        ''' <summary>
        '''   Raises the <see cref="GridCellSizeChanged"/> event.
        ''' </summary>
        ''' <param name="e">
        '''   The <see cref="System.EventArgs"/> instance containing the event data.
        ''' </param>
        Protected Overridable Sub OnGridCellSizeChanged(e As EventArgs)
            Dim handler As EventHandler

            InitializeGridTile()

            handler = CType(Events(_eventGridCellSizeChanged), EventHandler)

            handler?.Invoke(Me, e)
        End Sub

        ''' <summary>
        '''   Raises the <see cref="GridColorAlternateChanged"/> event.
        ''' </summary>
        ''' <param name="e">
        '''   The <see cref="System.EventArgs"/> instance containing the event data.
        ''' </param>
        Protected Overridable Sub OnGridColorAlternateChanged(e As EventArgs)
            Dim handler As EventHandler

            InitializeGridTile()

            handler = CType(Events(_eventGridColorAlternateChanged), EventHandler)

            handler?.Invoke(Me, e)
        End Sub

        ''' <summary>
        '''   Raises the <see cref="GridColorChanged"/> event.
        ''' </summary>
        ''' <param name="e">
        '''   The <see cref="System.EventArgs"/> instance containing the event data.
        ''' </param>
        Protected Overridable Sub OnGridColorChanged(e As EventArgs)
            Dim handler As EventHandler

            InitializeGridTile()

            handler = CType(Events(_eventGridColorChanged), EventHandler)

            handler?.Invoke(Me, e)
        End Sub

        ''' <summary>
        '''   Raises the <see cref="GridDisplayModeChanged"/> event.
        ''' </summary>
        ''' <param name="e">
        '''   The <see cref="System.EventArgs"/> instance containing the event data.
        ''' </param>
        Protected Overridable Sub OnGridDisplayModeChanged(e As EventArgs)
            Dim handler As EventHandler

            InitializeGridTile()
            Invalidate()

            handler = CType(Events(_eventGridDisplayModeChanged), EventHandler)

            handler?.Invoke(Me, e)
        End Sub

        ''' <summary>
        '''   Raises the <see cref="GridScaleChanged"/> event.
        ''' </summary>
        ''' <param name="e">
        '''   The <see cref="System.EventArgs"/> instance containing the event data.
        ''' </param>
        Protected Overridable Sub OnGridScaleChanged(e As EventArgs)
            Dim handler As EventHandler

            InitializeGridTile()

            handler = CType(Events(_eventGridScaleChanged), EventHandler)

            handler?.Invoke(Me, e)
        End Sub

        ''' <summary>
        '''   Raises the <see cref="ImageBorderColorChanged"/> event.
        ''' </summary>
        ''' <param name="e">
        '''   The <see cref="EventArgs"/> instance containing the event data.
        ''' </param>
        Protected Overridable Sub OnImageBorderColorChanged(e As EventArgs)
            Dim handler As EventHandler

            Invalidate()

            handler = CType(Events(_eventImageBorderColorChanged), EventHandler)

            handler?.Invoke(Me, e)
        End Sub

        ''' <summary>
        '''   Raises the <see cref="ImageBorderStyleChanged"/> event.
        ''' </summary>
        ''' <param name="e">
        '''   The <see cref="EventArgs"/> instance containing the event data.
        ''' </param>
        Protected Overridable Sub OnImageBorderStyleChanged(e As EventArgs)
            Dim handler As EventHandler

            Invalidate()

            handler = CType(Events(_eventImageBorderStyleChanged), EventHandler)

            handler?.Invoke(Me, e)
        End Sub

        ''' <summary>
        '''   Raises the <see cref="ImageChanged"/> event.
        ''' </summary>
        ''' <param name="e">
        '''   The <see cref="System.EventArgs"/> instance containing the event data.
        ''' </param>
        Protected Overridable Sub OnImageChanged(e As EventArgs)
            Dim handler As EventHandler

            IsAnimating = False

            If Image IsNot Nothing Then
                Try
                    IsAnimating = ImageAnimator.CanAnimate(Image)
                    If IsAnimating Then
                        Call ImageAnimator.Animate(Image, New EventHandler(AddressOf OnFrameChangedHandler))
                    End If
                Catch __unusedArgumentException1__ As ArgumentException
                    ' probably a disposed image, ignore
                End Try
            End If

            AdjustLayout()

            handler = CType(Events(_eventImageChanged), EventHandler)

            handler?.Invoke(Me, e)
        End Sub

        ''' <summary>
        '''   Raises the <see cref="InterpolationModeChanged"/> event.
        ''' </summary>
        ''' <param name="e">
        '''   The <see cref="System.EventArgs"/> instance containing the event data.
        ''' </param>
        Protected Overridable Sub OnInterpolationModeChanged(e As EventArgs)
            Dim handler As EventHandler

            Invalidate()

            handler = CType(Events(_eventInterpolationModeChanged), EventHandler)

            handler?.Invoke(Me, e)
        End Sub

        ''' <summary>
        '''   Raises the <see cref="InvertMouseChanged"/> event.
        ''' </summary>
        ''' <param name="e">
        '''   The <see cref="System.EventArgs"/> instance containing the event data.
        ''' </param>
        Protected Overridable Sub OnInvertMouseChanged(e As EventArgs)
            Dim handler As EventHandler

            handler = CType(Events(_eventInvertMouseChanged), EventHandler)

            handler?.Invoke(Me, e)
        End Sub

        ''' <summary>
        '''   Raises the <see cref="System.Windows.Forms.Control.KeyDown"/> event.
        ''' </summary>
        ''' <param name="e">
        '''   A <see cref="T:System.Windows.Forms.KeyEventArgs"/> that contains the event data.
        ''' </param>
        Protected Overrides Sub OnKeyDown(e As KeyEventArgs)
            MyBase.OnKeyDown(e)

            ProcessScrollingShortcuts(e)

            If ShortcutsEnabled AndAlso AllowZoom AndAlso SizeMode = ImageBoxSizeMode.Normal Then
                ProcessImageShortcuts(e)
            End If
        End Sub

        ''' <summary>
        '''   Raises the <see cref="LimitSelectionToImageChanged"/> event.
        ''' </summary>
        ''' <param name="e">
        '''   The <see cref="System.EventArgs"/> instance containing the event data.
        ''' </param>
        Protected Overridable Sub OnLimitSelectionToImageChanged(e As EventArgs)
            Dim handler As EventHandler

            handler = CType(Events(_eventLimitSelectionToImageChanged), EventHandler)

            handler?.Invoke(Me, e)
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

            If e.Button <> MouseButtons.None Then
                If _panStyle = ImageBoxPanStyle.Free Then
                    ' already panning, abort
                    ProcessPanEvents(ImageBoxPanStyle.None)
                Else
                    _mouseDownStart = ImageBoxNativeMethods.GetTickCount()
                    If AllowFreePan AndAlso e.Button = System.Windows.Forms.MouseButtons.Middle Then
                        ProcessPanning(e)
                    End If
                End If
            End If

            SetCursor(e.Location)
        End Sub

        ''' <summary>
        '''   Raises the <see cref="System.Windows.Forms.Control.MouseMove"/> event.
        ''' </summary>
        ''' <param name="e">
        '''   A <see cref="T:System.Windows.Forms.MouseEventArgs"/> that contains the event data.
        ''' </param>
        Protected Overrides Sub OnMouseMove(e As MouseEventArgs)
            MyBase.OnMouseMove(e)

            If e.Button <> MouseButtons.None Then
                ProcessPanning(e)
                ProcessSelection(e)
            End If

            SetCursor(e.Location)
        End Sub

        ''' <summary>
        '''   Raises the <see cref="System.Windows.Forms.Control.MouseUp"/> event.
        ''' </summary>
        ''' <param name="e">
        '''   A <see cref="T:System.Windows.Forms.MouseEventArgs"/> that contains the event data.
        ''' </param>
        Protected Overrides Sub OnMouseUp(e As MouseEventArgs)
            Dim doNotProcessClick As Boolean

            MyBase.OnMouseUp(e)

            doNotProcessClick = _panStyle <> ImageBoxPanStyle.None OrElse IsSelecting

            If _panStyle = ImageBoxPanStyle.Standard OrElse _panStyle = ImageBoxPanStyle.Free AndAlso ImageBoxNativeMethods.GetTickCount() > _mouseDownStart + SystemInformation.DoubleClickTime Then
                ProcessPanEvents(ImageBoxPanStyle.None)
            End If

            If IsSelecting Then
                EndDrag()
            End If
            WasDragCancelled = False

            If Not doNotProcessClick AndAlso AllowZoom AndAlso AllowClickZoom AndAlso _panStyle = ImageBoxPanStyle.None AndAlso SizeMode = ImageBoxSizeMode.Normal Then
                If e.Button = MouseButtons.Left AndAlso ModifierKeys = Keys.None Then
                    ProcessMouseZoom(True, e.Location)
                ElseIf e.Button = MouseButtons.Right OrElse e.Button = MouseButtons.Left AndAlso ModifierKeys <> Keys.None Then
                    ProcessMouseZoom(False, e.Location)
                End If
            End If
        End Sub

        ''' <summary>
        '''   Raises the <see cref="System.Windows.Forms.Control.MouseWheel"/> event.
        ''' </summary>
        ''' <param name="e">
        '''   A <see cref="T:System.Windows.Forms.MouseEventArgs"/> that contains the event data.
        ''' </param>
        Protected Overrides Sub OnMouseWheel(e As MouseEventArgs)
            MyBase.OnMouseWheel(e)

            If MouseWheelMode = ImageBoxMouseWheelMode.Zoom Then
                DoMouseWheelZoom(e)
            ElseIf MouseWheelMode = ImageBoxMouseWheelMode.ScrollAndZoom Then
                If ModifierKeys = Keys.Control Then
                    DoMouseWheelZoom(e)
                ElseIf VScroll AndAlso ModifierKeys = Keys.None Then
                    Dim scrollDelta = SystemInformation.MouseWheelScrollLines * VerticalScroll.SmallChange
                    MyBase.ScrollTo(HorizontalScroll.Value, VerticalScroll.Value + If(e.Delta > 0, -scrollDelta, scrollDelta))
                ElseIf HScroll AndAlso ModifierKeys = Keys.Shift Then
                    Dim scrollDelta = SystemInformation.MouseWheelScrollLines * HorizontalScroll.SmallChange
                    MyBase.ScrollTo(HorizontalScroll.Value + If(e.Delta > 0, -scrollDelta, scrollDelta), VerticalScroll.Value)
                End If
            End If
        End Sub

        ''' <summary>
        ''' Raises the <see cref="MouseWheelModeChanged"/> event.
        ''' </summary>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Protected Overridable Sub OnMouseWheelModeChanged(e As EventArgs)
            Dim handler As EventHandler

            handler = CType(Events(_eventMouseWheelModeChanged), EventHandler)

            handler?.Invoke(Me, e)
        End Sub

        ''' <summary>
        '''   Raises the <see cref="System.Windows.Forms.Control.PaddingChanged"/> event.
        ''' </summary>
        ''' <param name="e">
        '''   An <see cref="T:System.EventArgs"/> that contains the event data.
        ''' </param>
        Protected Overrides Sub OnPaddingChanged(e As EventArgs)
            MyBase.OnPaddingChanged(e)
            AdjustLayout()
        End Sub

        ''' <summary>
        '''   Raises the <see cref="System.Windows.Forms.Control.Paint"/> event.
        ''' </summary>
        ''' <param name="e">
        '''   A <see cref="T:System.Windows.Forms.PaintEventArgs"/> that contains the event data.
        ''' </param>
        Protected Overrides Sub OnPaint(e As PaintEventArgs)
            If AllowPainting Then
                ' draw the background
                DrawBackground(e)

                ' draw the image
                If Not ViewSize.IsEmpty Then
                    DrawImageBorder(e.Graphics)
                End If
                If VirtualMode Then
                    OnVirtualDraw(e)
                ElseIf Image IsNot Nothing Then
                    DrawImage(e.Graphics)
                End If

                ' draw the grid
                If ShowPixelGrid AndAlso Not VirtualMode Then
                    DrawPixelGrid(e.Graphics)
                End If

                ' draw the selection
                If SelectionRegion <> Rectangle.Empty Then
                    DrawSelection(e)
                End If

                ' text
                If Not String.IsNullOrEmpty(Text) AndAlso TextDisplayMode <> ImageBoxGridDisplayMode.None Then
                    DrawText(e)
                End If

                If _panStyle = ImageBoxPanStyle.Free Then
                    DrawPanAllSymbol(e)
                End If

                MyBase.OnPaint(e)
            End If
        End Sub

        ''' <summary>
        '''   Raises the <see cref="PanEnd"/> event.
        ''' </summary>
        ''' <param name="e">
        '''   The <see cref="System.EventArgs"/> instance containing the event data.
        ''' </param>
        Protected Overridable Sub OnPanEnd(e As EventArgs)
            Dim handler As EventHandler

            handler = CType(Events(_eventPanEnd), EventHandler)

            handler?.Invoke(Me, e)
        End Sub

        ''' <summary>
        ''' Raises the <see cref="PanModeChanged"/> event.
        ''' </summary>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Protected Overridable Sub OnPanModeChanged(e As EventArgs)
            Dim handler As EventHandler

            handler = CType(Events(_eventPanModeChanged), EventHandler)

            handler?.Invoke(Me, e)
        End Sub

        ''' <summary>
        '''   Raises the <see cref="PanStart"/> event.
        ''' </summary>
        ''' <param name="e">
        '''   The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.
        ''' </param>
        Protected Overridable Sub OnPanStart(e As CancelEventArgs)
            Dim handler As EventHandler

            handler = CType(Events(_eventPanStart), EventHandler)

            handler?.Invoke(Me, e)
        End Sub

        ''' <summary>
        '''   Raises the <see cref="System.Windows.Forms.Control.ParentChanged"/> event.
        ''' </summary>
        ''' <param name="e">
        '''   An <see cref="T:System.EventArgs"/> that contains the event data.
        ''' </param>
        Protected Overrides Sub OnParentChanged(e As EventArgs)
            MyBase.OnParentChanged(e)
            AdjustLayout()
        End Sub

        ''' <summary>
        '''   Raises the <see cref="PixelGridColorChanged"/> event.
        ''' </summary>
        ''' <param name="e">
        '''   The <see cref="EventArgs"/> instance containing the event data.
        ''' </param>
        Protected Overridable Sub OnPixelGridColorChanged(e As EventArgs)
            Dim handler As EventHandler

            Invalidate()

            handler = CType(Events(_eventPixelGridColorChanged), EventHandler)

            handler?.Invoke(Me, e)
        End Sub

        ''' <summary>
        ''' Raises the <see cref="PixelGridThresholdChanged"/> event.
        ''' </summary>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Protected Overridable Sub OnPixelGridThresholdChanged(e As EventArgs)
            Dim handler As EventHandler

            handler = CType(Events(_eventPixelGridThresholdChanged), EventHandler)

            handler?.Invoke(Me, e)
        End Sub

        ''' <summary>
        '''   Raises the <see cref="System.Windows.Forms.Control.Resize"/> event.
        ''' </summary>
        ''' <param name="e">
        '''   An <see cref="T:System.EventArgs"/> that contains the event data.
        ''' </param>
        Protected Overrides Sub OnResize(e As EventArgs)
            AdjustLayout()

            MyBase.OnResize(e)
        End Sub

        ''' <summary>
        ''' Raises the <see cref="ScaleTextChanged"/> event.
        ''' </summary>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Protected Overridable Sub OnScaleTextChanged(e As EventArgs)
            Dim handler As EventHandler

            Invalidate()

            handler = CType(Events(_eventScaleTextChanged), EventHandler)

            handler?.Invoke(Me, e)
        End Sub

        ''' <summary>
        '''   Raises the <see cref="System.Windows.Forms.ScrollableControl.Scroll"/> event.
        ''' </summary>
        ''' <param name="se">
        '''   A <see cref="T:System.Windows.Forms.ScrollEventArgs"/> that contains the event data.
        ''' </param>
        Protected Overrides Sub OnScroll(se As ScrollEventArgs)
            Invalidate()

            MyBase.OnScroll(se)
        End Sub

        ''' <summary>
        '''   Raises the <see cref="Selected"/> event.
        ''' </summary>
        ''' <param name="e">
        '''   The <see cref="System.EventArgs"/> instance containing the event data.
        ''' </param>
        Protected Overridable Sub OnSelected(e As EventArgs)
            Dim handler As EventHandler(Of EventArgs)

            Select Case SelectionMode
                Case ImageBoxSelectionMode.Zoom
                    If SelectionRegion.Width > SelectionDeadZone AndAlso SelectionRegion.Height > SelectionDeadZone Then
                        ZoomToRegion(SelectionRegion)
                        SelectNone()
                    End If
            End Select

            handler = CType(Events(_eventSelected), EventHandler(Of EventArgs))

            handler?.Invoke(Me, e)
        End Sub

        ''' <summary>
        '''   Raises the <see cref="Selecting"/> event.
        ''' </summary>
        ''' <param name="e">
        '''   The <see cref="System.EventArgs"/> instance containing the event data.
        ''' </param>
        Protected Overridable Sub OnSelecting(e As ImageBoxCancelEventArgs)
            Dim handler As EventHandler(Of ImageBoxCancelEventArgs)

            handler = CType(Events(_eventSelecting), EventHandler(Of ImageBoxCancelEventArgs))

            handler?.Invoke(Me, e)
        End Sub

        ''' <summary>
        '''   Raises the <see cref="SelectionColorChanged"/> event.
        ''' </summary>
        ''' <param name="e">
        '''   The <see cref="System.EventArgs"/> instance containing the event data.
        ''' </param>
        Protected Overridable Sub OnSelectionColorChanged(e As EventArgs)
            Dim handler As EventHandler

            handler = CType(Events(_eventSelectionColorChanged), EventHandler)

            handler?.Invoke(Me, e)
        End Sub

        ''' <summary>
        '''   Raises the <see cref="SelectionModeChanged"/> event.
        ''' </summary>
        ''' <param name="e">
        '''   The <see cref="System.EventArgs"/> instance containing the event data.
        ''' </param>
        Protected Overridable Sub OnSelectionModeChanged(e As EventArgs)
            Dim handler As EventHandler

            handler = CType(Events(_eventSelectionModeChanged), EventHandler)

            handler?.Invoke(Me, e)
        End Sub

        ''' <summary>
        '''   Raises the <see cref="SelectionRegionChanged"/> event.
        ''' </summary>
        ''' <param name="e">
        '''   The <see cref="System.EventArgs"/> instance containing the event data.
        ''' </param>
        Protected Overridable Sub OnSelectionRegionChanged(e As EventArgs)
            Dim handler As EventHandler

            Invalidate()

            handler = CType(Events(_eventSelectionRegionChanged), EventHandler)

            handler?.Invoke(Me, e)
        End Sub

        ''' <summary>
        '''   Raises the <see cref="ShortcutsEnabledChanged"/> event.
        ''' </summary>
        ''' <param name="e">
        '''   The <see cref="EventArgs"/> instance containing the event data.
        ''' </param>
        Protected Overridable Sub OnShortcutsEnabledChanged(e As EventArgs)
            Dim handler As EventHandler

            handler = CType(Events(_eventShortcutsEnabledChanged), EventHandler)

            handler?.Invoke(Me, e)
        End Sub

        ''' <summary>
        '''   Raises the <see cref="ShowPixelGridChanged"/> event.
        ''' </summary>
        ''' <param name="e">
        '''   The <see cref="EventArgs"/> instance containing the event data.
        ''' </param>
        Protected Overridable Sub OnShowPixelGridChanged(e As EventArgs)
            Dim handler As EventHandler

            Invalidate()

            handler = CType(Events(_eventShowPixelGridChanged), EventHandler)

            handler?.Invoke(Me, e)
        End Sub

        ''' <summary>
        ''' Raises the <see cref="SizeModeChanged"/> event.
        ''' </summary>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Protected Overridable Sub OnSizeModeChanged(e As EventArgs)
            Dim handler As EventHandler

            AdjustLayout()

            handler = CType(Events(_eventSizeModeChanged), EventHandler)

            handler?.Invoke(Me, e)
        End Sub

        ''' <summary>
        '''   Raises the <see cref="SizeToFitChanged"/> event.
        ''' </summary>
        ''' <param name="e">
        '''   The <see cref="System.EventArgs"/> instance containing the event data.
        ''' </param>
        Protected Overridable Sub OnSizeToFitChanged(e As EventArgs)
            Dim handler As EventHandler

            AdjustLayout()

            handler = CType(Events(_eventSizeToFitChanged), EventHandler)

            handler?.Invoke(Me, e)
        End Sub

        ''' <summary>
        ''' Raises the <see cref="TextAlignChanged"/> event.
        ''' </summary>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Protected Overridable Sub OnTextAlignChanged(e As EventArgs)
            Dim handler As EventHandler

            Invalidate()

            handler = CType(Events(_eventTextAlignChanged), EventHandler)

            handler?.Invoke(Me, e)
        End Sub

        ''' <summary>
        ''' Raises the <see cref="TextBackColorChanged"/> event.
        ''' </summary>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Protected Overridable Sub OnTextBackColorChanged(e As EventArgs)
            Dim handler As EventHandler

            Invalidate()

            handler = CType(Events(_eventTextBackColorChanged), EventHandler)

            handler?.Invoke(Me, e)
        End Sub

        ''' <summary>
        ''' Raises the <see cref="E:System.Windows.Forms.Control.TextChanged"/> event.
        ''' </summary>
        ''' <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        Protected Overrides Sub OnTextChanged(e As EventArgs)
            MyBase.OnTextChanged(e)

            Invalidate()
        End Sub

        ''' <summary>
        ''' Raises the <see cref="TextDisplayModeChanged"/> event.
        ''' </summary>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Protected Overridable Sub OnTextDisplayModeChanged(e As EventArgs)
            Dim handler As EventHandler

            Invalidate()

            handler = CType(Events(_eventTextDisplayModeChanged), EventHandler)

            handler?.Invoke(Me, e)
        End Sub

        ''' <summary>
        ''' Raises the <see cref="TextPaddingChanged"/> event.
        ''' </summary>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Protected Overridable Sub OnTextPaddingChanged(e As EventArgs)
            Dim handler As EventHandler

            handler = CType(Events(_eventTextPaddingChanged), EventHandler)

            Invalidate()

            handler?.Invoke(Me, e)
        End Sub

        ''' <summary>
        '''   Raises the <see cref="VirtualDraw"/> event.
        ''' </summary>
        ''' <param name="e">
        '''   The <see cref="PaintEventArgs"/> instance containing the event data.
        ''' </param>
        Protected Overridable Sub OnVirtualDraw(e As PaintEventArgs)
            Dim handler As PaintEventHandler

            handler = CType(Events(_eventVirtualDraw), PaintEventHandler)

            handler?.Invoke(Me, e)
        End Sub

        ''' <summary>
        '''   Raises the <see cref="VirtualModeChanged"/> event.
        ''' </summary>
        ''' <param name="e">
        '''   The <see cref="EventArgs"/> instance containing the event data.
        ''' </param>
        Protected Overridable Sub OnVirtualModeChanged(e As EventArgs)
            Dim handler As EventHandler

            AdjustLayout()

            handler = CType(Events(_eventVirtualModeChanged), EventHandler)

            handler?.Invoke(Me, e)
        End Sub

        ''' <summary>
        '''   Raises the <see cref="VirtualSizeChanged"/> event.
        ''' </summary>
        ''' <param name="e">
        '''   The <see cref="EventArgs"/> instance containing the event data.
        ''' </param>
        Protected Overridable Sub OnVirtualSizeChanged(e As EventArgs)
            Dim handler As EventHandler

            AdjustLayout()

            handler = CType(Events(_eventVirtualSizeChanged), EventHandler)

            handler?.Invoke(Me, e)
        End Sub

        ''' <summary>
        '''   Raises the <see cref="ZoomChanged"/> event.
        ''' </summary>
        ''' <param name="e">
        '''   The <see cref="System.EventArgs"/> instance containing the event data.
        ''' </param>
        Protected Overridable Sub OnZoomChanged(e As EventArgs)
            Dim handler As EventHandler

            AdjustLayout()

            handler = CType(Events(_eventZoomChanged), EventHandler)

            handler?.Invoke(Me, e)
        End Sub

        ''' <summary>
        ''' Raises the <see cref="Zoomed"/> event.
        ''' </summary>
        ''' <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        Protected Overridable Sub OnZoomed(e As ImageBoxZoomEventArgs)
            Dim handler As EventHandler(Of ImageBoxZoomEventArgs)

            handler = CType(Events(_eventZoomed), EventHandler(Of ImageBoxZoomEventArgs))

            handler?.Invoke(Me, e)
        End Sub

        ''' <summary>
        '''   Raises the <see cref="ZoomLevelsChanged"/> event.
        ''' </summary>
        ''' <param name="e">
        '''   The <see cref="EventArgs"/> instance containing the event data.
        ''' </param>
        Protected Overridable Sub OnZoomLevelsChanged(e As EventArgs)
            Dim handler As EventHandler

            handler = CType(Events(_eventZoomLevelsChanged), EventHandler)

            handler?.Invoke(Me, e)
        End Sub

        ''' <summary>
        '''   Processes shortcut keys for zooming and selection
        ''' </summary>
        ''' <param name="e">
        '''   The <see cref="KeyEventArgs"/> instance containing the event data.
        ''' </param>
        Protected Overridable Sub ProcessImageShortcuts(e As KeyEventArgs)
            Dim currentPixel As System.Drawing.Point
            Dim currentZoom As Integer
            Dim relativePoint As System.Drawing.Point

            relativePoint = CenterPoint
            currentPixel = PointToImage(relativePoint)
            currentZoom = Zoom

            Select Case e.KeyCode
                Case Keys.Home
                    If AllowZoom Then
                        PerformActualSize(ImageBoxActionSources.User)
                    End If

                Case Keys.PageDown, Keys.Oemplus
                    If AllowZoom Then
                        PerformZoomIn(ImageBoxActionSources.User, True)
                    End If

                Case Keys.PageUp, Keys.OemMinus
                    If AllowZoom Then
                        PerformZoomOut(ImageBoxActionSources.User, True)
                    End If
            End Select

            If Zoom <> currentZoom Then
                ScrollTo(currentPixel, relativePoint)
            End If
        End Sub

        ''' <summary>
        '''   Processes zooming with the mouse. Attempts to keep the pre-zoom image pixel under the mouse after the zoom has completed.
        ''' </summary>
        ''' <param name="isZoomIn">
        '''   if set to <c>true</c> zoom in, otherwise zoom out.
        ''' </param>
        ''' <param name="cursorPosition">The cursor position.</param>
        Protected Overridable Sub ProcessMouseZoom(isZoomIn As Boolean, cursorPosition As System.Drawing.Point)
            PerformZoom(If(isZoomIn, ImageBoxZoomActions.ZoomIn, ImageBoxZoomActions.ZoomOut), ImageBoxActionSources.User, True, cursorPosition)
        End Sub

        ''' <summary>
        '''   Performs mouse based panning
        ''' </summary>
        ''' <param name="e">
        '''   The <see cref="MouseEventArgs"/> instance containing the event data.
        ''' </param>
        Protected Overridable Sub ProcessPanning(e As MouseEventArgs)
            If CanPan(e.Button) Then
                If _panStyle = ImageBoxPanStyle.None AndAlso (HScroll OrElse VScroll) Then
                    _startMousePosition = e.Location

                    ProcessPanEvents(If(e.Button = MouseButtons.Middle AndAlso _allowFreePan, ImageBoxPanStyle.Free, ImageBoxPanStyle.Standard))
                End If
            End If

            If _panStyle = ImageBoxPanStyle.Standard Then
                Dim x As Integer
                Dim y As Integer
                Dim position As System.Drawing.Point

                If Not InvertMouse Then
                    x = -_startScrollPosition.X + (_startMousePosition.X - e.Location.X)
                    y = -_startScrollPosition.Y + (_startMousePosition.Y - e.Location.Y)
                Else
                    x = -(_startScrollPosition.X + (_startMousePosition.X - e.Location.X))
                    y = -(_startScrollPosition.Y + (_startMousePosition.Y - e.Location.Y))
                End If

                position = New System.Drawing.Point(x, y)

                UpdateScrollPosition(position)
            End If
        End Sub

        ''' <summary>
        '''   Processes shortcut keys for scrolling
        ''' </summary>
        ''' <param name="e">
        '''   The <see cref="KeyEventArgs"/> instance containing the event data.
        ''' </param>
        Protected Overridable Sub ProcessScrollingShortcuts(e As KeyEventArgs)
            Select Case e.KeyCode
                Case Keys.Left
                    AdjustScroll(-If(e.Modifiers = Keys.None, HorizontalScroll.SmallChange, HorizontalScroll.LargeChange), 0)

                Case Keys.Right
                    AdjustScroll(If(e.Modifiers = Keys.None, HorizontalScroll.SmallChange, HorizontalScroll.LargeChange), 0)

                Case Keys.Up
                    AdjustScroll(0, -If(e.Modifiers = Keys.None, VerticalScroll.SmallChange, VerticalScroll.LargeChange))

                Case Keys.Down
                    AdjustScroll(0, If(e.Modifiers = Keys.None, VerticalScroll.SmallChange, VerticalScroll.LargeChange))
            End Select
        End Sub

        ''' <summary>
        '''   Performs mouse based region selection
        ''' </summary>
        ''' <param name="e">
        '''   The <see cref="MouseEventArgs"/> instance containing the event data.
        ''' </param>
        Protected Overridable Sub ProcessSelection(e As MouseEventArgs)
            If SelectionMode <> ImageBoxSelectionMode.None AndAlso e.Button = MouseButtons.Left AndAlso Not WasDragCancelled Then
                If Not IsSelecting Then
                    StartDrag(e)
                End If

                If IsSelecting Then
                    Dim x As Single
                    Dim y As Single
                    Dim w As Single
                    Dim h As Single
                    Dim imageOffset As System.Drawing.Point
                    Dim selection As RectangleF

                    imageOffset = GetImageViewPort().Location

                    If e.X < _startMousePosition.X Then
                        x = e.X
                        w = _startMousePosition.X - e.X
                    Else
                        x = _startMousePosition.X
                        w = e.X - _startMousePosition.X
                    End If

                    If e.Y < _startMousePosition.Y Then
                        y = e.Y
                        h = _startMousePosition.Y - e.Y
                    Else
                        y = _startMousePosition.Y
                        h = e.Y - _startMousePosition.Y
                    End If

                    x = x - imageOffset.X - AutoScrollPosition.X
                    y = y - imageOffset.Y - AutoScrollPosition.Y

                    x = x / CSng(ZoomFactor)
                    y = y / CSng(ZoomFactor)
                    w = w / CSng(ZoomFactor)
                    h = h / CSng(ZoomFactor)

                    If w <> 0 AndAlso h <> 0 Then
                        selection = New RectangleF(x, y, w, h)
                        If LimitSelectionToImage Then
                            selection = FitRectangle(selection)
                        End If

                        SelectionRegion = selection
                    End If
                End If
            End If
        End Sub

        ''' <summary>
        ''' Resets the <see cref="SizeMode"/> property whilsts retaining the original <see cref="Zoom"/>.
        ''' </summary>
        Protected Sub RestoreSizeMode()
            If SizeMode <> ImageBoxSizeMode.Normal Then
                Dim previousZoom As Integer

                previousZoom = Zoom
                SizeMode = ImageBoxSizeMode.Normal
                Zoom = previousZoom ' Stop the zoom getting reset to 100% before calculating the new zoom
            End If
        End Sub

        ''' <summary>
        ''' Initializes a selection or drag operation.
        ''' </summary>
        ''' <param name="e">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
        Protected Overridable Sub StartDrag(e As MouseEventArgs)
            Dim args As ImageBoxCancelEventArgs

            args = New ImageBoxCancelEventArgs(e.Location)

            OnSelecting(args)

            WasDragCancelled = args.Cancel
            IsSelecting = Not args.Cancel
            If IsSelecting Then
                SelectNone()

                _startMousePosition = e.Location
            End If
        End Sub

        ''' <summary>
        '''   Updates the scroll position.
        ''' </summary>
        ''' <param name="position">The position.</param>
        Protected Overridable Sub UpdateScrollPosition(position As System.Drawing.Point)
            AutoScrollPosition = position
            Invalidate()
            OnScroll(New ScrollEventArgs(ScrollEventType.EndScroll, 0))
        End Sub

        Private Function CanPan(button As MouseButtons) As Boolean
            Return (HScroll OrElse VScroll) AndAlso (_panMode And CType(button, ImageBoxPanMode)) <> 0 AndAlso Not ViewSize.IsEmpty AndAlso (_selectionMode = ImageBoxSelectionMode.None OrElse button <> MouseButtons.Left)
        End Function

        Private Sub CreateTimer()
            _freePanTimer = New Timer With {
        .Enabled = True,
        .Interval = _freePanTimerInterval
      }

            AddHandler _freePanTimer.Tick, AddressOf FreePanTimerTickHandler

            _freePanTimer.Start()
        End Sub

        Private Sub DoMouseWheelZoom(e As MouseEventArgs)
            If AllowZoom AndAlso SizeMode = ImageBoxSizeMode.Normal Then
                ' The MouseWheel event can contain multiple "spins" of the wheel so we need to adjust accordingly
                Dim spins As Integer = CInt(Math.Abs(e.Delta / SystemInformation.MouseWheelScrollDelta))

                ' TODO: Really should update the source method to handle multiple increments rather than calling it multiple times
                For i = 0 To spins - 1
                    ProcessMouseZoom(e.Delta > 0, e.Location)
                Next
            End If
        End Sub

        Private Sub DrawPanAllSymbol(e As PaintEventArgs)
            Dim g As Graphics
            Dim x As Integer
            Dim y As Integer

            g = e.Graphics

            x = _startMousePosition.X - (_panAllSymbol.Width >> 1)
            y = _startMousePosition.Y - (_panAllSymbol.Height >> 1)

            g.DrawImage(_panAllSymbol, x, y)
        End Sub

        Private Sub FreePanTimerTickHandler(sender As Object, e As EventArgs)
            Dim location As System.Drawing.Point
            Dim direction As ImageBoxPanDirection
            Dim distance As Integer
            Dim ox As Integer
            Dim oy As Integer

            location = PointToClient(MousePosition)
            direction = GetPanDirection(location)
            distance = GetDistance(_startMousePosition.X, _startMousePosition.Y, location.X, location.Y)

            ox = 0
            oy = 0

            Select Case direction
                Case ImageBoxPanDirection.Up
                    oy = -distance
                Case ImageBoxPanDirection.Down
                    oy = +distance
                Case ImageBoxPanDirection.Left
                    ox = -distance
                Case ImageBoxPanDirection.Right
                    ox = +distance
            End Select

            If ox <> 0 OrElse oy <> 0 Then
                AdjustScroll(ox, oy)
            End If
        End Sub

        ''' <summary>
        ''' Gets the distance between two points.
        ''' </summary>
        ''' <param name="x1">The first x value.</param>
        ''' <param name="y1">The first y value.</param>
        ''' <param name="x2">The second x value.</param>
        ''' <param name="y2">The second y value.</param>
        ''' <returns>
        ''' The distance.
        ''' </returns>
        Private Function GetDistance(x1 As Integer, y1 As Integer, x2 As Integer, y2 As Integer) As Integer
            Dim dx As Integer
            Dim dy As Integer
            Dim distance As Double

            dx = x2 - x1
            dy = y2 - y1
            distance = Math.Sqrt(dx * dx + dy * dy)

            Return Convert.ToInt32(distance)
        End Function

        ''' <summary>
        ''' Gets the distance between two values.
        ''' </summary>
        ''' <param name="x1">The first value.</param>
        ''' <param name="x2">The second value.</param>
        ''' <returns>
        ''' The distance.
        ''' </returns>
        Private Function GetDistance(x1 As Integer, x2 As Integer) As Integer
            Dim dx As Integer
            Dim distance As Double

            dx = x2 - x1
            distance = Math.Sqrt(dx * dx)

            Return Convert.ToInt32(distance)
        End Function

        ''' <summary>
        ''' Gets the System.Drawing.Size of the image.
        ''' </summary>
        ''' <remarks>If an error occurs, for example due to the image being disposed, an empty System.Drawing.Size is returned</remarks>
        ''' <returns>System.Drawing.Size.</returns>
        Private Function GetImageSize() As System.Drawing.Size
            Dim result As System.Drawing.Size
            ' HACK: This whole thing stinks. Hey MS, how about an IsDisposed property for images?
            If Image IsNot Nothing Then
                Try
                    result = Image.Size
                Catch
                    result = System.Drawing.Size.Empty
                End Try
            Else
                result = System.Drawing.Size.Empty
            End If

            Return result
        End Function

        Private Function GetPanDirection(location As System.Drawing.Point) As ImageBoxPanDirection
            Dim result As ImageBoxPanDirection
            Dim x As Integer
            Dim y As Integer

            x = location.X - _startMousePosition.X
            y = location.Y - _startMousePosition.Y

            If x >= -_panAllDeadSize AndAlso x <= _panAllDeadSize AndAlso y >= -_panAllDeadSize AndAlso y <= _panAllDeadSize Then
                result = ImageBoxPanDirection.None
            Else
                Dim distanceX As Integer

                distanceX = location.X - _startMousePosition.X

                If -y > Math.Abs(distanceX) Then
                    result = ImageBoxPanDirection.Up
                ElseIf y > Math.Abs(distanceX) Then
                    result = ImageBoxPanDirection.Down
                Else
                    result = If(distanceX < 0, ImageBoxPanDirection.Left, ImageBoxPanDirection.Right)
                End If
            End If

            Return result
        End Function

        ''' <summary>
        ''' Returns an appropriate zoom level based on the specified action, relative to the current zoom level.
        ''' </summary>
        ''' <param name="action">The action to determine the zoom level.</param>
        ''' <exception cref="System.ArgumentOutOfRangeException">Thrown if an unsupported action is specified.</exception>
        Private Function GetZoomLevel(action As ImageBoxZoomActions) As Integer
            Dim result As Integer

            Select Case action
                Case ImageBoxZoomActions.None
                    result = Zoom
                Case ImageBoxZoomActions.ZoomIn
                    result = ZoomLevels.NextZoom(Zoom)
                Case ImageBoxZoomActions.ZoomOut
                    result = ZoomLevels.PreviousZoom(Zoom)
                Case ImageBoxZoomActions.ActualSize
                    result = 100
                Case Else
                    Throw New ArgumentOutOfRangeException(NameOf(action))
            End Select

            Return result
        End Function

        ''' <summary>
        '''   Initializes the grid tile.
        ''' </summary>
        Private Sub InitializeGridTile()
            _texture?.Dispose()
            _gridTile?.Dispose()

            If GridDisplayMode <> ImageBoxGridDisplayMode.None AndAlso GridCellSize <> 0 Then
                If GridScale <> ImageBoxGridScale.None Then
                    _gridTile = CreateGridTileImage(GridCellSize, GridColor, GridColorAlternate)
                    _texture = New TextureBrush(_gridTile)
                Else
                    _texture = New SolidBrush(GridColor)
                End If
            End If

            Invalidate()
        End Sub

        Private Sub KillTimer()
            If _freePanTimer IsNot Nothing Then
                _freePanTimer.Stop()
                RemoveHandler _freePanTimer.Tick, AddressOf FreePanTimerTickHandler
                _freePanTimer.Dispose()
                _freePanTimer = Nothing
            End If
        End Sub

        ''' <summary>
        ''' Called when the animation frame changes.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="eventArgs">The <see cref="EventArgs"/> instance containing the event data.</param>
        Private Sub OnFrameChangedHandler(sender As Object, eventArgs As EventArgs)
            Invalidate()
        End Sub

        ''' <summary>
        ''' Resets the zoom to 100%.
        ''' </summary>
        ''' <param name="source">The source that initiated the action.</param>
        Private Sub PerformActualSize(source As ImageBoxActionSources)
            SizeMode = ImageBoxSizeMode.Normal
            SetZoom(100, ImageBoxZoomActions.ActualSize Or If(Zoom < 100, ImageBoxZoomActions.ZoomIn, ImageBoxZoomActions.ZoomOut), source)
        End Sub

        ''' <summary>
        ''' Performs a zoom action.
        ''' </summary>
        ''' <param name="action">The action to perform.</param>
        ''' <param name="source">The source that initiated the action.</param>
        ''' <param name="preservePosition"><c>true</c> if the current scrolling position should be preserved relative to the new zoom level, <c>false</c> to reset.</param>
        Private Sub PerformZoom(action As ImageBoxZoomActions, source As ImageBoxActionSources, preservePosition As Boolean)
            PerformZoom(action, source, preservePosition, CenterPoint)
        End Sub

        ''' <summary>
        ''' Performs a zoom action.
        ''' </summary>
        ''' <param name="action">The action to perform.</param>
        ''' <param name="source">The source that initiated the action.</param>
        ''' <param name="preservePosition"><c>true</c> if the current scrolling position should be preserved relative to the new zoom level, <c>false</c> to reset.</param>
        ''' <param name="relativePoint">A <see cref="System.Drawing.Point"/> describing the current center of the control.</param>
        Private Sub PerformZoom(action As ImageBoxZoomActions, source As ImageBoxActionSources, preservePosition As Boolean, relativePoint As System.Drawing.Point)
            Dim currentPixel As System.Drawing.Point
            Dim currentZoom As Integer
            Dim newZoom As Integer

            currentPixel = PointToImage(relativePoint)
            currentZoom = Zoom
            newZoom = GetZoomLevel(action)

            RestoreSizeMode()
            SetZoom(newZoom, action, source)

            If preservePosition AndAlso Zoom <> currentZoom Then
                ScrollTo(currentPixel, relativePoint)
            End If
        End Sub

        ''' <summary>
        ''' Zooms into the image
        ''' </summary>
        ''' <param name="source">The source that initiated the action.</param>
        ''' <param name="preservePosition"><c>true</c> if the current scrolling position should be preserved relative to the new zoom level, <c>false</c> to reset.</param>
        Private Sub PerformZoomIn(source As ImageBoxActionSources, preservePosition As Boolean)
            PerformZoom(ImageBoxZoomActions.ZoomIn, source, preservePosition)
        End Sub

        ''' <summary>
        ''' Zooms out of the image
        ''' </summary>
        ''' <param name="source">The source that initiated the action.</param>
        ''' <param name="preservePosition"><c>true</c> if the current scrolling position should be preserved relative to the new zoom level, <c>false</c> to reset.</param>
        Private Sub PerformZoomOut(source As ImageBoxActionSources, preservePosition As Boolean)
            PerformZoom(ImageBoxZoomActions.ZoomOut, source, preservePosition)
        End Sub

        ''' <summary>
        ''' Raises either the PanStart or PanEnd events
        ''' </summary>
        ''' <param name="panStyle">The new pan style.</param>
        Private Sub ProcessPanEvents(panStyle As ImageBoxPanStyle)
            If _panStyle <> panStyle Then
                KillTimer()

                If panStyle = ImageBoxPanStyle.None Then
                    _panStyle = ImageBoxPanStyle.None
                    Invalidate()
                    OnPanEnd(EventArgs.Empty)
                Else
                    Dim args As CancelEventArgs

                    args = New CancelEventArgs()

                    OnPanStart(args)

                    If Not args.Cancel Then
                        _panStyle = panStyle

                        If panStyle = ImageBoxPanStyle.Free Then
                            Call LoadPanResources()

                            CreateTimer()
                        End If

                        _startScrollPosition = AutoScrollPosition
                    End If

                    Invalidate()
                End If
            End If
        End Sub

        ''' <summary>
        ''' Sets the mouse cursor based on the current control state
        ''' </summary>
        ''' <param name="location">The location of the mouse in client co-ordinates.</param>
        Private Sub SetCursor(location As System.Drawing.Point)
            Dim cursor As Cursor

            cursor = GetCursor(location)

            If _currentCursor IsNot cursor Then
                _currentCursor = cursor

                ' have to use this.Cursor and not Cursor.Current
                ' otherwise the cursor gets reset back to Default
                ' after clicking to initiate Free Pan
                ' As a result, the Cursor property has been hidden
                ' to discourage users setting it manually
                Me.Cursor = cursor
            End If
        End Sub

        ''' <summary>
        ''' Updates the current zoom.
        ''' </summary>
        ''' <param name="value">The new zoom value.</param>
        ''' <param name="actions">The zoom actions that caused the value to be updated.</param>
        ''' <param name="source">The source of the zoom operation.</param>
        Private Sub SetZoom(value As Integer, actions As ImageBoxZoomActions, source As ImageBoxActionSources)
            Dim previousZoom As Integer

            previousZoom = Zoom

            If value < MinZoom Then
                value = MinZoom
            ElseIf value > MaxZoom Then
                value = MaxZoom
            End If

            If _zoom <> value Then
                _zoom = value

                OnZoomChanged(EventArgs.Empty)

                OnZoomed(New ImageBoxZoomEventArgs(actions, source, previousZoom, Zoom))
            End If
        End Sub

#End Region

#Region "Other"

        Private Shared _panAllCursor As Cursor

        Private Shared _panAllSymbol As Bitmap

#End Region
    End Class
End Namespace
