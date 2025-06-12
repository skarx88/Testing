Imports System.IO
Imports System.Runtime.Serialization
Imports devDept.Eyeshot
Imports Zuken.E3.HarnessAnalyzer.Settings

Namespace D3D.Consolidated.Designs

    Partial Public Class ConsolidatedDesign

        Private _viewPortSizeSettings As New ViewPortSizeSettings

        Property ViewportSizingEnabled As Boolean = True

        Private Sub ProcessResizeOnMove(e As MouseEventArgs)
            If Not _isViewportMoving Then
                If ViewportSizingEnabled Then
                    _mouseGapState = MouseSizeMoveViewportState.None
                    _inBetweenMovingViewports = Nothing

                    Dim mouseVp As Integer = GetViewportUnderMouse(e.Location)
                    If mouseVp <> -1 Then
                        Me.Cursor = Me.DefaultCursor
                    Else
                        _vpMoving_CurrUp = GetNextVpUp(e.Location)
                        _vpMoving_CurrDown = GetNextVpDown(e.Location)
                        _vpMoving_CurrLeft = GetNextVpLeft(e.Location)
                        _vpMoving_CurrRight = GetNextVpRight(e.Location)

                        If _vpMoving_CurrUp IsNot Nothing AndAlso _vpMoving_CurrDown IsNot Nothing Then
                            Me.Cursor = Cursors.SizeNS
                            _mouseGapState = MouseSizeMoveViewportState.UpDown
                        ElseIf _vpMoving_CurrLeft IsNot Nothing AndAlso _vpMoving_CurrRight IsNot Nothing Then
                            Me.Cursor = Cursors.SizeWE
                            _mouseGapState = MouseSizeMoveViewportState.LeftRight
                        End If
                    End If
                End If
            ElseIf e.Button.HasFlag(System.Windows.Forms.MouseButtons.Left) Then
                If Me.DisplayRectangle.Contains(e.Location) Then
                    Select Case _mouseGapState
                        Case MouseSizeMoveViewportState.LeftRight
                            Dim vpLeftI As ViewportInfo = _inBetweenMovingViewports.Item1
                            Dim vpRightI As ViewportInfo = _inBetweenMovingViewports.Item2

                            Dim diff As Integer = _viewportMovingStartPos.X - e.Location.X
                            If diff <> 0 Then
                                With vpLeftI.OrgSize
                                    If vpLeftI.Viewport IsNot Nothing Then
                                        vpLeftI.Viewport.Size = New Size(.Width - diff, .Height)
                                    End If
                                End With

                                With vpRightI
                                    If .Viewport IsNot Nothing Then
                                        .Viewport.Size = New Size(.OrgSize.Width + diff, .OrgSize.Height)
                                        .Viewport.Location = New Point(.OrgLocation.X - diff, .Viewport.Location.Y)
                                    End If
                                End With

                                For Each vpAddi As ViewportBaseInfo In vpLeftI.VerticalVps
                                    If vpAddi.Viewport IsNot Nothing Then
                                        vpAddi.Viewport.Size = New Size(vpAddi.OrgSize.Width - diff, vpAddi.OrgSize.Height)
                                    End If
                                Next

                                For Each vpAddi As ViewportBaseInfo In vpRightI.VerticalVps
                                    If vpAddi.Viewport IsNot Nothing Then
                                        vpAddi.Viewport.Size = New Size(vpAddi.OrgSize.Width + diff, vpAddi.OrgSize.Height)
                                        vpAddi.Viewport.Location = New Point(vpAddi.OrgLocation.X - diff, vpAddi.Viewport.Location.Y)
                                    End If
                                Next
                            End If
                        Case MouseSizeMoveViewportState.UpDown
                            Dim vpUpI As ViewportInfo = _inBetweenMovingViewports.Item1
                            Dim vpDownI As ViewportInfo = _inBetweenMovingViewports.Item2

                            Dim diff As Integer = _viewportMovingStartPos.Y - e.Location.Y
                            If diff <> 0 Then
                                With vpUpI.OrgSize
                                    If vpUpI.Viewport IsNot Nothing Then
                                        vpUpI.Viewport.Size = New Size(.Width, .Height - diff)
                                    End If
                                End With

                                With vpDownI
                                    If .Viewport IsNot Nothing Then
                                        .Viewport.Size = New Size(.OrgSize.Width, .OrgSize.Height + diff)
                                        .Viewport.Location = New Point(.OrgLocation.X, .OrgLocation.Y - diff)
                                    End If
                                End With

                                For Each vpAddi As ViewportBaseInfo In vpUpI.HorizontalVps
                                    If vpAddi.Viewport IsNot Nothing Then
                                        vpAddi.Viewport.Size = New Size(vpAddi.OrgSize.Width, vpAddi.OrgSize.Height - diff)
                                    End If
                                Next

                                For Each vpAddi As ViewportBaseInfo In vpDownI.HorizontalVps
                                    If vpAddi.Viewport IsNot Nothing Then
                                        vpAddi.Viewport.Size = New Size(vpAddi.OrgSize.Width, vpAddi.OrgSize.Height + diff)
                                        vpAddi.Viewport.Location = New Point(vpAddi.OrgLocation.X, vpAddi.OrgLocation.Y - diff)
                                    End If
                                Next
                            End If
                    End Select

                    Me.Invalidate()
                End If
            Else
                EndViewportMoving()
            End If

        End Sub

        Private Sub EndViewportMoving()
            _isViewportMoving = False
            _vpMoving_CurrLeft = Nothing
            _vpMoving_CurrRight = Nothing
            _vpMoving_CurrUp = Nothing
            _vpMoving_CurrDown = Nothing
            _inBetweenMovingViewports = Nothing
        End Sub

        Private Sub StartViewportMoving()
            If Me.Visible Then
                Select Case _mouseGapState
                    Case MouseSizeMoveViewportState.LeftRight, MouseSizeMoveViewportState.UpDown
                        _isViewportMoving = True
                        Select Case _mouseGapState
                            Case MouseSizeMoveViewportState.LeftRight
                                _inBetweenMovingViewports = New Tuple(Of ViewportInfo, ViewportInfo)(New ViewportInfo(Me, _vpMoving_CurrLeft), New ViewportInfo(Me, _vpMoving_CurrRight))
                            Case MouseSizeMoveViewportState.UpDown
                                _inBetweenMovingViewports = New Tuple(Of ViewportInfo, ViewportInfo)(New ViewportInfo(Me, _vpMoving_CurrUp), New ViewportInfo(Me, _vpMoving_CurrDown))
                        End Select
                End Select
            Else
                _isViewportMoving = False
            End If
        End Sub

        Public Sub SaveViewportSizeSettings(filePath As String)
            Dim settings As ViewPortSizeSettings = CreateViewportSizeSettings()

            Using fs As New FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None)
                settings.SaveTo(fs)
            End Using
        End Sub

        Public Function LoadViewportSettings(filePath As String) As Boolean
            Using fs As New FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None)
                If fs.Length > 0 Then
                    If XmlFile.IsXml(fs) Then
                        fs.Position = 0
                        _viewPortSizeSettings = ViewPortSizeSettings.Load(fs)
                    ElseIf BinaryFile.IsBinFormatted(fs) Then
                        fs.Position = 0
                        Dim oldSettings As Compatibility.ViewPortSizeSettingCollection_2023 = Compatibility.ViewPortSizeSettingCollection_2023.Load(fs)
                        _viewPortSizeSettings = New ViewPortSizeSettings()
                        For Each oldVp As Compatibility.ViewportSizeSetting_2023 In oldSettings.GetSettings
                            _viewPortSizeSettings.Add(New ViewportSizeSetting(oldVp.Index, oldVp.Location, oldVp.SizePercent))
                        Next
                    Else
                        Throw New FileFormatException("Can't load viewport size settings: Invalid file format for deserializing!")
                    End If
                End If
            End Using
            Return CopyViewportSettingsToViewports()
        End Function

        Public Function CreateViewportSizeSettings() As ViewPortSizeSettings
            If _viewPortSizeSettings Is Nothing Then
                _viewPortSizeSettings = New ViewPortSizeSettings
            End If

            _viewPortSizeSettings.Clear()
            Dim i As Integer = -1
            For Each vp As Viewport In Me.Viewports
                i += 1
                _viewPortSizeSettings.AddNew(i, vp.Location, vp.Size)
            Next
            Return _viewPortSizeSettings
        End Function

        Public Function CopyViewportSettingsToViewports() As Boolean
            If Me.Viewports.Count > 0 Then
                _viewPortSizeSettings.CalculateForEach(Me.Viewports.Count, Me.GetBounds,
                                                       Sub(info)
                                                           With Me.Viewports(info.Index)
                                                               .Size = info.CalculatedViewportSize
                                                               .Location = info.CalculatedViewportLocation
                                                           End With
                                                       End Sub)
                EnsureViewportGaps()
                Return True
            End If
            Return False
        End Function

        Private Sub EnsureViewportGaps()
            Dim dicX As Dictionary(Of Integer, List(Of Viewport)) = Me.Viewports.Cast(Of Viewport).GroupBy(Function(vp) vp.Location.X).ToDictionary(Function(grp) grp.Key, Function(grp) grp.ToList)
            Dim dicY As Dictionary(Of Integer, List(Of Viewport)) = Me.Viewports.Cast(Of Viewport).GroupBy(Function(vp) vp.Location.Y).ToDictionary(Function(grp) grp.Key, Function(grp) grp.ToList)

            Dim prevVp As Viewport = Nothing
            For Each vpXGrp As KeyValuePair(Of Integer, List(Of Viewport)) In dicX
                prevVp = Nothing
                For Each vpX As Viewport In vpXGrp.Value.OrderBy(Function(vp) vp.Location.Y)
                    If prevVp IsNot Nothing Then
                        Dim gapY As Integer = Me.Size.Height - (prevVp.Size.Height) - (vpX.Size.Height) - (Me.ViewportsGap)
                        If gapY <> 0 Then
                            Dim gapDiff As Integer = Me.ViewportsGap - gapY
                            vpX.Location = New Point(vpX.Location.X, vpX.Location.Y + gapDiff + CInt(gapDiff * 0.1))
                            vpX.Size = New Size(vpX.Size.Width, vpX.Size.Height - gapDiff - CInt(gapDiff * 0.1))
                        End If
                    End If
                    prevVp = vpX
                Next
            Next

            For Each vpYGrp As KeyValuePair(Of Integer, List(Of Viewport)) In dicY ' horizontal
                prevVp = Nothing
                For Each vpY As Viewport In vpYGrp.Value.OrderBy(Function(vp) vp.Location.X)
                    If prevVp IsNot Nothing Then
                        Dim gapX As Integer = Me.Size.Width - (prevVp.Size.Width) - (vpY.Size.Width) - (Me.ViewportsGap)
                        If gapX <> 0 Then
                            Dim gapDiff As Integer = Me.ViewportsGap - gapX
                            vpY.Location = New Point(vpY.Location.X + gapDiff + CInt(gapDiff * 0.1), vpY.Location.Y)
                            vpY.Size = New Size(vpY.Size.Width - gapDiff - CInt(gapDiff * 0.1), vpY.Size.Height)
                        End If
                    End If
                    prevVp = vpY
                Next
            Next
        End Sub

        Private Function GetNextVpLeft(pt As System.Drawing.Point) As Viewport
            Return Me.Viewports.Cast(Of Viewport).Where(Function(vp) vp.Location.X < pt.X AndAlso pt.Y >= vp.Location.Y AndAlso pt.Y <= vp.Location.Y + vp.Size.Height).FirstOrDefault
        End Function

        Private Function GetNextVpRight(pt As System.Drawing.Point) As Viewport
            Return Me.Viewports.Cast(Of Viewport).Where(Function(vp) vp.Location.X > pt.X AndAlso pt.Y >= vp.Location.Y AndAlso pt.Y <= vp.Location.Y + vp.Size.Height).FirstOrDefault
        End Function

        Private Function GetNextVpUp(pt As System.Drawing.Point) As Viewport
            Return Me.Viewports.Cast(Of Viewport).Where(Function(vp) vp.Location.Y < pt.Y AndAlso pt.X >= vp.Location.X AndAlso pt.X <= vp.Location.X + vp.Size.Width).FirstOrDefault
        End Function

        Private Function GetNextVpDown(pt As System.Drawing.Point) As Viewport
            Return Me.Viewports.Cast(Of Viewport).Where(Function(vp) vp.Location.Y > pt.Y AndAlso pt.X >= vp.Location.X AndAlso pt.X <= vp.Location.X + vp.Size.Width).FirstOrDefault
        End Function

        Private Function GetViewport(pos As System.Drawing.Point) As Viewport
            For Each vp As Viewport In Me.Viewports
                If vp.Contains(pos) Then
                    Return vp
                End If
            Next
            Return Nothing
        End Function

        Private Class ViewportBaseInfo
            Private _vp As Viewport
            Private _orgSize As Size
            Private _orgLoc As Point

            Public Sub New(vp As Viewport)
                _vp = vp
                StoreVpInfos()
            End Sub

            Overridable Property Viewport As Viewport
                Get
                    Return _vp
                End Get
                Set(value As Viewport)
                    If Not _vp Is value Then
                        _vp = value
                        StoreVpInfos()
                    End If
                End Set
            End Property

            Private Sub StoreVpInfos()
                If _vp IsNot Nothing Then
                    With _vp
                        _orgSize = New Size(.Size.Width, .Size.Height)
                        _orgLoc = New Point(.Location.X, .Location.Y)
                    End With
                Else
                    _orgSize = Size.Empty
                    _orgLoc = Point.Empty

                End If
            End Sub

            ReadOnly Property OrgSize As Size
                Get
                    Return _orgSize
                End Get
            End Property

            ReadOnly Property OrgLocation As Point
                Get
                    Return _orgLoc
                End Get
            End Property

        End Class

        Private Class ViewportInfo
            Inherits ViewportBaseInfo

            Private _verticalVps As New List(Of ViewportBaseInfo)
            Private _horizontalVps As New List(Of ViewportBaseInfo)
            Private _owner As devDept.Eyeshot.Design

            Public Sub New()
                Me.New(Nothing)
            End Sub

            Private Sub New(owner As devDept.Eyeshot.Design)
                Me.New(owner, Nothing)
            End Sub

            Public Sub New(owner As devDept.Eyeshot.Design, vp As Viewport)
                MyBase.New(vp)
                _owner = owner
                StoreVertHorzInfos()
            End Sub

            ReadOnly Property VerticalVps As List(Of ViewportBaseInfo)
                Get
                    Return _verticalVps
                End Get
            End Property

            ReadOnly Property HorizontalVps As List(Of ViewportBaseInfo)
                Get
                    Return _horizontalVps
                End Get
            End Property

            Public Overrides Property Viewport As Viewport
                Get
                    Return MyBase.Viewport
                End Get
                Set(value As Viewport)
                    MyBase.Viewport = value
                    StoreVertHorzInfos()
                End Set
            End Property

            Private Function GetAdditionalViewportsVertically(vp As Viewport) As List(Of ViewportBaseInfo)
                Dim list As New List(Of ViewportBaseInfo)
                Dim i As Integer = -1
                For Each vpf As Viewport In _owner.Viewports
                    i += 1
                    If vp.Location.X = vpf.Location.X AndAlso Not vpf Is vp Then
                        list.Add(New ViewportBaseInfo(vpf))
                    End If
                Next
                Return list
            End Function


            Private Function GetAdditionalViewportsHorizontal(vp As Viewport) As List(Of ViewportBaseInfo)
                Dim list As New List(Of ViewportBaseInfo)
                Dim i As Integer = -1
                For Each vpf As Viewport In _owner.Viewports
                    i += 1
                    If vp.Location.Y = vpf.Location.Y AndAlso Not vpf Is vp Then
                        list.Add(New ViewportBaseInfo(vpf))
                    End If
                Next
                Return list
            End Function

            Private Sub StoreVertHorzInfos()
                If Me.Viewport IsNot Nothing Then
                    _horizontalVps = GetAdditionalViewportsHorizontal(Me.Viewport)
                    _verticalVps = GetAdditionalViewportsVertically(Me.Viewport)
                Else
                    _horizontalVps.Clear()
                    _verticalVps.Clear()
                End If
            End Sub

        End Class

        Private Enum MouseSizeMoveViewportState
            None = 0
            UpDown = 1
            LeftRight = 2
        End Enum

    End Class


End Namespace
