Imports System.Text

Public Class SvgTableCompareEntry

    Private _childs As List(Of SvgTableCompareEntry)
    Private _group As VdSVGGroup
    Private _hasMatchEntry As Boolean
    Private _level As Integer
    Private _parent As SvgTableCompareEntry
    Private _texts As List(Of VdTextEx)

    Public Sub New(group As VdSVGGroup, level As Integer, parent As SvgTableCompareEntry)
        _childs = New List(Of SvgTableCompareEntry)
        _texts = New List(Of VdTextEx)
        _group = group
        _level = level
        _parent = parent
    End Sub

    Public Function GetMatchString() As String
        Dim matchString As New StringBuilder

        For Each text As VdTextEx In _texts
            If (text.TextString <> String.Empty) Then
                matchString.Append(text.TextString.Trim)
            End If
        Next

        Return matchString.ToString
    End Function

    Public Function GetMinXCoordinate() As Double
        Return GetMinXCoordinate_Recursively(Me)
    End Function

    Public Function GetCompleteMatchString() As String
        Dim completeMatchString As New StringBuilder

        Dim matchString As String = GetMatchString()
        If (matchString <> String.Empty) Then
            completeMatchString.Append(matchString)
        End If

        For Each child As SvgTableCompareEntry In _childs
            matchString = child.GetCompleteMatchString
            If (matchString <> String.Empty) Then
                completeMatchString.Append(matchString)
            End If
        Next

        Return completeMatchString.ToString
    End Function


    Private Function GetMinXCoordinate_Recursively(tableCompareEntry As SvgTableCompareEntry) As Double
        If (tableCompareEntry.Parent IsNot Nothing) Then
            GetMinXCoordinate_Recursively(tableCompareEntry.Parent)
        Else
            Return tableCompareEntry.Group.ChildGroups.GetBoundingBox(True, True).Left
        End If

        Return 0
    End Function

    Public ReadOnly Property Childs() As List(Of SvgTableCompareEntry)
        Get
            Return _childs
        End Get
    End Property

    Public ReadOnly Property Group() As VdSVGGroup
        Get
            Return _group
        End Get
    End Property

    Public Property HasMatchEntry() As Boolean
        Get
            Return _hasMatchEntry
        End Get
        Set(value As Boolean)
            _hasMatchEntry = value
        End Set
    End Property

    Public ReadOnly Property Level() As Integer
        Get
            Return _level
        End Get
    End Property

    Public ReadOnly Property Parent() As SvgTableCompareEntry
        Get
            Return _parent
        End Get
    End Property

    Public ReadOnly Property Texts() As List(Of VdTextEx)
        Get
            Return _texts
        End Get
    End Property

End Class