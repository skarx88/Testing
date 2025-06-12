Imports System.Drawing
Imports System.Runtime.Serialization
Imports System.Windows

<Serializable>
<KnownType(GetType(List(Of ViewportSizeSetting)))>
<KnownType(GetType(ViewportSizeSetting))>
Public Class ViewPortSizeSettings

    <DataMember> Private _list As New List(Of ViewportSizeSetting)

    Public Sub New()
    End Sub

    Public Shared Function Load(s As System.IO.Stream) As ViewPortSizeSettings
        Return DataContractXmlFile.ReadObject(Of ViewPortSizeSettings)(s)
    End Function

    Public Sub SaveTo(s As System.IO.Stream)
        DataContractXmlFile.WriteObject(Me, s)
    End Sub

    Public Function AddNew(viewportIndex As Integer, location As System.Drawing.Point, size As System.Drawing.Size) As ViewportSizeSetting
        Dim percWidth As Integer = CInt(size.Width * 100 / size.Width)
        Dim percHeight As Integer = CInt(size.Height * 100 / size.Height)
        Dim percX As Integer = CInt(location.X * 100 / size.Width)
        Dim percY As Integer = CInt(location.Y * 100 / size.Height)
        Dim setting As New ViewportSizeSetting(viewportIndex, New System.Drawing.Point(percX, percY), New System.Drawing.Size(percWidth, percHeight))
        _list.Add(setting)
        Return setting
    End Function

    Public Sub Add(vp As ViewportSizeSetting)
        _list.Add(vp)
    End Sub

    Public Sub Clear()
        _list.Clear()
    End Sub

    ReadOnly Property Count As Integer
        Get
            Return _list.Count
        End Get
    End Property

    Public Sub CalculateForEach(totalViewportsCount As Integer, vpContainerControlBounds As Rectangle, action As Action(Of CalculatedViewportBoundsInfo))
        For Each vpS As Settings.ViewportSizeSetting In _list
            If vpS.Index <= totalViewportsCount - 1 Then
                Dim calcBounds As CalculatedViewportBoundsInfo = GetCalculatedViewportBoundsCore(vpS, vpContainerControlBounds)
                action.Invoke(calcBounds)
            End If
        Next
    End Sub

    Public Function GetCalculatedViewportBounds(viewportIndex As Integer, vpContainerControlBounds As Rectangle) As CalculatedViewportBoundsInfo
        Dim vps As ViewportSizeSetting = _list.Where(Function(vp) vp.Index = viewportIndex).FirstOrDefault
        If vps IsNot Nothing Then
            Return GetCalculatedViewportBoundsCore(vps, vpContainerControlBounds)
        End If
        Return Nothing
    End Function

    Private Function GetCalculatedViewportBoundsCore(vps As ViewportSizeSetting, vpContainerControlBounds As Rectangle) As CalculatedViewportBoundsInfo
        Dim currentWidth As Integer = CInt(vpContainerControlBounds.Width * vps.SizePercent.Width / 100)
        Dim currentHeight As Integer = CInt(vpContainerControlBounds.Height * vps.SizePercent.Height / 100)
        Dim cPosX As Integer = CInt(vpContainerControlBounds.Width * vps.Location.X / 100)
        Dim cPosY As Integer = CInt(vpContainerControlBounds.Size.Height * vps.Location.Y / 100)
        Dim info As New CalculatedViewportBoundsInfo(vps.Index, New System.Drawing.Size(currentWidth, currentHeight), New System.Drawing.Point(cPosX, cPosY))
        Return info
    End Function

    Public Class CalculatedViewportBoundsInfo

        Public Sub New(viewportIndex As Integer, size As System.Drawing.Size, location As System.Drawing.Point)
            Me.Index = viewportIndex
            Me.CalculatedViewportLocation = location
            Me.CalculatedViewportSize = size
        End Sub

        ReadOnly Property CalculatedViewportSize As System.Drawing.Size
        ReadOnly Property CalculatedViewportLocation As System.Drawing.Point

        ReadOnly Property Index As Integer
    End Class

End Class
