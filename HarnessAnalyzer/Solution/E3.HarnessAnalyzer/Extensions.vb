Imports System.ComponentModel
Imports System.IO
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports Infragistics.Win.Misc
Imports Infragistics.Win.UltraWinDataSource
Imports Infragistics.Win.UltraWinDock
Imports Infragistics.Win.UltraWinGrid
Imports Infragistics.Win.UltraWinTabControl
Imports Infragistics.Win.UltraWinTabs
Imports Infragistics.Win.UltraWinToolbars
Imports Infragistics.Win.UltraWinTree
Imports VectorDraw.Geometry
Imports VectorDraw.Professional.vdCollections
Imports VectorDraw.Professional.vdPrimaries
Imports Zuken.E3.HarnessAnalyzer.Checks.Cavities.Settings
Imports Zuken.E3.HarnessAnalyzer.QualityStamping
Imports Zuken.E3.HarnessAnalyzer.Settings
Imports Zuken.E3.HarnessAnalyzer.Settings.WeightSettings
Imports Zuken.E3.Lib.Converter.Unit
Imports Zuken.E3.Lib.IO.Files
Imports Zuken.E3.Lib.IO.Files.Hcv

<HideModuleName>
Friend Module Extensions

    <Extension>
    Public Sub ReRaiseSelectedTabItemChanged(tabControl As UltraTabControl)
        Dim tm As TabManager = Reflection.UtilsEx.GetPropertyValues(Of TabManager)(tabControl).SingleOrDefault
        If tm Is Nothing Then
            Throw New Exception($"Can't raise SelectedTabChanged-Event: coudn't find tabManager in tabControl ({tabControl.Name})")
        Else
            tm.RaiseEventReflection(NameOf(TabManager.SelectedTabItemChanging), New TabManager.SelectedTabItemChangingEventArgs(tabControl.SelectedTab))
            tm.RaiseEventReflection(NameOf(TabManager.SelectedTabItemChanged), New TabManager.SelectedTabItemChangedEventArgs(tabControl.SelectedTab, Nothing))
        End If
    End Sub

    <Extension>
    Public Sub ReRaiseActiveTabItemChanged(tabControl As UltraTabControl)
        Dim activeTabChangingEventArgs As New ActiveTabChangingEventArgs(tabControl.ActiveTab)
        Reflection.UtilsEx.TryInvokeMethod(tabControl, "RaiseEvent", {UltraTabControlEventId.ActiveTabChanging, activeTabChangingEventArgs})

        Dim activeTabChangedEventArgs As New ActiveTabChangedEventArgs(tabControl.ActiveTab, Nothing)
        Reflection.UtilsEx.TryInvokeMethod(tabControl, "RaiseEvent", {UltraTabControlEventId.ActiveTabChanged, activeTabChangedEventArgs})
    End Sub

    <Extension>
    Public Function AsFileContainer(ccResultInfo As CheckedCompareResultInformation) As [Lib].IO.Files.Hcv.CheckedCompareResultInfoContainerFile
        Select Case ccResultInfo.Type
            Case KnownContainerFileFlags.GCRI
                Return New [Lib].IO.Files.Hcv.GraphicalCheckedCompareResultInfoContainerFile(ccResultInfo.AsStream.ToArray)
            Case KnownContainerFileFlags.TCRI
                Return New [Lib].IO.Files.Hcv.TechnicalCheckedCompareResultInfoContainerFile(ccResultInfo.AsStream.ToArray)
            Case KnownContainerFileFlags.Unspecified
                Throw New NotSupportedException($"Creation of ""{NameOf(KnownContainerFileFlags.Unspecified)}"" file container not supported for ""{ccResultInfo.GetType.Name}""")
            Case Else
                Throw New NotImplementedException(NameOf(ccResultInfo) + ": " + ccResultInfo.ToString + " not implemented!")
        End Select
    End Function

    <Extension>
    Friend Sub EnsureVisible(row As UltraGridRow)
        row.Band.Layout.Grid.ActiveRowScrollRegion.ScrollRowIntoView(row)
    End Sub

    <Extension>
    Public Sub AutoSize(dockArea As DockAreaPane)
        Dim with_max As Integer = 0
        For Each pane As DockableControlPane In dockArea.Panes.OfType(Of DockableControlPane)
            Dim size As System.Drawing.Size = Nothing
            If pane.Control IsNot Nothing Then
                Dim control As Control = pane.Control
                control.SuspendLayout()
                SetControlAndChildsToAutoSize(control, True)
                control.ResumeLayout(True)
                size = control.Size

                For Each tree As UltraTree In control.GetAllChilds.OfType(Of UltraTree)
                    Dim width As Single = GetLongestVisibleNodeWidth(tree)
                    If width > size.Width Then
                        size = New Size(CInt(width), size.Height)
                    End If
                Next
            Else
                size = pane.Size
            End If

            If size.Width > with_max Then
                with_max = size.Width
            End If
        Next

        dockArea.Size = New Size(with_max, dockArea.Size.Height)
    End Sub

    Private Function GetLongestVisibleNodeWidth(tree As UltraTree) As Single
        Dim widthMax As Single = 0
        Using g As Graphics = tree.CreateGraphics
            For Each node As UltraTreeNode In GetAllVisibleNodes(tree.Nodes)
                Dim count As Integer = node.FullPath.Split(node.Control.PathSeparator).Length
                Dim size As SizeF = g.MeasureString(node.Text, tree.Font)

                Dim width As Single = size.Width + (count * (node.Control.LeftImagesSize.Width * (node.LeftImages.Count + 2)))
                If width > widthMax Then
                    widthMax = width
                End If
            Next
        End Using
        Return widthMax
    End Function


    Private Function GetAllVisibleNodes(nodes As TreeNodesCollection) As List(Of UltraTreeNode)
        Dim list As New List(Of UltraTreeNode)
        For Each node As UltraTreeNode In nodes
            If node.Visible Then
                list.Add(node)
                If node.HasVisibleNodes Then
                    list.AddRange(GetAllVisibleNodes(node.Nodes))
                End If
            End If
        Next
        Return list
    End Function

    Private Sub SetControlAndChildsToAutoSize(ctrl As Control, autoSize As Boolean)

        If TypeOf ctrl Is UserControl OrElse TypeOf ctrl Is Form Then
            If Not TypeOf ctrl Is UltraPanelClientArea Then
                ctrl.MaximumSize = New System.Drawing.Size(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height)
                ctrl.MinimumSize = New Size(ctrl.Width, ctrl.Height)
            End If

            If TypeOf ctrl Is UserControl Then
                CType(ctrl, UserControl).AutoSizeMode = AutoSizeMode.GrowAndShrink
                CType(ctrl, UserControl).AutoSize = autoSize
            End If

            If TypeOf ctrl Is Form Then
                CType(ctrl, Form).AutoSizeMode = AutoSizeMode.GrowAndShrink
                CType(ctrl, Form).AutoSize = autoSize
            End If
        End If

        For Each c As Control In ctrl.Controls
            SetControlAndChildsToAutoSize(c, autoSize)
        Next
    End Sub

    <Extension>
    Public Function TrySetCellHidden(row As Infragistics.Win.UltraWinGrid.UltraGridRow, key As String, hidden As Boolean) As Boolean
        If row.Cells.Exists(key) Then
            row.Cells(key).Hidden = hidden
            Return True
        End If
        Return False
    End Function

    <Extension>
    Public Function ToIds(modules As IEnumerable(Of E3.Lib.Schema.Kbl.Module)) As IEnumerable(Of String)
        Return modules.Select(Function(m) m.SystemId)
    End Function

    <Extension>
    Public Sub SetChecked(button As StateButtonTool, value As Boolean, Optional eventsEnabled As Boolean = True)
        If Not eventsEnabled Then button.ToolbarsManager.EventManager.SetEnabled(ToolbarEventIds.ToolClick, False)
        button.Checked = False
        If Not eventsEnabled Then button.ToolbarsManager.EventManager.SetEnabled(ToolbarEventIds.ToolClick, True)
    End Sub

    <Extension>
    Public Function RegExEmptyReplace(input As String, pattern As String) As String
        Return input.RegExReplace(pattern, String.Empty)
    End Function

    <Extension>
    Public Function TryAddOrGet(columns As UltraDataColumnsCollection, name As String) As UltraDataColumn
        If Not columns.Exists(name) Then
            Return columns.Add(name)
        End If
        Return columns(name)
    End Function

    <Extension>
    Public Function GetCompareInfoOrDefault(hcv As HcvFile) As AggregatedCheckedCompareResultInfoResult
        Dim ccList As New List(Of CheckedCompareResultInfoResult)
        Try
            For Each c As DataContainerFile In hcv
                If TypeOf c Is CheckedCompareResultInfoContainerFile Then
                    With CType(c, CheckedCompareResultInfoContainerFile)
                        If .HasData Then
                            Try
                                ccList.Add(New CheckedCompareResultInfoResult(.ToCompareInfo))
                            Catch ex As Exception
                                ccList.Add(New CheckedCompareResultInfoResult(ex))
                            End Try
                        End If
                    End With
                End If
            Next

            If ccList.Count = 0 Then
                Return AggregatedCheckedCompareResultInfoResult.Success
            Else
                Return New AggregatedCheckedCompareResultInfoResult(ccList)
            End If
        Catch ex As Exception
