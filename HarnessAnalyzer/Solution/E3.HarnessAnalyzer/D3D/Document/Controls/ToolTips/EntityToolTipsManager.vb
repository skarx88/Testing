Imports devDept.Eyeshot.Entities
Imports Zuken.E3.HarnessAnalyzer.D3D.Document.Controls


Public Class EntityToolTipsManager
    Inherits ToolTipControlExManager

    Public Event ToolTipSelectionChanged(sender As Object, e As ToolTipSelectionChangedEventArgs)

    Public Sub New(hostControl As ContainerControl)
        MyBase.New(hostControl)
    End Sub

    Public Shadows Sub Show(entity As IEntity, model As E3.Lib.Model.IEEModel, contextMenu As ContextMenuStrip, Optional size As Nullable(Of System.Drawing.Size) = Nothing, Optional position As Nullable(Of System.Drawing.Point) = Nothing, Optional delayed As Boolean = False)
        MyBase.Show(Nothing, contextMenu, size, position, delayed, New EntityData(entity, model))
    End Sub

    Public Shadows Sub Hide(entity As IEntity, Optional delayed As Boolean = False, Optional forceHidePinned As Boolean = False)
        MyBase.Hide(Nothing, delayed, forceHidePinned, New EntityData(entity, Nothing))
    End Sub

    Public Shadows Sub Close(entity As IEntity, Optional delayed As Boolean = False, Optional forceClosePinned As Boolean = False)
        MyBase.Close(Nothing, delayed, forceClosePinned, New EntityData(entity, Nothing))
    End Sub

    Protected Overrides Function IsEqual(compare As ICompareControlsData, userData As Object) As Boolean
        Dim data1 As EntityData = TryCast(userData, EntityData)
        Dim ttControl As ToolTips.EntityToolTipControl = TryCast(compare.Control1, ToolTips.EntityToolTipControl)

        If ttControl Is Nothing Then
            ttControl = TryCast(compare.Control2, ToolTips.EntityToolTipControl)
        End If

        If ttControl Is Nothing AndAlso TypeOf compare Is ICreatingData Then
            Dim data2 As EntityData = TryCast(CType(compare, ICreatingData).UserData, EntityData)
            If data2 IsNot Nothing Then
                Return data2.Entity Is data1.Entity
            End If
        End If

        If ttControl IsNot Nothing AndAlso ttControl.Entity Is data1.Entity Then
            Return True
        End If

        Return False
    End Function

    Protected Overrides Function NewToolTip(info As DockingInfo, control As Control, Optional size As Size? = Nothing, Optional autoCloseType As HideCloseType = HideCloseType.Close, Optional userData As Object = Nothing) As ToolTipControlEx
        Dim data As EntityData = CType(userData, EntityData)
        Dim toolTipControl As ToolTips.EntityToolTipControl = CreateNewPopUpControl(data.Entity, data.Model)
        Return MyBase.NewToolTip(info, toolTipControl, If(ToolTipSize.HasValue, ToolTipSize.Value, size), autoCloseType, userData)
    End Function

    Protected Overrides Sub OnHidden(e As ToolTipExEventArgs)
        If TypeOf e.ToolTip.Control Is ToolTips.EntityToolTipControl Then
            DestroyPopUpControl(CType(e.ToolTip.Control, ToolTips.EntityToolTipControl))
        End If

        MyBase.OnHidden(e)
    End Sub

    Protected Overrides Sub OnBeforeShown(e As CancelToolTipExInfoEventArgs)
        If TypeOf e.ToolTip.Control Is ToolTips.EntityToolTipControl Then
            e.ToolTip.Caption = CType(e.ToolTip.Control, ToolTips.EntityToolTipControl).Text
        End If

        MyBase.OnBeforeShown(e)
    End Sub

    Protected Overrides Function IsToolTipControlUnderMouseCursor(control As Control, userData As Object) As Boolean
        Dim entity As IEntity = Nothing
        If (TypeOf (MyBase.HostControl) Is Document3DControl) Then
            With CType(MyBase.HostControl, Document3DControl)
                entity = .Model3DControl1.Design.GetEntityUnderMouseCursor
            End With
        ElseIf (TypeOf (MyBase.HostControl) Is D3D.D3DComparerCntrl) Then
            With CType(MyBase.HostControl, D3D.D3DComparerCntrl)
                entity = .Design3D.GetEntityUnderMouseCursor
            End With
        End If
        If entity Is Nothing OrElse control Is Nothing OrElse entity IsNot CType(userData, EntityData).Entity Then
            Return False
        End If

        Return True
    End Function

    Private Function CreateNewPopUpControl(entity As IEntity, model As E3.Lib.Model.IEEModel) As ToolTips.EntityToolTipControl
        Static i As Integer = 0
        Me.HostControl.SuspendLayout()
        Dim popUpControl As New ToolTips.EntityToolTipControl()

        i += 1
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Document3DControl))
        resources.ApplyResources(popUpControl, "PopUpControl1")
        popUpControl.Name = "PopUpControl" + i.ToString
        HostControl.Controls.Add(popUpControl)
        HostControl.ResumeLayout(False)

        AddPopUpControlEvents(popUpControl)

        popUpControl.CreateControl()
        If entity IsNot Nothing Then
            popUpControl.SetEntity(entity, model)
        End If
        Return popUpControl
    End Function

    Private Sub AddPopUpControlEvents(c As ToolTips.EntityToolTipControl)
        AddHandler c.SelectionChanged, AddressOf PopUpControl1_SelectionChanged
    End Sub

    Private Sub RemovePopUpControlEvents(c As ToolTips.EntityToolTipControl)
        RemoveHandler c.SelectionChanged, AddressOf PopUpControl1_SelectionChanged
    End Sub

    Private Sub DestroyPopUpControl(c As ToolTips.EntityToolTipControl)
        RemovePopUpControlEvents(c)
        c.Dispose()
    End Sub

    Private Sub PopUpControl1_SelectionChanged(ByVal sender As Object, ByVal e As ToolTipSelectionChangedEventArgs)
        RaiseEvent ToolTipSelectionChanged(Me, e)
    End Sub

    Property ToolTipSize As Nullable(Of System.Drawing.Size)
        Get
            If Not My.Settings.DocumentToolTipSize.IsEmpty Then
                Return My.Settings.DocumentToolTipSize
            End If
            Return Nothing
        End Get
        Set(value As Nullable(Of System.Drawing.Size))
            My.Settings.DocumentToolTipSize = value.GetValueOrDefault
        End Set
    End Property

    Protected Overrides Sub OnSizeChanged(e As ToolTipExEventArgs)
        If e.ToolTip.IsVisible Then
            ToolTipSize = e.ToolTip.Size
        End If
        MyBase.OnSizeChanged(e)
    End Sub

    Private Class EntityData
        Public Sub New(entity As IEntity, model As E3.Lib.Model.IEEModel)
            Me.Entity = entity
            Me.Model = model
        End Sub

        Property Entity As IEntity
        Property Model As E3.Lib.Model.IEEModel
    End Class

    Private Class DoubleEntityData
        Inherits EntityData

        Public Sub New(entity1 As IEntity, entity2 As IEntity, Optional model As E3.Lib.Model.IEEModel = Nothing)
            MyBase.New(entity1, model)
            Me.Entity2 = entity2
        End Sub

        ReadOnly Property Entity2 As IEntity
    End Class


End Class
