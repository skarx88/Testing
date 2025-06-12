<Reflection.Obfuscation(Feature:="renaming", ApplyToMembers:=False, Exclude:=True)> ' needed to avoid resource-file-names obfuscation errors, see issue #5
Friend Class SplashScreen
    Inherits Form

    Public Sub New()
        InitializeComponent()
        Init(False)
    End Sub

    Public Sub New(showCloseButton As Boolean)
        InitializeComponent()

        Me.btnClose.Visible = showCloseButton
        Me.lblMessage.Visible = Not showCloseButton

        Init(showCloseButton)
    End Sub

    Private Sub Init(showCloseButton As Boolean)
        Me.lblDescription.Text = My.Application.Info.Description
        Me.lblDisclaimer.Text = String.Format(SplashScreenStrings.Disclaimer_Msg, vbCrLf)
        Me.lblMessage.Text = SplashScreenStrings.Status_Msg
        Dim versionPrefix As String = String.Empty

        Dim isPreVersion As Boolean = False
        Dim productVersion As String = E3.Lib.DotNet.Expansions.Devices.My.Application.Info.ProductVersion.ToLower
        If productVersion.Contains("beta") Then
            isPreVersion = True
            versionPrefix = "BETA-"
        ElseIf productVersion.Contains("alpha") Then
            isPreVersion = True
            versionPrefix = "ALPHA-"
        ElseIf productVersion.Contains("dev") Then
            isPreVersion = True
            versionPrefix = "DEV.R-"
        ElseIf productVersion.Contains("debug") Then
            isPreVersion = True
            versionPrefix = "DEBUG-"
        End If

        If showCloseButton Then
            Me.lblBuild.Text = String.Format("{0}Version: {1}", versionPrefix, My.Application.Info.Version.ToString(4))
        Else
            Me.lblBuild.Text = String.Format("{0}Version: {1}", versionPrefix, My.Application.Info.Version.ToString(3))
        End If

        Using ms As New IO.MemoryStream(My.Resources.SplashScreenSvg)
            Dim xml_doc As XDocument = XDocument.Load(ms)
            For Each element As XElement In xml_doc.Root.Descendants
                If element.HasAttributes AndAlso element.Attribute("id") IsNot Nothing Then
                    If element.Attribute("id")?.Value = "VersionText" Then
                        If isPreVersion Then
                            element.Value = versionPrefix + element.Value.Split(" "c).First
                        Else
                            element.Value = String.Format("{0}{1}", Now.Year.ToString.First, My.Application.Info.Version.Major.ToString.PadLeft(3, "0"c))
                        End If
                        Exit For
                    End If
                End If
            Next

            Using ms2 As New System.IO.MemoryStream
                xml_doc.Save(ms2)
                ms2.Seek(0, IO.SeekOrigin.Begin)
                Using svgFile As E3.Lib.IO.Files.Svg.SvgFile = E3.Lib.IO.Files.Svg.SvgFile.Open(ms2, Nothing, [Lib].IO.Files.Svg.SvgRenderType.Skia)
                    Me.picLogo.Image = svgFile.Draw()
                End Using
            End Using
        End Using

    End Sub

    Public Sub SetMessageSuffix(suffix As String)
        Me.InvokeOrDefault(
        Sub()
            Me.lblMessage.Text = SplashScreenStrings.Status_Msg + vbCrLf + $"({suffix})"
        End Sub)
    End Sub

    Private Sub SplashScreen_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        If e.KeyCode = Keys.Escape Then
            Me.Close()
        End If
    End Sub

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Me.Close()
    End Sub

    Private Sub lblZuken_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles llblZuken.LinkClicked
        Me.llblZuken.Links(Me.llblZuken.Links.IndexOf(e.Link)).Visited = True

        System.Diagnostics.ProcessEx.Start(Me.llblZuken.Text)
    End Sub

    Friend WriteOnly Property TxtBorrowedLicense() As String
        Set(value As String)
            If InvokeRequired Then
                Invoke(Sub() Me.lblBorrowLic.Text = value)
            Else
                Me.lblBorrowLic.Text = value
            End If
        End Set
    End Property

End Class