#If DEBUG Or CONFIG = "Debug" Then
            Throw
#Else
            Return AggregatedCheckedCompareResultInfoResult.Faulted(ex.Message)
#End If
        End Try
    End Function

    <Extension>
    Public Function ToCompareInfo(containerFile As CheckedCompareResultInfoContainerFile) As CheckedCompareResultInformation
        Using s As Stream = containerFile.GetDataAsStream
            Dim info As CheckedCompareResultInformation = CheckedCompareResultInformation.Load(s, containerFile.FullName)
            info.Type = CType(containerFile.Type, [Lib].IO.Files.Hcv.KnownContainerFileFlags)
            Return info
        End Using
    End Function

    <Extension>
    Public Function ToValidyCheck(containerFile As ValidtyCheckContainerFile) As ValidityCheckContainer
        Using s As Stream = containerFile.GetDataAsStream
            Return ValidityCheckContainer.Load(s)
        End Using
    End Function

    <Extension>
    Public Function ToRedliningInfoOrDefault(containerFile As RedliningInfoContainerFile) As RedliningInformation
        If (containerFile?.HasData).GetValueOrDefault Then
            Using s As Stream = containerFile.GetDataAsStream
                Try
                    Return RedliningInformation.Load(s, containerFile.FullName)
                Catch ex As Exception
