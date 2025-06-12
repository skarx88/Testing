Imports Infragistics.Win.UltraWinListView
Imports Zuken.E3.HarnessAnalyzer.Project.Documents
Imports Zuken.E3.Lib.IO.Files

<Reflection.Obfuscation(Feature:="renaming", ApplyToMembers:=False, Exclude:=True)> ' needed to avoid resource-file-names obfuscation errors, see issue #5
Public Class DocumentFilterControl

    Public Event ItemCheckedChanged(sender As Object, e As ItemCheckedEventArgs)

    Private WithEvents _project As HarnessAnalyzerProject

    Public Sub New(project As HarnessAnalyzerProject)
        InitializeComponent()

        _project = project

        Me.ulvDrawings.ViewSettingsList.ImageSize = New Drawing.Size(0, 0)
    End Sub

    Property EventsEnabled As Boolean = True

    Public Sub AddDocument(document As HcvDocument, text As String)
        Dim documentItem As New DocumentItem(GetKey(document), text)
        Dim listItem As New UltraListViewItem(GetKey(document))

        listItem.CheckState = CheckState.Checked
        listItem.Tag = documentItem
        listItem.Value = text

        Me.ulvDrawings.Items.Add(listItem)
    End Sub

    Public Function RemoveDocument(document As HcvDocument) As Boolean
        Dim lvItem As UltraListViewItem = Nothing

        For Each item As UltraListViewItem In ulvDrawings.Items
            If TypeOf item.Tag Is DocumentItem Then
                If CType(item.Tag, DocumentItem).Key = GetKey(document) Then
                    lvItem = item
                    Exit For
                End If
            End If
        Next

        If lvItem IsNot Nothing Then
            Me.ulvDrawings.Items.Remove(lvItem)
            Return True
        End If

        Return False
    End Function

    Private Function GetKey(document As HcvDocument) As String
        Return document.Id.ToString
    End Function

    Public Sub SetCheckStateOfItem(document As HcvDocument, checked As Boolean)
        SetCheckStateOfItem(GetKey(document), checked)
    End Sub

    Public Sub SetCheckStateOfItem(documentId As String, checked As Boolean)
        Dim listItem As UltraListViewItem = If(Me.ulvDrawings.Items.Exists(documentId), Me.ulvDrawings.Items(documentId), Nothing)
        If (listItem IsNot Nothing) Then
            Me.ulvDrawings.Items(documentId).CheckState = If(checked, CheckState.Checked, CheckState.Unchecked)
        End If
    End Sub

    Private Sub ulvDrawings_ItemCheckStateChanged(sender As Object, e As ItemCheckStateChangedEventArgs) Handles ulvDrawings.ItemCheckStateChanged
        Dim docItem As DocumentItem = DirectCast(e.Item.Tag, DocumentItem)
        If (EventsEnabled) Then
            RaiseEvent ItemCheckedChanged(Me, New ItemCheckedEventArgs(docItem, e.Item.CheckState = CheckState.Checked))
        End If
    End Sub

    Private Sub _project_DocumentStateChanged(sender As Object, e As WorkStateResultFileEventArgs) Handles _project.DocumentStateChanged
        Dim document As HcvDocument = CType(e.File, HcvDocument)
        If Not document.IsOpen Then
            Dim listItem As UltraListViewItem = If(Me.ulvDrawings.Items.Exists(GetKey(document)), Me.ulvDrawings.Items(GetKey(document)), Nothing)
            If (listItem IsNot Nothing) Then
                Me.ulvDrawings.Items.Remove(listItem)
            End If
        End If
    End Sub

    Public Class DocumentItem
        Public Sub New(key As String, text As String)
            Me.Text = text
            Me.Key = key
        End Sub

        Property Text As String
        Property Key As String
    End Class

    Public Class ItemCheckedEventArgs
        Inherits EventArgs

        Private _item As DocumentItem
        Private _checked As Boolean

        Public Sub New(item As DocumentItem, checked As Boolean)
            MyBase.New()
            _item = item
            _checked = checked
        End Sub

        ReadOnly Property Checked As Boolean
            Get
                Return _checked
            End Get
        End Property

        ReadOnly Property Item As DocumentItem
            Get
                Return _item
            End Get
        End Property

    End Class

End Class