#If DEBUG Or CONFIG = "Debug" Then
                    Throw
#Else
                    Return GetDefaultReliningInfo
#End If
                End Try
            End Using
        Else
            Return GetDefaultReliningInfo()
        End If
    End Function

    Private Function GetDefaultReliningInfo() As RedliningInformation
        Return New RedliningInformation With {
                    .CreatedBy = System.Security.Principal.WindowsIdentity.GetCurrent.Name,
                    .CreatedOn = Now,
                    .Name = RedliningInfoContainerFile.FILE_NAME
                }
    End Function

    <Extension>
    Public Function ToQmStampsOrDefault(containerFile As QmStampsContainerFile) As QMStamps
        If (containerFile?.HasData).GetValueOrDefault Then
            Using s As Stream = containerFile.GetDataAsStream
                Return QMStamps.LoadOrEmpty(s, containerFile.FullName)
            End Using
        Else
            Return New QMStamps With {
                        .CreatedBy = System.Security.Principal.WindowsIdentity.GetCurrent.Name,
                        .CreatedOn = Now,
                        .Name = QmStampsContainerFile.FILE_NAME
            }
        End If
    End Function

    <Extension>
    Public Function ToMemoListOrDefault(containerFile As MemolistContainerFile) As Memolist
        If (containerFile?.HasData).GetValueOrDefault Then
            Using s As Stream = containerFile.GetDataAsStream
                Return Memolist.Load(s, containerFile.FullName)
            End Using
        Else
            Return New Memolist With
            {.CreatedBy = System.Security.Principal.WindowsIdentity.GetCurrent.Name,
             .CreatedOn = Now,
             .Name = MemolistContainerFile.FILE_NAME
            }
        End If
    End Function

    <Extension>
    Public Function GetAsContainerFile(settings As CavityCheckSettings) As [Lib].IO.Files.Hcv.CavityCheckSettingsContainerFile
        Using ms As New MemoryStream
            settings.Save(ms)
            ms.Position = 0
            Return New Hcv.CavityCheckSettingsContainerFile(ms.ToArray)
        End Using
    End Function

    <Extension>
    Public Function GetAsContainerFile(mList As Memolist) As [Lib].IO.Files.Hcv.MemolistContainerFile
        Using ms As New MemoryStream
            mList.Save(ms)
            ms.Position = 0
            Return New Hcv.MemolistContainerFile(mList.Name, ms.ToArray)
        End Using
    End Function

    <Extension>
    Public Function GetAsContainerFile(redLInfo As RedliningInformation) As [Lib].IO.Files.Hcv.RedliningInfoContainerFile
        Using ms As New MemoryStream
            redLInfo.Save(ms)
            ms.Position = 0
            Return New Hcv.RedliningInfoContainerFile(redLInfo.Name, ms.ToArray)
        End Using
    End Function

    <Extension>
    Public Function GetAsContainerFile(qmStampInfo As QMStamps) As [Lib].IO.Files.Hcv.QmStampsContainerFile
        Using ms As New MemoryStream
            qmStampInfo.Save(ms)
            ms.Position = 0
            Return New Hcv.QmStampsContainerFile(qmStampInfo.Name, ms.ToArray)
        End Using
    End Function

    <Extension>
    Friend Function FindMaterialSpecsFor2(matSpecList As MaterialSpecList, generalWire As General_wire) As List(Of MaterialSpec)
        Dim lst As New List(Of MaterialSpec)

        If generalWire IsNot Nothing Then
            If Not String.IsNullOrEmpty(matSpecList.Settings.MaterialSpecField) Then
                Dim prop As PropertyInfo = generalWire.GetType.GetProperty(matSpecList.Settings.MaterialSpecField)
                If prop Is Nothing Then
                    Throw New MaterialFieldNotFoundException(String.Format("Material specification field ""{0}"" not found in part ""{1}""", matSpecList.Settings.MaterialSpecField, generalWire.GetType.Name), matSpecList.Settings.MaterialSpecField, generalWire, Nothing)
                End If

                Dim mSpecValue As Object = prop.GetValue(generalWire)
                If mSpecValue IsNot Nothing Then
                    For Each spec As MaterialSpec In matSpecList
                        Dim isMatch As Boolean
                        Try
                            isMatch = Regex.IsMatch(mSpecValue.ToString, spec.SpecRegEx)
                        Catch ex As ArgumentException
                            Throw New RegexException(ex.Message, spec.SpecRegEx, spec, ex)
                        End Try

                        If isMatch Then
                            lst.Add(spec)
                        End If
                    Next
                End If
            End If
        Else
            Throw New ArgumentException("Please define an occurrence or part to search the material specification for!", NameOf(generalWire))
        End If

        Return lst
    End Function

#Region "Collection-Extensions"

    Friend Enum AddOrUpdateResult
        Added
        OnlyUpdate
    End Enum

    <Extension>
    Friend Function AddOrUpdate(Of TKey, TValue, TList As IList)(dic As Dictionary(Of TKey, TList), key As TKey, value As TValue, Optional newFactory As Func(Of TList) = Nothing, Optional distinct As Boolean = False) As AddOrUpdateResult
        Dim res As AddOrUpdateResult = AddOrUpdateResult.OnlyUpdate
        If Not dic.ContainsKey(key) Then
            Dim newList As TList
            If newFactory IsNot Nothing Then
                newList = newFactory.Invoke()
            Else
                newList = Activator.CreateInstance(Of TList)()
            End If

            dic.Add(key, newList)
            res = AddOrUpdateResult.Added
        End If

        Dim list As TList = dic(key)
        If Not TypeOf value Is String AndAlso TypeOf value Is IEnumerable Then
            For Each item As Object In CType(value, IEnumerable)
                If Not distinct OrElse Not list.Contains(item) Then
                    list.Add(item)
                End If
            Next
        Else
            If Not distinct OrElse Not list.Contains(value) Then
                list.Add(value)
            End If
        End If

        Return res
    End Function

    <Extension>
    Friend Function AddOrUpdateGet(Of TKey, TValue, TList As IList)(dic As Dictionary(Of TKey, TList), key As TKey, value As TValue, Optional newFactory As Func(Of TList) = Nothing, Optional distinct As Boolean = False) As TList
        If Not dic.ContainsKey(key) Then
            Dim newList As TList
            If newFactory IsNot Nothing Then
                newList = newFactory.Invoke()
            Else
                newList = Activator.CreateInstance(Of TList)()
            End If

            dic.Add(key, newList)
        End If

        Dim list As TList = dic(key)
        If Not distinct OrElse Not list.Contains(value) Then
            list.Add(value)
        End If

        Return list
    End Function

    <Extension>
    Friend Function TryAddOrMergeValues(Of Tkey, TValue, TList As IList)(ByVal dic As Dictionary(Of Tkey, TList), key As Tkey, value As TValue) As Boolean
        If dic.ContainsKey(key) Then
            dic.AddOrUpdate(key, value, Nothing, True)
            Return True
        End If
        Return False
    End Function

    <Extension>
    Friend Function TryAdd(Of Tkey, TValue)(ByVal dic As Dictionary(Of Tkey, TValue), key As Tkey, value As TValue) As Boolean
        If dic.ContainsKey(key) Then
            dic.Add(key, value)
            Return True
        End If
        Return False
    End Function

    <Extension>
    Friend Function GetOrAddNew(Of Tkey, TValue As New)(dic As Dictionary(Of Tkey, TValue), key As Tkey) As TValue
        If Not dic.ContainsKey(key) Then
            Dim value As TValue = Activator.CreateInstance(Of TValue)()
            dic.Add(key, value)
            Return value
        Else
            Return dic(key)
        End If
    End Function

    <Extension>
    Friend Function AddOrGet(Of Tkey, TValue)(ByRef dic As Dictionary(Of Tkey, TValue), key As Tkey, value As TValue) As TValue
        If dic.TryAdd(key, value) Then
            Return value
        Else
            Return dic(key)
        End If
    End Function

    <Extension>
    Friend Function TryRemove(Of Tkey, TValue, TList As IList)(ByRef dic As Dictionary(Of Tkey, TList), key As Tkey, value As TValue) As Boolean
        If Not dic.ContainsKey(key) Then
            dic(key).Remove(value)
            If dic(key).Count = 0 Then
                dic.Remove(key)
            End If
            Return True
        End If
        Return False
    End Function

    <Extension>
    Friend Sub AddRange(Of T)(bList As BindingList(Of T), items As IEnumerable(Of T))
        For Each item As T In items
            bList.Add(item)
        Next
    End Sub

    <Extension>
    Public Sub ForEach(Of T)(numerable As IEnumerable(Of T), action As Action(Of T))
        For Each item As T In numerable
            action.Invoke(item)
        Next
    End Sub

    <Extension>
    Public Function RemoveItem(entities As vdEntities, obj As VectorDraw.Professional.vdPrimaries.vdFigure, removeHandle As Boolean) As Boolean
        If entities.RemoveItem(obj) Then
            If removeHandle AndAlso obj.Handle.Value <> 0 AndAlso entities.Document IsNot Nothing Then
                Dim mHandleTable As Dictionary(Of ULong, VectorDraw.Professional.vdObjects.vdObject) = CType(entities.Document.GetType.GetField("mHandleTable", BindingFlags.NonPublic Or BindingFlags.Instance).GetValue(entities.Document), Dictionary(Of ULong, VectorDraw.Professional.vdObjects.vdObject))
                SyncLock mHandleTable
                    If mHandleTable.ContainsKey(obj.Handle.Value) Then
                        Dim mHandleObj As VectorDraw.Professional.vdObjects.vdObject = Nothing
                        If mHandleTable.TryGetValue(obj.Handle.Value, mHandleObj) Then
                            mHandleTable.Remove(obj.Handle.Value)
                            obj.Handle.InternalSetValue(0)

                            Dim lastId As ULong = 0
                            If mHandleTable.Count > 0 Then
                                lastId = mHandleTable.Last.Key
                            End If
                            entities.Document.SetCurrentHandle(lastId)
                            SyncLock entities.Document.HandleTableGuid
                                entities.Document.HandleTableGuid.RemoveObjectGuid(mHandleObj)
                            End SyncLock
                        End If
                    End If
                End SyncLock
            End If
            Return True
        End If
        Return False
    End Function

#End Region

#Region "KBLObjectExtensions"

    <Extension>
    Friend Function GetLength(gWireOcc As General_wire_occurrence) As Numerical_value
        If TypeOf gWireOcc Is Wire_occurrence Then
            Return GetLength(DirectCast(gWireOcc, Wire_occurrence))
        ElseIf TypeOf gWireOcc Is Special_wire_occurrence Then
            Return GetLength(DirectCast(gWireOcc, Special_wire_occurrence))
        End If
        Return Nothing
    End Function

    <Extension>
    Friend Function GetLength(wireOcc As Wire_occurrence) As Numerical_value
        With My.Application.MainForm.GeneralSettings
            Return GetLengthInternal(wireOcc.Length_information, .DefaultWireLengthType)
        End With
        Return Nothing
    End Function

    <Extension>
    Friend Function GetLength(cableOcc As Special_wire_occurrence) As Numerical_value
        With My.Application.MainForm.GeneralSettings
            Return GetLengthInternal(cableOcc.Length_information, .DefaultCableLengthType)
        End With
        Return Nothing
    End Function

    <Extension>
    Friend Function GetLength(coreOcc As Core_occurrence) As Numerical_value
        With My.Application.MainForm.GeneralSettings
            Return GetLengthInternal(coreOcc.Length_information, .DefaultWireLengthType)
        End With
        Return Nothing
    End Function

    <Extension>
    Friend Function GetLength(coreOcc As IKblWireCoreOccurrence) As Numerical_value
        With My.Application.MainForm.GeneralSettings
            Return GetLengthInternal(coreOcc.Length_information, .DefaultWireLengthType)
        End With
        Return Nothing
    End Function

    Private Function GetLengthInternal(lengthInfos As Wire_length(), defaultLengthType As String) As Numerical_value
        For Each li As Wire_length In lengthInfos
            If li.Length_type.ToLower = defaultLengthType.ToLower Then
                Return li.Length_value
            End If
        Next
        Return Nothing
    End Function
    <Extension>
    Friend Function GetCable(coreOcc As Core_occurrence, kblMapper As KblMapper) As Special_wire_occurrence
        If kblMapper.KBLCoreCableMapper.ContainsKey(coreOcc.SystemId) Then
            Dim cableId As String = kblMapper.KBLCoreCableMapper(coreOcc.SystemId)
            Return CType(kblMapper.KBLOccurrenceMapper(cableId), Special_wire_occurrence)
        End If
        Return Nothing
    End Function
    <Extension>
    Friend Function GetGeneralWire(genOcc As General_wire_occurrence, kblMapper As KblMapper) As General_wire
        If kblMapper.KBLPartMapper.ContainsKey(genOcc.Part) Then
            Return CType(kblMapper.KBLPartMapper(genOcc.Part), General_wire)
        End If
        Return Nothing
    End Function
    <Extension>
    Friend Function GetCore(genOcc As Core_occurrence, kblMapper As KblMapper) As Core
        If kblMapper.KBLPartMapper.ContainsKey(genOcc.Part) Then
            Return CType(kblMapper.KBLPartMapper(genOcc.Part), Core)
        End If
        Return Nothing
    End Function
    <Extension>
    Friend Function IsKSL(connOcc As Connector_occurrence, kblMapper As KblMapper) As Boolean
        Dim part As Object = Nothing
        If kblMapper.KBLPartMapper.TryGetValue(connOcc.Part, part) AndAlso TypeOf part Is Connector_housing Then
            Return CType(part, Connector_housing).IsKSL
        End If
        Return False
    End Function

    ''' <summary>
    ''' Returns the weight average weight for a core from the cable weight
    ''' </summary>
    ''' <param name="genOcc"></param>
    ''' <param name="kblMapper"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Extension>
    Friend Function GetAverageWeight(genOcc As Core_occurrence, kblMapper As KblMapper) As WeightNumericValue
        Dim cable As Special_wire_occurrence = genOcc.GetCable(kblMapper)
        Dim genWire As General_wire = cable.GetGeneralWire(kblMapper)
        If genWire.Mass_information IsNot Nothing Then
            Dim inputUnit As Unit = Nothing
            If kblMapper.KBLUnitMapper.TryGetValue(genWire.Mass_information.Unit_component, inputUnit) Then
                Dim pair As CalcUnitPair = inputUnit.ToCalcUnitPair
                Return New WeightNumericValue() With {.LengthUnit = pair.Second, .Unit = pair.First, .Value = genWire.Mass_information.Value_component / cable.Core_occurrence.Length}
            End If
        End If
        Return New WeightNumericValue With {.Value = 0, .Unit = CalcUnit.Gram}
    End Function

    <Extension>
    Friend Function GetWeight(wireOrCable As General_wire_occurrence, kblMapper As KblMapper) As WeightNumericValue
        Dim genWire As General_wire = wireOrCable.GetGeneralWire(kblMapper)
        If genWire.Mass_information IsNot Nothing Then
            Dim unit As Unit = kblMapper.GetUnit(genWire.Mass_information.Unit_component)
            If unit IsNot Nothing Then
                Dim pair As CalcUnitPair = unit.ToCalcUnitPair
                Return New WeightNumericValue With {.LengthUnit = pair.Second, .Unit = pair.First, .Value = genWire.Mass_information.Value_component}
            End If
        End If
        Return New WeightNumericValue With {.Value = 0, .Unit = CalcUnit.Gram}
    End Function

    <Extension>
    Friend Function GetWiresAndCores(seg As Segment, kblMapper As KblMapper) As List(Of IKblWireCoreOccurrence)
        Dim list As New List(Of IKblWireCoreOccurrence)
        Dim wireOrCoreIds As List(Of String) = Nothing
        If kblMapper.KBLSegmentWireMapper.TryGetValue(seg.SystemId, wireOrCoreIds) Then
            For Each id As String In wireOrCoreIds
                Dim wireOrCore As IKblWireCoreOccurrence = kblMapper.GetWireOrCoreOccurrence(id)
                If wireOrCore IsNot Nothing Then
                    list.Add(wireOrCore)
                End If
            Next
        End If
        Return list
    End Function

    <Extension>
    Friend Function IsTableOrRedlining(grp As VdSVGGroup) As Boolean
        Return grp.SVGType = SvgType.table.ToString OrElse grp.SVGType = SvgType.row.ToString OrElse grp.SVGType = SvgType.cell.ToString OrElse grp.SVGType = SvgType.ref.ToString OrElse (grp.SVGType = SvgType.dimension.ToString) OrElse grp.SymbolType = SvgSymbolType.Redlining.ToString
    End Function

    <Extension>
    Friend Function Extract(numValue As Numerical_value, kblMapper As KblMapper) As NumericValue
        If numValue IsNot Nothing AndAlso kblMapper.KBLUnitMapper IsNot Nothing AndAlso kblMapper.KBLUnitMapper.ContainsKey(numValue.Unit_component) Then
            Dim inputUnit As Unit = Nothing
            If kblMapper.KBLUnitMapper.TryGetValue(numValue.Unit_component, inputUnit) Then
                Dim pair As CalcUnitPair = inputUnit.ToCalcUnitPair
                Return New NumericValue() With {.Unit = pair.First, .Value = numValue.Value_component}
            End If
        End If
        Return Nothing
    End Function

    <Extension>
    Friend Function ToLogEventArgs(result As IResult, Optional fileName As String = "") As List(Of LogEventArgs)
        Dim lst As New List(Of LogEventArgs)
        If TypeOf result Is IAggregatedResult Then
            For Each res As IResult In CType(result, IAggregatedResult).Results
                lst.AddRange(res.ToLogEventArgs(fileName))
            Next
        ElseIf result.IsFaulted AndAlso TypeOf result Is [Lib].IO.Files.Dsi.ImportedObjectNameResult Then
            Dim objNameRes As [Lib].IO.Files.Dsi.ImportedObjectNameResult = CType(result, [Lib].IO.Files.Dsi.ImportedObjectNameResult)
            Dim msg As String = objNameRes.ErrorCode.AsMessage(objNameRes.Name, fileName)
            If msg Is Nothing Then
                msg = objNameRes.Message
            End If

            If [Lib].IO.Files.Dsi.HasOnlyWarnings(objNameRes) Then
                lst.Add(New LogEventArgs(LogEventArgs.LoggingLevel.Warning, msg))
            Else
                lst.Add(New LogEventArgs(LogEventArgs.LoggingLevel.Error, msg))
            End If
        ElseIf result.IsFaulted Then
            lst.Add(New LogEventArgs(LogEventArgs.LoggingLevel.Error, result.Message))
        ElseIf result.IsCancelled Then
            lst.Add(New LogEventArgs(LogEventArgs.LoggingLevel.Warning, result.Message))
        ElseIf Not String.IsNullOrWhiteSpace(result.Message) Then
            lst.Add(New LogEventArgs(LogEventArgs.LoggingLevel.Information, result.Message))
        End If
        Return lst
    End Function

    <Extension>
    Friend Function AsMessage(code As Dsi.DsiImportErrorCode, objName As String, dsiFileName As String) As String
        Select Case code
            Case Dsi.DsiImportErrorCode.ErrorImportCables
                Return String.Format(ErrorStrings.DSIImport_ErrorImportCable, objName, dsiFileName)
            Case Dsi.DsiImportErrorCode.ErrorImportComponent
                Return String.Format(ErrorStrings.DSIImport_ErrorImportComponent, objName, dsiFileName)
            Case Dsi.DsiImportErrorCode.ErrorImportHarness
                Return String.Format(ErrorStrings.DSIImport_ErrorHarnessOrUnsupportedVersion_Msg, objName, dsiFileName)
            Case Dsi.DsiImportErrorCode.ErrorImportRouting
                Return String.Format(ErrorStrings.DSIImport_ErrorImportRouting, objName, dsiFileName)
            Case Dsi.DsiImportErrorCode.ErrorImportSegment
                Return String.Format(ErrorStrings.DSIImport_ErrorImportSegment, objName, dsiFileName)
            Case Dsi.DsiImportErrorCode.ErrorImportWire
                Return String.Format(ErrorStrings.DSIImport_ErrorImportWire, objName, dsiFileName)
            Case Dsi.DsiImportErrorCode.ErrorImportWireProtection
                Return String.Format(ErrorStrings.DSIImport_ErrorImportWireProtection, objName, dsiFileName)
            Case Dsi.DsiImportErrorCode.ErrorVersionNotSupported
                Return String.Format(ErrorStrings.DSIImport_ErrorHarnessOrUnsupportedVersion_Msg, objName, dsiFileName)
            Case Dsi.DsiImportErrorCode.WarningMissingConnectorReference
                Return String.Format(ErrorStrings.DSIImport_WarningMissingConnectorReference, objName, dsiFileName)
            Case Dsi.DsiImportErrorCode.WarningMissingEndNode
                Return String.Format(ErrorStrings.DSIImport_WarningMissingEndNode, objName, dsiFileName)
            Case Dsi.DsiImportErrorCode.WarningMissingNodeReference
                Return String.Format(ErrorStrings.DSIImport_WarningMissingNodeReference, objName, dsiFileName)
            Case Dsi.DsiImportErrorCode.Undefined
                Return Nothing
            Case Else
                Throw New NotImplementedException($"Error code ""{code.ToString}"" not implemented!")
        End Select
    End Function


#End Region

#Region "EnumExtensions"
    <System.Runtime.CompilerServices.Extension>
    Public Function GetFlags(value As [Enum]) As IEnumerable(Of [Enum])
        Return GetFlags(value, [Enum].GetValues(value.[GetType]()).Cast(Of [Enum])().ToArray())
    End Function

    ''' <summary>
    ''' gets all the individual flags for a type. So values containing multiple bits are left out.
    ''' </summary>
    ''' <param name="value"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <System.Runtime.CompilerServices.Extension>
    Public Function GetIndividualFlags(value As [Enum]) As IEnumerable(Of [Enum])
        Return GetFlags(value, GetFlagValues(value.[GetType]()).ToArray())
    End Function

    Private Function GetFlags(value As [Enum], values As [Enum]()) As IEnumerable(Of [Enum])
        Dim bits As ULong = Convert.ToUInt64(value)
        Dim results As New List(Of [Enum])()
        For i As Integer = values.Length - 1 To 0 Step -1
            Dim mask As ULong = Convert.ToUInt64(values(i))
            If i = 0 AndAlso mask = 0L Then
                Exit For
            End If
            If (bits And mask) = mask Then
                results.Add(values(i))
                bits -= mask
            End If
        Next
        If bits <> 0L Then
            Return Enumerable.Empty(Of [Enum])()
        End If
        If Convert.ToUInt64(value) <> 0L Then
            results.Reverse()
            Return results
        End If
        If bits = Convert.ToUInt64(value) AndAlso values.Length > 0 AndAlso Convert.ToUInt64(values(0)) = 0L Then
            Return values.Take(1)
        End If
        Return Enumerable.Empty(Of [Enum])()
    End Function

    Private Function GetFlagValues(enumType As Type) As IEnumerable(Of [Enum])
        Dim flag As ULong = &H1
        Dim list As New List(Of [Enum])
        For Each value As [Enum] In [Enum].GetValues(enumType).Cast(Of [Enum])()
            Dim bits As ULong = Convert.ToUInt64(value)
            If bits = 0L Then
                'yield return value;
                Continue For
            End If
            ' skip the zero value
            While flag < bits
                flag <<= 1
            End While
            If flag = bits Then
                list.Add(value)
            End If
        Next
        Return list
    End Function

#End Region

#Region "vdObjectExtensions"

    <Extension>
    Public Function GetRootGroup(group As VdSVGGroup) As VdSVGGroup
        If (group.ParentGroup Is Nothing) Then
            Return group
        Else
            Return GetRootGroup(group.ParentGroup)
        End If
    End Function


    <Extension>
    Public Function ToVdSelection(vdObjects As IEnumerable(Of vdFigure), Optional itemCheck As vdSelection.AddItemCheck = vdSelection.AddItemCheck.Nochecking) As vdSelection
        Dim entities As New vdEntities
        For Each item As vdFigure In vdObjects
            entities.Add(item)
        Next

        Dim selection As New vdSelection()
        selection.AddRange(entities, itemCheck)
        Return selection
    End Function

    <Extension>
    Public Function GetIdsIfIssue(group As vdFigure, Optional returnNothingWhenEmpty As Boolean = False) As String()
        If TypeOf group Is IssueReporting.VdIssue Then
            Return New String() {CType(group, IssueReporting.VdIssue).Issue.Id}
        End If

        If returnNothingWhenEmpty Then
            Return Nothing
        End If

        Return Array.Empty(Of String)
    End Function

    <Extension>
    Public Function GetIdsIfStamp(group As vdFigure, Optional returnNothingWhenEmpty As Boolean = False) As String()
        If TypeOf group Is QualityStamping.VdStamp Then
            Return New String() {CType(group, QualityStamping.VdStamp).Reference.List.Owner.Id}
        End If

        If returnNothingWhenEmpty Then
            Return Nothing
        End If

        Return Array.Empty(Of String)
    End Function

#End Region

#Region "ImageExtensions"
    <Extension>
    Public Sub DrawOverlay(ByRef img As System.Drawing.Image, pt As POINT, overlay As System.Drawing.Image)
        DrawOverlay(img, New PointF(CSng(pt.X), CSng(pt.Y)), overlay)
    End Sub

    <Extension>
    Public Sub DrawOverlay(ByRef img As System.Drawing.Image, pt As PointF, overlay As System.Drawing.Image)
        Dim imgClone As System.Drawing.Image = CType(img.Clone, Image)
        Using g As Graphics = Graphics.FromImage(imgClone)
            g.DrawImage(overlay, pt)
        End Using
        img = imgClone
    End Sub
#End Region



End Module
